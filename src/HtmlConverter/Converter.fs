module HtmlConverter.Converter

open System.Collections.Generic
open Thoth.Json
open Fable.Core.JsInterop
open System

let [<Literal>] INDENT_WITH = " "
let [<Literal>] INDENT_SIZE = 4

let indent (depth : int) =
    String.replicate (depth * INDENT_SIZE) INDENT_WITH

let escapeValue (typ : References.AttributeType) (value : string) =
    match typ with
    | References.AttributeType.String ->
        "\"" + value + "\""

    | References.AttributeType.Func -> "(fun _ -> ())"

    | References.AttributeType.Float ->
        // Make sure, the number is a valid float
        // This is a really simple check
        if value.Contains(".") then
            value
        else
            value + "."

    | References.AttributeType.Bool ->
        match value with
        | "true" -> "true"
        | "false" -> "false"
        | x -> failwithf "`%s` is not a valid bool value" x

    | References.AttributeType.Obj
    | References.AttributeType.Int -> value

module String =

    let suffix suf s : string = s + suf

    let prefix pre s : string = pre + s

let indentationSize = 4

let [<Literal>] PREFIX_DATA_ATTRIBUTE  = "data-"

let [<Literal>] PREFIX_STYLE_HTML_ATTR = "Style [ "

let attributesToString (currentRow : string) (attributes : (string * string) list) : string list =
    attributes
    |> List.map (fun (key, value) ->
        References.attributes
        |> List.tryFind (fun (_, htmlName, _) ->
            key.ToLower() = htmlName.ToLower()
        )
        |> function
            | Some (fsharpName, _, typ) ->
                fsharpName + " " + (escapeValue typ value)
            | None ->
                if key.Contains(PREFIX_DATA_ATTRIBUTE) then
                    "HTMLAttr.Data (\"" + key.Substring(PREFIX_DATA_ATTRIBUTE.Length) + "\", " + (escapeValue References.AttributeType.String value) + ")"
                elif key = "style" then
                    value.Split(';')
                    |> Array.filter (String.IsNullOrWhiteSpace >> not)
                    |> Array.map(fun style ->
                        match style.Split(':') with
                        | [| styleProp; styleValue |] ->
                            // Clean whitespaces spaces
                            let styleProp = styleProp.Trim()
                            let styleValue = styleValue.Trim()
                            // Convert into PascalCase
                            let stylePropPascalCase =
                                styleProp.Split('-')
                                |> Array.map(fun str ->
                                    str.[..0].ToUpperInvariant() + str.[1..]
                                )
                                |> String.concat ""

                            // If the pascalCaseVersion is known, then use it
                            // Otherwise, use a the custom case
                            match Array.contains stylePropPascalCase References.cssProps with
                            | true -> stylePropPascalCase + " " + (escapeValue References.AttributeType.String styleValue)
                            | false -> "CSSProp.Custom (\"" + styleProp + "\", " + (escapeValue References.AttributeType.String styleValue) + ")"

                        | invalid ->
                            invalid
                            |> String.concat ":"
                            |> String.prefix "invalid_css: "
                    )
                    |> Array.mapi (fun index str ->
                        if index = 0 then
                            str
                        else
                            let lastIndex = currentRow.LastIndexOf '['
                            String.replicate (lastIndex + 2) " "
                                + String.replicate (PREFIX_STYLE_HTML_ATTR.Length) " "
                                + str
                    )
                    |> String.concat "\n"
                    |> String.prefix PREFIX_STYLE_HTML_ATTR
                    |> String.suffix " ]"
                else
                    "HTMLAttr.Custom (\"" + key + "\", " + (escapeValue References.AttributeType.String value) + ")"
    )

let private voidElements : string list =
    [ "area"
      "base"
      "br"
      "col"
      "embed"
      "hr"
      "img"
      "input"
      "keygen"
      "link"
      "menuitem"
      "meta"
      "param"
      "source"
      "track"
      "wbr" ]

let countFirstOccurenceSize (getStr : string) (chkdChar : char) =
    let rec loop i count =
        if i < getStr.Length then
            if getStr.[i] = chkdChar then loop (i+1) (count+1)
            else count
        else count
    loop 0 0

