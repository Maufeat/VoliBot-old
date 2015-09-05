using Ini;
using LoLLauncher;
using LoLLauncher.RiotObjects.Platform.Catalog.Champion;
using LoLLauncher.RiotObjects.Platform.Clientfacade.Domain;
using LoLLauncher.RiotObjects.Platform.Game;
using LoLLauncher.RiotObjects.Platform.Game.Message;
using LoLLauncher.RiotObjects.Platform.Matchmaking;
using LoLLauncher.RiotObjects.Platform.Statistics;
using LoLLauncher.RiotObjects.Platform.Summoner;
using LoLLauncher.RiotObjects.Platform.Trade;
using LoLLauncher.RiotObjects.Team.Dto;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using VoliBot.LoLLauncher.RiotObjects.Platform.Messaging;
using VoliBot.Utils;
using VoliBots;

namespace VoliBot
{
    public partial class vClient : Form
    {

        public VoliBot _parent;
        // ---------- Account and Summoner Infos -------------
        public String _username = "";
        public String _password = "";
        public BaseRegion _region;
        public double _summonerID;
        public String _summonerName;
        public double _profileIcon;
        public double _summonerLevel;
        public double _IPBalance;
        public double _RPBalance;
        public ChampionDTO[] _myChampions;
        public QueueTypes QueueType { get; set; }
        public QueueTypes ActualQueueType { get; set; }
        // ---------- All Variables huehue -------------------
        public bool firstTimeInLobby = true;
        public bool firstTimeInQueuePop = true;
        public bool firstTimeInCustom = true;
        public bool firstTimeInPostChampSelect = true;
        public int m_leaverBustedPenalty { get; set; }
        public string m_accessToken { get; set; }
        public Process exeProcess;
        // ---------- Threads --------------------------------
        public Thread errorThread;
        // ---------- RTMP Connections and Listeners ---------
        public LoLConnection _connection = new LoLConnection();
        public LoginDataPacket _loginDataPacket { get; set; }
        TimeSpan t;
        const int GWL_STYLE = -16;
        const long WS_VISIBLE = 0x10000000,
                    WS_MAXIMIZE = 0x01000000,
                    WS_BORDER = 0x00800000,
                    WS_CHILD = 0x40000000;
        internal exListBoxItem _controllerListItem;
        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll")]
        static extern bool MoveWindow(IntPtr Handle, int x, int y, int w, int h, bool repaint);
        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr Handle, int Msg, int wParam, int lParam);
        System.Timers.Timer dispatcherTimer;
        string specificFolder;
        private bool _continue = false;

        public vClient(String user, String pass, String region, VoliBot parent, bool autoconnect)
        {
            _username = user;
            _password = pass;
            _parent = parent;
            _region = BaseRegion.GetRegion(region);
            InitializeComponent();
            groupBox1.Text = user;
            this.Text = user;
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            specificFolder = Path.Combine(folder, "VoliBot");
            if (autoconnect)
            {
                toolStripStatusLabel1_Click(this, EventArgs.Empty);
            }
        }
        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {
            _connection.OnConnect += new LoLConnection.OnConnectHandler(connection_OnConnect);
            _connection.OnError += new LoLConnection.OnErrorHandler(connection_OnError);
            _connection.OnMessageReceived += new LoLConnection.OnMessageReceivedHandler(connection_OnMessageReceived);
            updateStatus(msgStatus.INFO, "Connecting...");
            _connection.Connect(_username, _password, _region.PVPRegion, Config.clientSeason + "." + Config.clientSubVersion);
        }

        internal void addListBoxItem(exListBoxItem eLBI)
        {
            _controllerListItem = eLBI;
        }

        private void toolStripDropDownButton1_Click(object sender, EventArgs e)
        {
            _connection.Disconnect();
        }
        private void connection_OnConnect(object sender, EventArgs e)
        {
            _connection.OnDisconnect += new LoLConnection.OnDisconnectHandler(connection_OnDisconnect);
            _connection.OnLogin += new LoLConnection.OnLoginHandler(connection_OnLogin);
            _connection.OnLoginQueueUpdate += new LoLConnection.OnLoginQueueUpdateHandler(connection_OnLoginQueueUpdate);
        }

