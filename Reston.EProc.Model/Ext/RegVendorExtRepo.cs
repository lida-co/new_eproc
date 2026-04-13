using Reston.Pinata.Model.JimbisModel;
using System;
using System.Collections.Generic;
using Reston.Pinata.Model.PengadaanRepository.View;
using Reston.Pinata.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using System.Data;
using System.Data.Entity;

namespace Reston.Eproc.Model.Ext
{
    public interface IRegVendorExtRepo : ITransactionSupportedRepository
    {
        int AddVendorOri(RegVendor v);
        Guid AddVendorExt(RegVendorExt v);
        Guid AddVendorExtBankInfo(RegVendorExtBankInfo v); 
        Guid AddVendorExtEquipment(List<RegVendorExtEquipment> v);
        Guid AddVendorExtFinStatement(RegVendorExtFinStatement v);
        Guid AddVendorExtHumanResource(List<RegVendorExtHumanResource> v);
        Guid AddVendorExtJobHistory(List<RegVendorExtJobHistory> v);
        Guid AddVendorExtPerson(List<RegVendorExtPerson> v);

        Guid AddVendorDocumentImageExt(RegDocumentImageExt v);
        Guid AddVendorDocumentExt(RegDocumentExt v);

        int CheckNomor(string no);
        void Save();
        
        List<ReferenceData> GetData(string qualifier);
        List<ReferenceData> GetListDukcapil(string qualifier, string attr1);
        List<ReferenceData> GetListSegmentasi(string qualifier, string attr1);
        List<ReferenceData> GetListSegmentasiSubBidangUsaha(string qualifier, string attr1); 
        List<ReferenceData> GetDataSegmentasiSubBidangUsahaCode(string qualifier, string dataarr);
        List<ReferenceData> GetkodeposCode(string qualifier, string provcode, string citycode, string districtcode, string subdistrictcode);

        RegVendor GetRegVendor(string noPengajuan);
        RegVendorExt GetRegVendorExt(string noPengajuan);
        RegRiwayatPengajuanVendor GetRegRiwayatPengajuanVendor(int vendorId);

        int RegisterVerifiedVendor(Vendor v);
        int RegisterVerifiedVendorEXT(string v);
    }

    public class RegVendorExtRepo : IRegVendorExtRepo
    {
        AppDbContext ctx;
        public RegVendorExtRepo(AppDbContext j)
        {
            ctx = j;
            ctx.Configuration.LazyLoadingEnabled = true;
        }

