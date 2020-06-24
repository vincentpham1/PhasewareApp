using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PhasewareInterview.Models
{
    public class IncidentModel
    {
        [Required]
        public int IncidentID { get; set; }

        [MaxLength(100)]
        public string DepartmentName { get; set; }

        [MaxLength(150)]
        public string CustomerName { get; set; }

        [MaxLength(250)]
        public string Description { get; set; }

        [MaxLength(150)]
        public string AgentFullName { get; set; }
    }

    public class JsonIncidentModel
    {
        public int Count { get; set; }

        public IEnumerable<IncidentModel> Results { get; set; }
    }
}