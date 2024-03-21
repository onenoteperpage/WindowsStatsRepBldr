namespace WindowsStatsRepBldr.Helper;

internal class HelpMenu
{
    public static void ShowHelp(string targetFile)
    {
        // Set the console color for the header
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("Windows Stats Report Builder - Version v2.0");
        Console.WriteLine("-------------------------------------------\n");
        Console.ResetColor();

        // Instructions for file lookup
        Console.WriteLine($"By default, the application looks for a file named \"{targetFile}.sqlite\" in the same directory.\n");

        // Instructions for using the filename switch
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Alternatively, you can specify the path to the SQLite file using the \"/filename=\" switch.\n");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write($"Example: ");
        Console.ResetColor();
        Console.WriteLine($"C:\\WindowsStatsRepBldr.exe /filename=\"C:\\path\\to\\your\\{targetFile}.sqlite\"\n");
    }
}

