using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using DiscordSCLDbot215.Modules;
using DiscordSCLDbot215.Handlers;
using System;
using System.Threading.Tasks;
using Victoria;

namespace DiscordSCLDbot215.Services
{
    public class DiscordService
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandHandler _commandHandler;
        private readonly ServiceProvider _services;
        private readonly UserJoinedHandler _UJhandler;
        private readonly LavaNode _lavaNode;
        private readonly LavaLinkAudio _audioService;

        public DiscordService()
        {
            _services = ConfigureServices();
            _client = _services.GetRequiredService<DiscordSocketClient>();
            _commandHandler = _services.GetRequiredService<CommandHandler>();
            _UJhandler = _services.GetRequiredService<UserJoinedHandler>();
            _lavaNode = _services.GetRequiredService<LavaNode>();
            _audioService = _services.GetRequiredService<LavaLinkAudio>();

            SubscribeLavaLinkEvents();
            SubscribeDiscordEvents();
        }

        /* 初始化 Discord Client. */
        public async Task InitializeAsync()
        {
            Log_text(SCLD.version);
            Log_text("「" + SCLD.bot_name + "」" + "正在登入");
            await _client.LoginAsync(TokenType.Bot, SCLD.bot_Token);
            await _client.StartAsync();
            Log_text("「" + SCLD.bot_name + "」" + "登入成功，開始招待");

            await _commandHandler.InitializeAsync();

            await Task.Delay(-1);
        }

        /* Hook Any Client Events Up Here. */
        private void SubscribeLavaLinkEvents()
        {
            _lavaNode.OnLog += LogAsync;
            _lavaNode.OnTrackEnded += _audioService.TrackEnded;
            _lavaNode.OnTrackStarted += _audioService.TrackStarted;
        }

        private void SubscribeDiscordEvents()
        {
            _client.Ready += ReadyAsync;
            _client.Log += LogAsync;
        }


        /* Used when the Client Fires the ReadyEvent. */
        private async Task ReadyAsync()
        {
            try
            {
                if (!_lavaNode.IsConnected)
                {
                    await _lavaNode.ConnectAsync();
                    if (_lavaNode.IsConnected)
                        await LoggingService.LogInformationAsync("Music", $"與 LavaLink 連接成功");
                    else
                        await LoggingService.LogInformationAsync("Music", $"與 LavaLink 連接失敗");
                }
                
                await _client.SetGameAsync(SCLD.game_state);
            }
            catch (Exception ex)
            {
                await LoggingService.LogInformationAsync(ex.Source, ex.Message);
            }
        }

        /* Used whenever we want to log something to the Console. 
            Todo: Hook in a Custom LoggingService. */
        private async Task LogAsync(LogMessage logMessage)
        {
            //Console.WriteLine("Source: " + logMessage.Source);
            //Console.WriteLine("Severity: " + logMessage.Severity);
            //Console.WriteLine("Message: " + logMessage.Message);
            if (logMessage.Severity != LogSeverity.Debug)
                await LoggingService.LogAsync(logMessage.Source, logMessage.Severity, logMessage.Message);
        }

        /* Configure our Services for Dependency Injection. */
        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandler>()
                .AddSingleton<UserJoinedHandler>()
                .AddSingleton<LavaLinkAudio>()
                //.AddSingleton<MusicModule>()
                //.AddSingleton<LavaNode>()
                //.AddSingleton<LavaConfig>()
                .AddLavaNode(x => {
                    x.SelfDeaf = false;  //true就聽不到
                    x.Authorization = "rethertherhrt";
                })
                //.AddSingleton<GlobalData>()
                .BuildServiceProvider();
        }

        private void Log_text(String content)
        {
            //IRole role = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Super OP");  //抓到 Super OP 的身分
            string timestamp = "[" + DateTime.Now.ToString("HH:mm:ss") + "] ";
            string logtext = timestamp + content;
            new SCLD_tools().write_log_file(logtext);
            Console.WriteLine(logtext);
        }

    }
}
