using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Definitions;
using DotNetNuke.Entities.Tabs;
using TechTalk.SpecFlow;

namespace DotNetNuke.Tests.Website.Steps
{
    [Binding]
    public class ExtensionSteps : AutomationBase
    {
        [Given(@"There is a (.*) module on the page with these permissions")]
        public void GivenAModuleOnThePage(string moduleName, Table permissions)
        {
            var controller = new ModuleController();
            var modules = controller.GetTabModules(Page.TabID);
            var module = modules.Where(m => m.Value.ModuleDefinition.FriendlyName == moduleName);
            if (!module.Any())
            {
                AddModuleToPage(Page, GetModuleDefinition(moduleName, moduleName), moduleName, "", true);
            }
        }

        ///-----------------------------------------------------------------------------
        ///<summary>
        ///  AddModuleToPage adds a module to a Page
        ///</summary>
        ///<remarks>
        ///</remarks>
        ///<param name = "page">The Page to add the Module to</param>
        ///<param name = "moduleDefId">The Module Deinition Id for the module to be aded to this tab</param>
        ///<param name = "moduleTitle">The Module's title</param>
        ///<param name = "moduleIconFile">The Module's icon</param>
        ///<param name = "inheritPermissions">Inherit the Pages View Permisions</param>
        ///<history>
        ///  [cnurse]	11/16/2004	created
        ///</history>
        ///-----------------------------------------------------------------------------
        public static int AddModuleToPage(TabInfo page, int moduleDefId, string moduleTitle, string moduleIconFile, bool inheritPermissions)
        {
            var moduleController = new ModuleController();
            ModuleInfo moduleInfo;
            int moduleId = Null.NullInteger;

            if ((page != null))
            {
                bool isDuplicate = false;
                foreach (var kvp in moduleController.GetTabModules(page.TabID))
                {
                    moduleInfo = kvp.Value;
                    if (moduleInfo.ModuleDefID == moduleDefId)
                    {
                        isDuplicate = true;
                        moduleId = moduleInfo.ModuleID;
                    }
                }

                if (!isDuplicate)
                {
                    moduleInfo = new ModuleInfo
                    {
                        ModuleID = Null.NullInteger,
                        PortalID = page.PortalID,
                        TabID = page.TabID,
                        ModuleOrder = -1,
                        ModuleTitle = moduleTitle,
                        PaneName = Globals.glbDefaultPane,
                        ModuleDefID = moduleDefId,
                        CacheTime = 0,
                        IconFile = moduleIconFile,
                        AllTabs = false,
                        Visibility = VisibilityState.None,
                        InheritViewPermissions = inheritPermissions
                    };
                    moduleId = moduleController.AddModule(moduleInfo);
                }
            }

            return moduleId;
        }

        /// <summary>
        /// GetModuleDefinition gets the Module Definition Id of a module
        /// </summary>
        ///	<param name="desktopModuleName">The Friendly Name of the Module to Add</param>
        ///	<param name="moduleDefinitionName">The Module Definition Name</param>
        ///	<returns>The Module Definition Id of the Module (-1 if no module definition)</returns>
        /// <history>
        /// 	[cnurse]	11/16/2004	created
        /// </history>
        /// -----------------------------------------------------------------------------
        private static int GetModuleDefinition(string desktopModuleName, string moduleDefinitionName)
        {
            // get desktop module
            var desktopModule = DesktopModuleController.GetDesktopModuleByModuleName(desktopModuleName, Null.NullInteger);
            if (desktopModule == null)
            {
                return -1;
            }

            // get module definition
            ModuleDefinitionInfo objModuleDefinition = ModuleDefinitionController.GetModuleDefinitionByFriendlyName(moduleDefinitionName, desktopModule.DesktopModuleID);
            if (objModuleDefinition == null)
            {
                return -1;
            }


            return objModuleDefinition.ModuleDefID;
        }

    }
}
