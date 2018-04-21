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

module Util =

    let visitFile (visitor: string->string) (fileName : string) =
        File.ReadAllLines(fileName)
        |> Array.map (visitor)
        |> fun lines -> File.WriteAllLines(fileName, lines)

    let replaceLines (replacer: string->Match->string option) (reg: Regex) (fileName: string) =
        fileName |> visitFile (fun line ->
            let m = reg.Match(line)
            if not m.Success
            then line
            else
                match replacer line m with
                | None -> line
                | Some newLine -> newLine)

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

// --------------------------------------------------------------------------------------
// Build a NuGet package
let needsPublishing (versionRegex: Regex) (releaseNotes: ReleaseNotes.ReleaseNotes) projFile =
    printfn "Project: %s" projFile
    if releaseNotes.NugetVersion.ToUpper().EndsWith("NEXT")
    then
        printfn "Version in Release Notes ends with NEXT, don't publish yet."
        false
    else
        File.ReadLines(projFile)
        |> Seq.tryPick (fun line ->
            let m = versionRegex.Match(line)
            if m.Success then Some m else None)
        |> function
            | None -> failwith "Couldn't find version in project file"
            | Some m ->
                let sameVersion = m.Groups.[1].Value = releaseNotes.NugetVersion
                if sameVersion then
                    printfn "Already version %s, no need to publish." releaseNotes.NugetVersion
                not sameVersion

let toPackageReleaseNotes (notes: string list) =
    "* " + String.Join("\n * ", notes)
    |> (fun txt -> txt.Replace("\"", "\\\""))

let pushNuget (releaseNotes: ReleaseNotes.ReleaseNotes) (projFile: string) =
    let versionRegex = Regex("<Version>(.*?)</Version>", RegexOptions.IgnoreCase)

    if needsPublishing versionRegex releaseNotes projFile then
        let projDir = Path.GetDirectoryName(projFile)
        let nugetKey =
            match Environment.environVarOrNone "NUGET_KEY" with
            | Some nugetKey -> nugetKey
            | None -> failwith "The Nuget API key must be set in a NUGET_KEY environmental variable"
        (versionRegex, projFile)
        ||> Util.replaceLines (fun line _ ->
                                    versionRegex.Replace(line, "<Version>"+releaseNotes.NugetVersion+"</Version>") |> Some)

        let result =
            DotNet.exec
                (DotNet.Options.withWorkingDirectory projDir)
                "pack"
                (sprintf "-c Release /p:PackageReleaseNotes=\"%s\"" (toPackageReleaseNotes releaseNotes.Notes))

        if not result.OK then failwithf "dotnet fable failed with code %i" result.ExitCode

        Directory.GetFiles(projDir </> "bin" </> "Release", "*.nupkg")
        |> Array.find (fun nupkg -> nupkg.Contains(releaseNotes.NugetVersion))
        |> (fun nupkg ->
            Paket.push (fun p -> { p with ApiKey = nugetKey
                                          WorkingDir = Path.getDirectory nupkg }))

Target.create "PublishNugets" (fun _ ->
    !! "./src/HtmlConverter/Fable.HtmlConverter.fsproj"
    |> Seq.iter(fun s ->
        let projFile = s
        let projDir = IO.Path.GetDirectoryName(projFile)
        let release = projDir </> "RELEASE_NOTES.md" |> ReleaseNotes.load
        pushNuget release projFile
    )
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

"Restore"
    ==> "PublishNugets"

"CI"
    <== [ "Build"
          "RunTests"]

// Start build
Target.runOrDefault "Build"
