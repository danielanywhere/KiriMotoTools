:: CreateBuildingAndRunning.cmd
:: Create the BuildingAndRunningKiriMotoTools.md file from Docs/BuildingAndRunningKiriMotoTools.odt.
:: This command is meant to be run from within the Scripts folder.
SET FAR=C:\Files\Dropbox\Develop\Shared\FindAndReplace\Source\FindAndReplace\bin\Debug\net6.0\FindAndReplace.exe
SET SOURCE=..\Docs\BuildingAndRunningKiriMotoTools.odt
SET TARGET=..\Docs\BuildingAndRunningKiriMotoTools.md
SET PATTERN=ReadmePostProcessing.json

:: When the image has a URL assigned it isn't placed in the output. Use 'Image' or 'Banner' blocks.
PANDOC -t markdown_strict --embed-resources=false --wrap=none "%SOURCE%" -o "%TARGET%"
"%FAR%" /wait "/files:%TARGET%" "/patternfile:%PATTERN%"
