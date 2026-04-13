using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Reston.EProc.Web.ViewModels
{
    public class VendorExtViewModel
    {
        public int id { get; set; }
        public string gCaptchaResponse { get; set; }
        public string gCaptchaK { get; set; }
        public string NoPengajuan { get; set; }
        public int TipeVendor { get; set; }
        public string Nama { get; set; }
        public string Alamat { get; set; }
        public string Provinsi { get; set; }
        public string Kota { get; set; }
        public string KodePos { get; set; }
        public string Website { get; set; }
        public string Email { get; set; }
        public string Telepon { get; set; }
        public string StatusAkhir { get; set; }
        public string JenisRekanan { get; set; }

        public string FirstLevelDivisionCode { get; set; }
        public string SecondLevelDivisionCode { get; set; }
        public string ThirdLevelDivisionCode { get; set; }
        public int VendorId { get; set; }

        public VendorRegExtViewModel VendorRegExt { get; set; }
        public VendorPersonExtViewModel[] VendorPersonExt { get; set; }
        public VendorBankInfoExtViewModel VendorBankInfoExt { get; set; }
        public VendorHumanResourceExtViewModel[] VendorHumanResourceExt { get; set; }
        public VendorFinStatementExtViewModel VendorFinStatementExt { get; set; }
        public VendorEquipmentExtViewModel[] VendorEquipmentExt { get; set; }
        public VendorJobHistoryExtViewModel[] VendorJobHistoryExt { get; set; }

        public VendorDokumenExt NPWP { get; set; }
        public VendorDokumenExt PKP { get; set; }
        public VendorDokumenExt LaporanKeuangan { get; set; }
        public VendorDokumenExt RekeningKoran { get; set; }
        public VendorDokumenExt DRT { get; set; }
        public VendorDokumenExt AktaPendirian { get; set; }
        public VendorDokumenExt SKKemenkumham { get; set; }
        public VendorDokumenExt BeritaNegara { get; set; }
        public VendorDokumenExt AktaNegara { get; set; }
        public VendorDokumenExt IndivGiid { get; set; }
        public VendorDokumenExt ProfilPerusahaan { get; set; }
        public VendorDokumenExt SIUP { get; set; }
        public VendorDokumenExt SIUJK { get; set; }
        public VendorDokumenExt NIB { get; set; }
        public VendorDokumenExt Sertifikat { get; set; }
        public VendorDokumenExt TDP { get; set; }
        public VendorDokumenExt Domisili { get; set; }
        public VendorDokumenExt SertifikasiTenagaAhli { get; set; }
        public VendorDokumenExt CVTenagaAhli { get; set; }
        public VendorDokumenExt VendorEquipmentExt1 { get; set; }
        public VendorDokumenExt VendorEquipmentExt2 { get; set; }
        public VendorDokumenExt VendorJobHistoryExt1 { get; set; }  


    }

    public class VendorRegExtViewModel
    {
        public string JenisVendor { get; set; }
        public string NPWP { get; set; }
        public string PKP { get; set; }
        //public bool isPKP { get; set; }
        //public bool isNonPKP { get; set; }
        public string isPKP { get; set; }
        public string isNonPKP { get; set; }

        public string KategoriVendor { get; set; }

        public string BentukBadanUsaha { get; set; }

        public string StatusPerusahaan { get; set; }

        public DateTime EstablishedDate { get; set; }

        public string CountryCode { get; set; }

        public string FirstLevelDivisionCode { get; set; }
        public string SecondLevelDivisionCode { get; set; }
        public string ThirdLevelDivisionCode { get; set; }
        public string PostalCode { get; set; }
        public string Fax { get; set; }
        public string WorkUnitCode { get; set; }
        public string SegBidangUsahaCode { get; set; }
        public List<string> SegKelompokUsahaCode { get; set; }
        public string SegKelompokUsahaCodeIT { get; set; }
        public string SegKelompokUsahaCodeNonIT { get; set; }
        public string SegKelompokUsahaCodeKonstruksi { get; set; }
        public List<string> SegSubBidangUsahaCode { get; set; }
        public string SegKualifikasiGrade { get; set; }

        public string IndivName { get; set; }
        public string IndivAbbrevName { get; set; }
        public string IndivGiidNo { get; set; }
        public DateTime? IndivGiidValidUntil { get; set; }
        public Guid IndivGiidDocId { get; set; }
        public string IndivAddress { get; set; }
        public string IndivCountryCode { get; set; }
        public string IndivFirstLevelDivisionCode { get; set; }
        public string IndivSecondLevelDivisionCode { get; set; }
        public string IndivThirdLevelDivisionCode { get; set; }
        public string IndivPostalCode { get; set; }
        public string IndivContactPersonName { get; set; }
        public string IndivContactPhoneNum { get; set; }
        public string IndivContactEmail { get; set; }

        public string PrinRepOfficeAddress { get; set; }
        public string PrinRepOfficeContactPhoneNum { get; set; }
        public string PrinRepOfficeFaxNum { get; set; }
        public string PrinRepOfficeEmail { get; set; }
        public string PrinWebsite { get; set; }
        public string PrinRepPosition { get; set; }
        public string PrinRepOfficeLocalAddress { get; set; }
        public string CPName { get; set; }
        public string KategoriUsaha { get; set; }

        public string DirPersonGiidNo { get; set; }
        public string DirPersonName { get; set; }
        public string DirPersonPosition { get; set; }
        public string DirPersonReligionCode { get; set; }
        public DateTime? DirPersonBirthDay { get; set; }
    }

    public class VendorBankInfoExtViewModel
    {
        public string BankCode { get; set; }
        public string BankAddress { get; set; }
        public string BankCity { get; set; }
        public string Branch { get; set; }
        public string AccNumber { get; set; }
        public string AccName { get; set; }
        public string AccCurrencyCode { get; set; }
        public string BankCountry { get; set; }
    }

    public class VendorPersonExtViewModel
    {
        public string Name { get; set; }
        public string Position { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public string ContactAddress { get; set; }
        public string ReligionCode { get; set; }
        public string GiidNo { get; set; }
        public DateTime? BirthDay { get; set; }
    }

    public class VendorHumanResourceExtViewModel
    {
        public string ResourceFullName { get; set; }
        public DateTime? ResourceDateOfBirth { get; set; }
        public string ResourceExperienceCode { get; set; }
        public string ResourceExpertise { get; set; }
        public Guid ResourceCVDocId { get; set; }
        public string ResourceLastEduCode { get; set; }
        public Guid ResourceLastEduDocId { get; set; }
        public string ResourceLastEduIssuer { get; set; }
        public Guid ResourceCertificationDocId { get; set; }
        public string ResourceCertificationIssuer { get; set; }
    }

    public class VendorFinStatementExtViewModel
    {
        public string FinStmtDocNumber { get; set; }
        public string FinStmtIssuer { get; set; }
        public DateTime? FinStmtIssueDate { get; set; }
        public DateTime? FinStmtValidThruDate { get; set; }
        public string FinStmtDocumentId { get; set; }
        public string FinStmtYear { get; set; }
        public string FinStmtCurrencyCode { get; set; }
        public decimal FinStmtAktivaLancar { get; set; }
        public decimal FinStmtHutangLancar { get; set; }
        public decimal FinStmtRasioLikuiditas { get; set; }
        public decimal FinStmtTotalHutang { get; set; }
        public decimal FinStmtEkuitas { get; set; }
        public decimal FinStmtDebtToEquityRatio { get; set; }
        public decimal FinStmtNetProfitLoss { get; set; }
        public decimal FinStmtReturnOfEquity { get; set; }
        public decimal FinStmtKas { get; set; }
        public decimal FinStmtTotalAktiva { get; set; }
        public string FinStmtAuditStatusCode { get; set; }
        public string base64 { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
    }

    public class VendorEquipmentExtViewModel
    {
        public string EquipmentName { get; set; }
        public string EquipmentQty { get; set; }
        public string EquipmentCapacity { get; set; }
        public string EquipmentMake { get; set; }
        public string EquipmentMakeYear { get; set; }
        public string EquipmentConditionCode { get; set; }
        public string EquipmentLocation { get; set; }
        public Guid EquipmentOwnershipDocId { get; set; }
        public Guid EquipmentPictureDocId { get; set; }
        public string base64 { get; set; }
        public string ContentType { get; set; } 
        public string FileName { get; set; }
        public string base64a { get; set; }
        public string ContentTypea { get; set; }
        public string FileNamea { get; set; }
    }

    public class VendorJobHistoryExtViewModel
    {
        public Guid Id { get; set; }
        public Guid RegVendorExtId { get; set; }
        public string JobTitle { get; set; }
        public string JobClient { get; set; }
        public string JobLocation { get; set; }
        public Nullable<DateTime> JobStartDate { get; set; }
        public string JobContractNum { get; set; }
        public Nullable<DateTime> JobContractDate { get; set; }
        public decimal? JobContractAmount { get; set; }
        public string JobContractAmountCurrencyCode { get; set; }
        public Nullable<Guid> JobContractDocId { get; set; }
        public string JobType { get; set; }
        public string base64 { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
    }

    public class VendorDokumenExt
    {
        public string Nomor { get; set; }
        public string Pembuat { get; set; }
        public DateTime? TanggalTerbit { get; set; }
        public DateTime? TanggalBerakhir { get; set; }
        public string TipeDokumen { get; set; }
        //public HttpPostedFileBase Content { get; set; }
        public string base64 { get; set; }
        public string base64a { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
    }

    public enum ETipeVendorExt
    {
        NONE = 0,
        PERUSAHAAN = 1, PERORANGAN = 2, PRINCIPAL = 3, NON_REGISTER = 4
    }
}
