using Postulate.Orm.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Models
{
    [DereferenceExpression("[Name]")]
    public class Region : BaseTable
    {
        [MaxLength(100)]
        public string Name { get; set; }
    }
}
