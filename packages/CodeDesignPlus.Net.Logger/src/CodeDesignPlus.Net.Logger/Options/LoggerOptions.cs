using System.ComponentModel.DataAnnotations;

namespace CodeDesignPlus.Net.Logger.Options
{
    public class LoggerOptions : IValidatableObject
    {
        public const string Section = "Logger";

        public bool Enable { get; set; }

        [Url]
        public string OTelEndpoint { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (Enable)
                Validator.TryValidateProperty(OTelEndpoint, new ValidationContext(this) { MemberName = nameof(OTelEndpoint) }, results);

            return results;
        }
    }
}
