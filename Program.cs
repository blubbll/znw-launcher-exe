using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Launcher
{
    static class Program
    {

        //vars
        public static string token = null;
        public static List<string> games = new List<string> { };

        public static string userDir = $"{AppDomain.CurrentDomain.BaseDirectory}/user";

        //filedirs
        public static string tokenFile = $"{userDir}/token";
        public static string gamesFile = $"{userDir}/games";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}