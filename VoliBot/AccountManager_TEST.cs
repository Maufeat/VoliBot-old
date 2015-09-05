using LoLLauncher;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VoliBot.Utils;

namespace VoliBot
{
    public partial class AccountManager_TEST : Form
    {
        private GifImage gifImage = null;
        private LoLConnection _connection = new LoLConnection();
        public AccountManager_TEST(string username, string password, string region)
        {
            InitializeComponent();
            gifImage = new GifImage(Properties.Resources.table);
            gifImage.ReverseAtEnd = false;
            pictureBox1.Image = gifImage.GetFrame(0);
            timer1.Enabled = true;
            _connection = new LoLConnection();
            _connection.OnLogin += new LoLConnection.OnLoginHandler(connection_OnLogin);
            _connection.OnError += new LoLConnection.OnErrorHandler(connection_OnError);
            BaseRegion testRegion = BaseRegion.GetRegion(region);
            _connection.Connect(username, password, testRegion.PVPRegion, Config.clientSeason + "." + Config.clientSubVersion);

        }

        private void AccountManager_TEST_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = Properties.Resources.table;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            pictureBox1.Image = gifImage.GetNextFrame();
        }
        private void connection_OnError(object sender, Error error)
        {
            Invoke((Action)(() =>
            {
                label1.Text = "Test Result:";
                label2.Text = "ERROR - Account don't works.";
                button1.Enabled = true;
                _connection.Disconnect();
            }));
        }
        private void connection_OnLogin(object sender, string username, string ipAddress)
        {
            Invoke((Action)(() =>
            {
                label1.Text = "Test Result:";
                label2.Text = "SUCCESS - Account works.";
                gifImage = new GifImage(Properties.Resources.glasses);
                button1.Enabled = true;
                _connection.Disconnect();
            }));
        }
        private static Image GetImageFromURL(string url)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            HttpWebResponse httpWebReponse = (HttpWebResponse)httpWebRequest.GetResponse();
            Stream stream = httpWebReponse.GetResponseStream();
            return Image.FromStream(stream);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
