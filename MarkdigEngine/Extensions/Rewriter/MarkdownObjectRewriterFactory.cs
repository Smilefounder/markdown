﻿using System;
using System.Collections.Generic;

using MarkdigEngine.Plugin;
using Markdig.Syntax;

namespace MarkdigEngine
{
    public static class MarkdownObjectRewriterFactory
    {
        public static IMarkdownObjectRewriter FromValidators(
            IEnumerable<IMarkdownObjectValidator> validators,
            Action<IMarkdownObject> preProcess = null,
            Action<IMarkdownObject> postProcess = null)
        {
            if (validators == null)
            {
                throw new ArgumentNullException(nameof(validators));
            }

            return new MarkdownTokenValidatorAdapter(validators, preProcess, postProcess);
        }

        public static IMarkdownObjectRewriter FromValidator(
            IMarkdownObjectValidator validator,
            Action<IMarkdownObject> preProcess = null,
            Action<IMarkdownObject> postProcess = null)
        {
            if (validator == null)
            {
                throw new ArgumentNullException(nameof(validator));
            }

            return new MarkdownTokenValidatorAdapter(validator, preProcess, postProcess);
        }
    }
}
