#region

using LoLLauncher;
using System;
using System.Net;

#endregion

namespace VoliBot.BaseRegions
{
    public class TR : BaseRegion
    {
        public override string RegionName
        {
            get { return "TR"; }
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
            get { return "TR1"; }
        }

        public override string ChatName
        {
            get { return "tr"; }
        }

        public override string Locale
        {
            get { return "en_US"; }
        }

        public override Uri NewsAddress
        {
            get { return new Uri("http://tr.leagueoflegends.com/tr/rss.xml"); }
            //This returns english (not spanish) characters
        }

        public override RegioN PVPRegion
        {
            get { return RegioN.TR; }
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
            get { return new Uri("http://spectator.tr.lol.riotgames.com:80/observer-mode/rest/"); }
        }

        public override string SpectatorIpAddress
        {
            get { return "95.172.65.242:80"; }
            set { }
        }
    }
}