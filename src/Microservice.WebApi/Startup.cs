using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using FluentValidation;
using Microservice.WebApi.Models;
using Microservice.WebApi.Services;
using Microservice.WebApi.Validators;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Microservice.WebApi;

[ExcludeFromCodeCoverage]
public class Startup(IConfiguration configuration)
{
    public IConfiguration Configuration { get; } = configuration;

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthentication();

        app.UseAuthorization();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // Add Services
        services.AddSingleton<IProblemsService, ProblemsService>();

        // Add Validators
        services.AddScoped<IValidator<ProblemModel>, ProblemModelValidator>();

        // Setup Controllers
        services
            .AddControllers()
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
            });

        // Add Swagger
        services.AddSwaggerGen(options =>
        {
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Tsa.Submissions.Coding.WebApi", Version = "v1" });

            options.EnableAnnotations();
        });
    }
}
