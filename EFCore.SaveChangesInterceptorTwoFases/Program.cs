
using EFCore.SaveChangesInterceptorTwoFases;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("Hello, World!");

var services = new ServiceCollection();

services.AddScoped<TransactionManager>();

services.AddSingleton(sp =>
{
    var con = new SqliteConnection("DataSource=:memory:");
    con.Open();
    return con;
});

services.AddDbContextPool<MyDbContext>((sp, optionsBuilder) =>
{
    optionsBuilder.UseSqlite(sp.GetRequiredService<SqliteConnection>());
    optionsBuilder.AddInterceptors(new MyInterceptor());
});

var sp = services.BuildServiceProvider();

// Init database
var scope = sp.CreateScope();

var db = scope.ServiceProvider.GetRequiredService<MyDbContext>();
await db.Database.EnsureCreatedAsync();
db.Database.Migrate();

scope.Dispose();

// insert Blog

scope = sp.CreateScope();
db = scope.ServiceProvider.GetRequiredService<MyDbContext>();

db.Add(new Blog()
{
    Id = 1,
    Name= "Test",
    Description= "Test",
    Posts = new List<Post>()
});

db.SaveChanges();

scope.Dispose();

sp.Dispose();