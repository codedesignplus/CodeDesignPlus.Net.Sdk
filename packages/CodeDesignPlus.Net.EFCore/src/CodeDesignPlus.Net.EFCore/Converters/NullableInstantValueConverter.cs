using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NodaTime.Text;

namespace CodeDesignPlus.Net.EFCore.Converters;

/// <summary>
/// Value converter for <see cref="Instant"/> to <see cref="DateTime"/>.
/// </summary>
public class NullableInstantValueConverter : ValueConverter<Instant?, DateTime?>
{
    /// <summary>
    /// Initializes a new instance of <see cref="NullableInstantValueConverter"/>.
    /// </summary>
    public NullableInstantValueConverter() : 
        base(
            instant => instant.HasValue ? instant.Value.ToDateTimeUtc() : null,
            dateTime => dateTime.HasValue ? Instant.FromDateTimeUtc(dateTime.Value) : null
        )
    {

    }
}