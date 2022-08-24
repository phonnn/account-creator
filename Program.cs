﻿using System;
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
                bool allDone = false;
                string deviceId = deviceStr.Split("\t")[0];
                Creator creator = new(deviceId);
                Console.WriteLine(creator.DeviceId);
                do
                {
                    creator.WifiCheck();
                    creator.ScreenOn();
                    creator.Unlock();
                    creator.OpenMailRegister();
                    bool nameCheck = creator.NameInput();
                    if (!nameCheck) continue;

                    bool genderCheck = creator.BirthdayAndGender();
                    if (!genderCheck) continue;

                    bool addressCheck = creator.PickGmail();
                    if (!addressCheck) continue;

                    bool passCheck = creator.PasswordInput("kenail123");
                    if (!passCheck) continue;

                    bool phoneCheck = creator.AddPhone();
                    if (!phoneCheck) continue;

                    bool reviewCheck = creator.Review();
                    if (!reviewCheck) continue;

                    bool privacyCheck = creator.Privacy();
                    if (privacyCheck)
                    {
                        allDone = true;
                    }
                    else
                    {
                        continue;
                    }
                } while (!allDone);

                //creator.AmazonRegister("Kenail123");

            }
        }
    }
}