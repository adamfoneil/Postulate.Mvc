using System;
using System.Data;
using Postulate.Orm.Abstract;
using Postulate.Orm.Interfaces;
using Postulate.Orm.Attributes;

namespace Sample.Models
{
    public class UserProfile : Record<int>, IUserProfile
    {
        public string UserName { get; set; }

        [ForeignKey(typeof(Organization))]
        public int OrganizationId { get; set; }

        public DateTime GetLocalTime(IDbConnection connection)
        {
            return DateTime.Now;
        }
    }
}
