using CarBookingTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.Json.Serialization;
using System.Runtime.Serialization;

namespace CarBookingBE.DTOs
{
    public class RequestDetailDTO
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string RequestCode { get; set; }
        public AccountDTO SenderUser { get; set; }
        public DepartmentDTO Department { get; set; }
        public AccountDTO ReceiverUser { get; set; }
        public DateTime? Created { get; set; }
        public string Mobile { get; set; }
        public string CostCenter { get; set; }
        public int? TotalPassengers { get; set; }
        public DateTime? UsageFrom { get; set; }
        public DateTime? UsageTo { get; set; }
        public DateTime? PickTime { get; set; }
        public string PickLocation { get; set; }
        public string Destination { get; set; }
        public string Reason { get; set; }
        public Account ShareUser { get; set; }
        public string Note { get; set; }
        public bool? ApplyNote { get; set; }

        public string Status { get; set; }

        public bool IsDeleted { get; set; }

        public List<RequestWorkflowDTO> RequestWorkflow { get; set; }

    }
}