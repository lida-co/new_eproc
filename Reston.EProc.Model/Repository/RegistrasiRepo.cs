using Reston.Pinata.Model.JimbisModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reston.Pinata.Model.Repository
{
    public class RegistrasiRepo : IRegistrasiRepo
    {
        AppDbContext ctx;
        public RegistrasiRepo(AppDbContext j)
        {
            ctx = j;
            ctx.Configuration.LazyLoadingEnabled = true;
        }

        public RegVendor GetVendor(int id){
            return ctx.RegVendors.Find(id);
        }

        public RegVendor GetVendor(string noPengajuan)
        {
            RegVendor rv = ctx.RegVendors.Where(x => x.NoPengajuan == noPengajuan).FirstOrDefault();
            if (rv != null) return rv;
            return null;
        }

        public List<RegVendor> GetVendors(ETipeVendor tipe, EStatusVendor status, int limit,string search) {
            if (limit > 0) {
                var lv =
                ctx.RegVendors.Where(x => (tipe == ETipeVendor.NONE || x.TipeVendor == tipe) &&
                    (status == EStatusVendor.NONE || x.StatusAkhir == status));
                if (!string.IsNullOrEmpty(search)) lv = lv.Where(d => d.Nama.Contains(search)).OrderByDescending(x => x.Id).Take(limit);                    
                return lv.ToList();
            }
            return new List<RegVendor>();
        }

        public List<RegVendor> GetAllVendor()
        {
            return ctx.RegVendors.ToList();
        }

        public void Save()
        {
            ctx.SaveChanges();
        }

        public int AddVendor(RegVendor v) {
            ctx.RegVendors.Add(v);
            ctx.SaveChanges();
            return v.Id;
        }

        public RegDokumen GetDokumen(Guid id)
        {
            return ctx.RegDokumens.Find(id);
        }

        public IQueryable<RegDokumen> GetAllDokumenByVendor(int vendorid)
        {
            RegVendor v = ctx.RegVendors.Find(vendorid);
            if (v != null)
                return v.RegDokumen.AsQueryable();
            return null;
        }

        public Guid AddDokumen(RegDokumen d)
        {
            ctx.RegDokumens.Add(d);
            ctx.SaveChanges();
            return d.Id;
        }

        public int RegisterVerifiedVendor(Vendor v) {
            try
            {
                ctx.Vendors.Add(v);
                ctx.SaveChanges();
                return v.Id;
            }
            catch (Exception e) {
                
            }
            return 0;
        }

        public int CheckNomor(string no)
        {
            try
            {
                RegVendor v = ctx.RegVendors.Where(x => x.NoPengajuan.StartsWith(no)).OrderByDescending(x => x.NoPengajuan).FirstOrDefault();
                if (v != null)
                {
                    return Int32.Parse(v.NoPengajuan.Substring(6, 4)) + 1;
                }
            }
            catch (Exception e)
            {
                return 1;
            }
            return 1;
        }

        public CaptchaRegistration GetCaptchaRegistration(Guid id) {
            return ctx.CaptchaRegistration.Find(id);
        }

        public void AddCaptchaRegistration(CaptchaRegistration c) {
            //delete expired
            var expired = ctx.CaptchaRegistration.Where(x => x.ExpiredDate < DateTime.Now);
            ctx.CaptchaRegistration.RemoveRange(expired);

            ctx.CaptchaRegistration.Add(c);
            ctx.SaveChanges();
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
                    IndivContactEmail = regVendorExt.IndivContactEmail,
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
                    IsPKP = regVendorExt.IsPKP,

                    DirPersonGiidNo = regVendorExt.DirPersonGiidNo,
                    DirPersonName = regVendorExt.DirPersonName,
                    DirPersonPosition = regVendorExt.DirPersonPosition,
                    DirPersonReligionCode = regVendorExt.DirPersonReligionCode,
                    DirPersonBirthDay = regVendorExt.DirPersonBirthDay
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
    }
}
