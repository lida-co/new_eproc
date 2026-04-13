using Reston.Pinata.Model.JimbisModel;
using Reston.Pinata.Model.Asuransi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reston.Pinata.Model.Repository
{
    public interface IProdukRepo
    {
        Produk GetProduk(int id);
        List<RiwayatHarga> GetHargaProdukRegion(int id, string region);
        List<Produk> GetAllProduk();
        List<KategoriSpesifikasi> GetAllKategori();
        List<KategoriSpesifikasi> GetAllKategoriException();
        List<KategoriSpesifikasi> GetAllKategoriException2();
        List<Produk> GetProduks(string name = null, string region = null, string kategori = null,string klasifikasi=null);

        void SaveProduk(Produk p);

        void DeleteProduk(int id);

        void DeleteKategori(KategoriSpesifikasi d);

        void SaveKategori(KategoriSpesifikasi k);

        void AddRiwayatHarga(RiwayatHarga r);

        //KategoriSpesifikasi SaveDeleteKategori(bool? IsDeleted );

        void Save();

        List<RiwayatHarga> GetRiwayatHarga(int id, string region);

        KategoriSpesifikasi GetKategoriSpesifikasi(int id);

        KategoriSpesifikasi GetKategoriSpesifikasiByProduk(int id);

        List<KategoriSpesifikasi> GetTemplateKategoriSpesifikasi();

        List<AtributSpesifikasi> GetDaftarAtributSpesifikasi(int id);

        List<KategoriSpesifikasi> GetTemplateKategoriSpesifikasis();
    }

    public interface IAsuransiRepo
    {
        //List<InsuranceTarifTemplate> GetAllInsuranceTarifTemplate();
        void SaveTarifAsuransi(InsuranceTarifTemplate p);
        void DeleteTarifAsuransi(int id);
        void Save();
    }
}
