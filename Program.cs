using System.Text;
using HosterBackend.Data;
using HosterBackend.Data.Entities;
using HosterBackend.Interfaces;
using HosterBackend.Repositories;
using HosterBackend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<DataContext>(opt => {
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
    options => {
        var tokenKey = builder.Configuration["TokenKey"] ?? throw new Exception("Không tìm thấy khóa token trong file config");
        options.TokenValidationParameters = new TokenValidationParameters{
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    }
);
// builder.Services.AddHostedService<DomainExpiryNotifierService>();
builder.Services.AddControllers().AddNewtonsoftJson();
// builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<ITokenService,TokenService>();
builder.Services.AddScoped<IMailService,MailService>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IEmployeeRepository,EmployeeRepository>();
builder.Services.AddScoped<IAuthorizeRepository,AuthorizeRepository>();
builder.Services.AddScoped<IRoleRepository,RoleRepository>();
builder.Services.AddScoped<ICustomerRepository,CustomerRepository>();
builder.Services.AddScoped<ICustomerTypeRepository,CustomerTypeRepository>();
builder.Services.AddScoped<IDomainAccountRepository,DomainAccountRepository>();
builder.Services.AddScoped<IDomainProductRepository,DomainProductRepository>();
builder.Services.AddScoped<IOrderRepository,OrderRepository>();
builder.Services.AddScoped<IPaymentMethodRepository,PaymentMethodRepository>();
builder.Services.AddScoped<IDiscountRepository,DiscountRepository>();
builder.Services.AddScoped<IRegisteredDomainRepository, RegisteredDomainRepository>();
builder.Services.AddScoped<IPasswordResetTokenRepository,PasswordResetTokenRepository>();
builder.Services.AddScoped<INewRepository, NewRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseCors("AllowAll");
app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<DataContext>();
    await context.Database.MigrateAsync();
    await Seed.SeedData<Role>(context, "Data/RoleSeedData.json");
    await Seed.SeedEmployees(context);
    await Seed.SeedData<CustomerType>(context, "Data/CustomerTypeData.json");
    await Seed.SeedData<Customer>(context, "Data/CustomerData.json");
    await Seed.SeedData<DomainProduct>(context, "Data/DomainProductData.json");
    await Seed.SeedData<DomainAccount>(context, "Data/DomainAccountData.json");
    await Seed.SeedData<PaymentMethod>(context, "Data/PaymentMethodData.json");
    await Seed.SeedData<Discount>(context, "Data/DiscountData.json");
    await Seed.SeedData<Category>(context, "Data/CategoryData.json");
    await Seed.SeedData<New>(context,"Data/NewData.json");
    
    

}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Có lỗi trong quá trình seed dữ liệu");
}


app.Run();

