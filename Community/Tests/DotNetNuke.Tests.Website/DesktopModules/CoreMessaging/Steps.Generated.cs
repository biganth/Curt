using TechTalk.SpecFlow;

namespace DotNetNuke.Tests.Website.DesktopModules.CoreMessaging
{
    [Binding]
    public partial class Steps : AutomationBase
    {
        [When(@"Click the Check Messages button located beside your display name in the top right corner of any page")]
        public void WhenClickTheCheckMessagesButtonLocatedBesideYourDisplayNameInTheTopRightCornerOfAnyPage()
        {
            UI.MessagesLink(Driver).Click();
        }

        [When(@"Click the Compose New Message button or link")]
        public void WhenClickTheComposeNewMessageButtonOrLink()
        {
            UI.ComposeNewMessageLink(Driver).Click();
        }

        [When(@"In the Subject text box, enter the subject of this message (.*)")]
        public void WhenInTheSubjectTextBoxEnterTheSubjectOfThisMessageSubject(string subject)
        {
            UI.SubjectTextBox(Driver).SendKeys(subject);
        }

        [When(@"In the Your Message text box, enter the body of this message (.*)")]
        public void WhenInTheYourMessageTextBoxEnterTheBodyOfThisMessageMessage(string message)
        {
            UI.MessageTextBox(Driver).SendKeys(message);
        }

        [When(@"Click the Send button")]
        public void WhenClickTheSendButton()
        {
            UI.SendButton(Driver).Click();
        }

        [When(@"Click the Load More link to view additional messages")]
        public void WhenClickTheLoadMoreLinkToViewAdditionalMessages()
        {
        }

    }
}
