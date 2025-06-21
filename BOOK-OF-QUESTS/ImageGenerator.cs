using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json; // Не забудьте установить пакет Newtonsoft.Json

// --- Классы с данными, как в исходном коде ---

public class EasyReq
{
    // Атрибут JsonProperty нужен, чтобы при отправке в JSON
    // имя поля было в "snake_case", как требует API.
    [JsonProperty("prompt")]
    public string prompt { get; set; }

    [JsonProperty("negative_prompt")]
    public string negative_prompt { get; set; } = "lowres, bad anatomy, bad hands, text, error, missing fingers, extra digit, fewer digits, cropped, worst quality, low quality, normal quality, jpeg artifacts, signature, watermark, username, blurry";

    [JsonProperty("steps")]
    public int steps { get; set; } = 5;

    [JsonProperty("sampler_name")]
    public string sampler_name { get; set; } = "Euler a";

    [JsonProperty("width")]
    public int width { get; set; } = 1024;

    [JsonProperty("height")]
    public int height { get; set; } = 1024;

    [JsonProperty("cfg_scale")]
    public double cfg_scale { get; set; } = 1.3;

    [JsonProperty("seed")]
    public long seed { get; set; } = -1;

    // Конструктор, который устанавливает только prompt
    public EasyReq(string prompt)
    {
        this.prompt = prompt;
    }
}

public class ImageResponse
{
    [JsonProperty("images")]
    public List<string> images { get; set; }

    [JsonProperty("parameters")]
    public object parameters { get; set; }

    [JsonProperty("info")]
    public string info { get; set; }

    // В исходном коде Unity было поле msg, возможно, оно есть в вашем JSON ответе
    public string msg { get; set; }
}


// --- Основная логика ---

public class ImageGenerator
{
    private static readonly HttpClient client = new HttpClient();
    private readonly string _url;

    public ImageGenerator(string baseUrl)
    {
        _url = baseUrl;
        // Настройка статичных заголовков
        var authString = Convert.ToBase64String(Encoding.UTF8.GetBytes("jarvis:money"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authString);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<byte[]> GenerateAndSaveImage(string prompt)
    {
        Console.WriteLine("Запрос на генерацию изображения...");

        // 1. Создаем объект запроса. Остальные поля уже имеют значения по умолчанию.
        var easyReq = new EasyReq(prompt);

        string jsonPayload = JsonConvert.SerializeObject(easyReq);
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        try
        {
            // 2. Отправка POST-запроса
            HttpResponseMessage response = await client.PostAsync(_url + "/sdapi/v1/txt2img", content);

            Console.WriteLine($"Код ответа: {(int)response.StatusCode} ({response.StatusCode})");
            response.EnsureSuccessStatusCode();

            // 3. Чтение и десериализация ответа
            string jsonResponse = await response.Content.ReadAsStringAsync();
            ImageResponse resp = JsonConvert.DeserializeObject<ImageResponse>(jsonResponse);

            if (resp?.images == null || resp.images.Count == 0)
            {
                Console.WriteLine("Ответ не содержит изображений.");
                return null;
            }

            // 4. Декодирование изображения в массив байтов
            byte[] imageBytes = Convert.FromBase64String(resp.images[0]);
            return imageBytes;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Произошла ошибка: {e.Message}");
            return null;
        }
    }
}
