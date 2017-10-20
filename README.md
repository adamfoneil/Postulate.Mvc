# Postulate.Mvc

Nuget package: **Postulate.Mvc**

This library is a set of base classes and helpers for improving productivity in ASP.NET MVC5 when used with [Postulate.Orm](https://github.com/adamosoftware/Postulate.Orm).

- [BaseController](/Postulate.Mvc/BaseController.cs) enhances your controllers by offering
    - data access through the [Db](/Postulate.Mvc/BaseController.cs#L22) property as well as [SaveRecord](/Postulate.Mvc/BaseController.cs#L55), [DeleteRecord](/Postulate.Mvc/BaseController.cs#L72), and [UpdateRecord](/Postulate.Mvc/BaseController.cs#L38) methods. These can greatly simplify typical Controller CRUD actions if you're using Postulate.Orm.
    - convenient and efficient drop down list filling with the [FillSelectLists](/Postulate.Mvc/BaseController.cs#L128) method and its several overloads. FillSelectLists executes multiple list-filling queries in one round trip using Dapper's QueryMultiple method.
    - access to Json-based user profile data with the [LoadUserData](/Postulate.Mvc/BaseController.cs#L212) method.
    
- [BaseProfileController](/Postulate.Mvc/BaseProfileController.cs) builds on BaseController by integrating strong-typed access to your user profile class within your controller. Just about any web app requires user profile data at the controller level. Inherit from this class to incorporate user profile data directly into your controllers. Your user profile model class must implement [IUserProfile](https://github.com/adamosoftware/Postulate.Orm/blob/master/PostulateV1/Interfaces/IUserProfile.cs). In addition, this class offers
    - profile validation with [ProfileRule](/Postulate.Mvc/BaseProfileController.cs#L27) property. Specify a rule that describes a well-formed user profile. For example, in a multi-tenant system, you might need to ensure that all users have a tenant selected. By setting the ProfileRule for the controller, users are automatically redirected to the profile page of your choice (set by the [ProfileUpdateUrl](/Postulate.Mvc/BaseProfileController.cs#L32) property) to complete their setup if necessary.
    - access to user profile data through the [CurrentUser](/Postulate.Mvc/BaseProfileController.cs#L22) property.

- [HtmlHelpers](/Postulate.Mvc/Extensions/HtmlHelpers.cs) provides some miscellaneous helpers. One in particular [ActionNameField](/Postulate.Mvc/Extensions/Helpers.cs#12) passes the current action name to a controller for easy redirect back to a failing page in case of an error.

- [SelectListQuery](/Postulate.Mvc/SelectListQuery.cs) describes a query used to fill a drop down list, and is used by BaseController.FillSelectLists.

## What's the difference between BaseController.LoadUserData and BaseProfileController?

BaseController.LoadUserData deserializes a class from a Json file from **~/App_Data/UserData/{userName}**. This is handy when you have user settings that you're too lazy to store in the database because they're too specific, complex, or rarely used. Just be sure you preserve the ~/App_Data folder when publishing! BaseProfileController is preferred for accessing user profile data that is universal in your app and that is strong-typed in the database.
