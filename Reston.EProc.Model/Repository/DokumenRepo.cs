using Reston.Pinata.Model.JimbisModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reston.Pinata.Model.Repository
{
    public class DokumenRepo : IDokumenRepo
    {
        AppDbContext ctx;
        public DokumenRepo(AppDbContext j)
        {
            ctx = j;
            ctx.Configuration.LazyLoadingEnabled = true;
        }

        public Dokumen GetDokumen(Guid id){
            return ctx.Dokumens.Find(id);
        }

        public IQueryable<Dokumen> GetAllDokumenByVendor(int vendorid)
        {
            Vendor v = ctx.Vendors.Find(vendorid);
            if (v != null)
                return v.Dokumen.AsQueryable();
            return null;
        }

        public Guid AddDokumen(Dokumen d) {
            ctx.Dokumens.Add(d);
            ctx.SaveChanges();
            return d.Id;
        }
    }
}
