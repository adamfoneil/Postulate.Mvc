using Postulate.Orm.Attributes;
using Postulate.Orm.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Postulate.Orm.Abstract;

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

        public DateTime GetLocalTime()
        {
            return DateTime.Now;
        }

        public override bool AllowSave(IDbConnection connection, SqlDb<int> db, out string message)
        {
            if (!UserName.Equals(db.UserName))
            {
                message = "You may update your own user profile only.";
                return false;
            }

            message = null;
            return true;            
        }
    }
}