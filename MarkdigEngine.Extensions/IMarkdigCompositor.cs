// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace MarkdigEngine.Extensions
{
    using System.Collections.Generic;

    using Microsoft.DocAsCode.Plugins;

    public interface IMarkdigCompositor
    {
        string Markup(MarkdownContext context, MarkdownServiceParameters parameters);
        void ReportDependency(string file);
    }
}
