using Ini;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VoliBot.Utils;

namespace VoliBot
{
    public partial class WelcomeWindow : Form
    {
        public VoliBot _parent;
        string specificFolder;
        bool _forConfig ;
        public WelcomeWindow(VoliBot parent, bool isOnlyForConfig = false)
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            specificFolder = Path.Combine(folder, "VoliBot");
            _parent = parent;
            _forConfig = isOnlyForConfig;
            InitializeComponent();
        }
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams parms = base.CreateParams;
                if (!_forConfig)
                {
                    parms.ClassStyle |= 0x200;
                }
                return parms;
            }
        }

        private void WelcomeWindow_Load(object sender, EventArgs e)
        {

            // Regions
            foreach (string region in Enums.regions)
            {
                comboBox1.Items.Add(region);
            }
            comboBox1.SelectedIndex = 0;
            // Spells
            foreach (string spell in Enums.spells)
            {
                comboBox2.Items.Add(spell);
                comboBox3.Items.Add(spell);
            }
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
            // Queues
            foreach (string queue in Enums.queues)
            {
                comboBox4.Items.Add(queue);
            }
            comboBox4.SelectedIndex = 0;
            // Champions
            comboBox5.Items.Add((object)"RANDOM");
            foreach (string champions in Enums.champions)
            {
                comboBox5.Items.Add(Basic.UppercaseFirst(champions));
            }
            comboBox5.SelectedIndex = 0;

            if (_forConfig)
            {
                button6.Text = "Close";
                tabControl1.SelectTab(1);
                label4.Text = "Global Settings";
                button6.Click += button6_alternate;
                textBox1.Text = Config.defaultPath;
                Text = "Global / Default Configuration";
                richTextBox2.Text = "Need to change settings, mh? Configurate here the default settings for new bots.";

                for (int i = 0; i < comboBox1.Items.Count; i++)
                {
                    if (comboBox1.GetItemText(comboBox1.Items[i]) == Config.defaultRegion)
                    {
                        comboBox1.SelectedIndex = i;
                    }
                }
                for (int i = 0; i < comboBox2.Items.Count; i++)
                {
                    if (comboBox2.GetItemText(comboBox2.Items[i]) == Config.defaultSlotOne)
                    {
                        comboBox2.SelectedIndex = i;
                    }
                }
                for (int i = 0; i < comboBox3.Items.Count; i++)
                {
                    if (comboBox3.GetItemText(comboBox3.Items[i]) == Config.defaultSlotTwo)
                    {
                        comboBox3.SelectedIndex = i;
                    }
                }
                for (int i = 0; i < comboBox4.Items.Count; i++)
                {
                    if (comboBox4.GetItemText(comboBox4.Items[i]) == Config.defaultQueue)
                    {
                        comboBox4.SelectedIndex = i;
                    }
                }
                for (int i = 0; i < comboBox5.Items.Count; i++)
                {
                    if (comboBox5.GetItemText(comboBox5.Items[i]) == Config.defaultChampion)
                    {
                        comboBox5.SelectedIndex = i;
                    }
                }
            }
        }
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.facebook.com/volibot/");
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Thanks for your interesst in our Twitter-Network. But it currently doesn't exist.");
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Thanks for your interesst in our Videos. But they currently doesn't exist.");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!File.Exists(specificFolder + "\\config.ini"))
            {
                MessageBox.Show(specificFolder + "\\config.ini");
                tabControl1.SelectTab(1);
            }
            else
            {
                _parent.accpetedAgreement();
                this.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("To use VoliBot, you have to accept. :/");
            Application.Exit();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Wow! Thanks!\nBut please Note: If you only want to donate less than 0,50€ please keep it, I'll get 0,00€ because of the PayPal fees, still thank you!");
            Process.Start("http://volibot.com/#our-team");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (_forConfig)
            {
                textBox1.Text = Config.defaultPath;
                for (int i = 0; i < comboBox1.Items.Count; i++)
                {
                    if (comboBox1.GetItemText(comboBox1.Items[i]) == Config.defaultRegion)
                    {
                        comboBox1.SelectedIndex = i;
                    }
                }
                for (int i = 0; i < comboBox2.Items.Count; i++)
                {
                    if (comboBox2.GetItemText(comboBox2.Items[i]) == Config.defaultSlotOne)
                    {
                        comboBox2.SelectedIndex = i;
                    }
                }
                for (int i = 0; i < comboBox3.Items.Count; i++)
                {
                    if (comboBox3.GetItemText(comboBox3.Items[i]) == Config.defaultSlotTwo)
                    {
                        comboBox3.SelectedIndex = i;
                    }
                }
                for (int i = 0; i < comboBox4.Items.Count; i++)
                {
                    if (comboBox4.GetItemText(comboBox4.Items[i]) == Config.defaultQueue)
                    {
                        comboBox4.SelectedIndex = i;
                    }
                }
                for (int i = 0; i < comboBox5.Items.Count; i++)
                {
                    if (comboBox5.GetItemText(comboBox5.Items[i]) == Config.defaultChampion)
                    {
                        comboBox5.SelectedIndex = i;
                    }
                }
            }
            else
            {
                textBox1.Text = "";
                comboBox1.SelectedIndex = 0;
                comboBox2.SelectedIndex = 0;
                comboBox3.SelectedIndex = 0;
                comboBox4.SelectedIndex = 0;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(0);
        }

        private void button6_alternate(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string file = specificFolder + "\\config.ini";

            if (!Directory.Exists(specificFolder))
                Directory.CreateDirectory(specificFolder);

            if (!File.Exists(file))
            {
                using (StreamWriter sw = File.CreateText(file))
                {
                    sw.Write("[General]" + Environment.NewLine + "LauncherPath=" + textBox1.Text + Environment.NewLine + "DefaultRegion=" + comboBox1.SelectedItem.ToString() + Environment.NewLine + "DefaultSpell1=" + comboBox2.SelectedItem.ToString() + Environment.NewLine + "DefaultSpell2=" + comboBox3.SelectedItem.ToString() + Environment.NewLine + "DefaultQueue=" + comboBox4.SelectedItem.ToString() + Environment.NewLine + "DefaultChampion=" + comboBox5.SelectedItem.ToString());
                }
            }
            else
            {
                using (StreamWriter sw = File.CreateText(file))
                {
                    sw.Write("[General]" + Environment.NewLine + "LauncherPath=" + textBox1.Text + Environment.NewLine + "DefaultRegion=" + comboBox1.SelectedItem.ToString() + Environment.NewLine + "DefaultSpell1=" + comboBox2.SelectedItem.ToString() + Environment.NewLine + "DefaultSpell2=" + comboBox3.SelectedItem.ToString() + Environment.NewLine + "DefaultQueue=" + comboBox4.SelectedItem.ToString() + Environment.NewLine + "DefaultChampion=" + comboBox5.SelectedItem.ToString());
                }
            }

            Config.defaultPath = textBox1.Text;
            Config.defaultRegion = comboBox1.SelectedItem.ToString();
            Config.defaultSlotOne = comboBox2.SelectedItem.ToString();
            Config.defaultSlotTwo = comboBox3.SelectedItem.ToString();
            Config.defaultQueue = comboBox4.SelectedItem.ToString();
            Config.defaultChampion = comboBox5.SelectedItem.ToString();


            if (_forConfig)
            {
                MessageBox.Show(Config.defaultPath);
            }
            else
            {
                this.Close();
            }

            _parent.accpetedAgreement();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog opd = new OpenFileDialog();
            opd.Filter = "lol.launcher.exe|lol.launcher.exe";
            String strfilename = "";
            if (opd.ShowDialog() == DialogResult.OK) // Test result.
            {
                strfilename = opd.FileName;
            }
            else
            {
                return;
            }
            textBox1.Text = Path.GetDirectoryName(strfilename) + "\\"; // <-- For debugging use.
        }
    }
}
