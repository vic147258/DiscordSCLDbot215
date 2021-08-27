using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordSCLDbot215.Handlers;
using DiscordSCLDbot215.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Victoria;
using Victoria.Enums;


namespace DiscordSCLDbot215.Modules
{
    public class MusicModule : ModuleBase<SocketCommandContext>
    {

        private LavaNode _lavaNode;
        public MusicModule(LavaNode lavaNode)
                => _lavaNode = lavaNode;

        public LavaLinkAudio AudioService { get; set; }


        [Command("Join")]
        [Alias("來", "加入", "不要走")]
        public async Task JoinAndPlay()
        {
            if (!check_permission()) return;
            await AudioService.JoinAsync(Context);
        }

        [Command("Leave")]
        [Alias("去", "滾", "離開", "去你媽的", "去妳媽的")]
        public async Task Leave()
        {
            if (!check_permission()) return;
            await AudioService.LeaveAsync(Context);
        }


        [Command("Play")]
        [Alias("P", "播", "播放", "播歌", "點歌", "點播", "請開始你的表演")]
        public async Task Play([Remainder]string search)
        {
            if (!check_permission()) return;
            await AudioService.PlayAsync(Context, search);
        }

        [Command("PlayTop")]
        [Alias("PT", "TP", "插播", "差播", "插歌", "差歌", "接著播放")]
        public async Task PlayTop([Remainder]string search)
        {
            if (!check_permission()) return;
            await AudioService.InsertPlayAsync(Context, search);
        }

        [Command("PlayTopSkip")]
        [Alias("PTS", "卡播", "cp", "咖波", "咖播", "卡波")]
        public async Task PlayTopSkip([Remainder]string search)
        {
            if (!check_permission()) return;
            await AudioService.InsertskipPlayAsync(Context, search);
        }

        [Command("RemoveAt")]
        [Alias("刪歌", "R", "Remove", "刪除歌曲", "移除歌曲")]
        public async Task RemoveItem(int index)
        {
            if (!check_permission()) return;
            await AudioService.RemoveItemAsync(Context, index.ToString());
        }

        [Command("RemoveAt")]
        [Alias("刪歌", "R", "Remove", "刪除歌曲", "移除歌曲")]
        public async Task RemoveItem([Remainder]string index)
        {
            if (!check_permission()) return;
            await AudioService.RemoveItemAsync(Context, index);
        }

        [Command("Stop")]
        [Alias("停", "停止", "閉嘴", "取消")]
        public async Task Stop()
        {
            if (!check_permission()) return;
            await AudioService.StopAsync(Context);
        }

        [Command("List", RunMode = RunMode.Async)]
        [Alias("L", "Ls", "Q", "清單", "歌單", "歌曲列表", "queue", "佇列")]
        public async Task List()
        {
            if (!check_permission()) return;
            await AudioService.ListAsync(Context, 1);
        }

        [Command("List", RunMode = RunMode.Async)]
        [Alias("L", "Ls", "Q", "清單", "歌單", "歌曲列表", "queue", "佇列")]
        public async Task List_page(int page)
        {
            if (!check_permission()) return;
            await AudioService.ListAsync(Context, page);
        }

        [Command("Skip")]
        [Alias("S", "跳", "跳過", "切歌", "卡歌", "快轉")]
        public async Task Skip()
        {
            if (!check_permission()) return;
            await AudioService.SkipTrackAsync(Context);
        }

        [Command("Skip", RunMode = RunMode.Async)]
        [Alias("S", "跳", "跳過", "切歌", "卡歌", "快轉")]
        public async Task Skip(string s)
        {
            if (!check_permission()) return;
            await AudioService.SkipTrackAsync(Context, s);
        }

        [Command("Replay")]
        [Alias("重播", "重新", "重來")]
        public async Task Replay()
        {
            if (!check_permission()) return;
            await AudioService.SkipTrackAsync(Context, "0");
        }

        [Command("Volume", RunMode = RunMode.Async)]
        [Alias("VV", "VVV", "VVV", "音量")]
        public async Task Volume()
        {
            if (!check_permission()) return;
            await AudioService.SetVolumeAsync(Context, 0);
        }

        [Command("Volume", RunMode = RunMode.Async)]
        [Alias("V", "VV", "VVV", "VVVV", "音量", "Vol", "Vo")]
        public async Task Volume(int volume)
        {
            if (!check_permission()) return;
            await AudioService.SetVolumeAsync(Context, volume);
        }

        [Command("ChangeVoiceChannel")]
        [Alias("換頻道", "修復", "修正")]
        public async Task ChangeVoiceChannel()
        {
            if (!check_permission_hight()) return;
            await AudioService.ChangeVoiceChannelAsync(Context);
        }

        [Command("Pause")]
        [Alias("暫停", "不要繼續", "不要播")]
        public async Task Pause()
        {
            if (!check_permission()) return;
            await AudioService.PauseAsync(Context);
        }

