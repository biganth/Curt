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
using System.Collections.Specialized;
using System.Web.UI;

using DotNetNuke.UI.WebControls;


#endregion

namespace DotNetNuke.Web.UI.WebControls
{
    public class DnnFileEditControl : IntegerEditControl
    {
        #region Private Fields

        private DnnFilePicker _fileControl;

        #endregion

        #region Public Properties

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   Gets or sets the current file extension filter.
        /// </summary>
        /// <history>
        ///   [anurse]	08/11/2006 documented
        /// </history>
        /// -----------------------------------------------------------------------------
        public string FileFilter { get; set; }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   Gets or sets the current file path.
        /// </summary>
        /// <history>
        ///   [cnurse]	07/02/2007 created
        /// </history>
        /// -----------------------------------------------------------------------------
        public string FilePath { get; set; }

        #endregion

        #region Protected Methods

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   Creates the control contained within this control
        /// </summary>
        /// <history>
        ///   [cnurse]	07/31/2006 created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected override void CreateChildControls()
        {
            //First clear the controls collection
            Controls.Clear();

            //Create Table
            _fileControl = new DnnFilePicker
                               {
                                   ID = string.Format("{0}FileControl", ID),
                                   FileFilter = FileFilter,
                                   FilePath = FilePath,
                                   Permissions = "ADD",
                                   UsePersonalFolder = true,
                                   ShowFolders = false,
                                   User = User
                               };

            //Add table to Control
            Controls.Add(_fileControl);

            base.CreateChildControls();
        }

        protected override void OnInit(EventArgs e)
        {
            EnsureChildControls();
            base.OnInit(e);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   Runs before the control is rendered.
        /// </summary>
        /// <history>
        ///   [cnurse]	07/31/2006 created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            _fileControl.FileID = IntegerValue;

            if (Page != null)
            {
                Page.RegisterRequiresPostBack(this);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   Renders the control in edit mode
        /// </summary>
        /// <param name = "writer">An HtmlTextWriter to render the control to</param>
        /// <history>
        ///   [cnurse]	04/20/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected override void RenderEditMode(HtmlTextWriter writer)
        {
            RenderChildren(writer);
        }

        #endregion

        #region Public Methods

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   Loads the Post Back Data and determines whether the value has change
        /// </summary>
        /// <remarks>
        ///   In this case because the <see cref = "_fileControl" /> is a contained control, we do not need 
        ///   to process the PostBackData (it has been handled by the File Control).  We just use
        ///   this method as the Framework calls it for us.
        /// </remarks>
        /// <param name = "postDataKey">A key to the PostBack Data to load</param>
        /// <param name = "postCollection">A name value collection of postback data</param>
        /// <history>
        ///   [cnurse]	08/01/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public override bool LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            bool dataChanged = false;
            string presentValue = StringValue;
            string postedValue = postCollection[string.Format("{0}FileControl$File", postDataKey)];
            if (!presentValue.Equals(postedValue))
            {
                Value = postedValue;
                dataChanged = true;
            }
            return dataChanged;
        }

        #endregion
    }
}