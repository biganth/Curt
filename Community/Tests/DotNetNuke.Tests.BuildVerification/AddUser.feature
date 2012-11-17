Feature: Add User
	In order to have users on the site
	As an adminstrator
	I want to be able to add new users

@MustBeDefaultAdminCredentials
@Userdeadmau5MustNotExist
Scenario: Add User
	Given I am on the site home page
	And I have logged in as the host
	And I have cleared the dotnetnuke cache
	And I am on the Admin Page User Accounts
	When I click Add User
	And I fill in the user form
		| Control      | Value				|
		| User Name    | deadmau5			|
		| First Name   | Joel 			    |
		| Last name    | Zimmerman			|
		| Display Name | deadmau5			|
		| Email        | deadmau5@dnn.com	|
		| Password     | meowingtons		|
	And I click Add User
	Then User Account deadmau5 is created correctly
