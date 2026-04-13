using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reston.Pinata.Model.PengadaanRepository.View;

namespace Reston.Eproc.Model.Ext
{
    public class DataTablePemenangPengadaanNilaiVendor
    {
        public int draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public List<VWPemenangPengadaanNilaiVendor> data { get; set; }
    }

    public class VWPemenangPengadaanNilaiVendor
    {
        public Guid? Id { get; set; }
        public string NoPengadaan { get; set; }
        public Nullable<Guid> PengadaanId { get; set; }
        public string JudulPengadaan { get; set; }
        public Nullable<int> VendorId { get; set; }
        public string NamaVendor { get; set; }
        public string PIC { get; set; }
        public string CekCreate { get; set; }
        public string SudahNilai { get; set; }
        public string CekPenilai { get; set; }
        public Guid? apw { get; set; }
        public DateTime? created { get; set; }
        public decimal CurrentTotal { get; set; }
        public decimal CurrentAverage { get; set; }
    }

    public class VWdetailSPKNilaiVendor
    {
        public string IdSPK { get; set; }
        public string JudulPengadaan { get; set; }
        public string Deskripsi { get; set; }
        public Guid pengadaanId { get; set; }
        public string PemenangPengadaan { get; set; }
        public int VendorId { get; set; }
        public string CekCreate { get; set; }
        public Guid personilId { get; set; }
        public decimal total { get; set; }
        public decimal average { get; set; }
        public int counter { get; set; }
        public int counterAll { get; set; }
    }

    public class VWTenderScoringHeaderExt
    {
        public Guid Id { get; set; }
        public Guid PengadaanId { get; set; }
        public virtual ICollection<VWVendor> VendorId { get; set; }
        public decimal? Total { get; set; }
        public decimal? Averages { get; set; }
        public virtual ICollection<VWTenderScoringDetails> TenderScoringDetails { get; set; }
        public Guid header { get; set; }
        public virtual ICollection<VWTenderScoringBobot> TenderScoringBobot { get; set; }
        public virtual ICollection<VWTenderScoringPenilai> TenderScoringPenilais { get; set; }
    }



    public class VWTenderScoringPenilai
    {
        public Guid Id { get; set; }
        public Guid TenderScoringHeaderId { get; set; }
        public Guid PengadaanId { get; set; }
        public int VendorId { get; set; }
        public string nama { get; set; }
        public Guid UserId { get; set; }
    }

    
    public class VWApprisalWorksheet
    {
        public Guid Id { get; set; }
        public Guid PengadaanId { get; set; }
        public string NoSPk { get; set; }
        public int VendorId { get; set; }
        public decimal CurrentTotal { get; set; }
        public decimal CurrentAverage { get; set; }
        public int Status { get; set; }
        public Nullable<DateTime> CreatedOn { get; set; }
        public Guid CreatedBy { get; set; }
        public Nullable<DateTime> UpdatedOn { get; set; }
        public Nullable<Guid> UpdatedBy { get; set; }
    }
    
    public class VWApprisalWorksheetDetail
    {
        public Guid Id { get; set; }
        public Guid ApprisalWorksheetId { get; set; }
        public string QuestionCode { get; set; }
        public int Weight { get; set; }
    }
    
    public class VWApprisalWorksheetResponse
    {
        public Guid Id { get; set; }
        public Guid ApprisalWorksheetId { get; set; }
        public Guid AppriserUserId { get; set; }
        public Nullable<DateTime> CreatedOn { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<DateTime> UpdatedOn { get; set; }
        public Nullable<Guid> UpdatedBy { get; set; }
    }
    
    public class VWApprisalWorksheetResposeDetail
    {
        public Guid Id { get; set; }
        public Guid ApprisalWorksheetResposeId { get; set; }
        public string QuestionCode { get; set; }
        public int? Score { get; set; }
    }
    
