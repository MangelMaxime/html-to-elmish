module HtmlParser.Parser.Tokenizer

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
    | ExclamationMark
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
