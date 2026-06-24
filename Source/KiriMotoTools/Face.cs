/*
 * Copyright (c). 2025 Daniel Patterson, MCSD (danielanywhere).
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

using static KiriMotoTools.KiriMotoToolsUtil;

namespace KiriMotoTools
{
	//*-------------------------------------------------------------------------*
	//*	FaceCollection																													*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of FaceItem Items.
	/// </summary>
	public class FaceCollection : List<FaceItem>
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//private static float epsilon = 1e-6f;
		private static float epsilon = 1e-3f;

		//*-----------------------------------------------------------------------*
		//* ClipPolygon																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Clip the two polygons to a single, shared polygon shape.
		/// </summary>
		/// <param name="subject">
		/// Reference to the subject polygon to clip.
		/// </param>
		/// <param name="clip">
		/// Reference to the clip to remove from the subject.
		/// </param>
		/// <returns>
		/// Reference to the collection of points remaining in the clipped
		/// polygon, if found. Otherwise, an empty collection.
		/// </returns>
		private static List<FVector2> ClipPolygon(List<FVector2> subject,
			List<FVector2> clip)
		{
			bool bPIsInside = false;
			bool bQIsInside = false;
			int i = 0;
			int iCount = 0;
			List<FVector2> input = null;
			int j = 0;
			int jCount = 0;
			List<FVector2> output = new List<FVector2>();
			FVector2 vectorA = null;
			FVector2 vectorB = null;
			FVector2 vectorP = null;
			FVector2 vectorQ = null;

			if(subject?.Count > 0)
			{
				output.AddRange(subject);
				if(clip?.Count > 0)
				{
					iCount = clip.Count;
					for(i = 0; i < iCount; i ++)
					{
						vectorA = clip[i];
						vectorB = clip[(i + 1) % iCount];

						input = output.ToList();
						output.Clear();

						if(input.Count > 0)
						{
							jCount = input.Count;
							for(j = 0; j < jCount; j++)
							{
								vectorP = input[j];
								vectorQ = input[(j + 1) % jCount];

								bPIsInside = IsLeft(vectorA, vectorB, vectorP);
								bQIsInside = IsLeft(vectorA, vectorB, vectorQ);

								if(bPIsInside && bQIsInside)
								{
									output.Add(vectorQ);
								}
								else if(bPIsInside && !bQIsInside)
								{
									output.Add(Intersect(vectorA, vectorB, vectorP, vectorQ));
								}
								else if(!bPIsInside && bQIsInside)
								{
									output.Add(Intersect(vectorA, vectorB, vectorP, vectorQ));
									output.Add(vectorQ);
								}
							}
						}
						else
						{
							break;
						}
					}
				}
				else
				{
					output.AddRange(subject);
				}
			}
			return output;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Cross																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Calculate a cross-product between the two vectors.
		/// </summary>
		/// <param name="u">
		/// Reference to the first vector to compare.
		/// </param>
		/// <param name="v">
		/// Reference to the second vector to compare.
		/// </param>
		/// <returns>
		/// The cross product of the two vectors. Otherwise, 0.
		/// </returns>
		private static float Cross(FVector2 u, FVector2 v)
		{
			float result = 0f;
			if(u != null && v != null)
			{
				result = (u.X * v.Y) - (u.Y * v.X);
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetSharedArea																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the approximate shared area between two faces.
		/// </summary>
		/// <param name="face1">
		/// Reference to the first face to compare.
		/// </param>
		/// <param name="face2">
		/// Reference to the second face to compare.
		/// </param>
		/// <returns>
		/// Reference to the 2D shared area between the two triangles, if found.
		/// Otherwise, null;
		/// </returns>
		private static FArea GetSharedArea(FaceItem face1, FaceItem face2)
		{
			List<FVector2> intersections = null;
			float maxX = 0f;
			float maxY = 0f;
			float minX = 0f;
			float minY = 0f;
			FArea result = null;
			List<FVector2> triangle1 = null;
			List<FVector2> triangle2 = null;

			if(face1 != null && face2 != null)
			{
				triangle1 = face1.Vectors.Select(v => To2D(v)).ToList();
				triangle2 = face2.Vectors.Select(v => To2D(v)).ToList();
				intersections = ClipPolygon(triangle1, triangle2);
				if(intersections.Count > 0)
				{
					minX = intersections.Min(x => x.X);
					minY = intersections.Min(y => y.Y);
					maxX = intersections.Max(x => x.X);
					maxY = intersections.Max(y => y.Y);
					result = new FArea(
						minX, minY,
						maxX - minX, maxY - minY);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Intersect																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Intersect two lines and return the intersection point between them.
		/// </summary>
		/// <param name="a">
		/// Reference to the starting point of line A.
		/// </param>
		/// <param name="b">
		/// Reference to the ending point of line A.
		/// </param>
		/// <param name="p">
		/// Reference to the starting point of line P.
		/// </param>
		/// <param name="q">
		/// Reference to the ending point of line P.
		/// </param>
		/// <returns>
		/// An intersecting point between the two lines, if found. Otherwise, null.
		/// </returns>
		private static FVector2 Intersect(FVector2 a, FVector2 b,
			FVector2 p, FVector2 q)
		{
			FVector2 ab = b - a;
			float crossAbPq = 0f;
			FVector2 pq = q - p;
			float t = 0f;

			crossAbPq = Cross(ab, pq);
			if(Math.Abs(crossAbPq) > epsilon)
			{
				t = Cross(p - a, pq) / Cross(ab, pq);
			}
			return a + ab * t;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* IsLeft
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the specified point is left of the
		/// provided line.
		/// </summary>
		/// <param name="lineStart">
		/// Reference to the start of the line.
		/// </param>
		/// <param name="lineEnd">
		/// Reference to the end of the line to test.
		/// </param>
		/// <param name="point">
		/// Reference to the point location to test.
		/// </param>
		/// <returns>
		/// True if the point is to the left of the line. Otherwise, false.
		/// </returns>
		private static bool IsLeft(FVector2 lineStart, FVector2 lineEnd,
			FVector2 point)
		{
			return
				((lineEnd.X - lineStart.X) * (point.Y - lineStart.Y) -
				(lineEnd.Y - lineStart.Y) * (point.X - lineStart.X)) >= epsilon;
		}
		//*-----------------------------------------------------------------------*

		//*************************************************************************
		//*	Protected																															*
		//*************************************************************************
		//*************************************************************************
		//*	Public																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//* DeselectObstructed																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Deselect any faces that have partial or complete faces above them.
		/// </summary>
		/// <param name="faces">
		/// Reference to the collection of faces to inspect.
		/// </param>
		/// <returns>
		/// Count of items deselected, if found. Otherwise, 0.
		/// </returns>
		public static int DeselectObstructed(List<FaceItem> faces)
		{
			FArea activeBounds = null;
			List<FaceItem> activeFaces = null;
			FArea comparisonBounds = null;
			List<FaceItem> comparisonFaces = null;
			List<FVector2> intersections = null;
			float maxX = 0f;
			float maxY = 0f;
			float minX = 0f;
			float minY = 0f;
			int result = 0;
			FArea sharedArea = null;
			float zHigh = 0f;
			float zLow = 0f;

			if(faces?.Count > 0)
			{
				activeFaces = faces.FindAll(x => x.Selected == true);
				comparisonFaces = faces.FindAll(x => x.Selected == false);
				foreach(FaceItem activeItem in activeFaces)
				{
					zLow =
						Math.Min(Math.Min(activeItem.Vectors[0].Z,
							activeItem.Vectors[1].Z),
							activeItem.Vectors[2].Z);
					minX =
						Math.Min(Math.Min(activeItem.Vectors[0].X,
							activeItem.Vectors[1].X),
							activeItem.Vectors[2].X);
					minY =
						Math.Min(Math.Min(activeItem.Vectors[0].Y,
							activeItem.Vectors[1].Y),
							activeItem.Vectors[2].Y);
					maxX =
						Math.Max(Math.Max(activeItem.Vectors[0].X,
							activeItem.Vectors[1].X),
							activeItem.Vectors[2].X);
					maxY =
						Math.Max(Math.Max(activeItem.Vectors[0].Y,
							activeItem.Vectors[1].Y),
							activeItem.Vectors[2].Y);
					activeBounds = new FArea(minX, minY, maxX - minX, maxY - minY);
					foreach(FaceItem comparisonItem in comparisonFaces)
					{
						zHigh =
							Math.Max(Math.Max(comparisonItem.Vectors[0].Z,
								comparisonItem.Vectors[1].Z),
								comparisonItem.Vectors[2].Z);
						if(zHigh > zLow)
						{
							//	The comparison face might be over the active item.
							minX =
								Math.Min(Math.Min(comparisonItem.Vectors[0].X,
									comparisonItem.Vectors[1].X),
									comparisonItem.Vectors[2].X);
							minY =
								Math.Min(Math.Min(comparisonItem.Vectors[0].Y,
									comparisonItem.Vectors[1].Y),
									comparisonItem.Vectors[2].Y);
							maxX =
								Math.Max(Math.Max(comparisonItem.Vectors[0].X,
									comparisonItem.Vectors[1].X),
									comparisonItem.Vectors[2].X);
							maxY =
								Math.Max(Math.Max(comparisonItem.Vectors[0].Y,
									comparisonItem.Vectors[1].Y),
									comparisonItem.Vectors[2].Y);
							comparisonBounds =
								new FArea(minX, minY, maxX - minX, maxY - minY);
							intersections =
								FArea.GetIntersections(comparisonBounds, activeBounds);
							if(intersections.Count > 0)
							{
								//	When the bounding boxes intersect in 2D, there may be an
								//	overlap of shapes, but there is also a possibility of
								//	cohabitation.
								sharedArea = GetSharedArea(comparisonItem, activeItem);
								if(sharedArea != null)
								{
									activeItem.Selected = false;
									result++;
								}
							}
						}
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* DeselectSurface																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Deselect anything at or above the surface.
		/// </summary>
		/// <param name="faces">
		/// Reference to the list of faces to inspect.
		/// </param>
		/// <param name="surface">
		/// Surface level.
		/// </param>
		/// <returns>
		/// The count of faces deselected because of being on the surface.
		/// </returns>
		public static int DeselectSurface(List<FaceItem> faces, float surface)
		{
			List<FaceItem> activeFaces = null;
			float difference = 0f;
			int result = 0;

			if(faces?.Count > 0)
			{
				activeFaces = faces.FindAll(x => x.Selected);
				foreach(FaceItem faceItem in activeFaces)
				{
					if(
						faceItem.Vectors[0].Z - surface > -epsilon ||
						faceItem.Vectors[1].Z - surface > -epsilon ||
						faceItem.Vectors[2].Z - surface > -epsilon)
					{
						faceItem.Selected = false;
						result++;
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetSelectedIndices																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the list of selected face indices.
		/// </summary>
		/// <param name="faces">
		/// Reference to the list of faces to inspect.
		/// </param>
		/// <returns>
		/// Reference to the list of selected indices in the caller's list, if
		/// found. Otherwise, an empty list.
		/// </returns>
		public static List<int> GetSelectedIndices(List<FaceItem> faces)
		{
			int count = 0;
			int index = 0;
			List<int> result = new List<int>();

			if(faces?.Count > 0)
			{
				count = faces.Count;
				for(index = 0; index < count; index ++)
				{
					if(faces[index].Selected)
					{
						result.Add(index);
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	SelectUpFacing																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Select any up-facing faces, skipping all vertical and down facing
		/// items.
		/// </summary>
		/// <param name="faces">
		/// Reference to the collection of faces to inspect.
		/// </param>
		/// <returns>
		/// Count of items selected, if found. Otherwise, 0.
		/// </returns>
		public static int SelectUpFacing(List<FaceItem> faces)
		{
			FVector3 edge1 = null;
			FVector3 edge2 = null;
			float magnitude = 0f;
			FVector3 normal = null;
			float normalizedZ = 0f;
			int result = 0;
			FVector3[] vectors = null;

			if(faces?.Count > 0)
			{
				foreach(FaceItem faceItem in faces)
				{
					vectors = faceItem.Vectors;
					edge1 = vectors[1] - vectors[0];
					edge2 = vectors[2] - vectors[0];
					normal = FVector3.CrossProduct(edge1, edge2);
					magnitude = FVector3.Magnitude(normal);
					if(magnitude > 0f)
					{
						normalizedZ = normal.Z / magnitude;
						if(normalizedZ > epsilon)
						{
							faceItem.Selected = true;
							result++;
						}
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	FaceItem																																*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about an individual face.
	/// </summary>
	public class FaceItem
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
		//*	Selected																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Selected">Selected</see>.
		/// </summary>
		private bool mSelected = false;
		/// <summary>
		/// Get/Set a value indicating whether this face is selected.
		/// </summary>
		public bool Selected
		{
			get { return mSelected; }
			set { mSelected = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Vectors																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Vectors">Vectors</see>.
		/// </summary>
		private FVector3[] mVectors = new FVector3[3];
		/// <summary>
		/// Get a reference to the array of vectors on this face.
		/// </summary>
		public FVector3[] Vectors
		{
			get { return mVectors; }
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
