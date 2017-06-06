using Postulate.Mvc;

namespace SampleWebApp.SelectListQueries
{
    public class CustomerTypeSelect : SelectListQuery
    {
        public CustomerTypeSelect() : base(
            @"SELECT [Id] AS [Value], [Name] AS [Text] 
            FROM [dbo].[CustomerType] 
            WHERE [OrganizationId]=@orgId AND [IsActive]=1
            ORDER BY [Name]", 
            "CustomerTypeList", "TypeId")
        {
        }        
    }
}