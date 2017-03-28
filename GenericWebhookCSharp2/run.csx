#r "Newtonsoft.Json"

using System;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public static async Task<object> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info($"Webhook was triggered!");

    string jsonContent = await req.Content.ReadAsStringAsync();
    dynamic data = JsonConvert.DeserializeObject(jsonContent);

    if (data.msg == null || data.cipher == null) {
        return req.CreateResponse(HttpStatusCode.BadRequest, new {
            error = "Bad request"
        });
    }

    var jObj = (JObject)data.cipher;
    var dict = new Dictionary<int, char>();

    foreach (JToken token in jObj.Children())
    {
        if (token is JProperty)
        {
            var prop = token as JProperty;
            dict.Add(Convert.ToInt32(prop.Value), Convert.ToChar(prop.Name));
        }
    }

    var chars = Regex.Matches((string)data.msg, ".{2}").Cast<Match>().Select(m => dict[Convert.ToInt32(m.Value)]);

    return req.CreateResponse(HttpStatusCode.OK, new {
        key = data.key,
        result = new string(chars.ToArray())
    });
}