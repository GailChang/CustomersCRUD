// 使用環境中的 WebApplication 類別中 static Method CreateBuilder() 準備應用系統產生器
using Microsoft.EntityFrameworkCore;
using MyService.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// 需要取出應用系統組態配置(連接字串)
ConfigurationManager manager = builder.Configuration;
String connectionString = manager.GetConnectionString("nor");
//加入自訂的 DbContext Service
builder.Services.AddDbContext<MyService.Models.NorthwindDB>(
    // Lambda
    (builder) =>
    {
        builder.UseSqlServer(connectionString);     //配置連接字串
    }
    );

//取出允許前端 Domain 屬性
String[] corsURL = manager.GetSection("CORS").Get<String[]>();
//自訂CORS政策服務，允許前端服務可以過來
builder.Services.AddCors(
   (options) =>
   {
       options.AddPolicy("myweb",
           (builder) =>
           {
               builder.AllowAnyHeader();
               builder.AllowAnyMethod();
               builder.AllowAnyOrigin();
           });
   });

builder.Services.AddEndpointsApiExplorer();
// 加入 Swagger 服務
builder.Services.AddSwaggerGen();
var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // 使用 Middleware
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
// 使用 Cors Policy Middleware
app.UseCors("myweb");
app.MapControllers();

app.Run(); //BOOT
