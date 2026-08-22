// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AdvancedPaste.Models;
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
}
