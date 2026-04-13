using Model.Helper;
using Reston.Eproc.Model.Monitoring.Entities;
using Reston.Eproc.Model.Monitoring.Model;
using Reston.Eproc.Model.Monitoring.Repository;
using Reston.Pinata.Model;
using Reston.Pinata.Model.Helper;
using Reston.Pinata.WebService;
using Reston.Pinata.WebService.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Diagnostics;
using System.IO;
using System.Net.Http.Headers;
using Microsoft.Reporting.WebForms;
using System.Reflection;

namespace Reston.EProc.Web.Controllers
{
    public class MonitoringSelectionController : BaseController
    {

        private string FILE_DOKUMEN_PATH = System.Configuration.ConfigurationManager.AppSettings["FILE_UPLOAD_DOCPRO"];
        private string FILE_REPORT_PATH = System.Configuration.ConfigurationManager.AppSettings["FILE_REPORT_PATH"];

        private IMoritoringRepo _repository;

        internal ResultMessage result = new ResultMessage();

        public MonitoringSelectionController()
        {
            _repository = new MonitoringRepo(new AppDbContext());
        }
        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public IHttpActionResult ListProyek()
        {
            string search = HttpContext.Current.Request["search"].ToString();
            int start = Convert.ToInt32(HttpContext.Current.Request["start"]);
            int length = Convert.ToInt32(HttpContext.Current.Request["length"]);
            string klasifikasi = HttpContext.Current.Request["klasifikasi"].ToString();
            if (klasifikasi == "")
            {
                return Json(_repository.GetDataListProyekMonitoring(search, start, length, null));
            }
            Klasifikasi dklasifikasi = (Klasifikasi)Convert.ToInt32(klasifikasi);
            return Json(_repository.GetDataListProyekMonitoring(search, start, length, dklasifikasi));
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public ResultMessage deletedokumen(Guid Id)
        {
            try
            {
                result = _repository.DeleteDokumenProyek(Id, UserId());
            }
            catch (Exception ex)
            {
                result.message = ex.ToString();
                result.status = HttpStatusCode.ExpectationFailed;
            }
            return result;
        }
        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_vendor)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public IHttpActionResult ListProyekDetailMonitoringPembayaran()
        {
            string search = HttpContext.Current.Request["search"].ToString();
            int start = Convert.ToInt32(HttpContext.Current.Request["start"]);
            int length = Convert.ToInt32(HttpContext.Current.Request["length"]);
            //Guid Id = Guid.Parse(HttpContext.Current.Request["Id"].ToString());
            Guid Id = Guid.Parse(HttpContext.Current.Request["Id"]);

            return Json(_repository.GetDataListProyekDetailMonitoringPembayaran(search, start, length, Id));
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_vendor)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public IHttpActionResult ListProyekRekanan()
        {
            string search = HttpContext.Current.Request["search"].ToString();
            int start = Convert.ToInt32(HttpContext.Current.Request["start"]);
            int length = Convert.ToInt32(HttpContext.Current.Request["length"]);
            string klasifikasi = HttpContext.Current.Request["klasifikasi"].ToString();

            if (klasifikasi == "")
            {
                return Json(_repository.GetDataListProyekMonitoringRekanan(search, start, length, null, UserId()));
            }

            Klasifikasi dklasifikasi = (Klasifikasi)Convert.ToInt32(klasifikasi);
            return Json(_repository.GetDataListProyekMonitoringRekanan(search, start, length, dklasifikasi, UserId()));
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_vendor)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public IHttpActionResult ListProyekDetailMonitoring()
        {
            string search = HttpContext.Current.Request["search"].ToString();
            int start = Convert.ToInt32(HttpContext.Current.Request["start"]);
            int length = Convert.ToInt32(HttpContext.Current.Request["length"]);
            //Guid Id = Guid.Parse(HttpContext.Current.Request["Id"].ToString());
            Guid Id = Guid.Parse(HttpContext.Current.Request["Id"]);
            string klasifikasi = HttpContext.Current.Request["klasifikasi"].ToString();

            if (klasifikasi == "")
            {
                return Json(_repository.GetDataListProyekDetailMonitoring(search, start, length, Id, null));
            }

            Klasifikasi dklasifikasi = (Klasifikasi)Convert.ToInt32(klasifikasi);
            return Json(_repository.GetDataListProyekDetailMonitoring(search, start, length, Id, dklasifikasi));
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_vendor)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public IHttpActionResult TampilJudulDetail()
        {
            Guid proyekId;
            var idValue = HttpContext.Current.Request["Id"];

            if (!Guid.TryParse(idValue, out proyekId))
            {
                return Json(new
                {
                    status = 400,
                    message = "Id tidak valid (harus GUID)"
                });
            }

            //return Json(_repository.GetCekLihatNilai(proyekId));

            //Guid ProyekId = Guid.Parse(HttpContext.Current.Request["Id"].ToString());

            return Json(_repository.GetDetailProyek(proyekId, UserId()));
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user, IdLdapConstants.Roles.pRole_procurement_vendor,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public IHttpActionResult TampilJudul()
        {
            return Json(_repository.GetResumeProyek());
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public IHttpActionResult List()
        {
            string search = HttpContext.Current.Request["search"].ToString();
            int start = Convert.ToInt32( HttpContext.Current.Request["start"]);
            int length = Convert.ToInt32( HttpContext.Current.Request["length"]);
            string status = HttpContext.Current.Request["status"].ToString();

            if (status == "")
            {
                return Json(_repository.GetDataMonitoringSelection(search, start, length, null));
            }

            StatusSeleksi dStatusSeleksi = (StatusSeleksi)Convert.ToInt32(status);
            return Json(_repository.GetDataMonitoringSelection(search, start, length, dStatusSeleksi));
        }


        //[HttpPost]
        //[ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
        //                                    IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
        //                                     IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        //[System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        //public IHttpActionResult GetList()
        //{
        //    try
        //    {
        //        int start = Convert.ToInt32(System.Web.HttpContext.Current.Request["start"].ToString());
        //        string search = System.Web.HttpContext.Current.Request["search"].ToString();
        //        int length = Convert.ToInt32(System.Web.HttpContext.Current.Request["length"].ToString());
        //        //string klasifikasi = System.Web.HttpContext.Current.Request["klasifikasi"].ToString();
        //        var data = _repository.GetDataTarif(search, start, length);
        //        //data.data = data.data.Where(d => d.PksId != null).ToList();
        //        return Json(data);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new DataTableTarif());
        //    }
        //}



        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public IHttpActionResult GetList()
        {
            try
            {
                int start = Convert.ToInt32(System.Web.HttpContext.Current.Request["start"].ToString());
                string title = System.Web.HttpContext.Current.Request["title"].ToString();
                int length = Convert.ToInt32(System.Web.HttpContext.Current.Request["length"].ToString());
                string type = System.Web.HttpContext.Current.Request["type"].ToString();
                //return Json(_repository.GetDataTarif(search, start, length));
                var data = _repository.GetDataTarif(title, start, length, type);

                // test
                //get the fucking user fulname

                //getUserFullName(data);
                //for (var i = 0; i < data.recordsTotal; i++)
                //{
                //    var usera = await userDetail(data.data[i].CreatedBy);
                //    data.data[i].CreatedBy = usera.FullName;
                //}

                return Json(data);
            }
            catch (Exception ex)
            {
                return Json(new DataTableTarif());
            }
            
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public IHttpActionResult GetListTambah(Guid documentId)
        {
            try
            {
                //int start = Convert.ToInt32(System.Web.HttpContext.Current.Request["start"].ToString());
                //string search = System.Web.HttpContext.Current.Request["search"].ToString();
                //int length = Convert.ToInt32(System.Web.HttpContext.Current.Request["length"].ToString());
                var data = _repository.GetDataTarifTambah(documentId);//search, start, length);
                return Json(data);
            }
            catch (Exception ex)
            {
                return Json(new DataTableBenefit());
            }

        }






       
        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public ResultMessage delete(int Id)
        {
            try
            {
                result = _repository.deleteTarifTemplate(Id, UserId());
            }
            catch (Exception ex)
            {
                result.message = ex.ToString();
                result.status = HttpStatusCode.ExpectationFailed;
            }
            return result;
        }
        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public ResultMessage deleteEach(int Id)
        {
            try
            {
                result = _repository.deleteBenefitRate(Id, UserId());
            }
            catch (Exception ex)
            {
                result.message = ex.ToString();
                result.status = HttpStatusCode.ExpectationFailed;
            }
            return result;
        }



        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public IHttpActionResult ListSedangBerjalan()
        {
            string search = HttpContext.Current.Request["search"].ToString();
            int start = Convert.ToInt32(HttpContext.Current.Request["start"]);
            int length = Convert.ToInt32(HttpContext.Current.Request["length"]);
            string status = HttpContext.Current.Request["status"].ToString();

            if (status == "")
            {
                return Json(_repository.GetDataMonitoringSelectionSedangBerjalan(search, start, length, null));
            }

            StatusSeleksi dStatusSeleksi = (StatusSeleksi)Convert.ToInt32(status);
            return Json(_repository.GetDataMonitoringSelectionSedangBerjalan(search, start, length, dStatusSeleksi));
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public IHttpActionResult ListSelesai()
        {
            string search = HttpContext.Current.Request["search"].ToString();
            int start = Convert.ToInt32(HttpContext.Current.Request["start"]);
            int length = Convert.ToInt32(HttpContext.Current.Request["length"]);
            string status = HttpContext.Current.Request["status"].ToString();

            if (status == "")
            {
                return Json(_repository.GetDataMonitoringSelectionSelesai(search, start, length, null));
            }

            StatusSeleksi dStatusSeleksi = (StatusSeleksi)Convert.ToInt32(status);
            return Json(_repository.GetDataMonitoringSelectionSelesai(search, start, length, dStatusSeleksi));
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public IHttpActionResult ListDraf()
        {
            string search = HttpContext.Current.Request["search"].ToString();
            int start = Convert.ToInt32(HttpContext.Current.Request["start"]);
            int length = Convert.ToInt32(HttpContext.Current.Request["length"]);
            string status = HttpContext.Current.Request["status"].ToString();

            if (status == "")
            {
                return Json(_repository.GetDataMonitoringSelectionDraf(search, start, length, null));
            }

            StatusSeleksi dStatusSeleksi = (StatusSeleksi)Convert.ToInt32(status);
            return Json(_repository.GetDataMonitoringSelectionDraf(search, start, length, dStatusSeleksi));
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public ResultMessage toFinish()
        {
            var idValue = HttpContext.Current.Request["aIdProyek"];
            var status = HttpContext.Current.Request["aStatus"];

            if (string.IsNullOrEmpty(idValue))
            {
                return new ResultMessage
                {
                    status = HttpStatusCode.BadRequest,
                    message = "aIdProyek tidak boleh kosong"
                };
            }

            Guid xProyekId;
            if (!Guid.TryParse(idValue, out xProyekId))
            {
                return new ResultMessage
                {
                    status = HttpStatusCode.BadRequest,
                    message = "Format aIdProyek tidak valid (harus GUID)"
                };
            }

            return _repository.toFinishRepo(xProyekId, status, UserId());
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public ResultMessage TidakMonitor(Guid Id)
        {
            Guid spkid = Id;
            string status = "Tidak Dimonitor";

            return _repository.toTidakDimonitor(spkid, status, UserId());
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public ResultMessage toDisable()
        {
            Guid xProyekId = Guid.Parse(HttpContext.Current.Request["aIdProyek"].ToString());
            string xStatus = HttpContext.Current.Request["aStatus"].ToString();

            return _repository.toDisableRepo(xProyekId, xStatus, UserId());
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public ResultMessage toEnable()
        {
            Guid xProyekId = Guid.Parse(HttpContext.Current.Request["aIdProyek"].ToString());
            string xStatus = HttpContext.Current.Request["aStatus"].ToString();

            return _repository.toEnableRepo(xProyekId, xStatus, UserId());
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public ResultMessage Add()
        {
            Guid PengadaanId = Guid.Parse(HttpContext.Current.Request["aPengadaanId"].ToString());
            string StatusSeleksi = HttpContext.Current.Request["aStatusSeleksi"].ToString();
            string StatusMonitoring = HttpContext.Current.Request["aStatusMonitoring"].ToString();

            StatusSeleksi dStatusSeleksi = (StatusSeleksi)Convert.ToInt32(StatusSeleksi);
            StatusMonitored dStatusMonitoring = (StatusMonitored)Convert.ToInt32(StatusMonitoring);

            return _repository.Save(PengadaanId, dStatusMonitoring, dStatusSeleksi,UserId());
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_vendor)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]

        public ResultMessage UpdateProgressPekerjaan(List<TahapanProyek> Tahapan)
        {
            try
            {
                result = _repository.SimpanProgresPekerjaan(Tahapan, UserId());
            }
            catch (Exception ex)
            {
                result.message = ex.ToString();
                result.status = HttpStatusCode.ExpectationFailed;
            }
            return result;
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_vendor)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]

        public ResultMessage UpdateProgressPembayaran(List<TahapanProyek> Tahapan)
        {
            try
            {
                result = _repository.SimpanProgresPembayaran(Tahapan, UserId());
            }
            catch (Exception ex)
            {
                result.message = ex.ToString();
                result.status = HttpStatusCode.ExpectationFailed;
            }

            return result;
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public HttpResponseMessage OpenFile(Guid Id)
        {
            DokumenProyek d = _repository.GetDokumenProyek(Id);
            var path = FILE_DOKUMEN_PATH + d.URL;
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new FileStream(path, FileMode.Open);
            result.Content = new StreamContent(stream);

            result.Content.Headers.ContentType = new MediaTypeHeaderValue(d.ContentType);

            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = d.URL
            };

            return result;
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance,IdLdapConstants.Roles.pRole_procurement_vendor)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public async Task<IHttpActionResult> UploadFile()
        {
            var httpRequest = HttpContext.Current.Request;

            // 🔥 ambil parameter dulu dengan aman
            var idValue = httpRequest["Id"];
            var namaDokumen = httpRequest["NamaDokumen"];

            if (string.IsNullOrEmpty(idValue))
                return Json(new { success = false, message = "Id kosong" });

            if (!Guid.TryParse(idValue, out Guid dokumenId))
                return Json(new { success = false, message = "Format Id tidak valid" });

            if (string.IsNullOrEmpty(namaDokumen))
                namaDokumen = "Dokumen";

            if (!Request.Content.IsMimeMultipartContent())
                return Content(HttpStatusCode.UnsupportedMediaType, "Bukan multipart");

            string root = HttpContext.Current.Server.MapPath("~/UploadDokumenMonitor");

            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);

            var provider = new MultipartMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(provider);

            string namaFileSave = "";
            string contentType = "";
            long sizeFile = 0;

            foreach (var file in provider.Contents)
            {
                var fileName = file.Headers.ContentDisposition.FileName?.Trim('\"');

                if (string.IsNullOrEmpty(fileName))
                    continue;

                var extension = Path.GetExtension(fileName); // 🔥 lebih aman
                byte[] buffer = await file.ReadAsByteArrayAsync();

                contentType = file.Headers.ContentType?.ToString();
                sizeFile = buffer.Length;

                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");

                // 🔥 sanitize nama file
                string safeNamaDokumen = namaDokumen.Replace(" ", "_");

                namaFileSave = $"Dokumen{dokumenId}-{safeNamaDokumen}-{timestamp}{extension}";

                var fullPath = Path.Combine(root, namaFileSave);

                try
                {
                    File.WriteAllBytes(fullPath, buffer);
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            }

            if (string.IsNullOrEmpty(namaFileSave))
            {
                return Json(new { success = false, message = "File tidak ditemukan" });
            }

            try
            {
                var result = _repository.saveDokumenProyeks(dokumenId, namaFileSave, contentType, UserId());

                return Json(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        // Report Monitoring
        public HttpResponseMessage ReportMonitoring(string dari, string sampai)
        {
            try
            {
                LocalReport lr = new LocalReport();
                string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + FILE_REPORT_PATH;

                path = Path.Combine(path, "ReportMonitoring.rdlc");
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

                var Monitoring = _repository.GetReportMonitoring(oDari, oSampai, UserId());

                ReportDataSource rd = new ReportDataSource("Monitoring", Monitoring);
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
                    FileName = "Report-Monitoring" + UserId() + DateTime.Now.ToString("dd-MM-yy") + ".xls"
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
        // Report Pekerjaan
        public HttpResponseMessage ReportPekerjaan(Guid Id)
        {
            try
            {
                LocalReport lr = new LocalReport();
                string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + FILE_REPORT_PATH;

                path = Path.Combine(path, "ReportPekerjaan.rdlc");
                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }

                else
                {
                    //return View("Index");
                }
                var id = Id;

                var Pekerjaan = _repository.GetReportPekerjaan(id, UserId());
                int i = 0;
                foreach (var item in Pekerjaan)
                {
                    if (i != 0)
                    {
                        item.Pengadaan = "";
                    }
                    i = 1;
                }

                ReportDataSource rd = new ReportDataSource("Pekerjaan", Pekerjaan);
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
                    FileName = "Report-Progress-Pelaksanaan-Project" + UserId() + DateTime.Now.ToString("dd-MM-yy") + ".xls"
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
        // Report Pembayaran
        public HttpResponseMessage ReportPembayaran(Guid Id)
        {
            try
            {
                LocalReport lr = new LocalReport();
                string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + FILE_REPORT_PATH;

                path = Path.Combine(path, "ReportPembayaran.rdlc");
                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }

                else
                {
                    //return View("Index");
                }
                var id = Id;

                var Pembayaran = _repository.GetReportPembayaran(id, UserId());
                int i = 0;
                foreach (var item in Pembayaran)
                {
                    if (i != 0)
                    {
                        item.Pengadaan = "";
                    }
                    i = 1;
                }

                ReportDataSource rd = new ReportDataSource("Pembayaran", Pembayaran);
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
                    FileName = "Report-Pembayaran-Project" + UserId() + DateTime.Now.ToString("dd-MM-yy") + ".xls"
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
        // Report Penilaian Vendor
        public HttpResponseMessage ReportPenilaian(Guid Id)
        {
            try
            {
                LocalReport lr = new LocalReport();
                string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + FILE_REPORT_PATH;

                path = Path.Combine(path, "ReportPenilaianVendor.rdlc");
                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }

                else
                {
                    //return View("Index");
                }
                var id = Id;

                var PenilaianVendor = _repository.GetReportPenilaianVendor(id, UserId());

                int i = 0;
                foreach (var item in PenilaianVendor)
                {
                    if (i != 0)
                    {
                        item.Judul = "";
                        item.Vendor = "";
                    }
                    i = 1;
                }

                ReportDataSource rd = new ReportDataSource("PenilaianVendor", PenilaianVendor);
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
                    FileName = "Report-Nilai-Kinerja-Vendor" + UserId() + DateTime.Now.ToString("dd-MM-yy") + ".xls"
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
        // Report Penilaian Vendor
        public HttpResponseMessage ReportPenilaianDirect(string NoSPK)
        {
            try
            {
                LocalReport lr = new LocalReport();
                string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + FILE_REPORT_PATH;

                path = Path.Combine(path, "ReportPenilaianVendor.rdlc");
                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }

                else
                {
                    //return View("Index");
                }
                var nospk = NoSPK;

                var PenilaianVendor = _repository.GetReportPenilaianVendorDirect(nospk, UserId());

                int i = 0;
                foreach (var item in PenilaianVendor)
                {
                    if (i != 0)
                    {
                        item.Judul = "";
                        item.Vendor = "";
                    }
                    i = 1;
                }

                ReportDataSource rd = new ReportDataSource("PenilaianVendor", PenilaianVendor);
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
                    FileName = "Report-Nilai-Kinerja-Vendor" + UserId() + DateTime.Now.ToString("dd-MM-yy") + ".xls"
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

        [ApiAuthorize]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_master_user, IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public IHttpActionResult ListPengadaan()
        {
            string search = HttpContext.Current.Request["search"].ToString();
            int start = Convert.ToInt32(HttpContext.Current.Request["start"]);
            int length = Convert.ToInt32(HttpContext.Current.Request["length"]);
            return Json(_repository.GetDataListPengadaan(search, start, length));
        }
        
        [HttpGet]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_master_user, IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        public IHttpActionResult GetEditPIC(Guid id)
        {
            var templateDoc = _repository.GetDataPIC(id).Select(x => new ViewMonitoringSelection()
            {
                Id = x.Id,
                Judul = x.Judul,
                NoPengadaan = x.NoPengadaan,
                PIC = x.PersonilPengadaans.Where(dd => dd.tipe == "pic").FirstOrDefault() == null ? "" : x.PersonilPengadaans.Where(dd => dd.tipe == "pic").FirstOrDefault().Nama
            }).FirstOrDefault();

            return Json(templateDoc);
        }
    }
}
