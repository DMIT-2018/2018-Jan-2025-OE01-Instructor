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

//Example to show key/values in Dictionaries
//This is for example only - You do not need to know this!
Dictionary<string, List<string>> Test = new();

Test.Add("a", new List<string> { "a", "b"});
Test.Add("b", new List<string> { "c", "d"});

Test["a"].Dump();
Test["b"].Dump();

foreach(KeyValuePair<string, List<string>> kvp in Test)
{
	$"Key: {kvp.Key}, Value1: {kvp.Value[0]}, Value2: {kvp.Value[1]}".Dump();
}
//End Example

//Grouping in Linq
Albums
	//Can use navigational Properties in the Group By
	.GroupBy(x => x.Artist.Name)
	.Select(x => new
	{
		//Provides the value it is grouped by
		Artist = x.Key,
		//Can be used to get other values from the database
		//We use this to group by the Primary Key when there is a chance 
		//the other field we want to display has repeated information
		//(Artists
		//	.Where(a => a.ArtistId == x.Key)
		//	.Select(a => a.Name).FirstOrDefault()),
		//ToList here dumps all the objects in the group to a list
		Albums = x.ToList()
	}).Dump();
	
//Group By with Aggregates, counts of Albums per artist 
Artists
	.GroupBy(a => a.Name)
	.Select(a => new
	{
		Artist = a.Key,
		AlbumCount = a.Sum(c => c.Albums.Count)
	}).Dump();
	
//Grouping by ReleaseYear and Label (two attributes)
Albums
	//Need to create a dynamic (anonymous) object to use two or more attributes
	.GroupBy(x => new {x.ReleaseYear, x.ReleaseLabel})
	.Select(x => new
	{
		Year = x.Key.ReleaseYear,
		Label = x.Key.ReleaseLabel,
		Count = x.Count(),
		Album = x.Select(a => new
		{
			Title = a.Title,
			Artist = a.Artist.Name
		}).ToList()
	})
	.Where(x => x.Count > 1)
	.OrderBy(x => x.Year)
	.ThenBy(x => x.Label)
	.Dump();
	
	