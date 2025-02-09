﻿using CodeDesignPlus.Net.Core.Abstractions;

namespace CodeDesignPlus.Net.PubSub.Test.Helpers;

/// <summary>
/// Implementación por defecto del servicio <see cref="IStartup"/>
/// </summary>
public class Startup : IStartup
{
    /// <summary>
    /// Este metodo es invocado por el SDK CodeDesignPlus en el momento de iniciar la aplicación para registrar servicios personalizados.
    /// </summary>
    /// <param name="services">Provee acceso al contenedor de dependencias de .net core</param>
    /// <param name="configuration">Provee acceso a las diferentes fuentes de configuración</param>
    public void Initialize(IServiceCollection services, IConfiguration configuration)
    {
       
    }
}