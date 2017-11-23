using Postulate.Mvc;
using Sample.Models;

namespace SampleWebApp.SelectListQueries
{
    public class SelectListQueryBase : SelectListQuery
    {
        public SelectListQueryBase(string sql, string valueProperty) : base(sql, valueProperty, new DemoDb())
        {
        }
    }
}