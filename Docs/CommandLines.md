# Example Command Lines

Following are some example command lines.

<p>&nbsp;</p>

## General Purpose Commands

**Add an Auto-Pocket Operation to Kiri Project**

```
kirimototools /wait /action:autopocket "/tool:ball 2mm"
  /workingpath:C:\Files
  /infile:KiriMotoMeshCAMProject.kmz
  /outfile:KiriMotoMeshCAMProject.kmz
```

<p>&nbsp;</p>

**Create a Blank CAM Project**

```
kirimototools /wait /action:createcamproject
  /infile:C:\Temp\OpenSCAD\MyPart.stl
  /outfile:C:\Temp\KiriProjects\KiriMotoMeshCAMProject.kmz
```

<p>&nbsp;</p>

**Run a List of Tasks**

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

<p>&nbsp;</p>

**Use the Same Configuration File for Multiple Tasks**

Command line 1.

```
kirimototools /wait /workingpath:C:\Temp
  /configfile:KiriConfig-MyPart.json
  /infile:Drawings\OpenSCAD\MyPart-SideA.stl
  /outfile:MyPart-SideA.kmz
```

Command line 2.

```
kirimototools /wait /workingpath:C:\Temp
  /configfile:KiriConfig-MyPart.json
  /infile:Drawings\OpenSCAD\MyPart-SideB.stl
  /outfile:MyPart-SideB.kmz
```

Configuration file.

```
{
 "Remarks":
 [
  "KiriMotoTools configuration file.",
  "Input and output filenames are specified at the command-line..."
 ],
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
