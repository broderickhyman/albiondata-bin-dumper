using Extractor.Extractors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Extractor
{
  public class LocalizationData
  {
    public const string ItemPrefix = "@ITEMS_";
    public const string DescPostfix = "_DESC";

    public Dictionary<string, Dictionary<string, string>> LocalizedNames = new Dictionary<string, Dictionary<string, string>>();
    public Dictionary<string, Dictionary<string, string>> LocalizedDescriptions = new Dictionary<string, Dictionary<string, string>>();

    public LocalizationData(string mainGameFolder, string outputFolderPath)
    {
      var xmlFileLocation = BaseExtractor.DecryptBinFile(Path.Combine(mainGameFolder, @".\game\Albion-Online_Data\StreamingAssets\GameData\localization.bin"), outputFolderPath);

      var xmlDoc = new XmlDocument();
      using (var inputStream = File.OpenRead(xmlFileLocation))
      {
        xmlDoc.Load(inputStream);

        var rootNode = xmlDoc.LastChild.LastChild;

        foreach (XmlNode node in rootNode.ChildNodes)
        {
          if (node.NodeType == XmlNodeType.Element)
          {
            var tuid = node.Attributes["tuid"];
            if (tuid?.Value.StartsWith(ItemPrefix) == true)
            {
              var languages = node.ChildNodes
                  .Cast<XmlNode>()
                  .ToDictionary(x => x.Attributes["xml:lang"].Value, y => y.LastChild.InnerText);
              // is the item description
              if (tuid.Value.EndsWith(DescPostfix))
              {
                LocalizedDescriptions[tuid.Value] = languages;
              }
              // is item name
              else
              {
                LocalizedNames[tuid.Value] = languages;
              }
            }
          }
        }
      }
    }
  }
}
