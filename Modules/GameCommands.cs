using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MineStatLib;
using Newtonsoft.Json;
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
    public class GameCommands : ModuleBase<SocketCommandContext>
    {

        [Command("info")]

        public async Task info([Remainder] string text)
        {
            var user = Context.User as SocketGuildUser;
            Console.WriteLine($@"{DateTime.Now,-19} NomiBot: " + user + " issued command 'info'");
            int num = RandomNumber(0, 100);

            await ReplyAsync(user + $", я думаю, что вероятность {num}%");
        }

        private readonly Random _random = new Random();

        // Generates a random number within a range.      
        public int RandomNumber(int min, int max)
        {
            return _random.Next(min, max);
        }


        [Command("who")]

        public async Task who([Remainder] string text)
        {
            var users = Context.User as SocketGuildUser;
            Console.WriteLine($@"{DateTime.Now,-19} NomiBot: " + users + " issued command 'who'");
            var user = Context.Guild.Users.ToList();

        label:
            try
            {
                Random random = new Random();
                int index = random.Next(user.Count);
                await ReplyAsync(user[index] + ", " + text);

            }
            catch (Exception ex)
            {
                goto label;
            }

        }

        [Command("choose")]

        public async Task choose(string firstArg, string or, [Remainder] string secondArg)
        {
            var user = Context.User as SocketGuildUser;
            Console.WriteLine($@"{DateTime.Now,-19} NomiBot: " + user + " issued command 'choose'");
            List<string> argOutput = new List<string>();
            argOutput.Add(firstArg);
            argOutput.Add(secondArg);
            argOutput.Add("я откажусь от выбора");
            int i = RandomNumber(0, 2);
            await ReplyAsync("Я выбираю: " + argOutput[i]);
        }
    }
}
