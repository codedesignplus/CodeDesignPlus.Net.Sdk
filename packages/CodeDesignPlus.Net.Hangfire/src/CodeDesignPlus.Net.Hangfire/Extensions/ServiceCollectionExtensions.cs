namespace CodeDesignPlus.Net.Hangfire.Extensions;

/// <summary>
/// Métodos de extensión para registrar Hangfire en el ecosistema CodeDesignPlus.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registra Hangfire en el contenedor de DI usando Redis (vía <see cref="IRedisFactory"/>)
    /// como storage, y auto-descubre los jobs marcados con
    /// <see cref="RecurringJobOptionsAttribute"/>.
    /// </summary>
    /// <typeparam name="TProgram">Tipo del punto de entrada del microservicio.
    /// Se usa para obtener el ensamblado donde se buscan los jobs.</typeparam>
    /// <param name="services">Colección de servicios de DI.</param>
    /// <param name="configuration">Configuración de la aplicación.</param>
    /// <returns>La misma colección de servicios para encadenamiento.</returns>
    public static IServiceCollection AddHangfire<TProgram>(this IServiceCollection services, IConfiguration configuration) 
        where TProgram : class
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var options = new HangfireOptions();
        configuration.GetSection(HangfireOptions.Section).Bind(options);

        if (!options.Enable)
            return services;

        services
            .AddOptions<HangfireOptions>()
            .Bind(configuration.GetSection(HangfireOptions.Section))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddScoped<IJobService, JobService>();

        services.AddHangfire((serviceProvider, config) =>
        {
            var redisFactory = serviceProvider.GetRequiredService<IRedisFactory>();
            var redis = redisFactory.Create(FactoryConst.RedisCore);

            config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseRedisStorage(redis.Connection, new RedisStorageOptions
                {
                    Prefix = options.Prefix
                });
        });

        services.AddHangfireServer(serverOptions =>
        {
            serverOptions.WorkerCount = options.WorkerCount;
            serverOptions.Queues = options.Queues;
        });

        RegisterRecurrentJobs<TProgram>(services);

        return services;
    }

    /// <summary>
    /// Monta el dashboard de Hangfire (solo si <c>Dashboard.Enable = true</c> en la configuración)
    /// y registra automáticamente los recurring jobs descubiertos vía reflexión.
    /// </summary>
    /// <typeparam name="TProgram">Tipo del punto de entrada del microservicio.</typeparam>
    /// <param name="app">Constructor de la aplicación.</param>
    /// <param name="configuration">Configuración de la aplicación.</param>
    /// <returns>El mismo <see cref="IApplicationBuilder"/> para encadenamiento.</returns>
    public static IApplicationBuilder UseHangfireDashboard<TProgram>(
        this IApplicationBuilder app,
        IConfiguration configuration)
        where TProgram : class
    {
        ArgumentNullException.ThrowIfNull(app);
        ArgumentNullException.ThrowIfNull(configuration);

        var options = new HangfireOptions();
        configuration.GetSection(HangfireOptions.Section).Bind(options);

        if (!options.Enable)
            return app;

        if (options.Dashboard.Enable)
        {
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                DarkModeEnabled = true,
                Authorization =
                [
                    new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions
                    {
                        RequireSsl = false,
                        SslRedirect = false,
                        LoginCaseSensitive = true,
                        Users =
                        [
                            new BasicAuthAuthorizationUser
                            {
                                Login = options.Dashboard.Username,
                                PasswordClear = options.Dashboard.Password
                            }
                        ]
                    })
                ]
            });
        }

        RegisterRecurringJobs<TProgram>(app.ApplicationServices);

        return app;
    }

    /// <summary>
    /// Registra como servicios Scoped todas las clases del ensamblado de
    /// <typeparamref name="TProgram"/> que implementen <see cref="IRecurrentJob"/>.
    /// </summary>
    private static void RegisterRecurrentJobs<TProgram>(IServiceCollection services)
        where TProgram : class
    {
        var assembly = typeof(TProgram).Assembly;

        var jobTypes = assembly.GetTypes()
            .Where(t => typeof(IRecurrentJob).IsAssignableFrom(t)
                     && t.IsClass
                     && !t.IsAbstract)
            .ToList();

        foreach (var jobType in jobTypes)
            services.AddScoped(jobType);
    }

    /// <summary>
    /// Registra en Hangfire como recurring jobs todas las clases del ensamblado de
    /// <typeparamref name="TProgram"/> que implementen <see cref="IRecurrentJob"/> y
    /// estén decoradas con <see cref="RecurringJobOptionsAttribute"/>.
    /// </summary>
    private static void RegisterRecurringJobs<TProgram>(IServiceProvider serviceProvider)
        where TProgram : class
    {
        var assembly = typeof(TProgram).Assembly;

        var jobTypes = assembly.GetTypes()
            .Where(t => typeof(IRecurrentJob).IsAssignableFrom(t)
                     && t.IsClass
                     && !t.IsAbstract
                     && t.GetCustomAttribute<RecurringJobOptionsAttribute>() != null)
            .ToList();

        if (jobTypes.Count == 0)
            return;

        var recurringJobManager = serviceProvider.GetRequiredService<IRecurringJobManager>();

        foreach (var jobType in jobTypes)
        {
            var attr = jobType.GetCustomAttribute<RecurringJobOptionsAttribute>()!;
            var jobId = attr.JobId ?? ToKebabCase(jobType.Name);

            // Construye la lambda: (TJob job) => job.ExecuteAsync(JobCancellationToken.Null)
            var parameter = Expression.Parameter(jobType, "job");
            var cancelToken = Expression.Constant(JobCancellationToken.Null, typeof(IJobCancellationToken));
            var methodInfo = jobType.GetMethod(nameof(IRecurrentJob.ExecuteAsync))!;
            var methodCall = Expression.Call(parameter, methodInfo, cancelToken);

            var lambdaDelegateType = typeof(Action<>).MakeGenericType(jobType);
            var lambda = Expression.Lambda(lambdaDelegateType, methodCall, parameter);

            var addOrUpdateMethod = typeof(IRecurringJobManager)
                .GetMethods()
                .First(m => m.Name == nameof(IRecurringJobManager.AddOrUpdate)
                         && m.IsGenericMethod
                         && m.GetParameters().Length == 4)
                .MakeGenericMethod(jobType);

            addOrUpdateMethod.Invoke(recurringJobManager,
            [
                jobId,
                lambda,
                attr.CronExpression,
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.FindSystemTimeZoneById(attr.Timezone)
                }
            ]);
        }
    }

    /// <summary>
    /// Convierte un nombre en PascalCase a kebab-case.
    /// Ejemplo: "DailyReportJob" → "daily-report-job".
    /// </summary>
    private static string ToKebabCase(string name)
    {
        return string.Concat(name.Select((c, i) =>
            i > 0 && char.IsUpper(c)
                ? "-" + char.ToLower(c)
                : char.ToLower(c).ToString()));
    }
}
