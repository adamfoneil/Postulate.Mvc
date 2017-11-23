using Postulate.Orm.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Sample.Models
{
    [DereferenceExpression("[Name]")]
    public class CustomerType : BaseTable
    {
        [PrimaryKey]
        [ForeignKey(typeof(Organization))]
        public int OrganizationId { get; set; }

        [MaxLength(50)]
        [PrimaryKey]
        public string Name { get; set; }

        public bool IsActive { get; set; } = true;
    }
}