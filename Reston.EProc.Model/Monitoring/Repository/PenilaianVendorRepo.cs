using Reston.Eproc.Model.Monitoring.Entities;
using Reston.Eproc.Model.Monitoring.Model;
using Reston.Eproc.Model.Monitoring.Repository;
using Reston.Pinata.Model;
using Reston.Pinata.Model.Helper;
using Reston.Pinata.Model.PengadaanRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Reston.Eproc.Model.Monitoring.Repository
{
    public interface IPenilaianVendorRepo
    {
        ViewPengadaanPenilaian GetTampilJudul(string NoSPK);
        ResultMessage SimpanPenilaian(PenilaianVendorHeader PenilaianHeader, Guid UserId);
        DataTableViewPenilaian GetDataPenilaian(string NoSPK);
        ViewListPenilaian GetNilai(string NoSPK);
        ViewListPenilaian GetCekSudahDiNilai(string NoSPK);
    }
    public class PenilaianVendorRepo : IPenilaianVendorRepo
    {
        AppDbContext ctx;
        ResultMessage msg = new ResultMessage();

        public PenilaianVendorRepo(AppDbContext j)
        {
            ctx = j;
            ctx.Configuration.LazyLoadingEnabled = true;
        }

        public ViewPengadaanPenilaian GetTampilJudul(string NoSPK)
        {
            var spk = ctx.Spk.Where(d => d.NoSPk == NoSPK).Select(d => new ViewPengadaanPenilaian
            {
                Id = d.Id,
                Judul = d.PemenangPengadaan.Pengadaan.Judul,
                Spk_Id = d.Id.ToString(),
                vendor_Id = d.PemenangPengadaan.VendorId.ToString()
            }).FirstOrDefault();

            return spk;
        }

        public DataTableViewPenilaian GetDataPenilaian(string NoSPK)
        {
            DataTableViewPenilaian dtd = new DataTableViewPenilaian();
            var headerPenilaian = ctx.PenilaianVendorHeaders.Where(a => a.Spk.NoSPk == NoSPK).FirstOrDefault();

            var caripenilaian = ctx.ReferenceDatas.Where(d => d.Qualifier == "Penilaian").Select(d => new ViewListPenilaian()
            {
                Id = d.Id,
                NamaPenilaian = d.LocalizedName
            }).ToList();
            foreach (var item in caripenilaian)
            {
                item.Nilai = headerPenilaian == null ? "" :
                                headerPenilaian.PenilaianVendorDetails.Where(dd => dd.ReferenceDataId == item.Id).FirstOrDefault() == null ? ""
                                    : headerPenilaian.PenilaianVendorDetails.Where(dd => dd.ReferenceDataId == item.Id).FirstOrDefault().Nilai.ToString();
                item.Catatan_item = headerPenilaian == null ? "" :
                            headerPenilaian.PenilaianVendorDetails.Where(dd => dd.ReferenceDataId == item.Id).FirstOrDefault() == null ? ""
                                : headerPenilaian.PenilaianVendorDetails.Where(dd => dd.ReferenceDataId == item.Id).FirstOrDefault().Catatan_item;


            }
            dtd.data = caripenilaian;
            return dtd;
        }

        public ResultMessage SimpanPenilaian(PenilaianVendorHeader PenilaianVendorHeaders, Guid UserId)
        {
            ResultMessage msg = new ResultMessage();
            try
            {
                var oData = ctx.PenilaianVendorHeaders.Where(d => d.Spk_Id == PenilaianVendorHeaders.Spk_Id).FirstOrDefault();
                if (oData != null)
                {
                    var oChildData = ctx.PenilaianVendorDetails.Where(d => d.PenilaianVendorHeaderId == oData.Id);
                    ctx.PenilaianVendorDetails.RemoveRange(oChildData);
                    ctx.PenilaianVendorHeaders.Remove(oData);
                }

                foreach (var item in PenilaianVendorHeaders.PenilaianVendorDetails)
                {
                    item.CreatedOn = DateTime.Now;
                    item.CreatedBy = UserId;
                }
                ctx.PenilaianVendorHeaders.Add(PenilaianVendorHeaders);
                ctx.SaveChanges(UserId.ToString());
            }
            catch (Exception ex)
            {
                msg.status = HttpStatusCode.ExpectationFailed;
                msg.message = ex.ToString();
            }
            return msg;
        }

        public ViewListPenilaian GetNilai(string NoSPK)
        {
            var penilaianheader = ctx.PenilaianVendorHeaders.Where(d => d.Spk.NoSPk == NoSPK).FirstOrDefault();
            if (penilaianheader == null)
            {
                return new ViewListPenilaian { };
            }
            else
            {
                return new ViewListPenilaian
                {
                    Catatan = penilaianheader.Catatan,
                    Total_nilai = penilaianheader.Total_nilai.ToString(),
                    Jumlah_penilaian = penilaianheader.PenilaianVendorDetails.Where(d => d.Nilai > 0).Count().ToString()
                };
            }
        }

        public ViewListPenilaian GetCekSudahDiNilai(string NoSPK)
        {
            //buat cek penilaian dengan NoSPK
            var idspk = ctx.Spk.Where(d => d.NoSPk == NoSPK).FirstOrDefault();
            var isSudahDinilaiDariNoSPK = ctx.PenilaianVendorHeaders.Where(d => d.Spk_Id == idspk.Id).FirstOrDefault();
            //buat cek penilaian dengan ProyID
            var idspk2 = ctx.Spk.Where(d => d.NoSPk == NoSPK).FirstOrDefault();
            var proyekID = ctx.RencanaProyeks.Where(d => d.SpkId == idspk2.Id).FirstOrDefault();
            if (proyekID != null)
            {
                var isSudahDinilaiDariProyID = ctx.PenilaianVendorHeaders.Where(d => d.ProyekId == proyekID.Id).FirstOrDefault();
                //buattampil
                var proykan = ctx.PenilaianVendorHeaders.Where(d => d.ProyekId == proyekID.Id).FirstOrDefault();
                if (isSudahDinilaiDariProyID != null)
                {
                    return new ViewListPenilaian
                    {
                        isSudahDinilaiDariProyID = 1,
                        proyID = proykan.ProyekId.ToString()
                    };
                }
                else if (isSudahDinilaiDariNoSPK != null)
                {
                    return new ViewListPenilaian
                    {
                        isSudahDinilaiDariNoSPK = 1,
                        //proyID = proykan.ProyekId.ToString()
                    };
                }
                else
                {
                    return new ViewListPenilaian
                    {
                        isSudahDinilaiDariProyID = 0,
                        isSudahDinilaiDariNoSPK = 0
                    };
                }
            }
            else if (isSudahDinilaiDariNoSPK != null)
            {
                return new ViewListPenilaian
                {
                    isSudahDinilaiDariNoSPK = 1,
                    // proyID = proykan.ProyekId.ToString()
                };
            }
            else
            {
                return new ViewListPenilaian
                {
                    isSudahDinilaiDariProyID = 0,
                    isSudahDinilaiDariNoSPK = 0
                };
            }
        }
    }
}