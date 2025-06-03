using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;


namespace RazorPagesToDo.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public List<Todo> Todos { get; set; } = new();

    [BindProperty]
    public string Name { get; set; }

    public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public async Task OnGet()
    {
        var client = _httpClientFactory.CreateClient();

        var response = await client.GetAsync("http://localhost:5215/todo");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();

        Todos = JsonConvert.DeserializeObject<List<Todo>>(result);
    }

    public async Task<IActionResult> OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var client = _httpClientFactory.CreateClient();

        var newTask = new Todo
        {
            Name = Name,
            IsComplete = false
        };

        var response = await client.PostAsJsonAsync("http://localhost:5215/todo", newTask);
        response.EnsureSuccessStatusCode();

        return RedirectToPage();
    }


    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var client = _httpClientFactory.CreateClient();

        var response = await client.DeleteAsync($"http://localhost:5215/todo/{id}");
        response.EnsureSuccessStatusCode();

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostToggleCompleteAsync(int id, bool isComplete)
{
    var client = _httpClientFactory.CreateClient();

    var todoUpdate = new { IsComplete = isComplete };

    var response = await client.PutAsJsonAsync($"http://localhost:5215/todo/{id}", todoUpdate);
    response.EnsureSuccessStatusCode();

    return RedirectToPage();
}


}
