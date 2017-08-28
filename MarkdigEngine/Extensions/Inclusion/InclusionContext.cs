namespace MarkdigEngine
{
    public class InclusionContext
    {
        public string Title { get; set; }

        public string IncludedFilePath { get; set; }

        public string GetRaw()
        {
            return $"[!include[{Title}]({IncludedFilePath})]";
        }
    }
}
