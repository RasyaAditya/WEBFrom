using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using WEBFrom.Data;
using WEBFrom.Models;

namespace WEBFrom.Controllers
{
    public class RequestController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly string SenderEmail = "rasya.aditya040807@gmail.com";
        private readonly string AppPassword = "whjz ucaw mncl gwxg";

        public RequestController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SubmitSuccess()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitRequest(EmployeeRequest model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            if (string.IsNullOrWhiteSpace(model.RequestorEmail))
            {
                ModelState.AddModelError("", "Email requestor wajib diisi.");
                return View("Index", model);
            }

            model.SubmissionTime = DateTime.Now;
            model.Status = "Pending";
            model.ApprovalToken = Guid.NewGuid();

            _context.EmployeeRequests.Add(model);
            await _context.SaveChangesAsync();

            SendFullEmailToHR(model);

            return RedirectToAction("SubmitSuccess");
        }

        [HttpGet]
        public async Task<IActionResult> ProcessApproval(Guid token, string decision)
        {
            var request = await _context.EmployeeRequests
                .FirstOrDefaultAsync(x => x.ApprovalToken == token);

            if (request == null)
                return Content("Request tidak ditemukan.");

            if (request.Status != "Pending")
                return View("ApprovalResult", request);

            request.Status = decision;
            await _context.SaveChangesAsync();

            SendReplyToUser(request);

            return View("ApprovalResult", request);
        }

