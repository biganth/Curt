﻿#region Copyright
// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2012
// by DotNetNuke Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web;

using DotNetNuke.Common;
using DotNetNuke.Common.Internal;
using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel;
using DotNetNuke.Data;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Users;
using DotNetNuke.Instrumentation;
using DotNetNuke.Services.FileSystem.Internal;

using ICSharpCode.SharpZipLib.Zip;

namespace DotNetNuke.Services.FileSystem
{
    /// <summary>
    /// Exposes methods to manage files.
    /// </summary>
    public class FileManager : ComponentBase<IFileManager, FileManager>, IFileManager
    {
        #region Properties

        private IDictionary<string, string> _contentTypes;

        protected IDictionary<string, string> ContentTypes
        {
            get
            {
                if (_contentTypes == null)
                {
                    _contentTypes = new Dictionary<string, string>();
                    _contentTypes.Add("txt", "text/plain");
                    _contentTypes.Add("htm", "text/html");
                    _contentTypes.Add("html", "text/html");
                    _contentTypes.Add("rtf", "text/richtext");
                    _contentTypes.Add("jpg", "image/jpeg");
                    _contentTypes.Add("jpeg", "image/jpeg");
                    _contentTypes.Add("gif", "image/gif");
                    _contentTypes.Add("bmp", "image/bmp");
                    _contentTypes.Add("png", "image/png");
                    _contentTypes.Add("ico", "image/x-icon");
                    _contentTypes.Add("mp3", "audio/mpeg");
                    _contentTypes.Add("wma", "audio/x-ms-wma");
                    _contentTypes.Add("mpg", "video/mpeg");
                    _contentTypes.Add("mpeg", "video/mpeg");
                    _contentTypes.Add("avi", "video/avi");
                    _contentTypes.Add("mp4", "video/mp4");
                    _contentTypes.Add("wmv", "video/x-ms-wmv");
                    _contentTypes.Add("pdf", "application/pdf");
                    _contentTypes.Add("doc", "application/msword");
                    _contentTypes.Add("dot", "application/msword");
                    _contentTypes.Add("docx", "application/msword");
                    _contentTypes.Add("dotx", "application/msword");
                    _contentTypes.Add("csv", "text/csv");
                    _contentTypes.Add("xls", "application/x-msexcel");
                    _contentTypes.Add("xlt", "application/x-msexcel");
                    _contentTypes.Add("xlsx", "application/x-msexcel");
                    _contentTypes.Add("xltx", "application/x-msexcel");
                    _contentTypes.Add("ppt", "application/vnd.ms-powerpoint");
                    _contentTypes.Add("pps", "application/vnd.ms-powerpoint");
                    _contentTypes.Add("pptx", "application/vnd.ms-powerpoint");
                    _contentTypes.Add("ppsx", "application/vnd.ms-powerpoint");
                }

                return _contentTypes;
            }
        }
        #endregion
        #region Constants

        private const int BufferSize = 4096;

        #endregion

        #region Constructor

        internal FileManager()
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds a file to the specified folder.
        /// </summary>
        /// <param name="folder">The folder where to add the file.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="fileContent">The content of the file.</param>
        /// <returns>A <see cref="DotNetNuke.Services.FileSystem.IFileInfo">IFileInfo</see> as specified by the parameters.</returns>
        public virtual IFileInfo AddFile(IFolderInfo folder, string fileName, Stream fileContent)
        {
            DnnLog.MethodEntry();

            return AddFile(folder, fileName, fileContent, true);
        }

        /// <summary>
        /// Adds a file to the specified folder.
        /// </summary>
        /// <param name="folder">The folder where to add the file.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="fileContent">The content of the file.</param>
        /// <param name="overwrite">Indicates if the file has to be over-written if it exits.</param>
        /// <returns>A <see cref="DotNetNuke.Services.FileSystem.IFileInfo">IFileInfo</see> as specified by the parameters.</returns>
        public virtual IFileInfo AddFile(IFolderInfo folder, string fileName, Stream fileContent, bool overwrite)
        {
            DnnLog.MethodEntry();

            return AddFile(folder, fileName, fileContent, overwrite, false, GetContentType(Path.GetExtension(fileName)));
        }

