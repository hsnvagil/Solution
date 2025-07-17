using App.API.Endpoints;
using App.API.Middlewares;
using App.Application;
using App.Infrastructure;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddInfrastructureServices();
builder.Services.AddApplicationServices();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<RequestResponseLoggerMiddleware>();
app.UseHttpsRedirection();

HomeEndpoints.AddRoutes(app);

app.Run();

// for test
public partial class Program { }
