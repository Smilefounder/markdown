using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;

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
        /// Show sourceFile, sourceStartLineNumber, sourceEndLineNumber in HTML tags
        /// </summary>
        public bool EnableSourceInfo { get; }

        public ImmutableHashSet<string> InclusionSet { get; }

        public List<string> Dependency { get; private set; }

        public MarkdownContext(string filePath, string basePath, bool enableSourceInfo = false, ImmutableHashSet<string> inclusionSet = null, List<string> dependency = null)
        {
            FilePath = filePath;
            BasePath = basePath;
            EnableSourceInfo = enableSourceInfo;
            InclusionSet = inclusionSet;
            Dependency = dependency ?? new List<string>();
        }

        public MarkdownContext(MarkdownContext context)
        {
            BasePath = context.BasePath;
            FilePath = context.FilePath;
            EnableSourceInfo = context.EnableSourceInfo;
            InclusionSet = context.InclusionSet;
            Dependency = context.Dependency;
        }

        public MarkdownContext SetInclusionSet(ImmutableHashSet<string> set)
        {
            return new MarkdownContext(FilePath, BasePath, EnableSourceInfo, set, Dependency);
        }

        public MarkdownContext AddIncludeFile(string filePath)
        {
            var set = InclusionSet ?? ImmutableHashSet<string>.Empty;
            var cloneSet = set.Add(filePath);

            return new MarkdownContext(FilePath, BasePath, EnableSourceInfo, cloneSet, Dependency);
        }

        public void ReportDependency(string filePath)
        {
            Dependency.Add(filePath);
        }
    }
}
