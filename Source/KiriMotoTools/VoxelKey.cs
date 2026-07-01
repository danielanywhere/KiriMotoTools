using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiriMotoTools
{
	//*-------------------------------------------------------------------------*
	//*	public struct VoxelKey																									*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Representation of a voxel coordinate.
	/// </summary>
	public struct VoxelKey
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*************************************************************************
		//*	Public																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//*	_Constructor																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Create a new instance of the VoxelKey item.
		/// </summary>
		/// <param name="x">
		/// The X coordinate of this instance.
		/// </param>
		/// <param name="y">
		/// The Y coordinate of this instance.
		/// </param>
		/// <param name="z">
		/// The Z coordinate of this instance.
		/// </param>
		public VoxelKey(int x, int y, int z)
		{
			X = x;
			Y = y;
			Z = z;
			mHashCode = HashCode.Combine(x, y, z);
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Equals																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether this item is equal to another in
		/// member values.
		/// </summary>
		/// <param name="obj">
		/// Reference to the other object to compare.
		/// </param>
		/// <returns>
		/// True if the two objects are equal. Otherwise, false.
		/// </returns>
		public override bool Equals(object obj) =>
			obj is VoxelKey other && X == other.X && Y == other.Y && Z == other.Z;
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetHashCode																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the unique hash code for this voxel location.
		/// </summary>
		private int mHashCode = 0;
		public override int GetHashCode() => mHashCode;
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ToString																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the string representation of this object.
		/// </summary>
		/// <returns>
		/// The string representation of this object.
		/// </returns>
		public override string ToString()
		{
			return $"X:{X:0.###};Y:{Y:0.###};Z:{Z:0.###}";
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* X																																			*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get the X position of this coordinate.
		/// </summary>
		public int X { get; }
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Y																																			*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get the Y position of this coordinate.
		/// </summary>
		public int Y { get; }
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Z																																			*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get the Z position of this coordinate.
		/// </summary>
		public int Z { get; }
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
