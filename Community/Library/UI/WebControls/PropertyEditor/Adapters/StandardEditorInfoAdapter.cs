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

using System.ComponentModel;
using System.Reflection;
using System.Web.UI.WebControls;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Profile;
using DotNetNuke.Entities.Users;

#endregion

namespace DotNetNuke.UI.WebControls
{
    /// -----------------------------------------------------------------------------
    /// Project:    DotNetNuke
    /// Namespace:  DotNetNuke.UI.WebControls
    /// Class:      StandardEditorInfoAdapter
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The StandardEditorInfoAdapter control provides an Adapter for standard datasources
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    ///     [cnurse]	05/05/2006	created
    /// </history>
    /// -----------------------------------------------------------------------------
    public class StandardEditorInfoAdapter : IEditorInfoAdapter
    {
        private readonly object DataSource;
        private readonly string FieldName;

        public StandardEditorInfoAdapter(object dataSource, string fieldName)
        {
            DataSource = dataSource;
            FieldName = fieldName;
        }

        #region IEditorInfoAdapter Members

        public EditorInfo CreateEditControl()
        {
            EditorInfo editInfo = null;
            PropertyInfo objProperty = GetProperty(DataSource, FieldName);
            if (objProperty != null)
            {
                editInfo = GetEditorInfo(DataSource, objProperty);
            }
            return editInfo;
        }

        public bool UpdateValue(PropertyEditorEventArgs e)
        {
            bool changed = e.Changed;
            object oldValue = e.OldValue;
            object newValue = e.Value;
            bool _IsDirty = Null.NullBoolean;
            if (DataSource != null)
            {
                PropertyInfo objProperty = DataSource.GetType().GetProperty(e.Name);
                if (objProperty != null)
                {
                    if ((!(ReferenceEquals(newValue, oldValue))) || changed)
                    {
                        objProperty.SetValue(DataSource, newValue, null);
                        _IsDirty = true;
                    }
                }
            }
            return _IsDirty;
        }

        public bool UpdateVisibility(PropertyEditorEventArgs e)
        {
            return false;
        }

        #endregion

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetEditorInfo builds an EditorInfo object for a propoerty
        /// </summary>
        /// <history>
        /// 	[cnurse]	05/05/2006	Created
        /// </history>
        /// -----------------------------------------------------------------------------
        private EditorInfo GetEditorInfo(object dataSource, PropertyInfo objProperty)
        {
            var editInfo = new EditorInfo();

            //Get the Name of the property
            editInfo.Name = objProperty.Name;

            //Get the value of the property
            editInfo.Value = objProperty.GetValue(dataSource, null);

            //Get the type of the property
            editInfo.Type = objProperty.PropertyType.AssemblyQualifiedName;

            //Get the Custom Attributes for the property
            editInfo.Attributes = objProperty.GetCustomAttributes(true);

            //Get Category Field
            editInfo.Category = string.Empty;
            object[] categoryAttributes = objProperty.GetCustomAttributes(typeof (CategoryAttribute), true);
            if (categoryAttributes.Length > 0)
            {
                var category = (CategoryAttribute) categoryAttributes[0];
                editInfo.Category = category.Category;
            }
			
            //Get EditMode Field

            if (!objProperty.CanWrite)
            {
                editInfo.EditMode = PropertyEditorMode.View;
            }
            else
            {
                object[] readOnlyAttributes = objProperty.GetCustomAttributes(typeof (IsReadOnlyAttribute), true);
                if (readOnlyAttributes.Length > 0)
                {
                    var readOnlyMode = (IsReadOnlyAttribute) readOnlyAttributes[0];
                    if (readOnlyMode.IsReadOnly)
                    {
                        editInfo.EditMode = PropertyEditorMode.View;
                    }
                }
            }
			
            //Get Editor Field
            editInfo.Editor = "UseSystemType";
            object[] editorAttributes = objProperty.GetCustomAttributes(typeof (EditorAttribute), true);
            if (editorAttributes.Length > 0)
            {
                EditorAttribute editor = null;
                for (int i = 0; i <= editorAttributes.Length - 1; i++)
                {
                    if (((EditorAttribute) editorAttributes[i]).EditorBaseTypeName.IndexOf("DotNetNuke.UI.WebControls.EditControl") >= 0)
                    {
                        editor = (EditorAttribute) editorAttributes[i];
                        break;
                    }
                }
                if (editor != null)
                {
                    editInfo.Editor = editor.EditorTypeName;
                }
            }
			
            //Get Required Field
            editInfo.Required = false;
            object[] requiredAttributes = objProperty.GetCustomAttributes(typeof (RequiredAttribute), true);
            if (requiredAttributes.Length > 0)
            {
                //The property may contain multiple edit mode types, so make sure we only use DotNetNuke editors.
                var required = (RequiredAttribute) requiredAttributes[0];
                if (required.Required)
                {
                    editInfo.Required = true;
                }
            }
			
            //Get Css Style
            editInfo.ControlStyle = new Style();
            object[] StyleAttributes = objProperty.GetCustomAttributes(typeof (ControlStyleAttribute), true);
            if (StyleAttributes.Length > 0)
            {
                var attribute = (ControlStyleAttribute) StyleAttributes[0];
                editInfo.ControlStyle.CssClass = attribute.CssClass;
                editInfo.ControlStyle.Height = attribute.Height;
                editInfo.ControlStyle.Width = attribute.Width;
            }
			
            //Get LabelMode Field
            editInfo.LabelMode = LabelMode.Left;
            object[] labelModeAttributes = objProperty.GetCustomAttributes(typeof (LabelModeAttribute), true);
            if (labelModeAttributes.Length > 0)
            {
                var mode = (LabelModeAttribute) labelModeAttributes[0];
                editInfo.LabelMode = mode.Mode;
            }
			
            //Set ResourceKey Field
            editInfo.ResourceKey = string.Format("{0}_{1}", dataSource.GetType().Name, objProperty.Name);

            //Get Validation Expression Field
            editInfo.ValidationExpression = string.Empty;
            object[] regExAttributes = objProperty.GetCustomAttributes(typeof (RegularExpressionValidatorAttribute), true);
            if (regExAttributes.Length > 0)
            {
                var regExAttribute = (RegularExpressionValidatorAttribute) regExAttributes[0];
                editInfo.ValidationExpression = regExAttribute.Expression;
            }
			
            //Set Visibility
            editInfo.ProfileVisibility = new ProfileVisibility
                                             {
                                                 VisibilityMode = UserVisibilityMode.AllUsers
                                             };

            return editInfo;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetProperty returns the property that is being "bound" to
        /// </summary>
        /// <history>
        /// 	[cnurse]	05/05/2006	Created
        /// </history>
        /// -----------------------------------------------------------------------------
        private PropertyInfo GetProperty(object dataSource, string fieldName)
        {
            if (dataSource != null)
            {
                BindingFlags Bindings = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
                PropertyInfo objProperty = dataSource.GetType().GetProperty(fieldName, Bindings);
                return objProperty;
            }
            else
            {
                return null;
            }
        }
    }
}
