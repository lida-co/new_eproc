using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Validation;
using Model.Helper;
using Reston.Pinata.Model.PengadaanRepository.View;
using Reston.Pinata.Model;
using System.Configuration;
using Reston.Pinata.Model.JimbisModel;
using Reston.Pinata.Model.Helper;
using System.Net;
using Reston.Eproc.Model.Monitoring.Model;
using Reston.Eproc.Model.Monitoring.Entities;
using Reston.Pinata.Model.PengadaanRepository;
using Reston.Pinata.Model.Asuransi;
using Reston.Eproc.Model.Ext;
using System.Globalization;

namespace Reston.Eproc.Model.Ext
{
    public interface INilaiVendorRepo
    {
        DataTablePemenangPengadaanNilaiVendor GetDataListPemenangPengadaanNilaiVendor(string search, int start, int length);
        DataTablePemenangPengadaanNilaiVendor GetDataListPemenangPengadaanNilaiVendorsudah(string search, int start, int length);
        VWdetailSPKNilaiVendor GetdetailSPKNilaiVendor(string IdSPK);
        ResultMessage AddPertanyaan(VWTenderScoringHeaderExt vwtenderscoringheader, Guid userId);
        List<VWdetailSPKNilaiVendor> Getampilpersonilpenilai(string IdSPK);
        List<VWPersonilPenilaian> GetPersonPenilai(Guid Id, int VendorId);
        VWApprisalWorksheet CekPertanyaan(Guid Id, int VendorId);
        List<VWTenderScoring> GetPointPenilaian(Guid Id, int VendorId);
        List<VWVendor> GetAssessment(Guid Id, int VendorId);
        int nextToDelete(Guid Id, int VendorId);
        List<VWTenderScoring> GetValueAssessment(Guid Id, Guid UserIdAssessment, int VendorId);
        List<VWTenderScoring> GetQuestion(Guid Id, int VendorId);
        //List<VWPersonilPengadaan> GetDropDownPenilai(Guid Id);
        List<ViewPertanyaan> GetDataPertanyaan();
        ResultMessage AddAssessment(VWTenderScoringDetails vwtenderscoringuser, Guid Id, int VendorId, decimal Total, Guid UserId);
        DataTablePemenangPengadaanNilaiVendor GetDataListPemenangPengadaanNilaiVendorAssesment(string search, int start, int length, Guid UserId);
        DataTableVWVendorwithSanksi GetListVendorWithSanksi(string search, string status, string bidang, string kelompok, int start, int length);
        VWSanksi GetCekVendorSanksi(int VendorId);
        ResultMessage AddVendorSanksi(VWSanksi vwsanksi, Guid UserId);
        DataTableVWVendorwithSanksi GetListSanksi(string search, int start, int length);
        DataTableVWVendorwithSanksi GetListRiwayatSanksi(int VendorId, string search, int start, int length);
        DataTablePemenangPengadaanNilaiVendor GetListRiwayatPenilaian(int VendorId, string search, int start, int length);
        VendorExtViewModelJaws GetVendor(string noPengajuan);
        VendorExtViewModelJaws GetVendorDetailNew(int idVendor);
        RegDocumentImageExt GetDokumen(Guid Iddok);
        DataTableVWRekananPencarian GetrekananPencarian(string search, string status, string tipe, int start, int length);
        VWSanksi GetdetailSanksiVendor(int VendorId);
        int GetIdVendorFromOwner(Guid owner);
    }

    public class NilaiVendorRepo : INilaiVendorRepo
    {
        AppDbContext ctx;

        public NilaiVendorRepo(AppDbContext j)
        {
            ctx = j;
            //ctx.Configuration.ProxyCreationEnabled = false;
            ctx.Configuration.LazyLoadingEnabled = true;
        }

        public DataTablePemenangPengadaanNilaiVendor GetDataListPemenangPengadaanNilaiVendor(string search, int start, int length)
        {
            search = search == null ? "" : search;
            DataTablePemenangPengadaanNilaiVendor dtTable = new DataTablePemenangPengadaanNilaiVendor();
            if (length > 0)
            {
                IQueryable<PemenangPengadaan> data = ctx.PemenangPengadaans.AsQueryable();

                dtTable.recordsTotal = data.Count();
                //if (!string.IsNullOrEmpty(search))
                //{
                //    data = data.Where(d => d.Pengadaan.NoPengadaan.Contains(search) || d.Pengadaan.Judul.Contains(search) || d.Vendor.Nama.Contains(search));
                //}
                //data = data.Where(u => u.Pengadaan.GroupPengadaan == EGroupPengadaan.DALAMPELAKSANAAN);
                dtTable.recordsFiltered = data.Count();
                data = data.OrderByDescending(d => d.CreateOn).Skip(start).Take(length);

                dtTable.data = data.Select(d => new VWPemenangPengadaanNilaiVendor
                {
                    Id = d.PengadaanId,
                    NoPengadaan = ctx.Spk.Where(x => x.PemenangPengadaanId == d.Id).FirstOrDefault().NoSPk,
                    JudulPengadaan = d.Pengadaan.Judul,
                    NamaVendor = d.Vendor.Nama,
                    CekCreate = ctx.ApprisalWorksheets.Where(xx => xx.PengadaanId == d.PengadaanId && xx.VendorId == d.Vendor.Id).FirstOrDefault() == null ? "belum" : "sudah",
                    PIC = d.Pengadaan.PersonilPengadaans.Where(dd => dd.tipe == "pic").FirstOrDefault() == null ? "" : d.Pengadaan.PersonilPengadaans.Where(dd => dd.tipe == "pic").FirstOrDefault().Nama
                }).ToList();

                if (!string.IsNullOrEmpty(search))
                {
                    dtTable.data = dtTable.data.Where(d => d.NoPengadaan.Contains(search) || d.JudulPengadaan.Contains(search) || d.NamaVendor.Contains(search) || d.PIC.Contains(search)).ToList();
                }
                dtTable.data = dtTable.data.Where(d => d.NoPengadaan != null).ToList();
            }
            return dtTable;
        }

        public DataTablePemenangPengadaanNilaiVendor GetDataListPemenangPengadaanNilaiVendorsudah(string search, int start, int length)
        {
            search = search == null ? "" : search;
            DataTablePemenangPengadaanNilaiVendor dtTable = new DataTablePemenangPengadaanNilaiVendor();
            if (length > 0)
            {
                IQueryable<PemenangPengadaan> data = ctx.PemenangPengadaans.AsQueryable();

                dtTable.recordsTotal = data.Count();
                //if (!string.IsNullOrEmpty(search))
                //{
                //    data = data.Where(d => d.Pengadaan.NoPengadaan.Contains(search) || d.Pengadaan.Judul.Contains(search) || d.Vendor.Nama.Contains(search));
                //}
                //data = data.Where(u => u.Pengadaan.GroupPengadaan == EGroupPengadaan.DALAMPELAKSANAAN);
                dtTable.recordsFiltered = data.Count();
                data = data.OrderByDescending(d => d.CreateOn).Skip(start).Take(length);

                dtTable.data = data.Select(d => new VWPemenangPengadaanNilaiVendor
                {
                    Id = d.PengadaanId,
                    NoPengadaan = ctx.Spk.Where(x => x.PemenangPengadaanId == d.Id).FirstOrDefault().NoSPk,
                    JudulPengadaan = d.Pengadaan.Judul,
                    NamaVendor = d.Vendor.Nama,
                    CekCreate = ctx.TenderScoringHeaders.Where(xx => xx.PengadaanId == d.PengadaanId && xx.VendorId == d.Vendor.Id).FirstOrDefault() == null ? "belum" : "sudah",
                    PIC = d.Pengadaan.PersonilPengadaans.Where(dd => dd.tipe == "pic").FirstOrDefault() == null ? "" : d.Pengadaan.PersonilPengadaans.Where(dd => dd.tipe == "pic").FirstOrDefault().Nama
                }).ToList();

                if (!string.IsNullOrEmpty(search))
                {
                    dtTable.data = dtTable.data.Where(d => d.NoPengadaan.Contains(search) || d.JudulPengadaan.Contains(search) || d.NamaVendor.Contains(search) || d.PIC.Contains(search)).ToList();
                }
                dtTable.data = dtTable.data.Where(d => d.CekCreate == "sudah").ToList();

            }
            return dtTable;
        }


        public VWdetailSPKNilaiVendor GetdetailSPKNilaiVendor(string IdSPK)
        {
            VWdetailSPKNilaiVendor Getdetail = (from a in ctx.Spk
                                                join b in ctx.PemenangPengadaans on a.PemenangPengadaanId equals b.Id
                                                join c in ctx.Pengadaans on b.PengadaanId equals c.Id
                                                join d in ctx.Vendors on b.VendorId equals d.Id
                                                where a.NoSPk == IdSPK
                                                select new VWdetailSPKNilaiVendor
                                                {
                                                    JudulPengadaan = c.Judul,
                                                    Deskripsi = c.Keterangan,
                                                    IdSPK = a.NoSPk,
                                                    pengadaanId = c.Id,
                                                    PemenangPengadaan = b.Vendor.Nama,
                                                    VendorId = b.Vendor.Id,
                                                    CekCreate = ctx.ApprisalWorksheets.Where(xx => xx.PengadaanId == c.Id && xx.VendorId == b.Vendor.Id).FirstOrDefault() == null ? "belum" : "sudah"
                                                }).FirstOrDefault();


            int counter = 0;
            int counterAll = 0;
            decimal total = 0;
            decimal average = 0;

            //ApprisalWorksheet a = ctx.ApprisalWorksheets.Find(WorksheetId);
            if (Getdetail.CekCreate == "sudah")
            {
                var WorksheetId = ctx.ApprisalWorksheets.Where(x => x.PengadaanId == Getdetail.pengadaanId && x.VendorId == Getdetail.VendorId).FirstOrDefault().Id;
                var cek = ctx.ApprisalWorksheetResponses.Where(xx => xx.ApprisalWorksheetId == WorksheetId).ToList();
                counterAll = cek.Count();
                foreach (var i in cek)
                {
                    var cekpenilai = ctx.ApprisalWorksheetResposeDetails.Where(xx => xx.ApprisalWorksheetResposeId == i.Id).FirstOrDefault() == null ? "belum" : "sudah";
                    if (cekpenilai == "sudah") counter = counter + 1;
                }

                total = ctx.ApprisalWorksheets.Where(xx => xx.Id == WorksheetId).FirstOrDefault().CurrentTotal;
                average = ctx.ApprisalWorksheets.Where(xx => xx.Id == WorksheetId).FirstOrDefault().CurrentAverage;
            }

            VWdetailSPKNilaiVendor Getdetails = new VWdetailSPKNilaiVendor();
            Getdetails.JudulPengadaan = Getdetail.JudulPengadaan;
            Getdetails.Deskripsi = Getdetail.Deskripsi;
            Getdetails.IdSPK = Getdetail.IdSPK;
            Getdetails.pengadaanId = Getdetail.pengadaanId;
            Getdetails.PemenangPengadaan = Getdetail.PemenangPengadaan;
            Getdetails.VendorId = Getdetail.VendorId;
            Getdetails.CekCreate = Getdetail.CekCreate;
            Getdetails.average = average;
            Getdetails.total = total;
            Getdetails.counter = counter;
            Getdetails.counterAll = counterAll;

            return Getdetails;
        }

        public ResultMessage AddPertanyaan(VWTenderScoringHeaderExt vwtenderscoringheader, Guid userId)
        {
            ResultMessage result = new ResultMessage();
            try
            {
                int venId = 0;
                Guid scorhead = Guid.Empty;
                //TenderScoringHeader tenderScoringHeader = new TenderScoringHeader();
                //foreach (var i in vwtenderscoringheader.VendorId)
                //{
                //    tenderScoringHeader.PengadaanId = vwtenderscoringheader.PengadaanId;
                //    tenderScoringHeader.VendorId = Convert.ToInt32(i.Id);
                //    tenderScoringHeader.Total = Convert.ToInt32(vwtenderscoringheader.Total);
                //    tenderScoringHeader.Average = Convert.ToDecimal(vwtenderscoringheader.Averages);
                //    ctx.TenderScoringHeaders.Add(tenderScoringHeader);
                //    ctx.SaveChanges();
                //    venId = Convert.ToInt32(i.Id);
                //}

                ApprisalWorksheet ApprisalWorksheet = new ApprisalWorksheet();
                foreach (var i in vwtenderscoringheader.VendorId)
                {
                    ApprisalWorksheet.PengadaanId = vwtenderscoringheader.PengadaanId;
                    //ApprisalWorksheet.NoSPk = 
                    ApprisalWorksheet.VendorId = Convert.ToInt32(i.Id);
                    ApprisalWorksheet.CreatedOn = DateTime.Now;
                    ApprisalWorksheet.CreatedBy = userId;
                    //tenderScoringHeader.PengadaanId = vwtenderscoringheader.PengadaanId;
                    //tenderScoringHeader.VendorId = Convert.ToInt32(i.Id);
                    //tenderScoringHeader.Total = Convert.ToInt32(vwtenderscoringheader.Total);
                    //tenderScoringHeader.Average = Convert.ToDecimal(vwtenderscoringheader.Averages);
                    ctx.ApprisalWorksheets.Add(ApprisalWorksheet);
                    ctx.SaveChanges();
                    venId = Convert.ToInt32(i.Id);
                }

                //TenderScoringBobot tsb = new TenderScoringBobot();
                //foreach (var i in vwtenderscoringheader.TenderScoringBobot)
                //{
                //    tsb.PengadaanId = vwtenderscoringheader.PengadaanId;
                //    tsb.Code = i.Code;
                //    tsb.Bobot = i.Bobot;
                //    ctx.TenderScoringBobots.Add(tsb);
                //    ctx.SaveChanges();
                //}

                var apwd = ctx.ApprisalWorksheets.Where(x => x.PengadaanId == vwtenderscoringheader.PengadaanId && x.VendorId == venId).FirstOrDefault().Id;
                ApprisalWorksheetDetail ApprisalWorksheetDetail = new ApprisalWorksheetDetail();
                foreach (var i in vwtenderscoringheader.TenderScoringBobot)
                {
                    ApprisalWorksheetDetail.ApprisalWorksheetId = apwd;
                    ApprisalWorksheetDetail.QuestionCode = i.Code;
                    ApprisalWorksheetDetail.Weight = i.Bobot;
                    ctx.ApprisalWorksheetDetails.Add(ApprisalWorksheetDetail);
                    ctx.SaveChanges();
                }

                ApprisalWorksheetResponse ApprisalWorksheetResponse = new ApprisalWorksheetResponse();
                foreach (var i in vwtenderscoringheader.TenderScoringPenilais)
                {
                    ApprisalWorksheetResponse.ApprisalWorksheetId = apwd;
                    ApprisalWorksheetResponse.AppriserUserId = i.UserId;
                    ApprisalWorksheetResponse.CreatedOn = DateTime.Now;
                    ApprisalWorksheetResponse.CreatedBy = userId;
                    ctx.ApprisalWorksheetResponses.Add(ApprisalWorksheetResponse);
                    ctx.SaveChanges();
                }


                //var tshid = ctx.TenderScoringHeaders.Where(x => x.PengadaanId == vwtenderscoringheader.PengadaanId).ToList();
                //TenderScoringDetail tenderScoringDetail = new TenderScoringDetail();
                //foreach (var i in tshid)
                //{
                //    scorhead = i.Id;
                //    foreach (var a in vwtenderscoringheader.TenderScoringDetails)
                //    {
                //        tenderScoringDetail.TenderScoringHeaderId = i.Id;
                //        tenderScoringDetail.Code = a.Code;
                //        tenderScoringDetail.TotalAllUser = Convert.ToInt32(a.Total_All_User);
                //        tenderScoringDetail.AverageAllUser = Convert.ToDecimal(a.Averages_All_User);
                //        ctx.TenderScoringDetails.Add(tenderScoringDetail);
                //        ctx.SaveChanges();
                //    }
                //}


                //TenderScoringPenilai tenderScoringPenilai = new TenderScoringPenilai();
                //foreach (var i in vwtenderscoringheader.TenderScoringPenilais)
                //{
                //    tenderScoringPenilai.TenderScoringHeaderId = scorhead;
                //    tenderScoringPenilai.PengadaanId = vwtenderscoringheader.PengadaanId;
                //    tenderScoringPenilai.UserId = i.UserId;
                //    tenderScoringPenilai.VendorId = venId;
                //    ctx.TenderScoringPenilais.Add(tenderScoringPenilai);
                //    ctx.SaveChanges();
                //}

                result.message = Common.UpdateSukses();
                result.status = System.Net.HttpStatusCode.OK;
                result.Id = vwtenderscoringheader.PengadaanId.ToString();
                return result;
            }
            catch (Exception ex)
            {
                result.message = ex.ToString();
                result.status = System.Net.HttpStatusCode.InternalServerError;
                return result;
            }

        }

        //public List<VWTenderScoring> GetPointPenilaian(Guid Id)
        //{
        //    List<VWTenderScoring> vwTenderScoring = (from b in ctx.TenderScoringDetails
        //                                             join c in ctx.TenderScoringHeaders on b.TenderScoringHeaderId equals c.Id
        //                                             join d in ctx.TenderScoringBobots on b.Code equals d.Code
        //                                             join e in ctx.ReferenceDatas on b.Code equals e.Code
        //                                             //where c.PengadaanId == Id <-Punya si-Gondrong
        //                                             where c.PengadaanId == Id && d.PengadaanId == Id
        //                                             select new VWTenderScoring
        //                                             {
        //                                                 Code = b.Code,
        //                                                 LocalizedName = e.LocalizedName,
        //                                                 Bobot = d.Bobot
        //                                             }).Distinct().ToList();
        //    return vwTenderScoring;
        //}


