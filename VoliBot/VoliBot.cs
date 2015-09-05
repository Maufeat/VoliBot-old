using Ini;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VoliBot.Utils;
using VoliBots;

namespace VoliBot
{
    public partial class VoliBot : Form
    {
        public static List<vClient> mdiChilds = new List<vClient>();
        public static string specificFolder;
        int id = 1;
        public VoliBot()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.DoubleBuffer,true);
            this.SetStyle(ControlStyles.UserPaint ,true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint,true);
            ToolStripSeparator tspleft = new ToolStripSeparator();
            ToolStripSeparator tspright = new ToolStripSeparator();
            tspright.Alignment = ToolStripItemAlignment.Right;
            menuStrip1.Items.Add(tspleft);
            menuStrip1.Items.Add(tspright); 
        }

        private void accountsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AccountManager am = new AccountManager(this);
            am.ShowDialog();
        }
        internal void onlyUpdateListItemStatusAndLevel(exListBoxItem elbi, String newStatus, String newLevel)
        {
            exListBoxItem toUpdate = (exListBoxItem)exListBox1.Items[exListBox1.Items.IndexOf(elbi)];
            toUpdate.Details = newStatus; toUpdate.Level = newLevel;
            exListBox1.Refresh();
        }
        internal void updateListItem(exListBoxItem elbi, String newName, String newStatus, String newLevel, Image newImage)
        {
            exListBoxItem toUpdate = (exListBoxItem)exListBox1.Items[exListBox1.Items.IndexOf(elbi)];
            toUpdate.Title = newName; toUpdate.Details = newStatus; toUpdate.Level = newLevel; toUpdate.ItemImage = newImage;
            exListBox1.Refresh();
        }
        internal void updateListItemImage(exListBoxItem elbi, Image newImage)
        {
            exListBoxItem toUpdate = (exListBoxItem)exListBox1.Items[exListBox1.Items.IndexOf(elbi)];
            toUpdate.ItemImage = newImage;
            exListBox1.Refresh();
        }
        public void addMdiChild(String username, String password, String region, bool autoConnect = false)
        {
            if(mdiChilds.Count<vClient>(u => u._username == username) > 0)
            {
                MessageBox.Show("This Client already exist.");
                return;
            }
            Image summonerIcon = Basic.returnIcon(0);
            vClient t = new vClient(username, password, region, this, autoConnect);
            exListBoxItem eLBI = new exListBoxItem(t._username, t._username, "Status: Waiting...", "", summonerIcon);
            t.addListBoxItem(eLBI);
            exListBox1.Items.Add(eLBI);
            mdiChilds.Add(t);
            t.MdiParent = this;
            t.Show();
            id++;
        }
        internal void removeMdiChild(String username, exListBoxItem elbi)
        {
            vClient toRemove = mdiChilds.FirstOrDefault<vClient>(u => u._username == username);
            try {
                exListBox1.Items.Remove(elbi);
            } catch (Exception e)
            {
                MessageBox.Show("Error on remove Child. Please contact me with the Message blow!\n\n" + e.Message);
            }
            mdiChilds.Remove(toRemove);
        }

        private void VoliBot_Load(object sender, EventArgs e)
        {
            WelcomeWindow wwForm = new WelcomeWindow(this);
            wwForm.MdiParent = this;
            wwForm.Show();
        }
        private void LoadConfigs()
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            specificFolder = Path.Combine(folder, "VoliBot");

            if (File.Exists(specificFolder + "\\config.ini"))
            {
                IniFile iniFile = new IniFile(specificFolder + "\\config.ini");
                Config.defaultPath = iniFile.IniReadValue("General", "LauncherPath");
                Config.defaultRegion = iniFile.IniReadValue("General", "DefaultRegion");
                Config.defaultSlotOne = iniFile.IniReadValue("General", "DefaultSpell1");
                Config.defaultSlotTwo = iniFile.IniReadValue("General", "DefaultSpell2");
                Config.defaultQueue = iniFile.IniReadValue("General", "DefaultQueue");
                Config.defaultChampion = iniFile.IniReadValue("General", "DefaultChampion");
            }
        }
        protected override void OnLoad(EventArgs e)
        {
            foreach (Control ctl in this.Controls)
            {
                if (ctl is MdiClient)
                {
                    this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
                    ctl.BackgroundImage = Properties.Resources.dayum;
                    break;
                }
            }
            base.OnLoad(e);
        }
        public void accpetedAgreement()
        {
            accountsToolStripMenuItem.Enabled = true;
            startVoliBotOLDToolStripMenuItem.Enabled = true;
            championsToolStripMenuItem.Enabled = true;
        }

        private static Image GetImageFromURL(string url)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            HttpWebResponse httpWebReponse = (HttpWebResponse)httpWebRequest.GetResponse();
            Stream stream = httpWebReponse.GetResponseStream();
            return Image.FromStream(stream);
        }

        private void exListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void exListBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                int index = exListBox1.IndexFromPoint(e.Location);
                if (index != ListBox.NoMatches)
                {
                    exListBoxItem ix = (exListBoxItem)exListBox1.Items[index];
                    vClient client = mdiChilds.FirstOrDefault<vClient>(u => u._username == ix.Id);
                    if (client.WindowState == FormWindowState.Maximized)
                    {
                        client.WindowState = FormWindowState.Normal;
                    }
                    else
                    {
                        client.WindowState = FormWindowState.Maximized;
                    }
                }
            }
            catch (Exception) { }
        }

        private void donateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("http://volibot.com/#our-team");
        }

        private void VoliBot_FormClosed(object sender, FormClosedEventArgs e)
        {
            foreach(vClient toClose in mdiChilds){
                try
                {
                    toClose.killContainedLeague();
                } catch (Exception){}
            }
        }

        private void championsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WelcomeWindow wwForm = new WelcomeWindow(this, true);
            wwForm.MdiParent = this;
            wwForm.Show();
        }

        private void groupsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Comming Soon™\n Donation's will help tho :p");
        }

        private void startVoliBotOLDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            old_volibot ovForm = new old_volibot(this);
            ovForm.MdiParent = this;
            ovForm.Show();
        }
    }
}
