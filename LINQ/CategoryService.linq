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

//For a Add/Insert we are returning the PK of the main new record.
public int AddCategory(CategoryView categoryView)
{
	List<Exception> errorList = [];
	#region Business Rules
	//rule: Category View cannot be null
	if (categoryView == null)
		throw new ArgumentNullException("No category was supplied");
	//Call the Validation Method to return the list of errors
	//May return an empty list if there are no errors. (this is good)
	errorList = ValidateCategory(categoryView);
	#endregion

	//create a new category to transfer the information from our view to the category record.
	Category category = new();
	//Transfer the information from the View Model to the new record
	//We never update or add the Primary Key
	//Most of the time the database will automatically create the PK
	//Examples: Integer incrementing, new GUID
	//When updating, you can't and should never update the PK of a record.
	category.CategoryName = categoryView.CategoryName;
	//We can update/add the logical delete flag in case this record needs to be deleted.
	category.RemoveFromViewFlag = categoryView.RemoveFromViewFlag;

	//Check for errors, are there any errors in the list:
	//NOTE: YOU SHOULD ONLY HAVE ONE CHECK FOR ERRORS IN ORDER TO ROLL BACK IN A METHOD
	if (errorList.Count > 0)
	{
		//RollBack
		ChangeTracker.Clear();
		throw new AggregateException("Unable to add the category, please check error message(s)", errorList);
	}
	else
	{
		//Save the changes locally
		Categories.Add(category);
		//Remember to use a try/catch around the SaveChanges to throw a nice exception
		try
		{
			SaveChanges();
			//For an insert/add always return the new PK for the record.
			return category.CategoryID;
		}
		catch (Exception ex)
		{
			throw new Exception($"An error occured while saving: {ex.Message}", ex);
		}
	}
}

//For an edit we are returning the number of records that were changed
public int EditCategory(CategoryView categoryView)
{
	List<Exception> errorList = [];
	//Insert Business Logic
	//rule: Category View cannot be null
	if (categoryView == null)
		throw new ArgumentNullException("No category was supplied");
	ValidateCategoryWithOut(categoryView, out errorList);
	//Find the existing category
	//We will have the PK in order to search
	Category category = Categories
									.Where(x => x.CategoryID == categoryView.CategoryID
											&& !x.RemoveFromViewFlag)
									.FirstOrDefault();
	if (category == null)
	{
		errorList.Add(new InvalidOperationException($"A Category with the Category ID: {categoryView.CategoryID} and Category Name: {categoryView.CategoryName} cannot be found or has been deleted. Cannot update the record."));
	}
	else
	{
		category.CategoryName = categoryView.CategoryName;
		category.RemoveFromViewFlag = categoryView.RemoveFromViewFlag;
	}
	if(errorList.Count > 0)
	{
		//RollBack
		ChangeTracker.Clear();
		throw new AggregateException("Unable to add the category, please check error message(s)", errorList);
	}
	else
	{
		Categories.Update(category);
		try
		{
			//For an update we return SaveChanges as this returns the number of records that were changes.
			//So we can ensure that at least one record was updated.
			return SaveChanges();
		}
		catch (Exception ex)
		{
			throw new Exception($"An error occured while saving: {ex.Message}", ex);
		}
	}
}

//Validation is the same for Add or Update, so we consulidated it to one Method.
public List<Exception> ValidateCategory(CategoryView categoryView)
{
	List<Exception> errorList = [];
	//rule: Category name cannot be empty or whitespace
	if (string.IsNullOrWhiteSpace(categoryView.CategoryName))
		errorList.Add(new ArgumentException("Category name cannot be empty of white space."));
	//rule: Category cannot have the same name as another category
	bool exists = Categories
					.Where(x => x.CategoryName == categoryView.CategoryName)
					.Any();
	if (exists)
		errorList.Add(new ArgumentException($"A category with the name {categoryView.CategoryName} already exists."));
	return errorList;
}
//Example with Out - You do not need to know this at all
public void ValidateCategoryWithOut(CategoryView categoryView, out List<Exception> errorList) 
{
	errorList = [];
	//rule: Category name cannot be empty or whitespace
	if (string.IsNullOrWhiteSpace(categoryView.CategoryName))
		errorList.Add(new ArgumentException("Category name cannot be empty of white space."));
	//rule: Category cannot have the same name as another category
	bool exists = Categories
					.Where(x => x.CategoryName == categoryView.CategoryName)
					.Any();
	if (exists)
		errorList.Add(new ArgumentException($"A category with the name {categoryView.CategoryName} already exists."));
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

