using System;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Microsoft.Data.Sqlite;
using System.Data;
using Telegram.Bot.Types.Enums;

namespace app8
{
    internal class AdvController
    {
        private static ParseMode _parseMode = new ParseMode();
        private static BaseMechanics _baseMechanic = new BaseMechanics();

        // Реклама: Обязательная подписка (ОП) и показы
        private long _opChan1 = _baseMechanic._opChan1;
        private string _opChanName1 = _baseMechanic._opChanName1;
        private string _opBut1 = _baseMechanic._opBut1;

        private long _opChan2 = _baseMechanic._opChan2;
        private string _opChanName2 = _baseMechanic._opChanName2;
        private string _opBut2 = _baseMechanic._opBut2;

        private long _opChan3 = _baseMechanic._opChan1;
        private string _opChanName3 = _baseMechanic._opChanName3;
        private string _opBut3 = _baseMechanic._opBut3;

        private long _opChan4 = _baseMechanic._opChan1;
        private string _opChanName4 = _baseMechanic._opChanName4;
        private string _opBut4 = _baseMechanic._opBut4;

        private string _opChanShow = _baseMechanic._opChanShow;
        private string _opChanNameShow = _baseMechanic._opChanNameShow;
        private string _opChanShowUrl = _baseMechanic._opChanShowUrl;
        private string _opChanShowBut = _baseMechanic._opChanShowBut;


        private bool _needSub1 = false;
        private bool _needSub2 = false;
        private bool _needSub3 = false;
        private bool _needSub4 = false;

        async public void SubscrCheck(ITelegramBotClient botClient, Message message)
        {
            if (message.Text.Contains("opchan"))
            {
                string msg = message.Text.Substring(0, message.Text.IndexOf(' '));

                switch (msg)
                {
                    case "opchan1":

                        SetOP(botClient, message, _opChan1, _opBut1, _opChanName1, 1);
                        break;

                    case "opchanname1":

                        SetOPName(botClient, message, _opChanName1);
                        break;

                    case "opchan2":

                        SetOP(botClient, message, _opChan2, _opBut2, _opChanName2, 2);
                        break;

                    case "opchanname2":

                        SetOPName(botClient, message, _opChanName2);
                        break;

                    case "opchan3":

                        SetOP(botClient, message, _opChan3, _opBut3, _opChanName3, 3);
                        break;

                    case "opchanname3":

                        SetOPName(botClient, message, _opChanName3);
                        break;

                    case "opchan4":

                        SetOP(botClient, message, _opChan4, _opBut4, _opChanName4, 4);
                        break;

                    case "opchanname4":

                        SetOPName(botClient, message, _opChanName4);
                        break;

                    case "opchanshow":

                        _opChanShow = message.Text.Substring(11);
                        if (_opChanShow == "0")
                        {
                            _opChanShow = "0";
                            _opChanNameShow = "0";
                            _opChanShowBut = "0";
                            Message sendMessage = await botClient.SendTextMessageAsync(
                                         chatId: message.Chat.Id,
                                         "Ссылка для показов сброшена"
                            );
                        }
                        else
                        {
                            Message sendMessage = await botClient.SendTextMessageAsync(
                                    chatId: message.Chat.Id,
                                    "" + _opChanShow + "", _parseMode = ParseMode.Html
                                    );
                        }
                        break;

                    case "opchannameshow":

                        _opChanNameShow = message.Text.Substring(15);
                        _opChanShowUrl = _opChanNameShow.Substring(0, _opChanNameShow.IndexOf('₽'));
                        _opChanShowBut = _opChanNameShow.Substring(_opChanNameShow.IndexOf('₽') + 1);

                        InlineKeyboardMarkup inlineKeyboard = new(new[]
                                                            { new[]{ InlineKeyboardButton.WithUrl(text:_opChanShowBut,url:_opChanShowUrl) }
                                                            });

                        Message sentMessage = await botClient.SendTextMessageAsync(
                                    chatId: message.Chat.Id,
                                    "" + _opChanShow + "",
                                    replyMarkup: inlineKeyboard
                                    );
                        break;
                }

                SaveOP();
            }

            if (_opChan1 != 0)
            {
                var user_channel_status1 = await botClient.GetChatMemberAsync(_opChan1, long.Parse(Convert.ToString(message.Chat.Id)), default);
                if (user_channel_status1.Status.ToString().ToLower() == "left" || user_channel_status1.Status.ToString().ToLower() == "kicked")
                    _needSub1 = true;
                else
                    _opBut1 = "";
            }

            if (_opChan2 != 0)
            {
                var user_channel_status2 = await botClient.GetChatMemberAsync(_opChan2, long.Parse(Convert.ToString(message.Chat.Id)), default);
                if (user_channel_status2.Status.ToString().ToLower() == "left" || user_channel_status2.Status.ToString().ToLower() == "kicked")
                    _needSub2 = true;
                else
                    _opBut2 = "";
            }

            if (_opChan3 != 0)
            {
                var user_channel_status3 = await botClient.GetChatMemberAsync(_opChan3, long.Parse(Convert.ToString(message.Chat.Id)), default);
                if (user_channel_status3.Status.ToString().ToLower() == "left" || user_channel_status3.Status.ToString().ToLower() == "kicked")
                    _needSub3 = true;
                else
                    _opBut3 = "";
            }

            if (_opChan4 != 0)
            {
                var user_channel_status4 = await botClient.GetChatMemberAsync(_opChan4, long.Parse(Convert.ToString(message.Chat.Id)), default);
                if (user_channel_status4.Status.ToString().ToLower() == "left" || user_channel_status4.Status.ToString().ToLower() == "kicked")
                    _needSub4 = true;
                else
                    _opBut4 = "";
            }

            if ((_needSub1 == true || _needSub2 == true || _needSub3 == true || _needSub4 == true) && _baseMechanic._paid == "0")
                _baseMechanic._needSub = true;
        }

