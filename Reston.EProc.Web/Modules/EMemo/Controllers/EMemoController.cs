using Model.Helper;
using NLog;
using Reston.Eproc.Model.EMemo;
//using Reston.EProc.Web.ViewModels;
using Reston.Helper.Util;
using Reston.Pinata.Model;
using Reston.Pinata.WebService;
using Reston.Pinata.WebService.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Reston.EProc.Web.Base.Attributes;
using Reston.EProc.Web.Base.ViewModels;
using Reston.EProc.Web.Modules.EMemo.ViewModels;
using Reston.EProc.Web.Modules.EMemo.Services;
using Reston.Pinata.Model.PengadaanRepository.View;
using System.Diagnostics;
using Reston.Pinata.Model.PengadaanRepository;

namespace Reston.EProc.Web.Modules.EMemo.Controllers
{
    [RequestHandler]
    public class EMemoController : BaseController
    {
        private static Logger _log = LogManager.GetCurrentClassLogger();

        private AppDbContext db;

        private EMemoService eMemoService;

        private IPengadaanRepo _repository;

        public EMemoController()
        {
            db = new AppDbContext();
            eMemoService = new EMemoService(db);
        }

        [HttpPost]
        public IHttpActionResult Search([FromBody] EMemoRequest request)
        {
            //Console.WriteLine("aaaa" + request);

            var response = eMemoService.Search(request, UserId());
           


            var status = response.Header?.StatusCode;
            if (status != Base.ViewModels.StatusCode.SUCCESS)
            {
                if (status == Base.ViewModels.StatusCode.BAD_REQUEST)
                    return BadRequest(response.Header?.StatusMessage);
                else
                    return InternalServerError(new Exception(response.Header?.StatusMessage));
            }
            else
            {
                return Json(response);
            }

        }


        [HttpGet]
        public IHttpActionResult Get(string docId)
        {
            if (string.IsNullOrEmpty(docId) || !Guid.TryParse(docId, out Guid result))
                return BadRequest();

            var request = new EMemoRequest()
            {
                Header = new BaseHeader()
                {
                    UserId = UserId().ToString()
                },
                Detail = new EMemoRequestDetail()
                {
                    Id = Guid.Parse(docId)
                }
            };

            var response = eMemoService.Get(request, UserId());

            var status = response.Header?.StatusCode;
            if (status != Base.ViewModels.StatusCode.SUCCESS)
            {
                if (status == Base.ViewModels.StatusCode.BAD_REQUEST)
                    return BadRequest(response.Header?.StatusMessage);
                else if (status == Base.ViewModels.StatusCode.NOT_FOUND)
                    return NotFound();
                else
                    return InternalServerError(new Exception(response.Header?.StatusMessage));
            }
            else
            {
                return Json(response);
            }
        }

        [HttpPost]
        public IHttpActionResult Create([FromBody] EMemoRequest request)
        {
            var response = eMemoService.Create(request);

            var status = response.Header?.StatusCode;
            if (status != Base.ViewModels.StatusCode.SUCCESS)
            {
                if (status == Base.ViewModels.StatusCode.BAD_REQUEST)
                    return BadRequest(response.Header?.StatusMessage);
                else
                    return InternalServerError(new Exception(response.Header?.StatusMessage));
            }
            else
            {
                return Json(response);
            }
        }

        [HttpPost]
        public IHttpActionResult Update([FromBody] EMemoRequest request)
        {

            if (request.Detail.Id == null || Guid.Empty.Equals(request.Detail.Id))
                return BadRequest("Update request should carry a valid entity identifier.");

            var response = eMemoService.Update(request);

            var status = response.Header?.StatusCode;
            if (status != Base.ViewModels.StatusCode.SUCCESS)
            {
                if (status == Base.ViewModels.StatusCode.BAD_REQUEST)
                    return BadRequest(response.Header?.StatusMessage);
                else if (status == Base.ViewModels.StatusCode.NOT_FOUND)
                    return NotFound();
                else
                    return InternalServerError(new Exception(response.Header?.StatusMessage));
            }
            else
            {
                return Json(response);
            }
        }

        [HttpPost]
        public IHttpActionResult CreateApprovalWorkflow([FromBody] EMemoApprovalWorkflowRequest request)
        {
            var response = eMemoService.CreateApprovalWorkflow(request);

            var status = response.Header?.StatusCode;
            if (status != Base.ViewModels.StatusCode.SUCCESS)
            {
                if (status == Base.ViewModels.StatusCode.BAD_REQUEST)
                    return BadRequest(response.Header?.StatusMessage);
                else if (status == Base.ViewModels.StatusCode.NOT_FOUND)
                    return NotFound();
                else
                    return InternalServerError(new Exception(response.Header?.StatusMessage));
            }
            else
            {
                return Json(response);
            }
        }

