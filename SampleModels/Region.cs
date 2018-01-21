using Postulate.Orm.Attributes;
using Postulate.Orm.Abstract;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Sample.Models
{
    [DereferenceExpression("[Name]")]
    public class Region : BaseTable
    {
        [MaxLength(100)]
        public string Name { get; set; }

        //[MaxLength(10)]
        //public string Code { get; set; }
    }

    public class RegionSeedData : SeedData<Region, int>
    {
        public override string ExistsCriteria => "[dbo].[Region] WHERE [Name]=@name";

        public override IEnumerable<Region> Records => new Region[]
        {
            new Region() { Name = "North" },
            new Region() { Name = "South" },
            new Region() { Name = "East" },
            new Region() { Name = "West" }
        };
    }
}