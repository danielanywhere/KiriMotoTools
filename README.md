# KiriMotoTools

<img src="Images/KiriMotoToolsBanner.png" width="100%" alt="KiriMotoTools Page Banner" />

<p>&nbsp;</p>

As one of the few open-source slicing applications that also accommodate full 3D CAM CNC operations on mesh-based designs like STL objects, [**Kiri:Moto**](https://github.com/GridSpace/grid-apps) does the job beautifully. However, the more dense your mesh becomes, the more manual work is required of you to select hundreds or even thousands of faces while working in pocketing and contouring modes.

**KiriMotoTools** is a command-line application that allows you to achieve a near-instantaneous turn-around on design changes, reducing your Kiri:Moto interaction time to just a couple of minutes instead of hours.

<p>&nbsp;</p>

NOTE: This is an early version with some limitations. These will gradually be resolved over the coming days.

-   When creating a CAM project, the stock is automatically 10mm wider, 10mm deeper, and the same height as the mesh object.
-   Tabs are not inserted automatically.
-   Only AutoPocket and Outline CAM operations have been implemented at this point.

<p>&nbsp;</p>

## 🆕G-Code Redundancy Report and Optimization Tasks

Two new actions have been added to help you experience up to a 95% time and energy savings on your Kiri:Moto CAM CNC jobs.

<p>&nbsp;</p>

### The Issue

The procedure of roughing out any intricate area with bits of decreasing size has long been a popular way to eliminate most of the time that would otherwise be spent waiting, sometimes for days, for the smallest selected bit to trace every square unit of the entire volume, not to mention that the single-bit approach could easily wear out multiple bits on a single job.

While the Kiri:Moto CAM CNC mode does offer a roughing action, that particular action is overly eager on clearing surfaces that will be completely removed with outlines.

The CAM CNC mode also easily supports multiple passes using the same Pocket action, and when using this approach with a decreasing list of tool sizes, very few of the tracks from each of the previous actions are counted as fulfilled during the rendering phase, leading to a job that, although legitimate in tool selection, takes several times longer to complete than it should by taking an increasing number of no-touch passes as the job wears on.

To verify whether you have multiple wasted tracks in any g-code file, you can run the /action:ReportGCodeOverlap command as shown in the following example.

```batch
kirimototools /wait /action:reportgcodeoverlap /infile:C:\Temp\MyGCodeFile.nc

```

<p>&nbsp;</p>

### The Workaround

To help you work around this issue of wasted effort, KiriMotoTools now includes a voxel-based volume optimizer that removes any tracks from the file that are purely redundant.

The following tactic is adopted to drastically reduce the number of actions in a typical Kiri:Moto CAM CNC file, or any other variation that tends to output extremely superfluous activities.

-   For each redundant G1 track:
-   Retract the tool to the safe height.
-   Move to the next non-redundant X,Y location. Mark as *Current*.
-   Plunge the tool to the *Current* Z location.

Because a quick fix was imperative, this version doesn't create the cleanest possible track layout, mainly because of the noisy retract-and-plunge actions that are created as a result of clean-up. However, even with the new noise in your job, you will probably experience jobs finishing in 10 to 20 minutes that were taking up to 5 hours time.

Use the /action:OptimizeGCode parameter to create a cleaned-up version of your file as shown in the example below.

```batch
kirimototools /wait /action:optimizegcode
  /infile:C:\Temp\MyGCodeFile.nc
  /outfile:C:\Temp\MyGCodeFile-Optimized.nc


```

<p>&nbsp;</p>

## Auto-Pocket

Pocketing is the CAM process of clearing material inside a closed boundary. Most CAM systems struggle to infer clean pocket boundaries, depths, and feature intent from shape data, and because of this, pocketing and contouring information usually needs to be selected manually. When this type of manual selection feature is applied to triangle-mesh surfaces, you might have already found that hundreds or even thousands of those facets have to be selected manually.

One of the key attributes of KiriMotoTools is that it has eliminated any need to perform human-supervised pocketing on Kiri:Moto. Using the AutoPocket action, you can update your existing Kiri project file of any complexity with a single command similar to the following:

```batch
kirimototools /wait /action:autopocket "/tool:ball 2mm"
  /workingpath:C:\Files
  /infile:KiriMotoMeshCAMProject.kmz
  /outfile:KiriMotoMeshCAMProject.kmz

```

<p>&nbsp;</p>

### Algorithm

The algorithm for this procedure is fairly straightforward.

 - Assume that the tool head is always perfectly vertical above the work (limits this version to 3D machines).

 - Select all faces that are turned even slightly upward.

 - Deselect all faces partially or completely behind any other face.

 - Deselect all faces equal in height or above the top of the stock.

You can take a look at:

[Source/KiriMotoTools/KiriMotoAction.cs](Source/KiriMotoTools/KiriMotoAction.cs) -&gt; KiriMotoActionItem.AutoPocket

for a closer view.

<p>&nbsp;</p>

### Improved Multi-Pass Support

Especially in cases of complex carving operations, it is much faster to define multiple identical pocket operations with each operation using a slightly smaller tool than the one before.

Using the auto-pocket feature, multi-pass operations become easy to add to any project.

<p>&nbsp;</p>

## CNC Project Creation

You can also now create a full project using a basic configuration file that reduces your overall Kiri:Moto steps to a minimal count.

<p>&nbsp;</p>

### Without KiriMotoTools

When you maintain your project directly in Kiri:Moto, you use the following general workflow to create the project.

-   Create new Project.

-   Import STL file.

-   Set stock size.

-   Add pocket.

    -   Set tool.
    -   Select faces.

-   Add outline.

-   Set tool.

-   Click **Slice** to review slicing.

-   Click **Animate** to review activity.

-   Click **Export** to generate the .NC file.

Each time you update your design file, you need to perform the following steps.

-   Delete the STL object.
-   Import the modified STL object.
-   Select faces on every pocket operation.
-   Click **Slice** to review slicing.
-   Click **Animate** to review activity.
-   Click **Export** to generate the .NC file.

<p>&nbsp;</p>

### With KiriMotoTools

When you maintain your project with KiriMotoTools, use the following general workflow to create the project.

-   Create a small configuration file specifying the STL filename, Kiri filename, operations, and properties similar to the following:

```batch
{
 "Remark": "KiriMotoTools configuration file for Linear Bearing Side A.",
 "InputFilename": "Drawings/OpenSCAD/LinearBearingLayer17p932SideA.stl",
 "OutputFilename": "LinearBearingLayer17p932SideA.kmz",
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

-   Run the KiriMotoTools command to create the project with a command similar to the following:

```batch
kirimototools /wait /action:batch
  /workingpath:C:\Files
  /configfile:LinearBearingSideA.json

```

-   Click **Slice** to review slicing.
-   Click **Animate** to review activity.
-   Click **Export** to generate the .NC file.

Each time you update your design file, perform the following steps.

-   Run the KiriMotoTools command to create the project with a command similar to the following:

```batch
kirimototools /wait /action:batch
  /workingpath:C:\Files
  /configfile:LinearBearingSideA.json

```

-   Click **Slice** to review slicing.
-   Click **Animate** to review activity.
-   Click **Export** to generate the .NC file.

<p>&nbsp;</p>

## Summary

Now, you can truly benefit from the potential 20x design speed increase, as well as the availability of hundreds of free editors in different categories, that freestyle mesh editing affords over traditional constraint-based modeling. The long, tedious design hours and limited collaborativity of constraint-based modeling never have to be a restriction to your productivity again.

<p>&nbsp;</p>

## Running KiriMotoTools

This is a .NET managed console application that runs on Linux, macOS, and Windows.

Please read the [**Docs/CommandLineSyntax.txt**](Docs/CommandLineSyntax.txt) file for more information. You can also get some sample ideas from [**Docs/CommandLines.md**](Docs/CommandLines.md).

On any system with .NET Core SDK 8 or greater installed, you can get the project and run it initially using the following commands.

```batch
git clone https://github.com/danielanywhere/KiriMotoTools
cd KiriMotoTools/Source/KiriMotoTools
dotnet run -- /? /wait

```

A full description of compiling and running SvgTools on your PC is also available at [**Docs/BuildingAndRunningKiriMotoTools.md**](Docs/BuildingAndRunningKiriMotoTools.md).
