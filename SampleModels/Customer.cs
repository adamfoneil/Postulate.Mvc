using Postulate.Orm.Abstract;
using Postulate.Orm.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace Sample.Models
{
    [TrackChanges(IgnoreProperties = "ModifiedBy,DateModified")]
    public class Customer : BaseTable
    {
        [Postulate.Orm.Attributes.ForeignKey(typeof(Organization))]
        public int OrganizationId { get; set; }

        [MaxLength(100)]
        [Required]
        public string LastName { get; set; }

        [MaxLength(100)]
        [Required]
        public string FirstName { get; set; }

        [MaxLength(100)]
        public string Address { get; set; }

        [MaxLength(50)]
        public string City { get; set; }

        [MaxLength(2)]
        public string State { get; set; }

        [MaxLength(5)]
        public string ZipCode { get; set; }

        [Postulate.Orm.Attributes.ForeignKey(typeof(CustomerType))]
        public int TypeId { get; set; }

        [Postulate.Orm.Attributes.ForeignKey(typeof(Region))]
        public int RegionId { get; set; }

        [NotMapped]
        public string CustomerTypeName { get; set; }

        [NotMapped]
        public string RegionName { get; set; }


        public override bool AllowDelete(IDbConnection connection, SqlDb<int> db, out string message)
        {
            if (db.UserName.Equals("adamosoftware@gmail.com"))
            {
                message = "Adam is not allowed to delete customers.";
                return false;
            }

            message = null;
            return true;
        }
    }
}