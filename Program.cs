using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Net;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.IO;
using System.Net;
using NomiBotDS.Conn;
using System.Configuration;

namespace NomiBotDS
{
    class Program
    {



        private static readonly Version
            ProgramVersion =
                Assembly.GetExecutingAssembly().GetName()
                    .Version;

        static void Main(string[] args)
        {
            tokenGeneration tokens = new tokenGeneration();
            var timer = new Timer(e => tokens.Generate(), null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
            new Program().MainAsync().GetAwaiter().GetResult();

        }

        private readonly DiscordSocketClient _client;

        private readonly CommandService _commands;
        private readonly IServiceProvider _services;

        public Program()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                AlwaysDownloadUsers = true

            });

            _commands = new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Info,

                CaseSensitiveCommands = false,
            });
            _client.Log += Log;
            _commands.Log += Log;

            _services = ConfigureServices();

        }

        private static IServiceProvider ConfigureServices()
        {
            var map = new ServiceCollection()
                .AddSingleton(new InfoModule());

            return map.BuildServiceProvider();
        }

        private static Task Log(LogMessage message)
        {
            switch (message.Severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
            }
            Console.WriteLine($"{DateTime.Now,-19} [{message.Severity,8}] {message.Source}: {message.Message} {message.Exception}");
            Console.ResetColor();

            return Task.CompletedTask;
        }

        private async Task MainAsync()
        {
            // Centralize the logic for commands into a separate method.
            await InitCommands();
            var token = ConfigurationManager.AppSettings.Get("token");
            // Login and connect.
            await _client.LoginAsync(TokenType.Bot,
                // < DO NOT HARDCODE YOUR TOKEN >
                token);
            await _client.StartAsync();

            // Wait infinitely so your bot actually stays connected.
            await Task.Delay(Timeout.Infinite);

        }


        private async Task BotInfo()
        {
            IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());
            Console.Title = $@"DiscordBot v{ProgramVersion} | Developed by nominori-dev";
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($@" ");
            Console.WriteLine($@" ");
            Console.WriteLine($@" ");
            Console.WriteLine($@" ");
            Console.WriteLine($@" ");
            Console.WriteLine($@"                       ._____.           __   ", Console.ForegroundColor);
            Console.WriteLine($@"   ____   ____   _____ |__\_ |__   _____/  |_ ", Console.ForegroundColor);
            Console.WriteLine($@"  /    \ /  _ \ /     \|  || __ \ /  _ \   __\", Console.ForegroundColor);
            Console.WriteLine($@" |   |  (  <_> )  Y Y  \  || \_\ (  <_> )  |  ", Console.ForegroundColor);
            Console.WriteLine($@" |___|  /\____/|__|_|  /__||___  /\____/|__|  ", Console.ForegroundColor);
            Console.WriteLine($@"      \/             \/        \/             ", Console.ForegroundColor);
            Console.WriteLine($@" ");
            Console.WriteLine($@" ");
            Console.WriteLine($@"================================================");
            Console.WriteLine($@"Current ip: {localIPs.ToString()}");
            Console.WriteLine($@"Database: testdb");
            Console.WriteLine($@"Server ip: play.wwmap.ga");
            Console.WriteLine($@"Server type: Discord Gateway ");
            Console.WriteLine($@"NomiBot: Developed by nominori-dev");
            Console.WriteLine($@"NomiBot: Web: www.wwmap.ga");
            Console.WriteLine($@"================================================");
            Console.WriteLine($@" ");
            Console.WriteLine($@" ");
            _client.SetGameAsync(";help");

        }

        private async Task InitCommands()
        {
            // Either search the program and add all Module classes that can be found.
            // Module classes MUST be marked 'public' or they will be ignored.
            // You also need to pass your 'IServiceProvider' instance now,
            // so make sure that's done before you get here.
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
            // Or add Modules manually if you prefer to be a little more explicit:
            // Note that the first one is 'Modules' (plural) and the second is 'Module' (singular).

            await BotInfo();
            // Subscribe a handler to see if a message invokes a command.
            _client.MessageReceived += HandleCommandAsync;
        }

        static public void ConOutput()
        {
            FileStream ostrm;
            StreamWriter writer;
            TextWriter oldOut = Console.Out;
            try
            {
                ostrm = new FileStream("./Redirect.txt", FileMode.OpenOrCreate, FileAccess.Write);
                writer = new StreamWriter(ostrm);
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot open Redirect.txt for writing");
                Console.WriteLine(e.Message);
                return;
            }
            Console.SetOut(writer);
            Console.WriteLine("This is a line of text");
            Console.WriteLine("Everything written to Console.Write() or");
            Console.WriteLine("Console.WriteLine() will be written to a file");
            Console.SetOut(oldOut);
            writer.Close();
            ostrm.Close();
            Console.WriteLine("Done");
        }

        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            ConOutput();
        }
        private async Task HandleCommandAsync(SocketMessage arg)
        {
            // Проверка на системное сообщение
            var msg = arg as SocketUserMessage;
            if (msg == null) return;

            // Проверка чтобы бот не отвечал себе или другим ботам
            if (msg.Author.Id == _client.CurrentUser.Id || msg.Author.IsBot) return;

            // Create a number to track where the prefix ends and the command begins
            int pos = 0;
            // Replace the '!' with whatever character
            // you want to prefix your commands with.
            // Uncomment the second half if you also want
            // commands to be invoked by mentioning the bot instead.
            if (msg.HasCharPrefix(';', ref pos) /* || msg.HasMentionPrefix(_client.CurrentUser, ref pos) */)
            {
                // Create a Command Context.
                var context = new SocketCommandContext(_client, msg);

                // Execute the command. (result does not indicate a return value, 
                // rather an object stating if the command executed successfully).
                var result = await _commands.ExecuteAsync(context, pos, _services);

                // Uncomment the following lines if you want the bot
                // to send a message if it failed.
                // This does not catch errors from commands with 'RunMode.Async',
                // subscribe a handler for '_commands.CommandExecuted' to see those.
                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                    await msg.Channel.SendMessageAsync(result.ErrorReason);
            }
        }
    }
}
