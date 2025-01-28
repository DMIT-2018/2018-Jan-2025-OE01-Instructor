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
		Revenue = x.Sum(x => x.Quantity * x.UnitPrice)
	}).Dump();
	
// Group Order Details by, Customer City and Product Category
// Show city, Category, and Count
OrderDetails
	.GroupBy( x => new {
						//Yes, you can name your keys, you don't need to typically
						//Linq automatically names the 'fields' by the field name in anon types.
						//Example City and CategoryName as they are the actual field names from the entities.
						//However, you may need to name them if you are grouping by fields that have the same
						//name (this is rare, don't expect to need this)
						Bob = x.Order.Customer.City,
						x.Product.Category.CategoryName
					})
	.Select(x => new {
		City = x.Key.Bob,
		Category = x.Key.CategoryName,
		Count = x.Count()
	})
	.OrderBy(x => x.City)
	.ThenBy(x => x.Category)
	.Dump();
	
