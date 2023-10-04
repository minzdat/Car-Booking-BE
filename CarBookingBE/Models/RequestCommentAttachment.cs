using CarBookingTest.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CarBookingBE.Models
{
    [Table("RequestCommentAttachment")]
    public class RequestCommentAttachment
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Path { get; set; }
        public Guid? RequestCommentId { get; set; }
        [ForeignKey("RequestCommentId")]
        public RequestComment RequestComment { get; set; }
        [JsonIgnore]
        public bool IsDeleted { get; set; }
    }
}