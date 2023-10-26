using CodeDesignPlus.Net.Event.Bus.Test.Helpers.Events;
using Newtonsoft.Json;

namespace CodeDesignPlus.Net.Event.Bus.Test.Exceptions
{

    /// <summary>
    /// Pruebas unitarias a la clase <see cref="EventHandlerAlreadyRegisteredException{TEvent, TEventHandler}"/>
    /// </summary>
    public class EventHandlerAlreadyRegisteredExceptionTest
    {
        /// <summary>
        /// Valida el constructor por defecto de la excepción
        /// </summary>
        [Fact]
        public void Constructor_WithoutArguments_Exception()
        {
            // Arrange & Act
            var exception = new EventHandlerAlreadyRegisteredException<UserRegisteredEvent, UserRegisteredEventHandler>();

            // Assert
            Assert.NotEmpty(exception.Message);
            Assert.Null(exception.InnerException);
        }

        /// <summary>
        /// Valida el constructor con el mensaje
        /// </summary>
        [Fact]
        public void Constructor_Message_Exception()
        {
            // Arrange 
            var message = Guid.NewGuid().ToString();

            // Act
            var exception = new EventHandlerAlreadyRegisteredException<UserRegisteredEvent, UserRegisteredEventHandler>(message);

            // Assert
            Assert.Equal(message, exception.Message);
            Assert.Null(exception.InnerException);
        }

        /// <summary>
        /// Valida el constructor con el mensaje y la excepción interna
        /// </summary>
        [Fact]
        public void Constructor_InnerException_Exception()
        {
            // Arrange 
            var message = Guid.NewGuid().ToString();
            var innerException = new InvalidOperationException("The operation is invalid");

            // Act
            var exception = new EventHandlerAlreadyRegisteredException<UserRegisteredEvent, UserRegisteredEventHandler>(message, innerException);

            // Assert
            Assert.Equal(innerException, exception.InnerException);
            Assert.Equal(message, exception.Message);
            Assert.NotNull(exception.InnerException);
        }

        /// <summary>
        /// Valida el constructor con el mensaje y la excepción interna
        /// </summary>
        [Fact]
        public void Constructor_Serealization_Exception()
        {
            // Arrange 
            var message = Guid.NewGuid().ToString();
            var innerException = new InvalidOperationException("The operation is invalid");

            // Act

            var exception = new EventHandlerAlreadyRegisteredException<UserRegisteredEvent, UserRegisteredEventHandler>(message, innerException);

            var serialize = JsonConvert.SerializeObject(exception);

            var result = JsonConvert.DeserializeObject(serialize, typeof(EventHandlerAlreadyRegisteredException<UserRegisteredEvent, UserRegisteredEventHandler>)) as EventHandlerAlreadyRegisteredException<UserRegisteredEvent, UserRegisteredEventHandler>;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Message);
            Assert.NotNull(result.InnerException);
            Assert.Equal(innerException, exception.InnerException);
            Assert.Equal(message, exception.Message);
        }

        [Fact]
        public void EventType_ReturnsCorrectType()
        {
            // Arrange
            var exception = new EventHandlerAlreadyRegisteredException<UserRegisteredEvent, UserRegisteredEventHandler>();

            // Act
            var eventType = exception.EventType;

            // Assert
            Assert.Equal(typeof(UserRegisteredEvent), eventType);
        }

        [Fact]
        public void EventHandlerType_ReturnsCorrectType()
        {
            // Arrange
            var exception = new EventHandlerAlreadyRegisteredException<UserRegisteredEvent, UserRegisteredEventHandler>();

            // Act
            var eventHandlerType = exception.EventHandlerType;

            // Assert
            Assert.Equal(typeof(UserRegisteredEventHandler), eventHandlerType);
        }
    }
}
