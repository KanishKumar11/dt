var builder = WebApplication.CreateBuilder(args);

// If the platform provides HTTP_PORTS/HTTPS_PORTS (e.g., Coolify), let ASP.NET Core use them by unsetting ASPNETCORE_URLS
var httpPorts = Environment.GetEnvironmentVariable("HTTP_PORTS");
var httpsPorts = Environment.GetEnvironmentVariable("HTTPS_PORTS");
if (!string.IsNullOrEmpty(httpPorts) || !string.IsNullOrEmpty(httpsPorts))
{
    // Unset ASPNETCORE_URLS to allow HTTP_PORTS to take effect without conflicts
    Environment.SetEnvironmentVariable("ASPNETCORE_URLS", null);
}
else
{
    // Fallback to default port 80 if no platform ports provided
    Environment.SetEnvironmentVariable("ASPNETCORE_URLS", "http://+:80");
}

// Add services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Log effective environment and bindings for debugging
app.Logger.LogInformation("HTTP_PORTS='{httpPorts}' HTTPS_PORTS='{httpsPorts}' ASPNETCORE_URLS='{asp}'", Environment.GetEnvironmentVariable("HTTP_PORTS"), Environment.GetEnvironmentVariable("HTTPS_PORTS"), Environment.GetEnvironmentVariable("ASPNETCORE_URLS"));
app.Logger.LogInformation("Hosting environment: {env}", app.Environment.EnvironmentName);
app.Logger.LogInformation("Now listening on: {urls}", string.Join(',', app.Urls));

// Enable Swagger in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// In-memory sample data
var todos = new List<Todo>
{
    new Todo { Id = 1, Title = "Buy milk", IsComplete = false }
};

app.MapGet("/", () => Results.Ok("Hello World!"));

app.MapGet("/health", () => Results.Ok("OK"));

app.MapGet("/todos", () => Results.Ok(todos));

app.MapGet("/todos/{id}", (int id) =>
    todos.FirstOrDefault(t => t.Id == id) is not Todo t ? Results.NotFound() : Results.Ok(t));

app.MapPost("/todos", (Todo todo) =>
{
    todo.Id = todos.Any() ? todos.Max(t => t.Id) + 1 : 1;
    todos.Add(todo);
    return Results.Created($"/todos/{todo.Id}", todo);
});

app.MapPut("/todos/{id}", (int id, Todo input) =>
{
    var todo = todos.FirstOrDefault(t => t.Id == id);
    if (todo is null) return Results.NotFound();
    todo.Title = input.Title;
    todo.IsComplete = input.IsComplete;
    return Results.NoContent();
});

app.MapDelete("/todos/{id}", (int id) =>
{
    var todo = todos.FirstOrDefault(t => t.Id == id);
    if (todo is null) return Results.NotFound();
    todos.Remove(todo);
    return Results.NoContent();
});

app.Run();
