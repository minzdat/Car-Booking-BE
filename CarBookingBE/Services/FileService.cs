using CarBookingBE.Utils;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Web;
using CarBookingTest.Models;
using QRCoder;
using System.Drawing;
using IronPdf;
using System.Windows.Media.Animation;
using CarBookingBE.DTOs;
using System.Data.Entity;

namespace CarBookingBE.Services
{
    public class FileService
    {
        MyDbContext _db = new MyDbContext();
        UtilMethods _util = new UtilMethods();
        RequestService requestService = new RequestService();
        public Result<string> uploadAvatar(string currentId, HttpPostedFile postedFile)
        {
            try
            {
                var curId = Guid.Parse(currentId);
                string curUserId = curId.ToString().ToLower();
                string[] acceptExtensionImg = { ".png", ".jpg", ".jpeg" };
                if(currentId == null)
                {
                    return new Result<string>(false, "Missing user id of the avatar !");
                }
                if (postedFile == null || postedFile.FileName.Length == 0)
                {
                    return new Result<string>(false, "Missing file !", currentId);
                }
                if (!acceptExtensionImg.Contains(Path.GetExtension(postedFile.FileName)))
                {
                    return new Result<string>(false, "Not support file type ! Please provide image file(.png, .jpg, .jpeg)");
                }
                if (postedFile.ContentLength > (5 * 1024 * 1024))
                {
                    return new Result<string>(false, "The maximum size of file is 20MB !");
                }
                string pathToSave = Path.Combine(HttpContext.Current.Server.MapPath($"~/Files/Avatar"), curUserId);
                if (!Directory.Exists(pathToSave))
                {
                    Directory.CreateDirectory(pathToSave);
                }
                postedFile.SaveAs($"{pathToSave}/{postedFile.FileName}");
                return new Result<string>(true, "Upload file successfully !", $"Files/Avatar/{curUserId}/{postedFile.FileName}");
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<string>(false, "Internal error !");
            }
        }