        public void SaveOP()
        {
            IDbConnection dbcon04 = new SqliteConnection("Data Source=Savings.db");

            dbcon04.Open();

            IDbCommand savetechnic1 = dbcon04.CreateCommand();
            savetechnic1.CommandText = "UPDATE op SET id = '" + _opChan1 + "', url = '" + _opChanName1 + "' WHERE opnum = 'op1'";
            savetechnic1.ExecuteNonQuery();
            savetechnic1.Dispose();

            IDbCommand savetechnic2 = dbcon04.CreateCommand();
            savetechnic2.CommandText = "UPDATE op SET id = '" + _opChan2 + "', url = '" + _opChanName2 + "'  WHERE opnum = 'op2'";
            savetechnic2.ExecuteNonQuery();
            savetechnic2.Dispose();

            IDbCommand savetechnic3 = dbcon04.CreateCommand();
            savetechnic3.CommandText = "UPDATE op SET id = '" + _opChan3 + "', url = '" + _opChanName3 + "'  WHERE opnum = 'op3'";
            savetechnic3.ExecuteNonQuery();
            savetechnic3.Dispose();

            IDbCommand savetechnic4 = dbcon04.CreateCommand();
            savetechnic4.CommandText = "UPDATE op SET id = '" + _opChan4 + "', url = '" + _opChanName4 + "'  WHERE opnum = 'op4'";
            savetechnic4.ExecuteNonQuery();
            savetechnic4.Dispose();

            IDbCommand savetechnic5 = dbcon04.CreateCommand();
            savetechnic5.CommandText = "UPDATE op SET id = '" + _opChanShow + "', url = '" + _opChanNameShow + "'  WHERE opnum = 'opshow'";
            savetechnic5.ExecuteNonQuery();
            savetechnic5.Dispose();

            dbcon04.Close();
        }

        async private void SetOP(ITelegramBotClient botClient, Message message, long opChan, string opBut, string opName, int num)
        {
            opChan = (long)Convert.ToDouble(message.Text.Substring(8));

            if (opChan == 0)
            {
                opBut = ""; opName = "t.me";
                Message sentMessage = await botClient.SendTextMessageAsync(
                             chatId: message.Chat.Id,
                             "ОП Канал " + num + " стерт"
                             );
            }

            else
            {
                opBut = "Подписаться";
                Message sentMessage = await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        "Успешно. Текущий канал для OP: '" + opChan + "'" + "\n" + ". Теперь укажи ссылку через opchanname"
                        );
            }
        }
        async private void SetOPName(ITelegramBotClient botClient, Message message, string opName)
        {
            opName = message.Text.Substring(12);
            Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    "Успешно. Текущий канал для OP: '" + opName + "'" + "\n" + ". Не забудь задать id "
                    );
        }
    }
}
