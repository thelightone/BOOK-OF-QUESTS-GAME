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


namespace app8
{
    internal class InlineHandler
    {
        private static ParseMode _parseMode = new ParseMode();

        // ОПЛАТА ПО КИВИ АПИ
        private static string _secretKey = "XXXX";

        private string _paid = "false";
        private string _payday = "0";

        private CallbackQuery _callbackQuery;

        async public void MessageHandler(ITelegramBotClient botClient, Update update)
        {
            _callbackQuery = update.CallbackQuery;

            switch (_callbackQuery.Data)
            {
                case "Пригласить":

                    await botClient.SendTextMessageAsync(
                    chatId: _callbackQuery.Message.Chat.Id,
                    text: "Твоя персональная ссылка для приглашения, перешлите ее друзьям: " + "\n"
                    + "t.me/top_gamez_bot?start=" + _callbackQuery.Message.Chat.Id + "");
                    break;

                case "Месяц":

                    //HandlerPattern(botClient, _linkMonth, _responseMonth, _callbackQuery.Data, _billIDWeek, 30);
                    await botClient.SendInvoiceAsync(chatId: _callbackQuery.Message.Chat.Id,
                                               "Отключить рекламу", "Поддержите проект и наслаждайтесь игрой без рекламы!", "unlock_X", "",
                                               "XTR", new List<LabeledPrice>() { new LabeledPrice("Price", 1) },
                                               photoUrl: "https://as2.ftcdn.net/v2/jpg/05/01/47/61/1000_F_501476117_i0AkipqtbO0vq6YGfECQVDhsvyJeUDDl.jpg");
                    break;

            }

        }

        public void SuccesfulBuy()
        {
            IDbConnection dbcon17 = new SqliteConnection("Data Source=Savings.db");
            dbcon17.Open();
            IDbCommand savetechnic1 = dbcon17.CreateCommand();
            savetechnic1.CommandText = "UPDATE Savings SET Paid = 1 WHERE  ChatId='" + _callbackQuery.Message.Chat.Id.ToString() + "'";
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

                    reader3.Read();

                    var source2 = Convert.ToInt32(reader3.GetInt32(2));
                    var source3 = Convert.ToInt32(reader3.GetInt32(3));
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

