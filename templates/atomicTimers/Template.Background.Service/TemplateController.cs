public class TemplateController
{
    [RunsEvery("00:00:01")]
    public async Task<string> Tick() 
    {
        await Task.Delay(10);
        return "sec passed...";
    }
}