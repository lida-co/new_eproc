using Microsoft.Owin.FileSystems;
using Reston.Pinata.Model;
using Reston.Pinata.Model.JimbisModel;
using Reston.Pinata.Model.Asuransi;
using Reston.Pinata.Model.Repository;
using Reston.Pinata.WebService.ViewModels;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net.Http.Headers;
using System.Net;
using Model.Helper;
using Reston.Pinata.WebService.Helper;
using System.Web;
using Reston.Eproc.Model.Monitoring.Model;
using Reston.Eproc.Model.Monitoring.Repository;
using Reston.Pinata.Model.PengadaanRepository.View;
using Reston.Pinata.Model.Helper;
using Reston.Pinata.Model.PengadaanRepository;
using Newtonsoft.Json;


namespace Reston.Pinata.WebService
{


    public class AsuransiController : BaseController
    {
        private AppDbContext _modelContext;
        public AsuransiController()
        {
            _modelContext = new AppDbContext();
        }

        
        //AppDbContext ctx;
        //public AsuransiController(AppDbContext j)
        //{
        //    ctx = j;
        //}
        ResultMessage msg = new ResultMessage();

        private IMoritoringRepo _repository;

        //-------------
        //private IMoritoringRepo _repository;
        //public AsuransiController()
        //{
        //    _repository = new MonitoringRepo(new AppDbContext());
        //}
        //---------------
        public AsuransiController(AppDbContext modelContext)
        {
            _modelContext = modelContext;
        }

        

        [HttpPost]
        public IHttpActionResult AddBenefit([FromUri] Guid documentID, [FromBody] BenefitRateTemplate benefRate)
        {
            _modelContext.BenefitRateTemplates.Add(benefRate);
            var newDoc = new InsuranceTarifBenefitTemplate()
            {
                DocumentId = documentID,
                BenefitRateId = benefRate
            };

            _modelContext.InsuranceTarifBenefitTemplates.Add(newDoc);
            _modelContext.SaveChanges();

            return Json(newDoc);
        }

        [HttpPost]
        public IHttpActionResult AddBenefNoTpl([FromUri] Guid DocumentIdBaru, [FromBody] BenefitRate benefRate)
        {
            _modelContext.BenefitRates.Add(benefRate);
            var newDoc = new InsuranceTarifBenefit()
            {
                DocumentId = DocumentIdBaru,
                BenefitRateId = benefRate
            };

            _modelContext.InsuranceTarifBenefits.Add(newDoc);
            _modelContext.SaveChanges();

            return Json(newDoc);
        }

        [HttpPost]
        public IHttpActionResult EditBenefNoTpl([FromUri] Guid DocumentIdBaru, [FromBody] BenefitRate benefRate)
        {
            _modelContext.BenefitRates.Add(benefRate);
            //var newDoc = new InsuranceTarifBenefit()
            //{
            //    DocumentId = DocumentIdBaru,
            //    BenefitRateId = benefRate
            //};

            //_modelContext.InsuranceTarifBenefits.Add(newDoc);
            _modelContext.SaveChanges();

            return Json(benefRate);
        }

        [HttpPost]
        public IHttpActionResult SelaluSimpan(int DocumentTitle, [FromBody] BenefitRate benefRate)
        {
            _modelContext.BenefitRates.Add(benefRate);
            var bla = new InsuranceTarifTemplate() { 
                DocumentTitle = DocumentTitle.ToString()
            };

            _modelContext.InsuranceTarifTemplates.Add(bla);
            _modelContext.SaveChanges();

            return Json(bla);
        }


        [HttpPost]
        public IHttpActionResult Save()
        {

            var userId = UserId();
            var newDoc = new InsuranceTarifTemplate()
            {
                DocumentId = Guid.NewGuid(),
                DocumentTitle = "(Belum Ada Judul)",
                CreatedBy = userId.ToString(),
                CreatedTS = DateTime.Now,
                BenefitType = ""
            };

            _modelContext.InsuranceTarifTemplates.Add(newDoc);
            _modelContext.SaveChanges();

            return Json(newDoc.DocumentId);
        }

        [HttpPost]
        public IHttpActionResult Create()
        {

            var userId = UserId();
            var newDoc = new InsuranceTarifTemplate()
            {
                DocumentId = Guid.NewGuid(),
                DocumentTitle = "(Belum Ada Judul)",
                CreatedBy = userId.ToString(),
                CreatedTS = DateTime.Now,
                BenefitType = ""
            };

            _modelContext.InsuranceTarifTemplates.Add(newDoc);
            _modelContext.SaveChanges();

            return Json(newDoc.DocumentId);
        }

