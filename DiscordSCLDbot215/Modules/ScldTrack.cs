using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Victoria;

namespace DiscordSCLDbot215.Modules
{
    class ScldTrack
    {
        public LavaTrack _Track;
        public SocketGroupUser _orderuset;


        public LavaTrack Track
        {
            get { return _Track; }
            set { _Track = value; }
        }

        public SocketGroupUser orderuset
        {
            get { return _orderuset; }
            set { _orderuset = value; }
        }
    }
}
