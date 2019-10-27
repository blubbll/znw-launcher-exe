using Launcher.classes;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using static Launcher.Program;

namespace Launcher
{
    public enum BrowserEmulationVersion
    {
        Default = 0,
        Version7 = 7000,
        Version8 = 8000,
        Version8Standards = 8888,
        Version9 = 9000,
        Version9Standards = 9999,
        Version10 = 10000,
        Version10Standards = 10001,
        Version11 = 11000,
        Version11Edge = 11001
    }

    public partial class Form1 : Form
    {
        public Form1()
        {
            //use latest ie on device
            var appName = System.Diagnostics.Process.GetCurrentProcess().ProcessName + ".exe";
            using (var Key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", true))
                Key.SetValue(appName, 99999, RegistryValueKind.DWord);

            InitializeComponent();
            this.WindowState = FormWindowState.Normal;

            myBrowser();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            const double size = .75;
            this.Height = Convert.ToInt32(Screen.PrimaryScreen.Bounds.Height * size);
            this.Width = Convert.ToInt32(Screen.PrimaryScreen.Bounds.Width * size);
            this.CenterToScreen();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            myBrowser();
        }

        private void myBrowser()
        {
            //Create user dir if not exist

            if (!Directory.Exists(userDir)) Directory.CreateDirectory(userDir);

            //read stored vars
            if (File.Exists(tokenFile))
                token = File.ReadAllText(tokenFile);
            if (File.Exists(gamesFile))
                gamelist = new List<string>(File.ReadAllLines(gamesFile));

            //construct web launch url
            var url = $"https://launcher.znw.gg/?" +
                $"{(token != null ? $"token={token}" : "")}" +
                $"{(gamelist.Count != 0 ? $"&games={String.Join(",", gamelist)}" : "")}";
            //Connect to Webapp to show the GUI
            webBrowser1.Navigate(url);

            //Events
            webBrowser1.DocumentTitleChanged += new EventHandler(webpage_DocumentTitleChanged);
            webBrowser1.StatusTextChanged += new EventHandler(webpage_StatusTextChanged);
            webBrowser1.Navigated += new WebBrowserNavigatedEventHandler(webpage_Navigated);
            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webpage_DocumentCompleted);
        }

        private void webpage_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
        }

        private DateTime lastEventUpdate = DateTime.Now;

        private void webpage_DocumentTitleChanged(object sender, EventArgs e)
        {
            //cooldown, stupid ie...
            if (lastEventUpdate.AddSeconds(5) < DateTime.Now)
            {
                lastEventUpdate = DateTime.Now;

                //get title
                var title = webBrowser1.DocumentTitle.ToString();
                //title change was event
                if (title.StartsWith("<") && title.EndsWith(">"))
                {
                    //get event and args
                    var @event = title.Split(new string[] { "<EVENT-" }, StringSplitOptions.None)[1].Split(';')[0];
                    var args = title.Split(';')[1].Split('>')[0];

                    switch (@event)
                    {
                        case "TOKEN":
                            {
                                File.WriteAllText(Program.tokenFile, args);
                            }
                            break;

                        case "GAME_ADD":
                            {
                                games.install(args);
                                gamelist.Add(args);
                                File.WriteAllLines(Program.gamesFile, gamelist);
                            }
                            break;

                        case "GAME_START":
                            {
                                gamelist.Add(args);
                                File.WriteAllLines(Program.gamesFile, gamelist);
                            }
                            break;

                        case "GAME_REMOVE":
                            {
                                gamelist.Remove(args);
                                File.WriteAllLines(Program.gamesFile, gamelist);
                            }
                            break;

                        default:
                            {
                                MessageBox.Show($"Event: {@event}, \nArgs: {args}");
                            }
                            break;
                    }
                }
            }
        }

        private void webpage_StatusTextChanged(object sender, EventArgs e)
        {
        }

        private void webpage_ProgressChanged(object sender, WebBrowserProgressChangedEventArgs e)
        {
        }

        private void webpage_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            webBrowser1.Refresh();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            webBrowser1.GoForward();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            webBrowser1.GoBack();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            webBrowser1.GoHome();
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            webBrowser1.ShowPrintPreviewDialog();
        }
    }
}