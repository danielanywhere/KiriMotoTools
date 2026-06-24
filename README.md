# KiriMotoTools

<img src="Images/KiriMotoToolsBanner.png" width="100%" alt="KiriMotoTools Page Banner" />

<p>&nbsp;</p>

As one of the few open-source slicing applications that also accommodate full 3D CAM CNC operations on mesh-based designs like STL objects, [**Kiri:Moto**](https://github.com/GridSpace/grid-apps) does the job beautifully. However, the more dense your mesh becomes, the more manual work is required of you to select hundreds or even thousands of faces while working in pocketing and contouring modes.

**KiriMotoTools** is a command-line application that allows you to achieve a near-instantaneous turn-around on design changes, reducing your Kiri:Moto interaction time to just a couple of minutes instead of hours.

<p>&nbsp;</p>

## Auto-Pocket

Pocketing is the CAM process of clearing material inside a closed boundary. Most CAM systems struggle to infer clean pocket boundaries, depths, and feature intent from shape data, and because of this, pocketing and contouring information usually needs to be selected manually. When this type of manual selection feature is applied to triangle-mesh surfaces, you might have already found that hundreds or even thousands of those facets have to be selected manually.

One of the key attributes of KiriMotoTools is that it has eliminated any need to perform human-supervised pocketing on Kiri:Moto. Using the AutoPocket action, you can update your existing Kiri project file of any complexity with a single command.

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

-   Create a small configuration file specifying the STL filename, Kiri filename, operations, and properties.
-   Run the KiriMotoTools command to create the project.
-   Click **Slice** to review slicing.
-   Click **Animate** to review activity.
-   Click **Export** to generate the .NC file.

Each time you update your design file, perform the following steps.

-   Run the command to create the project.
-   Click **Slice** to review slicing.
-   Click **Animate** to review activity.
-   Click **Export** to generate the .NC file.

<p>&nbsp;</p>

## Summary

Now, you can truly benefit from the 20x design speed increase, as well as the availability of hundreds of free editors in different categories, that freestyle mesh editing affords over traditional constraint-based modeling. The long, tedious design hours and limited collaborativity of constraint-based modeling never have to be a problem again.

<p>&nbsp;</p>

## Running KiriMotoTools

This is a .NET managed console application that runs on Linux, macOS, and Windows.

More information about installing and running on your platform is coming within the next couple of days.
