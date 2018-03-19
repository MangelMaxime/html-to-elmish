(function(l, i, v, e) { v = l.createElement(i); v.async = 1; v.src = '//' + (location.host || 'localhost').split(':')[0] + ':35729/livereload.js?snipver=1'; e = l.getElementsByTagName(i)[0]; e.parentNode.insertBefore(v, e)})(document, 'script');
(function (global, factory) {
	typeof exports === 'object' && typeof module !== 'undefined' ? factory() :
	typeof define === 'function' && define.amd ? define(factory) :
	(factory());
}(this, (function () { 'use strict';

function padWithZeros(i, length) {
    let str = i.toString(10);
    while (str.length < length) {
        str = "0" + str;
    }
    return str;
}
function offsetToString(offset) {
    const isMinus = offset < 0;
    offset = Math.abs(offset);
    const hours = ~~(offset / 3600000);
    const minutes = (offset % 3600000) / 60000;
    return (isMinus ? "-" : "+") +
        padWithZeros(hours, 2) + ":" +
        padWithZeros(minutes, 2);
}
function toHalfUTCString(date, half) {
    const str = date.toISOString();
    return half === "first"
        ? str.substring(0, str.indexOf("T"))
        : str.substring(str.indexOf("T") + 1, str.length - 1);
}
function toISOString(d, utc) {
    if (utc) {
        return d.toISOString();
    }
    else {
        // JS Date is always local
        const printOffset = d.kind == null ? true : d.kind === 2 /* Local */;
        return padWithZeros(d.getFullYear(), 4) + "-" +
            padWithZeros(d.getMonth() + 1, 2) + "-" +
            padWithZeros(d.getDate(), 2) + "T" +
            padWithZeros(d.getHours(), 2) + ":" +
            padWithZeros(d.getMinutes(), 2) + ":" +
            padWithZeros(d.getSeconds(), 2) + "." +
            padWithZeros(d.getMilliseconds(), 3) +
            (printOffset ? offsetToString(d.getTimezoneOffset() * -60000) : "");
    }
}
function toISOStringWithOffset(dateWithOffset, offset) {
    const str = dateWithOffset.toISOString();
    return str.substring(0, str.length - 1) + offsetToString(offset);
}
function toStringWithCustomFormat(date, format, utc) {
    return format.replace(/(\w)\1*/g, (match) => {
        let rep = match;
        switch (match.substring(0, 1)) {
            case "y":
                const y = utc ? date.getUTCFullYear() : date.getFullYear();
                rep = match.length < 4 ? y % 100 : y;
                break;
            case "M":
                rep = (utc ? date.getUTCMonth() : date.getMonth()) + 1;
                break;
            case "d":
                rep = utc ? date.getUTCDate() : date.getDate();
                break;
            case "H":
                rep = utc ? date.getUTCHours() : date.getHours();
                break;
            case "h":
                const h = utc ? date.getUTCHours() : date.getHours();
                rep = h > 12 ? h % 12 : h;
                break;
            case "m":
                rep = utc ? date.getUTCMinutes() : date.getMinutes();
                break;
            case "s":
                rep = utc ? date.getUTCSeconds() : date.getSeconds();
                break;
        }
        if (rep !== match && rep < 10 && match.length > 1) {
            rep = "0" + rep;
        }
        return rep;
    });
}
function toStringWithOffset(date, format) {
    const d = new Date(date.getTime() + date.offset);
    if (!format) {
        return d.toISOString().replace(/\.\d+/, "").replace(/[A-Z]|\.\d+/g, " ") + offsetToString(date.offset);
    }
    else if (format.length === 1) {
        switch (format) {
            case "D":
            case "d": return toHalfUTCString(d, "first");
            case "T":
            case "t": return toHalfUTCString(d, "second");
            case "O":
            case "o": return toISOStringWithOffset(d, date.offset);
            default: throw new Error("Unrecognized Date print format");
        }
    }
    else {
        return toStringWithCustomFormat(d, format, true);
    }
}
function toStringWithKind(date, format) {
    const utc = date.kind === 1 /* UTC */;
    if (!format) {
        return utc ? date.toUTCString() : date.toLocaleString();
    }
    else if (format.length === 1) {
        switch (format) {
            case "D":
            case "d":
                return utc ? toHalfUTCString(date, "first") : date.toLocaleDateString();
            case "T":
            case "t":
                return utc ? toHalfUTCString(date, "second") : date.toLocaleTimeString();
            case "O":
            case "o":
                return toISOString(date, utc);
            default:
                throw new Error("Unrecognized Date print format");
        }
    }
    else {
        return toStringWithCustomFormat(date, format, utc);
    }
}
function toString(date, format) {
    return date.offset != null
        ? toStringWithOffset(date, format)
        : toStringWithKind(date, format);
}
function compare(x, y) {
    const xtime = x.getTime();
    const ytime = y.getTime();
    return xtime === ytime ? 0 : (xtime < ytime ? -1 : 1);
}

const types = new Map();
function setType(fullName, cons) {
    types.set(fullName, cons);
}
var FSymbol = {
    reflection: Symbol("reflection"),
};

class NonDeclaredType {
    constructor(kind, definition, generics) {
        this.kind = kind;
        this.definition = definition;
        this.generics = generics;
    }
    Equals(other) {
        if (this.kind === other.kind && this.definition === other.definition) {
            return typeof this.generics === "object"
                // equalsRecords should also work for Type[] (tuples)
                ? equalsRecords(this.generics, other.generics)
                : this.generics === other.generics;
        }
        return false;
    }
}
function Tuple(types) {
    return new NonDeclaredType("Tuple", null, types);
}
function makeGeneric(typeDef, genArgs) {
    return new NonDeclaredType("GenericType", typeDef, genArgs);
}
function hasInterface(obj, interfaceName) {
    if (interfaceName === "System.Collections.Generic.IEnumerable") {
        return typeof obj[Symbol.iterator] === "function";
    }
    else if (typeof obj[FSymbol.reflection] === "function") {
        const interfaces = obj[FSymbol.reflection]().interfaces;
        return Array.isArray(interfaces) && interfaces.indexOf(interfaceName) > -1;
    }
    return false;
}
/**
 * Returns:
 * - Records: array with names of fields
 * - Classes: array with names of getters
 * - Nulls and unions: empty array
 * - JS Objects: The result of calling Object.getOwnPropertyNames
 */
function getPropertyNames(obj) {
    if (obj == null) {
        return [];
    }
    const propertyMap = typeof obj[FSymbol.reflection] === "function" ? obj[FSymbol.reflection]().properties || [] : obj;
    return Object.getOwnPropertyNames(propertyMap);
}
function toString$1(obj, quoteStrings = false) {
    function isObject(x) {
        return x !== null && typeof x === "object" && !(x instanceof Number)
            && !(x instanceof String) && !(x instanceof Boolean);
    }
    if (obj == null || typeof obj === "number") {
        return String(obj);
    }
    if (typeof obj === "string") {
        return quoteStrings ? JSON.stringify(obj) : obj;
    }
    if (obj instanceof Date) {
        return toString(obj);
    }
    if (typeof obj.ToString === "function") {
        return obj.ToString();
    }
    if (hasInterface(obj, "FSharpUnion")) {
        const info = obj[FSymbol.reflection]();
        const uci = info.cases[obj.tag];
        switch (uci.length) {
            case 1:
                return uci[0];
            case 2:
                // For simplicity let's always use parens, in .NET they're ommitted in some cases
                return uci[0] + " (" + toString$1(obj.data, true) + ")";
            default:
                return uci[0] + " (" + obj.data.map((x) => toString$1(x, true)).join(",") + ")";
        }
    }
    try {
        return JSON.stringify(obj, (k, v) => {
            return v && v[Symbol.iterator] && !Array.isArray(v) && isObject(v) ? Array.from(v)
                : v && typeof v.ToString === "function" ? toString$1(v) : v;
        });
    }
    catch (err) {
        // Fallback for objects with circular references
        return "{" + Object.getOwnPropertyNames(obj).map((k) => k + ": " + String(obj[k])).join(", ") + "}";
    }
}
function hash(x) {
    if (typeof x === typeof 1) {
        return x * 2654435761 | 0;
    }
    if (x != null && typeof x.GetHashCode === "function") {
        return x.GetHashCode();
    }
    else {
        const s = toString$1(x);
        let h = 5381;
        let i = 0;
        const len = s.length;
        while (i < len) {
            h = (h * 33) ^ s.charCodeAt(i++);
        }
        return h;
    }
}
function equals$1(x, y) {
    // Optimization if they are referencially equal
    if (x === y) {
        return true;
    }
    else if (x == null) {
        return y == null;
    }
    else if (y == null) {
        return false;
    }
    else if (typeof x !== "object" || typeof y !== "object") {
        return x === y;
        // Equals override or IEquatable implementation
    }
    else if (typeof x.Equals === "function") {
        return x.Equals(y);
    }
    else if (typeof y.Equals === "function") {
        return y.Equals(x);
    }
    else if (Object.getPrototypeOf(x) !== Object.getPrototypeOf(y)) {
        return false;
    }
    else if (Array.isArray(x)) {
        if (x.length !== y.length) {
            return false;
        }
        for (let i = 0; i < x.length; i++) {
            if (!equals$1(x[i], y[i])) {
                return false;
            }
        }
        return true;
    }
    else if (ArrayBuffer.isView(x)) {
        if (x.byteLength !== y.byteLength) {
            return false;
        }
        const dv1 = new DataView(x.buffer);
        const dv2 = new DataView(y.buffer);
        for (let i = 0; i < x.byteLength; i++) {
            if (dv1.getUint8(i) !== dv2.getUint8(i)) {
                return false;
            }
        }
        return true;
    }
    else if (x instanceof Date) {
        return x.getTime() === y.getTime();
    }
    else {
        return false;
    }
}
function comparePrimitives(x, y) {
    return x === y ? 0 : (x < y ? -1 : 1);
}
function compare$1(x, y) {
    // Optimization if they are referencially equal
    if (x === y) {
        return 0;
    }
    else if (x == null) {
        return y == null ? 0 : -1;
    }
    else if (y == null) {
        return 1; // everything is bigger than null
    }
    else if (typeof x !== "object" || typeof y !== "object") {
        return x === y ? 0 : (x < y ? -1 : 1);
        // Some types (see Long.ts) may just implement the function and not the interface
        // else if (hasInterface(x, "System.IComparable"))
    }
    else if (typeof x.CompareTo === "function") {
        return x.CompareTo(y);
    }
    else if (typeof y.CompareTo === "function") {
        return y.CompareTo(x) * -1;
    }
    else if (Object.getPrototypeOf(x) !== Object.getPrototypeOf(y)) {
        return -1;
    }
    else if (Array.isArray(x)) {
        if (x.length !== y.length) {
            return x.length < y.length ? -1 : 1;
        }
        for (let i = 0, j = 0; i < x.length; i++) {
            j = compare$1(x[i], y[i]);
            if (j !== 0) {
                return j;
            }
        }
        return 0;
    }
    else if (ArrayBuffer.isView(x)) {
        if (x.byteLength !== y.byteLength) {
            return x.byteLength < y.byteLength ? -1 : 1;
        }
        const dv1 = new DataView(x.buffer);
        const dv2 = new DataView(y.buffer);
        for (let i = 0, b1 = 0, b2 = 0; i < x.byteLength; i++) {
            b1 = dv1.getUint8(i), b2 = dv2.getUint8(i);
            if (b1 < b2) {
                return -1;
            }
            if (b1 > b2) {
                return 1;
            }
        }
        return 0;
    }
    else if (x instanceof Date) {
        return compare(x, y);
    }
    else if (typeof x === "object") {
        const xhash = hash(x);
        const yhash = hash(y);
        if (xhash === yhash) {
            return equals$1(x, y) ? 0 : -1;
        }
        else {
            return xhash < yhash ? -1 : 1;
        }
    }
    else {
        return x < y ? -1 : 1;
    }
}
function equalsRecords(x, y) {
    // Optimization if they are referencially equal
    if (x === y) {
        return true;
    }
    else {
        const keys = getPropertyNames(x);
        for (const key of keys) {
            if (!equals$1(x[key], y[key])) {
                return false;
            }
        }
        return true;
    }
}
function compareRecords(x, y) {
    // Optimization if they are referencially equal
    if (x === y) {
        return 0;
    }
    else {
        const keys = getPropertyNames(x);
        for (const key of keys) {
            const res = compare$1(x[key], y[key]);
            if (res !== 0) {
                return res;
            }
        }
        return 0;
    }
}
function compareUnions(x, y) {
    if (x === y) {
        return 0;
    }
    else {
        const res = x.tag < y.tag ? -1 : (x.tag > y.tag ? 1 : 0);
        return res !== 0 ? res : compare$1(x.data, y.data);
    }
}

class Some {
    constructor(value) {
        this.value = value;
    }
    // We don't prefix it with "Some" for consistency with erased options
    ToString() {
        return toString$1(this.value);
    }
    Equals(other) {
        if (other == null) {
            return false;
        }
        else {
            return equals$1(this.value, other instanceof Some
                ? other.value : other);
        }
    }
    CompareTo(other) {
        if (other == null) {
            return 1;
        }
        else {
            return compare$1(this.value, other instanceof Some
                ? other.value : other);
        }
    }
}
function getValue(x, acceptNull) {
    if (x == null) {
        if (!acceptNull) {
            throw new Error("Option has no value");
        }
        return null;
    }
    else {
        return x instanceof Some ? x.value : x;
    }
}

// This module is split from List.ts to prevent cyclic dependencies
function ofArray(args, base) {
    let acc = base || new List();
    for (let i = args.length - 1; i >= 0; i--) {
        acc = new List(args[i], acc);
    }
    return acc;
}
class List {
    constructor(head, tail) {
        this.head = head;
        this.tail = tail;
    }
    ToString() {
        return "[" + Array.from(this).map((x) => toString$1(x)).join("; ") + "]";
    }
    Equals(other) {
        // Optimization if they are referencially equal
        if (this === other) {
            return true;
        }
        else {
            let cur1 = this;
            let cur2 = other;
            while (equals$1(cur1.head, cur2.head)) {
                cur1 = cur1.tail;
                cur2 = cur2.tail;
                if (cur1 == null) {
                    return cur2 == null;
                }
            }
            return false;
        }
    }
    CompareTo(other) {
        // Optimization if they are referencially equal
        if (this === other) {
            return 0;
        }
        else {
            let cur1 = this;
            let cur2 = other;
            let res = compare$1(cur1.head, cur2.head);
            while (res === 0) {
                cur1 = cur1.tail;
                cur2 = cur2.tail;
                if (cur1 == null) {
                    return cur2 == null ? 0 : -1;
                }
                res = compare$1(cur1.head, cur2.head);
            }
            return res;
        }
    }
    get length() {
        let cur = this;
        let acc = 0;
        while (cur.tail != null) {
            cur = cur.tail;
            acc++;
        }
        return acc;
    }
    [Symbol.iterator]() {
        let cur = this;
        return {
            next: () => {
                const tmp = cur;
                cur = cur.tail;
                return { done: tmp.tail == null, value: tmp.head };
            },
        };
    }
    //   append(ys: List<T>): List<T> {
    //     return append(this, ys);
    //   }
    //   choose<U>(f: (x: T) => U, xs: List<T>): List<U> {
    //     return choose(f, this);
    //   }
    //   collect<U>(f: (x: T) => List<U>): List<U> {
    //     return collect(f, this);
    //   }
    //   filter(f: (x: T) => boolean): List<T> {
    //     return filter(f, this);
    //   }
    //   where(f: (x: T) => boolean): List<T> {
    //     return filter(f, this);
    //   }
    //   map<U>(f: (x: T) => U): List<U> {
    //     return map(f, this);
    //   }
    //   mapIndexed<U>(f: (i: number, x: T) => U): List<U> {
    //     return mapIndexed(f, this);
    //   }
    //   partition(f: (x: T) => boolean): [List<T>, List<T>] {
    //     return partition(f, this) as [List<T>, List<T>];
    //   }
    //   reverse(): List<T> {
    //     return reverse(this);
    //   }
    //   slice(lower: number, upper: number): List<T> {
    //     return slice(lower, upper, this);
    //   }
    [FSymbol.reflection]() {
        return {
            type: "Microsoft.FSharp.Collections.FSharpList",
            interfaces: ["System.IEquatable", "System.IComparable"],
        };
    }
}

function toList(xs) {
    return foldBack((x, acc) => new List(x, acc), xs, new List());
}
function fold(f, acc, xs) {
    if (Array.isArray(xs) || ArrayBuffer.isView(xs)) {
        return xs.reduce(f, acc);
    }
    else {
        let cur;
        for (let i = 0, iter = xs[Symbol.iterator]();; i++) {
            cur = iter.next();
            if (cur.done) {
                break;
            }
            acc = f(acc, cur.value, i);
        }
        return acc;
    }
}
function foldBack(f, xs, acc) {
    const arr = Array.isArray(xs) || ArrayBuffer.isView(xs) ? xs : Array.from(xs);
    for (let i = arr.length - 1; i >= 0; i--) {
        acc = f(arr[i], acc, i);
    }
    return acc;
}

function append$1(xs, ys) {
    return fold((acc, x) => new List(x, acc), ys, reverse$1(xs));
}
function reverse$1(xs) {
    return fold((acc, x) => new List(x, acc), new List(), xs);
}
function head$1(xs) {
    if (xs.head !== undefined) {
        return xs.head;
    }
    else {
        throw new Error("The input list was empty.");
    }
}

function create$2(pattern, options) {
    let flags = "g";
    flags += options & 1 ? "i" : "";
    flags += options & 2 ? "m" : "";
    return new RegExp(pattern, flags);
}
function matches(str, pattern, options = 0) {
    let reg;
    reg = str instanceof RegExp
        ? (reg = str, str = pattern, reg.lastIndex = options, reg)
        : reg = create$2(pattern, options);
    if (!reg.global) {
        throw new Error("Non-global RegExp"); // Prevent infinite loop
    }
    let m = reg.exec(str);
    const matches = [];
    while (m !== null) {
        matches.push(m);
        m = reg.exec(str);
    }
    return matches;
}
function replace(reg, input, replacement, limit, offset = 0) {
    function replacer() {
        let res = arguments[0];
        if (limit !== 0) {
            limit--;
            const match = [];
            const len = arguments.length;
            for (let i = 0; i < len - 2; i++) {
                match.push(arguments[i]);
            }
            match.index = arguments[len - 2];
            match.input = arguments[len - 1];
            res = replacement(match);
        }
        return res;
    }
    if (typeof reg === "string") {
        const tmp = reg;
        reg = create$2(input, limit);
        input = tmp;
        limit = undefined;
    }
    if (typeof replacement === "function") {
        limit = limit == null ? -1 : limit;
        return input.substring(0, offset) + input.substring(offset).replace(reg, replacer);
    }
    else {
        // $0 doesn't work with JS regex, see #1155
        replacement = replacement.replace(/\$0/g, (s) => "$&");
        if (limit != null) {
            let m;
            const sub1 = input.substring(offset);
            const _matches = matches(reg, sub1);
            const sub2 = matches.length > limit ? (m = _matches[limit - 1], sub1.substring(0, m.index + m[0].length)) : sub1;
            return input.substring(0, offset) + sub2.replace(reg, replacement)
                + input.substring(offset + sub2.length);
        }
        else {
            return input.replace(reg, replacement);
        }
    }
}

function CurriedLambda(f, expectedArgsLength) {
    if (f.curried === true) {
        return f;
    }
    const curriedFn = (...args) => {
        const args2 = args.map((x) => typeof x === "function" ? CurriedLambda(x) : x);
        const actualArgsLength = Math.max(args2.length, 1);
        expectedArgsLength = Math.max(expectedArgsLength || f.length, 1);
        if (actualArgsLength >= expectedArgsLength) {
            const restArgs = args2.splice(expectedArgsLength);
            const res = f(...args2);
            if (typeof res === "function") {
                const newLambda = CurriedLambda(res);
                return restArgs.length === 0 ? newLambda : newLambda(...restArgs);
            }
            else {
                return res;
            }
        }
        else {
            return CurriedLambda((...args3) => {
                return f(...args2.concat(args3));
            }, expectedArgsLength - actualArgsLength);
        }
    };
    curriedFn.curried = true;
    return curriedFn;
}

const Lexer = function (__exports) {
    const TokenType = __exports.TokenType = class TokenType {
        constructor(tag) {
            this.tag = tag | 0;
        }

        [FSymbol.reflection]() {
            return {
                type: "HtmlConverter.Parser.Lexer.TokenType",
                interfaces: ["FSharpUnion", "System.IEquatable", "System.IComparable"],
                cases: [["OpeningComment"], ["ClosingComment"], ["LeftAngleBracket"], ["RightAngleBracket"], ["EqualSign"], ["DoubleQuoteMark"], ["SingleQuoteMark"], ["ForwardSlash"], ["Dash"], ["Whitespace"], ["Word"]]
            };
        }

        Equals(other) {
            return this.tag === other.tag;
        }

        CompareTo(other) {
            return comparePrimitives(this.tag, other.tag);
        }

    };
    setType("HtmlConverter.Parser.Lexer.TokenType", TokenType);
    const reservedCharTokenLookup = __exports.reservedCharTokenLookup = ofArray([[new TokenType(2), "<"], [new TokenType(3), ">"], [new TokenType(4), "="], [new TokenType(5), "\""], [new TokenType(6), "'"], [new TokenType(7), "/"], [new TokenType(8), "-"]]);
    const commentSequences = __exports.commentSequences = ofArray([[new TokenType(0), "<!--"], [new TokenType(1), "-->"]]);
    const wordRegex = __exports.wordRegex = "[^<>=\"'\\/-\\s]+";
    const doubleQuotedStringRegex = __exports.doubleQuotedStringRegex = "[^\\\\\"]+";
    const wildcards = __exports.wildcards = ofArray([[new TokenType(9), "(\\s)+"], [new TokenType(10), wordRegex]]);
    const tokenizerGrammar = __exports.tokenizerGrammar = append$1(commentSequences, append$1(reservedCharTokenLookup, wildcards));

    const consumeToken = __exports.consumeToken = function (_arg1_0, _arg1_1, str) {
        const _arg1 = [_arg1_0, _arg1_1];
        const regex = create$2("^" + _arg1[1]);
        const matches$$1 = toList(matches(regex, str));

        if (matches$$1.tail != null) {
            const token = [_arg1[0], matches$$1.head[0]];
            const remainder = replace(regex, str, "", 1);
            return [token, remainder];
        } else {
            return null;
        }
    };

    const consumeFirstTokenMatch = __exports.consumeFirstTokenMatch = function (tokenRecipes, str) {
        consumeFirstTokenMatch: while (true) {
            if (tokenRecipes.tail != null) {
                const matchValue = consumeToken(tokenRecipes.head[0], tokenRecipes.head[1], str);

                if (matchValue != null) {
                    return getValue(matchValue);
                } else {
                    tokenRecipes = tokenRecipes.tail;
                    str = str;
                    continue consumeFirstTokenMatch;
                }
            } else {
                return null;
            }
        }
    };

    const tokenize = __exports.tokenize = function (str) {
        const internalTokenize = function (accTokens, reminderString) {
            internalTokenize: while (true) {
                const matchValue = consumeFirstTokenMatch(tokenizerGrammar, reminderString);

                if (matchValue != null) {
                    const token = getValue(matchValue)[0];
                    const remainderRemainderString = getValue(matchValue)[1];
                    accTokens = append$1(accTokens, ofArray([token]));
                    reminderString = remainderRemainderString;
                    continue internalTokenize;
                } else {
                    return accTokens;
                }
            }
        };

        return internalTokenize(new List(), str);
    };

    return __exports;
}({});

class _Node {
    constructor(tag, data) {
        this.tag = tag | 0;
        this.data = data;
    }

    [FSymbol.reflection]() {
        return {
            type: "HtmlConverter.Parser.Node",
            interfaces: ["FSharpUnion", "System.IEquatable", "System.IComparable"],
            cases: [["Named", NodeData], ["Anonymous", NodeValue]]
        };
    }

    Equals(other) {
        return this === other || this.tag === other.tag && equals$1(this.data, other.data);
    }

    CompareTo(other) {
        return compareUnions(this, other) | 0;
    }

}
setType("HtmlConverter.Parser.Node", _Node);
class NodeValue {
    constructor(tag, data) {
        this.tag = tag | 0;
        this.data = data;
    }

    [FSymbol.reflection]() {
        return {
            type: "HtmlConverter.Parser.NodeValue",
            interfaces: ["FSharpUnion", "System.IEquatable", "System.IComparable"],
            cases: [["Leaf", "string"], ["Children", makeGeneric(List, {
                T: _Node
            })]]
        };
    }

    Equals(other) {
        return this === other || this.tag === other.tag && equals$1(this.data, other.data);
    }

    CompareTo(other) {
        return compareUnions(this, other) | 0;
    }

}
setType("HtmlConverter.Parser.NodeValue", NodeValue);
class NodeData {
    constructor(name, value) {
        this.Name = name;
        this.Value = value;
    }

    [FSymbol.reflection]() {
        return {
            type: "HtmlConverter.Parser.NodeData",
            interfaces: ["FSharpRecord", "System.IEquatable", "System.IComparable"],
            properties: {
                Name: "string",
                Value: NodeValue
            }
        };
    }

    Equals(other) {
        return equalsRecords(this, other);
    }

    CompareTo(other) {
        return compareRecords(this, other) | 0;
    }

}
setType("HtmlConverter.Parser.NodeData", NodeData);
class ParseResult {
    constructor(tag, data) {
        this.tag = tag | 0;
        this.data = data;
    }

    [FSymbol.reflection]() {
        return {
            type: "HtmlConverter.Parser.ParseResult",
            interfaces: ["FSharpUnion", "System.IEquatable", "System.IComparable"],
            cases: [["Keep", _Node], ["Ignore"], ["NoMatch"]]
        };
    }

    Equals(other) {
        return this === other || this.tag === other.tag && equals$1(this.data, other.data);
    }

    CompareTo(other) {
        return compareUnions(this, other) | 0;
    }

}
setType("HtmlConverter.Parser.ParseResult", ParseResult);
class ConsumeTokenResult {
    constructor(tag, data) {
        this.tag = tag | 0;
        this.data = data;
    }

    [FSymbol.reflection]() {
        return {
            type: "HtmlConverter.Parser.ConsumeTokenResult",
            interfaces: ["FSharpUnion", "System.IEquatable", "System.IComparable"],
            cases: [["Match", Tuple(["string", makeGeneric(List, {
                T: Tuple([Lexer.TokenType, "string"])
            })])], ["NotMach", makeGeneric(List, {
                T: Tuple([Lexer.TokenType, "string"])
            })]]
        };
    }

    Equals(other) {
        return this === other || this.tag === other.tag && equals$1(this.data, other.data);
    }

    CompareTo(other) {
        return compareUnions(this, other) | 0;
    }

}
setType("HtmlConverter.Parser.ConsumeTokenResult", ConsumeTokenResult);
function parseToken(expectedType, tokens) {
    if (tokens.tail == null) {
        return new ConsumeTokenResult(1, new List());
    } else {
        const tokenValue = tokens.head[1];
        const tokenType = tokens.head[0];

        if (tokenType.Equals(expectedType)) {
            return new ConsumeTokenResult(0, [tokenValue, tokens.tail]);
        } else {
            return new ConsumeTokenResult(1, tokens);
        }
    }
}
function parseTokenIgnore(expectedType, tokens) {
    const matchValue = parseToken(expectedType, tokens);

    if (matchValue.tag === 1) {
        return [new ParseResult(2), matchValue.data];
    } else {
        const remainderTokens = matchValue.data[1];
        return [new ParseResult(1), remainderTokens];
    }
}
function parseTokenKeep(expectedType, tokens) {
    const matchValue = parseToken(expectedType, tokens);

    if (matchValue.tag === 1) {
        return [new ParseResult(2), matchValue.data];
    } else {
        const tokenValue = matchValue.data[0];
        const remainderTokens = matchValue.data[1];
        return [new ParseResult(0, new _Node(1, new NodeValue(0, tokenValue))), remainderTokens];
    }
}
function named(name, parseFunction, tokens) {
    const matchValue = parseFunction(tokens);
    const $var1 = matchValue[0].tag === 0 ? matchValue[0].data.tag === 1 ? [0, matchValue[0].data.data, matchValue[1]] : [1, matchValue] : [1, matchValue];

    switch ($var1[0]) {
        case 0:
            return [new ParseResult(0, new _Node(0, new NodeData(name, $var1[1]))), $var1[2]];

        case 1:
            return $var1[1];
    }
}
function optional(parseFunction, tokens) {
    const matchValue = parseFunction(tokens);

    if (matchValue[0].tag === 2) {
        return [new ParseResult(1), matchValue[1]];
    } else {
        return matchValue;
    }
}
function parseAnyFunction(parseFunctions) {
    var impl;
    return CurriedLambda((impl = function (parseFunctions_1, tokens) {
        if (parseFunctions_1.tail == null) {
            return [new ParseResult(2), tokens];
        } else {
            const matchValue = parseFunctions_1.head(tokens);

            if (matchValue[0].tag === 2) {
                return impl(parseFunctions_1.tail, tokens);
            } else {
                return matchValue;
            }
        }
    }, CurriedLambda(impl)(parseFunctions)));
}
function parseMultipleFunction(repeatedFunction) {
    var impl;
    return CurriedLambda((impl = function (nodes, remainderTokens) {
        const matchValue = repeatedFunction(remainderTokens);

        if (matchValue[0].tag === 1) {
            return impl(nodes, matchValue[1]);
        } else if (matchValue[0].tag === 2) {
            return [nodes, matchValue[1]];
        } else {
            const node = matchValue[0].data;
            return impl(append$1(nodes, ofArray([node])), matchValue[1]);
        }
    }, function (_arg1) {
        if (_arg1.tail == null) {
            return [new ParseResult(0, new _Node(1, new NodeValue(1, new List()))), new List()];
        } else {
            const patternInput = impl(new List(), _arg1);
            return [new ParseResult(0, new _Node(1, new NodeValue(1, patternInput[0]))), patternInput[1]];
        }
    }));
}
function parseSequenceFunction(parseFunctions, _arg1) {
    if (_arg1.tail == null) {
        return [new ParseResult(2), new List()];
    } else {
        const impl = function (parseFunctions_1, nodes, remainderTokens) {
            impl: while (true) {
                if (parseFunctions_1.tail == null) {
                    return [new ParseResult(0, new _Node(1, new NodeValue(1, nodes))), remainderTokens];
                } else {
                    const matchValue = parseFunctions_1.head(remainderTokens);

                    if (matchValue[0].tag === 1) {
                        parseFunctions_1 = parseFunctions_1.tail;
                        nodes = nodes;
                        remainderTokens = matchValue[1];
                        continue impl;
                    } else if (matchValue[0].tag === 2) {
                        return [new ParseResult(2), _arg1];
                    } else {
                        const node = matchValue[0].data;
                        parseFunctions_1 = parseFunctions_1.tail;
                        nodes = append$1(nodes, ofArray([node]));
                        remainderTokens = matchValue[1];
                        continue impl;
                    }
                }
            }
        };

        return impl(parseFunctions, new List(), _arg1);
    }
}
function parseAtLeastOneFunction(parseFunction) {
    var sequenceFunction;
    var cleanUpMessyResult;
    var parseFunctions;
    return CurriedLambda((sequenceFunction = (parseFunctions = ofArray([parseFunction, parseMultipleFunction(parseFunction)]), function (_arg1) {
        return parseSequenceFunction(parseFunctions, _arg1);
    }), cleanUpMessyResult = function (messyResult) {
        if (messyResult.tag === 0) {
            const getChildren = function (messyNode) {
                const $var2 = messyNode.tag === 1 ? messyNode.data.tag === 1 ? [0, messyNode.data.data] : [1] : [1];

                switch ($var2[0]) {
                    case 0:
                        return $var2[1];

                    case 1:
                        throw new Error("Can't extract children info from this messyNode");
                }
            };

            const split$$1 = function (children) {
                const $var3 = children.tail != null ? children.tail.tail != null ? [0, children.head, children.tail.head] : [1] : [1];

                switch ($var3[0]) {
                    case 0:
                        return [$var3[1], $var3[2]];

                    case 1:
                        throw new Error("Can't split children");
                }
            };

            const patternInput = split$$1(getChildren(messyResult.data));
            const tailNodes = getChildren(patternInput[1]);
            return new ParseResult(0, new _Node(1, new NodeValue(1, new List(patternInput[0], tailNodes))));
        } else {
            return messyResult;
        }
    }, function (tokens) {
        const patternInput_1 = sequenceFunction(tokens);
        return [cleanUpMessyResult(patternInput_1[0]), patternInput_1[1]];
    }));
}

function tests(_arg1) {
    QUnit.module("HtmlConverter.Parser.Lexer");
    QUnit.test("consumeToken `<` token", function (test) {
        (function (_arg2) {
            if (_arg2 != null) {
                const token = getValue(_arg2)[0];
                const remainderString = getValue(_arg2)[1];

                (function (arg00, arg10) {
                    test.deepEqual(arg00, arg10);
                })([new Lexer.TokenType(2), "<"], token);

                (function (arg00_1, arg10_1) {
                    test.equal(arg00_1, arg10_1);
                })("br/>", remainderString);
            } else {
                test.ok(false, "Token `<` not consumed");
            }
        })(Lexer.consumeToken(new Lexer.TokenType(2), "<", "<br/>"));
    });
    QUnit.test("consumeToken `>` token", function (test_1) {
        (function (_arg3) {
            if (_arg3 != null) {
                const token_1 = getValue(_arg3)[0];
                const remainderString_1 = getValue(_arg3)[1];

                (function (arg00_2, arg10_2) {
                    test_1.deepEqual(arg00_2, arg10_2);
                })([new Lexer.TokenType(3), ">"], token_1);

                (function (arg00_3, arg10_3) {
                    test_1.equal(arg00_3, arg10_3);
                })("some text", remainderString_1);
            } else {
                test_1.ok(false, "Token `>` not consumed");
            }
        })(Lexer.consumeToken(new Lexer.TokenType(3), ">", ">some text"));
    });
    QUnit.test("consumeToken `=` token", function (test_2) {
        (function (_arg4) {
            if (_arg4 != null) {
                const token_2 = getValue(_arg4)[0];
                const remainderString_2 = getValue(_arg4)[1];

                (function (arg00_4, arg10_4) {
                    test_2.deepEqual(arg00_4, arg10_4);
                })([new Lexer.TokenType(4), "="], token_2);

                (function (arg00_5, arg10_5) {
                    test_2.equal(arg00_5, arg10_5);
                })("'my-value'", remainderString_2);
            } else {
                test_2.ok(false, "Token `=` not consumed");
            }
        })(Lexer.consumeToken(new Lexer.TokenType(4), "=", "='my-value'"));
    });
    QUnit.test("consumeToken `\"` token", function (test_3) {
        (function (_arg5) {
            if (_arg5 != null) {
                const token_3 = getValue(_arg5)[0];
                const remainderString_3 = getValue(_arg5)[1];

                (function (arg00_6, arg10_6) {
                    test_3.deepEqual(arg00_6, arg10_6);
                })([new Lexer.TokenType(5), "\""], token_3);

                (function (arg00_7, arg10_7) {
                    test_3.equal(arg00_7, arg10_7);
                })("my-value\"", remainderString_3);
            } else {
                test_3.ok(false, "Token `\"` not consumed");
            }
        })(Lexer.consumeToken(new Lexer.TokenType(5), "\"", "\"my-value\""));
    });
    QUnit.test("consumeToken `'` token", function (test_4) {
        (function (_arg6) {
            if (_arg6 != null) {
                const token_4 = getValue(_arg6)[0];
                const remainderString_4 = getValue(_arg6)[1];

                (function (arg00_8, arg10_8) {
                    test_4.deepEqual(arg00_8, arg10_8);
                })([new Lexer.TokenType(6), "'"], token_4);

                (function (arg00_9, arg10_9) {
                    test_4.equal(arg00_9, arg10_9);
                })("my-value'", remainderString_4);
            } else {
                test_4.ok(false, "Token `'` not consumed");
            }
        })(Lexer.consumeToken(new Lexer.TokenType(6), "'", "'my-value'"));
    });
    QUnit.test("consumeToken `/` token", function (test_5) {
        (function (_arg7) {
            if (_arg7 != null) {
                const token_5 = getValue(_arg7)[0];
                const remainderString_5 = getValue(_arg7)[1];

                (function (arg00_10, arg10_10) {
                    test_5.deepEqual(arg00_10, arg10_10);
                })([new Lexer.TokenType(7), "/"], token_5);

                (function (arg00_11, arg10_11) {
                    test_5.equal(arg00_11, arg10_11);
                })(">some text'", remainderString_5);
            } else {
                test_5.ok(false, "Token `/` not consumed");
            }
        })(Lexer.consumeToken(new Lexer.TokenType(7), "/", "/>some text'"));
    });
    QUnit.test("consumeToken `-` token", function (test_6) {
        (function (_arg8) {
            if (_arg8 != null) {
                const token_6 = getValue(_arg8)[0];
                const remainderString_6 = getValue(_arg8)[1];

                (function (arg00_12, arg10_12) {
                    test_6.deepEqual(arg00_12, arg10_12);
                })([new Lexer.TokenType(8), "-"], token_6);

                (function (arg00_13, arg10_13) {
                    test_6.equal(arg00_13, arg10_13);
                })("value", remainderString_6);
            } else {
                test_6.ok(false, "Token `-` not consumed");
            }
        })(Lexer.consumeToken(new Lexer.TokenType(8), "-", "-value"));
    });
    QUnit.test("consumeWhitespace", function (test_7) {
        var tupledArg;

        (function (_arg9) {
            if (_arg9 != null) {
                const token_7 = getValue(_arg9)[0];
                const remainderString_7 = getValue(_arg9)[1];

                (function (arg00_14, arg10_14) {
                    test_7.deepEqual(arg00_14, arg10_14);
                })([new Lexer.TokenType(9), "    "], token_7);

                (function (arg00_15, arg10_15) {
                    test_7.equal(arg00_15, arg10_15);
                })("test", remainderString_7);
            } else {
                test_7.ok(false, "Whitespace token not consumed");
            }
        })((tupledArg = head$1(Lexer.wildcards), Lexer.consumeToken(tupledArg[0], tupledArg[1], "    test")));
    });
    QUnit.test("consumeFirstTokenMatch OpeningComment", function (test_8) {
        (function (_arg10) {
            if (_arg10 != null) {
                const token_8 = getValue(_arg10)[0];
                const remainderString_8 = getValue(_arg10)[1];

                (function (arg00_16, arg10_16) {
                    test_8.deepEqual(arg00_16, arg10_16);
                })([new Lexer.TokenType(0), "<!--"], token_8);

                (function (arg00_17, arg10_17) {
                    test_8.equal(arg00_17, arg10_17);
                })(" A comment -->", remainderString_8);
            } else {
                test_8.ok(false);
            }
        })(Lexer.consumeFirstTokenMatch(Lexer.tokenizerGrammar, "<!-- A comment -->"));
    });
    QUnit.test("tokenize", function (test_9) {
        const tokens = Lexer.tokenize("<div>Hello, I am a test</div>");
        const expected = ofArray([[new Lexer.TokenType(2), "<"], [new Lexer.TokenType(10), "div"], [new Lexer.TokenType(3), ">"], [new Lexer.TokenType(10), "Hello,"], [new Lexer.TokenType(9), " "], [new Lexer.TokenType(10), "I"], [new Lexer.TokenType(9), " "], [new Lexer.TokenType(10), "am"], [new Lexer.TokenType(9), " "], [new Lexer.TokenType(10), "a"], [new Lexer.TokenType(9), " "], [new Lexer.TokenType(10), "test"], [new Lexer.TokenType(2), "<"], [new Lexer.TokenType(7), "/"], [new Lexer.TokenType(10), "div"], [new Lexer.TokenType(3), ">"]]);

        (function (arg00_18, arg10_18) {
            test_9.deepEqual(arg00_18, arg10_18);
        })(expected, tokens);
    });
}

function tests$1(_arg1) {
    QUnit.module("HtmlConverter.Parser");
    const tokens = Lexer.tokenize("<");
    QUnit.test("parseToken with Math", function (test) {
        (function (arg00, arg10) {
            test.deepEqual(arg00, arg10);
        })(parseToken(new Lexer.TokenType(2), tokens), new ConsumeTokenResult(0, ["<", new List()]));
    });
    QUnit.test("parseToken with no match", function (test_1) {
        (function (arg00_1, arg10_1) {
            test_1.deepEqual(arg00_1, arg10_1);
        })(parseToken(new Lexer.TokenType(3), tokens), new ConsumeTokenResult(1, tokens));
    });
    QUnit.test("parseTokenIgnore", function (test_2) {
        (function (arg00_2, arg10_2) {
            test_2.deepEqual(arg00_2, arg10_2);
        })(parseTokenIgnore(new Lexer.TokenType(2), tokens), [new ParseResult(1), new List()]);
    });
    QUnit.test("parseTokenKeep with Anonymous node", function (test_3) {
        (function (arg00_3, arg10_3) {
            test_3.deepEqual(arg00_3, arg10_3);
        })(parseTokenKeep(new Lexer.TokenType(2), tokens), [new ParseResult(0, new _Node(1, new NodeValue(0, "<"))), new List()]);
    });
    QUnit.test("parseTokenKeep with Named node", function (test_4) {
        var expectedType;

        (function (arg00_4, arg10_4) {
            test_4.deepEqual(arg00_4, arg10_4);
        })(named("LEFT_ANGLE_BRACKET", (expectedType = new Lexer.TokenType(2), function (tokens_1) {
            return parseTokenKeep(expectedType, tokens_1);
        }), tokens), [new ParseResult(0, new _Node(0, new NodeData("LEFT_ANGLE_BRACKET", new NodeValue(0, "<")))), new List()]);
    });
    QUnit.test("parseTokenKeep with Word TokenType", function (test_5) {
        (function (arg00_5, arg10_5) {
            test_5.deepEqual(arg00_5, arg10_5);
        })(parseTokenKeep(new Lexer.TokenType(10), Lexer.tokenize("h1")), [new ParseResult(0, new _Node(1, new NodeValue(0, "h1"))), new List()]);
    });
    QUnit.test("parseOptional with Keep result", function (test_6) {
        var expectedType_1;

        (function (arg00_6, arg10_6) {
            test_6.deepEqual(arg00_6, arg10_6);
        })(optional((expectedType_1 = new Lexer.TokenType(2), function (tokens_2) {
            return parseTokenKeep(expectedType_1, tokens_2);
        }), tokens), [new ParseResult(0, new _Node(1, new NodeValue(0, "<"))), new List()]);
    });
    QUnit.test("parseOptional with Ignore result", function (test_7) {
        var expectedType_2;

        (function (arg00_7, arg10_7) {
            test_7.deepEqual(arg00_7, arg10_7);
        })(optional((expectedType_2 = new Lexer.TokenType(3), function (tokens_3) {
            return parseTokenKeep(expectedType_2, tokens_3);
        }), tokens), [new ParseResult(1), tokens]);
    });
    QUnit.test("parseAnyFunction with at least one match", function (test_8) {
        var expectedType_3;
        var expectedType_4;

        (function (arg00_8, arg10_8) {
            test_8.deepEqual(arg00_8, arg10_8);
        })(parseAnyFunction(ofArray([(expectedType_3 = new Lexer.TokenType(3), function (tokens_4) {
            return parseTokenKeep(expectedType_3, tokens_4);
        }), (expectedType_4 = new Lexer.TokenType(2), function (tokens_5) {
            return parseTokenKeep(expectedType_4, tokens_5);
        })]))(tokens), [new ParseResult(0, new _Node(1, new NodeValue(0, "<"))), new List()]);
    });
    QUnit.test("parseAnyFunction with no match", function (test_9) {
        var expectedType_5;
        var expectedType_6;

        (function (arg00_9, arg10_9) {
            test_9.deepEqual(arg00_9, arg10_9);
        })(parseAnyFunction(ofArray([(expectedType_5 = new Lexer.TokenType(3), function (tokens_6) {
            return parseTokenKeep(expectedType_5, tokens_6);
        }), (expectedType_6 = new Lexer.TokenType(10), function (tokens_7) {
            return parseTokenKeep(expectedType_6, tokens_7);
        })]))(tokens), [new ParseResult(2), tokens]);
    });
    QUnit.test("parseMultipleFunction", function (test_10) {
        var expectedType_7;
        var expectedType_8;

        (function (arg00_10, arg10_10) {
            test_10.deepEqual(arg00_10, arg10_10);
        })(parseMultipleFunction((expectedType_7 = new Lexer.TokenType(2), function (tokens_8) {
            return parseTokenKeep(expectedType_7, tokens_8);
        }))(Lexer.tokenize("<<")), [new ParseResult(0, new _Node(1, new NodeValue(1, ofArray([new _Node(1, new NodeValue(0, "<")), new _Node(1, new NodeValue(0, "<"))])))), new List()]);

        (function (arg00_11, arg10_11) {
            test_10.deepEqual(arg00_11, arg10_11);
        })(parseMultipleFunction((expectedType_8 = new Lexer.TokenType(2), function (tokens_9) {
            return parseTokenKeep(expectedType_8, tokens_9);
        }))(Lexer.tokenize("<div>")), [new ParseResult(0, new _Node(1, new NodeValue(1, ofArray([new _Node(1, new NodeValue(0, "<"))])))), ofArray([[new Lexer.TokenType(10), "div"], [new Lexer.TokenType(3), ">"]])]);
    });
    QUnit.test("parseSequenceFunction", function (test_11) {
        var expectedType_9;
        var expectedType_10;
        var expectedType_11;
        let parseTag;
        const parseFunctions = ofArray([(expectedType_9 = new Lexer.TokenType(2), function (tokens_10) {
            return parseTokenIgnore(expectedType_9, tokens_10);
        }), (expectedType_10 = new Lexer.TokenType(10), function (tokens_11) {
            return parseTokenKeep(expectedType_10, tokens_11);
        }), (expectedType_11 = new Lexer.TokenType(3), function (tokens_12) {
            return parseTokenIgnore(expectedType_11, tokens_12);
        })]);

        parseTag = function (_arg1_1) {
            return parseSequenceFunction(parseFunctions, _arg1_1);
        };

        (function (arg00_12, arg10_12) {
            test_11.deepEqual(arg00_12, arg10_12);
        })(parseTag(Lexer.tokenize("<div>")), [new ParseResult(0, new _Node(1, new NodeValue(1, ofArray([new _Node(1, new NodeValue(0, "div"))])))), new List()]);

        (function (arg00_13, arg10_13) {
            test_11.deepEqual(arg00_13, arg10_13);
        })(parseTag(Lexer.tokenize("<>")), [new ParseResult(2), Lexer.tokenize("<>")]);
    });
    QUnit.test("parseAtLeastOneFunction", function (test_12) {
        var expectedType_12;
        var expectedType_13;
        var expectedType_14;
        const parseAtLeastOneLeftOrRightBracket = parseAtLeastOneFunction(parseAnyFunction(ofArray([(expectedType_12 = new Lexer.TokenType(2), function (tokens_13) {
            return parseTokenKeep(expectedType_12, tokens_13);
        }), (expectedType_13 = new Lexer.TokenType(3), function (tokens_14) {
            return parseTokenKeep(expectedType_13, tokens_14);
        })])));

        (function (arg00_14, arg10_14) {
            test_12.deepEqual(arg00_14, arg10_14);
        })(parseAtLeastOneFunction((expectedType_14 = new Lexer.TokenType(2), function (tokens_15) {
            return parseTokenKeep(expectedType_14, tokens_15);
        }))(Lexer.tokenize("<<<>")), [new ParseResult(0, new _Node(1, new NodeValue(1, ofArray([new _Node(1, new NodeValue(0, "<")), new _Node(1, new NodeValue(0, "<")), new _Node(1, new NodeValue(0, "<"))])))), ofArray([[new Lexer.TokenType(3), ">"]])]);

        (function (arg00_15, arg10_15) {
            test_12.deepEqual(arg00_15, arg10_15);
        })(parseAtLeastOneLeftOrRightBracket(Lexer.tokenize("<>!")), [new ParseResult(0, new _Node(1, new NodeValue(1, ofArray([new _Node(1, new NodeValue(0, "<")), new _Node(1, new NodeValue(0, ">"))])))), ofArray([[new Lexer.TokenType(10), "!"]])]);

        (function (arg00_16, arg10_16) {
            test_12.deepEqual(arg00_16, arg10_16);
        })(parseAtLeastOneLeftOrRightBracket(Lexer.tokenize("!>")), [new ParseResult(2), Lexer.tokenize("!>")]);
    });
}

tests();
tests$1();

})));
//# sourceMappingURL=tests.bundle.js.map
