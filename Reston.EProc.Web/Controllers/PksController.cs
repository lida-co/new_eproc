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
    public class PksController : BaseController
    {
        private IPksRepo _repository;
        private IWorkflowRepository _workflowrepo;
        private IPengadaanRepo _repoPengadaan;
        private string DocumentType = "PKS";
        private string FILE_DOKUMEN_PKS_PATH = System.Configuration.ConfigurationManager.AppSettings["FILE_DOKUMEN_PKS_PATH"];

        private string FILE_REPORT_PATH = System.Configuration.ConfigurationManager.AppSettings["FILE_REPORT_PATH"];

        //fchr
        private bool ContainsHtml(string input)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(input, "<.*?>");
        }

        private bool ContainsDangerousInput(string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            // block karakter utama XSS
            return System.Text.RegularExpressions.Regex.IsMatch(input, @"[<>]");
        }

        //end fchr

        private static Logger _log = LogManager.GetCurrentClassLogger();

        public PksController()
        {
            _repository = new PksRepo(new AppDbContext());
            _repoPengadaan = new PengadaanRepo(new AppDbContext());
            _workflowrepo = new WorkflowRepository(new HelperContext());
        }

        public PksController(PksRepo repository)
        {
            _repository = repository;
        }

        [HttpPost]
        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_legal_admin)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public async Task< IHttpActionResult> List()
        {
            try
            {
                int start = Convert.ToInt32(System.Web.HttpContext.Current.Request["start"].ToString());
                string search = System.Web.HttpContext.Current.Request["search"].ToString();
                int length = Convert.ToInt32(System.Web.HttpContext.Current.Request["length"].ToString());
                string klasifikasi = System.Web.HttpContext.Current.Request["klasifikasi"].ToString();
                var data = _repository.List(search, start, length, klasifikasi);
                //data.data = data.data.Where(d => (long.Parse(d.TanggalSelesai.GetValueOrDefault(DateTime.Now).ToString("yyyyMMdd")) - long.Parse(DateTime.Now.ToString("yyyyMMdd"))) <= 180).ToList();

                //int olo = data.data.Count();

                foreach (var item in data.data)
                {
                    if (item.WorkflowId != null)
                    {
                        List<Reston.Helper.Model.ViewWorkflowModel> getDoc = _workflowrepo.ListDocumentWorkflow(UserId(), item.WorkflowId.Value, Reston.Helper.Model.DocumentStatus.PENGAJUAN, DocumentType, 0, 0);
                        if (getDoc.Where(d => d.CurrentUserId == UserId()).FirstOrDefault() != null) item.Approver = 1;
                        //item.lastApprover = _workflowrepo.isLastApprover(item.Id, item.WorkflowTemplateId.Value).Id;
                    }
                    Userx user = await userDetail(item.CreateBy.ToString());
                    if(user!=null)
                        item.CreatedName = user.Nama;
                }
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
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_legal_admin)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public IHttpActionResult ListPengadaan()
        {
            try
            {
                int start = Convert.ToInt32(System.Web.HttpContext.Current.Request["start"].ToString());
                string search = System.Web.HttpContext.Current.Request["search"].ToString();
                int length = Convert.ToInt32(System.Web.HttpContext.Current.Request["length"].ToString());
                string klasifikasi = System.Web.HttpContext.Current.Request["klasifikasi"].ToString();
                var data = _repository.ListPengadaan(search, start, length, klasifikasi);
                return Json(data);
            }
            catch (Exception ex)
            {
                return Json(new DataTablePksTemplate());
            }
        }


        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_legal_admin)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public IHttpActionResult Detail(Guid Id)
        {
            try
            {
                var oPks = _repository.detail(Id, UserId());
                DateTime uhuy = oPks.TanggalSelesai.GetValueOrDefault(DateTime.Now);
                long n = long.Parse(DateTime.Now.ToString("yyyyMMdd"));
                long i = long.Parse(uhuy.ToString("yyyyMMdd"));
                long lala = (i-n);
                if (oPks.WorkflowId != null)
                {
                    //oPks.Approver= _workflowrepo.isThisUserLastApprover(oPks.WorkflowId.Value, UserId());
                    try
                    {
                        //var ResultCurrentApprover = _workflowrepo.CurrentApproveUserSegOrder(Id);
                        //if (!string.IsNullOrEmpty(ResultCurrentApprover.Id))
                        //{
                        //    oPks.Approver =Convert.ToInt32(ResultCurrentApprover.Id.Split('#')[0]);
                        //}
                        //int isAprrover = UserId() == ApproverId ? 1 : 0;
                        List<Reston.Helper.Model.ViewWorkflowModel> getDoc = _workflowrepo.ListDocumentWorkflow(UserId(), oPks.WorkflowId.Value, Reston.Helper.Model.DocumentStatus.PENGAJUAN, DocumentType, 0, 0);
                        if (getDoc.Where(d => d.CurrentUserId == UserId()).FirstOrDefault() != null) oPks.Approver = 1;
                       
                    }
                    catch { }
                }
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
        public async Task<IHttpActionResult> ajukan(Guid Id)
        {
            HttpStatusCode respon = HttpStatusCode.NotFound;
            string message = "";
            string idx = "0";
            try
            {
                var pks = _repository.get(Id);
                var Legal = await listUser(IdLdapConstants.App.Roles.IdLdaplegal_admin);

                #region BuatAtauUpdateTamplate

                var WorkflowMasterTemplateDetails = new List<WorkflowMasterTemplateDetail>(){
                                //new WorkflowMasterTemplateDetail()
                                //    {
                                //        NameValue="Gen.By.System",
                                //        SegOrder=1,
                                //        UserId=pks.PemenangPengadaan.Pengadaan.PersonilPengadaans.Where(d=>d.tipe=="staff").FirstOrDefault().PersonilId
                                //    },
                                 new WorkflowMasterTemplateDetail()
                                    {
                                        NameValue="Gen.By.System",
                                        SegOrder=1,
                                        UserId=Legal[0]
                                    }
                            };
                WorkflowMasterTemplate MasterTemplate = new WorkflowMasterTemplate()
                {
                    ApprovalType = ApprovalType.BERTINGKAT,
                    CreateBy = UserId(),
                    CreateOn = DateTime.Now,
                    DescValue = "WorkFlow PKS Pengadaan=> " + pks.PemenangPengadaan.Pengadaan.Judul,
                    NameValue = "Generate By System ",
                    WorkflowMasterTemplateDetails = WorkflowMasterTemplateDetails
                };
                var resultTemplate = _workflowrepo.SaveWorkFlow(MasterTemplate, UserId());
                pks.WorkflowId = Convert.ToInt32(resultTemplate.Id);
                #endregion

                if (pks.WorkflowId != null)
                {
                    pks.StatusPks = StatusPks.Ajukan;
                    var savePks = _repository.save(pks, UserId());
                    var resultx = _workflowrepo.PengajuanDokumen(new Guid(savePks.Id), Convert.ToInt32(resultTemplate.Id), DocumentType);
                    if (string.IsNullOrEmpty(resultx.Id))
                    {
                        pks.StatusPks = StatusPks.Draft;
                        _repository.save(pks, UserId());
                        return Json(resultx);
                    }

                    _repository.AddRiwayatDokumenPks(new RiwayatDokumenPks()
                    {
                        ActionDate=DateTime.Now,
                        //Comment = Note,
                        Status="Pengajuan Dokumen Pks",
                        PksId=pks.Id,
                        UserId=UserId()
                    }, UserId());
                    return Json(new ResultMessage()
                    {
                        Id = resultx.Id,
                        message="Berhasil",
                        status=HttpStatusCode.OK
                    });

                    
                }
                else
                {
                    return Json(new ResultMessage() { 
                        message="GAGAL BUAT WORKFLOW APPROVAL",
                        status=HttpStatusCode.NotImplemented
                    });
                }

            }
            catch (Exception ex)
            {
                respon = HttpStatusCode.NotImplemented;
                message = ex.ToString();
                idx = "0";
            }
            return Json(new ResultMessage());
        }

        [HttpPost]
        [ApiAuthorize(
    IdLdapConstants.Roles.pRole_procurement_head,
    IdLdapConstants.Roles.pRole_procurement_staff,
    IdLdapConstants.Roles.pRole_procurement_end_user,
    IdLdapConstants.Roles.pRole_procurement_manager,
    IdLdapConstants.Roles.pRole_compliance)]
        public IHttpActionResult Save([FromBody] VWPks pks)
        {
            // 🔒 VALIDASI BASIC
            if (pks == null)
                return BadRequest("Request tidak valid");

            if (pks.Id == Guid.Empty)
                return BadRequest("Id tidak valid");

            if (string.IsNullOrWhiteSpace(pks.Title))
                return BadRequest("Title tidak boleh kosong");

            if (string.IsNullOrWhiteSpace(pks.Note))
                return BadRequest("Note tidak boleh kosong");

            // 🔒 ANTI XSS (block karakter HTML)
            if (ContainsDangerousInput(pks.Title) || ContainsDangerousInput(pks.Note))
                return BadRequest("Input mengandung karakter berbahaya");

            // 🔒 VALIDASI TANGGAL
            DateTime? tanggalMulai = null;
            DateTime? tanggalSelesai = null;

            if (!string.IsNullOrWhiteSpace(pks.TanggalMulaiStr))
            {
                if (!DateTime.TryParseExact(
                    pks.TanggalMulaiStr,
                    "dd/MM/yyyy",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out DateTime tglMulai))
                {
                    return BadRequest("Format Tanggal Mulai tidak valid");
                }
                tanggalMulai = tglMulai;
            }

            if (!string.IsNullOrWhiteSpace(pks.TanggalSelesaiStr))
            {
                if (!DateTime.TryParseExact(
                    pks.TanggalSelesaiStr,
                    "dd/MM/yyyy",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out DateTime tglSelesai))
                {
                    return BadRequest("Format Tanggal Selesai tidak valid");
                }
                tanggalSelesai = tglSelesai;
            }

            try
            {
                var ndata = new Pks
                {
                    Id = pks.Id,

                    // 🔐 OPSI 1: simpan RAW (pastikan frontend encode)
                    //Note = pks.Note,
                    //Title = pks.Title,

                    // 🔐 OPSI 2 (lebih aman):
                    Note = HttpUtility.HtmlEncode(pks.Note),
                    Title = HttpUtility.HtmlEncode(pks.Title),

                    PemenangPengadaanId = pks.PemenangPengadaanId,
                    TanggalMulai = tanggalMulai,
                    TanggalSelesai = tanggalSelesai
                };

                var result = _repository.save(ndata, UserId());

                return Ok(new
                {
                    message = "Sukses",
                    data = result
                });
            }
            catch
            {
                return InternalServerError();
            }
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_legal_admin)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public async Task<IHttpActionResult> UploadFile( Guid id,string tipe)
        {
            var oPks = _repository.get(id);
            TipeBerkas t = (TipeBerkas)Enum.Parse(typeof(TipeBerkas), tipe);
            if ( t == TipeBerkas.FinalLegalPks)
            {
                if (oPks.WorkflowId == null) return Json(new ResultMessage()
                {
                    status = HttpStatusCode.Forbidden,
                    message = Common.Forbiden()
                });
                //List<Reston.Helper.Model.ViewWorkflowModel> getDoc =
                //    _workflowrepo.ListDocumentWorkflow(UserId(), oPks.WorkflowId.Value, Reston.Helper.Model.DocumentStatus.PENGAJUAN, DocumentType, 0, 0);
              
                //if (getDoc.Where(d => d.CurrentUserId == UserId()).FirstOrDefault() == null )
                //    return Json(new ResultMessage()
                //    {
                //        status = HttpStatusCode.Forbidden,
                //        message = Common.Forbiden()
                //    });
                
            }
            if (t == TipeBerkas.DraftPKS && oPks.StatusPks==StatusPks.Approve)
            {
                return Json(new ResultMessage()
                {
                    status = HttpStatusCode.Forbidden,
                    message = Common.Forbiden()
                });

            }
            if (t == TipeBerkas.AssignedPks && oPks.CreateBy != UserId())
            {
                return Json(new ResultMessage()
                {
                    status = HttpStatusCode.Forbidden,
                    message = Common.Forbiden()
                });
            }


                var uploadPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            bool isSavedSuccessfully = true;
            string filePathSave = FILE_DOKUMEN_PKS_PATH;//+id ;
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
                filePathSave +=  newGuid.ToString() + "." + extension;
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
                    DokumenPks dokumen = new DokumenPks
                    {
                        File = fileName,
                        ContentType = contentType,
                        PksId = id,
                        SizeFile = sizeFile,
                        Tipe=t
                    };
                    return Json( _repository.saveDokumen(dokumen, UserId()).Id);
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
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_vendor, IdLdapConstants.Roles.pRole_legal_admin)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public List<VWDokumenPks> getDokumens( Guid Id,TipeBerkas tipe)
        {
            return _repository.GetListDokumenPks(Id,tipe);
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                           IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                            IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_vendor, IdLdapConstants.Roles.pRole_legal_admin)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public ResultMessage deleteDokumenPks(Guid Id)
        {
            try
            {
                int Approver = 0;
                var oPks = _repository.getDokPks(Id);
                if (oPks.Pks.WorkflowId != null)
                {
                    List<Reston.Helper.Model.ViewWorkflowModel> getDoc =
                        _workflowrepo.ListDocumentWorkflow(UserId(), oPks.Pks.WorkflowId.Value, Reston.Helper.Model.DocumentStatus.PENGAJUAN, DocumentType, 0, 0);
                    if (getDoc.Where(d => d.CurrentUserId == UserId()).FirstOrDefault() == null) Approver = 1;
                }
                var result = _repository.deleteDokumenPks(Id, UserId(), Approver);
                if (result == 1)
                {
                    return new ResultMessage()
                    {
                        status = HttpStatusCode.OK,
                        message =Common.DeleteSukses(),
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
            catch(Exception ex)
            {
                return new ResultMessage()
                {
                    status = HttpStatusCode.NotImplemented,
                    message =ex.ToString() ,
                    Id = "0"
                };
            }
        }


        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                          IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                           IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_vendor)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public ResultMessage delete(Guid Id)
        {
            return _repository.deletePks(Id, UserId());
        }


        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_legal_admin)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public IHttpActionResult Tolak(Guid Id,string Note)
        {
            var result = new Reston.Helper.Util.ResultMessageWorkflowState();
            try
            {               
                result = _workflowrepo.ApproveDokumen(Id, UserId(), Note, Reston.Helper.Model.WorkflowStatusState.REJECTED);
                if (result.data != null)
                {
                    if (result.data.DocumentStatus == Reston.Helper.Model.DocumentStatus.REJECTED)
                    {
                        ///_repository.ChangeStatusPersetujuanPemenang(Id, StatusPengajuanPemenang.REJECTED, UserId());
                        var resultx = _repository.TolakPks(Id, UserId());
                    }

                    _repository.AddRiwayatDokumenPks(new RiwayatDokumenPks()
                    {
                        ActionDate = DateTime.Now,
                        Comment = Note,
                        Status = "Dokumen Pks DiTolak Oleh: " + CurrentUser.UserName,
                        UserId = UserId()
                    }, UserId());
                }
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new Reston.Helper.Util.ResultMessageWorkflowState()
                {
                    status=HttpStatusCode.NotImplemented,
                    message=ex.ToString()
                });
            }
        }

        [HttpPost]
        [ApiAuthorize(
    IdLdapConstants.Roles.pRole_procurement_head,
    IdLdapConstants.Roles.pRole_procurement_staff,
    IdLdapConstants.Roles.pRole_procurement_end_user,
    IdLdapConstants.Roles.pRole_procurement_manager,
    IdLdapConstants.Roles.pRole_compliance,
    IdLdapConstants.Roles.pRole_legal_admin)]
        public IHttpActionResult Setujui([FromBody] SetujuiRequest req)
        {
            if (req == null)
                return BadRequest("Request tidak valid");

            if (req.Id == Guid.Empty)
                return BadRequest("Id tidak valid");

            // 🔐 Validasi XSS
            if (ContainsDangerousInput(req.Note) || ContainsDangerousInput(req.NoPks))
                return BadRequest("Input mengandung karakter berbahaya");

            try
            {
                var pks = _repository.get(req.Id);
                if (pks == null)
                    return NotFound();

                var result = _workflowrepo.ApproveDokumen(
                    req.Id,
                    UserId(),
                    "",
                    Reston.Helper.Model.WorkflowStatusState.APPROVED
                );

                if (!string.IsNullOrEmpty(result.Id))
                {
                    var nRiwayatDokumen = new RiwayatDokumenPks
                    {
                        Status = "Dokumen Pengadaan DiSetujui",
                        Comment = HttpUtility.HtmlEncode(req.Note),
                        PksId = req.Id,
                        UserId = UserId(),
                        ActionDate = DateTime.Now
                    };

                    _repository.AddRiwayatDokumenPks(nRiwayatDokumen, UserId());

                    var oViewWorkflowState = _workflowrepo.StatusDocument(req.Id, pks.WorkflowId.Value);

                    if (oViewWorkflowState.DocumentStatus == DocumentStatus.APPROVED)
                    {
                        _repository.SetujuiPks(req.Id, req.NoPks, UserId());
                    }
                }

                return Ok(new
                {
                    message = "Dokumen berhasil disetujui",
                    data = result
                });
            }
            catch
            {
                return InternalServerError();
            }
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public IHttpActionResult Edit(Guid Id)
        {
            
            return Json(_repository.ChangeStatus(Id,StatusPks.Draft, UserId()));
            
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_legal_admin)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public HttpResponseMessage OpenFile(Guid Id)
        {
            var data = _repository.getDokPks(Id);
            var path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + FILE_DOKUMEN_PKS_PATH + data.File;
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
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_legal_admin)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public IHttpActionResult Pending(Guid Id, string note)
        {
            var change = _repository.ChangeStatus(Id, StatusPks.Pending, UserId());
            if (!string.IsNullOrEmpty(change.Id))
            {
                _repository.AddRiwayatDokumenPks(new RiwayatDokumenPks()
                {
                    ActionDate = DateTime.Now,
                    Comment = note,
                    Status = "Dokumen Pendding",
                    PksId = Id,
                    UserId = UserId()
                }, UserId());
            }
            return Json(change); 

        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_legal_admin)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public async Task< IHttpActionResult> ListCatatan(Guid Id)
        {
            var data = _repository.ListCatatanPKs(Id);
            foreach(var item in data){
                var user = await userDetail(item.CreatedBy.ToString());
                if (user != null) item.Nama = user.Nama;
            }
            return Json(data);

        }

        [HttpPost]
        [ApiAuthorize( 
            IdLdapConstants.Roles.pRole_procurement_head
            , IdLdapConstants.Roles.pRole_procurement_staff
            , IdLdapConstants.Roles.pRole_procurement_end_user
            , IdLdapConstants.Roles.pRole_procurement_manager
            , IdLdapConstants.Roles.pRole_compliance
            , IdLdapConstants.Roles.pRole_legal_admin)]
        public IHttpActionResult SendNote([FromBody] SendNoteRequest req)
        {
            if (req == null)
                return BadRequest("Request tidak valid");

            if (req.Id == Guid.Empty)
                return BadRequest("Id tidak valid");

            if (string.IsNullOrWhiteSpace(req.note))
                return BadRequest("Note tidak boleh kosong");

            if (System.Text.RegularExpressions.Regex.IsMatch(req.note, @"[<>]"))
                return BadRequest("Input tidak valid (HTML tidak diperbolehkan)");

            try
            {
                var result = _repository.AddRiwayatDokumenPks(new RiwayatDokumenPks()
                {
                    ActionDate = DateTime.Now,
                    Comment = req.note, // atau HtmlEncode(req.note)
                    PksId = req.Id,
                    UserId = UserId(),
                    Status = "Catatan Pending"
                }, UserId());

                if (result?.Id != null)
                {
                    return Json(new ResultMessage()
                    {
                        Id = result.Id.ToString(),
                        message = Common.SaveSukses(),
                        status = HttpStatusCode.OK
                    });
                }

                return Json(new ResultMessage()
                {
                    message = "Gagal Save",
                    status = HttpStatusCode.NotModified
                });
            }
            catch
            {
                return InternalServerError();
            }
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                    IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                     IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        // Report PKS
        public HttpResponseMessage ReportPKS(string dari, string sampai)
        {
            try
            {
                LocalReport lr = new LocalReport();
                string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + FILE_REPORT_PATH;

                path = Path.Combine(path, "ReportPKS.rdlc");
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

                var PKS = _repository.GetReportPKS(oDari, oSampai, UserId());

                ReportDataSource rd = new ReportDataSource("PKS", PKS);
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
                    FileName = "Report-PKS" + UserId() + DateTime.Now.ToString("dd-MM-yy") + ".xls"
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

        [HttpPost]
        [ApiAuthorize( IdLdapConstants.Roles.pRole_legal_admin)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        //public async Task<IHttpActionResult> ceklegal()
        public IHttpActionResult ceklegal()
        {
            return Json(1);
        }


        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public IHttpActionResult SaveSetuju(VWPks pks)
        {
            try
            {
                var ndata = new Pks();
                ndata.Id = pks.Id;
                ndata.Note = pks.Note;
                ndata.Title = pks.Title;
                ndata.PemenangPengadaanId = pks.PemenangPengadaanId;

                if (!string.IsNullOrEmpty(pks.TanggalMulaiStr))
                {
                    try
                    {
                        ndata.TanggalMulai = Common.ConvertDate(pks.TanggalMulaiStr, "dd/MM/yyyy");
                    }
                    catch { }
                }
                if (!string.IsNullOrEmpty(pks.TanggalSelesaiStr))
                {
                    try
                    {
                        ndata.TanggalSelesai = Common.ConvertDate(pks.TanggalSelesaiStr, "dd/MM/yyyy");
                    }
                    catch { }
                }
                return Json(_repository.save(ndata, UserId()));
            }
            catch (Exception ex)
            {
                return Json(new VWPks());
            }
        }

        [HttpPost]
        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_staff)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        //public async Task<IHttpActionResult> ceklegal()
        public IHttpActionResult cekstaffproc()
        {
            return Json(1);
        }

        [HttpPost]
        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_legal_admin)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public async Task<IHttpActionResult> ListPerpanjangan()
        {
            try
            {
                var request = HttpContext.Current.Request;
                int start = int.Parse(request["start"]);
                int length = int.Parse(request["length"]);
                string search = request["search"] ?? "";
                string klasifikasi = request["klasifikasi"] ?? "";

                var data = _repository.List(search, start, length, klasifikasi);

                var nowInt = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
                data.data = data.data
                    .Where(d =>
                        (int.Parse(d.TanggalSelesai.GetValueOrDefault(DateTime.Now).ToString("yyyyMMdd")) - nowInt) <= 180 &&
                        d.CreateBy == UserId()
                    ).ToList();

                // ✅ Parallelize user lookup
                var tasks = data.data.Select(async item =>
                {
                    if (item.WorkflowId.HasValue)
                    {
                        var docs = _workflowrepo.ListDocumentWorkflow(UserId(), item.WorkflowId.Value, Reston.Helper.Model.DocumentStatus.PENGAJUAN, DocumentType, 0, 0);
                        if (docs.Any(d => d.CurrentUserId == UserId()))
                            item.Approver = 1;
                    }

                    var user = await userDetail(item.CreateBy.ToString());
                    if (user != null)
                        item.CreatedName = user.Nama;
                });

                await Task.WhenAll(tasks);

                return Json(data);
            }
            catch (Exception ex)
            {
                // Log error (opsional)
                return Json(new DataTablePksTemplate()); // kosongkan sebagai fallback
            }
        }

    }

}
