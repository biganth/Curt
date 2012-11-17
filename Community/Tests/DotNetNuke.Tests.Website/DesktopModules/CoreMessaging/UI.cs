using OpenQA.Selenium;

namespace DotNetNuke.Tests.Website.DesktopModules.CoreMessaging
{
    class UI
    {
        public static IWebElement MessagesLink(IWebDriver driver) { return driver.FindDnnElementByXpath(driver, "//*[@id='dnn_dnnUser_messageLink']/strong"); }
        public static IWebElement ComposeNewMessageLink(IWebDriver driver) { return driver.FindDnnElementByXpath(driver, "//*[@id='dnnCoreMessaging']/div[3]/a[1]"); }
        public static IWebElement SendToTextBox(IWebDriver driver) { return driver.FindDnnElementByXpath(driver, "//*[@id='token-input-to']"); }
        public static IWebElement SubjectTextBox(IWebDriver driver) { return driver.FindDnnElementByXpath(driver, "//*[@id='subject']"); }
        public static IWebElement MessageTextBox(IWebDriver driver) { return driver.FindDnnElementByXpath(driver, "//*[@id='bodytext']"); }
        public static IWebElement SendToDropDown(IWebDriver driver) { return driver.FindDnnElementByXpath(driver, "//*[@id='tokenResults']"); }
        public static IWebElement SendButton(IWebDriver driver) { return driver.FindDnnElementByXpath(driver, "//*[@id='Body']/div[6]/div[3]/div/button[1]/span"); }
        public static IWebElement ReplySendButton(IWebDriver driver) { return driver.FindDnnElementByXpath(driver, "//*[@id='dnn_dnnUser_messageLink']/strong"); }
        public static IWebElement CancelButton(IWebDriver driver) { return driver.FindDnnElementByXpath(driver, ""); }
        public static IWebElement Message(IWebDriver driver) { return driver.FindDnnElementByXpath(driver, "//*[@id='inbox']/li[1]"); }
        public static IWebElement MessageTitle(IWebDriver driver) { return driver.FindDnnElementByXpath(driver, "//*[@id='inbox']/li[1]/ul/li[3]/dl/dt/a"); }
        public static IWebElement SelectedReplyLink(IWebDriver driver) { return driver.FindDnnElementByXpath(driver, "//*[@id='SelectedReply']"); }
        public static IWebElement SelectedMessage(IWebDriver driver) { return driver.FindDnnElementByXpath(driver, "//*[@id='replyMessage']"); }
        public static IWebElement SelectedReplyButton(IWebDriver driver) { return driver.FindDnnElementByXpath(driver, "//*[@id='SelectedReplyButton']"); }
    }
}
