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

using ActionEngine;
using Geometry;

namespace KiriMotoTools
{
	//*-------------------------------------------------------------------------*
	//*	GCodeCursor																															*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about the current status of the g-code at its current
	/// tracking position.
	/// </summary>
	public class GCodeCursor
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		/// <summary>
		/// The previously known position.
		/// </summary>
		private FVector3 mLastPosition = new FVector3();

		//*-----------------------------------------------------------------------*
		//* mPosition_CoordinateChanged																						*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// A coordinate has changed on the Position property.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Floating point event arguments.
		/// </param>
		private void mPosition_CoordinateChanged(object sender,
			FloatPointEventArgs e)
		{
			PositionUpdated();
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* PositionUpdated																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Update the position-sensitive values based upon the current position
		/// delta.
		/// </summary>
		private void PositionUpdated()
		{
			if(mPosition?.Z > mLastPosition.Z)
			{
				mPenDown = false;
			}
			FVector3.TransferValues(mPosition, mLastPosition);
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* UpdateCursorPosition																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Update the cursor's current position from the X, Y, and Z parameters.
		/// </summary>
		/// <param name="cursor">
		/// Reference to the cursor to update.
		/// </param>
		private void UpdateCursorPosition()
		{
			GCodeParameterItem parameter = null;

			parameter =
				mParameters.FirstOrDefault(x => x.Name == "X");
			if(parameter != null)
			{
				if(mIsRelative)
				{
					mPosition.X += parameter.Value;
				}
				else
				{
					mPosition.X = parameter.Value;
				}
			}
			parameter =
				mParameters.FirstOrDefault(x => x.Name == "Y");
			if(parameter != null)
			{
				if(mIsRelative)
				{
					mPosition.Y += parameter.Value;
				}
				else
				{
					mPosition.Y = parameter.Value;
				}
			}
			parameter =
				mParameters.FirstOrDefault(x => x.Name == "Z");
			if(parameter != null)
			{
				if(mIsRelative)
				{
					mPosition.Z += parameter.Value;
				}
				else
				{
					mPosition.Z = parameter.Value;
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
		//*	_Constructor																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Create a new instance of the GCodeCursor item.
		/// </summary>
		public GCodeCursor()
		{
			Position = new FVector3();
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Action																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Action">Action</see>.
		/// </summary>
		private GCodeActionItem mAction = null;
		/// <summary>
		/// Get/Set a reference to the current complete action at the cursor.
		/// </summary>
		public GCodeActionItem Action
		{
			get { return mAction; }
			set { mAction = value; }
		}
		//*-----------------------------------------------------------------------*

		////*-----------------------------------------------------------------------*
		////*	ActionName																														*
		////*-----------------------------------------------------------------------*
		///// <summary>
		///// Private member for <see cref="ActionName">ActionName</see>.
		///// </summary>
		//private string mActionName = "";
		///// <summary>
		///// Get/Set the current command action name (G or M).
		///// </summary>
		//public string ActionName
		//{
		//	get { return mActionName; }
		//	set { mActionName = value; }
		//}
		////*-----------------------------------------------------------------------*

		////*-----------------------------------------------------------------------*
		////*	ActionIndex																														*
		////*-----------------------------------------------------------------------*
		///// <summary>
		///// Private member for <see cref="ActionIndex">ActionIndex</see>.
		///// </summary>
		//private int mActionIndex = 0;
		///// <summary>
		///// Get/Set the current action index.
		///// </summary>
		//public int ActionIndex
		//{
		//	get { return mActionIndex; }
		//	set { mActionIndex = value; }
		//}
		////*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Capture																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Capture the current cursor position and settings.
		/// </summary>
		public void Capture()
		{
			UpdateCursorPosition();
			mVectors.Add(new GCodeVectorItem()
			{
				Action = mAction,
				PenDown = mPenDown,
				ToolDiameter = mToolDiameter,
				Vertex = new FVector3(mPosition)
			});
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Comment																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Comment">Comment</see>.
		/// </summary>
		private string mComment = "";
		/// <summary>
		/// Get/Set the comment on this line.
		/// </summary>
		public string Comment
		{
			get { return mComment; }
			set { mComment = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	IsRelative																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="IsRelative">IsRelative</see>.
		/// </summary>
		private bool mIsRelative = false;
		/// <summary>
		/// Get/Set a value indicating whether coordinates are relative.
		/// </summary>
		public bool IsRelative
		{
			get { return mIsRelative; }
			set { mIsRelative = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	LastPositionHadZ																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="LastPositionHadZ">LastPositionHadZ</see>.
		/// </summary>
		private bool mLastPositionHadZ = false;
		/// <summary>
		/// Get/Set a value indicating whether the last position specified Z.
		/// </summary>
		public bool LastPositionHadZ
		{
			get { return mLastPositionHadZ; }
			set { mLastPositionHadZ = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Line																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Line">Line</see>.
		/// </summary>
		private GCodeLineItem mLine = null;
		/// <summary>
		/// Get/Set a reference to the currently active g-code line.
		/// </summary>
		public GCodeLineItem Line
		{
			get { return mLine; }
			set { mLine = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Parameters																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Parameters">Parameters</see>.
		/// </summary>
		private GCodeParameterCollection mParameters =
			new GCodeParameterCollection();
		/// <summary>
		/// Get a reference to the collection of parameters for this command.
		/// </summary>
		public GCodeParameterCollection Parameters
		{
			get { return mParameters; }
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
		//*	Position																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Position">Position</see>.
		/// </summary>
		private FVector3 mPosition = null;
		/// <summary>
		/// Get/Set a reference to the current absolute position of the cursor.
		/// </summary>
		public FVector3 Position
		{
			get { return mPosition; }
			set
			{
				if(mPosition != null)
				{
					mPosition.CoordinateChanged -= mPosition_CoordinateChanged;
				}
				mPosition = value;
				if(value != null)
				{
					mPosition.CoordinateChanged += mPosition_CoordinateChanged;
				}
				PositionUpdated();
			}
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
