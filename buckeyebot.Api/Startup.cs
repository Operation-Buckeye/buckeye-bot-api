using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using buckeyebot.Data;
using buckeyebot.Api.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;


namespace buckeyebot.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins("http://localhost:3000")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddDbContext<StoreContext>(options => 
                options.UseSqlite("Data Source=../Registrar.sqlite",
                    b => b.MigrationsAssembly("buckeyebot.Api")));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "jet.piranha.Api v1"));
            }

            app.UseHttpsRedirection();

            app.UseCors();
            app.UseAuthorization();
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            var builder = WebApplication.CreateBuilder(args);

            string authority = builder.Configuration["Auth0: Authority"] ??
            throw new ArgumentNullException("Auth0: Authority");

            string audience = builder.Configuration["Auth0: Audience"] ??
            throw new ArgumentNullException("Auth0: Audience");

            builder.Services.AddControllers();

            builder.Services.AddAuthentication(options => 
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallangeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Authority = authority;
                options.Audience = audience;

            });

            builder.Services.AddAuthorization(options => 

            {
                options.AddPolicy("delete:catalog", policy =>
                    policy.RequireAuthenticatedUser().RequireClaim("scope", "delete:catalog"));  

            });
        }
    }
}
