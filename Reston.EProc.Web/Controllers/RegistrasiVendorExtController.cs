using Model.Helper;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using Reston.Eproc.Model.Ext;
using Reston.EProc.Web.ViewModels;
using Reston.Pinata.Model;
using Reston.Pinata.Model.JimbisModel;
using Reston.Pinata.WebService.Helper;
using Reston.Pinata.WebService.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Reston.Pinata.WebService
{
    public class RegistrasiVendorExtController : BaseController
    {
        private IRegVendorExtRepo _repository;
        private string FILE_TEMP_PATH = System.Configuration.ConfigurationManager.AppSettings["FILE_UPLOAD_TEMP"]; //config soon
        private string FILE_VENDOR_PATH = System.Configuration.ConfigurationManager.AppSettings["FILE_UPLOAD_REG"]; //--^

        //fchr
        private string SanitizeInput(string input)
        {
            return string.IsNullOrEmpty(input)
                ? input
                : System.Web.HttpUtility.HtmlEncode(input.Trim());
        }

        private bool IsValidBase64(string base64)
        {
            if (string.IsNullOrWhiteSpace(base64))
                return false;

            try
            {
                Convert.FromBase64String(base64);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private byte[] SafeBase64Decode(string base64)
        {
            try
            {
                return Convert.FromBase64String(base64);
            }
            catch
            {
                throw new Exception("Invalid file format");
            }
        }

        private bool IsAllowedFile(string base64)
        {
            if (string.IsNullOrEmpty(base64)) return false;

            foreach (var sign in base64Signatures)
            {
                if (base64.ToLower().Contains(sign))
                    return true;
            }
            return false;
        }
        //end fchr

        private string[] allowedContentTypes = {
              "application/pdf"
            , "application/x-pdf"
            , "image/jpeg"
            , "image/png"
        };

        private string[] allowedFileExtentions = {
              "pdf"
            , "jpg"
            , "jpeg"
            , "png"
        };

        private string[] base64Signatures = {
              "jvberi0"
            , "ivborw0kggo"
            , "/9j/"
        };

        private bool isAllowedSignature = false;

        public RegistrasiVendorExtController()
        {
            _repository = new RegVendorExtRepo(new AppDbContext());
        }

        public RegistrasiVendorExtController(RegVendorExtRepo repository)
        {
            _repository = repository;
        }

        public IEnumerable<string> Get()
        {
            return new[] { "Vendor is so", "more of it", "more" };
        }

        [HttpPost]
        public string AddVendorExt([FromBody] VendorExtViewModel model)
        {
            var trans = _repository.BeginTransaction();

            try
            {
                if (model == null)
                    throw new Exception("Invalid request");

                Int64 nomor = Int64.Parse(GenerateNomorVendor((int)model.TipeVendor));

                RegVendor v = new RegVendor
                {
                    NoPengajuan = nomor.ToString(),
                    TipeVendor = (ETipeVendor)model.TipeVendor,
                    Nama = SanitizeInput(model.Nama),
                    Alamat = SanitizeInput(model.Alamat),
                    Provinsi = SanitizeInput(model.Provinsi),
                    Kota = SanitizeInput(model.Kota),
                    KodePos = SanitizeInput(model.KodePos),
                    Email = SanitizeInput(model.Email),
                    Website = SanitizeInput(model.Website),
                    Telepon = SanitizeInput(model.Telepon),
                    StatusAkhir = EStatusVendor.NEW
                };

                #region Vendor Person
                if (model.VendorPersonExt != null)
                {
                    List<RegVendorPerson> lvpOri = new List<RegVendorPerson>();
                    foreach (var vpvm in model.VendorPersonExt)
                    {
                        if (!string.IsNullOrWhiteSpace(vpvm.Name))
                        {
                            lvpOri.Add(new RegVendorPerson
                            {
                                Nama = SanitizeInput(vpvm.Name),
                                Jabatan = SanitizeInput(vpvm.Position),
                                Email = SanitizeInput(vpvm.ContactEmail),
                                Telepon = SanitizeInput(vpvm.ContactPhone),
                                Active = true
                            });
                        }
                    }
                    v.RegVendorPerson = lvpOri;
                }
                #endregion

                #region Vendor Bank
                if (model.VendorBankInfoExt != null)
                {
                    RegBankInfo biOri = new RegBankInfo
                    {
                        NamaBank = SanitizeInput(model.VendorBankInfoExt.BankCode),
                        Cabang = SanitizeInput(model.VendorBankInfoExt.Branch),
                        NomorRekening = SanitizeInput(model.VendorBankInfoExt.AccNumber),
                        NamaRekening = SanitizeInput(model.VendorBankInfoExt.AccName),
                        Active = true
                    };

                    if (!string.IsNullOrWhiteSpace(biOri.NamaBank))
                        v.RegBankInfo = new List<RegBankInfo>() { biOri };
                }
                #endregion

                var regvendorId = _repository.AddVendorOri(v);

                var guidvenregext = Guid.NewGuid();

                #region Vendor Reg Ext
                if (model.VendorRegExt != null)
                {
                    var vRegExt = new RegVendorExt
                    {
                        Id = guidvenregext,
                        RegVendorId = regvendorId,
                        JenisVendor = model.TipeVendor.ToString(),
                        KategoriVendor = SanitizeInput(model.VendorRegExt.KategoriVendor),
                        BentukBadanUsaha = SanitizeInput(model.VendorRegExt.BentukBadanUsaha),
                        StatusPerusahaan = SanitizeInput(model.VendorRegExt.StatusPerusahaan),
                        EstablishedDate = model.VendorRegExt.EstablishedDate,
                        KategoriUsaha = SanitizeInput(model.VendorRegExt.KategoriUsaha),
                        CountryCode = SanitizeInput(model.VendorRegExt.CountryCode),
                        FirstLevelDivisionCode = SanitizeInput(model.FirstLevelDivisionCode),
                        SecondLevelDivisionCode = SanitizeInput(model.SecondLevelDivisionCode),
                        ThirdLevelDivisionCode = SanitizeInput(model.ThirdLevelDivisionCode),
                        PostalCode = SanitizeInput(model.VendorRegExt.PostalCode),
                        Fax = SanitizeInput(model.VendorRegExt.Fax),
                        WorkUnitCode = SanitizeInput(model.VendorRegExt.WorkUnitCode),
                        DirPersonGiidNo = SanitizeInput(model.VendorRegExt.DirPersonGiidNo),
                        DirPersonName = SanitizeInput(model.VendorRegExt.DirPersonName),
                        DirPersonPosition = SanitizeInput(model.VendorRegExt.DirPersonPosition),
                        DirPersonReligionCode = SanitizeInput(model.VendorRegExt.DirPersonReligionCode),
                        DirPersonBirthDay = model.VendorRegExt.DirPersonBirthDay
                    };

                    if (model.VendorRegExt.SegKelompokUsahaCode != null && model.VendorRegExt.SegKelompokUsahaCode.Any())
                    {
                        vRegExt.SegKelompokUsahaCode = string.Join(",", model.VendorRegExt.SegKelompokUsahaCode);
                    }

                    _repository.AddVendorExt(vRegExt);
                }
                #endregion

                #region Helper File Upload
                Func<string, byte[]> safeFile = (base64) =>
                {
                    if (!IsValidBase64(base64) || !IsAllowedFile(base64))
                        throw new Exception("File not allowed");

                    return SafeBase64Decode(base64);
                };
                #endregion

                #region Contoh File (NPWP)
                if (model.NPWP != null && model.NPWP.Nomor != null)
                {
                    var guiddocext = Guid.NewGuid();

                    RegDocumentExt detaildoc = new RegDocumentExt()
                    {
                        Id = guiddocext,
                        Nomor = SanitizeInput(model.NPWP.Nomor),
                        RegVendorExtId = guidvenregext,
                        TipeDokumen = (int)EDocumentType.NPWP
                    };
                    _repository.AddVendorDocumentExt(detaildoc);

                    if (model.NPWP.base64 != null)
                    {
                        RegDocumentImageExt npwpdoc = new RegDocumentImageExt()
                        {
                            Id = Guid.NewGuid(),
                            Content = safeFile(model.NPWP.base64),
                            FileName = SanitizeInput(model.NPWP.FileName),
                            ContentType = SanitizeInput(model.NPWP.ContentType),
                            RegDocumenExtId = guiddocext
                        };
                        _repository.AddVendorDocumentImageExt(npwpdoc);
                    }
                }
                #endregion

                #region Riwayat
                RegRiwayatPengajuanVendor rp = new RegRiwayatPengajuanVendor()
                {
                    Komentar = "Pengajuan rekanan BARU.",
                    Status = EStatusVendor.NEW,
                    Urutan = 0,
                    Metode = EMetodeVerifikasiVendor.NONE,
                    Waktu = DateTime.Now
                };

                v.RegRiwayatPengajuanVendor = new List<RegRiwayatPengajuanVendor>() { rp };
                #endregion

                _repository.Save();
                trans.Commit();

                return v.NoPengajuan;
            }
            catch (Exception e)
            {
                trans.Rollback();
                throw;
            }
        }

        public static byte[] GetBytes(Stream stream)
        {
            var bytes = new byte[stream.Length];
            stream.Seek(0, SeekOrigin.Begin);
            stream.ReadAsync(bytes, 0, bytes.Length);
            stream.Dispose();
            return bytes;
        }

        private string GenerateNomorVendor(int tipe)
        {
            string no = "";
            DateTime d = DateTime.Now;
            no = ("" + d.Year).Substring(2, 2) + (d.Month < 10 ? "0" + d.Month : "" + d.Month) + "0" + tipe;
            string ctrn = "000" + _repository.CheckNomor(no);
            no += ctrn.Substring(ctrn.Length - 4, 4);
            return no;
        }

        [HttpGet]
        public List<ReferenceDataViewModel> GetAllProvinsi()
        {
            return _repository.GetData("DUKCAPILPROV").Select(x => new ReferenceDataViewModel()
            {
                id = x.Id,
                Code = x.Code,
                Name = x.LocalizedName,
                Desc = x.LocalizedDesc,
                Str1 = x.StringAttr1,
                Int1 = x.IntAttr1,
                Flag1 = x.FlagAttr1
            }).ToList();
        }

        [HttpGet]
        public List<ReferenceDataViewModel> GetListKota(string q)
        {
            var listKota = new List<ReferenceDataViewModel>();
            if (q != "")
            {
                listKota = _repository.GetListDukcapil("DUKCAPILKOTA", q).Select(x => new ReferenceDataViewModel()
                {
                    id = x.Id,
                    Code = x.Code,
                    Name = x.LocalizedName,
                    Desc = x.LocalizedDesc,
                    Str1 = x.StringAttr1,
                    Int1 = x.IntAttr1,
                    Flag1 = x.FlagAttr1
                }).ToList();
            }
            return listKota;
        }

        [HttpGet]
        public List<ReferenceDataViewModel> GetListKecamatan(string q)
        {
            var listKota = new List<ReferenceDataViewModel>();
            if (q != "")
            {
                listKota = _repository.GetListDukcapil("DUKCAPILKECAMATAN", q).Select(x => new ReferenceDataViewModel()
                {
                    id = x.Id,
                    Code = x.Code,
                    Name = x.LocalizedName,
                    Desc = x.LocalizedDesc,
                    Str1 = x.StringAttr1,
                    Int1 = x.IntAttr1,
                    Flag1 = x.FlagAttr1
                }).ToList();
            }
            return listKota;
        }

        [HttpGet]
        public List<ReferenceDataViewModel> GetListKelurahan(string q)
        {
            var listKota = new List<ReferenceDataViewModel>();
            if (q != "")
            {
                listKota = _repository.GetListDukcapil("DUKCAPILKELURAHAN", q).Select(x => new ReferenceDataViewModel()
                {
                    id = x.Id,
                    Code = x.Code,
                    Name = x.LocalizedName,
                    Desc = x.LocalizedDesc,
                    Str1 = x.StringAttr1,
                    Int1 = x.IntAttr1,
                    Flag1 = x.FlagAttr1
                }).ToList();
            }
            return listKota;
        }

        [HttpGet]
        public List<ReferenceDataViewModel> GetListCountry()
        {
            return _repository.GetData(RefDataQualifier.COUNTRY).Select(x => new ReferenceDataViewModel()
            {
                id = x.Id,
                Code = x.Code,
                Name = x.LocalizedName,
                Desc = x.LocalizedDesc,
                Str1 = x.StringAttr1,
                Int1 = x.IntAttr1,
                Flag1 = x.FlagAttr1
            }).ToList();
        }

        [HttpGet]
        public List<ReferenceDataViewModel> GetListSegBidangUsahaCode()
        {
            return _repository.GetData("SegBidangUsahaCode").Select(x => new ReferenceDataViewModel()
            {
                id = x.Id,
                Code = x.Code,
                Name = x.LocalizedName,
                Desc = x.LocalizedDesc,
                Str1 = x.StringAttr1,
                Int1 = x.IntAttr1,
                Flag1 = x.FlagAttr1
            }).ToList();
        }

        [HttpGet]
        public List<ReferenceDataViewModel> GetListSegKlmpkUsahaCode(string q)
        {
            return _repository.GetListSegmentasi("segKlmpkUsahaCode", q).Select(x => new ReferenceDataViewModel()
            {
                id = x.Id,
                Code = x.Code,
                Name = x.LocalizedName,
                Desc = x.LocalizedDesc,
                Str1 = x.StringAttr1,
                Int1 = x.IntAttr1,
                Flag1 = x.FlagAttr1
            }).ToList();
        }

        [HttpGet]
        public List<ReferenceDataViewModel> GetListSegSubBidangUsahaCode(string q)
        {
            return _repository.GetListSegmentasiSubBidangUsaha("SegSubBidangUsahaCode", q).Select(x => new ReferenceDataViewModel()
            {
                id = x.Id,
                Code = x.Code,
                Name = x.LocalizedName,
                Desc = x.LocalizedDesc,
                Str1 = x.StringAttr1,
                Int1 = x.IntAttr1,
                Flag1 = x.FlagAttr1
            }).ToList();
        }

        [HttpGet]
        public List<ReferenceDataViewModel> GetDataSegSubBidangUsahaCode(string q)
        {
            return _repository.GetDataSegmentasiSubBidangUsahaCode("SegSubBidangUsahaCode", q).Select(x => new ReferenceDataViewModel()
            {
                id = x.Id,
                Code = x.Code,
                Name = x.LocalizedName,
                Desc = x.LocalizedDesc,
                Str1 = x.StringAttr1,
                Int1 = x.IntAttr1,
                Flag1 = x.FlagAttr1
            }).ToList();
        }

        [HttpGet]
        public List<ReferenceDataViewModel> GetkodeposCode(string q, string r, string s, string t)
        {
            return _repository.GetkodeposCode("DUKCAPILPOS", q, r, s, t).Select(x =>
            new ReferenceDataViewModel()
            {
                id = x.Id,
                Code = x.Code,
                Name = x.LocalizedName,
                Desc = x.LocalizedDesc,
                Str1 = x.StringAttr1,
                Int1 = x.IntAttr1,
                Flag1 = x.FlagAttr1
            }).ToList();
        }

        [HttpGet]
        public List<ReferenceDataViewModel> GetListSegKualifikasiGradeCode()
        {
            return _repository.GetData("SegKualifikasiGradeCode").Select(x => new ReferenceDataViewModel()
            {
                id = x.Id,
                Code = x.Code,
                Name = x.LocalizedName,
                Desc = x.LocalizedDesc,
                Str1 = x.StringAttr1,
                Int1 = x.IntAttr1,
                Flag1 = x.FlagAttr1
            }).ToList();
        }

        public List<ReferenceDataViewModel> GetEducation()
        {
            return _repository.GetData("TINGKAT_PENDIDIKAN").Select(x => new ReferenceDataViewModel()
            {
                id = x.Id,
                Code = x.Code,
                Name = x.LocalizedName,
                Desc = x.LocalizedDesc,
                Str1 = x.StringAttr1,
                Int1 = x.IntAttr1,
                Flag1 = x.FlagAttr1
            }).OrderBy(x => x.Str1).ToList();
        }

        public List<ReferenceDataViewModel> GetBusinessCategory()
        {
            return _repository.GetData("KATEGORI_USAHA").Select(x => new ReferenceDataViewModel()
            {
                id = x.Id,
                Code = x.Code,
                Name = x.LocalizedName,
                Desc = x.LocalizedDesc,
                Str1 = x.StringAttr1,
                Int1 = x.IntAttr1,
                Flag1 = x.FlagAttr1
            }).OrderBy(x => x.Str1).ToList();
        }

        public List<ReferenceDataViewModel> GetMasterData(string q)
        {
            return _repository.GetData(q).Select(x => new ReferenceDataViewModel()
            {
                id = x.Id,
                Code = x.Code,
                Name = x.LocalizedName,
                Desc = x.LocalizedDesc,
                Str1 = x.StringAttr1,
                Int1 = x.IntAttr1,
                Flag1 = x.FlagAttr1
            }).OrderBy(x => x.Code).ToList();
        }

        [HttpPost]
        public async Task<IHttpActionResult> UploadFile(string tipe)
        {
            //bool isSavedSuccessfully = true;
            string filePathSave = FILE_TEMP_PATH;
            var s = await Request.Content.ReadAsStreamAsync();
            var provider = new MultipartMemoryStreamProvider();
            Guid tempFileName = Guid.NewGuid();
            string contentType = "";
            string fileExtension = "";
            string base64String = string.Empty;
            string filename = string.Empty;

            await Request.Content.ReadAsMultipartAsync(provider);
            foreach (var file in provider.Contents)
            {
                filename = file.Headers.ContentDisposition.FileName.Trim('\"');
                contentType = file.Headers.ContentType.ToString();
                fileExtension = filename.Substring(filename.IndexOf(".") + 1, filename.Length - filename.IndexOf(".") - 1);
                byte[] buffer = await file.ReadAsByteArrayAsync();

                // sanitize the uploaded file content
                // 1. Cannot have unsanctioned content type
                if (string.IsNullOrEmpty(contentType) || !allowedContentTypes.Contains(contentType.ToLower()))
                {
                    throw new Exception("Invalid content uploaded.");
                }
                // 2. Cannot have unsactioned file extension
                if (string.IsNullOrEmpty(fileExtension) || !allowedFileExtentions.Contains(fileExtension.ToLower()))
                {
                    throw new Exception("Invalid content uploaded.");
                }
                // 3. Try to heuristically sniff the content. 
                string tmpContentData = Encoding.UTF8.GetString(buffer);
                if (tmpContentData.IndexOf("<html", StringComparison.OrdinalIgnoreCase) >= 0
                    || tmpContentData.IndexOf("<form", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    throw new Exception("Invalid content uploaded.");
                }

                base64String = Convert.ToBase64String(buffer);

                //filePathSave += tempFileName.ToString() + "." + fileExtension;
                //var uploadPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase; //new PhysicalFileSystem(@"..\Reston.Pinata\WebService\Upload\Vendor\Dokumen\");

                //try
                //{
                //    FileStream fs = new FileStream(uploadPath.ToString() + filePathSave, FileMode.CreateNew);
                //    fs.Close();
                //    //isSavedSuccessfully = true;
                //}
                //catch (Exception e)
                //{
                //    return InternalServerError();
                //}
            }
            return Json(new { base64 = base64String, ContentType = contentType, FileName = filename });
        }

        [HttpPost]
        public async Task<IHttpActionResult> UploadFileMultipleReturn(string tipe)
        {
            //bool isSavedSuccessfully = true;
            string filePathSave = FILE_TEMP_PATH;
            var s = await Request.Content.ReadAsStreamAsync();
            var provider = new MultipartMemoryStreamProvider();
            Guid tempFileName = Guid.NewGuid();
            string contentType = "";
            string fileExtension = "";
            string base64String = string.Empty;
            string filename = string.Empty;

            await Request.Content.ReadAsMultipartAsync(provider);
            foreach (var file in provider.Contents)
            {
                filename = file.Headers.ContentDisposition.FileName.Trim('\"');
                contentType = file.Headers.ContentType.ToString();
                fileExtension = filename.Substring(filename.IndexOf(".") + 1, filename.Length - filename.IndexOf(".") - 1);
                byte[] buffer = await file.ReadAsByteArrayAsync();
                base64String = Convert.ToBase64String(buffer, 0, buffer.Length);

            }
            if (tipe == "Equipbase64")
                return Json(new { base64 = base64String, base64a = "", ContentType = contentType, FileName = filename, ContentTypea = "", FileNamea = "" });
            else
                return Json(new { base64 = "", base64a = base64String, ContentType = "", FileName = "", ContentTypea = contentType, FileNamea = filename });
            //return Json(new { base64 = base64String, ContentType = contentType, FileName = filename });
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_staff)]
        [HttpGet]
        public async Task<int> VerifyVendorExt(string no)
        {
            RegVendor rv = _repository.GetRegVendor(no);
            RegVendorExt rvExt = _repository.GetRegVendorExt(no);
            RegRiwayatPengajuanVendor rpp = rv.RegRiwayatPengajuanVendor.Where(x => x.Metode == EMetodeVerifikasiVendor.DESK).FirstOrDefault();
            //RegRiwayatPengajuanVendor rpp = _repository.GetRegRiwayatPengajuanVendor(rvExt.RegVendorId);
            if (rv != null && rpp != null)
            {
                var newRegPengVen = new RegRiwayatPengajuanVendor();
                rv.StatusAkhir = EStatusVendor.VERIFIED;
                RegRiwayatPengajuanVendor rp = new RegRiwayatPengajuanVendor()
                {
                    Metode = EMetodeVerifikasiVendor.NONE,
                    Status = EStatusVendor.VERIFIED,
                    Komentar = "Pengajuan rekanan telah TERVERIFIKASI, oleh " + CurrentUser.UserName,
                    Waktu = DateTime.Now,

                };
                rv.RegRiwayatPengajuanVendor.Add(rp);
                _repository.Save();

                // Creating User Account?
                Guid iduser = Guid.NewGuid();
                string rand = CreatePassword(8);
                string uname = await TestCreateUser(rv.NoPengajuan, rand, rv.Nama, iduser);
                //sending email notification
                try
                {
                    Reston.Pinata.WebService.Helper.Mailer.sendText(rv.Nama, rv.Email,
                        System.Configuration.ConfigurationManager.AppSettings["MAIL_VENDOR_VERIFICATION_TITLE"],
                        String.Format(System.Configuration.ConfigurationManager.AppSettings["MAIL_VENDOR_VERIFICATION_BODY"].ToString(), rv.Nama, uname, rand, IdLdapConstants.IDM.Url));
                }
                catch (Exception e)
                {

                }

                // Creating Vendor
                return RegisterVerifiedVendor(rv, iduser.ToString());
            }
            return -1;//not found
            //return "tak ada";
        }

        private string CreatePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string number = "1234567890";
            System.Text.StringBuilder res = new System.Text.StringBuilder();
            Random rnd = new Random();
            int i = length / 4;
            while (0 < i--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
                res.Append(lower[rnd.Next(lower.Length)]);
                res.Append(upper[rnd.Next(upper.Length)]);
                res.Append(number[rnd.Next(number.Length)]);
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString().Substring(0, length);
        }

        [Authorize]
        private async Task<string> TestCreateUser(string username, string password, string displayname, Guid guid)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var user = new { UserName = username, DisplayName = displayname, NewPassword = password, IsLdapUser = false, guid = guid };
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(user);
            HttpContent content = new StringContent(json);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            //var stringContent = new StringContent(user.ToString(), System.Text.Encoding.UTF8, "application/json");
            //stringContent.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");
            HttpResponseMessage reply = await client.PostAsync(
                    string.Format("{0}/{1}", IdLdapConstants.IDM.Url, "admin/NewVendorUser"), content);
            if (reply.IsSuccessStatusCode)
            {
                return reply.Content.ReadAsStringAsync().Result;
            };
            return "";
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_staff)]
        [Authorize]
        private int RegisterVerifiedVendor(RegVendor rv, string owner)
        {
            //RegVendor rv = _repository.GetVendor(no);
            try
            {
                if (rv != null && rv.StatusAkhir == EStatusVendor.VERIFIED)
                {
                    Vendor v = new Vendor()
                    {
                        NomorVendor = rv.NoPengajuan,
                        Nama = rv.Nama,
                        Alamat = rv.Alamat,
                        Provinsi = rv.Provinsi,
                        Kota = rv.Kota,
                        Email = rv.Email,
                        KodePos = rv.KodePos,
                        Website = rv.Website,
                        Telepon = rv.Telepon,
                        StatusAkhir = rv.StatusAkhir,
                        TipeVendor = rv.TipeVendor
                    };
                    if (rv.RegVendorPerson != null)
                    {
                        List<VendorPerson> lvp = new List<VendorPerson>();
                        foreach (RegVendorPerson rvp in rv.RegVendorPerson)
                        {
                            VendorPerson vp = new VendorPerson()
                            {
                                Nama = rvp.Nama,
                                Active = rvp.Active,
                                Email = rvp.Email,
                                Jabatan = rvp.Jabatan,
                                Telepon = rvp.Telepon
                            };
                            lvp.Add(vp);
                        }
                        v.VendorPerson = lvp;
                    }
                    if (rv.RegBankInfo != null)
                    {
                        List<BankInfo> lvp = new List<BankInfo>();
                        foreach (RegBankInfo rvp in rv.RegBankInfo)
                        {
                            BankInfo vp = new BankInfo()
                            {
                                NamaBank = rvp.NamaBank,
                                Active = rvp.Active,
                                Cabang = rvp.Cabang,
                                NamaRekening = rvp.NamaRekening,
                                NomorRekening = rvp.NomorRekening
                            };
                            lvp.Add(vp);
                        }
                        v.BankInfo = lvp;
                    }

                    v.Owner = Guid.Parse(owner);
                    int ias = _repository.RegisterVerifiedVendor(v);
                    int iasEXT = _repository.RegisterVerifiedVendorEXT(v.NomorVendor); //Untuk replicate EXT from reg to normal
                    return ias;
                    //return "harusnya sih berhasil, nih id nya:"+ias;
                }
            }
            catch (Exception e)
            {
                //return e.ToString();
            }
            return 0; //"vendor null or wrong status lol";
        }

    }
}