        public Result<string> copyAvatarFromTemp(string userId, string fileName)
        {
            try
            {
                if(userId == null || fileName == null)
                {
                    return new Result<string>(false, "Missing parameter(s) !");
                }
                //need initial
                var userAvararFolder = HttpContext.Current.Server.MapPath($"~/Files/Avatar/{userId}");
                var destPath = $"{userAvararFolder}/{fileName}";
                if (!Directory.Exists(userAvararFolder))
                {
                    Directory.CreateDirectory(userAvararFolder);
                }
                var tempPath = HttpContext.Current.Server.MapPath($"~/Files/Avatar/temp/{fileName}");

                if (File.Exists(tempPath))
                {
                    if(!File.Exists(destPath))
                    {
                        File.Copy(tempPath, destPath);
                    }
                    File.Delete(tempPath);
                    return new Result<string>(true, "Finish uploading avatar !", $"Files/Avatar/{userId}/{fileName}");
                }
                return new Result<string>(false, "File by path does not exist !");
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<string>(false, "Internal error !");
            }
        }
        public Result<string> uploadAvatarTemp(HttpPostedFile postedFile)
        {
            try
            {
                string[] acceptExtensionImg = { ".png", ".jpg", ".jpeg" };
                if (postedFile == null || postedFile.FileName.Length == 0)
                {
                    return new Result<string>(false, "Missing file !");
                }
                if (!acceptExtensionImg.Contains(Path.GetExtension(postedFile.FileName)))
                {
                    return new Result<string>(false, "Not support file type ! Please provide image file(.png, .jpg, .jpeg)");
                }
                if (postedFile.ContentLength > (5 * 1024 * 1024))
                {
                    return new Result<string>(false, "The maximum size of file is 20MB !");
                }
                string pathToSave = HttpContext.Current.Server.MapPath($"~/Files/Avatar/temp");
                if (!Directory.Exists(pathToSave))
                {
                    Directory.CreateDirectory(pathToSave);
                }
                postedFile.SaveAs($"{pathToSave}/{postedFile.FileName}");
                return new Result<string>(true, "Upload file successfully !", $"{pathToSave}/{postedFile.FileName}");
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<string>(false, "Internal error !");
            }
        }
        public Result<string> uploadSignatureTemp(HttpPostedFile postedFile)
        {
            try
            {
                if (postedFile == null || postedFile.FileName.Length == 0)
                {
                    return new Result<string>(false, "Missing file !");
                }
                if (!postedFile.ContentType.StartsWith("image/"))
                {
                    return new Result<string>(false, "Not support file type ! Please provide image file (.png, .jpg, .jpeg, ...)");
                }
                if (postedFile.ContentLength > (5 * 1024 * 1024))
                {
                    return new Result<string>(false, "The maximum size of file is 20MB !");
                }
                string pathToSave = HttpContext.Current.Server.MapPath($"~/Files/Signature/temp");
                if (!Directory.Exists(pathToSave))
                {
                    Directory.CreateDirectory(pathToSave);
                }
                postedFile.SaveAs($"{pathToSave}/{postedFile.FileName}");
                return new Result<string>(true, "Upload file successfully !", $"{pathToSave}/{postedFile.FileName}");
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<string>(false, "Internal error !");
            }
        }
        public Result<string> copySignatureFromTemp(string userId, string fileName)
        {
            try
            {
                if (userId == null || fileName == null)
                {
                    return new Result<string>(false, "Missing parameter(s) !");
                }
                //need initial
                var userAvararFolder = HttpContext.Current.Server.MapPath($"~/Files/Signature/{userId}");
                var destPath = $"{userAvararFolder}/{fileName}";
                if (!Directory.Exists(userAvararFolder))
                {
                    Directory.CreateDirectory(userAvararFolder);
                }
                var tempPath = HttpContext.Current.Server.MapPath($"~/Files/Signature/temp/{fileName}");

                if (File.Exists(tempPath))
                {
                    if (!File.Exists(destPath))
                    {
                        File.Copy(tempPath, destPath);
                    }
                    File.Delete(tempPath);
                    var storePath = $"Files/Signature/{userId}/{fileName}";
                    /*var uid = Guid.Parse(userId);
                    var user = _db.Users.FirstOrDefault(u => u.Id == uid && u.IsDeleted == false);
                    if(user == null)
                    {
                        return new Result<string>(false, "User does not exist !");
                    }
                    user.AvatarPath = storePath;*/
                    return new Result<string>(true, "Finish uploading avatar !", storePath);
                }
                return new Result<string>(false, "File by path does not exist !");
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<string>(false, "Internal error !");
            }
        }
        public Result<HttpResponseMessage> writeToExcelAndDownload(Guid curId)
        {
            try
            {
                //all requests
                var lRequests = _db.Requests.Include(r => r.SenderUser).Where(r => r.IsDeleted == false).ToList();
                if(!lRequests.Any())
                {
                    return new Result<HttpResponseMessage>(false, "There's no data !");
                }

                // khởi tạo wb rỗng
                XSSFWorkbook wb = new XSSFWorkbook();

                // Tạo ra 1 sheet
                ISheet sheet = wb.CreateSheet();

                // Bắt đầu ghi lên sheet

                // Tạo row
                var row0 = sheet.CreateRow(0);
                // Merge lại row đầu 3 cột
                row0.CreateCell(0); // tạo ra cell trc khi merge
                CellRangeAddress cellMerge = new CellRangeAddress(0, 0, 0, 2);
                sheet.AddMergedRegion(cellMerge);
                row0.GetCell(0).SetCellValue("REPORT OF VEHICLES");

                // Ghi tên cột ở row 1
                var row1 = sheet.CreateRow(1);
                row1.CreateCell(0).SetCellValue("Current date");
                row1.CreateCell(1).SetCellValue("Pick date");
                row1.CreateCell(2).SetCellValue("Pick time");
                row1.CreateCell(3).SetCellValue("Rotation");
                row1.CreateCell(4).SetCellValue("Request ID");
                row1.CreateCell(5).SetCellValue("Status");
                row1.CreateCell(6).SetCellValue("Vehicle type");
                row1.CreateCell(7).SetCellValue("From date");
                row1.CreateCell(8).SetCellValue("To date");
                row1.CreateCell(9).SetCellValue("Usage time from");
                row1.CreateCell(10).SetCellValue("Usage time to");
                row1.CreateCell(11).SetCellValue("Requestor");
                row1.CreateCell(12).SetCellValue("Function");
                row1.CreateCell(13).SetCellValue("Cost center");
                row1.CreateCell(14).SetCellValue("Mobile");
                row1.CreateCell(15).SetCellValue("From location");
                row1.CreateCell(16).SetCellValue("To location");
                row1.CreateCell(17).SetCellValue("Driver");
                row1.CreateCell(18).SetCellValue("Mobile");
                row1.CreateCell(19).SetCellValue("Car plate");
                row1.CreateCell(20).SetCellValue("Note");

                // bắt đầu duyệt mảng và ghi tiếp tục
                int rowIndex = 2;
                foreach (var item in lRequests)
                {
                    // tao row mới
                    var newRow = sheet.CreateRow(rowIndex);
                    var vehicleRequest = _db.VehicleRequests.Include(v => v.User).Where(v => v.IsDeleted == false && v.RequestId == item.Id).FirstOrDefault();

                    if (vehicleRequest == null)
                    {
                        //return new Result<bool>(false, "Fetch data fail !");
                        vehicleRequest = new Models.VehicleRequest
                        {
                            User = new Account
                            {
                                FirstName = "",
                                LastName = "",
                            },
                            Type = true,
                            DriverCarplate = "",
                        };
                    }

                    // set giá trị
                    newRow.CreateCell(0).SetCellValue(string.Format("{0:dd-MM-yyyy}", item.Created));
                    newRow.CreateCell(1).SetCellValue(string.Format("{0:dd-MM-yyyy}", item.Created));
                    newRow.CreateCell(2).SetCellValue(string.Format("{0:dd-MM-yyyy}", item.PickTime));
                    newRow.CreateCell(3).SetCellValue(string.Format("{0:HH:mm}", item.PickTime));
                    newRow.CreateCell(4).SetCellValue(item.RequestCode);
                    newRow.CreateCell(5).SetCellValue(item.Status);
                    newRow.CreateCell(6).SetCellValue(vehicleRequest.Type == true ? "Company vehicle" : "Rented car, taxi");
                    newRow.CreateCell(7).SetCellValue(string.Format("{0:dd-MM-yyyy}", item.UsageFrom));
                    newRow.CreateCell(8).SetCellValue(string.Format("{0:dd-MM-yyyy}", item.UsageTo));
                    newRow.CreateCell(9).SetCellValue(string.Format("{0:HH:mm}", item.UsageFrom));
                    newRow.CreateCell(10).SetCellValue(string.Format("{0:HH:mm}", item.UsageTo));
                    newRow.CreateCell(11).SetCellValue(item.SenderUser.FirstName + " " + item.SenderUser.LastName);
                    newRow.CreateCell(12).SetCellValue(item.SenderUser.Function);
                    newRow.CreateCell(13).SetCellValue(item.CostCenter);
                    newRow.CreateCell(14).SetCellValue(item.Mobile);
                    newRow.CreateCell(15).SetCellValue(item.PickLocation);
                    newRow.CreateCell(16).SetCellValue(item.Destination);
                    newRow.CreateCell(17).SetCellValue(vehicleRequest.User.FirstName + " " + vehicleRequest.User.LastName);
                    newRow.CreateCell(18).SetCellValue(item.Mobile);
                    newRow.CreateCell(19).SetCellValue(vehicleRequest.DriverCarplate);
                    newRow.CreateCell(20).SetCellValue(item.Note);

                    // tăng index
                    rowIndex++;
                }

                // xong hết thì save file lại
                var fileName = "CarBooking_Export_" + DateTime.Now.ToString("ddMMyy");
                Trace.WriteLine(fileName);
                string pathToSave = Path.Combine(HttpContext.Current.Server.MapPath($"~/Files/Excel/{curId.ToString().ToLower()}"));
                if (!Directory.Exists(pathToSave))
                {
                    Directory.CreateDirectory(pathToSave);
                }
                //Create # CreateNew
                FileStream fs = new FileStream($"{pathToSave}/{fileName}.xlsx", FileMode.Create);
                wb.Write(fs);
                return new Result<HttpResponseMessage>(true, "Write to excel file successfully !", downloadFileExcel(curId.ToString().ToLower(), fileName));
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<HttpResponseMessage>(false, "Internal error !");
            }
        }

