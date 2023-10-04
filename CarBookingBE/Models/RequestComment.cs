using CarBookingBE.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace CarBookingTest.Models
{
    [Table("RequestComment")]
    public class RequestComment
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid? UserId { get; set; }
        [ForeignKey("UserId")]
        public Account Account { get; set; }
        public string Content { get; set; }
        public DateTime Created { get; set; }
        public Guid? RequestId { get; set; }
        [ForeignKey("RequestId")]
        public Request Request { get; set; }
        [JsonIgnore]
        public bool IsDeleted { get; set; }

        [IgnoreDataMember]
        public virtual ICollection<RequestCommentAttachment> RequestCommentAttachments { get; set; }
    }
}