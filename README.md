# Postulate.Mvc

Noget package coming soon. In the meantime, here is a walkthrough on a more realistic use of Postulate.Orm in an MVC project.

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

## 3. Create a controller that inherits from SqlServerDbController&lt;TDb, TKey&gt;

Postulate.Mvc provides a special controller type that encapsulates common CRUD actions along with some exception handling that makes it simple to execute CRUD actions safely.
