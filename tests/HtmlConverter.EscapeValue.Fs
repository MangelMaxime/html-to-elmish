module HtmlConverter.EscapeValue

open HtmlConverter.Converter
open HtmlConverter.References
open Mocha

describe "HtmlConverter.Converter.escapeValue" (fun _ ->

    it "escapeValue: String" (fun _ ->
        Assert.strictEqual(
            escapeValue AttributeType.String "maxime",
            "\"maxime\""
        )
    )

    it "escapeValue: Bool" (fun _ ->
        Assert.strictEqual(
            escapeValue AttributeType.Bool "false",
            "false"
        )

        Assert.strictEqual(
            escapeValue AttributeType.Bool "true",
            "true"
        )

        Assert.throws(
            fun () -> escapeValue AttributeType.Bool "NaN"
        )
    )

    it "escapeValue: Int" (fun _ ->
        Assert.strictEqual(
            escapeValue AttributeType.Int "2",
            "2"
        )
    )

    it "escapeValue: Obj" (fun _ ->
        Assert.strictEqual(
            escapeValue AttributeType.Obj "maxime",
            "maxime"
        )
    )

    it "escapeValue: Func" (fun _ ->
        Assert.strictEqual(
            escapeValue AttributeType.Func "function () ...",
            "(fun _ -> ())"
        )
    )


    it "escapeValue: Float" (fun _ ->
        Assert.strictEqual(
            escapeValue AttributeType.Float "1",
            "1."
        )

        Assert.strictEqual(
            escapeValue AttributeType.Float "1.2",
            "1.2"
        )
    )
)
