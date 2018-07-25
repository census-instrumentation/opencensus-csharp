using System.Collections.Generic;
using Grpc.Core;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Steeltoe.Management.Census.Impl.Trace.Export.Grpc;
using Steeltoe.Management.Census.Impl.Trace.Listeners;
using Steeltoe.Management.Census.Trace;
using Steeltoe.Management.Census.Trace.Export;

namespace WebSampleApp
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
            services.AddSingleton<ITraceComponent, TraceComponent>();
            services.AddSingleton<IHandler>(new OcdHandler("127.0.0.1:50051", ChannelCredentials.Insecure));

            var subs = new DiagnosticSourceSubscriber(new HashSet<string>{"Microsoft.AspNetCore", "HttpHandlerDiagnosticListener" });

            services.AddSingleton<DiagnosticSourceSubscriber>(subs);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
            IHostingEnvironment env,
//            TelemetryConfiguration config,
            ITraceComponent traceComponent,
            IHandler ocd,
            DiagnosticSourceSubscriber subscriber,
            IApplicationLifetime applicationLifetime)
        {
            traceComponent.ExportComponent.SpanExporter.RegisterHandler("ocd", ocd);
            subscriber.Subscribe();
            applicationLifetime.ApplicationStopping.Register(subscriber.Dispose);
//            config.DisableTelemetry = true;
//            TelemetryConfiguration.Active.DisableTelemetry = true;


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
