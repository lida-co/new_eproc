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
using Novacode;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Drawing;
using Microsoft.Reporting.WebForms;
using System.Reflection;
using System.Xml.Linq;
using Reston.Helper.Repository;
using NLog;

namespace Reston.Pinata.WebService.Controllers
{
    public class ReportController : BaseController
    {
        private static Logger _log = LogManager.GetCurrentClassLogger();

        private IPengadaanRepo _repository;
        private IRksRepo _rksrepo;
        private IWorkflowRepository _workflowrepo;
        internal ResultMessage result = new ResultMessage();
        private string FILE_TEMP_PATH = System.Configuration.ConfigurationManager.AppSettings["FILE_UPLOAD_TEMP"];
        private string FILE_DOKUMEN_PATH = System.Configuration.ConfigurationManager.AppSettings["FILE_UPLOAD_DOC"];
        private string FILE_REPORT_PATH = System.Configuration.ConfigurationManager.AppSettings["FILE_REPORT_PATH"];
        public ReportController()
        {
            _repository = new PengadaanRepo(new AppDbContext());
            _rksrepo = new RksRepo(new AppDbContext());
            _workflowrepo = new WorkflowRepository(new Reston.Helper.HelperContext());
        }

        public ReportController(PengadaanRepo repository)
        {
            _repository = repository;
        }

        [Authorize]
        public async Task<IHttpActionResult> UploadFile(string tipe, Guid id)
        {
            var uploadPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            bool isSavedSuccessfully = true;
            string filePathSave = FILE_DOKUMEN_PATH;//+id ;
            string fileName = tipe;
            if (Directory.Exists(uploadPath + filePathSave) == false)
            {
                Directory.CreateDirectory(uploadPath + filePathSave);
            }

            var s = await Request.Content.ReadAsStreamAsync();
            var provider = new MultipartMemoryStreamProvider();

            await Request.Content.ReadAsMultipartAsync(provider);
            string contentType = "";
            Guid newGuid = Guid.NewGuid();
            long sizeFile = 0;
            foreach (var file in provider.Contents)
            {
                string filename = file.Headers.ContentDisposition.FileName.Trim('\"');
                string extension = filename.Substring(filename.IndexOf(".") + 1, filename.Length - filename.IndexOf(".") - 1);
                byte[] buffer = await file.ReadAsByteArrayAsync();
                contentType = file.Headers.ContentType.ToString();
                sizeFile = buffer.Length;
                filePathSave += tipe + "-" + newGuid.ToString() + "." + extension;
                fileName += "-" + newGuid.ToString() + "." + extension;
                // var uploadPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase; //new PhysicalFileSystem(@"..\Reston.Pinata\WebService\Upload\Vendor\Dokumen\");

                try
                {
                    FileStream fs = new FileStream(uploadPath.ToString() + filePathSave, FileMode.CreateNew);
                    await fs.WriteAsync(buffer, 0, buffer.Length);

                    fs.Close();

                    isSavedSuccessfully = true;
                }
                catch (Exception e)
                {
                    return InternalServerError();
                }
            }
            Guid DokumenId = Guid.NewGuid();
            TipeBerkas t = (TipeBerkas)Enum.Parse(typeof(TipeBerkas), tipe);
            DokumenPengadaan dokumen = new DokumenPengadaan
            {
                File = fileName,
                Tipe = t,
                ContentType = contentType,
                PengadaanId = id,
                SizeFile = sizeFile
            };

            if (isSavedSuccessfully)
            {
                try
                {
                    DokumenPengadaan dokumenUpdate = _repository.saveDokumenPengadaan(dokumen, UserId());
                    if (t == TipeBerkas.BeritaAcaraAanwijzing)
                    {
                        //  int x = _repository.UpdateStatus(id, EStatusPengadaan.SUBMITPENAWARAN);
                    }
                    return Json(dokumen.Id);
                }
                catch (Exception ex)
                {
                    return Json(0);
                }
            }

            return Json(dokumen.Id);
        }

        [Authorize]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public async Task<HttpResponseMessage> BerkasAanwzjing(Guid Id)
        {
            var pengadaan = _repository.GetPengadaan(Id, UserId(), 0);
            var jadwalAanwijzing = _repository.getPelaksanaanAanwijing(Id);
            string fileName = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Template\Berita Acara Pemberian Penjelasan.docx";

            string outputFileName = "BA-Aanwizjing-" + Id.ToString() + "-" + DateTime.Now.ToString("dd-MM-yy") + ".docx";

            string OutFileNama = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Temp\" + outputFileName;
            // Create a document in memory:
            //string outputFileName =
            //        string.Format(fileName, "BAcoooooooooooooot", DateTime.Now.ToString("dd-MM-yy"));
            var streamx = new FileStream(fileName, FileMode.Open);
            try
            {
                var BeritaAcara = _repository.getBeritaAcaraByTipe(Id, TipeBerkas.BeritaAcaraAanwijzing, UserId());
                var doc = DocX.Load(streamx);//.Create(OutFileNama);
                doc.ReplaceText("{pengadaan_name}", pengadaan.Judul == null ? "" : pengadaan.Judul);
                doc.ReplaceText("{pengadaan_name_judul}", pengadaan.Judul == null ? "" : pengadaan.Judul.ToUpper());
                doc.ReplaceText("{nomor_berita_acara}", BeritaAcara == null ? "" : BeritaAcara.NoBeritaAcara == null ? "" : BeritaAcara.NoBeritaAcara);
                doc.ReplaceText("{pengadaan_unit_pemohon}", pengadaan.UnitKerjaPemohon == null ? "" : pengadaan.UnitKerjaPemohon);

                var kandidat = _repository.getKandidatHadir(Id, UserId());
                //var kandidat = _repository.getKandidatPengadaan(Id, UserId());
                //var kandidattidakhadir = _repository.getKandidatTidakHadir(Id, UserId());
                var table = doc.AddTable(kandidat.Count(), 1);
                int rowIndex = 0;
                foreach (var item in kandidat)
                {

                    table.Rows[rowIndex].Cells[0].Paragraphs.First().Append((rowIndex + 1) + ". " + "....................   Mewakili: " + item.Vendor.Nama);
                    //table.Rows[rowIndex].Cells[0].Paragraphs.First().Append((rowIndex + 1) + ". " + "....................   Mewakili: " + item.NamaVendor);
                    table.Rows[rowIndex].Cells[0].Paragraphs.First().FontSize(11).Font(new FontFamily("Calibri"));
                    table.Rows[rowIndex].Cells[0].Width = 500;
                    rowIndex++;
                }

                table.Alignment = Alignment.left;

                foreach (var paragraph in doc.Paragraphs)
                {
                    paragraph.FindAll("{tabel}").ForEach(index => paragraph.InsertTableBeforeSelf(table));

                }
                doc.ReplaceText("{tabel}", "");


                // Panitia
                var panitia = _repository.getListPersonilPengadaan(Id);
                panitia = panitia.Where(d => d.tipe != PengadaanConstants.StaffPeranan.Tim).ToList();
                var table4 = doc.AddTable(panitia.Count(), 1);
                rowIndex = 0;
                foreach (var item in panitia)
                {
                    table4.Rows[rowIndex].Cells[0].Paragraphs.First().Append((rowIndex + 1) + ". " + "" + item.Nama);
                    table4.Rows[rowIndex].Cells[0].Paragraphs.First().FontSize(11).Font(new FontFamily("Calibri"));
                    table4.Rows[rowIndex].Cells[0].Width = 200;
                    rowIndex++;
                }

                table4.Alignment = Alignment.left;

                foreach (var paragraph in doc.Paragraphs)
                {
                    paragraph.FindAll("{tabel4}").ForEach(index => paragraph.InsertTableBeforeSelf(table4));

                }
                doc.ReplaceText("{tabel4}", "");


                //Tanggal
                if (BeritaAcara != null)
                {
                    //doc.ReplaceText("{pengadaan_jadwal_hari}", Common.ConvertHari((int)BeritaAcara.tanggal.Value.DayOfWeek));
                    doc.ReplaceText("{pengadaan_jadwal_tanggal}", BeritaAcara.tanggal.Value.Day.ToString() +
                        " " + Common.ConvertNamaBulan(BeritaAcara.tanggal.Value.Month) +
                        " " + BeritaAcara.tanggal.Value.Year.ToString());
                    doc.ReplaceText("{tempat_tanggal}", ".............," + BeritaAcara.tanggal.Value.Day.ToString() +
                        " " + Common.ConvertNamaBulan(BeritaAcara.tanggal.Value.Month) +
                        " " + BeritaAcara.tanggal.Value.Year.ToString());
                }
                else
                {
                    doc.ReplaceText("{pengadaan_jadwal_hari} ", " - ");
                    doc.ReplaceText("{pengadaan_jadwal_tanggal}", "");
                    doc.ReplaceText("{tempat_tanggal}", "...............,...........................");
                }

                // Personil Persetujuan 
                var listPersonil = _repository.getListPersonilPengadaan(Id);
                var NamePic = listPersonil.Where(d => d.tipe == "pic").FirstOrDefault().Nama;
                doc.ReplaceText("{nama_pic}", NamePic);

                //tambah tabel persetujuan tahapan
                var table2 = await getTablePersetujuan(pengadaan.Id, EStatusPengadaan.AANWIJZING, doc);

                table2.Alignment = Alignment.center;
                //table.AutoFit = AutoFit.Contents;

                foreach (var paragraph in doc.Paragraphs)
                {
                    paragraph.FindAll("{tabel2}").ForEach(index => paragraph.InsertTableBeforeSelf(table2));
                }
                doc.ReplaceText("{tabel2}", "");
                //end

                // Kandidat Tidak Hadir
                var kandidattidakhadir = _repository.getKandidatTidakHadir(Id, UserId());
                if (kandidattidakhadir.Count() > 0)
                {
                    var table3 = doc.AddTable(kandidattidakhadir.Count(), 1);
                    int rowIndex2 = 0;
                    foreach (var item in kandidattidakhadir)
                    {
                        table3.Rows[rowIndex2].Cells[0].Paragraphs.First().Append((rowIndex2 + 1) + ". " + "....................   Mewakili: " + item.Vendor.Nama);
                        table3.Rows[rowIndex2].Cells[0].Paragraphs.First().FontSize(11).Font(new FontFamily("Calibri"));
                        table3.Rows[rowIndex2].Cells[0].Width = 500;
                        rowIndex2++;
                    }

                    table3.Alignment = Alignment.left;

                    foreach (var paragraph in doc.Paragraphs)
                    {
                        paragraph.FindAll("{tabel3}").ForEach(index => paragraph.InsertTableBeforeSelf(table3));

                    }
                    doc.ReplaceText("{tabel3}", "");
                }
                else
                {
                    doc.ReplaceText("{tabel3}", "-");

                }
                doc.SaveAs(OutFileNama);
                streamx.Close();
            }
            catch
            {
                streamx.Close();
            }
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new FileStream(OutFileNama, FileMode.Open);
            result.Content = new StreamContent(stream);
            //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.wordprocessingml.document");

            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = outputFileName
            };

            return result;
        }

        private async Task<Table> getTablePersetujuan(Guid PengadaanId, EStatusPengadaan status, DocX doc)
        {
            var personilPersetujuan = _repository.GetPersetujuanTahapan(PengadaanId, status);
            var table2 = doc.AddTable(personilPersetujuan.Count() + 1, 3);
            Border WhiteBorder = new Border(BorderStyle.Tcbs_single, BorderSize.four, 1, Color.Black);
            table2.SetBorder(TableBorderType.Bottom, WhiteBorder);
            table2.SetBorder(TableBorderType.Left, WhiteBorder);
            table2.SetBorder(TableBorderType.Right, WhiteBorder);
            table2.SetBorder(TableBorderType.Top, WhiteBorder);
            table2.SetBorder(TableBorderType.InsideV, WhiteBorder);
            table2.SetBorder(TableBorderType.InsideH, WhiteBorder);
            int rowIndex = 0;

            table2.Rows[rowIndex].Cells[0].Paragraphs.First().Append("Nama");
            table2.Rows[rowIndex].Cells[0].Paragraphs.First().FontSize(11).Font(new FontFamily("Calibri"));
            table2.Rows[rowIndex].Cells[0].Paragraphs.First().Alignment = Alignment.center;
            table2.Rows[rowIndex].Cells[0].Width = 500;
            table2.Rows[rowIndex].Cells[1].Paragraphs.First().Append("Tanggal");
            table2.Rows[rowIndex].Cells[1].Paragraphs.First().FontSize(11).Font(new FontFamily("Calibri"));
            table2.Rows[rowIndex].Cells[1].Paragraphs.First().Alignment = Alignment.center;
            table2.Rows[rowIndex].Cells[1].Width = 500;
            table2.Rows[rowIndex].Cells[2].Paragraphs.First().Append("Status");
            table2.Rows[rowIndex].Cells[2].Paragraphs.First().FontSize(11).Font(new FontFamily("Calibri"));
            table2.Rows[rowIndex].Cells[2].Paragraphs.First().Alignment = Alignment.center;
            table2.Rows[rowIndex].Cells[2].Width = 500;
            rowIndex++;
            foreach (var item in personilPersetujuan)
            {
                var user = await userDetail(item.UserId.ToString());
                table2.Rows[rowIndex].Cells[0].Paragraphs.First().Append(user.FullName);
                table2.Rows[rowIndex].Cells[0].Paragraphs.First().FontSize(11).Font(new FontFamily("Calibri"));
                table2.Rows[rowIndex].Cells[0].Width = 500;
                table2.Rows[rowIndex].Cells[1].Paragraphs.First().Append(item.CreatedOn == null ? "" : item.CreatedOn.Value.Day.ToString() +
                    " " + Common.ConvertNamaBulan(item.CreatedOn.Value.Month) +
                    " " + item.CreatedOn.Value.Year.ToString());
                table2.Rows[rowIndex].Cells[1].Paragraphs.First().FontSize(11).Font(new FontFamily("Calibri"));
                table2.Rows[rowIndex].Cells[1].Width = 500;
                table2.Rows[rowIndex].Cells[2].Paragraphs.First().Append(item.Status.ToString());
                table2.Rows[rowIndex].Cells[2].Paragraphs.First().FontSize(11).Font(new FontFamily("Calibri"));
                table2.Rows[rowIndex].Cells[2].Width = 500;
                rowIndex++;
            }

            return table2;
        }

        private async Task<Table> getTableDisposisi(Guid PengadaanId, EStatusPengadaan status, DocX doc)
        {
            //UserId();
            var pengadaan2 = _repository.GetPengadaanByiD(PengadaanId);

            var worflowDetail = _workflowrepo.getWorflowByWorkflowId(pengadaan2.PersetujuanPemenangs.LastOrDefault().WorkflowId.Value);
            var userterkait = _repository.GetCommentUserTerkait(PengadaanId);

            var table2 = doc.AddTable(worflowDetail.Count() + 1 + userterkait.Count(), 2);

            Border WhiteBorder = new Border(BorderStyle.Tcbs_single, BorderSize.four, 1, Color.Black);
            table2.SetBorder(TableBorderType.Bottom, WhiteBorder);
            table2.SetBorder(TableBorderType.Left, WhiteBorder);
            table2.SetBorder(TableBorderType.Right, WhiteBorder);
            table2.SetBorder(TableBorderType.Top, WhiteBorder);
            table2.SetBorder(TableBorderType.InsideV, WhiteBorder);
            table2.SetBorder(TableBorderType.InsideH, WhiteBorder);
            int rowIndex = 0;

            table2.Rows[rowIndex].Cells[0].Paragraphs.First().Append("Nama");
            table2.Rows[rowIndex].Cells[0].Paragraphs.First().FontSize(11).Font(new FontFamily("Calibri"));
            table2.Rows[rowIndex].Cells[0].Paragraphs.First().Alignment = Alignment.center;
            table2.Rows[rowIndex].Cells[0].Width = 500;
            table2.Rows[rowIndex].Cells[1].Paragraphs.First().Append("Disposisi");
            table2.Rows[rowIndex].Cells[1].Paragraphs.First().FontSize(11).Font(new FontFamily("Calibri"));
            table2.Rows[rowIndex].Cells[1].Paragraphs.First().Alignment = Alignment.center;
            table2.Rows[rowIndex].Cells[1].Width = 500;
            rowIndex++;

            foreach (var item in userterkait)
            {
                var usera = await userDetail(item.UserId.ToString());
                if (item.setuju == 1)
                {
                    table2.Rows[rowIndex].Cells[0].Paragraphs.First().Append(usera.FullName);
                    table2.Rows[rowIndex].Cells[0].Paragraphs.First().FontSize(11).Font(new FontFamily("Calibri"));
                    table2.Rows[rowIndex].Cells[0].Width = 500;
                    table2.Rows[rowIndex].Cells[1].Paragraphs.First().Append("Menyetujui. Catatan : " + item.disposisi);
                    table2.Rows[rowIndex].Cells[1].Paragraphs.First().FontSize(11).Font(new FontFamily("Calibri"));
                    table2.Rows[rowIndex].Cells[1].Width = 500;
                    rowIndex++;
                }
                if (item.setuju == 2)
                {
                    table2.Rows[rowIndex].Cells[0].Paragraphs.First().Append(usera.FullName);
                    table2.Rows[rowIndex].Cells[0].Paragraphs.First().FontSize(11).Font(new FontFamily("Calibri"));
                    table2.Rows[rowIndex].Cells[0].Width = 500;
                    table2.Rows[rowIndex].Cells[1].Paragraphs.First().Append("Tidak Menyetujui. Catatan :" + item.disposisi);
                    table2.Rows[rowIndex].Cells[1].Paragraphs.First().FontSize(11).Font(new FontFamily("Calibri"));
                    table2.Rows[rowIndex].Cells[1].Width = 500;
                }
            }

            foreach (var item in worflowDetail)
            {
                var user = await userDetail(item.UserId.ToString());
                table2.Rows[rowIndex].Cells[0].Paragraphs.First().Append(user.FullName);
                table2.Rows[rowIndex].Cells[0].Paragraphs.First().FontSize(11).Font(new FontFamily("Calibri"));
                table2.Rows[rowIndex].Cells[0].Width = 500;
                table2.Rows[rowIndex].Cells[1].Paragraphs.First().Append(item.Comment);
                table2.Rows[rowIndex].Cells[1].Paragraphs.First().FontSize(11).Font(new FontFamily("Calibri"));
                table2.Rows[rowIndex].Cells[1].Width = 500;
                rowIndex++;
            }

            return table2;
        }

        private async Task<Table> getPemenangPengadaan(Guid Id, DocX doc)
        {
            var pemenang = _repository.getPemenangPengadaan(Id, UserId());

            var table2 = doc.AddTable(pemenang.Count() + 1, 2);
            //Border WhiteBorder = new Border(BorderStyle.Tcbs_single, BorderSize.four, 1, Color.Black);
            Border WhiteBorder = new Border(BorderStyle.Tcbs_none, 0, 0, Color.White);
            table2.SetBorder(TableBorderType.Bottom, WhiteBorder);
            table2.SetBorder(TableBorderType.Left, WhiteBorder);
            table2.SetBorder(TableBorderType.Right, WhiteBorder);
            table2.SetBorder(TableBorderType.Top, WhiteBorder);
            table2.SetBorder(TableBorderType.InsideV, WhiteBorder);
            table2.SetBorder(TableBorderType.InsideH, WhiteBorder);
            int rowIndex = 0;

            //table2.Rows[rowIndex].Cells[0].Paragraphs.First().Append("Nama Pemenang");
            //table2.Rows[rowIndex].Cells[0].Paragraphs.First().FontSize(11).Font(new FontFamily("Calibri"));
            //table2.Rows[rowIndex].Cells[0].Paragraphs.First().Alignment = Alignment.center;
            //table2.Rows[rowIndex].Cells[0].Width = 300;
            //table2.Rows[rowIndex].Cells[1].Paragraphs.First().Append("Harga");
            //table2.Rows[rowIndex].Cells[1].Paragraphs.First().FontSize(11).Font(new FontFamily("Calibri"));
            //table2.Rows[rowIndex].Cells[1].Paragraphs.First().Alignment = Alignment.center;
            //table2.Rows[rowIndex].Cells[1].Width = 200;
            //rowIndex++;
            foreach (var item in pemenang)
            {
                table2.Rows[rowIndex].Cells[0].Paragraphs.First().Append((rowIndex + 1) + ". " + item.NamaVendor);
                table2.Rows[rowIndex].Cells[0].Paragraphs.First().FontSize(11).Font(new FontFamily("Calibri"));
                table2.Rows[rowIndex].Cells[0].Width = 200;
                table2.Rows[rowIndex].Cells[1].Paragraphs.First().Append(item.total == null ? "" : item.total.Value.ToString("C", MyConverter.formatCurrencyIndo()));
                table2.Rows[rowIndex].Cells[1].Paragraphs.First().FontSize(11).Font(new FontFamily("Calibri"));
                table2.Rows[rowIndex].Cells[1].Width = 200;
                rowIndex++;
            }

            return table2;
        }


        [Authorize]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public HttpResponseMessage BerkasDaftarRapat(Guid Id)
        {
            var pengadaan = _repository.GetPengadaan(Id, UserId(), 0);
            var jadwalAanwijzing = _repository.getPelaksanaanAanwijing(Id);
            string fileName = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Template\Daftar Rapat.docx";

            string outputFileName = "Daftar-Rapat-" + Id.ToString() + "-" + DateTime.Now.ToString("dd-MM-yy") + ".docx";

            string OutFileNama = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Temp\" + outputFileName;
            // Create a document in memory:
            //string outputFileName =
            //        string.Format(fileName, "BAcoooooooooooooot", DateTime.Now.ToString("dd-MM-yy"));
            var streamx = new FileStream(fileName, FileMode.Open);
            try
            {
                var doc = DocX.Load(streamx);//.Create(OutFileNama);
                doc.SaveAs(OutFileNama);
                streamx.Close();
            }
            catch
            {
                streamx.Close();
            }
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new FileStream(OutFileNama, FileMode.Open);
            result.Content = new StreamContent(stream);
            //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.wordprocessingml.document");

            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = outputFileName
            };

