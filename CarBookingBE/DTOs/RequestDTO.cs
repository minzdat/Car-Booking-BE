using CarBookingTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarBookingBE.DTOs
{
    public class RequestDTO
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string RequestCode { get; set; }
        public AccountDTO SenderUser { get; set; }
        public DepartmentDTO Department { get; set; }
        public AccountDTO ReceiveUser { get; set; }

        public DateTime? Created { get; set; }
        public DateTime? UsageFrom { get; set; }
        public DateTime? UsageTo { get; set; }
        public string Status { get; set; }

        public List<Guid?> RequestWorkflows { get; set; }

    }


}