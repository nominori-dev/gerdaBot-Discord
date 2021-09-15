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
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace NomiBotDS
{


    // Create a module with no prefix
    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        // ~say hello world -> hello world
        [Command("say")]
        [Summary("Echoes a message.")]
        public Task SayAsync([Remainder][Summary("The text to echo")] string echo)
            => ReplyAsync(echo);
        // ReplyAsync is a method on ModuleBase 
    }

    public class TestCommands : ModuleBase<SocketCommandContext>
    {

    }
}
