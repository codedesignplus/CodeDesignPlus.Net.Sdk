namespace CodeDesignPlus.Net.Criteria.Test.Extensions;

/// <summary>
/// Un filtro sobre una propiedad opcional (Instant?, int?) se convierte como su tipo de fondo (pendings/277). Antes
/// caía en Convert.ChangeType y lanzaba InvalidCastException.
/// </summary>
public class NullableValuesTest
{
    private sealed class Pass
    {
        public string Name { get; set; } = string.Empty;
        public Instant? ValidUntil { get; set; }
        public int? Seats { get; set; }
    }

    private static readonly Instant Now = Instant.FromUtc(2026, 10, 5, 12, 0);

    private static readonly List<Pass> Passes =
    [
        new() { Name = "Vencido", ValidUntil = Now - Duration.FromHours(1), Seats = 1 },
        new() { Name = "Vigente", ValidUntil = Now + Duration.FromHours(1), Seats = 3 },
        new() { Name = "Sin fin", ValidUntil = null, Seats = null },
    ];

    private static string[] Apply(string filters)
    {
        var criteria = new MC.Criteria { Filters = filters };

        return [.. Passes.AsQueryable().Where(criteria.GetFilterExpression<Pass>()).Select(x => x.Name)];
    }

    [Fact]
    public void AnOptionalInstantIsComparedAsAnInstant()
    {
        var cutoff = Now.ToString();

        Assert.Equal(["Vencido"], Apply($"ValidUntil<{cutoff}"));
        Assert.Equal(["Vigente"], Apply($"ValidUntil>={cutoff}"));
    }

    [Fact]
    public void AnOptionalIntIsComparedAsAnInt()
    {
        Assert.Equal(["Vigente"], Apply("Seats>=2"));
    }
}
