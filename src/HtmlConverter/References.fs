module HtmlConverter.References

[<RequireQualifiedAccess>]
type AttributeType =
    | String
    | Int
    | Float
    | Obj
    | Func
    | Bool

type FSharpName = string
type HtmlName = string

let attributes : (FSharpName * HtmlName * AttributeType) list =
    [ // EventListener
      "OnCut", "onCut", AttributeType.Func
      "OnPaste", "onPaste", AttributeType.Func
      "OnCompositionEnd", "onCompositionEnd", AttributeType.Func
      "OnCompositionStart", "onCompositionStart", AttributeType.Func
      "OnCopy", "onCopy", AttributeType.Func
      "OnCompositionUpdate", "onCompositionUpdate", AttributeType.Func
      "OnFocus", "onFocus", AttributeType.Func
      "OnBlur", "onBlur", AttributeType.Func
      "OnChange", "onChange", AttributeType.Func
      "OnInput", "onInput", AttributeType.Func
      "OnSubmit", "onSubmit", AttributeType.Func
      "OnLoad", "onLoad", AttributeType.Func
      "OnError", "onError", AttributeType.Func
      "OnKeyDown", "onKeyDown", AttributeType.Func
      "OnKeyPress", "onKeyPress", AttributeType.Func
      "OnKeyUp", "onKeyUp", AttributeType.Func
      "OnAbort", "onAbort", AttributeType.Func
      "OnCanPlay", "onCanPlay", AttributeType.Func
      "OnCanPlayThrough", "onCanPlayThrough", AttributeType.Func
      "OnDurationChange", "onDurationChange", AttributeType.Func
      "OnEmptied", "onEmptied", AttributeType.Func
      "OnEncrypted", "onEncrypted", AttributeType.Func
      "OnEnded", "onEnded", AttributeType.Func
      "OnLoadedData", "onLoadedData", AttributeType.Func
      "OnLoadedMetadata", "onLoadedMetadata", AttributeType.Func
      "OnLoadStart", "onLoadStart", AttributeType.Func
      "OnPause", "onPause", AttributeType.Func
      "OnPlay", "onPlay", AttributeType.Func
      "OnPlaying", "onPlaying", AttributeType.Func
      "OnProgress", "onProgress", AttributeType.Func
      "OnRateChange", "onRateChange", AttributeType.Func
      "OnSeeked", "onSeeked", AttributeType.Func
      "OnSeeking", "onSeeking", AttributeType.Func
      "OnStalled", "onStalled", AttributeType.Func
      "OnSuspend", "onSuspend", AttributeType.Func
      "OnTimeUpdate", "onTimeUpdate", AttributeType.Func
      "OnVolumeChange", "onVolumeChange", AttributeType.Func
      "OnWaiting", "onWaiting", AttributeType.Func
      "OnClick", "onClick", AttributeType.Func
      "OnContextMenu", "onContextMenu", AttributeType.Func
      "OnDoubleClick", "onDoubleClick", AttributeType.Func
      "OnDrag", "onDrag", AttributeType.Func
      "OnDragEnd", "onDragEnd", AttributeType.Func
      "OnDragEnter", "onDragEnter", AttributeType.Func
      "OnDragExit", "onDragExit", AttributeType.Func
      "OnDragLeave", "onDragLeave", AttributeType.Func
      "OnDragOver", "onDragOver", AttributeType.Func
      "OnDragStart", "onDragStart", AttributeType.Func
      "OnDrop", "onDrop", AttributeType.Func
      "OnMouseDown", "onMouseDown", AttributeType.Func
      "OnMouseEnter", "onMouseEnter", AttributeType.Func
      "OnMouseLeave", "onMouseLeave", AttributeType.Func
      "OnMouseMove", "onMouseMove", AttributeType.Func
      "OnMouseOut", "onMouseOut", AttributeType.Func
      "OnMouseOver", "onMouseOver", AttributeType.Func
      "OnMouseUp", "onMouseUp", AttributeType.Func
      "OnSelect", "onSelect", AttributeType.Func
      "OnTouchCancel", "onTouchCancel", AttributeType.Func
      "OnTouchEnd", "onTouchEnd", AttributeType.Func
      "OnTouchMove", "onTouchMove", AttributeType.Func
      "OnTouchStart", "onTouchStart", AttributeType.Func
      "OnScroll", "onScroll", AttributeType.Func
      "OnWheel", "onWheel", AttributeType.Func
      "OnAnimationStart", "onAnimationStart", AttributeType.Func
      "OnAnimationEnd", "onAnimationEnd", AttributeType.Func
      "OnAnimationIteration", "onAnimationIteration", AttributeType.Func
      "OnTransitionEnd", "onTransitionEnd", AttributeType.Func
      // HTMLAttributes
      "DefaultChecked", "defaultChecked", AttributeType.Bool
      "DefaultValue", "defaultValue", AttributeType.String
      "Accept", "accept", AttributeType.String
      "AcceptCharset", "acceptCharset", AttributeType.String
      "AccessKey", "accessKey", AttributeType.String
      "Action", "action", AttributeType.String
      "AllowFullScreen", "allowFullScreen", AttributeType.Bool
      "AllowTransparency", "allowTransparency", AttributeType.Bool
      "Alt", "alt", AttributeType.String
      "AriaHasPopup", "aria-haspopup", AttributeType.Bool
      "AriaExpanded", "aria-expanded", AttributeType.Bool
      "Async", "async", AttributeType.Bool
      "AutoComplete", "autoComplete", AttributeType.String
      "AutoFocus", "autoFocus", AttributeType.Bool
      "AutoPlay", "autoPlay", AttributeType.Bool
      "Capture", "capture", AttributeType.Bool
      "CellPadding", "cellPadding", AttributeType.Obj
      "CellSpacing", "cellSpacing", AttributeType.Obj
      "CharSet", "charSet", AttributeType.String
      "Challenge", "challenge", AttributeType.String
      "Checked", "checked", AttributeType.Bool
      "ClassID", "classID", AttributeType.String
      "ClassName", "className", AttributeType.String
      "Class", "class",  AttributeType.String
      "Cols", "cols", AttributeType.Float
      "ColSpan", "colSpan", AttributeType.Float
      "Content", "content", AttributeType.String
      "ContentEditable", "contentEditable", AttributeType.Bool
      "ContextMenu", "contextMenu", AttributeType.String
      "Controls", "controls", AttributeType.Bool
      "Coords", "coords", AttributeType.String
      "CrossOrigin", "crossOrigin", AttributeType.String
      "DataToggle", "data-toggle", AttributeType.String
      "DateTime", "dateTime", AttributeType.String
      "Default", "default", AttributeType.Bool
      "Defer", "defer", AttributeType.Bool
      "Dir", "dir", AttributeType.String
      "Disabled", "disabled", AttributeType.Bool
      "Download", "download", AttributeType.Obj
      "Draggable", "draggable", AttributeType.Bool
      "EncType", "encType", AttributeType.String
      "Form", "form", AttributeType.String
      "FormAction", "formAction", AttributeType.String
      "FormEncType", "formEncType", AttributeType.String
      "FormMethod", "formMethod", AttributeType.String
      "FormNoValidate", "formNoValidate", AttributeType.Bool
      "FormTarget", "formTarget", AttributeType.String
      "FrameBorder", "frameBorder", AttributeType.Obj
      "Headers", "headers", AttributeType.String
      "Height", "height", AttributeType.Obj
      "Hidden", "hidden", AttributeType.Bool
      "High", "high", AttributeType.Float
      "Href", "href", AttributeType.String
      "HrefLang", "hrefLang", AttributeType.String
      "HtmlFor", "htmlFor", AttributeType.String
      "HttpEquiv", "httpEquiv", AttributeType.String
      "Icon", "icon", AttributeType.String
      "Id", "id", AttributeType.String
      "InputMode", "inputMode", AttributeType.String
      "Integrity", "integrity", AttributeType.String
      "Is", "is", AttributeType.String
      "KeyParams", "keyParams", AttributeType.String
      "KeyType", "keyType", AttributeType.String
      "Kind", "kind", AttributeType.String
      "Label", "label", AttributeType.String
      "Lang", "lang", AttributeType.String
      "List", "list", AttributeType.String
      "Loop", "loop", AttributeType.Bool
      "Low", "low", AttributeType.Float
      "Manifest", "manifest", AttributeType.String
      "MarginHeight", "marginHeight", AttributeType.Float
      "MarginWidth", "marginWidth", AttributeType.Float
      "Max", "max", AttributeType.Obj
      "MaxLength", "maxLength", AttributeType.Float
      "Media", "media", AttributeType.String
      "MediaGroup", "mediaGroup", AttributeType.String
      "Method", "method", AttributeType.String
      "Min", "min", AttributeType.Obj
      "MinLength", "minLength", AttributeType.Float
      "Multiple", "multiple", AttributeType.Bool
      "Muted", "muted", AttributeType.Bool
      "Name", "name", AttributeType.String
      "NoValidate", "noValidate", AttributeType.Bool
      "Open", "open", AttributeType.Bool
      "Optimum", "pptimum", AttributeType.Float
      "Pattern", "pattern", AttributeType.String
      "Placeholder", "placeholder", AttributeType.String
      "Poster", "poster", AttributeType.String
      "Preload", "preload", AttributeType.String
      "RadioGroup", "radioGroup", AttributeType.String
      "ReadOnly", "readOnly", AttributeType.Bool
      "Rel", "rel", AttributeType.String
      "Required", "required", AttributeType.Bool
      "Role", "role", AttributeType.String
      "Rows", "rows", AttributeType.Float
      "RowSpan", "rowSpan", AttributeType.Float
      "Sandbox", "sandbox", AttributeType.String
      "Scope", "scope", AttributeType.String
      "Scoped", "scoped", AttributeType.Bool
      "Scrolling", "scrolling", AttributeType.String
      "Seamless", "seamless", AttributeType.Bool
      "Selected", "selected", AttributeType.Bool
      "Shape", "shape", AttributeType.String
      "Size", "size", AttributeType.Float
      "Sizes", "sizes", AttributeType.String
      "Span", "span", AttributeType.Float
      "SpellCheck", "spellCheck", AttributeType.Bool
      "Src", "src", AttributeType.String
      "SrcDoc", "srcDoc", AttributeType.String
      "SrcLang", "srcLang", AttributeType.String
      "SrcSet", "srcSet", AttributeType.String
      "Start", "start", AttributeType.Float
      "Step", "step", AttributeType.Obj
      "Summary", "summary", AttributeType.String
      "TabIndex", "tabIndex", AttributeType.Float
      "Target", "target", AttributeType.String
      "Title", "title", AttributeType.String
      "Type", "type", AttributeType.String
      "UseMap", "useMap", AttributeType.String
      "Value", "value", AttributeType.String
      "Width", "width", AttributeType.Obj
      "Wmode", "wmode", AttributeType.String
      "Wrap", "wrap", AttributeType.String
      "About", "about", AttributeType.String
      "Datatype", "datatype", AttributeType.String
      "Inlist", "inlist", AttributeType.Obj
      "Prefix", "prefix", AttributeType.String
      "Property", "property", AttributeType.String
      "Resource", "resource", AttributeType.String
      "Typeof", "typeof", AttributeType.String
      "Vocab", "vocab", AttributeType.String
      "AutoCapitalize", "autoCapitalize", AttributeType.String
      "AutoCorrect", "autoCorrect", AttributeType.String
      "AutoSave", "autoSave", AttributeType.String
      "ItemProp", "itemProp", AttributeType.String
      "ItemScope", "itemScope", AttributeType.Bool
      "ItemType", "itemType", AttributeType.String
      "ItemID", "itemID", AttributeType.String
      "ItemRef", "itemRef", AttributeType.String
      "Results", "results", AttributeType.Float
      "Security", "security", AttributeType.String
      "Unselectable", "unselectable", AttributeType.Bool
    //   "Style", "style" of CSSProp list
    //   "Data", "data" of string * obj
    //| Custom of string * obj
       ]

