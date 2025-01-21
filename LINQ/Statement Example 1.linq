<Query Kind="Statements">
  <Connection>
    <ID>cfb33577-51c0-4e30-8a96-e2d8d1ba966a</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Server>MOMSDESKTOP\SQLEXPRESS</Server>
    <AllowDateOnlyTimeOnly>true</AllowDateOnlyTimeOnly>
    <DeferDatabasePopulation>true</DeferDatabasePopulation>
    <Database>ChinookSept2018_Test</Database>
    <DriverData>
      <LegacyMFA>false</LegacyMFA>
    </DriverData>
  </Connection>
</Query>

//Where Title contains "dance" and the release label is "EMI"
Albums.Where(x => x.Title.Contains("dance") && x.ReleaseLabel == "EMI").Dump();

Customers.Where(x => x.Country == "Canada")
			.Select(x => x.FirstName).Dump("Customer First Names from Canada");
//Store Variables in Statements
List<Albums> yearAlbums = Albums.Where(x => x.ReleaseYear == 2000).ToList();

//I have to dump the output
yearAlbums.Dump();

//Single data (not a table, just one variable)
yearAlbums.Count.Dump("Total Albums Released in 2000");

//ORDER BY EXAMPLE
//Ascending Order By (A - Z)
Artists.OrderBy(x => x.Name)
	.Select(x => new 
	{
		ArtistID = x.ArtistId,
		Name = x.Name
	})
	.Dump();

//Decending (Z - A)
Artists.OrderByDescending(x => x.Name)
	.Select(x => new
	{
		ArtistID = x.ArtistId,
		Name = x.Name
	})
	.Dump();

//Ordering by more!!
Albums
	.OrderBy(x => x.ReleaseYear)
	.ThenBy(x => x.Title)
	.Dump();

//Mix Descending and Ascending
//Can be mixed any which way we want
//Can be more than 2
Albums
	.OrderBy(x => x.ReleaseYear)
	.ThenByDescending(x => x.Title)
	.Dump();

Albums
	.OrderByDescending(x => x.ReleaseYear)
	.ThenBy(x => x.Title)
	.Dump();

//More than 2!
Albums
	.OrderBy(x => x.ReleaseYear)
	.ThenBy(x => x.ReleaseLabel)
	.ThenBy(x => x.Title)
	.Dump();

// Order By is important for Grid/Tables displays, Select lists
// Example: Sort select list of Artist Alphanumerically.
// We likely want to show data in different ordering than the primary key

//Navigational Properties EXAMPLES
Albums
	.Select(x => new 
	{
		//Give the data names and what should go there
		AlbumID = x.AlbumId,
		//Select more by command separating the list of [title] = [value]
		Title = x.Title,
		//Navigating to the Artist Entity to get the name of the
		//Artist associated with the Album
		Artist = x.Artist.Name
		//Cannot navigate directly to Children Tables, this will not work!
		//TrackName = x.Tracks.Name
	}).Dump();

// Tracks Example => Genre
Tracks
	.Select(t => new
	{
		TrackName = t.Name,
		Genre = t.Genre.Name
	}).Dump();
	
// Combine the above two examples
Albums
	.Where(x => x.AlbumId < 6)
	// Can order by anything in the Dataset before the Select
	// Example: Title
	// Remember we are ordering by the whole dataset, which will take more resources.
	// Always filter your dataset as much as possible before ordering or performing analysis with the data
	.OrderBy(x => x.Title)
	.Select(x => new 
	{
		AlbumID = x.AlbumId,
		Album = x.Title,
		Artist = x.Artist.Name,
		// We need to include the placeholder (x) in order
		// To only see related children for each record
		NumOfTracks = x.Tracks.Count,
		TotalTrackPrice = x.Tracks.Sum(t => t.UnitPrice),
		Tracks = x.Tracks
					.Select(t => new
					{
						TrackName = t.Name,
						Genre = t.Genre.Name
					})
	})
	// We can only order by what we selected and how we named it
	// After the select Example: Album
	.OrderBy(x => x.Album)
	.Dump();
	
//Ternary Operator Example
int number1 = 10;
int number2 = 15;

//if(number1 > number2)
	//number1.Dump();
//else
	//number2.Dump();
		
(number1 > number2 ? number1 : number2).Dump();

//Return the Album Title, Year, Label. For Label if null put in Unknown
// In C# I could use a if statement but it will be fairly complex and may require a loop
Albums
	.Select(x => new 
	{
		Album = x.Title,
		Year = x.ReleaseYear,
		Label = x.ReleaseLabel == null ? "Unknown" : x.ReleaseLabel
	})
	.Dump();

// Classify the output by Decade
// ReleaseYear < 1970 = "Oldies"
// ReleaseYear 1970 - 1979 = "70s"
// ReleaseYear 1980 - 1989 = "80s"
// ReleaseYear 1990 - 1999 = "90s"
// ReleaseYear 2000 or Newer = "Modern"
// Remember you can only return one value!
Albums
	.Select(x => new
	{
		Album = x.Title,
		Year = x.ReleaseYear,
		Decade = x.ReleaseYear < 1970 ? "Oldies" :
					x.ReleaseYear < 1980 ? "70s" :
					x.ReleaseYear < 1990 ? "80s" :
					x.ReleaseYear < 2000 ? "90s" : "Modern"
	})
	// Order by the right things that gives the logical results
	// In case order by year as it is an int
	// Ordering by Decade is possible but it is a string so AlphaNumber (0-9 then a-z)
	// Ordering by Decade put Oldies at the bottom
	.OrderBy(x => x.Year)
	.Dump();
	
//if(x.ReleaseYear < 1970)
	//return "Oldies"
//else
	//if(x.ReleaseYear < 1980)
		//return "70s"
	//else
		//if(.....)
		
		//else
			//return "Modern"
			
// Where Example with Date Search
// Find all employees born after in or after 1970
Employees
	.Where(x => x.BirthDate >= new DateTime(1970, 1, 1)).Dump();

//Using parse without a format dd/mm/yyyy
Employees
	.Where(x => x.BirthDate >= DateTime.Parse("01/01/1970")).Dump();
	




