using HtmlAgilityPack;
using System.Text;

namespace laythoikhoabieucuataoios17
{
    public class ScheduleItem
    {
        // Khởi tạo giá trị mặc định để tránh lỗi Non-nullable
        public string Day { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Session { get; set; } = string.Empty;
        public string SubjectInfo { get; set; } = string.Empty;
    }

    public static class ScheduleService
    {
        const string URL = "https://www.ttn.edu.vn/libraries/tnu/tkbieusinhvien.php";
        const string PAYLOAD = "msv=24103036&dk=10";

        public static async Task<List<ScheduleItem>> GetScheduleAsync()
        {
            var results = new List<ScheduleItem>();

            using (var client = new HttpClient())
            {
                // Cookie này cần còn hạn (lấy mới nếu app không chạy)
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
                client.DefaultRequestHeaders.Add("Referer", "https://www.ttn.edu.vn/?option=com_tnu&view=sinhvien");
                client.DefaultRequestHeaders.Add("Origin", "https://www.ttn.edu.vn");
                client.DefaultRequestHeaders.Add("Cookie", "b22691e170ccb76f2cd71526493be36a=mn92famsifppb0s20n0jsl89e7; sj_financial_tpl=sj_financial");

                var content = new StringContent(PAYLOAD, Encoding.UTF8, "application/x-www-form-urlencoded");

                try
                {
                    var response = await client.PostAsync(URL, content);
                    var htmlContent = await response.Content.ReadAsStringAsync();

                    var doc = new HtmlDocument();
                    doc.LoadHtml(htmlContent);

                    var tables = doc.DocumentNode.SelectNodes("//table");

                    if (tables != null)
                    {
                        for (int t = 0; t < tables.Count; t++)
                        {
                            var table = tables[t];
                            var rows = table.SelectNodes("tr");
                            if (rows == null || rows.Count < 2) continue;

                            var headers = rows[0].SelectNodes("th");

                            for (int r = 1; r < rows.Count; r++)
                            {
                                var cells = rows[r].SelectNodes("td");
                                if (cells == null) continue;

                                string sessionName = cells[0].InnerText.Trim();

                                for (int c = 1; c < cells.Count; c++)
                                {
                                    string cellText = cells[c].InnerText.Trim();
                                    if (!string.IsNullOrEmpty(cellText))
                                    {
                                        string rawDay = headers[c].InnerText;
                                        // Xử lý text html
                                        string dayName = rawDay.Replace("<br/>", " ").Trim();
                                        string info = System.Web.HttpUtility.HtmlDecode(cellText).Replace("<br/>", "\n");

                                        results.Add(new ScheduleItem
                                        {
                                            Day = dayName,
                                            Session = sessionName,
                                            SubjectInfo = info
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    results.Add(new ScheduleItem { SubjectInfo = "Lỗi: " + ex.Message });
                }
            }

            return results;
        }
    }
}