        public List<VWdetailSPKNilaiVendor> Getampilpersonilpenilai(string IdSPK)
        {
            var Getdetail = (from a in ctx.Spk
                             join b in ctx.PemenangPengadaans on a.PemenangPengadaanId equals b.Id
                             join c in ctx.Pengadaans on b.PengadaanId equals c.Id
                             join d in ctx.Vendors on b.VendorId equals d.Id
                             join e in ctx.TenderScoringPenilais on b.PengadaanId equals e.PengadaanId
                             where a.NoSPk == IdSPK && e.VendorId == b.Vendor.Id
                             select new VWdetailSPKNilaiVendor
                             {
                                 personilId = e.UserId
                             }).ToList();
            return Getdetail;
        }


        public List<VWPersonilPenilaian> GetPersonPenilai(Guid Id, int VendorId)
        {
            List<VWPersonilPenilaian> VWPersonilPenilaian = (from a in ctx.ApprisalWorksheets
                                                             join b in ctx.ApprisalWorksheetResponses on a.Id equals b.ApprisalWorksheetId
                                                             where a.PengadaanId == Id && a.VendorId == VendorId
                                                             select new VWPersonilPenilaian
                                                             {
                                                                 AppriserUserId = b.AppriserUserId
                                                             }).ToList();
            return VWPersonilPenilaian;
        }

        public VWApprisalWorksheet CekPertanyaan(Guid Id, int VendorId)
        {
            VWApprisalWorksheet cekpertanyaan = (from b in ctx.ApprisalWorksheets
                                                 where b.PengadaanId == Id && b.VendorId == VendorId
                                                 select new VWApprisalWorksheet
                                                 {
                                                     PengadaanId = b.PengadaanId,
                                                     VendorId = b.VendorId
                                                 }).FirstOrDefault();
            return cekpertanyaan;
        }

        public List<VWTenderScoring> GetPointPenilaian(Guid Id, int VendorId)
        {
            List<VWTenderScoring> vwTenderScoring = (from b in ctx.ApprisalWorksheets
                                                     join c in ctx.ApprisalWorksheetDetails on b.Id equals c.ApprisalWorksheetId
                                                     join d in ctx.ReferenceDatas on c.QuestionCode equals d.Code
                                                     //where c.PengadaanId == Id <-Punya si-Gondrong
                                                     where b.PengadaanId == Id && b.VendorId == VendorId
                                                     select new VWTenderScoring
                                                     {
                                                         Code = c.QuestionCode,
                                                         LocalizedName = d.LocalizedName,
                                                         Bobot = c.Weight
                                                     }).Distinct().ToList();
            return vwTenderScoring;
        }

        public List<VWVendor> GetAssessment(Guid Id, int VendorId)
        {
            List<VWVendor> vendor = (from b in ctx.Vendors
                                     join c in ctx.ApprisalWorksheets on b.Id equals c.VendorId
                                     where c.PengadaanId == Id && c.VendorId == VendorId
                                     select new VWVendor
                                     {
                                         Id = b.Id,
                                         Nama = b.Nama
                                     }).ToList();
            return vendor;
        }



        public int nextToDelete(Guid Id, int VendorId)
        {
            try
            {
                var WorksheetId = ctx.ApprisalWorksheets.Where(x => x.PengadaanId == Id && x.VendorId == VendorId).FirstOrDefault().Id;
                var WorksheetResponseId = ctx.ApprisalWorksheetResponses.Where(x => x.ApprisalWorksheetId == WorksheetId).FirstOrDefault().Id;

                List<ApprisalWorksheetResposeDetail> resdet = ctx.ApprisalWorksheetResposeDetails.Where(x => x.ApprisalWorksheetResposeId == WorksheetResponseId).ToList();
                foreach (var a in resdet)
                {
                    ctx.ApprisalWorksheetResposeDetails.Remove(a);
                    ctx.SaveChanges();
                }

                List<ApprisalWorksheetResponse> res = ctx.ApprisalWorksheetResponses.Where(x => x.ApprisalWorksheetId == WorksheetId).ToList();
                foreach (var b in res)
                {
                    ctx.ApprisalWorksheetResponses.Remove(b);
                    ctx.SaveChanges();
                }

                List<ApprisalWorksheetDetail> sheetdet = ctx.ApprisalWorksheetDetails.Where(x => x.ApprisalWorksheetId == WorksheetId).ToList();
                foreach (var c in sheetdet)
                {
                    ctx.ApprisalWorksheetDetails.Remove(c);
                    ctx.SaveChanges();
                }

                List<ApprisalWorksheet> sheet = ctx.ApprisalWorksheets.Where(x => x.Id == WorksheetId).ToList();
                foreach (var d in sheet)
                {
                    ctx.ApprisalWorksheets.Remove(d);
                    ctx.SaveChanges();
                }

                return 1;
            }
            catch { return 0; }

        }

        //public List<VWPersonilPengadaan> GetDropDownPenilai(Guid Id)
        //{
        //    List<VWPersonilPengadaan> listpenilai = (from b in ctx.PersonilPengadaans
        //                                             where b.PengadaanId == Id
        //                                             select new VWPersonilPengadaan
        //                                             {
        //                                                 PersonilId = b.PersonilId,
        //                                                 Nama = b.Nama,
        //                                                 Jabatan = b.Jabatan,
        //                                                 PengadaanId = b.PengadaanId
        //                                             }).ToList();
        //    return listpenilai;
        //}

        public List<VWTenderScoring> GetValueAssessment(Guid Id, Guid UserIdAssessment, int VendorId)
        {
            var WorksheetId = ctx.ApprisalWorksheets.Where(x => x.PengadaanId == Id && x.VendorId == VendorId).FirstOrDefault().Id;
            var WorksheetResponseId = ctx.ApprisalWorksheetResponses.Where(x => x.ApprisalWorksheetId == WorksheetId && x.AppriserUserId == UserIdAssessment).FirstOrDefault().Id;

            List<VWTenderScoring> valtenderscoringusers = (from a in ctx.ApprisalWorksheetResponses
                                                           join b in ctx.ApprisalWorksheetResposeDetails on a.Id equals b.ApprisalWorksheetResposeId
                                                           where a.ApprisalWorksheetId == WorksheetId && b.ApprisalWorksheetResposeId == WorksheetResponseId
                                                           select new VWTenderScoring
                                                           {
                                                               PengadaanId = Id,
                                                               VendorId = VendorId,
                                                               Code = b.QuestionCode,
                                                               LocalizedName = ctx.ReferenceDatas.Where(xx => xx.Code == b.QuestionCode).FirstOrDefault().LocalizedName,
                                                               UserId = UserIdAssessment,
                                                               Score = b.Score,
                                                               Bobot = ctx.ApprisalWorksheetDetails.Where(xx => xx.ApprisalWorksheetId == WorksheetId && xx.QuestionCode == b.QuestionCode).FirstOrDefault().Weight
                                                           }).Distinct().ToList();

            // List<VWTenderScoring> valtenderscoringuser = (from b in ctx.ApprisalWorksheets
            //                                               join c in ctx.ApprisalWorksheetDetails on b.Id equals c.ApprisalWorksheetId
            //                                               join d in ctx.ApprisalWorksheetResponses on b.Id equals d.ApprisalWorksheetId
            //                                               join f in ctx.ApprisalWorksheetResposeDetails on d.Id equals f.ApprisalWorksheetResposeId
            //                                               join e in ctx.ReferenceDatas on f.QuestionCode equals e.Code
            //                                               //where d.VendorId == VendorId && d.PengadaanId == Id && b.UserId == UserIdAssessment <-Punya si-Gondrong
            //                                               where b.VendorId == VendorId && b.PengadaanId == Id && d.AppriserUserId == UserIdAssessment
            //                                               select new VWTenderScoring
            //                                               {
            //                                                   PengadaanId = b.PengadaanId,
            //                                                   VendorId = b.VendorId,
            //                                                   Code = e.Code,
            //                                                   LocalizedName = e.LocalizedName,
            //                                                   UserId = d.AppriserUserId,
            //                                                   Score = f.Score,
            //                                                   Bobot = c.Weight
            //                                               }).Distinct().ToList();
            return valtenderscoringusers;
        }

        public List<VWTenderScoring> GetQuestion(Guid Id, int VendorId)
        {
            List<VWTenderScoring> refdata = (from b in ctx.ApprisalWorksheets
                                             join c in ctx.ApprisalWorksheetDetails on b.Id equals c.ApprisalWorksheetId
                                             join e in ctx.ReferenceDatas on c.QuestionCode equals e.Code
                                             // join f in ctx.ApprisalWorksheetResposeDetails on c.QuestionCode equals f.QuestionCode
                                             //where d.VendorId == VendorId && d.PengadaanId == Id && b.UserId == UserIdAssessment <-Punya si-Gondrong
                                             where b.VendorId == VendorId && b.PengadaanId == Id
                                             select new VWTenderScoring
                                             {
                                                 TenderScoringDetailId = b.Id,
                                                 Code = c.QuestionCode,
                                                 LocalizedName = e.LocalizedName,
                                                 Bobot = c.Weight
                                             }).Distinct().ToList();
            return refdata;
        }

        public List<ViewPertanyaan> GetDataPertanyaan()
        {
            var pertanyaan = ctx.ReferenceDatas.Where(x => x.Qualifier.Equals("QUALIFIER_VENDOR_CRITERIA")).ToList();
            List<ViewPertanyaan> listpertanyaan = new List<ViewPertanyaan>();
            foreach (var item in pertanyaan)
            {
                ViewPertanyaan vp = new ViewPertanyaan();
                vp.Code = item.Code;
                vp.LocalizedName = item.LocalizedName;
                listpertanyaan.Add(vp);
            }
            return listpertanyaan;
        }


        public ResultMessage AddAssessment(VWTenderScoringDetails vwtenderscoringdetail, Guid Id, int VendorId, decimal Total, Guid UserId)
        {
            ResultMessage result = new ResultMessage();
            try
            {
                var WorksheetId = ctx.ApprisalWorksheets.Where(x => x.PengadaanId == Id && x.VendorId == VendorId).FirstOrDefault().Id;
                var WorksheetResponseId = ctx.ApprisalWorksheetResponses.Where(x => x.ApprisalWorksheetId == WorksheetId && x.AppriserUserId == UserId).FirstOrDefault().Id;

                ApprisalWorksheetResposeDetail responDetail = new ApprisalWorksheetResposeDetail();
                foreach (var i in vwtenderscoringdetail.TenderScoringUser)
                {
                    responDetail.ApprisalWorksheetResposeId = WorksheetResponseId;
                    responDetail.QuestionCode = i.code;
                    responDetail.Score = i.Score;
                    ctx.ApprisalWorksheetResposeDetails.Add(responDetail);
                    ctx.SaveChanges();
                }

                decimal jum = 0;
                ApprisalWorksheet a = ctx.ApprisalWorksheets.Find(WorksheetId);
                var cek = ctx.ApprisalWorksheetResponses.Where(xx => xx.ApprisalWorksheetId == WorksheetId).ToList();
                foreach (var i in cek)
                {
                    var cekpenilai = ctx.ApprisalWorksheetResposeDetails.Where(xx => xx.ApprisalWorksheetResposeId == i.Id).FirstOrDefault() == null ? "belum" : "sudah";
                    if (cekpenilai == "sudah") jum = jum + 1;
                }

                //decimal average = 0;
                if (jum != 0)
                {
                    a.CurrentAverage = (a.CurrentTotal + Total) / jum;
                }
                else
                {
                    a.CurrentAverage = Total;
                }
                a.CurrentTotal = a.CurrentTotal + Total;
                ctx.SaveChanges();

                //    TenderScoringUser tenderScoringUser = new TenderScoringUser();
                //foreach (var i in vwtenderscoringdetail.TenderScoringUser)
                //{
                //    
                //    tenderScoringUser.TenderScoringDetailId = i.VWTenderScoringDetailId;
                //    tenderScoringUser.UserId = UserId;
                //    tenderScoringUser.Score = i.Score;
                //    ctx.TenderScoringUsers.Add(tenderScoringUser);
                //    ctx.SaveChanges();
                //}
                //
                //var countuser = (from b in ctx.TenderScoringUsers
                //                 join c in ctx.TenderScoringDetails on b.TenderScoringDetailId equals c.Id
                //                 join d in ctx.TenderScoringHeaders on c.TenderScoringHeaderId equals d.Id
                //                 where d.PengadaanId == Id
                //                 select b.UserId).Distinct().Count();
                //
                //var tsh = ctx.TenderScoringHeaders.Where(x => x.PengadaanId == Id && x.VendorId == VendorId).FirstOrDefault();
                //var tshid = tsh.Id;
                //var total_tsh = tsh.Total;
                //var average_tsh = tsh.Average;
                //tsh.Total = Total + total_tsh;
                //tsh.Average = Convert.ToDecimal(tsh.Total) / Convert.ToDecimal(countuser);
                //ctx.SaveChanges();
                //
                ////var tsd = ctx.TenderScoringDetails.Where(x => x.TenderScoringHeaderId == tshid).ToList();
                //TenderScoringDetail tenderScoringDetail = new TenderScoringDetail();
                //foreach (var a in vwtenderscoringdetail.TenderScoringUser)
                //{
                //    var tsd = ctx.TenderScoringDetails.Where(x => x.TenderScoringHeaderId == tshid && x.Id == a.VWTenderScoringDetailId).ToList();
                //
                //    ////perhitungan nilai bobot
                //    //foreach (var i in vwtenderscoringdetail.TenderScoringUser)
                //    //{
                //
                //    //}
                //
                //    foreach (var i in tsd)
                //    {
                //        var x = a.Bobot;
                //        decimal y = 100;
                //        var z = x / y;
                //        i.TotalAllUser = i.TotalAllUser + (a.Score * z);
                //        i.AverageAllUser = Convert.ToDecimal(i.TotalAllUser) / Convert.ToDecimal(countuser);
                //        ctx.SaveChanges();
                //    }
                //}

                result.message = Common.UpdateSukses();
                result.status = System.Net.HttpStatusCode.OK;
                //result.Id = tenderScoringHeader.PengadaanId.ToString();
                return result;
            }
            catch (Exception ex)
            {
                result.message = ex.ToString();
                result.status = System.Net.HttpStatusCode.InternalServerError;
                return result;
            }

        }



        public DataTablePemenangPengadaanNilaiVendor GetDataListPemenangPengadaanNilaiVendorAssesment(string search, int start, int length, Guid UserId)
        {

            search = search == null ? "" : search;

            DataTablePemenangPengadaanNilaiVendor tp = new DataTablePemenangPengadaanNilaiVendor();
            // record total yang tampil 
            tp.recordsTotal = (from b in ctx.ApprisalWorksheets
                               join d in ctx.ApprisalWorksheetResponses on b.Id equals d.ApprisalWorksheetId
                               select new VWTenderScoring
                               {
                                   PengadaanId = b.PengadaanId,
                                   VendorId = b.VendorId,
                                   UserId = d.AppriserUserId
                               }).Distinct().Count();


            tp.recordsFiltered = (from b in ctx.ApprisalWorksheets
                                  join d in ctx.ApprisalWorksheetResponses on b.Id equals d.ApprisalWorksheetId
                                  where d.AppriserUserId == UserId
                                  select new VWTenderScoring
                                  {
                                      PengadaanId = b.PengadaanId,
                                      VendorId = b.VendorId,
                                      UserId = d.AppriserUserId
                                  }).Distinct().Count();


            var VWPemenangPengadaanNilaiVendorss = (
                                                    from b in ctx.ApprisalWorksheets
                                                    join d in ctx.ApprisalWorksheetResponses on b.Id equals d.ApprisalWorksheetId
                                                    //where d.VendorId == VendorId && d.PengadaanId == Id && b.UserId == UserIdAssessment <-Punya si-Gondrong
                                                    where d.AppriserUserId == UserId
                                                    select new VWPemenangPengadaanNilaiVendor
                                                    {
                                                        Id = b.PengadaanId,
                                                        NoPengadaan = ctx.Spk.Where(x => x.PemenangPengadaan.Pengadaan.Id == b.PengadaanId && x.PemenangPengadaan.Vendor.Id == b.VendorId).FirstOrDefault().NoSPk,
                                                        JudulPengadaan = ctx.PemenangPengadaans.Where(x => x.PengadaanId == b.PengadaanId && x.Vendor.Id == b.VendorId).FirstOrDefault().Pengadaan.Judul,
                                                        NamaVendor = ctx.PemenangPengadaans.Where(x => x.PengadaanId == b.PengadaanId && x.Vendor.Id == b.VendorId).FirstOrDefault().Vendor.Nama,
                                                        VendorId = b.VendorId,
                                                        CekCreate = ctx.ApprisalWorksheets.Where(xx => xx.PengadaanId == b.PengadaanId && xx.VendorId == b.VendorId).FirstOrDefault() == null ? "belum" : "sudah",
                                                        SudahNilai = ctx.ApprisalWorksheetResposeDetails.Where(zz => zz.ApprisalWorksheetResposeId == d.Id).FirstOrDefault() == null ? "belum" : "sudah",
                                                        PIC = ctx.PersonilPengadaans.Where(cc => cc.PengadaanId == b.PengadaanId && cc.tipe == "pic").FirstOrDefault().Nama == null ? "" : ctx.PersonilPengadaans.Where(cc => cc.PengadaanId == b.PengadaanId && cc.tipe == "pic").FirstOrDefault().Nama
                                                    }).Distinct().ToList();

            List<VWPemenangPengadaanNilaiVendor> Ulu = new List<VWPemenangPengadaanNilaiVendor>();
            foreach (var item in VWPemenangPengadaanNilaiVendorss)
            {
                VWPemenangPengadaanNilaiVendor Ula = new VWPemenangPengadaanNilaiVendor();
                Ula.Id = item.Id;
                Ula.NoPengadaan = item.NoPengadaan;
                Ula.JudulPengadaan = item.JudulPengadaan;
                Ula.NamaVendor = item.NamaVendor;
                Ula.VendorId = item.VendorId;
                Ula.CekCreate = item.CekCreate;
                Ula.SudahNilai = item.SudahNilai;
                Ula.PIC = item.PIC;
                Ulu.Add(Ula);
            }

            tp.data = Ulu.Select(
                aa => new VWPemenangPengadaanNilaiVendor
                {
                    Id = aa.Id,
                    NoPengadaan = aa.NoPengadaan,
                    JudulPengadaan = aa.JudulPengadaan,
                    NamaVendor = aa.NamaVendor,
                    VendorId = aa.VendorId,
                    CekCreate = aa.CekCreate,
                    SudahNilai = aa.SudahNilai,
                    PIC = aa.PIC
                }).ToList();



            if (!string.IsNullOrEmpty(search))
            {
                tp.data = tp.data.Where(d => d.NoPengadaan.Contains(search) || d.JudulPengadaan.Contains(search) || d.NamaVendor.Contains(search) || d.PIC.Contains(search)).ToList();
            }

            return tp;
        }

