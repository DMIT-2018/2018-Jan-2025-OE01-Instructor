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