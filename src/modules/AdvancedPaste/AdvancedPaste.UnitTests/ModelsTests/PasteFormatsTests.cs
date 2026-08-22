// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AdvancedPaste.Models;
using Microsoft.PowerToys.Settings.UI.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdvancedPaste.UnitTests.ModelsTests;

[TestClass]
public sealed class PasteFormatsTests
{
    [TestMethod]
    public void PersistedFormatIdsRemainStable()
    {
        Assert.AreEqual(0, (int)PasteFormats.PlainText);
        Assert.AreEqual(1, (int)PasteFormats.Markdown);
        Assert.AreEqual(2, (int)PasteFormats.Json);
        Assert.AreEqual(3, (int)PasteFormats.FixSpellingAndGrammar);
        Assert.AreEqual(4, (int)PasteFormats.ImageToText);
        Assert.AreEqual(5, (int)PasteFormats.PasteAsTxtFile);
        Assert.AreEqual(6, (int)PasteFormats.PasteAsPngFile);
        Assert.AreEqual(7, (int)PasteFormats.PasteAsHtmlFile);
        Assert.AreEqual(8, (int)PasteFormats.TranscodeToMp3);
        Assert.AreEqual(9, (int)PasteFormats.TranscodeToMp4);
        Assert.AreEqual(10, (int)PasteFormats.KernelQuery);
        Assert.AreEqual(11, (int)PasteFormats.CustomTextTransformation);
        Assert.AreEqual(12, (int)PasteFormats.SingleLine);
        Assert.AreEqual(13, (int)PasteFormats.LowerCase);
        Assert.AreEqual(14, (int)PasteFormats.UpperCase);
        Assert.AreEqual(15, (int)PasteFormats.TitleCase);
        Assert.AreEqual(16, (int)PasteFormats.SentenceCase);
        Assert.AreEqual(17, (int)PasteFormats.ToggleCase);
        Assert.AreEqual(18, (int)PasteFormats.CamelCase);
        Assert.AreEqual(19, (int)PasteFormats.PascalCase);
        Assert.AreEqual(20, (int)PasteFormats.SnakeCase);
        Assert.AreEqual(21, (int)PasteFormats.ScreamingSnakeCase);
        Assert.AreEqual(22, (int)PasteFormats.KebabCase);
    }

    [DataTestMethod]
    [DataRow(PasteFormats.LowerCase, AdvancedPasteTextCaseAction.PropertyNames.LowerCase)]
    [DataRow(PasteFormats.UpperCase, AdvancedPasteTextCaseAction.PropertyNames.UpperCase)]
    [DataRow(PasteFormats.TitleCase, AdvancedPasteTextCaseAction.PropertyNames.TitleCase)]
    [DataRow(PasteFormats.SentenceCase, AdvancedPasteTextCaseAction.PropertyNames.SentenceCase)]
    [DataRow(PasteFormats.ToggleCase, AdvancedPasteTextCaseAction.PropertyNames.ToggleCase)]
    [DataRow(PasteFormats.CamelCase, AdvancedPasteTextCaseAction.PropertyNames.CamelCase)]
    [DataRow(PasteFormats.PascalCase, AdvancedPasteTextCaseAction.PropertyNames.PascalCase)]
    [DataRow(PasteFormats.SnakeCase, AdvancedPasteTextCaseAction.PropertyNames.SnakeCase)]
    [DataRow(PasteFormats.ScreamingSnakeCase, AdvancedPasteTextCaseAction.PropertyNames.ScreamingSnakeCase)]
    [DataRow(PasteFormats.KebabCase, AdvancedPasteTextCaseAction.PropertyNames.KebabCase)]
    public void TextCaseFormatsUseExpectedAdditionalActionIpcKeys(PasteFormats format, string expectedIpcKey)
    {
        Assert.AreEqual(expectedIpcKey, PasteFormat.MetadataDict[format].IPCKey);
    }

    [TestMethod]
    public void TextCaseAdditionalActionsAreDisabledByDefault()
    {
        var textCase = new AdvancedPasteTextCaseAction();

        Assert.IsTrue(textCase.IsShown);
        Assert.IsFalse(textCase.LowerCase.IsShown);
        Assert.IsFalse(textCase.UpperCase.IsShown);
        Assert.IsFalse(textCase.TitleCase.IsShown);
        Assert.IsFalse(textCase.SentenceCase.IsShown);
        Assert.IsFalse(textCase.ToggleCase.IsShown);
        Assert.IsFalse(textCase.CamelCase.IsShown);
        Assert.IsFalse(textCase.PascalCase.IsShown);
        Assert.IsFalse(textCase.SnakeCase.IsShown);
        Assert.IsFalse(textCase.ScreamingSnakeCase.IsShown);
        Assert.IsFalse(textCase.KebabCase.IsShown);
    }
}
