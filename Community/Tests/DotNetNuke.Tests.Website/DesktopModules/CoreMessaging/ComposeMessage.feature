Feature: Compose a Message
	In order to Send a message 
	As a user
	I want to be able to send a message to other users

Scenario Outline: Sending a Message to a Role
	Given Login as UID=<Scenario User Name> PWD=<Scenario Password> Role=<Scenario Role>
	When Click the Check Messages button located beside your display name in the top right corner of any page 
	And Click the Compose New Message button or link
	And In the Send to text box, begin typing in the user name, first name or last name of the message recipient or the name of the Role <To>
	And In the Subject text box, enter the subject of this message <Subject>
	And In the Your Message text box, enter the body of this message <Message>
	And Click the Send button
	Then A summary of the sent message is now displayed in the Message Center <Subject>

Examples: 
| Scenario User Name | Scenario Password | Scenario Role  | To               | Subject         | Message             |
| philip.beadle      | password          | Administrators | Registered Users | DotNetNuke Rox! | We love DotNetNuke. |