        public Guid AddVendorExt(RegVendorExt v)
        {
            ctx.RegVendorExts.Add(v);
            ctx.SaveChanges();
            return v.Id;
            //if (v.Id != Guid.Empty && v.Id != null)
            //{
            //    RegVendorExt vEdit = ctx.RegVendorExts.Find(v.Id);

            //    vEdit.RegVendorId = v.RegVendorId;
            //    vEdit.JenisVendor = v.JenisVendor;
            //    vEdit.KategoriVendor = v.KategoriVendor;
            //    vEdit.BentukBadanUsaha = v.BentukBadanUsaha;
            //    vEdit.StatusPerusahaan = v.StatusPerusahaan;
            //    vEdit.EstablishedDate = v.EstablishedDate; 
            //    vEdit.CountryCode = v.CountryCode;
            //    vEdit.FirstLevelDivisionCode = v.FirstLevelDivisionCode;
            //    vEdit.SecondLevelDivisionCode = v.SecondLevelDivisionCode;
            //    vEdit.ThirdLevelDivisionCode = v.ThirdLevelDivisionCode;
            //    vEdit.PostalCode = v.PostalCode;
            //    vEdit.Fax = v.Fax;
            //    vEdit.WorkUnitCode = v.WorkUnitCode;
            //    vEdit.SegBidangUsahaCode = v.SegBidangUsahaCode;
            //    vEdit.SegKelompokUsahaCode = v.SegKelompokUsahaCode;
            //    vEdit.SegSubBidangUsahaCode = v.SegSubBidangUsahaCode;
            //    vEdit.SegKualifikasiGrade = v.SegKualifikasiGrade;

            //    vEdit.IndivName = v.IndivName;
            //    vEdit.IndivAbbrevName = v.IndivAbbrevName;
            //    vEdit.IndivGiidNo = v.IndivGiidNo;
            //    vEdit.IndivGiidValidUntil = v.IndivGiidValidUntil;
            //    vEdit.IndivAddress = v.IndivAddress;
            //    vEdit.IndivCountryCode = v.IndivCountryCode;
            //    vEdit.IndivFirstLevelDivisionCode = v.IndivFirstLevelDivisionCode;
            //    vEdit.IndivSecondLevelDivisionCode = v.IndivSecondLevelDivisionCode;
            //    vEdit.IndivThirdLevelDivisionCode = v.IndivThirdLevelDivisionCode;
            //    vEdit.IndivPostalCode = v.IndivPostalCode;
            //    vEdit.IndivContactPersonName = v.IndivContactPersonName;
            //    vEdit.IndivContactPhoneNum = v.IndivContactPhoneNum;

            //    vEdit.PrinRepOfficeAddress = v.PrinRepOfficeAddress;
            //    vEdit.PrinRepOfficeContactPhoneNum = v.PrinRepOfficeContactPhoneNum;
            //    vEdit.PrinRepOfficeFaxNum = v.PrinRepOfficeFaxNum;
            //    vEdit.PrinRepOfficeEmail = v.PrinRepOfficeEmail;
            //    vEdit.PrinWebsite = v.PrinWebsite;
            //    //ctx.Vendors.Add(vEdit);
            //    ctx.SaveChanges();
            //    return vEdit.Id;
            //}
            //else
            //{
            //    ctx.RegVendorExts.Add(v);
            //    ctx.SaveChanges();
            //    return v.Id;
            //}
        }

        public int AddVendorOri(RegVendor v)
        {
            ctx.RegVendors.Add(v);
            ctx.SaveChanges();
            return v.Id;
        }

        public Guid AddVendorExtBankInfo(RegVendorExtBankInfo v)
        {
            ctx.RegVendorExtBankInfoes.Add(v);
            ctx.SaveChanges();
            return v.Id;

            //if (v.Id != Guid.Empty && v.Id != null)
            //{
            //    RegVendorExtBankInfo vEdit = ctx.RegVendorExtBankInfoes.Find(v.Id);

            //    vEdit.RegVendorExtId = v.RegVendorExtId;
            //    vEdit.BankCode = v.BankCode;
            //    vEdit.BankAddress = v.BankAddress;
            //    vEdit.BankCity = v.BankCity;
            //    vEdit.Branch = v.Branch;
            //    vEdit.AccNumber = v.AccNumber;
            //    vEdit.AccName = v.AccName;
            //    vEdit.AccCurrencyCode = v.AccCurrencyCode;

            //    //ctx.Vendors.Add(vEdit);
            //    ctx.SaveChanges();
            //    return vEdit.Id;
            //}
            //else
            //{
            //    ctx.RegVendorExtBankInfoes.Add(v);
            //    ctx.SaveChanges();
            //    return v.Id;
            //}
        }

