using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Validation;
using Model.Helper;
using Reston.Pinata.Model.PengadaanRepository.View;
using System.Configuration;
using Reston.Pinata.Model.JimbisModel;
using Newtonsoft.Json;
using System.Net;
using Reston.Helper.Util;
using NLog;

namespace Reston.Pinata.Model.PengadaanRepository
{
    public interface ISpkRepo
    {
        int Delete(Guid Id, Guid UserId);

        DataTableSpkTemplate List(string search, int start, int limit, string klasifikasi);
        DataTablePksTemplate ListPks(string search, int start, int limit, string klasifikasi);
        VWSpk detail(Guid Id, Guid UserId);
        Spk saveSpkPertam(Spk spk, Guid UserId);
        ResultMessage save(Spk spk, Guid UserId);
        ResultMessage ChangeStatus(Guid Id,StatusSpk status, Guid UserId);
        ResultMessage saveDokumen(DokumenSpk dokPks, Guid UserId);
        ResultMessage deleteSpk(Guid Id, Guid UserId);
        int deleteDokumenSpk(Guid Id, Guid UserId, int approver);
        Spk get(Guid id);
        DokumenSpk getDokSpk(Guid id);
        RiwayatDokumenSpk AddRiwayatDokumenSpk(RiwayatDokumenSpk dtRiwayatDokumenSpk, Guid UserId);
        List<VWDokumenSPK> GetListDokumenSpk(Guid Id);
        List<VWReportSpk> GetReportSPK(DateTime? dari, DateTime? sampai, Guid UserId, string divisi);
        List<VWReportSpk> GetReportSPK2(DateTime? dari, DateTime? sampai, Guid UserId);
        ResultMessage savenonpks(Spk spk, int vendorId, Guid UserId);
        //ResultMessage savenonpks(Spk spk, Guid UserId);
        VWSpk detailnonpks(Guid Id, Guid UserId);
        DataTableSpkTemplate ListNonPKS(string search, int start, int limit, string klasifikasi);
        List<VWReportSpk> ReportSPKNONPKS(DateTime? dari, DateTime? sampai, Guid UserId, string divisi);
        List<VWReportSpk> ReportSPKNONPKS2(DateTime? dari, DateTime? sampai, Guid UserId);

        int AddVendorNonReg(Vendor vendor);
        ResultMessage saveDokumenSpkNonPks(DokumenSpkNonPks dokumenSpkNonPks, Guid UserId);
        List<VWDokumenSPK> GetListDokumenSpkNonPks(string klasifikasi, Guid id);
        DokumenSpkNonPks getDokSpkNonPks(Guid id, string klasifikasi);
        int deleteDokumenSpkNonPks(Guid Id, Guid UserId, int approver);
        ResultMessage saveAtributDokumenNonPks(Guid id, string klasifikasi, string NoDok, DateTime? TglDok);
        VWSpk detaildoknonpks(Guid Id, Guid UserId);
    }
    public class SpkRepo : ISpkRepo
    {
        AppDbContext ctx;
        private IPengadaanRepo _repoPengadaan;
        private static Logger _log = LogManager.GetCurrentClassLogger();

        public SpkRepo(AppDbContext j)
        {
            ctx = j;
            //ctx.Configuration.ProxyCreationEnabled = false;
            ctx.Configuration.LazyLoadingEnabled = true;
            _repoPengadaan = new PengadaanRepo(new AppDbContext());
        }

        ResultMessage msg = new ResultMessage();

        public void Save()
        {
            ctx.SaveChanges();
        }

