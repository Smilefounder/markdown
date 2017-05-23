using System;
using System.IO;
using System.Text;

namespace MarkdigEngine.Extensions
{
    public static class ExtensionsHelper
    {
        public static string GetAbsolutlyPath(string basePath, string currentFilePath, string referencedFilePath)
        {
            if (string.IsNullOrEmpty(basePath))
            {
                throw new ArgumentException(nameof(basePath));
            }

            if (string.IsNullOrEmpty(currentFilePath))
            {
                throw new ArgumentException(nameof(currentFilePath));
            }

            if (string.IsNullOrEmpty(referencedFilePath))
            {
                throw new ArgumentException(nameof(referencedFilePath));
            }

            currentFilePath = Path.Combine(basePath, currentFilePath);
            var currentFolder = Path.GetDirectoryName(currentFilePath);

            var tempFolderName = new StringBuilder();

            var sb = new StringBuilder(currentFolder);

            for (int index = 0; index < referencedFilePath.Length; index++)
            {
                if (referencedFilePath[index] == '/' || referencedFilePath[index] == '\\')
                {
                    var folderName = tempFolderName.ToString();
                    tempFolderName = new StringBuilder();

                    switch (folderName)
                    {
                        case ".":
                            break;
                        case "..":
                            int slashPosition = sb.Length - 1;
                            while (slashPosition >= 0 && sb[slashPosition] != '/' && sb[slashPosition] != '\\')
                            {
                                slashPosition--;
                            }

                            sb.Length = slashPosition;
                            break;
                        case "~":
                            sb.Length = basePath.Length;
                            break;
                        default:
                            sb.Append('/').Append(folderName);
                            break;
                    }

                }
                else
                {
                    tempFolderName.Append(referencedFilePath[index]);
                }
            }

            if (tempFolderName.Length != 0)
            {
                sb.Append('/').Append(tempFolderName.ToString());
            }

            return sb.ToString();
        }
    }
}
