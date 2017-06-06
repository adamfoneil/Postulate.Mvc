﻿using Postulate.Orm.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;

namespace Sample.Models
{
    public class Customer : BaseTable
    {
        [ForeignKey(typeof(Organization))]
        public int OrganizationId { get; set; }

        [MaxLength(100)]
        [Required]
        public string LastName { get; set; }

        [MaxLength(100)]
        [Required]
        public string FirstName { get; set; }

        [ForeignKey(typeof(CustomerType))]
        public int TypeId { get; set; }

        [ForeignKey(typeof(Region))]
        public int RegionId { get; set; }

        public override bool AllowDelete(IDbConnection connection, string userName, out string message)
        {
            if (userName.Equals("adamosoftware@gmail.com"))
            {
                message = "Adam is not allowed to delete customers.";
                return false;
            }

            message = null;
            return true;
        }
    }
}