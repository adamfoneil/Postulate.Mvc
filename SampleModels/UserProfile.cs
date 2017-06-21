using System;
using System.Data;
using Postulate.Orm.Interfaces;
using Postulate.Orm.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Sample.Models
{
    [TrackChanges]
    public class UserProfile : BaseTable, IUserProfile
    {
        [MaxLength(100)]
        [PrimaryKey]
        [ColumnAccess(Access.InsertOnly)]
        public string UserName { get; set; }

        [ForeignKey(typeof(Organization))]
        public int OrganizationId { get; set; }

        public DateTime GetLocalTime(IDbConnection connection)
        {
            return DateTime.Now;
        }
    }
}
