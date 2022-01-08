using camunda.helper.Camunda.Worker.Handlers;
using Camunda.Worker;
using Camunda.Worker.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Camunda worker startUp
builder.Services.AddExternalTaskClient()
    .ConfigureHttpClient((provider, client) =>
    {
        client.BaseAddress = new Uri("http://127.0.0.1:6060/engine-rest");
    });

builder.Services.AddCamundaWorker("PrepareTeaCamundaWorker", 1)
    .AddHandler<BoilWaterHandler>()
    .AddHandler<AddTeaLeavesHandler>()
    .AddHandler<StrainAndServeHandler>();
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
