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

        public MarkdownValidatorBuilder Mvb { get; }

        public MarkdownContext(string filePath,
            string basePath,
            MarkdownValidatorBuilder mvb,
            string content,
            bool isInline,
            ImmutableHashSet<string> inclusionSet)
        {
            Content = content;
            FilePath = filePath;
            BasePath = basePath;
            Mvb = mvb;
            IsInline = isInline;
            InclusionSet = inclusionSet;
        }
    }
}
