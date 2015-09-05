using System;
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
    public partial class old_volibot : Form
    {
        private VoliBot parent;
        public old_volibot(VoliBot _parent)
        {
            parent = _parent;
            InitializeComponent();
        }

        private void old_volibot_Load(object sender, EventArgs e)
        {
            textBox1.Text = Config.clientSeason;
            textBox4.Text = Config.defaultPath;
            foreach(string region in Enums.regions){
                comboBox1.Items.Add(region);
            }
            comboBox1.SelectedIndex = 1;
            foreach (string spell in Enums.spells)
            {
                comboBox2.Items.Add(spell);
                comboBox3.Items.Add(spell);
            }
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 1;
            foreach (string queue in Enums.queues)
            {
                comboBox4.Items.Add(queue);
            }
            comboBox3.SelectedIndex = 4;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog opd = new OpenFileDialog();
            opd.Filter = "accounts.txt|accounts.txt";
            opd.Filter = "Other txt file|*.txt";
            String strfilename = "";
            if (opd.ShowDialog() == DialogResult.OK) // Test result.
            {
                strfilename = opd.FileName;
            }
            else
            {
                return;
            }
            textBox2.Text = strfilename;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            console csle = new console(textBox1.Text, textBox2.Text, Convert.ToInt32(numericUpDown1.Value), Convert.ToInt32(numericUpDown2.Value), comboBox1.Text, textBox3.Text, comboBox2.Text, comboBox3.Text, checkBox1.Checked, checkBox2.Checked, textBox4.Text, textBox4.SelectedText);
            csle.MdiParent = parent;
            csle.Show();
        }

        private void button4_Click(object sender, EventArgs e)
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
            textBox4.Text = strfilename;
        }
    }
}
