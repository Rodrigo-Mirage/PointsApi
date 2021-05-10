using System;
using System.ComponentModel.DataAnnotations;

namespace PointsApi.Models
{
    public class PointLog
    {
        [Key]
        public int LogId { get; set; }
        public string UserID { get; set; }
        public int ProductId { get; set; }
        public string VendorId { get; set; }
        public double Value { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