        /// <summary>
        /// Adds a file to the specified folder.
        /// </summary>
        /// <param name="folder">The folder where to add the file.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="fileContent">The content of the file.</param>
        /// <param name="overwrite">Indicates if the file has to be over-written if it exists.</param>
        /// <param name="checkPermissions">Indicates if permissions have to be met.</param>
        /// <param name="contentType">The content type of the file.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when folder, fileName or fileContent are null.</exception>
        /// <exception cref="DotNetNuke.Services.FileSystem.FolderProviderException">Thrown when the underlying system throw an exception.</exception>
        /// <exception cref="DotNetNuke.Services.FileSystem.InvalidFileExtensionException">Thrown when the extension of the specified file is not allowed.</exception>
        /// <exception cref="DotNetNuke.Services.FileSystem.NoSpaceAvailableException">Thrown when the portal has no space available to store the specified file.</exception>
        /// <exception cref="DotNetNuke.Services.FileSystem.PermissionsNotMetException">Thrown when permissions are not met.</exception>
        /// <returns>A <see cref="DotNetNuke.Services.FileSystem.IFileInfo">IFileInfo</see> as specified by the parameters.</returns>
        public virtual IFileInfo AddFile(IFolderInfo folder, string fileName, Stream fileContent, bool overwrite, bool checkPermissions, string contentType)
        {
            DnnLog.MethodEntry();

            Requires.NotNull("folder", folder);
            Requires.NotNullOrEmpty("fileName", fileName);

            if (checkPermissions && !FolderPermissionControllerWrapper.Instance.CanAddFolder(folder))
            {
                throw new PermissionsNotMetException(Localization.Localization.GetExceptionMessage("AddFilePermissionsNotMet", "Permissions are not met. The file has not been added."));
            }

            if (!IsAllowedExtension(fileName))
            {
                throw new InvalidFileExtensionException(string.Format(Localization.Localization.GetExceptionMessage("AddFileExtensionNotAllowed", "The extension '{0}' is not allowed. The file has not been added."), Path.GetExtension(fileName)));
            }

            var folderMapping = FolderMappingController.Instance.GetFolderMapping(folder.FolderMappingID);
            var folderProvider = FolderProvider.Instance(folderMapping.FolderProviderType);

            bool needToWriteFile = fileContent != null && (overwrite || !folderProvider.FileExists(folder, fileName));
            bool usingSeekableStream = false;

            if (fileContent != null && !needToWriteFile && FileExists(folder, fileName))
            {
                return GetFile(folder, fileName);
            }

            var file = new FileInfo
            {
                PortalId = folder.PortalID,
                FileName = fileName,
                Extension = Path.GetExtension(fileName).Replace(".", ""),
                Width = Null.NullInteger,
                Height = Null.NullInteger,
                ContentType = contentType,
                Folder =
                    TestableGlobals.Instance.GetSubFolderPath(
                        Path.Combine(folder.PhysicalPath, fileName), folder.PortalID),
                FolderId = folder.FolderID,
                LastModificationTime = DateTime.Now
            };

            try
            {
                if (needToWriteFile)
                {
                    if (!fileContent.CanSeek)
                    {
                        fileContent = GetSeekableStream(fileContent);
                        usingSeekableStream = true;
                    }

                    file.Size = (int)fileContent.Length;

                    if (!PortalControllerWrapper.Instance.HasSpaceAvailable(folder.PortalID, file.Size))
                    {
                        throw new NoSpaceAvailableException(
                            Localization.Localization.GetExceptionMessage("AddFileNoSpaceAvailable",
                                                                          "The portal has no space available to store the specified file. The file has not been added."));
                    }
                }
                else
                {
                    file.Size = (int)folderProvider.GetFileSize(file);
                }

                file.FileId = DataProvider.Instance().AddFile(file.PortalId,
                                                              file.UniqueId,
                                                              file.VersionGuid,
                                                              file.FileName,
                                                              file.Extension,
                                                              file.Size,
                                                              file.Width,
                                                              file.Height,
                                                              file.ContentType,
                                                              file.Folder,
                                                              file.FolderId,
                                                              GetCurrentUserID(),
                                                              file.SHA1Hash,
                                                              file.LastModificationTime);
                try
                {
                    if (needToWriteFile)
                    {
                        folderProvider.AddFile(folder, fileName, fileContent);
                    }

                    file.LastModificationTime = folderProvider.GetLastModificationTime(file);
                }
                catch (Exception ex)
                {
                    DnnLog.Error(ex);

                    DataProvider.Instance().DeleteFile(file.PortalId, file.FileName, file.FolderId);

                    throw new FolderProviderException(
                        Localization.Localization.GetExceptionMessage("AddFileUnderlyingSystemError",
                                                                      "The underlying system threw an exception. The file has not been added."),
                        ex);
                }

                return UpdateFile(file, false);
            }
            finally
            {
                if (usingSeekableStream)
                {
                    fileContent.Dispose();
                }
            }
        }

        /// <summary>
        /// Copies the specified file into the specified folder.
        /// </summary>
        /// <param name="file">The file to copy.</param>
        /// <param name="destinationFolder">The folder where to copy the file to.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when file or destinationFolder are null.</exception>
        /// <returns>A <see cref="DotNetNuke.Services.FileSystem.IFileInfo">IFileInfo</see> with the information of the copied file.</returns>
        public virtual IFileInfo CopyFile(IFileInfo file, IFolderInfo destinationFolder)
        {
            DnnLog.MethodEntry();

            Requires.NotNull("file", file);
            Requires.NotNull("destinationFolder", destinationFolder);

            if (file.FolderMappingID == destinationFolder.FolderMappingID)
            {
                if (!FolderPermissionControllerWrapper.Instance.CanAddFolder(destinationFolder))
                {
                    throw new PermissionsNotMetException(Localization.Localization.GetExceptionMessage("CopyFilePermissionsNotMet", "Permissions are not met. The file has not been copied."));
                }

                if (!PortalControllerWrapper.Instance.HasSpaceAvailable(destinationFolder.PortalID, file.Size))
                {
                    throw new NoSpaceAvailableException(Localization.Localization.GetExceptionMessage("CopyFileNoSpaceAvailable", "The portal has no space available to store the specified file. The file has not been copied."));
                }

                var folderMapping = FolderMappingController.Instance.GetFolderMapping(file.FolderMappingID);
                try
                {
                    FolderProvider.Instance(folderMapping.FolderProviderType).CopyFile(file.Folder, file.FileName, destinationFolder.FolderPath, folderMapping);
                }
                catch (Exception ex)
                {
                    DnnLog.Error(ex);
                    throw new FolderProviderException(Localization.Localization.GetExceptionMessage("CopyFileUnderlyingSystemError", "The underlying system throw an exception. The file has not been copied."), ex);
                }

                var fileId = DataProvider.Instance().AddFile(
                    file.PortalId,
                    file.UniqueId,
                    file.VersionGuid,
                    file.FileName,
                    file.Extension,
                    file.Size,
                    file.Width,
                    file.Height,
                    file.ContentType,
                    destinationFolder.FolderPath,
                    destinationFolder.FolderID,
                    GetCurrentUserID(),
                    file.SHA1Hash,
                    DateTime.Now);

                return GetFile(fileId);
            }

            using (var fileContent = GetFileContent(file))
            {
                return AddFile(destinationFolder, file.FileName, fileContent, true, true, file.ContentType);
            }
        }

