using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Net.Http;

namespace Amazon_console
{
    internal partial class Creator
    {
        public bool OpenChrome()
        {
            adb.Execute(deviceId, "am force-stop com.android.chrome");
            adb.Execute(deviceId, "am start -n com.android.chrome/com.google.android.apps.chrome.Main");
            bool check;
            string uiXml;
            int attempt = 0;
            do
            {
                if (attempt > 3) break;
                attempt += 1;
                adb.Sleep(2000);
                uiXml = GetUI();
                check = uiXml.Contains("Accept &amp; continue");
                if (check)
                {
                    (int X, int Y) acceptPos = getTapFromUI(uiXml, "Accept &amp; continue");
                    adb.Execute(deviceId, $"input tap {acceptPos.X} {acceptPos.Y}");
                }
            } while (!check);

            adb.Sleep(1000);
            uiXml = GetUI();
            if (!uiXml.Contains("Turn on sync?"))
            {
                return false;
            }

            (int X, int Y) syncPos = getTapFromUI(uiXml, "Yes, I'm in");
            adb.Execute(deviceId, $"input tap {syncPos.X} {syncPos.Y}");

            return true;
        }

        public bool AmazonSignin()
        {
            bool check;
            string uiXml;
            int attempt = 0;

            do
            {
                if (attempt > 3) return false;
                attempt++;
                adb.Sleep(2000);
                uiXml = GetUI();
                check = uiXml.Contains("Search or type web address");
            } while (!check);

            (int X, int Y) searchPos = getTapFromUI(uiXml, "Search or type web address");
            adb.Execute(deviceId, $"input tap {searchPos.X} {searchPos.Y}");
            QwetyInput("www.amazon.com\n");

            check = false;
            attempt = 0;
            do
            {
                if (attempt > 3) return false;
                attempt++;
                adb.Sleep(2000);
                uiXml = GetUI();
                check = uiXml.Contains("Sign In");
            } while (!check);

            (int X, int Y) signinPos = getTapFromUI(uiXml, "nav-button-avatar");
            adb.Execute(deviceId, $"input tap {signinPos.X} {signinPos.Y}");

            check = false;
            attempt = 0;
            do
            {
                if (attempt > 3) return false;
                attempt++;
                adb.Sleep(2000);
                uiXml = GetUI();
                check = uiXml.Contains("Create account. New to Amazon?");
            } while (!check);

            (int X, int Y) createPos = getTapFromUI(uiXml, "android.widget.RadioButton");
            adb.Execute(deviceId, $"input tap {createPos.X} {createPos.Y}");

            return true;
        }

        public bool AmazonRegisterChrome()
        {
            bool check;
            string uiXml;
            int attempt = 0;

            do
            {
                if (attempt > 3) return false;
                attempt++;
                adb.Sleep(2000);
                uiXml = GetUI();
                check = uiXml.Contains("Welcome");
            } while (!check);

            adb.Execute(deviceId, $"input tap 540 823");
            QwetyInput(firstName + " " + lastName);
            adb.Execute(deviceId, "input keyevent KEYCODE_BACK");

            adb.Execute(deviceId, $"input tap 540 1080");
            QwetyInput(mailAddress);
            adb.Execute(deviceId, "input keyevent KEYCODE_BACK");

            adb.Execute(deviceId, $"input tap 540 1332");
            QwetyInput(mailPassword);
            adb.Execute(deviceId, "input keyevent KEYCODE_BACK");

            adb.Execute(deviceId, $"input tap 540 1176");

            //wait for verify
            do
            {
                adb.Sleep(2000);
                uiXml = GetUI();
                check = uiXml.Contains(firstName) | uiXml.Contains(lastName);
            } while (!check);

            return true;
        }

    }

}
//