        public Guid AddVendorExtEquipment(List<RegVendorExtEquipment> v)
        {
            foreach (var item in v)
            {
                ctx.RegVendorExtEquipments.Add(item);
                ctx.SaveChanges();
                
            }
            return v[0].RegVendorExtId;

            //if (v.Id != Guid.Empty && v.Id != null)
            //{
            //    RegVendorExtEquipment vEdit = ctx.RegVendorExtEquipments.Find(v.Id);

            //    vEdit.RegVendorExtId = v.RegVendorExtId;
            //    vEdit.EquipmentName = v.EquipmentName;
            //    vEdit.EquipmentQty = v.EquipmentQty;
            //    vEdit.EquipmentCapacity = v.EquipmentCapacity;
            //    vEdit.EquipmentMake = v.EquipmentMake;
            //    vEdit.EquipmentMakeYear = v.EquipmentMakeYear;
            //    vEdit.EquipmentConditionCode = v.EquipmentConditionCode;
            //    vEdit.EquipmentLocation = v.EquipmentLocation;
            //    vEdit.EquipmentOwnershipDocId = v.EquipmentOwnershipDocId;
            //    vEdit.EquipmentPictureDocId = v.EquipmentPictureDocId;

            //    //ctx.Vendors.Add(vEdit);
            //    ctx.SaveChanges();
            //    return vEdit.Id;
            //}
            //else
            //{
            //    ctx.RegVendorExtEquipments.Add(v);
            //    ctx.SaveChanges();
            //    return v.Id;
            //}
        }

        public Guid AddVendorExtFinStatement(RegVendorExtFinStatement v)
        {
            ctx.RegVendorExtFinStatements.Add(v);
            ctx.SaveChanges();
            return v.Id;

            //if (v.Id != Guid.Empty && v.Id != null)
            //{
            //    RegVendorExtFinStatement vEdit = ctx.RegVendorExtFinStatements.Find(v.Id);

            //    vEdit.RegVendorExtId = v.RegVendorExtId;
            //    vEdit.FinStmtDocNumber = v.FinStmtDocNumber;
            //    vEdit.FinStmtIssuer = v.FinStmtIssuer;
            //    vEdit.FinStmtIssueDate  = v.FinStmtIssueDate;
            //    vEdit.FinStmtValidThruDate = v.FinStmtValidThruDate;
            //    vEdit.FinStmtDocumentId = v.FinStmtDocumentId;
            //    vEdit.FinStmtYear  = v.FinStmtYear;
            //    vEdit.FinStmtCurrencyCode = v.FinStmtCurrencyCode;
            //    vEdit.FinStmtAktivaLancar = v.FinStmtAktivaLancar;
            //    vEdit.FinStmtHutangLancar = v.FinStmtHutangLancar;
            //    vEdit.FinStmtRasioLikuiditas = v.FinStmtRasioLikuiditas;
            //    vEdit.FinStmtTotalHutang  = v.FinStmtTotalHutang;
            //    vEdit.FinStmtEkuitas  = v.FinStmtEkuitas;
            //    vEdit.FinStmtDebtToEquityRatio = v.FinStmtDebtToEquityRatio;
            //    vEdit.FinStmtNetProfitLoss = v.FinStmtNetProfitLoss;
            //    vEdit.FinStmtReturnOfEquity = v.FinStmtReturnOfEquity;
            //    vEdit.FinStmtKas = v.FinStmtKas;
            //    vEdit.FinStmtTotalAktiva = v.FinStmtTotalAktiva;
            //    vEdit.FinStmtAuditStatusCode = v.FinStmtAuditStatusCode;
            //    //ctx.Vendors.Add(vEdit);
            //    ctx.SaveChanges();
            //    return vEdit.Id;
            //}
            //else
            //{
            //    ctx.RegVendorExtFinStatements.Add(v);
            //    ctx.SaveChanges();
            //    return v.Id;
            //}
        }

