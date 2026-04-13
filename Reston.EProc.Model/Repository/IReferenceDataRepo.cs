using Reston.Pinata.Model.JimbisModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reston.Pinata.Model.Repository
{
    public interface IReferenceDataRepo
    {
        List<ReferenceData> GetData(string qualifier, string code);
        List<ReferenceData> GetData(string qualifier);
        List<ReferenceData> GetDataPengadaan(string qualifier, int id);
        //List<ReferenceData> GetPenilaians();
        ReferenceData GetDataById(int id);
        void Delete(ReferenceData d);
        void SaveData(ReferenceData d);

        /*==========================================================================================*/

        List<MasterBranch> GetBranch();
        List<MasterBranch> GetDataBranch(string kodebranch, string namabranch, int? provbranch);
        void SaveDataBranch(MasterBranch d);
        MasterBranch GetDataByIdBranch(int id);
        void DeleteBranch(MasterBranch d);

        /*==========================================================================================*/

        List<MasterDepartment> GetDepartment();
        List<MasterDepartment> GetDepartmentWithBranch(string branch);
        List<MasterDepartment> GetDataDepartment(string kodedepartment, string namadepartment);
        void SaveDataDepartment(MasterDepartment d);
        MasterDepartment GetDataByIdDepartment(int id);
        void DeleteDepartment(MasterDepartment d);

        /*==========================================================================================*/

        void SaveDataDepartmentBranch(MasterBranchDepartmentRelationship d);
        MasterBranchDepartmentRelationship GetDataByIdDepartmentBranch(int id);
        List<MasterBranchDepartmentRelationship> GetListDataByIdDepartmentBranch(int FK_Branch_Id, int FK_Department_Id);
        void DeleteDepartmentBranch(MasterBranchDepartmentRelationship d);

        string GetDataAdditional(string qualifier, string code);
    }
}
