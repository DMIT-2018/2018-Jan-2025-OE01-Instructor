<Query Kind="Statements">
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

//Class Examples - Contoso

//Strongly Typed - T2 - This example is not Strongly Type! (We will fix it later)
Invoices
	.Select(x => new 
	{
		InvoiceNo = x.InvoiceID,
		InvoiceDate = x.DateKey,
		// Formatting the value in a string substitution
		// $"{Value to Format:Format}"
		Amount = $"{x.TotalAmount:C}",
		Name = x.Customer.FirstName + " " + x.Customer.LastName,
		StoreName = x.Store.StoreName,
		City = x.Store.Geography.CityName,
		Priority = x.TotalAmount >= 10000 ? "High Priority" : "Low Priority"
	}).Dump();