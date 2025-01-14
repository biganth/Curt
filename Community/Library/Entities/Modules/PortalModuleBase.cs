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
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Web.UI;

using DotNetNuke.Common;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Modules;

#endregion

namespace DotNetNuke.Entities.Modules
{
    /// -----------------------------------------------------------------------------
    /// Project	 : DotNetNuke
    /// Class	 : PortalModuleBase
    ///
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The PortalModuleBase class defines a custom base class inherited by all
    /// desktop portal modules within the Portal.
    ///
    /// The PortalModuleBase class defines portal specific properties
    /// that are used by the portal framework to correctly display portal modules
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    ///		[cnurse]	09/17/2004	Added Documentation
    ///								Modified LocalResourceFile to be Writeable
    ///		[cnurse]	10/21/2004	Modified Settings property to get both
    ///								TabModuleSettings and ModuleSettings
    ///     [cnurse]    12/15/2007  Refactored to support the new IModuleControl
    ///                             Interface
    /// </history>
    /// -----------------------------------------------------------------------------
    public class PortalModuleBase : UserControlBase, IModuleControl
    {
        private string _localResourceFile;
        private ModuleInstanceContext _moduleContext;

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ModuleActionCollection Actions
        {
            get
            {
                return ModuleContext.Actions;
            }
            set
            {
                ModuleContext.Actions = value;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Control ContainerControl
        {
            get
            {
                return Globals.FindControlRecursive(this, "ctr" + ModuleId);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// The EditMode property is used to determine whether the user is in the 
        /// Administrator role
        /// Cache
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        ///   [cnurse] 01/19/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public bool EditMode
        {
            get
            {
                return ModuleContext.EditMode;
            }
        }

        public string HelpURL
        {
            get
            {
                return ModuleContext.HelpURL;
            }
            set
            {
                ModuleContext.HelpURL = value;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsEditable
        {
            get
            {
                return ModuleContext.IsEditable;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ModuleInfo ModuleConfiguration
        {
            get
            {
                return ModuleContext.Configuration;
            }
            set
            {
                ModuleContext.Configuration = value;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int PortalId
        {
            get
            {
                return ModuleContext.PortalId;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int TabId
        {
            get
            {
                return ModuleContext.TabId;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int TabModuleId
        {
            get
            {
                return ModuleContext.TabModuleId;
            }
            set
            {
                ModuleContext.TabModuleId = value;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ModuleId
        {
            get
            {
                return ModuleContext.ModuleId;
            }
            set
            {
                ModuleContext.ModuleId = value;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public UserInfo UserInfo
        {
            get
            {
                return PortalSettings.UserInfo;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int UserId
        {
            get
            {
                return PortalSettings.UserId;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PortalAliasInfo PortalAlias
        {
            get
            {
                return PortalSettings.PortalAlias;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Hashtable Settings
        {
            get
            {
                return ModuleContext.Settings;
            }
        }

        #region IModuleControl Members

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the underlying base control for this ModuleControl
        /// </summary>
        /// <returns>A String</returns>
        /// <history>
        /// 	[cnurse]	12/17/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public Control Control
        {
            get
            {
                return this;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the Path for this control (used primarily for UserControls)
        /// </summary>
        /// <returns>A String</returns>
        /// <history>
        /// 	[cnurse]	12/16/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public string ControlPath
        {
            get
            {
                return TemplateSourceDirectory + "/";
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the Name for this control
        /// </summary>
        /// <returns>A String</returns>
        /// <history>
        /// 	[cnurse]	12/16/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public string ControlName
        {
            get
            {
                return GetType().Name.Replace("_", ".");
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the local resource file for this control
        /// </summary>
        /// <returns>A String</returns>
        /// <history>
        /// 	[cnurse]	12/16/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public string LocalResourceFile
        {
            get
            {
                string fileRoot;
                if (string.IsNullOrEmpty(_localResourceFile))
                {
                    fileRoot = Path.Combine(ControlPath, Localization.LocalResourceDirectory + "/" + ID);
                }
                else
                {
                    fileRoot = _localResourceFile;
                }
                return fileRoot;
            }
            set
            {
                _localResourceFile = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the Module Context for this control
        /// </summary>
        /// <returns>A ModuleInstanceContext</returns>
        /// <history>
        /// 	[cnurse]	12/16/2007  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public ModuleInstanceContext ModuleContext
        {
            get
            {
                if (_moduleContext == null)
                {
                    _moduleContext = new ModuleInstanceContext(this);
                }
                return _moduleContext;
            }
        }

        #endregion

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string EditUrl()
        {
            return ModuleContext.EditUrl();
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string EditUrl(string ControlKey)
        {
            return ModuleContext.EditUrl(ControlKey);
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string EditUrl(string KeyName, string KeyValue)
        {
            return ModuleContext.EditUrl(KeyName, KeyValue);
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string EditUrl(string KeyName, string KeyValue, string ControlKey)
        {
            return ModuleContext.EditUrl(KeyName, KeyValue, ControlKey);
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string EditUrl(string KeyName, string KeyValue, string ControlKey, params string[] AdditionalParameters)
        {
            return ModuleContext.EditUrl(KeyName, KeyValue, ControlKey, AdditionalParameters);
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string EditUrl(int TabID, string ControlKey, bool PageRedirect, params string[] AdditionalParameters)
        {
            return ModuleContext.NavigateUrl(TabID, ControlKey, PageRedirect, AdditionalParameters);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Helper method that can be used to add an ActionEventHandler to the Skin for this 
        /// Module Control
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        ///   [cnurse] 17/9/2004  Added Documentation
        /// </history>
        /// -----------------------------------------------------------------------------
        protected void AddActionHandler(ActionEventHandler e)
        {
            UI.Skins.Skin ParentSkin = UI.Skins.Skin.GetParentSkin(this);
            if (ParentSkin != null)
            {
                ParentSkin.RegisterModuleActionEvent(ModuleId, e);
            }
        }

        protected string LocalizeString(string key)
        {
            return Localization.GetString(key, LocalResourceFile);
        }



        public int GetNextActionID()
        {
            return ModuleContext.GetNextActionID();
        }

        #region "Obsolete methods"

        // CONVERSION: Remove obsoleted methods (FYI some core modules use these, such as Links)
        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   The CacheDirectory property is used to return the location of the "Cache"
        ///   Directory for the Module
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        ///   [cnurse] 04/28/2005  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        [Obsolete("This property is deprecated.  Plaese use ModuleController.CacheDirectory()")]
        public string CacheDirectory
        {
            get
            {
                return PortalController.GetCurrentPortalSettings().HomeDirectoryMapPath + "Cache";
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   The CacheFileName property is used to store the FileName for this Module's
        ///   Cache
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        ///   [cnurse] 04/28/2005  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        [Obsolete("This property is deprecated.  Please use ModuleController.CacheFileName(TabModuleID)")]
        public string CacheFileName
        {
            get
            {
                string strCacheKey = "TabModule:";
                strCacheKey += TabModuleId + ":";
                strCacheKey += Thread.CurrentThread.CurrentUICulture.ToString();
                return PortalController.GetCurrentPortalSettings().HomeDirectoryMapPath + "Cache" + "\\" + Globals.CleanFileName(strCacheKey) + ".resources";
            }
        }

        [Obsolete("This property is deprecated.  Please use ModuleController.CacheKey(TabModuleID)")]
        public string CacheKey
        {
            get
            {
                string strCacheKey = "TabModule:";
                strCacheKey += TabModuleId + ":";
                strCacheKey += Thread.CurrentThread.CurrentUICulture.ToString();
                return strCacheKey;
            }
        }

        // CONVERSION: Obsolete pre 5.0 => Remove in 5.0
        [ObsoleteAttribute(
            "The HelpFile() property was deprecated in version 2.2. Help files are now stored in the /App_LocalResources folder beneath the module with the following resource key naming convention: ModuleHelp.Text"
            )]
        public string HelpFile { get; set; }

        [Obsolete("ModulePath was renamed to ControlPath and moved to IModuleControl in version 5.0")]
        public string ModulePath
        {
            get
            {
                return ControlPath;
            }
        }

        [Obsolete("This property is deprecated.  Please use ModuleController.CacheFileName(TabModuleID)")]
        public string GetCacheFileName(int tabModuleId)
        {
            string strCacheKey = "TabModule:";
            strCacheKey += tabModuleId + ":";
            strCacheKey += Thread.CurrentThread.CurrentUICulture.ToString();
            return PortalController.GetCurrentPortalSettings().HomeDirectoryMapPath + "Cache" + "\\" + Globals.CleanFileName(strCacheKey) + ".resources";
        }

        [Obsolete("This property is deprecated.  Please use ModuleController.CacheKey(TabModuleID)")]
        public string GetCacheKey(int tabModuleId)
        {
            string strCacheKey = "TabModule:";
            strCacheKey += tabModuleId + ":";
            strCacheKey += Thread.CurrentThread.CurrentUICulture.ToString();
            return strCacheKey;
        }

        [Obsolete("Deprecated in DNN 5.0. Please use ModulePermissionController.HasModulePermission(ModuleConfiguration.ModulePermissions, PermissionKey) ")]
        public bool HasModulePermission(string PermissionKey)
        {
            return ModulePermissionController.HasModulePermission(ModuleConfiguration.ModulePermissions, PermissionKey);
        }

        [Obsolete("This method is deprecated.  Plaese use ModuleController.SynchronizeModule(ModuleId)")]
        public void SynchronizeModule()
        {
            ModuleController.SynchronizeModule(ModuleId);
        }

        #endregion
    }
}
