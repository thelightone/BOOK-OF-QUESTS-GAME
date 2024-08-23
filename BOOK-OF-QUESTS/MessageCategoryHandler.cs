using System;
using System.Threading.Tasks;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Microsoft.Data.Sqlite;
using System.Data;
using Telegram.Bot.Types.Enums;
using Qiwi.BillPayments.Web;

namespace app8
{
    internal class MessageCategoryHandler
    {
        private static ParseMode _parseMode = new ParseMode();
        private static BaseMechanics _baseMechanic = new BaseMechanics();
        private bool haveEnergy = true;
        public ReplyKeyboardMarkup prevKeyboard;

        async public void BaseMessageHandler(ITelegramBotClient botClient, Message message)
        {
            string messageText = message.Text;

            switch (messageText)
            {
                //ГЛАВНОЕ МЕНЮ
                case ("/start"):
                    _baseMechanic.SubCheck(botClient, message);
                    _baseMechanic.Сontiniue(botClient, message);
                    if (_baseMechanic.showedEnergyEnd != "1")
                        ReferalCheck(botClient, message);
                    _baseMechanic.SaveProgress(botClient, message);
                    _baseMechanic.SaveTechnic(botClient, message);
                    break;
                case ("🔘 Вернуться в Главное меню"):
                    _baseMechanic.SubCheck(botClient, message);
                    _baseMechanic.Сontiniue(botClient, message);
                    if (_baseMechanic.showedEnergyEnd != "1")
                        ReferalCheck(botClient, message);
                    _baseMechanic.SaveProgress(botClient, message);
                    _baseMechanic.SaveTechnic(botClient, message);
                    break;
                case ("🔘 Новая игра"):
                    OnNewGamePress(botClient, message);
                    break;
                case ("🔘 Начать новую игру"):
                    _baseMechanic.NewGame(botClient, message);
                    break;
                case ("🔘 Продолжить игру"):
                    _baseMechanic.SubCheck(botClient, message);
                    _baseMechanic.Quest1(botClient, message);
                    _baseMechanic.SaveProgress(botClient, message);
                    _baseMechanic.SaveTechnic(botClient, message);
                    break;
                case ("🔘 К выбору категорий"):
                    _baseMechanic.StartbotChoosegame(botClient, message);
                    break;
                case ("☎️ Контакты"):
                    Contacts(botClient, message);
                    break;
                case ("🔘 Об авторах"):
                    Authors(botClient, message);
                    break;
                case ("🔘 Помощь"):
                    Help(botClient, message);
                    break;
                case ("🔘 Связаться с нами"):
                    ContactUs(botClient, message);
                    break;
                case ("⚡️ x"):
                    EnergyInfo(botClient, message);
                    break;
                case ("👑 Подписка"):
                    OnSubPress(botClient, message);
                    break;
                case ("🔘 Пользовательское соглашение"):
                    message = await botClient.SendDocumentAsync(
                        chatId: message.Chat.Id,
                        document: "https://github.com/thelightone/questgame/raw/main/%D0%9F%D0%BE%D0%BB%D1%8C%D0%B7%D0%BE%D0%B2%D0%B0%D1%82%D0%B5%D0%BB%D1%8C%D1%81%D0%BA%D0%BE%D0%B5%20%D1%81%D0%BE%D0%B3%D0%BB%D0%B0%D1%88%D0%B5%D0%BD%D0%B8%D0%B5%20Book%20of%20Quests.pdf");
                    break;

                // КАТЕГОРИИ
                case ("🏰 RPG"):
                    RPGGames(botClient, message);
                    break;
                case ("☠️ Хоррор"):
                    HorrorGames(botClient, message);
                    break;
                case ("💋 18+"):
                    AdultGames(botClient, message);
                    break;
                case ("🔘 Да, я старше 18 лет"):
                    AdultGamesEntrance(botClient, message);
                    break;
                case ("🔘 Нет, мне нет 18 лет"):
                    _baseMechanic.StartbotChoosegame(botClient, message);
                    break;

                // ИГРЫ
                case ("🏰 'Не прыгай в Пропасть'"):
                    SwitchGame(botClient, message, "1", "14");
                    break;
                case ("🗡 'Легенда о Страннике. Часть I'"):
                    SwitchGame(botClient, message, "2", "99");
                    break;
                case ("🎩 'Шарманщик'"):
                    SwitchGame(botClient, message, "3", "701");                   
                    break;
                case ("🌸 'Знакомство с Лили'"):
                    SwitchGame(botClient, message, "4", "900");
                    break;
                case ("🏫 'Встреча выпускников'"):
                    SwitchGame(botClient, message, "5", "1001");
                    break;
                case ("♟ 'Встреча выпускников: Шахматный клуб'"):
                    SwitchGame(botClient, message, "6", "1137");
                    break;

                // АДМИН ПАНЕЛЬ
                case ("stats"):
                    Message sentMessage = await botClient.SendTextMessageAsync(
                       chatId: message.Chat.Id,
                       "Всего пользователей:'" + _baseMechanic._playersTotal + "'" + "\n" +
                       "ОП:'" + "\n" +
                        "'" + _baseMechanic._opChanName1 + "'" + "\n" +
                        "'" + _baseMechanic._opChanName2 + "'" + "\n" +
                        "'" + _baseMechanic._opChanName3 + "'" + "\n" +
                        "'" + _baseMechanic._opChanName4 + "'");
                    break;
                case ("utm "):
                    Statictic(botClient, message);
                    break;
                case ("pokaz"):
                    sentMessage = await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        "Показы:" + "\n" + "\n" +
                        "Ссылка: " + _baseMechanic._opChanShow + "\n" +
                        "Показов: " + _baseMechanic._opButShow + "");
                    break;
                default:
                    TrialEnd(botClient, message);
                    if(haveEnergy)
                    ButtonCheck(botClient, message);
                    break;
            } 
        }

