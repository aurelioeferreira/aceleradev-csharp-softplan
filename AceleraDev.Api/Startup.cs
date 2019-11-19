using AceleraDev.Application.Mapping;
using AceleraDev.CrossCutting.IoC;
using AceleraDev.Data.Context;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace AceleraDev.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. 
        // Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                // Desabilitar refer�ncia circular na serialiaz��o dos json
                .AddNewtonsoftJson(opt =>
                    opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

            // Configura��o da inje��o de dependencias
            RegisterIoC.Register(services);

            // Configura��o do AutoMapper
            services.AddAutoMapper(typeof(AutoMappingDomainToViewModel));
            services.AddAutoMapper(typeof(AutoMappingViewModelToDomain));

            // Configura��o do contexto ef
            services.AddDbContext<AceleraDevContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // Configura��o do mongodb
            MongoDbContext.ConnectionString = Configuration.GetConnectionString("mongodb");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
