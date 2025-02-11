<Query Kind="Program">
  <Connection>
    <ID>fc06acea-5ea4-4540-bb2f-87d2f386c43e</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Driver Assembly="(internal)" PublicKeyToken="no-strong-name">LINQPad.Drivers.EFCore.DynamicDriver</Driver>
    <AllowDateOnlyTimeOnly>true</AllowDateOnlyTimeOnly>
    <Server>MOMSDESKTOP\SQLEXPRESS</Server>
    <Database>OLTP-DMIT2018</Database>
    <DisplayName>OLTP-DMIT2018-Entity</DisplayName>
    <DriverData>
      <EncryptSqlTraffic>True</EncryptSqlTraffic>
      <PreserveNumeric1>True</PreserveNumeric1>
      <EFProvider>Microsoft.EntityFrameworkCore.SqlServer</EFProvider>
    </DriverData>
  </Connection>
</Query>

//	Driver is responsible for orchestrating the flow by calling 
//	various methods and classes that contain the actual business logic 
//	or data processing operations.
void Main()
{
	//Header Information
	Console.WriteLine("=====================");
	Console.WriteLine("==== Get Category Pass ====");
	Console.WriteLine("=====================");
	//Pass
	TestGetCategory("Province").Dump("Pass - Valid Category Name");
	TestGetCategory("People").Dump("Pass - Valid Input - No Category Found");

	//Header Information
	Console.WriteLine("=====================");
	Console.WriteLine("==== Get Category Fail ====");
	Console.WriteLine("=====================");
	TestGetCategory(null).Dump("Fail - Category name was null.");
	TestGetCategory("  ").Dump("Fail - Category name was whitespace.");
	TestGetCategory(string.Empty).Dump("Fail - Category name was empty.");
	
}

//	This region contains methods used for testing the functionality
//	of the application's business logic and ensuring correctness.
#region Test Methods
public CategoryView TestGetCategory(string categoryName)
{
	try 
	{
		var results = GetCategory(categoryName);
		if(results == null)
			return new CategoryView();
		else
			return results;
	}
	catch(AggregateException ex)
	{
		foreach (var error in ex.InnerExceptions)
		{
			error.Message.Dump();
		}
	}
	catch(ArgumentNullException ex)
	{
		GetInnerException(ex).Message.Dump();
	}
	catch(Exception ex)
	{
		GetInnerException(ex).Message.Dump();
	}
	return null;
}
#endregion

//	This region contains support methods for testing
#region Support Methods
public Exception GetInnerException(System.Exception ex)
{
	while (ex.InnerException != null)
		ex = ex.InnerException;
	return ex;
}
#endregion

//	This region contains all methods responsible 
//	for executing business logic and operations.
#region Methods
public CategoryView GetCategory(string categoryName)
{
	#region Business Logic
	List<Exception> errorList = [];
	//rule: category name is required
	if(string.IsNullOrWhiteSpace(categoryName))
		throw new ArgumentNullException("Category name is required.");
	#endregion
	
	return Categories
			.Where(x => x.CategoryName == categoryName
					&& !x.RemoveFromViewFlag)
			.Select(x => new CategoryView
			{
				CategoryID = x.CategoryID,
				CategoryName = x.CategoryName,
				RemoveFromViewFlag = x.RemoveFromViewFlag
			}).FirstOrDefault();
}
#endregion

//	This region includes the view models used to 
//	represent and structure data for the UI.
#region View Models
public class CategoryView
{
	public int CategoryID { get; set; }
	public string CategoryName { get; set; }
	public bool RemoveFromViewFlag { get; set; }
}
#endregion

