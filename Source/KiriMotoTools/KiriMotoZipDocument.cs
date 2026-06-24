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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ActionEngine;
using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json.Linq;

namespace KiriMotoTools
{
	//*-------------------------------------------------------------------------*
	//*	KiriMotoZipDocumentCollection																						*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of KiriMotoZipDocumentItem Items.
	/// </summary>
	public class KiriMotoZipDocumentCollection : List<KiriMotoZipDocumentItem>
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
	//*	KiriMotoZipDocumentItem																									*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about an individual Kiri:Moto zip document.
	/// </summary>
	public class KiriMotoZipDocumentItem : ActionDocumentItem
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
		//*	_Constructor																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Create a new instance of the KiriMotoZipDocumentItem item.
		/// </summary>
		public KiriMotoZipDocumentItem()
		{
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Create a new instance of the KiriMotoZipDocumentItem item.
		/// </summary>
		/// <param name="filename">
		/// The fully qualified path and filename of the document.
		/// </param>
		public KiriMotoZipDocumentItem(string filename)
		{
			Name = filename;
			InitializeDocument(filename);
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetJsonContent																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the JSON content of this file.
		/// </summary>
		/// <returns>
		/// The serialized JSON content of the loaded file, if present. Otherwise,
		/// an empty string.
		/// </returns>
		public string GetJsonContent()
		{
			string result = "";

			if(mJson != null)
			{
				result = mJson.ToString(Newtonsoft.Json.Formatting.None);
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	InitializeDocument																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Initialize the document object.
		/// </summary>
		/// <param name="filename">
		/// Fully qualified path a filename of the document to load.
		/// </param>
		public void InitializeDocument(string filename)
		{

			if(filename?.Length > 0)
			{
				ReadFile(filename);
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Json																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Json">Json</see>.
		/// </summary>
		private dynamic mJson = null;
		/// <summary>
		/// Get/Set a reference to the Newtonsoft Json JObject controlling the
		/// workspace.
		/// </summary>
		public dynamic Json
		{
			get { return mJson; }
			set { mJson = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Modified																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Modified">Modified</see>.
		/// </summary>
		private bool mModified = false;
		/// <summary>
		/// Get/Set a value indicating whether the contents of this document have
		/// been modified.
		/// </summary>
		public bool Modified
		{
			get { return mModified; }
			set { mModified = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ReadFile																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Read the file content from the fully qualified workspace path and
		/// filename.
		/// </summary>
		/// <param name="filename">
		/// Fully qualified path and filename of the workspace to read.
		/// </param>
		/// <returns>
		/// The content of the workspace's workspace.json file.
		/// </returns>
		/// <remarks>
		/// In this version, only workspace.json is read.
		/// </remarks>
		public void ReadFile(string filename)
		{
			string content = "";
			ZipEntry entry = null;
			ZipFile file = null;
			FileStream fs = null;

			if(filename?.Length > 0 && File.Exists(filename))
			{
				try
				{
					fs = File.OpenRead(filename);
					file = new ZipFile(fs);
					foreach(ZipEntry zipEntryItem in file)
					{
						if(zipEntryItem.Name.ToLower() == "workspace.json")
						{
							using(Stream zipStream = file.GetInputStream(zipEntryItem))
							{
								using(StreamReader reader =
									new StreamReader(zipStream, Encoding.UTF8))
								{
									content = reader.ReadToEnd();
									mJson = JObject.Parse(content);
									break;
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					Trace.WriteLine(
						$" Error reading KiriMoto file: {ex.Message}",
						$"{MessageImportanceEnum.Err}");
				}
				finally
				{
					if(file != null)
					{
						file.IsStreamOwner = true;    //	Close underlying streams.
						file.Close();
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* WriteFile																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Write the contents of the workspace.json file to the specified
		/// KiriMoto Zip file.
		/// </summary>
		/// <param name="filename">
		/// Fully qualified path and filename of the file to write.
		/// </param>
		public void WriteFile(string filename)
		{
			ZipEntry entry = null;

			if(filename?.Length > 0)
			{
				try
				{
					if(File.Exists(filename))
					{
						File.Delete(filename);
					}
					using(FileStream fsOut = File.Create(filename))
					{
						using(ZipOutputStream zipStream = new ZipOutputStream(fsOut))
						{
							//	Compression level (0-9).
							zipStream.SetLevel(3);

							entry = new ZipEntry("workspace.json");
							zipStream.PutNextEntry(entry);
							using(StreamWriter writer = new StreamWriter(zipStream))
							{
								writer.Write(GetJsonContent());
								writer.Flush();
							}
							zipStream.Finish();
							Trace.WriteLine(
								$"KiriMoto file written: {Path.GetFileName(filename)}",
								$"{MessageImportanceEnum.Info}");
						}
					}
				}
				catch(Exception ex)
				{
					Trace.WriteLine($"Error writing KiriMoto file: {ex.Message}",
						$"{MessageImportanceEnum.Err}");
				}
			}
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*

}
