using camunda.helper.Camunda.Worker.Handlers;
using camunda.helper.Camunda.Worker.Handlers.Saga.Orchestrator;
using Camunda.Worker;
using Camunda.Worker.Client;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
             options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();

//Camunda worker startUp
builder.Services.AddExternalTaskClient()
    .ConfigureHttpClient((provider, client) =>
    {
        client.BaseAddress = new Uri("http://127.0.0.1:6060/engine-rest");
    });

builder.Services.AddCamundaWorker("PrepareTeaCamundaWorker", 1)
    .AddHandler<BoilWaterHandler>()
    .AddHandler<AddTeaLeavesHandler>()
    .AddHandler<StrainAndServeHandler>()
    .AddHandler<CheckItemsAvailabilityHandler>()
    .AddHandler<InformInventoryHandler>()
    //saga orchestrator handlers
    .AddHandler<CreateOrderHandler>()
    .AddHandler<ProcessPaymentHandler>()
    .AddHandler<UpdateInventoryHandler>()
    .AddHandler<DeliverOrderHandler>()
    .AddHandler<CancelOrderHandler>()
    .AddHandler<ReversePaymentHandler>()
    .AddHandler<ReverseInventoryHandler>()
    .AddHandler<CancelDeliveryHandler>();
//

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
