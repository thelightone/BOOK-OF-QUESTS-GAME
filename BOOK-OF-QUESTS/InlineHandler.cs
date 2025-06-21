using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Microsoft.Data.Sqlite;
using System.Data;
using Telegram.Bot.Types.Enums;
using System.Collections.Generic;
using Telegram.Bot.Types.Payments;
using System.Runtime.CompilerServices;
using System.ComponentModel.Design;
using static System.Net.WebRequestMethods;


namespace app8
{
    internal class InlineHandler
    {
        private static ParseMode _parseMode = new ParseMode();
        private CallbackQuery _callbackQuery;

        async public void MessageHandler(ITelegramBotClient botClient, Update update)
        {
            try
            {
                _callbackQuery = update.CallbackQuery;
                if (_callbackQuery == null)
                {
                    Console.WriteLine("CallbackQuery is null");
                    return;
                }

                Console.WriteLine($"Received callback: {_callbackQuery.Data}");

                switch (_callbackQuery.Data)
                {
                    case "Генерировать":
                        var chatMember = await botClient.GetChatMemberAsync(
                            chatId: -1001531639213,
                            userId: _callbackQuery.From.Id
                        );

                        if (chatMember.Status == ChatMemberStatus.Left || chatMember.Status == ChatMemberStatus.Kicked)
                        {
                            await botClient.SendTextMessageAsync(
                                chatId: _callbackQuery.Message.Chat.Id,
                                text: "❤️ Для генерации изображения, пожалуйста, подпишитесь на наш канал.",
                                replyMarkup: new InlineKeyboardMarkup(new[]
                                {
                                    new InlineKeyboardButton[]
                                    {
                                        InlineKeyboardButton.WithUrl("📢 Подписаться", "https://t.me/book_of_quests"),
                                        InlineKeyboardButton.WithCallbackData("✅ Я подписался", "Генерировать")
                                    }
                                })
                            );
                        }
                        else
                        {
                            // Устанавливаем состояние ожидания промпта
                            if (!MainPath._waitingForPrompt.ContainsKey(_callbackQuery.From.Id))
                            {
                                MainPath._waitingForPrompt.Add(_callbackQuery.From.Id, true);
                            }
                            else
                            {
                                MainPath._waitingForPrompt[_callbackQuery.From.Id] = true;
                            }

                            await botClient.SendTextMessageAsync(
                                chatId: _callbackQuery.Message.Chat.Id,
                                text: "Введите описание изображения. Рекомендуется описывать изображение как можно подробнее:",
                                replyMarkup: new InlineKeyboardMarkup(new[]
                                {
                                    new InlineKeyboardButton[]
                                    {
                                        InlineKeyboardButton.WithCallbackData("Назад", "Главное меню")
                                    }
                                })
                            );
                        }
                        break;

                    case "Пригласить":
                        await botClient.SendTextMessageAsync(
                            chatId: _callbackQuery.Message.Chat.Id,
                            text: "Твоя персональная ссылка для приглашения, перешлите ее друзьям: " + "\n"
                            + "t.me/top_gamez_bot?start=" + _callbackQuery.Message.Chat.Id + "");
                        break;

                    case "Месяц":
                        await botClient.SendInvoiceAsync(
                            chatId: _callbackQuery.Message.Chat.Id,
                            title: "Получить премиум",
                            description: "Поддержите проект и получите безлимитные генерации и отсутствие рекламы на месяц!",
                            payload: "unlock_X",
                            providerToken: "",
                            currency: "XTR",
                            prices: new List<LabeledPrice>() { new LabeledPrice("Price", 99) },
                            photoUrl: "https://github.com/thelightone/BOOK-OF-QUESTS-GAME/blob/main/photo_2025-06-18_18-39-15.jpg?raw=true"
                        );
                        break;
                    case "Главное меню":
                        InlineKeyboardMarkup mainMenuKeyboard = new InlineKeyboardMarkup(new[]
                        {
                            new InlineKeyboardButton[]
                            {
                                InlineKeyboardButton.WithCallbackData("🖼 Генерировать изображение", "Генерировать")
                            },
                            new InlineKeyboardButton[]
                            {
                                InlineKeyboardButton.WithCallbackData("👑 Подписка", "Месяц")
                            }
                        });

                        await botClient.SendPhotoAsync(
                            chatId: _callbackQuery.Message.Chat.Id,
                            photo: "https://github.com/thelightone/BOOK-OF-QUESTS-GAME/blob/main/photo_2025-06-18_18-45-17.jpg?raw=true",
                            caption: "👨‍🎨 <b>MidJoBot</b> - самый быстрый бот для генерации изображений." + "\n" +
                                "Генерируйте изображения за секунды!" + "\n" + "\n" +
                                "Используемая сеть - <b>Stable Diffusion.</b>",
                            parseMode: ParseMode.Html,
                            replyMarkup: mainMenuKeyboard
                        );
                        break;

                    default:
                        await botClient.AnswerCallbackQueryAsync(
                            callbackQueryId: _callbackQuery.Id,
                            text: "Неизвестная команда"
                        );
                        break;
                }

                // Отвечаем на callback query, чтобы убрать часики
                await botClient.AnswerCallbackQueryAsync(_callbackQuery.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in MessageHandler: {ex}");
                if (_callbackQuery != null)
                {
                    try
                    {
                        await botClient.AnswerCallbackQueryAsync(
                            callbackQueryId: _callbackQuery.Id,
                            text: "Произошла ошибка при обработке запроса"
                        );
                    }
                    catch { }
                }
            }
        }

        public void SuccesfulBuy()
        {
            IDbConnection dbcon17 = new SqliteConnection("Data Source=Savings.db");
            dbcon17.Open();
            IDbCommand savetechnic1 = dbcon17.CreateCommand();
            savetechnic1.CommandText = "UPDATE Savings SET Paid = 1, payDate = datetime('now') WHERE ChatId = '" + _callbackQuery.Message.Chat.Id.ToString() + "'";
            savetechnic1.ExecuteNonQuery();
            savetechnic1.Dispose();
            dbcon17.Close();
        }

        async public void Statictic(ITelegramBotClient botClient, Message message)
        {
            var utmnum = message.Text.Substring(4);

            IDbConnection statsutm = new SqliteConnection("Data Source=Savings.db");
            statsutm.Open();

            IDbCommand statsutm2 = statsutm.CreateCommand();
            statsutm2.CommandText = "SELECT count(*) FROM Savings WHERE source ='" + utmnum + "' AND joindate = '" + DateTime.Today + "' ";
            int countutmtoday = Convert.ToInt32(statsutm2.ExecuteScalar());
            statsutm2.Dispose();

            IDbCommand statsutm3 = statsutm.CreateCommand();
            statsutm3.CommandText = "SELECT count(*) FROM Savings WHERE source ='" + utmnum + "' AND joindate = '" + DateTime.Today.AddDays(-1) + "' ";
            int countutmyest = Convert.ToInt32(statsutm3.ExecuteScalar());
            statsutm3.Dispose();

            IDbCommand statsutm4 = statsutm.CreateCommand();
            statsutm4.CommandText = "SELECT count(*) FROM Savings WHERE source ='" + utmnum + "' ";
            int countutmall = Convert.ToInt32(statsutm4.ExecuteScalar());
            statsutm4.Dispose();
            statsutm.Close();

            Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    "Всего пользователей пришло:'" + countutmall + "'" + "\n" +
                    "Вчера:'" + countutmyest + "'" + "\n" +
                    "Сегодня:'" + countutmtoday + "'");
        }

