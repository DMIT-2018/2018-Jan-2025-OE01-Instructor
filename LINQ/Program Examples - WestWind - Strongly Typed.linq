<Query Kind="Program">
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

void Main()
{
	//Program to return all the Customers first and last names as "FirstName LastName" and the total of their orders
	var customerList = GetCustomerAndTotals();
	//anonymous example
	customerList
		.Where(x => x.LastOrderDate.HasValue && x.FirstOrderDate.HasValue)
		.Select(x => new
		{
			ContactName = x.ContactName,
			AllOrderTotals = x.Orders.Sum(o => o.OrderSubTotal),
			CustomerYears = (x.LastOrderDate.Value.Year - x.FirstOrderDate.Value.Year)
		}).Dump();
}

#region Methods
public List<CustomerView> GetCustomerAndTotals() 
{
	return Customers
		.Select(x => new CustomerView 
		{
			ContactName = x.ContactName,
			OrderCount = x.Orders.Count(),
			LastOrderDate = x.Orders.Max(o => o.OrderDate),
			FirstOrderDate = x.Orders.Min(o => o.OrderDate),
			Orders = x.Orders.Select(o => new OrderView 
			{
				OrderID = o.OrderID,
				OrderDate = o.OrderDate,
				// When we do work inside the Aggregate function (methods) we just need to ensure
				// we end up with one value to Min, Max, Sum, Count, or Average
				OrderSubTotal = o.OrderDetails.Sum(od => (od.UnitPrice * od.Quantity) * (1.00m - (decimal)od.Discount)),
				AvgProductPrice = o.OrderDetails.Average(od => od.UnitPrice)
			}).ToList()
		}).ToList();
}
#endregion

#region View Models
public class CustomerView
{
	public string ContactName { get; set; }
	public List<OrderView> Orders { get; set; }
	public int OrderCount { get; set; }
	//Max Example
	public DateTime? LastOrderDate { get; set; }
	//Min Example
	public DateTime? FirstOrderDate { get; set; }
}
public class OrderView
{
	public int OrderID { get; set; }
	public DateTime? OrderDate { get; set; }
	public decimal OrderSubTotal { get; set; }
	public decimal AvgProductPrice { get; set; }
}
#endregion
