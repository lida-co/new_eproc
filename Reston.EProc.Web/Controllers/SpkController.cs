using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Model.Helper;
using Newtonsoft.Json;
using Reston.Pinata.Model;
//using Reston.Pinata.Model.Helper;
using Reston.Pinata.Model.JimbisModel;
using Reston.Pinata.Model.PengadaanRepository;
using Reston.Pinata.Model.PengadaanRepository.View;
using Reston.Pinata.WebService.Helper;
using Reston.Pinata.WebService.ViewModels;
using Webservice.Helper.Util;
using Reston.Helper.Repository;
using Reston.Helper;
using Reston.Helper.Util;
using Reston.Helper.Model;
using System.Web;
using Microsoft.Reporting.WebForms;
using System.Reflection;
using NLog;

namespace Reston.Pinata.WebService.Controllers
{
    public class SpkController : BaseController
    {
        private ISpkRepo _repository;
        private IWorkflowRepository _workflowrepo;
        private IPengadaanRepo _repoPengadaan;
        private string DocumentType = "SPK";
        private string FILE_DOKUMEN_SPK_PATH = System.Configuration.ConfigurationManager.AppSettings["FILE_DOKUMEN_SPK_PATH"];
        private string FILE_DOKUMEN_PATH = System.Configuration.ConfigurationManager.AppSettings["FILE_UPLOAD_DOC"];
        private string FILE_REPORT_PATH = System.Configuration.ConfigurationManager.AppSettings["FILE_REPORT_PATH"];
        private static Logger _log = LogManager.GetCurrentClassLogger();
        public SpkController()
        {
            _repository = new SpkRepo(new AppDbContext());
            _repoPengadaan = new PengadaanRepo(new AppDbContext());
            _workflowrepo = new WorkflowRepository(new HelperContext());
        }

        public SpkController(SpkRepo repository)
        {
            _repository = repository;
        }

        [HttpPost]
        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public IHttpActionResult ListSPK()
        {
            try
            {
                int start = Convert.ToInt32(System.Web.HttpContext.Current.Request["start"].ToString());
                string search = System.Web.HttpContext.Current.Request["search"].ToString();
                int length = Convert.ToInt32(System.Web.HttpContext.Current.Request["length"].ToString());
                string klasifikasi = System.Web.HttpContext.Current.Request["klasifikasi"].ToString();
                var data = _repository.List(search, start, length, klasifikasi);
                //data.data = data.data.Where(d => d.PengadaanId != null).ToList();
                return Json(data);
            }
            catch (Exception ex)
            {
                return Json(new DataTablePksTemplate());
            }
        }

        [HttpPost]
        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public IHttpActionResult ListPks()
        {
            try
            {
                int start = Convert.ToInt32(System.Web.HttpContext.Current.Request["start"].ToString());
                string search = System.Web.HttpContext.Current.Request["search"].ToString();
                int length = Convert.ToInt32(System.Web.HttpContext.Current.Request["length"].ToString());
                string klasifikasi = System.Web.HttpContext.Current.Request["klasifikasi"].ToString();
                var data = _repository.ListPks(search, start, length, klasifikasi);
                return Json(data);
            }
            catch (Exception ex)
            {
                return Json(new DataTableSpkTemplate());
            }
        }