        public async void connection_OnMessageReceived(object sender, object message)
        {
            if (message is GameDTO)
            {
                GameDTO game = message as GameDTO;
                switch (game.GameState)
                {
                    case "START_REQUESTED":
                        break;
                    case "FAILED_TO_START":
                        Console.WriteLine("Failed to Start!");
                        break;
                    case "CHAMP_SELECT":
                        firstTimeInCustom = true;
                        firstTimeInQueuePop = true;
                        if (firstTimeInLobby)
                        {
                            firstTimeInLobby = false;
                            //checkAndUpdateQueueType();
                            updateStatus(msgStatus.INFO, "In Champion Select");
                            Invoke((Action)(() =>
                            {
                                _parent.onlyUpdateListItemStatusAndLevel(_controllerListItem, "Status: Champion Select", _summonerLevel.ToString());
                            }));
                            object obj = await _connection.SetClientReceivedGameMessage(game.Id, "CHAMP_SELECT_CLIENT");
                            if (QueueType != QueueTypes.ARAM)
                            {
                                Invoke((Action)(async () =>
                                {
                                    if (comboBox2.Text != "" && comboBox2.Text != "RANDOM")
                                    {

                                        int Spell1;
                                        int Spell2;
                                        if (!checkBox1.Checked)
                                        {
                                            Spell1 = Enums.spellToId(comboBox3.Text.ToUpper());
                                            Spell2 = Enums.spellToId(comboBox4.Text.ToUpper());
                                        }
                                        else
                                        {
                                            var random = new Random();
                                            var spellList = new List<int> { 13, 6, 7, 1, 11, 21, 12, 3, 14, 2, 4 };

                                            int index = random.Next(spellList.Count);
                                            int index2 = random.Next(spellList.Count);

                                            int randomSpell1 = spellList[index];
                                            int randomSpell2 = spellList[index2];

                                            if (randomSpell1 == randomSpell2)
                                            {
                                                int index3 = random.Next(spellList.Count);
                                                randomSpell2 = spellList[index3];
                                            }

                                            Spell1 = Convert.ToInt32(randomSpell1);
                                            Spell2 = Convert.ToInt32(randomSpell2);
                                        }

                                        await _connection.SelectSpells(Spell1, Spell2);

                                        await _connection.SelectChampion(Enums.championToId(comboBox2.Text.ToUpper().Replace(" ", "-")));
                                        await _connection.ChampionSelectCompleted();

                                    }
                                    else if (comboBox2.Text == "RANDOM")
                                    {

                                        int Spell1;
                                        int Spell2;
                                        if (!checkBox1.Checked)
                                        {
                                            Spell1 = Enums.spellToId(comboBox3.Text.ToUpper());
                                            Spell2 = Enums.spellToId(comboBox4.Text.ToUpper());
                                        }
                                        else
                                        {
                                            var random = new Random();
                                            var spellList = new List<int> { 13, 6, 7, 1, 11, 21, 12, 3, 14, 2, 4 };

                                            int index = random.Next(spellList.Count);
                                            int index2 = random.Next(spellList.Count);

                                            int randomSpell1 = spellList[index];
                                            int randomSpell2 = spellList[index2];

                                            if (randomSpell1 == randomSpell2)
                                            {
                                                int index3 = random.Next(spellList.Count);
                                                randomSpell2 = spellList[index3];
                                            }

                                            Spell1 = Convert.ToInt32(randomSpell1);
                                            Spell2 = Convert.ToInt32(randomSpell2);
                                        }

                                        await _connection.SelectSpells(Spell1, Spell2);

                                        var randAvailableChampsArray = _myChampions.Shuffle();
                                        await _connection.SelectChampion(randAvailableChampsArray.First(champ => champ.Owned || champ.FreeToPlay).ChampionId);
                                        await _connection.ChampionSelectCompleted();

                                    }
                                    else
                                    {

                                        int Spell1;
                                        int Spell2;
                                        if (!checkBox1.Checked)
                                        {
                                            Spell1 = Enums.spellToId(comboBox3.Text.ToUpper());
                                            Spell2 = Enums.spellToId(comboBox4.Text.ToUpper());
                                        }
                                        else
                                        {
                                            var random = new Random();
                                            var spellList = new List<int> { 13, 6, 7, 1, 11, 21, 12, 3, 14, 2, 4 };

                                            int index = random.Next(spellList.Count);
                                            int index2 = random.Next(spellList.Count);

                                            int randomSpell1 = spellList[index];
                                            int randomSpell2 = spellList[index2];

                                            if (randomSpell1 == randomSpell2)
                                            {
                                                int index3 = random.Next(spellList.Count);
                                                randomSpell2 = spellList[index3];
                                            }

                                            Spell1 = Convert.ToInt32(randomSpell1);
                                            Spell2 = Convert.ToInt32(randomSpell2);
                                        }

                                        await _connection.SelectSpells(Spell1, Spell2);

                                        await _connection.SelectChampion(_myChampions.First(champ => champ.Owned || champ.FreeToPlay).ChampionId);
                                        await _connection.ChampionSelectCompleted();
                                    }
                                }));
                            }
                            break;
                        }
                        else
                            break;
                    case "POST_CHAMP_SELECT":
                        this.firstTimeInLobby = false;
                        if (this.firstTimeInPostChampSelect)
                        {
                            this.firstTimeInPostChampSelect = false;
                            this.updateStatus(msgStatus.INFO, "(Post Champ Select)");
                            break;
                        }
                        else
                            break;
                    case "IN_QUEUE":
                        this.updateStatus(msgStatus.INFO, "In Queue");
                        Invoke((Action)(() =>
                        {
                            _parent.onlyUpdateListItemStatusAndLevel(_controllerListItem, "Status: In Queue", _summonerLevel.ToString());
                        }));
                        break;
                    case "TERMINATED":
                        this.updateStatus(msgStatus.INFO, "Re-entering queue");
                        this.firstTimeInPostChampSelect = true;
                        this.firstTimeInQueuePop = true;
                        break;
                    case "JOINING_CHAMP_SELECT":
                        if (this.firstTimeInQueuePop && game.StatusOfParticipants.Contains("1"))
                        {
                            this.updateStatus(msgStatus.INFO, "Accepted Queue");
                            this.firstTimeInQueuePop = false;
                            this.firstTimeInLobby = true;
                            object obj = await this._connection.AcceptPoppedGame(true);
                            break;
                        }
                        else
                            break;
                    default:
                        this.updateStatus(msgStatus.INFO, "[DEFAULT]" + game.GameStateString);
                        break;
                }
            }
            else if (message.GetType() == typeof(TradeContractDTO))
            {
                TradeContractDTO tradeDto = message as TradeContractDTO;
                if (tradeDto != null)
                {
                    switch (tradeDto.State)
                    {
                        case "PENDING":
                            if (tradeDto != null)
                            {
                                object obj = await this._connection.AcceptTrade(tradeDto.RequesterInternalSummonerName, (int)tradeDto.RequesterChampionId);
                                break;
                            }
                            else
                                break;
                    }
                }
            }
            else if (message is PlayerCredentialsDto)
            {
                firstTimeInPostChampSelect = true;
                PlayerCredentialsDto dto = message as PlayerCredentialsDto;
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.WorkingDirectory = FindLoLExe();
                startInfo.FileName = "League of Legends.exe";
                startInfo.Arguments = "\"8394\" \"LoLLauncher.exe\" \"\" \"" + dto.ServerIp + " " +
                    dto.ServerPort + " " + dto.EncryptionKey + " " + dto.SummonerId + "\"";
                updateStatus(msgStatus.INFO, "Launching League of Legends");
                Invoke((Action)(() =>
                {
                    _parent.onlyUpdateListItemStatusAndLevel(_controllerListItem, "Status: In Game", _summonerLevel.ToString());
                }));
                new Thread(() =>
                {

                    exeProcess = Process.Start(startInfo);
                    exeProcess.Exited += new EventHandler(exeProcess_Exited);
                    while (exeProcess.MainWindowHandle == IntPtr.Zero) { }
                    Invoke((Action)(() =>
                    {
                        button1.Enabled = false;
                        if (checkBox2.Checked)
                        {
                            SetParent(exeProcess.MainWindowHandle, this.lolContainer.Handle);
                            MoveWindow(exeProcess.MainWindowHandle, 0, 0, lolContainer.Width, lolContainer.Height, true);
                            SetWindowLong(exeProcess.MainWindowHandle, GWL_STYLE, (int)(WS_VISIBLE + (WS_MAXIMIZE | WS_BORDER)));
                            this.lolContainer.BringToFront();
                            lolContainer.Resize += moveWindows;
                            this.FormClosed += new FormClosedEventHandler(
                                 delegate(object sender2, FormClosedEventArgs e)
                                 {
                                     SendMessage(exeProcess.MainWindowHandle, 83, 0, 0);
                                     try { killContainedLeague(); }
                                     catch { }
                                     Thread.Sleep(1000);
                                 }
                            );
                        }
                    }));
                }).Start();
            }
            else if (!(message is GameNotification) && !(message is SearchingForMatchNotification))
            {
                if (message is EndOfGameStats)
                {
                    if (!_continue)
                    {
                        Invoke((Action)(() =>
                        {
                            button2.Enabled = false;
                            button1.Enabled = true;
                        }));
                        return;
                    }
                    Invoke((Action)(() =>
                    {
                        button2.Enabled = true;
                    }));
                    object obj4 = await _connection.ackLeaverBusterWarning();
                    object obj5 = await _connection.callPersistenceMessaging(new SimpleDialogMessageResponse()
                    {
                        AccountID = _loginDataPacket.AllSummonerData.Summoner.SumId,
                        MsgID = _loginDataPacket.AllSummonerData.Summoner.SumId,
                        Command = "ack"
                    });
                    MatchMakerParams matchParams = new MatchMakerParams();
                    checkAndUpdateQueueType();
                    if (QueueType == QueueTypes.INTRO_BOT)
                    {
                        matchParams.BotDifficulty = "INTRO";
                    }
                    else if (QueueType == QueueTypes.BEGINNER_BOT)
                    {
                        matchParams.BotDifficulty = "EASY";
                    }
                    else if (QueueType == QueueTypes.MEDIUM_BOT)
                    {
                        matchParams.BotDifficulty = "MEDIUM";
                    }
                    if ((int)QueueType != 0)
                    {
                        matchParams.QueueIds = new Int32[1] { (int)QueueType };
                        SearchingForMatchNotification m = await _connection.AttachToQueue(matchParams);

                        if (m.PlayerJoinFailures == null)
                        {
                            this.updateStatus(msgStatus.INFO, "In Queue: " + QueueType.ToString());
                            Invoke((Action)(() =>
                            {
                                _parent.onlyUpdateListItemStatusAndLevel(_controllerListItem, "Status: In Queue", _summonerLevel.ToString());
                            }));
                        }
                        else
                        {
                            foreach (var failure in m.PlayerJoinFailures)
                            {
                                if (failure.ReasonFailed == "LEAVER_BUSTED")
                                {
                                    m_accessToken = failure.AccessToken;
                                    if (failure.LeaverPenaltyMillisRemaining > m_leaverBustedPenalty)
                                    {
                                        m_leaverBustedPenalty = failure.LeaverPenaltyMillisRemaining;
                                    }
                                }
                            }

                            if (String.IsNullOrEmpty(m_accessToken))
                            {
                                foreach (var failure in m.PlayerJoinFailures)
                                {
                                    updateStatus(msgStatus.INFO, "Dodge Remaining Time: " + Convert.ToString((failure.DodgePenaltyRemainingTime / 1000 / (float)60)).Replace(",", ":") + "...");
                                }
                            }
                            else
                            {
                                double minutes = m_leaverBustedPenalty / 1000 / (float)60;
                                updateStatus(msgStatus.INFO, "Waiting out leaver buster: " + minutes + " minutes!");
                                t = TimeSpan.FromMinutes((int)minutes);
                                Tick();
                                System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(m_leaverBustedPenalty));
                                m = await _connection.AttachToLowPriorityQueue(matchParams, m_accessToken);
                                if (m.PlayerJoinFailures == null)
                                {
                                    updateStatus(msgStatus.INFO, "Succesfully joined lower priority queue!");
                                }
                                else
                                {
                                    updateStatus(msgStatus.ERROR, "There was an error in joining lower priority queue.\nDisconnecting.");
                                    _connection.Disconnect();
                                }
                            }
                        }
                    }
                }
                else if (message.ToString().Contains("EndOfGameStats"))
                {
                    /*if (itsMe.SummonerLevel != 30)
                    {
                        if (itsMe.LastGameIP > itsMe.IPBalance)
                        {
                            itsMe.IPEarned = itsMe.LastGameIP - itsMe.IPBalance;
                        }
                        else
                        {
                            Connection.SessionXP = Connection.SummEXP - Connection.lastgameSummEXP;
                            requestXP = Connection.SummEXP - Connection.lastgameSummEXP;
                        }
                    }*/
                    EndOfGameStats eog = new EndOfGameStats();
                    this.connection_OnMessageReceived(sender, (object)eog);
                    this.exeProcess.Exited -= new EventHandler(this.exeProcess_Exited);
                    this.exeProcess.Kill();
                    Thread.Sleep(500);
                    if (this.exeProcess.Responding)
                        Process.Start("taskkill /F /IM \"League of Legends.exe\"");
                }
            }
        }
        private void connection_OnLogin(object sender, string username, string ipAddress)
        {
            updateStatus(msgStatus.INFO, "Logging in...");
            new Thread((ThreadStart)(async () =>
            {
                this._loginDataPacket = await _connection.GetLoginDataPacketForUser();
                object obj1 = await _connection.Subscribe("bc", this._loginDataPacket.AllSummonerData.Summoner.AcctId);
                object obj2 = await _connection.Subscribe("cn", this._loginDataPacket.AllSummonerData.Summoner.AcctId);
                object obj3 = await _connection.Subscribe("gn", this._loginDataPacket.AllSummonerData.Summoner.AcctId);
                if (this._loginDataPacket.AllSummonerData == null)
                {
                    Random random = new Random();
                    string summonerName = _username;
                    if (summonerName.Length > 16)
                        summonerName = summonerName.Substring(0, 12) + new Random().Next(1000, 9999).ToString();
                    AllSummonerData defaultSummoner = await _connection.CreateDefaultSummoner(summonerName);
                    updateStatus(msgStatus.INFO, "Created Summoner: " + summonerName);
                }
                object obj4 = await _connection.ackLeaverBusterWarning();
                object obj5 = await _connection.callPersistenceMessaging(new SimpleDialogMessageResponse()
                {
                    AccountID = this._loginDataPacket.AllSummonerData.Summoner.SumId,
                    MsgID = this._loginDataPacket.AllSummonerData.Summoner.SumId,
                    Command = "ack"
                });
                this._summonerID = this._loginDataPacket.AllSummonerData.Summoner.SumId;
                this._summonerName = this._loginDataPacket.AllSummonerData.Summoner.Name;
                this._profileIcon = this._loginDataPacket.AllSummonerData.Summoner.ProfileIconId;
                this._summonerLevel = this._loginDataPacket.AllSummonerData.SummonerLevel.Level;
                this._IPBalance = this._loginDataPacket.IpBalance;
                this._RPBalance = this._loginDataPacket.RpBalance;
                this._myChampions = await _connection.GetAvailableChampions();
                updateStatus(msgStatus.INFO, "Logged in as " + this._summonerName + " @ level " + this._summonerLevel);
                PlayerDTO player = await _connection.CreatePlayer();
                updateSummonerUI();
                if (this._loginDataPacket.ReconnectInfo != null && this._loginDataPacket.ReconnectInfo.Game != null)
                    connection_OnMessageReceived(sender, (object)this._loginDataPacket.ReconnectInfo.PlayerCredentials);
                updateStatusBar("Logged in");
            })).Start();
        }
        private void updateSummonerUI()
        {
            Invoke((Action)(() =>
            {
                toolStripDropDownButton1.Text = "Disconnect";
                toolStripDropDownButton1.Image = Properties.Resources.disconnect;
                toolStripDropDownButton1.Click -= toolStripStatusLabel1_Click;
                toolStripDropDownButton1.Click += toolStripDropDownButton1_Click;
                toolStripStatusLabel2.Text = "RP: " + _RPBalance;
                toolStripStatusLabel1.Text = "IP: " + _IPBalance;
                groupBox1.Text = _summonerName;
                comboBox1.Enabled = true;
                button1.Enabled = true;
                updateTitle(_summonerName + " | Level : " + _summonerLevel);
                Image icon = Basic.returnIcon(_loginDataPacket.AllSummonerData.Summoner.ProfileIconId);
                _parent.updateListItem(_controllerListItem, _summonerName, "Status: Logged in", _summonerLevel.ToString(), icon);
                getAndChangeIcon(_loginDataPacket.AllSummonerData.Summoner.ProfileIconId);
                foreach (ChampionDTO champion in _myChampions)
                {
                    if (champion.Owned)
                    {
                        comboBox2.Items.Add(Basic.UppercaseFirst(Enums.championToString(champion.ChampionId)));
                    }
                    else if (champion.FreeToPlay)
                    {
                        comboBox2.Items.Add(Basic.UppercaseFirst(Enums.championToString(champion.ChampionId)) + " | [FREE]");
                        comboBox2.Items.Add(Basic.UppercaseFirst(Enums.championToString(champion.ChampionId)));
                    }
                }
            }));
        }

        private void connection_OnLoginQueueUpdate(object sender, int positionInLine)
        {
            updateStatus(msgStatus.INFO, "Login Queue Position: " + positionInLine);
        }
        public void connection_OnDisconnect(object sender, EventArgs e)
        {
            _connection.OnConnect -= new LoLConnection.OnConnectHandler(connection_OnConnect);
            _connection.OnError -= new LoLConnection.OnErrorHandler(connection_OnError);
            _connection.OnMessageReceived -= new LoLConnection.OnMessageReceivedHandler(connection_OnMessageReceived);
            _connection.OnDisconnect -= new LoLConnection.OnDisconnectHandler(connection_OnDisconnect);
            _connection.OnLogin -= new LoLConnection.OnLoginHandler(connection_OnLogin);
            _connection.OnLoginQueueUpdate -= new LoLConnection.OnLoginQueueUpdateHandler(connection_OnLoginQueueUpdate);
            updateStatus(msgStatus.INFO, "Disconnected");
            Invoke((Action)(() =>
            {
                toolStripDropDownButton1.Text = "Connect";
                toolStripDropDownButton1.Image = Properties.Resources.connect;
                toolStripDropDownButton1.Click -= toolStripDropDownButton1_Click;
                toolStripDropDownButton1.Click += toolStripStatusLabel1_Click;
                toolStripStatusLabel2.Text = "RP: 0";
                toolStripStatusLabel1.Text = "IP: 0";
                groupBox1.Text = _username;
                updateTitle(_username);
                button1.Enabled = false;
                button2.Enabled = false;
            }));
        }
        void exeProcess_Exited(object sender, EventArgs e)
        {
            updateStatus(msgStatus.INFO, "Restart League of Legends.");
            lolContainer.Resize -= moveWindows;
            Thread.Sleep(1000);
            if (_loginDataPacket.ReconnectInfo != null && _loginDataPacket.ReconnectInfo.Game != null)
            {
                this.connection_OnMessageReceived(sender, (object)_loginDataPacket.ReconnectInfo.PlayerCredentials);
            }
            else
                this.connection_OnMessageReceived(sender, (object)new EndOfGameStats());
        }
        private void connection_OnError(object sender, Error error)
        {
            if (error.Type == ErrorType.AuthKey || error.Type == ErrorType.General)
            {
                Thread errorThread = new Thread(() =>
                {
                    updateStatus(msgStatus.INFO, "Unable to connect. Try to reconnect.");
                });
                errorThread.Start();
                return;
            }
            if (error.Message.Contains("is not owned by summoner"))
            {
                return;
            }
            else if (error.Message.Contains("Your summoner level is too low to select the spell"))
            {
                var random = new Random();
                var spellList = new List<int> { 13, 6, 7, 10, 1, 11, 21, 12, 3, 14, 2, 4 };

                int index = random.Next(spellList.Count);
                int index2 = random.Next(spellList.Count);

                int randomSpell1 = spellList[index];
                int randomSpell2 = spellList[index2];

                if (randomSpell1 == randomSpell2)
                {
                    int index3 = random.Next(spellList.Count);
                    randomSpell2 = spellList[index3];
                }

                int Spell1 = Convert.ToInt32(randomSpell1);
                int Spell2 = Convert.ToInt32(randomSpell2);
                return;
            }
            updateStatus(msgStatus.ERROR, error.Message);
        }

        private void updateStatus(msgStatus type, String msg)
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
            richTextBox1.AppendText("[" + DateTime.Now.ToShortTimeString() + "]", Color.Blue);
            richTextBox1.AppendText(" ", Color.Black);
            richTextBox1.AppendText(_username, Color.DarkBlue);
            richTextBox1.AppendText(": ", Color.Black);
            richTextBox1.AppendText(msg, Color.Black);
            richTextBox1.AppendText(Environment.NewLine, Color.Black);
        }

        private void updateStatus(msgStatus type, String msg, Color msgClr)
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
            richTextBox1.AppendText("[" + DateTime.Now.ToShortTimeString() + "]", Color.Blue);
            richTextBox1.AppendText(" ", Color.Black);
            richTextBox1.AppendText(_username, Color.DarkBlue);
            richTextBox1.AppendText(": ", Color.Black);
            richTextBox1.AppendText(msg, msgClr);
            richTextBox1.AppendText(Environment.NewLine, Color.Black);
        }
        private void updateStatusBar(String msg)
        {
            toolStripStatusLabel3.Text = "Status: " + msg;
        }
        private void updateTitle(String msg)
        {
            this.Text = msg;
        }
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }
        public void getAndChangeIcon(int id)
        {
            string url = "http://ddragon.leagueoflegends.com/cdn/5.15.1/img/profileicon/" + id.ToString() + ".png";
            string path = specificFolder + "\\assets\\icons";
            string file = path + "\\" + id + ".png";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            if (!File.Exists(file))
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile(url, file);
                }
            }
            Image _icon = Image.FromFile(file);
            this.Icon = Icon.FromHandle(new Bitmap(_icon, new Size(16, 16)).GetHicon());
            this.pictureBox1.Image = _icon;
        }
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        public void killContainedLeague()
        {
            IntPtr toKill = lolContainer.Handle;
            SendMessage(toKill, 0x10, IntPtr.Zero, IntPtr.Zero);
        }
        private void vClient_Load(object sender, EventArgs e)
        {
            getAndChangeIcon(0);
            foreach (string queue in Enums.queues)
            {
                comboBox1.Items.Add(queue);
            }
            foreach (string spell in Enums.spells)
            {
                comboBox3.Items.Add(Basic.UppercaseFirst(spell));
                comboBox4.Items.Add(Basic.UppercaseFirst(spell));
            }
            comboBox2.Items.Add((object)"RANDOM");
            checkLogsAndStats();
        }
        private void checkLogsAndStats()
        {
            string path = specificFolder + "\\accounts\\" + _username;

            IniFile iniStats = new IniFile(path + "\\stats\\" + DateTime.Today.ToString("d") + ".txt");

            if (!File.Exists(path + "\\config.ini"))
            {
                // Create config for account if not exist
                Directory.CreateDirectory(path);
                using (StreamWriter sw = File.CreateText(path + "\\config.ini"))
                {
                    sw.Write("[General]" + Environment.NewLine + "QueueType=" + Config.defaultQueue + Environment.NewLine + "LastSpell1=" + Config.defaultSlotOne + Environment.NewLine + "LastSpell2=" + Config.defaultSlotTwo + Environment.NewLine + "LastChampion=RANDOM" + Environment.NewLine + "LastPath=" + Config.defaultPath);
                }
                // Create stats path if not exist
                if (!Directory.Exists(path + "\\stats"))
                {
                    Directory.CreateDirectory(path + "\\stats");
                    if (!File.Exists(path + "\\stats\\" + DateTime.Today.ToString("d") + ".txt"))
                    {
                        using (StreamWriter sw1 = File.CreateText(path + "\\stats\\" + DateTime.Today.ToString("d") + ".txt"))
                        {
                            sw1.Write("[Stats]" + Environment.NewLine + "Matches=0" + Environment.NewLine + "Wins=0" + Environment.NewLine + "IP=0" + Environment.NewLine + "XP=0");
                        }
                    }
                }
                for (int i = 0; i < comboBox1.Items.Count; i++)
                {
                    if (comboBox1.GetItemText(comboBox1.Items[i]) == Config.defaultQueue)
                    {
                        comboBox1.SelectedIndex = i;
                    }
                }
                for (int i = 0; i < comboBox2.Items.Count; i++)
                {
                    if (comboBox2.GetItemText(comboBox2.Items[i]) == Config.defaultChampion)
                    {
                        comboBox2.SelectedIndex = i;
                    }
                }
                for (int i = 0; i < comboBox3.Items.Count; i++)
                {
                    if (comboBox3.GetItemText(comboBox3.Items[i]) == Config.defaultSlotOne)
                    {
                        comboBox3.SelectedIndex = i;
                    }
                }
                for (int i = 0; i < comboBox4.Items.Count; i++)
                {
                    if (comboBox4.GetItemText(comboBox4.Items[i]) == Config.defaultSlotTwo)
                    {
                        comboBox4.SelectedIndex = i;
                    }
                }
                textBox1.Text = Config.defaultPath;
            }
            else if (File.Exists(path + "\\config.ini"))
            {
                IniFile ini = new IniFile(path + "\\config.ini");
                textBox1.Text = ini.IniReadValue("General", "LastPath");
                for (int i = 0; i < comboBox1.Items.Count; i++)
                {
                    if (comboBox1.GetItemText(comboBox1.Items[i]) == ini.IniReadValue("General", "QueueType"))
                    {
                        comboBox1.SelectedIndex = i;
                    }
                }
                for (int i = 0; i < comboBox2.Items.Count; i++)
                {
                    if (comboBox2.GetItemText(comboBox2.Items[i]) == ini.IniReadValue("General", "LastChampion"))
                    {
                        comboBox2.SelectedIndex = i;
                    }
                }
                for (int i = 0; i < comboBox3.Items.Count; i++)
                {
                    if (comboBox3.GetItemText(comboBox3.Items[i]) == ini.IniReadValue("General", "LastSpell1"))
                    {
                        comboBox3.SelectedIndex = i;
                    }
                }
                for (int i = 0; i < comboBox4.Items.Count; i++)
                {
                    if (comboBox4.GetItemText(comboBox4.Items[i]) == ini.IniReadValue("General", "LastSpell2"))
                    {
                        comboBox4.SelectedIndex = i;
                    }
                }
            }
            /*if (!File.Exists(path + "\\stats\\" + DateTime.Today.ToString("d") + ".txt"))
            {
                using (StreamWriter sw1 = File.CreateText(path + "\\stats\\" + DateTime.Today.ToString("d") + ".txt"))
                {
                    sw1.Write("[Stats]" + Environment.NewLine + Environment.NewLine + "Matches=0" + Environment.NewLine + "Wins=0" + Environment.NewLine + "IP=0" + Environment.NewLine + "XP=0");
                }
            }*/
            foreach (string s in Directory.GetFiles(path + "\\stats\\", "*.txt"))
            {
                try
                {
                    string thatDate = System.IO.Path.GetFileNameWithoutExtension(s);
                    IniFile iniDate = new IniFile(s);

                    TreeNode dateNode = new TreeNode();
                    TreeNode matchesChildNode = new TreeNode();
                    TreeNode winsChildNode = new TreeNode();
                    TreeNode losesChildNode = new TreeNode();
                    TreeNode ipChildNode = new TreeNode();
                    TreeNode rpChildNode = new TreeNode();

                    dateNode.Text = thatDate;
                    matchesChildNode.Text = "Matches: " + iniDate.IniReadValue("Stats", "Matches");
                    winsChildNode.Text = "Wins: " + iniDate.IniReadValue("Stats", "Wins");
                    losesChildNode.Text = "Loses: " + (Convert.ToInt32(iniDate.IniReadValue("Stats", "Matches")) - Convert.ToInt32(iniDate.IniReadValue("Stats", "Wins")));
                    ipChildNode.Text = "IP Earned: " + iniDate.IniReadValue("Stats", "IP");
                    rpChildNode.Text = "RP Earned: " + iniDate.IniReadValue("Stats", "XP");

                    treeView1.Nodes.Add(dateNode);
                    dateNode.Nodes.Add(matchesChildNode);
                    dateNode.Nodes.Add(winsChildNode);
                    dateNode.Nodes.Add(losesChildNode);
                    dateNode.Nodes.Add(ipChildNode);
                    dateNode.Nodes.Add(rpChildNode);
                    /*TreeNode ParentNode = new TreeNode();
                    ParentNode.Text = "RootNode";
                    ParentNode.ForeColor = Color.Black;
                    ParentNode.BackColor = Color.White;
                    ParentNode.ImageIndex = 0;
                    ParentNode.SelectedImageIndex = 0;
                    treeView1.Nodes.Add(ParentNode);

                    TreeNode ChildNode1 = new TreeNode();
                    ChildNode1.Text = "Child 1";
                    ChildNode1.ForeColor = Color.Black;
                    ChildNode1.BackColor = Color.White;
                    ChildNode1.ImageIndex = 0;
                    ChildNode1.SelectedImageIndex = 0;
                    ParentNode.Nodes.Add(ChildNode1); */
                }
                catch
                {

                }
            }
        }
        private void checkAndUpdateQueueType()
        {
            Invoke(new Action(() =>
            {
                try
                {
                    if (comboBox1.Text != "")
                    {
                        if (comboBox1.Text.ToString().Contains("5vs5"))
                        {
                            QueueType = QueueTypes.NORMAL_5x5;
                        }
                        else if (comboBox1.Text.ToString().Contains("3vs3"))
                        {
                            QueueType = QueueTypes.NORMAL_3x3;
                        }
                        else if (comboBox1.Text.ToString().Contains("ARAM"))
                        {
                            QueueType = QueueTypes.ARAM;
                        }
                        else if (comboBox1.Text.ToString().Contains("Intro"))
                        {
                            QueueType = QueueTypes.INTRO_BOT;
                        }
                        else if (comboBox1.Text.ToString().Contains("Beginner"))
                        {
                            QueueType = QueueTypes.BEGINNER_BOT;
                        }
                        else if (comboBox1.Text.ToString().Contains("Intermediate"))
                        {
                            QueueType = QueueTypes.MEDIUM_BOT;
                        }
                    }
                    else
                    {
                        updateStatus(msgStatus.ERROR, "You have to select a queue");
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            })); try
            {
                //SetUp
                if (_summonerLevel < 3.0 && QueueType == QueueTypes.NORMAL_5x5)
                {
                    this.updateStatus(msgStatus.INFO, "Need to be Level 3 before NORMAL_5x5 queue.");
                    this.updateStatus(msgStatus.INFO, "Joins Co-Op vs AI (Beginner) queue until 3");
                    QueueType = QueueTypes.BEGINNER_BOT;
                    ActualQueueType = QueueTypes.NORMAL_5x5;
                }
                else if (_summonerLevel < 6.0 && QueueType == QueueTypes.ARAM)
                {
                    this.updateStatus(msgStatus.INFO, "Need to be Level 6 before ARAM queue.");
                    this.updateStatus(msgStatus.INFO, "Joins Co-Op vs AI (Beginner) queue until 6");
                    QueueType = QueueTypes.BEGINNER_BOT;
                    ActualQueueType = QueueTypes.ARAM;
                }
                else if (_summonerLevel < 7.0 && QueueType == QueueTypes.NORMAL_3x3)
                {
                    this.updateStatus(msgStatus.INFO, "Need to be Level 7 before NORMAL_3x3 queue.");
                    this.updateStatus(msgStatus.INFO, "Joins Co-Op vs AI (Beginner) queue until 7");
                    QueueType = QueueTypes.BEGINNER_BOT;
                    ActualQueueType = QueueTypes.NORMAL_3x3;
                }
                //Check if is available to join queue.
                if (_summonerLevel == 3 && ActualQueueType == QueueTypes.NORMAL_5x5)
                {
                    QueueType = ActualQueueType;
                }
                else if (_summonerLevel == 6 && ActualQueueType == QueueTypes.ARAM)
                {
                    QueueType = ActualQueueType;
                }
                else if (_summonerLevel == 7 && ActualQueueType == QueueTypes.NORMAL_3x3)
                {
                    QueueType = ActualQueueType;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("^2");
            }
        }
        private async void buyBoost()
        {
            try
            {
                string url = await _connection.GetStoreUrl();
                HttpClient httpClient = new HttpClient();
                await httpClient.GetStringAsync(url);

                string storeURL = "https://store." + _region.ChatName + ".lol.riotgames.com/store/tabs/view/boosts/1";
                await httpClient.GetStringAsync(storeURL);

                string purchaseURL = "https://store." + _region.ChatName + ".lol.riotgames.com/store/purchase/item";

                List<KeyValuePair<string, string>> storeItemList = new List<KeyValuePair<string, string>>();
                storeItemList.Add(new KeyValuePair<string, string>("item_id", "boosts_2"));
                storeItemList.Add(new KeyValuePair<string, string>("currency_type", "rp"));
                storeItemList.Add(new KeyValuePair<string, string>("quantity", "1"));
                storeItemList.Add(new KeyValuePair<string, string>("rp", "260"));
                storeItemList.Add(new KeyValuePair<string, string>("ip", "null"));
                storeItemList.Add(new KeyValuePair<string, string>("duration_type", "PURCHASED"));
                storeItemList.Add(new KeyValuePair<string, string>("duration", "3"));
                HttpContent httpContent = new FormUrlEncodedContent(storeItemList);
                await httpClient.PostAsync(purchaseURL, httpContent);

                updateStatus(msgStatus.INFO, "Bought 'XP Boost: 3 Days'!");
                httpClient.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        private String FindLoLExe()
        {
            try
            {
                if (!textBox1.Text.EndsWith("\\"))
                {
                    textBox1.Text = textBox1.Text + "\\";
                }
                String installPath = textBox1.Text;
                if (installPath.Contains("notfound"))
                    return installPath;
                installPath += @"RADS\solutions\lol_game_client_sln\releases\";
                installPath = Directory.EnumerateDirectories(installPath).OrderBy(f => new DirectoryInfo(f).CreationTime).Last();
                installPath += @"\deploy\";
                return installPath;
            }
            catch (DirectoryNotFoundException)
            {
                //MessageBox.Show("[Options][" + _summonerName + "] LauncherPath error! Directory not found.");
                return "";
            }
        }
        private void vClient_FormClosing(object sender, FormClosingEventArgs e)
        {
            _parent.removeMdiChild(_username, _controllerListItem);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked && exeProcess != null && lolContainer.Handle == null)
            {
                SetParent(exeProcess.MainWindowHandle, this.lolContainer.Handle);
                MoveWindow(exeProcess.MainWindowHandle, 0, 0, lolContainer.Width, lolContainer.Height, true);
                SetWindowLong(exeProcess.MainWindowHandle, GWL_STYLE, (int)(WS_VISIBLE + (WS_MAXIMIZE | WS_BORDER)));
                this.lolContainer.BringToFront();
                lolContainer.Resize += moveWindows;
                this.FormClosed += new FormClosedEventHandler(
                     delegate(object sender2, FormClosedEventArgs ve)
                     {
                         SendMessage(exeProcess.MainWindowHandle, 83, 0, 0);
                         try { killContainedLeague(); }
                         catch { }
                         Thread.Sleep(1000);
                     }
                );
            }
        }

        public void moveWindows(object sender2, EventArgs ve)
        {
            MoveWindow(exeProcess.MainWindowHandle, 0, 0, lolContainer.Width, lolContainer.Height, true);
        }
        FormWindowState LastWindowState = FormWindowState.Minimized;
        private void vClient_Resize(object sender, EventArgs e)
        {
            // When window state changes
            if (WindowState != LastWindowState)
            {
                LastWindowState = WindowState;


                if (WindowState == FormWindowState.Maximized)
                {
                    ShowIcon = true;
                }
                if (WindowState == FormWindowState.Normal)
                {
                    ShowIcon = false;
                }
            }
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

        private void vClient_SizeChanged(object sender, EventArgs e)
        {
            //this.Text = "W: " + this.Size.Width + " H:" + this.Size.Height;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(0);
            _continue = true;
            if (!File.Exists(FindLoLExe() + "League of Legends.exe"))
            {
                updateStatus(msgStatus.ERROR, "League Of Legends not found! Edit \"LauncherPath\" in the Options-Tab!");
                return;
            }
            else
            {
                updateStatus(msgStatus.INFO, "League Of Legends found!", Color.Green);
            }
            new Thread((ThreadStart)(async () =>
            {
                object obj4 = await _connection.ackLeaverBusterWarning();
                object obj5 = await _connection.callPersistenceMessaging(new SimpleDialogMessageResponse()
                {
                    AccountID = _loginDataPacket.AllSummonerData.Summoner.SumId,
                    MsgID = _loginDataPacket.AllSummonerData.Summoner.SumId,
                    Command = "ack"
                });
                MatchMakerParams matchParams = new MatchMakerParams();
                checkAndUpdateQueueType();
                if (QueueType == QueueTypes.INTRO_BOT)
                {
                    matchParams.BotDifficulty = "INTRO";
                }
                else if (QueueType == QueueTypes.BEGINNER_BOT)
                {
                    matchParams.BotDifficulty = "EASY";
                }
                else if (QueueType == QueueTypes.MEDIUM_BOT)
                {
                    matchParams.BotDifficulty = "MEDIUM";
                }
                updateStatus(msgStatus.INFO, QueueType.ToString());
                if ((int)QueueType != 0)
                {
                    matchParams.QueueIds = new Int32[1] { (int)QueueType };
                    SearchingForMatchNotification m = await _connection.AttachToQueue(matchParams);

                    if (m.PlayerJoinFailures == null)
                    {
                        updateStatus(msgStatus.INFO, "In Queue: " + QueueType.ToString());
                        Invoke(new Action(() =>
                        {
                            button1.Enabled = false;
                            button2.Enabled = true;
                        }));
                    }
                    else
                    {
                        foreach (var failure in m.PlayerJoinFailures)
                        {
                            if (failure.ReasonFailed == "LEAVER_BUSTED")
                            {
                                m_accessToken = failure.AccessToken;
                                if (failure.LeaverPenaltyMillisRemaining > m_leaverBustedPenalty)
                                {
                                    m_leaverBustedPenalty = failure.LeaverPenaltyMillisRemaining;
                                }
                            }
                        }

                        if (String.IsNullOrEmpty(m_accessToken))
                        {
                            foreach (var failure in m.PlayerJoinFailures)
                            {
                                updateStatus(msgStatus.INFO, "Dodge Remaining Time: " + Convert.ToString((failure.DodgePenaltyRemainingTime / 1000 / (float)60)).Replace(",", ":") + "...");
                            }
                        }
                        else
                        {
                            double minutes = m_leaverBustedPenalty / 1000 / (float)60;
                            updateStatus(msgStatus.INFO, "Waiting out leaver buster: " + minutes + " minutes!");
                            t = TimeSpan.FromMinutes((int)minutes);
                            Tick();
                            System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(m_leaverBustedPenalty));
                            m = await _connection.AttachToLowPriorityQueue(matchParams, m_accessToken);
                            if (m.PlayerJoinFailures == null)
                            {
                                updateStatus(msgStatus.INFO, "Succesfully joined lower priority queue!");
                            }
                            else
                            {
                                updateStatus(msgStatus.ERROR, "There was an error in joining lower priority queue.\nDisconnecting.");
                                _connection.Disconnect();
                            }
                        }
                    }
                }
            })).Start();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            _continue = false;
            Invoke(new Action(async () =>
            {
                bool cancel = await _connection.CancelFromQueueIfPossible((int)QueueType);
                if (cancel)
                {
                    updateStatus(msgStatus.INFO, "Dodged from Queue.");
                    button1.Enabled = true;
                    button2.Enabled = false;
                }
                else
                {
                    updateStatus(msgStatus.INFO, "Couldn't dodge Queue.");
                }
            }));
        }
        public void Tick()
        {
            dispatcherTimer = new System.Timers.Timer(1000);
            dispatcherTimer.Elapsed += new ElapsedEventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = 1000;
            dispatcherTimer.Start();
        }
        private void dispatcherTimer_Tick(object sender, ElapsedEventArgs e)
        {
            t = t.Subtract(TimeSpan.FromSeconds(1)); //Subtract a second and reassign
            if (t.Seconds < 0)
            {

                Invoke((Action)(() =>
                {
                    _parent.onlyUpdateListItemStatusAndLevel(_controllerListItem, "Status: In Queue", _summonerLevel.ToString());
                }));
                dispatcherTimer.Stop();
                return;
            }
            int cMinutes = 0;
            int cSeconds = 0;
            foreach (char c in t.Minutes.ToString())
            {
                ++cMinutes;
            }
            foreach (char c in t.Seconds.ToString())
            {
                ++cSeconds;
            }
            string minutes = t.Minutes.ToString();
            string seconds = t.Seconds.ToString();
            if (cMinutes == 1)
            {
                minutes = "0" + t.Minutes;
            }
            if (cSeconds == 1)
            {
                seconds = "0" + t.Seconds;
            }
            Invoke((Action)(() =>
            {
                _parent.onlyUpdateListItemStatusAndLevel(_controllerListItem, "Status: Waiting " + minutes + ":" + seconds, _summonerLevel.ToString());
            }));
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        { 
            QueueType = (QueueTypes)System.Enum.Parse(typeof(QueueTypes), comboBox1.Text);
            string path = specificFolder + "\\accounts\\" + _username;
            IniFile ini = new IniFile(path + "\\config.ini");
            ini.IniWriteValue("General", "QueueType", comboBox1.Text);      
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string path = specificFolder + "\\accounts\\" + _username;
            IniFile ini = new IniFile(path + "\\config.ini");
            ini.IniWriteValue("General", "LastChampion", comboBox2.Text);
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            string path = specificFolder + "\\accounts\\" + _username;
            IniFile ini = new IniFile(path + "\\config.ini");
            ini.IniWriteValue("General", "LastSpell1", comboBox3.Text);
            ini.IniWriteValue("General", "LastSpell2", comboBox4.Text);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string path = specificFolder + "\\accounts\\" + _username;
            IniFile ini = new IniFile(path + "\\config.ini");
            ini.IniWriteValue("General", "LastPath", textBox1.Text);
        }

        private void statusStrip2_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            Basic.ReplaceGameConfig(textBox1.Text);
            updateStatus(msgStatus.INFO, "Config replaced. (\"DELETE CONFIG\" brings back default)");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Basic.DeleteGameConfig(textBox1.Text);
        }
    }
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return source.Shuffle(new Random());
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rng)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (rng == null) throw new ArgumentNullException("rng");

            return source.ShuffleIterator(rng);
        }

        private static IEnumerable<T> ShuffleIterator<T>(
            this IEnumerable<T> source, Random rng)
        {
            List<T> buffer = source.ToList();
            for (int i = 0; i < buffer.Count; i++)
            {
                int j = rng.Next(i, buffer.Count);
                yield return buffer[j];

                buffer[j] = buffer[i];
            }
        }
    }
    public enum msgStatus
    {
        ERROR,
        INFO,
        DEBUG,
        UNDEFINED
    }

}
