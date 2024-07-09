namespace CodeDesignPlus.Net.PubSub.Test.Exceptions
{
    /// <summary>
    /// Pruebas unitarias a la clase <see cref="EventNotExistException"/>
    /// </summary>
    public class EventNotExistExceptionTest
    {
        /// <summary>
        /// Valida el constructor por defecto de la excepción
        /// </summary>
        [Fact]
        public void Constructor_WithoutArguments_Exception()
        {
            // Arrange & Act
            var exception = new EventNotExistException();

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
            var exception = new EventNotExistException(message);

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
            var exception = new EventNotExistException(message, innerException);

            // Assert
            Assert.Equal(innerException, exception.InnerException);
            Assert.Equal(message, exception.Message);
            Assert.NotNull(exception.InnerException);
        }

    }
}
