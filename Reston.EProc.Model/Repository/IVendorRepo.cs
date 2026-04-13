using Reston.Pinata.Model.JimbisModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reston.Pinata.Model.Repository
{
    public interface IVendorRepo
    {
        Vendor GetVendor(int id);
        Vendor GetVendorByUser(Guid id);
        List<Vendor> GetVendors(ETipeVendor tipe, EStatusVendor status, int limit, string search);
        List<Vendor> GetAllVendor();
        List<Vendor> GetAllVendors();
        List<Vendor> GetAllVendorsWNon();
        //List<BankInfo> GetBankInfo(Guid Id, int Vendor_Id);
        int AddVendor(Vendor v);
        int CheckNomor(string no);
        void Save();
        int CheckNPWP(string npwp);
        DataTableViewVendor GetDataListVendorNonRegister(string search, int start, int length);
        void DeleteVendorNonRegister(int? id);
    }
}
