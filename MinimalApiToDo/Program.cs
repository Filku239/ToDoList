using MinimalApiToDo.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

var listToDo = new List<TodoItem>();

app.MapPost("/todo", (TodoItem todoItem) =>
{
    listToDo.Add(todoItem);
    return Results.Created($"/todo/{todoItem.Id}", todoItem);
});

app.MapGet("/todo", () => listToDo);

app.MapGet("/todo/{id}", (int id) =>
{
    var todoItem = listToDo.FirstOrDefault(x => x.Id == id);
    return todoItem is null ? Results.NotFound() : Results.Ok(todoItem);
});

app.MapDelete("todo/{id}", (int id) =>
{
    var todoItem = listToDo.FirstOrDefault(x => x.Id == id);
    if (todoItem is null)
    {
        return Results.NotFound();
    }

    listToDo.Remove(todoItem);
    return Results.NoContent();
});

app.MapPut("/todo/{id}", (int id) =>
{
    var todoItem = listToDo.FirstOrDefault(x => x.Id == id);
    if (todoItem is null)
    {
        return Results.NotFound();
    }

    todoItem.IsComplete = !todoItem.IsComplete;
    return Results.Ok(todoItem);
});

app.UseCors("AllowAll");

app.Run();