            return result;
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                          IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                           IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_approver)]
        public HttpResponseMessage ReportPengadaan(string dari, string sampai, string divisi)
        {
            try
            {
                LocalReport lr = new LocalReport();
                string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Report\BukaAmplop.rdlc";
                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //return View("Index");
                }
                var oDari = Common.ConvertDate(dari, "dd/MM/yyyy");
                var oSampai = Common.ConvertDate(sampai, "dd/MM/yyyy");
                string oDivisi = divisi;

                var oKandidatPengadaan = _repository.GetRepotPengadan(oDari, oSampai, UserId(), oDivisi);

                ReportDataSource rd = new ReportDataSource("DataSet2", oKandidatPengadaan);
                lr.DataSources.Add(rd);
                string param1 = "";
                string filename = "";
                string param2 = "";
                string paramSemester = "";
                string paramTahunAjaran = "";


                string reportType = "doc";
                string mimeType;
                string encoding;
                string fileNameExtension;


                string[] streamids = null;
                String extension = null;
                Byte[] bytes = null;
                Warning[] warnings;

                bytes = lr.Render("Excel", null, out mimeType, out encoding, out extension, out streamids, out warnings);

                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                Stream stream = new MemoryStream(bytes);

                result.Content = new StreamContent(stream);

                //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");

                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = "Report-Tender" + UserId() + DateTime.Now.ToString("dd-MM-yy") + ".xls"
                };

                return result;
            }
            catch (ReflectionTypeLoadException ex)
            {
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                StringBuilder sb = new StringBuilder();
                foreach (Exception exSub in ex.LoaderExceptions)
                {
                    sb.AppendLine(exSub.Message);
                    FileNotFoundException exFileNotFound = exSub as FileNotFoundException;
                    if (exFileNotFound != null)
                    {
                        if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                        {
                            sb.AppendLine("Fusion Log:");
                            sb.AppendLine(exFileNotFound.FusionLog);
                        }
                    }
                    sb.AppendLine();
                }
                result.Content = new StringContent(sb.ToString());

                return result;
                //Display or log the error based on your application.
            }
        }



        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public async Task<HttpResponseMessage> BerkasBukaAmplop(Guid Id)
        {
            var pengadaan = _repository.GetPengadaan(Id, UserId(), 0);
            var jadwalBukaAmplop = _repository.getPelaksanaanBukaAmplop(Id);
            string fileName = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Template\BERITA ACARA BUKA AMPLOP.docx";
            var BeritaAcara = _repository.getBeritaAcaraByTipe(Id, TipeBerkas.BeritaAcaraBukaAmplop, UserId());
            string outputFileName = "BA-Buka-Amplop-" + (BeritaAcara == null ? "" : BeritaAcara.NoBeritaAcara.Replace("/", "-")) + "-" + DateTime.Now.ToString("dd-MM-yy") + ".docx";

            string OutFileNama = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Temp\" + outputFileName;

            var streamx = new FileStream(fileName, FileMode.Open);
            try
            {

                var doc = DocX.Load(streamx);//.Create(OutFileNama);
                doc.ReplaceText("{pengadaan_name}", pengadaan.Judul == null ? "" : pengadaan.Judul);
                doc.ReplaceText("{pengadaan_name_judul}", pengadaan.Judul == null ? "" : pengadaan.Judul.ToUpper());
                doc.ReplaceText("{nomor_berita_acara}", BeritaAcara == null ? "" : BeritaAcara.NoBeritaAcara == null ? "" : BeritaAcara.NoBeritaAcara);
                doc.ReplaceText("{pengadaan_unit_pemohon}", pengadaan.UnitKerjaPemohon == null ? "" : pengadaan.UnitKerjaPemohon);

                doc.ReplaceText("{tempat_tanggal}", BeritaAcara == null ? "" : "...............," + BeritaAcara.tanggal == null ? "................" :
                       BeritaAcara.tanggal.Value.Day + " " + Common.ConvertNamaBulan(BeritaAcara.tanggal.Value.Month) + " " +
                       BeritaAcara.tanggal.Value.Year);

                doc.ReplaceText("{pengadaan_jadwal_hari}", BeritaAcara == null ? "" : BeritaAcara.tanggal == null ? "" :
                       Common.ConvertHari(BeritaAcara.tanggal.Value.Day));
                doc.ReplaceText("{pengadaan_jadwal_tanggal}", BeritaAcara == null ? "" : BeritaAcara.tanggal.Value.Day + " " + Common.ConvertNamaBulan(BeritaAcara.tanggal.Value.Month) +
                      " " + BeritaAcara.tanggal.Value.Year);

                // Kandidat Kirim Penawaran
                var kandidat = _repository.getKandidatKirim(Id, UserId());
                if (kandidat.Count() > 0)
                {
                    var table = doc.AddTable(kandidat.Count(), 1);
                    Border WhiteBorder2 = new Border(BorderStyle.Tcbs_none, 0, 0, Color.White);
                    table.SetBorder(TableBorderType.Bottom, WhiteBorder2);
                    table.SetBorder(TableBorderType.Left, WhiteBorder2);
                    table.SetBorder(TableBorderType.Right, WhiteBorder2);
                    table.SetBorder(TableBorderType.Top, WhiteBorder2);
                    table.SetBorder(TableBorderType.InsideV, WhiteBorder2);
                    table.SetBorder(TableBorderType.InsideH, WhiteBorder2);
                    int rowIndex2 = 0;
                    foreach (var item in kandidat)
                    {
                        table.Rows[rowIndex2].Cells[0].Paragraphs.First().Append((rowIndex2 + 1) + ". " + item.Vendor.Nama);
                        table.Rows[rowIndex2].Cells[0].Paragraphs.First().FontSize(11).Font(new FontFamily("Calibri"));
                        table.Rows[rowIndex2].Cells[0].Width = 500;
                        rowIndex2++;
                    }

                    table.Alignment = Alignment.left;
                    foreach (var paragraph in doc.Paragraphs)
                    {
                        paragraph.FindAll("{vendor}").ForEach(index => paragraph.InsertTableBeforeSelf(table));

                    }
                    doc.ReplaceText("{vendor}", "");
                }
                else
                {
                    doc.ReplaceText("{vendor}", "-");

                }
                // End Kandidat Kirim Penawaran

                // Kandidat Tidak Kirim Penawaran
                var kandidattidakkirim = _repository.getKandidatTidakKirim(Id, UserId());
                if (kandidattidakkirim.Count() > 0)
                {
                    var table = doc.AddTable(kandidattidakkirim.Count(), 1);
                    Border WhiteBorder2 = new Border(BorderStyle.Tcbs_none, 0, 0, Color.White);
                    table.SetBorder(TableBorderType.Bottom, WhiteBorder2);
                    table.SetBorder(TableBorderType.Left, WhiteBorder2);
                    table.SetBorder(TableBorderType.Right, WhiteBorder2);
                    table.SetBorder(TableBorderType.Top, WhiteBorder2);
                    table.SetBorder(TableBorderType.InsideV, WhiteBorder2);
                    table.SetBorder(TableBorderType.InsideH, WhiteBorder2);
                    int rowIndex2 = 0;
                    foreach (var item in kandidattidakkirim)
                    {
                        table.Rows[rowIndex2].Cells[0].Paragraphs.First().Append((rowIndex2 + 1) + ". " + item.Vendor.Nama);
                        table.Rows[rowIndex2].Cells[0].Paragraphs.First().FontSize(11).Font(new FontFamily("Calibri"));
                        table.Rows[rowIndex2].Cells[0].Width = 500;
                        rowIndex2++;
                    }

                    table.Alignment = Alignment.left;
                    foreach (var paragraph in doc.Paragraphs)
                    {
                        paragraph.FindAll("{tidakkirim}").ForEach(index => paragraph.InsertTableBeforeSelf(table));

                    }
                    doc.ReplaceText("{tidakkirim}", "");
                }
                else
                {
                    doc.ReplaceText("{tidakkirim}", "-");

                }
                // End Kandidat Tidak Kirim Penawaran

                // Panitia
                var panitia = _repository.getPersonilPengadaan(Id);
                var tablePanitia = doc.AddTable(panitia.Count(), 1);
                Border WhiteBorder = new Border(BorderStyle.Tcbs_none, 0, 0, Color.White);
                tablePanitia.SetBorder(TableBorderType.Bottom, WhiteBorder);
                tablePanitia.SetBorder(TableBorderType.Left, WhiteBorder);
                tablePanitia.SetBorder(TableBorderType.Right, WhiteBorder);
                tablePanitia.SetBorder(TableBorderType.Top, WhiteBorder);
                tablePanitia.SetBorder(TableBorderType.InsideV, WhiteBorder);
                tablePanitia.SetBorder(TableBorderType.InsideH, WhiteBorder);
                int rowIndex = 0;
                foreach (var item in panitia)
                {
                    tablePanitia.Rows[rowIndex].Cells[0].Paragraphs.First().Append((rowIndex + 1) + ". " + item.Nama);
                    tablePanitia.Rows[rowIndex].Cells[0].Paragraphs.First().FontSize(11).Font(new FontFamily("Calibri"));
                    tablePanitia.Rows[rowIndex].Cells[0].Width = 500;
                    rowIndex++;
                }

                tablePanitia.Alignment = Alignment.left;
                foreach (var paragraph in doc.Paragraphs)
                {
                    paragraph.FindAll("{panitia}").ForEach(index => paragraph.InsertTableBeforeSelf(tablePanitia));

                }
                doc.ReplaceText("{panitia}", "");
                // End Panitia

                //tambah tabel persetujuan tahapan
                var table3 = await getTablePersetujuan(pengadaan.Id, EStatusPengadaan.BUKAAMPLOP, doc);

                table3.Alignment = Alignment.center;
                //table.AutoFit = AutoFit.Contents;

                foreach (var paragraph in doc.Paragraphs)
                {
                    paragraph.FindAll("{table3}").ForEach(index => paragraph.InsertTableBeforeSelf(table3));

                }
                doc.ReplaceText("{table3}", "");
                //end

                if (_repository.CekBukaAmplopTahapan(Id) == 1)
                {

                    var oVWRKSVendors = _repository.getRKSPenilaian2Report(pengadaan.Id, UserId());
                    var table2 = doc.AddTable(oVWRKSVendors.hps.Count() + 1, (oVWRKSVendors.vendors.Count * 2) + 7);
                    Border BlankBorder = new Border(BorderStyle.Tcbs_single, BorderSize.one, 0, Color.Black);
                    table2.SetBorder(TableBorderType.Bottom, BlankBorder);
                    table2.SetBorder(TableBorderType.Left, BlankBorder);
                    table2.SetBorder(TableBorderType.Right, BlankBorder);
                    table2.SetBorder(TableBorderType.Top, BlankBorder);
                    table2.SetBorder(TableBorderType.InsideV, BlankBorder);
                    table2.SetBorder(TableBorderType.InsideH, BlankBorder);


                    int indexRow = 0;
                    table2.Rows[indexRow].Cells[0].Paragraphs.First().Append("NO");
                    table2.Rows[indexRow].Cells[0].Width = 10;
                    table2.Rows[indexRow].Cells[1].Paragraphs.First().Append("Nama");
                    table2.Rows[indexRow].Cells[2].Paragraphs.First().Append("Item");
                    table2.Rows[indexRow].Cells[3].Paragraphs.First().Append("Satuan");
                    table2.Rows[indexRow].Cells[4].Paragraphs.First().Append("Jumlah");
                    table2.Rows[indexRow].Cells[5].Paragraphs.First().Append("Keterangan Item");
                    table2.Rows[indexRow].Cells[6].Paragraphs.First().Append("Harga HPS");
                    int headerCol = 7;
                    foreach (var item in oVWRKSVendors.vendors)
                    {
                        table2.Rows[indexRow].Cells[headerCol].Paragraphs.First().Append(item.nama);
                        headerCol++;
                        table2.Rows[indexRow].Cells[headerCol].Paragraphs.First().Append("Total (" + item.nama + ")");
                        headerCol++;
                    }
                    indexRow++;
                    var itemlast = oVWRKSVendors.hps.Last();
                    int no = 1;
                    decimal? subtotal = 0;
                    foreach (var item in oVWRKSVendors.hps)
                    {
                        var currentGroup = item.grup;
                        var index = oVWRKSVendors.hps.IndexOf(item);
                        if (item.harga != null && item.jumlah != null)
                            subtotal += item.harga * item.jumlah;
                        if (item.Equals(itemlast))
                        {
                            table2.Rows[indexRow].Cells[0].Paragraphs.First().Append("");
                            table2.Rows[indexRow].Cells[0].Width = 10;
                        }
                        else
                        {

                            if (item.jumlah > 0)
                            {
                                table2.Rows[indexRow].Cells[0].Paragraphs.First().Append(no.ToString());
                                table2.Rows[indexRow].Cells[0].Width = 10;
                                no++;
                            }
                            else
                            {
                                table2.Rows[indexRow].Cells[0].Paragraphs.First().Append("");
                                table2.Rows[indexRow].Cells[0].Width = 10;
                            }
                        }


                        table2.Rows[indexRow].Cells[1].Paragraphs.First().Append(item.judul != "undefined" ? item.judul : "");
                        //Regex example #1 "<.*?>"
                        string dekripsi = item.item != null ? Regex.Replace(item.item, @"<.*?>", string.Empty) : "";
                        //Regex example #2
                        // string result2 = Regex.Replace(dekripsi, @"<[^>].+?>", "");
                        table2.Rows[indexRow].Cells[2].Paragraphs.First().Append(dekripsi);
                        var info = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                        info.NumberFormat.CurrencyDecimalDigits = 0;
                        info.NumberFormat.CurrencySymbol = "Rp ";
                        info.NumberFormat.CurrencyGroupSeparator = ".";
                        info.NumberFormat.CurrencyDecimalSeparator = ",";

                        table2.Rows[indexRow].Cells[3].Paragraphs.First().Append(item.satuan);
                        table2.Rows[indexRow].Cells[4].Paragraphs.First().Append(item.jumlah.ToString());
                        table2.Rows[indexRow].Cells[5].Paragraphs.First().Append(item.keteranganItem);
                        if ((item.harga != null && item.jumlah != null) || item.isTotal == 1)
                        {
                            //decimal harga = item.harga.Value * item.jumlah.Value;
                            table2.Rows[indexRow].Cells[6].Paragraphs.First().Append(item.harga.Value.ToString("C", MyConverter.formatCurrencyIndo()));
                        }
                        else table2.Rows[indexRow].Cells[6].Paragraphs.First().Append("");

                        /*sub totol asumsi kalo next group nya beda*/
                        if (index < oVWRKSVendors.hps.Count() - 1)
                        {
                            if (currentGroup != null && currentGroup != oVWRKSVendors.hps[index + 1].grup)
                            {
                                table2.Rows[indexRow].Cells[5].Paragraphs.First().Append("Sub Total");
                                table2.Rows[indexRow].Cells[6].Paragraphs.First().Append(subtotal.Value.ToString("C", MyConverter.formatCurrencyIndo()));
                                subtotal = 0;
                                no = 1;
                            }
                        }

                        int nexCol = 7;

                        foreach (var itemx in oVWRKSVendors.vendors)
                        {
                            if (itemx.items.Where(d => d.Id == item.Id) != null)
                            {
                                var itemxx = itemx.items.Where(d => d.Id == item.Id).FirstOrDefault();
                                var harga = itemxx == null ? "" : itemxx.harga == null ? "" : itemxx.harga.Value.ToString("C", MyConverter.formatCurrencyIndo());
                                var jumlah = itemxx == null ? "" : itemxx.jumlah == null ? "" : itemxx.jumlah.Value.ToString();
                                if (index < oVWRKSVendors.hps.Count() - 1)
                                {
                                    if (currentGroup != oVWRKSVendors.hps[index + 1].grup)
                                    {
                                        if (itemxx != null)
                                        {
                                            if (itemxx.subtotal != null)
                                                table2.Rows[indexRow].Cells[nexCol].Paragraphs.First().Append(itemxx.subtotal.totalGroup != null ? itemxx.subtotal.totalGroup.Value.ToString("C", MyConverter.formatCurrencyIndo()) : "");
                                        }
                                    }
                                    else
                                    {
                                        if (jumlah != string.Empty && harga != string.Empty)
                                        {
                                            table2.Rows[indexRow].Cells[nexCol].Paragraphs.First().Append(harga);
                                        }

                                    }
                                }
                                if (item.isTotal == 1)
                                {
                                    table2.Rows[indexRow].Cells[nexCol].Paragraphs.First().Append(harga);
                                }

                                nexCol++;

                                if (jumlah != string.Empty && harga != string.Empty)
                                {
                                    var total = itemxx.jumlah * itemxx.harga;
                                    table2.Rows[indexRow].Cells[nexCol].Paragraphs.First().Append(total.Value.ToString("C", MyConverter.formatCurrencyIndo()));
                                }
                                else table2.Rows[indexRow].Cells[nexCol].Paragraphs.First().Append("");
                                /*sub totol asumsi kalo next group nya beda*/

                            }
                            else table2.Rows[indexRow].Cells[nexCol].Paragraphs.First().Append("");

                            nexCol++;
                        }

                        indexRow++;
                    }

                    System.IO.MemoryStream ms2 = new System.IO.MemoryStream();
                    DocX doc2 = DocX.Create(ms2);

                    doc2.PageLayout.Orientation = Novacode.Orientation.Landscape;
                    Paragraph p = doc2.InsertParagraph();
                    p.Append("Lampiran").Bold();
                    p.Alignment = Alignment.left;
                    p.AppendLine();
                    Table t = doc2.InsertTable(table2);
                    doc.InsertSection();
                    doc.InsertDocument(doc2);
                    //doc.InsertTable(table2);

                }
                doc.SaveAs(OutFileNama);
                streamx.Close();
            }
            catch
            {
                streamx.Close();
            }
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            if (_repository.CekBukaAmplopTahapan(Id) == 1)
            {
                var stream = new FileStream(OutFileNama, FileMode.Open);
                result.Content = new StreamContent(stream);
                //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.wordprocessingml.document");

                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = outputFileName
                };
            }
            else result.Content = new StringContent("Anda Tidak Memiliki Akses");
            return result;
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public async Task<HttpResponseMessage> BerkasKlarfikasi(Guid Id)
        {
            var pengadaan = _repository.GetPengadaan(Id, UserId(), 0);
            var jadwalKlarifikasi = _repository.getPelaksanaanKlarifikasi(Id, UserId());
            string fileName = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Template\BERITA ACARA RAPAT KLARIFIKASI DAN NEGOSIASI.docx";
            var BeritaAcara = _repository.getBeritaAcaraByTipe(Id, TipeBerkas.BeritaAcaraKlarifikasi, UserId());
            string outputFileName = "Berkas-Klarifikasi-" + (BeritaAcara == null ? "" : BeritaAcara.NoBeritaAcara.Replace("/", "-")) + "-" + DateTime.Now.ToString("dd-MM-yy") + ".docx";

            string OutFileNama = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Temp\" + outputFileName;

            var streamx = new FileStream(fileName, FileMode.Open);

            var doc = DocX.Load(streamx);

            try
            {
                doc.ReplaceText("{pengadaan_name}", pengadaan.Judul == null ? "" : pengadaan.Judul);
                doc.ReplaceText("{pengadaan_name_judul}", pengadaan.Judul == null ? "" : pengadaan.Judul.ToUpper());
                doc.ReplaceText("{nomor_berita_acara}", BeritaAcara == null ? "" : BeritaAcara.NoBeritaAcara == null ? "" : BeritaAcara.NoBeritaAcara);
                doc.ReplaceText("{pengadaan_unit_pemohon}", pengadaan.UnitKerjaPemohon == null ? "" : pengadaan.UnitKerjaPemohon);

                doc.ReplaceText("{tempat_tanggal}", "...............," + (BeritaAcara == null ? "................" :
                        (BeritaAcara.tanggal.Value.Day + " " + Common.ConvertNamaBulan(BeritaAcara.tanggal.Value.Month) + " " +
                        BeritaAcara.tanggal.Value.Year)));

                doc.ReplaceText("{pengadaan_jadwal_hari}", BeritaAcara == null ? "" :
                       Common.ConvertHari(BeritaAcara.tanggal.Value.Day));
                doc.ReplaceText("{pengadaan_jadwal_tanggal}", BeritaAcara == null ? "" : BeritaAcara.tanggal.Value.Day + " " + Common.ConvertNamaBulan(BeritaAcara.tanggal.Value.Month) +
                      " " + BeritaAcara.tanggal.Value.Year);

                var kandidat = _repository.GetVendorsKlarifikasiByPengadaanId2(Id);
                if (kandidat.Count() > 0)
                {
                    var table = doc.AddTable(kandidat.Count(), 1);
                    Border BlankBorder = new Border(BorderStyle.Tcbs_none, 0, 0, Color.White);
                    table.SetBorder(TableBorderType.Bottom, BlankBorder);
                    table.SetBorder(TableBorderType.Left, BlankBorder);
                    table.SetBorder(TableBorderType.Right, BlankBorder);
                    table.SetBorder(TableBorderType.Top, BlankBorder);
                    table.SetBorder(TableBorderType.InsideV, BlankBorder);
                    table.SetBorder(TableBorderType.InsideH, BlankBorder);

                    int rowIndex = 0;
                    foreach (var item in kandidat)
                    {
                        table.Rows[rowIndex].Cells[0].Paragraphs.First().Append((rowIndex + 1) + ". " + item.Nama);
                        table.Rows[rowIndex].Cells[0].Paragraphs.First().FontSize(11).Font(new FontFamily("Calibri"));
                        table.Rows[rowIndex].Cells[0].Width = 550;
                        rowIndex++;
                    }

                    table.Alignment = Alignment.left;
                    foreach (var paragraph in doc.Paragraphs)
                    {
                        paragraph.FindAll("{vendor}").ForEach(index => paragraph.InsertTableBeforeSelf(table));
                    }
                    doc.ReplaceText("{vendor}", "");
                }

                var panitia = _repository.getPersonilPengadaan(Id);
                var tablePanitia = doc.AddTable(panitia.Count(), 1);
                Border WhiteBorder = new Border(BorderStyle.Tcbs_none, 0, 0, Color.White);
                tablePanitia.SetBorder(TableBorderType.Bottom, WhiteBorder);
                tablePanitia.SetBorder(TableBorderType.Left, WhiteBorder);
                tablePanitia.SetBorder(TableBorderType.Right, WhiteBorder);
                tablePanitia.SetBorder(TableBorderType.Top, WhiteBorder);
                tablePanitia.SetBorder(TableBorderType.InsideV, WhiteBorder);
                tablePanitia.SetBorder(TableBorderType.InsideH, WhiteBorder);
                int rowIndex2 = 0;
                foreach (var item in panitia)
                {
                    tablePanitia.Rows[rowIndex2].Cells[0].Paragraphs.First().Append((rowIndex2 + 1) + ". " + item.Nama);
                    tablePanitia.Rows[rowIndex2].Cells[0].Paragraphs.First().FontSize(11).Font(new FontFamily("Calibri"));
                    tablePanitia.Rows[rowIndex2].Cells[0].Width = 500;
                    rowIndex2++;
                }

                tablePanitia.Alignment = Alignment.left;
                foreach (var paragraph in doc.Paragraphs)
                {
                    paragraph.FindAll("{panitia}").ForEach(index => paragraph.InsertTableBeforeSelf(tablePanitia));

                }
                doc.ReplaceText("{panitia}", "");
                // End Panitia


                //tambah tabel persetujuan tahapan
                var table3 = await getTablePersetujuan(pengadaan.Id, EStatusPengadaan.KLARIFIKASI, doc);

                table3.Alignment = Alignment.center;
                //table.AutoFit = AutoFit.Contents;

                foreach (var paragraph in doc.Paragraphs)
                {
                    paragraph.FindAll("{table3}").ForEach(index => paragraph.InsertTableBeforeSelf(table3));

                }
                doc.ReplaceText("{table3}", "");
                //end

                doc.SaveAs(OutFileNama);
                streamx.Close();
            }
            catch
            {
                streamx.Close();
            }
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new FileStream(OutFileNama, FileMode.Open);
            result.Content = new StreamContent(stream);
            //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.wordprocessingml.document");

            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = outputFileName
            };

            return result;
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public async Task<HttpResponseMessage> BerkasKlarfikasiLanjutan(Guid Id)
        {
            var pengadaan = _repository.GetPengadaan(Id, UserId(), 0);
            var jadwalKlarifikasi = _repository.getPelaksanaanKlarifikasiLanjutan(Id, UserId());
            string fileName = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Template\BERITA ACARA RAPAT KLARIFIKASI DAN NEGOSIASI LANJUTAN new.docx";
            var BeritaAcara = _repository.getBeritaAcaraByTipe(Id, TipeBerkas.BeritaAcaraKlarifikasiLanjutan, UserId());
            string outputFileName = "Berkas-Klarifikasi-Lanjutan" + (BeritaAcara == null ? "" : BeritaAcara.NoBeritaAcara.Replace("/", "-")) + "-" + DateTime.Now.ToString("dd-MM-yy") + ".docx";

            string OutFileNama = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Temp\" + outputFileName;

            var streamx = new FileStream(fileName, FileMode.Open);

            var doc = DocX.Load(streamx);

            try
            {
                doc.ReplaceText("{pengadaan_name}", pengadaan.Judul == null ? "" : pengadaan.Judul);
                doc.ReplaceText("{pengadaan_name_judul}", pengadaan.Judul == null ? "" : pengadaan.Judul.ToUpper());
                doc.ReplaceText("{nomor_berita_acara}", BeritaAcara == null ? "" : BeritaAcara.NoBeritaAcara == null ? "" : BeritaAcara.NoBeritaAcara);
                doc.ReplaceText("{pengadaan_unit_pemohon}", pengadaan.UnitKerjaPemohon == null ? "" : pengadaan.UnitKerjaPemohon);

                doc.ReplaceText("{tempat_tanggal}", "...............," + (BeritaAcara == null ? "................" :
                        (BeritaAcara.tanggal.Value.Day + " " + Common.ConvertNamaBulan(BeritaAcara.tanggal.Value.Month) + " " +
                        BeritaAcara.tanggal.Value.Year)));

                doc.ReplaceText("{pengadaan_jadwal_hari}", BeritaAcara == null ? "" :
                       Common.ConvertHari(BeritaAcara.tanggal.Value.Day));
                doc.ReplaceText("{pengadaan_jadwal_tanggal}", BeritaAcara == null ? "" : BeritaAcara.tanggal.Value.Day + " " + Common.ConvertNamaBulan(BeritaAcara.tanggal.Value.Month) +
                      " " + BeritaAcara.tanggal.Value.Year);

                var kandidat = _repository.GetVendorsKlarifikasiByPengadaanId2(Id);
                if (kandidat.Count() > 0)
                {
                    var table = doc.AddTable(kandidat.Count(), 1);
                    Border BlankBorder = new Border(BorderStyle.Tcbs_none, 0, 0, Color.White);
                    table.SetBorder(TableBorderType.Bottom, BlankBorder);
                    table.SetBorder(TableBorderType.Left, BlankBorder);
                    table.SetBorder(TableBorderType.Right, BlankBorder);
                    table.SetBorder(TableBorderType.Top, BlankBorder);
                    table.SetBorder(TableBorderType.InsideV, BlankBorder);
                    table.SetBorder(TableBorderType.InsideH, BlankBorder);

                    int rowIndex = 0;
                    foreach (var item in kandidat)
                    {
                        table.Rows[rowIndex].Cells[0].Paragraphs.First().Append((rowIndex + 1) + ". " + item.Nama);
                        table.Rows[rowIndex].Cells[0].Paragraphs.First().FontSize(11).Font(new FontFamily("Calibri"));
                        table.Rows[rowIndex].Cells[0].Width = 550;
                        rowIndex++;
                    }

                    table.Alignment = Alignment.left;
                    foreach (var paragraph in doc.Paragraphs)
                    {
                        paragraph.FindAll("{vendor}").ForEach(index => paragraph.InsertTableBeforeSelf(table));
                    }
                    doc.ReplaceText("{vendor}", "");
                }

                var panitia = _repository.getPersonilPengadaan(Id);
                var tablePanitia = doc.AddTable(panitia.Count(), 1);
                Border WhiteBorder = new Border(BorderStyle.Tcbs_none, 0, 0, Color.White);
                tablePanitia.SetBorder(TableBorderType.Bottom, WhiteBorder);
                tablePanitia.SetBorder(TableBorderType.Left, WhiteBorder);
                tablePanitia.SetBorder(TableBorderType.Right, WhiteBorder);
                tablePanitia.SetBorder(TableBorderType.Top, WhiteBorder);
                tablePanitia.SetBorder(TableBorderType.InsideV, WhiteBorder);
                tablePanitia.SetBorder(TableBorderType.InsideH, WhiteBorder);
                int rowIndex2 = 0;
                foreach (var item in panitia)
                {
                    tablePanitia.Rows[rowIndex2].Cells[0].Paragraphs.First().Append((rowIndex2 + 1) + ". " + item.Nama);
                    tablePanitia.Rows[rowIndex2].Cells[0].Paragraphs.First().FontSize(11).Font(new FontFamily("Calibri"));
                    tablePanitia.Rows[rowIndex2].Cells[0].Width = 500;
                    rowIndex2++;
                }

                tablePanitia.Alignment = Alignment.left;
                foreach (var paragraph in doc.Paragraphs)
                {
                    paragraph.FindAll("{panitia}").ForEach(index => paragraph.InsertTableBeforeSelf(tablePanitia));

                }
                doc.ReplaceText("{panitia}", "");
                // End Panitia


                //tambah tabel persetujuan tahapan
                var table3 = await getTablePersetujuan(pengadaan.Id, EStatusPengadaan.KLARIFIKASILANJUTAN, doc);

                table3.Alignment = Alignment.center;
                //table.AutoFit = AutoFit.Contents;

                foreach (var paragraph in doc.Paragraphs)
                {
                    paragraph.FindAll("{table3}").ForEach(index => paragraph.InsertTableBeforeSelf(table3));

                }
                doc.ReplaceText("{table3}", "");
                //end

                doc.SaveAs(OutFileNama);
                streamx.Close();
            }
            catch
            {
                streamx.Close();
            }
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new FileStream(OutFileNama, FileMode.Open);
            result.Content = new StreamContent(stream);
            //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.wordprocessingml.document");

            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = outputFileName
            };

            return result;
        }
        //public async Task<HttpResponseMessage> BerkasKlarfikasiLanjutan(Guid Id)
        //{
        //    var pengadaan = _repository.GetPengadaan(Id, UserId(), 0);
        //    var jadwalKlarifikasi = _repository.getPelaksanaanKlarifikasi(Id, UserId());
        //    string fileName = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Template\BERITA ACARA RAPAT KLARIFIKASI DAN NEGOSIASI LANJUTAN.docx";
        //    var BeritaAcara = _repository.getBeritaAcaraByTipe(Id, TipeBerkas.BeritaAcaraKlarifikasiLanjutan, UserId());
        //    string outputFileName = "Berkas-Klarifikasi-lanjutan-" + (BeritaAcara == null ? "" : BeritaAcara.NoBeritaAcara.Replace("/", "-")) + "-" + DateTime.Now.ToString("dd-MM-yy") + ".docx";

        //    string OutFileNama = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Temp\" + outputFileName;

        //    var streamx = new FileStream(fileName, FileMode.Open);

        //    var doc = DocX.Load(streamx);

        //    try
        //    {


        //        doc.ReplaceText("{pengadaan_name}", pengadaan.Judul == null ? "" : pengadaan.Judul);
        //        doc.ReplaceText("{pengadaan_name_judul}", pengadaan.Judul == null ? "" : pengadaan.Judul.ToUpper());
        //        doc.ReplaceText("{nomor_berita_acara}", BeritaAcara == null ? "" : BeritaAcara.NoBeritaAcara == null ? "" : BeritaAcara.NoBeritaAcara);
        //        doc.ReplaceText("{pengadaan_unit_pemohon}", pengadaan.UnitKerjaPemohon == null ? "" : pengadaan.UnitKerjaPemohon);

        //        doc.ReplaceText("{tempat_tanggal}", "...............," + (BeritaAcara == null ? "................" :
        //                (BeritaAcara.tanggal.Value.Day + " " + Common.ConvertNamaBulan(BeritaAcara.tanggal.Value.Month) + " " +
        //                BeritaAcara.tanggal.Value.Year)));

        //        doc.ReplaceText("{pengadaan_jadwal_hari}", BeritaAcara == null ? "" :
        //               Common.ConvertHari(BeritaAcara.tanggal.Value.Day));
        //        doc.ReplaceText("{pengadaan_jadwal_tanggal}", BeritaAcara == null ? "" : BeritaAcara.tanggal.Value.Day + " " + Common.ConvertNamaBulan(BeritaAcara.tanggal.Value.Month) +
        //              " " + BeritaAcara.tanggal.Value.Year);
        //        var kandidat = _repository.GetVendorsKlarifikasiByPengadaanId(Id);
        //        var table = doc.AddTable(kandidat.Count(), 1);
        //        Border BlankBorder = new Border(BorderStyle.Tcbs_none, 0, 0, Color.White);
        //        table.SetBorder(TableBorderType.Bottom, BlankBorder);
        //        table.SetBorder(TableBorderType.Left, BlankBorder);
        //        table.SetBorder(TableBorderType.Right, BlankBorder);
        //        table.SetBorder(TableBorderType.Top, BlankBorder);
        //        table.SetBorder(TableBorderType.InsideV, BlankBorder);
        //        table.SetBorder(TableBorderType.InsideH, BlankBorder);

        //        int rowIndex = 0;
        //        foreach (var item in kandidat)
        //        {
        //            table.Rows[rowIndex].Cells[0].Paragraphs.First().Append((rowIndex + 1) + ". " + item.Nama);
        //            table.Rows[rowIndex].Cells[0].Paragraphs.First().FontSize(11).Font(new FontFamily("Calibri"));
        //            table.Rows[rowIndex].Cells[0].Width = 550;
        //            rowIndex++;
        //        }

        //        table.Alignment = Alignment.center;
        //        foreach (var paragraph in doc.Paragraphs)
        //        {
        //            paragraph.FindAll("{vendor}").ForEach(index => paragraph.InsertTableBeforeSelf(table));

        //        }
        //        doc.ReplaceText("{vendor}", "");


        //        //tambah tabel persetujuan tahapan
        //        var table3 = await getTablePersetujuan(pengadaan.Id, EStatusPengadaan.KLARIFIKASILANJUTAN, doc);

        //        table3.Alignment = Alignment.center;
        //        //table.AutoFit = AutoFit.Contents;

        //        foreach (var paragraph in doc.Paragraphs)
        //        {
        //            paragraph.FindAll("{table3}").ForEach(index => paragraph.InsertTableBeforeSelf(table3));

        //        }
        //        doc.ReplaceText("{table3}", "");
        //        //end

        //        doc.SaveAs(OutFileNama);
        //        streamx.Close();
        //    }
        //    catch
        //    {
        //        streamx.Close();
        //    }
        //    HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
        //    var stream = new FileStream(OutFileNama, FileMode.Open);
        //    result.Content = new StreamContent(stream);
        //    //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        //    result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.wordprocessingml.document");

        //    result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
        //    {
        //        FileName = outputFileName
        //    };

        //    return result;
        //}




        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public async Task<HttpResponseMessage> BerkasPenilaian(Guid Id)
        {
            var pengadaan = _repository.GetPengadaan(Id, UserId(), 0);
            var jadwalKlarifikasi = _repository.getPelaksanaanKlarifikasi(Id, UserId());
            string fileName = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Template\Berita Acara Penilaian new.docx";
            var BeritaAcara = _repository.getBeritaAcaraByTipe(Id, TipeBerkas.BeritaAcaraPenilaian, UserId());
            string outputFileName = "Berkas-Penilaian" + (BeritaAcara == null ? "" : BeritaAcara.NoBeritaAcara.Replace("/", "-")) + "-" + DateTime.Now.ToString("dd-MM-yy") + ".docx";

            string OutFileNama = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Temp\" + outputFileName;

            var streamx = new FileStream(fileName, FileMode.Open);

            var doc = DocX.Load(streamx);

            try
            {
                doc.ReplaceText("{pengadaan_name}", pengadaan.Judul == null ? "" : pengadaan.Judul);
                doc.ReplaceText("{pengadaan_name_judul}", pengadaan.Judul == null ? "" : pengadaan.Judul.ToUpper());
                doc.ReplaceText("{nomor_berita_acara}", BeritaAcara == null ? "" : BeritaAcara.NoBeritaAcara == null ? "" : BeritaAcara.NoBeritaAcara);
                doc.ReplaceText("{pengadaan_unit_pemohon}", pengadaan.UnitKerjaPemohon == null ? "" : pengadaan.UnitKerjaPemohon);

                doc.ReplaceText("{tempat_tanggal}", "...............," + (BeritaAcara == null ? "................" :
                        (BeritaAcara.tanggal.Value.Day + " " + Common.ConvertNamaBulan(BeritaAcara.tanggal.Value.Month) + " " +
                        BeritaAcara.tanggal.Value.Year)));

                doc.ReplaceText("{pengadaan_jadwal_hari}", BeritaAcara == null ? "" :
                       Common.ConvertHari(BeritaAcara.tanggal.Value.Day));
                doc.ReplaceText("{pengadaan_jadwal_tanggal}", BeritaAcara == null ? "" : BeritaAcara.tanggal.Value.Day + " " + Common.ConvertNamaBulan(BeritaAcara.tanggal.Value.Month) +
                      " " + BeritaAcara.tanggal.Value.Year);

                var kandidat = _repository.GetVendorsKlarifikasiByPengadaanId2(Id);
                if (kandidat.Count() > 0)
                {
                    var table = doc.AddTable(kandidat.Count(), 1);
                    Border BlankBorder = new Border(BorderStyle.Tcbs_none, 0, 0, Color.White);
                    table.SetBorder(TableBorderType.Bottom, BlankBorder);
                    table.SetBorder(TableBorderType.Left, BlankBorder);
                    table.SetBorder(TableBorderType.Right, BlankBorder);
                    table.SetBorder(TableBorderType.Top, BlankBorder);
                    table.SetBorder(TableBorderType.InsideV, BlankBorder);
                    table.SetBorder(TableBorderType.InsideH, BlankBorder);

                    int rowIndex = 0;
                    foreach (var item in kandidat)
                    {
                        table.Rows[rowIndex].Cells[0].Paragraphs.First().Append((rowIndex + 1) + ". " + item.Nama);
                        table.Rows[rowIndex].Cells[0].Paragraphs.First().FontSize(11).Font(new FontFamily("Calibri"));
                        table.Rows[rowIndex].Cells[0].Width = 550;
                        rowIndex++;
                    }

                    table.Alignment = Alignment.left;
                    foreach (var paragraph in doc.Paragraphs)
                    {
                        paragraph.FindAll("{vendor}").ForEach(index => paragraph.InsertTableBeforeSelf(table));
                    }
                    doc.ReplaceText("{vendor}", "");
                }

                var panitia = _repository.getPersonilPengadaan(Id);
                var tablePanitia = doc.AddTable(panitia.Count(), 1);
                Border WhiteBorder = new Border(BorderStyle.Tcbs_none, 0, 0, Color.White);
                tablePanitia.SetBorder(TableBorderType.Bottom, WhiteBorder);
                tablePanitia.SetBorder(TableBorderType.Left, WhiteBorder);
                tablePanitia.SetBorder(TableBorderType.Right, WhiteBorder);
                tablePanitia.SetBorder(TableBorderType.Top, WhiteBorder);
                tablePanitia.SetBorder(TableBorderType.InsideV, WhiteBorder);
                tablePanitia.SetBorder(TableBorderType.InsideH, WhiteBorder);
                int rowIndex2 = 0;
                foreach (var item in panitia)
                {
                    tablePanitia.Rows[rowIndex2].Cells[0].Paragraphs.First().Append((rowIndex2 + 1) + ". " + item.Nama);
                    tablePanitia.Rows[rowIndex2].Cells[0].Paragraphs.First().FontSize(11).Font(new FontFamily("Calibri"));
                    tablePanitia.Rows[rowIndex2].Cells[0].Width = 500;
                    rowIndex2++;
                }

                tablePanitia.Alignment = Alignment.left;
                foreach (var paragraph in doc.Paragraphs)
                {
                    paragraph.FindAll("{panitia}").ForEach(index => paragraph.InsertTableBeforeSelf(tablePanitia));

                }
                doc.ReplaceText("{panitia}", "");
                // End Panitia


                //tambah tabel persetujuan tahapan
                var table3 = await getTablePersetujuan(pengadaan.Id, EStatusPengadaan.PENILAIAN, doc);

                table3.Alignment = Alignment.center;
                //table.AutoFit = AutoFit.Contents;

                foreach (var paragraph in doc.Paragraphs)
                {
                    paragraph.FindAll("{table3}").ForEach(index => paragraph.InsertTableBeforeSelf(table3));

                }
                doc.ReplaceText("{table3}", "");
                //end

                doc.SaveAs(OutFileNama);
                streamx.Close();
            }
            catch
            {
                streamx.Close();
            }
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new FileStream(OutFileNama, FileMode.Open);
            result.Content = new StreamContent(stream);
            //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.wordprocessingml.document");

            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = outputFileName
            };

            return result;
        }
        //public async Task< HttpResponseMessage> BerkasPenilaian(Guid Id)
        //{
        //    ViewPengadaan pengadaan = this._repository.GetPengadaan(Id, base.UserId(), 0);
        //    string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Template\Berita Acara Penilaian new.docx";
        //    string str2 = "BA-Penilaian-" + base.UserId().ToString() + "-" + DateTime.Now.ToString("dd-MM-yy") + ".docx";
        //    string filename = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Temp\" + str2;
        //    FileStream stream = new FileStream(path, FileMode.Open);
        //    try
        //    {
        //        BeritaAcara acara = this._repository.getBeritaAcaraByTipe(Id, TipeBerkas.BeritaAcaraPenilaian, base.UserId());
        //        DocX cx = DocX.Load(stream);
        //        cx.ReplaceText("{pengadaan_name}", (pengadaan.Judul == null) ? "" : pengadaan.Judul, false, RegexOptions.None, null, null, MatchFormattingOptions.SubsetMatch);
        //        cx.ReplaceText("{pengadaan_name_judul}", (pengadaan.Judul == null) ? "" : pengadaan.Judul.ToUpper(), false, RegexOptions.None, null, null, MatchFormattingOptions.SubsetMatch);
        //        cx.ReplaceText("{nomor_berita_acara}", (acara == null) ? "" : acara.NoBeritaAcara, false, RegexOptions.None, null, null, MatchFormattingOptions.SubsetMatch);
        //        cx.ReplaceText("{pengadaan_unit_pemohon}", (pengadaan.UnitKerjaPemohon == null) ? "" : pengadaan.UnitKerjaPemohon, false, RegexOptions.None, null, null, MatchFormattingOptions.SubsetMatch);
        //        cx.ReplaceText("{tempat_tanggal}", "...............," + (acara == null ? "................" : Common.ConvertNamaBulan(acara.tanggal.Value.Month)));
        //        cx.ReplaceText("{pengadaan_jadwal_hari}", (acara == null) ? "" : Common.ConvertHari(acara.tanggal.Value.Day), false, RegexOptions.None, null, null, MatchFormattingOptions.SubsetMatch);
        //        cx.ReplaceText("{pengadaan_jadwal_tanggal}", (acara == null) ? "" : string.Concat(new object[] { acara.tanggal.Value.Day, " ", Common.ConvertNamaBulan(acara.tanggal.Value.Month), " ", acara.tanggal.Value.Year }), false, RegexOptions.None, null, null, MatchFormattingOptions.SubsetMatch);
        //        List<VWPembobotanPengadaan> list = this._repository.getKriteriaPembobotan(Id);
        //        string text = ((from d in list
        //                        where d.NamaKreteria.Contains("Harga")
        //                        select d).FirstOrDefault<VWPembobotanPengadaan>() == null) ? "0" : (from d in list
        //                                                                                            where d.NamaKreteria.Contains("Harga")
        //                                                                                            select d).FirstOrDefault<VWPembobotanPengadaan>().Bobot.ToString();
        //        cx.ReplaceText("{harga}", text + "%", false, RegexOptions.None, null, null, MatchFormattingOptions.SubsetMatch);
        //        string str5 = ((from d in list
        //                        where d.NamaKreteria.Contains("Teknis")
        //                        select d).FirstOrDefault<VWPembobotanPengadaan>() == null) ? "0" : (from d in list
        //                                                                                            where d.NamaKreteria.Contains("Teknis")
        //                                                                                            select d).FirstOrDefault<VWPembobotanPengadaan>().Bobot.ToString();
        //        cx.ReplaceText("{teknis}", str5 + "%", false, RegexOptions.None, null, null, MatchFormattingOptions.SubsetMatch);
        //        List<VWRekananPenilaian> source = this._repository.getKandidatPengadaan2(Id, base.UserId());
        //        Table table = cx.AddTable(4, source.Count<VWRekananPenilaian>() + 3);
        //        Border BlankBorder = new Border(BorderStyle.Tcbs_single, BorderSize.one, 0, Color.Black);
        //        table.SetBorder(TableBorderType.Bottom, BlankBorder);
        //        table.SetBorder(TableBorderType.Left, BlankBorder);
        //        table.SetBorder(TableBorderType.Right, BlankBorder);
        //        table.SetBorder(TableBorderType.Top, BlankBorder);
        //        table.SetBorder(TableBorderType.InsideV, BlankBorder);
        //        table.SetBorder(TableBorderType.InsideH, BlankBorder);

        //        table.Rows[0].Cells[0].Paragraphs.First<Paragraph>().Append("No");
        //        table.Rows[0].Cells[0].Paragraphs.First<Paragraph>().FontSize(11.0).Font(new FontFamily("Calibri"));
        //        table.Rows[0].Cells[0].Width = 10.0;
        //        table.Rows[0].Cells[1].Paragraphs.First<Paragraph>().Append("Faktor");
        //        table.Rows[0].Cells[1].Paragraphs.First<Paragraph>().FontSize(11.0).Font(new FontFamily("Calibri"));
        //        table.Rows[0].Cells[2].Paragraphs.First<Paragraph>().Append("Bobot");
        //        table.Rows[0].Cells[2].Paragraphs.First<Paragraph>().FontSize(11.0).Font(new FontFamily("Calibri"));
        //        int num = 3;
        //        foreach (VWRekananPenilaian penilaian in source)
        //        {
        //            table.Rows[0].Cells[num].Paragraphs.First<Paragraph>().Append("Nilai " + penilaian.NamaVendor);
        //            table.Rows[0].Cells[num].Paragraphs.First<Paragraph>().FontSize(11.0).Font(new FontFamily("Calibri"));
        //            num++;
        //        }
        //        table.Rows[1].Cells[0].Paragraphs.First<Paragraph>().Append("1");
        //        table.Rows[1].Cells[0].Paragraphs.First<Paragraph>().FontSize(11.0).Font(new FontFamily("Calibri"));
        //        table.Rows[1].Cells[0].Width = 10.0;
        //        table.Rows[1].Cells[1].Paragraphs.First<Paragraph>().Append("Teknis");
        //        table.Rows[1].Cells[1].Paragraphs.First<Paragraph>().FontSize(11.0).Font(new FontFamily("Calibri"));
        //        table.Rows[1].Cells[2].Paragraphs.First<Paragraph>().Append(str5);
        //        table.Rows[1].Cells[2].Paragraphs.First<Paragraph>().FontSize(11.0).Font(new FontFamily("Calibri"));
        //        num = 3;
        //        foreach (VWRekananPenilaian penilaian in source)
        //        {
        //            VWPembobotanPengadaanVendor vendor = (from d in this._repository.getPembobtanPengadaanVendor(Id, penilaian.VendorId.Value, base.UserId())
        //                                                  where d.NamaKreteria.Contains("Teknis")
        //                                                  select d).FirstOrDefault<VWPembobotanPengadaanVendor>();
        //            table.Rows[1].Cells[num].Paragraphs.First<Paragraph>().Append((vendor == null) ? "-" : vendor.Nilai.Value.ToString());
        //            table.Rows[1].Cells[num].Paragraphs.First<Paragraph>().FontSize(11.0).Font(new FontFamily("Calibri"));
        //            num++;
        //        }
        //        table.Rows[2].Cells[0].Paragraphs.First<Paragraph>().Append("2");
        //        table.Rows[2].Cells[0].Paragraphs.First<Paragraph>().FontSize(11.0).Font(new FontFamily("Calibri"));
        //        table.Rows[2].Cells[0].Width = 10.0;
        //        table.Rows[2].Cells[1].Paragraphs.First<Paragraph>().Append("Harga");
        //        table.Rows[2].Cells[1].Paragraphs.First<Paragraph>().FontSize(11.0).Font(new FontFamily("Calibri"));
        //        table.Rows[2].Cells[2].Paragraphs.First<Paragraph>().Append(text);
        //        table.Rows[2].Cells[2].Paragraphs.First<Paragraph>().FontSize(11.0).Font(new FontFamily("Calibri"));
        //        num = 3;
        //        foreach (VWRekananPenilaian penilaian in source)
        //        {
        //            VWPembobotanPengadaanVendor vendor2 = (from d in this._repository.getPembobtanPengadaanVendor(Id, penilaian.VendorId.Value, base.UserId())
        //                                                   where d.NamaKreteria.Contains("Harga")
        //                                                   select d).FirstOrDefault<VWPembobotanPengadaanVendor>();
        //            table.Rows[2].Cells[num].Paragraphs.First<Paragraph>().Append((vendor2 == null) ? "-" : vendor2.Nilai.Value.ToString());
        //            table.Rows[2].Cells[num].Paragraphs.First<Paragraph>().FontSize(11.0).Font(new FontFamily("Calibri"));
        //            num++;
        //        }
        //        table.Rows[3].Cells[0].Paragraphs.First<Paragraph>().Append("");
        //        table.Rows[3].Cells[0].Paragraphs.First<Paragraph>().FontSize(11.0).Font(new FontFamily("Calibri"));
        //        table.Rows[3].Cells[0].Width = 10.0;
        //        table.Rows[3].Cells[1].Paragraphs.First<Paragraph>().Append("");
        //        table.Rows[3].Cells[1].Paragraphs.First<Paragraph>().FontSize(11.0).Font(new FontFamily("Calibri"));
        //        table.Rows[3].Cells[2].Paragraphs.First<Paragraph>().Append("Score");
        //        table.Rows[3].Cells[2].Paragraphs.First<Paragraph>().FontSize(11.0).Font(new FontFamily("Calibri"));
        //        num = 3;
        //        foreach (VWRekananPenilaian penilaian in source)
        //        {
        //            List<VWPembobotanPengadaanVendor> list3 = this._repository.getPembobtanPengadaanVendor(Id, penilaian.VendorId.Value, base.UserId());
        //            int num2 = ((from d in list3
        //                         where d.NamaKreteria.Contains("Harga")
        //                         select d).FirstOrDefault<VWPembobotanPengadaanVendor>() == null) ? 0 : (from d in list3
        //                                                                                                 where d.NamaKreteria.Contains("Harga")
        //                                                                                                 select d).FirstOrDefault<VWPembobotanPengadaanVendor>().Nilai.Value;
        //            int num3 = ((from d in list3
        //                         where d.NamaKreteria.Contains("Harga")
        //                         select d).FirstOrDefault<VWPembobotanPengadaanVendor>() == null) ? 0 : (from d in list3
        //                                                                                                 where d.NamaKreteria.Contains("Harga")
        //                                                                                                 select d).FirstOrDefault<VWPembobotanPengadaanVendor>().Bobot.Value;
        //            int num4 = ((from d in list3
        //                         where d.NamaKreteria.Contains("Teknis")
        //                         select d).FirstOrDefault<VWPembobotanPengadaanVendor>() == null) ? 0 : (from d in list3
        //                                                                                                 where d.NamaKreteria.Contains("Teknis")
        //                                                                                                 select d).FirstOrDefault<VWPembobotanPengadaanVendor>().Nilai.Value;
        //            int num5 = ((from d in list3
        //                         where d.NamaKreteria.Contains("Teknis")
        //                         select d).FirstOrDefault<VWPembobotanPengadaanVendor>() == null) ? 0 : (from d in list3
        //                                                                                                 where d.NamaKreteria.Contains("Teknis")
        //                                                                                                 select d).FirstOrDefault<VWPembobotanPengadaanVendor>().Bobot.Value;
        //            int num6 = (num2 * num3) / 100;
        //            int num7 = (num4 * num5) / 100;
        //            int num8 = num6 + num7;
        //            table.Rows[3].Cells[num].Paragraphs.First<Paragraph>().Append(num8.ToString());
        //            table.Rows[3].Cells[num].Paragraphs.First<Paragraph>().FontSize(11.0).Font(new FontFamily("Calibri"));
        //            num++;
        //        }
        //        table.Alignment = Alignment.center;
        //        //using (IEnumerator<Paragraph> enumerator2 = cx.Paragraphs.GetEnumerator())
        //        //{
        //        //    while (enumerator2.MoveNext())
        //        //    {
        //        //        Action<int> action = null;
        //        //        Paragraph paragraph = enumerator2.Current;
        //        //        if (action == null)
        //        //        {
        //        //            action = delegate(int index)
        //        //            {
        //        //                paragraph.InsertTableBeforeSelf(table);
        //        //            };
        //        //        }
        //        //        paragraph.FindAll("{penilaian}").ForEach(action);
        //        //    }
        //        //}
        //        System.IO.MemoryStream ms2 = new System.IO.MemoryStream();
        //        DocX doc2 = DocX.Create(ms2);

        //        doc2.PageLayout.Orientation = Novacode.Orientation.Landscape;
        //        Paragraph p = doc2.InsertParagraph();
        //        p.Append("Lampiran").Bold();
        //        p.Alignment = Alignment.left;
        //        p.AppendLine();
        //        Table t = doc2.InsertTable(table);
        //        cx.InsertSection();
        //        cx.InsertDocument(doc2);
        //        //cx.ReplaceText("{penilaian}", "", false, RegexOptions.None, null, null, MatchFormattingOptions.SubsetMatch);

        //        //tambah tabel persetujuan tahapan
        //        var table3 = await getTablePersetujuan(pengadaan.Id, EStatusPengadaan.PENILAIAN, cx);

        //        table3.Alignment = Alignment.center;
        //        //table.AutoFit = AutoFit.Contents;

        //        foreach (var paragraph in cx.Paragraphs)
        //        {
        //            paragraph.FindAll("{table3}").ForEach(index => paragraph.InsertTableBeforeSelf(table3));

        //        }
        //        cx.ReplaceText("{table3}", "");
        //        //end

        //        cx.SaveAs(filename);
        //        stream.Close();
        //    }
        //    catch
        //    {
        //        stream.Close();
        //    }
        //    HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.OK);
        //    FileStream content = new FileStream(filename, FileMode.Open);
        //    message.Content = new StreamContent(content);
        //    message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        //    ContentDispositionHeaderValue value2 = new ContentDispositionHeaderValue("attachment")
        //    {
        //        FileName = str2
        //    };
        //    message.Content.Headers.ContentDisposition = value2;
        //    return message;
        //}

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public async Task<HttpResponseMessage> BerkasPemenang(Guid Id)
        {
            var pengadaan = _repository.GetPengadaan(Id, UserId(), 0);
            var jadwalKlarifikasi = _repository.getPelaksanaanKlarifikasi(Id, UserId());
            var jadwalPemenang = _repository.getPelaksanaanPemenang(Id, UserId());
            string fileName = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Template\NOTA BERSAMA Usulan Pemenang.docx";
            string outputFileName = "BA-Pemenang-" + pengadaan.NoPengadaan.Replace("/", "-") + "-" + DateTime.Now.ToString("dd-MM-yy") + ".docx";

            string OutFileNama = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Temp\" + outputFileName;
            System.IO.MemoryStream ms2 = new System.IO.MemoryStream();
            var docM = DocX.Create(ms2);

            string fileNameDisposisi = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Template\NOTA BERSAMA Usulan Pemenang Disposisi.docx";
            //var streamxDisposisi = new FileStream(fileNameDisposisi, FileMode.Open);
            var streamx = new FileStream(fileName, FileMode.Open);
            try
            {
                var doc = DocX.Load(streamx);
                var BeritaAcara = _repository.getBeritaAcaraByTipe(Id, TipeBerkas.BeritaAcaraPenentuanPemenang, UserId());

                doc.ReplaceText("{pengadaan_name_judul}", pengadaan.Judul == null ? "" : pengadaan.Judul.ToUpper());
                doc.ReplaceText("{pengadaan_jadwal_tanggal}", BeritaAcara == null ? "" : BeritaAcara.tanggal.Value.Day + " " + Common.ConvertNamaBulan(BeritaAcara.tanggal.Value.Month) +
                      " " + BeritaAcara.tanggal.Value.Year);
                doc.ReplaceText("{nomor_berita_acara}", BeritaAcara == null ? "" : BeritaAcara.NoBeritaAcara);

                // Tambah Tabel Kandidat Pemenang
                var tblPemenang = await getPemenangPengadaan(Id, docM);
                tblPemenang.Alignment = Alignment.left;
                foreach (var paragraph in doc.Paragraphs)
                {
                    paragraph.FindAll("{tabel_pemenang}").ForEach(index => paragraph.InsertTableBeforeSelf(tblPemenang));

                }
                doc.ReplaceText("{tabel_pemenang}", "");

                // Tambah Tabel Disposisi
                var tblDisposisi = await getTableDisposisi(pengadaan.Id, EStatusPengadaan.PEMENANG, docM);
                tblDisposisi.Alignment = Alignment.center;
                foreach (var paragraph in doc.Paragraphs)
                {
                    paragraph.FindAll("{table_disposisi}").ForEach(index => paragraph.InsertTableBeforeSelf(tblDisposisi));

                }
                doc.ReplaceText("{table_disposisi}", "");

                // Tambah Tabel Persetujuan Tahapan
                var table3 = await getTablePersetujuan(pengadaan.Id, EStatusPengadaan.PEMENANG, doc);

                table3.Alignment = Alignment.center;
                //table.AutoFit = AutoFit.Contents;

                foreach (var paragraph in doc.Paragraphs)
                {
                    paragraph.FindAll("{tabel_persetujuan}").ForEach(index => paragraph.InsertTableBeforeSelf(table3));

                }
                doc.ReplaceText("{tabel_persetujuan}", "");
                //end

                docM.InsertDocument(doc);
                streamx.Close();
            }
            catch { streamx.Close(); }
            docM.SaveAs(OutFileNama);
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new FileStream(OutFileNama, FileMode.Open);
            result.Content = new StreamContent(stream);
            //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.wordprocessingml.document");

            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = outputFileName
            };

            return result;
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public HttpResponseMessage BerkasSPK2(string Judul, string Tanggal_SPK, string Nilai_SPK, string Vendor)
        {
            //var pengadaan = _repository.GetPengadaan(Id, UserId(), 0);
            //var jadwalKlarifikasi = _repository.getPelaksanaanKlarifikasi(Id, UserId());
            //var jadwalPemenang = _repository.getPelaksanaanPemenang(Id, UserId());
            string fileName = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Template\SPK2.docx";
            string outputFileName = "BA-SPK-" + Judul + "-" + DateTime.Now.ToString("dd-MM-yy") + ".docx";

            string OutFileNama = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Temp\" + outputFileName;

            // var streamx = new FileStream(fileName, FileMode.Open);

            // var doc = DocX.Load(streamx);

            System.IO.MemoryStream ms2 = new System.IO.MemoryStream();
            var docM = DocX.Create(ms2);

            //var pemenangx = _repository.getPemenangPengadaan(Id, UserId());
            //foreach (var item in pemenangx)
            //{
            var streamx = new FileStream(fileName, FileMode.Open);
            //var BeritaAcara = _repository.getBeritaAcaraByTipeandVendor(Id, TipeBerkas.SuratPerintahKerja, item.VendorId.Value, UserId());

            try
            {
                var doc = DocX.Load(streamx);
                doc.ReplaceText("{judul_spk}", Judul == null ? "" : Judul);
                //doc.ReplaceText("{pengadaan_name_judul}", pengadaan.Judul == null ? "" : pengadaan.Judul.ToUpper());
                //doc.ReplaceText("{nomor_berita_acara}", BeritaAcara == null ? "" : BeritaAcara.NoBeritaAcara);
                //doc.ReplaceText("{pengadaan_unit_pemohon}", pengadaan.UnitKerjaPemohon == null ? "" : pengadaan.UnitKerjaPemohon);
                doc.ReplaceText("{tanggal_spk}", "...............," + Tanggal_SPK == null ? "................" : Tanggal_SPK);

                //doc.ReplaceText("{pengadaan_jadwal_hari}", BeritaAcara.tanggal == null ? "" :
                //       Common.ConvertHari(BeritaAcara.tanggal.Value.Day));
                //doc.ReplaceText("{pengadaan_jadwal_tanggal}", BeritaAcara.tanggal.Value.Day + " " + Common.ConvertNamaBulan(BeritaAcara.tanggal.Value.Month) +
                //      " " + BeritaAcara.tanggal.Value.Year);


                //var pemenang = _repository.getPemenangPengadaan(Vendor);
                var vendor = _repository.GetVendorByName(Vendor.ToString());
                doc.ReplaceText("{vendor}", Vendor == null ? "" : Vendor);
                doc.ReplaceText("{nilai_spk}", Nilai_SPK == null ? "" : Nilai_SPK);
                doc.ReplaceText("{alamat}", vendor == null ? "" : vendor.Alamat.ToString());
                doc.ReplaceText("{terbilang}", Nilai_SPK == null ? "" : MyConverter.Terbilang(Nilai_SPK.ToString()) + " Rupiah");
                // doc.SaveAs(OutFileNama);
                docM.InsertSection();

                docM.InsertDocument(doc); //doc.SaveAs(OutFileNama);
                streamx.Close();
                // streamx.Close();
            }
            catch
            {
                streamx.Close();
            }
            //}
            docM.SaveAs(OutFileNama);
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new FileStream(OutFileNama, FileMode.Open);
            result.Content = new StreamContent(stream);
            //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.wordprocessingml.document");

            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = outputFileName
            };

            return result;
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public HttpResponseMessage BerkasSPK(Guid Id)
        {
            var pengadaan = _repository.GetPengadaan(Id, UserId(), 0);
            _log.Debug("var Pengadaan = {0}", pengadaan);
            var jadwalKlarifikasi = _repository.getPelaksanaanKlarifikasi(Id, UserId());
            _log.Debug("var jadwalKlarifikasi = {0}", jadwalKlarifikasi);
            var jadwalPemenang = _repository.getPelaksanaanPemenang(Id, UserId());
            _log.Debug("var jadwalPemenang = {0}", jadwalPemenang);
            string fileName = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Template\SPK.docx";
            _log.Debug("var fileName = {0}", fileName);
            string outputFileName = "BA-SPK-" + pengadaan.NoPengadaan.Replace("/", "-") + "-" + DateTime.Now.ToString("dd-MM-yy") + ".docx";
            _log.Debug("var outputFileName = {0}", outputFileName);

            string OutFileNama = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Temp\" + outputFileName;
            _log.Debug("var OutFileNama = {0}", OutFileNama);

            // var streamx = new FileStream(fileName, FileMode.Open);

            // var doc = DocX.Load(streamx);

            System.IO.MemoryStream ms2 = new System.IO.MemoryStream();
            var docM = DocX.Create(ms2);
            _log.Debug("var docM = {0}", docM);

            var pemenangx = _repository.getPemenangPengadaan(Id, UserId());
            _log.Debug("var pemenangx = {0}", pemenangx);
            foreach (var item in pemenangx)
            {
                var streamx = new FileStream(fileName, FileMode.Open);
                var BeritaAcara = _repository.getBeritaAcaraByTipeandVendor(Id, TipeBerkas.SuratPerintahKerja, item.VendorId.Value, UserId());

                try
                {
                    var doc = DocX.Load(streamx);
                    doc.ReplaceText("{pengadaan_name}", pengadaan.Judul == null ? "" : pengadaan.Judul);
                    doc.ReplaceText("{pengadaan_name_judul}", pengadaan.Judul == null ? "" : pengadaan.Judul.ToUpper());
                    doc.ReplaceText("{nomor_berita_acara}", BeritaAcara == null ? "" : BeritaAcara.NoBeritaAcara);
                    doc.ReplaceText("{pengadaan_unit_pemohon}", pengadaan.UnitKerjaPemohon == null ? "" : pengadaan.UnitKerjaPemohon);
                    //doc.ReplaceText("{tempat_tanggal}", "...............," + BeritaAcara == null ? "................" :
                    //        (BeritaAcara.tanggal.Value.Day + " " + Common.ConvertNamaBulan(BeritaAcara.tanggal.Value.Month) + " " +
                    //        BeritaAcara.tanggal.Value.Year));

                    if (BeritaAcara == null)
                    {
                        doc.ReplaceText("{tempat_tanggal}", "................");
                        doc.ReplaceText("{pengadaan_jadwal_hari}", "");
                        doc.ReplaceText("{pengadaan_jadwal_tanggal}", "");
                    }
                    else
                    {
                        doc.ReplaceText("{tempat_tanggal}", (BeritaAcara.tanggal.Value.Day + " " + Common.ConvertNamaBulan(BeritaAcara.tanggal.Value.Month) + " " +
                            BeritaAcara.tanggal.Value.Year));
                        doc.ReplaceText("{pengadaan_jadwal_hari}", Common.ConvertHari(BeritaAcara.tanggal.Value.Day));
                        doc.ReplaceText("{pengadaan_jadwal_tanggal}", BeritaAcara.tanggal.Value.Day + " " + Common.ConvertNamaBulan(BeritaAcara.tanggal.Value.Month) +
                          " " + BeritaAcara.tanggal.Value.Year);
                    }

                    //doc.ReplaceText("{pengadaan_jadwal_hari}", BeritaAcara.tanggal == null ? "" :
                    //       Common.ConvertHari(BeritaAcara.tanggal.Value.Day));
                    //doc.ReplaceText("{pengadaan_jadwal_tanggal}", BeritaAcara.tanggal.Value.Day + " " + Common.ConvertNamaBulan(BeritaAcara.tanggal.Value.Month) +
                    //      " " + BeritaAcara.tanggal.Value.Year);


                    // var pemenang = _repository.getPemenangPengadaan(Id, UserId());
                    var vendor = _repository.GetVendorById(item.VendorId.Value);
                    doc.ReplaceText("{kandidat_pemenang}", item.NamaVendor);
                    doc.ReplaceText("{total_pengadaan}", item.total == null ? "" : item.total.Value.ToString("C", MyConverter.formatCurrencyIndo()));
                    doc.ReplaceText("{alamat}", vendor.Alamat.ToString());
                    doc.ReplaceText("{terbilang}", item.total == null ? "" : MyConverter.Terbilang(item.total.Value.ToString()) + " Rupiah");
                    // doc.SaveAs(OutFileNama);
                    //docM.InsertSection();

                    docM.InsertDocument(doc); //doc.SaveAs(OutFileNama);
                    streamx.Close();
                    // streamx.Close();
                }
                catch
                {
                    streamx.Close();
                }
            }
            docM.SaveAs(OutFileNama);
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            _log.Debug("var result = {0}", result);
            var stream = new FileStream(OutFileNama, FileMode.Open);
            _log.Debug("var stream = {0}", stream);
            result.Content = new StreamContent(stream);
            //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.wordprocessingml.document");

            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = outputFileName
            };
            _log.Debug("var result = {0}", result);

            return result;
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public HttpResponseMessage LembarDisposisi(Guid Id)
        {
            var pengadaan = _repository.GetPengadaan(Id, UserId(), 0);
            var jadwalKlarifikasi = _repository.getPelaksanaanKlarifikasi(Id, UserId());
            var jadwalPemenang = _repository.getPelaksanaanPemenang(Id, UserId());
            string fileName = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Template\LEMBAR DISPOSISI.docx";

            string outputFileName = "Lembar-Disposisi-" + UserId().ToString() + "-" + DateTime.Now.ToString("dd-MM-yy") + ".docx";

            string OutFileNama = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Temp\" + outputFileName;

            var streamx = new FileStream(fileName, FileMode.Open);

            var doc = DocX.Load(streamx);

            try
            {
                doc.ReplaceText("{judul_pengadaan}", pengadaan.Judul == null ? "" : pengadaan.Judul);
                doc.ReplaceText("{no_pengadaan}", pengadaan.NoPengadaan == null ? "" : pengadaan.NoPengadaan);
                doc.ReplaceText("{unit_pemohon}", pengadaan.UnitKerjaPemohon == null ? "" : pengadaan.UnitKerjaPemohon);
                doc.ReplaceText("{region}", pengadaan.Region == null ? "" : pengadaan.Region);

                if (jadwalPemenang != null)
                {
                    doc.ReplaceText("{tanggal}", jadwalPemenang.Mulai.Value.Day.ToString() +
                        " " + Common.ConvertNamaBulan(jadwalPemenang.Mulai.Value.Month) +
                        " " + jadwalPemenang.Mulai.Value.Year.ToString());
                }
                else
                {
                    doc.ReplaceText("{tanggal}", "");
                }


                doc.SaveAs(OutFileNama);
                streamx.Close();
            }
            catch
            {
                streamx.Close();
            }
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new FileStream(OutFileNama, FileMode.Open);
            result.Content = new StreamContent(stream);
            //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.wordprocessingml.document");

            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = outputFileName
            };

            return result;
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public HttpResponseMessage CetakRKSKlarfikasi(Guid Id)
        {
            var pengadaan = _repository.GetPengadaan(Id, UserId(), 0);
            var jadwalKlarifikasi = _repository.getPelaksanaanKlarifikasi(Id, UserId());
            string fileName = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Template\Klarifikasi-Pemenang.docx";

            string outputFileName = "Cetak-RKS-Klarifikasi" + UserId().ToString() + "-" + DateTime.Now.ToString("dd-MM-yy") + ".docx";

            string OutFileNama = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Temp\" + outputFileName;

            using (var streamx = new FileStream(fileName, FileMode.Open))
            {

                var doc = DocX.Load(streamx);//.Create(OutFileNama);
                doc.PageLayout.Orientation = Novacode.Orientation.Landscape;
                doc.ReplaceText("{pengadaan_name_judul}", pengadaan.Judul == null ? "" : pengadaan.Judul.ToUpper());
                doc.ReplaceText("{nomor_berita_acara}", pengadaan.NoPengadaan == null ? "" : pengadaan.NoPengadaan);
                doc.ReplaceText("{pengadaan_unit_pemohon}", pengadaan.UnitKerjaPemohon == null ? "" : pengadaan.UnitKerjaPemohon);


                var oVWRKSVendors = _repository.getRKSKlarifikasiPenilaian(pengadaan.Id, UserId());
                var table = doc.AddTable(oVWRKSVendors.hps.Count() + 1, (oVWRKSVendors.vendors.Count * 2) + 7);
                Border BlankBorder = new Border(BorderStyle.Tcbs_single, BorderSize.one, 0, Color.Black);
                table.SetBorder(TableBorderType.Bottom, BlankBorder);
                table.SetBorder(TableBorderType.Left, BlankBorder);
                table.SetBorder(TableBorderType.Right, BlankBorder);
                table.SetBorder(TableBorderType.Top, BlankBorder);
                table.SetBorder(TableBorderType.InsideV, BlankBorder);
                table.SetBorder(TableBorderType.InsideH, BlankBorder);

                int indexRow = 0;
                table.Rows[indexRow].Cells[0].Paragraphs.First().Append("NO");
                table.Rows[indexRow].Cells[0].Width = 10;
                table.Rows[indexRow].Cells[1].Paragraphs.First().Append("Nama");
                table.Rows[indexRow].Cells[2].Paragraphs.First().Append("Item");
                table.Rows[indexRow].Cells[3].Paragraphs.First().Append("Satuan");
                table.Rows[indexRow].Cells[4].Paragraphs.First().Append("Jumlah");
                table.Rows[indexRow].Cells[5].Paragraphs.First().Append("Keterangan Item");
                table.Rows[indexRow].Cells[6].Paragraphs.First().Append("Harga HPS");
                int headerCol = 7;
                foreach (var item in oVWRKSVendors.vendors)
                {
                    table.Rows[indexRow].Cells[headerCol].Paragraphs.First().Append(item.nama);
                    headerCol++;
                    table.Rows[indexRow].Cells[headerCol].Paragraphs.First().Append("Total (" + item.nama + ")");
                    headerCol++;
                }
                indexRow++;
                var itemlast = oVWRKSVendors.hps.Last();
                decimal? subtotal = 0;
                int no = 1;
                foreach (var item in oVWRKSVendors.hps)
                {
                    var currentGroup = item.grup;
                    var index = oVWRKSVendors.hps.IndexOf(item);
                    if (item.harga != null && item.jumlah != null)
                        subtotal += item.harga * item.jumlah;
                    if (item.Equals(itemlast))
                    {
                        table.Rows[indexRow].Cells[0].Paragraphs.First().Append("");
                        table.Rows[indexRow].Cells[0].Width = 10;
                    }
                    else
                    {

                        if (item.jumlah > 0)
                        {
                            table.Rows[indexRow].Cells[0].Paragraphs.First().Append(no.ToString());
                            table.Rows[indexRow].Cells[0].Width = 10;
                            no++;
                        }
                        else
                        {
                            table.Rows[indexRow].Cells[0].Paragraphs.First().Append("");
                            table.Rows[indexRow].Cells[0].Width = 10;
                        }
                    }
                    //Regex example #1 "<.*?>"
                    string dekripsi = "";
                    if (item.item != null) dekripsi = Regex.Replace(item.item, @"<.*?>", string.Empty);
                    //Regex example #2
                    // string result2 = Regex.Replace(dekripsi, @"<[^>].+?>", "");
                    table.Rows[indexRow].Cells[1].Paragraphs.First().Append(item.judul != "undefined" ? item.judul : "");
                    table.Rows[indexRow].Cells[2].Paragraphs.First().Append(dekripsi);
                    table.Rows[indexRow].Cells[3].Paragraphs.First().Append(item.satuan);
                    table.Rows[indexRow].Cells[4].Paragraphs.First().Append(item.jumlah == null ? "" : item.jumlah.Value.ToString("C", MyConverter.formatCurrencyIndoTanpaSymbol()));
                    table.Rows[indexRow].Cells[5].Paragraphs.First().Append(item.keteranganItem);
                    if ((item.harga != null && item.jumlah != null) || item.isTotal == 1)
                    {
                        //decimal harga = item.harga.Value * item.jumlah.Value;
                        table.Rows[indexRow].Cells[6].Paragraphs.First().Append(item.harga.Value.ToString("C", MyConverter.formatCurrencyIndo()));
                    }
                    else table.Rows[indexRow].Cells[6].Paragraphs.First().Append("");
                    /*sub totol asumsi kalo next group nya beda*/
                    if (index < oVWRKSVendors.hps.Count() - 1)
                    {
                        if (currentGroup != null && currentGroup != oVWRKSVendors.hps[index + 1].grup)
                        {
                            table.Rows[indexRow].Cells[5].Paragraphs.First().Append("Sub Total");
                            table.Rows[indexRow].Cells[6].Paragraphs.First().Append(subtotal.Value.ToString("C", MyConverter.formatCurrencyIndo()));
                            subtotal = 0;
                            // no = 1;
                        }
                    }
                    int nexCol = 7;
                    foreach (var itemx in oVWRKSVendors.vendors)
                    {
                        if (itemx.items.Where(d => d.Id == item.Id) != null)
                        {
                            var itemxx = itemx.items.Where(d => d.Id == item.Id).FirstOrDefault();
                            var harga = itemxx == null ? "" : itemxx.harga == null ? "" : itemxx.harga.Value.ToString("C", MyConverter.formatCurrencyIndo());
                            var jumlah = itemxx == null ? "" : itemxx.jumlah == null ? "" : itemxx.jumlah.Value.ToString();
                            if (index < oVWRKSVendors.hps.Count() - 1)
                            {
                                if (currentGroup != oVWRKSVendors.hps[index + 1].grup)
                                {
                                    if (itemxx != null)
                                    {
                                        if (itemxx.subtotal != null)
                                            table.Rows[indexRow].Cells[nexCol].Paragraphs.First().Append(itemxx.subtotal.totalGroup != null ? itemxx.subtotal.totalGroup.Value.ToString("C", MyConverter.formatCurrencyIndo()) : "");
                                    }
                                }
                                else
                                {
                                    if (jumlah != string.Empty && harga != string.Empty)
                                    {
                                        table.Rows[indexRow].Cells[nexCol].Paragraphs.First().Append(harga);
                                    }

                                }
                            }
                            if (item.isTotal == 1)
                            {
                                table.Rows[indexRow].Cells[nexCol].Paragraphs.First().Append(harga);
                            }

                            nexCol++;

                            if (jumlah != string.Empty && harga != string.Empty)
                            {
                                var total = itemxx.jumlah * itemxx.harga;
                                table.Rows[indexRow].Cells[nexCol].Paragraphs.First().Append(total.Value.ToString("C", MyConverter.formatCurrencyIndo()));
                            }
                            else table.Rows[indexRow].Cells[nexCol].Paragraphs.First().Append("");
                            /*sub totol asumsi kalo next group nya beda*/

                        }
                        else table.Rows[indexRow].Cells[nexCol].Paragraphs.First().Append("");

                        nexCol++;
                    }

                    indexRow++;
                }
                // Insert table at index where tag #TABLE# is in document.
                //doc.InsertTable(table);
                foreach (var paragraph in doc.Paragraphs)
                {
                    paragraph.FindAll("{tabel}").ForEach(index => paragraph.InsertTableAfterSelf((table)));
                }
                //Remove tag
                doc.ReplaceText("{tabel}", "");

                doc.SaveAs(OutFileNama);
                streamx.Close();
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                var stream = new FileStream(OutFileNama, FileMode.Open);
                result.Content = new StreamContent(stream);
                //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.wordprocessingml.document");

                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = outputFileName
                };
                return result;
            }


        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public HttpResponseMessage CetakRKSKlarfikasiXls(Guid Id)
        {
            var pengadaan = _repository.GetPengadaan(Id, UserId(), 0);
            var jadwalKlarifikasi = _repository.getPelaksanaanKlarifikasi(Id, UserId());
            string outputFileName = "Cetak-RKS-Klarifikasi" + UserId().ToString() + "-" + DateTime.Now.ToString("dd-MM-yy") + ".xls";

            var Judul = pengadaan.Judul == null ? "" : pengadaan.Judul;
            var UnitPemohon = pengadaan.UnitKerjaPemohon == null ? "" : pengadaan.UnitKerjaPemohon;


            var oVWRKSVendors = _repository.getRKSKlarifikasiPenilaian(pengadaan.Id, UserId());
            int i = 0;

            var ms = new System.IO.MemoryStream();
            try
            {
                using (var sl = new SpreadsheetLight.SLDocument())
                {
                    sl.SetCellValue(1, 1, "Proyek");
                    sl.SetCellValue(1, 2, UnitPemohon);
                    sl.SetCellValue(2, 1, "Judul");
                    sl.SetCellValue(2, 2, Judul);
                    sl.SetCellValue(3, 1, "Penawaran Harga Klarifikasi dan Negoisasi Rekanan sebagai berikut:");
                    var rowNum = 4;
                    //write header
                    sl.SetCellValue(rowNum, 1, "No");
                    sl.SetCellValue(rowNum, 2, "Nama");
                    sl.SetCellValue(rowNum, 3, "Item");
                    sl.SetCellValue(rowNum, 4, "Satuan");
                    sl.SetCellValue(rowNum, 5, "Jumlah");
                    sl.SetCellValue(rowNum, 6, "Keterangan Item");
                    sl.SetCellValue(rowNum, 7, "Harga HPS");

                    int headerCol = 8;
                    foreach (var item in oVWRKSVendors.vendors)
                    {
                        sl.SetCellValue(rowNum, headerCol, item.nama);
                        headerCol++;
                        sl.SetCellValue(rowNum, headerCol, "Total (" + item.nama + ")");
                        headerCol++;
                    }
                    rowNum++;
                    //write data
                    var itemlast = oVWRKSVendors.hps.Last();
                    decimal? subtotal = 0;
                    int no = 1;
                    foreach (var item in oVWRKSVendors.hps)
                    {
                        var currentGroup = item.grup;
                        var index = oVWRKSVendors.hps.IndexOf(item);
                        if (item.harga != null && item.jumlah != null)
                            subtotal += item.harga * item.jumlah;
                        if (item.Equals(itemlast))
                        {
                            sl.SetCellValue(rowNum, 1, "");
                        }
                        else
                        {
                            if (item.jumlah > 0)
                            {
                                sl.SetCellValue(rowNum, 1, no);
                                no++;
                            }
                            else
                            {
                                sl.SetCellValue(rowNum, 1, "");
                            }
                        }
                        string dekripsi = "";
                        if (item.item != null) dekripsi = Regex.Replace(item.item, @"<.*?>", string.Empty);
                        if (i == 1)
                        {
                            rowNum++;
                            sl.SetCellValue(rowNum, 2, item.judul != "undefined" ? item.judul : "");
                            i = 0;
                        }
                        else
                        {
                            sl.SetCellValue(rowNum, 2, item.judul != "undefined" ? item.judul : "");
                        }
                        sl.SetCellValue(rowNum, 3, dekripsi);
                        sl.SetCellValue(rowNum, 4, item.satuan);
                        if (item.jumlah == null) sl.SetCellValue(rowNum, 5, "");
                        else sl.SetCellValue(rowNum, 5, item.jumlah.Value);
                        sl.SetCellValue(rowNum, 6, item.keteranganItem);
                        if ((item.harga != null && item.jumlah != null) || item.isTotal == 1)
                        {
                            //decimal harga = item.harga.Value * item.jumlah.Value;
                            sl.SetCellValue(rowNum, 7, item.harga.Value);
                        }
                        else sl.SetCellValue(rowNum, 7, "");
                        /*sub totol asumsi kalo next group nya beda*/
                        if (index < oVWRKSVendors.hps.Count() - 1)
                        {
                            if (currentGroup != null && currentGroup != oVWRKSVendors.hps[index + 1].grup)
                            {
                                if (item.level != 0)
                                {
                                    rowNum++;
                                    sl.SetCellValue(rowNum, 6, "Sub Total");
                                    sl.SetCellValue(rowNum, 7, subtotal.Value.ToString("#.##"));
                                    subtotal = 0;
                                    rowNum--;
                                    i = 1;
                                }
                            }
                        }
                        int nextCol = 8;
                        foreach (var itemx in oVWRKSVendors.vendors)
                        {
                            if (itemx.items.Where(d => d.Id == item.Id) != null)
                            {
                                var itemxx = itemx.items.Where(d => d.Id == item.Id).FirstOrDefault();
                                var harga = itemxx == null ? "" : itemxx.harga == null ? "" : itemxx.harga.Value.ToString("#.##");
                                var jumlah = itemxx == null ? "" : itemxx.jumlah == null ? "" : itemxx.jumlah.Value.ToString("#.##");
                                //-----
                                if (jumlah != string.Empty && harga != string.Empty)
                                {
                                    sl.SetCellValue(rowNum, nextCol, harga);
                                }

                                if (index < oVWRKSVendors.hps.Count() - 1)
                                {
                                    if (currentGroup != oVWRKSVendors.hps[index + 1].grup)
                                    {
                                        if (itemxx.subtotal != null)
                                        {
                                            if (i == 1)
                                            {
                                                rowNum++;
                                                sl.SetCellValue(rowNum, nextCol, itemxx.subtotal.totalGroup != null ? itemxx.subtotal.totalGroup.Value.ToString("#.##") : "");
                                                rowNum--;
                                            }
                                            else
                                            {
                                                sl.SetCellValue(rowNum, nextCol, itemxx.subtotal.totalGroup != null ? itemxx.subtotal.totalGroup.Value.ToString("#.##") : "");
                                            }
                                        }
                                    }
                                    //else
                                    //{
                                    //    if (jumlah != string.Empty && harga != string.Empty)
                                    //    {
                                    //        sl.SetCellValue(rowNum, nextCol, harga);
                                    //    }

                                    //}
                                }
                                if (item.isTotal == 1)
                                {
                                    sl.SetCellValue(rowNum, nextCol, harga);
                                }

                                nextCol++;

                                if (jumlah != string.Empty && harga != string.Empty)
                                {
                                    var total = itemxx.jumlah * itemxx.harga;
                                    sl.SetCellValue(rowNum, nextCol, total.Value.ToString("#.##"));
                                }
                                else sl.SetCellValue(rowNum, nextCol, "");
                                /*sub totol asumsi kalo next group nya beda*/
                            }
                            else sl.SetCellValue(rowNum, nextCol, "");

                            nextCol++;
                        }
                        rowNum++;
                    }
                    sl.SetColumnWidth(1, 10.0);

                    sl.SaveAs(ms);
                }
            }
            catch { }

            ms.Position = 0;
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new StreamContent(ms);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = outputFileName
            };
            return result;
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public HttpResponseMessage CetakRKSKlarfikasiLanjutanXls(Guid Id)
        {
            var pengadaan = _repository.GetPengadaan(Id, UserId(), 0);
            var jadwalKlarifikasi = _repository.getPelaksanaanKlarifikasi(Id, UserId());
            string outputFileName = "Cetak-RKS-Klarifikasi-Lanjutan-" + UserId().ToString() + "-" + DateTime.Now.ToString("dd-MM-yy") + ".xls";

            var Judul = pengadaan.Judul == null ? "" : pengadaan.Judul;
            var UnitPemohon = pengadaan.UnitKerjaPemohon == null ? "" : pengadaan.UnitKerjaPemohon;


            var oVWRKSVendors = _repository.getRKSKlarifikasiLanjutanPenilaian(pengadaan.Id, UserId());
            int i = 0;

            var ms = new System.IO.MemoryStream();
            try
            {
                using (var sl = new SpreadsheetLight.SLDocument())
                {
                    sl.SetCellValue(1, 1, "Proyek");
                    sl.SetCellValue(1, 2, UnitPemohon);
                    sl.SetCellValue(2, 1, "Judul");
                    sl.SetCellValue(2, 2, Judul);
                    sl.SetCellValue(3, 1, "Penawaran Harga Klarifikasi dan Negoisasi Lanjutan Rekanan sebagai berikut:");
                    var rowNum = 4;
                    //write header
                    sl.SetCellValue(rowNum, 1, "No");
                    sl.SetCellValue(rowNum, 2, "Nama");
                    sl.SetCellValue(rowNum, 3, "Item");
                    sl.SetCellValue(rowNum, 4, "Satuan");
                    sl.SetCellValue(rowNum, 5, "Jumlah");
                    sl.SetCellValue(rowNum, 6, "Keterangan Item");
                    sl.SetCellValue(rowNum, 7, "Harga HPS");

                    int headerCol = 8;
                    foreach (var item in oVWRKSVendors.vendors)
                    {
                        sl.SetCellValue(rowNum, headerCol, item.nama);
                        headerCol++;
                        sl.SetCellValue(rowNum, headerCol, "Total (" + item.nama + ")");
                        headerCol++;
                    }
                    rowNum++;
                    //write data
                    var itemlast = oVWRKSVendors.hps.Last();
                    decimal? subtotal = 0;
                    int no = 1;
                    foreach (var item in oVWRKSVendors.hps)
                    {
                        var currentGroup = item.grup;
                        var index = oVWRKSVendors.hps.IndexOf(item);
                        if (item.harga != null && item.jumlah != null)
                            subtotal += item.harga * item.jumlah;
                        if (item.Equals(itemlast))
                        {
                            sl.SetCellValue(rowNum, 1, "");
                        }
                        else
                        {
                            if (item.jumlah > 0)
                            {
                                sl.SetCellValue(rowNum, 1, no);
                                no++;
                            }
                            else
                            {
                                sl.SetCellValue(rowNum, 1, "");
                            }
                        }
                        string dekripsi = "";
                        if (item.item != null) dekripsi = Regex.Replace(item.item, @"<.*?>", string.Empty);
                        if (i == 1)
                        {
                            rowNum++;
                            sl.SetCellValue(rowNum, 2, item.judul != "undefined" ? item.judul : "");
                            i = 0;
                        }
                        else
                        {
                            sl.SetCellValue(rowNum, 2, item.judul != "undefined" ? item.judul : "");
                        }
                        sl.SetCellValue(rowNum, 3, dekripsi);
                        sl.SetCellValue(rowNum, 4, item.satuan);
                        if (item.jumlah == null) sl.SetCellValue(rowNum, 5, "");
                        else sl.SetCellValue(rowNum, 5, item.jumlah.Value);
                        sl.SetCellValue(rowNum, 6, item.keteranganItem);
                        if ((item.harga != null && item.jumlah != null) || item.isTotal == 1)
                        {
                            //decimal harga = item.harga.Value * item.jumlah.Value;
                            sl.SetCellValue(rowNum, 7, item.harga.Value);
                        }
                        else sl.SetCellValue(rowNum, 7, "");
                        /*sub totol asumsi kalo next group nya beda*/
                        if (index < oVWRKSVendors.hps.Count() - 1)
                        {
                            if (currentGroup != null && currentGroup != oVWRKSVendors.hps[index + 1].grup)
                            {
                                if (item.level != 0)
                                {
                                    rowNum++;
                                    sl.SetCellValue(rowNum, 6, "Sub Total");
                                    sl.SetCellValue(rowNum, 7, subtotal.Value.ToString("#.##"));
                                    subtotal = 0;
                                    rowNum--;
                                    i = 1;
                                }
                            }
                        }
                        int nextCol = 8;
                        foreach (var itemx in oVWRKSVendors.vendors)
                        {
                            if (itemx.items.Where(d => d.Id == item.Id) != null)
                            {
                                var itemxx = itemx.items.Where(d => d.Id == item.Id).FirstOrDefault();
                                var harga = itemxx == null ? "" : itemxx.harga == null ? "" : itemxx.harga.Value.ToString("#.##");
                                var jumlah = itemxx == null ? "" : itemxx.jumlah == null ? "" : itemxx.jumlah.Value.ToString("#.##");
                                //-----
                                if (jumlah != string.Empty && harga != string.Empty)
                                {
                                    sl.SetCellValue(rowNum, nextCol, harga);
                                }

                                if (index < oVWRKSVendors.hps.Count() - 1)
                                {
                                    if (currentGroup != oVWRKSVendors.hps[index + 1].grup)
                                    {
                                        if (itemxx.subtotal != null)
                                        {
                                            if (i == 1)
                                            {
                                                rowNum++;
                                                sl.SetCellValue(rowNum, nextCol, itemxx.subtotal.totalGroup != null ? itemxx.subtotal.totalGroup.Value.ToString("#.##") : "");
                                                rowNum--;
                                            }
                                            else
                                            {
                                                sl.SetCellValue(rowNum, nextCol, itemxx.subtotal.totalGroup != null ? itemxx.subtotal.totalGroup.Value.ToString("#.##") : "");
                                            }
                                        }
                                    }
                                    //else
                                    //{
                                    //    if (jumlah != string.Empty && harga != string.Empty)
                                    //    {
                                    //        sl.SetCellValue(rowNum, nextCol, harga);
                                    //    }

                                    //}
                                }
                                if (item.isTotal == 1)
                                {
                                    sl.SetCellValue(rowNum, nextCol, harga);
                                }

                                nextCol++;

                                if (jumlah != string.Empty && harga != string.Empty)
                                {
                                    var total = itemxx.jumlah * itemxx.harga;
                                    sl.SetCellValue(rowNum, nextCol, total.Value.ToString("#.##"));
                                }
                                else sl.SetCellValue(rowNum, nextCol, "");
                                /*sub totol asumsi kalo next group nya beda*/
                            }
                            else sl.SetCellValue(rowNum, nextCol, "");

                            nextCol++;
                        }
                        rowNum++;
                    }
                    sl.SetColumnWidth(1, 10.0);

                    sl.SaveAs(ms);
                }
            }
            catch { }

            ms.Position = 0;
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new StreamContent(ms);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = outputFileName
            };
            return result;
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public HttpResponseMessage CetakRKSKlarfikasiVendor(Guid Id, int VendorId)
        {
            var pengadaan = _repository.GetPengadaan(Id, UserId(), 0);
            var jadwalKlarifikasi = _repository.getPelaksanaanKlarifikasi(Id, UserId());
            string fileName = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Template\Klarifikasi-Pemenang.docx";

            string outputFileName = "Cetak-RKS-Klarifikasi-" + UserId().ToString() + "-" + DateTime.Now.ToString("dd-MM-yy") + ".docx";

            string OutFileNama = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Temp\" + outputFileName;

            var streamx = new FileStream(fileName, FileMode.Open);
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            try
            {
                var doc = DocX.Load(streamx);//.Create(OutFileNama);
                doc.ReplaceText("{pengadaan_name_judul}", pengadaan.Judul == null ? "" : pengadaan.Judul.ToUpper());
                doc.ReplaceText("{nomor_berita_acara}", pengadaan.NoPengadaan == null ? "" : pengadaan.NoPengadaan);
                doc.ReplaceText("{pengadaan_unit_pemohon}", pengadaan.UnitKerjaPemohon == null ? "" : pengadaan.UnitKerjaPemohon);


                var oVWRKSVendors = _repository.getRKSKlarifikasiPenilaianVendor2(pengadaan.Id, UserId(), VendorId);
                var table = doc.AddTable(oVWRKSVendors.hps.Count() + 1, oVWRKSVendors.vendors.Count + 5);
                int no = 1;

                int indexRow = 0;
                table.Rows[indexRow].Cells[0].Paragraphs.First().Append("NO");
                table.Rows[indexRow].Cells[0].Width = 10;
                table.Rows[indexRow].Cells[1].Paragraphs.First().Append("Nama");
                table.Rows[indexRow].Cells[2].Paragraphs.First().Append("Item");
                table.Rows[indexRow].Cells[3].Paragraphs.First().Append("Jumlah");
                table.Rows[indexRow].Cells[4].Paragraphs.First().Append("Harga HPS");
                int headerCol = 5;
                foreach (var item in oVWRKSVendors.vendors)
                {
                    table.Rows[indexRow].Cells[headerCol].Paragraphs.First().Append(item.nama);
                    headerCol++;
                }
                indexRow++;
                var itemlast = oVWRKSVendors.hps.Last();
                foreach (var item in oVWRKSVendors.hps)
                {
                    if (item.Equals(itemlast))
                    {
                        table.Rows[indexRow].Cells[0].Paragraphs.First().Append("");
                        table.Rows[indexRow].Cells[0].Width = 10;
                    }
                    else
                    {

                        if (item.jumlah > 0)
                        {
                            table.Rows[indexRow].Cells[0].Paragraphs.First().Append(no.ToString());
                            table.Rows[indexRow].Cells[0].Width = 10;
                            no++;
                        }
                        else
                        {
                            table.Rows[indexRow].Cells[0].Paragraphs.First().Append("");
                            table.Rows[indexRow].Cells[0].Width = 10;
                        }
                    }
                    table.Rows[indexRow].Cells[1].Paragraphs.First().Append(item.judul != "undefined" ? item.judul : "");
                    //Regex example #1 "<.*?>"
                    string dekripsi = Regex.Replace(item.item, @"<.*?>", string.Empty);
                    //Regex example #2
                    // string result2 = Regex.Replace(dekripsi, @"<[^>].+?>", "");
                    table.Rows[indexRow].Cells[2].Paragraphs.First().Append(dekripsi);
                    table.Rows[indexRow].Cells[3].Paragraphs.First().Append(item.jumlah == null ? "" : item.jumlah.Value.ToString("C", MyConverter.formatCurrencyIndoTanpaSymbol()));
                    //table.Rows[indexRow].Cells[4].Paragraphs.First().Append(item.harga == null ? "" : item.harga.Value.ToString("C", MyConverter.formatCurrencyIndo()));
                    if ((item.harga != null && item.jumlah != null) || item.isTotal == 1)
                    {
                        //decimal harga = item.harga.Value * item.jumlah.Value;
                        table.Rows[indexRow].Cells[4].Paragraphs.First().Append(item.harga.Value.ToString("C", MyConverter.formatCurrencyIndo()));
                    }
                    else table.Rows[indexRow].Cells[4].Paragraphs.First().Append("");
                    int nexCol = 5;
                    foreach (var itemx in oVWRKSVendors.vendors)
                    {
                        if (itemx.items.Where(d => d.Id == item.Id) != null)
                        {
                            if (item.harga != null && item.harga != null)
                            {
                                table.Rows[indexRow].Cells[nexCol].Paragraphs.First().Append(itemx.items.Where(d => d.Id == item.Id).FirstOrDefault() == null ? "" : itemx.items.Where(d => d.Id == item.Id).FirstOrDefault().harga.Value.ToString("C", MyConverter.formatCurrencyIndo()));
                            }
                            else table.Rows[indexRow].Cells[nexCol].Paragraphs.First().Append("");
                        }
                        else table.Rows[indexRow].Cells[nexCol].Paragraphs.First().Append("");

                        nexCol++;
                    }

                    indexRow++;
                }
                // Insert table at index where tag #TABLE# is in document.
                //doc.InsertTable(table);
                foreach (var paragraph in doc.Paragraphs)
                {
                    paragraph.FindAll("{tabel}").ForEach(index => paragraph.InsertTableAfterSelf((table)));
                }
                //Remove tag
                doc.ReplaceText("{tabel}", "");

                doc.SaveAs(OutFileNama);
                streamx.Close();

                var stream = new FileStream(OutFileNama, FileMode.Open);
                result.Content = new StreamContent(stream);
                //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.wordprocessingml.document");

                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = outputFileName
                };
            }
            catch
            {
                streamx.Close();
                result.Content = new StringContent("Internal Server Error");

            }
            return result;
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public HttpResponseMessage CetakRKSXLSKlarfikasiVendor(Guid Id, int VendorId)
        {
            var pengadaan = _repository.GetPengadaan(Id, UserId(), 0);
            var jadwalKlarifikasi = _repository.getPelaksanaanKlarifikasi(Id, UserId());

            string outputFileName = "Cetak-RKS-Klarifikasi-" + UserId().ToString() + "-" + DateTime.Now.ToString("dd-MM-yy") + ".xlsx";

            var Judul = pengadaan.Judul == null ? "" : pengadaan.Judul.ToUpper();
            var NoPengadaan = pengadaan.NoPengadaan == null ? "" : pengadaan.NoPengadaan;
            var UnitPemohon = pengadaan.UnitKerjaPemohon == null ? "" : pengadaan.UnitKerjaPemohon;


            var oVWRKSVendors = _repository.getRKSKlarifikasiPenilaianVendor2(pengadaan.Id, UserId(), VendorId);


            var ms = new System.IO.MemoryStream();
            try
            {
                using (var sl = new SpreadsheetLight.SLDocument())
                {
                    sl.SetCellValue(1, 1, "Proyek");
                    sl.SetCellValue(1, 2, UnitPemohon);
                    sl.SetCellValue(2, 1, "Judul");
                    sl.SetCellValue(2, 2, Judul);
                    sl.SetCellValue(3, 1, "Penawaran Harga Klarifikasi dan Negoisasi Rekanan sebagai berikut:");
                    var rowNum = 4;
                    //write header
                    sl.SetCellValue(rowNum, 1, "No");
                    sl.SetCellValue(rowNum, 2, "Item");
                    sl.SetCellValue(rowNum, 3, "Jumlah");
                    sl.SetCellValue(rowNum, 4, "Harga HPS");

                    int headerCol = 5;
                    foreach (var item in oVWRKSVendors.vendors)
                    {
                        sl.SetCellValue(rowNum, headerCol, item.nama);
                        headerCol++;
                    }
                    rowNum++;
                    //write data
                    var itemlast = oVWRKSVendors.hps.Last();
                    int no = 1;
                    foreach (var item in oVWRKSVendors.hps)
                    {
                        if (item.Equals(itemlast))
                        {
                            sl.SetCellValue(rowNum, 1, "");
                        }
                        else
                        {
                            if (item.jumlah > 0)
                            {
                                sl.SetCellValue(rowNum, 1, no);
                                no++;
                            }
                            else
                            {
                                sl.SetCellValue(rowNum, 1, "");
                            }
                        }
                        string dekripsi = Regex.Replace(item.item, @"<.*?>", string.Empty);
                        sl.SetCellValue(rowNum, 2, dekripsi);
                        if (item.harga == null) sl.SetCellValue(rowNum, 3, "");
                        else sl.SetCellValue(rowNum, 3, item.harga.Value);
                        if (item.jumlah == null) sl.SetCellValue(rowNum, 4, "");
                        else sl.SetCellValue(rowNum, 4, item.jumlah.Value);
                        int nextCol = 5;
                        foreach (var itemx in oVWRKSVendors.vendors)
                        {
                            if (itemx.items.Where(d => d.Id == item.Id) != null)
                            {
                                var itemxx = itemx.items.Where(d => d.Id == item.Id).FirstOrDefault();
                                if (itemxx == null)
                                {
                                    sl.SetCellValue(rowNum, nextCol, "");
                                }
                                else
                                {
                                    if (itemxx.harga == null) sl.SetCellValue(rowNum, nextCol, "");
                                    else sl.SetCellValue(rowNum, nextCol, itemxx.harga.Value);
                                }
                            }
                            else sl.SetCellValue(rowNum, nextCol, "");

                            nextCol++;
                        }
                        rowNum++;
                    }
                    sl.SetColumnWidth(1, 10.0);

                    sl.SaveAs(ms);
                }
            }
            catch { }

            ms.Position = 0;
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            //  var stream = new FileStream(ms, FileMode.Open);
            result.Content = new StreamContent(ms);
            //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = outputFileName
            };
            return result;
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public HttpResponseMessage CetakRKSPenilaianAll(Guid Id)
        {
            var pengadaan = _repository.GetPengadaan(Id, UserId(), 0);
            var jadwalKlarifikasi = _repository.getPelaksanaanKlarifikasi(Id, UserId());
            string fileName = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Template\Klarifikasi-Pemenang.docx";

            string outputFileName = "Cetak-RKS-Penilaian-" + UserId().ToString() + "-" + DateTime.Now.ToString("dd-MM-yy") + ".docx";

            string OutFileNama = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Temp\" + outputFileName;

            var streamx = new FileStream(fileName, FileMode.Open);

            var doc = DocX.Load(streamx);//.Create(OutFileNama);
            doc.PageLayout.Orientation = Novacode.Orientation.Landscape;
            doc.ReplaceText("{pengadaan_name_judul}", pengadaan.Judul == null ? "" : pengadaan.Judul.ToUpper());
            doc.ReplaceText("{nomor_berita_acara}", pengadaan.NoPengadaan == null ? "" : pengadaan.NoPengadaan);
            doc.ReplaceText("{pengadaan_unit_pemohon}", pengadaan.UnitKerjaPemohon == null ? "" : pengadaan.UnitKerjaPemohon);


            var oVWRKSVendors = _repository.getRKSPenilaian2Report(pengadaan.Id, UserId());
            var table = doc.AddTable(oVWRKSVendors.hps.Count() + 1, (oVWRKSVendors.vendors.Count * 2) + 7);
            Border BlankBorder = new Border(BorderStyle.Tcbs_single, BorderSize.one, 0, Color.Black);
            table.SetBorder(TableBorderType.Bottom, BlankBorder);
            table.SetBorder(TableBorderType.Left, BlankBorder);
            table.SetBorder(TableBorderType.Right, BlankBorder);
            table.SetBorder(TableBorderType.Top, BlankBorder);
            table.SetBorder(TableBorderType.InsideV, BlankBorder);
            table.SetBorder(TableBorderType.InsideH, BlankBorder);

            int indexRow = 0;
            table.Rows[indexRow].Cells[0].Paragraphs.First().Append("NO");
            table.Rows[indexRow].Cells[0].Width = 10;
            table.Rows[indexRow].Cells[1].Paragraphs.First().Append("Nama");
            table.Rows[indexRow].Cells[2].Paragraphs.First().Append("Item");
            table.Rows[indexRow].Cells[3].Paragraphs.First().Append("Satuan");
            table.Rows[indexRow].Cells[4].Paragraphs.First().Append("Jumlah");
            table.Rows[indexRow].Cells[5].Paragraphs.First().Append("Keterangan Item");
            table.Rows[indexRow].Cells[6].Paragraphs.First().Append("Harga HPS");
            int headerCol = 7;

            foreach (var item in oVWRKSVendors.vendors)
            {
                table.Rows[indexRow].Cells[headerCol].Paragraphs.First().Append(item.nama);
                headerCol++;
                table.Rows[indexRow].Cells[headerCol].Paragraphs.First().Append("Total (" + item.nama + ")");
                headerCol++;
            }
            indexRow++;
            int no = 1;
            decimal? subtotal = 0;
            var itemlast = oVWRKSVendors.hps.Last();
            foreach (var item in oVWRKSVendors.hps)
            {
                var currentGroup = item.grup;
                var index = oVWRKSVendors.hps.IndexOf(item);
                if (item.harga != null && item.jumlah != null)
                    subtotal += item.harga * item.jumlah;
                if (item.Equals(itemlast))
                {
                    table.Rows[indexRow].Cells[0].Paragraphs.First().Append("");
                    table.Rows[indexRow].Cells[0].Width = 10;
                }
                else
                {

                    if (item.jumlah > 0)
                    {
                        table.Rows[indexRow].Cells[0].Paragraphs.First().Append(no.ToString());
                        table.Rows[indexRow].Cells[0].Width = 10;
                        no++;
                    }
                    else
                    {
                        table.Rows[indexRow].Cells[0].Paragraphs.First().Append("");
                        table.Rows[indexRow].Cells[0].Width = 10;
                    }
                }
                //Regex example #1 "<.*?>"
                string dekripsi = "";
                if (item.item != null) dekripsi = Regex.Replace(item.item, @"<.*?>", string.Empty);
                //Regex example #2
                // string result2 = Regex.Replace(dekripsi, @"<[^>].+?>", "");
                table.Rows[indexRow].Cells[1].Paragraphs.First().Append(item.judul != "undefined" ? item.judul : "");
                table.Rows[indexRow].Cells[2].Paragraphs.First().Append(dekripsi);
                table.Rows[indexRow].Cells[3].Paragraphs.First().Append(item.satuan);
                table.Rows[indexRow].Cells[4].Paragraphs.First().Append(item.jumlah == null ? "" : item.jumlah.Value.ToString("C", MyConverter.formatCurrencyIndoTanpaSymbol()));
                table.Rows[indexRow].Cells[5].Paragraphs.First().Append(item.keteranganItem);
                if ((item.harga != null && item.jumlah != null) || item.isTotal == 1)
                {
                    //decimal harga = item.harga.Value * item.jumlah.Value;
                    table.Rows[indexRow].Cells[6].Paragraphs.First().Append(item.harga.Value.ToString("C", MyConverter.formatCurrencyIndo()));
                }
                else table.Rows[indexRow].Cells[6].Paragraphs.First().Append("");
                /*sub totol asumsi kalo next group nya beda*/
                if (index < oVWRKSVendors.hps.Count() - 1)
                {
                    if (currentGroup != null && currentGroup != oVWRKSVendors.hps[index + 1].grup)
                    {
                        table.Rows[indexRow].Cells[5].Paragraphs.First().Append("Sub Total");
                        table.Rows[indexRow].Cells[6].Paragraphs.First().Append(subtotal.Value.ToString("C", MyConverter.formatCurrencyIndo()));
                        subtotal = 0;
                        // no = 1;
                    }
                }
                int nexCol = 7;
                foreach (var itemx in oVWRKSVendors.vendors)
                {
                    if (itemx.items.Where(d => d.Id == item.Id) != null)
                    {
                        var itemxx = itemx.items.Where(d => d.Id == item.Id).FirstOrDefault();
                        var harga = itemxx == null ? "" : itemxx.harga == null ? "" : itemxx.harga.Value.ToString("C", MyConverter.formatCurrencyIndo());
                        var jumlah = itemxx == null ? "" : itemxx.jumlah == null ? "" : itemxx.jumlah.Value.ToString();
                        if (index < oVWRKSVendors.hps.Count() - 1)
                        {
                            if (currentGroup != oVWRKSVendors.hps[index + 1].grup)
                            {
                                if (itemxx != null)
                                {
                                    if (itemxx.subtotal != null)
                                        table.Rows[indexRow].Cells[nexCol].Paragraphs.First().Append(itemxx.subtotal.totalGroup != null ? itemxx.subtotal.totalGroup.Value.ToString("C", MyConverter.formatCurrencyIndo()) : "");
                                }
                            }
                            else
                            {
                                if (jumlah != string.Empty && harga != string.Empty)
                                {
                                    table.Rows[indexRow].Cells[nexCol].Paragraphs.First().Append(harga);
                                }

                            }
                        }
                        if (item.isTotal == 1)
                        {
                            table.Rows[indexRow].Cells[nexCol].Paragraphs.First().Append(harga);
                        }

                        nexCol++;

                        if (jumlah != string.Empty && harga != string.Empty)
                        {
                            var total = itemxx.jumlah * itemxx.harga;
                            table.Rows[indexRow].Cells[nexCol].Paragraphs.First().Append(total.Value.ToString("C", MyConverter.formatCurrencyIndo()));
                        }
                        else table.Rows[indexRow].Cells[nexCol].Paragraphs.First().Append("");
                        /*sub totol asumsi kalo next group nya beda*/

                    }
                    else table.Rows[indexRow].Cells[nexCol].Paragraphs.First().Append("");

                    nexCol++;
                }

                indexRow++;
            }
            // Insert table at index where tag #TABLE# is in document.
            //doc.InsertTable(table);
            foreach (var paragraph in doc.Paragraphs)
            {
                paragraph.FindAll("{tabel}").ForEach(index => paragraph.InsertTableAfterSelf((table)));
            }
            //Remove tag
            doc.ReplaceText("{tabel}", "");

            doc.SaveAs(OutFileNama);
            streamx.Close();
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new FileStream(OutFileNama, FileMode.Open);
            result.Content = new StreamContent(stream);
            //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.wordprocessingml.document");

            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = outputFileName
            };

            return result;
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public HttpResponseMessage CetakRKSPenilaianAllXls(Guid Id)
        {
            var pengadaan = _repository.GetPengadaan(Id, UserId(), 0);
            var jadwalKlarifikasi = _repository.getPelaksanaanKlarifikasi(Id, UserId());

            string outputFileName = "Cetak-RKS-Penilaian-" + UserId().ToString() + "-" + DateTime.Now.ToString("dd-MM-yy") + ".xlsx";

            var Judul = pengadaan.Judul == null ? "" : pengadaan.Judul.ToUpper();
            var NoPengadaan = pengadaan.NoPengadaan == null ? "" : pengadaan.NoPengadaan;
            var UnitPemohon = pengadaan.UnitKerjaPemohon == null ? "" : pengadaan.UnitKerjaPemohon;


            var oVWRKSVendors = _repository.getRKSPenilaian2Report(pengadaan.Id, UserId());
            int i = 0;

            var ms = new System.IO.MemoryStream();
            try
            {
                using (var sl = new SpreadsheetLight.SLDocument())
                {
                    sl.SetCellValue(1, 1, "Proyek");
                    sl.SetCellValue(1, 2, UnitPemohon);
                    sl.SetCellValue(2, 1, "Judul");
                    sl.SetCellValue(2, 2, Judul);
                    sl.SetCellValue(3, 1, "Penawaran Harga Klarifikasi dan Negoisasi Rekanan sebagai berikut:");
                    var rowNum = 4;
                    //write header
                    sl.SetCellValue(rowNum, 1, "No");
                    sl.SetCellValue(rowNum, 2, "Nama");
                    sl.SetCellValue(rowNum, 3, "Item");
                    sl.SetCellValue(rowNum, 4, "Satuan");
                    sl.SetCellValue(rowNum, 5, "Jumlah");
                    sl.SetCellValue(rowNum, 6, "Keterangan Item");
                    sl.SetCellValue(rowNum, 7, "Harga HPS");

                    int headerCol = 8;
                    foreach (var item in oVWRKSVendors.vendors)
                    {
                        sl.SetCellValue(rowNum, headerCol, item.nama);
                        headerCol++;
                        sl.SetCellValue(rowNum, headerCol, "Total (" + item.nama + ")");
                        headerCol++;
                    }
                    rowNum++;

                    //write data
                    var itemlast = oVWRKSVendors.hps.Last();
                    decimal? subtotal = 0;
                    int no = 1;
                    foreach (var item in oVWRKSVendors.hps)
                    {
                        var currentGroup = item.grup;
                        var index = oVWRKSVendors.hps.IndexOf(item);
                        if (item.harga != null && item.jumlah != null)
                            subtotal += item.harga * item.jumlah;
                        if (item.Equals(itemlast))
                        {
                            sl.SetCellValue(rowNum, 1, "");
                        }
                        else
                        {
                            if (item.jumlah > 0)
                            {
                                sl.SetCellValue(rowNum, 1, no);
                                no++;
                            }
                            else
                            {
                                sl.SetCellValue(rowNum, 1, "");
                            }
                        }
                        string dekripsi = "";
                        if (item.item != null) dekripsi = Regex.Replace(item.item, @"<.*?>", string.Empty);
                        if (i == 1)
                        {
                            rowNum++;
                            sl.SetCellValue(rowNum, 2, item.judul != "undefined" ? item.judul : "");
                            i = 0;
                        }
                        else
                        {
                            sl.SetCellValue(rowNum, 2, item.judul != "undefined" ? item.judul : "");
                        }
                        sl.SetCellValue(rowNum, 3, dekripsi);
                        sl.SetCellValue(rowNum, 4, item.satuan);
                        if (item.jumlah == null) sl.SetCellValue(rowNum, 5, "");
                        else sl.SetCellValue(rowNum, 5, item.jumlah.Value);
                        sl.SetCellValue(rowNum, 6, item.keteranganItem);
                        if ((item.harga != null && item.jumlah != null) || item.isTotal == 1)
                        {
                            //decimal harga = item.harga.Value * item.jumlah.Value;
                            sl.SetCellValue(rowNum, 7, item.harga.Value);
                        }
                        else sl.SetCellValue(rowNum, 7, "");
                        /*sub totol asumsi kalo next group nya beda*/
                        if (index < oVWRKSVendors.hps.Count() - 1)
                        {
                            if (currentGroup != null && currentGroup != oVWRKSVendors.hps[index + 1].grup)
                            {
                                if (item.level != 0)
                                {
                                    rowNum++;
                                    sl.SetCellValue(rowNum, 6, "Sub Total");
                                    sl.SetCellValue(rowNum, 7, subtotal.Value.ToString("#.##"));
                                    subtotal = 0;
                                    rowNum--;
                                    i = 1;
                                }
                            }
                        }
                        int nextCol = 8;
                        foreach (var itemx in oVWRKSVendors.vendors)
                        {
                            if (itemx.items.Where(d => d.Id == item.Id) != null)
                            {
                                var itemxx = itemx.items.Where(d => d.Id == item.Id).FirstOrDefault();
                                var harga = itemxx == null ? "" : itemxx.harga == null ? "" : itemxx.harga.Value.ToString("#.##");
                                var jumlah = itemxx == null ? "" : itemxx.jumlah == null ? "" : itemxx.jumlah.Value.ToString("#.##");
                                //-----
                                if (jumlah != string.Empty && harga != string.Empty)
                                {
                                    sl.SetCellValue(rowNum, nextCol, harga);
                                }

                                if (index < oVWRKSVendors.hps.Count() - 1)
                                {
                                    if (currentGroup != oVWRKSVendors.hps[index + 1].grup)
                                    {
                                        if (itemxx != null)
                                        {
                                            if (itemxx.subtotal != null)
                                            {
                                                if (i == 1)
                                                {
                                                    rowNum++;
                                                    sl.SetCellValue(rowNum, nextCol, itemxx.subtotal.totalGroup != null ? itemxx.subtotal.totalGroup.Value.ToString("#.##") : "");
                                                    rowNum--;
                                                }
                                                else
                                                {
                                                    sl.SetCellValue(rowNum, nextCol, itemxx.subtotal.totalGroup != null ? itemxx.subtotal.totalGroup.Value.ToString("#.##") : "");
                                                }
                                            }
                                        }
                                    }
                                    //else
                                    //{
                                    //    if (jumlah != string.Empty && harga != string.Empty)
                                    //    {
                                    //        sl.SetCellValue(rowNum, nextCol, harga);
                                    //    }

                                    //}
                                }
                                if (item.isTotal == 1)
                                {
                                    sl.SetCellValue(rowNum, nextCol, harga);
                                }

                                nextCol++;

                                if (jumlah != string.Empty && harga != string.Empty)
                                {
                                    var total = itemxx.jumlah * itemxx.harga;
                                    sl.SetCellValue(rowNum, nextCol, total.Value.ToString("#.##"));
                                }
                                else sl.SetCellValue(rowNum, nextCol, "");
                                /*sub totol asumsi kalo next group nya beda*/
                            }
                            else sl.SetCellValue(rowNum, nextCol, "");

                            nextCol++;
                        }
                        rowNum++;
                    }
                    sl.SetColumnWidth(1, 10.0);

                    sl.SaveAs(ms);
                }
            }
            catch { }

            ms.Position = 0;
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            //  var stream = new FileStream(ms, FileMode.Open);
            result.Content = new StreamContent(ms);
            //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = outputFileName
            };
            return result;
        }

        //Buat Word Create RKS New
        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public HttpResponseMessage CetakHPSNew(Guid Id)
        {
            var rkstemplate = _rksrepo.getRksTemplate(Id);
            //var jadwalKlarifikasi = _repository.getPelaksanaanKlarifikasi(Id, UserId());
            string fileName = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Template\template-hps-new.docx";

            string outputFileName = "Cetak-HPS" + UserId().ToString() + "-" + DateTime.Now.ToString("dd-MM-yy") + ".docx";

            string OutFileNama = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Temp\" + outputFileName;

            var streamx = new FileStream(fileName, FileMode.Open);
            try
            {
                var doc = DocX.Load(streamx);//.Create(OutFileNama);
                doc.ReplaceText("{judul}", rkstemplate.Title);
                doc.ReplaceText("{deskripsi}", rkstemplate.Description);
                doc.ReplaceText("{klasifikasi}", rkstemplate.Klasifikasi.ToString());
                doc.ReplaceText("{region}", rkstemplate.Region);

                var oVWRKSDetail = rkstemplate.RKSDetailTemplate;
                var table = doc.AddTable(oVWRKSDetail.Count() + 1, 7);


                int indexRow = 0;
                table.Rows[indexRow].Cells[0].Paragraphs.First().Append("Nama");
                table.Rows[indexRow].Cells[1].Paragraphs.First().Append("Item");
                table.Rows[indexRow].Cells[2].Paragraphs.First().Append("Satuan");
                table.Rows[indexRow].Cells[3].Paragraphs.First().Append("Jumlah");
                table.Rows[indexRow].Cells[3].Width = 10;
                table.Rows[indexRow].Cells[4].Paragraphs.First().Append("Hps Satuan");
                table.Rows[indexRow].Cells[5].Paragraphs.First().Append("Total");
                table.Rows[indexRow].Cells[6].Paragraphs.First().Append("Keterangan");
                indexRow++;
                decimal subtotal = 0;
                decimal totalall = 0;
                foreach (var item in oVWRKSDetail)
                {
                    if (item.level == 0)
                    {
                        table.Rows[indexRow].Cells[0].Paragraphs.First().Append(item.judul == null ? "" : item.judul.ToString());

                    }
                    else if (item.level == 1)
                    {
                        table.Rows[indexRow].Cells[1].Paragraphs.First().Append(item.item == null ? "" : Regex.Replace(item.item.ToString(), @"<.*?>", string.Empty));
                        table.Rows[indexRow].Cells[2].Paragraphs.First().Append(item.satuan == null ? "" : item.satuan.ToString());
                        table.Rows[indexRow].Cells[3].Paragraphs.First().Append(item.jumlah == null ? "" : item.jumlah.Value.ToString());
                        table.Rows[indexRow].Cells[3].Width = 10;
                        table.Rows[indexRow].Cells[4].Paragraphs.First().Append(item.hps == null ? "" : item.hps.Value.ToString("C", MyConverter.formatCurrencyIndo()));
                        decimal? total = item.jumlah * item.hps;
                        table.Rows[indexRow].Cells[5].Paragraphs.First().Append(total == null ? "" : total.Value.ToString("C", MyConverter.formatCurrencyIndo()));
                        table.Rows[indexRow].Cells[6].Paragraphs.First().Append(item.keterangan);
                        subtotal = subtotal + total.Value;
                        totalall = totalall + total.Value;
                    }
                    else if (item.level == 2)
                    {
                        table.Rows[indexRow].Cells[4].Paragraphs.First().Append("Sub Total");
                        table.Rows[indexRow].Cells[5].Paragraphs.First().Append(subtotal.ToString("C", MyConverter.formatCurrencyIndo()));
                        subtotal = 0;
                    }
                    indexRow++;
                }

                doc.ReplaceText("{total}", totalall.ToString("C", MyConverter.formatCurrencyIndo()));

                // Insert table at index where tag #TABLE# is in document.
                //doc.InsertTable(table);
                foreach (var paragraph in doc.Paragraphs)
                {
                    paragraph.FindAll("{tabel}").ForEach(index => paragraph.InsertTableAfterSelf((table)));
                }
                //Remove tag
                doc.ReplaceText("{tabel}", "");

                doc.SaveAs(OutFileNama);
                streamx.Close();
            }
            catch
            {
                streamx.Close();
            }

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new FileStream(OutFileNama, FileMode.Open);
            result.Content = new StreamContent(stream);
            //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.wordprocessingml.document");

            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = outputFileName
            };

            return result;
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        // Create RKS Excel New
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public HttpResponseMessage CetakHPSXLSNew(Guid Id)
        {
            var rkstemplate = _rksrepo.getRksTemplate(Id);
            var spc = new System.Data.DataTable("Jimbis");
            string outputFileName = "Cetak-HPS" + UserId().ToString() + "-" + DateTime.Now.ToString("dd-MM-yy") + ".xlsx";
            string OutFileNama = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Temp\" + outputFileName;

            var ms = new System.IO.MemoryStream();
            try
            {
                var oVWRKSDetail = rkstemplate.RKSDetailTemplate;

                using (var sl = new SpreadsheetLight.SLDocument())
                {
                    sl.SetCellValue(1, 1, "Judul");
                    sl.SetCellValue(1, 2, rkstemplate.Title);
                    sl.SetCellValue(2, 1, "Deskripsi");
                    sl.SetCellValue(2, 2, rkstemplate.Description);
                    sl.SetCellValue(3, 1, "Klasifikasi");
                    sl.SetCellValue(3, 2, rkstemplate.Klasifikasi.ToString());
                    sl.SetCellValue(4, 1, "Region");
                    sl.SetCellValue(4, 2, rkstemplate.Region);

                    var rowNum = 6;
                    //write header
                    sl.SetCellValue(rowNum, 1, "Nama");
                    sl.SetCellValue(rowNum, 2, "Item");
                    sl.SetCellValue(rowNum, 3, "Satuan");
                    sl.SetCellValue(rowNum, 4, "Jumlah");
                    sl.SetCellValue(rowNum, 5, "Hps");
                    sl.SetCellValue(rowNum, 6, "Total");
                    sl.SetCellValue(rowNum, 7, "Keterangan");
                    rowNum++;
                    //write data
                    decimal subtotal = 0;
                    decimal totalall = 0;
                    foreach (var item in oVWRKSDetail)
                    {
                        if (item.level == 0)
                        {
                            sl.SetCellValue(rowNum, 1, (item.judul));
                        }
                        else if (item.level == 1)
                        {
                            sl.SetCellValue(rowNum, 2, Regex.Replace(item.item.ToString(), @"<.*?>", string.Empty));
                            sl.SetCellValue(rowNum, 3, item.satuan);
                            if (item.jumlah == null) sl.SetCellValue(rowNum, 4, "");
                            else sl.SetCellValue(rowNum, 4, item.jumlah.Value);
                            if (item.hps == null) sl.SetCellValue(rowNum, 5, "");
                            else sl.SetCellValue(rowNum, 5, item.hps.Value.ToString("C", MyConverter.formatCurrencyIndo()));
                            decimal? total = item.jumlah * item.hps;
                            if (total == null) sl.SetCellValue(rowNum, 6, "");
                            else sl.SetCellValue(rowNum, 6, total.Value.ToString("C", MyConverter.formatCurrencyIndo()));
                            sl.SetCellValue(rowNum, 7, item.keterangan);
                            subtotal = subtotal + total.Value;
                            totalall = totalall + total.Value;
                        }
                        else if (item.level == 2)
                        {
                            sl.SetCellValue(rowNum, 5, "Sub Total");
                            sl.SetCellValue(rowNum, 6, subtotal.ToString("C", MyConverter.formatCurrencyIndo()));
                            subtotal = 0;
                        }
                        rowNum++;
                    }

                    sl.SetCellValue(5, 1, "Total HPS");
                    sl.SetCellValue(5, 2, totalall.ToString("C", MyConverter.formatCurrencyIndo()));

                    //add filter
                    sl.SetColumnWidth(4, 12.0);
                    sl.SetColumnWidth(5, 30.0);
                    sl.SetColumnWidth(6, 30.0);
                    sl.SaveAs(ms);
                }

            }
            catch
            {

            }

            ms.Position = 0;
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            //  var stream = new FileStream(ms, FileMode.Open);
            result.Content = new StreamContent(ms);
            //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = outputFileName
            };
            return result;
        }


        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public HttpResponseMessage CetakHPS(Guid Id)
        {
            var pengadaan = _repository.GetPengadaan(Id, UserId(), 0);
            //var jadwalKlarifikasi = _repository.getPelaksanaanKlarifikasi(Id, UserId());
            string fileName = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Template\template-hps.docx";

            string outputFileName = "Cetak-HPS" + UserId().ToString() + "-" + DateTime.Now.ToString("dd-MM-yy") + ".docx";

            string OutFileNama = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Temp\" + outputFileName;

            var streamx = new FileStream(fileName, FileMode.Open);
            try
            {
                var doc = DocX.Load(streamx);//.Create(OutFileNama);
                doc.ReplaceText("{pengadaan_name_judul}", pengadaan.Judul == null ? "" : pengadaan.Judul);
                doc.ReplaceText("{nomor_berita_acara}", pengadaan.NoPengadaan == null ? "" : pengadaan.NoPengadaan);
                doc.ReplaceText("{pengadaan_unit_pemohon}", pengadaan.UnitKerjaPemohon == null ? "" : pengadaan.UnitKerjaPemohon);

                var oVWRKSDetail = _repository.getRKSDetails(pengadaan.Id, UserId());
                var table = doc.AddTable(oVWRKSDetail.Count() + 1, 7);

                int indexRow = 0;
                table.Rows[indexRow].Cells[0].Paragraphs.First().Append("Nama");
                table.Rows[indexRow].Cells[1].Paragraphs.First().Append("Item");
                table.Rows[indexRow].Cells[2].Paragraphs.First().Append("Satuan");
                table.Rows[indexRow].Cells[3].Paragraphs.First().Append("Jumlah");
                table.Rows[indexRow].Cells[3].Width = 10;
                table.Rows[indexRow].Cells[4].Paragraphs.First().Append("Hps Satuan");
                table.Rows[indexRow].Cells[5].Paragraphs.First().Append("Total");
                table.Rows[indexRow].Cells[6].Paragraphs.First().Append("Keterangan");
                indexRow++;
                decimal subtotal = 0;
                decimal totalall = 0;
                foreach (var item in oVWRKSDetail)
                {
                    if (item.level == 0)
                    {
                        table.Rows[indexRow].Cells[0].Paragraphs.First().Append(item.judul == null ? "" : item.judul.ToString());

                    }
                    else if (item.level == 1)
                    {
                        table.Rows[indexRow].Cells[1].Paragraphs.First().Append(item.item == null ? "" : Regex.Replace(item.item.ToString(), @"<.*?>", string.Empty));
                        table.Rows[indexRow].Cells[2].Paragraphs.First().Append(item.satuan == null ? "" : item.satuan.ToString());
                        table.Rows[indexRow].Cells[3].Paragraphs.First().Append(item.jumlah == null ? "" : item.jumlah.Value.ToString());
                        table.Rows[indexRow].Cells[3].Width = 10;
                        table.Rows[indexRow].Cells[4].Paragraphs.First().Append(item.hps == null ? "" : item.hps.Value.ToString("C", MyConverter.formatCurrencyIndo()));
                        decimal? total = item.jumlah * item.hps;
                        table.Rows[indexRow].Cells[5].Paragraphs.First().Append(total == null ? "" : total.Value.ToString("C", MyConverter.formatCurrencyIndo()));
                        table.Rows[indexRow].Cells[6].Paragraphs.First().Append(item.keterangan);
                        subtotal = subtotal + total.Value;
                        totalall = totalall + total.Value;
                    }
                    else if (item.level == 2)
                    {
                        table.Rows[indexRow].Cells[4].Paragraphs.First().Append("Sub Total");
                        table.Rows[indexRow].Cells[5].Paragraphs.First().Append(subtotal.ToString("C", MyConverter.formatCurrencyIndo()));
                        subtotal = 0;
                    }
                    indexRow++;
                }
                doc.ReplaceText("{total_hps}", totalall.ToString("C", MyConverter.formatCurrencyIndo()));
                // Insert table at index where tag #TABLE# is in document.
                //doc.InsertTable(table);
                foreach (var paragraph in doc.Paragraphs)
                {
                    paragraph.FindAll("{tabel}").ForEach(index => paragraph.InsertTableAfterSelf((table)));
                }
                //Remove tag
                doc.ReplaceText("{tabel}", "");

                doc.SaveAs(OutFileNama);
                streamx.Close();
            }
            catch
            {
                streamx.Close();
            }

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new FileStream(OutFileNama, FileMode.Open);
            result.Content = new StreamContent(stream);
            //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.wordprocessingml.document");

            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = outputFileName
            };

            return result;
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public HttpResponseMessage CetakHPSAsuransi(Guid Id, Guid DocumentIdBaru)
        {
            var pengadaan = _repository.GetPengadaan(Id, UserId(), 0);
            //var jadwalKlarifikasi = _repository.getPelaksanaanKlarifikasi(Id, UserId());
            string fileName = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Template\template-hps-asuransi.docx";

            string outputFileName = "Cetak-HPS-Asuransi" + UserId().ToString() + "-" + DateTime.Now.ToString("dd-MM-yy") + ".docx";

            string OutFileNama = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Temp\" + outputFileName;

            var streamx = new FileStream(fileName, FileMode.Open);
            try
            {
                var doc = DocX.Load(streamx);//.Create(OutFileNama);
                doc.ReplaceText("{pengadaan_name_judul}", pengadaan.Judul == null ? "" : pengadaan.Judul);
                doc.ReplaceText("{nomor_berita_acara}", pengadaan.NoPengadaan == null ? "" : pengadaan.NoPengadaan);
                doc.ReplaceText("{pengadaan_unit_pemohon}", pengadaan.UnitKerjaPemohon == null ? "" : pengadaan.UnitKerjaPemohon);

                var oVWRKSDetail = _repository.getRKSAsuransiDetails(DocumentIdBaru, UserId());
                var table = doc.AddTable(oVWRKSDetail.Count() + 1, 5);

                int indexRow = 0;
                table.Rows[indexRow].Cells[0].Paragraphs.First().Append("Benefit");
                table.Rows[indexRow].Cells[1].Paragraphs.First().Append("Coverage");
                table.Rows[indexRow].Cells[2].Paragraphs.First().Append("Region");
                table.Rows[indexRow].Cells[3].Paragraphs.First().Append("Rate");
                table.Rows[indexRow].MergeCells(3, 4);
                indexRow++;
                foreach (var item in oVWRKSDetail)
                {

                    table.Rows[indexRow].Cells[0].Paragraphs.First().Append(item.BenefitCode == null ? "" : item.BenefitCode.ToString());
                    table.Rows[indexRow].Cells[1].Paragraphs.First().Append(item.BenefitCoverage == null ? "" : item.BenefitCoverage.ToString());
                    table.Rows[indexRow].Cells[2].Paragraphs.First().Append(item.RegionCode == null ? "" : item.RegionCode.ToString());
                    if (item.IsRange == false)
                    {
                        table.Rows[indexRow].Cells[3].Paragraphs.First().Append(item.Rate == null ? "" : item.Rate.ToString());
                        table.Rows[indexRow].MergeCells(3, 4);
                    }
                    if (item.IsRange == true)
                    {
                        table.Rows[indexRow].Cells[3].Paragraphs.First().Append(item.RateLowerLimit == null ? "" : item.RateLowerLimit.ToString());
                        table.Rows[indexRow].Cells[4].Paragraphs.First().Append(item.RateUpperLimit == null ? "" : item.RateUpperLimit.ToString());
                    }
                    if (item.IsOpen == true)
                    {
                        table.Rows[indexRow].Cells[3].Paragraphs.First().Append("Perlu Underwriting");
                        table.Rows[indexRow].MergeCells(3, 4);
                    }

                    indexRow++;
                }
                //doc.ReplaceText("{total_hps}", totalall.ToString("C", MyConverter.formatCurrencyIndo()));
                // Insert table at index where tag #TABLE# is in document.
                //doc.InsertTable(table);
                foreach (var paragraph in doc.Paragraphs)
                {
                    paragraph.FindAll("{tabel}").ForEach(index => paragraph.InsertTableAfterSelf((table)));
                }
                //Remove tag
                doc.ReplaceText("{tabel}", "");

                doc.SaveAs(OutFileNama);
                streamx.Close();
            }
            catch
            {
                streamx.Close();
            }

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new FileStream(OutFileNama, FileMode.Open);
            result.Content = new StreamContent(stream);
            //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.wordprocessingml.document");

            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = outputFileName
            };

            return result;
        }


        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public HttpResponseMessage CetakHPSXLS(Guid Id)
        {
            var pengadaan = _repository.GetPengadaan(Id, UserId(), 0);
            var spc = new System.Data.DataTable("Jimbis");
            string outputFileName = "Cetak-HPS" + UserId().ToString() + "-" + DateTime.Now.ToString("dd-MM-yy") + ".xlsx";
            string OutFileNama = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Temp\" + outputFileName;

            var ms = new System.IO.MemoryStream();
            try
            {
                var oVWRKSDetail = _repository.getRKSDetails(pengadaan.Id, UserId());

                using (var sl = new SpreadsheetLight.SLDocument())
                {
                    sl.SetCellValue(1, 1, "Judul Pengadaan");
                    sl.SetCellValue(1, 2, pengadaan.Judul == null ? "" : pengadaan.Judul);
                    sl.SetCellValue(2, 1, "Nomor Pengadaan");
                    sl.SetCellValue(2, 2, pengadaan.NoPengadaan == null ? "" : pengadaan.NoPengadaan);
                    sl.SetCellValue(3, 1, "Unit Kerja");
                    sl.SetCellValue(3, 2, pengadaan.UnitKerjaPemohon == null ? "" : pengadaan.UnitKerjaPemohon);

                    var rowNum = 6;
                    //write header
                    sl.SetCellValue(rowNum, 1, "Nama");
                    sl.SetCellValue(rowNum, 2, "Item");
                    sl.SetCellValue(rowNum, 3, "Satuan");
                    sl.SetCellValue(rowNum, 4, "Jumlah");
                    sl.SetCellValue(rowNum, 5, "Hps");
                    sl.SetCellValue(rowNum, 6, "Total");
                    sl.SetCellValue(rowNum, 7, "Keterangan");
                    rowNum++;
                    //write data
                    decimal subtotal = 0;
                    decimal totalall = 0;
                    foreach (var item in oVWRKSDetail)
                    {
                        if (item.level == 0)
                        {
                            sl.SetCellValue(rowNum, 1, (item.judul));
                        }
                        else if (item.level == 1)
                        {
                            sl.SetCellValue(rowNum, 2, Regex.Replace(item.item.ToString(), @"<.*?>", string.Empty));
                            sl.SetCellValue(rowNum, 3, item.satuan);
                            if (item.jumlah == null) sl.SetCellValue(rowNum, 4, "");
                            else sl.SetCellValue(rowNum, 4, item.jumlah.Value);
                            if (item.hps == null) sl.SetCellValue(rowNum, 5, "");
                            else sl.SetCellValue(rowNum, 5, item.hps.Value.ToString("C", MyConverter.formatCurrencyIndo()));
                            decimal? total = item.jumlah * item.hps;
                            if (total == null) sl.SetCellValue(rowNum, 6, "");
                            else sl.SetCellValue(rowNum, 6, total.Value.ToString("C", MyConverter.formatCurrencyIndo()));
                            sl.SetCellValue(rowNum, 7, item.keterangan);
                            subtotal = subtotal + total.Value;
                            totalall = totalall + total.Value;
                        }
                        else if (item.level == 2)
                        {
                            sl.SetCellValue(rowNum, 5, "Sub Total");
                            sl.SetCellValue(rowNum, 6, subtotal.ToString("C", MyConverter.formatCurrencyIndo()));
                            subtotal = 0;
                        }
                        rowNum++;
                    }
                    sl.SetCellValue(4, 1, "Total HPS");
                    sl.SetCellValue(4, 2, totalall.ToString("C", MyConverter.formatCurrencyIndo()));

                    //add filter
                    sl.SetColumnWidth(4, 12.0);
                    sl.SetColumnWidth(5, 30.0);
                    sl.SetColumnWidth(6, 30.0);

                    sl.SaveAs(ms);
                }

            }
            catch
            {

            }

            ms.Position = 0;
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            //  var stream = new FileStream(ms, FileMode.Open);
            result.Content = new StreamContent(ms);
            //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = outputFileName
            };
            return result;
        }


        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public HttpResponseMessage CetakHPSXLSAsuransi(Guid Id, Guid DocumentIdBaru)
        {
            var pengadaan = _repository.GetPengadaan(Id, UserId(), 0);
            var spc = new System.Data.DataTable("Jimbis");
            string outputFileName = "Cetak-HPS-Asuransi-" + UserId().ToString() + "-" + DateTime.Now.ToString("dd-MM-yy") + ".xlsx";
            string OutFileNama = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Temp\" + outputFileName;

            var ms = new System.IO.MemoryStream();
            try
            {
                var oVWRKSDetail = _repository.getRKSAsuransiDetails(DocumentIdBaru, UserId());

                using (var sl = new SpreadsheetLight.SLDocument())
                {
                    sl.SetCellValue(1, 1, "Judul Pengadaan");
                    sl.SetCellValue(1, 2, pengadaan.Judul == null ? "" : pengadaan.Judul);
                    sl.SetCellValue(2, 1, "Nomor Pengadaan");
                    sl.SetCellValue(2, 2, pengadaan.NoPengadaan == null ? "" : pengadaan.NoPengadaan);
                    sl.SetCellValue(3, 1, "Unit Kerja");
                    sl.SetCellValue(3, 2, pengadaan.UnitKerjaPemohon == null ? "" : pengadaan.UnitKerjaPemohon);

                    var rowNum = 6;
                    //write header
                    SpreadsheetLight.SLStyle style = sl.CreateStyle();
                    style.Border.LeftBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Medium;
                    style.Border.RightBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Medium;
                    style.Border.BottomBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Medium;
                    style.Border.TopBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Medium;
                    style.Alignment.Horizontal = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;


                    sl.SetCellValue(rowNum, 1, "Benefit");
                    sl.SetCellValue(rowNum, 2, "Coverage");
                    sl.SetCellValue(rowNum, 3, "Region");
                    sl.SetCellValue(rowNum, 4, "Rate");
                    sl.MergeWorksheetCells(rowNum, 4, rowNum, 5);

                    sl.SetCellStyle(rowNum, 1, style);
                    sl.SetCellStyle(rowNum, 2, style);
                    sl.SetCellStyle(rowNum, 3, style);
                    sl.SetCellStyle(rowNum, 4, style);
                    sl.SetCellStyle(rowNum, 5, style);

                    rowNum++;
                    //write data
                    //decimal subtotal = 0;
                    //decimal totalall = 0;
                    foreach (var item in oVWRKSDetail)
                    {
                        sl.SetCellValue(rowNum, 1, item.BenefitCode);
                        sl.SetCellValue(rowNum, 2, item.BenefitCoverage);
                        sl.SetCellValue(rowNum, 3, item.RegionCode);

                        if (item.IsRange == false)
                        {
                            sl.SetCellValue(rowNum, 4, (item.Rate == null ? "" : item.Rate.ToString()));
                            sl.MergeWorksheetCells(rowNum, 4, rowNum, 5);
                        }

                        if (item.IsRange == true)
                        {
                            sl.SetCellValue(rowNum, 4, (item.RateLowerLimit == null ? "" : item.RateLowerLimit.ToString()));
                            sl.SetCellValue(rowNum, 5, (item.RateUpperLimit == null ? "" : item.RateUpperLimit.ToString()));
                        }

                        if (item.IsOpen == true)
                        {
                            sl.SetCellValue(rowNum, 4, "Perlu Underwriting");
                            sl.MergeWorksheetCells(rowNum, 4, rowNum, 5);
                        }

                        rowNum++;
                    }
                    // sl.SetCellValue(4, 1, "Total HPS");
                    //sl.SetCellValue(4, 2, totalall.ToString("C", MyConverter.formatCurrencyIndo()));

                    //add filter
                    sl.SetColumnWidth(1, 66.0);
                    sl.SetColumnWidth(2, 24.0);
                    sl.SetColumnWidth(3, 10.0);
                    sl.SetColumnWidth(4, 10.0);
                    sl.SetColumnWidth(5, 10.0);
                    sl.SaveAs(ms);
                }

            }
            catch
            {

            }

            ms.Position = 0;
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            //  var stream = new FileStream(ms, FileMode.Open);
            result.Content = new StreamContent(ms);
            //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = outputFileName
            };
            return result;
        }



        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public HttpResponseMessage CetakVendor()
        {
            //var Vendors = _repository.GetAllVendors();
            var Vendors = _repository.GetCetakVendor();


            string outputFileName = "Cetak-Vendor" + UserId().ToString() + "-" + DateTime.Now.ToString("dd-MM-yy") + ".xlsx";
            string OutFileNama = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Temp\" + outputFileName;

            var ms = new System.IO.MemoryStream();
            try
            {
                // No 	Supplier Name	Jenis Usaha	Supplier Address	Supplier Phone	Contact Person Name	PKP / Non PKP	NPWP

                using (var sl = new SpreadsheetLight.SLDocument())
                {
                    SpreadsheetLight.SLStyle style = sl.CreateStyle();
                    style.Border.LeftBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Medium;
                    style.Border.RightBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Medium;
                    style.Border.BottomBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Medium;
                    style.Border.TopBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Medium;

                    sl.SetCellValue(1, 1, "NO");
                    sl.SetCellValue(1, 2, "Supplier Name");
                    sl.SetCellValue(1, 3, "Jenis Usaha");
                    sl.SetCellValue(1, 4, "Kota");
                    sl.SetCellValue(1, 5, "Provinsi");
                    sl.SetCellValue(1, 6, "Email");
                    sl.SetCellValue(1, 7, "Kode Pos");
                    sl.SetCellValue(1, 8, "No Pengajuan");
                    sl.SetCellValue(1, 9, "Status");
                    sl.SetCellValue(1, 10, "Website");
                    sl.SetCellValue(1, 11, "Nama Bank");
                    sl.SetCellValue(1, 12, "Nama Rekening");
                    sl.SetCellValue(1, 13, "Cabang Bank");
                    sl.SetCellValue(1, 14, "Nomor Rekening");
                    sl.SetCellValue(1, 15, "Supplier Phone");
                    sl.SetCellValue(1, 16, "Supplier Address");
                    sl.SetCellValue(1, 17, "Contact Person Name");



                    //style
                    sl.SetCellStyle(1, 1, style);
                    sl.SetCellStyle(1, 2, style);
                    sl.SetCellStyle(1, 3, style);
                    sl.SetCellStyle(1, 4, style);
                    sl.SetCellStyle(1, 5, style);
                    sl.SetCellStyle(1, 6, style);
                    sl.SetCellStyle(1, 7, style);
                    sl.SetCellStyle(1, 8, style);
                    sl.SetCellStyle(1, 9, style);
                    sl.SetCellStyle(1, 10, style);
                    sl.SetCellStyle(1, 11, style);
                    sl.SetCellStyle(1, 12, style);
                    sl.SetCellStyle(1, 13, style);
                    sl.SetCellStyle(1, 14, style);
                    sl.SetCellStyle(1, 15, style);
                    sl.SetCellStyle(1, 16, style);
                    sl.SetCellStyle(1, 17, style);

                    int row = 2;
                    int no = 1;


                    foreach (var item in Vendors)
                    {                        
                        sl.SetCellValue(row, 1, no);
                        sl.SetCellValue(row, 2, item.Nama);
                        sl.SetCellValue(row, 3, item.TipeVendor.ToString());
                        sl.SetCellValue(row, 4, item.Kota);
                        sl.SetCellValue(row, 5, item.Provinsi);
                        sl.SetCellValue(row, 6, item.Email);
                        sl.SetCellValue(row, 7, item.KodePos);
                        sl.SetCellValue(row, 8, item.NomorVendor);
                        sl.SetCellValue(row, 9, item.StatusAkhir.ToString());
                        sl.SetCellValue(row, 10, item.Website);
                        foreach (var itemBank in item.BankInfo)
                        {
                            sl.SetCellValue(row, 11, itemBank.NamaBank);
                            sl.SetCellValue(row, 12, itemBank.NamaRekening);
                            sl.SetCellValue(row, 13, itemBank.Cabang);
                            sl.SetCellValue(row, 14, itemBank.NomorRekening);
                        }
                        sl.SetCellValue(row, 15, item.Telepon);
                        sl.SetCellValue(row, 16, item.Alamat);
                        sl.SetCellValue(row, 17, item.VendorPerson != null ? item.VendorPerson.FirstOrDefault().Nama : "");
                        
                        //style
                        sl.SetCellStyle(row, 1, style);
                        sl.SetCellStyle(row, 2, style);
                        sl.SetCellStyle(row, 3, style);
                        sl.SetCellStyle(row, 4, style);
                        sl.SetCellStyle(row, 5, style);
                        sl.SetCellStyle(row, 6, style);
                        sl.SetCellStyle(row, 7, style);
                        sl.SetCellStyle(row, 8, style);
                        sl.SetCellStyle(row, 9, style);
                        sl.SetCellStyle(row, 10, style);
                        sl.SetCellStyle(row, 11, style);
                        sl.SetCellStyle(row, 12, style);
                        sl.SetCellStyle(row, 13, style);
                        sl.SetCellStyle(row, 14, style);
                        sl.SetCellStyle(row, 15, style);
                        sl.SetCellStyle(row, 16, style);
                        sl.SetCellStyle(row, 17, style);

                        row++;
                        no++;



                    }

                    //add filter
                    sl.SetColumnWidth(1, 5.0);
                    sl.SetColumnWidth(2, 30.0);
                    sl.SetColumnWidth(3, 30.0);
                    sl.SetColumnWidth(4, 60.0);
                    sl.SetColumnWidth(5, 30.0);
                    sl.SetColumnWidth(6, 30.0);
                    sl.SetColumnWidth(7, 30.0);
                    sl.SetColumnWidth(8, 30.0);
                    sl.SetColumnWidth(9, 30.0);
                    sl.SetColumnWidth(10, 30.0);
                    sl.SetColumnWidth(11, 60.0);
                    sl.SetColumnWidth(12, 30.0);
                    sl.SetColumnWidth(13, 30.0);
                    sl.SetColumnWidth(14, 30.0);
                    sl.SetColumnWidth(15, 30.0);
                    sl.SetColumnWidth(16, 30.0);
                    sl.SetColumnWidth(17, 30.0);

                    //
                    SpreadsheetLight.SLStyle style2 = sl.CreateStyle();
                    style2.Font.Bold = true;

                    sl.SetRowStyle(1, style2);


                    //style.Border.LeftBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Thick;
                    //style.Border.LeftBorder.Color = System.Drawing.Color.BlanchedAlmond;

                    //style.Border.BottomBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.DashDotDot;
                    //style.Border.BottomBorder.Color = System.Drawing.Color.Brown;

                    //style.SetRightBorder(DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Hair, System.Drawing.Color.Blue);
                    //// Alternatively, use the "long-form" version:
                    //// style.Border.RightBorder.BorderStyle = BorderStyleValues.Hair;
                    //// style.Border.RightBorder.Color = System.Drawing.Color.Blue;

                    //style.SetTopBorder(DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Double, SpreadsheetLight.SLThemeColorIndexValues.Accent6Color);
                    //// Alternatively, use the "long-form" version:
                    //// style.Border.TopBorder.BorderStyle = BorderStyleValues.Double;
                    //// style.Border.TopBorder.SetBorderThemeColor(SLThemeColorIndexValues.Accent6Color);

                    //// The "0.2" means "lightens the accent 3 colour by 20%".
                    //// A negative value darkens the given theme colour.
                    //style.SetDiagonalBorder(DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.MediumDashDotDot, SpreadsheetLight.SLThemeColorIndexValues.Accent3Color, 0.2);
                    //// Alternatively, use the "long-form" version:
                    //// style.Border.DiagonalBorder.BorderStyle = BorderStyleValues.MediumDashDotDot;
                    //// style.Border.DiagonalBorder.SetBorderThemeColor(SLThemeColorIndexValues.Accent3Color, 0.2);

                    //style.Border.DiagonalUp = true;
                    //style.Border.DiagonalDown = true;

                    sl.SaveAs(ms);
                }

            }
            catch
            {

            }

            ms.Position = 0;
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            //  var stream = new FileStream(ms, FileMode.Open);
            result.Content = new StreamContent(ms);
            //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = outputFileName
            };
            return result;
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public HttpResponseMessage CetakPenilaianVendor(string dari, string sampai)
        {
            //LocalReport lr = new LocalReport();
            //string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + FILE_REPORT_PATH;

            //path = Path.Combine(path, "ReportPenilaianVendor.rdlc");
            //if (System.IO.File.Exists(path))
            //{
            //    lr.ReportPath = path;
            //}

            //else
            //{
            //    //return View("Index");
            //}

            var oDari = Common.ConvertDate(dari, "dd/MM/yyyy");
            var oSampai = Common.ConvertDate(sampai, "dd/MM/yyyy");

            //masalah di arguments
            var Proyek = _repository.GetAllPenilaianProyekRevByHarry(oDari, oSampai);

            foreach (var item_a in Proyek)
            {
                var detail_penilaian = _repository.GetHeaderDetail(item_a.rksheader);

            }

            string outputFileName = "Cetak-Penilaian-Vendor" + UserId().ToString() + "-" + DateTime.Now.ToString("dd-MM-yy") + ".xlsx";
            string OutFileNama = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Temp\" + outputFileName;

            var ms = new System.IO.MemoryStream();
            try
            {
                // No 	NamaProject User	Vendor	criteria(Quality of Product Quality of Service  cost  Delivery  Flexibility Responsiveness  Average Score)	Note

                using (var sl = new SpreadsheetLight.SLDocument())
                {
                    SpreadsheetLight.SLStyle style = sl.CreateStyle();
                    style.Border.LeftBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Medium;
                    style.Border.RightBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Medium;
                    style.Border.BottomBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Medium;
                    style.Border.TopBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Medium;

                    sl.SetCellValue(2, 1, "No");
                    sl.SetCellValue(2, 2, "Project");
                    sl.SetCellValue(2, 3, "User");
                    sl.SetCellValue(2, 4, "Vendor");
                    sl.SetCellValue(2, 5, "Quality of Product");
                    sl.SetCellValue(2, 6, "Quality of Service");
                    sl.SetCellValue(2, 7, "Cost");
                    sl.SetCellValue(2, 8, "Delivery");
                    sl.SetCellValue(2, 9, "Flexibility");
                    sl.SetCellValue(2, 10, "Responsiveness");
                    sl.SetCellValue(2, 11, "Average Score");
                    sl.SetCellValue(2, 12, "Note");

                    //style
                    sl.SetCellStyle(2, 1, style);
                    sl.SetCellStyle(2, 2, style);
                    sl.SetCellStyle(2, 3, style);
                    sl.SetCellStyle(2, 4, style);
                    sl.SetCellStyle(2, 5, style);
                    sl.SetCellStyle(2, 6, style);
                    sl.SetCellStyle(2, 7, style);
                    sl.SetCellStyle(2, 8, style);
                    sl.SetCellStyle(2, 9, style);
                    sl.SetCellStyle(2, 10, style);
                    sl.SetCellStyle(2, 11, style);
                    sl.SetCellStyle(2, 12, style);


                    int row = 3;
                    int no = 1;


                    foreach (var item in Proyek)
                    {
                        sl.SetCellValue(row, 1, no);
                        sl.SetCellValue(row, 2, item.NamaProject);
                        sl.SetCellValue(row, 3, item.User);
                        sl.SetCellValue(row, 4, item.Vendor);


                        VWReportPenilaianVendorDetail nmn = new VWReportPenilaianVendorDetail();

                        nmn = _repository.GetHeaderDetail(item.rksheader);

                        sl.SetCellValue(row, 5, nmn.QualityOfProduct);
                        sl.SetCellValue(row, 6, nmn.QualityOfService);
                        sl.SetCellValue(row, 7, nmn.Cost);
                        sl.SetCellValue(row, 8, nmn.Delivery);
                        sl.SetCellValue(row, 9, nmn.Flexibility);
                        sl.SetCellValue(row, 10, nmn.Responsiveness);

                        decimal averageScore = (nmn.QualityOfProduct + nmn.QualityOfService + nmn.Cost + nmn.Delivery + nmn.Flexibility + nmn.Responsiveness) / 6;

                        sl.SetCellValue(row, 11, averageScore);
                        sl.SetCellValue(row, 12, item.Note);

                        //style
                        //sl.SetCellStyle(row, 1, style);
                        //sl.SetCellStyle(row, 2, style);
                        //sl.SetCellStyle(row, 3, style);
                        //sl.SetCellStyle(row, 4, style);
                        //sl.SetCellStyle(row, 5, style);
                        //sl.SetCellStyle(row, 6, style);
                        //sl.SetCellStyle(row, 7, style);
                        //sl.SetCellStyle(row, 8, style);
                        //sl.SetCellStyle(row, 9, style);
                        //sl.SetCellStyle(row, 10, style);
                        //sl.SetCellStyle(row, 11, style);

                        row++;
                        no++;

                    }

                    //add filter
                    sl.SetColumnWidth(1, 5.0);
                    sl.SetColumnWidth(2, 30.0);
                    sl.SetColumnWidth(3, 15.0);
                    sl.SetColumnWidth(4, 30.0);
                    sl.SetColumnWidth(5, 15.20);
                    sl.SetColumnWidth(6, 15.20);
                    sl.SetColumnWidth(7, 15.20);
                    sl.SetColumnWidth(8, 15.20);
                    sl.SetColumnWidth(9, 15.20);
                    sl.SetColumnWidth(10, 15.20);
                    sl.SetColumnWidth(11, 15.20);
                    sl.SetColumnWidth(12, 15.20);

                    //
                    SpreadsheetLight.SLStyle style2 = sl.CreateStyle();
                    style2.Font.Bold = true;

                    sl.SetRowStyle(1, style2);

                    sl.SaveAs(ms);
                }

            }
            catch
            {

            }

            ms.Position = 0;
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            //  var stream = new FileStream(ms, FileMode.Open);
            result.Content = new StreamContent(ms);
            //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = outputFileName
            };
            return result;
        }


        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public HttpResponseMessage CetakRKSPenilaianAsuransiAll(Guid Id)
        {
            var pengadaan = _repository.GetPengadaan(Id, UserId(), 0);
            var jadwalKlarifikasi = _repository.getPelaksanaanKlarifikasi(Id, UserId());
            string fileName = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Template\Klarifikasi-Pemenang-Asuransi.docx";

            string outputFileName = "Cetak-RKS-Penilaian-Asuransi-" + UserId().ToString() + "-" + DateTime.Now.ToString("dd-MM-yy") + ".docx";

            string OutFileNama = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Temp\" + outputFileName;

            var streamx = new FileStream(fileName, FileMode.Open);

            var doc = DocX.Load(streamx);//.Create(OutFileNama);
            doc.PageLayout.Orientation = Novacode.Orientation.Landscape;
            doc.ReplaceText("{pengadaan_name_judul}", pengadaan.Judul == null ? "" : pengadaan.Judul.ToUpper());
            doc.ReplaceText("{nomor_berita_acara}", pengadaan.NoPengadaan == null ? "" : pengadaan.NoPengadaan);
            doc.ReplaceText("{pengadaan_unit_pemohon}", pengadaan.UnitKerjaPemohon == null ? "" : pengadaan.UnitKerjaPemohon);


            var oVWRKSVendors = _repository.getRKSPenilaianAsuransi(pengadaan.Id, UserId());
            var table = doc.AddTable(oVWRKSVendors.hps.Count() + 1, (oVWRKSVendors.vendors.Count * 2) + 6);
            Border BlankBorder = new Border(BorderStyle.Tcbs_single, BorderSize.one, 0, Color.Black);
            table.SetBorder(TableBorderType.Bottom, BlankBorder);
            table.SetBorder(TableBorderType.Left, BlankBorder);
            table.SetBorder(TableBorderType.Right, BlankBorder);
            table.SetBorder(TableBorderType.Top, BlankBorder);
            table.SetBorder(TableBorderType.InsideV, BlankBorder);
            table.SetBorder(TableBorderType.InsideH, BlankBorder);

            int indexRow = 0;
            table.Rows[indexRow].Cells[0].Paragraphs.First().Append("NO");
            table.Rows[indexRow].Cells[0].Width = 10;
            table.Rows[indexRow].Cells[1].Paragraphs.First().Append("Benefit");
            table.Rows[indexRow].Cells[2].Paragraphs.First().Append("Coverage");
            table.Rows[indexRow].Cells[3].Paragraphs.First().Append("Region");
            table.Rows[indexRow].Cells[4].Paragraphs.First().Append("Rate");
            table.Rows[indexRow].MergeCells(4, 5);
            int headerCol = 5;
            int headerCol2 = 6;
            //List<int> countList = new List<int>();
            //List<string> NameList = new List<string>();
            foreach (var item in oVWRKSVendors.vendors)
            {
                table.Rows[indexRow].Cells[headerCol].Paragraphs.First().Append(item.NamaVendor);
                table.Rows[indexRow].MergeCells(headerCol, headerCol2);
                headerCol++;
                headerCol2++;
            }
            indexRow++;
            int no = 1;
            //decimal? subtotal = 0;
            var itemlast = oVWRKSVendors.hps.Last();
            for (var i = 0; i < oVWRKSVendors.hps.Count(); i++)
            {
                //var currentGroup = item.grup;
                //var index = oVWRKSVendors.hps.IndexOf(i);
                table.Rows[indexRow].Cells[0].Paragraphs.First().Append(no.ToString());
                table.Rows[indexRow].Cells[0].Width = 10;
                no++;
                //Regex example #1 "<.*?>"
                //string dekripsi = "";
                //if (item.item != null) dekripsi = Regex.Replace(item.item, @"<.*?>", string.Empty);
                //Regex example #2
                // string result2 = Regex.Replace(dekripsi, @"<[^>].+?>", "");
                table.Rows[indexRow].Cells[1].Paragraphs.First().Append(oVWRKSVendors.hps[i].BenefitCode != "undefined" ? oVWRKSVendors.hps[i].BenefitCode : "");
                table.Rows[indexRow].Cells[2].Paragraphs.First().Append(oVWRKSVendors.hps[i].BenefitCoverage != "undefined" ? oVWRKSVendors.hps[i].BenefitCoverage : "");
                table.Rows[indexRow].Cells[3].Paragraphs.First().Append(oVWRKSVendors.hps[i].RegionCode != "undefined" ? oVWRKSVendors.hps[i].RegionCode : "");
                if (oVWRKSVendors.hps[i].IsOpen == true)
                {
                    table.Rows[indexRow].Cells[4].Paragraphs.First().Append("Perlu Underwriting");
                    table.Rows[indexRow].MergeCells(4, 5);
                }
                else
                {
                    if (oVWRKSVendors.hps[i].IsRange == true)
                    {
                        table.Rows[indexRow].Cells[4].Paragraphs.First().Append(oVWRKSVendors.hps[i].RateLowerLimit == null ? "" : oVWRKSVendors.hps[i].RateLowerLimit.ToString());
                        table.Rows[indexRow].Cells[5].Paragraphs.First().Append(oVWRKSVendors.hps[i].RateUpperLimit == null ? "" : oVWRKSVendors.hps[i].RateUpperLimit.ToString());
                    }
                    if (oVWRKSVendors.hps[i].IsRange == false)
                    {
                        table.Rows[indexRow].Cells[4].Paragraphs.First().Append(oVWRKSVendors.hps[i].Rate == null ? "" : oVWRKSVendors.hps[i].Rate.ToString());
                        table.Rows[indexRow].MergeCells(4, 5);
                    }

                }

                /*sub totol asumsi kalo next group nya beda*/
                int nexCol = 5;
                int nexCol2 = 6;

                for (var z = 0; z < oVWRKSVendors.vendors.Count(); z++)
                {
                    if (oVWRKSVendors.hps[i].IsOpen == true)
                    {
                        table.Rows[indexRow].Cells[nexCol].Paragraphs.First().Append("Perlu Underwriting");
                        table.Rows[indexRow].MergeCells(nexCol, nexCol2);
                    }
                    else
                    {
                        if (oVWRKSVendors.hps[i].IsRange == true)
                        {
                            table.Rows[indexRow].Cells[nexCol + 1].Paragraphs.First().Append(oVWRKSVendors.vendors[z].itemAsuransi[i].RateLowerLimit == null ? "" : oVWRKSVendors.vendors[z].itemAsuransi[i].RateLowerLimit.ToString());
                            table.Rows[indexRow].Cells[nexCol2 + 1].Paragraphs.First().Append(oVWRKSVendors.vendors[z].itemAsuransi[i].RateUpperLimit == null ? "" : oVWRKSVendors.vendors[z].itemAsuransi[i].RateUpperLimit.ToString());
                            nexCol++;
                            nexCol2++;
                        }
                        if (oVWRKSVendors.hps[i].IsRange == false)
                        {
                            table.Rows[indexRow].Cells[nexCol].Paragraphs.First().Append(oVWRKSVendors.vendors[z].itemAsuransi[i].Rate == null ? "" : oVWRKSVendors.vendors[z].itemAsuransi[i].Rate.ToString());
                            table.Rows[indexRow].MergeCells(nexCol, nexCol2);
                        }
                    }
                    nexCol++;
                    nexCol2++;
                    //nexCol = nexCol + 2;
                    //nexCol2 = nexCol2 + 2;
                }

                indexRow++;
            }
            // Insert table at index where tag #TABLE# is in document.
            //doc.InsertTable(table);
            foreach (var paragraph in doc.Paragraphs)
            {
                paragraph.FindAll("{tabel}").ForEach(index => paragraph.InsertTableAfterSelf((table)));
            }
            //Remove tag
            doc.ReplaceText("{tabel}", "");

            doc.SaveAs(OutFileNama);
            streamx.Close();
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new FileStream(OutFileNama, FileMode.Open);
            result.Content = new StreamContent(stream);
            //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.wordprocessingml.document");

            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = outputFileName
            };

            return result;
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public HttpResponseMessage CetakRKSKlarifikasiLanjutanAll(Guid Id)
        {
            var pengadaan = _repository.GetPengadaan(Id, UserId(), 0);
            var jadwalKlarifikasi = _repository.getPelaksanaanKlarifikasi(Id, UserId());
            string fileName = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Template\Klarifikasi-Pemenang-Asuransi.docx";

            string outputFileName = "Cetak-RKS-Klarifikasi-Lanjutan-Asuransi-" + UserId().ToString() + "-" + DateTime.Now.ToString("dd-MM-yy") + ".docx";

            string OutFileNama = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Temp\" + outputFileName;

            var streamx = new FileStream(fileName, FileMode.Open);

            var doc = DocX.Load(streamx);//.Create(OutFileNama);
            doc.PageLayout.Orientation = Novacode.Orientation.Landscape;
            doc.ReplaceText("{pengadaan_name_judul}", pengadaan.Judul == null ? "" : pengadaan.Judul.ToUpper());
            doc.ReplaceText("{nomor_berita_acara}", pengadaan.NoPengadaan == null ? "" : pengadaan.NoPengadaan);
            doc.ReplaceText("{pengadaan_unit_pemohon}", pengadaan.UnitKerjaPemohon == null ? "" : pengadaan.UnitKerjaPemohon);


            var oVWRKSVendors = _repository.getRKSPenilaianKlarifikasiLanjutanAsuransi(pengadaan.Id, UserId());
            var table = doc.AddTable(oVWRKSVendors.hps.Count() + 1, (oVWRKSVendors.vendors.Count * 2) + 6);
            Border BlankBorder = new Border(BorderStyle.Tcbs_single, BorderSize.one, 0, Color.Black);
            table.SetBorder(TableBorderType.Bottom, BlankBorder);
            table.SetBorder(TableBorderType.Left, BlankBorder);
            table.SetBorder(TableBorderType.Right, BlankBorder);
            table.SetBorder(TableBorderType.Top, BlankBorder);
            table.SetBorder(TableBorderType.InsideV, BlankBorder);
            table.SetBorder(TableBorderType.InsideH, BlankBorder);

            int indexRow = 0;
            table.Rows[indexRow].Cells[0].Paragraphs.First().Append("NO");
            table.Rows[indexRow].Cells[0].Width = 10;
            table.Rows[indexRow].Cells[1].Paragraphs.First().Append("Benefit");
            table.Rows[indexRow].Cells[2].Paragraphs.First().Append("Coverage");
            table.Rows[indexRow].Cells[3].Paragraphs.First().Append("Region");
            table.Rows[indexRow].Cells[4].Paragraphs.First().Append("Rate");
            table.Rows[indexRow].MergeCells(4, 5);
            int headerCol = 5;
            int headerCol2 = 6;
            //List<int> countList = new List<int>();
            //List<string> NameList = new List<string>();
            foreach (var item in oVWRKSVendors.vendors)
            {
                table.Rows[indexRow].Cells[headerCol].Paragraphs.First().Append(item.NamaVendor);
                table.Rows[indexRow].MergeCells(headerCol, headerCol2);
                headerCol++;
                headerCol2++;
            }
            indexRow++;
            int no = 1;
            //decimal? subtotal = 0;
            var itemlast = oVWRKSVendors.hps.Last();
            for (var i = 0; i < oVWRKSVendors.hps.Count(); i++)
            {
                //var currentGroup = item.grup;
                //var index = oVWRKSVendors.hps.IndexOf(i);
                table.Rows[indexRow].Cells[0].Paragraphs.First().Append(no.ToString());
                table.Rows[indexRow].Cells[0].Width = 10;
                no++;
                //Regex example #1 "<.*?>"
                //string dekripsi = "";
                //if (item.item != null) dekripsi = Regex.Replace(item.item, @"<.*?>", string.Empty);
                //Regex example #2
                // string result2 = Regex.Replace(dekripsi, @"<[^>].+?>", "");
                table.Rows[indexRow].Cells[1].Paragraphs.First().Append(oVWRKSVendors.hps[i].BenefitCode != "undefined" ? oVWRKSVendors.hps[i].BenefitCode : "");
                table.Rows[indexRow].Cells[2].Paragraphs.First().Append(oVWRKSVendors.hps[i].BenefitCoverage != "undefined" ? oVWRKSVendors.hps[i].BenefitCoverage : "");
                table.Rows[indexRow].Cells[3].Paragraphs.First().Append(oVWRKSVendors.hps[i].RegionCode != "undefined" ? oVWRKSVendors.hps[i].RegionCode : "");
                if (oVWRKSVendors.hps[i].IsOpen == true)
                {
                    table.Rows[indexRow].Cells[4].Paragraphs.First().Append("Perlu Underwriting");
                    table.Rows[indexRow].MergeCells(4, 5);
                }
                else
                {
                    if (oVWRKSVendors.hps[i].IsRange == true)
                    {
                        table.Rows[indexRow].Cells[4].Paragraphs.First().Append(oVWRKSVendors.hps[i].RateLowerLimit == null ? "" : oVWRKSVendors.hps[i].RateLowerLimit.ToString());
                        table.Rows[indexRow].Cells[5].Paragraphs.First().Append(oVWRKSVendors.hps[i].RateUpperLimit == null ? "" : oVWRKSVendors.hps[i].RateUpperLimit.ToString());
                    }
                    if (oVWRKSVendors.hps[i].IsRange == false)
                    {
                        table.Rows[indexRow].Cells[4].Paragraphs.First().Append(oVWRKSVendors.hps[i].Rate == null ? "" : oVWRKSVendors.hps[i].Rate.ToString());
                        table.Rows[indexRow].MergeCells(4, 5);
                    }

                }

                /*sub totol asumsi kalo next group nya beda*/
                int nexCol = 5;
                int nexCol2 = 6;

                for (var z = 0; z < oVWRKSVendors.vendors.Count(); z++)
                {
                    if (oVWRKSVendors.hps[i].IsOpen == true)
                    {
                        table.Rows[indexRow].Cells[nexCol].Paragraphs.First().Append("Perlu Underwriting");
                        table.Rows[indexRow].MergeCells(nexCol, nexCol2);
                    }
                    else
                    {
                        if (oVWRKSVendors.hps[i].IsRange == true)
                        {
                            table.Rows[indexRow].Cells[nexCol + 1].Paragraphs.First().Append(oVWRKSVendors.vendors[z].itemAsuransi[i].RateLowerLimit == null ? "" : oVWRKSVendors.vendors[z].itemAsuransi[i].RateLowerLimit.ToString());
                            table.Rows[indexRow].Cells[nexCol2 + 1].Paragraphs.First().Append(oVWRKSVendors.vendors[z].itemAsuransi[i].RateUpperLimit == null ? "" : oVWRKSVendors.vendors[z].itemAsuransi[i].RateUpperLimit.ToString());
                            nexCol++;
                            nexCol2++;
                        }
                        if (oVWRKSVendors.hps[i].IsRange == false)
                        {
                            table.Rows[indexRow].Cells[nexCol].Paragraphs.First().Append(oVWRKSVendors.vendors[z].itemAsuransi[i].Rate == null ? "" : oVWRKSVendors.vendors[z].itemAsuransi[i].Rate.ToString());
                            table.Rows[indexRow].MergeCells(nexCol, nexCol2);
                        }
                    }
                    nexCol++;
                    nexCol2++;
                    //nexCol = nexCol + 2;
                    //nexCol2 = nexCol2 + 2;
                }

                indexRow++;
            }
            // Insert table at index where tag #TABLE# is in document.
            //doc.InsertTable(table);
            foreach (var paragraph in doc.Paragraphs)
            {
                paragraph.FindAll("{tabel}").ForEach(index => paragraph.InsertTableAfterSelf((table)));
            }
            //Remove tag
            doc.ReplaceText("{tabel}", "");

            doc.SaveAs(OutFileNama);
            streamx.Close();
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new FileStream(OutFileNama, FileMode.Open);
            result.Content = new StreamContent(stream);
            //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.wordprocessingml.document");

            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = outputFileName
            };

            return result;
        }


        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public HttpResponseMessage CetakRKSKlarfikasiAsuransi(Guid Id)
        {
            var pengadaan = _repository.GetPengadaan(Id, UserId(), 0);
            var jadwalKlarifikasi = _repository.getPelaksanaanKlarifikasi(Id, UserId());
            string fileName = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Template\Klarifikasi-Pemenang-Asuransi.docx";

            string outputFileName = "Cetak-RKS-Klarifikasi-Asuransi-" + UserId().ToString() + "-" + DateTime.Now.ToString("dd-MM-yy") + ".docx";

            string OutFileNama = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Temp\" + outputFileName;

            var streamx = new FileStream(fileName, FileMode.Open);

            var doc = DocX.Load(streamx);//.Create(OutFileNama);
            doc.PageLayout.Orientation = Novacode.Orientation.Landscape;
            doc.ReplaceText("{pengadaan_name_judul}", pengadaan.Judul == null ? "" : pengadaan.Judul.ToUpper());
            doc.ReplaceText("{nomor_berita_acara}", pengadaan.NoPengadaan == null ? "" : pengadaan.NoPengadaan);
            doc.ReplaceText("{pengadaan_unit_pemohon}", pengadaan.UnitKerjaPemohon == null ? "" : pengadaan.UnitKerjaPemohon);

            var oVWRKSVendorsBA = _repository.getRKSPenilaianAsuransi(pengadaan.Id, UserId());
            var oVWRKSVendors = _repository.getRKSKlarifikasiAsuransi(pengadaan.Id, UserId());
            var table = doc.AddTable(oVWRKSVendors.hps.Count() + 1, (oVWRKSVendors.vendors.Count * 4) + 6);
            Border BlankBorder = new Border(BorderStyle.Tcbs_single, BorderSize.one, 0, Color.Black);
            table.SetBorder(TableBorderType.Bottom, BlankBorder);
            table.SetBorder(TableBorderType.Left, BlankBorder);
            table.SetBorder(TableBorderType.Right, BlankBorder);
            table.SetBorder(TableBorderType.Top, BlankBorder);
            table.SetBorder(TableBorderType.InsideV, BlankBorder);
            table.SetBorder(TableBorderType.InsideH, BlankBorder);

            int indexRow = 0;
            table.Rows[indexRow].Cells[0].Paragraphs.First().Append("NO");
            table.Rows[indexRow].Cells[0].Width = 10;
            table.Rows[indexRow].Cells[1].Paragraphs.First().Append("Benefit");
            table.Rows[indexRow].Cells[2].Paragraphs.First().Append("Coverage");
            table.Rows[indexRow].Cells[3].Paragraphs.First().Append("Region");
            table.Rows[indexRow].Cells[4].Paragraphs.First().Append("Rate");
            table.Rows[indexRow].MergeCells(4, 5);
            int headercolBA = 5;
            int headercolBA2 = 6;
            int headerCol = 7;
            int headerCol2 = 8;
            //List<int> countList = new List<int>();
            //List<string> NameList = new List<string>();
            foreach (var item in oVWRKSVendors.vendors)
            {
                table.Rows[indexRow].Cells[headerCol].Paragraphs.First().Append(item.NamaVendor);
                table.Rows[indexRow].MergeCells(headerCol, headerCol2);
                //headerCol++;
                //headerCol2++;
                headerCol = headerCol + 3;
                headerCol2 = headerCol2 + 3;
            }
            foreach (var item in oVWRKSVendorsBA.vendors)
            {
                table.Rows[indexRow].Cells[headercolBA].Paragraphs.First().Append(item.NamaVendor).Append(" (Buka Amplop)");
                table.Rows[indexRow].MergeCells(headercolBA, headercolBA2);
                //headerCol++;
                //headerCol2++;
                headercolBA = headercolBA + 2;
                headercolBA2 = headercolBA2 + 2;
            }
            indexRow++;
            int no = 1;
            //decimal? subtotal = 0;
            var itemlast = oVWRKSVendors.hps.Last();
            for (var i = 0; i < oVWRKSVendors.hps.Count(); i++)
            {
                //var currentGroup = item.grup;
                //var index = oVWRKSVendors.hps.IndexOf(i);
                table.Rows[indexRow].Cells[0].Paragraphs.First().Append(no.ToString());
                table.Rows[indexRow].Cells[0].Width = 10;
                no++;
                //Regex example #1 "<.*?>"
                //string dekripsi = "";
                //if (item.item != null) dekripsi = Regex.Replace(item.item, @"<.*?>", string.Empty);
                //Regex example #2
                // string result2 = Regex.Replace(dekripsi, @"<[^>].+?>", "");
                table.Rows[indexRow].Cells[1].Paragraphs.First().Append(oVWRKSVendors.hps[i].BenefitCode != "undefined" ? oVWRKSVendors.hps[i].BenefitCode : "");
                table.Rows[indexRow].Cells[2].Paragraphs.First().Append(oVWRKSVendors.hps[i].BenefitCoverage != "undefined" ? oVWRKSVendors.hps[i].BenefitCoverage : "");
                table.Rows[indexRow].Cells[3].Paragraphs.First().Append(oVWRKSVendors.hps[i].RegionCode != "undefined" ? oVWRKSVendors.hps[i].RegionCode : "");
                if (oVWRKSVendors.hps[i].IsOpen == true)
                {
                    table.Rows[indexRow].Cells[4].Paragraphs.First().Append("Perlu Underwriting");
                    table.Rows[indexRow].MergeCells(4, 5);
                }
                else
                {
                    if (oVWRKSVendors.hps[i].IsRange == true)
                    {
                        table.Rows[indexRow].Cells[4].Paragraphs.First().Append(oVWRKSVendors.hps[i].RateLowerLimit == null ? "" : oVWRKSVendors.hps[i].RateLowerLimit.ToString());
                        table.Rows[indexRow].Cells[5].Paragraphs.First().Append(oVWRKSVendors.hps[i].RateUpperLimit == null ? "" : oVWRKSVendors.hps[i].RateUpperLimit.ToString());
                    }
                    if (oVWRKSVendors.hps[i].IsRange == false)
                    {
                        table.Rows[indexRow].Cells[4].Paragraphs.First().Append(oVWRKSVendors.hps[i].Rate == null ? "" : oVWRKSVendors.hps[i].Rate.ToString());
                        table.Rows[indexRow].MergeCells(4, 5);
                    }

                }

                /*sub totol asumsi kalo next group nya beda*/
                int nexColBA = 5;
                int nexColBA2 = 6;
                int nexCol = 7;
                int nexCol2 = 8;

                for (var z = 0; z < oVWRKSVendors.vendors.Count(); z++)
                {
                    if (oVWRKSVendors.hps[i].IsOpen == true)
                    {
                        table.Rows[indexRow].Cells[nexCol].Paragraphs.First().Append("Perlu Underwriting");
                        table.Rows[indexRow].MergeCells(nexCol, nexCol2);
                    }
                    else
                    {
                        if (oVWRKSVendors.hps[i].IsRange == true)
                        {
                            table.Rows[indexRow].Cells[nexCol + 1].Paragraphs.First().Append(oVWRKSVendors.vendors[z].itemAsuransi[i].RateLowerLimit == null ? "" : oVWRKSVendors.vendors[z].itemAsuransi[i].RateLowerLimit.ToString());
                            table.Rows[indexRow].Cells[nexCol2 + 1].Paragraphs.First().Append(oVWRKSVendors.vendors[z].itemAsuransi[i].RateUpperLimit == null ? "" : oVWRKSVendors.vendors[z].itemAsuransi[i].RateUpperLimit.ToString());
                            nexCol++;
                            nexCol2++;
                        }
                        if (oVWRKSVendors.hps[i].IsRange == false)
                        {
                            table.Rows[indexRow].Cells[nexCol].Paragraphs.First().Append(oVWRKSVendors.vendors[z].itemAsuransi[i].Rate == null ? "" : oVWRKSVendors.vendors[z].itemAsuransi[i].Rate.ToString());
                            table.Rows[indexRow].MergeCells(nexCol, nexCol2);
                        }
                    }
                    //nexCol++;
                    //nexCol2++;
                    nexCol = nexCol + 3;
                    nexCol2 = nexCol2 + 3;
                }

                for (var z = 0; z < oVWRKSVendorsBA.vendors.Count(); z++)
                {
                    if (oVWRKSVendors.hps[i].IsOpen == true)
                    {
                        table.Rows[indexRow].Cells[nexColBA].Paragraphs.First().Append("Perlu Underwriting");
                        table.Rows[indexRow].MergeCells(nexColBA, nexColBA2);
                    }
                    else
                    {
                        if (oVWRKSVendors.hps[i].IsRange == true)
                        {
                            table.Rows[indexRow].Cells[nexColBA + 2].Paragraphs.First().Append(oVWRKSVendorsBA.vendors[z].itemAsuransi[i].RateLowerLimit == null ? "" : oVWRKSVendorsBA.vendors[z].itemAsuransi[i].RateLowerLimit.ToString());
                            table.Rows[indexRow].Cells[nexColBA2 + 2].Paragraphs.First().Append(oVWRKSVendorsBA.vendors[z].itemAsuransi[i].RateUpperLimit == null ? "" : oVWRKSVendorsBA.vendors[z].itemAsuransi[i].RateUpperLimit.ToString());
                            nexColBA++;
                            nexColBA2++;
                        }
                        if (oVWRKSVendors.hps[i].IsRange == false)
                        {
                            table.Rows[indexRow].Cells[nexColBA].Paragraphs.First().Append(oVWRKSVendorsBA.vendors[z].itemAsuransi[i].Rate == null ? "" : oVWRKSVendorsBA.vendors[z].itemAsuransi[i].Rate.ToString());
                            table.Rows[indexRow].MergeCells(nexColBA, nexColBA2);
                        }
                    }
                    //nexCol++;
                    //nexCol2++;
                    nexColBA = nexColBA + 2;
                    nexColBA2 = nexColBA2 + 2;
                }

                indexRow++;
            }
            // Insert table at index where tag #TABLE# is in document.
            //doc.InsertTable(table);
            foreach (var paragraph in doc.Paragraphs)
            {
                paragraph.FindAll("{tabel}").ForEach(index => paragraph.InsertTableAfterSelf((table)));
            }
            //Remove tag
            doc.ReplaceText("{tabel}", "");

            doc.SaveAs(OutFileNama);
            streamx.Close();
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new FileStream(OutFileNama, FileMode.Open);
            result.Content = new StreamContent(stream);
            //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.wordprocessingml.document");

            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = outputFileName
            };

            return result;
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public HttpResponseMessage CetakPenilaianAsuransi(Guid Id)
        {
            var pengadaan = _repository.GetPengadaan(Id, UserId(), 0);
            var jadwalKlarifikasi = _repository.getPelaksanaanKlarifikasi(Id, UserId());
            string fileName = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Template\Klarifikasi-Pemenang-Asuransi.docx";

            string outputFileName = "Cetak-RKS-Klarifikasi-Lanjutan-Asuransi-" + UserId().ToString() + "-" + DateTime.Now.ToString("dd-MM-yy") + ".docx";

            string OutFileNama = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Temp\" + outputFileName;

            var streamx = new FileStream(fileName, FileMode.Open);

            var doc = DocX.Load(streamx);//.Create(OutFileNama);
            doc.PageLayout.Orientation = Novacode.Orientation.Landscape;
            doc.ReplaceText("{pengadaan_name_judul}", pengadaan.Judul == null ? "" : pengadaan.Judul.ToUpper());
            doc.ReplaceText("{nomor_berita_acara}", pengadaan.NoPengadaan == null ? "" : pengadaan.NoPengadaan);
            doc.ReplaceText("{pengadaan_unit_pemohon}", pengadaan.UnitKerjaPemohon == null ? "" : pengadaan.UnitKerjaPemohon);


            var oVWRKSVendors = _repository.getRKSPenilaianAsuransiNilai(pengadaan.Id, UserId());
            var table = doc.AddTable(oVWRKSVendors.hps.Count() + 1, (oVWRKSVendors.vendors.Count * 2) + 6);
            Border BlankBorder = new Border(BorderStyle.Tcbs_single, BorderSize.one, 0, Color.Black);
            table.SetBorder(TableBorderType.Bottom, BlankBorder);
            table.SetBorder(TableBorderType.Left, BlankBorder);
            table.SetBorder(TableBorderType.Right, BlankBorder);
            table.SetBorder(TableBorderType.Top, BlankBorder);
            table.SetBorder(TableBorderType.InsideV, BlankBorder);
            table.SetBorder(TableBorderType.InsideH, BlankBorder);

            int indexRow = 0;
            table.Rows[indexRow].Cells[0].Paragraphs.First().Append("NO");
            table.Rows[indexRow].Cells[0].Width = 10;
            table.Rows[indexRow].Cells[1].Paragraphs.First().Append("Benefit");
            table.Rows[indexRow].Cells[2].Paragraphs.First().Append("Coverage");
            table.Rows[indexRow].Cells[3].Paragraphs.First().Append("Region");
            table.Rows[indexRow].Cells[4].Paragraphs.First().Append("Rate");
            table.Rows[indexRow].MergeCells(4, 5);
            int headerCol = 5;
            int headerCol2 = 6;
            //List<int> countList = new List<int>();
            //List<string> NameList = new List<string>();
            foreach (var item in oVWRKSVendors.vendors)
            {
                table.Rows[indexRow].Cells[headerCol].Paragraphs.First().Append(item.NamaVendor);
                table.Rows[indexRow].MergeCells(headerCol, headerCol2);
                headerCol++;
                headerCol2++;
            }
            indexRow++;
            int no = 1;
            //decimal? subtotal = 0;
            var itemlast = oVWRKSVendors.hps.Last();
            for (var i = 0; i < oVWRKSVendors.hps.Count(); i++)
            {
                //var currentGroup = item.grup;
                //var index = oVWRKSVendors.hps.IndexOf(i);
                table.Rows[indexRow].Cells[0].Paragraphs.First().Append(no.ToString());
                table.Rows[indexRow].Cells[0].Width = 10;
                no++;
                //Regex example #1 "<.*?>"
                //string dekripsi = "";
                //if (item.item != null) dekripsi = Regex.Replace(item.item, @"<.*?>", string.Empty);
                //Regex example #2
                // string result2 = Regex.Replace(dekripsi, @"<[^>].+?>", "");
                table.Rows[indexRow].Cells[1].Paragraphs.First().Append(oVWRKSVendors.hps[i].BenefitCode != "undefined" ? oVWRKSVendors.hps[i].BenefitCode : "");
                table.Rows[indexRow].Cells[2].Paragraphs.First().Append(oVWRKSVendors.hps[i].BenefitCoverage != "undefined" ? oVWRKSVendors.hps[i].BenefitCoverage : "");
                table.Rows[indexRow].Cells[3].Paragraphs.First().Append(oVWRKSVendors.hps[i].RegionCode != "undefined" ? oVWRKSVendors.hps[i].RegionCode : "");
                if (oVWRKSVendors.hps[i].IsOpen == true)
                {
                    table.Rows[indexRow].Cells[4].Paragraphs.First().Append("Perlu Underwriting");
                    table.Rows[indexRow].MergeCells(4, 5);
                }
                else
                {
                    if (oVWRKSVendors.hps[i].IsRange == true)
                    {
                        table.Rows[indexRow].Cells[4].Paragraphs.First().Append(oVWRKSVendors.hps[i].RateLowerLimit == null ? "" : oVWRKSVendors.hps[i].RateLowerLimit.ToString());
                        table.Rows[indexRow].Cells[5].Paragraphs.First().Append(oVWRKSVendors.hps[i].RateUpperLimit == null ? "" : oVWRKSVendors.hps[i].RateUpperLimit.ToString());
                    }
                    if (oVWRKSVendors.hps[i].IsRange == false)
                    {
                        table.Rows[indexRow].Cells[4].Paragraphs.First().Append(oVWRKSVendors.hps[i].Rate == null ? "" : oVWRKSVendors.hps[i].Rate.ToString());
                        table.Rows[indexRow].MergeCells(4, 5);
                    }

                }

                /*sub totol asumsi kalo next group nya beda*/
                int nexCol = 5;
                int nexCol2 = 6;

                for (var z = 0; z < oVWRKSVendors.vendors.Count(); z++)
                {
                    if (oVWRKSVendors.hps[i].IsOpen == true)
                    {
                        table.Rows[indexRow].Cells[nexCol].Paragraphs.First().Append("Perlu Underwriting");
                        table.Rows[indexRow].MergeCells(nexCol, nexCol2);
                    }
                    else
                    {
                        if (oVWRKSVendors.hps[i].IsRange == true)
                        {
                            table.Rows[indexRow].Cells[nexCol + 1].Paragraphs.First().Append(oVWRKSVendors.vendors[z].itemAsuransi[i].RateLowerLimit == null ? "" : oVWRKSVendors.vendors[z].itemAsuransi[i].RateLowerLimit.ToString());
                            table.Rows[indexRow].Cells[nexCol2 + 1].Paragraphs.First().Append(oVWRKSVendors.vendors[z].itemAsuransi[i].RateUpperLimit == null ? "" : oVWRKSVendors.vendors[z].itemAsuransi[i].RateUpperLimit.ToString());
                            nexCol++;
                            nexCol2++;
                        }
                        if (oVWRKSVendors.hps[i].IsRange == false)
                        {
                            table.Rows[indexRow].Cells[nexCol].Paragraphs.First().Append(oVWRKSVendors.vendors[z].itemAsuransi[i].Rate == null ? "" : oVWRKSVendors.vendors[z].itemAsuransi[i].Rate.ToString());
                            table.Rows[indexRow].MergeCells(nexCol, nexCol2);
                        }
                    }
                    nexCol++;
                    nexCol2++;
                    //nexCol = nexCol + 2;
                    //nexCol2 = nexCol2 + 2;
                }

                indexRow++;
            }
            // Insert table at index where tag #TABLE# is in document.
            //doc.InsertTable(table);
            foreach (var paragraph in doc.Paragraphs)
            {
                paragraph.FindAll("{tabel}").ForEach(index => paragraph.InsertTableAfterSelf((table)));
            }
            //Remove tag
            doc.ReplaceText("{tabel}", "");

            doc.SaveAs(OutFileNama);
            streamx.Close();
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new FileStream(OutFileNama, FileMode.Open);
            result.Content = new StreamContent(stream);
            //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.wordprocessingml.document");

            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = outputFileName
            };

            return result;
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public HttpResponseMessage CetakRKSPenilaianAsuransiAllXls(Guid Id)
        {
            var pengadaan = _repository.GetPengadaan(Id, UserId(), 0);
            var jadwalKlarifikasi = _repository.getPelaksanaanKlarifikasi(Id, UserId());

            string outputFileName = "Cetak-RKS-Penilaian-Asuransi-" + UserId().ToString() + "-" + DateTime.Now.ToString("dd-MM-yy") + ".xlsx";

            var Judul = pengadaan.Judul == null ? "" : pengadaan.Judul.ToUpper();
            var NoPengadaan = pengadaan.NoPengadaan == null ? "" : pengadaan.NoPengadaan;
            var UnitPemohon = pengadaan.UnitKerjaPemohon == null ? "" : pengadaan.UnitKerjaPemohon;


            var oVWRKSVendors = _repository.getRKSPenilaianAsuransi(pengadaan.Id, UserId());


            var ms = new System.IO.MemoryStream();
            try
            {
                using (var sl = new SpreadsheetLight.SLDocument())
                {
                    sl.SetCellValue(1, 1, "Proyek");
                    sl.SetCellValue(1, 2, UnitPemohon);
                    sl.SetCellValue(2, 1, "Judul");
                    sl.SetCellValue(2, 2, Judul);
                    sl.SetCellValue(3, 1, "Penawaran Harga Klarifikasi dan Negoisasi Rekanan sebagai berikut:");
                    var rowNum = 4;
                    //write header
                    sl.SetCellValue(rowNum, 1, "No");
                    sl.SetCellValue(rowNum, 2, "Benefit");
                    sl.SetCellValue(rowNum, 3, "Coverage");
                    sl.SetCellValue(rowNum, 4, "Region");
                    sl.SetCellValue(rowNum, 5, "Rate");
                    sl.MergeWorksheetCells(rowNum, 5, rowNum, 6);

                    int headerCol = 7;
                    int headerCol2 = 8;
                    foreach (var item in oVWRKSVendors.vendors)
                    {
                        sl.SetCellValue(rowNum, headerCol, item.NamaVendor);
                        sl.MergeWorksheetCells(rowNum, headerCol, rowNum, headerCol2);
                        headerCol = headerCol + 2;
                        headerCol2 = headerCol2 + 2;
                    }
                    rowNum++;
                    //write data
                    var itemlast = oVWRKSVendors.hps.Last();
                    // decimal? subtotal = 0;
                    int no = 1;
                    for (var i = 0; i < oVWRKSVendors.hps.Count(); i++)
                    {
                        sl.SetCellValue(rowNum, 1, no);
                        no++;
                        sl.SetCellValue(rowNum, 2, oVWRKSVendors.hps[i].BenefitCode != "undefined" ? oVWRKSVendors.hps[i].BenefitCode : "");
                        sl.SetCellValue(rowNum, 3, oVWRKSVendors.hps[i].BenefitCoverage != "undefined" ? oVWRKSVendors.hps[i].BenefitCoverage : "");
                        sl.SetCellValue(rowNum, 4, oVWRKSVendors.hps[i].RegionCode != "undefined" ? oVWRKSVendors.hps[i].RegionCode : "");
                        if (oVWRKSVendors.hps[i].IsOpen == true)
                        {
                            sl.SetCellValue(rowNum, 5, "Perlu Underwriting");
                            sl.MergeWorksheetCells(rowNum, 5, rowNum, 6);
                        }
                        else
                        {
                            if (oVWRKSVendors.hps[i].IsRange == false)
                            {
                                sl.SetCellValue(rowNum, 5, (oVWRKSVendors.hps[i].Rate == null ? "" : oVWRKSVendors.hps[i].Rate.ToString()));
                                sl.MergeWorksheetCells(rowNum, 5, rowNum, 6);
                            }
                            if (oVWRKSVendors.hps[i].IsRange == true)
                            {
                                sl.SetCellValue(rowNum, 5, (oVWRKSVendors.hps[i].RateLowerLimit == null ? "" : oVWRKSVendors.hps[i].RateLowerLimit.ToString()));
                                sl.SetCellValue(rowNum, 6, (oVWRKSVendors.hps[i].RateUpperLimit == null ? "" : oVWRKSVendors.hps[i].RateUpperLimit.ToString()));
                            }
                        }
                        int nextCol = 7;
                        int nextCol2 = 8;
                        for (var z = 0; z < oVWRKSVendors.vendors.Count(); z++)
                        {
                            if (oVWRKSVendors.hps[i].IsOpen == true)
                            {
                                sl.SetCellValue(rowNum, nextCol, "Perlu Underwriting");
                                sl.MergeWorksheetCells(rowNum, nextCol, rowNum, nextCol2);
                            }
                            else
                            {
                                if (oVWRKSVendors.hps[i].IsRange == false)
                                {
                                    sl.SetCellValue(rowNum, nextCol, (oVWRKSVendors.vendors[z].itemAsuransi[i].Rate == null ? "" : oVWRKSVendors.vendors[z].itemAsuransi[i].Rate.ToString()));
                                    sl.MergeWorksheetCells(rowNum, nextCol, rowNum, nextCol2);
                                }
                                if (oVWRKSVendors.hps[i].IsRange == true)
                                {
                                    sl.SetCellValue(rowNum, nextCol, (oVWRKSVendors.vendors[z].itemAsuransi[i].RateLowerLimit == null ? "" : oVWRKSVendors.vendors[z].itemAsuransi[i].RateLowerLimit.ToString()));
                                    sl.SetCellValue(rowNum, nextCol2, (oVWRKSVendors.vendors[z].itemAsuransi[i].RateUpperLimit == null ? "" : oVWRKSVendors.vendors[z].itemAsuransi[i].RateUpperLimit.ToString()));
                                }
                            }

                            nextCol = nextCol + 2;
                            nextCol2 = nextCol2 + 2;
                        }
                        rowNum++;
                    }

                    sl.SetColumnWidth(1, 10.0);

                    sl.SaveAs(ms);
                }
            }
            catch { }

            ms.Position = 0;
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            //  var stream = new FileStream(ms, FileMode.Open);
            result.Content = new StreamContent(ms);
            //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = outputFileName
            };
            return result;
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public HttpResponseMessage CetakRKSKlarfikasiAsuransiXls(Guid Id)
        {
            var pengadaan = _repository.GetPengadaan(Id, UserId(), 0);
            var jadwalKlarifikasi = _repository.getPelaksanaanKlarifikasi(Id, UserId());

            string outputFileName = "Cetak-RKS-Klarifikasi-Asuransi-" + UserId().ToString() + "-" + DateTime.Now.ToString("dd-MM-yy") + ".xlsx";

            var Judul = pengadaan.Judul == null ? "" : pengadaan.Judul.ToUpper();
            var NoPengadaan = pengadaan.NoPengadaan == null ? "" : pengadaan.NoPengadaan;
            var UnitPemohon = pengadaan.UnitKerjaPemohon == null ? "" : pengadaan.UnitKerjaPemohon;

            var oVWRKSVendorsBA = _repository.getRKSPenilaianAsuransi(pengadaan.Id, UserId());
            var oVWRKSVendors = _repository.getRKSKlarifikasiAsuransi(pengadaan.Id, UserId());


            var ms = new System.IO.MemoryStream();
            try
            {
                using (var sl = new SpreadsheetLight.SLDocument())
                {
                    sl.SetCellValue(1, 1, "Proyek");
                    sl.SetCellValue(1, 2, UnitPemohon);
                    sl.SetCellValue(2, 1, "Judul");
                    sl.SetCellValue(2, 2, Judul);
                    sl.SetCellValue(3, 1, "Penawaran Harga Klarifikasi dan Negoisasi Rekanan sebagai berikut:");
                    var rowNum = 4;
                    //write header
                    sl.SetCellValue(rowNum, 1, "No");
                    sl.SetCellValue(rowNum, 2, "Benefit");
                    sl.SetCellValue(rowNum, 3, "Coverage");
                    sl.SetCellValue(rowNum, 4, "Region");
                    sl.SetCellValue(rowNum, 5, "Rate");
                    sl.MergeWorksheetCells(rowNum, 5, rowNum, 6);

                    int headerCol = 9;
                    int headerCol2 = 10;
                    foreach (var item in oVWRKSVendors.vendors)
                    {
                        sl.SetCellValue(rowNum, headerCol, item.NamaVendor);
                        sl.MergeWorksheetCells(rowNum, headerCol, rowNum, headerCol2);
                        headerCol = headerCol + 4;
                        headerCol2 = headerCol2 + 4;
                    }
                    int headerColBA = 7;
                    int headerColBA2 = 8;
                    foreach (var itemBA in oVWRKSVendorsBA.vendors)
                    {
                        sl.SetCellValue(rowNum, headerColBA, itemBA.NamaVendor + " (Buka Amplop)");
                        //sl.SetCellValue(rowNum, headerColBA, " (Buka Amplop)");
                        sl.MergeWorksheetCells(rowNum, headerColBA, rowNum, headerColBA2);
                        headerColBA = headerColBA + 4;
                        headerColBA2 = headerColBA2 + 4;
                    }
                    rowNum++;
                    //write data
                    var itemlast = oVWRKSVendors.hps.Last();
                    // decimal? subtotal = 0;
                    int no = 1;
                    for (var i = 0; i < oVWRKSVendors.hps.Count(); i++)
                    {
                        sl.SetCellValue(rowNum, 1, no);
                        no++;
                        sl.SetCellValue(rowNum, 2, oVWRKSVendors.hps[i].BenefitCode != "undefined" ? oVWRKSVendors.hps[i].BenefitCode : "");
                        sl.SetCellValue(rowNum, 3, oVWRKSVendors.hps[i].BenefitCoverage != "undefined" ? oVWRKSVendors.hps[i].BenefitCoverage : "");
                        sl.SetCellValue(rowNum, 4, oVWRKSVendors.hps[i].RegionCode != "undefined" ? oVWRKSVendors.hps[i].RegionCode : "");
                        if (oVWRKSVendors.hps[i].IsOpen == true)
                        {
                            sl.SetCellValue(rowNum, 5, "Perlu Underwriting");
                            sl.MergeWorksheetCells(rowNum, 5, rowNum, 6);
                        }
                        else
                        {
                            if (oVWRKSVendors.hps[i].IsRange == false)
                            {
                                sl.SetCellValue(rowNum, 5, (oVWRKSVendors.hps[i].Rate == null ? "" : oVWRKSVendors.hps[i].Rate.ToString()));
                                sl.MergeWorksheetCells(rowNum, 5, rowNum, 6);
                            }
                            if (oVWRKSVendors.hps[i].IsRange == true)
                            {
                                sl.SetCellValue(rowNum, 5, (oVWRKSVendors.hps[i].RateLowerLimit == null ? "" : oVWRKSVendors.hps[i].RateLowerLimit.ToString()));
                                sl.SetCellValue(rowNum, 6, (oVWRKSVendors.hps[i].RateUpperLimit == null ? "" : oVWRKSVendors.hps[i].RateUpperLimit.ToString()));
                            }
                        }
                        int nextColBA = 7;
                        int nextColBA2 = 8;
                        int nextCol = 9;
                        int nextCol2 = 10;
                        for (var z = 0; z < oVWRKSVendors.vendors.Count(); z++)
                        {

                            if (oVWRKSVendors.hps[i].IsOpen == true)
                            {
                                sl.SetCellValue(rowNum, nextCol, "Perlu Underwriting");
                                sl.MergeWorksheetCells(rowNum, nextCol, rowNum, nextCol2);
                            }
                            else
                            {
                                if (oVWRKSVendors.hps[i].IsRange == false)
                                {
                                    sl.SetCellValue(rowNum, nextCol, (oVWRKSVendors.vendors[z].itemAsuransi[i].Rate == null ? "" : oVWRKSVendors.vendors[z].itemAsuransi[i].Rate.ToString()));
                                    sl.MergeWorksheetCells(rowNum, nextCol, rowNum, nextCol2);
                                }
                                if (oVWRKSVendors.hps[i].IsRange == true)
                                {
                                    sl.SetCellValue(rowNum, nextCol, (oVWRKSVendors.vendors[z].itemAsuransi[i].RateLowerLimit == null ? "" : oVWRKSVendors.vendors[z].itemAsuransi[i].RateLowerLimit.ToString()));
                                    sl.SetCellValue(rowNum, nextCol2, (oVWRKSVendors.vendors[z].itemAsuransi[i].RateUpperLimit == null ? "" : oVWRKSVendors.vendors[z].itemAsuransi[i].RateUpperLimit.ToString()));
                                }
                            }

                            nextCol = nextCol + 4;
                            nextCol2 = nextCol2 + 4;
                        }

                        for (var zz = 0; zz < oVWRKSVendorsBA.vendors.Count(); zz++)
                        {
                            if (oVWRKSVendors.hps[i].IsOpen == true)
                            {
                                sl.SetCellValue(rowNum, nextColBA, "Perlu Underwriting");
                                sl.MergeWorksheetCells(rowNum, nextColBA, rowNum, nextColBA2);
                            }
                            else
                            {
                                if (oVWRKSVendors.hps[i].IsRange == false)
                                {
                                    sl.SetCellValue(rowNum, nextColBA, (oVWRKSVendorsBA.vendors[zz].itemAsuransi[i].Rate == null ? "" : oVWRKSVendorsBA.vendors[zz].itemAsuransi[i].Rate.ToString()));
                                    sl.MergeWorksheetCells(rowNum, nextColBA, rowNum, nextColBA2);
                                }
                                if (oVWRKSVendors.hps[i].IsRange == true)
                                {
                                    sl.SetCellValue(rowNum, nextColBA, (oVWRKSVendorsBA.vendors[zz].itemAsuransi[i].RateLowerLimit == null ? "" : oVWRKSVendorsBA.vendors[zz].itemAsuransi[i].RateLowerLimit.ToString()));
                                    sl.SetCellValue(rowNum, nextColBA2, (oVWRKSVendorsBA.vendors[zz].itemAsuransi[i].RateUpperLimit == null ? "" : oVWRKSVendorsBA.vendors[zz].itemAsuransi[i].RateUpperLimit.ToString()));
                                }
                            }

                            nextColBA = nextColBA + 4;
                            nextColBA2 = nextColBA2 + 4;
                        }
                        rowNum++;
                    }

                    sl.SetColumnWidth(1, 10.0);

                    sl.SaveAs(ms);
                }
            }
            catch { }

            ms.Position = 0;
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            //  var stream = new FileStream(ms, FileMode.Open);
            result.Content = new StreamContent(ms);
            //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = outputFileName
            };
            return result;
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public HttpResponseMessage CetakRKSKlarifikasiLanjutanAllXls(Guid Id)
        {
            var pengadaan = _repository.GetPengadaan(Id, UserId(), 0);
            var jadwalKlarifikasi = _repository.getPelaksanaanKlarifikasi(Id, UserId());

            string outputFileName = "Cetak-RKS-Klarifikasi-Lanjutan-Asuransi-" + UserId().ToString() + "-" + DateTime.Now.ToString("dd-MM-yy") + ".xlsx";

            var Judul = pengadaan.Judul == null ? "" : pengadaan.Judul.ToUpper();
            var NoPengadaan = pengadaan.NoPengadaan == null ? "" : pengadaan.NoPengadaan;
            var UnitPemohon = pengadaan.UnitKerjaPemohon == null ? "" : pengadaan.UnitKerjaPemohon;


            var oVWRKSVendors = _repository.getRKSPenilaianKlarifikasiLanjutanAsuransi(pengadaan.Id, UserId());


            var ms = new System.IO.MemoryStream();
            try
            {
                using (var sl = new SpreadsheetLight.SLDocument())
                {
                    sl.SetCellValue(1, 1, "Proyek");
                    sl.SetCellValue(1, 2, UnitPemohon);
                    sl.SetCellValue(2, 1, "Judul");
                    sl.SetCellValue(2, 2, Judul);
                    sl.SetCellValue(3, 1, "Penawaran Harga Klarifikasi dan Negoisasi Rekanan sebagai berikut:");
                    var rowNum = 4;
                    //write header
                    sl.SetCellValue(rowNum, 1, "No");
                    sl.SetCellValue(rowNum, 2, "Benefit");
                    sl.SetCellValue(rowNum, 3, "Coverage");
                    sl.SetCellValue(rowNum, 4, "Region");
                    sl.SetCellValue(rowNum, 5, "Rate");
                    sl.MergeWorksheetCells(rowNum, 5, rowNum, 6);

                    int headerCol = 7;
                    int headerCol2 = 8;
                    foreach (var item in oVWRKSVendors.vendors)
                    {
                        sl.SetCellValue(rowNum, headerCol, item.NamaVendor);
                        sl.MergeWorksheetCells(rowNum, headerCol, rowNum, headerCol2);
                        headerCol = headerCol + 2;
                        headerCol2 = headerCol2 + 2;
                    }
                    rowNum++;
                    //write data
                    var itemlast = oVWRKSVendors.hps.Last();
                    // decimal? subtotal = 0;
                    int no = 1;
                    for (var i = 0; i < oVWRKSVendors.hps.Count(); i++)
                    {
                        sl.SetCellValue(rowNum, 1, no);
                        no++;
                        sl.SetCellValue(rowNum, 2, oVWRKSVendors.hps[i].BenefitCode != "undefined" ? oVWRKSVendors.hps[i].BenefitCode : "");
                        sl.SetCellValue(rowNum, 3, oVWRKSVendors.hps[i].BenefitCoverage != "undefined" ? oVWRKSVendors.hps[i].BenefitCoverage : "");
                        sl.SetCellValue(rowNum, 4, oVWRKSVendors.hps[i].RegionCode != "undefined" ? oVWRKSVendors.hps[i].RegionCode : "");
                        if (oVWRKSVendors.hps[i].IsOpen == true)
                        {
                            sl.SetCellValue(rowNum, 5, "Perlu Underwriting");
                            sl.MergeWorksheetCells(rowNum, 5, rowNum, 6);
                        }
                        else
                        {
                            if (oVWRKSVendors.hps[i].IsRange == false)
                            {
                                sl.SetCellValue(rowNum, 5, (oVWRKSVendors.hps[i].Rate == null ? "" : oVWRKSVendors.hps[i].Rate.ToString()));
                                sl.MergeWorksheetCells(rowNum, 5, rowNum, 6);
                            }
                            if (oVWRKSVendors.hps[i].IsRange == true)
                            {
                                sl.SetCellValue(rowNum, 5, (oVWRKSVendors.hps[i].RateLowerLimit == null ? "" : oVWRKSVendors.hps[i].RateLowerLimit.ToString()));
                                sl.SetCellValue(rowNum, 6, (oVWRKSVendors.hps[i].RateUpperLimit == null ? "" : oVWRKSVendors.hps[i].RateUpperLimit.ToString()));
                            }
                        }
                        int nextCol = 7;
                        int nextCol2 = 8;
                        for (var z = 0; z < oVWRKSVendors.vendors.Count(); z++)
                        {
                            if (oVWRKSVendors.hps[i].IsOpen == true)
                            {
                                sl.SetCellValue(rowNum, nextCol, "Perlu Underwriting");
                                sl.MergeWorksheetCells(rowNum, nextCol, rowNum, nextCol2);
                            }
                            else
                            {
                                if (oVWRKSVendors.hps[i].IsRange == false)
                                {
                                    sl.SetCellValue(rowNum, nextCol, (oVWRKSVendors.vendors[z].itemAsuransi[i].Rate == null ? "" : oVWRKSVendors.vendors[z].itemAsuransi[i].Rate.ToString()));
                                    sl.MergeWorksheetCells(rowNum, nextCol, rowNum, nextCol2);
                                }
                                if (oVWRKSVendors.hps[i].IsRange == true)
                                {
                                    sl.SetCellValue(rowNum, nextCol, (oVWRKSVendors.vendors[z].itemAsuransi[i].RateLowerLimit == null ? "" : oVWRKSVendors.vendors[z].itemAsuransi[i].RateLowerLimit.ToString()));
                                    sl.SetCellValue(rowNum, nextCol2, (oVWRKSVendors.vendors[z].itemAsuransi[i].RateUpperLimit == null ? "" : oVWRKSVendors.vendors[z].itemAsuransi[i].RateUpperLimit.ToString()));
                                }
                            }

                            nextCol = nextCol + 2;
                            nextCol2 = nextCol2 + 2;
                        }
                        rowNum++;
                    }

                    sl.SetColumnWidth(1, 10.0);

                    sl.SaveAs(ms);
                }
            }
            catch { }

            ms.Position = 0;
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            //  var stream = new FileStream(ms, FileMode.Open);
            result.Content = new StreamContent(ms);
            //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = outputFileName
            };
            return result;
        }


        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public HttpResponseMessage AssessmentPenilaian(Guid Id)
        {
            var pengadaan = _repository.GetPengadaan(Id, UserId(), 0);
            string outputFileName = "Cetak-Assessment-Penilaian" + UserId().ToString() + "-" + DateTime.Now.ToString("dd-MM-yy") + ".xlsx";
            string OutFileNama = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Temp\" + outputFileName;

            var reportassessment = _repository.ReportAssessment(Id);
            var refdata = _repository.cekmasterdata(Id);
            var penilaian = _repository.UrutanPertanyaan(Id);
            //Jaw's Logic
            var nilaibobot = _repository.nilaibobot(Id);
            var detailnilaibobot = _repository.detailnilaibobot(Id);
            //var bobot = _repository.BobotPenilaian(Id);
            var ms = new System.IO.MemoryStream();
            try
            {
                using (var sl = new SpreadsheetLight.SLDocument())
                {
                    var merge = 5;
                    var merger = 5;
                    var rowIndex = 5;
                    var columnIndex = 5;
                    var columnIndexNilai = 5;
                    var nilverage = 5;
                    var number = 1;
                    string[] pointpenilaian = new string[20];
                    string[] barispenilaian = new string[20];
                    string[] sivendor = new string[20];
                    string[] kolomvendor = new string[20];
                    int kyt = 0;

                    SpreadsheetLight.SLStyle style = sl.CreateStyle();
                    style.SetFontBold(true);
                    style.Alignment.Horizontal = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
                    style.Alignment.Vertical = DocumentFormat.OpenXml.Spreadsheet.VerticalAlignmentValues.Center;
                    style.Border.LeftBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Thin;
                    style.Border.RightBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Thin;
                    style.Border.BottomBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Thin;
                    style.Border.TopBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Thin;

                    SpreadsheetLight.SLStyle stylevalue = sl.CreateStyle();
                    stylevalue.Alignment.Horizontal = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
                    stylevalue.Alignment.Vertical = DocumentFormat.OpenXml.Spreadsheet.VerticalAlignmentValues.Center;
                    stylevalue.Border.LeftBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Thin;
                    stylevalue.Border.RightBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Thin;
                    stylevalue.Border.BottomBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Thin;
                    stylevalue.Border.TopBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Thin;

                    SpreadsheetLight.SLStyle styleitem = sl.CreateStyle();
                    styleitem.Border.LeftBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Thin;
                    styleitem.Border.RightBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Thin;
                    styleitem.Border.BottomBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Thin;
                    styleitem.Border.TopBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Thin;

                    sl.MergeWorksheetCells(2, 2, 4, 2);
                    sl.SetCellValue(2, 2, "No");
                    sl.SetCellStyle(3, 2, style);
                    sl.SetCellStyle(4, 2, style);
                    sl.SetCellStyle(2, 2, style);
                    sl.MergeWorksheetCells(2, 3, 4, 3);
                    sl.SetCellValue(2, 3, "Kriteria Penilaian");
                    sl.SetCellStyle(2, 3, style);
                    sl.SetCellStyle(3, 3, style);
                    sl.SetCellStyle(4, 3, style);
                    sl.MergeWorksheetCells(2, 4, 4, 4);
                    sl.SetCellValue(2, 4, "Bobot Nilai");
                    sl.SetCellStyle(2, 4, style);
                    sl.SetCellStyle(3, 4, style);
                    sl.SetCellStyle(4, 4, style);

                    for (var i = 0; i < refdata.Count(); i++)
                    {
                        pointpenilaian[i] = penilaian[i].pertanyaan;
                    }

                    //bobot
                    int[] BobotNilai = new int[20];
                    for (var i = 0; i < refdata.Count(); i++)
                    {
                        BobotNilai[i] = refdata[i].Bobot;
                    }

                    for (var i = 0; i < refdata.Count(); i++)
                    {
                        sl.SetCellValue(rowIndex, 2, number);
                        sl.SetCellStyle(rowIndex, 2, style);
                        sl.SetCellValue(rowIndex, 3, pointpenilaian[i]);
                        sl.SetCellStyle(rowIndex, 3, styleitem);
                        sl.SetCellValue(rowIndex, 4, BobotNilai[i]);
                        sl.SetCellStyle(rowIndex, 4, style);
                        rowIndex++;
                        number++;
                    }

                    for (var i = 0; i < reportassessment.TenderScoringHeaders.Count(); i++)
                    {
                        sl.SetCellValue(4, nilverage, "Nilai");
                        sl.SetCellStyle(4, nilverage, style);
                        sl.SetColumnWidth(4, nilverage, 15.0);
                        sl.SetCellValue(4, nilverage + 1, "Average");
                        sl.SetCellStyle(4, nilverage + 1, style);
                        sl.SetColumnWidth(4, nilverage + 1, 15.0);

                        sl.SetCellValue(rowIndex, nilverage, reportassessment.TenderScoringHeaders[i].Total.ToString());
                        sl.SetCellStyle(rowIndex, nilverage, style);
                        sl.SetCellValue(rowIndex, nilverage + 1, reportassessment.TenderScoringHeaders[i].Averages.ToString());
                        sl.SetCellStyle(rowIndex, nilverage + 1, style);

                        nilverage = nilverage + 2;
                        merger = merger + 2;
                    }

                    for (var i = 0; i < penilaian.Count(); i++)
                    {
                        sivendor[kyt] = penilaian[i].namasivendor;
                        kyt++;
                        i = (i + (penilaian.Count() / reportassessment.Vendors.Count())) - 1;
                    }

                    for (var i = 0; i < reportassessment.Vendors.Count(); i++)
                    {
                        sl.MergeWorksheetCells(3, merge, 3, merge + 1);
                        sl.SetCellValue(3, columnIndex, sivendor[i]);
                        sl.SetCellStyle(3, columnIndex, style);
                        sl.SetCellStyle(3, merge, style);
                        sl.SetCellStyle(3, merge + 1, style);

                        columnIndex = columnIndex + 2;
                        merge = merge + 2;

                        sl.MergeWorksheetCells(2, 5, 2, merger - 1);
                        sl.SetCellValue(2, 5, "Hasil Penilaian");
                        sl.SetCellStyle(2, 5, style);
                        var h = merge - 5;
                        for (var q = 1; q < h; q++)
                        {
                            sl.SetCellStyle(2, 5 + q, style);
                        }
                        sl.SetCellStyle(2, merge - 1, style);
                    }

                    var k = 0;
                    var m = 0;
                    var n = 0;

                    for (var j = 0; j < reportassessment.TenderScoringDetails.Count(); j++)
                    {
                        if (m + 1 == reportassessment.ReferenceDatas.Count())
                        {
                            sl.SetCellValue(5 + k, columnIndexNilai, reportassessment.TenderScoringDetails[j].Total_All_User.ToString());
                            sl.SetCellStyle(5 + k, columnIndexNilai, stylevalue);
                            sl.SetCellValue(5 + k, columnIndexNilai + 1, reportassessment.TenderScoringDetails[j].Averages_All_User.ToString());
                            sl.SetCellStyle(5 + k, columnIndexNilai + 1, stylevalue);
                            k = 0;
                            m = 0;
                            columnIndexNilai = columnIndexNilai + 2;
                            n = n + 1;
                        }
                        if (n == j)
                        {
                            sl.SetCellValue(5 + k, columnIndexNilai, reportassessment.TenderScoringDetails[j].Total_All_User.ToString());
                            sl.SetCellStyle(5 + k, columnIndexNilai, stylevalue);
                            sl.SetCellValue(5 + k, columnIndexNilai + 1, reportassessment.TenderScoringDetails[j].Averages_All_User.ToString());
                            sl.SetCellStyle(5 + k, columnIndexNilai + 1, stylevalue);
                            k = k + 1;
                            m = m + 1;
                            n = n + 1;
                        }
                    }

                    sl.MergeWorksheetCells(rowIndex, 2, rowIndex, 3);
                    sl.SetCellValue(rowIndex, 2, "Total");
                    sl.SetCellStyle(rowIndex, 2, style);
                    sl.SetCellStyle(rowIndex, 3, style);
                    sl.SetCellStyle(rowIndex, 4, style);


                    //========================================//

                    var rowIndex2 = 15;
                    var columnIndex2 = 4;
                    var merge2 = 4;
                    var merger2 = 4;
                    var row = 15;

                    //untuk Nama Vendor
                    sl.MergeWorksheetCells(12, 2, 14, 2); //baris-kolom
                    sl.SetCellValue(12, 2, "Nama Vendor"); //baris-kolom
                    sl.SetCellStyle(12, 2, style);
                    sl.SetCellStyle(13, 2, style);
                    sl.SetCellStyle(14, 2, style);
                    //Untuk Nama Penilai
                    sl.MergeWorksheetCells(12, 3, 14, 3);
                    sl.SetCellValue(12, 3, "Nama Penilai");
                    sl.SetCellStyle(12, 3, style);
                    sl.SetCellStyle(13, 3, style);
                    sl.SetCellStyle(14, 3, style);
                    // Untuk Header Kriteria Peilaian
                    int colpertanyaan = nilaibobot.Count();
                    var xx = 4 + colpertanyaan;
                    sl.MergeWorksheetCells(12, 4, 12, xx - 1);
                    sl.SetCellValue(12, 4, "Kriteria Penilaian (100 %)");
                    sl.SetCellStyle(12, 4, style);
                    for (var i = 0; i < colpertanyaan; i++)
                    {
                        sl.SetCellStyle(12, 4 + i, style);
                    }
                    sl.SetCellStyle(12, xx - 1, style);
                    //Untuk Header Sub Total
                    sl.MergeWorksheetCells(12, xx, 14, xx);
                    sl.SetCellValue(12, xx, "Subtotal");
                    sl.SetCellStyle(12, xx, style);
                    sl.SetCellStyle(13, xx, style);
                    sl.SetCellStyle(14, xx, style);

                    for (var i = 0; i < colpertanyaan; i++)
                    {
                        sl.SetCellValue(13, 4 + i, nilaibobot[i].pertanyaan);
                        sl.SetCellStyle(13, 4 + i, style);
                        string bbt = nilaibobot[i].Bobot + " %";
                        sl.SetCellValue(14, 4 + i, bbt);
                        sl.SetCellStyle(14, 4 + i, style);
                    }

                    int rowUser = 0;
                    int rowVendor = 15;
                    for (var z = 0; z < detailnilaibobot.ListVendor.Count(); z++)
                    {
                        rowUser = detailnilaibobot.ListUser.Where(x => x.VendorId == detailnilaibobot.ListVendor[z].thereisvendor).Count();
                        var eachuser = detailnilaibobot.ListUser.Where(x => x.VendorId == detailnilaibobot.ListVendor[z].thereisvendor).OrderBy(o => o.UserId).ToList();

                        sl.MergeWorksheetCells(rowVendor, 2, rowVendor + rowUser - 1, 2);
                        sl.SetCellValue(rowVendor, 2, detailnilaibobot.ListVendor[z].NamaVendor);
                        sl.SetCellStyle(rowVendor, 2, style);
                        int q = rowVendor + rowUser;
                        for (var i = 0; i < rowUser; i++)
                        {
                            sl.SetCellStyle(rowVendor + i, 2, style);

                            sl.SetCellValue(rowVendor + i, 3, eachuser[i].NamaUser);
                            sl.SetCellStyle(rowVendor + i, 3, style);

                            var listscore = detailnilaibobot.ListScore.Where(x => x.VendorId == detailnilaibobot.ListVendor[z].thereisvendor && x.UserId == eachuser[i].UserId).OrderBy(o => o.code).ToList();
                            //var lsSqcore = listscore.Where(ak => ak.UserId == eachuser[i].UserId).OrderBy(ak => ak.code).ToList();
                            double subtotal = 0.0;
                            for (var t = 0; t < listscore.Count(); t++)
                            {
                                double a = (double)listscore[t].Bobot.Value / 100;
                                //decimal? a = 20 / 100;
                                var b = listscore[t].Score.Value;
                                subtotal = subtotal + (a * b);
                                sl.SetCellValue(rowVendor + i, 4 + t, listscore[t].Score.Value);
                                sl.SetCellStyle(rowVendor + i, 4 + t, style);
                            }
                            sl.SetCellValue(rowVendor + i, xx, subtotal.ToString());
                            sl.SetCellStyle(rowVendor + i, xx, style);
                        }
                        sl.SetCellStyle(q - 1, 2, style);
                        rowVendor = rowVendor + rowUser;
                    }

                    //add filter
                    sl.SetColumnWidth(3, 50.0);
                    sl.SetColumnWidth(4, 15.0);
                    sl.SaveAs(ms);
                }
            }
            catch
            {
            }

            ms.Position = 0;
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            //  var stream = new FileStream(ms, FileMode.Open);
            result.Content = new StreamContent(ms);
            //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = outputFileName
            };
            return result;
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public async Task<HttpResponseMessage> BerkasBukaAmplopAsuransi(Guid Id)
        {
            var pengadaan = _repository.GetPengadaan(Id, UserId(), 0);
            var jadwalBukaAmplop = _repository.getPelaksanaanBukaAmplop(Id);
            string fileName = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Template\BERITA ACARA BUKA AMPLOP ASURANSI.docx";
            var BeritaAcara = _repository.getBeritaAcaraByTipe(Id, TipeBerkas.BeritaAcaraBukaAmplop, UserId());
            string outputFileName = "BA-Buka-Amplop-Asuransi" + (BeritaAcara == null ? "" : BeritaAcara.NoBeritaAcara.Replace("/", "-")) + "-" + DateTime.Now.ToString("dd-MM-yy") + ".docx";

            string OutFileNama = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Temp\" + outputFileName;

            var streamx = new FileStream(fileName, FileMode.Open);
            try
            {

                var doc = DocX.Load(streamx);//.Create(OutFileNama);
                doc.ReplaceText("{pengadaan_name}", pengadaan.Judul == null ? "" : pengadaan.Judul);
                doc.ReplaceText("{pengadaan_name_judul}", pengadaan.Judul == null ? "" : pengadaan.Judul.ToUpper());
                doc.ReplaceText("{nomor_berita_acara}", BeritaAcara == null ? "" : BeritaAcara.NoBeritaAcara == null ? "" : BeritaAcara.NoBeritaAcara);
                doc.ReplaceText("{pengadaan_unit_pemohon}", pengadaan.UnitKerjaPemohon == null ? "" : pengadaan.UnitKerjaPemohon);

                doc.ReplaceText("{tempat_tanggal}", BeritaAcara == null ? "" : "...............," + BeritaAcara.tanggal == null ? "................" :
                       BeritaAcara.tanggal.Value.Day + " " + Common.ConvertNamaBulan(BeritaAcara.tanggal.Value.Month) + " " +
                       BeritaAcara.tanggal.Value.Year);

                doc.ReplaceText("{pengadaan_jadwal_hari}", BeritaAcara == null ? "" : BeritaAcara.tanggal == null ? "" :
                       Common.ConvertHari(BeritaAcara.tanggal.Value.Day));
                doc.ReplaceText("{pengadaan_jadwal_tanggal}", BeritaAcara == null ? "" : BeritaAcara.tanggal.Value.Day + " " + Common.ConvertNamaBulan(BeritaAcara.tanggal.Value.Month) +
                      " " + BeritaAcara.tanggal.Value.Year);

                // Kandidat Kirim Penawaran
                var kandidat = _repository.getKandidatKirimAsuransi(Id, UserId());
                if (kandidat.Count() > 0)
                {
                    var table = doc.AddTable(kandidat.Count(), 1);
                    Border WhiteBorder2 = new Border(BorderStyle.Tcbs_none, 0, 0, Color.White);
                    table.SetBorder(TableBorderType.Bottom, WhiteBorder2);
                    table.SetBorder(TableBorderType.Left, WhiteBorder2);
                    table.SetBorder(TableBorderType.Right, WhiteBorder2);
                    table.SetBorder(TableBorderType.Top, WhiteBorder2);
                    table.SetBorder(TableBorderType.InsideV, WhiteBorder2);
                    table.SetBorder(TableBorderType.InsideH, WhiteBorder2);
                    int rowIndex2 = 0;
                    foreach (var item in kandidat)
                    {
                        table.Rows[rowIndex2].Cells[0].Paragraphs.First().Append((rowIndex2 + 1) + ". " + item.NamaVendor);
                        table.Rows[rowIndex2].Cells[0].Paragraphs.First().FontSize(11).Font(new FontFamily("Calibri"));
                        table.Rows[rowIndex2].Cells[0].Width = 500;
                        rowIndex2++;
                    }

                    table.Alignment = Alignment.left;
                    foreach (var paragraph in doc.Paragraphs)
                    {
                        paragraph.FindAll("{vendor}").ForEach(index => paragraph.InsertTableBeforeSelf(table));

                    }
                    doc.ReplaceText("{vendor}", "");
                }
                else
                {
                    doc.ReplaceText("{vendor}", "-");

                }
                // End Kandidat Kirim Penawaran

                // Kandidat Tidak Kirim Penawaran
                var kandidattidakkirim = _repository.getKandidatTidakKirimAsuransi(Id, UserId());
                if (kandidattidakkirim.Count() > 0)
                {
                    var table = doc.AddTable(kandidattidakkirim.Count(), 1);
                    Border WhiteBorder2 = new Border(BorderStyle.Tcbs_none, 0, 0, Color.White);
                    table.SetBorder(TableBorderType.Bottom, WhiteBorder2);
                    table.SetBorder(TableBorderType.Left, WhiteBorder2);
                    table.SetBorder(TableBorderType.Right, WhiteBorder2);
                    table.SetBorder(TableBorderType.Top, WhiteBorder2);
                    table.SetBorder(TableBorderType.InsideV, WhiteBorder2);
                    table.SetBorder(TableBorderType.InsideH, WhiteBorder2);
                    int rowIndex2 = 0;
                    foreach (var item in kandidattidakkirim)
                    {
                        table.Rows[rowIndex2].Cells[0].Paragraphs.First().Append((rowIndex2 + 1) + ". " + item.NamaVendor);
                        table.Rows[rowIndex2].Cells[0].Paragraphs.First().FontSize(11).Font(new FontFamily("Calibri"));
                        table.Rows[rowIndex2].Cells[0].Width = 500;
                        rowIndex2++;
                    }

                    table.Alignment = Alignment.left;
                    foreach (var paragraph in doc.Paragraphs)
                    {
                        paragraph.FindAll("{tidakkirim}").ForEach(index => paragraph.InsertTableBeforeSelf(table));

                    }
                    doc.ReplaceText("{tidakkirim}", "");
                }
                else
                {
                    doc.ReplaceText("{tidakkirim}", "-");

                }
                // End Kandidat Tidak Kirim Penawaran

                // Panitia
                var panitia = _repository.getPersonilPengadaan(Id);
                var tablePanitia = doc.AddTable(panitia.Count(), 1);
                Border WhiteBorder = new Border(BorderStyle.Tcbs_none, 0, 0, Color.White);
                tablePanitia.SetBorder(TableBorderType.Bottom, WhiteBorder);
                tablePanitia.SetBorder(TableBorderType.Left, WhiteBorder);
                tablePanitia.SetBorder(TableBorderType.Right, WhiteBorder);
                tablePanitia.SetBorder(TableBorderType.Top, WhiteBorder);
                tablePanitia.SetBorder(TableBorderType.InsideV, WhiteBorder);
                tablePanitia.SetBorder(TableBorderType.InsideH, WhiteBorder);
                int rowIndex = 0;
                foreach (var item in panitia)
                {
                    tablePanitia.Rows[rowIndex].Cells[0].Paragraphs.First().Append((rowIndex + 1) + ". " + item.Nama);
                    tablePanitia.Rows[rowIndex].Cells[0].Paragraphs.First().FontSize(11).Font(new FontFamily("Calibri"));
                    tablePanitia.Rows[rowIndex].Cells[0].Width = 500;
                    rowIndex++;
                }

                tablePanitia.Alignment = Alignment.left;
                foreach (var paragraph in doc.Paragraphs)
                {
                    paragraph.FindAll("{panitia}").ForEach(index => paragraph.InsertTableBeforeSelf(tablePanitia));

                }
                doc.ReplaceText("{panitia}", "");
                // End Panitia

                //tambah tabel persetujuan tahapan
                var table3 = await getTablePersetujuan(pengadaan.Id, EStatusPengadaan.BUKAAMPLOP, doc);

                table3.Alignment = Alignment.center;
                //table.AutoFit = AutoFit.Contents;

                foreach (var paragraph in doc.Paragraphs)
                {
                    paragraph.FindAll("{table3}").ForEach(index => paragraph.InsertTableBeforeSelf(table3));

                }
                doc.ReplaceText("{table3}", "");
                //end

                if (_repository.CekBukaAmplopTahapan(Id) == 1)
                {

                    var oVWRKSVendors = _repository.getRKSPenilaianAsuransi(pengadaan.Id, UserId());
                    var table = doc.AddTable(oVWRKSVendors.hps.Count() + 1, (oVWRKSVendors.vendors.Count * 2) + 6);
                    Border BlankBorder = new Border(BorderStyle.Tcbs_single, BorderSize.one, 0, Color.Black);
                    table.SetBorder(TableBorderType.Bottom, BlankBorder);
                    table.SetBorder(TableBorderType.Left, BlankBorder);
                    table.SetBorder(TableBorderType.Right, BlankBorder);
                    table.SetBorder(TableBorderType.Top, BlankBorder);
                    table.SetBorder(TableBorderType.InsideV, BlankBorder);
                    table.SetBorder(TableBorderType.InsideH, BlankBorder);

                    int indexRow = 0;
                    table.Rows[indexRow].Cells[0].Paragraphs.First().Append("NO");
                    table.Rows[indexRow].Cells[0].Width = 10;
                    table.Rows[indexRow].Cells[1].Paragraphs.First().Append("Benefit");
                    table.Rows[indexRow].Cells[2].Paragraphs.First().Append("Coverage");
                    table.Rows[indexRow].Cells[3].Paragraphs.First().Append("Region");
                    table.Rows[indexRow].Cells[4].Paragraphs.First().Append("Rate");
                    table.Rows[indexRow].MergeCells(4, 5);
                    int headerCol = 5;
                    int headerCol2 = 6;
                    //List<int> countList = new List<int>();
                    //List<string> NameList = new List<string>();
                    foreach (var item in oVWRKSVendors.vendors)
                    {
                        table.Rows[indexRow].Cells[headerCol].Paragraphs.First().Append(item.NamaVendor);
                        table.Rows[indexRow].MergeCells(headerCol, headerCol2);
                        headerCol++;
                        headerCol2++;
                    }
                    indexRow++;
                    int no = 1;
                    //decimal? subtotal = 0;
                    var itemlast = oVWRKSVendors.hps.Last();
                    for (var i = 0; i < oVWRKSVendors.hps.Count(); i++)
                    {
                        //var currentGroup = item.grup;
                        //var index = oVWRKSVendors.hps.IndexOf(i);
                        table.Rows[indexRow].Cells[0].Paragraphs.First().Append(no.ToString());
                        table.Rows[indexRow].Cells[0].Width = 10;
                        no++;
                        //Regex example #1 "<.*?>"
                        //string dekripsi = "";
                        //if (item.item != null) dekripsi = Regex.Replace(item.item, @"<.*?>", string.Empty);
                        //Regex example #2
                        // string result2 = Regex.Replace(dekripsi, @"<[^>].+?>", "");
                        table.Rows[indexRow].Cells[1].Paragraphs.First().Append(oVWRKSVendors.hps[i].BenefitCode != "undefined" ? oVWRKSVendors.hps[i].BenefitCode : "");
                        table.Rows[indexRow].Cells[2].Paragraphs.First().Append(oVWRKSVendors.hps[i].BenefitCoverage != "undefined" ? oVWRKSVendors.hps[i].BenefitCoverage : "");
                        table.Rows[indexRow].Cells[3].Paragraphs.First().Append(oVWRKSVendors.hps[i].RegionCode != "undefined" ? oVWRKSVendors.hps[i].RegionCode : "");
                        if (oVWRKSVendors.hps[i].IsOpen == true)
                        {
                            table.Rows[indexRow].Cells[4].Paragraphs.First().Append("Perlu Underwriting");
                            table.Rows[indexRow].MergeCells(4, 5);
                        }
                        else
                        {
                            if (oVWRKSVendors.hps[i].IsRange == true)
                            {
                                table.Rows[indexRow].Cells[4].Paragraphs.First().Append(oVWRKSVendors.hps[i].RateLowerLimit == null ? "" : oVWRKSVendors.hps[i].RateLowerLimit.ToString());
                                table.Rows[indexRow].Cells[5].Paragraphs.First().Append(oVWRKSVendors.hps[i].RateUpperLimit == null ? "" : oVWRKSVendors.hps[i].RateUpperLimit.ToString());
                            }
                            if (oVWRKSVendors.hps[i].IsRange == false)
                            {
                                table.Rows[indexRow].Cells[4].Paragraphs.First().Append(oVWRKSVendors.hps[i].Rate == null ? "" : oVWRKSVendors.hps[i].Rate.ToString());
                                table.Rows[indexRow].MergeCells(4, 5);
                            }

                        }

                        /*sub totol asumsi kalo next group nya beda*/
                        int nexCol = 5;
                        int nexCol2 = 6;

                        for (var z = 0; z < oVWRKSVendors.vendors.Count(); z++)
                        {
                            if (oVWRKSVendors.hps[i].IsOpen == true)
                            {
                                table.Rows[indexRow].Cells[nexCol].Paragraphs.First().Append("Perlu Underwriting");
                                table.Rows[indexRow].MergeCells(nexCol, nexCol2);
                            }
                            else
                            {
                                if (oVWRKSVendors.hps[i].IsRange == true)
                                {
                                    table.Rows[indexRow].Cells[nexCol + 1].Paragraphs.First().Append(oVWRKSVendors.vendors[z].itemAsuransi[i].RateLowerLimit == null ? "" : oVWRKSVendors.vendors[z].itemAsuransi[i].RateLowerLimit.ToString());
                                    table.Rows[indexRow].Cells[nexCol2 + 1].Paragraphs.First().Append(oVWRKSVendors.vendors[z].itemAsuransi[i].RateUpperLimit == null ? "" : oVWRKSVendors.vendors[z].itemAsuransi[i].RateUpperLimit.ToString());
                                    nexCol++;
                                    nexCol2++;
                                }
                                if (oVWRKSVendors.hps[i].IsRange == false)
                                {
                                    table.Rows[indexRow].Cells[nexCol].Paragraphs.First().Append(oVWRKSVendors.vendors[z].itemAsuransi[i].Rate == null ? "" : oVWRKSVendors.vendors[z].itemAsuransi[i].Rate.ToString());
                                    table.Rows[indexRow].MergeCells(nexCol, nexCol2);
                                }
                            }
                            nexCol++;
                            nexCol2++;
                            //nexCol = nexCol + 2;
                            //nexCol2 = nexCol2 + 2;
                        }

                        indexRow++;
                    }

                    System.IO.MemoryStream ms2 = new System.IO.MemoryStream();
                    DocX doc2 = DocX.Create(ms2);

                    doc2.PageLayout.Orientation = Novacode.Orientation.Landscape;
                    Paragraph p = doc2.InsertParagraph();
                    p.Append("Lampiran").Bold();
                    p.Alignment = Alignment.left;
                    p.AppendLine();
                    Table t = doc2.InsertTable(table);
                    doc.InsertSection();
                    doc.InsertDocument(doc2);
                    //doc.InsertTable(table2);

                }
                doc.SaveAs(OutFileNama);
                streamx.Close();
            }
            catch
            {
                streamx.Close();
            }
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            if (_repository.CekBukaAmplopTahapan(Id) == 1)
            {
                var stream = new FileStream(OutFileNama, FileMode.Open);
                result.Content = new StreamContent(stream);
                //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.wordprocessingml.document");

                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = outputFileName
                };
            }
            else result.Content = new StringContent("Anda Tidak Memiliki Akses");
            return result;
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public HttpResponseMessage BerkasSPKAsuransi(Guid Id)
        {
            var pengadaan = _repository.GetPengadaan(Id, UserId(), 0);
            var jadwalKlarifikasi = _repository.getPelaksanaanKlarifikasi(Id, UserId());
            var jadwalPemenang = _repository.getPelaksanaanPemenang(Id, UserId());
            string fileName = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Template\SPKAsuransi.docx";
            string outputFileName = "BA-SPK-Asuransi-" + pengadaan.NoPengadaan.Replace("/", "-") + "-" + DateTime.Now.ToString("dd-MM-yy") + ".docx";

            string OutFileNama = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Temp\" + outputFileName;

            // var streamx = new FileStream(fileName, FileMode.Open);

            // var doc = DocX.Load(streamx);

            System.IO.MemoryStream ms2 = new System.IO.MemoryStream();
            var docM = DocX.Create(ms2);

            var pemenangx = _repository.getPemenangPengadaan(Id, UserId());
            foreach (var item in pemenangx)
            {
                var streamx = new FileStream(fileName, FileMode.Open);
                var BeritaAcara = _repository.getBeritaAcaraByTipeandVendor(Id, TipeBerkas.SuratPerintahKerja, item.VendorId.Value, UserId());

                try
                {
                    var doc = DocX.Load(streamx);
                    doc.ReplaceText("{pengadaan_name}", pengadaan.Judul == null ? "" : pengadaan.Judul);
                    doc.ReplaceText("{pengadaan_name_judul}", pengadaan.Judul == null ? "" : pengadaan.Judul.ToUpper());
                    doc.ReplaceText("{nomor_berita_acara}", BeritaAcara == null ? "" : BeritaAcara.NoBeritaAcara);
                    doc.ReplaceText("{pengadaan_unit_pemohon}", pengadaan.UnitKerjaPemohon == null ? "" : pengadaan.UnitKerjaPemohon);
                    doc.ReplaceText("{tempat_tanggal}", "...............," + BeritaAcara == null ? "................" :
                            (BeritaAcara.tanggal.Value.Day + " " + Common.ConvertNamaBulan(BeritaAcara.tanggal.Value.Month) + " " +
                            BeritaAcara.tanggal.Value.Year));

                    doc.ReplaceText("{pengadaan_jadwal_hari}", BeritaAcara.tanggal == null ? "" :
                           Common.ConvertHari(BeritaAcara.tanggal.Value.Day));
                    doc.ReplaceText("{pengadaan_jadwal_tanggal}", BeritaAcara.tanggal.Value.Day + " " + Common.ConvertNamaBulan(BeritaAcara.tanggal.Value.Month) +
                          " " + BeritaAcara.tanggal.Value.Year);


                    // var pemenang = _repository.getPemenangPengadaan(Id, UserId());
                    var vendor = _repository.GetVendorById(item.VendorId.Value);
                    doc.ReplaceText("{kandidat_pemenang}", item.NamaVendor);
                    doc.ReplaceText("{total_pengadaan}", item.total == null ? "" : item.total.Value.ToString("C", MyConverter.formatCurrencyIndo()));
                    doc.ReplaceText("{alamat}", vendor.Alamat.ToString());
                    doc.ReplaceText("{terbilang}", item.total == null ? "" : MyConverter.Terbilang(item.total.Value.ToString()) + " Rupiah");
                    // doc.SaveAs(OutFileNama);
                    //docM.InsertSection();

                    docM.InsertDocument(doc); //doc.SaveAs(OutFileNama);
                    streamx.Close();
                    // streamx.Close();
                }
                catch
                {
                    streamx.Close();
                }
            }
            docM.SaveAs(OutFileNama);
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new FileStream(OutFileNama, FileMode.Open);
            result.Content = new StreamContent(stream);
            //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.wordprocessingml.document");

            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = outputFileName
            };

            return result;
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_approver)]
        public HttpResponseMessage ReportPengadaan2(string dari, string sampai)
        {
            try
            {
                LocalReport lr = new LocalReport();
                string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Report\BukaAmplop.rdlc";
                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //return View("Index");
                }
                var oDari = Common.ConvertDate(dari, "dd/MM/yyyy");
                var oSampai = Common.ConvertDate(sampai, "dd/MM/yyyy");

                var oKandidatPengadaan = _repository.GetRepotPengadan2(oDari, oSampai, UserId());

                ReportDataSource rd = new ReportDataSource("DataSet2", oKandidatPengadaan);
                lr.DataSources.Add(rd);
                string param1 = "";
                string filename = "";
                string param2 = "";
                string paramSemester = "";
                string paramTahunAjaran = "";


                string reportType = "doc";
                string mimeType;
                string encoding;
                string fileNameExtension;


                string[] streamids = null;
                String extension = null;
                Byte[] bytes = null;
                Warning[] warnings;

                bytes = lr.Render("Excel", null, out mimeType, out encoding, out extension, out streamids, out warnings);

                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                Stream stream = new MemoryStream(bytes);

                result.Content = new StreamContent(stream);

                //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");

                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = "Report-Tender" + UserId() + DateTime.Now.ToString("dd-MM-yy") + ".xls"
                };

                return result;
            }
            catch (ReflectionTypeLoadException ex)
            {
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                StringBuilder sb = new StringBuilder();
                foreach (Exception exSub in ex.LoaderExceptions)
                {
                    sb.AppendLine(exSub.Message);
                    FileNotFoundException exFileNotFound = exSub as FileNotFoundException;
                    if (exFileNotFound != null)
                    {
                        if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                        {
                            sb.AppendLine("Fusion Log:");
                            sb.AppendLine(exFileNotFound.FusionLog);
                        }
                    }
                    sb.AppendLine();
                }
                result.Content = new StringContent(sb.ToString());

                return result;
                //Display or log the error based on your application.
            }
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance, IdLdapConstants.Roles.pRole_approver)]
        public HttpResponseMessage ReportPengadaan3(string dari, string sampai, EStatusPengadaan status)
        {
            try
            {
                LocalReport lr = new LocalReport();
                string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Report\BukaAmplop.rdlc";
                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //return View("Index");
                }
                var oDari = Common.ConvertDate(dari, "dd/MM/yyyy");
                var oSampai = Common.ConvertDate(sampai, "dd/MM/yyyy");
                var oStatus = status;

                var oKandidatPengadaan = _repository.GetRepotPengadan3(oDari, oSampai, UserId(), oStatus);

                ReportDataSource rd = new ReportDataSource("DataSet2", oKandidatPengadaan);
                lr.DataSources.Add(rd);
                string param1 = "";
                string filename = "";
                string param2 = "";
                string paramSemester = "";
                string paramTahunAjaran = "";


                string reportType = "doc";
                string mimeType;
                string encoding;
                string fileNameExtension;


                string[] streamids = null;
                String extension = null;
                Byte[] bytes = null;
                Warning[] warnings;

                bytes = lr.Render("Excel", null, out mimeType, out encoding, out extension, out streamids, out warnings);

                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                Stream stream = new MemoryStream(bytes);

                result.Content = new StreamContent(stream);

                //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");

                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = "Report-Tender" + UserId() + DateTime.Now.ToString("dd-MM-yy") + ".xls"
                };

                return result;
            }
            catch (ReflectionTypeLoadException ex)
            {
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                StringBuilder sb = new StringBuilder();
                foreach (Exception exSub in ex.LoaderExceptions)
                {
                    sb.AppendLine(exSub.Message);
                    FileNotFoundException exFileNotFound = exSub as FileNotFoundException;
                    if (exFileNotFound != null)
                    {
                        if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                        {
                            sb.AppendLine("Fusion Log:");
                            sb.AppendLine(exFileNotFound.FusionLog);
                        }
                    }
                    sb.AppendLine();
                }
                result.Content = new StringContent(sb.ToString());

                return result;
                //Display or log the error based on your application.
            }
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public HttpResponseMessage ReportPengadaan4(string dari, string sampai, EStatusPengadaan status, string divisi)
        {
            try
            {
                LocalReport lr = new LocalReport();
                string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Report\BukaAmplop.rdlc";
                if (System.IO.File.Exists(path))
                {
                    lr.ReportPath = path;
                }
                else
                {
                    //return View("Index");
                }
                var oDari = Common.ConvertDate(dari, "dd/MM/yyyy");
                var oSampai = Common.ConvertDate(sampai, "dd/MM/yyyy");
                var oStatus = status;
                var oDivisi = divisi;

                var oKandidatPengadaan = _repository.GetRepotPengadan4(oDari, oSampai, UserId(), oStatus, oDivisi);

                ReportDataSource rd = new ReportDataSource("DataSet2", oKandidatPengadaan);
                lr.DataSources.Add(rd);
                string param1 = "";
                string filename = "";
                string param2 = "";
                string paramSemester = "";
                string paramTahunAjaran = "";


                string reportType = "doc";
                string mimeType;
                string encoding;
                string fileNameExtension;


                string[] streamids = null;
                String extension = null;
                Byte[] bytes = null;
                Warning[] warnings;

                bytes = lr.Render("Excel", null, out mimeType, out encoding, out extension, out streamids, out warnings);

                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                Stream stream = new MemoryStream(bytes);

                result.Content = new StreamContent(stream);

                //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");

                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = "Report-Tender" + UserId() + DateTime.Now.ToString("dd-MM-yy") + ".xls"
                };

                return result;
            }
            catch (ReflectionTypeLoadException ex)
            {
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                StringBuilder sb = new StringBuilder();
                foreach (Exception exSub in ex.LoaderExceptions)
                {
                    sb.AppendLine(exSub.Message);
                    FileNotFoundException exFileNotFound = exSub as FileNotFoundException;
                    if (exFileNotFound != null)
                    {
                        if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                        {
                            sb.AppendLine("Fusion Log:");
                            sb.AppendLine(exFileNotFound.FusionLog);
                        }
                    }
                    sb.AppendLine();
                }
                result.Content = new StringContent(sb.ToString());

                return result;
                //Display or log the error based on your application.
            }
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public HttpResponseMessage BerkasSuratMenang(Guid Id, string Surat)
        {
            var pengadaan = _repository.GetPengadaan(Id, UserId(), 0);
            var jadwalKlarifikasi = _repository.getPelaksanaanKlarifikasi(Id, UserId());
            var jadwalPemenang = _repository.getPelaksanaanPemenang(Id, UserId());
            string fileName = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Template\SuratMenang.docx";
            string outputFileName = "Surat-Menang-" + pengadaan.NoPengadaan.Replace("/", "-") + "-" + DateTime.Now.ToString("dd-MM-yy") + ".docx";

            string OutFileNama = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Temp\" + outputFileName;

            // var streamx = new FileStream(fileName, FileMode.Open);

            // var doc = DocX.Load(streamx);

            System.IO.MemoryStream ms2 = new System.IO.MemoryStream();
            var docM = DocX.Create(ms2);

            var oPemenang = _repository.getPemenangPengadaan(Id, UserId());
            //var oKandidat = _repository.getKandidatPengadaan(Id, UserId());
            //var oCekdlu = oKandidat;

            //for (var i = 0; i < oPemenang.Count(); i++)
            //{
            //    var pemenang = oPemenang[i].VendorId;

            //    for (var a = 0; a < oKandidat.Count(); a++)
            //    {
            //        if (pemenang == oKandidat[a].VendorId)
            //        {
            //            oCekdlu.Remove(oCekdlu.Single(r => r.VendorId == pemenang));
            //        }
            //    }
            //}

            //var oKalah = oCekdlu;

            foreach (var item in oPemenang)
            {
                var streamx = new FileStream(fileName, FileMode.Open);
                //var BeritaAcara = _repository.getBeritaAcaraByTipeandVendor(Id, TipeBerkas.SuratPerintahKerja, item.VendorId.Value, UserId());

                try
                {
                    var doc = DocX.Load(streamx);
                    var noMenang = _repository.GenerateNoDOKUMEN(UserId(), System.Configuration.ConfigurationManager.AppSettings["KODE_MENANG"].ToString(), TipeNoDokumen.MEANANG);
                    doc.ReplaceText("{pengadaan_name}", pengadaan.Judul == null ? "" : pengadaan.Judul);
                    doc.ReplaceText("{pengadaan_name_judul}", pengadaan.Judul == null ? "" : pengadaan.Judul.ToUpper());
                    doc.ReplaceText("{nomor_menang}", noMenang == null ? "" : noMenang);
                    doc.ReplaceText("{body}", Surat == null ? "" : Surat);
                    //doc.ReplaceText("{tempat_tanggal}", "...............," + BeritaAcara == null ? "................" :
                    //        (BeritaAcara.tanggal.Value.Day + " " + Common.ConvertNamaBulan(BeritaAcara.tanggal.Value.Month) + " " +
                    //        BeritaAcara.tanggal.Value.Year));

                    //if (BeritaAcara == null)
                    //{
                    //    doc.ReplaceText("{tempat_tanggal}", "................");
                    //    doc.ReplaceText("{pengadaan_jadwal_hari}", "");
                    //    doc.ReplaceText("{pengadaan_jadwal_tanggal}", "");
                    //}
                    //else
                    //{
                    //    doc.ReplaceText("{tempat_tanggal}", (BeritaAcara.tanggal.Value.Day + " " + Common.ConvertNamaBulan(BeritaAcara.tanggal.Value.Month) + " " +
                    //        BeritaAcara.tanggal.Value.Year));
                    //    doc.ReplaceText("{pengadaan_jadwal_hari}", Common.ConvertHari(BeritaAcara.tanggal.Value.Day));
                    //    doc.ReplaceText("{pengadaan_jadwal_tanggal}", BeritaAcara.tanggal.Value.Day + " " + Common.ConvertNamaBulan(BeritaAcara.tanggal.Value.Month) +
                    //      " " + BeritaAcara.tanggal.Value.Year);
                    //}

                    //doc.ReplaceText("{pengadaan_jadwal_hari}", BeritaAcara.tanggal == null ? "" :
                    //       Common.ConvertHari(BeritaAcara.tanggal.Value.Day));
                    //doc.ReplaceText("{pengadaan_jadwal_tanggal}", BeritaAcara.tanggal.Value.Day + " " + Common.ConvertNamaBulan(BeritaAcara.tanggal.Value.Month) +
                    //      " " + BeritaAcara.tanggal.Value.Year);


                    // var pemenang = _repository.getPemenangPengadaan(Id, UserId());
                    var vendor = _repository.GetVendorById(item.VendorId.Value);
                    doc.ReplaceText("{kandidat_menang}", item.NamaVendor);
                    doc.ReplaceText("{total_pengadaan}", item.total == null ? "" : item.total.Value.ToString("C", MyConverter.formatCurrencyIndo()));
                    doc.ReplaceText("{alamat}", vendor.Alamat.ToString());
                    doc.ReplaceText("{tempat_tanggal}", DateTime.Now.ToString("dd MMMM yyyy"));
                    // doc.SaveAs(OutFileNama);
                    //docM.InsertSection();

                    docM.InsertDocument(doc); //doc.SaveAs(OutFileNama);
                    streamx.Close();
                    // streamx.Close();
                }
                catch
                {
                    streamx.Close();
                }
            }
            docM.SaveAs(OutFileNama);
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new FileStream(OutFileNama, FileMode.Open);
            result.Content = new StreamContent(stream);
            //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.wordprocessingml.document");

            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = outputFileName
            };

            return result;
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public HttpResponseMessage BerkasSuratKalah(Guid Id, string Surat)
        {
            var pengadaan = _repository.GetPengadaan(Id, UserId(), 0);
            var jadwalKlarifikasi = _repository.getPelaksanaanKlarifikasi(Id, UserId());
            var jadwalPemenang = _repository.getPelaksanaanPemenang(Id, UserId());
            string fileName = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Template\SuratKalah.docx";
            string outputFileName = "Surat-Kalah-" + pengadaan.NoPengadaan.Replace("/", "-") + "-" + DateTime.Now.ToString("dd-MM-yy") + ".docx";

            string OutFileNama = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Temp\" + outputFileName;

            // var streamx = new FileStream(fileName, FileMode.Open);

            // var doc = DocX.Load(streamx);

            System.IO.MemoryStream ms2 = new System.IO.MemoryStream();
            var docM = DocX.Create(ms2);

            var oPemenang = _repository.getPemenangPengadaan(Id, UserId());
            var oKandidat = _repository.getKandidatPengadaan(Id, UserId());
            var oCekdlu = oKandidat;

            for (var i = 0; i < oPemenang.Count(); i++)
            {
                var pemenang = oPemenang[i].VendorId;

                for (var a = 0; a < oKandidat.Count(); a++)
                {
                    if (pemenang == oKandidat[a].VendorId)
                    {
                        oCekdlu.Remove(oCekdlu.Single(r => r.VendorId == pemenang));
                    }
                }
            }

            var oKalah = oCekdlu;

            foreach (var item in oKalah)
            {
                var streamx = new FileStream(fileName, FileMode.Open);
                //var BeritaAcara = _repository.getBeritaAcaraByTipeandVendor(Id, TipeBerkas.SuratPerintahKerja, item.VendorId.Value, UserId());

                try
                {
                    var doc = DocX.Load(streamx);
                    var noKalah = _repository.GenerateNoDOKUMEN(UserId(), System.Configuration.ConfigurationManager.AppSettings["KODE_KALAH"].ToString(), TipeNoDokumen.KALAH);
                    doc.ReplaceText("{pengadaan_name}", pengadaan.Judul == null ? "" : pengadaan.Judul);
                    doc.ReplaceText("{pengadaan_name_judul}", pengadaan.Judul == null ? "" : pengadaan.Judul.ToUpper());
                    doc.ReplaceText("{nomor_kalah}", noKalah == null ? "" : noKalah);
                    doc.ReplaceText("{body}", Surat == null ? "" : Surat);
                    //doc.ReplaceText("{tempat_tanggal}", "...............," + BeritaAcara == null ? "................" :
                    //        (BeritaAcara.tanggal.Value.Day + " " + Common.ConvertNamaBulan(BeritaAcara.tanggal.Value.Month) + " " +
                    //        BeritaAcara.tanggal.Value.Year));

                    //if (BeritaAcara == null)
                    //{
                    //    doc.ReplaceText("{tempat_tanggal}", "................");
                    //    doc.ReplaceText("{pengadaan_jadwal_hari}", "");
                    //    doc.ReplaceText("{pengadaan_jadwal_tanggal}", "");
                    //}
                    //else
                    //{
                    //    doc.ReplaceText("{tempat_tanggal}", (BeritaAcara.tanggal.Value.Day + " " + Common.ConvertNamaBulan(BeritaAcara.tanggal.Value.Month) + " " +
                    //        BeritaAcara.tanggal.Value.Year));
                    //    doc.ReplaceText("{pengadaan_jadwal_hari}", Common.ConvertHari(BeritaAcara.tanggal.Value.Day));
                    //    doc.ReplaceText("{pengadaan_jadwal_tanggal}", BeritaAcara.tanggal.Value.Day + " " + Common.ConvertNamaBulan(BeritaAcara.tanggal.Value.Month) +
                    //      " " + BeritaAcara.tanggal.Value.Year);
                    //}

                    //doc.ReplaceText("{pengadaan_jadwal_hari}", BeritaAcara.tanggal == null ? "" :
                    //       Common.ConvertHari(BeritaAcara.tanggal.Value.Day));
                    //doc.ReplaceText("{pengadaan_jadwal_tanggal}", BeritaAcara.tanggal.Value.Day + " " + Common.ConvertNamaBulan(BeritaAcara.tanggal.Value.Month) +
                    //      " " + BeritaAcara.tanggal.Value.Year);


                    // var pemenang = _repository.getPemenangPengadaan(Id, UserId());
                    var vendor = _repository.GetVendorById(item.VendorId.Value);
                    doc.ReplaceText("{kandidat_kalah}", item.NamaVendor);
                    doc.ReplaceText("{total_pengadaan}", item.total == null ? "" : item.total.Value.ToString("C", MyConverter.formatCurrencyIndo()));
                    doc.ReplaceText("{alamat}", vendor.Alamat.ToString());
                    doc.ReplaceText("{tempat_tanggal}", DateTime.Now.ToString("dd MMMM yyyy"));
                    // doc.SaveAs(OutFileNama);
                    //docM.InsertSection();

                    docM.InsertDocument(doc); //doc.SaveAs(OutFileNama);
                    streamx.Close();
                    // streamx.Close();
                }
                catch
                {
                    streamx.Close();
                }
            }
            docM.SaveAs(OutFileNama);
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new FileStream(OutFileNama, FileMode.Open);
            result.Content = new StreamContent(stream);
            //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.wordprocessingml.document");

            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = outputFileName
            };

            return result;
        }

        [ApiAuthorize(IdLdapConstants.Roles.pRole_procurement_head,
                                            IdLdapConstants.Roles.pRole_procurement_staff, IdLdapConstants.Roles.pRole_procurement_end_user,
                                             IdLdapConstants.Roles.pRole_procurement_manager, IdLdapConstants.Roles.pRole_compliance)]
        [System.Web.Http.AcceptVerbs("GET", "POST", "HEAD")]
        public HttpResponseMessage BerkasSPKNonPKS(string Judul, string Tanggal_SPK, string Nilai_SPK, string Vendor, string No_SPK)
        {
            //var pengadaan = _repository.GetPengadaan(Id, UserId(), 0);
            //var jadwalKlarifikasi = _repository.getPelaksanaanKlarifikasi(Id, UserId());
            //var jadwalPemenang = _repository.getPelaksanaanPemenang(Id, UserId());
            string fileName = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Template\SPK-NonPKS.docx";
            string outputFileName = "BA-SPK-" + Judul + "-" + DateTime.Now.ToString("dd-MM-yy") + ".docx";

            string OutFileNama = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Download\Report\Temp\" + outputFileName;

            // var streamx = new FileStream(fileName, FileMode.Open);

            // var doc = DocX.Load(streamx);

            System.IO.MemoryStream ms2 = new System.IO.MemoryStream();
            var docM = DocX.Create(ms2);

            //var pemenangx = _repository.getPemenangPengadaan(Id, UserId());
            //foreach (var item in pemenangx)
            //{
            var streamx = new FileStream(fileName, FileMode.Open);
            //var BeritaAcara = _repository.getBeritaAcaraByTipeandVendor(Id, TipeBerkas.SuratPerintahKerja, item.VendorId.Value, UserId());

            try
            {
                var doc = DocX.Load(streamx);
                doc.ReplaceText("{nomor_spk_non_pks}", No_SPK == null ? "" : No_SPK);
                doc.ReplaceText("{judul_spk}", Judul == null ? "" : Judul);
                //doc.ReplaceText("{pengadaan_name_judul}", pengadaan.Judul == null ? "" : pengadaan.Judul.ToUpper());
                //doc.ReplaceText("{nomor_berita_acara}", BeritaAcara == null ? "" : BeritaAcara.NoBeritaAcara);
                //doc.ReplaceText("{pengadaan_unit_pemohon}", pengadaan.UnitKerjaPemohon == null ? "" : pengadaan.UnitKerjaPemohon);
                doc.ReplaceText("{tanggal_spk}", "...............," + Tanggal_SPK == null ? "................" : Tanggal_SPK);

                //doc.ReplaceText("{pengadaan_jadwal_hari}", BeritaAcara.tanggal == null ? "" :
                //       Common.ConvertHari(BeritaAcara.tanggal.Value.Day));
                //doc.ReplaceText("{pengadaan_jadwal_tanggal}", BeritaAcara.tanggal.Value.Day + " " + Common.ConvertNamaBulan(BeritaAcara.tanggal.Value.Month) +
                //      " " + BeritaAcara.tanggal.Value.Year);


                //var pemenang = _repository.getPemenangPengadaan(Vendor);
                var vendor = _repository.GetVendorByName(Vendor.ToString());
                doc.ReplaceText("{vendor}", Vendor == null ? "" : Vendor);
                doc.ReplaceText("{nilai_spk}", Nilai_SPK == null ? "" : Nilai_SPK);
                doc.ReplaceText("{alamat}", vendor == null ? "" : vendor.Alamat.ToString());
                doc.ReplaceText("{terbilang}", Nilai_SPK == null ? "" : MyConverter.Terbilang(Nilai_SPK.ToString()) + " Rupiah");
                // doc.SaveAs(OutFileNama);
                docM.InsertSection();

                docM.InsertDocument(doc); //doc.SaveAs(OutFileNama);
                streamx.Close();
                // streamx.Close();
            }
            catch
            {
                streamx.Close();
            }
            //}
            docM.SaveAs(OutFileNama);
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new FileStream(OutFileNama, FileMode.Open);
            result.Content = new StreamContent(stream);
            //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.wordprocessingml.document");

            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = outputFileName
            };

            return result;
        }
    }
}



// Append some text.
//p.Append("Hello World").Font(new FontFamily("Arial")).Bold();

//p = p.InsertParagraphAfterSelf(String.Empty);
//p.Alignment = Alignment.center;
//p.Font(new FontFamily("Arial"));
//p.FontSize(12);

//p.Append("Some text").Bold();

//p.Append("I am ").Append("bold").Bold()
//.Append(" and I am ")
//.Append("italic").Italic().Append(".")
//.AppendLine("I am ")
//.Append("Arial Black")
//.Font(new FontFamily("Arial Black"))
//.Append(" and I am not.")
//.AppendLine("I am ")
//.Append("BLUE").Color(Color.Blue)
//.Append(" and I am")
//.Append("Red").Color(Color.Red).Append(".");