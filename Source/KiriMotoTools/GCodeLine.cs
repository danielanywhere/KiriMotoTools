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

namespace KiriMotoTools
{
	//*-------------------------------------------------------------------------*
	//*	GCodeLineCollection																											*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of GCodeLineItem Items.
	/// </summary>
	public class GCodeLineCollection : List<GCodeLineItem>
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
	//*	GCodeLineItem																														*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about a raw g-code line.
	/// </summary>
	public class GCodeLineItem
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
		//*	LineIndex																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="LineIndex">LineIndex</see>.
		/// </summary>
		private int mLineIndex = 0;
		/// <summary>
		/// Get/Set the index of the file at which this line is located.
		/// </summary>
		public int LineIndex
		{
			get { return mLineIndex; }
			set { mLineIndex = value; }
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
		/// Get/Set a value indicating whether this line is selected.
		/// </summary>
		public bool Selected
		{
			get { return mSelected; }
			set { mSelected = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Value																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Value">Value</see>.
		/// </summary>
		private string mValue = "";
		/// <summary>
		/// Get/Set the raw value of the line.
		/// </summary>
		public string Value
		{
			get { return mValue; }
			set { mValue = value; }
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*


}
