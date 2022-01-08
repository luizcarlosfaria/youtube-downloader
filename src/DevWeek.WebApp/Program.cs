using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped((sp) =>
{
    return new RabbitMQ.Client.ConnectionFactory()
    {
        Uri = new Uri(builder.Configuration.GetSection("DevWeek:RabbitMQ:ConnectionString").Get<string>())
    };
});

builder.Services.AddScoped((sp) =>
{
    return sp.GetRequiredService<ConnectionFactory>().CreateConnection();
});

builder.Services.AddTransient((sp) =>
{
    return sp.GetRequiredService<IConnection>().CreateModel();
});

builder.Services.AddSingleton((sp) =>
{
    return builder.Configuration;
});

builder.Services.AddSingleton((sp) =>
{
    return StackExchange.Redis.ConnectionMultiplexer.Connect(builder.Configuration.GetSection("DevWeek:Redis:ConnectionString").Get<string>(), null);
});



builder.Services.AddSingleton((sp) =>
{
    return new Minio.MinioClient(
        builder.Configuration.GetSection("DevWeek:S3:Endpoint").Get<string>(),
        builder.Configuration.GetSection("DevWeek:S3:AccessKey").Get<string>(),
        builder.Configuration.GetSection("DevWeek:S3:SecretKey").Get<string>());
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