        [HttpPost]
        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public IHttpActionResult Detail(Guid Id)
        {
            try
            {
                var oPks = _repository.detail(Id, UserId());
                return Json(oPks);
            }
            catch (Exception ex)
            {
                return Json(new VWPks());
            }
        }


        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public IHttpActionResult Save(VWSpk vspk)
        {
            try
            {
                var spk = new Spk();
                spk.PksId = vspk.PksId;
                spk.Id = vspk.Id;
                spk.NilaiSPK = vspk.NilaiSPK;
                if (!string.IsNullOrEmpty(vspk.TanggalSPKStr)) spk.TanggalSPK = Common.ConvertDate(vspk.TanggalSPKStr, "dd/MM/yyyy HH:mm");
                return Json(_repository.save(spk, UserId()));
            }
            catch (Exception ex)
            {
                return Json(new VWSpk());
            }
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public async Task<IHttpActionResult> UploadFile(Guid id)
        {
            var oSpk = _repository.get(id);

            if (oSpk.StatusSpk != StatusSpk.Draft)
                return Json("00000000-0000-0000-0000-000000000000");

            var uploadPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            bool isSavedSuccessfully = true;
            string filePathSave = FILE_DOKUMEN_SPK_PATH;//+id ;
            string fileName = "";

            var s = await Request.Content.ReadAsStreamAsync();
            var provider = new MultipartMemoryStreamProvider();

            await Request.Content.ReadAsMultipartAsync(provider);
            string contentType = "";
            Guid newGuid = Guid.NewGuid();
            long sizeFile = 0;
            foreach (var file in provider.Contents)
            {
                string filename = file.Headers.ContentDisposition.FileName.Trim('\"');
                string extension = filename.Substring(filename.IndexOf(".") + 1, filename.Length - filename.IndexOf(".") - 1);
                byte[] buffer = await file.ReadAsByteArrayAsync();
                contentType = file.Headers.ContentType.ToString();
                sizeFile = buffer.Length;
                filePathSave += newGuid.ToString() + "." + extension;
                fileName += newGuid.ToString() + "." + extension;
                // var uploadPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase; //new PhysicalFileSystem(@"..\Reston.Pinata\WebService\Upload\Vendor\Dokumen\");

                try
                {
                    FileStream fs = new FileStream(uploadPath.ToString() + filePathSave, FileMode.CreateNew);
                    await fs.WriteAsync(buffer, 0, buffer.Length);

                    fs.Close();

                    isSavedSuccessfully = true;
                }
                catch (Exception e)
                {
                    return InternalServerError();
                }
            }
            Guid DokumenId = Guid.NewGuid();


            if (isSavedSuccessfully)
            {
                try
                {
                    DokumenSpk dokumen = new DokumenSpk
                    {
                        File = fileName,
                        ContentType = contentType,
                        SpkId = id,
                        SizeFile = sizeFile
                    };
                    return Json(_repository.saveDokumen(dokumen, UserId()).Id);
                }
                catch (Exception ex)
                {
                    return Json("00000000-0000-0000-0000-000000000000");
                }
            }
            return Json("00000000-0000-0000-0000-000000000000");
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_vendor)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public List<VWDokumenSPK> getDokumens(Guid Id)
        {
            return _repository.GetListDokumenSpk(Id);
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                         IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                          IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_vendor)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public ResultMessage deleteDokumenPks(Guid Id)
        {
            try
            {
                int Approver = 0;
                var oSpk = _repository.getDokSpk(Id);
                var result = _repository.deleteDokumenSpk(Id, UserId(), Approver);
                if (result == 1)
                {
                    return new ResultMessage()
                    {
                        status = HttpStatusCode.OK,
                        message = Common.DeleteSukses(),
                        Id = "1"
                    };
                }
                else
                {
                    return new ResultMessage()
                    {
                        status = HttpStatusCode.NotImplemented,
                        message = "error",
                        Id = "0"
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResultMessage()
                {
                    status = HttpStatusCode.NotImplemented,
                    message = ex.ToString(),
                    Id = "0"
                };
            }
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                         IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                          IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public IHttpActionResult ChangeSatus(Guid Id, StatusSpk status)
        {
            _repository.ChangeStatus(Id, status, UserId());
            _repository.AddRiwayatDokumenSpk(new RiwayatDokumenSpk()
            {
                ActionDate = DateTime.Now,
                Status = "Berubah Status: " + status.ToString()
            }, UserId());
            return Json(_repository.ChangeStatus(Id, status, UserId()));

        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                       IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                        IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_vendor)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public ResultMessage delete(Guid Id)
        {
            return _repository.deleteSpk(Id, UserId());
        }

        [AcceptVerbs("GET", "POST", "HEAD")]
        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                       IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                        IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        public HttpResponseMessage OpenFile(Guid Id)
        {
            var data = _repository.getDokSpk(Id);
            var path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + FILE_DOKUMEN_SPK_PATH + data.File;
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new FileStream(path, FileMode.Open);
            result.Content = new StreamContent(stream);
            //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentType = new MediaTypeHeaderValue(data.ContentType);

            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = data.File
            };

            return result;
        }

        // Report SPK
        public HttpResponseMessage ReportSPK(string dari, string sampai, string divisi)
        {
            try
            {
                LocalReport lr = new LocalReport();
                string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + FILE_REPORT_PATH;

                path = Path.Combine(path, "ReportSPK.rdlc");
                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }

                else
                {
                    //return View("Index");
                }
                var oDari = Common.ConvertDate(dari, "dd/MM/yyyy");
                var oSampai = Common.ConvertDate(sampai, "dd/MM/yyyy");
                string oDivisi = divisi;

                var SPK = _repository.GetReportSPK(oDari, oSampai, UserId(), oDivisi);

                ReportDataSource rd = new ReportDataSource("SPK", SPK);
                lr.DataSources.Add(rd);
                string param1 = "";
                string filename = "";
                string param2 = "";
                string paramSemester = "";
                string paramTahunAjaran = "";


                string reportType = "doc";
                string mimeType;
                string encoding;
                string fileNameExtension;


                string[] streamids = null;
                String extension = null;
                Byte[] bytes = null;
                Warning[] warnings;

                bytes = lr.Render("Excel", null, out mimeType, out encoding, out extension, out streamids, out warnings);

                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                Stream stream = new MemoryStream(bytes);

                result.Content = new StreamContent(stream);

                //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");

                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = "Report-SPK" + UserId() + DateTime.Now.ToString("dd-MM-yy") + ".xls"
                };

                return result;
            }
            catch (ReflectionTypeLoadException ex)
            {
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                StringBuilder sb = new StringBuilder();
                foreach (Exception exSub in ex.LoaderExceptions)
                {
                    sb.AppendLine(exSub.Message);
                    FileNotFoundException exFileNotFound = exSub as FileNotFoundException;
                    if (exFileNotFound != null)
                    {
                        if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                        {
                            sb.AppendLine("Fusion Log:");
                            sb.AppendLine(exFileNotFound.FusionLog);
                        }
                    }
                    sb.AppendLine();
                }
                result.Content = new StringContent(sb.ToString());

                return result;
                //Display or log the error based on your application.
            }
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                    IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                     IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public HttpResponseMessage ReportSPK2(string dari, string sampai)
        {
            try
            {
                LocalReport lr = new LocalReport();
                string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + FILE_REPORT_PATH;

                path = Path.Combine(path, "ReportSPK.rdlc");
                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }

                else
                {
                    //return View("Index");
                }
                var oDari = Common.ConvertDate(dari, "dd/MM/yyyy");
                var oSampai = Common.ConvertDate(sampai, "dd/MM/yyyy");

                var SPK = _repository.GetReportSPK2(oDari, oSampai, UserId());

                ReportDataSource rd = new ReportDataSource("SPK", SPK);
                lr.DataSources.Add(rd);
                string param1 = "";
                string filename = "";
                string param2 = "";
                string paramSemester = "";
                string paramTahunAjaran = "";


                string reportType = "doc";
                string mimeType;
                string encoding;
                string fileNameExtension;


                string[] streamids = null;
                String extension = null;
                Byte[] bytes = null;
                Warning[] warnings;

                bytes = lr.Render("Excel", null, out mimeType, out encoding, out extension, out streamids, out warnings);

                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                Stream stream = new MemoryStream(bytes);

                result.Content = new StreamContent(stream);

                //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");

                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = "Report-SPK" + UserId() + DateTime.Now.ToString("dd-MM-yy") + ".xls"
                };

                return result;
            }
            catch (ReflectionTypeLoadException ex)
            {
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                StringBuilder sb = new StringBuilder();
                foreach (Exception exSub in ex.LoaderExceptions)
                {
                    sb.AppendLine(exSub.Message);
                    FileNotFoundException exFileNotFound = exSub as FileNotFoundException;
                    if (exFileNotFound != null)
                    {
                        if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                        {
                            sb.AppendLine("Fusion Log:");
                            sb.AppendLine(exFileNotFound.FusionLog);
                        }
                    }
                    sb.AppendLine();
                }
                result.Content = new StringContent(sb.ToString());

                return result;
                //Display or log the error based on your application.
            }
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                          IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                           IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public IHttpActionResult SaveNonPKS(VWSpk vspk)
        {
            try
            {
                if (vspk.TanggalSPKStr.Equals("Invalid date")) {
                    vspk.TanggalSPKStr = null;
                }
                //Vendor vendor = new Vendor();
                int vendorId = 0;
                var spk = new Spk() {
                    Id = vspk.Id,
                    NilaiSPK = vspk.NilaiSPK,
                    Title = vspk.Judul
                };
                vendorId = vspk.VendorIdNonPKS;
            //int IdNonReg = 0;

            //if (vspk.VendorIdNonPKS == 0) {
            //    vendor.Nama = vspk.VendorNonReg;
            //    vendor.TipeVendor = ETipeVendor.NON_REGISTER;
            //    IdNonReg = _repository.AddVendorNonReg(vendor);
            //};
            //spk.PksId = vspk.PksId;

            //if (vspk.VendorIdNonPKS == 0) { vendorId = IdNonReg; } else { vendorId = vspk.VendorIdNonPKS; }

            if (!string.IsNullOrEmpty(vspk.TanggalSPKStr)) spk.TanggalSPK = Common.ConvertDate(vspk.TanggalSPKStr, "dd/MM/yyyy HH:mm");
                return Json(_repository.savenonpks(spk, vendorId, UserId()));
                //return Json(_repository.savenonpks(spk, UserId()));
            }
            catch (Exception ex)
            {
                return Json(new VWSpk());
            }
        }

        [HttpPost]
        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                          IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                           IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public IHttpActionResult DetailNonPKS(Guid Id)
        {
            try
            {
                var oPks = _repository.detailnonpks(Id, UserId());
                return Json(oPks);
            }
            catch (Exception ex)
            {
                return Json(new VWPks());
            }
        }

        [HttpPost]
        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                          IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                           IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public IHttpActionResult ListSPKNonPKS()
        {
            try
            {
                int start = Convert.ToInt32(System.Web.HttpContext.Current.Request["start"].ToString());
                string search = System.Web.HttpContext.Current.Request["search"].ToString();
                int length = Convert.ToInt32(System.Web.HttpContext.Current.Request["length"].ToString());
                string klasifikasi = System.Web.HttpContext.Current.Request["klasifikasi"].ToString();
                var data = _repository.ListNonPKS(search, start, length, klasifikasi);
                data.data = data.data.Where(d => d.PengadaanId == null).ToList();

                return Json(data);
            }
            catch (Exception ex)
            {
                return Json(new DataTablePksTemplate());
            }
        }

        public HttpResponseMessage ReportSPKNONPKS(string dari, string sampai, string divisi)
        {
            try
            {
                LocalReport lr = new LocalReport();
                string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + FILE_REPORT_PATH;

                path = Path.Combine(path, "ReportSPKNONPKS.rdlc");
                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }

                else
                {
                    //return View("Index");
                }
                var oDari = Common.ConvertDate(dari, "dd/MM/yyyy");
                var oSampai = Common.ConvertDate(sampai, "dd/MM/yyyy");
                string oDivisi = divisi;

                var SPK = _repository.ReportSPKNONPKS(oDari, oSampai, UserId(), oDivisi);

                ReportDataSource rd = new ReportDataSource("SPK", SPK);
                lr.DataSources.Add(rd);
                string param1 = "";
                string filename = "";
                string param2 = "";
                string paramSemester = "";
                string paramTahunAjaran = "";


                string reportType = "doc";
                string mimeType;
                string encoding;
                string fileNameExtension;


                string[] streamids = null;
                String extension = null;
                Byte[] bytes = null;
                Warning[] warnings;

                bytes = lr.Render("Excel", null, out mimeType, out encoding, out extension, out streamids, out warnings);

                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                Stream stream = new MemoryStream(bytes);

                result.Content = new StreamContent(stream);

                //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");

                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = "Report-SPK" + UserId() + DateTime.Now.ToString("dd-MM-yy") + ".xls"
                };

                return result;
            }
            catch (ReflectionTypeLoadException ex)
            {
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                StringBuilder sb = new StringBuilder();
                foreach (Exception exSub in ex.LoaderExceptions)
                {
                    sb.AppendLine(exSub.Message);
                    FileNotFoundException exFileNotFound = exSub as FileNotFoundException;
                    if (exFileNotFound != null)
                    {
                        if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                        {
                            sb.AppendLine("Fusion Log:");
                            sb.AppendLine(exFileNotFound.FusionLog);
                        }
                    }
                    sb.AppendLine();
                }
                result.Content = new StringContent(sb.ToString());

                return result;
                //Display or log the error based on your application.
            }
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                    IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                     IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public HttpResponseMessage ReportSPKNONPKS2(string dari, string sampai)
        {
            try
            {
                LocalReport lr = new LocalReport();
                string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + FILE_REPORT_PATH;

                path = Path.Combine(path, "ReportSPKNONPKS.rdlc");
                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }

                else
                {
                    //return View("Index");
                }
                var oDari = Common.ConvertDate(dari, "dd/MM/yyyy");
                var oSampai = Common.ConvertDate(sampai, "dd/MM/yyyy");

                var SPK = _repository.ReportSPKNONPKS2(oDari, oSampai, UserId());

                ReportDataSource rd = new ReportDataSource("SPK", SPK);
                lr.DataSources.Add(rd);
                string param1 = "";
                string filename = "";
                string param2 = "";
                string paramSemester = "";
                string paramTahunAjaran = "";


                string reportType = "doc";
                string mimeType;
                string encoding;
                string fileNameExtension;


                string[] streamids = null;
                String extension = null;
                Byte[] bytes = null;
                Warning[] warnings;

                bytes = lr.Render("Excel", null, out mimeType, out encoding, out extension, out streamids, out warnings);

                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                Stream stream = new MemoryStream(bytes);

                result.Content = new StreamContent(stream);

                //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");

                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = "Report-SPK" + UserId() + DateTime.Now.ToString("dd-MM-yy") + ".xls"
                };

