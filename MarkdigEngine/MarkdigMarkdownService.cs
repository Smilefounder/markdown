// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace MarkdigEngine
{
    using System.Collections.Generic;
    using System.Collections.Immutable;

    using MarkdigEngine.Extensions;

    using Microsoft.DocAsCode.Plugins;

    public class MarkdigMarkdownService : IMarkdownService
    {
        private readonly MarkdownServiceParameters _parameters;
        private readonly MarkdownValidatorBuilder _mvb;

        public MarkdigMarkdownService(
            MarkdownServiceParameters parameters,
            ICompositionContainer container = null)
        {
            _parameters = parameters;
            _mvb = MarkdownValidatorBuilder.Create(parameters, container);
        }

        public MarkupResult Markup(string content, string path)
        {
            var context = new MarkdownContextBuilder()
                            .WithFilePath(path)
                            .WithBasePath(_parameters.BasePath)
                            .WithMvb(_mvb)
                            .WithContent(content)
                            .Build();

            var dependency = new HashSet<string>();
            var compositor = new MarkdigCompositor(dependency);

            return new MarkupResult
            {
                Html = compositor.Markup(context, _parameters),
                Dependency = dependency.ToImmutableArray()
            };
        }
    }
}
