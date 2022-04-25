using Dna;
using Dna.AspNet;

using Microsoft.AspNetCore.ResponseCompression;

using ResourceGroupTenants.Relational.Emails.FluentServices;
using ResourceGroupTenants.Relational.Extensions;

using System.IO.Compression;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseDnaFramework(construct =>
{
    // To add system log file log file on release mode
#if RELEASE
                                  //construct.AddFileLogger();
#endif

});
// Add services to the container. 
// Add Cors service
builder.Services.AddCors();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddResponseCompression(options =>
{
    //  options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/json" });
})
           //Set the corresponding compression level for different compression types
           .Configure<GzipCompressionProviderOptions>(options =>
           {
               //Using the Optimal way to compress is necessarily the best way to compress
               options.Level = CompressionLevel.Optimal;
           });
// Add Tenant extension service
builder.Services.AddResourceMigrateTenantDatabases(builder.Configuration);

// Add controllers
builder.Services.AddControllers().AddApplicationPart(Assembly.Load(new AssemblyName("ResourceGroupTenants.Relational")));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// add Swagger gen
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseDnaFramework();
app.UseResponseCompression();
FrameworkDI.Logger.LogCriticalSource("We are live!!!!");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(options =>
                     // For specific origins
                     //options.WithOrigins("http://localhost:4200")
                     options
                     .AllowAnyOrigin()
                     .AllowAnyMethod()
                     .AllowAnyHeader());
// Use Dna Framework
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();