                return result;
            }
            catch (ReflectionTypeLoadException ex)
            {
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                StringBuilder sb = new StringBuilder();
                foreach (Exception exSub in ex.LoaderExceptions)
                {
                    sb.AppendLine(exSub.Message);
                    FileNotFoundException exFileNotFound = exSub as FileNotFoundException;
                    if (exFileNotFound != null)
                    {
                        if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                        {
                            sb.AppendLine("Fusion Log:");
                            sb.AppendLine(exFileNotFound.FusionLog);
                        }
                    }
                    sb.AppendLine();
                }
                result.Content = new StringContent(sb.ToString());

                return result;
                //Display or log the error based on your application.
            }
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public async Task<IHttpActionResult> UploadFileNonPks(string klasifikasi, Guid id)
        {
            var oSpk = _repository.get(id);
            _log.Debug("var oSpk = {0}", oSpk);

            if (oSpk.StatusSpk != StatusSpk.Draft)
                return Json("00000000-0000-0000-0000-000000000000");

            var uploadPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            _log.Debug("var uploadPath = {0}", uploadPath);
            bool isSavedSuccessfully = true;
            _log.Debug("var isSavedSuccessfully = {0}", isSavedSuccessfully);
            string filePathSave = FILE_DOKUMEN_SPK_PATH;//+id ;
            _log.Debug("var filePathSave = {0}", filePathSave);
            string fileName = "";

            var s = await Request.Content.ReadAsStreamAsync();
            var provider = new MultipartMemoryStreamProvider();

            await Request.Content.ReadAsMultipartAsync(provider);
            string contentType = "";
            Guid newGuid = Guid.NewGuid();
            long sizeFile = 0;
            foreach (var file in provider.Contents)
            {
                string filename = file.Headers.ContentDisposition.FileName.Trim('\"');
                string extension = filename.Substring(filename.IndexOf(".") + 1, filename.Length - filename.IndexOf(".") - 1);
                byte[] buffer = await file.ReadAsByteArrayAsync();
                contentType = file.Headers.ContentType.ToString();
                sizeFile = buffer.Length;
                filePathSave += newGuid.ToString() + "." + extension;
                _log.Debug("var filePathSave = {0}", filePathSave);
                fileName += newGuid.ToString() + "." + extension;
                _log.Debug("var fileName = {0}", fileName);
                // var uploadPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase; //new PhysicalFileSystem(@"..\Reston.Pinata\WebService\Upload\Vendor\Dokumen\");

                try
                {
                    FileStream fs = new FileStream(uploadPath.ToString() + filePathSave, FileMode.CreateNew);
                    await fs.WriteAsync(buffer, 0, buffer.Length);

                    fs.Close();

                    isSavedSuccessfully = true;
                    _log.Debug("var isSavedSuccessfully1 = {0}", isSavedSuccessfully);
                }
                catch (Exception e)
                {
                    return InternalServerError();
                }
            }
            Guid DokumenId = Guid.NewGuid();


            if (isSavedSuccessfully)
            {
                try
                {
                    AppDbContext ctx = new AppDbContext();
                    DokumenSpkNonPks dokumen = new DokumenSpkNonPks
                    {
                        File = fileName,
                        ContentType = contentType,
                        SpkId = id,
                        SizeFile = sizeFile,
                        Klasifikasi = klasifikasi
                    };
                    _log.Debug("var dokumen = {0}", dokumen.File);
                    _log.Debug("var ContentType = {0}", dokumen.ContentType);
                    _log.Debug("var SpkId = {0}", dokumen.SpkId);
                    _log.Debug("var SizeFile = {0}", dokumen.SizeFile);
                    _log.Debug("var Klasifikasi = {0}", dokumen.Klasifikasi);
                    //return Json(_repository.saveDokumenSpkNonPks(dokumen, UserId()).Id); _log.Debug("var dokumenSpkNonPks.SpkId = {0}", dokumenSpkNonPks.SpkId);
                    var oData = ctx.Spk.Find(dokumen.SpkId);
                    _log.Debug("var oData = {0}", oData);
                    if (oData == null) return Json("00000000-0000-0000-0000-000000000000"); ;
                    dokumen.CreateOn = DateTime.Now;
                    _log.Debug("var dokumenSpkNonPks.CreateOn = {0}", dokumen.CreateOn);
                    dokumen.CreateBy = UserId();
                    _log.Debug("var dokumenSpkNonPks.CreateBy = {0}", dokumen.CreateBy);
                    ctx.DokumenSpkNonPks.Add(dokumen);
                    ctx.SaveChanges();
                    return Json(dokumen.SpkId);
                }
                catch (Exception ex)
                {
                    return Json("00000000-0000-0000-0000-000000000000");
                }
            }
            return Json("00000000-0000-0000-0000-000000000000");
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_vendor)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public List<VWDokumenSPK> getDokumensNonPks(string klasifikasi, Guid id)
        {
            return _repository.GetListDokumenSpkNonPks(klasifikasi,id);
        }