        public DataTableVWVendorwithSanksi GetListVendorWithSanksi(string search, string status, string bidang, string kelompok, int start, int length)
        {

            search = search == null ? "" : search;
            status = status == null ? "" : status;

            DataTableVWVendorwithSanksi tp = new DataTableVWVendorwithSanksi();
            // record total yang tampil 
            tp.recordsTotal = (from b in ctx.Vendors
                               join c in ctx.Sanksis on b.Id equals c.VendorId into ps
                               from p in ps.DefaultIfEmpty()
                               select new VWSanksi
                               {
                                   VendorId = b.Id,
                                   NomorVendor = b.NomorVendor,
                                   DecisionTypeCode = p.DecisionTypeCode,
                                   DecisionDescription = p.DecisionDescription
                               }).Distinct().Count();

            tp.recordsFiltered = tp.recordsTotal;

            var ListVendorWithSanksi = (from b in ctx.Vendors
                                        join c in ctx.Sanksis on b.Id equals c.VendorId into ps
                                        //join c in q on b.Id equals c.VendorId into ps
                                        from p in ps.DefaultIfEmpty()
                                        select new VWSanksi
                                        {
                                            VendorId = b.Id,
                                            NamaVendor = b.Nama,
                                            NomorVendor = b.NomorVendor,
                                            Bidangvendor = ctx.VendorExts.Where(x => x.VendorId == b.Id).FirstOrDefault() == null ? "" : ctx.VendorExts.Where(x => x.VendorId == b.Id).FirstOrDefault().SegBidangUsahaCode,
                                            kelompokvendor = ctx.VendorExts.Where(x => x.VendorId == b.Id).FirstOrDefault() == null ? "" : ctx.VendorExts.Where(x => x.VendorId == b.Id).FirstOrDefault().SegKelompokUsahaCode,
                                            DecisionTypeCode = String.IsNullOrEmpty(p.DecisionTypeCode) ? "SV01" : p.DecisionTypeCode,
                                            DecisionDescription = String.IsNullOrEmpty(p.DecisionDescription) ? "" : p.DecisionDescription,
                                            DecisionValidUntil = String.IsNullOrEmpty(p.DecisionValidUntil.ToString()) ? DateTime.MinValue : p.DecisionValidUntil,
                                            CreatedOn = String.IsNullOrEmpty(p.DecisionValidUntil.ToString()) ? DateTime.MinValue : p.CreatedOn,
                                        }).Distinct().OrderByDescending(xx => xx.CreatedOn).ToList();

            List<VWSanksi> Ulu = new List<VWSanksi>();
            foreach (var item in ListVendorWithSanksi)
            {
                VWSanksi Ula = new VWSanksi();
                Ula.VendorId = item.VendorId;
                Ula.NamaVendor = item.NamaVendor;
                Ula.NomorVendor = item.NomorVendor;
                Ula.Bidangvendor = item.Bidangvendor;
                Ula.kelompokvendor = item.kelompokvendor;
                Ula.DecisionTypeCode = IsDateBeforeOrToday(item.DecisionValidUntil) == false ? "SV01" : item.DecisionTypeCode;
                Ula.DecisionDescription = IsDateBeforeOrToday(item.DecisionValidUntil) == false ? "" : item.DecisionDescription;
                Ula.DalamMasaSanksi = IsDateBeforeOrToday(item.DecisionValidUntil) == true ? "masih" : "tidak";
                var check = Ulu.Where(xx => xx.VendorId == Ula.VendorId).FirstOrDefault() == null ? "belum" : "ada";
                if (check == "belum") Ulu.Add(Ula);
            }

            tp.data = Ulu.Select(
                aa => new VWSanksi
                {
                    VendorId = aa.VendorId,
                    NamaVendor = aa.NamaVendor,
                    NomorVendor = aa.NomorVendor,
                    Bidangvendor = aa.Bidangvendor,
                    kelompokvendor = aa.kelompokvendor,
                    DecisionTypeCode = aa.DecisionTypeCode,
                    DecisionDescription = aa.DecisionDescription
                }).ToList();



            if (!string.IsNullOrEmpty(search))
            {
                tp.data = tp.data.Where(d => d.NamaVendor.Contains(search) || d.NomorVendor.Contains(search)).ToList();
            }

            if (!string.IsNullOrEmpty(status))
            {
                if (status == "SV01")
                {
                    tp.data = tp.data.Where(d => d.DecisionTypeCode != "SV02" && d.DecisionTypeCode != "SV03" && d.DecisionTypeCode != "SV04").ToList();
                }
                else
                {
                    tp.data = tp.data.Where(d => d.DecisionTypeCode.Contains(status)).ToList();
                }
            }

            if (!string.IsNullOrEmpty(bidang))
            {
                tp.data = tp.data.Where(d => d.Bidangvendor.Contains(bidang)).ToList();
            }

            if (string.IsNullOrEmpty(bidang)) kelompok = "";
            if (!string.IsNullOrEmpty(kelompok))
            {
                if (kelompok != "null")
                {
                    List<VWSanksi> dataKel = new List<VWSanksi>();

                    char[] delimiterChars = { ',' };
                    string text = kelompok.Replace("\"", "").Replace("[", "").Replace("]", "");
                    string[] words = text.Split(delimiterChars);
                    foreach (var word in words)
                    {
                        string a = word.ToString();
                        var ads = tp.data.Where(d => d.kelompokvendor.Contains(a)).ToList();
                        foreach (var i in ads)
                        {
                            var check = dataKel.Where(xx => xx.VendorId == i.VendorId).FirstOrDefault() == null ? "belum" : "ada";
                            if (check == "belum") dataKel.Add(i);

                        }
                    }

                    tp.data = dataKel;
                }
            }

            return tp;
        }

        public VWSanksi GetCekVendorSanksi(int VendorId)
        {

            VWSanksi ListVendorWithSanksi = (from b in ctx.Vendors
                                             join c in ctx.Sanksis on b.Id equals c.VendorId into ps
                                             from p in ps.DefaultIfEmpty()
                                             where b.Id == VendorId
                                             select new VWSanksi
                                             {
                                                 VendorId = b.Id,
                                                 NamaVendor = b.Nama,
                                                 NomorVendor = b.NomorVendor,
                                                 DecisionTypeCode = String.IsNullOrEmpty(p.DecisionTypeCode) ? "SV01" : p.DecisionTypeCode,
                                                 DecisionDescription = String.IsNullOrEmpty(p.DecisionDescription) ? "" : p.DecisionDescription
                                             }).Distinct().FirstOrDefault();

            return ListVendorWithSanksi;
        }


        public ResultMessage AddVendorSanksi(VWSanksi vwsanksi, Guid UserId)
        {
            ResultMessage result = new ResultMessage();
            try
            {
                //var WorksheetId = ctx.ApprisalWorksheets.Where(x => x.PengadaanId == Id && x.VendorId == VendorId).FirstOrDefault().Id;
                //var WorksheetResponseId = ctx.ApprisalWorksheetResponses.Where(x => x.ApprisalWorksheetId == WorksheetId && x.AppriserUserId == UserId).FirstOrDefault().Id;
                //
                Sanksi sanksi = new Sanksi();

                sanksi.VendorId = vwsanksi.VendorId;
                sanksi.DecisionTypeCode = vwsanksi.DecisionTypeCode;
                sanksi.DecisionDescription = vwsanksi.DecisionDescription;
                sanksi.DecisionValidFrom = vwsanksi.DecisionValidFrom;
                sanksi.DecisionValidUntil = vwsanksi.DecisionValidUntil;
                sanksi.CreatedOn = DateTime.Now;
                sanksi.CreatedBy = UserId;
                ctx.Sanksis.Add(sanksi);
                ctx.SaveChanges();

                result.message = Common.SaveSukses();
                result.status = System.Net.HttpStatusCode.OK;
                //result.Id = tenderScoringHeader.PengadaanId.ToString();
                return result;
            }
            catch (Exception ex)
            {
                result.message = ex.ToString();
                result.status = System.Net.HttpStatusCode.InternalServerError;
                return result;
            }

        }


        public DataTableVWVendorwithSanksi GetListSanksi(string search, int start, int length)
        {

            search = search == null ? "" : search;
            DataTableVWVendorwithSanksi dtTable = new DataTableVWVendorwithSanksi();
            DataTableVWVendorwithSanksi dtTableAdd = new DataTableVWVendorwithSanksi();
            if (length > 0)
            {
                IQueryable<Sanksi> data = ctx.Sanksis.AsQueryable().OrderByDescending(x => x.CreatedOn);

                dtTable.recordsTotal = data.Count();
                dtTable.recordsFiltered = data.Count();
                data = data.OrderByDescending(d => d.CreatedOn).Skip(start).Take(length);

                dtTable.data = data.Select(d => new VWSanksi
                {
                    VendorId = d.VendorId,
                    NomorVendor = ctx.Vendors.Where(x => x.Id == d.VendorId).FirstOrDefault().NomorVendor,
                    NamaVendor = ctx.Vendors.Where(x => x.Id == d.VendorId).FirstOrDefault().Nama,
                    DecisionTypeCode = d.DecisionTypeCode,
                    DecisionDescription = d.DecisionDescription,
                    DecisionValidFrom = d.DecisionValidFrom,
                    DecisionValidUntil = d.DecisionValidUntil,
                }).ToList();

                List<VWSanksi> Ulu = new List<VWSanksi>();
                foreach (var item in dtTable.data)
                {
                    VWSanksi Ula = new VWSanksi();
                    Ula.VendorId = item.VendorId;
                    Ula.NomorVendor = item.NomorVendor;
                    Ula.NamaVendor = item.NamaVendor;
                    Ula.DecisionTypeCode = item.DecisionTypeCode;
                    Ula.DecisionDescription = item.DecisionDescription;
                    Ula.DecisionValidFrom = item.DecisionValidFrom;
                    Ula.DecisionValidUntil = item.DecisionValidUntil;
                    Ula.DalamMasaSanksi = IsDateBeforeOrToday(item.DecisionValidUntil) == true ? "masih" : "tidak";
                    var check = Ulu.Where(xx => xx.VendorId == item.VendorId).FirstOrDefault() == null ? "belum" : "ada";
                    if (check == "belum") Ulu.Add(Ula);
                    //Ulu.Add(Ula);
                }

                dtTableAdd.recordsTotal = dtTable.recordsTotal;
                dtTableAdd.recordsFiltered = dtTable.recordsFiltered;
                dtTableAdd.data = Ulu.Select(
                aa => new VWSanksi
                {
                    VendorId = aa.VendorId,
                    NamaVendor = aa.NamaVendor,
                    NomorVendor = aa.NomorVendor,
                    DecisionTypeCode = aa.DecisionTypeCode,
                    DecisionDescription = aa.DecisionDescription,
                    DecisionValidFrom = aa.DecisionValidFrom,
                    DecisionValidUntil = aa.DecisionValidUntil,
                    DalamMasaSanksi = aa.DalamMasaSanksi,
                }).ToList();

                dtTableAdd.data = dtTableAdd.data.Where(d => d.DalamMasaSanksi == "masih").ToList();

                if (!string.IsNullOrEmpty(search))
                {
                    dtTableAdd.data = dtTableAdd.data.Where(d => d.NamaVendor.Contains(search) || d.NomorVendor.Contains(search)).ToList();
                }
            }

            return dtTableAdd;
        }

        public DataTableVWVendorwithSanksi GetListRiwayatSanksi(int VendorId, string search, int start, int length)
        {

            search = search == null ? "" : search;
            DataTableVWVendorwithSanksi dtTable = new DataTableVWVendorwithSanksi();
            if (length > 0)
            {
                IQueryable<Sanksi> data = ctx.Sanksis.AsQueryable().Where(xx => xx.VendorId == VendorId);

                dtTable.recordsTotal = data.Count();
                dtTable.recordsFiltered = data.Count();
                data = data.OrderByDescending(d => d.CreatedOn).Skip(start).Take(length);

                dtTable.data = data.Select(d => new VWSanksi
                {
                    DecisionTypeCode = d.DecisionTypeCode,
                    DecisionValidFrom = d.DecisionValidFrom,
                    DecisionValidUntil = d.DecisionValidUntil,
                    CreatedBy = d.CreatedBy
                }).ToList();

                if (!string.IsNullOrEmpty(search))
                {
                    dtTable.data = dtTable.data.Where(d => d.NamaVendor.Contains(search) || d.NomorVendor.Contains(search)).ToList();
                }
            }

            return dtTable;
        }


        public DataTablePemenangPengadaanNilaiVendor GetListRiwayatPenilaian(int VendorId, string search, int start, int length)
        {

            search = search == null ? "" : search;
            DataTablePemenangPengadaanNilaiVendor dtTable = new DataTablePemenangPengadaanNilaiVendor();
            if (length > 0)
            {
                IQueryable<ApprisalWorksheet> data = ctx.ApprisalWorksheets.AsQueryable().Where(xx => xx.VendorId == VendorId);

                dtTable.recordsTotal = data.Count();
                dtTable.recordsFiltered = data.Count();
                data = data.OrderByDescending(d => d.CreatedOn).Skip(start).Take(length);

                dtTable.data = data.Select(d => new VWPemenangPengadaanNilaiVendor
                {
                    JudulPengadaan = ctx.PemenangPengadaans.Where(x => x.PengadaanId == d.PengadaanId).FirstOrDefault().Pengadaan.Judul,
                    NoPengadaan = ctx.Spk.Where(x => x.PemenangPengadaan.Pengadaan.Id == d.PengadaanId).FirstOrDefault().NoSPk,
                    //NoPengadaan = ctx.Spk.Where(x => x.PemenangPengadaanId == d.PengadaanId).FirstOrDefault().NoSPk,
                    created = d.CreatedOn,
                    CurrentTotal = d.CurrentTotal,
                    CurrentAverage = d.CurrentAverage
                }).ToList();

                //if (!string.IsNullOrEmpty(search))
                //{
                //    dtTable.data = dtTable.data.Where(d => d.NamaVendor.Contains(search) || d.NomorVendor.Contains(search)).ToList();
                //}
            }

            return dtTable;
        }

