using System.Text.Json;
using System.Text.RegularExpressions;
using WindowsStatsRepBldr.DBFunc;
using WindowsStatsRepBldr.Graph;
using WindowsStatsRepBldr.Helper;
using WindowsStatsRepBldr.Models;

namespace WindowsStatsRepBldr
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // clear the console screen
            Console.Clear();

            // set variables
            string currentYearMonth = DateTime.Now.ToString("yyyyMM");
            string sqliteFileName = $"{currentYearMonth}.sqlite";

            // set the sqliteFileName to the name of the provided file, by path is OK too
            if (args.Length == 1 && args[0].ToLower().Contains("/filename="))
            {
                sqliteFileName = args[0].ToLower().Replace("/filename=", "");
                currentYearMonth = Path.GetFileName(sqliteFileName).Substring(0, 6);

                // is the currentYearMonth valid?
                bool isValid = Regex.IsMatch(currentYearMonth, @"^\d{6}$");
                if (!isValid)
                {
                    HelpMenu.ShowHelp(targetFile: currentYearMonth);
                    Environment.Exit(1);
                }
            }

            // if the sqliteFileName is not found, write a message, readline to exit
            if (!File.Exists(sqliteFileName))
            {
                HelpMenu.ShowHelp(targetFile: currentYearMonth);
                Environment.Exit(1);
            }

            Console.Write("What is the name of the machine report file ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(sqliteFileName);
            Console.ResetColor();
            Console.WriteLine(" targets?");
            Console.WriteLine();

            string? machineName = null;
            while (string.IsNullOrEmpty(machineName))
            {
                Console.Write("Provide machine name for report: ");
                machineName = Console.ReadLine();
            }

            // replace "_" with "-" because it's needed later for splitting
            machineName = machineName.Replace("_", "-").ToUpperInvariant();
            string pattern = "[^a-zA-Z0-9-_]"; // Matches any character that is not alphanumeric, "-" or "_"
            machineName = Regex.Replace(machineName, pattern, "");

            // clear the console screen
            Console.Clear();

            // write the name to console
            Console.WriteLine($"The graphs will use: {machineName}");

            // create the directory for target machine report
            string outputDir = Path.Join(AppDomain.CurrentDomain.BaseDirectory, $"{currentYearMonth}_{machineName}");
            if (Directory.Exists(outputDir))
            {
                Directory.Delete(outputDir, true);
            }
            Directory.CreateDirectory(outputDir);

            Console.WriteLine($"\nOutput dir: {outputDir}");

            // store json file paths
            List<string> jsonFiles = new List<string>();

            // write out all the found table names
            string[] tableNames = GetDB.GetTableNames(sqliteFileName);
            Console.WriteLine();
            Console.WriteLine("Found tables in DB:");
            foreach (string tblName in tableNames)
            {
                Console.WriteLine($"  - {tblName}");

                List<WinboxDataObj> sortedWinboxDataObjs = GetDB.GetWinboxDataObjs(sqliteFilePath: sqliteFileName, tableName: tblName);

                string jsonString = JsonSerializer.Serialize(sortedWinboxDataObjs);
                
                // Write the JSON string to a file
                string filePath = Path.Join(outputDir, $"{machineName}_{tblName}.json"); // Specify the file path where you want to save the JSON file
                File.WriteAllText(filePath, jsonString);

                jsonFiles.Add(filePath);
            }

            Console.WriteLine();

            // create graphs
            foreach (string jsonFile in jsonFiles)
            {
                Console.WriteLine($"Target file: {jsonFile}");
                await Plotter.CreateGraphAsync(jsonPath: jsonFile, dateCode: currentYearMonth, outputDirectory: outputDir, hostname: machineName);
            }
        }
    }
}
