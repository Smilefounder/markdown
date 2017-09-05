using System.Collections.Generic;
using System.Collections.Immutable;

using Microsoft.DocAsCode.Plugins;

namespace MarkdigEngine
{
    public class MarkdownContext
    {
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

        public ICompositionContainer Container { get; }

        public ImmutableHashSet<string> InclusionSet { get; }

        public List<string> Dependency { get; private set; }

        public MarkdownContext(string filePath,
            string basePath,
            bool isInline = false,
            ImmutableHashSet<string> inclusionSet = null,
            List<string> dependency = null,
            ICompositionContainer container = null)
        {
            FilePath = filePath;
            BasePath = basePath;
            IsInline = isInline;
            InclusionSet = inclusionSet;
            Dependency = dependency ?? new List<string>();
            Container = container;
        }

        public MarkdownContext(MarkdownContext context)
        {
            BasePath = context.BasePath;
            FilePath = context.FilePath;
            InclusionSet = context.InclusionSet;
            Dependency = context.Dependency;
        }

        public MarkdownContext AddIncludeFile(string filePath)
        {
            var set = InclusionSet ?? ImmutableHashSet<string>.Empty;
            var cloneSet = set.Add(filePath);

            return new MarkdownContext(FilePath, BasePath, IsInline, cloneSet, Dependency);
        }

        public void ReportDependency(string filePath)
        {
            if (!Dependency.Contains(filePath)) Dependency.Add(filePath);
        }
    }
}
