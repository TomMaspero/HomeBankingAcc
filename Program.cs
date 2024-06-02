using AccProject.Models;
using HomeBankingAcc.Models;
using HomeBankingAcc.Repositories;
using HomeBankingAcc.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Add context to the container
builder.Services.AddDbContext<HomeBankingContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("MyDbConnection"))
    );

// Add repositories to the container
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ILoanRepository, LoanRepository>();
builder.Services.AddScoped<ICardRepository, CardRepository>();

var app = builder.Build();

//Create a scope to get the context and initialize the database
using (var scope = app.Services.CreateScope())
{
    try
    {
        var service = scope.ServiceProvider;
        var context = service.GetRequiredService<HomeBankingContext>();
        DbInitializer.Initialize(context);

    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the DB");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.MapControllers();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
