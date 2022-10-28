using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace TodoList.FunctionApp
{
    public class TodoApi
    {
        static List<Todo> items = new();
        private readonly ILogger<TodoApi> _log;

        public TodoApi(ILogger<TodoApi> log)
        {
            _log = log;
        }

        [Function("CreateTodo")]
        public async Task<IActionResult> CreateTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todo")] HttpRequestData req)
        {
            _log.LogInformation("Creating a new todo list item");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var input = JsonConvert.DeserializeObject<TodoCreateModel>(requestBody);

            var todo = new Todo { TaskDescription = input.TaskDescription };

            items.Add(todo);

            return new OkObjectResult(todo);
        }

        [Function("GetTodos")]
        public IActionResult GetTodos(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo")] HttpRequestData req)
        {
            _log.LogInformation("Getting todo items list");

            return new OkObjectResult(items);
        }

        [Function("GetTodoById")]
        public IActionResult GetTodoById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo/{id}")] HttpRequestData req,
            string id)
        {
            var todo = items.FirstOrDefault(t => t.Id == id);

            if (todo == null) return new NotFoundResult();

            return new OkObjectResult(todo);
        }

        [Function("UpdateTodo")]
        public async Task<IActionResult> UpdateTodo(
          [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "todo/{id}")] HttpRequestData req,
          string id)
        {
            var todo = items.FirstOrDefault(t => t.Id == id);

            if (todo == null) return new NotFoundResult();

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var updatedTodoItem = JsonConvert.DeserializeObject<TodoUpdateModel>(requestBody);

            todo.IsCompleted = updatedTodoItem.IsCompleted;

            if (!string.IsNullOrEmpty(updatedTodoItem.TaskDescription))
            {
                todo.TaskDescription = updatedTodoItem.TaskDescription;
            }

            return new OkObjectResult(todo);
        }

        [Function("DeleteTodo")]
        public IActionResult DeleteTodo(
           [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "todo/{id}")] HttpRequestData req,
           string id)
        {
            var todo = items.FirstOrDefault(t => t.Id == id);

            if (todo == null) return new NotFoundResult();

            items.Remove(todo);

            return new OkObjectResult(todo);
        }
    }
}
