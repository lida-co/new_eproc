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
using Reston.Eproc.Model.Monitoring.Entities;


namespace Reston.Pinata.Model.PengadaanRepository
{
    public interface IBudgetRepo
    {
        DataTableBudget List(int start, int limit, string cari);
        ResultMessage add(List<COA> nlstcoa, Guid UserId);
        ResultMessage saveDokumenBudget(DokumenBudget dokumenSpkNonPks, Guid UserId);
        List<VWDokumenBudget> GetListDokumenBudget();
        DataTableBudgeting GetAllCOAPengadaan(/*string search,*/string department, string branch, string year, string jenis, /*string month,*/ int start, int length, int sortColumn, string sortDirection);
        Guid GetDataHeader(Guid PengadaanId);
        List<BudgetingPengadaanHeader> GetDataHeader(Guid PengadaanId, decimal TotalInput);
        List<BudgetingPengadaanDetail> GetDataDetail(Guid HeaderId);
        VWDokumenBudget CekUpload();
        void RemoveHeaderCOA(Guid PengadaanId);
        BudgetingPengadaanHeader SaveAllocation(BudgetingPengadaanHeader budgetAllocation);
        DataTableBudgeting GetLoadCOAPengadaan(string branch, string department, string year, /*string month,*/ string jenispembelanjaan, string pengadaanid, int start, int length);
        ResultMessage SaveBudgetingsFromExcel(List<Budgeting> budgetings);
        int? GetDokumenBudgetLatestVersion(string tahun, string jenis);
    }

    public class BudgetRepo : IBudgetRepo
    {
        AppDbContext ctx;

        public BudgetRepo(AppDbContext j)
        {
            ctx = j;
            //ctx.Configuration.ProxyCreationEnabled = false;
            ctx.Configuration.LazyLoadingEnabled = true;
        }

        ResultMessage msg = new ResultMessage();



        public DataTableBudget List(int start, int limit, string cari)
        {
            cari = cari == null ? "" : cari;
            DataTableBudget dtTable = new DataTableBudget();
            if (limit > 0)
            {
                var data = ctx.COAs.AsQueryable();
                dtTable.recordsTotal = data.Count();
                if (!string.IsNullOrEmpty(cari))
                {
                    data = data.Where(d => d.Pengadaan.Judul.Contains(cari));
                }
                dtTable.recordsFiltered = data.Count();
                data = data.OrderByDescending(d => d.UploadedOn).Skip(start).Take(limit);
                var lol = data.ToList();
                dtTable.data = data.Select(d => new VVWCOA
                {
                    Id = d.Id,
                    Divisi = d.Divisi,
                    GroupAset = d.GroupAset,
                    JenisAset = d.JenisAset,
                    NilaiAset = d.NilaiAset,
                    Periode = d.Periode,
                    Region = d.Region,
                    NoCoa = d.NoCoa
                }).ToList();
            }
            return dtTable;
        }

        public ResultMessage add(List<COA> nlstcoa, Guid UserId)
        {
            try
            {
                foreach (var ncoa in nlstcoa)
                {
                    var oCoa = ctx.COAs.Where(d => d.NoCoa == ncoa.NoCoa).FirstOrDefault();
                    if (oCoa == null)
                    {
                        //var pengadaan = ctx.Pengadaans.Where(d => d.NoCOA == ncoa.NoCoa).FirstOrDefault();
                        ////if (pengadaan != null)
                       // {
                           // ncoa.PengadaanId = pengadaan.Id;
                            ncoa.UploadedBy = UserId;
                            ncoa.UploadedOn = DateTime.Now;
                            ctx.COAs.Add(ncoa);
                       // }
                    }
                    else
                    {
                        oCoa.Divisi = ncoa.Divisi;
                        oCoa.GroupAset = ncoa.GroupAset;
                        oCoa.JenisAset = ncoa.JenisAset;
                        oCoa.ModifiedUploadBy = UserId;
                        oCoa.NilaiAset = ncoa.NilaiAset;
                        oCoa.Periode = ncoa.Periode;
                        oCoa.Region = ncoa.Region;
                    }
                }
                ctx.SaveChanges();
                return new ResultMessage()
                {
                    message=Common.SaveSukses(),
                    status=HttpStatusCode.OK
                };
            }
            catch(Exception ex)
            {
                return  new ResultMessage()
                {
                    message = ex.ToString(),
                    status=HttpStatusCode.NotImplemented
                };;
            }
        }

