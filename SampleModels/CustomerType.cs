using Postulate.Orm.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Models
{
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
