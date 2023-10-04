using CarBookingBE.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CarBookingTest.Models
{
    [Table("RequestWorkflow")]
    public class RequestWorkflow
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? UserId { get; set; }
        [ForeignKey("UserId")]
        [JsonIgnore]
        public Account User { get; set; }
        public int Level { get; set; }
        public string Status { get; set; }
        [JsonIgnore]
        public bool IsDeleted { get; set; }
        public Guid? RequestId { get; set; }
        [ForeignKey("RequestId")]
        [JsonIgnore]
        public Request Request { get; set; }

        public static implicit operator RequestWorkflow(RequestWorkflowDTO v)
        {
            throw new NotImplementedException();
        }
    }
}