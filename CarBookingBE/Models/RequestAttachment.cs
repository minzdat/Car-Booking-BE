using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CarBookingTest.Models
{
    [Table("RequestAttachment")]
    public class RequestAttachment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Path { get; set; }
        public Guid? RequestId { get; set; }
        [ForeignKey("RequestId")]
        public Request Request { get; set; }

        [JsonIgnore]
        public bool IsDeleted { get; set; }
    }
}