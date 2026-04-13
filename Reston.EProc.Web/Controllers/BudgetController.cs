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
using SpreadsheetLight;
using Reston.Eproc.Model.Monitoring.Entities;


namespace Reston.Pinata.WebService.Controllers
{
    public class BudgetController : BaseController
    {
        private IPengadaanRepo _repoPengadaan;
        private IBudgetRepo _repository;
        private string FILE_UPLOAD_BUDGET = System.Configuration.ConfigurationManager.AppSettings["FILE_UPLOAD_BUDGET"];

        public BudgetController()
        {
            _repoPengadaan = new PengadaanRepo(new AppDbContext());
            _repository = new BudgetRepo(new AppDbContext());
        }

        public BudgetController(BudgetRepo repository)
        {
            _repository = repository;
        }
       
      
        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                          IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                           IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public IHttpActionResult List()
        {
            try
            {
              int start = Convert.ToInt32(System.Web.HttpContext.Current.Request["start"].ToString());
              string cari = System.Web.HttpContext.Current.Request["cari"].ToString();
              int length = Convert.ToInt32(System.Web.HttpContext.Current.Request["length"].ToString()); 
              var data = _repository.List( start, length,cari);
              return Json(data);
            }
            catch (Exception ex)
            {
              return Json(new DataTablePksTemplate());
            }
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                               IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                                IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]      
        public IHttpActionResult importXls()
        {
            var file = HttpContext.Current.Request.Files.Count > 0 ?
            HttpContext.Current.Request.Files[0] : null;

            if (file == null || file.ContentLength == 0)
            {
                return Json("Error");
            }

            List<COA> lstCoa = new List<COA>();
            using (var xls = new SLDocument(file.InputStream))
            {
             
                for (var i = 2; i <= xls.GetWorksheetStatistics().EndRowIndex; i++)
                {
                  COA nCoa = new COA();
                  nCoa.NoCoa = xls.GetCellValueAsString(i, 1);
                  nCoa.Region = xls.GetCellValueAsString(i, 2);
                  nCoa.Divisi = xls.GetCellValueAsString(i, 3);
                  nCoa.Periode = xls.GetCellValueAsString(i, 4);
                  nCoa.GroupAset = xls.GetCellValueAsString(i, 5);
                  nCoa.JenisAset = xls.GetCellValueAsString(i, 6);
                  nCoa.NilaiAset = xls.GetCellValueAsString(i, 7);
                  lstCoa.Add(nCoa);
                }  
            }
            return Json(_repository.add(lstCoa, UserId()));          
        }

        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public async Task<IHttpActionResult> UploadBudget(string Tahun, string Jenis)
        {
            //var oSpk = _repository.get();

            //if (oSpk.StatusSpk != StatusSpk.Draft)
            //    return Json("00000000-0000-0000-0000-000000000000");

            var uploadPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            bool isSavedSuccessfully = false;
            string filePathSave = FILE_UPLOAD_BUDGET;//+id ;
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
                    return InternalServerError(e);
                }
            }
            //Guid dokumenId = Guid.NewGuid();


            if (isSavedSuccessfully)
            {
                try
                {
                    DokumenBudget dokumen = new DokumenBudget
                    {
                        File = uploadPath + filePathSave, // <--- yang ini berubah asalnya File = fileName
                        ContentType = contentType,
                        SizeFile = sizeFile,
                        Year = Tahun,
                        Jenis = Jenis
                    };
                    // savw dokumen to repo
                    // return sucess
                    _repository.saveDokumenBudget(dokumen, UserId());
                    return Ok();
                }
                catch (Exception ex)
                {
                    // return internal server error
                    return InternalServerError(ex);
                    ;                   // return Json("00000000-0000-0000-0000-000000000000");
                }
            }

