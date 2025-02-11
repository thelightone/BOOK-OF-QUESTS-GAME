﻿using System.Threading;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using Telegram.Bot.Types.Payments;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.Data.Sqlite;
using System.Data;


namespace app8
{
    internal class MainPath
    {
        private static InlineHandler _inlineHandler = new InlineHandler();
        private static ParseMode _parseMode = new ParseMode();
        private static SqlToExcel _sqlToExcel = new SqlToExcel();

        async public void MessageReceive(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            var message = update.Message;
            var bot = TelegramController.botClient;

            switch (update)
            {
                case { PreCheckoutQuery: { } preCheckoutQuery }:
                    if (preCheckoutQuery is { InvoicePayload: "unlock_X", Currency: "XTR", TotalAmount: 1 })
                        await bot.AnswerPreCheckoutQueryAsync(preCheckoutQuery.Id);
                    else
                        await bot.AnswerPreCheckoutQueryAsync(preCheckoutQuery.Id, "Ошибка оплаты, попробуйте еще раз.");
                    break;
                case { Message.SuccessfulPayment: { } successfulPayment }:
                    System.IO.File.AppendAllText("payments.log", $"\n{DateTime.Now}: " +
                       $"User {update.Message.From} paid for {successfulPayment.InvoicePayload}: " +
                       $"{successfulPayment.TelegramPaymentChargeId} {successfulPayment.ProviderPaymentChargeId}");
                    if (successfulPayment.InvoicePayload is "unlock_X")
                    {
                        await bot.SendTextMessageAsync(update.Message.Chat, "Спасибо за вашу поддержку!");
                        _inlineHandler.SuccesfulBuy();
                    }
                    break;
                default:
                    {
                        if (update.Type == UpdateType.CallbackQuery)
                        {
                            _inlineHandler.MessageHandler(botClient, update);
                        }

                        else if (message != null)
                        {
                            try
                            {
                                string messageText = message.Text;
                                Console.WriteLine(message.Text);
                                if (messageText.Contains("/start"))
                                {
                                    SavePlayer(bot, message);
                                }
                                else if(messageText.Contains("utm"))
                                {
                                    _inlineHandler.Statictic(botClient,message);    
                                }
                                else if (messageText.Contains("users"))
                                {
                                    _sqlToExcel.SaveExcel(bot, update);
                                }

                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex);
                            }

                        }
                        break;
                    }
            };



        }

        async private static void SavePlayer(TelegramBotClient botClient, Message message)
        {
            await SendInitialMessage(botClient, message);
            Console.WriteLine("SavePlayer");
            IDbConnection dbcon05 = new SqliteConnection("Data Source = Savings.db");

            dbcon05.Open();
            IDbCommand firstsave = dbcon05.CreateCommand();
            firstsave.CommandText = "SELECT count(*) FROM Savings WHERE ChatId='" + Convert.ToString(message.Chat.Id) + "'";
            int count = Convert.ToInt32(firstsave.ExecuteScalar());
            firstsave.Dispose();
            dbcon05.Close();

            if (count == 0)
            {
                Console.WriteLine("CreatePlayer");
                IDbConnection dbcon09 = new SqliteConnection("Data Source = Savings.db");

                dbcon09.Open();
                IDbCommand firstsave2 = dbcon09.CreateCommand();
                firstsave2.CommandText = "INSERT INTO Savings (ChatId, Paid, refblockon)" +
                "VALUES (@ChatId,0,0)";
                firstsave2.Parameters.Add(new SqliteParameter("@ChatId", Convert.ToString(message.Chat.Id)));
                firstsave2.ExecuteNonQuery();
                firstsave2.Dispose();
                dbcon09.Close();

                _inlineHandler.ReferalCheck(botClient, message);
                Console.WriteLine("PlayerSaved");
            }

        }

        private static async Task SendInitialMessage(TelegramBotClient botClient, Message message)
        {
            // Создание клавиатуры с кнопками
            InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
        new InlineKeyboardButton[]
        {
            InlineKeyboardButton.WithWebApp("🚀 Начать игру", new WebAppInfo { Url = "https://thelightone.github.io/GameBundle4/" })
        },
        new InlineKeyboardButton[]
        {
            InlineKeyboardButton.WithCallbackData("🔕 Отключить рекламу", "Месяц")
        },
        new InlineKeyboardButton[]
        {
            InlineKeyboardButton.WithCallbackData("👬 Пригласить друга", "Пригласить")
        }
    });

            // Отправка сообщения с inline клавиатурой
            var sentMessage = await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "🎮 Погрузись в увлекательные приключения вместе с <b>TOP GAMEZ.</b>",
                parseMode: ParseMode.Html, // Разметка HTML
                replyMarkup: inlineKeyboard // Клавиатура
                                            // messageThreadId: message.MessageThreadId // Можно удалить, если не используете многопоточность
            );
        }
    }
}

//// Путь к файлу базы данных SQLite
//string sqliteDbPath = "path_to_your_database.db";

//// Запрос для извлечения данных из базы данных SQLite
//string query = "SELECT * FROM your_table";

//// Получаем данные из SQLite
//SQLiteHelper sqliteHelper = new SQLiteHelper(sqliteDbPath);
//List<Dictionary<string, object>> sqliteData = sqliteHelper.GetData(query);

//// Преобразуем данные в формат, подходящий для Google Sheets
//List<IList<object>> sheetData = new List<IList<object>>();

//// Заголовки (если нужно, можно добавить заголовки из словаря)
//List<object> header = new List<object>();
//foreach (var column in sqliteData[0].Keys)
//{
//    header.Add(column);
//}
//sheetData.Add(header);

//// Данные
//foreach (var row in sqliteData)
//{
//    List<object> rowData = new List<object>();
//    foreach (var column in row.Values)
//    {
//        rowData.Add(column);
//    }
//    sheetData.Add(rowData);
//}

//// ID вашей таблицы Google Sheets
//string spreadsheetId = "your_spreadsheet_id";

//// Диапазон (например, "Sheet1!A1" - начать с ячейки A1)
//string range = "Sheet1!A1";

//// Обновляем Google Sheets
//GoogleSheetsHelper googleSheetsHelper = new GoogleSheetsHelper("path_to_your_credentials.json");
//googleSheetsHelper.UpdateSheet(spreadsheetId, range, sheetData);

//Console.WriteLine("Данные успешно скопированы в Google Sheets.");
//    }

