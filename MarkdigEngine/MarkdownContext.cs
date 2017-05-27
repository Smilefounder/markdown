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

        private const string FilePathSetKey = "FilePathSet";
        private const string DependencyKey = "DependencyKey";

        public ImmutableDictionary<string, object> Variables { get; private set; }

        public MarkdownContext(string filePath, string basePath, ImmutableDictionary<string, object> variables = null)
        {
            FilePath = filePath;
            BasePath = basePath;
            Variables = variables ?? ImmutableDictionary<string, object>.Empty.SetItem(DependencyKey, new List<string>());
        }

        public MarkdownContext(MarkdownContext context)
        {
            BasePath = context.BasePath;
            FilePath = context.FilePath;
            Variables = context.Variables;
        }

        public MarkdownContext SetFilePathSet(ImmutableHashSet<string> set)
        {
            return CreateContext(Variables.SetItem(FilePathSetKey, set));
        }

        public ImmutableHashSet<string> GetFilePathSet()
        {
            if (!Variables.ContainsKey(FilePathSetKey))
            {
                var context = SetFilePathSet(ImmutableHashSet<string>.Empty);
                Variables = context.Variables;
            }

            return (ImmutableHashSet<string>)Variables[FilePathSetKey];
        }

        public MarkdownContext AddFilePath(string filePath)
        {
            var set = GetFilePathSet();
            var cloneSet = set.Add(filePath);

            return CreateContext(Variables.SetItem(FilePathSetKey, cloneSet));
        }

        public MarkdownContext SetDependency(List<string> dependency)
        {
            return CreateContext(Variables.SetItem(DependencyKey, dependency));
        }

        public List<string> GetDependency()
        {
            if (!Variables.ContainsKey(DependencyKey))
            {
                var context = CreateContext(Variables.SetItem(DependencyKey, new List<string>()));
                Variables = context.Variables;
            }

            return (List<string>)Variables[DependencyKey];
        }

        public void ReportDependency(string filePath)
        {
            var d = GetDependency();
            d.Add(filePath);
        }

        public MarkdownContext CreateContext(ImmutableDictionary<string, object> variables)
        {
            var clone = (MarkdownContext)MemberwiseClone();
            clone.Variables = variables;

            return clone;
        }
    }
}
