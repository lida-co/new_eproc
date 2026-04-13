using Microsoft.Owin.FileSystems;
using Reston.Pinata.Model;
using Reston.Pinata.Model.JimbisModel;
using Reston.Pinata.Model.Repository;
using Reston.Pinata.WebService.ViewModels;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net.Http.Headers;
using System.Net;
using Model.Helper;
using Reston.Pinata.WebService.Helper;
using System.Security.Claims;
using System.Threading;
using Reston.EProc.Web.ViewModels;
using Reston.Eproc.Model.Ext;
using System.Web;
using Reston.Pinata.WebService;

namespace Reston.EProc.Web.Controllers
{
    public class VendorExtController : BaseController
    {
        private IVendorExtRepo _repository;
        //AppDbContext ctx;
        public VendorExtController()
        {
            _repository = new VendorExtRepo(new AppDbContext());
        }

        public VendorExtController(VendorExtRepo repository)
        {
            _repository = repository;
        }

        [ApiAuthorize(new string[] { IdLdapConstants.Roles.pRole_procurement_vendor, IdLdapConstants.Roles.pRole_procurement_vendor })]
        [HttpPost]
        public int EditVendor([FromBody] VendorExtViewModelJaws model)
        {
            var username = CurrentUser.UserName;
            var userId = CurrentUser.Subject;
            var roles = CurrentUser.Roles;
            bool enforceDataOwnershipPolicy = true;

            // Procurement staffs is not the owner of vendor data, but is allowed to edit
            if (roles != null && roles.Contains("procurement_staff")) 
            {
                enforceDataOwnershipPolicy = false;
            }

            VendorExtViewModelJaws v = _repository.EditVendorExt(model, username, enforceDataOwnershipPolicy);

            //_repository.Save();

            //return model.id;
            return 1;
        }


    }
}