let htmlToElmish (htmlCode : string) =
    let mutable fsharpCode = ""
    let mutable depth = -1
    let context = Dictionary<int, bool>()

    let handler = createEmpty<Htmlparser2.Handler>


    handler.onopentag <- fun (name : string) (attributes : obj) ->
        let attributes =
            // Unsafe unwrap, be we consider the libs always produce valid keyValues mapping
            match Decode.fromValue "$" (Decode.keyValuePairs Decode.string) attributes with
            | Ok attributes -> attributes
            | Error msg -> failwith msg

        depth <- depth + 1
        //                               is in dict
        //                               |     value of key `depth`
        if context.TryGetValue(depth) = (true, true) then
            fsharpCode <- fsharpCode + "\n" + (indent depth) + " "
        else
            context.[depth] <- true

        if depth <> 0 then
            fsharpCode <- fsharpCode + " "

        // Try to detect inlined tag in TextNode
        let lastRow = fsharpCode.Split('\n') |> Array.last
        let trimed = lastRow.Trim()
        if trimed.StartsWith("[") && trimed.Length <> 1 then
            fsharpCode <- fsharpCode.TrimEnd()
            fsharpCode <- fsharpCode + "\n" + String.replicate (countFirstOccurenceSize lastRow ' ' + 2) INDENT_WITH

        // Add tag name
        fsharpCode <- fsharpCode + name
        // Add open attributes bracket
        fsharpCode <- fsharpCode + " ["

        // If we have some attributes
        if attributes.Length > 0 then
            let attrs =
                attributes
                |> attributesToString (fsharpCode.Split('\n') |> Array.last)
                |> List.mapi (fun index attr ->
                    if index = 0 then
                        " " + attr
                    else
                        let lastRow = fsharpCode.Split('\n') |> Array.last
                        let lastIndex = lastRow.LastIndexOf '['
                        String.replicate (lastIndex + 2) " " + attr
                )
                |> String.concat "\n"
                // Add space before the closing bracket
                |> String.suffix " "
            fsharpCode <- fsharpCode + attrs
        else
            // Add space between attr list brackets, to have good display
            fsharpCode <- fsharpCode + " "

        // Close attribue bracket
        fsharpCode <- fsharpCode + "]"

        // If the tag can haev children open the bracket
        if not (List.contains name voidElements) then
            fsharpCode <- fsharpCode + "\n" + (indent (depth + 1)) + "["


    handler.ontext <- fun rawText ->
        let text = rawText.Trim()
        if not (String.IsNullOrWhiteSpace text) then
            // Try to detect inlined tag in TextNode
            let lastRow = fsharpCode.Split('\n') |> Array.last
            let trimed = lastRow.Trim()
            // If the trimed text is empty, don't try to fixed spaces
            if not (String.IsNullOrEmpty trimed) then
                fsharpCode <- fsharpCode.TrimEnd()
                // If the text is the first children of it's parent, add one space
                // Example:
                // div [ ]
                //     [ str "my text here" ]
                //      ^
                if trimed.StartsWith("[") && trimed.Length = 1 then
                    fsharpCode <- fsharpCode + " "
                // If the text is the second child of it's parent and previous tag is self closing, go to the line + indent by same space as last line + add 2 extra spaces
                // Example:
                // div [ ]
                //     [ input [ Type "checkbox" ]
                //       str "Press enter to submit" ]
                // ^^^^^^
                elif trimed.StartsWith("[") && trimed.Length <> 1 then
                    fsharpCode <- fsharpCode + "\n" + String.replicate (countFirstOccurenceSize lastRow ' ' + 2) " "
                // If the text is not the first or second child of it's parent and previous tag is self closing, go to the line + indent by same space as last line
                // Example:
                // p [ ]
                //     [ span [ ]
                //         [ str "texte n°1" ]
                //       span [ ]
                //         [ str "texte n°2" ]
                //       br [ ]
                //       str "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean efficitur sit amet massa fringilla egestas. Nullam condimentum luctus turpis." ]
                // ^^^^^^
                else
                    fsharpCode <- fsharpCode + "\n" + String.replicate (countFirstOccurenceSize lastRow ' ') " "

            if text.Length > 0 then
                fsharpCode <- fsharpCode + "str \"" + text + "\""

    handler.onclosetag <- fun name ->
        //                                   is in dict
        //                                   |     value of key `depth + 1`
        if context.TryGetValue(depth + 1) = (true, true) then
            context.Remove (depth + 1) |> ignore

        depth <- depth - 1
        if not (List.contains name voidElements) then
            fsharpCode <- fsharpCode + " ]"

    let parser = Htmlparser2.exports.Parser.Create(handler)
    parser.write(htmlCode)
    fsharpCode
