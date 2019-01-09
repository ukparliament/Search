// MIT License
//
// Copyright (c) 2019 UK Parliament
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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

            foreach (var mapping in Configuration.QueryMappings)
            {
                mvc.OutputFormatters.Insert(0, new FeedFormatter(mapping.MediaType, mapping.writeMethod));
                mvc.FormatterMappings.SetMediaTypeMappingForFormat(mapping.Extension, mapping.MediaType);
                mvc.FormatterMappings.SetMediaTypeMappingForFormat(mapping.MediaType, mapping.MediaType);
            }

            foreach (var mapping in Configuration.OpenApiMappings)
            {
                mvc.OutputFormatters.Insert(0, new OpenApiFormatter(mapping.MediaType, mapping.WriterType));
                mvc.FormatterMappings.SetMediaTypeMappingForFormat(mapping.Extension, mapping.MediaType);
                mvc.FormatterMappings.SetMediaTypeMappingForFormat(mapping.MediaType, mapping.MediaType);
            }
        }

        private static void ConfigureRouteOptions(RouteOptions routes)
        {
            routes.ConstraintMap.Add("query", typeof(QueryExtensionConstraint));
            routes.ConstraintMap.Add("openapi", typeof(OpenApiExtensionConstraint));
        }

        private static void ConfigureSwaggerUI(SwaggerUIOptions swaggerUI)
        {
            swaggerUI.DocumentTitle = "UK Parliament Search Service";
            swaggerUI.SwaggerEndpoint("./openapi", "live");
        }
    }
}