        [HttpGet]
        public IHttpActionResult GetApprovalWorkflowDetail(string docId)
        {
            if (string.IsNullOrEmpty(docId) || !Guid.TryParse(docId, out Guid result))
                return BadRequest();

            var request = new EMemoApprovalWorkflowDetailRequest()
            {
                Header = new BaseHeader()
                {
                    UserId = UserId().ToString()
                },
                Detail = new EMemoApprovalWorkflowDetailRequestDetail()
                {
                    Id = docId,
                }
            };

            var response = eMemoService.GetApprovalWorkflowDetails(request);

            var status = response.Header?.StatusCode;
            if (status != Base.ViewModels.StatusCode.SUCCESS)
            {
                if (status == Base.ViewModels.StatusCode.BAD_REQUEST)
                    return BadRequest(response.Header?.StatusMessage);
                else if (status == Base.ViewModels.StatusCode.NOT_FOUND)
                    return NotFound();
                else
                    return InternalServerError(new Exception(response.Header?.StatusMessage));
            }
            else
            {
                return Json(response);
            }
        }

        [HttpPost]
        public IHttpActionResult SearchApprovalWorkflowDetails([FromBody] EMemoApprovalWorkflowDetailRequest request)
        {
            var response = eMemoService.SearchApprovalWorkflowDetails(request);

            var status = response.Header?.StatusCode;
            if (status != Base.ViewModels.StatusCode.SUCCESS)
            {
                if (status == Base.ViewModels.StatusCode.BAD_REQUEST)
                    return BadRequest(response.Header?.StatusMessage);
                else if (status == Base.ViewModels.StatusCode.NOT_FOUND)
                    return NotFound();
                else
                    return InternalServerError(new Exception(response.Header?.StatusMessage));
            }
            else
            {
                return Json(response);
            }
        }

        [HttpPost]
        public IHttpActionResult UpdateApprovalWorkflowDetail([FromBody] EMemoApprovalWorkflowDetailRequest request)
        {
            var response = eMemoService.UpdateApprovalWorkflowDetail(request);

            var status = response.Header?.StatusCode;
            if (status != Base.ViewModels.StatusCode.SUCCESS)
            {
                if (status == Base.ViewModels.StatusCode.BAD_REQUEST)
                    return BadRequest(response.Header?.StatusMessage);
                else if (status == Base.ViewModels.StatusCode.NOT_FOUND)
                    return NotFound();
                else
                    return InternalServerError(new Exception(response.Header?.StatusMessage));
            }
            else
            {
                return Json(response);
            }

        }

