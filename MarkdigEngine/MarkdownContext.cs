using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;

namespace MarkdigEngine
{
    public class MarkdownContext
    {
        // This two private members are used for quickly getting the line number of one charactor
        // lineOffsets[5] = 255 means the 6th lines ends at the 255th character of the text
        private int previousLineNumber;
        private List<int> lineEnds;

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

        public void ResetlineEnds(string text)
        {
            previousLineNumber = 0;
            lineEnds = new List<int>();
            for (int position = 0; position < text.Length; position++)
            {
                var c = text[position];
                if (c == '\r' || c == '\n')
                {
                    if (c == '\r' && position + 1 < text.Length && text[position + 1] == '\n')
                    {
                        position++;
                    }
                    lineEnds.Add(position);
                }
            }
            lineEnds.Add(text.Length - 1);
        }

        /// <summary>
        /// Should call ResetlineEnds() first, and call GetLineNumber with an incremental position
        /// </summary>
        /// <param name="position"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public int GetLineNumber(int position, int start)
        {
            int lineNumber = start > previousLineNumber ? start : previousLineNumber;

            var absoluteFilePath = Path.Combine(BasePath, FilePath);
            if (lineEnds == null && File.Exists(absoluteFilePath))
            {
                ResetlineEnds(File.ReadAllText(absoluteFilePath));
            }

            if (lineEnds == null || lineNumber >= lineEnds.Count)
            {
                previousLineNumber = start;
                return start;
            }

            for (; lineNumber < lineEnds.Count; lineNumber++)
            {
                if (position <= lineEnds[lineNumber])
                {
                    previousLineNumber = lineNumber;
                    return lineNumber;
                }
            }

            previousLineNumber = start;
            return start;
        }
    }
}
