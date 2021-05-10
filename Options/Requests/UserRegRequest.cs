using System.ComponentModel.DataAnnotations;

namespace PointsApi.Models.Requests
{
    public class UserRegRequest{
        [Required]
        public string UserName {get;set;}
        [Required]
        [EmailAddress]
        public string Email {get;set;}

        [Required]
        public string Password {get;set;}
    }

}