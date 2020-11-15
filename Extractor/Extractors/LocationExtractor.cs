using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Extractor.Extractors
{
  public class LocationExtractor : BaseExtractor
  {
    public LocationExtractor(string mainGameFolder, string outputFolderPath, ExportMode exportMode, ExportType exportType) : base(mainGameFolder, outputFolderPath, exportMode, exportType)
    {
    }

    protected override void ExtractFromXML(Stream inputXmlFile, MultiStream outputStream, Action<MultiStream, IDContainer, bool> writeItem, LocalizationData localizationData = default)
    {
      var xmlDoc = new XmlDocument();
      xmlDoc.Load(inputXmlFile);

      var rootNode = xmlDoc.LastChild.FirstChild;
      var first = true;
      foreach (XmlNode node in rootNode.ChildNodes)
      {
        if (node.NodeType == XmlNodeType.Element)
        {
          var locID = node.Attributes["id"].Value;
          var locName = node.Attributes["displayname"].Value;

          writeItem(outputStream, new IDContainer()
          {
            Index = locID,
            UniqueName = locName
          }, first);
          if (first)
          {
            first = false;
          }
        }
      }
    }

    protected override string GetBinFilePath()
    {
      return Path.Combine(mainGameFolder, @".\game\Albion-Online_Data\StreamingAssets\GameData\cluster\world.bin");
    }
  }
}
