using Dapper;
using Postulate.Orm.Abstract;
using Postulate.Orm.Attributes;
using Postulate.Orm.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;

namespace Sample.Models
{
    [TrackChanges(IgnoreProperties = "DateModified,ModifiedBy")]
    public class UserProfile : BaseTable, IUserProfile
    {
        [MaxLength(100)]
        [PrimaryKey]
        [ColumnAccess(Access.InsertOnly)]
        public string UserName { get; set; }

        [Postulate.Orm.Attributes.ForeignKey(typeof(Organization))]
        public int OrganizationId { get; set; }

        [DefaultExpression("0")]
        public bool TrackNavigation { get; set; }

        public DateTime GetLocalTime()
        {
            return DateTime.Now;
        }

        [NotMapped]
        public string[] SystemRoles { get; set; }

        public override void BeforeView(IDbConnection connection, SqlDb<int> db)
        {
            SystemRoles = GetSytemRoles(connection);
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

        public string[] GetSytemRoles(IDbConnection connection)
        {
            return connection.Query<string>(
                @"SELECT
                    [r].[Name]
                FROM
                    [dbo].[AspNetUserRoles] [ur] INNER JOIN [dbo].[AspNetUsers] [u] ON [u].[Id]=[ur].[UserId]
                    INNER JOIN [dbo].[AspNetRoles] [r] ON [ur].[RoleId]=[r].[Id]
                WHERE
                    [u].[UserName]=@name", new { name = UserName }).ToArray();
        }

        public bool HasRole(IDbConnection connection, string roleName)
        {
            return GetSytemRoles(connection).Contains(roleName);
        }
    }
}