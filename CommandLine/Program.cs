using Extractor;
using Extractor.Extractors;
using System;
using System.IO;

namespace CommandLine
{
  internal static class Program
  {
    private static string outputFolderPath = "";
    private static ExportType exportType;
    private static ExportMode exportMode;
    private static string mainGameFolder = "";

    private static void Main(string[] args)
    {
      ParseCommandline(args);

      RunExtractions();

      Console.Out.WriteLine("\nPress Any Key to Quit");
      Console.ReadKey();
    }

    public static void RunExtractions()
    {
      Console.Out.WriteLine("#---- Starting Extraction Operation ----#");

      string exportTypeString;
      if (exportType == ExportType.TextList)
      {
        exportTypeString = "Text List";
      }
      else if (exportType == ExportType.Json)
      {
        exportTypeString = "JSON";
      }
      else
      {
        exportTypeString = "Text List and JSON";
      }

      var localizationData = new LocalizationData(mainGameFolder, outputFolderPath);

      switch (exportMode)
      {
        case ExportMode.Item_Extraction:
          ExtractItems(localizationData, exportTypeString);
          break;
        case ExportMode.Location_Extraction:
          ExtractLocations(exportTypeString);
          break;
        case ExportMode.Dump_All_XML:
          DumpAllXml();
          break;
        case ExportMode.Extract_Items_Locations:
          ExtractItems(localizationData, exportTypeString);
          ExtractLocations(exportTypeString);
          break;
        case ExportMode.Everything:
          ExtractItems(localizationData, exportTypeString);
          ExtractLocations(exportTypeString);
          DumpAllXml();
          break;
      }
      Console.Out.WriteLine("#---- Finished Extraction Operation ----#");
    }

    public static void ExtractItems(LocalizationData localizationData, string exportTypeString)
    {
      Console.Out.WriteLine("--- Starting Extraction of Items as " + exportTypeString + " ---");
      new ItemExtractor(mainGameFolder, outputFolderPath, exportMode, exportType).Extract(localizationData);
      Console.Out.WriteLine("--- Extraction Complete! ---");
    }

    public static void ExtractLocations(string exportTypeString)
    {
      Console.Out.WriteLine("--- Starting Extraction of Locations as " + exportTypeString + " ---");
      new LocationExtractor(mainGameFolder, outputFolderPath, exportMode, exportType).Extract();
      Console.Out.WriteLine("--- Extraction Complete! ---");
    }

    public static void DumpAllXml()
    {
      Console.Out.WriteLine("--- Starting Extraction of All Files as XML ---");
      new BinaryDumper().Extract(mainGameFolder, outputFolderPath);
      Console.Out.WriteLine("--- Extraction Complete! ---");
    }

    private static void PrintHelp()
    {
      Console.WriteLine("How to use:\nao-id-extractor.exe modeID outFormat [outFolder]\n" +
          "modeID\t\t#Extraction 0=Item Extraction, 1=Location Extraction, 2=Dump All, 3=Extract Items & Locations, 4=Everything\n" +
          "outFormat\t#l=Text List, j=JSON b=Both\n" +
          "[outFolder]\t#OPTIONAL: Output folder path. Default: current directory\n" +
          "[gameFolder]\t#OPTIONAL: Location of the main AlbionOnline folder");
    }

    private static void ParseCommandline(string[] args)
    {
      if (args.Length >= 2)
      {
        var exportMode = int.Parse(args[0]);
        if (exportMode >= 0 && exportMode <= 4)
        {
          Program.exportMode = (ExportMode)exportMode;
        }
        else
        {
          PrintHelp();
          return;
        }

        if (args[1] == "l" || args[1] == "j" || args[1] == "b")
        {
          exportType = ExportType.Both;
          switch (args[1])
          {
            case "l":
              exportType = ExportType.TextList;
              break;
            case "j":
              exportType = ExportType.Json;
              break;
          }
        }
        else
        {
          PrintHelp();
          return;
        }

        if (args.Length >= 3)
        {
          if (string.IsNullOrWhiteSpace(args[2]))
          {
            outputFolderPath = Directory.GetCurrentDirectory();
          }
          else
          {
            outputFolderPath = args[2];
          }
        }

        if (args.Length == 4)
        {
          mainGameFolder = args[3];
        }
      }
      else
      {
        PrintHelp();
      }
    }
  }
}