        public void ReferalCheck(ITelegramBotClient botClient, Message message)
        {
            Console.WriteLine("CheckReferal");
            IDbConnection dbcon31 = new SqliteConnection("Data Source=Savings.db");

            dbcon31.Open();
            IDbCommand firstsave = dbcon31.CreateCommand();
            firstsave.CommandText = "SELECT * FROM Savings WHERE ChatId='" + Convert.ToString(message.Chat.Id) + "'";
            IDataReader reader2 = firstsave.ExecuteReader();

            reader2.Read();

            var _refBlockOn = Convert.ToInt32(reader2.GetInt32(6));
            reader2.Dispose();
            firstsave.Dispose();
            dbcon31.Close();

            if (_refBlockOn != 1)
            {
                dbcon31.Open();

                var _joinDate = DateTime.Today.ToString();
                string _source = "0";
                if (message.Text.Length > 6)
                {
                    _source = message.Text.Substring(7);
                }
                IDbCommand energyplus2 = dbcon31.CreateCommand();
                energyplus2.CommandText = "UPDATE Savings SET joindate = '" + _joinDate + "', source = '" + _source + "', source2 = 0,  source3 = 0,refblockon = 1 WHERE ChatId = " + message.Chat.Id.ToString() + "";
                energyplus2.ExecuteNonQuery();
                energyplus2.Dispose();

                if (_source != null)
                {
                    IDbCommand addSource2 = dbcon31.CreateCommand();
                    addSource2.CommandText = "SELECT * FROM Savings WHERE ChatId='" + _source + "'";
                    IDataReader reader3 = addSource2.ExecuteReader();

                    int source2 = 0;
                    int source3 = 0;
                    reader3.Read();
                    try
                    {
                        source2 = Convert.ToInt32(reader3.GetInt32(2));
                        source3 = Convert.ToInt32(reader3.GetInt32(3));
                    }
                    catch { }
                    Console.WriteLine(source2);
                    Console.WriteLine(source3);
                    reader3.Dispose();
                    addSource2.Dispose();

                    IDbCommand addSources = dbcon31.CreateCommand();
                    addSources.CommandText = "UPDATE Savings SET source2 = '" + source2 + "', source3 = '" + source3 + "' WHERE ChatId = " + message.Chat.Id.ToString() + "";
                    addSources.ExecuteNonQuery();
                    addSources.Dispose();
                }

                dbcon31.Close();

            }
        }
    }



}