        public DataTableVWRekananPencarian GetrekananPencarian(string search, string status, string tipe, int start, int length)
        {

            search = search == null ? "" : search;
            status = status == null ? "" : status;
            tipe = tipe == null ? "" : tipe;

            DataTableVWRekananPencarian tp = new DataTableVWRekananPencarian();
            // record total yang tampil 
            tp.recordsTotal = (from b in ctx.Vendors
                               join c in ctx.Sanksis on b.Id equals c.VendorId into ps
                               from p in ps.DefaultIfEmpty()
                               select new VWSanksi
                               {
                                   VendorId = b.Id,
                                   NomorVendor = b.NomorVendor,
                                   DecisionTypeCode = p.DecisionTypeCode,
                                   DecisionDescription = p.DecisionDescription
                               }).Distinct().Count();

            tp.recordsFiltered = tp.recordsTotal;

            var ListVendorWithSanksi = (from b in ctx.Vendors
                                        join c in ctx.Sanksis on b.Id equals c.VendorId into ps
                                        //join c in q on b.Id equals c.VendorId into ps
                                        from p in ps.DefaultIfEmpty()
                                        select new VWSanksi
                                        {
                                            VendorId = b.Id,
                                            NamaVendor = b.Nama,
                                            NomorVendor = b.NomorVendor,
                                            DecisionTypeCode = String.IsNullOrEmpty(p.DecisionTypeCode) ? "SV01" : p.DecisionTypeCode,
                                            DecisionDescription = String.IsNullOrEmpty(p.DecisionDescription) ? "" : p.DecisionDescription,
                                            DecisionValidUntil = String.IsNullOrEmpty(p.DecisionValidUntil.ToString()) ? DateTime.MinValue : p.DecisionValidUntil,
                                            CreatedOn = String.IsNullOrEmpty(p.DecisionValidUntil.ToString()) ? DateTime.MinValue : p.CreatedOn,
                                        }).Distinct().OrderByDescending(xx => xx.CreatedOn).ToList();

            List<VWSanksi> Ulu = new List<VWSanksi>();
            foreach (var item in ListVendorWithSanksi)
            {
                VWSanksi Ula = new VWSanksi();
                Ula.VendorId = item.VendorId;
                Ula.NamaVendor = item.NamaVendor;
                Ula.NomorVendor = item.NomorVendor;
                Ula.DecisionTypeCode = IsDateBeforeOrToday(item.DecisionValidUntil) == false ? "SV01" : item.DecisionTypeCode;
                Ula.DecisionDescription = IsDateBeforeOrToday(item.DecisionValidUntil) == false ? "" : item.DecisionDescription;
                Ula.DalamMasaSanksi = IsDateBeforeOrToday(item.DecisionValidUntil) == true ? "masih" : "tidak";
                Ula.TipeVendor = ctx.VendorExts.Where(x => x.VendorId == item.VendorId).FirstOrDefault() == null ? "" : ctx.VendorExts.Where(x => x.VendorId == item.VendorId).FirstOrDefault().JenisVendor;
                var check = Ulu.Where(xx => xx.VendorId == Ula.VendorId).FirstOrDefault() == null ? "belum" : "ada";
                if (check == "belum") Ulu.Add(Ula);
            }

            tp.data = Ulu.Select(
                aa => new VWRekananPencarian
                {
                    IdVendor = aa.VendorId,
                    NamaVendor = aa.NamaVendor,
                    NomorVendor = aa.NomorVendor,
                    StatusSanksi = aa.DecisionTypeCode,
                    TipeVendor = aa.TipeVendor,
                }).ToList();

            //tp.data = (from a in Ulu
            //           join b in ctx.VendorExts on a.VendorId equals b.VendorId
            //          select new VWRekananPencarian
            //          {
            //              IdVendor = a.VendorId,
            //              NamaVendor = a.NamaVendor,
            //              NomorVendor = a.NomorVendor,
            //              StatusSanksi = a.DecisionTypeCode,
            //              TipeVendor = b.JenisVendor
            //          }).Distinct().ToList();

            if (!string.IsNullOrEmpty(search))
            {
                tp.data = tp.data.Where(d => (d.NamaVendor?.Contains(search) ?? false) || (d.NomorVendor?.Contains(search) ?? false)).ToList();
            }

            if (!string.IsNullOrEmpty(tipe))
            {
                tp.data = tp.data.Where(d => d.TipeVendor.Contains(tipe)).ToList();
            }

            if (!string.IsNullOrEmpty(status))
            {
                if (status == "SV01")
                {
                    tp.data = tp.data.Where(d => d.StatusSanksi != "SV02" && d.StatusSanksi != "SV03" && d.StatusSanksi != "SV04").ToList();
                }
                else
                {
                    tp.data = tp.data.Where(d => d.StatusSanksi.Contains(status)).ToList();
                }
            }

            return tp;
        }

