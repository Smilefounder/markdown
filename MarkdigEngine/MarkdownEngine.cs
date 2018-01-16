// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace MarkdigEngine
{
    using System.Collections.Generic;

    using MarkdigEngine.Extensions;

    using Markdig;
    using Microsoft.DocAsCode.Plugins;

    public class MarkdownEngine : IMarkdownEngine
    {
        private HashSet<string> _dependency;

        public MarkdownEngine(HashSet<string> dependency)
        {
            _dependency = dependency;
        }

        public string Markup(MarkdownContext context, MarkdownServiceParameters parameters)
        {
            var builder = new MarkdownPipelineBuilder()
                                .UseMarkdigAdvancedExtensions()
                                .UseDfmExtensions(this, context, parameters)
                                .RemoveUnusedExtensions();

            var pipeline = builder.Build();

            return Markdown.ToHtml(context.Content, pipeline);
        }

        public void ReportDependency(string file)
        {
            _dependency = _dependency ?? new HashSet<string>();
            _dependency.Add(file);
        }
    }
}
