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

namespace app8
{
    internal class MainPath
    {
        private static BaseMechanics _baseMechanic = new BaseMechanics();
        private static InlineHandler _inlineHandler = new InlineHandler();
        private static AdvController _advController = new AdvController();
        private static MessageCategoryHandler _messageCategoryHandler = new MessageCategoryHandler();
        async public void MessageReceive(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            var message = update.Message;
            // if (message != null || update.Type == UpdateType.CallbackQuery)
            //{
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
                        ReplyKeyboardMarkup keyBoard;
                        if (_messageCategoryHandler.prevKeyboard != null)
                        {
                            keyBoard = _messageCategoryHandler.prevKeyboard;

                            keyBoard.IsPersistent = true;
                        }
                        else
                        {
                            keyBoard = new(new[]
                {
                            new KeyboardButton[] {"🔘 Новая игра"},
                            new KeyboardButton[] {"👑 Подписка","☎️ Контакты"},
                 })
                            {
                                ResizeKeyboard = true
                            };
                        }
                        await bot.SendTextMessageAsync(update.Message.Chat, "Подписка успешно оформлена!", replyMarkup: keyBoard);
                        _inlineHandler.SuccesfulBuy();
                    }
                    break;
                default:
                    {
                        if (update.Type == UpdateType.CallbackQuery)
                        {
                            _inlineHandler.MessageHandler(botClient, update);
                        }

                        // Получение сообщения



                        else if (message != null)
                        {
                            try
                            {
                                // Идентификация пользователя и создание аккаунта
                                _baseMechanic.Initial(botClient, message);

                                // Проверка наличия необходимых подписок
                                //_advController.SubscrCheck(botClient, message);
                                //_messageCategoryHandler.TrialEnd(botClient, message);

                                // Обработка сообщения
                                _messageCategoryHandler.BaseMessageHandler(botClient, message);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex);
                            }
                            // }
                        }
                        break;
                    }
            };



        }



    }
}
