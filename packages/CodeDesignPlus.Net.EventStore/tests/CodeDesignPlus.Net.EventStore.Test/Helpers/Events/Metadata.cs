namespace CodeDesignPlus.Net.EventStore.Test.Helpers.Events;

public class Metadata
{
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string EventOrigin { get; set; } = "Application"; // Por defecto es "Aplicación", pero podría ser "Web", "Mobile", etc.
    // Otros campos comunes a todos los metadatos pueden ser agregados aquí
}

public class OrderMetadata : Metadata
{
    public Guid? LastModifiedByUserId { get; set; } // El ID del usuario que modificó por última vez la orden
    public required string OrderChannel { get; set; } // Ejemplo: "Web", "MobileApp", "Phone"
    // Otros campos específicos de la orden pueden ser agregados aquí
}

public class ProductMetadata : Metadata
{
    public bool IsPromotionalItem { get; set; } // Si el producto es un item promocional
    public required string Supplier { get; set; } // Proveedor del producto
    // Otros campos específicos del producto pueden ser agregados aquí
}

public class ClientMetadata : Metadata
{
    public required string ClientSegment { get; set; } // Ejemplo: "Regular", "VIP", "Gold"
    public bool HasActiveSubscription { get; set; } // Si el cliente tiene una suscripción activa
    // Otros campos específicos del cliente pueden ser agregados aquí
}