        public Guid AddVendorExtHumanResource(List<RegVendorExtHumanResource> v)
        {
            foreach (var item in v)
            {
                ctx.RegVendorExtHumanResources.Add(item);
                ctx.SaveChanges();
            }
            
            return v[0].RegVendorExtId;

            //if (v.Id != Guid.Empty && v.Id != null)
            //{
            //    RegVendorExtHumanResource vEdit = ctx.RegVendorExtHumanResources.Find(v.Id);

            //    vEdit.RegVendorExtId = v.RegVendorExtId;
            //    vEdit.ResourceFullName = v.ResourceFullName;
            //    vEdit.ResourceDateOfBirth = v.ResourceDateOfBirth;
            //    vEdit.ResourceExperienceCode = v.ResourceExperienceCode;
            //    vEdit.ResourceExpertise = v.ResourceExpertise;
            //    vEdit.ResourceCVDocId = v.ResourceCVDocId;
            //    vEdit.ResourceLastEduCode = v.ResourceLastEduCode;
            //    vEdit.ResourceLastEduDocId = v.ResourceLastEduDocId;
            //    vEdit.ResourceLastEduIssuer = v.ResourceLastEduIssuer;
            //    vEdit.ResourceCertificationDocId = v.ResourceCertificationDocId;
            //    vEdit.ResourceCertificationIssuer = v.ResourceCertificationIssuer;
            //    //ctx.Vendors.Add(vEdit);
            //    ctx.SaveChanges();
            //    return vEdit.Id;
            //}
            //else
            //{
            //    ctx.RegVendorExtHumanResources.Add(v);
            //    ctx.SaveChanges();
            //    return v.Id;
            //}
        }

        public Guid AddVendorExtJobHistory(List<RegVendorExtJobHistory> v)
        {
            foreach (var item in v)
            {
                ctx.RegVendorExtJobHistories.Add(item);
                ctx.SaveChanges();
            }
            return v[0].RegVendorExtId;
        }

        public Guid AddVendorExtPerson(List<RegVendorExtPerson> v)
        {
            foreach(var item in v)
            {
                ctx.RegVendorExtPersons.Add(item);
                ctx.SaveChanges();
            }
            return v[0].RegVendorExtId;
        }

        public Guid AddVendorDocumentExt(RegDocumentExt v)
        {
            var datadoc = new RegDocumentExt();
            datadoc.Id = v.Id;
            datadoc.Nomor = v.Nomor;
            datadoc.Penerbit = v.Penerbit;
            if (v.TanggalTerbit.HasValue) datadoc.TanggalTerbit = v.TanggalTerbit;
            if (v.TanggalBerakhir.HasValue) datadoc.TanggalBerakhir = v.TanggalBerakhir;
            datadoc.TipeDokumen = v.TipeDokumen;
            datadoc.RegVendorExtId = v.RegVendorExtId;
            datadoc.Active = true;
            ctx.RegDocumentExts.Add(datadoc);
            ctx.SaveChanges();
            return v.Id;
        }

        public Guid AddVendorDocumentImageExt(RegDocumentImageExt v)
        {
            var imagedoc = new RegDocumentImageExt();
            imagedoc.Id = v.Id;
            imagedoc.ContentType = v.ContentType;
            imagedoc.FileName = v.FileName;
            imagedoc.Content = v.Content;
            imagedoc.RegDocumenExtId = v.RegDocumenExtId;
            ctx.RegDocumentImageExts.Add(imagedoc);

            //ctx.RegDocumentImageExts.Add(v);

            ctx.SaveChanges();
            return v.Id;
        }