        public HttpResponseMessage downloadFileExcel(string id, string fileName)
        {
            try
            {
                var fileUrl = $"http://localhost:63642/Files/Excel/{id}/{fileName}.xlsx";
                Trace.WriteLine(fileUrl);
                using (WebClient client = new WebClient())
                {
                    byte[] fileData = client.DownloadData(fileUrl);

                    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Content = new ByteArrayContent(fileData);
                    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = fileName
                    };
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                    return response;
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        public Result<string> createQRCode(string link)
        {
            try
            {
                var curId = _util.getCurId();
                if(!curId.Success)
                {
                    return new Result<string>(false, "Current user not found !");
                }
                string id = curId.Data.ToString();
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(link, QRCodeGenerator.ECCLevel.Q);

                // Create the QR code
                QRCode qrCode = new QRCode(qrCodeData);

                // Convert the QR code to a bitmap image
                Bitmap qrCodeImage = qrCode.GetGraphic(20);

                string pathToSave = Path.Combine(HttpContext.Current.Server.MapPath($"~/Files/QRCode"));
                if (!Directory.Exists(pathToSave))
                {
                    Directory.CreateDirectory(pathToSave);
                }
                // Save the QR code image to a local file
                string imagePath = $"{pathToSave}/{id}.png";
                qrCodeImage.Save(imagePath, System.Drawing.Imaging.ImageFormat.Png);
                return new Result<string>(true, $"Files/QRCode/{id}.png");
            }
            catch(Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<string>(false, "Internal error !");
            }
        }

        public HttpResponseMessage writeDownPdf(string requestId)
        {
            try
            {
                var check = writeRequestToPdf(requestId);
                if (!check.Success)
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }
                return downloadFilePdf(requestId, check.Data);
            }
            catch(Exception e)
            {
                Trace.WriteLine(e.Message);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        public Result<string> writeRequestToPdf(string requestId)
        {

            try
            {
                if (requestId == null) return new Result<string>(false, "Missing id of current user !");
                var requestData = requestService.GetRequestById(requestId);
                if(!requestData.Success)
                {
                    return new Result<string>(false, "Request by id does not exist !");
                }
                var request = requestData.Data;
                string pathToSave = Path.Combine(HttpContext.Current.Server.MapPath($"~/Files/Pdf"));
                if(!Directory.Exists(pathToSave))
                {
                    Directory.CreateDirectory(pathToSave);
                }
                string pdfFilePath = Path.Combine(pathToSave, $"{request.RequestCode}.pdf");
                string htmlFilePath = Path.Combine(pathToSave, "minify.html");

                var result = createHtmlFromRequest(request, htmlFilePath);
                if (!result) return new Result<string>(false, "Html content error !");
                //string htmlContent = File.ReadAllText(htmlFilePath);

                // Create a PDF renderer
                var renderer = new ChromePdfRenderer();

                // Convert the HTML file to PDF
                var pdfDocument = renderer.RenderHtmlFileAsPdf(htmlFilePath);

                // Save the PDF to a file
                pdfDocument.SaveAs(pdfFilePath);

                downloadFilePdf(requestId, request.RequestCode);
                Trace.WriteLine("download finish");
                return new Result<string>(true, "write pdf ok !", request.RequestCode);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return new Result<string>(false, "write pdf fail !");
            }
        }

        public HttpResponseMessage downloadFilePdf(string id, string fileName)
        {
            try
            {
                /*var fileUrl = $"http://localhost:63642/Files/Pdf/{fileName}.pdf";
                using (WebClient client = new WebClient())
                {
                    byte[] fileData = client.DownloadData(fileUrl);

                    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Content = new ByteArrayContent(fileData);
                    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = fileName
                    };
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                }*/
                var filePath = HttpContext.Current.Server.MapPath($"~/Files/Pdf/{fileName}.pdf");
                byte[] fileContent = File.ReadAllBytes(filePath);

                // Set the Content-Disposition header to suggest a filename for the browser to use when saving the file
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new ByteArrayContent(fileContent);
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = $"{fileName}.pdf" // Replace "file.pdf" with the desired filename
                };

                // Set the appropriate content type for PDF files
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

                return response;
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        public bool createHtmlFromRequest(RequestDetailDTO r, string htmlFilePath)
        {
            if(r == null)
            {
                return false;
            }
            
            var host = "http://localhost:63642";
            string format = "dd/MM/yyyy hh:mm tt";
            var wf = _db.RequestWorkflows.Include(w => w.User).OrderBy(w => w.Level).Where(w => w.IsDeleted == false && w.RequestId == r.Id).ToList();
            var hasWf = wf.Any();
            var am = _db.RequestAttachments.Where(a => a.IsDeleted == false && a.RequestId == r.Id).ToList();
            var hasAm = am.Any();
            var cmt = _db.RequestComments.Include(c => c.Account).OrderBy(c => c.Created).Where(c => c.IsDeleted == false && c.RequestId == r.Id).ToList();
            var hasCmt = cmt.Any();

            /*var qrCode = createQRCode($"http://localhost:3000/setting/profile/{wf.UserId}");
            if (!qrCode.Success)
            {
                return false;
            }
            var qrPath = qrCode.Data;*/

            string documentSigners = "";
            if(hasWf)
            {
                documentSigners += @"
                    <!-- document signer -->
                    <div class=""title-cbr text-left m-top"">Document Signers</div>";
                foreach (var item in wf)
                {
                    
                    documentSigners += @"
                    <div class=""m-top"">
                        <div class=""full-line m-top""></div>
                        <div class=""flex-row align-center gap-12"">
                            <div class=""col-3"">
                                <div>
                                    <div class=""flex-row pt-8"">
                                        <div class=""title min-width"">Title</div>
                                        <div>" + (item.User.JobTitle != null ? item.User.JobTitle : "") + @"</div>
                                    </div>
                                </div>
                                <div class=""p-top"">
                                    <div class=""full-line-grey""></div>
                                    <div class=""flex-row pt-8"">
                                        <div class=""title min-width"">Email</div>
                                        <div>" + (item.User.Email != null ? item.User.Email : "") + @"</div>
                                    </div>
                                </div>
                                <div class=""p-top"">
                                    <div class=""full-line-grey""></div>
                                    <div class=""flex-row pt-8"">
                                        <div class=""title min-width"">Status</div>
                                        <div>APPROVED</div>
                                    </div>
                                </div>
                            </div>
                            <div style=""padding: 8px 8px 0 8px;"">
                                <img src=""https://drive.google.com/uc?export=view&id=1IKjLsNESdLfgbYFTDROq599dQd58ILlO"" alt=""qrCodeImage"" width=""120"" height=""120"">
                            </div>
                        </div>
                        <div class=""p-top"">
                            <div class=""full-line-grey""></div>
                            <div class=""flex-row pt-8"">
                                <div class=""title min-width"">Name</div>
                                <div>" + $"{(item.User.FirstName != null ? item.User.FirstName : "")} {(item.User.LastName != null ? item.User.LastName : "")}" + @"</div>
                            </div>
                        </div>
                    </div>
                ";
                }
            }

            string astring = "";
            if(hasAm)
            {
                foreach (var a in am)
                {
                    astring += @"
                    <div class=""flex-col m-top"">
                        <div class=""flex-row gap-36"">
                            <div class=""col-3"">" + r.Created?.ToString("ddd, dd MMM yyyy HH:mm:ss zzz") + @"</div>
                            <a class=""col-4"" href=" + $"{host}/{a.Path}" + @">" + $"{a.Path.Replace($"{host}/Files/Attachments/{r.RequestCode}", "")}" + @"</a>
                            <span class=""col-3"">"+ $"{(r.SenderUser.FirstName != null ? r.SenderUser.FirstName : "")} {(r.SenderUser.LastName != null ? r.SenderUser.LastName : "")}" + @"</span>
                        </div>
                    </div>
                ";
                }
            }

            string relatedDocument = @"
                <!-- related document -->
                <div>
                    <div class=""title-cbr text-left m-top"">Related document</div>
                    <div class=""full-line m-top""></div>"+ astring +@"
                </div>
            ";
            string discussionLog = "";
            if(hasCmt)
            {
                string c = "";
                foreach (var item in cmt)
                {
                    c += @"
                        <div class=""flex-col m-top"">
                            <div class=""flex-row gap-36"">
                                <div class=""col-3"">"+ item.Created.ToString("ddd, dd MMM yyyy HH:mm:ss zzz") + @"</div>
                                <div class=""col-4"">"+ item.Content +@"</div>
                                <span class=""col-3"">"+ $"{(item.Account.FirstName != null ? item.Account.FirstName : "")} {(item.Account.LastName != null ? item.Account.LastName : "")}" + @"</span>
                            </div>
                        </div>";
                }
                discussionLog = @"
                <!-- Discussion log -->
                <div>
                    <div class=""title text-left m-top"">Discussion log</div>
                    <div class=""full-line m-top""></div>
                    " + c +@"
                </div>
            ";
            }
            try
            {
                string htmlContent = @"
                <!DOCTYPE html>
                <html lang=""en"">
                <style>.title{font-weight:700}.title-cbr{font-weight:700;font-size:20px}.m-top{margin-top:20px}.p-top{padding-top:10px}.text-center{text-align:center}.text-left{text-align:left}.flex-row{display:flex}.flex-col{display:flex;flex-direction:column}.align-center{align-items:center}.gap-12{gap:12px}.gap-36{gap:36px}.line{height:1.2px;width:60%;background-color:#000}.full-line{height:2px;background-color:#000}.full-line-grey{height:1.2px;background-color:grey}.col-3{flex:1}.col-4{flex:2}.p-16{padding:0 16px}.pt-8{padding-top:8px}.note{font-size:14px;margin-top:20px;text-align:justify}.label-radio{font-size:18px}.height-30{height:30px}.min-width{min-width:152px}.radio{border-radius:50%;border:1px solid black;height:14px;width:14px;display:flex;justify-content:center;align-items:center;}.radio-check{border-radius:50%;border:1px solid black;height:6px;width:6px;background-color:black;}.gap-8{gap:4px;}</style>
                <body>
                    <div>
                        <img src=""https://drive.google.com/uc?export=view&id=1-ib3rZw6Dq_oaBMyJarHckgEfaIOmYjf"" alt=""logoOpus"">
                    </div>
                    <div class=""flex-row"" style=""justify-content: space-around;"">
                        <div class=""flex-col align-center gap-12"">
                            <span class=""title"">OPUS SOLUTION COMPANY</span>
                            <div class=""line""></div>
                            <span>No: "+ r.RequestCode + @"</span>
                        </div>
                        <div class=""flex-col align-center gap-12"">
                            <span class=""title"">Status: "+ r.Status +@"</span>
                            <div class=""line""></div>
                            <span>10/07/2023 15:48 PM</span>
                        </div>
                    </div>

                    <div class=""p-16"">
                        <div class=""title-cbr m-top text-center"">CAR BOOKING REQUEST</div>
                        <div class=""flex-col gap-12 m-top"">
                            <div class=""flex-row"">
                                <div class=""flex-col col-3"">
                                    <div class=""title"">Applicant</div>
                                    <span>"+ $"{r.SenderUser.FirstName} {r.SenderUser.LastName}" +@"</span>
                                </div>
                                <div class=""flex-col col-3"">
                                    <div class=""title"">Department</div>
                                    <span>"+ $"{r.Department.Name}" +@"</span>
                                </div>
                                <div class=""flex-col col-3"">
                                    <div class=""title"">User</div>
                                    <span>"+ $"{r.ReceiverUser.FirstName} {r.ReceiverUser.LastName}" +@"</span>
                                </div>
                                <div class=""flex-col col-3"">
                                    <div class=""title"">Mobile</div>
                                    <span>"+ $"{r.Mobile}" +@"</span>
                                </div>
                            </div>
                            <div class=""flex-row"">
                                <div class=""flex-col col-3"">
                                    <div class=""title"">Cost Center</div>
                                    <span>"+ $"{r.CostCenter}" +@"</span>
                                </div>
                                <div class=""flex-col col-3"">
                                    <div class=""title"">Total Passengers</div>
                                    <span>"+ $"{r.TotalPassengers}" +@"</span>
                                </div>
                                <div class=""flex-col col-3"">
                                    <div class=""title"">Usage time from</div>
                                    <span>"+ $"{r.UsageFrom?.ToString(format)} - {r.UsageTo?.ToString(format)}" +@"</span>
                                </div>
                                <div class=""flex-col col-3"">
                                    <div class=""title"">Pick time</div>
                                    <span>"+ $"{r.PickTime?.ToString(format)}" +@"</span>
                                </div>
                            </div>
                            <div class=""flex-row"">
                                <div class=""flex-col col-3"">
                                    <div class=""title"">Pick Location</div>
                                    <span>"+ $"{r.PickLocation}" +@"</span>
                                </div>
                                <div class=""flex-col col-3"">
                                    <div class=""title"">Destination</div>
                                    <span>"+ $"{r.Destination}" +@"</span>
                                </div>
                                <div class=""flex-col col-3"">
                                    <div class=""title"">Reason</div>
                                    <span>"+ $"{r.Reason}" +@"</span>
                                </div>
                                <div class=""flex-col col-3"">
                                    <div class=""title""></div>
                                    <span></span>
                                </div>
                            </div>
                        </div>
                        <p class='note'>Note: In case the Administration Department does not have enough vehicles to meet the department's vehicle dispatching requirements, the Department The Administration proposes to arrange alternative means of transportation (hire a car, or use a taxi card, Grab,...) and Costs will be accounted for according to the required department.</p>
                        <div class=""flex-row gap-12"">
                            <div class=""flex-row align-center height-30 gap-8"">
                                <div class=""radio"">
                                    <div class="+ (r.ApplyNote == true ? "radio-check" : "") + @"></div>
                                </div>
                                <label for=""yes"" class=""label-radio"">Yes</label>
                            </div>
                            <div class=""flex-row align-center height-30 gap-8"">
                                <div class=""radio"">
                                    <div class=" + (r.ApplyNote == false ? "radio-check" : "") + @"></div>
                                </div>
                                <label for=""no"" class=""label-radio"">No</label>
                            </div>
                        </div>"+ documentSigners + relatedDocument + discussionLog + @"
                    </div>
                </body>
                </html>";
                File.WriteAllText(htmlFilePath, htmlContent);
                return true;
            }
            catch(Exception e)
            {
                Trace.WriteLine(e.Message);
                return false;
            }
        }

        public Result<string> deleteAllFilesInFolder(string filePath)
        {
            try
            {
                if(filePath == null || filePath.Length == 0)
                {
                    return new Result<string>(false, "File path is empty !");
                }
                string pathToDel = HttpContext.Current.Server.MapPath($"~/{filePath}");
                if (Directory.Exists(pathToDel))
                {
                    string[] files = Directory.GetFiles(pathToDel);

                    foreach (string file in files)
                    {
                        File.Delete(file);
                    }

                    return new Result<string>(true, "Delete files successfully !");
                }
                return new Result<string>(false, "File path does not exist !");
            }
            catch(Exception e )
            {
                Trace.WriteLine(e.Message);
                return new Result<string>(false, "Internal error !");
            }
        }
    }
}