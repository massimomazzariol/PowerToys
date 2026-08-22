// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;

using AdvancedPaste.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdvancedPaste.UnitTests.HelpersTests;

[TestClass]
public sealed class TextCaseHelperTests
{
    [TestMethod]
    public void ToUpperCase_GermanCulture_PreservesUmlauts()
    {
        var culture = CultureInfo.GetCultureInfo("de-DE");

        Assert.AreEqual("FÜR SCHÖNE HÄUSER", TextCaseHelper.ToUpperCase("für schöne häuser", culture));
    }

    [TestMethod]
    public void ToUpperCase_TurkishCulture_UsesDottedCapitalI()
    {
        var culture = CultureInfo.GetCultureInfo("tr-TR");

        Assert.AreEqual("İSTANBUL", TextCaseHelper.ToUpperCase("istanbul", culture));
    }

    [TestMethod]
    public void ToUpperCase_ItalianCulture_UsesLatinCapitalI()
    {
        var culture = CultureInfo.GetCultureInfo("it-IT");

        Assert.AreEqual("ISTANBUL", TextCaseHelper.ToUpperCase("istanbul", culture));
    }

    [TestMethod]
    public void ToLowerCase_TurkishCulture_UsesDotlessLowercaseI()
    {
        var culture = CultureInfo.GetCultureInfo("tr-TR");

        Assert.AreEqual("ıi", TextCaseHelper.ToLowerCase("Iİ", culture));
    }

    [TestMethod]
    public void ToUpperCase_AzeriCulture_UsesDottedCapitalI()
    {
        var culture = CultureInfo.GetCultureInfo("az-Latn-AZ");

        Assert.AreEqual("İZMİR", TextCaseHelper.ToUpperCase("izmir", culture));
    }

    [TestMethod]
    public void HumanTextCases_DefaultToCurrentCulture()
    {
        var originalCulture = CultureInfo.CurrentCulture;

        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("tr-TR");
            Assert.AreEqual("İSTANBUL", TextCaseHelper.ToUpperCase("istanbul"));
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }

    [TestMethod]
    public void ToTitleCase_NormalizesAllUppercaseInputBeforeTitleCasing()
    {
        var culture = CultureInfo.GetCultureInfo("en-US");

        Assert.AreEqual("Hello World", TextCaseHelper.ToTitleCase("HELLO WORLD", culture));
    }

    [TestMethod]
    public void ToTitleCase_IsDeterministicNotGrammaticalCorrection()
    {
        var culture = CultureInfo.GetCultureInfo("de-DE");

        Assert.AreEqual("Per Anhalter Durch Die Galaxis", TextCaseHelper.ToTitleCase("PER ANHALTER DURCH DIE GALAXIS", culture));
    }

    [TestMethod]
    public void ToSentenceCase_CapitalizesAfterSentenceAndLineBoundaries()
    {
        var culture = CultureInfo.GetCultureInfo("en-US");
        const string input = "HELLO WORLD. THIS IS ANOTHER SENTENCE!\nTHIRD ONE?";
        const string expected = "Hello world. This is another sentence!\nThird one?";

        Assert.AreEqual(expected, TextCaseHelper.ToSentenceCase(input, culture));
    }

    [TestMethod]
    public void ToSentenceCase_RecognizesCommonUnicodeSentenceTerminators()
    {
        var culture = CultureInfo.GetCultureInfo("en-US");
        const string input = "FIRST。 SECOND？ THIRD！ FOURTH";
        const string expected = "First。 Second？ Third！ Fourth";

        Assert.AreEqual(expected, TextCaseHelper.ToSentenceCase(input, culture));
    }

    [TestMethod]
    public void ToSentenceCase_DoesNotAttemptGermanGrammarRecovery()
    {
        var culture = CultureInfo.GetCultureInfo("de-DE");

        Assert.AreEqual("Guten morgen. Mein name ist max.", TextCaseHelper.ToSentenceCase("GUTEN MORGEN. MEIN NAME IST MAX.", culture));
    }

    [TestMethod]
    public void ToggleCase_TurkishCulture_UsesCultureSpecificMappings()
    {
        var culture = CultureInfo.GetCultureInfo("tr-TR");

        Assert.AreEqual("İıiI", TextCaseHelper.ToggleCase("iIİı", culture));
    }

    [TestMethod]
    public void ToggleCase_PreservesSupplementaryUnicodeCharacters()
    {
        var culture = CultureInfo.GetCultureInfo("en-US");

        Assert.AreEqual("A😀b", TextCaseHelper.ToggleCase("a😀B", culture));
    }

    [TestMethod]
    public void HumanTextCases_HandleCyrillicAndPreserveUncasedScripts()
    {
        var culture = CultureInfo.GetCultureInfo("ru-RU");

        Assert.AreEqual("ПРИВЕТ МИР", TextCaseHelper.ToUpperCase("Привет Мир", culture));
        Assert.AreEqual("مرحبا بالعالم 中文", TextCaseHelper.ToUpperCase("مرحبا بالعالم 中文", culture));
    }

    [TestMethod]
    [DataRow("hello world", "helloWorld")]
    [DataRow("hello-world", "helloWorld")]
    [DataRow("hello_world", "helloWorld")]
    [DataRow("helloWorld", "helloWorld")]
    [DataRow("HelloWorld", "helloWorld")]
    [DataRow("XMLHttpRequest", "xmlHttpRequest")]
    [DataRow("HTTP server", "httpServer")]
    [DataRow("version2Test", "version2Test")]
    [DataRow("XML2Parser", "xml2Parser")]
    [DataRow("München Hauptbahnhof", "münchenHauptbahnhof")]
    public void ToCamelCase_TokenizesCommonIdentifierStyles(string input, string expected)
    {
        Assert.AreEqual(expected, TextCaseHelper.ToCamelCase(input));
    }

