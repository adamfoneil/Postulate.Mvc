using System;
using Postulate.Mvc;

namespace SampleWebApp.SelectListQueries
{
    public class RegionSelect : SelectListQueryBase
    {
        public RegionSelect(string valueProperty) : base(
            "SELECT [Id] AS [Value], [Name] AS [Text] FROM [dbo].[Region] ORDER BY [Name]", 
            valueProperty)
        {
        }
    }
}