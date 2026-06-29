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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Geometry;

namespace KiriMotoTools
{
	//*-------------------------------------------------------------------------*
	//*	GCodeLevelCollection																										*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of GCodeLevelItem Items.
	/// </summary>
	public class GCodeLevelCollection : List<GCodeLevelItem>
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
		//*-----------------------------------------------------------------------*
		//*	Add																																		*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Add an item to the collection by member values.
		/// </summary>
		/// <param name="action">
		/// Reference to the g-code action from which the vector is being created.
		/// </param>
		/// <param name="vertex">
		/// Reference to the vertex to be recorded.
		/// </param>
		/// <param name="toolDiameter">
		/// The tool diameter.
		/// </param>
		/// <param name="penDown">
		/// Value indicating whether the tool is down at this vector.
		/// </param>
		/// <returns>
		/// Reference to the level item added or updated, if found.
		/// Otherwise, null.
		/// </returns>
		public GCodeLevelItem Add(GCodeActionItem action, FVector3 vertex,
			float toolDiameter, bool penDown)
		{
			GCodeLevelItem result = null;

			if(vertex != null)
			{
				//result = this.FirstOrDefault(z =>
				//	z.Level == vertex.Z &&
				//	z.ToolDiameter == toolDiameter);
				//if(result == null)
				//{
				//	result = new GCodeLevelItem()
				//	{
				//		Level = vertex.Z,
				//		ToolDiameter = toolDiameter
				//	};
				//	this.Add(result);
				//}
				result = this.FirstOrDefault(z =>
					z.Level == vertex.Z);
				if(result == null)
				{
					result = new GCodeLevelItem()
					{
						Level = vertex.Z
					};
					this.Add(result);
				}
				result.Vectors.Add(action, new FVector3(vertex),
					penDown, toolDiameter);
			}
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Add an item to the collection by its cursor information.
		/// </summary>
		/// <param name="action">
		/// Reference to the g-code action from which the vector is being created.
		/// </param>
		/// <param name="cursor">
		/// Reference to the cursor containing 3D position and tool diameter
		/// information.
		/// </param>
		/// <returns>
		/// Reference to the level item added or updated, if found.
		/// Otherwise, null.
		/// </returns>
		public GCodeLevelItem Add(GCodeCursor cursor)
		{
			GCodeLevelItem result = null;
			if(cursor != null)
			{
				//result = this.FirstOrDefault(z =>
				//	z.Level == cursor.Position.Z &&
				//	z.ToolDiameter == cursor.ToolDiameter);
				//if(result == null)
				//{
				//	result = new GCodeLevelItem()
				//	{
				//		Level = cursor.Position.Z,
				//		ToolDiameter = cursor.ToolDiameter
				//	};
				//	this.Add(result);
				//}
				result = this.FirstOrDefault(z =>
					z.Level == cursor.Position.Z);
				if(result == null)
				{
					result = new GCodeLevelItem()
					{
						Level = cursor.Position.Z
					};
					this.Add(result);
				}
				result.Vectors.Add(cursor.Action, new FVector3(cursor.Position),
					cursor.PenDown, cursor.ToolDiameter);
			}
			return result;
		}
		//*-----------------------------------------------------------------------*



	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	GCodeLevelItem																													*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about an individual Z-level.
	/// </summary>
	public class GCodeLevelItem
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
		//*-----------------------------------------------------------------------*
		//*	Level																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Level">Level</see>.
		/// </summary>
		private float mLevel = 0f;
		/// <summary>
		/// Get/Set the height of this level.
		/// </summary>
		public float Level
		{
			get { return mLevel; }
			set { mLevel = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ToolDiameter																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="ToolDiameter">ToolDiameter</see>.
		/// </summary>
		private float mToolDiameter = 0f;
		/// <summary>
		/// Get/Set the diameter of the tool for this level.
		/// </summary>
		public float ToolDiameter
		{
			get { return mToolDiameter; }
			set { mToolDiameter = value; }
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
