
using Reston.Pinata.Model.JimbisModel;
using System;
using System.Collections.Generic;
using Reston.Pinata.Model.PengadaanRepository.View;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reston.Pinata.Model.Repository
{
    public class VendorRepo : IVendorRepo
    {
        AppDbContext ctx;
        public VendorRepo(AppDbContext j) {
            ctx = j;
            ctx.Configuration.LazyLoadingEnabled = true;
        }

        public Vendor GetVendor(int id){
            Vendor v = ctx.Vendors.Find(id);
            if (v != null) {
                return v;
            }
            return null;
        }

        public Vendor GetVendorByUser(Guid id) {
            Vendor v = ctx.Vendors.Where(x => x.Owner == id).FirstOrDefault();
            if (v != null) return v;
            return null;
        }

        public List<Vendor> GetVendors(ETipeVendor tipe, EStatusVendor status, int limit,string search) {
            if (limit > 0) {
                var lv =
                ctx.Vendors.Where(x => (tipe == ETipeVendor.NONE || x.TipeVendor == tipe) &&
                    (status == EStatusVendor.NONE || x.StatusAkhir == status));
                if (!string.IsNullOrEmpty(search)) lv = lv.Where(d => d.Nama.Contains(search));
                lv = lv.OrderByDescending(x => x.Id).Take(limit);
                return lv.ToList();
            }
            return new List<Vendor>();
        }

        public List<Vendor> GetAllVendor()
        {
            return ctx.Vendors.ToList();
        }

        public List<Vendor> GetAllVendors()
        {
            var ls = ctx.Vendors.Where(x => x.StatusAkhir == EStatusVendor.VERIFIED).Distinct();
            ls = ls.OrderBy(x => x.Nama);
            return ls.ToList();
        }

        public void Save()
        {
            ctx.SaveChanges();
        }

        public int AddVendor(Vendor v) {
            if (v.Id != 0)
            {
                Vendor vEdit = ctx.Vendors.Find(v.Id);
                vEdit.TipeVendor = v.TipeVendor;
                vEdit.NomorVendor = v.NomorVendor;
                vEdit.Nama = v.Nama;
                vEdit.Alamat = v.Alamat;
                vEdit.Provinsi = v.Provinsi;
                vEdit.Kota = v.Kota;
                vEdit.KodePos = v.KodePos;
                vEdit.Email = v.Email;
                vEdit.Website = v.Website;
                vEdit.Telepon = v.Telepon;
                //ctx.Vendors.Add(vEdit);
                ctx.SaveChanges();
                return vEdit.Id;
            }
            else {
                ctx.Vendors.Add(v);
                ctx.SaveChanges();
                return v.Id;
            }
        }

        public int CheckNomor(string no) {
            try
            {
                Vendor v = ctx.Vendors.Where(x => x.NomorVendor.StartsWith(no)).OrderByDescending(x => x.NomorVendor).FirstOrDefault();
                if (v != null)
                {
                    return Int32.Parse(v.NomorVendor.Substring(6, 4)) + 1;
                }
            }
            catch (Exception e) {
                return 1;
            }
            return 1;
        }

        public int CheckNPWP(string npwp)
        {
            //VWNoNPWP NPWP = new VWNoNPWP();
            VWNoNPWP NPWP = (from a in ctx.Dokumens
                          join b in ctx.DokumenDetails on a.Id equals b.Id
                          select new VWNoNPWP
                          {
                              NoNPWP = b.Nomor,
                              TipeDokumen = a.TipeDokumen
                          }).Where(a => a.NoNPWP == npwp && a.TipeDokumen == EDocumentType.NPWP).FirstOrDefault();
            if (NPWP == null)
            {
                return 0;
            }
            else if (NPWP.NoNPWP != null)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }

        public List<Vendor> GetAllVendorsWNon()
        {
            var ls = ctx.Vendors.Where(x => x.StatusAkhir == EStatusVendor.VERIFIED || x.TipeVendor == ETipeVendor.NON_REGISTER).Distinct();
            ls = ls.OrderBy(x => x.Nama);
            return ls.ToList();
        }

        public DataTableViewVendor GetDataListVendorNonRegister(string search, int start, int length)
        {
            search = search == null ? "" : search;
            DataTableViewVendor dtTable = new DataTableViewVendor();
            if (length > 0)
            {
                System.Linq.IQueryable<Vendor> data = ctx.Vendors.AsQueryable();
                //Vendor asdas = ctx.Vendors.Distinct();

                dtTable.recordsTotal = data.Count();
                if (!string.IsNullOrEmpty(search))
                {
                    data = data.Where(d => d.Nama.Contains(search));
                }
                data = data.Where(u => u.TipeVendor == ETipeVendor.NON_REGISTER);
                dtTable.recordsFiltered = data.Count();
                data = data.OrderByDescending(d => d.Nama).Skip(start).Take(length);

                dtTable.data = data.Select(d => new ViewVendor
                {
                    Id = d.Id,
                    Nama = d.Nama,
                    Email = d.Email,
                    Telepon = d.Telepon,
                    Alamat = d.Alamat
                }).OrderBy(x => x.Nama).ToList();
            }
            return dtTable;
        }

        public void DeleteVendorNonRegister(int? id)
        {
            //Produk p = ctx.Produks.Find(id);
            //if (p != null)
            //{
            //    ctx.RiwayatHargas.RemoveRange(p.RiwayatHarga);
            //    ctx.AtributSpesifikasis.RemoveRange(p.KategoriSpesifikasi.AtributSpesifikasi);
            //    ctx.KategoriSpesifikasis.Remove(p.KategoriSpesifikasi);
            //    ctx.Produks.Remove(p);
            //    ctx.SaveChanges();
            //}
            Vendor p = ctx.Vendors.Find(id);
            ctx.Vendors.Remove(p);
            ctx.SaveChanges();
        }

    }
}