        [HttpPost]
        public IHttpActionResult CreateNoTPL(Guid PengadaanId)
        {

            var userId = UserId();
            var newDoc = new InsuranceTarif()
            {
                DocumentId = Guid.NewGuid(),
                DocumentTitle = "(Belum Ada Judul)",
                CreatedBy = userId.ToString(),
                CreatedTS = DateTime.Now,
                BenefitType = "",
                PengadaanId = PengadaanId
            };

            _modelContext.InsuranceTarifs.Add(newDoc);
            _modelContext.SaveChanges();

            return Json(newDoc.DocumentId);
        }

        [HttpGet]
        public IHttpActionResult GetEdit(int Id)
        {
            var templateDoc = _modelContext.BenefitRateTemplates.Where(doc => doc.Id == Id).FirstOrDefault();

            return Json(templateDoc);
        }

        [HttpGet]
        public IHttpActionResult GetEditFromPengadaan(int Id)
        {
            var templateDoc = _modelContext.BenefitRates.Where(doc => doc.Id == Id).FirstOrDefault();

            return Json(templateDoc);
        }

        [HttpGet]
       public IHttpActionResult Get(Guid documentId)
       {
           var templateDoc = _modelContext.InsuranceTarifTemplates.Where(doc => doc.DocumentId == documentId).FirstOrDefault();

           return Json(templateDoc);
       }

        [HttpGet]
        public IHttpActionResult Get()
        {
            var templateDoc = _modelContext.InsuranceTarifTemplates.FirstOrDefault();

            return Json(templateDoc);
        }

        [HttpPost]
        public IHttpActionResult savePenawaranAsuransi(ViewBenefitRate data)
        {
            JimbisEncrypt code = new JimbisEncrypt();
            
            var update = _modelContext.HargaRekananAsuransis.Where(d => d.Id == data.HargaId).FirstOrDefault();

            update.RateEncrypt = code.Encrypt(data.Rate == null ? "0" : data.Rate.ToString());
            update.RateLowerLimitEncrypt = code.Encrypt(data.RateLowerLimit == null ? "0" : data.RateLowerLimit.ToString());
            update.RateUpperLimitEncrypt = code.Encrypt(data.RateUpperLimit == null ? "0" : data.RateUpperLimit.ToString());

            _modelContext.SaveChanges();

            return Json(update.Id);
        }

        [HttpPost]
        public IHttpActionResult savePenawaranAsuransiKlarifikasi(ViewBenefitRate data)
        {
            JimbisEncrypt code = new JimbisEncrypt();

            var update = _modelContext.HargaKlarifikasiRekananAsuransis.Where(d => d.Id == data.HargaId).FirstOrDefault();

            update.Rate = data.Rate;
            update.RateLowerLimit = data.RateLowerLimit;
            update.RateUpperLimit = data.RateUpperLimit;

            _modelContext.SaveChanges();

            return Json(update.Id);
        }

        [HttpPost]
        public IHttpActionResult savePenawaranAsuransiKlarifikasiLanjutan(ViewBenefitRate data)
        {
            JimbisEncrypt code = new JimbisEncrypt();

            var update = _modelContext.HargaKlarifikasiLanjutanAsuransis.Where(d => d.Id == data.HargaId).FirstOrDefault();

            update.Rate = data.Rate;
            update.RateLowerLimit = data.RateLowerLimit;
            update.RateUpperLimit = data.RateUpperLimit;

            _modelContext.SaveChanges();

            return Json(update.Id);
        }

        [HttpPost]
        public IHttpActionResult SaveEditRKSAsuransi(ViewBenefitRate data)
        {
            JimbisEncrypt code = new JimbisEncrypt();

            var update = _modelContext.BenefitRates.Where(d => d.Id == data.Id).FirstOrDefault();

            update.Rate = data.Rate;
            update.RateLowerLimit = data.RateLowerLimit;
            update.RateUpperLimit = data.RateUpperLimit;
            update.BenefitCode = data.BenefitCode;
            update.BenefitCoverage = data.BenefitCoverage;
            update.RegionCode = data.RegionCode;
            update.IsOpen = data.IsOpen;
            update.IsRange = data.IsRange;

            _modelContext.SaveChanges();

            return Json(update.Id);
        }

        [HttpPost]
        public IHttpActionResult SaveEditTemplateRKSAsuransi(ViewBenefitRate data)
        {
            JimbisEncrypt code = new JimbisEncrypt();

            var update = _modelContext.BenefitRateTemplates.Where(d => d.Id == data.Id).FirstOrDefault();

            update.Rate = data.Rate;
            update.RateLowerLimit = data.RateLowerLimit;
            update.RateUpperLimit = data.RateUpperLimit;
            update.BenefitCode = data.BenefitCode;
            update.BenefitCoverage = data.BenefitCoverage;
            update.RegionCode = data.RegionCode;
            update.IsOpen = data.IsOpen;
            update.IsRange = data.IsRange;

            _modelContext.SaveChanges();

            return Json(update.Id);
        }

    }
}