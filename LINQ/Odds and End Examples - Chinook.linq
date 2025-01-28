<Query Kind="Statements">
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

//First - Returns the first result
// Does not need a where clause
Albums.First().Dump();
//Can be after a Where Clause
// Will throw an exception if no results are found
Albums
	.Where(x => x.Title.Contains("awedfSEA fsarfg"))
	.First()
	.Dump();
	
//FirstOrDefault
//The default provides the default for the datatype of the collection
//Example - Objects = null, int = 0, string = null, etc.
Albums
	.Where(x => x.Title.Contains("SDFASDFASDFAESR"))
	.FirstOrDefault()
	.Dump();

//Be aware when using FirstOrDefault that you know the 
//default of the dataType
List<int> exampleList = [9, 7];

exampleList
	.Where(x => x > 10)
	.FirstOrDefault()
	.Dump();
	
//Single - Expects only 1 result
//This will throw an exception if you get more than one result
Albums.Where(x => x.AlbumId == 1)
	.Single().Dump();

//SingleOrDefault
//Still throws an exception for more than one result
//but if no results are found it return the default
Albums.Where(x => x.AlbumId < 0)
	.SingleOrDefault().Dump();
//Will still throw an exception
Albums.Where(x => x.AlbumId > 1)
	.SingleOrDefault().Dump();
	
//Distinct - Return only one copy of each whole record (all the field have to be unique for the record)
Albums
	.Where(x => x.ReleaseYear > 1970)
	.OrderBy(x => x.ReleaseYear)
	.ThenBy(x => x.ReleaseLabel)
	.Select(x => new {
		x.ReleaseYear,
		Label = x.ReleaseLabel
	}).Distinct().Dump();
	
//Take - Can act like TOP in SQL and just get the first n (number) of records
Albums
	.Take(5)
	.Dump();
	
//Skip - is like the opposite of Take
Albums
	.Skip(5)
	.Dump();

//You can combine Take and Skip for interesting results
//Example: Get the third most expensive track
Tracks
	.OrderByDescending(x => x.UnitPrice)
	.Skip(2)
	.Take(1).Dump();
	
//Any - Results true or false, true is anything in the collection matches the condition
//Commonly used for Exists checks (example, does an employee with a particular 
//employeeID exist in the database.
Albums
	.Any(x => x.AlbumId == 1000)
	.Dump();
	
//All checks if everything in the collection matches the condition
Albums
	.All(x => x.AlbumId > 0)
	.Dump();