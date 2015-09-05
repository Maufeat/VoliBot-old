using LoLLauncher;
using LoLLauncher.RiotObjects.Platform.Catalog.Champion;
using LoLLauncher.RiotObjects.Platform.Clientfacade.Domain;
using LoLLauncher.RiotObjects.Platform.Game;
using LoLLauncher.RiotObjects.Platform.Game.Message;
using LoLLauncher.RiotObjects.Platform.Matchmaking;
using LoLLauncher.RiotObjects.Platform.Statistics;
using LoLLauncher.RiotObjects;
using LoLLauncher.RiotObjects.Leagues.Pojo;
using LoLLauncher.RiotObjects.Platform.Game.Practice;
using LoLLauncher.RiotObjects.Platform.Harassment;
using LoLLauncher.RiotObjects.Platform.Leagues.Client.Dto;
using LoLLauncher.RiotObjects.Platform.Login;
using LoLLauncher.RiotObjects.Platform.Reroll.Pojo;
using LoLLauncher.RiotObjects.Platform.Statistics.Team;
using LoLLauncher.RiotObjects.Platform.Summoner;
using LoLLauncher.RiotObjects.Platform.Summoner.Boost;
using LoLLauncher.RiotObjects.Platform.Summoner.Masterybook;
using LoLLauncher.RiotObjects.Platform.Summoner.Runes;
using LoLLauncher.RiotObjects.Platform.Summoner.Spellbook;
using LoLLauncher.RiotObjects.Platform.Game.Map;
using LoLLauncher.RiotObjects.Team;
using LoLLauncher.RiotObjects.Team.Dto;
using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using VoliBot.Utils;
using VoliBot.BaseRegions;
using VoliBot.Utils;
using LoLLauncher.RiotObjects.Platform.Trade;
using VoliBot.LoLLauncher.RiotObjects.Platform.Messaging;
using VoliBot;
using System.Windows.Forms;

namespace RitoBot
{
    internal class OldVoliBot
    {
        public Process exeProcess;
        public GameDTO currentGame = new GameDTO();
        public ChampionDTO[] availableChampsArray;
        public LoginDataPacket loginPacket = new LoginDataPacket();
        public LoLConnection connection = new LoLConnection();
        public List<ChampionDTO> availableChamps = new List<ChampionDTO>();

        public bool firstTimeInLobby = true;
        public bool firstTimeInQueuePop = true;
        public bool firstTimeInCustom = true;
        public bool firstTimeInPostChampSelect = true;
        public bool reAttempt = false;

        public string Accountname;
        public string Password;
        public string ipath;
        public string errorMSG1;
        public string errorMSG2;

        public BaseRegion baseRegion;

        public string sumName { get; set; }
        public double sumId { get; set; }
        public double sumLevel { get; set; }
        public double archiveSumLevel { get; set; }
        public double rpBalance { get; set; }

        public QueueTypes queueType { get; set; }
        public QueueTypes actualQueueType { get; set; }

        public int relogTry = 0;
        public int m_leaverBustedPenalty { get; set; }
        public string m_accessToken { get; set; }

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        //Variables
        public console parent;

