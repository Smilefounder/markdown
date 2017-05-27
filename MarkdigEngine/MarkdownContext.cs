using System.Collections.Generic;
using System.Collections.Immutable;

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

        public ImmutableHashSet<string> InclusionSet { get; }

        public List<string> Dependency { get; private set; }

        public MarkdownContext(string filePath, string basePath, ImmutableHashSet<string> inclusionSet = null, List<string> dependency = null)
        {
            FilePath = filePath;
            BasePath = basePath;
            InclusionSet = inclusionSet;
            Dependency = dependency ?? new List<string>();
        }

        public MarkdownContext(MarkdownContext context)
        {
            BasePath = context.BasePath;
            FilePath = context.FilePath;
            InclusionSet = context.InclusionSet;
            Dependency = context.Dependency;
        }

        public MarkdownContext SetInclusionSet(ImmutableHashSet<string> set)
        {
            return new MarkdownContext(FilePath, BasePath, set, Dependency);
        }

        public MarkdownContext AddIncludeFile(string filePath)
        {
            var set = InclusionSet ?? ImmutableHashSet<string>.Empty;
            var cloneSet = set.Add(filePath);

            return new MarkdownContext(FilePath, BasePath, cloneSet, Dependency);
        }

        public void ReportDependency(string filePath)
        {
            Dependency.Add(filePath);
        }
    }
}
