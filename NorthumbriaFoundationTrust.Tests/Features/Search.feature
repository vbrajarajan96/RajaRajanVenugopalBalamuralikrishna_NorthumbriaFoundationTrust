Feature: Search functionality
  As a Northumbria Foundation user
  I want to be able to perform a search for information regarding Quality and safety

  Background:
    Given the user is on the Northumbria NHS homepage

  @ui @nhs @smoke
  Scenario: Search for "Quality and safety" using the search button
    When the user enters "Quality and safety" in the search field
    And the user clicks the search button
    Then results relevant to "Quality and safety" are displayed
    And the user selects the "Quality and safety" link on the results page
    And the user navigates to the "Continually improving services" page
    Then the page shows the relevant information about this section

  @ui @nhs @regression
  Scenario: Search for "Quality and safety" using the Enter key
    When the user enters "Quality and safety" in the search field
    And the user presses Enter to submit the search
    Then results relevant to "Quality and safety" are displayed
    And the user selects the "Quality and safety" link on the results page
    And the user navigates to the "Continually improving services" page
    Then the page shows the relevant information about this section
