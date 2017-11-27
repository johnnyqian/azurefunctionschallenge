#r "Newtonsoft.Json"

using System;
using System.Text;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;

public static async Task<object> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info($"Webhook was triggered!");

    string jsonContent = await req.Content.ReadAsStringAsync();
    InputMessage data = JsonConvert.DeserializeObject<InputMessage>(jsonContent);

    string hookUri = System.Environment.GetEnvironmentVariable("SlackWebHookUrl", EnvironmentVariableTarget.Process);
    var success = (data.Status == "success" && data.Complete);

    var postData = new {
        icon_emoji = success ? ":shipit:" : ":warning:",
        username = GetTitle(data, success),
        text = GetBody(data)
    };

    var client = new HttpClient();

    // post in json formatter
    HttpResponseMessage response = await client.PostAsync(hookUri,
        new StringContent(JsonConvert.SerializeObject(postData), Encoding.UTF8, "application/json"));

    if (response.IsSuccessStatusCode)
    {
        return req.CreateResponse(HttpStatusCode.OK, new
        {
            text = "OK."
        });
    }
    else
    {
        return req.CreateResponse(response.StatusCode, new
        {
            text = "Error occurred!"
        });
    }
}

public static string GetTitle(dynamic data, bool success)
{
    // return success ? "deployed:" : "failed: " + data.siteName + ".azurewebsites.net";
    return (success ? "deployed:" : "failed:") + " alpha.grandagro.com";
}

public static string GetBody(dynamic data)
{
    return
        "Initiated by: " + ValueOrDefault(data.Author, "unknown") +
        "\r\n" +
        "Commit id: " + data.Id + 
        "\r\n" +
        "```" +
        ValueOrDefault(data.Message, "null commit message") +
        "```";
}

public static string ValueOrDefault(string s, string sDefault)
{
    if (string.IsNullOrEmpty(s))
        return sDefault;
    return s;
}

public class InputMessage
{
    public string Id { get; set; }

    public string Status { get; set; }

    public string StatusText { get; set; }

    public string AuthorEmail { get; set; }

    public string Author { get; set; }

    public string Message { get; set; }

    public string Progress { get; set; }

    public string Deployer { get; set; }

    public DateTime ReceivedTime { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public DateTime LastSuccessEndTime { get; set; }

    public bool Complete { get; set; }

    public string SiteName { get; set; }
}
