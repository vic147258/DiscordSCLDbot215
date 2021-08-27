using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordSCLDbot215.Handlers;
using DiscordSCLDbot215.Services;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DiscordSCLDbot215.Modules
{
    public class Command_list : ModuleBase<SocketCommandContext>
    {
        String br = Convert.ToChar(13).ToString() + Convert.ToChar(10).ToString(); //打/n也可以
                                                                                   //String tab = Convert.ToChar(9).ToString();

        #region 說明系列


        [Command("版本", RunMode = RunMode.Async)]
        [Alias("v", "版")]
        public async Task the_version()
        {
            Log_text("叫版本", Context.User);

            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (check_Role(Context.User, SCLD.SCLD_OPid)) is_permission += 500;
            if (is_permission == 0) return;
            #endregion

            await Context.Message.DeleteAsync();

            IUserMessage text_complete = await Context.Channel.SendMessageAsync(SCLD.version);

            await Task.Delay(10000);
            
            await text_complete.DeleteAsync();
        }

        [Command("測試", RunMode = RunMode.Async)]
        [Alias("test")]
        public async Task textcommmmmmd([Remainder]string asdfg)
        {
            Log_text("測試", Context.User);

            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (check_Role(Context.User, SCLD.SCLD_OPid)) is_permission += 500;
            if (is_permission == 0) return;
            #endregion

            //(Context.User as SocketUser).AvatarId;
            //https://i.ytimg.com/vi/{wwwwwwwww}/hqdefault.jpg
            //$"https://cdn.discordapp.com/avatars/{Context.User.Id}/{Context.User.AvatarId}.png?size=16"


            string Embed_title = "崁入的標題";
            string Embed_description = "內文內文內文內文";

            EmbedAuthorBuilder EAuthor = new EmbedAuthorBuilder() { Name = (Context.User as SocketGuildUser).Nickname, IconUrl = $"https://cdn.discordapp.com/avatars/{Context.User.Id}/{Context.User.AvatarId}.png?size=1024" };

            List<EmbedFieldBuilder> Embed_table = new List<EmbedFieldBuilder>();
            Embed_table.Add(new EmbedFieldBuilder() { Name = "標題1", Value = "幹你娘幹你娘幹你娘幹你娘幹你娘幹你娘幹你娘幹你娘", IsInline = true });
            Embed_table.Add(new EmbedFieldBuilder() { Name = "標題2", Value = "操你嗎操你嗎操你嗎操你嗎操你嗎操你嗎操你嗎操你嗎", IsInline = true });
            Embed_table.Add(new EmbedFieldBuilder() { Name = "標題3", Value = "凱凱凱凱凱凱凱凱凱凱凱凱凱凱", IsInline = true });

            string ThumbnailUrl = $"https://i.ytimg.com/vi/{asdfg}/hqdefault.jpg";

            await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed(Embed_title, Embed_description, Color.Green, EAuthor, Embed_table, ThumbnailUrl));

            IUserMessage text_complete = await Context.Channel.SendMessageAsync(asdfg);

            await Task.Delay(10000);
            await Context.Message.DeleteAsync();
            await text_complete.DeleteAsync();
        }



        [Command("help", RunMode = RunMode.Async)]
        [Alias("說明", "幫助", "?")]
        public async Task get_help()
        {
            Log_text("顯示指令清單", Context.User);

            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (check_Role(Context.User, SCLD.SCLD_OPid)) is_permission += 500;
            if (is_permission == 0) return;
            #endregion

            String textallcomm = "";
            /*
            textallcomm += "!說明 | 查詢所有指令\n";
            textallcomm += "!身分 | 查詢您的身分\n";
            textallcomm += "!身分 @<User> | 查詢某使用者的身分\n";
            textallcomm += "!重複 <內容> | 重複您的內容\n";
            textallcomm += "!重複 #<頻道> <內容> | 重複您的內容\n";
            textallcomm += "!歡迎 | 顯示歡迎內容\n";
            textallcomm += "!歡迎 @<User>  | 顯示歡迎某使用者的內容\n";
            textallcomm += "!更新歡迎 <內容> | 更新歡迎內容；{User}=使用者名稱、{Guild}=群名稱\n";
            textallcomm += "!紀錄 <幾條> | 查詢最近的幾條訊息\n";
            textallcomm += "!紀錄 @<User> <幾條> | 查詢某使用者最近的幾條訊息\n";
            textallcomm += "!強調 <訊息ID> | 顯示某個使用者說過的話\n";
            textallcomm += "!強調 #<頻道> <訊息ID> | 顯示在某個頻道的某個使用者說過的話\n";
            textallcomm += "!清除 <幾條> | 刪除最近的幾條訊息\n";
            textallcomm += "!清除 @<User> <幾條> | 刪除某使用者最近的幾條訊息\n";
            textallcomm += "!清除 #<頻道> <幾條> | 刪除某頻道最近的幾條訊息\n";
            textallcomm += "!清除 #<頻道> @<User> <幾條> | 刪除某頻道的某個人的最近的幾條訊息\n";
            textallcomm += "!修改 <文字ID> <內容> | 修改某條訊息\n";
            */
            textallcomm += "請參閱：http://go.skyey.tw/sscld";


            IUserMessage text_complete = await ReplyAsync(textallcomm);
            await Context.Message.DeleteAsync();

            await Task.Delay(10000);
            await text_complete.DeleteAsync();
        }

        #endregion

        #region 學人說話系列

        //重複 [內容]
        [Command("重複")]
        public async Task repeat_text([Remainder]String the_text)
        {
            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (is_permission == 0) return;
            #endregion

            Log_text("重複", Context.User);

            await repeat_text(Context.Channel as SocketTextChannel, the_text);
        }

        //重複 [頻道] [內容]
        [Command("重複")]
        public async Task repeat_text(SocketTextChannel textChannel, [Remainder]String the_text)
        {
            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (is_permission == 0) return;
            #endregion

            Log_text("重複", Context.User);
            await textChannel.SendMessageAsync(the_text);
            await Context.Message.DeleteAsync();
        }

        #endregion

        #region 更新 GTA 的 Line 通知系列

        //更新GTA提醒
        [Command("修改GTA提醒", RunMode = RunMode.Async)]
        public async Task update_GTA_line2([Remainder]String the_text) { await update_welcome_personal(the_text); }
        [Command("更新GTA提醒", RunMode = RunMode.Async)]
        public async Task update_GTA_line([Remainder]String the_text)
        {
            Log_text("更新GTA提醒", Context.User);

            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (is_permission == 0) return;
            #endregion

            System.IO.File.WriteAllText("GTA_to_line.txt", the_text);

            IUserMessage text_complete = await Context.Channel.SendMessageAsync("（已更新 GTA 的通知訊息）");
            await Context.Message.DeleteAsync();
            await Task.Delay(3000);
            await text_complete.DeleteAsync();

            //存好以後顯示

            await show_GTA_line();
            /*
            String textfileall = "以下是您更新後的預覽 (GTA通知)\n------------------------\n";
            String[] textfile = System.IO.File.ReadAllLines("GTA_to_line.txt");

            for (int i = 0; i < textfile.Length; i++)
            {
                textfileall += textfile[i].Replace("{User}", Context.Message.Author.Mention).Replace("{Guild}", (Context.Message.Author as SocketGuildUser).Guild.Name) + "\n";
            }

            await Context.Channel.SendMessageAsync(textfileall);*/
        }

        //顯示GTA提醒
        [Command("顯示GTA提醒", RunMode = RunMode.Async)]
        public async Task show_GTA_line()
        {
            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (is_permission == 0) return;
            #endregion

            String textfileall = "以下是您更新後的預覽 ( GTA 通知)\n------------------------\n";
            String[] textfile = System.IO.File.ReadAllLines("GTA_to_line.txt");

            for (int i = 0; i < textfile.Length; i++)
            {
                SocketGuildUser cxus = Context.Guild.Users.FirstOrDefault(x => x.Id == Context.Message.Author.Id);
                textfileall += textfile[i].Replace("{User}", Context.Message.Author.Username).Replace("{Nick}", cxus.Nickname).Replace("{Guild}", (Context.Message.Author as SocketGuildUser).Guild.Name) + "\n";
            }

            await Context.Channel.SendMessageAsync(textfileall);
        }

        #endregion

        #region 更新歡迎私訊系列

        //更新歡迎
        [Command("修改歡迎私訊", RunMode = RunMode.Async)]
        public async Task update_welcome_personal2([Remainder]String the_text) { await update_welcome_personal(the_text); }
        [Command("更新歡迎私訊", RunMode = RunMode.Async)]
        public async Task update_welcome_personal([Remainder]String the_text)
        {
            Log_text("更新歡迎私訊", Context.User);

            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (is_permission == 0) return;
            #endregion

            System.IO.File.WriteAllText("wwwwpersonal.txt", the_text);

            IUserMessage text_complete = await Context.Channel.SendMessageAsync("（已更新歡迎私人訊息）");
            await Context.Message.DeleteAsync();
            await Task.Delay(3000);
            await text_complete.DeleteAsync();

            //存好以後顯示

            String textfileall = "以下是您更新後的預覽 (私人訊息)\n------------------------\n";
            String[] textfile = System.IO.File.ReadAllLines("wwwwpersonal.txt");

            for (int i = 0; i < textfile.Length; i++)
            {
                textfileall += textfile[i].Replace("{User}", Context.Message.Author.Mention).Replace("{Guild}", (Context.Message.Author as SocketGuildUser).Guild.Name) + "\n";
            }

            await Context.Channel.SendMessageAsync(textfileall);
        }

        #endregion

        #region 更新 隨機罵人 清單

        //更新隨機罵人
        [Command("修改罵人", RunMode = RunMode.Async)]
        public async Task update_random_fuck2([Remainder]String the_text) { await update_random_fuck(the_text); }
        [Command("更新罵人", RunMode = RunMode.Async)]
        public async Task update_random_fuck([Remainder]String the_text)
        {
            Log_text("更新罵人清單", Context.User);

            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (is_permission == 0) return;
            #endregion

            System.IO.File.WriteAllText("fuck_list.txt", the_text);

            IUserMessage text_complete = await Context.Channel.SendMessageAsync("（已更新罵人的清單）");
            await Context.Message.DeleteAsync();
            await Task.Delay(3000);
            await text_complete.DeleteAsync();

            //存好以後顯示

            await show_random_fuck();

        }

        //顯示罵人內容
        [Command("顯示罵人", RunMode = RunMode.Async)]
        public async Task show_random_fuck()
        {
            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (is_permission == 0) return;
            #endregion

            String textfileall = "以下是您更新後的預覽 (罵人清單)\n------------------------\n";
            String[] textfile = System.IO.File.ReadAllLines("fuck_list.txt");

            for (int i = 0; i < textfile.Length; i++)
                textfileall += textfile[i] + "\n";

            await Context.Channel.SendMessageAsync(textfileall);
        }

        #endregion

        #region 更新 隨機問候 清單

        //更新隨機問候
        [Command("修改問候", RunMode = RunMode.Async)]
        public async Task update_random_Greetings2([Remainder]String the_text) { await update_random_Greetings(the_text); }
        [Command("更新問候", RunMode = RunMode.Async)]
        public async Task update_random_Greetings([Remainder]String the_text)
        {
            Log_text("更新問候清單", Context.User);

            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (is_permission == 0) return;
            #endregion

            System.IO.File.WriteAllText("Greetings_list.txt", the_text);

            IUserMessage text_complete = await Context.Channel.SendMessageAsync("（已更新問候的清單）");
            await Context.Message.DeleteAsync();
            await Task.Delay(3000);
            await text_complete.DeleteAsync();

            //存好以後顯示

            await show_random_Greetings();

        }

        //顯示問候內容
        [Command("顯示問候", RunMode = RunMode.Async)]
        public async Task show_random_Greetings()
        {
            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (is_permission == 0) return;
            #endregion

            String textfileall = "以下是您更新後的預覽 (問候清單)\n------------------------\n";
            String[] textfile = System.IO.File.ReadAllLines("Greetings_list.txt");

            for (int i = 0; i < textfile.Length; i++)
            {
                textfileall += textfile[i] + "\n";
            }

            await Context.Channel.SendMessageAsync(textfileall);
        }

        #endregion

        #region 語音系列

        /*
        [Command("語音+", RunMode = RunMode.Async)]
        public async Task JoinVoiceChannel1() { await JoinVoiceChannel(); }

        [Command("幻境來", RunMode = RunMode.Async)]
        public async Task JoinVoiceChannel()
        {
            Log_text("叫幻境來頻道", Context.User);

            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (check_Role(Context.User, SCLD.SCLD_OPid)) is_permission += 500;
            if (Context.User.Id == SCLD.vicid) is_permission += 999;
            if (is_permission == 0) return;
            #endregion

            await ReplyAsync(embed: await AudioService.JoinAsync(Context.Guild, Context.User as IVoiceState, Context.Channel as ITextChannel));

            await Task.Delay(3000);

        }


        [Command("幻境走", RunMode = RunMode.Async)]
        public async Task leaveVoiceChannel()
        {
            Log_text("叫幻境離開頻道", Context.User);

            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (check_Role(Context.User, SCLD.SCLD_OPid)) is_permission += 500;
            if (is_permission == 0) return;
            #endregion

            await ReplyAsync(embed: await AudioService.LeaveAsync(Context.Guild));

            await Task.Delay(3000);

        }


        [Command("幻境播", RunMode = RunMode.Async)]
        public async Task playVoiceChannel([Remainder]string search)
        {
            Log_text("叫幻境離撥指定的歌", Context.User);

            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (check_Role(Context.User, SCLD.SCLD_OPid)) is_permission += 500;
            if (is_permission == 0) return;
            #endregion

            await ReplyAsync(embed: await AudioService.PlayAsync(Context.User as SocketGuildUser, Context.Guild, search));

            await Task.Delay(3000);

        }

            */
        #endregion

        #region 指令發話系列(GTAOP只能叫用不能修改)

        #region 訊息1
        //更新GTA接待
        [Command("修改GTA接待", RunMode = RunMode.Async)]
        public async Task update_GTAwelcome1_4([Remainder]String the_text) { await update_GTAwelcome1(the_text); }
        [Command("修改GTA接待1", RunMode = RunMode.Async)]
        public async Task update_GTAwelcome1_3([Remainder]String the_text) { await update_GTAwelcome1(the_text); }
        [Command("更新GTA接待", RunMode = RunMode.Async)]
        public async Task update_GTAwelcome1_2([Remainder]String the_text) { await update_GTAwelcome1(the_text); }
        [Command("更新GTA接待1", RunMode = RunMode.Async)]
        public async Task update_GTAwelcome1([Remainder]String the_text)
        {
            Log_text("更新GTA接待1", Context.User);

            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (check_Role(Context.User, SCLD.SCLD_OPid)) is_permission += 500;
            if (is_permission == 0) return;
            #endregion

            System.IO.File.WriteAllText("GTAwelcome1.txt", the_text);

            IUserMessage text_complete = await Context.Channel.SendMessageAsync("（已更新GTA接待1的清單）");
            await Context.Message.DeleteAsync();
            await Task.Delay(3000);
            await text_complete.DeleteAsync();

            //存好以後顯示

            await show_update_GTAwelcome1();

        }

        //顯示GTA接待1
        [Command("顯示GTA接待1", RunMode = RunMode.Async)]
        public async Task show_update_GTAwelcome1()
        {
            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (check_Role(Context.User, SCLD.SCLD_OPid)) is_permission += 500;
            if (is_permission == 0) return;
            #endregion

            String textfileall = "以下是您更新後的預覽 (GTA接待1)\n------------------------\n";
            String[] textfile = System.IO.File.ReadAllLines("GTAwelcome1.txt");

            for (int i = 0; i < textfile.Length; i++)
                textfileall += textfile[i] + "\n";

            await Context.Channel.SendMessageAsync(textfileall);
        }

        //直接使用GTA接待
        [Command("GTA接待", RunMode = RunMode.Async)]
        public async Task show_GTAwelcome1_2() { await show_GTAwelcome1(); }
        [Command("GTA接待1", RunMode = RunMode.Async)]
        public async Task show_GTAwelcome1()
        {
            // if (!(check_Super_OP(Context.User) || check_GTAOP(Context.User))) return;
            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (check_Role(Context.User, SCLD.SCLD_OPid)) is_permission += 500;
            if (check_Role(Context.User, SCLD.GTA_OPid)) is_permission += 100;
            if (is_permission == 0) return;
            #endregion


            String textfileall = "";
            String[] textfile = System.IO.File.ReadAllLines("GTAwelcome1.txt");

            for (int i = 0; i < textfile.Length; i++)
                textfileall += textfile[i] + "\n";

            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync(textfileall);
        }
        #endregion

        #region 訊息2
        //更新GTA接待2
        [Command("修改GTA接待2", RunMode = RunMode.Async)]
        public async Task update_GTAwelcome2_2([Remainder]String the_text) { await update_GTAwelcome2(the_text); }
        [Command("更新GTA接待2", RunMode = RunMode.Async)]
        public async Task update_GTAwelcome2([Remainder]String the_text)
        {
            Log_text("更新GTA接待2", Context.User);

            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (check_Role(Context.User, SCLD.SCLD_OPid)) is_permission += 500;
            if (is_permission == 0) return;
            #endregion

            System.IO.File.WriteAllText("GTAwelcome2.txt", the_text);

            IUserMessage text_complete = await Context.Channel.SendMessageAsync("（已更新GTA接待2的清單）");
            await Context.Message.DeleteAsync();
            await Task.Delay(3000);
            await text_complete.DeleteAsync();

            //存好以後顯示

            await show_update_GTAwelcome2();

        }

        //顯示GTA接待2
        [Command("顯示GTA接待2", RunMode = RunMode.Async)]
        public async Task show_update_GTAwelcome2()
        {
            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (check_Role(Context.User, SCLD.SCLD_OPid)) is_permission += 500;
            if (is_permission == 0) return;
            #endregion

            String textfileall = "以下是您更新後的預覽 (GTA接待2)\n------------------------\n";
            String[] textfile = System.IO.File.ReadAllLines("GTAwelcome2.txt");

            for (int i = 0; i < textfile.Length; i++)
                textfileall += textfile[i] + "\n";

            await Context.Channel.SendMessageAsync(textfileall);
        }

        //直接使用GTA接待2
        [Command("GTA接待2", RunMode = RunMode.Async)]
        public async Task show_GTAwelcome2()
        {
            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (check_Role(Context.User, SCLD.SCLD_OPid)) is_permission += 500;
            if (check_Role(Context.User, SCLD.GTA_OPid)) is_permission += 100;
            if (is_permission == 0) return;
            #endregion


            String textfileall = "";
            String[] textfile = System.IO.File.ReadAllLines("GTAwelcome2.txt");

            for (int i = 0; i < textfile.Length; i++)
                textfileall += textfile[i] + "\n";

            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync(textfileall);
        }
        #endregion

        #region 訊息3
        //更新GTA接待3
        [Command("修改GTA接待3", RunMode = RunMode.Async)]
        public async Task update_GTAwelcome3_2([Remainder]String the_text) { await update_GTAwelcome3(the_text); }
        [Command("更新GTA接待3", RunMode = RunMode.Async)]
        public async Task update_GTAwelcome3([Remainder]String the_text)
        {
            Log_text("更新GTA接待3", Context.User);

            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (check_Role(Context.User, SCLD.SCLD_OPid)) is_permission += 500;
            if (is_permission == 0) return;
            #endregion

            System.IO.File.WriteAllText("GTAwelcome3.txt", the_text);

            IUserMessage text_complete = await Context.Channel.SendMessageAsync("（已更新GTA接待3的清單）");
            await Context.Message.DeleteAsync();
            await Task.Delay(3000);
            await text_complete.DeleteAsync();

            //存好以後顯示

            await show_update_GTAwelcome3();

        }

        //顯示GTA接待3
        [Command("顯示GTA接待3", RunMode = RunMode.Async)]
        public async Task show_update_GTAwelcome3()
        {
            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (check_Role(Context.User, SCLD.SCLD_OPid)) is_permission += 500;
            if (is_permission == 0) return;
            #endregion

            String textfileall = "以下是您更新後的預覽 (GTA接待3)\n------------------------\n";
            String[] textfile = System.IO.File.ReadAllLines("GTAwelcome3.txt");

            for (int i = 0; i < textfile.Length; i++)
                textfileall += textfile[i] + "\n";

            await Context.Channel.SendMessageAsync(textfileall);
        }

        //直接使用GTA接待3
        [Command("GTA接待3", RunMode = RunMode.Async)]
        public async Task show_GTAwelcome3()
        {
            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (check_Role(Context.User, SCLD.SCLD_OPid)) is_permission += 500;
            if (check_Role(Context.User, SCLD.GTA_OPid)) is_permission += 100;
            if (is_permission == 0) return;
            #endregion

            String textfileall = "";
            String[] textfile = System.IO.File.ReadAllLines("GTAwelcome3.txt");

            for (int i = 0; i < textfile.Length; i++)
                textfileall += textfile[i] + "\n";

            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync(textfileall);
        }
        #endregion

        #region 提醒玩家上 Discord
        //更新沒來
        [Command("更新沒來", RunMode = RunMode.Async)]
        public async Task update_GTA_no_come([Remainder]String the_text)
        {
            Log_text("更新沒來", Context.User);

            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (check_Role(Context.User, SCLD.SCLD_OPid)) is_permission += 500;
            if (is_permission == 0) return;
            #endregion

            System.IO.File.WriteAllText("GTAnoDC.txt", the_text);

            IUserMessage text_complete = await Context.Channel.SendMessageAsync("（已更新GTA沒來的內容）");
            await Context.Message.DeleteAsync();
            await Task.Delay(3000);
            await text_complete.DeleteAsync();

            //存好以後顯示
            await show_update_GTA_no_come();

        }

        //顯示沒來
        [Command("顯示沒來", RunMode = RunMode.Async)]
        public async Task show_update_GTA_no_come()
        {
            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (check_Role(Context.User, SCLD.SCLD_OPid)) is_permission += 500;
            if (is_permission == 0) return;
            #endregion

            String textfileall = "以下是您更新後的預覽 (GTA沒來提醒)\n------------------------\n" + Context.User.Mention + " ";
            String[] textfile = System.IO.File.ReadAllLines("GTAnoDC.txt");

            for (int i = 0; i < textfile.Length; i++)
                textfileall += textfile[i] + "\n";

            EmbedBuilder eb1 = new EmbedBuilder();
            eb1.WithDescription(textfileall);

            await Context.Channel.SendMessageAsync("", false, eb1.Build());

        }

        #region 沒來
        //使用沒來  (!沒來 @sunskyey#5875)
        [Command("沒來", RunMode = RunMode.Async)]
        public async Task show_GTA_no_come(SocketGuildUser userName)
        {

            Log_text("輸入沒來指令", Context.User);

            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (check_Role(Context.User, SCLD.SCLD_OPid)) is_permission += 500;
            if (check_Role(Context.User, SCLD.GTA_OPid)) is_permission += 100;
            if ((check_Channel(Context.Message, SCLD.團隊中心頻道id) || check_Channel(Context.Message, SCLD.指令執行頻道id)) == false) is_permission = 0;
            if (is_permission == 0) return;
            #endregion

            String textfileall = "" + userName.Mention + "\n";
            String[] textfile = System.IO.File.ReadAllLines("GTAnoDC.txt");

            for (int i = 0; i < textfile.Length; i++)
                textfileall += textfile[i] + "\n"; 

            //崁入
            //EmbedBuilder eb1 = new EmbedBuilder();
            //eb1.WithDescription(textfileall);

            SocketTextChannel channel_001 = Context.Guild.GetChannel(SCLD.幫會大廳頻道id) as SocketTextChannel;
            //崁入
            //await channel_001.SendMessageAsync("", false, eb1.Build());
            await channel_001.SendMessageAsync(textfileall);

            await Task.Delay(3000);
            await Context.Message.DeleteAsync();
            //await Context.Channel.SendMessageAsync(textfileall);
        }
        #endregion

        #endregion

        #endregion

        #region 更新歡迎系列

        //更新歡迎
        [Command("修改歡迎", RunMode = RunMode.Async)]
        public async Task update_welcome2([Remainder]String the_text) { await update_welcome(the_text); }
        [Command("更新歡迎", RunMode = RunMode.Async)]
        public async Task update_welcome([Remainder]String the_text)
        {
            Log_text("更新歡迎", Context.User);

            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (is_permission == 0) return;
            #endregion

            System.IO.File.WriteAllText("wwww.txt", the_text);

            IUserMessage text_complete = await Context.Channel.SendMessageAsync("（已更新歡迎訊息）");
            await Context.Message.DeleteAsync();
            await Task.Delay(3000);
            await text_complete.DeleteAsync();

            //存好以後顯示

            String textfileall = "以下是您更新後的預覽 (頻道訊息)\n------------------------\n";
            String[] textfile = System.IO.File.ReadAllLines("wwww.txt");

            for (int i = 0; i < textfile.Length; i++)
            {
                textfileall += textfile[i].Replace("{User}", Context.Message.Author.Mention).Replace("{Guild}", (Context.Message.Author as SocketGuildUser).Guild.Name) + "\n";
            }

            await Context.Channel.SendMessageAsync(textfileall);
        }

        #endregion

        #region 歡迎系列

        [Command("歡迎")]
        public async Task get_Welcome() { await get_Welcome(Context.User as SocketGuildUser); }
        [Command("歡迎")]
        public async Task get_Welcome(SocketGuildUser userName)
        {
            Log_text("顯示歡迎", Context.User);

            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (is_permission == 0) return;
            #endregion

            String textfileall = "";
            String[] textfile = System.IO.File.ReadAllLines("wwww.txt");

            for (int i = 0; i < textfile.Length; i++)
            {
                textfileall += textfile[i].Replace("{User}", userName.Mention).Replace("{Guild}", (userName as SocketGuildUser).Guild.Name) + "\n";
            }

            IUserMessage text_complete = await ReplyAsync(textfileall);

            await Context.Message.DeleteAsync();

        }

        #endregion

        #region 強調系列

        //強調
        [Command("強調")]
        public async Task Reference_text(ulong text_id)
        {
            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (is_permission == 0) return;
            #endregion

            await Reference_text(Context.Channel as SocketTextChannel, text_id);
        }

        //修改 頻道
        [Command("強調")]
        public async Task Reference_text(SocketTextChannel textChannel, ulong text_id)
        {
            Log_text("強調", Context.User);

            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (is_permission == 0) return;
            #endregion

            IUserMessage text_message = await textChannel.GetMessageAsync(text_id) as IUserMessage;

            await textChannel.SendMessageAsync(text_message.Author.Mention + " 曾經說過了一句話：\n" + text_message.Content);

            //await ReplyAsync("（已處裡訊息修改）");

        }

        #endregion

        #region 修改訊息系列

        //修改
        [Command("修改")]
        public async Task edit_text(ulong text_id, [Remainder]String the_text) { await edit_text(Context.Channel as SocketTextChannel, text_id, the_text); }

        //修改 頻道
        [Command("修改")]
        public async Task edit_text(SocketTextChannel textChannel, ulong text_id, [Remainder]String the_text)
        {
            Log_text("修改訊息", Context.User);

            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (is_permission == 0) return;
            #endregion

            IUserMessage text_message = await textChannel.GetMessageAsync(text_id) as IUserMessage;

            EmbedBuilder eb = new EmbedBuilder();
            eb.WithDescription("凱凱好帥");

            await text_message.ModifyAsync(m => { m.Content = the_text; });   //m.Embed = eb.Build();

            await ReplyAsync("（已處裡訊息修改）");

        }

        #endregion

        #region 清除系列

        //清除
        [Command("刪除", RunMode = RunMode.Async)]
        public async Task get_delmessage1() { await get_delmessage(); }
        [Command("清除", RunMode = RunMode.Async)]
        public async Task get_delmessage()
        {
            Log_text("清除", Context.User);

            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (is_permission == 0) return;
            #endregion

            IUserMessage text_complete = await ReplyAsync("!清除 <幾行>");
            await Context.Message.DeleteAsync();
            await Task.Delay(5000);
            await text_complete.DeleteAsync();

        }

        //清除 幾行
        [Command("刪除", RunMode = RunMode.Async)]
        public async Task get_delmessage1(int line_count) { await get_delmessage(line_count); }
        [Command("清除", RunMode = RunMode.Async)]
        public async Task get_delmessage(int line_count)
        {
            Log_text("清除", Context.User);

            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (Context.User.Id == SCLD.vicid) is_permission += 999;
            if (is_permission == 0) return;
            #endregion

            if (line_count > 1000 && line_count > 0)
            {
                IUserMessage text_temp = await ReplyAsync("（訊息清除失敗！！！！！）\n（最多刪除1000則訊息）");
                await Context.Message.DeleteAsync();
                await Task.Delay(5000);
                await text_temp.DeleteAsync();
                return;
            }

            await Context.Message.DeleteAsync();

            IEnumerable<IMessage> messages = await Context.Channel.GetMessagesAsync(line_count).FlattenAsync();
            await ((ITextChannel)Context.Channel).DeleteMessagesAsync(messages);

            IUserMessage text_complete = await ReplyAsync("（清除了 " + (messages.Count()).ToString() + " 個訊息）");

            Log_text_nouser("刪除了 " + (messages.Count()).ToString() + "個訊息");

            await Task.Delay(5000);
            await text_complete.DeleteAsync();
        }



        //清除 頻道 幾行
        [Command("刪除", RunMode = RunMode.Async)]
        public async Task get_delmessage1(SocketTextChannel textChannel, int line_count) { await get_delmessage(textChannel, line_count); }
        [Command("清除", RunMode = RunMode.Async)]
        public async Task get_delmessage(SocketTextChannel textChannel, int line_count)
        {
            Log_text("清除 帶頻道", Context.User);

            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (is_permission == 0) return;
            #endregion

            if (line_count > 1000 && line_count > 0)
            {
                IUserMessage text_temp = await ReplyAsync("（訊息清除失敗！！！！！）\n（最多刪除1000則訊息）");
                await Context.Message.DeleteAsync();
                await Task.Delay(5000);
                await text_temp.DeleteAsync();
                return;
            }


            await Context.Message.DeleteAsync();

            IEnumerable<IMessage> messages = await textChannel.GetMessagesAsync(line_count).FlattenAsync(); //選定的頻道
            await ((ITextChannel)textChannel).DeleteMessagesAsync(messages);

            //IUserMessage text_complete = await ReplyAsync("（清除了 " + textChannel.Name + " 頻道的 " + (messages.Count()).ToString() + " 個訊息）");
            IUserMessage text_complete = await ReplyAsync("（" + textChannel.Mention + " 刪除 " + (messages.Count()).ToString() + " 個訊息）");

            Log_text_nouser("刪除了 " + textChannel.Name + " 頻道的" + (messages.Count()).ToString() + "個訊息");

            await Task.Delay(5000);
            await text_complete.DeleteAsync();
        }


        //清除 使用者
        [Command("刪除", RunMode = RunMode.Async)]
        public async Task get_delmessage1(SocketGuildUser userName) { await get_delmessage(); }
        [Command("清除", RunMode = RunMode.Async)]
        public async Task get_delmessage(SocketGuildUser userName) { await get_delmessage(); }

        //清除 使用者 幾行
        [Command("刪除", RunMode = RunMode.Async)]
        public async Task get_delmessage1(SocketGuildUser userName, int line_count) { await get_delmessage(userName, line_count); }
        [Command("清除", RunMode = RunMode.Async)]
        public async Task get_delmessage(SocketGuildUser userName, int line_count)
        {
            Log_text("清除", Context.User);

            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (is_permission == 0) return;
            #endregion

            if (line_count > 1000 && line_count > 0)
            {
                IUserMessage text_temp = await ReplyAsync("（訊息清除失敗！！！！！）\n（最多刪除1000則訊息）");
                await Context.Message.DeleteAsync();
                await Task.Delay(5000);
                await text_temp.DeleteAsync();
                return;
            }

            await Context.Message.DeleteAsync();  //因為是刪除別人的 所以自己的先刪掉

            //讀取訊息，然後篩選
            IEnumerable<IMessage> messages = await Context.Channel.GetMessagesAsync().FlattenAsync();
            IEnumerable<IMessage> messages2 = messages.Where(m => m.Author == userName).Take(line_count);

            await ((ITextChannel)Context.Channel).DeleteMessagesAsync(messages2);

            IUserMessage text_complete = await ReplyAsync("（清除了 " + userName.Nickname + " 的 **" + messages2.Count().ToString() + "** 則訊息：）");

            Log_text_nouser("刪除了 " + userName.Nickname + " 的 " + (messages2.Count()).ToString() + " 個訊息");

            await Task.Delay(5000);
            await text_complete.DeleteAsync();
        }

        //清除 頻道 使用者 幾行
        [Command("刪除", RunMode = RunMode.Async)]
        public async Task get_delmessage1(SocketTextChannel textChannel, SocketGuildUser userName, int line_count) { await get_delmessage(textChannel, userName, line_count); }
        [Command("清除", RunMode = RunMode.Async)]
        public async Task get_delmessage(SocketTextChannel textChannel, SocketGuildUser userName, int line_count)
        {
            Log_text("清除 帶頻道", Context.User);

            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (is_permission == 0) return;
            #endregion

            if (line_count > 1000 && line_count > 0)
            {
                IUserMessage text_temp = await ReplyAsync("（訊息清除失敗！！！！！）\n（最多刪除1000則訊息）");
                await Context.Message.DeleteAsync();
                await Task.Delay(5000);
                await text_temp.DeleteAsync();
                return;
            }

            await Context.Message.DeleteAsync();

            //讀取訊息，然後篩選
            IEnumerable<IMessage> messages = await textChannel.GetMessagesAsync().FlattenAsync();
            IEnumerable<IMessage> messages2 = messages.Where(m => m.Author == userName).Take(line_count);
            await ((ITextChannel)textChannel).DeleteMessagesAsync(messages2);


            //IUserMessage text_complete = await ReplyAsync("（清除了 " + textChannel.Name + " 頻道的 " + (messages.Count()).ToString() + " 個訊息）");
            IUserMessage text_complete = await ReplyAsync("（" + textChannel.Mention + " 刪除 " + userName.Mention + " 的 " + (messages2.Count()).ToString() + " 個訊息）");

            Log_text_nouser("刪除了 " + textChannel.Name + " 頻道的 " + userName.Nickname + " 的 " + (messages.Count()).ToString() + "個訊息");

            await Task.Delay(5000);
            await text_complete.DeleteAsync();
        }


        //避開清除某人專用 頻道 使用者 幾行
        [Command("刪除跳過", RunMode = RunMode.Async)]
        public async Task get_delmessage_exemption2(SocketTextChannel textChannel, SocketGuildUser userName, int line_count) { await get_delmessage_exemption(textChannel, userName, line_count); }
        [Command("清除跳過", RunMode = RunMode.Async)]
        public async Task get_delmessage_exemption(SocketTextChannel textChannel, SocketGuildUser userName, int line_count)
        {
            Log_text("跳過清除 帶頻道", Context.User);

            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (is_permission == 0) return;
            #endregion

            if (line_count > 1000 && line_count > 0)
            {
                IUserMessage text_temp = await ReplyAsync("（訊息清除失敗！！！！！）\n（最多刪除1000則訊息）");
                await Context.Message.DeleteAsync();
                await Task.Delay(5000);
                await text_temp.DeleteAsync();
                return;
            }

            await Context.Message.DeleteAsync();

            //讀取訊息，然後篩選
            IEnumerable<IMessage> messages = await textChannel.GetMessagesAsync().FlattenAsync();
            IEnumerable<IMessage> messages2 = messages.Where(m => m.Author != userName).Take(line_count);
            await ((ITextChannel)textChannel).DeleteMessagesAsync(messages2);


            //IUserMessage text_complete = await ReplyAsync("（清除了 " + textChannel.Name + " 頻道的 " + (messages.Count()).ToString() + " 個訊息）");
            IUserMessage text_complete = await ReplyAsync("（" + textChannel.Mention + " 刪除除了 " + userName.Mention + " 的 " + (messages2.Count()).ToString() + " 個訊息）");

            Log_text_nouser("刪除了 " + textChannel.Name + " 頻道除了 " + userName.Nickname + " 的 " + (messages.Count()).ToString() + "個訊息");

            await Task.Delay(5000);
            await text_complete.DeleteAsync();
        }

        #endregion

        #region 全清系列

        [Command("全清", RunMode = RunMode.Async)]
        public async Task alldelete()
        {
            Log_text("清除全部以指定的頻道", Context.User);

            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (is_permission == 0) return;
            #endregion

            String[] textfile = System.IO.File.ReadAllLines("AllDeleteChannel.txt");

            String replay_message = "";

            for (int i = 0; i < textfile.Length; i++)
            {
                SocketTextChannel the_channel = Context.Guild.GetChannel(ulong.Parse(textfile[i])) as SocketTextChannel;
                IEnumerable<IMessage> messages = await the_channel.GetMessagesAsync(999).FlattenAsync();
                await ((ITextChannel)the_channel).DeleteMessagesAsync(messages);

                Log_text_nouser("刪除了 " + the_channel.Name + " 頻道的" + (messages.Count()).ToString() + "個訊息");
                replay_message += "（" + the_channel.Mention + " 刪除 " + (messages.Count()).ToString() + " 個訊息）\n";
            }
            
            IUserMessage text_complete = await ReplyAsync(replay_message);
            await Task.Delay(60000);
            await text_complete.DeleteAsync();
        }

        #region 讀寫

        [Command("更新全清", RunMode = RunMode.Async)]
        public async Task update_alldelete([Remainder]String the_text)
        {
            Log_text("更新全清清單", Context.User);

            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (is_permission == 0) return;
            #endregion

            System.IO.File.WriteAllText("AllDeleteChannel.txt", the_text);

            IUserMessage text_complete = await Context.Channel.SendMessageAsync("（已更新全清的清單）");
            await Context.Message.DeleteAsync();
            await Task.Delay(3000);
            await text_complete.DeleteAsync();

            //存好以後顯示

            await show_alldelete();

        }

        [Command("顯示全清", RunMode = RunMode.Async)]
        public async Task show_alldelete()
        {
            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (is_permission == 0) return;
            #endregion

            String textfileall = "以下是您更新後的預覽 (全清清單)\n------------------------\n";
            String[] textfile = System.IO.File.ReadAllLines("AllDeleteChannel.txt");


            for (int i = 0; i < textfile.Length; i++)
                textfileall += textfile[i] + " - " + (Context.Guild.GetChannel(ulong.Parse(textfile[i])) as SocketTextChannel).Mention + "\n";

            await Context.Channel.SendMessageAsync(textfileall);
        }

        #endregion

        #endregion

        #region 查看系列

        //查看
        [Command("查看", RunMode = RunMode.Async)]
        public async Task get_hhhmessage1() { await get_hhhmessage(); }
        [Command("歷史", RunMode = RunMode.Async)]
        public async Task get_hhhmessage()
        {
            Log_text("查紀錄", Context.User);

            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (is_permission == 0) return;
            #endregion

            IUserMessage text_complete = await ReplyAsync("!紀錄 <誰> <幾行>");
            await Context.Message.DeleteAsync();
            await Task.Delay(3000);
            await text_complete.DeleteAsync();

        }

        //查看 使用者
        [Command("查看", RunMode = RunMode.Async)]
        public async Task get_hhhmessage1(SocketGuildUser userName) { await get_hhhmessage(); }
        [Command("歷史", RunMode = RunMode.Async)]
        public async Task get_hhhmessage(SocketGuildUser userName) { await get_hhhmessage(); }

        //查看 使用者 行數
        [Command("查看", RunMode = RunMode.Async)]
        public async Task get_hhhmessage1(SocketGuildUser userName, int line_count) { await get_hhhmessage(userName, line_count); }
        [Command("歷史", RunMode = RunMode.Async)]
        public async Task get_hhhmessage(SocketGuildUser userName, int line_count)
        {
            Log_text("查紀錄", Context.User);

            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (is_permission == 0) return;
            #endregion

            if (line_count > 1000 && line_count > 0)
            {
                IUserMessage text_temp = await ReplyAsync("（訊息搜尋失敗！！！！！）\n（最多搜尋1000則訊息）");
                await Context.Message.DeleteAsync();
                await Task.Delay(3000);
                await text_temp.DeleteAsync();
                return;
            }


            await Context.Message.DeleteAsync();  //收到指令後直接刪除

            //讀取訊息，然後篩選
            IEnumerable<IMessage> viewmessages = await Context.Channel.GetMessagesAsync().FlattenAsync();
            IEnumerable<IMessage> viewmessages2 = viewmessages.Where(m => m.Author == userName).Take(line_count);
            IMessage[] mmmviewarr = viewmessages2.ToArray();


            String asdsads = userName.Nickname + "的 **" + mmmviewarr.Length.ToString() + "** 則訊息：\n";
            for (int i = mmmviewarr.Length - 1; i >= 0; i--)
                asdsads += "【" + mmmviewarr[i].CreatedAt.AddHours(8).ToString("MM/dd tt hh:mm:ss") + "】" + (mmmviewarr[i].Author as SocketGuildUser).Nickname + " : " + mmmviewarr[i].Content + "\n";


            IUserMessage text_complete = await ReplyAsync(asdsads);

            Log_text_nouser("查看了 " + (viewmessages2.Count()).ToString() + " 個訊息");
        }

        #endregion

        #region 群組操作系列

        [Command("我的身分")]
        [Alias("身分", "群組", "我的群組", "誰")]
        public async Task get_roles()
        {
            Log_text("我的身分", Context.User);

            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (is_permission == 0) return;
            #endregion

            IRole[] role_arr = (Context.User as SocketGuildUser).Roles.ToArray();

            String roletext = Context.User.Mention + " 的身分為：\n";
            for (int i = 0; i < role_arr.Length; i++)
            {
                if (role_arr[i].Name.Equals("@everyone"))
                    continue;
                roletext += role_arr[i].Name + ", id: " + role_arr[i].Id + "\n";
            }

            await Context.Message.DeleteAsync();

            await Context.Channel.SendMessageAsync(roletext);

        }


        [Command("他的身分")]
        [Alias("身分", "群組", "他的群組", "誰")]
        public async Task get_roles(SocketGuildUser userName)
        {
            Log_text("查詢身分", Context.User);


            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (is_permission == 0) return;
            #endregion

            IRole[] role_arr = (userName as SocketGuildUser).Roles.ToArray();

            String roletext = "";
            roletext += "本名：" + userName.ToString() + "\n";
            roletext += "ID：" + userName.Id + "\n";
            roletext += userName.Mention + " 的身分為：\n";
            for (int i = 0; i < role_arr.Length; i++)
            {
                if (role_arr[i].Name.Equals("@everyone"))
                    continue;
                roletext += role_arr[i].Name + ", id: " + role_arr[i].Id + "\n";
            }

            await Context.Message.DeleteAsync();

            await Context.Channel.SendMessageAsync(roletext);

        }




        [Command("新增身分")]
        public async Task qwe_Setrolo(SocketGuildUser userName, SocketRole roleName)
        {
            Log_text("新增身分", Context.User);


            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (is_permission == 0) return;
            #endregion

            await Context.Message.DeleteAsync();

            SocketGuildUser user = Context.User as SocketGuildUser;

            Log_text_nouser("發指令者是 Super OP");
            if (user.GuildPermissions.ManageRoles)
            {
                Log_text_nouser("變更群組");
                await userName.AddRoleAsync(roleName);
            }

        }

        #endregion

        #region 剔除系列

        [Command("剔除")]
        public async Task KickUser(SocketGuildUser userName)
        {
            Log_text("剔除", Context.User);


            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (check_Role(Context.User, SCLD.SCLD_OPid)) is_permission += 500;
            if (is_permission == 0) return;
            #endregion

            Log_text_nouser("輸入者權限足夠，繼續送出剔除指令");

            //這樣也可以
            //is_permission = 0;

            EmbedBuilder ebKick = new EmbedBuilder();//
         

            //驗證要被剔除的身分
            int is_kick = 0;
            if (userName.Id == SCLD.Founderid) is_kick += 8888;

            if (is_kick == 8888)
            {
                Log_text_nouser("創群人無法被剔除");
                IUserMessage temp_mess01 = await Context.Channel.SendMessageAsync(Context.User.Mention + "\n" + "你他媽連創辦人都敢剔除，你翅膀很硬膩。");
                await Task.Delay(5000);
                await temp_mess01.DeleteAsync();
                await Context.Message.DeleteAsync();
                return;
            }
            if (check_Role(userName, SCLD.布里茨id)) is_kick += 4761;
            if (check_Role(userName, SCLD.Super_OPid)) is_kick += 1000;
            if (check_Role(userName, SCLD.SCLD_OPid)) is_kick += 500;
            if (check_Role(userName, SCLD.GTA_OPid)) is_kick += 100;

            if (is_kick > 0)
            {
                Log_text_nouser("該身分組無法剔除");
                IUserMessage temp_mess01 = await Context.Channel.SendMessageAsync("含有無法剔除的身分組，此王八蛋無法被剔除。");
                await Task.Delay(5000);
                await temp_mess01.DeleteAsync();
                await Context.Message.DeleteAsync();
                return;
            }


            
            // Do Stuff
            if ((Context.User as SocketGuildUser).GuildPermissions.KickMembers)
            {
                Log_text_nouser("該身份組可以剔除");
                Log_text_nouser((Context.User as SocketGuildUser).Nickname + " 成功剔除 " + userName.Nickname + " (" + userName.Username + ") (" + userName.ToString() + ")" + "。");
                await Task.Delay(1000);
                await userName.KickAsync();


                //ebKick.WithDescription(re_message); //誰打指令加了誰 的訊息
                

                String temp_text = Context.User.Mention + "成功剔除 " + userName.Mention + "。";
                ebKick.WithDescription(temp_text);
                //IUserMessage temp_mess02 = await Context.Channel.SendMessageAsync(Context.User.Mention +"成功剔除 "  + userName.Mention + "。");
                IUserMessage temp_mess02 = await Context.Channel.SendMessageAsync("",false,ebKick.Build());
                await Context.Message.DeleteAsync();
                await Task.Delay(5000);
                await temp_mess02.DeleteAsync();
            }

        }

        #endregion

        #region 更新接待設定系列

        [Command("更新接待設定", RunMode = RunMode.Async)]
        public async Task set_auto_role([Remainder]String the_text)
        {
            Log_text("更新接待設定", Context.User);

            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (is_permission == 0) return;
            #endregion

            System.IO.File.WriteAllText("auto_role.txt", the_text);

            IUserMessage text_complete = await Context.Channel.SendMessageAsync("（已更新接待設定）");
            await Context.Message.DeleteAsync();
            await Task.Delay(3000);
            await text_complete.DeleteAsync();

            //存好以後顯示

            String textfileall = "以下是您更新後的預覽 (接待設定)\n------------------------\n";
            String[] textfile = System.IO.File.ReadAllLines("auto_role.txt");

            for (int i = 0; i < textfile.Length; i++)
            {
                textfileall += textfile[i] + "\n";
            }

            await Context.Channel.SendMessageAsync(textfileall);
        }

        #endregion

        #region 重新指向系列
        //這裡面都重新導向 所以不需要做驗證，等到方法後才去進行身分驗證


        [Command("加入", RunMode = RunMode.Async)]
        public async Task enter_what(String the_type, SocketGuildUser userName)
        {
            if (the_type.Equals("GTA"))
                await OKGTA(userName);
        }

        [Command("GTA", RunMode = RunMode.Async)]
        public async Task enter_GTA(String the_type)
        {
            if (the_type.Equals("接待"))
                await show_GTAwelcome1();
            if (the_type.Equals("接待1"))
                await show_GTAwelcome1();
            if (the_type.Equals("接待2"))
                await show_GTAwelcome2();
            if (the_type.Equals("接待3"))
                await show_GTAwelcome3();
        }

        [Command("修改", RunMode = RunMode.Async)]
        public async Task update_Onmyoji2(String the_type, [Remainder]String the_text) { await update_Onmyoji(the_type, the_text); }
        [Command("更新", RunMode = RunMode.Async)]
        public async Task update_Onmyoji(String the_type, [Remainder]String the_text)
        {
            if (the_type.Equals("SP"))
                await update_Onmyoji_SP(the_text);
            if (the_type.Equals("SSR"))
                await update_Onmyoji_SSR(the_text);
            if (the_type.Equals("SR"))
                await update_Onmyoji_SR(the_text);
            if (the_type.Equals("R"))
                await update_Onmyoji_R(the_text);
            if (the_type.Equals("N"))
                await update_Onmyoji_N(the_text);
        }

        [Command("卡池", RunMode = RunMode.Async)]
        public async Task show_Onmyoji2(String the_type) { await show_Onmyoji(the_type); }
        [Command("顯示", RunMode = RunMode.Async)]
        public async Task show_Onmyoji(String the_type)
        {
            if (the_type.Equals("SP"))
                await show_Onmyoji_SP();
            if (the_type.Equals("SSR"))
                await show_Onmyoji_SSR();
            if (the_type.Equals("SR"))
                await show_Onmyoji_SR();
            if (the_type.Equals("R"))
                await show_Onmyoji_R();
            if (the_type.Equals("N"))
                await show_Onmyoji_N();
        }


        #endregion

        #region GTA的OP專用

        [Command("加入GTA", RunMode = RunMode.Async)]
        public async Task OKGTA(SocketGuildUser userName)
        {
            Log_text("加人進GTA", Context.User);


            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (check_Role(Context.User, SCLD.SCLD_OPid)) is_permission += 500;
            if (check_Role(Context.User, SCLD.GTA_OPid)) is_permission += 100;
            if (is_permission == 0) return;
            #endregion

            //驗證 Discord 群暱稱有無正確修改
            if (userName.Nickname == null || userName.Nickname == "")
            {
                //此處為星空專案，在那邊該該叫所以多改這段出來
                //if (userName.ToString().IndexOf("GTA◎") < 0 || userName.ToString().IndexOf("【") < 0 || userName.ToString().IndexOf("】") < 0)
                {
                    IUserMessage temp_mess = await Context.Channel.SendMessageAsync(Context.User.Mention + "\n" + "對方群暱稱為空白，請對方本名不要根本群要求暱稱相同。");
                    await Context.Message.DeleteAsync();
                    await Task.Delay(15000);
                    await temp_mess.DeleteAsync();
                    return;
                }
            }
            else
            {
                if (userName.Nickname.IndexOf("GTA◎") < 0 || userName.Nickname.IndexOf("【") < 0 || userName.Nickname.IndexOf("】") < 0)
                {
                    IUserMessage temp_mess = await Context.Channel.SendMessageAsync(Context.User.Mention + "\n" + "你他媽人家還沒改暱稱你就加人家");
                    await Context.Message.DeleteAsync();
                    await Task.Delay(5000);
                    await temp_mess.DeleteAsync();
                    return;
                }
            }



            //驗證被加入成員是否已經加入「GTA」幫會
            if (check_Role(userName, SCLD.俠盜獵車手id))
            {
                IUserMessage temp_mess = await Context.Channel.SendMessageAsync("此成員已加入「GTA」幫會。");
                await Context.Message.DeleteAsync();
                await Task.Delay(5000);
                await temp_mess.DeleteAsync();
                return;
            }


            /*
           //驗證這個人的群組可不可以變更他人權限
            if (user.GuildPermissions.ManageRoles)
            {
                Log_text_nouser("變更群組");
                //await userName.AddRoleAsync(roleName);
            }
            */
            Log_text_nouser("權限已確認");

            SocketGuildUser user = Context.User as SocketGuildUser;

            IRole role = user.Guild.Roles.FirstOrDefault(x => x.Id == SCLD.俠盜獵車手id);  //俠盜獵車手的身分ID
            IRole role2 = user.Guild.Roles.FirstOrDefault(x => x.Id == SCLD.俠盜獵車手待審核id);  //俠盜獵車手(待審核)的身分ID
            await userName.AddRoleAsync(role);
            await userName.RemoveRoleAsync(role2);
            Log_text_nouser("身分已修改");

            String re_message = "「" + Context.User.Mention + "」" + "將" + "「" + userName.Mention+"」" + "加入「GTA」幫會。";
            String re_message_lobby = userName.Mention + "\n歡迎加入幫會，遊玩時請前往 Discord 與大家一起同樂，如有任何遊戲上的問題都可以提問唷。";


            //傳送line
            try
            {
                String sss = "";
                SocketGuildUser cxus = Context.Guild.Users.FirstOrDefault(x => x.Id == Context.User.Id);
                sss += "「" + cxus.Nickname+ "」" + "將" + "「"+userName.Nickname+ "」" + " 加入「GTA」幫會。";
                //sss += "Discord 名稱為：" + reaction.User.Value;

                //isRock.LineBot.Utility.PushMessage("C7b17951555f37fa81ccc497796b39db4", sss, "7sh6efcRpyWNHThgbpAH9F28vhjys9yLwBPcy5DouzKiREg0vjzrU6ojViIxU6FoloqCa2QRbMvzQ2zzvnIy4NUubxBdEvbNS61yxCVzi5EPK5tSEzbmHUjJRCgDqhF0jkOxDhR63OZ5amB4SuDC4QdB04t89/1O/w1cDnyilFU=");
                String[] AccessToken = System.IO.File.ReadAllLines("SCLD_Token.txt");
                new SCLD_tools().post_line_notify(AccessToken[0], sss);
            }
            catch { }

            EmbedBuilder eb1 = new EmbedBuilder();
            EmbedBuilder eb2 = new EmbedBuilder();
            eb1.WithDescription(re_message); //誰打指令加了誰 的訊息
            //eb2.WithDescription(re_message_lobby); //歡迎訊息

            //管理頻道ID
            //SocketTextChannel channel1 = user.Guild.GetChannel(515518372532846592) as SocketTextChannel;
            //await channel1.SendMessageAsync("", false, eb1.Build());
            //幫會大廳ID
            SocketTextChannel channel2 = user.Guild.GetChannel(SCLD.幫會大廳頻道id) as SocketTextChannel;
            await channel2.SendMessageAsync(re_message_lobby);
            //await channel2.SendMessageAsync("", false, eb2.Build());
            //紀錄頻道ID
            SocketTextChannel channel3 = user.Guild.GetChannel(SCLD.gta加入通知頻道id) as SocketTextChannel;
            await channel3.SendMessageAsync("", false, eb1.Build());



            //回應當前的頻道
            //await ReplyAsync("", false, eb1.Build());
            IUserMessage text_response = await Context.Channel.SendMessageAsync("（已成功將玩家加入「GTA」幫會）");
            await Context.Message.DeleteAsync();
            await Task.Delay(10000);
            await text_response.DeleteAsync();



        }




        #endregion

        #region 召喚系列

        #region 設定卡池


        [Command("更新SP", RunMode = RunMode.Async)]
        public async Task update_Onmyoji_SP([Remainder]String the_text)
        {
            Log_text("更新SP卡池", Context.User);

            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (is_permission == 0) return;
            #endregion

            System.IO.File.WriteAllText("Onmyoji_SP.txt", the_text);

            IUserMessage text_complete = await Context.Channel.SendMessageAsync("（已更新陰陽師卡池-SP）");
            await Context.Message.DeleteAsync();
            await Task.Delay(3000);
            await text_complete.DeleteAsync();

            //存好以後顯示

            String textfileall = "";
            String[] textfile = System.IO.File.ReadAllLines("Onmyoji_SP.txt");

            textfileall = "目前的 " + get_emote("SP") + " 卡池共 " + textfile.Length + " 隻，如下：\n";

            String em_text = "";
            for (int i = 0; i < textfile.Length; i++)
            {
                if (i != 0 & i % 3 == 0)
                    em_text += "\n";
                else if (i != 0)
                    em_text += "｜";
                em_text += c_space(textfile[i], 4);
            }

            EmbedBuilder eb = new EmbedBuilder();
            eb.WithDescription(em_text);

            await Context.Channel.SendMessageAsync(textfileall, false, eb.Build());
        }

        [Command("更新SSR", RunMode = RunMode.Async)]
        public async Task update_Onmyoji_SSR([Remainder]String the_text)
        {
            Log_text("更新SSR卡池", Context.User);

            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (is_permission == 0) return;
            #endregion

            System.IO.File.WriteAllText("Onmyoji_SSR.txt", the_text);

            IUserMessage text_complete = await Context.Channel.SendMessageAsync("（已更新陰陽師卡池-SSR）");
            await Context.Message.DeleteAsync();
            await Task.Delay(3000);
            await text_complete.DeleteAsync();

            //存好以後顯示

            String textfileall = "";
            String[] textfile = System.IO.File.ReadAllLines("Onmyoji_SSR.txt");

            textfileall = "目前的 " + get_emote("SSR") + " 卡池共 " + textfile.Length + " 隻，如下：\n";

            String em_text = "";
            for (int i = 0; i < textfile.Length; i++)
            {
                if (i != 0 & i % 3 == 0)
                    em_text += "\n";
                else if (i != 0)
                    em_text += "｜";
                em_text += c_space(textfile[i], 4);
            }

            EmbedBuilder eb = new EmbedBuilder();
            eb.WithDescription(em_text);

            await Context.Channel.SendMessageAsync(textfileall, false, eb.Build());
        }

        [Command("更新SR", RunMode = RunMode.Async)]
        public async Task update_Onmyoji_SR([Remainder]String the_text)
        {
            Log_text("更新SR卡池", Context.User);

            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (is_permission == 0) return;
            #endregion

            System.IO.File.WriteAllText("Onmyoji_SR.txt", the_text);

            IUserMessage text_complete = await Context.Channel.SendMessageAsync("（已更新陰陽師卡池-SR）");
            await Context.Message.DeleteAsync();
            await Task.Delay(3000);
            await text_complete.DeleteAsync();

            //存好以後顯示

            String textfileall = "";
            String[] textfile = System.IO.File.ReadAllLines("Onmyoji_SR.txt");

            textfileall = "目前的 " + get_emote("SR") + " 卡池共 " + textfile.Length + " 隻，如下：\n";

            String em_text = "";
            for (int i = 0; i < textfile.Length; i++)
            {
                if (i != 0 & i % 3 == 0)
                    em_text += "\n";
                else if (i != 0)
                    em_text += "｜";
                em_text += c_space(textfile[i], 4);
            }

            EmbedBuilder eb = new EmbedBuilder();
            eb.WithDescription(em_text);

            await Context.Channel.SendMessageAsync(textfileall, false, eb.Build());
        }

        [Command("更新R", RunMode = RunMode.Async)]
        public async Task update_Onmyoji_R([Remainder]String the_text)
        {
            Log_text("更新R卡池", Context.User);

            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (is_permission == 0) return;
            #endregion

            System.IO.File.WriteAllText("Onmyoji_R.txt", the_text);

            IUserMessage text_complete = await Context.Channel.SendMessageAsync("（已更新陰陽師卡池-R）");
            await Context.Message.DeleteAsync();
            await Task.Delay(3000);
            await text_complete.DeleteAsync();

            //存好以後顯示

            String textfileall = "";
            String[] textfile = System.IO.File.ReadAllLines("Onmyoji_R.txt");

            textfileall = "目前的 " + get_emote("R") + " 卡池共 " + textfile.Length + " 隻，如下：\n";

            String em_text = "";
            for (int i = 0; i < textfile.Length; i++)
            {
                if (i != 0 & i % 3 == 0)
                    em_text += "\n";
                else if (i != 0)
                    em_text += "｜";
                em_text += c_space(textfile[i], 4);
            }

            EmbedBuilder eb = new EmbedBuilder();
            eb.WithDescription(em_text);

            await Context.Channel.SendMessageAsync(textfileall, false, eb.Build());
        }

        [Command("更新N", RunMode = RunMode.Async)]
        public async Task update_Onmyoji_N([Remainder]String the_text)
        {
            Log_text("更新N卡池", Context.User);

            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (is_permission == 0) return;
            #endregion

            System.IO.File.WriteAllText("Onmyoji_N.txt", the_text);

            IUserMessage text_complete = await Context.Channel.SendMessageAsync("（已更新陰陽師卡池-N）");
            await Context.Message.DeleteAsync();
            await Task.Delay(3000);
            await text_complete.DeleteAsync();

            //存好以後顯示

            String textfileall = "";
            String[] textfile = System.IO.File.ReadAllLines("Onmyoji_N.txt");

            textfileall = "目前的 " + get_emote("N") + " 卡池共 " + textfile.Length + " 隻，如下：\n";

            String em_text = "";
            for (int i = 0; i < textfile.Length; i++)
            {
                if (i != 0 & i % 3 == 0)
                    em_text += "\n";
                else if (i != 0)
                    em_text += "｜";
                em_text += c_space(textfile[i], 4);
            }

            EmbedBuilder eb = new EmbedBuilder();
            eb.WithDescription(em_text);

            await Context.Channel.SendMessageAsync(textfileall, false, eb.Build());
        }

        #endregion

        #region 顯示卡池


        [Command("顯示SP", RunMode = RunMode.Async)]
        public async Task show_Onmyoji_SP1() { await show_Onmyoji_SP(); }
        [Command("卡池SP", RunMode = RunMode.Async)]
        public async Task show_Onmyoji_SP()
        {
            Log_text("顯示卡池SP", Context.User);

            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (check_Role(Context.User, SCLD.SCLD_OPid)) is_permission += 500;
            if (check_Role(Context.User, SCLD.陰陽師OPid)) is_permission += 50;
            if (is_permission == 0) return;
            #endregion

            String textfileall = "";
            String[] textfile = System.IO.File.ReadAllLines("Onmyoji_SP.txt");

            textfileall = "目前的 " + get_emote("SP") + " 卡池共 " + textfile.Length + " 隻，如下：\n";

            String em_text = "";
            for (int i = 0; i < textfile.Length; i++)
            {
                if (i != 0 & i % 3 == 0)
                    em_text += "\n";
                else if (i != 0)
                    em_text += "｜";
                em_text += c_space(textfile[i], 4);
            }

            EmbedBuilder eb = new EmbedBuilder();
            eb.WithDescription(em_text);

            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync(textfileall, false, eb.Build());
        }


        [Command("顯示SSR", RunMode = RunMode.Async)]
        public async Task show_Onmyoji_SSR1() { await show_Onmyoji_SSR(); }
        [Command("卡池SSR", RunMode = RunMode.Async)]
        public async Task show_Onmyoji_SSR()
        {
            Log_text("顯示卡池SSR", Context.User);

            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (check_Role(Context.User, SCLD.SCLD_OPid)) is_permission += 500;
            if (check_Role(Context.User, SCLD.陰陽師OPid)) is_permission += 50;
            if (is_permission == 0) return;
            #endregion


            String textfileall = "";
            String[] textfile = System.IO.File.ReadAllLines("Onmyoji_SSR.txt");

            textfileall = "目前的 " + get_emote("SSR") + " 卡池共 " + textfile.Length + " 隻，如下：\n";

            String em_text = "";
            for (int i = 0; i < textfile.Length; i++)
            {
                if (i != 0 & i % 3 == 0)
                    em_text += "\n";
                else if (i != 0)
                    em_text += "｜";
                em_text += c_space(textfile[i], 4);
            }

            EmbedBuilder eb = new EmbedBuilder();
            eb.WithDescription(em_text);

            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync(textfileall, false, eb.Build());


        }


        [Command("顯示SR", RunMode = RunMode.Async)]
        public async Task show_Onmyoji_SR1() { await show_Onmyoji_SR(); }
        [Command("卡池SR", RunMode = RunMode.Async)]
        public async Task show_Onmyoji_SR()
        {
            Log_text("顯示卡池SR", Context.User);

            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (check_Role(Context.User, SCLD.SCLD_OPid)) is_permission += 500;
            if (check_Role(Context.User, SCLD.陰陽師OPid)) is_permission += 50;
            if (is_permission == 0) return;
            #endregion

            String textfileall = "";
            String[] textfile = System.IO.File.ReadAllLines("Onmyoji_SR.txt");

            textfileall = "目前的 " + get_emote("SR") + " 卡池共 " + textfile.Length + " 隻，如下：\n";

            String em_text = "";
            for (int i = 0; i < textfile.Length; i++)
            {
                if (i != 0 & i % 3 == 0)
                    em_text += "\n";
                else if (i != 0)
                    em_text += "｜";
                em_text += c_space(textfile[i], 4);
            }

            EmbedBuilder eb = new EmbedBuilder();
            eb.WithDescription(em_text);

            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync(textfileall, false, eb.Build());
        }

        [Command("顯示R", RunMode = RunMode.Async)]
        public async Task show_Onmyoji_R1() { await show_Onmyoji_R(); }
        [Command("卡池R", RunMode = RunMode.Async)]
        public async Task show_Onmyoji_R()
        {
            Log_text("顯示卡池R", Context.User);

            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (check_Role(Context.User, SCLD.SCLD_OPid)) is_permission += 500;
            if (check_Role(Context.User, SCLD.陰陽師OPid)) is_permission += 50;
            if (is_permission == 0) return;
            #endregion

            String textfileall = "";
            String[] textfile = System.IO.File.ReadAllLines("Onmyoji_R.txt");

            textfileall = "目前的 " + get_emote("R") + " 卡池共 " + textfile.Length + " 隻，如下：\n";

            String em_text = "";
            for (int i = 0; i < textfile.Length; i++)
            {
                if (i != 0 & i % 3 == 0)
                    em_text += "\n";
                else if (i != 0)
                    em_text += "｜";
                em_text += c_space(textfile[i], 4);
            }

            EmbedBuilder eb = new EmbedBuilder();
            eb.WithDescription(em_text);

            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync(textfileall, false, eb.Build());
        }

        [Command("顯示N", RunMode = RunMode.Async)]
        public async Task show_Onmyoji_N1() { await show_Onmyoji_R(); }
        [Command("卡池N", RunMode = RunMode.Async)]
        public async Task show_Onmyoji_N()
        {
            Log_text("顯示卡池N", Context.User);

            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (check_Role(Context.User, SCLD.SCLD_OPid)) is_permission += 500;
            if (check_Role(Context.User, SCLD.陰陽師OPid)) is_permission += 50;
            if (is_permission == 0) return;
            #endregion

            String textfileall = "";
            String[] textfile = System.IO.File.ReadAllLines("Onmyoji_N.txt");

            textfileall = "目前的 " + get_emote("N") + " 卡池共 " + textfile.Length + " 隻，如下：\n";


            String em_text = "";
            for (int i = 0; i < textfile.Length; i++)
            {
                if (i != 0 & i % 3 == 0)
                    em_text += "\n";
                else if (i != 0)
                    em_text += "｜";
                em_text += c_space(textfile[i], 4);
            }

            EmbedBuilder eb = new EmbedBuilder();
            eb.WithDescription(em_text);

            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync(textfileall, false, eb.Build());
        }



        #endregion

        #region 召喚中

        [Command("召喚", RunMode = RunMode.Async)]
        public async Task get_SSR()
        {
            /*
            if (!(check_Super_OP(Context.User) || check_Onmyoji(Context.User)))  //驗證是不是 Super OP 跟 陰陽師
                return;
            //510499589527306250  陰陽師的頻道ID
            if (check_Onmyoji(Context.User) && !check_Super_OP(Context.User))  //只是陰陽師 但是沒有 Super OP
                if (Context.Message.Channel.Id != 510499589527306250) return; //限制頻道，不是下面這頻道就跳掉
            */

            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.陰陽師id)) is_permission += 1;
            if (check_Role(Context.User, SCLD.陰陽師OPid)) is_permission += 1;
            //if (check_Onmyoji(Context.User)) is_permission += 1;
            //if (Context.Message.Channel.Id != SCLD.陰陽師頻道id) is_permission = 0;  //頻道驗證，陰陽師頻道的ID
            if (check_Channel(Context.Message, SCLD.召喚頻道id) == false) is_permission = 0;   //頻道驗證，陰陽師頻道的ID
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (is_permission == 0) return;
            #endregion


            //await get_SSR(1);
            String the_message = "";

            the_message += "!召喚 <數量>\n";
            the_message += "    超過10抽只會顯示SP與SSR\n";
            the_message += "!召喚 <數量> 顯示\n";
            the_message += "    顯示包含SR與R卡的式神\n";

            the_message += "請參閱：http://go.skyey.tw/sscld";

            await Context.Message.DeleteAsync();
            await ReplyAsync(the_message);
        }


        //召喚 幾隻
        [Command("召喚", RunMode = RunMode.Async)]
        public async Task get_SSR(int replay)
        {
            await get_SSR(replay, 1, "");
        }

        //召喚 幾隻 幾倍
        [Command("召喚", RunMode = RunMode.Async)]
        public async Task get_SSR(int replay, double multiplier)
        {
            await get_SSR(replay, multiplier, "");
        }

        //召喚 幾隻 顯示
        [Command("召喚", RunMode = RunMode.Async)]
        public async Task get_SSR(int replay, string is_show)
        {
            await get_SSR(replay, 1, is_show);
        }

        //召喚-完整資訊
        [Command("召喚", RunMode = RunMode.Async)]
        public async Task get_SSR(int replay,double multiplier, string is_show)
        {
            Log_text("陰陽師 " + replay.ToString() + " 抽", Context.User);

            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.陰陽師id)) is_permission += 1;
            if (Context.Message.Channel.Id != SCLD.召喚頻道id) is_permission = 0;  //頻道驗證，陰陽師頻道的ID
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (is_permission == 0) return;
            #endregion


            #region 錯誤排除，範圍控制

            if (replay > 1000)
            {
                await ReplyAsync("請不要貪心，你知道1000抽課金差不多要6萬嗎?");
                return;
            }

            if (replay < 1)
            {
                return;
            }

            #endregion

            String the_message = Context.User.Mention + "\n召喚 " + replay + " 隻式神(" + (multiplier == 1 ? "原始倍率" : multiplier.ToString("#0.0") + " 倍") + ")：\n";



            #region 召喚與文字排版

            Onmyoji_card Onmyoji_pool = new Onmyoji_card();
            Onmyoji_pool.read_card_pool(); //載入卡池，避免抽卡狂讀檔案，所以寫成這樣
            
            if (replay <= 10)
            {
                //****************
                //10抽以下全部顯示
                //****************

                Onmyoji_card[] aaa = new Onmyoji_card[10];

                Log_text_nouser("準備開抽");

                try
                {
                    for (int i = 0; i < replay; i++)
                    {
                        aaa[i] = Onmyoji_pool.new_card(get_rate(Context.User.Id) * multiplier);
                    the_message += get_emote(aaa[i].rarity) + " " + aaa[i].name + "\n";
                    }
                }
                catch (Exception e)
                {
                    Log_text_nouser(e.ToString());
                }
                
            }
            else 
            {
                //*****************
                //超過10抽 全部整理
                //*****************

                List<Onmyoji_card> card_list = new List<Onmyoji_card>();
                for (int i = 0; i < replay; i++)
                {
                    card_list.Add(Onmyoji_pool.new_card(get_rate(Context.User.Id) * multiplier));
                }


                //幫不同稀有度的是神分類
                IEnumerable<Onmyoji_card> card_SP = card_list.Where(x => x.rarity == "SP");
                IEnumerable<Onmyoji_card> card_SSR = card_list.Where(x => x.rarity == "SSR");
                IEnumerable<Onmyoji_card> card_SR = card_list.Where(x => x.rarity == "SR");
                IEnumerable<Onmyoji_card> card_R = card_list.Where(x => x.rarity == "R");


                


                //顯示不同稀有度的式神有幾隻

                the_message += get_emote("SP") + " " + card_SP.Count().ToString() + " 隻\n";
                the_message += get_emote("SSR") + " " + card_SSR.Count().ToString() + " 隻\n";
                the_message += get_emote("SR") + " " + card_SR.Count().ToString() + " 隻\n";
                the_message += get_emote("R") + " " + card_R.Count().ToString() + " 隻\n";


                //顯示 SP 的數量
                if (card_SP.Count() > 0)
                {
                    IEnumerable<IGrouping<string, Onmyoji_card>> result = card_SP.GroupBy(x => x.name);
                    the_message += "\nSP 有 " + result.Count() + " 種：\n";
                    result = result.OrderByDescending(x => x.Count()); //排序
                    String temp_card_list = "";
                    foreach (IGrouping<string, Onmyoji_card> group in result)
                    {
                        int count = 0;
                        foreach (Onmyoji_card qwefqwefqwefsdfasdfefqewf in group)
                            count++;
                        temp_card_list += get_emote("SP") + " " + group.Key + " × " + count + "\n";
                    }
                    the_message += temp_card_list;

                    //result = null;
                    //card_SP = null;
                }

                //顯示 SSR 的數量
                if (card_SSR.Count() > 0)
                {
                    IEnumerable<IGrouping<string, Onmyoji_card>> result = card_SSR.GroupBy(x => x.name);
                    the_message += "\nSSR 有 " + result.Count() + " 種：\n";
                    result = result.OrderByDescending(x => x.Count()); //排序
                    String temp_card_list = "";
                    foreach (IGrouping<string, Onmyoji_card> group in result)
                    {
                        int count = 0;
                        foreach (Onmyoji_card qwefqwefqwefqwefsdfasdfasdf in group)
                            count++;
                        temp_card_list += get_emote("SSR") + " " + group.Key + " × " + count + "\n";
                    }
                    the_message += temp_card_list;

                    //result = null;
                    //card_SSR = null;
                }

                //有條件的顯示
                if (is_show.Equals("顯示"))
                {
                    //顯示 SR 的數量
                    if (card_SR.Count() > 0)
                    {
                        IEnumerable<IGrouping<string, Onmyoji_card>> result = card_SR.GroupBy(x => x.name);
                        the_message += "\n" + get_emote("SR") + " 有 " + result.Count() + " 種：\n";
                        result = result.OrderByDescending(x => x.Count()); //排序
                        String temp_card_list = "";
                        foreach (IGrouping<string, Onmyoji_card> group in result)
                        {
                            int count = 0;
                            foreach (Onmyoji_card ergrwergwergwergwergwregqergqewf in group)
                                count++;
                            temp_card_list += group.Key + " × " + count + "\n";
                        }
                        the_message += temp_card_list;

                        //result = null;
                        //card_SR = null;
                    }

                    //顯示 R 的數量
                    if (card_R.Count() > 0)
                    {
                        IEnumerable<IGrouping<string, Onmyoji_card>> result = card_R.GroupBy(x => x.name);
                        the_message += "\n" + get_emote("R") + " 有 " + result.Count() + " 種：\n";
                        result = result.OrderByDescending(x => x.Count()); //排序
                        String temp_card_list = "";
                        foreach (IGrouping<string, Onmyoji_card> group in result)
                        {
                            int count = 0;
                            foreach (Onmyoji_card wefewfegretherhewrtgwergeqr in group)
                                count++;
                            temp_card_list += group.Key + " × " + count + "\n";
                        }
                        the_message += temp_card_list;

                        //result = null;
                        //card_R = null;

                    }
                }
                //card_list = null;
                //card_list.Clear();
               
            }

            #endregion



            await Context.Message.DeleteAsync();


            EmbedBuilder eb = new EmbedBuilder();
            eb.WithDescription(the_message);

            await ReplyAsync("", false, eb.Build());


            //GC.Collect(); //手動清除記憶體的廢物

            //await ReplyAsync(the_message);
        }

        #endregion

        #region 找伺服器的 Emoji ， 機率偷改 ， 名稱補空白

        String c_space(String the_text,int len)
        {
            if (the_text.Length < len)
            {
                int lost_count = len - the_text.Length;
                for (int i = 0; i < lost_count; i++)
                {
                    the_text += "　";
                }
            }

            return the_text;
        }


        /// <summary>
        /// 找伺服器的 Emoji
        /// </summary>
        /// <param name="the_text">圖案名稱</param>
        /// <returns></returns>
        GuildEmote get_emote(String the_text)
        {
            if (the_text.Equals("R"))
                the_text = "Rc";
            if (the_text.Equals("N"))
                the_text = "Nc";
            return Context.Guild.Emotes.First(e => e.Name == the_text);
        }

        /// <summary>
        /// 機率偷改
        /// </summary>
        /// <param name="the_user">使用者ID</param>
        /// <returns></returns>
        double get_rate(ulong the_user)
        {
            //464807640346918942  電貓
            //338898570969219076  VIC
            //364978943620677635  凱開

            List<ulong> double_user = new List<ulong>();
            List<ulong> Reduce_user = new List<ulong>();

            double_user.Add(464807640346918942);  //電貓
            //double_user.Add(364978943620677635);  //凱開


            //Reduce_user.Add(338898570969219076);  //VIC

            if (double_user.Where(x => x == the_user).Count() > 0)
                return 2;

            if (Reduce_user.Where(x => x == the_user).Count() > 0)
                return 0.5;

            return 1;
        }

        #endregion

        #endregion

        #region 驗證身分 寫log 到小黑窗

        private Boolean check_Channel(SocketMessage now_chid, ulong the_chid)
        {
            return now_chid.Channel.Id == the_chid;
        }


        private Boolean check_Role(SocketUser the_user, ulong the_Role_id)
        {
            IRole role = ((the_user as SocketGuildUser) as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Id == the_Role_id);  //有身分進來就驗證
            //    腳色 = (把使用者轉成 SocketGuildUser 然後再轉成 IGuildUser)的 腳色 (搜尋 ID 當成預設值)  (理論上只會找到一個)   
            //Log_text_nouser(role.Name);
            return (the_user as SocketGuildUser).Roles.Contains(role);
        }



        private void Log_text(String com_name, SocketUser loguser)
        {
            //IRole role = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Super OP");  //抓到 Super OP 的身分
            string timestamp = "[" + DateTime.Now.ToString("HH:mm:ss") + "] ";
            string logtext = timestamp + "收到 " + com_name + " 來自 " + (loguser as SocketGuildUser).Nickname + " (" + loguser.Username + ") (" + loguser.ToString() + ")";
            new SCLD_tools().write_log_file(logtext);
            Console.WriteLine(logtext);
        }

        private void Log_text_nouser(String content)
        {
            //IRole role = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Super OP");  //抓到 Super OP 的身分
            string timestamp = "[" + DateTime.Now.ToString("HH:mm:ss") + "] ";
            string logtext = timestamp + content;
            new SCLD_tools().write_log_file(logtext);
            Console.WriteLine(logtext);
        }

        #endregion

    }
}
