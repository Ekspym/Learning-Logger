using IPodnik.Infrastructure.Core.Logging;
using IPodnik.Server.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.InstallAgentServer(builder.Configuration);

var app = builder.Build();
// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.ConfigureMachineInfo();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
