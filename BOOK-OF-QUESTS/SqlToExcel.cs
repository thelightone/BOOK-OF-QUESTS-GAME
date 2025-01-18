using System;
using Microsoft.Data.Sqlite;
using OfficeOpenXml;
using System.IO;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.InputFiles;
using System.Data;

class SqlToExcel
{
    async public void SaveExcel(TelegramBotClient bot, Update update)
    {
        try
        {
            // Путь к базе данных SQLite
            string sqliteDbPath = @"Savings.db";
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // SQL запрос для извлечения данных
            string query = "SELECT * FROM Savings WHERE paid = 0";

            // Устанавливаем подключение к базе данных
            using (var connection = new SqliteConnection($"Data Source={sqliteDbPath}"))
            {
                connection.Open();

                // Выполняем запрос
                using (var command = new SqliteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {

                    // Создаем Excel-файл
                    using (var package = new ExcelPackage())
                    {
                        // Добавляем новый лист
                        var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                        // Записываем заголовки в первую строку
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            worksheet.Cells[1, i + 1].Value = reader.GetName(i); // Заголовки столбцов
                        }

                        // Записываем данные в Excel
                        int row = 2;
                        while (reader.Read())
                        {
                            for (int col = 0; col < reader.FieldCount; col++)
                            {
                                worksheet.Cells[row, col + 1].Value = reader.GetValue(col);
                            }
                            row++;
                        }

                        // Сохраняем Excel-файл
                        string filePath = @"output.xlsx";  // Путь к файлу
                        FileInfo fi = new FileInfo(filePath);
                        package.SaveAs(fi);

                        IDbCommand firstsave = connection.CreateCommand();
                        firstsave.CommandText = "SELECT count(*) FROM Savings WHERE paid = 1";
                        int count = Convert.ToInt32(firstsave.ExecuteScalar());
                        firstsave.Dispose();
                        connection.Close();

                        using (var stream = new FileStream(@"output.xlsx", FileMode.Open))
                        {
                            await bot.SendDocumentAsync(update.Message.Chat, new InputOnlineFile(stream, "output.xlsx"));
                            await bot.SendTextMessageAsync(update.Message.Chat, "Подписок куплено: " + count.ToString());
                        }

                        Console.WriteLine("Данные успешно записаны в Excel!");

                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);  // Удаление файла
                            Console.WriteLine("Файл удален после отправки.");
                        }
                    }
                }
            }
        }
        catch { }
    }
}