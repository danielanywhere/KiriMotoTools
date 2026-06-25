# Example Command Lines

Following are some example command lines.

<p>&nbsp;</p>

## General Purpose Commands

Add an Auto-Pocket Operation to Kiri Project

```
kirimototools /wait /action:autopocket "/tool:ball 2mm"
  /workingpath:C:\Files
  /infile:KiriMotoMeshCAMProject.kmz
  /outfile:KiriMotoMeshCAMProject.kmz
```

Create a Blank CAM Project

```
kirimototools /wait /action:createcamproject
  /infile:C:\Temp\OpenSCAD\MyPart.stl
  /outfile:C:\Temp\KiriProjects\KiriMotoMeshCAMProject.kmz
```

Run a List of Tasks

```
kirimototools /wait /action:batch
  /workingpath:C:\Temp
  /configfile:KiriProjects\KiriMotoToolsConfig-MyPart.json
```

... using a configuration file similar to the following.

```
{
 "Remark": "KiriMotoTools configuration file for My Part.",
 "InputFilename": "OpenSCAD/MyPart.stl",
 "OutputFilename": "KiriProjects/KiriMotoMeshCAMProject.kmz",
 "Action": "TaskList",
 "Actions":
 [
  {
   "Action": "CreateCAMProject"
  },
  {
   "Action": "AutoPocket",
   "Tool": "end 2mm"
  },
  {
   "Action": "AutoPocket",
   "Tool": "ball 1mm"
  },
  {
   "Action": "Outline",
   "Tool": "end 4mm"
  }
 ]
}
```
