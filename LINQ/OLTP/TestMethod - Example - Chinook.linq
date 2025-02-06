<Query Kind="Program">
  <Connection>
    <ID>8de3010c-824d-4f86-96f8-e5c1262e92e5</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Server>MOMSDESKTOP\SQLEXPRESS</Server>
    <AllowDateOnlyTimeOnly>true</AllowDateOnlyTimeOnly>
    <DeferDatabasePopulation>true</DeferDatabasePopulation>
    <Database>ChinookSept2018</Database>
    <DriverData>
      <LegacyMFA>false</LegacyMFA>
    </DriverData>
  </Connection>
</Query>

//	Driver is responsible for orchestrating the flow by calling 
//	various methods and classes that contain the actual business logic 
//	or data processing operations.
void Main()
{
	#region Get Artist (By ID)
	// Pass Tests
	Console.WriteLine("============");
	Console.WriteLine("---- Artist By ID Pass ----");
	Console.WriteLine("============");
	TestGetArtist_ByID(1).Dump("Pass - Valid ID");
	TestGetArtist_ByID(1000).Dump("Pass - Valid ID - No artist found");

	// Fail Tests
	Console.WriteLine("============");
	Console.WriteLine("---- Artist By ID Fail ----");
	Console.WriteLine("============");
	// rule: Artist ID must be valid > 0
	//Remember: 0 is a special case, always test it separate from negative numbers.
	TestGetArtist_ByID(0).Dump("Fail - ArtistID must be valid - 0 Test");
	TestGetArtist_ByID(-12).Dump("Fail - ArtistID must be valid - Negative Test");
	#endregion

	#region Get Artists (By PartialName)
	Console.WriteLine("============");
	Console.WriteLine("---- Artists By Partial Name Pass ----");
	Console.WriteLine("============");
	//Pass 
	TestGetArtists_ByPartialName("ABB").Dump("Pass - Valid Artist Found");
	TestGetArtists_ByPartialName("ABC").Dump("Pass - Valid Name - No Artists Found");
	Console.WriteLine("============");
	Console.WriteLine("---- Artists By Partial Name Fail ----");
	Console.WriteLine("============");
	//Fail
	//rule: Partial Name cannot be null or white space
	//We should test null, empty, and white space to ensure the correct check is done.
	TestGetArtists_ByPartialName("").Dump("Fail - Artist Name was empty");
	TestGetArtists_ByPartialName("   ").Dump("Fail - Artist Name was White Space");
	TestGetArtists_ByPartialName(null).Dump("Fail - Artist Name was null");
	
	#endregion
}

//	This region contains methods used for testing the functionality
//	of the application's business logic and ensuring correctness.
#region Test Methods
public ArtistEditView TestGetArtist_ByID(int artistID)
{
	try
	{
		//For single item returns
		//Save the results to a variable
		//Check if the result is null
		//Return a new empty Record() if null, else return the results.
		var results = GetArtist_ByID(artistID);
		if(results == null)
			return new ArtistEditView();
		else
			return results;
	}
	catch (ArgumentNullException ex)
	{
		GetInnerException(ex).Message.Dump();
	}
	catch (Exception ex)
	{
		GetInnerException(ex).Message.Dump();
	}
	return null; //Ensure we always have a valid return, even when there are exceptions
}
public List<ArtistEditView> TestGetArtists_ByPartialName(string partialName)
{
	try
	{
		return GetArtists_ByPartialName(partialName);
	}
	catch (ArgumentNullException ex)
	{
		GetInnerException(ex).Message.Dump();
	}
	catch (Exception ex)
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
public ArtistEditView GetArtist_ByID(int artistID)
{
	// rule: ArtistID must be valid - greater than 0
	if (artistID <= 0)
		throw new ArgumentNullException("Please provide a valid artist ID. (greater than 0)");

	return Artists
			.Where(x => x.ArtistId == artistID)
			.Select(x => new ArtistEditView
			{
				ArtistID = x.ArtistId,
				Name = x.Name
			}).FirstOrDefault();
}
public List<ArtistEditView> GetArtists_ByPartialName(string partialName) 
{
	if(string.IsNullOrWhiteSpace(partialName))
		throw new ArgumentNullException("Artist name is required.");
		
	return Artists
			.Where(x => x.Name.ToLower().Contains(partialName.ToUpper()))
			.Select(x => new ArtistEditView
			{
				ArtistID = x.ArtistId,
				Name = x.Name
			})
			.OrderBy(x => x.Name)
			.ToList();
}
#endregion

//	This region includes the view models used to 
//	represent and structure data for the UI.
#region View Models
public class ArtistEditView
{
	public int ArtistID { get; set; }
	public string Name { get; set; }
}
#endregion

