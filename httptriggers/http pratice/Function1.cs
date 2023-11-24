
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Dynamic;
using System.Collections.Generic;
using Newtonsoft.Json;
using http_pratice.NewFolder;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.Azure.Cosmos;
using System.Drawing;
using System.Configuration;
using System.ComponentModel;

namespace http_pratice
{
    public class Function1
    {

        private const string DatabaseName = "Student";
        private const string CollectionName = "StudentInformation";
        private readonly CosmosClient _cosmosClient;
        private Microsoft.Azure.Cosmos.Container  documentContainer;

        public Function1(CosmosClient cosmosClient)
        {
            _cosmosClient = cosmosClient;
            documentContainer = _cosmosClient.GetContainer("Student", "StudentInformation");
        }

        [FunctionName("GetInfo")]

        public static async Task<IActionResult> GetInfo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "getallstudentinfo")] HttpRequest req,
             [CosmosDB(DatabaseName,CollectionName,Connection ="CosmosDBConnectionString",

            CreateIfNotExists = true
        )]
        IAsyncCollector<Student> documents,
        ILogger log)
        {
            log.LogInformation("Records are getting");
            string respondmessage = "Retrived all students information sucessfull";
            dynamic getmydata = new ExpandoObject();
            getmydata.message = respondmessage;
            getmydata.Data = documents;
            string jsondata = Newtonsoft.Json.JsonConvert.SerializeObject(getmydata);
            log.LogInformation("successfully");
            return new OkObjectResult(jsondata);
        }




        [FunctionName("PostInfo")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
         
        ILogger log)
        {
            log.LogInformation("Records are Object creating");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<Student>(requestBody);

            //Validate the input
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(data, new ValidationContext(data, null, null), validationResults, true))
            {
                return new BadRequestObjectResult($"Invalid input: {string.Join(", ", validationResults.Select(v => v.ErrorMessage))}");
            }


            var item = new Student
            {
               id=data.id,
                Name = data.Name,
                Age = data.Age,
                DOB = data.DOB,
                Phone = data.Phone,
                Email = data.Email

            };
            
            log.LogInformation($"Employee object: {JsonConvert.SerializeObject(item)}");
            var cosmosDBConnection = Environment.GetEnvironmentVariable("CosmosDBConnectionString");
            var cosmosClient = new CosmosClient(cosmosDBConnection);
            var container = cosmosClient.GetContainer(DatabaseName, CollectionName);

            // Use CreateItemAsync to save to Cosmos DB
            await container.UpsertItemAsync(item, new PartitionKey(item.id));
            log.LogInformation($"Employee added to Cosmos DB: {item.id}");
                return new OkObjectResult("Data is valid. Employee record saved to Cosmos DB.");
            
            

        }
        [FunctionName("GetShoppingCartItemById")]
        public async Task<IActionResult> GetShoppingCartItemById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "getshoppingcartitembyid/{id}")]
             HttpRequest req, ILogger log, string id)

        {
            log.LogInformation($"Getting Shopping Cart Item with ID: {id}");
            try
            {
                var item = await documentContainer.ReadItemAsync<Student>(id, new Microsoft.Azure.Cosmos.PartitionKey(id));
                string getmessage = "Retrived  an item successfully by Id";
                dynamic gmydata = new ExpandoObject();
                gmydata.message = getmessage;
                gmydata.Data = item.Resource;
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(gmydata);
                return new OkObjectResult(json);
            }
            catch (CosmosException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                string responseMessage = "Invalid input params,Please check";
                return new NotFoundObjectResult(responseMessage);
            }
        }
        [FunctionName("DeleteShoppingCartItem")]
        public async Task<IActionResult> DeleteShoppingCartItems(
                   [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "delshoppingcartitem/{id}")] HttpRequest req,
                   ILogger log, string id )
        {
            log.LogInformation($"Deleting Shopping Cart Item with ID: {id}");

            await documentContainer.DeleteItemAsync<Student>(id, new Microsoft.Azure.Cosmos.PartitionKey(id));
            string responseMessage = "Deleted sucessfully";
            return new OkObjectResult(responseMessage);
        }
    }
}
    







