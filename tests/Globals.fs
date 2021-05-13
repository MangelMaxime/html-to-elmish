[<AutoOpen>]
module Globals

open Fable.Core
open System

[<Import("*", "assert")>]
let Assert: Node.Assert.IExports = jsNative

let always x _ = x

module String =

    let toUnixNewLine (s: string) =
        s.Replace("\r\n", "\n")

    let split (c : char) (text : string) =
        text.Split (c)

    let trimEnd (text : string) =
        text.TrimEnd()

    let skipFirstLineIfEmpty (lines : string list) =
        match lines with
        | head::tail ->
            if String.IsNullOrEmpty head then
                tail
            else
                lines

        | [ ] ->
            lines

    let adaptText (text : string) =
        text
        |> toUnixNewLine // Uniform the newlines
        |> split '\n' // Split into lines
        |> Seq.toList
        |> skipFirstLineIfEmpty // Remove the first line if empty
        |> String.concat "\n" // Rebuild a single string
        |> trimEnd // Trim whitespaces at the end of the string
