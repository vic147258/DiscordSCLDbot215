using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Victoria;

namespace DiscordSCLDbot215.DataStructs
{
    class Hotforjson
    {
        public string Author { get; set; }
        public bool CanSeek { get; set; }
        public TimeSpan Duration { get; set; }
        public string Hash { get; set; }
        public string Id { get; set; }
        public bool IsStream { get; set; }
        public TimeSpan Position { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }

        public int Count { get; set; }
        public DateTime Date { get; set; }
        public ulong Order_user_id { get; set; }

        [JsonConstructor]
        public Hotforjson(string author, bool canSeek, TimeSpan duration, string hash, string id, bool isStream, TimeSpan position, string title, string url, DateTime date, ulong order_user_id)
        {
            Author = author;
            CanSeek = canSeek;
            Duration = duration;
            Hash = hash;
            Id = id;
            IsStream = isStream;
            Position = position;
            Title = title;
            Url = url;

            Date = date;
            Order_user_id = order_user_id;
        }

        public Hotforjson(LavaTrack LT, DateTime date, ulong order_user_id)
        {
            Author = LT.Author;
            CanSeek = LT.CanSeek;
            Duration = LT.Duration;
            Hash = LT.Hash;
            Id = LT.Id;
            IsStream = LT.IsStream;
            Position = LT.Position;
            Title = LT.Title;
            Url = LT.Url;

            Date = date;
            Order_user_id = order_user_id;
        }
    }
}
