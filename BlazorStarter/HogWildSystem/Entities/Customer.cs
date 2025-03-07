﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HogWildSystem.Entities;

[Table("Customer")]
public partial class Customer
{
    [Key]
    public int CustomerID { get; set; }

    [Required]
    [StringLength(20)]
    [Unicode(false)]
    public string FirstName { get; set; }

    [Required]
    [StringLength(50)]
    [Unicode(false)]
    public string LastName { get; set; }

    [Required]
    [StringLength(50)]
    [Unicode(false)]
    public string Address1 { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Address2 { get; set; }

    [Required]
    [StringLength(50)]
    [Unicode(false)]
    public string City { get; set; }

    public int ProvStateID { get; set; }

    public int CountryID { get; set; }

    [Required]
    [StringLength(20)]
    [Unicode(false)]
    public string PostalCode { get; set; }

    [Required]
    [StringLength(20)]
    [Unicode(false)]
    public string Phone { get; set; }

    [Required]
    [StringLength(250)]
    [Unicode(false)]
    public string Email { get; set; }

    public int StatusID { get; set; }

    public bool RemoveFromViewFlag { get; set; }

    [ForeignKey("CountryID")]
    [InverseProperty("CustomerCountries")]
    public virtual Lookup Country { get; set; }

    [InverseProperty("Customer")]
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    [ForeignKey("ProvStateID")]
    [InverseProperty("CustomerProvStates")]
    public virtual Lookup ProvState { get; set; }

    [ForeignKey("StatusID")]
    [InverseProperty("CustomerStatuses")]
    public virtual Lookup Status { get; set; }
}