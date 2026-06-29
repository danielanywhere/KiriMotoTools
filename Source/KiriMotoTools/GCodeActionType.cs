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
	//*	GCodeActionType																													*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Enumeration of the possible g-code action types.
	/// </summary>
	public enum GCodeActionType
	{
		/// <summary>
		/// No g-code specified, or unknown.
		/// </summary>
		None = 0,
		/// <summary>
		/// A general preparatory code (G).
		/// </summary>
		GCode,
		/// <summary>
		/// A miscellaneous code (M).
		/// </summary>
		MCode,
		/// <summary>
		/// A complete line comment.
		/// </summary>
		Comment,
		/// <summary>
		/// An empty line.
		/// </summary>
		Blank
	}
	//*-------------------------------------------------------------------------*

}
