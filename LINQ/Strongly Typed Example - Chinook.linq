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

//When using strongly typed datasets, we need to define classes, so we need a C# Program
void Main()
{
	var anonResults = GetSongsByPartialName("Dance");
	// We cannot do anything with these results, it's very limited.
	// We can't count, order, or do anything specifically with this data.
	// anonResults.Count.Dump();
	anonResults.Dump("Anonymous Data");
	var strongResults = GetStrongSongsByPartialName("Dance");
	strongResults.Dump("Strong Typed Results");
	// We can now do further analysis or count, order, etc. the strongly typed results.
	strongResults.Count().Dump("Count of Song Results");
	// We can access the individual properties of each record in the results
	// because it has a defined (strong) type.
	strongResults.Select(x => x.SongTitle).FirstOrDefault().Dump("First Song");
	
	//Genre, the AlbumName, the TrackName, & the ArtistName for display in a Grid on the GUI
	var trackInfo = GetAllTracksInformation();
	trackInfo.Dump("Track Information");
	
	foreach(var track in trackInfo)
	{
		// Since this is strong typed (has a data definition) we can access the individual Properties
		if(track.Artist == "AC/DC")
			track.Dump();
	}
	
}

#region Methods
// IEnumerable is a collection type, this is the base class for a List()
// To use Anonymous datasets, you can return a IEnumerable
// Note: It has no defined type
public IEnumerable GetSongsByPartialName(string partialName)
{
	return Tracks
			.Where(x => x.Name.ToLower().Contains(partialName.ToLower()))
			.Select(x => new 
			{
				AlbumTitle = x.Album.Title,
				SongTitle = x.Name,
				Artist = x.Album.Artist.Name
			});
}

// We can make this a strongly typed result
public IEnumerable<SongView> GetStrongSongsByPartialName(string partialName)
{
	return Tracks
			.Where(x => x.Name.ToLower().Contains(partialName.ToLower()))
			.Select(x => new SongView
			{
				AlbumTitle = x.Album.Title,
				SongTitle = x.Name,
				Artist = x.Album.Artist.Name
			});
}

public IEnumerable<TrackInfoView> GetAllTracksInformation() 
{
	return Tracks
			.Select(x => new TrackInfoView
			{
				Genre = x.Genre.Name,
				Album = x.Album.Title,
				Track = x.Name,
				Artist = x.Album.Artist.Name
			});
}

#endregion

#region ViewModels
public class SongView
{
	//Give the class Auto-implemented properties
	// We can do read-only, calculated, fully implemented if needed.
	public string AlbumTitle { get; set; }
	public string SongTitle { get; set; }
	public string Artist { get; set; }
}
//Genre, the AlbumName, the TrackName, & the ArtistName for display in a Grid on the GUI
public class TrackInfoView 
{
	public string Genre { get; set; }
	public string Album { get; set; }
	public string Track { get; set; }
	public string Artist { get; set; }
}
#endregion
