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

namespace KiriMotoTools
{
	//*-------------------------------------------------------------------------*
	//*	GCodeToolShapeCollection																								*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of GCodeToolShapeItem Items.
	/// </summary>
	public class GCodeToolShapeCollection : List<GCodeToolShapeItem>
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
	//*	GCodeToolShapeItem																											*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about an individual shape made with a specific tool pass.
	/// </summary>
	public class GCodeToolShapeItem
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
		//*	EndingIndex																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="EndingIndex">EndingIndex</see>.
		/// </summary>
		private int mEndingIndex = 0;
		/// <summary>
		/// Get/Set the ending index of entries containing this tool.
		/// </summary>
		public int EndingIndex
		{
			get { return mEndingIndex; }
			set { mEndingIndex = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Minkowskies																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Minkowskies">Minkowskies</see>.
		/// </summary>
		private List<Paths64> mMinkowskies = new List<Paths64>();
		/// <summary>
		/// Get/Set a reference to the pill-shaped Minkowskies made during this
		/// tool pass.
		/// </summary>
		public List<Paths64> Minkowskies
		{
			get { return mMinkowskies; }
			set { mMinkowskies = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Shapes																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Shapes">Shapes</see>.
		/// </summary>
		private List<Paths64> mShapes = new List<Paths64>();
		/// <summary>
		/// Get a reference to the list of unioned shapes.
		/// </summary>
		public List<Paths64> Shapes
		{
			get { return mShapes; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	StartingIndex																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="StartingIndex">StartingIndex</see>.
		/// </summary>
		private int mStartingIndex = 0;
		/// <summary>
		/// Get/Set the starting index of entries containing this tool.
		/// </summary>
		public int StartingIndex
		{
			get { return mStartingIndex; }
			set { mStartingIndex = value; }
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

	}
	//*-------------------------------------------------------------------------*

}
