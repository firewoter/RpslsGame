using Game.Domain.GameAggregate;
using Game.Infrastructure;

namespace Game.API;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.Configure<GameConfig>(_configuration.GetSection(nameof(GameConfig)));
        
        services.AddScoped<IGame, Gameplay>();
        services.AddScoped<IGameRules, GameRules>();
        services.AddScoped<IGameMovesRepository, GameMovesAndRulesRepository>();
        services.AddScoped<IGameRulesRepository, GameMovesAndRulesRepository>();
        services.AddScoped<IRandomIntRepository, RandomIntRepository>();
        
        services.AddAutoMapper(typeof(Startup).Assembly);

        services.AddHttpClient();
    }

    public void Configure(
        IApplicationBuilder app,
        IWebHostEnvironment env,
        ILogger<Startup> logger)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.UseHttpsRedirection();
        
        app.UseAuthorization();

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}