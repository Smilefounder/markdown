using System;
using System.IO;
using System.Text;

namespace MarkdigEngine.Extensions
{
    public static class ExtensionsHelper
    {
        public static string GetAbsolutePathOfRefFile(string basePath, string filePath, string refPath)
        {
            if (refPath[0] == '~')
            {
                return GetAbsolutePathWithTilde(basePath, refPath);
            }

            var currentDirectory = Path.GetDirectoryName(filePath);

            return NormalizePath(Path.Combine(basePath, currentDirectory, refPath));
        }

        public static string GetAbsolutePathWithTilde(string basePath, string tildePath)
        {
            if (string.IsNullOrEmpty(tildePath))
            {
                throw new ArgumentException($"{nameof(tildePath)} can't be null or empty.");
            }

            if (string.IsNullOrEmpty(basePath))
            {
                throw new ArgumentException($"{nameof(basePath)} can't be null or empty.");
            }

            if (!tildePath.StartsWith("~"))
            {
                throw new ArgumentException($"{nameof(tildePath)} should start with ~.");
            }

            if (!Path.IsPathRooted(basePath))
            {
                throw new ArgumentException($"{nameof(basePath)} should be an absolute path.");
            }

            return GetAbsolutePathWithTildeCore(basePath, tildePath);
        }

        public static string NormalizePath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return path;
            }

            return Path.GetFullPath(path).Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }

        #region private methods
        private static string GetAbsolutePathWithTildeCore(string basePath, string tildePath)
        {
            var index = 1;
            var ch = tildePath[index];
            while (ch == '/' || ch == '\\')
            {
                index++;
                ch = tildePath[index];
            }

            if (index == tildePath.Length)
            {
                return basePath;
            }

            var pathWithoutTilde = tildePath.Substring(index);

            return NormalizePath(Path.Combine(basePath, pathWithoutTilde));
        }
        #endregion
    }
}
