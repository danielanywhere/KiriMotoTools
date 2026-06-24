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
using System.Globalization;
using System.IO;
using System.Text;

using Geometry;

namespace KiriMotoTools
{
	//*-------------------------------------------------------------------------*
	//*	StlReader																																*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Basic STL file reader.
	/// </summary>
	public class StlReader
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		/// <summary>
		/// Allowed whitespace characters.
		/// </summary>
		private static readonly char[] mWhitespaces =
			new char[] { '\t', '\r', '\n', ' ' };

		//*-----------------------------------------------------------------------*
		//*	ReadAsciiStl																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Read ASCII version STL content.
		/// </summary>
		/// <param name="inputStream">
		/// Reference to the input stream containing ASCII STL information.
		/// </param>
		/// <param name="vertices">
		/// Reference to the collection of vertices to be filled.
		/// </param>
		private static void ReadAsciiStl(Stream inputStream,
			List<FVector3> vertices)
		{
			string content = "";
			int index = 0;
			bool parsedX = false;
			bool parsedY = false;
			bool parsedZ = false;
			StreamReader reader = default(StreamReader);
			string[] tokens = null;
			float x = 0.0f;
			float y = 0.0f;
			float z = 0.0f;

			if(inputStream?.CanRead == true && vertices != null)
			{
				reader = new StreamReader(inputStream,
					Encoding.ASCII, true, 1024, true);
				content = reader.ReadToEnd();
				tokens = content.Split(mWhitespaces,
					StringSplitOptions.RemoveEmptyEntries);
				for(index = 0; index < tokens.Length; index ++)
				{
					if(index + 3 < tokens.Length &&
						string.Equals(tokens[index], "vertex",
							StringComparison.OrdinalIgnoreCase))
					{
						parsedX = float.TryParse(tokens[index + 1],
							NumberStyles.Float, CultureInfo.InvariantCulture, out x);
						parsedY = float.TryParse(tokens[index + 2],
							NumberStyles.Float, CultureInfo.InvariantCulture, out y);
						parsedZ = float.TryParse(tokens[index + 3],
							NumberStyles.Float, CultureInfo.InvariantCulture, out z);
						if(parsedX && parsedY && parsedZ)
						{
							vertices.Add(new FVector3(x, y, z));
						}
						index += 3;
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ReadBinaryStl																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Read Binary version STL content.
		/// </summary>
		/// <param name="inputStream">
		/// Reference to the input stream containing ASCII STL information.
		/// </param>
		/// <param name="vertices">
		/// Reference to the collection of vertices to be filled.
		/// </param>
		private static void ReadBinaryStl(Stream inputStream,
			List<FVector3> vertices)
		{
			UInt16 attributeByteCount = 0;
			byte[] header = new byte[80];
			BinaryReader reader = default(BinaryReader);
			ulong triCount = 0;
			ulong triIndex = 0;
			int vertIndex = 0;
			float x = 0.0f;
			float y = 0.0f;
			float z = 0.0f;

			if(inputStream?.CanRead == true && vertices != null)
			{
				reader = new BinaryReader(inputStream);
				reader.Read(header, 0, 80);
				triCount = reader.ReadUInt32();

				for(triIndex = 0; triIndex < triCount; triIndex ++)
				{
					//	Skip normals.
					reader.ReadSingle();
					reader.ReadSingle();
					reader.ReadSingle();

					for(vertIndex = 0; vertIndex < 3; vertIndex ++)
					{
						x = reader.ReadSingle();
						y = reader.ReadSingle();
						z = reader.ReadSingle();
						vertices.Add(new FVector3(x, y, z));
					}

					attributeByteCount = reader.ReadUInt16();
					if(attributeByteCount > 0)
					{
						//	Skip any additional attribute bytes.
						reader.ReadBytes(attributeByteCount);
					}
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
		//*	ReadStl																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Read an ASCII or binary STL file stream and return a corresponding
		/// collection of 3D vertices.
		/// </summary>
		/// <param name="inputStream">
		/// Reference to the input stream to read.
		/// </param>
		/// <returns>
		/// Reference to a collection of 3D vertices found in the caller's
		/// source, if found. Otherwise, an empty list.
		/// </returns>
		public static List<FVector3> ReadStl(Stream inputStream)
		{
			int bytesRead = 0;
			byte[] header = null;
			string headerText = "";
			bool isAscii = false;
			long originalPosition = 0;
			List<FVector3> vertices = null;

			if(inputStream?.CanRead == true)
			{
				header = new byte[80];
				vertices = new List<FVector3>();
				originalPosition = 0;
				if(inputStream.CanSeek)
				{
					originalPosition = inputStream.Position;
				}
				bytesRead = inputStream.Read(header, 0, 80);
				if(bytesRead == 80)
				{
					headerText = Encoding.ASCII.GetString(header);
					if(headerText.StartsWith("solid",
						StringComparison.OrdinalIgnoreCase))
					{
						isAscii = true;
					}
					else
					{
						isAscii = false;
					}
				}
				if(inputStream.CanSeek)
				{
					inputStream.Position = originalPosition;
				}
				if(isAscii)
				{
					ReadAsciiStl(inputStream, vertices);
				}
				else
				{
					ReadBinaryStl(inputStream, vertices);
				}
			}
			return vertices;
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*

}
