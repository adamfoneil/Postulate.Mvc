using Postulate.Orm.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Sample.Models
{
    [DereferenceExpression("[Name]")]
    public class Region : BaseTable
    {
        [MaxLength(100)]
        public string Name { get; set; }
    }
}