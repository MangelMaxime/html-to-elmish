module Tests.Parser

open HtmlConverter.Parser
open HtmlConverter.Parser.Lexer
open QUnit

let tests _ =
    registerModule("HtmlConverter.Parser")

    let tokens = tokenize "<"

    testCase "parseToken with Math" <| fun test ->
        test.deepEqual
            (parseToken LeftAngleBracket tokens)
            (ConsumeTokenResult.Match ("<", []))

    testCase "parseToken with no match" <| fun test ->
        test.deepEqual
            (parseToken RightAngleBracket tokens)
            (ConsumeTokenResult.NotMach tokens)

    testCase "parseTokenIgnore" <| fun test ->
        test.deepEqual
            (parseTokenIgnore LeftAngleBracket tokens)
            (ParseResult.Ignore, [])

    testCase "parseTokenKeep with Anonymous node" <| fun test ->
        test.deepEqual
            (parseTokenKeep LeftAngleBracket tokens)
            ("<"
             |> NodeValue.Leaf
             |> Node.Anonymous
             |> ParseResult.Keep, [])

    testCase "parseTokenKeep with Named node" <| fun test ->
        test.deepEqual
            (named "LEFT_ANGLE_BRACKET" (parseTokenKeep LeftAngleBracket) tokens)
            ({ Name = "LEFT_ANGLE_BRACKET"
               Value = NodeValue.Leaf "<" }
             |> Node.Named
             |> ParseResult.Keep, [])

    testCase "parseTokenKeep with Word TokenType" <| fun test ->
        test.deepEqual
            (parseTokenKeep Word (tokenize "h1"))
            ("h1"
             |> NodeValue.Leaf
             |> Node.Anonymous
             |> ParseResult.Keep, [])

    testCase "parseOptional with Keep result" <| fun test ->
        test.deepEqual
            (optional (parseTokenKeep LeftAngleBracket) tokens)
            ("<"
             |> NodeValue.Leaf
             |> Node.Anonymous
             |> ParseResult.Keep, [])

    testCase "parseOptional with Ignore result" <| fun test ->
        test.deepEqual
            (optional (parseTokenKeep RightAngleBracket) tokens)
            (ParseResult.Ignore, tokens)

    testCase "parseAnyFunction with at least one match" <| fun test ->
        test.deepEqual
            (parseAnyFunction [ parseTokenKeep RightAngleBracket
                                parseTokenKeep LeftAngleBracket ] tokens)
            ("<"
             |> NodeValue.Leaf
             |> Node.Anonymous
             |> ParseResult.Keep, [])

    testCase "parseAnyFunction with no match" <| fun test ->
        test.deepEqual
            (parseAnyFunction [ parseTokenKeep RightAngleBracket
                                parseTokenKeep Word ] tokens)
            (ParseResult.NoMatch, tokens)

    testCase "parseMultipleFunction" <| fun test ->
        test.deepEqual
            (parseMultipleFunction (parseTokenKeep LeftAngleBracket) (tokenize "<<"))
            ( [ Node.Anonymous (NodeValue.Leaf "<")
                Node.Anonymous(NodeValue.Leaf "<") ]
             |> NodeValue.Children
             |> Node.Anonymous
             |> ParseResult.Keep, [])


        test.deepEqual
            (parseMultipleFunction (parseTokenKeep LeftAngleBracket) (tokenize "<div>"))
            ( [ Node.Anonymous (NodeValue.Leaf "<") ]
             |> NodeValue.Children
             |> Node.Anonymous
             |> ParseResult.Keep, [ Word, "div"
                                    RightAngleBracket, ">"])

    testCase "parseSequenceFunction" <| fun test ->
        let parseTag =
            parseSequenceFunction [ parseTokenIgnore LeftAngleBracket
                                    parseTokenKeep Word
                                    parseTokenIgnore RightAngleBracket ]
        test.deepEqual
            (parseTag (tokenize "<div>"))
            ( [ Node.Anonymous (NodeValue.Leaf "div") ]
             |> NodeValue.Children
             |> Node.Anonymous
             |> ParseResult.Keep, []
            )

        test.deepEqual
            (parseTag (tokenize "<>"))
            (ParseResult.NoMatch, (tokenize "<>"))

    testCase "parseAtLeastOneFunction" <| fun test ->
        let parseAtLeastOneLeftOrRightBracket =
            [ parseTokenKeep LeftAngleBracket
              parseTokenKeep RightAngleBracket ]
            |> parseAnyFunction
            |> parseAtLeastOneFunction

        test.deepEqual
            (parseAtLeastOneFunction (parseTokenKeep LeftAngleBracket) (tokenize "<<<>"))
            ( [ Node.Anonymous (NodeValue.Leaf "<")
                Node.Anonymous (NodeValue.Leaf "<")
                Node.Anonymous (NodeValue.Leaf "<") ]
              |> NodeValue.Children
              |> Node.Anonymous
              |> ParseResult.Keep, [ RightAngleBracket, ">"])

        test.deepEqual
            (parseAtLeastOneLeftOrRightBracket (tokenize "<>!"))
            ( [ Node.Anonymous (NodeValue.Leaf "<")
                Node.Anonymous (NodeValue.Leaf ">") ]
              |> NodeValue.Children
              |> Node.Anonymous
              |> ParseResult.Keep, [ Word, "!"] )

        test.deepEqual
            (parseAtLeastOneLeftOrRightBracket (tokenize "!>"))
            ( ParseResult.NoMatch, (tokenize "!>") )
