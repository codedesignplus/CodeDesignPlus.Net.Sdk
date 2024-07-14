﻿using System.ComponentModel.DataAnnotations;
using CodeDesignPlus.Net.PubSub.Abstractions.Options;

namespace CodeDesignPlus.Net.Redis.PubSub.Options;

/// <summary>
/// Options to setting of the Redis.PubSub
/// </summary>
public class RedisPubSubOptions
{
    /// <summary>
    /// Name of the setions used in the appsettings
    /// </summary>
    public static readonly string Section = "RedisPubSub";

    /// <summary>
    /// Get or sets the Enable
    /// </summary>
    public bool Enable { get; set; }
    /// <summary>
    /// Get or sets the PubSub options
    /// </summary>
    public PubSubOptions PubSub { get; set; } = new ();
}
