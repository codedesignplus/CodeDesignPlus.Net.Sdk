using System;

namespace CodeDesignPlus.Net.Microservice.Domain.ValueObjects;

public sealed partial class AddressValueObject
{
    private AddressValueObject(string country, string state, string city, string address, int codePostal)
    {
        this.Country = country;
        this.State = state;
        this.City = city;
        this.Address = address;
        this.CodePostal = codePostal;
    }

    public string Country { get; private set; }
    public string State { get; private set; }
    public string City { get; private set; }
    public string Address { get; private set; }
    public int CodePostal { get; private set; }

    public static AddressValueObject Create(string country, string state, string city, string address, int codePostal)
    {
        return new AddressValueObject(country, state, city, address, codePostal);
    }
}
