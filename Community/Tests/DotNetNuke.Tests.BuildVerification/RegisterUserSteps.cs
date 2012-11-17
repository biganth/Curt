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
using DotNetNuke.Entities.Users;
using DotNetNuke.Tests.Steps;
using DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects;
using TechTalk.SpecFlow;

namespace DotNetNuke.Tests.BuildVerification
{
    [Binding]
    public class RegisterUserSteps : WatiNTest
    {
        public RegisterPage RegisterPage
        {
            get
            {
                return GetPage<RegisterPage>();
            }
        }

        private string currentUserName;

        /// <summary>
        /// Clicks either the register link displayed on every page, or the register button on the user registration form.
        /// If a user is logged in this step will click the link for the users profile page. 
        /// </summary>
        [When(@"I click the Register link")]
        public void WhenIClickTheRegisterLink()
        {
            if(HomePage.RegisterLink.Text != "Login")
            {
                IEInstance.GoTo(SiteUrl + "/logoff.aspx");
            }
            HomePage.RegisterLink.Click();
        }

        [When(@"I click the Register button")]
        public void WhenIClickTheRegisterButton()
        {
            RegisterPage.RegisterButton.Click();
        }

        /// <summary>
        /// Fills in the register user form with information from the table.
        /// </summary>
        /// <param name="table">A table with information about the user that will be created.
        /// The table must be in the following format:
        /// | Control      | Value						|
		/// | User Name    | {userName}     			|
        /// | Email        | {email}                	|
        /// | Password     | {password}					|
        /// | Display Name | {displayName}      		|
		/// </param>
        [When(@"I fill in the Register User form")]
        public void WhenIFillInTheRegisterUserForm(Table table)
        {
            currentUserName = table.Rows[0]["Value"];
            RegisterPage.UserNameField.Value = table.Rows[0]["Value"];
            RegisterPage.EmailField.Value = table.Rows[1]["Value"];
            RegisterPage.PasswordField.Value = table.Rows[2]["Value"];
            RegisterPage.ConfirmPasswordField.Value = table.Rows[2]["Value"];
            RegisterPage.DisplayNameField.Value = table.Rows[3]["Value"];
        }

        /// <summary>
        /// Checks that the users display name now appears as the text for the register/user profile link.
        /// </summary>
        /// <param name="displayName">The users display name.</param>
        [Then(@"I am logged in as the (.*) user")]
        public void IAmLoggedInAsTheRegisterUser(string displayName)
        {
            IEInstance.WaitUntilContainsText(displayName);
            WatiNAssert.AssertIsTrue(HomePage.RegisterLink.Text.StartsWith(displayName), displayName + "NotLoggedIn.jpg", HomePage.RegisterLink.Text);
        }

        [AfterScenario("RemoveAddedUser")]
        public void AfterScenario()
        {
            var userToDelete = UserController.GetUserByName(0, currentUserName);
            if (currentUserName != null)
            {
                UserController.RemoveUser(userToDelete);
            }

            HomePage.LoginLink.Click();
        }
    }
}
