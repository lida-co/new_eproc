using Reston.Pinata.Model.JimbisModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reston.Pinata.Model.Repository
{
    public class ReferenceDataRepo : IReferenceDataRepo
    {
        AppDbContext ctx;
        public ReferenceDataRepo(AppDbContext j)
        {
            ctx = j;
        }

        public List<ReferenceData> GetData(string qualifier, string code){
            return ctx.ReferenceDatas.Where(x => x.Qualifier == qualifier && (code == null || x.Code == code)).OrderBy(d => d.LocalizedName).ToList();
        }

        public List<ReferenceData> GetData(string qualifier)
        {
            return ctx.ReferenceDatas.Where(x => x.Qualifier.Equals(qualifier)).ToList();
        }

        public List<ReferenceData> GetDataPengadaan(string qualifier, int id)
        {
            return ctx.ReferenceDatas.Where(x => x.Qualifier == qualifier && x.Id == id).ToList();
        }
        //public List<ReferenceData> GetPenilaians(string Id)
        //{
        //    var oPenilaian = ctx.ReferenceDatas.Where(d => d.Qualifier == RefDataQualifier).OrderByDescending(d => d.Id).FirstOrDefault();
        //    return ctx.ReferenceDatas.Where(x => x.Qualifier.Equals(Penilaian).ToList();
        //}

        public void SaveData(ReferenceData d) {
            ctx.ReferenceDatas.Add(d);
            ctx.SaveChanges();
        }

        public void Delete(ReferenceData d) {
            ctx.ReferenceDatas.Remove(d);
            ctx.SaveChanges();
        }

        public ReferenceData GetDataById(int id){
            return ctx.ReferenceDatas.Find(id);
        }

        /*=======================================================================*/

        public List<MasterBranch> GetBranch()
        {
            return ctx.MasterBranchs.ToList();
        }

        public List<MasterBranch> GetDataBranch(string kodebranch, string namabranch, int? provbranch)
        {
            return ctx.MasterBranchs.Where(x => x.Branch_Code == kodebranch && x.Branch_Name == namabranch).ToList();
        }

        public void SaveDataBranch(MasterBranch d)
        {
            ctx.MasterBranchs.Add(d);
            ctx.SaveChanges();
        }

        public MasterBranch GetDataByIdBranch(int id)
        {
            return ctx.MasterBranchs.Find(id);
        }

        public void DeleteBranch(MasterBranch d)
        {
            ctx.MasterBranchs.Remove(d);
            ctx.SaveChanges();
        }

        /*=======================================================================*/

        public List<MasterDepartment> GetDepartment()
        {
            return ctx.MasterDepartments.ToList();
        }

        public List<MasterDepartment> GetDepartmentWithBranch(string branch)
        {
            var Hasil = (from a in ctx.MasterDepartments
                         join b in ctx.MasterBranchDepartmentRelationships on a.Department_Id equals b.FK_Department_Id
                         join c in ctx.MasterBranchs on b.FK_Branch_Id equals c.Branch_Id
                         where c.Branch_Name.Equals(branch)
                         select a);
            return Hasil.ToList();
        }

        public List<MasterDepartment> GetDataDepartment(string kodedepartment, string namadepartment)
        {
            return ctx.MasterDepartments.Where(x => x.Department_Code == kodedepartment && x.Department_Name == namadepartment).ToList();
        }

        public void SaveDataDepartment(MasterDepartment d)
        {
            ctx.MasterDepartments.Add(d);
            ctx.SaveChanges();
        }

        public MasterDepartment GetDataByIdDepartment(int id)
        {
            return ctx.MasterDepartments.Find(id);
        }

        public void DeleteDepartment(MasterDepartment d)
        {
            ctx.MasterDepartments.Remove(d);
            ctx.SaveChanges();
        }

        /*=======================================================================*/

        public void SaveDataDepartmentBranch(MasterBranchDepartmentRelationship d)
        {
            ctx.MasterBranchDepartmentRelationships.Add(d);
            ctx.SaveChanges();
        }

        public MasterBranchDepartmentRelationship GetDataByIdDepartmentBranch(int id)
        {
            return ctx.MasterBranchDepartmentRelationships.Find(id);
        }

        public void DeleteDepartmentBranch(MasterBranchDepartmentRelationship d)
        {
            ctx.MasterBranchDepartmentRelationships.Remove(d);
            ctx.SaveChanges();
        }

        public List<MasterBranchDepartmentRelationship> GetListDataByIdDepartmentBranch(int FK_Branch_Id, int FK_Department_Id)
        {
            return ctx.MasterBranchDepartmentRelationships.Where(x => x.FK_Branch_Id == FK_Branch_Id && x.FK_Department_Id == FK_Department_Id).ToList();
        }


        public string GetDataAdditional(string qualifier, string code)
        {
            var a = "";
            a = ctx.ReferenceDatas.Where(x => x.Qualifier == qualifier && x.Code == code).FirstOrDefault() == null ? "" : ctx.ReferenceDatas.Where(x => x.Qualifier == qualifier && (code == null || x.Code == code)).FirstOrDefault().LocalizedName;

            return a;
        }
    }
}
