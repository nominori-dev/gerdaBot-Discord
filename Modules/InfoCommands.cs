using Discord.Commands;
using Discord.WebSocket;
using MineStatLib;
using Newtonsoft.Json;
using NomiBotDS.Conn;
using NomiBotDS.Models;
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
    public class InfoCommands : ModuleBase<SocketCommandContext>
    {
        Data data = new Data();
        tokenGeneration tokens = new tokenGeneration();

        [Command("ping")]
        public async Task Ping()
        {
            var user = Context.User as SocketGuildUser;
            await ReplyAsync("Pong!");
            Console.WriteLine($@"{DateTime.Now,-19} NomiBot: Pinged by " + user);

        }


        [Command("whois")]
        public async Task Whois([Remainder] string nickname)
        {
            var user = Context.User as SocketGuildUser;
            var Message = await Context.Channel.SendMessageAsync("``` \n Пожалуйста подождите....\n" + "```");
            string token = tokens.readToken();
            string username, age, nation, skin_color, organization, work;
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.GetAsync($"https://newsenseapi.herokuapp.com/api/users/samp/{nickname}");
                string responseBody = await response.Content.ReadAsStringAsync();
                dynamic stuff = JsonConvert.DeserializeObject(responseBody);
                username = stuff.username;
                age = stuff.age;
                nation = stuff.nation;
                skin_color = stuff.skin_color;
                organization = stuff.organization;
                work = stuff.work;
                if(username == nickname)
                {
                    await Message.ModifyAsync(msg => msg.Content = $"💡 Информация о игроке {username} \n\n Возраст: {age}\n Национальность: {nation}\n Цвет кожи: {skin_color}\n Фракция: {organization}\n\n🔭 Информация получена с NS API");
                }
                else
                {
                    await Message.ModifyAsync(msg => msg.Content = "```diff\n" + "- Не найдено игрока с указанным ником\n" + "```");
                }

            }
            catch (Exception e)
            {
                await Message.ModifyAsync(msg => msg.Content = "```diff\n" + "- Не найдено игрока с указанным ником\n" + "```");
                Console.WriteLine(e.ToString());
                Console.WriteLine(token);
            }

            Console.WriteLine($@"{DateTime.Now,-19} NomiBot: " + user + " issued command 'help'");
        }


        [Command("wwmap")]

        public async Task Command()
        {
            var user = Context.User as SocketGuildUser;
            await ReplyAsync("зачем...");
            Console.WriteLine($@"{DateTime.Now,-19} NomiBot: Wwmap'ed by " + user);

        }

        [Command("about")]

        public async Task About()
        {

            await ReplyAsync(data.About);
            var user = Context.User as SocketGuildUser;
            Console.WriteLine($@"{DateTime.Now,-19} NomiBot: " + user + " issued command 'about'");
        }

        [Command("version")]

        public async Task Version()
        {

            await ReplyAsync(data.Version);
            var user = Context.User as SocketGuildUser;
            Console.WriteLine($@"{DateTime.Now,-19} NomiBot: " + user + " issued command 'version'");
        }

        [Command("link")]
        public async Task Link()
        {
            var invites = await Context.Guild.GetInvitesAsync();
            await ReplyAsync(invites.Select(x => x.Url).FirstOrDefault());
        }


        [Command("help")]

        public async Task Help()
        {
            var user = Context.User as SocketGuildUser;
            var Message = await Context.Channel.SendMessageAsync("``` \n Пожалуйста подождите....\n" + "```");
            string token = tokens.readToken();
            string channelMessage = String.Empty;
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.GetAsync($"https://newsenseapi.herokuapp.com/api/news/4");
                string responseBody = await response.Content.ReadAsStringAsync();
                dynamic stuff = JsonConvert.DeserializeObject(responseBody);
                string chMessage = stuff.info;
                string result = chMessage.Replace(@"\n", "\n");
                await Message.ModifyAsync(msg => msg.Content = chMessage);

            }
            catch (Exception e)
            {
                await Message.ModifyAsync(msg => msg.Content = "```diff\n" + "- Не найдено новостей с указанным айди\n" + "```");
                await ReplyAsync(e.Message);
            }

            Console.WriteLine($@"{DateTime.Now,-19} NomiBot: " + user + " issued command 'help'");
        }

        [Command("minecraft")]

        public async Task Server()
        {
            var user = Context.User as SocketGuildUser;
            Console.WriteLine($@"{DateTime.Now,-19} NomiBot: " + user + " issued command 'server'");
            var Message = await Context.Channel.SendMessageAsync("```diff\n" + "- Проверяю статус сервера!\nПожалуйста подождите....\n" + "```");
            Console.WriteLine($@"{DateTime.Now,-19} NomiBot: checking minecraft server status...");
            MineStat ms = new MineStat("play.wwmap.ga", 25565);
            if (ms.ServerUp)
            {
                await Message.ModifyAsync(msg => msg.Content = "```diff\n" + "- Сервер включен!\n" + "```");
                Console.WriteLine($@"{DateTime.Now,-19} NomiBot: server working!");
            }
            else
            {
                await Message.ModifyAsync(msg => msg.Content = "```diff\n" + "- Сервер выключен :(\n" + "```");
                Console.WriteLine($@"{DateTime.Now,-19} NomiBot: server offline :(");
            }
        }

    }
}
