using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using LoLLauncher;

namespace VoliBot
{
    public abstract class BaseRegion
    {
        public abstract string RegionName { get; }

        public abstract bool Garena { get; }

        public abstract string InternalName { get; }

        public abstract string ChatName { get; }

        public abstract Uri NewsAddress { get; }

        public abstract string Locale { get; }

        public abstract RegioN PVPRegion { get; }

        public abstract IPAddress[] PingAddresses { get; }

        public abstract Uri SpectatorLink { get; }

        //ONLY SET FOR CS SERVER
        public abstract string SpectatorIpAddress { get; set; }

        public abstract string Location { get; }

        public static BaseRegion GetRegion(String requestedRegion)
        {
            requestedRegion = requestedRegion.ToUpper();
            BaseRegion toReturn;
            switch (requestedRegion)
            {
                case "BR":
                    toReturn = new BaseRegions.BR();
                    return toReturn;
                case "EUW":
                    toReturn = new BaseRegions.EUW();
                    return toReturn;
                case "EUNE":
                    toReturn = new BaseRegions.EUNE();
                    return toReturn;
                case "KR":
                    toReturn = new BaseRegions.KR();
                    return toReturn;
                case "LAN":
                    toReturn = new BaseRegions.LAN();
                    return toReturn;
                case "LAS":
                    toReturn = new BaseRegions.LAS();
                    return toReturn;
                case "NA":
                    toReturn = new BaseRegions.NA();
                    return toReturn;
                case "OCE":
                    toReturn = new BaseRegions.OCE();
                    return toReturn;
                case "RU":
                    toReturn = new BaseRegions.RU();
                    return toReturn;
                case "TR":
                    toReturn = new BaseRegions.TR();
                    return toReturn;
                default:
                    return null;
            }
        }
    }
    public static class RichTextBoxExtensions
    {
        public static void AppendText(this RichTextBox box, string text, Color color)
        {
            try {
                if (box.InvokeRequired)
                    box.Invoke((Action)(() => AppendText(box, text, color)));
                else
                {
                    box.SelectionStart = box.TextLength;
                    box.SelectionLength = 0;

                    box.SelectionColor = color;
                    box.AppendText(text);
                    box.SelectionColor = box.ForeColor;
                }
            }
            catch (Exception)
            {
                //MessageBox.Show(e.Message);
            }
        }
    }
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new VoliBot());
        }
    }
}
