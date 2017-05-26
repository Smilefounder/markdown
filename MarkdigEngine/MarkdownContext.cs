namespace MarkdigEngine
{
    public class MarkdownContext
    {
        /// <summary>
        /// Absolute path of `~`, the directory contains docfx.json.
        /// </summary>
        public string BasePath { get; set; }

        /// <summary>
        /// Relative path of current markdown file.
        /// </summary>
        public string FilePath { get; set; }
    }
}
