using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Microsoft.Data.Sqlite;
using System.Data;
using Telegram.Bot.Types.Enums;
using System.Globalization;

namespace app8
{
    internal class BaseMechanics
    {
        private static ParseMode _parseMode = new ParseMode();
        private static AdvController _advController = new AdvController();

        // ТЕХНИЧЕСКИЕ ПЕРЕМЕННЫЕ (Кнопки, описания комнат и т.д)
        public string _сurGame = "0";
        public string _stageQuest = "0";
        public string _invitation = "0";
        public string _findingsNum = "0";
        public string _finding = "nofindings";
        public string _textQuest = "1";
        public string _photolinkQuest = "1";
        public string _halfBut1TextQuest = "0";
        public string _halfBut2TextQuest = "0";
        public string _halfBut3TextQuest = "0";
        public string _halfBut4TextQuest = "0";
        public string _but1TextQuest = "1";
        public string _but2TextQuest = "1";
        public string _but3TextQuest = "1";
        public string _but4TextQuest = "1";
        public string _condition = "false";
        public string _addText1 = "1";
        public string _addText2 = "1";
        public string _addText3 = "1";
        public string _addText4 = "1";
        public string _chosedBut = "0";
        public string _placeChosBut = "0";
        public string _source = "0";
        public string _joinDate = "0";
        public string _payday = "false";
        public string _payLink = "false";
        public string _url = "0";
        public string _paid = "0";
        public int _subnumberQuest = 1;
        public int _numBut1 = 1;
        public int _numBut2 = 1;
        public int _numBut3 = 1;
        public int _numBut4 = 1;
        public int _energy = 0;
        public int _refBlockOn = 0;
        public int _playersTotal = 0;
        public bool _needSub = false;

        // Квест: Подземелье
        private bool _haveFire = false;
        private bool _haveKey = false;
        private bool _solvedQuest = false;
        private bool _haveStick1 = false;
        private bool _haveStick2 = false;
        private bool _haveStick0 = false;
        private bool _oskolok = false;
        private bool _stranger = false;
        private bool _sdelkaOtkaz = false;
        private bool _monsterDead = false;

        //Квест: Легенда о страннике 1
        private bool _haveStones = false;
        private bool _haveShovel = false;
        private bool _haveMeet = false;
        private bool _fisher = false;
        private bool _mistake = false;
        private bool _havePlant = false;
        private bool _ask = false;
        private bool _findFigure = false;

        //Квест: Шарманщик
        private bool _checkTable = false;
        private bool _checkBed = false;
        private bool _checkKomod = false;
        private bool _stayHome = false;
        private bool _checkKitch = false;
        private bool _checkAll = false;

        // Реклама: Обязательная подписка (ОП) и показы
        public long _opChan1 = 0;
        public string _opChanName1 = "0";
        public string _opBut1 = "0";

        public long _opChan2 = 0;
        public string _opChanName2 = "0";
        public string _opBut2 = "0";

        public long _opChan3 = 0;
        public string _opChanName3 = "0";
        public string _opBut3 = "0";

        public long _opChan4 = 0;
        public string _opChanName4 = "0";
        public string _opBut4 = "0";

        public string _opChanShow = "0";
        public string _opChanNameShow = "0";
        public string _opButShow = "0";
        public string _opChanShowUrl = "0";
        public string _opChanShowBut = "0";


        // ГЛАВНОЕ МЕНЮ
        async public void StartbotChoosegame(ITelegramBotClient botClient, Message message)
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[] {
                                new KeyboardButton[] { "🏰 RPG" },
                                new KeyboardButton[] { "☠️ Хоррор", "💋 18+" },
                                new KeyboardButton[] { "🔘 Вернуться в Главное меню" },
                            })
            {
                ResizeKeyboard = true
            };

