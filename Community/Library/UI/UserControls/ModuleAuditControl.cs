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
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;

#endregion

namespace DotNetNuke.UI.UserControls
{
    public abstract class ModuleAuditControl : UserControl
    {
        private const string MyFileName = "ModuleAuditControl.ascx";
        private string _systemUser;
        protected Label lblCreatedBy;
        protected Label lblUpdatedBy;

        public ModuleAuditControl()
        {
            LastModifiedDate = String.Empty;
            LastModifiedByUser = String.Empty;
            CreatedByUser = String.Empty;
            CreatedDate = String.Empty;
        }

        public string CreatedDate { private get; set; }

        public string CreatedByUser { private get; set; }

        public string LastModifiedByUser { private get; set; }

        public string LastModifiedDate { private get; set; }

        public BaseEntityInfo Entity { private get; set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            try
            {
                if (Entity != null)
                {
                    CreatedByUser = Entity.CreatedByUserID.ToString();
                    CreatedDate = Entity.CreatedOnDate.ToString();
                    LastModifiedByUser = Entity.LastModifiedByUserID.ToString();
                    LastModifiedDate = Entity.LastModifiedOnDate.ToString();
                }

                //check to see if updated check is redundant
                bool isCreatorAndUpdater = false;
                if (Regex.IsMatch(CreatedByUser, "^\\d+$") && Regex.IsMatch(LastModifiedByUser, "^\\d+$") && CreatedByUser == LastModifiedByUser)
                {
                    isCreatorAndUpdater = true;
                }

                _systemUser = Localization.GetString("SystemUser", Localization.GetResourceFile(this, MyFileName));
                ShowCreatedString();
                ShowUpdatedString(isCreatorAndUpdater);
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void ShowCreatedString()
        {
            if (Regex.IsMatch(CreatedByUser, @"^-?\d+$"))
            {
                if (int.Parse(CreatedByUser) == Null.NullInteger)
                {
                    CreatedByUser = _systemUser;
                }
                else
                {
                    //contains a UserID
                    UserInfo userInfo = UserController.GetUserById(PortalController.GetCurrentPortalSettings().PortalId, int.Parse(CreatedByUser));
                    if (userInfo != null)
                    {
                        CreatedByUser = userInfo.DisplayName;
                    }
                }
            }
            string createdString = Localization.GetString("CreatedBy", Localization.GetResourceFile(this, MyFileName));
            lblCreatedBy.Text = string.Format(createdString, CreatedByUser, CreatedDate);

        }

        private void ShowUpdatedString(bool isCreatorAndUpdater)
        {
            //check to see if audit contains update information
            if (string.IsNullOrEmpty(LastModifiedDate))
            {
                return;
            }

            if (Regex.IsMatch(LastModifiedByUser, @"^-?\d+$"))
            {
                if (isCreatorAndUpdater)
                {
                    LastModifiedByUser = CreatedByUser;
                }
                else if (int.Parse(LastModifiedByUser) == Null.NullInteger)
                {
                    LastModifiedByUser = _systemUser;
                }
                else
                {
                    //contains a UserID
                    UserInfo userInfo = UserController.GetUserById(PortalController.GetCurrentPortalSettings().PortalId, int.Parse(LastModifiedByUser));
                    if (userInfo != null)
                    {
                        LastModifiedByUser = userInfo.DisplayName;
                    }
                }
            }

            string updatedByString = Localization.GetString("UpdatedBy", Localization.GetResourceFile(this, MyFileName));
            lblUpdatedBy.Text = string.Format(updatedByString, LastModifiedByUser, LastModifiedDate);
        }
    }
}