    public class VWSanksi
    {
        public Guid Id { get; set; }
        public int VendorId { get; set; }
        public string DecisionTypeCode { get; set; }
        public string DecisionDescription { get; set; }
        public DateTime DecisionValidFrom { get; set; }
        public DateTime DecisionValidUntil { get; set; }
        public Nullable<DateTime> CreatedOn { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<DateTime> UpdatedOn { get; set; }
        public Nullable<Guid> UpdatedBy { get; set; }
        
        public string NomorVendor { get; set; }
        public string NamaVendor { get; set; }
        public string Bidangvendor { get; set; }
        public string kelompokvendor { get; set; }
        public string DalamMasaSanksi { get; set; }
        public string TipeVendor { get; set; }
    }

    public class VWPersonilPenilaian
    {
        public string NamaPenilai { get; set; }
        public Guid AppriserUserId { get; set; }
    }

    public class DataTableVWVendorwithSanksi
    {
        public int draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public List<VWSanksi> data { get; set; }
    }

    public class DataTableVWRekananPencarian
    {
        public int draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public List<VWRekananPencarian> data { get; set; }
    }

    public class VWRekananPencarian
    {
        public int IdVendor { get; set; }
        public string NamaVendor { get; set; }
        public string NomorVendor { get; set; }
        public string TipeVendor { get; set; }
        public string StatusSanksi { get; set; }
    }



    public class VendorExtViewModelJaws
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

        //for edit
        public string ProvinsiCode { get; set; }
        public string FirstLevelDivisionCodeCode { get; set; }
        public string SecondLevelDivisionCodeCode { get; set; }
        public string ThirdLevelDivisionCodeCode { get; set; }

        public VendorRegExtViewModels VendorRegExt { get; set; }
        public List<VendorPersonExtViewModels> VendorPersonExt { get; set; }
        public VendorBankInfoExtViewModels VendorBankInfoExt { get; set; }
        public List<VendorHumanResourceExtViewModels> VendorHumanResourceExt { get; set; }
        public VendorFinStatementExtViewModels VendorFinStatementExt { get; set; }
        public List<VendorEquipmentExtViewModels> VendorEquipmentExt { get; set; }
        public List<VendorJobHistoryExtViewModels> VendorJobHistoryExt { get; set; }

        public VendorDokumenExts NPWP { get; set; }
        public VendorDokumenExts PKP { get; set; }
        public VendorDokumenExts TDP { get; set; }
        public VendorDokumenExts SIUP { get; set; }
        public VendorDokumenExts SIUJK { get; set; }
        public VendorDokumenExts AKTA { get; set; }
        public VendorDokumenExts PENGADAAN { get; set; }
        public VendorDokumenExts KTP { get; set; }
        public VendorDokumenExts SERTIFIKAT { get; set; }
        public VendorDokumenExts NPWPPemilik { get; set; }
        public VendorDokumenExts KTPPemilik { get; set; }
        public VendorDokumenExts DOMISILI { get; set; }
        public VendorDokumenExts LAPORANKEUANGAN { get; set; }
        public VendorDokumenExts REKENINGKORAN { get; set; }
        public VendorDokumenExts DRT { get; set; }
        public VendorDokumenExts AKTAPENDIRIAN { get; set; }
        public VendorDokumenExts SKKEMENKUMHAM { get; set; }
        public VendorDokumenExts BERITANEGARA { get; set; }
        public VendorDokumenExts AKTAPERUBAHAN { get; set; }
        public VendorDokumenExts PROFILPERUSAHAAN { get; set; }
        public VendorDokumenExts NIB { get; set; }
        public VendorDokumenExts IndivGiid { get; set; }

        public VendorDokumenExts DokumenSertifikatCV { get; set; }
        public VendorDokumenExts BuktiKepemilikanPeralatan { get; set; }
        public VendorDokumenExts FotoPeralatan { get; set; }
        public VendorDokumenExts BuktiKerjasama { get; set; }
        public VendorDokumenExts LaporanDataKeuangan { get; set; }
        public VendorDokumenExts CVTenagaAhli { get; set; }
    }

    public class VendorRegExtViewModels
    {
        public string JenisVendor { get; set; }
        public string NPWP { get; set; }
        public string PKP { get; set; }

        
        public string KategoriUsaha { get; set; }
        public string KategoriVendor { get; set; }

        public string BentukBadanUsaha { get; set; }

        public string StatusPerusahaan { get; set; }

        public DateTime EstablishedDate { get; set; }

        public string CountryCode { get; set; }
        //for edit
        public string CountryCodeCode { get; set; }