        async public void TrialEnd(ITelegramBotClient botClient, Message message)
        {
            string _paid = "0";
            try
            {
                IDbConnection dbcon010 = new SqliteConnection("Data Source = Savings.db");

                dbcon010.Open();
                IDbCommand loading = dbcon010.CreateCommand();
                loading.CommandText =
                    "SELECT * FROM Savings WHERE ChatId ='" + Convert.ToString(message.Chat.Id) + "' ";
                IDataReader reader2 = loading.ExecuteReader();

                reader2.Read();

                _paid = reader2.GetString(10);

                reader2.Dispose();
                loading.Dispose();
                dbcon010.Close();

            }
            catch (Exception e)
            {

            }
            prevKeyboard = _baseMechanic._curKeyboard;

            haveEnergy = true;
            if (
                    (_paid != "1" && _baseMechanic.showedEnergyEnd == "1" && _baseMechanic._energy == 0)
                    || (_paid != "1" && _baseMechanic.showedEnergyEnd == "0"
                    && (((Convert.ToInt16(_baseMechanic._stageQuest) > 20) && _baseMechanic._сurGame == "1" && (Convert.ToInt16(_baseMechanic._stageQuest) < 70))// Пропасть
                    || ((Convert.ToInt16(_baseMechanic._stageQuest) > 184) && _baseMechanic._сurGame == "2" && (Convert.ToInt16(_baseMechanic._stageQuest) < 700))// Легенда 1
                    || ((Convert.ToInt16(_baseMechanic._stageQuest) > 756) && _baseMechanic._сurGame == "3" && (Convert.ToInt16(_baseMechanic._stageQuest) < 810))//Шарманщик
                    || ((Convert.ToInt16(_baseMechanic._stageQuest) > 1054) && _baseMechanic._сurGame == "5" && (Convert.ToInt16(_baseMechanic._stageQuest) < 1077))// Встреча вып 1
                    || ((Convert.ToInt16(_baseMechanic._stageQuest) > 1103) && _baseMechanic._сurGame == "6" && (Convert.ToInt16(_baseMechanic._stageQuest) < 1127))// Встреча вып 2 
                    || ((Convert.ToInt16(_baseMechanic._stageQuest) > 954) && _baseMechanic._сurGame == "4" && (Convert.ToInt16(_baseMechanic._stageQuest) < 1001))))//Лили
                    )
            {

                _baseMechanic.SubCheck(botClient, message);
                _baseMechanic.showedEnergyEnd = "1";

                haveEnergy = false;

                ReplyKeyboardMarkup replyKeyboardMarkup = new(new[] { new KeyboardButton[] { "⚡️ x 0" },
                                        new KeyboardButton[] { "🔘 Вернуться в Главное меню" },})

                {
                    ResizeKeyboard = true
                };

                Message sentMessage = await botClient.SendTextMessageAsync(
                      chatId: message.Chat.Id,
                      text: "У вас закончилась Энергия⚡️." + "\n" +
                      "Вы получите 10⚡️ через час.",
                      messageThreadId: message.MessageThreadId, _parseMode = ParseMode.Html,
                      replyMarkup: replyKeyboardMarkup);

                InlineKeyboardMarkup inlineKeyboard = new(new[] {

                       new[]{
                           InlineKeyboardButton.WithCallbackData(text: "🏆 Месяц - 149₽ (-53%) ",  callbackData: "Месяц") },

                       new[]{
                           InlineKeyboardButton.WithCallbackData( text:"Пригласить друга",  callbackData:"Пригласить") }, });

                sentMessage = await botClient.SendTextMessageAsync(
                           chatId: message.Chat.Id,
                           text: "⚜️ <b>Пригласите друзей:</b> " + "\n" +
                          "Получите 30⚡️ за каждого друга." + "\n" + "\n" +
                          "⚜️ <b>Получите Премиум:</b> " + "\n" +
                          "Бесконечная Энергия⚡️ и никакой рекламы!",
                          messageThreadId: message.MessageThreadId, _parseMode = ParseMode.Html, replyMarkup: inlineKeyboard);

                _baseMechanic.SaveProgress(botClient, message);
                _baseMechanic.SaveTechnic(botClient, message);
                _baseMechanic.EnergyRestore(botClient, message);
            }
        }

