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
using System.Xml.XPath;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Installer.Dependencies;
using DotNetNuke.Services.Installer.Packages;

#endregion

namespace DotNetNuke.Services.Installer.Installers
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The PackageInstaller class is an Installer for Packages
    /// </summary>
    /// <history>
    /// 	[cnurse]	01/16/2008  created
    /// </history>
    /// -----------------------------------------------------------------------------
    public class PackageInstaller : ComponentInstallerBase
    {
		#region Private Members
	
        private readonly SortedList<int, ComponentInstallerBase> _componentInstallers = new SortedList<int, ComponentInstallerBase>();
        private PackageInfo _installedPackage;
		
		#endregion

		#region Constructors
        
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// This Constructor creates a new PackageInstaller instance
        /// </summary>
        /// <param name="package">A PackageInfo instance</param>
        /// <history>
        /// 	[cnurse]	01/21/2008  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public PackageInstaller(PackageInfo package)
        {
            IsValid = true;
            DeleteFiles = Null.NullBoolean;
            Package = package;
            if (!string.IsNullOrEmpty(package.Manifest))
            {
				//Create an XPathDocument from the Xml
                var doc = new XPathDocument(new StringReader(package.Manifest));
                XPathNavigator nav = doc.CreateNavigator().SelectSingleNode("package");
                ReadComponents(nav);
            }
            else
            {
                ComponentInstallerBase installer = InstallerFactory.GetInstaller(package.PackageType);
                if (installer != null)
                {
					//Set package
                    installer.Package = package;

                    //Set type
                    installer.Type = package.PackageType;
                    _componentInstallers.Add(0, installer);
                }
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// This Constructor creates a new PackageInstaller instance
        /// </summary>
        /// <param name="info">An InstallerInfo instance</param>
        /// <param name="packageManifest">The manifest as a string</param>
        /// <history>
        /// 	[cnurse]	01/16/2008  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public PackageInstaller(string packageManifest, InstallerInfo info)
        {
            IsValid = true;
            DeleteFiles = Null.NullBoolean;
            Package = new PackageInfo(info);
            Package.Manifest = packageManifest;

            if (!string.IsNullOrEmpty(packageManifest))
            {
				//Create an XPathDocument from the Xml
                var doc = new XPathDocument(new StringReader(packageManifest));
                XPathNavigator nav = doc.CreateNavigator().SelectSingleNode("package");
                ReadManifest(nav);
            }
        }
		
		#endregion

		#region Public Properties

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets whether the Packages files are deleted when uninstalling the
        /// package
        /// </summary>
        /// <value>A Boolean value</value>
        /// <history>
        /// 	[cnurse]	01/31/2008  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public bool DeleteFiles { get; set; }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets whether the Package is Valid
        /// </summary>
        /// <value>A Boolean value</value>
        /// <history>
        /// 	[cnurse]	01/16/2008  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public bool IsValid { get; private set; }
		
		#endregion

		#region Private Methods

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// The CheckSecurity method checks whether the user has the appropriate security
        /// </summary>
        /// <history>
        /// 	[cnurse]	09/04/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        private void CheckSecurity()
        {
            PackageType type = PackageController.GetPackageType(Package.PackageType);
            if (type == null)
            {
                //This package type not registered
				Log.Logs.Clear();
                Log.AddFailure(Util.SECURITY_NotRegistered + " - " + Package.PackageType);
                IsValid = false;
            }
            else
            {
                if (type.SecurityAccessLevel > Package.InstallerInfo.SecurityAccessLevel)
                {
                    Log.Logs.Clear();
                    Log.AddFailure(Util.SECURITY_Installer);
                    IsValid = false;
                }
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// The ReadComponents method reads the components node of the manifest file.
        /// </summary>
        /// <history>
        /// 	[cnurse]	01/21/2008  created
        /// </history>
        /// -----------------------------------------------------------------------------
        private void ReadComponents(XPathNavigator manifestNav)
        {
            foreach (XPathNavigator componentNav in manifestNav.CreateNavigator().Select("components/component"))
            {
                //Set default order to next value (ie the same as the size of the collection)
				int order = _componentInstallers.Count;
				
                string type = componentNav.GetAttribute("type", "");
                if (InstallMode == InstallMode.Install)
                {
                    string installOrder = componentNav.GetAttribute("installOrder", "");
                    if (!string.IsNullOrEmpty(installOrder))
                    {
                        order = int.Parse(installOrder);
                    }
                }
                else
                {
                    string unInstallOrder = componentNav.GetAttribute("unInstallOrder", "");
                    if (!string.IsNullOrEmpty(unInstallOrder))
                    {
                        order = int.Parse(unInstallOrder);
                    }
                }
                if (Package.InstallerInfo != null)
                {
                    Log.AddInfo(Util.DNN_ReadingComponent + " - " + type);
                }
                ComponentInstallerBase installer = InstallerFactory.GetInstaller(componentNav, Package);
                if (installer == null)
                {
                    Log.AddFailure(Util.EXCEPTION_InstallerCreate);
                }
                else
                {
                    _componentInstallers.Add(order, installer);
                    Package.InstallerInfo.AllowableFiles += ", " + installer.AllowableFiles;
                }
            }
        }

        private string ReadTextFromFile(string source)
        {
            string strText = Null.NullString;
            if (Package.InstallerInfo.InstallMode != InstallMode.ManifestOnly)
            {
				//Load from file
                strText = FileSystemUtils.ReadFile(Package.InstallerInfo.TempInstallFolder + "\\" + source);
            }
            return strText;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// The ValidateVersion method checks whether the package is already installed
        /// </summary>
        /// <history>
        /// 	[cnurse]	07/24/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        private void ValidateVersion(string strVersion)
        {
            if (string.IsNullOrEmpty(strVersion))
            {
                IsValid = false;
                return;
            }
            Package.Version = new Version(strVersion);
            if (_installedPackage != null)
            {
                Package.InstalledVersion = _installedPackage.Version;
                Package.InstallerInfo.PackageID = _installedPackage.PackageID;

                if (Package.InstalledVersion > Package.Version)
                {
                    Log.AddFailure(Util.INSTALL_Version + " - " + Package.InstalledVersion.ToString(3));
                    IsValid = false;
                }
                else if (Package.InstalledVersion == Package.Version)
                {
                    Package.InstallerInfo.Installed = true;
                    Package.InstallerInfo.PortalID = _installedPackage.PortalID;
                }
            }
        }
		
		#endregion

	    #region Public Methods

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// The Commit method commits the package installation
        /// </summary>
        /// <history>
        /// 	[cnurse]	08/01/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public override void Commit()
        {
            for (int index = 0; index <= _componentInstallers.Count - 1; index++)
            {
                ComponentInstallerBase compInstaller = _componentInstallers.Values[index];
                if (compInstaller.Version >= Package.InstalledVersion && compInstaller.Completed)
                {
                    compInstaller.Commit();
                }
            }
            if (Log.Valid)
            {
                Log.AddInfo(Util.INSTALL_Committed);
            }
            else
            {
                Log.AddFailure(Util.INSTALL_Aborted);
            }
            Package.InstallerInfo.PackageID = Package.PackageID;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// The Install method installs the components of the package
        /// </summary>
        /// <history>
        /// 	[cnurse]	07/25/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public override void Install()
        {
            bool isCompleted = true;
            try
            {
				//Save the Package Information
                if (_installedPackage != null)
                {
                    Package.PackageID = _installedPackage.PackageID;
                }
				
                //Save Package
                PackageController.SavePackage(Package);

                //Iterate through all the Components
                for (int index = 0; index <= _componentInstallers.Count - 1; index++)
                {
                    ComponentInstallerBase compInstaller = _componentInstallers.Values[index];
                    if ((_installedPackage == null) || (compInstaller.Version > Package.InstalledVersion) || (Package.InstallerInfo.RepairInstall))
                    {
                        Log.AddInfo(Util.INSTALL_Start + " - " + compInstaller.Type);
                        compInstaller.Install();
                        if (compInstaller.Completed)
                        {
                            if (compInstaller.Skipped)
                            {
                                Log.AddInfo(Util.COMPONENT_Skipped + " - " + compInstaller.Type);
                            }
                            else
                            {
                                Log.AddInfo(Util.COMPONENT_Installed + " - " + compInstaller.Type);
                            }
                        }
                        else
                        {
                            Log.AddFailure(Util.INSTALL_Failed + " - " + compInstaller.Type);
                            isCompleted = false;
                            break;
                        }
                    }
                }
            }
            catch (Exception)
            {
                Log.AddFailure(Util.INSTALL_Aborted + " - " + Package.Name);
            }
            if (isCompleted)
            {
				//All components successfully installed so Commit any pending changes
                Commit();
            }
            else
            {
				//There has been a failure so Rollback
                Rollback();
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// The ReadManifest method reads the manifest file and parses it into components.
        /// </summary>
        /// <history>
        /// 	[cnurse]	07/24/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public override void ReadManifest(XPathNavigator manifestNav)
        {
            //Get Name Property
            Package.Name = Util.ReadAttribute(manifestNav, "name", Log, Util.EXCEPTION_NameMissing);

            //Get Type
            Package.PackageType = Util.ReadAttribute(manifestNav, "type", Log, Util.EXCEPTION_TypeMissing);

            //If Skin or Container then set PortalID
            if (Package.PackageType == "Skin" || Package.PackageType == "Container")
            {
                Package.PortalID = Package.InstallerInfo.PortalID;
            }
            CheckSecurity();
            if (!IsValid)
            {
                return;
            }
			
            //Attempt to get the Package from the Data Store (see if its installed)
            _installedPackage = PackageController.GetPackageByName(Package.PortalID, Package.Name);

            //Get IsSystem
            Package.IsSystemPackage = bool.Parse(Util.ReadAttribute(manifestNav, "isSystem", false, Log, "", bool.FalseString));

            //Get Version
            string strVersion = Util.ReadAttribute(manifestNav, "version", Log, Util.EXCEPTION_VersionMissing);
            ValidateVersion(strVersion);
            if (!IsValid)
            {
                return;
            }
            Log.AddInfo(Util.DNN_ReadingPackage + " - " + Package.PackageType + " - " + Package.Name);
            Package.FriendlyName = Util.ReadElement(manifestNav, "friendlyName", Package.Name);
            Package.Description = Util.ReadElement(manifestNav, "description");

            XPathNavigator foldernameNav = null;
            Package.FolderName = String.Empty;
            switch (Package.PackageType)
            {
                case "Module":
                case "Auth_System":
                    foldernameNav = manifestNav.SelectSingleNode("components/component/files");
                    if (foldernameNav != null) Package.FolderName = Util.ReadElement(foldernameNav, "basePath").Replace('\\', '/');
                    break;
                case "Container":
                    foldernameNav = manifestNav.SelectSingleNode("components/component/containerFiles");
                    if (foldernameNav != null) Package.FolderName = Globals.glbContainersPath +  Util.ReadElement(foldernameNav, "containerName").Replace('\\', '/');
                    break;
                case "Skin":
                    foldernameNav = manifestNav.SelectSingleNode("components/component/skinFiles");
                    if (foldernameNav != null) Package.FolderName = Globals.glbSkinsPath + Util.ReadElement(foldernameNav, "skinName").Replace('\\', '/');
                    break;
                default:
                    break;
            }

            //Get Icon
            XPathNavigator iconFileNav= manifestNav.SelectSingleNode("iconFile");
            if (iconFileNav != null)
            {
                if ((iconFileNav.Value != string.Empty) && (Package.PackageType == "Module" || Package.PackageType == "Auth_System" || Package.PackageType == "Container" || Package.PackageType == "Skin"))
                {
                    if (iconFileNav.Value.StartsWith("~/"))
                    {
                        Package.IconFile = iconFileNav.Value;
                    }
                    else
                    {
                        Package.IconFile = (String.IsNullOrEmpty(Package.FolderName) ? "" :  Package.FolderName + "/") + iconFileNav.Value;
                        Package.IconFile = (!Package.IconFile.StartsWith("~/")) ? "~/" + Package.IconFile : Package.IconFile;
                    }
                }
            }
			//Get Author
            XPathNavigator authorNav = manifestNav.SelectSingleNode("owner");
            if (authorNav != null)
            {
                Package.Owner = Util.ReadElement(authorNav, "name");
                Package.Organization = Util.ReadElement(authorNav, "organization");
                Package.Url = Util.ReadElement(authorNav, "url");
                Package.Email = Util.ReadElement(authorNav, "email");
            }
			
            //Get License
            XPathNavigator licenseNav = manifestNav.SelectSingleNode("license");
            if (licenseNav != null)
            {
                string licenseSrc = Util.ReadAttribute(licenseNav, "src");
                if (string.IsNullOrEmpty(licenseSrc))
                {
					//Load from element
                    Package.License = licenseNav.Value;
                }
                else
                {
                    Package.License = ReadTextFromFile(licenseSrc);
                }
            }
            if (string.IsNullOrEmpty(Package.License))
            {
				//Legacy Packages have no license
                Package.License = Util.PACKAGE_NoLicense;
            }
			
            //Get Release Notes
            XPathNavigator relNotesNav = manifestNav.SelectSingleNode("releaseNotes");
            if (relNotesNav != null)
            {
                string relNotesSrc = Util.ReadAttribute(relNotesNav, "src");
                if (string.IsNullOrEmpty(relNotesSrc))
                {
					//Load from element
                    Package.ReleaseNotes = relNotesNav.Value;
                }
                else
                {
                    Package.ReleaseNotes = ReadTextFromFile(relNotesSrc);
                }
            }
            if (string.IsNullOrEmpty(Package.ReleaseNotes))
            {
				//Legacy Packages have no Release Notes
				Package.ReleaseNotes = Util.PACKAGE_NoReleaseNotes;
            }
			
            //Parse the Dependencies
            IDependency dependency = null;
            foreach (XPathNavigator dependencyNav in manifestNav.CreateNavigator().Select("dependencies/dependency"))
            {
                dependency = DependencyFactory.GetDependency(dependencyNav);
                if (!dependency.IsValid)
                {
                    Log.AddFailure(dependency.ErrorMessage);
                    return;
                }
            }
			
            //Read Components
            ReadComponents(manifestNav);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// The Rollback method rolls back the package installation
        /// </summary>
        /// <history>
        /// 	[cnurse]	07/31/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public override void Rollback()
        {
            for (int index = 0; index <= _componentInstallers.Count - 1; index++)
            {
                ComponentInstallerBase compInstaller = _componentInstallers.Values[index];
                if (compInstaller.Version > Package.InstalledVersion && compInstaller.Completed)
                {
                    Log.AddInfo(Util.COMPONENT_RollingBack + " - " + compInstaller.Type);
                    compInstaller.Rollback();
                    Log.AddInfo(Util.COMPONENT_RolledBack + " - " + compInstaller.Type);
                }
            }
			
            //If Previously Installed Package exists then we need to update the DataStore with this 
            if (_installedPackage == null)
            {
				//No Previously Installed Package - Delete newly added Package
                PackageController.DeletePackage(Package);
            }
            else
            {
				//Previously Installed Package - Rollback to Previously Installed
                PackageController.SavePackage(_installedPackage);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// The Uninstall method uninstalls the components of the package
        /// </summary>
        /// <history>
        /// 	[cnurse]	07/25/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public override void UnInstall()
        {
			//Iterate through all the Components
            for (int index = 0; index <= _componentInstallers.Count - 1; index++)
            {
                ComponentInstallerBase compInstaller = _componentInstallers.Values[index];
                var fileInstaller = compInstaller as FileInstaller;
                if (fileInstaller != null)
                {
                    fileInstaller.DeleteFiles = DeleteFiles;
                }
                Log.ResetFlags();
                Log.AddInfo(Util.UNINSTALL_StartComp + " - " + compInstaller.Type);
                compInstaller.UnInstall();
                Log.AddInfo(Util.COMPONENT_UnInstalled + " - " + compInstaller.Type);
                if (Log.Valid)
                {
                    Log.AddInfo(Util.UNINSTALL_SuccessComp + " - " + compInstaller.Type);
                }
                else
                {
                    Log.AddWarning(Util.UNINSTALL_WarningsComp + " - " + compInstaller.Type);
                }
            }
			
            //Remove the Package information from the Data Store
            PackageController.DeletePackage(Package);
        }
		
		#endregion
    }
}
