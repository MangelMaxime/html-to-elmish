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

let sourceFile = "src/HtmlToElmish.fsproj"

let jsLibsOutput = "public" </> "libs"

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
    !! "**/bin"
    ++ "**/obj"
    ++ jsLibsOutput
    |> Shell.CleanDirs
)

Target.Create "YarnInstall" (fun _ ->
    yarn "install"
)

Target.Create "Restore" (fun _ ->
    DotNet.Restore id sourceFile
)

Target.Create "Build" (fun _ ->
    let proc = DotNet.Exec (fun p ->
                                { p with WorkingDirectory = "src" } ) "fable" "webpack --port free -- -p"
    if proc.ExitCode <> 1 then
        failwithf "Dotnet existed with code: %i\n Message: \n %A" proc.ExitCode proc.Messages
)

Target.Create "Watch" (fun _ ->
    let proc = DotNet.Exec (fun p ->
                                { p with WorkingDirectory = "src" } ) "fable" "webpack-dev-server --port free"
    if proc.ExitCode <> 1 then
        failwithf "Dotnet existed with code: %i\n Message: \n %A" proc.ExitCode proc.Messages
)

Target.Create "CopyMonacoModules" (fun _ ->
    let requireJsOutput = jsLibsOutput </> "requirejs"
    let vsOutput = jsLibsOutput </> "vs"
    Directory.create requireJsOutput
    Directory.create vsOutput
    Shell.cp ("./node_modules" </> "requirejs" </> "require.js") requireJsOutput
    Shell.cp_r ("./node_modules" </> "monaco-editor" </> "min" </> "vs") vsOutput
)

Target.RunOrDefault "Build"

Target.Create "Setup" Target.DoNothing

"Clean"
    ==> "YarnInstall"
    ==> "Restore"
    ==> "CopyMonacoModules"
    ==> "Setup"

"Setup"
    ==> "Watch"

"Setup"
    ==> "Build"