        public string FirstLevelDivisionCode { get; set; }
        public string SecondLevelDivisionCode { get; set; }
        public string ThirdLevelDivisionCode { get; set; }
        public string PostalCode { get; set; }
        public string Fax { get; set; }
        public string WorkUnitCode { get; set; }
        public string SegBidangUsahaCode { get; set; }
        public string SegBidangUsahaCodes { get; set; }
        public List<string> SegKelompokUsahaCode { get; set; }
        public string SegKelompokUsahaCodeSingle{ get; set; }
        public string SegKelompokUsahaCodeIT { get; set; }
        public string SegKelompokUsahaCodeNonIT { get; set; }
        public string SegKelompokUsahaCodeKonstruksi { get; set; }
        public string SegSubBidangUsahaCode { get; set; }
        public string SegKualifikasiGrade { get; set; }

        public string IndivName { get; set; }
        public string IndivAbbrevName { get; set; }
        public string IndivGiidNo { get; set; }
        public DateTime IndivGiidValidUntil { get; set; }
        public Guid IndivGiidDocId { get; set; }
        public string IndivAddress { get; set; }
        public string IndivCountryCode { get; set; }
        public string IndivFirstLevelDivisionCode { get; set; }
        public string IndivSecondLevelDivisionCode { get; set; }
        public string IndivThirdLevelDivisionCode { get; set; }
        public string IndivPostalCode { get; set; }
        public string IndivContactPersonName { get; set; }
        public string IndivContactPhoneNum { get; set; }
        public string IndivTaxNo { get; set; }
        public string IndivStateProvinceCode { get; set; }
        public string IndivPhoneNum { get; set; }
        public string IndivFax { get; set; }
        public string IndivEmail { get; set; }
        public string IndivContactEmail { get; set; }

        public string PrinRepOfficeAddress { get; set; }
        public string PrinRepOfficeContactPhoneNum { get; set; }
        public string PrinRepOfficeFaxNum { get; set; }
        public string PrinRepOfficeEmail { get; set; }
        public string PrinWebsite { get; set; }
        public string PrinRepPosition { get; set; }
        public string PrinRepOfficeLocalAddress { get; set; }

        public string SubDistrict { get; set; }
        public string Village { get; set; }

        public string CPName { get; set; }
        public bool IsPKP { get; set; }

        public string DirPersonGiidNo { get; set; }
        public string DirPersonName { get; set; }
        public string DirPersonPosition { get; set; }
        public string DirPersonReligionCode { get; set; }
        public DateTime? DirPersonBirthDay { get; set; }
    }

    public class VendorBankInfoExtViewModels
    {
        public string BankCode { get; set; }
        public string BankAddress { get; set; }
        public string BankCity { get; set; }
        public string Branch { get; set; }
        public string AccNumber { get; set; }
        public string AccName { get; set; }
        public string AccCurrencyCode { get; set; }
        public string BankCountry { get; set; }

        //for edit
        public string BankCodeCode { get; set; }
        public string BankCountryCode { get; set; }
    }

    public class VendorPersonExtViewModels
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

    public class VendorHumanResourceExtViewModels
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
        public string ResourceExperienceYears { get; set; }
    }

    public class VendorFinStatementExtViewModels
    {
        public string FinStmtDocNumber { get; set; }
        public string FinStmtIssuer { get; set; }
        public DateTime? FinStmtIssueDate { get; set; }
        public DateTime? FinStmtValidThruDate { get; set; }
        public string FinStmtDocumentId { get; set; }
        public string FinStmtYear { get; set; }
        public string FinStmtCurrencyCode { get; set; }
        //for edit
        public string FinStmtCurrencyCodeCode { get; set; }
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

    public class VendorEquipmentExtViewModels
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

    public class VendorJobHistoryExtViewModels
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
        //for edit
        public string JobContractAmountCurrencyCodeCode { get; set; }
        public Nullable<Guid> JobContractDocId { get; set; }
        public string JobType { get; set; }
        public string base64 { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
    }

    public class VendorDokumenExts
    {
        public Guid Iddok { get; set; }
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

    public class RootObject
    {
        public List<string> value { get; set; }
    }
}
