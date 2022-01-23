using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//InMemory Db Context
builder.Services.AddDbContext<DeliveryDb>(opt => opt.UseInMemoryDatabase("Deliveries"));
//

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


#region rest endpoint implementations
app.MapGet("/Delivery", async (DeliveryDb db) =>
await db.Deliveries.ToListAsync()
)
.WithName("GetDeliveries");

app.MapGet("/Delivery/{id}", async (Guid orderId, DeliveryDb db) =>
    await db.Deliveries.FirstOrDefaultAsync(p => p.OrderId == orderId)
        is Delivery Delivery
            ? Results.Ok(Delivery)
            : Results.NotFound()
)
.WithName("GetDelivery");

app.MapPost("/Delivery", async (DeliveryDTO deliveryDTO, DeliveryDb db) =>
{
    Delivery delivery = new Delivery
    {
        Id = Guid.NewGuid(),
        OrderId = deliveryDTO.OrderId,
        DeliveryStatus = "Initiated"
    };
    db.Deliveries.Add(delivery);
    await db.SaveChangesAsync();

    return Results.Created($"/Delivery/{delivery.OrderId}", delivery);
})
.WithName("CreateDelivery");

app.MapDelete("/Delivery/{orderId}", async (Guid orderId, DeliveryDb db) =>
{
    var delivery = await db.Deliveries.FirstOrDefaultAsync(p => p.OrderId == orderId);
    if(delivery is null)
    {
        return Results.NotFound();
    }

    delivery.DeliveryStatus = "Cancelled";
    await db.SaveChangesAsync();

    return Results.NoContent();
})
.WithName("CancelDelivery");
#endregion
app.Run();
app.Run("https://localhost:3004");



#region Entities
class Delivery
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public string DeliveryStatus { get; set; }
}

class DeliveryDTO
{
    public Guid OrderId { get; set; }
}


class DeliveryDb : DbContext
{
    public DeliveryDb(DbContextOptions<DeliveryDb> options) : base(options)
    { }

    public DbSet<Delivery> Deliveries => Set<Delivery>();
}
#endregion