module HtmlConverter.React

open HtmlConverter.Converter
open HtmlConverter.References
open Mocha

describe "HtmlConverter.Converter.React" (fun _ ->


    it "simple tag" (fun _ ->
        let result = htmlToElmish "<div></div>"

        let expected =
            """
div [ ]
    [ ]
            """
            |> String.adaptText

        Assert.strictEqual(result, expected)
    )


    it "self-closing tag" (fun _ ->
        let result = htmlToElmish "<br/>"

        Assert.strictEqual(
            result,
            "br [ ]"
        )
    )


    it "tag with one attribute" (fun _ ->
        let result =
            htmlToElmish """<div class="button"></div>"""

        let expected =
            """
div [ Class "button" ]
    [ ]
            """
            |> String.adaptText

        Assert.strictEqual(
            result,
            expected
        )
    )

    it "tag with several attributes" (fun _ ->
        let result =
            htmlToElmish """<div class="button" height="50" onClick="onClick();"></div>"""

        let expected =
            """
div [ Class "button"
      Height 50
      OnClick (fun _ -> ()) ]
    [ ]
            """
            |> String.adaptText

        Assert.strictEqual(
            result,
            expected
        )
    )

    it "nested tag with several attributes" (fun _ ->
        let result =
            htmlToElmish
                """
<div class="button" height="50">
    <div class="button" height="50">
        <div class="button" height="50">
            Maxime
        </div>
    </div>
</div>
                """

        let expected =
            """
div [ Class "button"
      Height 50 ]
    [ div [ Class "button"
            Height 50 ]
        [ div [ Class "button"
                Height 50 ]
            [ str "Maxime" ] ] ]
            """
            |> String.adaptText

        Assert.strictEqual(
            result,
            expected
        )
    )


    it "nested children" (fun _ ->
        let result =
            htmlToElmish
                """
<div>
    <span>Hello,</span>
    <span>Maxime</span>
</div>
                """

        let expected =
            """
div [ ]
    [ span [ ]
        [ str "Hello," ]
      span [ ]
        [ str "Maxime" ] ]
            """
            |> String.adaptText

        Assert.strictEqual(
            result,
            expected
        )
    )



    it "nested children with inline text tag" (fun _ ->
        let result =
            htmlToElmish
                """
<div class="container">
    <div class="notification">
        This container is <strong>centered</strong>
    </div>
</div>
                """

        let expected =
            """
div [ Class "container" ]
    [ div [ Class "notification" ]
        [ str "This container is"
          strong [ ]
            [ str "centered" ] ] ]
            """
            |> String.adaptText

        Assert.strictEqual(
            result,
            expected
        )
    )

    it "self closing without backslash" (fun _ ->
        let result =
            htmlToElmish
                """
<input name="firstname">
                """

        let expected =
            """
input [ Name "firstname" ]
            """
            |> String.adaptText

        Assert.strictEqual(
            result,
            expected
        )
    )

    it "If the text is not the first or second child of it's parent and previous tag is self closing" (fun _ ->
        let result =
            htmlToElmish
                """
<p>
    <span>texte n°1</span>
    <span>texte n°2</span>
    <br>
    Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean efficitur sit amet massa fringilla egestas. Nullam condimentum luctus turpis.
</p>
                """

        let expected =
            """
p [ ]
    [ span [ ]
        [ str "texte n°1" ]
      span [ ]
        [ str "texte n°2" ]
      br [ ]
      str "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean efficitur sit amet massa fringilla egestas. Nullam condimentum luctus turpis." ]
            """
            |> String.adaptText

        Assert.strictEqual(
            result,
            expected
        )
    )

    it "If the text is the second child of it's parent and previous tag is self closing" (fun _ ->
        let result =
            htmlToElmish
                """
<div><input type="checkbox"> Press enter to submit</div>
                """

        let expected =
            """
div [ ]
    [ input [ Type "checkbox" ]
      str "Press enter to submit" ]
            """
            |> String.adaptText

        Assert.strictEqual(
            result,
            expected
        )
    )


    it "If the text is the first children of it's parent" (fun _ ->
        let result =
            htmlToElmish
                """
<div>my text here</div>
                """

        let expected =
            """
div [ ]
    [ str "my text here" ]
            """
            |> String.adaptText

        Assert.strictEqual(
            result,
            expected
        )
    )
)
