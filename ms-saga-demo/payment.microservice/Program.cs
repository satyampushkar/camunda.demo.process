using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//InMemory Db Context
builder.Services.AddDbContext<PaymentDb>(opt => opt.UseInMemoryDatabase("Payments"));
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
app.MapGet("/Payment", async (PaymentDb db) =>
await db.Payments.ToListAsync()
)
.WithName("GetPayments");

app.MapGet("/Payment/{id}", async (Guid orderId, PaymentDb db) =>
    await db.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId)
        is Payment Payment
            ? Results.Ok(Payment)
            : Results.NotFound()
)
.WithName("GetPayment");

app.MapPost("/Payment", async (PaymentDTO paymentDetail, PaymentDb db) =>
{
    Payment payment = new Payment
    {
        Id = Guid.NewGuid(),
        OrderId = paymentDetail.OrderId,
        PaymentAmount = paymentDetail.PaymentAmount,
        PaymentStatus = "Processed"
    };
    db.Payments.Add(payment);
    await db.SaveChangesAsync();

    return Results.Created($"/Payment/{payment.Id}", payment);
})
.WithName("CreatePayment");

app.MapDelete("/Payment/{orderId}", async (Guid orderId, PaymentDb db) =>
{
    var payment = await db.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId);
    if(payment is null)
    {
        return Results.NotFound();
    }

    payment.PaymentStatus = "Reversed";
    await db.SaveChangesAsync();

    return Results.NoContent();
})
.WithName("CancelPayment");
#endregion
app.Run();
app.Run("https://localhost:3002");



#region Entities
class Payment
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public double PaymentAmount { get; set; }
    public string PaymentStatus { get; set; }
}

class PaymentDTO
{
    public Guid OrderId { get; set; }
    public double PaymentAmount { get; set; }
}


class PaymentDb : DbContext
{
    public PaymentDb(DbContextOptions<PaymentDb> options) : base(options)
    { }

    public DbSet<Payment> Payments => Set<Payment>();
}
#endregion