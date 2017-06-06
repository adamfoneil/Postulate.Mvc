using Postulate.Orm.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Postulate.Orm.Enums;
using System.Data;
using Postulate.Orm.Attributes;

namespace Sample.Models
{
    [ColumnAccess("OrganizationId", Access.InsertOnly)]
    public abstract class BaseTable : Record<int>
    {
        [MaxLength(50)]
        public string CreatedBy { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.Now;

        [MaxLength(50)]
        public string ModifiedBy { get; set; }

        public DateTime? DateModified { get; set; }

        public override void BeforeSave(IDbConnection connection, string userName, SaveAction action)
        {
            switch (action)
            {
                case SaveAction.Insert:
                    CreatedBy = userName;
                    break;

                case SaveAction.Update:
                    ModifiedBy = userName;
                    DateModified = DateTime.Now;
                    break;
            }
        }
    }
}