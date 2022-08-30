using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Amazon_console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string adbPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + "\\adb.exe";
            ADBClient adb = new();
            adb.AdbPath = adbPath;

            List<string> devices = adb.Devices();
            foreach (var deviceStr in devices)
            {
                bool createdCheck = false;
                bool recoveryCheck = false;
                bool chromeCheck = false;
                bool amazonCheck = false;
                bool amazonLoginCheck = false;
                string deviceId = deviceStr.Split("\t")[0];
                Creator creator = new(deviceId);
                Console.WriteLine(creator.DeviceId);

                do
                {
                    creator.WifiCheck();
                    creator.ScreenOn();
                    creator.Unlock();
                    bool openCheck = creator.OpenMailRegister();
                    if (!openCheck) continue;

                    try
                    {
                        bool nameCheck = creator.NameInput();
                        if (!nameCheck) continue;
                    }
                    catch
                    {
                        continue;
                    }

                    bool genderCheck = creator.BirthdayAndGender();
                    if (!genderCheck) continue;

                    bool addressCheck = creator.PickGmail();
                    if (!addressCheck) continue;

                    bool passCheck = creator.PasswordInput("Anhminh123");
                    if (!passCheck) continue;

                    bool phoneCheck = creator.AddPhone();
                    if (!phoneCheck) continue;

                    bool reviewCheck = creator.Review();
                    if (!reviewCheck) continue;

                    bool privacyCheck = creator.Privacy();
                    if (!privacyCheck) continue;

                    createdCheck = creator.LastStep();
                } while (!createdCheck);

                adb.Sleep(1000);
                do
                {
                    creator.ScreenOn();
                    creator.Unlock();
                    recoveryCheck = creator.OpenGmail();

                    //bool openCheck = creator.OpenGmail();
                    //if (!openCheck) continue;

                    //bool accountCheck = creator.OpenGoogleAccount();
                    //if (!accountCheck) continue;

                    //bool securityCheck = creator.OpenSecurity();
                    //if (!securityCheck) continue;

                    //bool addSecurityCheck = creator.AddSecurity();
                    //if (!addSecurityCheck) continue;

                    //bool inputCheck = creator.InputRecoveryMail();
                    //if (!inputCheck) continue;

                    //recoveryCheck = creator.VerifyRecovery();
                } while (!recoveryCheck) ;

                adb.Sleep(1000);
                do
                {
                    if (!creator.OpenChrome()) continue;
                    if (!creator.AmazonSigninPage()) continue;

                    amazonCheck = creator.AmazonRegisterChrome();
                } while (!amazonCheck);

                do
                {
                    if (!creator.AmazonOpen(0)) continue;
                    if (!creator.AmazonLogin()) continue;
                    amazonLoginCheck = creator.AmazonMobileCheck();
                } while (!amazonLoginCheck);
            }
        }
    }
}
