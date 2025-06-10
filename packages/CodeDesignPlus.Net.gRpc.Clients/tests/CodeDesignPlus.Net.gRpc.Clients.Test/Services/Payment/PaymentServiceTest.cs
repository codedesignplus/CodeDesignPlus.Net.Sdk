using System;
using CodeDesignPlus.Net.gRpc.Clients.Abstractions.Options;
using CodeDesignPlus.Net.gRpc.Clients.Extensions;
using CodeDesignPlus.Net.gRpc.Clients.Services.Payment;

namespace CodeDesignPlus.Net.gRpc.Clients.Test.Services.Payment;

public class PaymentServiceTest
{
    [Fact]
    public async Task PayAsync_Temp()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { $"{GrpcClientsOptions.Section}:PaymenPaymenttUrl", "http://localhost:5001" }
            })
            .Build();

        var services = new ServiceCollection();

        
        services.AddGrpcClients(configuration);

        var serviceProvider = services.BuildServiceProvider();

        var paymentService = serviceProvider.GetRequiredService<IPaymentGrpc>();

        var request = new PayRequest()
        {
            Id = Guid.NewGuid().ToString(),
            Transaction = new Transaction
            {
                Order = new Order
                {
                    Description = "Custom Description",
                    Buyer = new Buyer
                    {
                        FullName = "Wilzon Liscano",
                        EmailAddress = "wliscano@codedesignplus.com",
                        ContactPhone = "3107545341",
                        DniNumber = "1022389704",
                        ShippingAddress = new Address
                        {
                            Street = "Calle 3a 53c 13",
                            Country = "CO",
                            State = "Bogota D.C",
                            City = "Bogota D.C",
                            PostalCode = "111611",
                            Phone = "3107545341"
                        },
                    },
                    Amount = new Amount
                    {
                        Value = 150000,
                        Currency = "COP"
                    },
                    Tax = new Amount
                    {
                        Value = 0,
                        Currency = "COP"
                    },
                    TaxReturnBase = new Amount
                    {
                        Value = 0,
                        Currency = "COP"
                    }
                },
                Payer = new Payer
                {
                    FullName = "Wilzon Liscano Galindo",
                    EmailAddress = "wliscano@codedesignplus.com",
                    DniNumber = "1022389704",
                    DniType = "CC",
                    ContactPhone = "3107545341",
                    BillingAddress = new Address
                    {
                        Street = "Calle 3a 53c 13",
                        Country = "CO",
                        State = "Bogota D.C",
                        City = "Bogota D.C",
                        PostalCode = "111611",
                        Phone = "3107545341"
                    }
                },
                PaymentMethod = "VISA",
                CreditCard = new CreditCard
                {
                    Number = "4037997623271984",
                    SecurityCode = "321",
                    ExpirationDate = "2030/12",
                    Name = "APPROVED"
                },
                Pse = null!,
                DeviceSessionId = "vghs6tvkcle931686k1900o6e1",
                IpAddress = "127.0.0.1",
                Cookie = "pt1t38347bs6jc9ruv2ecpv7o2",
                UserAgent = "Mozilla/5.0 (Windows NT 5.1; rv:18.0) Gecko/20100101 Firefox/18.0"
            }
        };

        try
        {
             await paymentService.PayAsync(request, CancellationToken.None);
            
        }
        catch (Exception ex)
        {
            Assert.Fail($"Payment failed: {ex.Message}");
        }
    }
}

//Request
/*
{
  "id": "{{$guid}}",
  "transaction": {
    "order": {
      "description": "Custom Description",
      "buyer": {
        "fullName": "Wilzon Liscano",
        "emailAddress": "wliscano@codedesignplus.com",
        "contactPhone": "3107545341",
        "dniNumber": "1022389704",
        "shippingAddress": {
          "street": "Calle 3a 53c 13",
          "country": "CO",
          "state": "Bogota D.C",
          "city": "Bogota D.C",
          "postalCode": "111611",
          "phone": "3107545341"
        }
      },
      "ammount": {
        "value": 150000,
        "currency": "COP"
      },
      "tax": {
        "value": 0,
        "currency": "COP"
      },
      "taxReturnBase": {
        "value": 0,
        "currency": "COP"
      }
    },
    "payer": {
      "fullName": "Wilzon Liscano Galindo",
      "emailAddress": "wliscano@codedesignplus.com",
      "dniNumber": "1022389704",
      "dniType": "CC",
      "contactPhone": "3107545341",
      "billingAddress": {
        "street": "Calle 3a 53c 13",
        "country": "CO",
        "state": "Bogota D.C",
        "city": "Bogota D.C",
        "postalCode": "111611",
        "phone": "3107545341"
      }
    },
    "paymentMethod": "VISA",
    "creditCard": {
      "number": "4037997623271984",
      "securityCode": "321",
      "expirationDate": "2030/12",
      "name": "APPROVED"
    },
    "pse": {
      "pseCode": "Bancolombia",
      "pseResponseUrl": "https://mi-tienda.com/respuesta-pse",
      "typePerson": "N"
    },
    "deviceSessionId": "vghs6tvkcle931686k1900o6e1",
    "ipAddress": "127.0.0.1",
    "cookie": "pt1t38347bs6jc9ruv2ecpv7o2",
    "userAgent": "Mozilla/5.0 (Windows NT 5.1; rv:18.0) Gecko/20100101 Firefox/18.0"
  }
}
*/