
using GestaoFacil.Dados;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using GestaoFacil.Dados.Repositories.Interfaces;
using GestaoFacil.Dados.Repositories;
using AutoMapper;
using Microsoft.OpenApi.Models;
using GestaoFacil.Dados.Helpers;
using System.Linq;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;

namespace GestaoFacil.WebApi
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

            #region AutoMapper configuração
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile(new DTOMapperProfile());
            });
            #endregion

            IMapper mapper = config.CreateMapper();
            services.AddSingleton(mapper);


            services.AddControllers();
            services.AddDbContext<Context>(options => options.UseSqlServer(Util.GetConnectionString("Base")));

            //Registrando como serviço minhas interfaces pra ser usado nos controles... 
            services.AddTransient<IFuncionarioRepository,  FuncionarioRepository>();
            services.AddTransient<IServicoRepository, ServicoRepository>();
            //services.AddScoped<IServicoRepository, ServicoRepository>();

            services.AddMvc();
            services.AddMvc(options => options.EnableEndpointRouting = false);

            services.AddApiVersioning(cfg =>
            {
                cfg.ReportApiVersions = true;  // essa opção vai mostra no HEADERS as versiões suportada ex:(spi-suported  = 1.0 ,2.0)  //// ativar a disponibilização do versionamento da API
                cfg.AssumeDefaultVersionWhenUnspecified = true; // parâmetro complementar ao ".DefaultApiVersion"
                cfg.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0); // versão default - padrão ou sugerida.

            });

            services.AddSwaggerGen(cfg => {
                cfg.ResolveConflictingActions(apiDescription => apiDescription.First()); // para resolver conflitos de versões da API no Swagger, com mesmo nome.
                cfg.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "V1",
                    Title = "GestaoFacil API"

                });

                //WebApi.xml
                var CaminhoProjeto = PlatformServices.Default.Application.ApplicationBasePath;   //recupera o caminho do projeto...
                var NomeProjeto = $"{PlatformServices.Default.Application.ApplicationName}.xml"; //recupera o nome do projeto...
                var CaminhoArquivoXMLComentario = Path.Combine(CaminhoProjeto, NomeProjeto);
                cfg.IncludeXmlComments(CaminhoArquivoXMLComentario);
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            // app.UseRouting();
            app.UseDeveloperExceptionPage();
            app.UseMvc();


            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "MimicAPI");
                c.RoutePrefix = string.Empty;
            });


        }
    }
}
