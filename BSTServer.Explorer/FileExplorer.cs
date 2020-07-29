using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BSTServer.Explorer
{
    public class FileExplorer
    {
        private bool _initialized;
        private DirectoryInfo _homeDirectory;

        public List<VisibleRule> VisibleRule { get; set; } = new List<VisibleRule>();

        public void InitializeHomeDirectory(Uri uri)
        {
            if (!uri.IsAbsoluteUri)
                throw new UriFormatException("Absolute URI is required");

            var stringPath = uri.AbsolutePath;
            if (!Directory.Exists(stringPath))
                throw new DirectoryNotFoundException("Cannot find specified directory: " + stringPath);

            _homeDirectory = new DirectoryInfo(stringPath);
            _initialized = true;
        }

        public DirectoryObject GetTargetDirectoryInfo(string userRole, string userName, params string[] relativePath)
        {
            if (!_initialized) throw new Exception("Home directory is not initialized.");
            if (relativePath?.Contains("..") == true)
                throw new UriFormatException("Relative path is not supported");
            var targetPath = Path.Combine(new[] { _homeDirectory.FullName }.Concat(relativePath ?? new string[] { }).ToArray());
            targetPath = new DirectoryInfo(targetPath).FullName;
            if (!Directory.Exists(targetPath))
                throw new DirectoryNotFoundException("Cannot find specified directory: " + GetRelativePathString(targetPath));
            if (!CheckAccess(ref targetPath, userRole, userName))
                throw new DirectoryNotFoundException("No access to specified directory: " + GetRelativePathString(targetPath));

            var di = new DirectoryInfo(targetPath);

            var directories = di.EnumerateDirectories().Select(k => new DirectoryDesc()
            {
                Name = k.Name,
                RelativePath = GetRelativePathString(k.FullName),
                CreationTime = k.CreationTime,
                LastAccessTime = k.LastAccessTime,
                LastWriteTime = k.LastWriteTime
            });

            var files = di.EnumerateFiles().Select(k => new FileDesc()
            {
                Name = k.Name,
                RelativePath = GetRelativePathString(k.FullName),
                CreationTime = k.CreationTime,
                LastAccessTime = k.LastAccessTime,
                LastWriteTime = k.LastWriteTime,
                Size = k.Length,
            });

            return new DirectoryObject()
            {
                Directories = directories.ToList(),
                Files = files.ToList(),
                RelativePath = GetRelativePathString(targetPath)
            };
        }

        private string GetRelativePathString(string targetPath)
        {
            if (targetPath.StartsWith(_homeDirectory.FullName))
                return "~" + Path.DirectorySeparatorChar + targetPath.Replace(_homeDirectory.FullName, "")
                    .TrimStart(Path.DirectorySeparatorChar);
            return null;
        }

        private bool CheckAccess(ref string targetPath, string userRole, string userName)
        {
            var dirInfo = new DirectoryInfo(targetPath);
            targetPath = dirInfo.FullName;
            if (!targetPath.Contains(_homeDirectory.FullName))
                return false;
            // todo
            return true;
        }
    }

    public class VisibleRule
    {
        public string MinRole { get; set; }
        public string UserName { get; set; }
        public string Path { get; set; }

        public bool IsVisible { get; set; }
        public bool IsSelfOrSub { get; set; }
        public string[] SubItemFilters { get; set; }
    }

    public class DirectoryObject
    {
        public char DirectorySeparatorChar { get; set; } = Path.DirectorySeparatorChar;
        public bool CanCreateFiles { get; set; } = true;
        public List<DirectoryDesc> Directories { get; set; } = new List<DirectoryDesc>();
        public List<FileDesc> Files { get; set; } = new List<FileDesc>();
        public string RelativePath { get; set; }
    }

    public class ExplorerDesc
    {
        public DateTime CreationTime { get; set; }
        public DateTime LastWriteTime { get; set; }
        public DateTime LastAccessTime { get; set; }
        public string Name { get; set; }
        public string RelativePath { get; set; }
    }

    public class FileDesc : ExplorerDesc
    {
        public bool CanDisable { get; set; } = true;
        public bool CanDelete { get; set; } = true;
        public bool CanEdit { get; set; } = true;
        public long Size { get; set; }
        public bool IsEnabled { get; set; } = true;
    }

    public class DirectoryDesc : ExplorerDesc
    {
    }
}
