using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NodaTime.Text;

namespace CodeDesignPlus.Net.EFCore.Converters;

/// <summary>
/// Value converter for <see cref="Instant"/> to <see cref="DateTime"/>.
/// </summary>
public class InstantValueConverter : ValueConverter<Instant, DateTime>
{
    /// <summary>
    /// Initializes a new instance of <see cref="InstantValueConverter"/>.
    /// </summary>
    public InstantValueConverter() 
        : base(instant => instant.ToDateTimeUtc(), dateTime => Instant.FromDateTimeUtc(dateTime))
    {

    }
}