        private void SendFullEmailToHR(EmployeeRequest m)
        {
            string baseUrl = $"{Request.Scheme}://{Request.Host}";
            string approveUrl = $"{baseUrl}/Request/ProcessApproval?token={m.ApprovalToken}&decision=Approved";
            string rejectUrl = $"{baseUrl}/Request/ProcessApproval?token={m.ApprovalToken}&decision=Rejected";

            string body = $@"
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background:#f4f6f8; padding:20px; font-family:Arial, sans-serif;"">
  <tr>
    <td align=""center"">

      <!-- Card -->
      <table width=""700"" cellpadding=""0"" cellspacing=""0"" style=""background:#ffffff; border-radius:6px; overflow:hidden;"">
        
        <!-- Header -->
        <tr>
          <td style=""padding:20px 25px; border-bottom:1px solid #e5e5e5;"">
            <h2 style=""margin:0; font-size:22px; color:#000;"">
              New GD Profile Request has been submitted
            </h2>
          </td>
        </tr>

        
        

        <tr>
          <td style=""border-top:1px solid #e5e5e5;""></td>
        </tr>

        <!-- Main Content -->
        <tr>
          <td style=""padding:20px 25px;"">
            <table width=""100%"" cellpadding=""0"" cellspacing=""0"">
              <tr valign=""top"">

                <!-- Avatar -->
                <td width=""90"" align=""center"">
                  <div style=""
                    width:70px;
                    height:70px;
                    background:#1a73e8;
                    color:#ffffff;
                    border-radius:50%;
                    font-size:26px;
                    font-weight:bold;
                    line-height:70px;
                    text-align:center;"">
                    RA
                  </div>
                </td>

                <!-- Detail -->
                <td style=""padding-left:20px;"">
                  <p style=""margin:0 0 10px 0;"">
              Diminta oleh <b>{m.LastName?.ToString().ToUpper()} "" {m.FirstName?.ToString()} "" <{m.EmailAxa}></b> &lt;
              <a href=""mailto:{m.RequestorEmail}"" style=""color:#1a73e8; text-decoration:none;"">
                {m.RequestorEmail}
              </a>&gt;
            </p>

            <p style=""margin:4px 0;""><b>Requestor Email :</b> {m.RequestorEmail}</p>
            <p style=""margin:4px 0;""><b>Reason :</b> {m.Reason}</p>
            <p style=""margin:4px 0;""><b>Submission Time :</b> {m.SubmissionTime:G}</p>
                  <p style=""margin:4px 0;""><b>Entity :</b> {m.Entity}</p>
                  <p style=""margin:4px 0;""><b>Worker Type :</b> {m.WorkerType}</p>
                  <p style=""margin:4px 0;""><b>ID Person YES :</b> {m.IDPersonYes}</p>
                  <p style=""margin:4px 0;""><b>Personal Title :</b> {m.PersonalTitle}</p>
                  <p style=""margin:4px 0;""><b>Full Name (According to KTP) :</b> {m.FullName}</p>
                  <p style=""margin:4px 0;""><b>First Name :</b> {m.FirstName}</p>
                  <p style=""margin:4px 0;""><b>Last Name (Only 1 Syilable) :</b> {m.LastName}</p>
                  <p style=""margin:4px 0;""><b>Site Address :</b> {m.SiteAddress}</p>
                  <p style=""margin:4px 0;""><b>City :</b> {m.City}</p>
                  <p style=""margin:4px 0;""><b>Office Floor (Example : 10th Floor) :</b> {m.OfficeFloor}</p>
                  <p style=""margin:4px 0;""><b>Office Phone (Start with +62) :</b> {m.OfficePhone}</p>
                  <p style=""margin:4px 0;""><b>Office Fax (Start with +62) :</b> {m.OfficeFax}</p>
                  <p style=""margin:4px 0;""><b>Mobile Phone (Start with +62) :</b> {m.MobilePhone}</p>
                  <p style=""margin:4px 0;""><b>Email AXA :</b>
                    <a href=""mailto:{m.EmailAxa}"" style=""color:#1a73e8; text-decoration:none;"">
                      {m.EmailAxa}
                    </a>
                  </p>
                  <p style=""margin:4px 0;""><b>Professional Family :</b> {m.ProfesionalFamily}</p>
                  <p style=""margin:4px 0;""><b>Title / Position :</b> {m.TitlePositionName}</p>
                  <p style=""margin:4px 0;""><b>Department [Superior's Department] :</b> {m.Department}</p>
                  <p style=""margin:4px 0;""><b>Reporting to [Manager Name] :</b> {m.ReportingTo}</p>
                  <p style=""margin:4px 0;""><b>Contract Start Date :</b> {m.ContractStartDate:yyyy-MM-dd}</p>
                  <p style=""margin:4px 0;""><b>Contract / Projected End Date :</b> {m.ContractEndDate:yyyy-MM-dd}</p>

                  <p style=""margin:4px 0;""><b>Tanggal Dibuat {DateTime.Now.ToString()} </b></p>
                </td>

              </tr>
            </table>
          </td>
        </tr>

        <!-- Footer Buttons -->
        <tr>
          <td style=""padding:25px; border-top:1px solid #e5e5e5;"">
            <a href=""{approveUrl}"" style=""
              background:#1a73e8;
              color:#ffffff;
              padding:10px 24px;
              text-decoration:none;
              border-radius:4px;
              font-weight:bold;
              display:inline-block;"">
              Setujui &gt;
            </a>

            &nbsp;&nbsp;

            <a href=""{rejectUrl}"" style=""
              background:#f2f2f2;
              color:#333333;
              padding:10px 24px;
              text-decoration:none;
              border-radius:4px;
              font-weight:bold;
              border:1px solid #cccccc;
              display:inline-block;"">
              Tolak &gt;
            </a>
          </td>
        </tr>

      </table>
      <!-- End Card -->

    </td>
  </tr>
</table>
";


            ExecuteMail(
                SenderEmail,
                "hris-mailbox@axa.co.id",
                "New GD Profile Request Submitted",
                body
            );
        }

        private void SendReplyToUser(EmployeeRequest m)
        {
            string body = $@"
            <p>Halo {m.RequestorName},</p>
            <p>Request GD Profile untuk <b>{m.FullName}</b>
            telah <b>{m.Status}</b>.</p>
            <p>Terima kasih.</p>";

            ExecuteMail(
                SenderEmail,
                m.RequestorEmail,
                $"Update Status Request: {m.Status}",
                body
            );
        }

        private void ExecuteMail(string from, string to, string subject, string body)
        {
            try
            {
                using var smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(SenderEmail, AppPassword)
                };

                using var mail = new MailMessage(from, to)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                throw new Exception("Gagal mengirim email: " + ex.Message);
            }
        }
    }
}
