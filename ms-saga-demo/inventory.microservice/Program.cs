using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//InMemory Db Context
builder.Services.AddDbContext<InventoryDb>(opt => opt.UseInMemoryDatabase("Inventory"));

//

var app = builder.Build();

// using (var scope = app.Services.CreateScope())
// {
//     var inventoryDbContext = scope.ServiceProvider.GetRequiredService<InventoryDb>();
//     inventoryDbContext.Inventory.Add(new Inventory { Id=1, Products = new List<Product>
//     {
//         new Product { InventoryId = 1, Id = 121, Name = "Prod1", UnitPrice = 50, UnitsAvaialable = 1000},
//         new Product { InventoryId = 1, Id = 122, Name = "Prod2", UnitPrice = 500, UnitsAvaialable = 1000},
//         new Product { InventoryId = 1, Id = 123, Name = "Prod3", UnitPrice = 100, UnitsAvaialable = 1000},
//         new Product { InventoryId = 1, Id = 124, Name = "Prod4", UnitPrice = 200, UnitsAvaialable = 1000},
//         new Product { InventoryId = 1, Id = 125, Name = "Prod5", UnitPrice = 250, UnitsAvaialable = 1000}
//     } });
//     // inventoryDbContext.Products.Add(new Product { InventoryId = 1, Id = 121, Name = "Prod1", UnitPrice = 50, UnitsAvaialable = 1000});
//     // inventoryDbContext.Products.Add(new Product { InventoryId = 1, Id = 122, Name = "Prod2", UnitPrice = 500, UnitsAvaialable = 1000});
//     // inventoryDbContext.Products.Add(new Product { InventoryId = 1, Id = 123, Name = "Prod3", UnitPrice = 100, UnitsAvaialable = 1000});
//     // inventoryDbContext.Products.Add(new Product { InventoryId = 1, Id = 124, Name = "Prod4", UnitPrice = 200, UnitsAvaialable = 1000});
//     // inventoryDbContext.Products.Add(new Product { InventoryId = 1, Id = 125, Name = "Prod5", UnitPrice = 250, UnitsAvaialable = 1000});
//     await inventoryDbContext.SaveChangesAsync();
// }

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

#region rest endpoint implementations
app.MapGet("/Inventory", async (InventoryDb db) =>
await db.Inventory.Include(i => i.Products).ToListAsync()
)
.WithName("GetInventories");

app.MapGet("/Inventory/{id}", async (int id, InventoryDb db) =>
    await db.Inventory.Include(i => i.Products).FirstOrDefaultAsync(p => p.Id == id)
        is Inventory Inventory
            ? Results.Ok(Inventory)
            : Results.NotFound()
)
.WithName("GetInventory");

app.MapGet("/Inventory/ProductInfo/{productId}", async (int productId, InventoryDb db) =>    
{
    var productDetails = new List<Inventory>();
    var inventoryList = await db.Inventory.Include(i => i.Products).ToListAsync();
    foreach (var inventory in inventoryList) 
    {
        var productInfo = inventory.Products.FirstOrDefault(p => p.Id == productId);
        if (productInfo != null)
        {
            productDetails.Add(new Inventory { Id = inventory.Id, Products = new List<Product> { productInfo } });
        }
    }

    return Results.Ok(productDetails);
}
)
.WithName("GetProductInfo");

app.MapPost("/Inventory", async (Inventory inventory, InventoryDb db) =>
{
    db.Inventory.Add(inventory);
    await db.SaveChangesAsync();

    return Results.Created($"/Inventory/{inventory.Id}", inventory);
})
.WithName("CreateInventory");

app.MapPut("/Inventory/{id}", async (int id, InventoryPutDTO input, InventoryDb db) =>
{

    var inventory = await db.Inventory.Include(i => i.Products).FirstOrDefaultAsync(p => p.Id == id);

    if (inventory is null)
    {
        return Results.NotFound();
    }
    
    if (input.Action.ToLower().Equals("removeproduct"))
    {
        input.Products.ForEach(p => {
            Product prod = inventory.Products.FirstOrDefault(ip => ip.Id == p.ProductId);
            if (prod != null)
            {
                prod.UnitsAvaialable -= p.Units;
            }
        });        
    }

    if (input.Action.ToLower().Equals("addproduct"))
    {
        input.Products.ForEach(p => {
            Product prod = inventory.Products.FirstOrDefault(ip => ip.Id == p.ProductId);
            if (prod != null)
            {
                prod.UnitsAvaialable += p.Units;
            }
        });
    }
    await db.SaveChangesAsync();

    return Results.NoContent();
});
#endregion
app.Run();
app.Run("https://localhost:3003");


#region Entities
class Inventory
{
    public int Id { get; set; }
    public List<Product> Products { get; set; }
}

class Product
{
    // public Inventory Inventory { get; set; }
    // public int InventoryId { get; set; }
    
    public int Id { get; set; }
    public string Name {get; set;}
    public decimal UnitPrice { get; set; }
    public int UnitsAvaialable { get; set; }
}


class InventoryPutDTO
{
    public Guid OrderId { get; set; }
    public List<ProductDTO>? Products { get; set;}
    public string Action { get; set; } // AddProduct & RemoveProduct
}

class ProductDTO 
{
    public int ProductId { get; set; }
    public int Units { get; set; }
}


class InventoryDb : DbContext
{
    public InventoryDb(DbContextOptions<InventoryDb> options) : base(options)
    { }

    public DbSet<Inventory> Inventory => Set<Inventory>();
    public DbSet<Product> Products => Set<Product>();

    // protected override void OnModelCreating(ModelBuilder modelBuilder)
    // {
    //     //modelBuilder.Entity<Inventory>().HasMany(i => i.Products).WithOne();
    //     modelBuilder.Entity<Product>().HasOne(p => p.Inventory).WithMany( i => i.Products).HasForeignKey(p => p.InventoryId);

    //     modelBuilder.Entity<Inventory>()
    //     .HasData(new Inventory { Id=1 });

    //     modelBuilder.Entity<Product>()
    //         .HasData(
    //             new Product { InventoryId = 1, Id = 121, Name = "Prod1", UnitPrice = 50, UnitsAvaialable = 1000},
    //             new Product { InventoryId = 1, Id = 122, Name = "Prod2", UnitPrice = 500, UnitsAvaialable = 1000},
    //             new Product { InventoryId = 1, Id = 123, Name = "Prod3", UnitPrice = 100, UnitsAvaialable = 1000},
    //             new Product { InventoryId = 1, Id = 124, Name = "Prod4", UnitPrice = 200, UnitsAvaialable = 1000},
    //             new Product { InventoryId = 1, Id = 125, Name = "Prod5", UnitPrice = 250, UnitsAvaialable = 1000});

    // }
}
#endregion