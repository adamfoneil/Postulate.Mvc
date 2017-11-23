namespace SampleWebApp.SelectListQueries
{
    public class OrgSelect : SelectListQueryBase
    {
        public OrgSelect() : base("SELECT [Id] AS [Value], [Name] AS [Text] FROM [dbo].[Organization] ORDER BY [Name]", "OrganizationId")
        {
        }
    }
}