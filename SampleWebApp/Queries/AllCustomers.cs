using Postulate.Orm.Attributes;
using Sample.Models;

namespace SampleWebApp.Queries
{
    public class AllCustomers : DemoDbQuery<Customer>
    {
        public AllCustomers() : base(
            @"SELECT
                [c].*,
                [ct].[Name] AS [CustomerTypeName],
                [r].[Name] AS [RegionName]
            FROM
                [dbo].[Customer] [c]
                INNER JOIN [dbo].[CustomerType] AS [ct] ON [c].[TypeId]=[ct].[Id]
                INNER JOIN [dbo].[Region] AS [r] ON [c].[RegionId]=[r].[Id]
            WHERE
                [c].[OrganizationId]=@orgId {andWhere}
            ORDER BY
                [LastName], [FirstName]")
        {
        }

        public int OrgId { get; set; }

        [Where("[LastName] LIKE '%'+@lastName+'%'")]
        public string LastName { get; set; }

        public int? TypeId { get; set; }

        public int? RegionId { get; set; }
    }
}