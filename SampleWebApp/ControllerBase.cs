using Postulate.Mvc;
using Sample.Models;
using System.Web.Mvc;
using static SampleWebApp.Controllers.ManageController;

namespace SampleWebApp
{
    public class ControllerBase : BaseProfileController<DemoDb, int, UserProfile>
    {
        public ControllerBase()
        {
            ProfileRule = (record) => record.OrganizationId != 0;
        }

        protected override ActionResult ProfileUpdateRedirect => RedirectToAction("Index", "Manage", new { message = ManageMessageId.ProfileMissing });
    }
}