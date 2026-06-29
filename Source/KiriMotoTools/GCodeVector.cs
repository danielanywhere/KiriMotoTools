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

using Clipper2Lib;
using Geometry;

namespace KiriMotoTools
{
	//*-------------------------------------------------------------------------*
	//*	GCodeVectorCollection																										*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of GCodeVectorItem Items.
	/// </summary>
	public class GCodeVectorCollection : List<GCodeVectorItem>
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		/// <summary>
		/// The multiplier used to convert the natural floating point vertex to
		/// an integer for conversion to Minkowsky sums.
		/// </summary>
		private const double mMinkowskiMultiplier = 10000d;

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
		/// Add a new item by member values.
		/// </summary>
		/// <param name="action">
		/// Reference to the g-code action from which this vector was created.
		/// </param>
		/// <param name="vertex">
		/// Reference to the absolute vertex.
		/// </param>
		/// <param name="penDown">
		/// A value indicating whether the pen is DOWN (true), or up (false).
		/// </param>
		/// <param name="toolDiameter">
		/// The tool diameter at this location.
		/// </param>
		/// <returns>
		/// Reference to the newly added GCodeVectorItem.
		/// </returns>
		public GCodeVectorItem Add(GCodeActionItem action, FVector3 vertex,
			bool penDown, float toolDiameter)
		{
			Path64 pattern = null;
			GCodeVectorItem previous = null;
			GCodeVectorItem result = new GCodeVectorItem();
			double toolRadius = 0d;
			Path64 track = null;

			if(vertex != null)
			{
				if(this.Count > 0)
				{
					previous = this[this.Count - 1];
				}
				result.Action = action;
				result.Vertex = new FVector3(vertex);
				result.PenDown = penDown;
				result.ToolDiameter = toolDiameter;
				this.Add(result);
				if(result.PenDown && previous?.PenDown == true)
				{
					toolRadius = (double)(result.ToolDiameter / 2);
					pattern = Clipper.Ellipse(new Point64(0, 0), toolRadius, toolRadius);
					track = new Path64()
					{
						new Point64(
							(double)previous.Vertex.X * mMinkowskiMultiplier,
							(double)previous.Vertex.Y * mMinkowskiMultiplier),
						new Point64(
							(double)result.Vertex.X * mMinkowskiMultiplier,
							(double)result.Vertex.Y * mMinkowskiMultiplier)
					};
					result.Minkowski = Clipper.MinkowskiSum(pattern, track, true);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	GCodeVectorItem																													*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about an individual GCode vertex and pen status.
	/// </summary>
	public class GCodeVectorItem
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
		//*	Action																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Action">Action</see>.
		/// </summary>
		private GCodeActionItem mAction = null;
		/// <summary>
		/// Get/Set a reference to the action from which this vector was created.
		/// </summary>
		public GCodeActionItem Action
		{
			get { return mAction; }
			set { mAction = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Minkowski																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Minkowski">Minkowski</see>.
		/// </summary>
		private Paths64 mMinkowski = null;
		/// <summary>
		/// Get/Set a reference to the pill-shaped Minkowski between this vertex
		/// and the previous one.
		/// </summary>
		public Paths64 Minkowski
		{
			get { return mMinkowski; }
			set { mMinkowski = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	PenDown																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="PenDown">PenDown</see>.
		/// </summary>
		private bool mPenDown = false;
		/// <summary>
		/// Get/Set a value indicating whether the pen is DOWN (true), or
		/// up (false).
		/// </summary>
		public bool PenDown
		{
			get { return mPenDown; }
			set { mPenDown = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Selected																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Selected">Selected</see>.
		/// </summary>
		private bool mSelected = false;
		/// <summary>
		/// Get/Set a value indicating whether this vector is selected.
		/// </summary>
		public bool Selected
		{
			get { return mSelected; }
			set { mSelected = value; }
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
		/// Get/Set the size of the tool.
		/// </summary>
		public float ToolDiameter
		{
			get { return mToolDiameter; }
			set { mToolDiameter = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Vertex																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Vertex">Vertex</see>.
		/// </summary>
		private FVector3 mVertex = new FVector3();
		/// <summary>
		/// Get/Set a reference to the tangible vertex.
		/// </summary>
		public FVector3 Vertex
		{
			get { return mVertex; }
			set { mVertex = value; }
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