            // return client error / internal server error
            return InternalServerError();
            //return Json("00000000-0000-0000-0000-000000000000");
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_vendor)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public List<VWDokumenBudget> getDokumensBudget()
        {
            return _repository.GetListDokumenBudget();
        }

        //[ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.Roles.pRole_procurement_head, IdLdapConstants.Roles.pRole_procurement_manager })]
        ////public List<VWBudgeting> GetLoadCOABudget(string branch, string department, string year, string month, string jenispembelanjaan, string pengadaanid, int start, int length)
        //public DataTableBudgeting GetLoadCOABudget(string branch, string department, string year, string month, string jenispembelanjaan, string pengadaanid, int start, int length)
        //{
        //    //return _repository.GetLoadCOAPengadaan(branch, department, year, month, jenispembelanjaan, pengadaanid, start, length).ToList();
        //    return _repository.GetLoadCOAPengadaan(branch, department, year, month, jenispembelanjaan, pengadaanid, start, length);
        //}

        [ApiAuthorize]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public IHttpActionResult GetLoadCOAPengadaan(BudgetAllocationFilterVM filter)
        {
            int start = Convert.ToInt32(HttpContext.Current.Request["start"]);
            int length = Convert.ToInt32(HttpContext.Current.Request["length"]);
            filter.Month = "";
            //return Json(new { aaData = GetLoadCOABudget(filter.Branch, filter.Department, filter.Year, filter.Month, filter.Jenispembelanjaan, filter.Pengadaanid, start, length) });
            return Json(_repository.GetLoadCOAPengadaan(filter.Branch, filter.Department, filter.Year, /*filter.Month,*/ filter.Jenispembelanjaan, filter.Pengadaanid, start, length));
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head, IdLdapConstants.Roles.pRole_approver,
                                           IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                            IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_vendor,
                                            IdLdapConstants.Roles.pRole_direksi)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public IHttpActionResult GetTblCOAPengadaan()
        {
            int sortColumn = -1;
            string sortDirection = "asc";
            if (HttpContext.Current.Request["order[0][column]"] != null)
            {
                sortColumn = int.Parse(HttpContext.Current.Request["order[0][column]"]);
            }
            if (HttpContext.Current.Request["order[0][dir]"] != null)
            {
                sortDirection = HttpContext.Current.Request["order[0][dir]"];
            }
            //string search = HttpContext.Current.Request["search"].ToString();
            string branch = HttpContext.Current.Request["branch"].ToString();
            string department = HttpContext.Current.Request["department"].ToString();
            string year = HttpContext.Current.Request["year"].ToString();
            string jenis = HttpContext.Current.Request["jenis"].ToString();
            //string month = HttpContext.Current.Request["month"].ToString();
            int start = Convert.ToInt32(HttpContext.Current.Request["start"]);
            int length = Convert.ToInt32(HttpContext.Current.Request["length"]);
            return Json(_repository.GetAllCOAPengadaan(/*search,*/department , branch, year, jenis, /*month,*/ start, length, sortColumn, sortDirection));
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                           IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                            IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public VWDokumenBudget CekUpload()
        {
            return _repository.CekUpload();
        }

        [HttpPost]
        public Task<HttpResponseMessage> SaveAllocation(BudgetAllocHeaderVM budgetAllocHeaderVM)
        {

            if (ModelState.IsValid)
            {
                // remove existing allocation if any
                _repository.RemoveHeaderCOA(budgetAllocHeaderVM.PengadaanId);
                // save new allocation
                BudgetingPengadaanHeader header = new BudgetingPengadaanHeader
                {
                    //Id = Guid.NewGuid(),
                    PengadaanId = budgetAllocHeaderVM.PengadaanId,
                    TotalInput = budgetAllocHeaderVM.TotalAllocatedAmount,
                    BudgetingPengadaanDetails = new List<BudgetingPengadaanDetail>(),
                    CreatedBy = UserId(),
                    CreatedOn = DateTime.Now,
                    ModifiedBy = UserId(),
                    ModifiedOn = DateTime.Now
                };

                if (budgetAllocHeaderVM.BudgetAllocDetails != null && budgetAllocHeaderVM.BudgetAllocDetails.Count() > 0)
                {
                    budgetAllocHeaderVM.BudgetAllocDetails.ForEach(detail =>
                    {
                        if (detail == null) return;

                        header.BudgetingPengadaanDetails.Add(new BudgetingPengadaanDetail
                        {
                            Branch = detail.Branch,
                            Department = detail.Department,
                            NoCOA = detail.COA,
                            Month = detail.Month,
                            Input = detail.Amount,
                            Year = detail.Year,
                            BudgetType = detail.BudgetType,
                            Version = detail.Version,
                            CreatedBy = UserId(),
                            CreatedOn = DateTime.Now,
                            ModifiedBy = UserId(),
                            ModifiedOn = DateTime.Now
                        });
                    });
                    var savedAlloc = _repository.SaveAllocation(header);
                    BudgetAllocHeaderVM savedAllocVM = new BudgetAllocHeaderVM
                    {
                        PengadaanId = savedAlloc.PengadaanId,
                        TotalAllocatedAmount = savedAlloc.TotalInput
                    };
                    return Task.FromResult(Request.CreateResponse(HttpStatusCode.OK, savedAllocVM, Configuration.Formatters.JsonFormatter));
                }
                return Task.FromResult(Request.CreateResponse(HttpStatusCode.OK));
            }
            else
            {
                var response =  new HttpResponseMessage(HttpStatusCode.BadRequest);
                response.Content = new StringContent("Invalid parameter format ");
                return Task.FromResult(response);

            }
             
        }
    }

    public class BudgetAllocHeaderVM
    {
        public Guid PengadaanId { get; set; }
        public Decimal TotalAllocatedAmount { get; set; }
        public List<BudgetAllocDetailVM> BudgetAllocDetails { get; set; }
    }

    public class BudgetAllocDetailVM
    {
        public string Branch { get; set; }
        public string Department { get; set; }
        public string COA { get; set; }
        public Decimal Amount { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string BudgetType { get; set; }
        public int Version { get; set; }


    }

    public class BudgetAllocationFilterVM
    {
        public string Branch { get; set; }
        public string Department { get; set; }
        public string Year { get; set; }
        public string Month { get; set; }
        public string Jenispembelanjaan { get; set; }
        public string Pengadaanid { get; set; }
    }
}
