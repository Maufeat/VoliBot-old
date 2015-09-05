using LoLLauncher;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VoliBot.Utils;

namespace VoliBot
{
    public partial class AccountManager_ADD : Form
    {

        protected AccountManager _parent;
        private LoLConnection _connection = new LoLConnection();
        private AccountManager_TEST _testDialog;

        public AccountManager_ADD(AccountManager paps)
        {
            _parent = paps;
            InitializeComponent();
        }

        private void AccountManager_ADD_Load(object sender, EventArgs e)
        {
            foreach (string region in Enums.regions)
            {
                comboBox1.Items.Add((object)region);
                if (region == Config.defaultRegion)
                {
                    comboBox1.SelectedItem = (object)region;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _parent.addAccount(textBox1.Text, textBox2.Text, comboBox1.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _testDialog = new AccountManager_TEST(textBox1.Text, textBox2.Text, comboBox1.Text);
            _testDialog.ShowDialog();
        }
    }
}
