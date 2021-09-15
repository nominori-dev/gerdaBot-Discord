using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using NomiBotDS.Conn;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NomiBotDS.Modules
{
    public class NewsCommands : ModuleBase<SocketCommandContext>
    {
        tokenGeneration tokens = new tokenGeneration();

        [Command("announce")]
        public async Task Announce(string message, ulong id)
        {
            var client = Context.Client;
            ulong channelID = id;

            var channel = client.GetChannel(channelID) as SocketTextChannel;
            await channel.SendMessageAsync(message);
        }

        [Command("news")]
        public async Task News(ulong id, long dbid)
        {
            var Message = await Context.Channel.SendMessageAsync("```diff\n" + "- Проверяю введенные данные!\nПожалуйста подождите....\n" + "```");
            var user = Context.User as SocketGuildUser;
            var role = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Персонал");
            if (user.Roles.Contains(role))
            {
                string token = tokens.readToken();
                try
                {
                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    HttpResponseMessage response = await client.GetAsync($"https://newsenseapi.herokuapp.com/api/news/{dbid}");
                    string responseBody = await response.Content.ReadAsStringAsync();
                    dynamic stuff = JsonConvert.DeserializeObject(responseBody);
                    string chMessage = stuff.info;
                    var dsclient = Context.Client;
                    ulong channelID = id;

                    var channel = dsclient.GetChannel(channelID) as SocketTextChannel;
                    await Message.ModifyAsync(msg => msg.Content = "```diff\n" + "- Отправляю сообщение в указанный канал!\n" + "```");
                    if (channel != null)
                    {
                        await channel.SendMessageAsync(chMessage);
                    }
                    else
                    {
                        await Message.ModifyAsync(msg => msg.Content = "```diff\n" + "- Не найдено канала с указанным айди\n" + "```");
                    }

                }
                catch (Exception e)
                {
                    await Message.ModifyAsync(msg => msg.Content = "```diff\n" + "- Не найдено новостей с указанным айди\n" + "```");
                    await ReplyAsync(e.Message);
                }
            }
            else
            {
                await Message.ModifyAsync(msg => msg.Content = "```diff\n" + "- У вас нет прав!\n" + "```");
            }

        }
    }
}
