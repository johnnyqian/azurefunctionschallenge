#r "Newtonsoft.Json"

using System;
using System.Net;
using Newtonsoft.Json;

public static object Run(HttpRequestMessage req, out OutputTable outputTable, TraceWriter log)
{
    log.Info($"Webhook was triggered!");

    string jsonContent = req.Content.ReadAsStringAsync().Result;
    dynamic data = JsonConvert.DeserializeObject(jsonContent);

    var item = new OutputTable();
    item.PartitionKey = "AzureFunction";
    item.RowKey = data.key;
    item.ArrayOfValues = String.Join(",",data.ArrayOfValues.ToObject<int[]>());
    outputTable = item;

    return req.CreateResponse(HttpStatusCode.OK, new {});
}

public class OutputTable
{
    public string PartitionKey {get; set;}

    public string RowKey {get; set;}

    public string ArrayOfValues {get; set;}
}