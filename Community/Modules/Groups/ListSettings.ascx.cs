#region Copyright

// 
// DotNetNukeŽ - http://www.dotnetnuke.com
// Copyright (c) 2002-2011
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

using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Security.Roles;
using DotNetNuke.Security.Roles.Internal;
using DotNetNuke.Services.Localization;
using System.Web.UI.WebControls;
using DotNetNuke.Modules.Groups.Components;
using DotNetNuke.Entities.Tabs;
using System.Collections.Generic;
using DotNetNuke.Entities.Modules.Definitions;


#endregion

namespace DotNetNuke.Modules.Groups
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The Settings class manages Module Settings
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class ListSettings : GroupsSettingsBase
    {
        #region Base Method Implementations

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// LoadSettings loads the settings from the Database and displays them
        /// </summary>
        /// -----------------------------------------------------------------------------
        public override void LoadSettings()
        {
            try
            {
                if (Page.IsPostBack == false)
                {
                    BindGroups();
                    BindPages();
                    if (Settings.ContainsKey(Constants.DefaultRoleGroupSetting)) {
                        drpRoleGroup.SelectedIndex = drpRoleGroup.Items.IndexOf(drpRoleGroup.Items.FindByValue(Settings[Constants.DefaultRoleGroupSetting].ToString()));
                    }
                    if (Settings.ContainsKey(Constants.GroupViewPage)) {
                        drpGroupViewPage.SelectedIndex = drpGroupViewPage.Items.IndexOf(drpGroupViewPage.Items.FindByValue(Settings[Constants.GroupViewPage].ToString()));
                    }
                    if (Settings.ContainsKey(Constants.GroupListTemplate)) {
                        txtListTemplate.Text = Settings[Constants.GroupListTemplate].ToString();
                    }
                    if (Settings.ContainsKey(Constants.GroupViewTemplate))
                    {
                        txtViewTemplate.Text = Settings[Constants.GroupViewTemplate].ToString();
                    }
                    if (Settings.ContainsKey(Constants.GroupModerationEnabled)) {
                        chkGroupModeration.Checked = Convert.ToBoolean(Settings[Constants.GroupModerationEnabled].ToString());
                    }
                    if (Settings.ContainsKey(Constants.GroupLoadView)) {
                        drpViewMode.SelectedIndex = drpViewMode.Items.IndexOf(drpViewMode.Items.FindByValue(Settings[Constants.GroupLoadView].ToString()));
                    }
                    
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// UpdateSettings saves the modified settings to the Database
        /// </summary>
        /// -----------------------------------------------------------------------------
        public override void UpdateSettings()
        {
            try
            {
                var modules = new ModuleController();
                modules.UpdateTabModuleSetting(this.TabModuleId, Constants.DefaultRoleGroupSetting, drpRoleGroup.SelectedItem.Value);
                modules.UpdateTabModuleSetting(this.TabModuleId, Constants.GroupViewPage, drpGroupViewPage.SelectedItem.Value);
                modules.UpdateTabModuleSetting(this.TabModuleId, Constants.GroupListTemplate, txtListTemplate.Text);
                modules.UpdateTabModuleSetting(this.TabModuleId, Constants.GroupViewTemplate, txtViewTemplate.Text);
                modules.UpdateTabModuleSetting(this.TabModuleId, Constants.GroupModerationEnabled, chkGroupModeration.Checked.ToString());
                modules.UpdateTabModuleSetting(this.TabModuleId, Constants.GroupLoadView, drpViewMode.SelectedItem.Value);
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        #endregion
        private void BindGroups() {
            var arrGroups = RoleController.GetRoleGroups(PortalId);

            drpRoleGroup.Items.Add(new ListItem(Localization.GetString("GlobalRoles"), "-1"));

            foreach (RoleGroupInfo roleGroup in arrGroups) {
                drpRoleGroup.Items.Add(new ListItem(roleGroup.RoleGroupName, roleGroup.RoleGroupID.ToString()));
            }
        }
        private void BindPages() {
            ModuleController mc = new ModuleController();
            TabController tc = new TabController();
            TabInfo tabInfo;
            foreach (ModuleInfo moduleInfo in mc.GetModules(PortalId)) {

                if (moduleInfo.DesktopModule.ModuleName.Contains("Social Groups") && moduleInfo.IsDeleted == false) {
                    tabInfo = tc.GetTab(moduleInfo.TabID, PortalId, false);
                    if (tabInfo != null) {
                        if (tabInfo.IsDeleted == false) {

                            foreach (KeyValuePair<string, ModuleDefinitionInfo> def in moduleInfo.DesktopModule.ModuleDefinitions) {
                                if (moduleInfo.ModuleDefinition.FriendlyName == def.Key) {
                                    
                                        if (drpGroupViewPage.Items.FindByValue(tabInfo.TabID.ToString()) == null) {
                                            drpGroupViewPage.Items.Add(new ListItem(tabInfo.TabName + " - " + def.Key, tabInfo.TabID.ToString()));
                                        }
                                                             

                                    }

                                }
                            }



                        }
                    }
                }
            }
        
    }
}