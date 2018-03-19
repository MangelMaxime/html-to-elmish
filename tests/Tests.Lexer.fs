module Tests.Lexer

open HtmlConverter.Parser.Lexer
open QUnit

let tests _ =
    registerModule("HtmlConverter.Parser.Lexer")

    testCase "consumeToken `<` token" <| fun test ->
        consumeToken (LeftAngleBracket, "<") "<br/>"
        |> function
            | None -> test.failwith "Token `<` not consumed"
            | Some (token, remainderString) ->
                test.deepEqual (LeftAngleBracket, "<") token
                test.equal "br/>" remainderString

    testCase "consumeToken `>` token" <| fun test ->
        consumeToken (RightAngleBracket, ">") ">some text"
        |> function
            | None -> test.failwith "Token `>` not consumed"
            | Some (token, remainderString) ->
                test.deepEqual (RightAngleBracket, ">") token
                test.equal "some text" remainderString

    testCase "consumeToken `=` token" <| fun test ->
        consumeToken (EqualSign, "=") "='my-value'"
        |> function
            | None -> test.failwith "Token `=` not consumed"
            | Some (token, remainderString) ->
                test.deepEqual (EqualSign, "=") token
                test.equal "'my-value'" remainderString

    testCase "consumeToken `\"` token" <| fun test ->
        consumeToken (DoubleQuoteMark, "\"") "\"my-value\""
        |> function
            | None -> test.failwith "Token `\"` not consumed"
            | Some (token, remainderString) ->
                test.deepEqual (DoubleQuoteMark, "\"") token
                test.equal "my-value\"" remainderString

    testCase "consumeToken `'` token" <| fun test ->
        consumeToken (SingleQuoteMark, "'") "'my-value'"
        |> function
            | None -> test.failwith "Token `'` not consumed"
            | Some (token, remainderString) ->
                test.deepEqual (SingleQuoteMark, "'") token
                test.equal "my-value'" remainderString

    testCase "consumeToken `/` token" <| fun test ->
        consumeToken (ForwardSlash, "/") "/>some text'"
        |> function
            | None -> test.failwith "Token `/` not consumed"
            | Some (token, remainderString) ->
                test.deepEqual (ForwardSlash, "/") token
                test.equal ">some text'" remainderString

    testCase "consumeToken `-` token" <| fun test ->
        consumeToken (Dash, "-") "-value"
        |> function
            | None -> test.failwith "Token `-` not consumed"
            | Some (token, remainderString) ->
                test.deepEqual (Dash, "-") token
                test.equal "value" remainderString

    testCase "consumeWhitespace" <| fun test ->
        consumeToken wildcards.Head "    test"
        |> function
            | None -> test.failwith "Whitespace token not consumed"
            | Some (token, remainderString) ->
                test.deepEqual (Whitespace, "    ") token
                test.equal "test" remainderString

    testCase "consumeFirstTokenMatch OpeningComment" <| fun test ->
        consumeFirstTokenMatch tokenizerGrammar "<!-- A comment -->"
        |> function
            | None -> test.fail()
            | Some (token, remainderString) ->
                test.deepEqual (OpeningComment, "<!--") token
                test.equal " A comment -->" remainderString

    testCase "tokenize" <| fun test ->
        let tokens = tokenize "<div>Hello, I am a test</div>"

        let expected =
            [ LeftAngleBracket, "<"
              Word, "div"
              RightAngleBracket, ">"
              Word, "Hello,"
              Whitespace, " "
              Word, "I"
              Whitespace, " "
              Word, "am"
              Whitespace, " "
              Word, "a"
              Whitespace, " "
              Word, "test"
              LeftAngleBracket, "<"
              ForwardSlash, "/"
              Word, "div"
              RightAngleBracket, ">" ]

        test.deepEqual expected tokens
