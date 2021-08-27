using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Victoria;

namespace DiscordSCLDbot215.Handlers
{
    class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _services;
        //private readonly LavaNode _lavaNode;

        /* 取得任何我們需要的 DI. */
        public CommandHandler(IServiceProvider services, LavaNode lavaNode)
        {
            _commands = services.GetRequiredService<CommandService>();
            _client = services.GetRequiredService<DiscordSocketClient>();
            _services = services;
            //_lavaNode = lavaNode;


            HookEvents();
        }

        /* 初始化 CommandService. */
        public async Task InitializeAsync()
        {
            await _commands.AddModulesAsync(
                assembly: Assembly.GetEntryAssembly(),
                services: _services);
            //_client.Ready += _client_Ready;
        }

        /*
        private async Task _client_Ready()
        {
            if (!_lavaNode.IsConnected)
            {
                await _lavaNode.ConnectAsync();
                if (_lavaNode.IsConnected)
                    Console.WriteLine("有連線");
                else
                    Console.WriteLine("沒連線");
            }
        }
        */
        /* 勾住指令特殊事件 */
        public void HookEvents()
        {
            _client.MessageReceived += HandleCommandAsync;
            _commands.CommandExecuted += CommandExecutedAsync;
            _commands.Log += LogAsync;
        }


        /* 當 MessageRecived 事件觸發後. 在這邊處裡訊息. */
        private Task HandleCommandAsync(SocketMessage socketMessage)
        {
            var argPos = 0;
            //確認指令是否正常，忽略不需要關心的事，私人訊息或是其他機器人的訊息
            if (!(socketMessage is SocketUserMessage message) || message.Author.IsBot || message.Author.IsWebhook || message.Channel is IPrivateChannel)
                return Task.CompletedTask;

            /* 驗證前字元 */
            if (!message.HasStringPrefix(SCLD.DefaultPrefix, ref argPos))
                return Task.CompletedTask;

            /* 建立指令內容的模塊 */
            var context = new SocketCommandContext(_client, socketMessage as SocketUserMessage);

            /*
            // 不收指令的頻道 
            var blacklistedChannelCheck = from a in GlobalData.Config.BlacklistedChannels
                                          where a == context.Channel.Id
                                          select a;
            var blacklistedChannel = blacklistedChannelCheck.FirstOrDefault();

            // 驗證頻道. 
            
            if (blacklistedChannel == context.Channel.Id)
            {
                return Task.CompletedTask;
            }
            else
            */

            var result = _commands.ExecuteAsync(context, argPos, _services, MultiMatchHandling.Best);

            /* Report any errors if the command didn't execute succesfully. */
            //if (!result.Result.IsSuccess)
            //{
            //    context.Channel.SendMessageAsync(result.Result.ErrorReason);
            //}

            /* If everything worked fine, command will run. */
            return result;
        }

        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            /* command is unspecified when there was a search failure (command not found); we don't care about these errors */
            if (!command.IsSpecified)
                return;

            /* the command was succesful, we don't care about this result, unless we want to log that a command succeeded. */
            if (result.IsSuccess)
                return;

            /* the command failed, let's notify the user that something happened. */
            //await context.Channel.SendMessageAsync($"error: {result}");
        }

        /* Used whenever we want to log something to the Console. 
            Todo: Hook in a Custom LoggingService. */
        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());

            return Task.CompletedTask;
        }




        /*
        private DiscordSocketClient _cliemt;

        private CommandService _service;

        private readonly AudioService _serviceA;



        public CommandHandler(DiscordSocketClient cliemt)
        {
            _cliemt = cliemt;

            _service = new CommandService();

            _service.AddModulesAsync(Assembly.GetEntryAssembly(),null);

            _cliemt.MessageReceived += HandleCommandAsync;

            _serviceA = new AudioService();
        }


        /// <summary>
        /// 指令事件
        /// </summary>
        /// <param name="s">訊息的物件</param>
        /// <returns></returns>
        private async Task HandleCommandAsync(SocketMessage s)
        {
            
            SocketUserMessage msg = s as SocketUserMessage;
            if (msg == null) return;


            //if (msg.Author.Id == 511469478391971851 && msg.Content.ToString().Equals("測試成功!!"))
            //    await msg.DeleteAsync();

            if (msg.Author.Id == 511469478391971851) return;  //自己的訊息忽略




            var context = new SocketCommandContext(_cliemt, msg);


            if (context.Message.ToString() == "幻境來")
            {
                await _serviceA.JoinAudio(context.Guild, (context.User as IVoiceState).VoiceChannel);
            }
            if (context.Message.ToString() == "幻境播")
            {
                Console.WriteLine("收到播放");
                await _serviceA.SendAudioAsync(context.Guild, context.Channel, @"D:\程式\DiscordSCLDbot215\DiscordSCLDbot215\bin\Debug\netcoreapp2.1\Butter-Fly.mp3");
            }
            if (context.Message.ToString() == "幻境來播")
            {
                Console.WriteLine("收到來播");
                await _serviceA.JoinAudio(context.Guild, (context.User as IVoiceState).VoiceChannel);
                await _serviceA.SendAudioAsync(context.Guild, context.Channel, @"D:\程式\DiscordSCLDbot215\DiscordSCLDbot215\bin\Debug\netcoreapp2.1\Butter-Fly.mp3");
            }


            #region 執行指令 + 打錯指令的回應
            int argPos = 0;
            if (msg.HasCharPrefix('!', ref argPos))
            {
                var result = await _service.ExecuteAsync(context, argPos, null);

                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                {
                    //await context.Channel.SendMessageAsync(result.ErrorReason);
                }

                if (!result.IsSuccess)
                {
                    //await context.Channel.SendMessageAsync(result.ErrorReason);
                }

            }
            #endregion
            
        }*/
    }
}
