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
using Reston.Eproc.Model.Ext;
using Reston.Pinata.Model.PengadaanRepository;
using Reston.Pinata.Model.PengadaanRepository.View;
using Reston.EProc.Web.ViewModels;
using Reston.Pinata.Model.JimbisModel;

namespace Reston.EProc.Web.Controllers
{
    public class NilaiVendorController : BaseController
    {
        
        private INilaiVendorRepo _repository;
        AppDbContext ctx;

        public NilaiVendorController()
        {
            _repository = new NilaiVendorRepo(new AppDbContext());
        }

        public NilaiVendorController(AppDbContext j)
        {
            ctx = j;
            //ctx.Configuration.ProxyCreationEnabled = false;
            ctx.Configuration.LazyLoadingEnabled = true;
        }

        internal ResultMessage result = new ResultMessage();

        //[ApiAuthorize]
        //[ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_master_user, IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                           IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                           IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance,
                                           IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_direksi, IdLdapConstants.Roles.pRole_procurement_vendor,
                                           IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_dirut)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public IHttpActionResult ListPemenangPengadaanNilaiVendor()
        {
            string search = HttpContext.Current.Request["search"].ToString();
            int start = Convert.ToInt32(HttpContext.Current.Request["start"]);
            int length = Convert.ToInt32(HttpContext.Current.Request["length"]);
            return Json(_repository.GetDataListPemenangPengadaanNilaiVendor(search, start, length));
        }

