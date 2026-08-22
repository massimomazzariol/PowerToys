// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AdvancedPaste.Helpers;

public static class TextCaseHelper
{
    private enum IdentifierRuneKind
    {
        Separator,
        UppercaseLetter,
        LowercaseLetter,
        UncasedLetter,
        Digit,
        Mark,
    }

    public static string ToLowerCase(string text, CultureInfo culture = null)
    {
        ArgumentNullException.ThrowIfNull(text);
        return text.ToLower(culture ?? CultureInfo.CurrentCulture);
    }

    public static string ToUpperCase(string text, CultureInfo culture = null)
    {
        ArgumentNullException.ThrowIfNull(text);
        return text.ToUpper(culture ?? CultureInfo.CurrentCulture);
    }

    public static string ToTitleCase(string text, CultureInfo culture = null)
    {
        ArgumentNullException.ThrowIfNull(text);

        culture ??= CultureInfo.CurrentCulture;

        // TextInfo.ToTitleCase preserves words that are already all-uppercase as acronyms.
        // Normalize first so this action behaves as a case conversion (for example,
        // "HELLO WORLD" -> "Hello World") rather than preserving the original casing.
        return culture.TextInfo.ToTitleCase(text.ToLower(culture));
    }

    public static string ToSentenceCase(string text, CultureInfo culture = null)
    {
        ArgumentNullException.ThrowIfNull(text);

        if (text.Length == 0)
        {
            return string.Empty;
        }

        culture ??= CultureInfo.CurrentCulture;
        var normalized = text.ToLower(culture);
        var result = new StringBuilder(normalized.Length);
        var capitalizeNextLetter = true;

        foreach (var rune in normalized.EnumerateRunes())
        {
            if (capitalizeNextLetter && IsLetter(rune))
            {
                result.Append(rune.ToString().ToUpper(culture));
                capitalizeNextLetter = false;
            }
            else
            {
                result.Append(rune.ToString());
            }

            if (IsSentenceBoundary(rune))
            {
                capitalizeNextLetter = true;
            }
        }

        return result.ToString();
    }

    public static string ToggleCase(string text, CultureInfo culture = null)
    {
        ArgumentNullException.ThrowIfNull(text);

        culture ??= CultureInfo.CurrentCulture;
        var result = new StringBuilder(text.Length);

        foreach (var rune in text.EnumerateRunes())
        {
            var category = Rune.GetUnicodeCategory(rune);

            if (category == UnicodeCategory.LowercaseLetter)
            {
                result.Append(rune.ToString().ToUpper(culture));
            }
            else if (category is UnicodeCategory.UppercaseLetter or UnicodeCategory.TitlecaseLetter)
            {
                result.Append(rune.ToString().ToLower(culture));
            }
            else
            {
                result.Append(rune.ToString());
            }
        }

        return result.ToString();
    }

    public static string ToCamelCase(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        return ToMixedCaseIdentifier(TokenizeIdentifier(text), capitalizeFirstToken: false);
    }

    public static string ToPascalCase(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        return ToMixedCaseIdentifier(TokenizeIdentifier(text), capitalizeFirstToken: true);
    }

    public static string ToSnakeCase(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        return JoinIdentifierTokens(TokenizeIdentifier(text), "_", uppercase: false);
    }

    public static string ToScreamingSnakeCase(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        return JoinIdentifierTokens(TokenizeIdentifier(text), "_", uppercase: true);
    }

    public static string ToKebabCase(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        return JoinIdentifierTokens(TokenizeIdentifier(text), "-", uppercase: false);
    }

    private static string ToMixedCaseIdentifier(IReadOnlyList<string> tokens, bool capitalizeFirstToken)
    {
        if (tokens.Count == 0)
        {
            return string.Empty;
        }

        var result = new StringBuilder();

        for (var i = 0; i < tokens.Count; i++)
        {
            var token = tokens[i].ToLowerInvariant();
            var capitalize = i > 0 || capitalizeFirstToken;
            result.Append(capitalize ? CapitalizeIdentifierToken(token) : token);
        }

        return result.ToString();
    }

    private static string JoinIdentifierTokens(IReadOnlyList<string> tokens, string separator, bool uppercase)
    {
        if (tokens.Count == 0)
        {
            return string.Empty;
        }

        var result = new StringBuilder();

        for (var i = 0; i < tokens.Count; i++)
        {
            if (i > 0)
            {
                result.Append(separator);
            }

            result.Append(uppercase ? tokens[i].ToUpperInvariant() : tokens[i].ToLowerInvariant());
        }

        return result.ToString();
    }

    private static string CapitalizeIdentifierToken(string token)
    {
        var result = new StringBuilder(token.Length);
        var capitalized = false;

        foreach (var rune in token.EnumerateRunes())
        {
            if (!capitalized && IsLetter(rune))
            {
                result.Append(rune.ToString().ToUpperInvariant());
                capitalized = true;
            }
            else
            {
                result.Append(rune.ToString());
            }
        }

        return result.ToString();
    }

