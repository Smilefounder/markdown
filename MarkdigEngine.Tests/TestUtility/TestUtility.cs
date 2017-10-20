﻿using System;
using System.Collections.Generic;
using System.IO;

using MarkdigEngine.Extensions;

using Microsoft.DocAsCode.Plugins;
using Xunit;

namespace MarkdigEngine.Tests
{
    public static class TestUtility
    {
        public static void AssertEqual(string expected, string actual, Func<string, MarkupResult> markup)
        {
            var result = markup(actual);
            Assert.Equal(expected.Replace("\r\n", "\n"), result.Html);
        }

        public static void AssertEqual(string expected, string actual, Func<string, string, MarkupResult> markup)
        {
            var result = markup(actual, null);
            Assert.Equal(expected.Replace("\r\n", "\n"), result.Html);
        }

        public static MarkupResult Markup(string content, string filePath = null)
        {
            var parameter = new MarkdownServiceParameters
            {
                BasePath = ".",
                Extensions = new Dictionary<string, object>
                {
                    { LineNumberExtension.EnableSourceInfo, true }
                }
            };
            var service = new MarkdigMarkdownService(parameter);

            return service.Markup(content, filePath ?? string.Empty);
        }

        public static MarkupResult MarkupWithoutSourceInfo(string content, string filePath = null)
        {
            var parameter = new MarkdownServiceParameters
            {
                BasePath = "."
            };
            var service = new MarkdigMarkdownService(parameter);

            return service.Markup(content, filePath ?? string.Empty);
        }

        public static void WriteToFile(string file, string content)
        {
            var dir = Path.GetDirectoryName(file);
            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }

            File.WriteAllText(file, content);
        }
    }
}