using IruddBlog.Commands;
using IruddBlog.Infra.Auth;
using IruddBlog.Infra.PostBackup;
using IruddBlog.Infra.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IruddBlog
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
            services.AddSingleton<Infra.IClock, Infra.SystemClock>();
            services.AddSingleton<IGoogleAccountService, GoogleAccountService>();
            services.AddSingleton<IBlogSettings, BlogSettings>();
            services.AddSingleton<IPostBackupServiceFactory, PostBackupServiceFactory>();
            services.AddSingleton<IFileSystem, FileSystem>();
            services.AddSingleton<IRandomIdGenerator, RandomIdGenerator>();
            services.AddSingleton<IGetBlogPostsCommand, GetBlogPostsCommand>();
            services.AddSingleton<INewBlogPostCommand, NewBlogPostCommand>();
            services.AddSingleton<ITemporaryImageHostCommand, TemporaryImageHostCommand>();

            services.AddMvc(option => option.EnableEndpointRouting = false);//.AddNewtonsoftJson();

            services.AddAuthentication(BearerTokenWithGoogleDefaults.AuthenticationScheme)
                .AddBearerTokenWithGoogleId(o =>
                {

                });

            services.AddAuthorization(options =>
                {
                    options.AddPolicy(
                        "MustBeBlogOwner",
                        policyBuilder => policyBuilder.RequireClaim("iruddblog.isowner", "true"));
                });

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IPostBackupServiceFactory postBackupServiceFactory)
        {
            var backupService = postBackupServiceFactory.CreateService();
            backupService.DownloadPostsFromBackupIfNoLocalPostsExist();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseAuthentication();

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}
