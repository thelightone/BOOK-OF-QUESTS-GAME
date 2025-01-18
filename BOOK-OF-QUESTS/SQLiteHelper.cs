using System;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

public class SQLiteHelper
{
    private string connectionString;

    public SQLiteHelper(string dbFilePath)
    {
        connectionString = $"Data Source={dbFilePath}";
    }

    public List<Dictionary<string, object>> GetData(string query)
    {
        List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (var command = new SqliteCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var row = new Dictionary<string, object>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row[reader.GetName(i)] = reader.GetValue(i);
                        }
                        data.Add(row);
                    }
                }
            }
        }

        return data;
    }
}