        async private void ButtonCheck(ITelegramBotClient botClient, Message message)
        {
            if (message.Text != null)
            {
                if (message.Text.Contains(_baseMechanic._but1TextQuest))
                {
                    OnButtonPress(botClient, message, "1", _baseMechanic._addText1, _baseMechanic._numBut1);
                }
                else if (message.Text.Contains(_baseMechanic._but2TextQuest))
                {
                    OnButtonPress(botClient, message, "2", _baseMechanic._addText2, _baseMechanic._numBut2);
                }
                else if (message.Text.Contains(_baseMechanic._but3TextQuest))
                {
                    OnButtonPress(botClient, message, "3", _baseMechanic._addText3, _baseMechanic._numBut3);
                }
                else if (message.Text.Contains(_baseMechanic._but4TextQuest))
                {
                    OnButtonPress(botClient, message, "4", _baseMechanic._addText4, _baseMechanic._numBut4);
                }
                else
                {
                    try
                    {
                        Message sentMessage = await botClient.SendPhotoAsync(
                                 chatId: message.Chat.Id,
                                 photo: "https://github.com/thelightone/questgame/raw/main/IMG_3030.jpg",
                                 caption: "Пожалуйста, используйте кнопки." + "\n" + "\n" +
                                 "Для переключения клавиатуры, нажмите сюда." + "\n" + "\n" +
                                 "Если не работают кнопки, нажмите 'Меню'->'Главное меню'->'Продолжить'."
                                 );
                    } catch (Exception e) { }
                }
            }
        }

