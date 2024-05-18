using API.Persistence;

namespace API;

public static class StartupLike
{
    public static IApplicationBuilder InitializeDatabase(this IApplicationBuilder app)
    {
        using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var context = serviceScope.ServiceProvider.GetService<CongestionTaxDbContext>();
            DbInitializer.Initialize(context);
        }

        return app;
    }
}