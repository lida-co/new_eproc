using Reston.Pinata.Model.JimbisModel;
using System;
using System.Collections.Generic;
using Reston.Pinata.Model.PengadaanRepository.View;
using Reston.Pinata.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reston.Eproc.Model.Ext
{
    public interface IVendorExtRepo
    {
        VendorExtViewModelJaws GetVendorDetailNew(int idVendor);
        Vendor GetVendorByUser(Guid id);
        Vendor GetVendorById(int id);
        //VendorExtViewModelJaws EditVendorExt(VendorExtViewModelJaws model, string username);
        //Guid AddVendorDocumentExt(RegDocumentExt v);
        VendorExtViewModelJaws EditVendorExt(VendorExtViewModelJaws model, string username, bool validateOwnership);

    }

    public class VendorExtRepo : IVendorExtRepo
    {
        AppDbContext ctx;
        public VendorExtRepo(AppDbContext j)
        {
            ctx = j;
            ctx.Configuration.LazyLoadingEnabled = true;
        }

        public Vendor GetVendorByUser(Guid id)
        {
            Vendor v = ctx.Vendors.Where(x => x.Owner == id).FirstOrDefault();
            if (v != null) return v;
            return null;
        }

        public Vendor GetVendorById(int id)
        {
            Vendor v = ctx.Vendors.Where(x => x.Id == id).FirstOrDefault();
            if (v != null) return v;
            return null;
        }

        public ReferenceData getNameMaster(string code)
        {
            ReferenceData rm = ctx.ReferenceDatas.Where(x => x.Qualifier.Equals("DUKCAPILPROV") && x.Code.Equals(code)).FirstOrDefault();
            if (rm != null) return rm;
            return null;
        }

        public VendorExtViewModelJaws GetVendorDetailNew(int idVendor)
        {
            //int IdRegVendor = ctx.RegVendors.Where(x => x.NoPengajuan == noPengajuan).FirstOrDefault().Id;

            VendorExt vendorExt = ctx.VendorExts.Where(x => x.VendorId == idVendor).FirstOrDefault();

            Vendor rv = ctx.Vendors.Where(x => x.Id == idVendor).FirstOrDefault();

            Guid vendorExtId = vendorExt.Id;

            VendorExtViewModelJaws vm = new VendorExtViewModelJaws
            {
                id = idVendor,
                TipeVendor = Int16.Parse(vendorExt.JenisVendor),
                FirstLevelDivisionCode = vendorExt.FirstLevelDivisionCode,
                SecondLevelDivisionCode = vendorExt.SecondLevelDivisionCode,
                ThirdLevelDivisionCode = vendorExt.ThirdLevelDivisionCode,
                Nama = rv.Nama,
                Alamat = rv.Alamat,
                Email = rv.Email,
                Website = rv.Website,
                Telepon = rv.Telepon,
            };


            VendorRegExtViewModels VendorRegExt = new VendorRegExtViewModels
            {
                KategoriUsaha = vendorExt.KategoriUsaha,
                KategoriVendor = vendorExt.KategoriVendor,
                BentukBadanUsaha = vendorExt.BentukBadanUsaha,
                StatusPerusahaan = vendorExt.StatusPerusahaan,
                EstablishedDate = vendorExt.EstablishedDate == null ? DateTime.MinValue : vendorExt.EstablishedDate.Value,
                CountryCode = ctx.ReferenceDatas.Where(x => x.Code == vendorExt.CountryCode).FirstOrDefault().LocalizedName,
                PostalCode = vendorExt.PostalCode,
                Fax = vendorExt.Fax,
                WorkUnitCode = vendorExt.WorkUnitCode,
                SegBidangUsahaCode = vendorExt.SegBidangUsahaCode,
                SegKelompokUsahaCodeSingle = vendorExt.SegKelompokUsahaCode,
                SegSubBidangUsahaCode = vendorExt.SegSubBidangUsahaCode,
                SegKualifikasiGrade = vendorExt.SegKualifikasiGrade,
                IndivName = vendorExt.IndivName,
                IndivAbbrevName = vendorExt.IndivAbbrevName,
                IndivGiidNo = vendorExt.IndivGiidNo,
                IndivGiidValidUntil = vendorExt.IndivGiidValidUntil == null ? DateTime.MinValue : vendorExt.IndivGiidValidUntil.Value,
                IndivAddress = vendorExt.IndivAddress,
                IndivCountryCode = vendorExt.IndivCountryCode,
                IndivFirstLevelDivisionCode = vendorExt.IndivFirstLevelDivisionCode,
                IndivSecondLevelDivisionCode = vendorExt.IndivSecondLevelDivisionCode,
                IndivThirdLevelDivisionCode = vendorExt.IndivThirdLevelDivisionCode,
                IndivPostalCode = vendorExt.IndivPostalCode,
                IndivContactPersonName = vendorExt.IndivContactPersonName,
                IndivContactPhoneNum = vendorExt.IndivContactPhoneNum,
                PrinRepOfficeAddress = vendorExt.PrinRepOfficeAddress,
                PrinRepOfficeContactPhoneNum = vendorExt.PrinRepOfficeContactPhoneNum,
                PrinRepOfficeFaxNum = vendorExt.PrinRepOfficeFaxNum,
                PrinRepOfficeEmail = vendorExt.PrinRepOfficeEmail,
                PrinWebsite = vendorExt.PrinWebsite,
                SubDistrict = vendorExt.SubDistrict,
                Village = vendorExt.Village,
                PrinRepPosition = vendorExt.PrinRepPosition,
                IndivGiidDocId = vendorExt.IndivGiidDocId,
                PrinRepOfficeLocalAddress = vendorExt.PrinRepOfficeLocalAddress,
                CPName = vendorExt.CPName,
                IsPKP = vendorExt.IsPKP,
            };


            //VendorBankInfoExtViewModels bankExt = new VendorBankInfoExtViewModels
            VendorBankInfoExtViewModels bankExt = (from a in ctx.VendorExtBankInfoes
                                                   where a.VendorExtId == vendorExtId
                                                   select new VendorBankInfoExtViewModels
                                                   {
                                                       BankCode = ctx.ReferenceDatas.Where(x => x.Code == a.BankCode).FirstOrDefault().LocalizedName,
                                                       BankAddress = a.BankAddress,
                                                       BankCity = a.BankCity,
                                                       Branch = a.Branch,
                                                       AccNumber = a.AccNumber,
                                                       AccName = a.AccName,
                                                       AccCurrencyCode = a.AccCurrencyCode,
                                                       BankCountry = ctx.ReferenceDatas.Where(x => x.Code == vendorExt.CountryCode).FirstOrDefault().LocalizedName,
                                                   }).Distinct().FirstOrDefault();

            List<VendorPersonExtViewModels> PersonExt = (from a in ctx.VendorExtPersons
                                                         where a.VendorExtId == vendorExtId
                                                         select new VendorPersonExtViewModels
                                                         {
                                                             Name = a.Name,
                                                             Position = a.Position,
                                                             ReligionCode = a.ReligionCode,
                                                             GiidNo = a.GiidNo,
                                                             BirthDay = a.BirthDay == null ? DateTime.MinValue : a.BirthDay.Value
                                                         }).Distinct().ToList();

            VendorDokumenExts NPWPdocs = (from a in ctx.DocumentExts
                                          join b in ctx.DocumentImageExts on a.Id equals b.DocumenExtId
                                          where a.VendorExtId == vendorExtId && a.TipeDokumen == 0
                                          select new VendorDokumenExts
                                          {
                                              Iddok = b.Id,
                                              Nomor = a.Nomor,
                                              Pembuat = a.Penerbit,
                                              TipeDokumen = a.TipeDokumen.ToString(),
                                              TanggalTerbit = a.TanggalTerbit,
                                              TanggalBerakhir = a.TanggalBerakhir,
                                              //base64 = b.Content.ToString(),
                                              FileName = b.FileName,
                                              ContentType = b.ContentType
                                          }).Distinct().FirstOrDefault();

            VendorDokumenExts PKPdocs = (from a in ctx.DocumentExts
                                         join b in ctx.DocumentImageExts on a.Id equals b.DocumenExtId
                                         where a.VendorExtId == vendorExtId && a.TipeDokumen == 1
                                         select new VendorDokumenExts
                                         {
                                             Iddok = b.Id,
                                             Nomor = a.Nomor,
                                             Pembuat = a.Penerbit,
                                             TipeDokumen = a.TipeDokumen.ToString(),
                                             TanggalTerbit = a.TanggalTerbit,
                                             TanggalBerakhir = a.TanggalBerakhir,
                                             //base64 = b.Content.ToString(),
                                             FileName = b.FileName,
                                             ContentType = b.ContentType
                                         }).Distinct().FirstOrDefault();

            VendorDokumenExts TDPdocs = (from a in ctx.DocumentExts
                                         join b in ctx.DocumentImageExts on a.Id equals b.DocumenExtId
                                         where a.VendorExtId == vendorExtId && a.TipeDokumen == 2
                                         select new VendorDokumenExts
                                         {
                                             Iddok = b.Id,
                                             Nomor = a.Nomor,
                                             Pembuat = a.Penerbit,
                                             TipeDokumen = a.TipeDokumen.ToString(),
                                             TanggalTerbit = a.TanggalTerbit,
                                             TanggalBerakhir = a.TanggalBerakhir,
                                             //base64 = b.Content.ToString(),
                                             FileName = b.FileName,
                                             ContentType = b.ContentType
                                         }).Distinct().FirstOrDefault();

            VendorDokumenExts SIUPdocs = (from a in ctx.DocumentExts
                                          join b in ctx.DocumentImageExts on a.Id equals b.DocumenExtId
                                          where a.VendorExtId == vendorExtId && a.TipeDokumen == 3
                                          select new VendorDokumenExts
                                          {
                                              Iddok = b.Id,
                                              Nomor = a.Nomor,
                                              Pembuat = a.Penerbit,
                                              TipeDokumen = a.TipeDokumen.ToString(),
                                              TanggalTerbit = a.TanggalTerbit,
                                              TanggalBerakhir = a.TanggalBerakhir,
                                              //base64 = b.Content.ToString(),
                                              FileName = b.FileName,
                                              ContentType = b.ContentType
                                          }).Distinct().FirstOrDefault();

            VendorDokumenExts SIUJKdocs = (from a in ctx.DocumentExts
                                           join b in ctx.DocumentImageExts on a.Id equals b.DocumenExtId
                                           where a.VendorExtId == vendorExtId && a.TipeDokumen == 4
                                           select new VendorDokumenExts
                                           {
                                               Iddok = b.Id,
                                               Nomor = a.Nomor,
                                               Pembuat = a.Penerbit,
                                               TipeDokumen = a.TipeDokumen.ToString(),
                                               TanggalTerbit = a.TanggalTerbit,
                                               TanggalBerakhir = a.TanggalBerakhir,
                                               //base64 = b.Content.ToString(),
                                               FileName = b.FileName,
                                               ContentType = b.ContentType
                                           }).Distinct().FirstOrDefault();

            VendorDokumenExts AKTAdocs = (from a in ctx.DocumentExts
                                          join b in ctx.DocumentImageExts on a.Id equals b.DocumenExtId
                                          where a.VendorExtId == vendorExtId && a.TipeDokumen == 5
                                          select new VendorDokumenExts
                                          {
                                              Iddok = b.Id,
                                              Nomor = a.Nomor,
                                              Pembuat = a.Penerbit,
                                              TipeDokumen = a.TipeDokumen.ToString(),
                                              TanggalTerbit = a.TanggalTerbit,
                                              TanggalBerakhir = a.TanggalBerakhir,
                                              //base64 = b.Content.ToString(),
                                              FileName = b.FileName,
                                              ContentType = b.ContentType
                                          }).Distinct().FirstOrDefault();

            VendorDokumenExts PENGADAANdocs = (from a in ctx.DocumentExts
                                               join b in ctx.DocumentImageExts on a.Id equals b.DocumenExtId
                                               where a.VendorExtId == vendorExtId && a.TipeDokumen == 6
                                               select new VendorDokumenExts
                                               {
                                                   Iddok = b.Id,
                                                   Nomor = a.Nomor,
                                                   Pembuat = a.Penerbit,
                                                   TipeDokumen = a.TipeDokumen.ToString(),
                                                   TanggalTerbit = a.TanggalTerbit,
                                                   TanggalBerakhir = a.TanggalBerakhir,
                                                   //base64 = b.Content.ToString(),
                                                   FileName = b.FileName,
                                                   ContentType = b.ContentType
                                               }).Distinct().FirstOrDefault();

            VendorDokumenExts KTPdocs = (from a in ctx.DocumentExts
                                         join b in ctx.DocumentImageExts on a.Id equals b.DocumenExtId
                                         where a.VendorExtId == vendorExtId && a.TipeDokumen == 7
                                         select new VendorDokumenExts
                                         {
                                             Iddok = b.Id,
                                             Nomor = a.Nomor,
                                             Pembuat = a.Penerbit,
                                             TipeDokumen = a.TipeDokumen.ToString(),
                                             TanggalTerbit = a.TanggalTerbit,
                                             TanggalBerakhir = a.TanggalBerakhir,
                                             //base64 = b.Content.ToString(),
                                             FileName = b.FileName,
                                             ContentType = b.ContentType
                                         }).Distinct().FirstOrDefault();

            VendorDokumenExts SERTIFIKATdocs = (from a in ctx.DocumentExts
                                                join b in ctx.DocumentImageExts on a.Id equals b.DocumenExtId
                                                where a.VendorExtId == vendorExtId && a.TipeDokumen == 8
                                                select new VendorDokumenExts
                                                {
                                                    Iddok = b.Id,
                                                    Nomor = a.Nomor,
                                                    Pembuat = a.Penerbit,
                                                    TipeDokumen = a.TipeDokumen.ToString(),
                                                    TanggalTerbit = a.TanggalTerbit,
                                                    TanggalBerakhir = a.TanggalBerakhir,
                                                    //base64 = b.Content.ToString(),
                                                    FileName = b.FileName,
                                                    ContentType = b.ContentType
                                                }).Distinct().FirstOrDefault();

            VendorDokumenExts NPWPPemilikdocs = (from a in ctx.DocumentExts
                                                 join b in ctx.DocumentImageExts on a.Id equals b.DocumenExtId
                                                 where a.VendorExtId == vendorExtId && a.TipeDokumen == 9
                                                 select new VendorDokumenExts
                                                 {
                                                     Iddok = b.Id,
                                                     Nomor = a.Nomor,
                                                     Pembuat = a.Penerbit,
                                                     TipeDokumen = a.TipeDokumen.ToString(),
                                                     TanggalTerbit = a.TanggalTerbit,
                                                     TanggalBerakhir = a.TanggalBerakhir,
                                                     //base64 = b.Content.ToString(),
                                                     FileName = b.FileName,
                                                     ContentType = b.ContentType
                                                 }).Distinct().FirstOrDefault();

            VendorDokumenExts KTPPemilikdocs = (from a in ctx.DocumentExts
                                                join b in ctx.DocumentImageExts on a.Id equals b.DocumenExtId
                                                where a.VendorExtId == vendorExtId && a.TipeDokumen == 10
                                                select new VendorDokumenExts
                                                {
                                                    Iddok = b.Id,
                                                    Nomor = a.Nomor,
                                                    Pembuat = a.Penerbit,
                                                    TipeDokumen = a.TipeDokumen.ToString(),
                                                    TanggalTerbit = a.TanggalTerbit,
                                                    TanggalBerakhir = a.TanggalBerakhir,
                                                    //base64 = b.Content.ToString(),
                                                    FileName = b.FileName,
                                                    ContentType = b.ContentType
                                                }).Distinct().FirstOrDefault();

            VendorDokumenExts DOMISILIdocs = (from a in ctx.DocumentExts
                                              join b in ctx.DocumentImageExts on a.Id equals b.DocumenExtId
                                              where a.VendorExtId == vendorExtId && a.TipeDokumen == 11
                                              select new VendorDokumenExts
                                              {
                                                  Iddok = b.Id,
                                                  Nomor = a.Nomor,
                                                  Pembuat = a.Penerbit,
                                                  TipeDokumen = a.TipeDokumen.ToString(),
                                                  TanggalTerbit = a.TanggalTerbit,
                                                  TanggalBerakhir = a.TanggalBerakhir,
                                                  //base64 = b.Content.ToString(),
                                                  FileName = b.FileName,
                                                  ContentType = b.ContentType
                                              }).Distinct().FirstOrDefault();

            VendorDokumenExts LAPORANKEUANGANdocs = (from a in ctx.DocumentExts
                                                     join b in ctx.DocumentImageExts on a.Id equals b.DocumenExtId
                                                     where a.VendorExtId == vendorExtId && a.TipeDokumen == 12
                                                     select new VendorDokumenExts
                                                     {
                                                         Iddok = b.Id,
                                                         Nomor = a.Nomor,
                                                         Pembuat = a.Penerbit,
                                                         TipeDokumen = a.TipeDokumen.ToString(),
                                                         TanggalTerbit = a.TanggalTerbit,
                                                         TanggalBerakhir = a.TanggalBerakhir,
                                                         //base64 = b.Content.ToString(),
                                                         FileName = b.FileName,
                                                         ContentType = b.ContentType
                                                     }).Distinct().FirstOrDefault();

            VendorDokumenExts REKENINGKORANdocs = (from a in ctx.DocumentExts
                                                   join b in ctx.DocumentImageExts on a.Id equals b.DocumenExtId
                                                   where a.VendorExtId == vendorExtId && a.TipeDokumen == 13
                                                   select new VendorDokumenExts
                                                   {
                                                       Iddok = b.Id,
                                                       Nomor = a.Nomor,
                                                       Pembuat = a.Penerbit,
                                                       TipeDokumen = a.TipeDokumen.ToString(),
                                                       TanggalTerbit = a.TanggalTerbit,
                                                       TanggalBerakhir = a.TanggalBerakhir,
                                                       //base64 = b.Content.ToString(),
                                                       FileName = b.FileName,
                                                       ContentType = b.ContentType
                                                   }).Distinct().FirstOrDefault();

            VendorDokumenExts DRTdocs = (from a in ctx.DocumentExts
                                         join b in ctx.DocumentImageExts on a.Id equals b.DocumenExtId
                                         where a.VendorExtId == vendorExtId && a.TipeDokumen == 14
                                         select new VendorDokumenExts
                                         {
                                             Iddok = b.Id,
                                             Nomor = a.Nomor,
                                             Pembuat = a.Penerbit,
                                             TipeDokumen = a.TipeDokumen.ToString(),
                                             TanggalTerbit = a.TanggalTerbit,
                                             TanggalBerakhir = a.TanggalBerakhir,
                                             //base64 = b.Content.ToString(),
                                             FileName = b.FileName,
                                             ContentType = b.ContentType
                                         }).Distinct().FirstOrDefault();

            VendorDokumenExts AKTAPENDIRIANdocs = (from a in ctx.DocumentExts
                                                   join b in ctx.DocumentImageExts on a.Id equals b.DocumenExtId
                                                   where a.VendorExtId == vendorExtId && a.TipeDokumen == 15
                                                   select new VendorDokumenExts
                                                   {
                                                       Iddok = b.Id,
                                                       Nomor = a.Nomor,
                                                       Pembuat = a.Penerbit,
                                                       TipeDokumen = a.TipeDokumen.ToString(),
                                                       TanggalTerbit = a.TanggalTerbit,
                                                       TanggalBerakhir = a.TanggalBerakhir,
                                                       //base64 = b.Content.ToString(),
                                                       FileName = b.FileName,
                                                       ContentType = b.ContentType
                                                   }).Distinct().FirstOrDefault();

            VendorDokumenExts SKKEMENKUMHAMdocs = (from a in ctx.DocumentExts
                                                   join b in ctx.DocumentImageExts on a.Id equals b.DocumenExtId
                                                   where a.VendorExtId == vendorExtId && a.TipeDokumen == 16
                                                   select new VendorDokumenExts
                                                   {
                                                       Iddok = b.Id,
                                                       Nomor = a.Nomor,
                                                       Pembuat = a.Penerbit,
                                                       TipeDokumen = a.TipeDokumen.ToString(),
                                                       TanggalTerbit = a.TanggalTerbit,
                                                       TanggalBerakhir = a.TanggalBerakhir,
                                                       //base64 = b.Content.ToString(),
                                                       FileName = b.FileName,
                                                       ContentType = b.ContentType
                                                   }).Distinct().FirstOrDefault();

            VendorDokumenExts BERITANEGARAdocs = (from a in ctx.DocumentExts
                                                  join b in ctx.DocumentImageExts on a.Id equals b.DocumenExtId
                                                  where a.VendorExtId == vendorExtId && a.TipeDokumen == 17
                                                  select new VendorDokumenExts
                                                  {
                                                      Iddok = b.Id,
                                                      Nomor = a.Nomor,
                                                      Pembuat = a.Penerbit,
                                                      TipeDokumen = a.TipeDokumen.ToString(),
                                                      TanggalTerbit = a.TanggalTerbit,
                                                      TanggalBerakhir = a.TanggalBerakhir,
                                                      //base64 = b.Content.ToString(),
                                                      FileName = b.FileName,
                                                      ContentType = b.ContentType
                                                  }).Distinct().FirstOrDefault();

            VendorDokumenExts AKTAPERUBAHANdocs = (from a in ctx.DocumentExts
                                                   join b in ctx.DocumentImageExts on a.Id equals b.DocumenExtId
                                                   where a.VendorExtId == vendorExtId && a.TipeDokumen == 18
                                                   select new VendorDokumenExts
                                                   {
                                                       Iddok = b.Id,
                                                       Nomor = a.Nomor,
                                                       Pembuat = a.Penerbit,
                                                       TipeDokumen = a.TipeDokumen.ToString(),
                                                       TanggalTerbit = a.TanggalTerbit,
                                                       TanggalBerakhir = a.TanggalBerakhir,
                                                       //base64 = b.Content.ToString(),
                                                       FileName = b.FileName,
                                                       ContentType = b.ContentType
                                                   }).Distinct().FirstOrDefault();


            vm.VendorRegExt = VendorRegExt;
            vm.VendorBankInfoExt = bankExt;
            vm.VendorPersonExt = PersonExt;
            vm.NPWP = NPWPdocs;
            vm.PKP = PKPdocs;
            vm.TDP = TDPdocs;
            vm.SIUP = SIUPdocs;
            vm.SIUJK = SIUJKdocs;
            vm.AKTA = AKTAdocs;
            vm.PENGADAAN = PENGADAANdocs;
            vm.KTP = KTPdocs;
            vm.SERTIFIKAT = SERTIFIKATdocs;
            vm.NPWPPemilik = NPWPPemilikdocs;
            vm.KTPPemilik = KTPPemilikdocs;
            vm.DOMISILI = DOMISILIdocs;
            vm.LAPORANKEUANGAN = LAPORANKEUANGANdocs;
            vm.REKENINGKORAN = REKENINGKORANdocs;
            vm.DRT = DRTdocs;
            vm.AKTAPENDIRIAN = AKTAPENDIRIANdocs;
            vm.SKKEMENKUMHAM = SKKEMENKUMHAMdocs;
            vm.BERITANEGARA = BERITANEGARAdocs;
            vm.AKTAPERUBAHAN = AKTAPERUBAHANdocs;

            var zzzz = 0;
            //RegVendorExt vendorExtss = ctx.RegVendorExts.Where(x => x.RegVendorId == IdRegVendor).FirstOrDefault();

            return vm;
        }

        public int RegisterVerifiedVendorEXT(string v)
        {
            try
            {
                int IdRegVendor = ctx.RegVendors.Where(x => x.NoPengajuan == v).FirstOrDefault().Id;
                var vendor = ctx.Vendors.Where(x => x.NomorVendor == v).FirstOrDefault();

                var regVendorExt = ctx.RegVendorExts.Where(x => x.RegVendorId == IdRegVendor).FirstOrDefault();
                VendorExt vendorExt = new VendorExt()
                {
                    Id = regVendorExt.Id,
                    VendorId = vendor.Id,
                    JenisVendor = regVendorExt.JenisVendor,
                    KategoriUsaha = regVendorExt.KategoriUsaha,
                    KategoriVendor = regVendorExt.KategoriVendor,
                    BentukBadanUsaha = regVendorExt.BentukBadanUsaha,
                    StatusPerusahaan = regVendorExt.StatusPerusahaan,
                    EstablishedDate = regVendorExt.EstablishedDate,
                    CountryCode = regVendorExt.CountryCode,
                    FirstLevelDivisionCode = regVendorExt.FirstLevelDivisionCode,
                    SecondLevelDivisionCode = regVendorExt.SecondLevelDivisionCode,
                    ThirdLevelDivisionCode = regVendorExt.ThirdLevelDivisionCode,
                    PostalCode = regVendorExt.PostalCode,
                    Fax = regVendorExt.Fax,
                    WorkUnitCode = regVendorExt.WorkUnitCode,
                    SegBidangUsahaCode = regVendorExt.SegBidangUsahaCode,
                    SegKelompokUsahaCode = regVendorExt.SegKelompokUsahaCode,
                    SegSubBidangUsahaCode = regVendorExt.SegSubBidangUsahaCode,
                    SegKualifikasiGrade = regVendorExt.SegKualifikasiGrade,
                    IndivName = regVendorExt.IndivName,
                    IndivAbbrevName = regVendorExt.IndivAbbrevName,
                    IndivGiidNo = regVendorExt.IndivGiidNo,
                    IndivGiidValidUntil = regVendorExt.IndivGiidValidUntil,
                    IndivAddress = regVendorExt.IndivAddress,
                    IndivCountryCode = regVendorExt.IndivCountryCode,
                    IndivFirstLevelDivisionCode = regVendorExt.IndivFirstLevelDivisionCode,
                    IndivSecondLevelDivisionCode = regVendorExt.IndivSecondLevelDivisionCode,
                    IndivThirdLevelDivisionCode = regVendorExt.IndivThirdLevelDivisionCode,
                    IndivPostalCode = regVendorExt.IndivPostalCode,
                    IndivContactPersonName = regVendorExt.IndivContactPersonName,
                    IndivContactPhoneNum = regVendorExt.IndivContactPhoneNum,
                    PrinRepOfficeAddress = regVendorExt.PrinRepOfficeAddress,
                    PrinRepOfficeContactPhoneNum = regVendorExt.PrinRepOfficeContactPhoneNum,
                    PrinRepOfficeFaxNum = regVendorExt.PrinRepOfficeFaxNum,
                    PrinRepOfficeEmail = regVendorExt.PrinRepOfficeEmail,
                    PrinWebsite = regVendorExt.PrinWebsite,
                    SubDistrict = regVendorExt.SubDistrict,
                    Village = regVendorExt.Village,
                    PrinRepPosition = regVendorExt.PrinRepPosition,
                    IndivGiidDocId = regVendorExt.IndivGiidDocId,
                    PrinRepOfficeLocalAddress = regVendorExt.PrinRepOfficeLocalAddress,
                    CPName = regVendorExt.CPName,
                    IsPKP = regVendorExt.IsPKP
                };
                ctx.VendorExts.Add(vendorExt);
                ctx.SaveChanges();

                var regVendorExtBankInfo = ctx.RegVendorExtBankInfoes.Where(x => x.RegVendorExtId == regVendorExt.Id).FirstOrDefault();
                VendorExtBankInfo vendorExtBankInfo = new VendorExtBankInfo()
                {
                    Id = regVendorExtBankInfo.Id,
                    VendorExtId = vendorExt.Id,
                    BankCode = regVendorExtBankInfo.BankCode,
                    BankAddress = regVendorExtBankInfo.BankAddress,
                    BankCity = regVendorExtBankInfo.BankCity,
                    Branch = regVendorExtBankInfo.Branch,
                    AccNumber = regVendorExtBankInfo.AccNumber,
                    AccName = regVendorExtBankInfo.AccName,
                    AccCurrencyCode = regVendorExtBankInfo.AccCurrencyCode,
                    BankCountry = regVendorExtBankInfo.BankCountry
                };
                ctx.VendorExtBankInfoes.Add(vendorExtBankInfo);
                ctx.SaveChanges();

                var regVendorExtPerson = ctx.RegVendorExtPersons.Where(x => x.RegVendorExtId == regVendorExt.Id).Distinct().ToList();
                if (regVendorExtPerson.Count > 0)
                {
                    foreach (RegVendorExtPerson rvep in regVendorExtPerson)
                    {
                        VendorExtPerson vep = new VendorExtPerson()
                        {
                            Id = rvep.Id,
                            VendorExtId = regVendorExt.Id,
                            Name = rvep.Name,
                            Position = rvep.Position,
                            ContactPhone = rvep.ContactPhone,
                            ContactEmail = rvep.ContactEmail,
                            ContactAddress = rvep.ContactAddress,
                            ReligionCode = rvep.ReligionCode,
                            GiidNo = rvep.GiidNo,
                            BirthDay = rvep.BirthDay,
                        };
                        ctx.VendorExtPersons.Add(vep);
                        ctx.SaveChanges();
                    }
                }

                var regVendorExtHumanResource = ctx.RegVendorExtHumanResources.Where(x => x.RegVendorExtId == regVendorExt.Id).Distinct().ToList();
                if (regVendorExtHumanResource.Count() > 0)
                {
                    foreach (RegVendorExtHumanResource rvehr in regVendorExtHumanResource)
                    {
                        VendorExtHumanResource vehr = new VendorExtHumanResource()
                        {
                            Id = rvehr.Id,
                            VendorExtId = regVendorExt.Id,
                            ResourceFullName = rvehr.ResourceFullName,
                            ResourceDateOfBirth = rvehr.ResourceDateOfBirth,
                            ResourceExperienceCode = rvehr.ResourceExperienceCode,
                            ResourceExpertise = rvehr.ResourceExpertise,
                            ResourceCVDocId = rvehr.ResourceCVDocId,
                            ResourceLastEduCode = rvehr.ResourceLastEduCode,
                            ResourceLastEduDocId = rvehr.ResourceLastEduDocId,
                            ResourceLastEduIssuer = rvehr.ResourceLastEduIssuer,
                            ResourceCertificationDocId = rvehr.ResourceCertificationDocId,
                            ResourceCertificationIssuer = rvehr.ResourceCertificationIssuer,
                        };
                        ctx.VendorExtHumanResources.Add(vehr);
                        ctx.SaveChanges();
                    }
                }

                var regVendorExtFinStatement = ctx.RegVendorExtFinStatements.Where(x => x.RegVendorExtId == regVendorExt.Id).Distinct().ToList();
                if (regVendorExtFinStatement.Count > 0)
                {
                    foreach (RegVendorExtFinStatement rvefs in regVendorExtFinStatement)
                    {
                        VendorExtFinStatement vefs = new VendorExtFinStatement()
                        {
                            Id = rvefs.Id,
                            VendorExtId = regVendorExt.Id,
                            FinStmtDocNumber = rvefs.FinStmtDocNumber,
                            FinStmtIssuer = rvefs.FinStmtIssuer,
                            FinStmtIssueDate = rvefs.FinStmtIssueDate,
                            FinStmtValidThruDate = rvefs.FinStmtValidThruDate,
                            FinStmtDocumentId = rvefs.FinStmtDocumentId,
                            FinStmtYear = rvefs.FinStmtYear,
                            FinStmtCurrencyCode = rvefs.FinStmtCurrencyCode,
                            FinStmtAktivaLancar = rvefs.FinStmtAktivaLancar,
                            FinStmtHutangLancar = rvefs.FinStmtHutangLancar,
                            FinStmtRasioLikuiditas = rvefs.FinStmtRasioLikuiditas,
                            FinStmtTotalHutang = rvefs.FinStmtTotalHutang,
                            FinStmtEkuitas = rvefs.FinStmtEkuitas,
                            FinStmtDebtToEquityRation = rvefs.FinStmtDebtToEquityRation,
                            FinStmtNetProfitLoss = rvefs.FinStmtNetProfitLoss,
                            FinStmtReturnOfEquity = rvefs.FinStmtReturnOfEquity,
                            FinStmtKas = rvefs.FinStmtKas,
                            FinStmtTotalAktiva = rvefs.FinStmtTotalAktiva,
                            FinStmtAuditStatusCode = rvefs.FinStmtAuditStatusCode,
                            FinStmtDocId = rvefs.FinStmtDocId,
                        };
                        ctx.VendorExtFinStatements.Add(vefs);
                        ctx.SaveChanges();
                    }
                }

                var regVendorExtEquipment = ctx.RegVendorExtEquipments.Where(x => x.RegVendorExtId == regVendorExt.Id).Distinct().ToList();
                if (regVendorExtEquipment.Count > 0)
                {
                    foreach (RegVendorExtEquipment rvee in regVendorExtEquipment)
                    {
                        VendorExtEquipment vee = new VendorExtEquipment()
                        {
                            Id = rvee.Id,
                            VendorExtId = regVendorExt.Id,
                            EquipmentName = rvee.EquipmentName,
                            EquipmentQty = rvee.EquipmentQty,
                            EquipmentCapacity = rvee.EquipmentCapacity,
                            EquipmentMake = rvee.EquipmentMake,
                            EquipmentMakeYear = rvee.EquipmentMakeYear,
                            EquipmentConditionCode = rvee.EquipmentConditionCode,
                            EquipmentLocation = rvee.EquipmentLocation,
                            EquipmentOwnershipDocId = rvee.EquipmentOwnershipDocId,
                            EquipmentPicture = rvee.EquipmentPicture,
                        };
                        ctx.VendorExtEquipments.Add(vee);
                        ctx.SaveChanges();
                    }
                }

                var regVendorExtJobHistory = ctx.RegVendorExtJobHistories.Where(x => x.RegVendorExtId == regVendorExt.Id).Distinct().ToList();
                if (regVendorExtJobHistory.Count > 0)
                {
                    foreach (RegVendorExtJobHistory rvejh in regVendorExtJobHistory)
                    {
                        VendorExtJobHistory vejh = new VendorExtJobHistory()
                        {
                            Id = rvejh.Id,
                            VendorExtId = regVendorExt.Id,
                            JobTitle = rvejh.JobTitle,
                            JobClient = rvejh.JobClient,
                            JobLocation = rvejh.JobLocation,
                            JobStartDate = rvejh.JobStartDate,
                            JobContractNum = rvejh.JobContractNum,
                            JobContractDate = rvejh.JobContractDate,
                            JobContractAmount = rvejh.JobContractAmount,
                            JobContractAmountCurrencyCode = rvejh.JobContractAmountCurrencyCode,
                            JobContractDocId = rvejh.JobContractDocId,
                            JobType = rvejh.JobType,
                        };
                        ctx.VendorExtJobHistories.Add(vejh);
                        ctx.SaveChanges();
                    }
                }

                var regVendorDocumentExt = ctx.RegVendorDocumentExts.Where(x => x.RegVendorExtId == regVendorExt.Id).Distinct().ToList();
                if (regVendorDocumentExt.Count > 0)
                {
                    foreach (RegVendorDocumentExt rvde in regVendorDocumentExt)
                    {
                        VendorDocumentExt vde = new VendorDocumentExt()
                        {
                            Id = rvde.Id,
                            Nomor = rvde.Nomor,
                            Pembuat = rvde.Pembuat,
                            TanggalTerbit = rvde.TanggalTerbit,
                            TanggalBerakhir = rvde.TanggalBerakhir,
                            TipeDokumen = rvde.TipeDokumen,
                            Content = rvde.Content,
                            VendorExtId = regVendorExt.Id,
                        };
                        ctx.VendorDocumentExts.Add(vde);
                        ctx.SaveChanges();
                    }
                }

                var regDocumentExt = ctx.RegDocumentExts.Where(x => x.RegVendorExtId == regVendorExt.Id).Distinct().ToList();
                if (regDocumentExt.Count > 0)
                {
                    foreach (RegDocumentExt rvp in regDocumentExt)
                    {
                        var regDocumentImageExt = ctx.RegDocumentImageExts.Where(x => x.RegDocumenExtId == rvp.Id).FirstOrDefault();
                        DocumentExt DE = new DocumentExt()
                        {
                            Id = rvp.Id,
                            Nomor = rvp.Nomor,
                            Penerbit = rvp.Penerbit,
                            TipeDokumen = rvp.TipeDokumen,
                            TanggalTerbit = rvp.TanggalTerbit,
                            TanggalBerakhir = rvp.TanggalBerakhir,
                            Active = rvp.Active,
                            VendorExtId = regVendorExt.Id,
                        };
                        ctx.DocumentExts.Add(DE);
                        ctx.SaveChanges();

                        DocumentImageExt DIE = new DocumentImageExt()
                        {
                            Id = regDocumentImageExt.Id,
                            Content = regDocumentImageExt.Content,
                            DocumenExtId = regDocumentImageExt.RegDocumenExtId,
                            FileName = regDocumentImageExt.FileName,
                            ContentType = regDocumentImageExt.ContentType,
                        };
                        ctx.DocumentImageExts.Add(DIE);
                        ctx.SaveChanges();
                    }
                }

            }
            catch (Exception e)
            {

            }
            return 0;
        }

        //public VendorExtViewModelJaws EditVendorExt(VendorExtViewModelJaws model, string username)
        //{
        //    return EditVendorExt(model, username, true);
        //}

        public VendorExtViewModelJaws EditVendorExt(VendorExtViewModelJaws model, string username, bool validateOwnership)
        {
            Vendor v = GetVendorById(model.id);

            // If validateOwnership is set, we have to enforce Data Ownership Policy 
            if (validateOwnership && v.NomorVendor != username) {
                throw new DataOwnershipException("This user does not own the data that is being edited.");
            }

            Vendor vOld = v;
            VendorExt vExtOld = new VendorExt();
            VendorExtBankInfo biExtOld = new VendorExtBankInfo();
            DocumentExt npwpDocxOld = new DocumentExt();
            DocumentExt pkpDocxOld = new DocumentExt();
            DocumentExt tdpDocxOld = new DocumentExt();
            DocumentExt siupDocxOld = new DocumentExt();
            DocumentExt siujkDocxOld = new DocumentExt();
            DocumentExt aktaDocxOld = new DocumentExt();
            //DocumentExt pengadaanDocxOld = new DocumentExt();
            DocumentExt ktpDocxOld = new DocumentExt();
            DocumentExt sertifDocxOld = new DocumentExt();
            DocumentExt npwppemilikDocxOld = new DocumentExt();
            DocumentExt ktppemilikDocxOld = new DocumentExt();
            DocumentExt domisiliDocxOld = new DocumentExt();
            DocumentExt lapuangDocxOld = new DocumentExt();
            DocumentExt rekkoranDocxOld = new DocumentExt();
            DocumentExt drtDocxOld = new DocumentExt();
            DocumentExt aktapendiriDocxOld = new DocumentExt();
            DocumentExt skkemenkumhamDocxOld = new DocumentExt();
            DocumentExt beritanegaraDocxOld = new DocumentExt();
            DocumentExt aktaperubahnDocxOld = new DocumentExt();
            DocumentExt profilperusahaanDocxOld = new DocumentExt();
            DocumentExt nibDocxOld = new DocumentExt();
            DocumentExt doksertifcvDocxOld = new DocumentExt();
            DocumentExt buktikepemilikanperalatanDocxOld = new DocumentExt();
            DocumentExt fotoperalatanDocxOld = new DocumentExt();
            DocumentExt buktikerjasamaDocxOld = new DocumentExt();
            DocumentExt laporankeuanganDocxOld = new DocumentExt();

            //edit
            if (model != null)
            {
                if (model.Provinsi != null)
                {
                    v.Provinsi = model.Provinsi;
                }
                //else
                //{
                //    v.Provinsi = "";
                //}

                v.Nama = model.Nama;
                v.Alamat = model.Alamat;
                v.Email = model.Email;
                v.Website = model.Website;
                v.Telepon = model.Telepon;

                #region info
                var vExt = ctx.VendorExts.Where(x => x.VendorId == v.Id ).FirstOrDefault();
                if (vExt != null)
                {
                    vExtOld = vExt;
                    vExt.KategoriUsaha = model.VendorRegExt.KategoriUsaha;
                    vExt.KategoriVendor = model.VendorRegExt.KategoriVendor;
                    vExt.BentukBadanUsaha = model.VendorRegExt.BentukBadanUsaha;
                    vExt.StatusPerusahaan = model.VendorRegExt.StatusPerusahaan;
                    vExt.EstablishedDate = model.VendorRegExt.EstablishedDate == null ? DateTime.MinValue : model.VendorRegExt.EstablishedDate;
                    vExt.CountryCode = model.VendorRegExt.CountryCode;
                    vExt.PostalCode = model.VendorRegExt.PostalCode;
                    vExt.Fax = model.VendorRegExt.Fax;
                    vExt.WorkUnitCode = model.VendorRegExt.WorkUnitCode;

                    vExt.FirstLevelDivisionCode = model.FirstLevelDivisionCode;
                    vExt.SecondLevelDivisionCode = model.SecondLevelDivisionCode;
                    vExt.ThirdLevelDivisionCode = model.ThirdLevelDivisionCode;

                    vExt.SegBidangUsahaCode = model.VendorRegExt.SegBidangUsahaCode;
                    string ndatastring = string.Empty;
                    foreach (var jdata in model.VendorRegExt.SegKelompokUsahaCode)
                    {
                        ndatastring = ndatastring + jdata.ToString() + ',';
                    }
                    vExt.SegKelompokUsahaCode = ndatastring;
                    //vExt.SegKelompokUsahaCode = model.VendorRegExt.SegKelompokUsahaCodeList;

                    vExt.SegSubBidangUsahaCode = model.VendorRegExt.SegSubBidangUsahaCode;
                    vExt.SegKualifikasiGrade = model.VendorRegExt.SegKualifikasiGrade;

                    vExt.IndivName = model.VendorRegExt.IndivName;
                    vExt.IndivAbbrevName = model.VendorRegExt.IndivAbbrevName;
                    vExt.IndivGiidNo = model.VendorRegExt.IndivGiidNo;
                    vExt.IndivGiidValidUntil = model.VendorRegExt.IndivGiidValidUntil == null ? DateTime.MinValue : model.VendorRegExt.IndivGiidValidUntil;
                    vExt.IndivAddress = model.VendorRegExt.IndivAddress;
                    vExt.IndivCountryCode = model.VendorRegExt.IndivCountryCode;
                    vExt.IndivFirstLevelDivisionCode = model.VendorRegExt.IndivFirstLevelDivisionCode;
                    vExt.IndivSecondLevelDivisionCode = model.VendorRegExt.IndivSecondLevelDivisionCode;
                    vExt.IndivThirdLevelDivisionCode = model.VendorRegExt.IndivThirdLevelDivisionCode;
                    vExt.IndivPostalCode = model.VendorRegExt.IndivPostalCode;
                    vExt.IndivContactPersonName = model.VendorRegExt.IndivContactPersonName;
                    vExt.IndivContactPhoneNum = model.VendorRegExt.IndivContactPhoneNum;
                    vExt.PrinRepOfficeAddress = model.VendorRegExt.PrinRepOfficeAddress;
                    vExt.PrinRepOfficeContactPhoneNum = model.VendorRegExt.PrinRepOfficeContactPhoneNum;
                    vExt.PrinRepOfficeFaxNum = model.VendorRegExt.PrinRepOfficeFaxNum;
                    vExt.PrinRepOfficeEmail = model.VendorRegExt.PrinRepOfficeEmail;
                    vExt.PrinWebsite = model.VendorRegExt.PrinWebsite;
                    vExt.SubDistrict = model.VendorRegExt.SubDistrict;
                    vExt.Village = model.VendorRegExt.Village;
                    vExt.PrinRepPosition = model.VendorRegExt.PrinRepPosition;
                    vExt.IndivGiidDocId = model.VendorRegExt.IndivGiidDocId;
                    vExt.PrinRepOfficeLocalAddress = model.VendorRegExt.PrinRepOfficeLocalAddress;
                    vExt.CPName = model.VendorRegExt.CPName;
                    //if ( !model.PKP.Nomor.Equals("") && model.PKP.Nomor != null)
                    //{
                    //    vExt.IsPKP = model.VendorRegExt.IsPKP;
                    //}
                    vExt.IsPKP = model.VendorRegExt.IsPKP;

                    vExt.DirPersonGiidNo = model.VendorRegExt.DirPersonGiidNo;
                    vExt.DirPersonName = model.VendorRegExt.DirPersonName;
                    vExt.DirPersonPosition = model.VendorRegExt.DirPersonPosition;
                    vExt.DirPersonReligionCode = model.VendorRegExt.DirPersonReligionCode;
                    vExt.DirPersonBirthDay = model.VendorRegExt.DirPersonBirthDay;

                    var biExt = ctx.VendorExtBankInfoes.Where(x => x.VendorExtId == vExt.Id).FirstOrDefault();
                    if (biExt != null)
                    {
                        biExtOld = biExt;
                        biExt.BankCode = model.VendorBankInfoExt.BankCode;
                        biExt.BankAddress = model.VendorBankInfoExt.BankAddress;
                        biExt.BankCity = model.VendorBankInfoExt.BankCity;
                        biExt.Branch = model.VendorBankInfoExt.Branch;
                        biExt.AccNumber = model.VendorBankInfoExt.AccNumber;
                        biExt.AccName = model.VendorBankInfoExt.AccName;
                        biExt.AccCurrencyCode = model.VendorBankInfoExt.AccCurrencyCode;
                        biExt.BankCountry = model.VendorBankInfoExt.BankCountry;
                    }
                    else
                    {
                        VendorExtBankInfo bi = new VendorExtBankInfo()
                        {
                            Id = Guid.NewGuid(),
                            BankCode = model.VendorBankInfoExt.BankCode,
                            BankAddress = model.VendorBankInfoExt.BankAddress,
                            BankCity = model.VendorBankInfoExt.BankCity,
                            Branch = model.VendorBankInfoExt.Branch,
                            AccNumber = model.VendorBankInfoExt.AccNumber,
                            AccName = model.VendorBankInfoExt.AccName,
                            AccCurrencyCode = model.VendorBankInfoExt.AccCurrencyCode,
                            VendorExtId = vExt.Id,
                            BankCountry = model.VendorBankInfoExt.BankCountry
                        };
                        AddVendorExtBankInfo(bi);
                    }

                    var perExt = ctx.VendorExtPersons.Where(x => x.VendorExtId == vExt.Id).ToList();
                    if (perExt.Count > 0 && model.VendorPersonExt.Count > 0)
                    {
                        for (int i = 0; i < perExt.Count; i++)
                        {
                            if (model.VendorPersonExt[i] != null)
                            {
                                perExt[i].Name = model.VendorPersonExt[i].Name;
                                perExt[i].Position = model.VendorPersonExt[i].Position;
                                perExt[i].ReligionCode = model.VendorPersonExt[i].ReligionCode;
                                perExt[i].GiidNo = model.VendorPersonExt[i].GiidNo;
                                perExt[i].BirthDay = model.VendorPersonExt[i].BirthDay == null ? DateTime.MinValue : model.VendorPersonExt[i].BirthDay.Value;
                                perExt[i].ContactAddress = model.VendorPersonExt[i].ContactAddress;
                                perExt[i].ContactEmail = model.VendorPersonExt[i].ContactEmail;
                                perExt[i].ContactPhone = model.VendorPersonExt[i].ContactPhone;
                            }
                        }
                    }
                    else
                    {
                        List<VendorExtPerson> lvp = new List<VendorExtPerson>();
                        foreach (var item in model.VendorPersonExt)
                        {
                            if (item.Name != null && item.Name != "")
                            {
                                VendorExtPerson n = new VendorExtPerson()
                                {
                                    Id = Guid.NewGuid(),
                                    Name = item.Name,
                                    Position = item.Position,
                                    ContactPhone = item.ContactPhone,
                                    ContactEmail = item.ContactEmail,
                                    ContactAddress = item.ContactAddress,
                                    ReligionCode = item.ReligionCode,
                                    GiidNo = item.GiidNo,
                                    VendorExtId = vExt.Id
                                };
                                if (item.BirthDay != null)
                                    n.BirthDay = item.BirthDay;
                                lvp.Add(n);
                            }
                        }
                        if (lvp.Count != 0) AddVendorExtPerson(lvp);
                    }

                    var hrExt = ctx.VendorExtHumanResources.Where(x => x.VendorExtId == vExt.Id).ToList();
                    if (hrExt.Count > 0)
                    {
                        for (int i = 0; i < hrExt.Count; i++)
                        {
                            if (model.VendorHumanResourceExt[i] != null)
                            {
                                hrExt[i].ResourceFullName = model.VendorHumanResourceExt[i].ResourceFullName;
                                hrExt[i].ResourceDateOfBirth = model.VendorHumanResourceExt[i].ResourceDateOfBirth;
                                hrExt[i].ResourceLastEduCode = model.VendorHumanResourceExt[i].ResourceLastEduCode;
                                hrExt[i].ResourceExperienceCode = model.VendorHumanResourceExt[i].ResourceExperienceCode;
                                hrExt[i].ResourceExpertise = model.VendorHumanResourceExt[i].ResourceExpertise;
                            }
                        }
                    }
                    else
                    {
                        List<VendorExtHumanResource> listhrExt = new List<VendorExtHumanResource>();
                        foreach (var item in model.VendorHumanResourceExt)
                        {
                            if (item.ResourceFullName != null && item.ResourceFullName != "")
                            {
                                VendorExtHumanResource hrExtAdd = new VendorExtHumanResource()
                                {
                                    Id = Guid.NewGuid(),
                                    ResourceFullName = item.ResourceFullName,
                                    ResourceDateOfBirth = item.ResourceDateOfBirth,
                                    ResourceExperienceCode = item.ResourceExperienceCode,
                                    ResourceExpertise = item.ResourceExpertise,
                                    ResourceCVDocId = item.ResourceCVDocId,
                                    ResourceLastEduCode = item.ResourceLastEduCode,
                                    ResourceLastEduDocId = item.ResourceLastEduDocId,
                                    ResourceLastEduIssuer = item.ResourceLastEduIssuer,
                                    ResourceCertificationDocId = item.ResourceCertificationDocId,
                                    ResourceCertificationIssuer = item.ResourceCertificationIssuer,
                                    ResourceExperienceYears = item.ResourceExperienceYears,
                                    VendorExtId = vExt.Id

                                };

                                if (model.DokumenSertifikatCV != null)
                                {
                                    var guiddocext = Guid.NewGuid();
                                    DocumentExt detaildoc = new DocumentExt()
                                    {
                                        Id = guiddocext,
                                        Nomor = model.DokumenSertifikatCV.Nomor,
                                        Penerbit = model.DokumenSertifikatCV.Pembuat,
                                        TanggalTerbit = model.DokumenSertifikatCV.TanggalTerbit,
                                        TanggalBerakhir = model.DokumenSertifikatCV.TanggalBerakhir,
                                        VendorExtId = vExt.Id,
                                        TipeDokumen = (int)EDocumentType.DokumenSertifikatCV
                                    };
                                    AddVendorDocumentExt(detaildoc);

                                    DocumentImageExt regVendorDocumentExtSertifikasiTenagaAhli = new DocumentImageExt()
                                    {
                                        Id = Guid.NewGuid(),
                                        Content = Convert.FromBase64String(model.DokumenSertifikatCV.base64),
                                        FileName = model.DokumenSertifikatCV.FileName,
                                        ContentType = model.DokumenSertifikatCV.ContentType,
                                        DocumenExtId = guiddocext
                                    };
                                    AddVendorDocumentImageExt(regVendorDocumentExtSertifikasiTenagaAhli);

                                    hrExtAdd.ResourceCertificationDocId = detaildoc.Id;
                                }

                                listhrExt.Add(hrExtAdd);
                            }
                        }
                        if (listhrExt.Count != 0) AddVendorExtHumanResource(listhrExt);
                    }

                    var vequipExt = ctx.VendorExtEquipments.Where(x => x.VendorExtId == vExt.Id).ToList();
                    if (vequipExt.Count > 0)
                    {
                        for (int i = 0; i < vequipExt.Count; i++)
                        {
                            if (model.VendorEquipmentExt[i] != null)
                            {
                                vequipExt[i].EquipmentName = model.VendorEquipmentExt[i].EquipmentName;
                                vequipExt[i].EquipmentQty = model.VendorEquipmentExt[i].EquipmentQty;
                                vequipExt[i].EquipmentCapacity = model.VendorEquipmentExt[i].EquipmentCapacity;
                                vequipExt[i].EquipmentMake = model.VendorEquipmentExt[i].EquipmentMake;
                                vequipExt[i].EquipmentMakeYear = model.VendorEquipmentExt[i].EquipmentMakeYear;
                                vequipExt[i].EquipmentConditionCode = model.VendorEquipmentExt[i].EquipmentConditionCode;
                                vequipExt[i].EquipmentLocation = model.VendorEquipmentExt[i].EquipmentLocation;
                            }
                        }
                    }
                    else
                    {
                        List<VendorExtEquipment> listEquipment = new List<VendorExtEquipment>();
                        foreach (var item in model.VendorEquipmentExt)
                        {
                            if (item.EquipmentName != null && item.EquipmentName != "")
                            {
                                VendorExtEquipment equipmentAdd = new VendorExtEquipment()
                                {
                                    Id = Guid.NewGuid(),
                                    EquipmentName = item.EquipmentName,
                                    EquipmentQty = item.EquipmentQty,
                                    EquipmentCapacity = item.EquipmentCapacity,
                                    EquipmentMake = item.EquipmentMake,
                                    EquipmentMakeYear = item.EquipmentMakeYear,
                                    EquipmentConditionCode = item.EquipmentConditionCode,
                                    EquipmentLocation = item.EquipmentLocation,
                                    //EquipmentOwnershipDocId = item.EquipmentOwnershipDocId,
                                    //EquipmentPicture = item.EquipmentPictureDocId,
                                    VendorExtId = vExt.Id
                                };

                                if (model.BuktiKepemilikanPeralatan.base64 != null)
                                {
                                    var guiddocext = Guid.NewGuid();
                                    DocumentExt detaildoc = new DocumentExt()
                                    {
                                        Id = guiddocext,
                                        //Nomor = model.VendorRegExt.IndivGiidNo,
                                        //TanggalBerakhir = model.VendorRegExt.IndivGiidValidUntil,
                                        VendorExtId = vExt.Id,
                                        TipeDokumen = (int)EDocumentType.BuktiKepemilikanPeralatan
                                    };
                                    AddVendorDocumentExt(detaildoc);
                                    DocumentImageExt ownershipDoc = new DocumentImageExt()
                                    {
                                        Id = Guid.NewGuid(),
                                        //VendorExtId = guidvenregext,
                                        Content = Convert.FromBase64String(model.BuktiKepemilikanPeralatan.base64),
                                        FileName = model.BuktiKepemilikanPeralatan.FileName,
                                        ContentType = model.BuktiKepemilikanPeralatan.ContentType,
                                        DocumenExtId = detaildoc.Id
                                    };
                                    AddVendorDocumentImageExt(ownershipDoc);
                                    equipmentAdd.EquipmentOwnershipDocId = ownershipDoc.Id;
                                }

                                if (model.FotoPeralatan != null)
                                {
                                    var guiddocext = Guid.NewGuid();
                                    DocumentExt detaildoc = new DocumentExt()
                                    {
                                        Id = guiddocext,
                                        //Nomor = model.VendorRegExt.IndivGiidNo,
                                        //TanggalBerakhir = model.VendorRegExt.IndivGiidValidUntil,
                                        VendorExtId = vExt.Id,
                                        TipeDokumen = (int)EDocumentType.FotoPeralatan
                                    };
                                    AddVendorDocumentExt(detaildoc);
                                    DocumentImageExt ownershipDoc = new DocumentImageExt()
                                    {
                                        Id = Guid.NewGuid(),
                                        //VendorExtId = guidvenregext,
                                        Content = Convert.FromBase64String(model.FotoPeralatan.base64),
                                        FileName = model.FotoPeralatan.FileName,
                                        ContentType = model.FotoPeralatan.ContentType,
                                        DocumenExtId = detaildoc.Id

                                    };
                                    AddVendorDocumentImageExt(ownershipDoc);
                                    equipmentAdd.EquipmentPicture = ownershipDoc.Id;
                                }
                                listEquipment.Add(equipmentAdd);
                            }
                        }
                        if (listEquipment.Count != 0) AddVendorExtEquipment(listEquipment);
                    }

                    var jhExt = ctx.VendorExtJobHistories.Where(x => x.VendorExtId == vExt.Id).ToList();
                    if (jhExt.Count > 0)
                    {
                        for (int i = 0; i < jhExt.Count; i++)
                        {
                            if (model.VendorJobHistoryExt[i] != null)
                            {
                                jhExt[i].JobTitle = model.VendorJobHistoryExt[i].JobTitle;
                                jhExt[i].JobLocation = model.VendorJobHistoryExt[i].JobLocation;
                                jhExt[i].JobClient = model.VendorJobHistoryExt[i].JobClient;
                                jhExt[i].JobType = model.VendorJobHistoryExt[i].JobType;
                                jhExt[i].JobStartDate = model.VendorJobHistoryExt[i].JobStartDate;
                                jhExt[i].JobContractNum = model.VendorJobHistoryExt[i].JobContractNum;
                                jhExt[i].JobContractDate = model.VendorJobHistoryExt[i].JobContractDate;
                                jhExt[i].JobContractAmount = model.VendorJobHistoryExt[i].JobContractAmount;
                                jhExt[i].JobContractAmountCurrencyCode = model.VendorJobHistoryExt[i].JobContractAmountCurrencyCode;
                            }
                        }
                    }
                    else
                    {
                        List<VendorExtJobHistory> ListJobhistory = new List<VendorExtJobHistory>();
                        foreach (var item in model.VendorJobHistoryExt)
                        {
                            if (item.JobTitle != null && item.JobTitle != "")
                            {
                                VendorExtJobHistory jobHistoryExt = new VendorExtJobHistory()
                                {
                                    Id = Guid.NewGuid(),
                                    JobTitle = item.JobTitle,
                                    JobClient = item.JobClient,
                                    JobLocation = item.JobLocation,
                                    JobStartDate = item.JobStartDate,
                                    JobContractNum = item.JobContractNum,
                                    JobContractDate = item.JobContractDate,
                                    JobContractAmount = item.JobContractAmount,
                                    JobContractAmountCurrencyCode = item.JobContractAmountCurrencyCode,
                                    //JobContractDocId = item.JobContractDocId,
                                    JobType = item.JobType,
                                    VendorExtId = vExt.Id

                                };
                                if (model.BuktiKerjasama.base64 != null)
                                {
                                    var guiddocext = Guid.NewGuid();
                                    DocumentExt detaildoc = new DocumentExt()
                                    {
                                        Id = guiddocext,
                                        //Nomor = model.VendorRegExt.IndivGiidNo,
                                        //TanggalBerakhir = model.VendorRegExt.IndivGiidValidUntil,
                                        VendorExtId = vExt.Id,
                                        TipeDokumen = (int)EDocumentType.BuktiKerjasama
                                    };
                                    AddVendorDocumentExt(detaildoc);
                                    DocumentImageExt regVendorDocumentExtRekeningKoran = new DocumentImageExt()
                                    {
                                        Id = Guid.NewGuid(),
                                        Content = Convert.FromBase64String(model.BuktiKerjasama.base64),
                                        FileName = model.BuktiKerjasama.FileName,
                                        ContentType = model.BuktiKerjasama.ContentType,
                                        DocumenExtId = guiddocext
                                    };
                                    AddVendorDocumentImageExt(regVendorDocumentExtRekeningKoran);
                                    jobHistoryExt.JobContractDocId = detaildoc.Id;
                                }
                                ListJobhistory.Add(jobHistoryExt);
                            }
                        }
                        if (ListJobhistory.Count != 0) AddVendorExtJobHistory(ListJobhistory);
                    }

                    var isFeExtProcess = false;
                    var feExt = ctx.VendorExtFinStatements.Where(x => x.VendorExtId == vExt.Id).FirstOrDefault();
                    if (feExt != null)
                    {
                        feExt.FinStmtYear = model.VendorFinStatementExt.FinStmtYear;
                        feExt.FinStmtCurrencyCode = model.VendorFinStatementExt.FinStmtCurrencyCode;
                        feExt.FinStmtAktivaLancar = model.VendorFinStatementExt.FinStmtAktivaLancar;
                        feExt.FinStmtHutangLancar = model.VendorFinStatementExt.FinStmtHutangLancar;
                        feExt.FinStmtRasioLikuiditas = model.VendorFinStatementExt.FinStmtRasioLikuiditas;
                        feExt.FinStmtTotalHutang = model.VendorFinStatementExt.FinStmtTotalHutang;
                        feExt.FinStmtEkuitas = model.VendorFinStatementExt.FinStmtEkuitas;
                        feExt.FinStmtDebtToEquityRation = model.VendorFinStatementExt.FinStmtDebtToEquityRatio;
                        feExt.FinStmtNetProfitLoss = model.VendorFinStatementExt.FinStmtNetProfitLoss;
                        feExt.FinStmtReturnOfEquity = model.VendorFinStatementExt.FinStmtReturnOfEquity;
                        feExt.FinStmtKas = model.VendorFinStatementExt.FinStmtKas;
                        feExt.FinStmtTotalAktiva = model.VendorFinStatementExt.FinStmtTotalAktiva;
                        feExt.FinStmtAuditStatusCode = model.VendorFinStatementExt.FinStmtAuditStatusCode;

                    }
                    else
                    {
                        VendorExtFinStatement finStateExt = new VendorExtFinStatement()
                        {
                            Id = Guid.NewGuid(),
                            FinStmtDocNumber = model.VendorFinStatementExt.FinStmtDocNumber,
                            FinStmtIssuer = model.VendorFinStatementExt.FinStmtIssuer,
                            FinStmtIssueDate = model.VendorFinStatementExt.FinStmtIssueDate,
                            FinStmtValidThruDate = model.VendorFinStatementExt.FinStmtValidThruDate,
                            FinStmtDocumentId = model.VendorFinStatementExt.FinStmtDocumentId,
                            FinStmtYear = model.VendorFinStatementExt.FinStmtYear,
                            FinStmtCurrencyCode = model.VendorFinStatementExt.FinStmtCurrencyCode,
                            FinStmtAktivaLancar = model.VendorFinStatementExt.FinStmtAktivaLancar,
                            FinStmtHutangLancar = model.VendorFinStatementExt.FinStmtHutangLancar,
                            FinStmtRasioLikuiditas = model.VendorFinStatementExt.FinStmtRasioLikuiditas,
                            FinStmtTotalHutang = model.VendorFinStatementExt.FinStmtTotalHutang,
                            FinStmtEkuitas = model.VendorFinStatementExt.FinStmtEkuitas,
                            FinStmtDebtToEquityRation = model.VendorFinStatementExt.FinStmtDebtToEquityRatio,
                            FinStmtNetProfitLoss = model.VendorFinStatementExt.FinStmtNetProfitLoss,
                            FinStmtReturnOfEquity = model.VendorFinStatementExt.FinStmtReturnOfEquity,
                            FinStmtKas = model.VendorFinStatementExt.FinStmtKas,
                            FinStmtTotalAktiva = model.VendorFinStatementExt.FinStmtTotalAktiva,
                            FinStmtAuditStatusCode = model.VendorFinStatementExt.FinStmtAuditStatusCode,
                            VendorExtId = vExt.Id,
                        };
                        if (model.VendorFinStatementExt.base64 != null && feExt == null)
                        {
                            var guiddocext = Guid.NewGuid();
                            DocumentExt detaildoc = new DocumentExt()
                            {
                                Id = guiddocext,
                                VendorExtId = vExt.Id,
                                TipeDokumen = (int)EDocumentType.LaporanDataKeuangan
                            };
                            AddVendorDocumentExt(detaildoc);
                            DocumentImageExt findoc = new DocumentImageExt()
                            {
                                Id = Guid.NewGuid(),
                                Content = Convert.FromBase64String(model.VendorFinStatementExt.base64),
                                FileName = model.VendorFinStatementExt.FileName,
                                ContentType = model.VendorFinStatementExt.ContentType,
                                DocumenExtId = detaildoc.Id
                            };
                            AddVendorDocumentImageExt(findoc);
                            finStateExt.FinStmtDocId = findoc.Id;
                        }
                        AddVendorExtFinStatement(finStateExt);
                    }
                    #endregion

                    #region docx
                    var npwpDocx = ctx.DocumentExts.Where(x => x.VendorExtId == vExt.Id && x.TipeDokumen == 0).FirstOrDefault();
                    if (model.NPWP != null && model.NPWP.base64 != "" && model.NPWP.base64 != null)
                    {
                        npwpDocxOld = npwpDocx;
                        npwpDocx.Nomor = model.NPWP.Nomor;
                        npwpDocx.Penerbit = model.NPWP.Pembuat;
                        npwpDocx.TanggalTerbit = model.NPWP.TanggalTerbit;
                        npwpDocx.TanggalBerakhir = model.NPWP.TanggalBerakhir;

                        var npwpImageDocx = ctx.DocumentImageExts.Where(x => x.DocumenExtId == npwpDocx.Id).FirstOrDefault();
                        if (npwpImageDocx != null)
                        {
                            npwpImageDocx.Content = Convert.FromBase64String(model.NPWP.base64);
                            npwpImageDocx.FileName = model.NPWP.FileName;
                            npwpImageDocx.ContentType = model.NPWP.ContentType;
                        }
                    }

                    var pkpDocx = ctx.DocumentExts.Where(x => x.VendorExtId == vExt.Id && x.TipeDokumen == 1).FirstOrDefault();
                    if (model.PKP != null && model.PKP.base64 != "" && model.PKP.base64 != null)
                    {
                        if (pkpDocx == null)
                        {
                            var guiddocext = Guid.NewGuid();
                            DocumentExt pkpnewDocx = new DocumentExt()
                            {
                                Id = guiddocext,
                                Nomor = model.PKP.Nomor,
                                VendorExtId = vExt.Id,
                                TipeDokumen = (int)EDocumentType.PKP,
                            };
                            ctx.DocumentExts.Add(pkpnewDocx);

                            DocumentImageExt pkpNewdoc = new DocumentImageExt()
                            {
                                Id = Guid.NewGuid(),
                                Content = Convert.FromBase64String(model.PKP.base64),
                                FileName = model.PKP.FileName,
                                ContentType = model.PKP.ContentType,
                                DocumenExtId = guiddocext
                            };
                            ctx.DocumentImageExts.Add(pkpNewdoc);
                        }
                        else
                        {
                            pkpDocxOld = pkpDocx;
                            pkpDocx.Nomor = model.PKP.Nomor;
                            pkpDocx.Penerbit = model.PKP.Pembuat;
                            pkpDocx.TanggalTerbit = model.PKP.TanggalTerbit;
                            pkpDocx.TanggalBerakhir = model.PKP.TanggalBerakhir;

                            var pkpImageDocx = ctx.DocumentImageExts.Where(x => x.DocumenExtId == pkpDocx.Id).FirstOrDefault();
                            if (pkpImageDocx != null)
                            {
                                pkpImageDocx.Content = Convert.FromBase64String(model.PKP.base64);
                                pkpImageDocx.FileName = model.PKP.FileName;
                                pkpImageDocx.ContentType = model.PKP.ContentType;
                            }
                        }
                    }

                    var tdpDocx = ctx.DocumentExts.Where(x => x.VendorExtId == vExt.Id && x.TipeDokumen == 2).FirstOrDefault();
                    if (model.TDP != null && model.TDP.base64 != "" && model.TDP.base64 != null)
                    {
                        tdpDocxOld = tdpDocx;
                        tdpDocx.Nomor = model.TDP.Nomor;
                        tdpDocx.Penerbit = model.TDP.Pembuat;
                        tdpDocx.TanggalTerbit = model.TDP.TanggalTerbit;
                        tdpDocx.TanggalBerakhir = model.TDP.TanggalBerakhir;

                        var tdpImageDocx = ctx.DocumentImageExts.Where(x => x.DocumenExtId == tdpDocx.Id).FirstOrDefault();
                        if (tdpImageDocx != null)
                        {
                            tdpImageDocx.Content = Convert.FromBase64String(model.TDP.base64);
                            tdpImageDocx.FileName = model.TDP.FileName;
                            tdpImageDocx.ContentType = model.TDP.ContentType;
                        }
                    }

                    var siupDocx = ctx.DocumentExts.Where(x => x.VendorExtId == vExt.Id && x.TipeDokumen == 3).FirstOrDefault();
                    if (model.SIUP != null && model.SIUP.base64 != "" && model.SIUP.base64 != null)
                    {
                        siupDocxOld = siupDocxOld;
                        siupDocx.Nomor = model.SIUP.Nomor;
                        siupDocx.Penerbit = model.SIUP.Pembuat;
                        siupDocx.TanggalTerbit = model.SIUP.TanggalTerbit;
                        siupDocx.TanggalBerakhir = model.SIUP.TanggalBerakhir;

                        var siupImageDocx = ctx.DocumentImageExts.Where(x => x.DocumenExtId == siupDocx.Id).FirstOrDefault();
                        if (siupImageDocx != null)
                        {
                            siupImageDocx.Content = Convert.FromBase64String(model.SIUP.base64);
                            siupImageDocx.FileName = model.SIUP.FileName;
                            siupImageDocx.ContentType = model.SIUP.ContentType;
                        }
                    }

                    var siujkDocx = ctx.DocumentExts.Where(x => x.VendorExtId == vExt.Id && x.TipeDokumen == 4).FirstOrDefault();
                    if (model.SIUJK != null && model.SIUJK.base64 != "" && model.SIUJK.base64 != null)
                    {
                        siujkDocxOld = siujkDocx;
                        siujkDocx.Nomor = model.SIUJK.Nomor;
                        siujkDocx.Penerbit = model.SIUJK.Pembuat;
                        siujkDocx.TanggalTerbit = model.SIUJK.TanggalTerbit;
                        siujkDocx.TanggalBerakhir = model.SIUJK.TanggalBerakhir;

                        var siujkimageDocx = ctx.DocumentImageExts.Where(x => x.DocumenExtId == siujkDocx.Id).FirstOrDefault();
                        if (siujkimageDocx != null)
                        {
                            siujkimageDocx.Content = Convert.FromBase64String(model.SIUJK.base64);
                            siujkimageDocx.FileName = model.SIUJK.FileName;
                            siujkimageDocx.ContentType = model.SIUJK.ContentType;
                        }
                    }

                    var aktaDocx = ctx.DocumentExts.Where(x => x.VendorExtId == vExt.Id && x.TipeDokumen == 5).FirstOrDefault();
                    if (model.AKTA != null && model.AKTA.base64 != "" && model.AKTA.base64 != null)
                    {
                        aktaDocxOld = aktaDocx;
                        aktaDocx.Nomor = model.AKTA.Nomor;
                        aktaDocx.Penerbit = model.AKTA.Pembuat;
                        aktaDocx.TanggalTerbit = model.AKTA.TanggalTerbit;
                        aktaDocx.TanggalBerakhir = model.AKTA.TanggalBerakhir;

                        var aktaimageDocx = ctx.DocumentImageExts.Where(x => x.DocumenExtId == aktaDocx.Id).FirstOrDefault();
                        if (aktaimageDocx != null)
                        {
                            aktaimageDocx.Content = Convert.FromBase64String(model.AKTA.base64);
                            aktaimageDocx.FileName = model.AKTA.FileName;
                            aktaimageDocx.ContentType = model.AKTA.ContentType;
                        }
                    }

                    var pengadaanDocx = ctx.DocumentExts.Where(x => x.VendorExtId == vExt.Id && x.TipeDokumen == 6).FirstOrDefault();
                    if (model.PENGADAAN != null && model.PENGADAAN.base64 != "" && model.PENGADAAN.base64 != null)
                    {
                        //pengadaanDocxOld = pengadaanDocx;
                        pengadaanDocx.Nomor = model.PENGADAAN.Nomor;
                        pengadaanDocx.Penerbit = model.PENGADAAN.Pembuat;
                        pengadaanDocx.TanggalTerbit = model.PENGADAAN.TanggalTerbit;
                        pengadaanDocx.TanggalBerakhir = model.PENGADAAN.TanggalBerakhir;

                        var pengadaanimageDocx = ctx.DocumentImageExts.Where(x => x.DocumenExtId == pengadaanDocx.Id).FirstOrDefault();
                        if (pengadaanimageDocx != null)
                        {
                            pengadaanimageDocx.Content = Convert.FromBase64String(model.PENGADAAN.base64);
                            pengadaanimageDocx.FileName = model.PENGADAAN.FileName;
                            pengadaanimageDocx.ContentType = model.PENGADAAN.ContentType;
                        }
                    }

                    var ktpDocx = ctx.DocumentExts.Where(x => x.VendorExtId == vExt.Id && x.TipeDokumen == 7).FirstOrDefault();
                    if (model.KTP != null && model.KTP.base64 != "" && model.KTP.base64 != null)
                    {
                        ktpDocxOld = ktpDocx;
                        ktpDocx.Nomor = model.KTP.Nomor;
                        ktpDocx.Penerbit = model.KTP.Pembuat;
                        ktpDocx.TanggalTerbit = model.KTP.TanggalTerbit;
                        ktpDocx.TanggalBerakhir = model.KTP.TanggalBerakhir;

                        var ktpimageDocx = ctx.DocumentImageExts.Where(x => x.DocumenExtId == ktpDocx.Id).FirstOrDefault();
                        if (ktpimageDocx != null)
                        {
                            ktpimageDocx.Content = Convert.FromBase64String(model.KTP.base64);
                            ktpimageDocx.FileName = model.KTP.FileName;
                            ktpimageDocx.ContentType = model.KTP.ContentType;
                        }
                    }

                    var sertifDocx = ctx.DocumentExts.Where(x => x.VendorExtId == vExt.Id && x.TipeDokumen == 8).FirstOrDefault();
                    if (model.SERTIFIKAT != null && model.SERTIFIKAT.base64 != "" && model.SERTIFIKAT.base64 != null)
                    {
                        sertifDocxOld = sertifDocx;
                        sertifDocx.Nomor = model.SERTIFIKAT.Nomor;
                        sertifDocx.Penerbit = model.SERTIFIKAT.Pembuat;
                        sertifDocx.TanggalTerbit = model.SERTIFIKAT.TanggalTerbit;
                        sertifDocx.TanggalBerakhir = model.SERTIFIKAT.TanggalBerakhir;

                        var sertifimageDocx = ctx.DocumentImageExts.Where(x => x.DocumenExtId == sertifDocx.Id).FirstOrDefault();
                        if (sertifimageDocx != null)
                        {
                            sertifimageDocx.Content = Convert.FromBase64String(model.SERTIFIKAT.base64);
                            sertifimageDocx.FileName = model.SERTIFIKAT.FileName;
                            sertifimageDocx.ContentType = model.SERTIFIKAT.ContentType;
                        }
                    }

                    var npwppemilikDocx = ctx.DocumentExts.Where(x => x.VendorExtId == vExt.Id && x.TipeDokumen == 9).FirstOrDefault();
                    if (model.NPWPPemilik != null && model.NPWPPemilik.base64 != "" && model.NPWPPemilik.base64 != null)
                    {
                        npwpDocxOld = npwpDocx;
                        npwppemilikDocx.Nomor = model.NPWPPemilik.Nomor;
                        npwppemilikDocx.Penerbit = model.NPWPPemilik.Pembuat;
                        npwppemilikDocx.TanggalTerbit = model.NPWPPemilik.TanggalTerbit;
                        npwppemilikDocx.TanggalBerakhir = model.NPWPPemilik.TanggalBerakhir;

                        var npwppemilikimageDocx = ctx.DocumentImageExts.Where(x => x.DocumenExtId == npwppemilikDocx.Id).FirstOrDefault();
                        if (npwppemilikimageDocx != null)
                        {
                            npwppemilikimageDocx.Content = Convert.FromBase64String(model.NPWPPemilik.base64);
                            npwppemilikimageDocx.FileName = model.NPWPPemilik.FileName;
                            npwppemilikimageDocx.ContentType = model.NPWPPemilik.ContentType;
                        }
                    }

                    var ktppemilikDocx = ctx.DocumentExts.Where(x => x.VendorExtId == vExt.Id && x.TipeDokumen == 10).FirstOrDefault();
                    if (model.KTPPemilik != null && model.KTPPemilik.base64 != "" && model.KTPPemilik.base64 != null)
                    {
                        ktppemilikDocxOld = ktppemilikDocx;
                        ktppemilikDocx.Nomor = model.KTPPemilik.Nomor;
                        ktppemilikDocx.Penerbit = model.KTPPemilik.Pembuat;
                        ktppemilikDocx.TanggalTerbit = model.KTPPemilik.TanggalTerbit;
                        ktppemilikDocx.TanggalBerakhir = model.KTPPemilik.TanggalBerakhir;

                        var ktppemilikimageDocx = ctx.DocumentImageExts.Where(x => x.DocumenExtId == ktppemilikDocx.Id).FirstOrDefault();
                        if (ktppemilikimageDocx != null)
                        {
                            ktppemilikimageDocx.Content = Convert.FromBase64String(model.KTPPemilik.base64);
                            ktppemilikimageDocx.FileName = model.KTPPemilik.FileName;
                            ktppemilikimageDocx.ContentType = model.KTPPemilik.ContentType;
                        }
                    }

                    var domisiliDocx = ctx.DocumentExts.Where(x => x.VendorExtId == vExt.Id && x.TipeDokumen == 11).FirstOrDefault();
                    if (model.DOMISILI != null && model.DOMISILI.base64 != "" && model.DOMISILI.base64 != null)
                    {
                        domisiliDocxOld = domisiliDocx;
                        domisiliDocx.Nomor = model.DOMISILI.Nomor;
                        domisiliDocx.Penerbit = model.DOMISILI.Pembuat;
                        domisiliDocx.TanggalTerbit = model.DOMISILI.TanggalTerbit;
                        domisiliDocx.TanggalBerakhir = model.DOMISILI.TanggalBerakhir;

                        var domisiliimageDocx = ctx.DocumentImageExts.Where(x => x.DocumenExtId == domisiliDocx.Id).FirstOrDefault();
                        if (domisiliimageDocx != null)
                        {
                            domisiliimageDocx.Content = Convert.FromBase64String(model.DOMISILI.base64);
                            domisiliimageDocx.FileName = model.DOMISILI.FileName;
                            domisiliimageDocx.ContentType = model.DOMISILI.ContentType;
                        }
                    }

                    var lapuangDocx = ctx.DocumentExts.Where(x => x.VendorExtId == vExt.Id && x.TipeDokumen == 12).FirstOrDefault();
                    if (model.LAPORANKEUANGAN != null && model.LAPORANKEUANGAN.base64 != "" && model.LAPORANKEUANGAN.base64 != null)
                    {
                        lapuangDocxOld = lapuangDocx;
                        lapuangDocx.Nomor = model.LAPORANKEUANGAN.Nomor;
                        lapuangDocx.Penerbit = model.LAPORANKEUANGAN.Pembuat;
                        lapuangDocx.TanggalTerbit = model.LAPORANKEUANGAN.TanggalTerbit;
                        lapuangDocx.TanggalBerakhir = model.LAPORANKEUANGAN.TanggalBerakhir;

                        var lapuangimageDocx = ctx.DocumentImageExts.Where(x => x.DocumenExtId == lapuangDocx.Id).FirstOrDefault();
                        if (lapuangimageDocx != null)
                        {
                            lapuangimageDocx.Content = Convert.FromBase64String(model.LAPORANKEUANGAN.base64);
                            lapuangimageDocx.FileName = model.LAPORANKEUANGAN.FileName;
                            lapuangimageDocx.ContentType = model.LAPORANKEUANGAN.ContentType;
                        }
                    }

                    var rekkoranDocx = ctx.DocumentExts.Where(x => x.VendorExtId == vExt.Id && x.TipeDokumen == 13).FirstOrDefault();
                    if (model.REKENINGKORAN != null && model.REKENINGKORAN.base64 != "" && model.REKENINGKORAN.base64 != null)
                    {
                        rekkoranDocxOld = rekkoranDocx;
                        rekkoranDocx.Nomor = model.REKENINGKORAN.Nomor;
                        rekkoranDocx.Penerbit = model.REKENINGKORAN.Pembuat;
                        rekkoranDocx.TanggalTerbit = model.REKENINGKORAN.TanggalTerbit;
                        rekkoranDocx.TanggalBerakhir = model.REKENINGKORAN.TanggalBerakhir;

                        var rekkoranimageDocx = ctx.DocumentImageExts.Where(x => x.DocumenExtId == rekkoranDocx.Id).FirstOrDefault();
                        if (rekkoranimageDocx != null)
                        {
                            rekkoranimageDocx.Content = Convert.FromBase64String(model.REKENINGKORAN.base64);
                            rekkoranimageDocx.FileName = model.REKENINGKORAN.FileName;
                            rekkoranimageDocx.ContentType = model.REKENINGKORAN.ContentType;
                        }
                    }

                    var drtDocx = ctx.DocumentExts.Where(x => x.VendorExtId == vExt.Id && x.TipeDokumen == 14).FirstOrDefault();
                    if (model.DRT != null && model.DRT.base64 != "" && model.DRT.base64 != null)
                    {
                        drtDocxOld = drtDocx;
                        drtDocx.Nomor = model.DRT.Nomor;
                        drtDocx.Penerbit = model.DRT.Pembuat;
                        drtDocx.TanggalTerbit = model.DRT.TanggalTerbit;
                        drtDocx.TanggalBerakhir = model.DRT.TanggalBerakhir;

                        var imageDocx = ctx.DocumentImageExts.Where(x => x.DocumenExtId == drtDocx.Id).FirstOrDefault();
                        if (imageDocx != null)
                        {
                            imageDocx.Content = Convert.FromBase64String(model.DRT.base64);
                            imageDocx.FileName = model.DRT.FileName;
                            imageDocx.ContentType = model.DRT.ContentType;
                        }
                    }

                    var aktapendiriDocx = ctx.DocumentExts.Where(x => x.VendorExtId == vExt.Id && x.TipeDokumen == 15).FirstOrDefault();
                    if (model.AKTAPENDIRIAN != null && model.AKTAPENDIRIAN.base64 != "" && model.AKTAPENDIRIAN.base64 != null)
                    {
                        aktapendiriDocxOld = aktapendiriDocx;
                        aktapendiriDocx.Nomor = model.AKTAPENDIRIAN.Nomor;
                        aktapendiriDocx.Penerbit = model.AKTAPENDIRIAN.Pembuat;
                        aktapendiriDocx.TanggalTerbit = model.AKTAPENDIRIAN.TanggalTerbit;
                        aktapendiriDocx.TanggalBerakhir = model.AKTAPENDIRIAN.TanggalBerakhir;

                        var aktapendiriimageDocx = ctx.DocumentImageExts.Where(x => x.DocumenExtId == aktapendiriDocx.Id).FirstOrDefault();
                        if (aktapendiriimageDocx != null)
                        {
                            aktapendiriimageDocx.Content = Convert.FromBase64String(model.AKTAPENDIRIAN.base64);
                            aktapendiriimageDocx.FileName = model.AKTAPENDIRIAN.FileName;
                            aktapendiriimageDocx.ContentType = model.AKTAPENDIRIAN.ContentType;
                        }
                    }

                    var skkemenkumhamDocx = ctx.DocumentExts.Where(x => x.VendorExtId == vExt.Id && x.TipeDokumen == 16).FirstOrDefault();
                    if (model.SKKEMENKUMHAM != null && model.SKKEMENKUMHAM.base64 != "" && model.SKKEMENKUMHAM.base64 != null)
                    {
                        skkemenkumhamDocxOld = skkemenkumhamDocx;
                        skkemenkumhamDocx.Nomor = model.SKKEMENKUMHAM.Nomor;
                        skkemenkumhamDocx.Penerbit = model.SKKEMENKUMHAM.Pembuat;
                        skkemenkumhamDocx.TanggalTerbit = model.SKKEMENKUMHAM.TanggalTerbit;
                        skkemenkumhamDocx.TanggalBerakhir = model.SKKEMENKUMHAM.TanggalBerakhir;

                        var skkemenkumhamimageDocx = ctx.DocumentImageExts.Where(x => x.DocumenExtId == skkemenkumhamDocx.Id).FirstOrDefault();
                        if (skkemenkumhamimageDocx != null)
                        {
                            skkemenkumhamimageDocx.Content = Convert.FromBase64String(model.SKKEMENKUMHAM.base64);
                            skkemenkumhamimageDocx.FileName = model.SKKEMENKUMHAM.FileName;
                            skkemenkumhamimageDocx.ContentType = model.SKKEMENKUMHAM.ContentType;
                        }
                    }

                    var beritanegaraDocx = ctx.DocumentExts.Where(x => x.VendorExtId == vExt.Id && x.TipeDokumen == 17).FirstOrDefault();
                    if (model.BERITANEGARA != null && model.BERITANEGARA.base64 != "" && model.BERITANEGARA.base64 != null)
                    {
                        beritanegaraDocxOld = beritanegaraDocx;
                        beritanegaraDocx.Nomor = model.BERITANEGARA.Nomor;
                        beritanegaraDocx.Penerbit = model.BERITANEGARA.Pembuat;
                        beritanegaraDocx.TanggalTerbit = model.BERITANEGARA.TanggalTerbit;
                        beritanegaraDocx.TanggalBerakhir = model.BERITANEGARA.TanggalBerakhir;

                        var beritanegaraimageDocx = ctx.DocumentImageExts.Where(x => x.DocumenExtId == beritanegaraDocx.Id).FirstOrDefault();
                        if (beritanegaraimageDocx != null)
                        {
                            beritanegaraimageDocx.Content = Convert.FromBase64String(model.BERITANEGARA.base64);
                            beritanegaraimageDocx.FileName = model.BERITANEGARA.FileName;
                            beritanegaraimageDocx.ContentType = model.BERITANEGARA.ContentType;
                        }
                    }

                    var aktaperubahnDocx = ctx.DocumentExts.Where(x => x.VendorExtId == vExt.Id && x.TipeDokumen == 18).FirstOrDefault();
                    if (model.AKTAPERUBAHAN != null && model.AKTAPERUBAHAN.base64 != "" && model.AKTAPERUBAHAN.base64 != null)
                    {
                        aktaperubahnDocxOld = aktaperubahnDocx;
                        aktaperubahnDocx.Nomor = model.AKTAPERUBAHAN.Nomor;
                        aktaperubahnDocx.Penerbit = model.AKTAPERUBAHAN.Pembuat;
                        aktaperubahnDocx.TanggalTerbit = model.AKTAPERUBAHAN.TanggalTerbit;
                        aktaperubahnDocx.TanggalBerakhir = model.AKTAPERUBAHAN.TanggalBerakhir;

                        var aktaperubahnimageDocx = ctx.DocumentImageExts.Where(x => x.DocumenExtId == aktaperubahnDocx.Id).FirstOrDefault();
                        if (aktaperubahnimageDocx != null)
                        {
                            aktaperubahnimageDocx.Content = Convert.FromBase64String(model.AKTAPERUBAHAN.base64);
                            aktaperubahnimageDocx.FileName = model.AKTAPERUBAHAN.FileName;
                            aktaperubahnimageDocx.ContentType = model.AKTAPERUBAHAN.ContentType;
                        }
                    }

                    var profilperusahaanDocx = ctx.DocumentExts.Where(x => x.VendorExtId == vExt.Id && x.TipeDokumen == 19).FirstOrDefault();
                    if (model.PROFILPERUSAHAAN != null && model.PROFILPERUSAHAAN.base64 != "" && model.PROFILPERUSAHAAN.base64 != null)
                    {
                        profilperusahaanDocxOld = profilperusahaanDocx;
                        profilperusahaanDocx.Nomor = model.PROFILPERUSAHAAN.Nomor;
                        profilperusahaanDocx.Penerbit = model.PROFILPERUSAHAAN.Pembuat;
                        profilperusahaanDocx.TanggalTerbit = model.PROFILPERUSAHAAN.TanggalTerbit;
                        profilperusahaanDocx.TanggalBerakhir = model.PROFILPERUSAHAAN.TanggalBerakhir;

                        var profilperusahaanimageDocx = ctx.DocumentImageExts.Where(x => x.DocumenExtId == profilperusahaanDocx.Id).FirstOrDefault();
                        if (profilperusahaanimageDocx != null)
                        {
                            profilperusahaanimageDocx.Content = Convert.FromBase64String(model.PROFILPERUSAHAAN.base64);
                            profilperusahaanimageDocx.FileName = model.PROFILPERUSAHAAN.FileName;
                            profilperusahaanimageDocx.ContentType = model.PROFILPERUSAHAAN.ContentType;
                        }
                    }

                    var nibDocx = ctx.DocumentExts.Where(x => x.VendorExtId == vExt.Id && x.TipeDokumen == 20).FirstOrDefault();
                    if (model.NIB != null && model.NIB.base64 != "" && model.NIB.base64 != null)
                    {
                        nibDocxOld = nibDocx;
                        nibDocx.Nomor = model.NIB.Nomor;
                        nibDocx.Penerbit = model.NIB.Pembuat;
                        nibDocx.TanggalTerbit = model.NIB.TanggalTerbit;
                        nibDocx.TanggalBerakhir = model.NIB.TanggalBerakhir;

                        var nibimageDocx = ctx.DocumentImageExts.Where(x => x.DocumenExtId == nibDocx.Id).FirstOrDefault();
                        if (nibimageDocx != null)
                        {
                            nibimageDocx.Content = Convert.FromBase64String(model.NIB.base64);
                            nibimageDocx.FileName = model.NIB.FileName;
                            nibimageDocx.ContentType = model.NIB.ContentType;
                        }
                    }

                    var doksertifcvDocx = ctx.DocumentExts.Where(x => x.VendorExtId == vExt.Id && x.TipeDokumen == 21).FirstOrDefault();
                    if (model.DokumenSertifikatCV != null && model.DokumenSertifikatCV.base64 != "" && model.DokumenSertifikatCV.base64 != null && doksertifcvDocx != null)
                    {
                        doksertifcvDocxOld = doksertifcvDocx;
                        if (model.DokumenSertifikatCV.Nomor != null)
                            doksertifcvDocx.Nomor = model.DokumenSertifikatCV.Nomor;
                        if (model.DokumenSertifikatCV.Pembuat != null)
                            doksertifcvDocx.Penerbit = model.DokumenSertifikatCV.Pembuat;
                        if (model.DokumenSertifikatCV.TanggalTerbit != null)
                            doksertifcvDocx.TanggalTerbit = model.DokumenSertifikatCV.TanggalTerbit;
                        if (model.DokumenSertifikatCV.TanggalBerakhir != null)
                            doksertifcvDocx.TanggalBerakhir = model.DokumenSertifikatCV.TanggalBerakhir;

                        var doksertifcvimageDocx = ctx.DocumentImageExts.Where(x => x.DocumenExtId == doksertifcvDocx.Id).FirstOrDefault();
                        if (doksertifcvimageDocx != null)
                        {
                            doksertifcvimageDocx.Content = Convert.FromBase64String(model.DokumenSertifikatCV.base64);
                            doksertifcvimageDocx.FileName = model.DokumenSertifikatCV.FileName;
                            doksertifcvimageDocx.ContentType = model.DokumenSertifikatCV.ContentType;
                        }
                    }

                    var buktikepemilikanperalatanDocx = ctx.DocumentExts.Where(x => x.VendorExtId == vExt.Id && x.TipeDokumen == 22).FirstOrDefault();
                    if (model.BuktiKepemilikanPeralatan != null && model.BuktiKepemilikanPeralatan.base64 != "" && model.BuktiKepemilikanPeralatan.base64 != null && buktikepemilikanperalatanDocx != null)
                    {
                        buktikepemilikanperalatanDocxOld = buktikepemilikanperalatanDocx;
                        if (model.BuktiKepemilikanPeralatan.Nomor != null)
                            buktikepemilikanperalatanDocx.Nomor = model.BuktiKepemilikanPeralatan.Nomor;
                        if (model.BuktiKepemilikanPeralatan.Pembuat != null)
                            buktikepemilikanperalatanDocx.Penerbit = model.BuktiKepemilikanPeralatan.Pembuat;
                        if (model.BuktiKepemilikanPeralatan.TanggalTerbit != null)
                            buktikepemilikanperalatanDocx.TanggalTerbit = model.BuktiKepemilikanPeralatan.TanggalTerbit;
                        if (model.BuktiKepemilikanPeralatan.TanggalBerakhir != null)
                            buktikepemilikanperalatanDocx.TanggalBerakhir = model.BuktiKepemilikanPeralatan.TanggalBerakhir;

                        var buktikepemilikanperalatanimageDocx = ctx.DocumentImageExts.Where(x => x.DocumenExtId == buktikepemilikanperalatanDocx.Id).FirstOrDefault();
                        if (buktikepemilikanperalatanimageDocx != null)
                        {
                            buktikepemilikanperalatanimageDocx.Content = Convert.FromBase64String(model.BuktiKepemilikanPeralatan.base64);
                            buktikepemilikanperalatanimageDocx.FileName = model.BuktiKepemilikanPeralatan.FileName;
                            buktikepemilikanperalatanimageDocx.ContentType = model.BuktiKepemilikanPeralatan.ContentType;
                        }
                    }

                    var fotoperalatanDocx = ctx.DocumentExts.Where(x => x.VendorExtId == vExt.Id && x.TipeDokumen == 23).FirstOrDefault();
                    if (model.FotoPeralatan != null && model.FotoPeralatan.base64 != "" && model.FotoPeralatan.base64 != null && fotoperalatanDocx != null)
                    {
                        fotoperalatanDocxOld = fotoperalatanDocx;
                        if (model.FotoPeralatan.Nomor != null)
                            fotoperalatanDocx.Nomor = model.FotoPeralatan.Nomor;
                        if (model.FotoPeralatan.Pembuat != null)
                            fotoperalatanDocx.Penerbit = model.FotoPeralatan.Pembuat;
                        if (model.FotoPeralatan.TanggalTerbit != null)
                            fotoperalatanDocx.TanggalTerbit = model.FotoPeralatan.TanggalTerbit;
                        if (model.FotoPeralatan.TanggalBerakhir != null)
                            fotoperalatanDocx.TanggalBerakhir = model.FotoPeralatan.TanggalBerakhir;

                        var fotoperalatanimageDocx = ctx.DocumentImageExts.Where(x => x.DocumenExtId == fotoperalatanDocx.Id).FirstOrDefault();
                        if (fotoperalatanimageDocx != null)
                        {
                            fotoperalatanimageDocx.Content = Convert.FromBase64String(model.FotoPeralatan.base64);
                            fotoperalatanimageDocx.FileName = model.FotoPeralatan.FileName;
                            fotoperalatanimageDocx.ContentType = model.FotoPeralatan.ContentType;
                        }
                    }

                    var buktikerjasamaDocx = ctx.DocumentExts.Where(x => x.VendorExtId == vExt.Id && x.TipeDokumen == 24).FirstOrDefault();
                    if (model.BuktiKerjasama != null && model.BuktiKerjasama.base64 != "" && model.BuktiKerjasama.base64 != null && buktikerjasamaDocx != null)
                    {
                        buktikerjasamaDocxOld = buktikerjasamaDocx;
                        if (model.BuktiKerjasama.Nomor != null)
                            buktikerjasamaDocx.Nomor = model.BuktiKerjasama.Nomor;
                        if (model.BuktiKerjasama.Pembuat != null)
                            buktikerjasamaDocx.Penerbit = model.BuktiKerjasama.Pembuat;
                        if (model.BuktiKerjasama.TanggalTerbit != null)
                            buktikerjasamaDocx.TanggalTerbit = model.BuktiKerjasama.TanggalTerbit;
                        if (model.BuktiKerjasama.TanggalBerakhir != null)
                            buktikerjasamaDocx.TanggalBerakhir = model.BuktiKerjasama.TanggalBerakhir;

                        var buktikerjasamaimageDocx = ctx.DocumentImageExts.Where(x => x.DocumenExtId == buktikerjasamaDocx.Id).FirstOrDefault();
                        if (buktikerjasamaimageDocx != null)
                        {
                            buktikerjasamaimageDocx.Content = Convert.FromBase64String(model.BuktiKerjasama.base64);
                            buktikerjasamaimageDocx.FileName = model.BuktiKerjasama.FileName;
                            buktikerjasamaimageDocx.ContentType = model.BuktiKerjasama.ContentType;
                        }
                    }

                    var laporankeuanganDocx = ctx.DocumentExts.Where(x => x.VendorExtId == vExt.Id && x.TipeDokumen == 24).FirstOrDefault();
                    if (model.LaporanDataKeuangan != null && model.LaporanDataKeuangan.base64 != "" && model.LaporanDataKeuangan.base64 != null && laporankeuanganDocx != null)
                    {
                        laporankeuanganDocxOld = laporankeuanganDocx;
                        if (model.LaporanDataKeuangan.Nomor != null)
                            laporankeuanganDocx.Nomor = model.LaporanDataKeuangan.Nomor;
                        if (model.LaporanDataKeuangan.Pembuat != null)
                            laporankeuanganDocx.Penerbit = model.LaporanDataKeuangan.Pembuat;
                        if (model.LaporanDataKeuangan.TanggalTerbit != null)
                            laporankeuanganDocx.TanggalTerbit = model.LaporanDataKeuangan.TanggalTerbit;
                        if (model.LaporanDataKeuangan.TanggalBerakhir != null)
                            laporankeuanganDocx.TanggalBerakhir = model.LaporanDataKeuangan.TanggalBerakhir;

                        var laporankeuanganimageDocx = ctx.DocumentImageExts.Where(x => x.DocumenExtId == laporankeuanganDocx.Id).FirstOrDefault();
                        if (laporankeuanganimageDocx != null)
                        {
                            laporankeuanganimageDocx.Content = Convert.FromBase64String(model.LaporanDataKeuangan.base64);
                            laporankeuanganimageDocx.FileName = model.LaporanDataKeuangan.FileName;
                            laporankeuanganimageDocx.ContentType = model.LaporanDataKeuangan.ContentType;
                        }
                    }

                    if (vOld != null || vExtOld != null || biExtOld != null || npwpDocx != null || pkpDocx != null || tdpDocx != null || siupDocx != null ||
                    siujkDocx != null || aktaDocx != null || ktpDocx != null || domisiliDocx != null || lapuangDocx != null || rekkoranDocx != null ||
                    drtDocx != null || aktapendiriDocx != null || skkemenkumhamDocx != null || aktaperubahnDocx != null || nibDocx != null ||
                    laporankeuanganDocx != null)
                    {
                        #region info
                        string a = vOld.Nama;
                        string b = vOld.Alamat;
                        string c = vOld.Provinsi; //code
                        string d = vExtOld.FirstLevelDivisionCode; //code kota
                        string e = vExtOld.PostalCode;
                        string f = vOld.Email;
                        string g = vOld.Website;
                        string h = vOld.Telepon;
                        #endregion

                        #region compare info
                        if (a != null) a = "Nama dari " + a + " menjadi " + v.Nama + ", ";
                        if (b != null) b = "Alamat dari " + b + " menjadi " + v.Alamat + ", ";
                        if (c != null) c = "Provinsi dari " + c + " menjadi " + v.Provinsi + ", ";
                        if (d != null) d = "Kota dari " + d + " menjadi " + vExt.FirstLevelDivisionCode + ", ";
                        if (e != null) e = "Kode Pos dari " + e + " menjadi " + vExt.PostalCode + ", ";
                        if (f != null) f = "Email dari " + f + " menjadi " + v.Email + ", ";
                        if (g != null) g = "Website dari " + g + " menjadi " + v.Website + ", ";
                        if (h != null) h = "Telepon dari " + h + " menjadi " + v.Telepon + ", ";
                        #endregion

                        #region docx
                        string i = null; string NPWPbeforeNomor = null; string NPWPafterNomor = null;
                        if ((npwpDocx != null && npwpDocxOld != null) && npwpDocxOld.Nomor != npwpDocx.Nomor)
                        {
                            NPWPbeforeNomor = " Nomor: " + npwpDocxOld.Nomor;
                            NPWPafterNomor = " Nomor: " + npwpDocx.Nomor;
                            i = "File Dokumen NPWP, Data Akta dari " + NPWPbeforeNomor + " menjadi " + NPWPafterNomor + ", ";
                        }
                        string j = null; string PKPbeforeNomor = null; string PKPafterNomor = null;
                        if ((pkpDocx != null && pkpDocxOld != null) && pkpDocxOld.Nomor != pkpDocx.Nomor)
                        {
                            PKPbeforeNomor = " Nomor: " + pkpDocxOld.Nomor;
                            PKPafterNomor = " Nomor: " + pkpDocx.Nomor;
                            j = "File Dokumen PKP, Data Akta dari " + PKPbeforeNomor + " menjadi " + PKPafterNomor + ", ";
                        }
                        string k = null; string TDPbeforeNomor = null; string TDPafterNomor = null;
                        if ((tdpDocx != null && tdpDocxOld != null) && tdpDocxOld.Nomor != tdpDocx.Nomor)
                        {
                            TDPbeforeNomor = " Nomor: " + tdpDocxOld.Nomor;
                            TDPafterNomor = " Nomor: " + tdpDocx.Nomor;
                            k = "File Dokumen TDP, Data Akta dari " + TDPbeforeNomor + " menjadi " + TDPafterNomor + ", ";
                        }
                        string l = null; string SIUPbeforeNomor = null; string SIUPafterNomor = null;
                        if ((siupDocx != null && siupDocxOld != null) && siupDocxOld.Nomor != siupDocx.Nomor)
                        {
                            SIUPbeforeNomor = " Nomor: " + siupDocxOld.Nomor;
                            SIUPafterNomor = " Nomor: " + siupDocx.Nomor;
                            l = "File Dokumen SIUP, Data Akta dari " + SIUPbeforeNomor + " menjadi " + SIUPafterNomor + ", ";
                        }
                        string m = null; string SIUJKbeforeNomor = null; string SIUJKafterNomor = null;
                        if ((siujkDocx != null && siujkDocxOld != null) && siujkDocxOld.Nomor != siujkDocx.Nomor)
                        {
                            SIUJKbeforeNomor = " Nomor: " + siujkDocxOld.Nomor;
                            SIUJKafterNomor = " Nomor: " + siujkDocx.Nomor;
                            m = "File Dokumen SIUJK, Data Akta dari " + SIUJKbeforeNomor + " menjadi " + SIUJKafterNomor + ", ";
                        }
                        string n = null; string AKTAbeforeNomor = null; string AKTAafterNomor = null;
                        if ((aktaDocx != null && aktaDocxOld != null) && aktaDocxOld.Nomor != aktaDocx.Nomor)
                        {
                            AKTAbeforeNomor = " Nomor: " + aktaDocxOld.Nomor;
                            AKTAafterNomor = " Nomor: " + aktaDocx.Nomor;
                            n = "File Dokumen Akta, Data Akta dari " + AKTAbeforeNomor + " menjadi " + AKTAafterNomor + ", ";
                        }
                        string o = null; string KTPbeforeNomor = null; string KTPafterNomor = null;
                        if ((ktpDocx != null && ktpDocxOld != null) && ktpDocxOld.Nomor != ktpDocx.Nomor)
                        {
                            KTPbeforeNomor = " Nomor: " + ktpDocxOld.Nomor;
                            KTPafterNomor = " Nomor: " + ktpDocx.Nomor;
                            m = "File Dokumen KTP, Data Akta dari " + KTPbeforeNomor + " menjadi " + KTPafterNomor + ", ";
                        }
                        string p = null; string DOMISILIbeforeNomor = null; string DOMISILIafterNomor = null;
                        if ((domisiliDocx != null && domisiliDocxOld != null) && domisiliDocxOld.Nomor != domisiliDocx.Nomor)
                        {
                            DOMISILIbeforeNomor = " Nomor: " + domisiliDocxOld.Nomor;
                            DOMISILIafterNomor = " Nomor: " + domisiliDocx.Nomor;
                            p = "File Dokumen Domisili, Data Akta dari " + DOMISILIbeforeNomor + " menjadi " + DOMISILIafterNomor + ", ";
                        }
                        string q = null; string LAPORANKEUANGANbeforeNomor = null; string LAPORANKEUANGAafterNomor = null;
                        if ((lapuangDocx != null && lapuangDocxOld != null) && lapuangDocxOld.Nomor != lapuangDocx.Nomor)
                        {
                            LAPORANKEUANGANbeforeNomor = " Nomor: " + lapuangDocxOld.Nomor;
                            LAPORANKEUANGAafterNomor = " Nomor: " + lapuangDocx.Nomor;
                            q = "File Dokumen Laporan Keuangan, Data Akta dari " + LAPORANKEUANGANbeforeNomor + " menjadi " + LAPORANKEUANGAafterNomor + ", ";
                        }
                        string r = null; string REKKORANbeforeNomor = null; string REKKORANafterNomor = null;
                        if ((rekkoranDocx != null && rekkoranDocxOld != null) && rekkoranDocxOld.Nomor != rekkoranDocx.Nomor)
                        {
                            REKKORANbeforeNomor = " Nomor: " + rekkoranDocxOld.Nomor;
                            REKKORANafterNomor = " Nomor: " + rekkoranDocx.Nomor;
                            r = "File Dokumen Rekening Koran, Data Akta dari " + REKKORANbeforeNomor + " menjadi " + REKKORANafterNomor + ", ";
                        }
                        string s = null; string DRTbeforeNomor = null; string DRTafterNomor = null;
                        if ((drtDocx != null && drtDocxOld != null) && drtDocxOld.Nomor != drtDocx.Nomor)
                        {
                            DRTbeforeNomor = " Nomor: " + drtDocxOld.Nomor;
                            DRTafterNomor = " Nomor: " + drtDocx.Nomor;
                            s = "File Dokumen DRT, Data Akta dari " + DRTbeforeNomor + " menjadi " + DRTafterNomor + ", ";
                        }
                        string t = null; string AKTAPENDIRIANbeforeNomor = null; string AKTAPENDIRIANafterNomor = null;
                        if ((aktapendiriDocx != null && aktapendiriDocxOld != null) && aktapendiriDocxOld.Nomor != aktapendiriDocx.Nomor)
                        {
                            AKTAPENDIRIANbeforeNomor = " Nomor: " + aktapendiriDocxOld.Nomor;
                            AKTAPENDIRIANafterNomor = " Nomor: " + aktapendiriDocx.Nomor;
                            t = "File Dokumen Akta Pendirian, Data Akta dari " + AKTAPENDIRIANbeforeNomor + " menjadi " + AKTAPENDIRIANafterNomor + ", ";
                        }
                        string u = null; string SKKEMENKUMHAMbeforeNomor = null; string SKKEMENKUMHAMafterNomor = null;
                        if ((skkemenkumhamDocx != null && skkemenkumhamDocxOld != null) && skkemenkumhamDocxOld.Nomor != skkemenkumhamDocx.Nomor)
                        {
                            SKKEMENKUMHAMbeforeNomor = " Nomor: " + skkemenkumhamDocxOld.Nomor;
                            SKKEMENKUMHAMafterNomor = " Nomor: " + skkemenkumhamDocx.Nomor;
                            u = "File Dokumen SK Kemenkumham, Data Akta dari " + SKKEMENKUMHAMbeforeNomor + " menjadi " + SKKEMENKUMHAMafterNomor + ", ";
                        }
                        string w = null; string AKTAPERUBAHANbeforeNomor = null; string AKTAPERUBAHANafterNomor = null;
                        if ((aktaperubahnDocx != null && aktaperubahnDocxOld != null) && aktaperubahnDocxOld.Nomor != aktaperubahnDocx.Nomor)
                        {
                            AKTAPERUBAHANbeforeNomor = " Nomor: " + aktaperubahnDocxOld.Nomor;
                            AKTAPERUBAHANafterNomor = " Nomor: " + aktaperubahnDocx.Nomor;
                            w = "File Dokumen Akta Perubahan, Data Akta dari " + AKTAPERUBAHANbeforeNomor + " menjadi " + AKTAPERUBAHANafterNomor + ", ";
                        }
                        string x = null; string NIBbeforeNomor = null; string NIBafterNomor = null;
                        if ((nibDocx != null && nibDocxOld != null) && nibDocxOld.Nomor != nibDocx.Nomor)
                        {
                            NIBbeforeNomor = " Nomor: " + nibDocxOld.Nomor;
                            NIBafterNomor = " Nomor: " + nibDocx.Nomor;
                            x = "File Dokumen NIB, Data Akta dari " + NIBbeforeNomor + " menjadi " + NIBafterNomor + ", ";
                        }
                        string y = null; string LAPORANNERACAKEUANGANbeforeNomor = null; string LAPORANNERACAKEUANGANafterNomor = null;
                        if ((laporankeuanganDocx != null && laporankeuanganDocxOld != null) && laporankeuanganDocxOld.Nomor != laporankeuanganDocx.Nomor)
                        {
                            LAPORANNERACAKEUANGANbeforeNomor = " Nomor: " + laporankeuanganDocxOld.Nomor;
                            LAPORANNERACAKEUANGANafterNomor = " Nomor: " + laporankeuanganDocx.Nomor;
                            y = "File Dokumen Laporan Data Keuangan (Neraca Keuangan), Data Akta dari " + LAPORANNERACAKEUANGANbeforeNomor + " menjadi " + LAPORANNERACAKEUANGANafterNomor + ", ";
                        }
                        #endregion

                        RiwayatPengajuanVendor rp = new RiwayatPengajuanVendor()
                        {
                            Komentar = "Pengajuan PERUBAHAN " + a + b + c + d + e + f + g + h + i + j + k + l + m + n + o + p + q + r + s + t + u + w + x + y + " oleh " + username,
                            Status = EStatusVendor.UPDATED,
                            Urutan = 0,
                            Metode = EMetodeVerifikasiVendor.NONE,
                            Waktu = DateTime.Now
                        };
                        v.RiwayatPengajuanVendor = new List<RiwayatPengajuanVendor>() { rp };
                    }
                    else
                    {
                        RiwayatPengajuanVendor rp = new RiwayatPengajuanVendor()
                        {
                            Komentar = "Pengajuan PERUBAHAN , oleh " + username,
                            Status = EStatusVendor.UPDATED,
                            Urutan = 0,
                            Metode = EMetodeVerifikasiVendor.NONE,
                            Waktu = DateTime.Now
                        };
                        v.RiwayatPengajuanVendor = new List<RiwayatPengajuanVendor>() { rp };
                    }
                    ctx.SaveChanges();
                    #endregion
                }
                else
                {
                    AddVendorExtFromEdit(model);

                    RiwayatPengajuanVendor rp = new RiwayatPengajuanVendor()
                    {
                        Komentar = "Pengajuan PERUBAHAN , oleh " + username,
                        Status = EStatusVendor.UPDATED,
                        Urutan = 0,
                        Metode = EMetodeVerifikasiVendor.NONE,
                        Waktu = DateTime.Now
                    };
                    v.RiwayatPengajuanVendor = new List<RiwayatPengajuanVendor>() { rp };
                    ctx.SaveChanges();

                }
            }
            return model;
        }

        public string AddVendorExtFromEdit(VendorExtViewModelJaws model)
        //public string AddVendorExt(VendorExtViewModel model)
        {   
            Vendor v = GetVendorById(model.id);
            //NoPengajuan = GenerateNoPengajuan(),
            //NoPengajuan = nomor.ToString(),
            v.TipeVendor = (ETipeVendor)model.TipeVendor;
            v.Nama = model.Nama;
            v.Alamat = model.Alamat;
            //mapp to province name
            //v.Provinsi = model.Provinsi;
            if (model.VendorRegExt.CountryCode.Equals("ID") 
                && (model.Provinsi != null && !(model.Provinsi.Equals(""))))
            {
                v.Provinsi = getNameMaster(model.Provinsi).LocalizedName;
            }
            v.Kota = model.Kota;
            v.KodePos = model.VendorRegExt.PostalCode;
            v.Email = model.Email;
            v.Website = model.Website;
            v.Telepon = model.Telepon;
            v.StatusAkhir = EStatusVendor.UPDATED;

            //Vendor v = new Vendor
            //{
            //    //NoPengajuan = GenerateNoPengajuan(),
            //    //NoPengajuan = nomor.ToString(),
            //    TipeVendor = (ETipeVendor)model.TipeVendor,
            //    Nama = model.Nama,
            //    Alamat = model.Alamat,
            //    Provinsi = model.Provinsi,
            //    Kota = model.Kota,
            //    KodePos = model.KodePos,
            //    Email = model.Email,
            //    Website = model.Website,
            //    Telepon = model.Telepon,
            //    StatusAkhir = EStatusVendor.UPDATED
            //};

            if (model.VendorPersonExt != null)
            {
                List<VendorPerson> lvpOri = new List<VendorPerson>();
                foreach (var vpvm in model.VendorPersonExt)
                {
                    if (vpvm.Name != null && vpvm.Name != "") //persons without name are ignored
                        lvpOri.Add(new VendorPerson
                        {
                            Nama = vpvm.Name,
                            Jabatan = vpvm.Position,
                            Email = vpvm.ContactEmail,
                            Telepon = vpvm.ContactPhone,
                            Active = true
                        });
                };
                v.VendorPerson = lvpOri;
            }
            if (model.VendorBankInfoExt != null)
            {
                BankInfo biOri = new BankInfo
                {
                    NamaBank = model.VendorBankInfoExt.BankCode,
                    Cabang = model.VendorBankInfoExt.Branch,
                    NomorRekening = model.VendorBankInfoExt.AccNumber,
                    NamaRekening = model.VendorBankInfoExt.AccName,

                    Active = true
                };

                if (biOri.NamaBank != null && biOri.NamaBank != "") //banks without name are ignored
                    v.BankInfo = new List<BankInfo>() { biOri };
            }

            //var regvendorId = _repository.AddVendorOri(v);

            var guidvenregext = Guid.NewGuid();
            var pkpGuid = Guid.NewGuid();
            var guidCompanyProfile = Guid.NewGuid();
            if (model.VendorRegExt != null)
            {
                //mapping vendorregext
                var vRegExt = new VendorExt();

                vRegExt.Id = guidvenregext;
                vRegExt.VendorId = model.id;
                vRegExt.JenisVendor = model.TipeVendor.ToString();
                vRegExt.KategoriVendor = model.VendorRegExt.KategoriVendor;
                vRegExt.BentukBadanUsaha = model.VendorRegExt.BentukBadanUsaha;
                vRegExt.StatusPerusahaan = model.VendorRegExt.StatusPerusahaan;
                vRegExt.EstablishedDate = model.VendorRegExt.EstablishedDate;
                vRegExt.KategoriUsaha = model.VendorRegExt.KategoriUsaha;

                vRegExt.CountryCode = model.VendorRegExt.CountryCode;
                vRegExt.FirstLevelDivisionCode = model.FirstLevelDivisionCode;
                vRegExt.SecondLevelDivisionCode = model.SecondLevelDivisionCode;
                vRegExt.ThirdLevelDivisionCode = model.ThirdLevelDivisionCode;
                vRegExt.PostalCode = model.VendorRegExt.PostalCode;
                vRegExt.Fax = model.VendorRegExt.Fax;
                vRegExt.WorkUnitCode = model.VendorRegExt.WorkUnitCode;

                vRegExt.DirPersonGiidNo = model.VendorRegExt.DirPersonGiidNo;
                vRegExt.DirPersonName = model.VendorRegExt.DirPersonName;
                vRegExt.DirPersonPosition = model.VendorRegExt.DirPersonPosition;
                vRegExt.DirPersonReligionCode = model.VendorRegExt.DirPersonReligionCode;
                vRegExt.DirPersonBirthDay = model.VendorRegExt.DirPersonBirthDay;

                if (model.VendorRegExt.IndivAbbrevName != "" && model.VendorRegExt.IndivAbbrevName != null)
                {
                    //DocumentImageExt indivGiidData = new DocumentImageExt()
                    //{
                    //    Id = Guid.NewGuid(),
                    //    Content = Convert.FromBase64String(model.IndivGiid.base64),
                    //    FileName = model.IndivGiid.FileName,
                    //    ContentType = model.IndivGiid.ContentType,
                    //};
                    //_repository.AddVendorDocumentImageExt(indivGiidData);

                    var guiddocext = Guid.NewGuid();
                    DocumentExt detaildoc = new DocumentExt()
                    {
                        Id = guiddocext,
                        Nomor = model.KTP.Nomor,
                        TanggalBerakhir = model.KTP.TanggalBerakhir,
                        VendorExtId = guidvenregext,
                        TipeDokumen = (int)EDocumentType.KTP
                    };
                    AddVendorDocumentExt(detaildoc);
                    DocumentImageExt regVendorDocumentExtRekeningKoran = new DocumentImageExt()
                    {
                        Id = Guid.NewGuid(),
                        Content = Convert.FromBase64String(model.KTP.base64),
                        FileName = model.KTP.FileName,
                        ContentType = model.KTP.ContentType,
                        DocumenExtId = guiddocext
                    };
                    AddVendorDocumentImageExt(regVendorDocumentExtRekeningKoran);

                    vRegExt.IndivGiidDocId = detaildoc.Id;
                    vRegExt.IndivName = model.Nama;
                    vRegExt.IndivAbbrevName = model.VendorRegExt.IndivAbbrevName;
                    vRegExt.IndivGiidNo = model.KTP.Nomor;
                    vRegExt.IndivGiidValidUntil = model.KTP.TanggalBerakhir;
                }

                vRegExt.IndivAddress = model.Alamat;
                vRegExt.IndivCountryCode = model.VendorRegExt.CountryCode;
                vRegExt.IndivFirstLevelDivisionCode = model.FirstLevelDivisionCode;
                vRegExt.IndivSecondLevelDivisionCode = model.SecondLevelDivisionCode;
                vRegExt.IndivThirdLevelDivisionCode = model.ThirdLevelDivisionCode;
                vRegExt.IndivPostalCode = model.VendorRegExt.PostalCode;
                vRegExt.IndivContactPersonName = model.VendorRegExt.IndivContactPersonName;
                vRegExt.IndivContactPhoneNum = model.VendorRegExt.IndivContactPhoneNum;

                vRegExt.IndivTaxNo = model.NPWP.Nomor;
                vRegExt.IndivStateProvinceCode = model.Provinsi;
                vRegExt.IndivPhoneNum = model.VendorRegExt.IndivPhoneNum;
                vRegExt.IndivFax = model.VendorRegExt.IndivFax;
                vRegExt.IndivEmail = model.VendorRegExt.IndivEmail;
                vRegExt.IndivContactEmail = model.VendorRegExt.IndivEmail;

                vRegExt.PrinRepOfficeAddress = model.VendorRegExt.PrinRepOfficeAddress;
                vRegExt.PrinRepOfficeLocalAddress = model.VendorRegExt.PrinRepOfficeLocalAddress;
                vRegExt.PrinRepOfficeContactPhoneNum = model.VendorRegExt.PrinRepOfficeContactPhoneNum;
                vRegExt.PrinRepOfficeFaxNum = model.VendorRegExt.PrinRepOfficeFaxNum;
                vRegExt.PrinRepOfficeEmail = model.VendorRegExt.PrinRepOfficeEmail;
                vRegExt.PrinWebsite = model.VendorRegExt.PrinWebsite;
                vRegExt.PrinRepPosition = model.VendorRegExt.PrinRepPosition;

                vRegExt.CPName = model.VendorRegExt.CPName;

                vRegExt.SegBidangUsahaCode = model.VendorRegExt.SegBidangUsahaCode;

                string ndatastring = string.Empty;
                if (model.VendorRegExt.SegKelompokUsahaCode != null)
                {
                    foreach (var jdata in model.VendorRegExt.SegKelompokUsahaCode)
                    {
                        ndatastring = ndatastring + jdata.ToString() + ',';
                    }
                    ndatastring = ndatastring.Remove(ndatastring.Length - 1, 1);
                    vRegExt.SegKelompokUsahaCode = ndatastring;
                }
                
                //if (model.VendorRegExt.SegBidangUsahaCode == "1")
                //    vRegExt.SegKelompokUsahaCode = model.VendorRegExt.SegKelompokUsahaCodeIT;
                //if (model.VendorRegExt.SegBidangUsahaCode == "2")
                //    vRegExt.SegKelompokUsahaCode = model.VendorRegExt.SegKelompokUsahaCodeNonIT;
                //if (model.VendorRegExt.SegBidangUsahaCode == "3")
                //    vRegExt.SegKelompokUsahaCode = model.VendorRegExt.SegKelompokUsahaCodeKonstruksi;

                //string ndatasub = string.Empty;
                //foreach (var kdata in model.VendorRegExt.SegSubBidangUsahaCode)
                //{
                //    ndatasub = ndatasub + kdata.ToString() + ',';
                //}
                //ndatasub = ndatasub.Remove(ndatasub.Length - 1, 1);
                //vRegExt.SegSubBidangUsahaCode = ndatasub;

                vRegExt.SegKualifikasiGrade = model.VendorRegExt.SegKualifikasiGrade;

                
                if (model.PKP != null && model.PKP.Nomor != null)
                {
                    vRegExt.IsPKP = false;
                    vRegExt.PKPDocId = pkpGuid;
                }
                vRegExt.NomorPKP = model.PKP.Nomor;

                if(model.PROFILPERUSAHAAN != null && model.PROFILPERUSAHAAN.base64 != null)
                {
                    vRegExt.CompanyProfileDocId = guidCompanyProfile;
                }

                //v.RegVendorExt = vRegExt;
                AddVendorExt(vRegExt);
            }

            if (model.VendorPersonExt != null)
            {
                List<VendorExtPerson> lvp = new List<VendorExtPerson>();
                foreach (var item in model.VendorPersonExt)
                {
                    if (item.Name != null && item.Name != "")
                    {
                        VendorExtPerson n = new VendorExtPerson()
                        {
                            Id = Guid.NewGuid(),
                            Name = item.Name,
                            Position = item.Position,
                            ContactPhone = item.ContactPhone,
                            ContactEmail = item.ContactEmail,
                            ContactAddress = item.ContactAddress,
                            ReligionCode = item.ReligionCode,
                            GiidNo = item.GiidNo,
                            VendorExtId = guidvenregext
                        };
                        if (item.BirthDay != null)
                            n.BirthDay = item.BirthDay;
                        lvp.Add(n);
                    }
                }
                if (lvp.Count != 0) AddVendorExtPerson(lvp);
            }

            if (model.VendorBankInfoExt != null)
            {
                VendorExtBankInfo bi = new VendorExtBankInfo()
                {
                    Id = Guid.NewGuid(),
                    BankCode = model.VendorBankInfoExt.BankCode,
                    BankAddress = model.VendorBankInfoExt.BankAddress,
                    BankCity = model.VendorBankInfoExt.BankCity,
                    Branch = model.VendorBankInfoExt.Branch,
                    AccNumber = model.VendorBankInfoExt.AccNumber,
                    AccName = model.VendorBankInfoExt.AccName,
                    AccCurrencyCode = model.VendorBankInfoExt.AccCurrencyCode,
                    VendorExtId = guidvenregext,
                    BankCountry = model.VendorBankInfoExt.BankCountry
                };
                AddVendorExtBankInfo(bi);
            }

            //RegVendorExtHumanResource
            if (model.VendorHumanResourceExt != null)
            {
                List<VendorExtHumanResource> listhrExt = new List<VendorExtHumanResource>();
                foreach (var item in model.VendorHumanResourceExt)
                {
                    if (item.ResourceFullName != null && item.ResourceFullName != "")
                    {
                        VendorExtHumanResource hrExt = new VendorExtHumanResource()
                        {
                            Id = Guid.NewGuid(),
                            ResourceFullName = item.ResourceFullName,
                            ResourceDateOfBirth = item.ResourceDateOfBirth,
                            ResourceExperienceCode = item.ResourceExperienceCode,
                            ResourceExpertise = item.ResourceExpertise,
                            ResourceCVDocId = item.ResourceCVDocId,
                            ResourceLastEduCode = item.ResourceLastEduCode,
                            ResourceLastEduDocId = item.ResourceLastEduDocId,
                            ResourceLastEduIssuer = item.ResourceLastEduIssuer,
                            ResourceCertificationDocId = item.ResourceCertificationDocId,
                            ResourceCertificationIssuer = item.ResourceCertificationIssuer,
                            ResourceExperienceYears = item.ResourceExperienceYears,
                            VendorExtId = guidvenregext

                        };

                        if (model.DokumenSertifikatCV != null)
                        {
                            var guiddocext = Guid.NewGuid();
                            DocumentExt detaildoc = new DocumentExt()
                            {
                                Id = guiddocext,
                                Nomor = model.DokumenSertifikatCV.Nomor,
                                Penerbit = model.DokumenSertifikatCV.Pembuat,
                                TanggalTerbit = model.DokumenSertifikatCV.TanggalTerbit,
                                TanggalBerakhir = model.DokumenSertifikatCV.TanggalBerakhir,
                                VendorExtId = guidvenregext,
                                TipeDokumen = (int)EDocumentType.DokumenSertifikatCV
                            };
                            AddVendorDocumentExt(detaildoc);

                            DocumentImageExt regVendorDocumentExtSertifikasiTenagaAhli = new DocumentImageExt()
                            {
                                Id = Guid.NewGuid(),
                                Content = Convert.FromBase64String(model.DokumenSertifikatCV.base64),
                                FileName = model.DokumenSertifikatCV.FileName,
                                ContentType = model.DokumenSertifikatCV.ContentType,
                                DocumenExtId = guiddocext
                            };
                            AddVendorDocumentImageExt(regVendorDocumentExtSertifikasiTenagaAhli);

                            hrExt.ResourceCertificationDocId = detaildoc.Id;
                        }

                        listhrExt.Add(hrExt);
                    }
                }
                if (listhrExt.Count != 0) AddVendorExtHumanResource(listhrExt);
            }

            //RegVendorExtFinStatement
            if (model.VendorFinStatementExt != null && model.VendorFinStatementExt.FinStmtYear != null)
            {
                VendorExtFinStatement finStateExt = new VendorExtFinStatement()
                {
                    Id = Guid.NewGuid(),
                    FinStmtDocNumber = model.VendorFinStatementExt.FinStmtDocNumber,
                    FinStmtIssuer = model.VendorFinStatementExt.FinStmtIssuer,
                    FinStmtIssueDate = model.VendorFinStatementExt.FinStmtIssueDate,
                    FinStmtValidThruDate = model.VendorFinStatementExt.FinStmtValidThruDate,
                    FinStmtDocumentId = model.VendorFinStatementExt.FinStmtDocumentId,
                    FinStmtYear = model.VendorFinStatementExt.FinStmtYear,
                    FinStmtCurrencyCode = model.VendorFinStatementExt.FinStmtCurrencyCode,
                    FinStmtAktivaLancar = model.VendorFinStatementExt.FinStmtAktivaLancar,
                    FinStmtHutangLancar = model.VendorFinStatementExt.FinStmtHutangLancar,
                    FinStmtRasioLikuiditas = model.VendorFinStatementExt.FinStmtRasioLikuiditas,
                    FinStmtTotalHutang = model.VendorFinStatementExt.FinStmtTotalHutang,
                    FinStmtEkuitas = model.VendorFinStatementExt.FinStmtEkuitas,
                    FinStmtDebtToEquityRation = model.VendorFinStatementExt.FinStmtDebtToEquityRatio,
                    FinStmtNetProfitLoss = model.VendorFinStatementExt.FinStmtNetProfitLoss,
                    FinStmtReturnOfEquity = model.VendorFinStatementExt.FinStmtReturnOfEquity,
                    FinStmtKas = model.VendorFinStatementExt.FinStmtKas,
                    FinStmtTotalAktiva = model.VendorFinStatementExt.FinStmtTotalAktiva,
                    FinStmtAuditStatusCode = model.VendorFinStatementExt.FinStmtAuditStatusCode,
                    VendorExtId = guidvenregext,
                };
                if (model.VendorFinStatementExt.base64 != null)
                {
                    var guiddocext = Guid.NewGuid();
                    DocumentExt detaildoc = new DocumentExt()
                    {
                        Id = guiddocext,
                        VendorExtId = guidvenregext,
                        TipeDokumen = (int)EDocumentType.LaporanDataKeuangan
                    };
                    AddVendorDocumentExt(detaildoc);
                    DocumentImageExt findoc = new DocumentImageExt()
                    {
                        Id = Guid.NewGuid(),
                        Content = Convert.FromBase64String(model.VendorFinStatementExt.base64),
                        FileName = model.VendorFinStatementExt.FileName,
                        ContentType = model.VendorFinStatementExt.ContentType,
                        DocumenExtId = detaildoc.Id
                    };
                    AddVendorDocumentImageExt(findoc);
                    finStateExt.FinStmtDocId = findoc.Id;
                }
                AddVendorExtFinStatement(finStateExt);
            }

            //RegVendorExtEquipment
            if (model.VendorJobHistoryExt != null)
            {
                List<VendorExtJobHistory> ListJobhistory = new List<VendorExtJobHistory>();
                foreach (var item in model.VendorJobHistoryExt)
                {
                    if (item.JobTitle != null && item.JobTitle != "")
                    {
                        VendorExtJobHistory jobHistoryExt = new VendorExtJobHistory()
                        {
                            Id = Guid.NewGuid(),
                            JobTitle = item.JobTitle,
                            JobClient = item.JobClient,
                            JobLocation = item.JobLocation,
                            JobStartDate = item.JobStartDate,
                            JobContractNum = item.JobContractNum,
                            JobContractDate = item.JobContractDate,
                            JobContractAmount = item.JobContractAmount,
                            JobContractAmountCurrencyCode = item.JobContractAmountCurrencyCode,
                            //JobContractDocId = item.JobContractDocId,
                            JobType = item.JobType,
                            VendorExtId = guidvenregext

                        };
                        if (model.BuktiKerjasama.base64 != null)
                        {
                            //DocumentImageExt jobcontractdoc = new DocumentImageExt()
                            //{
                            //    Id = Guid.NewGuid(),
                            //    //VendorExtId = guidvenregext,
                            //    Content = Convert.FromBase64String(item.base64),
                            //    FileName = item.FileName,
                            //    ContentType = item.ContentType,
                            //};

                            var guiddocext = Guid.NewGuid();
                            DocumentExt detaildoc = new DocumentExt()
                            {
                                Id = guiddocext,
                                //Nomor = model.VendorRegExt.IndivGiidNo,
                                //TanggalBerakhir = model.VendorRegExt.IndivGiidValidUntil,
                                VendorExtId = guidvenregext,
                                TipeDokumen = (int)EDocumentType.BuktiKerjasama
                            };
                            AddVendorDocumentExt(detaildoc);
                            DocumentImageExt regVendorDocumentExtRekeningKoran = new DocumentImageExt()
                            {
                                Id = Guid.NewGuid(),
                                Content = Convert.FromBase64String(model.BuktiKerjasama.base64),
                                FileName = model.BuktiKerjasama.FileName,
                                ContentType = model.BuktiKerjasama.ContentType,
                                DocumenExtId = guiddocext
                            };
                            AddVendorDocumentImageExt(regVendorDocumentExtRekeningKoran);
                            jobHistoryExt.JobContractDocId = detaildoc.Id;
                        }
                        ListJobhistory.Add(jobHistoryExt);
                    }
                }
                if (ListJobhistory.Count != 0) AddVendorExtJobHistory(ListJobhistory);
            }

            //RegVendorExtJobHistory
            if (model.VendorEquipmentExt != null)
            {
                List<VendorExtEquipment> listEquipment = new List<VendorExtEquipment>();
                foreach (var item in model.VendorEquipmentExt)
                {
                    if (item.EquipmentName != null && item.EquipmentName != "")
                    {
                        VendorExtEquipment equipment = new VendorExtEquipment()
                        {
                            Id = Guid.NewGuid(),
                            EquipmentName = item.EquipmentName,
                            EquipmentQty = item.EquipmentQty,
                            EquipmentCapacity = item.EquipmentCapacity,
                            EquipmentMake = item.EquipmentMake,
                            EquipmentMakeYear = item.EquipmentMakeYear,
                            EquipmentConditionCode = item.EquipmentConditionCode,
                            EquipmentLocation = item.EquipmentLocation,
                            //EquipmentOwnershipDocId = item.EquipmentOwnershipDocId,
                            //EquipmentPicture = item.EquipmentPictureDocId,
                            VendorExtId = guidvenregext
                        };

                        if (model.BuktiKepemilikanPeralatan.base64 != null)
                        {
                            var guiddocext = Guid.NewGuid();
                            DocumentExt detaildoc = new DocumentExt()
                            {
                                Id = guiddocext,
                                //Nomor = model.VendorRegExt.IndivGiidNo,
                                //TanggalBerakhir = model.VendorRegExt.IndivGiidValidUntil,
                                VendorExtId = guidvenregext,
                                TipeDokumen = (int)EDocumentType.BuktiKepemilikanPeralatan
                            };
                            AddVendorDocumentExt(detaildoc);
                            DocumentImageExt ownershipDoc = new DocumentImageExt()
                            {
                                Id = Guid.NewGuid(),
                                //VendorExtId = guidvenregext,
                                Content = Convert.FromBase64String(model.BuktiKepemilikanPeralatan.base64),
                                FileName = model.BuktiKepemilikanPeralatan.FileName,
                                ContentType = model.BuktiKepemilikanPeralatan.ContentType,
                                DocumenExtId = detaildoc.Id
                            };
                            AddVendorDocumentImageExt(ownershipDoc);
                            equipment.EquipmentOwnershipDocId = ownershipDoc.Id;
                        }

                        if (model.FotoPeralatan != null)
                        {
                            var guiddocext = Guid.NewGuid();
                            DocumentExt detaildoc = new DocumentExt()
                            {
                                Id = guiddocext,
                                //Nomor = model.VendorRegExt.IndivGiidNo,
                                //TanggalBerakhir = model.VendorRegExt.IndivGiidValidUntil,
                                VendorExtId = guidvenregext,
                                TipeDokumen = (int)EDocumentType.FotoPeralatan
                            };
                            AddVendorDocumentExt(detaildoc);
                            DocumentImageExt ownershipDoc = new DocumentImageExt()
                            {
                                Id = Guid.NewGuid(),
                                //VendorExtId = guidvenregext,
                                Content = Convert.FromBase64String(model.FotoPeralatan.base64),
                                FileName = model.FotoPeralatan.FileName,
                                ContentType = model.FotoPeralatan.ContentType,
                                DocumenExtId = detaildoc.Id

                            };
                            AddVendorDocumentImageExt(ownershipDoc);
                            equipment.EquipmentPicture = ownershipDoc.Id;
                        }
                        listEquipment.Add(equipment);
                    }
                }
                if (listEquipment.Count != 0) AddVendorExtEquipment(listEquipment);
            }

            //get the root folder where file will be store
            //string root = HttpContext.Current.Server.MapPath("~/UploadFile");

            //var provider = new MultipartFormDataStreamProvider(root);
            //await Request.Content.ReadAsMultipartAsync(provider);

            if (model.LAPORANKEUANGAN != null && model.LAPORANKEUANGAN.Nomor != null)
            {
                var guiddocext = Guid.NewGuid();
                DocumentExt detaildoc = new DocumentExt()
                {
                    Id = guiddocext,
                    Nomor = model.LAPORANKEUANGAN.Nomor,
                    Penerbit = model.LAPORANKEUANGAN.Pembuat,
                    TanggalTerbit = model.LAPORANKEUANGAN.TanggalTerbit,
                    TanggalBerakhir = model.LAPORANKEUANGAN.TanggalBerakhir,
                    VendorExtId = guidvenregext,
                    TipeDokumen = (int)EDocumentType.LAPORANKEUANGAN
                };
                AddVendorDocumentExt(detaildoc);

                DocumentImageExt regVendorDocumentExtLaporanKeuangan = new DocumentImageExt()
                {
                    Id = Guid.NewGuid(),
                    Content = Convert.FromBase64String(model.LAPORANKEUANGAN.base64),
                    FileName = model.LAPORANKEUANGAN.FileName,
                    ContentType = model.LAPORANKEUANGAN.ContentType,
                    DocumenExtId = guiddocext
                };
                AddVendorDocumentImageExt(regVendorDocumentExtLaporanKeuangan);
            }

            if (model.REKENINGKORAN != null && model.REKENINGKORAN.Nomor != null)
            {
                var guiddocext = Guid.NewGuid();
                DocumentExt detaildoc = new DocumentExt()
                {
                    Id = guiddocext,
                    Nomor = model.REKENINGKORAN.Nomor,
                    Penerbit = model.REKENINGKORAN.Pembuat,
                    TanggalTerbit = model.REKENINGKORAN.TanggalTerbit,
                    TanggalBerakhir = model.REKENINGKORAN.TanggalBerakhir,
                    VendorExtId = guidvenregext,
                    TipeDokumen = (int)EDocumentType.REKENINGKORAN
                };
                AddVendorDocumentExt(detaildoc);
                DocumentImageExt regVendorDocumentExtRekeningKoran = new DocumentImageExt()
                {
                    Id = Guid.NewGuid(),
                    Content = Convert.FromBase64String(model.REKENINGKORAN.base64),
                    FileName = model.REKENINGKORAN.FileName,
                    ContentType = model.REKENINGKORAN.ContentType,
                    DocumenExtId = guiddocext
                };
                AddVendorDocumentImageExt(regVendorDocumentExtRekeningKoran);
            }

            if (model.DRT != null && model.DRT.Nomor != null)
            {
                var guiddocext = Guid.NewGuid();
                DocumentExt detaildoc = new DocumentExt()
                {
                    Id = guiddocext,
                    Nomor = model.DRT.Nomor,
                    Penerbit = model.DRT.Pembuat,
                    TanggalTerbit = model.DRT.TanggalTerbit,
                    TanggalBerakhir = model.DRT.TanggalBerakhir,
                    VendorExtId = guidvenregext,
                    TipeDokumen = (int)EDocumentType.DRT
                };
                AddVendorDocumentExt(detaildoc);
                DocumentImageExt regVendorDocumentExtDRT = new DocumentImageExt()
                {
                    Id = Guid.NewGuid(),
                    Content = Convert.FromBase64String(model.DRT.base64),
                    FileName = model.DRT.FileName,
                    ContentType = model.DRT.ContentType,
                    DocumenExtId = guiddocext
                };
                AddVendorDocumentImageExt(regVendorDocumentExtDRT);
            }

            if (model.AKTAPENDIRIAN != null && model.AKTAPENDIRIAN.Nomor != null)
            {
                var guiddocext = Guid.NewGuid();
                DocumentExt detaildoc = new DocumentExt()
                {
                    Id = guiddocext,
                    Nomor = model.AKTAPENDIRIAN.Nomor,
                    Penerbit = model.AKTAPENDIRIAN.Pembuat,
                    TanggalTerbit = model.AKTAPENDIRIAN.TanggalTerbit,
                    TanggalBerakhir = model.AKTAPENDIRIAN.TanggalBerakhir,
                    VendorExtId = guidvenregext,
                    TipeDokumen = (int)EDocumentType.AKTAPENDIRIAN
                };
                AddVendorDocumentExt(detaildoc);
                DocumentImageExt regVendorDocumentExtAktaPendirian = new DocumentImageExt()
                {
                    Id = Guid.NewGuid(),
                    Content = Convert.FromBase64String(model.AKTAPENDIRIAN.base64),
                    FileName = model.AKTAPENDIRIAN.FileName,
                    ContentType = model.AKTAPENDIRIAN.ContentType,
                    DocumenExtId = guiddocext
                };
                AddVendorDocumentImageExt(regVendorDocumentExtAktaPendirian);
            }

            if (model.SKKEMENKUMHAM != null && model.SKKEMENKUMHAM.Nomor != null)
            {
                var guiddocext = Guid.NewGuid();
                DocumentExt detaildoc = new DocumentExt()
                {
                    Id = guiddocext,
                    Nomor = model.SKKEMENKUMHAM.Nomor,
                    Penerbit = model.SKKEMENKUMHAM.Pembuat,
                    TanggalTerbit = model.SKKEMENKUMHAM.TanggalTerbit,
                    TanggalBerakhir = model.SKKEMENKUMHAM.TanggalBerakhir,
                    VendorExtId = guidvenregext,
                    TipeDokumen = (int)EDocumentType.SKKEMENKUMHAM
                };
                AddVendorDocumentExt(detaildoc);
                DocumentImageExt regVendorDocumentExtSKKemenkumham = new DocumentImageExt()
                {
                    Id = Guid.NewGuid(),
                    Content = Convert.FromBase64String(model.SKKEMENKUMHAM.base64),
                    FileName = model.SKKEMENKUMHAM.FileName,
                    ContentType = model.SKKEMENKUMHAM.ContentType,
                    DocumenExtId = guiddocext
                };
                AddVendorDocumentImageExt(regVendorDocumentExtSKKemenkumham);
            }

            if (model.BERITANEGARA != null && model.BERITANEGARA.Nomor != null)
            {
                var guiddocext = Guid.NewGuid();
                DocumentExt detaildoc = new DocumentExt()
                {
                    Id = guiddocext,
                    Nomor = model.BERITANEGARA.Nomor,
                    Penerbit = model.BERITANEGARA.Pembuat,
                    TanggalTerbit = model.BERITANEGARA.TanggalTerbit,
                    TanggalBerakhir = model.BERITANEGARA.TanggalBerakhir,
                    VendorExtId = guidvenregext,
                    TipeDokumen = (int)EDocumentType.BERITANEGARA
                };
                AddVendorDocumentExt(detaildoc);
                DocumentImageExt regVendorDocumentExtBeritaNegara = new DocumentImageExt()
                {
                    Id = Guid.NewGuid(),
                    Content = Convert.FromBase64String(model.BERITANEGARA.base64),
                    FileName = model.BERITANEGARA.FileName,
                    ContentType = model.BERITANEGARA.ContentType,
                    DocumenExtId = guiddocext
                };
                AddVendorDocumentImageExt(regVendorDocumentExtBeritaNegara);
            }

            if (model.AKTAPERUBAHAN != null && model.AKTAPERUBAHAN.Nomor != null)
            {
                var guiddocext = Guid.NewGuid();
                DocumentExt detaildoc = new DocumentExt()
                {
                    Id = guiddocext,
                    Nomor = model.AKTAPERUBAHAN.Nomor,
                    Penerbit = model.AKTAPERUBAHAN.Pembuat,
                    TanggalTerbit = model.AKTAPERUBAHAN.TanggalTerbit,
                    TanggalBerakhir = model.AKTAPERUBAHAN.TanggalBerakhir,
                    VendorExtId = guidvenregext,
                    TipeDokumen = (int)EDocumentType.AKTANEGARA
                };
                AddVendorDocumentExt(detaildoc);
                DocumentImageExt regVendorDocumentExtAktaNegara = new DocumentImageExt()
                {
                    Id = Guid.NewGuid(),
                    Content = Convert.FromBase64String(model.AKTAPERUBAHAN.base64),
                    FileName = model.AKTAPERUBAHAN.FileName,
                    ContentType = model.AKTAPERUBAHAN.ContentType,
                    DocumenExtId = guiddocext
                };
                AddVendorDocumentImageExt(regVendorDocumentExtAktaNegara);
            }

            if (model.NPWP != null && model.NPWP.Nomor != null)
            {
                var guiddocext = Guid.NewGuid();
                DocumentExt detaildoc = new DocumentExt()
                {
                    Id = guiddocext,
                    Nomor = model.NPWP.Nomor,
                    VendorExtId = guidvenregext,
                    TipeDokumen = (int)EDocumentType.NPWP
                };
                AddVendorDocumentExt(detaildoc);
                DocumentImageExt npwpdoc = new DocumentImageExt()
                {
                    Id = Guid.NewGuid(),
                    Content = Convert.FromBase64String(model.NPWP.base64),
                    FileName = model.NPWP.FileName,
                    ContentType = model.NPWP.ContentType,
                    DocumenExtId = guiddocext
                };
                AddVendorDocumentImageExt(npwpdoc);
            }

            if (model.PKP != null && model.PKP.Nomor != null)
            {
                var guiddocext = Guid.NewGuid();
                DocumentExt detaildoc = new DocumentExt()
                {
                    //Id = guiddocext,
                    Id = pkpGuid, //use guid from pkpGuid
                    Nomor = model.PKP.Nomor,
                    VendorExtId = guidvenregext,
                    TipeDokumen = (int)EDocumentType.PKP,
                };
                AddVendorDocumentExt(detaildoc);
                if (model.PKP.base64 != null)
                {
                    DocumentImageExt pkpdoc = new DocumentImageExt()
                    {
                        Id = Guid.NewGuid(),
                        Content = Convert.FromBase64String(model.PKP.base64),
                        FileName = model.PKP.FileName,
                        ContentType = model.PKP.ContentType,
                        DocumenExtId = guiddocext
                    };
                    AddVendorDocumentImageExt(pkpdoc);
                }
            }

            if (model.PROFILPERUSAHAAN != null && model.PROFILPERUSAHAAN.ContentType != null)
            {
                var guiddocext = Guid.NewGuid();
                DocumentExt detaildoc = new DocumentExt()
                {
                    //Id = guiddocext,
                    Id = guidCompanyProfile,
                    Nomor = model.PROFILPERUSAHAAN.Nomor,
                    Penerbit = model.PROFILPERUSAHAAN.Pembuat,
                    TanggalTerbit = model.PROFILPERUSAHAAN.TanggalTerbit,
                    TanggalBerakhir = model.PROFILPERUSAHAAN.TanggalBerakhir,
                    VendorExtId = guidvenregext,
                    TipeDokumen = (int)EDocumentType.PROFILPERUSAHAAN
                };
                AddVendorDocumentExt(detaildoc);

                DocumentImageExt regVendorDocumentExtProfilPerusahaan = new DocumentImageExt()
                {
                    Id = Guid.NewGuid(),
                    Content = Convert.FromBase64String(model.PROFILPERUSAHAAN.base64),
                    FileName = model.PROFILPERUSAHAAN.FileName,
                    ContentType = model.PROFILPERUSAHAAN.ContentType,
                    DocumenExtId = guidCompanyProfile
                };
                AddVendorDocumentImageExt(regVendorDocumentExtProfilPerusahaan);
            }

            if (model.SIUP != null && model.SIUP.Nomor != null)
            {
                var guiddocext = Guid.NewGuid();
                DocumentExt detaildoc = new DocumentExt()
                {
                    Id = guiddocext,
                    Nomor = model.SIUP.Nomor,
                    Penerbit = model.SIUP.Pembuat,
                    TanggalTerbit = model.SIUP.TanggalTerbit,
                    TanggalBerakhir = model.SIUP.TanggalBerakhir,
                    VendorExtId = guidvenregext,
                    TipeDokumen = (int)EDocumentType.SIUP
                };
                AddVendorDocumentExt(detaildoc);

                DocumentImageExt regVendorDocumentExtSIUP = new DocumentImageExt()
                {
                    Id = Guid.NewGuid(),
                    Content = Convert.FromBase64String(model.SIUP.base64),
                    FileName = model.SIUP.FileName,
                    ContentType = model.SIUP.ContentType,
                    DocumenExtId = guiddocext
                };
                AddVendorDocumentImageExt(regVendorDocumentExtSIUP);
            }

            if (model.SIUJK != null && model.SIUJK.Nomor != null)
            {
                var guiddocext = Guid.NewGuid();
                DocumentExt detaildoc = new DocumentExt()
                {
                    Id = guiddocext,
                    Nomor = model.SIUJK.Nomor,
                    Penerbit = model.SIUJK.Pembuat,
                    TanggalTerbit = model.SIUJK.TanggalTerbit,
                    TanggalBerakhir = model.SIUJK.TanggalBerakhir,
                    VendorExtId = guidvenregext,
                    TipeDokumen = (int)EDocumentType.SIUJK
                };
                AddVendorDocumentExt(detaildoc);

                DocumentImageExt regVendorDocumentExtSIUJK = new DocumentImageExt()
                {
                    Id = Guid.NewGuid(),
                    Content = Convert.FromBase64String(model.SIUJK.base64),
                    FileName = model.SIUJK.FileName,
                    ContentType = model.SIUJK.ContentType,
                    DocumenExtId = guiddocext
                };
                AddVendorDocumentImageExt(regVendorDocumentExtSIUJK);
            }

            if (model.NIB != null && model.NIB.Nomor != null)
            {
                var guiddocext = Guid.NewGuid();
                DocumentExt detaildoc = new DocumentExt()
                {
                    Id = guiddocext,
                    Nomor = model.NIB.Nomor,
                    Penerbit = model.NIB.Pembuat,
                    TanggalTerbit = model.NIB.TanggalTerbit,
                    TanggalBerakhir = model.NIB.TanggalBerakhir,
                    VendorExtId = guidvenregext,
                    TipeDokumen = (int)EDocumentType.NIB
                };
                AddVendorDocumentExt(detaildoc);

                DocumentImageExt regVendorDocumentExtNIB = new DocumentImageExt()
                {
                    Id = Guid.NewGuid(),
                    Content = Convert.FromBase64String(model.NIB.base64),
                    FileName = model.NIB.FileName,
                    ContentType = model.NIB.ContentType,
                    DocumenExtId = guiddocext
                };
                AddVendorDocumentImageExt(regVendorDocumentExtNIB);
            }

            if (model.SERTIFIKAT != null && model.SERTIFIKAT.Nomor != null)
            {
                var guiddocext = Guid.NewGuid();
                DocumentExt detaildoc = new DocumentExt()
                {
                    Id = guiddocext,
                    Nomor = model.SERTIFIKAT.Nomor,
                    Penerbit = model.SERTIFIKAT.Pembuat,
                    TanggalTerbit = model.SERTIFIKAT.TanggalTerbit,
                    TanggalBerakhir = model.SERTIFIKAT.TanggalBerakhir,
                    VendorExtId = guidvenregext,
                    TipeDokumen = (int)EDocumentType.SERTIFIKAT
                };
                AddVendorDocumentExt(detaildoc);

                DocumentImageExt regVendorDocumentExtSertifikat = new DocumentImageExt()
                {
                    Id = Guid.NewGuid(),
                    Content = Convert.FromBase64String(model.SERTIFIKAT.base64),
                    FileName = model.SERTIFIKAT.FileName,
                    ContentType = model.SERTIFIKAT.ContentType,
                    DocumenExtId = guiddocext
                };
                AddVendorDocumentImageExt(regVendorDocumentExtSertifikat);
            }

            if (model.TDP != null && model.TDP.Nomor != null)
            {
                var guiddocext = Guid.NewGuid();
                DocumentExt detaildoc = new DocumentExt()
                {
                    Id = guiddocext,
                    Nomor = model.TDP.Nomor,
                    Penerbit = model.TDP.Pembuat,
                    TanggalTerbit = model.TDP.TanggalTerbit,
                    TanggalBerakhir = model.TDP.TanggalBerakhir,
                    VendorExtId = guidvenregext,
                    TipeDokumen = (int)EDocumentType.TDP
                };
                AddVendorDocumentExt(detaildoc);

                DocumentImageExt regVendorDocumentExtTDP = new DocumentImageExt()
                {
                    Id = Guid.NewGuid(),
                    Content = Convert.FromBase64String(model.TDP.base64),
                    FileName = model.TDP.FileName,
                    ContentType = model.TDP.ContentType,
                    DocumenExtId = guiddocext
                };
                AddVendorDocumentImageExt(regVendorDocumentExtTDP);
            }

            if (model.DOMISILI != null && model.DOMISILI.ContentType != null)
            {
                var guiddocext = Guid.NewGuid();
                DocumentExt detaildoc = new DocumentExt()
                {
                    Id = guiddocext,
                    Nomor = model.DOMISILI.Nomor,
                    Penerbit = model.DOMISILI.Pembuat,
                    TanggalTerbit = model.DOMISILI.TanggalTerbit,
                    TanggalBerakhir = model.DOMISILI.TanggalBerakhir,
                    VendorExtId = guidvenregext,
                    TipeDokumen = (int)EDocumentType.DOMISILI
                };
                AddVendorDocumentExt(detaildoc);

                DocumentImageExt regVendorDocumentExtDomisili = new DocumentImageExt()
                {
                    Id = Guid.NewGuid(),
                    Content = Convert.FromBase64String(model.DOMISILI.base64),
                    FileName = model.DOMISILI.FileName,
                    ContentType = model.DOMISILI.ContentType,
                    DocumenExtId = guiddocext
                };
                AddVendorDocumentImageExt(regVendorDocumentExtDomisili);
            }

            Save();

            return v.Id.ToString();
        }

        public Guid AddVendorDocumentExt(DocumentExt v)
        {
            var datadoc = new DocumentExt();
            datadoc.Id = v.Id;
            datadoc.Nomor = v.Nomor;
            datadoc.Penerbit = v.Penerbit;
            datadoc.TanggalTerbit = v.TanggalTerbit;
            datadoc.TanggalBerakhir = v.TanggalBerakhir;
            datadoc.TipeDokumen = v.TipeDokumen;
            datadoc.VendorExtId = v.VendorExtId;
            datadoc.Active = true;
            ctx.DocumentExts.Add(datadoc);
            ctx.SaveChanges();
            return v.Id;
        }

        public Guid AddVendorDocumentImageExt(DocumentImageExt v)
        {
            var imagedoc = new DocumentImageExt();
            imagedoc.Id = v.Id;
            imagedoc.ContentType = v.ContentType;
            imagedoc.FileName = v.FileName;
            imagedoc.Content = v.Content;
            imagedoc.DocumenExtId = v.DocumenExtId;
            ctx.DocumentImageExts.Add(imagedoc);

            //ctx.DocumentImageExts.Add(v);

            ctx.SaveChanges();
            return v.Id;
        }

        public Guid AddVendorExt(VendorExt v)
        {
            ctx.VendorExts.Add(v);
            ctx.SaveChanges();
            return v.Id;
        }

        public Guid AddVendorExtPerson(List<VendorExtPerson> v)
        {
            foreach (var item in v)
            {
                ctx.VendorExtPersons.Add(item);
                ctx.SaveChanges();
            }
            return v[0].VendorExtId;
        }

        public Guid AddVendorExtBankInfo(VendorExtBankInfo v)
        {
            ctx.VendorExtBankInfoes.Add(v);
            ctx.SaveChanges();
            return v.Id;
        }

        public Guid AddVendorExtHumanResource(List<VendorExtHumanResource> v)
        {
            foreach (var item in v)
            {
                ctx.VendorExtHumanResources.Add(item);
                ctx.SaveChanges();
            }

            return v[0].VendorExtId;
        }

        public Guid AddVendorExtFinStatement(VendorExtFinStatement v)
        {
            ctx.VendorExtFinStatements.Add(v);
            ctx.SaveChanges();
            return v.Id;
        }

        public Guid AddVendorExtJobHistory(List<VendorExtJobHistory> v)
        {
            foreach (var item in v)
            {
                ctx.VendorExtJobHistories.Add(item);
                ctx.SaveChanges();
            }
            return v[0].VendorExtId;
        }

        public Guid AddVendorExtEquipment(List<VendorExtEquipment> v)
        {
            foreach (var item in v)
            {
                ctx.VendorExtEquipments.Add(item);
                ctx.SaveChanges();

            }
            return v[0].VendorExtId;
        }

        public void Save()
        {
            ctx.SaveChanges();
        }
    }

    public class DataOwnershipException : Exception
    {
        public DataOwnershipException() : base("The current user does not own this data.")
        {
        }

        public DataOwnershipException(string message) : base(message)
        {
        }

        public DataOwnershipException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
