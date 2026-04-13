using Reston.Pinata.Model.JimbisModel;
using Reston.Pinata.Model.Asuransi;
using Reston.Pinata.Model.PengadaanRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reston.Pinata.Model.Repository
{
    public class ProdukRepo : IProdukRepo
    {
        public const string KATEGORI_TAMPLATE_STRING = "TEMPLATE";

        AppDbContext ctx;
        public ProdukRepo(AppDbContext j) {
            ctx = j;
            ctx.Configuration.LazyLoadingEnabled = true;
        }

        public Produk GetProduk(int id){
            return ctx.Produks.Find(id);
        }

        public void DeleteKategori(KategoriSpesifikasi d)
        {
            ctx.KategoriSpesifikasis.Remove(d);
            ctx.SaveChanges();
        }

        public List<RiwayatHarga> GetHargaProdukRegion(int id, string region)
        {
            Produk p = ctx.Produks.Find(id);

            return p.RiwayatHarga.Where(x=>(region==null || x.Region == region)).ToList();
        }

        public List<Produk> GetAllProduk()
        {
            return ctx.Produks.ToList();
        }

        public List<Produk> GetProduks(string name = null, string region = null, string kategori = null,string klasifikasi=null) {
            //return ctx.Produks.Where(x => (name==null || x.Nama == name)).ToList();
            var Klasifikasi =(KlasifikasiPengadaan) Convert.ToInt32(klasifikasi);
            return (from a in ctx.Produks
                    where (name == null || a.Nama.ToLower().Contains(name.ToLower())) 
                        //&& (region== "" || b.Region == region)
                        && (kategori== null|| a.KategoriSpesifikasi.Nama == kategori) &&  (klasifikasi == null || a.Klasifikasi == Klasifikasi)
                    select a
                         ).ToList();

        }

        public void SaveProduk(Produk p) {           
            ctx.Produks.Add(p);
            ctx.SaveChanges();
        }

        public void DeleteProduk(int id) {
            Produk p = ctx.Produks.Find(id);
            if (p != null) {
                ctx.RiwayatHargas.RemoveRange(p.RiwayatHarga);
                ctx.AtributSpesifikasis.RemoveRange(p.KategoriSpesifikasi.AtributSpesifikasi);
                ctx.KategoriSpesifikasis.Remove(p.KategoriSpesifikasi);
                ctx.Produks.Remove(p);
                ctx.SaveChanges();
            }
        }

        public void Save() {
            ctx.SaveChanges();
        }

        //public KategoriSpesifikasi SaveDeleteKategori()
        //{
        //    return ctx.KategoriSpesifikasis.Where(x => x.IsDeleted);

        //}

        public void SaveKategori(KategoriSpesifikasi k) {
            ctx.KategoriSpesifikasis.Add(k);
            ctx.SaveChanges();
        }

        public void AddRiwayatHarga(RiwayatHarga r) {
            ctx.RiwayatHargas.Add(r);
            ctx.SaveChanges();
        }

        public List<RiwayatHarga> GetRiwayatHarga(int id, string region) {
            return ctx.Produks.Find(id).RiwayatHarga.Where(x=>region==null || x.Region == region).ToList();
        }

        public KategoriSpesifikasi GetKategoriSpesifikasiByProduk(int id)
        {
            return ctx.Produks.Find(id).KategoriSpesifikasi;
        }

        public KategoriSpesifikasi GetKategoriSpesifikasi(int id) {
            return ctx.KategoriSpesifikasis.Find(id);
        }

        public List<KategoriSpesifikasi> GetAllKategoriMaster()
        {
            return ctx.KategoriSpesifikasis.ToList();
        }

        public List<KategoriSpesifikasi> GetAllKategori()
        {

            var ls = ctx.KategoriSpesifikasis.Where(x => x.Nama != null).Distinct();
            ls = ls.OrderBy(x => x.Nama);
            return ls.ToList();
        }

        public List<KategoriSpesifikasi> GetAllKategoriException()
        {

            var ls = ctx.KategoriSpesifikasis.Where(x => x.IsDeleted != 1).Distinct();
            ls = ls.OrderBy(x => x.Nama);
            return ls.ToList();
        }

        public List<KategoriSpesifikasi> GetAllKategoriException2()
        {

            var ls = ctx.KategoriSpesifikasis.Where(x => x.IsDeleted != 1 && x.Deskripsi == "TEMPLATE").Distinct();
            ls = ls.OrderBy(x => x.Nama);
            return ls.ToList();
        }

        public List<KategoriSpesifikasi> GetTemplateKategoriSpesifikasi() {
            return ctx.KategoriSpesifikasis.Where(x=>x.Deskripsi.Equals(KATEGORI_TAMPLATE_STRING)).ToList();
        }

        public List<KategoriSpesifikasi> GetTemplateKategoriSpesifikasis()
        {
            return ctx.KategoriSpesifikasis.Where(x => x.Deskripsi.Equals(KATEGORI_TAMPLATE_STRING) && x.IsDeleted != 1).OrderBy(x=>x.Nama).ToList();
            //var ls = ctx.KategoriSpesifikasis.Where(x => x.Deskripsi.Equals(KATEGORI_TAMPLATE_STRING) && x => x.IsDeleted != 1).ToList();
            //ls = ls.OrderBy(x => x.Nama);
            //return ls.ToList();
        }

        public List<AtributSpesifikasi> GetDaftarAtributSpesifikasi(int id) {
            return ctx.KategoriSpesifikasis.Find(id).AtributSpesifikasi.ToList();
        }
    }

    public class AsuransiRepo : IAsuransiRepo
    {
        

        AppDbContext ctx;
        public AsuransiRepo(AppDbContext j)
        {
            ctx = j;
            ctx.Configuration.LazyLoadingEnabled = true;
        }

        public void SaveTarifAsuransi(InsuranceTarifTemplate p)
        {
            ctx.InsuranceTarifTemplates.Add(p);
            ctx.SaveChanges();
        }
        public void DeleteTarifAsuransi(int id)
        {
            InsuranceTarifTemplate p = ctx.InsuranceTarifTemplates.Find(id);
            if (p != null)
            {
                ctx.InsuranceTarifTemplates.Remove(p);
                ctx.SaveChanges();
            }
        }

        public void Save()
        {
            ctx.SaveChanges();
        }

    }
}
