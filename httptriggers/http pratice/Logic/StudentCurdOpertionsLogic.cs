
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Dynamic;
namespace http_pratice.Label
{
    public static class StudentCurdOpertionsLabel
    {
        public static IActionResult CreateResponse(bool success, object data, string message)
        {
            dynamic responseData = new ExpandoObject();
            responseData.success = success;
            responseData.message = message;
            responseData.Data = data;
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(responseData);
            return new OkObjectResult(json);
        }
    }
}

