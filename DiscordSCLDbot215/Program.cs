using Discord;
using Discord.Audio;
using Discord.Rest;
using Discord.WebSocket;
using DiscordSCLDbot215.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;


/*
 * 有裝的 Nuget
 * Discord.Net
 * Discord.Net.Commands
 * Discord.Net.Core
 * Discord.Net.WebSocket
 * 
 * 
 * 
 */


namespace DiscordSCLDbot215
{
    class Program
    {
        private static Task Main()
            => new DiscordService().InitializeAsync();

        /*
        static void Main(string[] args)
       => new Program().StartAsync().GetAwaiter().GetResult();


        
        private DiscordSocketClient _client;

        private CommandHandler _Chandler;

        private UserJoinedHandler _UJhandler;


        private async Task StartAsync()
        {

            Console.WriteLine(SCLD.version);
            Console.WriteLine("「"+SCLD.bot_name + "」" + "正在登入");

            _client = new DiscordSocketClient();

            await _client.LoginAsync(TokenType.Bot, SCLD.bot_Token);

            await _client.StartAsync();
            //await _client.StopAsync();

            Console.WriteLine("「" + SCLD.bot_name + "」" + "登入成功");

            Console.WriteLine("「" + SCLD.bot_name + "」" + "開始招待");

            await _client.SetGameAsync("esports.skyey.tw");

            //附加文字擷取
            _Chandler = new CommandHandler(_client);

            _UJhandler = new UserJoinedHandler(_client);

            await Task.Delay(-1);
        }
        */
    }
}
