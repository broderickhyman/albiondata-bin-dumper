# THIS REPO IS ARCHIVED! THE ACTIVE REPO CAN BE FOUND [HERE](https://github.com/ao-data/albiondata-bin-dumper)! (ao-data/albiondata-bin-dumper)
<br/><br/><br/><br/><br/><br/><br/><br/>


# albiondata-bin-dumper
This tool allows you to dump the info in the Albion Online ".bin" Files, convert them to XML/JSON files. It also extract the ID's and Names of Items and Locations.

## Requirements
* Visual Studio 2019 (to build)

## How to use:
```
Usage: .\CommandLine.exe [options]

Options:
  -t|--export-type <EXPORT_TYPE>                Export Type
                                                Allowed values are: TextList, Json, Both
  -m|--export-mode <EXPORT_MODE>                Export Mode
                                                Allowed values are: ItemExtraction, LocationExtraction, DumpAllXML, Everything
  -d|--main-game-folder <MAIN_GAME_FOLDER>      Game Folder
  -o|--output-folder-path <OUTPUT_FOLDER_PATH>  Output Folder
  -?|-h|--help                                  Show help information
```