        /// <summary>
        /// Deletes the specified file.
        /// </summary>
        /// <param name="file">The file to delete.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when file is null.</exception>
        /// <exception cref="DotNetNuke.Services.FileSystem.FolderProviderException">Thrown when the underlying system throw an exception.</exception>
        public virtual void DeleteFile(IFileInfo file)
        {
            DnnLog.MethodEntry();

            Requires.NotNull("file", file);

            var folderMapping = FolderMappingController.Instance.GetFolderMapping(file.FolderMappingID);

            try
            {
                FolderProvider.Instance(folderMapping.FolderProviderType).DeleteFile(file);
            }
            catch (Exception ex)
            {
                DnnLog.Error(ex);

                throw new FolderProviderException(Localization.Localization.GetExceptionMessage("DeleteFileUnderlyingSystemError", "The underlying system threw an exception. The file has not been deleted."), ex);
            }

            DataProvider.Instance().DeleteFile(file.PortalId, file.FileName, file.FolderId);
        }

        /// <summary>
        /// Deletes the specified files.
        /// </summary>
        /// <param name="files">The files to delete.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when files is null.</exception>
        public virtual void DeleteFiles(IEnumerable<IFileInfo> files)
        {
            DnnLog.MethodEntry();

            Requires.NotNull("files", files);

            foreach (var file in files)
            {
                DeleteFile(file);
            }
        }

        /// <summary>
        /// Checks the existence of the specified file in the specified folder.
        /// </summary>
        /// <param name="folder">The folder where to check the existence of the file.</param>
        /// <param name="fileName">The file name to check the existence of.</param>
        /// <returns>A bool value indicating whether the file exists or not in the specified folder.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when folder is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when fileName is null or empty.</exception>
        /// <exception cref="DotNetNuke.Services.FileSystem.FolderProviderException">Thrown when the underlying system throw an exception.</exception>
        public virtual bool FileExists(IFolderInfo folder, string fileName)
        {
            DnnLog.MethodEntry();

            Requires.NotNull("folder", folder);
            Requires.NotNullOrEmpty("fileName", fileName);

            var file = GetFile(folder, fileName);
            var existsFile = file != null;
            var folderMapping = FolderMappingController.Instance.GetFolderMapping(folder.PortalID, folder.FolderMappingID);

            try
            {
                existsFile = existsFile && FolderProvider.Instance(folderMapping.FolderProviderType).FileExists(folder, fileName);
            }
            catch (Exception ex)
            {
                DnnLog.Error(ex);

                throw new FolderProviderException(Localization.Localization.GetExceptionMessage("UnderlyingSystemError", "The underlying system threw an exception."), ex);
            }

            return existsFile;
        }

        /// <summary>
        /// Gets the Content Type for the specified file extension.
        /// </summary>
        /// <param name="extension">The file extension.</param>
        /// <returns>The Content Type for the specified extension.</returns>
        public virtual string GetContentType(string extension)
        {
            DnnLog.MethodEntry();

            if (string.IsNullOrEmpty(extension)) return "application/octet-stream";

            var key = extension.TrimStart('.').ToLowerInvariant();
            return ContentTypes.ContainsKey(key) ? ContentTypes[key] : "application/octet-stream";
        }

        /// <summary>
        /// Gets the file metadata for the specified file.
        /// </summary>
        /// <param name="fileID">The file identifier.</param>
        /// <returns>The <see cref="DotNetNuke.Services.FileSystem.IFileInfo">IFileInfo</see> object with the metadata of the specified file.</returns>
        public virtual IFileInfo GetFile(int fileID)
        {
            DnnLog.MethodEntry();

            var strCacheKey = "GetFileById" + fileID;
            var file = DataCache.GetCache(strCacheKey);
            if (file == null)
            {
                file = CBOWrapper.Instance.FillObject<FileInfo>(DataProvider.Instance().GetFileById(fileID));
                if (file != null)
                {
                    var intCacheTimeout = 20 * Convert.ToInt32(GetPerformanceSetting());
                    DataCache.SetCache(strCacheKey, file, TimeSpan.FromMinutes(intCacheTimeout));
                }
            }
            return (IFileInfo)file;
        }

        /// <summary>
        /// Gets the file metadata for the specified file.
        /// </summary>
        /// <param name="folder">The folder where the file is stored.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>The <see cref="DotNetNuke.Services.FileSystem.IFileInfo">IFileInfo</see> object with the metadata of the specified file.</returns>
        public virtual IFileInfo GetFile(IFolderInfo folder, string fileName)
        {
            DnnLog.MethodEntry();

            Requires.NotNullOrEmpty("fileName", fileName);
            Requires.NotNull("folder", folder);

            return CBOWrapper.Instance.FillObject<FileInfo>(DataProvider.Instance().GetFile(fileName, folder.FolderID));
        }

