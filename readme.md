Export [markdig](https://github.com/lunet-io/markdig) as [docfx](https://github.com/dotnet/docfx) markdown engine.

Usage:
1. Clone source code from GitHub.
2. Compile.
3. Copy output `MarkdigEngine.dll` to x/`plugins` folder.
4. Update `docfx.json`:
   * add x folder to template
   * add `"markdownEngineName": "markdig"`