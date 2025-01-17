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

#region Usings

using System;
using System.Collections;
using System.Globalization;

using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Definitions;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Modules.Groups.Components;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Log.EventLog;

#endregion

namespace DotNetNuke.Modules.Groups
{
    public partial class Setup : GroupsModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            btnGo.Click += btGo_Click;
        }

        public void btGo_Click(object sender, EventArgs e)
        {
            //Setup Child Page - Main View/Activity
            TabInfo tab = CreatePage(PortalSettings.ActiveTab, PortalId, TabId, "Group Activity", false);
            //Add Module to Child Page
            int groupViewModuleId = AddModule(tab, PortalId, "Social Groups", "ContentPaneProfile");
            int journalModuleId = AddModule(tab, PortalId, "Journal", "ContentPaneProfile");
            int consoleId = AddModule(tab, PortalId, "Console", "RightPaneProfile");

            var mc = new ModuleController();

            ModuleInfo groupConsoleModule = mc.GetModule(consoleId, tab.TabID);
            TabInfo memberTab = CreatePage(PortalSettings.ActiveTab, PortalId, tab.TabID, "Members", false);
            mc.CopyModule(groupConsoleModule, memberTab, "RightPaneProfile", true);

            ModuleInfo groupViewModule = mc.GetModule(groupViewModuleId, tab.TabID);
            mc.CopyModule(groupViewModule, memberTab, "ContentPaneProfile", true);
            AddModule(memberTab, PortalId, "DotNetNuke.Modules.MemberDirectory", "ContentPaneProfile");


            //List Settings
            var modules = new ModuleController();
            modules.UpdateTabModuleSetting(TabModuleId, Constants.GroupLoadView, GroupMode.List.ToString());
            modules.UpdateTabModuleSetting(TabModuleId, Constants.GroupViewPage, tab.TabID.ToString(CultureInfo.InvariantCulture));

            Response.Redirect(Request.RawUrl);
        }

        private TabInfo CreatePage(TabInfo tab, int portalId, int parentTabId, string tabName, bool includeInMenu)
        {
            int id = -1;
            var tc = new TabController();
            var tPermissions = new TabPermissionCollection();
            var newTab = new TabInfo();
            if ((tab != null))
            {
                foreach (TabPermissionInfo t in tab.TabPermissions)
                {
                    var tNew = new TabPermissionInfo
                                   {
                                       AllowAccess = t.AllowAccess,
                                       DisplayName = t.DisplayName,
                                       ModuleDefID = t.ModuleDefID,
                                       PermissionCode = t.PermissionCode,
                                       PermissionID = t.PermissionID,
                                       PermissionKey = t.PermissionKey,
                                       PermissionName = t.PermissionName,
                                       RoleID = t.RoleID,
                                       RoleName = t.RoleName,
                                       TabID = -1,
                                       TabPermissionID = -1,
                                       UserID = t.UserID,
                                       Username = t.Username
                                   };
                    newTab.TabPermissions.Add(t);
                }
            }
            newTab.ParentId = parentTabId;
            newTab.PortalID = portalId;
            newTab.TabName = tabName;
            newTab.Title = tabName;
            newTab.IsVisible = includeInMenu;
            newTab.SkinSrc = "[G]Skins/DarkKnight/2-Column-Right-SocialProfile-Mega-Menu.ascx";
            id = tc.AddTab(newTab);
            tab = tc.GetTab(id, portalId, true);

            return tab;
        }

        private int AddModule(TabInfo tab, int portalId, string moduleName, string pane)
        {
            int id = -1;
            var mc = new ModuleController();            
            int desktopModuleId = GetDesktopModuleId(portalId, moduleName);
            int moduleId = -1;
            if (desktopModuleId > -1)
            {
                if (moduleId <= 0)
                {
                    moduleId = AddNewModule(tab, string.Empty, desktopModuleId, pane, 0, string.Empty);
                }
                id = moduleId;
                ModuleInfo mi = mc.GetModule(moduleId, tab.TabID);
                if (moduleName == "Social Groups")
                {
                    mc.UpdateTabModuleSetting(mi.TabModuleID, Constants.GroupLoadView, GroupMode.View.ToString());
                    mc.UpdateTabModuleSetting(mi.TabModuleID, Constants.GroupListPage, tab.TabID.ToString(CultureInfo.InvariantCulture));
                }
                if (moduleName == "Console")
                {
                    mc.UpdateTabModuleSetting(mi.TabModuleID, "AllowSizeChange", "False");
                    mc.UpdateTabModuleSetting(mi.TabModuleID, "AllowViewChange", "False");
                    mc.UpdateTabModuleSetting(mi.TabModuleID, "IncludeParent", "True");
                    mc.UpdateTabModuleSetting(mi.TabModuleID, "Mode", "Group");
                    mc.UpdateTabModuleSetting(mi.TabModuleID, "DefaultSize", "IconNone");
                    mc.UpdateTabModuleSetting(mi.TabModuleID, "ParentTabID", tab.TabID.ToString(CultureInfo.InvariantCulture));
                }
                if (moduleName == "DotNetNuke.Modules.MemberDirectory")
                {
                    mc.UpdateModuleSetting(mi.ModuleID, "FilterBy", "Group");
                    mc.UpdateModuleSetting(mi.ModuleID, "FilterPropertyValue", "");
                    mc.UpdateModuleSetting(mi.ModuleID, "FilterValue", "-1");
                    mc.UpdateTabModuleSetting(mi.TabModuleID, "DisplaySearch", "False");
                }
            }

            return id;
        }

        private int GetDesktopModuleId(int portalId, string moduleName)
        {
            DesktopModuleInfo info = DesktopModuleController.GetDesktopModuleByModuleName(moduleName, portalId);
            return info == null ? -1 : info.DesktopModuleID;
        }

        private int AddNewModule(TabInfo tab, string title, int desktopModuleId, string paneName, int permissionType, string align)
        {
            TabPermissionCollection objTabPermissions = tab.TabPermissions;
            var objPermissionController = new PermissionController();
            var objModules = new ModuleController();
            int j;
            var mdc = new ModuleDefinitionController();

            foreach (ModuleDefinitionInfo objModuleDefinition in mdc.GetModuleDefinitions(desktopModuleId))
            {
                var objModule = new ModuleInfo();
                objModule.Initialize(tab.PortalID);

                objModule.PortalID = tab.PortalID;
                objModule.TabID = tab.TabID;
                if (string.IsNullOrEmpty(title))
                {
                    objModule.ModuleTitle = objModuleDefinition.FriendlyName;
                }
                else
                {
                    objModule.ModuleTitle = title;
                }
                objModule.PaneName = paneName;
                objModule.ModuleDefID = objModuleDefinition.ModuleDefID;
                objModule.CacheTime = 0;
                objModule.InheritViewPermissions = true;
                objModule.DisplayTitle = false;

                // get the default module view permissions
                ArrayList arrSystemModuleViewPermissions = objPermissionController.GetPermissionByCodeAndKey("SYSTEM_MODULE_DEFINITION", "VIEW");

                // get the permissions from the page
                foreach (TabPermissionInfo objTabPermission in objTabPermissions)
                {
                    if (objTabPermission.PermissionKey == "VIEW" && permissionType == 0)
                    {
                        //Don't need to explicitly add View permisisons if "Same As Page"
                        continue;
                    }

                    // get the system module permissions for the permissionkey
                    ArrayList arrSystemModulePermissions = objPermissionController.GetPermissionByCodeAndKey("SYSTEM_MODULE_DEFINITION", objTabPermission.PermissionKey);
                    // loop through the system module permissions
                    for (j = 0; j <= arrSystemModulePermissions.Count - 1; j++)
                    {
                        // create the module permission
                        PermissionInfo objSystemModulePermission = default(PermissionInfo);
                        objSystemModulePermission = (PermissionInfo) arrSystemModulePermissions[j];
                        if (objSystemModulePermission.PermissionKey == "VIEW" && permissionType == 1 && objTabPermission.PermissionKey != "EDIT")
                        {
                            //Only Page Editors get View permissions if "Page Editors Only"
                            continue;
                        }

                        ModulePermissionInfo objModulePermission = AddModulePermission(objModule,
                                                                                       objSystemModulePermission,
                                                                                       objTabPermission.RoleID,
                                                                                       objTabPermission.UserID,
                                                                                       objTabPermission.AllowAccess);

                        // ensure that every EDIT permission which allows access also provides VIEW permission
                        if (objModulePermission.PermissionKey == "EDIT" & objModulePermission.AllowAccess)
                        {
                            ModulePermissionInfo objModuleViewperm = AddModulePermission(objModule,
                                                                                         (PermissionInfo) arrSystemModuleViewPermissions[0],
                                                                                         objModulePermission.RoleID,
                                                                                         objModulePermission.UserID,
                                                                                         true);
                        }
                    }
                }

                objModule.AllTabs = false;
                objModule.Alignment = align;

                return objModules.AddModule(objModule);
            }
            return -1;
        }

        private ModulePermissionInfo AddModulePermission(ModuleInfo objModule, PermissionInfo permission, int roleId, int userId, bool allowAccess)
        {
            var objModulePermission = new ModulePermissionInfo();
            objModulePermission.ModuleID = objModule.ModuleID;
            objModulePermission.PermissionID = permission.PermissionID;
            objModulePermission.RoleID = roleId;
            objModulePermission.UserID = userId;
            objModulePermission.PermissionKey = permission.PermissionKey;
            objModulePermission.AllowAccess = allowAccess;

            // add the permission to the collection
            if (objModule.ModulePermissions == null)
            {
                objModule.ModulePermissions = new ModulePermissionCollection();
            }
            if (!objModule.ModulePermissions.Contains(objModulePermission))
            {
                objModule.ModulePermissions.Add(objModulePermission);
            }

            return objModulePermission;
        }
    }
}