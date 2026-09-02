using inmobiliaria.Models;

var builder = WebApplication.CreateBuilder(args);

// registrar la database y el repositorio de propietario
builder.Services.AddSingleton<Database>();
builder.Services.AddScoped<PropietarioRepository>();
builder.Services.AddScoped<InquilinoRepository>();
builder.Services.AddScoped<InmuebleRepository>();
builder.Services.AddScoped<TipoInmuebleRepository>();
builder.Services.AddScoped<ReservaRepository>();

// Add services to the container.
builder.Services.AddControllersWithViews();

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

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
