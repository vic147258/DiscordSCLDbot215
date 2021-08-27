using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Victoria;

namespace DiscordSCLDbot215.DataStructs
{
    class Trackforjson
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

        [JsonConstructor]
        public Trackforjson(string author, bool canSeek, TimeSpan duration, string hash, string id, bool isStream, TimeSpan position, string title, string url)
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
        }

    }
}
