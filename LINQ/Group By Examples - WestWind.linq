<Query Kind="Statements">
  <Connection>
    <ID>2963e05e-d046-494f-8322-6be4054e0dc3</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Server>MOMSDESKTOP\SQLEXPRESS</Server>
    <AllowDateOnlyTimeOnly>true</AllowDateOnlyTimeOnly>
    <DeferDatabasePopulation>true</DeferDatabasePopulation>
    <Database>WestWind-2024</Database>
    <DriverData>
      <LegacyMFA>false</LegacyMFA>
    </DriverData>
  </Connection>
</Query>

//Group by Year and calculate the total revenue for the year
OrderDetails
	.GroupBy(x => x.Order.OrderDate.Value.Year)
	.Select(x => new
	{
		Year = x.Key,
		Revenue = $"{x.Sum(x => x.Quantity * x.UnitPrice):C2}"
	}).Dump();