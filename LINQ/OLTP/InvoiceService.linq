<Query Kind="Program">
  <Connection>
    <ID>fc06acea-5ea4-4540-bb2f-87d2f386c43e</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Driver Assembly="(internal)" PublicKeyToken="no-strong-name">LINQPad.Drivers.EFCore.DynamicDriver</Driver>
    <AllowDateOnlyTimeOnly>true</AllowDateOnlyTimeOnly>
    <Server>MOMSDESKTOP\SQLEXPRESS</Server>
    <Database>OLTP-DMIT2018</Database>
    <DisplayName>OLTP-DMIT2018-Entity</DisplayName>
    <DriverData>
      <EncryptSqlTraffic>True</EncryptSqlTraffic>
      <PreserveNumeric1>True</PreserveNumeric1>
      <EFProvider>Microsoft.EntityFrameworkCore.SqlServer</EFProvider>
    </DriverData>
  </Connection>
</Query>

void Main()
{
	var invoice = GetInvoice(29, 8, 7);
	AddOrEditInvoice(invoice);
}

// You can define other methods, fields, classes and namespaces here
#region Method
//This method will get a existing invoice or create a new invoice.
public InvoiceView GetInvoice(int invoiceID, int customerID, int employeeID)
{
	//Business Rules

	//Rule: Both a customerID and EmployeeID must be provided.
	if (invoiceID == 0 && customerID == 0)
		throw new ArgumentNullException("No customer was provided.");
	if (employeeID == 0)
		throw new ArgumentNullException("No employee was provided.");
	//Handle both new and existing invoices
	// For new invoices the caller must supply the Customer and Employee ID
	// For existing invoices the caller must supply the Invoice and Employee ID
	// - If the invoice is updated later we might need to know who updated it.
	// We still check for the CustomerID because it is needed for new but we can
	// check if the invoiceID is 0 as well.

	//Have to create a invoice
	InvoiceView invoice = null;
	//Check if new
	if (customerID > 0 && invoiceID == 0)
	{
		invoice = new();
		invoice.CustomerID = customerID;
		invoice.EmployeeID = employeeID;
		invoice.InvoiceDate = DateOnly.FromDateTime(DateTime.Today);
		invoice.CustomerName = GetCustomerFullName(customerID);
		invoice.EmployeeName = GetEmployeeFullName(employeeID);
	}

	//if not new
	else
	{
		invoice = Invoices
					.Where(x => x.InvoiceID == invoiceID
						&& !x.RemoveFromViewFlag)
					.Select(x => new InvoiceView
					{
						InvoiceID = x.InvoiceID,
						InvoiceDate = x.InvoiceDate,
						CustomerID = x.CustomerID,
						CustomerName = x.Customer.FirstName + " " + x.Customer.LastName,
						EmployeeID = x.EmployeeID,
						EmployeeName = x.Employee.FirstName + " " + x.Employee.LastName,
						SubTotal = x.SubTotal,
						Tax = x.Tax,
						InvoiceLines = x.InvoiceLines
							.Where(il => !il.RemoveFromViewFlag)
							.Select(il => new InvoiceLineView
							{
								InvoiceLineID = il.InvoiceLineID,
								InvoiceID = il.InvoiceID,
								PartID = il.PartID,
								Quantity = il.Quantity,
								Description = il.Part.Description,
								Price = il.Price,
								Taxable = il.Part.Taxable,
								RemoveFromViewFlag = il.RemoveFromViewFlag
							}).ToList(),
						RemoveFormViewFlag = x.RemoveFromViewFlag
					})
					.FirstOrDefault();
	}
	//Now return the new or existing invoice
	return invoice;
}
//Add a new invoice and update an existing invoice
public InvoiceView AddOrEditInvoice(InvoiceView invoiceView)
{
	//Business Rules
	//	create a list<Exception> to contain all discovered errors
	List<Exception> errorList = new List<Exception>();
	//rule: invoice cannot be null
	if(invoiceView == null)
		throw new ArgumentNullException("No invoice was supplied.");
	//rule: invoice must have invoice lines
	if(invoiceView.InvoiceLines.Count == 0)
		throw new ArgumentNullException("Invoice must have invoice lines.");
	//rule: Customer must be supplied
	if(invoiceView.CustomerID == 0)
		throw new ArgumentNullException("No customer was supplied.");
	//rule: Employee must be supplied
	if(invoiceView.EmployeeID == 0)
		throw new ArgumentNullException("No employee was supplied.");
	//rule: invoice line quantities must be greater than zero
	foreach(InvoiceLineView line in invoiceView.InvoiceLines)
	{
		if (line.Quantity < 1)
			errorList.Add(new ArgumentException($"Invoice line {line.Description} has a value less then 1."));
	}
	
	//pre check to see if there are any errors
	if(errorList.Count > 0)
	{
		//This is like a rollback in SQL
		ChangeTracker.Clear();
		//Throw the errors
		string errorMsg = "Unable to add or edit Invoice or Invoice Lines. Please check error message(s)";
		throw new AggregateException(errorMsg, errorList);
	}
	// Now we know there are no major exceptions, so we can process the Add or Update.
	// Create a list InvoiceLines that already exist in the database
	List<InvoiceLineView> databaseInvoiceLineViews = InvoiceLines
		.Where(x => x.InvoiceID == invoiceView.InvoiceID)
		.Select(x => new InvoiceLineView
		{
			InvoiceLineID = x.InvoiceLineID,
			InvoiceID = x.InvoiceID,
			PartID = x.PartID,
			Quantity = x.Quantity,
			Description = x.Part.Description,
			Price = x.Price,
			Taxable = x.Part.Taxable,
			RemoveFromViewFlag = x.RemoveFromViewFlag
		}).ToList();
	
	//Get a list of Parts from our database - this will be used to update Quantity On Hand and to make sure the Parts given exist.
	
	return null;
}
//Supporting Methods
public string GetCustomerFullName(int customerID)
{
	return Customers
		.Where(x => x.CustomerID == customerID)
		.Select(x => $"{x.FirstName} {x.LastName}").FirstOrDefault();
}
public string GetEmployeeFullName(int employeeId)
{
	return Employees
		.Where(x => x.EmployeeID == employeeId)
		.Select(x => $"{x.FirstName} {x.LastName}").FirstOrDefault();
}
#endregion

#region View Models
public class InvoiceView
{
	public int InvoiceID { get; set; }
	public DateOnly InvoiceDate { get; set; }
	public int CustomerID { get; set; }
	public string CustomerName { get; set; } = string.Empty;
	public int EmployeeID { get; set; }
	public string EmployeeName { get; set; } = string.Empty;
	public decimal SubTotal { get; set; }
	public decimal Tax { get; set; }
	public decimal Total => SubTotal + Tax;
	//Always make sure that a child list in a View is defaulted to an empty list
	//You can use = [] to do this.
	public List<InvoiceLineView> InvoiceLines { get; set; } = [];
	public bool RemoveFormViewFlag { get; set; }
}
public class InvoiceLineView
{
	public int InvoiceLineID { get; set; }
	public int InvoiceID { get; set; }
	public int PartID { get; set; }
	public int Quantity { get; set; }
	public string Description { get; set; } = string.Empty;
	public decimal Price { get; set; }
	public bool Taxable { get; set; }
	public decimal ExtentPrice => Price * Quantity;
	public bool RemoveFromViewFlag { get; set; }
}
#endregion