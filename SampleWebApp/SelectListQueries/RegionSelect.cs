using System;
using Postulate.Mvc;

namespace SampleWebApp.SelectListQueries
{
    public class RegionSelect : SelectListQuery
    {
        public RegionSelect() : base(
            "SELECT [Id] AS [Value], [Name] AS [Text] FROM [dbo].[Region] ORDER BY [Name]", 
            "RegionList", "RegionId")
        {
        }
    }
}