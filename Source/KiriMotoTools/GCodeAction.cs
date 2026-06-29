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
	//*	GCodeActionCollection																										*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of GCodeActionItem Items.
	/// </summary>
	public class GCodeActionCollection : List<GCodeActionItem>
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
	//*	GCodeActionItem																													*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about an individual g-code action.
	/// </summary>
	public class GCodeActionItem
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
		//*	ActionIndex																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="ActionIndex">ActionIndex</see>.
		/// </summary>
		private int mActionIndex = 0;
		/// <summary>
		/// Get/Set the command index of the action.
		/// </summary>
		public int ActionIndex
		{
			get { return mActionIndex; }
			set { mActionIndex = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ActionType																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="ActionType">ActionType</see>.
		/// </summary>
		private GCodeActionType mActionType = GCodeActionType.None;
		/// <summary>
		/// Get/Set the type of action.
		/// </summary>
		public GCodeActionType ActionType
		{
			get { return mActionType; }
			set { mActionType = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetActionType																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the action type enumeration corresponding to the supplied
		/// action name.
		/// </summary>
		/// <param name="actionName">
		/// Name of the action to translate.
		/// </param>
		/// <returns>
		/// The g-code action type corresponding to the provided name.
		/// </returns>
		public static GCodeActionType GetActionType(string actionName)
		{
			GCodeActionType result = GCodeActionType.None;

			if(actionName != null)
			{
				switch(actionName.ToUpper())
				{
					case "":
						result = GCodeActionType.Blank;
						break;
					case ";":
						result = GCodeActionType.Comment;
						break;
					case "G":
						result = GCodeActionType.GCode;
						break;
					case "M":
						result = GCodeActionType.MCode;
						break;
				}
			}
			return result;
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
		/// Get/Set the raw line from which this action was created.
		/// </summary>
		public GCodeLineItem Line
		{
			get { return mLine; }
			set { mLine = value; }
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
		/// Get/Set a value indicating whether this action is selected.
		/// </summary>
		public bool Selected
		{
			get { return mSelected; }
			set { mSelected = value; }
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*

}
