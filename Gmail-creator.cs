using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Net.Http;

namespace Amazon_console
{
    internal partial class Creator
    {
        private string firstName;
        private string lastName;
        private string mailAddress;
        private string mailPassword;

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
            
            if (!CheckUI("text=\"Sign in\" resource-id=\"headingText\"")) return false;
            (int X, int Y) position = getTapFromUI(uiXml, "android.widget.Spinner");
            adb.Execute(deviceId, $"input tap {position.X} {position.Y}");

            if (!CheckUI("For myself")) return false;
            position = getTapFromUI(uiXml, "For myself");
            adb.Execute(deviceId, $"input tap {position.X} {position.Y}");

            return true;
        }

        public bool NameInput()
        {
            if (!CheckUI("text=\"Enter your name\"")) return false;
            if (string.IsNullOrEmpty(firstName) | string.IsNullOrEmpty(lastName))
            {
                (firstName, lastName) = RandomName();
            }

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
            if (!CheckUI("text=\"Enter your birthday and gender\"")) return false;

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
            if (!CheckUI("text=\"Choose your Gmail address Pick a Gmail address or create your own\"")) return false;

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
                adb.Execute(deviceId, "input keyevent KEYCODE_BACK");
            }

            (int X, int Y) nextPos = getTapFromUI(uiXml, "NEXT");
            adb.Execute(deviceId, $"input tap {nextPos.X} {nextPos.Y}");

