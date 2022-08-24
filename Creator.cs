using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Net.Http;

namespace Amazon_console
{
    internal class Creator
    {
        private string deviceId;
        private string firstName;
        private string lastName;
        private string mailAddress;
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
            string alphabet = "abcdefghijklmnopqrstuvwxyz. ";
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

        public bool WifiCheck()
        {
            adb.Execute(deviceId, "\"dumpsys wifi | grep mNetworkInfo\"");
            string outLines = adb.Output;
            if (outLines != "mNetworkInfo [type: WIFI[], state: UNKNOWN/IDLE, reason: (unspecified), extra: (none), failover: false, available: false, roaming: false]")
            {
                return false;
            }
            return true;
        }

        public void ScreenOn()
        {
            adb.Execute(deviceId, "\"dumpsys window | grep screenState\"");
            string outLines = adb.Output;
            if (outLines.Trim() == "screenState=SCREEN_STATE_OFF")
            {
                adb.Execute(deviceId, "input keyevent KEYCODE_POWER");
            }
        }

        public void Unlock()
        {
            adb.Execute(deviceId, "\"dumpsys window | grep mDreamingLockscreen\"");
            string outLines = adb.Output;
            outLines = outLines.TrimStart();
            outLines = outLines.TrimEnd();
            if (outLines == "mShowingDream=false mDreamingLockscreen=true mDreamingSleepToken=null")
            {
                adb.Execute(deviceId, "input keyevent KEYCODE_MENU");
            }
        }

        public bool OpenMailRegister()
        {
            adb.Execute(deviceId, "am force-stop com.google.android.gms");
            adb.Execute(deviceId, "am start -n com.google.android.gms/.auth.uiflows.addaccount.AccountIntroActivity");
            bool check;
            string uiXml;
            do
            {
                adb.Sleep(2000); //wait for AccountIntroActivity load
                uiXml = GetUI();
                check = uiXml.Contains("text=\"Sign in\" resource-id=\"headingText\"");

            } while (!check);

            (int X, int Y) position = getTapFromUI(uiXml, "android.widget.Spinner");

            adb.Execute(deviceId, $"input tap {position.X} {position.Y}");

            uiXml = GetUI();
            position = getTapFromUI(uiXml, "For myself");
            adb.Execute(deviceId, $"input tap {position.X} {position.Y}");

            return true;
        }

        public bool NameInput()
        {
            string uiXml = GetUI();
            if (!uiXml.Contains("text=\"Enter your name\""))
            {
                return false;
            }
            (firstName, lastName) = RandomName();

            (int X, int Y) position = getTapFromUI(uiXml, "firstName"); //First Name
            adb.Execute(deviceId, $"input tap {position.X} {position.Y}");
            QwetyInput(firstName);
            adb.Execute(deviceId, "input keyevent KEYCODE_BACK");

            position = getTapFromUI(uiXml, "lastName"); //Last Name
            adb.Execute(deviceId, $"input tap {position.X} {position.Y}");
            QwetyInput(lastName);
            adb.Execute(deviceId, "input keyevent KEYCODE_BACK");

            position = getTapFromUI(uiXml, "NEXT"); //NEXT
            adb.Execute(deviceId, $"input tap {position.X} {position.Y}");

            return true;
        }

