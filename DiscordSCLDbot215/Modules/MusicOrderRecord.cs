using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordSCLDbot215
{
    class MusicOrderRecord
    {
        private String _SearchText = "N";
        private DateTime _Time = DateTime.Now;

        public String SearchText
        {
            get { return _SearchText; }
            set { _SearchText = value; }
        }

        public DateTime Time
        {
            get { return _Time; }
            set { _Time = value; }
        }
    }
}