        [Command("Resume")]
        [Alias("繼續", "不要暫停", "不要停")]
        public async Task Resume()
        {
            if (!check_permission()) return;
            await AudioService.ResumeAsync(Context);
        }

        [Command("loop", RunMode = RunMode.Async)]
        [Alias("循環")]
        public async Task loop()
        {
            if (!check_permission()) return;
            await AudioService.LoopAsync(Context);
        }

        [Command("Random")]
        [Alias("R", "隨機", "打亂")]
        public async Task random()
        {
            if (!check_permission()) return;
            await AudioService.RandomAsync(Context);
        }


        [Command("FindRepeated", RunMode = RunMode.Async)]
        [Alias("找重複", "重複的歌", "查重複")]
        public async Task FindRepeated()
        {
            if (!check_permission()) return;
            await AudioService.FindRepeatedAsync(Context);
        }

        [Command("FindList", RunMode = RunMode.Async)]
        [Alias("找歌", "尋找", "找", "find")]
        public async Task FindList(string the_text)
        {
            if (!check_permission()) return;
            await AudioService.FindListAsync(Context, the_text);
        }

        [Command("FindTrackLength", RunMode = RunMode.Async)]
        [Alias("找大於")]
        public async Task FindLength(string s)
        {
            if (!check_permission()) return;
            await AudioService.FindLengthAsync(Context, s);
        }


        [Command("move")]
        [Alias("交換", "移動", "移歌", "m")]
        public async Task Switchitem(int index1,int index2)
        {
            if (!check_permission()) return;
            await AudioService.SwitchAsync(Context, index1, index2);
        }

        [Command("move")]
        [Alias("至頂", "置頂", "top", "上移", "交換", "移動", "移歌", "m")]
        public async Task Switchitem(int index1)
        {
            if (!check_permission()) return;
            await AudioService.SwitchAsync(Context, index1);
        }
        /*
        [Command("showplaylist")]
        [Alias("蔚藍清單", "SSL")]
        public async Task ShowList()
        {
            if (!check_permission()) return;
            await AudioService.ShowListAsync(Context, "");
        }

        [Command("showplaylist")]
        [Alias("蔚藍清單", "SSL", "SSList")]
        public async Task ShowList(string listname)
        {
            if (!check_permission()) return;
            await AudioService.ShowListAsync(Context, listname);
        }

        [Command("playplaylist")]
        [Alias("播放蔚藍清單", "播蔚藍清單", "play蔚藍清單", "p蔚藍清單", "SSP", "SSPlay")]
        public async Task PlayPlaylist(string listname)
        {
            if (!check_permission()) return;
            await AudioService.PlayPlaylistAsync(Context, listname);
        }

        [Command("editplaylist")]
        [Alias("修改蔚藍清單", "更新蔚藍清單", "SSEdit", "SSE")]
        public async Task EditPlaylist(string listname, [Remainder]string content)
        {
            if (!check_permission_hight()) return;
            await AudioService.EditPlaylistAsync(Context, listname, content);
        }

        [Command("deleteplaylist")]
        [Alias("刪除蔚藍清單", "SSD", "SSDelete")]
        public async Task deletePlaylist(string listname)
        {
            if (!check_permission_hight()) return;
            await AudioService.deletePlaylistAsync(Context, listname);
        }
        */
        [Command("Lyrics")]
        [Alias("歌詞")]
        public async Task CallLyrics()
        {
            if (!check_permission()) return;
            await AudioService.CallLyricsAsync(Context);
        }

        [Command("PlayProgres", RunMode = RunMode.Async)]
        [Alias("進度", "時間")]
        public async Task PlayProgres()
        {
            if (!check_permission()) return;
            await AudioService.PlayProgresAsync(Context);
        }


        [Command("Save", RunMode = RunMode.Async)]
        [Alias("儲存", "存檔", "存")]
        public async Task SaveNowList(string filename)
        {
            if (!check_permission_hight()) return;
            await AudioService.SaveNowListAsync(Context, filename);
        }

        [Command("Load", RunMode = RunMode.Async)]
        [Alias("讀取", "讀檔", "讀", "取")]
        public async Task LoadNowList(string filename)
        {
            if (!check_permission_hight()) return;
            await AudioService.LoadNowListAsync(Context, filename);
        }

        [Command("ShowSave", RunMode = RunMode.Async)]
        [Alias("顯示存檔", "存檔顯示", "檢視存檔", "存檔檢視", "查看存檔", "存檔查看", "看存檔")]
        public async Task ShowSave()
        {
            if (!check_permission_hight()) return;
            await AudioService.ShowSaveAsync(Context, "");
        }

        [Command("ShowSave", RunMode = RunMode.Async)]
        [Alias("顯示存檔", "存檔顯示", "檢視存檔", "存檔檢視", "查看存檔", "存檔查看", "看存檔")]
        public async Task ShowSave(string filename)
        {
            if (!check_permission_hight()) return;
            await AudioService.ShowSaveAsync(Context, filename);
        }