    private static IReadOnlyList<string> TokenizeIdentifier(string text)
    {
        var runes = new List<Rune>();
        foreach (var rune in text.EnumerateRunes())
        {
            runes.Add(rune);
        }

        var tokens = new List<string>();
        var currentToken = new StringBuilder();
        var previousKind = IdentifierRuneKind.Separator;

        for (var i = 0; i < runes.Count; i++)
        {
            var rune = runes[i];
            var currentKind = GetIdentifierRuneKind(rune);

            if (currentKind == IdentifierRuneKind.Separator)
            {
                AddTokenIfAny(tokens, currentToken);
                previousKind = IdentifierRuneKind.Separator;
                continue;
            }

            if (currentKind == IdentifierRuneKind.Mark)
            {
                // Keep combining marks attached to their surrounding token instead of
                // treating decomposed Unicode text as multiple words.
                currentToken.Append(rune.ToString());
                continue;
            }

            var nextKind = GetNextSignificantIdentifierRuneKind(runes, i + 1);
            var splitBeforeCurrent = currentToken.Length > 0 &&
                                     currentKind == IdentifierRuneKind.UppercaseLetter &&
                                     (previousKind is IdentifierRuneKind.LowercaseLetter or IdentifierRuneKind.UncasedLetter or IdentifierRuneKind.Digit ||
                                      (previousKind == IdentifierRuneKind.UppercaseLetter && nextKind == IdentifierRuneKind.LowercaseLetter));

            if (splitBeforeCurrent)
            {
                AddTokenIfAny(tokens, currentToken);
            }

            currentToken.Append(rune.ToString());
            previousKind = currentKind;
        }

        AddTokenIfAny(tokens, currentToken);
        return tokens;
    }

    private static void AddTokenIfAny(List<string> tokens, StringBuilder currentToken)
    {
        if (currentToken.Length == 0)
        {
            return;
        }

        tokens.Add(currentToken.ToString());
        currentToken.Clear();
    }

    private static IdentifierRuneKind GetNextSignificantIdentifierRuneKind(IReadOnlyList<Rune> runes, int startIndex)
    {
        for (var i = startIndex; i < runes.Count; i++)
        {
            var kind = GetIdentifierRuneKind(runes[i]);
            if (kind != IdentifierRuneKind.Mark)
            {
                return kind;
            }
        }

        return IdentifierRuneKind.Separator;
    }

    private static IdentifierRuneKind GetIdentifierRuneKind(Rune rune)
    {
        return Rune.GetUnicodeCategory(rune) switch
        {
            UnicodeCategory.UppercaseLetter or UnicodeCategory.TitlecaseLetter => IdentifierRuneKind.UppercaseLetter,
            UnicodeCategory.LowercaseLetter => IdentifierRuneKind.LowercaseLetter,
            UnicodeCategory.ModifierLetter or UnicodeCategory.OtherLetter => IdentifierRuneKind.UncasedLetter,
            UnicodeCategory.DecimalDigitNumber or UnicodeCategory.LetterNumber or UnicodeCategory.OtherNumber => IdentifierRuneKind.Digit,
            UnicodeCategory.NonSpacingMark or UnicodeCategory.SpacingCombiningMark or UnicodeCategory.EnclosingMark => IdentifierRuneKind.Mark,
            _ => IdentifierRuneKind.Separator,
        };
    }

    private static bool IsLetter(Rune rune)
    {
        return Rune.GetUnicodeCategory(rune) is
            UnicodeCategory.UppercaseLetter or
            UnicodeCategory.LowercaseLetter or
            UnicodeCategory.TitlecaseLetter or
            UnicodeCategory.ModifierLetter or
            UnicodeCategory.OtherLetter;
    }

    private static bool IsSentenceBoundary(Rune rune)
    {
        // This is intentionally deterministic punctuation-based sentence casing, not
        // language detection or Unicode sentence-boundary analysis. Include common
        // ASCII and Unicode sentence terminators plus all line separators.
        return rune.Value switch
        {
            0x0021 or // !
            0x002E or // .
            0x003F or // ?
            0x000A or // LF
            0x000B or // VT
            0x000C or // FF
            0x000D or // CR
            0x0085 or // NEL
            0x061F or // Arabic question mark
            0x06D4 or // Arabic full stop
            0x0964 or // Devanagari danda
            0x0965 or // Devanagari double danda
            0x2028 or // Unicode line separator
            0x2029 or // Unicode paragraph separator
            0x3002 or // Ideographic full stop
            0xFF01 or // Fullwidth exclamation mark
            0xFF0E or // Fullwidth full stop
            0xFF1F => true, // Fullwidth question mark
            _ => false,
        };
    }
}
