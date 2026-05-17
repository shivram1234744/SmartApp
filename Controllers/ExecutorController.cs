using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrustPlus.Models;

namespace TrustPlus.Controllers
{
    [Authorize(Roles = "Executor")]
    public class ExecutorController : Controller
    {
        private readonly DbManager _db;

        public ExecutorController(DbManager db)
        {
            _db = db;
        }

        public IActionResult Dashboard()
        {
            var data = _db.Client_Dtl.ToList();

            foreach (var item in data)
            {
                var total = _db.Verification_Status.Count(x => x.ClientId == item.Id);

                var completed = _db.Verification_Status.Count(x =>
                    x.ClientId == item.Id &&
                    x.Status == "Completed");

                item.Progress = total > 0 ? (completed * 100 / total) : 0;
            }

            var pending = _db.Client_Dtl
    .Count(x => x.Status == "Pending");

            var inProgress = _db.Client_Dtl
                .Count(x => x.Status == "In Progress");

            var complete = _db.Client_Dtl
                .Count(x => x.Status == "Completed");

            ViewBag.Pending = pending;
            ViewBag.InProgress = inProgress;
            ViewBag.Completed = complete;
            //var data = _db.Client_Dtl
            //    .OrderByDescending(x => x.Id)
            //    .ToList();

            return View(data);
     
        }
        public IActionResult MyCases()
        {
            var data = _db.Client_Dtl.ToList();

            foreach (var item in data)
            {
                var total = _db.Verification_Status.Count(x => x.ClientId == item.Id);

                var completed = _db.Verification_Status.Count(x =>
                    x.ClientId == item.Id &&
                    x.Status == "Completed");

                item.Progress = total > 0 ? (completed * 100 / total) : 0;
            }

            var pending = _db.Client_Dtl
    .Count(x => x.Status == "Pending");

            var inProgress = _db.Client_Dtl
                .Count(x => x.Status == "In Progress");

            var complete = _db.Client_Dtl
                .Count(x => x.Status == "Completed");

            ViewBag.Pending = pending;
            ViewBag.InProgress = inProgress;
            ViewBag.Completed = complete;
            //var data = _db.Client_Dtl
            //    .OrderByDescending(x => x.Id)
            //    .ToList();

            return View(data);


        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Details(int id, bool success = false)
        {
           
            if (id != 0)
            {
                VerificationSubmitVM vm = new VerificationSubmitVM();

                vm.ClientId = id;

                vm.Verifications = (from s in _db.Verification_Status
                                    join n in _db.Mst_VerificationChecks
                                    on s.VarId equals n.Id into temp
                                    from n in temp.DefaultIfEmpty()

                                    where s.ClientId == id

                                    select new Verification
                                    {
                                        Id = s.Id,
                                        ClientId = s.ClientId,
                                        VarId = s.VarId,
                                        Status = s.Status,
                                        CreatedAt = s.CreatedAt,
                                        DocumentPath = s.DocumentPath,
                                        ExecutorNotes = s.ExecutorNotes,
                                        UpdatedAt = s.UpdatedAt,
                                        Flag = s.Flag,
                                        Name = n != null ? n.Name : ""
                                    }).ToList();

               
                var total = vm.Verifications.Count;

                var completed = vm.Verifications.Count(x =>
                    x.Status != null && x.Status == "Completed"
                );

                vm.Progress = total > 0
                    ? (int)((completed * 100.0) / total)
                    : 0;

                var client = _db.Client_Dtl.FirstOrDefault(x => x.Id == id);

                ViewBag.Data = client;

                vm.FinalReport = client?.FinalReport;

                return View(vm);
            }

            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        public async Task<IActionResult> SubmitVerification(VerificationSubmitVM model)
        {
            if (!ModelState.IsValid)
            {
                return View("Details", model);
            }

            // =========================
            // FINAL REPORT UPLOAD
            // Client_Dtl TABLE
            // =========================

            string finalReportPath = "";

            if (model.FinalReportFile != null)
            {
                string uploadFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/uploads");

                // Folder create if not exists
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                string fileName = Guid.NewGuid().ToString()
                                + Path.GetExtension(model.FinalReportFile.FileName);

                string fullPath = Path.Combine(uploadFolder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await model.FinalReportFile.CopyToAsync(stream);
                }

                finalReportPath = fileName;
            }

            // =========================
            // UPDATE CLIENT_DTL
            // =========================

            var clientData = await _db.Client_Dtl
                .FirstOrDefaultAsync(x => x.Id == model.ClientId);

            if (clientData != null)
            {
                if (!string.IsNullOrEmpty(finalReportPath))
                {
                    clientData.FinalReport = finalReportPath;
                }

                clientData.UpdatedAt = DateTime.Now;
            }

            // =========================
            // UPDATE tbl_verification
            // =========================

            foreach (var item in model.Verifications)
            {
                var verificationData = await _db.Verification_Status
                    .FirstOrDefaultAsync(x => x.Id == item.Id);

                if (verificationData != null)
                {
                    string documentPath = verificationData.DocumentPath;

                    // =========================
                    // EVIDENCE FILE UPLOAD
                    // =========================

                    if (item.EvidenceFile != null)
                    {
                        string uploadFolder = Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot/uploads");

                        if (!Directory.Exists(uploadFolder))
                        {
                            Directory.CreateDirectory(uploadFolder);
                        }

                        string fileName = Guid.NewGuid().ToString()
                                        + Path.GetExtension(item.EvidenceFile.FileName);

                        string fullPath = Path.Combine(uploadFolder, fileName);

                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            await item.EvidenceFile.CopyToAsync(stream);
                        }

                        documentPath = fileName;
                    }

                    // =========================
                    // UPDATE DATA
                    // =========================

                    verificationData.Status = item.Status;
                    verificationData.Flag = item.Flag;
                    verificationData.ExecutorNotes = item.ExecutorNotes;
                    verificationData.DocumentPath = documentPath;
                    verificationData.UpdatedAt = DateTime.Now;
                }
            }

            // SAVE DATABASE
            await _db.SaveChangesAsync();

            TempData["success"] = "Verification Updated Successfully";
            return RedirectToAction("Details", new { id = model.ClientId });
        }
    }
}
