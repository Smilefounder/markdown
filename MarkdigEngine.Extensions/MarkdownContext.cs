using System.Collections.Generic;
using System.Collections.Immutable;

namespace MarkdigEngine.Extensions
{
    public class MarkdownContext
    {
        /// <summary>
        /// Content of current markdown file.
        /// </summary>
        public string Content { get; }

        /// <summary>
        /// Absolute path of `~`, the directory contains docfx.json.
        /// </summary>
        public string BasePath { get; }

        /// <summary>
        /// Relative path of current markdown file.
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// Indicate if this file is inline included.
        /// </summary>
        public bool IsInline { get; }

        public ImmutableHashSet<string> InclusionSet { get; }

        public List<string> Dependency { get; private set; }

        public MarkdownValidatorBuilder Mvb { get; }

        public MarkdownContext(string filePath,
            string basePath,
            MarkdownValidatorBuilder mvb,
            string content = null,
            bool isInline = false,
            ImmutableHashSet<string> inclusionSet = null,
            List<string> dependency = null
            )
        {
            Content = content;
            FilePath = filePath;
            BasePath = basePath;
            Mvb = mvb;
            IsInline = isInline;
            InclusionSet = inclusionSet;
            Dependency = dependency ?? new List<string>();
        }

        public MarkdownContext(MarkdownContext context)
        {
            Content = context.Content;
            FilePath = context.FilePath;
            BasePath = context.BasePath;
            Mvb = context.Mvb;
            IsInline = context.IsInline;
            InclusionSet = context.InclusionSet;
            Dependency = context.Dependency;
        }

        public MarkdownContext AddIncludeFile(string filePath)
        {
            var set = InclusionSet ?? ImmutableHashSet<string>.Empty;
            var cloneSet = set.Add(filePath);

            return new MarkdownContext(FilePath, BasePath, Mvb, Content, IsInline, cloneSet, Dependency);
        }

        public void ReportDependency(string filePath)
        {
            if (!Dependency.Contains(filePath)) Dependency.Add(filePath);
        }
    }
}
