using Dna.AspNet;

using ResourceGroupTenants.Relational.Emails.FluentServices;
using ResourceGroupTenants.Relational.Extensions;

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
builder.Services.AddHttpContextAccessor();
// Add Tenant extension service
builder.Services.AddResourceMigrateTenantDatabases(builder.Configuration);
builder.Services.AddControllers().AddApplicationPart(Assembly.Load(new AssemblyName("ResourceGroupTenants.Relational")));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();  

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Use Dna Framework
app.UseDnaFramework();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
