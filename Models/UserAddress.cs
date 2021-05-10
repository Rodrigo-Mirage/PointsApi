using System;
using System.ComponentModel.DataAnnotations;

namespace PointsApi.Models
{
    public class UserAddess
    {
        [Key]
        public int AddressId { get; set; }
        public string Name { get; set; }
    }
}
