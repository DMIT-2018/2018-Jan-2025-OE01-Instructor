<Query Kind="Program">
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

void Main()
{
	GetAlbumsByYear(2000).Dump("Albums in the year 2000");
	 TestMethod();
}

// You can define other methods, fields, classes and namespaces here
List<Albums> GetAlbumsByYear(int year)
{
	return Albums
			.Where(x => x.ReleaseYear == year).ToList();
}

void TestMethod() 
{
	Albums
		.Where(x => x.Artist.Name.ToLower().StartsWith("a")).Dump();
}