        public Spk saveSpkPertam(Spk spk, Guid UserId)
        {
            var oSpk = ctx.Spk.Where(d => d.NoSPk == spk.NoSPk && d.PksId == null).FirstOrDefault();

            spk.PemenangPengadaan = ctx.PemenangPengadaans.Find(spk.PemenangPengadaanId);
            if (oSpk == null)
            {
                
                //var TotalHargaKandidat = spk.PemenangPengadaan.Pengadaan.RKSHeaders.FirstOrDefault() == null ? null :
                //           spk.PemenangPengadaan.Pengadaan.RKSHeaders.FirstOrDefault().RKSDetails.Where(dd =>
                //               dd.RKSHeaderId == spk.PemenangPengadaan.Pengadaan.RKSHeaders.FirstOrDefault().Id)
                //               .Sum(dx => dx.HargaKlarifikasiRekanan.Where(ddx => ddx.RKSDetailId == dx.Id).FirstOrDefault() == null ? 0 : dx.HargaKlarifikasiRekanan.Where(ddx => ddx.RKSDetailId == dx.Id).FirstOrDefault().harga * dx.jumlah);
                decimal? TotalHargaKandidat = 0;
                var nhargavendorLanjutan=(from b in ctx.HargaKlarifikasiLanLanjutans
                 join c in ctx.RKSDetails on b.RKSDetailId equals c.Id
                 join d in ctx.RKSHeaders on c.RKSHeaderId equals d.Id
                                  where d.PengadaanId == spk.PemenangPengadaan.Pengadaan.Id && b.VendorId == spk.PemenangPengadaan.VendorId
                 select new item
                 {
                     Id = c.Id,
                     harga = b.harga,
                     jumlah = c.jumlah
                 }).ToList();
                var nhargavendorKlarifikasi = (from b in ctx.HargaKlarifikasiRekanans
                                    join c in ctx.RKSDetails on b.RKSDetailId equals c.Id
                                    join d in ctx.RKSHeaders on c.RKSHeaderId equals d.Id
                                    where d.PengadaanId == spk.PemenangPengadaan.Pengadaan.Id && b.VendorId == spk.PemenangPengadaan.VendorId
                                    select new item
                                    {
                                        Id = c.Id,
                                        harga = b.harga,
                                        jumlah = c.jumlah
                                    }).ToList();

                if (nhargavendorLanjutan.Count() > 0)
                {
                    TotalHargaKandidat = nhargavendorLanjutan.Sum(d => d.harga * d.jumlah);
                }
                else
                {
                    TotalHargaKandidat = nhargavendorKlarifikasi.Sum(d => d.harga * d.jumlah);
                }
                spk.CreateOn = DateTime.Now;
                spk.CreateBy = UserId;
                spk.NilaiSPK = TotalHargaKandidat;
                ctx.Spk.Add(spk);
            }
            else
            {
                var TotalHargaKandidat = spk.PemenangPengadaan.Pengadaan.RKSHeaders.FirstOrDefault() == null ? null :
                           spk.PemenangPengadaan.Pengadaan.RKSHeaders.FirstOrDefault().RKSDetails.Where(dd =>
                               dd.RKSHeaderId == spk.PemenangPengadaan.Pengadaan.RKSHeaders.FirstOrDefault().Id)
                               .Sum(dx => dx.HargaKlarifikasiRekanan.Where(ddx => ddx.RKSDetailId == dx.Id).FirstOrDefault() == null ? 0 : dx.HargaKlarifikasiRekanan.Where(ddx => ddx.RKSDetailId == dx.Id).FirstOrDefault().harga == null ? 0 : dx.HargaKlarifikasiRekanan.Where(ddx => ddx.RKSDetailId == dx.Id).FirstOrDefault().harga*dx.jumlah);
                
                spk.NilaiSPK = TotalHargaKandidat;
                oSpk.NoSPk = spk.NoSPk;
                oSpk.Note = spk.Note;
                oSpk.DokumenPengadaanId = spk.DokumenPengadaanId;
                oSpk.PemenangPengadaanId = spk.PemenangPengadaanId;
                oSpk.StatusSpk = spk.StatusSpk;
                oSpk.Title = spk.Title;
                oSpk.WorkflowId = spk.WorkflowId;
                oSpk.TanggalSPK = spk.TanggalSPK;
                oSpk.ModifiedBy = UserId;
                oSpk.ModifiedOn = DateTime.Now;
            }
            ctx.SaveChanges();
            return oSpk;
        }

        public int Delete(Guid Id, Guid UserId)
        {
            try
            {
                var oSpk = ctx.Spk.Find(Id);
                if (oSpk != null)
                {
                    ctx.Spk.Remove(oSpk);
                }
                ctx.SaveChanges();
                return 1;
            }
            catch
            {
                return 0;
            }


        }
        
        public DataTableSpkTemplate List(string search, int start, int limit, string klasifikasi)
        {
            search = search == null ? "" : search;
            DataTableSpkTemplate dtTable = new DataTableSpkTemplate();
            if (limit > 0)
            {
                //var data = ctx.Spk.Where(d=>d.NoSPk!=null).AsQueryable();
                System.Linq.IQueryable<Spk> data = ctx.Spk.AsQueryable();
                dtTable.recordsTotal = data.Count();
                //var data = ctx.Spk.AsQueryable();
                if (!string.IsNullOrEmpty(klasifikasi))
                {
                    data = data.Where(d => d.PemenangPengadaan.Pengadaan.JenisPekerjaan == klasifikasi);
                }
                if (!string.IsNullOrEmpty(search))
                {
                    data = data.Where(d => d.Title.Contains(search));
                }
                //data = data.Where(y => y.Pengadaan.Id != null);
                //dtTable.data = dtTable.data.Where(x => x.PengadaanId != null).ToList();
                dtTable.data = data.Select(d => new VWSpk
                {
                    Id = d.Id,//d.PemenangPengadaan.Pengadaan.Id,
                    PemenangPengadaanId = d.PemenangPengadaanId,
                    NoSpk = d.NoSPk,
                    Judul = d.PemenangPengadaan.Pengadaan.Judul,
                    JenisPekerjaan = d.PemenangPengadaan.Pengadaan.JenisPekerjaan,
                    Vendor = d.PemenangPengadaan.Vendor.Nama,
                    AturanPengadaan = d.PemenangPengadaan.Pengadaan.AturanPengadaan,
                    VendorId = d.PemenangPengadaan.VendorId,
                    PengadaanId = d.PemenangPengadaan.Pengadaan.Id,
                    StatusSpk=d.StatusSpk,
                    StatusSpkName=d.StatusSpk.ToString(),
                    NilaiSPK=d.NilaiSPK,
                    PksId = d.PksId,
                    HPS = d.PemenangPengadaan.Pengadaan.RKSHeaders.FirstOrDefault() == null ? null :
                           d.PemenangPengadaan.Pengadaan.RKSHeaders.FirstOrDefault().RKSDetails.Where(dd =>
                               dd.RKSHeaderId == d.PemenangPengadaan.Pengadaan.RKSHeaders.FirstOrDefault().Id)
                               .Sum(dx => dx.HargaKlarifikasiRekanan.Where(ddx => ddx.RKSDetailId == dx.Id).FirstOrDefault() == null ? 0 : dx.HargaKlarifikasiRekanan.Where(ddx => ddx.RKSDetailId == dx.Id).FirstOrDefault().harga * dx.jumlah)
                }).Where(o => o.PengadaanId != null).ToList();
                dtTable.recordsFiltered = dtTable.data.Count();
                dtTable.data = dtTable.data.OrderByDescending(d => d.CreateOn).Skip(start).Take(limit).ToList();
            }
            return dtTable;
        }