        public OldVoliBot(string username, string password, console _parent, QueueTypes queue)
        {
            parent = _parent;
            ipath = parent.lolPath; Accountname = username; Password = password; queueType = queue;
            baseRegion = BaseRegion.GetRegion(_parent.region.ToString());
            connection.OnConnect += new LoLConnection.OnConnectHandler(connection_OnConnect);
            connection.OnDisconnect += new LoLConnection.OnDisconnectHandler(connection_OnDisconnect);
            connection.OnError += new LoLConnection.OnErrorHandler(connection_OnError);
            connection.OnLogin += new LoLConnection.OnLoginHandler(connection_OnLogin);
            connection.OnLoginQueueUpdate += new LoLConnection.OnLoginQueueUpdateHandler(connection_OnLoginQueueUpdate);
            connection.OnMessageReceived += new LoLConnection.OnMessageReceivedHandler(connection_OnMessageReceived);
            string pass = Regex.Replace(password, @"\s+", "");
            connection.Connect(Accountname, pass, baseRegion.PVPRegion, _parent.currentVersion + "." + Config.clientSubVersion);
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
                        parent.updateStatus(msgStatus.ERROR, "Failed to Start", Accountname);
                        break;
                    case "CHAMP_SELECT":
                        firstTimeInCustom = true;
                        firstTimeInQueuePop = true;
                        if (firstTimeInLobby)
                        {
                            firstTimeInLobby = false;
                            updateStatus("In Champion Select", Accountname);
                            object obj = await connection.SetClientReceivedGameMessage(game.Id, "CHAMP_SELECT_CLIENT");
                            if (queueType != QueueTypes.ARAM)
                            {
                                if (parent.championToPick != "" && parent.championToPick != "RANDOM")
                                {

                                    int Spell1;
                                    int Spell2;
                                    if (!parent.randomSpell)
                                    {
                                        Spell1 = Enums.spellToId(parent.spell1);
                                        Spell2 = Enums.spellToId(parent.spell2);
                                    }
                                    else
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

                                        Spell1 = Convert.ToInt32(randomSpell1);
                                        Spell2 = Convert.ToInt32(randomSpell2);
                                    }

                                    await connection.SelectSpells(Spell1, Spell2);

                                    await connection.SelectChampion(Enums.championToId(parent.championToPick));
                                    await connection.ChampionSelectCompleted();
                                }
                                else if (parent.championToPick == "RANDOM")
                                {

                                    int Spell1;
                                    int Spell2;
                                    if (!parent.randomSpell)
                                    {
                                        Spell1 = Enums.spellToId(parent.spell1);
                                        Spell2 = Enums.spellToId(parent.spell2);
                                    }
                                    else
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

                                        Spell1 = Convert.ToInt32(randomSpell1);
                                        Spell2 = Convert.ToInt32(randomSpell2);
                                    }

                                    await connection.SelectSpells(Spell1, Spell2);

                                    var randAvailableChampsArray = availableChampsArray.Shuffle();
                                    await connection.SelectChampion(randAvailableChampsArray.First(champ => champ.Owned || champ.FreeToPlay).ChampionId);
                                    await connection.ChampionSelectCompleted();

                                }
                                else
                                {

                                    int Spell1;
                                    int Spell2;
                                    if (!parent.randomSpell)
                                    {
                                        Spell1 = Enums.spellToId(parent.spell1);
                                        Spell2 = Enums.spellToId(parent.spell2);
                                    }
                                    else
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

                                        Spell1 = Convert.ToInt32(randomSpell1);
                                        Spell2 = Convert.ToInt32(randomSpell2);
                                    }

                                    await connection.SelectSpells(Spell1, Spell2);

                                    await connection.SelectChampion(availableChampsArray.First(champ => champ.Owned || champ.FreeToPlay).ChampionId);
                                    await connection.ChampionSelectCompleted();
                                }
                            }
                            break;
                        }
                        else
                            break;
                    case "POST_CHAMP_SELECT":
                        firstTimeInLobby = false;
                        if (firstTimeInPostChampSelect)
                        {
                            firstTimeInPostChampSelect = false;
                            updateStatus("(Post Champ Select)", Accountname);
                        }
                        break;
                    case "IN_QUEUE":
                        updateStatus("In Queue", Accountname);
                        break;
                    case "TERMINATED":
                        updateStatus("Re-entering queue", Accountname);
                        firstTimeInPostChampSelect = true;
                        firstTimeInQueuePop = true;
                        break;
                    case "JOINING_CHAMP_SELECT":
                        if (this.firstTimeInQueuePop && game.StatusOfParticipants.Contains("1"))
                        {
                            updateStatus("Accepted Queue", Accountname);
                            firstTimeInQueuePop = false;
                            firstTimeInLobby = true;
                            object obj = await connection.AcceptPoppedGame(true);
                            break;
                        }
                        else
                            break;
                    default:
                        updateStatus("[DEFAULT]" + game.GameStateString, Accountname);
                        break;
                }
            }
            else if (message.GetType() == typeof(TradeContractDTO))
            {
                var tradeDto = message as TradeContractDTO;
                if (tradeDto == null)
                    return;
                switch (tradeDto.State)
                {
                    case "PENDING":
                        {
                            if (tradeDto != null)
                                await connection.AcceptTrade(tradeDto.RequesterInternalSummonerName, (int)tradeDto.RequesterChampionId);
                        }
                        break;
                }
                return;
            }
            else if (message is PlayerCredentialsDto)
            {
                firstTimeInPostChampSelect = true;
                PlayerCredentialsDto dto = message as PlayerCredentialsDto;
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.CreateNoWindow = false;
                startInfo.WorkingDirectory = FindLoLExe();
                startInfo.FileName = "League of Legends.exe";
                startInfo.Arguments = "\"8394\" \"LoLLauncher.exe\" \"\" \"" + dto.ServerIp + " " +
                    dto.ServerPort + " " + dto.EncryptionKey + " " + dto.SummonerId + "\"";
                updateStatus("Launching League of Legends\n", Accountname);

                new Thread(() =>
                {
                    exeProcess = Process.Start(startInfo);
                    exeProcess.Exited += new EventHandler(exeProcess_Exited);
                    while (exeProcess.MainWindowHandle == IntPtr.Zero) { }
                    exeProcess.PriorityClass = ProcessPriorityClass.Idle;
                    exeProcess.EnableRaisingEvents = true;
                    //Thread.Sleep(1000);
                }).Start();

            }
            else if (message is EndOfGameStats)
            {
                if (exeProcess != null)
                {
                    exeProcess.Exited -= exeProcess_Exited;
                    exeProcess.Kill();
                    Thread.Sleep(500);
                    if (exeProcess.Responding)
                    {
                        Process.Start("taskkill /F /IM \"League of Legends.exe\"");
                    }
                    loginPacket = await this.connection.GetLoginDataPacketForUser();
                    archiveSumLevel = sumLevel;
                    sumLevel = loginPacket.AllSummonerData.SummonerLevel.Level;
                    if (sumLevel != archiveSumLevel)
                    {
                        levelUp();
                    }
                }
                AttachToQueue();
            }
        }
        private async void AttachToQueue()
        {

            MatchMakerParams matchParams = new MatchMakerParams();
            //Set BotParams
            if (queueType == QueueTypes.INTRO_BOT)
            {
                matchParams.BotDifficulty = "INTRO";
            }
            else if (queueType == QueueTypes.BEGINNER_BOT)
            {
                matchParams.BotDifficulty = "EASY";
            }
            else if (queueType == QueueTypes.MEDIUM_BOT)
            {
                matchParams.BotDifficulty = "MEDIUM";
            }
            //Check if is available to join queue.
            if (sumLevel == 3 && actualQueueType == QueueTypes.NORMAL_5x5)
            {
                queueType = actualQueueType;
            }
            else if (sumLevel == 6 && actualQueueType == QueueTypes.ARAM)
            {
                queueType = actualQueueType;
            }
            else if (sumLevel == 7 && actualQueueType == QueueTypes.NORMAL_3x3)
            {
                queueType = actualQueueType;
            }
            matchParams.QueueIds = new Int32[1] { (int)queueType };
            SearchingForMatchNotification m = await connection.AttachToQueue(matchParams);

            if (m.PlayerJoinFailures == null)
            {
                this.updateStatus("In Queue: " + queueType.ToString(), Accountname);
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
                    else if (failure.ReasonFailed == "LEAVER_BUSTER_TAINTED_WARNING")
                    {
                        //updateStatus("Please login on your LoL Client and type I Agree to the message that comes up.", Accountname);
                         await connection.ackLeaverBusterWarning();
                         await connection.callPersistenceMessaging(new SimpleDialogMessageResponse()
                        {
                            AccountID = loginPacket.AllSummonerData.Summoner.SumId,
                            MsgID = loginPacket.AllSummonerData.Summoner.SumId,
                            Command = "ack"
                        });
                        connection_OnMessageReceived(null, (object)new EndOfGameStats());
                    }
                }

                if (String.IsNullOrEmpty(m_accessToken))
                {
                    // Queue dodger or something else
                }
                else
                {
                    updateStatus("Waiting out leaver buster: " + m_leaverBustedPenalty / 1000 / (float)60 + " minutes!", Accountname);
                    System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(m_leaverBustedPenalty));
                    m = await connection.AttachToLowPriorityQueue(matchParams, m_accessToken);
                    if (m.PlayerJoinFailures == null)
                    {
                        this.updateStatus("Succesfully joined lower priority queue!", Accountname);
                    }
                    else
                    {
                        this.updateStatus("There was an error in joining lower priority queue.\nDisconnecting.", Accountname);
                        connection.Disconnect();
                        parent.lognNewAccount();
                    }
                }
            }
        }
        private void connection_OnLoginQueueUpdate(object sender, int positionInLine)
        {
            if (positionInLine <= 0)
                return;
            updateStatus("Position to login: " + (object)positionInLine, Accountname);
        }
        private void connection_OnLogin(object sender, string username, string ipAddress)
        {
            new Thread((ThreadStart)(async () =>
            {
                updateStatus("Connecting...", Accountname);
                loginPacket = await connection.GetLoginDataPacketForUser();
                if (loginPacket.AllSummonerData == null)
                {
                    Random rnd = new Random();
                    String summonerName = Accountname;
                    if (summonerName.Length > 16)
                        summonerName = summonerName.Substring(0, 11) + new Random().Next(1000, 9999).ToString();
                    updateStatus("Create Summoner: " + summonerName, Accountname);
                    await connection.CreateDefaultSummoner(summonerName);
                }
                await connection.Subscribe("bc", loginPacket.AllSummonerData.Summoner.AcctId);
                await connection.Subscribe("cn", loginPacket.AllSummonerData.Summoner.AcctId);
                await connection.Subscribe("gn", loginPacket.AllSummonerData.Summoner.AcctId);
                sumLevel = loginPacket.AllSummonerData.SummonerLevel.Level;
                sumName = loginPacket.AllSummonerData.Summoner.Name;
                sumId = loginPacket.AllSummonerData.Summoner.SumId;
                rpBalance = loginPacket.RpBalance;
                if (sumLevel > parent.maxLevel || sumLevel == parent.maxLevel)
                {
                    connection.Disconnect();
                    updateStatus("Summoner: " + sumName + " is already max level.", Accountname);
                    updateStatus("Log into new account.", Accountname);
                    parent.lognNewAccount();
                    return;
                }
                if (sumLevel < 3.0 && queueType == QueueTypes.NORMAL_5x5)
                {
                    this.updateStatus("Need to be Level 3 before NORMAL_5x5 queue.", Accountname);
                    this.updateStatus("Joins Co-Op vs AI (Beginner) queue until 3", Accountname);
                    queueType = QueueTypes.BEGINNER_BOT;
                    actualQueueType = QueueTypes.NORMAL_5x5;
                }
                else if (sumLevel < 6.0 && queueType == QueueTypes.ARAM)
                {
                    this.updateStatus("Need to be Level 6 before ARAM queue.", Accountname);
                    this.updateStatus("Joins Co-Op vs AI (Beginner) queue until 6", Accountname);
                    queueType = QueueTypes.BEGINNER_BOT;
                    actualQueueType = QueueTypes.ARAM;
                }
                else if (sumLevel < 7.0 && queueType == QueueTypes.NORMAL_3x3)
                {
                    this.updateStatus("Need to be Level 7 before NORMAL_3x3 queue.", Accountname);
                    this.updateStatus("Joins Co-Op vs AI (Beginner) queue until 7", Accountname);
                    queueType = QueueTypes.BEGINNER_BOT;
                    actualQueueType = QueueTypes.NORMAL_3x3;
                }
                if ((loginPacket.AllSummonerData.Summoner.ProfileIconId == -1 || loginPacket.AllSummonerData.Summoner.ProfileIconId == 1))
                {
                    double[] ids = new double[Convert.ToInt32(sumId)];
                    string icons = await connection.GetSummonerIcons(ids);
                    List<int> availableIcons = new List<int> { };
                    var random = new Random();
                    for (int i = 0; i < 29; i++)
                    {
                        availableIcons.Add(i);
                    }
                    foreach (var id in icons)
                    {
                        availableIcons.Add(Convert.ToInt32(id));
                    }
                    int index = random.Next(availableIcons.Count);
                    int randomIcon = availableIcons[index];
                    await connection.UpdateProfileIconId(randomIcon);
                }
                if (rpBalance == 400.0 && parent.buyBoost && sumLevel < 5)
                {
                    updateStatus("Buying XP Boost", Accountname);
                    try
                    {
                        Task t = new Task(buyBoost);
                        t.Start();
                    }
                    catch (Exception exception)
                    {
                        updateStatus("Couldn't buy RP Boost.\n" + exception, Accountname);
                    }
                }
                updateStatus("Logged in as " + loginPacket.AllSummonerData.Summoner.Name + " @ level " + loginPacket.AllSummonerData.SummonerLevel.Level, Accountname);
                availableChampsArray = await connection.GetAvailableChampions();
                PlayerDTO player = await connection.CreatePlayer();
                if (loginPacket.ReconnectInfo != null && loginPacket.ReconnectInfo.Game != null)
                {
                    connection_OnMessageReceived(sender, (object)loginPacket.ReconnectInfo.PlayerCredentials);
                }
                else
                    connection_OnMessageReceived(sender, (object)new EndOfGameStats());
            })).Start();
        }
        private void connection_OnError(object sender, LoLLauncher.Error error)
        {
            if (error.Type == ErrorType.AuthKey || error.Type == ErrorType.General)
            {
                if (reAttempt)
                {
                    return;
                }
                updateStatus("Unable to connect. Try one reconnect.", Accountname);
                reAttempt = true;
                connection.Connect(Accountname, Password, baseRegion.PVPRegion, Config.clientSeason  +"." + Config.clientSubVersion);
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
            if (error.Message.Contains("Unable to get Auth Key"))
            {
                updateStatus("Login Failed", Accountname);
                return;
            }
            updateStatus("["+ error.Type +"]error received:\n" + error.Message, Accountname);
        }
        private void connection_OnDisconnect(object sender, EventArgs e)
        {
            updateStatus("Disconnected", Accountname);
        }
        private void connection_OnConnect(object sender, EventArgs e)
        {
        }
        async void exeProcess_Exited(object sender, EventArgs e)
        {
            updateStatus("Restart League of Legends.", Accountname);
            loginPacket = await connection.GetLoginDataPacketForUser();
            if (this.loginPacket.ReconnectInfo != null && this.loginPacket.ReconnectInfo.Game != null)
            {
                this.connection_OnMessageReceived(sender, (object)this.loginPacket.ReconnectInfo.PlayerCredentials);
            }
            else
                this.connection_OnMessageReceived(sender, (object)new EndOfGameStats());
        }
        private void updateStatus(string status, string accname)
        {
            parent.updateStatus(msgStatus.INFO, status, accname);
            
        }
        private void levelUp()
        {
            updateStatus("Level Up: " + sumLevel, Accountname);
            rpBalance = loginPacket.RpBalance;
            if (sumLevel >= parent.maxLevel)
            {
                connection.Disconnect();
                if (!connection.IsConnected())
                {
                    parent.lognNewAccount();
                }
            }
            if (rpBalance == 400.0 && parent.buyBoost && sumLevel < 5)
            {
                updateStatus("Buying XP Boost", Accountname);
                try
                {
                    Task t = new Task(buyBoost);
                    t.Start();
                }
                catch (Exception exception)
                {
                    updateStatus("Couldn't buy RP Boost.\n" + exception, Accountname);
                }
            }
        }
        private async void buyBoost()
        {
            try
            {
                string url = await connection.GetStoreUrl();
                HttpClient httpClient = new HttpClient();
                await httpClient.GetStringAsync(url);

                string storeURL = "https://store." + baseRegion.ChatName + ".lol.riotgames.com/store/tabs/view/boosts/1";
                await httpClient.GetStringAsync(storeURL);

                string purchaseURL = "https://store." + baseRegion.ChatName + ".lol.riotgames.com/store/purchase/item";

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

                updateStatus("Bought 'XP Boost: 3 Days'!", Accountname);
                httpClient.Dispose();
            }
            catch (Exception e)
            {
                parent.updateStatus(msgStatus.ERROR, e.Message, Accountname);
            }
        }
        private String FindLoLExe()
        {
            String installPath = ipath;
            if (installPath.Contains("notfound"))
                return installPath;
            installPath += @"RADS\solutions\lol_game_client_sln\releases\";
            installPath = Directory.EnumerateDirectories(installPath).OrderBy(f => new DirectoryInfo(f).CreationTime).Last();
            installPath += @"\deploy\";
            return installPath;
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
}