using gamershop.Server.Database;
using gamershop.Server.Repositories;
using gamershop.Server.Services;
using gamershop.Server.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using gamershop.Shared.Models;
using gamershop.Server.Repositories.Interfaces;
using AutoMapper;
using gamershop.Server.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add message queue
builder.Services.AddSingleton<SimpleMessageQueue<(Order, string, double)>>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Add IConfiguration
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

// Add DB factory
builder.Services.AddSingleton<DbConnectionFactory>();

// Add repositories
builder.Services.AddSingleton<OrderRepository>();
builder.Services.AddSingleton<PaymentRepository>();
builder.Services.AddSingleton<TransactionRepository>();
builder.Services.AddHostedService<TransactionService>();
builder.Services.AddSingleton<OrderService>();
builder.Services.AddSingleton<IPaymentService, PaymentService>();
// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Add services
builder.Services.AddSingleton<MappingProfile>();
builder.Services.AddSingleton<IProductService, ProductService>();

// Register the product interface service
builder.Services.AddSingleton<IProductRepository, ProductRepository>();

// Add Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });
});

// Configure the number of instances of OrderController
const int numberOfOrderControllerInstances = 2;

// Register OrderControllerFactory
builder.Services.AddSingleton<OrderControllerFactory>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    return new OrderControllerFactory(numberOfOrderControllerInstances, configuration);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1"));
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();

// Use OrderController instances from the OrderControllerFactory
var orderControllerFactory = app.Services.GetRequiredService<OrderControllerFactory>();
for (int i = 0; i < numberOfOrderControllerInstances; i++)
{
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers().WithMetadata(orderControllerFactory.GetNextInstance());
    });
}

app.MapFallbackToFile("index.html");

app.Run();
