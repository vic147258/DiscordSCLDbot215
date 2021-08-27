using Discord;
using Discord.WebSocket;
using DiscordSCLDbot215.Handlers;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Victoria;
using Victoria.EventArgs;
using Victoria.Enums;
using Victoria.Responses.Rest;
using Discord.Commands;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Globalization;
using Newtonsoft.Json;
using DiscordSCLDbot215.DataStructs;

namespace DiscordSCLDbot215.Services
{
    public sealed class LavaLinkAudio
    {
        private readonly LavaNode _lavaNode;

        public LavaLinkAudio(LavaNode lavaNode)
            => _lavaNode = lavaNode;


        #region 加入
        public async Task JoinAsync(SocketCommandContext Context)
        {
            await LoggingService.LogInformationAsync("Music", $"收到指令({(Context.User as IGuildUser).Nickname})：{Context.Message}");

            await Context.Message.DeleteAsync();

            if (_lavaNode.HasPlayer(Context.Guild))
            {
                await Context.Channel.SendMessageAsync(
                 embed: await EmbedHandler.CreateErrorEmbed(SCLD.music_name, "我已經到 " + (Context.User as IVoiceState).VoiceChannel.Name + "\nby " + Context.User.Mention));
                return;
            }

            if ((Context.User as IVoiceState).VoiceChannel is null)
            {
                await Context.Channel.SendMessageAsync(
                 embed: await EmbedHandler.CreateErrorEmbed(SCLD.music_name + " - 加入", "你必須連接到一個語音頻道!\nby " + Context.User.Mention));
                return;
            }

            try
            {
                await _lavaNode.JoinAsync((Context.User as IVoiceState).VoiceChannel, (Context.Channel as ITextChannel));
                await Context.Channel.SendMessageAsync(
                 embed: await EmbedHandler.CreateBasicEmbed(SCLD.music_name + " - 加入", $"已加入 {(Context.User as IVoiceState).VoiceChannel.Name}\nby " + Context.User.Mention, Color.Green));
            }
            catch (Exception ex)
            {
                await Context.Channel.SendMessageAsync(
                 embed: await EmbedHandler.CreateErrorEmbed(SCLD.music_name + " - 加入", ex.Message + $"\nby {Context.User.Mention}"));
            }
        }
        #endregion

        #region 播放
        public async Task PlayAsync(SocketCommandContext Context, string Searchquery)
        {
            await LoggingService.LogInformationAsync("Music", $"收到指令({(Context.User as IGuildUser).Nickname})：{Context.Message}");
            await Context.Channel.TriggerTypingAsync();

            #region 自動進入設定
            if ((Context.User as IVoiceState).VoiceChannel is null)
            {
                await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed(SCLD.music_name + "", "你必須先加入一個語音頻道"));
                return;
            }

