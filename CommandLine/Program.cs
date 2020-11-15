using Extractor;
using Extractor.Extractors;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.ComponentModel.DataAnnotations;

namespace CommandLine
{
  internal class Program
  {
    [Option(Description = "Export Type", ShortName = "t")]
    private ExportType ExportType { get; } = ExportType.Both;

    [Option(Description = "Export Mode", ShortName = "m")]
    private ExportMode ExportMode { get; } = ExportMode.Everything;

    [Required]
    [Option(Description = "Game Folder", ShortName = "d")]
    private string MainGameFolder { get; } = "";

    [Required]
    [Option(Description = "Output Folder", ShortName = "o")]
    private string OutputFolderPath { get; } = "";

    public static int Main(string[] args) => CommandLineApplication.Execute<Program>(args);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Called with reflection")]
    private void OnExecute()
    {
      RunExtractions();
    }

    public void RunExtractions()
    {
      Console.Out.WriteLine("#---- Starting Extraction Operation ----#");

      string exportTypeString;
      if (ExportType == ExportType.TextList)
      {
        exportTypeString = "Text List";
      }
      else if (ExportType == ExportType.Json)
      {
        exportTypeString = "JSON";
      }
      else
      {
        exportTypeString = "Text List and JSON";
      }

      var localizationData = new LocalizationData(MainGameFolder, OutputFolderPath);

      switch (ExportMode)
      {
        case ExportMode.ItemExtraction:
          ExtractItems(localizationData, exportTypeString);
          break;
        case ExportMode.LocationExtraction:
          ExtractLocations(exportTypeString);
          break;
        case ExportMode.DumpAllXML:
          DumpAllXml();
          break;
        case ExportMode.Everything:
          ExtractItems(localizationData, exportTypeString);
          ExtractLocations(exportTypeString);
          DumpAllXml();
          break;
      }
      Console.Out.WriteLine("#---- Finished Extraction Operation ----#");
    }

    public void ExtractItems(LocalizationData localizationData, string exportTypeString)
    {
      Console.Out.WriteLine("--- Starting Extraction of Items as " + exportTypeString + " ---");
      new ItemExtractor(MainGameFolder, OutputFolderPath, ExportMode, ExportType).Extract(localizationData);
      Console.Out.WriteLine("--- Extraction Complete! ---");
    }

    public void ExtractLocations(string exportTypeString)
    {
      Console.Out.WriteLine("--- Starting Extraction of Locations as " + exportTypeString + " ---");
      new LocationExtractor(MainGameFolder, OutputFolderPath, ExportMode, ExportType).Extract();
      Console.Out.WriteLine("--- Extraction Complete! ---");
    }

    public void DumpAllXml()
    {
      Console.Out.WriteLine("--- Starting Extraction of All Files as XML ---");
      new BinaryDumper().Extract(MainGameFolder, OutputFolderPath);
      Console.Out.WriteLine("--- Extraction Complete! ---");
    }
  }
}
