using Game.API;
using Game.Domain.GameAggregate;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Test.Game.API.Helpers;

public class WebAppFactory : WebApplicationFactory<Startup>
{
    public Mock<IRandomIntRepository> RandomIntRepositoryMock { get; private set; }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        RandomIntRepositoryMock = new Mock<IRandomIntRepository>();
        RandomIntRepositoryMock.Setup(x => x.Next()).ReturnsAsync(0);
        builder.ConfigureTestServices(services =>
            services.AddScoped<IRandomIntRepository>(sp => RandomIntRepositoryMock.Object));
    }
}