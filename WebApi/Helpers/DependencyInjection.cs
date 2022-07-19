using WebApi.Model;
using WebApi.Services;

namespace WebApi.Helpers
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JWTAuthOptions>(configuration.GetSection(JWTAuthOptions.JWTAuth));

            services.AddScoped<ITokenManager, TokenManager>();
            services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}
