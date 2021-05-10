using System.ComponentModel.DataAnnotations;
using PointsApi.Options;
using PointsApi.Models;
using System.Collections.Generic;

namespace PointsApi.Models.Results
{
    public class HistoryResult {
        public double Total {get;set;}
        public List<PointLog> HistoryItens{get;set;}
    }

}