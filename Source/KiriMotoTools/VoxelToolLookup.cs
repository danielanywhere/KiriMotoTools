using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiriMotoTools
{
	//*-------------------------------------------------------------------------*
	//*	VoxelToolLookupCollection																								*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of VoxelToolLookupItem Items.
	/// </summary>
	public class VoxelToolLookupCollection : List<VoxelToolLookupItem>
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
		//* GetToolLookup																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the voxel tool lookup record corresponding to the provided
		/// mill type and tool diameter.
		/// </summary>
		/// <param name="millType">
		/// The mill type of the tool info to retrieve.
		/// </param>
		/// <param name="toolDiameter">
		/// The diameter of the tool, in natural units (mm or in).
		/// </param>
		/// <returns>
		/// Reference to the voxel tool lookup record corresponding to the
		/// specified mill type and tool diameter.
		/// </returns>
		public VoxelToolLookupItem GetToolLookup(MillTypeEnum millType,
			float toolDiameter)
		{
			float multiplier = (GCodeVectorItem.VoxelPrecision != 0f ?
				1f / GCodeVectorItem.VoxelPrecision : 0f);
			int offsetX = 0;
			int offsetY = 0;
			int offsetZ = 0;
			VoxelToolLookupItem result = null;
			int toolRadius = 0;
			int trsq = 0;

			if(millType != MillTypeEnum.None && toolDiameter > 0f)
			{
				result = this.FirstOrDefault(x =>
					x.MillType == millType && x.ToolDiameter == toolDiameter);
				if(result == null)
				{
					result = new VoxelToolLookupItem()
					{
						MillType = millType,
						ToolDiameter = toolDiameter
					};
					toolRadius =
						(int)Math.Ceiling((toolDiameter * multiplier) / 2f);
					trsq = toolRadius * toolRadius;
					for(offsetX = -toolRadius; offsetX <= toolRadius; offsetX ++)
					{
						for(offsetY = -toolRadius; offsetY <= toolRadius; offsetY ++)
						{
							for(offsetZ = -toolRadius; offsetZ <= toolRadius; offsetZ ++)
							{
								//	The tips of all tools touch at offsetZ == -toolRadius.
								switch(millType)
								{
									case MillTypeEnum.Ball:
										if((offsetX * offsetX) +
											(offsetY * offsetY) +
											(offsetZ * offsetZ) <= trsq)
										{
											//	The center of the ball sphere is
											//	{toolRadius} above the tip.
											result.RelativeVoxels.Add(new IVector3()
											{
												X = offsetX,
												Y = offsetY,
												Z = offsetZ + toolRadius
											});
										}
										break;
									case MillTypeEnum.End:
										if((offsetX * offsetX) +
											(offsetY * offsetY) <= trsq)
										{
											result.RelativeVoxels.Add(new IVector3()
											{
												X = offsetX,
												Y = offsetY,
												Z = offsetZ + toolRadius
											});
										}
										break;
									case MillTypeEnum.Vee:
										//	TODO: V tip not yet implemented.
										break;
								}
							}
						}
					}
					this.Add(result);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	VoxelToolLookupItem																											*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Lookup information for voxel coverage by an individual tool.
	/// </summary>
	public class VoxelToolLookupItem
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
		//*	MillType																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="MillType">MillType</see>.
		/// </summary>
		private MillTypeEnum mMillType = MillTypeEnum.End;
		/// <summary>
		/// Get/Set the mill type for this tool.
		/// </summary>
		public MillTypeEnum MillType
		{
			get { return mMillType; }
			set { mMillType = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	RelativeVoxels																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="RelativeVoxels">RelativeVoxels</see>.
		/// </summary>
		private List<IVector3> mRelativeVoxels = new List<IVector3>();
		/// <summary>
		/// Get a reference to the collection of relative voxel positions to look
		/// up for the shape of this tool.
		/// </summary>
		public List<IVector3> RelativeVoxels
		{
			get { return mRelativeVoxels; }
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
		/// Get/Set the flute diameter of the tool.
		/// </summary>
		public float ToolDiameter
		{
			get { return mToolDiameter; }
			set { mToolDiameter = value; }
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
