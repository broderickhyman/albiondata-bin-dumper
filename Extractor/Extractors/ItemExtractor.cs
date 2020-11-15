using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Extractor.Extractors
{
  public class ItemExtractor : BaseExtractor
  {
    public ItemExtractor(string mainGameFolder, string outputFolderPath, ExportMode exportMode, ExportType exportType) : base(mainGameFolder, outputFolderPath, exportMode, exportType)
    {
    }

    protected override void ExtractFromXML(Stream inputXmlFile, MultiStream outputStream, Action<MultiStream, IDContainer, bool> writeItem, LocalizationData localizationData = default)
    {
      var journals = new List<IDContainer>();
      var xmlDoc = new XmlDocument();
      xmlDoc.Load(inputXmlFile);

      var rootNode = xmlDoc.LastChild;

      var index = 1;
      var first = true;
      foreach (XmlNode node in rootNode.ChildNodes)
      {
        if (node.NodeType == XmlNodeType.Element && !string.IsNullOrEmpty(node.Attributes["uniquename"]?.Value))
        {
          var uniqueName = node.Attributes["uniquename"].Value;
          var enchantmentLevel = node.Attributes["enchantmentlevel"];
          var description = node.Attributes["descriptionlocatag"];
          var name = node.Attributes["descvariable0"];
          var enchantment = "";
          if (enchantmentLevel != null && enchantmentLevel.Value != "0")
          {
            enchantment = "@" + enchantmentLevel.Value;
          }
          var localizationNameVariable = name != null ? name.Value : LocalizationData.ItemPrefix + uniqueName;
          if (uniqueName.Contains("ARTEFACT"))
          {
            localizationNameVariable = LocalizationData.ItemPrefix + uniqueName;
          }
          var container = new ItemContainer()
          {
            Index = index.ToString(),
            UniqueName = uniqueName + enchantment,
            LocalizationDescriptionVariable = description != null ? description.Value : LocalizationData.ItemPrefix + uniqueName + LocalizationData.DescPostfix,
            LocalizationNameVariable = localizationNameVariable
          };
          SetLocalization(localizationData, container);
          writeItem(outputStream, container, first);
          if (first)
          {
            first = false;
          }
          index++;

          if (node.Name == "journalitem")
          {
            journals.Add(new ItemContainer()
            {
              UniqueName = uniqueName
            });
          }

          var element = FindElement(node, "enchantments");
          if (element != null)
          {
            foreach (XmlElement childNode in element.ChildNodes)
            {
              var enchantmentName = node.Attributes["uniquename"].Value + "@" + childNode.Attributes["enchantmentlevel"].Value;
              container = new ItemContainer()
              {
                Index = index.ToString(),
                UniqueName = enchantmentName,
                LocalizationDescriptionVariable = description != null ? description.Value : LocalizationData.ItemPrefix + uniqueName + LocalizationData.DescPostfix,
                LocalizationNameVariable = name != null ? name.Value : LocalizationData.ItemPrefix + uniqueName
              };
              SetLocalization(localizationData, container);
              writeItem(outputStream, container, false);

              index++;
            }
          }
        }
      }

      foreach (ItemContainer j in journals)
      {
        var container = new ItemContainer()
        {
          Index = index.ToString(),
          UniqueName = j.UniqueName + "_EMPTY",
          LocalizationDescriptionVariable = LocalizationData.ItemPrefix + j.UniqueName + "_EMPTY" + LocalizationData.DescPostfix,
          LocalizationNameVariable = LocalizationData.ItemPrefix + j.UniqueName + "_EMPTY"
        };
        SetLocalization(localizationData, container);
        writeItem(outputStream, container, false);
        index++;
        container = new ItemContainer()
        {
          Index = index.ToString(),
          UniqueName = j.UniqueName + "_FULL",
          LocalizationDescriptionVariable = LocalizationData.ItemPrefix + j.UniqueName + "_FULL" + LocalizationData.DescPostfix,
          LocalizationNameVariable = LocalizationData.ItemPrefix + j.UniqueName + "_FULL"
        };
        SetLocalization(localizationData, container);
        writeItem(outputStream, container, false);
        index++;
      }
    }

    private void SetLocalization(LocalizationData data, ItemContainer item)
    {
      if (data == default) return;
      if (data.LocalizedDescriptions.TryGetValue(item.LocalizationDescriptionVariable, out var descriptions))
      {
        item.LocalizedDescriptions = descriptions;
      }
      if (data.LocalizedNames.TryGetValue(item.LocalizationNameVariable, out var names))
      {
        item.LocalizedNames = names;
      }
    }

    protected override string GetBinFilePath()
    {
      return Path.Combine(mainGameFolder, @".\game\Albion-Online_Data\StreamingAssets\GameData\items.bin");
    }
  }
}