        async private void AdultGamesEntrance(ITelegramBotClient botClient, Message message)
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                {
                        new KeyboardButton[] { "🌸 'Знакомство с Лили'" },
                        new KeyboardButton[] { "🏫 'Встреча выпускников'" },
                        new KeyboardButton[] { "♟ 'Встреча выпускников: Шахматный клуб'" },
                        new KeyboardButton[] { "🔘 К выбору категорий" }
                     })
            {
                ResizeKeyboard = true
            };

            Message sentMessage = await botClient.SendPhotoAsync(
                    chatId: message.Chat.Id,
                    photo: "https://github.com/thelightone/questgame/raw/main/18+.jpg",
                                                 messageThreadId: null,
                    caption: "Выберите игру:",
                    _parseMode = ParseMode.Html,
                    replyMarkup: replyKeyboardMarkup);

            _baseMechanic.SaveProgress(botClient, message);
            _baseMechanic.SaveTechnic(botClient, message);
        }

        async private void AdultGames(ITelegramBotClient botClient, Message message)
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                {
                      new KeyboardButton[] { "🔘 Да, я старше 18 лет" },
                      new KeyboardButton[] { "🔘 Нет, мне нет 18 лет" },
                      new KeyboardButton[] { "🔘 К выбору категорий" },
                    })

            {
                ResizeKeyboard = true
            };

            Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    "Вход в этот раздел доступен только лицам старше 18 лет. Вам есть 18 лет?",
                    replyMarkup: replyKeyboardMarkup);

            _baseMechanic.SaveProgress(botClient, message);
            _baseMechanic.SaveTechnic(botClient, message);
        }

        async private void HorrorGames(ITelegramBotClient botClient, Message message)
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                {
                        new KeyboardButton[] { "🎩 'Шарманщик'" },
                        new KeyboardButton[] { "🔘 К выбору категорий" }
                    })
            {
                ResizeKeyboard = true
            };

            Message sentMessage = await botClient.SendPhotoAsync(
                chatId: message.Chat.Id,
                photo: "https://github.com/thelightone/questgame/raw/main/horror.jpg",
                messageThreadId: null, caption: "Выберите игру:",
                _parseMode = ParseMode.Html,
                replyMarkup: replyKeyboardMarkup);

            _baseMechanic.SaveProgress(botClient, message);
            _baseMechanic.SaveTechnic(botClient, message);
        }

        async private void RPGGames(ITelegramBotClient botClient, Message message)
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                {
                        new KeyboardButton[] { "🏰 'Не прыгай в Пропасть'", },
                        new KeyboardButton[] { "🗡 'Легенда о Страннике. Часть I'", },
                        new KeyboardButton[] { "🔘 К выбору категорий" }
                    })
            {
                ResizeKeyboard = true
            };

            Message sentMessage = await botClient.SendPhotoAsync(
                  chatId: message.Chat.Id,
                  photo: "https://github.com/thelightone/questgame/raw/main/RPG.jpg",
                                               messageThreadId: null,
                  caption: "Выберите игру:",
                 _parseMode = ParseMode.Html, replyMarkup: replyKeyboardMarkup);

            _baseMechanic.SaveProgress(botClient, message);
            _baseMechanic.SaveTechnic(botClient, message);
        }

        async private void EnergyInfo(ITelegramBotClient botClient, Message message)
        {
            _baseMechanic.SubCheck(botClient, message);
            InlineKeyboardMarkup inlineKeyboard = new(
                                  new[] {
                                          new[]{
                                            InlineKeyboardButton.WithCallbackData(text: "🏆 Месяц - 149₽ (-53%) ",  callbackData: "Месяц") },

                                         new[]{
                                            InlineKeyboardButton.WithCallbackData( text:"Пригласить друга",  callbackData:"Пригласить") },
                                   });

            Message sentMessage = await botClient.SendTextMessageAsync(
                  chatId: message.Chat.Id,
                  text: "Это ваша ⚡️Энергия. Она расходуется на каждое действие и восстанавливается со временем." + "\n" +
                  "<b>Если вы не хотите ждать</b>:" + "\n" + "\n" +
                  "⚜️ <b>Пригласите друзей:</b> " + "\n" +
                  "Получите 30⚡️ за каждого друга." + "\n" + "\n" +
                  "⚜️ <b>Получите Премиум:</b> " + "\n" +
                  "Бесконечная Энергия⚡️ и никакой рекламы!",
                  messageThreadId: message.MessageThreadId, _parseMode = ParseMode.Html, replyMarkup: inlineKeyboard);
        }

        async private void ContactUs(ITelegramBotClient botClient, Message message)
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
               {
                        new KeyboardButton[] { "🔘 Об авторах" },
                        new KeyboardButton[] { "🔘 Помощь" },
                        new KeyboardButton[] { "🔘 Вернуться в Главное меню" },
                   })
            {
                ResizeKeyboard = true
            };

            Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    "С вопросами, пожеланиями и предложениями по сотрудничеству, вы можете обратиться сюда - @Khachapuri666",
                    replyMarkup: replyKeyboardMarkup);
        }

        async private void Help(ITelegramBotClient botClient, Message message)
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
               {
                       new KeyboardButton[] { "🔘 Об авторах" },
                       new KeyboardButton[] { "🔘 Связаться с нами" },
                       new KeyboardButton[] { "🔘 Вернуться в Главное меню" },
                   })
            {
                ResizeKeyboard = true
            };

            Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    "🔘 Если у вас что-то зависло/не работает:" + "\n" +
                    "Нажмите 'Меню'(синяя кнопка, слева над клавиатурой) -> '/start' -> 'Продолжить'" + "\n" +
                    "Если ошибка сохраняется, попробуйте удалить и заново войти в бота." + "\n" + "\n" +
                    "🔘 В случае, если ошибка связана с оплатой - напишите нам на @Khachapuri666, и вышлите ваш ChatId: '" + Convert.ToString(message.Chat.Id) + "'.",
                    replyMarkup: replyKeyboardMarkup);
        }

        async private void Authors(ITelegramBotClient botClient, Message message)
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
               {
                        new KeyboardButton[] { "🔘 Помощь" },
                        new KeyboardButton[] { "🔘 Связаться с нами" },
                        new KeyboardButton[] { "🔘 Вернуться в Главное меню" },
                   })
            {
                ResizeKeyboard = true
            };

            Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    "Наша команда:" + "\n" + "\n" +
                    "🐵@Khachapuri666 - тех.лид, автор квестов" + "\n" +
                    "🐰@SnezhkaBond - автор квестов" + "\n" +
                    "🐯@tematibr - тех.консультант" + "\n" +
                    "🐻@adhhda - тех. консультант" + "\n" +
                    "🦁@echoscomplex - python-программист" + "\n" +
                    "🐙@ex_future - автор музыки" + "\n" +
                    "🤖DALL-E 2 - художник",
                    replyMarkup: replyKeyboardMarkup);
        }

        async private void Contacts(ITelegramBotClient botClient, Message message)
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
               {
                       new KeyboardButton[] { "🔘 Об авторах" },
                       new KeyboardButton[] { "🔘 Помощь" },
                       new KeyboardButton[] { "🔘 Связаться с нами" },
                       new KeyboardButton[] { "🔘 Вернуться в Главное меню" },
                   })
            {
                ResizeKeyboard = true
            };

            Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    "По какому поводу вы обращаетесь?",
                    replyMarkup: replyKeyboardMarkup);
        }

        async private void OnNewGamePress(ITelegramBotClient botClient, Message message)
        {
            if (_baseMechanic._stageQuest != "0")
            {
                ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                    {
                            new KeyboardButton[] { "🔘 Начать новую игру" },
                            new KeyboardButton[] { "🔘 Вернуться в Главное меню" },
                    })

                {
                    ResizeKeyboard = true
                };

                Message sentMessage = await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        "Вы уверены? Текущий прогресс будет потерян.",
                        replyMarkup: replyKeyboardMarkup);
            }
            else
            {
                _baseMechanic.NewGame(botClient, message);
            }
        }

        async private void StartQuest(ITelegramBotClient botClient, Message message, string curGame, string stageQuest)
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                {
                        new KeyboardButton[] { " ", },
                })

            {
                ResizeKeyboard = true
            };

            Message sentMessage = await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "❗️Не нажимайте на несколько кнопок одновременно - это может привести к поломкам." + "\n" + "\n" +
                "Для возврата в Меню, пользуйтесь этой кнопкой:" + "\n" + "⬇️",
                messageThreadId: message.MessageThreadId,
                _parseMode = ParseMode.Html,
                replyMarkup: replyKeyboardMarkup);

            _baseMechanic._сurGame = curGame;
            _baseMechanic._stageQuest = stageQuest;

            await Task.Delay(2000);
            _baseMechanic.Quest1(botClient, message);

            _baseMechanic.SaveProgress(botClient, message);
            _baseMechanic.SaveTechnic(botClient, message);
        }

        async private void OPCheck(ITelegramBotClient botClient, Message message)
        {
            InlineKeyboardMarkup inlineKeyboard = new(new[]
            {
                    new[] {
                        InlineKeyboardButton.WithUrl(text: _baseMechanic._opBut1, url: _baseMechanic._opChanName1),
                        InlineKeyboardButton.WithUrl(text: _baseMechanic._opBut2, url: _baseMechanic._opChanName2)},

                    new[] {
                        InlineKeyboardButton.WithUrl(text: _baseMechanic._opBut3, url: _baseMechanic._opChanName3),
                        InlineKeyboardButton.WithUrl(text: _baseMechanic._opBut4, url: _baseMechanic._opChanName4)}
                });

            Message sentMessage2 = await botClient.SendTextMessageAsync(
                      chatId: message.Chat.Id,
                      text: "⚔️ Для начала игры, поддержите нас подпиской на спонсоров. Благодаря этому, игра может работать." + "\n" + "Затем нажмите кнопку еще раз.",
                                      messageThreadId: message.MessageThreadId,
                _parseMode = ParseMode.Html,
                      replyMarkup: inlineKeyboard);
        }

        async private void OnSubPress(ITelegramBotClient botClient, Message message)
        {
            _baseMechanic.SubCheck(botClient, message);

            if (_baseMechanic._paid != "1")
            {
                InlineKeyboardMarkup inlineKeyboard = new(new[] {

                        new[]{
                            InlineKeyboardButton.WithCallbackData(text: "🏆 Месяц - 149₽ (-53%) ",  callbackData: "Месяц") },

                        new[]{
                            InlineKeyboardButton.WithCallbackData( text:"Пригласить друга",  callbackData:"Пригласить") }, });

                Message sentMessage2 = await botClient.SendPhotoAsync(
                              chatId: message.Chat.Id,
                              photo: "https://github.com/thelightone/questgame/raw/main/podpiska.jpg",
                              caption: "👑 Премиум-подписка дает полный доступ ко всем квестам, включая новые квесты каждый месяц." + "\n" + "\n" +
                              "☕️ Стоимость подписки от 79 руб - дешевле, чем чашка кофе!" + "\n" + "\n" +
                              "⏱ Удобная оплата банковской картой всего за 2 минуты.",
                              replyMarkup: inlineKeyboard);
            }
            else
            {
                if (_baseMechanic._payday == "false")
                {
                    Message sentMessage2 = await botClient.SendPhotoAsync(
                          chatId: message.Chat.Id,
                          photo: "https://github.com/thelightone/questgame/raw/main/podpiska.jpg",
                          caption: "👑 Ваша подписка активна и дает вам полный доступ ко всем квестам, включая новые квесты каждый месяц!",
                          replyMarkup: new InlineKeyboardMarkup(
                                           InlineKeyboardButton.WithUrl(
                                               text: "Управлять подпиской",
                                               url: "https://t.me/book_of_quests_paymentsbot")));
                }
                else
                {
                    Message sentMessage2 = await botClient.SendPhotoAsync(
                          chatId: message.Chat.Id,
                          photo: "https://github.com/thelightone/questgame/raw/main/podpiska.jpg",
                          caption: "👑 Ваша подписка активна и дает вам полный доступ ко всем квестам." + "\n" +
                          "Дата окончания: " + _baseMechanic._payday.Replace("0:00:00", " "));
                }
            }
        }

        async private void Statictic(ITelegramBotClient botClient, Message message)
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

        async private void ReferalCheck(ITelegramBotClient botClient, Message message)
        {
            IDbConnection dbcon31 = new SqliteConnection("Data Source=Savings.db");
            dbcon31.Open();

            if (_baseMechanic._refBlockOn == 0)
            {
                _baseMechanic._joinDate = DateTime.Today.ToString();

                if (message.Text.Length > 6) _baseMechanic._source = message.Text.Substring(7);

                IDbCommand energyplus2 = dbcon31.CreateCommand();
                energyplus2.CommandText = "UPDATE Savings SET joindate = '" + _baseMechanic._joinDate + "', source = '" + _baseMechanic._source + "', refblockon = 1 WHERE ChatId = " + message.Chat.Id.ToString() + "";
                energyplus2.ExecuteNonQuery();
                energyplus2.Dispose();

                IDbCommand energyplus = dbcon31.CreateCommand();
                energyplus.CommandText = "UPDATE Savings SET energy = energy+30 WHERE ChatId = '" + _baseMechanic._source + "'";
                energyplus.ExecuteNonQuery();
                energyplus.Dispose();

                ReplyKeyboardMarkup replyKeyboardMarkup = new(new[] { new KeyboardButton[] { "🔘 Продолжить игру" }, })
                {
                    ResizeKeyboard = true
                };
                try
                {
                    Message sentMessage = await botClient.SendTextMessageAsync(
                        chatId: _baseMechanic._source,
                        text: "Поздравляем, ваш друг присоединился к игре! Вы получили 30 единиц ⚡️ Энергии.",
                        replyMarkup: replyKeyboardMarkup);
                }
                catch { return; }
            }
            dbcon31.Close();
        }

        async private void OnButtonPress(ITelegramBotClient botClient, Message message, string number, string addText, int numBut)
        {
            _baseMechanic.SubCheck(botClient, message);
            _baseMechanic._chosedBut = number;

            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[] { new KeyboardButton[] { " ", }, })
            {
                ResizeKeyboard = true
            };

            Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: addText,
                    replyMarkup: replyKeyboardMarkup);

            _baseMechanic._chosedBut = number;
            _baseMechanic.FindingCheck(botClient, message);
            
            await Task.Delay(500);
            if (numBut == 7)
            {
                _baseMechanic.Checkpoint(botClient, message);
            }

            _baseMechanic.ComplexFindings(botClient, message);

            await Task.Delay(1000);

            _baseMechanic._stageQuest = Convert.ToString(numBut);
            _baseMechanic.Quest1(botClient, message);



            if (_baseMechanic._energy > 0) _baseMechanic._energy = _baseMechanic._energy - 1;

            _baseMechanic.SaveProgress(botClient, message);
            _baseMechanic.SaveTechnic(botClient, message);
        }

        private void SwitchGame(ITelegramBotClient botClient, Message message, string game, string stage)
        {
            if (_baseMechanic._needSub == false)
                StartQuest(botClient, message, game, stage);
            else
                OPCheck(botClient, message);
        }
    }
}
