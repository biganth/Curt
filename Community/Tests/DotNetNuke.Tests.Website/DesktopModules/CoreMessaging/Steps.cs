using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Social.Messaging;
using DotNetNuke.Services.Social.Messaging.Internal;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using TechTalk.SpecFlow;

namespace DotNetNuke.Tests.Website.DesktopModules.CoreMessaging
{
    public partial class Steps
    {
        [Given(@"There must be at least one Message in the Message Center (.*), (.*)")]
        public void GivenThereMustBeAtLeastOneMessageInTheMessageCenter(String subject, String body)
        {
            var user = UserController.GetCurrentUserInfo();

            var inbox = InternalMessagingController.Instance.GetRecentInbox(User.UserID, -1, 10);
            if (inbox.Conversations.Count == 0)
            {
                var users = new List<UserInfo> { user };
                MessagingController.Instance.SendMessage(new Message { Body = body, Subject = subject }, null, users, null);
            }
        }

        [When(@"In the Send to text box, begin typing in the user name, first name or last name of the message recipient or the name of the Role (.*)")]
        public void WhenInTheSendToTextBoxBeginTypingInTheUserNameFirstNameOrLastNameOfTheMessageRecipientOrTheNameOfTheRole_ThisWillDisplayAListOfTheMatchingUsersAndRolesThatYouCanChooseFromTo(string to)
        {
            UI.SendToTextBox(Driver).SendKeys(to);
            Driver.FindDnnElementByXpath(Driver, "//*[@id='tokenResults']/li").Click();
        }

        [When(@"Hover your mouse over the message title and then click the Reply link")]
        public void WhenHoverYourMouseOverTheMessageTitleAndThenClickTheReplyLink()
        {
            var action = new Actions(Driver);
            action.MoveToElement(UI.Message(Driver));
        }

        [When(@"Click the message title and then click the Reply link")]
        public void WhenClickTheMessageTitleAndThenClickTheReplyLink()
        {
            UI.MessageTitle(Driver).Click();
            UI.SelectedReplyLink(Driver).Click();
        }

        [When(@"In the Message text box, enter the body of this message (.*)")]
        public void WhenInTheMessageTextBoxEnterTheBodyOfThisMessageMessage(string message)
        {
            UI.SelectedMessage(Driver).SendKeys(message);
        }

        [When(@"Click the Reply button")]
        public void WhenClickTheReplyButton()
        {
            UI.SelectedReplyButton(Driver).Click();        
        }

        [Then(@"A summary of the sent message is now displayed in the Message Center (.*)")]
        public void ThenASummaryOfTheSentMessageIsNowDisplayedInTheMessageCenter(string subject)
        {
            Driver.FindElements(By.LinkText(subject));
        }

        [Then(@"The reply will now be displayed below the original message (.*)")]
        public void ThenTheReplyWillNowBeDisplayedBelowTheOriginalMessage(string message)
        {
            Driver.FindElements(By.LinkText(message));
        }
    }
}
