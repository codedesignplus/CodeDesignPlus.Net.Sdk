﻿using CodeDesignPlus.Net.PubSub.Abstractions;

namespace CodeDesignPlus.Net.Kafka.Test.Helpers.Events;


[Topic("product-topic")]
public class ProductCreatedEvent : EventBase
{
    public required string Name { get; set; }
}
