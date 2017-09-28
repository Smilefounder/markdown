using System;
using System.Collections.Generic;
using System.Collections.Immutable;

using Markdig.Syntax;

namespace MarkdigEngine.Extensions
{
    internal class MarkdownTokenValidatorAdapter : IMarkdownObjectRewriter
    {
        private Action<IMarkdownObject> _preProcess;
        private Action<IMarkdownObject> _postProcess;

        public ImmutableArray<IMarkdownObjectValidator> Validators { get; }

        public MarkdownTokenValidatorAdapter(
            IEnumerable<IMarkdownObjectValidator> validators, 
            Action<IMarkdownObject> preProcess, 
            Action<IMarkdownObject> postProcess)
        {
            Validators = validators.ToImmutableArray();
            _preProcess = preProcess;
            _postProcess = postProcess;
        }

        public MarkdownTokenValidatorAdapter(
            IMarkdownObjectValidator validator,
            Action<IMarkdownObject> preProcess,
            Action<IMarkdownObject> postProcess)
        {
            Validators = new[] { validator }.ToImmutableArray();
            _preProcess = preProcess;
            _postProcess = postProcess;
        }

        public IMarkdownObject Rewrite(IMarkdownObject markdownObject)
        {
            foreach (var validator in Validators)
            {
                validator.Validate(markdownObject);
            }

            return markdownObject;
        }

        public void PreProcess(IMarkdownObject markdownObject)
        {
            _preProcess?.Invoke(markdownObject);
        }

        public void PostProcess(IMarkdownObject markdownObject)
        {
            _postProcess?.Invoke(markdownObject);
        }
    }
}