            return true;
        }
        public bool PasswordInput(string password)
        {
            if (!CheckUI("text=\"Create a strong password\"")) return false;

            (int X, int Y) passwordPos = getTapFromUI(uiXml, "password");
            adb.Execute(deviceId, $"input tap {passwordPos.X} {passwordPos.Y}");
            QwetyInput(password);

            mailPassword = password; //save password

            uiXml = GetUI();
            (int X, int Y) nextPos = getTapFromUI(uiXml, "NEXT");
            adb.Execute(deviceId, $"input tap {nextPos.X} {nextPos.Y}");

            return true;
        }
        public bool AddPhone()
        {
            if (!CheckUI("text=\"Add phone number?\"")) return false;

            adb.Execute(deviceId, "input swipe 500 1700 100 100");
            adb.Execute(deviceId, "input tap 115 1788"); //Skip

            return true;
        }
        public bool Review()
        {
            if (!CheckUI("text=\"Review your account info\"")) return false;

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
            if (!CheckUI("text=\"Privacy and Terms\"")) return false;

            adb.Execute(deviceId, "input swipe 500 1700 100 100");
            adb.Execute(deviceId, "input swipe 500 1700 100 100");
            adb.Execute(deviceId, "input tap 867 1788"); //I agree

            return true;
        }
        
        public bool LastStep()
        {
            if (!CheckUI("Google services")) return false;

            adb.Execute(deviceId, "input swipe 500 1700 100 100");
            adb.Execute(deviceId, "input tap 867 1788"); //Accept

            return true;
        }

        public bool OpenGmail()
        {
            adb.Execute(deviceId, "am force-stop com.google.android.gm");
            adb.Execute(deviceId, "am force-stop com.google.android.gms");
            adb.Execute(deviceId, "am start -n com.google.android.gm/com.google.android.gm.ui.MailActivityGmail");

            if (CheckUI("Search in mail")) return true;
            if (!CheckUI("New in Gmail")) return false;

            (int X, int Y) acceptPos = getTapFromUI(uiXml, "Got it");
            adb.Execute(deviceId, $"input tap {acceptPos.X} {acceptPos.Y}");

            if (!CheckUI(mailAddress)) return false;

            acceptPos = getTapFromUI(uiXml, "TAKE ME TO GMAIL");
            adb.Execute(deviceId, $"input tap {acceptPos.X} {acceptPos.Y}");

            return true;
        }

        public bool OpenGoogleAccount()
        {
            
            if (CheckUI("Google Meet, now in Gmail"))
            {
                (int X, int Y) acceptPos = getTapFromUI(uiXml, "Got it");
                adb.Execute(deviceId, $"input tap {acceptPos.X} {acceptPos.Y}");
            }

            if (!CheckUI("Search in mail")) return false;
            (int X, int Y) ringPos = getTapFromUI(uiXml, "com.google.android.gm:id/og_apd_ring_view");
            adb.Execute(deviceId, $"input tap {ringPos.X} {ringPos.Y}");

            if (!CheckUI("Manage accounts on this device")) return false;
            (int X, int Y) accountPos = getTapFromUI(uiXml, "android.widget.Button");
            adb.Execute(deviceId, $"input tap {accountPos.X} {accountPos.Y}");

            if (!CheckUI("com.google.android.gms:id/skip_button")) 
            {
                (int X, int Y) startedPos = getTapFromUI(uiXml, "com.google.android.gms:id/skip_button");
                adb.Execute(deviceId, $"input tap {startedPos.X} {startedPos.Y}");
            }

            return true;
        }

        public bool OpenSecurity()
        {
            if (!CheckUI("You have security recommendations")) return false;
            (int X, int Y) securityPos = getTapFromUI(uiXml, "You have security recommendations");
            adb.Execute(deviceId, $"input tap {securityPos.X} {securityPos.Y}");

            if (!CheckUI("Security Checkup")) return false;
            adb.Execute(deviceId, "input swipe 500 1700 100 100");

            (int X, int Y) recoveryPos;
            do
            {
                if (!CheckUI("Issues found in sign-in &amp; recovery Add ways to verify it's you")) return false;
                recoveryPos = getTapFromUI(uiXml, "Issues found in sign-in &amp; recovery Add ways to verify it's you");
                adb.Execute(deviceId, $"input tap {recoveryPos.X} {recoveryPos.Y}");
            } while (recoveryPos.X != 0 & recoveryPos.Y != 0);

            if (!CheckUI("Add a recovery email")) return false;
            (int X, int Y) addPos = getTapFromUI(uiXml, "Add a recovery email");
            adb.Execute(deviceId, $"input tap {addPos.X} {addPos.Y}");

            return true;
        }
        
        public bool AddSecurity()
        {
            if (!CheckUI("To continue, first verify")) return false;

            (int X, int Y) passPos = getTapFromUI(uiXml, "android.widget.EditText");
            adb.Execute(deviceId, $"input tap {passPos.X} {passPos.Y}");

            QwetyInput(mailPassword);
            adb.Execute(deviceId, "input keyevent KEYCODE_BACK");

            (int X, int Y) nextPos = getTapFromUI(uiXml, "Next");
            adb.Execute(deviceId, $"input tap {nextPos.X} {nextPos.Y}");

            return true;
        }

        public bool InputRecoveryMail()
        {
            if (!CheckUI("Add recovery email")) return false;

            (int X, int Y) inputPos = getTapFromUI(uiXml, "android.widget.EditText");
            adb.Execute(deviceId, $"input tap {inputPos.X} {inputPos.Y}");

            string recoveryMail = mailAddress.Replace("gmail", "yopmail");
            QwetyInput(recoveryMail);
            adb.Execute(deviceId, "input keyevent KEYCODE_BACK");

            (int X, int Y) nextPos = getTapFromUI(uiXml, "Next");
            adb.Execute(deviceId, $"input tap {nextPos.X} {nextPos.Y}");

            return true;
        }
        public bool VerifyRecovery()
        {
            if (!CheckUI("Verify your recovery email")) return false;

            adb.Execute(deviceId, "input keyevent KEYCODE_BACK");

            (int X, int Y) skipPos = getTapFromUI(uiXml, "Verify later");
            adb.Execute(deviceId, $"input tap {skipPos.X} {skipPos.Y}");

            if (!CheckUI("You can update this anytime")) return false;
            (int X, int Y) donePos = getTapFromUI(uiXml, "Done");
            adb.Execute(deviceId, $"input tap {donePos.X} {donePos.Y}");

            return true;
        }
    }
}
