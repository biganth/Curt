#region Copyright
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
using System;
using System.Threading;
using DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects;
using DotNetNuke.Tests.Steps;
using NUnit.Framework;
using TechTalk.SpecFlow;
using Table = TechTalk.SpecFlow.Table;

namespace DotNetNuke.Tests.BuildVerification
{
    [Binding]
    public class AddUserSteps : WatiNTest
    {
        public UserPage UserPage
        {
            get
            {
                return GetPage<UserPage>();
            }
        }

        public HostUserPage HostUserPage
        {
            get
            {
                return GetPage<HostUserPage>();
            }
        }

        /// <summary>
        /// Adds a user with the information defined in the table.
        /// The user will be given the display name '{firstName} {lastName}'. 
        /// </summary>
        /// <param name="table">A table detailing the users credentials. 
        /// The table must follow the format below:
        ///| Control      | Value                   |
		///| User Name    | {userName}			    |
		///| First Name   | {firstName}			    |
		///| Last name    | {lastName}			    |
		///| Email        | {email}         		|
		///| Password     | {password}		    	|
        /// </param>
        [Given(@"The following new user has been created")]
        public void GivenTheFollowingNewUserHasBeenCreated(Table table)
        {
            string username = table.Rows[0]["Value"];
            string firstName = table.Rows[1]["Value"];
            string lastName = table.Rows[2]["Value"];
            string email = table.Rows[3]["Value"];
            string password = table.Rows[4]["Value"];
            string displayName = firstName + " " + lastName;
            IEInstance.GoTo(SiteUrl + "/admin/useraccounts.aspx");
            IEInstance.WaitForComplete();
            UserPage.AddNewUserStartLink.Click();
            IEInstance.WaitForComplete();
            UserPage.UserNameField.Value = username;
            UserPage.FirstNameField.Value = firstName;
            UserPage.LastNameField.Value = lastName;
            UserPage.DisplayNameField.Value = displayName;
            UserPage.EmailField.Value = email;
            UserPage.PasswordField.Value = password;
            UserPage.ConfirmPasswordField.Value = password;
            UserPage.AddNewUserLink.Click();
            IEInstance.WaitForComplete();
        }

        /// <summary>
        /// Clicks the add user link from either the User/Super user page, or the Edit User Accounts page. 
        /// </summary>
        [When(@"I click Add User")]
        public void WhenIClickAddUser()
        {
            IEInstance.WaitForComplete();
            if (HomePage.PopUpFrame != null || HomePage.ModuleTitleSpan.InnerHtml.Contains("Edit User Accounts"))
            {
                Assert.IsTrue(UserPage.AddNewUserLink.Exists);
                UserPage.AddNewUserLink.Click();
            }
            else
            {
                Assert.IsTrue(UserPage.AddNewUserStartLink.Exists);
                UserPage.AddNewUserStartLink.Click();
            }
            IEInstance.WaitForComplete();
        }

        /// <summary>
        /// Fills in the user form with the information specified in the table.
        /// </summary>
        ///  <param name="table">A table detailing the users credentials. 
        /// The table must follow the format below:
        ///| Control      | Value                   |
        ///| User Name    | {userName}			    |
        ///| First Name   | {firstName}			    |
        ///| Display Name | {displayName}           |
        ///| Last name    | {lastName}			    |
        ///| Email        | {email}         		|
        ///| Password     | {password}		    	|
        /// </param>
        [When(@"I fill in the user form")]
        public void WhenIFillInTheUserForm(TechTalk.SpecFlow.Table table)
        {
            IEInstance.WaitForComplete();
            UserPage.UserNameField.Value = table.Rows[0]["Value"];
            UserPage.FirstNameField.Value = table.Rows[1]["Value"];
            UserPage.LastNameField.Value = table.Rows[2]["Value"];
            UserPage.DisplayNameField.Value = table.Rows[3]["Value"];
            UserPage.EmailField.Value = table.Rows[4]["Value"];
            UserPage.PasswordField.Value = table.Rows[5]["Value"];
            UserPage.ConfirmPasswordField.Value = table.Rows[5]["Value"];
        }

        /// <summary>
        /// Checks that the username specified appears in the user table on the user accounts page.
        /// The test must already be on the User Accounts page. 
        /// </summary>
        /// <param name="userName">The username of the newly created user.</param>
        [Then(@"User Account (.*) is created correctly")]
        public void ThenUserIsCreatedCorrectly(string userName)
        {
            IEInstance.WaitForComplete();
            UserPage.AllDisplayLink.Click();
            IEInstance.WaitForComplete();
            WatiNAssert.AssertIsTrue(UserPage.UserTable.InnerHtml.Contains(userName), userName + "NotInUserTable.jpg");
        }

        /// <summary>
        /// Checks that the username specified appears in the user table on the Superuser Accounts page.
        /// The test must already be on the Superuser Accounts page. 
        /// </summary>
        /// <param name="userName">The username of the new Superuser</param>
        [Then(@"Super User Account (.*) is created correctly")]
        public void ThenSuperUserAccountIsCreatedCorrectly(string userName)
        {
            IEInstance.GoToNoWait(SiteUrl + "/Host/SuperuserAccounts.aspx");
            Thread.Sleep(2000);
            WatiNAssert.AssertIsTrue(HostUserPage.UserTable.InnerHtml.Contains(userName), userName + "NotInHostUserTable.jpg");
        }
    }
}