        public DataTablePksTemplate ListPks(string search, int start, int limit, string klasifikasi)
        {
            search = search == null ? "" : search;
            DataTablePksTemplate dtTable = new DataTablePksTemplate();
            if (limit > 0)
            {

                var data = ctx.Pks.Where(d=>d.StatusPks==StatusPks.Approve).AsQueryable();
                dtTable.recordsTotal = data.Count();
                if (!string.IsNullOrEmpty(klasifikasi))
                {
                    data = data.Where(d => d.PemenangPengadaan.Pengadaan.JenisPekerjaan == klasifikasi);
                }
                if (!string.IsNullOrEmpty(search))
                {
                    data = data.Where(d => d.Title == d.Title);
                }
                dtTable.recordsFiltered = data.Count();
                data = data.OrderByDescending(d => d.CreateOn).Skip(start).Take(limit);
                dtTable.data = data.Select(d => new VWPks
                {
                    Id = d.Id,
                    PemenangPengadaanId = d.PemenangPengadaanId,
                    NoPks = d.NoDokumen,
                    NoPengadaan=d.PemenangPengadaan.Pengadaan.NoPengadaan,
                    Judul = d.PemenangPengadaan.Pengadaan.Judul,
                    JenisPekerjaan = d.PemenangPengadaan.Pengadaan.JenisPekerjaan,
                    Vendor = d.PemenangPengadaan.Vendor.Nama,
                    AturanPengadaan = d.PemenangPengadaan.Pengadaan.AturanPengadaan,
                    VendorId = d.PemenangPengadaan.VendorId,
                    PengadaanId = d.PemenangPengadaan.Pengadaan.Id,
                    StatusPks = d.StatusPks,
                    StatusPksName = d.StatusPks.ToString(),
                    WorkflowId = d.WorkflowId,
                    HPS = d.PemenangPengadaan.Pengadaan.RKSHeaders.FirstOrDefault() == null ? null :
                            d.PemenangPengadaan.Pengadaan.RKSHeaders.FirstOrDefault().RKSDetails.Where(dd =>
                                dd.RKSHeaderId == d.PemenangPengadaan.Pengadaan.RKSHeaders.FirstOrDefault().Id)
                                .Sum(dx => dx.HargaKlarifikasiRekanan.Where(ddx => ddx.RKSDetailId == dx.Id).FirstOrDefault() == null ? 0 : dx.HargaKlarifikasiRekanan.Where(ddx => ddx.RKSDetailId == dx.Id).FirstOrDefault().harga * dx.jumlah)
                }).ToList();

            }
            return dtTable;
        }

