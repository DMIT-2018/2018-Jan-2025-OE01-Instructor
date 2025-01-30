//	Driver is responsible for orchestrating the flow by calling 
//	various methods and classes that contain the actual business logic 
//	or data processing operations.
void Main()
{
	// Pass Tests
	Console.WriteLine("============");
	Console.WriteLine("---- Artist Pass ----");
	Console.WriteLine("============");
	TestGetArtist_ByID(1).Dump("Pass - Valid ID");
	TestGetArtist_ByID(1000).Dump("Pass - Valid ID - No artist found");

	// Fail Tests
	Console.WriteLine("============");
	Console.WriteLine("---- Artist Fail ----");
	Console.WriteLine("============");
	// rule: Artist ID must be valid > 0
	TestGetArtist_ByID(0).Dump("Fail - ArtistID must be valid");
}

//	This region contains methods used for testing the functionality
//	of the application's business logic and ensuring correctness.
#region Test Methods
public ArtistEditView TestGetArtist_ByID(int artistID)
{
	try 
	{
		return GetArtist_ByID(artistID);
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
	if(artistID <= 0)
		throw new ArgumentNullException("Please provide a valid artist ID. (greater than 0)");
		
	return Artists
			.Where(x => x.ArtistId == artistID)
			.Select(x => new ArtistEditView
			{
				ArtistID = x.ArtistId,
				Name = x.Name
			}).FirstOrDefault();
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