        [AcceptVerbs("GET", "POST", "HEAD")]
        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                       IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                        IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        public HttpResponseMessage OpenFileNonPks(Guid Id, string klasifikasi)
        {
            var data = _repository.getDokSpkNonPks(Id,klasifikasi);
            var path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + FILE_DOKUMEN_SPK_PATH + data.File;
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new FileStream(path, FileMode.Open);
            result.Content = new StreamContent(stream);
            //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentType = new MediaTypeHeaderValue(data.ContentType);

            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = data.File
            };

            return result;
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                         IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                          IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_vendor)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public ResultMessage deleteDokumenSpkNonPks(Guid Id)
        {
            try
            {
                int Approver = 0;
                var oSpk = _repository.getDokSpk(Id);
                var result = _repository.deleteDokumenSpkNonPks(Id, UserId(), Approver);
                if (result == 1)
                {
                    return new ResultMessage()
                    {
                        status = HttpStatusCode.OK,
                        message = Common.DeleteSukses(),
                        Id = "1"
                    };
                }
                else
                {
                    return new ResultMessage()
                    {
                        status = HttpStatusCode.NotImplemented,
                        message = "error",
                        Id = "0"
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResultMessage()
                {
                    status = HttpStatusCode.NotImplemented,
                    message = ex.ToString(),
                    Id = "0"
                };
            }
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                       IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                        IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_vendor)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public ResultMessage SimpanAtributDokumenNonPks(Guid Id, string klasifikasi, string NoDok, string TglDokString)
        {
            DokumenSpkNonPks hold = new DokumenSpkNonPks();
            if (!string.IsNullOrEmpty(TglDokString)) hold.TglDokumen = Common.ConvertDate(TglDokString, "dd/MM/yyyy HH:mm");
            return _repository.saveAtributDokumenNonPks(Id, klasifikasi, NoDok, hold.TglDokumen);
        }

        [HttpPost]
        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                          IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                           IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public IHttpActionResult DetailDokNonPKS(Guid Id)
        {
            try
            {
                var oPks = _repository.detaildoknonpks(Id, UserId());
                return Json(oPks);
            }
            catch (Exception ex)
            {
                return Json(new VWPks());
            }
        }

    }
    
}
