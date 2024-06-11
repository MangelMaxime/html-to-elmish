module Tests.HtmlConverter

open HtmlConverter.Converter
open HtmlConverter.References
open QUnit

let private toUnixNewLine (s: string) =
    s.Replace("\r\n", "\n")

type Asserter with
    member test.deepEqualIgnoreNewLineSeparator (a: string list) (b: string list) =
        let aUnix = a |> List.map toUnixNewLine
        let bUnix = b |> List.map toUnixNewLine
        test.deepEqual aUnix bUnix

    member test.equalIgnoreNewLineSeparator (a: string) (b: string) =
        let aUnix = a |> toUnixNewLine
        let bUnix = b |> toUnixNewLine
        test.equal aUnix bUnix

let tests _ =
    registerModule("HtmlConverter")

    testCase "escapeValue: String" <| fun test ->
        test.equalIgnoreNewLineSeparator
            (escapeValue AttributeType.String "maxime")
            "\"maxime\""

    testCase "escapeValue: Bool" <| fun test ->
        test.equalIgnoreNewLineSeparator
            (escapeValue AttributeType.Bool "false")
            "false"

        test.equalIgnoreNewLineSeparator
            (escapeValue AttributeType.Bool "true")
            "true"

        try
            (escapeValue AttributeType.Bool "NaN") |> ignore
            test.unexpected()
        with
            | _ -> test.pass()

    testCase "escapeValue: Int" <| fun test ->
        test.equalIgnoreNewLineSeparator
            (escapeValue AttributeType.Int "2")
            "2"

    testCase "escapeValue: Obj" <| fun test ->
        test.equalIgnoreNewLineSeparator
            (escapeValue AttributeType.Obj "maxime")
            "maxime"

    testCase "escapeValue: Func" <| fun test ->
        test.equalIgnoreNewLineSeparator
            (escapeValue AttributeType.Func "function () ...")
            "(fun _ -> ())"

    testCase "escapeValue: Float" <| fun test ->
        test.equalIgnoreNewLineSeparator
            (escapeValue AttributeType.Float "1")
            "1."

        test.equalIgnoreNewLineSeparator
            (escapeValue AttributeType.Float "1.2")
            "1.2"

    testCase "attributesToString: one attribute" <| fun test ->
        let result =
            [ "class", "button" ]
            |> attributesToString ""

        test.deepEqualIgnoreNewLineSeparator
            result
            [ "Class \"button\"" ]


    testCase "attributesToString: inline style with one rule" <| fun test ->
        let result =
            [ "style", "color:red" ]
            |> attributesToString ""

        test.deepEqualIgnoreNewLineSeparator
            result
            [ "Style [ Color \"red\" ]" ]

    testCase "attributesToString: inline style with custom CSSProp" <| fun test ->
        let result =
            [ "style", "custom-css: red" ]
            |> attributesToString ""

        test.deepEqualIgnoreNewLineSeparator
            result
            [ """Style [ CSSProp.Custom ("custom-css", "red") ]""" ]

    testCase "attributesToString: inline style with several rules" <| fun test ->
        let result =
            [ "style", "color:red; background-color: blue" ]
            |> attributesToString ""

        test.deepEqualIgnoreNewLineSeparator
            result
            [ """Style [ Color "red"
         BackgroundColor "blue" ]""" ]

    testCase "attributesToString: data attributes are converted to HTMLAttr.Data" <| fun test ->
        let result =
            [ "data-target", "button" ]
            |> attributesToString ""

        test.deepEqualIgnoreNewLineSeparator
            result
            [ "HTMLAttr.Data (\"target\", \"button\")" ]

    testCase "attributesToString: unkown attributes are converted to HTMLAttr.Custom" <| fun test ->
        let result =
            [ "customAttribute", "button" ]
            |> attributesToString ""

        test.deepEqualIgnoreNewLineSeparator
            result
            [ "HTMLAttr.Custom (\"customAttribute\", \"button\")" ]

    testCase "attributesToString: several attributes" <| fun test ->
        let result =
            [ "class", "button"
              "onClick", "button"
              "height", "50" ]
            |> attributesToString ""

        test.deepEqualIgnoreNewLineSeparator
            result
            [ "Class \"button\""
              "OnClick (fun _ -> ())"
              "Height 50" ]

    testCase "simple tag" <| fun test ->
        let result = htmlToElmish "<div></div>"

        test.equalIgnoreNewLineSeparator
            result
            """div [ ]
    [ ]"""

    testCase "self-closing tag" <| fun test ->
        let result = htmlToElmish "<br/>"
        test.equalIgnoreNewLineSeparator
            result
            "br [ ]"

    testCase "tag with one attribute" <| fun test ->
        let result =
            htmlToElmish """<div class="button"></div>"""

        test.equalIgnoreNewLineSeparator
            result
            """div [ Class "button" ]
    [ ]"""

    testCase "tag with several attributes" <| fun test ->
        let result =
            htmlToElmish """<div class="button" height="50" onClick="onClick();"></div>"""

        test.equalIgnoreNewLineSeparator
            result
            """div [ Class "button"
      Height 50
      OnClick (fun _ -> ()) ]
    [ ]"""

    testCase "nested tag with several attributes" <| fun test ->
        let result =
            htmlToElmish """<div class="button" height="50">
    <div class="button" height="50">
        <div class="button" height="50">
            Maxime
        </div>
    </div>
</div>"""

        test.equalIgnoreNewLineSeparator
            result
            """div [ Class "button"
      Height 50 ]
    [ div [ Class "button"
            Height 50 ]
        [ div [ Class "button"
                Height 50 ]
            [ str "Maxime" ] ] ]"""

    testCase "nested children" <| fun test ->
        let result =
            htmlToElmish
                """<div>
    <span>Hello,</span>
    <span>Maxime</span>
</div>"""

        test.equalIgnoreNewLineSeparator
            result
            """div [ ]
    [ span [ ]
        [ str "Hello," ]
      span [ ]
        [ str "Maxime" ] ]"""

    testCase "nested children with inline text tag" <| fun test ->
        let result =
            htmlToElmish
                """<div class="container">
    <div class="notification">
        This container is <strong>centered</strong>
    </div>
</div>"""

        test.equalIgnoreNewLineSeparator
            result
            """div [ Class "container" ]
    [ div [ Class "notification" ]
        [ str "This container is"
          strong [ ]
            [ str "centered" ] ] ]"""


    testCase "self closing without backslash" <| fun test ->
        let result =
            htmlToElmish """<input name="firstname">"""

        test.equalIgnoreNewLineSeparator
            result
            """input [ Name "firstname" ]"""

    testCase "If the text is not the first or second child of it's parent and previous tag is self closing" <| fun test ->
        let result =
            htmlToElmish """<p>
    <span>texte n°1</span>
    <span>texte n°2</span>
    <br>
    Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean efficitur sit amet massa fringilla egestas. Nullam condimentum luctus turpis.
</p>"""
        test.equalIgnoreNewLineSeparator
            result
            """p [ ]
    [ span [ ]
        [ str "texte n°1" ]
      span [ ]
        [ str "texte n°2" ]
      br [ ]
      str "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean efficitur sit amet massa fringilla egestas. Nullam condimentum luctus turpis." ]"""

    testCase "If the text is the second child of it's parent and previous tag is self closing" <| fun test ->
        let result =
            htmlToElmish """<div><input type="checkbox"> Press enter to submit</div>"""

        test.equalIgnoreNewLineSeparator
            result
            """div [ ]
    [ input [ Type "checkbox" ]
      str "Press enter to submit" ]"""

    testCase "If the text is the first children of it's parent" <| fun test ->
        let result =
            htmlToElmish """<div>my text here</div>"""

        test.equalIgnoreNewLineSeparator
            result
            """div [ ]
    [ str "my text here" ]"""
