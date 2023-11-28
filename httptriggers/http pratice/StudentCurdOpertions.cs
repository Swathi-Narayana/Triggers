
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
using http_pratice.Modles;
using System.Diagnostics;
using Microsoft.VisualBasic;
using System.ComponentModel;
using http_pratice.Label;
using System.Collections;

namespace http_pratice
{
    public class StudentCurdOpertions
    { 
        private const string DatabaseName = "Student";
        private const string CollectionName = "StudentInformation";
        private readonly CosmosClient _cosmosClient;
        private Microsoft.Azure.Cosmos.Container documentContainer;

        public StudentCurdOpertions(CosmosClient cosmosClient)
        {
            _cosmosClient = cosmosClient;
            documentContainer = _cosmosClient.GetContainer("Student", "StudentInformation");
        }
        [FunctionName(Constant.GetAllStudent)]
        public async Task<IActionResult> GetAllStudentInfo(
            [HttpTrigger(AuthorizationLevel.Anonymous, Httpverbs.Get, Route =Routes.GetStudent)] HttpRequest req,
             [CosmosDB(DatabaseName,CollectionName,Connection ="CosmosDBConnectionString",
            CreateIfNotExists = true
        )]
       System.Collections.Generic.IEnumerable<Student> documents,
        ILogger log)

        {
            log.LogInformation("Getting list of all students");
            try
            {
              


                string gmessage = "Retrieved all items successfully";
                return StudentCurdOpertionsLabel.CreateResponse(true,documents, gmessage);
                
            }
            catch (Exception e) 
            {
                string gmessage = "Invalid Http verb";
                return StudentCurdOpertionsLabel.CreateResponse(false, null, gmessage);

            }

        }


        [FunctionName(Constant.GetByIdStudent)]
        public async Task<IActionResult> GetStudentInfoById(
            [HttpTrigger(AuthorizationLevel.Anonymous, Httpverbs.Get, Route = Routes.GetStudentId)]
             HttpRequest req, ILogger log, string id)

        {
            log.LogInformation($"Getting Student Cart Item with ID: {id}");
           
            try
            {
                var item = await documentContainer.ReadItemAsync<Student>(id, new Microsoft.Azure.Cosmos.PartitionKey(id));
                string gmessage = "Retrived  an item successfully by Id";
                return StudentCurdOpertionsLabel.CreateResponse(true, item.Resource, gmessage);
            }
            catch (CosmosException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                string gmessage = "Invalid input params,Please check";
                return StudentCurdOpertionsLabel.CreateResponse(false, null, gmessage);
            }
        }


        [FunctionName(Constant.PostStudent)]
        public async Task<IActionResult> PostStudentInfo(
            [HttpTrigger(AuthorizationLevel.Anonymous, Httpverbs.Post, Route = Routes.PostStudent)] HttpRequest req,

        ILogger log)
        {
            log.LogInformation("Records are Object creating");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<Student>(requestBody);
            var item = JsonConvert.DeserializeObject<Student>(requestBody);
            var validationResults = new List<ValidationResult>();
           

            try
            {
                
                // Validation
                if (!Validator.TryValidateObject(data, new ValidationContext(data, null, null), validationResults, true))
                {
                    throw new ValidationException(string.Join(", ", validationResults.Select(v => v.ErrorMessage)));
                }
                try
                {
                    var existingItem = await documentContainer.ReadItemAsync<Student>(data.id, new PartitionKey(data.id));
                    string errorMessage = $"Item with id '{data.id}' already exists.";
                    return StudentCurdOpertionsLabel.CreateResponse(false, null, errorMessage);
                }
                catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    
                    log.LogInformation($"Employee object: {JsonConvert.SerializeObject(data)}");
                    await documentContainer.UpsertItemAsync(data, new PartitionKey(data.id));
                    string successMessage = "Created an item successfully";
                    return StudentCurdOpertionsLabel.CreateResponse(true, data, successMessage);
                }
              
            }
            catch (ValidationException validationEx)
            {
                dynamic message = new ExpandoObject();
                message.errors = validationEx.Message;
                return StudentCurdOpertionsLabel.CreateResponse(false, null, message.errors);
            }
            
           
        }


        [FunctionName(Constant.DeleteStudent)]
        public async Task<IActionResult> DeleteStudentInfo(
                   [HttpTrigger(AuthorizationLevel.Anonymous, Httpverbs.Delete, Route = Routes.DeleteStudent)] HttpRequest req,
                   ILogger log, string id)
        {
            try
            {
                log.LogInformation($"Deleting StudentInfo Item with ID: {id}");
                await documentContainer.DeleteItemAsync<Student>(id, new Microsoft.Azure.Cosmos.PartitionKey(id));
                string gmessage = "Deleted sucessfully";
                return StudentCurdOpertionsLabel.CreateResponse(true,null,gmessage);
            }
            catch (CosmosException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                string gmessage = "Invalid input params,Please check";
                return StudentCurdOpertionsLabel.CreateResponse(false, null, gmessage);
            }
        }


        [FunctionName(Constant.UpadteStudent)]
        public async Task<IActionResult> UpdateStudentInfo(
                  [HttpTrigger(AuthorizationLevel.Anonymous, Httpverbs.Put, Route = Routes.UpdateStudent)] HttpRequest req,
                  ILogger log, string id)
        {
            log.LogInformation($"Update Student Info with ID: {id}");

            string requestData = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<UpdateShoppingCartItem>(requestData);
           
            try
            {
                var item = await documentContainer.ReadItemAsync<Student>(id, new Microsoft.Azure.Cosmos.PartitionKey(id));
                item.Resource.Name = data.Name;
                await documentContainer.UpsertItemAsync(item.Resource);
                string gmessage = "Updated successfully";
                return StudentCurdOpertionsLabel.CreateResponse(true, item.Resource, gmessage);
            }
            catch (CosmosException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                string gmessage = "Invalid input params,Please check";
                return StudentCurdOpertionsLabel.CreateResponse(false, null, gmessage);
            }

        }

    }
}








