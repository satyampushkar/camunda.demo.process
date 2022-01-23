using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//InMemory Db Context
builder.Services.AddDbContext<OrderDb>(opt => opt.UseInMemoryDatabase("Orders"));
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
app.MapGet("/order", async (OrderDb db) =>
await db.Orders.Include(o => o.OrderItems).ToListAsync()
)
.WithName("GetOrders");

app.MapGet("/order/{id}", async (Guid id, OrderDb db) =>
    await db.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.Id == id)
        is Order order
            ? Results.Ok(order)
            : Results.NotFound()
)
.WithName("GetOrder");

app.MapPost("/order", async (List<OrderDTO> orderItems, OrderDb db) =>
{
    var orderId = Guid.NewGuid();
    double orderAmount = 0;

    foreach (var orderItem in orderItems.ToList())
    {
        orderAmount +=  (double)(orderItem.Units * orderItem.UnitPrice);
    }

    Order order = new Order
    {
        Id = orderId,
        OrderDate = DateTime.Now,
        OrderStatus = "Created",
        OrderAmount = orderAmount,
        OrderItems = orderItems.ToList().Select(x => new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                ProductId = x.ProductId,
                UnitPrice = x.UnitPrice,
                Units = x.Units
            }).ToList()

    };
    db.Orders.Add(order);
    await db.SaveChangesAsync();

    //return Results.Created($"/order/{order.Id}", order);
    return Results.Ok(order.Id);
})
.WithName("CreateOrder");

app.MapDelete("/order/{id}", async (Guid id, OrderDb db) =>
{
    var order = await db.Orders.FindAsync(id);
    if(order is null)
    {
        return Results.NotFound();
    }

    order.OrderStatus = "Cancelled";
    await db.SaveChangesAsync();

    return Results.NoContent();
})
.WithName("CancelOrder");
#endregion
app.Run();
app.Run("https://localhost:3001");



#region Entities
class Order
{
    public Guid Id { get; set; }
    public DateTime OrderDate { get; set; }
    public string OrderStatus { get; set; }
    public double OrderAmount {get; set;}


    public List<OrderItem> OrderItems { get; set; }
}

class OrderItem
{
    public Guid Id { get; set; }    
    public int ProductId { get; set; }
    public decimal UnitPrice { get; set; }
    public int Units { get; set; }


    public Guid? OrderId { get; set; }
    //public Order Order { get; set; }
}

class OrderDTO
{
    public int ProductId { get; set; }
    public decimal UnitPrice { get; set; }
    public int Units { get; set; }
}

class OrderDb : DbContext
{
    public OrderDb(DbContextOptions<OrderDb> options) : base(options)
    { }

    public DbSet<Order> Orders => Set<Order>();
    //public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{
    //    modelBuilder.Entity<Order>()
    //        .HasMany(o => o.OrderItems)
    //        .WithOne();
    //}
} 
#endregion