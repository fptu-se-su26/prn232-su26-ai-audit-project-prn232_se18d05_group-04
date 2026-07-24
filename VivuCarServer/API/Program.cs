using API.Configurations;

EnvironmentConfiguration.LoadEnvFile();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddVivuCarOData();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder
    .Services.AddVivuCarDatabase(builder.Configuration)
    .AddVivuCarAuthentication()
    .AddVivuCarRepositories()
    .AddVivuCarServices()
    .AddVivuCarRateLimiting()
    .AddVivuCarJwtAuthentication(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseRateLimiter();
app.UseAuthorization();

app.MapControllers();

app.Run();