let cssProps : string array =
    [| "AlignContent"; "AlignItems"; "AlignSelf"; "AlignmentAdjust"; "AlignmentBaseline"; "All"; "Animation"; "AnimationDelay"; "AnimationDirection"; "AnimationDuration";
      "AnimationFillMode"; "AnimationIterationCount"; "AnimationName"; "AnimationPlayState"; "AnimationTimingFunction"; "Appearance"; "BackfaceVisibility"; "Background";
      "BackgroundAttachment"; "BackgroundBlendMode"; "BackgroundClip"; "BackgroundColor"; "BackgroundComposite"; "BackgroundImage"; "BackgroundOrigin"; "BackgroundPosition";
      "BackgroundPositionX"; "BackgroundPositionY"; "BackgroundRepeat"; "BackgroundSize"; "BaselineShift"; "Behavior"; "BlockSize"; "Border"; "BorderBlockEnd"; "BorderBlockEndColor";
      "BorderBlockEndStyle"; "BorderBlockEndWidth"; "BorderBlockStart"; "BorderBlockStartColor"; "BorderBlockStartStyle"; "BorderBlockStartWidth"; "BorderBottom"; "BorderBottomColor";
      "BorderBottomLeftRadius"; "BorderBottomRightRadius"; "BorderBottomStyle"; "BorderBottomWidth"; "BorderCollapse"; "BorderColor"; "BorderCornerShape"; "BorderImage";
      "BorderImageOutset"; "BorderImageRepeat"; "BorderImageSlice"; "BorderImageSource"; "BorderImageWidth"; "BorderInlineEnd"; "BorderInlineEndColor"; "BorderInlineEndStyle";
      "BorderInlineEndWidth"; "BorderInlineStart"; "BorderInlineStartColor"; "BorderInlineStartStyle"; "BorderInlineStartWidth"; "BorderLeft"; "BorderLeftColor"; "BorderLeftStyle";
      "BorderLeftWidth"; "BorderRadius"; "BorderRight"; "BorderRightColor"; "BorderRightStyle"; "BorderRightWidth"; "BorderSpacing"; "BorderStyle"; "BorderTop"; "BorderTopColor";
      "BorderTopLeftRadius"; "BorderTopRightRadius"; "BorderTopStyle"; "BorderTopWidth"; "BorderWidth"; "Bottom"; "BoxAlign"; "BoxDecorationBreak"; "BoxDirection"; "BoxFlex";
      "BoxFlexGroup"; "BoxLineProgression"; "BoxLines"; "BoxOrdinalGroup"; "BoxShadow"; "BoxSizing"; "BreakAfter"; "BreakBefore"; "BreakInside"; "CaptionSide"; "CaretColor"; "Clear";
      "Clip"; "ClipPath"; "ClipRule"; "Color"; "ColorInterpolation"; "ColorInterpolationFilters"; "ColorProfile"; "ColorRendering"; "ColumnCount"; "ColumnFill"; "ColumnGap"; "ColumnRule";
      "ColumnRuleColor"; "ColumnRuleStyle"; "ColumnRuleWidth"; "ColumnSpan"; "ColumnWidth"; "Columns"; "Content"; "CounterIncrement"; "CounterReset"; "Cue"; "CueAfter"; "Cursor"; "Direction";
      "Display"; "DominantBaseline"; "EmptyCells"; "EnableBackground"; "Fill"; "FillOpacity"; "FillRule"; "Filter"; "Flex"; "FlexAlign"; "FlexBasis"; "FlexDirection"; "FlexFlow"; "FlexGrow";
      "FlexItemAlign"; "FlexLinePack"; "FlexOrder"; "FlexShrink"; "FlexWrap"; "Float"; "FloodColor"; "FloodOpacity"; "FlowFrom"; "Font"; "FontFamily"; "FontFeatureSettings"; "FontKerning";
      "FontLanguageOverride"; "FontSize"; "FontSizeAdjust"; "FontStretch"; "FontStyle"; "FontSynthesis"; "FontVariant"; "FontVariantAlternates"; "FontVariantCaps"; "FontVariantEastAsian";
      "FontVariantLigatures"; "FontVariantNumeric"; "FontVariantPosition"; "FontWeight"; "GlyphOrientationHorizontal"; "GlyphOrientationVertical"; "Grid"; "GridArea"; "GridAutoColumns";
      "GridAutoFlow"; "GridAutoRows"; "GridColumn"; "GridColumnEnd"; "GridColumnGap"; "GridColumnStart"; "GridGap"; "GridRow"; "GridRowEnd"; "GridRowGap"; "GridRowPosition"; "GridRowSpan";
      "GridRowStart"; "GridTemplate"; "GridTemplateAreas"; "GridTemplateColumns"; "GridTemplateRows"; "HangingPunctuation"; "Height"; "HyphenateLimitChars"; "HyphenateLimitLines";
      "HyphenateLimitZone"; "Hyphens"; "ImageOrientation"; "ImageRendering"; "ImageResolution"; "ImeMode"; "InlineSize"; "Isolation"; "JustifyContent"; "Kerning"; "LayoutGrid"; "LayoutGridChar";
      "LayoutGridLine"; "LayoutGridMode"; "LayoutGridType"; "Left"; "LetterSpacing"; "LightingColor"; "LineBreak"; "LineClamp"; "LineHeight"; "ListStyle"; "ListStyleImage"; "ListStylePosition";
      "ListStyleType"; "Margin"; "MarginBlockEnd"; "MarginBlockStart"; "MarginBottom"; "MarginInlineEnd"; "MarginInlineStart"; "MarginLeft"; "MarginRight"; "MarginTop"; "MarkerEnd";
      "MarkerMid"; "MarkerStart"; "MarqueeDirection"; "MarqueeStyle"; "Mask"; "MaskBorder"; "MaskBorderRepeat"; "MaskBorderSlice"; "MaskBorderSource"; "MaskBorderWidth"; "MaskClip";
      "MaskComposite"; "MaskImage"; "MaskMode"; "MaskOrigin"; "MaskPosition"; "MaskRepeat"; "MaskSize"; "MaskType"; "MaxFontSize"; "MaxHeight"; "MaxWidth"; "MinBlockSize"; "MinHeight";
      "MinInlineSize"; "MinWidth"; "MixBlendMode"; "ObjectFit"; "ObjectPosition"; "OffsetBlockEnd"; "OffsetBlockStart"; "OffsetInlineEnd"; "OffsetInlineStart"; "Opacity"; "Order";
      "Orphans"; "Outline"; "OutlineColor"; "OutlineOffset"; "OutlineStyle"; "OutlineWidth"; "Overflow"; "OverflowStyle"; "OverflowWrap"; "OverflowX"; "OverflowY"; "Padding";
      "PaddingBlockEnd"; "PaddingBlockStart"; "PaddingBottom"; "PaddingInlineEnd"; "PaddingInlineStart"; "PaddingLeft"; "PaddingRight"; "PaddingTop"; "PageBreakAfter"; "PageBreakBefore";
      "PageBreakInside"; "Pause"; "PauseAfter"; "PauseBefore"; "Perspective"; "PerspectiveOrigin"; "PointerEvents"; "Position"; "PunctuationTrim"; "Quotes"; "RegionFragment"; "Resize";
      "RestAfter"; "RestBefore"; "Right"; "RubyAlign"; "RubyMerge"; "RubyPosition"; "ScrollBehavior"; "ScrollSnapCoordinate"; "ScrollSnapDestination"; "ScrollSnapType"; "ShapeImageThreshold";
      "ShapeInside"; "ShapeMargin"; "ShapeOutside"; "ShapeRendering"; "Speak"; "SpeakAs"; "StopColor"; "StopOpacity"; "Stroke"; "StrokeDasharray"; "StrokeDashoffset"; "StrokeLinecap";
      "StrokeLinejoin"; "StrokeMiterlimit"; "StrokeOpacity"; "StrokeWidth"; "TabSize"; "TableLayout"; "TextAlign"; "TextAlignLast"; "TextAnchor"; "TextCombineUpright"; "TextDecoration";
      "TextDecorationColor"; "TextDecorationLine"; "TextDecorationLineThrough"; "TextDecorationNone"; "TextDecorationOverline"; "TextDecorationSkip"; "TextDecorationStyle";
      "TextDecorationUnderline"; "TextEmphasis"; "TextEmphasisColor"; "TextEmphasisPosition"; "TextEmphasisStyle"; "TextHeight"; "TextIndent"; "TextJustify"; "TextJustifyTrim";
      "TextKashidaSpace"; "TextLineThrough"; "TextLineThroughColor"; "TextLineThroughMode"; "TextLineThroughStyle"; "TextLineThroughWidth"; "TextOrientation"; "TextOverflow"; "TextOverline";
      "TextOverlineColor"; "TextOverlineMode"; "TextOverlineStyle"; "TextOverlineWidth"; "TextRendering"; "TextScript"; "TextShadow"; "TextTransform"; "TextUnderlinePosition"; "TextUnderlineStyle";
      "Top"; "TouchAction"; "Transform"; "TransformBox"; "TransformOrigin"; "TransformOriginZ"; "TransformStyle"; "Transition"; "TransitionDelay"; "TransitionDuration"; "TransitionProperty";
      "TransitionTimingFunction"; "UnicodeBidi"; "UnicodeRange"; "UserFocus"; "UserInput"; "VerticalAlign"; "Visibility"; "VoiceBalance"; "VoiceDuration"; "VoiceFamily"; "VoicePitch";
      "VoiceRange"; "VoiceRate"; "VoiceStress"; "VoiceVolume"; "WhiteSpace"; "WhiteSpaceTreatment"; "Widows"; "Width"; "WillChange"; "WordBreak"; "WordSpacing"; "WordWrap"; "WrapFlow";
      "WrapMargin"; "WrapOption"; "WritingMode"; "ZIndex"; "Zoom" |]
