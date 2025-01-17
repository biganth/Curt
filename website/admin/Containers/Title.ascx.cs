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
using System.Web;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins;
using DotNetNuke.UI.WebControls;

#endregion

namespace DotNetNuke.UI.Containers
{
    /// -----------------------------------------------------------------------------
    /// <summary></summary>
    /// <remarks></remarks>
    /// <history>
    /// 	[cniknet]	10/15/2004	Replaced public members with properties and removed
    ///                             brackets from property names
    /// </history>
    /// -----------------------------------------------------------------------------
    public partial class Title : SkinObjectBase
    {
        private const string MyFileName = "Title.ascx";
        #region "Public Members"
        public string CssClass { get; set; }

        #endregion

        private bool CanEditModule()
        {
            var canEdit = false;
            if (ModuleControl != null && ModuleControl.ModuleContext.ModuleId > Null.NullInteger)
            {
                canEdit = (PortalSettings.UserMode == PortalSettings.Mode.Edit) && TabPermissionController.CanAdminPage() && !Globals.IsAdminControl();
            }
            return canEdit;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            titleLabel.UpdateLabel += UpdateTitle;
        }


        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            //public attributes
            if (!String.IsNullOrEmpty(CssClass))
            {
                titleLabel.CssClass = CssClass;
            }
            string moduleTitle = Null.NullString;
            if (ModuleControl != null)
            {
                moduleTitle = Localization.LocalizeControlTitle(ModuleControl);
            }
            if (moduleTitle == Null.NullString)
            {
                moduleTitle = " ";
            }
            titleLabel.Text = EncodeTitle(moduleTitle);
            titleLabel.EditEnabled = false;
            titleToolbar.Visible = false;

            if (CanEditModule() && PortalSettings.InlineEditorEnabled)
            {
                titleLabel.EditEnabled = true;
                titleToolbar.Visible = true;
            }

        }

        private string EncodeTitle(string moduleTitle)
        {
            if (titleLabel.EncodeText)
            {
                return HttpUtility.HtmlEncode(moduleTitle);
            }
            return moduleTitle;
        }
        private string DecodeTitle(string moduleTitle)
        {
            if (titleLabel.EncodeText)
            {
                return HttpUtility.HtmlDecode(moduleTitle);
            }
            return moduleTitle;
        }


        private void UpdateTitle(object source, DNNLabelEditEventArgs e)
        {
            if (CanEditModule())
            {
                var moduleController = new ModuleController();
                ModuleInfo moduleInfo = moduleController.GetModule(ModuleControl.ModuleContext.ModuleId, ModuleControl.ModuleContext.TabId, false);

                moduleInfo.ModuleTitle = DecodeTitle(e.Text);
                moduleController.UpdateModule(moduleInfo);
            }
        }
    }
}