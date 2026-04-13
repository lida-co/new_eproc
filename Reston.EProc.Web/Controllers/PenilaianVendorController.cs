using Model.Helper;
using Reston.Eproc.Model.Monitoring.Entities;
using Reston.Eproc.Model.Monitoring.Model;
using Reston.Eproc.Model.Monitoring.Repository;
using Reston.Pinata.Model;
using Reston.Pinata.Model.Helper;
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
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Reston.EProc.Web.Controllers
{
    public class PenilaianVendorController : BaseController
    {
        private IPenilaianVendorRepo _repository;
        internal ResultMessage result = new ResultMessage();

        public PenilaianVendorController()
        {
            _repository = new PenilaianVendorRepo(new AppDbContext());
        }

        public IHttpActionResult NgeheTampilJudulDetail()
        {
            string NoSPK = HttpContext.Current.Request["Id"];

            return Json( _repository.GetTampilJudul(NoSPK));
        }

        public IHttpActionResult TampilPenilaian()
        {
            string NoSpk = HttpContext.Current.Request["Id"];
            return Json(_repository.GetDataPenilaian(NoSpk));
        }

        public ResultMessage SimpanNilai(PenilaianVendorHeader PenilaianHeader)
        {
            try
            {
                result = _repository.SimpanPenilaian(PenilaianHeader, UserId());
            }
            catch(Exception ex)
            {
                result.message = ex.ToString();
                result.status = HttpStatusCode.ExpectationFailed;
            }

            return result;
        }

        public IHttpActionResult TampilNilai()
        {
            string IdProyek = HttpContext.Current.Request["Id"];
            return Json(_repository.GetNilai(IdProyek));
        }
        public IHttpActionResult CekSudahDiNilai()
        {
            string NoSPK = HttpContext.Current.Request["Id"];

            return Json(_repository.GetCekSudahDiNilai(NoSPK));
        }
    }
}
