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

        private const string FilePathSetKey = "FilePathSet";

        public ImmutableDictionary<string, object> Variables { get; private set; }

        public MarkdownContext(string filePath, string basePath, ImmutableDictionary<string, object> variables = null)
        {
            FilePath = filePath;
            BasePath = basePath;
            Variables = variables ?? ImmutableDictionary<string, object>.Empty;
        }

        public MarkdownContext(MarkdownContext context)
        {
            BasePath = context.BasePath;
            FilePath = context.FilePath;
            Variables = context.Variables;
        }

        public MarkdownContext SetFilePathStack(ImmutableHashSet<string> filePathSet)
        {
            return CreateContext(Variables.SetItem(FilePathSetKey, filePathSet));
        }

        public ImmutableHashSet<string> GetFilePathStack()
        {
            if (Variables.ContainsKey(FilePathSetKey))
            {
                return (ImmutableHashSet<string>)Variables[FilePathSetKey];
            }

            return null;
        }

        public MarkdownContext CreateContext(ImmutableDictionary<string, object> variables)
        {
            var clone = (MarkdownContext)MemberwiseClone();
            clone.Variables = variables;

            return clone;
        }
    }
}
