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
	try
	{
		AggregateTestMethod("", "", 31, "F");	
	}
	catch(AggregateException ex)
	{
		//note: must call ex.InnerExceptions to get the actual exceptions from the List
		foreach(var error in ex.InnerExceptions)
		{
			error.Message.Dump();
		}	
	}
	catch(Exception ex)
	{
		GetInnerException(ex).Message.Dump();
	}
	//Argument Null Exceptions
	try
	{
		ArgumentNullExceptionTest(0);
	}
	catch(ArgumentNullException ex)
	{
		GetInnerException(ex).Message.Dump();
	}
	//Remember to always include this Exception catch
	//this makes sure anything unexpected is caught
	catch (Exception ex)
	{
		GetInnerException(ex).Message.Dump();
	}
	try
	{
		var track = ArgumentNullExceptionTest(10000);
		if(track == null)
		{
			//Add to an error message, such as "Track {10000} does not exist."
		}
		else
		{
			track.Dump();
		}
	}
	catch (ArgumentNullException ex)
	{
		GetInnerException(ex).Message.Dump();
	}
	//Remember to always include this Exception catch
	//this makes sure anything unexpected is caught
	catch (Exception ex)
	{
		GetInnerException(ex).Message.Dump();
	}
	
}

//	This region contains methods used for testing the functionality
//	of the application's business logic and ensuring correctness.
#region Test Methods
public void AggregateTestMethod(string firstName, string lastName, int testValue, string paymentType)
{
	//We first have to create a list to hold our exceptions
	List<Exception> errorList = new();
	
	//rule: firstName cannot be empty or null
	if(string.IsNullOrWhiteSpace(firstName))
	{
		errorList.Add(new ArgumentNullException("First name is required and cannot be empty."));
	}
	//rule: Last name cannot be empty of null
	if (string.IsNullOrWhiteSpace(lastName))
	{
		errorList.Add(new ArgumentNullException("Last name is required and cannot be empty."));
	}
	
	//rule: testValue must be less than 30
	if(testValue > 30)
		errorList.Add(new ArgumentException("Test Value must be less than 30"));
		
	//rule: Payment Type must be M, D, or C
	if(paymentType != "M" && paymentType != "D" && paymentType != "C")
		errorList.Add(new ArgumentOutOfRangeException("Payment type must be M, D, or C."));
		
	if(errorList.Count > 0)
	{
		throw new AggregateException("Unable to proceed! Please review errors", errorList);
	}
	else 
	{
		/* Actual method code, such as processing the data */
	}
}

public Tracks ArgumentNullExceptionTest(int trackID)
{
	List<Exception> errorList = new();
	if(trackID <= 0)
	{
		throw new ArgumentNullException("A track ID must be provided.");
	}
	return Tracks
			.Where(x => x.TrackId == trackID)
			.FirstOrDefault();
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
