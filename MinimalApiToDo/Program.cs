using MinimalApiToDo.Models;
using Microsoft.EntityFrameworkCore;
using MinimalApiToDo.Data;
using DotNetEnv;

Env.Load();
var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

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

builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseSqlServer(connectionString));



var app = builder.Build();

app.MapPost("/todo", async (TodoItem todoItem, TodoDbContext db) =>
{
    db.TodoItems.Add(todoItem);
    await db.SaveChangesAsync();
    return Results.Created($"/todo/{todoItem.Id}", todoItem);
});

app.MapGet("/todo", async (TodoDbContext db) =>
{
    var todoItems = await db.TodoItems.ToListAsync();
    return todoItems;
});

app.MapGet("/todo/{id}", (int id,TodoDbContext db) =>
{
    var todoItem = db.TodoItems.FirstOrDefault(x => x.Id == id);    
    return todoItem;
});

app.MapDelete("todo/{id}", (int id,TodoDbContext db) =>
{
    var todoItem = db.TodoItems.FirstOrDefault(x => x.Id == id);
    db.TodoItems.Remove(todoItem);
    db.SaveChanges();
    return Results.Ok();
});

app.MapPut("/todo/{id}", (int id,TodoDbContext db) =>
{
    var todoItem = db.TodoItems.FirstOrDefault(x => x.Id == id);
    todoItem.IsComplete = !todoItem.IsComplete;
    db.SaveChanges();
    return Results.Ok();
});

app.UseCors("AllowAll");


app.Run();