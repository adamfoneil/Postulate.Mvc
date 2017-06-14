using System.Data;
using System.Web.Mvc;
using Postulate.Mvc;
using Sample.Models;

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

        /// <summary>
        /// If an inactive customer type was used, this will get it in order to populate the dropdown -- otherwise, the field will look empty in the view
        /// </summary>
        public override SelectListItem GetMissingItem(IDbConnection connection, object id)
        {
            var item = new DemoDb().Find<CustomerType>((int)id);
            return new SelectListItem() { Value = item.Id.ToString(), Text = item.Name };
        }
    }
}