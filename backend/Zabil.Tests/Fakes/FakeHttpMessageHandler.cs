namespace Zabil.Tests.Fakes;

public sealed class FakeHttpMessageHandler(HttpResponseMessage response) : HttpMessageHandler
{
    public HttpRequestMessage? LastRequest { get; private set; }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        LastRequest = request;
        return Task.FromResult(response);
    }
}
