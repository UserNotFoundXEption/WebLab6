using Newtonsoft.Json.Linq;
using System.Text.Json.Nodes;

namespace WebLab1
{
    public class WebSocketHandler
    {
        public string HandleMessage(JObject jsonMessage)
        {
            string? type = jsonMessage["type"]?.ToString();
            switch (type)
            {
                case "sendOnChat":
                    {
                        int senderId = jsonMessage["senderId"]?.ToObject<int>() ?? 0;
                        int receiverId = jsonMessage["receiverId"]?.ToObject<int>() ?? 0;
                        string? message = jsonMessage["message"]?.ToString();
                        bool bothAdmins = senderId == 0 && receiverId == 0;
                        bool noAdmin = senderId != 0 && receiverId != 0;
                        bool wrongId = senderId < 0 || receiverId < 0 || senderId > 3 || receiverId > 3;
                        if (bothAdmins || noAdmin || wrongId || message == null || message == "")
                        {
                            return ToJson(new { type, data = "wrong sendOnChat parameters" });
                        }
                        AddNewMessage(senderId, receiverId, message);
                        return ToJson(new { type, data = new { senderId, receiverId, message } });
                    }
                case "fetchChat":
                    {
                        int chatId = jsonMessage["chatId"]?.ToObject<int>() ?? 0;
                        if (chatId < 1 || chatId > 3)
                        {
                            return ToJson(new { type, data = "wrong fetchChat parameters" });
                        }
                        return ToJson(new { type, data = GetChat(chatId) });
                    }
                case "addReport":
                    {
                        int userId = jsonMessage["userId"]?.ToObject<int>() ?? 0;
                        string? report = jsonMessage["report"]?.ToString();
                        if (report == null)
                        {
                            return ToJson(new { type, data = "addReport received empty report" });
                        }
                        if(userId < 1 || userId > 3)
                        {
                            return ToJson(new { type, data = "addReport received wrong userId" });
                        }
                        int reportId = AddNewReport(userId, report);
                        return ToJson(new { type, data = new { reportId, userId, report, status = "Pending" } });
                    }          
                case "deleteReport":
                    {
                        int reportId = jsonMessage["reportId"]?.ToObject<int>() ?? 0;
                        bool deletedReport = DeleteReport(reportId);
                        if(!deletedReport)
                        {
                            return ToJson(new { type, data = "deleteReport couldn't find report to delete" });
                        }
                        return ToJson(new { type, data = new { reportId } });
                    }
                case "fetchReports":
                    {
                        int userId = jsonMessage["userId"]?.ToObject<int>() ?? 0;
                        if (userId < 0 || userId > 3)
                        {
                            return ToJson(new { type, data = "wrong fetchReports userId" });
                        }
                        return ToJson(new { type, data  = FetchReports(userId) });
                    }
                case "editReport":
                    {
                        int reportId = jsonMessage["reportId"]?.ToObject<int>() ?? 0;
                        string? status = jsonMessage["status"]?.ToString();
                        if(status != "Pending" && status != "In Progress" && status != "Completed")
                        {
                            return ToJson(new { type, data = "editReport received wrong report status" });
                        }
                        bool changedReportStatus = ChangeReportStatus(reportId, status);
                        if (!changedReportStatus)
                        {
                            return ToJson(new { type, data = "editReport received wrong reportId" });
                        }
                        return ToJson(new { type, data = new { reportId, status } });
                    }
                default:
                    {
                        return ToJson(new { type, data = "wrong websocket message type" });
                    }
            }   
        }

        private string ToJson(object data)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(data);
        }

        private void AddNewMessage(int senderId, int receiverId, string message)
        {
            int chatId = 0;
            if(senderId > 0)
            {
                chatId = senderId;
            }
            if(receiverId > 0)
            {
                chatId = receiverId;
            }
            switch (chatId)
            {
                case 1:
                    Data.chat1.Add(new ChatMessage(senderId, receiverId, message));
                    break;
                case 2:
                    Data.chat2.Add(new ChatMessage(senderId, receiverId, message));
                    break;
                case 3:
                    Data.chat3.Add(new ChatMessage(senderId, receiverId, message));
                    break;
            }
        }

        private List<ChatMessage> GetChat(int chatId)
        {
            switch (chatId)
            {
                case 1:
                    return Data.chat1;
                case 2:
                    return Data.chat2;
                case 3:
                    return Data.chat3;
                default:
                    return new List<ChatMessage>();
            }
        }

        private int AddNewReport(int userId, string report)
        {
            int reportId = Data.reportCount;
            Data.reportCount++;
            Data.reports.Add(new Report(reportId, userId, report, "Pending"));
            return reportId;
        }

        private bool DeleteReport(int reportId)
        {
            Report reportToDelete = null;
            foreach(Report report in Data.reports)
            {
                if(report.reportId == reportId)
                {
                    reportToDelete = report;
                }
            }
            if (reportToDelete == null)
            {
                return false;

            }
            Data.reports.Remove(reportToDelete);
            return true;
        }

        private List<Report> FetchReports(int userId)
        {
            if(userId == 0)
            {
                return Data.reports;
            }
            else
            {
                List<Report> result = new List<Report>();
                foreach(Report report in Data.reports)
                {
                    if(report.userId == userId)
                    {
                        result.Add(report);
                    }
                }
                return result;
            }
        }

        private bool ChangeReportStatus(int reportId, string status)
        {
            bool foundReport = false;
            foreach (Report report in Data.reports)
            {
                if (report.reportId == reportId)
                {
                    report.status = status;
                    foundReport = true;
                }
            }
            return foundReport;
        }
    }
}