        [Command("DeleteSave", RunMode = RunMode.Async)]
        [Alias("刪除存檔", "存檔刪除")]
        public async Task DeleteSave(string filename)
        {
            if (!check_permission_hight()) return;
            await AudioService.DeleteSaveAsync(Context, filename);
        }


        [Command("Recode")]
        [Alias("點歌記錄", "記錄", "紀錄", "log")]
        public async Task SOR()
        {
            if (!check_permission()) return;
            await AudioService.SeeOrderRecord(Context, Context.User as SocketGuildUser);
        }


        [Command("Recode")]
        [Alias("點歌記錄", "記錄", "清單", "紀錄", "log")]
        public async Task SOR(SocketGuildUser userName)
        {
            if (!check_permission_hight()) return;
            await AudioService.SeeOrderRecord(Context, userName);
        }

        [Command("DeleteRecode")]
        [Alias("刪除點歌記錄", "刪除記錄", "刪除紀錄", "deletelog")]
        public async Task DeleteSOR()
        {
            if (!check_permission()) return;
            await AudioService.DeleteOrderRecord(Context, Context.User as SocketGuildUser);
        }

        [Command("DeleteRecode")]
        [Alias("刪除點歌記錄", "刪除記錄", "刪除紀錄", "deletelog")]
        public async Task DeleteSOR(SocketGuildUser userName)
        {
            if (!check_permission_hight()) return;
            await AudioService.DeleteOrderRecord(Context, userName);
        }


        [Command("HotShow", RunMode = RunMode.Async)]
        [Alias("熱門", "顯示熱門", "熱門顯示", "Hot")]
        public async Task HotShow()
        {
            if (!check_permission()) return;
            await AudioService.HotShowAsync(Context);
        }

        [Command("Hotplay", RunMode = RunMode.Async)]
        [Alias("播熱門", "播放熱門", "P熱門", "熱門播放", "熱門播", "加入熱門", "讀熱門")]
        public async Task HotPlay()
        {
            if (!check_permission()) return;
            await AudioService.HotPlayAsync(Context, 5);
        }

        [Command("Hotplay", RunMode = RunMode.Async)]
        [Alias("播熱門", "播放熱門", "P熱門", "熱門播放", "熱門播", "加入熱門", "讀熱門")]
        public async Task HotPlay(int num)
        {
            if (!check_permission()) return;
            await AudioService.HotPlayAsync(Context, num);
        }




        private Boolean check_permission()
        {
            IVoiceState voiceState = Context.User as IVoiceState;

            int is_permission = 0;
            if (check_Channel(Context.Message, SCLD.點歌專區id) && check_VoiceChannel(Context.User as IVoiceState, SCLD.蔚藍音樂id)) is_permission += 10;
            if (check_Channel(Context.Message, SCLD.點歌專區id) && check_VoiceChannel(Context.User as IVoiceState, SCLD.幻境音樂id)) is_permission += 10;
            if (check_Channel(Context.Message, SCLD.vic打字頻道) && check_VoiceChannel(Context.User as IVoiceState, SCLD.vic講話頻道)) is_permission += 10;
            if (check_Channel(Context.Message, SCLD.vic好友打字頻道) && check_VoiceChannel(Context.User as IVoiceState, SCLD.vic好友講話頻道)) is_permission += 10;
            if (check_Channel(Context.Message, SCLD.vic實體打字頻道) && check_VoiceChannel(Context.User as IVoiceState, SCLD.vic實體講話頻道)) is_permission += 10;
            if (check_Channel(Context.Message, SCLD.vic測試打字頻道) && check_VoiceChannel(Context.User as IVoiceState, SCLD.vic測試講話頻道)) is_permission += 10;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (Context.User.Id == SCLD.vicid) is_permission += 999;
            if (Context.User.Id == SCLD.vic14777id) is_permission += 999;

            if (is_permission > 0)
                return true;
            else
                return false;
        }

        private Boolean check_permission_hight()
        {
            IVoiceState voiceState = Context.User as IVoiceState;

            int is_permission = 0;
            if (check_Role(Context.User, SCLD.Super_OPid)) is_permission += 1000;
            if (Context.User.Id == SCLD.vicid) is_permission += 999;
            if (Context.User.Id == SCLD.vic14777id) is_permission += 999;
            if (check_Channel(Context.Message, SCLD.vic測試打字頻道) && check_VoiceChannel(Context.User as IVoiceState, SCLD.vic測試講話頻道)) is_permission += 10;

            if (is_permission > 0)
                return true;
            else
                return false;
        }

        private Boolean check_Channel(SocketMessage now_chid, ulong the_chid)
        {
            return now_chid.Channel.Id == the_chid;
        }

        private Boolean check_VoiceChannel(IVoiceState voiceState, ulong the_chid)
        {
            return voiceState.VoiceChannel.Id == the_chid;
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
