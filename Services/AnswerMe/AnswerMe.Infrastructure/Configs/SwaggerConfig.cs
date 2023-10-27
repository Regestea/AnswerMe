using System.Reflection;
using AnswerMe.Infrastructure.Hubs;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace AnswerMe.Infrastructure.Configs
{
    public static class SwaggerConfig
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);



            services.AddEndpointsApiExplorer();

            var hubAssemblies=new List<Assembly>
            {
                typeof(GroupRoomHub).Assembly,
                typeof(OnlineHub).Assembly,
                typeof(PrivateRoomHub).Assembly
            };


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Answer Me", Version = "v1" });

                c.AddSignalRSwaggerGen(ssgOptions =>
                    {
                        ssgOptions.ScanAssemblies(hubAssemblies);
                        ssgOptions.UseHubXmlCommentsSummaryAsTagDescription = true;
                        ssgOptions.UseHubXmlCommentsSummaryAsTag = true;
                        ssgOptions.UseXmlComments(xmlPath);
                    }
                );
                
                var securitySchema = new OpenApiSecurityScheme
                {
                    Description =
                        "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };

                c.AddSecurityDefinition("Bearer", securitySchema);

                var securityRequirement = new OpenApiSecurityRequirement
                {
                    { securitySchema, new[] { "Bearer" } }
                };

                c.AddSecurityRequirement(securityRequirement);
                c.IncludeXmlComments(xmlPath);

            });



            return services;
        }
    }
}