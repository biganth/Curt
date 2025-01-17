﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:1.8.1.0
//      SpecFlow Generator Version:1.8.0.0
//      Runtime Version:4.0.30319.269
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace DotNetNuke.Tests.Website.DesktopModules.Admin.Security
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "1.8.1.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("How to add a new user to a DotNetNuke site using the Add New User module")]
    public partial class HowToAddANewUserToADotNetNukeSiteUsingTheAddNewUserModuleFeature
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "AddUser.feature"
#line hidden
        
        [NUnit.Framework.TestFixtureSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "How to add a new user to a DotNetNuke site using the Add New User module", "In order to have users on the site\r\nAs an adminstrator\r\nI want to be able to add " +
                    "new users", ProgrammingLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [NUnit.Framework.TestFixtureTearDownAttribute()]
        public virtual void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [NUnit.Framework.SetUpAttribute()]
        public virtual void TestInitialize()
        {
        }
        
        [NUnit.Framework.TearDownAttribute()]
        public virtual void ScenarioTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioSetup(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioStart(scenarioInfo);
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Add User from the Users Accounts Logged In As a User in the Administrators Role")]
        [NUnit.Framework.TestCaseAttribute("deadmau5", "password", "Administrators", "wilson", "Jon", "Wilson", "Wilson", "wilson@dnncorp.com", "True", "True", "False", "password", "password", new string[0])]
        [NUnit.Framework.TestCaseAttribute("philt3r", "password", "Administrators", "tiesto", "The", "Dutchman", "Tiesto", "tiesto@dnncorp.com", "True", "True", "False", "password", "password", new string[0])]
        public virtual void AddUserFromTheUsersAccountsLoggedInAsAUserInTheAdministratorsRole(string scenarioUserName, string scenarioPassword, string scenarioRole, string userName, string firstName, string lastName, string displayName, string email, string authorize, string notify, string randonPassword, string password, string confirmPassword, string[] exampleTags)
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Add User from the Users Accounts Logged In As a User in the Administrators Role", exampleTags);
#line 6
this.ScenarioSetup(scenarioInfo);
#line hidden
            TechTalk.SpecFlow.Table table1 = new TechTalk.SpecFlow.Table(new string[] {
                        "Role",
                        "Permission",
                        "Value"});
            table1.AddRow(new string[] {
                        "All Users",
                        "View",
                        "Allowed"});
#line 7
 testRunner.Given("There is a Page called User Accounts with these permissions", ((string)(null)), table1);
#line hidden
            TechTalk.SpecFlow.Table table2 = new TechTalk.SpecFlow.Table(new string[] {
                        "Role",
                        "Permission",
                        "Value"});
            table2.AddRow(new string[] {
                        "User Account Administrator",
                        "View",
                        "Allowed"});
#line 10
 testRunner.And("There is a User Accounts module on the page with these permissions", ((string)(null)), table2);
#line 13
 testRunner.Given(string.Format("Login as UID={0} PWD={1} Role={2}", scenarioUserName, scenarioPassword, scenarioRole));
#line 14
 testRunner.And("I click Add User");
#line 15
 testRunner.When(string.Format("I set Add User User Name to {0}", userName));
#line 16
 testRunner.And(string.Format("I set Add User First Name to {0}", firstName));
#line 17
 testRunner.And(string.Format("I set Add User Last Name to {0}", lastName));
#line 18
 testRunner.And(string.Format("I set Add User Display Name to {0}", userName));
#line 19
 testRunner.And(string.Format("I set Add User Email to {0}", email));
#line 20
 testRunner.And(string.Format("I set Add User Password to {0}", password));
#line 21
 testRunner.And(string.Format("I set Add User Authorize to {0}", authorize));
#line 22
 testRunner.And(string.Format("I set Add User Notify to {0}", notify));
#line 23
 testRunner.And(string.Format("I set Add User Random Password to {0}", randonPassword));
#line 24
 testRunner.And(string.Format("I set Add User Password to {0}", password));
#line 25
 testRunner.And(string.Format("I set Add User Confirm Password to {0}", confirmPassword));
#line 26
 testRunner.And("I click Add New User");
#line 27
 testRunner.Then("The newly added user account can now be viewed and modified using the User Accoun" +
                    "ts module");
#line 28
 testRunner.And("If Authorize is checked the new user will automatically gain access to the Regist" +
                    "ered User role and any roles set for Auto Assignment");
#line 29
 testRunner.And("If Authorize is unchecked the new user will be created but will not be able to ac" +
                    "cess the restricted areas of the site");
#line 30
 testRunner.And("If Notify is checked the new user will be sent a notification email");
#line 31
 testRunner.And("If Notify is unchecked the new user will not be sent a notification email");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
