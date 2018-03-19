namespace HtmlConverter

module Parser =

    module Lexer =

        open System.Text.RegularExpressions

        type TokenType =
            | OpeningComment
            | ClosingComment
            | LeftAngleBracket
            | RightAngleBracket
            | EqualSign
            | DoubleQuoteMark
            | SingleQuoteMark
            | ForwardSlash
            | Dash
            | Whitespace
            | Word

        type Chars = char list

        type TokenValue = string

        type Token = TokenType * TokenValue

        type Tokens = Token list

        type TokenRecipe = TokenType * string

        type RemainderString = string

        let reservedCharTokenLookup : TokenRecipe list =
            [ LeftAngleBracket, "<"
              RightAngleBracket, ">"
              EqualSign, "="
              DoubleQuoteMark, "\""
              SingleQuoteMark, "'"
              ForwardSlash, "/"
              Dash, "-" ]

        let commentSequences : TokenRecipe list =
            [ OpeningComment, "<!--"
              ClosingComment, "-->" ]

        let wordRegex : string =
            "[^<>=\"'\/-\\s]+"

        let doubleQuotedStringRegex : string =
            """[^\\"]+"""

        let wildcards : TokenRecipe list =
            [ Whitespace, "(\\s)+"
              Word, wordRegex ]

        let tokenizerGrammar : TokenRecipe list =
            commentSequences @ reservedCharTokenLookup @ wildcards

        let consumeToken ((tokenType , regexString ) : TokenRecipe) (str : string) : (Token * RemainderString) option  =
            let regex = Regex("^" + regexString)
            let matches = regex.Matches str |> Seq.cast<Match> |> Seq.toList
            match matches with
            | [] ->
                None

            | head::_ ->
                let token = (tokenType, head.Value)
                let remainder = regex.Replace(str, "", 1)

                Some(token, remainder)

        let rec consumeFirstTokenMatch (tokenRecipes : TokenRecipe list) (str : string) : (Token * RemainderString) option =
            match tokenRecipes with
            | [] ->
                None

            | tokenRecipe :: tailTokenRecipes ->
                match consumeToken tokenRecipe str with
                | None ->
                    consumeFirstTokenMatch tailTokenRecipes str

                | Some result ->
                    Some result

        let tokenize (str : string) : Token list =
            let rec internalTokenize accTokens reminderString =
                match consumeFirstTokenMatch tokenizerGrammar reminderString with
                | None ->
                    accTokens

                | Some (token, remainderRemainderString) ->
                    internalTokenize (accTokens @ [ token ]) remainderRemainderString

            internalTokenize [] str

    open Lexer

    [<RequireQualifiedAccess>]
    type Node =
        | Named of NodeData
        | Anonymous of NodeValue

    and NodeValue =
        | Leaf of string
        | Children of Node list

    and NodeData =
        { Name : string
          Value : NodeValue }

    [<RequireQualifiedAccess>]
    type ParseResult =
        | Keep of Node
        | Ignore
        | NoMatch

    type ParseResults = ParseResult list
    type Nodes = Node list
    type RemainderTokens = Token list

    [<RequireQualifiedAccess>]
    type ConsumeTokenResult =
        | Match of (string * RemainderTokens)
        | NotMach of RemainderTokens

    type ParseFunction = Tokens -> (ParseResult * RemainderTokens)
    type ParseFunctions = ParseFunction list

    let parseToken (expectedType : TokenType) : Tokens -> ConsumeTokenResult =
        fun tokens ->
            match tokens with
            | (tokenType, tokenValue):: tailTokens ->
                if tokenType = expectedType then
                    ConsumeTokenResult.Match (tokenValue, tailTokens)
                else
                    ConsumeTokenResult.NotMach tokens
            | [] ->
                ConsumeTokenResult.NotMach []

    let parseTokenIgnore (expectedType : TokenType) : ParseFunction =
        fun tokens ->
            match (parseToken expectedType tokens) with
            | ConsumeTokenResult.Match  (_, remainderTokens) ->
                ParseResult.Ignore, remainderTokens
            | ConsumeTokenResult.NotMach remainderTokens ->
                ParseResult.NoMatch, remainderTokens

    let parseTokenKeep (expectedType : TokenType) : ParseFunction =
        fun tokens ->
            match (parseToken expectedType tokens) with
            | ConsumeTokenResult.Match (tokenValue, remainderTokens) ->
                Leaf tokenValue
                |> Node.Anonymous
                |> ParseResult.Keep, remainderTokens
            | ConsumeTokenResult.NotMach remainderTokens ->
                ParseResult.NoMatch, remainderTokens

    let named (name : string) (parseFunction : ParseFunction) : ParseFunction =
        fun tokens ->
            match parseFunction tokens with
            // If the node is anonyne, named it
            | ParseResult.Keep (Node.Anonymous nodeValue), remainderTokens ->
                { Name = name
                  Value = nodeValue }
                |> Node.Named
                |> ParseResult.Keep, remainderTokens
            // Otherwise, forward the result directly
            | result -> result

    let ignore (parseFunction : ParseFunction) : ParseFunction =
        fun tokens ->
            match parseFunction tokens with
            // If there was a match, ignore it
            | ParseResult.Keep _, remainderTokens ->
                ParseResult.Ignore, remainderTokens
            // Otherwise, forward the result directly
            | result -> result

    let optional (parseFunction : ParseFunction) : ParseFunction =
        fun tokens ->
            match parseFunction tokens with
            // If nothing matched, mark it as ignored
            // Equivalent to None case
            | ParseResult.NoMatch, remainderTokens ->
                ParseResult.Ignore, remainderTokens
            // Otherwise, forward the result directly
            // Equivalent to Some case
            | result -> result

    let parseAnyFunction (parseFunctions : ParseFunctions) : ParseFunction =
        let rec impl parseFunctions tokens =
            match  parseFunctions with
            | parseFunction::remainderParseFunctions ->
                match parseFunction tokens with
                // If we didn't match, try the next parse function
                | ParseResult.NoMatch, _ ->
                    impl remainderParseFunctions tokens
                // We got match, forward the result
                | result -> result
            | [] ->
                ParseResult.NoMatch, tokens

        impl parseFunctions

    let parseMultipleFunction (repeatedFunction : ParseFunction) : ParseFunction =
        let rec impl (nodes : Nodes) (remainderTokens : RemainderTokens) =
            match repeatedFunction remainderTokens with
            | ParseResult.Keep node, remainderTokens ->
                impl (nodes @ [node]) remainderTokens
            | ParseResult.Ignore, remainderTokens ->
                impl nodes remainderTokens
            | ParseResult.NoMatch , remainderTokens ->
                nodes, remainderTokens

        function
        | [] ->
            []
            |> NodeValue.Children
            |> Node.Anonymous
            |> ParseResult.Keep, []
        | tokens ->
            let (nodes, remainderTokens) = impl [] tokens
            nodes
            |> NodeValue.Children
            |> Node.Anonymous
            |> ParseResult.Keep, remainderTokens

    let parseSequenceFunction (parseFunctions : ParseFunctions) : ParseFunction =
        function
        | [] ->
            ParseResult.NoMatch, []
        | tokens ->
            let rec impl parseFunctions nodes remainderTokens =
                match parseFunctions with
                | parseFunction::remainderParseFunctions ->
                    match parseFunction remainderTokens with
                    | ParseResult.Keep node, remainderTokens ->
                        impl remainderParseFunctions (nodes @ [node]) remainderTokens
                    | ParseResult.Ignore, remainderTokens ->
                        impl remainderParseFunctions nodes remainderTokens
                    | ParseResult.NoMatch, _ ->
                        ParseResult.NoMatch, tokens

                | [] ->
                    nodes
                    |> NodeValue.Children
                    |> Node.Anonymous
                    |> ParseResult.Keep, remainderTokens

            impl parseFunctions [] tokens

    let parseAtLeastOneFunction (parseFunction : ParseFunction) : ParseFunction =
        let sequenceFunction =
            parseSequenceFunction
                [ parseFunction
                  parseMultipleFunction parseFunction ]

        let cleanUpMessyResult messyResult =
            match messyResult with
            | ParseResult.Keep messyNode ->
                let getChildren messyNode =
                    match messyNode with
                    | Node.Anonymous (NodeValue.Children children) ->
                        children
                    | _ -> failwith "Can't extract children info from this messyNode"

                let split children =
                    match children with
                    | a::b::_ ->
                        (a,b)
                    | _ -> failwith "Can't split children"

                let (headNode, tailMess) = split (getChildren messyNode)
                let tailNodes = getChildren tailMess

                headNode :: tailNodes
                |> NodeValue.Children
                |> Node.Anonymous
                |> ParseResult.Keep

            | _ -> messyResult

        fun tokens ->
            let (messyResult, remainderTokens) = sequenceFunction tokens
            cleanUpMessyResult messyResult, remainderTokens
