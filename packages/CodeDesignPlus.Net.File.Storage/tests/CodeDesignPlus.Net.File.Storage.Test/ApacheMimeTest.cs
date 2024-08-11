namespace CodeDesignPlus.Net.File.Storage.Test;

public class ApacheMimeTest
{

    [Fact]
    public void Temp()
    {
        // Arrange
        var apacheMimes = ApacheMime.ApacheMimes;


        var apacheMimes2 = ApacheMime.ApacheMimes;

        // Act
        var result = apacheMimes.Count;

        // Assert
        Assert.True(result > 0);
    }
}