    [TestMethod]
    [DataRow("hello world", "HelloWorld")]
    [DataRow("XMLHttpRequest", "XmlHttpRequest")]
    [DataRow("version2Test", "Version2Test")]
    [DataRow("München Hauptbahnhof", "MünchenHauptbahnhof")]
    public void ToPascalCase_TokenizesCommonIdentifierStyles(string input, string expected)
    {
        Assert.AreEqual(expected, TextCaseHelper.ToPascalCase(input));
    }

    [TestMethod]
    [DataRow("hello world", "hello_world")]
    [DataRow("XMLHttpRequest", "xml_http_request")]
    [DataRow("version2Test", "version2_test")]
    [DataRow("München Hauptbahnhof", "münchen_hauptbahnhof")]
    public void ToSnakeCase_TokenizesCommonIdentifierStyles(string input, string expected)
    {
        Assert.AreEqual(expected, TextCaseHelper.ToSnakeCase(input));
    }

    [TestMethod]
    [DataRow("hello world", "HELLO_WORLD")]
    [DataRow("XMLHttpRequest", "XML_HTTP_REQUEST")]
    [DataRow("München Hauptbahnhof", "MÜNCHEN_HAUPTBAHNHOF")]
    public void ToScreamingSnakeCase_TokenizesCommonIdentifierStyles(string input, string expected)
    {
        Assert.AreEqual(expected, TextCaseHelper.ToScreamingSnakeCase(input));
    }

    [TestMethod]
    [DataRow("hello world", "hello-world")]
    [DataRow("XMLHttpRequest", "xml-http-request")]
    [DataRow("München Hauptbahnhof", "münchen-hauptbahnhof")]
    public void ToKebabCase_TokenizesCommonIdentifierStyles(string input, string expected)
    {
        Assert.AreEqual(expected, TextCaseHelper.ToKebabCase(input));
    }

    [TestMethod]
    public void IdentifierCases_PreserveDecomposedUnicodeCombiningMarks()
    {
        const string input = "Cafe\u0301 Noir";
        const string expected = "cafe\u0301_noir";

        Assert.AreEqual(expected, TextCaseHelper.ToSnakeCase(input));
    }

    [TestMethod]
    public void IdentifierCases_PreserveUncasedUnicodeScripts()
    {
        Assert.AreEqual("中文_测试", TextCaseHelper.ToSnakeCase("中文 测试"));
    }

    [TestMethod]
    public void IdentifierCases_DoNotDependOnCurrentCulture()
    {
        var originalCulture = CultureInfo.CurrentCulture;

        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("tr-TR");
            Assert.AreEqual("istanbulIzmir", TextCaseHelper.ToCamelCase("istanbul izmir"));

            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            Assert.AreEqual("istanbulIzmir", TextCaseHelper.ToCamelCase("istanbul izmir"));
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }

    [TestMethod]
    public void EmptyInput_ReturnsEmptyString()
    {
        Assert.AreEqual(string.Empty, TextCaseHelper.ToLowerCase(string.Empty));
        Assert.AreEqual(string.Empty, TextCaseHelper.ToUpperCase(string.Empty));
        Assert.AreEqual(string.Empty, TextCaseHelper.ToTitleCase(string.Empty));
        Assert.AreEqual(string.Empty, TextCaseHelper.ToSentenceCase(string.Empty));
        Assert.AreEqual(string.Empty, TextCaseHelper.ToggleCase(string.Empty));
        Assert.AreEqual(string.Empty, TextCaseHelper.ToCamelCase(string.Empty));
        Assert.AreEqual(string.Empty, TextCaseHelper.ToPascalCase(string.Empty));
        Assert.AreEqual(string.Empty, TextCaseHelper.ToSnakeCase(string.Empty));
        Assert.AreEqual(string.Empty, TextCaseHelper.ToScreamingSnakeCase(string.Empty));
        Assert.AreEqual(string.Empty, TextCaseHelper.ToKebabCase(string.Empty));
    }

    [TestMethod]
    public void IdentifierCases_SeparatorOnlyInput_ReturnsEmptyString()
    {
        Assert.AreEqual(string.Empty, TextCaseHelper.ToCamelCase(" _- / "));
        Assert.AreEqual(string.Empty, TextCaseHelper.ToSnakeCase(" _- / "));
    }

    [TestMethod]
    public void NullInput_ThrowsForAllTransformations()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => TextCaseHelper.ToLowerCase(null));
        Assert.ThrowsExactly<ArgumentNullException>(() => TextCaseHelper.ToUpperCase(null));
        Assert.ThrowsExactly<ArgumentNullException>(() => TextCaseHelper.ToTitleCase(null));
        Assert.ThrowsExactly<ArgumentNullException>(() => TextCaseHelper.ToSentenceCase(null));
        Assert.ThrowsExactly<ArgumentNullException>(() => TextCaseHelper.ToggleCase(null));
        Assert.ThrowsExactly<ArgumentNullException>(() => TextCaseHelper.ToCamelCase(null));
        Assert.ThrowsExactly<ArgumentNullException>(() => TextCaseHelper.ToPascalCase(null));
        Assert.ThrowsExactly<ArgumentNullException>(() => TextCaseHelper.ToSnakeCase(null));
        Assert.ThrowsExactly<ArgumentNullException>(() => TextCaseHelper.ToScreamingSnakeCase(null));
        Assert.ThrowsExactly<ArgumentNullException>(() => TextCaseHelper.ToKebabCase(null));
    }
}