        /// <summary>
        /// Gets the file metadata for the specified file.
        /// </summary>
        /// <param name="portalId">The portal ID or Null.NullInteger for the Host</param>
        /// <param name="relativePath">Relative path to the file.</param>
        /// <remarks>Host and portal settings commonly return a relative path to a file.  This method uses that relative path to fetch file metadata.</remarks>
        /// <returns>The <see cref="DotNetNuke.Services.FileSystem.IFileInfo">IFileInfo</see> object with the metadata of the specified file.</returns>
        public virtual IFileInfo GetFile(int portalId, string relativePath)
        {
            DnnLog.MethodEntry();

            Requires.NotNullOrEmpty("relateivePath", relativePath);

            var folderPath = "";
            var seperatorIndex = relativePath.LastIndexOf('/');

            if (seperatorIndex > 0)
            {
                folderPath = relativePath.Substring(0, seperatorIndex + 1);
            }

            var folderInfo = FolderManager.Instance.GetFolder(portalId, folderPath);
            if (folderInfo == null)
            {
                return null;
            }
            else
            {
                var fileName = relativePath.Substring(folderPath.Length);
                return GetFile(folderInfo, fileName);
            }
        }

        /// <summary>
        /// Gets the content of the specified file.
        /// </summary>
        /// <param name="file">The file to get the content from.</param>
        /// <returns>A stream with the content of the file.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when file is null.</exception>
        /// <exception cref="DotNetNuke.Services.FileSystem.FolderProviderException">Thrown when the underlying system throw an exception.</exception>
        public virtual Stream GetFileContent(IFileInfo file)
        {
            DnnLog.MethodEntry();

            Requires.NotNull("file", file);

            Stream stream = null;

            var folder = FolderManager.Instance.GetFolder(file.FolderId);

            if (folder != null)
            {
                var folderMapping = FolderMappingController.Instance.GetFolderMapping(folder.FolderMappingID);

                if (folderMapping != null)
                {
                    try
                    {
                        stream = FolderProvider.Instance(folderMapping.FolderProviderType).GetFileStream(folder, file.FileName);
                    }
                    catch (Exception ex)
                    {
                        DnnLog.Error(ex);

                        throw new FolderProviderException(Localization.Localization.GetExceptionMessage("UnderlyingSystemError", "The underlying system threw an exception"), ex);
                    }
                }
            }

            return stream;
        }

        /// <summary>
        /// Gets a seekable Stream based on the specified non-seekable Stream.
        /// </summary>
        /// <param name="stream">A non-seekable Stream.</param>
        /// <returns>A seekable Stream.</returns>
        public virtual Stream GetSeekableStream(Stream stream)
        {
            DnnLog.MethodEntry();

            Requires.NotNull("stream", stream);

            if (stream.CanSeek) return stream;

            var folderPath = GetHostMapPath();
            string filePath;

            do
            {
                filePath = Path.Combine(folderPath, Path.GetRandomFileName()) + ".resx";
            } while (File.Exists(filePath));

            var fileStream = GetAutoDeleteFileStream(filePath);

            var array = new byte[BufferSize];

            int bytesRead;
            while ((bytesRead = stream.Read(array, 0, BufferSize)) > 0)
            {
                fileStream.Write(array, 0, bytesRead);
            }

            fileStream.Position = 0;

            return fileStream;
        }

        /// <summary>
        /// Gets the direct Url to the file.
        /// </summary>
        /// <param name="file">The file to get the Url.</param>
        /// <returns>The direct Url to the file.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when file is null.</exception>
        /// <exception cref="DotNetNuke.Services.FileSystem.FolderProviderException">Thrown when the underlying system throw an exception.</exception>
        public string GetUrl(IFileInfo file)
        {
            DnnLog.MethodEntry();

            Requires.NotNull("file", file);

            var folderMapping = FolderMappingController.Instance.GetFolderMapping(file.PortalId, file.FolderMappingID);

            try
            {
                return FolderProvider.Instance(folderMapping.FolderProviderType).GetFileUrl(file);
            }
            catch (Exception ex)
            {
                DnnLog.Error(ex);

                throw new FolderProviderException(Localization.Localization.GetExceptionMessage("UnderlyingSystemError", "The underlying system threw an exception."), ex);
            }
        }

