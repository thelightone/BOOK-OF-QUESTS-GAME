using System.Threading;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;

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
            if (update.Type == UpdateType.CallbackQuery)
            {
                _inlineHandler.MessageHandler(botClient, update);
            }

            // Получение сообщения
            var message = update.Message;

            // Идентификация пользователя и создание аккаунта
            _baseMechanic.Initial(botClient, message);

            // Проверка наличия необходимых подписок
            _advController.SubscrCheck(botClient, message);
            _messageCategoryHandler.TrialEnd(botClient, message);

            // Обработка сообщения
            _messageCategoryHandler.MessageHandler(botClient, message);
        }
    }
}
