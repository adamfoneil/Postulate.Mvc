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
        [Required]
        [ColumnAccess(Access.InsertOnly)]
        public string CreatedBy { get; set; }

        [ColumnAccess(Access.InsertOnly)]
        public DateTime DateCreated { get; set; } = DateTime.Now;

        [MaxLength(50)]
        [ColumnAccess(Access.UpdateOnly)]
        public string ModifiedBy { get; set; }

        [ColumnAccess(Access.UpdateOnly)]
        public DateTime? DateModified { get; set; }

        public override void BeforeSave(IDbConnection connection, SqlDb<int> db, SaveAction action)
        {
            switch (action)
            {
                case SaveAction.Insert:
                    CreatedBy = db.UserName;
                    break;

                case SaveAction.Update:
                    ModifiedBy = db.UserName;
                    DateModified = DateTime.Now;
                    break;
            }
        }
    }
}