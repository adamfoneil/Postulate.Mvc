using Postulate.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SampleWebApp.SelectListQueries
{
    public class OrgSelect : SelectListQuery
    {
        public OrgSelect() : base("SELECT [Id] AS [Value], [Name] AS [Text] FROM [dbo].[Organization] ORDER BY [Name]", "OrgSelect", "OrganizationId")
        {
        }
    }
}