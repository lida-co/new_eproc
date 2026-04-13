using Microsoft.Owin.FileSystems;
using Model.Helper;
using Reston.Pinata.Model;
using Reston.Pinata.Model.Asuransi;
using Reston.Pinata.Model.JimbisModel;
using Reston.Pinata.Model.Repository;
using Reston.Pinata.WebService.Helper;
using Reston.Pinata.WebService.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Reston.Pinata.WebService
{
    public class ProdukController : BaseController
    {
        private AppDbContext _modelContext;
        private IProdukRepo _repository;
        private IAsuransiRepo _repositoryAsuransi;

        //fchr
        private string Sanitize(string input)
        {
            return HttpUtility.HtmlEncode(input);
        }

        private bool ContainsDangerousInput(string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            return System.Text.RegularExpressions.Regex.IsMatch(input, @"[<>]");
        }
        //end fchr

        public ProdukController()
        {
            _repository = new ProdukRepo(new AppDbContext());
            _repositoryAsuransi = new AsuransiRepo(new AppDbContext());
            _modelContext = new AppDbContext();
        }

        public ProdukController(ProdukRepo repository)
        {
            _repository = repository;
            _repositoryAsuransi = new AsuransiRepo(new AppDbContext());
        }

        public ProdukController(AsuransiRepo repository)
        {
            _repositoryAsuransi = new AsuransiRepo(new AppDbContext());
        }

        public IEnumerable<string> Get()
        {
            return new[] { "Produk is so", "more of it", "more" };
        }

        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.Roles.pRole_procurement_head, IdLdapConstants.Roles.pRole_procurement_manager })]
        public Produk GetProduk(int id)
        {
            return _repository.GetProduk(id) ?? null;
        }

        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.Roles.pRole_procurement_head, IdLdapConstants.Roles.pRole_procurement_manager })]
        public IHttpActionResult GetAllProduk(int draw, int length, int start, string name, string region, string kategori,string klasifikasi)
        {
            //System.Web.HttpContext.Current.Request["a"].ToString();

            int total = _repository.GetAllProduk().Count();
            List<Produk> lp = _repository.GetProduks(name, region, kategori,klasifikasi);
            lp = lp.Where(x => (region == null || (x.RiwayatHarga.LastOrDefault() != null && x.RiwayatHarga.LastOrDefault().Region == region))).ToList();
            List<ProdukSummaryViewModel> lsm = (from a in lp
                                                where (name == null || a.Nama.ToLower().Contains(name.ToLower()))
                                                select new ProdukSummaryViewModel()
                                                {
                                                    Id = a.Id,
                                                    Nama = a.Nama,
                                                    Klasifikasi=a.Klasifikasi.ToString(),
                                                    Price = a.RiwayatHarga.LastOrDefault() != null ? a.RiwayatHarga.LastOrDefault().Harga : 0,
                                                    Region = a.RiwayatHarga.LastOrDefault() != null ? a.RiwayatHarga.LastOrDefault().Region : "",
                                                    LastUpdate = a.RiwayatHarga.LastOrDefault() != null ? a.RiwayatHarga.LastOrDefault().Tanggal.ToLocalTime().ToShortDateString() : "",
                                                    Source = a.RiwayatHarga.LastOrDefault() != null ? a.RiwayatHarga.LastOrDefault().Sumber : "",
                                                    Satuan = a.Satuan
                                                }).Skip(start).Take(length).ToList();
            return Json(new { aaData = lsm, recordsTotal = total, recordsFiltered = lp.Count });
        }

        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.Roles.pRole_procurement_head, IdLdapConstants.Roles.pRole_procurement_manager })]
        public List<string> GetAllKategori()
        {
            return _repository.GetAllKategoriException2().Select(x => x.Nama).Distinct().ToList();
            //List<KategoriSpesifikasi> lstRiwyat = new List<KategoriSpesifikasi>();
            //try
            //{
            //    var kategori = _repository.lstRiwayatDokumen(Id).OrderBy(d => d.ActionDate);
            //    foreach (var item in riwayat)
            //    {
            //        var userx = new Userx();
            //        if (item.UserId != null)
            //            userx = await userDetail(item.UserId.ToString());
            //        VWRiwayatDokumen nVWRiwayatDokumen = new VWRiwayatDokumen();
            //        nVWRiwayatDokumen.Id = item.Id;
            //        nVWRiwayatDokumen.Nama = userx.Nama;
            //        nVWRiwayatDokumen.ActionDate = item.ActionDate;
            //        nVWRiwayatDokumen.Status = item.Status;
            //        nVWRiwayatDokumen.Comment = item.Comment;
            //        lstRiwyat.Add(nVWRiwayatDokumen);
            //    }
            //}
            //catch
            //{

            //}
            //return lstRiwyat;
        }

        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.Roles.pRole_procurement_head, IdLdapConstants.Roles.pRole_procurement_manager })]
        public List<KategoriSpesifikasiViewModel> GetAllKategoriMaster()
        {
            //var update = _repository.GetAllKategori().Where(d => d.Id == id).FirstOrDefault();
            return _repository.GetAllKategoriException2().Select(x => new KategoriSpesifikasiViewModel()
            //return _repository.GetAllKategori().Select(x => new KategoriSpesifikasiViewModel()
            {
                id = x.Id,
                NamaKategori = x.Nama,
                DeskripsiKategori = x.Deskripsi
            }).ToList();
        }

        [HttpGet]
        //[ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_admin })]
        [ApiAuthorize]
        public IHttpActionResult GetTblKategoriMaster()
        {
            return Json(new { aaData = GetAllKategoriMaster() });
        }

        [ApiAuthorize(new string[] {
    IdLdapConstants.Roles.pRole_procurement_user,
    IdLdapConstants.Roles.pRole_procurement_admin,
    IdLdapConstants.Roles.pRole_procurement_head,
    IdLdapConstants.Roles.pRole_procurement_manager
})]
        [HttpDelete]
        public IHttpActionResult DeleteProduk([FromBody] int? id)
        {
            if (id == null)
            {
                return BadRequest("ID tidak boleh null");
            }

            _repository.DeleteProduk(id.Value);

            return Ok(new { message = "Berhasil dihapus" });
        }

        //[HttpGet]
        //[ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        ////[ApiAuthorize]
        //public IHttpActionResult DeleteKategori(int id)
        //{
        //    //KategoriSpesifikasi r = _repository.GetKategoriSpesifikasi(id);
        //    //if (r != null)
        //    //{
        //    //    _repository.DeleteKategori(r);
        //    //    return "Sukses!";
        //    //}
        //    //return "Gagal! Tidak ditemukan.";
        //    {
        //        //JimbisEncrypt code = new JimbisEncrypt();

        //        //var user = db.users.Where(m => m.isNew == false).FirstOrDefault();
        //        //user.isNew = true

        //        var update = _repository.GetAllKategori().Where(d => d.Id == id).FirstOrDefault();

        //        update.IsDeleted = 1;

        //        _repository.Save();

        //        return Json(update.IsDeleted);
        //    }
        //}

        [HttpPost]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        public IHttpActionResult DeleteKategori(int id)
        {
            var update = _repository.GetAllKategori().FirstOrDefault(d => d.Id == id);

            if (update == null)
            {
                return Content(System.Net.HttpStatusCode.NotFound, new { message = "Data tidak ditemukan" });
            }

            update.IsDeleted = 1;
            _repository.Save();

            return Ok(new { message = "Sukses!", isDeleted = update.IsDeleted });
        }

        [HttpPost]
        [ApiAuthorize(new string[] { 
            IdLdapConstants.Roles.pRole_procurement_user, 
            IdLdapConstants.Roles.pRole_procurement_admin, 
            IdLdapConstants.Roles.pRole_procurement_head, 
            IdLdapConstants.Roles.pRole_procurement_manager 
        })]
        public IHttpActionResult SaveProduk([FromBody] ProdukViewModel model)
        {
            if (model == null)
                return BadRequest("Request tidak valid");

            if (string.IsNullOrWhiteSpace(model.Nama))
                return BadRequest("Nama produk wajib diisi");

            if (ContainsDangerousInput(model.Nama) || ContainsDangerousInput(model.Deskripsi) || ContainsDangerousInput(model.Satuan) )
            {
                return BadRequest("Input mengandung karakter berbahaya");
            }

            try
            {
                var p = new Produk
                {
                    Nama = model.Nama,
                    Deskripsi = model.Deskripsi,
                    Satuan = model.Satuan,
                    Klasifikasi = model.Klasifikasi
                };

                if (model.Spesifikasi != null)
                {
                    if (ContainsDangerousInput(model.Spesifikasi.NamaKategori))
                        return BadRequest("Nama kategori tidak valid");

                    var ks = new KategoriSpesifikasi
                    {
                        Nama = model.Spesifikasi.NamaKategori
                    };

                    if (model.Spesifikasi.DaftarAtribut != null)
                    {
                        var la = new List<AtributSpesifikasi>();

                        foreach (var sp in model.Spesifikasi.DaftarAtribut)
                        {
                            if (ContainsDangerousInput(sp.NamaAtribut) ||
                                ContainsDangerousInput(sp.Nilai))
                            {
                                return BadRequest("Atribut mengandung karakter berbahaya");
                            }

                            la.Add(new AtributSpesifikasi
                            {
                                Nama = sp.NamaAtribut,
                                Nilai = sp.Nilai
                            });
                        }

                        ks.AtributSpesifikasi = la;
                    }

                    p.KategoriSpesifikasi = ks;
                }

                _repository.SaveProduk(p);

                if (model.Spesifikasi?.NamaKategori != null &&
                    model.Spesifikasi?.DaftarAtribut != null)
                {
                    var nk = new KategoriSpesifikasi
                    {
                        Nama = model.Spesifikasi.NamaKategori,
                        Deskripsi = ProdukRepo.KATEGORI_TAMPLATE_STRING
                    };

                    var nla = new List<AtributSpesifikasi>();

                    foreach (var sp in model.Spesifikasi.DaftarAtribut)
                    {
                        nla.Add(new AtributSpesifikasi
                        {
                            Nama = sp.NamaAtribut
                        });
                    }

                    nk.AtributSpesifikasi = nla;

                    _repository.SaveKategori(nk);
                }

                return Ok(new
                {
                    message = "Produk berhasil disimpan",
                    id = p.Id
                });
            }
            catch
            {
                return InternalServerError();
            }
        }

        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.Roles.pRole_procurement_head, IdLdapConstants.Roles.pRole_procurement_manager })]
        public List<KategoriSpesifikasi> GetTemplateKategoriSpesifikasi()
        {
            return _repository.GetTemplateKategoriSpesifikasi() ?? null;
        }

        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.Roles.pRole_procurement_head, IdLdapConstants.Roles.pRole_procurement_manager })]
        public List<KategoriSpesifikasi> GetTemplateKategoriSpesifikasis()
        {
            return _repository.GetTemplateKategoriSpesifikasis() ?? null;
        }

        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.Roles.pRole_procurement_head, IdLdapConstants.Roles.pRole_procurement_manager })]
        public ProdukViewModel GetDetailProduk(int id)
        {
            Produk p = _repository.GetProduk(id);
            //KategoriSpesifikasi s = _repository.GetKategoriSpesifikasiByProduk(id);
            ProdukViewModel pvm = new ProdukViewModel()
            {
                Nama = p.Nama,
                id = p.Id,
                Deskripsi = p.Deskripsi,
                Satuan = p.Satuan,
                HargaTerakhir = p.RiwayatHarga.LastOrDefault() != null ? p.RiwayatHarga.LastOrDefault().Harga : 0,
                Currency = p.RiwayatHarga.LastOrDefault() != null ? p.RiwayatHarga.LastOrDefault().Currency : "",
                DefaultRegion = p.RiwayatHarga.LastOrDefault() != null ? p.RiwayatHarga.LastOrDefault().Region : ""
                //Spesifikasi = p.KategoriSpesifikasi
            };
            if (p.KategoriSpesifikasi != null)
            {
                KategoriSpesifikasiViewModel ks = new KategoriSpesifikasiViewModel()
                {
                    NamaKategori = p.KategoriSpesifikasi.Nama,
                    id = p.KategoriSpesifikasi.Id
                };
                List<AtributSpesifikasiViewModel> lam = new List<AtributSpesifikasiViewModel>();
                foreach (AtributSpesifikasi a in p.KategoriSpesifikasi.AtributSpesifikasi)
                {
                    AtributSpesifikasiViewModel na = new AtributSpesifikasiViewModel()
                    {
                        NamaAtribut = a.Nama,
                        Nilai = a.Nilai
                    };
                    lam.Add(na);
                }
                ks.DaftarAtribut = lam;
                pvm.Spesifikasi = ks;
            }
            return pvm;
        }

        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.Roles.pRole_procurement_head, IdLdapConstants.Roles.pRole_procurement_manager })]
        public KategoriSpesifikasi GetKategoriSpesifikasiByProduk(int id)
        {
            return _repository.GetKategoriSpesifikasiByProduk(id);
        }

        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.Roles.pRole_procurement_head, IdLdapConstants.Roles.pRole_procurement_manager })]
        public IHttpActionResult GetRiwayatHarga(int id)
        {
            var rh = _repository.GetRiwayatHarga(id, null);
            List<RiwayatHargaViewModel> rvm = (from a in rh
                                               select new RiwayatHargaViewModel()
                                               {
                                                   Tanggal = a.Tanggal.ToLocalTime().ToShortDateString().Substring(0, 10),
                                                   Harga = a.Harga,
                                                   Currency = a.Currency,
                                                   Sumber = a.Sumber,
                                                   User = a.User
                                               }).ToList();
            return Json(new { aaData = rh });
        }

        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.Roles.pRole_procurement_head, IdLdapConstants.Roles.pRole_procurement_manager })]
        public RiwayatHargaTabelViewModel GetRiwayatHarga2(int id, string region, bool desc = false)
        {
            List<RiwayatHarga> rh = _repository.GetRiwayatHarga(id, region);
            if (desc)
                rh = rh.OrderByDescending(x => x.Tanggal).ToList();
            List<RiwayatHargaViewModel> rvm = (from a in rh
                                               select new RiwayatHargaViewModel()
                                               {
                                                   IdProduk=a.Id,
                                                   Tanggal = a.Tanggal.ToString("yyyy-MM-dd"),
                                                   Harga = a.Harga,
                                                   Currency = a.Currency,
                                                   Sumber = a.Sumber,
                                                   User = a.User
                                               }).ToList();
            return new RiwayatHargaTabelViewModel() { aaData = rvm };
        }

        [HttpPost]
        [ApiAuthorize(new string[] {
    IdLdapConstants.Roles.pRole_procurement_user,
    IdLdapConstants.Roles.pRole_procurement_admin,
    IdLdapConstants.Roles.pRole_procurement_head,
    IdLdapConstants.Roles.pRole_procurement_manager })]
        public IHttpActionResult TambahHarga([FromBody] RiwayatHargaViewModel model)
        {
            if (model == null)
                return BadRequest("Request tidak valid");

            if (model.IdProduk <= 0)
                return BadRequest("IdProduk tidak valid");

            if (model.Harga <= 0)
                return BadRequest("Harga harus lebih dari 0");

            if (ContainsDangerousInput(model.Sumber) ||
                ContainsDangerousInput(model.Region) ||
                ContainsDangerousInput(model.Currency))
            {
                return BadRequest("Input mengandung karakter berbahaya");
            }

            try
            {
                var p = _repository.GetProduk(model.IdProduk);

                if (p == null)
                    return NotFound();

                var rh = new RiwayatHarga()
                {
                    Harga = model.Harga,
                    Currency = HttpUtility.HtmlEncode(model.Currency),
                    Region = HttpUtility.HtmlEncode(model.Region),
                    Sumber = HttpUtility.HtmlEncode(model.Sumber),
                    Tanggal = DateTime.Now,
                    User = CurrentUser.UserName
                };

                if (model.id > 0)
                    rh.Id = model.id;

                if (rh.Id > 0)
                {
                    var old = p.RiwayatHarga.FirstOrDefault(d => d.Id == rh.Id);

                    if (old != null)
                    {
                        old.Harga = rh.Harga;
                        old.Region = rh.Region;
                        old.Sumber = rh.Sumber;
                        old.Currency = rh.Currency;
                        old.User = CurrentUser.UserName;
                    }
                }
                else
                {
                    p.RiwayatHarga.Add(rh);
                }

                _repository.Save();

                return Ok(new
                {
                    message = "Berhasil simpan harga"
                });
            }
            catch
            {
                return InternalServerError();
            }
        }

        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.Roles.pRole_procurement_head, IdLdapConstants.Roles.pRole_procurement_manager })]
        public KategoriSpesifikasi GetKategoriSpesifikasi(int id)
        {
            return _repository.GetKategoriSpesifikasi(id);
        }

        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.Roles.pRole_procurement_head, IdLdapConstants.Roles.pRole_procurement_manager })]
        public List<AtributSpesifikasi> GetDaftarAtributSpesifikasi(int id)
        {//id kategori
            return _repository.GetDaftarAtributSpesifikasi(id);
        }

        public List<string> GetAllRegionDetail(int id)
        {
            Produk p = _repository.GetProduk(id);
            if (p != null)
            {
                return p.RiwayatHarga.Select(x => x.Region).Distinct().ToList();
            }
            return new List<string>();
        }



        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.Roles.pRole_procurement_head, IdLdapConstants.Roles.pRole_procurement_manager })]
        public int SaveTarifAsuransi(InsuranceTarifTemplate model)
        {
            if (model == null)
                return 0;
            InsuranceTarifTemplate p = new InsuranceTarifTemplate()
            {
                DocumentTitle = model.DocumentTitle,
                BenefitType = model.BenefitType
            };

            //KategoriSpesifikasi ks = new KategoriSpesifikasi()
            //{
            //    Nama = model.Spesifikasi.NamaKategori
            //};

            //List<AtributSpesifikasi> la = new List<AtributSpesifikasi>();
            //if (model.Spesifikasi.DaftarAtribut != null)
            //{
            //    foreach (AtributSpesifikasiViewModel sp in model.Spesifikasi.DaftarAtribut)
            //    {
            //        AtributSpesifikasi a = new AtributSpesifikasi()
            //        {
            //            Nama = sp.NamaAtribut,
            //            Nilai = sp.Nilai
            //        };
            //        la.Add(a);
            //    }
            //    ks.AtributSpesifikasi = la;
            //}
            //p.KategoriSpesifikasi = ks;

            //save produk
            _repositoryAsuransi.SaveTarifAsuransi(p);

            ////processing new template category
            //if (model.Spesifikasi.NamaKategori != null && model.Spesifikasi.DaftarAtribut != null)
            //{ //create new template
            //    KategoriSpesifikasi nk = new KategoriSpesifikasi()
            //    {
            //        Nama = model.Spesifikasi.NamaKategori,
            //        Deskripsi = ProdukRepo.KATEGORI_TAMPLATE_STRING
            //    };
            //    List<AtributSpesifikasi> nla = new List<AtributSpesifikasi>();
            //    foreach (AtributSpesifikasiViewModel sp in model.Spesifikasi.DaftarAtribut)
            //    {
            //        AtributSpesifikasi na = new AtributSpesifikasi()
            //        {
            //            Nama = sp.NamaAtribut
            //        };
            //        nla.Add(na);
            //    }
            //    nk.AtributSpesifikasi = nla;
            //    //save kategori
            //    _repository.SaveKategori(nk);
            //}
            return p.Id;
        }

        [HttpGet]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        public IHttpActionResult EditKategori(int id)
        {
            var templateDoc = _repository.GetAllKategori().FirstOrDefault(doc => doc.Id == id);

            if (templateDoc == null)
            {
                return Content(System.Net.HttpStatusCode.NotFound, new { message = "Data tidak ditemukan" });
            }

            return Ok(templateDoc);
        }

        [HttpPost]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.App.Roles.IdLdapProcurementAdminRole })]
        //[ApiAuthorize]
        public IHttpActionResult SaveEditKategori(KategoriSpesifikasi data)
        {
            //JimbisEncrypt code = new JimbisEncrypt();

            var update = _modelContext.KategoriSpesifikasis.Where(d => d.Id == data.Id).FirstOrDefault();

            if (update == null) return BadRequest("Data not found");

            if (string.IsNullOrWhiteSpace(data.Nama)) return BadRequest("Name tidak boleh kosong");

            if (string.IsNullOrWhiteSpace(data.Deskripsi)) return BadRequest("Description tidak boleh kosong");

            if (System.Text.RegularExpressions.Regex.IsMatch(data.Nama, @"[<>]"))
                return BadRequest("Input tidak valid (HTML tidak diperbolehkan)");

            if (System.Text.RegularExpressions.Regex.IsMatch(data.Deskripsi, @"[<>]"))
                return BadRequest("Input tidak valid (HTML tidak diperbolehkan)");

            //update.Qualifier = arrData.Qualifier;
            update.Id =  data.Id;
            update.Nama = Sanitize(data.Nama);
            update.Deskripsi = Sanitize(data.Deskripsi);

            _modelContext.SaveChanges();

            return Json(update.Id);
        }
    }
}