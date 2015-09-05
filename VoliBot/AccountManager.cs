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
    public partial class AccountManager : Form
    {
        public static VoliBot _parent;
        public string specificFolder;
        public List<AccountInBox> accountsInBox = new List<AccountInBox>();
        public AccountManager(VoliBot parent)
        {
            _parent = parent;
            InitializeComponent(); 
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            specificFolder = Path.Combine(folder, "VoliBot");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _parent.addMdiChild(textBox1.Text, textBox2.Text, comboBox1.Text);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            _parent.addMdiChild(textBox1.Text, textBox2.Text, comboBox1.Text);
        }

        public void addAccount(string username, string password, string region)
        {
            AccountInBox newAccountForBox = new AccountInBox(username, password, region);
            accountsInBox.Add(newAccountForBox);
            string temporaryPassword = "";
            for(int i = 0; i < password.Length; i++){
                temporaryPassword = temporaryPassword + "•";
            } 
            using (StreamWriter newTask = new StreamWriter(specificFolder + "\\accounts.txt", true))
            {
                newTask.WriteLine(username + "|" + password + "|" + region);
            }
            //MessageBox.Show(username + "|" + temporaryPassword + "|" + region);
            listBox1.Items.Add(username + "|" + temporaryPassword + "|" + region);
        }

        private void AccountManager_Load(object sender, EventArgs e)
        {
            ImageList ilButtonAdd = new ImageList();
            ilButtonAdd.Images.Add(Properties.Resources.add);
            ilButtonAdd.ImageSize = new Size(16, 16);
            button2.ImageList = ilButtonAdd;
            button2.ImageIndex = 0;

            ImageList ilButtonDelete = new ImageList();
            ilButtonDelete.Images.Add(Properties.Resources.adelete);
            ilButtonDelete.ImageSize = new Size(16, 16);
            button3.ImageList = ilButtonDelete;
            button3.ImageIndex = 0;
            if (File.Exists(specificFolder + "\\accounts.txt"))
            {
                TextReader tr = File.OpenText(specificFolder + "\\accounts.txt");
                string line;
                while ((line = tr.ReadLine()) != null)
                {
                    string[] lineSeperated = line.Split('|');
                    AccountInBox aib = new AccountInBox(lineSeperated[0], lineSeperated[1], lineSeperated[2]);
                    accountsInBox.Add(aib);
                    string temporaryPassword = "";
                    for (int i = 0; i < lineSeperated[1].Length; i++)
                    {
                        temporaryPassword = temporaryPassword + "•";
                    }
                    listBox1.Items.Add(lineSeperated[0] + "|" + temporaryPassword + "|" + lineSeperated[2]);
                }
                tr.Close();
            }
            foreach (string region in Enums.regions)
            {
                comboBox1.Items.Add(region);
            }
            comboBox1.SelectedIndex = 1;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AccountManager_ADD AddAnAccountForm = new AccountManager_ADD(this);
            AddAnAccountForm.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var selectedItems = listBox1.SelectedItems;
            for (int i = selectedItems.Count - 1; i >= 0; i--)
            {
                string itemText = selectedItems[i].ToString();
                string[] itemSeperator = itemText.Split('|');

                string username = itemSeperator[0];
                int passwordLength = itemSeperator[1].Length;
                string region = itemSeperator[2];
                AccountInBox toDelete = accountsInBox.FirstOrDefault<AccountInBox>(u => u._username == username && u._region == region && u._password.Length == passwordLength);
                accountsInBox.Remove(toDelete);
                listBox1.Items.Remove(selectedItems[i]);
            }
            using (StreamWriter newTask = new StreamWriter(specificFolder + "\\accounts.txt", false))
            {
                foreach (AccountInBox toSave in accountsInBox)
                {
                    newTask.WriteLine(toSave._username + "|" + toSave._password + "|" + toSave._region);
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var selectedItems = listBox1.SelectedItems;
            for (int i = selectedItems.Count - 1; i >= 0; i--)
            {
                string itemText = selectedItems[i].ToString();
                string[] itemSeperator = itemText.Split('|');

                string username = itemSeperator[0];
                int passwordLength = itemSeperator[1].Length;
                string region = itemSeperator[2];

                AccountInBox toSelect = accountsInBox.FirstOrDefault<AccountInBox>(u => u._username == username && u._region == region && u._password.Length == passwordLength);
                if (!checkBox1.Checked)
                {
                    _parent.addMdiChild(toSelect._username, toSelect._password, toSelect._region, false);
                }
                else
                {
                    _parent.addMdiChild(toSelect._username, toSelect._password, toSelect._region, true);
                }
            }
            if (selectedItems.Count > 1)
            {
                this.Close();
            }
        }
    }

    public class AccountInBox
    {
        public string _username;
        public string _password;
        public string _region;

        public AccountInBox(string username, string password, string region)
        {
            _username = username;
            _password = password;
            _region = region;
        }
    }
}
