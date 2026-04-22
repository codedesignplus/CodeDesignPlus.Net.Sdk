namespace CodeDesignPlus.Net.Security.Test.Models;

public class StateTest
{
    [Fact]
    public void State_Create_SetsId_Correctly()
    {
        var id = Guid.NewGuid();
        var state = State.Create(id, "TestState", "TS");
        Assert.Equal(id, state.Id);
    }

    [Fact]
    public void State_Create_SetsName_Correctly()
    {
        var state = State.Create(Guid.NewGuid(), "TestState", "TS");
        Assert.Equal("TestState", state.Name);
    }

    [Fact]
    public void State_Create_SetsCode_Correctly()
    {
        var state = State.Create(Guid.NewGuid(), "TestState", "TS");
        Assert.Equal("TS", state.Code);
    }
}
