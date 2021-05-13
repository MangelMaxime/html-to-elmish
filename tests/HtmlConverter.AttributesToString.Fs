module HtmlConverter.AttributesToString

open HtmlConverter.Converter
open Mocha

describe "HtmlConverter.Converter.attributesToString" (fun _ ->
    it "attributesToString: one attribute" (fun _ ->
        let result =
            [ "class", "button" ]
            |> attributesToString ""

        Assert.deepStrictEqual(
            result,
            [ "Class \"button\"" ]
        )
    )

    it "attributesToString: inline style with one rule" (fun _ ->
        let result =
            [ "style", "color:red" ]
            |> attributesToString ""

        Assert.deepStrictEqual(
            result,
            [ "Style [ Color \"red\" ]" ]
        )
    )


    it "attributesToString: inline style with custom CSSProp" (fun _ ->
        let result =
            [ "style", "custom-css: red" ]
            |> attributesToString ""

        Assert.deepStrictEqual(
            result,
            [ """Style [ CSSProp.Custom ("custom-css", "red") ]""" ]
        )
    )


    it "attributesToString: inline style with several rules" (fun _ ->
        let result =
            [ "style", "color:red; background-color: blue" ]
            |> attributesToString ""

        let expected =
            [
                """
Style [ Color "red"
         BackgroundColor "blue" ]
                """
            ]
            |> List.map String.adaptText

        Assert.deepStrictEqual(
            result,
            expected
        )
    )

    it "attributesToString: data attributes are converted to HTMLAttr.Data" (fun _ ->
        let result =
            [ "data-target", "button" ]
            |> attributesToString ""

        Assert.deepStrictEqual(
            result,
            [ "HTMLAttr.Data (\"target\", \"button\")" ]
        )
    )

    it "attributesToString: unkown attributes are converted to HTMLAttr.Custom" (fun _ ->
        let result =
            [ "customAttribute", "button" ]
            |> attributesToString ""

        Assert.deepStrictEqual(
            result,
            [ "HTMLAttr.Custom (\"customAttribute\", \"button\")" ]
        )
    )

    it "attributesToString: several attributes" (fun _ ->
        let result =
            [ "class", "button"
              "onClick", "button"
              "height", "50" ]
            |> attributesToString ""

        Assert.deepStrictEqual(
            result,
            [ "Class \"button\""
              "OnClick (fun _ -> ())"
              "Height 50" ]
        )
    )
)
