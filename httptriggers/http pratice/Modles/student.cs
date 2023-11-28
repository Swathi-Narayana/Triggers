using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.WindowsAzure.Storage.Table;
using System.Drawing;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;

namespace http_pratice.NewFolder
{
    
    public class Student 
    {
        [JsonProperty("id")]
        public string id { get; set; } = Guid.NewGuid().ToString();
       
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Name should only contain alphabets")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Age is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Age must be a positive number.")]
        public int Age { get; set; }

       [Required(ErrorMessage = "Date of Birth is required")]
        public DateTime? DOB { get; set; }

        [Required(ErrorMessage = "Phone is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone number should have 10 digits")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Phone number should only contain numeric digits")]
        public string Phone { get; set; }

        
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
    }

    public class UpdateShoppingCartItem
    {
        public String Name { get; set; }
    }

}