        public int CheckNomor(string no)
        {
            try
            {
                var v = ctx.RegVendors.Where(x => x.NoPengajuan.StartsWith(no)).OrderByDescending(x => x.NoPengajuan).FirstOrDefault();
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

        public void Save()
        {
            ctx.SaveChanges();
        }

        public List<ReferenceData> GetData(string qualifier)
        {
            //return ctx.ReferenceDatas.Where(x => x.Qualifier.Contains(qualifier)).OrderBy(d => d.LocalizedName).ToList();
            try
            {
                //weird bug, cannot use single return code, 
                var newdata = ctx.ReferenceDatas.Where(x => x.Qualifier.Contains(qualifier)).ToList();
                newdata = newdata.OrderBy(d => d.LocalizedName).ToList();
                return newdata;
            }
            catch (Exception ex)
            {
                return new List<ReferenceData>();
            }
        }

        public List<ReferenceData> GetListDukcapil(string qualifier, string attr1)
        {
            //return ctx.ReferenceDatas.Where(x => x.Qualifier.Contains(qualifier) && x.StringAttr1 == attr1).OrderBy(d => d.LocalizedName).ToList();
            try
            {
                //weird bug, cannot use single return code, 
                var newdata = ctx.ReferenceDatas.Where(x => x.Qualifier.Contains(qualifier) && x.StringAttr1 == attr1).ToList();
                newdata = newdata.OrderBy(d => d.LocalizedName).ToList();
                return newdata;

                //var List1 = new List<String>() { qualifier, attr1 };
                //var reducedList = (from item in ctx.ReferenceDatas
                //                   where List1.Contains(item.Qualifier) && List1.Contains(item.StringAttr1)
                //                   select item).OrderBy(d => d.LocalizedName).ToList();
                //return reducedList;
            }
            catch (Exception ex)
            {
                return new List<ReferenceData>();
            }
        }

        public List<ReferenceData> GetListSegmentasi(string qualifier, string attr1)
        {
            try
            {
                //weird bug, cannot use single return code, 
                var newdata = ctx.ReferenceDatas.Where(x => x.Qualifier.Contains(qualifier) && x.StringAttr1 == attr1).ToList();
                newdata = newdata.OrderBy(d => d.LocalizedName).ToList();  
                return newdata;
            }
            catch (Exception ex)
            {
                return new List<ReferenceData>();
            }
            
        }

        public List<ReferenceData> GetListSegmentasiSubBidangUsaha(string qualifier, string attr1)
        {
            return ctx.ReferenceDatas.Where(x => x.Qualifier.Contains(qualifier) && x.StringAttr1 == attr1).ToList();
        }

        public List<ReferenceData> GetDataSegmentasiSubBidangUsahaCode(string qualifier, string dataarr)
        {
            var data = new List<ReferenceData>();
            var nParse = dataarr.Split(',');
            foreach(var n in nParse)
            {
                var newRefData = new List<ReferenceData>();
                newRefData = ctx.ReferenceDatas.Where(x => x.Qualifier.Contains(qualifier) && x.Code == n).ToList();
                data.AddRange(newRefData);
            }
            return data;
        }
        
        public List<ReferenceData> GetkodeposCode(string qualifier, string provcode, string citycode, string districtcode, string subdistrictcode)
        {
            var provcodeInt = int.Parse(provcode); //4th foreign key need to be int
            var newRefData = ctx.ReferenceDatas.Where(x => x.Qualifier.Contains(qualifier) 
                                                    && x.StringAttr1 == subdistrictcode 
                                                    && x.StringAttr2 == districtcode
                                                    && x.StringAttr3 == citycode
                                                    && x.IntAttr1 == provcodeInt).ToList();
            return newRefData;
        }

        public RegVendor GetRegVendor(string noPengajuan)
        {
            RegVendor rv = ctx.RegVendors.Where(x => x.NoPengajuan == noPengajuan).FirstOrDefault();
            if (rv != null) return rv;
            return null;
        }

        public RegVendorExt GetRegVendorExt(string noPengajuan)
        {

            return new RegVendorExt();
        }

        public RegRiwayatPengajuanVendor GetRegRiwayatPengajuanVendor(int vendorId)
        {
            return new RegRiwayatPengajuanVendor();
        }

        public int RegisterVerifiedVendor(Vendor v)
        {
            try
            {
                ctx.Vendors.Add(v);
                ctx.SaveChanges();
                return v.Id;
            }
            catch (Exception e)
            {

            }
            return 0;
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

        public DbContextTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            return ctx.Database.BeginTransaction(isolationLevel);
        }

        public DbContextTransaction BeginTransaction()
        {
            return BeginTransaction(IsolationLevel.ReadCommitted);
        }
    }
}
