Feature: Reply to a Message
	In order to reply to a message sent to me
	As a user of the site
	I want to be abvle to easily reply to messages sent to me

Scenario Outline: Replying To A Message via Reply Link
	Given Login as UID=<Scenario User Name> PWD=<Scenario Password> Role=<Scenario Role>
	And There must be at least one Message in the Message Center Test Message, Test Body
	When Click the Check Messages button located beside your display name in the top right corner of any page
	And Click the message title and then click the Reply link 
	And In the Message text box, enter the body of this message <Message>
	And Click the Reply button
	Then The reply will now be displayed below the original message <Message>
	Examples: 
| Scenario User Name | Scenario Password | Scenario Role  | Message                        |
| philip.beadle      | password          | Administrators | Replying to a selected message |