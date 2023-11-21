using CodeDesignPlus.Net.PubSub.Test.Helpers.Events;

namespace CodeDesignPlus.Net.PubSub.Test.Abstractions;

/// <summary>
/// Pruebas unitarias a la clase <see cref="Bus.Abstractions.EventBase"/>
/// </summary>
public class EventBaseTest
{
    /// <summary>
    /// Valida que se genere automaticamente el identificador y la fecha del evento
    /// </summary>
    [Fact]
    public void Constructor_Default_GetValues()
    {
        //Arrange
        var date = DateTime.UtcNow;

        //Act
        var PubSub = new UserRegisteredEvent()
        {
            Id = new Random().Next(1, 1000),
            Age = (ushort)new Random().Next(1, 100),
            Name = nameof(UserRegisteredEvent.Name),
            User = nameof(UserRegisteredEvent.User),
        };

        //Assert
        Assert.True(PubSub.Id > 0);
        Assert.NotEmpty(PubSub.IdEvent.ToString());
        Assert.True(PubSub.EventDate > date);
        Assert.Equal(nameof(UserRegisteredEvent.Name), PubSub.Name);
        Assert.Equal(nameof(UserRegisteredEvent.User), PubSub.User);
        Assert.True(PubSub.Age > 0);
    }

    /// <summary>
    /// Valida que se asigne el identificador y la fecha del evento
    /// </summary>
    [Fact]
    public void Constructor_Overload_GetValues()
    {
        //Arrange
        var date = DateTime.UtcNow;
        var guid = Guid.NewGuid();

        //Act
        var PubSub = new UserRegisteredEvent()
        {
            Id = new Random().Next(1, 1000),
            EventDate = date,
            IdEvent = guid,
            Age = (ushort)new Random().Next(1, 100),
            Name = nameof(UserRegisteredEvent.Name),
            User = nameof(UserRegisteredEvent.User),
        };

        //Assert
        Assert.True(PubSub.Id > 0);
        Assert.Equal(guid.ToString(), PubSub.IdEvent.ToString());
        Assert.Equal(PubSub.EventDate, date);
        Assert.Equal(nameof(UserRegisteredEvent.Name), PubSub.Name);
        Assert.Equal(nameof(UserRegisteredEvent.User), PubSub.User);
        Assert.True(PubSub.Age > 0);
    }

    /// <summary>
    /// Valida que se genere la excepción si la fecha es menor a DateTime.Min
    /// </summary>
    [Fact]
    public void Constructor_Overload_Equals()
    {
        //Arrange
        var idEvent1 = Guid.NewGuid();
        var idEvent2 = idEvent1;

        var event1 = new UserRegisteredEvent()
        {
            IdEvent = idEvent1,
            EventDate = DateTime.UtcNow,
            Name = nameof(UserRegisteredEvent.Name),
            User = nameof(UserRegisteredEvent.User)
        };


        var event2 = new UserRegisteredEvent()
        {
            IdEvent = idEvent2,
            EventDate = DateTime.UtcNow,
            Name = nameof(UserRegisteredEvent.Name),
            User = nameof(UserRegisteredEvent.User)
        };


        //Act
        var result = event1.Equals(event2);

        //Assert
        Assert.Equal(event1, event2);
    }

    /// <summary>
    /// Valida que se genere la excepción si la fecha es menor a DateTime.Min
    /// </summary>
    [Fact]
    public void Constructor_Overload_GetHashCode()
    {
        //Arrange
        var idEvent1 = Guid.NewGuid();
        var expected = HashCode.Combine(idEvent1);

        var event1 = new UserRegisteredEvent()
        {
            IdEvent = idEvent1,
            EventDate = DateTime.UtcNow,
            Name = nameof(UserRegisteredEvent.Name),
            User = nameof(UserRegisteredEvent.User)
        };


        //Act
        var result = event1.GetHashCode();

        //Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Equals_OtherIsNull_ReturnFalse()
    {
        //Arrange
        var idEvent1 = Guid.NewGuid();

        var event1 = new UserRegisteredEvent()
        {
            IdEvent = idEvent1,
            EventDate = DateTime.UtcNow,
            Name = nameof(UserRegisteredEvent.Name),
            User = nameof(UserRegisteredEvent.User)
        };

        //Act
        var result = event1.Equals(null);

        //Assert
        Assert.False(result);
    }
}