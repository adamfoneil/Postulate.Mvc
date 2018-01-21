using Postulate.Mvc;
using Sample.Models;
using System.Web.Mvc;
using static SampleWebApp.Controllers.ManageController;
using System;

namespace SampleWebApp
{
    public class ControllerBase : ProfileControllerBase<DemoDb, int, UserProfile>
    {
        public ControllerBase()
        {
            ExceptionView = "/Views/Shared/Error.chstml";
        }

        protected override ActionResult ProfileUpdateRedirect => RedirectToAction("Index", "Manage", new { message = ManageMessageId.ProfileMissing });

        protected override Func<UserProfile, bool> ProfileRule => (record) => record.OrganizationId != 0;        
    }
}