using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Extractor.Extractors
{
  public class IDContainer
  {
    public string Index { get; set; }
    public string UniqueName { get; set; }
  }

  public class ItemContainer : IDContainer
  {
    public string LocalizationNameVariable { get; set; }
    public string LocalizationDescriptionVariable { get; set; }
    public Dictionary<string, string> LocalizedNames { get; set; }
    public Dictionary<string, string> LocalizedDescriptions { get; set; }
  }

  public enum ExportType
  {
    TextList,
    Json,
    Both
  }

  public enum ExportMode
  {
    Item_Extraction,
    Location_Extraction,
    Dump_All_XML,
    Extract_Items_Locations,
    Everything
  }

  public abstract class BaseExtractor
  {
    protected readonly string outputFolderPath;
    protected readonly string mainGameFolder;
    protected readonly ExportType exportType;
    protected readonly ExportMode exportMode;

    public BaseExtractor(string mainGameFolder, string outputFolderPath, ExportMode exportMode, ExportType exportType)
    {
      this.outputFolderPath = outputFolderPath;
      this.mainGameFolder = mainGameFolder;
      this.exportType = exportType;
      this.exportMode = exportMode;
    }

    protected abstract string GetBinFilePath();
    protected abstract void ExtractFromXML(Stream inputXmlFile, MultiStream outputStream, Action<MultiStream, IDContainer, bool> writeItem, LocalizationData localizationData = default);

    protected XmlElement FindElement(XmlNode node, string elementName)
    {
      foreach (XmlNode childNode in node.ChildNodes)
      {
        if (childNode is XmlElement ele && ele.Name == elementName)
        {
          return ele;
        }
      }

      return null;
    }

    public void Extract(LocalizationData localizationData = default)
    {
      var xmlPath = DecryptBinFile(GetBinFilePath(), outputFolderPath);
      using (var inputFile = File.OpenRead(xmlPath))
      {
        var streamTypes = new List<StreamType>();
        if (exportType == ExportType.TextList || exportType == ExportType.Both)
        {
          const ExportType exportType = ExportType.TextList;
          streamTypes.Add(new StreamType
          {
            Stream = GetExportStream(exportType),
            ExportType = exportType
          });
        }
        if (exportType == ExportType.Json || exportType == ExportType.Both)
        {
          const ExportType exportType = ExportType.Json;
          streamTypes.Add(new StreamType
          {
            Stream = GetExportStream(exportType),
            ExportType = exportType
          });
        }
        var multiStream = new MultiStream(streamTypes.ToArray());

        ExtractFromXML(inputFile, multiStream, WriteItem, localizationData);

        foreach (var streamType in streamTypes)
        {
          CloseExportStream(streamType.Stream, streamType.ExportType);
          streamType.Stream.Close();
        }
      }
    }

    public static string DecryptBinFile(string binFile, string outputFolderPath)
    {
      var binFileWOE = Path.GetFileNameWithoutExtension(binFile);

      Console.Out.WriteLine("Extracting " + binFileWOE + ".bin...");

      var finalOutPath = Path.ChangeExtension(Path.Combine(outputFolderPath, binFile.Substring(binFile.LastIndexOf("GameData\\") + 9)), ".xml");
      Directory.CreateDirectory(Path.GetDirectoryName(finalOutPath));

      using (var outputStream = File.Create(finalOutPath))
      {
        BinaryDecrypter.DecryptBinaryFile(binFile, outputStream);
      }

      return finalOutPath;
    }

    private Stream GetExportStream(ExportType exportType)
    {
      var filePathWithoutExtension = Path.Combine(outputFolderPath, "formatted", Path.GetFileNameWithoutExtension(GetBinFilePath()));
      if (!Directory.Exists(Path.GetDirectoryName(filePathWithoutExtension)))
      {
        var di = Directory.CreateDirectory(Path.GetDirectoryName(filePathWithoutExtension));
      }

      if (exportType == ExportType.TextList)
      {
        return File.Create(filePathWithoutExtension + ".txt");
      }
      else if (exportType == ExportType.Json)
      {
        var stream = File.Create(filePathWithoutExtension + ".json");
        WriteString(stream, "[" + Environment.NewLine);
        return stream;
      }
      return File.Create(filePathWithoutExtension + ".txt");
    }

    private void CloseExportStream(Stream stream, ExportType exportType)
    {
      if (exportType == ExportType.Json)
      {
        WriteString(stream, Environment.NewLine + "]");
      }
    }

    private void WriteItem(MultiStream multiStream, IDContainer idContainer, bool first = false)
    {
      foreach (var streamType in multiStream.StreamTypes)
      {
        var output = new StringBuilder();
        if (streamType.ExportType == ExportType.TextList)
        {
          output.AppendFormat("{0,4}: {1,-65}", idContainer.Index, idContainer.UniqueName);
          if (idContainer is ItemContainer itemContainer && itemContainer.LocalizedNames != null)
          {
            var englishNames = itemContainer.LocalizedNames.Where(x => x.Key == "EN-US");
            if (englishNames.Any())
            {
              output.AppendFormat(": {0}", englishNames.First().Value);
            }
          }
          output.AppendLine();
        }
        else if (streamType.ExportType == ExportType.Json)
        {
          if (!first)
          {
            output.AppendLine(",");
          }
          output.Append(JsonConvert.SerializeObject(idContainer, Newtonsoft.Json.Formatting.Indented));
        }
        WriteString(streamType, output.ToString());
        output.Clear();
      }
    }

    private void WriteString(StreamType stream, string val)
    {
      var buffer = Encoding.UTF8.GetBytes(val);
      stream.Stream.Write(buffer, 0, buffer.Length);
    }

    private void WriteString(Stream stream, string val)
    {
      var buffer = Encoding.UTF8.GetBytes(val);
      stream.Write(buffer, 0, buffer.Length);
    }
  }
}
