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
            |> attributesToString

        test.deepEqual
            result
            [ "Class \"button\"" ]

    testCase "attributesToString: unkown attributes are converted to HTMLAttr.Custom" <| fun test ->
        let result =
            [ "customAttribute", "button" ]
            |> attributesToString

        test.deepEqual
            result
            [ "HTMLAttr.Custom (\"customAttribute\", \"button\")" ]

    testCase "attributesToString: several attributes" <| fun test ->
        let result =
            [ "class", "button"
              "onClick", "button"
              "height", "50" ]
            |> attributesToString

        test.deepEqual
            result
            [ "Class \"button\""
              "OnClick (fun _ -> ())"
              "Height 50" ]

    testCase "simple tag" <| fun test ->
        let result = htmlToElmish 4 " " "<div></div>"

        test.equal
            result
            """div [ ]
    [ ]"""

    testCase "self-closing tag" <| fun test ->
        let result = htmlToElmish 4 " " "<br/>"
        test.equal
            result
            "br [ ]"

    testCase "tag with one attribute" <| fun test ->
        let result =
            htmlToElmish 4 " " """<div class="button"></div>"""

        test.equal
            result
            """div [ Class "button" ]
    [ ]"""

    testCase "tag with several attributes" <| fun test ->
        let result =
            htmlToElmish 4 " " """<div class="button" height="50" onClick="onClick();"></div>"""

        test.equal
            result
            """div [ Class "button"
      Height 50
      OnClick (fun _ -> ()) ]
    [ ]"""

    testCase "nested tag with several attributes" <| fun test ->
        let result =
            htmlToElmish 4 " " """<div class="button" height="50">
    <div class="button" height="50">
        <div class="button" height="50">
            Maxime
        </div>
    </div>
</div>"""

        test.equal
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
            htmlToElmish 4 " "
                """<div>
    <span>Hello,</span>
    <span>Maxime</span>
</div>"""

        test.equal
            result
            """div [ ]
    [ span [ ]
        [ str "Hello," ]
      span [ ]
        [ str "Maxime" ] ]"""

    testCase "nested children with inline text tag" <| fun test ->
        let result =
            htmlToElmish 4 " "
                """<div class="container">
    <div class="notification">
        This container is <strong>centered</strong>
    </div>
</div>"""

        test.equal
            result
            """div [ Class "container" ]
    [ div [ Class "notification" ]
        [ str "This container is"
          strong [ ]
            [ str "centered" ] ] ]"""


    testCase "self closing without backslash" <| fun test ->
        let result =
            htmlToElmish 4 " " """<input name="firstname">"""

        test.equal
            result
            """input [ Name "firstname" ]"""

    testCase "If the text is not the first or second child of it's parent and previous tag is self closing" <| fun test ->
        let result =
            htmlToElmish 4 " " """<p>
    <span>texte n°1</span>
    <span>texte n°2</span>
    <br>
    Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean efficitur sit amet massa fringilla egestas. Nullam condimentum luctus turpis.
</p>"""
        test.equal
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
            htmlToElmish 4 " " """<div><input type="checkbox"> Press enter to submit</div>"""

        test.equal
            result
            """div [ ]
    [ input [ Type "checkbox" ]
      str "Press enter to submit" ]"""

    testCase "If the text is the first children of it's parent" <| fun test ->
        let result =
            htmlToElmish 4 " " """<div>my text here</div>"""

        test.equal
            result
            """div [ ]
    [ str "my text here" ]"""
