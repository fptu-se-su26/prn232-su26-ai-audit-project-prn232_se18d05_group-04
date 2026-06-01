using API.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddVivuCarOData();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services
    .AddVivuCarDataAccess(builder.Configuration)
    .AddVivuCarRepositories()
    .AddVivuCarServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
