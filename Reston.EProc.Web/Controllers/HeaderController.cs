using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Model.Helper;
using Newtonsoft.Json;
using Reston.Pinata.Model;
using Reston.Pinata.Model.Helper;
using Reston.Pinata.Model.JimbisModel;
using Reston.Pinata.Model.PengadaanRepository;
using Reston.Pinata.Model.PengadaanRepository.View;
using Reston.Pinata.WebService.Helper;
using Reston.Pinata.WebService.ViewModels;
using Reston.Eproc.Model.Monitoring.Repository;
using System.Diagnostics;

namespace Reston.Pinata.WebService.Controllers
{
    public class HeaderController : BaseController
    {
        private IPengadaanRepo _repository;
        internal ResultMessage result = new ResultMessage();

        public HeaderController()
        {
            _repository = new PengadaanRepo(new AppDbContext());
        }

        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_staff,
            IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_procurement_head,
            IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_end_user,
            IdLdapConstants.Roles.pRole_procurement_vendor, IdLdapConstants.App.Roles.IdLdapSuperAdminRole, IdLdapConstants.App.Roles.IdLdapUserRole)]
        public ResultMessage cekRole()
        {
            ResultMessage oResul = new ResultMessage();
            oResul.status = HttpStatusCode.OK;
            oResul.message = String.Join(", ", Roles().ToArray());
            return oResul;
        }

        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_staff,
            IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_procurement_head,
            IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_procurement_end_user,
            IdLdapConstants.Roles.pRole_procurement_vendor, IdLdapConstants.App.Roles.IdLdapSuperAdminRole, IdLdapConstants.App.Roles.IdLdapUserRole)]
        public HttpStatusCode cekLogin()
        {
            var user = UserId();
            return HttpStatusCode.OK;
        }

        private async Task<List<Menu>> cekdasboard(List<Menu> menu)
        {
            var data2 = _repository.ListPerpanjanganPKS(UserId());
            data2 = data2.Where(d => (long.Parse(d.TanggalSelesai.GetValueOrDefault(DateTime.Now).ToString("yyyyMMdd")) - long.Parse(DateTime.Now.ToString("yyyyMMdd"))) <= 180).ToList();
            int PKSPerpanjang = data2.Count();

            var dasbord = menu.Where(d => d.menu == "Dashboard").FirstOrDefault();
            var userApprover = await listUser(IdLdapConstants.Roles.pRole_approver);
            //var total = _repository.ListCount(UserId(), userApprover);
            var total = _repository.ListCount(UserId(), userApprover, PKSPerpanjang);

            if (dasbord != null)
            {
                dasbord.menu = dasbord.menu + " (" + total.TotalSeluruhPersetujuan + ")";
            }
            return menu;
        }

        [Authorize]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public async Task<List<Menu>> GetMenu()
        {

            var menu = new List<Menu>();

            // The logout menu
            menu.Add(new Menu
            {
                id = 0,
                menu = "Logout",
                url = "/api/header/signout",
                css = "fa fa-sign-out",
            });

            // Other menu based on roles
            var roles = Roles();
            System.Diagnostics.Debug.WriteLine("---------------------------------------------------------------------------------"+roles);
            var db = new AppDbContext();
            var roleMenu = db.RoleMenu.Where(d => roles.Contains(d.Role)).Select(d => new Menu()
            {
                id = d.Menu.Id,
                menu = d.Menu.menu,
                url = d.Menu.url,
                css = d.Menu.css,
                OrderId = d.Menu.OrderId
            }).Distinct().OrderBy(d => d.OrderId).ToList();
            menu.AddRange(roleMenu);

            // modify the menus further if needed...
            return await cekdasboard(menu);
        }

        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public void Signout()
        {
            Request.GetOwinContext().Authentication.SignOut();
            Redirect(IdLdapConstants.IDM.Url);
        }

        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public async Task<IHttpActionResult> GetUrl()
        {
            try
            {


                return Json(new { proc = IdLdapConstants.Proc.Url, idsrv = IdLdapConstants.IDM.Url });
            }
            catch
            {
                return Json("");
            }
        }

        [Authorize]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public async Task<IHttpActionResult> User()
        {
            try
            {
                Userx user = await userDetail(UserId().ToString());
                return Json(user);
            }
            catch
            {
                return Json("");
            }
        }



    }
}
