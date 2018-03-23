#r "paket: groupref netcorebuild //"
#load ".fake/build.fsx/intellisense.fsx"

open System
open System.IO
open System.Text.RegularExpressions
open Fake.Core
open Fake.Core.TargetOperators
open Fake.DotNet
open Fake.IO
open Fake.IO.Globbing.Operators
open Fake.IO.FileSystemOperators

#if MONO
// prevent incorrect output encoding (e.g. https://github.com/fsharp/FAKE/issues/1196)
System.Console.OutputEncoding <- System.Text.Encoding.UTF8
#endif

let jsLibsOutput = "src" </> "WebApp" </> "public" </> "libs"
let testsDist = "tests" </> "dist"

let yarn args =
    let code =
        Process.Exec
            (fun info ->
                { info with
                    FileName = "yarn"
                    Arguments = args
                }
            )
            (TimeSpan.FromMinutes 10.)
    if code <> 0 then
        failwithf "Yarn exited with code: %i" code
    else
        ()

Target.Create "Clean" (fun _ ->
    !! "src/**/bin"
    ++ "src/**/obj"
    ++ "tests/**/bin"
    ++ "tests/**/obj"
    ++ jsLibsOutput
    ++ testsDist
    |> Shell.CleanDirs
)

Target.Create "YarnInstall" (fun _ ->
    yarn "install"
)

Target.Create "Restore" (fun _ ->
    !! "src/**/*.fsproj"
    ++ "tests/Tests.fsproj"
    |> Seq.iter (fun file -> DotNet.Restore id file)
)

Target.Create "Build" (fun _ ->
    let proc = DotNet.Exec (fun p ->
                                { p with WorkingDirectory = "src/WebApp" } ) "fable" "webpack --port free -- -p"
    if proc.ExitCode <> 0 then
        failwithf "Dotnet existed with code: %i\n" proc.ExitCode
)

Target.Create "Watch" (fun _ ->
    let proc = DotNet.Exec (fun p ->
                                { p with WorkingDirectory = "src/WebApp" } ) "fable" "webpack-dev-server --port free --"
    if proc.ExitCode <> 0 then
        failwithf "Dotnet existed with code: %i\n" proc.ExitCode
)

Target.Create "CopyMonacoModules" (fun _ ->
    let requireJsOutput = jsLibsOutput </> "requirejs"
    let vsOutput = jsLibsOutput </> "vs"
    Directory.create requireJsOutput
    Directory.create vsOutput
    Shell.cp ("./node_modules" </> "requirejs" </> "require.js") requireJsOutput
    Shell.cp_r ("./node_modules" </> "monaco-editor" </> "min" </> "vs") vsOutput
)

Target.Create "CopyQUnitModules" (fun _ ->
    Directory.create testsDist
    Shell.cp_r ("./node_modules" </> "qunitjs" </> "qunit") (testsDist </> "qunit")
)

Target.Create "RunLiveTests" (fun _ ->
    let proc = DotNet.Exec (fun p ->
                                { p with WorkingDirectory = "tests" } ) "fable" "yarn-run webpack-dev-server --port free -- --config tests/webpack.config.js"

    if proc.ExitCode <> 0 then
        failwithf "Dotnet existed with code: %i\n" proc.ExitCode
)

Target.Create "BuildTests" (fun _ ->
    let proc = DotNet.Exec (fun p ->
                                { p with WorkingDirectory = "tests" } ) "fable" "yarn-run webpack --port free -- -p --config tests/webpack.config.js"

    if proc.ExitCode <> 0 then
        failwithf "Dotnet existed with code: %i\n" proc.ExitCode
)

Target.Create "RunTests" (fun _ ->
    yarn "run qunit tests/dist/bundle.js"
)

Target.Create "Setup" Target.DoNothing

Target.Create "Release" Target.DoNothing

"Clean"
    ==> "YarnInstall"
    ==> "Restore"
    ==> "CopyMonacoModules"
    ==> "Setup"

"Setup"
    ==> "CopyQUnitModules"
    ==> "RunLiveTests"

"Setup"
    ==> "Watch"

// A "Release" include a "Setup"
"Setup"
    ==> "Release"

"Setup"
    ==> "BuildTests"
    ==> "RunTests"

// A "Release" include a "RunTests"
"RunTests"
    ==> "Release"

// Start build
Target.RunOrDefault "Build"
