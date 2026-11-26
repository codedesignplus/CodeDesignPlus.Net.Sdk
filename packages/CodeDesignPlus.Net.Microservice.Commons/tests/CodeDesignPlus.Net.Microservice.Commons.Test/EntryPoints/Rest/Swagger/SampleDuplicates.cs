// Dos tipos que se llaman igual en namespaces distintos, como LicenseDto de License y de Order en ms-licenses
// (pendings/753). Sin SchemaIdSelector la generación del Swagger falla entera.

namespace CodeDesignPlus.Net.Microservice.Commons.Test.EntryPoints.Rest.Swagger.License.DataTransferObjects
{
    public sealed class LicenseDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}

namespace CodeDesignPlus.Net.Microservice.Commons.Test.EntryPoints.Rest.Swagger.Order.DataTransferObjects
{
    public sealed class LicenseDto
    {
        public Guid Id { get; set; }

        public int Seats { get; set; }
    }
}
