using MyWeb.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
//先取出連接字串
String connectionString = builder.Configuration.GetConnectionString("nor");
//建構自己設計資料屬性物件
DbConfig config = new DbConfig()
{
    connectionString = connectionString,
    userName = "",
    password = ""
};  
//將自訂的物件 變成 Singleton 服務(獨一無二)
builder.Services.AddSingleton(config.GetType(), config);

//封裝 CustomersService 組態配置
IConfigurationSection section = builder.Configuration.GetSection("CustomersService");
//建構自訂物件模組
CustomersService customersServices = new CustomersService();
section.Bind(customersServices);
builder.Services.AddSingleton(typeof(CustomersService), customersServices);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
