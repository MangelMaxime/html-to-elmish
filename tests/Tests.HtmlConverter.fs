module Tests.HtmlConverter

open HtmlConverter.Converter
open HtmlConverter.References
open QUnit

let tests _ =
    registerModule("HtmlConverter")

    testCase "escapeValue: String" <| fun test ->
        test.equal
            (escapeValue AttributeType.String "maxime")
            "\"maxime\""

    testCase "escapeValue: Bool" <| fun test ->
        test.equal
            (escapeValue AttributeType.Bool "false")
            "false"

        test.equal
            (escapeValue AttributeType.Bool "true")
            "true"

        try
            (escapeValue AttributeType.Bool "NaN") |> ignore
            test.unexpected()
        with
            | _ -> test.pass()

    testCase "escapeValue: Int" <| fun test ->
        test.equal
            (escapeValue AttributeType.Int "2")
            "2"

    testCase "escapeValue: Obj" <| fun test ->
        test.equal
            (escapeValue AttributeType.Obj "maxime")
            "maxime"

    testCase "escapeValue: Func" <| fun test ->
        test.equal
            (escapeValue AttributeType.Func "function () ...")
            "(fun _ -> ())"

    testCase "escapeValue: Float" <| fun test ->
        test.equal
            (escapeValue AttributeType.Float "1")
            "1."

        test.equal
            (escapeValue AttributeType.Float "1.2")
            "1.2"

    testCase "attributesToString: one attribute" <| fun test ->
        let result =
            [ "class", "button" ]
            |> Map.ofList
            |> attributesToString 0 0

        test.equal
            result
            """[ Class "button" ]"""

    testCase "attributesToString: unkown attributes are converted to HTMLAttr.Custom" <| fun test ->
        let result =
            [ "customAttribute", "button" ]
            |> Map.ofList
            |> attributesToString 0 0

        test.equal
            result
            """[ HTMLAttr.Custom (customAttribute, "button") ]"""

    testCase "attributesToString: several attributes" <| fun test ->
        let result =
            [ "class", "button"
              "onClick", "button"
              "height", "50" ]
            |> Map.ofList
            |> attributesToString 0 0

        test.equal
            result
            """[ Class "button"
  Height 50
  OnClick (fun _ -> ()) ]"""

    testCase "attributesToString: several attributes with offset" <| fun test ->
        let result =
            [ "class", "button"
              "onClick", "button"
              "height", "50" ]
            |> Map.ofList
            |> attributesToString 0 4

        test.equal
            result
            """[ Class "button"
      Height 50
      OnClick (fun _ -> ()) ]"""

    testCase "simple tag" <| fun test ->
        htmlToElmsh "<div></div>"
        |> function
            | Ok result ->
                test.equal
                    result
                    """div [ ]
    [ ]"""
            | Error msg -> test.unexpected msg

    testCase "self-closing tag" <| fun test ->
        htmlToElmsh "<br/>"
        |> function
            | Ok result ->
                test.equal
                    result
                    "br [ ]"
            | Error msg -> test.unexpected msg

    testCase "tag with one attribute" <| fun test ->
        htmlToElmsh """<div class="button"></div>"""
        |> function
            | Ok result ->
                test.equal
                    result
                    """div [ Class "button" ]
    [ ]"""
            | Error msg -> test.unexpected msg

    testCase "tag with several attributes" <| fun test ->
        htmlToElmsh """<div class="button" height="50" onClick="onClick();"></div>"""
        |> function
            | Ok result ->
                test.equal
                    result
                    """div [ Class "button"
      Height 50
      OnClick (fun _ -> ()) ]
    [ ]"""
            | Error msg -> test.unexpected msg

    testCase "nested children" <| fun test ->
        htmlToElmsh """<div>
    <span>Hello, </span>
    <span>Maxime</span>
</div>"""
        |> function
            | Ok result ->
                test.equal
                    result
                    """div [ ]
    [ span [ ]
        [ str "Hello, " ]
      span [ ]
        [ str "Maxime" ] ]"""
            | Error msg -> test.unexpected msg

    testCase "nested children with inline text tag" <| fun test ->
        htmlToElmsh """<div class="container">
    <div class="notification">
        This container is <strong>centered</strong> on desktop.
    </div>
</div>"""
        |> function
            | Ok result ->
                test.equal
                    """div [ Class "container" ]
    [ div [ Class "notification" ]
        [ str "This container is "
          strong [ ]
            [ str "centered" ]
          str " on desktop." ] ]"""
                    result
            | Error msg -> test.unexpected msg

    testCase "self closing withouth backslash" <| fun test ->
        htmlToElmsh """<input name="firstname">"""
        |> function
            | Ok result ->
                test.equal
                    """input [ Name "firstname"]"""
                    result
            | Error msg -> test.unexpected msg
