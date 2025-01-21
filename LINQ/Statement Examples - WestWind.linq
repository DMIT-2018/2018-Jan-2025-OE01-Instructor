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

//More Date Examples
// How many days did it take for each order to ship?
Orders
	.Where(x => x.ShippedDate != null)
	.Select(x => new 
	{
		OrderNumber = x.OrderID,
		OrderDate = x.OrderDate,
		ShippingDate = x.ShippedDate,
		// Use .Value to get to the 'Value' (the DateTime) of a nullable Datatype
		// In this example OrderDate and ShippedDate are both DateTime?
		// The ? indicates it can be null when normally DateTime is not nullable.
		// Date Math - Can use TotalDays to find the number of days between two dates.
		DaysToShip = (x.ShippedDate.Value - x.OrderDate.Value).Days,
		// Years can be generalized unless you need more exact values
		YearsToShip = x.ShippedDate.Value.Year - x.OrderDate.Value.Year,
		YearsToShipExact = Math.Round((x.ShippedDate.Value - x.OrderDate.Value).Days / 365.25, 2, MidpointRounding.AwayFromZero),
		// Same generalization with months, or more exact
		MonthsToShip = ((x.ShippedDate.Value.Year - x.OrderDate.Value.Year) * 12) + (x.ShippedDate.Value.Month - x.OrderDate.Value.Month) + (x.ShippedDate.Value.Day >= x.OrderDate.Value.Day ? 0 : -1),
		MonthsToShipExact = ((x.ShippedDate.Value - x.OrderDate.Value).Days / (365.25/12))
	}).ToList().Dump();
	
	
	
	