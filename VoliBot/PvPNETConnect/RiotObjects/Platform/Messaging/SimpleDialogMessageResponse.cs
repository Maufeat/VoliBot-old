using LoLLauncher;
using LoLLauncher.RiotObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoliBot.LoLLauncher.RiotObjects.Platform.Messaging
{

    public class SimpleDialogMessageResponse : RiotGamesObject
    {
        private string type = "com.riotgames.platform.messaging.persistence.SimpleDialogMessageResponse";
        private SimpleDialogMessageResponse.Callback callback;

        public override string TypeName
        {
            get
            {
                return this.type;
            }
        }

        [InternalName("msgId")]
        public double MsgID { get; set; }

        [InternalName("accountId")]
        public double AccountID { get; set; }

        [InternalName("command")]
        public string Command { get; set; }

        public SimpleDialogMessageResponse()
        {
        }

        public SimpleDialogMessageResponse(SimpleDialogMessageResponse.Callback callback)
        {
            this.callback = callback;
        }

        public SimpleDialogMessageResponse(TypedObject result)
        {
            this.SetFields<SimpleDialogMessageResponse>(this, result);
        }

        public override void DoCallback(TypedObject result)
        {
            this.SetFields<SimpleDialogMessageResponse>(this, result);
            this.callback(this);
        }

        public delegate void Callback(SimpleDialogMessageResponse result);
    }
}
