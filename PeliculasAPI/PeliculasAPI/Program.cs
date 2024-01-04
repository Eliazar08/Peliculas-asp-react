using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NetTopologySuite;
using PeliculasAPI;
using PeliculasAPI.ApiBehavior;
using PeliculasAPI.Filtros;
using PeliculasAPI.Utilidades;
using AutoMapper;

;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddSingleton(provider =>
   new MapperConfiguration(config =>
   {
       var geometryFactory = provider.GetRequiredService<GeometryFactory>();
       config.AddProfile(new AutoMapperProfiles(geometryFactory));
   }).CreateMapper());

builder.Services.AddTransient<IAlmacenadorArchivos,AlmacenadorArchivosLocal>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(FiltroDeExcepcion));
    options.Filters.Add(typeof (ParsearBadRequests));
}).ConfigureApiBehaviorOptions(BehaviorBadRequests.Parsear);

builder.Services.AddDbContext<ApplicationDbContext>(option =>
    option.UseSqlServer(builder.Configuration.GetConnectionString("defaultConnection"),
    sqlServer=> sqlServer.UseNetTopologySuite()));

builder.Services.AddSingleton<GeometryFactory>(NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326));
builder.Services.AddCors(option =>
{
    var frontendURL = builder.Configuration.GetValue<string>("frontend_url");
    option.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins(frontendURL).AllowAnyMethod().AllowAnyHeader()
        .WithExposedHeaders(new string[] { "cantidadTotalRegistros" } );
    });
});

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
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();

app.UseCors();

app.UseAuthorization();

app.MapControllers();


app.Run();
