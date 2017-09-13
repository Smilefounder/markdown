using Markdig;

namespace MarkdigEngine.Plugin
{
    public interface IMarkdigCustomizer
    {
        /// <summary>
        /// Customize the <see cref="MarkdownPipelineBuilder">
        /// </summary>
        /// <param name="builder">The original pipeline builder from MarkdigEngine</param>
        /// <returns>The customized pipline builder</returns>
        MarkdownPipelineBuilder Customize(MarkdownPipelineBuilder builder);
    }
}