        /// <summary>
        /// Moves the specified file into the specified folder.
        /// </summary>
        /// <param name="file">The file to move.</param>
        /// <param name="destinationFolder">The folder where to move the file to.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when file or destinationFolder are null.</exception>
        /// <exception cref="DotNetNuke.Services.FileSystem.FolderProviderException">Thrown when the underlying system throw an exception.</exception>
        /// <exception cref="DotNetNuke.Services.FileSystem.InvalidFileExtensionException">Thrown when the extension of the specified file is not allowed.</exception>
        /// <exception cref="DotNetNuke.Services.FileSystem.NoSpaceAvailableException">Thrown when the portal has no space available to store the specified file.</exception>
        /// <exception cref="DotNetNuke.Services.FileSystem.PermissionsNotMetException">Thrown when permissions are not met.</exception>
        /// <returns>An <see cref="DotNetNuke.Services.FileSystem.IFileInfo">IFileInfo</see> with the information of the moved file.</returns>
        public virtual IFileInfo MoveFile(IFileInfo file, IFolderInfo destinationFolder)
        {
            DnnLog.MethodEntry();

            Requires.NotNull("file", file);
            Requires.NotNull("destinationFolder", destinationFolder);

            //check whether the file is already in the dest folder.
            if(file.FolderId == destinationFolder.FolderID)
            {
                return file;
            }

            var existingFile = GetFile(destinationFolder, file.FileName);
            if (existingFile != null)
            {
                DeleteFile(existingFile);
            }

            var destinationFolderMapping = FolderMappingController.Instance.GetFolderMapping(destinationFolder.FolderMappingID);
            var destinationFolderProvider = FolderProvider.Instance(destinationFolderMapping.FolderProviderType);

            using (var fileContent = GetFileContent(file))
            {
                try
                {
                    if (!fileContent.CanSeek)
                    {
                        using (var seekableStream = GetSeekableStream(fileContent))
                        {
                            destinationFolderProvider.AddFile(destinationFolder, file.FileName, seekableStream);
                        }
                    }
                    else
                    {
                        destinationFolderProvider.AddFile(destinationFolder, file.FileName, fileContent);
                    }
                }
                catch (Exception ex)
                {
                    DnnLog.Error(ex);
                    throw new FolderProviderException(Localization.Localization.GetExceptionMessage("UnderlyingSystemError", "The underlying system threw an exception."), ex);
                }
            }

            var sourceFolderMapping = file.FolderMappingID == destinationFolder.FolderMappingID
                ? destinationFolderMapping
                : FolderMappingController.Instance.GetFolderMapping(file.FolderMappingID);

            var sourceFolderProvider = sourceFolderMapping.FolderProviderType == destinationFolderMapping.FolderProviderType
                ? destinationFolderProvider
                : FolderProvider.Instance(sourceFolderMapping.FolderProviderType);

            try
            {
                // We can't delete the file until the fileContent resource has been released
                sourceFolderProvider.DeleteFile(file);
            }
            catch (Exception ex)
            {
                DnnLog.Error(ex);
                throw new FolderProviderException(Localization.Localization.GetExceptionMessage("UnderlyingSystemError", "The underlying system threw an exception."), ex);
            }

            file.FolderId = destinationFolder.FolderID;
            file.Folder = destinationFolder.FolderPath;

            return UpdateFile(file);
        }

        /// <summary>
        /// Renames the specified file.
        /// </summary>
        /// <param name="file">The file to rename</param>
        /// <param name="newFileName">The new filename to assign to the file.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when file is null.</exception>
        /// <exception cref="DotNetNuke.Services.FileSystem.FileAlreadyExistsException">Thrown when the folder already contains a file with the same name.</exception>
        /// <returns>An <see cref="DotNetNuke.Services.FileSystem.IFileInfo">IFileInfo</see> with the information of the renamed file.</returns>
        public virtual IFileInfo RenameFile(IFileInfo file, string newFileName)
        {
            DnnLog.MethodEntry();

            Requires.NotNull("file", file);
            Requires.NotNullOrEmpty("newFileName", newFileName);

            if (file.FileName == newFileName) return file;

            if (!IsAllowedExtension(newFileName))
            {
                throw new InvalidFileExtensionException(string.Format(Localization.Localization.GetExceptionMessage("AddFileExtensionNotAllowed", "The extension '{0}' is not allowed. The file has not been added."), Path.GetExtension(newFileName)));
            }

            var folder = FolderManager.Instance.GetFolder(file.FolderId);

            if (FileExists(folder, newFileName))
            {
                throw new FileAlreadyExistsException(Localization.Localization.GetExceptionMessage("RenameFileAlreadyExists", "This folder already contains a file with the same name. The file has not been renamed."));
            }

            var folderMapping = FolderMappingController.Instance.GetFolderMapping(file.FolderMappingID);

            try
            {
                FolderProvider.Instance(folderMapping.FolderProviderType).RenameFile(file, newFileName);
            }
            catch (Exception ex)
            {
                DnnLog.Error(ex);

                throw new FolderProviderException(Localization.Localization.GetExceptionMessage("RenameFileUnderlyingSystemError", "The underlying system threw an exception. The file has not been renamed."), ex);
            }

            file.FileName = newFileName;
            return UpdateFile(file);
        }

        /// <summary>
        /// Sets the specified FileAttributes of the file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="fileAttributes">The file attributes to add.</param>
        public void SetAttributes(IFileInfo file, FileAttributes fileAttributes)
        {
            DnnLog.MethodEntry();

            Requires.NotNull("file", file);

            var folderMapping = FolderMappingController.Instance.GetFolderMapping(file.FolderMappingID);

            try
            {
                FolderProvider.Instance(folderMapping.FolderProviderType).SetFileAttributes(file, fileAttributes);
            }
            catch (Exception ex)
            {
                DnnLog.Error(ex);
                throw new FolderProviderException(Localization.Localization.GetExceptionMessage("UnderlyingSystemError", "The underlying system threw an exception."), ex);
            }
        }

        /// <summary>
        /// Extracts the files and folders contained in the specified zip file to the folder where the file belongs.
        /// </summary>
        /// <param name="file">The file to unzip.</param>
        /// <exception cref="System.ArgumentException">Thrown when file is not a zip compressed file.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown when file or destination folder are null.</exception>
        public virtual void UnzipFile(IFileInfo file)
        {
            DnnLog.MethodEntry();

            Requires.NotNull("file", file);

            var destinationFolder = FolderManager.Instance.GetFolder(file.FolderId);

            UnzipFile(file, destinationFolder);
        }

