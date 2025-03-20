<Query Kind="Program">
  <Connection>
    <ID>6ebe7b82-7b43-4450-84c8-0a19b4eb5325</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Driver Assembly="(internal)" PublicKeyToken="no-strong-name">LINQPad.Drivers.EFCore.DynamicDriver</Driver>
    <AllowDateOnlyTimeOnly>true</AllowDateOnlyTimeOnly>
    <Server>MOMSDESKTOP\SQLEXPRESS</Server>
    <Database>Territory</Database>
    <DriverData>
      <EncryptSqlTraffic>True</EncryptSqlTraffic>
      <PreserveNumeric1>True</PreserveNumeric1>
      <EFProvider>Microsoft.EntityFrameworkCore.SqlServer</EFProvider>
    </DriverData>
  </Connection>
</Query>

void Main()
{
	GetCountry(1).Dump();
	
	CountryView mexico = new CountryView
	{
		CountryID = 0,
		CountryName = "Mexico",
		Provinces = new List<ProvinceView>
		{
			new ProvinceView
			{
				ProvinceID = 0, //0 cause it is new
				ProvinceName = "Jalisco",
				Area = 78599,
				Cities = new List<CityView>
				{
					new CityView
					{
						CityID = 0, //0 cause it is new
						CityName = "Guadalajara",
						Population = 1500000
					},
					new CityView
					{
						CityID = 0,
						CityName = "Puerto Vallarta",
						Population = 300000
					}
				}
			},
			new ProvinceView
			{
				ProvinceID = 0, // New province
	            ProvinceName = "Nuevo León",
				Area = 64220, // Area in square kilometers
	            Cities = new List<CityView>
				{
					new CityView
					{
						CityID = 0, // New city
	                    CityName = "Monterrey",
						Population = 1100000
					},
					new CityView
					{
						CityID = 0, // New city
	                    CityName = "San Pedro Garza García",
						Population = 132000
					}
				}
			}
		}
	};

	mexico.Dump();
	
	AddEditCountry(mexico);
	
	GetCountry(3).Dump();
	}

// You can define other methods, fields, classes and namespaces here

#region Methods
public CountryView GetCountry(int countryID)
{
	//rule: countryID must be valid
	if(countryID == 0)
	{
		throw new ArgumentNullException("CountryID must be provided.");
	}
	return Countries
		.Where(x => x.CountryID == countryID)
		.Select(c => new CountryView
		{
			CountryID = c.CountryID,
			CountryName = c.CountryName,
			Provinces	= c.Provinces
				.Select(p => new ProvinceView
				{
					ProvinceID = p.ProvinceID,
					CountryID = p.CountryID,
					ProvinceName = p.ProvinceName,
					Area = p.Area,
					Cities = p.Cities
						.Select(x => new CityView
						{
							CityID = x.CityID,
							ProvinceID = x.ProvinceID,
							CityName = x.CityName,
							Population = x.Population
						})
						.OrderBy(x => x.CityName).ToList()
				})
				.OrderBy(p => p.ProvinceName)
				.ToList()
		}).FirstOrDefault();
}

public void AddEditCountry(CountryView countryView)
{
	//rule: countryView cannot be null
	if (countryView == null)
		throw new ArgumentNullException(nameof(countryView));
		
	//Add Error Checking here if needed
	
	// Check if country already exists
	Country country = Countries
						.Where(x => x.CountryID == countryView.CountryID)
						.FirstOrDefault(); // Will return null if the country ID is not found
						
	// Check if null - make new
	if(country == null)
		country = new Country();
	
	//Update country properties
	country.CountryName = countryView.CountryName;
	
	// Loop through each Province
	foreach(var provinceView in countryView.Provinces)
	{
		// Check if the province already exists
		Province province = Provinces
								.Where(p => p.ProvinceID == provinceView.ProvinceID)
								.Include(p => p.Cities)
								.FirstOrDefault();

		if (province == null)
		{
			province = new Province();
		}

		// Update the province properties
		province.ProvinceName = provinceView.ProvinceName;
		province.Area = provinceView.Area;
		
		//Loop through cities (inside the province loop)
		foreach(var cityView in provinceView.Cities)
		{
			// Check if the city already exists
			City city = Cities
							.Where(c => c.CityID == cityView.CityID)
							.FirstOrDefault();

			if (city == null)
			{
				city = new City();
			}

			city.CityName = cityView.CityName;
			city.Population = cityView.Population;

			//Check if new or update
			if (city.CityID == 0)
			{
				//Add it to our instance of a province record
				province.Cities.Add(city);
			}
			else
			{
				//We cannot update on the instance of the province
				//We update directly to the database.
				Cities.Update(city);
			}
		}
		//Check if province is new or update
		//Note: Make sure this is before you leave the province foreach loop
		if (province.ProvinceID == 0)
		{
			//Add it to our instance of a country record
			country.Provinces.Add(province);
		}
		else
		{
			//We cannot update on the instance of the country
			//We update directly to the database.
			Provinces.Update(province);
		}
	}
	//Check if the country is new or update
	if (country.CountryID == 0)
	{
		//Since this is the parent (top level) record, 
		//we add directly to the database.
		Countries.Add(country);
	}
	else
	{
		//Update directly in the database.
		Countries.Update(country);
	}
	try
	{
		SaveChanges();	
	}
	catch (Exception ex)
	{
		throw new Exception("An error occurred while saving Country data.", ex);
	}
}
#endregion

#region View Models
public class CountryView
{
	public int CountryID { get; set; }
	public string CountryName { get; set; }
	public List<ProvinceView> Provinces { get; set; } = new();
}

public class ProvinceView
{
	public int ProvinceID { get; set; }
	public int CountryID { get; set; }
	public string ProvinceName { get; set; }
	public int Area { get; set; }
	public List<CityView> Cities { get; set; } = new();
}

public class CityView
{
	public int CityID { get; set; }
	public int ProvinceID { get; set; }
	public string CityName { get; set; }
	public int Population { get; set; }
}
#endregion