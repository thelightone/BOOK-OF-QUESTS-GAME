﻿using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace app8
{
    class TelegramController
    {
        private static MainPath _mainPath = new MainPath();
        public static TelegramBotClient botClient;
        static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(AppContext.BaseDirectory);
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            botClient = new TelegramBotClient("5469591033:AAFNTWouLXFX4Ywd72k2YNatwCEJVmnCvDo");
            botClient.StartReceiving(Update, Error);
            Console.ReadLine();

            async static Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
            {
                _mainPath.MessageReceive(botClient, update, token);
            }

            static Task Error(ITelegramBotClient botClient, Exception exception, CancellationToken token)
            {

                Console.WriteLine($"\n\n================= {DateTime.Now} ================= \n\n{exception}");
                Console.WriteLine("Перезапускаю сервер...");

                System.Environment.Exit(0);
                return null;

            }

        }
    }
}












