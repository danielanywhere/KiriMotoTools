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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ActionEngine;
using Newtonsoft.Json;
using StyleAgnosticCommandArgs;

using static KiriMotoTools.KiriMotoToolsUtil;

namespace KiriMotoTools
{
	//*-------------------------------------------------------------------------*
	//*	Program																																	*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Main instance of the KiriMotoTools application.
	/// </summary>
	public class Program
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
		//*	_Main																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Configure and run the application.
		/// </summary>
		public static async Task Main(string[] args)
		{
			string actionName = "";
			bool bActivity = false;
			bool bShowHelp = false; //	Flag - Explicit Show Help.
			CommandArgCollection commandArgs = null;
			string key = "";        //	Current Parameter Key.
			string lowerArg = "";   //	Current Lowercase Argument.
			StringBuilder message = new StringBuilder();
			NameValueCollection nameValues = null;
			Program prg = new Program();  //	Initialized instance.

			ConsoleTraceListener consoleListener = new ConsoleTraceListener();
			Trace.Listeners.Add(consoleListener);

			Console.WriteLine("KiriMotoTools.exe");

			KiriMotoActionItem.RecognizedActions.AddRange(new string[]
			{
				"AutoPocket",
				"CreateCAMProject",
				"OptimizeGCode",
				"Outline",
				"ReportGCodeOverlap",
				"TaskList"
			});

			prg.mActionItem = new KiriMotoActionItem();

			commandArgs = new CommandArgCollection(args);
			foreach(CommandArgItem argItem in commandArgs)
			{
				key = argItem.Name.ToLower();
				switch(key)
				{
					case "":
						key = argItem.Value.ToLower();
						switch(key)
						{
							case "?":
								bShowHelp = true;
								break;
							case "wait":
								prg.mWaitAfterEnd = true;
								break;
						}
						break;
					case "action":
						actionName = KiriMotoActionItem.GetActionName(argItem.Value);
						if(actionName != "None")
						{
							prg.ActionItem.Action = actionName;
							bActivity = true;
						}
						else
						{
							message.Append("Error: No action specified...");
							bShowHelp = true;
						}
						break;
					case "configfile":
						prg.ActionItem.ConfigFilename = argItem.Value;
						break;
					case "infile":
						prg.ActionItem.InputNames.Add(argItem.Value);
						break;
					case "option":
						prg.ActionItem.Options.Add(argItem.Value);
						break;
					case "outfile":
						prg.ActionItem.OutputFilename = argItem.Value;
						break;
					case "properties":
						try
						{
							nameValues = JsonConvert.DeserializeObject<NameValueCollection>(
							 argItem.Value);
							foreach(NameValueItem propertyItem in nameValues)
							{
								prg.mActionItem.Properties.Add(propertyItem);
							}
						}
						catch(Exception ex)
						{
							Console.WriteLine($"Error parsing properties: {ex.Message}");
							bShowHelp = true;
						}
						break;
					case "tool":
						prg.ActionItem.Tool = argItem.Value;
						break;
					case "workingpath":
						prg.ActionItem.WorkingPath = argItem.Value;
						break;
				}
			}
			if(!bShowHelp && !bActivity)
			{
				message.AppendLine(
				 "Please specify an action or a stand-alone activity.");
				bShowHelp = true;
			}
			if(bShowHelp)
			{
				// Display Syntax.
				Console.WriteLine(message.ToString() + "\r\n" + ResourceMain.Syntax);
			}
			else
			{
				// Run the configured application.
				await prg.Run();
			}
			if(prg.mWaitAfterEnd)
			{
				Console.WriteLine("Press [Enter] to exit...");
				Console.ReadLine();
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ActionItem																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="ActionItem">ActionItem</see>.
		/// </summary>
		private KiriMotoActionItem mActionItem = null;
		/// <summary>
		/// Get/Set a reference to the main Kiri:Moto action for this session.
		/// </summary>
		public KiriMotoActionItem ActionItem
		{
			get { return mActionItem; }
			set { mActionItem = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Run																																		*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Run the configured application.
		/// </summary>
		public async Task Run()
		{
			string inputFilename = "";
			KiriMotoZipDocumentItem kiriMotoDocument = null;
			string outputFilename = "";

			if(!ActionEngine.ActionEngineUtil.ActionIsNone(mActionItem.Action))
			{
				if(mActionItem.InputNames.Count > 0)
				{
					inputFilename = mActionItem.InputNames[0];
					if(inputFilename.Length > 0)
					{
						Trace.WriteLine($"Input filename: {inputFilename}",
							$"{MessageImportanceEnum.Info}");
					}
					if(mActionItem.InputNames[0].ToLower().EndsWith(".kmz"))
					{
						kiriMotoDocument = new KiriMotoZipDocumentItem(
							ActionEngine.ActionEngineUtil.AbsolutePath(
								mActionItem.WorkingPath, mActionItem.InputNames[0]));
						mActionItem.WorkingDocument = kiriMotoDocument;
						mActionItem.WorkingJson = kiriMotoDocument.Json;
					}
				}
				await mActionItem.Run();
				//	In this version, the output filename must be specified explicitly.
				if(mActionItem.WorkingDocument != null &&
					mActionItem.WorkingDocument is KiriMotoZipDocumentItem kiriDocument)
				{
					if(kiriDocument.Modified == true)
					{
						if(mActionItem.OutputFilename.Length > 0)
						{
							outputFilename = mActionItem.OutputFilename;
						}
						else
						{
							outputFilename = mActionItem.Actions.FirstOrDefault(f =>
								f.OutputFilename.Length > 0)?.OutputFilename;
						}
						if(outputFilename.Length > 0)
						{
							kiriDocument.WriteFile(
								ActionEngineUtil.AbsolutePath(
									mActionItem.WorkingPath, outputFilename));
						}
						else
						{
							Trace.WriteLine("Could not save Kiri document. " +
								"Output filename was not specified.",
								$"{MessageImportanceEnum.Info}");
						}
					}
					else
					{
						Trace.WriteLine("No changes were made to the KiriMoto document.",
							$"{MessageImportanceEnum.Info}");
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	WaitAfterEnd																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="WaitAfterEnd">WaitAfterEnd</see>.
		/// </summary>
		private bool mWaitAfterEnd = false;
		/// <summary>
		/// Get/Set a value indicating whether to wait for user keypress after
		/// processing has completed.
		/// </summary>
		public bool WaitAfterEnd
		{
			get { return mWaitAfterEnd; }
			set { mWaitAfterEnd = value; }
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
