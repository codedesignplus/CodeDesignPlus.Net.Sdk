using CodeDesignPlus.Net.Core.Abstractions.Attributes;
using CodeDesignPlus.Net.Microservice.Commons.EntryPoints.Rest.Swagger;
using CodeDesignPlus.Net.ValueObjects.Financial;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NodaTime;

namespace CodeDesignPlus.Net.Microservice.Commons.Test.EntryPoints.Rest.Swagger;

// Las mismas formas que tienen los contratos de los micros (pendings/753): un value object del dominio con setters
// privados y constructor [JsonConstructor], fechas de NodaTime, un enum que viaja como entero, dinero de entrada y
// de salida, y JsonIgnore de los dos motores de JSON.

public enum SampleStatus
{
    Draft = 0,
    Active = 1,
    Cancelled = 5
}

/// <summary>
/// Como <c>Unit</c> de parking: setters privados, se llena por el constructor.
/// </summary>
public sealed class SampleSpot
{
    public Guid Id { get; private set; }

    public string Number { get; private set; }

    /// <summary>
    /// Calculada: se escribe en la respuesta y no se lee del body.
    /// </summary>
    public string Label => $"Spot {this.Number}";

    /// <summary>
    /// Como <c>RaffleAssignment.IsHeld</c>: Newtonsoft no la escribe.
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    public bool IsAssigned => this.Number is not null;

    [Newtonsoft.Json.JsonConstructor]
    private SampleSpot(Guid id, string number)
    {
        this.Id = id;
        this.Number = number;
    }

    public static SampleSpot Create(Guid id, string number) => new(id, number);
}

public sealed class CreateSampleDto
{
    public Guid Id { get; set; }

    public LocalDate StartDate { get; set; }

    public LocalDate? EndDate { get; set; }

    public Instant At { get; set; }

    public LocalTime OpensAt { get; set; }

    public Duration Length { get; set; }

    public SampleStatus Status { get; set; }

    public SampleSpot Spot { get; set; }

    public MoneyInput Price { get; set; }

    public string Code { get; set; }

    public string Email { get; set; }

    public int Seats { get; set; }

    public decimal Rate { get; set; }

    public string Notes { get; set; }

    /// <summary>
    /// Como <c>PlatformIssuer</c> de factura electrónica: la pone el endpoint, el body no.
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    public bool PlatformIssuer { get; set; }
}

/// <summary>
/// El comando al que el controller mapea <see cref="CreateSampleDto"/>: el validador es suyo, no del DTO.
/// </summary>
public sealed record CreateSampleCommand(Guid Id, string Code, string Email, int Seats, decimal Rate, string Notes, string Internal);

public sealed class CreateSampleCommandValidator : AbstractValidator<CreateSampleCommand>
{
    public CreateSampleCommandValidator()
    {
        this.RuleFor(x => x.Id).NotEmpty();
        this.RuleFor(x => x.Code).NotEmpty().MaximumLength(32).Matches("^[A-Z0-9-]+$");
        this.RuleFor(x => x.Email).EmailAddress();
        this.RuleFor(x => x.Seats).InclusiveBetween(1, 500);
        this.RuleFor(x => x.Rate).GreaterThan(0);

        // Condicional: no hace obligatorio el campo.
        this.RuleFor(x => x.Notes).NotEmpty().When(x => x.Seats > 100);

        // El DTO no la tiene: no debe aparecer en su esquema.
        this.RuleFor(x => x.Internal).NotEmpty();
    }
}

/// <summary>
/// Como los comandos de factura electrónica, que llegan como body tal cual.
/// </summary>
public sealed record RenameSampleCommand(Guid Id, string Name);

public sealed class RenameSampleCommandValidator : AbstractValidator<RenameSampleCommand>
{
    public RenameSampleCommandValidator()
    {
        this.RuleFor(x => x.Name).NotEmpty().Length(3, 64);
    }
}

public sealed class SampleDto
{
    public Guid Id { get; set; }

    public SampleStatus Status { get; set; }

    public SampleSpot Spot { get; set; }

    public Money Price { get; set; }

    public List<SampleSpot> History { get; set; } = [];

    /// <summary>
    /// Como <c>WithholdingRule.Key</c>: el atributo es de System.Text.Json y en el cable no tiene efecto.
    /// </summary>
    [System.Text.Json.Serialization.JsonIgnore]
    public string Key => $"{this.Id}:{this.Status}";
}

/// <summary>
/// Como el resumen de cartera de invoicing: montos en long junto a la moneda, contadores en int.
/// </summary>
public sealed class SampleSummaryDto
{
    public long TotalDue { get; set; }

    public long? AccruedInterest { get; set; }

    public int Units { get; set; }

    public string Currency { get; set; }

    [NotMoney]
    public long SizeInBytes { get; set; }

    /// <summary>
    /// El File de ValueObjects: su Size es de bytes y lleva [NotMoney] en el SDK.
    /// </summary>
    public CodeDesignPlus.Net.ValueObjects.FileServer.File Attachment { get; set; }
}

/// <summary>
/// Sin la moneda al lado: el long sigue siendo dinero, salvo el marcado (regla 74).
/// </summary>
public sealed class SampleAmountDto
{
    public long Amount { get; set; }

    [NotMoney]
    public long Consecutive { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public sealed class SampleController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult Create([FromBody] CreateSampleDto data) => this.NoContent();

    [HttpPut("{id}/name")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult Rename(Guid id, [FromBody] RenameSampleCommand data) => this.NoContent();

    [HttpGet("{id}")]
    [ProducesResponseType<SampleDto>(StatusCodes.Status200OK)]
    public IActionResult Get(Guid id) => this.Ok(new SampleDto { Id = id });

    [HttpGet("summary")]
    [ProducesResponseType<SampleSummaryDto>(StatusCodes.Status200OK)]
    public IActionResult GetSummary() => this.Ok(new SampleSummaryDto());

    [HttpGet("amount")]
    [ProducesResponseType<SampleAmountDto>(StatusCodes.Status200OK)]
    public IActionResult GetAmount() => this.Ok(new SampleAmountDto());

    [HttpGet("page")]
    [ProducesResponseType<CodeDesignPlus.Net.Core.Abstractions.Models.Pager.Pagination<SampleDto>>(StatusCodes.Status200OK)]
    public IActionResult GetPage() => this.Ok();

    [HttpGet("internal")]
    [ExcludeFromMcp]
    [ProducesResponseType<SampleDto>(StatusCodes.Status200OK)]
    public IActionResult GetInternal() => this.Ok(new SampleDto());

    [HttpGet("public")]
    [AllowAnonymous]
    [ProducesResponseType<SampleDto>(StatusCodes.Status200OK)]
    public IActionResult GetPublic() => this.Ok(new SampleDto());
}

/// <summary>
/// Dos respuestas con tipos que se llaman igual; se monta solo en la prueba de ids de esquema.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class DuplicateNamesController : ControllerBase
{
    [HttpGet("license")]
    [ProducesResponseType<License.DataTransferObjects.LicenseDto>(StatusCodes.Status200OK)]
    public IActionResult GetLicense() => this.Ok(new License.DataTransferObjects.LicenseDto());

    [HttpGet("order-license")]
    [ProducesResponseType<Order.DataTransferObjects.LicenseDto>(StatusCodes.Status200OK)]
    public IActionResult GetOrderLicense() => this.Ok(new Order.DataTransferObjects.LicenseDto());
}
