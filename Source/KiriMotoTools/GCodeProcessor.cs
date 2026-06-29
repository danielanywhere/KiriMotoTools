/*
 * Copyright (c). 2026 Daniel Patterson, MCSD (danielanywhere).
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
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using ActionEngine;
using Clipper2Lib;

namespace KiriMotoTools
{
	//*-------------------------------------------------------------------------*
	//*	GCodeProcessor																													*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// G-Code processing functionality for KiriMotoTools.
	/// </summary>
	public class GCodeProcessor
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		/// <summary>
		/// Allowed whitespace characters on a line.
		/// </summary>
		private static readonly char[] mWhitespace = new char[] { '\t', ' ' };

		//*-----------------------------------------------------------------------*
		//* ApplyCursor																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Apply the contents of the current cursor and clear the parameters.
		/// </summary>
		/// <param name="cursor">
		/// Reference to the cursor to be processed and reset.
		/// </param>
		private void ApplyCursor(GCodeCursor cursor)
		{

			if(cursor != null)
			{
				if(cursor.Action != null)
				{
					switch(cursor.Action.ActionType)
					{
						case GCodeActionType.GCode:
							switch(cursor.Action.ActionIndex)
							{
								case 0:
									//	Rapid position.
									cursor.PenDown = false;
									cursor.Capture();
									break;
								case 1:
									//	Linear interpolation.
									cursor.PenDown = true;
									cursor.Capture();
									break;
								case 2:
									//	Circular interpolation, clockwise.
									//	TODO: Support circular interpolation.
									break;
								case 3:
									//	Circular interpolation, counterclockwise.
									//	TODO: Support circular interpolation.
									break;
								case 4:
								//	Dwell.
								case 5:
								//	High speed control.
								//	Cubic spline interpolation.
								case 17:
								//	XY plane selection.
								case 18:
								//	XZ plane selection.
								case 19:
								//	YZ plane selection.
								case 20:
								//	Activate inches.
								case 21:
								//	Activate mm.
								case 28:
								//	Return to machine home.
								case 30:
								//	Return to secondary reference point.
								case 40:
								//	Tool radius compensation off.
								case 41:
								//	Cutter compensation left.
								case 42:
								//	Cutter compensation right.
								case 43:
								//	Tool length compensation positive.
								case 49:
								//	Tool length compensation cancel.
								case 53:
								//	Machine coordinate system.
								case 54:
								//	Work coordinate system 1.
								case 55:
								//	Work coordinate system 2.
								case 56:
								//	Work coordinate system 3.
								case 57:
								//	Work coordinate system 4.
								case 58:
								//	Work coordinate system 5.
								case 59:
								//	Work coordinate system 6.
								case 80:
								//	Canned cycle cancel.
								case 81:
								//	Standard drilling cycle.
								case 82:
								//	Drilling cycle with dwell.
								case 83:
								//	Peck drilling cycle.
								case 84:
									//	Tapping cycle.
									cursor.Action = null;
									break;
								case 90:
									//	Activate absolute distance mode.
									cursor.IsRelative = false;
									cursor.Action = null;
									break;
								case 91:
									//	Activate incremental (relative) distance mode.
									cursor.IsRelative = true;
									cursor.Action = null;
									break;
								case 93:
								//	Inverse time feed rate.
								case 94:
								//	Feed per minute.
								case 95:
								//	Feed per revolution.
								default:
									cursor.Action = null;
									break;
							}
							break;
						case GCodeActionType.MCode:
							switch(cursor.Action.ActionIndex)
							{
								case 0:
								//	Program stop / mandatory pause.
								case 1:
								//	Optional stop.
								case 2:
								//	Program end.
								case 3:
								//	Spindle on, clockwise.
								case 4:
								//	Spindle on, counterclockwise.
								case 5:
									//	Spindle stop.
									cursor.Action = null;
									break;
								case 6:
									//	Execute tool change.
									if(cursor.Comment.Length > 0)
									{
										cursor.ToolDiameter = KiriMotoToolsUtil.ToFloat(
											ActionEngineUtil.GetValue(
												cursor.Comment, ResourceMain.rxNumeric, "numeric"));
									}
									cursor.Action = null;
									break;
								case 7:
								//	Mist coolant on.
								case 8:
								//	Flood coolant on.
								case 9:
								//	All coolant off.
								case 10:
								//	Chuck / vise clamp.
								case 11:
								//	Chuck / vise unclamp.
								case 19:
								//	Spindle orientation.
								case 30:
								//	End program and rewind.
								case 41:
								//	Low gear.
								case 42:
								//	Low medium gear.
								case 43:
								//	High medium gear.
								case 44:
								//	High gear.
								case 48:
								//	Enable feed / speed overrides.
								case 49:
								//	Disable feed / speed overrides.
								case 60:
								//	Automatic palette change.
								case 97:
								//	Haas internal subroutine call.
								case 98:
								//	Subprogram call.
								case 99:
								//	Subprogram return / loop.
								case 130:
								//	Activate through-spindle coolant.
								case 131:
								//	Deactivate through-spindle coolant.
								default:
									cursor.Action = null;
									break;
							}
							break;
					}
				}
				cursor.Parameters.Clear();
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ReadLines																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Read the individual lines of the caller's stream into the line
		/// collection.
		/// </summary>
		/// <param name="stream">
		/// Reference to the stream to read.
		/// </param>
		/// <param name="lines">
		/// Reference to the collection of lines to fill.
		/// </param>
		private static void ReadLines(Stream stream, List<string> lines)
		{
			string line = "";

			if(stream?.CanRead == true && lines != null)
			{
				using(StreamReader reader = new StreamReader(stream, Encoding.ASCII,
					detectEncodingFromByteOrderMarks:false, bufferSize:1024,
					leaveOpen:true))
				{
					while((line = reader.ReadLine()) != null)
					{
						lines.Add(line);
					}
				}
			}
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Read the individual lines of the caller's stream into the line
		/// collection.
		/// </summary>
		/// <param name="stream">
		/// Reference to the stream to read.
		/// </param>
		/// <param name="lines">
		/// Reference to the collection of lines to fill.
		/// </param>
		private static void ReadLines(TextReader reader, List<string> lines)
		{
			string line = "";

			if(reader != null && lines != null)
			{
				while((line = reader.ReadLine()) != null)
				{
					lines.Add(line);
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*************************************************************************
		//*	Protected																															*
		//*************************************************************************
		//*************************************************************************
		//*	Public																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//* FindRedundantTracks																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Find all redundant tracks in the data.
		/// </summary>
		public void FindRedundantTracks()
		{

		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetRedundantTracks																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a list of g-code vectors that represent fully redundant passes.
		/// </summary>
		/// <returns>
		/// Reference to a list of g-code vector items that represent repeated
		/// passes.
		/// </returns>
		public List<GCodeVectorItem> GetRedundantTracks()
		{
			bool bFound = false;
			int count = 0;
			double finishArea = 0d;
			//int focusCount = 0;
			//int focusIndex = 0;
			//GCodeVectorItem focusVector = null;
			int index = 0;
			Paths64 intersection = null;
			double intersectionArea = 0d;
			int localCount = 0;
			//List<float> levels = null;
			Paths64 minkowski = null;
			List<Paths64> minkowskies = new List<Paths64>();
			int percentage = 0;
			List<GCodeVectorItem> result = new List<GCodeVectorItem>();
			int testCount = 0;
			int testIndex = 0;
			List<GCodeToolShapeItem> testShapes = null;
			GCodeVectorItem testVector = null;
			int toolCount = 0;
			int toolIndex = 0;
			float toolDiameter = 0f;
			float toolDiameterLast = 0f;
			List<float> toolDiameters = new List<float>();
			GCodeToolShapeItem toolShape = null;
			List<GCodeToolShapeItem> toolShapes = new List<GCodeToolShapeItem>();
			GCodeVectorItem vector = null;
			List<GCodeVectorItem> vectors = null;

			//levels = ZLevels.Select(z => z.Level).ToList();
			foreach(GCodeLevelItem levelItem in mZLevels)
			{
				Trace.WriteLine(
					$"Get redundant tracks at level: {levelItem.Level:0.###}",
					$"{MessageImportanceEnum.Info}");
				toolShapes.Clear();
				localCount = 0;
				vectors = levelItem.Vectors;
				count = vectors.Count;
				toolDiameters.Clear();
				toolDiameter = 0f;
				toolDiameterLast = 0f;
				for(index = 0; index < count; index ++)
				{
					vector = vectors[index];
					toolDiameter = vector.ToolDiameter;
					if(toolDiameter != toolDiameterLast)
					{
						if(!toolDiameters.Contains(toolDiameter))
						{
							toolDiameters.Add(toolDiameter);
						}
					}
					toolDiameterLast = toolDiameter;
				}
				foreach(float toolDiameterItem in toolDiameters)
				{
					toolShape = new GCodeToolShapeItem()
					{
						ToolDiameter = toolDiameterItem
					};
					toolShape.StartingIndex =
						vectors.FindIndex(t => t.ToolDiameter == toolDiameterItem);
					toolShape.EndingIndex =
						vectors.FindLastIndex(t => t.ToolDiameter == toolDiameterItem);
					toolShapes.Add(toolShape);
				}
				minkowskies.Clear();
				foreach(GCodeToolShapeItem toolShapeItem in toolShapes)
				{
					for(index = toolShapeItem.StartingIndex;
						index <= toolShapeItem.EndingIndex; index ++)
					{
						vector = vectors[index];
						if(vector.Minkowski != null)
						{
							//	If this vector has a Minkowski, it has an active segment
							//	in the current tool.
							minkowskies.Add(vector.Minkowski);
						}
						else
						{
							//	When this item doesn't have a Minkowski, it is not
							//	a member of a segment. Any existing shape is finished.
							if(minkowskies.Count > 0)
							{
								minkowski = new Paths64();
								foreach(Paths64 shapeItem in minkowskies)
								{
									minkowski.AddRange(shapeItem);
								}
								toolShapeItem.Minkowskies.Add(minkowski);
							}
							minkowskies.Clear();
						}
					}
					if(minkowskies.Count > 0)
					{
						minkowski = new Paths64();
						foreach(Paths64 shapeItem in minkowskies)
						{
							minkowski.AddRange(shapeItem);
						}
						toolShapeItem.Minkowskies.Add(minkowski);
					}
					minkowskies.Clear();
				}
				foreach(GCodeToolShapeItem toolShapeItem in toolShapes)
				{
					foreach(Paths64 minkowskiItem in toolShapeItem.Minkowskies)
					{
						minkowski = Clipper.Union(minkowskiItem, FillRule.NonZero);
						minkowski = Clipper.SimplifyPaths(minkowski, 0.001d);
						toolShapeItem.Shapes.Add(minkowski);
					}
				}
				//	TODO: Place each track in each of the pools not participating
				//	in that tool's diameter.
				toolCount = toolShapes.Count;
				for(toolIndex = toolCount - 1; toolIndex > 0; toolIndex --)
				{
					toolShape = toolShapes[toolIndex];
					testShapes = toolShapes.Skip(0).Take(toolIndex).ToList();
					for(index = toolShape.EndingIndex;
						index >= toolShape.StartingIndex; index --)
					{
						vector = vectors[index];
						bFound = false;
						if(vector.Minkowski != null)
						{
							foreach(GCodeToolShapeItem shapeItem in testShapes)
							{
								foreach(Paths64 minkowskiItem in shapeItem.Shapes)
								{
									intersection = Clipper.Intersect(
										vector.Minkowski, minkowskiItem, FillRule.NonZero);
									intersectionArea = Clipper.Area(intersection);
									finishArea = Clipper.Area(vector.Minkowski);
									if(Math.Abs(finishArea - intersectionArea) < 0.01d)
									{
										result.Add(vector);
										localCount++;
										bFound = true;
										break;
									}
								}
								if(bFound)
								{
									break;
								}
							}
						}
					}
				}
				percentage = (int)(((float)localCount / (float)count) * 100f);
				Trace.WriteLine($"      {localCount} of {count} ({percentage}%)");
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Initialize																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Initialize the processor for the current content loaded in the Lines
		/// collection.
		/// </summary>
		public void Initialize()
		{
			bool bSame = false;
			string code = "";
			int count = 0;
			GCodeCursor cursor = new GCodeCursor();
			GCodeLineItem gline = null;
			int index = 0;
			float lastZ = 0f;
			string line = "";
			int lineIndex = 0;
			List<float> levels = new List<float>();
			Match match = null;
			MatchCollection matches = null;
			string number = "";
			string tu = "";
			List<GCodeVectorItem> vectors = null;

			mZLevels.Clear();
			foreach(string lineItem in mLines)
			{
				cursor.Line = new GCodeLineItem()
				{
					LineIndex = lineIndex,
					Value = lineItem
				};
				tu = lineItem.ToUpper();
				//	Process and remove comment.
				cursor.Comment = "";
				match = Regex.Match(lineItem, @";(?<comment>.*)");
				if(match.Success)
				{
					cursor.Comment = ActionEngineUtil.GetValue(match, "comment").Trim();
					if(match.Index > 0)
					{
						line = tu.Substring(0, match.Index);
					}
					else
					{
						line = "";
					}
				}
				else
				{
					line = tu;
				}

				matches = Regex.Matches(line, ResourceMain.rxGCodeLine);
				foreach(Match matchItem in matches)
				{
					code = ActionEngineUtil.GetValue(matchItem, "code");
					number = ActionEngineUtil.GetValue(matchItem, "value");
					switch(code)
					{
						case "G":
						case "M":
							//	New action.
							if(cursor.Parameters.Count > 0)
							{
								ApplyCursor(cursor);
							}
							cursor.Action = new GCodeActionItem()
							{
								Line = cursor.Line,
								ActionType = GCodeActionItem.GetActionType(code),
								ActionIndex = ActionEngineUtil.ToInt(number)
							};
							//cursor.ActionName = code;
							//cursor.ActionIndex = ActionEngineUtil.ToInt(number);
							cursor.Parameters.Clear();
							break;
						default:
							//	New parameter.
							if(cursor.Parameters.Exists(x => x.Name == code))
							{
								//	If a parameter is being repeated, it is because we are
								//	starting on the next implicit command.
								ApplyCursor(cursor);
							}
							if(cursor.Action != null)
							{
								//	If an action is active, then add the parameter.
								cursor.Parameters.Add(new GCodeParameterItem()
								{
									Name = code,
									Value = KiriMotoToolsUtil.ToFloat(number)
								});
							}
							break;
					}
				}
				if(cursor.Action != null)
				{
					ApplyCursor(cursor);
				}
				lineIndex++;
			}
			//	Separate the definitive levels.
			lastZ = 0f;
			foreach(GCodeVectorItem vectorItem in cursor.Vectors)
			{
				if(vectorItem.PenDown && vectorItem.Vertex.Z == lastZ && !bSame)
				{
					if(!levels.Contains(lastZ))
					{
						levels.Add(lastZ);
					}
					bSame = true;
				}
				else
				{
					bSame = false;
				}
				lastZ = vectorItem.Vertex.Z;
			}
			//	Disconnect diagonal segments.

			foreach(float levelItem in levels)
			{
				vectors = cursor.Vectors.FindAll(z => z.Vertex.Z == levelItem);
				foreach(GCodeVectorItem vectorItem in vectors)
				{
					ZLevels.Add(vectorItem.Action,
						vectorItem.Vertex, vectorItem.ToolDiameter, vectorItem.PenDown);
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Lines																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Lines">Lines</see>.
		/// </summary>
		private List<string> mLines = new List<string>();
		/// <summary>
		/// Get a reference to the list of raw g-code lines currently loaded.
		/// </summary>
		public List<string> Lines
		{
			get { return mLines; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Parse																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Parse the g-code from the caller's stream.
		/// </summary>
		public static GCodeProcessor Parse(Stream inputStream)
		{
			GCodeProcessor result = new GCodeProcessor();

			Trace.WriteLine("Reading g-code file.",
				$"{MessageImportanceEnum.Info}");
			ReadLines(inputStream, result.Lines);
			Trace.WriteLine("Initializing tool paths.",
				$"{MessageImportanceEnum.Info}");
			result.Initialize();
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		public static GCodeProcessor Parse(string content)
		{
			GCodeProcessor result = new GCodeProcessor();

			if(content?.Length > 0)
			{
				using(StringReader reader = new StringReader(content))
				{
					ReadLines(reader, result.Lines);
					result.Initialize();
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ZLevels																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="ZLevels">ZLevels</see>.
		/// </summary>
		private GCodeLevelCollection mZLevels = new GCodeLevelCollection();
		/// <summary>
		/// Get a reference to the list of Z levels in this code.
		/// </summary>
		public GCodeLevelCollection ZLevels
		{
			get { return mZLevels; }
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
