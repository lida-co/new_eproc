using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reston.Pinata.Model.JimbisModel
{
    [Table("RegVendor", Schema = AppDbContext.VENDORREG_SCHEMA_NAME)]
    public class RegVendor
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(12)]
        public string NoPengajuan { get; set; }

        public ETipeVendor TipeVendor{ get; set;}

        [MaxLength(255)]
        public string Nama { get; set; }

        [MaxLength(1000)]
        public string Alamat { get; set; }

        [MaxLength(100)]
        public string Provinsi { get; set; }

        [MaxLength(100)]
        public string Kota { get; set; }

        [MaxLength(6)]
        public string KodePos { get; set; }

        [MaxLength(100)]
        public string Website { get; set; }

        [MaxLength(150)]
        public string Email { get; set; }

        [MaxLength(20)]
        public string Telepon { get; set; }

        public EStatusVendor StatusAkhir { get; set; }

        public virtual ICollection<RegBankInfo> RegBankInfo { get; set; }
        public virtual ICollection<RegVendorPerson> RegVendorPerson { get; set; }
        public virtual ICollection<RegRiwayatPengajuanVendor> RegRiwayatPengajuanVendor { get; set; }
        public virtual ICollection<RegDokumen> RegDokumen { get; set; }

    }

    [Table("RegBankInfo", Schema = AppDbContext.VENDORREG_SCHEMA_NAME)]
    public class RegBankInfo {
        [Key]
        public int Id { get; set; }

        [MaxLength(100)]
        public string NamaBank { get; set; }

        [MaxLength(100)]
        public string Cabang { get; set; }

        [MaxLength(50)]
        public string NomorRekening { get; set; }

        [MaxLength(255)]
        public string NamaRekening { get; set; }

        public Nullable<bool> Active { get; set; }
    }

    [Table("RegVendorPerson", Schema = AppDbContext.VENDORREG_SCHEMA_NAME)]
    public class RegVendorPerson
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(100)]
        public string Nama { get; set; }

        [MaxLength(100)]
        public string Jabatan { get; set; }

        [MaxLength(20)]
        public string Telepon { get; set; }

        [MaxLength(150)]
        public string Email { get; set; }

        public Nullable<bool> Active { get; set; }
    }

    [Table("RegRiwayatPengajuanVendor", Schema = AppDbContext.VENDORREG_SCHEMA_NAME)]
    public class RegRiwayatPengajuanVendor
    {
        [Key]
        public int Id { get; set; }

        public Nullable<DateTime> Waktu { get; set; }

        public EStatusVendor Status { get; set; }

        public EMetodeVerifikasiVendor Metode { get; set; }

        [MaxLength(2000)]
        public string Komentar { get; set; }

        public int Urutan { get; set; }
    }

    [Table("CaptchaRegistration", Schema = AppDbContext.VENDORREG_SCHEMA_NAME)]
    public class CaptchaRegistration {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [MaxLength(10)]
        public string Text { get; set; }
        public DateTime ExpiredDate { get; set; }
    }

    [Table("RegVendorExt", Schema = AppDbContext.VENDORREG_SCHEMA_NAME)]
    public class RegVendorExt
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("RegVendorId")]
        public RegVendor RegVendor { get; set; }
        [Required]
        public int RegVendorId { get; set; }

        //public int RegVendorId { get; set; }

        public string JenisVendor { get; set; }

        public string KategoriVendor { get; set; }

        public string BentukBadanUsaha { get; set; }

        public string StatusPerusahaan { get; set; }

        public DateTime? EstablishedDate { get; set; }

        public string CountryCode { get; set; }
        public string SubDistrict { get; set; }
        public string Village { get; set; }

        public string FirstLevelDivisionCode { get; set; }

        public string SecondLevelDivisionCode { get; set; }

        public string ThirdLevelDivisionCode { get; set; }
        public string PostalCode { get; set; }
        public string Fax { get; set; }
        public string WorkUnitCode { get; set; }
        public string SegBidangUsahaCode { get; set; }
        public string SegKelompokUsahaCode { get; set; }
        public string SegSubBidangUsahaCode { get; set; }
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
        public bool IsPKP { get; set; }
        public string KategoriUsaha { get; set; }

        public string DirPersonGiidNo { get; set; } 
        public string DirPersonName { get; set; }
        public string DirPersonPosition { get; set; }
        public string DirPersonReligionCode { get; set; }
        public DateTime? DirPersonBirthDay { get; set; }


    }

    [Table("RegVendorExtBankInfo", Schema = AppDbContext.VENDORREG_SCHEMA_NAME)]
    public class RegVendorExtBankInfo
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("RegVendorExtId")]
        public RegVendorExt RegVendorExt { get; set; }
        [Required]
        public Guid RegVendorExtId { get; set; }

        public string BankCode { get; set; }

        public string BankAddress { get; set; }

        public string BankCity { get; set; }

        public string Branch { get; set; }
        public string AccNumber { get; set; }
        public string AccName { get; set; }
        public string AccCurrencyCode { get; set; }
        public string BankCountry { get; set; }

    }

    [Table("RegVendorExtPerson", Schema = AppDbContext.VENDORREG_SCHEMA_NAME)]
    public class RegVendorExtPerson
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("RegVendorExtId")]
        public RegVendorExt RegVendorExt { get; set; }
        [Required]
        public Guid RegVendorExtId { get; set; }

        public string Name { get; set; }

        public string Position { get; set; }

        public string ContactPhone { get; set; }

        public string ContactEmail { get; set; }
        public string ContactAddress { get; set; }
        public string ReligionCode { get; set; }
        public string GiidNo { get; set; }
        public DateTime? BirthDay { get; set; }

    }

    [Table("RegVendorExtHumanResource", Schema = AppDbContext.VENDORREG_SCHEMA_NAME)]
    public class RegVendorExtHumanResource
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("RegVendorExtId")]
        public RegVendorExt RegVendorExt { get; set; }
        [Required]
        public Guid RegVendorExtId { get; set; }

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

    [Table("RegVendorExtFinStatement", Schema = AppDbContext.VENDORREG_SCHEMA_NAME)]
    public class RegVendorExtFinStatement
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("RegVendorExtId")]
        public RegVendorExt RegVendorExt { get; set; }
        [Required]
        public Guid RegVendorExtId { get; set; }

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
        public decimal FinStmtDebtToEquityRation { get; set; }
        public decimal FinStmtNetProfitLoss { get; set; }
        public decimal FinStmtReturnOfEquity { get; set; }

        public decimal FinStmtKas { get; set; }
        public decimal FinStmtTotalAktiva { get; set; }
        public string FinStmtAuditStatusCode { get; set; }
        public Guid FinStmtDocId { get; set; }

    }

    [Table("RegVendorExtEquipment", Schema = AppDbContext.VENDORREG_SCHEMA_NAME)]
    public class RegVendorExtEquipment
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("RegVendorExtId")]
        public RegVendorExt RegVendorExt { get; set; }
        [Required]
        public Guid RegVendorExtId { get; set; }

        public string EquipmentName { get; set; }

        public string EquipmentQty { get; set; }

        public string EquipmentCapacity { get; set; }

        public string EquipmentMake { get; set; }
        public string EquipmentMakeYear { get; set; }
        public string EquipmentConditionCode { get; set; }
        public string EquipmentLocation { get; set; }
        public Guid EquipmentOwnershipDocId { get; set; }
        public Guid EquipmentPicture { get; set; }

    }

    [Table("RegVendorExtJobHistory", Schema = AppDbContext.VENDORREG_SCHEMA_NAME)]
    public class RegVendorExtJobHistory
    {
        [Key]
        public Guid Id { get; set; }

        public Guid RegVendorExtId { get; set; }

        [MaxLength(200)]
        public string JobTitle { get; set; }

        [MaxLength(100)]
        public string JobClient { get; set; }

        [MaxLength(100)]
        public string JobLocation { get; set; }

        public Nullable<DateTime> JobStartDate { get; set; }

        [MaxLength(100)]
        public string JobContractNum { get; set; }

        public Nullable<DateTime> JobContractDate { get; set; }

        public decimal? JobContractAmount { get; set; }

        [MaxLength(5)]
        public string JobContractAmountCurrencyCode { get; set; }

        public Nullable<Guid> JobContractDocId { get; set; }
        public string JobType { get; set; }
    }

    [Table("RegVendorDocumentExt", Schema = AppDbContext.VENDORREG_SCHEMA_NAME)]
    public class RegVendorDocumentExt
    {
        [Key]
        public Guid Id { get; set; }

        public Guid RegVendorExtId { get; set; }

        [MaxLength(100)]
        public string Nomor { get; set; }

        [MaxLength(100)]
        public string Pembuat { get; set; }

        public Nullable<DateTime> TanggalTerbit { get; set; }
        public Nullable<DateTime> TanggalBerakhir { get; set; }

        public int TipeDokumen { get; set; }

        public byte[] Content { get; set; }
    }

    [Table("RegDocumentExt", Schema = AppDbContext.VENDORREG_SCHEMA_NAME)]
    public class RegDocumentExt
    {
        [Key]
        public Guid Id { get; set; }
        public Guid RegVendorExtId { get; set; }
        [MaxLength(100)]
        public string Nomor { get; set; }
        [MaxLength(100)]
        public string Penerbit { get; set; }
        public int TipeDokumen { get; set; }
        public Nullable<DateTime> TanggalTerbit { get; set; }
        public Nullable<DateTime> TanggalBerakhir { get; set; }
        public bool Active { get; set; }
    }

    [Table("RegDocumentImageExt", Schema = AppDbContext.VENDORREG_SCHEMA_NAME)]
    public class RegDocumentImageExt 
    {
        [Key]
        public Guid Id { get; set; }
        public byte[] Content { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public Guid RegDocumenExtId { get; set; }

    }


    [Table("VendorExt", Schema = AppDbContext.VENDOR_SCHEMA_NAME)]
    public class VendorExt
    {
        [Key]
        public Guid Id { get; set; }

        public int VendorId { get; set; }

        //public int RegVendorId { get; set; }

        public string JenisVendor { get; set; }

        public string KategoriVendor { get; set; }

        public string BentukBadanUsaha { get; set; }

        public string StatusPerusahaan { get; set; }

        public DateTime? EstablishedDate { get; set; }

        public string CountryCode { get; set; }
        public string SubDistrict { get; set; }
        public string Village { get; set; }

        public string FirstLevelDivisionCode { get; set; }

        public string SecondLevelDivisionCode { get; set; }

        public string ThirdLevelDivisionCode { get; set; }
        public string PostalCode { get; set; }
        public string Fax { get; set; }
        public string WorkUnitCode { get; set; }
        public string SegBidangUsahaCode { get; set; }
        public string SegKelompokUsahaCode { get; set; }
        public string SegSubBidangUsahaCode { get; set; }
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
        public string CPName { get; set; }
        public bool IsPKP { get; set; }
        public string NomorPKP { get; set; }
        public Guid? PKPDocId { get; set; }
        public string KategoriUsaha { get; set; }

        public string DirPersonGiidNo { get; set; }
        public string DirPersonName { get; set; }
        public string DirPersonPosition { get; set; }
        public string DirPersonReligionCode { get; set; }
        public DateTime? DirPersonBirthDay { get; set; }

        public Guid? CompanyProfileDocId { get; set; }

    }

    [Table("VendorExtBankInfo", Schema = AppDbContext.VENDOR_SCHEMA_NAME)]
    public class VendorExtBankInfo
    {
        [Key]
        public Guid Id { get; set; }

        public Guid VendorExtId { get; set; }

        public string BankCode { get; set; }

        public string BankAddress { get; set; }

        public string BankCity { get; set; }

        public string Branch { get; set; }
        public string AccNumber { get; set; }
        public string AccName { get; set; }
        public string AccCurrencyCode { get; set; }
        public string BankCountry { get; set; }

    }

    [Table("VendorExtPerson", Schema = AppDbContext.VENDOR_SCHEMA_NAME)]
    public class VendorExtPerson
    {
        [Key]
        public Guid Id { get; set; }
            
        public Guid VendorExtId { get; set; }

        public string Name { get; set; }

        public string Position { get; set; }

        public string ContactPhone { get; set; }

        public string ContactEmail { get; set; }
        public string ContactAddress { get; set; }
        public string ReligionCode { get; set; }
        public string GiidNo { get; set; }
        public DateTime? BirthDay { get; set; }

    }

    [Table("VendorExtHumanResource", Schema = AppDbContext.VENDOR_SCHEMA_NAME)]
    public class VendorExtHumanResource
    {
        [Key]
        public Guid Id { get; set; }

        public Guid VendorExtId { get; set; }

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

    [Table("VendorExtFinStatement", Schema = AppDbContext.VENDOR_SCHEMA_NAME)]
    public class VendorExtFinStatement
    {
        [Key]
        public Guid Id { get; set; }

        public Guid VendorExtId { get; set; }

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
        public decimal FinStmtDebtToEquityRation { get; set; }
        public decimal FinStmtNetProfitLoss { get; set; }
        public decimal FinStmtReturnOfEquity { get; set; }

        public decimal FinStmtKas { get; set; }
        public decimal FinStmtTotalAktiva { get; set; }
        public string FinStmtAuditStatusCode { get; set; }
        public Guid FinStmtDocId { get; set; }

    }

    [Table("VendorExtEquipment", Schema = AppDbContext.VENDOR_SCHEMA_NAME)]
    public class VendorExtEquipment
    {
        [Key]
        public Guid Id { get; set; }

        public Guid VendorExtId { get; set; }

        public string EquipmentName { get; set; }

        public string EquipmentQty { get; set; }

        public string EquipmentCapacity { get; set; }

        public string EquipmentMake { get; set; }
        public string EquipmentMakeYear { get; set; }
        public string EquipmentConditionCode { get; set; }
        public string EquipmentLocation { get; set; }
        public Guid EquipmentOwnershipDocId { get; set; }
        public Guid EquipmentPicture { get; set; }

    }

    [Table("VendorExtJobHistory", Schema = AppDbContext.VENDOR_SCHEMA_NAME)]
    public class VendorExtJobHistory
    {
        [Key]
        public Guid Id { get; set; }

        public Guid VendorExtId { get; set; }

        [MaxLength(200)]
        public string JobTitle { get; set; }

        [MaxLength(100)]
        public string JobClient { get; set; }

        [MaxLength(100)]
        public string JobLocation { get; set; }

        public Nullable<DateTime> JobStartDate { get; set; }

        [MaxLength(100)]
        public string JobContractNum { get; set; }

        public Nullable<DateTime> JobContractDate { get; set; }

        public decimal? JobContractAmount { get; set; }

        [MaxLength(5)]
        public string JobContractAmountCurrencyCode { get; set; }

        public Nullable<Guid> JobContractDocId { get; set; }
        public string JobType { get; set; }
    }

    [Table("VendorDocumentExt", Schema = AppDbContext.VENDOR_SCHEMA_NAME)]
    public class VendorDocumentExt
    {
        [Key]
        public Guid Id { get; set; }

        public Guid VendorExtId { get; set; }

        [MaxLength(100)]
        public string Nomor { get; set; }

        [MaxLength(100)]
        public string Pembuat { get; set; }

        public Nullable<DateTime> TanggalTerbit { get; set; }
        public Nullable<DateTime> TanggalBerakhir { get; set; }

        public int TipeDokumen { get; set; }

        public byte[] Content { get; set; }
    }

    [Table("DocumentExt", Schema = AppDbContext.VENDOR_SCHEMA_NAME)]
    public class DocumentExt
    {
        [Key]
        public Guid Id { get; set; }
        public Guid VendorExtId { get; set; }
        [MaxLength(100)]
        public string Nomor { get; set; }
        [MaxLength(100)]
        public string Penerbit { get; set; }
        public int TipeDokumen { get; set; }
        public Nullable<DateTime> TanggalTerbit { get; set; }
        public Nullable<DateTime> TanggalBerakhir { get; set; }
        public bool Active { get; set; }
    }

    [Table("DocumentImageExt", Schema = AppDbContext.VENDOR_SCHEMA_NAME)]
    public class DocumentImageExt
    {
        [Key]
        public Guid Id { get; set; }
        public byte[] Content { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public Guid DocumenExtId { get; set; }

    }

}
