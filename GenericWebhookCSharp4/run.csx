#r "Newtonsoft.Json"
#r "Microsoft.WindowsAzure.Storage"

using System;
using Microsoft.WindowsAzure.Storage.Table;
using System.Net;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;

public static object Run(HttpRequestMessage req, IQueryable<outTable> inputTable, TraceWriter log)
{
    log.Info($"Webhook was triggered!");

    string jsonContent = req.Content.ReadAsStringAsync().Result;
    dynamic data = JsonConvert.DeserializeObject(jsonContent);
    
    string key = (string)data.key;

    var result = inputTable.Where(r => r.RowKey == key).Single();

    var numbers = result.ArrayOfValues.Split(',').Select(x => Convert.ToInt32(x)).ToArray();

    Array.Sort(numbers);
    
    var message = new Message {
        Key = key,
        ArrayOfValues = numbers
    };

    return req.CreateResponse(HttpStatusCode.OK, message);
}

class Message
{
	public string Key {get; set;}
	public int [] ArrayOfValues {get; set;}
}

public class outTable : TableEntity
{
	public string  ArrayOfValues { get; set; }
}