        public ResultMessage save(Spk spk, Guid UserId)
        {
            try
            {
                var oldPks = ctx.Pks.Find(spk.PksId);
                if (oldPks == null) return new ResultMessage()
                {
                    message = HttpStatusCode.MethodNotAllowed.ToString(),
                    status = HttpStatusCode.MethodNotAllowed
                };
                if (spk.Id != Guid.Empty && spk.Id != null)
                {
                    var oldSpk = ctx.Spk.Find(spk.Id);
                    if (oldSpk == null) return new ResultMessage();
                    oldSpk.Note = spk.Note;
                    oldSpk.Title = spk.Title;
                    oldSpk.PksId = spk.PksId;
                    oldSpk.TanggalSPK = spk.TanggalSPK;
                    oldSpk.NilaiSPK = spk.NilaiSPK;
                    oldSpk.ModifiedBy = UserId;
                    oldSpk.ModifiedOn = DateTime.Now;
                    ctx.SaveChanges(UserId.ToString());
                    return new ResultMessage()
                    {
                        Id = spk.Id.ToString(),
                        message = Common.UpdateSukses(),
                        status = HttpStatusCode.OK
                    };
                }
                else
                {
                    spk.PksId = oldPks.Id;
                    spk.PemenangPengadaanId = oldPks.PemenangPengadaanId;
                    spk.CreateBy = UserId;
                    spk.CreateOn = DateTime.Now;
                    ctx.Spk.Add(spk);
                    ctx.SaveChanges(UserId.ToString());
                    return new ResultMessage()
                    {
                        Id = spk.Id.ToString(),
                        message = Common.SaveSukses(),
                        status = HttpStatusCode.OK
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResultMessage()
                {
                    message = ex.ToString(),
                    status = HttpStatusCode.ExpectationFailed
                };

            }
        }

        public Spk get(Guid id)
        {
            return ctx.Spk.Find(id);
        }

        public ResultMessage deleteSpk(Guid Id, Guid UserId)
        {
            try
            {
                var oldData = ctx.Spk.Find(Id);
                if (oldData.CreateBy != UserId) return new ResultMessage()
                {
                    message = HttpStatusCode.MethodNotAllowed.ToString(),
                    status = HttpStatusCode.MethodNotAllowed
                };
                var oDokSpk = ctx.DokumenSpk.Where(d => d.SpkId == oldData.Id);
                ctx.DokumenSpk.RemoveRange(oDokSpk);
                ctx.Spk.Remove(oldData);
                ctx.SaveChanges(UserId.ToString());
                msg.status = HttpStatusCode.OK;
                msg.message = "Sukses";
            }
            catch (Exception ex)
            {
                msg.status = HttpStatusCode.ExpectationFailed;
                msg.message = ex.ToString();
            }
            return msg;
        }

        public VWSpk detail(Guid Id,Guid UserId)
        {
            return ctx.Spk.Where(d => d.Id == Id).Select(d => new VWSpk()
            {
                Id = d.Id,
                PksId=d.PksId,
                NoPks=d.Pks.NoDokumen,
                JenisPekerjaan = d.Pengadaan.JenisPekerjaan,
                PemenangPengadaanId = d.PemenangPengadaan.Id,
                Judul = d.PemenangPengadaan.Pengadaan.Judul,
                NoPengadaan = d.PemenangPengadaan.Pengadaan.NoPengadaan,
                Keterangan = d.PemenangPengadaan.Pengadaan.Keterangan,
                HPS =  d.PemenangPengadaan.Pengadaan.RKSHeaders.FirstOrDefault() == null ? null :
                           d.PemenangPengadaan.Pengadaan.RKSHeaders.FirstOrDefault().RKSDetails.Where(dd =>
                               dd.RKSHeaderId == d.PemenangPengadaan.Pengadaan.RKSHeaders.FirstOrDefault().Id)
                               .Sum(dx => dx.HargaKlarifikasiRekanan.Where(ddx => ddx.RKSDetailId == dx.Id).FirstOrDefault() == null ? 0 : dx.HargaKlarifikasiRekanan.Where(ddx => ddx.RKSDetailId == dx.Id).FirstOrDefault().harga * dx.jumlah),
                NoSpk = d.NoSPk,
                Vendor = d.PemenangPengadaan.Vendor.Nama,
                StatusSpk = d.StatusSpk,
                StatusSpkName = d.StatusSpk.ToString(),
                isOwner=d.CreateBy==UserId?1:0,
                Note=d.Note,
                WorkflowId=d.WorkflowId,
                TanggalSPK=d.TanggalSPK,
                NilaiSPK=d.NilaiSPK
            }).FirstOrDefault();
        }

        public ResultMessage saveDokumen(DokumenSpk dokSpk, Guid UserId)
        {
            try
            {
                var oData = ctx.Spk.Find(dokSpk.SpkId);
                if (oData == null) return new ResultMessage
                {
                    status = HttpStatusCode.Forbidden,
                    message = Common.Forbiden()
                };
                dokSpk.CreateOn = DateTime.Now;
                dokSpk.CreateBy = UserId;
                ctx.DokumenSpk.Add(dokSpk);
                ctx.SaveChanges();
                return new ResultMessage
                {
                    status = HttpStatusCode.OK,
                    message = Common.SaveSukses(),
                    Id = dokSpk.Id.ToString()
                };
            }
            catch(Exception ex){
                return new ResultMessage()
                {
                    status = HttpStatusCode.NotImplemented,
                    message = ex.ToString()
                };
            }
        }

        public List<VWDokumenSPK> GetListDokumenSpk(Guid Id)
        {
            return ctx.DokumenSpk.Where(d => d.SpkId == Id ).Select(d => new VWDokumenSPK()
            {
                Id=d.Id,
                SpkId = d.SpkId,
                SizeFile=d.SizeFile,
                ContentType=d.ContentType,
                File=d.File
            }).ToList();
        }

        public int deleteDokumenSpk(Guid Id, Guid UserId,int approver)
        {
            try
            {
                DokumenSpk doku = ctx.DokumenSpk.Find(Id);
                int isMine = doku.CreateBy == UserId ? 1 : 0;
                if (doku.Spk.StatusSpk != StatusSpk.Draft) return 0;
                if (isMine == 1)
                {
                    ctx.DokumenSpk.Remove(doku);
                    ctx.SaveChanges(UserId.ToString());
                    return 1;
                }
                return 0;
            }
            catch { return 0; }
        }

        public RiwayatDokumenSpk AddRiwayatDokumenSpk(RiwayatDokumenSpk dtRiwayatDokumenSpk, Guid UserId)
        {
            try
            {
                ctx.RiwayatDokumenSpk.Add(dtRiwayatDokumenSpk);
                ctx.SaveChanges(UserId.ToString());
                return dtRiwayatDokumenSpk;
            }
            catch
            {
                return new RiwayatDokumenSpk();
            }
        }
    
        public DokumenSpk getDokSpk(Guid id)
        {
            var cek1 = ctx.DokumenSpk.Find(id);

            if(cek1 == null)
            {
                cek1 = ctx.DokumenSpk.Where(d => d.SpkId == id).FirstOrDefault(); 
            }

            return cek1;
        }

        public ResultMessage ChangeStatus(Guid Id, StatusSpk status, Guid UserId)
        {
            try
            {
                var oData = ctx.Spk.Find(Id);
                oData.StatusSpk = status;
                if (status == StatusSpk.Aktif)
                {
                    if (string.IsNullOrEmpty(oData.NoSPk))
                    {
                        oData.NoSPk = _repoPengadaan.GenerateBeritaAcaraSPK(UserId);
                    }
                }
                ctx.SaveChanges(UserId.ToString());
                return new ResultMessage()
                {
                    Id=Id.ToString(),message=Common.SaveSukses(),status=HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new ResultMessage()
                {
                    message=ex.ToString(),status=HttpStatusCode.NotImplemented
                };
            }
        }

        public List<VWReportSpk> GetReportSPK(DateTime? dari, DateTime? sampai, Guid UserId, string divisi)
        {
            var oReport = (from b in ctx.Spk
                           join c in ctx.PemenangPengadaans on b.PemenangPengadaanId equals c.Id
                           join d in ctx.Vendors on c.VendorId equals d.Id
                           join e in ctx.Pengadaans on c.PengadaanId equals e.Id
                           join f in ctx.PersonilPengadaans on e.Id equals f.PengadaanId
                           where b.CreateOn >= dari && b.CreateOn <= sampai && f.tipe == "pic" && e.UnitKerjaPemohon == divisi//c.tanggal >= dari && c.tanggal <= sampai// && c.Tipe == TipeBerkas.BeritaAcaraPenentuanPemenang
                           select new VWReportSpk
                           {
                               NoSpk = b.NoSPk,
                               Title = e.Judul,
                               Vendor = d.Nama,
                               PIC = f.Nama,
                               Divisi = e.UnitKerjaPemohon,
                               TanggalSPK = b.TanggalSPK.ToString(),
                               NilaiSPK = b.NilaiSPK.ToString(),

                           }).Distinct().ToList();
            return oReport;
        }

        public List<VWReportSpk> GetReportSPK2(DateTime? dari, DateTime? sampai, Guid UserId)
        {
            var oReport = (from b in ctx.Spk
                           join c in ctx.PemenangPengadaans on b.PemenangPengadaanId equals c.Id
                           join d in ctx.Vendors on c.VendorId equals d.Id
                           join e in ctx.Pengadaans on c.PengadaanId equals e.Id
                           join f in ctx.PersonilPengadaans on e.Id equals f.PengadaanId
                           where b.CreateOn >= dari && b.CreateOn <= sampai && f.tipe == "pic"//c.tanggal >= dari && c.tanggal <= sampai// && c.Tipe == TipeBerkas.BeritaAcaraPenentuanPemenang
                           select new VWReportSpk
                           {
                               NoSpk = b.NoSPk,
                               Title = e.Judul,
                               Vendor = d.Nama,
                               PIC = f.Nama,
                               Divisi = e.UnitKerjaPemohon,
                               TanggalSPK = b.TanggalSPK.ToString(),
                               NilaiSPK = b.NilaiSPK.ToString(),

                           }).Distinct().ToList();
            return oReport;
        }

        public ResultMessage savenonpks(Spk spk, int vendorId, Guid UserId)
        //public ResultMessage savenonpks(Spk spk, Guid UserId)
        {
            try
            {
                //var oldPks = ctx.Pks.Find(spk.PksId);
                //if (oldPks == null) return new ResultMessage()
                //{
                //    message = HttpStatusCode.MethodNotAllowed.ToString(),
                //    status = HttpStatusCode.MethodNotAllowed
                //};
                if (spk.Id != Guid.Empty && spk.Id != null)
                {
                    var oldSpk = ctx.Spk.Find(spk.Id);
                    if (oldSpk == null) return new ResultMessage();
                    oldSpk.Note = spk.Note;
                    oldSpk.Title = spk.Title;
                    //oldSpk.PksId = spk.PksId;
                    oldSpk.TanggalSPK = spk.TanggalSPK;
                    oldSpk.NilaiSPK = spk.NilaiSPK;
                    oldSpk.ModifiedBy = UserId;
                    oldSpk.ModifiedOn = DateTime.Now;
                    ctx.SaveChanges(UserId.ToString());

                    var oldPP = ctx.PemenangPengadaans.Find(oldSpk.PemenangPengadaanId);
                    oldPP.VendorId = vendorId;
                    //oldPP.VendorId = spk.PemenangPengadaan.VendorId;
                    oldPP.ModifiedBy = UserId;
                    oldPP.ModifiedOn = DateTime.Now;
                    ctx.SaveChanges();

                    return new ResultMessage()
                    {
                        Id = spk.Id.ToString(),
                        message = Common.UpdateSukses(),
                        status = HttpStatusCode.OK
                    };
                }
                else
                {
                    //spk.PksId = oldPks.Id;
                    //spk.PemenangPengadaanId = oldPks.PemenangPengadaanId;

                    //Create new pemenang pengadaan

                    var PP = new PemenangPengadaan()
                    {
                        Id = Guid.NewGuid(),
                        VendorId = vendorId,
                        //VendorId = spk.PemenangPengadaan.VendorId,
                        CreatedBy = UserId,
                        CreateOn = DateTime.Now
                    };
                    ctx.PemenangPengadaans.Add(PP);
                    ctx.SaveChanges();

                    var newSPK = new Spk();
                    {
                        newSPK.Title = spk.Title;
                        newSPK.PemenangPengadaanId = PP.Id;
                        newSPK.CreateBy = UserId;
                        newSPK.CreateOn = DateTime.Now;
                        newSPK.Note = spk.Note;
                        newSPK.TanggalSPK = spk.TanggalSPK;
                        newSPK.NilaiSPK = spk.NilaiSPK;
                        newSPK.ModifiedBy = UserId;
                        newSPK.ModifiedOn = DateTime.Now;
                    }
                    //spk.PemenangPengadaanId = PP.Id;
                    //spk.CreateBy = UserId;
                    //spk.CreateOn = DateTime.Now;
                    ctx.Spk.Add(newSPK);
                    ctx.SaveChanges(UserId.ToString());
                    return new ResultMessage()
                    {
                        Id = newSPK.Id.ToString(),
                        message = Common.SaveSukses(),
                        status = HttpStatusCode.OK
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResultMessage()
                {
                    message = ex.ToString(),
                    status = HttpStatusCode.ExpectationFailed
                };

            }
        }

        public VWSpk detailnonpks(Guid Id, Guid UserId)
        {
            return ctx.Spk.Where(d => d.Id == Id).Select(d => new VWSpk()
            {
                Id = d.Id,
                PksId = d.PksId,
                NoPks = d.Pks.NoDokumen,
                JenisPekerjaan = d.Pengadaan.JenisPekerjaan,
                PemenangPengadaanId = d.PemenangPengadaan.Id,
                Judul = d.Title,
                NoPengadaan = d.PemenangPengadaan.Pengadaan.NoPengadaan,
                Keterangan = d.PemenangPengadaan.Pengadaan.Keterangan,
                HPS = d.PemenangPengadaan.Pengadaan.RKSHeaders.FirstOrDefault() == null ? null :
                           d.PemenangPengadaan.Pengadaan.RKSHeaders.FirstOrDefault().RKSDetails.Where(dd =>
                               dd.RKSHeaderId == d.PemenangPengadaan.Pengadaan.RKSHeaders.FirstOrDefault().Id)
                               .Sum(dx => dx.HargaKlarifikasiRekanan.Where(ddx => ddx.RKSDetailId == dx.Id).FirstOrDefault() == null ? 0 : dx.HargaKlarifikasiRekanan.Where(ddx => ddx.RKSDetailId == dx.Id).FirstOrDefault().harga * dx.jumlah),
                NoSpk = d.NoSPk,
                Vendor = d.PemenangPengadaan.Vendor.Nama,
                VendorId = d.PemenangPengadaan.Vendor.Id,
                StatusSpk = d.StatusSpk,
                StatusSpkName = d.StatusSpk.ToString(),
                isOwner = d.CreateBy == UserId ? 1 : 0,
                Note = d.Note,
                WorkflowId = d.WorkflowId,
                TanggalSPK = d.TanggalSPK,
                NilaiSPK = d.NilaiSPK,
                TipeVendor = d.PemenangPengadaan.Vendor.TipeVendor
            }).FirstOrDefault();
        }


        public DataTableSpkTemplate ListNonPKS(string search, int start, int limit, string klasifikasi)
        {
            search = search == null ? "" : search;
            DataTableSpkTemplate dtTable = new DataTableSpkTemplate();
            if (limit > 0)
            {
                //var data = ctx.Spk.Where(d=>d.NoSPk!=null).AsQueryable();
                //var selectData = ctx.Spk.Where(a => a.PksId == null).ToList();
                //var selectData2 = selectData.Where(a => a.Pengadaan.Id == null).ToList();
                var data = ctx.Spk.Where(a => a.PksId == null).AsQueryable();
                dtTable.recordsTotal = data.Count();
                if (!string.IsNullOrEmpty(klasifikasi))
                {
                    data = data.Where(d => d.PemenangPengadaan.Pengadaan.JenisPekerjaan == klasifikasi);
                }
                if (!string.IsNullOrEmpty(search))
                {
                    data = data.Where(d => d.Title.Contains(search));
                }
                dtTable.recordsFiltered = data.Count();
                data = data.OrderByDescending(d => d.CreateOn).Skip(start).Take(limit);
                var lol = data.ToList();
                dtTable.data = data.Select(d => new VWSpk
                {
                    Id = d.Id,//d.PemenangPengadaan.Pengadaan.Id,
                    PemenangPengadaanId = d.PemenangPengadaanId,
                    NoSpk = d.NoSPk,
                    Judul = d.Title,
                    JenisPekerjaan = d.PemenangPengadaan.Pengadaan.JenisPekerjaan,
                    Vendor = d.PemenangPengadaan.Vendor.Nama,
                    AturanPengadaan = d.PemenangPengadaan.Pengadaan.AturanPengadaan,
                    VendorId = d.PemenangPengadaan.VendorId,
                    PengadaanId = d.PemenangPengadaan.Pengadaan.Id,
                    StatusSpk = d.StatusSpk,
                    StatusSpkName = d.StatusSpk.ToString(),
                    NilaiSPK = d.NilaiSPK,
                    PksId = d.PksId,
                    HPS = d.PemenangPengadaan.Pengadaan.RKSHeaders.FirstOrDefault() == null ? null :
                           d.PemenangPengadaan.Pengadaan.RKSHeaders.FirstOrDefault().RKSDetails.Where(dd =>
                               dd.RKSHeaderId == d.PemenangPengadaan.Pengadaan.RKSHeaders.FirstOrDefault().Id)
                               .Sum(dx => dx.HargaKlarifikasiRekanan.Where(ddx => ddx.RKSDetailId == dx.Id).FirstOrDefault() == null ? 0 : dx.HargaKlarifikasiRekanan.Where(ddx => ddx.RKSDetailId == dx.Id).FirstOrDefault().harga * dx.jumlah)
                }).ToList();
            }
            return dtTable;
        }

        public List<VWReportSpk> ReportSPKNONPKS(DateTime? dari, DateTime? sampai, Guid UserId, string divisi)
        {
            var oReport = (from b in ctx.Spk
                           join c in ctx.PemenangPengadaans on b.PemenangPengadaanId equals c.Id
                           join d in ctx.Vendors on c.VendorId equals d.Id
                           //join e in ctx.Pengadaans on c.PengadaanId equals e.Id
                           //join f in ctx.PersonilPengadaans on e.Id equals f.PengadaanId
                           where c.PengadaanId == null && b.CreateOn >= dari && b.CreateOn <= sampai /*&& f.tipe == "pic" && e.UnitKerjaPemohon == divisi*/ //c.tanggal >= dari && c.tanggal <= sampai// && c.Tipe == TipeBerkas.BeritaAcaraPenentuanPemenang
                           select new VWReportSpk
                           {
                               NoSpk = b.NoSPk,
                               Title = b.Title, // e.Judul,
                               Vendor = d.Nama,
                               //PIC = f.Nama,
                               //Divisi = e.UnitKerjaPemohon,
                               TanggalSPK = b.TanggalSPK.ToString(),
                               NilaiSPK = b.NilaiSPK.ToString(),

                           }).Distinct().ToList();
            return oReport;
        }

        public List<VWReportSpk> ReportSPKNONPKS2(DateTime? dari, DateTime? sampai, Guid UserId)
        {
            var oReport = (from b in ctx.Spk
                           join c in ctx.PemenangPengadaans on b.PemenangPengadaanId equals c.Id
                           join d in ctx.Vendors on c.VendorId equals d.Id
                           //join e in ctx.Pengadaans on c.PengadaanId equals e.Id
                           //join f in ctx.PersonilPengadaans on e.Id equals f.PengadaanId
                           where c.PengadaanId == null && b.CreateOn >= dari && b.CreateOn <= sampai /*&& f.tipe == "pic"*/   //c.tanggal >= dari && c.tanggal <= sampai// && c.Tipe == TipeBerkas.BeritaAcaraPenentuanPemenang
                           select new VWReportSpk
                           {
                               NoSpk = b.NoSPk,
                               Title = b.Title, // e.Judul,
                               Vendor = d.Nama,
                               //PIC = f.Nama,
                               //Divisi = e.UnitKerjaPemohon,
                               TanggalSPK = b.TanggalSPK.ToString(),
                               NilaiSPK = b.NilaiSPK.ToString(),

                           }).Distinct().ToList();
            return oReport;
        }

        public int AddVendorNonReg(Vendor vendor)
        {
            ctx.Vendors.Add(vendor);
            ctx.SaveChanges();
            return vendor.Id;
        }

        

        public ResultMessage saveDokumenSpkNonPks(DokumenSpkNonPks dokumenSpkNonPks, Guid UserId)
        {
            try
            {
                _log.Debug("var dokumenSpkNonPks.SpkId = {0}", dokumenSpkNonPks.SpkId);
                var oData = ctx.Spk.Find(dokumenSpkNonPks.SpkId);
                _log.Debug("var oData = {0}", oData);
                if (oData == null) return new ResultMessage
                {
                    status = HttpStatusCode.Forbidden,
                    message = Common.NotSave()
                };
                dokumenSpkNonPks.CreateOn = DateTime.Now;
                _log.Debug("var dokumenSpkNonPks.CreateOn = {0}", dokumenSpkNonPks.CreateOn);
                dokumenSpkNonPks.CreateBy = UserId;
                _log.Debug("var dokumenSpkNonPks.CreateBy = {0}", dokumenSpkNonPks.CreateBy);
                ctx.DokumenSpkNonPks.Add(dokumenSpkNonPks);
                ctx.SaveChanges();
                return new ResultMessage
                {
                    status = HttpStatusCode.OK,
                    message = Common.SaveSukses(),
                    Id = dokumenSpkNonPks.Id.ToString()
                };
            }
            catch (Exception ex)
            {
                return new ResultMessage()
                {
                    status = HttpStatusCode.NotImplemented,
                    message = ex.ToString()
                };
            }
        }

        public List<VWDokumenSPK> GetListDokumenSpkNonPks(string klasifikasi, Guid id)
        {
            return ctx.DokumenSpkNonPks.Where(d => d.SpkId == id && d.Klasifikasi == klasifikasi).Select(d => new VWDokumenSPK()
            {
                Id = d.Id,
                SpkId = d.SpkId,
                SizeFile = d.SizeFile,
                ContentType = d.ContentType,
                File = d.File
            }).ToList();
        }

        public DokumenSpkNonPks getDokSpkNonPks(Guid id, string klasifikasi)
        {
            var cek1 = ctx.DokumenSpkNonPks.Find(id);

            if (cek1 == null)
            {
                cek1 = ctx.DokumenSpkNonPks.Where(d => d.SpkId == id && d.Klasifikasi == klasifikasi).FirstOrDefault();
            }

            return cek1;
        }

        public int deleteDokumenSpkNonPks(Guid Id, Guid UserId, int approver)
        {
            try
            {
                DokumenSpkNonPks doku = ctx.DokumenSpkNonPks.Find(Id);
                int isMine = doku.CreateBy == UserId ? 1 : 0;
                if (doku.Spk.StatusSpk != StatusSpk.Draft) return 0;
                if (isMine == 1)
                {
                    ctx.DokumenSpkNonPks.Remove(doku);
                    ctx.SaveChanges(UserId.ToString());
                    return 1;
                }
                return 0;
            }
            catch { return 0; }
        }

        public ResultMessage saveAtributDokumenNonPks(Guid id, string klasifikasi, string NoDok, DateTime? TglDok)
        {
            try
            {
                DokumenSpkNonPks findDokNoPks = ctx.DokumenSpkNonPks.Where(x => x.SpkId == id && x.Klasifikasi == klasifikasi).FirstOrDefault();
                if (findDokNoPks == null) return new ResultMessage()
                {
                    message = HttpStatusCode.MethodNotAllowed.ToString(),
                    status = HttpStatusCode.MethodNotAllowed
                };
                if (id != Guid.Empty && id != null)
                {
                    var oldDokNoPks = ctx.DokumenSpkNonPks.Find(findDokNoPks.Id);
                    if (oldDokNoPks == null) return new ResultMessage();
                    oldDokNoPks.NoDokumen = NoDok;
                    oldDokNoPks.TglDokumen = TglDok;
                    ctx.SaveChanges();
                    return new ResultMessage()
                    {
                        Id = findDokNoPks.Id.ToString(),
                        message = Common.SaveSukses(),
                        status = HttpStatusCode.OK
                    };
                }
                else
                {
                    return new ResultMessage();
                    //DokumenSpkNonPks newDokNoPks = new DokumenSpkNonPks();
                    //spk.PksId = oldPks.Id;
                    //spk.PemenangPengadaanId = oldPks.PemenangPengadaanId;
                    //spk.CreateBy = UserId;
                    //spk.CreateOn = DateTime.Now;
                    //ctx.Spk.Add(spk);
                    //ctx.SaveChanges(UserId.ToString());
                    //return new ResultMessage()
                    //{
                    //    Id = spk.Id.ToString(),
                    //    message = Common.SaveSukses(),
                    //    status = HttpStatusCode.OK
                    //};
                }
            }
            catch (Exception ex)
            {
                return new ResultMessage()
                {
                    message = ex.ToString(),
                    status = HttpStatusCode.ExpectationFailed
                };

            }
        }

        public VWSpk detaildoknonpks(Guid Id, Guid UserId)
        {
            string a = "Aanwijzing";
            string b = "Memo";
            string c = "SubmitPenawaran";
            string d = "Klarifikasi";
            string e = "KlarifikasiLanjutan";
            string f = "Penilaian";
            string g = "UsulanPemenang";
            string h = "DokumenLain";
            List <DokumenSpkNonPks> doc = ctx.DokumenSpkNonPks.Where(t => t.SpkId == Id).ToList();
            return new VWSpk()
            {
                 NoAanwijzing = doc.Where(y => y.Klasifikasi == a).Select(y => y.NoDokumen).FirstOrDefault()
                ,TglAanwijzing = doc.Where(y => y.Klasifikasi == a).Select(y => y.TglDokumen).FirstOrDefault()
                ,NoMemo = doc.Where(y => y.Klasifikasi == b).Select(y => y.NoDokumen).FirstOrDefault()
                ,TglMemo = doc.Where(y => y.Klasifikasi == b).Select(y => y.TglDokumen).FirstOrDefault()
                ,NoSubPen = doc.Where(y => y.Klasifikasi == c).Select(y => y.NoDokumen).FirstOrDefault()
                ,TglSubPen = doc.Where(y => y.Klasifikasi == c).Select(y => y.TglDokumen).FirstOrDefault()
                ,NoKlarfNeg = doc.Where(y => y.Klasifikasi == d).Select(y => y.NoDokumen).FirstOrDefault()
                ,TglKlarfNeg = doc.Where(y => y.Klasifikasi == d).Select(y => y.TglDokumen).FirstOrDefault()
                ,NoKlarfNegLan = doc.Where(y => y.Klasifikasi == e).Select(y => y.NoDokumen).FirstOrDefault()
                ,TglKlarfNegLan = doc.Where(y => y.Klasifikasi == e).Select(y => y.TglDokumen).FirstOrDefault()
                ,NoPenilai = doc.Where(y => y.Klasifikasi == f).Select(y => y.NoDokumen).FirstOrDefault()
                ,TglPenilai = doc.Where(y => y.Klasifikasi == f).Select(y => y.TglDokumen).FirstOrDefault()
                ,NoUsPen = doc.Where(y => y.Klasifikasi == g).Select(y => y.NoDokumen).FirstOrDefault()
                ,TglUsPen = doc.Where(y => y.Klasifikasi == g).Select(y => y.TglDokumen).FirstOrDefault()
                ,NoDokLain = doc.Where(y => y.Klasifikasi == h).Select(y => y.NoDokumen).FirstOrDefault()
                ,TglDokLain = doc.Where(y => y.Klasifikasi == h).Select(y => y.TglDokumen).FirstOrDefault()
            };
        }

    }
}


