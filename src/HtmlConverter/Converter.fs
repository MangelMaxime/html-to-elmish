module HtmlConverter.Converter

open System
open Fable.SimpleXml

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

let attributesToString (indentationLevel : int) (offset : int) (attributes : Map<string,string>) =
    let indentation = String.replicate (indentationLevel * indentationSize) " "
    let offset = String.replicate offset " "

    // If there no attributes
    if attributes.Count = 0 then
        "[ ]"
    else
        attributes
        |> Map.toList
        |> List.map (fun (key, value) ->

            References.attributes
            |> List.tryFind (fun (_, htmlName, _) ->
                key.ToLower() = htmlName.ToLower()
            )
            |> function
                | Some (fsharpName, _, typ) ->
                    fsharpName + " " + (escapeValue typ value)
                | None ->
                    "HTMLAttr.Custom (" + key + ", " + (escapeValue References.AttributeType.String value) + ")"
        )
        |> List.mapi(fun index s ->
            // First attributes, open the list
            if index = 0 then
                "[ " + s
            else
                offset + "  " + s
                |> String.prefix indentation
        )
        // Each attribute have it's own row
        |> String.concat ("\n")
        // Close the list
        |> String.suffix " ]"

let rec xmlElementToElmish (indentationLevel : int) (isFirst : bool) (isLast : bool) (xmlElement : XmlElement)  =
    let tagName = xmlElement.Name
    let attributes = xmlElement.Attributes
    let content = xmlElement.Content
    let isSelfClosing = xmlElement.SelfClosing
    let isTextNode = xmlElement.IsTextNode
    let children = xmlElement.Children

    if isTextNode then
        let trimedContent =
            match (isFirst, isLast) with
            | true, false -> content.TrimStart()
            | false, true -> content.TrimEnd()
            | true, true -> content.Trim()
            | false, false -> content

        printfn "%A" content
        "str \"" + trimedContent + "\""
    else
        let childrenStr =
            if isSelfClosing then
                ""
            else
                if children.Length = 0 then
                    if String.IsNullOrWhiteSpace content then
                        "[ ]"
                    else
                        "[ str \"" + content + "\" ]"
                    |> String.prefix (String.replicate ((indentationLevel + 1) * indentationSize) " ")
                else
                    children
                    //  We remove non significant whitespaces
                    |> List.filter (fun element ->
                        // not (String.IsNullOrWhiteSpace element.Content) || not element.IsTextNode
                        element.IsTextNode && not (String.IsNullOrWhiteSpace element.Content) || not element.IsTextNode
                    )
                    |> List.mapi (fun index element ->
                        xmlElementToElmish (indentationLevel + 1) (index = 0) (index = children.Length - 1) element
                    )
                    |> List.mapi (fun index s ->
                        if index = 0 then
                            "[ " + s
                        else
                            "  " + s
                        // |> String.prefix indentation
                        |> String.prefix (String.replicate ((indentationLevel + 1) * indentationSize) " ")
                    )
                    |> String.concat "\n"
                    |> String.suffix " ]"
                |> String.prefix "\n"

        tagName + " " + (attributesToString indentationLevel (tagName.Length + 1) attributes) + childrenStr

let htmlToElmsh (html : string) =
    match SimpleXml.tryParseElement html with
    | None ->
        Error "Failed to parse the htmlCode"
    | Some xmlElement ->
        xmlElementToElmish 0 true false xmlElement
        |> Ok
