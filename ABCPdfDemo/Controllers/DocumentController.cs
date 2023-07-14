using Microsoft.AspNetCore.Mvc;
using WebSupergoo.ABCpdf12; 
using Microsoft.AspNetCore.Authorization;

namespace ABCPdf.Demo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DocumentController : BaseController
    {

        private readonly ILogger<DocumentController> _logger;

        public DocumentController(ILogger<DocumentController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("License")]
        public IActionResult PDFLicense()
        {
            try
            {
                var key = "";
                
                var msg = "New License: " + XSettings.LicenseDescription;
                if (XSettings.InstallLicense(key))
                    msg += "\nLicense Installed Successfully: " + XSettings.LicenseDescription;
                else
                    msg += "\nLicense Installation Failed";
                return Content(msg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " - Exception");

                return ExceptionProblemDetails(ex, HttpContext);
            }
        }
        
        [HttpGet]
        [Route("CreatePDF")]
        public IActionResult CreatePDF(string fileName, string msg)
        {
            try
            {
                using (var doc = new Doc())
                {
                    doc.FontSize = 96;
                    doc.AddText(msg);
                    Response.Headers.Clear();
                    Response.Headers.Add("content-disposition", "attachment; filename=" + fileName + ".pdf");
                    return new FileStreamResult(doc.GetStream(), "application/pdf");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " - Exception");

                return ExceptionProblemDetails(ex, HttpContext);
            }
        }
        
        [HttpPost]
        [Route("Docx2PDF")]
        public async Task<IActionResult> Docx2PDF(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogError("No file was uploaded.");
                return BadRequest("No file was uploaded.");
            }
            try
            {
                using (var doc = new Doc())
                {
                    using (var stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);
                        stream.Position = 0;

                        doc.Read(stream, new XReadOptions() { FileExtension = "docx" });

                        using (var pdfStream = new MemoryStream())
                        {
                            doc.Save(pdfStream);
                            pdfStream.Position = 0;

                            var contentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                            {
                                FileName = $"{file.FileName}.pdf"
                            };
                            Response.Headers.Add("Content-Disposition", contentDisposition.ToString());

                            _logger.LogInformation("Docx to PDF Converted.");

                            return File(pdfStream.ToArray(), "application/pdf");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " - Exception");

                return ExceptionProblemDetails(ex, HttpContext);
            }
        }
    }
}ion");

                return ExceptionProblemDetails(ex, HttpContext);
            }
        }
    }
}