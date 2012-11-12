﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using DotNetNuke.Web.Services;
using System.Web.Mvc;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Content.Common;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using DotNetNuke.Services.Localization;
using System.Text;
//ORIGINAL LINE: Imports System.Web.HttpContext

using System.Web;
namespace DotNetNuke.Modules.ActiveForums
{
	[ValidateAntiForgeryToken()]
	public class AdminServiceController : DnnController
	{


		[DnnAuthorize()]
		public string ToggleURLHandler(int ModuleId)
		{
			Entities.Modules.ModuleController objModules = new Entities.Modules.ModuleController();
			SettingsInfo objSettings = new SettingsInfo();
			objSettings.MainSettings = objModules.GetModuleSettings(ModuleId);
			ConfigUtils cfg = new ConfigUtils();
			bool success = false;
			if (Utilities.IsRewriteLoaded())
			{
				success = cfg.DisableRewriter(Server.MapPath("~/web.config"));
				return "disabled";
			}
			else
			{
				success = cfg.EnableRewriter(Server.MapPath("~/web.config"));
				return "enabled";
			}

		}

		[DnnAuthorize()]
		public ActionResult RunMaintenance(int ModuleId, int ForumId, int olderThan, int byUserId, int lastActive, bool withNoReplies, bool dryRun) {
			Entities.Modules.ModuleController objModules = new Entities.Modules.ModuleController();
			SettingsInfo objSettings = new SettingsInfo();
			objSettings.MainSettings = objModules.GetModuleSettings(ModuleId);
			int rows = DataProvider.Instance().Forum_Maintenance(ForumId, olderThan, lastActive, byUserId, withNoReplies, dryRun, objSettings.DeleteBehavior);
			if (dryRun) {
			   
				return Json(new {Result = string.Format(Utilities.GetSharedResource("[RESX:Maint:DryRunResults]", true), rows.ToString())});
			
			} else {
				return Json(new { Result = Utilities.GetSharedResource("[RESX:ProcessComplete]", true) });
			}
		}

		[DnnAuthorize()]
		public string GetSecurityGrid(int GroupId, int ForumId)
		{
			StringBuilder sb = new StringBuilder();



			return sb.ToString();
		}

		[DnnAuthorize(), AcceptVerbs(HttpVerbs.Post)]
		public string ToggleSecurity(int ModuleId, string Action, int PermissionsId, string SecurityId, int SecurityType, string SecurityKey, string ReturnId)
		{
			Data.Common db = new Data.Common();
			StringBuilder sb = new StringBuilder();
			switch (Action)
			{
				case "delete":
				{
					Permissions.RemoveObjectFromAll(SecurityId, SecurityType, PermissionsId);
					return string.Empty;
				}
				case "addobject":
				{
					if (SecurityType == 1)
					{
						UserController uc = new UserController();
						User ui = uc.GetUser(PortalSettings.PortalId, ModuleId, SecurityId);
						if (ui != null)
						{
							SecurityId = ui.UserId.ToString();
						}
						else
						{
							SecurityId = string.Empty;
						}
					}
					else
					{
						if (SecurityId.Contains(":"))
						{
							SecurityType = 2;
						}
					}
					if (! (string.IsNullOrEmpty(SecurityId)))
					{
						string permSet = db.GetPermSet(PermissionsId, "View");
						permSet = Permissions.AddPermToSet(SecurityId, SecurityType, permSet);
						db.SavePermSet(PermissionsId, "View", permSet);
					}
					return string.Empty;
				}
				default:
				{
					string permSet = db.GetPermSet(PermissionsId, SecurityKey);
					if (Action == "remove")
					{
						permSet = Permissions.RemovePermFromSet(SecurityId, SecurityType, permSet);
					}
					else
					{
						permSet = Permissions.AddPermToSet(SecurityId, SecurityType, permSet);
					}
					db.SavePermSet(PermissionsId, SecurityKey, permSet);
					return Action + "|" + ReturnId.ToString();
				}
			}



		}

	}
}
