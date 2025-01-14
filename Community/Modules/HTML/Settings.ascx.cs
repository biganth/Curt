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
using System.Collections.Generic;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;

#endregion

namespace DotNetNuke.Modules.Html
{

    /// <summary>
    ///   The Settings ModuleSettingsBase is used to manage the 
    ///   settings for the HTML Module
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    ///   [leupold]	    08/12/2007	created
    /// </history>
    public partial class Settings : ModuleSettingsBase
    {

        #region Event Handlers

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            cboWorkflow.SelectedIndexChanged += OnWorkflowSelectedIndexChanged;
        }

        protected void OnWorkflowSelectedIndexChanged(object sender, EventArgs e)
        {
            DisplayWorkflowDetails();
        }

        #endregion

        #region Private Methods

        private void DisplayWorkflowDetails()
        {
            if ((cboWorkflow.SelectedValue != null))
            {
                var objWorkflow = new WorkflowStateController();
                var strDescription = "";
                var arrStates = objWorkflow.GetWorkflowStates(int.Parse(cboWorkflow.SelectedValue));
                if (arrStates.Count > 0)
                {
                    foreach (WorkflowStateInfo objState in arrStates)
                    {
                        strDescription = strDescription + " >> " + "<strong>" + objState.StateName + "</strong>";
                    }
                    strDescription = strDescription + "<br />" + ((WorkflowStateInfo) arrStates[0]).Description;
                }
                lblDescription.Text = strDescription;
            }
        }

        #endregion

        #region Base Method Implementations

        /// <summary>
        ///   LoadSettings loads the settings from the Database and displays them
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// </history>
        public override void LoadSettings()
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    var htmlTextController = new HtmlTextController();
                    var workflowStateController = new WorkflowStateController();

                    // get replace token settings
                    if (ModuleSettings["HtmlText_ReplaceTokens"] != null)
                    {
                        chkReplaceTokens.Checked = Convert.ToBoolean(ModuleSettings["HtmlText_ReplaceTokens"]);
                    }

                    // get workflow/version settings
                    var arrWorkflows = new ArrayList();
                    foreach (WorkflowStateInfo objState in workflowStateController.GetWorkflows(PortalId))
                    {
                        if (!objState.IsDeleted)
                        {
                            arrWorkflows.Add(objState);
                        }
                    }
                    cboWorkflow.DataSource = arrWorkflows;
                    cboWorkflow.DataBind();
                    var workflow = htmlTextController.GetWorkflow(ModuleId, TabId, PortalId);
                    if ((cboWorkflow.Items.FindByValue(workflow.Value.ToString()) != null))
                    {
                        cboWorkflow.Items.FindByValue(workflow.Value.ToString()).Selected = true;
                    }
                    DisplayWorkflowDetails();


                    if (rblApplyTo.Items.FindByValue(workflow.Key) != null)
                    {
                        rblApplyTo.Items.FindByValue(workflow.Key).Selected = true;
                    }
                }
                //Module failed to load
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        ///   UpdateSettings saves the modified settings to the Database
        /// </summary>
        public override void UpdateSettings()
        {
            try
            {
                var htmlTextController = new HtmlTextController();
                var objWorkflow = new WorkflowStateController();

                // update replace token setting
                var objModules = new ModuleController();
                objModules.UpdateModuleSetting(ModuleId, "HtmlText_ReplaceTokens", chkReplaceTokens.Checked.ToString());

                // disable module caching if token replace is enabled
                if (chkReplaceTokens.Checked)
                {
                    ModuleInfo objModule = objModules.GetModule(ModuleId, TabId, false);
                    if (objModule.CacheTime > 0)
                    {
                        objModule.CacheTime = 0;
                        objModules.UpdateModule(objModule);
                    }
                }

                // update workflow/version settings
                switch (rblApplyTo.SelectedValue)
                {
                    case "Module":
                        htmlTextController.UpdateWorkflow(ModuleId, rblApplyTo.SelectedValue, Int32.Parse(cboWorkflow.SelectedValue), chkReplace.Checked);
                        break;
                    case "Page":
                        htmlTextController.UpdateWorkflow(TabId, rblApplyTo.SelectedValue, Int32.Parse(cboWorkflow.SelectedValue), chkReplace.Checked);
                        break;
                    case "Site":
                        htmlTextController.UpdateWorkflow(PortalId, rblApplyTo.SelectedValue, Int32.Parse(cboWorkflow.SelectedValue), chkReplace.Checked);
                        break;
                }

                //Module failed to load
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        #endregion

    }
}