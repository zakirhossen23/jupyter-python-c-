using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Reimbursement_Web_System.Models
{
    public class Reimbursement
    {
        [Key]
        public int Id { get; set; }
        public int TicketCRF { get; set; }
        public Ticket Ticket { get; set; }

        [Required]
        [Display(Name = "Date")]
        [DataType(DataType.Date, ErrorMessage = "Date only")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [MaxLength(255)]
        [Display(Name = "Nature of Expenditure ")]
        public String NatureOfExpenditure { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public Boolean Status { get; set; }

    }
}