        //[ApiAuthorize]
        //[ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_master_user, IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                           IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                           IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance,
                                           IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_direksi, IdLdapConstants.Roles.pRole_procurement_vendor,
                                           IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_dirut)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public IHttpActionResult ListPemenangPengadaanNilaiVendorsudah()
        {
            string search = HttpContext.Current.Request["search"].ToString();
            int start = Convert.ToInt32(HttpContext.Current.Request["start"]);
            int length = Convert.ToInt32(HttpContext.Current.Request["length"]);
            return Json(_repository.GetDataListPemenangPengadaanNilaiVendorsudah(search, start, length));
        }


        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                           IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                           IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance,
                                           IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_direksi, IdLdapConstants.Roles.pRole_procurement_vendor,
                                           IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_dirut)]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public VWdetailSPKNilaiVendor detailSPKNilaiVendor()
        {
            string NoSPK = HttpContext.Current.Request["Id"];
            var detailSPKNilai = _repository.GetdetailSPKNilaiVendor(NoSPK);
            return detailSPKNilai;
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                           IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                           IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance,
                                           IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_direksi, IdLdapConstants.Roles.pRole_procurement_vendor,
                                           IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_dirut)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public IHttpActionResult SaveCreatePertanyaan(VWTenderScoringHeaderExt vwtenderscoringheader)
        {
            HttpStatusCode respon = HttpStatusCode.NotFound;
            string message = "";
            string id = "";
            try
            {
                var createpenilaian = _repository.AddPertanyaan(vwtenderscoringheader, UserId());
                respon = HttpStatusCode.OK;
                message = Common.SaveSukses();
            }
            catch (Exception ex)
            {
                respon = HttpStatusCode.NotImplemented;
                message = ex.ToString();
            }
            finally
            {
                result.status = respon;
                result.message = message;
                result.Id = id;
            }
            return Json(result);
        }



        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                           IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                           IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance,
                                           IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_direksi, IdLdapConstants.Roles.pRole_procurement_vendor,
                                           IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_dirut)]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public List<VWdetailSPKNilaiVendor> tampilpersonilpenilai()
        {
            string NoSPK = HttpContext.Current.Request["Id"];
            var detailSPKNilai = _repository.Getampilpersonilpenilai(NoSPK);

            //foreach (var i in detailSPKNilai)
            //{
            //   
            //}

            return detailSPKNilai;
        }


        //public IHttpActionResult PersonilPenilaian(Guid Id)
        //{
        //    return Json(_repository.GetPointPenilaian(Id));
        //}

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                           IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                           IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance,
                                           IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_direksi, IdLdapConstants.Roles.pRole_procurement_vendor,
                                           IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_dirut)]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public async Task<IHttpActionResult> GetPersonPenilai(Guid Id, int VendorId)
        {

            var VWPersonilPenilaian = _repository.GetPersonPenilai(Id, VendorId);
            
            List<VWPersonilPenilaian> perpen = new List<VWPersonilPenilaian>();
            foreach (var item in VWPersonilPenilaian)
            {
                VWPersonilPenilaian vp = new VWPersonilPenilaian();
                vp.AppriserUserId = item.AppriserUserId;
                var userx = new Userx();
                if (UserId() != null)
                    userx = await userDetail(item.AppriserUserId.ToString());
                vp.NamaPenilai = userx.Nama;
                perpen.Add(vp);
            }
            return Json(perpen);
        }
        
        public async Task<IHttpActionResult> TampilPenilai(Guid Id)
        {
            var userx = new Userx();
            if (UserId() != null)
                userx = await userDetail(UserId().ToString());
            VWASPNetUser aspnetuser = new VWASPNetUser();
            aspnetuser.DisplayName = userx.Nama;
            aspnetuser.Position = userx.jabatan;
            return Json(aspnetuser);
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                           IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                           IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance,
                                           IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_direksi, IdLdapConstants.Roles.pRole_procurement_vendor,
                                           IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_dirut)]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult CekCreatePertanyaan(Guid Id, int VendorId)
        {
            return Json(_repository.CekPertanyaan(Id, VendorId));
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                           IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                           IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance,
                                           IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_direksi, IdLdapConstants.Roles.pRole_procurement_vendor,
                                           IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_dirut)]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult PointPenilaian(Guid Id, int VendorId)
        {
            return Json(_repository.GetPointPenilaian(Id, VendorId));
        }



        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                           IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                           IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance,
                                           IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_direksi, IdLdapConstants.Roles.pRole_procurement_vendor,
                                           IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_dirut)]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        //public IHttpActionResult cekmasteruser()
        public IHttpActionResult TampilAssessment(Guid Id, int VendorId)
        {
            return Json(_repository.GetAssessment(Id, VendorId));
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                           IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                           IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance,
                                           IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_direksi, IdLdapConstants.Roles.pRole_procurement_vendor,
                                           IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_dirut)]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        //public IHttpActionResult cekmasteruser()
        public int deletePenilaian(Guid Id, int VendorId)
        {
            //Guid PengadaanId = Guid.Parse(HttpContext.Current.Request["PengadaanId"].ToString());
            //var data = _repository.cekRKSBiasaAtauAsuransi(PengadaanId);//search, start, length);
            //var a = 1;
            //return Json(a);
            return _repository.nextToDelete(Id, VendorId);
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                           IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                           IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance,
                                           IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_direksi, IdLdapConstants.Roles.pRole_procurement_vendor,
                                           IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_dirut)]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        //public IHttpActionResult cekmasteruser()
        public async Task<IHttpActionResult> TampilDropDownPenilai(Guid Id, int VendorId)
        //public IHttpActionResult TampilDropDownPenilai(Guid Id, int VendorId)
        {
            var VWPersonilPenilaian = _repository.GetPersonPenilai(Id, VendorId);

            List<VWPersonilPenilaian> perpen = new List<VWPersonilPenilaian>();
            foreach (var item in VWPersonilPenilaian)
            {
                VWPersonilPenilaian vp = new VWPersonilPenilaian();
                vp.AppriserUserId = item.AppriserUserId;
                var userx = new Userx();
                if (UserId() != null)
                    userx = await userDetail(item.AppriserUserId.ToString());
                    vp.NamaPenilai = userx.Nama;
                    vp.AppriserUserId = item.AppriserUserId;
                perpen.Add(vp);
            }
            return Json(perpen);
        }


        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                           IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                           IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance,
                                           IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_direksi, IdLdapConstants.Roles.pRole_procurement_vendor,
                                           IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_dirut)]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult GetDataValue(Guid Id, Guid UserIdAssessment, int VendorId)
        {
            return Json(_repository.GetValueAssessment(Id, UserIdAssessment, VendorId));
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                           IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                           IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance,
                                           IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_direksi, IdLdapConstants.Roles.pRole_procurement_vendor,
                                           IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_dirut)]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult TampilQuestion(Guid Id, int VendorId)
        {
            return Json(_repository.GetQuestion(Id, VendorId));
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                           IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                           IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance,
                                           IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_direksi, IdLdapConstants.Roles.pRole_procurement_vendor,
                                           IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_dirut)]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult TampilPertanyaan()
        {
            return Json(_repository.GetDataPertanyaan());
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                           IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                           IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance,
                                           IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_direksi, IdLdapConstants.Roles.pRole_procurement_vendor,
                                           IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_dirut)]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult SaveCreateAssessment(VWTenderScoringDetails vwtenderscoringdetail, Guid Id, int VendorId, decimal Total)
        {
            HttpStatusCode respon = HttpStatusCode.NotFound;
            string message = "";
            string id = "";
            try
            {
                var createpenilaian = _repository.AddAssessment(vwtenderscoringdetail, Id, VendorId, Total, UserId());
                respon = HttpStatusCode.OK;
                message = Common.SaveSukses();
            }
            catch (Exception ex)
            {
                respon = HttpStatusCode.NotImplemented;
                message = ex.ToString();
            }
            finally
            {
                result.status = respon;
                result.message = message;
                result.Id = id;
            }
            return Json(result);
        }


        //[ApiAuthorize]
        //[ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_master_user, IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                           IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                           IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance,
                                           IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_direksi, IdLdapConstants.Roles.pRole_procurement_vendor,
                                           IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_dirut)]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult ListPemenangPengadaanNilaiVendorAssesment()
        {
            string search = HttpContext.Current.Request["search"].ToString();
            int start = Convert.ToInt32(HttpContext.Current.Request["start"]);
            int length = Convert.ToInt32(HttpContext.Current.Request["length"]);
            return Json(_repository.GetDataListPemenangPengadaanNilaiVendorAssesment(search, start, length, UserId()));
        }

        //[ApiAuthorize]
        //[ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_master_user, IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                           IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                           IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance,
                                           IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_direksi, IdLdapConstants.Roles.pRole_procurement_vendor,
                                           IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_dirut)]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult ListVendorWithSanksi()
        {
            

            string search = HttpContext.Current.Request["search"].ToString();
            string status = HttpContext.Current.Request["status"].ToString();
            string bidang = HttpContext.Current.Request["bidang"].ToString();
            string kelompok = HttpContext.Current.Request["kelompok"].ToString();

            //if (kelompok != string.Empty) {
            //    char[] delimiterChars = {','};
            //    string text = kelompok.Replace("\"", "").Replace("[", "").Replace("]", "");
            //    string[] words = text.Split(delimiterChars);
            //    foreach (var word in words)
            //    {
            //        string a = word.ToString();
            //    }
            //}
                              
            int start = Convert.ToInt32(HttpContext.Current.Request["start"]);
            int length = Convert.ToInt32(HttpContext.Current.Request["length"]);
            return Json(_repository.GetListVendorWithSanksi(search, status, bidang, kelompok, start, length));
        }


        //[ApiAuthorize]
        //[ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_master_user, IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                           IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                           IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance,
                                           IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_direksi, IdLdapConstants.Roles.pRole_procurement_vendor,
                                           IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_dirut)]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult CekVendorSanksi(int VendorId)
        {
            return Json(_repository.GetCekVendorSanksi(VendorId));
        }


        [HttpPost]
        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                           IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                           IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance,
                                           IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_direksi, IdLdapConstants.Roles.pRole_procurement_vendor,
                                           IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_dirut)]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult VendorSanksi(VWSanksi vwsanksi)
        {
            HttpStatusCode respon = HttpStatusCode.NotFound;
            string message = "";
            string id = "";
            try
            {
                var createpenilaian = _repository.AddVendorSanksi(vwsanksi, UserId());
                respon = HttpStatusCode.OK;
                message = Common.SaveSukses();
            }
            catch (Exception ex)
            {
                respon = HttpStatusCode.NotImplemented;
                message = ex.ToString();
            }
            finally
            {
                result.status = respon;
                result.message = message;
                result.Id = id;
            }
            return Json(result);
        }


        //[ApiAuthorize]
        //[ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_master_user, IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                           IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                           IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance,
                                           IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_direksi, IdLdapConstants.Roles.pRole_procurement_vendor,
                                           IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_dirut)]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult ListSanksi()
        {
            string search = HttpContext.Current.Request["search"].ToString();
            int start = Convert.ToInt32(HttpContext.Current.Request["start"]);
            int length = Convert.ToInt32(HttpContext.Current.Request["length"]);
            return Json(_repository.GetListSanksi(search, start, length));
        }


        //[ApiAuthorize]
        //[ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_master_user, IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                           IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                           IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance,
                                           IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_direksi, IdLdapConstants.Roles.pRole_procurement_vendor,
                                           IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_dirut)]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult ListRiwayatSanksi(int VendorId)
        {
            string search = HttpContext.Current.Request["search"].ToString();
            int start = Convert.ToInt32(HttpContext.Current.Request["start"]);
            int length = Convert.ToInt32(HttpContext.Current.Request["length"]);
            return Json(_repository.GetListRiwayatSanksi(VendorId, search, start, length));

        }

        //[ApiAuthorize]
        //[ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_master_user, IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                           IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                           IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance,
                                           IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_direksi, IdLdapConstants.Roles.pRole_procurement_vendor,
                                           IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_dirut)]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult ListRiwayatPenilaian(int VendorId)
        {
            string search = HttpContext.Current.Request["search"].ToString();
            int start = Convert.ToInt32(HttpContext.Current.Request["start"]);
            int length = Convert.ToInt32(HttpContext.Current.Request["length"]);
            return Json(_repository.GetListRiwayatPenilaian(VendorId, search, start, length));

        }

        //[ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head, IdLdapConstants.Roles.pRole_approver,
        //                                    IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
        //                                     IdLdapConstants.Roles.pRole_procurement_manager,
        //                                     IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_direksi, IdLdapConstants.Roles.pRole_procurement_vendor)]
        //[System.Web.Http.AcceptVerbs("GET", "POST")]
        //public VWdetailSPKNilaiVendor detailSPKNilaiVendor()
        //{
        //    string NoSPK = HttpContext.Current.Request["Id"];
        //    var detailSPKNilai = _repository.GetdetailSPKNilaiVendor(NoSPK);
        //    return detailSPKNilai;
        //}
        //
        //GetVendorDetail

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                           IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                           IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance,
                                           IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_direksi, IdLdapConstants.Roles.pRole_procurement_vendor,
                                           IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_dirut)]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult GetVendorDetail(string no)
        {
            //do something, prevent access etc
            //VendorExtViewModel vm = new VendorExtViewModel();
            VendorExtViewModelJaws v = _repository.GetVendor(no);

            return Json(v);
            //return v;
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                           IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                           IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance,
                                           IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_direksi, IdLdapConstants.Roles.pRole_procurement_vendor,
                                           IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_dirut)]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult GetVendorDetailNew(int idVendor)
        {
            VendorExtViewModelJaws v = _repository.GetVendorDetailNew(idVendor);
            return Json(v);
        }


        //[ApiAuthorize]
        //[ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_master_user, IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                           IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                           IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance,
                                           IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_direksi, IdLdapConstants.Roles.pRole_procurement_vendor,
                                           IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_dirut)]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult rekananPencarian()
        {
            string search = HttpContext.Current.Request["search"].ToString();
            string status = HttpContext.Current.Request["status"].ToString();
            string tipe = HttpContext.Current.Request["tipe"].ToString();
            int start = Convert.ToInt32(HttpContext.Current.Request["start"]);
            int length = Convert.ToInt32(HttpContext.Current.Request["length"]);
            return Json(_repository.GetrekananPencarian(search, status, tipe, start, length));
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                           IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                           IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance,
                                           IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_direksi, IdLdapConstants.Roles.pRole_procurement_vendor,
                                           IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_dirut)]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public HttpResponseMessage GetFile(Guid Iddok) {

            var doc = _repository.GetDokumen(Iddok);

            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(doc.Content)
            };
            result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("inline")
            {
                FileName = doc.FileName
            };
            result.Content.Headers.ContentType = new MediaTypeHeaderValue(doc.ContentType);

            return result;
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                           IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                           IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance,
                                           IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_direksi, IdLdapConstants.Roles.pRole_procurement_vendor,
                                           IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_dirut)]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public IHttpActionResult detailSanksiVendor(int VendorId)
        {
            return Json(_repository.GetdetailSanksiVendor(VendorId));
        }

        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_vendor })]
        public IHttpActionResult GetVendorDetailNewSideTerdaftar()
        {
            var id = _repository.GetIdVendorFromOwner(UserId());
            VendorExtViewModelJaws v = _repository.GetVendorDetailNew(id);
            return Json(v);
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                           IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                           IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance,
                                           IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_direksi, IdLdapConstants.Roles.pRole_procurement_vendor,
                                           IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_dirut)]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public bool CheckVedndorEXT(int idVendor)
        {
            VendorExtViewModelJaws v = _repository.GetVendorDetailNew(idVendor);
            bool isEXT = false;
            if (v.VendorRegExt != null) isEXT = true; 
            return isEXT;
        }
    }
}
