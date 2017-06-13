# Postulate.Mvc

Nuget package coming soon. In the meantime, here is a walkthrough on a more realistic use of [Postulate.Orm](https://github.com/adamosoftware/Postulate.Orm) in an ASP.NET MVC project.

## 1. Create a model class project separate from your web app project

It's not only a good architectural approach, but it simplifies things for the Schema Merge app since only your model classes will be present. I've had strange exceptions trying to use Schema Merge from a web app project, so I advise from the start creating a separate project to hold model classes.

Install the Schema Merge app itself [here](https://github.com/adamosoftware/Postulate.Orm/releases), and if you like, check out my video walkthrough on [Vimeo](https://vimeo.com/219400011). Verify that you can build model classes and that your database exists as expected.

Relevant source:
- [SampleModels/App.config](https://github.com/adamosoftware/Postulate.Mvc/blob/master/SampleModels/App.config) Notice I used "DefaultConnection" as the connection name. This is so it plays nicely with ASP.NET identity.

- [SampleModels/DemoDb2.cs](https://github.com/adamosoftware/Postulate.Mvc/blob/master/SampleModels/DemoDb2.cs) This is my SqlServerDb class, which all subsequent model classes and the Schema Merge app depend on.

- [SampleModels/Customer.cs](https://github.com/adamosoftware/Postulate.Mvc/blob/master/SampleModels/Customer.cs) This is the one model class I'll be working with in these instructions. Note it inherits from [BaseTable](https://github.com/adamosoftware/Postulate.Mvc/blob/master/SampleModels/BaseTable.cs) which will be relevant in a while.

## 2. Make a few tweaks to your ~/Views/Web.config

There are a couple manual additions I recommend to the ~/Views/Web.config file. You want to add a couple namespaces so that references to objects in Razor views can be simplified a bit.

    <add namespace="Sample.Models"/>
    <add namespace="Postulate.Mvc"/>

Most importantly, adding the **Postulate.Mvc** namespace will make it easy to add some useful helpers later.

Relevant source: [Views/Web.config](https://github.com/adamosoftware/Postulate.Mvc/blob/master/SampleWebApp/Views/Web.config)

## 3. Create a controller that inherits from BaseController&lt;TDb, TKey, TProfile&gt;

Postulate.Mvc provides a special controller type that encapsulates common CRUD actions along with some exception handling that makes it simple to execute CRUD actions safely. In addition, it offers a way to standardize access to user profile data via the TProfile type argument, and it has a great way to streamline and standardize how you fill MVC SelectLists.

Relevant source:
- [BaseController.cs](https://github.com/adamosoftware/Postulate.Mvc/blob/master/Postulate.Mvc/BaseController.cs) Shows the low-level implementation of CRUD methods and the related exception handling.

- [CustomerController.cs](https://github.com/adamosoftware/Postulate.Mvc/blob/master/SampleWebApp/Controllers/CustomerController.cs) shows BaseController in use. Note that the code for the actions is pretty minimal. Notice that I use a `Save` action for both inserts and updates. Note that the [Generate](/SampleWebApp/Controllers/CustomerController.cs#L87) action uses my [Test Data Gen](https://github.com/adamosoftware/TestDataGen) library.

- [Customer.cs](https://github.com/adamosoftware/Postulate.Mvc/blob/master/SampleModels/Customer.cs) To test adding delete permissions, I added an `AllowDelete` override to my Customer model class. This is a silly example because it has a hardcoded user name, but it demonstrates how you can check permissions on CRUD actions without adding complexity to your controllers.

## 4. Use [FillSelectLists](https://github.com/adamosoftware/Postulate.Mvc/blob/master/Postulate.Mvc/BaseController.cs#L121) protected method

Postulate.Mvc offers an efficient and powerful way to fill multple SelectLists at a time that uses [Dapper's](https://github.com/StackExchange/Dapper) QueryMultiple method and Postulate.Mvc type [SelectListQuery](https://github.com/adamosoftware/Postulate.Mvc/blob/master/Postulate.Mvc/SelectListQuery.cs).

Have a look at [CustomerController.cs](https://github.com/adamosoftware/Postulate.Mvc/blob/master/SampleWebApp/Controllers/CustomerController.cs#L32) where FillSelectListsInner is called. Notice this is called both by the Edit and Create actions -- this is how the select lists for these views get filled.

    FillSelectListsInner(record, new { orgId = CurrentUser.OrganizationId }, new RegionSelect(), new CustomerTypeSelect());

A few things to say about the FillSelectLists call:

- The `record` argument is where the selected values of all select lists come from. In this case, we're looking at the Customer.TypeId and Customer.RegionId properties -- these are the two properties with dropdown values.

- The `new { orgId = CurrentUser.OrganizationId }` anonymous object is used by Dapper's QueryMultiple method. It uses the protected CurrentUser property to access the user's strongly-typed profile information. One of the queries requires an @orgId parameter, so it is set here.

- The params argument `new RegionSelect(), new CustomerTypeSelect()` are of type [SelectListQuery](https://github.com/adamosoftware/Postulate.Mvc/blob/master/Postulate.Mvc/SelectListQuery.cs). They are the queries themselves called by the method: [CustomerTypeSelect](https://github.com/adamosoftware/Postulate.Mvc/blob/master/SampleWebApp/SelectListQueries/CustomerTypeSelect.cs) and [RegionSelect](https://github.com/adamosoftware/Postulate.Mvc/blob/master/SampleWebApp/SelectListQueries/RegionSelect.cs).

## 5. Create views to go with the CustomerController actions

As a convention, I typically have my **Create** and **Edit** actions share a partial view called **\_Form** that works both for inserts and updates to avoid redundant markup between the two actions: [\_Form](https://github.com/adamosoftware/Postulate.Mvc/blob/master/SampleWebApp/Views/Customer/_Form.cshtml). This is not especially necessary in principle. But to use this approach, I need something to help my controller [Save](https://github.com/adamosoftware/Postulate.Mvc/blob/master/SampleWebApp/Controllers/CustomerController.cs#L32) action tell which action to try again in case a save fails. For that, I use a Postulate.Mvc helper method **Html.ActionNameField()**. (That's why we added "Postulate.Mvc" to the namespaces in the web.config file.)

Note also that my **\_Form** partial view has **TempData["error"]** call near the top. This is what will display any server-side error message once to the user.

Since the [Create](https://github.com/adamosoftware/Postulate.Mvc/blob/master/SampleWebApp/Views/Customer/Create.cshtml) and [Edit](https://github.com/adamosoftware/Postulate.Mvc/blob/master/SampleWebApp/Views/Customer/Edit.cshtml) views share most of the same markup, they are relatively bare on their own.

## What about the Index action?

I didn't mention anything about the [Index](https://github.com/adamosoftware/Postulate.Mvc/blob/master/SampleWebApp/Controllers/CustomerController.cs#L15) action since that relies on another package [Postulate.Sql](https://github.com/adamosoftware/Postulate.Sql). Early on I decided to refactor query capability out of Postulate.Orm since that was not really dependent on CRUD actions. Paginated results and strong-typed queries could be used separately from CRUD, so they ended up in a different package.

Like any typical Index action, mine queries a list for display to the user. Have a look at this [commit](https://github.com/adamosoftware/Postulate.Mvc/commit/1f42413adca245913f8bcaa740f021a724f9d52b#diff-1f2b93ccaf1211720155daf38c87c741) to see how I changed from using an inline query to a strong-typed query using Postulate.Sql. I'm still using inline SQL ultimately, but it's encapsulated in a way that's more testable and less fragile than if I'd embedded directly in the controller. Querying is a big topic, so I'll cover that when I can catch my breath.

