// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.IO
{
    partial class FileSystemInfo
    {
        private FileStatus _fileStatus;

        protected FileSystemInfo()
        {
            FileStatus.Initialize(ref _fileStatus, this is DirectoryInfo);
        }

        internal void Invalidate() => _fileStatus.Invalidate();

        internal unsafe void Init(ref FileStatus fileStatus)
        {
            _fileStatus = fileStatus;
            _fileStatus.EnsureStatInitialized(FullPath);
        }

        public FileAttributes Attributes
        {
            get => _fileStatus.GetAttributes(FullPath, Name);
            set => _fileStatus.SetAttributes(FullPath, value);
        }

        internal bool ExistsCore => _fileStatus.GetExists(FullPath);

        internal DateTimeOffset CreationTimeCore
        {
            get => _fileStatus.GetCreationTime(FullPath);
            set => _fileStatus.SetCreationTime(FullPath, value);
        }

        internal DateTimeOffset LastAccessTimeCore
        {
            get => _fileStatus.GetLastAccessTime(FullPath);
            set => _fileStatus.SetLastAccessTime(FullPath, value);
        }

        internal DateTimeOffset LastWriteTimeCore
        {
            get => _fileStatus.GetLastWriteTime(FullPath);
            set => _fileStatus.SetLastWriteTime(FullPath, value);
        }

        internal long LengthCore => _fileStatus.GetLength(FullPath);

        public void Refresh() => _fileStatus.Refresh(FullPath);

        internal static void ThrowNotFound(string path)
        {
            // Windows distinguishes between whether the directory or the file isn't found,
            // and throws a different exception in these cases.  We attempt to approximate that
            // here; there is a race condition here, where something could change between
            // when the error occurs and our checks, but it's the best we can do, and the
            // worst case in such a race condition (which could occur if the file system is
            // being manipulated concurrently with these checks) is that we throw a
            // FileNotFoundException instead of DirectoryNotFoundException.

            bool directoryError = !Directory.Exists(Path.GetDirectoryName(PathHelpers.TrimEndingDirectorySeparator(path)));
            throw Interop.GetExceptionForIoErrno(new Interop.ErrorInfo(Interop.Error.ENOENT), path, directoryError);
        }
    }
}
