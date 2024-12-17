namespace WebLab1
{
    public class Report
    {
        public int reportId;
        public int userId;
        public string report;
        public string status;

        public Report(int reportId, int userId, string report, string status)
        {
            this.reportId = reportId;
            this.userId = userId;
            this.report = report;
            this.status = status;
        }
    }
}
