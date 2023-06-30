using A3_API_Project.Identity;
using A3_API_Project.Interfaces;
using A3_API_Project.Models.Cinema.DTO;
using A3_API_Project.Models.IDP;
using A3_API_Project.Repository;
using A3_API_Project.Repository.Interfaces;
using A3_API_Project.Repository.Utils;
using A3_API_Project.Services;
using AspNetCoreRateLimit;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;

namespace A3_API_Project
{
    public class Startup
    {
        private byte[] _secretKey = Encoding.ASCII.GetBytes("MINHA_CHAVE_SECRETA_AQUI");

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(ConfigurarJwtBearerOptions);

            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.WithOrigins("http://localhost:3000", "http://localhost:3000/Quiz", "http://127.0.0.1:50714", "http://localhost:50714")
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));
            //services.AddControllersWithViews();
            services.AddControllers()
                 .AddFluentValidation(config => config.RegisterValidatorsFromAssemblyContaining<CinemaDTO>());


            // configura swagger
            services.AddSwaggerGen(ConfigurarSwaggerGenOptions);
            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1",
            //        new OpenApiInfo
            //        {
            //            Title = "A3_API_Project",
            //            Version = "v1"
            //        });

                // Config para mostrar os comentarios <sumary> contidos nos endpoints da camada CONTROLLER no SWAGGER
                //
                //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                //c.CustomOperationIds(opId =>
                //{
                //    return $"{ opId.ActionDescriptor.RouteValues["controller"]}_{opId.ActionDescriptor.RouteValues["action"]}";
                //});
                //c.IncludeXmlComments(xmlPath);
            //});




            // services
            services.AddTransient<CinemaRepository>();
            services.AddTransient<AuthRepository>();
            services.AddTransient<LocalidadeRepository>();

            // utils
            services.AddScoped<DbSession>();
            services.AddControllers().AddNewtonsoftJson();


            // AspnetCoreRateLimit - limitar acesso a endpoint
            // https://github.com/stefanprodan/AspNetCoreRateLimit/wiki/IpRateLimitMiddleware#setup
            services.AddOptions();
            services.AddMemoryCache();
            services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            services.AddHttpContextAccessor();
            services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

            // identity
            services.AddTransient<IUserStore<ApplicationUser>, UserStore>();
            services.AddTransient<IRoleStore<ApplicationRole>, RoleStore>();
            services.AddIdentity<ApplicationUser, ApplicationRole>().AddDefaultTokenProviders();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddTransient<IEmailSender, EmailSender>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors("MyPolicy");


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // For mobile apps, allow http traffic.
                app.UseHttpsRedirection();
            }

            app.UseSwagger();
            //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));

            app.UseSwaggerUI(c =>
            {
                //c.RoutePrefix = string.Empty;
                //c.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Model);
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
            });


            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            /* AspnetCoreRateLimit - limitar acesso a endpoint
             * https://github.com/stefanprodan/AspNetCoreRateLimit/wiki/IpRateLimitMiddleware#setup
             *
             * se usar este método abaixo, ativa o limite de chamadas aos endpoints */
            //app.UseIpRateLimiting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void ConfigurarJwtBearerOptions(JwtBearerOptions jwtOptions)
        {

            jwtOptions.RequireHttpsMetadata = false;
            jwtOptions.SaveToken = true;
            jwtOptions.IncludeErrorDetails = true;
            jwtOptions.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(_secretKey),
                ValidateIssuer = false,
                ValidateAudience = false,
                //  altera o padrao que é de cinco minutos de expiracao no minimo para zero
                ClockSkew = TimeSpan.Zero
            };

            jwtOptions.Events = new JwtBearerEvents
            {
                OnTokenValidated = async context =>
                {
                    UserManager<ApplicationUser> userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();

                    var user = userManager.GetUserAsync(context.HttpContext.User).ConfigureAwait(false).GetAwaiter().GetResult();

                    if (user is null)
                    {
                        var err = new { Erro = "Usuário não encontrado. (Mensagem para uso em modo DESENVOLVIMENTO)" };
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = MediaTypeNames.Application.Json;
                        await context.Response.BodyWriter.WriteAsync(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(err)));

                        // necessário para injetar o erro dentro do contexto atual da Response invés do contexto da controller
                        context.Fail(err.Erro);
                    }

                    // recupera as permissões do contexto do token em uso
                    List<Claim> perfisUserNoContexto = context.Principal.FindAll(x => x.Type == ClaimTypes.Role).ToList();

                    // recupera as permissões de usuário devidamente cadastradas no momento
                    List<Claim> perfisUserCadastrados = userManager.GetClaimsAsync(user).ConfigureAwait(false).GetAwaiter().GetResult()
                        .Where(x => x.Type == ClaimTypes.Role).ToList();


                    // se no contexto ele tiver mais permissões que as cadastradas, algo está errado...
                    // pode ter sido deletado ou removido alguma permissão do usuário e o token não está atualizado.
                    // Validar se as atuais estao de acordo com a requisição 
                    if (!context.Response.HasStarted && perfisUserNoContexto.Count != perfisUserCadastrados.Count)
                    {
                        var err = new { Erro = "Houve divergência nas permissões do usuário. Faça login novamente para validar as permissões..." };
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = MediaTypeNames.Application.Json;
                        await context.Response.BodyWriter.WriteAsync(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(err)));

                        // necessário para injetar o erro dentro do contexto atual da Response invés do contexto da controller
                        context.Fail(err.Erro);

                    }
                    else
                    {

                        bool hasPerfil = false;
                        foreach (var perfilContextoResponse in perfisUserNoContexto)
                        {
                            hasPerfil = perfisUserCadastrados.Any(x => x.Value.Contains(perfilContextoResponse.Value));

                            // se houver qualquer perfil no contexto que não esteja de acordo com os perfis cadastrados
                            // assume que há uma divergência de perfis e encerra verificação ...
                            if (!hasPerfil) break;
                        }

                        // dispara erro se houver divergencia nos perfis
                        if (!hasPerfil && !context.Response.HasStarted)
                        {
                            var err = new
                            {
                                Erro = "As permissões do usuário não estão de acordo com as permissões da requisição." +
                                " Faça login novamente para validar as permissões..."
                            };
                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            context.Response.ContentType = MediaTypeNames.Application.Json;
                            await context.Response.BodyWriter.WriteAsync(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(err)));

                            // necessário para injetar o erro dentro do contexto atual da Response invés do contexto da controller
                            context.Fail(err.Erro);
                        }
                    }
                },

                OnForbidden = async context =>
                {

                    if (!context.Response.HasStarted)
                    {
                        var err = new { Erro = "Perfil não autorizado para acessar este recurso..." };

                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = MediaTypeNames.Application.Json;
                        await context.Response.BodyWriter.WriteAsync(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(err)));

                        // necessário para injetar o erro dentro do contexto atual da Response invés do contexto da controller
                        context.Fail(err.Erro);
                    }
                },
                OnAuthenticationFailed = async context =>
                {
                    // aqui pode ser validado o token ou qualquer outra coisa da requisição
                    // de momento esta apenas personalizando uma mensagem de retorno indicando falha
                    if (!context.Response.HasStarted)
                    {
                        var err = new { Erro = "Erro: Ocorreu uma falha na autenticação. Tente efetuar login novamente para acessar este recurso..." };
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = MediaTypeNames.Application.Json;
                        await context.Response.BodyWriter.WriteAsync(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(err)));

                        context.Fail(err.Erro);
                    }
                },
                OnChallenge = async context =>
                {

                    var hasToken = !string.IsNullOrEmpty(context.Request.Headers["Authorization"]);

                    if (!context.Response.HasStarted)
                    {
                        // Isso pode ser acionado se o cabeçalho de autorização estiver ausente ou malformado, trate-os de maneira diferente.
                        var err = hasToken ? $"Erro: Um token inválido foi incluído na solicitação." : $"Erro: Um token deve ser incluído na solicitação.";
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = MediaTypeNames.Application.Json;
                        await context.Response.BodyWriter.WriteAsync(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(err)));
                    }
                    context.HandleResponse();
                }
            };


        }

        private void ConfigurarSwaggerGenOptions(SwaggerGenOptions swaggerOptions)
        {

            swaggerOptions.SwaggerDoc("v1", new OpenApiInfo { Title = "A3_API_Project", Version = "v1" });

            // Config para mostrar os comentarios<sumary> contidos nos endpoints da camada CONTROLLER no SWAGGER
            // requer ajustes no arquivo .csproj
            // https://www.treinaweb.com.br/blog/documentando-uma-asp-net-core-web-api-com-o-swagger
            //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}_Swagger_Doc.xml";
            //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            //swaggerOptions.CustomOperationIds(opId =>
            //{
            //    return $"{ opId.ActionDescriptor.RouteValues["controller"]}_{opId.ActionDescriptor.RouteValues["action"]}";
            //});
            //swaggerOptions.IncludeXmlComments(xmlPath);

            swaggerOptions.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Cabeçalho JWT Authorization utilizando o padrão Bearer. Insira 'Bearer'[space] e o seu token de autênticação.\r\n\r\nExemplo: \"Bearer 12345abcdef\"",
            });

            swaggerOptions.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        //public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        //{
        //    if (env.IsDevelopment())
        //    {
        //        app.UseDeveloperExceptionPage();
        //    }
        //    else
        //    {
        //        app.UseExceptionHandler("/Home/Error");
        //        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        //        app.UseHsts();
        //    }
        //    app.UseHttpsRedirection();
        //    app.UseStaticFiles();

        //    app.UseRouting();

        //    app.UseAuthorization();

        //    app.UseEndpoints(endpoints =>
        //    {
        //        endpoints.MapControllerRoute(
        //            name: "default",
        //            pattern: "{controller=Home}/{action=Index}/{id?}");
        //    });
        //}
    }
}
