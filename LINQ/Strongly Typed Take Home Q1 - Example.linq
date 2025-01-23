<Query Kind="Program">
  <Connection>
    <ID>8adc4b0b-6cab-4b78-9b39-90e988323172</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Server>MOMSDESKTOP\SQLEXPRESS</Server>
    <AllowDateOnlyTimeOnly>true</AllowDateOnlyTimeOnly>
    <DeferDatabasePopulation>true</DeferDatabasePopulation>
    <Database>Contoso</Database>
    <DriverData>
      <LegacyMFA>false</LegacyMFA>
    </DriverData>
  </Connection>
</Query>

void Main()
{
	GetInventoryReorder_ByCategoryAndStore(5, "Cell phones").Dump();
}

#region Methods
public List<InventorySummaryView> GetInventoryReorder_ByCategoryAndStore(int storeID, string categoryName) 
{
	return Inventories
			.Where(x => x.Store.StoreID == storeID 
				&& x.Product.ProductSubcategory.ProductCategory.ProductCategoryName == categoryName)
			.Select(x => new InventorySummaryView
			{
				StoreID = x.Store.StoreID,
				StoreName = x.Store.StoreName,
				ProductName = x.Product.ProductName,
				CategoryName = x.Product.ProductSubcategory.ProductCategory.ProductCategoryName,
				Reorder = x.OnOrderQuantity + x.OnHandQuantity >= x.SafetyStockQuantity ? "No" : "Yes"
			})
			.OrderBy(x => x.StoreID)
			.ToList();
}
#endregion

#region ViewModels
public class InventorySummaryView
{
	public int StoreID { get; set; }
	public string StoreName { get; set; } = string.Empty;
	public string ProductName { get; set; } = string.Empty;
	public string Reorder { get; set; } = string.Empty;
	public string CategoryName { get; set; } = string.Empty;
}
#endregion
