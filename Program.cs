using Newtonsoft.Json.Linq;
namespace Steam_App_Manifest_Fetcher
{
    internal class Program
    {
        private static string documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        private static string databaseFile = "";
        private static string destination = "";
        public static int index;

        public static async Task Main(string[] args)
        {
            Console.Title = "Steam App Manifest Fetcher";
            databaseFile = GetDatabasePath();
            destination = GetDestinationPath();

            if (args.Length > 0)
            {
                List<int> ids = [];
                foreach (var item in args)
                {
                    if (int.TryParse(item, out int id)) ids.Add(id);
                    else continue;
                }
                await GenerateMultipleAppManifests(ids);
                return;
            }

            WelcomeUser();
            Console.Write("Index: ");
            GetIndex();

            switch (index)
            {
                case 1:
                    GenerateSingleAppManifest();
                    break;
                case 2:
                    await GenerateMultipleAppManifests([]);
                    break;
                case 3:
                    SearchApplications();
                    break;
                case 4:
                    await UpdateDatabase();
                    break;
                case 5:
                    Console.Write("Press any key to continue...");
                    break;
                default:
                    Console.Clear();
                    await Main(args);
                    break;
            }
            static void GetIndex()
            {
                while (!int.TryParse(Console.ReadLine(), out index))
                {
                    if (index > 1 && index < 6) return;
                    Console.Clear();
                    WelcomeUser();
                }
            }
            Console.ReadKey();
        }
        public static void WelcomeUser()
        {
            Console.WriteLine("Welcome to Steam App Manifest Fetcher!\n\n" +
                "This is a program that fetches and generates an app manifest based on an app id provided.\n" +
                "It will then place it in your steam folder and appear in your library.\n" +
                "Just make to restart Steam after this.");
            Console.WriteLine($"\nSteam folder location: {destination}");
            Console.Write("Database: ");
            
            if (File.Exists(databaseFile))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Available\n\n");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Not Available\n\n");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            Console.WriteLine("Select any option below by typing their respective index");
            Console.Write("1. Generate Single App Manifest File\n"
                            + "2. Generate Multiple App Manifest Files\n"
                            + "3. Search Applications\n");
            Console.Write("4. Update Database ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("(SELECT ONLY IF NECESSARY)\n");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("5. Quit\n");
        }
        public static async Task UpdateDatabase(bool databaseExisted = true)
        {
            HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(300);
            Console.WriteLine("Downloading database... ");
            var response = await client.GetAsync("https://api.steampowered.com/ISteamApps/GetAppList/v2/");

            string responseContent = await response.Content.ReadAsStringAsync();
            using (TextWriter writer = new StreamWriter(databaseFile))
            {
                writer.Write(responseContent);
                Console.WriteLine("Updated Database");
            }
            if (databaseExisted) await Goodbye();
        }
        public static async void SearchApplications()
        {
            if (!File.Exists(databaseFile)) await UpdateDatabase(databaseExisted: false);
            while (true)
            {
                Console.Write("Search app: ");
                string? search = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(search)) break;
                JObject apps = JObject.Parse(File.ReadAllText(databaseFile));
                int counter = 1;
                Console.WriteLine();
                for (int i = 0; i < apps["applist"]["apps"].Count(); i++)
                {
                    string indexedSearchName = apps["applist"]["apps"][i]["name"].ToString();
                    string indexedSearchId = apps["applist"]["apps"][i]["appid"].ToString();
                    if (indexedSearchName.Contains(search.Trim()))
                    {
                        Console.WriteLine($"{counter}. {indexedSearchName} : {indexedSearchId}");
                        counter++;
                    }
                }
                Console.WriteLine("\nDone searching!");
            }
            await Goodbye();
        }
        public static async void GenerateSingleAppManifest()
        {
            if (!await CheckDatabaseFile()) return;
            HttpClient client = new HttpClient();

            Console.Write("App id: ");
            if (!int.TryParse(Console.ReadLine(), out int appId))
            {
                Console.Clear();
                GenerateSingleAppManifest();
                return;
            }

            if (appId == 0 || appId < 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid Index");
                Console.ForegroundColor = ConsoleColor.Gray;
                await Goodbye();
                return;
            }

            var response = await client.GetAsync($"https://store.steampowered.com/api/appdetails?appids={appId}");
            var responseString = await response.Content.ReadAsStringAsync();
            JObject Jobject = JObject.Parse(responseString);

            if (Jobject[appId.ToString()]["success"].ToString() == "False")
            {
                Console.WriteLine("Invalid App");
                await Goodbye();
                return;
            }
            string stringToWrite = GetAppManifestFormat(Jobject, appId);
            WriteAppManifest(appId, stringToWrite);
            await Goodbye();
        }
        public static async Task GenerateMultipleAppManifests(List<int> appIds)
        {
            if (!await CheckDatabaseFile()) return;
            HttpClient client = new HttpClient();

            int counter = 0;
            List<int> ids = appIds;

            if (ids.Count > 0)
            {
                for (int i = 0; i < ids.Count; i++)
                {
                    var response = await client.GetAsync($"https://store.steampowered.com/api/appdetails?appids={ids[i]}");
                    var responseString = await response.Content.ReadAsStringAsync();
                    JObject Jobject = JObject.Parse(responseString);

                    if (Jobject[ids[i].ToString()]["success"].ToString() == "False")
                    {
                        Console.WriteLine("Invalid App: " + ids[i]);
                        continue;
                    }
                    string stringToWrite = GetAppManifestFormat(Jobject, ids[i]);

                    WriteAppManifest(ids[i], stringToWrite);
                }
                return;
            }

            Console.WriteLine("When you're done adding apps, press ENTER");
            while (true)
            {
                Console.Write($"App {counter + 1}: ");
                try
                {
                    ids.Add(int.Parse(Console.ReadLine()));
                    if (ids[counter] == 0 || ids[counter] < 0)
                    {
                        ids.RemoveAt(counter);
                        break;
                    }
                }
                catch (FormatException) { break; }
                counter++;
            }

            for (int i = 0; i < ids.Count; i++)
            {
                var response = await client.GetAsync($"https://store.steampowered.com/api/appdetails?appids={ids[i]}");
                var responseString = await response.Content.ReadAsStringAsync();
                JObject Jobject = JObject.Parse(responseString);

                if (Jobject[ids[i].ToString()]["success"].ToString() == "False")
                {
                    Console.WriteLine("Invalid App: " + ids[i]);
                    continue;
                }
                string stringToWrite = GetAppManifestFormat(Jobject, ids[i]);

                WriteAppManifest(ids[i], stringToWrite);
            }
            await Goodbye();
        }
        public static string GetAppManifestFormat(JObject Jobject, int appId)
        {
            string stringToWrite = $"" +
                    $"\"AppState\"\r\n" +
                    $"{{\r\n" +
                    $"      \"appid\"           \"{appId}\"\r\n" +
                    $"      \"Universe\"        \"1\"\r\n" +
                    $"      \"installdir\"      \"{Jobject[appId.ToString()]["data"]["name"]}\"\r\n" +
                    $"      \"StateFlags\"      \"1026\"\r\n}}";
            Console.WriteLine($"Generated App Manifest for {Jobject[appId.ToString()]["data"]["name"]}");
            return stringToWrite;
        }
        public static void WriteAppManifest(int id, string stringToWrite)
        {
            if (OperatingSystem.IsWindows()) File.WriteAllText($"{destination}\\appmanifest_{id}.acf", stringToWrite);
            else if (OperatingSystem.IsLinux()) File.WriteAllText($"{destination}/appmanifest_{id}.acf", stringToWrite);
        }
        public static string GetDestinationPath()
        {
            if (OperatingSystem.IsWindows()) return "C:\\Program Files (x86)\\Steam\\steamapps";
            else if (OperatingSystem.IsLinux()) return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/Steam/steamapps";
            return "";
        }
        public static string GetDatabasePath()
        {
            if (OperatingSystem.IsWindows()) return documentsFolder + "\\database.txt";
            else if (OperatingSystem.IsLinux()) return documentsFolder + "/database.txt";
            return "";
        }
        public static async Task<bool> CheckDatabaseFile()
        {
            if (!File.Exists(databaseFile))
            {
                Console.WriteLine("Fetching database. Please wait a moment...");
                await UpdateDatabase(databaseExisted: false);
                Console.WriteLine("Launch the program again :)");
                await Goodbye();
                return false;
            }
            return true;
        }
        public static async Task Goodbye()
        {
            await Task.Run(() => Console.Write("Press any key to continue..."));
        }
    }
}