        public bool BirthdayAndGender()
        {
            string uiXml = GetUI();
            if (!uiXml.Contains("text=\"Enter your birthday and gender\""))
            {
                return false;
            }

            int index = rnd.Next(0, 2);

            DateTime birthday = RandomDay();

            (int X, int Y) monthPos = getTapFromUI(uiXml, "Month");
            (int X, int Y) dayPos = getTapFromUI(uiXml, "day");
            (int X, int Y) yearPos = getTapFromUI(uiXml, "year");
            (int X, int Y) genderPos = getTapFromUI(uiXml, "Gender");
            (int X, int Y) nextPos = getTapFromUI(uiXml, "NEXT");

            adb.Execute(deviceId, $"input tap {monthPos.X} {monthPos.Y}"); //Month tap
            adb.Execute(deviceId, Entities.Month[birthday.Month]);         //Month choose

            adb.Execute(deviceId, $"input tap {dayPos.X} {dayPos.Y}"); //Day tap
            NumpadInput(birthday.Day);

            adb.Execute(deviceId, $"input tap {yearPos.X} {yearPos.Y}"); //Year tap
            NumpadInput(birthday.Year);

            adb.Execute(deviceId, "input keyevent KEYCODE_BACK");

            adb.Execute(deviceId, $"input tap {genderPos.X} {genderPos.Y}"); //Gender tap
            adb.Execute(deviceId, Entities.Gender[index]);                   //Gender choose

            adb.Execute(deviceId, $"input tap {nextPos.X} {nextPos.Y}"); //NEXT tap

            return true;
        }
        public bool PickGmail()
        {
            string uiXml = GetUI();
            if (!uiXml.Contains("text=\"Choose your Gmail address Pick a Gmail address or create your own\""))
            {
                return false;
            }

            (int X, int Y) pickPos = getTapFromUI(uiXml, "selectionc0"); //pick 1st gmail
            (int X, int Y) createPos = getTapFromUI(uiXml, "Create your own Gmail address"); //create new gmail
            adb.Execute(deviceId, $"input tap {pickPos.X} {pickPos.Y}");

            if (pickPos.X == createPos.X & pickPos.Y == createPos.Y)
            {
                uiXml = GetUI();
                (int X, int Y) inputPos = getTapFromUI(uiXml, "android.widget.EditText");
                adb.Execute(deviceId, $"input tap {inputPos.X} {inputPos.Y}");

                int num = rnd.Next();
                mailAddress = firstName.ToLower() + lastName.ToLower() + num.ToString();
                QwetyInput(mailAddress);
            }

            adb.Execute(deviceId, "input keyevent KEYCODE_BACK");
            (int X, int Y) nextPos = getTapFromUI(uiXml, "NEXT");
            adb.Execute(deviceId, $"input tap {nextPos.X} {nextPos.Y}");

            return true;
        }
        public bool PasswordInput(string password)
        {
            string uiXml = GetUI();
            if (!uiXml.Contains("text=\"Create a strong password\""))
            {
                return false;
            }

            (int X, int Y) passwordPos = getTapFromUI(uiXml, "password");
            adb.Execute(deviceId, $"input tap {passwordPos.X} {passwordPos.Y}");
            QwetyInput(password);
            adb.Execute(deviceId, "input keyevent KEYCODE_BACK");

            (int X, int Y) nextPos = getTapFromUI(uiXml, "NEXT");
            adb.Execute(deviceId, $"input tap {nextPos.X} {nextPos.Y}");

            return true;
        }
        public bool AddPhone()
        {
            string uiXml = GetUI();
            if (!uiXml.Contains("text=\"Add phone number?\""))
            {
                return false;
            }

            adb.Execute(deviceId, $"input swipe 500 1700 100 100");
            adb.Execute(deviceId, $"input tap 115 1788"); //Skip

            return true;
        }
        public bool Review()
        {
            string uiXml = GetUI();
            if (!uiXml.Contains("text=\"Review your account info\""))
            {
                return false;
            }

            //get email address
            string pattern = @"\w+@gmail.com";
            Regex rx = new(pattern);
            Match match = rx.Match(uiXml);
            mailAddress = match.Groups[0].Value;

            (int X, int Y) nextPos = getTapFromUI(uiXml, "NEXT");
            adb.Execute(deviceId, $"input tap {nextPos.X} {nextPos.Y}");

            return true;
        }

        public bool Privacy()
        {
            string uiXml = GetUI();
            if (!uiXml.Contains("text=\"Privacy and Terms\""))
            {
                return false;
            }

            adb.Execute(deviceId, $"input swipe 500 1700 100 100");
            adb.Execute(deviceId, $"input swipe 500 1700 100 100");
            adb.Execute(deviceId, $"input tap 867 1788"); //I agree

            return true;
        }

        public bool AmazonOpen()
        {
            adb.Execute(deviceId, "am force-stop com.amazon.venezia");
            adb.Execute(deviceId, "am start -n com.amazon.venezia/com.amazon.venezia.Launcher");

            bool check;
            string uiXml;
            do
            {
                adb.Sleep(2000); //wait for Amazon Registration load
                uiXml = GetUI();
                check = uiXml.Contains("Welcome to the Amazon Appstore");
            } while (!check);

            (int X, int Y) createPos = getTapFromUI(uiXml, "Create a new Amazon account");
            adb.Execute(deviceId, $"input tap {createPos.X} {createPos.Y}");

            return true;
        }

        public bool AmazonRegister(string password)
        {
            string uiXml = GetUI();
            if (!uiXml.Contains("Create account"))
            {
                return false;
            }

            (int X, int Y) namePos = getTapFromUI(uiXml, "ap_customer_name");
            adb.Execute(deviceId, $"input tap {namePos.X} {namePos.Y}");
            QwetyInput(firstName + " " + lastName);
            adb.Execute(deviceId, "input keyevent KEYCODE_BACK");

            (int X, int Y) emailPos = getTapFromUI(uiXml, "ap_email");
            adb.Execute(deviceId, $"input tap {emailPos.X} {emailPos.Y}");
            QwetyInput(mailAddress);
            adb.Execute(deviceId, "input keyevent KEYCODE_BACK");

            (int X, int Y) passPos = getTapFromUI(uiXml, "ap_email");
            adb.Execute(deviceId, $"input tap {passPos.X} {passPos.Y}");
            QwetyInput(password);
            adb.Execute(deviceId, "input keyevent KEYCODE_BACK");

            (int X, int Y) createPos = getTapFromUI(uiXml, "Create your Amazon account");
            adb.Execute(deviceId, $"input tap {createPos.X} {createPos.Y}");

            return true;
        }
    }
}
