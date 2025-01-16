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

void Main()
{
	//NESTED QUERIES - List Artists 1-5 and their Albums nested
	List<ArtistView> artistList = 
	Artists
		.Where(x => x.ArtistId <= 5)
		.Select(x => new ArtistView
		{
			Artist = x.Name,
			//When nesting we cannot use the same variable (we already used x)
			//Because we have navigation properties (using x), we can navigate to the album
			//Collection which only contains Albums related to the Artist!
			Albums = x.Albums
						.OrderBy(a => a.Title)
						//This is like a Sub-Query in SQL - We don't need this!
						//.Where(a => a.Artist.ArtistId == x.ArtistId)
						.Select(a => new AlbumView
						{
							Album = a.Title,
							Label = a.ReleaseLabel,
							Year = a.ReleaseYear,
							Tracks = a.Tracks
										.Select(t => new TrackView
										{
											TrackID = t.TrackId,
											Name = t.Name,
											//Converted to Seconds 
											Length = t.Milliseconds / 1000 
										}).ToList()
						//To List take the results of the LINQ and casts it to a List type
						}).ToList()
		}).ToList();
		artistList.Dump();
		foreach(var artist in artistList)
		{
			$"{artist.Artist} (Album Count: {artist.Albums.Count()}, Track Count: {artist.Albums.Sum(a => a.Tracks.Count())})".Dump();
		}
		// We can go up and grab single parent records (can only go up to make single parents, can only make lists of children)
		// We respect the PK => FK relationships in the database we are using
		List<TrackView> trackList = 
		Tracks
			.Select(t => new TrackView
			{
				TrackID = t.TrackId,
				Name = t.Name,
				Length = t.Milliseconds / 1000,
				Album = new AlbumView
				{
					Album = t.Album.Title,
					Label = t.Album.ReleaseLabel,
					Year = t.Album.ReleaseYear
				}
			}).ToList();
		trackList.Dump();
		
}

#region ViewModels
public class ArtistView 
{
	public string Artist { get; set; }
	//Referencing the ViewModel for the Albums
	//A collection of AlbumViews
	public List<AlbumView> Albums { get; set; }
}
public class AlbumView
{
	public string Album { get; set; }
	public string Label { get; set; }
	public int Year { get; set; }
	public List<TrackView> Tracks { get; set; }
}
public class TrackView
{
	public int TrackID { get; set; }
	public string Name { get; set; }
	public int Length { get; set; }
	public AlbumView Album { get; set; }
}
#endregion
