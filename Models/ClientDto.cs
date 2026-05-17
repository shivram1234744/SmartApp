using System.ComponentModel.DataAnnotations.Schema;

namespace TrustPlus.Models
{

    [Table("Client_Dtl")]
    public class VerificationRequest
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public DateTime? DOB { get; set; }
        public string? Position { get; set; }
        public string? Department { get; set; }

       public string? DocumentPath { get; set; }

        public string? VerifyType { get; set; }

        public string? Priority { get; set; }
        public string? ExecutorNotes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public class VerificationVM
    {
        public string? Name { get; set; }
       


        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public DateTime? DOB { get; set; }
        public string? Position { get; set; }
        public string? Department { get; set; }
        public string? Priority { get; set; }

      
        public IFormFile DocumentPath { get; set; }

        public List<int> VerifyTypes { get; set; }
    }

    public class Mst_VerificationCheck
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ClientDtl
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        [NotMapped]
        public string? EmployeeName { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? DOB { get; set; }
        public string? Position { get; set; }
        public string? Department { get; set; }
        public string? VerifyType { get; set; }
        public string? Priority { get; set; }
        public string? RequestNo { get; set; }
        public string? Status { get; set; }
        public string? DocumentPath { get; set; }
        public string? FinalReport { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [NotMapped]
        public int Progress { get; set; }


    }
    [Table("tbl_verification")]
    public class Verification
    {
        public int Id { get; set; }
        public int? Emp_Id { get; set; }

        public int ClientId { get; set; }

        public int VarId { get; set; }

        public string? Status { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string? DocumentPath { get; set; }

        public string? Flag { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string? ExecutorNotes { get; set; }

        // ONLY FOR VIEW
        [NotMapped]
        public string? Name { get; set; }

        // FILE UPLOAD
        [NotMapped]
        public IFormFile? EvidenceFile { get; set; }
    }

    public class VerificationSubmitVM
    {
        public int ClientId { get; set; }

        public List<Verification> Verifications { get; set; } = new List<Verification>();

        public IFormFile? FinalReportFile { get; set; }
        public string? FinalReport { get; set; }
        public int Progress { get; set; }
    }

}