            //Check the guild has a player available.
            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                try
                {
                    await _lavaNode.JoinAsync((Context.User as IVoiceState).VoiceChannel, Context.Channel as ITextChannel);
                }
                catch (Exception ex)
                {
                    await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed(SCLD.music_name + " - 加入", ex.Message));
                }
                //return await EmbedHandler.CreateErrorEmbed("Music, Play", "我沒辦法加入一個語音頻道");
            }
            #endregion

            await Context.Message.DeleteAsync();

            try
            {
                OrderRecord(Context, Searchquery);
            }
            catch { }

            try
            {
                var player = _lavaNode.GetPlayer(Context.Guild);

                #region 音量設定
                await player.UpdateVolumeAsync(read_Volume(Context.Guild.Id));
                #endregion

                LavaTrack first_track = null;

                String show_text = "";
                int song_index = player.Queue.Count + 1;
                int show_count = 5;
                int count_qu = 0;

                Boolean is_play = false;

                if (player.Track != null && player.PlayerState is PlayerState.Playing || player.PlayerState is PlayerState.Paused)
                    is_play = true;

                foreach (string query in Searchquery.Split(Convert.ToChar(10).ToString()))
                {
                    int Searchquery_type = 0;  //1是網址  //2是搜尋

                    var search = Uri.IsWellFormedUriString(query, UriKind.Absolute) ?
                        await _lavaNode.SearchAsync(query)
                           : await _lavaNode.SearchYouTubeAsync(query);
                    //SearchResponse search = new SearchResponse();
                    if (Uri.IsWellFormedUriString(query, UriKind.Absolute))
                    {
                        //search = await _lavaNode.SearchAsync(query);
                        Searchquery_type = 1;
                    }
                    else
                    {
                        //search = await _lavaNode.SearchYouTubeAsync(query);
                        Searchquery_type = 2;
                    }

                    if (search.LoadStatus == LoadStatus.NoMatches)
                    {
                        //await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed(SCLD.music_name, $"我沒辦法找到關於 {query} 的資料"));
                        //return;

                        if (count_qu <= show_count)
                        {
                            count_qu++;
                            show_text += $"找不到「{query}」\n";
                        }
                        continue;
                    }



                    #region 網址跑這邊
                    LavaTrack track;

                    if (Searchquery_type == 1) //網址
                    {
                        //是播放清單

                        foreach (LavaTrack temp_trck in search.Tracks)
                        {
                            HotRecord(Context, search.Tracks.ToList());  //熱門紀錄

                            String trackInfo = $"[{temp_trck.Title}]({temp_trck.Url})\n";

                            if (first_track != null || is_play)
                            {
                                count_qu++;
                                player.Queue.Enqueue(temp_trck);
                                if (count_qu <= show_count)
                                {
                                    show_text += $"{song_index}. ";
                                    show_text += trackInfo;
                                }
                            }
                            else
                            {
                                count_qu++;
                                show_text += $"{song_index}. ";
                                show_text += trackInfo;
                                first_track = temp_trck;
                            }
                            song_index++;
                        }
                    }
                    #endregion

                    #region 一般文字跑這邊

                    if (Searchquery_type == 2) //字丟給YT搜尋
                    {
                        track = search.Tracks.FirstOrDefault();

                        HotRecord(Context, new List<LavaTrack> { track });//熱門紀錄

                        String trackInfo = "";
                        trackInfo += $"[{track.Title}]({track.Url})\n";

                        if (first_track != null || is_play)
                        {
                            count_qu++;
                            player.Queue.Enqueue(track);
                            if (count_qu <= show_count)
                            {
                                show_text += $"{song_index}. ";
                                show_text += trackInfo;
                            }
                        }
                        else
                        {
                            count_qu++;
                            show_text += $"{song_index}. ";
                            show_text += trackInfo;
                            first_track = track;
                        }
                    }
                    #endregion

                    song_index++;
                }  //大迴圈結束

                if (count_qu <= show_count)
                {
                    await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed(SCLD.music_name + " - 已加入清單", show_text + $"\nby {Context.User.Mention}\n", Color.Blue));
                }
                else
                {
                    await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed(SCLD.music_name + " - 已加入清單", show_text + $"與其他 {count_qu - show_count} 首歌被加入待播清單" + $"\n\nby {Context.User.Mention}\n", Color.Blue));
                }

                if (first_track != null)
                    await player.PlayAsync(first_track);
                else
                    await UpdateNowPlay(Context.Guild);

            }
            catch (Exception ex)
            {
                await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed(SCLD.music_name + " - 播放", ex.Message));
            }

        }
        #endregion

        #region 插入播放
        public async Task InsertPlayAsync(SocketCommandContext Context, string query)
        {
            await LoggingService.LogInformationAsync("Music", $"收到指令({(Context.User as IGuildUser).Nickname})：{Context.Message}");

            #region 自動進入設定
            if ((Context.User as IVoiceState).VoiceChannel is null)
            {
                await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed(SCLD.music_name + "", "你必須先加入一個語音頻道"));
                return;
            }

            //Check the guild has a player available.
            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                try
                {
                    await _lavaNode.JoinAsync((Context.User as IVoiceState).VoiceChannel, Context.Channel as ITextChannel);
                }
                catch (Exception ex)
                {
                    await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed(SCLD.music_name + " - 加入", ex.Message));
                }
                //return await EmbedHandler.CreateErrorEmbed("Music, Play", "我沒辦法加入一個語音頻道");
            }
            #endregion

            await Context.Channel.TriggerTypingAsync();

            await Context.Message.DeleteAsync();

            try
            {
                OrderRecord(Context, query);
            }
            catch { }

            try
            {
                //Get the player for that guild.
                var player = _lavaNode.GetPlayer(Context.Guild);

                #region 音量設定
                await player.UpdateVolumeAsync(read_Volume(Context.Guild.Id));
                #endregion

                //Find The Youtube Track the User requested.
                LavaTrack track;

                var search = Uri.IsWellFormedUriString(query, UriKind.Absolute) ?
                    await _lavaNode.SearchAsync(query)
                    : await _lavaNode.SearchYouTubeAsync(query);

                //If we couldn't find anything, tell the user.
                if (search.LoadStatus == LoadStatus.NoMatches)
                {
                    await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed(SCLD.music_name, $"我沒辦法找到關於 {query} 的資料"));
                    return;
                }

                track = search.Tracks.FirstOrDefault();
                HotRecord(Context, new List<LavaTrack>() { track });//熱門紀錄
                String trackInfo = "";
                trackInfo += $"[{track.Title}]({track.Url})\n";
                trackInfo += $"{track.Duration}\n";
                trackInfo += $"by {Context.User.Mention}\n";


                if (player.Track != null && player.PlayerState is PlayerState.Playing || player.PlayerState is PlayerState.Paused)
                {
                    DefaultQueue<LavaTrack> temp = new DefaultQueue<LavaTrack>();

                    foreach (LavaTrack track000 in player.Queue.Reverse())
                    {
                        temp.Enqueue(track000);
                    }

                    temp.Enqueue(track);

                    player.Queue.Clear();

                    foreach (LavaTrack track000 in temp.Reverse())
                    {
                        player.Queue.Enqueue(track000);
                    }

                    //await LoggingService.LogInformationAsync(SCLD.music_name + " - 已插入清單", $"{track.Title}");
                    await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed(SCLD.music_name + " - 已插入清單", "1. " + trackInfo, Color.Blue));
                    await UpdateNowPlay(Context.Guild);
                    return;
                }

                await player.PlayAsync(track);
                await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed(SCLD.music_name + " - 已插入清單", "1. " + trackInfo, Color.Blue));
            }
            catch (Exception ex)
            {
                await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed(SCLD.music_name + " - 播放", ex.Message));
            }

        }
        #endregion

        #region 卡歌插入播放
        public async Task InsertskipPlayAsync(SocketCommandContext Context, string query)
        {
            await LoggingService.LogInformationAsync("Music", $"收到指令({(Context.User as IGuildUser).Nickname})：{Context.Message}");

            #region 自動進入設定
            if ((Context.User as IVoiceState).VoiceChannel is null)
            {
                await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed(SCLD.music_name + "", "你必須先加入一個語音頻道"));
                return;
            }

            //Check the guild has a player available.
            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                try
                {
                    await _lavaNode.JoinAsync((Context.User as IVoiceState).VoiceChannel, Context.Channel as ITextChannel);
                }
                catch (Exception ex)
                {
                    await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed(SCLD.music_name + " - 加入", ex.Message));
                }
                //return await EmbedHandler.CreateErrorEmbed("Music, Play", "我沒辦法加入一個語音頻道");
            }
            #endregion

            await Context.Channel.TriggerTypingAsync();

            await Context.Message.DeleteAsync();

            try
            {
                //Get the player for that guild.
                var player = _lavaNode.GetPlayer(Context.Guild);

                #region 音量設定
                await player.UpdateVolumeAsync(read_Volume(Context.Guild.Id));
                #endregion

                //Find The Youtube Track the User requested.
                LavaTrack track;

                var search = Uri.IsWellFormedUriString(query, UriKind.Absolute) ?
                    await _lavaNode.SearchAsync(query)
                    : await _lavaNode.SearchYouTubeAsync(query);

                try
                {
                    OrderRecord(Context, query);
                }
                catch { }

                //If we couldn't find anything, tell the user.
                if (search.LoadStatus == LoadStatus.NoMatches)
                {
                    await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed(SCLD.music_name, $"我沒辦法找到關於 {query} 的資料"));
                    return;
                }

                track = search.Tracks.FirstOrDefault();

                HotRecord(Context, new List<LavaTrack>() { track });//熱門紀錄

                //await Context.Message.DeleteAsync();

                if (player.Track != null && player.PlayerState is PlayerState.Playing || player.PlayerState is PlayerState.Paused)
                {
                    DefaultQueue<LavaTrack> temp = new DefaultQueue<LavaTrack>();

                    foreach (LavaTrack track000 in player.Queue.Reverse())
                    {
                        temp.Enqueue(track000);
                    }

                    temp.Enqueue(track);

                    player.Queue.Clear();

                    foreach (LavaTrack track000 in temp.Reverse())
                    {
                        player.Queue.Enqueue(track000);
                    }

                    await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed(SCLD.music_name + " - 卡歌", $"當前歌曲取消播放：\n[{player.Track.Title}]({player.Track.Url})\n改為播放：\n[{track.Title}]({track.Url})" + $"\nby {Context.User.Mention}", Color.DarkRed));

                    await player.SkipAsync();
                    return;
                }
                await player.PlayAsync(track);

            }
            catch (Exception ex)
            {
                await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed(SCLD.music_name + " - 播放", ex.Message));
            }

        }
        #endregion

        #region 刪除歌曲
        public async Task RemoveItemAsync(SocketCommandContext Context, string sindex)
        {
            await LoggingService.LogInformationAsync("Music", $"收到指令({(Context.User as IGuildUser).Nickname})：{Context.Message}");

            await Context.Message.DeleteAsync();

            try
            {
                var player = _lavaNode.GetPlayer(Context.Guild);
                List<int> delete_index = new List<int>();

                sindex = sindex.Replace("～", "~");
                if (sindex.IndexOf("~") > 0)
                {
                    string[] rang = sindex.Replace(" ", "").Split("~");
                    try
                    {
                        int ind_s = int.Parse(rang[0]);
                        int ind_e = int.Parse(rang[1]);
                        if (ind_s > ind_e)
                        {
                            ind_s = ind_s ^ ind_e;
                            ind_e = ind_s ^ ind_e;
                            ind_s = ind_s ^ ind_e;
                        }
                        if (ind_s > player.Queue.Count || ind_s < 1 || ind_e > player.Queue.Count || ind_e < 1)
                        {
                            await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed(SCLD.music_name + " - 移除歌曲", $"請輸清單範圍內的編號(1 ~ {player.Queue.Count})\nby { Context.User.Mention}", Color.DarkRed));
                            return;
                        }
                        for (int i = ind_s; i <= ind_e; i++)
                            delete_index.Add(i);

                    }
                    catch
                    {
                        await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed(SCLD.music_name + " - 移除歌曲", $"輸入數字不正確", Color.DarkRed));
                        return;
                    }
                }
                else if (sindex.IndexOf(",") > 0)
                {
                    string[] rang = sindex.Replace(" ", "").Split(",");
                    try
                    {
                        foreach (string indexxx in rang)
                            delete_index.Add(int.Parse(indexxx));
                    }
                    catch
                    {
                        await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed(SCLD.music_name + " - 移除歌曲", $"輸入數字不正確", Color.DarkRed));
                        return;
                    }
                }
                else
                {
                    string[] rang = sindex.Split(" ");
                    try
                    {
                        foreach (string indexxx in rang)
                            delete_index.Add(int.Parse(indexxx));
                    }
                    catch
                    {
                        await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed(SCLD.music_name + " - 移除歌曲", $"輸入數字不正確", Color.DarkRed));
                        return;
                    }
                }

                List<LavaTrack> del_tracks = new List<LavaTrack>();
                DefaultQueue<LavaTrack> newQueue = new DefaultQueue<LavaTrack>();

                int tempindex = 1;
                foreach (LavaTrack track000 in player.Queue)
                {
                    if (!delete_index.Contains(tempindex))
                        newQueue.Enqueue(track000);
                    else
                        del_tracks.Add(track000);
                    tempindex++;
                }

                player.Queue.Clear();

                foreach (LavaTrack track000 in newQueue)
                {
                    player.Queue.Enqueue(track000);
                }

                //await LoggingService.LogInformationAsync(SCLD.music_name + " - 已移除歌曲", $"[{del_track.Title}]({del_track.Url})");

                String infoText = "";
                foreach (LavaTrack del_track in del_tracks)
                {
                    infoText += $"{del_track.Title}\n";
                }

                if (infoText.Length > 1850) //避免爆掉而已
                    infoText = infoText.Substring(0, 1900);

                infoText += $"\nby {Context.User.Mention}\n";
                await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed(SCLD.music_name + " - 已移除歌曲", infoText, Color.Red));
                await UpdateNowPlay(Context.Guild);
            }
            catch (Exception ex)
            {
                await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed(SCLD.music_name + " - 刪歌", ex.Message));
            }
        }
        #endregion

        #region 播放進度
        public async Task PlayProgresAsync(SocketCommandContext Context)
        {
            await LoggingService.LogInformationAsync("Music", $"收到指令({(Context.User as IGuildUser).Nickname})：{Context.Message}");

            await Context.Message.DeleteAsync();

            try
            {

                var player = _lavaNode.GetPlayer(Context.Guild);

                string show_text = "";

                if (player.Track == null)
                {
                    IUserMessage the_messageerr = await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed(SCLD.music_name + " - 播放進度", "沒有歌曲正在播放" + $"\nby {Context.User.Mention}", Color.DarkBlue));
                    Thread.Sleep(5000);
                    await the_messageerr.DeleteAsync();
                    return;
                }

                TimeSpan TS = player.Track.Position;

                double nowtimedouble = player.Track.Position.TotalSeconds;
                double alltimedouble = player.Track.Duration.TotalSeconds;

                if (player.Track.IsStream)
                {
                    show_text += "正在播放的連結是直播\n已播放時間：";
                    show_text += $"`{time_swap(TS)}`";
                }
                else
                {
                    show_text += "`";
                    Boolean is_use = false;
                    for (int i = 0; i <= 28; i++)
                    {
                        if (i >= Math.Ceiling((nowtimedouble / alltimedouble) * 100 * 0.28) && !is_use)
                        {
                            show_text += "🔘";
                            is_use = true;
                        }
                        else
                            show_text += "▬";
                    }
                    show_text += "`\n";
                    show_text += "`" + time_swap(player.Track.Position) + " / " + time_swap(player.Track.Duration) + "`\n";
                }


                IUserMessage the_message = await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed(SCLD.music_name + " - 播放進度", show_text + $"\nby {Context.User.Mention}", Color.DarkBlue));
                Thread.Sleep(20000);
                await the_message.DeleteAsync();
            }
            catch (InvalidOperationException ex)
            {
                await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed(SCLD.music_name + " - 進度", ex.Message + $"\nby {Context.User.Mention}"));
            }
        }
        #endregion

        #region 離開
        public async Task LeaveAsync(SocketCommandContext Context)
        {
            await LoggingService.LogInformationAsync("Music", $"收到指令({(Context.User as IGuildUser).Nickname})：{Context.Message}");

            try
            {
                if (!_lavaNode.TryGetPlayer(Context.Guild, out LavaPlayer player))
                {
                    await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed(SCLD.music_name, $"我不在任何一個頻道。" + $"\nby {Context.User.Mention}", Color.DarkGrey));
                    return;
                }

                //if The Player is playing, Stop it.
                if (player.PlayerState is PlayerState.Playing)
                {
                    await player.StopAsync();
                }

                player.Queue.Clear();

                await _lavaNode.LeaveAsync(player.VoiceChannel);

                await Context.Message.DeleteAsync();
                //await LoggingService.LogInformationAsync(SCLD.music_name, $"我已經離開了。");
                await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed(SCLD.music_name, $"感謝您的聆聽，我已經離開語音頻道了。" + $"\nby {Context.User.Mention}", Color.DarkGrey));
            }
            catch (InvalidOperationException ex)
            {
                await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed(SCLD.music_name + "離開", ex.Message + $"\nby {Context.User.Mention}"));
            }

        }
        #endregion

        #region 清單
        public async Task ListAsync(SocketCommandContext Context, int page)
        {
            await LoggingService.LogInformationAsync("Music", $"收到指令({(Context.User as IGuildUser).Nickname})：{Context.Message}");

            await Context.Message.DeleteAsync();

            try
            {
                /* Create a string builder we can use to format how we want our list to be displayed. */
                var descriptionBuilder = new StringBuilder();

                /* Get The Player and make sure it isn't null. */
                var player = _lavaNode.GetPlayer(Context.Guild);
                if (player == null)
                {
                    await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed(SCLD.music_name + " - 清單", $"無法取得播放者\n你確定現在要使用這個機器人? 確認 {SCLD.DefaultPrefix}Help 來獲得更多關於這個機器人的訊息" + $"\nby {Context.User.Mention}"));
                    return;
                }
                if (player.PlayerState is PlayerState.Playing)
                {
                    /*If the queue count is less than 1 and the current track IS NOT null then we wont have a list to reply with.
                        In this situation we simply return an embed that displays the current track instead. */
                    if (player.Queue.Count < 1 && player.Track != null)
                    {
                        IUserMessage the_messageerr = await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed(SCLD.music_name + " - 清單", $"正在播放：\n[{player.Track.Title}]({player.Track.Url})\n清單中沒有歌曲" + $"\nby {Context.User.Mention}", Color.DarkRed));
                        Thread.Sleep(20000);
                        await the_messageerr.DeleteAsync();
                        return;
                    }
                    else
                    {
                        int pageSize = 15;
                        int maxpage = player.Queue.Count / pageSize;
                        maxpage += (player.Queue.Count % pageSize != 0) ? 1 : 0;

                        if (page < 1 || page > maxpage)
                        {
                            IUserMessage the_messageerr = await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed(SCLD.music_name + " - 清單", $"正在播放：\n[{player.Track.Title}]({player.Track.Url})\n請輸入正確頁碼 1 ~ {maxpage}" + $"\nby {Context.User.Mention}", Color.DarkRed));
                            Thread.Sleep(20000);
                            await the_messageerr.DeleteAsync();
                            return;
                        }

                        int trackNum = 1;
                        foreach (LavaTrack track in player.Queue)
                        {
                            if (trackNum > (page - 1) * pageSize && trackNum < page * pageSize + 1)
                            {
                                descriptionBuilder.Append($"{trackNum}. [{track.Title}]({track.Url})\n");
                            }
                            trackNum++;
                        }

                        IUserMessage the_message = await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed(SCLD.music_name + " - 清單", $"正在播放：\n[{player.Track.Title}]({player.Track.Url})\n待播清單({player.Queue.Count}首歌, {page}/{maxpage})：\n{descriptionBuilder}" + $"\nby {Context.User.Mention}", Color.Green));
                        Thread.Sleep(60000);
                        await the_message.DeleteAsync();
                    }
                }
                else
                {
                    IUserMessage the_message = await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed(SCLD.music_name + " - 清單", "目前沒有播放任何東西。" + $"\nby {Context.User.Mention}"));
                    Thread.Sleep(20000);
                    await the_message.DeleteAsync();
                }
            }
            catch (Exception ex)
            {
                await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed(SCLD.music_name + " - 清單", ex.Message + $"\nby {Context.User.Mention}"));
            }

        }
        #endregion

        #region 跳過
        public async Task SkipTrackAsync(SocketCommandContext Context)
        {
            await LoggingService.LogInformationAsync("Music", $"收到指令({(Context.User as IGuildUser).Nickname})：{Context.Message}");

            await Context.Message.DeleteAsync();
            try
            {
                var player = _lavaNode.GetPlayer(Context.Guild);

                /* Check if the player exists */
                if (player == null)
                    await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed(SCLD.music_name + " - 清單", $"無法取得播放者\n你確定現在要使用這個機器人? 確認 {SCLD.DefaultPrefix}Help 來獲得更多關於這個機器人的訊息" + $"\nby {Context.User.Mention}"));

                if (player.Queue.Count < 1)
                {
                    try
                    {
                        var currentTrack = player.Track;
                        await player.StopAsync();
                        //await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed(SCLD.music_name + " - 卡歌", $"無法跳過最後一首歌，或者是無歌曲正在播放" + $"\n\n或許你可以輸入 {SCLD.DefaultPrefix}停止" + $"\nby {Context.User.Mention}"));
                        await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed(SCLD.music_name + " - 卡歌", $"已將最後一首歌停止播放\n[{currentTrack.Title}]({currentTrack.Url})" + $"\nby {Context.User.Mention}", Color.Red));
                    }
                    catch (Exception ex)
                    {
                        await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed(SCLD.music_name + " - 卡歌", ex.Message + $"\nby {Context.User.Mention}"));
                    }
                }
                else
                {
                    try
                    {
                        /* Save the current song for use after we skip it. */
                        var currentTrack = player.Track;
                        /* Skip the current song. */
                        //await LoggingService.LogInformationAsync(SCLD.music_name, $"卡歌: {currentTrack.Title}");
                        await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed(SCLD.music_name + " - 卡歌", $"我已將當前歌曲取消播放\n[{currentTrack.Title}]({currentTrack.Url})" + $"\nby {Context.User.Mention}", Color.DarkRed));

                        await player.SkipAsync();

                        //await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed("正在播放：", $"[{player.Track.Title}]({player.Track.Url})", Color.Blue));

                    }
                    catch (Exception ex)
                    {
                        await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed(SCLD.music_name + " - 卡歌", ex.Message + $"\nby {Context.User.Mention}"));
                    }

                }
            }
            catch (Exception ex)
            {
                await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed(SCLD.music_name + " - 卡歌", ex.Message + $"\nby {Context.User.Mention}"));
            }
        }

        public async Task SkipTrackAsync(SocketCommandContext Context, string cTS)
        {
            await Context.Message.DeleteAsync();

            TimeSpan TS; // = TimeSpan.Parse(cTS);

            try
            {
                var player = _lavaNode.GetPlayer(Context.Guild);

                if (player.Track.IsStream)
                {
                    IUserMessage the_messageerr = await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed(SCLD.music_name + " - 快轉", $"直播無法快轉" + $"\nby {Context.User.Mention}", Color.Red));
                    Thread.Sleep(5000);
                    await the_messageerr.DeleteAsync();
                    return;
                }

                int hhh = 0, mmm = 0, sss = 0;

                string[] timestring = cTS.Split(":");

                int temp_A = 0;
                for (int i = timestring.Length - 1; i >= 0; i--)
                {
                    if (temp_A == 0)
                        sss = int.Parse(timestring[i]);
                    if (temp_A == 1)
                        mmm = int.Parse(timestring[i]);
                    if (temp_A == 2)
                        hhh = int.Parse(timestring[i]);
                    temp_A++;
                }

                TS = new TimeSpan(hhh, mmm, sss);

                await player.SeekAsync(TS);
                IUserMessage the_message = await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed(SCLD.music_name + " - 快轉", $"[{player.Track.Title}]({player.Track.Url})\n已快轉至 {time_swap(TS)}" + $"\nby {Context.User.Mention}", Color.Green));
                Thread.Sleep(5000);
                await the_message.DeleteAsync();
            }
            catch (Exception ex)
            {
                await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed(SCLD.music_name + " - 快轉", "您選的時間可能超過歌曲長度\n" + ex.Message + $"\nby {Context.User.Mention}"));
            }
        }
        #endregion

        #region 停止
        public async Task StopAsync(SocketCommandContext Context)
        {
            await LoggingService.LogInformationAsync("Music", $"收到指令({(Context.User as IGuildUser).Nickname})：{Context.Message}");

            await Context.Message.DeleteAsync();

            try
            {
                var player = _lavaNode.GetPlayer(Context.Guild);

                if (player == null)
                {
                    await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed(SCLD.music_name + " - 清單", $"無法取得播放者\n你確定現在要使用這個機器人? 確認 {SCLD.DefaultPrefix}Help 來獲得更多關於這個機器人的訊息" + $"\nby {Context.User.Mention}"));
                    return;
                }

                /* Check if the player exists, if it does, check if it is playing.
                     If it is playing, we can stop.*/
                if (player.PlayerState is PlayerState.Playing)
                {
                    await player.StopAsync();
                    //player.Queue.Clear();
                }

                //await LoggingService.LogInformationAsync(SCLD.music_name, $"機器人已停止");
                await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed("Music Stop", "我已經停止播放，播放清單也已清空。" + $"\nby {Context.User.Mention}", Color.Blue));
                await ClearAsync(Context);
            }
            catch (Exception ex)
            {
                await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed(SCLD.music_name + "停止", ex.Message));
            }
        }

        public async Task ClearAsync(SocketCommandContext Context)
        {
            var player = _lavaNode.GetPlayer(Context.Guild);
            Thread.Sleep(3000);
            player.Queue.Clear();
        }
        #endregion

        #region 設定音量
        public async Task SetVolumeAsync(SocketCommandContext Context, int volume)
        {
            await LoggingService.LogInformationAsync("Music", $"收到指令({(Context.User as IGuildUser).Nickname})：{Context.Message}");

            await Context.Message.DeleteAsync();

            if (volume == 0)
            {
                try
                {
                    if (!_lavaNode.TryGetPlayer(Context.Guild, out LavaPlayer player))
                    {
                        await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed(SCLD.music_name, $"未在播放中，無法讀取音量。" + $"\nby {Context.User.Mention}", Color.DarkGrey));
                        return;
                    }
                    IUserMessage the_message = await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed(SCLD.music_name + " - 音量", $"當前音量為 {player.Volume}" + $"\nby {Context.User.Mention}", Color.Green));
                    Thread.Sleep(20000);
                    await the_message.DeleteAsync();
                    return;
                }
                catch (InvalidOperationException ex)
                {
                    await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed(SCLD.music_name + " - 音量", ex.Message + $"\nby {Context.User.Mention}"));
                    return;
                }
            }

            if (volume > 200 || volume <= 0)
            {
                await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed(SCLD.music_name + " - 音量", $"音量的範圍為 1 到 200" + $"\nby {Context.User.Mention}", Color.DarkRed));
                return;
            }



            try
            {
                var player = _lavaNode.GetPlayer(Context.Guild);
                await player.UpdateVolumeAsync((ushort)volume);
                set_Volume(Context.Guild.Id, volume);
                //await LoggingService.LogInformationAsync(SCLD.music_name, $"音量設置為: {volume}" + $"\nby {Context.User.Mention}");
                IUserMessage the_message = await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed(SCLD.music_name + " - 音量", $"音量設置為 {volume}" + $"\nby {Context.User.Mention}", Color.Green));
                Thread.Sleep(10000);
                await the_message.DeleteAsync();
            }
            catch (InvalidOperationException ex)
            {
                await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed(SCLD.music_name + " - 音量", ex.Message + $"\nby {Context.User.Mention}"));
            }
        }
        #endregion

        #region 暫停+繼續
        public async Task ChangeVoiceChannelAsync(SocketCommandContext Context)
        {
            await LoggingService.LogInformationAsync("Music", $"收到指令({(Context.User as IGuildUser).Nickname})：{Context.Message}");

            await Context.Message.DeleteAsync();
            try
            {
                var player = _lavaNode.GetPlayer(Context.Guild);
                await player.PauseAsync();
                Thread.Sleep(1000);
                await player.ResumeAsync();
            }
            catch (InvalidOperationException ex)
            {
                await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed(SCLD.music_name + " - 暫停", ex.Message + $"\nby {Context.User.Mention}"));
            }
        }
        #endregion

        #region 暫停
        public async Task PauseAsync(SocketCommandContext Context)
        {
            await LoggingService.LogInformationAsync("Music", $"收到指令({(Context.User as IGuildUser).Nickname})：{Context.Message}");

            await Context.Message.DeleteAsync();
            try
            {
                var player = _lavaNode.GetPlayer(Context.Guild);
                if (!(player.PlayerState is PlayerState.Playing))
                {
                    await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed(SCLD.music_name + " - 暫停", $"沒有東西可以暫停" + $"\nby {Context.User.Mention}", Color.Red));
                }

                await player.PauseAsync();
                await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed(SCLD.music_name + " - 暫停", $"[{player.Track.Title}]({player.Track.Url})" + $"\nby {Context.User.Mention}", Color.Red));
            }
            catch (InvalidOperationException ex)
            {
                await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed(SCLD.music_name + " - 暫停", ex.Message + $"\nby {Context.User.Mention}"));
            }
        }
        #endregion

        #region 繼續
        public async Task ResumeAsync(SocketCommandContext Context)
        {
            await LoggingService.LogInformationAsync("Music", $"收到指令({(Context.User as IGuildUser).Nickname})：{Context.Message}");

            await Context.Message.DeleteAsync();
            try
            {
                var player = _lavaNode.GetPlayer(Context.Guild);

                if (player.PlayerState is PlayerState.Paused)
                {
                    await player.ResumeAsync();
                }
                await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed(SCLD.music_name + " - 繼續", $"[{player.Track.Title}]({player.Track.Url})" + $"\nby {Context.User.Mention}", Color.Green));
            }
            catch (InvalidOperationException ex)
            {
                await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed(SCLD.music_name + " - 繼續", ex.Message + $"\nby {Context.User.Mention}"));
            }
        }
        #endregion

        #region 隨機
        public async Task RandomAsync(SocketCommandContext Context)
        {
            await LoggingService.LogInformationAsync("Music", $"收到指令({(Context.User as IGuildUser).Nickname})：{Context.Message}");

            await Context.Message.DeleteAsync();
            try
            {
                var player = _lavaNode.GetPlayer(Context.Guild);

                Random rand = new Random(Guid.NewGuid().GetHashCode());

                List<LavaTrack> listTrack = new List<LavaTrack>();

                //LavaTrack nextTrack = null;
                List<LavaTrack> nextTrack = new List<LavaTrack>();

                if (player.Queue.Count == 0)
                {
                    await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed(SCLD.music_name + " - 隨機", $"清單中沒有歌曲" + $"\nby {Context.User.Mention}", Color.Red));
                    return;
                }

                foreach (LavaTrack track000 in player.Queue)
                {
                    listTrack.Add(track000);
                }

                listTrack = listTrack.OrderBy(num => rand.Next()).ToList<LavaTrack>();

                player.Queue.Clear();

                foreach (LavaTrack track000 in listTrack)
                {
                    nextTrack.Add(track000);
                    player.Queue.Enqueue(track000);
                }


                String trackInfo = $"已將待播歌曲隨機排列\n";
                /*
                int now_index = 1;
                int show_size = 7;
                foreach (LavaTrack tamp_track in nextTrack)
                {
                    if (now_index > show_size)
                    {
                        trackInfo += $"({(nextTrack.Count - show_size).ToString()} songs remaining...)\n";
                        break;
                    }
                    if (GetBytes_length(tamp_track.Title) > 45)
                        trackInfo += $"{now_index}. [{to_string_length(tamp_track.Title.Replace("[", "").Replace("]", ""), 45)}...]({tamp_track.Url})\n";
                    else
                        trackInfo += $"{now_index}. [{tamp_track.Title}]({tamp_track.Url})\n";
                    now_index++;
                }*/
                trackInfo += $"\nby {Context.User.Mention}";

                await UpdateNowPlay(Context.Guild);
                IUserMessage the_message = await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed(SCLD.music_name + " - 隨機", trackInfo, Color.Green));
                Thread.Sleep(5000);
                await the_message.DeleteAsync();
            }
            catch (InvalidOperationException ex)
            {
                await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed(SCLD.music_name + " - 隨機", ex.Message + $"\nby {Context.User.Mention}"));
            }
        }
        #endregion

        #region 找重複
        public async Task FindRepeatedAsync(SocketCommandContext Context)
        {
            await LoggingService.LogInformationAsync("Music", $"收到指令({(Context.User as IGuildUser).Nickname})：{Context.Message}");

            await Context.Message.DeleteAsync();

            try
            {
                var player = _lavaNode.GetPlayer(Context.Guild);

                int indexxx = 1;
                int all_count = 1;
                string to_show = "";
                foreach (LavaTrack track000 in player.Queue)
                {
                    if (player.Queue.Count(x => x.Id.Equals(track000.Id)) > 1 || player.Track.Id.Equals(track000.Id))
                    {
                        if (all_count > 1750)
                        {
                            to_show += "字數太多，無法顯示\n";
                            break;
                        }
                        all_count += track000.Title.Length;
                        to_show += indexxx + ". " + track000.Title + "\n";
                    }
                    indexxx++;
                }

                if (to_show == "")
                    to_show = "沒有\n";

                IUserMessage the_message = await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed(SCLD.music_name + " - 重複歌曲", $"{to_show}" + $"\nby {Context.User.Mention}", Color.Green));
                Thread.Sleep(20000);
                await the_message.DeleteAsync();
            }
            catch (InvalidOperationException ex)
            {
                await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed(SCLD.music_name + " - 移動", ex.Message + $"\nby {Context.User.Mention}"));
            }
        }
        #endregion

        #region 找歌
        public async Task FindListAsync(SocketCommandContext Context, string the_text)
        {
            await LoggingService.LogInformationAsync("Music", $"收到指令({(Context.User as IGuildUser).Nickname})：{Context.Message}");

            await Context.Message.DeleteAsync();

            try
            {
                var player = _lavaNode.GetPlayer(Context.Guild);

                int indexxx = 1;
                int all_count = 0;
                string to_show = "";
                foreach (LavaTrack track000 in player.Queue)
                {
                    if (track000.Title.IndexOf(the_text) >= 0)
                    {
                        if (all_count > 1750)
                        {
                            to_show += "字數太多，無法顯示\n";
                            break;
                        }
                        all_count += track000.Title.Length;
                        to_show += indexxx + ". " + track000.Title + "\n";
                    }
                    indexxx++;
                }

                if (to_show == "")
                    to_show = "找不到\n";

                IUserMessage the_message = await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed(SCLD.music_name + " - 找歌", $"搜尋「{the_text}」的結果：\n{to_show}" + $"\nby {Context.User.Mention}", Color.Green));
                Thread.Sleep(20000);
                await the_message.DeleteAsync();
            }
            catch (InvalidOperationException ex)
            {
                await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed(SCLD.music_name + " - 找歌", ex.Message + $"\nby {Context.User.Mention}"));
            }
        }
        #endregion

        #region 找時長
        public async Task FindLengthAsync(SocketCommandContext Context, string cTS)
        {
            await LoggingService.LogInformationAsync("Music", $"收到指令({(Context.User as IGuildUser).Nickname})：{Context.Message}");

            await Context.Message.DeleteAsync();

            TimeSpan TS;

            try
            {
                var player = _lavaNode.GetPlayer(Context.Guild);

                int hhh = 0, mmm = 0, sss = 0;

                string[] timestring = cTS.Split(":");

                int temp_A = 0;
                for (int i = timestring.Length - 1; i >= 0; i--)
                {
                    if (temp_A == 0)
                        sss = int.Parse(timestring[i]);
                    if (temp_A == 1)
                        mmm = int.Parse(timestring[i]);
                    if (temp_A == 2)
                        hhh = int.Parse(timestring[i]);
                    temp_A++;
                }

                TS = new TimeSpan(hhh, mmm, sss);



                int indexxx = 1;
                int all_count = 0;
                string to_show = "";
                foreach (LavaTrack track000 in player.Queue)
                {
                    if (track000.Duration >= TS)
                    {
                        if (all_count > 1750)
                        {
                            to_show += "字數太多，無法顯示\n";
                            break;
                        }
                        all_count += track000.Title.Length;
                        to_show += indexxx + ". " + track000.Title + "\n";
                    }
                    indexxx++;
                }

                if (to_show == "")
                    to_show = "找不到\n";

                IUserMessage the_message = await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed(SCLD.music_name + " - 找歌", $"搜尋時間大於「{time_swap(TS)}」的結果：\n{to_show}" + $"\nby {Context.User.Mention}", Color.Green));
                Thread.Sleep(20000);
                await the_message.DeleteAsync();
            }
            catch (InvalidOperationException ex)
            {
                await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed(SCLD.music_name + " - 找時間大於", ex.Message + $"\nby {Context.User.Mention}"));
            }
        }
        #endregion

        #region 交換
        public async Task SwitchAsync(SocketCommandContext Context, int item1, int item2)
        {
            await LoggingService.LogInformationAsync("Music", $"收到指令({(Context.User as IGuildUser).Nickname})：{Context.Message}");

            await Context.Message.DeleteAsync();
            if (item1 < item2)
            {
                item1 = item1 ^ item2;
                item2 = item1 ^ item2;
                item1 = item1 ^ item2;
            }

            try
            {
                var player = _lavaNode.GetPlayer(Context.Guild);

                if (item1 < 1 || item1 > player.Queue.Count || item2 < 1 || item2 > player.Queue.Count)
                {
                    await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed(SCLD.music_name + " - 移動", $"數字超出範圍 (1 ~ {player.Queue.Count})" + $"\nby {Context.User.Mention}", Color.Green));
                    return;
                }

                if (item1 == item2)
                {
                    IUserMessage the_messageerr = await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed(SCLD.music_name + " - 移動", $"待播清單移動完成" + $"\nby {Context.User.Mention}", Color.Green));
                    Thread.Sleep(5000);
                    await the_messageerr.DeleteAsync();
                    return;
                }

                List<LavaTrack> listTrack = new List<LavaTrack>();

                foreach (LavaTrack track000 in player.Queue)
                {
                    listTrack.Add(track000);
                }

                LavaTrack tmp = listTrack[item1 - 1];
                listTrack[item1 - 1] = listTrack[item2 - 1];
                listTrack[item2 - 1] = tmp;

                player.Queue.Clear();

                foreach (LavaTrack track000 in listTrack)
                {
                    player.Queue.Enqueue(track000);
                }

                await UpdateNowPlay(Context.Guild);
                IUserMessage the_message = await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed(SCLD.music_name + " - 移動", $"待播清單移動完成" + $"\nby {Context.User.Mention}", Color.Green));
                Thread.Sleep(5000);
                await the_message.DeleteAsync();
            }
            catch (InvalidOperationException ex)
            {
                await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed(SCLD.music_name + " - 移動", ex.Message + $"\nby {Context.User.Mention}"));
            }
        }

        public async Task SwitchAsync(SocketCommandContext Context, int item1)
        {
            await LoggingService.LogInformationAsync("Music", $"收到指令({(Context.User as IGuildUser).Nickname})：{Context.Message}");

            await Context.Message.DeleteAsync();

            try
            {
                var player = _lavaNode.GetPlayer(Context.Guild);

                if (item1 < 2 || item1 > player.Queue.Count)
                {
                    await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed(SCLD.music_name + " - 移動", $"數字超出範圍 (2 ~ {player.Queue.Count})" + $"\nby {Context.User.Mention}", Color.Green));
                    return;
                }

                List<LavaTrack> listTrack = new List<LavaTrack>();

                foreach (LavaTrack track000 in player.Queue)
                {
                    listTrack.Add(track000);
                }

                player.Queue.Clear();
                player.Queue.Enqueue(listTrack[item1 - 1]);
                listTrack.RemoveAt(item1 - 1);
                foreach (LavaTrack track000 in listTrack)
                {
                    player.Queue.Enqueue(track000);
                }

                await UpdateNowPlay(Context.Guild);
                IUserMessage the_message = await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed(SCLD.music_name + " - 移動", $"待播清單移動完成" + $"\nby {Context.User.Mention}", Color.Green));
                Thread.Sleep(5000);
                await the_message.DeleteAsync();
            }
            catch (InvalidOperationException ex)
            {
                await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed(SCLD.music_name + " - 移動", ex.Message + $"\nby {Context.User.Mention}"));
            }
        }
        #endregion

        #region 循環
        public async Task LoopAsync(SocketCommandContext Context)
        {
            await LoggingService.LogInformationAsync("Music", $"收到指令({(Context.User as IGuildUser).Nickname})：{Context.Message}");

            await Context.Message.DeleteAsync();
            if (!read_loop(Context.Guild.Id))  //如果沒開
            {
                set_loop(Context.Guild.Id, true);
                IUserMessage the_message = await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed("循環設定", $"開啟" + $"\nby {Context.User.Mention}", Color.Green));
                Thread.Sleep(5000);
                await the_message.DeleteAsync();
            }
            else
            {
                set_loop(Context.Guild.Id, false);
                IUserMessage the_message = await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed("循環設定", $"關閉" + $"\nby {Context.User.Mention}", Color.Red));
                Thread.Sleep(5000);
                await the_message.DeleteAsync();
            }
        }
        #endregion

        #region 歌詞

        private static readonly IEnumerable<int> Range = Enumerable.Range(1900, 2000);
        public async Task CallLyricsAsync(SocketCommandContext Context)
        {
            await LoggingService.LogInformationAsync("Music", $"收到指令({(Context.User as IGuildUser).Nickname})：{Context.Message}");

            await Context.Message.DeleteAsync();

            try
            {
                var player = _lavaNode.GetPlayer(Context.Guild);
                /*
                //ValueTask<string> aaaa = await player.Track.FetchLyricsFromGeniusAsync();
                string aaaa = await player.Track.FetchLyricsFromGeniusAsync();

                string to_show = "";

                to_show = aaaa;
                */


                var lyrics = await player.Track.FetchLyricsFromGeniusAsync();
                if (string.IsNullOrWhiteSpace(lyrics))
                {
                    await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed(SCLD.music_name + " - 歌詞", $"找不到歌詞\n\nby {Context.User.Mention}", Color.Green));
                    return;
                }

                var splitLyrics = lyrics.Split('\n');
                var stringBuilder = new StringBuilder();
                foreach (var line in splitLyrics)
                {
                    if (Range.Contains(stringBuilder.Length))
                    {
                        await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed(SCLD.music_name + " - 歌詞", $"{stringBuilder}\n\nby {Context.User.Mention}", Color.Green));
                        stringBuilder.Clear();
                    }
                    else
                    {
                        stringBuilder.AppendLine(line);
                    }
                }

                //await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed(SCLD.music_name + " - 歌詞", to_show + $"\n\nby {Context.User.Mention}", Color.Green));
            }
            catch (InvalidOperationException ex)
            {
                await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed(SCLD.music_name + " - 歌詞", ex.Message + $"\nby {Context.User.Mention}"));
            }
        }
        #endregion

        #region 播放清單存取

        #region 存檔
        public async Task SaveNowListAsync(SocketCommandContext Context, string filename)
        {
            await LoggingService.LogInformationAsync("Music", $"收到指令({(Context.User as IGuildUser).Nickname})：{Context.Message}");

            await Context.Message.DeleteAsync();

            string path = "data/music/" + Context.Guild.Id.ToString() + "/save/";
            string filepath = path + filename + ".json";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            try
            {
                var player = _lavaNode.GetPlayer(Context.Guild);

                var json = string.Empty;
                List<LavaTrack> LT_list = new List<LavaTrack>();

                if (player.Track != null)
                    LT_list.Add(player.Track);
                foreach (LavaTrack LT in player.Queue)
                    LT_list.Add(LT);


                json = JsonConvert.SerializeObject(LT_list, Formatting.Indented);
                File.WriteAllText(filepath, json, new UTF8Encoding(false));

                IUserMessage the_message = await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed(SCLD.music_name + " - 儲存", $"儲存成功\n下次輸入`{SCLD.DefaultPrefix}讀取`即可將歌曲放入播放清單中。\n\nby {Context.User.Mention}", Color.Green));
                Thread.Sleep(10000);
                await the_message.DeleteAsync();
            }
            catch (InvalidOperationException ex)
            {
                await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed(SCLD.music_name + " - 儲存", ex.Message + $"\nby {Context.User.Mention}"));
            }
        }
        #endregion

        #region 讀取
        public async Task LoadNowListAsync(SocketCommandContext Context, string filename)
        {
            await LoggingService.LogInformationAsync("Music", $"收到指令({(Context.User as IGuildUser).Nickname})：{Context.Message}");

            #region 自動進入設定
            if ((Context.User as IVoiceState).VoiceChannel is null)
            {
                await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed(SCLD.music_name + "", "你必須先加入一個語音頻道"));
                return;
            }

            //Check the guild has a player available.
            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                try
                {
                    await _lavaNode.JoinAsync((Context.User as IVoiceState).VoiceChannel, Context.Channel as ITextChannel);
                }
                catch (Exception ex)
                {
                    await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed(SCLD.music_name + " - 加入", ex.Message));
                }
                //return await EmbedHandler.CreateErrorEmbed("Music, Play", "我沒辦法加入一個語音頻道");
            }
            #endregion

            await Context.Message.DeleteAsync();

            string path = "data/music/" + Context.Guild.Id.ToString() + "/save/";
            string filepath = path + filename + ".json";

            if (!File.Exists(filepath))
            {
                IUserMessage the_messageerr = await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed(SCLD.music_name + " - 讀取", $"{Context.User.Mention} 尚未儲存過清單。", Color.Red));
                Thread.Sleep(10000);
                await the_messageerr.DeleteAsync();
                return;
            }

            try
            {
                var player = _lavaNode.GetPlayer(Context.Guild);

                #region 音量設定
                await player.UpdateVolumeAsync(read_Volume(Context.Guild.Id));
                #endregion

                var json = string.Empty;

                List<Trackforjson> LTJ_list = new List<Trackforjson>();
                json = File.ReadAllText(filepath, new UTF8Encoding(false));
                LTJ_list = JsonConvert.DeserializeObject<List<Trackforjson>>(json);


                foreach (Trackforjson TFJ in LTJ_list)
                {
                    player.Queue.Enqueue(toLavaTrack(TFJ));
                }

                //Thread.Sleep(2000);

                if (player.Track == null || player.PlayerState is PlayerState.Stopped)
                {
                    player.Queue.TryDequeue(out var queueable);
                    await player.PlayAsync(queueable);
                }
                else
                {
                    await UpdateNowPlay(Context.Guild);
                }

                IUserMessage the_message = await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed(SCLD.music_name + " - 讀取", $"已讀取 {filename} 。", Color.Green));

                Thread.Sleep(10000);
                await the_message.DeleteAsync();
            }
            catch (InvalidOperationException ex)
            {
                await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed(SCLD.music_name + " - 讀取", ex.Message + $"\nby {Context.User.Mention}"));
            }
        }
        #endregion

        #region 顯示
        public async Task ShowSaveAsync(SocketCommandContext Context, string listName)
        {
            await LoggingService.LogInformationAsync("Music", $"收到指令({(Context.User as IGuildUser).Nickname})：{Context.Message}");

            await Context.Message.DeleteAsync();

            if (listName == "")
            {
                List<string> pl = getplayerlist(Context.Guild.Id);

                string list = "";
                foreach (string s in pl)
                    list += s + "\n";

                IUserMessage the_message = await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed("存檔清單", list + $"\nby {Context.User.Mention}", Color.Green));
                Thread.Sleep(30000);
                await the_message.DeleteAsync();
            }
            else
            {
                string path = "data/music/" + Context.Guild.Id.ToString() + "/save/";
                string filepath = path + listName + ".json";

                IUserMessage the_message;

                if (!File.Exists(filepath))
                {
                    the_message = await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed($"「{listName}」 - 清單內容", $"找不到此清單\nby {Context.User.Mention}", Color.Green));
                    Thread.Sleep(10000);
                    await the_message.DeleteAsync();
                    return;
                }
                var json = string.Empty;

                List<Trackforjson> LTJ_list = new List<Trackforjson>();
                json = File.ReadAllText(filepath, new UTF8Encoding(false));
                LTJ_list = JsonConvert.DeserializeObject<List<Trackforjson>>(json);

                String content = "";
                int all_count = 0;
                int indexxx = 1;
                content += $"共 {LTJ_list.Count} 首：\n";
                foreach (Trackforjson TFJ in LTJ_list)
                {
                    if (all_count > 1750)
                    {
                        content += "字數太多，無法顯示\n";
                        break;
                    }
                    all_count += TFJ.Title.Length;
                    content += indexxx.ToString() + ". " + TFJ.Title + "\n";
                    indexxx++;
                }

                the_message = await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed($"「{listName}」 - 內容", content + $"\nby {Context.User.Mention}", Color.Green));

                Thread.Sleep(60000);
                await the_message.DeleteAsync();
            }

        }
        #endregion

        #region 刪除
        public async Task DeleteSaveAsync(SocketCommandContext Context, string listName)
        {
            await LoggingService.LogInformationAsync("Music", $"收到指令({(Context.User as IGuildUser).Nickname})：{Context.Message}");

            string path = "data/music/" + Context.Guild.Id.ToString() + "/save/";
            string filepath = path + listName + ".json";

            IUserMessage the_message;

            try
            {
                await Context.Message.DeleteAsync();
                if (File.Exists(filepath))
                {
                    System.IO.File.Delete(filepath);
                    the_message = await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed("刪除成功", $"存檔名稱：「{listName}」", Color.Red));
                }
                else
                {
                    the_message = await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed("刪除", $"找不到檔案「{listName}」", Color.Red));
                }
                Thread.Sleep(10000);
                await the_message.DeleteAsync();
            }
            catch (Exception ex)
            {
                await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed(SCLD.music_name + " - 刪除存檔", ex.Message));
            }
        }
        #endregion

        #endregion

        #region 更新正在播放訊息
        public async Task UpdateNowPlay(IGuild Guild)
        {
            await LoggingService.LogInformationAsync("Music", $"執行更新正在播放訊息");

            Thread.Sleep(1000);
            var player = _lavaNode.GetPlayer(Guild);

            Boolean is_loop = read_loop(Guild.Id);

            try
            {
                string ThumbnailUrl = $"https://i.ytimg.com/vi/{player.Track.Id}/hqdefault.jpg";

                ulong old_id = readnowplayfile(Guild.Id);

                if (old_id != 0)
                {
                    IUserMessage old_message = await player.TextChannel.GetMessageAsync(old_id) as IUserMessage;

                    Embed eb = await EmbedHandler.CreateBasicEmbed("正在播放：", GetNowPlay(player), Color.DarkPurple, ThumbnailUrl);

                    if (old_message != null)
                        await old_message.ModifyAsync(m => m.Embed = eb);
                }
            }
            catch (Exception ex) { }
        }
        #endregion

        #region 取得正在播放的字
        public String GetNowPlay(LavaPlayer player)
        {
            List<LavaTrack> nextTrack = new List<LavaTrack>();
            Boolean is_loop = read_loop(player.VoiceChannel.GuildId);

            foreach (LavaTrack LT in player.Queue)
                nextTrack.Add(LT);

            String trackInfo = "";
            TimeSpan allTime = new TimeSpan(0);

            trackInfo += $"[{player.Track.Title}]({player.Track.Url})\n";

            if (!player.Track.IsStream)
                allTime = allTime.Add(player.Track.Duration.Subtract(player.Track.Position));



            int now_index = 1;
            int show_size = 7;
            foreach (LavaTrack tamp_track in nextTrack)
            {
                if (now_index == 1)
                    trackInfo += $"Next:\n";

                if (now_index > show_size)
                {
                    /* //全顯示
                    if (GetBytes_length(tamp_track.Title) > 12)
                        trackInfo += $"{now_index}. {to_string_length(tamp_track.Title, 12)}...　";
                    else
                        trackInfo += $"{now_index}. {tamp_track.Title}　";
                    if (now_index == nextTrack.Count)
                        trackInfo += "\n";
                    now_index++;
                    continue;*/

                    if (nextTrack.Count - show_size == 1)
                        trackInfo += $"({(nextTrack.Count - show_size).ToString()} song remaining...)\n";
                    else
                        trackInfo += $"({(nextTrack.Count - show_size).ToString()} songs remaining...)\n";

                    break;
                }

                if (GetBytes_length(tamp_track.Title) > 41)
                    trackInfo += $"{now_index}. [{to_string_length(tamp_track.Title.Replace("[", "").Replace("]", ""), 41)}...]({tamp_track.Url})\n";
                else
                    trackInfo += $"{now_index}. [{tamp_track.Title}]({tamp_track.Url})\n";
                now_index++;
            }
            foreach (LavaTrack tamp_track in nextTrack)
            {
                if (!tamp_track.IsStream)
                    allTime = allTime.Add(tamp_track.Duration);
            }

            if (is_loop && nextTrack.Count == 0)
                trackInfo += $"Next: Loop Enable\n";
            else if (nextTrack.Count == 0)
                trackInfo += $"Next: None\n";

            if (player.Track.IsStream)
                trackInfo += $"\nLive, Loop: {(is_loop ? "on" : "off")}, Volume: {player.Volume.ToString()}, Total: {time_swap(allTime)}";
            else
                trackInfo += $"\n{time_swap(player.Track.Duration)}, Loop: {(is_loop ? "on" : "off")}, Volume: {player.Volume.ToString()}, Total: {time_swap(allTime)}";

            return trackInfo;

            //GuildEmote SCLDEMG = args.Player.VoiceChannel.Guild.Emotes.First(e => e.Name == "SCLD");

        }
        #endregion

        #region 歌曲開始
        public async Task TrackStarted(TrackStartEventArgs args)
        {
            //await LoggingService.LogInformationAsync("Music", $"歌曲開始事件");
            Thread.Sleep(1500);

            int errpoin = 0;
            try
            {
                string ThumbnailUrl = $"https://i.ytimg.com/vi/{args.Track.Id}/hqdefault.jpg";
                errpoin = 1;
                IUserMessage text_complete = await args.Player.TextChannel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed("正在播放：", GetNowPlay(args.Player), Color.DarkPurple, ThumbnailUrl));
                errpoin = 2;
                ulong old_id = readnowplayfile(args.Player.VoiceChannel.GuildId);
                errpoin = 3;
                if (old_id != 0)
                {
                    errpoin = 4;
                    IMessage old_message = await args.Player.TextChannel.GetMessageAsync(old_id);
                    errpoin = 5;
                    if (old_message != null)
                        await old_message.DeleteAsync();
                    errpoin = 6;
                }
                errpoin = 7;
                savenowplayfile(args.Player.VoiceChannel.GuildId, text_complete.Id);
                errpoin = 8;

                //await args.Player.TextChannel.SendMessageAsync("正在播放：\n" + trackInfo);
            }
            catch (Exception ex)
            {
                await LoggingService.LogInformationAsync(SCLD.music_name + " - 自動顯示歌曲", $"出錯了，位置：" + errpoin.ToString());
                //await args.Player.TextChannel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed(SCLD.music_name + " - 自動顯示", ex.Message));
            }
        }
        #endregion

        #region 歌曲結束
        public async Task TrackEnded(TrackEndedEventArgs args)
        {
            //await LoggingService.LogInformationAsync("Music", $"歌曲結束事件");

            Boolean is_loop = false;
            if (args.Player.VoiceChannel != null)
                is_loop = read_loop(args.Player.VoiceChannel.GuildId);
            else
                return;


            if (is_loop)
            {
                //args.Player.Queue.Enqueue(args.Track as LavaTrack);
                SearchResponse search = await _lavaNode.SearchAsync("https://www.youtube.com/watch?v=" + args.Track.Id);
                args.Player.Queue.Enqueue(search.Tracks[0]);
            }

            //Console.WriteLine("結束：" + args.Track.Title);

            if (!args.Reason.ShouldPlayNext())
            {
                return;
            }

            if (!args.Player.Queue.TryDequeue(out var queueable))
            {
                //await args.Player.TextChannel.SendMessageAsync("Playback Finished.");
                return;
            }

            if (!(queueable is LavaTrack track))
            {
                await args.Player.TextChannel.SendMessageAsync("播放清單中的下一個項目不是歌曲");
                return;
            }

            Thread.Sleep(500);

            await args.Player.PlayAsync(track);
            //await args.Player.TextChannel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed("正在播放：", $"[{track.Title}]({track.Url})\n{track.Duration}", Color.Blue));
        }
        #endregion

        #region player 更新
        public async Task PlayerUpdated(PlayerUpdateEventArgs args)
        {
            await LoggingService.LogInformationAsync("Music", $"player 更新事件");

            //_lavaNode.OnPlayerUpdated += _audioService.PlayerUpdated;  //這句放在 DiscordService.cs
            string nowtime = "";
            string alltime = "";
            nowtime = args.Player.Track.Position.ToString();
            alltime = args.Player.Track.Duration.ToString();
            await args.Player.TextChannel.SendMessageAsync(nowtime + "/" + alltime);
        }
        #endregion

        #region 點歌紀錄
        public void OrderRecord(SocketCommandContext Context, string Order)
        {
            LoggingService.LogInformationAsync("Music", $"點歌紀錄：{Context.Message}");

            string path = "data/music/" + Context.Guild.Id.ToString() + "/OrderRecord/";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            if (Order.IndexOf(Convert.ToChar(10).ToString()) > 0)
            {
                string writetext = "";
                foreach (string query in Order.Split(Convert.ToChar(10).ToString()))
                {
                    writetext += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + Convert.ToChar(9).ToString() + query + "\n";
                }
                System.IO.File.AppendAllText(path + Context.User.Id + ".txt", writetext);
            }
            else
            {
                System.IO.File.AppendAllText(path + Context.User.Id + ".txt", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + Convert.ToChar(9).ToString() + Order + "\n");
            }
        }
        #endregion

        #region 點歌紀錄查詢
        public async Task SeeOrderRecord(SocketCommandContext Context, SocketGuildUser userName)
        {
            await LoggingService.LogInformationAsync("Music", $"收到指令({(Context.User as IGuildUser).Nickname})：{Context.Message}");

            await Context.Message.DeleteAsync();

            string path = "data/music/" + Context.Guild.Id.ToString() + "/OrderRecord/";
            string filename = path + userName.Id + ".txt";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            if (!File.Exists(filename))
            {
                await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed("點播記錄", $"沒有找到播放記錄", Color.Green));
            }
            String[] textfile = System.IO.File.ReadAllLines(filename);
            List<MusicOrderRecord> records = new List<MusicOrderRecord>();
            foreach (string temp_record in textfile)
            {
                string[] temppp = temp_record.Split(Convert.ToChar(9).ToString());
                string oneordertext = "";
                for (int i = 1; i < temppp.Count(); i++)
                {
                    oneordertext += temppp[i];
                }


                records.Add(new MusicOrderRecord() { SearchText = oneordertext, Time = DateTime.Parse(temppp[0]) });

            }
            List<MusicOrderRecord> show_records = records.Where(x => x.Time > DateTime.Now.AddDays(-14)).ToList();

            string to_show = "";
            string to_save = "";

            foreach (MusicOrderRecord s in show_records)
            {
                //to_show += s.SearchText + "\n";
                to_save += s.Time.ToString("yyyy/MM/dd HH:mm:ss") + Convert.ToChar(9).ToString() + s.SearchText + "\n";
            }

            int all_count = 0;
            List<string> showlist = new List<string>();
            for (int i = records.Count() - 1; i >= 0; i--)
            {
                if (showlist.Count(xx => xx.Equals(records[i].SearchText)) == 0)
                {
                    if (all_count > 1750)
                        break;
                    all_count += records[i].SearchText.Length;
                    showlist.Add(records[i].SearchText);
                }
            }
            showlist.Reverse();
            foreach (string s in showlist)
            {
                to_show += s + "\n";
            }

            System.IO.File.WriteAllText(filename, to_save);
            await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed("點播記錄", userName.Mention + " 的點播記錄：\n\n" + to_show + "\n", Color.LightOrange));
        }
        #endregion

        #region 點歌紀錄刪除
        public async Task DeleteOrderRecord(SocketCommandContext Context, SocketGuildUser userName)
        {
            await LoggingService.LogInformationAsync("Music", $"收到指令({(Context.User as IGuildUser).Nickname})：{Context.Message}");

            await Context.Message.DeleteAsync();

            string path = "data/music/" + Context.Guild.Id.ToString() + "/OrderRecord/";
            string filename = path + userName.Id + ".txt";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            if (!File.Exists(filename))
            {
                await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed(userName.Mention + " - 點播記錄", $"沒有找到播放記錄", Color.Green));
            }
            System.IO.File.Delete(filename);
            await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed("點播記錄", $"已刪除", Color.Red));
        }
        #endregion

        #region 熱門存

        public void HotRecord(SocketCommandContext Context, List<LavaTrack> the_Track)
        {
            //await LoggingService.LogInformationAsync("Music", $"收到指令({(Context.User as IGuildUser).Nickname})：{Context.Message}");


            string path = "data/music/" + Context.Guild.Id.ToString() + "/";
            string filepath = path + "hot_music.json";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            List<Hotforjson> LTH_list = new List<Hotforjson>();

            var json = string.Empty;


            if (File.Exists(filepath))
            {
                json = File.ReadAllText(filepath, new UTF8Encoding(false));
                LTH_list = JsonConvert.DeserializeObject<List<Hotforjson>>(json);
            }

            LTH_list = LTH_list.Where(x => x.Date > DateTime.Now.AddDays(-30)).ToList();

            foreach (LavaTrack LT in the_Track)
                LTH_list.Add(new Hotforjson(LT, DateTime.Now, Context.User.Id));

            json = JsonConvert.SerializeObject(LTH_list, Formatting.Indented);
            File.WriteAllText(filepath, json, new UTF8Encoding(false));

        }

        #endregion

        #region 熱門顯示
        public async Task HotShowAsync(SocketCommandContext Context)
        {
            await LoggingService.LogInformationAsync("Music", $"收到指令({(Context.User as IGuildUser).Nickname})：{Context.Message}");

            await Context.Message.DeleteAsync();

            string path = "data/music/" + Context.Guild.Id.ToString() + "/";
            string filepath = path + "hot_music.json";

            string show_text = "";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            List<Hotforjson> LTH_list = new List<Hotforjson>();

            var json = string.Empty;


            if (File.Exists(filepath))
            {
                json = File.ReadAllText(filepath, new UTF8Encoding(false));
                LTH_list = JsonConvert.DeserializeObject<List<Hotforjson>>(json);
            }

            LTH_list = LTH_list.Where(x => x.Date > DateTime.Now.AddDays(-30)).ToList();

            json = JsonConvert.SerializeObject(LTH_list, Formatting.Indented);
            File.WriteAllText(filepath, json, new UTF8Encoding(false));

            IEnumerable<IGrouping<string, Hotforjson>> result = LTH_list.GroupBy(x => x.Id);
            result = result.OrderByDescending(x => x.Count());
            int indexxx = 1;
            show_text += "以下是近30天的歌曲排行：\n";
            foreach (IGrouping<string, Hotforjson> group in result)
            {
                if (show_text.Length > 1750)
                {
                    show_text += "字數太多，無法顯示\n";
                    break;
                }
                if (indexxx > 15) break;

                show_text += $"{indexxx++}. ({group.Count()}) [{group.FirstOrDefault().Title}]({group.FirstOrDefault().Url})\n";
                //show_text += $"{indexxx++}. ({group.Count()}) {group.FirstOrDefault().Title}\n";
            }

            IUserMessage the_message = await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed($"熱門歌曲", show_text + $"\nby {Context.User.Mention}", Color.Green));

            Thread.Sleep(60000);
            await the_message.DeleteAsync();

        }
        #endregion

        #region 熱門播放
        public async Task HotPlayAsync(SocketCommandContext Context, int numbe)
        {
            await LoggingService.LogInformationAsync("Music", $"收到指令({(Context.User as IGuildUser).Nickname})：{Context.Message}");

            await Context.Message.DeleteAsync();

            #region 自動進入設定
            if ((Context.User as IVoiceState).VoiceChannel is null)
            {
                await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed(SCLD.music_name + "", "你必須先加入一個語音頻道"));
                return;
            }

            //Check the guild has a player available.
            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                try
                {
                    await _lavaNode.JoinAsync((Context.User as IVoiceState).VoiceChannel, Context.Channel as ITextChannel);
                }
                catch (Exception ex)
                {
                    await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed(SCLD.music_name + " - 加入", ex.Message));
                }
                //return await EmbedHandler.CreateErrorEmbed("Music, Play", "我沒辦法加入一個語音頻道");
            }
            #endregion

            string path = "data/music/" + Context.Guild.Id.ToString() + "/";
            string filepath = path + "hot_music.json";

            string show_text = "";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            List<Hotforjson> LTH_list = new List<Hotforjson>();

            var json = string.Empty;


            if (File.Exists(filepath))
            {
                json = File.ReadAllText(filepath, new UTF8Encoding(false));
                LTH_list = JsonConvert.DeserializeObject<List<Hotforjson>>(json);
            }

            LTH_list = LTH_list.Where(x => x.Date > DateTime.Now.AddDays(-30)).ToList();

            json = JsonConvert.SerializeObject(LTH_list, Formatting.Indented);
            File.WriteAllText(filepath, json, new UTF8Encoding(false));

            IEnumerable<IGrouping<string, Hotforjson>> result = LTH_list.GroupBy(x => x.Id);
            result = result.OrderByDescending(x => x.Count());
            List<LavaTrack> LTs = new List<LavaTrack>();

            try
            {
                var player = _lavaNode.GetPlayer(Context.Guild);

                #region 音量設定
                await player.UpdateVolumeAsync(read_Volume(Context.Guild.Id));
                #endregion

                int indexxx = 0;
                foreach (IGrouping<string, Hotforjson> group in result)
                {
                    indexxx++;
                    player.Queue.Enqueue(toLavaTrack(group.FirstOrDefault()));
                    if (indexxx >= numbe) break;
                }

                //Thread.Sleep(2000);

                if (player.Track == null || player.PlayerState is PlayerState.Stopped)
                {
                    player.Queue.TryDequeue(out var queueable);
                    await player.PlayAsync(queueable);
                }
                else
                {
                    await UpdateNowPlay(Context.Guild);
                }

                show_text += $"已將 {indexxx} 首熱門歌曲加入播放。\n";

                IUserMessage the_message = await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateBasicEmbed($"熱門歌曲", show_text + $"\nby {Context.User.Mention}", Color.Green));

                Thread.Sleep(10000);
                await the_message.DeleteAsync();

            }
            catch (InvalidOperationException ex)
            {
                await Context.Channel.SendMessageAsync(embed: await EmbedHandler.CreateErrorEmbed(SCLD.music_name + " - 播放熱門", ex.Message + $"\nby {Context.User.Mention}"));
            }
        }
        #endregion

        #region 參數

        LavaTrack toLavaTrack(Hotforjson TFH)
        {
            return new LavaTrack(TFH.Hash, TFH.Id, TFH.Title, TFH.Author, TFH.Url, TFH.Position, (long)(TFH.Duration.TotalSeconds * 1000), TFH.CanSeek, TFH.IsStream);
        }
        LavaTrack toLavaTrack(Trackforjson TFJ)
        {
            return new LavaTrack(TFJ.Hash, TFJ.Id, TFJ.Title, TFJ.Author, TFJ.Url, TFJ.Position, (long)(TFJ.Duration.TotalSeconds * 1000), TFJ.CanSeek, TFJ.IsStream);
        }


        string time_swap(TimeSpan TS)
        {
            //int day = (int)Math.Floor(TS.TotalDays);
            int HH = (int)TS.TotalHours;
            int MM = TS.Minutes;
            int SS = TS.Seconds;

            string toshow = "";


            //if (day != 0)
            //    toshow += day.ToString() + " day ";

            if (HH != 0)
            {
                if (HH >= 10)
                    toshow += HH.ToString() + ":";
                else
                    toshow += HH.ToString("00") + ":";
            }

            if (MM != 0 || HH != 0)
                toshow += MM.ToString("00") + ":";

            toshow += SS.ToString("00");


            return toshow;
        }

        string to_string_length(string the_s, int len)
        {
            if (System.Text.Encoding.Default.GetByteCount(the_s) > len)
                return to_string_length(the_s.Substring(0, the_s.Length - 1), len);
            else
                return the_s;
        }
        int GetBytes_length(string the_s)
        {
            return System.Text.Encoding.Default.GetBytes(the_s).Length;
        }



        ulong readnowplayfile(ulong GUID)
        {
            string path = "data/music/" + GUID.ToString() + "/";

            if (!Directory.Exists(path))
                return 0;
            if (!File.Exists(path + "Now_play_id.txt"))
                return 0;

            return ulong.Parse(System.IO.File.ReadAllText(path + "Now_play_id.txt"));
        }
        void savenowplayfile(ulong GUID, ulong the_ID)
        {
            string path = "data/music/" + GUID.ToString() + "/";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            System.IO.File.WriteAllText(path + "Now_play_id.txt", the_ID.ToString());
        }


        /*
        String readfile(ulong GUID, string listName)
        {
            string path = "data/music/" + GUID.ToString() + "/save/";

            if (!Directory.Exists(path))
                return "";
            if (!File.Exists(path + listName + ".txt"))
                return "";

            //return System.IO.File.ReadAllLines(path + listName + ".txt").ToList();
            return System.IO.File.ReadAllText(path + listName + ".txt");
        }
        void savefile(ulong GUID, string listName, string content)
        {
            string path = "data/music/" + GUID.ToString() + "/save/";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            System.IO.File.WriteAllText(path + listName + ".txt", content);
        }
        */

        List<string> getplayerlist(ulong GUID)
        {
            string path = "data/music/" + GUID.ToString() + "/save/";

            if (!Directory.Exists(path))
                return new List<string>();

            List<string> files = new List<string>(Directory.EnumerateFiles(path));
            for (int i = 0; i < files.Count; i++)
            {
                files[i] = files[i].Split("/")[files[i].Split("/").Length - 1].Split(".")[0];
            }
            return files;
        }

        Boolean read_loop(ulong GUID)
        {
            string path = "data/music/" + GUID.ToString() + "/";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            try
            {
                String[] textfile = System.IO.File.ReadAllLines(path + "musicloop.txt");
                return textfile[0] == "1" ? true : false;
            }
            catch
            {
                System.IO.File.WriteAllText(path + "musicloop.txt", "0");
                return false;
            }
        }
        void set_loop(ulong GUID, Boolean is_loop)
        {
            string path = "data/music/" + GUID.ToString() + "/";
            System.IO.File.WriteAllText(path + "musicloop.txt", is_loop ? "1" : "0");
        }


        ushort read_Volume(ulong GUID)
        {
            string path = "data/music/" + GUID.ToString() + "/";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            try
            {
                String[] textfile = System.IO.File.ReadAllLines(path + "Volume.txt");
                return ushort.Parse(textfile[0]);
            }
            catch
            {
                System.IO.File.WriteAllText(path + "Volume.txt", "100");
                return 100;
            }
        }
        void set_Volume(ulong GUID, int vol)
        {
            string path = "data/music/" + GUID.ToString() + "/";
            System.IO.File.WriteAllText(path + "Volume.txt", vol.ToString());
        }
        #endregion

    }
}
