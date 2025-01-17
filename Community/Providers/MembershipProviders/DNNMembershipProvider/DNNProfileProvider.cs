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
using System.Data;
using System.Globalization;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Profile;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework;
using DotNetNuke.Security.Membership.Data;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Log.EventLog;

#endregion

// ReSharper disable CheckNamespace
namespace DotNetNuke.Security.Profile
// ReSharper restore CheckNamespace
{
    /// -----------------------------------------------------------------------------
    /// Project:    DotNetNuke
    /// Namespace:  DotNetNuke.Security.Profile
    /// Class:      DNNProfileProvider
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The DNNProfileProvider overrides the default ProfileProvider to provide
    /// a purely DotNetNuke implementation
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// -----------------------------------------------------------------------------
    public class DNNProfileProvider : ProfileProvider
    {
        #region Private Members

        private readonly DataProvider _dataProvider;

        #endregion

        #region Constructors

        public DNNProfileProvider()
        {
            _dataProvider = DataProvider.Instance();
            if (_dataProvider == null)
            {
				//get the provider configuration based on the type
                string defaultprovider = Data.DataProvider.Instance().DefaultProviderName;
                const string dataProviderNamespace = "DotNetNuke.Security.Membership.Data";
                if (defaultprovider == "SqlDataProvider")
                {
                    _dataProvider = new SqlDataProvider();
                }
                else
                {
                    string providerType = dataProviderNamespace + "." + defaultprovider;
                    _dataProvider = (DataProvider) Reflection.CreateObject(providerType, providerType, true);
                }
                ComponentFactory.RegisterComponentInstance<DataProvider>(_dataProvider);
            }
        }

        #endregion

        #region Private Methods

        private void UpdateTimeZoneInfo(UserInfo user, ProfilePropertyDefinitionCollection properties)
        {
            ProfilePropertyDefinition newTimeZone = properties["PreferredTimeZone"];
            ProfilePropertyDefinition oldTimeZone = properties["TimeZone"];
            if (newTimeZone != null && oldTimeZone != null)
            {
                //Old timezone is present but new is not...we will set that up.
                if (!string.IsNullOrEmpty(oldTimeZone.PropertyValue) && string.IsNullOrEmpty(newTimeZone.PropertyValue))
                {
                    int oldOffset;
                    int.TryParse(oldTimeZone.PropertyValue, out oldOffset);
                    TimeZoneInfo timeZoneInfo = Localization.ConvertLegacyTimeZoneOffsetToTimeZoneInfo(oldOffset);
                    newTimeZone.PropertyValue = timeZoneInfo.Id;
                    UpdateUserProfile(user);
                }
                //It's also possible that the new value is set but not the old value. We need to make them backwards compatible
                else if (!string.IsNullOrEmpty(newTimeZone.PropertyValue) && string.IsNullOrEmpty(oldTimeZone.PropertyValue))
                {
                    TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(newTimeZone.PropertyValue);
                    if (timeZoneInfo != null)
                    {
                        oldTimeZone.PropertyValue = timeZoneInfo.BaseUtcOffset.TotalMinutes.ToString(CultureInfo.InvariantCulture);
                    }
                }
            }
        }

        #endregion

        #region Public Methods

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets whether the Provider Properties can be edited
        /// </summary>
        /// <returns>A Boolean</returns>
        /// -----------------------------------------------------------------------------
        public override bool CanEditProviderProperties
        {
            get
            {
                return true;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetUserProfile retrieves the UserProfile information from the Data Store
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="user">The user whose Profile information we are retrieving.</param>
        /// -----------------------------------------------------------------------------
        public override void GetUserProfile(ref UserInfo user)
        {
            ProfilePropertyDefinition profProperty;

            int portalId = user.IsSuperUser ? Globals.glbSuperUserAppName : user.PortalID;
            var properties = ProfileController.GetPropertyDefinitionsByPortal(portalId, true, false);

            //Load the Profile properties
            if (user.UserID > Null.NullInteger)
            {
                IDataReader dr = _dataProvider.GetUserProfile(user.UserID);
                try
                {
                    while (dr.Read())
                    {
						//Ensure the data reader returned is valid
                        if (!string.Equals(dr.GetName(0), "ProfileID", StringComparison.InvariantCultureIgnoreCase))
                        {
                            break;
                        }
                        int definitionId = Convert.ToInt32(dr["PropertyDefinitionId"]);
                        profProperty = properties.GetById(definitionId);
                        if (profProperty != null)
                        {
                            profProperty.PropertyValue = Convert.ToString(dr["PropertyValue"]);
                            var extendedVisibility = string.Empty;
                            if (dr.GetSchemaTable().Select("ColumnName = 'ExtendedVisibility'").Length > 0)
                            {
                                extendedVisibility = Convert.ToString(dr["ExtendedVisibility"]);
                            }
                            profProperty.ProfileVisibility = new ProfileVisibility(portalId, extendedVisibility)
                                                                 {
                                                                     VisibilityMode = (UserVisibilityMode)dr["Visibility"]
                                                                 };
                        }
                    }
                }
                finally
                {
                    CBO.CloseDataReader(dr, true);
                }
            }
                      
            //Clear the profile
            user.Profile.ProfileProperties.Clear();
            
			//Add the properties to the profile
			foreach (ProfilePropertyDefinition property in properties)
            {
                profProperty = property;
                if (string.IsNullOrEmpty(profProperty.PropertyValue) && !string.IsNullOrEmpty(profProperty.DefaultValue))
                {
                    profProperty.PropertyValue = profProperty.DefaultValue;
                }
                user.Profile.ProfileProperties.Add(profProperty);
            }

            //Clear IsDirty Flag
            user.Profile.ClearIsDirty();

            //Ensure old and new TimeZone properties are in synch
            UpdateTimeZoneInfo(user, properties);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// UpdateUserProfile persists a user's Profile to the Data Store
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="user">The user to persist to the Data Store.</param>
        /// -----------------------------------------------------------------------------
        public override void UpdateUserProfile(UserInfo user)
        {
            ProfilePropertyDefinitionCollection properties = user.Profile.ProfileProperties;

            //Ensure old and new TimeZone properties are in synch
            var newTimeZone = properties["PreferredTimeZone"];
            var oldTimeZone = properties["TimeZone"];
            if (oldTimeZone != null && newTimeZone != null)
            {   //preference given to new property, if new is changed then old should be updated as well.
                if (newTimeZone.IsDirty && !string.IsNullOrEmpty(newTimeZone.PropertyValue))
                {
                    var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(newTimeZone.PropertyValue);
                    if (timeZoneInfo != null)
                        oldTimeZone.PropertyValue = timeZoneInfo.BaseUtcOffset.TotalMinutes.ToString(CultureInfo.InvariantCulture);
                }
                //however if old is changed, we need to update new as well
                else if (oldTimeZone.IsDirty)
                {
                    int oldOffset;
                    int.TryParse(oldTimeZone.PropertyValue, out oldOffset);
                    newTimeZone.PropertyValue = Localization.ConvertLegacyTimeZoneOffsetToTimeZoneInfo(oldOffset).Id;                    
                }
            }
            
            foreach (ProfilePropertyDefinition profProperty in properties)
            {
                if ((profProperty.PropertyValue != null) && (profProperty.IsDirty))
                {
                    var objSecurity = new PortalSecurity();
                    string propertyValue = objSecurity.InputFilter(profProperty.PropertyValue, PortalSecurity.FilterFlag.NoScripting);
                    _dataProvider.UpdateProfileProperty(Null.NullInteger, user.UserID, profProperty.PropertyDefinitionId, 
                                                propertyValue, (int) profProperty.ProfileVisibility.VisibilityMode, 
                                                profProperty.ProfileVisibility.ExtendedVisibilityString(), DateTime.Now);
                    var objEventLog = new EventLogController();
                    objEventLog.AddLog(user, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, "", "USERPROFILE_UPDATED");
                }
            }
        }

        #endregion
    }
}
