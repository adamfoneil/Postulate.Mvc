using Postulate.Orm.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Sample.Models
{
    [DereferenceExpression("[Name]")]
    public class Organization : BaseTable
    {
        [MaxLength(100)]
        public string Name { get; set; }
    }
}
