using Reston.Eproc.Model.Monitoring.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reston.Pinata.Model.JimbisModel;
//using Reston.Pinata.Model.PengadaanAsuransiRepository;

namespace Reston.Pinata.Model.PengadaanRepository
{
    [Table("Pengadaan", Schema = AppDbContext.PENGADAAN_SCHEMA_NAME)]
    public class Pengadaan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [MaxLength(255)]
        public string Judul { get; set; }
        [MaxLength(500)]
        public string Keterangan { get; set; }
        [MaxLength(50)]
        public string AturanPengadaan { get; set; }
        [MaxLength(50)]
        public string AturanBerkas { get; set; }
        [MaxLength(50)]
        public string AturanPenawaran { get; set; }
        [MaxLength(50)]
        public string MataUang { get; set; }
        [MaxLength(50)]
        public string PeriodeAnggaran { get; set; }
        [MaxLength(50)]
        public string JenisPembelanjaan { get; set; }
        public Nullable<Guid> HpsId { get; set; }
        public string TitleDokumenNotaInternal { get; set; }
        [MaxLength(50)]
        public string UnitKerjaPemohon { get; set; }
        public string TitleDokumenLain { get; set; }
        [MaxLength(50)]
        public string Region { get; set; }
        [MaxLength(50)]
        public string Provinsi { get; set; }
        [MaxLength(50)]
        public string KualifikasiRekan { get; set; }
        [MaxLength(50)]
        public string JenisPekerjaan { get; set; }
        public string NoCOA { get; set; }
        [MaxLength(500)]
        public string JumlahCOA { get; set; }
        [MaxLength(500)]
        public string Branch { get; set; }
        [MaxLength(500)]
        public string Department { get; set; }
        public Nullable<EStatusPengadaan> Status { get; set; }
        public Nullable<EGroupPengadaan> GroupPengadaan { get; set; }
        public string TitleBerkasRujukanLain { get; set; }
        public string NoPengadaan { get; set; }
        public Nullable<DateTime> CreatedOn { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<DateTime> ModifiedOn { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public Nullable<DateTime> TanggalMenyetujui { get; set; }
        public Nullable<Decimal> Pagu { get; set; }
        public Nullable<int> WorkflowId { get; set; }
        public Nullable<int> PengadaanLangsung { get; set; }
        public virtual ICollection<DokumenPengadaan> DokumenPengadaans { get; set; }
        public virtual ICollection<KandidatPengadaan> KandidatPengadaans { get; set; }
        public virtual ICollection<JadwalPengadaan> JadwalPengadaans { get; set; }
        public virtual ICollection<PersonilPengadaan> PersonilPengadaans { get; set; }
        public virtual ICollection<RKSHeader> RKSHeaders { get; set; }
        public virtual ICollection<BintangPengadaan> BintangPengadaans { get; set; }
        public virtual ICollection<MonitoringPekerjaan> MonitoringPekerjaans { get; set; }
        public virtual ICollection<JadwalPelaksanaan> JadwalPelaksanaans { get; set; }
        public virtual ICollection<PersetujuanPemenang> PersetujuanPemenangs { get; set; }
        public virtual ICollection<RencanaProyek> RencanaProyeks { get; set; }
        public virtual ICollection<BeritaAcara> BeritaAcaras { get; set; }
        public virtual ICollection<PemenangPengadaan> PemenangPengadaans { get; set; }
        public virtual ICollection<PersetujuanTahapan> PersetujuanTahapans { get; set; }
        public virtual ICollection<PersetujuanTerkait> PersetujuanTerkait { get; set; }
        
    }

    [Table("DokumenPengadaan", Schema = AppDbContext.PENGADAAN_SCHEMA_NAME)]
    public class DokumenPengadaan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [ForeignKey("Pengadaan")]
        public Nullable<Guid> PengadaanId { get; set; }
        [MaxLength(1000)]
        public string File { get; set; }
        [MaxLength(255)]
        public string ContentType { get; set; }
        [MaxLength(255)]
        public string Title { get; set; }
         [MaxLength(25)]
        public string NoDokumen { get; set; }
        public Nullable<DateTime> CreateOn { get; set; }
        public Nullable<Guid> CreateBy { get; set; }
        public Nullable<DateTime> ModifiedOn { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public Nullable<TipeBerkas> Tipe { get; set; }
        public Nullable<long> SizeFile { get; set; }
        [ForeignKey("Vendor")]
        public Nullable<int> VendorId { get; set; }

        public virtual Pengadaan Pengadaan { get; set; }
        public virtual Vendor Vendor { get; set; }
    }


    [Table("KandidatPengadaan", Schema = AppDbContext.PENGADAAN_SCHEMA_NAME)]
    public class KandidatPengadaan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [ForeignKey("Pengadaan")]
        public Nullable<Guid> PengadaanId { get; set; }
        [ForeignKey("Vendor")]
        public Nullable<int> VendorId { get; set; }
        public addKandidatType? addKandidatType { get; set; }
        public virtual Pengadaan Pengadaan { get; set; }
        public virtual Vendor Vendor { get; set; }
    }

