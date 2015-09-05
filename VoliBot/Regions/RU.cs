#region

using LoLLauncher;
using System;
using System.Net;

#endregion

namespace VoliBot.BaseRegions
{
    public class RU : BaseRegion
    {
        public override string RegionName
        {
            get { return "RU"; }
        }

        public override string Location
        {
            get { return null; }
        }

        public override bool Garena
        {
            get { return false; }
        }

        public override string InternalName
        {
            get { return "RU"; }
        }

        public override string ChatName
        {
            get { return "ru"; }
        }

        public override string Locale
        {
            get { return "en_US"; }
        }

        public override Uri NewsAddress
        {
            get { return new Uri("http://ru.leagueoflegends.com/ru/rss.xml"); }
        }

        public override RegioN PVPRegion
        {
            get { return RegioN.RU; }
        }

        public override IPAddress[] PingAddresses
        {
            get
            {
                return new IPAddress[]
                {
                    //No known IP address
                };
            }
        }

        public override Uri SpectatorLink
        {
            get { return new Uri("http://spectator.ru.lol.riotgames.com/observer-mode/rest/"); }
        }

        public override string SpectatorIpAddress
        {
            get { return "95.172.65.242"; }
            set { }
        }
    }
}