using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reston.Eproc.Model.Ext
{
    public class VWRegVendorExt
    {
        public Guid Id { get; set; }

        public int RegVendorId { get; set; }

        [MaxLength(5)]
        public string JenisVendor { get; set; }

        [MaxLength(5)]
        public string KategoriVendor { get; set; }

        [MaxLength(5)]
        public string BentukBadanUsaha { get; set; }

        [MaxLength(5)]
        public string StatusPerusahaan { get; set; }

        public Nullable<DateTime> EstablishedDate { get; set; }

        [MaxLength(5)]
        public string CountryCode { get; set; }

        [MaxLength(5)]
        public string FirstLevelDivisionCode { get; set; }

        [MaxLength(5)]
        public string SecondLevelDivisionCode { get; set; }

        [MaxLength(5)]
        public string ThirdLevelDivisionCode { get; set; }

        [MaxLength(5)]
        public string PostalCode { get; set; }

        [MaxLength(20)]
        public string Fax { get; set; }

        [MaxLength(5)]
        public string WorkUnitCode { get; set; }

        [MaxLength(5)]
        public string SegBidangUsahaCode { get; set; }

        [MaxLength(5)]
        public string SegKelompokUsahaCode { get; set; }

        [MaxLength(5)]
        public string SegSubBidangUsahaCode { get; set; }

        [MaxLength(5)]
        public string SegKualifikasiGrade { get; set; }

        [MaxLength(250)]
        public string IndivName { get; set; }

        [MaxLength(20)]
        public string IndivAbbrevName { get; set; }

        [MaxLength(20)]
        public string IndivGiidNo { get; set; }

        public Nullable<DateTime> IndivGiidValidUntil { get; set; }

        [MaxLength(500)]
        public string IndivAddress { get; set; }

        [MaxLength(5)]
        public string IndivCountryCode { get; set; }

        [MaxLength(5)]
        public string IndivFirstLevelDivisionCode { get; set; }

        [MaxLength(5)]
        public string IndivSecondLevelDivisionCode { get; set; }

        [MaxLength(5)]
        public string IndivThirdLevelDivisionCode { get; set; }

        [MaxLength(10)]
        public string IndivPostalCode { get; set; }

        [MaxLength(250)]
        public string IndivContactPersonName { get; set; }

        [MaxLength(20)]
        public string IndivContactPhoneNum { get; set; }

        [MaxLength(250)]
        public string PrinRepOfficeAddress { get; set; }

        [MaxLength(20)]
        public string PrinRepOfficeContactPhoneNum { get; set; }

        [MaxLength(20)]
        public string PrinRepOfficeFaxNum { get; set; }

        [MaxLength(50)]
        public string PrinRepOfficeEmail { get; set; }

        [MaxLength(200)]
        public string PrinWebsite { get; set; }
    }

    public class VWRegVendorExtPerson
    {
        public Guid Id { get; set; }

        public Guid RegVendorExtId { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string Position { get; set; }

        [MaxLength(20)]
        public string ContactPhone { get; set; }

        [MaxLength(100)]
        public string ContactEmail { get; set; }

        [MaxLength(200)]
        public string ContactAddress { get; set; }

        [MaxLength(5)]
        public string ReligionCode { get; set; }
    }

    public class VWRegVendorExtBankInfo
    {
        public Guid Id { get; set; }

        public Guid RegVendorExtId { get; set; }

        [MaxLength(5)]
        public string BankCode { get; set; }

        [MaxLength(500)]
        public string BankAddress { get; set; }

        [MaxLength(150)]
        public string BankCity { get; set; }

        [MaxLength(100)]
        public string Branch { get; set; }

        [MaxLength(50)]
        public string AccNumber { get; set; }

        [MaxLength(250)]
        public string AccName { get; set; }

        [MaxLength(5)]
        public string AccCurrencyCode { get; set; }
    }

    public class VWRegVendorExtFinStatement
    {
        public Guid Id { get; set; }

        public Guid RegVendorExtId { get; set; }

        [MaxLength(100)]
        public string FinStmtDocNumber { get; set; }

        [MaxLength(100)]
        public string FinStmtIssuer { get; set; }

        public Nullable<DateTime> FinStmtIssueDate { get; set; }

        public Nullable<DateTime> FinStmtValidThruDate { get; set; }

        public Nullable<Guid> FinStmtDocumentId { get; set; }

        [MaxLength(4)]
        public string FinStmtYear { get; set; }

        [MaxLength(5)]
        public string FinStmtCurrencyCode { get; set; }

        public decimal? FinStmtAktivaLancar { get; set; }

        public decimal? FinStmtHutangLancar { get; set; }

        public decimal? FinStmtRasioLikuiditas { get; set; }

        public decimal? FinStmtTotalHutang { get; set; }

        public decimal? FinStmtEkuitas { get; set; }

        public decimal? FinStmtDebtToEquityRation { get; set; }

        public decimal? FinStmtNetProfitLoss { get; set; }

        public decimal? FinStmtReturnOfEquity { get; set; }

        public decimal? FinStmtKas { get; set; }

        public decimal? FinStmtTotalAktiva { get; set; }

        [MaxLength(5)]
        public string FinStmtAuditStatusCode { get; set; }

        public Nullable<Guid> FinStmtDocId { get; set; }
    }

    public class VWRegVendorExtHumanResource
    {
        public Guid Id { get; set; }

        public Guid RegVendorExtId { get; set; }

        [MaxLength(100)]
        public string ResourceFullName { get; set; }

        public Nullable<DateTime> ResourceDateOfBirth { get; set; }

        [MaxLength(5)]
        public string ResourceExperienceCode { get; set; }

        [MaxLength(200)]
        public string ResourceExpertise { get; set; }

        public Nullable<Guid> ResourceCVDocId { get; set; }

        [MaxLength(5)]
        public string ResourceLastEduCode { get; set; }

        public Nullable<Guid> ResourceLastEduDocId { get; set; }

        [MaxLength(100)]
        public string ResourceLastEduIssuer { get; set; }

        public Nullable<Guid> ResourceCertificationDocId { get; set; }

        [MaxLength(100)]
        public string ResourceCertificationIssuer { get; set; }
    }

    public class VWRegVendorExtEquipment
    {
        public Guid Id { get; set; }

        public Guid VendorRegExtId { get; set; }

        [MaxLength(100)]
        public string EquipmentName { get; set; }

        [MaxLength(100)]
        public string EquipmentQty { get; set; }

        [MaxLength(100)]
        public string EquipmentCapacity { get; set; }

        [MaxLength(100)]
        public string EquipmentMake { get; set; }

        [MaxLength(4)]
        public string EquipmentMakeYear { get; set; }

        [MaxLength(5)]
        public string EquipmentConditionCode { get; set; }

        [MaxLength(100)]
        public string EquipmentLocation { get; set; }

        public Nullable<Guid> EquipmentOwnershipDocId { get; set; }

        public Nullable<Guid> EquipmentPicture { get; set; }
    }

    public class VWRegVendorExtJobHistory
    {
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
    }

}
