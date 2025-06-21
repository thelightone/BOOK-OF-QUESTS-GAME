using System.Threading;
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
using System.Collections;
using System.IO;
using GTranslatorAPI;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace app8
{
    internal class MainPath
    {
        private static InlineHandler _inlineHandler = new InlineHandler();
        private static ParseMode _parseMode = new ParseMode();
        private static SqlToExcel _sqlToExcel = new SqlToExcel();
        public static Dictionary<long, bool> _waitingForPrompt = new Dictionary<long, bool>();

        /// <summary>
        /// Проверяет, истекла ли подписка пользователя
        /// </summary>
        /// <param name="payDateStr">Дата оплаты в строковом формате</param>
        /// <returns>true если подписка истекла, false если активна или дата не указана</returns>
        private static bool IsSubscriptionExpired(string payDateStr)
        {
            if (string.IsNullOrEmpty(payDateStr))
                return false;
                
            try
            {
                var payDate = DateTime.Parse(payDateStr);
                var monthsSincePayment = (DateTime.Now - payDate).TotalDays / 30.44; // Примерно 30.44 дня в месяце
                return monthsSincePayment >= 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при парсинге даты оплаты: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Проверяет лимиты генераций для пользователя
        /// </summary>
        /// <param name="bot">Клиент бота</param>
        /// <param name="chatId">ID чата</param>
        /// <param name="userId">ID пользователя</param>
        /// <returns>true если генерация разрешена, false если достигнут лимит</returns>
        private static async Task<bool> CheckGenerationLimits(TelegramBotClient bot, long chatId, long userId)
        {
            using (var connection = new SqliteConnection("Data Source=Savings.db"))
            {
                connection.Open();
                var checkCommand = connection.CreateCommand();
                checkCommand.CommandText = @"
                    SELECT generationsToday, paid, lastGenerationDate, payDate 
                    FROM Savings 
                    WHERE ChatId = @chatId";
                checkCommand.Parameters.AddWithValue("@chatId", userId.ToString());
                
                using (var reader = checkCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var isPaid = Convert.ToInt32(reader["paid"]) == 1;
                        
                        // Проверяем, не истекла ли подписка для платных пользователей
                        if (isPaid)
                        {
                            var payDateStr = reader["payDate"] as string;
                            if (IsSubscriptionExpired(payDateStr))
                            {
                                // Подписка истекла, сбрасываем статус платного пользователя
                                var resetPaidCommand = connection.CreateCommand();
                                resetPaidCommand.CommandText = "UPDATE Savings SET paid = 0 WHERE ChatId = @chatId";
                                resetPaidCommand.Parameters.AddWithValue("@chatId", userId.ToString());
                                resetPaidCommand.ExecuteNonQuery();
                                isPaid = false;
                                
                                // Проверяем количество генераций для пользователя с истекшей подпиской
                                var generationsCount = Convert.ToInt32(reader["generationsToday"]);
                                if (generationsCount >= 2)
                                {
                                    var premiumKeyboard = new InlineKeyboardMarkup(new[]
                                    {
                                        new []
                                        {
                                            InlineKeyboardButton.WithCallbackData("👑 Подписка", "Месяц")
                                        }
                                    });

                                    await bot.SendTextMessageAsync(
                                        chatId: chatId,
                                        text: "Ваша подписка истекла. Вы достигли лимита бесплатных генераций (2 в день).\nПриобретите премиум подписку для неограниченного количества генераций!",
                                        replyMarkup: premiumKeyboard
                                    );
                                    return false;
                                }
                            }
                        }
                        
                        if (!isPaid)
                        {
                            // Проверяем дату последней генерации
                            var lastGenDate = reader["lastGenerationDate"] as string;
                            if (!string.IsNullOrEmpty(lastGenDate))
                            {
                                var lastDate = DateTime.Parse(lastGenDate);
                                if ((DateTime.Now - lastDate).TotalDays > 1)
                                {
                                    // Сбрасываем счетчик если прошло больше дня
                                    var resetCommand = connection.CreateCommand();
                                    resetCommand.CommandText = "UPDATE Savings SET generationsToday = 0 WHERE ChatId = @chatId";
                                    resetCommand.Parameters.AddWithValue("@chatId", userId.ToString());
                                    resetCommand.ExecuteNonQuery();
                                    
                                    // После сброса счетчика получаем обновленное значение
                                    var updatedCount = 0;
                                    var updatedCheckCommand = connection.CreateCommand();
                                    updatedCheckCommand.CommandText = "SELECT generationsToday FROM Savings WHERE ChatId = @chatId";
                                    updatedCheckCommand.Parameters.AddWithValue("@chatId", userId.ToString());
                                    updatedCount = Convert.ToInt32(updatedCheckCommand.ExecuteScalar());
                                    
                                    if (updatedCount >= 2)
                                    {
                                        var premiumKeyboard = new InlineKeyboardMarkup(new[]
                                        {
                                            new []
                                            {
                                                InlineKeyboardButton.WithCallbackData("👑 Подписка", "Месяц")
                                            }
                                        });

                                        await bot.SendTextMessageAsync(
                                            chatId: chatId,
                                            text: "Вы достигли лимита бесплатных генераций (2 в день).\nПриобретите премиум подписку для неограниченного количества генераций!",
                                            replyMarkup: premiumKeyboard
                                        );
                                        return false;
                                    }
                                }
                                else
                                {
                                    var generationsCount = Convert.ToInt32(reader["generationsToday"]);
                                    if (generationsCount >= 2)
                                    {
                                        var premiumKeyboard = new InlineKeyboardMarkup(new[]
                                        {
                                            new []
                                            {
                                                InlineKeyboardButton.WithCallbackData("👑 Подписка", "Месяц")
                                            }
                                        });

                                        await bot.SendTextMessageAsync(
                                            chatId: chatId,
                                            text: "Вы достигли лимита бесплатных генераций (2 в день).\nПриобретите премиум подписку для неограниченного количества генераций!",
                                            replyMarkup: premiumKeyboard
                                        );
                                        return false;
                                    }
                                }
                            }
                            else
                            {
                                var generationsCount = Convert.ToInt32(reader["generationsToday"]);
                                if (generationsCount >= 2)
                                {
                                    var premiumKeyboard = new InlineKeyboardMarkup(new[]
                                    {
                                        new []
                                        {
                                            InlineKeyboardButton.WithCallbackData("👑 Подписка", "Месяц")
                                        }
                                    });

                                    await bot.SendTextMessageAsync(
                                        chatId: chatId,
                                        text: "Вы достигли лимита бесплатных генераций (2 в день).\nПриобретите премиум подписку для неограниченного количества генераций!",
                                        replyMarkup: premiumKeyboard
                                    );
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }

        async public void MessageReceive(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            var message = update.Message;
            var bot = TelegramController.botClient;

            switch (update)
            {
                case { PreCheckoutQuery: { } preCheckoutQuery }:
                    if (preCheckoutQuery is { InvoicePayload: "unlock_X", Currency: "XTR", TotalAmount: 99 })
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
                        await botClient.SendTextMessageAsync(
                                chatId: update.Message.Chat.Id,
                                text: "⭐️ Спасибо за важу поддержку! Генерировать далее?",
                                replyMarkup: new InlineKeyboardMarkup(new[]
                                {
                                      new InlineKeyboardButton[]
                                                {
                                                    InlineKeyboardButton.WithCallbackData("🖼 Генерировать следущее", "Генерировать")
                                                },
                                                new InlineKeyboardButton[]
                                                {
                                                    InlineKeyboardButton.WithCallbackData("Назад", "Главное меню")
                                                }
                                })
                            );
                        _inlineHandler.SuccesfulBuy();
                    }
                    break;

                case { CallbackQuery: { } callbackQuery }:
                    try
                    {
                        _inlineHandler.MessageHandler(botClient, update);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка при обработке callback: {ex}");
                        await bot.SendTextMessageAsync(
                            chatId: callbackQuery.Message.Chat.Id,
                            text: "Произошла ошибка при обработке запроса. Пожалуйста, попробуйте позже."
                        );
                    }
                    break;

                case { Message: { } incomingMessage }:
                    try
                    {
                        string messageText = incomingMessage.Text;
                        Console.WriteLine(incomingMessage.Text);
                        if (messageText.Contains("/start"))
                        {
                            SavePlayer(bot, incomingMessage);
                        }
                        else if (messageText.Contains("utm"))
                        {
                            _inlineHandler.Statictic(botClient, incomingMessage);
                        }
                        else if (messageText.Contains("users"))
                        {
                            _sqlToExcel.SaveExcel(bot, update);
                        }
                        else
                        {
                            var chatMember = await bot.GetChatMemberAsync(
                                chatId: -1001531639213,
                                userId: incomingMessage.From.Id
                            );

                            if (chatMember.Status == ChatMemberStatus.Left || chatMember.Status == ChatMemberStatus.Kicked)
                            {
                                await bot.SendTextMessageAsync(
                                    chatId: incomingMessage.Chat.Id,
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
                                _waitingForPrompt[incomingMessage.From.Id] = false;
                                return;
                            }

                            var translator = new GTranslatorAPIClient();
                            var result = await translator.TranslateAsync(Languages.ru, Languages.en, messageText);

                            messageText = result.TranslatedText + " detailed, many details, 4k, uhd, deep color";

                            if (result.TranslatedText != null)
                            {
                                try
                                {
                                    // Проверяем лимиты генераций
                                    if (!await CheckGenerationLimits(bot, incomingMessage.Chat.Id, incomingMessage.From.Id))
                                    {
                                        return; // Лимит достигнут, выходим из метода
                                    }

                                    var generator = new ImageGenerator("http://95.165.164.57:7467");
                                    byte[] imageBytes = await generator.GenerateAndSaveImage(messageText);
                                    if (imageBytes != null)
                                    {
                                        using (var stream = new MemoryStream(imageBytes))
                                        {
                                            // Создание клавиатуры с кнопками
                                            InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(new[]
                                            {
                                                new InlineKeyboardButton[]
                                                {
                                                    InlineKeyboardButton.WithCallbackData("🖼 Генерировать следущее", "Генерировать")
                                                },
                                                new InlineKeyboardButton[]
                                                {
                                                    InlineKeyboardButton.WithCallbackData("Назад", "Главное меню")
                                                }
                                            });

                                            await bot.SendPhotoAsync(
                                                chatId: incomingMessage.Chat.Id,
                                                photo: new Telegram.Bot.Types.InputFiles.InputOnlineFile(stream),
                                                caption: "Prompt: " + result.TranslatedText,
                                                replyMarkup: inlineKeyboard
                                            );

                                            // Увеличиваем счетчик генераций после успешной генерации
                                            using (var updateConnection = new SqliteConnection("Data Source=Savings.db"))
                                            {
                                                updateConnection.Open();
                                                var updateCommand = updateConnection.CreateCommand();
                                                updateCommand.CommandText = @"
                                                    UPDATE Savings 
                                                    SET generationsToday = COALESCE(generationsToday, 0) + 1,
                                                        lastGenerationDate = datetime('now')
                                                    WHERE ChatId = @chatId";
                                                updateCommand.Parameters.AddWithValue("@chatId", incomingMessage.From.Id.ToString());
                                                updateCommand.ExecuteNonQuery();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        await bot.SendTextMessageAsync(
                                            chatId: incomingMessage.Chat.Id,
                                            text: "Извините, не удалось сгенерировать изображение. Попробуйте позже."
                                        );
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Ошибка при обновлении счетчика генераций: {ex.Message}");
                                }
                            }
                            _waitingForPrompt[incomingMessage.From.Id] = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                        await bot.SendTextMessageAsync(
                            chatId: incomingMessage.Chat.Id,
                            text: "Произошла ошибка при обработке сообщения. Пожалуйста, попробуйте позже."
                        );
                        if (_waitingForPrompt.ContainsKey(incomingMessage.From.Id))
                        {
                            _waitingForPrompt[incomingMessage.From.Id] = false;
                        }
                    }
                    break;
            }
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
                firstsave2.CommandText = "INSERT INTO Savings (ChatId, Paid, refblockon,generationsToday)" +
                "VALUES (@ChatId,0,0,0)";
                firstsave2.Parameters.Add(new SqliteParameter("@ChatId", Convert.ToString(message.Chat.Id)));
                firstsave2.ExecuteNonQuery();
                firstsave2.Dispose();
                dbcon09.Close();

                _inlineHandler.ReferalCheck(botClient, message);
                Console.WriteLine("PlayerSaved");
            }
        }

        private static List<long> PlayersIds()
        {
            List<long> userIds = new List<long>();

            using (IDbConnection getIds = new SqliteConnection("Data Source = Savings.db"))
            {
                getIds.Open();
                string query = "SELECT ChatId FROM Savings";
                IDbCommand command = getIds.CreateCommand();

                command.CommandText = query;

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        userIds.Add(reader.GetInt64(0));
                    }
                }
            }

            return userIds;
        }

        private static async Task SendInitialMessage(TelegramBotClient botClient, Message message)
        {
            // Создание клавиатуры с кнопками
            InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
        new InlineKeyboardButton[]
        {
            InlineKeyboardButton.WithCallbackData("🖼 Генерировать изображение", "Генерировать" )
        },
        new InlineKeyboardButton[]
        {
            InlineKeyboardButton.WithCallbackData("👑 Подписка", "Месяц")
        }});

            // Отправка сообщения с inline клавиатурой
            var sentMessage = await botClient.SendPhotoAsync(
                         chatId: message.Chat.Id,
                             photo: "https://github.com/thelightone/BOOK-OF-QUESTS-GAME/blob/main/photo_2025-06-18_18-45-17.jpg?raw=true",
                             caption: "👨‍🎨 <b>MidJoBot</b> - самый быстрый бот для генерации изображений." + "\n" +
                                 "Генерируйте изображения за секунды!" + "\n" + "\n" +
                                 "Используемая сеть - <b>Stable Diffusion.</b>",
                             parseMode: ParseMode.Html, // Разметка HTML
                             replyMarkup: inlineKeyboard // Клавиатура
                                                         // messageThreadId: message.MessageThreadId // Можно удалить, если не используете многопоточность
                         );
        }
    }
}




