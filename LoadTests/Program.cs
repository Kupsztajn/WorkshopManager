using NBomber.Contracts;
using NBomber.CSharp;
using NBomber;
using NBomber.Contracts.Stats;

var httpClient = new HttpClient();

var scenario = Scenario.Create("vehicles_api_test", async context =>
{
    var response = await httpClient.GetAsync("http://localhost:5299/Part/List");

    return response.IsSuccessStatusCode
        ? Response.Ok()
        : Response.Fail(statusCode: response.StatusCode.ToString());
})
.WithWarmUpDuration(TimeSpan.FromSeconds(2))
.WithLoadSimulations(Simulation.InjectRandom(
    minRate: 50, 
    maxRate: 51, 
    interval: TimeSpan.FromSeconds(1), 
    during: TimeSpan.FromSeconds(2)
    ));

NBomberRunner
    .RegisterScenarios(scenario)
    .WithReportFileName("report")
    // eksportujemy m.in do formatu Markdown, który można łatwo przekonwertować na PDF
    .Run();