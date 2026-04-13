using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reston.Pinata.WebService.ViewModels
{
    public class ReferenceDataViewModel
    {
        public ReferenceDataViewModel()
        {
            //this.VendorPerson;
        }
        public int id { get;set;}
        public string Code { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public string Str1 { get; set; }
        public int? Int1 { get; set; }
        public bool? Flag1 { get; set; }
    }

    public class MasterBranchViewModel
    {
        public MasterBranchViewModel()
        {
            //this.VendorPerson;
        }
        public int Branch_Id { get; set; }
        public string Branch_Code { get; set; }
        public string Branch_Name { get; set; }
        public Nullable<int> FK_Prov_Id { get; set; }
        public string Prov_Name { get; set; }
    }

    public class MasterDepartmentViewModel
    {
        public MasterDepartmentViewModel()
        {
            //this.VendorPerson;
        }
        public int Department_Id { get; set; }
        public string Department_Code { get; set; }
        public string Department_Name { get; set; }
    }

    public class MasterBranchDepartmentRelationshipViewModel
    {
        public MasterBranchDepartmentRelationshipViewModel()
        {
            //this.VendorPerson;
        }
        public int Id { get; set; }
        public string FK_Branch_Id { get; set; }
        public string FK_Department_Id { get; set; }
        public int Branch_Id { get; set; }
        public int Department_Id { get; set; }
    }
}
