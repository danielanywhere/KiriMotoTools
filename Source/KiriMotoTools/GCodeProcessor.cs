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
									//	In this version, tool diameter is set on a
									//	line-by-line basis.
									//if(cursor.Comment.Length > 0)
									//{
									//	cursor.ToolDiameter = KiriMotoToolsUtil.ToFloat(
									//		ActionEngineUtil.GetValue(
									//			cursor.Comment, ResourceMain.rxNumeric, "numeric"));
									//}
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
		//* InterpolatePath																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Interpolate the points in the line between the start and end vertices.
		/// </summary>
		/// <param name="start">
		/// Start vertex.
		/// </param>
		/// <param name="end">
		/// End vertex.
		/// </param>
		/// <returns>
		/// Reference to the list of vertices along the line.
		/// </returns>
		private static List<IVector3> InterpolatePath(IVector3 start,
			IVector3 end)
		{
			int distance = 0;
			int index = 0;
			List<IVector3> result = new List<IVector3>();
			int distX = 0;
			int distY = 0;
			int distZ = 0;

			if(start != null && end != null)
			{
				distX = end.X - start.X;
				distY = end.Y - start.Y;
				distZ = end.Z - start.Z;
				distance = (int)Math.Sqrt(
					(double)(distX * distX) +
					(double)(distY * distY) +
					(double)(distZ * distZ));

				if(distance > 0)
				{
					for(index = 0; index <= distance; index++)
					{
						result.Add(new IVector3()
						{
							X = start.X + ((index * distX) / distance),
							Y = start.Y + ((index * distY) / distance),
							Z = start.Z + ((index * distZ) / distance)
						});
					}
				}
				else
				{
					result.Add(new IVector3()
					{
						X = start.X,
						Y = start.Y,
						Z = start.Z
					});
				}
			}
			return result;
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
			float multiplier = 0f;
			int occupiedCount = 0;
			HashSet<VoxelKey> occupiedKeys = new HashSet<VoxelKey>();
			int offsetX = 0;
			int offsetY = 0;
			int offsetZ = 0;
			List<IVector3> pathPoints = null;
			IVector3 previousVoxel = null;
			List<GCodeVectorItem> result = new List<GCodeVectorItem>();
			int toolRadius = 0;
			int visitCount = 0;
			VoxelKey visitKey;

			if(GCodeVectorItem.VoxelPrecision != 0f)
			{
				multiplier = 1f / GCodeVectorItem.VoxelPrecision;
			}
			foreach(GCodeVectorItem vectoritem in mVectors)
			{
				if(previousVoxel != null && vectoritem.PenDown)
				{
					toolRadius =
						(int)Math.Ceiling((vectoritem.ToolDiameter * multiplier) / 2f);
					pathPoints = InterpolatePath(previousVoxel, vectoritem.Voxel);
					occupiedCount = 0;
					visitCount = 0;
					foreach(IVector3 pointItem in pathPoints)
					{
						for(offsetX = -toolRadius; offsetX <= toolRadius; offsetX++)
						{
							for(offsetY = -toolRadius; offsetY <= toolRadius; offsetY++)
							{
								//for(offsetZ = 0; offsetZ <= toolRadius; offsetZ++)
								//{
									if((offsetX * offsetX) +
										(offsetY * offsetY) +
										(offsetZ * offsetZ) <= (toolRadius * toolRadius))
									{
										visitCount++;
										visitKey = new VoxelKey(
											pointItem.X + offsetX,
											pointItem.Y + offsetY,
											pointItem.Z + offsetZ);
										if(occupiedKeys.Contains(visitKey))
										{
											occupiedCount++;
										}
										else
										{
											occupiedKeys.Add(visitKey);
										}
									}
								//}
							}
						}
					}
					//if(visitCount > 0 &&
					//	((float)occupiedCount / (float)visitCount) > 0.9f)
					//{
					//	//	This track is completely redundant.
					//	result.Add(vectoritem);
					//}
					if(visitCount > 0 && occupiedCount == visitCount)
					{
						//	This track is completely redundant.
						result.Add(vectoritem);
					}
				}
				previousVoxel = vectoritem.Voxel;
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

			foreach(string lineItem in mLines)
			{
				cursor.LineItem = new GCodeLineItem()
				{
					LineIndex = lineIndex,
					Value = lineItem
				};
				tu = lineItem.ToUpper();
				//	Update tool diameter.
				match = Regex.Match(lineItem, ResourceMain.rxToolDiameter);
				if(match.Success)
				{
					cursor.ToolDiameter =
						KiriMotoToolsUtil.ToFloat(
							ActionEngineUtil.GetValue(match, "diameter"));
				}
				//	Update safe height.
				match = Regex.Match(lineItem, ResourceMain.rxSafeHeight);
				if(match.Success)
				{
					cursor.SafeHeight =
						KiriMotoToolsUtil.ToFloat(
							ActionEngineUtil.GetValue(match, "height"));
				}
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
								LineItem = cursor.LineItem,
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
			mSafeHeight = cursor.SafeHeight;
			mVectors.Clear();
			mVectors.AddRange(cursor.Vectors);
			GCodeVectorCollection.InitializeVoxels(mVectors);
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
		//*	SafeHeight																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="SafeHeight">SafeHeight</see>.
		/// </summary>
		private float mSafeHeight = 1f;
		/// <summary>
		/// Get/Set the safe height of the tool above the stock.
		/// </summary>
		public float SafeHeight
		{
			get { return mSafeHeight; }
			set { mSafeHeight = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Vectors																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Vectors">Vectors</see>.
		/// </summary>
		private GCodeVectorCollection mVectors = new GCodeVectorCollection();
		/// <summary>
		/// Get a reference to the collection of vectors at this level.
		/// </summary>
		public GCodeVectorCollection Vectors
		{
			get { return mVectors; }
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
