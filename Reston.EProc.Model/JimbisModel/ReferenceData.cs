using Reston.Eproc.Model.Monitoring.Entities;
using Reston.Pinata.Model.PengadaanRepository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reston.Pinata.Model.JimbisModel
{
    [Table("ReferenceData", Schema=AppDbContext.MASTER_SCHEMA_NAME)]
    public class ReferenceData
    {
        public const int QUALIIFIER_MAX_LENGTH = 100;
        public const int CODE_MAX_LENGTH = 30;

        private const string QUALIFIER_CODE_INDEX_NAME = "IndexQualifierCode";

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(QUALIIFIER_MAX_LENGTH)]
        [Index(QUALIFIER_CODE_INDEX_NAME, Order = 1, IsUnique = true)]
        public string Qualifier { get; set; }

        [Required]
        [MaxLength(CODE_MAX_LENGTH)]
        [Index(QUALIFIER_CODE_INDEX_NAME, Order = 2, IsUnique = true)]
        public string Code { get; set; }

        [Required]
        [MaxLength(500)]
        public string LocalizedName { get; set; }

        [MaxLength(500)]
        public string LocalizedDesc { get; set; }

        [MaxLength(256)]
        public string StringAttr1 { get; set; }
        [MaxLength(256)]
        public string StringAttr2 { get; set; }
        [MaxLength(256)]
        public string StringAttr3 { get; set; }

        public Nullable<int> IntAttr1 { get; set; }
        public Nullable<int> IntAttr2 { get; set; }
        public Nullable<int> IntAttr3 { get; set; }

        public Nullable<bool> FlagAttr1 { get; set; }
        public Nullable<bool> FlagAttr2 { get; set; }
        public Nullable<bool> FlagAttr3 { get; set; }
        public virtual ICollection<RencanaProyek> RencanaProyeks { get; set; }
        public virtual ICollection<PemenangPengadaan> PemenangPengadaans { get; set; }
        public virtual ICollection<PenilaianVendorHeader> PenilaianVendorHeaders { get; set; }
        public virtual ICollection<PenilaianVendorDetail> PenilaianVendorDetails { get; set; }
    }

    //fchr

    public class AddBankRequest
    {
        public string Code { get; set; }
        public string Nama { get; set; }
        public string Deskripsi { get; set; }
    }

    public class AddQualifierInsBenefit
    {
        public string Code { get; set; }
        public string LocalizedName { get; set; }
        public string LocalizedDesc { get; set; }
        public string StringAttr1 { get; set; }
    }

    public class AddQUALIFIER_INS_REGION
    {
        public string Code { get; set; }
        public string LocalizedName { get; set; }
        public string LocalizedDesc { get; set; }
        public string StringAttr1 { get; set; }
    }

    public class AddDukcapilKota
    {
        public string Code { get; set; }
        public string LocalizedName { get; set; }
        public string LocalizedDesc { get; set; }
        public string StringAttr1 { get; set; }
    }

    public class AddDukcapilKecamatan
    {
        public string Code { get; set; }
        public string LocalizedName { get; set; }
        public string LocalizedDesc { get; set; }
        public string StringAttr1 { get; set; }
    }

    public class AddDukcapilNegara
    {
        public string code { get; set; }
        public string nama { get; set; }
        public string deskripsi { get; set; }
    }

    public class AddDukcapilProvinsi
    {
        public string code { get; set; }
        public string nama { get; set; }
        public string deskripsi { get; set; }
    }

    public class AddPertanyaan
    {
        public string code { get; set; }
        public string nama { get; set; }
        public string deskripsi { get; set; }
    }

    public class AddSatuan
    {
        public string code { get; set; }
        public string nama { get; set; }
        public string deskripsi { get; set; }
    }

    public class AddRegion
    {
        public string code { get; set; }
        public string nama { get; set; }
        public string deskripsi { get; set; }
    }

    public class AddCurrency
    {
        public string code { get; set; }
        public string nama { get; set; }
        public string deskripsi { get; set; }
    }

    public class AddPenilaian
    {
        public string code { get; set; }
        public string nama { get; set; }
        public string deskripsi { get; set; }
    }

    public class AddUnitKerja
    {
        public string code { get; set; }
        public string nama { get; set; }
        public string deskripsi { get; set; }
    }

    public class AddPeriodeAnggaran
    {
        public string code { get; set; }
        public string nama { get; set; }
        public string deskripsi { get; set; }
    }

    public class AddQUALIFIER_INS_BENEF_TYPE
    {
        public string code { get; set; }
        public string nama { get; set; }
        public string deskripsi { get; set; }
    }

    public class DeleteRequest
    {
        public int Id { get; set; }
    }

    //end fchr

    [Table("Branch", Schema = AppDbContext.MASTER_SCHEMA_NAME)]
    public class MasterBranch
    {
        [Key]
        public int Branch_Id { get; set; }

        [MaxLength(50)]
        public string Branch_Code { get; set; }

        [MaxLength(255)]
        public string Branch_Name { get; set; }

        public Nullable<int> FK_Prov_Id { get; set; }
    }

    [Table("Department", Schema = AppDbContext.MASTER_SCHEMA_NAME)]
    public class MasterDepartment
    {
        [Key]
        public int Department_Id { get; set; }

        [MaxLength(50)]
        public string Department_Code { get; set; }

        [MaxLength(255)]
        public string Department_Name { get; set; }
    }

    [Table("BranchDepartmentRelationship", Schema = AppDbContext.MASTER_SCHEMA_NAME)]
    public class MasterBranchDepartmentRelationship
    {
        [Key]
        public int Id { get; set; }

        public int FK_Branch_Id { get; set; }

        public int FK_Department_Id { get; set; }
    }
}
