using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Microsoft.Data.Sqlite;
using System.Data;
using Telegram.Bot.Types.Enums;
using Qiwi.BillPayments.Client;
using Qiwi.BillPayments.Model.In;
using Qiwi.BillPayments.Model;
using Qiwi.BillPayments.Model.Out;
using System.Collections.Generic;
using Telegram.Bot.Types.Payments;


namespace app8
{
    internal class InlineHandler
    {
        private static ParseMode _parseMode = new ParseMode();

        // ОПЛАТА ПО КИВИ АПИ
        private static string _secretKey = "XXXX";
        private BillPaymentsClient _client = BillPaymentsClientFactory.Create(_secretKey);

        private string _linkWeek = "https://t.me/book_of_quests_paymentsbot";
        private string _linkMonth = "https://t.me/book_of_quests_paymentsbot";
        private string _linkYear = "https://t.me/book_of_quests_paymentsbot";
        private string _billIDWeek = null;
        private string _billIDUnl = null;
        private string _billIDMonth = null;
        private string _paid = "false";
        private string _payday = "0";

        private BillResponse _responseWeek;
        private BillResponse _responseUnl;
        private BillResponse _responseMonth;

        private CallbackQuery _callbackQuery;

        async public void MessageHandler(ITelegramBotClient botClient, Update update)
        {
            _callbackQuery = update.CallbackQuery;

            switch (_callbackQuery.Data)
            {
                case "Пригласить":

                    await botClient.SendTextMessageAsync(
                    chatId: _callbackQuery.Message.Chat.Id,
                    text: "Ваша персональная ссылка для приглашения, перешлите ее друзьям: " + "\n" + "t.me/book_of_quests_bot?start=" + _callbackQuery.Message.Chat.Id + "");
                    break;

                case "Месяц":

                    //HandlerPattern(botClient, _linkMonth, _responseMonth, _callbackQuery.Data, _billIDWeek, 30);
                    await botClient.SendInvoiceAsync(_callbackQuery.Message.Chat,
                                               "Получить премиум-подписку", "Дает безлимитный доступ ко всем квестам на месяц.", "unlock_X", "",
                                               "XTR", new List<LabeledPrice>() { new LabeledPrice("Price", 1) }, photoUrl: "https://cdn-icons-png.flaticon.com/512/891/891386.png");
                    break;

            }

        }

        public void SuccesfulBuy()
        {
            _paid = "1";
            _payday = DateTime.Today.AddDays(30).ToString();

            IDbConnection dbcon17 = new SqliteConnection("Data Source=Savings.db");
            dbcon17.Open();
            IDbCommand savetechnic1 = dbcon17.CreateCommand();
            savetechnic1.CommandText = "UPDATE Savings SET paid = '" + _paid + "', payday = '" + _payday + "' WHERE  ChatId='" + _callbackQuery.Message.Chat.Id.ToString() + "'";
            savetechnic1.ExecuteNonQuery();
            savetechnic1.Dispose();
            dbcon17.Close();
        }

        //private void QiwiPay()
        //{
        //    string billid = Guid.NewGuid().ToString();

        //    var paymentCreateWeek = _client.CreateBill(
        //        info: new CreateBillInfo
        //        {
        //            BillId = Guid.NewGuid().ToString(),
        //            Amount = new MoneyAmount
        //            {
        //                ValueDecimal = 79.0m,
        //                CurrencyEnum = CurrencyEnum.Rub
        //            },
        //            Comment = "BOOK OF QUESTS",
        //            ExpirationDateTime = DateTime.Now.AddMinutes(10),
        //            Customer = new Customer
        //            {
        //                Email = "example@mail.org",
        //                Account = Guid.NewGuid().ToString(),
        //                Phone = "79123456789"
        //            },
        //            SuccessUrl = new Uri("https://t.me/book_of_quests"),

        //        });

        //    var paymentCreateMonth = _client.CreateBill(
        //          info: new CreateBillInfo
        //          {
        //              BillId = Guid.NewGuid().ToString(),
        //              Amount = new MoneyAmount
        //              {
        //                  ValueDecimal = 149.0m,
        //                  CurrencyEnum = CurrencyEnum.Rub
        //              },
        //              Comment = "BOOK OF QUESTS",
        //              ExpirationDateTime = DateTime.Now.AddMinutes(10),
        //              Customer = new Customer
        //              {
        //                  Email = "example@mail.org",
        //                  Account = Guid.NewGuid().ToString(),
        //                  Phone = "79123456789"
        //              },
        //              SuccessUrl = new Uri("https://t.me/book_of_quests"),

