using AccProject.Models;
using HomeBankingAcc.Models;
using HomeBankingAcc.Repositories;
using HomeBankingAcc.Repositories.Implementations;
using HomeBankingAcc.Services;
using HomeBankingAcc.Services.Implementations;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add context to the container
builder.Services.AddDbContext<HomeBankingContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("MyDbConnection"))
    );

//Add Authentication to the container
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromMinutes(10 );
    options.LoginPath = new PathString("/index.html"); //ruta a la que se redirige al usuario cuando falla la sesión
});

//Add authorization to the container
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ClientOnly", policy => policy.RequireClaim("Client"));
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim("Admin"));
});


// Add repositories to the container
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ILoanRepository, LoanRepository>();
builder.Services.AddScoped<ICardRepository, CardRepository>();
builder.Services.AddScoped<IClientLoanRepository, ClientLoanRepository>();

//Add services
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<ICardService, CardService>();
builder.Services.AddScoped<IAccountService, AccountService>();

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDefaultFiles(); //para que abra directamente el login
app.UseStaticFiles();

app.UseRouting();

app.MapControllers();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