            Message sentMessage = await botClient.SendPhotoAsync(
                chatId: message.Chat.Id,
                photo: "https://github.com/thelightone/questgame/raw/main/categories.jpg",
                caption: "Выберите категорию:",
                replyMarkup: replyKeyboardMarkup);
        }

        async public void Сontiniue(ITelegramBotClient botClient, Message message)
        {
            ReplyKeyboardMarkup replyKeyboardMarkup;

            if (_stageQuest == "0")
            {
                replyKeyboardMarkup = new(new[]
                 {
                            new KeyboardButton[] {"🔘 Новая игра"},
                            new KeyboardButton[] {"👑 Подписка","☎️ Контакты"},
                 })
                {
                    ResizeKeyboard = true
                };
            }
            else
            {
                replyKeyboardMarkup = new(new[]
                   {
                            new KeyboardButton[] {"🔘 Продолжить игру"},
                            new KeyboardButton[] {"🔘 Новая игра"},
                            new KeyboardButton[] {"👑 Подписка","☎️ Контакты"},
                   })
                {
                    ResizeKeyboard = true
                };
            }

            Message sentMessage = await botClient.SendPhotoAsync(
                         chatId: message.Chat.Id,
                         photo: "https://github.com/thelightone/questgame/raw/main/mainmenu.jpg",
                         caption: "<b>BOOK OF QUESTS</b> - лучшие игры прямо в твоём Телеграм!" + "\n" + "\n" +
                         "Жми <b>'Новая игра'</b> для начала ⬇️",
                         _parseMode = ParseMode.Html,
                         replyMarkup: replyKeyboardMarkup);
        }

        async public void СontiniueAlt(ITelegramBotClient botClient, Message message)
        {
            ReplyKeyboardMarkup replyKeyboardMarkup;

            if (_stageQuest == "0")
            {
                replyKeyboardMarkup = new(new[]
                {
                            new KeyboardButton[] {"🔘 Новая игра"},
                            new KeyboardButton[] {"☎️ Контакты"},
                 })
                {
                    ResizeKeyboard = true
                };
            }
            else
            {
                replyKeyboardMarkup = new(new[]
                {
                            new KeyboardButton [] {"🔘 Продолжить игру"},
                            new KeyboardButton[] {"🔘 Новая игра"},
                            new KeyboardButton[] {"☎️ Контакты"},
                 })
                {
                    ResizeKeyboard = true
                };
            }

            Message sentMessage = await botClient.SendPhotoAsync(
                         chatId: message.Chat.Id,
                         photo: "https://github.com/thelightone/questgame/raw/main/mainmenu.jpg",
                         caption: "<b>BOOK OF QUESTS</b> - лучшие игры прямо в твоём Телеграм!" + "\n" + "\n" +
                         "Жми <b>'Новая игра'</b> для начала ⬇️",
                         _parseMode = ParseMode.Html, replyMarkup: replyKeyboardMarkup);
        }

        //ТЕЛО ИГРЫ
        async public void Quest1(ITelegramBotClient botClient, Message message)
        {
            Database(botClient, message);

            ReplyKeyboardMarkup replyKeyboardMarkup;

            if (_url != "0" && _paid == "0")
            {
                var psevdenergy = _energy - 1;
                if (psevdenergy < 0) psevdenergy = 0;

                replyKeyboardMarkup = new(new[]
                {
                            new KeyboardButton[] { _but1TextQuest, _but3TextQuest },
                            new KeyboardButton[] { _but2TextQuest, _but4TextQuest },
                            new KeyboardButton[] { "⚡️ x "+(psevdenergy) },
                    })
                {
                    ResizeKeyboard = true
                };
            }
            else
            {
                replyKeyboardMarkup = new(new[]
                {
                            new KeyboardButton[] { _but1TextQuest, _but3TextQuest },
                            new KeyboardButton[] { _but2TextQuest, _but4TextQuest },
                    })
                {
                    ResizeKeyboard = true
                };
            }

            if (_photolinkQuest != "1")
            {
                Message sentMessage = await botClient.SendPhotoAsync(
                    chatId: message.Chat.Id,
                    photo: _photolinkQuest,
                    caption: _textQuest,
                    replyMarkup: replyKeyboardMarkup);
            }
            else
            {
                Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    _textQuest,
                    replyMarkup: replyKeyboardMarkup);
            }

            await Task.Delay(1000);

            CheckAdvShow(botClient, message);
        }

        async public void CheckAdvShow(ITelegramBotClient botClient, Message message)
        {
            if (_paid == "0" && _opChanShow != "0" && (_stageQuest == "7" || _energy == 7))
            {
                InlineKeyboardMarkup inlineKeyboard = new(new[] { new[] { InlineKeyboardButton.WithUrl(text: _opChanShowBut, url: _opChanShowUrl) } });

                Message sentMessage = await botClient.SendTextMessageAsync(
                               chatId: message.Chat.Id,
                               "" + _opChanShow + "", _parseMode = ParseMode.Html,
                               disableWebPagePreview: true,
                               replyMarkup: inlineKeyboard);

                IDbConnection dbcon2 = new SqliteConnection("Data Source=Savings.db");

                dbcon2.Open();
                IDbCommand checkpoint2 = dbcon2.CreateCommand();
                int opbutshow2 = Convert.ToInt32(_opButShow) + 1;
                checkpoint2.CommandText = "UPDATE op SET but = '" + opbutshow2 + "' WHERE opnum = 'opshow'";
                checkpoint2.ExecuteNonQuery();
                checkpoint2.Dispose();
                dbcon2.Close();
            }
        }

        async public void FindingCheck(ITelegramBotClient botClient, Message message)
        {
            await Task.Delay(100);

            if (_chosedBut == _placeChosBut)
            {
                switch (_finding)
                {
                    case "haveFire":
                        _haveFire = true;
                        break;
                    case "haveKey":
                        _haveKey = true;
                        break;
                    case "solvedQuest":
                        _solvedQuest = true;
                        break;
                    case "haveStick1":
                        _haveStick1 = true;
                        break;
                    case "haveStick2":
                        _haveStick2 = true;
                        break;
                    case "oskolok":
                        _oskolok = true;
                        break;
                    case "stranger":
                        _stranger = true;
                        break;
                    case "sdelkaOtkaz":
                        _sdelkaOtkaz = true;
                        break;
                    case "monsterDead":
                        _monsterDead = true;
                        break;
                    case "haveStones":
                        _haveStones = true;
                        break;
                    case "haveShovel":
                        _haveShovel = true;
                        break;
                    case "haveMeet":
                        _haveMeet = true;
                        break;
                    case "fisher":
                        _fisher = true;
                        break;
                    case "mistake":
                        _mistake = true;
                        break;
                    case "havePlant":
                        _havePlant = true;
                        break;
                    case "ask":
                        _ask = true;
                        break;
                    case "findFigure":
                        _findFigure = true;
                        break;
                    case "checkTable":
                        _checkTable = true;
                        break;
                    case "checkBed":
                        _checkBed = true;
                        break;
                    case "checkKomod":
                        _checkKomod = true;
                        break;
                    case "stayHome":
                        _stayHome = true;
                        break;
                    case "checkKitch":
                        _checkKitch = true;
                        break;
                    case "checkAll":
                        _checkAll = true;
                        break;
                }
            }
        }

        async public void ComplexFindings(ITelegramBotClient botClient, Message message)
        {
            if (_haveStick1 == true && _haveStick2 == true && _haveStick0 == false)
            {
                await botClient.SendPhotoAsync(
                chatId: message.Chat.Id,
                photo: "https://github.com/thelightone/questgame/raw/main/stick.png",
                caption: "Вы собрали Волшебный Посох ⚡️");
                _haveStick0 = true;
                await Task.Delay(1000);
            }
            else if (_checkBed == true && _checkTable == true && _checkKomod == true && _checkAll == false)
            {
                _checkAll = true;
            }
            SaveProgress(botClient, message);
        }

        async public void SubCheck(ITelegramBotClient botClient, Message message)
        {
            if (_payday == "false")
            {
                var user_channel_status = await botClient.GetChatMemberAsync(-1001620051798, long.Parse(Convert.ToString(message.Chat.Id)), default);
                if (user_channel_status.Status.ToString().ToLower() == "left"
                 || user_channel_status.Status.ToString().ToLower() == "kicked")
                {
                    _paid = "0";
                }
                else
                {
                    _paid = "1";
                    _url = "1";
                }
            }
            else if (_payday == "2")
            {
                var b = DateTime.Today;

                DateTime a2;
                if (_payday.Contains("/"))
                {
                    a2 = DateTime.Parse(_payday, CultureInfo.GetCultureInfo("en-US"));
                }
                else
                {
                    a2 = DateTime.Parse(_payday, CultureInfo.GetCultureInfo("ru-RU"));
                }

                if (b < a2)
                    _paid = "1";
                else
                {
                    _paid = "0"; _payday = "false";
                }
            }
        }

        async public void EnergyRestore(ITelegramBotClient botClient, Message message)
        {
            await Task.Delay(3600000);
            _energy = 11;
            SaveProgress(botClient, message);

            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[] { new KeyboardButton[] { "🔘 Продолжить игру" }, })
            {
                ResizeKeyboard = true
            };

            Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Ваша ⚡️Энергия  восстановлена!",
                    _parseMode = ParseMode.Html,
                    replyMarkup: replyKeyboardMarkup);
        }


        private void SetupOP(IDbConnection dbcon, long OPchan, string OPChanName, string opBut, string name)
        {
            IDbCommand opcheck1 = dbcon.CreateCommand();
            opcheck1.CommandText = "SELECT * FROM op WHERE opnum LIKE " + name + "";
            IDataReader opreader1 = opcheck1.ExecuteReader();
            opreader1.Read();
            OPchan = (long)Convert.ToDouble(opreader1.GetString(1));
            OPChanName = opreader1.GetString(2);
            opBut = "Подписаться";

            if (OPchan == 0)
                opBut = "";

            opcheck1.Dispose();
            opreader1.Dispose();
        }


        public void NewGame(ITelegramBotClient botClient, Message message)
        {
            _stageQuest = "0";
            _haveFire = false;
            _haveKey = false;
            _solvedQuest = false;
            _haveStick1 = false;
            _haveStick2 = false;
            _haveStick0 = false;
            _oskolok = false;
            _stranger = false;
            _sdelkaOtkaz = false;
            _monsterDead = false;
            _haveStones = false;
            _haveShovel = false;
            _haveMeet = false;
            _fisher = false;
            _mistake = false;
            _havePlant = false;
            _ask = false;
            _findFigure = false;
            _checkTable = false;
            _checkBed = false;
            _checkKomod = false;
            _stayHome = false;
            _checkKitch = false;
            _checkAll = false;
            _findingsNum = "0";
            _finding = "nofindings";
            _subnumberQuest = 1;
            _textQuest = "1";
            _photolinkQuest = "1";
            _halfBut1TextQuest = "0";
            _halfBut2TextQuest = "0";
            _halfBut3TextQuest = "0";
            _halfBut4TextQuest = "0";
            _but1TextQuest = "1";
            _but2TextQuest = "1";
            _but3TextQuest = "1";
            _but4TextQuest = "1";
            _condition = "false";
            _addText1 = "1";
            _addText2 = "1";
            _addText3 = "1";
            _addText4 = "1";
            _numBut1 = 1;
            _numBut2 = 1;
            _numBut3 = 1;
            _numBut4 = 1;
            _chosedBut = "0";
            _placeChosBut = "0";

            StartbotChoosegame(botClient, message);
            SaveProgress(botClient, message);
            SaveTechnic(botClient, message);
        }

        public void Database(ITelegramBotClient botClient, Message message)

        {
            IDbConnection dbcon = new SqliteConnection("Data Source=databaseforapp8.db");
            dbcon.Open();

            IDbCommand dbcmd = dbcon.CreateCommand();
            dbcmd.CommandText = "SELECT * FROM app8database WHERE Number ='" + _stageQuest + "' ";

            IDataReader reader = dbcmd.ExecuteReader();
            reader.Read();
            _condition = reader.GetString(20);
            reader.Dispose();
            dbcmd.Dispose();

            _subnumberQuest = 1;

            if ((_condition == "haveFire" && _haveFire == true)
                     || (_condition == "haveKey" && _haveKey == true)
                     || (_condition == "solvedQuest" && _solvedQuest == true)
                     || (_condition == "haveStick1" && _haveStick1 == true)
                     || (_condition == "haveStick2" && _haveStick2 == true)
                     || (_condition == "haveStick0" && _haveStick0 == true)
                     || (_condition == "oskolok" && _oskolok == true)
                     || (_condition == "stranger" && _stranger == true)
                     || (_condition == "sdelkaOtkaz" && _sdelkaOtkaz == true)
                     || (_condition == "monsterDead" && _monsterDead == true)
                     || (_condition == "haveStones" && _haveStones == true)
                     || (_condition == "haveShovel" && _haveShovel == true)
                     || (_condition == "haveMeet" && _haveMeet == true)
                     || (_condition == "fisher" && _fisher == true)
                     || (_condition == "mistake" && _mistake == true)
                     || (_condition == "havePlant" && _havePlant == true)
                     || (_condition == "ask" && _ask == true)
                     || (_condition == "findFigure" && _findFigure == true)
                     || (_condition == "checkTable" && _checkTable == true)
                     || (_condition == "checkBed" && _checkBed == true)
                     || (_condition == "checkKomod" && _checkKomod == true)
                     || (_condition == "stayHome" && _stayHome == true)
                     || (_condition == "checkKitch" && _checkKitch == true)
                     || (_condition == "checkAll" && _checkAll == true)
                )
                _subnumberQuest = 2;

            dbcmd = dbcon.CreateCommand();
            dbcmd.CommandText = "SELECT * FROM app8database WHERE Number LIKE'" + _stageQuest + "' AND SubNumber LIKE " + _subnumberQuest + "";

            reader = dbcmd.ExecuteReader();
            reader.Read();

            _subnumberQuest = reader.GetInt32(1);
            _textQuest = reader.GetString(4);

            _halfBut1TextQuest = reader.GetString(6);
            if (_halfBut1TextQuest == "0")
            {
                _but1TextQuest = "";
                _addText1 = "1";
                _numBut1 = Convert.ToInt32(_stageQuest);
            }
            else
            {
                _but1TextQuest = _halfBut1TextQuest;
                _addText1 = reader.GetString(7);
                _numBut1 = reader.GetInt32(5);
            }

            _halfBut2TextQuest = reader.GetString(9);
            if (_halfBut2TextQuest == "0")
            {
                _but2TextQuest = "";
                _addText2 = "1";
                _numBut2 = Convert.ToInt32(_stageQuest);
            }
            else
            {
                _but2TextQuest = _halfBut2TextQuest;
                _addText2 = reader.GetString(10);
                _numBut2 = reader.GetInt32(8);
            }

            _halfBut3TextQuest = reader.GetString(12);
            if (_halfBut3TextQuest == "0")
            {
                _but3TextQuest = "";
                _addText3 = "1";
                _numBut3 = Convert.ToInt32(_stageQuest);
            }
            else
            {
                _but3TextQuest = _halfBut3TextQuest;
                _addText3 = reader.GetString(13);
                _numBut3 = reader.GetInt32(11);
            }

            _halfBut4TextQuest = reader.GetString(15);
            if (_halfBut4TextQuest == "0")
            {
                _but4TextQuest = "";
                _addText4 = "1";
                _numBut4 = Convert.ToInt32(_stageQuest);
            }
            else
            {
                _but4TextQuest = _halfBut4TextQuest;
                _addText4 = reader.GetString(16);
                _numBut4 = reader.GetInt32(14);
            }

            _photolinkQuest = reader.GetString(17);
            _findingsNum = reader.GetString(18);
            _finding = reader.GetString(19);


            if (_findingsNum == Convert.ToString(_numBut1)) _placeChosBut = "1";
            else if (_findingsNum == Convert.ToString(_numBut2)) _placeChosBut = "2";
            else if (_findingsNum == Convert.ToString(_numBut3)) _placeChosBut = "3";
            else if (_findingsNum == Convert.ToString(_numBut4)) _placeChosBut = "4";

            FindingCheck(botClient, message);

            reader.Dispose();
            dbcmd.Dispose();
            dbcon.Close();
        }

        public void Checkpoint(ITelegramBotClient botClient, Message message)
        {
            IDbConnection dbcon03 = new SqliteConnection("Data Source=databaseforapp8.db");

            dbcon03.Open();
            IDbCommand checkpoint = dbcon03.CreateCommand();
            checkpoint.CommandText = "UPDATE app8database SET But1Link = '" + (Convert.ToInt32(_stageQuest)) + "' WHERE Number = 7";
            checkpoint.ExecuteNonQuery();
            checkpoint.Dispose();
            dbcon03.Close();
        }

        public void SaveProgress(ITelegramBotClient botClient, Message message)
        {

            IDbConnection dbcon20 = new SqliteConnection("Data Source=Savings.db");

            dbcon20.Open();
            IDbCommand savegame3 = dbcon20.CreateCommand();
            savegame3.CommandText = "UPDATE Savings SET Stagequest=@stagequest, HaveFire=@havefire, HaveKey=@havekey, SolvedQuest=@solvedquest, havestick1=@havestick1, havestick2=@havestick2, havestick0=@havestick0, CurGame=@curgame, invitation=@invitation, paid=@paid, oskolok=@oskolok, stranger=@stranger, sdelkaotkaz=@sdelkaotkaz, monsterdead=@monsterdead, pay_link=@pay_link, payday=@payday, url=@url, havestones=@havestones, haveshovel=@haveshovel, havemeet=@havemeet, fisher=@fisher, mistake=@mistake, haveplant=@haveplant, ask=@ask, findfigure=@findfigure, checktable=@checktable, checkbed=@checkbed, checkkomod=@checkkomod, stayhome=@stayhome, checkkitch=@checkkitch, checkall=@checkall, energy=@energy, refblockon=@refblockon, source=@source, joindate=@joindate WHERE ChatId=@chatid";

            savegame3.Parameters.Add(new SqliteParameter("@chatid", message.Chat.Id.ToString()));
            savegame3.Parameters.Add(new SqliteParameter("@stagequest", _stageQuest.ToString()));
            savegame3.Parameters.Add(new SqliteParameter("@havefire", _haveFire.ToString()));
            savegame3.Parameters.Add(new SqliteParameter("@havekey", _haveKey.ToString()));
            savegame3.Parameters.Add(new SqliteParameter("@solvedquest", _solvedQuest.ToString()));
            savegame3.Parameters.Add(new SqliteParameter("@havestick1", _haveStick1.ToString()));
            savegame3.Parameters.Add(new SqliteParameter("@havestick2", _haveStick2.ToString()));
            savegame3.Parameters.Add(new SqliteParameter("@havestick0", _haveStick0.ToString()));
            savegame3.Parameters.Add(new SqliteParameter("@curgame", _сurGame.ToString()));
            savegame3.Parameters.Add(new SqliteParameter("@invitation", _invitation.ToString()));
            savegame3.Parameters.Add(new SqliteParameter("@paid", _paid.ToString()));
            savegame3.Parameters.Add(new SqliteParameter("@oskolok", _oskolok.ToString()));
            savegame3.Parameters.Add(new SqliteParameter("@stranger", _stranger.ToString()));
            savegame3.Parameters.Add(new SqliteParameter("@sdelkaotkaz", _sdelkaOtkaz.ToString()));
            savegame3.Parameters.Add(new SqliteParameter("@monsterdead", _monsterDead.ToString()));
            savegame3.Parameters.Add(new SqliteParameter("@pay_link", _payLink.ToString()));
            savegame3.Parameters.Add(new SqliteParameter("@payday", _payday.ToString()));
            savegame3.Parameters.Add(new SqliteParameter("@url", _url.ToString()));
            savegame3.Parameters.Add(new SqliteParameter("@havestones", _haveStones.ToString()));
            savegame3.Parameters.Add(new SqliteParameter("@haveshovel", _haveShovel.ToString()));
            savegame3.Parameters.Add(new SqliteParameter("@havemeet", _haveMeet.ToString()));
            savegame3.Parameters.Add(new SqliteParameter("@fisher", _fisher.ToString()));
            savegame3.Parameters.Add(new SqliteParameter("@mistake", _mistake.ToString()));
            savegame3.Parameters.Add(new SqliteParameter("@haveplant", _havePlant.ToString()));
            savegame3.Parameters.Add(new SqliteParameter("@ask", _ask.ToString()));
            savegame3.Parameters.Add(new SqliteParameter("@findfigure", _findFigure.ToString()));
            savegame3.Parameters.Add(new SqliteParameter("@checktable", _checkTable.ToString()));
            savegame3.Parameters.Add(new SqliteParameter("@checkbed", _checkBed.ToString()));
            savegame3.Parameters.Add(new SqliteParameter("@checkkomod", _checkKomod.ToString()));
            savegame3.Parameters.Add(new SqliteParameter("@stayhome", _stayHome.ToString()));
            savegame3.Parameters.Add(new SqliteParameter("@checkkitch", _checkKitch.ToString()));
            savegame3.Parameters.Add(new SqliteParameter("@checkall", _checkAll.ToString()));
            savegame3.Parameters.Add(new SqliteParameter("@energy", _energy.ToString()));
            savegame3.Parameters.Add(new SqliteParameter("@refblockon", _refBlockOn.ToString()));
            savegame3.Parameters.Add(new SqliteParameter("@source", _source.ToString()));
            savegame3.Parameters.Add(new SqliteParameter("@joindate", _joinDate.ToString()));

            savegame3.ExecuteNonQuery();
            savegame3.Dispose();
            dbcon20.Close();
        }

        public void SaveTechnic(ITelegramBotClient botClient, Message message)
        {
            IDbConnection dbcon3 = new SqliteConnection("Data Source=technic.db");

            dbcon3.Open();
            IDbCommand savetechnic = dbcon3.CreateCommand();

            savetechnic.CommandText = "UPDATE technic SET FindingsNum = @FindingsNum, Finding = @Finding, subnumberQuest1 = @subnumberQuest1, TextQuest1 = @TextQuest1, photolinkQuest1 = @photolinkQuest1, HalfBut1TextQuest1 = @HalfBut1TextQuest1, HalfBut2TextQuest1 = @HalfBut2TextQuest1, HalfBut3TextQuest1 = @HalfBut3TextQuest1, HalfBut4TextQuest1 = @HalfBut4TextQuest1, But1TextQuest1 = @But1TextQuest1, But2TextQuest1 = @But2TextQuest1, But3TextQuest1 = @But3TextQuest1, But4TextQuest1 = @But4TextQuest1, condition = @condition, AddText1 = @AddText1, AddText2 = @AddText2, AddText3 = @AddText3, AddText4 = @AddText4, NumBut1 = @NumBut1, NumBut2 = @NumBut2, NumBut3 = @NumBut3, NumBut4 = @NumBut4, ChosedBut = @ChosedBut, PlaceChosBut = @PlaceChosBut WHERE ChatId = @ChatId";
            savetechnic.Parameters.Add(new SqliteParameter("@ChatId", message.Chat.Id.ToString()));
            savetechnic.Parameters.Add(new SqliteParameter("@FindingsNum", _findingsNum.ToString()));
            savetechnic.Parameters.Add(new SqliteParameter("@Finding", _finding.ToString()));
            savetechnic.Parameters.Add(new SqliteParameter("@subnumberQuest1", _subnumberQuest.ToString()));
            savetechnic.Parameters.Add(new SqliteParameter("@TextQuest1", _textQuest.ToString()));
            savetechnic.Parameters.Add(new SqliteParameter("@photolinkQuest1", _photolinkQuest.ToString()));
            savetechnic.Parameters.Add(new SqliteParameter("@HalfBut1TextQuest1", _halfBut1TextQuest.ToString()));
            savetechnic.Parameters.Add(new SqliteParameter("@HalfBut2TextQuest1", _halfBut2TextQuest.ToString()));
            savetechnic.Parameters.Add(new SqliteParameter("@HalfBut3TextQuest1", _halfBut3TextQuest.ToString()));
            savetechnic.Parameters.Add(new SqliteParameter("@HalfBut4TextQuest1", _halfBut4TextQuest.ToString()));
            savetechnic.Parameters.Add(new SqliteParameter("@But1TextQuest1", _but1TextQuest.ToString()));
            savetechnic.Parameters.Add(new SqliteParameter("@But2TextQuest1", _but2TextQuest.ToString()));
            savetechnic.Parameters.Add(new SqliteParameter("@But3TextQuest1", _but3TextQuest.ToString()));
            savetechnic.Parameters.Add(new SqliteParameter("@But4TextQuest1", _but4TextQuest.ToString()));
            savetechnic.Parameters.Add(new SqliteParameter("@condition", _condition.ToString()));
            savetechnic.Parameters.Add(new SqliteParameter("@AddText1", _addText1.ToString()));
            savetechnic.Parameters.Add(new SqliteParameter("@AddText2", _addText2.ToString()));
            savetechnic.Parameters.Add(new SqliteParameter("@AddText3", _addText3.ToString()));
            savetechnic.Parameters.Add(new SqliteParameter("@AddText4", _addText4.ToString()));
            savetechnic.Parameters.Add(new SqliteParameter("@NumBut1", _numBut1.ToString()));
            savetechnic.Parameters.Add(new SqliteParameter("@NumBut2", _numBut2.ToString()));
            savetechnic.Parameters.Add(new SqliteParameter("@NumBut3", _numBut3.ToString()));
            savetechnic.Parameters.Add(new SqliteParameter("@NumBut4", _numBut4.ToString()));
            savetechnic.Parameters.Add(new SqliteParameter("@ChosedBut", _chosedBut.ToString()));
            savetechnic.Parameters.Add(new SqliteParameter("@PlaceChosBut", _placeChosBut.ToString()));

            savetechnic.ExecuteNonQuery();
            savetechnic.Dispose();
            dbcon3.Close();
        }

        public void Initial(ITelegramBotClient botClient, Message message)
        {
            IDbConnection dbcon05 = new SqliteConnection("Data Source = Savings.db");

            dbcon05.Open();
            IDbCommand firstsave = dbcon05.CreateCommand();
            firstsave.CommandText = "SELECT count(*) FROM Savings WHERE ChatId='" + Convert.ToString(message.Chat.Id) + "'";
            int count = Convert.ToInt32(firstsave.ExecuteScalar());
            firstsave.Dispose();
            dbcon05.Close();

            if (count == 0)
            {
                IDbConnection dbcon09 = new SqliteConnection("Data Source = Savings.db");

                dbcon09.Open();
                IDbCommand firstsave2 = dbcon09.CreateCommand();
                firstsave2.CommandText = "INSERT INTO Savings (ChatId, Stagequest, HaveFire, HaveKey, SolvedQuest, havestick1, havestick2, havestick0, CurGame, invitation, paid, oskolok, stranger, sdelkaotkaz, monsterdead, pay_link, payday, url, havestones, haveshovel, havemeet, fisher, mistake, haveplant, ask, findfigure, checktable, checkbed, checkkomod, stayhome, checkkitch, checkall, energy, refblockon, source, joindate)" +
                "VALUES (@ChatId, @Stagequest, @HaveFire, @HaveKey, @SolvedQuest, @havestick1, @havestick2, @havestick0, @CurGame, @invitation, @paid, @oskolok, @stranger, @sdelkaotkaz, @monsterdead, @pay_link, @payday, @url, @havestones, @haveshovel, @havemeet, @fisher, @mistake, @haveplant, @ask, @findfigure, @checktable, @checkbed, @checkkomod, @stayhome, @checkkitch, @checkall, @energy, @refblockon, @source, @joindate)";
                firstsave2.Parameters.Add(new SqliteParameter("@ChatId", Convert.ToString(message.Chat.Id)));
                firstsave2.Parameters.Add(new SqliteParameter("@Stagequest", "0"));
                firstsave2.Parameters.Add(new SqliteParameter("@HaveFire", "false"));
                firstsave2.Parameters.Add(new SqliteParameter("@HaveKey", "false"));
                firstsave2.Parameters.Add(new SqliteParameter("@SolvedQuest", "false"));
                firstsave2.Parameters.Add(new SqliteParameter("@havestick1", "false"));
                firstsave2.Parameters.Add(new SqliteParameter("@havestick2", "false"));
                firstsave2.Parameters.Add(new SqliteParameter("@havestick0", "false"));
                firstsave2.Parameters.Add(new SqliteParameter("@CurGame", "0"));
                firstsave2.Parameters.Add(new SqliteParameter("@invitation", "0"));
                firstsave2.Parameters.Add(new SqliteParameter("@paid", "0"));
                firstsave2.Parameters.Add(new SqliteParameter("@oskolok", "false"));
                firstsave2.Parameters.Add(new SqliteParameter("@stranger", "false"));
                firstsave2.Parameters.Add(new SqliteParameter("@sdelkaotkaz", "false"));
                firstsave2.Parameters.Add(new SqliteParameter("@monsterdead", "false"));
                firstsave2.Parameters.Add(new SqliteParameter("@pay_link", "false"));
                firstsave2.Parameters.Add(new SqliteParameter("@payday", "false"));
                firstsave2.Parameters.Add(new SqliteParameter("@url", "0"));
                firstsave2.Parameters.Add(new SqliteParameter("@havestones", "false"));
                firstsave2.Parameters.Add(new SqliteParameter("@haveshovel", "false"));
                firstsave2.Parameters.Add(new SqliteParameter("@havemeet", "false"));
                firstsave2.Parameters.Add(new SqliteParameter("@fisher", "false"));
                firstsave2.Parameters.Add(new SqliteParameter("@mistake", "false"));
                firstsave2.Parameters.Add(new SqliteParameter("@haveplant", "false"));
                firstsave2.Parameters.Add(new SqliteParameter("@ask", "false"));
                firstsave2.Parameters.Add(new SqliteParameter("@findfigure", "false"));
                firstsave2.Parameters.Add(new SqliteParameter("@checktable", "false"));
                firstsave2.Parameters.Add(new SqliteParameter("@checkbed", "false"));
                firstsave2.Parameters.Add(new SqliteParameter("@checkkomod", "false"));
                firstsave2.Parameters.Add(new SqliteParameter("@stayhome", "false"));
                firstsave2.Parameters.Add(new SqliteParameter("@checkkitch", "false"));
                firstsave2.Parameters.Add(new SqliteParameter("@checkall", "false"));
                firstsave2.Parameters.Add(new SqliteParameter("@energy", "0"));
                firstsave2.Parameters.Add(new SqliteParameter("@refblockon", "0"));
                firstsave2.Parameters.Add(new SqliteParameter("@source", "0"));
                firstsave2.Parameters.Add(new SqliteParameter("@joindate", "0"));

                firstsave2.ExecuteNonQuery();
                firstsave2.Dispose();
                dbcon09.Close();
            }

            IDbConnection dbcon010 = new SqliteConnection("Data Source = Savings.db");

            dbcon010.Open();
            IDbCommand loading = dbcon010.CreateCommand();
            loading.CommandText =
                "SELECT * FROM Savings WHERE ChatId ='" + Convert.ToString(message.Chat.Id) + "' ";
            IDataReader reader2 = loading.ExecuteReader();
            Console.WriteLine("6");
            reader2.Read();

            _stageQuest = reader2.GetString(1);
            _haveFire = Convert.ToBoolean(reader2.GetString(2));
            _haveKey = Convert.ToBoolean(reader2.GetString(3));
            _solvedQuest = Convert.ToBoolean(reader2.GetString(4));
            _haveStick1 = Convert.ToBoolean(reader2.GetString(5));
            _haveStick2 = Convert.ToBoolean(reader2.GetString(6));
            _haveStick0 = Convert.ToBoolean(reader2.GetString(7));
            _сurGame = reader2.GetString(8);
            _invitation = reader2.GetString(9);
            _paid = reader2.GetString(10);
            _oskolok = Convert.ToBoolean(reader2.GetString(11));
            _stranger = Convert.ToBoolean(reader2.GetString(12));
            _sdelkaOtkaz = Convert.ToBoolean(reader2.GetString(13));
            _monsterDead = Convert.ToBoolean(reader2.GetString(14));
            _payLink = reader2.GetString(15);
            _payday = reader2.GetString(16);
            _url = reader2.GetString(17);
            _haveStones = Convert.ToBoolean(reader2.GetString(18));
            _haveShovel = Convert.ToBoolean(reader2.GetString(19));
            _haveMeet = Convert.ToBoolean(reader2.GetString(20));
            _fisher = Convert.ToBoolean(reader2.GetString(21));
            _mistake = Convert.ToBoolean(reader2.GetString(22));
            _havePlant = Convert.ToBoolean(reader2.GetString(23));
            _ask = Convert.ToBoolean(reader2.GetString(24));
            _findFigure = Convert.ToBoolean(reader2.GetString(25));
            _checkTable = Convert.ToBoolean(reader2.GetString(26));
            _checkBed = Convert.ToBoolean(reader2.GetString(27));
            _checkKomod = Convert.ToBoolean(reader2.GetString(28));
            _stayHome = Convert.ToBoolean(reader2.GetString(29));
            _checkKitch = Convert.ToBoolean(reader2.GetString(30));
            _checkAll = Convert.ToBoolean(reader2.GetString(31));
            _energy = Convert.ToInt32(reader2.GetInt32(32));
            _refBlockOn = Convert.ToInt32(reader2.GetInt32(33));
            _source = reader2.GetString(34);
            _joinDate = reader2.GetString(35);

            reader2.Dispose();
            loading.Dispose();
            dbcon010.Close();

            IDbConnection dbcon012 = new SqliteConnection("Data Source = Savings.db");
            dbcon012.Open();

            IDbCommand players = dbcon012.CreateCommand();
            players.CommandText = "SELECT count(*) FROM Savings";
            _playersTotal = Convert.ToInt32(players.ExecuteScalar());
            players.Dispose();

            SetupOP(dbcon012, _opChan1, _opChanName1, _opBut1, "'op1'");
            SetupOP(dbcon012, _opChan2, _opChanName2, _opBut2, "'op2'");
            SetupOP(dbcon012, _opChan3, _opChanName3, _opBut3, "'op3'");
            SetupOP(dbcon012, _opChan4, _opChanName4, _opBut4, "'op4'");

            IDbCommand opcheckshow = dbcon012.CreateCommand();
            opcheckshow.CommandText = "SELECT * FROM op WHERE opnum LIKE 'opshow'";
            IDataReader opreadershow = opcheckshow.ExecuteReader();
            opreadershow.Read();
            _opChanShow = opreadershow.GetString(1); ;
            _opChanNameShow = opreadershow.GetString(2);
            _opButShow = opreadershow.GetString(3);
            _opChanShowUrl = "0";
            _opChanShowBut = "0";

            if (_opChanNameShow.Contains("₽"))
            {
                _opChanShowUrl = _opChanNameShow.Substring(0, _opChanNameShow.IndexOf('₽'));
                _opChanShowBut = _opChanNameShow.Substring(_opChanNameShow.IndexOf('₽') + 1);
            }
            opcheckshow.Dispose();
            opreadershow.Dispose();
            dbcon012.Close();

            IDbConnection dbcon06 = new SqliteConnection("Data Source=technic.db");
            dbcon06.Open();

            IDbCommand technicadd1 = dbcon06.CreateCommand();
            technicadd1.CommandText = "SELECT count(*) FROM technic WHERE ChatId='" + Convert.ToString(message.Chat.Id) + "'";
            int count3 = Convert.ToInt32(technicadd1.ExecuteScalar());
            technicadd1.Dispose();
            dbcon06.Close();

            if (count3 == 0)
            {
                IDbConnection dbcon013 = new SqliteConnection("Data Source=technic.db");
                dbcon013.Open();

                IDbCommand technicadd2 = dbcon013.CreateCommand();
                technicadd2.CommandText = "INSERT INTO technic (ChatId, FindingsNum, Finding, subnumberQuest1, TextQuest1, photolinkQuest1, HalfBut1TextQuest1, HalfBut2TextQuest1, HalfBut3TextQuest1, HalfBut4TextQuest1, But1TextQuest1, But2TextQuest1, But3TextQuest1, But4TextQuest1, condition, AddText1, AddText2, AddText3, AddText4, NumBut1, NumBut2, NumBut3, NumBut4, ChosedBut, PlaceChosBut) VALUES (@ChatId, @FindingsNum, @Finding, @subnumberQuest1, @TextQuest1, @photolinkQuest1, @HalfBut1TextQuest1, @HalfBut2TextQuest1, @HalfBut3TextQuest1, @HalfBut4TextQuest1, @But1TextQuest1, @But2TextQuest1, @But3TextQuest1, @But4TextQuest1, @condition, @AddText1, @AddText2, @AddText3, @AddText4, @NumBut1, @NumBut2, @NumBut3, @NumBut4, @ChosedBut, @PlaceChosBut)";
                technicadd2.Parameters.Add(new SqliteParameter("@ChatId", message.Chat.Id.ToString()));
                technicadd2.Parameters.Add(new SqliteParameter("@FindingsNum", "0"));
                technicadd2.Parameters.Add(new SqliteParameter("@Finding", "nofindings"));
                technicadd2.Parameters.Add(new SqliteParameter("@subnumberQuest1", "1"));
                technicadd2.Parameters.Add(new SqliteParameter("@TextQuest1", "1"));
                technicadd2.Parameters.Add(new SqliteParameter("@photolinkQuest1", "1"));
                technicadd2.Parameters.Add(new SqliteParameter("@HalfBut1TextQuest1", "0"));
                technicadd2.Parameters.Add(new SqliteParameter("@HalfBut2TextQuest1", "0"));
                technicadd2.Parameters.Add(new SqliteParameter("@HalfBut3TextQuest1", "0"));
                technicadd2.Parameters.Add(new SqliteParameter("@HalfBut4TextQuest1", "0"));
                technicadd2.Parameters.Add(new SqliteParameter("@But1TextQuest1", "1"));
                technicadd2.Parameters.Add(new SqliteParameter("@But2TextQuest1", "1"));
                technicadd2.Parameters.Add(new SqliteParameter("@But3TextQuest1", "1"));
                technicadd2.Parameters.Add(new SqliteParameter("@But4TextQuest1", "1"));
                technicadd2.Parameters.Add(new SqliteParameter("@condition", "false"));
                technicadd2.Parameters.Add(new SqliteParameter("@AddText1", "1"));
                technicadd2.Parameters.Add(new SqliteParameter("@AddText2", "1"));
                technicadd2.Parameters.Add(new SqliteParameter("@AddText3", "1"));
                technicadd2.Parameters.Add(new SqliteParameter("@AddText4", "1"));
                technicadd2.Parameters.Add(new SqliteParameter("@NumBut1", "1"));
                technicadd2.Parameters.Add(new SqliteParameter("@NumBut2", "1"));
                technicadd2.Parameters.Add(new SqliteParameter("@NumBut3", "1"));
                technicadd2.Parameters.Add(new SqliteParameter("@NumBut4", "1"));
                technicadd2.Parameters.Add(new SqliteParameter("@ChosedBut", "0"));
                technicadd2.Parameters.Add(new SqliteParameter("@PlaceChosBut", "0"));

                technicadd2.ExecuteNonQuery();
                technicadd2.Dispose();
                dbcon013.Close();
            }

            IDbConnection dbcon014 = new SqliteConnection("Data Source=technic.db");
            dbcon014.Open();

            IDbCommand technikload = dbcon014.CreateCommand();
            technikload.CommandText = "SELECT * FROM technic WHERE ChatId ='" + Convert.ToString(message.Chat.Id) + "' ";
            IDataReader reader3 = technikload.ExecuteReader();
            reader3.Read();

            _findingsNum = reader3.GetString(1);
            _finding = reader3.GetString(2);
            _subnumberQuest = Convert.ToInt32(reader3.GetString(3));
            _textQuest = reader3.GetString(4);
            _photolinkQuest = reader3.GetString(5);
            _halfBut1TextQuest = reader3.GetString(6).Trim();
            _halfBut2TextQuest = reader3.GetString(7).Trim();
            _halfBut3TextQuest = reader3.GetString(8).Trim();
            _halfBut4TextQuest = reader3.GetString(9).Trim();
            _but1TextQuest = reader3.GetString(10).Trim();
            _but2TextQuest = reader3.GetString(11).Trim();
            _but3TextQuest = reader3.GetString(12).Trim();
            _but4TextQuest = reader3.GetString(13).Trim();
            _condition = reader3.GetString(14);
            _addText1 = reader3.GetString(15);
            _addText2 = reader3.GetString(16);
            _addText3 = reader3.GetString(17);
            _addText4 = reader3.GetString(18);
            _numBut1 = Convert.ToInt32(reader3.GetString(19));
            _numBut2 = Convert.ToInt32(reader3.GetString(20));
            _numBut3 = Convert.ToInt32(reader3.GetString(21));
            _numBut4 = Convert.ToInt32(reader3.GetString(22));
            _chosedBut = reader3.GetString(23);
            _placeChosBut = reader3.GetString(24);

            technikload.Dispose();
            reader3.Dispose();
            dbcon014.Close();
        }
    }
}


