using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Automation;
using System.Windows.Forms;

namespace ChromeWithProxy
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string ip = textBox1.Text;
            string port = textBox2.Text;
            if (String.IsNullOrEmpty(ip) || string.IsNullOrEmpty(port))
                return;
            const string ChromeAppKey = @"\Software\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe";
            string ChromePath = (string)(Registry.GetValue("HKEY_LOCAL_MACHINE" + ChromeAppKey, "", null) ??
                                Registry.GetValue("HKEY_CURRENT_USER" + ChromeAppKey, "", null));
            if (string.IsNullOrEmpty(ChromePath))
                MessageBox.Show("Could not find Chrome.exe", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            foreach (Process process in Process.GetProcessesByName("chrome"))
            {
                if (process.MainWindowHandle == IntPtr.Zero) // some have no UI
                    continue;

                AutomationElement element = AutomationElement.FromHandle(process.MainWindowHandle);
                if (element != null)
                {
                    ((WindowPattern)element.GetCurrentPattern(WindowPattern.Pattern)).Close();
                }
            }
            List<string> param = new List<string>()
            {                
                " --proxy-server=\"",
                ip,
                ":",
                port,
                "\""
            };
            string parameter = string.Join("", param.ToArray());
            ProcessStartInfo stInfo = new ProcessStartInfo();
            stInfo.FileName = ChromePath;
            stInfo.Arguments = parameter;
            Process.Start(stInfo);
            this.Close();
        }
    }
}
