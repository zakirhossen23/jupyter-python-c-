using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Reimbursement_Web_System.Models
{
    public enum Status
    {
        [Display(Name = "Director Approved")]
        DirectorApproved,
        [Display(Name = "HSU Approved")]
        HSUApproved,
        [Display(Name = "HR Approved")]
        HRApproved,
        [Display(Name = "SDAS Approved")]
        SDASApproved,
        [Display(Name = "Finance Approved")]
        FinanceApproved,
        [Display(Name = "Director Rejected")]
        DirectorRejected,
        [Display(Name = "HSU Rejected")]
        HSURejected,
        [Display(Name = "HR Rejected")]
        HRRejected,
        [Display(Name = "SDAS Rejected")]
        SDASRejected,
        [Display(Name = "Finance Rejected")]
        FinanceRejected
    }

    public class Ticket
    {
        [Key]
        public int CRF { get; set; }
        public virtual User User { get; set; }


        [Required]
        [Display(Name = "Date Filed")]
        [DataType(DataType.Date, ErrorMessage = "Date only")]
        [DisplayFormat(DataFormatString = "{yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateFiled { get; set; }


        [Display(Name = "Date Completed")]
        [DataType(DataType.Date, ErrorMessage = "Date only")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DateCompleted { get; set; }


        [Display(Name = "Last Updated")]
        public DateTime? UpdateDateFiled { get; set; }
        public Status? Status { get; set; }
        [MaxLength(50)]
        [Required]
        public String Purpose { get; set; }
        [MaxLength(50)]
        [Required]
        public String Office { get; set; }
        [MaxLength(255)]
        [Display(Name = "Director Remarks")]
        [DataType(DataType.MultilineText)]
        public String DirectorRemarks { get; set; }
        [MaxLength(255)]
        [Display(Name = "Director Internal Comments")]
        [DataType(DataType.MultilineText)]
        public String DirectorInternalComments { get; set; }
        [MaxLength(255)]
        [Display(Name = "HSU Remarks")]
        [DataType(DataType.MultilineText)]
        public String HSURemarks { get; set; }
        [MaxLength(255)]
        [Display(Name = "HSU Internal Comments")]
        [DataType(DataType.MultilineText)]
        public String HSUInternalComments { get; set; }
        [MaxLength(255)]
        [Display(Name = "HR Remarks")]
        [DataType(DataType.MultilineText)]
        public String HRRemarks { get; set; }
        [MaxLength(255)]
        [Display(Name = "HR Internal Comments")]
        [DataType(DataType.MultilineText)]
        public String HRInternalComments { get; set; }
        [MaxLength(255)]
        [Display(Name = "SDAS Remarks")]
        [DataType(DataType.MultilineText)]
        public String SDASRemarks { get; set; }
        [MaxLength(255)]
        [Display(Name = "SDAS Internal Comments")]
        [DataType(DataType.MultilineText)]
        public String SDASInternalComments { get; set; }
        [MaxLength(255)]
        [Display(Name = "Finance Remarks")]
        [DataType(DataType.MultilineText)]
        public String FinanceRemarks { get; set; }
        [MaxLength(255)]
        [Display(Name = "Finance Internal Comments")]
        [DataType(DataType.MultilineText)]
        public String FinanceInternalComments { get; set; }
        public virtual List<Reimbursement> Reimbursement { get; set; }
        public virtual List<Media> Medias { get; set; }

        [MaxLength(255)]
        [Display(Name = "Director Status")]
        public String DirectorStatus { get; set; }

        [MaxLength(255)]
        [Display(Name = "HSU Status")]
        public String HSUStatus { get; set; }

        [MaxLength(255)]
        [Display(Name = "HR Status")]
        public String HRStatus { get; set; }

        [MaxLength(255)]
        [Display(Name = "SDAS Status")]
        public String SDASStatus { get; set; }

        [MaxLength(255)]
        [Display(Name = "Finance Status")]
        public String FinanceStatus { get; set; }



        [NotMapped]
        public HttpPostedFileBase[] ImagesUpload { get; set; }

    }
}