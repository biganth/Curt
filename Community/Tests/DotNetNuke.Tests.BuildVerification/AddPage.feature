Feature: Add Page
	In order to add content to my site
	As an administrator
	I want to be able to add pages to the site


@MustBeDefaultAdminCredentials
@MustUseRibbonBar
@PageDesignersMustNotExist
Scenario: Add A Simple Page From The Ribbon Bar
	Given I am on the site home page
	And I have logged in as the admin
	When I press Add from the Ribbon Bar
	And I enter the page name Designers into the page settings 
	And I click Add Page
	Then A page called Designers should be created


