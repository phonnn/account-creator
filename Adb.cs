using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Amazon_console
{
    public partial class ADBClient : Component
    {
        // ----------------------------------------- Adb.exe path, leave blank if in same directory as app or included in PATH
        private string adbPath = "adb.exe";
        public string AdbPath
        {
            get { return adbPath; }
            set
            {
                if (File.Exists(value)) adbPath = value;
            }
        }

        // ----------------------------------------- Adb command timeout - usable in push and pull to avoid hanging while executing
        private int adbTimeout;
        public int AdbTimeout
        {
            get { return adbTimeout > 0 ? adbTimeout : 5000; }
            set { adbTimeout = value; }
        }

        // ----------------------------------------- Create our emulated shell here and assign events

        // Create a background thread an assign work event to our emulated shell method
        BackgroundWorker CMD = new BackgroundWorker();
        private Process Shell;

        public ADBClient()
        {
            CMD.DoWork += new DoWorkEventHandler(CMD_Send);
        }

        // Needed data types for our emulated shell
        string Command = "";
        bool Complete = false;

        // Create an emulated shell for executing commands
        private void CMD_Send(object sender, DoWorkEventArgs e)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                FileName = "cmd.exe",
                Arguments = "/C \"" + Command + "\""
            };

            using (Process process = Process.Start(startInfo))
            {
                if (Command.StartsWith("\"" + adbPath + "\" logcat"))
                {
                    Complete = true;
                    process.WaitForExit();
                    return;
                }

                if (!process.WaitForExit(AdbTimeout))
                    process.Kill();

                Output = process.StandardOutput.ReadToEnd();
                Complete = true;
            };
        }

        // Send a command to emulated shell
        private void SendCommand(string command)
        {
            CMD.WorkerSupportsCancellation = true;
            Command = command;
            CMD.RunWorkerAsync();
            while (!Complete) Sleep(500);
            Complete = false;
        }

        // Sleep until output
        public void Sleep(int milliseconds)
        {
            DateTime delayTime = DateTime.Now.AddMilliseconds(milliseconds);
            while (DateTime.Now < delayTime)
            {
                Application.DoEvents();
            }
        }

        // ----------------------------------------- Allow public modifiers to get output

        public string Output { get; private set; }

        // ----------------------------------------- Functions
        public List<string> Devices()
        {
            SendCommand("\"" + adbPath + "\" devices");

            string[] outLines = Output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            return outLines.Skip(1).ToList();
        }
        public void Execute(string id, string command, bool asroot = false)
        {
            if (asroot)
            {
                SendCommand("\"" + adbPath + "\" -s " + id + " shell su -c \"" + command + "\"");
            }
            else
            {
                SendCommand("\"" + adbPath + "\" -s " + id + " shell " + command);
                Console.WriteLine("\"" + adbPath + "\" -s " + id + " shell " + command);
            }
        }
    }
}