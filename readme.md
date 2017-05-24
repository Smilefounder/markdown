## Use MarkdigEngine in docfx

Export [markdig](https://github.com/lunet-io/markdig) as [docfx](https://github.com/dotnet/docfx) markdown engine.

Usage:
1. Clone source code from GitHub.
2. Compile.
3. Copy output `MarkdigEngine.dll` to x/`plugins` folder.
4. Update `docfx.json`:
   * add x folder to template
   * add `"markdownEngineName": "markdig"`

## Use MarkdigEngine in OP repo

1. Change `.openpublishing.publish.config.json`.
   - Add configuration item `customized_template_paths` under docset node.
     ```json
     "customized_template_paths": [
         "_dependentPackages/MarkdigEngine/content"
       ]
     ```
   - Add configuration item `dependent_packages` at top level.
     ```json
     "dependent_packages": [
       {
         "id": "Microsoft.DocAsCode.MarkdigEngine",
         "nuget_feed": "https://www.myget.org/F/op-dev/api/v2",
         "path_to_root": "_dependentPackages/MarkdigEngine",
         "version": "latest"
       }
     ]
     ```
2. Change `docfx.json`
   - Add configuration item `markdownEngineName` under `build` node.
   ```json
   "markdownEngineName": "markdig"
   ```




   
