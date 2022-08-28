using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Net.Http;

namespace Amazon_console
{
    internal partial class Creator
    {
        //action = 0 => login
        //action = 1 => register
        public bool AmazonOpen(int action)
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

            if(action == 0)
            {
                (int X, int Y) createPos = getTapFromUI(uiXml, @"Already an Amazon customer\? Sign In");
                adb.Execute(deviceId, $"input tap {createPos.X} {createPos.Y}");
            }

            if(action == 1)
            {
                (int X, int Y) createPos = getTapFromUI(uiXml, "Create a new Amazon account");
                adb.Execute(deviceId, $"input tap {createPos.X} {createPos.Y}");
            }

            return true;
        }

        public bool AmazonRegister()
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

            (int X, int Y) passPos = getTapFromUI(uiXml, "ap_password");
            adb.Execute(deviceId, $"input tap {passPos.X} {passPos.Y}");
            QwetyInput(mailPassword);
            adb.Execute(deviceId, "input keyevent KEYCODE_BACK");

            (int X, int Y) createPos = getTapFromUI(uiXml, "Create your Amazon account");
            adb.Execute(deviceId, $"input tap {createPos.X} {createPos.Y}");

            return true;
        }

        public bool AmazonLogin()
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
                check = uiXml.Contains("Sign in");
            } while (!check);

            (int X, int Y) emailPos = getTapFromUI(uiXml, "ap_email");
            adb.Execute(deviceId, $"input tap {emailPos.X} {emailPos.Y}");
            QwetyInput("pfannerstilllindsay18919fk@gmail.com");
            //QwetyInput(mailAddress);
            adb.Execute(deviceId, "input keyevent KEYCODE_BACK");

            (int X, int Y) passPos = getTapFromUI(uiXml, "ap_password");
            adb.Execute(deviceId, $"input tap {passPos.X} {passPos.Y}");
            QwetyInput("kenail123");
            //QwetyInput(mailPassword);
            adb.Execute(deviceId, "input keyevent KEYCODE_BACK");

            (int X, int Y) createPos = getTapFromUI(uiXml, "Sign-In");
            adb.Execute(deviceId, $"input tap {createPos.X} {createPos.Y}");

            return true;
        }

        public bool AmazonMobileCheck()
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
                check = uiXml.Contains("Add mobile number");
            } while (!check);

            (int X, int Y) skipPos = getTapFromUI(uiXml, "Not now");
            adb.Execute(deviceId, $"input tap {skipPos.X} {skipPos.Y}");

            return true;
        }
    }
}
