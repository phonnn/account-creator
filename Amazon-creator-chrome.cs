using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Net.Http;

namespace Amazon_console
{
    internal partial class Creator
    {
        public bool OpenChrome()
        {
            adb.Execute(deviceId, "pm clear com.android.chrome");
            adb.Execute(deviceId, "am start -n com.android.chrome/com.google.android.apps.chrome.Main -d https://www.amazon.com");
            if (CheckUI("Accept &amp; continue"))
            {
                (int X, int Y) acceptPos = getTapFromUI(uiXml, "Accept &amp; continue");
                adb.Execute(deviceId, $"input tap {acceptPos.X} {acceptPos.Y}");
            } 
            else
            {
                return true;
            }

            adb.Sleep(1000);
            if (!CheckUI("Turn on sync?")) return false;

            (int X, int Y) syncPos;
            try
            {
                syncPos = getTapFromUI(uiXml, "Yes, I'm in");
            } catch 
            {
                syncPos = getTapFromUI(uiXml, "No thanks");
            }
            adb.Execute(deviceId, $"input tap {syncPos.X} {syncPos.Y}");

            adb.Sleep(3000);
            adb.Execute(deviceId, "input tap 750 310");

            return true;
        }

        public bool AmazonSigninPage()
        {
            //Console.WriteLine("open");

            //if (CheckUI("Search or type web address"))
            //{
            //    (int X, int Y) searchPos = getTapFromUI(uiXml, "Search or type web address");
            //    adb.Execute(deviceId, $"input tap {searchPos.X} {searchPos.Y}");
            //    adb.Execute(deviceId, "input text \"www.amazon.com\"");
            //    adb.Execute(deviceId, "input keyevent 66");
            //    adb.Sleep(2000);
            //    adb.Execute(deviceId, "input tap 750 310");
            //}

            if (!CheckUI("Create account. New to Amazon?")) return false;
            (int X, int Y) createPos = getTapFromUI(uiXml, "android.widget.RadioButton");
            adb.Execute(deviceId, $"input tap {createPos.X} {createPos.Y}");

            return true;
        }

        public bool AmazonRegisterChrome()
        {
            adb.Execute(deviceId, "input tap 540 823");
            adb.Execute(deviceId, $"input text \"{firstName}%s{lastName}\"");
            adb.Execute(deviceId, "input keyevent KEYCODE_BACK");

            adb.Execute(deviceId, "input tap 540 1080");
            adb.Execute(deviceId, $"input text \"{mailAddress}\"");
            adb.Execute(deviceId, "input keyevent KEYCODE_BACK");

            adb.Execute(deviceId, "input tap 540 1330");
            adb.Execute(deviceId, $"input text \"anhminh123\"");
            adb.Execute(deviceId, "input keyevent 66");

            //wait for verify
            do
            {
                adb.Sleep(2000);
                uiXml = GetUI();
            } while (uiXml.Contains("Verify email address") | uiXml.Contains("Create amazon accounts"));

            return true;
        }

    }

}
//