        /// <summary>
        /// Extracts the files and folders contained in the specified zip file to the specified folder.
        /// </summary>
        /// <param name="file">The file to unzip.</param>
        /// <param name="destinationFolder">The folder to unzip too</param>
        /// <exception cref="System.ArgumentException">Thrown when file is not a zip compressed file.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown when file or destination folder are null.</exception>
        public virtual void UnzipFile(IFileInfo file, IFolderInfo destinationFolder)
        {
            DnnLog.MethodEntry();

            Requires.NotNull("file", file);
            Requires.NotNull("destinationFolder", destinationFolder);

            if (file.Extension != "zip")
            {
                throw new ArgumentException(Localization.Localization.GetExceptionMessage("InvalidZipFile", "The file specified is not a zip compressed file."));
            }

            ExtractFolders(file, destinationFolder);
            ExtractFiles(file, destinationFolder);
        }

        /// <summary>
        /// Updates the metadata of the specified file.
        /// </summary>
        /// <param name="file">The file to update.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when file is null.</exception>
        /// <returns>A <see cref="DotNetNuke.Services.FileSystem.IFileInfo">IFileInfo</see> as the updated file.</returns>
        public virtual IFileInfo UpdateFile(IFileInfo file)
        {
            return UpdateFile(file, true);
        }

        /// <summary>
        /// Regenerates the hash and updates the metadata of the specified file.
        /// </summary>
        /// <param name="file">The file to update.</param>
        /// <param name="fileContent">Stream used to regenerate the hash.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when file is null.</exception>
        /// <returns>A <see cref="DotNetNuke.Services.FileSystem.IFileInfo">IFileInfo</see> as the updated file.</returns>
        public virtual IFileInfo UpdateFile(IFileInfo file, Stream fileContent)
        {
            DnnLog.MethodEntry();

            Requires.NotNull("file", file);

            if (fileContent != null)
            {
                if (IsImageFile(file))
                {
                    Image image = null;

                    try
                    {
                        image = GetImageFromStream(fileContent);

                        file.Width = image.Width;
                        file.Height = image.Height;
                    }
                    catch
                    {
                        file.ContentType = "application/octet-stream";
                    }
                    finally
                    {
                        if (image != null)
                        {
                            image.Dispose();
                        }
                    }
                }

                file.SHA1Hash = GetHash(fileContent);
            }
            //get file size from folder provider.
            try
            {
                var folderMapping = FolderMappingController.Instance.GetFolderMapping(file.FolderMappingID);
                if (folderMapping != null)
                {
                    var folderProvider = FolderProvider.Instance(folderMapping.FolderProviderType);
                    file.Size = (int)folderProvider.GetFileSize(file);
                    file.LastModificationTime = folderProvider.GetLastModificationTime(file);
                }
            }
            catch (Exception ex)
            {
                DnnLog.Error(ex);
            }

            return UpdateFile(file);
        }

        /// <summary>
        /// Writes the content of the specified file into the specified stream.
        /// </summary>
        /// <param name="file">The file to write into the stream.</param>
        /// <param name="stream">The stream to write to.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when file or stream are null.</exception>
        public virtual void WriteFile(IFileInfo file, Stream stream)
        {
            DnnLog.MethodEntry();

            Requires.NotNull("file", file);
            Requires.NotNull("stream", stream);

            using (var srcStream = GetFileContent(file))
            {
                const int bufferSize = 4096;
                var buffer = new byte[bufferSize];

                int bytesRead;
                while ((bytesRead = srcStream.Read(buffer, 0, bufferSize)) > 0)
                {
                    stream.Write(buffer, 0, bytesRead);
                }
            }
        }

        /// <summary>
        /// Downloads the specified file.
        /// </summary>
        /// <param name="file">The file to download.</param>
        /// <param name="contentDisposition">Indicates how to display the document once downloaded.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when file is null.</exception>
        /// <exception cref="DotNetNuke.Services.FileSystem.PermissionsNotMetException">Thrown when permissions are not met.</exception>
        public virtual void WriteFileToResponse(IFileInfo file, ContentDisposition contentDisposition)
        {
            DnnLog.MethodEntry();

            Requires.NotNull("file", file);

            var folder = FolderManager.Instance.GetFolder(file.FolderId);

            if (!FolderPermissionControllerWrapper.Instance.CanViewFolder(folder))
            {
                throw new PermissionsNotMetException(Localization.Localization.GetExceptionMessage("WriteFileToResponsePermissionsNotMet", "Permissions are not met. The file cannot be downloaded."));
            }

            if (IsFileAutoSyncEnabled())
            {
                AutoSyncFile(file);
            }

            WriteFileToHttpContext(file, contentDisposition);
        }

        #endregion

        #region Internal Methods

        /// <summary>This member is reserved for internal use and is not intended to be used directly from your code.</summary>
        internal virtual void AutoSyncFile(IFileInfo file)
        {
            var folderMapping = FolderMappingController.Instance.GetFolderMapping(file.FolderMappingID);

            var newFileSize = FolderProvider.Instance(folderMapping.FolderProviderType).GetFileSize(file);
            if (newFileSize > 0)
            {
                if (file.Size != newFileSize)
                {
                    file.Size = (int)newFileSize;

                    using (var fileContent = GetFileContent(file))
                    {
                        UpdateFile(file, fileContent);
                    }
                }
            }
            else
            {
                DeleteFile(file);
            }
        }

