using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Net.Http;

namespace Amazon_console
{
    internal partial class Creator
    {
        private string deviceId;
        private readonly string adbPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + "\\adb.exe";
        private readonly ADBClient adb = new();
        private readonly HttpClient client = new();

        public Creator(string id)
        {
            deviceId = id;
            adb.AdbPath = adbPath;
        }

        public string DeviceId
        {
            get { return deviceId; }
        }
        private Random rnd = new();

        private DateTime RandomDay()
        {
            DateTime today = DateTime.Today;
            DateTime start = new DateTime(today.Year - 35, 1, 1);
            DateTime end = new DateTime(today.Year - 18, today.Month, today.Day);
            int range = (end - start).Days;
            return start.AddDays(rnd.Next(range));
        }

        private (string first, string last) RandomName()
        {
            HttpRequestMessage webRequest = new(HttpMethod.Post, "https://www.behindthename.com/random/random.php?gender=both&number=2&sets=1&surname=&all=yes");

            var response = client.Send(webRequest);
            using var reader = new StreamReader(response.Content.ReadAsStream());
            string result = reader.ReadToEnd();

            string pattern = @"class=""plain"">(\w+)";
            Regex rx = new(pattern);
            MatchCollection matches = rx.Matches(result);

            return (matches[0].Groups[1].Value, matches[1].Groups[1].Value);
        }

        public void QwetyInput(string input)
        {
            string alphabet = "abcdefghijklmnopqrstuvwxyz. \n";
            StringComparison condition = StringComparison.OrdinalIgnoreCase;
            bool isStringKeyboard = true;

            foreach (char c in input.ToLower())
            {
                // check keyboard
                if (alphabet.Contains(c, condition) ^ isStringKeyboard)
                {
                    // change qwerty keyboard from alphabet to number and vice versa
                    adb.Execute(deviceId, "input tap 85 1830");
                    isStringKeyboard = !isStringKeyboard;
                }
                adb.Execute(deviceId, Entities.KeyStore[c]);
            }
        }

        public void NumpadInput(int input)
        {
            string inputStr = input.ToString();
            foreach (char c in inputStr)
            {
                adb.Execute(deviceId, Entities.NumberStore[c]);
                //adb.Sleep(200);
            }
        }

        public string GetUI()
        {
            adb.Execute(deviceId, "uiautomator dump");
            adb.Execute(deviceId, "cat /sdcard/window_dump.xml");
            return adb.Output;
        }

        public (int X, int Y) getTapFromUI(string uiXml, string searchText)
        {
            string pattern = @"""" + searchText + @"""[\w+| |=|""|\.|\-|:/]+\[(\d+),(\d+)\]\[(\d+),(\d+)\]";

            Regex rx = new(pattern, RegexOptions.IgnoreCase);
            Match match = rx.Match(uiXml);

            int X = (Int16.Parse(match.Groups[1].Value) + Int16.Parse(match.Groups[3].Value)) / 2;
            int Y = (Int16.Parse(match.Groups[2].Value) + Int16.Parse(match.Groups[4].Value)) / 2;

            return (X, Y);
        }
    }
}
