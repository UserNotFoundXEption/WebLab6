using Newtonsoft.Json.Linq;
using TechTalk.SpecFlow;
using NUnit;
using NUnit.Framework;
using Newtonsoft.Json;

namespace SpecFlowTests.StepDefinitions
{
    [Binding]
    public class StepDefinitions
    {
        private WebSocketHandler handler = new WebSocketHandler();
        private string type;
        private int senderId;
        private int receiverId;
        private string message;
        private int chatId;
        private int userId;
        private int reportId;
        private string reportStatus;
        private string reportContent;
        private JObject messageToHandle;
        private JObject response;

        // Given steps
        [Given(@"a JSON message with type ""(.*)""")]
        public void GivenAJsonMessageWithType(string messageType)
        {
            type = messageType;
        }

        [Given(@"senderId is (.*)")]
        public void GivenSenderId(int sender)
        {
            senderId = sender;
        }

        [Given(@"receiverId is (.*)")]
        public void GivenReceiverId(int receiver)
        {
            receiverId = receiver;
        }

        [Given(@"message is ""(.*)""")]
        public void GivenMessage(string msg)
        {
            if(msg == "null")
            {
                msg = null;
            }
            message = msg;
        }

        [Given(@"chatId is (.*)")]
        public void GivenChatIdIs(int id)
        {
            chatId = id;
        }

        [Given(@"userId is (.*)")]
        public void GivenUserId(int id)
        {
            userId = id;
        }

        [Given(@"report is ""(.*)""")]
        public void GivenReportContent(string content)
        {
            reportContent = content;
        }

        [Given(@"reportId is (.*)")]
        public void GivenReportId(int id)
        {
            reportId = id;
        }

        [Given(@"status is ""(.*)""")]
        public void GivenStatus(string status)
        {
            reportStatus = status;
        }

        // When steps
        [When(@"the sendOnChat message")]
        public void WhenTheSendOnChatMessage()
        {
            messageToHandle = new JObject
            {
                ["type"] = type,
                ["senderId"] = senderId,
                ["receiverId"] = receiverId,
                ["message"] = message
            };
        }

        [When(@"the fetchChat message")]
        public void WhenTheFetchChatMessage()
        {
            messageToHandle = new JObject
            {
                ["type"] = type,
                ["chatId"] = chatId
            };
        }

        [When(@"the addReport message")]
        public void WhenTheAddReportMessage()
        {
            messageToHandle = new JObject
            {
                ["type"] = type,
                ["reportId"] = reportId,
                ["report"] = reportContent,
                ["status"] = "Pending"
            };
        }

        [When(@"the deleteReport message")]
        public void WhenTheDeleteReportMessage()
        {
            messageToHandle = new JObject
            {
                ["type"] = type,
                ["reportId"] = reportId,
            };
        }

        [When(@"the fetchReports message")]
        public void WhenTheFetchReportsMessage()
        {
            messageToHandle = new JObject
            {
                ["type"] = type,
                ["userId"] = userId
            };
        }

        [When(@"the editReport message")]
        public void WhenTheEditReportMessage()
        {
            messageToHandle = new JObject
            {
                ["type"] = type,
                ["reportId"] = reportId,
                ["status"] = "Pending"
            };
        }

        [When(@"the default message")]
        public void WhenDefaultMessage()
        {
            messageToHandle = new JObject(new { type = "some wrong type", data = new {} });
        }

        [When(@"message is handled")]
        public void WhenMessageIsHandled()
        {
            string responseString = handler.HandleMessage(messageToHandle);
            response = JObject.Parse(responseString);
        }

        // Then steps
        [Then(@"the response should contain type ""(.*)""")]
        public void ThenTheResponseShouldContainType(string expectedType)
        {
            var resultType = response["type"]?.ToString();
            Assert.AreEqual(resultType, expectedType);
        }

        [Then(@"the data should be ""(.*)""")]
        public void ThenTheDataShouldBe(string expectedData)
        {
            var resultData = response["data"]?.ToString();
            try
            {
                var actualJsonString = JsonConvert.SerializeObject(JObject.Parse(resultData), Formatting.None);
                var expectedJsonString = JsonConvert.SerializeObject(JObject.Parse(expectedData), Formatting.None);
                Assert.AreEqual(expectedJsonString, actualJsonString);
            }
            catch
            {
                Assert.AreEqual(expectedData, resultData);
            }
        }
    }
}