        public static bool IsDateBeforeOrToday(DateTime input)
        {
            var todaysDate = DateTime.Today;

            if (input < todaysDate)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        public RegDocumentImageExt GetDokumen(Guid Iddok)
        {

            DocumentImageExt dokumen1 = ctx.DocumentImageExts.Where(x => x.Id == Iddok).FirstOrDefault();
            RegDocumentImageExt dokumen2 = ctx.RegDocumentImageExts.Where(x => x.Id == Iddok).FirstOrDefault();

            RegDocumentImageExt sss = new RegDocumentImageExt();
            if (dokumen1 != null)
            {
                sss.Id = dokumen1.Id;
                sss.Content = dokumen1.Content;
                sss.FileName = dokumen1.FileName;
                sss.ContentType = dokumen1.ContentType;
                sss.RegDocumenExtId = dokumen1.DocumenExtId;
            }
            else
            {
                sss.Id = dokumen2.Id;
                sss.Content = dokumen2.Content;
                sss.FileName = dokumen2.FileName;
                sss.ContentType = dokumen2.ContentType;
                sss.RegDocumenExtId = dokumen2.RegDocumenExtId;
            }
            //RegDocumentImageExt dokumen = sss.FileName == null ? dokumen2 : sss;
            RegDocumentImageExt dokumen = sss;
            return dokumen;

        }

        public VendorExtViewModelJaws GetVendor(string noPengajuan)
        {
            try
            {
                int IdRegVendor = ctx.RegVendors.Where(x => x.NoPengajuan == noPengajuan).FirstOrDefault().Id;

                RegVendorExt vendorExt = ctx.RegVendorExts.Where(x => x.RegVendorId == IdRegVendor).FirstOrDefault();
                RegVendor rv = ctx.RegVendors.Where(x => x.NoPengajuan == noPengajuan).FirstOrDefault();

                if (vendorExt != null)
                {
                    Guid vendorExtId = vendorExt.Id;

                    List<VendorHumanResourceExtViewModels> hrExt = (from a in ctx.RegVendorExtHumanResources
                                                                    where a.RegVendorExtId == vendorExt.Id
                                                                    select new VendorHumanResourceExtViewModels
                                                                    {
                                                                        ResourceFullName = a.ResourceFullName,
                                                                        ResourceDateOfBirth = a.ResourceDateOfBirth,
                                                                        ResourceLastEduCode = a.ResourceLastEduCode,
                                                                        ResourceExperienceCode = a.ResourceExperienceCode,
                                                                        ResourceExpertise = a.ResourceExpertise
                                                                    }).Distinct().ToList();


                    List<VendorEquipmentExtViewModels> VEExt = (from a in ctx.RegVendorExtEquipments
                                                                where a.RegVendorExtId == vendorExt.Id
                                                                select new VendorEquipmentExtViewModels
                                                                {
                                                                    EquipmentName = a.EquipmentName,
                                                                    EquipmentQty = a.EquipmentQty,
                                                                    EquipmentCapacity = a.EquipmentCapacity,
                                                                    EquipmentMake = a.EquipmentMake,
                                                                    EquipmentMakeYear = a.EquipmentMakeYear,
                                                                    EquipmentConditionCode = a.EquipmentConditionCode,
                                                                    EquipmentLocation = a.EquipmentLocation
                                                                }).Distinct().ToList();

                    List<VendorJobHistoryExtViewModels> VJHExt = (from a in ctx.RegVendorExtJobHistories
                                                                  where a.RegVendorExtId == vendorExt.Id
                                                                  select new VendorJobHistoryExtViewModels
                                                                  {
                                                                      JobTitle = a.JobTitle,
                                                                      JobLocation = a.JobLocation,
                                                                      JobClient = a.JobClient,
                                                                      JobType = a.JobType,
                                                                      JobStartDate = a.JobStartDate,
                                                                      JobContractNum = a.JobContractNum,
                                                                      JobContractDate = a.JobContractDate,
                                                                      JobContractAmount = a.JobContractAmount,
                                                                      JobContractAmountCurrencyCodeCode = String.IsNullOrEmpty(a.JobContractAmountCurrencyCode) ? "" : ctx.ReferenceDatas.Where(x => x.Code == a.JobContractAmountCurrencyCode).FirstOrDefault().LocalizedName,
                                                                      JobContractAmountCurrencyCode = a.JobContractAmountCurrencyCode
                                                                  }).Distinct().ToList();

                    VendorFinStatementExtViewModels VFSExt = (from a in ctx.RegVendorExtFinStatements
                                                              where a.RegVendorExtId == vendorExt.Id
                                                              select new VendorFinStatementExtViewModels
                                                              {
                                                                  FinStmtYear = a.FinStmtYear,
                                                                  FinStmtCurrencyCode = String.IsNullOrEmpty(a.FinStmtCurrencyCode) ? "" : ctx.ReferenceDatas.Where(x => x.Code == a.FinStmtCurrencyCode).FirstOrDefault().LocalizedName,
                                                                  FinStmtCurrencyCodeCode = a.FinStmtCurrencyCode,
                                                                  FinStmtAktivaLancar = a.FinStmtAktivaLancar,
                                                                  FinStmtHutangLancar = a.FinStmtHutangLancar,
                                                                  FinStmtRasioLikuiditas = a.FinStmtRasioLikuiditas,
                                                                  FinStmtTotalHutang = a.FinStmtTotalHutang,
                                                                  FinStmtEkuitas = a.FinStmtEkuitas,
                                                                  FinStmtDebtToEquityRatio = a.FinStmtDebtToEquityRation,
                                                                  FinStmtNetProfitLoss = a.FinStmtNetProfitLoss,
                                                                  FinStmtReturnOfEquity = a.FinStmtReturnOfEquity,
                                                                  FinStmtKas = a.FinStmtKas,
                                                                  FinStmtTotalAktiva = a.FinStmtTotalAktiva,
                                                                  FinStmtAuditStatusCode = a.FinStmtAuditStatusCode
                                                              }).FirstOrDefault();

                    VendorExtViewModelJaws vm = new VendorExtViewModelJaws
                    {
                        id = vendorExt.RegVendorId,
                        TipeVendor = Int16.Parse(vendorExt.JenisVendor),
                        Provinsi = String.IsNullOrEmpty(rv.Provinsi) ? "" : ctx.ReferenceDatas.Where(x => x.Code == rv.Provinsi && x.Qualifier == "DUKCAPILPROV").FirstOrDefault().LocalizedName,
                        FirstLevelDivisionCode = String.IsNullOrEmpty(vendorExt.FirstLevelDivisionCode) ? "" : ctx.ReferenceDatas.Where(x => x.Code == vendorExt.FirstLevelDivisionCode && x.Qualifier == "DUKCAPILKOTA").FirstOrDefault().LocalizedName,
                        SecondLevelDivisionCode = String.IsNullOrEmpty(vendorExt.SecondLevelDivisionCode) ? "" : ctx.ReferenceDatas.Where(x => x.Code == vendorExt.SecondLevelDivisionCode && x.Qualifier == "DUKCAPILKECAMATAN").FirstOrDefault().LocalizedName,
                        ThirdLevelDivisionCode = String.IsNullOrEmpty(vendorExt.ThirdLevelDivisionCode) ? "" : ctx.ReferenceDatas.Where(x => x.Code == vendorExt.ThirdLevelDivisionCode && x.Qualifier == "DUKCAPILKELURAHAN").FirstOrDefault().LocalizedName,
                        Nama = rv.Nama,
                        Alamat = rv.Alamat,
                        Email = rv.Email,
                        Website = rv.Website,
                        Telepon = rv.Telepon,
                    };


                    VendorRegExtViewModels VendorRegExt = new VendorRegExtViewModels
                    {
                        KategoriUsaha = vendorExt.KategoriUsaha,
                        KategoriVendor = vendorExt.KategoriVendor,
                        BentukBadanUsaha = vendorExt.BentukBadanUsaha,
                        StatusPerusahaan = vendorExt.StatusPerusahaan,
                        EstablishedDate = vendorExt.EstablishedDate == null ? DateTime.MinValue : vendorExt.EstablishedDate.Value,
                        CountryCode = ctx.ReferenceDatas.Where(x => x.Code == vendorExt.CountryCode).FirstOrDefault().LocalizedName,
                        PostalCode = vendorExt.PostalCode,
                        Fax = vendorExt.Fax,
                        WorkUnitCode = vendorExt.WorkUnitCode,

                        SegBidangUsahaCode = vendorExt.SegBidangUsahaCode,
                        //changes multiple
                        SegKelompokUsahaCodeSingle = vendorExt.SegKelompokUsahaCode,

                        SegBidangUsahaCodes = ctx.ReferenceDatas.Where(x => x.Code == vendorExt.SegBidangUsahaCode && x.Qualifier == "SegBidangUsahaCode").FirstOrDefault().LocalizedName,
                        //changes multiple
                        //SegSubBidangUsahaCode = ctx.ReferenceDatas.Where(x => x.Code == vendorExt.SegSubBidangUsahaCode && x.Qualifier == "SegSubBidangUsahaCode").FirstOrDefault().LocalizedName,
                        //SegSubBidangUsahaCode = vendorExt.SegSubBidangUsahaCode,
                        SegKualifikasiGrade = ctx.ReferenceDatas.Where(x => x.Code == vendorExt.SegKualifikasiGrade && x.Qualifier == "SegKualifikasiGradeCode").FirstOrDefault().LocalizedName,

                        IndivName = vendorExt.IndivName,
                        IndivAbbrevName = vendorExt.IndivAbbrevName,
                        IndivGiidNo = vendorExt.IndivGiidNo,
                        IndivGiidValidUntil = vendorExt.IndivGiidValidUntil == null ? DateTime.MinValue : vendorExt.IndivGiidValidUntil.Value,
                        IndivAddress = vendorExt.IndivAddress,
                        IndivCountryCode = vendorExt.IndivCountryCode,
                        IndivFirstLevelDivisionCode = vendorExt.IndivFirstLevelDivisionCode,
                        IndivSecondLevelDivisionCode = vendorExt.IndivSecondLevelDivisionCode,
                        IndivThirdLevelDivisionCode = vendorExt.IndivThirdLevelDivisionCode,
                        IndivPostalCode = vendorExt.IndivPostalCode,
                        IndivContactPersonName = vendorExt.IndivContactPersonName,
                        IndivContactPhoneNum = vendorExt.IndivContactPhoneNum,
                        IndivContactEmail = vendorExt.IndivContactEmail,
                        PrinRepOfficeAddress = vendorExt.PrinRepOfficeAddress,
                        PrinRepOfficeContactPhoneNum = vendorExt.PrinRepOfficeContactPhoneNum,
                        PrinRepOfficeFaxNum = vendorExt.PrinRepOfficeFaxNum,
                        PrinRepOfficeEmail = vendorExt.PrinRepOfficeEmail,
                        PrinWebsite = vendorExt.PrinWebsite,
                        SubDistrict = vendorExt.SubDistrict,
                        Village = vendorExt.Village,
                        PrinRepPosition = vendorExt.PrinRepPosition,
                        IndivGiidDocId = vendorExt.IndivGiidDocId,
                        PrinRepOfficeLocalAddress = vendorExt.PrinRepOfficeLocalAddress,
                        CPName = vendorExt.CPName,
                        IsPKP = vendorExt.IsPKP,

                        DirPersonName = vendorExt.DirPersonName,
                        DirPersonGiidNo = vendorExt.DirPersonGiidNo,
                        DirPersonPosition = vendorExt.DirPersonPosition,
                        DirPersonReligionCode = vendorExt.DirPersonReligionCode,
                        DirPersonBirthDay = vendorExt.DirPersonBirthDay
                    };




                    //VendorBankInfoExtViewModels bankExt = new VendorBankInfoExtViewModels
                    VendorBankInfoExtViewModels bankExt = (from a in ctx.RegVendorExtBankInfoes
                                                           where a.RegVendorExtId == vendorExtId
                                                           select new VendorBankInfoExtViewModels
                                                           {
                                                               BankCode = ctx.ReferenceDatas.Where(x => x.Code == a.BankCode).FirstOrDefault().LocalizedName,
                                                               BankCodeCode = a.BankCode,
                                                               BankAddress = a.BankAddress,
                                                               BankCity = a.BankCity,
                                                               Branch = a.Branch,
                                                               AccNumber = a.AccNumber,
                                                               AccName = a.AccName,
                                                               AccCurrencyCode = a.AccCurrencyCode,
                                                               BankCountry = ctx.ReferenceDatas.Where(x => x.Code == vendorExt.CountryCode).FirstOrDefault().LocalizedName,
                                                               BankCountryCode = vendorExt.CountryCode
                                                           }).Distinct().FirstOrDefault();

                    List<VendorPersonExtViewModels> PersonExt = (from a in ctx.RegVendorExtPersons
                                                                 where a.RegVendorExtId == vendorExtId
                                                                 select new VendorPersonExtViewModels
                                                                 {
                                                                     Name = a.Name,
                                                                     Position = a.Position,
                                                                     ReligionCode = a.ReligionCode,
                                                                     GiidNo = a.GiidNo,
                                                                     BirthDay = a.BirthDay == null ? DateTime.MinValue : a.BirthDay.Value,
                                                                     ContactAddress = a.ContactAddress,
                                                                     ContactEmail = a.ContactEmail,
                                                                     ContactPhone = a.ContactPhone
                                                                 }).Distinct().ToList();

                    VendorDokumenExts NPWPdocs = (from a in ctx.RegDocumentExts
                                                  join b in ctx.RegDocumentImageExts on a.Id equals b.RegDocumenExtId
                                                  where a.RegVendorExtId == vendorExtId && a.TipeDokumen == 0
                                                  select new VendorDokumenExts
                                                  {
                                                      Iddok = b.Id,
                                                      Nomor = a.Nomor,
                                                      Pembuat = a.Penerbit,
                                                      TipeDokumen = a.TipeDokumen.ToString(),
                                                      TanggalTerbit = a.TanggalTerbit,
                                                      TanggalBerakhir = a.TanggalBerakhir,
                                                      //base64 = b.Content.ToString(),
                                                      FileName = b.FileName,
                                                      ContentType = b.ContentType
                                                  }).Distinct().FirstOrDefault();

                    VendorDokumenExts PKPdocs = (from a in ctx.RegDocumentExts
                                                 join b in ctx.RegDocumentImageExts on a.Id equals b.RegDocumenExtId
                                                 where a.RegVendorExtId == vendorExtId && a.TipeDokumen == 1
                                                 select new VendorDokumenExts
                                                 {
                                                     Iddok = b.Id,
                                                     Nomor = a.Nomor,
                                                     Pembuat = a.Penerbit,
                                                     TipeDokumen = a.TipeDokumen.ToString(),
                                                     TanggalTerbit = a.TanggalTerbit,
                                                     TanggalBerakhir = a.TanggalBerakhir,
                                                     //base64 = b.Content.ToString(),
                                                     FileName = b.FileName,
                                                     ContentType = b.ContentType
                                                 }).Distinct().FirstOrDefault();

                    VendorDokumenExts TDPdocs = (from a in ctx.RegDocumentExts
                                                 join b in ctx.RegDocumentImageExts on a.Id equals b.RegDocumenExtId
                                                 where a.RegVendorExtId == vendorExtId && a.TipeDokumen == 2
                                                 select new VendorDokumenExts
                                                 {
                                                     Iddok = b.Id,
                                                     Nomor = a.Nomor,
                                                     Pembuat = a.Penerbit,
                                                     TipeDokumen = a.TipeDokumen.ToString(),
                                                     TanggalTerbit = a.TanggalTerbit,
                                                     TanggalBerakhir = a.TanggalBerakhir,
                                                     //base64 = b.Content.ToString(),
                                                     FileName = b.FileName,
                                                     ContentType = b.ContentType
                                                 }).Distinct().FirstOrDefault();

                    VendorDokumenExts SIUPdocs = (from a in ctx.RegDocumentExts
                                                  join b in ctx.RegDocumentImageExts on a.Id equals b.RegDocumenExtId
                                                  where a.RegVendorExtId == vendorExtId && a.TipeDokumen == 3
                                                  select new VendorDokumenExts
                                                  {
                                                      Iddok = b.Id,
                                                      Nomor = a.Nomor,
                                                      Pembuat = a.Penerbit,
                                                      TipeDokumen = a.TipeDokumen.ToString(),
                                                      TanggalTerbit = a.TanggalTerbit,
                                                      TanggalBerakhir = a.TanggalBerakhir,
                                                      //base64 = b.Content.ToString(),
                                                      FileName = b.FileName,
                                                      ContentType = b.ContentType
                                                  }).Distinct().FirstOrDefault();

                    VendorDokumenExts SIUJKdocs = (from a in ctx.RegDocumentExts
                                                   join b in ctx.RegDocumentImageExts on a.Id equals b.RegDocumenExtId
                                                   where a.RegVendorExtId == vendorExtId && a.TipeDokumen == 4
                                                   select new VendorDokumenExts
                                                   {
                                                       Iddok = b.Id,
                                                       Nomor = a.Nomor,
                                                       Pembuat = a.Penerbit,
                                                       TipeDokumen = a.TipeDokumen.ToString(),
                                                       TanggalTerbit = a.TanggalTerbit,
                                                       TanggalBerakhir = a.TanggalBerakhir,
                                                       //base64 = b.Content.ToString(),
                                                       FileName = b.FileName,
                                                       ContentType = b.ContentType
                                                   }).Distinct().FirstOrDefault();

                    VendorDokumenExts AKTAdocs = (from a in ctx.RegDocumentExts
                                                  join b in ctx.RegDocumentImageExts on a.Id equals b.RegDocumenExtId
                                                  where a.RegVendorExtId == vendorExtId && a.TipeDokumen == 5
                                                  select new VendorDokumenExts
                                                  {
                                                      Iddok = b.Id,
                                                      Nomor = a.Nomor,
                                                      Pembuat = a.Penerbit,
                                                      TipeDokumen = a.TipeDokumen.ToString(),
                                                      TanggalTerbit = a.TanggalTerbit,
                                                      TanggalBerakhir = a.TanggalBerakhir,
                                                      //base64 = b.Content.ToString(),
                                                      FileName = b.FileName,
                                                      ContentType = b.ContentType
                                                  }).Distinct().FirstOrDefault();

                    VendorDokumenExts PENGADAANdocs = (from a in ctx.RegDocumentExts
                                                       join b in ctx.RegDocumentImageExts on a.Id equals b.RegDocumenExtId
                                                       where a.RegVendorExtId == vendorExtId && a.TipeDokumen == 6
                                                       select new VendorDokumenExts
                                                       {
                                                           Iddok = b.Id,
                                                           Nomor = a.Nomor,
                                                           Pembuat = a.Penerbit,
                                                           TipeDokumen = a.TipeDokumen.ToString(),
                                                           TanggalTerbit = a.TanggalTerbit,
                                                           TanggalBerakhir = a.TanggalBerakhir,
                                                           //base64 = b.Content.ToString(),
                                                           FileName = b.FileName,
                                                           ContentType = b.ContentType
                                                       }).Distinct().FirstOrDefault();

                    VendorDokumenExts KTPdocs = (from a in ctx.RegDocumentExts
                                                 join b in ctx.RegDocumentImageExts on a.Id equals b.RegDocumenExtId
                                                 where a.RegVendorExtId == vendorExtId && a.TipeDokumen == 7
                                                 select new VendorDokumenExts
                                                 {
                                                     Iddok = b.Id,
                                                     Nomor = a.Nomor,
                                                     Pembuat = a.Penerbit,
                                                     TipeDokumen = a.TipeDokumen.ToString(),
                                                     TanggalTerbit = a.TanggalTerbit,
                                                     TanggalBerakhir = a.TanggalBerakhir,
                                                     //base64 = b.Content.ToString(),
                                                     FileName = b.FileName,
                                                     ContentType = b.ContentType
                                                 }).Distinct().FirstOrDefault();

                    VendorDokumenExts SERTIFIKATdocs = (from a in ctx.RegDocumentExts
                                                        join b in ctx.RegDocumentImageExts on a.Id equals b.RegDocumenExtId
                                                        where a.RegVendorExtId == vendorExtId && a.TipeDokumen == 8
                                                        select new VendorDokumenExts
                                                        {
                                                            Iddok = b.Id,
                                                            Nomor = a.Nomor,
                                                            Pembuat = a.Penerbit,
                                                            TipeDokumen = a.TipeDokumen.ToString(),
                                                            TanggalTerbit = a.TanggalTerbit,
                                                            TanggalBerakhir = a.TanggalBerakhir,
                                                            //base64 = b.Content.ToString(),
                                                            FileName = b.FileName,
                                                            ContentType = b.ContentType
                                                        }).Distinct().FirstOrDefault();

                    VendorDokumenExts NPWPPemilikdocs = (from a in ctx.RegDocumentExts
                                                         join b in ctx.RegDocumentImageExts on a.Id equals b.RegDocumenExtId
                                                         where a.RegVendorExtId == vendorExtId && a.TipeDokumen == 9
                                                         select new VendorDokumenExts
                                                         {
                                                             Iddok = b.Id,
                                                             Nomor = a.Nomor,
                                                             Pembuat = a.Penerbit,
                                                             TipeDokumen = a.TipeDokumen.ToString(),
                                                             TanggalTerbit = a.TanggalTerbit,
                                                             TanggalBerakhir = a.TanggalBerakhir,
                                                             //base64 = b.Content.ToString(),
                                                             FileName = b.FileName,
                                                             ContentType = b.ContentType
                                                         }).Distinct().FirstOrDefault();

                    VendorDokumenExts KTPPemilikdocs = (from a in ctx.RegDocumentExts
                                                        join b in ctx.RegDocumentImageExts on a.Id equals b.RegDocumenExtId
                                                        where a.RegVendorExtId == vendorExtId && a.TipeDokumen == 10
                                                        select new VendorDokumenExts
                                                        {
                                                            Iddok = b.Id,
                                                            Nomor = a.Nomor,
                                                            Pembuat = a.Penerbit,
                                                            TipeDokumen = a.TipeDokumen.ToString(),
                                                            TanggalTerbit = a.TanggalTerbit,
                                                            TanggalBerakhir = a.TanggalBerakhir,
                                                            //base64 = b.Content.ToString(),
                                                            FileName = b.FileName,
                                                            ContentType = b.ContentType
                                                        }).Distinct().FirstOrDefault();

                    VendorDokumenExts DOMISILIdocs = (from a in ctx.RegDocumentExts
                                                      join b in ctx.RegDocumentImageExts on a.Id equals b.RegDocumenExtId
                                                      where a.RegVendorExtId == vendorExtId && a.TipeDokumen == 11
                                                      select new VendorDokumenExts
                                                      {
                                                          Iddok = b.Id,
                                                          Nomor = a.Nomor,
                                                          Pembuat = a.Penerbit,
                                                          TipeDokumen = a.TipeDokumen.ToString(),
                                                          TanggalTerbit = a.TanggalTerbit,
                                                          TanggalBerakhir = a.TanggalBerakhir,
                                                          //base64 = b.Content.ToString(),
                                                          FileName = b.FileName,
                                                          ContentType = b.ContentType
                                                      }).Distinct().FirstOrDefault();

                    VendorDokumenExts LAPORANKEUANGANdocs = (from a in ctx.RegDocumentExts
                                                             join b in ctx.RegDocumentImageExts on a.Id equals b.RegDocumenExtId
                                                             where a.RegVendorExtId == vendorExtId && a.TipeDokumen == 12
                                                             select new VendorDokumenExts
                                                             {
                                                                 Iddok = b.Id,
                                                                 Nomor = a.Nomor,
                                                                 Pembuat = a.Penerbit,
                                                                 TipeDokumen = a.TipeDokumen.ToString(),
                                                                 TanggalTerbit = a.TanggalTerbit,
                                                                 TanggalBerakhir = a.TanggalBerakhir,
                                                                 //base64 = b.Content.ToString(),
                                                                 FileName = b.FileName,
                                                                 ContentType = b.ContentType
                                                             }).Distinct().FirstOrDefault();

                    VendorDokumenExts REKENINGKORANdocs = (from a in ctx.RegDocumentExts
                                                           join b in ctx.RegDocumentImageExts on a.Id equals b.RegDocumenExtId
                                                           where a.RegVendorExtId == vendorExtId && a.TipeDokumen == 13
                                                           select new VendorDokumenExts
                                                           {
                                                               Iddok = b.Id,
                                                               Nomor = a.Nomor,
                                                               Pembuat = a.Penerbit,
                                                               TipeDokumen = a.TipeDokumen.ToString(),
                                                               TanggalTerbit = a.TanggalTerbit,
                                                               TanggalBerakhir = a.TanggalBerakhir,
                                                               //base64 = b.Content.ToString(),
                                                               FileName = b.FileName,
                                                               ContentType = b.ContentType
                                                           }).Distinct().FirstOrDefault();

                    VendorDokumenExts DRTdocs = (from a in ctx.RegDocumentExts
                                                 join b in ctx.RegDocumentImageExts on a.Id equals b.RegDocumenExtId
                                                 where a.RegVendorExtId == vendorExtId && a.TipeDokumen == 14
                                                 select new VendorDokumenExts
                                                 {
                                                     Iddok = b.Id,
                                                     Nomor = a.Nomor,
                                                     Pembuat = a.Penerbit,
                                                     TipeDokumen = a.TipeDokumen.ToString(),
                                                     TanggalTerbit = a.TanggalTerbit,
                                                     TanggalBerakhir = a.TanggalBerakhir,
                                                     //base64 = b.Content.ToString(),
                                                     FileName = b.FileName,
                                                     ContentType = b.ContentType
                                                 }).Distinct().FirstOrDefault();

                    VendorDokumenExts AKTAPENDIRIANdocs = (from a in ctx.RegDocumentExts
                                                           join b in ctx.RegDocumentImageExts on a.Id equals b.RegDocumenExtId
                                                           where a.RegVendorExtId == vendorExtId && a.TipeDokumen == 15
                                                           select new VendorDokumenExts
                                                           {
                                                               Iddok = b.Id,
                                                               Nomor = a.Nomor,
                                                               Pembuat = a.Penerbit,
                                                               TipeDokumen = a.TipeDokumen.ToString(),
                                                               TanggalTerbit = a.TanggalTerbit,
                                                               TanggalBerakhir = a.TanggalBerakhir,
                                                               //base64 = b.Content.ToString(),
                                                               FileName = b.FileName,
                                                               ContentType = b.ContentType
                                                           }).Distinct().FirstOrDefault();

                    VendorDokumenExts SKKEMENKUMHAMdocs = (from a in ctx.RegDocumentExts
                                                           join b in ctx.RegDocumentImageExts on a.Id equals b.RegDocumenExtId
                                                           where a.RegVendorExtId == vendorExtId && a.TipeDokumen == 16
                                                           select new VendorDokumenExts
                                                           {
                                                               Iddok = b.Id,
                                                               Nomor = a.Nomor,
                                                               Pembuat = a.Penerbit,
                                                               TipeDokumen = a.TipeDokumen.ToString(),
                                                               TanggalTerbit = a.TanggalTerbit,
                                                               TanggalBerakhir = a.TanggalBerakhir,
                                                               //base64 = b.Content.ToString(),
                                                               FileName = b.FileName,
                                                               ContentType = b.ContentType
                                                           }).Distinct().FirstOrDefault();

                    VendorDokumenExts BERITANEGARAdocs = (from a in ctx.RegDocumentExts
                                                          join b in ctx.RegDocumentImageExts on a.Id equals b.RegDocumenExtId
                                                          where a.RegVendorExtId == vendorExtId && a.TipeDokumen == 17
                                                          select new VendorDokumenExts
                                                          {
                                                              Iddok = b.Id,
                                                              Nomor = a.Nomor,
                                                              Pembuat = a.Penerbit,
                                                              TipeDokumen = a.TipeDokumen.ToString(),
                                                              TanggalTerbit = a.TanggalTerbit,
                                                              TanggalBerakhir = a.TanggalBerakhir,
                                                              //base64 = b.Content.ToString(),
                                                              FileName = b.FileName,
                                                              ContentType = b.ContentType
                                                          }).Distinct().FirstOrDefault();

                    VendorDokumenExts AKTAPERUBAHANdocs = (from a in ctx.RegDocumentExts
                                                           join b in ctx.RegDocumentImageExts on a.Id equals b.RegDocumenExtId
                                                           where a.RegVendorExtId == vendorExtId && a.TipeDokumen == 18
                                                           select new VendorDokumenExts
                                                           {
                                                               Iddok = b.Id,
                                                               Nomor = a.Nomor,
                                                               Pembuat = a.Penerbit,
                                                               TipeDokumen = a.TipeDokumen.ToString(),
                                                               TanggalTerbit = a.TanggalTerbit,
                                                               TanggalBerakhir = a.TanggalBerakhir,
                                                               //base64 = b.Content.ToString(),
                                                               FileName = b.FileName,
                                                               ContentType = b.ContentType
                                                           }).Distinct().FirstOrDefault();

                    VendorDokumenExts PROFILPERUSAHAANdocs = (from a in ctx.RegDocumentExts
                                                              join b in ctx.RegDocumentImageExts on a.Id equals b.RegDocumenExtId
                                                              where a.RegVendorExtId == vendorExtId && a.TipeDokumen == 19
                                                              select new VendorDokumenExts
                                                              {
                                                                  Iddok = b.Id,
                                                                  Nomor = a.Nomor,
                                                                  Pembuat = a.Penerbit,
                                                                  TipeDokumen = a.TipeDokumen.ToString(),
                                                                  TanggalTerbit = a.TanggalTerbit,
                                                                  TanggalBerakhir = a.TanggalBerakhir,
                                                                  //base64 = b.Content.ToString(),
                                                                  FileName = b.FileName,
                                                                  ContentType = b.ContentType
                                                              }).Distinct().FirstOrDefault();

                    VendorDokumenExts NIBdocs = (from a in ctx.RegDocumentExts
                                                 join b in ctx.RegDocumentImageExts on a.Id equals b.RegDocumenExtId
                                                 where a.RegVendorExtId == vendorExtId && a.TipeDokumen == 20
                                                 select new VendorDokumenExts
                                                 {
                                                     Iddok = b.Id,
                                                     Nomor = a.Nomor,
                                                     Pembuat = a.Penerbit,
                                                     TipeDokumen = a.TipeDokumen.ToString(),
                                                     TanggalTerbit = a.TanggalTerbit,
                                                     TanggalBerakhir = a.TanggalBerakhir,
                                                     //base64 = b.Content.ToString(),
                                                     FileName = b.FileName,
                                                     ContentType = b.ContentType
                                                 }).Distinct().FirstOrDefault();

                    VendorDokumenExts DokumenSertifikatCVdocs = (from a in ctx.RegDocumentExts
                                                                 join b in ctx.RegDocumentImageExts on a.Id equals b.RegDocumenExtId
                                                                 where a.RegVendorExtId == vendorExtId && a.TipeDokumen == 21
                                                                 select new VendorDokumenExts
                                                                 {
                                                                     Iddok = b.Id,
                                                                     Nomor = a.Nomor,
                                                                     Pembuat = a.Penerbit,
                                                                     TipeDokumen = a.TipeDokumen.ToString(),
                                                                     TanggalTerbit = a.TanggalTerbit,
                                                                     TanggalBerakhir = a.TanggalBerakhir,
                                                                     //base64 = b.Content.ToString(),
                                                                     FileName = b.FileName,
                                                                     ContentType = b.ContentType
                                                                 }).Distinct().FirstOrDefault();

                    VendorDokumenExts BuktiKepemilikanPeralatandocs = (from a in ctx.RegDocumentExts
                                                                       join b in ctx.RegDocumentImageExts on a.Id equals b.RegDocumenExtId
                                                                       where a.RegVendorExtId == vendorExtId && a.TipeDokumen == 22
                                                                       select new VendorDokumenExts
                                                                       {
                                                                           Iddok = b.Id,
                                                                           Nomor = a.Nomor,
                                                                           Pembuat = a.Penerbit,
                                                                           TipeDokumen = a.TipeDokumen.ToString(),
                                                                           TanggalTerbit = a.TanggalTerbit,
                                                                           TanggalBerakhir = a.TanggalBerakhir,
                                                                           //base64 = b.Content.ToString(),
                                                                           FileName = b.FileName,
                                                                           ContentType = b.ContentType
                                                                       }).Distinct().FirstOrDefault();

                    VendorDokumenExts FotoPeralatandocs = (from a in ctx.RegDocumentExts
                                                           join b in ctx.RegDocumentImageExts on a.Id equals b.RegDocumenExtId
                                                           where a.RegVendorExtId == vendorExtId && a.TipeDokumen == 23
                                                           select new VendorDokumenExts
                                                           {
                                                               Iddok = b.Id,
                                                               Nomor = a.Nomor,
                                                               Pembuat = a.Penerbit,
                                                               TipeDokumen = a.TipeDokumen.ToString(),
                                                               TanggalTerbit = a.TanggalTerbit,
                                                               TanggalBerakhir = a.TanggalBerakhir,
                                                               //base64 = b.Content.ToString(),
                                                               FileName = b.FileName,
                                                               ContentType = b.ContentType
                                                           }).Distinct().FirstOrDefault();

                    VendorDokumenExts BuktiKerjasamadocs = (from a in ctx.RegDocumentExts
                                                            join b in ctx.RegDocumentImageExts on a.Id equals b.RegDocumenExtId
                                                            where a.RegVendorExtId == vendorExtId && a.TipeDokumen == 24
                                                            select new VendorDokumenExts
                                                            {
                                                                Iddok = b.Id,
                                                                Nomor = a.Nomor,
                                                                Pembuat = a.Penerbit,
                                                                TipeDokumen = a.TipeDokumen.ToString(),
                                                                TanggalTerbit = a.TanggalTerbit,
                                                                TanggalBerakhir = a.TanggalBerakhir,
                                                                //base64 = b.Content.ToString(),
                                                                FileName = b.FileName,
                                                                ContentType = b.ContentType
                                                            }).Distinct().FirstOrDefault();

                    VendorDokumenExts LaporanDataKeuangandocs = (from a in ctx.RegDocumentExts
                                                                 join b in ctx.RegDocumentImageExts on a.Id equals b.RegDocumenExtId
                                                                 where a.RegVendorExtId == vendorExtId && a.TipeDokumen == 25
                                                                 select new VendorDokumenExts
                                                                 {
                                                                     Iddok = b.Id,
                                                                     Nomor = a.Nomor,
                                                                     Pembuat = a.Penerbit,
                                                                     TipeDokumen = a.TipeDokumen.ToString(),
                                                                     TanggalTerbit = a.TanggalTerbit,
                                                                     TanggalBerakhir = a.TanggalBerakhir,
                                                                     //base64 = b.Content.ToString(),
                                                                     FileName = b.FileName,
                                                                     ContentType = b.ContentType
                                                                 }).Distinct().FirstOrDefault();
                    VendorDokumenExts CVTenagaAhlidocs = (from a in ctx.RegDocumentExts
                                                          join b in ctx.RegDocumentImageExts on a.Id equals b.RegDocumenExtId
                                                          where a.RegVendorExtId == vendorExtId && a.TipeDokumen == 30
                                                          select new VendorDokumenExts
                                                          {
                                                              Iddok = b.Id,
                                                              Nomor = a.Nomor,
                                                              Pembuat = a.Penerbit,
                                                              TipeDokumen = a.TipeDokumen.ToString(),
                                                              TanggalTerbit = a.TanggalTerbit,
                                                              TanggalBerakhir = a.TanggalBerakhir,
                                                              //base64 = b.Content.ToString(),
                                                              FileName = b.FileName,
                                                              ContentType = b.ContentType
                                                          }).Distinct().FirstOrDefault();


                    vm.VendorRegExt = VendorRegExt;
                    vm.VendorBankInfoExt = bankExt;
                    vm.VendorPersonExt = PersonExt;
                    vm.VendorHumanResourceExt = hrExt;
                    vm.VendorEquipmentExt = VEExt;
                    vm.VendorJobHistoryExt = VJHExt;
                    vm.VendorFinStatementExt = VFSExt;
                    vm.NPWP = NPWPdocs;
                    vm.PKP = PKPdocs;
                    vm.TDP = TDPdocs;
                    vm.SIUP = SIUPdocs;
                    vm.SIUJK = SIUJKdocs;
                    vm.AKTA = AKTAdocs;
                    vm.PENGADAAN = PENGADAANdocs;
                    vm.KTP = KTPdocs;
                    vm.SERTIFIKAT = SERTIFIKATdocs;
                    vm.NPWPPemilik = NPWPPemilikdocs;
                    vm.KTPPemilik = KTPPemilikdocs;
                    vm.DOMISILI = DOMISILIdocs;
                    vm.LAPORANKEUANGAN = LAPORANKEUANGANdocs;
                    vm.REKENINGKORAN = REKENINGKORANdocs;
                    vm.DRT = DRTdocs;
                    vm.AKTAPENDIRIAN = AKTAPENDIRIANdocs;
                    vm.SKKEMENKUMHAM = SKKEMENKUMHAMdocs;
                    vm.BERITANEGARA = BERITANEGARAdocs;
                    vm.AKTAPERUBAHAN = AKTAPERUBAHANdocs;
                    vm.PROFILPERUSAHAAN = PROFILPERUSAHAANdocs;
                    vm.NIB = NIBdocs;

                    vm.DokumenSertifikatCV = DokumenSertifikatCVdocs;
                    vm.BuktiKepemilikanPeralatan = BuktiKepemilikanPeralatandocs;
                    vm.FotoPeralatan = FotoPeralatandocs;
                    vm.BuktiKerjasama = BuktiKerjasamadocs;
                    vm.LaporanDataKeuangan = LaporanDataKeuangandocs;
                    vm.CVTenagaAhli = CVTenagaAhlidocs;

                    //var zzzz = 0;
                    //RegVendorExt vendorExtss = ctx.RegVendorExts.Where(x => x.RegVendorId == IdRegVendor).FirstOrDefault();

                    return vm;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public VendorExtViewModelJaws GetVendorDetailNew(int idVendor)
        {
            //int IdRegVendor = ctx.RegVendors.Where(x => x.NoPengajuan == noPengajuan).FirstOrDefault().Id;
            Vendor rv = ctx.Vendors.Where(x => x.Id == idVendor).FirstOrDefault();

            VendorExt vendorExt = ctx.VendorExts.Where(x => x.VendorId == idVendor).FirstOrDefault();
            if (vendorExt == null)
            {
                var newData = new VendorExtViewModelJaws();
                newData.id = idVendor;
                newData.TipeVendor = 0;
                newData.Nama = rv.Nama;
                newData.Alamat = rv.Alamat;
                newData.Email = rv.Email;
                newData.Website = rv.Website;
                newData.Telepon = rv.Telepon;
                newData.Provinsi = rv.Provinsi;
                return newData;
            }

            Guid vendorExtId = vendorExt.Id;

            List<VendorHumanResourceExtViewModels> hrExt = (from a in ctx.VendorExtHumanResources
                                                            where a.VendorExtId == vendorExt.Id
                                                            select new VendorHumanResourceExtViewModels
                                                            {
                                                                ResourceFullName = a.ResourceFullName,
                                                                ResourceDateOfBirth = a.ResourceDateOfBirth,
                                                                ResourceLastEduCode = a.ResourceLastEduCode,
                                                                ResourceExperienceCode = a.ResourceExperienceCode,
                                                                ResourceExpertise = a.ResourceExpertise,
                                                                ResourceExperienceYears = a.ResourceExperienceYears
                                                            }).Distinct().ToList();


            List<VendorEquipmentExtViewModels> VEExt = (from a in ctx.VendorExtEquipments
                                                        where a.VendorExtId == vendorExt.Id
                                                        select new VendorEquipmentExtViewModels
                                                        {
                                                            EquipmentName = a.EquipmentName,
                                                            EquipmentQty = a.EquipmentQty,
                                                            EquipmentCapacity = a.EquipmentCapacity,
                                                            EquipmentMake = a.EquipmentMake,
                                                            EquipmentMakeYear = a.EquipmentMakeYear,
                                                            EquipmentConditionCode = a.EquipmentConditionCode,
                                                            EquipmentLocation = a.EquipmentLocation
                                                        }).Distinct().ToList();

            List<VendorJobHistoryExtViewModels> VJHExt = (from a in ctx.VendorExtJobHistories
                                                          where a.VendorExtId == vendorExt.Id
                                                          select new VendorJobHistoryExtViewModels
                                                          {
                                                              JobTitle = a.JobTitle,
                                                              JobLocation = a.JobLocation,
                                                              JobClient = a.JobClient,
                                                              JobType = a.JobType,
                                                              JobStartDate = a.JobStartDate,
                                                              JobContractNum = a.JobContractNum,
                                                              JobContractDate = a.JobContractDate,
                                                              JobContractAmount = a.JobContractAmount,
                                                              JobContractAmountCurrencyCode = a.JobContractAmountCurrencyCode
                                                              //JobContractAmountCurrencyCode = String.IsNullOrEmpty(a.JobContractAmountCurrencyCode) ? "" : ctx.ReferenceDatas.Where(x => x.Code == a.JobContractAmountCurrencyCode).FirstOrDefault().LocalizedName,
                                                          }).Distinct().ToList();

            VendorFinStatementExtViewModels VFSExt = (from a in ctx.VendorExtFinStatements
                                                      where a.VendorExtId == vendorExt.Id
                                                      select new VendorFinStatementExtViewModels
                                                      {
                                                          FinStmtYear = a.FinStmtYear,
                                                          FinStmtCurrencyCode = String.IsNullOrEmpty(a.FinStmtCurrencyCode) ? "" : ctx.ReferenceDatas.Where(x => x.Code == a.FinStmtCurrencyCode).FirstOrDefault().LocalizedName,
                                                          FinStmtCurrencyCodeCode = a.FinStmtCurrencyCode,
                                                          FinStmtAktivaLancar = a.FinStmtAktivaLancar,
                                                          FinStmtHutangLancar = a.FinStmtHutangLancar,
                                                          FinStmtRasioLikuiditas = a.FinStmtRasioLikuiditas,
                                                          FinStmtTotalHutang = a.FinStmtTotalHutang,
                                                          FinStmtEkuitas = a.FinStmtEkuitas,
                                                          FinStmtDebtToEquityRatio = a.FinStmtDebtToEquityRation,
                                                          FinStmtNetProfitLoss = a.FinStmtNetProfitLoss,
                                                          FinStmtReturnOfEquity = a.FinStmtReturnOfEquity,
                                                          FinStmtKas = a.FinStmtKas,
                                                          FinStmtTotalAktiva = a.FinStmtTotalAktiva,
                                                          FinStmtAuditStatusCode = a.FinStmtAuditStatusCode
                                                      }).FirstOrDefault();


            VendorExtViewModelJaws vm = new VendorExtViewModelJaws
            {
                id = idVendor,
                //TipeVendor = Int16.Parse(vendorExt.JenisVendor == null ? "10" : vendorExt.JenisVendor),

                //Provinsi = String.IsNullOrEmpty(rv.Provinsi) ? "" : ctx.ReferenceDatas.Where(x => x.Code == rv.Provinsi && x.Qualifier == "DUKCAPILPROV").FirstOrDefault().LocalizedName,
                //FirstLevelDivisionCode = String.IsNullOrEmpty(vendorExt.FirstLevelDivisionCode) ? "" : ctx.ReferenceDatas.Where(x => x.Code == vendorExt.FirstLevelDivisionCode && x.Qualifier == "DUKCAPILKOTA").FirstOrDefault().LocalizedName,
                //SecondLevelDivisionCode = String.IsNullOrEmpty(vendorExt.SecondLevelDivisionCode) ? "" : ctx.ReferenceDatas.Where(x => x.Code == vendorExt.SecondLevelDivisionCode && x.Qualifier == "DUKCAPILKECAMATAN").FirstOrDefault().LocalizedName,
                //ThirdLevelDivisionCode = String.IsNullOrEmpty(vendorExt.ThirdLevelDivisionCode) ? "" : ctx.ReferenceDatas.Where(x => x.Code == vendorExt.ThirdLevelDivisionCode && x.Qualifier == "DUKCAPILKELURAHAN").FirstOrDefault().LocalizedName,

                //for edit
                ProvinsiCode = rv.Provinsi,
                //FirstLevelDivisionCodeCode = vendorExt.FirstLevelDivisionCode,
                //SecondLevelDivisionCodeCode = vendorExt.SecondLevelDivisionCode,
                //ThirdLevelDivisionCodeCode = vendorExt.ThirdLevelDivisionCode,

                Nama = rv.Nama,
                Alamat = rv.Alamat,
                Email = rv.Email,
                Website = rv.Website,
                Telepon = rv.Telepon,
            };

            if (vendorExt.JenisVendor != null)
            {
                vm.TipeVendor = Int16.Parse(vendorExt.JenisVendor == null ? "10" : vendorExt.JenisVendor);

                var provLoczName = ctx.ReferenceDatas.Where(x => x.LocalizedName == rv.Provinsi && x.Qualifier == "DUKCAPILPROV").FirstOrDefault();
                if (provLoczName != null)
                {
                    vm.Provinsi = provLoczName.LocalizedName;
                }
                else
                {
                    if (!(String.IsNullOrEmpty(rv.Provinsi)))
                        vm.Provinsi = rv.Provinsi;
                    else
                        vm.Provinsi = "";
                }
                //vm.Provinsi = String.IsNullOrEmpty(rv.Provinsi) ? "" : ctx.ReferenceDatas.Where(x => x.LocalizedName == rv.Provinsi && x.Qualifier == "DUKCAPILPROV").FirstOrDefault().LocalizedName;
                //vm.FirstLevelDivisionCode = String.IsNullOrEmpty(vendorExt.FirstLevelDivisionCode) ? "" : ctx.ReferenceDatas.Where(x => x.Code == vendorExt.FirstLevelDivisionCode && x.Qualifier == "DUKCAPILKOTA").FirstOrDefault().LocalizedName;
                var FLDName = ctx.ReferenceDatas.Where(x => x.Code == vendorExt.FirstLevelDivisionCode && x.Qualifier == "DUKCAPILKOTA").FirstOrDefault();
                if (FLDName != null)
                {
                    vm.FirstLevelDivisionCode = FLDName.LocalizedName;
                }
                else
                {
                    vm.FirstLevelDivisionCode = "";
                }
                //vm.SecondLevelDivisionCode = String.IsNullOrEmpty(vendorExt.SecondLevelDivisionCode) ? "" : ctx.ReferenceDatas.Where(x => x.Code == vendorExt.SecondLevelDivisionCode && x.Qualifier == "DUKCAPILKECAMATAN").FirstOrDefault().LocalizedName;
                var SLDName = ctx.ReferenceDatas.Where(x => x.Code == vendorExt.SecondLevelDivisionCode && x.Qualifier == "DUKCAPILKECAMATAN").FirstOrDefault();
                if (SLDName != null)
                {
                    vm.SecondLevelDivisionCode = SLDName.LocalizedName;
                }
                else
                {
                    vm.SecondLevelDivisionCode = "";
                }
                //vm.ThirdLevelDivisionCode = String.IsNullOrEmpty(vendorExt.ThirdLevelDivisionCode) ? "" : ctx.ReferenceDatas.Where(x => x.Code == vendorExt.ThirdLevelDivisionCode && x.Qualifier == "DUKCAPILKELURAHAN").FirstOrDefault().LocalizedName;
                var TLDName = ctx.ReferenceDatas.Where(x => x.Code == vendorExt.ThirdLevelDivisionCode && x.Qualifier == "DUKCAPILKELURAHAN").FirstOrDefault();
                if (TLDName != null)
                {
                    vm.ThirdLevelDivisionCode = TLDName.LocalizedName;
                }
                else
                {
                    vm.ThirdLevelDivisionCode = "";
                }

                vm.FirstLevelDivisionCodeCode = vendorExt.FirstLevelDivisionCode;
                vm.SecondLevelDivisionCodeCode = vendorExt.SecondLevelDivisionCode;
                vm.ThirdLevelDivisionCodeCode = vendorExt.ThirdLevelDivisionCode;
            }
            else
            {
                vm.TipeVendor = 0;
                vm.Provinsi = "";
                vm.FirstLevelDivisionCodeCode = "";
                vm.SecondLevelDivisionCodeCode = "";
                vm.ThirdLevelDivisionCodeCode = "";

                vm.FirstLevelDivisionCodeCode = "";
                vm.SecondLevelDivisionCodeCode = "";
                vm.ThirdLevelDivisionCodeCode = "";
            }



            VendorRegExtViewModels VendorRegExt = new VendorRegExtViewModels
            {
                KategoriUsaha = vendorExt.KategoriUsaha,
                KategoriVendor = vendorExt.KategoriVendor,
                BentukBadanUsaha = vendorExt.BentukBadanUsaha,
                StatusPerusahaan = vendorExt.StatusPerusahaan,
                EstablishedDate = vendorExt.EstablishedDate == null ? DateTime.MinValue : vendorExt.EstablishedDate.Value,
                //CountryCode = ctx.ReferenceDatas.Where(x => x.Code == vendorExt.CountryCode).FirstOrDefault().LocalizedName,
                CountryCodeCode = vendorExt.CountryCode,
                PostalCode = vendorExt.PostalCode,
                Fax = vendorExt.Fax,
                WorkUnitCode = vendorExt.WorkUnitCode,

                SegBidangUsahaCode = vendorExt.SegBidangUsahaCode,
                //changes multiple
                SegKelompokUsahaCodeSingle = vendorExt.SegKelompokUsahaCode,

                //SegBidangUsahaCodes = ctx.ReferenceDatas.Where(x => x.Code == vendorExt.SegBidangUsahaCode && x.Qualifier == "SegBidangUsahaCode").FirstOrDefault().LocalizedName,
                //changes multiple
                //SegSubBidangUsahaCode = ctx.ReferenceDatas.Where(x => x.Code == vendorExt.SegSubBidangUsahaCode && x.Qualifier == "SegSubBidangUsahaCode").FirstOrDefault().LocalizedName,
                SegSubBidangUsahaCode = vendorExt.SegSubBidangUsahaCode,
                //SegKualifikasiGrade = ctx.ReferenceDatas.Where(x => x.Code == vendorExt.SegKualifikasiGrade && x.Qualifier == "SegKualifikasiGradeCode").FirstOrDefault().LocalizedName,

                IndivName = vendorExt.IndivName == null ? "" : vendorExt.IndivName,
                IndivAbbrevName = vendorExt.IndivAbbrevName == null ? "" : vendorExt.IndivAbbrevName,
                IndivGiidNo = vendorExt.IndivGiidNo == null ? "" : vendorExt.IndivGiidNo,
                IndivGiidValidUntil = vendorExt.IndivGiidValidUntil == null ? DateTime.MinValue : vendorExt.IndivGiidValidUntil.Value,
                IndivAddress = vendorExt.IndivAddress == null ? "" : vendorExt.IndivAddress,
                IndivCountryCode = vendorExt.IndivCountryCode == null ? "" : vendorExt.IndivCountryCode,
                IndivFirstLevelDivisionCode = vendorExt.IndivFirstLevelDivisionCode == null ? "" : vendorExt.IndivFirstLevelDivisionCode,
                IndivSecondLevelDivisionCode = vendorExt.IndivSecondLevelDivisionCode == null ? "" : vendorExt.IndivSecondLevelDivisionCode,
                IndivThirdLevelDivisionCode = vendorExt.IndivThirdLevelDivisionCode == null ? "" : vendorExt.IndivThirdLevelDivisionCode,
                IndivPostalCode = vendorExt.IndivPostalCode == null ? "" : vendorExt.IndivPostalCode,
                IndivContactPersonName = vendorExt.IndivContactPersonName == null ? "" : vendorExt.IndivContactPersonName,
                IndivContactPhoneNum = vendorExt.IndivContactPhoneNum == null ? "" : vendorExt.IndivContactPhoneNum,

                IndivStateProvinceCode = vendorExt.IndivStateProvinceCode,
                IndivPhoneNum = vendorExt.IndivPhoneNum,
                IndivFax = vendorExt.IndivFax,
                IndivEmail = vendorExt.IndivEmail,

                PrinRepOfficeAddress = vendorExt.PrinRepOfficeAddress,
                PrinRepOfficeContactPhoneNum = vendorExt.PrinRepOfficeContactPhoneNum,
                PrinRepOfficeFaxNum = vendorExt.PrinRepOfficeFaxNum,
                PrinRepOfficeEmail = vendorExt.PrinRepOfficeEmail,
                PrinWebsite = vendorExt.PrinWebsite,
                SubDistrict = vendorExt.SubDistrict,
                Village = vendorExt.Village,
                PrinRepPosition = vendorExt.PrinRepPosition,
                IndivGiidDocId = vendorExt.IndivGiidDocId,
                PrinRepOfficeLocalAddress = vendorExt.PrinRepOfficeLocalAddress,
                CPName = vendorExt.CPName,
                IsPKP = vendorExt.IsPKP,

                DirPersonGiidNo = vendorExt.DirPersonGiidNo,
                DirPersonName = vendorExt.DirPersonName,
                DirPersonPosition = vendorExt.DirPersonPosition,
                DirPersonReligionCode = vendorExt.DirPersonReligionCode,
                DirPersonBirthDay = vendorExt.DirPersonBirthDay
            };
            if (vendorExt.CountryCode != null)
            {
                VendorRegExt.CountryCode = ctx.ReferenceDatas.Where(x => x.Code == vendorExt.CountryCode).FirstOrDefault().LocalizedName;
            }

            if (vendorExt.SegBidangUsahaCode != null)
            {
                VendorRegExt.SegBidangUsahaCodes = ctx.ReferenceDatas.Where(x => x.Code == vendorExt.SegBidangUsahaCode && x.Qualifier == "SegBidangUsahaCode").FirstOrDefault() == null ? "" : ctx.ReferenceDatas.Where(x => x.Code == vendorExt.SegBidangUsahaCode && x.Qualifier == "SegBidangUsahaCode").FirstOrDefault().LocalizedName;
            }
            if (vendorExt.SegKualifikasiGrade != null)
            {
                //VendorRegExt.SegKualifikasiGrade = ctx.ReferenceDatas.Where(x => x.Code == vendorExt.SegKualifikasiGrade && x.Qualifier == "SegKualifikasiGradeCode").FirstOrDefault() == null ? "" : ctx.ReferenceDatas.Where(x => x.Code == vendorExt.SegKualifikasiGrade && x.Qualifier == "SegKualifikasiGradeCode").FirstOrDefault().LocalizedName;
                VendorRegExt.SegKualifikasiGrade = vendorExt.SegKualifikasiGrade;
            }
            else { VendorRegExt.SegKualifikasiGrade = ""; }
            //asdas

            //VendorBankInfoExtViewModels bankExt = new VendorBankInfoExtViewModels
            VendorBankInfoExtViewModels bankExt = (from a in ctx.VendorExtBankInfoes
                                                   where a.VendorExtId == vendorExtId
                                                   select new VendorBankInfoExtViewModels
                                                   {
                                                       BankCode = ctx.ReferenceDatas.Where(x => x.Code == a.BankCode).FirstOrDefault().LocalizedName,
                                                       BankAddress = a.BankAddress,
                                                       BankCity = a.BankCity,
                                                       Branch = a.Branch,
                                                       AccNumber = a.AccNumber,
                                                       AccName = a.AccName,
                                                       AccCurrencyCode = a.AccCurrencyCode,
                                                       BankCountry = ctx.ReferenceDatas.Where(x => x.Code == vendorExt.CountryCode).FirstOrDefault().LocalizedName,
                                                       //for edit
                                                       BankCodeCode = a.BankCode,
                                                       BankCountryCode = a.BankCountry
                                                   }).Distinct().FirstOrDefault();

            //VendorBankInfoExtViewModels bankExt = (from a in ctx.RegVendorExtBankInfoes
            //                                       where a.RegVendorExtId == vendorExtId
            //                                       select new VendorBankInfoExtViewModels
            //                                       {
            //                                           BankCode = ctx.ReferenceDatas.Where(x => x.Code == a.BankCode).FirstOrDefault().LocalizedName,
            //                                           BankCodeCode = a.BankCode,
            //                                           BankAddress = a.BankAddress,
            //                                           BankCity = a.BankCity,
            //                                           Branch = a.Branch,
            //                                           AccNumber = a.AccNumber,
            //                                           AccName = a.AccName,
            //                                           AccCurrencyCode = a.AccCurrencyCode,
            //                                           BankCountry = ctx.ReferenceDatas.Where(x => x.Code == vendorExt.CountryCode).FirstOrDefault().LocalizedName,
            //                                           BankCountryCode = vendorExt.CountryCode
            //                                       }).Distinct().FirstOrDefault();

            List<VendorPersonExtViewModels> PersonExt = (from a in ctx.VendorExtPersons
                                                         where a.VendorExtId == vendorExtId
                                                         select new VendorPersonExtViewModels
                                                         {
                                                             Name = a.Name,
                                                             Position = a.Position,
                                                             ReligionCode = a.ReligionCode,
                                                             GiidNo = a.GiidNo,
                                                             BirthDay = a.BirthDay == null ? DateTime.MinValue : a.BirthDay.Value,
                                                             ContactPhone = a.ContactPhone,
                                                             ContactEmail = a.ContactEmail,
                                                             ContactAddress = a.ContactAddress,
                                                         }).Distinct().ToList();

            VendorDokumenExts NPWPdocs = (from a in ctx.DocumentExts
                                          join b in ctx.DocumentImageExts on a.Id equals b.DocumenExtId
                                          where a.VendorExtId == vendorExtId && a.TipeDokumen == 0
                                          select new VendorDokumenExts
                                          {
                                              Iddok = b.Id,
                                              Nomor = a.Nomor,
                                              Pembuat = a.Penerbit,
                                              TipeDokumen = a.TipeDokumen.ToString(),
                                              TanggalTerbit = a.TanggalTerbit,
                                              TanggalBerakhir = a.TanggalBerakhir,
                                              //base64 = b.Content.ToString(),
                                              FileName = b.FileName,
                                              ContentType = b.ContentType
                                          }).Distinct().FirstOrDefault();
            if (NPWPdocs != null)
            {
                VendorRegExt.IndivTaxNo = NPWPdocs.Nomor;
            }
            

            VendorDokumenExts PKPdocs = (from a in ctx.DocumentExts
                                         join b in ctx.DocumentImageExts on a.Id equals b.DocumenExtId
                                         where a.VendorExtId == vendorExtId && a.TipeDokumen == 1
                                         select new VendorDokumenExts
                                         {
                                             Iddok = b.Id,
                                             Nomor = a.Nomor,
                                             Pembuat = a.Penerbit,
                                             TipeDokumen = a.TipeDokumen.ToString(),
                                             TanggalTerbit = a.TanggalTerbit,
                                             TanggalBerakhir = a.TanggalBerakhir,
                                             //base64 = b.Content.ToString(),
                                             FileName = b.FileName,
                                             ContentType = b.ContentType
                                         }).Distinct().FirstOrDefault();

            VendorDokumenExts TDPdocs = (from a in ctx.DocumentExts
                                         join b in ctx.DocumentImageExts on a.Id equals b.DocumenExtId
                                         where a.VendorExtId == vendorExtId && a.TipeDokumen == 2
                                         select new VendorDokumenExts
                                         {
                                             Iddok = b.Id,
                                             Nomor = a.Nomor,
                                             Pembuat = a.Penerbit,
                                             TipeDokumen = a.TipeDokumen.ToString(),
                                             TanggalTerbit = a.TanggalTerbit,
                                             TanggalBerakhir = a.TanggalBerakhir,
                                             //base64 = b.Content.ToString(),
                                             FileName = b.FileName,
                                             ContentType = b.ContentType
                                         }).Distinct().FirstOrDefault();

            VendorDokumenExts SIUPdocs = (from a in ctx.DocumentExts
                                          join b in ctx.DocumentImageExts on a.Id equals b.DocumenExtId
                                          where a.VendorExtId == vendorExtId && a.TipeDokumen == 3
                                          select new VendorDokumenExts
                                          {
                                              Iddok = b.Id,
                                              Nomor = a.Nomor,
                                              Pembuat = a.Penerbit,
                                              TipeDokumen = a.TipeDokumen.ToString(),
                                              TanggalTerbit = a.TanggalTerbit,
                                              TanggalBerakhir = a.TanggalBerakhir,
                                              //base64 = b.Content.ToString(),
                                              FileName = b.FileName,
                                              ContentType = b.ContentType
                                          }).Distinct().FirstOrDefault();

            VendorDokumenExts SIUJKdocs = (from a in ctx.DocumentExts
                                           join b in ctx.DocumentImageExts on a.Id equals b.DocumenExtId
                                           where a.VendorExtId == vendorExtId && a.TipeDokumen == 4
                                           select new VendorDokumenExts
                                           {
                                               Iddok = b.Id,
                                               Nomor = a.Nomor,
                                               Pembuat = a.Penerbit,
                                               TipeDokumen = a.TipeDokumen.ToString(),
                                               TanggalTerbit = a.TanggalTerbit,
                                               TanggalBerakhir = a.TanggalBerakhir,
                                               //base64 = b.Content.ToString(),
                                               FileName = b.FileName,
                                               ContentType = b.ContentType
                                           }).Distinct().FirstOrDefault();

            VendorDokumenExts AKTAdocs = (from a in ctx.DocumentExts
                                          join b in ctx.DocumentImageExts on a.Id equals b.DocumenExtId
                                          where a.VendorExtId == vendorExtId && a.TipeDokumen == 5
                                          select new VendorDokumenExts
                                          {
                                              Iddok = b.Id,
                                              Nomor = a.Nomor,
                                              Pembuat = a.Penerbit,
                                              TipeDokumen = a.TipeDokumen.ToString(),
                                              TanggalTerbit = a.TanggalTerbit,
                                              TanggalBerakhir = a.TanggalBerakhir,
                                              //base64 = b.Content.ToString(),
                                              FileName = b.FileName,
                                              ContentType = b.ContentType
                                          }).Distinct().FirstOrDefault();

            VendorDokumenExts PENGADAANdocs = (from a in ctx.DocumentExts
                                               join b in ctx.DocumentImageExts on a.Id equals b.DocumenExtId
                                               where a.VendorExtId == vendorExtId && a.TipeDokumen == 6
                                               select new VendorDokumenExts
                                               {
                                                   Iddok = b.Id,
                                                   Nomor = a.Nomor,
                                                   Pembuat = a.Penerbit,
                                                   TipeDokumen = a.TipeDokumen.ToString(),
                                                   TanggalTerbit = a.TanggalTerbit,
                                                   TanggalBerakhir = a.TanggalBerakhir,
                                                   //base64 = b.Content.ToString(),
                                                   FileName = b.FileName,
                                                   ContentType = b.ContentType
                                               }).Distinct().FirstOrDefault();

            VendorDokumenExts KTPdocs = (from a in ctx.DocumentExts
                                         join b in ctx.DocumentImageExts on a.Id equals b.DocumenExtId
                                         where a.VendorExtId == vendorExtId && a.TipeDokumen == 7
                                         select new VendorDokumenExts
                                         {
                                             Iddok = b.Id,
                                             Nomor = a.Nomor,
                                             Pembuat = a.Penerbit,
                                             TipeDokumen = a.TipeDokumen.ToString(),
                                             TanggalTerbit = a.TanggalTerbit,
                                             TanggalBerakhir = a.TanggalBerakhir,
                                             //base64 = b.Content.ToString(),
                                             FileName = b.FileName,
                                             ContentType = b.ContentType
                                         }).Distinct().FirstOrDefault();

            VendorDokumenExts SERTIFIKATdocs = (from a in ctx.DocumentExts
                                                join b in ctx.DocumentImageExts on a.Id equals b.DocumenExtId
                                                where a.VendorExtId == vendorExtId && a.TipeDokumen == 8
                                                select new VendorDokumenExts
                                                {
                                                    Iddok = b.Id,
                                                    Nomor = a.Nomor,
                                                    Pembuat = a.Penerbit,
                                                    TipeDokumen = a.TipeDokumen.ToString(),
                                                    TanggalTerbit = a.TanggalTerbit,
                                                    TanggalBerakhir = a.TanggalBerakhir,
                                                    //base64 = b.Content.ToString(),
                                                    FileName = b.FileName,
                                                    ContentType = b.ContentType
                                                }).Distinct().FirstOrDefault();

            VendorDokumenExts NPWPPemilikdocs = (from a in ctx.DocumentExts
                                                 join b in ctx.DocumentImageExts on a.Id equals b.DocumenExtId
                                                 where a.VendorExtId == vendorExtId && a.TipeDokumen == 9
                                                 select new VendorDokumenExts
                                                 {
                                                     Iddok = b.Id,
                                                     Nomor = a.Nomor,
                                                     Pembuat = a.Penerbit,
                                                     TipeDokumen = a.TipeDokumen.ToString(),
                                                     TanggalTerbit = a.TanggalTerbit,
                                                     TanggalBerakhir = a.TanggalBerakhir,
                                                     //base64 = b.Content.ToString(),
                                                     FileName = b.FileName,
                                                     ContentType = b.ContentType
                                                 }).Distinct().FirstOrDefault();

            VendorDokumenExts KTPPemilikdocs = (from a in ctx.DocumentExts
                                                join b in ctx.DocumentImageExts on a.Id equals b.DocumenExtId
                                                where a.VendorExtId == vendorExtId && a.TipeDokumen == 10
                                                select new VendorDokumenExts
                                                {
                                                    Iddok = b.Id,
                                                    Nomor = a.Nomor,
                                                    Pembuat = a.Penerbit,
                                                    TipeDokumen = a.TipeDokumen.ToString(),
                                                    TanggalTerbit = a.TanggalTerbit,
                                                    TanggalBerakhir = a.TanggalBerakhir,
                                                    //base64 = b.Content.ToString(),
                                                    FileName = b.FileName,
                                                    ContentType = b.ContentType
                                                }).Distinct().FirstOrDefault();

            VendorDokumenExts DOMISILIdocs = (from a in ctx.DocumentExts
                                              join b in ctx.DocumentImageExts on a.Id equals b.DocumenExtId
                                              where a.VendorExtId == vendorExtId && a.TipeDokumen == 11
                                              select new VendorDokumenExts
                                              {
                                                  Iddok = b.Id,
                                                  Nomor = a.Nomor,
                                                  Pembuat = a.Penerbit,
                                                  TipeDokumen = a.TipeDokumen.ToString(),
                                                  TanggalTerbit = a.TanggalTerbit,
                                                  TanggalBerakhir = a.TanggalBerakhir,
                                                  //base64 = b.Content.ToString(),
                                                  FileName = b.FileName,
                                                  ContentType = b.ContentType
                                              }).Distinct().FirstOrDefault();

            VendorDokumenExts LAPORANKEUANGANdocs = (from a in ctx.DocumentExts
                                                     join b in ctx.DocumentImageExts on a.Id equals b.DocumenExtId
                                                     where a.VendorExtId == vendorExtId && a.TipeDokumen == 12
                                                     select new VendorDokumenExts
                                                     {
                                                         Iddok = b.Id,
                                                         Nomor = a.Nomor,
                                                         Pembuat = a.Penerbit,
                                                         TipeDokumen = a.TipeDokumen.ToString(),
                                                         TanggalTerbit = a.TanggalTerbit,
                                                         TanggalBerakhir = a.TanggalBerakhir,
                                                         //base64 = b.Content.ToString(),
                                                         FileName = b.FileName,
                                                         ContentType = b.ContentType
                                                     }).Distinct().FirstOrDefault();

            VendorDokumenExts REKENINGKORANdocs = (from a in ctx.DocumentExts
                                                   join b in ctx.DocumentImageExts on a.Id equals b.DocumenExtId
                                                   where a.VendorExtId == vendorExtId && a.TipeDokumen == 13
                                                   select new VendorDokumenExts
                                                   {
                                                       Iddok = b.Id,
                                                       Nomor = a.Nomor,
                                                       Pembuat = a.Penerbit,
                                                       TipeDokumen = a.TipeDokumen.ToString(),
                                                       TanggalTerbit = a.TanggalTerbit,
                                                       TanggalBerakhir = a.TanggalBerakhir,
                                                       //base64 = b.Content.ToString(),
                                                       FileName = b.FileName,
                                                       ContentType = b.ContentType
                                                   }).Distinct().FirstOrDefault();

            VendorDokumenExts DRTdocs = (from a in ctx.DocumentExts
                                         join b in ctx.DocumentImageExts on a.Id equals b.DocumenExtId
                                         where a.VendorExtId == vendorExtId && a.TipeDokumen == 14
                                         select new VendorDokumenExts
                                         {
                                             Iddok = b.Id,
                                             Nomor = a.Nomor,
                                             Pembuat = a.Penerbit,
                                             TipeDokumen = a.TipeDokumen.ToString(),
                                             TanggalTerbit = a.TanggalTerbit,
                                             TanggalBerakhir = a.TanggalBerakhir,
                                             //base64 = b.Content.ToString(),
                                             FileName = b.FileName,
                                             ContentType = b.ContentType
                                         }).Distinct().FirstOrDefault();

            VendorDokumenExts AKTAPENDIRIANdocs = (from a in ctx.DocumentExts
                                                   join b in ctx.DocumentImageExts on a.Id equals b.DocumenExtId
                                                   where a.VendorExtId == vendorExtId && a.TipeDokumen == 15
                                                   select new VendorDokumenExts
                                                   {
                                                       Iddok = b.Id,
                                                       Nomor = a.Nomor,
                                                       Pembuat = a.Penerbit,
                                                       TipeDokumen = a.TipeDokumen.ToString(),
                                                       TanggalTerbit = a.TanggalTerbit,
                                                       TanggalBerakhir = a.TanggalBerakhir,
                                                       //base64 = b.Content.ToString(),
                                                       FileName = b.FileName,
                                                       ContentType = b.ContentType
                                                   }).Distinct().FirstOrDefault();

            VendorDokumenExts SKKEMENKUMHAMdocs = (from a in ctx.DocumentExts
                                                   join b in ctx.DocumentImageExts on a.Id equals b.DocumenExtId
                                                   where a.VendorExtId == vendorExtId && a.TipeDokumen == 16
                                                   select new VendorDokumenExts
                                                   {
                                                       Iddok = b.Id,
                                                       Nomor = a.Nomor,
                                                       Pembuat = a.Penerbit,
                                                       TipeDokumen = a.TipeDokumen.ToString(),
                                                       TanggalTerbit = a.TanggalTerbit,
                                                       TanggalBerakhir = a.TanggalBerakhir,
                                                       //base64 = b.Content.ToString(),
                                                       FileName = b.FileName,
                                                       ContentType = b.ContentType
                                                   }).Distinct().FirstOrDefault();

            VendorDokumenExts BERITANEGARAdocs = (from a in ctx.DocumentExts
                                                  join b in ctx.DocumentImageExts on a.Id equals b.DocumenExtId
                                                  where a.VendorExtId == vendorExtId && a.TipeDokumen == 17
                                                  select new VendorDokumenExts
                                                  {
                                                      Iddok = b.Id,
                                                      Nomor = a.Nomor,
                                                      Pembuat = a.Penerbit,
                                                      TipeDokumen = a.TipeDokumen.ToString(),
                                                      TanggalTerbit = a.TanggalTerbit,
                                                      TanggalBerakhir = a.TanggalBerakhir,
                                                      //base64 = b.Content.ToString(),
                                                      FileName = b.FileName,
                                                      ContentType = b.ContentType
                                                  }).Distinct().FirstOrDefault();

            VendorDokumenExts AKTAPERUBAHANdocs = (from a in ctx.DocumentExts
                                                   join b in ctx.DocumentImageExts on a.Id equals b.DocumenExtId
                                                   where a.VendorExtId == vendorExtId && a.TipeDokumen == 18
                                                   select new VendorDokumenExts
                                                   {
                                                       Iddok = b.Id,
                                                       Nomor = a.Nomor,
                                                       Pembuat = a.Penerbit,
                                                       TipeDokumen = a.TipeDokumen.ToString(),
                                                       TanggalTerbit = a.TanggalTerbit,
                                                       TanggalBerakhir = a.TanggalBerakhir,
                                                       //base64 = b.Content.ToString(),
                                                       FileName = b.FileName,
                                                       ContentType = b.ContentType
                                                   }).Distinct().FirstOrDefault();


            VendorDokumenExts PROFILPERUSAHAANdocs = (from a in ctx.DocumentExts
                                                      join b in ctx.DocumentImageExts on a.Id equals b.DocumenExtId
                                                      //where a.VendorExtId == vendorExtId && a.TipeDokumen == 19
                                                      where a.VendorExtId == vendorExtId && a.TipeDokumen == 19
                                                      select new VendorDokumenExts
                                                      {
                                                          Iddok = b.Id,
                                                          Nomor = a.Nomor,
                                                          Pembuat = a.Penerbit,
                                                          TipeDokumen = a.TipeDokumen.ToString(),
                                                          TanggalTerbit = a.TanggalTerbit,
                                                          TanggalBerakhir = a.TanggalBerakhir,
                                                          //base64 = b.Content.ToString(),
                                                          FileName = b.FileName,
                                                          ContentType = b.ContentType
                                                      }).Distinct().FirstOrDefault();

            VendorDokumenExts NIBdocs = (from a in ctx.DocumentExts
                                         join b in ctx.DocumentImageExts on a.Id equals b.DocumenExtId
                                         where a.VendorExtId == vendorExtId && a.TipeDokumen == 20
                                         select new VendorDokumenExts
                                         {
                                             Iddok = b.Id,
                                             Nomor = a.Nomor,
                                             Pembuat = a.Penerbit,
                                             TipeDokumen = a.TipeDokumen.ToString(),
                                             TanggalTerbit = a.TanggalTerbit,
                                             TanggalBerakhir = a.TanggalBerakhir,
                                             //base64 = b.Content.ToString(),
                                             FileName = b.FileName,
                                             ContentType = b.ContentType
                                         }).Distinct().FirstOrDefault();

            VendorDokumenExts DokumenSertifikatCVdocs = (from a in ctx.DocumentExts
                                                         join b in ctx.DocumentImageExts on a.Id equals b.DocumenExtId
                                                         where a.VendorExtId == vendorExtId && a.TipeDokumen == 21
                                                         select new VendorDokumenExts
                                                         {
                                                             Iddok = b.Id,
                                                             Nomor = a.Nomor,
                                                             Pembuat = a.Penerbit,
                                                             TipeDokumen = a.TipeDokumen.ToString(),
                                                             TanggalTerbit = a.TanggalTerbit,
                                                             TanggalBerakhir = a.TanggalBerakhir,
                                                             //base64 = b.Content.ToString(),
                                                             FileName = b.FileName,
                                                             ContentType = b.ContentType
                                                         }).Distinct().FirstOrDefault();

            VendorDokumenExts BuktiKepemilikanPeralatandocs = (from a in ctx.DocumentExts
                                                               join b in ctx.DocumentImageExts on a.Id equals b.DocumenExtId
                                                               where a.VendorExtId == vendorExtId && a.TipeDokumen == 22
                                                               select new VendorDokumenExts
                                                               {
                                                                   Iddok = b.Id,
                                                                   Nomor = a.Nomor,
                                                                   Pembuat = a.Penerbit,
                                                                   TipeDokumen = a.TipeDokumen.ToString(),
                                                                   TanggalTerbit = a.TanggalTerbit,
                                                                   TanggalBerakhir = a.TanggalBerakhir,
                                                                   //base64 = b.Content.ToString(),
                                                                   FileName = b.FileName,
                                                                   ContentType = b.ContentType
                                                               }).Distinct().FirstOrDefault();

            VendorDokumenExts FotoPeralatandocs = (from a in ctx.DocumentExts
                                                   join b in ctx.DocumentImageExts on a.Id equals b.DocumenExtId
                                                   where a.VendorExtId == vendorExtId && a.TipeDokumen == 23
                                                   select new VendorDokumenExts
                                                   {
                                                       Iddok = b.Id,
                                                       Nomor = a.Nomor,
                                                       Pembuat = a.Penerbit,
                                                       TipeDokumen = a.TipeDokumen.ToString(),
                                                       TanggalTerbit = a.TanggalTerbit,
                                                       TanggalBerakhir = a.TanggalBerakhir,
                                                       //base64 = b.Content.ToString(),
                                                       FileName = b.FileName,
                                                       ContentType = b.ContentType
                                                   }).Distinct().FirstOrDefault();

            VendorDokumenExts BuktiKerjasamadocs = (from a in ctx.DocumentExts
                                                    join b in ctx.DocumentImageExts on a.Id equals b.DocumenExtId
                                                    where a.VendorExtId == vendorExtId && a.TipeDokumen == 24
                                                    select new VendorDokumenExts
                                                    {
                                                        Iddok = b.Id,
                                                        Nomor = a.Nomor,
                                                        Pembuat = a.Penerbit,
                                                        TipeDokumen = a.TipeDokumen.ToString(),
                                                        TanggalTerbit = a.TanggalTerbit,
                                                        TanggalBerakhir = a.TanggalBerakhir,
                                                        //base64 = b.Content.ToString(),
                                                        FileName = b.FileName,
                                                        ContentType = b.ContentType
                                                    }).Distinct().FirstOrDefault();

            VendorDokumenExts LaporanDataKeuangandocs = (from a in ctx.DocumentExts
                                                         join b in ctx.DocumentImageExts on a.Id equals b.DocumenExtId
                                                         where a.VendorExtId == vendorExtId && a.TipeDokumen == 25
                                                         select new VendorDokumenExts
                                                         {
                                                             Iddok = b.Id,
                                                             Nomor = a.Nomor,
                                                             Pembuat = a.Penerbit,
                                                             TipeDokumen = a.TipeDokumen.ToString(),
                                                             TanggalTerbit = a.TanggalTerbit,
                                                             TanggalBerakhir = a.TanggalBerakhir,
                                                             //base64 = b.Content.ToString(),
                                                             FileName = b.FileName,
                                                             ContentType = b.ContentType
                                                         }).Distinct().FirstOrDefault();

            VendorDokumenExts CVTenagaAhlidocs = (from a in ctx.DocumentExts
                                                  join b in ctx.DocumentImageExts on a.Id equals b.DocumenExtId
                                                  where a.VendorExtId == vendorExtId && a.TipeDokumen == 30
                                                  select new VendorDokumenExts
                                                  {
                                                      Iddok = b.Id,
                                                      Nomor = a.Nomor,
                                                      Pembuat = a.Penerbit,
                                                      TipeDokumen = a.TipeDokumen.ToString(),
                                                      TanggalTerbit = a.TanggalTerbit,
                                                      TanggalBerakhir = a.TanggalBerakhir,
                                                      //base64 = b.Content.ToString(),
                                                      FileName = b.FileName,
                                                      ContentType = b.ContentType
                                                  }).Distinct().FirstOrDefault();
            vm.id = rv.Id;
            vm.VendorRegExt = VendorRegExt;
            vm.VendorBankInfoExt = bankExt;
            vm.VendorPersonExt = PersonExt;
            vm.VendorHumanResourceExt = hrExt;
            vm.VendorEquipmentExt = VEExt;
            vm.VendorJobHistoryExt = VJHExt;
            vm.VendorFinStatementExt = VFSExt;
            vm.NPWP = NPWPdocs;
            vm.PKP = PKPdocs;
            vm.TDP = TDPdocs;
            vm.SIUP = SIUPdocs;
            vm.SIUJK = SIUJKdocs;
            vm.AKTA = AKTAdocs;
            vm.PENGADAAN = PENGADAANdocs;
            vm.KTP = KTPdocs;
            vm.SERTIFIKAT = SERTIFIKATdocs;
            vm.NPWPPemilik = NPWPPemilikdocs;
            vm.KTPPemilik = KTPPemilikdocs;
            vm.DOMISILI = DOMISILIdocs;
            vm.LAPORANKEUANGAN = LAPORANKEUANGANdocs;
            vm.REKENINGKORAN = REKENINGKORANdocs;
            vm.DRT = DRTdocs;
            vm.AKTAPENDIRIAN = AKTAPENDIRIANdocs;
            vm.SKKEMENKUMHAM = SKKEMENKUMHAMdocs;
            vm.BERITANEGARA = BERITANEGARAdocs;
            vm.AKTAPERUBAHAN = AKTAPERUBAHANdocs;
            vm.PROFILPERUSAHAAN = PROFILPERUSAHAANdocs;
            vm.NIB = NIBdocs;

            vm.DokumenSertifikatCV = DokumenSertifikatCVdocs;
            vm.BuktiKepemilikanPeralatan = BuktiKepemilikanPeralatandocs;
            vm.FotoPeralatan = FotoPeralatandocs;
            vm.BuktiKerjasama = BuktiKerjasamadocs;
            vm.LaporanDataKeuangan = LaporanDataKeuangandocs;
            vm.CVTenagaAhli = CVTenagaAhlidocs;

            var zzzz = 0;
            //RegVendorExt vendorExtss = ctx.RegVendorExts.Where(x => x.RegVendorId == IdRegVendor).FirstOrDefault();

            return vm;
        }



        public VWSanksi GetdetailSanksiVendor(int VendorId)
        {
            var ListVendorWithSanksi = (from b in ctx.Vendors
                                        join c in ctx.Sanksis on b.Id equals c.VendorId into ps
                                        //join c in q on b.Id equals c.VendorId into ps
                                        from p in ps.DefaultIfEmpty()
                                        where b.Id == VendorId
                                        select new VWSanksi
                                        {
                                            VendorId = b.Id,
                                            NamaVendor = b.Nama,
                                            NomorVendor = b.NomorVendor,
                                            DecisionTypeCode = String.IsNullOrEmpty(p.DecisionTypeCode) ? "SV01" : p.DecisionTypeCode,
                                            DecisionDescription = String.IsNullOrEmpty(p.DecisionDescription) ? "" : p.DecisionDescription,
                                            DecisionValidUntil = String.IsNullOrEmpty(p.DecisionValidUntil.ToString()) ? DateTime.MinValue : p.DecisionValidUntil,
                                            CreatedOn = String.IsNullOrEmpty(p.DecisionValidUntil.ToString()) ? DateTime.MinValue : p.CreatedOn,
                                        }).Distinct().OrderByDescending(xx => xx.CreatedOn).ToList();

            List<VWSanksi> Ulu = new List<VWSanksi>();
            foreach (var item in ListVendorWithSanksi)
            {
                VWSanksi Ula = new VWSanksi();
                Ula.VendorId = item.VendorId;
                Ula.NamaVendor = item.NamaVendor;
                Ula.NomorVendor = item.NomorVendor;
                Ula.DecisionTypeCode = IsDateBeforeOrToday(item.DecisionValidUntil) == false ? "SV01" : item.DecisionTypeCode;
                Ula.DecisionDescription = IsDateBeforeOrToday(item.DecisionValidUntil) == false ? "" : item.DecisionDescription;
                Ula.DalamMasaSanksi = IsDateBeforeOrToday(item.DecisionValidUntil) == true ? "masih" : "tidak";
                Ula.DecisionValidUntil = item.DecisionValidUntil;
                var check = Ulu.Where(xx => xx.VendorId == Ula.VendorId).FirstOrDefault() == null ? "belum" : "ada";
                if (check == "belum") Ulu.Add(Ula);
            }

            VWSanksi detailSanksiVendor = new VWSanksi();
            detailSanksiVendor.VendorId = Ulu[0].VendorId;
            detailSanksiVendor.NamaVendor = Ulu[0].NamaVendor;
            detailSanksiVendor.NomorVendor = Ulu[0].NomorVendor;
            detailSanksiVendor.DecisionTypeCode = Ulu[0].DecisionTypeCode;
            detailSanksiVendor.DecisionDescription = Ulu[0].DecisionDescription;
            detailSanksiVendor.DalamMasaSanksi = Ulu[0].DalamMasaSanksi;
            detailSanksiVendor.DecisionValidUntil = Ulu[0].DecisionValidUntil;
            return detailSanksiVendor;
        }

        public int GetIdVendorFromOwner(Guid owner)
        {
            var id = ctx.Vendors.Where(x => x.Owner == owner).FirstOrDefault().Id;
            return id;
        }

    }
}
