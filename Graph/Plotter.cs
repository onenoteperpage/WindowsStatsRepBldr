using System.Text.Json;
using WindowsStatsRepBldr.Helper;
using WindowsStatsRepBldr.Models;

namespace WindowsStatsRepBldr.Graph;

public class Plotter
{
    public static async Task CreateGraphAsync(string jsonPath, string dateCode, string outputDirectory, string hostname)
    {
        List<DateTime> dataX = new List<DateTime>();
        List<double> dataY = new List<double>();

        // set filename
        string filename = Path.GetFileName(jsonPath);

        // set graphtype
        string graphType = StringMgr.ContainsAny(filename, new[] { "DRIVE", "RAM", "CPU" }, StringComparison.OrdinalIgnoreCase);

        try
        {
            // read the JSON file content
            string jsonString = await File.ReadAllTextAsync(jsonPath);

            // deserialize the JSON string to a List<WinboxDataObj>
            List<WinboxDataObj>? dataObjs = JsonSerializer.Deserialize<List<WinboxDataObj>>(jsonString);

            // iterate through the list and print the properties
            if (dataObjs != null)
            {
                // add graph labels
                switch (graphType)
                {
                    case "DRIVE":
                        // multiply by 100
                        foreach (var obj in dataObjs)
                        {
                            dataX.Add(obj.Timestamp);
                            dataY.Add(obj.Value * 100);
                        }
                        break;
                    default:
                        foreach (var obj in dataObjs)
                        {
                            dataX.Add(obj.Timestamp);
                            dataY.Add(obj.Value);
                        }
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }

        // create a new graph
        ScottPlot.Plot myPlot = new();

        // indicate X asix should use date/time values
        myPlot.Axes.DateTimeTicksBottom();

        // add the data
        myPlot.Add.Scatter(dataX, dataY);

        // add graph labels
        switch (graphType)
        {
            case "RAM":
                // straight up %
                myPlot.YLabel("RAM % Usage");
                myPlot.Title($"{dateCode} {hostname} RAM");
                break;
            case "DRIVE":
                // multiply by 100
                myPlot.YLabel("HDD % Usage");
                myPlot.Title($"{dateCode} {hostname} {filename.Replace(".json","").Replace($"{hostname}_","")}");
                break;
            case "CPU":
                // straight up %
                myPlot.YLabel("CPU % Usage");
                myPlot.Title($"{dateCode} {hostname} CPU");
                break;
            default:
                myPlot.YLabel("NOBODY");
                myPlot.Title("IS HOME");
                break;
        }
        myPlot.XLabel("Timestamp");

        // save graph
        myPlot.SavePng($"{Path.Combine(outputDirectory, filename.Replace(".json",""))}.png", 2400, 900);
    }
}

