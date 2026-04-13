using Microsoft.Owin.FileSystems;
using Reston.Pinata.Model;
using Reston.Pinata.Model.JimbisModel;
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
using Reston.Pinata.Model.Helper;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Web;
using DocumentFormat.OpenXml.Vml;
using DocumentFormat.OpenXml.Drawing;
using Newtonsoft.Json;
using DocumentFormat.OpenXml.Bibliography;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System.Text.RegularExpressions;
//using System.Web.Helpers;

namespace Reston.Pinata.WebService
{
    public class ReferenceDataController : ApiController
    {
        private AppDbContext _modelContext;
        private IReferenceDataRepo _repository;
        public ReferenceDataController()
        {
            _repository = new ReferenceDataRepo(new AppDbContext());
            _modelContext = new AppDbContext();
        }

        //fchr

        private string Sanitize(string input)
        {
            return HttpUtility.HtmlEncode(input);
        }

        private bool IsSafe(string input)
        {
            return !System.Text.RegularExpressions.Regex.IsMatch(input, @"[<>]");
        }

        //end fchr

        [HttpGet]
        public List<ReferenceDataViewModel> GetAllProvinsi()
        {
            return _repository.GetData(RefDataQualifier.PROVINCE, null).Select(x => new ReferenceDataViewModel()
            {
                id = x.Id,
                Code = x.Code,
                Name = x.LocalizedName,
                Desc = x.LocalizedDesc,
                Str1 = x.StringAttr1,
                Int1 = x.IntAttr1,
                Flag1 = x.FlagAttr1
            }).ToList();
        }

        [HttpGet]
        //[ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_admin})]
        [ApiAuthorize]
        public IHttpActionResult GetTblProvinsi()
        {
            return Json(new { aaData = GetAllProvinsi() });
        }

        [HttpGet]
        public List<ReferenceDataViewModel> GetAllCurrency()
        {
            return _repository.GetData(RefDataQualifier.CURRENCY, null).Select(x => new ReferenceDataViewModel()
            {
                id = x.Id,
                Code = x.Code,
                Name = x.LocalizedName,
                Desc = x.LocalizedDesc,
                Str1 = x.StringAttr1,
                Int1 = x.IntAttr1,
                Flag1 = x.FlagAttr1
            }).ToList();
        }

        [HttpGet]
        //[ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_admin})]
        [ApiAuthorize]
        public IHttpActionResult GetTblCurrency()
        {
            return Json(new { aaData = GetAllCurrency() });
        }

        //[ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.Roles.pRole_procurement_end_user, IdLdapConstants.Roles.pRole_procurement_head, IdLdapConstants.Roles.pRole_procurement_manager})]
        [HttpGet]
        [ApiAuthorize]
        public List<ReferenceDataViewModel> GetAllRegion()
        {
            return _repository.GetData(RefDataQualifier.REGION, null).Select(x => new ReferenceDataViewModel()
            {
                id = x.Id,
                Code = x.Code,
                Name = x.LocalizedName,
                Desc = x.LocalizedDesc,
                Str1 = x.StringAttr1,
                Int1 = x.IntAttr1,
                Flag1 = x.FlagAttr1
            }).ToList();
        }

        [HttpGet]
        [ApiAuthorize]
        public List<ReferenceDataViewModel> GetAllRegionPengadaan(int id)
        {
            return _repository.GetDataPengadaan(RefDataQualifier.REGION, id).Select(x => new ReferenceDataViewModel()
            {
                id = x.Id,
                Code = x.Code,
                Name = x.LocalizedName,
                Desc = x.LocalizedDesc,
                Str1 = x.StringAttr1,
                Int1 = x.IntAttr1,
                Flag1 = x.FlagAttr1
            }).ToList();
        }

        [HttpGet]
        //[ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_admin })]
        [ApiAuthorize]
        public IHttpActionResult GetTblRegion()
        {
            return Json(new { aaData = GetAllRegion() });
        }

        [HttpGet]
        public List<ReferenceDataViewModel> GetAllSatuan()
        {
            return _repository.GetData(RefDataQualifier.SATUAN, null).Select(x => new ReferenceDataViewModel()
            {
                id = x.Id,
                Code = x.Code,
                Name = x.LocalizedName,
                Desc = x.LocalizedDesc,
                Str1 = x.StringAttr1,
                Int1 = x.IntAttr1,
                Flag1 = x.FlagAttr1
            }).ToList();
        }

        [HttpGet]
        //[ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_admin })]
        [ApiAuthorize]
        public IHttpActionResult GetTblSatuan()
        {
            return Json(new { aaData = GetAllSatuan() });
        }

        [HttpGet]
        public List<ReferenceDataViewModel> GetAllJenis()
        {
            return _repository.GetData(RefDataQualifier.QUALIFIER_INS_BENEF_TYPE, null).Select(x => new ReferenceDataViewModel()
            {
                id = x.Id,
                Code = x.Code,
                Name = x.LocalizedName,
                Desc = x.LocalizedDesc,
                Str1 = x.StringAttr1,
                Int1 = x.IntAttr1,
                Flag1 = x.FlagAttr1
            }).ToList();
        }

        [HttpGet]
        //[ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_admin })]
        [ApiAuthorize]
        public IHttpActionResult GetTblJenis()
        {
            return Json(new { aaData = GetAllJenis() });
        }

        [HttpGet]
        //[ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_admin })]
        //[ApiAuthorize]
        public List<ReferenceDataViewModel> GetAllBank()
        {
            return _repository.GetData(RefDataQualifier.BANK, null).Select(x => new ReferenceDataViewModel()
            {
                id = x.Id,
                Code = x.Code,
                Name = x.LocalizedName,
                Desc = x.LocalizedDesc,
                Str1 = x.StringAttr1,
                Int1 = x.IntAttr1,
                Flag1 = x.FlagAttr1
            }).ToList();
        }

        [HttpGet]
        //[ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_admin })]
        [ApiAuthorize]
        public IHttpActionResult GetTblBank()
        {
            return Json(new { aaData = GetAllBank() });
        }

