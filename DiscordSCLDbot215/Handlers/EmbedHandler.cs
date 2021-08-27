using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordSCLDbot215.Handlers
{
    public static class EmbedHandler
    {
        /* This file is where we can store all the Embed Helper Tasks (So to speak). 
             We wrap all the creations of new EmbedBuilder's in a Task.Run to allow us to stick with Async calls. 
             All the Tasks here are also static which means we can call them from anywhere in our program. */
        public static async Task<Embed> CreateBasicEmbed(string title, string description, Color color)
        {
            var embed = await Task.Run(() => (new EmbedBuilder()
                .WithTitle(title)
                .WithDescription(description)
                .WithColor(color)
                .WithCurrentTimestamp()
                .Build()));
            return embed;
        }

        public static async Task<Embed> CreateBasicEmbed(string title, string description, Color color, string imgurl)
        {
            //圖片的
            var embed = await Task.Run(() => (new EmbedBuilder()
                .WithTitle(title)
                //.WithImageUrl(uurl)
                .WithThumbnailUrl(imgurl)
                .WithDescription(description)
                .WithColor(color)
                .WithCurrentTimestamp()
                .Build()));
            return embed;
        }

        public static async Task<Embed> CreateBasicEmbed(string title, string description, Color color, EmbedAuthorBuilder EAB, List<EmbedFieldBuilder> table, string imgurl)
        {
            //圖片的
            var embed = await Task.Run(() => (new EmbedBuilder()
                .WithTitle(title)
                //.WithImageUrl(uurl)
                .WithThumbnailUrl(imgurl)
                .WithAuthor(EAB)
                .WithFields(table)
                .WithDescription(description)
                .WithColor(color)
                .WithCurrentTimestamp()
                .Build()));
            return embed;
        }

        public static async Task<Embed> CreateErrorEmbed(string source, string error)
        {
            var embed = await Task.Run(() => new EmbedBuilder()
                .WithTitle($"錯誤 - {source}")
                .WithDescription($"**錯誤詳情**： \n{error}")
                .WithColor(Color.DarkRed)
                .WithCurrentTimestamp()
                .Build());
            return embed;
        }
    }
}
