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
			GCodeVectorItem previous = null;
			GCodeVectorItem result = new GCodeVectorItem();
			double toolRadius = 0d;

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
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	InitializeVoxels																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Initialize the voxels in the caller's collection.
		/// </summary>
		/// <param name="vectors">
		/// Reference to the collection of vectors whose voxels will be
		/// initialized.
		/// </param>
		public static void InitializeVoxels(List<GCodeVectorItem> vectors)
		{
			float multiplier = 0f;
			FVector3 vertex = null;

			if(vectors?.Count > 0)
			{
				if(GCodeVectorItem.VoxelPrecision != 0f)
				{
					multiplier = 1f / GCodeVectorItem.VoxelPrecision;
				}
				foreach(GCodeVectorItem vectorItem in vectors)
				{
					vertex = vectorItem.Vertex;
					if(vertex != null)
					{
						vectorItem.Voxel = new IVector3()
						{
							X = (int)Math.Ceiling(vertex.X * multiplier),
							Y = (int)Math.Ceiling(vertex.Y * multiplier),
							Z = (int)Math.Ceiling(vertex.Z * multiplier)
						};
					}
				}
			}
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

		//*-----------------------------------------------------------------------*
		//*	Voxel																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Voxel">Voxel</see>.
		/// </summary>
		private IVector3 mVoxel = new IVector3();
		/// <summary>
		/// Get/Set a reference to the voxel at this vertex.
		/// </summary>
		public IVector3 Voxel
		{
			get { return mVoxel; }
			set { mVoxel = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	VoxelPrecision																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="VoxelPrecision">VoxelPrecision</see>.
		/// </summary>
		private static float mVoxelPrecision = 0.25f;
		//private static float mVoxelPrecision = 0.5f;
		/// <summary>
		/// Get/Set the size of an individual voxel in this session.
		/// </summary>
		public static float VoxelPrecision
		{
			get { return mVoxelPrecision; }
			set { mVoxelPrecision = value; }
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
