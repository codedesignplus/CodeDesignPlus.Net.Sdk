using FluentValidation;
using FluentValidation.Validators;

namespace CodeDesignPlus.Net.xUnit.Microservice.Validations;

/// <summary>
/// Comprueba que los validadores de un ensamblado usen las longitudes estándar de <see cref="FieldLength"/> (regla 48
/// de rules/).
/// </summary>
/// <remarks>
/// La familia de un campo se deduce del nombre de la propiedad. Solo se revisan las propiedades de una familia; los
/// campos con formato propio (códigos, correo, teléfono, documento) no se tocan. Una excepción se declara con
/// <c>Tipo.Propiedad</c> y su motivo en la prueba del micro, nunca cambiando esta clase.
/// </remarks>
public static class StandardFieldLengths
{
    private static readonly string[] NotesNames = ["Notes", "Note", "Comments", "Comment", "Observations", "Observation", "Resolution", "AppealReason"];
    private static readonly string[] TitleNames = ["Title", "Subject", "Topic"];
    private static readonly string[] DescriptionNames = ["Description", "Reason"];

    /// <summary>
    /// Devuelve la longitud estándar de una propiedad, o <c>null</c> si no pertenece a ninguna familia.
    /// </summary>
    /// <param name="propertyName">Nombre de la propiedad; si es una ruta (<c>Contact.Name</c>), cuenta el último tramo.</param>
    public static int? ExpectedLength(string propertyName)
    {
        var name = propertyName[(propertyName.LastIndexOf('.') + 1)..];

        if (NotesNames.Any(x => name.EndsWith(x, StringComparison.Ordinal)))
            return FieldLength.Notes;

        if (TitleNames.Contains(name))
            return FieldLength.Title;

        if (DescriptionNames.Contains(name))
            return FieldLength.Description;

        if (name.EndsWith("Name", StringComparison.Ordinal))
            return FieldLength.Name;

        return null;
    }

    /// <summary>
    /// Busca en el ensamblado los validadores cuya longitud máxima no es la estándar de su familia.
    /// </summary>
    /// <param name="assembly">Ensamblado con los validadores (la capa Application del micro).</param>
    /// <param name="exceptions">Propiedades exentas, como <c>CreateLicenseCommand.TermsOfService</c>.</param>
    /// <returns>Una línea por incumplimiento: validador, propiedad, longitud encontrada y esperada.</returns>
    public static IReadOnlyList<string> FindViolations(Assembly assembly, params string[] exceptions)
    {
        var violations = new List<string>();

        var validators = assembly.GetTypes()
            .Where(x => !x.IsAbstract && !x.IsGenericTypeDefinition && typeof(IValidator).IsAssignableFrom(x))
            .Where(x => x.GetConstructor(Type.EmptyTypes) is not null);

        foreach (var type in validators)
        {
            var validator = (IValidator)Activator.CreateInstance(type)!;
            var descriptor = validator.CreateDescriptor();
            var target = validator.GetType().BaseType?.GetGenericArguments().FirstOrDefault()?.Name ?? type.Name;

            foreach (var member in descriptor.GetMembersWithValidators())
            {
                var expected = ExpectedLength(member.Key);

                if (expected is null || exceptions.Contains($"{target}.{member.Key}"))
                    continue;

                foreach (var (component, _) in member)
                {
                    if (component is ILengthValidator length && length.Max > 0 && length.Max != expected)
                        violations.Add($"{target}.{member.Key}: {length.Max}, se espera {expected}");
                }
            }
        }

        return violations;
    }
}
