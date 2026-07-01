using ActionEngine;
using Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace KiriMotoTools
{
	//*-------------------------------------------------------------------------*
	//*	IVector3																																*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// 3D int vector class.
	/// </summary>
	public class IVector3
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
		//*	_Implicit IVector3 = FVector3																					*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Cast a FVector3 value to IVector3.
		/// </summary>
		public static implicit operator IVector3(FVector3 value)
		{
			IVector3 rv = new IVector3();

			if(value != null)
			{
				rv.mX = (int)(value.X * 10000f);
				rv.mY = (int)(value.Y * 10000f);
				rv.mZ = (int)(value.Z * 10000f);
			}
			return rv;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Magnitude																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the magnitude of the provided vector.
		/// </summary>
		/// <param name="value">
		/// Reference to the vector to inspect.
		/// </param>
		/// <returns>
		/// The magnitude of the vector.
		/// </returns>
		public static int Magnitude(IVector3 value)
		{
			int result = 0;

			if(value != null)
			{
				result = (int)Math.Sqrt(
					(double)(value.mX * value.mX) +
					(double)(value.mY * value.mY) +
					(double)(value.mZ * value.mZ)
					);
			}
			return result;
		}
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
			return $"X:{mX:0.###};Y:{mY:0.###};Z:{mZ:0.###}";
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	X																																			*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="X">X</see>.
		/// </summary>
		private int mX = 0;
		/// <summary>
		/// Get/Set the X position.
		/// </summary>
		public int X
		{
			get { return mX; }
			set { mX = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Y																																			*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Y">Y</see>.
		/// </summary>
		private int mY = 0;
		/// <summary>
		/// Get/Set the Y position.
		/// </summary>
		public int Y
		{
			get { return mY; }
			set { mY = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Z																																			*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Z">Z</see>.
		/// </summary>
		private int mZ = 0;
		/// <summary>
		/// Get/Set the Z position.
		/// </summary>
		public int Z
		{
			get { return mZ; }
			set { mZ = value; }
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*

}
