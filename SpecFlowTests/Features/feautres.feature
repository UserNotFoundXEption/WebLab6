Feature: Step Definitions

  Scenario Outline: Send a message on chat with various validation rules
    Given a JSON message with type "sendOnChat"
    And senderId is <senderId>
    And receiverId is <receiverId>
    And message is "<message>"
    When the sendOnChat message
    And message is handled
    Then the response should contain type "sendOnChat"
    And the data should be "<expectedResult>"

    Examples:
      | senderId | receiverId | message      | expectedResult                                      |
      | 0        | 2          | Hello        | {"senderId":0,"receiverId":2,"message":"Hello"}   |
      | 0        | 0          | Hello        | wrong sendOnChat parameters                         |
      | -1       | 0          | Test         | wrong sendOnChat parameters                         |
      | 0        | 4          | Test         | wrong sendOnChat parameters                         |
      | 1        | 3          | Test         | wrong sendOnChat parameters                         |
      | 0        | 2          | null         | wrong sendOnChat parameters                         |

  Scenario Outline: Fetch chat messages with valid and invalid chatIds
    Given a JSON message with type "fetchChat"
    And chatId is <chatId>
    When the fetchChat message
    And message is handled
    Then the response should contain type "fetchChat"
    And the data should be "<expectedResult>"

    Examples:
      | chatId | expectedResult               |
      | 1      | A list of chat messages      |
      | 0      | wrong fetchChat parameters   |
      | 4      | wrong fetchChat parameters   |

  Scenario Outline: Add a new report with validation rules for userId and report
    Given a JSON message with type "addReport"
    And userId is <userId>
    And report is "<report>"
    When the addReport message
    And message is handled
    Then the response should contain type "addReport"
    And the data should be "<expectedResult>"

    Examples:
      | userId | report           | expectedResult                                                             |
      | 1      | System crash     | {"reportId":1, "userId":1, "report":"System crash", "status":"Pending"}    |
      | 2      | Server overload  | {"reportId":2, "userId":2, "report":"Server overload", "status":"Pending"} |
      | 0      | Invalid report   | addReport received wrong userId                                            |
      | 4      | Critical issue   | addReport received wrong userId                                            |
      | 1      | null             | addReport received empty report                                            |

  Scenario Outline: Delete a report with existing and non-existing report IDs
    Given a JSON message with type "deleteReport"
    And reportId is <reportId>
    When the deleteReport message
    And message is handled
    Then the response should contain type "deleteReport"
    And the data should be "<expectedResult>"

    Examples:
      | reportId | expectedResult                                      |
      | 1        | {"reportId":1}                                      |
      | 999      | deleteReport couldn't find report to delete         |
      | 0        | deleteReport couldn't find report to delete         |

  Scenario Outline: Fetch reports for valid and invalid user IDs
    Given a JSON message with type "fetchReports"
    And userId is <userId>
    When the fetchReports message
    And message is handled
    Then the response should contain type "fetchReports"
    And the data should be "<expectedResult>"

    Examples:
      | userId | expectedResult                  |
      | 1      | Reports for userId 1            |
      | 0      | All available reports           |
      | 4      | wrong fetchReports userId       |
      | -1     | wrong fetchReports userId       |

  Scenario Outline: Edit a report status with valid and invalid statuses
    Given a JSON message with type "editReport"
    And reportId is <reportId>
    And status is "<status>"
    When the editReport message
    And message is handled
    Then the response should contain type "editReport"
    And the data should be "<expectedResult>"

    Examples:
      | reportId | status        | expectedResult                                |
      | 1        | Pending       | {"reportId":1, "status":"Pending"}            |
      | 2        | In Progress   | {"reportId":2, "status":"In Progress"}        |
      | 999      | Pending       | editReport received wrong reportId            |
      | 1        | Ignored       | editReport received wrong report status       |

  Scenario: Handle a JSON message with an invalid message type
    Given a JSON message with type "invalidType"
    When the default message
    When message is handled
    Then the response should contain type "invalidType"
    And the data should be "wrong websocket message type"