    [Table("HistoryKandidatPengadaan", Schema = AppDbContext.PENGADAAN_SCHEMA_NAME)]
    public class HistoryKandidatPengadaan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Nullable<Guid> PengadaanId { get; set; }
        public Nullable<int> VendorId { get; set; }
        public addKandidatType? addKandidatType { get; set; }
    }

    public enum addKandidatType
    {
        PICADDED=1,VENDORSELFADDED=2
    }

    [Table("KualifikasiKandidat", Schema = AppDbContext.PENGADAAN_SCHEMA_NAME)]
    public class KualifikasiKandidat
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [ForeignKey("Pengadaan")]
        public Nullable<Guid> PengadaanId { get; set; }
        public string kualifikasi { get; set; }
        public virtual Pengadaan Pengadaan { get; set; }
    }

    [Table("JadwalPengadaan", Schema = AppDbContext.PENGADAAN_SCHEMA_NAME)]
    public class JadwalPengadaan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [ForeignKey("Pengadaan")]
        public Nullable<Guid> PengadaanId { get; set; }
        public string tipe { get; set; }
        public Nullable<DateTime> Mulai { get; set; }
        public Nullable<DateTime> Sampai { get; set; }
        public virtual Pengadaan Pengadaan { get; set; }
    }

    [Table("PersonilPengadaan", Schema = AppDbContext.PENGADAAN_SCHEMA_NAME)]
    public class PersonilPengadaan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [ForeignKey("Pengadaan")]
        public Nullable<Guid> PengadaanId { get; set; }
        public Nullable<Guid> PersonilId { get; set; }
        public string Nama { get; set; }
        public string Jabatan { get; set; }
        public string tipe { get; set; }
        public Nullable<int> isReady { get; set; }
        public virtual Pengadaan Pengadaan { get; set; }
    }

    [Table("RKSHeader", Schema = AppDbContext.PENGADAAN_SCHEMA_NAME)]
    public class RKSHeader
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
        public virtual ICollection<RKSDetail> RKSDetails { get; set; }
        public virtual Pengadaan Pengadaan { get; set; }
    }

    [Table("RKSDetail", Schema = AppDbContext.PENGADAAN_SCHEMA_NAME)]
    public class RKSDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [ForeignKey("RKSHeader")]
        public Nullable<Guid> RKSHeaderId { get; set; }
        public string judul { get; set; }
        public Nullable<int> level { get; set; }
        public int? grup { get; set; }
        public Nullable<Guid> ItemId { get; set; }
        public string item { get; set; }
        public string satuan { get; set; }
        public Nullable<decimal> jumlah { get; set; }
        public Nullable<decimal> hps { get; set; }
        public string keterangan { get; set; }
        public virtual RKSHeader RKSHeader { get; set; }
        public Nullable<DateTime> CreateOn { get; set; }
        public Nullable<Guid> CreateBy { get; set; }
        public Nullable<DateTime> ModifiedOn { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public  virtual ICollection<HargaKlarifikasiRekanan>  HargaKlarifikasiRekanan { get; set; }
        public virtual ICollection<HargaRekanan> HargaRekanan { get; set; }
    }

    [Table("RKSHeaderTemplate", Schema = AppDbContext.PENGADAAN_SCHEMA_NAME)]
    public class RKSHeaderTemplate
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
        public virtual ICollection<RKSDetailTemplate> RKSDetailTemplate { get; set; }
    }

    [Table("RKSDetailTemplate", Schema = AppDbContext.PENGADAAN_SCHEMA_NAME)]
    public class RKSDetailTemplate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [ForeignKey("RKSHeaderTemplate")]
        public Nullable<Guid> RKSHeaderTemplateId { get; set; }
        public string judul { get; set; }
        public Nullable<int> level { get; set; }
        public Nullable<int> group { get; set; }
        public Nullable<Guid> ItemId { get; set; }
        public string item { get; set; }
        public string satuan { get; set; }
        public Nullable<decimal> jumlah { get; set; }
        public Nullable<decimal> hps { get; set; }
        public string keterangan { get; set; }
        public virtual RKSHeaderTemplate RKSHeaderTemplate { get; set; }
        public Nullable<DateTime> CreateOn { get; set; }
        public Nullable<Guid> CreateBy { get; set; }
        public Nullable<DateTime> ModifiedOn { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
    }

    [Table("RiwayatPengadaan", Schema = AppDbContext.PENGADAAN_SCHEMA_NAME)]
    public class RiwayatPengadaan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [ForeignKey("Pengadaan")]
        public Nullable<Guid> PengadaanId { get; set; }
        public Nullable<DateTime> Waktu { get; set; }
        public EStatusPengadaan Status { get; set; }
        [MaxLength(1000)]
        public string Komentar { get; set; }
        public int Urutan { get; set; }
        public virtual Pengadaan Pengadaan { get; set; }
    }

    [Table("RiwayatDokumen", Schema = AppDbContext.PENGADAAN_SCHEMA_NAME)]
    public class RiwayatDokumen
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Nullable<Guid> UserId { get; set; }
        public Nullable<Guid> PengadaanId { get; set; }
        public Nullable<DateTime> ActionDate { get; set; }
        [MaxLength(500)]
        public String Comment { get; set; }
        [MaxLength(100)]
        public string Status { get; set; }
    }

    [Table("RiwayatMemo", Schema = AppDbContext.EMEMO_SCHEMA_NAME)]
    public class RiwayatMemo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Nullable<Guid> UserId { get; set; }
        public Nullable<Guid> EMemoId { get; set; }
        public Nullable<DateTime> CreateDate { get; set; }
        [MaxLength(500)]
        public String Version { get; set; }
        public string Content { get; set; }
    }

    [Table("MessagePengadaan", Schema = AppDbContext.PENGADAAN_SCHEMA_NAME)]
    public class MessagePengadaan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [ForeignKey("Pengadaan")]
        public Nullable<Guid> PengadaanId { get; set; }
        public Nullable<DateTime> Waktu { get; set; }
        public EStatusPengadaan Status { get; set; }
        [MaxLength(1000)]
        public string Message { get; set; }
        public int Urutan { get; set; }
        public Nullable<Guid> UserTo { get; set; }
        public Nullable<Guid> FromTo { get; set; }
        public virtual Pengadaan Pengadaan { get; set; }
    }

    [Table("BintangPengadaan", Schema = AppDbContext.PENGADAAN_SCHEMA_NAME)]
    public class BintangPengadaan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [ForeignKey("Pengadaan")]
        public Nullable<Guid> PengadaanId { get; set; }
        public Guid UserId { get; set; }
        public Nullable<int> StatusBintang { get; set; }
        public virtual Pengadaan Pengadaan { get; set; }
    }

    [Table("PelaksanaanAanwijzing", Schema = AppDbContext.PENGADAAN_SCHEMA_NAME)]
    public class PelaksanaanAanwijzing
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [ForeignKey("Pengadaan")]
        public Nullable<Guid> PengadaanId { get; set; }
        public Nullable<DateTime> Mulai { get; set; }
        public string IsiUndangan { get; set; }
        public virtual Pengadaan Pengadaan { get; set; }
    }

    [Table("KehadiranKandidatAanwijzing", Schema = AppDbContext.PENGADAAN_SCHEMA_NAME)]
    public class KehadiranKandidatAanwijzing
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [ForeignKey("Pengadaan")]
        public Nullable<Guid> PengadaanId { get; set; }
        public Nullable<int> VendorId { get; set; }
        public virtual Pengadaan Pengadaan { get; set; }
    }

    [Table("PelaksanaanSubmitPenawaran", Schema = AppDbContext.PENGADAAN_SCHEMA_NAME)]
    public class PelaksanaanSubmitPenawaran
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [ForeignKey("Pengadaan")]
        public Nullable<Guid> PengadaanId { get; set; }
        public Nullable<DateTime> Mulai { get; set; }
        public Nullable<DateTime> Sampai { get; set; }
        [ForeignKey("DokumenPengadaan")]
        public Nullable<Guid> DokomenPengadaanId { get; set; }
        public DokumenPengadaan DokumenPengadaan { get; set; }
        public virtual Pengadaan Pengadaan { get; set; }
    }

    [Table("PelaksanaanBukaAmplop", Schema = AppDbContext.PENGADAAN_SCHEMA_NAME)]
    public class PelaksanaanBukaAmplop
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [ForeignKey("Pengadaan")]
        public Nullable<Guid> PengadaanId { get; set; }
        public Nullable<DateTime> Mulai { get; set; }
        public Nullable<DateTime> Sampai { get; set; }
        [ForeignKey("DokumenPengadaan")]
        public Nullable<Guid> DokomenPengadaanId { get; set; }
        public DokumenPengadaan DokumenPengadaan { get; set; }
        public virtual Pengadaan Pengadaan { get; set; }
    }

    [Table("PersetujuanBukaAmplop", Schema = AppDbContext.PENGADAAN_SCHEMA_NAME)]
    public class PersetujuanBukaAmplop
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [ForeignKey("Pengadaan")]
        public Nullable<Guid> PengadaanId { get; set; }
        public Nullable<Guid> UserId { get; set; }
        public virtual Pengadaan Pengadaan { get; set; }
    }

    [Table("PelaksanaanPenilaianKandidat", Schema = AppDbContext.PENGADAAN_SCHEMA_NAME)]
    public class PelaksanaanPenilaianKandidat
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [ForeignKey("Pengadaan")]
        public Nullable<Guid> PengadaanId { get; set; }
        public Nullable<DateTime> Mulai { get; set; }
        public Nullable<DateTime> Sampai { get; set; }
        [ForeignKey("DokumenPengadaan")]
        public Nullable<Guid> DokomenPengadaanId { get; set; }
        public DokumenPengadaan DokumenPengadaan { get; set; }
        public virtual Pengadaan Pengadaan { get; set; }
    }

    [Table("PemenangPengadaan", Schema = AppDbContext.PENGADAAN_SCHEMA_NAME)]
    public class PemenangPengadaan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [ForeignKey("Pengadaan")]
        public Nullable<Guid> PengadaanId { get; set; }
        [ForeignKey("Vendor")]
        public Nullable<int> VendorId { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<DateTime> CreateOn { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public Nullable<DateTime> ModifiedOn { get; set; }
        public virtual Pengadaan Pengadaan { get; set; }
        public virtual Vendor Vendor { get; set; }
    }

    [Table("PelaksanaanKlarifikasi", Schema = AppDbContext.PENGADAAN_SCHEMA_NAME)]
    public class PelaksanaanKlarifikasi
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [ForeignKey("Pengadaan")]
        public Nullable<Guid> PengadaanId { get; set; }
        public Nullable<DateTime> Mulai { get; set; }
        public Nullable<DateTime> Sampai { get; set; }
        [ForeignKey("DokumenPengadaan")]
        public Nullable<Guid> DokomenPengadaanId { get; set; }
        public DokumenPengadaan DokumenPengadaan { get; set; }
        public virtual Pengadaan Pengadaan { get; set; }
    }

    [Table("JadwalPelaksanaan", Schema = AppDbContext.PENGADAAN_SCHEMA_NAME)]
    public class JadwalPelaksanaan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [ForeignKey("Pengadaan")]
        public Nullable<Guid> PengadaanId { get; set; }
        public Nullable<EStatusPengadaan> statusPengadaan { get; set; }
        public Nullable<DateTime> Mulai { get; set; }
        public Nullable<DateTime> Sampai { get; set; }
        public virtual Pengadaan Pengadaan { get; set; }
    }

    [Table("HargaRekanan", Schema = AppDbContext.PENGADAAN_SCHEMA_NAME)]
    public class HargaRekanan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [ForeignKey("RKSDetail")]
        public Nullable<Guid> RKSDetailId { get; set; }
        //[ForeignKey("RKSAsuransiDetail")]
        //public Nullable<Guid> RKSAsuransiDetailId { get; set; }
         [ForeignKey("Vendor")]
        public Nullable<int> VendorId { get; set; }
        public Nullable<decimal> harga { get; set; }
        public string hargaEncrypt { get; set; }
        public string keterangan { get; set; }
        public virtual RKSDetail RKSDetail { get; set; }
       // public virtual RKSAsuransiDetail RKSAsuransiDetail { get; set; }
        public virtual Vendor Vendor { get; set; }
    }

    [Table("PelaksanaanPemilihanKandidat", Schema = AppDbContext.PENGADAAN_SCHEMA_NAME)]
    public class PelaksanaanPemilihanKandidat
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [ForeignKey("Pengadaan")]
        public Nullable<Guid> PengadaanId { get; set; }
        public Nullable<int> VendorId { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public virtual Pengadaan Pengadaan { get; set; }
    }

    [Table("HargaKlarifikasiRekanan", Schema = AppDbContext.PENGADAAN_SCHEMA_NAME)]
    public class HargaKlarifikasiRekanan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [ForeignKey("RKSDetail")]
        public Nullable<Guid> RKSDetailId { get; set; }
        //[ForeignKey("RKSAsuransiDetail")]
        //public Nullable<Guid> RKSAsuransiDetailId { get; set; }
        public Nullable<int> VendorId { get; set; }
        public Nullable<decimal> harga { get; set; }
        public string keterangan { get; set; }
        public virtual RKSDetail RKSDetail { get; set; }
        //public virtual RKSAsuransiDetail RKSAsuransiDetail { get; set; }
    }

    [Table("HargaKlarifikasiLanLanjutan", Schema = AppDbContext.PENGADAAN_SCHEMA_NAME)]
    public class HargaKlarifikasiLanLanjutan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [ForeignKey("RKSDetail")]
        public Nullable<Guid> RKSDetailId { get; set; }
        //[ForeignKey("RKSAsuransiDetail")]
        //public Nullable<Guid> RKSAsuransiDetailId { get; set; }
        public Nullable<int> VendorId { get; set; }
        public Nullable<decimal> harga { get; set; }
        public string keterangan { get; set; }
        public virtual RKSDetail RKSDetail { get; set; }
        //public virtual RKSAsuransiDetail RKSAsuransiDetail { get; set; }
    }


    [Table("CatatanPengadaan", Schema = AppDbContext.PENGADAAN_SCHEMA_NAME)]
    public class CatatanPengadaan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Nullable<Guid> PengadaanId { get; set; }
        public Nullable<int> UserId { get; set; }
        public string Komentar { get; set; }
        public TipeCatatan tipeCatatan { get; set; }
    }

    [Table("KreteriaPembobotan", Schema = AppDbContext.PENGADAAN_SCHEMA_NAME)]
    public class KreteriaPembobotan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string NamaKreteria { get; set; }
        public Nullable<int> Bobot { get; set; }
    }

    [Table("PembobotanPengadaan", Schema = AppDbContext.PENGADAAN_SCHEMA_NAME)]
    public class PembobotanPengadaan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Nullable<Guid> PengadaanId { get; set; }
        public Nullable<Guid> KreteriaPembobotanId { get; set; }
        public Nullable<int> Bobot { get; set; }
    }

    [Table("PembobotanPengadaanVendor", Schema = AppDbContext.PENGADAAN_SCHEMA_NAME)]
    public class PembobotanPengadaanVendor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Nullable<Guid> PengadaanId { get; set; }
        public Nullable<int> VendorId { get; set; }
        public Nullable<Guid> KreteriaPembobotanId { get; set; }
        public Nullable<int> Nilai { get; set; }
    }

    [Table("NoDokumenGenerator", Schema = AppDbContext.PENGADAAN_SCHEMA_NAME)]
    public class NoDokumenGenerator
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string No { get; set; }
        public Nullable<TipeNoDokumen> tipe { get; set; }
        public Nullable<DateTime> CreateOn { get; set; }
        public Nullable<Guid> CreateBy { get; set; }
    }

    [Table("BeritaAcara", Schema = AppDbContext.PENGADAAN_SCHEMA_NAME)]
    public class BeritaAcara
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [ForeignKey("Pengadaan")]
        public Nullable<Guid> PengadaanId { get; set; }
        public Nullable<DateTime> tanggal { get; set; }
        public Nullable<TipeBerkas> Tipe { get; set; }
        public string NoBeritaAcara { get; set; }
        [ForeignKey("Vendor")]
        public Nullable<int> VendorId { get; set; }
        public virtual Pengadaan Pengadaan { get; set; }
        public virtual Vendor Vendor { get; set; }

    }

    [Table("ReportPengadaan", Schema = AppDbContext.PENGADAAN_SCHEMA_NAME)]
    public class ReportPengadaan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Nullable<Guid> PengadaanId { get; set; }
        public string Judul { get; set; }
        public string User { get; set; }
        public Nullable<decimal> hps { get; set; }
        public Nullable<decimal> realitas { get; set; }
        public Nullable<decimal> efisiensi { get; set; }
        public string Pemenang { get; set; }
        public Nullable<DateTime> Aanwjzing { get; set; }
        public Nullable<DateTime> PembukaanAmplop { get; set; }
        public Nullable<DateTime> Klasrifikasi { get; set; }
        public Nullable<DateTime> Scoring { get; set; }
        public Nullable<DateTime> NotaPemenang { get; set; }
        public Nullable<DateTime> SPK { get; set; }
    }

    [Table("PembatalanPengadaan", Schema = AppDbContext.PENGADAAN_SCHEMA_NAME)]
    public class PembatalanPengadaan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Nullable<Guid> PengadaanId { get; set; }
        public string Keterangan { get; set; }
        public DateTime CreateOn { get; set; }
        public Guid CreateBy { get; set; }
    }

    [Table("PenolakanPengadaan", Schema = AppDbContext.PENGADAAN_SCHEMA_NAME)]
    public class PenolakanPengadaan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Nullable<Guid> PengadaanId { get; set; }
        public string Keterangan { get; set; }
        public Nullable<int> status { get; set; }
        public DateTime CreateOn { get; set; }
        public Guid CreateBy { get; set; }
    }


    [Table("PersetujuanPemenang", Schema = AppDbContext.PENGADAAN_SCHEMA_NAME)]
    public class PersetujuanPemenang
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Nullable<Guid> PengadaanId { get; set; }
        public StatusPengajuanPemenang Status { get; set; }
        public string Note { get; set; }
        public Nullable<int> WorkflowId { get; set; }
        public  Nullable<DateTime> CreatedOn { get; set; }
        public  Nullable<Guid> CreatedBy { get; set; }
        public  Nullable<DateTime> ModifiedOn { get; set; }
        public  Nullable<Guid> ModifiedBy { get; set; }
        public virtual Pengadaan Pengadaan { get; set; }
    }

    [Table("PersetujuanTahapan", Schema = AppDbContext.PENGADAAN_SCHEMA_NAME)]
    public class PersetujuanTahapan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [ForeignKey("Pengadaan")]
        public Nullable<Guid> PengadaanId { get; set; }
        public Guid UserId { get; set; }
        public StatusTahapan Status{ get; set; }
        public EStatusPengadaan StatusPengadaan { get; set; }
        public Nullable<DateTime> CreatedOn { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<DateTime> ModifiedOn { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public virtual Pengadaan Pengadaan { get; set; }
    }

    [Table("LewatTahapan", Schema = AppDbContext.PENGADAAN_SCHEMA_NAME)]
    public class LewatTahapan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [ForeignKey("Pengadaan")]
        public Guid PengadaanId { get; set; }
        public EStatusPengadaan Status { get; set; }
        public bool Tambah { get; set; }
        public Nullable<DateTime> CreatedOn { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<DateTime> ModifiedOn { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public virtual Pengadaan Pengadaan { get; set; }
    }

    [Table("PersetujuanTerkait", Schema = AppDbContext.PENGADAAN_SCHEMA_NAME)]
    public class PersetujuanTerkait
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [ForeignKey("Pengadaan")]
        public Guid PengadaanId { get; set; }
        public Guid UserId { get; set; }
        public int setuju { get; set; }
        public string CommentPersetujuanTerkait { get; set; }
        public virtual Pengadaan Pengadaan { get; set; }
    }

    [Table("TenderScoringHeader", Schema = AppDbContext.PENGADAAN_SCHEMA_NAME)]
    public class TenderScoringHeader
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid PengadaanId { get; set; }
        public int VendorId { get; set; }
        public decimal? Total { get; set; }
        public decimal? Average { get; set; }
        public virtual ICollection<TenderScoringDetail> TenderScoringDetails { get; set; }
    }

    [Table("TenderScoringBobot", Schema = AppDbContext.PENGADAAN_SCHEMA_NAME)]
    public class TenderScoringBobot
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid PengadaanId { get; set; }
        public string Code { get; set; }
        public int Bobot { get; set; }
    }

    [Table("TenderScoringDetail", Schema = AppDbContext.PENGADAAN_SCHEMA_NAME)]
    public class TenderScoringDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid TenderScoringHeaderId { get; set; }
        public string Code { get; set; }
        public decimal? TotalAllUser { get; set; }
        public decimal? AverageAllUser { get; set; }
    }

    [Table("TenderScoringUser", Schema = AppDbContext.PENGADAAN_SCHEMA_NAME)]
    public class TenderScoringUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid TenderScoringDetailId { get; set; }
        public Guid UserId { get; set; }
        public int? Score { get; set; }
    }
    
    [Table("Budgeting", Schema = AppDbContext.PENGADAAN_SCHEMA_NAME)]
    public class Budgeting
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(255)]
        public string Branch { get; set; }
        [MaxLength(255)]
        public string Department { get; set; }
        [MaxLength(255)]
        public string Description { get; set; }
        [MaxLength(255)]
        public string COA { get; set; }
        [MaxLength(255)]
        public string Year { get; set; }
        [MaxLength(255)]
        public string Month { get; set; }
        public Nullable<decimal> BudgetAmount { get; set; }
        public Nullable<decimal> BudgetUsage { get; set; }
        public Nullable<decimal> BudgetLeft { get; set; }
        public Nullable<decimal> BudgetReserved { get; set; }
        public Nullable<int> Version { get; set; }
        [MaxLength(255)]
        public string Jenis { get; set; }
    }
    
    [Table("DokumenBudget", Schema = AppDbContext.PENGADAAN_SCHEMA_NAME)]
    public class DokumenBudget
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [MaxLength(1000)]
        public string File { get; set; }
        [MaxLength(255)]
        public string ContentType { get; set; }
        public Nullable<DateTime> CreateOn { get; set; }
        public Nullable<Guid> CreateBy { get; set; }
        public Nullable<DateTime> ModifiedOn { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public Nullable<long> SizeFile { get; set; }
        public Nullable<int> NoDokumen { get; set; }
        public Nullable<DateTime> TglDokumen { get; set; }
        public string ProcessId { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<int> Version { get; set; }
        public string Year { get; set; }
        public string Jenis { get; set; }

    }

    [Table("BudgetingPengadaanHeader", Schema = AppDbContext.PENGADAAN_SCHEMA_NAME)]
    public class BudgetingPengadaanHeader
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid PengadaanId { get; set; }
        public Nullable<DateTime> CreatedOn { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<DateTime> ModifiedOn { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public decimal TotalInput { get; set; }
        public List<BudgetingPengadaanDetail> BudgetingPengadaanDetails { get; set; }
    }

    [Table("BudgetingPengadaanDetail", Schema = AppDbContext.PENGADAAN_SCHEMA_NAME)]
    public class BudgetingPengadaanDetail
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("BudgetingPengadaanHeader")]
        public Guid BudgetingPengadaanId { get; set; }

        public BudgetingPengadaanHeader BudgetingPengadaanHeader { get; set; }

        [MaxLength(255)]
        public string Branch { get; set; }

        [MaxLength(255)]
        public string Department { get; set; }

        [MaxLength(255)]
        public string NoCOA { get; set; }

        public decimal Input { get; set; }

        public Nullable<DateTime> CreatedOn { get; set; }

        public Nullable<Guid> CreatedBy { get; set; }

        public Nullable<DateTime> ModifiedOn { get; set; }

        public Nullable<Guid> ModifiedBy { get; set; }

        [MaxLength(255)]
        public string Month { get; set; }

        public string Year { get; set; }

        public string BudgetType { get; set; }

        public int? Version { get; set; }
    }
    
    public class VWBudgeting
    {
        public int Id { get; set; }
        public Guid PengadaanId { get; set; }
        public string Branch { get; set; }
        public string Department { get; set; }
        public string Description { get; set; }
        public string COA { get; set; }
        public string NoCOA { get; set; }
        public string Year { get; set; }
        public string Month { get; set; }
        public Nullable<decimal> BudgetAmount { get; set; }
        public Nullable<decimal> BudgetUsage { get; set; }
        public Nullable<decimal> BudgetLeft { get; set; }
        public Nullable<decimal> BudgetReserved { get; set; }
        public Nullable<decimal> BudgetLeftTotal { get; set; }
        public Nullable<decimal> Inputbudget { get; set; }
        public Nullable<int> Version { get; set; }
        public Nullable<decimal> TotalInput { get; set; }
        public string BudgetType { get; set; }
        public Nullable<decimal> BudgetOnProcess { get; set; }
        public Nullable<decimal> SisaBudgetOnProcess { get; set; }
    }
    
    public class VWDokumenBudget
    {
        public Guid Id { get; set; }
        public string File { get; set; }
        public string ContentType { get; set; }
        public Nullable<DateTime> CreateOn { get; set; }
        public Nullable<Guid> CreateBy { get; set; }
        public Nullable<DateTime> ModifiedOn { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public Nullable<long> SizeFile { get; set; }
        public Nullable<int> NoDokumen { get; set; }
        public Nullable<DateTime> TglDokumen { get; set; }
        public string ProcessId { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<int> Version { get; set; }
        public string Year { get; set; }
        public string Jenis { get; set; }
        public string Uploader { get; set; }
        public string LastYear { get; set; }
        public Nullable<int> IsIdle { get; set; }
    }

    public class VWPersetujuanTerkait
    {
        public Guid Id { get; set; }
        public Guid PengadaanId { get; set; }
        public Guid UserId { get; set; }
        public string Nama { get; set; }
        public int setuju { get; set; }
        public string disposisi { get; set; }
        public int isthismine { get; set; }
        [MaxLength(500)]
        public string CommentPersetujuanTerkait { get; set; }
    }

    public class VWLewatTahapan
    {
        public Guid Id { get; set; }
        public Guid PengadaanId { get; set; }
        public EStatusPengadaan Status { get; set; }
        public bool Tambah { get; set; }
        public Nullable<DateTime> CreatedOn { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<DateTime> ModifiedOn { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
    }


    public enum StatusTahapan
    {
       Requested, Approved
    }
    public enum EStatusPengadaan
    {
        DRAFT=0, AJUKAN=1, DISETUJUI=2, AANWIJZING=3,
        SUBMITPENAWARAN=4, BUKAAMPLOP=5, KLARIFIKASI=7,
        KLARIFIKASILANJUTAN=12, PENILAIAN=6, PEMENANG=8, ARSIP=9, DITOLAK=10, DIBATALKAN=11
    }

    public enum EStatusPengadaanVendor
    {
        SUBMITPENAWARAN, KLARIFIKASI, DITOLAK
    }

    public enum EGroupPengadaan
    {
        PERLUPERHATIAN, DALAMPELAKSANAAN, BELUMTERJADWAL, ARSIP, ALL
    }

    public enum TipeBerkas
    {
        NOTA, DOKUMENLAIN, BerkasRujukanLain, BeritaAcaraAanwijzing, BeritaAcaraSubmitPenawaran, BeritaAcaraBukaAmplop, BeritaAcaraPenilaian, BeritaAcaraKlarifikasi, BeritaAcaraPenentuanPemenang, BerkasRekanan, BerkasRekananKlarifikasi, LembarDisposisi, SuratPerintahKerja, BeritaAcaraPendaftaran, DraftPKS, FinalLegalPks, AssignedPks, BeritaAcaraKlarifikasiLanjutan, BerkasRekananKlarifikasiLanjutan
        ,SURATKALAH, SURATMENANG
    }

    public enum TipeCatatan
    {
        Penilaian, Klarifikasi
    }

    public enum TipeNoDokumen
    {
        PENGADAAN, BERITAACARA, NOTA, SPK,PO,KALAH,MEANANG
    }

    public enum KlasifikasiPengadaan
    {
        SIPIL,NONSIPIL
    }

    public enum StatusPengajuanPemenang
    {
        BELUMDIAJUKAN,PENDING,APPROVED,REJECTED
    }

    public class DataTableBudgeting
    {
        public int draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public List<VWBudgeting> data { get; set; }
    }

    [Table("TenderScoringPenilai", Schema = AppDbContext.PENGADAAN_SCHEMA_NAME)]
    public class TenderScoringPenilai
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid TenderScoringHeaderId { get; set; }
        public Guid PengadaanId { get; set; }
        public int VendorId { get; set; }
        public Guid UserId { get; set; }
    }

    [Table("ApprisalWorksheet", Schema = AppDbContext.VENDOR_SCHEMA_NAME)]
    public class ApprisalWorksheet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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

    [Table("ApprisalWorksheetDetail", Schema = AppDbContext.VENDOR_SCHEMA_NAME)]
    public class ApprisalWorksheetDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid ApprisalWorksheetId { get; set; }
        public string QuestionCode { get; set; }
        public int Weight { get; set; }
    }

    [Table("ApprisalWorksheetResponse", Schema = AppDbContext.VENDOR_SCHEMA_NAME)]
    public class ApprisalWorksheetResponse
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid ApprisalWorksheetId { get; set; }
        public Guid AppriserUserId { get; set; }
        public Nullable<DateTime> CreatedOn { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<DateTime> UpdatedOn { get; set; }
        public Nullable<Guid> UpdatedBy { get; set; }
    }

    [Table("ApprisalWorksheetResposeDetail", Schema = AppDbContext.VENDOR_SCHEMA_NAME)]
    public class ApprisalWorksheetResposeDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid ApprisalWorksheetResposeId { get; set; }
        public string QuestionCode { get; set; }
        public int? Score { get; set; }
    }

    [Table("Sanksi", Schema = AppDbContext.VENDOR_SCHEMA_NAME)]
    public class Sanksi
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
    }
}
