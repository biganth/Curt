#region Copyright
// 
// DotNetNukeŽ - http://www.dotnetnuke.com
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
#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Installer.Log;
using DotNetNuke.Services.Installer.Packages;

using ICSharpCode.SharpZipLib.Zip;

#endregion

namespace DotNetNuke.Services.Installer.Writers
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The PackageWriter class
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	01/30/2008	created
    /// </history>
    /// -----------------------------------------------------------------------------
    public class PackageWriterBase
    {
#region "Private Members"

        private readonly Dictionary<string, InstallFile> _AppCodeFiles = new Dictionary<string, InstallFile>();
        private readonly Dictionary<string, InstallFile> _Assemblies = new Dictionary<string, InstallFile>();
        private readonly SortedList<string, InstallFile> _CleanUpFiles = new SortedList<string, InstallFile>();
        private readonly Dictionary<string, InstallFile> _Files = new Dictionary<string, InstallFile>();
        private readonly Dictionary<string, InstallFile> _Resources = new Dictionary<string, InstallFile>();
        private readonly Dictionary<string, InstallFile> _Scripts = new Dictionary<string, InstallFile>();
        private readonly List<string> _Versions = new List<string>();
        private string _BasePath = Null.NullString;
        private PackageInfo _Package;
		
		#endregion

	#region "Constructors"

        protected PackageWriterBase()
        {
        }

        public PackageWriterBase(PackageInfo package)
        {
            _Package = package;
            _Package.AttachInstallerInfo(new InstallerInfo());
        }
		
		#endregion

		#region "Protected Properties"
		
        protected virtual Dictionary<string, string> Dependencies
        {
            get
            {
                return new Dictionary<string, string>();
            }
        }
		
		#endregion

		#region "Public Properties"


        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets a Dictionary of AppCodeFiles that should be included in the Package
        /// </summary>
        /// <value>A Dictionary(Of String, InstallFile)</value>
        /// <history>
        /// 	[cnurse]	02/12/2008  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public Dictionary<string, InstallFile> AppCodeFiles
        {
            get
            {
                return _AppCodeFiles;
            }
        }

		 /// -----------------------------------------------------------------------------
		 /// <summary>
		 /// Gets and sets the Path for the Package's app code files
		 /// </summary>
		 /// <value>A String</value>
		 /// <history>
		 /// 	[cnurse]	02/12/2008  created
		 /// </history>
		 /// -----------------------------------------------------------------------------
         public string AppCodePath { get; set; }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets a Dictionary of Assemblies that should be included in the Package
        /// </summary>
        /// <value>A Dictionary(Of String, InstallFile)</value>
        /// <history>
        /// 	[cnurse]	01/31/2008  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public Dictionary<string, InstallFile> Assemblies
        {
            get
            {
                return _Assemblies;
            }
        }

		 /// -----------------------------------------------------------------------------
		 /// <summary>
		 /// Gets and sets the Path for the Package's assemblies
		 /// </summary>
		 /// <value>A String</value>
		 /// <history>
		 /// 	[cnurse]	01/31/2008  created
		 /// </history>
		 /// -----------------------------------------------------------------------------
         public string AssemblyPath { get; set; }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the Base Path for the Package
        /// </summary>
        /// <value>A String</value>
        /// <history>
        /// 	[cnurse]	01/31/2008  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public string BasePath
        {
            get
            {
                return _BasePath;
            }
            set
            {
                _BasePath = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets a Dictionary of CleanUpFiles that should be included in the Package
        /// </summary>
        /// <value>A Dictionary(Of String, InstallFile)</value>
        /// <history>
        /// 	[cnurse]	02/21/2008  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public SortedList<string, InstallFile> CleanUpFiles
        {
            get
            {
                return _CleanUpFiles;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets a Dictionary of Files that should be included in the Package
        /// </summary>
        /// <value>A Dictionary(Of String, InstallFile)</value>
        /// <history>
        /// 	[cnurse]	01/31/2008  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public Dictionary<string, InstallFile> Files
        {
            get
            {
                return _Files;
            }
        }

 /// -----------------------------------------------------------------------------
 /// <summary>
 /// Gets and sets whether a project file is found in the folder
 /// </summary>
 /// <value>A String</value>
 /// <history>
 /// 	[cnurse]	01/31/2008  created
 /// </history>
 /// -----------------------------------------------------------------------------
        public bool HasProjectFile { get; set; }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets whether to include Assemblies
        /// </summary>
        /// <value>A Boolean</value>
        /// <history>
        /// 	[cnurse]	02/06/2008  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public virtual bool IncludeAssemblies
        {
            get
            {
                return true;
            }
        }

		 /// <summary>
		 /// Gets and sets whether there are any errors in parsing legacy packages
		 /// </summary>
		 /// <value></value>
		 /// <returns></returns>
		 /// <remarks></remarks>
         public string LegacyError { get; set; }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the Logger
        /// </summary>
        /// <value>An Logger object</value>
        /// <history>
        /// 	[cnurse]	07/31/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public Logger Log
        {
            get
            {
                return Package.Log;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the associated Package
        /// </summary>
        /// <value>An PackageInfo object</value>
        /// <history>
        /// 	[cnurse]	07/24/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public PackageInfo Package
        {
            get
            {
                return _Package;
            }
            set
            {
                _Package = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets a Dictionary of Resources that should be included in the Package
        /// </summary>
        /// <value>A Dictionary(Of String, InstallFile)</value>
        /// <history>
        /// 	[cnurse]	02/11/2008  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public Dictionary<string, InstallFile> Resources
        {
            get
            {
                return _Resources;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets a Dictionary of Scripts that should be included in the Package
        /// </summary>
        /// <value>A Dictionary(Of String, InstallFile)</value>
        /// <history>
        /// 	[cnurse]	01/31/2008  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public Dictionary<string, InstallFile> Scripts
        {
            get
            {
                return _Scripts;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets a List of Versions that should be included in the Package
        /// </summary>
        /// <value>A List(Of String)</value>
        /// <history>
        /// 	[cnurse]	01/31/2008  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public List<string> Versions
        {
            get
            {
                return _Versions;
            }
        }
		
		#endregion

		#region "Private Methods"

        private void AddFilesToZip(ZipOutputStream stream, IDictionary<string, InstallFile> files, string basePath)
        {
            foreach (InstallFile packageFile in files.Values)
            {
                string filepath;
                if (string.IsNullOrEmpty(basePath))
                {
                    filepath = Path.Combine(Globals.ApplicationMapPath, packageFile.FullName);
                }
                else
                {
                    filepath = Path.Combine(Path.Combine(Globals.ApplicationMapPath, basePath), packageFile.FullName.Replace(basePath + "\\", ""));
                }
                if (File.Exists(filepath))
                {
                    string packageFilePath = packageFile.Path;
                    if (!string.IsNullOrEmpty(basePath))
                    {
                        packageFilePath = packageFilePath.Replace(basePath + "\\", "");
                    }
                    FileSystemUtils.AddToZip(ref stream, filepath, packageFile.Name, packageFilePath);
                    Log.AddInfo(string.Format(Util.WRITER_SavedFile, packageFile.FullName));
                }
            }
        }

        private void CreateZipFile(string zipFileName)
        {
            int CompressionLevel = 9;
            var zipFile = new FileInfo(zipFileName);

            string ZipFileShortName = zipFile.Name;

            FileStream strmZipFile = null;
            Log.StartJob(Util.WRITER_CreatingPackage);
            try
            {
                Log.AddInfo(string.Format(Util.WRITER_CreateArchive, ZipFileShortName));
                strmZipFile = File.Create(zipFileName);
                ZipOutputStream strmZipStream = null;
                try
                {
                    strmZipStream = new ZipOutputStream(strmZipFile);
                    strmZipStream.SetLevel(CompressionLevel);

                    //Add Files To zip
                    AddFilesToZip(strmZipStream, _Assemblies, "");
                    AddFilesToZip(strmZipStream, _AppCodeFiles, AppCodePath);
                    AddFilesToZip(strmZipStream, _Files, BasePath);
                    AddFilesToZip(strmZipStream, _CleanUpFiles, BasePath);
                    AddFilesToZip(strmZipStream, _Resources, BasePath);
                    AddFilesToZip(strmZipStream, _Scripts, BasePath);
                }
                catch (Exception ex)
                {
                    Exceptions.Exceptions.LogException(ex);
                    Log.AddFailure(string.Format(Util.WRITER_SaveFileError, ex));
                }
                finally
                {
                    if (strmZipStream != null)
                    {
                        strmZipStream.Finish();
                        strmZipStream.Close();
                    }
                }
                Log.EndJob(Util.WRITER_CreatedPackage);
            }
            catch (Exception ex)
            {
                Exceptions.Exceptions.LogException(ex);
                Log.AddFailure(string.Format(Util.WRITER_SaveFileError, ex));
            }
            finally
            {
                if (strmZipFile != null)
                {
                    strmZipFile.Close();
                }
            }
        }

        private void WritePackageEndElement(XmlWriter writer)
        {
			//Close components Element
            writer.WriteEndElement();

            //Close package Element
            writer.WriteEndElement();
        }

        private void WritePackageStartElement(XmlWriter writer)
        {
			//Start package Element
            writer.WriteStartElement("package");
            writer.WriteAttributeString("name", Package.Name);
            writer.WriteAttributeString("type", Package.PackageType);
            writer.WriteAttributeString("version", Package.Version.ToString(3));

            //Write FriendlyName
            writer.WriteElementString("friendlyName", Package.FriendlyName);

            //Write Description
            writer.WriteElementString("description", Package.Description);
            writer.WriteElementString("iconFile", Util.ParsePackageIconFileName(Package));
			
            //Write Author
            writer.WriteStartElement("owner");

            writer.WriteElementString("name", Package.Owner);
            writer.WriteElementString("organization", Package.Organization);
            writer.WriteElementString("url", Package.Url);
            writer.WriteElementString("email", Package.Email);

            //Write Author End
            writer.WriteEndElement();

            //Write License
            writer.WriteElementString("license", Package.License);

            //Write Release Notes
            writer.WriteElementString("releaseNotes", Package.ReleaseNotes);

            //Write Dependencies
            if (Dependencies.Count > 0)
            {
                writer.WriteStartElement("dependencies");
                foreach (KeyValuePair<string, string> kvp in Dependencies)
                {
                    writer.WriteStartElement("dependency");
                    writer.WriteAttributeString("type", kvp.Key);
                    writer.WriteString(kvp.Value);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
			
            //Write components Element
            writer.WriteStartElement("components");
        }
		
		#endregion

		#region "Protected Methods"

        protected virtual void AddFile(string fileName)
        {
            AddFile(new InstallFile(fileName, Package.InstallerInfo));
        }

        protected virtual void AddFile(string fileName, string sourceFileName)
        {
            AddFile(new InstallFile(fileName, sourceFileName, Package.InstallerInfo));
        }

        protected virtual void ConvertLegacyManifest(XPathNavigator legacyManifest, XmlWriter writer)
        {
        }

        protected virtual void GetFiles(bool includeSource, bool includeAppCode)
        {
            string baseFolder = Path.Combine(Globals.ApplicationMapPath, BasePath);
            if (Directory.Exists(baseFolder))
            {
				//Create the DirectoryInfo object
                var folderInfo = new DirectoryInfo(baseFolder);

                //Get the Project File in the folder
                FileInfo[] files = folderInfo.GetFiles("*.??proj");

                if (files.Length == 0) //Assume Dynamic (App_Code based) Module
                {
					//Add the files in the DesktopModules Folder
                    ParseFolder(baseFolder, baseFolder);

                    //Add the files in the AppCode Folder
                    if (includeAppCode)
                    {
                        string appCodeFolder = Path.Combine(Globals.ApplicationMapPath, AppCodePath);
                        ParseFolder(appCodeFolder, appCodeFolder);
                    }
                }
                else //WAP Project File is present
                {
                    HasProjectFile = true;

                    //Parse the Project files (probably only one)
                    foreach (FileInfo projFile in files)
                    {
                        ParseProjectFile(projFile, includeSource);
                    }
                }
            }
        }

        protected virtual void ParseFiles(DirectoryInfo folder, string rootPath)
        {
			//Add the Files in the Folder
            FileInfo[] files = folder.GetFiles();
            foreach (FileInfo file in files)
            {
                string filePath = folder.FullName.Replace(rootPath, "");
                if (filePath.StartsWith("\\"))
                {
                    filePath = filePath.Substring(1);
                }
                if (folder.FullName.ToLowerInvariant().Contains("app_code"))
                {
                    filePath = "[app_code]" + filePath;
                }
                if (file.Extension.ToLowerInvariant() != ".dnn" && (file.Attributes & FileAttributes.Hidden) == 0)
                {
                    AddFile(Path.Combine(filePath, file.Name));
                }
            }
        }

        protected virtual void ParseFolder(string folderName, string rootPath)
        {
            if (Directory.Exists(folderName))
            {
                var folder = new DirectoryInfo(folderName);

                //Recursively parse the subFolders
                DirectoryInfo[] subFolders = folder.GetDirectories();
                foreach (DirectoryInfo subFolder in subFolders)
                {
                    if ((subFolder.Attributes & FileAttributes.Hidden) == 0)
                    {
                        ParseFolder(subFolder.FullName, rootPath);
                    }
                }
				
				//Add the Files in the Folder
                ParseFiles(folder, rootPath);
            }
        }

        protected void ParseProjectFile(FileInfo projFile, bool includeSource)
        {
            string fileName = "";

            //Create an XPathDocument from the Xml
            var doc = new XPathDocument(new FileStream(projFile.FullName, FileMode.Open, FileAccess.Read));
            XPathNavigator rootNav = doc.CreateNavigator();
            var manager = new XmlNamespaceManager(rootNav.NameTable);
            manager.AddNamespace("proj", "http://schemas.microsoft.com/developer/msbuild/2003");
            rootNav.MoveToFirstChild();

            XPathNavigator assemblyNav = rootNav.SelectSingleNode("proj:PropertyGroup/proj:AssemblyName", manager);
            fileName = assemblyNav.Value;
            XPathNavigator buildPathNav = rootNav.SelectSingleNode("proj:PropertyGroup/proj:OutputPath", manager);
            string buildPath = buildPathNav.Value.Replace("..\\", "");
            buildPath = buildPath.Replace(AssemblyPath + "\\", "");
            AddFile(Path.Combine(buildPath, fileName + ".dll"));

            //Check for referenced assemblies
            foreach (XPathNavigator itemNav in rootNav.Select("proj:ItemGroup/proj:Reference", manager))
            {
                fileName = Util.ReadAttribute(itemNav, "Include");
                if (fileName.IndexOf(",") > -1)
                {
                    fileName = fileName.Substring(0, fileName.IndexOf(","));
                }
                if (
                    !(fileName.ToLowerInvariant().StartsWith("system") || fileName.ToLowerInvariant().StartsWith("microsoft") || fileName.ToLowerInvariant() == "dotnetnuke" ||
                      fileName.ToLowerInvariant() == "dotnetnuke.webutility" || fileName.ToLowerInvariant() == "dotnetnuke.webcontrols"))
                {
                    AddFile(fileName + ".dll");
                }
            }
			
            //Add all the files that are classified as None
            foreach (XPathNavigator itemNav in rootNav.Select("proj:ItemGroup/proj:None", manager))
            {
                fileName = Util.ReadAttribute(itemNav, "Include");
                AddFile(fileName);
            }
			
            //Add all the files that are classified as Content
            foreach (XPathNavigator itemNav in rootNav.Select("proj:ItemGroup/proj:Content", manager))
            {
                fileName = Util.ReadAttribute(itemNav, "Include");
                AddFile(fileName);
            }
			
            //Add all the files that are classified as Compile
            if (includeSource)
            {
                foreach (XPathNavigator itemNav in rootNav.Select("proj:ItemGroup/proj:Compile", manager))
                {
                    fileName = Util.ReadAttribute(itemNav, "Include");
                    AddFile(fileName);
                }
            }
        }

        protected virtual void WriteFilesToManifest(XmlWriter writer)
        {
            var fileWriter = new FileComponentWriter(BasePath, Files, Package);
            fileWriter.WriteManifest(writer);
        }

        protected virtual void WriteManifestComponent(XmlWriter writer)
        {
        }
		
		#endregion

		#region "Public Methods"

        public virtual void AddFile(InstallFile file)
        {
            switch (file.Type)
            {
                case InstallFileType.AppCode:
                    _AppCodeFiles[file.FullName.ToLower()] = file;
                    break;
                case InstallFileType.Assembly:
                    _Assemblies[file.FullName.ToLower()] = file;
                    break;
                case InstallFileType.CleanUp:
                    _CleanUpFiles[file.FullName.ToLower()] = file;
                    break;
                case InstallFileType.Script:
                    _Scripts[file.FullName.ToLower()] = file;
                    break;
                default:
                    _Files[file.FullName.ToLower()] = file;
                    break;
            }
            if ((file.Type == InstallFileType.CleanUp || file.Type == InstallFileType.Script) && Regex.IsMatch(file.Name, Util.REGEX_Version))
            {
                string version = Path.GetFileNameWithoutExtension(file.Name);
                if (!_Versions.Contains(version))
                {
                    _Versions.Add(version);
                }
            }
        }

        public void AddResourceFile(InstallFile file)
        {
            _Resources[file.FullName.ToLower()] = file;
        }

        public void CreatePackage(string archiveName, string manifestName, string manifest, bool createManifest)
        {
            if (createManifest)
            {
                WriteManifest(manifestName, manifest);
            }
            AddFile(manifestName);
            CreateZipFile(archiveName);
        }

        public void GetFiles(bool includeSource)
        {
			//Call protected method that does the work
            GetFiles(includeSource, true);
        }

		/// <summary>
		/// WriteManifest writes an existing manifest
		/// </summary>
		/// <param name="manifestName">The name of the manifest file</param>
		/// <param name="manifest">The manifest</param>
		/// <remarks>This overload takes a package manifest and writes it to a file</remarks>
        public void WriteManifest(string manifestName, string manifest)
        {
            XmlWriter writer = XmlWriter.Create(Path.Combine(Globals.ApplicationMapPath, Path.Combine(BasePath, manifestName)), XmlUtils.GetXmlWriterSettings(ConformanceLevel.Fragment));
            Log.StartJob(Util.WRITER_CreatingManifest);
            WriteManifest(writer, manifest);
            Log.EndJob(Util.WRITER_CreatedManifest);
        }

        /// <summary>
        /// WriteManifest writes a package manifest to an XmlWriter
        /// </summary>
        /// <param name="writer">The XmlWriter</param>
        /// <param name="manifest">The manifest</param>
        /// <remarks>This overload takes a package manifest and writes it to a Writer</remarks>
        public void WriteManifest(XmlWriter writer, string manifest)
        {
            WriteManifestStartElement(writer);
            writer.WriteRaw(manifest);

            //Close Dotnetnuke Element
            WriteManifestEndElement(writer);

            //Close Writer
            writer.Close();
        }

        /// <summary>
        /// WriteManifest writes the manifest assoicated with this PackageWriter to a string
        /// </summary>
        /// <param name="packageFragment">A flag that indicates whether to return the package element
        /// as a fragment (True) or whether to add the outer dotnetnuke and packages elements (False)</param>
        /// <returns>The manifest as a string</returns>
        /// <remarks></remarks>
        public string WriteManifest(bool packageFragment)
        {
			//Create a writer to create the processed manifest
            var sb = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(sb, XmlUtils.GetXmlWriterSettings(ConformanceLevel.Fragment));

            WriteManifest(writer, packageFragment);

            //Close XmlWriter
            writer.Close();

            //Return new manifest
            return sb.ToString();
        }

        public void WriteManifest(XmlWriter writer, bool packageFragment)
        {
            Log.StartJob(Util.WRITER_CreatingManifest);

            if (!packageFragment)
            {
				//Start dotnetnuke element
                WriteManifestStartElement(writer);
            }
			
            //Start package Element
            WritePackageStartElement(writer);

            //write Script Component
            if (Scripts.Count > 0)
            {
                var scriptWriter = new ScriptComponentWriter(BasePath, Scripts, Package);
                scriptWriter.WriteManifest(writer);
            }
			
            //write Clean Up Files Component
            if (CleanUpFiles.Count > 0)
            {
                var cleanupFileWriter = new CleanupComponentWriter(BasePath, CleanUpFiles);
                cleanupFileWriter.WriteManifest(writer);
            }
			
            //Write the Custom Component
            WriteManifestComponent(writer);

            //Write Assemblies Component
            if (Assemblies.Count > 0)
            {
                var assemblyWriter = new AssemblyComponentWriter(AssemblyPath, Assemblies, Package);
                assemblyWriter.WriteManifest(writer);
            }
			
            //Write AppCode Files Component
            if (AppCodeFiles.Count > 0)
            {
                var fileWriter = new FileComponentWriter(AppCodePath, AppCodeFiles, Package);
                fileWriter.WriteManifest(writer);
            }
			
            //write Files Component
            if (Files.Count > 0)
            {
                WriteFilesToManifest(writer);
            }
			
            //write ResourceFiles Component
            if (Resources.Count > 0)
            {
                var fileWriter = new ResourceFileComponentWriter(BasePath, Resources, Package);
                fileWriter.WriteManifest(writer);
            }
			
            //Close Package
            WritePackageEndElement(writer);

            if (!packageFragment)
            {
				//Close Dotnetnuke Element
                WriteManifestEndElement(writer);
            }
            Log.EndJob(Util.WRITER_CreatedManifest);
        }

        public static void WriteManifestEndElement(XmlWriter writer)
        {
			//Close packages Element
            writer.WriteEndElement();

            //Close root Element
            writer.WriteEndElement();
        }

        public static void WriteManifestStartElement(XmlWriter writer)
        {
			//Start the new Root Element
            writer.WriteStartElement("dotnetnuke");
            writer.WriteAttributeString("type", "Package");
            writer.WriteAttributeString("version", "5.0");

            //Start packages Element
            writer.WriteStartElement("packages");
        }
		
		#endregion
    }
}
