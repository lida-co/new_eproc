using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reston.Pinata.Model.JimbisModel;

namespace Reston.Pinata.Model.Asuransi
{
    [Table("InsuranceTarifTemplate", Schema = AppDbContext.ASURANSI_SCHEMA_NAME)]
    public class InsuranceTarifTemplate
    {
        [Key]
        public int Id { get; set; }
        public Guid DocumentId { get; set; }
        public string DocumentTitle { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedTS { get; set; }
        public string DeletedBy { get; set; }
        public DateTime? DeletedTS { get; set; }
        public bool? IsDeleted { get; set; }
        public string Owner { get; set; }
        public string BenefitType { get; set; }

        public InsuranceTarifTemplate ToList()
        {
            throw new NotImplementedException();
        }
    }

    [Table("InsuranceTarifBenefitTemplate", Schema = AppDbContext.ASURANSI_SCHEMA_NAME)]
    public class InsuranceTarifBenefitTemplate
    {
        [Key]
        public int Id { get; set; }
        public Guid DocumentId { get; set; }
        public BenefitRateTemplate BenefitRateId { get; set; }    
    }

    [Table("InsuranceTarif", Schema = AppDbContext.ASURANSI_SCHEMA_NAME)]
    public class InsuranceTarif
    {
        [Key]
        public int Id { get; set; }
        public Guid DocumentId { get; set; }
        public string DocumentTitle { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedTS { get; set; }
        public string DeletedBy { get; set; }
        public DateTime? DeletedTS { get; set; }
        public bool? IsDeleted { get; set; }
        public string Owner { get; set; }
        public string BenefitType { get; set; }
        public Guid PengadaanId { get; set; }
        public string InsuranceTarifTemplateDocumentId { get; set; }
    }

    [Table("InsuranceTarifBenefit", Schema = AppDbContext.ASURANSI_SCHEMA_NAME)]
    public class InsuranceTarifBenefit
    {
        [Key]
        public int Id { get; set; }
        public Guid DocumentId { get; set; }
        public BenefitRate BenefitRateId { get; set; }   
    }

    [Table("BenefitRateTemplate", Schema = AppDbContext.ASURANSI_SCHEMA_NAME)]
    public class BenefitRateTemplate
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(30)]
        public string BenefitCode { get; set; }
        [MaxLength(30)]
        public string BenefitCoverage { get; set; }
        [MaxLength(30)]
        public string RegionCode { get; set; }
        public bool? IsOpen { get; set; }
        public decimal? RateUpperLimit { get; set; }
        public decimal? RateLowerLimit { get; set; }
        public decimal? Rate { get; set; }
        public bool? IsRange { get; set; }
    }

    [Table("BenefitRate", Schema = AppDbContext.ASURANSI_SCHEMA_NAME)]
    public class BenefitRate
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(30)]
        public string BenefitCode { get; set; }
        [MaxLength(30)]
        public string BenefitCoverage { get; set; }
        [MaxLength(30)]
        public string RegionCode { get; set; }
        public bool? IsOpen { get; set; }
        public decimal? RateUpperLimit { get; set; }
        public decimal? RateLowerLimit { get; set; }
        public decimal? Rate { get; set; }
        public bool? IsRange { get; set; }
    }

    [Table("IncurenceValuePengadaan", Schema = AppDbContext.ASURANSI_SCHEMA_NAME)]
    public class IncurenceValuePengadaan
    {
        [Key]
        public Guid Id { get; set; }
        public Guid DocumentId { get; set; }
        public Guid PengadaanId { get; set; }

    }

    [Table("HargaRekananAsuransi", Schema = AppDbContext.ASURANSI_SCHEMA_NAME)]
    public class HargaRekananAsuransi
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("BenefitRate")]
        public int BenefitCodeId { get; set; }
        [ForeignKey("Vendor")]
        public int VendorId { get; set; }
        public decimal? RateUpperLimit { get; set; }
        public decimal? RateLowerLimit { get; set; }
        public decimal? Rate { get; set; }
        public string RateUpperLimitEncrypt { get; set; }
        public string RateLowerLimitEncrypt { get; set; }
        public string RateEncrypt { get; set; }
        public string Keterangan { get; set; }
        public virtual BenefitRate BenefitRate { get; set;}
        public virtual  Vendor Vendor { get; set; }
    }

    [Table("HargaKlarifikasiRekananAsuransi", Schema = AppDbContext.ASURANSI_SCHEMA_NAME)]
    public class HargaKlarifikasiRekananAsuransi
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("BenefitRate")]
        public int BenefitCodeId { get; set; }
        [ForeignKey("Vendor")]
        public int VendorId { get; set; }
        public decimal? RateUpperLimit { get; set; }
        public decimal? RateLowerLimit { get; set; }
        public decimal? Rate { get; set; }
        public string Keterangan { get; set; }
        public virtual BenefitRate BenefitRate { get; set; }
        public virtual Vendor Vendor { get; set; }
    }

    [Table("HargaKlarifikasiLanjutanAsuransi", Schema = AppDbContext.ASURANSI_SCHEMA_NAME)]
    public class HargaKlarifikasiLanjutanAsuransi
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("BenefitRate")]
        public int BenefitCodeId { get; set; }
        [ForeignKey("Vendor")]
        public int VendorId { get; set; }
        public decimal? RateUpperLimit { get; set; }
        public decimal? RateLowerLimit { get; set; }
        public decimal? Rate { get; set; }
        public string Keterangan { get; set; }
        public virtual BenefitRate BenefitRate { get; set; }
        public virtual Vendor Vendor { get; set; }
    }
}