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

using System.Collections.Generic;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Log.EventLog;

#endregion

namespace DotNetNuke.Entities.Modules
{
    /// -----------------------------------------------------------------------------
    /// Project	 : DotNetNuke
    /// Namespace: DotNetNuke.Entities.Modules
    /// Class	 : ModuleControlController
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// ModuleControlController provides the Business Layer for Module Controls
    /// </summary>
    /// <history>
    /// 	[cnurse]	01/14/2008   Documented
    /// </history>
    /// -----------------------------------------------------------------------------
    public class SkinControlController
    {
        private static readonly DataProvider dataProvider = DataProvider.Instance();

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// DeleteSkinControl deletes a Skin Control in the database
        /// </summary>
        /// <param name="skinControl">The Skin Control to delete</param>
        /// <history>
        /// 	[cnurse]	03/28/2008   Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static void DeleteSkinControl(SkinControlInfo skinControl)
        {
            dataProvider.DeleteSkinControl(skinControl.SkinControlID);
            var objEventLog = new EventLogController();
            objEventLog.AddLog(skinControl, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", EventLogController.EventLogType.SKINCONTROL_DELETED);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetSkinControl gets a single Skin Control from the database
        /// </summary>
        /// <param name="skinControlID">The ID of the SkinControl</param>
        /// <history>
        /// 	[cnurse]	03/28/2008   Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static SkinControlInfo GetSkinControl(int skinControlID)
        {
            return CBO.FillObject<SkinControlInfo>(dataProvider.GetSkinControl(skinControlID));
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetSkinControlByPackageID gets a single Skin Control from the database
        /// </summary>
        /// <param name="packageID">The ID of the Package</param>
        /// <history>
        /// 	[cnurse]	03/28/2008   Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static SkinControlInfo GetSkinControlByPackageID(int packageID)
        {
            return CBO.FillObject<SkinControlInfo>(dataProvider.GetSkinControlByPackageID(packageID));
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetSkinControlByKey gets a single Skin Control from the database
        /// </summary>
        /// <param name="key">The key of the Control</param>
        /// <history>
        /// 	[cnurse]	03/28/2008   Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static SkinControlInfo GetSkinControlByKey(string key)
        {
            return CBO.FillObject<SkinControlInfo>(dataProvider.GetSkinControlByKey(key));
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetSkinControls gets all the Skin Controls from the database
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/28/2008   Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static Dictionary<string, SkinControlInfo> GetSkinControls()
        {
            return CBO.FillDictionary("ControlKey", dataProvider.GetSkinControls(), new Dictionary<string, SkinControlInfo>());
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// SaveSkinControl updates a Skin Control in the database
        /// </summary>
        /// <param name="skinControl">The Skin Control to save</param>
        /// <history>
        /// 	[cnurse]	03/28/2008   Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static int SaveSkinControl(SkinControlInfo skinControl)
        {
            int skinControlID = skinControl.SkinControlID;
            var eventLogController = new EventLogController();
            if (skinControlID == Null.NullInteger)
            {
				//Add new Skin Control
                skinControlID = dataProvider.AddSkinControl(skinControl.PackageID,
                                                            skinControl.ControlKey,
                                                            skinControl.ControlSrc,
                                                            skinControl.SupportsPartialRendering,
                                                            UserController.GetCurrentUserInfo().UserID);
                eventLogController.AddLog(skinControl, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", EventLogController.EventLogType.SKINCONTROL_CREATED);
            }
            else
            {
				//Upgrade Skin Control
                dataProvider.UpdateSkinControl(skinControl.SkinControlID,
                                               skinControl.PackageID,
                                               skinControl.ControlKey,
                                               skinControl.ControlSrc,
                                               skinControl.SupportsPartialRendering,
                                               UserController.GetCurrentUserInfo().UserID);
                eventLogController.AddLog(skinControl, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", EventLogController.EventLogType.SKINCONTROL_UPDATED);
            }
            return skinControlID;
        }
    }
}
