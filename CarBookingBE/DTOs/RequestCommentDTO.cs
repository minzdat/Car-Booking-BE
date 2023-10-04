using CarBookingBE.DTOs;
using CarBookingBE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarBookingBE.DTOs
{
    public class RequestCommentDTO
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public AccountDTO Account { get; set; }
        public string Content { get; set; }
        public DateTime Created { get; set; }

        public ICollection<RequestCommentAttachment> RequestCommentAttachment { get; set; }
        /*public RequestDTO Request { get; set; }*/
    }
}