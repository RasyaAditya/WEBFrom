namespace WEBFrom.Models
{
    public class EmployeeRequest
    {
        public int Id { get; set; }

        
        public string? RequestorName { get; set; }
        public string RequestorEmail { get; set; } = null!; 
        public string? Reason { get; set; } 
        public DateTime SubmissionTime { get; set; } = DateTime.UtcNow;
        public string? Entity { get; set; } 
        public string? WorkerType { get; set; } 
        public string? IDPersonYes { get; set; } 
        public string? PersonalTitle { get; set; } 
        public string? FullName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? SiteAddress { get; set; } 
        public string? City { get; set; }
        public string? OfficeFloor { get; set; } 
        public string? OfficePhone { get; set; } 
        public string? OfficeFax { get; set; }
        public string? MobilePhone { get; set; }
        public string? EmailAxa { get; set; } 
        public string? ProfesionalFamily { get; set; } 
        public string? TitlePositionName { get; set; } 
        public string? Department { get; set; } 
        public string? ReportingTo { get; set; } 
        public DateTime? ContractStartDate { get; set; } 
        public DateTime? ContractEndDate { get; set; } 
        public string? Status { get; set; } = "Pending";
        public Guid ApprovalToken { get; set; } = Guid.NewGuid();
    }
}