        //          });

        //    var paymentCreateUnl = _client.CreateBill(
        //           info: new CreateBillInfo
        //           {
        //               BillId = Guid.NewGuid().ToString(),
        //               Amount = new MoneyAmount
        //               {
        //                   ValueDecimal = 199.0m,
        //                   CurrencyEnum = CurrencyEnum.Rub
        //               },
        //               Comment = "BOOK OF QUESTS",
        //               ExpirationDateTime = DateTime.Now.AddMinutes(10),
        //               Customer = new Customer
        //               {
        //                   Email = "example@mail.org",
        //                   Account = Guid.NewGuid().ToString(),
        //                   Phone = "79123456789"
        //               },
        //               SuccessUrl = new Uri("https://t.me/book_of_quests"),

        //           });

        //    _linkWeek = paymentCreateWeek.PayUrl.ToString();
        //    _linkYear = paymentCreateUnl.PayUrl.ToString();
        //    _linkMonth = paymentCreateMonth.PayUrl.ToString();

        //    _billIDWeek = paymentCreateWeek.BillId.ToString();
        //    _billIDMonth = paymentCreateMonth.BillId.ToString();
        //    _billIDUnl = paymentCreateUnl.BillId.ToString();

        //    _responseWeek = _client.GetBillInfo(_billIDWeek);
        //    _responseMonth = _client.GetBillInfo(_billIDMonth);
        //    _responseUnl = _client.GetBillInfo(_billIDUnl);
        //}

        //async private void HandlerPattern(ITelegramBotClient botClient, string urlLink, BillResponse response, string infoText, string billID, int duration)
        //{
        //    QiwiPay();

        //    InlineKeyboardMarkup inlineKeyboard = new(new[] {
        //        new[]{ InlineKeyboardButton.WithUrl(text: "💳 Банковской картой", url: urlLink) },
        //        new[]{ InlineKeyboardButton.WithUrl(text: "🥝 QIWI",  url:  urlLink) },
        //                                                    });

        //    Message sentMessage = await botClient.SendTextMessageAsync(
        //         chatId: _callbackQuery.Message.Chat.Id,
        //         text:"⚜️ Выберите способ оплаты:",
        //         (int?)(_parseMode = ParseMode.Html), replyMarkup: inlineKeyboard);

        //    string status = "0";

        //    do
        //    {
        //        await Task.Delay(60000);
        //        response = _client.GetBillInfo(billID);
        //        status = response.Status.ValueString;

        //        if (status == "PAID")
        //        {
        //            _paid = "1";
        //            string info = "0";
        //            _payday = DateTime.Today.AddDays(duration).ToString();
        //            info = infoText;

        //            IDbConnection dbcon17 = new SqliteConnection("Data Source=Savings.db");
        //            dbcon17.Open();
        //            IDbCommand savetechnic1 = dbcon17.CreateCommand();
        //            savetechnic1.CommandText = "UPDATE Savings SET paid = '" + _paid + "', payday = '" + _payday + "' WHERE  ChatId='" + _callbackQuery.Message.Chat.Id.ToString() + "'";
        //            savetechnic1.ExecuteNonQuery();
        //            savetechnic1.Dispose();
        //            dbcon17.Close();

        //            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[] { new KeyboardButton[] { "🔘 Продолжить игру" }, })
        //            {
        //                ResizeKeyboard = true
        //            };

        //            sentMessage = await botClient.SendTextMessageAsync(
        //                 chatId: _callbackQuery.Message.Chat.Id,
        //                 text: "Оплата успешно прошла!",
        //                 (int?)(_parseMode = ParseMode.Html), replyMarkup: replyKeyboardMarkup);

        //            sentMessage = await botClient.SendTextMessageAsync(
        //                chatId: 374579614,
        //                text: "Куплено: " + info,
        //                (int?)(_parseMode = ParseMode.Html));

        //            break;
        //        }
        //    }
        //    while (status != "EXPIRED" && status != "PAID");
        //}
    }
}

