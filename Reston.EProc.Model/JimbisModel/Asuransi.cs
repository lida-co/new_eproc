//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Reston.Pinata.Model.PengadaanRepository;

//namespace Reston.Pinata.Model.Asuransi
//{
//    [Table("Benefit", Schema = AppDbContext.CATALOG_SCHEMA_NAME)]
//    public class Benefit
//    {
//        [Key]
//        public int Id { get; set; }

//        [MaxLength(150)]
//        [Index]
//        public string Nama { get; set; }

//        [MaxLength(255)]
//        public string Deskripsi { get; set; }

//        //public KlasifikasiPengadaan Klasifikasi { get; set; }

//        public virtual ICollection<RiwayatUP> RiwayatUP { get; set; }
//        public virtual ICollection<RiwayatRate> RiwayatRate { get; set; }
//    }

//    [Table("RiwayatUP", Schema = AppDbContext.CATALOG_SCHEMA_NAME)]
//    public class RiwayatUP
//    {
//        [Key]
//        public int Id { get; set; }

//        public DateTime Tanggal { get; set; }

//        public decimal UangPertanggungan { get; set; }

//        [MaxLength(10)]
//        public string Currency { get; set; }

//        public string Region { get; set; }

//        [MaxLength(500)]
//        public string Sumber { get; set; }

//        [MaxLength(50)]
//        public string User { get; set; }
//    }

//    [Table("RiwayatRate", Schema = AppDbContext.CATALOG_SCHEMA_NAME)]
//    public class RiwayatRate
//    {
//        [Key]
//        public int Id { get; set; }

//        public DateTime Tanggal { get; set; }

//        public decimal Rate { get; set; }

//        [MaxLength(10)]
//        public string Currency { get; set; }

//        public string Region { get; set; }

//        [MaxLength(500)]
//        public string Sumber { get; set; }

//        [MaxLength(50)]
//        public string User { get; set; }
//    }

//    [Table("KategoriSpesifikasiAsuransi", Schema = AppDbContext.CATALOG_SCHEMA_NAME)]
//    public class KategoriSpesifikasiAsuransi
//    {
//        [Key]
//        public int Id { get; set; }

//        [MaxLength(150)]
//        public string Nama { get; set; }

//        [MaxLength(255)]
//        public string Deskripsi { get; set; }

//        public KategoriSpesifikasiAsuransi ParentKategori { get; set; }
//    }
    
//}
