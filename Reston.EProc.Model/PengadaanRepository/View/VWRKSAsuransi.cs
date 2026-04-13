using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reston.Pinata.Model.PengadaanAsuransiRepository.View
{
    public class VWRKSAsuransi
    {
        public string Judul { get; set; }
        public Nullable<Guid> pengadaanId { get; set; }
        public List<VWRKSAsuransiDetail> VWRKSAsuransiDetails { get; set; }
    }

    public class VWRKSAsuransiDetail
    {
        public Nullable<Guid> Id { get; set; }
        public Nullable<Guid> RKSHeaderId { get; set; }
        public Nullable<Guid> ItemId { get; set; }
        public string judul { get; set; }
        public Nullable<int> level { get; set; }
        public int? grup { get; set; }
        public string item { get; set; }
        public string satuan { get; set; }
        public Nullable<decimal> jumlah { get; set; }
        public Nullable<decimal> hps { get; set; }
        public Nullable<decimal> total { get; set; }
        public string keterangan { get; set; }
    }

    public class VWRKSAsuransiDetailRekanan
    {
        public Nullable<Guid> Id { get; set; }
        public Nullable<Guid> RKSAsuransiHeaderId { get; set; }
        public Nullable<Guid> ItemId { get; set; }
        public Nullable<Guid> HargaRekananId { get; set; }
        public string item { get; set; }
        public string judul { get; set; }
        public string keteranganItem { get; set; }
        public string satuan { get; set; }
        public Nullable<int> level { get; set; }
        public Nullable<int> grup { get; set; }
        public Nullable<decimal> harga { get; set; }
        public string hargaEncript { get; set; }
        public string keterangan { get; set; }
        public Nullable<decimal> jumlah { get; set; }
    }

    public class item
    {
        public Nullable<Guid> Id { get; set; }
        public Nullable<decimal> jumlah { get; set; }
        public Nullable<decimal> harga { get; set; }
        public string Keterangan { get; set; }
        public int? grup { get; set; }
        public subtotal subtotal { get; set; }
        public int isTotal { get; set; }
    }

    public class subtotal
    {
        public int? rksGroup { get; set; }
        public decimal? totalGroup { get; set; }
    }

    public class VWRKSAsuransiTemplate
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public String  Deskripsi { get; set; }
        public String Region { get; set; }
        public List<VWRKSAsuransiDetailTemplate> VWRKSAsuransiDetails { get; set; }
    }

    public class DataTable
    {
        public int draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public List<VWRKSAsuransiDetail> data { get; set; }
    }

    public class DataTableRksAsuransiTemplate
    {
        public int draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public List<VWRKSAsuransiTemplate> data { get; set; }
    }

    public class DataTableRksAsuransiDetailTemplate
    {
        public int draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public List<VWRKSAsuransiDetailTemplate> data { get; set; }
    }

    public class VWRKSAsuransiDetailTemplate
    {
        public Guid Id { get; set; }
        public Nullable<Guid> RKSAsuransiHeaderTemplateId { get; set; }
        public Nullable<Guid> ItemId { get; set; }
        public string item { get; set; }
        public string judul { get; set; }
        public Nullable<int> level{ get; set; }
        public Nullable<int> group { get; set; }
        public string satuan { get; set; }
        public Nullable<decimal> jumlah { get; set; }
        public Nullable<decimal> hps { get; set; }
        public Nullable<decimal> total { get; set; }
        public string keterangan { get; set; }
        public Nullable<DateTime> CreateOn { get; set; }
        public Nullable<Guid> CreateBy { get; set; }
        public Nullable<DateTime> ModifiedOn { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
    }
}