        [HttpGet]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_admin })]
        public List<ReferenceDataViewModel> GetAllPeriodeAnggaran()
        {
            return _repository.GetData(RefDataQualifier.PeriodeAnggaran, null).Select(x => new ReferenceDataViewModel()
            {
                id = x.Id,
                Code = x.Code,
                Name = x.LocalizedName,
                Desc = x.LocalizedDesc,
                Str1 = x.StringAttr1,
                Int1 = x.IntAttr1,
                Flag1 = x.FlagAttr1
            }).OrderBy(z => z.Name).ToList();
        }

        [HttpGet]
        //[ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_admin })]
        [ApiAuthorize]
        public IHttpActionResult GetTblPeriodeAnggaran()
        {
            return Json(new { aaData = GetAllPeriodeAnggaran() });
        }

        [HttpGet]
        public List<ReferenceDataViewModel> GetAllUnitKerja()
        {
            return _repository.GetData(RefDataQualifier.UnitKerja, null).Select(x => new ReferenceDataViewModel()
            {
                id = x.Id,
                Code = x.Code,
                Name = x.LocalizedName,
                Desc = x.LocalizedDesc,
                Str1 = x.StringAttr1,
                Int1 = x.IntAttr1,
                Flag1 = x.FlagAttr1
            }).OrderBy(x => x.Name).ToList();
        }

        [HttpGet]
        //[ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_admin })]
        [ApiAuthorize]
        public IHttpActionResult GetTblUnitKerja()
        {
            return Json(new { aaData = GetAllUnitKerja() });
        }

        [HttpGet]
        public List<ReferenceDataViewModel> GetAllJenisPekerjaan()
        {
            return _repository.GetData(RefDataQualifier.JenisPekerjaan, null).Select(x => new ReferenceDataViewModel()
            {
                id = x.Id,
                Code = x.Code,
                Name = x.LocalizedName,
                Desc = x.LocalizedDesc,
                Str1 = x.StringAttr1,
                Int1 = x.IntAttr1,
                Flag1 = x.FlagAttr1
            }).OrderBy(x => x.Name).ToList();
        }

        [HttpGet]
        //[ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_admin })]
        [ApiAuthorize]
        public IHttpActionResult GetTblJenisPekerjaan()
        {
            return Json(new { aaData = GetAllJenisPekerjaan() });
        }

        [HttpGet]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_admin })]
        public List<ReferenceDataViewModel> GetAllJenisPembelanjaan()
        {
            return _repository.GetData(RefDataQualifier.JenisPembelanjaan, null).Select(x => new ReferenceDataViewModel()
            {
                id = x.Id,
                Code = x.Code,
                Name = x.LocalizedName,
                Desc = x.LocalizedDesc,
                Str1 = x.StringAttr1,
                Int1 = x.IntAttr1,
                Flag1 = x.FlagAttr1
            }).OrderBy(x => x.Name).ToList();
        }

        [HttpGet]
        //[ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_admin })]
        [ApiAuthorize]
        public IHttpActionResult GetTblJenisPembelanjaan()
        {
            return Json(new { aaData = GetAllJenisPembelanjaan() });
        }

        [HttpGet]
        public List<ReferenceDataViewModel> GetAllPenilaian()
        {
            return _repository.GetData(RefDataQualifier.Penilaian, null).Select(x => new ReferenceDataViewModel()
            {
                id = x.Id,
                Code = x.Code,
                Name = x.LocalizedName,
                Desc = x.LocalizedDesc,
                Str1 = x.StringAttr1,
                Int1 = x.IntAttr1,
                Flag1 = x.FlagAttr1
            }).ToList();
        }

        [HttpGet]
        //[ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_admin })]
        [ApiAuthorize]
        public IHttpActionResult GetTblPenilaian()
        {
            return Json(new { aaData = GetAllPenilaian() });
        }

        [HttpGet]
        public List<ReferenceDataViewModel> GetAllInfoPerusahaan()
        {
            return _repository.GetData(RefDataQualifier.InfoPerusahaan, null).Select(x => new ReferenceDataViewModel()
            {
                id = x.Id,
                Code = x.Code,
                Name = x.LocalizedName,
                Desc = x.LocalizedDesc,
                Str1 = x.StringAttr1,
                Int1 = x.IntAttr1,
                Flag1 = x.FlagAttr1
            }).ToList();
        }

        [HttpGet]
        //[ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_admin })]
        [ApiAuthorize]
        public IHttpActionResult GetTblInfoPerusahaan()
        {
            return Json(new { aaData = GetAllInfoPerusahaan() });
        }

        [HttpGet]
        public List<ReferenceDataViewModel> GetAllPertanyaan()
        {
            return _repository.GetData(RefDataQualifier.Pertanyaan, null).Select(x => new ReferenceDataViewModel()
            {
                id = x.Id,
                Code = x.Code,
                Name = x.LocalizedName,
                Desc = x.LocalizedDesc,
                Str1 = x.StringAttr1,
                Int1 = x.IntAttr1,
                Flag1 = x.FlagAttr1
            }).ToList();
        }

        [HttpGet]
        //[ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_admin })]
        [ApiAuthorize]
        public IHttpActionResult GetTblPertanyaan()
        {
            return Json(new { aaData = GetAllPertanyaan() });
        }

        [HttpGet]
        public List<KategoriSpesifikasi> GetAllKategoriECatalogue()
        {
            return _modelContext.KategoriSpesifikasis.ToList();
        }

        //public List<KategoriSpesifikasi> GetAllKategoriECatalogue()
        //{
        //    var ls = _modelContext.KategoriSpesifikasis.Where(x => x.Nama != null).Distinct();
        //    ls = ls.OrderBy(x => x.Nama);
        //    return ls.ToList();
        //}

        [HttpGet]
        public IHttpActionResult GetTblKategoriECatalogue()
        {
            return Json(new { aaData = GetAllKategoriECatalogue() });
        }


        //master crud
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        //[ApiAuthorize]
        private string AddReferenceData(ReferenceData r)
        {
            List<ReferenceData> lr = _repository.GetData(r.Qualifier, r.Code);
            if (lr.Count > 0) //exist boss
            {
                return "Gagal! Duplikat data.";
            }
            _repository.SaveData(r);
            return "Sukses!";
        }

        [HttpPost]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        //[ApiAuthorize]
        public IHttpActionResult AddPeriodeAnggaran([FromBody] AddPeriodeAnggaran req)
        {
            //return AddReferenceData(new ReferenceData() { Qualifier = RefDataQualifier.PeriodeAnggaran, Code = code, LocalizedName = nama, LocalizedDesc = deskripsi });
            if (req == null)
                return BadRequest("Request tidak valid");

            if (string.IsNullOrWhiteSpace(req.code))
                return BadRequest("Code tidak boleh kosong");

            if (string.IsNullOrWhiteSpace(req.nama))
                return BadRequest("Nama tidak boleh kosong");

            if (string.IsNullOrWhiteSpace(req.deskripsi))
                return BadRequest("Deskripsi tidak boleh kosong");

            if (!IsSafe(req.code) || !IsSafe(req.nama) || !IsSafe(req.deskripsi))
                return BadRequest("Input mengandung karakter berbahaya");

            try
            {
                var result = AddReferenceData(new ReferenceData()
                {
                    Qualifier = RefDataQualifier.PeriodeAnggaran,
                    Code = HttpUtility.HtmlEncode(req.code),
                    LocalizedName = HttpUtility.HtmlEncode(req.nama),
                    LocalizedDesc = HttpUtility.HtmlEncode(req.deskripsi)
                });

                return Ok(new
                {
                    message = "Berhasil tambah data",
                    data = result
                });
            }
            catch
            {
                return InternalServerError();
            }
        }

        [HttpPost]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        //[ApiAuthorize]
        public IHttpActionResult AddUnitKerja([FromBody] AddUnitKerja req)
        {
            //return AddReferenceData(new ReferenceData() { Qualifier = RefDataQualifier.UnitKerja, Code = code, LocalizedName = nama, LocalizedDesc = deskripsi });
            if (req == null)
                return BadRequest("Request tidak valid");

            if (string.IsNullOrWhiteSpace(req.code))
                return BadRequest("Code tidak boleh kosong");

            if (string.IsNullOrWhiteSpace(req.nama))
                return BadRequest("Nama tidak boleh kosong");

            if (string.IsNullOrWhiteSpace(req.deskripsi))
                return BadRequest("Deskripsi tidak boleh kosong");

            if (!IsSafe(req.code) || !IsSafe(req.nama) || !IsSafe(req.deskripsi))
                return BadRequest("Input mengandung karakter berbahaya");

            try
            {
                var result = AddReferenceData(new ReferenceData()
                {
                    Qualifier = RefDataQualifier.UnitKerja,
                    Code = HttpUtility.HtmlEncode(req.code),
                    LocalizedName = HttpUtility.HtmlEncode(req.nama),
                    LocalizedDesc = HttpUtility.HtmlEncode(req.deskripsi)
                });

                return Ok(new
                {
                    message = "Berhasil tambah data",
                    data = result
                });
            }
            catch
            {
                return InternalServerError();
            }
        }

        [HttpPost]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        //[ApiAuthorize]
        public IHttpActionResult AddPenilaian([FromBody] AddPenilaian req)
        {
            //return AddReferenceData(new ReferenceData() { Qualifier = RefDataQualifier.Penilaian, Code = code, LocalizedName = nama, LocalizedDesc = deskripsi });
            if (req == null)
                return BadRequest("Request tidak valid");

            if (string.IsNullOrWhiteSpace(req.code))
                return BadRequest("Code tidak boleh kosong");

            if (string.IsNullOrWhiteSpace(req.nama))
                return BadRequest("Nama tidak boleh kosong");

            if (string.IsNullOrWhiteSpace(req.deskripsi))
                return BadRequest("Deskripsi tidak boleh kosong");

            if (!IsSafe(req.code) || !IsSafe(req.nama) || !IsSafe(req.deskripsi))
                return BadRequest("Input mengandung karakter berbahaya");

            try
            {
                var result = AddReferenceData(new ReferenceData()
                {
                    Qualifier = RefDataQualifier.Penilaian,
                    Code = HttpUtility.HtmlEncode(req.code),
                    LocalizedName = HttpUtility.HtmlEncode(req.nama),
                    LocalizedDesc = HttpUtility.HtmlEncode(req.deskripsi)
                });

                return Ok(new
                {
                    message = "Berhasil tambah data",
                    data = result
                });
            }
            catch
            {
                return InternalServerError();
            }
        }

        [HttpPost]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        //[ApiAuthorize]
        public IHttpActionResult AddPertanyaan([FromBody] AddPertanyaan req)
        {
            //return AddReferenceData(new ReferenceData() { Qualifier = RefDataQualifier.Pertanyaan, Code = code, LocalizedName = nama, LocalizedDesc = deskripsi });
            if (req == null)
                return BadRequest("Request tidak valid");

            if (string.IsNullOrWhiteSpace(req.code))
                return BadRequest("Code tidak boleh kosong");

            if (string.IsNullOrWhiteSpace(req.nama))
                return BadRequest("Nama tidak boleh kosong");

            if (string.IsNullOrWhiteSpace(req.deskripsi))
                return BadRequest("Deskripsi tidak boleh kosong");

            if (!IsSafe(req.code) || !IsSafe(req.nama) || !IsSafe(req.deskripsi))
                return BadRequest("Input mengandung karakter berbahaya");

            try
            {
                var result = AddReferenceData(new ReferenceData()
                {
                    Qualifier = RefDataQualifier.Pertanyaan,
                    Code = HttpUtility.HtmlEncode(req.code),
                    LocalizedName = HttpUtility.HtmlEncode(req.nama),
                    LocalizedDesc = HttpUtility.HtmlEncode(req.deskripsi)
                });

                return Ok(new
                {
                    message = "Berhasil tambah data",
                    data = result
                });
            }
            catch
            {
                return InternalServerError();
            }
        }

        [HttpGet]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        //[ApiAuthorize]
        public string AddJenisBelanja(string code, string nama, string deskripsi)
        {
            return AddReferenceData(new ReferenceData() { Qualifier = RefDataQualifier.JenisPembelanjaan, Code = code, LocalizedName = nama, LocalizedDesc = deskripsi });
        }
        [HttpGet]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        //[ApiAuthorize]
        public string AddJenisPekerjaan(string code, string nama, string deskripsi)
        {
            return AddReferenceData(new ReferenceData() { Qualifier = RefDataQualifier.JenisPekerjaan, Code = code, LocalizedName = nama, LocalizedDesc = deskripsi });
        }

        [HttpPost]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        //[ApiAuthorize]
        public IHttpActionResult AddCurrency([FromBody] AddCurrency req)
        {
            //return AddReferenceData(new ReferenceData() { Qualifier = RefDataQualifier.CURRENCY, Code = code, LocalizedName = nama, LocalizedDesc = deskripsi });
            if (req == null)
                return BadRequest("Request tidak valid");

            if (string.IsNullOrWhiteSpace(req.code))
                return BadRequest("Code tidak boleh kosong");

            if (string.IsNullOrWhiteSpace(req.nama))
                return BadRequest("Nama tidak boleh kosong");

            if (string.IsNullOrWhiteSpace(req.deskripsi))
                return BadRequest("Deskripsi tidak boleh kosong");

            if (!IsSafe(req.code) || !IsSafe(req.nama) || !IsSafe(req.deskripsi))
                return BadRequest("Input mengandung karakter berbahaya");

            try
            {
                var result = AddReferenceData(new ReferenceData()
                {
                    Qualifier = RefDataQualifier.CURRENCY,
                    Code = HttpUtility.HtmlEncode(req.code),
                    LocalizedName = HttpUtility.HtmlEncode(req.nama),
                    LocalizedDesc = HttpUtility.HtmlEncode(req.deskripsi)
                });

                return Ok(new
                {
                    message = "Berhasil tambah data",
                    data = result
                });
            }
            catch
            {
                return InternalServerError();
            }
        }

        [HttpPost]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        //[ApiAuthorize]
        public IHttpActionResult AddRegion([FromBody] AddRegion req)
        {
            //return AddReferenceData(new ReferenceData() { Qualifier = RefDataQualifier.REGION, Code = code, LocalizedName = nama, LocalizedDesc = deskripsi });
            if (req == null)
                return BadRequest("Request tidak valid");

            if (string.IsNullOrWhiteSpace(req.code))
                return BadRequest("Code tidak boleh kosong");

            if (string.IsNullOrWhiteSpace(req.nama))
                return BadRequest("Nama tidak boleh kosong");

            if (string.IsNullOrWhiteSpace(req.deskripsi))
                return BadRequest("Deskripsi tidak boleh kosong");

            if (!IsSafe(req.code) || !IsSafe(req.nama) || !IsSafe(req.deskripsi))
                return BadRequest("Input mengandung karakter berbahaya");

            try
            {
                var result = AddReferenceData(new ReferenceData()
                {
                    Qualifier = RefDataQualifier.REGION,
                    Code = HttpUtility.HtmlEncode(req.code),
                    LocalizedName = HttpUtility.HtmlEncode(req.nama),
                    LocalizedDesc = HttpUtility.HtmlEncode(req.deskripsi)
                });

                return Ok(new
                {
                    message = "Berhasil tambah data",
                    data = result
                });
            }
            catch
            {
                return InternalServerError();
            }
        }

        [HttpPost]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        //[ApiAuthorize]
        public IHttpActionResult AddSatuan([FromBody] AddSatuan req)
        {
            //return AddReferenceData(new ReferenceData() { Qualifier = RefDataQualifier.SATUAN, Code = code, LocalizedName = nama, LocalizedDesc = deskripsi });
            if (req == null)
                return BadRequest("Request tidak valid");

            if (string.IsNullOrWhiteSpace(req.code))
                return BadRequest("Code tidak boleh kosong");

            if (string.IsNullOrWhiteSpace(req.nama))
                return BadRequest("Nama tidak boleh kosong");

            if (string.IsNullOrWhiteSpace(req.deskripsi))
                return BadRequest("Deskripsi tidak boleh kosong");

            if (!IsSafe(req.code) || !IsSafe(req.nama) || !IsSafe(req.deskripsi))
                return BadRequest("Input mengandung karakter berbahaya");

            try
            {
                var result = AddReferenceData(new ReferenceData()
                {
                    Qualifier = RefDataQualifier.SATUAN,
                    Code = HttpUtility.HtmlEncode(req.code),
                    LocalizedName = HttpUtility.HtmlEncode(req.nama),
                    LocalizedDesc = HttpUtility.HtmlEncode(req.deskripsi)
                });

                return Ok(new
                {
                    message = "Berhasil tambah data",
                    data = result
                });
            }
            catch
            {
                return InternalServerError();
            }
        }

        [HttpPost]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        //[ApiAuthorize]
        public IHttpActionResult AddBank([FromBody] AddBankRequest req)
        {
            if (req == null)
                return BadRequest("Request tidak valid");

            if (string.IsNullOrWhiteSpace(req.Code))
                return BadRequest("Code tidak boleh kosong");

            if (string.IsNullOrWhiteSpace(req.Nama))
                return BadRequest("Nama tidak boleh kosong");

            if (string.IsNullOrWhiteSpace(req.Deskripsi))
                return BadRequest("Deskripsi tidak boleh kosong");

            if (!IsSafe(req.Code) || !IsSafe(req.Nama) || !IsSafe(req.Deskripsi))
                return BadRequest("Input mengandung karakter berbahaya");

            try
            {
                var result = AddReferenceData(new ReferenceData()
                {
                    Qualifier = RefDataQualifier.BANK,
                    Code = HttpUtility.HtmlEncode(req.Code),
                    LocalizedName = HttpUtility.HtmlEncode(req.Nama),
                    LocalizedDesc = HttpUtility.HtmlEncode(req.Deskripsi)
                });

                return Ok(new
                {
                    message = "Berhasil tambah data",
                    data = result
                });
            }
            catch
            {
                return InternalServerError();
            }
        }

        [HttpGet]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        //[ApiAuthorize]
        public string AddInfoPerusahaan(string code, string nama, string deskripsi)
        {
            return AddReferenceData(new ReferenceData() { Qualifier = RefDataQualifier.InfoPerusahaan, Code = code, LocalizedName = nama, LocalizedDesc = deskripsi });
        }
        [HttpGet]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        //[ApiAuthorize]
        public string AddProvinsi(string code, string nama, string deskripsi)
        {
            return AddReferenceData(new ReferenceData() { Qualifier = RefDataQualifier.PROVINCE, Code = code, LocalizedName = nama, LocalizedDesc = deskripsi });
        }

        [HttpGet]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        public IHttpActionResult EditReferenceData(int id)
        {
            var templateDoc = _modelContext.ReferenceDatas.FirstOrDefault(doc => doc.Id == id);

            if (templateDoc == null)
            {
                return Content(System.Net.HttpStatusCode.NotFound, new { message = "Data tidak ditemukan" });
            }

            return Ok(templateDoc);
        }

        [HttpGet]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        //[ApiAuthorize]
        public IHttpActionResult EditReferenceDataPenilaian(int id)
        {
            var templateDoc = _modelContext.ReferenceDatas.Where(doc => doc.Id == id && doc.Qualifier == RefDataQualifier.Penilaian).FirstOrDefault();

            return Json(templateDoc);
        }

        [HttpPost]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        //[ApiAuthorize]
        public IHttpActionResult SaveEditReferenceData(ReferenceData data)
        {
            //JimbisEncrypt code = new JimbisEncrypt();

            var update = _modelContext.ReferenceDatas.Where(d => d.Id == data.Id).FirstOrDefault();

            if (update == null) return BadRequest("Data not found");

            if (string.IsNullOrWhiteSpace(data.Code)) return BadRequest("Code tidak boleh kosong") ;

            if (string.IsNullOrWhiteSpace(data.LocalizedName)) return BadRequest("Name tidak boleh kosong") ;

            if (string.IsNullOrWhiteSpace(data.LocalizedDesc)) return BadRequest("Description tidak boleh kosong") ;

            //if (string.IsNullOrWhiteSpace(data.StringAttr1)) return BadRequest("Attribute 1 tidak boleh kosong") ;

            if (System.Text.RegularExpressions.Regex.IsMatch(data.Code, @"[<>]"))
                return BadRequest("Input tidak valid (HTML tidak diperbolehkan)");

            if (System.Text.RegularExpressions.Regex.IsMatch(data.LocalizedName, @"[<>]"))
                return BadRequest("Input tidak valid (HTML tidak diperbolehkan)");

            if (System.Text.RegularExpressions.Regex.IsMatch(data.LocalizedDesc, @"[<>]"))
                return BadRequest("Input tidak valid (HTML tidak diperbolehkan)");

            //if (System.Text.RegularExpressions.Regex.IsMatch(data.StringAttr1, @"[<>]"))
            //    return BadRequest("Input tidak valid (HTML tidak diperbolehkan)");

            //update.Qualifier = arrData.Qualifier;
            update.Code = Sanitize(data.Code);
            update.LocalizedName = Sanitize(data.LocalizedName);
            update.LocalizedDesc = Sanitize(data.LocalizedDesc);
            update.StringAttr1 = Sanitize(data.StringAttr1);
            //update.StringAttr2 = arrData.StringAttr2;
            //update.StringAttr3 = arrData.StringAttr3;
            //update.IntAttr1 = arrData.IntAttr1;
            //update.IntAttr2 = arrData.IntAttr2;
            //update.IntAttr3 = arrData.IntAttr3;
            //update.FlagAttr1 = arrData.FlagAttr1;
            //update.FlagAttr2 = arrData.FlagAttr2;
            //update.FlagAttr3 = arrData.FlagAttr3;

            _modelContext.SaveChanges();

            return Json(update.Id);
        }

        [HttpPost]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        public IHttpActionResult DeleteReferenceData([FromBody] DeleteRequest req)
        {
            if (req == null || req.Id <= 0)
                return BadRequest("Id tidak valid");

            try
            {
                var data = _repository.GetDataById(req.Id);

                if (data == null)
                {
                    return NotFound();
                }

                _repository.Delete(data);

                return Ok(new
                {
                    message = "Sukses hapus data"
                });
            }
            catch
            {
                return InternalServerError();
            }
        }

        [HttpGet]
        public IHttpActionResult Get(string qualifier)
        {
            var r = _repository.GetData(qualifier);

            return Json(r);
        }

        [HttpGet]
        public List<ReferenceDataViewModel> GetAllQUALIFIER_INS_BENEF_TYPE()
        {
            return _repository.GetData(RefDataQualifier.QUALIFIER_INS_BENEF_TYPE, null).Select(x => new ReferenceDataViewModel()
            {
                id = x.Id,
                Code = x.Code,
                Name = x.LocalizedName,
                Desc = x.LocalizedDesc,
                Str1 = x.StringAttr1,
                Int1 = x.IntAttr1,
                Flag1 = x.FlagAttr1
            }).ToList();
        }

        [HttpGet]
        //[ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_admin })]
        [ApiAuthorize]
        public IHttpActionResult GetTblQUALIFIER_INS_BENEF_TYPE()
        {
            return Json(new { aaData = GetAllQUALIFIER_INS_BENEF_TYPE() });
        }

        [HttpGet]
        public List<ReferenceDataViewModel> GetAllQUALIFIER_INS_BENEF_COVERAGE()
        {
            return _repository.GetData(RefDataQualifier.QUALIFIER_INS_BENEF_COVERAGE, null).Select(x => new ReferenceDataViewModel()
            {
                id = x.Id,
                Code = x.Code,
                Name = x.LocalizedName,
                Desc = x.LocalizedDesc,
                Str1 = x.StringAttr1,
                Int1 = x.IntAttr1,
                Flag1 = x.FlagAttr1
            }).ToList();
        }

        [HttpGet]
        //[ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_admin })]
        [ApiAuthorize]
        public IHttpActionResult GetTblQUALIFIER_INS_BENEF_COVERAGE()
        {
            return Json(new { aaData = GetAllQUALIFIER_INS_BENEF_COVERAGE() });
        }

        [HttpGet]
        public List<ReferenceDataViewModel> GetAllQUALIFIER_INS_REGION()
        {
            return _repository.GetData(RefDataQualifier.QUALIFIER_INS_REGION, null).Select(x => new ReferenceDataViewModel()
            {
                id = x.Id,
                Code = x.Code,
                Name = x.LocalizedName,
                Desc = x.LocalizedDesc,
                Str1 = x.StringAttr1,
                Int1 = x.IntAttr1,
                Flag1 = x.FlagAttr1
            }).ToList();
        }

        [HttpGet]
        //[ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_admin })]
        [ApiAuthorize]
        public IHttpActionResult GetTblQUALIFIER_INS_REGION()
        {
            return Json(new { aaData = GetAllQUALIFIER_INS_REGION() });
        }

        [HttpGet]
        public List<ReferenceDataViewModel> GetAllQUALIFIER_INS_BENEFIT()
        {
            return _repository.GetData(RefDataQualifier.QUALIFIER_INS_BENEFIT, null).Select(x => new ReferenceDataViewModel()
            {
                id = x.Id,
                Code = x.Code,
                Name = x.LocalizedName,
                Desc = x.LocalizedDesc,
                Str1 = x.StringAttr1,
                Int1 = x.IntAttr1,
                Flag1 = x.FlagAttr1
            }).ToList();
        }

        [HttpGet]
        //[ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_admin })]
        [ApiAuthorize]
        public IHttpActionResult GetTblQUALIFIER_INS_BENEFIT()
        {
            return Json(new { aaData = GetAllQUALIFIER_INS_BENEFIT() });
        }

        [HttpPost]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        //[ApiAuthorize]
        public IHttpActionResult AddQUALIFIER_INS_BENEF_TYPE([FromBody] AddQUALIFIER_INS_BENEF_TYPE req)
        {
            //return AddReferenceData(new ReferenceData() { Qualifier = RefDataQualifier.QUALIFIER_INS_BENEF_TYPE, Code = code, LocalizedName = nama, LocalizedDesc = deskripsi });
            if (req == null)
                return BadRequest("Request tidak valid");

            if (string.IsNullOrWhiteSpace(req.code))
                return BadRequest("Code tidak boleh kosong");

            if (string.IsNullOrWhiteSpace(req.nama))
                return BadRequest("Nama tidak boleh kosong");

            if (string.IsNullOrWhiteSpace(req.deskripsi))
                return BadRequest("Deskripsi tidak boleh kosong");

            if (!IsSafe(req.code) || !IsSafe(req.nama) || !IsSafe(req.deskripsi))
                return BadRequest("Input mengandung karakter berbahaya");

            try
            {
                var result = AddReferenceData(new ReferenceData()
                {
                    Qualifier = RefDataQualifier.QUALIFIER_INS_BENEF_TYPE,
                    Code = HttpUtility.HtmlEncode(req.code),
                    LocalizedName = HttpUtility.HtmlEncode(req.nama),
                    LocalizedDesc = HttpUtility.HtmlEncode(req.deskripsi)
                });

                return Ok(new
                {
                    message = "Berhasil tambah data",
                    data = result
                });
            }
            catch
            {
                return InternalServerError();
            }
        }

        [HttpGet]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        //[ApiAuthorize]
        public string AddQUALIFIER_INS_BENEF_COVERAGE(string Code, string LocalizedName, string LocalizedDesc, string StringAttr1)
        {
            return AddReferenceData(new ReferenceData() { Qualifier = RefDataQualifier.QUALIFIER_INS_BENEF_COVERAGE, Code = Code, LocalizedName = LocalizedName, LocalizedDesc = LocalizedDesc, StringAttr1 = StringAttr1 });
        }

        [HttpPost]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        //[ApiAuthorize]
        public IHttpActionResult AddQUALIFIER_INS_REGION([FromBody] AddQUALIFIER_INS_REGION req)
        {
            //return AddReferenceData(new ReferenceData() { Qualifier = RefDataQualifier.QUALIFIER_INS_REGION, Code = Code, LocalizedName = LocalizedName, LocalizedDesc = LocalizedDesc, StringAttr1 = StringAttr1 });
            if (req == null)
                return BadRequest("Request tidak valid");

            if (string.IsNullOrWhiteSpace(req.Code))
                return BadRequest("Code tidak boleh kosong");

            if (string.IsNullOrWhiteSpace(req.LocalizedName))
                return BadRequest("Nama tidak boleh kosong");

            if (string.IsNullOrWhiteSpace(req.LocalizedDesc))
                return BadRequest("Deskripsi tidak boleh kosong");

            if (string.IsNullOrWhiteSpace(req.StringAttr1))
                return BadRequest("Deskripsi tidak boleh kosong");

            if (!IsSafe(req.Code) || !IsSafe(req.LocalizedName) || !IsSafe(req.LocalizedName) || !IsSafe(req.StringAttr1))
                return BadRequest("Input mengandung karakter berbahaya");

            try
            {
                var result = AddReferenceData(new ReferenceData()
                {
                    Qualifier = RefDataQualifier.QUALIFIER_INS_REGION,
                    Code = HttpUtility.HtmlEncode(req.Code),
                    LocalizedName = HttpUtility.HtmlEncode(req.LocalizedName),
                    LocalizedDesc = HttpUtility.HtmlEncode(req.LocalizedName),
                    StringAttr1 = HttpUtility.HtmlEncode(req.StringAttr1)
                });

                return Ok(new
                {
                    message = "Berhasil tambah data",
                    data = result
                });
            }
            catch
            {
                return InternalServerError();
            }
        }

        [HttpPost]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        //[ApiAuthorize]
        public IHttpActionResult AddQUALIFIER_INS_BENEFIT([FromBody] AddQualifierInsBenefit req)
        {
            //return AddReferenceData(new ReferenceData() { Qualifier = RefDataQualifier.QUALIFIER_INS_BENEFIT, Code = Code, LocalizedName = LocalizedName, LocalizedDesc = LocalizedDesc, StringAttr1 = StringAttr1 });

            if (req == null)
                return BadRequest("Request tidak valid");

            if (string.IsNullOrWhiteSpace(req.Code))
                return BadRequest("Code tidak boleh kosong");

            if (string.IsNullOrWhiteSpace(req.LocalizedName))
                return BadRequest("Nama tidak boleh kosong");

            if (string.IsNullOrWhiteSpace(req.LocalizedDesc))
                return BadRequest("Deskripsi tidak boleh kosong");

            if (string.IsNullOrWhiteSpace(req.StringAttr1))
                return BadRequest("Deskripsi tidak boleh kosong");

            if (!IsSafe(req.Code) || !IsSafe(req.LocalizedName) || !IsSafe(req.LocalizedName) || !IsSafe(req.StringAttr1))
                return BadRequest("Input mengandung karakter berbahaya");

            try
            {
                var result = AddReferenceData(new ReferenceData()
                {
                    Qualifier = RefDataQualifier.QUALIFIER_INS_BENEFIT,
                    Code = HttpUtility.HtmlEncode(req.Code),
                    LocalizedName = HttpUtility.HtmlEncode(req.LocalizedName),
                    LocalizedDesc = HttpUtility.HtmlEncode(req.LocalizedName),
                    StringAttr1 = HttpUtility.HtmlEncode(req.StringAttr1)
                });

                return Ok(new
                {
                    message = "Berhasil tambah data",
                    data = result
                });
            }
            catch
            {
                return InternalServerError();
            }
        }
        /*===============================================================================================================================================================================================================================*/

        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.Roles.pRole_procurement_head, IdLdapConstants.Roles.pRole_procurement_manager })]
        public List<MasterBranchViewModel> GetAllBranch()
        {
            var oBranch = (from b in _modelContext.MasterBranchs
                           join c in _modelContext.ReferenceDatas on b.FK_Prov_Id equals c.Id
                           select new MasterBranchViewModel
                           {
                               Branch_Id = b.Branch_Id,
                               Branch_Code = b.Branch_Code,
                               Branch_Name = b.Branch_Name,
                               Prov_Name = c.LocalizedName
                           }).OrderBy(x => x.Branch_Name).ToList();
            return oBranch;
        }

        [HttpGet]
        //[ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_admin })]
        [ApiAuthorize]
        public IHttpActionResult GetTblBranch()
        {
            return Json(new { aaData = GetAllBranch() });
        }

        [HttpPost]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        public IHttpActionResult AddBranch([FromBody] MasterBranch model)
        {
            if (model == null)
                return BadRequest("Request tidak valid");

            // Sanitasi input
            model.Branch_Code = Sanitize(model.Branch_Code);
            model.Branch_Name = Sanitize(model.Branch_Name);

            if (string.IsNullOrWhiteSpace(model.Branch_Name))
                return BadRequest("Nama cabang wajib diisi");

            var result = SaveBranch(model);

            if (result.StartsWith("Gagal"))
                return BadRequest(result);

            return Ok(result);
        }

        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        //[ApiAuthorize]
        private string SaveBranch(MasterBranch r)
        {
            var lr = _repository.GetDataBranch(r.Branch_Code, r.Branch_Name, r.FK_Prov_Id);

            if (lr.Count > 0)
            {
                return "Gagal! Duplikat data.";
            }

            _repository.SaveDataBranch(r);
            return "Sukses!";
        }

        [HttpGet]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        //[ApiAuthorize]
        public IHttpActionResult EditBranch(int id)
        {
            var templateDoc = _modelContext.MasterBranchs.Where(d => d.Branch_Id == id).FirstOrDefault();

            return Json(templateDoc);
        }

        [HttpPost]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        //[ApiAuthorize]
        public IHttpActionResult SaveEditBranch(MasterBranch data)
        {
            //JimbisEncrypt code = new JimbisEncrypt();

            var update = _modelContext.MasterBranchs.Where(d => d.Branch_Id == data.Branch_Id).FirstOrDefault();

            //update.Qualifier = arrData.Qualifier;
            update.Branch_Code = data.Branch_Code;
            update.Branch_Name = data.Branch_Name;
            update.FK_Prov_Id = data.FK_Prov_Id;

            _modelContext.SaveChanges();

            return Json(update.Branch_Id);
        }

        [HttpPost]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        public IHttpActionResult DeleteBranch(int id)
        {
            MasterBranch r = _repository.GetDataByIdBranch(id);

            if (r == null)
            {
                return Content(System.Net.HttpStatusCode.NotFound, new { message = "Gagal! Tidak ditemukan." });
            }

            _repository.DeleteBranch(r);

            return Ok(new { message = "Sukses!" });
        }

        /*===============================================================================================================================================================================================================================*/

        [HttpGet]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_admin })]
        public List<MasterDepartmentViewModel> GetAllDepartment()
        {
            return _repository.GetDepartment().Select(x => new MasterDepartmentViewModel()
            {
                Department_Id = x.Department_Id,
                Department_Code = x.Department_Code,
                Department_Name = x.Department_Name
            }).OrderBy(x => x.Department_Name).ToList();
        }

        [HttpGet]
        public List<MasterDepartmentViewModel> GetAllDepartment(string branch)
        {
            return _repository.GetDepartmentWithBranch(branch).Select(x => new MasterDepartmentViewModel()
            {
                Department_Id = x.Department_Id,
                Department_Code = x.Department_Code,
                Department_Name = x.Department_Name
            }).OrderBy(x => x.Department_Name).Distinct().ToList();
        }

        [HttpGet]
        public List<MasterDepartmentViewModel> GetAllDepartmentPengadaanAdd(string branch)
        {
            return _repository.GetDepartmentWithBranch(branch).Select(x => new MasterDepartmentViewModel()
            {
                Department_Id = x.Department_Id,
                Department_Code = x.Department_Code,
                Department_Name = x.Department_Name
            }).OrderBy(x => x.Department_Name).Distinct().ToList();
        }

        [HttpGet]
        //[ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_admin })]
        [ApiAuthorize]
        public IHttpActionResult GetTblDepartment()
        {
            return Json(new { aaData = GetAllDepartment() });
        }

        [HttpGet]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        //[ApiAuthorize]
        public string AddDepartment(string Department_Code, string Department_Name)
        {
            return SaveDepartment(new MasterDepartment() { Department_Code = Department_Code, Department_Name = Department_Name });
        }

        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        //[ApiAuthorize]
        private string SaveDepartment(MasterDepartment r)
        {
            List<MasterDepartment> lr = _repository.GetDataDepartment(r.Department_Code, r.Department_Name);
            if (lr.Count > 0) //exist boss
            {
                return "Gagal! Duplikat data.";
            }
            _repository.SaveDataDepartment(r);
            return "Sukses!";
        }

        [HttpGet]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        //[ApiAuthorize]
        public IHttpActionResult EditDepartment(int id)
        {
            var templateDoc = _modelContext.MasterDepartments.Where(d => d.Department_Id == id).FirstOrDefault();

            return Json(templateDoc);
        }

        [HttpPost]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        //[ApiAuthorize]
        public IHttpActionResult SaveEditDepartment(MasterDepartment data)
        {
            //JimbisEncrypt code = new JimbisEncrypt();

            var update = _modelContext.MasterDepartments.Where(d => d.Department_Id == data.Department_Id).FirstOrDefault();

            //update.Qualifier = arrData.Qualifier;
            update.Department_Code = data.Department_Code;
            update.Department_Name = data.Department_Name;

            _modelContext.SaveChanges();

            return Json(update.Department_Id);
        }

        //[HttpGet]
        //[ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        ////[ApiAuthorize]
        //public string DeleteDepartment(int id)
        //{
        //    MasterDepartment r = _repository.GetDataByIdDepartment(id);
        //    if (r != null)
        //    {
        //        _repository.DeleteDepartment(r);
        //        return "Sukses!";
        //    }
        //    return "Gagal! Tidak ditemukan.";
        //}

        [HttpPost]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        public IHttpActionResult DeleteDepartment(int id)
        {
            MasterDepartment r = _repository.GetDataByIdDepartment(id);
            if (r != null)
            {
                _repository.DeleteDepartment(r);
                return Ok(new { message = "Sukses!" });
            }
            return Content(HttpStatusCode.NotFound, new { message = "Gagal! Tidak ditemukan." });
        }

        /*===============================================================================================================================================================================================================================*/

        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.Roles.pRole_procurement_head, IdLdapConstants.Roles.pRole_procurement_manager })]
        public List<MasterBranchDepartmentRelationshipViewModel> GetAllBranchDepartment(int Id)
        {
            var oBranchDepartment = (from b in _modelContext.MasterBranchDepartmentRelationships
                                     join c in _modelContext.MasterBranchs on b.FK_Branch_Id equals c.Branch_Id
                                     join d in _modelContext.MasterDepartments on b.FK_Department_Id equals d.Department_Id
                                     where Id == c.Branch_Id
                                     select new MasterBranchDepartmentRelationshipViewModel
                                     {
                                         Id = b.Id,
                                         FK_Branch_Id = c.Branch_Name,
                                         FK_Department_Id = d.Department_Name,
                                         Branch_Id = c.Branch_Id,
                                         Department_Id = d.Department_Id
                                     }).Distinct().ToList();
            return oBranchDepartment;
        }

        [HttpGet]
        //[ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_admin })]
        [ApiAuthorize]
        public IHttpActionResult GetTblBranchDepartment(int Id)
        {
            return Json(new { aaData = GetAllBranchDepartment(Id) });
        }

        [HttpGet]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        //[ApiAuthorize]
        public string AddDepartmentBranch(int Branch_Id, int Department_Id)
        {
            return SaveDepartmentBranch(new MasterBranchDepartmentRelationship() { FK_Branch_Id = Branch_Id, FK_Department_Id = Department_Id });
        }

        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        //[ApiAuthorize]
        private string SaveDepartmentBranch(MasterBranchDepartmentRelationship r)
        {
            List<MasterBranchDepartmentRelationship> lr = _repository.GetListDataByIdDepartmentBranch(r.FK_Branch_Id, r.FK_Department_Id);
            if (lr.Count > 0) //exist boss
            {
                return "Gagal! Duplikat data.";
            }
            _repository.SaveDataDepartmentBranch(r);
            return "Sukses!";
        }

        [HttpGet]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        //[ApiAuthorize]
        public IHttpActionResult EditDepartmentBranch(int idbranch, int iddept)
        {
            var templateDoc = _modelContext.MasterBranchDepartmentRelationships.Where(d => d.FK_Branch_Id == idbranch && d.FK_Department_Id == iddept).FirstOrDefault();

            return Json(templateDoc);
        }

        [HttpPost]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        //[ApiAuthorize]
        public IHttpActionResult SaveEditDepartmentBranch(MasterBranchDepartmentRelationship data)
        {
            //JimbisEncrypt code = new JimbisEncrypt();

            var update = _modelContext.MasterBranchDepartmentRelationships.Where(d => d.Id == data.Id).FirstOrDefault();

            //update.Qualifier = arrData.Qualifier;
            update.FK_Department_Id = data.FK_Department_Id;

            _modelContext.SaveChanges();

            return Json(update.Id);
        }

        [HttpPost]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        public IHttpActionResult DeleteDepartmentBranch(int id)
        {
            var r = _repository.GetDataByIdDepartmentBranch(id);

            if (r == null)
            {
                return Content(System.Net.HttpStatusCode.NotFound, new { message = "Gagal! Tidak ditemukan." });
            }

            _repository.DeleteDepartmentBranch(r);

            return Ok(new { message = "Sukses!" });
        }

        [HttpGet]
        //[ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_admin})]
        [ApiAuthorize]
        public IHttpActionResult GetTblPertanyaanPenilaianRekanan()
        {
            return Json(new { aaData = GetAllPertanyaanPenilaianRekanan() });
        }

        [HttpGet]
        public List<ReferenceDataViewModel> GetAllPertanyaanPenilaianRekanan()
        {
            return _repository.GetData(RefDataQualifier.QUALIFIER_VENDOR_CRITERIA, null).Select(x => new ReferenceDataViewModel()
            {
                id = x.Id,
                Code = x.Code,
                Name = x.LocalizedName,
                Desc = x.LocalizedDesc,
                Str1 = x.StringAttr1,
                Int1 = x.IntAttr1,
                Flag1 = x.FlagAttr1
            }).ToList();
        }

        [HttpGet]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        //[ApiAuthorize]
        public string AddPertanyaanPenilaianRekanan(string code, string nama, string deskripsi)
        {
            return AddReferenceData(new ReferenceData() { Qualifier = RefDataQualifier.QUALIFIER_VENDOR_CRITERIA, Code = code, LocalizedName = nama, LocalizedDesc = deskripsi });
        }

        [HttpGet]
        //[ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_admin})]
        [ApiAuthorize]
        public IHttpActionResult GetTblBidangUsaha()
        {
            return Json(new { aaData = GetAllBidangUsaha() });
        }

        [HttpGet]
        public List<ReferenceDataViewModel> GetAllBidangUsaha()
        {
            return _repository.GetData(RefDataQualifier.SegBidangUsahaCode, null).Select(x => new ReferenceDataViewModel()
            {
                id = x.Id,
                Code = x.Code,
                Name = x.LocalizedName,
                Desc = x.LocalizedDesc,
                Str1 = x.StringAttr1,
                Int1 = x.IntAttr1,
                Flag1 = x.FlagAttr1
            }).ToList();
        }

        [HttpGet]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        //[ApiAuthorize]
        public string AddBidangUsaha(string code, string nama, string deskripsi)
        {
            return AddReferenceData(new ReferenceData() { Qualifier = RefDataQualifier.SegBidangUsahaCode, Code = code, LocalizedName = nama, LocalizedDesc = deskripsi });
        }

        [HttpGet]
        //[ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_admin})]
        [ApiAuthorize]
        public IHttpActionResult GetTblKelompokUsaha()
        {
            return Json(new { aaData = GetAllKelompokUsaha() });
        }

        [HttpGet]
        public List<ReferenceDataViewModel> GetAllKelompokUsaha()
        {
            return _repository.GetData(RefDataQualifier.segKlmpkUsahaCode, null).Select(x => new ReferenceDataViewModel()
            {
                id = x.Id,
                Code = x.Code,
                Name = x.LocalizedName,
                Desc = x.LocalizedDesc,
                Str1 = _repository.GetDataAdditional(RefDataQualifier.SegBidangUsahaCode, x.StringAttr1),
                Int1 = x.IntAttr1,
                Flag1 = x.FlagAttr1
            }).ToList();
        }

        [HttpGet]
        //[ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_admin})]
        [ApiAuthorize]
        public IHttpActionResult GetTblSubBidangUsaha()
        {
            return Json(new { aaData = GetAllSubBidangUsaha() });
        }

        [HttpGet]
        public List<ReferenceDataViewModel> GetAllSubBidangUsaha()
        {
            return _repository.GetData(RefDataQualifier.SegSubBidangUsahaCode, null).Select(x => new ReferenceDataViewModel()
            {
                id = x.Id,
                Code = x.Code,
                Name = x.LocalizedName,
                Desc = x.LocalizedDesc,
                Str1 = _repository.GetDataAdditional(RefDataQualifier.segKlmpkUsahaCode, x.StringAttr1),
                Int1 = x.IntAttr1,
                Flag1 = x.FlagAttr1
            }).ToList();
        }

        [HttpGet]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        //[ApiAuthorize]
        public string AddKelompokUsaha(string Code, string LocalizedName, string LocalizedDesc, string StringAttr1)
        {
            return AddReferenceData(new ReferenceData() { Qualifier = RefDataQualifier.segKlmpkUsahaCode, Code = Code, LocalizedName = LocalizedName, LocalizedDesc = LocalizedDesc, StringAttr1 = StringAttr1 });
        }

        [HttpGet]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        //[ApiAuthorize]
        public string AddSubBidangUsaha(string Code, string LocalizedName, string LocalizedDesc, string StringAttr1)
        {
            return AddReferenceData(new ReferenceData() { Qualifier = RefDataQualifier.SegSubBidangUsahaCode, Code = Code, LocalizedName = LocalizedName, LocalizedDesc = LocalizedDesc, StringAttr1 = StringAttr1 });
        }

        [HttpGet]
        //[ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_admin})]
        [ApiAuthorize]
        public IHttpActionResult GetTblDukcapilNegara()
        {
            return Json(new { aaData = GetAllDukcapilNegara() });
        }

        [HttpGet]
        public List<ReferenceDataViewModel> GetAllDukcapilNegara()
        {
            return _repository.GetData(RefDataQualifier.COUNTRY, null).Select(x => new ReferenceDataViewModel()
            {
                id = x.Id,
                Code = x.Code,
                Name = x.LocalizedName,
                Desc = x.LocalizedDesc,
                Str1 = "",//_repository.GetDataAdditional(RefDataQualifier.SegBidangUsahaCode, x.StringAttr1),
                Int1 = x.IntAttr1,
                Flag1 = x.FlagAttr1
            }).ToList();
        }

        [HttpGet]
        //[ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_admin})]
        [ApiAuthorize]
        public IHttpActionResult GetTblDukcapilProvinsi()
        {
            return Json(new { aaData = GetAllDukcapilProvinsi() });
        }

        [HttpGet]
        public List<ReferenceDataViewModel> GetAllDukcapilProvinsi()
        {
            return _repository.GetData(RefDataQualifier.DUKCAPILPROV, null).Select(x => new ReferenceDataViewModel()
            {
                id = x.Id,
                Code = x.Code,
                Name = x.LocalizedName,
                Desc = x.LocalizedDesc,
                Str1 = "",//_repository.GetDataAdditional(RefDataQualifier.SegBidangUsahaCode, x.StringAttr1),
                Int1 = x.IntAttr1,
                Flag1 = x.FlagAttr1
            }).ToList();
        }

        [HttpGet]
        //[ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_admin})]
        [ApiAuthorize]
        public IHttpActionResult GetTblDukcapilKota()
        {
            return Json(new { aaData = GetAllDukcapilKota() });
        }

        [HttpGet]
        public List<ReferenceDataViewModel> GetAllDukcapilKota()
        {
            return _repository.GetData(RefDataQualifier.DUKCAPILKOTA, null).Select(x => new ReferenceDataViewModel()
            {
                id = x.Id,
                Code = x.Code,
                Name = x.LocalizedName,
                Desc = x.LocalizedDesc,
                Str1 = _repository.GetDataAdditional(RefDataQualifier.DUKCAPILPROV, x.StringAttr1),
                Int1 = x.IntAttr1,
                Flag1 = x.FlagAttr1
            }).ToList();
        }

        [HttpGet]
        //[ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_admin})]
        [ApiAuthorize]
        public IHttpActionResult GetTblDukcapilKecamatan()
        {
            return Json(new { aaData = GetAllDukcapilKecamatan() });
        }

        [HttpGet]
        public List<ReferenceDataViewModel> GetAllDukcapilKecamatan()
        {
            return _repository.GetData(RefDataQualifier.DUKCAPILKECAMATAN, null).Select(x => new ReferenceDataViewModel()
            {
                id = x.Id,
                Code = x.Code,
                Name = x.LocalizedName,
                Desc = x.LocalizedDesc,
                Str1 = _repository.GetDataAdditional(RefDataQualifier.DUKCAPILKOTA, x.StringAttr1),
                Int1 = x.IntAttr1,
                Flag1 = x.FlagAttr1
            }).ToList();
        }

        [HttpGet]
        //[ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_admin})]
        [ApiAuthorize]
        public IHttpActionResult GetTblDukcapilKelurahan()
        {
            return Json(new { aaData = GetAllDukcapilKelurahan() });
        }

        [HttpGet]
        public List<ReferenceDataViewModel> GetAllDukcapilKelurahan()
        {
            return _repository.GetData(RefDataQualifier.DUKCAPILKELURAHAN, null).Select(x => new ReferenceDataViewModel()
            {
                id = x.Id,
                Code = x.Code,
                Name = x.LocalizedName,
                Desc = x.LocalizedDesc,
                Str1 = _repository.GetDataAdditional(RefDataQualifier.DUKCAPILKECAMATAN, x.StringAttr1),
                Int1 = x.IntAttr1,
                Flag1 = x.FlagAttr1
            }).ToList();
        }

        [HttpGet]
        //[ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_admin})]
        [ApiAuthorize]
        public IHttpActionResult GetTblDukcapilPos()
        {
            return Json(new { aaData = GetAllDukcapilPos() });
        }

        [HttpGet]
        public List<ReferenceDataViewModel> GetAllDukcapilPos()
        {
            return _repository.GetData(RefDataQualifier.DUKCAPILPOS, null).Select(x => new ReferenceDataViewModel()
            {
                id = x.Id,
                Code = x.Code,
                Name = x.LocalizedName,
                Desc = x.LocalizedDesc,
                Str1 = _repository.GetDataAdditional(RefDataQualifier.DUKCAPILKELURAHAN, x.StringAttr1),
                Int1 = x.IntAttr1,
                Flag1 = x.FlagAttr1
            }).ToList();
        }

        [HttpPost]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        //[ApiAuthorize]
        public IHttpActionResult AddDukcapilNegara([FromBody] AddDukcapilNegara req)
        {
            //return AddReferenceData(new ReferenceData() { Qualifier = RefDataQualifier.COUNTRY, Code = code, LocalizedName = nama, LocalizedDesc = deskripsi });
            if (req == null)
                return BadRequest("Request tidak valid");

            if (string.IsNullOrWhiteSpace(req.code))
                return BadRequest("Code tidak boleh kosong");

            if (string.IsNullOrWhiteSpace(req.nama))
                return BadRequest("Nama tidak boleh kosong");

            if (string.IsNullOrWhiteSpace(req.deskripsi))
                return BadRequest("Deskripsi tidak boleh kosong");

            if (!IsSafe(req.code) || !IsSafe(req.nama) || !IsSafe(req.deskripsi))
                return BadRequest("Input mengandung karakter berbahaya");

            try
            {
                var result = AddReferenceData(new ReferenceData()
                {
                    Qualifier = RefDataQualifier.COUNTRY,
                    Code = HttpUtility.HtmlEncode(req.code),
                    LocalizedName = HttpUtility.HtmlEncode(req.nama),
                    LocalizedDesc = HttpUtility.HtmlEncode(req.deskripsi)
                });

                return Ok(new
                {
                    message = "Berhasil tambah data",
                    data = result
                });
            }
            catch
            {
                return InternalServerError();
            }
        }

        [HttpPost]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        //[ApiAuthorize]
        public IHttpActionResult AddDukcapilProvinsi([FromBody] AddDukcapilProvinsi req)
        {
            //return AddReferenceData(new ReferenceData() { Qualifier = RefDataQualifier.DUKCAPILPROV, Code = code, LocalizedName = nama, LocalizedDesc = deskripsi });
            if (req == null)
                return BadRequest("Request tidak valid");

            if (string.IsNullOrWhiteSpace(req.code))
                return BadRequest("Code tidak boleh kosong");

            if (string.IsNullOrWhiteSpace(req.nama))
                return BadRequest("Nama tidak boleh kosong");

            if (string.IsNullOrWhiteSpace(req.deskripsi))
                return BadRequest("Deskripsi tidak boleh kosong");

            if (!IsSafe(req.code) || !IsSafe(req.nama) || !IsSafe(req.deskripsi))
                return BadRequest("Input mengandung karakter berbahaya");

            try
            {
                var result = AddReferenceData(new ReferenceData()
                {
                    Qualifier = RefDataQualifier.DUKCAPILPROV,
                    Code = HttpUtility.HtmlEncode(req.code),
                    LocalizedName = HttpUtility.HtmlEncode(req.nama),
                    LocalizedDesc = HttpUtility.HtmlEncode(req.deskripsi)
                });

                return Ok(new
                {
                    message = "Berhasil tambah data",
                    data = result
                });
            }
            catch
            {
                return InternalServerError();
            }
        }

        [HttpPost]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        //[ApiAuthorize]
        public IHttpActionResult AddDukcapilKota([FromBody] AddDukcapilKota req)
        {
            //return AddReferenceData(new ReferenceData() { Qualifier = RefDataQualifier.DUKCAPILKOTA, Code = Code, LocalizedName = LocalizedName, LocalizedDesc = LocalizedDesc, StringAttr1 = StringAttr1 });
            if (req == null)
                return BadRequest("Request tidak valid");

            if (string.IsNullOrWhiteSpace(req.Code))
                return BadRequest("Code tidak boleh kosong");

            if (string.IsNullOrWhiteSpace(req.LocalizedName))
                return BadRequest("Nama tidak boleh kosong");

            if (string.IsNullOrWhiteSpace(req.LocalizedDesc))
                return BadRequest("Deskripsi tidak boleh kosong");

            if (string.IsNullOrWhiteSpace(req.StringAttr1))
                return BadRequest("Deskripsi tidak boleh kosong");

            if (!IsSafe(req.Code) || !IsSafe(req.LocalizedName) || !IsSafe(req.LocalizedName) || !IsSafe(req.StringAttr1))
                return BadRequest("Input mengandung karakter berbahaya");

            try
            {
                var result = AddReferenceData(new ReferenceData()
                {
                    Qualifier = RefDataQualifier.DUKCAPILKOTA,
                    Code = HttpUtility.HtmlEncode(req.Code),
                    LocalizedName = HttpUtility.HtmlEncode(req.LocalizedName),
                    LocalizedDesc = HttpUtility.HtmlEncode(req.LocalizedName),
                    StringAttr1 = HttpUtility.HtmlEncode(req.StringAttr1)
                });

                return Ok(new
                {
                    message = "Berhasil tambah data",
                    data = result
                });
            }
            catch
            {
                return InternalServerError();
            }
        }

        [HttpPost]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        //[ApiAuthorize]
        public IHttpActionResult AddDukcapilKecamatan([FromBody] AddDukcapilKecamatan req)
        {
            //return AddReferenceData(new ReferenceData() { Qualifier = RefDataQualifier.DUKCAPILKECAMATAN, Code = Code, LocalizedName = LocalizedName, LocalizedDesc = LocalizedDesc, StringAttr1 = StringAttr1 });
            if (req == null)
                return BadRequest("Request tidak valid");

            if (string.IsNullOrWhiteSpace(req.Code))
                return BadRequest("Code tidak boleh kosong");

            if (string.IsNullOrWhiteSpace(req.LocalizedName))
                return BadRequest("Nama tidak boleh kosong");

            if (string.IsNullOrWhiteSpace(req.LocalizedDesc))
                return BadRequest("Deskripsi tidak boleh kosong");

            if (string.IsNullOrWhiteSpace(req.StringAttr1))
                return BadRequest("Deskripsi tidak boleh kosong");

            if (!IsSafe(req.Code) || !IsSafe(req.LocalizedName) || !IsSafe(req.LocalizedName) || !IsSafe(req.StringAttr1))
                return BadRequest("Input mengandung karakter berbahaya");

            try
            {
                var result = AddReferenceData(new ReferenceData()
                {
                    Qualifier = RefDataQualifier.DUKCAPILKECAMATAN,
                    Code = HttpUtility.HtmlEncode(req.Code),
                    LocalizedName = HttpUtility.HtmlEncode(req.LocalizedName),
                    LocalizedDesc = HttpUtility.HtmlEncode(req.LocalizedName),
                    StringAttr1 = HttpUtility.HtmlEncode(req.StringAttr1)
                });

                return Ok(new
                {
                    message = "Berhasil tambah data",
                    data = result
                });
            }
            catch
            {
                return InternalServerError();
            }
        }

        [HttpGet]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        //[ApiAuthorize]
        public string AddDukcapilKelurahan(string Code, string LocalizedName, string LocalizedDesc, string StringAttr1)
        {
            return AddReferenceData(new ReferenceData() { Qualifier = RefDataQualifier.DUKCAPILKELURAHAN, Code = Code, LocalizedName = LocalizedName, LocalizedDesc = LocalizedDesc, StringAttr1 = StringAttr1 });
        }

        [HttpGet]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        //[ApiAuthorize]
        public string AddDukcapilPos(string Code, string LocalizedName, string LocalizedDesc, string StringAttr1)
        {
            return AddReferenceData(new ReferenceData() { Qualifier = RefDataQualifier.DUKCAPILPOS, Code = Code, LocalizedName = LocalizedName, LocalizedDesc = LocalizedDesc, StringAttr1 = StringAttr1 });
        }

        [HttpGet]
        public string GetPolicyPendaftaran()
        {
            var localPath = "\\View\\kebijakan-pendaftaran.html";
            try
            {
                var file = HttpContext.Current.Server.MapPath(localPath);
                return File.ReadAllText(file);
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        //[HttpPost]
        //[ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        //public IHttpActionResult UpdatePolicyPendaftaran([FromBody] JsonContent data)
        //{
        //    var localPath = "\\View\\kebijakan-pendaftaran.html";
        //    try
        //    {
        //        Regex rRemScript = new Regex(@"<script[^>]*>[\s\S]*?</script>");
        //        string output = rRemScript.Replace(data.Body, "");
        //        File.WriteAllText(HttpContext.Current.Server.MapPath(localPath), output);
        //        return Json(new { status = 1, message = "OK" });
        //    }
        //    catch (Exception e)
        //    {
        //        return Json(new { status = 0, message = "NotOK" });
        //    }
        //}
        
        [HttpPost]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        public IHttpActionResult UpdatePolicyPendaftaran([FromBody] string data)
        {
            var localPath = "\\View\\kebijakan-pendaftaran.html";
            try
            {
                Regex rRemScript = new Regex(@"<script[^>]*>[\s\S]*?</script>");
                string output = rRemScript.Replace(data, "");
                File.WriteAllText(HttpContext.Current.Server.MapPath(localPath), data);
                return Json(new { status = 1, message = data });
            }
            catch (Exception e)
            {
                return Json(new { status = 0, message = "NotOK" });
            }
        }

        public class JsonContent
        {
            public string Body { get; set; }
        } 

    }
}
