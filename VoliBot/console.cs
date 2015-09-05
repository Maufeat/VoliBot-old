using LoLLauncher;
using RitoBot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VoliBot.Utils;

namespace VoliBot
{
    public partial class console : Form
    {

        public string currentVersion = "5.16";
        public string accounts_txt_path = "";
        public string lolPath = "";
        public int maxBots = 5;
        public int maxLevel = 10;
        public string region = "EUW";
        public string championToPick = "Annie";
        public string spell1 = "Heal";
        public string spell2 = "Flash";
        public bool randomSpell = false;
        public bool replaceConfig = false;
        public string queueType = "ARAM";
        public bool buyBoost = false;


        public ArrayList accounts = new ArrayList();
        public ArrayList accountsNew = new ArrayList();

        public console(string cV, string atp, int mbots, int mlevel, string rgn, string ctp, string ss1, string ss2, bool rndss, bool rplccfg, string lolpath, string queuetyp)
        {
            currentVersion = cV;
            accounts_txt_path = atp;
            maxBots = mbots;
            maxLevel = mlevel;
            region = rgn;
            championToPick = ctp;
            spell1 = ss1;
            spell2 = ss2;
            lolPath = lolpath;
            randomSpell = rndss;
            queueType = queuetyp;
            replaceConfig = rplccfg;

            InitializeComponent();
        }

        private void console_Load(object sender, EventArgs e)
        {
            updateStatus(msgStatus.INFO, "Console initialized.", "CONSOLE");
            if (replaceConfig)
            {
                updateStatus(msgStatus.INFO, "Replacing Config.", "CONSOLE");
                try
                {
                    Basic.ReplaceGameConfig(lolPath);
                }
                catch (Exception) { }
            }
            updateStatus(msgStatus.INFO, "Loading Accounts", "CONSOLE");
            loadAccounts();
            int curRunning = 0;
            foreach (string acc in accounts)
            {
                try
                {
                    accountsNew.RemoveAt(0);
                    string[] stringSeparators = new string[] { "|" };
                    var result = acc.Split(stringSeparators, StringSplitOptions.None);
                    curRunning += 1;
                    if (result[2] != null)
                    {
                        QueueTypes queuetype = (QueueTypes)System.Enum.Parse(typeof(QueueTypes), result[2]);
                        OldVoliBot ovb = new OldVoliBot(result[0], result[1], this, queuetype);
                    }
                    else
                    {
                        QueueTypes queuetype = QueueTypes.ARAM;
                        OldVoliBot ovb = new OldVoliBot(result[0], result[1], this, queuetype);
                    }
                    if (curRunning == maxBots)
                        break;
                }
                catch (Exception ex)
                {
                    updateStatus(msgStatus.ERROR, "CountAccError: You may have an issue in your accounts.txt" + ex.Message, "CONSOLE");
                }
            }
        }
        public void loadAccounts()
        {
            try
            {
                TextReader tr = File.OpenText(accounts_txt_path);
                string line;
                while ((line = tr.ReadLine()) != null)
                {
                    accounts.Add(line);
                    accountsNew.Add(line);
                }
                tr.Close();
            }
            catch (Exception)
            {
                updateStatus(msgStatus.ERROR, "Account-List not found! Check your Configs!", "CONSOLE");
            }
        }
        public void lognNewAccount()
        {
            accountsNew = accounts;
            accounts.RemoveAt(0);
            int curRunning = 0;
            if (accounts.Count == 0)
            {
                updateStatus(msgStatus.INFO, "No more accounts to login.", "CONSOLE");
            }
            foreach (string acc in accounts)
            {
                string Accs = acc;
                string[] stringSeparators = new string[] { "|" };
                var result = Accs.Split(stringSeparators, StringSplitOptions.None);
                curRunning += 1;
                if (result[2] != null)
                {
                    QueueTypes queuetype = (QueueTypes)System.Enum.Parse(typeof(QueueTypes), result[2]);
                    OldVoliBot ovb = new OldVoliBot(result[0], result[1], this, queuetype);
                }
                else
                {
                    QueueTypes queuetype = QueueTypes.ARAM;
                    OldVoliBot ovb = new OldVoliBot(result[0], result[1], this, queuetype);
                }
                if (curRunning == maxBots)
                    break;
            }
        }

        public void updateStatus(msgStatus type, String msg, String _username)
        {
            switch (type)
            {
                case msgStatus.DEBUG:
                    richTextBox1.AppendText("[" + type + "]", Color.Pink);
                    break;
                case msgStatus.ERROR:
                    richTextBox1.AppendText("[" + type + "]", Color.Red);
                    break;
                case msgStatus.INFO:
                    richTextBox1.AppendText("[" + type + "]", Color.Blue);
                    break;
                default:
                    richTextBox1.AppendText("[" + type + "]", Color.Aqua);
                    break;
            }
            richTextBox1.AppendText("[" + DateTime.Now.ToShortTimeString() + "]", Color.Yellow);
            richTextBox1.AppendText(" ", Color.Black);
            richTextBox1.AppendText(_username, Color.Yellow);
            richTextBox1.AppendText(": ", Color.White);
            richTextBox1.AppendText(msg, Color.White);
            richTextBox1.AppendText(Environment.NewLine, Color.Black);
        }
    }
}