        /// <summary>This member is reserved for internal use and is not intended to be used directly from your code.</summary>
        internal virtual void ExtractFiles(IFileInfo file, IFolderInfo destinationFolder)
        {
            var folderManager = FolderManager.Instance;

            ZipInputStream zipInputStream = null;

            try
            {
                using (var fileContent = GetFileContent(file))
                {
                    zipInputStream = new ZipInputStream(fileContent);

                    var zipEntry = zipInputStream.GetNextEntry();

                    while (zipEntry != null)
                    {
                        if (!zipEntry.IsDirectory)
                        {
                            var fileName = Path.GetFileName(zipEntry.Name);

                            IFolderInfo parentFolder;
                            if (zipEntry.Name.IndexOf("/") == -1)
                            {
                                parentFolder = destinationFolder;
                            }
                            else
                            {
                                var folderPath = destinationFolder.FolderPath + zipEntry.Name.Substring(0, zipEntry.Name.LastIndexOf("/") + 1);
                                parentFolder = folderManager.GetFolder(file.PortalId, folderPath);
                            }

                            try
                            {
                                AddFile(parentFolder, fileName, zipInputStream, true);
                            }
                            catch (PermissionsNotMetException exc)
                            {
                                DnnLog.Warn(exc);
                            }
                            catch (NoSpaceAvailableException exc)
                            {
                                DnnLog.Warn(exc);
                            }
                            catch (InvalidFileExtensionException exc)
                            {
                                DnnLog.Warn(exc);
                            }
                            catch (Exception exc)
                            {
                                DnnLog.Error(exc);
                            }
                        }

                        zipEntry = zipInputStream.GetNextEntry();
                    }
                }
            }
            finally
            {
                if (zipInputStream != null)
                {
                    zipInputStream.Close();
                    zipInputStream.Dispose();
                }
            }
        }

        /// <summary>This member is reserved for internal use and is not intended to be used directly from your code.</summary>
        internal virtual void ExtractFolders(IFileInfo file, IFolderInfo destinationFolder)
        {
            var folderManager = FolderManager.Instance;

            ZipInputStream zipInputStream = null;

            try
            {
                using (var fileContent = GetFileContent(file))
                {
                    zipInputStream = new ZipInputStream(fileContent);

                    var zipEntry = zipInputStream.GetNextEntry();
                    var sortedFolders = new ArrayList();

                    while (zipEntry != null)
                    {
                        if (zipEntry.IsDirectory)
                        {
                            sortedFolders.Add(zipEntry.Name);
                        }

                        zipEntry = zipInputStream.GetNextEntry();
                    }

                    sortedFolders.Sort();

                    string folderPath;

                    IFolderInfo parentFolder;

                    var folderMappingController = FolderMappingController.Instance;
                    var folderMapping = folderMappingController.GetFolderMapping(destinationFolder.FolderMappingID);

                    foreach (string zipFolder in sortedFolders)
                    {
                        folderPath = PathUtils.Instance.RemoveTrailingSlash(zipFolder);

                        if (folderPath.IndexOf("/") == -1)
                        {
                            var newFolderPath = destinationFolder.FolderPath + PathUtils.Instance.FormatFolderPath(folderPath);
                            if (!folderManager.FolderExists(file.PortalId, newFolderPath))
                            {
                                folderManager.AddFolder(folderMapping, newFolderPath);
                            }
                        }
                        else
                        {
                            var zipFolders = folderPath.Split('/');

                            parentFolder = destinationFolder;

                            for (var i = 0; i < zipFolders.Length; i++)
                            {
                                var newFolderPath = parentFolder.FolderPath + PathUtils.Instance.FormatFolderPath(zipFolders[i]);
                                if (!folderManager.FolderExists(file.PortalId, newFolderPath))
                                {
                                    folderManager.AddFolder(folderMappingController.GetFolderMapping(parentFolder.FolderMappingID), newFolderPath);
                                }

                                parentFolder = folderManager.GetFolder(file.PortalId, newFolderPath);
                            }
                        }
                    }
                }
            }
            finally
            {
                if (zipInputStream != null)
                {
                    zipInputStream.Close();
                    zipInputStream.Dispose();
                }
            }
        }

        /// <summary>This member is reserved for internal use and is not intended to be used directly from your code.</summary>
        internal virtual Stream GetAutoDeleteFileStream(string filePath)
        {
            return new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read, BufferSize, FileOptions.DeleteOnClose);
        }

        /// <summary>This member is reserved for internal use and is not intended to be used directly from your code.</summary>
        internal virtual int GetCurrentUserID()
        {
            return UserController.GetCurrentUserInfo().UserID;
        }

        /// <summary>This member is reserved for internal use and is not intended to be used directly from your code.</summary>
        /// <returns>SHA1 hash of the file</returns>
        internal virtual string GetHash(Stream stream)
        {
            Requires.NotNull("stream", stream);

            var hashText = "";
            string hexValue;

            var hashData = SHA1.Create().ComputeHash(stream);

            foreach (var b in hashData)
            {
                hexValue = b.ToString("X").ToLower();
                //Lowercase for compatibility on case-sensitive systems
                hashText += (hexValue.Length == 1 ? "0" : "") + hexValue;
            }

            return hashText;
        }

