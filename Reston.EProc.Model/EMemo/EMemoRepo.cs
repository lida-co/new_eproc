using Reston.Helper.Util;
using Reston.Pinata.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reston.Eproc.Model.EMemo
{
    public interface IEMemoRepo
    {
        DataTableEMemo List(string search, int start, int limit, string DocumentNo);
        ResultMessage save(EMemo ememo, Guid UserId);
        EMemo get(Guid Id);
        DataTableUser ListUser(string search, int start, int limit);
        JimbisContext GetContext();
    }

    public class EMemoRepo : IEMemoRepo
    {
        JimbisContext ctx;

        public EMemoRepo(JimbisContext j)
        {
            ctx = j;
            ctx.Configuration.LazyLoadingEnabled = true;
        }

        public EMemo get(Guid Id)
        {
            return ctx.EMemos.Find(Id);
        }

        public DataTableEMemo List(string search, int start, int limit, string DocumentNo)
        {
            search = search == null ? "" : search;
            DataTableEMemo dtTable = new DataTableEMemo();
            if (limit > 0)
            {
                var data = ctx.EMemos.AsQueryable();
                dtTable.recordsTotal = data.Count();
                dtTable.recordsFiltered = data.Count();
                data = data.OrderByDescending(d => d.CreatedDate).Skip(start).Take(limit);
                dtTable.data = data.Select(d => new VWEMemo
                {
                    Id = d.Id.ToString(),
                    Subject = d.Subject,
                    DocumentNo = d.DocumentNo,
                    InternalRefNo = d.InternalRefNo,
                    HPSAmount = d.HPSAmount,
                    HPSCurrency = d.HPSCurrency,
                    IsDraft = d.IsDraft,
                    CreatedDate = d.CreatedDate.ToString(),
                    CreatedBy = d.CreatedBy
                }).ToList();
            }
            return dtTable;
        }

        public DataTableUser ListUser(string search, int start, int limit)
        {
            search = search == null ? "" : search;
            DataTableUser dtTable = new DataTableUser();
            if (limit > 0)
            {
                var data = ctx.Users.AsQueryable();
                dtTable.recordsTotal = data.Count();
                dtTable.recordsFiltered = data.Count();
                dtTable.data = data.Select(d => new VWUser
                {
                    Id = d.Id,
                    UserName = d.UserName,
                    DisplayName = d.DisplayName,
                    Position = d.Position
                }).ToList();
            }
            return dtTable;
        }

        public ResultMessage save(EMemo ememo, Guid UserId)
        {
            throw new NotImplementedException();
        }

        public JimbisContext GetContext()
        {
            return ctx;
        }
    }
}
