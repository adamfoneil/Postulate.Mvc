using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Models
{
    public class Organization : BaseTable
    {
        [MaxLength(100)]
        public string Name { get; set; }
    }
}
