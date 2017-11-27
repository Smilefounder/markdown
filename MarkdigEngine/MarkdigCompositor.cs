using System.Collections.Generic;

using MarkdigEngine.Extensions;

using Markdig;
using Microsoft.DocAsCode.Plugins;

namespace MarkdigEngine
{
    public class MarkdigCompositor : IMarkdigCompositor
    {
        private HashSet<string> _dependency;

        public MarkdigCompositor(HashSet<string> dependency)
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
