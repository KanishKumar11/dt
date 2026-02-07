var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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