        [HttpPost]
        public IHttpActionResult DocumentViewer([FromBody] EMemoDocumentViewerRequest request)
        {
            try
            {
                var reqDetail = request.Detail;
                if (reqDetail == null)
                    return BadRequest("Invalid request");

                byte[] data = Convert.FromBase64String(reqDetail.ContentDataBase64);

                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new ByteArrayContent(data);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(reqDetail.ContentType);
                ContentDispositionHeaderValue contentDisposition = new ContentDispositionHeaderValue("attachment");
                contentDisposition.FileName = reqDetail.ContentTitle;
                response.Content.Headers.ContentDisposition = contentDisposition;

                return ResponseMessage(response);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        //public async Task<HttpResponseMessage> getUsers(int? start, int? limit, string search)
        public IHttpActionResult SearchPersonels(int? start, int? limit, string search)
        {
            //var client = new HttpClient();
            //HttpResponseMessage reply = await client.GetAsync(
            //        string.Format("{0}/{1}", IdLdapConstants.IDM.Url, "admin/ListUser?start=" + start + "&limit=" + limit + "&filter=&name=" + name));//+IdLdapConstants.Roles.pRole_procurement_staff+
            //return reply;

            int offset = start.HasValue ? start.Value : 0;
            int take = limit.HasValue ? limit.Value : 10;


            var query = from userAccount in db.VUserAccounts
                        where !db.Vendors.Any(vendor => vendor.Owner.ToString() == userAccount.Id)
                        select userAccount;

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(userAccount => userAccount.DisplayName != null
                            && userAccount.DisplayName.Contains(search.Trim()));
            }

            var result = query
                .OrderBy(userAccount => userAccount.DisplayName)
                .Skip(offset)
                .Take(take)
                .ToList();

            var response = new
            {
                Header = new
                {
                    StatusCode = Reston.EProc.Web.Base.ViewModels.StatusCode.SUCCESS
                },
                Details = result
            };

            return Json(response);

        }

        [HttpPost]
        public IHttpActionResult SearchEMemoTemplates([FromBody] EMemoTemplateRequest request)
        {
            var response = eMemoService.SearchEMemoTemplates(request);

            var status = response.Header?.StatusCode;
            if (status != Base.ViewModels.StatusCode.SUCCESS)
            {
                if (status == Base.ViewModels.StatusCode.BAD_REQUEST)
                    return BadRequest(response.Header?.StatusMessage);
                else if (status == Base.ViewModels.StatusCode.NOT_FOUND)
                    return NotFound();
                else
                    return InternalServerError(new Exception(response.Header?.StatusMessage));
            }
            else
            {
                return Json(response);
            }
        }

        [HttpGet]
        public IHttpActionResult GetEMemoTemplate(string docId)
        {
            if (string.IsNullOrEmpty(docId) || !Guid.TryParse(docId, out Guid result))
                return BadRequest();

            var request = new EMemoTemplateRequest()
            {
                Header = new BaseHeader()
                {
                    UserId = UserId().ToString()
                },
                Detail = new EMemoTemplateRequestDetail()
                {
                    Id = Guid.Parse(docId)
                }
            };

            var response = eMemoService.GetEMemoTemplate(request);

            var status = response.Header?.StatusCode;
            if (status != Base.ViewModels.StatusCode.SUCCESS)
            {
                if (status == Base.ViewModels.StatusCode.BAD_REQUEST)
                    return BadRequest(response.Header?.StatusMessage);
                else if (status == Base.ViewModels.StatusCode.NOT_FOUND)
                    return NotFound();
                else
                    return InternalServerError(new Exception(response.Header?.StatusMessage));
            }
            else
            {
                return Json(response);
            }
        }

        [HttpGet]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_admin })]
        public IHttpActionResult getWorkUnits([FromUri] string sel)
        {

            var result = (from reff in db.ReferenceDatas
                          where reff.Qualifier.Contains(RefDataQualifier.UnitKerja)
                          select new
                          {
                              Value = reff.Code,
                              Label = reff.LocalizedName,
                              IsSelected = reff.Code.Equals(sel, StringComparison.OrdinalIgnoreCase)
                          }).ToList();

            return Json(result);

        }

        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        [HttpGet]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_admin })]
        public IHttpActionResult riwayatDokumen([FromUri] Guid Id)
        {

            var userx = new Userx();


            var result = (from log in db.EMemoLogs
                          where log.EMemoId == Id
                          select new
                          {
                              CreateDate = log.CreateDate,
                              Content = log.Content,
                              AttactmentPenawaran = log.AttactmentPenawaran,
                              AttachtmentAnalisa = log.AttachtmentAnalisa,
                              AttachmentLampiran = log.AttachmentLampiran,
                              AttactmentPenawaranType = log.AttactmentPenawaranType,
                              AttachtmentAnalisaType = log.AttachtmentAnalisaType,
                              AttachmentLampiranType = log.AttachmentLampiranType,
                              AttachtmentAnalisaFile = log.AttachtmentAnalisaFile,
                              AttachmentLampiranFile = log.AttachmentLampiranFile,
                              AttactmentPenawaranFiles = log.AttactmentPenawaranFiles
                          }).ToList();

            return Json(result);

        }



        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public async Task<List<VWRiwayatDokumen>> xxxriwayatDokumen(Guid Id)
        {
            List<VWRiwayatDokumen> lstRiwyat = new List<VWRiwayatDokumen>();
            try
            {
                //var riwayat = _repository.lstRiwayatMemo(Id).OrderByDescending(d => d.CreateDate);
                foreach (var item in db.EMemoLogs.Where(log => log.EMemoId == Id))
                {
                    var userx = new Userx();
                    if (item.UserId != null)
                        userx = await userDetail(item.UserId.ToString());
                    VWRiwayatDokumen nVWRiwayatDokumen = new VWRiwayatDokumen();
                    nVWRiwayatDokumen.Id = (Guid)item.Id;
                    nVWRiwayatDokumen.Nama = userx is null ? "n/a" : userx.Nama;
                    nVWRiwayatDokumen.ActionDate = item.CreateDate;
                    nVWRiwayatDokumen.Comment = item.Content;
                    lstRiwyat.Add(nVWRiwayatDokumen);
                }
            }
            catch
            {

            }
            return lstRiwyat;
        }




        [HttpGet]
        public IHttpActionResult GetRiwayat(string docId)
        {
            if (string.IsNullOrEmpty(docId) || !Guid.TryParse(docId, out Guid result))
                return BadRequest();

            var request = new EMemoRequest()
            {
                Header = new BaseHeader()
                {
                    UserId = UserId().ToString()
                },
                Detail = new EMemoRequestDetail()
                {
                    Id = Guid.Parse(docId)
                }
            };

            var response = eMemoService.Get(request, UserId());

            var status = response.Header?.StatusCode;
            if (status != Base.ViewModels.StatusCode.SUCCESS)
            {
                if (status == Base.ViewModels.StatusCode.BAD_REQUEST)
                    return BadRequest(response.Header?.StatusMessage);
                else if (status == Base.ViewModels.StatusCode.NOT_FOUND)
                    return NotFound();
                else
                    return InternalServerError(new Exception(response.Header?.StatusMessage));
            }
            else
            {
                return Json(response);
            }
        }

    }
}