        //public ResultMessage saveDokumenBudget(DokumenBudget dokumenBudget, Guid UserId)
        //{
        //    try
        //    {
        //        int oVer = ctx.DokumenBudgets.Count();
        //        dokumenBudget.Version = oVer + 1;
        //        dokumenBudget.Status = 1;

        //        //var oData = ctx.Spk.Find(dokumenSpkNonPks.SpkId);
        //        //if (oData == null) return new ResultMessage
        //        //{
        //        //    status = HttpStatusCode.Forbidden,
        //        //    message = Common.NotSave()
        //        //};
        //        dokumenBudget.CreatedOn = DateTime.Now;
        //        dokumenBudget.CreatedBy = UserId;
        //        ctx.DokumenBudgets.Add(dokumenBudget);
        //        ctx.SaveChanges();

        //        return new ResultMessage
        //        {
        //            status = HttpStatusCode.OK,
        //            message = Common.SaveSukses()
        //            //Id = dokumenSpkNonPks.Id.ToString()
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResultMessage()
        //        {
        //            status = HttpStatusCode.NotImplemented,
        //            message = ex.ToString()
        //        };
        //    }
        //}

        public ResultMessage saveDokumenBudget(DokumenBudget dokumenBudget, Guid UserId)
        {
            try
            {
                int oVer = 0;
                //var checkYear = (from a in ctx.DokumenBudgets
                //                  where a.Year == dokumenBudget.Year && a.Jenis == dokumenBudget.Jenis
                //                  select new VWDokumenBudget
                //                  {
                //                      Year = a.Year,
                //                      Jenis = a.Jenis
                //                  }).Distinct().OrderByDescending(x => x.Year).FirstOrDefault();
                //if (checkYear != null)
                //{
                //    oVer = ctx.DokumenBudgets.Where(u => u.Year == checkYear.Year && u.Jenis == checkYear.Jenis).Count();
                //}

                var budgetCoaList = (from budgetCoas in ctx.DokumenBudgets
                                     where budgetCoas.Year.Equals(dokumenBudget.Year)
                                        && budgetCoas.Jenis.Equals(dokumenBudget.Jenis) // filter belum ada
                                     select budgetCoas)
                                .GroupBy(x => x.Version)
                                .ToList()
                                .DefaultIfEmpty()
                                .Last();

                var maxVersion = budgetCoaList != null && budgetCoaList.First() != null ? budgetCoaList.First().Version : 0;
                if (maxVersion != null)
                {
                    oVer = Convert.ToInt32(maxVersion);
                }
                //else
                //{
                //    oVer = ctx.DokumenBudgets.Count();
                //}
                dokumenBudget.Version = oVer + 1;
                dokumenBudget.Status = 1;
                int second = DateTime.Now.Second;
                if (second != 0) second = 00;
                dokumenBudget.ProcessId = "PID-" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Minute + second;
                dokumenBudget.CreateOn = DateTime.Now;
                dokumenBudget.CreateBy = UserId;

                ctx.DokumenBudgets.Add(dokumenBudget);
                ctx.SaveChanges();

                return new ResultMessage
                {
                    status = HttpStatusCode.OK,
                    message = Common.SaveSukses()
                    //Id = dokumenSpkNonPks.Id.ToString()
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

        public int? GetDokumenBudgetLatestVersion(string tahun, string jenis)
        {
            return ctx.DokumenBudgets
                .Where(d => d.Year == tahun && d.Jenis == jenis)
                .OrderByDescending(d => d.Version)
                .Select(d => d.Version)
                .FirstOrDefault();
        }

        public List<VWDokumenBudget> GetListDokumenBudget()
        {
            return ctx.DokumenBudgets.Select(d => new VWDokumenBudget()
            {
                Id = d.Id,
                SizeFile = d.SizeFile,
                ContentType = d.ContentType,
                File = d.File
            }).ToList();
        }

        /// <summary>
        /// Insert/update list data Budgeting ke database.
        /// Data sudah di-parse dari Excel di controller.
        /// </summary>
        public ResultMessage SaveBudgetingsFromExcel(List<Budgeting> budgetings)
        {
            try
            {
                foreach (var item in budgetings)
                {
                    var existing = ctx.Budgetings.FirstOrDefault(b =>
                        b.Branch == item.Branch && b.Department == item.Department &&
                        b.COA == item.COA && b.Year == item.Year &&
                        b.Month == item.Month && b.Jenis == item.Jenis);

                    if (existing != null)
                    {
                        existing.BudgetAmount = item.BudgetAmount;
                        existing.BudgetUsage = item.BudgetUsage;
                        existing.BudgetLeft = item.BudgetLeft;
                        existing.Version = item.Version;
                        existing.Description = item.Description;
                    }
                    else
                    {
                        ctx.Budgetings.Add(item);
                    }
                }
                ctx.SaveChanges();
                return new ResultMessage { status = HttpStatusCode.OK, message = $"Berhasil import {budgetings.Count} baris" };
            }
            catch (Exception ex)
            {
                return new ResultMessage { status = HttpStatusCode.InternalServerError, message = ex.Message };
            }
        }

        public DataTableBudgeting GetAllCOAPengadaan(/*string search,*/string department, string branch, string year, string jenis, /*string month,*/ int start, int length, int sortColumn, string sortDirection)
        {
            //search = search == null ? "" : search;
            DataTableBudgeting dtTable = new DataTableBudgeting();
            if (length > 0)
            {
                System.Linq.IQueryable<Budgeting> result4 = ctx.Budgetings.AsQueryable();

                var data = result4.GroupBy(x => 
                    new { x.Branch ,
                        x.Department ,
                        x.Description ,
                        x.COA ,
                        x.Year ,
                        x.BudgetReserved ,
                        x.Version ,
                        x.Jenis}).Select(g =>
                    new { Id = g.Sum(x => x.Id)
                          ,g.Key.Branch
                          ,g.Key.Department
                          ,g.Key.Description
                          ,g.Key.COA
                          , BudgetAmount = g.Sum(x => x.BudgetAmount)
                          , BudgetUsage = g.Sum(x => x.BudgetUsage)
                          , BudgetLeft = g.Sum(x => x.BudgetLeft)
                          ,g.Key.Year
                          ,g.Key.BudgetReserved
                          ,g.Key.Version
                          ,g.Key.Jenis
                    });

                dtTable.recordsTotal = data.Count();
                if (!string.IsNullOrEmpty(branch))
                {
                    data = data.Where(d => d.Branch.Contains(branch));
                }
                if (!string.IsNullOrEmpty(year))
                {
                    data = data.Where(d => d.Year.Contains(year));
                }
                if (!string.IsNullOrEmpty(jenis))
                {
                    data = data.Where(d => d.Jenis.Contains(jenis));
                }
                if (!string.IsNullOrEmpty(department))
                {
                    data = data.Where(d => d.Department.Contains(department));
                }
                //if (!string.IsNullOrEmpty(month))
                //{
                //    data = data.Where(d => d.Month.Contains(month));
                //}
                //data = data.Where(u => u.TipeVendor == ETipeVendor.NON_REGISTER);
                if (!string.IsNullOrEmpty(year) && !string.IsNullOrEmpty(jenis))
                {
                    var budgetCoaList = (from budgetCoas in ctx.Budgetings
                                         where budgetCoas.Year.Equals(year)
                                             && budgetCoas.Jenis.Equals(jenis) // filter belum ada
                                         select budgetCoas)
                                .GroupBy(x => x.Version)
                                .ToList()
                                .DefaultIfEmpty()
                                .Last();
                    var maxVersion = budgetCoaList != null && budgetCoaList.First() != null ? budgetCoaList.First().Version : 0;

                    data = data.Where(d => d.Version == maxVersion);
                }
                dtTable.recordsFiltered = data.Count();
                if (sortDirection == "desc") {
                    if (sortColumn == 0) data = data.OrderByDescending(d => d.Branch).Skip(start).Take(length);
                    if (sortColumn == 1) data = data.OrderByDescending(d => d.Department).Skip(start).Take(length);
                    if (sortColumn == 2) data = data.OrderByDescending(d => d.Description).Skip(start).Take(length);
                    if (sortColumn == 3) data = data.OrderByDescending(d => d.COA).Skip(start).Take(length);
                    //if (sortColumn == 4) data = data.OrderByDescending(d => d.Month).Skip(start).Take(length);
                }
                if (sortDirection == "asc")
                {
                    if (sortColumn == 0) data = data.OrderBy(d => d.Branch).Skip(start).Take(length);
                    if (sortColumn == 1) data = data.OrderBy(d => d.Department).Skip(start).Take(length);
                    if (sortColumn == 2) data = data.OrderBy(d => d.Description).Skip(start).Take(length);
                    if (sortColumn == 3) data = data.OrderBy(d => d.COA).Skip(start).Take(length);
                    //if (sortColumn == 4) data = data.OrderBy(d => d.Month).Skip(start).Take(length);
                }

                dtTable.data = data.Select(x => new VWBudgeting
                {
                    Id = x.Id,
                    Branch = x.Branch,
                    Department = x.Department,
                    Description = x.Description,
                    COA = x.COA,
                    //Month = x.Month,
                    Year = x.Year, /*ctx.ReferenceDatas.Where(y => y.Code == x.Year).Select(y => y.LocalizedName).ToString(),*/
                    BudgetAmount = x.BudgetAmount,
                    BudgetUsage = x.BudgetUsage,
                    BudgetLeft = x.BudgetLeft,
                    BudgetReserved = x.BudgetReserved,
                    BudgetLeftTotal = x.BudgetLeft/* - x.BudgetUsage - x.BudgetReserved*/,
                    Inputbudget = null,
                    BudgetType = x.Jenis /* ctx.ReferenceDatas.Where(y => y.Code == x.Jenis).Select(y => y.LocalizedName).ToString()*/
                }).ToList();

                //dtTable.data = 
            }
            return dtTable;
        }


        public DataTableBudgeting GetLoadCOAPengadaan(string branch, string department, string year, /*string month,*/ string jenispembelanjaan, string pengadaanid, int start, int length)
        {
            // Trim semua parameter untuk menghindari masalah spasi
            branch = (branch ?? "").Trim();
            department = (department ?? "").Trim();
            year = (year ?? "").Trim();
            jenispembelanjaan = (jenispembelanjaan ?? "").Trim();
            pengadaanid = (pengadaanid ?? "").Trim();

            // Convert year Code (PA001) ke LocalizedName (TA-2021) untuk matching dengan Budgeting.Year
            string yearLocalizedName = year;
            if (!string.IsNullOrEmpty(year))
            {
                var refData = ctx.ReferenceDatas.FirstOrDefault(r => r.Code == year);
                if (refData != null)
                    yearLocalizedName = refData.LocalizedName;
            }

            var budgetCoaList = (from budgetCoas in ctx.Budgetings
                                 where (string.IsNullOrEmpty(branch) || budgetCoas.Branch.Contains(branch))
                                     && (string.IsNullOrEmpty(department) || budgetCoas.Department.Contains(department))
                                     && (string.IsNullOrEmpty(yearLocalizedName) || budgetCoas.Year.Contains(yearLocalizedName))
                                     && (string.IsNullOrEmpty(jenispembelanjaan) || budgetCoas.Jenis.Contains(jenispembelanjaan))
                                 select budgetCoas)
                                .GroupBy(x => x.Version)
                                .ToList()
                                .DefaultIfEmpty()
                                .Last();

            // Jika tidak ada data yang cocok, gunakan versi tertinggi yang ada
            // atau 0 jika memang tidak ada data sama sekali
            int maxVersion = 0;
            if (budgetCoaList != null && budgetCoaList.Any() && budgetCoaList.First() != null)
            {
                maxVersion = budgetCoaList.First().Version ?? 0;
            }
            else
            {
                // Fallback: cari versi tertinggi dari semua data yang ada untuk branch+department+year+jenis
                var anyVersion = ctx.Budgetings
                    .Where(b => (string.IsNullOrEmpty(branch) || b.Branch.Contains(branch))
                             && (string.IsNullOrEmpty(department) || b.Department.Contains(department))
                             && (string.IsNullOrEmpty(yearLocalizedName) || b.Year.Contains(yearLocalizedName))
                             && (string.IsNullOrEmpty(jenispembelanjaan) || b.Jenis.Contains(jenispembelanjaan)))
                    .Select(b => b.Version)
                    .DefaultIfEmpty(0)
                    .Max();
                maxVersion = anyVersion ?? 0;
            }

            var someguid = Guid.Parse(pengadaanid);

            //start grouping and sum by branch department coa
            System.Linq.IQueryable<Budgeting> groupBudgeting = ctx.Budgetings.AsQueryable();
            var data = groupBudgeting.GroupBy(x =>
                new {
                    x.Branch,
                    x.Department,
                    x.Description,
                    x.COA,
                    x.Year,
                    x.BudgetReserved,
                    x.Version,
                    x.Jenis
                }).Select(g =>
                 new {
                     Id = g.Sum(x => x.Id),
                     g.Key.Branch,
                     g.Key.Department,
                     g.Key.Description,
                     g.Key.COA,
                     BudgetAmount = g.Sum(x => x.BudgetAmount),
                     BudgetUsage = g.Sum(x => x.BudgetUsage),
                     BudgetLeft = g.Sum(x => x.BudgetLeft),
                     g.Key.Year,
                     g.Key.BudgetReserved,
                     g.Key.Version,
                     g.Key.Jenis
                 });
            //end
            //start grouping Budget On Process
            var BP = (from a in ctx.BudgetingPengadaanDetails
                      join b in ctx.BudgetingPengadaanHeaders on a.BudgetingPengadaanId equals b.Id
                      join c in ctx.Pengadaans on b.PengadaanId equals c.Id
                      where (string.IsNullOrEmpty(branch) || a.Branch.Contains(branch))
                             && (string.IsNullOrEmpty(department) || a.Department.Contains(department))
                             && (string.IsNullOrEmpty(year) || a.Year.Contains(year))
                             && (string.IsNullOrEmpty(jenispembelanjaan) || a.BudgetType.Contains(jenispembelanjaan))
                             && c.GroupPengadaan == EGroupPengadaan.DALAMPELAKSANAAN
                      select new VWBudgeting
                      {
                          Branch = a.Branch,
                          Department = a.Department,
                          Year = a.Year,
                          BudgetType = a.BudgetType,
                          BudgetReserved = a.Input,
                          NoCOA = a.NoCOA
                      }).ToList();

            List<VWBudgeting> dataBOP = BP.GroupBy(x =>
                new
                {
                    x.Branch,
                    x.Department,
                    x.NoCOA,
                    x.Year,
                    x.BudgetType
                }).Select(g =>
                 new VWBudgeting
                 {
                     Branch = g.Key.Branch,
                     Department = g.Key.Department,
                     NoCOA = g.Key.NoCOA,
                     BudgetReserved = g.Sum(x => x.BudgetReserved),
                     Year = g.Key.Year,
                     BudgetType = g.Key.BudgetType
                 }).ToList();
            //end
            
            var loadCOA = (from a in data
                           join e in (
                               from b in ctx.BudgetingPengadaanDetails
                               join c in ctx.BudgetingPengadaanHeaders on b.BudgetingPengadaanId equals c.Id into f
                               from g in f.DefaultIfEmpty()
                               where g.PengadaanId.Equals(someguid)
                               select new { NoCOA = b.NoCOA, /*Month = b.Month,*/ Inputbudget = b.Input, PengadaanId = g.PengadaanId }
                           ) on a.COA equals e.NoCOA into h
                           from i in h.Where(x => x.NoCOA.Equals(a.COA)).DefaultIfEmpty()
                           where (string.IsNullOrEmpty(branch) || a.Branch.Contains(branch))
                              && (string.IsNullOrEmpty(department) || a.Department.Contains(department))
                              && (string.IsNullOrEmpty(yearLocalizedName) || a.Year.Contains(yearLocalizedName))
                              && (string.IsNullOrEmpty(jenispembelanjaan) || a.Jenis.Contains(jenispembelanjaan))
                              && (maxVersion == 0 || a.Version == maxVersion)
                           select new VWBudgeting
                           {
                               //Checked = i != null ? a.COA.Equals(i.NoCOA) : false,
                               Id = a.Id,
                               Branch = a.Branch,
                               Department = a.Department,
                               Description = a.Description,
                               COA = a.COA,
                               //Month = a.Month,
                               Year = a.Year,
                               BudgetAmount = a.BudgetAmount,
                               BudgetUsage = a.BudgetUsage,
                               BudgetLeft = a.BudgetLeft,
                               NoCOA = i.NoCOA,
                               Inputbudget = i != null ? i.Inputbudget : 0,
                               PengadaanId = i != null ? i.PengadaanId : someguid,
                               BudgetType = a.Jenis,
                               Version = a.Version,
                               BudgetReserved = null
                           }).AsQueryable();
            
            length = 100;

            //search = search == null ? "" : search;
            DataTableBudgeting dtTable = new DataTableBudgeting();
            if (length > 0)
            {
                //if (month != null && !month.Trim().Equals(""))
                //{
                //    loadCOA =  loadCOA.Where(row => row.Month.Equals(month));
                //}


                dtTable.recordsTotal = loadCOA.Count();
                
                dtTable.recordsFiltered = loadCOA.Count();
                loadCOA = loadCOA.OrderBy(d => d.Branch).Skip(start).Take(length);

                dtTable.data = loadCOA.Select(a => new VWBudgeting
                {
                    Id = a.Id,
                    Branch = a.Branch,
                    Department = a.Department,
                    Description = a.Description,
                    COA = a.COA,
                    //Month = a.Month,
                    Year = a.Year,
                    BudgetAmount = a.BudgetAmount,
                    BudgetUsage = a.BudgetUsage,
                    BudgetLeft = a.BudgetLeft,
                    NoCOA = a.NoCOA,
                    Inputbudget = a != null ? a.Inputbudget : 0,
                    PengadaanId = a != null ? a.PengadaanId : someguid,
                    BudgetType = a.BudgetType,
                    Version = a.Version,
                    BudgetReserved = a.BudgetReserved
                }).ToList();
                
                foreach (var item in dtTable.data) {
                    if (item.BudgetReserved == null)
                    {
                        decimal? budresv = dataBOP.Where(z => z.Branch == item.Branch
                                               && z.Department == item.Department
                                               && z.NoCOA == item.COA
                                               && z.Year == item.Year
                                               && z.BudgetType == item.BudgetType).Select(x => x.BudgetReserved).Distinct().FirstOrDefault();
                        if (budresv != null)
                        {
                            item.BudgetReserved = budresv;
                        }
                        else
                        {
                            item.BudgetReserved = 0;
                        }
                    }
                }

            }
            return dtTable;
        }


        //public List<VWBudgeting> GetLoadCOAPengadaan(string branch, string department, string year, string month, string jenispembelanjaan, string pengadaanid, int start, int length)
        //{

        //    // ambil versi terakhir dari tabel Budget
        //    var budgetCoaList = (from budgetCoas in ctx.Budgetings
        //                         where budgetCoas.Branch.Equals(branch)
        //                             && budgetCoas.Department.Equals(department)
        //                             && budgetCoas.Year.Equals(year)
        //                             && budgetCoas.Jenis.Equals(jenispembelanjaan) // filter belum ada
        //                         select budgetCoas)
        //                        .GroupBy(x => x.Version)
        //                        .ToList()
        //                        .DefaultIfEmpty()
        //                        .Last();

        //    var maxVersion = budgetCoaList != null && budgetCoaList.First() != null ? budgetCoaList.First().Version : 0; // anggap versi = 0 jika sama sekali tidak ada data di budget

        //    var someguid = Guid.Parse(pengadaanid);

        //    var loadCOA = (from a in ctx.Budgetings
        //                   join e in (
        //                       from b in ctx.BudgetingPengadaanDetails
        //                       join c in ctx.BudgetingPengadaanHeaders on b.BudgetingPengadaanId equals c.Id into f
        //                       from g in f.DefaultIfEmpty()
        //                       where g.PengadaanId.Equals(someguid)
        //                       select new { NoCOA = b.NoCOA, Month = b.Month, Inputbudget = b.Input, PengadaanId = g.PengadaanId }
        //                   ) on a.COA equals e.NoCOA into h
        //                   from i in h.Where(x => x.Month.Equals(a.Month)).DefaultIfEmpty()
        //                   where a.Branch.Equals(branch)
        //                      && a.Department.Equals(department)
        //                      && a.Year.Equals(year)
        //                      && a.Jenis.Equals(jenispembelanjaan)
        //                      && a.Version == maxVersion // somehow cari cara untuk dapatkan versi yg aktif				
        //                   select new VWBudgeting
        //                   {
        //                       //Checked = i != null ? a.COA.Equals(i.NoCOA) : false,
        //                       Id = a.Id,
        //                       Branch = a.Branch,
        //                       Department = a.Department,
        //                       Description = a.Description,
        //                       COA = a.COA,
        //                       Month = a.Month,
        //                       Year = a.Year,
        //                       BudgetAmount = a.BudgetAmount,
        //                       BudgetUsage = a.BudgetUsage,
        //                       BudgetLeft = a.BudgetLeft,
        //                       NoCOA = i.NoCOA,
        //                       Inputbudget = i != null ? i.Inputbudget : 0,
        //                       PengadaanId = i != null ? i.PengadaanId : someguid,
        //                       BudgetType = a.Jenis,
        //                       Version = a.Version
        //                   }
        //        ).Distinct().ToList();

        //    if (month != null && !month.Trim().Equals(""))
        //    {
        //        return loadCOA.Where(row => row.Month.Equals(month)).ToList();
        //    }
        //    return loadCOA;
        //}

        public List<BudgetingPengadaanHeader> GetDataHeader(Guid PengadaanId, decimal TotalInput)
        {
            return ctx.BudgetingPengadaanHeaders.Where(x => x.PengadaanId == PengadaanId && x.TotalInput == TotalInput).ToList();
        }

        public Guid GetDataHeader(Guid PengadaanId)
        {
            var HeadId = ctx.BudgetingPengadaanHeaders.Where(x => x.PengadaanId == PengadaanId).Distinct().FirstOrDefault();
            return HeadId.Id;
        }

        public List<BudgetingPengadaanDetail> GetDataDetail(Guid HeaderId)
        {
            return ctx.BudgetingPengadaanDetails.Where(x => x.BudgetingPengadaanId == HeaderId).ToList();
        }

        public Guid SaveHeaderCOA(BudgetingPengadaanHeader d)
        {
            ctx.BudgetingPengadaanHeaders.Add(d);
            ctx.SaveChanges();
            return d.Id;
        }

        //public Guid UpdateHeaderCOA(BudgetingPengadaanHeader d)
        //{
        //    var update = ctx.BudgetingPengadaanHeaders.Where(x => x.PengadaanId == d.PengadaanId && x.TotalInput == d.TotalInput).FirstOrDefault();
        //    update.PengadaanId = d.PengadaanId;
        //    update.TotalInput = d.TotalInput;
        //    ctx.SaveChanges();
        //    return d.Id;
        //}

        public void RemoveHeaderCOA(Guid PengadaanId)
        {
            var removeheader = ctx.BudgetingPengadaanHeaders.Where(x => x.PengadaanId.Equals(PengadaanId)).FirstOrDefault();
            if (removeheader != null)
            {
                var removedetail = ctx.BudgetingPengadaanDetails.Where(x => x.BudgetingPengadaanId.Equals(removeheader.Id)).ToList();
                //ctx.BudgetingPengadaanDetails.RemoveRange(removeheader.BudgetingPengadaanDetails);
                ctx.BudgetingPengadaanDetails.RemoveRange(removedetail);
                ctx.BudgetingPengadaanHeaders.Remove(removeheader);
                ctx.SaveChanges();
            }

            
        }

        public void SaveDetailCOA(BudgetingPengadaanDetail d)
        {
            ctx.BudgetingPengadaanDetails.Add(d);
            ctx.SaveChanges();
        }

        //public void UpdateDetailCOA(BudgetingPengadaanDetail d)
        //{
        //    var update = ctx.BudgetingPengadaanDetails.Where(x => x.BudgetingPengadaanId == d.BudgetingPengadaanId && x.Branch == d.Branch && x.Department == d.Department && x.NoCOA == d.NoCOA && x.Input == d.Input && x.Month == d.Month).FirstOrDefault();
        //    update.BudgetingPengadaanId = d.BudgetingPengadaanId;
        //    update.Branch = d.Branch;
        //    update.Department = d.Department;
        //    update.NoCOA = d.NoCOA;
        //    update.Input = d.Input;
        //    update.Month = d.Month;
        //    ctx.SaveChanges();
        //}

        public VWDokumenBudget CekUpload()
        {

            //var oldVersion = ctx.DokumenBudgets.Where(z => z.Status == 4).OrderByDescending(x => x.Version).FirstOrDefault();
            var oldVersion = (from a in ctx.DokumenBudgets
                              where a.Status == 4
                              select new VWDokumenBudget
                              {
                                  CreateOn = a.CreateOn,
                                  Version = a.Version,
                                  ProcessId = a.ProcessId,
                                  IsIdle = 0
                              }).Distinct().OrderByDescending(x => x.CreateOn).FirstOrDefault();
            int? ver = 0;
            if (oldVersion != null) { ver = oldVersion.Version; }
            //var StatusUpload = ctx.DokumenBudgets.Where(z => z.Status == 2 && z.Version > oldVersion.Version || z.Status == 1 && z.Version > oldVersion.Version).OrderByDescending(x => x.Version).FirstOrDefault().Id;

            var StatusUpload = (from a in ctx.DokumenBudgets
                                join b in ctx.ReferenceDatas on a.Jenis equals b.Code
                                join c in ctx.ReferenceDatas on a.Year equals c.Code
                                where a.Status == 2 /*&& a.Version > ver*/ || a.Status == 1 /*&& a.Version > ver*/
                                select new VWDokumenBudget
                                {
                                    CreateOn = a.CreateOn,
                                    Version = a.Version,
                                    ProcessId = a.ProcessId,
                                    Jenis = b.LocalizedName,
                                    Year = c.LocalizedName,
                                    IsIdle = 1
                                }).Distinct().OrderByDescending(a => a.CreateOn).FirstOrDefault();

            if (StatusUpload != null)
            {
                return StatusUpload;
            }
            else
            {
                if (oldVersion != null)
                {
                    return oldVersion;
                }
                else
                {
                    VWDokumenBudget begining = new VWDokumenBudget { IsIdle = 2 };
                    return begining;
                }
            }
        }

        public Guid UpdateHeaderCOA(BudgetingPengadaanHeader d)
        {
            throw new NotImplementedException();
        }

        public void UpdateDetailCOA(BudgetingPengadaanDetail d)
        {
            throw new NotImplementedException();
        }

        public BudgetingPengadaanHeader SaveAllocation(BudgetingPengadaanHeader budgetAllocation)
        {
            ctx.BudgetingPengadaanHeaders.Add(budgetAllocation);
            ctx.SaveChanges();
            return budgetAllocation;
        }
    }
}


