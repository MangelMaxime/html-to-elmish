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
open Fake.JavaScript

#if MONO
// prevent incorrect output encoding (e.g. https://github.com/fsharp/FAKE/issues/1196)
System.Console.OutputEncoding <- System.Text.Encoding.UTF8
#endif

let jsLibsOutput = "src" </> "WebApp" </> "public" </> "libs"
let testsDist = "tests" </> "dist"

Target.create "Clean" (fun _ ->
    !! "src/**/bin"
    ++ "src/**/obj"
    ++ "tests/**/bin"
    ++ "tests/**/obj"
    ++ "src/WebApp/output"
    ++ jsLibsOutput
    ++ testsDist
    |> Shell.CleanDirs
)

Target.create "YarnInstall" (fun _ ->
    Yarn.install id
)

Target.create "Restore" (fun _ ->
    !! "src/**/*.fsproj"
    ++ "tests/Tests.fsproj"
    |> Seq.iter (fun file -> DotNet.restore id file)
)

Target.create "Build" (fun _ ->
    let proc = DotNet.exec (fun p ->
                                { p with WorkingDirectory = "src/WebApp" } ) "fable" "webpack --port free -- -p"
    if proc.ExitCode <> 0 then
        failwithf "Dotnet existed with code: %i\n" proc.ExitCode
)

Target.create "Watch" (fun _ ->
    let proc = DotNet.exec (fun p ->
                                { p with WorkingDirectory = "src/WebApp" } ) "fable" "webpack-dev-server --port free --"
    if proc.ExitCode <> 0 then
        failwithf "Dotnet existed with code: %i\n" proc.ExitCode
)

Target.create "CopyMonacoModules" (fun _ ->
    let requireJsOutput = jsLibsOutput </> "requirejs"
    let vsOutput = jsLibsOutput </> "vs"
    Directory.create requireJsOutput
    Directory.create vsOutput
    Shell.cp ("./node_modules" </> "requirejs" </> "require.js") requireJsOutput
    Shell.cp_r ("./node_modules" </> "monaco-editor" </> "min" </> "vs") vsOutput
)

Target.create "CopyQUnitModules" (fun _ ->
    Directory.create testsDist
    Shell.cp_r ("./node_modules" </> "qunitjs" </> "qunit") (testsDist </> "qunit")
)

Target.create "RunLiveTests" (fun _ ->
    let proc = DotNet.exec (fun p ->
                                { p with WorkingDirectory = "tests" } ) "fable" "yarn-run webpack-dev-server --port free -- --config tests/webpack.config.js"

    if proc.ExitCode <> 0 then
        failwithf "Dotnet existed with code: %i\n" proc.ExitCode
)

Target.create "BuildTests" (fun _ ->
    let proc = DotNet.exec (fun p ->
                                { p with WorkingDirectory = "tests" } ) "fable" "yarn-run webpack --port free -- -p --config tests/webpack.config.js"

    if proc.ExitCode <> 0 then
        failwithf "Dotnet existed with code: %i\n" proc.ExitCode
)

Target.create "RunTests" (fun _ ->
    Yarn.exec "run qunit tests/dist/bundle.js" id
)

Target.create "Release" (fun _ ->
    Yarn.exec "run gh-pages --dist src/WebApp/output" id
)

Target.create "Setup" Target.DoNothing

Target.create "CI" Target.DoNothing

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

// A "Build" include a "Setup"
"Setup"
    ==> "Build"

"Setup"
    ==> "BuildTests"
    ==> "RunTests"

// A "Release" include a "RunTests"
"RunTests"
    ==> "Release"

"Build"
    ==> "Release"

"CI"
    <== [ "Build"
          "RunTests"]

// Start build
Target.runOrDefault "Build"
