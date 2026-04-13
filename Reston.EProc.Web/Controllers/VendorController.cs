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
using System.Web;
using System.Web.Http;
using System.Net.Http.Headers;
using System.Net;
using Reston.Pinata.WebService.Helper;
using Model.Helper;
using Reston.Pinata.Model.PengadaanRepository.View;

namespace Reston.Pinata.WebService
{
    public class VendorController : BaseController
    {

        private IVendorRepo _repository;
        private AppDbContext _modelContext;
        private string FILE_TEMP_PATH = System.Configuration.ConfigurationManager.AppSettings["FILE_UPLOAD_TEMP"];
        private string FILE_VENDOR_PATH = System.Configuration.ConfigurationManager.AppSettings["FILE_UPLOAD_VENDOR"];
        public VendorController()
        {
            _repository = new VendorRepo(new AppDbContext());
        }

        public VendorController(VendorRepo repository)
        {
            _repository = repository;
        }

        public IEnumerable<string> Get()
        {
            return new[] { "Vendor is so", "more of it", "more" };
        }

        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_superadmin, IdLdapConstants.Roles.pRole_procurement_head, IdLdapConstants.Roles.pRole_procurement_end_user, IdLdapConstants.Roles.pRole_procurement_vendor, IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance })]
        public Vendor GetVendor(int id)
        {
            return _repository.GetVendor(id);
        }

        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_superadmin, IdLdapConstants.Roles.pRole_procurement_head, IdLdapConstants.Roles.pRole_procurement_end_user, IdLdapConstants.Roles.pRole_procurement_vendor, IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance })]
        public List<VendorViewModel> GetVendors(string tipe, string status, int limit, string search)
        {
            var lv =
            _repository.GetVendors(tipe != null ? (ETipeVendor)Enum.Parse(typeof(ETipeVendor), tipe) : ETipeVendor.NONE,
                status != null ? (EStatusVendor)Enum.Parse(typeof(EStatusVendor), status) : EStatusVendor.NONE
                , limit, search);
            if (lv != null)
                return lv.Where(x => search == null || x.Nama.ToLower().Contains(search.ToLower())).Select(x => new VendorViewModel() { id = x.Id, Nama = x.Nama, Alamat = x.Alamat, Provinsi = x.Provinsi, Telepon = x.Telepon, Email = x.Email, Website = x.Website, NoPengajuan = x.NomorVendor }).ToList();
            return new List<VendorViewModel>();
        }

        //[HttpGet]
        //public List<VendorViewModel> GetAllVendor()
        //{
        //    return _repository.GetAllVendor().Select(x => new VendorViewModel()
        //    {
        //        id = x.Id,
        //        Nama = x.Nama
        //    }).ToList();
        //}

        private string GenerateNomorVendor(int tipe)
        {
            string no = "";
            DateTime d = DateTime.Now;
            no = ("" + d.Year).Substring(2, 2) + (d.Month < 10 ? "0" + d.Month : "" + d.Month) + "1" + tipe;//1 means by staff
            string ctrn = "000" + _repository.CheckNomor(no);
            no += ctrn.Substring(ctrn.Length - 4, 4);
            return no;
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_staff)]
        [HttpPost]
        public async Task<int> AddVendor([FromBody]VendorViewModel model)
        {
            //access?
            Vendor v = new Vendor
            {
                TipeVendor = (ETipeVendor)model.TipeVendor,
                NomorVendor = GenerateNomorVendor((int)model.TipeVendor),
                Nama = model.Nama,
                Alamat = model.Alamat,
                Provinsi = model.Provinsi,
                Kota = model.Kota,
                KodePos = model.KodePos,
                Email = model.Email,
                Website = model.Website,
                Telepon = model.Telepon,
                StatusAkhir = EStatusVendor.VERIFIED
            };

            if (model.VendorPerson != null)
            {
                List<VendorPerson> lvp = new List<VendorPerson>();
                foreach (VendorPersonViewModel vpvm in model.VendorPerson)
                {
                    if (vpvm.Nama != null && vpvm.Nama != "") //persons without name are ignored
                        lvp.Add(new VendorPerson
                        {
                            Nama = vpvm.Nama,
                            Jabatan = vpvm.Jabatan,
                            Email = vpvm.Email,
                            Telepon = vpvm.Telepon,
                            Active = true
                        });
                }
                v.VendorPerson = lvp;
            }
            if (model.BankInfo != null)
            {
                BankInfo bi = new BankInfo
                {
                    NamaBank = model.BankInfo.Nama,
                    Cabang = model.BankInfo.Cabang,
                    NomorRekening = model.BankInfo.NomorRekening,
                    NamaRekening = model.BankInfo.NamaRekening,
                    Active = true
                };

                if (bi.NamaBank != null && bi.NamaBank != "") //banks without name are ignored
                    v.BankInfo = new List<BankInfo>() { bi };
            }

            RiwayatPengajuanVendor rp = new RiwayatPengajuanVendor() { Komentar = "Rekanan baru TERVERIFIKASI, oleh " + CurrentUser.UserName, Status = EStatusVendor.NEW, Urutan = 0, Metode = EMetodeVerifikasiVendor.NONE, Waktu = DateTime.Now };
            v.RiwayatPengajuanVendor = new List<RiwayatPengajuanVendor>() { rp };

            DokumenDetail NPWP = null;
            DokumenDetail KTP = null;
            DokumenDetail PKP = null;
            DokumenDetail NPWPPemilik = null;
            DokumenDetail KTPPemilik = null;
            DokumenDetail DOMISILI = null;
            AktaDokumenDetail Akta = null;
            AktaDokumenDetail AktaTerakhir = null;
            IzinUsahaDokumenDetail TDP = null;
            IzinUsahaDokumenDetail SIUP = null;
            IzinUsahaDokumenDetail SIUJK = null;

            _repository.AddVendor(v);
            //check npwp disini
            if (model.NPWP.File != null)
                NPWP = new DokumenDetail { File = CopyFileVendor(model.NPWP.File, v.Id), ContentType = model.NPWP.ContentType, Nomor = model.NPWP.Nomor, TipeDokumen = EDocumentType.NPWP, Vendor = new List<Vendor>() { v }, Active = true };
            if (model.KTP.File != null)
                KTP = new DokumenDetail { File = CopyFileVendor(model.KTP.File, v.Id), ContentType = model.KTP.ContentType, Nomor = model.KTP.Nomor, TipeDokumen = EDocumentType.KTP, Vendor = new List<Vendor>() { v }, Active = true };
            if (model.PKP.File != null)
                PKP = new DokumenDetail { File = CopyFileVendor(model.PKP.File, v.Id), ContentType = model.PKP.ContentType, Nomor = model.PKP.Nomor, TipeDokumen = EDocumentType.PKP, Vendor = new List<Vendor>() { v }, Active = true };
            if (model.NPWPPemilik.File != null)
                NPWPPemilik = new DokumenDetail { File = CopyFileVendor(model.NPWPPemilik.File, v.Id), ContentType = model.NPWPPemilik.ContentType, Nomor = model.NPWPPemilik.Nomor, TipeDokumen = EDocumentType.NPWPPemilik, Vendor = new List<Vendor>() { v }, Active = true };
            if (model.KTPPemilik.File != null)
                KTPPemilik = new DokumenDetail { File = CopyFileVendor(model.KTPPemilik.File, v.Id), ContentType = model.KTPPemilik.ContentType, Nomor = model.KTPPemilik.Nomor, TipeDokumen = EDocumentType.KTPPemilik, Vendor = new List<Vendor>() { v }, Active = true };
            if (model.DOMISILI.File != null)
                DOMISILI = new DokumenDetail { File = CopyFileVendor(model.DOMISILI.File, v.Id), ContentType = model.DOMISILI.ContentType, Nomor = model.DOMISILI.Nomor, TipeDokumen = EDocumentType.DOMISILI, Vendor = new List<Vendor>() { v }, Active = true };
            if (model.Akta.File != null)
                Akta = new AktaDokumenDetail { File = CopyFileVendor(model.Akta.File, v.Id), ContentType = model.Akta.ContentType, Nomor = model.Akta.Nomor, TipeDokumen = EDocumentType.AKTA, Notaris = model.Akta.Notaris, Tanggal = model.Akta.Tanggal, Vendor = new List<Vendor>() { v }, order = 1, Active = true };
            if (model.AktaTerakhir.File != null)
                AktaTerakhir = new AktaDokumenDetail { File = CopyFileVendor(model.AktaTerakhir.File, v.Id), ContentType = model.AktaTerakhir.ContentType, Nomor = model.AktaTerakhir.Nomor, TipeDokumen = EDocumentType.AKTA, Notaris = model.AktaTerakhir.Notaris, Tanggal = model.AktaTerakhir.Tanggal, Vendor = new List<Vendor>() { v }, order = 2, Active = true };
            if (model.TDP.File != null)
                TDP = new IzinUsahaDokumenDetail { File = CopyFileVendor(model.TDP.File, v.Id), ContentType = model.TDP.ContentType, Nomor = model.TDP.Nomor, TipeDokumen = EDocumentType.TDP, Instansi = model.TDP.Instansi, Klasifikasi = model.TDP.Klasifikasi, Kualifikasi = model.TDP.Kualifikasi, MasaBerlaku = model.TDP.MasaBerlaku, Vendor = new List<Vendor>() { v }, Active = true };
            if (model.SIUP.File != null)
                SIUP = new IzinUsahaDokumenDetail { File = CopyFileVendor(model.SIUP.File, v.Id), ContentType = model.SIUP.ContentType, Nomor = model.SIUP.Nomor, TipeDokumen = EDocumentType.SIUP, Instansi = model.SIUP.Instansi, Klasifikasi = model.SIUP.Klasifikasi, Kualifikasi = model.SIUP.Kualifikasi, MasaBerlaku = model.SIUP.MasaBerlaku, Vendor = new List<Vendor>() { v }, Active = true };
            if (model.SIUJK.File != null)
                SIUJK = new IzinUsahaDokumenDetail { File = CopyFileVendor(model.SIUJK.File, v.Id), ContentType = model.SIUJK.ContentType, Nomor = model.SIUJK.Nomor, TipeDokumen = EDocumentType.SIUJK, Instansi = model.SIUJK.Instansi, Klasifikasi = model.SIUJK.Klasifikasi, Kualifikasi = model.SIUJK.Kualifikasi, MasaBerlaku = model.SIUJK.MasaBerlaku, Vendor = new List<Vendor>() { v }, Active = true };

            v.Dokumen = new List<Dokumen>() { NPWP, KTP, PKP, Akta, AktaTerakhir, TDP, SIUJK, SIUP, NPWPPemilik, KTPPemilik, DOMISILI };

            v.Owner = Guid.NewGuid();

            //List<Vendor> lr = _repository.GetAllVendor();
            //if (lr.Count > 0) //exist boss
            //{
            //    return 0;
            //}
            _repository.Save();
            string rand = CreatePassword(8);

            await TestCreateUser(v.NomorVendor, rand, v.Nama, v.Owner);

            return v.Id;
        }

        private string CreatePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string number = "1234567890";
            System.Text.StringBuilder res = new System.Text.StringBuilder();
            Random rnd = new Random();
            int i = length / 4;
            while (0 < i--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
                res.Append(lower[rnd.Next(lower.Length)]);
                res.Append(upper[rnd.Next(upper.Length)]);
                res.Append(number[rnd.Next(number.Length)]);
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString().Substring(0, length);
        }

        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_vendor, IdLdapConstants.Roles.pRole_procurement_vendor })]
        [HttpPost]
        public int EditVendor([FromBody]VendorViewModel model)
        {
            //access?
            Vendor v = _repository.GetVendor(model.id);
            Vendor vold = new Vendor();
            BankInfo biOld = new BankInfo();
            var dokumens = v.Dokumen.Where(x => x.Active == true);
            List<Dokumen> li = dokumens.Where(x => x.TipeDokumen == EDocumentType.AKTA).ToList();
            List<AktaDokumenDetail> lan = new List<AktaDokumenDetail>();
            foreach (Dokumen d in li)
            {
                AktaDokumenDetail a = (AktaDokumenDetail)d;
                lan.Add(a);
            }
            DokumenDetail npwp = (DokumenDetail)dokumens.Where(x => x.TipeDokumen == EDocumentType.NPWP).FirstOrDefault();
            DokumenDetail ktp = (DokumenDetail)dokumens.Where(x => x.TipeDokumen == EDocumentType.KTP).FirstOrDefault();
            DokumenDetail ktpPemilik = (DokumenDetail)dokumens.Where(x => x.TipeDokumen == EDocumentType.KTPPemilik).FirstOrDefault();
            DokumenDetail npwpPemilik = (DokumenDetail)dokumens.Where(x => x.TipeDokumen == EDocumentType.NPWPPemilik).FirstOrDefault();
            DokumenDetail domisili = (DokumenDetail)dokumens.Where(x => x.TipeDokumen == EDocumentType.DOMISILI).FirstOrDefault();
            DokumenDetail pkp = (DokumenDetail)dokumens.Where(x => x.TipeDokumen == EDocumentType.PKP).FirstOrDefault();
            AktaDokumenDetail akta = (AktaDokumenDetail)lan.OrderBy(x => x.order).FirstOrDefault();
            AktaDokumenDetail aktaTerakhir = (AktaDokumenDetail)lan.Where(x => x.order != 1).OrderByDescending(x => x.order).FirstOrDefault();
            IzinUsahaDokumenDetail tdp = (IzinUsahaDokumenDetail)dokumens.Where(x => x.TipeDokumen == EDocumentType.TDP).FirstOrDefault();
            IzinUsahaDokumenDetail siup = (IzinUsahaDokumenDetail)dokumens.Where(x => x.TipeDokumen == EDocumentType.SIUP).FirstOrDefault();
            IzinUsahaDokumenDetail siujk = (IzinUsahaDokumenDetail)dokumens.Where(x => x.TipeDokumen == EDocumentType.SIUJK).FirstOrDefault();


            if (v == null) return 0;
            //v.TipeVendor = (ETipeVendor)model.TipeVendor;
            if (v.Nama != model.Nama)
            {
                vold.Nama = v.Nama;
                v.Nama = model.Nama ?? v.Nama;
            }
            else
            {
                v.Nama = model.Nama ?? v.Nama;
            }
            if (v.Alamat != model.Alamat)
            {
                vold.Alamat = v.Alamat;
                v.Alamat = model.Alamat ?? v.Alamat;
            }
            else
            {
                v.Alamat = model.Alamat ?? v.Alamat;
            }
            if (v.Provinsi != model.Provinsi)
            {
                vold.Provinsi = v.Provinsi;
                v.Provinsi = model.Provinsi ?? v.Provinsi;
            }
            else
            {
                v.Provinsi = model.Provinsi ?? v.Provinsi;
            }
            if (v.Kota != model.Kota)
            {
                vold.Kota = v.Kota;
                v.Kota = model.Kota ?? v.Kota;
            }
            else
            {
                v.Kota = model.Kota ?? v.Kota;
            }
            if (v.KodePos != model.KodePos)
            {
                vold.KodePos = v.KodePos;
                v.KodePos = model.KodePos ?? v.KodePos;
            }
            else
            {
                v.KodePos = model.KodePos ?? v.KodePos;
            }
            if (v.Email != model.Email)
            {
                vold.Email = v.Email;
                v.Email = model.Email ?? v.Email;
            }
            else
            {
                v.Email = model.Email ?? v.Email;
            }
            if (v.Website != model.Website)
            {
                vold.Website = v.Website;
                v.Website = model.Website ?? v.Website;
            }
            else
            {
                v.Website = model.Website ?? v.Website;
            }
            if (v.Telepon != model.Telepon)
            {
                vold.Telepon = v.Telepon;
                v.Telepon = model.Telepon ?? v.Telepon;
            }
            else
            {
                v.Telepon = model.Telepon ?? v.Telepon;
            }
            v.StatusAkhir = EStatusVendor.UPDATED;

            if (model.VendorPerson != null)
            {
                foreach (VendorPerson vp in v.VendorPerson)
                {
                    vp.Active = false;
                }
                List<VendorPerson> lvp = new List<VendorPerson>();
                foreach (VendorPersonViewModel vpvm in model.VendorPerson)
                {
                    if (vpvm.Nama != null && vpvm.Nama != "") //persons without name are ignored
                        lvp.Add(new VendorPerson
                        {
                            Nama = vpvm.Nama,
                            Jabatan = vpvm.Jabatan,
                            Email = vpvm.Email,
                            Telepon = vpvm.Telepon,
                            Active = true
                        });
                }
                v.VendorPerson = lvp;
            }

            if (v.BankInfo != null)
            {
                biOld = new BankInfo
                {
                    NamaBank = v.BankInfo.Where(x => x.Active == true).LastOrDefault() != null ? v.BankInfo.Where(x => x.Active == true).LastOrDefault().NamaBank : null,// 1 bank
                    NamaRekening = v.BankInfo.Where(x => x.Active == true).LastOrDefault() != null ? v.BankInfo.Where(x => x.Active == true).LastOrDefault().NamaRekening : null,// 1 bank
                    Cabang = v.BankInfo.Where(x => x.Active == true).LastOrDefault() != null ? v.BankInfo.Where(x => x.Active == true).LastOrDefault().Cabang : null,// 1 bank
                    NomorRekening = v.BankInfo.Where(x => x.Active == true).LastOrDefault() != null ? v.BankInfo.Where(x => x.Active == true).LastOrDefault().NomorRekening : null
                };
            }

            if (model.BankInfo != null)
            {
                foreach (BankInfo bif in v.BankInfo)
                {
                    bif.Active = false;
                }

                BankInfo bi = new BankInfo
                {
                    NamaBank = model.BankInfo.Nama,
                    Cabang = model.BankInfo.Cabang,
                    NomorRekening = model.BankInfo.NomorRekening,
                    NamaRekening = model.BankInfo.NamaRekening,
                    Active = true
                };

                if (bi.NamaBank != null && bi.NamaBank != "") //banks without name are ignored
                    v.BankInfo = new List<BankInfo>() { bi };
            }
            DokumenDetail NPWP = null;
            DokumenDetail NPWPold = (DokumenDetail)dokumens.Where(x => x.TipeDokumen == EDocumentType.NPWP).FirstOrDefault() ?? null;
            if (model.NPWP.File != null)
            {
                NPWP = new DokumenDetail { File = CopyFileVendor(model.NPWP.File, v.Id), ContentType = model.NPWP.ContentType, Nomor = model.NPWP.Nomor, TipeDokumen = EDocumentType.NPWP, Vendor = new List<Vendor>() { v }, Active = true };
                if (model.NPWP.id != null)
                {
                    Dokumen d = v.Dokumen.Where(x => x.TipeDokumen == EDocumentType.NPWP && x.Active == true).FirstOrDefault() ?? null;
                    if (d != null) d.Active = false;
                }
                v.Dokumen.Add(NPWP);
            }
            else { NPWP = null; }
            if ((NPWPold != null && model.NPWP.File == null) && NPWPold.Nomor != model.NPWP.Nomor)
            {
                NPWP = new DokumenDetail
                {
                    File = NPWPold.File,
                    ContentType = NPWPold.ContentType,
                    Nomor = model.NPWP.Nomor,
                    TipeDokumen = EDocumentType.NPWP,
                    Vendor = new List<Vendor>() { v },
                    Active = true
                };
                Dokumen d = v.Dokumen.Where(x => x.TipeDokumen == EDocumentType.NPWP && x.Active == true).FirstOrDefault() ?? null;
                if (d != null) d.Active = false;
                v.Dokumen.Add(NPWP);
            }
            DokumenDetail KTP = null;
            DokumenDetail KTPold = (DokumenDetail)dokumens.Where(x => x.TipeDokumen == EDocumentType.KTP).FirstOrDefault() ?? null;
            if (model.KTP.File != null)
            {
                KTP = new DokumenDetail { File = CopyFileVendor(model.KTP.File, v.Id), ContentType = model.KTP.ContentType, Nomor = model.KTP.Nomor, TipeDokumen = EDocumentType.KTP, Vendor = new List<Vendor>() { v }, Active = true };
                if (model.KTP.id != null)
                {
                    Dokumen d = v.Dokumen.Where(x => x.TipeDokumen == EDocumentType.KTP && x.Active == true).FirstOrDefault() ?? null;
                    if (d != null) d.Active = false;
                }
                v.Dokumen.Add(KTP);
            }
            if ((model.KTP.File == null && KTPold != null) && KTPold.Nomor != model.KTP.Nomor)
            {
                KTP = new DokumenDetail
                {
                    File = KTPold.File,
                    ContentType = KTPold.ContentType,
                    Nomor = model.KTP.Nomor,
                    TipeDokumen = EDocumentType.KTP,
                    Vendor = new List<Vendor>() { v },
                    Active = true
                };
                Dokumen d = v.Dokumen.Where(x => x.TipeDokumen == EDocumentType.KTP && x.Active == true).FirstOrDefault() ?? null;
                if (d != null) d.Active = false;
                v.Dokumen.Add(KTP);
            }
            DokumenDetail PKP = null;
            DokumenDetail PKPold = (DokumenDetail)dokumens.Where(x => x.TipeDokumen == EDocumentType.PKP).FirstOrDefault() ?? null;
            if (model.PKP.File != null)
            {
                PKP = new DokumenDetail { File = CopyFileVendor(model.PKP.File, v.Id), ContentType = model.PKP.ContentType, Nomor = model.PKP.Nomor, TipeDokumen = EDocumentType.PKP, Vendor = new List<Vendor>() { v }, Active = true };
                if (model.PKP.id != null)
                {
                    Dokumen d = v.Dokumen.Where(x => x.TipeDokumen == EDocumentType.PKP && x.Active == true).FirstOrDefault() ?? null;
                    if (d != null) d.Active = false;
                }
                v.Dokumen.Add(PKP);
            }
            if ((model.PKP.File == null && PKPold != null) && PKPold.Nomor != model.PKP.Nomor)
            {
                PKP = new DokumenDetail
                {
                    File = PKPold.File,
                    ContentType = PKPold.ContentType,
                    Nomor = model.PKP.Nomor,
                    TipeDokumen = EDocumentType.PKP,
                    Vendor = new List<Vendor>() { v },
                    Active = true
                };
                Dokumen d = v.Dokumen.Where(x => x.TipeDokumen == EDocumentType.PKP && x.Active == true).FirstOrDefault() ?? null;
                if (d != null) d.Active = false;
                v.Dokumen.Add(PKP);
            }
            DokumenDetail KTPPemilik = null;
            DokumenDetail KTPPemilikold = (DokumenDetail)dokumens.Where(x => x.TipeDokumen == EDocumentType.KTPPemilik).FirstOrDefault() ?? null;
            if (model.KTPPemilik.File != null)
            {
                KTPPemilik = new DokumenDetail { File = CopyFileVendor(model.KTPPemilik.File, v.Id), ContentType = model.KTPPemilik.ContentType, Nomor = model.KTPPemilik.Nomor, TipeDokumen = EDocumentType.KTPPemilik, Vendor = new List<Vendor>() { v }, Active = true };
                if (model.KTPPemilik.id != null)
                {
                    Dokumen d = v.Dokumen.Where(x => x.TipeDokumen == EDocumentType.KTPPemilik && x.Active == true).FirstOrDefault() ?? null;
                    if (d != null) d.Active = false;
                }
                v.Dokumen.Add(KTPPemilik);
            }
            if ((model.KTPPemilik.File == null && KTPPemilikold != null) && KTPPemilikold.Nomor != model.KTPPemilik.Nomor)
            {
                KTPPemilik = new DokumenDetail
                {
                    File = KTPPemilikold.File,
                    ContentType = KTPPemilikold.ContentType,
                    Nomor = model.KTPPemilik.Nomor,
                    TipeDokumen = EDocumentType.KTPPemilik,
                    Vendor = new List<Vendor>() { v },
                    Active = true
                };
                Dokumen d = v.Dokumen.Where(x => x.TipeDokumen == EDocumentType.KTPPemilik && x.Active == true).FirstOrDefault() ?? null;
                if (d != null) d.Active = false;
                v.Dokumen.Add(KTPPemilik);
            }
            DokumenDetail NPWPPemilik = null;
            DokumenDetail NPWPPemilikold = (DokumenDetail)dokumens.Where(x => x.TipeDokumen == EDocumentType.NPWPPemilik).FirstOrDefault() ?? null;
            if (model.NPWPPemilik.File != null)
            {
                NPWPPemilik = new DokumenDetail { File = CopyFileVendor(model.NPWPPemilik.File, v.Id), ContentType = model.NPWPPemilik.ContentType, Nomor = model.NPWPPemilik.Nomor, TipeDokumen = EDocumentType.NPWPPemilik, Vendor = new List<Vendor>() { v }, Active = true };
                if (model.NPWPPemilik.id != null)
                {
                    Dokumen d = v.Dokumen.Where(x => x.TipeDokumen == EDocumentType.NPWPPemilik && x.Active == true).FirstOrDefault() ?? null;
                    if (d != null) d.Active = false;
                }
                v.Dokumen.Add(NPWPPemilik);
            }
            if ((model.NPWPPemilik.File == null && NPWPPemilikold != null) && NPWPPemilikold.Nomor != model.NPWPPemilik.Nomor)
            {
                NPWPPemilik = new DokumenDetail
                {
                    File = NPWPPemilikold.File,
                    ContentType = NPWPPemilikold.ContentType,
                    Nomor = model.NPWPPemilik.Nomor,
                    TipeDokumen = EDocumentType.NPWPPemilik,
                    Vendor = new List<Vendor>() { v },
                    Active = true
                };
                Dokumen d = v.Dokumen.Where(x => x.TipeDokumen == EDocumentType.NPWPPemilik && x.Active == true).FirstOrDefault() ?? null;
                if (d != null) d.Active = false;
                v.Dokumen.Add(NPWPPemilik);
            }
            DokumenDetail DOMISILI = null;
            DokumenDetail DOMISILIold = (DokumenDetail)dokumens.Where(x => x.TipeDokumen == EDocumentType.DOMISILI).FirstOrDefault() ?? null;
            if (model.DOMISILI.File != null)
            {
                DOMISILI = new DokumenDetail { File = CopyFileVendor(model.DOMISILI.File, v.Id), ContentType = model.DOMISILI.ContentType, Nomor = model.DOMISILI.Nomor, TipeDokumen = EDocumentType.DOMISILI, Vendor = new List<Vendor>() { v }, Active = true };
                if (model.DOMISILI.id != null)
                {
                    Dokumen d = v.Dokumen.Where(x => x.TipeDokumen == EDocumentType.DOMISILI && x.Active == true).FirstOrDefault() ?? null;
                    if (d != null) d.Active = false;
                }
                v.Dokumen.Add(DOMISILI);
            }
            if ((model.DOMISILI.File == null && DOMISILIold != null) && DOMISILIold.Nomor != model.DOMISILI.Nomor)
            {
                DOMISILI = new DokumenDetail {
                    File = DOMISILIold.File,
                    ContentType = DOMISILIold.ContentType,
                    Nomor = model.DOMISILI.Nomor,
                    TipeDokumen = EDocumentType.DOMISILI,
                    Vendor = new List<Vendor>() { v },
                    Active = true };
                Dokumen d = v.Dokumen.Where(x => x.TipeDokumen == EDocumentType.DOMISILI && x.Active == true).FirstOrDefault() ?? null;
                if (d != null) d.Active = false;
                v.Dokumen.Add(DOMISILI);
            }
            List<Dokumen> add = v.Dokumen.Where(x => x.Active == true && x.TipeDokumen == EDocumentType.AKTA).ToList();
            List<AktaDokumenDetail> lad = new List<AktaDokumenDetail>();
            foreach (Dokumen d in add)
            {
                AktaDokumenDetail a = (AktaDokumenDetail)d;
                lad.Add(a);
            }
            AktaDokumenDetail Akta = null;
            AktaDokumenDetail Aktaold = (AktaDokumenDetail)lan.OrderBy(x => x.order).FirstOrDefault() ?? null;
            if (model.Akta.File != null)
            {
                Akta = new AktaDokumenDetail { File = CopyFileVendor(model.Akta.File, v.Id), ContentType = model.Akta.ContentType, Nomor = model.Akta.Nomor, TipeDokumen = EDocumentType.AKTA, Notaris = model.Akta.Notaris, Tanggal = model.Akta.Tanggal, Vendor = new List<Vendor>() { v }, order = 1, Active = true };
                if (model.Akta.id != null)
                {
                    AktaDokumenDetail d = (AktaDokumenDetail)v.Dokumen.Where(x => x.TipeDokumen == EDocumentType.AKTA && x.Active == true).FirstOrDefault() ?? null;
                    if (d != null) d.Active = false;

                }
                v.Dokumen.Add(Akta);
            }
            if ((Aktaold != null && model.Akta.File == null) && (Aktaold.Nomor != model.Akta.Nomor || Aktaold.Notaris != model.Akta.Notaris || Aktaold.Tanggal != model.Akta.Tanggal))
            {
                Akta = new AktaDokumenDetail
                {
                    File = Aktaold.File,
                    ContentType = Aktaold.ContentType,
                    Nomor = model.Akta.Nomor,
                    TipeDokumen = EDocumentType.AKTA,
                    Notaris = model.Akta.Notaris,
                    Tanggal = model.Akta.Tanggal,
                    Vendor = new List<Vendor>() { v },
                    order = 1,
                    Active = true
                };
                AktaDokumenDetail d = (AktaDokumenDetail)v.Dokumen.Where(x => x.TipeDokumen == EDocumentType.AKTA && x.Active == true).FirstOrDefault() ?? null;
                if (d != null) d.Active = false;
                v.Dokumen.Add(Akta);
            }
            AktaDokumenDetail AktaTerakhir = null;
            AktaDokumenDetail AktaTerakhirold = (AktaDokumenDetail)lan.Where(x => x.order != 1).OrderByDescending(x => x.order).FirstOrDefault() ?? null;
            if (model.AktaTerakhir.File != null)
            {
                AktaTerakhir = new AktaDokumenDetail { File = CopyFileVendor(model.AktaTerakhir.File, v.Id), ContentType = model.AktaTerakhir.ContentType, Nomor = model.AktaTerakhir.Nomor, TipeDokumen = EDocumentType.AKTA, Notaris = model.AktaTerakhir.Notaris, Tanggal = model.AktaTerakhir.Tanggal, Vendor = new List<Vendor>() { v }, order = 1, Active = true };
                if (model.AktaTerakhir.id != null)
                {
                    AktaDokumenDetail d = (AktaDokumenDetail)v.Dokumen.Where(x => x.TipeDokumen == EDocumentType.AKTA && x.Active == true).FirstOrDefault() ?? null;
                    if (d != null) d.Active = false;
                }
                v.Dokumen.Add(AktaTerakhir);
            }
            if ((AktaTerakhirold != null && model.AktaTerakhir.File == null) && (AktaTerakhirold.Nomor != model.AktaTerakhir.Nomor || AktaTerakhirold.Notaris != model.AktaTerakhir.Notaris || AktaTerakhirold.Tanggal != model.AktaTerakhir.Tanggal))
            {
                AktaTerakhir = new AktaDokumenDetail
                {
                    File = AktaTerakhirold.File,
                    ContentType = AktaTerakhirold.ContentType,
                    Nomor = model.AktaTerakhir.Nomor,
                    TipeDokumen = EDocumentType.AKTA,
                    Notaris = model.AktaTerakhir.Notaris,
                    Tanggal = model.AktaTerakhir.Tanggal,
                    Vendor = new List<Vendor>() { v },
                    order = 2,
                    Active = true
                };
                AktaDokumenDetail d = (AktaDokumenDetail)v.Dokumen.Where(x => x.TipeDokumen == EDocumentType.AKTA && x.Active == true).FirstOrDefault() ?? null;
                if (d != null) d.Active = false;
                v.Dokumen.Add(AktaTerakhir);
            }
            IzinUsahaDokumenDetail TDP = null;
            IzinUsahaDokumenDetail TDPold = (IzinUsahaDokumenDetail)dokumens.Where(x => x.TipeDokumen == EDocumentType.TDP).FirstOrDefault() ?? null;
            if (model.TDP.File != null)
            {
                TDP = new IzinUsahaDokumenDetail { File = CopyFileVendor(model.TDP.File, v.Id), ContentType = model.TDP.ContentType, Nomor = model.TDP.Nomor, TipeDokumen = EDocumentType.TDP, Instansi = model.TDP.Instansi, Klasifikasi = model.TDP.Klasifikasi, Kualifikasi = model.TDP.Kualifikasi, MasaBerlaku = model.TDP.MasaBerlaku, Vendor = new List<Vendor>() { v }, Active = true };
                if (model.TDP.id != null)
                {
                    Dokumen d = v.Dokumen.Where(x => x.TipeDokumen == EDocumentType.TDP && x.Active == true).FirstOrDefault() ?? null;
                    if (d != null) d.Active = false;
                }
                v.Dokumen.Add(TDP);
            }
            if ((TDPold != null && model.TDP.File == null) && (TDPold.Nomor != model.TDP.Nomor || TDPold.Instansi != model.TDP.Instansi || TDPold.Kualifikasi != model.TDP.Kualifikasi || TDPold.Klasifikasi != model.TDP.Klasifikasi || TDPold.MasaBerlaku != model.TDP.MasaBerlaku))
            {
                TDP = new IzinUsahaDokumenDetail
                {
                    File = TDPold.File,
                    ContentType = TDPold.ContentType,
                    Nomor = model.TDP.Nomor,
                    TipeDokumen = EDocumentType.TDP,
                    Instansi = model.TDP.Instansi,
                    Klasifikasi = model.TDP.Klasifikasi,
                    Kualifikasi = model.TDP.Kualifikasi,
                    MasaBerlaku = model.TDP.MasaBerlaku,
                    Vendor = new List<Vendor>() { v },
                    Active = true
                };
                Dokumen d = v.Dokumen.Where(x => x.TipeDokumen == EDocumentType.TDP && x.Active == true).FirstOrDefault() ?? null;
                if (d != null) d.Active = false;
                v.Dokumen.Add(TDP);
            }
            IzinUsahaDokumenDetail SIUP = null;
            IzinUsahaDokumenDetail SIUPold = (IzinUsahaDokumenDetail)dokumens.Where(x => x.TipeDokumen == EDocumentType.SIUP).FirstOrDefault() ?? null;
            if (model.SIUP.File != null)
            {
                SIUP = new IzinUsahaDokumenDetail { File = CopyFileVendor(model.SIUP.File, v.Id), ContentType = model.SIUP.ContentType, Nomor = model.SIUP.Nomor, TipeDokumen = EDocumentType.SIUP, Instansi = model.SIUP.Instansi, Klasifikasi = model.SIUP.Klasifikasi, Kualifikasi = model.SIUP.Kualifikasi, MasaBerlaku = model.SIUP.MasaBerlaku, Vendor = new List<Vendor>() { v }, Active = true };
                if (model.SIUP.id != null)
                {
                    Dokumen d = v.Dokumen.Where(x => x.TipeDokumen == EDocumentType.SIUP && x.Active == true).FirstOrDefault() ?? null;
                    if (d != null) d.Active = false;
                }
                v.Dokumen.Add(SIUP);
            }
            if ((SIUPold != null && model.SIUP.File == null) && (SIUPold.Nomor != model.SIUP.Nomor || SIUPold.Instansi != model.SIUP.Instansi || SIUPold.Kualifikasi != model.SIUP.Kualifikasi || SIUPold.Klasifikasi != model.SIUP.Klasifikasi || SIUPold.MasaBerlaku != model.SIUP.MasaBerlaku))
            {
                SIUP = new IzinUsahaDokumenDetail
                {
                    File = SIUPold.File,
                    ContentType = SIUPold.ContentType,
                    Nomor = model.SIUP.Nomor,
                    TipeDokumen = EDocumentType.SIUP,
                    Instansi = model.SIUP.Instansi,
                    Klasifikasi = model.SIUP.Klasifikasi,
                    Kualifikasi = model.SIUP.Kualifikasi,
                    MasaBerlaku = model.SIUP.MasaBerlaku,
                    Vendor = new List<Vendor>() { v },
                    Active = true
                };
                Dokumen d = v.Dokumen.Where(x => x.TipeDokumen == EDocumentType.SIUP && x.Active == true).FirstOrDefault() ?? null;
                if (d != null) d.Active = false;
                v.Dokumen.Add(SIUP);
            }
            IzinUsahaDokumenDetail SIUJK = null;
            IzinUsahaDokumenDetail SIUJKold = (IzinUsahaDokumenDetail)dokumens.Where(x => x.TipeDokumen == EDocumentType.SIUJK).FirstOrDefault() ?? null;
            if (model.SIUJK.File != null)
            {
                SIUJK = new IzinUsahaDokumenDetail { File = CopyFileVendor(model.SIUJK.File, v.Id), ContentType = model.SIUJK.ContentType, Nomor = model.SIUJK.Nomor, TipeDokumen = EDocumentType.SIUJK, Instansi = model.SIUJK.Instansi, Klasifikasi = model.SIUJK.Klasifikasi, Kualifikasi = model.SIUJK.Kualifikasi, MasaBerlaku = model.SIUJK.MasaBerlaku, Vendor = new List<Vendor>() { v }, Active = true };
                if (model.SIUJK.id != null)
                {
                    Dokumen d = v.Dokumen.Where(x => x.TipeDokumen == EDocumentType.SIUJK && x.Active == true).FirstOrDefault() ?? null;
                    if (d != null) d.Active = false;
                }
                v.Dokumen.Add(SIUJK);
            }
            if ((SIUJKold != null && model.SIUJK.File == null) && (SIUJKold.Nomor != model.SIUJK.Nomor || SIUJKold.Instansi != model.SIUJK.Instansi || SIUJKold.Kualifikasi != model.SIUJK.Kualifikasi || SIUJKold.Klasifikasi != model.SIUJK.Klasifikasi || SIUJKold.MasaBerlaku != model.SIUJK.MasaBerlaku))
            {
                SIUJK = new IzinUsahaDokumenDetail {
                        File = SIUJKold.File,
                        ContentType = SIUJKold.ContentType,
                        Nomor = model.SIUJK.Nomor,
                        TipeDokumen = EDocumentType.SIUJK,
                        Instansi = model.SIUJK.Instansi,
                        Klasifikasi = model.SIUJK.Klasifikasi,
                        Kualifikasi = model.SIUJK.Kualifikasi,
                        MasaBerlaku = model.SIUJK.MasaBerlaku,
                        Vendor = new List<Vendor>() { v }, Active = true };
                Dokumen d = v.Dokumen.Where(x => x.TipeDokumen == EDocumentType.SIUJK && x.Active == true).FirstOrDefault() ?? null;
                if (d != null) d.Active = false;
                v.Dokumen.Add(SIUJK);
            }

            //oldNPWP.Nomor = npwp.Nomor;
            if (vold != null || NPWP != null || KTP != null || PKP != null || KTPPemilik != null || NPWPPemilik != null || DOMISILI != null || Akta != null || AktaTerakhir != null || TDP != null || SIUP != null || SIUJK != null || model.BankInfo != null)
            {
                string a = vold.Nama;
                string b = vold.Alamat;
                string c = vold.Provinsi;
                string d = vold.Kota;
                string e = vold.KodePos;
                string f = vold.Email;
                string g = vold.Website;
                string h = vold.Telepon;
                string i = null;
                string i2 = null;
                string NPWPbeforeNomor = null; string NPWPafterNomor = null;
                if ((NPWP != null && NPWPold != null) && NPWPold.Nomor != NPWP.Nomor) { NPWPbeforeNomor = " Nomor: " + NPWPold.Nomor; NPWPafterNomor = " Nomor: " + NPWP.Nomor; }
                //if (NPWP != null) i = NPWP.Nomor;
                string j = null;
                string j2 = null;
                string KTPbeforeNomor = null; string KTPafterNomor = null;
                if ((KTP != null && KTPold != null) && KTPold.Nomor != KTP.Nomor) { KTPbeforeNomor = " Nomor: " + KTPold.Nomor; KTPafterNomor = " Nomor: " + KTP.Nomor; }
                //if (KTP != null) j = KTP.Nomor;
                string k = null;
                string k2 = null;
                string PKPbeforeNomor = null; string PKPafterNomor = null;
                if ((PKP != null && PKPold != null) && PKPold.Nomor != PKP.Nomor) { PKPbeforeNomor = " Nomor: " + PKPold.Nomor; PKPafterNomor = " Nomor: " + PKP.Nomor; }
                //if (PKP != null) k = PKP.Nomor;
                string l = null;
                string l2 = null;
                string KTPPemilikbeforeNomor = null; string KTPPemilikafterNomor = null;
                if ((KTPPemilik != null && KTPPemilikold != null) && KTPPemilikold.Nomor != KTPPemilik.Nomor) { KTPPemilikbeforeNomor = " Nomor: " + KTPPemilikold.Nomor; KTPPemilikafterNomor = " Nomor: " + KTPPemilik.Nomor; }
                //if (KTPPemilik != null) l = KTPPemilik.Nomor;
                string m = null;
                string m2 = null;
                string NPWPPemilikbeforeNomor = null; string NPWPPemilikafterNomor = null;
                if ((NPWPPemilik != null && NPWPPemilikold != null) && NPWPPemilikold.Nomor != NPWPPemilik.Nomor) { NPWPPemilikbeforeNomor = " Nomor: " + NPWPPemilikold.Nomor; NPWPPemilikafterNomor = " Nomor: " + NPWPPemilik.Nomor; }
                //if (NPWPPemilik != null) m = NPWPPemilik.Nomor;
                //Domisili
                string n = null;
                string n2 = null;
                string DOMISILIbeforeNomor = null; string DOMISILIafterNomor = null;
                if ((DOMISILI != null && DOMISILIold != null) && DOMISILIold.Nomor != DOMISILI.Nomor) { DOMISILIbeforeNomor = " Nomor: " + DOMISILIold.Nomor; DOMISILIafterNomor = " Nomor: " + DOMISILI.Nomor; }
                //if (DOMISILI != null) n = DOMISILI.Nomor;
                //Akta
                string o = null;
                string o2 = null;
                string AktabeforeNomor = null; string AktabeforeNotaris = null; string AktabeforeTanggal = null;
                string AktaafterNomor = null; string AktaafterNotaris = null; string AktaafterTanggal = null;
                if (Akta != null)
                {
                    if (Aktaold.Nomor != Akta.Nomor) { AktabeforeNomor = " Nomor: " + Aktaold.Nomor; AktaafterNomor = " Nomor: " + Akta.Nomor; }
                    if (Aktaold.Notaris != Akta.Notaris) { AktabeforeNotaris = " Notaris: " + Aktaold.Notaris; AktaafterNotaris = " Notaris: " + Akta.Notaris; }
                    if (Aktaold.Tanggal != Akta.Tanggal) { AktabeforeTanggal = " Tanggal: " + Aktaold.Tanggal; AktaafterTanggal = " Tanggal: " + Akta.Tanggal; }
                }
                //if (Akta != null) o = Akta.Nomor;
                //TDP
                string p = null;
                string p2 = null;
                string TDPbeforenomor = null; string TDPbeforeinstansi = null; string TDPbeforekualifikasi = null; string TDPbeforemasaberlaku = null; string TDPbeforeklasifikasi = null;
                string TDPafternomor = null; string TDPafterinstansi = null; string TDPafterkualifikasi = null; string TDPaftermasaberlaku = null; string TDPafterklasifikasi = null;
                if (TDP != null)
                {
                    if (TDPold.Nomor != TDP.Nomor) { TDPbeforenomor = " Nomor: " + TDPold.Nomor; TDPafternomor = " Nomor: " + TDP.Nomor; }
                    if (TDPold.Instansi != TDP.Instansi) { TDPbeforeinstansi = " Instansi: " + TDPold.Instansi; TDPafterinstansi = " Instansi: " + TDP.Instansi; }
                    if (TDPold.Kualifikasi != TDP.Kualifikasi) { TDPbeforekualifikasi = " Kualifikasi: " + TDPold.Kualifikasi; TDPafterkualifikasi = " Kualifikasi: " + TDP.Kualifikasi; }
                    if (TDPold.Klasifikasi != TDP.Klasifikasi) { TDPbeforeklasifikasi = " klasifikasi: " + TDPold.Klasifikasi; TDPafterklasifikasi = " klasifikasi: " + TDP.Klasifikasi; }
                    if (TDPold.MasaBerlaku != TDP.MasaBerlaku) { TDPbeforemasaberlaku = " Masa Berlaku: " + TDPold.MasaBerlaku.ToString(); TDPaftermasaberlaku = " Masa Berlaku: " + TDP.MasaBerlaku.ToString(); }
                }
                //if (TDP != null) p = TDP.Nomor;
                //SIUP
                string q = null;
                string q2 = null;
                string SIUPbeforenomor = null; string SIUPbeforeinstansi = null; string SIUPbeforekualifikasi = null; string SIUPbeforemasaberlaku = null; string SIUPbeforeklasifikasi = null;
                string SIUPafternomor = null; string SIUPafterinstansi = null; string SIUPafterkualifikasi = null; string SIUPaftermasaberlaku = null; string SIUPafterklasifikasi = null;
                if (SIUP != null)
                {
                    if (SIUPold.Nomor != SIUP.Nomor) { SIUPbeforenomor = " Nomor: " + SIUPold.Nomor; SIUPafternomor = " Nomor: " + SIUP.Nomor; }
                    if (SIUPold.Instansi != SIUP.Instansi) { SIUPbeforeinstansi = " Instansi: " + SIUPold.Instansi; SIUPafterinstansi = " Instansi: " + SIUP.Instansi; }
                    if (SIUPold.Kualifikasi != SIUP.Kualifikasi) { SIUPbeforekualifikasi = " Kualifikasi: " + SIUPold.Kualifikasi; SIUPafterkualifikasi = " Kualifikasi: " + SIUP.Kualifikasi; }
                    if (SIUPold.Klasifikasi != SIUP.Klasifikasi) { SIUPbeforeklasifikasi = " klasifikasi: " + SIUPold.Klasifikasi; SIUPafterklasifikasi = " klasifikasi: " + SIUP.Klasifikasi; }
                    if (SIUPold.MasaBerlaku != SIUP.MasaBerlaku) { SIUPbeforemasaberlaku = " Masa Berlaku: " + SIUPold.MasaBerlaku.ToString(); SIUPaftermasaberlaku = " Masa Berlaku: " + SIUP.MasaBerlaku.ToString(); }
                }
                //if (SIUP != null) q = SIUP.Nomor;
                //SIUJK
                string r = null;
                string r2 = null;
                string siujkbeforenomor = null; string siujkbeforeinstansi = null; string siujkbeforekualifikasi = null; string siujkbeforemasaberlaku = null; string siujkbeforeklasifikasi = null;
                string SIUJKafternomor = null; string SIUJKafterinstansi = null; string SIUJKafterkualifikasi = null; string SIUJKaftermasaberlaku = null; string SIUJKafterklasifikasi = null;
                if (SIUJK != null) {
                    if (SIUJKold.Nomor != SIUJK.Nomor) { siujkbeforenomor = " Nomor: " + SIUJKold.Nomor; SIUJKafternomor = " Nomor: " + SIUJK.Nomor; }
                    if (SIUJKold.Instansi != SIUJK.Instansi) { siujkbeforeinstansi = " Instansi: " + SIUJKold.Instansi; SIUJKafterinstansi = " Instansi: " + SIUJK.Instansi; }
                    if (SIUJKold.Kualifikasi != SIUJK.Kualifikasi) { siujkbeforekualifikasi = " Kualifikasi: " + SIUJKold.Kualifikasi; SIUJKafterkualifikasi = " Kualifikasi: " + SIUJK.Kualifikasi; }
                    if (SIUJKold.Klasifikasi != SIUJK.Klasifikasi) { siujkbeforeklasifikasi = " klasifikasi: " + SIUJKold.Klasifikasi; SIUJKafterklasifikasi = " klasifikasi: " + SIUJK.Klasifikasi; }
                    if (SIUJKold.MasaBerlaku != SIUJK.MasaBerlaku) { siujkbeforemasaberlaku = " Masa Berlaku: " + SIUJKold.MasaBerlaku.ToString(); SIUJKaftermasaberlaku = " Masa Berlaku: " + SIUJK.MasaBerlaku.ToString(); }
                }
                //if (SIUJK != null) r = SIUJK.Nomor;
                //Akta Terakhir
                string s = null;
                string s2 = null;
                string AktaTerakhirbeforeNomor = null; string AktaTerakhirbeforeNotaris = null; string AktaTerakhirbeforeTanggal = null;
                string AktaTerakhirafterNomor = null; string AktaTerakhirafterNotaris = null; string AktaTerakhirafterTanggal = null;
                if (AktaTerakhir != null)
                {
                    if (AktaTerakhirold.Nomor != AktaTerakhir.Nomor) { AktaTerakhirbeforeNomor = " Nomor: " + AktaTerakhirold.Nomor; AktaTerakhirafterNomor = " Nomor: " + AktaTerakhir.Nomor; }
                    if (AktaTerakhirold.Notaris != AktaTerakhir.Notaris) { AktaTerakhirbeforeNotaris = " Notaris: " + AktaTerakhirold.Notaris; AktaTerakhirafterNotaris = " Notaris: " + AktaTerakhir.Notaris; }
                    if (AktaTerakhirold.Tanggal != AktaTerakhir.Tanggal) { AktaTerakhirbeforeTanggal = " Tanggal: " + AktaTerakhirold.Tanggal; AktaTerakhirafterTanggal = " Tanggal: " + AktaTerakhir.Tanggal; }
                }
                //if (AktaTerakhir != null) s = AktaTerakhir.Nomor;
                //Bank Info
                string t = null;
                string banknamabefore = null; string banknamarekeningbefore = null; string banknomorbefore = null; string bankcabangbefore = null;
                string banknamaafter = null; string banknamarekeningafter = null; string banknomorafter = null; string bankcabangafter = null;
                if (model.BankInfo.NomorRekening != biOld.NomorRekening) { banknomorbefore = " Nomor: " + biOld.NomorRekening; banknomorafter = " Nomor: " + model.BankInfo.NomorRekening; }
                if (model.BankInfo.NamaRekening != biOld.NamaRekening) { banknamarekeningbefore = " A/Nama: " + biOld.NamaRekening; banknamarekeningafter = " A/Nama: " + model.BankInfo.NamaRekening; }
                if (model.BankInfo.Nama != biOld.NamaBank) { banknamabefore = " Bank: " + biOld.NamaBank; banknamaafter = " Bank: " + model.BankInfo.Nama; }
                if (model.BankInfo.Cabang != biOld.Cabang) { bankcabangbefore = " Cabang: " + biOld.Cabang; bankcabangafter = " Cabang: " + model.BankInfo.Cabang ; }
                //string t1 = null;
                //string t2 = null;
                //if (model.BankInfo.NomorRekening != biOld.NomorRekening || model.BankInfo.NamaRekening != biOld.NamaRekening || model.BankInfo.Nama != biOld.NamaBank) {
                //    t = model.BankInfo.Nama;
                //    t1 = model.BankInfo.NamaRekening;
                //    t2 = model.BankInfo.NomorRekening;
                //};

                if (a != null) a = "Nama dari " + a + " menjadi " + v.Nama + ", ";
                if (b != null) b = "Alamat dari " + b + " menjadi " + v.Alamat + ", ";
                if (c != null) c = "Provinsi dari " + c + " menjadi " + v.Provinsi + ", ";
                if (d != null) d = "Kota dari " + d + " menjadi " + v.Kota + ", ";
                if (e != null) e = "Kode Pos dari " + e + " menjadi " + v.KodePos + ", ";
                if (f != null) f = "Email dari " + f + " menjadi " + v.Email + ", ";
                if (g != null) g = "Website dari " + g + " menjadi " + v.Website + ", ";
                if (h != null) h = "Telepon dari " + h + " menjadi " + v.Telepon + ", ";
                //if (i != null) i = "NPWP dari " + npwp.Nomor + " menjadi " + i + ", ";
                if ((NPWP != null && NPWPold != null) && NPWP.File != NPWPold.File) i = "File Dokumen NPWP, ";
                if ((NPWP != null && NPWPold != null) && NPWPold.Nomor != NPWP.Nomor) i2 = "Data Akta dari " + NPWPbeforeNomor + " menjadi " + NPWPafterNomor + ", ";
                //if (j != null) j = "KTP dari " + ktp.Nomor + " menjadi " + j + ", ";
                if ((KTP != null && KTPold != null) && KTP.File != KTPold.File) j = "File Dokumen KTP, ";
                if ((KTP != null && KTPold != null) && KTPold.Nomor != KTP.Nomor) j2 = "Data Akta dari " + KTPbeforeNomor + " menjadi " + KTPafterNomor + ", ";
                //if (k != null) k = "PKP dari " + pkp.Nomor + " menjadi " + k + ", ";
                if ((PKP != null && PKPold != null) && PKP.File != PKPold.File) k = "File Dokumen PKP, ";
                if ((PKP != null && PKPold != null) && PKPold.Nomor != PKP.Nomor) k2 = "Data Akta dari " + PKPbeforeNomor + " menjadi " + PKPafterNomor + ", ";
                //if (l != null) l = "KTP dari " + ktpPemilik.Nomor + " menjadi " + l + ", ";
                if ((KTPPemilik != null && KTPPemilikold != null) && KTPPemilik.File != KTPPemilikold.File) l = "File Dokumen KTP Perusahaan, ";
                if ((KTPPemilik != null && KTPPemilikold != null) && KTPPemilikold.Nomor != KTPPemilik.Nomor) l2 = "Data Akta dari " + KTPPemilikbeforeNomor + " menjadi " + KTPPemilikafterNomor + ", ";
                //if (m != null) m = "NPWP dari " + npwpPemilik.Nomor + " menjadi " + m + ", ";
                if ((NPWPPemilik != null && NPWPPemilikold != null) && NPWPPemilik.File != NPWPPemilikold.File) m = "File Dokumen NPWP Perusahaan, ";
                if ((NPWPPemilik != null && NPWPPemilikold != null) && NPWPPemilikold.Nomor != NPWPPemilik.Nomor) m2 = "Data Akta dari " + NPWPPemilikbeforeNomor + " menjadi " + NPWPPemilikafterNomor + ", ";
                //if (n != null) n = "DOMISILI dari " + domisili.Nomor + " menjadi " + n + ", ";
                if ((DOMISILI != null && DOMISILIold != null) && DOMISILI.File != DOMISILIold.File) n = "File Dokumen DOMISILI, ";
                if ((DOMISILI != null && DOMISILIold != null) && DOMISILIold.Nomor != DOMISILI.Nomor) n2 = "Data Akta dari " + DOMISILIbeforeNomor + " menjadi " + DOMISILIafterNomor + ", ";
                //if (o != null) o = "AKTA dari " + akta.Nomor + " menjadi " + o + ", ";
                if ((Akta != null && Aktaold != null) && Akta.File != Aktaold.File) o = "File Dokumen Akta, ";
                if ((Akta != null && Aktaold != null) && (Aktaold.Nomor != Akta.Nomor || Aktaold.Notaris != Akta.Notaris || Aktaold.Tanggal != Akta.Tanggal)) o2 = "Data Akta dari " + AktabeforeNomor + AktabeforeNotaris + AktabeforeTanggal + " menjadi " + AktaafterNomor + AktaafterNotaris + AktaafterTanggal + ", ";
                //if (p != null) p = "TDP dari " + tdp.Nomor + " menjadi " + p + ", ";
                if ((TDP != null && TDPold != null) && TDP.File != TDPold.File) p = "File Dokumen TDP, ";
                if ((TDP != null && TDPold != null) && (TDPold.Nomor != TDP.Nomor || TDPold.Instansi != TDP.Instansi || TDPold.Kualifikasi != TDP.Kualifikasi || TDPold.Klasifikasi != TDP.Klasifikasi || TDPold.MasaBerlaku != TDP.MasaBerlaku)) p2 = "Data TDP dari " + TDPbeforenomor + TDPbeforeinstansi + TDPbeforekualifikasi + TDPbeforemasaberlaku + TDPbeforeklasifikasi + " menjadi " + TDPafternomor + TDPafterinstansi + TDPafterkualifikasi + TDPaftermasaberlaku + TDPafterklasifikasi + ", ";
                if ((SIUP != null && SIUPold != null) && SIUP.File != SIUPold.File) q = "File Dokumen SIUP, ";
                if ((SIUP != null && SIUPold != null) && (SIUPold.Nomor != SIUP.Nomor || SIUPold.Instansi != SIUP.Instansi || SIUPold.Kualifikasi != SIUP.Kualifikasi || SIUPold.Klasifikasi != SIUP.Klasifikasi || SIUPold.MasaBerlaku != SIUP.MasaBerlaku)) q2 = "Data SIUP dari " + SIUPbeforenomor + SIUPbeforeinstansi + SIUPbeforekualifikasi + SIUPbeforemasaberlaku + SIUPbeforeklasifikasi + " menjadi " + SIUPafternomor + SIUPafterinstansi + SIUPafterkualifikasi + SIUPaftermasaberlaku + SIUPafterklasifikasi + ", ";
                if ((SIUJK != null && SIUJKold != null) && SIUJK.File != SIUJKold.File) r = "File Dokumen SIUJK, ";
                if ((SIUJK != null && SIUJKold != null) && (SIUJKold.Nomor != SIUJK.Nomor || SIUJKold.Instansi != SIUJK.Instansi || SIUJKold.Kualifikasi != SIUJK.Kualifikasi || SIUJKold.Klasifikasi != SIUJK.Klasifikasi || SIUJKold.MasaBerlaku != SIUJK.MasaBerlaku)) r2 = "Data SIUJK dari " + siujkbeforenomor + siujkbeforeinstansi + siujkbeforekualifikasi + siujkbeforemasaberlaku + siujkbeforeklasifikasi + " menjadi " + SIUJKafternomor + SIUJKafterinstansi + SIUJKafterkualifikasi + SIUJKaftermasaberlaku + SIUJKafterklasifikasi + ", ";
                //if (s != null) s = "AKTA TERAKHIR dari " + aktaTerakhir.Nomor + " menjadi " + s + ", ";
                if ((AktaTerakhir != null && AktaTerakhirold != null) && AktaTerakhir.File != AktaTerakhirold.File) s = "File Dokumen Akta Terakhir, ";
                if ((AktaTerakhir != null && AktaTerakhirold != null) && (AktaTerakhirold.Nomor != AktaTerakhir.Nomor || AktaTerakhirold.Notaris != AktaTerakhir.Notaris || AktaTerakhirold.Tanggal != AktaTerakhir.Tanggal)) s2 = "Data AktaTerakhir dari " + AktaTerakhirbeforeNomor + AktaTerakhirbeforeNotaris + AktaTerakhirbeforeTanggal + " menjadi " + AktaTerakhirafterNomor + AktaTerakhirafterNotaris + AktaTerakhirafterTanggal + ", ";
                if (model.BankInfo.NomorRekening != biOld.NomorRekening || model.BankInfo.NamaRekening != biOld.NamaRekening || model.BankInfo.Nama != biOld.NamaBank || model.BankInfo.Cabang != biOld.Cabang) t = "BANK Info dari " + banknamabefore + bankcabangbefore + banknamarekeningbefore + banknomorbefore + " menjadi " + banknamaafter + bankcabangafter + banknamarekeningafter + banknomorafter + ", ";
                RiwayatPengajuanVendor rp = new RiwayatPengajuanVendor() { Komentar = "Pengajuan PERUBAHAN " + a + b + c + d + e + f + g + h + i + i2 + j + j2 + k + k2 + l + l2 + m + m2 + n + n2 + o + o2 + p + p2 + q + q2 + r + r2 + s + s2 + t + " oleh " + CurrentUser.UserName, Status = EStatusVendor.UPDATED, Urutan = 0, Metode = EMetodeVerifikasiVendor.NONE, Waktu = DateTime.Now };
                v.RiwayatPengajuanVendor = new List<RiwayatPengajuanVendor>() { rp };
            }
            else
            {
                RiwayatPengajuanVendor rp = new RiwayatPengajuanVendor() { Komentar = "Pengajuan PERUBAHAN , oleh " + CurrentUser.UserName, Status = EStatusVendor.UPDATED, Urutan = 0, Metode = EMetodeVerifikasiVendor.NONE, Waktu = DateTime.Now };
                v.RiwayatPengajuanVendor = new List<RiwayatPengajuanVendor>() { rp };
            }

            _repository.Save();

            return v.Id;
        }

        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_superadmin, IdLdapConstants.Roles.pRole_procurement_head, IdLdapConstants.Roles.pRole_procurement_end_user, IdLdapConstants.Roles.pRole_procurement_vendor, IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance })]
        public VendorViewModel GetVendorDetail(int id)
        {
            //do something, prevent access etc
            VendorViewModel vm = new VendorViewModel();
            Vendor v = _repository.GetVendor(id);
            if (v == null)
            {
                return null;
            }
            vm.TipeVendor = (int)v.TipeVendor;
            vm.id = v.Id;
            vm.Nama = v.Nama;
            vm.Alamat = v.Alamat;
            vm.NoPengajuan = v.NomorVendor;
            vm.Provinsi = v.Provinsi;
            vm.Kota = v.Kota;
            vm.KodePos = v.KodePos;
            vm.Email = v.Email;
            vm.Website = v.Website;
            vm.Telepon = v.Telepon;
            vm.StatusAkhir = v.StatusAkhir.ToString();

            if (v.BankInfo != null)
            {
                vm.BankInfo.Nama = v.BankInfo.Where(x => x.Active == true).LastOrDefault() != null ? v.BankInfo.Where(x => x.Active == true).LastOrDefault().NamaBank : null;// 1 bank
                vm.BankInfo.NamaRekening = v.BankInfo.Where(x => x.Active == true).LastOrDefault() != null ? v.BankInfo.Where(x => x.Active == true).LastOrDefault().NamaRekening : null;// 1 bank
                vm.BankInfo.Cabang = v.BankInfo.Where(x => x.Active == true).LastOrDefault() != null ? v.BankInfo.Where(x => x.Active == true).LastOrDefault().Cabang : null;// 1 bank
                vm.BankInfo.NomorRekening = v.BankInfo.Where(x => x.Active == true).LastOrDefault() != null ? v.BankInfo.Where(x => x.Active == true).LastOrDefault().NomorRekening : null;// 1 bank
            }
            else { vm.BankInfo = null; }

            List<VendorPersonViewModel> lvp = new List<VendorPersonViewModel>();
            if (v.VendorPerson != null)
            {
                foreach (VendorPerson vp in v.VendorPerson.Where(x => x.Active == true))
                {
                    lvp.Add(new VendorPersonViewModel() { Nama = vp.Nama, Jabatan = vp.Jabatan, Email = vp.Email, Telepon = vp.Telepon });
                }
                vm.VendorPerson = lvp.ToArray();
            }

            //var dokumens = _repository2.GetAllDokumenByVendor(id);
            var dokumens = v.Dokumen.Where(x => x.Active == true);
            List<Dokumen> la = dokumens.Where(x => x.TipeDokumen == EDocumentType.AKTA).ToList();
            List<AktaDokumenDetail> lad = new List<AktaDokumenDetail>();
            foreach (Dokumen d in la)
            {
                AktaDokumenDetail a = (AktaDokumenDetail)d;
                lad.Add(a);
            }
            //vm.NPWP.id = dokumens.Where(x => x.TipeDokumen == EDocumentType.NPWP).Select(a=>a.Id).FirstOrDefault().ToString() ?? "";
            //vm.PKP.id = dokumens.Where(x => x.TipeDokumen == EDocumentType.PKP).Select(a => a.Id).FirstOrDefault().ToString() ?? "";
            //vm.Akta.id = lad.OrderBy(x => x.order).Select(a => a.Id).FirstOrDefault().ToString() ?? ""; //suppose only 2 aktas
            //vm.AktaTerakhir.id = lad.Where(x=>x.order!=1).OrderByDescending(x => x.order).Select(a => a.Id).FirstOrDefault().ToString() ?? ""; //latest
            //vm.TDP.id = dokumens.Where(x => x.TipeDokumen == EDocumentType.TDP).Select(a => a.Id).FirstOrDefault().ToString() ?? "";
            //vm.SIUP.id = dokumens.Where(x => x.TipeDokumen == EDocumentType.SIUP).Select(a => a.Id).FirstOrDefault().ToString() ?? "";
            //vm.SIUJK.id = dokumens.Where(x => x.TipeDokumen == EDocumentType.SIUJK).Select(a => a.Id).FirstOrDefault().ToString() ?? "";
            DokumenDetail npwp = (DokumenDetail)dokumens.Where(x => x.TipeDokumen == EDocumentType.NPWP).FirstOrDefault();
            if (npwp != null) vm.NPWP = new DokumenDetailViewModel() { id = npwp.Id.ToString(), Nomor = npwp.Nomor, File = npwp.File, ContentType = npwp.ContentType };
            DokumenDetail ktp = (DokumenDetail)dokumens.Where(x => x.TipeDokumen == EDocumentType.KTP).FirstOrDefault();
            if (ktp != null) vm.KTP = new DokumenDetailViewModel() { id = ktp.Id.ToString(), Nomor = ktp.Nomor, File = ktp.File, ContentType = ktp.ContentType };
            DokumenDetail KTPPemilik = (DokumenDetail)dokumens.Where(x => x.TipeDokumen == EDocumentType.KTPPemilik).FirstOrDefault();
            if (KTPPemilik != null) vm.KTPPemilik = new DokumenDetailViewModel() { id = KTPPemilik.Id.ToString(), Nomor = KTPPemilik.Nomor, File = KTPPemilik.File, ContentType = KTPPemilik.ContentType };
            DokumenDetail NPWPPemilik = (DokumenDetail)dokumens.Where(x => x.TipeDokumen == EDocumentType.NPWPPemilik).FirstOrDefault();
            if (NPWPPemilik != null) vm.NPWPPemilik = new DokumenDetailViewModel() { id = NPWPPemilik.Id.ToString(), Nomor = NPWPPemilik.Nomor, File = NPWPPemilik.File, ContentType = NPWPPemilik.ContentType };
            DokumenDetail DOMISILI = (DokumenDetail)dokumens.Where(x => x.TipeDokumen == EDocumentType.DOMISILI).FirstOrDefault();
            if (DOMISILI != null) vm.DOMISILI = new DokumenDetailViewModel() { id = DOMISILI.Id.ToString(), Nomor = DOMISILI.Nomor, File = DOMISILI.File, ContentType = DOMISILI.ContentType };
            DokumenDetail pkp = (DokumenDetail)dokumens.Where(x => x.TipeDokumen == EDocumentType.PKP).FirstOrDefault();
            if (pkp != null) vm.PKP = new DokumenDetailViewModel() { id = pkp.Id.ToString(), Nomor = pkp.Nomor, File = pkp.File, ContentType = pkp.ContentType };
            AktaDokumenDetail akta = (AktaDokumenDetail)lad.OrderBy(x => x.order).FirstOrDefault();
            if (akta != null) vm.Akta = new AktaDokumenDetailViewModel() { id = akta.Id.ToString(), Nomor = akta.Nomor, File = akta.File, Notaris = akta.Notaris, Tanggal = akta.Tanggal, ContentType = akta.ContentType };
            AktaDokumenDetail aktaTerakhir = (AktaDokumenDetail)lad.Where(x => x.order != 1).OrderByDescending(x => x.order).FirstOrDefault();
            if (aktaTerakhir != null) vm.AktaTerakhir = new AktaDokumenDetailViewModel() { id = aktaTerakhir.Id.ToString(), Nomor = aktaTerakhir.Nomor, File = aktaTerakhir.File, Notaris = aktaTerakhir.Notaris, Tanggal = aktaTerakhir.Tanggal, ContentType = aktaTerakhir.ContentType };
            IzinUsahaDokumenDetail tdp = (IzinUsahaDokumenDetail)dokumens.Where(x => x.TipeDokumen == EDocumentType.TDP).FirstOrDefault();
            if (tdp != null) vm.TDP = new IzinUsahaDokumenDetailViewModel() { id = tdp.Id.ToString(), Nomor = tdp.Nomor, File = tdp.File, Instansi = tdp.Instansi, MasaBerlaku = tdp.MasaBerlaku, Klasifikasi = tdp.Klasifikasi, Kualifikasi = tdp.Kualifikasi, ContentType = tdp.ContentType };
            IzinUsahaDokumenDetail siup = (IzinUsahaDokumenDetail)dokumens.Where(x => x.TipeDokumen == EDocumentType.SIUP).FirstOrDefault();
            if (siup != null) vm.SIUP = new IzinUsahaDokumenDetailViewModel() { id = siup.Id.ToString(), Nomor = siup.Nomor, File = siup.File, Instansi = siup.Instansi, MasaBerlaku = siup.MasaBerlaku, Klasifikasi = siup.Klasifikasi, Kualifikasi = siup.Kualifikasi, ContentType = siup.ContentType };
            IzinUsahaDokumenDetail siujk = (IzinUsahaDokumenDetail)dokumens.Where(x => x.TipeDokumen == EDocumentType.SIUJK).FirstOrDefault();
            if (siujk != null) vm.SIUJK = new IzinUsahaDokumenDetailViewModel() { id = siujk.Id.ToString(), Nomor = siujk.Nomor, File = siujk.File, Instansi = siujk.Instansi, MasaBerlaku = siujk.MasaBerlaku, Klasifikasi = siujk.Klasifikasi, Kualifikasi = siujk.Kualifikasi, ContentType = siujk.ContentType };

            return vm;
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_vendor)]
        public VendorViewModel GetVendorDetailRekanan()
        {
            //string uid = UserId();
            return GetVendorDetail(_repository.GetVendorByUser(UserId()).Id);
        }

        public string CopyFileVendor(string uidFileName, int id)
        {
            //if (d == null) return false;
            var uploadPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            string fileLoc = uploadPath + FILE_TEMP_PATH + uidFileName;
            string fileVendorPathSave = FILE_VENDOR_PATH + id + @"\";
            if (Directory.Exists(uploadPath + fileVendorPathSave) == false)
            {
                Directory.CreateDirectory(uploadPath + fileVendorPathSave);
            }
            try
            {
                FileInfo fi = new FileInfo(fileLoc);
                fi.MoveTo(uploadPath + fileVendorPathSave + uidFileName);
            }
            catch (IOException ei)
            {
                return "";
            }
            return fileVendorPathSave + uidFileName;
        }

        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_superadmin, IdLdapConstants.Roles.pRole_procurement_head, IdLdapConstants.Roles.pRole_procurement_end_user, IdLdapConstants.Roles.pRole_procurement_vendor, IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance })]
        public List<StatusVerifikasiViewModel> GetStatusVerifikasi(int id)
        {
            Vendor v = _repository.GetVendor(id);
            List<StatusVerifikasiViewModel> sv = new List<StatusVerifikasiViewModel>();
            if (v != null)
            {
                RiwayatPengajuanVendor desk = v.RiwayatPengajuanVendor.Where(x => x.Metode == EMetodeVerifikasiVendor.DESK).OrderByDescending(x => x.Urutan).FirstOrDefault();
                if (desk != null)
                {
                    StatusVerifikasiViewModel s = new StatusVerifikasiViewModel() { Comment = desk.Komentar, Waktu = desk.Waktu, Metode = EMetodeVerifikasiVendor.DESK.ToString() };
                    sv.Add(s);
                }
                desk = v.RiwayatPengajuanVendor.Where(x => x.Metode == EMetodeVerifikasiVendor.PHONE).OrderByDescending(x => x.Urutan).FirstOrDefault();
                if (desk != null)
                {
                    StatusVerifikasiViewModel s = new StatusVerifikasiViewModel() { Comment = desk.Komentar, Waktu = desk.Waktu, Metode = EMetodeVerifikasiVendor.PHONE.ToString() };
                    sv.Add(s);
                }
                desk = v.RiwayatPengajuanVendor.Where(x => x.Metode == EMetodeVerifikasiVendor.VISIT).OrderByDescending(x => x.Urutan).FirstOrDefault();
                if (desk != null)
                {
                    StatusVerifikasiViewModel s = new StatusVerifikasiViewModel() { Comment = desk.Komentar, Waktu = desk.Waktu, Metode = EMetodeVerifikasiVendor.VISIT.ToString() };
                    sv.Add(s);
                }
            }
            return sv;
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_staff)]
        [HttpGet]
        public string DeskVerification(int id, string comment)
        {
            return VerificationSubmit(id, comment, EMetodeVerifikasiVendor.DESK);
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_staff)]
        [HttpGet]
        public string PhoneVerification(int id, string comment)
        {
            return VerificationSubmit(id, comment, EMetodeVerifikasiVendor.PHONE);
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_staff)]
        [HttpGet]
        public string VisitVerification(int id, string comment)
        {
            return VerificationSubmit(id, comment, EMetodeVerifikasiVendor.VISIT);
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_staff)]
        public string VerificationSubmit(int id, string comment, EMetodeVerifikasiVendor metode)
        {
            Vendor v = _repository.GetVendor(id);
            if (v != null)
            {
                int vd = v.RiwayatPengajuanVendor.Where(x => x.Metode == EMetodeVerifikasiVendor.DESK).Count() > 0 ? (metode != EMetodeVerifikasiVendor.DESK) ? 1 : 0 : 0;
                int vp = v.RiwayatPengajuanVendor.Where(x => x.Metode == EMetodeVerifikasiVendor.PHONE).Count() > 0 ? (metode != EMetodeVerifikasiVendor.PHONE) ? 1 : 0 : 0;
                int vv = v.RiwayatPengajuanVendor.Where(x => x.Metode == EMetodeVerifikasiVendor.VISIT).Count() > 0 ? (metode != EMetodeVerifikasiVendor.VISIT) ? 1 : 0 : 0;

                EStatusVendor status = vd + vp + vv == 0 ? EStatusVendor.PASS_1 :
                    vd + vp + vv == 1 ? EStatusVendor.PASS_2 :
                    vd + vp + vv == 2 ? EStatusVendor.PASS_3 :
                    vd + vp + vv == 3 ? EStatusVendor.VERIFIED :
                    EStatusVendor.VERIFIED;
                int urutan = v.RiwayatPengajuanVendor.OrderByDescending(x => x.Urutan).FirstOrDefault() != null ?
                    v.RiwayatPengajuanVendor.OrderByDescending(x => x.Urutan).Select(x => x.Urutan).FirstOrDefault() : 0;
                RiwayatPengajuanVendor rp = new RiwayatPengajuanVendor()
                {
                    Komentar = "Verifikasi " + status.ToString() + ", pesan: " + comment + ", oleh " + CurrentUser.UserName,
                    Urutan = urutan + 1,
                    Status = status,
                    Metode = metode,
                    Waktu = DateTime.Now
                };
                v.RiwayatPengajuanVendor.Add(rp);
                v.StatusAkhir = status;

                _repository.Save();
                return "Success!";
            }
            return "Vendor not found!";
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_staff)]
        [HttpGet]
        public string TolakPerubahan(int id, string msg)
        {
            Vendor v = _repository.GetVendor(id);
            if (v != null)
            {
                RiwayatPengajuanVendor rp = new RiwayatPengajuanVendor()
                {
                    Komentar = "Pengajuan perubahan DITOLAK dengan alasan: " + msg,
                    Metode = EMetodeVerifikasiVendor.NONE,
                    Status = EStatusVendor.REJECTED,
                    Waktu = DateTime.Now
                };
                v.RiwayatPengajuanVendor.Add(rp);
                v.StatusAkhir = EStatusVendor.REJECTED;
                _repository.Save();
                return "Sukses.";
            }
            return "Gagal.";
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_staff)]
        [HttpGet]
        public string DaftarHitam(int id, string msg)
        {
            Vendor v = _repository.GetVendor(id);
            if (v != null)
            {
                RiwayatPengajuanVendor rp = new RiwayatPengajuanVendor()
                {
                    Komentar = "DAFTAR HITAM. alasan: " + msg,
                    Metode = EMetodeVerifikasiVendor.NONE,
                    Status = EStatusVendor.BLACKLIST,
                    Waktu = DateTime.Now
                };
                v.RiwayatPengajuanVendor.Add(rp);
                v.StatusAkhir = EStatusVendor.BLACKLIST;
                _repository.Save();
                return "Sukses.";
            }
            return "Gagal.";
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_staff)]
        [HttpGet]
        public string DaftarPutih(int id, string msg)
        {
            Vendor v = _repository.GetVendor(id);
            if (v != null && v.StatusAkhir == EStatusVendor.BLACKLIST)
            {
                RiwayatPengajuanVendor rp = new RiwayatPengajuanVendor()
                {
                    Komentar = "KELUAR dari DAFTAR HITAM. alasan: " + msg,
                    Metode = EMetodeVerifikasiVendor.NONE,
                    Status = EStatusVendor.VERIFIED,
                    Waktu = DateTime.Now
                };
                v.RiwayatPengajuanVendor.Add(rp);
                v.StatusAkhir = EStatusVendor.VERIFIED;
                _repository.Save();
                return "Sukses.";
            }
            return "Gagal.";
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_staff)]
        [HttpGet]
        public string SetujuiPerubahan(int id)
        {
            Vendor v = _repository.GetVendor(id);
            if (v != null && v.StatusAkhir == EStatusVendor.UPDATED)
            {
                RiwayatPengajuanVendor rp = new RiwayatPengajuanVendor()
                {
                    Komentar = "Pengajuan perubahan DISETUJUI, oleh " + CurrentUser.UserName,
                    Metode = EMetodeVerifikasiVendor.NONE,
                    Status = EStatusVendor.VERIFIED,
                    Waktu = DateTime.Now
                };
                v.StatusAkhir = EStatusVendor.VERIFIED;
                v.RiwayatPengajuanVendor.Add(rp);
                _repository.Save();
                return "Sukses.";
            }
            return "Gagal.";
        }

        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_vendor })]
        public List<RiwayatPengajuanVendorViewModel> GetRiwayatVendor(int id, int skip = 0, int limit = 5)
        {
            Vendor v = _repository.GetVendor(id);
            if (v != null)
            {
                List<RiwayatPengajuanVendorViewModel> rv = v.RiwayatPengajuanVendor.Select(
                    x => new RiwayatPengajuanVendorViewModel()
                    {
                        Tanggal = ((DateTime)x.Waktu).ToString("yyyy-MM-dd HH:mm"),
                        Pesan = x.Komentar
                    }
                    ).OrderByDescending(x => x.Tanggal).Skip(skip).Take(limit).ToList();
                return rv;
            }
            return new List<RiwayatPengajuanVendorViewModel>();
        }


        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_vendor })]
        public List<RiwayatPengajuanVendorViewModel> GetRiwayatVendorDetail()
        {
            //string uid = UserId();
            return GetRiwayatVendor(_repository.GetVendorByUser(UserId()).Id);
        }


        [Authorize]
        private async Task<string> TestCreateUser(string username, string password, string displayname, Guid guid)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var user = new { UserName = username, DisplayName = displayname, NewPassword = password, IsLdapUser = false, guid = guid };
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(user);
            HttpContent content = new StringContent(json);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            //var stringContent = new StringContent(user.ToString(), System.Text.Encoding.UTF8, "application/json");
            //stringContent.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");
            HttpResponseMessage reply = await client.PostAsync(
                    string.Format("{0}/{1}", IdLdapConstants.IDM.Url, "admin/NewVendorUser"), content);
            if (reply.IsSuccessStatusCode)
            {
                return reply.Content.ReadAsStringAsync().Result;
            };
            return "";
        }

        //[ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.Roles.pRole_procurement_head, IdLdapConstants.Roles.pRole_procurement_manager })]
        //public List<string> GetAllVendor()
        //{
        //    return _repository.GetAllVendors().Select(x => x.Nama).Distinct().ToList();

        //}

        [HttpGet]
        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_user, IdLdapConstants.Roles.pRole_procurement_admin, IdLdapConstants.Roles.pRole_procurement_head, IdLdapConstants.Roles.pRole_procurement_manager })]
        public List<VendorViewModel> GetAllVendor()
        {
            return _repository.GetAllVendors().Select(x => new VendorViewModel()
            {
                id = x.Id,
                Nama = x.Nama
            }).ToList();
        }

        //[ApiAuthorize]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public int CheckNPWP(string npwp)
        {
            var result = _repository.CheckNPWP(npwp);
            return result;
        }

        //public IHttpActionResult GetVendorBank(Guid Id, int Vendor_Id)
        //{
        //    return Json(_repository.GetBankInfo(Id, Vendor_Id));
        //}
        //[ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_superadmin, IdLdapConstants.Roles.pRole_procurement_head, IdLdapConstants.Roles.pRole_procurement_end_user, IdLdapConstants.Roles.pRole_procurement_vendor, IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance })]
        //public List<VendorViewModel> GetVendors(string tipe, string status, int limit, string search)
        //{
        //    var lv =
        //    _repository.GetVendors(tipe != null ? (ETipeVendor)Enum.Parse(typeof(ETipeVendor), tipe) : ETipeVendor.NONE,
        //        status != null ? (EStatusVendor)Enum.Parse(typeof(EStatusVendor), status) : EStatusVendor.NONE
        //        , limit, search);
        //    if (lv != null)
        //        return lv.Where(x => search == null || x.Nama.ToLower().Contains(search.ToLower())).Select(x => new VendorViewModel() { id = x.Id, Nama = x.Nama, Alamat = x.Alamat, Provinsi = x.Provinsi, Telepon = x.Telepon, Email = x.Email, Website = x.Website, NoPengajuan = x.NomorVendor }).ToList();
        //    return new List<VendorViewModel>();
        //}

        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_superadmin, IdLdapConstants.Roles.pRole_procurement_head, IdLdapConstants.Roles.pRole_procurement_end_user, IdLdapConstants.Roles.pRole_procurement_vendor, IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance })]
        public List<VendorViewModel> GetVendorDetails(string tipe, string status, int limit, string search)
        {
            //do something, prevent access etc

            //List<Vendor> xx = _repository.GetAllVendor().OrderBy(x => x.Nama).ToList();
            List<Vendor> xx =
            _repository.GetVendors(tipe != null ? (ETipeVendor)Enum.Parse(typeof(ETipeVendor), tipe) : ETipeVendor.NONE,
                status != null ? (EStatusVendor)Enum.Parse(typeof(EStatusVendor), status) : EStatusVendor.NONE
                , limit, search).OrderBy(x => x.Nama).ToList();
            List<VendorViewModel> lu = new List<VendorViewModel>();
            foreach (var item in xx)
            {
                int id = item.Id;
                VendorViewModel vm = new VendorViewModel();

                Vendor v = _repository.GetVendor(id);
                if (v == null)
                {
                    return null;
                }
                vm.TipeVendor = (int)v.TipeVendor;
                vm.id = v.Id;
                vm.Nama = v.Nama;
                vm.Alamat = v.Alamat;
                vm.NoPengajuan = v.NomorVendor;
                vm.Provinsi = v.Provinsi;
                vm.Kota = v.Kota;
                vm.KodePos = v.KodePos;
                vm.Email = v.Email;
                vm.Website = v.Website;
                vm.Telepon = v.Telepon;
                vm.StatusAkhir = v.StatusAkhir.ToString();

                if (v.BankInfo != null)
                {
                    vm.BankInfo.Nama = v.BankInfo.Where(x => x.Active == true).LastOrDefault() != null ? v.BankInfo.Where(x => x.Active == true).LastOrDefault().NamaBank : null;// 1 bank
                    vm.BankInfo.NamaRekening = v.BankInfo.Where(x => x.Active == true).LastOrDefault() != null ? v.BankInfo.Where(x => x.Active == true).LastOrDefault().NamaRekening : null;// 1 bank
                    vm.BankInfo.Cabang = v.BankInfo.Where(x => x.Active == true).LastOrDefault() != null ? v.BankInfo.Where(x => x.Active == true).LastOrDefault().Cabang : null;// 1 bank
                    vm.BankInfo.NomorRekening = v.BankInfo.Where(x => x.Active == true).LastOrDefault() != null ? v.BankInfo.Where(x => x.Active == true).LastOrDefault().NomorRekening : null;// 1 bank
                }
                else { vm.BankInfo = null; }

                List<VendorPersonViewModel> lvp = new List<VendorPersonViewModel>();
                if (v.VendorPerson != null)
                {
                    foreach (VendorPerson vp in v.VendorPerson.Where(x => x.Active == true))
                    {
                        lvp.Add(new VendorPersonViewModel() { Nama = vp.Nama, Jabatan = vp.Jabatan, Email = vp.Email, Telepon = vp.Telepon });
                    }
                    vm.VendorPerson = lvp.ToArray();
                }

                //var dokumens = _repository2.GetAllDokumenByVendor(id);
                var dokumens = v.Dokumen.Where(x => x.Active == true);
                List<Dokumen> la = dokumens.Where(x => x.TipeDokumen == EDocumentType.AKTA).ToList();
                List<AktaDokumenDetail> lad = new List<AktaDokumenDetail>();
                foreach (Dokumen d in la)
                {
                    AktaDokumenDetail a = (AktaDokumenDetail)d;
                    lad.Add(a);
                }
                DokumenDetail npwp = (DokumenDetail)dokumens.Where(x => x.TipeDokumen == EDocumentType.NPWP).FirstOrDefault();
                if (npwp != null) vm.NPWP = new DokumenDetailViewModel() { id = npwp.Id.ToString(), Nomor = npwp.Nomor, File = npwp.File, ContentType = npwp.ContentType };
                DokumenDetail ktp = (DokumenDetail)dokumens.Where(x => x.TipeDokumen == EDocumentType.KTP).FirstOrDefault();
                if (ktp != null) vm.KTP = new DokumenDetailViewModel() { id = ktp.Id.ToString(), Nomor = ktp.Nomor, File = ktp.File, ContentType = ktp.ContentType };
                DokumenDetail KTPPemilik = (DokumenDetail)dokumens.Where(x => x.TipeDokumen == EDocumentType.KTPPemilik).FirstOrDefault();
                if (KTPPemilik != null) vm.KTPPemilik = new DokumenDetailViewModel() { id = KTPPemilik.Id.ToString(), Nomor = KTPPemilik.Nomor, File = KTPPemilik.File, ContentType = KTPPemilik.ContentType };
                DokumenDetail NPWPPemilik = (DokumenDetail)dokumens.Where(x => x.TipeDokumen == EDocumentType.NPWPPemilik).FirstOrDefault();
                if (NPWPPemilik != null) vm.NPWPPemilik = new DokumenDetailViewModel() { id = NPWPPemilik.Id.ToString(), Nomor = NPWPPemilik.Nomor, File = NPWPPemilik.File, ContentType = NPWPPemilik.ContentType };
                DokumenDetail DOMISILI = (DokumenDetail)dokumens.Where(x => x.TipeDokumen == EDocumentType.DOMISILI).FirstOrDefault();
                if (DOMISILI != null) vm.DOMISILI = new DokumenDetailViewModel() { id = DOMISILI.Id.ToString(), Nomor = DOMISILI.Nomor, File = DOMISILI.File, ContentType = DOMISILI.ContentType };
                DokumenDetail pkp = (DokumenDetail)dokumens.Where(x => x.TipeDokumen == EDocumentType.PKP).FirstOrDefault();
                if (pkp != null) vm.PKP = new DokumenDetailViewModel() { id = pkp.Id.ToString(), Nomor = pkp.Nomor, File = pkp.File, ContentType = pkp.ContentType };
                AktaDokumenDetail akta = (AktaDokumenDetail)lad.OrderBy(x => x.order).FirstOrDefault();
                if (akta != null) vm.Akta = new AktaDokumenDetailViewModel() { id = akta.Id.ToString(), Nomor = akta.Nomor, File = akta.File, Notaris = akta.Notaris, Tanggal = akta.Tanggal, ContentType = akta.ContentType };
                AktaDokumenDetail aktaTerakhir = (AktaDokumenDetail)lad.Where(x => x.order != 1).OrderByDescending(x => x.order).FirstOrDefault();
                if (aktaTerakhir != null) vm.AktaTerakhir = new AktaDokumenDetailViewModel() { id = aktaTerakhir.Id.ToString(), Nomor = aktaTerakhir.Nomor, File = aktaTerakhir.File, Notaris = aktaTerakhir.Notaris, Tanggal = aktaTerakhir.Tanggal, ContentType = aktaTerakhir.ContentType };
                IzinUsahaDokumenDetail tdp = (IzinUsahaDokumenDetail)dokumens.Where(x => x.TipeDokumen == EDocumentType.TDP).FirstOrDefault();
                if (tdp != null) vm.TDP = new IzinUsahaDokumenDetailViewModel() { id = tdp.Id.ToString(), Nomor = tdp.Nomor, File = tdp.File, Instansi = tdp.Instansi, MasaBerlaku = tdp.MasaBerlaku, Klasifikasi = tdp.Klasifikasi, Kualifikasi = tdp.Kualifikasi, ContentType = tdp.ContentType };
                IzinUsahaDokumenDetail siup = (IzinUsahaDokumenDetail)dokumens.Where(x => x.TipeDokumen == EDocumentType.SIUP).FirstOrDefault();
                if (siup != null) vm.SIUP = new IzinUsahaDokumenDetailViewModel() { id = siup.Id.ToString(), Nomor = siup.Nomor, File = siup.File, Instansi = siup.Instansi, MasaBerlaku = siup.MasaBerlaku, Klasifikasi = siup.Klasifikasi, Kualifikasi = siup.Kualifikasi, ContentType = siup.ContentType };
                IzinUsahaDokumenDetail siujk = (IzinUsahaDokumenDetail)dokumens.Where(x => x.TipeDokumen == EDocumentType.SIUJK).FirstOrDefault();
                if (siujk != null) vm.SIUJK = new IzinUsahaDokumenDetailViewModel() { id = siujk.Id.ToString(), Nomor = siujk.Nomor, File = siujk.File, Instansi = siujk.Instansi, MasaBerlaku = siujk.MasaBerlaku, Klasifikasi = siujk.Klasifikasi, Kualifikasi = siujk.Kualifikasi, ContentType = siujk.ContentType };


                lu.Add(vm);
            }

            return lu.Where(x => (long.Parse(x.TDP.MasaBerlaku.GetValueOrDefault(DateTime.Now).ToString("yyyyMMdd")) - long.Parse(DateTime.Now.ToString("yyyyMMdd"))) <= 0 && x.TipeVendor == 3 || (long.Parse(x.SIUP.MasaBerlaku.GetValueOrDefault(DateTime.Now).ToString("yyyyMMdd")) - long.Parse(DateTime.Now.ToString("yyyyMMdd"))) <= 0 && x.TipeVendor == 3 || (long.Parse(x.SIUJK.MasaBerlaku.GetValueOrDefault(DateTime.Now).ToString("yyyyMMdd")) - long.Parse(DateTime.Now.ToString("yyyyMMdd"))) <= 0 && x.TipeVendor == 3).ToList();
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head, IdLdapConstants.Roles.pRole_approver,
                                           IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                            IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_vendor,
                                            IdLdapConstants.Roles.pRole_direksi)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public IHttpActionResult DokumenVendorKadaluarsaCount()
        {
            //do something, prevent access etc

            List<Vendor> xx = _repository.GetAllVendor().ToList();
            List<VendorViewModel> lu = new List<VendorViewModel>();
            foreach (var item in xx)
            {
                int id = item.Id;
                VendorViewModel vm = new VendorViewModel();

                Vendor v = _repository.GetVendor(id);

                vm.TipeVendor = (int)v.TipeVendor;
                vm.id = v.Id;
                vm.Nama = v.Nama;
                vm.Alamat = v.Alamat;
                vm.NoPengajuan = v.NomorVendor;
                vm.Provinsi = v.Provinsi;
                vm.Kota = v.Kota;
                vm.KodePos = v.KodePos;
                vm.Email = v.Email;
                vm.Website = v.Website;
                vm.Telepon = v.Telepon;
                vm.StatusAkhir = v.StatusAkhir.ToString();

                if (v.BankInfo != null)
                {
                    vm.BankInfo.Nama = v.BankInfo.Where(x => x.Active == true).LastOrDefault() != null ? v.BankInfo.Where(x => x.Active == true).LastOrDefault().NamaBank : null;// 1 bank
                    vm.BankInfo.NamaRekening = v.BankInfo.Where(x => x.Active == true).LastOrDefault() != null ? v.BankInfo.Where(x => x.Active == true).LastOrDefault().NamaRekening : null;// 1 bank
                    vm.BankInfo.Cabang = v.BankInfo.Where(x => x.Active == true).LastOrDefault() != null ? v.BankInfo.Where(x => x.Active == true).LastOrDefault().Cabang : null;// 1 bank
                    vm.BankInfo.NomorRekening = v.BankInfo.Where(x => x.Active == true).LastOrDefault() != null ? v.BankInfo.Where(x => x.Active == true).LastOrDefault().NomorRekening : null;// 1 bank
                }
                else { vm.BankInfo = null; }

                List<VendorPersonViewModel> lvp = new List<VendorPersonViewModel>();
                if (v.VendorPerson != null)
                {
                    foreach (VendorPerson vp in v.VendorPerson.Where(x => x.Active == true))
                    {
                        lvp.Add(new VendorPersonViewModel() { Nama = vp.Nama, Jabatan = vp.Jabatan, Email = vp.Email, Telepon = vp.Telepon });
                    }
                    vm.VendorPerson = lvp.ToArray();
                }

                //var dokumens = _repository2.GetAllDokumenByVendor(id);
                var dokumens = v.Dokumen.Where(x => x.Active == true);
                List<Dokumen> la = dokumens.Where(x => x.TipeDokumen == EDocumentType.AKTA).ToList();
                List<AktaDokumenDetail> lad = new List<AktaDokumenDetail>();
                foreach (Dokumen d in la)
                {
                    AktaDokumenDetail a = (AktaDokumenDetail)d;
                    lad.Add(a);
                }
                DokumenDetail npwp = (DokumenDetail)dokumens.Where(x => x.TipeDokumen == EDocumentType.NPWP).FirstOrDefault();
                if (npwp != null) vm.NPWP = new DokumenDetailViewModel() { id = npwp.Id.ToString(), Nomor = npwp.Nomor, File = npwp.File, ContentType = npwp.ContentType };
                DokumenDetail ktp = (DokumenDetail)dokumens.Where(x => x.TipeDokumen == EDocumentType.KTP).FirstOrDefault();
                if (ktp != null) vm.KTP = new DokumenDetailViewModel() { id = ktp.Id.ToString(), Nomor = ktp.Nomor, File = ktp.File, ContentType = ktp.ContentType };
                DokumenDetail KTPPemilik = (DokumenDetail)dokumens.Where(x => x.TipeDokumen == EDocumentType.KTPPemilik).FirstOrDefault();
                if (KTPPemilik != null) vm.KTPPemilik = new DokumenDetailViewModel() { id = KTPPemilik.Id.ToString(), Nomor = KTPPemilik.Nomor, File = KTPPemilik.File, ContentType = KTPPemilik.ContentType };
                DokumenDetail NPWPPemilik = (DokumenDetail)dokumens.Where(x => x.TipeDokumen == EDocumentType.NPWPPemilik).FirstOrDefault();
                if (NPWPPemilik != null) vm.NPWPPemilik = new DokumenDetailViewModel() { id = NPWPPemilik.Id.ToString(), Nomor = NPWPPemilik.Nomor, File = NPWPPemilik.File, ContentType = NPWPPemilik.ContentType };
                DokumenDetail DOMISILI = (DokumenDetail)dokumens.Where(x => x.TipeDokumen == EDocumentType.DOMISILI).FirstOrDefault();
                if (DOMISILI != null) vm.DOMISILI = new DokumenDetailViewModel() { id = DOMISILI.Id.ToString(), Nomor = DOMISILI.Nomor, File = DOMISILI.File, ContentType = DOMISILI.ContentType };
                DokumenDetail pkp = (DokumenDetail)dokumens.Where(x => x.TipeDokumen == EDocumentType.PKP).FirstOrDefault();
                if (pkp != null) vm.PKP = new DokumenDetailViewModel() { id = pkp.Id.ToString(), Nomor = pkp.Nomor, File = pkp.File, ContentType = pkp.ContentType };
                AktaDokumenDetail akta = (AktaDokumenDetail)lad.OrderBy(x => x.order).FirstOrDefault();
                if (akta != null) vm.Akta = new AktaDokumenDetailViewModel() { id = akta.Id.ToString(), Nomor = akta.Nomor, File = akta.File, Notaris = akta.Notaris, Tanggal = akta.Tanggal, ContentType = akta.ContentType };
                AktaDokumenDetail aktaTerakhir = (AktaDokumenDetail)lad.Where(x => x.order != 1).OrderByDescending(x => x.order).FirstOrDefault();
                if (aktaTerakhir != null) vm.AktaTerakhir = new AktaDokumenDetailViewModel() { id = aktaTerakhir.Id.ToString(), Nomor = aktaTerakhir.Nomor, File = aktaTerakhir.File, Notaris = aktaTerakhir.Notaris, Tanggal = aktaTerakhir.Tanggal, ContentType = aktaTerakhir.ContentType };
                IzinUsahaDokumenDetail tdp = (IzinUsahaDokumenDetail)dokumens.Where(x => x.TipeDokumen == EDocumentType.TDP).FirstOrDefault();
                if (tdp != null) vm.TDP = new IzinUsahaDokumenDetailViewModel() { id = tdp.Id.ToString(), Nomor = tdp.Nomor, File = tdp.File, Instansi = tdp.Instansi, MasaBerlaku = tdp.MasaBerlaku, Klasifikasi = tdp.Klasifikasi, Kualifikasi = tdp.Kualifikasi, ContentType = tdp.ContentType };
                IzinUsahaDokumenDetail siup = (IzinUsahaDokumenDetail)dokumens.Where(x => x.TipeDokumen == EDocumentType.SIUP).FirstOrDefault();
                if (siup != null) vm.SIUP = new IzinUsahaDokumenDetailViewModel() { id = siup.Id.ToString(), Nomor = siup.Nomor, File = siup.File, Instansi = siup.Instansi, MasaBerlaku = siup.MasaBerlaku, Klasifikasi = siup.Klasifikasi, Kualifikasi = siup.Kualifikasi, ContentType = siup.ContentType };
                IzinUsahaDokumenDetail siujk = (IzinUsahaDokumenDetail)dokumens.Where(x => x.TipeDokumen == EDocumentType.SIUJK).FirstOrDefault();
                if (siujk != null) vm.SIUJK = new IzinUsahaDokumenDetailViewModel() { id = siujk.Id.ToString(), Nomor = siujk.Nomor, File = siujk.File, Instansi = siujk.Instansi, MasaBerlaku = siujk.MasaBerlaku, Klasifikasi = siujk.Klasifikasi, Kualifikasi = siujk.Kualifikasi, ContentType = siujk.ContentType };


                lu.Add(vm);
            }

            lu = lu.Where(x => (long.Parse(x.TDP.MasaBerlaku.GetValueOrDefault(DateTime.Now).ToString("yyyyMMdd")) - long.Parse(DateTime.Now.ToString("yyyyMMdd"))) <= 0 && x.TipeVendor == 3 || (long.Parse(x.SIUP.MasaBerlaku.GetValueOrDefault(DateTime.Now).ToString("yyyyMMdd")) - long.Parse(DateTime.Now.ToString("yyyyMMdd"))) <= 0 && x.TipeVendor == 3 || (long.Parse(x.SIUJK.MasaBerlaku.GetValueOrDefault(DateTime.Now).ToString("yyyyMMdd")) - long.Parse(DateTime.Now.ToString("yyyyMMdd"))) <= 0 && x.TipeVendor == 3).ToList();
            var le = lu.Count();
            return Json(le);

        }

        [HttpGet]
        public List<VendorViewModel> GetAllVendorWNon()
        {
            return _repository.GetAllVendorsWNon().Select(x => new VendorViewModel()
            {
                id = x.Id,
                Nama = x.Nama
            }).ToList();
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head, IdLdapConstants.Roles.pRole_approver,
                                           IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                            IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_vendor,
                                            IdLdapConstants.Roles.pRole_direksi)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public IHttpActionResult ListVendorNonRegister()
        {
            string search = HttpContext.Current.Request["search"].ToString();
            int start = Convert.ToInt32(HttpContext.Current.Request["start"]);
            int length = Convert.ToInt32(HttpContext.Current.Request["length"]);
            return Json(_repository.GetDataListVendorNonRegister(search, start, length));
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_staff)]
        [HttpPost]
        //public async Task<int> AddVendorNonRegistrasi([FromBody]VendorViewModel model)
        public int AddVendorNonRegistrasi([FromBody]VendorViewModel model)
        {
            //access?
            Vendor v = new Vendor
            {
                //TipeVendor = (ETipeVendor)model.TipeVendor,
                Id = model.id,
                TipeVendor = ETipeVendor.NON_REGISTER,
                NomorVendor = GenerateNomorVendor(4),
                Nama = model.Nama,
                Alamat = model.Alamat,
                Provinsi = model.Provinsi,
                Kota = model.Kota,
                KodePos = model.KodePos,
                Email = model.Email,
                Website = model.Website,
                Telepon = model.Telepon
                //,StatusAkhir = EStatusVendor.VERIFIED
            };

            

            //RiwayatPengajuanVendor rp = new RiwayatPengajuanVendor() { Komentar = "Rekanan baru TERVERIFIKASI, oleh " + CurrentUser.UserName, Status = EStatusVendor.NEW, Urutan = 0, Metode = EMetodeVerifikasiVendor.NONE, Waktu = DateTime.Now };
            //v.RiwayatPengajuanVendor = new List<RiwayatPengajuanVendor>() { rp };
            

            _repository.AddVendor(v);
            
            v.Owner = Guid.NewGuid();

            //List<Vendor> lr = _repository.GetAllVendor();
            //if (lr.Count > 0) //exist boss
            //{
            //    return 0;
            //}
            _repository.Save();
            //string rand = CreatePassword(8);

            //await TestCreateUser(v.NomorVendor, rand, v.Nama, v.Owner);

            return v.Id;
        }

        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_master_user })]
        [HttpGet]
        public void DeleteVendorNonRegister(int? id)
        {
            if (id != null)
            {
                _repository.DeleteVendorNonRegister(id);
            }
        }
    }
}
