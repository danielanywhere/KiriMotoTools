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

**Get a Redundancy Report on a G-code File**

```
kirimototools /wait /action:reportgcodeoverlap
  /infile:C:\Temp\OpenSCAD\MyPart.nc
```

<p>&nbsp;</p>

**Optimize a G-code File to Eliminate Wasted Effort**

```
kirimototools /wait /action:optimizegcode
  /infile:C:\Temp\OpenSCAD\MyPart.nc
  /outfile:C:\Temp\OpenSCAD\MyPart-Optimized.nc
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
 "MachineConfig":
 {
  "MaxSpindle": 10000,
  "HeaderCodes":
  [
   "; Safe height: 2mm",
   "G21 ; set units to MM (required)",
   "G90 ; absolute position mode (required)"
  ],
  "ToolChangeCodes":
  [
   "G0 Z0",
   "M5 ; stop the spindle.",
   "M0 ; program stop and wait for operator input.",
   ";M6 T{tool} ; change tool to '{tool_name}'.",
   "G0 Z0.99"
  ]
 },
 "Actions":
 [
  {
   "Action": "CreateCAMProject"
  },
  {
   "Action": "AutoPocket",
   "Tool": "end 2mm",
   "Remark": "The Properties collection is optional.",
   "Properties":
   [
    {
     "Name": "Spindle", "Value": 10000
    },
    {
     "Name": "MillingStyle", "Value": "Conventional"
    },
    {
     "Name": "StepOver", "Value": 0.5
    },
    {
     "Name": "StepDown", "Value": 0.5
    }
   ]
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
