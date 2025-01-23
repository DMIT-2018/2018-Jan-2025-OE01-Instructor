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
	GetManagerAndSubordinates().Dump();
}

#region Methods
public List<ManagerView> GetManagerAndSubordinates()
{
	return Employees
		.Select(x => new ManagerView
		{
			FirstName = x.FirstName,
			LastName = x.LastName,
			Position = x.Title,
			//Since this is a self referencing table (Employees => Employees)
			//We can use .Children to find the child records, or .Parent to find the parent.
			Subordinates = x.Children
				//SubordinateView does not have a field for LastName, so we have to orderBy before
				//we shape (select) the data
				.OrderBy(c => c.LastName)
				.Select(c => new SubordinateView
				{
					Name = $"{c.FirstName} {c.LastName}",
					Title = c.Title
				}).ToList()	
		})
		//Manager view has LastName so I can orderBy after
		.OrderBy(x => x.LastName).ToList();
}
#endregion

#region ViewModels
public class ManagerView 
{
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public string Position { get; set; }
	public List<SubordinateView> Subordinates { get; set; }
}

public class SubordinateView
{
	public string Name { get; set; }
	public string Title { get; set; }
}
#endregion