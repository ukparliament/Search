namespace Search
{
    using System;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Rewrite;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Swashbuckle.AspNetCore.SwaggerUI;

    public class Startup
    {
        private readonly Configuration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration.Get<Configuration>();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(IEngine), Type.GetType(this.configuration.Engine));

            services.AddMvc(Startup.SetupMvc);
            services.Configure<RouteOptions>(Startup.ConfigureRouteOptions);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRewriter(new RewriteOptions().AddRewrite("^$", "swagger/index.html", false).AddRewrite("^(swagger|favicon)(.+)$", "swagger/$1$2", true));
            app.UseMvc();
            app.UseSwaggerUI(Startup.ConfigureSwaggerUI);
        }

        private static void SetupMvc(MvcOptions mvc)
        {
            mvc.RespectBrowserAcceptHeader = true;

            mvc.OutputFormatters.Insert(0, new DescriptionFormatter());

            foreach (var mapping in Configuration.Mappings)
            {
                mvc.OutputFormatters.Insert(0, new FeedFormatter(mapping.MediaType, mapping.writeMethod));
                mvc.FormatterMappings.SetMediaTypeMappingForFormat(mapping.Extension, mapping.MediaType);
                mvc.FormatterMappings.SetMediaTypeMappingForFormat(mapping.MediaType, mapping.MediaType);
            }
        }

        private static void ConfigureRouteOptions(RouteOptions routes)
        {
            routes.ConstraintMap.Add("extension", typeof(ExtensionConstraint));
        }

        private static void ConfigureSwaggerUI(SwaggerUIOptions swaggerUI)
        {
            swaggerUI.DocumentTitle = "UK Parliament Search Service";
            swaggerUI.SwaggerEndpoint("./openapi.json", "live");
        }
    }
}
