using CarBookingTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarBookingBE.DTOs
{
    public class NewRequestDTO
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string RequestCode { get; set; }
        public Guid? SenderId { get; set; }
        public Guid? DepartmentId { get; set; }
        public Guid? ReceiverId { get; set; }
        public DateTime? Created { get; set; }
        public Guid? Share { get; set; }
        public bool? ApplyNote { get; set; }

        public string Status { get; set; }

        public virtual ICollection<RequestAttachment> RequestAttachments { get; set; }

        public virtual ICollection<RequestWorkflow> RequestWorkflows { get; set; }
    }
}