using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Reimbursement_Web_System.Models
{
    public class Media
    {
        [Key]
        public int Id { get; set; }
        public string ImagePath { get; set; }
        public int TicketCRF { get; set; }
        public Ticket Ticket { get; set; }
      //public string UploadDate { get; set; }
    }
}