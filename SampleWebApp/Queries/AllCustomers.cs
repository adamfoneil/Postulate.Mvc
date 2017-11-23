using Postulate.Orm.Attributes;
using System;

namespace SampleWebApp.Queries
{
    public class ViewCustomer
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public int TypeId { get; set; }
        public int RegionId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? DateModified { get; set; }
        public string Address { get; set; }
        public string CustomerTypeName { get; set; }
        public string RegionName { get; set; }
    }

    public class AllCustomers : DemoDbQuery<ViewCustomer>
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