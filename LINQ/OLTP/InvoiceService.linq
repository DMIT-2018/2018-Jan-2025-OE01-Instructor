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
	//GetInvoice(29, 8, 7).Dump();
	//InvoiceView newInvoice = GetInvoice(0, 8, 7);
	//newInvoice.Dump();
	//var newLine = new InvoiceLineView
	//{
	//	PartID = 4,
	//	Price = 60.00m,
	//	Description = "Rear brakes",
	//	Quantity = 2,
	//	Taxable = true
	//};
	//newInvoice.InvoiceLines.Add(newLine);
	//newInvoice.Dump();
	//
	//var savedInvoice = AddOrEditInvoice(newInvoice);
	//savedInvoice.Dump();
	
	//Invoice Editing Example - Add Line
	//InvoiceView myInvoice = GetInvoice(1166, 8, 7);
	//InvoiceLineView newLine2 = new InvoiceLineView {
	//	PartID = 9,
	//	Quantity = 2,
	//	Price = 25.00m
	//};
	//
	//myInvoice.InvoiceLines.Add(newLine2);
	//var editedInvoice = AddOrEditInvoice(myInvoice);
	//editedInvoice.Dump();
	
	//Invoice Editing Example - Edit Line
	//InvoiceView myInvoice = GetInvoice(1166, 8, 7);
	//InvoiceLineView myLine = myInvoice.InvoiceLines.Where(x => x.PartID == 9).FirstOrDefault();
	//myLine.Quantity = 1;
	//
	//var editedInvoice = AddOrEditInvoice(myInvoice);
	//editedInvoice.Dump();
	
	//Invoice Editing Example - Removing a Line
	InvoiceView myInvoice = GetInvoice(1166, 8, 7);
	InvoiceLineView myLine = myInvoice.InvoiceLines.Where(x => x.PartID == 9).FirstOrDefault();
	myInvoice.InvoiceLines.Remove(myLine);
	
	var editedInvoice = AddOrEditInvoice(myInvoice);
	editedInvoice.Dump();
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
	List<InvoiceLineView> databaseInvoiceLines = InvoiceLines
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
	List<Part> databaseParts = Parts.ToList();
	
	//Process if it is a new or existing Invoice
	Invoice invoice = Invoices
		.Where(x => x.InvoiceID == invoiceView.InvoiceID).FirstOrDefault();
	//Check if it was found, or if it defaulted to null
	if(invoice == null)
	{
		invoice = new();
	}
	//Either way we add the values from the invoiceView to the new or existing invoice
	invoice.InvoiceDate = invoiceView.InvoiceDate;
	invoice.CustomerID = invoiceView.CustomerID;
	invoice.EmployeeID = invoiceView.EmployeeID;
	
	//Process each Invoice Line for Potential Changes OR New Lines to be added.
	foreach(InvoiceLineView line in invoiceView.InvoiceLines)
	{
		//Get the single Part that might need to be updated
		Part part = databaseParts.Where(x => x.PartID == line.PartID).FirstOrDefault();
		//Get the single invoice Line from the database, if it exists!
		InvoiceLineView dbInvoiceLine = databaseInvoiceLines.Where(x => x.InvoiceLineID == line.InvoiceLineID).FirstOrDefault();
		//If it exists, the line from the database will not be null
		if(dbInvoiceLine != null)
		{
			//Check if the Quantity has changed
			if(line.Quantity != dbInvoiceLine.Quantity)
			{
				// If it has changed, we need to update the QOH for the part because it sold or not.
				// Examples:
				// Invoice line was previous 3, now is 4, there is 1 less part ON HAND
				// Part QOH was 10, 10 - (4 - 3) => 10 - 1 = 9
				// Invoice line was previous 5, now is 2, there are 3 more available to sell (ON HAND)
				// Part QOH was 10, 10 - (2 - 5) => 10 - (-3) =>  10 + 3 = 13
				part.QOH = part.QOH - (line.Quantity - dbInvoiceLine.Quantity);
				Parts.Update(part);
				
				//Update the invoice
				InvoiceLine existingInvoiceLine = InvoiceLines
					.Where(x => x.InvoiceLineID == line.InvoiceLineID).FirstOrDefault();
				existingInvoiceLine.Quantity = line.Quantity;
				InvoiceLines.Update(existingInvoiceLine);
			}
		}
		else
		{
			InvoiceLine invoiceLine = new();
			invoiceLine.PartID = line.PartID;
			invoiceLine.Quantity = line.Quantity;
			invoiceLine.Price = line.Price;
			invoiceLine.RemoveFromViewFlag = line.RemoveFromViewFlag;
			
			//Update the part QOH
			part.QOH = part.QOH - line.Quantity;
			Parts.Update(part);
			
			// What about the second FK - InvoiceID
			// If the Invoice is new then we don't have an InvoiceID yet
			// To solve this issue, we use the navigational Properties of Entity Framework
			// When added via navigational properties, adding to the database, automatically provides the InvoiceID.
			invoice.InvoiceLines.Add(invoiceLine);
		}
	}

	//Process each existing line in the database to logically delete them if they are no longer on the invoice
	//Loop over the database invoice lines and check if they exist on the given invoice
	foreach (InvoiceLineView line in databaseInvoiceLines)
	{
		if(!invoiceView.InvoiceLines.Any(x => x.InvoiceLineID == line.InvoiceLineID))
		{
			Part part = databaseParts.Where(x => x.PartID == line.PartID).FirstOrDefault();
			//Update the part to add back the previous quantity from the invoice line
			part.QOH = part.QOH + line.Quantity;
			Parts.Update(part);
			//We also have to logically delete the invoice line
			//Get the actual database record, update the remove from view flag and update the record in the database.
			InvoiceLine deletedInvoiceLine = InvoiceLines
				.Where(x => x.InvoiceLineID == line.InvoiceLineID).FirstOrDefault();
			deletedInvoiceLine.RemoveFromViewFlag = true;
			InvoiceLines.Update(deletedInvoiceLine);
		}
	}
	
	//Update the Subtotal for the invoice and Tax
	//reset
	invoice.SubTotal = 0;
	invoice.Tax = 0;
	foreach(InvoiceLine line in invoice.InvoiceLines)
	{
		if(line.RemoveFromViewFlag == false)
		{
			invoice.SubTotal = invoice.SubTotal + (line.Quantity * line.Price);
			//We need to check that the Part is actually taxable
			bool isTaxable = Parts.Where(x => x.PartID == line.PartID)
				.Select(x => x.Taxable).FirstOrDefault();
			invoice.Tax = invoice.Tax + (isTaxable
											? line.Quantity * line.Price * 0.05m
											: 0);
		}
	}
	
	//Check if this is an Add or an Update (one last time)
	if(invoice.InvoiceID == 0)
		Invoices.Add(invoice);
	else
		Invoices.Update(invoice);
	
	//Do a final check for any errors
	if(errorList.Count > 0)
	{
		ChangeTracker.Clear();
		throw new AggregateException("Unable to add or edit invoice. Please check error message(s)", errorList);
	}
	else
	{
		SaveChanges();
	}
	return GetInvoice(invoice.InvoiceID, invoice.CustomerID, invoice.EmployeeID);
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