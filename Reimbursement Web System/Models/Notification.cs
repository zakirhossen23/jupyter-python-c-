using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Reimbursement_Web_System.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }
        public string message { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}