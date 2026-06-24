/*
 * Copyright (c). 2025 Daniel Patterson, MCSD (danielanywhere).
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 * 
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using ActionEngine;
using Geometry;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json.Linq;

using static KiriMotoTools.KiriMotoToolsUtil;

namespace KiriMotoTools
{
	//*-------------------------------------------------------------------------*
	//*	KiriMotoActionCollection																								*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of KiriMotoActionItem Items.
	/// </summary>
	public class KiriMotoActionCollection :
		ActionCollectionBase<KiriMotoActionItem, KiriMotoActionCollection>
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*************************************************************************
		//*	Protected																															*
		//*************************************************************************
		//*************************************************************************
		//*	Public																																*
		//*************************************************************************


	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	KiriMotoActionItem																											*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about an individual Kiri:Moto action.
	/// </summary>
	public class KiriMotoActionItem :
		ActionItemBase<KiriMotoActionItem, KiriMotoActionCollection>
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//* AppendAutoPocket																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Append the auto-pocket operation details to the loaded workspace
		/// structure.
		/// </summary>
		/// <param name="json">
		/// Reference to the workspace.json structure to which the new pocket
		/// operation will be appended.
		/// </param>
		/// <param name="surfaceName">
		/// Name of the surface to record.
		/// </param>
		/// <param name="toolName">
		/// Name of the tool to assign.
		/// </param>
		/// <param name="selectedFaceIndices">
		/// Reference to the collection of selected face indices.
		/// </param>
		private static void AppendAutoPocket(dynamic json,
			string surfaceName, string toolName, List<int> selectedFaceIndices)
		{
			dynamic newOp = null;
			dynamic surfaces = null;

			if(json != null && selectedFaceIndices != null)
			{
				newOp = new JObject();
				newOp.type = "pocket";
				newOp.direction = "climb";
				newOp.spindle = 10000;
				newOp.tool = GetToolId(json, toolName);
				newOp.step = 0.25f;
				newOp.down = 0.25f;
				newOp.rate = 1100;
				newOp.plunge = 200;
				newOp.expand = 0;
				newOp.smooth = 3;
				newOp.refine = 20;
				newOp.follow = 5;
				newOp.contour = false;
				newOp.outline = false;
				newOp.ov_topz = 0f;
				newOp.ov_botz = 0f;
				newOp.tolerance = 0f;
				surfaces = new JObject();
				newOp.surfaces = surfaces;
				surfaces[surfaceName] =
					JArray.FromObject(selectedFaceIndices.ToArray());

				((JArray)json.settings.process.ops).Insert(
					json.settings.process.ops.Count - 1, newOp);

				((JArray)json.settings.sproc.CAM["default"].ops).Insert(
					json.settings.sproc.CAM["default"].ops.Count - 1, newOp);

			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* AutoPocket	                                                          *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Auto-pocket the shape.
		/// </summary>
		/// <param name="item">
		/// Reference to the action item containing processing information.
		/// </param>
		private void AutoPocket(KiriMotoActionItem item)
		{
			int count = 0;
			FaceItem face = null;
			FaceCollection faces = new FaceCollection();
			dynamic geo = null;
			int index = 0;
			string inputFilename = "";
			dynamic json = null;
			List<int> selectedFaces = null;
			string surfaceName = "";
			FVector3 vector = null;

			if(item?.WorkingJson != null)
			{
				json = item.WorkingJson;
				surfaceName = json.work[0].id;
				geo = json.work[0].geo;
				count = geo.Count;
				for(index = 0; index + 8 < count; index += 9)
				{
					face = new FaceItem();
					face.Vectors[0] = new FVector3()
					{
						X = (float)geo[index],
						Y = (float)geo[index + 1],
						Z = (float)geo[index + 2]
					};
					face.Vectors[1] = new FVector3()
					{
						X = (float)geo[index + 3],
						Y = (float)geo[index + 4],
						Z = (float)geo[index + 5]
					};
					face.Vectors[2] = new FVector3()
					{
						X = (float)geo[index + 6],
						Y = (float)geo[index + 7],
						Z = (float)geo[index + 8]
					};
					faces.Add(face);
				}
				//	Select upward faces and eliminate all faces behind.
				Trace.WriteLine($"Faces read: {faces.Count}",
					$"{MessageImportanceEnum.Info}");
				count = FaceCollection.SelectUpFacing(faces);
				Trace.WriteLine($"Upward faces selected: {count}",
					$"{MessageImportanceEnum.Info}");
				//	Deselect obstructed faces.
				Trace.WriteLine("     -> Deselecting obstructed faces...");
				count = FaceCollection.DeselectObstructed(faces);
				Trace.WriteLine($"Faces behind deselected: {count}",
					$"{MessageImportanceEnum.Info}");
				//	Deselect surface faces.
				Trace.WriteLine("     -> Deselecting surface faces...");
				count = FaceCollection.DeselectSurface(faces,
					(float)json.work[0].track.top);
				Trace.WriteLine($"Surface faces deselected: {count}",
					$"{MessageImportanceEnum.Info}");
				selectedFaces = FaceCollection.GetSelectedIndices(faces);
				//	Create an operation.
				AppendAutoPocket(json, surfaceName, item.Tool, selectedFaces);
				if(item.WorkingDocument != null &&
					item.WorkingDocument is KiriMotoZipDocumentItem kiriMotoDocument)
				{
					kiriMotoDocument.Modified = true;
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* CreateCAMProject																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Create a KiriMoto CAM project.
		/// </summary>
		/// <param name="item">
		/// Reference to the active action item.
		/// </param>
		private void CreateCAMProject(KiriMotoActionItem item)
		{
			KiriMotoZipDocumentItem kiriMotoDocument = null;

			if(item != null && item.OutputFilename?.Length > 0)
			{
				kiriMotoDocument = new KiriMotoZipDocumentItem()
				{
					Name = ActionEngineUtil.AbsolutePath(
						item.WorkingPath, item.OutputFilename)
				};
				kiriMotoDocument.Json = JObject.Parse(ResourceMain.BlankWorkspace);
				kiriMotoDocument.Modified = true;
				SetRootWorkingDocument(item, kiriMotoDocument);
				Trace.WriteLine("New CAM project created.",
					$"{MessageImportanceEnum.Info}");
				if(CheckElements(item, ActionElementEnum.InputFilename) &&
					item.InputNames[0].ToLower().EndsWith(".stl"))
				{
					CreateWorkRecord(item);
				}
				else
				{
					item.Break = true;
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* CreateWorkRecord																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Create the work record for the 
		/// </summary>
		/// <param name="item">
		/// Reference to the item for which the work record is being built.
		/// </param>
		private static void CreateWorkRecord(KiriMotoActionItem item)
		{
			float[] adjustedCoordinates = null;
			float cdx = 0f;
			float cdy = 0f;
			float cdz = 0f;
			string content = "";
			int count = 0;
			float depth = 0f;
			string filename = "";
			float height = 0f;
			string id = "";
			int index = 0;
			dynamic widget = null;
			float width = 0f;
			dynamic work = null;
			List<FVector3> vertices = new List<FVector3>();
			float x = 0f;
			float xMax = 0f;
			float xMin = 0f;
			float y = 0f;
			float yMax = 0f;
			float yMin = 0f;
			float z = 0f;
			float zMax = 0f;
			float zMin = 0f;

			if(item != null &&
				CheckElements(item, ActionElementEnum.InputFilename) &&
					item.InputNames[0].ToLower().EndsWith(".stl"))
			{
				filename = item.InputNames[0];
				id = $"m{Guid.NewGuid().ToString("N").Substring(24)}";
				try
				{
					Trace.WriteLine("Reading STL file...",
						$"{MessageImportanceEnum.Info}");
					using(FileStream fs = File.OpenRead(ActionEngineUtil.AbsolutePath(
							item.WorkingPath, filename)))
					{
						vertices = StlReader.ReadStl(fs);
					}

					xMax = vertices.Max(x => x.X);
					xMin = vertices.Min(x => x.X);
					yMax = vertices.Max(y => y.Y);
					yMin = vertices.Min(y => y.Y);
					zMax = vertices.Max(z => z.Z);
					zMin = vertices.Min(z => z.Z);

					width = xMax - xMin;
					depth = yMax - yMin;
					height = zMax - zMin;
					cdx = (xMin + xMax) / 2f;
					cdy = (yMin + yMax) / 2f;
					cdz = zMin;   //	This version of Kiri Moto uses offset from bottom.

					count = vertices.Count * 3;
					index = 0;
					adjustedCoordinates = new float[count];
					foreach(FVector3 vertexItem in vertices)
					{
						adjustedCoordinates[index] = vertexItem.X - cdx;
						adjustedCoordinates[index + 1] = vertexItem.Y - cdy;
						adjustedCoordinates[index + 2] = vertexItem.Z - cdz;
						index += 3;
					}

					work = new JObject();
					work.type = 100;
					work.id = id;
					work.ver = 1;
					work.json = true;
					work.group = id;
					work.track = new JObject();
					work.track.box = new JObject();
					work.track.box.w = width;   //	width of the object (X).
					work.track.box.h = depth;   // depth of the object (Y).
					work.track.box.d = height;  // height of the object (Z).
					work.track.scale = new JObject();
					work.track.scale.x = 1f;
					work.track.scale.y = 1f;
					work.track.scale.z = 1f;
					work.track.rot = new JObject();
					work.track.rot.x = 0f;
					work.track.rot.y = 0f;
					work.track.rot.z = 0f;
					work.track.pos = new JObject();
					work.track.pos.x = 0f;
					work.track.pos.y = 0f;
					work.track.pos.z = 0f;
					work.track.top = height; // topmost Z.
					work.track.mirror = false;
					work.track.indexed = false;
					work.track.indexRad = 0f;
					work.track.center = new JObject();
					work.track.center.dx = cdx; // X center of original object.
					work.track.center.dy = cdy; // Y center of original object.
					work.track.center.dz = cdz; // Z bottom of original object.
					work.track.tzoff = 0f;
					work.geo = JArray.FromObject(adjustedCoordinates);
					work.meta = new JObject();
					work.meta.url = null;
					work.meta.file = Path.GetFileName(filename);
					work.meta.saved = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
					work.meta.vertices = vertices.Count;
					work.meta.save = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
					work.anno = new JObject();
					work.anno.extruder = 0;
					work.anno.file = Path.GetFileName(filename);
					work.anno.url = null;
					work.anno.tab = new JArray();

					item.WorkingJson.work = new JArray();
					item.WorkingJson.work.Add(work);

					widget = new JObject();
					widget[id] = new JObject();
					widget[id].extruder = 0;
					widget[id].file = filename;
					widget[id].url = null;

					item.WorkingJson.settings.widget = widget;
				}
				catch(Exception ex)
				{
					Trace.WriteLine("Error reading object file " +
						$"{Path.GetFileName(Path.GetFileName(filename))}: {ex.Message}",
						$"{MessageImportanceEnum.Err}");
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetToolId																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the Tool ID of the named tool.
		/// </summary>
		/// <param name="json">
		/// Reference to the workspace.json structure in which the tool definitions
		/// will be enumerated.
		/// </param>
		/// <param name="toolName">
		/// The name of the tool to look up.
		/// </param>
		/// <returns>
		/// The ID of the specified tool, if found. Otherwise, the default value
		/// of 1000.
		/// </returns>
		private static long GetToolId(dynamic json, string toolName)
		{
			long result = 1000;
			string tl = "";

			if(json != null && toolName?.Length > 0)
			{
				tl = toolName.ToLower();
				foreach(dynamic toolItem in json.settings.tools)
				{
					if(toolItem.name.ToString().ToLower() == tl)
					{
						result = toolItem.id;
						break;
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ListActions																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Run the actions in the child list.
		/// </summary>
		/// <param name="item">
		/// Reference to the action item containing sub-actions to run.
		/// </param>
		private async Task ListActions(KiriMotoActionItem item)
		{
			if(item != null)
			{
				foreach(KiriMotoActionItem actionItem in item.Actions)
				{
					await actionItem.Run();
					if(actionItem.Break)
					{
						Trace.WriteLine("Break from list...",
							$"{MessageImportanceEnum.Info}");
						break;
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Outline																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Add an outline operation to the current workspace structure.
		/// </summary>
		/// <param name="item">
		/// Reference to the action item containing processing information.
		/// </param>
		private void Outline(KiriMotoActionItem item)
		{
			dynamic json = null;
			dynamic newOp = null;
			dynamic surfaces = null;

			if(item != null)
			{
				if(item.WorkingJson != null)
				{
					//	Working JSON is active.
					json = item.WorkingJson;
					newOp = new JObject();
					newOp.type = "outline";
					newOp.tool = GetToolId(json, item.Tool);
					newOp.direction = "climb";
					newOp.spindle = 10000;
					newOp.step = 0.4f;
					newOp.steps = 1;
					newOp.down = 0.5f;
					newOp.rate = 1100;
					newOp.plunge = 200;
					newOp.dogbones = false;
					newOp.revbones = false;
					newOp.omitthru = false;
					newOp.omitvoid = false;
					newOp.outside = false;
					newOp.inside = false;
					newOp.wide = false;
					newOp.ov_topz = 0f;
					newOp.ov_botz = 0f;

					((JArray)json.settings.process.ops).Insert(
						json.settings.process.ops.Count - 1, newOp);

					((JArray)json.settings.sproc.CAM["default"].ops).Insert(
						json.settings.sproc.CAM["default"].ops.Count - 1, newOp);

				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* SetRootWorkingDocument																								*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Set the instance of the root working document.
		/// </summary>
		/// <param name="item">
		/// Reference to the active action item.
		/// </param>
		/// <param name="document">
		/// Reference to the document to be made reference.
		/// </param>
		private static void SetRootWorkingDocument(KiriMotoActionItem item,
			KiriMotoZipDocumentItem document)
		{
			if(item != null && document != null)
			{
				if(item != null)
				{
					if(item.Parent?.Parent != null)
					{
						item.WorkingDocument = null;
						item.WorkingJson = null;
						SetRootWorkingDocument(item.Parent.Parent, document);
					}
					else
					{
						//	The root was found.
						item.WorkingDocument = document;
						item.WorkingJson = document.Json;
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*************************************************************************
		//*	Protected																															*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//* RunCustomAction                                                       *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Run your custom actions by overriding this method.
		/// </summary>
		protected async override void RunCustomAction()
		{
			string action = Action.ToLower();

			switch(action)
			{
				case "autopocket":
					AutoPocket(this);
					break;
				case "createcamproject":
					CreateCAMProject(this);
					break;
				case "outline":
					Outline(this);
					break;
				case "tasklist":
					await ListActions(this);
					break;
			}
		}
		//*-----------------------------------------------------------------------*

		//*************************************************************************
		//*	Public																																*
		//*************************************************************************

		//*-----------------------------------------------------------------------*
		//*	Break																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Break">Break</see>.
		/// </summary>
		private bool mBreak = false;
		/// <summary>
		/// Get/Set a value indicating whether to break from the local loop.
		/// </summary>
		/// <remarks>
		/// This property is not inheritable.
		/// </remarks>
		public bool Break
		{
			get { return mBreak; }
			set { mBreak = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Tool																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Tool">Tool</see>.
		/// </summary>
		private string mTool = "";
		/// <summary>
		/// Get/Set the name of the tool to use for the current operation.
		/// </summary>
		public string Tool
		{
			get
			{
				string result = mTool;

				if(string.IsNullOrEmpty(result))
				{
					if(Parent?.Parent != null)
					{
						result = Parent.Parent.Tool;
					}
					else
					{
						result = "";
					}
				}
				return result;
			}
			set
			{
				mTool = value;
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* WorkingJson																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="WorkingJson">WorkingJson</see>.
		/// </summary>
		private dynamic mWorkingJson = null;
		/// <summary>
		/// Get/Set a reference to the working JSON for this session level. This
		/// value is inheritable.
		/// </summary>
		public dynamic WorkingJson
		{
			get
			{
				dynamic result = mWorkingJson;

				if(result == null)
				{
					if(Parent?.Parent != null)
					{
						result = Parent.Parent.WorkingJson;
					}
					else
					{
						result = null;
					}
				}
				return result;
			}
			set
			{
				mWorkingJson = value;
			}
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
