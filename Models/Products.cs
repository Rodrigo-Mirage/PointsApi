using System;
using System.ComponentModel.DataAnnotations;

namespace PointsApi.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Photo_Url { get; set; }
        public double Value { get ; set; }
    }
}
