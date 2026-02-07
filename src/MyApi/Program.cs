var builder = WebApplication.CreateBuilder(args);

// If the platform provides HTTP_PORTS/HTTPS_PORTS (e.g., Coolify), prefer those and apply to Kestrel
var httpPorts = Environment.GetEnvironmentVariable("HTTP_PORTS");
var httpsPorts = Environment.GetEnvironmentVariable("HTTPS_PORTS");
if (!string.IsNullOrEmpty(httpPorts) || !string.IsNullOrEmpty(httpsPorts))
{
    var urlList = new List<string>();
    if (!string.IsNullOrEmpty(httpPorts))
    {
        // Use the first HTTP port if CSV provided
        var port = httpPorts.Split(',')[0].Trim();
        urlList.Add($"http://+:{port}");
    }
    if (!string.IsNullOrEmpty(httpsPorts))
    {
        var port = httpsPorts.Split(',')[0].Trim();
        urlList.Add($"https://+:{port}");
    }

    if (urlList.Any())
    {
        builder.WebHost.UseUrls(string.Join(';', urlList));
    }
}

// Add services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Log effective environment and bindings for debugging
app.Logger.LogInformation("HTTP_PORTS='{httpPorts}' HTTPS_PORTS='{httpsPorts}' ASPNETCORE_URLS='{asp}'", httpPorts, httpsPorts, Environment.GetEnvironmentVariable("ASPNETCORE_URLS"));
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
