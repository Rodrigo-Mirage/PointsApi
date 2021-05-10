using System.ComponentModel.DataAnnotations;
using PointsApi.Options;

namespace PointsApi.Models.Results
{
    public class Result {
        public bool Success {get;set;}
        public string Message {get;set;}
        public int Code {get;set;}
    }

}