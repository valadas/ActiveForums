﻿using System;
using System.Collections.Generic;
using System.Data;

namespace DotNetNuke.Modules.ActiveForums
{
#region RewardInfo
	public class RewardInfo
	{
	    public int RankId { get; set; }

	    public int PortalId { get; set; }

	    public int ModuleId { get; set; }

	    public string RankName { get; set; }

	    public int MinPosts { get; set; }

	    public int MaxPosts { get; set; }

	    public string Display { get; set; }
	}
#endregion
#region RewardController
	public class RewardController
	{
		public RewardInfo Reward_Save(RewardInfo reward)
		{
			int rankId = DataProvider.Instance().Ranks_Save(reward.PortalId, reward.ModuleId, reward.RankId, reward.RankName, reward.MinPosts, reward.MaxPosts, reward.Display);
			return Reward_Get(reward.PortalId, reward.ModuleId, rankId);
		}
		public void Reward_Delete(int PortalId, int ModuleId, int RankId)
		{
			DataProvider.Instance().Ranks_Delete(PortalId, ModuleId, RankId);
		}
		public RewardInfo Reward_Get(int PortalId, int ModuleID, int RankId)
		{
			var ri = new RewardInfo();
			IDataReader dr = DataProvider.Instance().Ranks_Get(PortalId, ModuleID, RankId);
			while (dr.Read())
			{
				ri.Display = dr["Display"].ToString();
				ri.MaxPosts = Convert.ToInt32(dr["MaxPosts"]);
				ri.MinPosts = Convert.ToInt32(dr["MinPosts"]);
				ri.ModuleId = ModuleID;
				ri.PortalId = PortalId;
				ri.RankId = Convert.ToInt32(dr["RankId"]);
				ri.RankName = Convert.ToString(dr["RankName"]);
			}
			dr.Close();
			return ri;
		}
		public List<RewardInfo> Reward_List(int PortalId, int ModuleId, bool UseCache)
		{
			string cacheKey = string.Format(CacheKeys.Rewards, ModuleId.ToString());
			List<RewardInfo> rl;
			if (UseCache)
			{
				rl = (List<RewardInfo>)(DataCache.CacheRetrieve(cacheKey));
				if (rl == null)
				{
					rl = Reward_List(PortalId, ModuleId);
					DataCache.CacheStore(cacheKey, rl);
				}
			}
			else
			{
				rl = Reward_List(PortalId, ModuleId);
			}
			return rl;
		}
		public List<RewardInfo> Reward_List(int PortalId, int ModuleId)
		{
			var rl = new List<RewardInfo>();
			IDataReader dr = DataProvider.Instance().Ranks_List(PortalId, ModuleId);
			dr.Read();
			dr.NextResult();
			while (dr.Read())
			{
				var ri = new RewardInfo
				             {
				                 Display = Convert.ToString(dr["Display"]),
				                 MaxPosts = Convert.ToInt32(dr["MaxPosts"]),
				                 MinPosts = Convert.ToInt32(dr["MinPosts"]),
				                 ModuleId = ModuleId,
				                 PortalId = PortalId,
				                 RankId = Convert.ToInt32(dr["RankId"]),
				                 RankName = Convert.ToString(dr["RankName"])
				             };
			    rl.Add(ri);
			}
			dr.Close();
			return rl;
		}
	}
#endregion


}
