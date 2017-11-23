using Postulate.Orm.Attributes;
using Postulate.Orm.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;

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

        [DefaultExpression("0")]
        public bool TrackNavigation { get; set; }

        public DateTime GetLocalTime(IDbConnection connection)
        {
            return DateTime.Now;
        }
    }
}