using Reston.Eproc.Model.Monitoring.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reston.Pinata.Model.Asuransi;
using Reston.Pinata.Model.PengadaanRepository;

namespace Reston.Pinata.Model.PengadaanAsuransiRepository
{
    [Table("RKSAsuransiHeader", Schema = JimbisContext.PENGADAAN_SCHEMA_NAME)]
    public class RKSAsuransiHeader
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [ForeignKey("Pengadaan")]
        public Nullable<Guid> PengadaanId { get; set; }
        public Nullable<decimal> Total { get; set; }
        public Nullable<DateTime> CreateOn { get; set; }
        public Nullable<Guid> CreateBy { get; set; }
        public Nullable<DateTime> ModifiedOn { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public virtual ICollection<RKSAsuransiDetail> RKSDetails { get; set; }
        public virtual Pengadaan Pengadaan { get; set; }
    }

    [Table("RKSAsuransiDetail", Schema = JimbisContext.PENGADAAN_SCHEMA_NAME)]
    public class RKSAsuransiDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [ForeignKey("RKSAsuransiHeader")]
        public Nullable<Guid> RKSAsuransiHeaderId { get; set; }
        public string judul { get; set; }
        public Nullable<int> level { get; set; }
        public int? grup { get; set; }
        public Nullable<Guid> ItemId { get; set; }
        public string item { get; set; }
        public string satuan { get; set; }
        public Nullable<decimal> jumlah { get; set; }
        public Nullable<decimal> hps { get; set; }
        public string keterangan { get; set; }
        public virtual RKSHeader RKSAsuransiHeader { get; set; }
        public Nullable<DateTime> CreateOn { get; set; }
        public Nullable<Guid> CreateBy { get; set; }
        public Nullable<DateTime> ModifiedOn { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public  virtual ICollection<HargaKlarifikasiRekanan>  HargaKlarifikasiRekanan { get; set; }
        public virtual ICollection<HargaRekanan> HargaRekanan { get; set; }
    }

    [Table("RKSAsuransiHeaderTemplate", Schema = JimbisContext.PENGADAAN_SCHEMA_NAME)]
    public class RKSAsuransiHeaderTemplate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public String Title { get; set; }
        public String Description { get; set; }
        public KlasifikasiPengadaan Klasifikasi { get; set; }
        public String Region { get; set; }
        public Nullable<decimal> Total { get; set; }
        public Nullable<DateTime> CreateOn { get; set; }
        public Nullable<Guid> CreateBy { get; set; }
        public Nullable<DateTime> ModifiedOn { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public virtual ICollection<RKSAsuransiDetailTemplate> RKSAsuransiDetailTemplate { get; set; }
    }

    [Table("RKSAsuransiDetailTemplate", Schema = JimbisContext.PENGADAAN_SCHEMA_NAME)]
    public class RKSAsuransiDetailTemplate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [ForeignKey("RKSAsuransiHeaderTemplate")]
        public Nullable<Guid> RKSAsuransiHeaderTemplateId { get; set; }
        public string judul { get; set; }
        public Nullable<int> level { get; set; }
        public Nullable<int> group { get; set; }
        public Nullable<Guid> ItemId { get; set; }
        public string item { get; set; }
        public string satuan { get; set; }
        public Nullable<decimal> jumlah { get; set; }
        public Nullable<decimal> hps { get; set; }
        public string keterangan { get; set; }
        public virtual RKSAsuransiHeaderTemplate RKSAsuransiHeaderTemplate { get; set; }
        public Nullable<DateTime> CreateOn { get; set; }
        public Nullable<Guid> CreateBy { get; set; }
        public Nullable<DateTime> ModifiedOn { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
    }

}
