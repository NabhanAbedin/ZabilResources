using Microsoft.AspNetCore.Mvc;
using Zabil.Api.Controllers;

namespace Zabil.Tests;

public class HelloControllerTests
{
    [Fact]
    public void Get_ReturnsHelloWorld()
    {
        var controller = new HelloController();

        var result = controller.Get();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Hello World", okResult.Value);
    }
}
