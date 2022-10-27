using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace TodoList.FunctionApp
{
    public static class TodoApi
    {
        static List<Todo> items = new();

        [Function("CreateTodo")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todo")] HttpRequestData req, 
            FunctionContext context)
        {
            var logger = context.GetLogger("TodoApi");
            logger.LogInformation("Creating a new todo list item");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var input = JsonConvert.DeserializeObject<TodoCreateModel>(requestBody);

            var todo = new Todo { TaskDescription = input.TaskDescription };

            items.Add(todo);

            return new OkObjectResult(todo);
        }
    }
}
