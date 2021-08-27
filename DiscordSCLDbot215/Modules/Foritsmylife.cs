using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord;
using System.Linq;
using System.Diagnostics;

namespace DiscordSCLDbot215.Modules
{
    public class Foritsmylife : ModuleBase<SocketCommandContext>
    {
        [Command("我的薪水", RunMode = RunMode.Async)]
        [Alias("抽薪水")]
        public async Task get_lottery()
        {

            #region 權限驗證
            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (check_Role(Context.User, SCLD.SCLD_OPid)) is_permission += 500;
            if (Context.User.Id == SCLD.itsmylifeid) is_permission += 999;
            if (is_permission == 0) return;
            #endregion

            //await Context.Message.DeleteAsync();

            Random rand = new Random(Guid.NewGuid().GetHashCode());

            int the_outpute = rand.Next(1, 10);
            the_outpute *= int.Parse("1".PadRight(rand.Next(0, 10), '0'));


            IUserMessage text_complete = await Context.Channel.SendMessageAsync(the_outpute.ToString("###,###"),false,null, null, null, new MessageReference(Context.Message.Id));

            //await Task.Delay(10000);

            //await text_complete.DeleteAsync();
        }





        private Boolean check_Role(SocketUser the_user, ulong the_Role_id)
        {
            IRole role = ((the_user as SocketGuildUser) as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Id == the_Role_id);  //有身分進來就驗證
            //    腳色 = (把使用者轉成 SocketGuildUser 然後再轉成 IGuildUser)的 腳色 (搜尋 ID 當成預設值)  (理論上只會找到一個)   
            //Console.WriteLine(role.Name);
            return (the_user as SocketGuildUser).Roles.Contains(role);
        }
    }
}