        /// <summary>
        /// Gets the hash of a file
        /// </summary>
        /// <param name="fileInfo">The file info.</param>
        /// <returns>SHA1 hash of the file</returns>
        internal virtual string GetHash(IFileInfo fileInfo)
        {
            using (var stream = GetFileContent(fileInfo))
            {
                return GetHash(stream);
            }
        }

        /// <summary>This member is reserved for internal use and is not intended to be used directly from your code.</summary>
        internal virtual string GetHostMapPath()
        {
            return Globals.HostMapPath;
        }

        /// <summary>This member is reserved for internal use and is not intended to be used directly from your code.</summary>
        internal virtual Image GetImageFromStream(Stream stream)
        {
            return Image.FromStream(stream);
        }

        /// <summary>This member is reserved for internal use and is not intended to be used directly from your code.</summary>
        internal virtual Globals.PerformanceSettings GetPerformanceSetting()
        {
            return Host.PerformanceSetting;
        }

        /// <summary>This member is reserved for internal use and is not intended to be used directly from your code.</summary>
        internal virtual bool IsAllowedExtension(string fileName)
        {
            var extension = Path.GetExtension(fileName);

            //regex matches a dot followed by 1 or more chars followed by a semi-colon
            //regex is meant to block files like "foo.asp;.png" which can take advantage
            //of a vulnerability in IIS6 which treasts such files as .asp, not .png
            return !string.IsNullOrEmpty(extension)
                   && Host.AllowedExtensionWhitelist.IsAllowedExtension(extension)
                   && !Regex.IsMatch(fileName, @"\..+;");
        }

        /// <summary>This member is reserved for internal use and is not intended to be used directly from your code.</summary>
        internal virtual bool IsFileAutoSyncEnabled()
        {
            return Host.EnableFileAutoSync;
        }

        /// <summary>This member is reserved for internal use and is not intended to be used directly from your code.</summary>
        internal virtual bool IsImageFile(IFileInfo file)
        {
            return (Globals.glbImageFileTypes + ",").IndexOf(file.Extension.ToLower().Replace(".", "") + ",") > -1;
        }

        /// <summary>This member is reserved for internal use and is not intended to be used directly from your code.</summary>
        internal virtual void WriteFileToHttpContext(IFileInfo file, ContentDisposition contentDisposition)
        {
            var scriptTimeOut = HttpContext.Current.Server.ScriptTimeout;

            HttpContext.Current.Server.ScriptTimeout = int.MaxValue;
            var objResponse = HttpContext.Current.Response;

            objResponse.ClearContent();
            objResponse.ClearHeaders();

            switch (contentDisposition)
            {
                case ContentDisposition.Attachment:
                    objResponse.AppendHeader("content-disposition", "attachment; filename=\"" + file.FileName + "\"");
                    break;
                case ContentDisposition.Inline:
                    objResponse.AppendHeader("content-disposition", "inline; filename=\"" + file.FileName + "\"");
                    break;
                default:
                    throw new ArgumentOutOfRangeException("contentDisposition");
            }

            objResponse.AppendHeader("Content-Length", file.Size.ToString());
            objResponse.ContentType = GetContentType(file.Extension.Replace(".", ""));

            try
            {
                using (var fileContent = GetFileContent(file))
                {
                    WriteStream(objResponse, fileContent);
                }
            }
            catch (Exception ex)
            {
                DnnLog.Error(ex);

                objResponse.Write("Error : " + ex.Message);
            }

            objResponse.Flush();
            objResponse.End();

            HttpContext.Current.Server.ScriptTimeout = scriptTimeOut;
        }

        /// <summary>This member is reserved for internal use and is not intended to be used directly from your code.</summary>
        internal virtual void WriteStream(HttpResponse objResponse, Stream objStream)
        {
            var bytBuffer = new byte[10000];
            try
            {
                if (objResponse.IsClientConnected)
                {
                    var intLength = objStream.Read(bytBuffer, 0, 10000);

                    while (objResponse.IsClientConnected && intLength > 0)
                    {
                        objResponse.OutputStream.Write(bytBuffer, 0, intLength);
                        objResponse.Flush();

                        intLength = objStream.Read(bytBuffer, 0, 10000);
                    }
                }
            }
            catch (Exception ex)
            {
                DnnLog.Error(ex);
                objResponse.Write("Error : " + ex.Message);
            }
            finally
            {
                if (objStream != null)
                {
                    objStream.Close();
                    objStream.Dispose();
                }
            }
        }

        /// <summary>
        /// Update file info to database.
        /// </summary>
        /// <param name="file">File info.</param>
        /// <param name="updateLazyload">Whether to update the lazyload properties: Width,Height,Sha1Hash.</param>
        internal virtual IFileInfo UpdateFile(IFileInfo file, bool updateLazyload)
        {
            DnnLog.MethodEntry();

            Requires.NotNull("file", file);

            DataProvider.Instance().UpdateFile(file.FileId,
                                               file.VersionGuid,
                                               file.FileName,
                                               file.Extension,
                                               file.Size,
                                               updateLazyload ? file.Width : Null.NullInteger,
                                               updateLazyload ? file.Height : Null.NullInteger,
                                               file.ContentType,
                                               file.Folder,
                                               file.FolderId,
                                               GetCurrentUserID(),
                                               updateLazyload ? file.SHA1Hash : Null.NullString,
                                               file.LastModificationTime);

            DataCache.RemoveCache("GetFileById" + file.FileId);

            return file;
        }

        #endregion
    }
}
