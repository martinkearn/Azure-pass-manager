using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.PlatformAbstractions;
using System.Reflection;
using Swashbuckle.AspNetCore.Swagger;
using System.IO;
using APM.Api.Models;

namespace APM.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // format the version as "'v'major[.minor][-status]"
            services.AddMvcCore().AddVersionedApiExplorer(options => options.GroupNameFormat = "'v'VVV");
            services.AddMvc();

            // Add app settings
            var appSettings = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettings);

            // Add app secret settings
            services.AddOptions();
            services.Configure<AppSecretSettings>(Configuration);

            #region Swagger

            services.AddSwaggerGen(
                options =>
                {
                    var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description, Configuration));
                    }

                    options.DescribeAllEnumsAsStrings();
                }
            );

            #endregion

            #region ApiVersioning

            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(0, 1);
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;

            });

            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();


            #region Swagger

            app.UseSwagger();
            app.UseSwaggerUI(
                options =>
                {
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint(
                            $"/swagger/{description.GroupName}/swagger.json",
                            description.GroupName.ToUpperInvariant()
                        );
                    }
                }
            );

            #endregion
        }

        static string XmlCommentsFilePath
        {
            get
            {
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var fileName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name + ".xml";
                return Path.Combine(basePath, fileName);
            }
        }

        static Info CreateInfoForApiVersion(ApiVersionDescription description, IConfiguration Configuration)
        {
            var _appName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            var info = new Info()
            {
                Title = $"{_appName} API {description.ApiVersion}",
                Version = description.ApiVersion.ToString(),
                Description = "TODO",
                Contact = new Contact()
                {
                    Name = Configuration.GetSection("About")["AuthorName"],
                    Email = Configuration.GetSection("About")["AuthorEmail"],
                    Url = Configuration.GetSection("About")["AuthorUrl"]
                },
                TermsOfService = _appName,
                License = new License()
                {
                    Name = Configuration.GetSection("About")["LicenseName"],
                    Url = Configuration.GetSection("About")["LicenseUrl"]
                }
            };


            if (description.IsDeprecated)
            {
                info.Description += " This API version has been deprecated.";
            }

            return info;
        }
    }
}
