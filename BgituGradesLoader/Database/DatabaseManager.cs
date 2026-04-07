using BgituGradesLoader.Database.Tables;
using Newtonsoft.Json;
using System.Text;

namespace BgituGradesLoader.Database
{
    public static class DatabaseManager
    {
        private const string API_LINK = "http://localhost:8080/api/";
        private const string API_NUKE = API_LINK + "migrations/truncate";
        private const string API_GROUP = API_LINK + "group";
        private const string API_DISCIPLINE = API_LINK + "discipline";
        private const string API_PAIR = API_LINK + "class";

        public static async Task NukeDatabase()
        {
            using HttpClient client = new();
            HttpRequestMessage request = CreateNewRequest(HttpMethod.Delete, API_NUKE);

            using HttpResponseMessage response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Не удалось очистить базу данных: {response.StatusCode}");
            }
        }

        public static async Task<DatabaseGroup> AddGroup(DatabaseGroup group)
        {
            return await AddObjectToDatabase(group, API_GROUP, "группу");
        }

        public static async Task<DatabaseDiscipline> AddDiscipline(DatabaseDiscipline discipline)
        {
            return await AddObjectToDatabase(discipline, API_DISCIPLINE, "дисциплину");
        }

        public static async Task AddPair(DatabasePair pair)
        {
            await AddObjectToDatabase(pair, API_PAIR, "пару");
        }

        public static async Task<List<DatabaseGroup>> AddGroups(List<DatabaseGroup> groups)
        {
            return await AddObjectToDatabase(groups, API_GROUP + "/bulk", "группы", "groups");
        }

        public static async Task<List<DatabaseDiscipline>> AddDisciplines(List<DatabaseDiscipline> disciplines)
        {
            return await AddObjectToDatabase(disciplines, API_DISCIPLINE + "/bulk", "дисциплины", "disciplines");
        }

        public static async Task AddPairs(List<DatabasePair> pairs)
        {
            await AddObjectToDatabase(pairs, API_PAIR + "/bulk", "пары", "classes");
        }

        private static async Task<T> AddObjectToDatabase<T>(T obj, string apiLink, string objName)
        {
            using HttpClient client = new();
            HttpRequestMessage request = CreateNewRequest(HttpMethod.Post, apiLink);
            string content = JsonConvert.SerializeObject(obj);
            request.Content = new StringContent(content, Encoding.UTF8, "application/json");

            using HttpResponseMessage response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Не удалось добавить {objName}: {response.StatusCode}");
                Console.ReadLine();
            }

            string result = await response.Content.ReadAsStringAsync();
            T? resultObject = JsonConvert.DeserializeObject<T>(result);

            if (resultObject == null)
            {
                Console.WriteLine($"Не удалось добавить {objName}: {response.StatusCode}");
                Console.ReadLine();
                return obj;
            }
            return resultObject;
        }

        private static async Task<List<T>> AddObjectToDatabase<T>(List<T> objects, string apiLink, string objName, string wrapperKey)
        {
            using HttpClient client = new();
            HttpRequestMessage request = CreateNewRequest(HttpMethod.Post, apiLink);

            string content = JsonConvert.SerializeObject(new Dictionary<string, object> { [wrapperKey] = objects });
            request.Content = new StringContent(content, Encoding.UTF8, "application/json");
            using HttpResponseMessage response = await client.SendAsync(request);

            string result = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Отправили {objects.Count} {objName}");
            Console.WriteLine($"Получили ответ: {result[..Math.Min(500, result.Length)]}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Не удалось добавить {objName}: {response.StatusCode}");
                Console.ReadLine();
                return objects;
            }

            List<T>? resultObjects = JsonConvert.DeserializeObject<List<T>>(result);
            Console.WriteLine($"Десериализовали: {resultObjects?.Count ?? 0} {objName}");

            return resultObjects ?? objects;
        }

        private static HttpRequestMessage CreateNewRequest(HttpMethod method, string link)
        {
            HttpRequestMessage request = new(method, link);
            string? apiKey = Environment.GetEnvironmentVariable("GRADES_API_KEY");
            Console.WriteLine(apiKey);
            apiKey = apiKey?.Trim(' ', '"');
            if (String.IsNullOrEmpty(apiKey))
                apiKey = "";

            request.Headers.Add("key", apiKey);
            return request;
        }

        public static async Task<string?> GetTableLink()
        {
            using HttpClient client = new();
            HttpRequestMessage request = CreateNewRequest(HttpMethod.Get, API_LINK + "settings");
            string? apiKey = Environment.GetEnvironmentVariable("GRADES_API_KEY");
            using HttpResponseMessage response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return null;

            string result = await response.Content.ReadAsStringAsync();
            var settings = JsonConvert.DeserializeObject<dynamic>(result);
            return settings?.calendarUrl;
        }
    }
}
