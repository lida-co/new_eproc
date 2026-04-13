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
using Reston.Pinata.Model.Helper;
using System.Net;
using Reston.Eproc.Model.Monitoring.Model;
using Reston.Eproc.Model.Monitoring.Entities;
using Reston.Pinata.Model.PengadaanRepository;
using Reston.Pinata.Model.Asuransi;
using Reston.Eproc.Model.Ext;

namespace Reston.Pinata.Model.PengadaanRepository
{
    public interface IPengadaanRepo
    {
        List<ViewPertanyaan> GetDataPertanyaan();
        List<VWTenderScoringHeader> GetHeaderPertanyaan(Guid Id);
        VWReportAssessment ReportAssessment(Guid Id);
        List<VWTenderScoringDetails> UrutanPertanyaan(Guid Id);
        List<VWTenderScoring> cekmasterdata(Guid Id);
        List<VWReferenceData> GetCountPertanyaan(Guid Id);
        List<VWTenderScoring> ScorePenilaian(Guid Id);
        ViewPengadaan GetPengadaan(Guid Id);
        VWTenderScoring CekPertanyaan(Guid Id);
        List<VWTenderScoring> GetDataAssessment(Guid Id, int VendorId, Guid UserId);
        List<VWTenderScoring> GetPointPenilaian(Guid Id);
        List<VWVendor> GetAssessment(Guid Id);
        List<VWTenderScoring> GetValueAssessment(Guid Id, Guid UserIdAssessment, int VendorId);
        List<VWPersonilPengadaan> GetDropDownPenilai(Guid Id);
        List<VWTenderScoring> GetQuestion(Guid Id, int VendorId);
        List<VWPemenangPengadaan> PemenangPengadaan(Guid Id);
        ViewPengadaan GetPengadaan(Guid id, Guid UserID, int approver);
        ViewPengadaan PersetujuanTahapan(Guid id, Guid UserID);
        Pengadaan GetPengadaanByiD(Guid id);

        DataPagePengadaan GetPengadaans(string search, int start, int limit, Guid? UserId, List<string> Roles, EGroupPengadaan groupstatus, List<Guid> lstMenejer, List<Guid> listHead);
        List<ViewPengadaan> GetPerhatianWorkflow(string search, int start, int limit, Guid? UserId);
        List<Pengadaan> GetAllPengadaan();

        Pengadaan AddPengadaan(Pengadaan pengadaan, Guid UserId, List<Guid> manager);
        //
        List<VWRKSDetail> getRKS(Guid id);
        List<VWRKSDetailRekanan> getRKSForRekanan(Guid id, Guid UserId);
        RKSHeader saveRks(RKSHeader rks, Guid UserId);
        VWRKSHeaderPengadaan GetRKSHeaderPengadaan(Guid id);
        //
        void Save();
        Pengadaan Persetujuan(Guid? UserId, Guid id, int approver);
        Pengadaan Penolakan(Guid? UserId, VWPenolakan vwPenolakan, int approver);
        List<ViewPengadaan> GetPerhatian(string search, int start, int limit, Guid? UserId, List<string> Roles, int approver, List<Guid> lstManager);

        DokumenPengadaan GetDokumenPengadaan(Guid Id);
        DokumenPengadaan GetDokumenPengadaanSpk(Guid PengadaanId, int VendorId);
        DokumenPengadaan saveDokumenPengadaan(DokumenPengadaan dokumenPengadaan, Guid UserId);
        List<VWDokumenPengadaan> GetListDokumenPengadaan(TipeBerkas tipe, Guid Id, Guid UserId);
        int deleteDokumen(Guid Id);
        KandidatPengadaan saveKandidatPengadaan(KandidatPengadaan kandidat, Guid UserId);
        int deleteKandidatPengadaan(Guid Id, Guid UserId);
        int deleteJadwalPengadaan(Guid Id, Guid UserId);
        JadwalPengadaan saveJadwalPengadaan(JadwalPengadaan Jadwal, Guid UserId);
        PersonilPengadaan savePersonilPengadaan(PersonilPengadaan Personil, Guid UserId);
        int deletePersonilPengadaan(Guid Id, Guid UserId);
        List<VWKandidatPengadaan> getListKandidatPengadaan(Guid PengadaanId);
        List<JadwalPengadaan> getListJadwalPengadaan(Guid PengadaanId);
        List<PersonilPengadaan> getListPersonilPengadaan(Guid PengadaanId);
        int DeletePengadaan(Guid Id, Guid UserId);
        int arsipkan(Guid Id, Guid UserId);

        JadwalPelaksanaan getPelaksanaanPendaftaran(Guid PengadaanId);

        JadwalPelaksanaan getPelaksanaanAanwijing(Guid PengadaanId);
        JadwalPelaksanaan addPelaksanaanAanwijing(JadwalPelaksanaan pelaksanaanAanwijzing, Guid UserId);
        List<VWKehadiranKandidatAanwijzing> getKehadiranAanwijzings(Guid PengadaanId);
        KehadiranKandidatAanwijzing addKehadiranAanwijzing(Guid Id, Guid UserId);
        int DeleteKehadiranAanwijzing(Guid Id, Guid UserId);
        JadwalPelaksanaan AddPelaksanaanSubmitPenawaran(JadwalPelaksanaan pelaksanaanSubmitPenawaran, Guid UserId);
        JadwalPelaksanaan getPelaksanaanSubmitPenawaran(Guid PengadaanId);
        JadwalPelaksanaan AddPelaksanaanBukaAmplop(JadwalPelaksanaan pelaksanaanBukaAmplop, Guid UserId);
        JadwalPelaksanaan getPelaksanaanBukaAmplop(Guid PengadaanId);
        PersetujuanBukaAmplop AddPersetujuanBukaAmplop(Guid PengadaanId, Guid UserId);
        List<VWPErsetujuanBukaAmplop> getPersetujuanBukaAmplop(Guid PengadaanId, Guid UserId);
        int deleteDokumenPelaksanaan(Guid Id, Guid UserId, int isApprovel);
        int deleteDokumenRekanan(Guid Id, Guid UserId);
        int UpdateStatus(Guid Id, EStatusPengadaan status);
        int cekStateDiSetujui(Guid PengadaanId);
        int AjukanPengadaan(Guid Id, Guid UserId, List<Guid> manager);
        int cekStateAanwijzing(Guid PengadaanId);
        int cekStateSubmitPenawaran(Guid PengadaanId);
        int cekStateBukaAmplop(Guid PengadaanId);

        int nextToState(Guid PengadaanId, Guid UserId, EStatusPengadaan state);

        KualifikasiKandidat addKualifikasiKandidat(KualifikasiKandidat dKualifikasiKandidat, Guid UserId);
        int deleteKualifikasiKandidat(Guid Id, Guid UserId);
        List<ViewPengadaan> GetPengadaansForRekanan(int start, int limit, Guid? UserId, List<string> Roles, EGroupPengadaan groupstatus);
        ViewPengadaan GetPengadaanForRekanan(Guid id, Guid UserId);
        List<VWRKSDetailRekanan> addHargaRekanan(List<VWRKSDetailRekanan> dlstHargaRekanan, Guid PengadaanId, Guid UserId);
        List<VWRekananSubmitHarga> getListRekananSubmit(Guid PengadaanId, Guid UserId);
        List<VWRekananPenilaian> getListRekananPenilaian(Guid PengadaanId, Guid UserId);
        List<VWRekananPenilaian> getListRekananPenilaian2(Guid PengadaanId, Guid UserId);
        JadwalPelaksanaan AddPelaksanaanPenilaian(JadwalPelaksanaan pelaksanaanPenilaian, Guid UserId);
        JadwalPelaksanaan getPelaksanaanPenilaian(Guid PengadaanId, Guid UserId);
        VWRKSVendors getRKSPenilaian(Guid PengadaanId, Guid UserId);
        VWRKSVendors getRKSPenilaian2(Guid PengadaanId, Guid UserId);
        PelaksanaanPemilihanKandidat addKandidatPilihan(PelaksanaanPemilihanKandidat oPelaksanaanPemilihanKandidat, Guid UserId);
        int deleteKandidatPilihan(PelaksanaanPemilihanKandidat oPelaksanaanPemilihanKandidat, Guid UserId);
        JadwalPelaksanaan AddPelaksanaanKlarifikasi(JadwalPelaksanaan pelaksanaanKlarifikasi, Guid UserId);
        JadwalPelaksanaan getPelaksanaanKlarifikasi(Guid PengadaanId, Guid UserId);
        JadwalPelaksanaan getPelaksanaanKlarifikasiLanjutan(Guid PengadaanId, Guid UserId);
        JadwalPelaksanaan AddPelaksanaanPemenang(JadwalPelaksanaan pelaksanaanPemenang, Guid UserId);
        JadwalPelaksanaan getPelaksanaanPemenang(Guid PengadaanId, Guid UserId);

        List<VWRKSDetailRekanan> addHargaKlarifikasiRekanan(List<VWRKSDetailRekanan> dlstHargaKlarifikasiRekanan, Guid PengadaanId, Guid UserId);
        List<VWRKSDetailRekanan> addHargaKlarifikasiLanjutanRekanan(List<VWRKSDetailRekanan> dlstHargaKlarifikasiRekanan, Guid PengadaanId, Guid UserId);
        List<VWRKSDetailRekanan> getRKSForKlarifikasiRekanan(Guid id, Guid UserId);
        List<VWRKSDetailRekanan> getRKSForKlarifikasiLanjutanRekanan(Guid id, Guid UserId);
        VWRKSVendors getRKSKlarifikasi(Guid PengadaanId, Guid UserId);
        VWRKSVendors getRKSKlarifikasiLanjutan(Guid PengadaanId, Guid UserId);
        List<VWRekananPenilaian> getListRekananKlarifikasiPenilaian(Guid PengadaanId, Guid UserId);
        List<VWRekananPenilaian> getListRekananKlarifikasiPenilaianLanjutan(Guid PengadaanId, Guid UserId);
        List<VWRekananSubmitHarga> getListRekananKlarifikasiSubmit(Guid PengadaanId, Guid UserId);
        List<VWRekananSubmitHarga> getListRekananKlarifikasiLanjutSubmit(Guid PengadaanId, Guid UserId);
        VWRKSVendors getRKSKlarifikasiPenilaianVendor(Guid PengadaanId, Guid UserId, int VendorId);
        VWRKSVendors getRKSKlarifikasiPenilaianVendor2(Guid PengadaanId, Guid UserId, int VendorId);

        List<vwProduk> GetAllProduk(string term);
        List<vwProduk> GetItemByRegion(string term, string region);
        List<vwProduk> GetAllSatuan(string term);
        dataVendors GetVendors(ETipeVendor tipe, int start, string filter, EStatusVendor status, int limit);
        RKSHeader AddTotalHps(Guid PengadaanId, decimal Total, Guid UserId);
        RKSHeader GetTotalHps(Guid PengadaanId, Guid UserId);
        BudgetingPengadaanHeader GetTotalCOA(Guid PengadaanId, Guid UserId);
        VWBudgeting GetLoadCOA(Guid PengadaanId, Guid UserId);
        List<VWVendor> GetVendorsByPengadaanId(Guid PengadaanId);
        VWRKSVendors getRKSKlarifikasiPenilaian(Guid PengadaanId, Guid UserId);
        VWRKSVendors getRKSKlarifikasiLanjutanPenilaian(Guid PengadaanId, Guid UserId);
        List<VWRKSDetail> getRKSDetails(Guid PengadaanId, Guid UserId);
        ResultMessage AddPertanyaan(VWTenderScoringHeader vwtenderscoringheader);
        ResultMessage AddPenilaian(VWTenderScoringDetails vwtenderscoringdetail);
        List<VWDokumenPengadaan> GetListDokumenVendor(TipeBerkas tipe, Guid Id, Guid UserId, int VendorId);
        List<ViewBenefitRate> getRKSAsuransiDetails(Guid PengadaanId, Guid UserId);
        //ResultMessage AddAssessment(VWTenderScoringDetails vwtenderscoringuser, Guid Id, int VendorId, int Total, decimal Average, Guid UserId);                List<VWPembobotanPengadaan> getKriteriaPembobotan(Guid PengadaanId);
        ResultMessage AddAssessment(VWTenderScoringDetails vwtenderscoringuser, Guid Id, int VendorId, decimal Total, decimal Average, Guid UserId);
        List<VWPembobotanPengadaan> getKriteriaPembobotan(Guid PengadaanId);
        int addPembobtanPengadaan(PembobotanPengadaan dataPembobotanPengadaan, Guid UserId);
        List<PembobotanPengadaan> getPembobtanPengadaan(Guid PengadaanId, Guid UserId);
        List<VWPembobotanPengadaanVendor> getPembobtanPengadaanVendor(Guid PengadaanId, int VendorId, Guid UserId);
        int addLstPembobtanPengadaan(List<PembobotanPengadaan> dataLstPembobotanPengadaan, Guid UserId);
        int addLstPenilaianKriteriaVendor(List<PembobotanPengadaanVendor> dataLstPenilaianKriteriaVendor, Guid UserId);
        string statusVendor(Guid PengadaanId, Guid UserId);

        List<VWRekananPenilaian> getListPenilaianByVendor(Guid PengadaanId, Guid UserId, int VendorId);
        VWRKSVendors getRKSPenilaian2Report(Guid PengadaanId, Guid UserId);
        int addPemenangPengadaan(PemenangPengadaan dtPemenangPengadaan, Guid UserId);
        Reston.Helper.Util.ResultMessage DeletePemenang(PemenangPengadaan dtPemenangPengadaan, Guid UserId);
        List<VWRekananPenilaian> getPemenangPengadaan(Guid PengadaanId, Guid UserId);
        List<VWRekananPenilaian> getKandidatPengadaan(Guid PengadaanId, Guid UserId);
        List<KandidatPengadaan> getKandidatTidakHadir(Guid PengadaanId, Guid UserId);
        List<KandidatPengadaan> getKandidatHadir(Guid PengadaanId, Guid UserId);
        List<KandidatPengadaan> getKandidatKirim(Guid PengadaanId, Guid UserId);
        List<KandidatPengadaan> getKandidatTidakKirim(Guid PengadaanId, Guid UserId);
        //List<KandidatPengadaan> getKandidatTidakMengirimPenawaran(Guid PengadaanId, Guid UserId);
        List<VWRekananPenilaian> getKandidatPengadaan2(Guid PengadaanId, Guid UserId);
        List<VWVendor> GetVendorsKlarifikasiByPengadaanId(Guid PengadaanId);
        List<VWVendor> GetVendorsKlarifikasiByPengadaanId2(Guid PengadaanId);

        BeritaAcara addBeritaAcara(BeritaAcara newBeritaAcara, Guid UserId);
        int DeleteBeritaAcara(Guid Id, Guid UserId);
        List<VWBeritaAcaraEnd> getBeritaAcara(Guid PengadaanId, Guid UserId);
        BeritaAcara getBeritaAcaraByTipe(Guid PengadaanId, TipeBerkas tipe, Guid UserId);
        int CekBukaAmplop(Guid PengadaanId);
        ViewVendors GetVendorById(int VendorId);
        ViewVendors GetVendorByName(string NamaVendor);
        List<VWReportPengadaan> GetRepotPengadan(DateTime? dari, DateTime? sampai, Guid UserId, string divisi);
        List<VWReportPengadaan> GetRepotPengadan2(DateTime? dari, DateTime? sampai, Guid UserId);
        List<VWReportPengadaan> GetRepotPengadan3(DateTime? dari, DateTime? sampai, Guid UserId, EStatusPengadaan status);
        List<VWReportPengadaan> GetRepotPengadan4(DateTime? dari, DateTime? sampai, Guid UserId, EStatusPengadaan status, string divisi);
        List<VWPOReportDetail> GetReportPO(DateTime? dari, DateTime? sampai, Guid UserId);
        List<VWReportPks> GetReportPKS(DateTime? dari, DateTime? sampai, Guid UserId);
        List<VWReportSpk> GetReportSPK(DateTime? dari, DateTime? sampai, Guid UserId);

        int PembatalanPengadaan(VWPembatalanPengadaan vwPembatalan, Guid UserId);
        List<VWStaffCharges> GetSummaryTotal(DateTime dari, DateTime sampai, int limit = Int32.MaxValue, int skip = 0);
        List<VWStaffCharges> GetStaffCharges(string charge, DateTime dari, DateTime sampai, int limit = Int32.MaxValue, int skip = 0);
        List<VWProgressReport> GetProgressReport(DateTime dari, DateTime sampai, int limit = Int32.MaxValue, int skip = 0);
        int isNotaUploaded(Guid PengadaanId, Guid UserId);
        int isSpkUploaded(Guid PengadaanId, Guid UserId);

        PenolakanPengadaan GetPenolakanMessage(Guid Id, Guid userId);
        PembatalanPengadaan GetPembatalanPengadaan(Guid Id, Guid userId);
        Vendor GetPemenang(Guid Id, Guid userId);


        ResultMessage TolakPengadaan(Guid Id);
        RiwayatDokumen AddRiwayatDokumen(RiwayatDokumen nRiwayatDokumen);
        List<RiwayatDokumen> lstRiwayatDokumen(Guid Id);
        Pengadaan ChangeStatusPengadaan(Guid Id, EStatusPengadaan status, Guid UserId);
        int nextToStateWithChangeScheduldDate(Guid PengadaanId, Guid UserId, EStatusPengadaan state, DateTime? from, DateTime? to);
        int backToState(Guid PengadaanId, Guid UserId, EStatusPengadaan state, DateTime? from, DateTime? to);
        List<PersonilPengadaan> getPersonilPengadaan(Guid PengadaanId);
        List<VWRiwayatPengadaan> GetRiwayatDokumenForVendor(Guid UserId);
        //workflow
        // Pengadaan PersetujuanWorkflow(Guid Id, Guid UserId);
        //int AjukanWorkflow(Guid Id, Guid UserId, Guid WorkflowtemplateId);
        Reston.Helper.Util.ResultMessage saveReadyPersonil(Guid Id, int ready, Guid UserId);
        DataTablePengadaan List(string search, int start, int limit, EStatusPengadaan status, int more, int spk, List<Guid> userAprrove, Guid UserId);
        VWCountListDokumen ListCount(Guid userId, List<Guid> userAprrover, int PKSPerpanjang);
        Reston.Helper.Util.ResultMessage CekPersetujuanPemenang(Guid Id, Guid UserId);
        Reston.Helper.Util.ResultMessage SavePersetujuanPemenang(PersetujuanPemenang oPersetujuanPemenang, Guid UserId);
        Reston.Helper.Util.ResultMessage MundurPersetujuan(Guid Id, ViewPengadaan MundurPersetujuan, Guid UserId);
        Reston.Helper.Util.ResultMessage DeletePersetujuanPemenang(Guid Id);
        StatusPengajuanPemenang StatusPersetujuanPemenang(Guid PengadaanId);
        PersetujuanPemenang ChangeStatusPersetujuanPemenang(Guid Id, StatusPengajuanPemenang status, Guid UserId);
        PersetujuanPemenang getPersetujuanPemenangByPengadaanId(Guid PengadaanId);
        PersetujuanPemenang getPersetujuanPemenangById(Guid Id);
        BeritaAcara getBeritaAcaraByTipeandVendor(Guid PengadaanId, TipeBerkas tipe, int VendorId, Guid UserId);
        string GenerateNoPengadaan(Guid UserId);
        string GenerateBeritaAcara(Guid UserId);
        string GenerateBeritaAcaraNota(Guid UserId);
        string GenerateBeritaAcaraSPK(Guid UserId);
        Reston.Helper.Util.ResultMessage RemoveRks(Guid PengadaanId, Guid UserId);
        JadwalPelaksanaan SaveJadwalPelaksanaan(JadwalPelaksanaan JPelaksanaan, Guid UserId);
        JadwalPelaksanaan GetJadwalPelaksanaan(Guid PengadaanId, Guid UserId, EStatusPengadaan status);
        PersetujuanTahapan SavePersetujuanTahapan(PersetujuanTahapan data, Guid UserId);
        List<VWPersetujuanTahapan> GetPersetujuanTahapan(Guid PengadaanId, EStatusPengadaan status);
        //PersetujuanTahapan ClearPersetujuanTahapan(PersetujuanTahapan data, Guid UserId);
        PersetujuanTahapan ClearPersetujuanTahapan(PersetujuanTahapan data, Guid PengadaanId);
        PersetujuanTahapan ClearPersetujuanTahapan2(PersetujuanTahapan data2, Guid PengadaanId);

        //pengadaan Terbuka
        KandidatPengadaan addKandidatPilihanVendor(KandidatPengadaan oKandidatPengadaan, Guid UserId);
        List<ViewPengadaan> GetPengadaanAnnouncment();
        int CekBukaAmplopTahapan(Guid PengadaanId);
        //tambah tahapan
        LewatTahapan SaveTahapan(LewatTahapan data, Guid UserId);
        //LewatTahapan MundurTahapan(Guid Id);
        List<LewatTahapan> getTahapan(Guid PengadaanId);
        PersetujuanTerkait savePersetujuanTerkait(PersetujuanTerkait data);
        int deletePersetujuanTerkait(Guid Id, Guid UserId);
        PersetujuanTerkait TerkaitSetuju(PersetujuanTerkait data);
        List<PersetujuanTerkait> GetUserTerkait(Guid PengadaanId);
        List<VWPersetujuanTerkait> GetCommentUserTerkait(Guid PengadaanId);
        string GenerateNoDOKUMEN(Guid UserId, string KODE, TipeNoDokumen tipe);
        //List<PersetujuanTahapan> GetTahapanPengadaan(Guid PengadaanId, string status);

        //ambil semua vendor
        List<ViewVendors> GetAllVendors();
        List<Vendor> GetCetakVendor();
        List<VWReportPenilaianVendorCetak> GetAllPenilaianProyek(DateTime? dari, DateTime? sampai);
        List<VWReportPenilaianVendorCetak> GetAllPenilaianProyekRevByHarry(DateTime? dari, DateTime? sampai);
        VWReportPenilaianVendorDetail GetHeaderDetail(Guid rksheader);
        List<VWReferenceData> GetPertanyaan(Guid Id);
        List<VWTenderScoringDetails> GetDetailPenilaian(Guid Id);
        List<VWVendor> GetNamaVendorPenilaianKandidat(Guid Id);
        List<VWTenderScoringHeader> GetHeaderaja(Guid Id);
        InsuranceTarif InsuranceTarif(Guid PengadaanId, Guid UserId);
        List<ViewBenefitRate> saveRksAsuransiFromTemplate(Guid PengadaanId, Guid UserId, Guid DocumentIdLama, Guid DocumentIdBaru);
        List<ViewBenefitRate> saveRksAsuransiToTemplate(Guid PengadaanId, Guid UserId, Guid DocumentIdBaru);
        DataTableBenefit GetDataAsuransi(Guid DocumentIdBaru);
        Reston.Helper.Util.ResultMessage deleteBenef(int Id, Guid UserId);
        ViewCekRKSBiasaAtauAsuransi cekRKSBiasaAtauAsuransi(Guid PengadaanId);
        ViewBenefitRate GetDetailBenef(int Id, Guid UserId);
        ViewBenefitRate GetDetailBenefKlarifikasi(int Id, Guid UserId);
        ViewBenefitRate GetDetailBenefKlarifikasiLanjutan(int Id, Guid UserId);
        DataTableBenefit GetDataHargaAsuransi(Guid PengadaanId, Guid UserId);
        DataTableBenefit GetDataHargaAsuransiKlarifikasi(Guid PengadaanId, Guid UserId);
        DataTableBenefit GetDataHargaAsuransiKlarifikasiLanjutan(Guid PengadaanId, Guid UserId);
        VWRKSVendorsAsuransi getRKSPenilaianAsuransi(Guid PengadaanId, Guid UserId);
        VWRKSVendorsAsuransi getRKSKlarifikasiAsuransi(Guid PengadaanId, Guid UserId);
        VWRKSVendorsAsuransi getRKSPenilaianKlarifikasiLanjutanAsuransi(Guid PengadaanId, Guid UserId);
        VWRKSVendorsAsuransi getRKSPenilaianAsuransiNilai(Guid PengadaanId, Guid UserId);
        List<ViewVendorBenefRate> getKandidatTidakKirimAsuransi(Guid PengadaanId, Guid UserId);
        List<ViewVendorBenefRate> getKandidatKirimAsuransi(Guid PengadaanId, Guid UserId);

        PersonilPengadaan deletePersonilPengadaanMasterUser(Guid Id, Guid UserId);
        PersonilPengadaan savePersonilPengadaanMasterUser(PersonilPengadaan Personil, Guid userId);
        List<VWPks> ListPerpanjanganPKS(Guid UserId);
        int nextToDelete(Guid Id);
        //Pengadaan simpancoa(string JumlahCoa, Guid PengadaanId);
        //PersonilPengadaan HistoryBackstep(Guid Id, Guid UserId);
        List<VWBudgeting> GetUsingCOA(Guid PengadaanId);
        //Jaw's Logic
        List<VWTenderScoringDetails> nilaibobot(Guid Id);
        VWReportAssessment detailnilaibobot(Guid Id);

    }
    public class PengadaanRepo : IPengadaanRepo
    {
        AppDbContext ctx;

        public List<ViewPertanyaan> GetDataPertanyaan()
        {
            var pertanyaan = ctx.ReferenceDatas.Where(x => x.Qualifier.Equals("Pertanyaan")).ToList();
            List<ViewPertanyaan> listpertanyaan = new List<ViewPertanyaan>();
            foreach (var item in pertanyaan)
            {
                ViewPertanyaan vp = new ViewPertanyaan();
                vp.Code = item.Code;
                vp.LocalizedName = item.LocalizedName;
                listpertanyaan.Add(vp);
            }
            return listpertanyaan;
        }

        public List<VWTenderScoring> cekmasterdata(Guid Id)
        {
            List<VWTenderScoring> vwTenderScoring = (from b in ctx.TenderScoringDetails
                                                     join c in ctx.TenderScoringHeaders on b.TenderScoringHeaderId equals c.Id
                                                     join d in ctx.TenderScoringBobots on b.Code equals d.Code
                                                     join e in ctx.ReferenceDatas on b.Code equals e.Code
                                                     where c.PengadaanId == Id && d.PengadaanId == Id
                                                     select new VWTenderScoring
                                                     {
                                                         Code = b.Code,
                                                         LocalizedName = e.LocalizedName,
                                                         Bobot = d.Bobot
                                                     }).Distinct().ToList();
            return vwTenderScoring;
        }

        public VWReportAssessment ReportAssessment(Guid Id)
        {
            VWReportAssessment report = (from a in ctx.TenderScoringHeaders
                                         where a.PengadaanId == Id
                                         select new VWReportAssessment
                                         {
                                             PengadaanId = Id,
                                             Total = a.Total,
                                             Average = a.Average,
                                             ReferenceDatas = (from b in ctx.TenderScoringDetails
                                                               join c in ctx.ReferenceDatas on b.Code equals c.Code
                                                               join d in ctx.TenderScoringHeaders on b.TenderScoringHeaderId equals d.Id
                                                               where d.PengadaanId == Id
                                                               select new VWReferenceData
                                                               {
                                                                   Code = b.Code,
                                                                   LocalizedName = c.LocalizedName
                                                               }).Distinct().ToList(),
                                             Vendors = (from b in ctx.Vendors
                                                        join c in ctx.TenderScoringHeaders on b.Id equals c.VendorId
                                                        where c.PengadaanId == Id
                                                        select new VWVendor
                                                        {
                                                            Id = b.Id,
                                                            Nama = b.Nama
                                                        }).Distinct().ToList(),
                                             TenderScoringHeaders = (from b in ctx.TenderScoringHeaders
                                                                     where b.PengadaanId == Id
                                                                     select new VWTenderScoringHeader
                                                                     {
                                                                         Total = b.Total,
                                                                         Averages = b.Average
                                                                     }).ToList(),
                                             TenderScoringDetails = (from b in ctx.TenderScoringHeaders
                                                                     join c in ctx.TenderScoringDetails on b.Id equals c.TenderScoringHeaderId into gj
                                                                     from c in gj.DefaultIfEmpty()
                                                                     where b.PengadaanId == Id
                                                                     select new VWTenderScoringDetails
                                                                     {
                                                                         Code = c.Code,
                                                                         Averages_All_User = c == null ? 0 : c.AverageAllUser,
                                                                         Total_All_User = c == null ? 0 : c.TotalAllUser
                                                                     }).ToList(),
                                             TenderScoringUsers = (from b in ctx.TenderScoringUsers
                                                                   join c in ctx.TenderScoringDetails on b.TenderScoringDetailId equals c.Id
                                                                   join d in ctx.TenderScoringHeaders on c.TenderScoringHeaderId equals d.Id
                                                                   join e in ctx.ReferenceDatas on c.Code equals e.Code
                                                                   where d.PengadaanId == Id
                                                                   select new VWTenderScoringUser
                                                                   {
                                                                       NamaVendor = ctx.Vendors.Where(x => x.Id == d.VendorId).FirstOrDefault().Nama,
                                                                       NamaUser = ctx.PersonilPengadaans.Where(x => x.PersonilId == b.UserId).FirstOrDefault().Nama,
                                                                       UserId = b.UserId,
                                                                       Pertanyaan = e.LocalizedName,
                                                                       Score = b.Score
                                                                   }).OrderBy(x => x.NamaUser).ToList(),
                                         }).FirstOrDefault();
            return report;
        }

        public List<VWTenderScoringDetails> UrutanPertanyaan(Guid Id)
        {
            var detail = (from b in ctx.TenderScoringHeaders
                          join c in ctx.TenderScoringDetails on b.Id equals c.TenderScoringHeaderId into gj
                          from c in gj.DefaultIfEmpty()
                          join e in ctx.ReferenceDatas on c.Code equals e.Code
                          join d in ctx.Vendors on b.VendorId equals d.Id
                          orderby c.Id
                          where b.PengadaanId == Id
                          select new VWTenderScoringDetails
                          {
                              Code = c.Code,
                              pertanyaan = e.LocalizedName,
                              VendorId = b.VendorId,
                              namasivendor = d.Nama

                          }).ToList();

            return detail;
        }

        public List<VWTenderScoringHeader> GetHeaderPertanyaan(Guid Id)
        {
            var headerto = (from a in ctx.TenderScoringHeaders
                            where a.PengadaanId == Id
                            select new VWTenderScoringHeader
                            {
                                Id = a.Id,
                                Total = a.Total,
                                Averages = a.Average
                            }).ToList();
            return headerto;
        }

        public List<VWReferenceData> GetCountPertanyaan(Guid Id)
        {
            var pertanyaan = (from a in ctx.TenderScoringHeaders
                              join b in ctx.TenderScoringDetails on a.Id equals b.TenderScoringHeaderId
                              join c in ctx.ReferenceDatas on b.Code equals c.Code
                              where a.PengadaanId == Id
                              select new VWReferenceData
                              {
                                  LocalizedName = c.LocalizedName
                              }).Distinct().ToList();
            return pertanyaan;
        }

        public PengadaanRepo(AppDbContext j)
        {
            ctx = j;
            //ctx.Configuration.ProxyCreationEnabled = false;
            ctx.Configuration.LazyLoadingEnabled = true;
        }

        Reston.Helper.Util.ResultMessage msg = new Reston.Helper.Util.ResultMessage();

        public int arsipkan(Guid Id, Guid UserId)
        {
            int ispic = (from xx in ctx.PersonilPengadaans
                         where xx.PengadaanId == Id && xx.tipe == "pic" && xx.PersonilId == UserId
                         select xx).Count() > 0 ? 1 : 0;
            if (ispic == 0) return 0;
            Pengadaan oPengadaan = ctx.Pengadaans.Find(Id);
            if (oPengadaan == null) return 0;
            oPengadaan.GroupPengadaan = EGroupPengadaan.ARSIP;
            ctx.SaveChanges();
            return 1;
        }

        public Pengadaan GetPengadaanByiD(Guid id)
        {
            return ctx.Pengadaans.Find(id);
        }

        public List<vwProduk> GetAllProduk(string term)
        {
            var oProduk = ctx.Produks.Where(d => d.Nama.Contains(term)).Take(15).ToList();

            List<vwProduk> LstnVwProduk = new List<vwProduk>();
            foreach (var item in oProduk)
            {
                vwProduk nVwProduk = new vwProduk();
                var Keterangan = "";
                nVwProduk.Id = item.Id;
                nVwProduk.Nama = item.Nama;
                nVwProduk.Satuan = item.Satuan;
                nVwProduk.Deskripsi = item.Deskripsi;
                nVwProduk.RiwayatHarga = item.RiwayatHarga.ToList();
                if (item.KategoriSpesifikasi != null)
                {
                    var oSpesifikasi = item.KategoriSpesifikasi.AtributSpesifikasi.ToList();
                    if (oSpesifikasi.Count() > 0)
                    {
                        var firstSpek = oSpesifikasi.FirstOrDefault();
                        var lasttSpek = oSpesifikasi.LastOrDefault();
                        foreach (var spek in oSpesifikasi)
                        {
                            if (spek == firstSpek)
                                Keterangan = spek.Nama + ": " + spek.Nilai + ", ";
                            else if (spek == lasttSpek) Keterangan = Keterangan + spek.Nama + ": " + spek.Nilai;
                            else Keterangan = Keterangan + ", " + spek.Nama + ": " + spek.Nilai + ", ";

                        }
                    }
                }
                nVwProduk.Spesifikasi = Keterangan;
                LstnVwProduk.Add(nVwProduk);
            }
            return LstnVwProduk;
        }

        public List<VWReportPenilaianVendorCetak> GetAllPenilaianProyek(DateTime? dari, DateTime? sampai)
        {
            var PenilaianVendor = (from a in ctx.RencanaProyeks
                                   join b in ctx.Spk on a.SpkId equals b.Id
                                   join c in ctx.PemenangPengadaans on b.PemenangPengadaanId equals c.Id
                                   join d in ctx.Pengadaans on c.PengadaanId equals d.Id
                                   join e in ctx.Vendors on c.VendorId equals e.Id
                                   join f in ctx.PenilaianVendorHeaders on a.Id equals f.ProyekId
                                   where b.TanggalSPK >= dari && b.TanggalSPK <= sampai
                                   select new VWReportPenilaianVendorCetak
                                   {
                                       NamaProject = d.Judul,
                                       User = d.UnitKerjaPemohon,
                                       Vendor = e.Nama,
                                       Note = f.Catatan,
                                       rksheader = f.Id
                                   }).ToList();

            return PenilaianVendor;
        }

        public List<VWReportPenilaianVendorCetak> GetAllPenilaianProyekRevByHarry(DateTime? dari, DateTime? sampai)
        {
            var PenilaianVendor = (from a in ctx.PenilaianVendorHeaders
                                   join b in ctx.Spk on a.Spk_Id equals b.Id
                                   join c in ctx.PemenangPengadaans on b.PemenangPengadaanId equals c.Id
                                   join d in ctx.Pengadaans on c.PengadaanId equals d.Id
                                   join e in ctx.Vendors on c.VendorId equals e.Id
                                   //join f in ctx.PenilaianVendorHeaders on a.Id equals f.ProyekId
                                   where b.TanggalSPK >= dari && b.TanggalSPK <= sampai
                                   select new VWReportPenilaianVendorCetak
                                   {
                                       NamaProject = d.Judul,
                                       User = d.UnitKerjaPemohon,
                                       Vendor = e.Nama,
                                       Note = a.Catatan,
                                       rksheader = a.Id
                                   }).ToList();

            return PenilaianVendor;
        }


        public List<VWPersetujuanTerkait> GetCommentUserTerkait(Guid PengadaanId)
        {
            var komen = (from a in ctx.PersetujuanTerkait

                         where a.PengadaanId == PengadaanId
                         select new VWPersetujuanTerkait
                         {
                             UserId = a.UserId,
                             setuju = a.setuju,
                             disposisi = a.CommentPersetujuanTerkait
                         }).ToList();
            return komen;

        }

        public List<VWTenderScoringHeader> GetHeaderPenilaianKandidat(Guid Id)
        {
            var penilaiankandidat = (from a in ctx.TenderScoringHeaders
                                     where a.PengadaanId == Id
                                     select new VWTenderScoringHeader
                                     {
                                         Total = a.Total,
                                         Averages = a.Average,
                                         Id = a.Id
                                     }).ToList();
            return penilaiankandidat;
        }

        public List<VWReferenceData> GetPertanyaan(Guid Id)
        {

            var pertanyaan = (from a in ctx.TenderScoringHeaders
                              join b in ctx.TenderScoringDetails on a.Id equals b.TenderScoringHeaderId
                              join c in ctx.ReferenceDatas on b.Code equals c.Code
                              where a.PengadaanId == Id
                              select new VWReferenceData
                              {
                                  LocalizedName = c.LocalizedName
                              }).Distinct().ToList();
            return pertanyaan;
        }

        public List<VWTenderScoringHeader> GetHeaderaja(Guid Id)
        {
            var headerto = (from a in ctx.TenderScoringHeaders
                            where a.PengadaanId == Id
                            select new VWTenderScoringHeader
                            {
                                Id = a.Id,
                                Total = a.Total,
                                Averages = a.Average
                            }).ToList();
            return headerto;
        }

        public List<VWTenderScoringDetails> GetDetailPenilaian(Guid Id)
        {
            //var detail = ctx.TenderScoringDetails.Where(d => d.TenderScoringHeaderId == header).ToList();

            //VWTenderScoringDetails listtender = new VWTenderScoringDetails();
            //foreach (var item in detail)
            //{
            //    listtender.Total_All_User = item.TotalAllUser;
            //    listtender.Averages_All_User = item.AverageAllUser;

            //}
            //return listtender;

            var detailnilai = (from b in ctx.TenderScoringHeaders
                               join a in ctx.TenderScoringDetails on b.Id equals a.TenderScoringHeaderId
                               where b.PengadaanId == Id
                               select new VWTenderScoringDetails
                               {
                                   Averages_All_User = a.AverageAllUser,
                                   Total_All_User = a.TotalAllUser,
                                   VendorId = b.VendorId
                               }).ToList();
            return detailnilai;
        }

        public List<VWVendor> GetNamaVendorPenilaianKandidat(Guid Id)
        {
            var namavendor = (from a in ctx.TenderScoringHeaders
                              join b in ctx.Vendors on a.VendorId equals b.Id
                              where a.PengadaanId == Id
                              select new VWVendor
                              {
                                  Nama = b.Nama,
                                  Id = b.Id
                              }
                              ).ToList();

            return namavendor;
        }

        public VWReportPenilaianVendorDetail GetHeaderDetail(Guid rksheader)
        {
            var detail = ctx.PenilaianVendorDetails.Where(d => d.PenilaianVendorHeaderId == rksheader).ToList();

            VWReportPenilaianVendorDetail nm = new VWReportPenilaianVendorDetail();

            foreach (var item_b in detail)
            {
                var refer = ctx.ReferenceDatas.Where(d => d.Id == item_b.ReferenceDataId).FirstOrDefault();

                if (refer.LocalizedName == "Quality of Products")
                {
                    nm.QualityOfProduct = item_b.Nilai;
                }
                else if (refer.LocalizedName == "Quality of Services")
                {
                    nm.QualityOfService = item_b.Nilai;
                }
                else if (refer.LocalizedName == "Cost")
                {
                    nm.Cost = item_b.Nilai;
                }
                else if (refer.LocalizedName == "Delivery")
                {
                    nm.Delivery = item_b.Nilai;
                }
                else if (refer.LocalizedName == "Flexibility")
                {
                    nm.Flexibility = item_b.Nilai;
                }
                else if (refer.LocalizedName == "Responsiveness")
                {
                    nm.Responsiveness = item_b.Nilai;
                }
            }
            return nm;

        }
        public List<vwProduk> GetItemByRegion(string term, string region)
        {
            var oProduk = ctx.Produks.Where(d => d.Nama.Contains(term)).Take(15).ToList();

            List<vwProduk> LstnVwProduk = new List<vwProduk>();
            foreach (var item in oProduk)
            {
                vwProduk nVwProduk = new vwProduk();
                var Keterangan = "";
                nVwProduk.Id = item.Id;
                nVwProduk.Nama = item.Nama;
                nVwProduk.Satuan = item.Satuan;
                nVwProduk.Deskripsi = item.Deskripsi;
                nVwProduk.RiwayatHarga = item.RiwayatHarga.Where(xx => xx.Region == region).ToList();
                if (item.KategoriSpesifikasi != null)
                {
                    var oSpesifikasi = item.KategoriSpesifikasi.AtributSpesifikasi.ToList();
                    if (oSpesifikasi.Count() > 0)
                    {
                        var firstSpek = oSpesifikasi.FirstOrDefault();
                        var lasttSpek = oSpesifikasi.LastOrDefault();
                        foreach (var spek in oSpesifikasi)
                        {
                            if (spek == firstSpek)
                                Keterangan = spek.Nama + ": " + spek.Nilai + ", ";
                            else if (spek == lasttSpek) Keterangan = Keterangan + spek.Nama + ": " + spek.Nilai;
                            else Keterangan = Keterangan + ", " + spek.Nama + ": " + spek.Nilai + ", ";

                        }
                    }
                }
                nVwProduk.Spesifikasi = Keterangan;
                LstnVwProduk.Add(nVwProduk);
            }
            return LstnVwProduk;
        }

        public List<vwProduk> GetAllSatuan(string term)
        {
            return ctx.Produks.Where(d => d.Satuan.Contains(term)).Select(d => new vwProduk { Satuan = d.Satuan }).Distinct().Take(15).ToList();
        }

        public static bool IsDateBeforeOrToday(DateTime input)
        {
            var todaysDate = DateTime.Today;

            if (input < todaysDate)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public dataVendors GetVendors(ETipeVendor tipe, int start, string filter, EStatusVendor status, int limit)
        {
            if (limit > 0)
            {
                dataVendors odataVendors = new dataVendors();
                var oData =
                ctx.Vendors.Where(x => ((tipe == ETipeVendor.NONE & tipe != ETipeVendor.NON_REGISTER) || x.TipeVendor == tipe)
                    && (status == EStatusVendor.NONE || x.StatusAkhir == status) && (x.StatusAkhir != EStatusVendor.BLACKLIST && x.TipeVendor != ETipeVendor.NON_REGISTER));

                var ListVendorWithSanksi = (from b in ctx.Vendors
                                            join c in ctx.Sanksis on b.Id equals c.VendorId into ps
                                            //join c in q on b.Id equals c.VendorId into ps
                                            from p in ps.DefaultIfEmpty()
                                            select new VWSanksi
                                            {
                                                VendorId = b.Id,
                                                NamaVendor = b.Nama,
                                                NomorVendor = b.NomorVendor,
                                                DecisionTypeCode = String.IsNullOrEmpty(p.DecisionTypeCode) ? "SV01" : p.DecisionTypeCode,
                                                DecisionDescription = String.IsNullOrEmpty(p.DecisionDescription) ? "" : p.DecisionDescription,
                                                DecisionValidUntil = String.IsNullOrEmpty(p.DecisionValidUntil.ToString()) ? DateTime.MinValue : p.DecisionValidUntil,
                                                CreatedOn = String.IsNullOrEmpty(p.DecisionValidUntil.ToString()) ? DateTime.MinValue : p.CreatedOn,
                                            }).Distinct().OrderByDescending(xx => xx.CreatedOn).ToList();

                List<VWSanksi> Ulu = new List<VWSanksi>();
                foreach (var item in ListVendorWithSanksi)
                {
                    VWSanksi Ula = new VWSanksi();
                    Ula.VendorId = item.VendorId;
                    Ula.NamaVendor = item.NamaVendor;
                    Ula.NomorVendor = item.NomorVendor;
                    Ula.DecisionTypeCode = IsDateBeforeOrToday(item.DecisionValidUntil) == false ? "SV01" : item.DecisionTypeCode;
                    Ula.DecisionDescription = IsDateBeforeOrToday(item.DecisionValidUntil) == false ? "" : item.DecisionDescription;
                    Ula.DalamMasaSanksi = IsDateBeforeOrToday(item.DecisionValidUntil) == true ? "masih" : "tidak";
                    var check = Ulu.Where(xx => xx.VendorId == Ula.VendorId).FirstOrDefault() == null ? "belum" : "ada";
                    if (check == "belum" && Ula.DecisionTypeCode == "SV03" && Ula.DalamMasaSanksi == "masih" ||
                        check == "belum" && Ula.DecisionTypeCode == "SV04" && Ula.DalamMasaSanksi == "masih") Ulu.Add(Ula);
                }

                //var zz = Ulu.Select(
                //aa => new VWSanksi
                //{
                //    VendorId = aa.VendorId,
                //    NamaVendor = aa.NamaVendor,
                //    NomorVendor = aa.NomorVendor,
                //    DecisionTypeCode = aa.DecisionTypeCode,
                //    DecisionDescription = aa.DecisionDescription
                //}).ToList();

                //oData = oData.Where(a => a.TipeVendor != ETipeVendor.NON_REGISTER);
                //odataVendors.totalRecord = oData.Count();
                if (!string.IsNullOrEmpty(filter)) oData = oData.Where(d => d.Nama.Contains(filter));

                List<ViewVendors> lv = oData.Select(d => new ViewVendors
                {
                    Nama = d.Nama,
                    id = d.Id,
                    Telepon = d.Telepon,
                    Alamat = d.Alamat,
                    //StatusAkhir = String.IsNullOrEmpty(Ulu.Select(xx => xx.VendorId == d.Id).) ? "Masuk" : "Tidak"
                }).Where(x => x.Nama != null).OrderByDescending(x => x.id).Skip(start).Take(limit)
                    .ToList();

                List<ViewVendors> Ulurs = new List<ViewVendors>();
                foreach (var item in lv)
                {
                    ViewVendors Ula = new ViewVendors();
                    Ula.Nama = item.Nama;
                    Ula.id = item.id;
                    Ula.Telepon = item.Telepon;
                    Ula.Alamat = item.Alamat;
                    Ula.StatusAkhir = Ulu.Where(xx => xx.VendorId == item.id).FirstOrDefault() == null ? "Masuk" : "Tidak";
                    Ulurs.Add(Ula);
                }

                odataVendors.Vendors = Ulurs.Where(xx => xx.StatusAkhir == "Masuk").ToList();
                odataVendors.totalRecord = lv.Count();
                return odataVendors;
            }
            return new dataVendors();
        }

        public List<VWVendor> GetVendorsByPengadaanId(Guid PengadaanId)
        {
            var xx = (from b in ctx.Vendors
                      join c in ctx.KandidatPengadaans on b.Id equals c.VendorId
                      where c.PengadaanId == PengadaanId
                      select new VWVendor
                      {
                          email = (from bb in ctx.Vendors
                                   where bb.Id == c.VendorId
                                   select bb).FirstOrDefault() != null ? (from bb in ctx.Vendors
                                                                          where bb.Id == c.VendorId
                                                                          select bb).FirstOrDefault().Email : "",
                          Nama = (from bb in ctx.Vendors
                                  where bb.Id == c.VendorId
                                  select bb).FirstOrDefault() != null ? (from bb in ctx.Vendors
                                                                         where bb.Id == c.VendorId
                                                                         select bb).FirstOrDefault().Nama : ""
                      }).Distinct().ToList();
            return xx;

        }

        public List<VWVendor> GetVendorsKlarifikasiByPengadaanId(Guid PengadaanId)
        {
            var xx = (from b in ctx.Vendors
                      join c in ctx.PemenangPengadaans on b.Id equals c.VendorId
                      where c.PengadaanId == PengadaanId
                      select new VWVendor
                      {
                          email = (from bb in ctx.Vendors
                                   where bb.Id == c.VendorId
                                   select bb).FirstOrDefault() != null ? (from bb in ctx.Vendors
                                                                          where bb.Id == c.VendorId
                                                                          select bb).FirstOrDefault().Email : "",
                          Nama = (from bb in ctx.Vendors
                                  where bb.Id == c.VendorId
                                  select bb).FirstOrDefault() != null ? (from bb in ctx.Vendors
                                                                         where bb.Id == c.VendorId
                                                                         select bb).FirstOrDefault().Nama : ""
                      }).Distinct().ToList();
            return xx;

        }

        public List<VWVendor> GetVendorsKlarifikasiByPengadaanId2(Guid PengadaanId)
        {
            var xx = (from b in ctx.Vendors
                      join c in ctx.KandidatPengadaans on b.Id equals c.VendorId
                      where c.PengadaanId == PengadaanId
                      select new VWVendor
                      {
                          email = (from bb in ctx.Vendors
                                   where bb.Id == c.VendorId
                                   select bb).FirstOrDefault() != null ? (from bb in ctx.Vendors
                                                                          where bb.Id == c.VendorId
                                                                          select bb).FirstOrDefault().Email : "",
                          Nama = (from bb in ctx.Vendors
                                  where bb.Id == c.VendorId
                                  select bb).FirstOrDefault() != null ? (from bb in ctx.Vendors
                                                                         where bb.Id == c.VendorId
                                                                         select bb).FirstOrDefault().Nama : ""
                      }).Distinct().ToList();
            return xx;

        }

        public List<VWVendor> GetAssessment(Guid Id)
        {
            List<VWVendor> vendor = (from b in ctx.Vendors
                                     join c in ctx.TenderScoringHeaders on b.Id equals c.VendorId
                                     where c.PengadaanId == Id
                                     select new VWVendor
                                     {
                                         Id = b.Id,
                                         Nama = b.Nama
                                     }).ToList();
            return vendor;
        }

        public List<VWTenderScoring> GetValueAssessment(Guid Id, Guid UserIdAssessment, int VendorId)
        {
            List<VWTenderScoring> valtenderscoringuser = (from b in ctx.TenderScoringUsers
                                                          join c in ctx.TenderScoringDetails on b.TenderScoringDetailId equals c.Id
                                                          join d in ctx.TenderScoringHeaders on c.TenderScoringHeaderId equals d.Id
                                                          join e in ctx.ReferenceDatas on c.Code equals e.Code
                                                          join f in ctx.TenderScoringBobots on c.Code equals f.Code
                                                          //where d.VendorId == VendorId && d.PengadaanId == Id && b.UserId == UserIdAssessment <-Punya si-Gondrong
                                                          where d.VendorId == VendorId && f.PengadaanId == Id && d.PengadaanId == Id && b.UserId == UserIdAssessment
                                                          select new VWTenderScoring
                                                          {
                                                              PengadaanId = d.PengadaanId,
                                                              VendorId = d.VendorId,
                                                              Code = e.Code,
                                                              LocalizedName = e.LocalizedName,
                                                              UserId = b.UserId,
                                                              Score = b.Score,
                                                              Bobot = f.Bobot
                                                          }).ToList();
            return valtenderscoringuser;
        }

        public List<VWPersonilPengadaan> GetDropDownPenilai(Guid Id)
        {
            List<VWPersonilPengadaan> listpenilai = (from b in ctx.PersonilPengadaans
                                                     where b.PengadaanId == Id
                                                     select new VWPersonilPengadaan
                                                     {
                                                         PersonilId = b.PersonilId,
                                                         Nama = b.Nama,
                                                         Jabatan = b.Jabatan,
                                                         PengadaanId = b.PengadaanId
                                                     }).ToList();
            return listpenilai;
        }

        public List<VWTenderScoring> GetQuestion(Guid Id, int VendorId)
        {
            List<VWTenderScoring> refdata = (from b in ctx.TenderScoringDetails
                                             join c in ctx.TenderScoringHeaders on b.TenderScoringHeaderId equals c.Id
                                             join d in ctx.ReferenceDatas on b.Code equals d.Code
                                             join e in ctx.TenderScoringBobots on b.Code equals e.Code
                                             where c.PengadaanId == Id && e.PengadaanId == Id && c.VendorId == VendorId
                                             select new VWTenderScoring
                                             {
                                                 TenderScoringDetailId = b.Id,
                                                 Code = b.Code,
                                                 LocalizedName = d.LocalizedName,
                                                 Bobot = e.Bobot
                                             }).Distinct().ToList();
            return refdata;
        }

        public ViewPengadaan GetPengadaan(Guid id)
        {

            ViewPengadaan vwPengadaan = (from b in ctx.Pengadaans
                                         where b.Id == id
                                         select new ViewPengadaan
                                         {
                                             Judul = b.Judul,
                                             NoPengadaan = b.NoPengadaan
                                         }).FirstOrDefault();
            return vwPengadaan;
        }

        public VWTenderScoring CekPertanyaan(Guid id)
        {

            VWTenderScoring cekpertanyaan = (from b in ctx.TenderScoringHeaders
                                             where b.PengadaanId == id
                                             select new VWTenderScoring
                                             {
                                                 PengadaanId = b.PengadaanId,
                                                 VendorId = b.VendorId
                                             }).FirstOrDefault();
            return cekpertanyaan;
        }

        public List<VWTenderScoring> ScorePenilaian(Guid Id)
        {
            List<VWTenderScoring> score = (
                from b in ctx.TenderScoringDetails
                join c in ctx.TenderScoringHeaders on b.TenderScoringHeaderId equals c.Id
                where c.PengadaanId == Id
                select new VWTenderScoring
                {
                    Score = b.TotalAllUser,
                    Total = b.AverageAllUser,
                }).ToList();

            return score;
        }

        public List<VWTenderScoring> GetDataAssessment(Guid Id, int VendorId, Guid UserId)
        {
            List<VWTenderScoring> vwTenderScoring = (from b in ctx.TenderScoringDetails
                                                     join c in ctx.TenderScoringHeaders on b.TenderScoringHeaderId equals c.Id
                                                     join d in ctx.TenderScoringUsers on b.Id equals d.TenderScoringDetailId
                                                     join e in ctx.ReferenceDatas on b.Code equals e.Code
                                                     join f in ctx.TenderScoringBobots on b.Code equals f.Code
                                                     //where c.PengadaanId == Id && c.VendorId == VendorId && d.UserId == UserId  <-Punya si-Gondrong
                                                     where c.PengadaanId == Id && f.PengadaanId == Id && c.VendorId == VendorId && d.UserId == UserId
                                                     select new VWTenderScoring
                                                     {
                                                         Score = d.Score,
                                                         Code = b.Code,
                                                         LocalizedName = e.LocalizedName,
                                                         Bobot = f.Bobot
                                                     }).Distinct().ToList();
            return vwTenderScoring;
        }

        public List<VWTenderScoring> GetPointPenilaian(Guid Id)
        {
            List<VWTenderScoring> vwTenderScoring = (from b in ctx.TenderScoringDetails
                                                     join c in ctx.TenderScoringHeaders on b.TenderScoringHeaderId equals c.Id
                                                     join d in ctx.TenderScoringBobots on b.Code equals d.Code
                                                     join e in ctx.ReferenceDatas on b.Code equals e.Code
                                                     //where c.PengadaanId == Id <-Punya si-Gondrong
                                                     where c.PengadaanId == Id && d.PengadaanId == Id
                                                     select new VWTenderScoring
                                                     {
                                                         Code = b.Code,
                                                         LocalizedName = e.LocalizedName,
                                                         Bobot = d.Bobot
                                                     }).Distinct().ToList();
            return vwTenderScoring;
        }

        public List<VWPemenangPengadaan> PemenangPengadaan(Guid id)
        {
            List<VWPemenangPengadaan> vwPemenang = (from b in ctx.PemenangPengadaans
                                                    where b.PengadaanId == id
                                                    select new VWPemenangPengadaan
                                                    {
                                                        Nama = b.Vendor.Nama,
                                                        VendorId = b.Vendor.Id
                                                    }).ToList();
            return vwPemenang;
        }

        public ViewPengadaan GetPengadaan(Guid id, Guid UserID, int approver)
        {
            // Guid manajer = new Guid(ConfigurationManager.AppSettings["manajer"].ToString());
            // int approver = UserID == manajer ? 1 : 0;
            int ispic = (from aa in ctx.PersonilPengadaans
                         where aa.PengadaanId == id && aa.tipe == "pic" && aa.PersonilId == UserID
                         select aa).Count() > 0 ? 1 : 0;
            int isteam = (from bb in ctx.PersonilPengadaans
                          where bb.PengadaanId == id && (bb.tipe == "tim" || bb.tipe == "pic") && bb.PersonilId == UserID
                          select bb).Count() > 0 ? 1 : 0;
            int isPersonil = (from cc in ctx.PersonilPengadaans
                              where cc.PengadaanId == id && cc.PersonilId == UserID
                              select cc).Count() > 0 ? 1 : 0;
            int isController = (from aa in ctx.PersonilPengadaans
                                where aa.PengadaanId == id && aa.tipe == "controller" && aa.PersonilId == UserID
                                select aa).Count() > 0 ? 1 : 0;
            int isCompliance = (from aa in ctx.PersonilPengadaans
                                where aa.PengadaanId == id && aa.tipe == "compliance" && aa.PersonilId == UserID
                                select aa).Count() > 0 ? 1 : 0;
            int isUser = (from aa in ctx.PersonilPengadaans
                          where aa.PengadaanId == id && aa.tipe == "staff" && aa.PersonilId == UserID
                          select aa).Count() > 0 ? 1 : 0;
            int isMundurTahapanPelaksanaan = (from aa in ctx.PersetujuanTerkait
                                              where aa.PengadaanId == id && aa.setuju == 2
                                              select aa).Count() > 0 ? 1 : 0;
            //int isMajuTahapanPelaksanaan = (from aa in ctx.PersetujuanTerkait
            //                                  where aa.PengadaanId == id && aa.setuju == 1
            //                                  select aa).Count() > 0 ? 1 : 0;

            ViewPengadaan oVWPengadaan = (from b in ctx.Pengadaans
                                          where b.Id == id
                                          select new ViewPengadaan
                                          {
                                              Approver = b.Status == EStatusPengadaan.AJUKAN ? approver : 0,
                                              Id = b.Id,
                                              Judul = b.Judul,
                                              AturanBerkas = b.AturanBerkas,
                                              AturanPenawaran = b.AturanPenawaran,
                                              AturanPengadaan = b.AturanPengadaan,
                                              Keterangan = b.Keterangan,
                                              Status = b.Status,
                                              StatusName = b.Status.ToString(),
                                              GroupPengadaan = b.GroupPengadaan,
                                              JenisPekerjaan = b.JenisPekerjaan,
                                              JenisPembelanjaan = b.JenisPembelanjaan,
                                              MataUang = b.MataUang,
                                              PeriodeAnggaran = b.PeriodeAnggaran,
                                              NoCOA = b.NoCOA,
                                              Pagu = b.Pagu,
                                              Region = b.Region,
                                              Provinsi = b.Provinsi,
                                              KualifikasiRekan = b.KualifikasiRekan,
                                              UnitKerjaPemohon = b.UnitKerjaPemohon,
                                              TitleDokumenNotaInternal = b.TitleDokumenNotaInternal,
                                              TitleDokumenLain = b.TitleDokumenLain,
                                              TitleBerkasRujukanLain = b.TitleBerkasRujukanLain,
                                              isCreated = UserID == b.CreatedBy ? 1 : 0,
                                              isPIC = ispic,
                                              isTEAM = isteam,
                                              isPersonil = isPersonil,
                                              isCompliance = isCompliance,
                                              isController = isController,
                                              isUser = isUser,
                                              isMundurTahapanPelaksanaan = isMundurTahapanPelaksanaan,
                                              Branch = b.Branch,
                                              Department = b.Department,
                                              JenisPembelanjaanString = ctx.ReferenceDatas.Where(x => x.Code.Equals(b.JenisPembelanjaan)).Select(x => x.LocalizedName).Distinct().FirstOrDefault(),
                                              PeriodeAnggaranString = ctx.ReferenceDatas.Where(x => x.Code.Equals(b.PeriodeAnggaran)).Select(x => x.LocalizedName).Distinct().FirstOrDefault(),

                                              NoPengadaan = b.NoPengadaan,
                                              PersonilPengadaans = (from bb in ctx.PersonilPengadaans
                                                                    where bb.PengadaanId == b.Id
                                                                    select new VWPersonilPengadaan
                                                                    {
                                                                        Id = bb.Id,
                                                                        Jabatan = bb.Jabatan,
                                                                        Nama = bb.Nama,
                                                                        PersonilId = bb.PersonilId,
                                                                        tipe = bb.tipe,
                                                                        isReady = bb.isReady,
                                                                        isMine = UserID == bb.PersonilId ? 1 : 0
                                                                    }).ToList(),
                                              KandidatPengadaans = (from bb in ctx.KandidatPengadaans
                                                                    join cc in ctx.Vendors on bb.VendorId equals cc.Id
                                                                    where bb.PengadaanId == b.Id
                                                                    select new VWKandidatPengadaan
                                                                    {
                                                                        Id = bb.Id,
                                                                        PengadaanId = bb.PengadaanId,
                                                                        VendorId = bb.VendorId,
                                                                        Nama = cc.Nama
                                                                    }).ToList(),
                                              JadwalPengadaans = (from bb in ctx.JadwalPengadaans
                                                                  where bb.PengadaanId == b.Id
                                                                  select new VWJadwalPengadaan
                                                                  {
                                                                      Id = bb.Id,
                                                                      PengadaanId = bb.PengadaanId,
                                                                      Mulai = bb.Mulai,
                                                                      Sampai = bb.Sampai,
                                                                      tipe = bb.tipe
                                                                  }).ToList(),
                                              DokumenPengadaans = (from bb in ctx.DokumenPengadaans
                                                                   where bb.PengadaanId == b.Id
                                                                   select new VWDokumenPengadaan
                                                                   {
                                                                       Id = bb.Id,
                                                                       PengadaanId = bb.PengadaanId,
                                                                       ContentType = bb.ContentType,
                                                                       File = bb.File,
                                                                       Tipe = bb.Tipe,
                                                                       Title = bb.Title
                                                                   }).ToList(),

                                              KualifikasiKandidats = (from bb in ctx.KualifikasiKandidats
                                                                      where bb.PengadaanId == b.Id
                                                                      select new VWKualifikasiKandidat
                                                                      {
                                                                          Id = bb.Id,
                                                                          PengadaanId = bb.PengadaanId,
                                                                          kualifikasi = bb.kualifikasi
                                                                      }).ToList(),
                                              isKlarifikasiLanjutan = (from bb in ctx.LewatTahapans
                                                                       where bb.PengadaanId == b.Id && bb.Status == EStatusPengadaan.KLARIFIKASILANJUTAN
                                                                       select bb).Count() > 0 ? 1 : 0,
                                              isPenilaian = (from bb in ctx.LewatTahapans
                                                             where bb.PengadaanId == b.Id && bb.Status == EStatusPengadaan.PENILAIAN
                                                             select bb).Count() > 0 ? 1 : 0,
                                              //isMundurTahapanPelaksanaan = (from aa in ctx.PersetujuanTerkait
                                              //                              where aa.PengadaanId == id && aa.setuju == 2
                                              //                              select aa).Count() > 0 ? 1 : 0,
                                              PengadaanLangsung = b.PengadaanLangsung,

                                          }).FirstOrDefault();

            if (oVWPengadaan.Status == EStatusPengadaan.DISETUJUI)
            {
                if (cekStateDiSetujui(oVWPengadaan.Id) == 1)
                {
                    oVWPengadaan.Status = EStatusPengadaan.AANWIJZING;
                }

            }

            /*tambah kandidat yang tidak hadir*/
            var historykandidat = (from bb in ctx.HistoryKandidatPengadaan
                                   join cc in ctx.Vendors on bb.VendorId equals cc.Id
                                   where bb.PengadaanId == id
                                   select new VWKandidatPengadaan
                                   {
                                       Id = bb.Id,
                                       PengadaanId = bb.PengadaanId,
                                       VendorId = bb.VendorId,
                                       Nama = cc.Nama
                                   }).ToList();
            oVWPengadaan.KandidatPengadaans.AddRange(historykandidat);

            /*****/

            return oVWPengadaan;
        }

        public ViewPengadaan PersetujuanTahapan(Guid id, Guid UserID)
        {
            ViewPengadaan VWPengadaan = (from b in ctx.Pengadaans
                                         where b.Id == id
                                         select new ViewPengadaan
                                         {
                                             isKlarifikasiLanjutan = (from bb in ctx.LewatTahapans
                                                                      where bb.PengadaanId == b.Id && bb.Status == EStatusPengadaan.KLARIFIKASILANJUTAN
                                                                      select bb).Count() > 0 ? 1 : 0,
                                             isMasukKlarifikasi = (from bb in ctx.LewatTahapans
                                                                   where bb.PengadaanId == b.Id && bb.Status == EStatusPengadaan.KLARIFIKASI
                                                                   select bb).Count() > 0 ? 1 : 0
                                         }).FirstOrDefault();

            return VWPengadaan;
        }

        public ViewPengadaan GetPengadaanForRekanan(Guid id, Guid UserId)
        {
            int isMasukKlarifikasi = 0;
            Vendor oVendor = ctx.Vendors.Where(d => d.Owner == UserId).FirstOrDefault();
            ViewPengadaan VWPengadaan = (from b in ctx.Pengadaans
                                         where b.Id == id
                                         select new ViewPengadaan
                                         {
                                             Id = b.Id,
                                             Judul = b.Judul,
                                             AturanBerkas = b.AturanBerkas,
                                             AturanPenawaran = b.AturanPenawaran,
                                             AturanPengadaan = b.AturanPengadaan,
                                             Keterangan = b.Keterangan,
                                             Status = b.Status,
                                             StatusName = b.Status.ToString(),
                                             GroupPengadaan = b.GroupPengadaan,
                                             JenisPekerjaan = b.JenisPekerjaan,
                                             JenisPembelanjaan = b.JenisPembelanjaan,
                                             MataUang = b.MataUang,
                                             PeriodeAnggaran = b.PeriodeAnggaran,
                                             Region = b.Region,
                                             Provinsi = b.Provinsi,
                                             KualifikasiRekan = b.KualifikasiRekan,
                                             UnitKerjaPemohon = b.UnitKerjaPemohon,
                                             JadwalPengadaans = (from bb in ctx.JadwalPengadaans
                                                                 where bb.PengadaanId == b.Id
                                                                 select new VWJadwalPengadaan
                                                                 {
                                                                     Id = bb.Id,
                                                                     PengadaanId = bb.PengadaanId,
                                                                     Mulai = bb.Mulai,
                                                                     Sampai = bb.Sampai,
                                                                     tipe = bb.tipe
                                                                 }).ToList(),
                                             KualifikasiKandidats = (from bb in ctx.KualifikasiKandidats
                                                                     where bb.PengadaanId == b.Id
                                                                     select new VWKualifikasiKandidat
                                                                     {
                                                                         Id = bb.Id,
                                                                         PengadaanId = bb.PengadaanId,
                                                                         kualifikasi = bb.kualifikasi
                                                                     }).ToList(),
                                             isMasukKlarifikasi = (from bb in ctx.PelaksanaanPemilihanKandidats
                                                                   where bb.PengadaanId == b.Id
                                                                   & bb.VendorId == oVendor.Id
                                                                   select bb).FirstOrDefault() == null ? 0 : 1,
                                             cekisMasukKlarifikasiLanjutan = (from bb in ctx.PemenangPengadaans
                                                                              where bb.PengadaanId == b.Id
                                                                              & bb.VendorId == oVendor.Id
                                                                              select bb).FirstOrDefault() == null ? 0 : 1,
                                             isKlarifikasiLanjutan = (from bb in ctx.LewatTahapans
                                                                      where bb.PengadaanId == b.Id && bb.Status == EStatusPengadaan.KLARIFIKASILANJUTAN
                                                                      select bb).Count() > 0 ? 1 : 0,
                                             isPenilaian = (from bb in ctx.LewatTahapans
                                                            where bb.PengadaanId == b.Id && bb.Status == EStatusPengadaan.PENILAIAN
                                                            select bb).Count() > 0 ? 1 : 0

                                         }).FirstOrDefault();
            if (VWPengadaan.Status == EStatusPengadaan.DISETUJUI)
            {
                if (cekStateDiSetujui(VWPengadaan.Id) == 1)
                {
                    VWPengadaan.Status = EStatusPengadaan.AANWIJZING;
                }

            }
            //if (VWPengadaan.Status == EStatusPengadaan.SUBMITPENAWARAN)
            //{
            //    if (cekStateSubmitPenawaran(VWPengadaan.Id) == 1)
            //    {
            //        VWPengadaan.Status = EStatusPengadaan.BUKAAMPLOP;
            //    }
            //}

            return VWPengadaan;
        }

        public DataPagePengadaan GetPengadaans(string search, int start, int limit, Guid? UserId, List<string> Roles, EGroupPengadaan groupstatus, List<Guid> lstMenejer, List<Guid> lstHead)
        {
            // Guid manajer =new Guid( ConfigurationManager.AppSettings["manajer"].ToString());
            search = search == null ? "" : search;
            DataPagePengadaan dataPagePengadaan = new DataPagePengadaan();
            if (limit > 0)
            {
                var VWPengadaans = (from b in ctx.Pengadaans
                                    join c in ctx.PersonilPengadaans on b.Id equals c.PengadaanId into ps
                                    from c in ps.DefaultIfEmpty()
                                    where b.GroupPengadaan == groupstatus &&
                                    //(c.PersonilId == UserId
                                    //|| c.tipe == "tim" || c.tipe == "pic" || (c.tipe=="tim"||c.tipe=="pic")
                                    (lstMenejer.Contains(UserId.Value) || (c.PersonilId == UserId) || b.CreatedBy == UserId || lstHead.Contains(UserId.Value) || c.Pengadaan.PersetujuanTerkait.Where(d => d.UserId == UserId).Count() > 0) //UserId == manajer)
                                    && (b.Judul.Contains(search) || b.NoPengadaan.Contains(search))
                                    group b by new
                                    {
                                        b.Id,
                                        b.Judul,
                                        b.AturanBerkas,
                                        b.AturanPenawaran,
                                        b.AturanPengadaan,
                                        b.Keterangan,
                                        b.Status,
                                        b.GroupPengadaan,
                                        b.TitleDokumenNotaInternal,
                                        b.TitleDokumenLain,
                                        b.TitleBerkasRujukanLain,
                                        b.CreatedBy,
                                        b.CreatedOn,
                                        b.NoPengadaan

                                    } into h
                                    select new ViewPengadaan
                                    {
                                        Id = h.Key.Id,
                                        Judul = h.Key.Judul,
                                        TitleDokumenNotaInternal = h.Key.TitleDokumenNotaInternal,
                                        TitleDokumenLain = h.Key.TitleDokumenLain,
                                        TitleBerkasRujukanLain = h.Key.TitleBerkasRujukanLain,
                                        isCreated = UserId == h.Key.CreatedBy ? 1 : 0,
                                        isPIC = (from xx in ctx.PersonilPengadaans
                                                 where xx.PengadaanId == h.Key.Id && xx.PersonilId == UserId && xx.tipe == "pic"
                                                 select new { xx.Id }).FirstOrDefault() != null ? 1 : 0,
                                        isTEAM = (from bb in ctx.PersonilPengadaans
                                                  where bb.PengadaanId == h.Key.Id && (bb.tipe == "tim" || bb.tipe == "pic") && bb.PersonilId == UserId
                                                  select bb).Count() > 0 ? 1 : 0,
                                        PersonilPengadaans = (from b in ctx.PersonilPengadaans
                                                              where b.PengadaanId == h.Key.Id
                                                              select new VWPersonilPengadaan
                                                              {
                                                                  Id = b.Id,
                                                                  Jabatan = b.Jabatan,
                                                                  Nama = b.Nama,
                                                                  PersonilId = b.PersonilId,
                                                                  tipe = b.tipe
                                                              }).ToList(),
                                        KandidatPengadaans = (from b in ctx.KandidatPengadaans
                                                              join c in ctx.Vendors on b.VendorId equals c.Id
                                                              where b.PengadaanId == h.Key.Id
                                                              select new VWKandidatPengadaan
                                                              {
                                                                  Id = b.Id,
                                                                  PengadaanId = b.PengadaanId,
                                                                  VendorId = b.VendorId,
                                                                  Nama = c.Nama
                                                              }).ToList(),
                                        JadwalPengadaans = (from b in ctx.JadwalPengadaans
                                                            where b.PengadaanId == h.Key.Id
                                                            select new VWJadwalPengadaan
                                                            {
                                                                Id = b.Id,
                                                                PengadaanId = b.PengadaanId,
                                                                Mulai = b.Mulai,
                                                                Sampai = b.Sampai,
                                                                tipe = b.tipe
                                                            }).ToList(),
                                        DokumenPengadaans = (from b in ctx.DokumenPengadaans
                                                             where b.PengadaanId == h.Key.Id
                                                             select new VWDokumenPengadaan
                                                             {
                                                                 Id = b.Id,
                                                                 PengadaanId = b.PengadaanId,
                                                                 ContentType = b.ContentType,
                                                                 File = b.File,
                                                                 Title = b.Title,
                                                                 Tipe = b.Tipe
                                                             }).ToList(),
                                        Keterangan = h.Key.Keterangan,
                                        Status = h.Key.Status,
                                        //StatusBintang = ctx.BintangPengadaans.Where(x => x.PengadaanId == h.Key.Id && x.UserId == UserId).FirstOrDefault() == null ? 0 : ctx.BintangPengadaans.Where(x => x.PengadaanId == h.Key.Id && x.UserId == UserId).FirstOrDefault().StatusBintang,
                                        AturanPengadaan = h.Key.AturanPengadaan,
                                        AturanBerkas = h.Key.AturanBerkas,
                                        AturanPenawaran = h.Key.AturanPenawaran,
                                        GroupPengadaan = h.Key.GroupPengadaan,
                                        CreatedOn = h.Key.CreatedOn,
                                        NoPengadaan = h.Key.NoPengadaan
                                    }).OrderByDescending(x => x.CreatedOn);//.Skip(start).Take(limit).ToList();

                dataPagePengadaan.TotalRecord = VWPengadaans.Count();
                dataPagePengadaan.data = VWPengadaans.Skip(start).Take(limit).ToList();
                return dataPagePengadaan;
            }
            return new DataPagePengadaan();
        }

        public List<ViewPengadaan> GetPerhatian(string search, int start, int limit, Guid? UserId, List<string> Roles, int approver, List<Guid> lstManager)
        {
            search = search == null ? "" : search;
            if (limit > 0)
            {
                List<ViewPengadaan> VWPengadaans = (from b in ctx.Pengadaans
                                                    join c in ctx.PersonilPengadaans on b.Id equals c.PengadaanId into ps
                                                    from c in ps.DefaultIfEmpty()
                                                    where ((b.Status == EStatusPengadaan.AJUKAN) && c.PersonilId == UserId) ||
                                                            (b.Status == EStatusPengadaan.DITOLAK && c.PersonilId == UserId) ||
                                                                (lstManager.Contains(UserId.Value) && b.Status == EStatusPengadaan.AJUKAN)
                                                                && (b.Judul.Contains(search) || b.NoPengadaan.Contains(search))
                                                    group b by new
                                                    {
                                                        b.Id,
                                                        b.Judul,
                                                        b.AturanBerkas,
                                                        b.AturanPenawaran,
                                                        b.AturanPengadaan,
                                                        b.Keterangan,
                                                        b.Status,
                                                        b.GroupPengadaan,
                                                        b.TitleDokumenNotaInternal,
                                                        b.TitleDokumenLain,
                                                        b.TitleBerkasRujukanLain,
                                                        b.CreatedBy,
                                                        b.CreatedOn,
                                                        b.NoPengadaan

                                                    } into h
                                                    select new ViewPengadaan
                                                    {
                                                        Id = h.Key.Id,
                                                        Judul = h.Key.Judul,
                                                        TitleDokumenNotaInternal = h.Key.TitleDokumenNotaInternal,
                                                        TitleDokumenLain = h.Key.TitleDokumenLain,
                                                        TitleBerkasRujukanLain = h.Key.TitleBerkasRujukanLain,
                                                        isCreated = UserId == h.Key.CreatedBy ? 1 : 0,
                                                        Approver = approver,
                                                        // Approver=(from bb in ctx.Workflows where bb.DocumentId==h.Key.Id select bb).Count()>0?0:
                                                        //         (from bb in ctx.Workflows where bb.DocumentId==h.Key.Id select bb).FirstOrDefault().NextUserId==UserId?1:0,
                                                        isPIC = (from xx in ctx.PersonilPengadaans
                                                                 where xx.PengadaanId == h.Key.Id && xx.PersonilId == UserId && xx.tipe == "pic"
                                                                 select new { xx.Id }).FirstOrDefault() != null ? 1 : 0,
                                                        isTEAM = (from bb in ctx.PersonilPengadaans
                                                                  where bb.PengadaanId == h.Key.Id && (bb.tipe == "tim" || bb.tipe == "pic") && bb.PersonilId == UserId
                                                                  select bb).Count() > 0 ? 1 : 0,
                                                        PersonilPengadaans = (from b in ctx.PersonilPengadaans
                                                                              where b.PengadaanId == h.Key.Id
                                                                              select new VWPersonilPengadaan
                                                                              {
                                                                                  Id = b.Id,
                                                                                  Jabatan = b.Jabatan,
                                                                                  Nama = b.Nama,
                                                                                  PersonilId = b.PersonilId,
                                                                                  tipe = b.tipe
                                                                              }).ToList(),
                                                        KandidatPengadaans = (from b in ctx.KandidatPengadaans
                                                                              join c in ctx.Vendors on b.VendorId equals c.Id
                                                                              where b.PengadaanId == h.Key.Id
                                                                              select new VWKandidatPengadaan
                                                                              {
                                                                                  Id = b.Id,
                                                                                  PengadaanId = b.PengadaanId,
                                                                                  VendorId = b.VendorId,
                                                                                  Nama = c.Nama
                                                                              }).ToList(),
                                                        JadwalPengadaans = (from b in ctx.JadwalPengadaans
                                                                            where b.PengadaanId == h.Key.Id
                                                                            select new VWJadwalPengadaan
                                                                            {
                                                                                Id = b.Id,
                                                                                PengadaanId = b.PengadaanId,
                                                                                Mulai = b.Mulai,
                                                                                Sampai = b.Sampai,
                                                                                tipe = b.tipe
                                                                            }).ToList(),
                                                        DokumenPengadaans = (from b in ctx.DokumenPengadaans
                                                                             where b.PengadaanId == h.Key.Id
                                                                             select new VWDokumenPengadaan
                                                                             {
                                                                                 Id = b.Id,
                                                                                 PengadaanId = b.PengadaanId,
                                                                                 ContentType = b.ContentType,
                                                                                 File = b.File,
                                                                                 Title = b.Title,
                                                                                 Tipe = b.Tipe
                                                                             }).ToList(),
                                                        Keterangan = h.Key.Keterangan,
                                                        Status = h.Key.Status,
                                                        //StatusBintang = ctx.BintangPengadaans.Where(x => x.PengadaanId == h.Key.Id && x.UserId == UserId).FirstOrDefault() == null ? 0 : ctx.BintangPengadaans.Where(x => x.PengadaanId == h.Key.Id && x.UserId == UserId).FirstOrDefault().StatusBintang,
                                                        AturanPengadaan = h.Key.AturanPengadaan,
                                                        AturanBerkas = h.Key.AturanBerkas,
                                                        AturanPenawaran = h.Key.AturanPenawaran,
                                                        GroupPengadaan = h.Key.GroupPengadaan,
                                                        CreatedOn = h.Key.CreatedOn,
                                                        NoPengadaan = h.Key.NoPengadaan
                                                    }).OrderByDescending(x => x.CreatedOn).ToList();
                return VWPengadaans;
            }

            return new List<ViewPengadaan>();
        }

        public List<ViewPengadaan> GetPerhatianWorkflow(string search, int start, int limit, Guid? UserId)
        {
            search = search == null ? "" : search;
            if (limit > 0)
            {
                List<ViewPengadaan> VWPengadaans = (from b in ctx.Pengadaans
                                                    join c in ctx.PersonilPengadaans on b.Id equals c.PengadaanId into ps
                                                    from c in ps.DefaultIfEmpty()
                                                    where ((b.Status == EStatusPengadaan.AJUKAN) && c.PersonilId == UserId) ||
                                                            (b.Status == EStatusPengadaan.DITOLAK && c.PersonilId == UserId) ||
                                                                (b.Status == EStatusPengadaan.AJUKAN)//lstDocumentId.Contains(b.Id) &&
                                                                && (b.Judul.Contains(search) || b.NoPengadaan.Contains(search))
                                                    group b by new
                                                    {
                                                        b.Id,
                                                        b.Judul,
                                                        b.AturanBerkas,
                                                        b.AturanPenawaran,
                                                        b.AturanPengadaan,
                                                        b.Keterangan,
                                                        b.Status,
                                                        b.GroupPengadaan,
                                                        b.TitleDokumenNotaInternal,
                                                        b.TitleDokumenLain,
                                                        b.TitleBerkasRujukanLain,
                                                        b.CreatedBy,
                                                        b.CreatedOn,
                                                        b.NoPengadaan,
                                                        b.WorkflowId

                                                    } into h
                                                    select new ViewPengadaan
                                                    {
                                                        Id = h.Key.Id,
                                                        Judul = h.Key.Judul,
                                                        TitleDokumenNotaInternal = h.Key.TitleDokumenNotaInternal,
                                                        TitleDokumenLain = h.Key.TitleDokumenLain,
                                                        TitleBerkasRujukanLain = h.Key.TitleBerkasRujukanLain,
                                                        WorkflowTemplateId = h.Key.WorkflowId,
                                                        // Approver=(from bb in lstDocument where bb.CurrentUserId==UserId && bb.DocumentId==h.Key.Id select bb).Count()>0?1:0,
                                                        isCreated = UserId == h.Key.CreatedBy ? 1 : 0,
                                                        isPIC = (from xx in ctx.PersonilPengadaans
                                                                 where xx.PengadaanId == h.Key.Id && xx.PersonilId == UserId && xx.tipe == "pic"
                                                                 select new { xx.Id }).FirstOrDefault() != null ? 1 : 0,
                                                        isTEAM = (from bb in ctx.PersonilPengadaans
                                                                  where bb.PengadaanId == h.Key.Id && (bb.tipe == "tim" || bb.tipe == "pic") && bb.PersonilId == UserId
                                                                  select bb).Count() > 0 ? 1 : 0,
                                                        PersonilPengadaans = (from b in ctx.PersonilPengadaans
                                                                              where b.PengadaanId == h.Key.Id
                                                                              select new VWPersonilPengadaan
                                                                              {
                                                                                  Id = b.Id,
                                                                                  Jabatan = b.Jabatan,
                                                                                  Nama = b.Nama,
                                                                                  PersonilId = b.PersonilId,
                                                                                  tipe = b.tipe
                                                                              }).ToList(),
                                                        KandidatPengadaans = (from b in ctx.KandidatPengadaans
                                                                              join c in ctx.Vendors on b.VendorId equals c.Id
                                                                              where b.PengadaanId == h.Key.Id
                                                                              select new VWKandidatPengadaan
                                                                              {
                                                                                  Id = b.Id,
                                                                                  PengadaanId = b.PengadaanId,
                                                                                  VendorId = b.VendorId,
                                                                                  Nama = c.Nama
                                                                              }).ToList(),
                                                        JadwalPengadaans = (from b in ctx.JadwalPengadaans
                                                                            where b.PengadaanId == h.Key.Id
                                                                            select new VWJadwalPengadaan
                                                                            {
                                                                                Id = b.Id,
                                                                                PengadaanId = b.PengadaanId,
                                                                                Mulai = b.Mulai,
                                                                                Sampai = b.Sampai,
                                                                                tipe = b.tipe
                                                                            }).ToList(),
                                                        DokumenPengadaans = (from b in ctx.DokumenPengadaans
                                                                             where b.PengadaanId == h.Key.Id
                                                                             select new VWDokumenPengadaan
                                                                             {
                                                                                 Id = b.Id,
                                                                                 PengadaanId = b.PengadaanId,
                                                                                 ContentType = b.ContentType,
                                                                                 File = b.File,
                                                                                 Title = b.Title,
                                                                                 Tipe = b.Tipe
                                                                             }).ToList(),
                                                        Keterangan = h.Key.Keterangan,
                                                        Status = h.Key.Status,
                                                        //StatusBintang = ctx.BintangPengadaans.Where(x => x.PengadaanId == h.Key.Id && x.UserId == UserId).FirstOrDefault() == null ? 0 : ctx.BintangPengadaans.Where(x => x.PengadaanId == h.Key.Id && x.UserId == UserId).FirstOrDefault().StatusBintang,
                                                        AturanPengadaan = h.Key.AturanPengadaan,
                                                        AturanBerkas = h.Key.AturanBerkas,
                                                        AturanPenawaran = h.Key.AturanPenawaran,
                                                        GroupPengadaan = h.Key.GroupPengadaan,
                                                        CreatedOn = h.Key.CreatedOn,
                                                        NoPengadaan = h.Key.NoPengadaan,
                                                    }).OrderByDescending(x => x.CreatedOn).ToList();
                //foreach (var item in VWPengadaans)
                //{
                //    var cekApprover = lstDocument.Where(d => d.DocumentId == item.Id && d.CurrentUserId == UserId&&d.WorkflowMasterTemplateId==item.WorkflowTemplateId).Count();

                //    if (cekApprover > 0) item.Approver = 1;
                //    else item.Approver = 0;
                //}

                return VWPengadaans;
            }

            return new List<ViewPengadaan>();
        }

        public DataTablePengadaan List(string search, int start, int limit, EStatusPengadaan status, int more, int spk, List<Guid> userAprrover, Guid userId)
        {//
            DataTablePengadaan oData = new DataTablePengadaan();
            var dt = ctx.Pengadaans.Where(d => d.Judul.Contains(search) && d.Status == status);
            if (more == 1 && status == EStatusPengadaan.DISETUJUI)
            {
                dt = ctx.Pengadaans.Where(d => d.Judul.Contains(search)
                    && d.Status >= status && d.Status != EStatusPengadaan.ARSIP
                    && d.Status != EStatusPengadaan.DITOLAK && d.Status != EStatusPengadaan.DIBATALKAN
                    && (d.PersonilPengadaans.Where(dd => dd.PersonilId == userId).Count() > 0 || userAprrover.Contains(userId) || d.PersetujuanTerkait.Where(dd => dd.UserId == userId).Count() > 0));
            }
            if (spk == 1 && status == EStatusPengadaan.PEMENANG)
            {
                dt = ctx.Pengadaans.Where(d => d.Judul.Contains(search)
                    && d.Status == status
                    && d.DokumenPengadaans.Where(dd => dd.Tipe == TipeBerkas.SuratPerintahKerja && dd.PengadaanId == d.Id).Count() > 0
                    && d.PersetujuanPemenangs.Count() > 0
                    && (d.PersonilPengadaans.Where(dd => dd.PersonilId == userId).Count() > 0 || userAprrover.Contains(userId) || d.PersetujuanTerkait.Where(dd => dd.UserId == userId).Count() > 0));
            }

            if (spk == 0 && status == EStatusPengadaan.PEMENANG)
            {
                dt = ctx.Pengadaans.Where(d => d.Judul.Contains(search) && d.Status == status
                    && d.PersetujuanPemenangs.Where(dd => dd.Status == StatusPengajuanPemenang.PENDING).Count() > 0
                    && (d.PersonilPengadaans.Where(dd => dd.PersonilId == userId).Count() > 0 || userAprrover.Contains(userId) || d.PersetujuanTerkait.Where(dd => dd.UserId == userId).Count() > 0));

            }
            if (more == 1 && status == EStatusPengadaan.PEMENANG)
            {
                dt = ctx.Pengadaans.Where(d => d.Judul.Contains(search) && d.Status == status
                    && d.PersetujuanPemenangs.Where(dd => dd.Status == StatusPengajuanPemenang.BELUMDIAJUKAN).Count() > 0
                    && (d.PersonilPengadaans.Where(dd => dd.PersonilId == userId).Count() > 0 || d.PersetujuanTerkait.Where(dd => dd.UserId == userId).Count() > 0));

            }
            if (more == 1 && status == EStatusPengadaan.DRAFT)
            {
                dt = ctx.Pengadaans.Where(d => d.Judul.Contains(search) && d.Status == status
                    && (d.PersonilPengadaans.Where(dd => dd.PersonilId == userId).Count() > 0));

            }
            oData.recordsFiltered = dt.Count();
            oData.recordsTotal = ctx.Pengadaans.Count();
            oData.data = dt.OrderByDescending(d => d.CreatedOn).Skip(start).Take(limit).Select(d => new ViewPengadaan
            {
                Judul = d.Judul,
                WorkflowTemplateId = d.WorkflowId,
                Region = d.Region,
                Status = d.Status,
                StatusPersetujuanPemenang = d.PersetujuanPemenangs.FirstOrDefault() == null ? StatusPengajuanPemenang.BELUMDIAJUKAN : d.PersetujuanPemenangs.FirstOrDefault().Status,
                StatusPersetujuanPemenangName = d.PersetujuanPemenangs.FirstOrDefault() == null ? StatusPengajuanPemenang.BELUMDIAJUKAN.ToString() : d.PersetujuanPemenangs.FirstOrDefault().Status.ToString(),
                StatusName = d.Status.ToString(),
                IdPersetujuanPemanang = d.PersetujuanPemenangs.FirstOrDefault() == null ? Guid.Empty : d.PersetujuanPemenangs.FirstOrDefault().Id,
                WorkflowPersetujuanPemenangTemplateId = d.PersetujuanPemenangs.FirstOrDefault() == null ? null : d.PersetujuanPemenangs.FirstOrDefault().WorkflowId,
                JenisPekerjaan = d.JenisPekerjaan,
                AturanPengadaan = d.AturanPengadaan,
                Id = d.Id,
                JadwalPengadaans = d.JadwalPengadaans.Select(dd => new VWJadwalPengadaan { Mulai = dd.Mulai, Sampai = dd.Sampai, tipe = dd.tipe }).ToList(),
                JadwalPelaksanaans = d.JadwalPelaksanaans.Select(dd => new VWJadwalPelaksanaan2 { Mulai = dd.Mulai, Sampai = dd.Sampai, statusPengadaan = dd.statusPengadaan }).ToList(),
                KandidatPengadaans = d.KandidatPengadaans.Select(dd => new VWKandidatPengadaan { Nama = dd.Vendor.Nama, Telepon = dd.Vendor.Telepon }).ToList(),
                HPS = ctx.RKSHeaders.Where(dd => dd.PengadaanId == d.Id).FirstOrDefault() == null ? 0 : ctx.RKSHeaders.Where(dd => dd.PengadaanId == d.Id).FirstOrDefault().RKSDetails.Sum(dx => dx.hps * dx.jumlah == null ? 0 : dx.hps * dx.jumlah),
                vendor = d.PemenangPengadaans.Select(dd => new VWVendor() { Id = dd.Vendor.Id, Nama = dd.Vendor.Nama }).ToList(),
                //Pemenang = dt.FirstOrDefault().PemenangPengadaans.FirstOrDefault().Vendor.Nama,
                lstPemenang = d.PemenangPengadaans.Select(dd => dd.Vendor.Nama).ToList(),
                PersonilPengadaans = d.PersonilPengadaans.Select(dd => new VWPersonilPengadaan { PersonilId = dd.PersonilId, Nama = dd.Nama, tipe = dd.tipe, Jabatan = dd.Jabatan }).ToList(),
                //asuransicek = ctx.InsuranceTarifs.Where(aa => aa.PengadaanId == d.Id).FirstOrDefault()
                cekisMasukKlarifikasiLanjutan = ctx.JadwalPelaksanaans.Where(a => a.PengadaanId == d.Id && a.statusPengadaan == EStatusPengadaan.KLARIFIKASILANJUTAN).FirstOrDefault() == null ? 0 : 1,
                BudgetAllocated = ctx.BudgetingPengadaanHeaders.Where(x => x.PengadaanId.Equals(d.Id)).Distinct().FirstOrDefault(),
                //BudgetDetails = ctx.BudgetingPengadaanDetails.Where(x => x.BudgetingPengadaanHeader.PengadaanId.Equals(d.Id)).ToList()
            }).ToList();
            //  return oData;

            try
            {
                foreach (var item in oData.data)
                {
                    var cekKlarifikasiLanjutx = cekKlarifikasiLanjut(item.Id);
                    var hargaVendors = "";
                    foreach (var itemx in item.vendor)
                    {
                        if (!cekKlarifikasiLanjutx)
                        {
                            var hargaPenawaranVendor = (from bb in ctx.HargaKlarifikasiRekanans
                                                        join cc in ctx.RKSDetails on bb.RKSDetailId equals cc.Id
                                                        join dd in ctx.RKSHeaders on cc.RKSHeaderId equals dd.Id
                                                        where dd.PengadaanId == item.Id && bb.VendorId == itemx.Id
                                                        select new item
                                                        {
                                                            harga = bb.harga,
                                                            jumlah = cc.jumlah
                                                        }).Sum(xx => xx.harga == null ? 0 : xx.harga * xx.jumlah);
                            string harga = hargaPenawaranVendor == null ? "-" : hargaPenawaranVendor.Value.ToString("C", MyConverter.formatCurrencyIndoTanpaSymbol());
                            hargaVendors += itemx.Nama + "( " + harga + " )";
                        }
                        else
                        {
                            var hargaPenawaranVendor = (from bb in ctx.HargaKlarifikasiLanLanjutans
                                                        join cc in ctx.RKSDetails on bb.RKSDetailId equals cc.Id
                                                        join dd in ctx.RKSHeaders on cc.RKSHeaderId equals dd.Id
                                                        where dd.PengadaanId == item.Id && bb.VendorId == itemx.Id
                                                        select new item
                                                        {
                                                            harga = bb.harga,
                                                            jumlah = cc.jumlah
                                                        }).Sum(xx => xx.harga == null ? 0 : xx.harga * xx.jumlah);
                            string harga = hargaPenawaranVendor == null ? "-" : hargaPenawaranVendor.Value.ToString("C", MyConverter.formatCurrencyIndoTanpaSymbol());
                            hargaVendors += itemx.Nama + "( " + harga + " )";
                        }
                        //var hargaPenawranVendor = (from bb in ctx.HargaKlarifikasiRekanans
                        //                           join cc in ctx.RKSDetails on bb.RKSDetailId equals cc.Id
                        //                           join dd in ctx.RKSHeaders on cc.RKSHeaderId equals dd.Id
                        //                           where dd.PengadaanId == item.Id && bb.VendorId == itemx.Id
                        //                           select new item
                        //                           {
                        //                               harga = bb.harga,
                        //                               jumlah = cc.jumlah
                        //                           }).Sum(xx => xx.harga * xx.jumlah);

                    }
                    item.HargaPemanang = hargaVendors;

                    if (item.BudgetAllocated != null)
                    {
                        // list of allocated COAS
                        var budgetPengadaanDetails = ctx.BudgetingPengadaanDetails.Where(x => x.BudgetingPengadaanId.Equals(item.BudgetAllocated.Id));
                        List<string> allocatedCoas = budgetPengadaanDetails.Select(x => x.NoCOA).Distinct().ToList();
                        // list of months
                        //List<string> months = budgetPengadaanDetails.Select(x => x.Month).Distinct().ToList();
                        // list of departments
                        List<string> depts = budgetPengadaanDetails.Select(x => x.Department).Distinct().ToList();
                        // list of branch
                        List<string> branches = budgetPengadaanDetails.Select(x => x.Branch).Distinct().ToList();
                        // year
                        List<string> years = budgetPengadaanDetails.Select(x => x.Year).Distinct().ToList();
                        // budget type (Jenis)
                        List<string> budgetType = budgetPengadaanDetails.Select(x => x.BudgetType).Distinct().ToList();
                        // versions
                        var budgetstring = string.Join("", budgetType);
                        var branchtstring = string.Join("", branches);
                        var deptstring = string.Join("", depts);
                        //var monthstring = string.Join("", months);
                        var yearstring = string.Join("", years);
                        var oldVersion = (from a in ctx.Budgetings
                                          where a.Year.Equals(yearstring)
                                                //&& a.Month.Equals(monthstring)
                                                && a.Department.Equals(deptstring)
                                                && a.Branch.Equals(branchtstring)
                                                && a.Jenis.Equals(budgetstring)
                                          select new VWBudgeting
                                          {
                                              Version = a.Version
                                          }).Distinct().OrderByDescending(x => x.Version).FirstOrDefault();
                        int? ver = 0;
                        if (oldVersion != null) { ver = oldVersion.Version; }

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
                                  where branches.Contains(a.Branch)
                                         && depts.Contains(a.Department)
                                         && years.Contains(a.Year)
                                         && budgetType.Contains(a.BudgetType)
                                         && allocatedCoas.Contains(a.NoCOA)
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
                        var lookupBudgets = data.Where(x => allocatedCoas.Contains(x.COA) &&
                                                                    //months.Contains(x.Month) &&
                                                                    depts.Contains(x.Department) &&
                                                                    branches.Contains(x.Branch) &&
                                                                    years.Contains(x.Year) &&
                                                                    budgetType.Contains(x.Jenis) &&
                                                                    ver == x.Version).ToList();
                        Decimal? totalBudgetLeft = new Decimal(0);
                        Decimal? totalBudgetOnProcess = new Decimal(0);
                        lookupBudgets.ForEach(x => totalBudgetLeft += x.BudgetLeft.GetValueOrDefault());
                        dataBOP.ForEach(x => totalBudgetOnProcess += x.BudgetReserved.GetValueOrDefault());

                        item.BudgetLeft = totalBudgetLeft;
                        item.BudgetOnProcess = totalBudgetOnProcess;
                        item.SisaBudgetEproc = totalBudgetLeft - totalBudgetOnProcess - item.BudgetAllocated.TotalInput;
                        item.BudgetDetails = budgetPengadaanDetails.Select(x => x.Input).ToList();
                    }

                }
            }
            catch { }

            return oData;
        }

        public VWCountListDokumen ListCount(Guid userId, List<Guid> userAprrover, int PKSPerpanjang)
        {
            return new VWCountListDokumen()
            {
                PengadaanDiSetujui = ctx.Pengadaans.Where(d => d.Status >= EStatusPengadaan.DISETUJUI
                    && d.Status != EStatusPengadaan.ARSIP && d.Status != EStatusPengadaan.DITOLAK
                    && d.Status != EStatusPengadaan.DIBATALKAN
                    && (d.PersonilPengadaans.Where(dd => dd.PersonilId == userId).Count() > 0 || userAprrover.Contains(userId) || d.PersetujuanTerkait.Where(dd => dd.UserId == userId).Count() > 0)).Count(),
                PengadaanDiTolak = ctx.Pengadaans.Where(d => d.Status == EStatusPengadaan.DITOLAK
                    && (d.PersonilPengadaans.Where(dd => dd.PersonilId == userId).Count() > 0 || userAprrover.Contains(userId) || d.PersetujuanTerkait.Where(dd => dd.UserId == userId).Count() > 0)).Count(),
                PengadaanButuhPerSetujuan = ctx.Pengadaans.Where(d => d.Status == EStatusPengadaan.AJUKAN
                && (d.PersonilPengadaans.Where(dd => dd.PersonilId == userId).Count() > 0 || userAprrover.Contains(userId))).Count(),
                PemenangButuhPerSetujuan = ctx.Pengadaans.Where(d => d.Status == EStatusPengadaan.PEMENANG
                    && d.PersetujuanPemenangs.Where(dd => dd.Status == StatusPengajuanPemenang.PENDING
                    && (d.PersonilPengadaans.Where(ddx => ddx.PersonilId == userId).Count() > 0 || userAprrover.Contains(userId) || d.PersetujuanTerkait.Where(ddx => ddx.UserId == userId).Count() > 0)).Count() > 0).Count(),
                PemenangDiSetujui = ctx.Pengadaans.Where(d => d.Status == EStatusPengadaan.PEMENANG
                    && d.DokumenPengadaans.Where(dd => dd.Tipe == TipeBerkas.SuratPerintahKerja
                        && dd.PengadaanId == d.Id).Count() > 0 && d.PersetujuanPemenangs.Count() > 0
                        && (d.PersonilPengadaans.Where(dd => dd.PersonilId == userId).Count() > 0 || userAprrover.Contains(userId) || d.PersetujuanTerkait.Where(dd => dd.UserId == userId).Count() > 0)).Count(),
                MonitorSelection = ctx.RencanaProyeks.Where(d => d.Status == "dijalankan").Count(),
                PersetujuanTerkait = ctx.Pengadaans.Where(d => d.Status == EStatusPengadaan.PEMENANG
                    && d.PersetujuanPemenangs.Where(dd => dd.Status == StatusPengajuanPemenang.BELUMDIAJUKAN
                    && (d.PersonilPengadaans.Where(ddx => ddx.PersonilId == userId).Count() > 0 || d.PersetujuanTerkait.Where(ddx => ddx.UserId == userId).Count() > 0)).Count() > 0).Count(),
                TotalSeluruhPersetujuan = ctx.Pengadaans.Where(d => d.Status == EStatusPengadaan.AJUKAN
                    && (d.PersonilPengadaans.Where(dd => dd.PersonilId == userId).Count() > 0 || userAprrover.Contains(userId) || d.PersetujuanTerkait.Where(dd => dd.UserId == userId).Count() > 0)).Count() + ctx.Pengadaans.Where(d => d.Status == EStatusPengadaan.PEMENANG
                        && d.PersetujuanPemenangs.Where(dd => dd.Status == StatusPengajuanPemenang.PENDING).Count() > 0
                        && (d.PersonilPengadaans.Where(dd => dd.PersonilId == userId).Count() > 0 || userAprrover.Contains(userId) || d.PersetujuanTerkait.Where(dd => dd.UserId == userId).Count() > 0)).Count() + ctx.Pengadaans.Where(d => d.Status == EStatusPengadaan.PEMENANG
                    && d.PersetujuanPemenangs.Where(dd => dd.Status == StatusPengajuanPemenang.BELUMDIAJUKAN
                    //  && (d.PersonilPengadaans.Where(ddx => ddx.PersonilId == userId).Count() > 0 || d.PersetujuanTerkait.Where(ddx => ddx.UserId == userId).Count() > 0)).Count() > 0).Count(),
                    //PengadaanBelumTerjadwal = ctx.Pengadaans.Where(d => d.GroupPengadaan == EGroupPengadaan.BELUMTERJADWAL && (d.PersonilPengadaans.Where(dd => dd.PersonilId == userId).Count() > 0)).Count()
                    && (d.PersonilPengadaans.Where(ddx => ddx.PersonilId == userId).Count() > 0 || d.PersetujuanTerkait.Where(ddx => ddx.UserId == userId).Count() > 0)).Count() > 0).Count() +
                    ctx.RegVendors.Where(d => d.StatusAkhir == EStatusVendor.NEW || d.StatusAkhir == EStatusVendor.PASS_1 || d.StatusAkhir == EStatusVendor.PASS_2 || d.StatusAkhir == EStatusVendor.PASS_3).Count(),
                PengadaanBelumTerjadwal = ctx.Pengadaans.Where(d => d.GroupPengadaan == EGroupPengadaan.BELUMTERJADWAL && (d.PersonilPengadaans.Where(dd => dd.PersonilId == userId).Count() > 0)).Count(),
                RequestPendaftaranVendor = ctx.RegVendors.Where(d => d.StatusAkhir == EStatusVendor.NEW || d.StatusAkhir == EStatusVendor.PASS_1 || d.StatusAkhir == EStatusVendor.PASS_2 || d.StatusAkhir == EStatusVendor.PASS_3).Count(),
                RequestPerubahanDataVendor = ctx.Vendors.Where(d => d.StatusAkhir == EStatusVendor.UPDATED).Count(),
                PerpanjanganPKS = PKSPerpanjang,
                PenilaianVendor = (from a in ctx.ApprisalWorksheetResponses
                                   join b in ctx.ApprisalWorksheetResposeDetails on a.Id equals b.ApprisalWorksheetResposeId into ps
                                   from p in ps.DefaultIfEmpty()
                                   where p.Id == null && a.AppriserUserId == userId
                                   select new VWApprisalWorksheetResposeDetail
                                   {
                                       QuestionCode = String.IsNullOrEmpty(p.QuestionCode) ? "" : p.QuestionCode
                                   }).ToList().Count(),
                EMemoNeedAttentionCount = (from eMemoWfDetail in ctx.ApprovalWorkflowDetails
                                           join eMemoWfHeader in ctx.ApprovalWorkflows on eMemoWfDetail.ApprovalWorkflowId equals eMemoWfHeader.Id
                                           where eMemoWfDetail.ParticipantUserId == userId
                                                && eMemoWfDetail.ApprovalDecission == null
                                                && eMemoWfHeader.WorkflowState != Eproc.Model.EMemo.ApprovalWorkflowState.CANCELLED
                                           select eMemoWfDetail)
                                           .Count(),
                ENotaNeedAttentionCount = (from eNotaWfDetail in ctx.ApprovalWorkflowDetailsNota
                                           join eNotaWfHeader in ctx.ApprovalWorkflowsNota on eNotaWfDetail.ApprovalWorkflowId equals eNotaWfHeader.Id
                                           where eNotaWfDetail.ParticipantUserId == userId
                                                && eNotaWfDetail.ApprovalDecission == null
                                                && eNotaWfHeader.WorkflowState != Eproc.Model.ENota.ApprovalWorkflowStateNota.CANCELLED
                                           select eNotaWfDetail)
                                           .Count()
            };
        }

        public List<ViewPengadaan> GetPengadaansForRekanan(int start, int limit, Guid? UserId, List<string> Roles, EGroupPengadaan groupstatus)
        {
            //Guid manajer = new Guid(ConfigurationManager.AppSettings["manajer"].ToString());
            if (limit > 0)
            {
                Vendor oVendor = ctx.Vendors.Where(xx => xx.Owner == UserId).FirstOrDefault();
                if (oVendor == null) return new List<ViewPengadaan>();
                List<ViewPengadaan> VWPengadaans = (from b in ctx.Pengadaans
                                                    join c in ctx.KandidatPengadaans on b.Id equals c.PengadaanId into ps
                                                    from c in ps.DefaultIfEmpty()
                                                    where (groupstatus == EGroupPengadaan.ALL || b.GroupPengadaan == groupstatus) && c.VendorId == oVendor.Id
                                                    //contoh vendor
                                                    group b by new
                                                    {
                                                        b.Id,
                                                        b.Judul,
                                                        b.AturanBerkas,
                                                        b.AturanPenawaran,
                                                        b.AturanPengadaan,
                                                        b.Keterangan,
                                                        b.Status,
                                                        b.GroupPengadaan,
                                                        b.TitleDokumenNotaInternal,
                                                        b.TitleDokumenLain,
                                                        b.TitleBerkasRujukanLain,
                                                        b.Region,
                                                        b.Provinsi,
                                                        b.CreatedOn,
                                                        b.NoPengadaan
                                                    } into h
                                                    select new ViewPengadaan
                                                    {
                                                        Id = h.Key.Id,
                                                        Judul = h.Key.Judul,
                                                        Region = h.Key.Region,
                                                        Provinsi = h.Key.Provinsi,
                                                        NoPengadaan = h.Key.NoPengadaan,
                                                        PersonilPengadaans = (from b in ctx.PersonilPengadaans
                                                                              where b.PengadaanId == h.Key.Id
                                                                              select new VWPersonilPengadaan
                                                                              {
                                                                                  Id = b.Id,
                                                                                  Jabatan = b.Jabatan,
                                                                                  Nama = b.Nama,
                                                                                  PersonilId = b.PersonilId,
                                                                                  tipe = b.tipe
                                                                              }).ToList(),
                                                        KandidatPengadaans = (from b in ctx.KandidatPengadaans
                                                                              join c in ctx.Vendors on b.VendorId equals c.Id
                                                                              where b.PengadaanId == h.Key.Id
                                                                              select new VWKandidatPengadaan
                                                                              {
                                                                                  Id = b.Id,
                                                                                  PengadaanId = b.PengadaanId,
                                                                                  VendorId = b.VendorId,
                                                                                  Nama = c.Nama
                                                                              }).ToList(),
                                                        JadwalPengadaans = (from b in ctx.JadwalPengadaans
                                                                            where b.PengadaanId == h.Key.Id
                                                                            select new VWJadwalPengadaan
                                                                            {
                                                                                Id = b.Id,
                                                                                PengadaanId = b.PengadaanId,
                                                                                Mulai = b.Mulai,
                                                                                Sampai = b.Sampai,
                                                                                tipe = b.tipe
                                                                            }).ToList(),
                                                        DokumenPengadaans = (from b in ctx.DokumenPengadaans
                                                                             where b.PengadaanId == h.Key.Id
                                                                             select new VWDokumenPengadaan
                                                                             {
                                                                                 Id = b.Id,
                                                                                 PengadaanId = b.PengadaanId,
                                                                                 ContentType = b.ContentType,
                                                                                 File = b.File,
                                                                                 Title = b.Title,
                                                                                 Tipe = b.Tipe
                                                                             }).ToList(),
                                                        Keterangan = h.Key.Keterangan,
                                                        Status = h.Key.Status,
                                                        //StatusBintang = ctx.BintangPengadaans.Where(x => x.PengadaanId == h.Key.Id && x.UserId == UserId).FirstOrDefault() == null ? 0 : ctx.BintangPengadaans.Where(x => x.PengadaanId == h.Key.Id && x.UserId == UserId).FirstOrDefault().StatusBintang,
                                                        AturanPengadaan = h.Key.AturanPengadaan,
                                                        AturanBerkas = h.Key.AturanBerkas,
                                                        AturanPenawaran = h.Key.AturanPenawaran,
                                                        GroupPengadaan = h.Key.GroupPengadaan,
                                                        CreatedOn = h.Key.CreatedOn
                                                    }).OrderByDescending(x => x.CreatedOn).Skip(start).Take(limit).ToList();
                return VWPengadaans;
            }
            return new List<ViewPengadaan>();
        }

        public List<Pengadaan> GetAllPengadaan()
        {
            return ctx.Pengadaans.ToList();
        }

        public int DeletePengadaan(Guid Id, Guid UserId)
        {

            Pengadaan Mpengadaan = ctx.Pengadaans.Find(Id);

            if (Mpengadaan != null)
            {
                int ispic = (from xx in ctx.PersonilPengadaans
                             where xx.PengadaanId == Id && xx.tipe == "pic" && xx.PersonilId == UserId
                             select xx).Count() > 0 ? 1 : 0;
                if ((Mpengadaan.Status == EStatusPengadaan.DRAFT || Mpengadaan.Status == EStatusPengadaan.DITOLAK) && (Mpengadaan.CreatedBy == UserId || ispic == 1))
                {
                    if (Mpengadaan.CreatedBy != UserId && ctx.PersonilPengadaans.Where(d => d.tipe == "pic" && d.PersonilId == UserId && d.PengadaanId == Mpengadaan.Id) == null) return 0;
                    ctx.DokumenPengadaans.RemoveRange(Mpengadaan.DokumenPengadaans);
                    ctx.JadwalPengadaans.RemoveRange(Mpengadaan.JadwalPengadaans);
                    ctx.KualifikasiKandidats.RemoveRange(ctx.KualifikasiKandidats.Where(d => d.PengadaanId == Mpengadaan.Id));
                    ctx.KandidatPengadaans.RemoveRange(Mpengadaan.KandidatPengadaans);
                    ctx.PersonilPengadaans.RemoveRange(Mpengadaan.PersonilPengadaans);
                    var oPembobotanPengadaan = ctx.PembobotanPengadaans.Where(d => d.PengadaanId == Id);
                    ctx.PembobotanPengadaans.RemoveRange(oPembobotanPengadaan);
                    //ctx.SaveChanges();
                    List<RKSHeader> mrksHeader = ctx.RKSHeaders.Where(d => d.PengadaanId == Mpengadaan.Id).ToList();
                    List<RKSHeader> lstRksheader = new List<RKSHeader>();
                    List<RKSDetail> lstRKSDetail = new List<RKSDetail>();
                    if (mrksHeader.Count() > 0)
                    {
                        foreach (var item in mrksHeader)
                        {
                            lstRksheader.Add(item);
                            //var oRksDetails = ctx.RKSDetails.Where(d => d.RKSHeaderId == item.Id);
                            lstRKSDetail.AddRange(item.RKSDetails);
                            //ctx.RKSDetails.RemoveRange(oRksDetails);                               
                            //ctx.RKSHeaders.Remove(item);
                        }
                    }
                    ctx.RKSDetails.RemoveRange(lstRKSDetail);
                    ctx.RKSHeaders.RemoveRange(lstRksheader);
                    var oMessage = ctx.MessagePengadaans.Where(d => d.PengadaanId == Mpengadaan.Id);
                    ctx.MessagePengadaans.RemoveRange(oMessage);
                    ctx.Pengadaans.Remove(Mpengadaan);
                    ctx.SaveChanges();
                    return 1;
                }
            }
            return 0;

        }

        public Pengadaan Persetujuan(Guid? UserId, Guid id, int approver)
        {
            //Guid manajer = new Guid(ConfigurationManager.AppSettings["manajer"].ToString());
            // if (UserId == manajer)
            if (approver == 1)
            {
                Pengadaan pengadaan = ctx.Pengadaans.Find(id);
                if (pengadaan != null)
                {
                    string noPengadaan = GenerateNoPengadaan(UserId.Value);

                    pengadaan.Status = EStatusPengadaan.DISETUJUI;
                    pengadaan.GroupPengadaan = EGroupPengadaan.DALAMPELAKSANAAN;
                    if (string.IsNullOrEmpty(pengadaan.NoPengadaan))
                        pengadaan.NoPengadaan = noPengadaan;
                    pengadaan.TanggalMenyetujui = DateTime.Now;
                    ctx.MessagePengadaans.RemoveRange(ctx.MessagePengadaans.Where(d => d.PengadaanId == id));
                    ctx.SaveChanges();
                    MessagePengadaan mMessagePengadaan = new MessagePengadaan();
                    mMessagePengadaan.PengadaanId = pengadaan.Id;
                    mMessagePengadaan.Message = "Pengadaan Disetujui";
                    mMessagePengadaan.Status = EStatusPengadaan.DISETUJUI;
                    mMessagePengadaan.UserTo = pengadaan.CreatedBy;
                    mMessagePengadaan.FromTo = UserId;
                    mMessagePengadaan.Waktu = DateTime.Now;

                    //JadwalPelaksanaan mJadwalPelaksanaan = new JadwalPelaksanaan();
                    //mJadwalPelaksanaan.PengadaanId = pengadaan.Id;
                    //mJadwalPelaksanaan.statusPengadaan = EStatusPengadaan.AANWIJZING;
                    //mJadwalPelaksanaan.Mulai = ctx.JadwalPengadaans.Where(d => d.PengadaanId == pengadaan.Id && d.tipe == PengadaanConstants.Jadwal.Aanwijzing)
                    //                        .FirstOrDefault().Mulai;
                    //ctx.JadwalPelaksanaans.Add(mJadwalPelaksanaan);
                    ctx.SaveChanges();
                }
                return pengadaan;
            }
            return new Pengadaan();
        }

        public Pengadaan Penolakan(Guid? UserId, VWPenolakan vwPenolakan, int approver)
        {
            //Guid manajer = new Guid(ConfigurationManager.AppSettings["manajer"].ToString());
            //if (UserId == manajer)
            if (approver == 1)
            {
                Pengadaan pengadaan = ctx.Pengadaans.Find(vwPenolakan.PenolakanId);
                if (pengadaan != null)
                {
                    pengadaan.Status = EStatusPengadaan.DITOLAK;
                    pengadaan.GroupPengadaan = EGroupPengadaan.BELUMTERJADWAL;
                    MessagePengadaan oMessagePengadaan = ctx.MessagePengadaans.Where(d => d.PengadaanId == vwPenolakan.PenolakanId).FirstOrDefault();

                    ctx.MessagePengadaans.RemoveRange(ctx.MessagePengadaans.Where(d => d.PengadaanId == vwPenolakan.PenolakanId));
                    ctx.SaveChanges();
                    MessagePengadaan mMessagePengadaan = new MessagePengadaan();
                    mMessagePengadaan.PengadaanId = pengadaan.Id;
                    mMessagePengadaan.Message = "Pengadaan ini DiTolak";
                    mMessagePengadaan.Status = EStatusPengadaan.DITOLAK;
                    mMessagePengadaan.UserTo = oMessagePengadaan.FromTo;
                    mMessagePengadaan.FromTo = UserId;
                    mMessagePengadaan.Waktu = DateTime.Now;
                    ctx.MessagePengadaans.Add(mMessagePengadaan);
                    ctx.SaveChanges();

                    PenolakanPengadaan mPenolakanPengadaan = ctx.PenolakanPengadaans.Where(d => d.PengadaanId == vwPenolakan.PenolakanId && d.status == 1).FirstOrDefault();
                    if (mPenolakanPengadaan != null)
                        mPenolakanPengadaan.status = 0;
                    else
                    {
                        mPenolakanPengadaan = new PenolakanPengadaan();
                        mPenolakanPengadaan.PengadaanId = vwPenolakan.PenolakanId;
                        mPenolakanPengadaan.Keterangan = vwPenolakan.AlasanPenolakan;
                        mPenolakanPengadaan.CreateBy = UserId.Value;
                        mPenolakanPengadaan.CreateOn = DateTime.Now;
                        mPenolakanPengadaan.status = 1;
                        ctx.PenolakanPengadaans.Add(mPenolakanPengadaan);
                    }
                    ctx.SaveChanges();
                }
                return pengadaan;
            }
            return new Pengadaan();
        }

        public ResultMessage TolakPengadaan(Guid Id)
        {
            ResultMessage result = new ResultMessage();
            Pengadaan pengadaan = ctx.Pengadaans.Find(Id);
            pengadaan.Status = EStatusPengadaan.DITOLAK;
            pengadaan.GroupPengadaan = EGroupPengadaan.BELUMTERJADWAL;
            try
            {
                ctx.SaveChanges();
                result.message = Common.UpdateSukses();
                result.status = System.Net.HttpStatusCode.OK;
                result.Id = pengadaan.Id.ToString();
                return result;
            }
            catch (Exception ex)
            {
                result.message = ex.ToString();
                result.status = System.Net.HttpStatusCode.InternalServerError;
                return result;
            }

        }

        public ResultMessage AddPenilaian(VWTenderScoringDetails vwtenderscoringdetail)
        {
            ResultMessage result = new ResultMessage();
            try
            {
                var tsdid = ctx.TenderScoringDetails.OrderByDescending(x => x.Id).FirstOrDefault().Id;
                TenderScoringUser tenderScoringUser = new TenderScoringUser();
                foreach (var a in vwtenderscoringdetail.TenderScoringUser)
                {
                    tenderScoringUser.TenderScoringDetailId = tsdid;
                    tenderScoringUser.Score = Convert.ToInt32(a.Score);
                    ctx.TenderScoringUsers.Add(tenderScoringUser);
                    ctx.SaveChanges();
                }

                result.message = Common.UpdateSukses();
                result.status = System.Net.HttpStatusCode.OK;
                result.Id = tenderScoringUser.TenderScoringDetailId.ToString();
                return result;
            }
            catch (Exception ex)
            {
                result.message = ex.ToString();
                result.status = System.Net.HttpStatusCode.InternalServerError;
                return result;
            }

        }

        public ResultMessage AddPertanyaan(VWTenderScoringHeader vwtenderscoringheader)
        {
            ResultMessage result = new ResultMessage();
            try
            {
                TenderScoringHeader tenderScoringHeader = new TenderScoringHeader();
                foreach (var i in vwtenderscoringheader.VendorId)
                {
                    tenderScoringHeader.PengadaanId = vwtenderscoringheader.PengadaanId;
                    tenderScoringHeader.VendorId = Convert.ToInt32(i.Id);
                    tenderScoringHeader.Total = Convert.ToInt32(vwtenderscoringheader.Total);
                    tenderScoringHeader.Average = Convert.ToDecimal(vwtenderscoringheader.Averages);
                    ctx.TenderScoringHeaders.Add(tenderScoringHeader);
                    ctx.SaveChanges();
                }

                TenderScoringBobot tsb = new TenderScoringBobot();
                foreach (var i in vwtenderscoringheader.TenderScoringBobot)
                {
                    tsb.PengadaanId = vwtenderscoringheader.PengadaanId;
                    tsb.Code = i.Code;
                    tsb.Bobot = i.Bobot;
                    ctx.TenderScoringBobots.Add(tsb);
                    ctx.SaveChanges();
                }

                var tshid = ctx.TenderScoringHeaders.Where(x => x.PengadaanId == vwtenderscoringheader.PengadaanId).ToList();
                TenderScoringDetail tenderScoringDetail = new TenderScoringDetail();
                foreach (var i in tshid)
                {
                    foreach (var a in vwtenderscoringheader.TenderScoringDetails)
                    {
                        tenderScoringDetail.TenderScoringHeaderId = i.Id;
                        tenderScoringDetail.Code = a.Code;
                        tenderScoringDetail.TotalAllUser = Convert.ToInt32(a.Total_All_User);
                        tenderScoringDetail.AverageAllUser = Convert.ToDecimal(a.Averages_All_User);
                        ctx.TenderScoringDetails.Add(tenderScoringDetail);
                        ctx.SaveChanges();
                    }
                }

                result.message = Common.UpdateSukses();
                result.status = System.Net.HttpStatusCode.OK;
                result.Id = tenderScoringHeader.PengadaanId.ToString();
                return result;
            }
            catch (Exception ex)
            {
                result.message = ex.ToString();
                result.status = System.Net.HttpStatusCode.InternalServerError;
                return result;
            }

        }

        public ResultMessage AddAssessment(VWTenderScoringDetails vwtenderscoringdetail, Guid Id, int VendorId, decimal Total, decimal Average, Guid UserId)
        {
            ResultMessage result = new ResultMessage();
            try
            {
                TenderScoringUser tenderScoringUser = new TenderScoringUser();
                foreach (var i in vwtenderscoringdetail.TenderScoringUser)
                {
                    tenderScoringUser.TenderScoringDetailId = i.VWTenderScoringDetailId;
                    tenderScoringUser.UserId = UserId;
                    tenderScoringUser.Score = i.Score;
                    ctx.TenderScoringUsers.Add(tenderScoringUser);
                    ctx.SaveChanges();
                }

                var countuser = (from b in ctx.TenderScoringUsers
                                 join c in ctx.TenderScoringDetails on b.TenderScoringDetailId equals c.Id
                                 join d in ctx.TenderScoringHeaders on c.TenderScoringHeaderId equals d.Id
                                 where d.PengadaanId == Id
                                 select b.UserId).Distinct().Count();

                var tsh = ctx.TenderScoringHeaders.Where(x => x.PengadaanId == Id && x.VendorId == VendorId).FirstOrDefault();
                var tshid = tsh.Id;
                var total_tsh = tsh.Total;
                var average_tsh = tsh.Average;
                tsh.Total = Total + total_tsh;
                tsh.Average = Convert.ToDecimal(tsh.Total) / Convert.ToDecimal(countuser);
                ctx.SaveChanges();

                //var tsd = ctx.TenderScoringDetails.Where(x => x.TenderScoringHeaderId == tshid).ToList();
                TenderScoringDetail tenderScoringDetail = new TenderScoringDetail();
                foreach (var a in vwtenderscoringdetail.TenderScoringUser)
                {
                    var tsd = ctx.TenderScoringDetails.Where(x => x.TenderScoringHeaderId == tshid && x.Id == a.VWTenderScoringDetailId).ToList();

                    ////perhitungan nilai bobot
                    //foreach (var i in vwtenderscoringdetail.TenderScoringUser)
                    //{

                    //}

                    foreach (var i in tsd)
                    {
                        var x = a.Bobot;
                        decimal y = 100;
                        var z = x / y;
                        i.TotalAllUser = i.TotalAllUser + (a.Score * z);
                        i.AverageAllUser = Convert.ToDecimal(i.TotalAllUser) / Convert.ToDecimal(countuser);
                        ctx.SaveChanges();
                    }
                }

                result.message = Common.UpdateSukses();
                result.status = System.Net.HttpStatusCode.OK;
                //result.Id = tenderScoringHeader.PengadaanId.ToString();
                return result;
            }
            catch (Exception ex)
            {
                result.message = ex.ToString();
                result.status = System.Net.HttpStatusCode.InternalServerError;
                return result;
            }

        }

        public void Save()
        {
            ctx.SaveChanges();
        }

        public Pengadaan AddPengadaan(Pengadaan pengadaan, Guid UserId, List<Guid> manager)
        {
            // Guid manajer =new Guid( ConfigurationManager.AppSettings["manajer"].ToString());
            if (pengadaan.Status == EStatusPengadaan.DRAFT || pengadaan.Status == EStatusPengadaan.AJUKAN)
            {
                Pengadaan Mpengadaan = ctx.Pengadaans.Find(pengadaan.Id);
                if (Mpengadaan != null)
                {
                    int isteam = (from bb in ctx.PersonilPengadaans
                                  where bb.PengadaanId == Mpengadaan.Id && (bb.tipe == "tim" || bb.tipe == "pic") && bb.PersonilId == UserId
                                  select bb).Count() > 0 ? 1 : 0;
                    int isCreated = Mpengadaan.CreatedBy == UserId ? 1 : 0;
                    if (isteam == 0 && isCreated == 0) return pengadaan;
                    if (Mpengadaan.Status == EStatusPengadaan.AJUKAN)
                    {
                        return pengadaan;
                    }
                    Mpengadaan.Judul = pengadaan.Judul;
                    Mpengadaan.Keterangan = pengadaan.Keterangan;
                    Mpengadaan.AturanBerkas = pengadaan.AturanBerkas;
                    Mpengadaan.AturanPenawaran = pengadaan.AturanPenawaran;
                    Mpengadaan.AturanPengadaan = pengadaan.AturanPengadaan;
                    Mpengadaan.GroupPengadaan = EGroupPengadaan.BELUMTERJADWAL;
                    Mpengadaan.JenisPekerjaan = pengadaan.JenisPekerjaan;
                    Mpengadaan.JenisPembelanjaan = pengadaan.JenisPembelanjaan;
                    Mpengadaan.KualifikasiRekan = pengadaan.KualifikasiRekan;
                    Mpengadaan.MataUang = pengadaan.MataUang;
                    Mpengadaan.PeriodeAnggaran = pengadaan.PeriodeAnggaran;
                    Mpengadaan.Provinsi = pengadaan.Provinsi;
                    Mpengadaan.Region = pengadaan.Region;
                    Mpengadaan.UnitKerjaPemohon = pengadaan.UnitKerjaPemohon;
                    Mpengadaan.TitleDokumenNotaInternal = pengadaan.TitleDokumenNotaInternal;
                    Mpengadaan.TitleDokumenLain = pengadaan.TitleDokumenLain;
                    Mpengadaan.TitleBerkasRujukanLain = pengadaan.TitleBerkasRujukanLain;
                    Mpengadaan.ModifiedBy = UserId;
                    Mpengadaan.Pagu = pengadaan.Pagu;
                    Mpengadaan.NoCOA = pengadaan.NoCOA;
                    Mpengadaan.Status = pengadaan.Status;
                    Mpengadaan.ModifiedOn = DateTime.Now;
                    Mpengadaan.WorkflowId = pengadaan.WorkflowId;
                    Mpengadaan.PengadaanLangsung = pengadaan.PengadaanLangsung;
                    Mpengadaan.Branch = pengadaan.Branch;
                    Mpengadaan.Department = pengadaan.Department;
                    if (Mpengadaan.JadwalPengadaans != null)
                    {
                        ctx.JadwalPengadaans.RemoveRange(Mpengadaan.JadwalPengadaans);
                    }
                    foreach (var item in pengadaan.JadwalPengadaans)
                    {
                        JadwalPengadaan mJadwalPengadaan = new JadwalPengadaan();
                        mJadwalPengadaan.Mulai = item.Mulai;
                        mJadwalPengadaan.Sampai = item.Sampai;
                        mJadwalPengadaan.tipe = item.tipe;
                        mJadwalPengadaan.PengadaanId = Mpengadaan.Id;
                        Mpengadaan.JadwalPengadaans.Add(mJadwalPengadaan);
                    }
                }
                else
                {
                    pengadaan.GroupPengadaan = EGroupPengadaan.BELUMTERJADWAL;
                    pengadaan.CreatedBy = UserId;
                    pengadaan.CreatedOn = DateTime.Now;
                    ctx.Pengadaans.Add(pengadaan);
                    ctx.SaveChanges();
                    RiwayatDokumen nRiwayatDokumen = new RiwayatDokumen();
                    nRiwayatDokumen.Status = "Pembuatan Dokumen Pengadaan";
                    nRiwayatDokumen.PengadaanId = pengadaan.Id;
                    nRiwayatDokumen.UserId = UserId;
                    AddRiwayatDokumen(nRiwayatDokumen);
                }
                if (pengadaan.Status == EStatusPengadaan.AJUKAN)
                {
                    RiwayatDokumen nRiwayatDokumen = new RiwayatDokumen();
                    nRiwayatDokumen.Status = "Pengajuan Dokumen Pengadaan";
                    nRiwayatDokumen.PengadaanId = pengadaan.Id;
                    nRiwayatDokumen.UserId = UserId;
                    AddRiwayatDokumen(nRiwayatDokumen);
                }
                try
                {
                    ctx.SaveChanges();
                    //if (pengadaan.Status == EStatusPengadaan.AJUKAN)
                    //{
                    //    AjukanWorkflow(pengadaan.Id, UserId, new Guid("B6EA58A3-345D-E611-A7D2-38EAA7E56C6E"));                       
                    //}
                }
                catch (Exception dbEx)
                {
                }
            }

            return pengadaan;
        }

        public RiwayatDokumen AddRiwayatDokumen(RiwayatDokumen nRiwayatDokumen)
        {
            nRiwayatDokumen.ActionDate = DateTime.Now;
            ctx.RiwayatDokumens.Add(nRiwayatDokumen);
            ctx.SaveChanges();
            return nRiwayatDokumen;
        }

        public List<RiwayatDokumen> lstRiwayatDokumen(Guid Id)
        {
            return ctx.RiwayatDokumens.Where(d => d.PengadaanId == Id).ToList();
        }

        public RKSHeader AddTotalHps(Guid PengadaanId, decimal Total, Guid UserId)
        {
            Pengadaan Mpengadaan = ctx.Pengadaans.Find(PengadaanId);
            if (Mpengadaan == null) return new RKSHeader();
            RKSHeader oRksHeader = new RKSHeader();
            oRksHeader.CreateBy = UserId;
            oRksHeader.CreateOn = DateTime.Now;
            oRksHeader.PengadaanId = PengadaanId;
            oRksHeader.Total = Total;
            ctx.RKSHeaders.Add(oRksHeader);
            ctx.SaveChanges();
            return new RKSHeader
            {
                Id = oRksHeader.Id,
                PengadaanId = oRksHeader.PengadaanId,
                Total = oRksHeader.Total

            };
        }

        public RKSHeader GetTotalHps(Guid PengadaanId, Guid UserId)
        {
            RKSHeader oRksHeader = ctx.RKSHeaders.Where(d => d.PengadaanId == PengadaanId).FirstOrDefault();
            if (oRksHeader == null) return new RKSHeader();
            return new RKSHeader
            {
                Id = oRksHeader.Id,
                PengadaanId = oRksHeader.PengadaanId,
                Total = oRksHeader.RKSDetails.Count() > 0 ? oRksHeader.RKSDetails.Sum(d => (d.hps != null ? d.hps.Value : 0) * (d.jumlah != null ? d.jumlah.Value : 0)) : 0

            };
        }

        public List<VWRKSDetail> getRKS(Guid id)
        {
            RKSHeader rksHeader = ctx.RKSHeaders.Where(d => d.PengadaanId == id).FirstOrDefault();
            if (rksHeader != null)
            {
                var rksDetails = rksHeader.RKSDetails.OrderBy(d => d.grup).ThenBy(d => d.level).ToList();
                List<VWRKSDetail> lstVWRksDetail = new List<VWRKSDetail>();
                foreach (var item in rksDetails)
                {
                    VWRKSDetail dVWRKSDetail = new VWRKSDetail();
                    dVWRKSDetail.judul = item.judul;
                    dVWRKSDetail.level = item.level;
                    dVWRKSDetail.grup = item.grup;
                    dVWRKSDetail.item = item.item;
                    dVWRKSDetail.satuan = item.satuan;
                    dVWRKSDetail.jumlah = item.jumlah;
                    dVWRKSDetail.hps = item.hps;
                    dVWRKSDetail.keterangan = item.keterangan;
                    if (item.hps != null && item.jumlah != null)
                    {
                        dVWRKSDetail.total = item.hps.Value * item.jumlah.Value;
                    }
                    lstVWRksDetail.Add(dVWRKSDetail);
                }
                return lstVWRksDetail;
            }
            return new List<VWRKSDetail>();
        }

        public List<VWRKSDetailRekanan> getRKSForRekanan(Guid id, Guid UserId)
        {
            RKSHeader rksHeader = ctx.RKSHeaders.Where(d => d.PengadaanId == id).FirstOrDefault();
            if (rksHeader != null)
            {
                List<VWRKSDetailRekanan> VWRksDEtail = (from b in rksHeader.RKSDetails
                                                        select new VWRKSDetailRekanan
                                                        {
                                                            Id = b.Id,
                                                            item = b.item,
                                                            keteranganItem = b.keterangan,
                                                            ItemId = b.ItemId,
                                                            jumlah = b.jumlah,
                                                            RKSHeaderId = b.RKSHeaderId,
                                                            satuan = b.satuan,
                                                            level = b.level,
                                                            grup = b.grup,
                                                            judul = b.judul,
                                                            hargaEncript = ctx.HargaRekanans.Where(d => d.RKSDetailId == b.Id && d.VendorId == ctx.Vendors.Where(xx => xx.Owner == UserId).FirstOrDefault().Id).FirstOrDefault() == null ? "" : ctx.HargaRekanans.Where(d => d.RKSDetailId == b.Id && d.VendorId == ctx.Vendors.Where(xx => xx.Owner == UserId).FirstOrDefault().Id).FirstOrDefault().hargaEncrypt,
                                                            HargaRekananId = ctx.HargaRekanans.Where(d => d.RKSDetailId == b.Id && d.VendorId == ctx.Vendors.Where(xx => xx.Owner == UserId).FirstOrDefault().Id).FirstOrDefault() == null ? Guid.Empty : ctx.HargaRekanans.Where(d => d.RKSDetailId == b.Id && d.VendorId == ctx.Vendors.Where(xx => xx.Owner == UserId).FirstOrDefault().Id).FirstOrDefault().Id,
                                                            harga = ctx.HargaRekanans.Where(d => d.RKSDetailId == b.Id && d.VendorId == ctx.Vendors.Where(xx => xx.Owner == UserId).FirstOrDefault().Id).FirstOrDefault() == null ? 0 : ctx.HargaRekanans.Where(d => d.RKSDetailId == b.Id && d.VendorId == ctx.Vendors.Where(xx => xx.Owner == UserId).FirstOrDefault().Id).FirstOrDefault().harga,
                                                            keterangan = ctx.HargaRekanans.Where(d => d.RKSDetailId == b.Id && d.VendorId == ctx.Vendors.Where(xx => xx.Owner == UserId).FirstOrDefault().Id).FirstOrDefault() == null ? "" : ctx.HargaRekanans.Where(d => d.RKSDetailId == b.Id && d.VendorId == ctx.Vendors.Where(xx => xx.Owner == UserId).FirstOrDefault().Id).FirstOrDefault().keterangan
                                                        }).ToList();
                foreach (var item in VWRksDEtail)
                {
                    JimbisEncrypt encod = new JimbisEncrypt();
                    if (item.hargaEncript != "")
                    {
                        decimal? harga = Convert.ToDecimal(encod.Decrypt(item.hargaEncript));
                        item.harga = harga;
                    }
                }
                return VWRksDEtail;
            }
            return new List<VWRKSDetailRekanan>();
        }

        public List<VWRKSDetailRekanan> getRKSForKlarifikasiRekanan(Guid id, Guid UserId)
        {
            RKSHeader rksHeader = ctx.RKSHeaders.Where(d => d.PengadaanId == id).FirstOrDefault();
            JimbisEncrypt encod = new JimbisEncrypt();

            if (rksHeader != null)
            {
                List<VWRKSDetailRekanan> VWRksDEtail = (from b in rksHeader.RKSDetails
                                                        select new VWRKSDetailRekanan
                                                        {
                                                            Id = b.Id,
                                                            item = b.item,
                                                            keteranganItem = b.keterangan,
                                                            ItemId = b.ItemId,
                                                            jumlah = b.jumlah,
                                                            RKSHeaderId = b.RKSHeaderId,
                                                            satuan = b.satuan,
                                                            level = b.level,
                                                            grup = b.grup,
                                                            judul = b.judul,
                                                            HargaRekananId = ctx.HargaKlarifikasiRekanans.Where(d => d.RKSDetailId == b.Id && d.VendorId == ctx.Vendors.Where(xx => xx.Owner == UserId).FirstOrDefault().Id).FirstOrDefault() == null ? Guid.Empty : ctx.HargaKlarifikasiRekanans.Where(d => d.RKSDetailId == b.Id && d.VendorId == ctx.Vendors.Where(xx => xx.Owner == UserId).FirstOrDefault().Id).FirstOrDefault().Id,
                                                            harga = ctx.HargaKlarifikasiRekanans.Where(d => d.RKSDetailId == b.Id && d.VendorId == ctx.Vendors.Where(xx => xx.Owner == UserId).FirstOrDefault().Id).FirstOrDefault() == null ? 0 : ctx.HargaKlarifikasiRekanans.Where(d => d.RKSDetailId == b.Id && d.VendorId == ctx.Vendors.Where(xx => xx.Owner == UserId).FirstOrDefault().Id).FirstOrDefault().harga,
                                                            keterangan = ctx.HargaKlarifikasiRekanans.Where(d => d.RKSDetailId == b.Id && d.VendorId == ctx.Vendors.Where(xx => xx.Owner == UserId).FirstOrDefault().Id).FirstOrDefault() == null ? "" : ctx.HargaKlarifikasiRekanans.Where(d => d.RKSDetailId == b.Id && d.VendorId == ctx.Vendors.Where(xx => xx.Owner == UserId).FirstOrDefault().Id).FirstOrDefault().keterangan
                                                        }).ToList();

                return VWRksDEtail;
            }
            return new List<VWRKSDetailRekanan>();
        }

        public List<VWRKSDetailRekanan> getRKSForKlarifikasiLanjutanRekanan(Guid id, Guid UserId)
        {
            RKSHeader rksHeader = ctx.RKSHeaders.Where(d => d.PengadaanId == id).FirstOrDefault();
            JimbisEncrypt encod = new JimbisEncrypt();

            if (rksHeader != null)
            {
                List<VWRKSDetailRekanan> VWRksDEtail = (from b in rksHeader.RKSDetails
                                                        select new VWRKSDetailRekanan
                                                        {
                                                            Id = b.Id,
                                                            item = b.item,
                                                            keteranganItem = b.keterangan,
                                                            ItemId = b.ItemId,
                                                            jumlah = b.jumlah,
                                                            RKSHeaderId = b.RKSHeaderId,
                                                            satuan = b.satuan,
                                                            level = b.level,
                                                            grup = b.grup,
                                                            judul = b.judul,
                                                            HargaRekananId = ctx.HargaKlarifikasiLanLanjutans.Where(d => d.RKSDetailId == b.Id && d.VendorId == ctx.Vendors.Where(xx => xx.Owner == UserId).FirstOrDefault().Id).FirstOrDefault() == null ? Guid.Empty : ctx.HargaKlarifikasiLanLanjutans.Where(d => d.RKSDetailId == b.Id && d.VendorId == ctx.Vendors.Where(xx => xx.Owner == UserId).FirstOrDefault().Id).FirstOrDefault().Id,
                                                            harga = ctx.HargaKlarifikasiLanLanjutans.Where(d => d.RKSDetailId == b.Id && d.VendorId == ctx.Vendors.Where(xx => xx.Owner == UserId).FirstOrDefault().Id).FirstOrDefault() == null ? 0 : ctx.HargaKlarifikasiLanLanjutans.Where(d => d.RKSDetailId == b.Id && d.VendorId == ctx.Vendors.Where(xx => xx.Owner == UserId).FirstOrDefault().Id).FirstOrDefault().harga,
                                                            keterangan = ctx.HargaKlarifikasiLanLanjutans.Where(d => d.RKSDetailId == b.Id && d.VendorId == ctx.Vendors.Where(xx => xx.Owner == UserId).FirstOrDefault().Id).FirstOrDefault() == null ? "" : ctx.HargaKlarifikasiLanLanjutans.Where(d => d.RKSDetailId == b.Id && d.VendorId == ctx.Vendors.Where(xx => xx.Owner == UserId).FirstOrDefault().Id).FirstOrDefault().keterangan
                                                        }).ToList();

                return VWRksDEtail;
            }
            return new List<VWRKSDetailRekanan>();
        }

        public RKSHeader saveRks(RKSHeader rks, Guid UserId)
        {
            int RKSEditAfterApprove = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["RKS_CHANGE_AFTER_APROVE"]);
            Pengadaan Mpengadaan = ctx.Pengadaans.Find(rks.PengadaanId);
            if (Mpengadaan != null)
            {
                if (Mpengadaan.Status == EStatusPengadaan.DRAFT || RKSEditAfterApprove == 1)
                {
                    if (rks.Id != Guid.Empty)
                    {
                        RKSHeader MRKSHeader = ctx.RKSHeaders.Find(rks.Id);
                        if (MRKSHeader != null)
                        {
                            MRKSHeader.ModifiedBy = UserId;
                            MRKSHeader.ModifiedOn = DateTime.Now;
                            foreach (var item in rks.RKSDetails)
                            {
                                if (item.Id != Guid.Empty)
                                {
                                    RKSDetail rksdetail = ctx.RKSDetails.Find(item.Id);
                                    rksdetail.judul = item.judul;
                                    rksdetail.level = item.level;
                                    rksdetail.grup = item.grup;
                                    rksdetail.hps = item.hps;
                                    rksdetail.item = item.item;
                                    rksdetail.ItemId = item.ItemId;
                                    rksdetail.jumlah = item.jumlah;
                                    rksdetail.satuan = item.satuan;
                                    rksdetail.keterangan = item.keterangan;
                                    rksdetail.ModifiedBy = UserId;
                                    rksdetail.ModifiedOn = DateTime.Now;
                                }
                                else
                                {
                                    RKSDetail newrksdetail = new RKSDetail();
                                    newrksdetail.hps = item.hps;
                                    newrksdetail.item = item.item;
                                    newrksdetail.judul = item.judul;
                                    newrksdetail.level = item.level;
                                    newrksdetail.grup = item.grup;
                                    newrksdetail.ItemId = item.ItemId;
                                    newrksdetail.jumlah = item.jumlah;
                                    newrksdetail.satuan = item.satuan;
                                    newrksdetail.keterangan = item.keterangan;
                                    newrksdetail.RKSHeaderId = MRKSHeader.Id;
                                    newrksdetail.CreateOn = DateTime.Now;
                                    newrksdetail.CreateBy = UserId;
                                    MRKSHeader.RKSDetails.Add(newrksdetail);
                                }
                            }
                            var rksrequesIds = rks.RKSDetails.Select(d => d.Id);
                            var removeRksDetail = MRKSHeader.RKSDetails.Where(d => !rksrequesIds.Contains(d.Id));
                            ctx.RKSDetails.RemoveRange(removeRksDetail);
                        }
                        else
                        {
                            rks.CreateBy = UserId;
                            rks.CreateOn = DateTime.Now;
                            ctx.RKSHeaders.Add(rks);
                        }

                    }
                    else
                    {
                        rks.CreateBy = UserId;
                        rks.CreateOn = DateTime.Now;
                        ctx.RKSHeaders.Add(rks);
                    }
                    ctx.SaveChanges();
                }
            }
            else rks.Id = Guid.Empty;

            return rks;
        }

        public Reston.Helper.Util.ResultMessage RemoveRks(Guid PengadaanId, Guid UserId)
        {
            var odata = ctx.RKSHeaders.Where(d => d.PengadaanId == PengadaanId).ToList();
            foreach (var item in odata)
            {
                var oDetail = ctx.RKSDetails.Where(d => d.RKSHeaderId == item.Id).ToList();
                if (oDetail.Count() > 0)
                    ctx.RKSDetails.RemoveRange(oDetail);
            }
            ctx.RKSHeaders.RemoveRange(odata);
            try
            {
                ctx.SaveChanges();
                return new Reston.Helper.Util.ResultMessage()
                {
                    status = HttpStatusCode.OK,
                    message = Common.DeleteSukses()
                };
            }
            catch (Exception ex)
            {
                return new Reston.Helper.Util.ResultMessage()
                {
                    status = HttpStatusCode.ExpectationFailed,
                    message = ex.ToString()
                };
            }
        }

        public VWRKSHeaderPengadaan GetRKSHeaderPengadaan(Guid id)
        {
            //var oData = ctx.RKSHeaderTemplate.Find(Id);
            Pengadaan Mpengadaan = ctx.Pengadaans.Find(id);
            if (Mpengadaan == null) return new VWRKSHeaderPengadaan();
            VWRKSHeaderPengadaan MrksHeder = (from b in ctx.RKSHeaders
                                              join c in ctx.Pengadaans on b.PengadaanId equals c.Id
                                              where c.Id == id
                                              select new VWRKSHeaderPengadaan
                                              {
                                                  Judul = c.Judul,
                                                  Keterangan = c.Keterangan,
                                                  RKSHeaderId = b.Id,
                                                  Status = c.Status
                                              }
                                   ).FirstOrDefault();

            if (MrksHeder == null) return new VWRKSHeaderPengadaan();
            return MrksHeder;
        }

        public DokumenPengadaan GetDokumenPengadaan(Guid Id)
        {
            //return ctx.DokumenPengadaans.Where(d=>d.File.Contains(file)).FirstOrDefault();
            return ctx.DokumenPengadaans.Find(Id);
        }

        public DokumenPengadaan GetDokumenPengadaanSpk(Guid PengadaanId, int VendorId)
        {
            return ctx.DokumenPengadaans.Where(d => d.PengadaanId == PengadaanId && d.VendorId == VendorId && d.Tipe == TipeBerkas.SuratPerintahKerja).FirstOrDefault();
        }
        public List<VWDokumenPengadaan> GetListDokumenPengadaan(TipeBerkas tipe, Guid Id, Guid UserId)
        {
            List<VWDokumenPengadaan> lstVWDok = new List<VWDokumenPengadaan>();
            var oData = (from b in ctx.DokumenPengadaans
                         where b.PengadaanId == Id && b.Tipe == tipe
                         select b);
            Vendor oVendor = ctx.Vendors.Where(d => d.Owner == UserId).FirstOrDefault();
            if (oVendor != null)
            {
                lstVWDok = oData.Where(d => d.VendorId == oVendor.Id).Select(d => new VWDokumenPengadaan
                {
                    ContentType = d.ContentType,
                    Id = d.Id,
                    PengadaanId = d.PengadaanId,
                    Tipe = d.Tipe,
                    Title = d.Title,
                    File = d.File,
                    SizeFile = d.SizeFile,
                }).ToList();
                if (tipe == TipeBerkas.BerkasRujukanLain)
                {
                    lstVWDok = oData.Select(d => new VWDokumenPengadaan
                    {
                        ContentType = d.ContentType,
                        Id = d.Id,
                        PengadaanId = d.PengadaanId,
                        Tipe = d.Tipe,
                        Title = d.Title,
                        File = d.File,
                        SizeFile = d.SizeFile,
                    }).ToList();
                    /* foreach (var item in oDokumen)
                     {
                         lstVWDok.Add(item);
                     }*/

                }
                if (tipe == TipeBerkas.DOKUMENLAIN)
                {
                    lstVWDok = oData.Select(d => new VWDokumenPengadaan
                    {
                        ContentType = d.ContentType,
                        Id = d.Id,
                        PengadaanId = d.PengadaanId,
                        Tipe = d.Tipe,
                        Title = d.Title,
                        File = d.File,
                        SizeFile = d.SizeFile,
                    }).ToList();

                }
            }
            else
            {
                lstVWDok = oData.Select(d => new VWDokumenPengadaan
                {
                    ContentType = d.ContentType,
                    Id = d.Id,
                    PengadaanId = d.PengadaanId,
                    Tipe = d.Tipe,
                    Title = d.Title,
                    File = d.File,
                    SizeFile = d.SizeFile,
                }).ToList();
            }

            return lstVWDok;

        }

        public List<VWDokumenPengadaan> GetListDokumenVendor(TipeBerkas tipe, Guid Id, Guid UserId, int VendorId)
        {
            List<VWDokumenPengadaan> lstVWDok = new List<VWDokumenPengadaan>();
            var oData = (from b in ctx.DokumenPengadaans
                         where b.PengadaanId == Id && b.Tipe == tipe && b.VendorId == VendorId
                         select b);
            Vendor oVendor = ctx.Vendors.Find(VendorId);
            if (oVendor != null)
            {
                lstVWDok = oData.Where(d => d.VendorId == oVendor.Id).Select(d => new VWDokumenPengadaan
                {
                    ContentType = d.ContentType,
                    Id = d.Id,
                    PengadaanId = d.PengadaanId,
                    Tipe = d.Tipe,
                    Title = d.Title,
                    File = d.File,
                    SizeFile = d.SizeFile,
                }).ToList();
            }
            else
            {
                lstVWDok = oData.Select(d => new VWDokumenPengadaan
                {
                    ContentType = d.ContentType,
                    Id = d.Id,
                    PengadaanId = d.PengadaanId,
                    Tipe = d.Tipe,
                    Title = d.Title,
                    File = d.File,
                    SizeFile = d.SizeFile,
                }).ToList();
            }

            return lstVWDok;

        }

        public DokumenPengadaan saveDokumenPengadaan(DokumenPengadaan dokumenPengadaan, Guid UserId)
        {

            DokumenPengadaan mdokPengadaan = ctx.DokumenPengadaans.Find(dokumenPengadaan.Id);
            if (dokumenPengadaan.Tipe == TipeBerkas.BeritaAcaraPenentuanPemenang)
            {
                mdokPengadaan = ctx.DokumenPengadaans.Where(d => d.PengadaanId == dokumenPengadaan.PengadaanId && d.VendorId == dokumenPengadaan.VendorId && d.Tipe == TipeBerkas.BeritaAcaraPenentuanPemenang).FirstOrDefault();
                if (mdokPengadaan != null)
                {
                    return new DokumenPengadaan();
                }
            }
            if (dokumenPengadaan.Tipe == TipeBerkas.SuratPerintahKerja)
            {
                mdokPengadaan = ctx.DokumenPengadaans.Where(d => d.PengadaanId == dokumenPengadaan.PengadaanId && d.VendorId == dokumenPengadaan.VendorId && d.Tipe == TipeBerkas.SuratPerintahKerja).FirstOrDefault();

                if (mdokPengadaan != null)
                {
                    return new DokumenPengadaan();
                }
            }
            if (mdokPengadaan != null)
            {
                mdokPengadaan.File = dokumenPengadaan.File;
                mdokPengadaan.ModifiedOn = DateTime.Now;
                mdokPengadaan.ModifiedBy = UserId;
                Vendor oVendor = ctx.Vendors.Where(d => d.Owner == UserId).FirstOrDefault();
                if (oVendor != null) mdokPengadaan.VendorId = oVendor.Id;
            }
            else
            {
                Vendor oVendor = ctx.Vendors.Where(d => d.Owner == UserId).FirstOrDefault();
                if (oVendor != null) dokumenPengadaan.VendorId = oVendor.Id;
                try
                {
                    dokumenPengadaan.CreateOn = DateTime.Now;
                    dokumenPengadaan.CreateBy = UserId;
                }
                catch (Exception ex) { }
                ctx.DokumenPengadaans.Add(dokumenPengadaan);
            }

            var pemenang = ctx.PemenangPengadaans.Where(d => d.PengadaanId == dokumenPengadaan.PengadaanId && d.VendorId == dokumenPengadaan.VendorId).FirstOrDefault();
            if (pemenang != null && dokumenPengadaan.Tipe == TipeBerkas.SuratPerintahKerja)
            {
                var oNoberita = ctx.BeritaAcaras.Where(d => d.PengadaanId == dokumenPengadaan.PengadaanId && d.VendorId == dokumenPengadaan.VendorId).FirstOrDefault();
                if (oNoberita == null) return new DokumenPengadaan();
                var oSpk = ctx.Spk.Where(d => d.NoSPk == oNoberita.NoBeritaAcara && d.PksId == null).FirstOrDefault();//ctx.Spk.Where(d => d.PemenangPengadaanId == pemenang.Id).FirstOrDefault();
                if (oSpk == null)
                {
                    oSpk = new Spk();

                    if (oNoberita.Tipe == TipeBerkas.SuratPerintahKerja)
                    {
                        oSpk.CreateOn = DateTime.Now;
                        oSpk.CreateBy = UserId;
                        oSpk.PemenangPengadaanId = pemenang.Id;
                        oSpk.Title = "SPK Pertama Untuk Pengadaan " + pemenang.Pengadaan.Judul;
                        oSpk.StatusSpk = StatusSpk.Aktif;
                        oSpk.NoSPk = oNoberita == null ? "" : oNoberita.NoBeritaAcara;
                        if (oNoberita.tanggal != null)
                            oSpk.TanggalSPK = oNoberita.tanggal;
                        oSpk.DokumenPengadaanId = dokumenPengadaan.Id;
                        ctx.Spk.Add(oSpk);

                        var newDokSpk = new DokumenSpk();
                        newDokSpk.SpkId = oSpk.Id;
                        newDokSpk.SizeFile = dokumenPengadaan.SizeFile;
                        newDokSpk.File = dokumenPengadaan.File;
                        newDokSpk.CreateBy = UserId;
                        newDokSpk.CreateOn = DateTime.Now;
                        newDokSpk.Title = dokumenPengadaan.Title;
                        newDokSpk.ContentType = dokumenPengadaan.ContentType;
                        ctx.DokumenSpk.Add(newDokSpk);
                    }
                }
                else
                {
                    var dokSpk = ctx.DokumenSpk.Where(d => d.SpkId == oSpk.Id).FirstOrDefault();
                    if (dokSpk == null)
                    {
                        dokSpk = new DokumenSpk();
                        dokSpk.SpkId = oSpk.Id;
                        dokSpk.SizeFile = dokumenPengadaan.SizeFile;
                        dokSpk.File = dokumenPengadaan.File;
                        dokSpk.CreateBy = UserId;
                        dokSpk.CreateOn = DateTime.Now;
                        dokSpk.Title = dokumenPengadaan.Title;
                        dokSpk.ContentType = dokumenPengadaan.ContentType;
                        ctx.DokumenSpk.Add(dokSpk);
                    }
                    else
                    {
                        dokSpk.SpkId = oSpk.Id;
                        dokSpk.SizeFile = dokumenPengadaan.SizeFile;
                        dokSpk.File = dokumenPengadaan.File;
                        dokSpk.ModifiedBy = UserId;
                        dokSpk.ModifiedOn = DateTime.Now;
                        dokSpk.Title = dokumenPengadaan.Title;
                        dokSpk.ContentType = dokumenPengadaan.ContentType;
                    }
                }
            }
            ctx.SaveChanges(UserId.ToString());
            return mdokPengadaan;
        }

        public int deleteDokumen(Guid Id)
        {
            try
            {

                DokumenPengadaan MdokPengadaan = ctx.DokumenPengadaans.Find(Id);
                Pengadaan mpengadaan = ctx.Pengadaans.Find(MdokPengadaan.PengadaanId);
                if (mpengadaan == null) return 0;
                if (mpengadaan.Status != EStatusPengadaan.DRAFT) return 0;
                if (MdokPengadaan.Tipe == TipeBerkas.SuratPerintahKerja)
                {
                    var oSpk = ctx.Spk.Where(d => d.DokumenPengadaanId == MdokPengadaan.Id);
                    ctx.Spk.RemoveRange(oSpk);
                }
                ctx.DokumenPengadaans.Remove(MdokPengadaan);
                ctx.SaveChanges();

                return 1;
            }
            catch { return 0; }
        }

        public KandidatPengadaan saveKandidatPengadaan(KandidatPengadaan kandidat, Guid UserId)
        {
            try
            {
                Pengadaan mpengadaan = ctx.Pengadaans.Find(kandidat.PengadaanId);
                if (mpengadaan == null) return kandidat;
                int isTeam = ctx.PersonilPengadaans.Where(d => d.PengadaanId == mpengadaan.Id && d.PersonilId == UserId && (d.tipe == PengadaanConstants.StaffPeranan.PIC || d.tipe == PengadaanConstants.StaffPeranan.Tim)).ToList().Count() > 0 ? 1 : 0;
                if (isTeam == 0)
                {
                    if (mpengadaan.CreatedBy != UserId) return kandidat;
                }
                if (mpengadaan.Status != EStatusPengadaan.DRAFT) return kandidat;
                KandidatPengadaan mKandidatPengadaan = ctx.KandidatPengadaans.Find(kandidat.Id);
                if (mKandidatPengadaan != null)
                {
                    mKandidatPengadaan.VendorId = kandidat.VendorId;
                    mpengadaan.ModifiedBy = UserId;
                    mpengadaan.ModifiedOn = DateTime.Now;
                    ctx.SaveChanges();
                    return mKandidatPengadaan;
                }
                else
                {
                    kandidat.addKandidatType = addKandidatType.PICADDED;
                    ctx.KandidatPengadaans.Add(kandidat);
                    ctx.SaveChanges();
                    return kandidat;
                }
            }
            catch
            {
                return kandidat;
            }
        }

        public int deleteKandidatPengadaan(Guid Id, Guid UserId)
        {
            try
            {
                KandidatPengadaan MKandidatPengadaan = ctx.KandidatPengadaans.Find(Id);
                Pengadaan mpengadaan = ctx.Pengadaans.Find(MKandidatPengadaan.PengadaanId);
                if (mpengadaan == null) return 0;
                if (mpengadaan.PersonilPengadaans.Where(d => d.PersonilId == UserId && d.tipe == PengadaanConstants.StaffPeranan.PIC).FirstOrDefault() == null) return 0;
                if (mpengadaan.Status != EStatusPengadaan.DRAFT && mpengadaan.AturanPengadaan == PengadaanConstants.AturanPengadaan.Tertutup) return 0;
                if (mpengadaan.Status != EStatusPengadaan.DISETUJUI && mpengadaan.AturanPengadaan == PengadaanConstants.AturanPengadaan.Terbuka) return 0;
                ctx.KandidatPengadaans.Remove(MKandidatPengadaan);
                ctx.SaveChanges();
                return 1;
            }
            catch { return 0; }
        }

        public int deleteJadwalPengadaan(Guid Id, Guid UserId)
        {
            try
            {
                JadwalPengadaan MJadwalPengadaan = ctx.JadwalPengadaans.Find(Id);
                Pengadaan mpengadaan = ctx.Pengadaans.Find(MJadwalPengadaan.PengadaanId);
                if (mpengadaan == null) return 0;
                if (mpengadaan.Status != EStatusPengadaan.DRAFT) return 0;
                ctx.JadwalPengadaans.Remove(MJadwalPengadaan);
                ctx.SaveChanges();
                return 1;
            }
            catch { return 0; }
        }

        public JadwalPengadaan saveJadwalPengadaan(JadwalPengadaan Jadwal, Guid UserId)
        {
            try
            {
                Pengadaan mpengadaan = ctx.Pengadaans.Find(Jadwal.PengadaanId);
                if (mpengadaan == null) return Jadwal;
                if (mpengadaan.Status != EStatusPengadaan.DRAFT) return Jadwal;
                JadwalPengadaan mJadwalPengadaan = ctx.JadwalPengadaans.Find(Jadwal.Id);
                if (mJadwalPengadaan != null)
                {
                    mJadwalPengadaan.Mulai = Jadwal.Mulai;
                    mJadwalPengadaan.Sampai = Jadwal.Sampai;
                    mJadwalPengadaan.tipe = Jadwal.tipe;
                    mpengadaan.ModifiedBy = UserId;
                    mpengadaan.ModifiedOn = DateTime.Now;
                    ctx.SaveChanges();
                    return mJadwalPengadaan;
                }
                else
                {
                    mpengadaan.ModifiedBy = UserId;
                    mpengadaan.ModifiedOn = DateTime.Now;
                    ctx.JadwalPengadaans.Add(Jadwal);
                    ctx.SaveChanges();
                    return Jadwal;
                }
            }
            catch
            {
                return Jadwal;
            }
        }

        public PersonilPengadaan savePersonilPengadaan(PersonilPengadaan Personil, Guid UserId)
        {
            try
            {
                Pengadaan mpengadaan = ctx.Pengadaans.Find(Personil.PengadaanId);
                if (mpengadaan == null) return Personil;
                if (mpengadaan.Status != EStatusPengadaan.DRAFT) return Personil;
                PersonilPengadaan mPersonilPengadaan = ctx.PersonilPengadaans.Find(Personil.Id);
                if (mPersonilPengadaan != null)
                {
                    mPersonilPengadaan.Nama = Personil.Nama;
                    mPersonilPengadaan.PersonilId = Personil.PersonilId;
                    mPersonilPengadaan.Jabatan = Personil.Jabatan;
                    mPersonilPengadaan.tipe = Personil.tipe;
                    mpengadaan.ModifiedBy = UserId;
                    mpengadaan.ModifiedOn = DateTime.Now;
                    ctx.SaveChanges();
                    return mPersonilPengadaan;
                }
                else
                {
                    mpengadaan.ModifiedBy = UserId;
                    mpengadaan.ModifiedOn = DateTime.Now;
                    ctx.PersonilPengadaans.Add(Personil);
                    ctx.SaveChanges();
                    return Personil;
                }
            }
            catch
            {
                return Personil;
            }
        }

        public int deletePersonilPengadaan(Guid Id, Guid UserId)
        {
            try
            {
                PersonilPengadaan MPersonilPengadaan = ctx.PersonilPengadaans.Find(Id);
                Pengadaan mpengadaan = ctx.Pengadaans.Find(MPersonilPengadaan.PengadaanId);
                if (mpengadaan == null) return 0;
                if (mpengadaan.Status != EStatusPengadaan.DRAFT) return 0;
                ctx.PersonilPengadaans.Remove(MPersonilPengadaan);
                ctx.SaveChanges();
                return 1;
            }
            catch { return 0; }
        }

        public List<VWKandidatPengadaan> getListKandidatPengadaan(Guid PengadaanId)
        {
            var kandidats = (from b in ctx.KandidatPengadaans
                             join c in ctx.Vendors on b.VendorId equals c.Id
                             where b.PengadaanId == PengadaanId
                             select new VWKandidatPengadaan
                             {
                                 Id = b.Id,
                                 Nama = c.Nama,
                                 PengadaanId = b.PengadaanId,
                                 VendorId = b.VendorId,
                                 Telepon = c.Telepon
                             }).ToList();
            /** tambah history */
            var Historykandidats = (from b in ctx.HistoryKandidatPengadaan
                                    join c in ctx.Vendors on b.VendorId equals c.Id
                                    where b.PengadaanId == PengadaanId
                                    select new VWKandidatPengadaan
                                    {
                                        Id = b.Id,
                                        Nama = c.Nama,
                                        PengadaanId = b.PengadaanId,
                                        VendorId = b.VendorId,
                                        Telepon = c.Telepon
                                    }).ToList();
            kandidats.AddRange(Historykandidats);
            /***/

            return kandidats;
        }

        public List<JadwalPengadaan> getListJadwalPengadaan(Guid PengadaanId)
        {
            return ctx.JadwalPengadaans.Where(d => d.PengadaanId == PengadaanId).ToList();
        }

        public List<PersonilPengadaan> getListPersonilPengadaan(Guid PengadaanId)
        {
            return ctx.PersonilPengadaans.Where(d => d.PengadaanId == PengadaanId).ToList();
        }

        public JadwalPelaksanaan getPelaksanaanPendaftaran(Guid PengadaanId)
        {
            //PelaksanaanAanwijzing MpelaksanaanAanwijzing = ctx.PelaksanaanAanwijzings.Where(d => d.PengadaanId == PengadaanId).FirstOrDefault();
            JadwalPelaksanaan MjadwalPelaksanaan = ctx.JadwalPelaksanaans.Where(d => d.PengadaanId == PengadaanId && d.statusPengadaan == EStatusPengadaan.DISETUJUI).FirstOrDefault();
            if (MjadwalPelaksanaan != null)
            {
                JadwalPelaksanaan MOJadwalPelaksanaan = new JadwalPelaksanaan();
                MOJadwalPelaksanaan.Id = MjadwalPelaksanaan.Id;
                MOJadwalPelaksanaan.PengadaanId = MjadwalPelaksanaan.PengadaanId;
                MOJadwalPelaksanaan.Mulai = MjadwalPelaksanaan.Mulai;
                MOJadwalPelaksanaan.Sampai = MjadwalPelaksanaan.Sampai;
                return MOJadwalPelaksanaan;
            }
            else
            {
                JadwalPelaksanaan MOJadwalPelaksanaan = new JadwalPelaksanaan();
                JadwalPengadaan Mjadawal = ctx.JadwalPengadaans.Where(d => d.PengadaanId == PengadaanId && d.tipe == PengadaanConstants.Jadwal.Pendaftaran).FirstOrDefault();
                if (Mjadawal != null)
                {
                    MOJadwalPelaksanaan.Mulai = Mjadawal.Mulai;
                    MOJadwalPelaksanaan.Sampai = Mjadawal.Sampai;
                    MOJadwalPelaksanaan.PengadaanId = PengadaanId;
                    MOJadwalPelaksanaan.Pengadaan = null;
                }
                return MOJadwalPelaksanaan;
            }
        }

        public JadwalPelaksanaan getPelaksanaanAanwijing(Guid PengadaanId)
        {
            //PelaksanaanAanwijzing MpelaksanaanAanwijzing = ctx.PelaksanaanAanwijzings.Where(d => d.PengadaanId == PengadaanId).FirstOrDefault();
            JadwalPelaksanaan MjadwalPelaksanaan = ctx.JadwalPelaksanaans.Where(d => d.PengadaanId == PengadaanId
                            && d.statusPengadaan == EStatusPengadaan.AANWIJZING).FirstOrDefault();
            if (MjadwalPelaksanaan != null)
            {
                JadwalPelaksanaan MOJadwalPelaksanaan = new JadwalPelaksanaan();
                MOJadwalPelaksanaan.Id = MjadwalPelaksanaan.Id;
                MOJadwalPelaksanaan.PengadaanId = MjadwalPelaksanaan.PengadaanId;
                MOJadwalPelaksanaan.Mulai = MjadwalPelaksanaan.Mulai;
                return MOJadwalPelaksanaan;
            }
            else
            {
                JadwalPelaksanaan MOJadwalPelaksanaan = new JadwalPelaksanaan();
                JadwalPengadaan Mjadawal = ctx.JadwalPengadaans.Where(d => d.PengadaanId == PengadaanId && d.tipe == PengadaanConstants.Jadwal.Aanwijzing).FirstOrDefault();
                if (Mjadawal != null)
                {
                    MOJadwalPelaksanaan.Mulai = Mjadawal.Mulai;
                    MOJadwalPelaksanaan.PengadaanId = PengadaanId;
                    MOJadwalPelaksanaan.Pengadaan = null;
                }
                return MOJadwalPelaksanaan;
            }
        }

        public JadwalPelaksanaan addPelaksanaanAanwijing(JadwalPelaksanaan pelaksanaanAanwijzing, Guid UserId)
        {
            Pengadaan Mpengadaaan = ctx.Pengadaans.Find(pelaksanaanAanwijzing.PengadaanId);
            PersonilPengadaan picPersonil = ctx.PersonilPengadaans.Where(d => d.PersonilId == UserId && d.tipe == "pic" && d.PengadaanId == pelaksanaanAanwijzing.PengadaanId).FirstOrDefault();
            if (picPersonil == null) return new JadwalPelaksanaan();
            if (ctx.Pengadaans.Find(pelaksanaanAanwijzing.PengadaanId) != null)
            {
                // PelaksanaanAanwijzing MpelaksanaanAanwijzing = ctx.PelaksanaanAanwijzings.Where(d => d.PengadaanId == pelaksanaanAanwijzing.PengadaanId).FirstOrDefault();
                JadwalPelaksanaan MjadwalPelaksanaan = ctx.JadwalPelaksanaans.Where(d => d.PengadaanId == pelaksanaanAanwijzing.PengadaanId
                        && d.statusPengadaan == EStatusPengadaan.AANWIJZING).FirstOrDefault();
                if (MjadwalPelaksanaan != null)
                {
                    //MpelaksanaanAanwijzing.IsiUndangan = pelaksanaanAanwijzing.IsiUndangan;
                    MjadwalPelaksanaan.Mulai = pelaksanaanAanwijzing.Mulai;
                }
                else
                {
                    MjadwalPelaksanaan = new JadwalPelaksanaan();
                    MjadwalPelaksanaan.PengadaanId = pelaksanaanAanwijzing.PengadaanId;
                    MjadwalPelaksanaan.statusPengadaan = EStatusPengadaan.AANWIJZING;
                    //MpelaksanaanAanwijzing.IsiUndangan = pelaksanaanAanwijzing.IsiUndangan;
                    MjadwalPelaksanaan.Mulai = pelaksanaanAanwijzing.Mulai;
                    ctx.JadwalPelaksanaans.Add(MjadwalPelaksanaan);
                }
                ctx.SaveChanges();

                JadwalPelaksanaan MOJadwalPelaksanaan = new JadwalPelaksanaan();
                MOJadwalPelaksanaan.Id = MjadwalPelaksanaan.Id;
                MOJadwalPelaksanaan.PengadaanId = MjadwalPelaksanaan.PengadaanId;
                MOJadwalPelaksanaan.Mulai = MOJadwalPelaksanaan.Mulai;
                return MOJadwalPelaksanaan;
            }
            return new JadwalPelaksanaan();
        }

        public List<VWKehadiranKandidatAanwijzing> getKehadiranAanwijzings(Guid PengadaanId)
        {
            var lstKehadiranKandidat = (from b in ctx.KandidatPengadaans
                                        where b.PengadaanId == PengadaanId
                                        select new VWKehadiranKandidatAanwijzing
                                        {
                                            Id = b.Id,
                                            VendorId = b.VendorId,
                                            PengadaanId = b.PengadaanId,
                                            NamaVendor = (from bb in ctx.Vendors
                                                          where bb.Id == b.VendorId
                                                          select bb).FirstOrDefault().Nama,
                                            Telp = (from bb in ctx.Vendors
                                                    where bb.Id == b.VendorId
                                                    select bb).FirstOrDefault().Telepon,
                                            hadir = (from bb in ctx.KehadiranKandidatAanwijzings
                                                     where bb.VendorId == b.VendorId && bb.PengadaanId == b.PengadaanId
                                                     select bb).Count() > 0 ? 1 : 0
                                        }).ToList();
            /*tambah history vendor*/
            var historyKandidat = (from b in ctx.HistoryKandidatPengadaan
                                   where b.PengadaanId == PengadaanId
                                   select new VWKehadiranKandidatAanwijzing
                                   {
                                       Id = b.Id,
                                       VendorId = b.VendorId,
                                       PengadaanId = b.PengadaanId,
                                       NamaVendor = (from bb in ctx.Vendors
                                                     where bb.Id == b.VendorId
                                                     select bb).FirstOrDefault().Nama,
                                       Telp = (from bb in ctx.Vendors
                                               where bb.Id == b.VendorId
                                               select bb).FirstOrDefault().Telepon,
                                       hadir = 0
                                   }).ToList();
            /****/
            lstKehadiranKandidat.AddRange(historyKandidat);
            return lstKehadiranKandidat;
        }

        public KehadiranKandidatAanwijzing addKehadiranAanwijzing(Guid Id, Guid UserId)
        {
            KandidatPengadaan MKandidatPengadaan = ctx.KandidatPengadaans.Find(Id);
            if (MKandidatPengadaan == null) return new KehadiranKandidatAanwijzing();
            PersonilPengadaan picPersonil = ctx.PersonilPengadaans.Where(d => d.PersonilId == UserId && d.tipe == "pic" && d.PengadaanId == MKandidatPengadaan.PengadaanId).FirstOrDefault();
            if (picPersonil == null) return new KehadiranKandidatAanwijzing();
            KehadiranKandidatAanwijzing mKehadiranAazwjing = ctx.KehadiranKandidatAanwijzings
                                .Where(d => d.PengadaanId == MKandidatPengadaan.PengadaanId && d.VendorId == MKandidatPengadaan.VendorId).FirstOrDefault();
            if (mKehadiranAazwjing != null) return mKehadiranAazwjing;
            mKehadiranAazwjing = new KehadiranKandidatAanwijzing();
            mKehadiranAazwjing.PengadaanId = MKandidatPengadaan.PengadaanId;
            mKehadiranAazwjing.VendorId = MKandidatPengadaan.VendorId;
            ctx.KehadiranKandidatAanwijzings.Add(mKehadiranAazwjing);
            ctx.SaveChanges();
            return mKehadiranAazwjing;
        }

        public int DeleteKehadiranAanwijzing(Guid Id, Guid UserId)
        {
            try
            {
                KandidatPengadaan MKandidatPengadaan = ctx.KandidatPengadaans.Find(Id);
                if (MKandidatPengadaan == null) return 0;
                PersonilPengadaan picPersonil = ctx.PersonilPengadaans.Where(d => d.PersonilId == UserId && d.tipe == "pic" && d.PengadaanId == MKandidatPengadaan.PengadaanId).FirstOrDefault();
                if (picPersonil == null) return 0;
                KehadiranKandidatAanwijzing mKejadiranAazwjing = ctx.KehadiranKandidatAanwijzings
                                    .Where(d => d.PengadaanId == MKandidatPengadaan.PengadaanId && d.VendorId == MKandidatPengadaan.VendorId).FirstOrDefault();
                if (mKejadiranAazwjing == null) return 0;
                ctx.KehadiranKandidatAanwijzings.Remove(mKejadiranAazwjing);
                ctx.SaveChanges();
                return 1;
            }
            catch
            {
                return 0;
            }
        }

        public JadwalPelaksanaan AddPelaksanaanSubmitPenawaran(JadwalPelaksanaan pelaksanaanSubmitPenawaran, Guid UserId)
        {
            Pengadaan Mpengadaaan = ctx.Pengadaans.Find(pelaksanaanSubmitPenawaran.PengadaanId);
            PersonilPengadaan picPersonil = ctx.PersonilPengadaans.Where(d => d.PersonilId == UserId && d.tipe == "pic" && d.PengadaanId == pelaksanaanSubmitPenawaran.PengadaanId).FirstOrDefault();
            if (picPersonil == null) return new JadwalPelaksanaan();
            if (ctx.Pengadaans.Find(pelaksanaanSubmitPenawaran.PengadaanId) != null)
            {
                // PelaksanaanAanwijzing MpelaksanaanAanwijzing = ctx.PelaksanaanAanwijzings.Where(d => d.PengadaanId == pelaksanaanAanwijzing.PengadaanId).FirstOrDefault();
                JadwalPelaksanaan MjadwalPelaksanaan = ctx.JadwalPelaksanaans.Where(d => d.PengadaanId == pelaksanaanSubmitPenawaran.PengadaanId
                        && d.statusPengadaan == EStatusPengadaan.SUBMITPENAWARAN).FirstOrDefault();

                if (MjadwalPelaksanaan != null)
                {
                    //MpelaksanaanAanwijzing.IsiUndangan = pelaksanaanAanwijzing.IsiUndangan;
                    if (pelaksanaanSubmitPenawaran.Mulai >= DateTime.Now)
                        MjadwalPelaksanaan.Mulai = pelaksanaanSubmitPenawaran.Mulai;
                    if (pelaksanaanSubmitPenawaran.Sampai > DateTime.Now)
                        MjadwalPelaksanaan.Sampai = pelaksanaanSubmitPenawaran.Sampai;
                }
                else
                {
                    MjadwalPelaksanaan = new JadwalPelaksanaan();
                    MjadwalPelaksanaan.PengadaanId = pelaksanaanSubmitPenawaran.PengadaanId;
                    //MpelaksanaanAanwijzing.IsiUndangan = pelaksanaanAanwijzing.IsiUndangan;
                    MjadwalPelaksanaan.statusPengadaan = EStatusPengadaan.SUBMITPENAWARAN;
                    if (pelaksanaanSubmitPenawaran.Mulai >= DateTime.Now)
                        MjadwalPelaksanaan.Mulai = pelaksanaanSubmitPenawaran.Mulai;
                    if (pelaksanaanSubmitPenawaran.Sampai > DateTime.Now)
                        MjadwalPelaksanaan.Sampai = pelaksanaanSubmitPenawaran.Sampai;
                    ctx.JadwalPelaksanaans.Add(MjadwalPelaksanaan);
                }
                ctx.SaveChanges();
                return MjadwalPelaksanaan;
            }
            return new JadwalPelaksanaan();
        }

        public JadwalPelaksanaan getPelaksanaanSubmitPenawaran(Guid PengadaanId)
        {
            //PelaksanaanAanwijzing MpelaksanaanAanwijzing = ctx.PelaksanaanAanwijzings.Where(d => d.PengadaanId == PengadaanId).FirstOrDefault();
            JadwalPelaksanaan MjadwalPelaksanaan = ctx.JadwalPelaksanaans.Where(d => d.PengadaanId == PengadaanId
                            && d.statusPengadaan == EStatusPengadaan.SUBMITPENAWARAN).FirstOrDefault();
            if (MjadwalPelaksanaan != null)
            {
                JadwalPelaksanaan MOJadwalPelaksanaan = new JadwalPelaksanaan();
                MOJadwalPelaksanaan.Id = MjadwalPelaksanaan.Id;
                MOJadwalPelaksanaan.PengadaanId = MjadwalPelaksanaan.PengadaanId;
                MOJadwalPelaksanaan.Mulai = MjadwalPelaksanaan.Mulai;
                MOJadwalPelaksanaan.Sampai = MjadwalPelaksanaan.Sampai;
                return MOJadwalPelaksanaan;
            }
            else
            {
                JadwalPelaksanaan MOJadwalPelaksanaan = new JadwalPelaksanaan();
                JadwalPengadaan Mjadawal = ctx.JadwalPengadaans.Where(d => d.PengadaanId == PengadaanId && d.tipe == PengadaanConstants.Jadwal.PengisianHarga).FirstOrDefault();
                if (Mjadawal != null)
                {
                    MOJadwalPelaksanaan.Mulai = Mjadawal.Mulai;
                    MOJadwalPelaksanaan.Sampai = Mjadawal.Sampai;
                    MOJadwalPelaksanaan.PengadaanId = PengadaanId;
                    MOJadwalPelaksanaan.Pengadaan = null;
                }
                return MOJadwalPelaksanaan;
            }
            // return ctx.PelaksanaanSubmitPenawarans.Where(d => d.PengadaanId == PengadaanId).FirstOrDefault();
        }

        public JadwalPelaksanaan AddPelaksanaanBukaAmplop(JadwalPelaksanaan pelaksanaanBukaAmplop, Guid UserId)
        {
            Pengadaan Mpengadaaan = ctx.Pengadaans.Find(pelaksanaanBukaAmplop.PengadaanId);
            PersonilPengadaan picPersonil = ctx.PersonilPengadaans.Where(d => d.PersonilId == UserId && d.tipe == "pic" && d.PengadaanId == pelaksanaanBukaAmplop.PengadaanId).FirstOrDefault();
            if (picPersonil == null) return new JadwalPelaksanaan();
            if (ctx.Pengadaans.Find(pelaksanaanBukaAmplop.PengadaanId) != null)
            {
                // PelaksanaanAanwijzing MpelaksanaanAanwijzing = ctx.PelaksanaanAanwijzings.Where(d => d.PengadaanId == pelaksanaanAanwijzing.PengadaanId).FirstOrDefault();
                JadwalPelaksanaan MjadwalPelaksanaan = ctx.JadwalPelaksanaans.Where(d => d.PengadaanId == pelaksanaanBukaAmplop.PengadaanId
                        && d.statusPengadaan == EStatusPengadaan.BUKAAMPLOP).FirstOrDefault();
                if (MjadwalPelaksanaan != null)
                {
                    MjadwalPelaksanaan.Mulai = pelaksanaanBukaAmplop.Mulai;
                    MjadwalPelaksanaan.Sampai = pelaksanaanBukaAmplop.Sampai;
                }
                else
                {
                    MjadwalPelaksanaan = new JadwalPelaksanaan();
                    MjadwalPelaksanaan.PengadaanId = pelaksanaanBukaAmplop.PengadaanId;
                    MjadwalPelaksanaan.statusPengadaan = EStatusPengadaan.BUKAAMPLOP;
                    MjadwalPelaksanaan.Mulai = pelaksanaanBukaAmplop.Mulai;
                    MjadwalPelaksanaan.Sampai = pelaksanaanBukaAmplop.Sampai;
                    ctx.JadwalPelaksanaans.Add(MjadwalPelaksanaan);
                }
                ctx.SaveChanges();
                return MjadwalPelaksanaan;
            }
            return new JadwalPelaksanaan();
        }

        public JadwalPelaksanaan getPelaksanaanBukaAmplop(Guid PengadaanId)
        {
            JadwalPelaksanaan MjadwalPelaksanaan = ctx.JadwalPelaksanaans.Where(d => d.PengadaanId == PengadaanId
                           && d.statusPengadaan == EStatusPengadaan.BUKAAMPLOP).FirstOrDefault();
            if (MjadwalPelaksanaan != null)
            {
                JadwalPelaksanaan MOJadwalPelaksanaan = new JadwalPelaksanaan();
                MOJadwalPelaksanaan.Id = MjadwalPelaksanaan.Id;
                MOJadwalPelaksanaan.PengadaanId = MjadwalPelaksanaan.PengadaanId;
                MOJadwalPelaksanaan.Mulai = MjadwalPelaksanaan.Mulai;
                MOJadwalPelaksanaan.Sampai = MjadwalPelaksanaan.Sampai;
                return MOJadwalPelaksanaan;
            }
            else
            {
                JadwalPelaksanaan MOJadwalPelaksanaan = new JadwalPelaksanaan();
                JadwalPengadaan Mjadawal = ctx.JadwalPengadaans.Where(d => d.PengadaanId == PengadaanId && d.tipe == PengadaanConstants.Jadwal.BukaAmplop).FirstOrDefault();
                if (Mjadawal != null)
                {
                    MOJadwalPelaksanaan.Mulai = Mjadawal.Mulai;
                    MOJadwalPelaksanaan.Sampai = Mjadawal.Sampai;
                    MOJadwalPelaksanaan.PengadaanId = PengadaanId;
                    MOJadwalPelaksanaan.Pengadaan = null;
                }
                return MOJadwalPelaksanaan;
            }
            //return ctx.PelaksanaanBukaAmplops.Where(d => d.PengadaanId == PengadaanId).FirstOrDefault();
        }

        public PersetujuanBukaAmplop AddPersetujuanBukaAmplop(Guid PengadaanId, Guid UserId)
        {
            PersonilPengadaan picPersonil = ctx.PersonilPengadaans.Where(
                d => d.PengadaanId == PengadaanId && d.PersonilId == UserId
                    && (d.tipe == PengadaanConstants.StaffPeranan.PIC ||
                    d.tipe == PengadaanConstants.StaffPeranan.Compliance ||
                    d.tipe == PengadaanConstants.StaffPeranan.Controller ||
                    d.tipe == PengadaanConstants.StaffPeranan.Staff)).FirstOrDefault();
            if (picPersonil == null) return new PersetujuanBukaAmplop();

            PersetujuanBukaAmplop mmPersetujuan = ctx.PersetujuanBukaAmplops.Where(d => d.PengadaanId == PengadaanId
                    && d.UserId == UserId).FirstOrDefault();
            if (mmPersetujuan != null) return mmPersetujuan;
            PersetujuanBukaAmplop mPersetujuanBukaAmplop = new PersetujuanBukaAmplop();
            mPersetujuanBukaAmplop.PengadaanId = PengadaanId;
            mPersetujuanBukaAmplop.UserId = UserId;
            ctx.PersetujuanBukaAmplops.Add(mPersetujuanBukaAmplop);
            ctx.SaveChanges();
            CekBukaAmplop(PengadaanId);
            return mPersetujuanBukaAmplop;
        }

        public List<VWPErsetujuanBukaAmplop> getPersetujuanBukaAmplop(Guid PengadaanId, Guid UserId)
        {
            //Guid manajer =new Guid( ConfigurationManager.AppSettings["manajer"].ToString());
            List<VWPErsetujuanBukaAmplop> lstPErsetujuan =
                (from b in ctx.PersetujuanBukaAmplops
                 join c in ctx.PersonilPengadaans on b.PengadaanId equals c.PengadaanId //into ps
                 where b.PengadaanId == PengadaanId && c.PersonilId == b.UserId
                 select new VWPErsetujuanBukaAmplop
                 {
                     Id = b.Id,
                     tipe = c.tipe,
                     UserId = c.PersonilId,
                     PengadaanId = c.PengadaanId,
                 }).ToList();
            return lstPErsetujuan;
        }

        public JadwalPelaksanaan SaveJadwalPelaksanaan(JadwalPelaksanaan JPelaksanaan, Guid UserId)
        {
            Pengadaan Mpengadaaan = ctx.Pengadaans.Find(JPelaksanaan.PengadaanId);
            PersonilPengadaan picPersonil = ctx.PersonilPengadaans.Where(d => d.PersonilId == UserId && d.tipe == "pic" && d.PengadaanId == JPelaksanaan.PengadaanId).FirstOrDefault();
            if (picPersonil == null) return new JadwalPelaksanaan();
            if (Mpengadaaan == null) return new JadwalPelaksanaan();
            JadwalPelaksanaan MjadwalPelaksanaan = Mpengadaaan.JadwalPelaksanaans.Where(d => d.PengadaanId == JPelaksanaan.PengadaanId
                        && d.statusPengadaan == JPelaksanaan.statusPengadaan).FirstOrDefault();
            if (MjadwalPelaksanaan != null)
            {
                MjadwalPelaksanaan.Mulai = JPelaksanaan.Mulai;
                MjadwalPelaksanaan.Sampai = JPelaksanaan.Sampai;
            }
            else
            {
                MjadwalPelaksanaan = new JadwalPelaksanaan();
                MjadwalPelaksanaan.PengadaanId = JPelaksanaan.PengadaanId;
                MjadwalPelaksanaan.statusPengadaan = JPelaksanaan.statusPengadaan;
                MjadwalPelaksanaan.Mulai = JPelaksanaan.Mulai;
                MjadwalPelaksanaan.Sampai = JPelaksanaan.Sampai;
                ctx.JadwalPelaksanaans.Add(MjadwalPelaksanaan);
            }
            ctx.SaveChanges(UserId.ToString());
            return MjadwalPelaksanaan;

        }

        public JadwalPelaksanaan GetJadwalPelaksanaan(Guid PengadaanId, Guid UserId, EStatusPengadaan status)
        {
            JadwalPelaksanaan MjadwalPelaksanaan = ctx.JadwalPelaksanaans.Where(d => d.PengadaanId == PengadaanId
                           && d.statusPengadaan == status).FirstOrDefault();
            if (MjadwalPelaksanaan != null)
            {
                JadwalPelaksanaan MOJadwalPelaksanaan = new JadwalPelaksanaan();
                MOJadwalPelaksanaan.Id = MjadwalPelaksanaan.Id;
                MOJadwalPelaksanaan.PengadaanId = MjadwalPelaksanaan.PengadaanId;
                MOJadwalPelaksanaan.Mulai = MjadwalPelaksanaan.Mulai;
                MOJadwalPelaksanaan.Sampai = MjadwalPelaksanaan.Sampai;
                return MOJadwalPelaksanaan;
            }
            else
            {
                JadwalPelaksanaan MOJadwalPelaksanaan = new JadwalPelaksanaan();
                var tipe = MapingStatus(status);
                JadwalPengadaan Mjadawal = ctx.JadwalPengadaans.Where(d => d.PengadaanId == PengadaanId && d.tipe == tipe).FirstOrDefault();
                if (Mjadawal != null)
                {
                    MOJadwalPelaksanaan.Mulai = Mjadawal.Mulai;
                    MOJadwalPelaksanaan.Sampai = Mjadawal.Sampai;
                    MOJadwalPelaksanaan.PengadaanId = PengadaanId;
                    MOJadwalPelaksanaan.Pengadaan = null;
                }
                return MOJadwalPelaksanaan;
            }
        }

        private string MapingStatus(EStatusPengadaan status)
        {
            switch (status)
            {
                case EStatusPengadaan.DISETUJUI:
                    return PengadaanConstants.Jadwal.Pendaftaran;
                case EStatusPengadaan.AANWIJZING:
                    return PengadaanConstants.Jadwal.Aanwijzing;
                case EStatusPengadaan.SUBMITPENAWARAN:
                    return PengadaanConstants.Jadwal.PengisianHarga;
                case EStatusPengadaan.KLARIFIKASI:
                    return PengadaanConstants.Jadwal.Klarifikasi;
                case EStatusPengadaan.KLARIFIKASILANJUTAN:
                    return PengadaanConstants.Jadwal.KlarifikasiLanjutan;
                case EStatusPengadaan.PENILAIAN:
                    return PengadaanConstants.Jadwal.Penilaian;
                case EStatusPengadaan.PEMENANG:
                    return PengadaanConstants.Jadwal.PenentuanPemenang;
                default: return "";
            }
        }

        public JadwalPelaksanaan AddPelaksanaanPenilaian(JadwalPelaksanaan pelaksanaanPenilaian, Guid UserId)
        {
            Pengadaan Mpengadaaan = ctx.Pengadaans.Find(pelaksanaanPenilaian.PengadaanId);
            PersonilPengadaan picPersonil = ctx.PersonilPengadaans.Where(d => d.PersonilId == UserId && d.tipe == "pic" && d.PengadaanId == pelaksanaanPenilaian.PengadaanId).FirstOrDefault();
            if (picPersonil == null) return new JadwalPelaksanaan();
            if (ctx.Pengadaans.Find(pelaksanaanPenilaian.PengadaanId) != null)
            {
                // PelaksanaanAanwijzing MpelaksanaanAanwijzing = ctx.PelaksanaanAanwijzings.Where(d => d.PengadaanId == pelaksanaanAanwijzing.PengadaanId).FirstOrDefault();
                JadwalPelaksanaan MjadwalPelaksanaan = ctx.JadwalPelaksanaans.Where(d => d.PengadaanId == pelaksanaanPenilaian.PengadaanId
                        && d.statusPengadaan == EStatusPengadaan.PENILAIAN).FirstOrDefault();
                if (MjadwalPelaksanaan != null)
                {
                    MjadwalPelaksanaan.Mulai = pelaksanaanPenilaian.Mulai;
                    MjadwalPelaksanaan.Sampai = pelaksanaanPenilaian.Sampai;
                }
                else
                {
                    MjadwalPelaksanaan = new JadwalPelaksanaan();
                    MjadwalPelaksanaan.PengadaanId = pelaksanaanPenilaian.PengadaanId;
                    MjadwalPelaksanaan.statusPengadaan = EStatusPengadaan.PENILAIAN;
                    MjadwalPelaksanaan.Mulai = pelaksanaanPenilaian.Mulai;
                    MjadwalPelaksanaan.Sampai = pelaksanaanPenilaian.Sampai;
                    ctx.JadwalPelaksanaans.Add(MjadwalPelaksanaan);
                }
                ctx.SaveChanges();
                return MjadwalPelaksanaan;
            }
            return new JadwalPelaksanaan();
        }

        public JadwalPelaksanaan getPelaksanaanPenilaian(Guid PengadaanId, Guid UserId)
        {
            JadwalPelaksanaan MjadwalPelaksanaan = ctx.JadwalPelaksanaans.Where(d => d.PengadaanId == PengadaanId
                           && d.statusPengadaan == EStatusPengadaan.PENILAIAN).FirstOrDefault();
            if (MjadwalPelaksanaan != null)
            {
                JadwalPelaksanaan MOJadwalPelaksanaan = new JadwalPelaksanaan();
                MOJadwalPelaksanaan.Id = MjadwalPelaksanaan.Id;
                MOJadwalPelaksanaan.PengadaanId = MjadwalPelaksanaan.PengadaanId;
                MOJadwalPelaksanaan.Mulai = MjadwalPelaksanaan.Mulai;
                MOJadwalPelaksanaan.Sampai = MjadwalPelaksanaan.Sampai;
                return MOJadwalPelaksanaan;
            }
            else
            {
                JadwalPelaksanaan MOJadwalPelaksanaan = new JadwalPelaksanaan();
                JadwalPengadaan Mjadawal = ctx.JadwalPengadaans.Where(d => d.PengadaanId == PengadaanId && d.tipe == PengadaanConstants.Jadwal.Penilaian).FirstOrDefault();
                if (Mjadawal != null)
                {
                    MOJadwalPelaksanaan.Mulai = Mjadawal.Mulai;
                    MOJadwalPelaksanaan.Sampai = Mjadawal.Sampai;
                    MOJadwalPelaksanaan.PengadaanId = PengadaanId;
                    MOJadwalPelaksanaan.Pengadaan = null;
                }
                return MOJadwalPelaksanaan;
            }
            //return ctx.PelaksanaanBukaAmplops.Where(d => d.PengadaanId == PengadaanId).FirstOrDefault();
        }

        public JadwalPelaksanaan AddPelaksanaanKlarifikasi(JadwalPelaksanaan pelaksanaanKlarifikasi, Guid UserId)
        {
            Pengadaan Mpengadaaan = ctx.Pengadaans.Find(pelaksanaanKlarifikasi.PengadaanId);
            PersonilPengadaan picPersonil = ctx.PersonilPengadaans.Where(d => d.PersonilId == UserId && d.tipe == "pic" && d.PengadaanId == pelaksanaanKlarifikasi.PengadaanId).FirstOrDefault();
            if (picPersonil == null) return new JadwalPelaksanaan();
            if (ctx.Pengadaans.Find(pelaksanaanKlarifikasi.PengadaanId) != null)
            {
                // PelaksanaanAanwijzing MpelaksanaanAanwijzing = ctx.PelaksanaanAanwijzings.Where(d => d.PengadaanId == pelaksanaanAanwijzing.PengadaanId).FirstOrDefault();
                JadwalPelaksanaan MjadwalPelaksanaan = ctx.JadwalPelaksanaans.Where(d => d.PengadaanId == pelaksanaanKlarifikasi.PengadaanId
                        && d.statusPengadaan == EStatusPengadaan.KLARIFIKASI).FirstOrDefault();
                if (MjadwalPelaksanaan != null)
                {
                    MjadwalPelaksanaan.Mulai = pelaksanaanKlarifikasi.Mulai;
                    MjadwalPelaksanaan.Sampai = pelaksanaanKlarifikasi.Sampai;
                }
                else
                {
                    MjadwalPelaksanaan = new JadwalPelaksanaan();
                    MjadwalPelaksanaan.PengadaanId = pelaksanaanKlarifikasi.PengadaanId;
                    MjadwalPelaksanaan.statusPengadaan = EStatusPengadaan.KLARIFIKASI;
                    MjadwalPelaksanaan.Mulai = pelaksanaanKlarifikasi.Mulai;
                    MjadwalPelaksanaan.Sampai = pelaksanaanKlarifikasi.Sampai;
                    ctx.JadwalPelaksanaans.Add(MjadwalPelaksanaan);
                }
                ctx.SaveChanges();
                return MjadwalPelaksanaan;
            }
            return new JadwalPelaksanaan();
        }

        public JadwalPelaksanaan getPelaksanaanKlarifikasi(Guid PengadaanId, Guid UserId)
        {
            JadwalPelaksanaan MjadwalPelaksanaan = ctx.JadwalPelaksanaans.Where(d => d.PengadaanId == PengadaanId
                           && d.statusPengadaan == EStatusPengadaan.KLARIFIKASI).FirstOrDefault();
            if (MjadwalPelaksanaan != null)
            {
                JadwalPelaksanaan MOJadwalPelaksanaan = new JadwalPelaksanaan();
                MOJadwalPelaksanaan.Id = MjadwalPelaksanaan.Id;
                MOJadwalPelaksanaan.PengadaanId = MjadwalPelaksanaan.PengadaanId;
                MOJadwalPelaksanaan.Mulai = MjadwalPelaksanaan.Mulai;
                MOJadwalPelaksanaan.Sampai = MjadwalPelaksanaan.Sampai;
                return MOJadwalPelaksanaan;
            }
            else
            {
                JadwalPelaksanaan MOJadwalPelaksanaan = new JadwalPelaksanaan();
                JadwalPengadaan Mjadawal = ctx.JadwalPengadaans.Where(d => d.PengadaanId == PengadaanId && d.tipe == PengadaanConstants.Jadwal.Klarifikasi).FirstOrDefault();
                if (Mjadawal != null)
                {
                    MOJadwalPelaksanaan.Mulai = Mjadawal.Mulai;
                    MOJadwalPelaksanaan.Sampai = Mjadawal.Sampai;
                    MOJadwalPelaksanaan.PengadaanId = PengadaanId;
                    MOJadwalPelaksanaan.Pengadaan = null;
                }
                return MOJadwalPelaksanaan;
            }
            //return ctx.PelaksanaanBukaAmplops.Where(d => d.PengadaanId == PengadaanId).FirstOrDefault();
        }

        public JadwalPelaksanaan getPelaksanaanKlarifikasiLanjutan(Guid PengadaanId, Guid UserId)
        {
            JadwalPelaksanaan MjadwalPelaksanaan = ctx.JadwalPelaksanaans.Where(d => d.PengadaanId == PengadaanId
                           && d.statusPengadaan == EStatusPengadaan.KLARIFIKASILANJUTAN).FirstOrDefault();
            if (MjadwalPelaksanaan != null)
            {
                JadwalPelaksanaan MOJadwalPelaksanaan = new JadwalPelaksanaan();
                MOJadwalPelaksanaan.Id = MjadwalPelaksanaan.Id;
                MOJadwalPelaksanaan.PengadaanId = MjadwalPelaksanaan.PengadaanId;
                MOJadwalPelaksanaan.Mulai = MjadwalPelaksanaan.Mulai;
                MOJadwalPelaksanaan.Sampai = MjadwalPelaksanaan.Sampai;
                return MOJadwalPelaksanaan;
            }
            else
            {
                JadwalPelaksanaan MOJadwalPelaksanaan = new JadwalPelaksanaan();
                JadwalPengadaan Mjadawal = ctx.JadwalPengadaans.Where(d => d.PengadaanId == PengadaanId && d.tipe == PengadaanConstants.Jadwal.KlarifikasiLanjutan).FirstOrDefault();
                if (Mjadawal != null)
                {
                    MOJadwalPelaksanaan.Mulai = Mjadawal.Mulai;
                    MOJadwalPelaksanaan.Sampai = Mjadawal.Sampai;
                    MOJadwalPelaksanaan.PengadaanId = PengadaanId;
                    MOJadwalPelaksanaan.Pengadaan = null;
                }
                return MOJadwalPelaksanaan;
            }
            //return ctx.PelaksanaanBukaAmplops.Where(d => d.PengadaanId == PengadaanId).FirstOrDefault();
        }

        public JadwalPelaksanaan AddPelaksanaanPemenang(JadwalPelaksanaan pelaksanaanPemenang, Guid UserId)
        {
            Pengadaan Mpengadaaan = ctx.Pengadaans.Find(pelaksanaanPemenang.PengadaanId);
            PersonilPengadaan picPersonil = ctx.PersonilPengadaans.Where(d => d.PersonilId == UserId && d.tipe == "pic" && d.PengadaanId == pelaksanaanPemenang.PengadaanId).FirstOrDefault();
            if (picPersonil == null) return new JadwalPelaksanaan();
            if (ctx.Pengadaans.Find(pelaksanaanPemenang.PengadaanId) != null)
            {
                // PelaksanaanAanwijzing MpelaksanaanAanwijzing = ctx.PelaksanaanAanwijzings.Where(d => d.PengadaanId == pelaksanaanAanwijzing.PengadaanId).FirstOrDefault();
                JadwalPelaksanaan MjadwalPelaksanaan = ctx.JadwalPelaksanaans.Where(d => d.PengadaanId == pelaksanaanPemenang.PengadaanId
                        && d.statusPengadaan == EStatusPengadaan.PEMENANG).FirstOrDefault();
                if (MjadwalPelaksanaan != null)
                {
                    MjadwalPelaksanaan.Mulai = pelaksanaanPemenang.Mulai;
                    MjadwalPelaksanaan.Sampai = pelaksanaanPemenang.Sampai;
                }
                else
                {
                    MjadwalPelaksanaan = new JadwalPelaksanaan();
                    MjadwalPelaksanaan.PengadaanId = pelaksanaanPemenang.PengadaanId;
                    MjadwalPelaksanaan.statusPengadaan = EStatusPengadaan.PEMENANG;
                    MjadwalPelaksanaan.Mulai = pelaksanaanPemenang.Mulai;
                    MjadwalPelaksanaan.Sampai = pelaksanaanPemenang.Sampai;
                    ctx.JadwalPelaksanaans.Add(MjadwalPelaksanaan);
                }
                ctx.SaveChanges();
                return MjadwalPelaksanaan;
            }
            return new JadwalPelaksanaan();
        }

        public JadwalPelaksanaan getPelaksanaanPemenang(Guid PengadaanId, Guid UserId)
        {
            JadwalPelaksanaan MjadwalPelaksanaan = ctx.JadwalPelaksanaans.Where(d => d.PengadaanId == PengadaanId
                           && d.statusPengadaan == EStatusPengadaan.PEMENANG).FirstOrDefault();
            if (MjadwalPelaksanaan != null)
            {
                JadwalPelaksanaan MOJadwalPelaksanaan = new JadwalPelaksanaan();
                MOJadwalPelaksanaan.Id = MjadwalPelaksanaan.Id;
                MOJadwalPelaksanaan.PengadaanId = MjadwalPelaksanaan.PengadaanId;
                MOJadwalPelaksanaan.Mulai = MjadwalPelaksanaan.Mulai;
                MOJadwalPelaksanaan.Sampai = MjadwalPelaksanaan.Sampai;
                return MOJadwalPelaksanaan;
            }
            else
            {
                JadwalPelaksanaan MOJadwalPelaksanaan = new JadwalPelaksanaan();
                JadwalPengadaan Mjadawal = ctx.JadwalPengadaans.Where(d => d.PengadaanId == PengadaanId && d.tipe == PengadaanConstants.Jadwal.PenentuanPemenang).FirstOrDefault();
                if (Mjadawal != null)
                {
                    MOJadwalPelaksanaan.Mulai = Mjadawal.Mulai;
                    MOJadwalPelaksanaan.Sampai = Mjadawal.Sampai;
                    MOJadwalPelaksanaan.PengadaanId = PengadaanId;
                    MOJadwalPelaksanaan.Pengadaan = null;
                }
                return MOJadwalPelaksanaan;
            }
            //return ctx.PelaksanaanBukaAmplops.Where(d => d.PengadaanId == PengadaanId).FirstOrDefault();
        }

        public int deleteDokumenPelaksanaan(Guid Id, Guid UserId, int isApprovel)
        {
            try
            {
                DokumenPengadaan MdokPengadaan = ctx.DokumenPengadaans.Find(Id);
                int isPic = ctx.PersonilPengadaans.Where(d => d.PengadaanId == MdokPengadaan.PengadaanId && d.PersonilId == UserId && d.tipe == "pic").ToList().Count() > 0 ? 1 : 0;
                if (isPic == 0) return 0;
                if (MdokPengadaan.Tipe == TipeBerkas.NOTA || MdokPengadaan.Tipe == TipeBerkas.DOKUMENLAIN || MdokPengadaan.Tipe == TipeBerkas.NOTA) return 0;
                if (MdokPengadaan.Tipe == TipeBerkas.SuratPerintahKerja)
                {
                    var pemenangId = MdokPengadaan.Pengadaan.PemenangPengadaans.Where(dd => dd.VendorId == MdokPengadaan.VendorId).FirstOrDefault().Id;
                    var oSpk = ctx.Spk.Where(d => d.PemenangPengadaanId == pemenangId).FirstOrDefault();
                    ctx.DokumenSpk.RemoveRange(oSpk.DokumenSpk);


                }
                ctx.DokumenPengadaans.Remove(MdokPengadaan);
                ctx.SaveChanges(UserId.ToString());
                return 1;
            }
            catch (Exception ex) { return 0; }
        }

        public int deleteDokumenRekanan(Guid Id, Guid UserId)
        {
            try
            {
                DokumenPengadaan MdokPengadaan = ctx.DokumenPengadaans.Find(Id);
                var vendor = ctx.Vendors.Where(d => d.Owner == UserId).FirstOrDefault();
                if (vendor == null) return 0;
                if (MdokPengadaan == null) return 0;
                Pengadaan oPengadaan = ctx.Pengadaans.Find(MdokPengadaan.PengadaanId);
                if (MdokPengadaan.Tipe != TipeBerkas.BerkasRekanan && MdokPengadaan.Tipe != TipeBerkas.BerkasRekananKlarifikasi && MdokPengadaan.Tipe != TipeBerkas.BerkasRekananKlarifikasiLanjutan) return 0;
                if (MdokPengadaan.Tipe == TipeBerkas.BerkasRekanan)
                {
                    if (oPengadaan.Status != EStatusPengadaan.SUBMITPENAWARAN) return 0;
                    cekStateSubmitPenawaran(oPengadaan.Id);
                }
                if (MdokPengadaan.Tipe == TipeBerkas.BerkasRekananKlarifikasi)
                {
                    if (oPengadaan.Status != EStatusPengadaan.KLARIFIKASI) return 0;
                    cekStateKlarifikasi(oPengadaan.Id);
                }
                if (MdokPengadaan.Tipe == TipeBerkas.BerkasRekananKlarifikasiLanjutan)
                {
                    if (oPengadaan.Status != EStatusPengadaan.KLARIFIKASILANJUTAN) return 0;
                    cekStateKlarifikasiLanjutan(oPengadaan.Id);
                }

                ctx.DokumenPengadaans.Remove(MdokPengadaan);
                ctx.SaveChanges();
                return 1;
            }
            catch { return 0; }
        }

        public int AjukanPengadaan(Guid Id, Guid UserId, List<Guid> manager)
        {
            // Guid manajer = new Guid(ConfigurationManager.AppSettings["manajer"].ToString());

            Pengadaan Mpengadaan = ctx.Pengadaans.Find(Id);
            if (Mpengadaan.Status == EStatusPengadaan.DRAFT)
            {
                ctx.MessagePengadaans.RemoveRange(ctx.MessagePengadaans.Where(d => d.PengadaanId == Id));
                ctx.SaveChanges();
                Mpengadaan.Status = EStatusPengadaan.AJUKAN;
                MessagePengadaan mMessagePengadaan = new MessagePengadaan();
                mMessagePengadaan.Message = "Mengajukan Pengadaan";
                mMessagePengadaan.PengadaanId = Mpengadaan.Id;
                mMessagePengadaan.UserTo = manager.FirstOrDefault();
                mMessagePengadaan.FromTo = UserId;
                mMessagePengadaan.Waktu = DateTime.Now;
                mMessagePengadaan.Status = EStatusPengadaan.AJUKAN;
                ctx.MessagePengadaans.Add(mMessagePengadaan);
                ctx.SaveChanges();
            }
            try
            {
                ctx.SaveChanges();
                return 1;
            }
            catch (Exception dbEx)
            {
                return 0;
            }

        }

        public int UpdateStatus(Guid Id, EStatusPengadaan status)
        {
            try
            {
                Pengadaan Mpengadaan = ctx.Pengadaans.Find(Id);
                Mpengadaan.Status = status;
                ctx.SaveChanges();
                return 1;
            }
            catch { return 0; }
        }

        public JadwalPelaksanaan saveJadwalPelaksanaan(JadwalPelaksanaan jadwalPelaksanaan)
        {
            Pengadaan Mpengadaan = ctx.Pengadaans.Where(d => d.Id == jadwalPelaksanaan.PengadaanId).FirstOrDefault();
            if (Mpengadaan == null) return new JadwalPelaksanaan();
            JadwalPelaksanaan MJadwalPelaksanaan = new JadwalPelaksanaan
            {
                PengadaanId = jadwalPelaksanaan.PengadaanId,
                Mulai = jadwalPelaksanaan.Mulai,
                Sampai = jadwalPelaksanaan.Sampai,
                statusPengadaan = jadwalPelaksanaan.statusPengadaan
            };
            ctx.JadwalPelaksanaans.Add(MJadwalPelaksanaan);
            return MJadwalPelaksanaan;
        }

        public int cekStateDiSetujui(Guid PengadaanId)
        {
            DateTime? anwijzingDate = new DateTime();
            JadwalPelaksanaan MJadwalPelaksanaan = ctx.JadwalPelaksanaans
                    .Where(d => d.statusPengadaan == EStatusPengadaan.AANWIJZING && d.PengadaanId == PengadaanId).FirstOrDefault();

            if (MJadwalPelaksanaan == null)
            {
                JadwalPengadaan mJadwalPengadaan = ctx.JadwalPengadaans.Where(d => d.PengadaanId == PengadaanId &&
                                    d.tipe == PengadaanConstants.Jadwal.Aanwijzing).FirstOrDefault();
                anwijzingDate = mJadwalPengadaan.Mulai;
                // if (mJadwalPengadaan == null) return 0;
                //anwijzingDate = mJadwalPengadaan.Mulai;
                //if (mJadwalPengadaan.Mulai < DateTime.Now)
                //{
                //    JadwalPelaksanaan mmJadwalPelaksanaan = new JadwalPelaksanaan();
                //    mmJadwalPelaksanaan.PengadaanId = PengadaanId;
                //    mmJadwalPelaksanaan.Mulai = DateTime.Now.AddDays(1);
                //    mmJadwalPelaksanaan.statusPengadaan = EStatusPengadaan.AANWIJZING;
                //    ctx.JadwalPelaksanaans.Add(mmJadwalPelaksanaan);
                //    ctx.SaveChanges();
                //    UpdateStatus(PengadaanId, EStatusPengadaan.AANWIJZING);
                //}
                //return 1;

            }
            else
            {
                anwijzingDate = MJadwalPelaksanaan.Mulai;
            }
            if (anwijzingDate == null) return 0;
            if (DateTime.Now < anwijzingDate)
            {
                return 0;
            }
            else
            {
                //MJadwalPelaksanaan.Mulai = DateTime.Now.AddDays(1);
                UpdateStatus(PengadaanId, EStatusPengadaan.AANWIJZING);
                return 1;
            }

        }

        public int cekStateAanwijzing(Guid PengadaanId)
        {
            DateTime? anwijzingDate = new DateTime();
            JadwalPelaksanaan MJadwalPelaksanaan = ctx.JadwalPelaksanaans
                    .Where(d => d.statusPengadaan == EStatusPengadaan.SUBMITPENAWARAN && d.PengadaanId == PengadaanId).FirstOrDefault();

            if (MJadwalPelaksanaan == null)
            {
                JadwalPengadaan mJadwalPengadaan = ctx.JadwalPengadaans.Where(d => d.PengadaanId == PengadaanId &&
                                    d.tipe == PengadaanConstants.Jadwal.PengisianHarga).FirstOrDefault();
                if (mJadwalPengadaan == null) return 0;
                anwijzingDate = mJadwalPengadaan.Mulai;
            }
            else anwijzingDate = MJadwalPelaksanaan.Mulai;

            if (anwijzingDate == null) return 0;
            if (DateTime.Now < anwijzingDate)
            {
                return 0;
            }
            else
            {
                UpdateStatus(PengadaanId, EStatusPengadaan.SUBMITPENAWARAN);
                return 1;
            }
        }

        public int cekStateSubmitPenawaran(Guid PengadaanId)
        {
            DateTime? SubmitPenawaranDate = new DateTime();


            JadwalPelaksanaan MJadwalPelaksanaan = ctx.JadwalPelaksanaans
                    .Where(d => d.statusPengadaan == EStatusPengadaan.SUBMITPENAWARAN && d.PengadaanId == PengadaanId).FirstOrDefault();

            if (MJadwalPelaksanaan == null)
            {
                JadwalPengadaan mJadwalPengadaan = ctx.JadwalPengadaans.Where(d => d.PengadaanId == PengadaanId &&
                                    d.tipe == PengadaanConstants.Jadwal.PengisianHarga).FirstOrDefault();

                if (mJadwalPengadaan == null) return 0;
                SubmitPenawaranDate = mJadwalPengadaan.Sampai;
            }
            else
            {
                SubmitPenawaranDate = MJadwalPelaksanaan.Sampai;
            }
            if (SubmitPenawaranDate == null)
            {
                return 0;
            }
            if (DateTime.Now < SubmitPenawaranDate) return 0;
            else
            {
                //JadwalPengadaan MMJadwalPengadaan = ctx.JadwalPengadaans
                //    .Where(d => d.PengadaanId == PengadaanId && d.tipe == PengadaanConstants.Jadwal.PengisianHarga)
                //    .FirstOrDefault();
                //if (SubmitPenawaranDate < DateTime.Now)
                //{
                //    JadwalPelaksanaan mmJadwalPelaksanaan = new JadwalPelaksanaan();
                //    mmJadwalPelaksanaan.Sampai = DateTime.Now.AddDays(1);
                //    mmJadwalPelaksanaan.PengadaanId = PengadaanId;
                //    mmJadwalPelaksanaan.Mulai=MMJadwalPengadaan.Mulai;
                //    mmJadwalPelaksanaan.statusPengadaan=EStatusPengadaan.BUKAAMPLOP;
                //    ctx.JadwalPelaksanaans.Add(mmJadwalPelaksanaan);
                //    ctx.SaveChanges();
                //}

                UpdateStatus(PengadaanId, EStatusPengadaan.BUKAAMPLOP);
                return 1;
            }
        }

        public int cekStateBukaAmplop(Guid PengadaanId)
        {
            DateTime? BukaAmplopDate = new DateTime();
            JadwalPelaksanaan MJadwalPelaksanaan = ctx.JadwalPelaksanaans
                    .Where(d => d.statusPengadaan == EStatusPengadaan.BUKAAMPLOP && d.PengadaanId == PengadaanId).FirstOrDefault();

            if (MJadwalPelaksanaan == null)
            {
                JadwalPengadaan mJadwalPengadaan = ctx.JadwalPengadaans.Where(d => d.PengadaanId == PengadaanId &&
                                    d.tipe == PengadaanConstants.Jadwal.BukaAmplop).FirstOrDefault();
                if (mJadwalPengadaan == null) return 0;
                BukaAmplopDate = mJadwalPengadaan.Sampai;
            }
            else BukaAmplopDate = MJadwalPelaksanaan.Sampai;

            if (BukaAmplopDate == null)
            {
                return 0;
            }
            if (DateTime.Now < BukaAmplopDate) return 0;
            else
            {
                //JadwalPengadaan MMJadwalPengadaan = ctx.JadwalPengadaans
                //    .Where(d => d.PengadaanId == PengadaanId && d.tipe == PengadaanConstants.Jadwal.BukaAmplop)
                //    .FirstOrDefault();
                //if (MMJadwalPengadaan.Sampai < DateTime.Now)
                //{
                //    JadwalPelaksanaan mmJadwalPelaksanaan = new JadwalPelaksanaan();
                //    mmJadwalPelaksanaan.Sampai = DateTime.Now.AddDays(1);
                //    mmJadwalPelaksanaan.PengadaanId = PengadaanId;
                //    mmJadwalPelaksanaan.Mulai = MMJadwalPengadaan.Mulai;
                //    mmJadwalPelaksanaan.statusPengadaan = EStatusPengadaan.PENILAIAN;
                //    ctx.JadwalPelaksanaans.Add(mmJadwalPelaksanaan);
                //    ctx.SaveChanges();
                //}
                UpdateStatus(PengadaanId, EStatusPengadaan.PENILAIAN);
                return 1;
            }
        }

        public int cekStatePenilaian(Guid PengadaanId)
        {
            DateTime? PenilaianDate = new DateTime();
            JadwalPelaksanaan MJadwalPelaksanaan = ctx.JadwalPelaksanaans
                    .Where(d => d.statusPengadaan == EStatusPengadaan.PENILAIAN && d.PengadaanId == PengadaanId).FirstOrDefault();

            if (MJadwalPelaksanaan == null)
            {
                JadwalPengadaan mJadwalPengadaan = ctx.JadwalPengadaans.Where(d => d.PengadaanId == PengadaanId &&
                                    d.tipe == PengadaanConstants.Jadwal.Penilaian).FirstOrDefault();
                if (mJadwalPengadaan == null) return 0;
                PenilaianDate = mJadwalPengadaan.Sampai;
            }
            else PenilaianDate = MJadwalPelaksanaan.Sampai;

            if (PenilaianDate == null)
            {
                return 0;
            }
            if (DateTime.Now < PenilaianDate) return 0;
            else
            {
                //JadwalPengadaan MMJadwalPengadaan = ctx.JadwalPengadaans
                //    .Where(d => d.PengadaanId == PengadaanId && d.tipe == PengadaanConstants.Jadwal.Penilaian)
                //    .FirstOrDefault();
                //if (MMJadwalPengadaan.Sampai < DateTime.Now)
                //{
                //    JadwalPelaksanaan mmJadwalPelaksanaan = new JadwalPelaksanaan();
                //    mmJadwalPelaksanaan.Sampai = DateTime.Now.AddDays(1);
                //    mmJadwalPelaksanaan.PengadaanId = PengadaanId;
                //    mmJadwalPelaksanaan.Mulai = MMJadwalPengadaan.Mulai;
                //    mmJadwalPelaksanaan.statusPengadaan = EStatusPengadaan.PENILAIAN;
                //    ctx.JadwalPelaksanaans.Add(mmJadwalPelaksanaan);
                //    ctx.SaveChanges();
                //}
                UpdateStatus(PengadaanId, EStatusPengadaan.KLARIFIKASI);
                return 1;
            }
        }

        public int cekStateKlarifikasi(Guid PengadaanId)
        {
            DateTime? KlarifikasiDate = new DateTime();
            JadwalPelaksanaan MJadwalPelaksanaan = ctx.JadwalPelaksanaans
                    .Where(d => d.statusPengadaan == EStatusPengadaan.KLARIFIKASI && d.PengadaanId == PengadaanId).FirstOrDefault();

            if (MJadwalPelaksanaan == null)
            {
                JadwalPengadaan mJadwalPengadaan = ctx.JadwalPengadaans.Where(d => d.PengadaanId == PengadaanId &&
                                    d.tipe == PengadaanConstants.Jadwal.Klarifikasi).FirstOrDefault();
                if (mJadwalPengadaan == null) return 0;
                KlarifikasiDate = mJadwalPengadaan.Sampai;
            }
            else KlarifikasiDate = MJadwalPelaksanaan.Sampai;

            if (KlarifikasiDate == null)
            {
                return 0;
            }
            if (DateTime.Now < KlarifikasiDate) return 0;
            else
            {
                //JadwalPengadaan MMJadwalPengadaan = ctx.JadwalPengadaans
                //    .Where(d => d.PengadaanId == PengadaanId && d.tipe == PengadaanConstants.Jadwal.Klarifikasi)
                //    .FirstOrDefault();
                //if (MMJadwalPengadaan.Sampai < DateTime.Now)
                //{
                //    JadwalPelaksanaan mmJadwalPelaksanaan = new JadwalPelaksanaan();
                //    mmJadwalPelaksanaan.Sampai = DateTime.Now.AddDays(1);
                //    mmJadwalPelaksanaan.PengadaanId = PengadaanId;
                //    mmJadwalPelaksanaan.Mulai = MMJadwalPengadaan.Mulai;
                //    mmJadwalPelaksanaan.statusPengadaan = EStatusPengadaan.PENILAIAN;
                //    ctx.JadwalPelaksanaans.Add(mmJadwalPelaksanaan);
                //    ctx.SaveChanges();
                //}
                UpdateStatus(PengadaanId, EStatusPengadaan.PEMENANG);
                return 1;
            }
        }

        public int cekStateKlarifikasiLanjutan(Guid PengadaanId)
        {
            DateTime? KlarifikasiDate = new DateTime();
            JadwalPelaksanaan MJadwalPelaksanaan = ctx.JadwalPelaksanaans
                    .Where(d => d.statusPengadaan == EStatusPengadaan.KLARIFIKASILANJUTAN && d.PengadaanId == PengadaanId).FirstOrDefault();

            if (MJadwalPelaksanaan == null)
            {
                JadwalPengadaan mJadwalPengadaan = ctx.JadwalPengadaans.Where(d => d.PengadaanId == PengadaanId &&
                                    d.tipe == PengadaanConstants.Jadwal.KlarifikasiLanjutan).FirstOrDefault();
                if (mJadwalPengadaan == null) return 0;
                KlarifikasiDate = mJadwalPengadaan.Sampai;
            }
            else KlarifikasiDate = MJadwalPelaksanaan.Sampai;

            if (KlarifikasiDate == null)
            {
                return 0;
            }
            if (DateTime.Now < KlarifikasiDate) return 0;
            else
            {
                UpdateStatus(PengadaanId, EStatusPengadaan.PEMENANG);
                return 1;
            }
        }

        public KualifikasiKandidat addKualifikasiKandidat(KualifikasiKandidat dKualifikasiKandidat, Guid UserId)
        {
            if (ctx.Pengadaans.Find(dKualifikasiKandidat.PengadaanId) == null) return dKualifikasiKandidat;
            ctx.KualifikasiKandidats.Add(dKualifikasiKandidat);
            ctx.SaveChanges();
            return dKualifikasiKandidat;
        }

        //rekanana masukan harga tawaran pada submit penawaran
        public List<VWRKSDetailRekanan> addHargaRekanan(List<VWRKSDetailRekanan> dlstHargaRekanan, Guid PengadaanId, Guid UserId)
        {
            JimbisEncrypt code = new JimbisEncrypt();
            List<VWRKSDetailRekanan> newLstVWRKSDetailRekanan = new List<VWRKSDetailRekanan>();
            if (ctx.Pengadaans.Find(PengadaanId) == null) return new List<VWRKSDetailRekanan>();
            else
            {
                if (ctx.Pengadaans.Find(PengadaanId).Status != EStatusPengadaan.SUBMITPENAWARAN) return new List<VWRKSDetailRekanan>();
                if (cekStateSubmitPenawaran(PengadaanId) == 1) return new List<VWRKSDetailRekanan>();
            }
            foreach (var item in dlstHargaRekanan)
            {
                var vendorId = ctx.Vendors.Where(xx => xx.Owner == UserId).FirstOrDefault().Id;
                var oldHargaRekanan = ctx.HargaRekanans.Where(d => d.RKSDetailId == item.Id && d.VendorId == vendorId).FirstOrDefault();

                //if (item.HargaRekananId != Guid.Empty && item.HargaRekananId != null)
                if (oldHargaRekanan != null)
                {
                    var hargaEncrypet = code.Encrypt(item.harga == null ? "0" : item.harga.ToString());
                    //var Dec = code.Decrypt(enco);
                    //HargaRekanan oldHargaRekanan = ctx.HargaRekanans.Find(item.HargaRekananId);
                    //HargaRekanan oldHargaRekanan = ctx.HargaRekanans.Find(item.HargaRekananId);
                    //oldHargaRekanan.harga = item.harga;
                    oldHargaRekanan.hargaEncrypt = hargaEncrypet;
                    oldHargaRekanan.keterangan = item.keterangan;
                    ctx.SaveChanges();

                    newLstVWRKSDetailRekanan.Add(new VWRKSDetailRekanan
                    {
                        Id = ctx.RKSDetails.Where(d => d.Id == oldHargaRekanan.RKSDetailId) == null ? Guid.Empty : ctx.RKSDetails.Where(d => d.Id == oldHargaRekanan.RKSDetailId).FirstOrDefault().Id,
                        item = ctx.RKSDetails.Where(d => d.Id == oldHargaRekanan.RKSDetailId) == null ? "" : ctx.RKSDetails.Where(d => d.Id == oldHargaRekanan.RKSDetailId).FirstOrDefault().item,
                        ItemId = ctx.RKSDetails.Where(d => d.Id == oldHargaRekanan.RKSDetailId) == null ? Guid.Empty : ctx.RKSDetails.Where(d => d.Id == oldHargaRekanan.RKSDetailId).FirstOrDefault().ItemId,
                        jumlah = ctx.RKSDetails.Where(d => d.Id == oldHargaRekanan.RKSDetailId) == null ? null : ctx.RKSDetails.Where(d => d.Id == oldHargaRekanan.RKSDetailId).FirstOrDefault().jumlah,
                        satuan = ctx.RKSDetails.Where(d => d.Id == oldHargaRekanan.RKSDetailId) == null ? "" : ctx.RKSDetails.Where(d => d.Id == oldHargaRekanan.RKSDetailId).FirstOrDefault().satuan,
                        harga = oldHargaRekanan.harga,
                        HargaRekananId = oldHargaRekanan.Id,
                        keterangan = oldHargaRekanan.keterangan
                    });
                }
                else
                {
                    HargaRekanan newHargaRekanan = new HargaRekanan();
                    var hargaEncrypet = code.Encrypt(item.harga == null ? "0" : item.harga.ToString());
                    //newHargaRekanan.harga = item.harga;
                    newHargaRekanan.hargaEncrypt = hargaEncrypet;
                    newHargaRekanan.keterangan = item.keterangan;
                    newHargaRekanan.RKSDetailId = item.Id;
                    newHargaRekanan.VendorId = ctx.Vendors.Where(xx => xx.Owner == UserId).FirstOrDefault().Id; //3;//contoh vendor
                    ctx.HargaRekanans.Add(newHargaRekanan);
                    ctx.SaveChanges();
                    newLstVWRKSDetailRekanan.Add(new VWRKSDetailRekanan
                    {
                        Id = ctx.RKSDetails.Where(d => d.Id == newHargaRekanan.RKSDetailId) == null ? Guid.Empty : ctx.RKSDetails.Where(d => d.Id == newHargaRekanan.RKSDetailId).FirstOrDefault().Id,
                        item = ctx.RKSDetails.Where(d => d.Id == newHargaRekanan.RKSDetailId) == null ? "" : ctx.RKSDetails.Where(d => d.Id == newHargaRekanan.RKSDetailId).FirstOrDefault().item,
                        ItemId = ctx.RKSDetails.Where(d => d.Id == newHargaRekanan.RKSDetailId) == null ? Guid.Empty : ctx.RKSDetails.Where(d => d.Id == newHargaRekanan.RKSDetailId).FirstOrDefault().ItemId,
                        jumlah = ctx.RKSDetails.Where(d => d.Id == newHargaRekanan.RKSDetailId) == null ? null : ctx.RKSDetails.Where(d => d.Id == newHargaRekanan.RKSDetailId).FirstOrDefault().jumlah,
                        satuan = ctx.RKSDetails.Where(d => d.Id == newHargaRekanan.RKSDetailId) == null ? "" : ctx.RKSDetails.Where(d => d.Id == newHargaRekanan.RKSDetailId).FirstOrDefault().satuan,
                        harga = newHargaRekanan.harga,
                        HargaRekananId = newHargaRekanan.Id,
                        keterangan = newHargaRekanan.keterangan
                    });
                }

            }

            return newLstVWRKSDetailRekanan;
        }

        //rekanana masukan harga tawaran pada klarifikasi 
        public List<VWRKSDetailRekanan> addHargaKlarifikasiRekanan(List<VWRKSDetailRekanan> dlstHargaKlarifikasiRekanan, Guid PengadaanId, Guid UserId)
        {
            List<VWRKSDetailRekanan> newLstVWRKSDetailRekanan = new List<VWRKSDetailRekanan>();
            if (ctx.Pengadaans.Find(PengadaanId) == null) return new List<VWRKSDetailRekanan>();
            else
            {
                if (ctx.Pengadaans.Find(PengadaanId).Status != EStatusPengadaan.KLARIFIKASI) return new List<VWRKSDetailRekanan>();
            }
            foreach (var item in dlstHargaKlarifikasiRekanan)
            {
                var vendorId = ctx.Vendors.Where(xx => xx.Owner == UserId).FirstOrDefault().Id;
                HargaKlarifikasiRekanan oldHargaKlarifikasiRekanan = ctx.HargaKlarifikasiRekanans.Where(d => d.VendorId == vendorId && d.RKSDetailId == item.Id).FirstOrDefault();
                //if (item.HargaRekananId != Guid.Empty && item.HargaRekananId != null)
                if (oldHargaKlarifikasiRekanan != null)
                {

                    oldHargaKlarifikasiRekanan.harga = item.harga;
                    oldHargaKlarifikasiRekanan.keterangan = item.keterangan;
                    ctx.SaveChanges();
                    newLstVWRKSDetailRekanan.Add(new VWRKSDetailRekanan
                    {
                        Id = ctx.RKSDetails.Where(d => d.Id == oldHargaKlarifikasiRekanan.RKSDetailId) == null ? Guid.Empty : ctx.RKSDetails.Where(d => d.Id == oldHargaKlarifikasiRekanan.RKSDetailId).FirstOrDefault().Id,
                        item = ctx.RKSDetails.Where(d => d.Id == oldHargaKlarifikasiRekanan.RKSDetailId) == null ? "" : ctx.RKSDetails.Where(d => d.Id == oldHargaKlarifikasiRekanan.RKSDetailId).FirstOrDefault().item,
                        ItemId = ctx.RKSDetails.Where(d => d.Id == oldHargaKlarifikasiRekanan.RKSDetailId) == null ? Guid.Empty : ctx.RKSDetails.Where(d => d.Id == oldHargaKlarifikasiRekanan.RKSDetailId).FirstOrDefault().ItemId,
                        jumlah = ctx.RKSDetails.Where(d => d.Id == oldHargaKlarifikasiRekanan.RKSDetailId) == null ? null : ctx.RKSDetails.Where(d => d.Id == oldHargaKlarifikasiRekanan.RKSDetailId).FirstOrDefault().jumlah,
                        satuan = ctx.RKSDetails.Where(d => d.Id == oldHargaKlarifikasiRekanan.RKSDetailId) == null ? "" : ctx.RKSDetails.Where(d => d.Id == oldHargaKlarifikasiRekanan.RKSDetailId).FirstOrDefault().satuan,
                        harga = oldHargaKlarifikasiRekanan.harga,
                        HargaRekananId = oldHargaKlarifikasiRekanan.Id,
                        keterangan = oldHargaKlarifikasiRekanan.keterangan
                    });
                }
                else
                {
                    HargaKlarifikasiRekanan newHargaKlarifikasiRekanan = new HargaKlarifikasiRekanan();
                    newHargaKlarifikasiRekanan.harga = item.harga;
                    newHargaKlarifikasiRekanan.keterangan = item.keterangan;
                    newHargaKlarifikasiRekanan.RKSDetailId = item.Id;
                    newHargaKlarifikasiRekanan.VendorId = ctx.Vendors.Where(xx => xx.Owner == UserId).FirstOrDefault().Id; //3;//contoh vendor
                    ctx.HargaKlarifikasiRekanans.Add(newHargaKlarifikasiRekanan);
                    ctx.SaveChanges();
                    newLstVWRKSDetailRekanan.Add(new VWRKSDetailRekanan
                    {
                        Id = ctx.RKSDetails.Where(d => d.Id == newHargaKlarifikasiRekanan.RKSDetailId) == null ? Guid.Empty : ctx.RKSDetails.Where(d => d.Id == newHargaKlarifikasiRekanan.RKSDetailId).FirstOrDefault().Id,
                        item = ctx.RKSDetails.Where(d => d.Id == newHargaKlarifikasiRekanan.RKSDetailId) == null ? "" : ctx.RKSDetails.Where(d => d.Id == newHargaKlarifikasiRekanan.RKSDetailId).FirstOrDefault().item,
                        ItemId = ctx.RKSDetails.Where(d => d.Id == newHargaKlarifikasiRekanan.RKSDetailId) == null ? Guid.Empty : ctx.RKSDetails.Where(d => d.Id == newHargaKlarifikasiRekanan.RKSDetailId).FirstOrDefault().ItemId,
                        jumlah = ctx.RKSDetails.Where(d => d.Id == newHargaKlarifikasiRekanan.RKSDetailId) == null ? null : ctx.RKSDetails.Where(d => d.Id == newHargaKlarifikasiRekanan.RKSDetailId).FirstOrDefault().jumlah,
                        satuan = ctx.RKSDetails.Where(d => d.Id == newHargaKlarifikasiRekanan.RKSDetailId) == null ? "" : ctx.RKSDetails.Where(d => d.Id == newHargaKlarifikasiRekanan.RKSDetailId).FirstOrDefault().satuan,
                        harga = newHargaKlarifikasiRekanan.harga,
                        HargaRekananId = newHargaKlarifikasiRekanan.Id,
                        keterangan = newHargaKlarifikasiRekanan.keterangan
                    });
                }

            }

            return newLstVWRKSDetailRekanan;
        }

        //rekanana masukan harga tawaran pada klarifikasi lanjutan
        public List<VWRKSDetailRekanan> addHargaKlarifikasiLanjutanRekanan(List<VWRKSDetailRekanan> dlstHargaKlarifikasiRekanan, Guid PengadaanId, Guid UserId)
        {
            List<VWRKSDetailRekanan> newLstVWRKSDetailRekanan = new List<VWRKSDetailRekanan>();
            if (ctx.Pengadaans.Find(PengadaanId) == null) return new List<VWRKSDetailRekanan>();
            else
            {
                if (ctx.Pengadaans.Find(PengadaanId).Status != EStatusPengadaan.KLARIFIKASILANJUTAN) return new List<VWRKSDetailRekanan>();
            }
            var vendorId = ctx.Vendors.Where(xx => xx.Owner == UserId).FirstOrDefault().Id;
            var cekLanjutan = ctx.PemenangPengadaans.Where(d => d.VendorId == vendorId && d.PengadaanId == PengadaanId).FirstOrDefault();

            if (cekLanjutan != null)
            {
                foreach (var item in dlstHargaKlarifikasiRekanan)
                {

                    HargaKlarifikasiLanLanjutan oldHargaKlarifikasiLanjutanRekanan = ctx.HargaKlarifikasiLanLanjutans.Where(d => d.VendorId == vendorId && d.RKSDetailId == item.Id).FirstOrDefault();
                    //if (item.HargaRekananId != Guid.Empty && item.HargaRekananId != null)
                    if (oldHargaKlarifikasiLanjutanRekanan != null)
                    {

                        oldHargaKlarifikasiLanjutanRekanan.harga = item.harga;
                        oldHargaKlarifikasiLanjutanRekanan.keterangan = item.keterangan;
                        ctx.SaveChanges();
                        newLstVWRKSDetailRekanan.Add(new VWRKSDetailRekanan
                        {
                            Id = ctx.RKSDetails.Where(d => d.Id == oldHargaKlarifikasiLanjutanRekanan.RKSDetailId) == null ? Guid.Empty : ctx.RKSDetails.Where(d => d.Id == oldHargaKlarifikasiLanjutanRekanan.RKSDetailId).FirstOrDefault().Id,
                            item = ctx.RKSDetails.Where(d => d.Id == oldHargaKlarifikasiLanjutanRekanan.RKSDetailId) == null ? "" : ctx.RKSDetails.Where(d => d.Id == oldHargaKlarifikasiLanjutanRekanan.RKSDetailId).FirstOrDefault().item,
                            ItemId = ctx.RKSDetails.Where(d => d.Id == oldHargaKlarifikasiLanjutanRekanan.RKSDetailId) == null ? Guid.Empty : ctx.RKSDetails.Where(d => d.Id == oldHargaKlarifikasiLanjutanRekanan.RKSDetailId).FirstOrDefault().ItemId,
                            jumlah = ctx.RKSDetails.Where(d => d.Id == oldHargaKlarifikasiLanjutanRekanan.RKSDetailId) == null ? null : ctx.RKSDetails.Where(d => d.Id == oldHargaKlarifikasiLanjutanRekanan.RKSDetailId).FirstOrDefault().jumlah,
                            satuan = ctx.RKSDetails.Where(d => d.Id == oldHargaKlarifikasiLanjutanRekanan.RKSDetailId) == null ? "" : ctx.RKSDetails.Where(d => d.Id == oldHargaKlarifikasiLanjutanRekanan.RKSDetailId).FirstOrDefault().satuan,
                            harga = oldHargaKlarifikasiLanjutanRekanan.harga,
                            HargaRekananId = oldHargaKlarifikasiLanjutanRekanan.Id,
                            keterangan = oldHargaKlarifikasiLanjutanRekanan.keterangan
                        });
                    }
                    else
                    {
                        HargaKlarifikasiLanLanjutan newHargaKlarifikasiLanjutanRekanan = new HargaKlarifikasiLanLanjutan();
                        newHargaKlarifikasiLanjutanRekanan.harga = item.harga;
                        newHargaKlarifikasiLanjutanRekanan.keterangan = item.keterangan;
                        newHargaKlarifikasiLanjutanRekanan.RKSDetailId = item.Id;
                        newHargaKlarifikasiLanjutanRekanan.VendorId = ctx.Vendors.Where(xx => xx.Owner == UserId).FirstOrDefault().Id; //3;//contoh vendor
                        ctx.HargaKlarifikasiLanLanjutans.Add(newHargaKlarifikasiLanjutanRekanan);
                        ctx.SaveChanges();
                        newLstVWRKSDetailRekanan.Add(new VWRKSDetailRekanan
                        {
                            Id = ctx.RKSDetails.Where(d => d.Id == newHargaKlarifikasiLanjutanRekanan.RKSDetailId) == null ? Guid.Empty : ctx.RKSDetails.Where(d => d.Id == newHargaKlarifikasiLanjutanRekanan.RKSDetailId).FirstOrDefault().Id,
                            item = ctx.RKSDetails.Where(d => d.Id == newHargaKlarifikasiLanjutanRekanan.RKSDetailId) == null ? "" : ctx.RKSDetails.Where(d => d.Id == newHargaKlarifikasiLanjutanRekanan.RKSDetailId).FirstOrDefault().item,
                            ItemId = ctx.RKSDetails.Where(d => d.Id == newHargaKlarifikasiLanjutanRekanan.RKSDetailId) == null ? Guid.Empty : ctx.RKSDetails.Where(d => d.Id == newHargaKlarifikasiLanjutanRekanan.RKSDetailId).FirstOrDefault().ItemId,
                            jumlah = ctx.RKSDetails.Where(d => d.Id == newHargaKlarifikasiLanjutanRekanan.RKSDetailId) == null ? null : ctx.RKSDetails.Where(d => d.Id == newHargaKlarifikasiLanjutanRekanan.RKSDetailId).FirstOrDefault().jumlah,
                            satuan = ctx.RKSDetails.Where(d => d.Id == newHargaKlarifikasiLanjutanRekanan.RKSDetailId) == null ? "" : ctx.RKSDetails.Where(d => d.Id == newHargaKlarifikasiLanjutanRekanan.RKSDetailId).FirstOrDefault().satuan,
                            harga = newHargaKlarifikasiLanjutanRekanan.harga,
                            HargaRekananId = newHargaKlarifikasiLanjutanRekanan.Id,
                            keterangan = newHargaKlarifikasiLanjutanRekanan.keterangan
                        });
                    }

                }
            }

            return newLstVWRKSDetailRekanan;
        }

        public List<VWRekananSubmitHarga> getListRekananSubmit(Guid PengadaanId, Guid UserId)
        {
            var xKandidatPengadaans = (from b in ctx.KandidatPengadaans where b.PengadaanId == PengadaanId select b).ToList();
            var xSubmitRekanan = (from b in ctx.HargaRekanans
                                  join c in ctx.RKSDetails on b.RKSDetailId equals c.Id
                                  join d in ctx.RKSHeaders on c.RKSHeaderId equals d.Id
                                  where d.PengadaanId == PengadaanId
                                  select b).Distinct().ToList();
            var xSubmitRekananAsuransi = (from b in ctx.HargaRekananAsuransis
                                          join c in ctx.BenefitRates on b.BenefitCodeId equals c.Id
                                          join d in ctx.InsuranceTarifBenefits on c.Id equals d.BenefitRateId.Id
                                          join e in ctx.InsuranceTarifs on d.DocumentId equals e.DocumentId
                                          where e.PengadaanId == PengadaanId
                                          select b).Distinct().ToList();
            List<VWRekananSubmitHarga> lstVWRekananSubmitHarga = new List<VWRekananSubmitHarga>();
            foreach (var item in xKandidatPengadaans)
            {
                VWRekananSubmitHarga mVWRekananSubmitHarga = new VWRekananSubmitHarga();
                mVWRekananSubmitHarga.VendorId = item.VendorId;
                mVWRekananSubmitHarga.status = 0;
                mVWRekananSubmitHarga.NamaVendor = ctx.Vendors.Find(item.VendorId).Nama;

                foreach (var itemx in xSubmitRekanan)
                {
                    if (itemx.VendorId == item.VendorId) mVWRekananSubmitHarga.status = 1;
                    //{
                    //     if (itemx.harga >= 0 ) mVWRekananSubmitHarga.status = 1;
                    //}
                }

                foreach (var itemc in xSubmitRekananAsuransi)
                {
                    if (itemc.VendorId == item.VendorId && (itemc.RateEncrypt != null || itemc.RateLowerLimitEncrypt != null || itemc.RateUpperLimitEncrypt != null)) mVWRekananSubmitHarga.status = 1;
                }

                lstVWRekananSubmitHarga.Add(mVWRekananSubmitHarga);
            }
            return lstVWRekananSubmitHarga;
        }

        public List<VWRekananPenilaian> getListRekananPenilaian(Guid PengadaanId, Guid UserId)
        {
            var xKandidatPengadaans = (from b in ctx.KandidatPengadaans
                                       join c in ctx.Vendors on b.VendorId equals c.Id
                                       where b.PengadaanId == PengadaanId
                                       select new VWRekananPenilaian
                                       {
                                           NamaVendor = c.Nama,
                                           VendorId = b.VendorId,
                                           NilaiKriteria = (from bb in ctx.PembobotanPengadaans
                                                            join cc in ctx.PembobotanPengadaanVendors on bb.KreteriaPembobotanId equals cc.KreteriaPembobotanId
                                                            where cc.PengadaanId == PengadaanId && cc.VendorId == b.VendorId && bb.PengadaanId == PengadaanId
                                                            select new
                                                            {
                                                                Bobot = bb.Bobot == null ? 0 : bb.Bobot.Value,
                                                                Nilai = cc.Nilai == null ? 0 : cc.Nilai.Value
                                                            }).Sum(dd => (dd.Nilai * dd.Bobot) / 100),
                                           total = (from bb in ctx.HargaRekanans
                                                    join cc in ctx.RKSDetails on bb.RKSDetailId equals cc.Id
                                                    join dd in ctx.RKSHeaders on cc.RKSHeaderId equals dd.Id
                                                    where dd.PengadaanId == PengadaanId && bb.VendorId == b.VendorId
                                                    select new item
                                                    {
                                                        harga = bb.harga,
                                                        jumlah = cc.jumlah
                                                    }).Sum(xx => xx.harga * xx.jumlah),
                                           terpilih = (from bb in ctx.PelaksanaanPemilihanKandidats
                                                       where bb.PengadaanId == PengadaanId &&
                                                       bb.VendorId == b.VendorId
                                                       select bb).FirstOrDefault() == null ? 0 : 1
                                       }).ToList();
            return xKandidatPengadaans;
        }

        public List<VWRekananPenilaian> getListRekananPenilaian2(Guid PengadaanId, Guid UserId)
        {
            var xKandidatPengadaans = (from b in ctx.PemenangPengadaans
                                       join c in ctx.Vendors on b.VendorId equals c.Id
                                       where b.PengadaanId == PengadaanId
                                       select new VWRekananPenilaian
                                       {
                                           NamaVendor = c.Nama,
                                           VendorId = b.VendorId,
                                           NilaiKriteria = (from bb in ctx.PembobotanPengadaans
                                                            join cc in ctx.PembobotanPengadaanVendors on bb.KreteriaPembobotanId equals cc.KreteriaPembobotanId
                                                            where cc.PengadaanId == PengadaanId && cc.VendorId == b.VendorId && bb.PengadaanId == PengadaanId
                                                            select new
                                                            {
                                                                Bobot = bb.Bobot == null ? 0 : bb.Bobot.Value,
                                                                Nilai = cc.Nilai == null ? 0 : cc.Nilai.Value
                                                            }).Sum(dd => (dd.Nilai * dd.Bobot) / 100),
                                           //total = (from bb in ctx.HargaRekanans
                                           //         join cc in ctx.RKSDetails on bb.RKSDetailId equals cc.Id
                                           //         join dd in ctx.RKSHeaders on cc.RKSHeaderId equals dd.Id
                                           //         where dd.PengadaanId == PengadaanId && bb.VendorId == b.VendorId
                                           //         select new item
                                           //         {
                                           //             harga = bb.harga,
                                           //             jumlah = cc.jumlah
                                           //         }).Sum(xx => xx.harga * xx.jumlah),
                                           terpilih = (from bb in ctx.PelaksanaanPemilihanKandidats
                                                       where bb.PengadaanId == PengadaanId &&
                                                       bb.VendorId == b.VendorId
                                                       select bb).FirstOrDefault() == null ? 0 : 1,
                                           TotalPenilaian = (ctx.TenderScoringHeaders.Where(x => x.VendorId == b.VendorId && x.PengadaanId == PengadaanId).Select(x => x.Average).FirstOrDefault())
                                       }).ToList();
            var cekKlrasfikasiLanjutanx = cekKlarifikasiLanjut(PengadaanId);
            foreach (var item in xKandidatPengadaans)
            {
                if (!cekKlrasfikasiLanjutanx)
                {
                    item.total = (from bb in ctx.HargaKlarifikasiRekanans
                                  join cc in ctx.RKSDetails on bb.RKSDetailId equals cc.Id
                                  join dd in ctx.RKSHeaders on cc.RKSHeaderId equals dd.Id
                                  where dd.PengadaanId == PengadaanId && bb.VendorId == item.VendorId
                                  select new item
                                  {
                                      harga = bb.harga,
                                      jumlah = cc.jumlah
                                  }).Sum(xx => xx.harga == null ? 0 : xx.harga * xx.jumlah);
                }
                else
                {
                    item.total = (from bb in ctx.HargaKlarifikasiLanLanjutans
                                  join cc in ctx.RKSDetails on bb.RKSDetailId equals cc.Id
                                  join dd in ctx.RKSHeaders on cc.RKSHeaderId equals dd.Id
                                  where dd.PengadaanId == PengadaanId && bb.VendorId == item.VendorId
                                  select new item
                                  {
                                      harga = bb.harga,
                                      jumlah = cc.jumlah
                                  }).Sum(xx => xx.harga == null ? 0 : xx.harga * xx.jumlah);
                }
            }

            return xKandidatPengadaans;
        }

        public List<VWRekananPenilaian> getListPenilaianByVendor(Guid PengadaanId, Guid UserId, int VendorId)
        {
            var xKandidatPengadaans = (from b in ctx.KandidatPengadaans
                                       join c in ctx.Vendors on b.VendorId equals c.Id
                                       where b.PengadaanId == PengadaanId & b.VendorId == VendorId
                                       select new VWRekananPenilaian
                                       {
                                           NamaVendor = c.Nama,
                                           VendorId = b.VendorId,
                                           NilaiKriteria = (from bb in ctx.PembobotanPengadaans
                                                            join cc in ctx.PembobotanPengadaanVendors on bb.KreteriaPembobotanId equals cc.KreteriaPembobotanId
                                                            where cc.PengadaanId == PengadaanId && cc.VendorId == b.VendorId && bb.PengadaanId == PengadaanId
                                                            select new
                                                            {
                                                                Bobot = bb.Bobot == null ? 0 : bb.Bobot.Value,
                                                                Nilai = cc.Nilai == null ? 0 : cc.Nilai.Value
                                                            }).Sum(dd => (dd.Nilai * dd.Bobot) / 100),
                                           total = (from bb in ctx.HargaRekanans
                                                    join cc in ctx.RKSDetails on bb.RKSDetailId equals cc.Id
                                                    join dd in ctx.RKSHeaders on cc.RKSHeaderId equals dd.Id
                                                    where dd.PengadaanId == PengadaanId && bb.VendorId == b.VendorId
                                                    select new item
                                                    {
                                                        harga = bb.harga,
                                                        jumlah = cc.jumlah
                                                    }).Sum(xx => xx.harga * xx.jumlah),
                                           terpilih = (from bb in ctx.PelaksanaanPemilihanKandidats
                                                       where bb.PengadaanId == PengadaanId &&
                                                       bb.VendorId == b.VendorId
                                                       select bb).FirstOrDefault() == null ? 0 : 1
                                       }).ToList();
            return xKandidatPengadaans;
        }

        public List<VWRekananSubmitHarga> getListRekananKlarifikasiSubmit(Guid PengadaanId, Guid UserId)
        {
            var xRekanan = (from b in ctx.KandidatPengadaans
                            where b.PengadaanId == PengadaanId
                            select b).Distinct().ToList();

            var xSubmitRekanan = (from b in ctx.HargaKlarifikasiRekanans
                                  join c in ctx.RKSDetails on b.RKSDetailId equals c.Id
                                  join d in ctx.RKSHeaders on c.RKSHeaderId equals d.Id
                                  where d.PengadaanId == PengadaanId
                                  select b).Distinct().ToList();

            var xSubmitRekananAsuransi = (from b in ctx.HargaKlarifikasiRekananAsuransis
                                          join c in ctx.BenefitRates on b.BenefitCodeId equals c.Id
                                          join d in ctx.InsuranceTarifBenefits on c.Id equals d.BenefitRateId.Id
                                          join e in ctx.InsuranceTarifs on d.DocumentId equals e.DocumentId
                                          where e.PengadaanId == PengadaanId
                                          select b).Distinct().ToList();

            var LstVendor = xRekanan.Select(d => new { VendorId = d.VendorId }).Distinct().ToList();
            List<VWRekananSubmitHarga> lstVWRekananSubmitHarga = new List<VWRekananSubmitHarga>();
            foreach (var item in LstVendor)
            {
                VWRekananSubmitHarga mVWRekananSubmitHarga = new VWRekananSubmitHarga();
                mVWRekananSubmitHarga.VendorId = item.VendorId;
                mVWRekananSubmitHarga.status = 0;
                mVWRekananSubmitHarga.NamaVendor = ctx.Vendors.Find(item.VendorId).Nama;

                foreach (var itemx in xSubmitRekanan)
                {
                    //if (itemx.VendorId == item.VendorId && itemx.harga > 0 ) mVWRekananSubmitHarga.status = 1;
                    if (itemx.VendorId == item.VendorId)
                    {
                        if (itemx.harga > 0) mVWRekananSubmitHarga.status = 1;
                    }
                }

                foreach (var itemc in xSubmitRekananAsuransi)
                {
                    if (itemc.VendorId == item.VendorId && (itemc.Rate != null || itemc.RateLowerLimit != null || itemc.RateUpperLimit != null)) mVWRekananSubmitHarga.status = 1;
                }
                lstVWRekananSubmitHarga.Add(mVWRekananSubmitHarga);
            }
            return lstVWRekananSubmitHarga;
        }

        public List<VWRekananSubmitHarga> getListRekananKlarifikasiLanjutSubmit(Guid PengadaanId, Guid UserId)
        {
            var xRekanan = (from b in ctx.PemenangPengadaans
                            where b.PengadaanId == PengadaanId
                            select b).Distinct().ToList();

            var xSubmitRekanan = (from b in ctx.HargaKlarifikasiLanLanjutans
                                  join c in ctx.RKSDetails on b.RKSDetailId equals c.Id
                                  join d in ctx.RKSHeaders on c.RKSHeaderId equals d.Id
                                  where d.PengadaanId == PengadaanId
                                  select b).Distinct().ToList();

            var xSubmitRekananAsuransi = (from b in ctx.HargaKlarifikasiLanjutanAsuransis
                                          join c in ctx.BenefitRates on b.BenefitCodeId equals c.Id
                                          join d in ctx.InsuranceTarifBenefits on c.Id equals d.BenefitRateId.Id
                                          join e in ctx.InsuranceTarifs on d.DocumentId equals e.DocumentId
                                          where e.PengadaanId == PengadaanId
                                          select b).Distinct().ToList();

            var LstVendor = xRekanan.Select(d => new { VendorId = d.VendorId }).Distinct().ToList();
            List<VWRekananSubmitHarga> lstVWRekananSubmitHarga = new List<VWRekananSubmitHarga>();
            foreach (var item in LstVendor)
            {
                VWRekananSubmitHarga mVWRekananSubmitHarga = new VWRekananSubmitHarga();
                mVWRekananSubmitHarga.VendorId = item.VendorId;
                mVWRekananSubmitHarga.status = 0;
                mVWRekananSubmitHarga.NamaVendor = ctx.Vendors.Find(item.VendorId).Nama;

                foreach (var itemx in xSubmitRekanan)
                {
                    if (itemx.VendorId == item.VendorId)
                    {
                        if (itemx.harga > 0) mVWRekananSubmitHarga.status = 1;
                    }
                }

                foreach (var itemc in xSubmitRekananAsuransi)
                {
                    if (itemc.VendorId == item.VendorId && (itemc.Rate != null || itemc.RateLowerLimit != null || itemc.RateUpperLimit != null)) mVWRekananSubmitHarga.status = 1;
                }
                lstVWRekananSubmitHarga.Add(mVWRekananSubmitHarga);
            }
            return lstVWRekananSubmitHarga;
        }

        public List<VWRekananPenilaian> getListRekananKlarifikasiPenilaian(Guid PengadaanId, Guid UserId)
        {
            var xKandidatPengadaans = (from b in ctx.KandidatPengadaans
                                       join c in ctx.Vendors on b.VendorId equals c.Id
                                       where b.PengadaanId == PengadaanId
                                       select new VWRekananPenilaian
                                       {
                                           NamaVendor = c.Nama,
                                           VendorId = b.VendorId,
                                           total = (from bb in ctx.HargaKlarifikasiRekanans
                                                    join cc in ctx.RKSDetails on bb.RKSDetailId equals cc.Id
                                                    join dd in ctx.RKSHeaders on cc.RKSHeaderId equals dd.Id
                                                    where dd.PengadaanId == PengadaanId && bb.VendorId == b.VendorId
                                                    select new item
                                                    {
                                                        harga = bb.harga,
                                                        jumlah = cc.jumlah
                                                    }).Sum(xx => xx.harga * xx.jumlah),
                                           terpilih = (from bb in ctx.PemenangPengadaans
                                                       where bb.PengadaanId == PengadaanId &&
                                                       bb.VendorId == b.VendorId
                                                       select bb).FirstOrDefault() == null ? 0 : 1,
                                           NilaiKriteria = (from bb in ctx.PembobotanPengadaans
                                                            join cc in ctx.PembobotanPengadaanVendors on bb.KreteriaPembobotanId equals cc.KreteriaPembobotanId
                                                            where cc.PengadaanId == PengadaanId && cc.VendorId == b.VendorId && bb.PengadaanId == PengadaanId
                                                            select new
                                                            {
                                                                Bobot = bb.Bobot == null ? 0 : bb.Bobot.Value,
                                                                Nilai = cc.Nilai == null ? 0 : cc.Nilai.Value
                                                            }).Sum(dd => (dd.Nilai * dd.Bobot) / 100)
                                       }).ToList();
            return xKandidatPengadaans;
        }

        public List<VWRekananPenilaian> getListRekananKlarifikasiPenilaianLanjutan(Guid PengadaanId, Guid UserId)
        {
            var xKandidatPengadaans = (from b in ctx.PemenangPengadaans
                                       join c in ctx.Vendors on b.VendorId equals c.Id
                                       where b.PengadaanId == PengadaanId
                                       select new VWRekananPenilaian
                                       {
                                           NamaVendor = c.Nama,
                                           VendorId = b.VendorId,
                                           total = (from bb in ctx.HargaKlarifikasiLanLanjutans
                                                    join cc in ctx.RKSDetails on bb.RKSDetailId equals cc.Id
                                                    join dd in ctx.RKSHeaders on cc.RKSHeaderId equals dd.Id
                                                    where dd.PengadaanId == PengadaanId && bb.VendorId == b.VendorId
                                                    select new item
                                                    {
                                                        harga = bb.harga,
                                                        jumlah = cc.jumlah
                                                    }).Sum(xx => xx.harga * xx.jumlah),
                                           terpilih = (from bb in ctx.PemenangPengadaans
                                                       where bb.PengadaanId == PengadaanId &&
                                                       bb.VendorId == b.VendorId
                                                       select bb).FirstOrDefault() == null ? 0 : 1,
                                           NilaiKriteria = (from bb in ctx.PembobotanPengadaans
                                                            join cc in ctx.PembobotanPengadaanVendors on bb.KreteriaPembobotanId equals cc.KreteriaPembobotanId
                                                            where cc.PengadaanId == PengadaanId && cc.VendorId == b.VendorId && bb.PengadaanId == PengadaanId
                                                            select new
                                                            {
                                                                Bobot = bb.Bobot == null ? 0 : bb.Bobot.Value,
                                                                Nilai = cc.Nilai == null ? 0 : cc.Nilai.Value
                                                            }).Sum(dd => (dd.Nilai * dd.Bobot) / 100)
                                       }).ToList();
            return xKandidatPengadaans;
        }

        public int addPemenangPengadaan(PemenangPengadaan dtPemenangPengadaan, Guid UserId)
        {
            var oPIC = ctx.PersonilPengadaans.Where(d => d.PengadaanId == dtPemenangPengadaan.PengadaanId &&
                d.tipe == PengadaanConstants.StaffPeranan.PIC && d.PersonilId == UserId).FirstOrDefault();
            if (oPIC == null) return 0;
            var oPemenangPengadaan = ctx.PemenangPengadaans.Where(d => d.PengadaanId == dtPemenangPengadaan.PengadaanId && d.VendorId == dtPemenangPengadaan.VendorId).FirstOrDefault();//

            if (oPemenangPengadaan == null)
            {
                dtPemenangPengadaan.CreatedBy = UserId;
                dtPemenangPengadaan.CreateOn = DateTime.Now;
                ctx.PemenangPengadaans.Add(dtPemenangPengadaan);
            }
            //else
            //{
            //    oPemenangPengadaan.ModifiedBy = UserId;
            //    oPemenangPengadaan.ModifiedOn = DateTime.Now;
            //    oPemenangPengadaan.VendorId = dtPemenangPengadaan.VendorId;
            //}

            try
            {
                ctx.SaveChanges(UserId.ToString());
                return 1;
            }
            catch
            {
                return 0;
            }

        }

        public Reston.Helper.Util.ResultMessage DeletePemenang(PemenangPengadaan dtPemenangPengadaan, Guid UserId)
        {
            var oPIC = ctx.PersonilPengadaans.Where(d => d.PengadaanId == dtPemenangPengadaan.PengadaanId &&
                d.tipe == PengadaanConstants.StaffPeranan.PIC && d.PersonilId == UserId).FirstOrDefault();
            if (oPIC == null) return new Reston.Helper.Util.ResultMessage();
            var oPemenangPengadaan = ctx.PemenangPengadaans.Where(d => d.PengadaanId == dtPemenangPengadaan.PengadaanId && d.VendorId == dtPemenangPengadaan.VendorId).FirstOrDefault();
            //var cekVEndor = ctx.PelaksanaanPemilihanKandidats.Where(d => d.PengadaanId == dtPemenangPengadaan.PengadaanId && dtPemenangPengadaan.VendorId == dtPemenangPengadaan.VendorId).FirstOrDefault();
            // if (cekVEndor == null) return new Reston.Helper.Util.ResultMessage() { status = HttpStatusCode.Forbidden };
            if (oPemenangPengadaan != null)
            {
                ctx.PemenangPengadaans.Remove(oPemenangPengadaan);
            }
            else return new Reston.Helper.Util.ResultMessage() { status = HttpStatusCode.NotFound };
            try
            {
                ctx.SaveChanges(UserId.ToString());
                return new Reston.Helper.Util.ResultMessage()
                {
                    Id = oPemenangPengadaan.Id.ToString(),
                    message = Common.DeleteSukses(),
                    status = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new Reston.Helper.Util.ResultMessage()
                {
                    status = HttpStatusCode.ExpectationFailed,
                    message = ex.ToString()
                };
            }

        }

        public VWRKSVendors getRKSPenilaian(Guid PengadaanId, Guid UserId)
        {
            VWRKSVendors newVWRKSVendors = new VWRKSVendors();

            List<VWRKSPenilaian> hps = (from b in ctx.RKSDetails
                                        join c in ctx.RKSHeaders on b.RKSHeaderId equals c.Id
                                        where c.PengadaanId == PengadaanId
                                        select new VWRKSPenilaian
                                        {
                                            Id = b.Id,
                                            harga = b.hps,
                                            judul = b.judul,
                                            item = b.item,
                                            jumlah = b.jumlah,
                                            satuan = b.satuan,
                                            level = b.level,
                                            grup = b.grup
                                        }).ToList();
            newVWRKSVendors.hps = hps;
            decimal? totalHps = hps.Sum(d => d.harga == null ? 0 : d.harga * d.jumlah);
            newVWRKSVendors.hps.Insert(0, new VWRKSPenilaian { item = "Total", harga = totalHps });

            List<VWVendorsHarga> vendors = new List<VWVendorsHarga>();

            var xKandidatPengadaans = (from b in ctx.KandidatPengadaans where b.PengadaanId == PengadaanId select b).ToList();
            foreach (var item in xKandidatPengadaans)
            {
                //into ps
                //                from c in ps.DefaultIfEmpty()
                VWVendorsHarga mVWVendorsHarga = new VWVendorsHarga();
                mVWVendorsHarga.nama = ctx.Vendors.Find(item.VendorId).Nama;
                mVWVendorsHarga.VendorId = item.VendorId;
                var oPembobotanPengadaanVendor = ctx.PembobotanPengadaanVendors.Where(
                                    d => d.PengadaanId == PengadaanId && d.VendorId == item.VendorId).ToList();
                var totalNilaiKirteria = 0;
                foreach (var itemKriteriaVendor in oPembobotanPengadaanVendor)
                {
                    var oPembobotanPengadaans = ctx.PembobotanPengadaans.Where(d => d.KreteriaPembobotanId == itemKriteriaVendor.KreteriaPembobotanId && d.PengadaanId == PengadaanId).FirstOrDefault();
                    var bobot = oPembobotanPengadaans == null ? 0 : oPembobotanPengadaans.Bobot == null ? 0 : oPembobotanPengadaans.Bobot.Value;
                    var nilai = itemKriteriaVendor.Nilai == null ? 0 : itemKriteriaVendor.Nilai.Value;
                    totalNilaiKirteria = totalNilaiKirteria + ((bobot * nilai) / 100);
                }
                mVWVendorsHarga.NIlaiKriteria = totalNilaiKirteria;

                mVWVendorsHarga.items = (from b in ctx.HargaRekanans
                                         join c in ctx.RKSDetails on b.RKSDetailId equals c.Id
                                         join d in ctx.RKSHeaders on c.RKSHeaderId equals d.Id
                                         where d.PengadaanId == PengadaanId && b.VendorId == item.VendorId
                                         select new item
                                         {
                                             jumlah = c.jumlah,
                                             harga = b.harga
                                         }).ToList();



                if (mVWVendorsHarga.items.Count > 0)
                {
                    decimal? total = mVWVendorsHarga.items.Sum(d => d.harga == null ? 0 : d.harga * d.jumlah);
                    mVWVendorsHarga.items.Insert(0, new item { harga = total });
                    vendors.Add(mVWVendorsHarga);
                }
            }
            newVWRKSVendors.vendors = vendors;
            return newVWRKSVendors;
        }


        public VWRKSVendorsAsuransi getRKSPenilaianAsuransi(Guid PengadaanId, Guid UserId)
        {
            VWRKSVendorsAsuransi VW = new VWRKSVendorsAsuransi();

            List<ViewBenefitRate> hps = (from a in ctx.InsuranceTarifs
                                         join b in ctx.InsuranceTarifBenefits on a.DocumentId equals b.DocumentId
                                         join c in ctx.BenefitRates on b.BenefitRateId.Id equals c.Id
                                         join d in ctx.ReferenceDatas on c.BenefitCode equals d.Code
                                         join e in ctx.ReferenceDatas on c.BenefitCoverage equals e.Code
                                         join f in ctx.ReferenceDatas on c.RegionCode equals f.Code
                                         where a.PengadaanId == PengadaanId
                                         orderby c.Id
                                         select new ViewBenefitRate
                                         {
                                             Id = c.Id,
                                             BenefitCode = d.LocalizedName,
                                             BenefitCoverage = e.LocalizedName,
                                             RegionCode = f.LocalizedName,
                                             Rate = c.Rate,
                                             RateLowerLimit = c.RateLowerLimit,
                                             RateUpperLimit = c.RateUpperLimit,
                                             IsRange = c.IsRange == null ? c.IsRange : c.IsRange,
                                             IsOpen = c.IsOpen == null ? c.IsOpen : c.IsOpen
                                         }).ToList();
            VW.hps = hps;

            List<ViewVendorBenefRate> vendors = new List<ViewVendorBenefRate>();

            var xKandidatPengadaans = (from b in ctx.KandidatPengadaans where b.PengadaanId == PengadaanId select b).ToList();

            foreach (var item in xKandidatPengadaans)
            {
                ViewVendorBenefRate it = new ViewVendorBenefRate();
                it.VendorId = item.VendorId;
                it.NamaVendor = ctx.Vendors.Find(item.VendorId).Nama;

                it.itemAsuransi = (from a in ctx.InsuranceTarifs
                                   join b in ctx.InsuranceTarifBenefits on a.DocumentId equals b.DocumentId
                                   join c in ctx.BenefitRates on b.BenefitRateId.Id equals c.Id
                                   join d in ctx.HargaRekananAsuransis on c.Id equals d.BenefitCodeId
                                   where d.VendorId == item.VendorId && a.PengadaanId == PengadaanId
                                   orderby c.Id
                                   select new ViewPenawaranVendor
                                   {
                                       Rate = d.Rate,
                                       RateLowerLimit = d.RateLowerLimit,
                                       RateUpperLimit = d.RateUpperLimit
                                   }).ToList();

                it.DokumenVendor = (from a in ctx.DokumenPengadaans
                                    where a.PengadaanId == PengadaanId && a.Tipe == TipeBerkas.BerkasRekanan && a.VendorId == it.VendorId
                                    select new ViewDokumenVendor
                                    {
                                        IdDokumen = a.Id,
                                    }).ToList();

                if (it.itemAsuransi.Count > 0)
                {
                    vendors.Add(it);
                }
            }
            VW.vendors = vendors;
            return VW;
        }

        public VWRKSVendorsAsuransi getRKSKlarifikasiAsuransi(Guid PengadaanId, Guid UserId)
        {
            VWRKSVendorsAsuransi VW = new VWRKSVendorsAsuransi();

            List<ViewBenefitRate> hps = (from a in ctx.InsuranceTarifs
                                         join b in ctx.InsuranceTarifBenefits on a.DocumentId equals b.DocumentId
                                         join c in ctx.BenefitRates on b.BenefitRateId.Id equals c.Id
                                         join d in ctx.ReferenceDatas on c.BenefitCode equals d.Code
                                         join e in ctx.ReferenceDatas on c.BenefitCoverage equals e.Code
                                         join f in ctx.ReferenceDatas on c.RegionCode equals f.Code
                                         where a.PengadaanId == PengadaanId
                                         orderby c.Id
                                         select new ViewBenefitRate
                                         {
                                             Id = c.Id,
                                             BenefitCode = d.LocalizedName,
                                             BenefitCoverage = e.LocalizedName,
                                             RegionCode = f.LocalizedName,
                                             Rate = c.Rate,
                                             RateLowerLimit = c.RateLowerLimit,
                                             RateUpperLimit = c.RateUpperLimit,
                                             IsRange = c.IsRange == null ? c.IsRange : c.IsRange,
                                             IsOpen = c.IsOpen == null ? c.IsOpen : c.IsOpen
                                         }).ToList();
            VW.hps = hps;

            List<ViewVendorBenefRate> vendors = new List<ViewVendorBenefRate>();

            var xKandidatPengadaans = (from b in ctx.KandidatPengadaans where b.PengadaanId == PengadaanId select b).ToList();

            foreach (var item in xKandidatPengadaans)
            {
                ViewVendorBenefRate it = new ViewVendorBenefRate();
                it.VendorId = item.VendorId;
                it.NamaVendor = ctx.Vendors.Find(item.VendorId).Nama;

                it.itemAsuransi = (from a in ctx.InsuranceTarifs
                                   join b in ctx.InsuranceTarifBenefits on a.DocumentId equals b.DocumentId
                                   join c in ctx.BenefitRates on b.BenefitRateId.Id equals c.Id
                                   join d in ctx.HargaKlarifikasiRekananAsuransis on c.Id equals d.BenefitCodeId
                                   where d.VendorId == item.VendorId && a.PengadaanId == PengadaanId
                                   orderby c.Id
                                   select new ViewPenawaranVendor
                                   {
                                       Rate = d.Rate,
                                       RateLowerLimit = d.RateLowerLimit,
                                       RateUpperLimit = d.RateUpperLimit
                                   }).ToList();

                it.DokumenVendor = (from a in ctx.DokumenPengadaans
                                    where a.PengadaanId == PengadaanId && a.Tipe == TipeBerkas.BerkasRekananKlarifikasi && a.VendorId == it.VendorId
                                    select new ViewDokumenVendor
                                    {
                                        IdDokumen = a.Id,
                                    }).ToList();

                if (it.itemAsuransi.Count > 0)
                {
                    vendors.Add(it);
                }
            }
            VW.vendors = vendors;
            return VW;
        }
        public VWRKSVendorsAsuransi getRKSPenilaianKlarifikasiLanjutanAsuransi(Guid PengadaanId, Guid UserId)
        {
            VWRKSVendorsAsuransi VW = new VWRKSVendorsAsuransi();

            List<ViewBenefitRate> hps = (from a in ctx.InsuranceTarifs
                                         join b in ctx.InsuranceTarifBenefits on a.DocumentId equals b.DocumentId
                                         join c in ctx.BenefitRates on b.BenefitRateId.Id equals c.Id
                                         join d in ctx.ReferenceDatas on c.BenefitCode equals d.Code
                                         join e in ctx.ReferenceDatas on c.BenefitCoverage equals e.Code
                                         join f in ctx.ReferenceDatas on c.RegionCode equals f.Code
                                         where a.PengadaanId == PengadaanId
                                         orderby c.Id
                                         select new ViewBenefitRate
                                         {
                                             Id = c.Id,
                                             BenefitCode = d.LocalizedName,
                                             BenefitCoverage = e.LocalizedName,
                                             RegionCode = f.LocalizedName,
                                             Rate = c.Rate,
                                             RateLowerLimit = c.RateLowerLimit,
                                             RateUpperLimit = c.RateUpperLimit,
                                             IsRange = c.IsRange == null ? c.IsRange : c.IsRange,
                                             IsOpen = c.IsOpen == null ? c.IsOpen : c.IsOpen
                                         }).ToList();
            VW.hps = hps;

            List<ViewVendorBenefRate> vendors = new List<ViewVendorBenefRate>();

            var xKandidatPengadaans = (from b in ctx.KandidatPengadaans where b.PengadaanId == PengadaanId select b).ToList();

            foreach (var item in xKandidatPengadaans)
            {
                ViewVendorBenefRate it = new ViewVendorBenefRate();
                it.VendorId = item.VendorId;
                it.NamaVendor = ctx.Vendors.Find(item.VendorId).Nama;

                it.itemAsuransi = (from a in ctx.InsuranceTarifs
                                   join b in ctx.InsuranceTarifBenefits on a.DocumentId equals b.DocumentId
                                   join c in ctx.BenefitRates on b.BenefitRateId.Id equals c.Id
                                   join d in ctx.HargaKlarifikasiLanjutanAsuransis on c.Id equals d.BenefitCodeId
                                   where d.VendorId == item.VendorId && a.PengadaanId == PengadaanId
                                   orderby c.Id
                                   select new ViewPenawaranVendor
                                   {
                                       Rate = d.Rate,
                                       RateLowerLimit = d.RateLowerLimit,
                                       RateUpperLimit = d.RateUpperLimit
                                   }).ToList();

                it.DokumenVendor = (from a in ctx.DokumenPengadaans
                                    where a.PengadaanId == PengadaanId && a.Tipe == TipeBerkas.BerkasRekananKlarifikasiLanjutan && a.VendorId == it.VendorId
                                    select new ViewDokumenVendor
                                    {
                                        IdDokumen = a.Id,
                                    }).ToList();

                if (it.itemAsuransi.Count > 0)
                {
                    vendors.Add(it);
                }
            }
            VW.vendors = vendors;
            return VW;
        }

        public VWRKSVendorsAsuransi getRKSPenilaianAsuransiNilai(Guid PengadaanId, Guid UserId)
        {
            VWRKSVendorsAsuransi VW = new VWRKSVendorsAsuransi();

            List<ViewBenefitRate> hps = (from a in ctx.InsuranceTarifs
                                         join b in ctx.InsuranceTarifBenefits on a.DocumentId equals b.DocumentId
                                         join c in ctx.BenefitRates on b.BenefitRateId.Id equals c.Id
                                         join d in ctx.ReferenceDatas on c.BenefitCode equals d.Code
                                         join e in ctx.ReferenceDatas on c.BenefitCoverage equals e.Code
                                         join f in ctx.ReferenceDatas on c.RegionCode equals f.Code
                                         where a.PengadaanId == PengadaanId
                                         orderby c.Id
                                         select new ViewBenefitRate
                                         {
                                             Id = c.Id,
                                             BenefitCode = d.LocalizedName,
                                             BenefitCoverage = e.LocalizedName,
                                             RegionCode = f.LocalizedName,
                                             Rate = c.Rate,
                                             RateLowerLimit = c.RateLowerLimit,
                                             RateUpperLimit = c.RateUpperLimit,
                                             IsRange = c.IsRange == null ? c.IsRange : c.IsRange,
                                             IsOpen = c.IsOpen == null ? c.IsOpen : c.IsOpen
                                         }).ToList();
            VW.hps = hps;

            List<ViewVendorBenefRate> vendors = new List<ViewVendorBenefRate>();

            var xKandidatPengadaans = (from b in ctx.KandidatPengadaans where b.PengadaanId == PengadaanId select b).ToList();

            foreach (var item in xKandidatPengadaans)
            {
                ViewVendorBenefRate it = new ViewVendorBenefRate();
                it.VendorId = item.VendorId;
                it.NamaVendor = ctx.Vendors.Find(item.VendorId).Nama;

                var nilai = (from a in ctx.InsuranceTarifs
                             join b in ctx.InsuranceTarifBenefits on a.DocumentId equals b.DocumentId
                             join c in ctx.BenefitRates on b.BenefitRateId.Id equals c.Id
                             join d in ctx.HargaKlarifikasiLanjutanAsuransis on c.Id equals d.BenefitCodeId
                             where d.VendorId == item.VendorId && a.PengadaanId == PengadaanId
                             orderby c.Id
                             select new ViewPenawaranVendor
                             {
                                 Rate = d.Rate,
                                 RateLowerLimit = d.RateLowerLimit,
                                 RateUpperLimit = d.RateUpperLimit
                             }).ToList();

                var dokv = (from a in ctx.DokumenPengadaans
                            where a.PengadaanId == PengadaanId && a.Tipe == TipeBerkas.BerkasRekananKlarifikasiLanjutan && a.VendorId == it.VendorId
                            select new ViewDokumenVendor
                            {
                                IdDokumen = a.Id,
                            }).ToList();

                if (nilai == null)
                {
                    it.itemAsuransi = (from a in ctx.InsuranceTarifs
                                       join b in ctx.InsuranceTarifBenefits on a.DocumentId equals b.DocumentId
                                       join c in ctx.BenefitRates on b.BenefitRateId.Id equals c.Id
                                       join d in ctx.HargaKlarifikasiRekananAsuransis on c.Id equals d.BenefitCodeId
                                       where d.VendorId == item.VendorId && a.PengadaanId == PengadaanId
                                       orderby c.Id
                                       select new ViewPenawaranVendor
                                       {
                                           Rate = d.Rate,
                                           RateLowerLimit = d.RateLowerLimit,
                                           RateUpperLimit = d.RateUpperLimit
                                       }).ToList();
                    it.DokumenVendor = (from a in ctx.DokumenPengadaans
                                        where a.PengadaanId == PengadaanId && a.Tipe == TipeBerkas.BerkasRekananKlarifikasiLanjutan && a.VendorId == it.VendorId
                                        select new ViewDokumenVendor
                                        {
                                            IdDokumen = a.Id,
                                        }).ToList();
                }
                else
                {
                    it.itemAsuransi = nilai;
                    it.DokumenVendor = dokv;
                }

                if (it.itemAsuransi.Count > 0)
                {
                    vendors.Add(it);
                }
            }
            VW.vendors = vendors;
            return VW;
        }

        public VWRKSVendors getRKSPenilaian2(Guid PengadaanId, Guid UserId)
        {
            VWRKSVendors newVWRKSVendors = new VWRKSVendors();

            List<VWRKSPenilaian> hps = (from b in ctx.RKSDetails
                                        join c in ctx.RKSHeaders on b.RKSHeaderId equals c.Id
                                        where c.PengadaanId == PengadaanId
                                        select new VWRKSPenilaian
                                        {
                                            Id = b.Id,
                                            harga = b.hps,
                                            judul = b.judul,
                                            item = b.item,
                                            jumlah = b.jumlah,
                                            satuan = b.satuan,
                                            level = b.level,
                                            grup = b.grup
                                        }).ToList();
            newVWRKSVendors.hps = hps;
            decimal? totalHps = hps.Sum(d => d.harga == null ? 0 : d.harga * d.jumlah);
            newVWRKSVendors.hps.Insert(0, new VWRKSPenilaian { item = "Total", harga = totalHps });

            List<VWVendorsHarga> vendors = new List<VWVendorsHarga>();

            var xKandidatPengadaans = (from b in ctx.KandidatPengadaans where b.PengadaanId == PengadaanId select b).ToList();
            foreach (var item in xKandidatPengadaans)
            {
                //into ps
                //                from c in ps.DefaultIfEmpty()
                VWVendorsHarga mVWVendorsHarga = new VWVendorsHarga();
                mVWVendorsHarga.nama = ctx.Vendors.Find(item.VendorId).Nama;
                mVWVendorsHarga.VendorId = item.VendorId;
                var oPembobotanPengadaanVendor = ctx.PembobotanPengadaanVendors.Where(
                                    d => d.PengadaanId == PengadaanId && d.VendorId == item.VendorId).ToList();
                var totalNilaiKirteria = 0;
                foreach (var itemKriteriaVendor in oPembobotanPengadaanVendor)
                {
                    var oPembobotanPengadaans = ctx.PembobotanPengadaans.Where(d => d.KreteriaPembobotanId == itemKriteriaVendor.KreteriaPembobotanId && d.PengadaanId == PengadaanId).FirstOrDefault();
                    var bobot = oPembobotanPengadaans == null ? 0 : oPembobotanPengadaans.Bobot == null ? 0 : oPembobotanPengadaans.Bobot.Value;
                    var nilai = itemKriteriaVendor.Nilai == null ? 0 : itemKriteriaVendor.Nilai.Value;
                    totalNilaiKirteria = totalNilaiKirteria + ((bobot * nilai) / 100);
                }
                mVWVendorsHarga.NIlaiKriteria = totalNilaiKirteria;

                if (cekKlarifikasiLanjut(PengadaanId))
                {
                    mVWVendorsHarga.items = (from b in ctx.HargaKlarifikasiLanLanjutans
                                             join c in ctx.RKSDetails on b.RKSDetailId equals c.Id
                                             join d in ctx.RKSHeaders on c.RKSHeaderId equals d.Id
                                             where d.PengadaanId == PengadaanId && b.VendorId == item.VendorId
                                             select new item
                                             {
                                                 jumlah = c.jumlah,
                                                 harga = b.harga
                                             }).ToList();
                }
                else
                {
                    mVWVendorsHarga.items = (from b in ctx.HargaKlarifikasiRekanans
                                             join c in ctx.RKSDetails on b.RKSDetailId equals c.Id
                                             join d in ctx.RKSHeaders on c.RKSHeaderId equals d.Id
                                             where d.PengadaanId == PengadaanId && b.VendorId == item.VendorId
                                             select new item
                                             {
                                                 jumlah = c.jumlah,
                                                 harga = b.harga
                                             }).ToList();
                }

                if (mVWVendorsHarga.items.Count > 0)
                {
                    decimal? total = mVWVendorsHarga.items.Sum(d => d.harga == null ? 0 : d.harga * d.jumlah);
                    mVWVendorsHarga.items.Insert(0, new item { harga = total });
                    vendors.Add(mVWVendorsHarga);
                }
            }
            newVWRKSVendors.vendors = vendors;
            return newVWRKSVendors;
        }

        private bool cekKlarifikasiLanjut(Guid PengadaanId)
        {
            if (ctx.HargaKlarifikasiLanLanjutans.Where(d => d.RKSDetail.RKSHeader.PengadaanId == PengadaanId).Count() > 0)
                return true;
            else return false;
        }

        public VWRKSVendors getRKSPenilaian2Report(Guid PengadaanId, Guid UserId)
        {
            VWRKSVendors newVWRKSVendors = new VWRKSVendors();

            List<VWRKSPenilaian> hps = (from b in ctx.RKSDetails
                                        join c in ctx.RKSHeaders on b.RKSHeaderId equals c.Id
                                        where c.PengadaanId == PengadaanId
                                        select new VWRKSPenilaian
                                        {
                                            Id = b.Id,
                                            harga = b.hps,
                                            item = b.item,
                                            keteranganItem = b.keterangan,
                                            jumlah = b.jumlah,
                                            satuan = b.satuan,
                                            level = b.level,
                                            grup = b.grup,
                                            judul = b.judul
                                        }).ToList();
            newVWRKSVendors.hps = hps;
            decimal? totalHps = hps.Sum(d => d.harga * d.jumlah);
            newVWRKSVendors.hps.Add(new VWRKSPenilaian { keteranganItem = "Total", harga = totalHps, isTotal = 1 });

            List<VWVendorsHarga> vendors = new List<VWVendorsHarga>();

            var xKandidatPengadaans = (from b in ctx.KandidatPengadaans where b.PengadaanId == PengadaanId select b).ToList();

            foreach (var item in xKandidatPengadaans)
            {
                //into ps
                //                from c in ps.DefaultIfEmpty()
                VWVendorsHarga mVWVendorsHarga = new VWVendorsHarga();
                mVWVendorsHarga.nama = ctx.Vendors.Find(item.VendorId).Nama;
                mVWVendorsHarga.VendorId = item.VendorId;
                var oPembobotanPengadaanVendor = ctx.PembobotanPengadaanVendors.Where(
                                    d => d.PengadaanId == PengadaanId && d.VendorId == item.VendorId).ToList();
                var totalNilaiKirteria = 0;
                foreach (var itemKriteriaVendor in oPembobotanPengadaanVendor)
                {
                    var oPembobotanPengadaans = ctx.PembobotanPengadaans.Where(d => d.KreteriaPembobotanId == itemKriteriaVendor.KreteriaPembobotanId && d.PengadaanId == PengadaanId).FirstOrDefault();
                    var bobot = oPembobotanPengadaans == null ? 0 : oPembobotanPengadaans.Bobot == null ? 0 : oPembobotanPengadaans.Bobot.Value;
                    var nilai = itemKriteriaVendor.Nilai == null ? 0 : itemKriteriaVendor.Nilai.Value;
                    totalNilaiKirteria = totalNilaiKirteria + ((bobot * nilai) / 100);
                }
                mVWVendorsHarga.NIlaiKriteria = totalNilaiKirteria;

                mVWVendorsHarga.items = (from b in ctx.HargaRekanans
                                         join c in ctx.RKSDetails on b.RKSDetailId equals c.Id
                                         join d in ctx.RKSHeaders on c.RKSHeaderId equals d.Id
                                         where d.PengadaanId == PengadaanId && b.VendorId == item.VendorId
                                         select new item
                                         {
                                             Id = c.Id,
                                             jumlah = c.jumlah,
                                             harga = b.harga,
                                             grup = c.grup
                                         }).ToList();

                foreach (var xitem in mVWVendorsHarga.items)
                {
                    subtotal xsubtotal = new subtotal();
                    if (mVWVendorsHarga.items.IndexOf(xitem) < mVWVendorsHarga.items.Count() - 1)
                    {
                        if (mVWVendorsHarga.items[mVWVendorsHarga.items.IndexOf(xitem) + 1].grup != xitem.grup)
                        {
                            xsubtotal.rksGroup = xitem.grup;
                            xsubtotal.totalGroup = mVWVendorsHarga.items.Where(d => d.grup == xitem.grup).Sum(d => d.harga * d.jumlah);
                            xitem.subtotal = xsubtotal;
                        }
                    }
                    else
                    {
                        xsubtotal.rksGroup = xitem.grup;
                        xsubtotal.totalGroup = mVWVendorsHarga.items.Where(d => d.grup == xitem.grup).Sum(d => d.harga * d.jumlah);
                        xitem.subtotal = xsubtotal;
                    }
                }

                if (mVWVendorsHarga.items.Count > 0)
                {
                    decimal? total = mVWVendorsHarga.items.Sum(d => d.harga * d.jumlah);
                    mVWVendorsHarga.items.Add(new item { harga = total, isTotal = 1 });
                }
                vendors.Add(mVWVendorsHarga);
            }

            newVWRKSVendors.vendors = vendors;
            return newVWRKSVendors;
        }

        public VWRKSVendors getRKSKlarifikasi(Guid PengadaanId, Guid UserId)
        {
            VWRKSVendors newVWRKSVendors = new VWRKSVendors();

            List<VWRKSPenilaian> hps = (from b in ctx.RKSDetails
                                        join c in ctx.RKSHeaders on b.RKSHeaderId equals c.Id
                                        where c.PengadaanId == PengadaanId
                                        select new VWRKSPenilaian
                                        {
                                            Id = b.Id,
                                            harga = b.hps,
                                            item = b.item,
                                            jumlah = b.jumlah,
                                            satuan = b.satuan,
                                            grup = b.grup,
                                            level = b.level,
                                            judul = b.judul
                                        }).ToList();
            newVWRKSVendors.hps = hps;
            decimal? totalHps = hps.Sum(d => d.harga * d.jumlah);
            newVWRKSVendors.hps.Insert(0, new VWRKSPenilaian { item = "Total", harga = totalHps });

            List<VWVendorsHarga> vendors = new List<VWVendorsHarga>();

            //var xKandidatPengadaans = (from b in ctx.PelaksanaanPemilihanKandidats
            //                           where b.PengadaanId == PengadaanId
            //                           select b).Distinct().ToList();
            var xKandidatPengadaans = (from b in ctx.HargaKlarifikasiRekanans
                                       where b.RKSDetail.RKSHeader.PengadaanId == PengadaanId
                                       select b.VendorId).Distinct().ToList();
            foreach (var item in xKandidatPengadaans)
            {
                //into ps
                //                from c in ps.DefaultIfEmpty()
                VWVendorsHarga mVWVendorsHarga = new VWVendorsHarga();
                mVWVendorsHarga.nama = ctx.Vendors.Find(item.Value).Nama;

                mVWVendorsHarga.items = (from b in ctx.HargaKlarifikasiRekanans
                                         join c in ctx.RKSDetails on b.RKSDetailId equals c.Id
                                         join d in ctx.RKSHeaders on c.RKSHeaderId equals d.Id
                                         where d.PengadaanId == PengadaanId && b.VendorId == item.Value
                                         select new item
                                         {
                                             harga = b.harga,
                                             jumlah = c.jumlah
                                         }).ToList();

                var oPembobotanPengadaanVendor = ctx.PembobotanPengadaanVendors.Where(
                                   d => d.PengadaanId == PengadaanId && d.VendorId == item.Value).ToList();
                var totalNilaiKirteria = 0;
                foreach (var itemKriteriaVendor in oPembobotanPengadaanVendor)
                {
                    var oPembobotanPengadaans = ctx.PembobotanPengadaans.Where(d => d.KreteriaPembobotanId == itemKriteriaVendor.KreteriaPembobotanId && d.PengadaanId == PengadaanId).FirstOrDefault();
                    var bobot = oPembobotanPengadaans == null ? 0 : oPembobotanPengadaans.Bobot == null ? 0 : oPembobotanPengadaans.Bobot.Value;
                    var nilai = itemKriteriaVendor.Nilai == null ? 0 : itemKriteriaVendor.Nilai.Value;
                    totalNilaiKirteria = totalNilaiKirteria + ((bobot * nilai) / 100);
                }
                mVWVendorsHarga.NIlaiKriteria = totalNilaiKirteria;

                if (mVWVendorsHarga.items.Count > 0)
                {
                    decimal? total = mVWVendorsHarga.items.Sum(d => d.harga * d.jumlah);
                    mVWVendorsHarga.items.Insert(0, new item { harga = total });
                    vendors.Add(mVWVendorsHarga);
                }
            }
            newVWRKSVendors.vendors = vendors;
            return newVWRKSVendors;
        }

        public VWRKSVendors getRKSKlarifikasiLanjutan(Guid PengadaanId, Guid UserId)
        {
            VWRKSVendors newVWRKSVendors = new VWRKSVendors();

            List<VWRKSPenilaian> hps = (from b in ctx.RKSDetails
                                        join c in ctx.RKSHeaders on b.RKSHeaderId equals c.Id
                                        where c.PengadaanId == PengadaanId
                                        select new VWRKSPenilaian
                                        {
                                            Id = b.Id,
                                            harga = b.hps,
                                            item = b.item,
                                            jumlah = b.jumlah,
                                            satuan = b.satuan,
                                            grup = b.grup,
                                            level = b.level,
                                            judul = b.judul
                                        }).ToList();
            newVWRKSVendors.hps = hps;
            decimal? totalHps = hps.Sum(d => d.harga * d.jumlah);
            newVWRKSVendors.hps.Insert(0, new VWRKSPenilaian { item = "Total", harga = totalHps });

            List<VWVendorsHarga> vendors = new List<VWVendorsHarga>();

            var xKandidatPengadaans = (from b in ctx.HargaKlarifikasiLanLanjutans
                                       where b.RKSDetail.RKSHeader.PengadaanId == PengadaanId
                                       select b.VendorId).Distinct().ToList();
            foreach (var item in xKandidatPengadaans)
            {

                VWVendorsHarga mVWVendorsHarga = new VWVendorsHarga();
                mVWVendorsHarga.nama = ctx.Vendors.Find(item.Value).Nama;

                mVWVendorsHarga.items = (from b in ctx.HargaKlarifikasiLanLanjutans
                                         join c in ctx.RKSDetails on b.RKSDetailId equals c.Id
                                         join d in ctx.RKSHeaders on c.RKSHeaderId equals d.Id
                                         where d.PengadaanId == PengadaanId && b.VendorId == item.Value
                                         select new item
                                         {
                                             harga = b.harga,
                                             jumlah = c.jumlah
                                         }).ToList();

                var oPembobotanPengadaanVendor = ctx.PembobotanPengadaanVendors.Where(
                                   d => d.PengadaanId == PengadaanId && d.VendorId == item.Value).ToList();
                var totalNilaiKirteria = 0;
                foreach (var itemKriteriaVendor in oPembobotanPengadaanVendor)
                {
                    var oPembobotanPengadaans = ctx.PembobotanPengadaans.Where(d => d.KreteriaPembobotanId == itemKriteriaVendor.KreteriaPembobotanId && d.PengadaanId == PengadaanId).FirstOrDefault();
                    var bobot = oPembobotanPengadaans == null ? 0 : oPembobotanPengadaans.Bobot == null ? 0 : oPembobotanPengadaans.Bobot.Value;
                    var nilai = itemKriteriaVendor.Nilai == null ? 0 : itemKriteriaVendor.Nilai.Value;
                    totalNilaiKirteria = totalNilaiKirteria + ((bobot * nilai) / 100);
                }
                mVWVendorsHarga.NIlaiKriteria = totalNilaiKirteria;

                if (mVWVendorsHarga.items.Count > 0)
                {
                    decimal? total = mVWVendorsHarga.items.Sum(d => d.harga * d.jumlah);
                    mVWVendorsHarga.items.Insert(0, new item { harga = total });
                    vendors.Add(mVWVendorsHarga);
                }
            }
            newVWRKSVendors.vendors = vendors;
            return newVWRKSVendors;
        }

        public VWRKSVendors getRKSKlarifikasiPenilaianVendor(Guid PengadaanId, Guid UserId, int VendorId)
        {
            VWRKSVendors newVWRKSVendors = new VWRKSVendors();

            List<VWRKSPenilaian> hps = (from b in ctx.RKSDetails
                                        join c in ctx.RKSHeaders on b.RKSHeaderId equals c.Id
                                        where c.PengadaanId == PengadaanId
                                        select new VWRKSPenilaian
                                        {
                                            Id = b.Id,
                                            harga = b.hps,
                                            item = b.item,
                                            jumlah = b.jumlah,
                                            satuan = b.satuan,
                                            level = b.level,
                                            grup = b.grup,
                                            judul = b.judul
                                        }).ToList();
            newVWRKSVendors.hps = hps;
            decimal? totalHps = hps.Sum(d => d.harga * d.jumlah);
            newVWRKSVendors.hps.Insert(0, new VWRKSPenilaian { item = "Total", harga = totalHps });

            List<VWVendorsHarga> vendors = new List<VWVendorsHarga>();

            var xKandidatPengadaans = (from b in ctx.KandidatPengadaans
                                       where b.PengadaanId == PengadaanId && b.VendorId == VendorId
                                       select b).Distinct().FirstOrDefault();

            VWVendorsHarga mVWVendorsHarga = new VWVendorsHarga();
            mVWVendorsHarga.nama = ctx.Vendors.Find(xKandidatPengadaans.VendorId).Nama + " (Submit Penawaran)";
            mVWVendorsHarga.Keterangan = "Keteragan Awal";

            var oPembobotanPengadaanVendor = ctx.PembobotanPengadaanVendors.Where(
                                    d => d.PengadaanId == PengadaanId && d.VendorId == xKandidatPengadaans.VendorId).ToList();
            var totalNilaiKirteria = 0;
            foreach (var itemKriteriaVendor in oPembobotanPengadaanVendor)
            {
                var oPembobotanPengadaans = ctx.PembobotanPengadaans.Where(d => d.KreteriaPembobotanId == itemKriteriaVendor.KreteriaPembobotanId && d.PengadaanId == PengadaanId).FirstOrDefault();
                var bobot = oPembobotanPengadaans == null ? 0 : oPembobotanPengadaans.Bobot == null ? 0 : oPembobotanPengadaans.Bobot.Value;
                var nilai = itemKriteriaVendor.Nilai == null ? 0 : itemKriteriaVendor.Nilai.Value;
                totalNilaiKirteria = totalNilaiKirteria + ((bobot * nilai) / 100);
            }
            mVWVendorsHarga.NIlaiKriteria = totalNilaiKirteria;
            mVWVendorsHarga.VendorId = mVWVendorsHarga.VendorId;
            mVWVendorsHarga.items = (from b in ctx.HargaRekanans
                                     join c in ctx.RKSDetails on b.RKSDetailId equals c.Id
                                     join d in ctx.RKSHeaders on c.RKSHeaderId equals d.Id
                                     where d.PengadaanId == PengadaanId && b.VendorId == xKandidatPengadaans.VendorId
                                     select new item
                                     {
                                         Id = c.Id,
                                         Keterangan = b.keterangan.ToString(),
                                         harga = b.harga,
                                         jumlah = c.jumlah,
                                         grup = c.grup
                                     }).ToList();
            foreach (var xitem in mVWVendorsHarga.items)
            {
                subtotal xsubtotal = new subtotal();
                if (mVWVendorsHarga.items.IndexOf(xitem) < mVWVendorsHarga.items.Count() - 1)
                {
                    if (mVWVendorsHarga.items[mVWVendorsHarga.items.IndexOf(xitem) + 1].grup != xitem.grup)
                    {
                        xsubtotal.rksGroup = xitem.grup;
                        xsubtotal.totalGroup = mVWVendorsHarga.items.Where(d => d.grup == xitem.grup).Sum(d => d.harga * d.jumlah);
                        xitem.subtotal = xsubtotal;
                    }
                }
                else
                {
                    xsubtotal.rksGroup = xitem.grup;
                    xsubtotal.totalGroup = mVWVendorsHarga.items.Where(d => d.grup == xitem.grup).Sum(d => d.harga * d.jumlah);
                    xitem.subtotal = xsubtotal;
                }
            }
            if (mVWVendorsHarga.items.Count > 0)
            {
                decimal? total = mVWVendorsHarga.items.Sum(d => d.harga * d.jumlah);
                mVWVendorsHarga.items.Insert(0, new item { harga = total });
                vendors.Add(mVWVendorsHarga);
            }
            //klarifikasi harga
            mVWVendorsHarga = new VWVendorsHarga();
            mVWVendorsHarga.nama = ctx.Vendors.Find(xKandidatPengadaans.VendorId).Nama + " (Klarifikasi Harga)";
            mVWVendorsHarga.Keterangan = "Keterangan Klarifikasi";
            var oPembobotanPengadaanVendor2 = ctx.PembobotanPengadaanVendors.Where(
                                    d => d.PengadaanId == PengadaanId && d.VendorId == xKandidatPengadaans.VendorId).ToList();
            var totalNilaiKirteria2 = 0;
            foreach (var itemKriteriaVendor in oPembobotanPengadaanVendor2)
            {
                var oPembobotanPengadaans = ctx.PembobotanPengadaans.Where(d => d.KreteriaPembobotanId == itemKriteriaVendor.KreteriaPembobotanId && d.PengadaanId == PengadaanId).FirstOrDefault();
                var bobot = oPembobotanPengadaans == null ? 0 : oPembobotanPengadaans.Bobot == null ? 0 : oPembobotanPengadaans.Bobot.Value;
                var nilai = itemKriteriaVendor.Nilai == null ? 0 : itemKriteriaVendor.Nilai.Value;
                totalNilaiKirteria2 = totalNilaiKirteria2 + ((bobot * nilai) / 100);
            }
            mVWVendorsHarga.NIlaiKriteria = totalNilaiKirteria2;
            mVWVendorsHarga.VendorId = mVWVendorsHarga.VendorId;
            mVWVendorsHarga.items = (from b in ctx.HargaKlarifikasiRekanans
                                     join c in ctx.RKSDetails on b.RKSDetailId equals c.Id
                                     join d in ctx.RKSHeaders on c.RKSHeaderId equals d.Id
                                     where d.PengadaanId == PengadaanId && b.VendorId == xKandidatPengadaans.VendorId
                                     select new item
                                     {
                                         Keterangan = b.keterangan.ToString(),
                                         harga = b.harga,
                                         jumlah = c.jumlah,
                                         Id = c.Id,
                                         grup = c.grup
                                     }).ToList();
            foreach (var xitem in mVWVendorsHarga.items)
            {
                subtotal xsubtotal = new subtotal();
                if (mVWVendorsHarga.items.IndexOf(xitem) < mVWVendorsHarga.items.Count() - 1)
                {
                    if (mVWVendorsHarga.items[mVWVendorsHarga.items.IndexOf(xitem) + 1].grup != xitem.grup)
                    {
                        xsubtotal.rksGroup = xitem.grup;
                        xsubtotal.totalGroup = mVWVendorsHarga.items.Where(d => d.grup == xitem.grup).Sum(d => d.harga * d.jumlah);
                        xitem.subtotal = xsubtotal;
                    }
                }
                else
                {
                    xsubtotal.rksGroup = xitem.grup;
                    xsubtotal.totalGroup = mVWVendorsHarga.items.Where(d => d.grup == xitem.grup).Sum(d => d.harga * d.jumlah);
                    xitem.subtotal = xsubtotal;
                }
            }
            if (mVWVendorsHarga.items.Count > 0)
            {
                decimal? total = mVWVendorsHarga.items.Sum(d => d.harga * d.jumlah);
                mVWVendorsHarga.items.Insert(0, new item { harga = total, isTotal = 1 });
                vendors.Add(mVWVendorsHarga);
            }

            //klarfikasi lanjut
            mVWVendorsHarga = new VWVendorsHarga();
            mVWVendorsHarga.nama = ctx.Vendors.Find(xKandidatPengadaans.VendorId).Nama + " (Klarifikasi Lanjut Harga)";
            mVWVendorsHarga.Keterangan = "Keterangan Klarifikasi Lanjut";

            mVWVendorsHarga.NIlaiKriteria = totalNilaiKirteria2;
            mVWVendorsHarga.VendorId = mVWVendorsHarga.VendorId;
            mVWVendorsHarga.items = (from b in ctx.HargaKlarifikasiLanLanjutans
                                     join c in ctx.RKSDetails on b.RKSDetailId equals c.Id
                                     join d in ctx.RKSHeaders on c.RKSHeaderId equals d.Id
                                     where d.PengadaanId == PengadaanId && b.VendorId == xKandidatPengadaans.VendorId
                                     select new item
                                     {
                                         Keterangan = b.keterangan.ToString(),
                                         harga = b.harga,
                                         jumlah = c.jumlah,
                                         grup = c.grup,
                                         Id = c.Id
                                     }).ToList();
            foreach (var xitem in mVWVendorsHarga.items)
            {
                subtotal xsubtotal = new subtotal();
                if (mVWVendorsHarga.items.IndexOf(xitem) < mVWVendorsHarga.items.Count() - 1)
                {
                    if (mVWVendorsHarga.items[mVWVendorsHarga.items.IndexOf(xitem) + 1].grup != xitem.grup)
                    {
                        xsubtotal.rksGroup = xitem.grup;
                        xsubtotal.totalGroup = mVWVendorsHarga.items.Where(d => d.grup == xitem.grup).Sum(d => d.harga * d.jumlah);
                        xitem.subtotal = xsubtotal;
                    }
                }
                else
                {
                    xsubtotal.rksGroup = xitem.grup;
                    xsubtotal.totalGroup = mVWVendorsHarga.items.Where(d => d.grup == xitem.grup).Sum(d => d.harga * d.jumlah);
                    xitem.subtotal = xsubtotal;
                }
            }
            if (mVWVendorsHarga.items.Count > 0)
            {
                decimal? total = mVWVendorsHarga.items.Sum(d => d.harga * d.jumlah);
                mVWVendorsHarga.items.Insert(0, new item { harga = total });
                vendors.Add(mVWVendorsHarga);
            }
            newVWRKSVendors.vendors = vendors;
            return newVWRKSVendors;
        }

        public VWRKSVendors getRKSKlarifikasiPenilaian(Guid PengadaanId, Guid UserId)
        {
            VWRKSVendors newVWRKSVendors = new VWRKSVendors();

            List<VWRKSPenilaian> hps = (from b in ctx.RKSDetails
                                        join c in ctx.RKSHeaders on b.RKSHeaderId equals c.Id
                                        where c.PengadaanId == PengadaanId
                                        select new VWRKSPenilaian
                                        {
                                            Id = b.Id,
                                            harga = b.hps,
                                            item = b.item,
                                            keteranganItem = b.keterangan,
                                            jumlah = b.jumlah,
                                            satuan = b.satuan,
                                            level = b.level,
                                            grup = b.grup,
                                            judul = b.judul
                                        }).ToList();
            newVWRKSVendors.hps = hps;
            decimal? totalHps = hps.Sum(d => d.harga * d.jumlah);
            newVWRKSVendors.hps.Add(new VWRKSPenilaian { keteranganItem = "Total", harga = totalHps, isTotal = 1 });

            List<VWVendorsHarga> vendors = new List<VWVendorsHarga>();

            //var kandidatTerpilih = (from b in ctx.PelaksanaanPemilihanKandidats
            //                        where b.PengadaanId == PengadaanId
            //                        select b).ToList();
            var kandidatTerpilih = (from b in ctx.HargaKlarifikasiRekanans
                                    where b.RKSDetail.RKSHeader.PengadaanId == PengadaanId
                                    select b.VendorId).Distinct().ToList();
            var lastItem = kandidatTerpilih.Last();
            foreach (var item in kandidatTerpilih)
            {
                //var xKandidatPengadaans = (from b in ctx.PelaksanaanPemilihanKandidats
                //                           where b.PengadaanId == PengadaanId && b.VendorId == item.VendorId
                //                           select b).Distinct().FirstOrDefault();
                var xKandidatPengadaans = (from b in ctx.HargaKlarifikasiRekanans
                                           where b.RKSDetail.RKSHeader.PengadaanId == PengadaanId && b.VendorId == item.Value
                                           select b).Distinct().FirstOrDefault();

                VWVendorsHarga mVWVendorsHarga = new VWVendorsHarga();
                mVWVendorsHarga.nama = ctx.Vendors.Find(xKandidatPengadaans.VendorId).Nama + " (Submit Penawaran)";

                mVWVendorsHarga.items = (from b in ctx.HargaRekanans
                                         join c in ctx.RKSDetails on b.RKSDetailId equals c.Id
                                         join d in ctx.RKSHeaders on c.RKSHeaderId equals d.Id
                                         where d.PengadaanId == PengadaanId && b.VendorId == xKandidatPengadaans.VendorId
                                         select new item
                                         {
                                             Id = c.Id,
                                             harga = b.harga,
                                             jumlah = c.jumlah,
                                             grup = c.grup
                                         }).ToList();
                foreach (var xitem in mVWVendorsHarga.items)
                {
                    subtotal xsubtotal = new subtotal();
                    if (mVWVendorsHarga.items.IndexOf(xitem) < mVWVendorsHarga.items.Count() - 1)
                    {
                        if (mVWVendorsHarga.items[mVWVendorsHarga.items.IndexOf(xitem) + 1].grup != xitem.grup)
                        {
                            xsubtotal.rksGroup = xitem.grup;
                            xsubtotal.totalGroup = mVWVendorsHarga.items.Where(d => d.grup == xitem.grup).Sum(d => d.harga * d.jumlah);
                            xitem.subtotal = xsubtotal;
                        }
                    }
                    else
                    {
                        xsubtotal.rksGroup = xitem.grup;
                        xsubtotal.totalGroup = mVWVendorsHarga.items.Where(d => d.grup == xitem.grup).Sum(d => d.harga * d.jumlah);
                        xitem.subtotal = xsubtotal;
                    }
                }
                if (mVWVendorsHarga.items.Count > 0)
                {
                    decimal? total = mVWVendorsHarga.items.Sum(d => d.harga * d.jumlah);
                    mVWVendorsHarga.items.Add(new item { harga = total, isTotal = 1 });
                    vendors.Add(mVWVendorsHarga);
                }

                mVWVendorsHarga = new VWVendorsHarga();
                mVWVendorsHarga.nama = ctx.Vendors.Find(xKandidatPengadaans.VendorId).Nama + " (Klarifikasi Harga)";

                mVWVendorsHarga.items = (from b in ctx.HargaKlarifikasiRekanans
                                         join c in ctx.RKSDetails on b.RKSDetailId equals c.Id
                                         join d in ctx.RKSHeaders on c.RKSHeaderId equals d.Id
                                         where d.PengadaanId == PengadaanId && b.VendorId == xKandidatPengadaans.VendorId
                                         select new item
                                         {
                                             Id = c.Id,
                                             harga = b.harga,
                                             jumlah = c.jumlah,
                                             grup = c.grup
                                         }).ToList();
                foreach (var xitem in mVWVendorsHarga.items)
                {
                    subtotal xsubtotal = new subtotal();
                    if (mVWVendorsHarga.items.IndexOf(xitem) < mVWVendorsHarga.items.Count() - 1)
                    {
                        if (mVWVendorsHarga.items[mVWVendorsHarga.items.IndexOf(xitem) + 1].grup != xitem.grup)
                        {
                            xsubtotal.rksGroup = xitem.grup;
                            xsubtotal.totalGroup = mVWVendorsHarga.items.Where(d => d.grup == xitem.grup).Sum(d => d.harga * d.jumlah);
                            xitem.subtotal = xsubtotal;
                        }
                    }
                    else
                    {
                        xsubtotal.rksGroup = xitem.grup;
                        xsubtotal.totalGroup = mVWVendorsHarga.items.Where(d => d.grup == xitem.grup).Sum(d => d.harga * d.jumlah);
                        xitem.subtotal = xsubtotal;
                    }
                }
                if (mVWVendorsHarga.items.Count > 0)
                {
                    decimal? total = mVWVendorsHarga.items.Sum(d => d.harga * d.jumlah);
                    mVWVendorsHarga.items.Add(new item { harga = total, isTotal = 1 });
                    vendors.Add(mVWVendorsHarga);
                }

            }

            newVWRKSVendors.vendors = vendors;
            return newVWRKSVendors;
        }

        public VWRKSVendors getRKSKlarifikasiLanjutanPenilaian(Guid PengadaanId, Guid UserId)
        {
            VWRKSVendors newVWRKSVendors = new VWRKSVendors();

            List<VWRKSPenilaian> hps = (from b in ctx.RKSDetails
                                        join c in ctx.RKSHeaders on b.RKSHeaderId equals c.Id
                                        where c.PengadaanId == PengadaanId
                                        select new VWRKSPenilaian
                                        {
                                            Id = b.Id,
                                            harga = b.hps,
                                            item = b.item,
                                            keteranganItem = b.keterangan,
                                            jumlah = b.jumlah,
                                            satuan = b.satuan,
                                            level = b.level,
                                            grup = b.grup,
                                            judul = b.judul
                                        }).ToList();
            newVWRKSVendors.hps = hps;
            decimal? totalHps = hps.Sum(d => d.harga * d.jumlah);
            newVWRKSVendors.hps.Add(new VWRKSPenilaian { keteranganItem = "Total", harga = totalHps, isTotal = 1 });

            List<VWVendorsHarga> vendors = new List<VWVendorsHarga>();

            //var kandidatTerpilih = (from b in ctx.PelaksanaanPemilihanKandidats
            //                        where b.PengadaanId == PengadaanId
            //                        select b).ToList();
            var kandidatTerpilih = (from b in ctx.HargaKlarifikasiLanLanjutans
                                    where b.RKSDetail.RKSHeader.PengadaanId == PengadaanId
                                    select b.VendorId).Distinct().ToList();
            var lastItem = kandidatTerpilih.Last();
            foreach (var item in kandidatTerpilih)
            {
                //var xKandidatPengadaans = (from b in ctx.PelaksanaanPemilihanKandidats
                //                           where b.PengadaanId == PengadaanId && b.VendorId == item.VendorId
                //                           select b).Distinct().FirstOrDefault();
                var xKandidatPengadaans = (from b in ctx.HargaKlarifikasiLanLanjutans
                                           where b.RKSDetail.RKSHeader.PengadaanId == PengadaanId && b.VendorId == item.Value
                                           select b).Distinct().FirstOrDefault();

                //VWVendorsHarga mVWVendorsHarga = new VWVendorsHarga();
                //mVWVendorsHarga.nama = ctx.Vendors.Find(xKandidatPengadaans.VendorId).Nama + " (Submit Penawaran)";

                //mVWVendorsHarga.items = (from b in ctx.HargaRekanans
                //                         join c in ctx.RKSDetails on b.RKSDetailId equals c.Id
                //                         join d in ctx.RKSHeaders on c.RKSHeaderId equals d.Id
                //                         where d.PengadaanId == PengadaanId && b.VendorId == xKandidatPengadaans.VendorId
                //                         select new item
                //                         {
                //                             Id = c.Id,
                //                             harga = b.harga,
                //                             jumlah = c.jumlah,
                //                             grup = c.grup
                //                         }).ToList();
                //foreach (var xitem in mVWVendorsHarga.items)
                //{
                //    subtotal xsubtotal = new subtotal();
                //    if (mVWVendorsHarga.items.IndexOf(xitem) < mVWVendorsHarga.items.Count() - 1)
                //    {
                //        if (mVWVendorsHarga.items[mVWVendorsHarga.items.IndexOf(xitem) + 1].grup != xitem.grup)
                //        {
                //            xsubtotal.rksGroup = xitem.grup;
                //            xsubtotal.totalGroup = mVWVendorsHarga.items.Where(d => d.grup == xitem.grup).Sum(d => d.harga * d.jumlah);
                //            xitem.subtotal = xsubtotal;
                //        }
                //    }
                //    else
                //    {
                //        xsubtotal.rksGroup = xitem.grup;
                //        xsubtotal.totalGroup = mVWVendorsHarga.items.Where(d => d.grup == xitem.grup).Sum(d => d.harga * d.jumlah);
                //        xitem.subtotal = xsubtotal;
                //    }
                //}
                //if (mVWVendorsHarga.items.Count > 0)
                //{
                //    decimal? total = mVWVendorsHarga.items.Sum(d => d.harga * d.jumlah);
                //    mVWVendorsHarga.items.Add(new item { harga = total, isTotal = 1 });
                //    vendors.Add(mVWVendorsHarga);
                //}

                VWVendorsHarga mVWVendorsHarga = new VWVendorsHarga();
                mVWVendorsHarga.nama = ctx.Vendors.Find(xKandidatPengadaans.VendorId).Nama + " (Klarifikasi Lanjutan)";

                mVWVendorsHarga.items = (from b in ctx.HargaKlarifikasiLanLanjutans
                                         join c in ctx.RKSDetails on b.RKSDetailId equals c.Id
                                         join d in ctx.RKSHeaders on c.RKSHeaderId equals d.Id
                                         where d.PengadaanId == PengadaanId && b.VendorId == xKandidatPengadaans.VendorId
                                         select new item
                                         {
                                             Id = c.Id,
                                             harga = b.harga,
                                             jumlah = c.jumlah,
                                             grup = c.grup
                                         }).ToList();
                foreach (var xitem in mVWVendorsHarga.items)
                {
                    subtotal xsubtotal = new subtotal();
                    if (mVWVendorsHarga.items.IndexOf(xitem) < mVWVendorsHarga.items.Count() - 1)
                    {
                        if (mVWVendorsHarga.items[mVWVendorsHarga.items.IndexOf(xitem) + 1].grup != xitem.grup)
                        {
                            xsubtotal.rksGroup = xitem.grup;
                            xsubtotal.totalGroup = mVWVendorsHarga.items.Where(d => d.grup == xitem.grup).Sum(d => d.harga * d.jumlah);
                            xitem.subtotal = xsubtotal;
                        }
                    }
                    else
                    {
                        xsubtotal.rksGroup = xitem.grup;
                        xsubtotal.totalGroup = mVWVendorsHarga.items.Where(d => d.grup == xitem.grup).Sum(d => d.harga * d.jumlah);
                        xitem.subtotal = xsubtotal;
                    }
                }
                if (mVWVendorsHarga.items.Count > 0)
                {
                    decimal? total = mVWVendorsHarga.items.Sum(d => d.harga * d.jumlah);
                    mVWVendorsHarga.items.Add(new item { harga = total, isTotal = 1 });
                    vendors.Add(mVWVendorsHarga);
                }

            }

            newVWRKSVendors.vendors = vendors;
            return newVWRKSVendors;
        }

        public VWRKSVendors getRKSKlarifikasiPenilaianVendor2(Guid PengadaanId, Guid UserId, int VendorId)
        {
            VWRKSVendors newVWRKSVendors = new VWRKSVendors();

            List<VWRKSPenilaian> hps = (from b in ctx.RKSDetails
                                        join c in ctx.RKSHeaders on b.RKSHeaderId equals c.Id
                                        where c.PengadaanId == PengadaanId
                                        select new VWRKSPenilaian
                                        {
                                            Id = b.Id,
                                            harga = b.hps,
                                            item = b.item,
                                            jumlah = b.jumlah,
                                            satuan = b.satuan,
                                            level = b.level,
                                            grup = b.grup,
                                            judul = b.judul
                                        }).ToList();
            newVWRKSVendors.hps = hps;
            decimal? totalHps = hps.Sum(d => d.harga * d.jumlah);
            newVWRKSVendors.hps.Add(new VWRKSPenilaian { item = "Total", harga = totalHps, isTotal = 1 });

            List<VWVendorsHarga> vendors = new List<VWVendorsHarga>();

            var xKandidatPengadaans = (from b in ctx.KandidatPengadaans
                                       where b.PengadaanId == PengadaanId && b.VendorId == VendorId
                                       select b).Distinct().FirstOrDefault();

            VWVendorsHarga mVWVendorsHarga = new VWVendorsHarga();
            mVWVendorsHarga.nama = ctx.Vendors.Find(xKandidatPengadaans.VendorId).Nama + " (Submit Penawaran)";

            mVWVendorsHarga.items = (from b in ctx.HargaRekanans
                                     join c in ctx.RKSDetails on b.RKSDetailId equals c.Id
                                     join d in ctx.RKSHeaders on c.RKSHeaderId equals d.Id
                                     where d.PengadaanId == PengadaanId && b.VendorId == xKandidatPengadaans.VendorId
                                     select new item
                                     {
                                         Id = c.Id,
                                         harga = b.harga,
                                         jumlah = c.jumlah
                                     }).ToList();
            if (mVWVendorsHarga.items.Count > 0)
            {
                decimal? total = mVWVendorsHarga.items.Sum(d => d.harga * d.jumlah);
                mVWVendorsHarga.items.Add(new item { harga = total });
                vendors.Add(mVWVendorsHarga);
            }

            mVWVendorsHarga = new VWVendorsHarga();
            mVWVendorsHarga.nama = ctx.Vendors.Find(xKandidatPengadaans.VendorId).Nama + " (Klarifikasi Harga)";

            mVWVendorsHarga.items = (from b in ctx.HargaKlarifikasiRekanans
                                     join c in ctx.RKSDetails on b.RKSDetailId equals c.Id
                                     join d in ctx.RKSHeaders on c.RKSHeaderId equals d.Id
                                     where d.PengadaanId == PengadaanId && b.VendorId == xKandidatPengadaans.VendorId
                                     select new item
                                     {
                                         Id = c.Id,
                                         harga = b.harga,
                                         jumlah = c.jumlah
                                     }).ToList();
            if (mVWVendorsHarga.items.Count > 0)
            {
                decimal? total = mVWVendorsHarga.items.Sum(d => d.harga * d.jumlah);
                mVWVendorsHarga.items.Add(new item { harga = total });
                vendors.Add(mVWVendorsHarga);
            }



            newVWRKSVendors.vendors = vendors;
            return newVWRKSVendors;
        }

        public List<VWRekananPenilaian> getPemenangPengadaan(Guid PengadaanId, Guid UserId)
        {
            var xKandidatPengadaans = (from b in ctx.PemenangPengadaans
                                       join c in ctx.Vendors on b.VendorId equals c.Id
                                       where b.PengadaanId == PengadaanId
                                       select new VWRekananPenilaian
                                       {
                                           Id = b.Id,
                                           NamaVendor = c.Nama,
                                           VendorId = b.VendorId,
                                           Email = c.Email,
                                           Alamat = c.Alamat,
                                           total = (from bb in ctx.HargaKlarifikasiRekanans
                                                    join cc in ctx.RKSDetails on bb.RKSDetailId equals cc.Id
                                                    join dd in ctx.RKSHeaders on cc.RKSHeaderId equals dd.Id
                                                    where dd.PengadaanId == PengadaanId && bb.VendorId == b.VendorId
                                                    select new item
                                                    {
                                                        harga = bb.harga,
                                                        jumlah = cc.jumlah
                                                    }).Sum(xx => xx.harga * xx.jumlah),
                                           terpilih = (from bb in ctx.PemenangPengadaans
                                                       where bb.PengadaanId == PengadaanId &&
                                                       bb.VendorId == b.VendorId
                                                       select bb).FirstOrDefault() == null ? 0 : 1,
                                           NilaiKriteria = (from bb in ctx.PembobotanPengadaans
                                                            join cc in ctx.PembobotanPengadaanVendors on bb.KreteriaPembobotanId equals cc.KreteriaPembobotanId
                                                            where cc.PengadaanId == PengadaanId && cc.VendorId == b.VendorId && bb.PengadaanId == PengadaanId
                                                            select new
                                                            {
                                                                Bobot = bb.Bobot == null ? 0 : bb.Bobot.Value,
                                                                Nilai = cc.Nilai == null ? 0 : cc.Nilai.Value
                                                            }).Sum(dd => (dd.Nilai * dd.Bobot) / 100),
                                           NoSPK = (from bb in ctx.BeritaAcaras
                                                    where bb.PengadaanId == PengadaanId &&
                                                       bb.VendorId == b.VendorId && bb.Tipe == TipeBerkas.SuratPerintahKerja
                                                    select bb).FirstOrDefault() == null ? "-" : (from bb in ctx.BeritaAcaras
                                                                                                 where bb.PengadaanId == PengadaanId &&
                                                                                                    bb.VendorId == b.VendorId && bb.Tipe == TipeBerkas.SuratPerintahKerja
                                                                                                 select bb).FirstOrDefault().NoBeritaAcara,
                                           TotalPenilaian = (ctx.TenderScoringHeaders.Where(x => x.VendorId == b.VendorId && x.PengadaanId == PengadaanId).Select(x => x.Average).FirstOrDefault())
                                       }).ToList();
            var cekKlarifikasiLanjutx = cekKlarifikasiLanjut(PengadaanId);
            foreach (var item in xKandidatPengadaans)
            {
                if (!cekKlarifikasiLanjutx)
                {
                    item.total = (from bb in ctx.HargaKlarifikasiRekanans
                                  join cc in ctx.RKSDetails on bb.RKSDetailId equals cc.Id
                                  join dd in ctx.RKSHeaders on cc.RKSHeaderId equals dd.Id
                                  where dd.PengadaanId == PengadaanId && bb.VendorId == item.VendorId
                                  select new item
                                  {
                                      harga = bb.harga,
                                      jumlah = cc.jumlah
                                  }).Sum(xx => xx.harga == null ? 0 : xx.harga * xx.jumlah);
                }
                else
                {
                    item.total = (from bb in ctx.HargaKlarifikasiLanLanjutans
                                  join cc in ctx.RKSDetails on bb.RKSDetailId equals cc.Id
                                  join dd in ctx.RKSHeaders on cc.RKSHeaderId equals dd.Id
                                  where dd.PengadaanId == PengadaanId && bb.VendorId == item.VendorId
                                  select new item
                                  {
                                      harga = bb.harga,
                                      jumlah = cc.jumlah
                                  }).Sum(xx => xx.harga == null ? 0 : xx.harga * xx.jumlah);
                }
            }
            return xKandidatPengadaans;
        }

        public List<VWRekananPenilaian> getKandidatPengadaan(Guid PengadaanId, Guid UserId)
        {
            var xKandidatPengadaans = (from b in ctx.KandidatPengadaans
                                       join c in ctx.Vendors on b.VendorId equals c.Id
                                       where b.PengadaanId == PengadaanId
                                       select new VWRekananPenilaian
                                       {
                                           NamaVendor = c.Nama,
                                           VendorId = b.VendorId,
                                           Email = c.Email,
                                           Alamat = c.Alamat,
                                           total = (from bb in ctx.HargaKlarifikasiRekanans
                                                    join cc in ctx.RKSDetails on bb.RKSDetailId equals cc.Id
                                                    join dd in ctx.RKSHeaders on cc.RKSHeaderId equals dd.Id
                                                    where dd.PengadaanId == PengadaanId && bb.VendorId == b.VendorId
                                                    select new item
                                                    {
                                                        harga = bb.harga,
                                                        jumlah = cc.jumlah
                                                    }).Sum(xx => xx.harga * xx.jumlah),
                                           terpilih = (from bb in ctx.PemenangPengadaans
                                                       where bb.PengadaanId == PengadaanId &&
                                                       bb.VendorId == b.VendorId
                                                       select bb).FirstOrDefault() == null ? 0 : 1,
                                           NilaiKriteria = (from bb in ctx.PembobotanPengadaans
                                                            join cc in ctx.PembobotanPengadaanVendors on bb.KreteriaPembobotanId equals cc.KreteriaPembobotanId
                                                            where cc.PengadaanId == PengadaanId && cc.VendorId == b.VendorId && bb.PengadaanId == PengadaanId
                                                            select new
                                                            {
                                                                Bobot = bb.Bobot == null ? 0 : bb.Bobot.Value,
                                                                Nilai = cc.Nilai == null ? 0 : cc.Nilai.Value
                                                            }).Sum(dd => (dd.Nilai * dd.Bobot) / 100)
                                       }).ToList();
            return xKandidatPengadaans;
        }

        public List<KandidatPengadaan> getKandidatTidakHadir(Guid PengadaanId, Guid UserId)
        {
            try
            {
                var hadir = ctx.KehadiranKandidatAanwijzings.Where(d => d.PengadaanId == PengadaanId).Select(d => d.VendorId).ToList();
                var kandidat = ctx.KandidatPengadaans.Where(d => d.PengadaanId == PengadaanId && !hadir.Contains(d.VendorId)).ToList();

                return kandidat;
            }
            catch { return new List<KandidatPengadaan>(); }
        }

        public List<KandidatPengadaan> getKandidatHadir(Guid PengadaanId, Guid UserId)
        {
            try
            {
                var hadir = ctx.KehadiranKandidatAanwijzings.Where(d => d.PengadaanId == PengadaanId).Select(d => d.VendorId).ToList();
                var kandidat = ctx.KandidatPengadaans.Where(d => d.PengadaanId == PengadaanId && hadir.Contains(d.VendorId)).ToList();

                return kandidat;
            }
            catch { return new List<KandidatPengadaan>(); }
        }

        public List<KandidatPengadaan> getKandidatKirim(Guid PengadaanId, Guid UserId)
        {
            try
            {
                var kirim = ctx.HargaRekanans.Where(d => d.RKSDetail.RKSHeader.PengadaanId == PengadaanId).Select(d => d.VendorId).Distinct().ToList();
                var kandidat = ctx.KandidatPengadaans.Where(d => d.PengadaanId == PengadaanId && kirim.Contains(d.VendorId)).ToList();

                return kandidat;
            }
            catch { return new List<KandidatPengadaan>(); }
        }

        public List<KandidatPengadaan> getKandidatTidakKirim(Guid PengadaanId, Guid UserId)
        {
            try
            {
                var kirim = ctx.HargaRekanans.Where(d => d.RKSDetail.RKSHeader.PengadaanId == PengadaanId).Select(d => d.VendorId).Distinct().ToList();
                var kandidat = ctx.KandidatPengadaans.Where(d => d.PengadaanId == PengadaanId && !kirim.Contains(d.VendorId)).ToList();

                return kandidat;
            }
            catch { return new List<KandidatPengadaan>(); }
        }


        //public List<KandidatPengadaan> getKandidatTidakMengirimPenawaran(Guid PengadaanId, Guid UserId)
        //{
        //    try
        //    {
        //        var kirim = ctx.PelaksanaanSubmitPenawarans.Where(d => d.PengadaanId == PengadaanId).Select(d => d.VendorId).ToList();
        //        var kandidat = ctx.KandidatPengadaans.Where(d => d.PengadaanId == PengadaanId && !kirim.Contains(d.VendorId)).ToList();

        //        return kandidat;
        //    }
        //    catch { return new List<KandidatPengadaan>(); }
        //}

        public List<VWRekananPenilaian> getKandidatPengadaan2(Guid PengadaanId, Guid UserId)
        {
            var xKandidatPengadaans = (from b in ctx.PemenangPengadaans
                                       join c in ctx.Vendors on b.VendorId equals c.Id
                                       where b.PengadaanId == PengadaanId
                                       select new VWRekananPenilaian
                                       {
                                           NamaVendor = c.Nama,
                                           VendorId = b.VendorId,
                                           Email = c.Email,
                                           //total = (from bb in ctx.HargaKlarifikasiRekanans
                                           //         join cc in ctx.RKSDetails on bb.RKSDetailId equals cc.Id
                                           //         join dd in ctx.RKSHeaders on cc.RKSHeaderId equals dd.Id
                                           //         where dd.PengadaanId == PengadaanId && bb.VendorId == b.VendorId
                                           //         select new item
                                           //         {
                                           //             harga = bb.harga,
                                           //             jumlah = cc.jumlah
                                           //         }).Sum(xx => xx.harga * xx.jumlah),
                                           terpilih = (from bb in ctx.PemenangPengadaans
                                                       where bb.PengadaanId == PengadaanId &&
                                                       bb.VendorId == b.VendorId
                                                       select bb).FirstOrDefault() == null ? 0 : 1,
                                           NilaiKriteria = (from bb in ctx.PembobotanPengadaans
                                                            join cc in ctx.PembobotanPengadaanVendors on bb.KreteriaPembobotanId equals cc.KreteriaPembobotanId
                                                            where cc.PengadaanId == PengadaanId && cc.VendorId == b.VendorId && bb.PengadaanId == PengadaanId
                                                            select new
                                                            {
                                                                Bobot = bb.Bobot == null ? 0 : bb.Bobot.Value,
                                                                Nilai = cc.Nilai == null ? 0 : cc.Nilai.Value
                                                            }).Sum(dd => (dd.Nilai * dd.Bobot) / 100)
                                       }).ToList();
            var cekKlarifikasiLanjutx = cekKlarifikasiLanjut(PengadaanId);
            foreach (var item in xKandidatPengadaans)
            {
                if (!cekKlarifikasiLanjutx)
                {
                    item.total = (from bb in ctx.HargaKlarifikasiRekanans
                                  join cc in ctx.RKSDetails on bb.RKSDetailId equals cc.Id
                                  join dd in ctx.RKSHeaders on cc.RKSHeaderId equals dd.Id
                                  where dd.PengadaanId == PengadaanId && bb.VendorId == item.VendorId
                                  select new item
                                  {
                                      harga = bb.harga,
                                      jumlah = cc.jumlah
                                  }).Sum(xx => xx.harga == null ? 0 : xx.harga * xx.jumlah);
                }
                else
                {
                    item.total = (from bb in ctx.HargaKlarifikasiLanLanjutans
                                  join cc in ctx.RKSDetails on bb.RKSDetailId equals cc.Id
                                  join dd in ctx.RKSHeaders on cc.RKSHeaderId equals dd.Id
                                  where dd.PengadaanId == PengadaanId && bb.VendorId == item.VendorId
                                  select new item
                                  {
                                      harga = bb.harga,
                                      jumlah = cc.jumlah
                                  }).Sum(xx => xx.harga == null ? 0 : xx.harga * xx.jumlah);
                }
            }
            return xKandidatPengadaans;
        }

        public List<VWRKSDetail> getRKSDetails(Guid PengadaanId, Guid UserId)
        {
            List<VWRKSDetail> hps = (from b in ctx.RKSDetails
                                     join c in ctx.RKSHeaders on b.RKSHeaderId equals c.Id
                                     where c.PengadaanId == PengadaanId
                                     select new VWRKSDetail
                                     {
                                         Id = b.Id,
                                         hps = b.hps,
                                         item = b.item,
                                         judul = b.judul,
                                         level = b.level,
                                         grup = b.grup,
                                         jumlah = b.jumlah,
                                         satuan = b.satuan,
                                         total = b.jumlah == null ? 0 : b.jumlah * b.hps,
                                         keterangan = b.keterangan
                                     }).OrderBy(d => d.grup).ThenBy(d => d.level).ToList();
            //hps.Add(new VWRKSDetail { item = "Total", total = hps.Sum(d => d.total) });
            return hps;
        }

        public List<ViewBenefitRate> getRKSAsuransiDetails(Guid DocumentIdBaru, Guid UserId)
        {
            List<ViewBenefitRate> hps = (from a in ctx.InsuranceTarifs
                                         join b in ctx.InsuranceTarifBenefits on a.DocumentId equals b.DocumentId
                                         join c in ctx.BenefitRates on b.BenefitRateId.Id equals c.Id
                                         join d in ctx.ReferenceDatas on c.BenefitCode equals d.Code
                                         join e in ctx.ReferenceDatas on c.BenefitCoverage equals e.Code
                                         join f in ctx.ReferenceDatas on c.RegionCode equals f.Code
                                         where b.DocumentId == DocumentIdBaru
                                         select new ViewBenefitRate
                                         {
                                             Id = c.Id,
                                             BenefitCode = d.LocalizedName,
                                             BenefitCoverage = e.LocalizedName,
                                             RegionCode = f.LocalizedName,
                                             Rate = c.Rate,
                                             RateLowerLimit = c.RateLowerLimit,
                                             RateUpperLimit = c.RateUpperLimit,
                                             IsRange = c.IsRange == null ? c.IsRange : c.IsRange,
                                             IsOpen = c.IsOpen == null ? c.IsOpen : c.IsOpen
                                         }).ToList();
            //hps.Add(new VWRKSDetail { item = "Total", total = hps.Sum(d => d.total) });
            return hps;
        }

        public List<VWPembobotanPengadaan> getKriteriaPembobotan(Guid PengadaanId)
        {
            var oKriteriaPembobotan = (from b in ctx.KreteriaPembobotans
                                       select new VWPembobotanPengadaan
                                       {
                                           Id = b.Id,
                                           NamaKreteria = b.NamaKreteria,
                                           Bobot = ctx.PembobotanPengadaans.Where(v => v.KreteriaPembobotanId == b.Id
                                            && v.PengadaanId == PengadaanId).FirstOrDefault() == null ? b.Bobot :
                                            ctx.PembobotanPengadaans.Where(v => v.KreteriaPembobotanId == b.Id
                                            && v.PengadaanId == PengadaanId).FirstOrDefault().Bobot
                                       }).OrderBy(d => d.NamaKreteria).ToList();

            return oKriteriaPembobotan;
        }

        public int addPembobtanPengadaan(PembobotanPengadaan dataPembobotanPengadaan, Guid UserId)
        {
            var oPembobotanPengadaan = ctx.PembobotanPengadaans.Where(
                             d => d.PengadaanId == dataPembobotanPengadaan.PengadaanId && d.KreteriaPembobotanId == dataPembobotanPengadaan.KreteriaPembobotanId).FirstOrDefault();

            int totalPembobotan = ctx.PembobotanPengadaans.Where(d => d.PengadaanId == dataPembobotanPengadaan.PengadaanId).Sum(d => d.Bobot) == null ? 0 : ctx.PembobotanPengadaans.Where(d => d.PengadaanId == dataPembobotanPengadaan.PengadaanId).Sum(d => d.Bobot).Value;


            try
            {
                if (oPembobotanPengadaan != null)
                {
                    totalPembobotan = oPembobotanPengadaan.Bobot == null ? 0 : totalPembobotan - oPembobotanPengadaan.Bobot.Value;
                    totalPembobotan = dataPembobotanPengadaan.Bobot == null ? 0 : totalPembobotan + dataPembobotanPengadaan.Bobot.Value;
                    if (totalPembobotan > 100) return 0;
                    oPembobotanPengadaan.Bobot = dataPembobotanPengadaan.Bobot;
                    //oPembobotanPengadaan.Nilai = dataPembobotanPengadaan.Nilai;
                    ctx.SaveChanges();
                    return 1;
                }
                else
                {
                    totalPembobotan = dataPembobotanPengadaan.Bobot == null ? 0 : totalPembobotan + dataPembobotanPengadaan.Bobot.Value;
                    if (totalPembobotan > 100) return 0;
                    ctx.PembobotanPengadaans.Add(dataPembobotanPengadaan);
                    ctx.SaveChanges();
                    return 1;
                }
            }
            catch
            {
                return 0;
            }
        }

        public int addLstPembobtanPengadaan(List<PembobotanPengadaan> dataLstPembobotanPengadaan, Guid UserId)
        {
            foreach (var item in dataLstPembobotanPengadaan)
            {
                var oPembobotanPengadaan = ctx.PembobotanPengadaans.Where(
                                 d => d.PengadaanId == item.PengadaanId && d.KreteriaPembobotanId == item.KreteriaPembobotanId).FirstOrDefault();

                int totalPembobotan = dataLstPembobotanPengadaan.Sum(d => d.Bobot) == null ? 0 : dataLstPembobotanPengadaan.Sum(d => d.Bobot).Value;

                //ctx.PembobotanPengadaans.Where(d => d.PengadaanId == item.PengadaanId).Sum(d => d.Bobot) == null ? 0 : ctx.PembobotanPengadaans.Where(d => d.PengadaanId == item.PengadaanId).Sum(d => d.Bobot).Value;
                try
                {
                    if (oPembobotanPengadaan != null)
                    {
                        // totalPembobotan = oPembobotanPengadaan.Bobot == null ? 0 : totalPembobotan - oPembobotanPengadaan.Bobot.Value;
                        //totalPembobotan = item.Bobot == null ? 0 : totalPembobotan + item.Bobot.Value;
                        if (totalPembobotan > 100) break;
                        oPembobotanPengadaan.Bobot = item.Bobot;
                        //oPembobotanPengadaan.Nilai = dataPembobotanPengadaan.Nilai;
                        ctx.SaveChanges();
                    }
                    else
                    {
                        //totalPembobotan = item.Bobot == null ? 0 : totalPembobotan + item.Bobot.Value;
                        if (totalPembobotan > 100) break;
                        ctx.PembobotanPengadaans.Add(item);
                        ctx.SaveChanges();
                    }
                }
                catch
                {
                    return 0;
                }
            }
            return 1;
        }

        public int addLstPenilaianKriteriaVendor(List<PembobotanPengadaanVendor> dataLstPenilaianKriteriaVendor, Guid UserId)
        {
            foreach (var item in dataLstPenilaianKriteriaVendor)
            {
                var oPembobotanPengadaanVendor = ctx.PembobotanPengadaanVendors.Where(
                                 d => d.PengadaanId == item.PengadaanId
                                     && d.KreteriaPembobotanId == item.KreteriaPembobotanId
                                    && d.VendorId == item.VendorId).FirstOrDefault();
                try
                {
                    if (oPembobotanPengadaanVendor != null)
                    {
                        oPembobotanPengadaanVendor.Nilai = item.Nilai;
                        ctx.SaveChanges();
                    }
                    else
                    {
                        ctx.PembobotanPengadaanVendors.Add(item);
                        ctx.SaveChanges();
                    }
                }
                catch
                {
                    return 0;
                }
            }
            return 1;
        }

        public List<PembobotanPengadaan> getPembobtanPengadaan(Guid PengadaanId, Guid UserId)
        {
            return ctx.PembobotanPengadaans.Where(d => d.PengadaanId == PengadaanId).ToList();
        }

        public List<VWPembobotanPengadaanVendor> getPembobtanPengadaanVendor(Guid PengadaanId, int VendorId, Guid UserId)
        {
            var oPembobotanVendor = (from b in ctx.KreteriaPembobotans
                                     join c in ctx.PembobotanPengadaans on b.Id equals c.KreteriaPembobotanId into ps
                                     from c in ps.DefaultIfEmpty()
                                         //where c.PengadaanId == PengadaanId
                                     select new VWPembobotanPengadaanVendor
                                     {
                                         Id = c.KreteriaPembobotanId,
                                         NamaKreteria = b.NamaKreteria,
                                         Bobot = ctx.PembobotanPengadaans.Where(v => v.KreteriaPembobotanId == b.Id
                                                  && v.PengadaanId == PengadaanId).FirstOrDefault() == null ? b.Bobot :
                                                  ctx.PembobotanPengadaans.Where(v => v.KreteriaPembobotanId == b.Id
                                                  && v.PengadaanId == PengadaanId).FirstOrDefault().Bobot,
                                         Nilai = ctx.PembobotanPengadaanVendors.Where(
                                                 d => d.KreteriaPembobotanId == c.KreteriaPembobotanId && d.PengadaanId == PengadaanId
                                              && d.VendorId == VendorId).FirstOrDefault() == null ? 0 :
                                              ctx.PembobotanPengadaanVendors.Where(
                                                d => d.KreteriaPembobotanId == c.KreteriaPembobotanId && d.PengadaanId == PengadaanId
                                              && d.VendorId == VendorId).FirstOrDefault().Nilai,
                                         VendorId = VendorId
                                     }).OrderBy(d => d.NamaKreteria).Distinct().ToList();
            return oPembobotanVendor;
        }

        public int deleteKualifikasiKandidat(Guid Id, Guid UserId)
        {
            KualifikasiKandidat mKualifikasiKandidat = ctx.KualifikasiKandidats.Find(Id);
            if (ctx.Pengadaans.Find(mKualifikasiKandidat.PengadaanId) == null) return 0;
            ctx.KualifikasiKandidats.Remove(mKualifikasiKandidat);
            ctx.SaveChanges();
            return 1;
        }

        public PelaksanaanPemilihanKandidat addKandidatPilihan(PelaksanaanPemilihanKandidat oPelaksanaanPemilihanKandidat, Guid UserId)
        {
            Pengadaan Mpengadaaan = ctx.Pengadaans.Find(oPelaksanaanPemilihanKandidat.PengadaanId);
            PersonilPengadaan picPersonil = ctx.PersonilPengadaans.Where(d => d.PersonilId == UserId && d.tipe == "pic" && d.PengadaanId == oPelaksanaanPemilihanKandidat.PengadaanId).FirstOrDefault();
            if (picPersonil == null)
            {
                var iscreated = ctx.PersonilPengadaans.Where(d => d.Pengadaan.CreatedBy == UserId && d.PengadaanId == oPelaksanaanPemilihanKandidat.PengadaanId).FirstOrDefault();
                if (iscreated == null) return new PelaksanaanPemilihanKandidat();
            }
            PelaksanaanPemilihanKandidat oldoPelaksanaanPemilihanKandidat =
                    ctx.PelaksanaanPemilihanKandidats.Where(d => d.PengadaanId == oPelaksanaanPemilihanKandidat.PengadaanId
                                    && d.VendorId == oPelaksanaanPemilihanKandidat.VendorId).FirstOrDefault();
            if (oldoPelaksanaanPemilihanKandidat == null)
            {
                oPelaksanaanPemilihanKandidat.CreatedBy = UserId;
                oPelaksanaanPemilihanKandidat.CreatedDate = DateTime.Now;
                ctx.PelaksanaanPemilihanKandidats.Add(oPelaksanaanPemilihanKandidat);
                //addPemenangPengadaan(new PemenangPengadaan() { PengadaanId = oPelaksanaanPemilihanKandidat.PengadaanId, VendorId = oPelaksanaanPemilihanKandidat.VendorId }, UserId);
            }
            else
            {
                ctx.PelaksanaanPemilihanKandidats.Remove(oldoPelaksanaanPemilihanKandidat);
                // DeletePemenang(new PemenangPengadaan() { PengadaanId = oPelaksanaanPemilihanKandidat.PengadaanId, VendorId = oPelaksanaanPemilihanKandidat.VendorId }, UserId);
            }
            ctx.SaveChanges();

            return oPelaksanaanPemilihanKandidat;
        }

        public string statusVendor(Guid PengadaanId, Guid UserId)
        {
            var VendorId = ctx.Vendors.Where(xx => xx.Owner == UserId).FirstOrDefault().Id;
            Pengadaan oPengadaan = ctx.Pengadaans.Find(PengadaanId);
            if (oPengadaan.Status == EStatusPengadaan.AANWIJZING)
            {
                return "Dalam Proses Aanwijzing";
            }
            else if (oPengadaan.Status == EStatusPengadaan.SUBMITPENAWARAN)
            {
                return "Dalam Proses Submit Penawaran";
            }
            else if (oPengadaan.Status == EStatusPengadaan.BUKAAMPLOP)
            {
                return "Dalam Proses Buka Amplop";
            }
            else if (oPengadaan.Status == EStatusPengadaan.PENILAIAN)
            {
                var oPelaksanaanPemilihanKandidat = ctx.PelaksanaanPemilihanKandidats.Where(d => d.PengadaanId == PengadaanId &&
                                d.VendorId == VendorId).FirstOrDefault();
                if (oPelaksanaanPemilihanKandidat == null)
                    return "Pengajuan Anda DiTolak";
                return "Dalam Proses Penilaian";

            }
            else if (oPengadaan.Status == EStatusPengadaan.KLARIFIKASI)
            {
                return "Dalam Proses Klarifikasi";
            }
            else if (oPengadaan.Status == EStatusPengadaan.KLARIFIKASILANJUTAN)
            {
                return "Dalam Proses Klarifikasi Lanjut";
            }
            else if (oPengadaan.Status == EStatusPengadaan.PEMENANG)
            {
                var oPelaksanaanPemenang = ctx.PemenangPengadaans.Where(d => d.PengadaanId == PengadaanId &&
                               d.VendorId == VendorId).FirstOrDefault();

                if (oPelaksanaanPemenang == null)
                    return "Pengajuan Anda DiTolak";
                var beritaAcara = ctx.BeritaAcaras.Where(d => d.PengadaanId == PengadaanId && d.Tipe == TipeBerkas.BeritaAcaraPenentuanPemenang).FirstOrDefault();

                if (beritaAcara == null)
                    return "Dalam Proses Penentuan Pemenang";
                else
                    return "Anda Sebagai Pemenang Pengadaan Ini";
            }
            else
            {
                return "Dalam Proses";
            }
        }

        public int deleteKandidatPilihan(PelaksanaanPemilihanKandidat oPelaksanaanPemilihanKandidat, Guid UserId)
        {
            Pengadaan Mpengadaaan = ctx.Pengadaans.Find(oPelaksanaanPemilihanKandidat.PengadaanId);
            PersonilPengadaan picPersonil = ctx.PersonilPengadaans.Where(d => d.PersonilId == UserId && d.tipe == "pic" && d.PengadaanId == oPelaksanaanPemilihanKandidat.PengadaanId).FirstOrDefault();
            if (picPersonil == null) return 0;
            var oldoPelaksanaanPemilihanKandidat =
                    ctx.PelaksanaanPemilihanKandidats.Where(d => d.PengadaanId == oPelaksanaanPemilihanKandidat.PengadaanId
                                    && d.VendorId == oPelaksanaanPemilihanKandidat.VendorId);
            ctx.PelaksanaanPemilihanKandidats.RemoveRange(oldoPelaksanaanPemilihanKandidat);
            ctx.SaveChanges();
            return 1;
        }

        public int nextToState(Guid PengadaanId, Guid UserId, EStatusPengadaan state)
        {
            Pengadaan Mpengadaaan = ctx.Pengadaans.Find(PengadaanId);
            PersonilPengadaan picPersonil = ctx.PersonilPengadaans.Where(d => d.PersonilId == UserId && d.tipe == "pic" && d.PengadaanId == PengadaanId).FirstOrDefault();
            if (picPersonil == null) return 0;
            Mpengadaaan.Status = state;
            ctx.SaveChanges();
            return 1;
        }


        public int isNotaUploaded(Guid PengadaanId, Guid UserId)
        {
            var beritaAcaraPe = ctx.DokumenPengadaans.Where(d => d.PengadaanId == PengadaanId && d.Tipe == TipeBerkas.BeritaAcaraPenentuanPemenang).FirstOrDefault();

            if (beritaAcaraPe == null) return 0;
            else return 1;
        }
        public int isSpkUploaded(Guid PengadaanId, Guid UserId)
        {
            var beritaAcaraPe = ctx.DokumenPengadaans.Where(d => d.PengadaanId == PengadaanId && d.Tipe == TipeBerkas.SuratPerintahKerja).FirstOrDefault();

            if (beritaAcaraPe == null) return 0;
            else return 1;
        }


        public string GenerateNoPengadaan(Guid UserId)
        {
            var oNoDokumen = ctx.NoDokumenGenerators.Where(d => d.tipe == TipeNoDokumen.PENGADAAN).OrderByDescending(d => d.Id).FirstOrDefault();
            var KodePengadaan = System.Configuration.ConfigurationManager.AppSettings["KODE_PENGADAAN"].ToString();
            if (oNoDokumen == null)
            {
                string newNODok = "1" + KodePengadaan + Common.ConvertBulanRomawi(DateTime.Now.Month) + "/" + DateTime.Now.Year;
                NoDokumenGenerator newoNoDokumen = new NoDokumenGenerator();
                newoNoDokumen.CreateOn = DateTime.Now;
                newoNoDokumen.CreateBy = UserId;
                newoNoDokumen.No = newNODok;
                newoNoDokumen.tipe = TipeNoDokumen.PENGADAAN;
                ctx.NoDokumenGenerators.Add(newoNoDokumen);
                ctx.SaveChanges();
                return newoNoDokumen.No;
            }
            else
            {
                var arrNo = oNoDokumen.No.Split('/');
                int NextNo = Convert.ToInt32(arrNo[0]);
                NextNo = NextNo + 1;
                int oldYear = Convert.ToInt32(arrNo[4]);
                string newNoDokmen = "";
                if (oldYear == DateTime.Now.Year)
                    newNoDokmen = NextNo.ToString() + KodePengadaan + Common.ConvertBulanRomawi(DateTime.Now.Month) + "/" + +DateTime.Now.Year;
                else newNoDokmen = "1" + KodePengadaan + Common.ConvertBulanRomawi(DateTime.Now.Month) + "/" + DateTime.Now.Year;

                NoDokumenGenerator newoNoDokumen = new NoDokumenGenerator();
                newoNoDokumen.CreateOn = DateTime.Now;
                newoNoDokumen.CreateBy = UserId;
                newoNoDokumen.No = newNoDokmen;
                newoNoDokumen.tipe = TipeNoDokumen.PENGADAAN;
                ctx.NoDokumenGenerators.Add(newoNoDokumen);
                ctx.SaveChanges();
                return newoNoDokumen.No;
            }
        }

        public string GenerateBeritaAcara(Guid UserId)
        {
            var oNoDokumen = ctx.NoDokumenGenerators.Where(d => d.tipe == TipeNoDokumen.BERITAACARA).OrderByDescending(d => d.Id).FirstOrDefault();
            var KodeBeritaAcara = System.Configuration.ConfigurationManager.AppSettings["KODE_BERITAACARA"].ToString();
            if (oNoDokumen == null)
            {
                string newNODok = "1" + KodeBeritaAcara + Common.ConvertBulanRomawi(DateTime.Now.Month) + "/" + DateTime.Now.Year;
                NoDokumenGenerator newoNoDokumen = new NoDokumenGenerator();
                newoNoDokumen.CreateOn = DateTime.Now;
                newoNoDokumen.CreateBy = UserId;
                newoNoDokumen.No = newNODok;
                newoNoDokumen.tipe = TipeNoDokumen.BERITAACARA;
                ctx.NoDokumenGenerators.Add(newoNoDokumen);
                ctx.SaveChanges();
                return newoNoDokumen.No;
            }
            else
            {
                var arrNo = oNoDokumen.No.Split('/');
                int NextNo = Convert.ToInt32(arrNo[0]);
                NextNo = NextNo + 1;
                int oldYear = Convert.ToInt32(arrNo[3]);
                string newNoDokmen = "";
                if (oldYear == DateTime.Now.Year)
                    newNoDokmen = NextNo.ToString() + KodeBeritaAcara + Common.ConvertBulanRomawi(DateTime.Now.Month) + "/" + +DateTime.Now.Year;
                else newNoDokmen = "1" + KodeBeritaAcara + Common.ConvertBulanRomawi(DateTime.Now.Month) + "/" + DateTime.Now.Year;

                NoDokumenGenerator newoNoDokumen = new NoDokumenGenerator();
                newoNoDokumen.CreateOn = DateTime.Now;
                newoNoDokumen.CreateBy = UserId;
                newoNoDokumen.No = newNoDokmen;
                newoNoDokumen.tipe = TipeNoDokumen.BERITAACARA;
                ctx.NoDokumenGenerators.Add(newoNoDokumen);
                ctx.SaveChanges();
                return newoNoDokumen.No;
            }
        }

        public string GenerateBeritaAcaraNota(Guid UserId)
        {
            var oNoDokumen = ctx.NoDokumenGenerators.Where(d => d.tipe == TipeNoDokumen.NOTA).OrderByDescending(d => d.Id).FirstOrDefault();
            var KodeNota = System.Configuration.ConfigurationManager.AppSettings["KODE_NOTA"].ToString();
            if (oNoDokumen == null)
            {
                string newNODok = "1" + KodeNota + Common.ConvertBulanRomawi(DateTime.Now.Month) + "/" + DateTime.Now.Year;
                NoDokumenGenerator newoNoDokumen = new NoDokumenGenerator();
                newoNoDokumen.CreateOn = DateTime.Now;
                newoNoDokumen.CreateBy = UserId;
                newoNoDokumen.No = newNODok;
                newoNoDokumen.tipe = TipeNoDokumen.NOTA;
                ctx.NoDokumenGenerators.Add(newoNoDokumen);
                ctx.SaveChanges();
                return newoNoDokumen.No;
            }
            else
            {
                var arrNo = oNoDokumen.No.Split('/');
                int NextNo = Convert.ToInt32(arrNo[0]);
                NextNo = NextNo + 1;
                int oldYear = Convert.ToInt32(arrNo[4]);
                string newNoDokmen = "";
                if (oldYear == DateTime.Now.Year)
                    newNoDokmen = NextNo.ToString() + KodeNota + Common.ConvertBulanRomawi(DateTime.Now.Month) + "/" + +DateTime.Now.Year;
                else newNoDokmen = "1" + KodeNota + Common.ConvertBulanRomawi(DateTime.Now.Month) + "/" + DateTime.Now.Year;

                NoDokumenGenerator newoNoDokumen = new NoDokumenGenerator();
                newoNoDokumen.CreateOn = DateTime.Now;
                newoNoDokumen.CreateBy = UserId;
                newoNoDokumen.No = newNoDokmen;
                newoNoDokumen.tipe = TipeNoDokumen.NOTA;
                ctx.NoDokumenGenerators.Add(newoNoDokumen);
                ctx.SaveChanges();
                return newoNoDokumen.No;
            }
        }

        public string GenerateBeritaAcaraSPK(Guid UserId)
        {
            var oNoDokumen = ctx.NoDokumenGenerators.Where(d => d.tipe == TipeNoDokumen.SPK).OrderByDescending(d => d.Id).FirstOrDefault();
            var KODESPK = System.Configuration.ConfigurationManager.AppSettings["KODE_SPK"].ToString();
            if (oNoDokumen == null)
            {
                string newNODok = "1" + KODESPK + Common.ConvertBulanRomawi(DateTime.Now.Month) + "/" + DateTime.Now.Year;
                NoDokumenGenerator newoNoDokumen = new NoDokumenGenerator();
                newoNoDokumen.CreateOn = DateTime.Now;
                newoNoDokumen.CreateBy = UserId;
                newoNoDokumen.No = newNODok;
                newoNoDokumen.tipe = TipeNoDokumen.SPK;
                ctx.NoDokumenGenerators.Add(newoNoDokumen);
                ctx.SaveChanges();
                return newoNoDokumen.No;
            }
            else
            {
                var arrNo = oNoDokumen.No.Split('/');
                int NextNo = Convert.ToInt32(arrNo[0]);
                NextNo = NextNo + 1;
                int oldYear = Convert.ToInt32(arrNo[4]);
                string newNoDokmen = "";
                if (oldYear == DateTime.Now.Year)
                    newNoDokmen = NextNo.ToString() + KODESPK + Common.ConvertBulanRomawi(DateTime.Now.Month) + "/" + +DateTime.Now.Year;
                else newNoDokmen = "1" + KODESPK + Common.ConvertBulanRomawi(DateTime.Now.Month) + "/" + DateTime.Now.Year;

                NoDokumenGenerator newoNoDokumen = new NoDokumenGenerator();
                newoNoDokumen.CreateOn = DateTime.Now;
                newoNoDokumen.CreateBy = UserId;
                newoNoDokumen.No = newNoDokmen;
                newoNoDokumen.tipe = TipeNoDokumen.SPK;
                ctx.NoDokumenGenerators.Add(newoNoDokumen);
                ctx.SaveChanges();
                return newoNoDokumen.No;
            }
        }
        public string GenerateNoDOKUMEN(Guid UserId, string KODE, TipeNoDokumen tipe)
        {
            var oNoDokumen = ctx.NoDokumenGenerators.Where(d => d.tipe == tipe).OrderByDescending(d => d.Id).FirstOrDefault();


            if (oNoDokumen == null)
            {
                string newNODok = "1" + KODE + Common.ConvertBulanRomawi(DateTime.Now.Month) + "/" + DateTime.Now.Year;
                NoDokumenGenerator newoNoDokumen = new NoDokumenGenerator();
                newoNoDokumen.CreateOn = DateTime.Now;
                newoNoDokumen.CreateBy = UserId;
                newoNoDokumen.No = newNODok;
                newoNoDokumen.tipe = tipe;
                ctx.NoDokumenGenerators.Add(newoNoDokumen);
                ctx.SaveChanges();
                return newoNoDokumen.No;
            }
            else
            {
                var arrNo = oNoDokumen.No.Split('/');
                int NextNo = Convert.ToInt32(arrNo[0]);
                NextNo = NextNo + 1;
                int oldYear = Convert.ToInt32(arrNo[4]);
                string newNoDokmen = "";
                if (oldYear == DateTime.Now.Year)
                    newNoDokmen = NextNo.ToString() + KODE + Common.ConvertBulanRomawi(DateTime.Now.Month) + "/" + +DateTime.Now.Year;
                else newNoDokmen = "1" + KODE + Common.ConvertBulanRomawi(DateTime.Now.Month) + "/" + DateTime.Now.Year;

                NoDokumenGenerator newoNoDokumen = new NoDokumenGenerator();
                newoNoDokumen.CreateOn = DateTime.Now;
                newoNoDokumen.CreateBy = UserId;
                newoNoDokumen.No = newNoDokmen;
                newoNoDokumen.tipe = tipe;
                ctx.NoDokumenGenerators.Add(newoNoDokumen);
                ctx.SaveChanges();
                return newoNoDokumen.No;
            }
        }

        public List<VWBeritaAcaraEnd> getBeritaAcara(Guid PengadaanId, Guid UserId)
        {
            return ctx.BeritaAcaras.Where(d => d.PengadaanId == PengadaanId).Select(d => new VWBeritaAcaraEnd()
            {
                Id = d.Id,
                NoBeritaAcara = d.NoBeritaAcara,
                PengadaanId = d.PengadaanId,
                tanggal = d.tanggal,
                Tipe = d.Tipe,
                VendorId = d.VendorId
            }).ToList();
        }

        public BeritaAcara getBeritaAcaraByTipe(Guid PengadaanId, TipeBerkas tipe, Guid UserId)
        {
            return ctx.BeritaAcaras.Where(d => d.PengadaanId == PengadaanId && d.Tipe == tipe).FirstOrDefault();
        }

        public BeritaAcara getBeritaAcaraByTipeandVendor(Guid PengadaanId, TipeBerkas tipe, int VendorId, Guid UserId)
        {
            return ctx.BeritaAcaras.Where(d => d.PengadaanId == PengadaanId && d.Tipe == tipe && d.VendorId == VendorId).FirstOrDefault();
        }

        public BeritaAcara addBeritaAcara(BeritaAcara newBeritaAcara, Guid UserId)
        {
            Pengadaan opengadaan = ctx.Pengadaans.Find(newBeritaAcara.PengadaanId);
            if (opengadaan == null) return new BeritaAcara();
            BeritaAcara oBeritaAcara = new BeritaAcara();
            if (newBeritaAcara.VendorId > 0)
            {
                oBeritaAcara = ctx.BeritaAcaras.Where(d => d.PengadaanId == newBeritaAcara.PengadaanId
                            && d.Tipe == newBeritaAcara.Tipe && d.VendorId == newBeritaAcara.VendorId).FirstOrDefault();
            }
            else
            {
                oBeritaAcara = ctx.BeritaAcaras.Where(d => d.PengadaanId == newBeritaAcara.PengadaanId
                            && d.Tipe == newBeritaAcara.Tipe).FirstOrDefault();
            }
            if (oBeritaAcara == null)
            {
                if (newBeritaAcara.Tipe == TipeBerkas.BeritaAcaraPenentuanPemenang)
                {
                    newBeritaAcara.NoBeritaAcara = GenerateBeritaAcaraNota(UserId);
                }
                else if (newBeritaAcara.Tipe == TipeBerkas.SuratPerintahKerja)
                {
                    newBeritaAcara.NoBeritaAcara = GenerateBeritaAcaraSPK(UserId);
                }
                else
                {
                    newBeritaAcara.NoBeritaAcara = GenerateBeritaAcara(UserId);
                }
                ctx.BeritaAcaras.Add(newBeritaAcara);

                ctx.SaveChanges();

                return newBeritaAcara;
            }
            else
            {
                oBeritaAcara.tanggal = newBeritaAcara.tanggal;
                ctx.SaveChanges(UserId.ToString());
                return oBeritaAcara;
            }
        }

        public int DeleteBeritaAcara(Guid Id, Guid UserId)
        {
            var OberitaAcara = ctx.BeritaAcaras.Find(Id);
            if (OberitaAcara != null)
            {
                ctx.BeritaAcaras.Remove(OberitaAcara);
            }
            try
            {
                ctx.SaveChanges(UserId.ToString());
                return 1;
            }
            catch
            {
                return 0;
            }

        }

        public int CekBukaAmplop(Guid PengadaanId)
        {
            List<VWPErsetujuanBukaAmplop> lstPErsetujuan =
                 (from b in ctx.PersetujuanBukaAmplops
                  join c in ctx.PersonilPengadaans on b.PengadaanId equals c.PengadaanId //into ps
                  where b.PengadaanId == PengadaanId && c.PersonilId == b.UserId
                  select new VWPErsetujuanBukaAmplop
                  {
                      Id = b.Id,
                      tipe = c.tipe,
                      UserId = c.PersonilId,
                      PengadaanId = c.PengadaanId,
                  }).ToList();

            var PIC = lstPErsetujuan.Where(d => d.tipe == PengadaanConstants.StaffPeranan.PIC).FirstOrDefault();
            if (PIC == null) return 0;
            var User = lstPErsetujuan.Where(d => d.tipe == PengadaanConstants.StaffPeranan.Staff).FirstOrDefault();
            if (User == null) return 0;
            var Compl = lstPErsetujuan.Where(d => d.tipe == PengadaanConstants.StaffPeranan.Compliance).FirstOrDefault();
            if (Compl == null) return 0;
            var Contr = lstPErsetujuan.Where(d => d.tipe == PengadaanConstants.StaffPeranan.Controller).FirstOrDefault();
            if (Compl == null) return 0;

            Pengadaan Mpengadaaan = ctx.Pengadaans.Find(PengadaanId);
            //PersonilPengadaan picPersonil = ctx.PersonilPengadaans.Where(d => d.PersonilId == UserId && d.tipe == "pic" && d.PengadaanId == PengadaanId).FirstOrDefault();
            ///if (picPersonil == null) return 0;           
            //cekasuransibukan
            var asuransicek = ctx.InsuranceTarifs.Where(d => d.PengadaanId == PengadaanId).FirstOrDefault();

            var oKandidatPengadaan = ctx.KandidatPengadaans.Where(d => d.PengadaanId == Mpengadaaan.Id).ToList();
            if (oKandidatPengadaan.Count() > 0)
            {
                foreach (var item in oKandidatPengadaan)
                {
                    if (asuransicek == null)
                    {
                        var xSubmitRekanan = (from b in ctx.HargaRekanans
                                              join c in ctx.RKSDetails on b.RKSDetailId equals c.Id
                                              join d in ctx.RKSHeaders on c.RKSHeaderId equals d.Id
                                              where d.PengadaanId == PengadaanId && b.VendorId == item.VendorId
                                              select b).Distinct().ToList();
                        foreach (var itemx in xSubmitRekanan)
                        {
                            JimbisEncrypt encod = new JimbisEncrypt();
                            decimal? harga = encod.Decrypt(itemx.hargaEncrypt) == "" ? 0 : Convert.ToDecimal(encod.Decrypt(itemx.hargaEncrypt));
                            itemx.harga = harga;
                        }
                    }
                    else
                    {
                        var xSubmitRekananAsuransi = (from b in ctx.HargaRekananAsuransis
                                                      join c in ctx.BenefitRates on b.BenefitCodeId equals c.Id
                                                      join d in ctx.InsuranceTarifBenefits on c.Id equals d.BenefitRateId.Id
                                                      join e in ctx.InsuranceTarifs on d.DocumentId equals e.DocumentId
                                                      where e.PengadaanId == PengadaanId && b.VendorId == item.VendorId
                                                      select b).Distinct().ToList();
                        foreach (var itemx in xSubmitRekananAsuransi)
                        {
                            JimbisEncrypt encod = new JimbisEncrypt();
                            decimal? Rate = encod.Decrypt(itemx.RateEncrypt) == "" ? 0 : Convert.ToDecimal(encod.Decrypt(itemx.RateEncrypt));
                            decimal? RateLower = encod.Decrypt(itemx.RateLowerLimitEncrypt) == "" ? 0 : Convert.ToDecimal(encod.Decrypt(itemx.RateLowerLimitEncrypt));
                            decimal? RateUpper = encod.Decrypt(itemx.RateUpperLimitEncrypt) == "" ? 0 : Convert.ToDecimal(encod.Decrypt(itemx.RateUpperLimitEncrypt));
                            itemx.Rate = Rate;
                            itemx.RateLowerLimit = RateLower;
                            itemx.RateUpperLimit = RateUpper;
                        }
                    }

                }
            }

            ctx.SaveChanges();

            return 1;
        }
        public ViewVendors GetVendorByName(string NamaVendor)
        {
            var oVendor = ctx.Vendors.Where(d => d.Nama == NamaVendor).Select(d => new ViewVendors
            {
                id = d.Id,
                Alamat = d.Alamat,
                Email = d.Email,
                Nama = d.Nama,
                Owner = d.Owner
            }).FirstOrDefault();
            return oVendor;
        }

        public ViewVendors GetVendorById(int VendorId)
        {
            var oVendor = ctx.Vendors.Where(d => d.Id == VendorId).Select(d => new ViewVendors
            {
                id = d.Id,
                Alamat = d.Alamat,
                Email = d.Email,
                Nama = d.Nama,
                Owner = d.Owner
            }).FirstOrDefault();
            return oVendor;
        }

        public decimal? efisiensi(Guid Id, Guid UserId)
        {
            var totalHps = (from bb in ctx.RKSHeaders
                            join cc in ctx.RKSDetails on bb.Id equals cc.RKSHeaderId
                            where bb.PengadaanId == Id
                            select cc).Sum(xx => xx.hps) == null ? 0 :
                                 (from bb in ctx.RKSHeaders
                                  join cc in ctx.RKSDetails on bb.Id equals cc.RKSHeaderId
                                  where bb.PengadaanId == Id
                                  select cc).Sum(xx => xx.hps).Value;
            var totalReliat = getPemenangPengadaan(Id, UserId).FirstOrDefault() == null ? 0 :
                getPemenangPengadaan(Id, UserId).FirstOrDefault().total;

            if (totalReliat == null) return null;
            //if (totalHps == null) return null;
            if (totalHps.Equals(null)) return null;
            //var scoring = ((totalHps - totalReliat) / totalHps) * 100;
            //return scoring.Value;
            if (totalHps != 0)
            {
                var scoring = ((totalHps - totalReliat) / totalHps) * 100;
                return scoring.Value;
            }
            else
            {
                var scoring = '1';
                return scoring;
            }
        }

        public List<VWReportPengadaan> GetRepotPengadan(DateTime? dari, DateTime? sampai, Guid UserId, string divisi)
        {
            var oReport = (from b in ctx.Pengadaans
                           join c in ctx.PersonilPengadaans on b.Id equals c.PengadaanId
                           where b.TanggalMenyetujui >= dari && b.TanggalMenyetujui <= sampai && c.tipe == "pic" && b.UnitKerjaPemohon == divisi//c.tanggal >= dari && c.tanggal <= sampai// && c.Tipe == TipeBerkas.BeritaAcaraPenentuanPemenang
                           select new VWReportPengadaan
                           {
                               PengadaanId = b.Id,
                               PIC = c.Nama,
                               Judul = b.Judul,
                               User = b.UnitKerjaPemohon,
                               hps = (from bb in ctx.RKSHeaders
                                      join cc in ctx.RKSDetails on bb.Id equals cc.RKSHeaderId
                                      where bb.PengadaanId == b.Id
                                      select cc).Sum(xx => xx.hps) == null ? 0 :
                                    (from bb in ctx.RKSHeaders
                                     join cc in ctx.RKSDetails on bb.Id equals cc.RKSHeaderId
                                     where bb.PengadaanId == b.Id
                                     select cc).Sum(xx => xx.hps * xx.jumlah).Value
                           }).Distinct().ToList();
            foreach (var item in oReport)
            {
                item.realitas = getPemenangPengadaan(item.PengadaanId.Value, UserId).FirstOrDefault() == null ? 0 :
                    getPemenangPengadaan(item.PengadaanId.Value, UserId).FirstOrDefault().total;
                item.efisiensi = efisiensi(item.PengadaanId.Value, UserId);
                item.Pemenang = getPemenangPengadaan(item.PengadaanId.Value, UserId).FirstOrDefault() == null ? "" :
                            getPemenangPengadaan(item.PengadaanId.Value, UserId).FirstOrDefault().NamaVendor;
                item.Aanwjzing = getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.BeritaAcaraAanwijzing, UserId) == null ? null :
                        getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.BeritaAcaraAanwijzing, UserId).tanggal;
                item.PembukaanAmplop = getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.BeritaAcaraBukaAmplop, UserId) == null ? null :
                        getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.BeritaAcaraBukaAmplop, UserId).tanggal;
                item.Klasrifikasi = getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.BeritaAcaraKlarifikasi, UserId) == null ? null :
                    getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.BeritaAcaraKlarifikasi, UserId).tanggal;
                item.Scoring = getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.BeritaAcaraPenilaian, UserId) == null ? null :
                    getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.BeritaAcaraPenilaian, UserId).tanggal;
                item.NotaPemenang = getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.BeritaAcaraPenentuanPemenang, UserId) == null ? null :
                    getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.BeritaAcaraPenentuanPemenang, UserId).tanggal;
                item.SPK = getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.SuratPerintahKerja, UserId) == null ? null :
                    getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.SuratPerintahKerja, UserId).tanggal;
                item.KlasrifikasiLanjut = getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.BeritaAcaraKlarifikasiLanjutan, UserId) == null ? null :
                    getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.BeritaAcaraKlarifikasiLanjutan, UserId).tanggal;

            }
            return oReport;
        }

        public List<VWStaffCharges> GetSummaryTotal(DateTime dari, DateTime sampai, int limit = Int32.MaxValue, int skip = 0)
        {

            var fPengadaan = ctx.Pengadaans
                .Where(b => b.TanggalMenyetujui >= dari && b.TanggalMenyetujui <= sampai)
                .OrderBy(x => x.TanggalMenyetujui)
                .Skip(skip).Take(limit);
            VWStaffCharges v0 = new VWStaffCharges { Nama = "BERJALAN", Jumlah = fPengadaan.Where(x => x.Status != EStatusPengadaan.DRAFT && x.Status != EStatusPengadaan.DITOLAK && x.Status != EStatusPengadaan.DIBATALKAN && x.PersetujuanPemenangs.Where(d => d.PengadaanId == x.Id && d.Status == StatusPengajuanPemenang.APPROVED).FirstOrDefault() == null).Count() };
            VWStaffCharges v1 = new VWStaffCharges { Nama = "SELESAI", Jumlah = fPengadaan.Where(x => x.PersetujuanPemenangs.Where(d => d.PengadaanId == x.Id && d.Status == StatusPengajuanPemenang.APPROVED).FirstOrDefault() != null).Count() };
            VWStaffCharges v2 = new VWStaffCharges { Nama = "BATAL", Jumlah = fPengadaan.Where(x => x.Status == EStatusPengadaan.DIBATALKAN).Count() };
            return new List<VWStaffCharges> { v0, v1, v2 };
        }

        public List<VWProgressReport> GetProgressReport(DateTime dari, DateTime sampai, int limit = Int32.MaxValue, int skip = 0)
        {
            return ctx.Pengadaans
                .Where(b => b.TanggalMenyetujui >= dari && b.TanggalMenyetujui <= sampai)
                .OrderBy(x => new { x.Status, x.TanggalMenyetujui })//.OrderBy(x => x.Status)
                .Skip(skip).Take(limit)
                .Select(x => new VWProgressReport { Judul = x.Judul, Progress = (int)x.Status }).ToList();
        }

        public List<VWStaffCharges> GetStaffCharges(string charge, DateTime dari, DateTime sampai, int limit = Int32.MaxValue, int skip = 0)
        {
            List<Guid> lg = ctx.Pengadaans
                .Where(b => b.TanggalMenyetujui >= dari && b.TanggalMenyetujui <= sampai)
                .OrderBy(x => x.TanggalMenyetujui)
                .Skip(skip).Take(limit)
                .Select(x => x.Id).ToList();
            if (lg != null)
            {
                return ctx.PersonilPengadaans
                    .Where(x => lg.Contains((Guid)x.PengadaanId) && x.tipe == charge)
                    .GroupBy(x => new { Nama = x.Nama })
                    .Select(x => new VWStaffCharges
                    {
                        Nama = x.Key.Nama,
                        Jumlah = x.Count()
                    }).ToList();
            }
            return new List<VWStaffCharges>();
        }
        public int PembatalanPengadaan(VWPembatalanPengadaan vwPembatalan, Guid UserId)
        {


            var oPIC = ctx.PersonilPengadaans.Where(d => d.PengadaanId == vwPembatalan.PengadaanId && d.tipe == PengadaanConstants.StaffPeranan.PIC).FirstOrDefault();
            if (oPIC == null) return 0;

            var oPengadaan = ctx.Pengadaans.Find(vwPembatalan.PengadaanId);
            oPengadaan.Status = EStatusPengadaan.DIBATALKAN;
            oPengadaan.GroupPengadaan = EGroupPengadaan.ARSIP;

            PembatalanPengadaan oPembatalanPengadaan = ctx.PembatalanPengadaans.Where(d => d.PengadaanId == vwPembatalan.PengadaanId).FirstOrDefault();
            if (oPembatalanPengadaan == null)
            {
                oPembatalanPengadaan = new PembatalanPengadaan();

                oPembatalanPengadaan.PengadaanId = vwPembatalan.PengadaanId;
                oPembatalanPengadaan.Keterangan = vwPembatalan.Keterangan;
                oPembatalanPengadaan.CreateOn = DateTime.Now;
                oPembatalanPengadaan.CreateBy = UserId;
                ctx.PembatalanPengadaans.Add(oPembatalanPengadaan);
            }
            ctx.SaveChanges();
            return 1;
        }

        public PenolakanPengadaan GetPenolakanMessage(Guid Id, Guid UserId)
        {
            return ctx.PenolakanPengadaans.Where(d => d.PengadaanId == Id && d.status == 1).FirstOrDefault();
        }

        public PembatalanPengadaan GetPembatalanPengadaan(Guid Id, Guid UserId)
        {
            return ctx.PembatalanPengadaans.Where(d => d.PengadaanId == Id).FirstOrDefault();
        }

        public Vendor GetPemenang(Guid Id, Guid userId)
        {
            if (isSpkUploaded(Id, userId) == 0)
                return new Vendor();
            else
            {
                int VEndorId = ctx.PemenangPengadaans.Where(d => d.PengadaanId == Id).FirstOrDefault() == null ? 0 : ctx.PemenangPengadaans.Where(d => d.PengadaanId == Id).FirstOrDefault().VendorId.Value;
                if (VEndorId == 0) return new Vendor();
                else
                {
                    Vendor oVendor = ctx.Vendors.Where(d => d.Id == VEndorId).FirstOrDefault();
                    if (oVendor == null) return new Vendor();
                    else return new Vendor
                    {
                        Id = oVendor.Id,
                        Nama = oVendor.Nama,
                        Email = oVendor.Email,
                        Telepon = oVendor.Telepon
                    };
                }
            }

        }

        public Pengadaan ChangeStatusPengadaan(Guid Id, EStatusPengadaan status, Guid UserId)
        {
            Pengadaan oPengadaan = ctx.Pengadaans.Find(Id);
            try
            {
                if (oPengadaan.PengadaanLangsung == 1)
                {
                    oPengadaan.NoPengadaan = GenerateNoPengadaan(UserId);
                    oPengadaan.TanggalMenyetujui = DateTime.Now;
                    if (oPengadaan.AturanPengadaan == "Pengadaan Tertutup") oPengadaan.Status = EStatusPengadaan.SUBMITPENAWARAN;
                }
                else if (EStatusPengadaan.DISETUJUI == status)
                {
                    oPengadaan.NoPengadaan = GenerateNoPengadaan(UserId);
                    oPengadaan.TanggalMenyetujui = DateTime.Now;
                    if (oPengadaan.AturanPengadaan == "Pengadaan Tertutup") oPengadaan.Status = EStatusPengadaan.AANWIJZING;
                    oPengadaan.Status = status;
                }
                oPengadaan.GroupPengadaan = EGroupPengadaan.DALAMPELAKSANAAN;
                ctx.SaveChanges();
                return oPengadaan;
            }
            catch
            {
                return new Pengadaan();
            }

        }

        public int nextToStateWithChangeScheduldDate(Guid PengadaanId, Guid UserId, EStatusPengadaan state, DateTime? from, DateTime? to)
        {
            Pengadaan Mpengadaaan = ctx.Pengadaans.Find(PengadaanId);
            PersonilPengadaan picPersonil = ctx.PersonilPengadaans.Where(d => d.PersonilId == UserId && d.tipe == "pic" && d.PengadaanId == PengadaanId).FirstOrDefault();
            if (picPersonil == null) return 0;

            Mpengadaaan.Status = state;
            #region kalo anwajzing liat kehadirannya
            if (state == EStatusPengadaan.SUBMITPENAWARAN)
            {
                var kandidatHadir = ctx.KehadiranKandidatAanwijzings.Where(d => d.PengadaanId == PengadaanId).Select(d => d.VendorId).ToList();
                var kandidatTidakHadir = ctx.KandidatPengadaans.Where(d => !kandidatHadir.Contains(d.VendorId) && d.PengadaanId == PengadaanId).ToList();


                foreach (var item in kandidatTidakHadir)
                {
                    var HistoryKandidat = new HistoryKandidatPengadaan();
                    HistoryKandidat.PengadaanId = item.PengadaanId;
                    HistoryKandidat.VendorId = item.VendorId;
                    HistoryKandidat.addKandidatType = item.addKandidatType;
                    ctx.HistoryKandidatPengadaan.Add(HistoryKandidat);

                }
                ctx.KandidatPengadaans.RemoveRange(kandidatTidakHadir);
                ctx.SaveChanges();

            }
            #endregion
            #region rubah jadwal pengadaal
            JadwalPelaksanaan dtJadwl = ctx.JadwalPelaksanaans.Where(d => d.PengadaanId == PengadaanId && d.statusPengadaan == state).FirstOrDefault();
            if (dtJadwl == null)
            {
                JadwalPelaksanaan newJadwal = new JadwalPelaksanaan();
                if (from != null)
                    newJadwal.Mulai = from;
                if (state != EStatusPengadaan.PEMENANG)
                    newJadwal.Sampai = to;
                newJadwal.statusPengadaan = state;
                newJadwal.PengadaanId = PengadaanId;
                if (from != null || to != null)
                    ctx.JadwalPelaksanaans.Add(newJadwal);
            }
            else
            {

                if (from != null && to != null)
                {
                    dtJadwl.Mulai = from;
                    if (state != EStatusPengadaan.PEMENANG)
                        dtJadwl.Sampai = to;
                }
            }
            try
            {
                RiwayatDokumen newRiwayatPengadaan = new RiwayatDokumen();
                newRiwayatPengadaan.PengadaanId = PengadaanId;
                newRiwayatPengadaan.Status = state.ToString();
                newRiwayatPengadaan.ActionDate = DateTime.Now;
                ctx.RiwayatDokumens.Add(newRiwayatPengadaan);
                RiwayatPengadaan newRiwayatPengadaan2 = new RiwayatPengadaan();
                newRiwayatPengadaan2.PengadaanId = PengadaanId;
                newRiwayatPengadaan2.Status = state;
                newRiwayatPengadaan2.Waktu = DateTime.Now;
                ctx.RiwayatPengadaans.Add(newRiwayatPengadaan2);
                ctx.SaveChanges();

                if (state == EStatusPengadaan.PEMENANG)
                {
                    var dt = ctx.PelaksanaanPemilihanKandidats.Where(d => d.PengadaanId == PengadaanId).ToList();
                    var pemenang = ctx.PemenangPengadaans.Where(d => d.PengadaanId == PengadaanId).ToList();
                    ctx.PemenangPengadaans.RemoveRange(pemenang);
                    ctx.SaveChanges();
                    foreach (var item in dt)
                    {
                        var ndata = new PemenangPengadaan()
                        {
                            PengadaanId = PengadaanId,
                            VendorId = item.VendorId,
                            CreateOn = DateTime.Now,
                            CreatedBy = UserId
                        };
                        ctx.PemenangPengadaans.Add(ndata);
                        ctx.SaveChanges();
                    }
                }

                return 1;
            }
            catch
            {
                return 0;
            }
            #endregion



        }

        public List<PersonilPengadaan> getPersonilPengadaan(Guid PengadaanId)
        {
            try
            {
                return ctx.PersonilPengadaans.Where(d => d.PengadaanId == PengadaanId).ToList();
            }
            catch
            {
                return new List<PersonilPengadaan>();
            }

        }

        public List<VWRiwayatPengadaan> GetRiwayatDokumenForVendor(Guid UserId)
        {
            var UserVendor = ctx.Vendors.Where(d => d.Owner == UserId).FirstOrDefault();
            if (UserVendor == null) return new List<VWRiwayatPengadaan>();
            var lstRiwyatDokumen = (from b in ctx.RiwayatPengadaans
                                    join c in ctx.Pengadaans on b.PengadaanId equals c.Id
                                    join d in ctx.KandidatPengadaans on b.PengadaanId equals d.PengadaanId
                                    where d.VendorId == UserVendor.Id && b.Status > EStatusPengadaan.DISETUJUI
                                    select new VWRiwayatPengadaan
                                    {
                                        Waktu = b.Waktu,
                                        Komentar = b.Komentar,
                                        Id = b.Id,
                                        PengadaanId = b.PengadaanId,
                                        Status = b.Status.ToString(),
                                        JudulPengadaan = c.Judul
                                    }).ToList();
            return lstRiwyatDokumen;
        }

        public Reston.Helper.Util.ResultMessage saveReadyPersonil(Guid Id, int ready, Guid UserId)
        {
            var msg = new Reston.Helper.Util.ResultMessage();
            try
            {
                var kandidat = ctx.Pengadaans.Find(Id).PersonilPengadaans.Where(d => d.PersonilId == UserId);
                foreach (var item in kandidat)
                {
                    if (item.PersonilId != UserId)
                    {
                        msg.status = HttpStatusCode.Forbidden;
                        msg.message = Common.Deny();
                    }
                    item.isReady = ready;
                }
                ctx.SaveChanges(UserId.ToString());
                msg.status = HttpStatusCode.OK;
                msg.message = Common.SaveSukses();
                msg.Id = kandidat.FirstOrDefault().Id.ToString();
            }
            catch (Exception ex)
            {
                msg.status = HttpStatusCode.ExpectationFailed;
                msg.message = ex.ToString();
            }
            return msg;
        }

        public Reston.Helper.Util.ResultMessage CekPersetujuanPemenang(Guid Id, Guid UserId)
        {
            if (ctx.Pengadaans.Find(Id) == null) return new Reston.Helper.Util.ResultMessage();
            if (ctx.Pengadaans.Find(Id).PersetujuanPemenangs.Count() == 0) return new Reston.Helper.Util.ResultMessage();
            if (ctx.Pengadaans.Find(Id).PersetujuanPemenangs.FirstOrDefault().Status == StatusPengajuanPemenang.APPROVED)
            {
                return new Reston.Helper.Util.ResultMessage()
                {
                    Id = Id.ToString(),
                    message = "Pemenang Sudah Di Setujui",
                    status = HttpStatusCode.OK
                };
            }
            return new Reston.Helper.Util.ResultMessage();
        }

        public PersetujuanPemenang getPersetujuanPemenangByPengadaanId(Guid PengadaanId)
        {
            return ctx.PersetujuanPemenangs.Where(d => d.PengadaanId == PengadaanId).FirstOrDefault();
        }
        public PersetujuanPemenang getPersetujuanPemenangById(Guid Id)
        {
            return ctx.PersetujuanPemenangs.Where(d => d.Id == Id).FirstOrDefault();
        }
        public Reston.Helper.Util.ResultMessage SavePersetujuanPemenang(PersetujuanPemenang oPersetujuanPemenang, Guid UserId)
        {
            try
            {
                var oldData = ctx.PersetujuanPemenangs.Where(d => d.PengadaanId == oPersetujuanPemenang.PengadaanId).FirstOrDefault();
                if (oldData == null)
                {
                    oPersetujuanPemenang.CreatedOn = DateTime.Now;
                    oPersetujuanPemenang.CreatedBy = UserId;
                    ctx.PersetujuanPemenangs.Add(oPersetujuanPemenang);
                }
                else
                {
                    oldData.Note = oPersetujuanPemenang.Note;
                    oldData.Status = oPersetujuanPemenang.Status;
                    oldData.WorkflowId = oPersetujuanPemenang.WorkflowId;
                }
                ctx.SaveChanges(UserId.ToString());
                return new Reston.Helper.Util.ResultMessage()
                {
                    Id = oldData == null ? oPersetujuanPemenang.Id.ToString() : oldData.Id.ToString(),
                    message = Common.SaveSukses(),
                    status = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new Reston.Helper.Util.ResultMessage()
                {
                    message = ex.ToString(),
                    status = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public Reston.Helper.Util.ResultMessage MundurPersetujuan(Guid Id, ViewPengadaan MundurPersetujuan, Guid UserId)
        {
            try
            {
                var oldData = ctx.Pengadaans.Where(d => d.Id == Id).FirstOrDefault();
                if (oldData != null)
                {
                    oldData.Status = MundurPersetujuan.Status;
                    ctx.SaveChanges();
                }
                return new Reston.Helper.Util.ResultMessage()
                {
                    Id = oldData == null ? oldData.Id.ToString() : oldData.Id.ToString(),
                    message = Common.SaveSukses(),
                    status = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new Reston.Helper.Util.ResultMessage()
                {
                    message = ex.ToString(),
                    status = HttpStatusCode.ExpectationFailed
                };
            }
        }

        public Reston.Helper.Util.ResultMessage DeletePersetujuanPemenang(Guid Id)
        {
            try
            {
                var oldData = ctx.PersetujuanPemenangs.Find(Id);
                if (oldData != null)
                {
                    ctx.PersetujuanPemenangs.Remove(oldData);
                }
                ctx.SaveChanges();
                return new Reston.Helper.Util.ResultMessage()
                {
                    Id = Id.ToString(),
                    message = Common.SaveSukses(),
                    status = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new Reston.Helper.Util.ResultMessage()
                {
                    message = ex.ToString(),
                    status = HttpStatusCode.ExpectationFailed
                };
            }

        }

        public StatusPengajuanPemenang StatusPersetujuanPemenang(Guid PengadaanId)
        { 
            try
            {
                var persetujuan = ctx.PersetujuanPemenangs.Where(d => d.PengadaanId == PengadaanId).FirstOrDefault();
                if (persetujuan != null)
                {
                    return persetujuan.Status;
                }
                else
                {
                    return StatusPengajuanPemenang.BELUMDIAJUKAN;
                }
            }
            catch (Exception ex)
            {
                return StatusPengajuanPemenang.BELUMDIAJUKAN;
            }

        }

        public PersetujuanPemenang ChangeStatusPersetujuanPemenang(Guid Id, StatusPengajuanPemenang status, Guid UserId)
        {
            try
            {
                var odata = ctx.PersetujuanPemenangs.Find(Id);
                odata.Status = status;
                ctx.SaveChanges();
                return odata;
            }
            catch
            {
                return new PersetujuanPemenang();
            }
        }

        #region persetujuan tiap tahapan

        public int CekBukaAmplopTahapan(Guid PengadaanId)
        {

            try
            {
                Pengadaan Mpengadaaan = ctx.Pengadaans.Find(PengadaanId);

                //user kagak perlu persetujuan
                var jumPersonil = Mpengadaaan.PersonilPengadaans.Where(d => d.tipe != PengadaanConstants.StaffPeranan.Tim && d.tipe != PengadaanConstants.StaffPeranan.User).Select(d => d.PersonilId).Distinct().ToList();

                var jumTahapanPersetujuan = Mpengadaaan.PersetujuanTahapans.Where(d => d.StatusPengadaan == EStatusPengadaan.BUKAAMPLOP).Select(d => d.UserId).Distinct().ToList();
                // if (jumTahapanPersetujuan != jumPersonil) return 0;
                var cekBukaAmplop = true;
                foreach (var item in jumPersonil)
                {
                    if (!jumTahapanPersetujuan.Contains(item.Value))
                    {
                        cekBukaAmplop = false;
                    }
                }
                if (cekBukaAmplop == false) return 0;
                //cekasuransibukan
                var asuransicek = ctx.InsuranceTarifs.Where(d => d.PengadaanId == PengadaanId).FirstOrDefault();

                var oKandidatPengadaan = ctx.KandidatPengadaans.Where(d => d.PengadaanId == Mpengadaaan.Id).ToList();
                if (oKandidatPengadaan.Count() > 0)
                {

                    foreach (var item in oKandidatPengadaan)
                    {
                        if (asuransicek == null)
                        {
                            var xSubmitRekanan = (from b in ctx.HargaRekanans
                                                  join c in ctx.RKSDetails on b.RKSDetailId equals c.Id
                                                  join d in ctx.RKSHeaders on c.RKSHeaderId equals d.Id
                                                  where d.PengadaanId == PengadaanId && b.VendorId == item.VendorId
                                                  select b).Distinct().ToList();
                            foreach (var itemx in xSubmitRekanan)
                            {
                                JimbisEncrypt encod = new JimbisEncrypt();
                                decimal? harga = encod.Decrypt(itemx.hargaEncrypt) == "" ? 0 : Convert.ToDecimal(encod.Decrypt(itemx.hargaEncrypt));
                                itemx.harga = harga;
                            }
                        }
                        else
                        {
                            var xSubmitRekananAsuransi = (from b in ctx.HargaRekananAsuransis
                                                          join c in ctx.BenefitRates on b.BenefitCodeId equals c.Id
                                                          join d in ctx.InsuranceTarifBenefits on c.Id equals d.BenefitRateId.Id
                                                          join e in ctx.InsuranceTarifs on d.DocumentId equals e.DocumentId
                                                          where e.PengadaanId == PengadaanId && b.VendorId == item.VendorId
                                                          select b).Distinct().ToList();
                            foreach (var itemx in xSubmitRekananAsuransi)
                            {
                                JimbisEncrypt encod = new JimbisEncrypt();
                                decimal? Rate = itemx.RateEncrypt == null ? 0 : Convert.ToDecimal(encod.Decrypt(itemx.RateEncrypt));
                                decimal? RateLower = itemx.RateLowerLimitEncrypt == null ? 0 : Convert.ToDecimal(encod.Decrypt(itemx.RateLowerLimitEncrypt));
                                decimal? RateUpper = itemx.RateUpperLimitEncrypt == null ? 0 : Convert.ToDecimal(encod.Decrypt(itemx.RateUpperLimitEncrypt));
                                itemx.Rate = Rate;
                                itemx.RateLowerLimit = RateLower;
                                itemx.RateUpperLimit = RateUpper;
                            }
                        }
                    }
                }

                ctx.SaveChanges();

                return 1;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return 0;
            }
        }

        public PersetujuanTahapan SavePersetujuanTahapan(PersetujuanTahapan data, Guid UserId)
        {
            var dataPengadaan = ctx.Pengadaans.Find(data.PengadaanId);
            if (dataPengadaan == null) return new PersetujuanTahapan();
            if (dataPengadaan.AturanPengadaan == "Pengadaan Tertutup" && dataPengadaan.Status == EStatusPengadaan.DISETUJUI)
            {
                dataPengadaan.Status = EStatusPengadaan.AANWIJZING;
            }

            var dataTahapan = dataPengadaan.PersetujuanTahapans.Where(d => d.UserId == UserId && d.StatusPengadaan == data.StatusPengadaan && d.PengadaanId == data.PengadaanId).FirstOrDefault();
            if (dataPengadaan.Status != data.StatusPengadaan) return new PersetujuanTahapan();
            if (dataTahapan == null)
            {
                data.UserId = UserId;
                data.CreatedBy = UserId;
                data.CreatedOn = DateTime.Now;
                ctx.PersetujuanTahapans.Add(data);
            }
            else
            {
                dataTahapan.ModifiedBy = UserId;
                dataTahapan.ModifiedOn = DateTime.Now;
            }
            ctx.SaveChanges(UserId.ToString());
            return new PersetujuanTahapan();
        }

        public List<VWPersetujuanTahapan> GetPersetujuanTahapan(Guid PengadaanId, EStatusPengadaan status)
        {
            var dataPengadaan = ctx.Pengadaans.Find(PengadaanId);
            if (dataPengadaan == null) return new List<VWPersetujuanTahapan>();
            var dataPersonil = dataPengadaan.PersonilPengadaans;
            if (dataPersonil == null) return new List<VWPersetujuanTahapan>();
            var dataPersonilTahapan = dataPersonil.Where(d => d.tipe != PengadaanConstants.StaffPeranan.Tim);
            //kalo buka amplop sama submit penawaran user tidak perlu persetujuan
            if (status == EStatusPengadaan.SUBMITPENAWARAN || status == EStatusPengadaan.BUKAAMPLOP)
            {
                dataPersonilTahapan = dataPersonilTahapan.Where(d => d.tipe != PengadaanConstants.StaffPeranan.User);
            }
            List<VWPersetujuanTahapan> lstVWPersetujuanTahapan = new List<VWPersetujuanTahapan>();
            foreach (var item in dataPersonilTahapan)
            {
                VWPersetujuanTahapan nVWPersetujuanTahapan = new VWPersetujuanTahapan();

                var Tahapan = dataPengadaan.PersetujuanTahapans
                                        .Where(d => d.UserId == item.PersonilId && d.StatusPengadaan == status).FirstOrDefault();
                if (Tahapan != null)
                {
                    nVWPersetujuanTahapan.Id = Tahapan.Id;
                    nVWPersetujuanTahapan.CreatedOn = Tahapan.CreatedOn;
                }
                nVWPersetujuanTahapan.PengadaanId = item.PengadaanId;
                nVWPersetujuanTahapan.Status = Tahapan == null ? StatusTahapan.Requested : Tahapan.Status;
                nVWPersetujuanTahapan.UserId = item.PersonilId;
                nVWPersetujuanTahapan.StatusPengadaan = status;
                nVWPersetujuanTahapan.StatusPengadaanName = status.ToString();


                lstVWPersetujuanTahapan.Add(nVWPersetujuanTahapan);
            }

            return lstVWPersetujuanTahapan;
        }


        #endregion

        #region pengadaanTerbuka
        public KandidatPengadaan addKandidatPilihanVendor(KandidatPengadaan oKandidatPengadaan, Guid UserId)
        {
            Pengadaan Mpengadaaan = ctx.Pengadaans.Find(oKandidatPengadaan.PengadaanId);

            KandidatPengadaan oldKandidatPengadaan =
                    ctx.KandidatPengadaans.Where(d => d.PengadaanId == oKandidatPengadaan.PengadaanId
                                    && d.VendorId == oKandidatPengadaan.VendorId).FirstOrDefault();
            if (oldKandidatPengadaan == null)
            {
                ctx.KandidatPengadaans.Add(oKandidatPengadaan);
                ctx.SaveChanges(UserId.ToString());

            }
            return oKandidatPengadaan;
        }

        public List<ViewPengadaan> GetPengadaanAnnouncment()
        {

            return ctx.Pengadaans.Where(b => b.Status == EStatusPengadaan.DISETUJUI).Select(b => new ViewPengadaan()
            {
                NoPengadaan = b.NoPengadaan,
                Id = b.Id,
                Judul = b.Judul,
                AturanBerkas = b.AturanBerkas,
                AturanPenawaran = b.AturanPenawaran,
                AturanPengadaan = b.AturanPengadaan,
                Keterangan = b.Keterangan,
                Status = b.Status,
                StatusName = b.Status.ToString(),
                GroupPengadaan = b.GroupPengadaan,
                JenisPekerjaan = b.JenisPekerjaan,
                JenisPembelanjaan = b.JenisPembelanjaan,
                MataUang = b.MataUang,
                PeriodeAnggaran = b.PeriodeAnggaran,
                NoCOA = b.NoCOA,
                Pagu = b.Pagu,
                Region = b.Region,
                Provinsi = b.Provinsi,
                KualifikasiRekan = b.KualifikasiRekan,
                UnitKerjaPemohon = b.UnitKerjaPemohon,
                TitleDokumenNotaInternal = b.TitleDokumenNotaInternal,
                TitleDokumenLain = b.TitleDokumenLain,
                TitleBerkasRujukanLain = b.TitleBerkasRujukanLain,
                Mulai = b.JadwalPelaksanaans.Where(d => d.statusPengadaan == EStatusPengadaan.DISETUJUI).FirstOrDefault() == null ? b.JadwalPengadaans.Where(d => d.tipe == PengadaanConstants.Jadwal.Pendaftaran).FirstOrDefault().Mulai : b.JadwalPelaksanaans.Where(d => d.statusPengadaan == EStatusPengadaan.DISETUJUI).FirstOrDefault().Mulai,
                Sampai = b.JadwalPelaksanaans.Where(d => d.statusPengadaan == EStatusPengadaan.DISETUJUI).FirstOrDefault() == null ? b.JadwalPengadaans.Where(d => d.tipe == PengadaanConstants.Jadwal.Pendaftaran).FirstOrDefault().Sampai : b.JadwalPelaksanaans.Where(d => d.statusPengadaan == EStatusPengadaan.DISETUJUI).FirstOrDefault().Sampai
            }).Where(d => d.Sampai >= DateTime.Now).ToList();


            //return (from b in ctx.Pengadaans
            //        join c in ctx.JadwalPengadaans on b.Id equals c.PengadaanId
            //        where b.AturanPengadaan == "Pengadaan Terbuka" && 
            //        c.tipe==PengadaanConstants.Jadwal.Pendaftaran && c.Sampai >= DateTime.Now && c.Mulai <= DateTime.Now
            //         && b.Status == EStatusPengadaan.DISETUJUI
            //        select b).Distinct().ToList();



        }
        #endregion

        #region tambah tahapan
        public LewatTahapan SaveTahapan(LewatTahapan data, Guid UserId)
        {
            var ckdata = ctx.Pengadaans.Find(data.PengadaanId);
            if (ckdata == null) return new LewatTahapan();

            var ndata = ctx.LewatTahapans.Where(d => d.PengadaanId == data.PengadaanId && d.Status == data.Status).FirstOrDefault();
            if (ndata != null)
            {
                ctx.LewatTahapans.Remove(ndata);
                ctx.SaveChanges(UserId.ToString());

                return new LewatTahapan();

            }
            else
            {
                data.CreatedBy = UserId;
                data.CreatedOn = DateTime.Now;
                ctx.LewatTahapans.Add(data);
                ctx.SaveChanges(UserId.ToString());
                return data;
            }

        }

        //public LewatTahapan MundurTahapan(Guid Id)
        //{
        //    var ckdata = ctx.Pengadaans.Find(Id);
        //    if (ckdata == null) return new LewatTahapan();

        //    var oPengadaan = ctx.Pengadaans.Find(Id);
        //    oPengadaan.Status = EStatusPengadaan.KLARIFIKASILANJUTAN;

        //    var ndata = ctx.LewatTahapans.Where(d => d.PengadaanId == Id && d.Status == oPengadaan.Status).FirstOrDefault();
        //    if (ndata != null)
        //    {
        //        ctx.LewatTahapans.Remove(ndata);
        //        //ctx.SaveChanges(UserId.ToString());

        //        return new LewatTahapan();

        //    }
        //    else
        //    {
        //        //ndata.CreatedBy = UserId;
        //        ndata.CreatedOn = DateTime.Now;
        //        ctx.LewatTahapans.Add(ndata);
        //        //ctx.SaveChanges(UserId.ToString());
        //        return ndata;
        //    }

        //}

        public List<LewatTahapan> getTahapan(Guid PengadaanId)
        {
            return ctx.LewatTahapans.Where(d => d.PengadaanId == PengadaanId).ToList();
        }
        #endregion

        #region persetujuan terkait
        public PersetujuanTerkait savePersetujuanTerkait(PersetujuanTerkait data)
        {
            var pengdaan = ctx.Pengadaans.Find(data.PengadaanId);
            if (pengdaan == null) return new PersetujuanTerkait();
            var odata = ctx.PersetujuanTerkait.Where(d => d.PengadaanId == data.PengadaanId && d.UserId == data.UserId).FirstOrDefault();
            if (odata != null) return odata;
            else
            {
                ctx.PersetujuanTerkait.Add(data);
                ctx.SaveChanges();
            }

            return data;
        }

        public int deletePersetujuanTerkait(Guid Id, Guid UserId)
        {
            try
            {
                var persetujuanTerkait = ctx.PersetujuanTerkait.Find(Id);
                if (persetujuanTerkait == null) return 0;
                var isPic = persetujuanTerkait.Pengadaan.PersonilPengadaans.Where(d => d.tipe == PengadaanConstants.StaffPeranan.PIC && d.PersonilId == UserId).FirstOrDefault() == null ? 0 : 1;
                if (isPic == 0) return 0;
                ctx.PersetujuanTerkait.Remove(persetujuanTerkait);
                ctx.SaveChanges();
                return 1;
            }
            catch { return 0; }
        }

        public PersetujuanTerkait TerkaitSetuju(PersetujuanTerkait data)
        {
            var pengdaan = ctx.Pengadaans.Find(data.PengadaanId);
            if (pengdaan == null) return new PersetujuanTerkait();
            var odata = ctx.PersetujuanTerkait.Where(d => d.PengadaanId == data.PengadaanId && d.UserId == data.UserId);
            if (odata == null) return new PersetujuanTerkait();
            else
            {
                foreach (var item in odata)
                {
                    item.setuju = data.setuju;
                    item.CommentPersetujuanTerkait = data.CommentPersetujuanTerkait;
                }
                ctx.SaveChanges();
            }

            return data;
        }

        public List<PersetujuanTerkait> GetUserTerkait(Guid PengadaanId)
        {
            return ctx.PersetujuanTerkait.Where(d => d.PengadaanId == PengadaanId).ToList();
        }

        public List<VWPOReportDetail> GetReportPO(DateTime? dari, DateTime? sampai, Guid UserId)
        {
            throw new NotImplementedException();
        }

        public List<VWReportPks> GetReportPKS(DateTime? dari, DateTime? sampai, Guid UserId)
        {
            throw new NotImplementedException();
        }

        public List<VWReportSpk> GetReportSPK(DateTime? dari, DateTime? sampai, Guid UserId)
        {
            throw new NotImplementedException();
        }

        #endregion

        public List<Vendor> GetCetakVendor()
        {
            return ctx.Vendors.Where(d => d.StatusAkhir == EStatusVendor.VERIFIED).ToList();
        }

        public List<ViewVendors> GetAllVendors()
        {
            return ctx.Vendors.Where(d => d.StatusAkhir == EStatusVendor.VERIFIED).Select(d => new ViewVendors
            {
                id = d.Id,
                Alamat = d.Alamat,
                Email = d.Email,
                KodePos = d.KodePos,
                Kota = d.Kota,
                Nama = d.Nama,
                Owner = d.Owner,
                Provinsi = d.Provinsi,
                StatusAkhir = d.StatusAkhir.ToString(),
                Telepon = d.Telepon,
                strTipeVendor = d.TipeVendor.ToString(),
                Website = d.Website,
                VendorPerson = d.VendorPerson.FirstOrDefault(),
                NPWP = ctx.DokumenDetails.Where(xx => xx.Id == d.Dokumen.Where(x => x.TipeDokumen == EDocumentType.NPWP).Select(x => x.Id).FirstOrDefault()).FirstOrDefault()
            }).ToList();
        }

        public InsuranceTarif InsuranceTarif(Guid PengadaanId, Guid UserId)
        {
            var cekadaga = ctx.InsuranceTarifs.Where(d => d.PengadaanId == PengadaanId).FirstOrDefault();

            if (cekadaga != null)
            {
                return cekadaga;
            }
            else
            {
                var userId = UserId;
                var newDoc = new InsuranceTarif()
                {
                    PengadaanId = PengadaanId,
                    DocumentId = Guid.NewGuid(),
                    DocumentTitle = "(Belum Ada Judul)",
                    CreatedBy = userId.ToString(),
                    CreatedTS = DateTime.Now,
                    BenefitType = ""
                };

                ctx.InsuranceTarifs.Add(newDoc);
                ctx.SaveChanges();

                return newDoc;
            }

        }


        public List<ViewBenefitRate> saveRksAsuransiFromTemplate(Guid PengadaanId, Guid UserId, Guid DocumentIdBaru, Guid DocumentIdLama)
        {

            // cek udh ada belum 
            var trf = (from a in ctx.InsuranceTarifBenefits
                       join b in ctx.BenefitRates on a.BenefitRateId.Id equals b.Id
                       where a.DocumentId == DocumentIdBaru
                       select new ViewTarifBenefit2
                       {
                           Id = a.Id,
                           BenefitRateId = a.BenefitRateId,
                           DocumentId = a.DocumentId
                       }).ToList();

            if (trf.Count() > 0)
            {
                foreach (var i in trf)
                {
                    var vwcooper = new InsuranceTarifBenefit();
                    vwcooper.Id = i.Id;
                    vwcooper.DocumentId = i.DocumentId;
                    vwcooper.BenefitRateId = i.BenefitRateId;

                    ctx.BenefitRates.Remove(vwcooper.BenefitRateId);

                    var vwmini = ctx.InsuranceTarifBenefits.Where(d => d.Id == vwcooper.Id).FirstOrDefault();

                    ctx.InsuranceTarifBenefits.Remove(vwmini);
                    ctx.SaveChanges();
                }
            }

            // call lama
            var benefitrates = (from b in ctx.InsuranceTarifBenefitTemplates
                                join c in ctx.BenefitRateTemplates on b.BenefitRateId.Id equals c.Id
                                where b.DocumentId == DocumentIdLama
                                select new ViewBenefitRate
                                {
                                    BenefitCode = c.BenefitCode,
                                    BenefitCoverage = c.BenefitCoverage,
                                    RegionCode = c.RegionCode,
                                    Rate = c.Rate,
                                    RateLowerLimit = c.RateLowerLimit,
                                    RateUpperLimit = c.RateUpperLimit,
                                    IsRange = c.IsRange,
                                    IsOpen = c.IsOpen,
                                    Id = c.Id
                                }).ToList();

            foreach (var i in benefitrates)
            {

                var baru = new BenefitRate();
                baru.Id = i.Id;
                baru.BenefitCode = i.BenefitCode;
                baru.BenefitCoverage = i.BenefitCoverage;
                baru.RegionCode = i.RegionCode;
                baru.Rate = i.Rate;
                baru.RateLowerLimit = i.RateLowerLimit;
                baru.RateUpperLimit = i.RateUpperLimit;
                baru.IsRange = i.IsRange;
                baru.IsOpen = i.IsOpen;

                var tarifbenefit = new InsuranceTarifBenefit();

                tarifbenefit.DocumentId = DocumentIdBaru;
                tarifbenefit.BenefitRateId = baru;

                ctx.InsuranceTarifBenefits.Add(tarifbenefit);
                ctx.SaveChanges();
            }
            return benefitrates;
        }

        public List<ViewBenefitRate> saveRksAsuransiToTemplate(Guid PengadaanId, Guid UserId, Guid DocumentIdBaru)
        {

            // cek udh ada belum 
            var trf = (from a in ctx.InsuranceTarifBenefitTemplates
                       join b in ctx.BenefitRateTemplates on a.BenefitRateId.Id equals b.Id
                       where a.DocumentId == DocumentIdBaru
                       select new ViewTarifBenefit3
                       {
                           Id = a.Id,
                           BenefitRateId = a.BenefitRateId,
                           DocumentId = a.DocumentId
                       }).ToList();


            if (trf.Count() > 0)
            {
                foreach (var i in trf)
                {
                    var odtemplate = new InsuranceTarifBenefitTemplate();
                    odtemplate.Id = i.Id;
                    odtemplate.DocumentId = i.DocumentId;
                    odtemplate.BenefitRateId = i.BenefitRateId;

                    ctx.BenefitRateTemplates.Remove(odtemplate.BenefitRateId);

                    var ndtemplate = ctx.InsuranceTarifBenefitTemplates.Where(d => d.Id == odtemplate.Id).FirstOrDefault();

                    ctx.InsuranceTarifBenefitTemplates.Remove(ndtemplate);

                    var ntartem = ctx.InsuranceTarifTemplates.Where(a => a.DocumentId == odtemplate.DocumentId).FirstOrDefault();

                    if (ntartem != null)
                    {
                        ctx.InsuranceTarifTemplates.Remove(ntartem);
                    }

                    ctx.SaveChanges();
                }
            }

            var userId = UserId;
            //var newId = Guid.NewGuid();
            var tariftemplate = ctx.InsuranceTarifs.Where(a => a.DocumentId == DocumentIdBaru).FirstOrDefault();
            var tariftotemplate = new InsuranceTarifTemplate();
            tariftotemplate.DocumentTitle = tariftemplate.DocumentTitle;
            tariftotemplate.DocumentId = DocumentIdBaru;
            tariftotemplate.BenefitType = tariftemplate.BenefitType;
            tariftotemplate.CreatedBy = userId.ToString();
            tariftotemplate.CreatedTS = DateTime.Now;

            ctx.InsuranceTarifTemplates.Add(tariftotemplate);
            ctx.SaveChanges();

            // call lama
            var benefitratetemplates = (from b in ctx.InsuranceTarifBenefits
                                        join c in ctx.BenefitRates on b.BenefitRateId.Id equals c.Id
                                        where b.DocumentId == DocumentIdBaru
                                        select new ViewBenefitRate
                                        {
                                            BenefitCode = c.BenefitCode,
                                            BenefitCoverage = c.BenefitCoverage,
                                            RegionCode = c.RegionCode,
                                            Rate = c.Rate,
                                            RateLowerLimit = c.RateLowerLimit,
                                            RateUpperLimit = c.RateUpperLimit,
                                            IsRange = c.IsRange,
                                            IsOpen = c.IsOpen,
                                            Id = c.Id
                                        }).ToList();

            foreach (var i in benefitratetemplates)
            {
                var qq = ctx.BenefitRateTemplates.OrderByDescending(d => d.Id).FirstOrDefault();

                var baru = new BenefitRateTemplate();
                //baru.Id = Convert.ToInt32(qq.Id)+1;
                baru.Id = i.Id;
                baru.BenefitCode = i.BenefitCode;
                baru.BenefitCoverage = i.BenefitCoverage;
                baru.RegionCode = i.RegionCode;
                baru.Rate = i.Rate;
                baru.RateLowerLimit = i.RateLowerLimit;
                baru.RateUpperLimit = i.RateUpperLimit;
                baru.IsRange = i.IsRange;
                baru.IsOpen = i.IsOpen;

                var tarifbenefittemplate = new InsuranceTarifBenefitTemplate();

                tarifbenefittemplate.DocumentId = DocumentIdBaru;
                tarifbenefittemplate.BenefitRateId = baru;

                ctx.InsuranceTarifBenefitTemplates.Add(tarifbenefittemplate);
                ctx.SaveChanges();
            }
            return benefitratetemplates;
        }

        public DataTableBenefit GetDataAsuransi(Guid DocumentIdBaru)
        {
            DataTableBenefit tp = new DataTableBenefit();
            // record total yang tampil 
            tp.recordsTotal = (from a in ctx.InsuranceTarifs
                               join b in ctx.InsuranceTarifBenefits on a.DocumentId equals b.DocumentId
                               join c in ctx.BenefitRates on b.BenefitRateId.Id equals c.Id
                               join d in ctx.ReferenceDatas on c.BenefitCode equals d.Code
                               join e in ctx.ReferenceDatas on c.BenefitCoverage equals e.Code
                               join f in ctx.ReferenceDatas on c.RegionCode equals f.Code
                               where a.DocumentId == DocumentIdBaru
                               select new ViewBenefitRate
                               {
                                   Id = c.Id,
                                   BenefitCode = d.LocalizedName,
                                   BenefitCoverage = e.LocalizedName,
                                   RegionCode = f.LocalizedName,
                                   Rate = c.Rate == null ? null : c.Rate,
                                   RateLowerLimit = c.RateLowerLimit == null ? c.RateLowerLimit : c.RateLowerLimit,
                                   RateUpperLimit = c.RateUpperLimit == null ? c.RateUpperLimit : c.RateUpperLimit,
                                   IsRange = c.IsRange == null ? c.IsRange : c.IsRange,
                                   IsOpen = c.IsOpen == null ? c.IsOpen : c.IsOpen
                               }).Count();

            // filter berdasarkan Id
            tp.recordsFiltered = (from a in ctx.InsuranceTarifs
                                  join b in ctx.InsuranceTarifBenefits on a.DocumentId equals b.DocumentId
                                  join c in ctx.BenefitRates on b.BenefitRateId.Id equals c.Id
                                  join d in ctx.ReferenceDatas on c.BenefitCode equals d.Code
                                  join e in ctx.ReferenceDatas on c.BenefitCoverage equals e.Code
                                  join f in ctx.ReferenceDatas on c.RegionCode equals f.Code
                                  where a.DocumentId == DocumentIdBaru
                                  select new ViewBenefitRate
                                  {
                                      Id = c.Id,
                                      BenefitCode = d.LocalizedName,
                                      BenefitCoverage = e.LocalizedName,
                                      RegionCode = f.LocalizedName,
                                      Rate = c.Rate == null ? null : c.Rate,
                                      RateLowerLimit = c.RateLowerLimit == null ? c.RateLowerLimit : c.RateLowerLimit,
                                      RateUpperLimit = c.RateUpperLimit == null ? c.RateUpperLimit : c.RateUpperLimit,
                                      IsRange = c.IsRange == null ? c.IsRange : c.IsRange,
                                      IsOpen = c.IsOpen == null ? c.IsOpen : c.IsOpen
                                  }).Count();

            var cariBenefitRate = (from a in ctx.InsuranceTarifs
                                   join b in ctx.InsuranceTarifBenefits on a.DocumentId equals b.DocumentId
                                   join c in ctx.BenefitRates on b.BenefitRateId.Id equals c.Id
                                   join d in ctx.ReferenceDatas on c.BenefitCode equals d.Code
                                   join e in ctx.ReferenceDatas on c.BenefitCoverage equals e.Code
                                   join f in ctx.ReferenceDatas on c.RegionCode equals f.Code
                                   where a.DocumentId == DocumentIdBaru
                                   select new ViewBenefitRate
                                   {
                                       Id = c.Id,
                                       BenefitCode = d.LocalizedName,
                                       BenefitCoverage = e.LocalizedName,
                                       RegionCode = f.LocalizedName,
                                       Rate = c.Rate == null ? null : c.Rate,
                                       RateLowerLimit = c.RateLowerLimit == null ? c.RateLowerLimit : c.RateLowerLimit,
                                       RateUpperLimit = c.RateUpperLimit == null ? c.RateUpperLimit : c.RateUpperLimit,
                                       FlagAttr1 = d.FlagAttr1 == null ? d.FlagAttr1 : d.FlagAttr1,
                                       IsRange = c.IsRange == null ? c.IsRange : c.IsRange,
                                       IsOpen = c.IsOpen == null ? c.IsOpen : c.IsOpen
                                   }).ToList();

            List<ViewBenefitRate> vListBenefitRate = new List<ViewBenefitRate>();
            foreach (var item in cariBenefitRate)
            {
                ViewBenefitRate nViewListBenefitRate = new ViewBenefitRate();
                nViewListBenefitRate.Id = item.Id;
                nViewListBenefitRate.BenefitCode = item.BenefitCode;
                nViewListBenefitRate.BenefitCoverage = item.BenefitCoverage;
                nViewListBenefitRate.RegionCode = item.RegionCode;
                nViewListBenefitRate.Rate = item.Rate;
                nViewListBenefitRate.RateLowerLimit = item.RateLowerLimit;
                nViewListBenefitRate.RateUpperLimit = item.RateUpperLimit;
                nViewListBenefitRate.FlagAttr1 = item.FlagAttr1;
                nViewListBenefitRate.IsRange = item.IsRange;
                nViewListBenefitRate.IsOpen = item.IsOpen;
                vListBenefitRate.Add(nViewListBenefitRate);
            }
            tp.data = cariBenefitRate.Select(
                aa => new ViewBenefitRate
                {
                    Id = aa.Id,
                    BenefitCode = aa.BenefitCode,
                    BenefitCoverage = aa.BenefitCoverage,
                    RegionCode = aa.RegionCode,
                    Rate = aa.Rate,
                    RateLowerLimit = aa.RateLowerLimit,
                    RateUpperLimit = aa.RateUpperLimit,
                    FlagAttr1 = aa.FlagAttr1,
                    IsRange = aa.IsRange,
                    IsOpen = aa.IsOpen
                }).OrderByDescending(c => c.FlagAttr1).ThenBy(d => d.BenefitCode).ToList();

            return tp;
        }
        public ViewCekRKSBiasaAtauAsuransi cekRKSBiasaAtauAsuransi(Guid PengadaanId)
        {
            var apakahBiasa = ctx.RKSHeaders.Where(d => d.PengadaanId == PengadaanId).FirstOrDefault();
            var apakahAsuransi = ctx.InsuranceTarifs.Where(d => d.PengadaanId == PengadaanId).FirstOrDefault();

            ViewCekRKSBiasaAtauAsuransi cek = new ViewCekRKSBiasaAtauAsuransi();
            if (apakahBiasa != null)
            {
                cek.RKSBiasa = true;
            }
            else
            {
                cek.RKSBiasa = false;
            }

            if (apakahAsuransi != null)
            {
                cek.RKSAsuransi = true;
            }
            else
            {
                cek.RKSAsuransi = false;
            }

            return cek;
        }

        public Reston.Helper.Util.ResultMessage deleteBenef(int Id, Guid UserId)
        {
            BenefitRate oData = ctx.BenefitRates.Find(Id);
            var oLstDataDetail = ctx.InsuranceTarifBenefits.Where(d => d.BenefitRateId.Id == oData.Id);
            ctx.InsuranceTarifBenefits.RemoveRange(oLstDataDetail);
            ctx.BenefitRates.Remove(oData);
            try
            {
                //oData.DeletedBy = UserId.ToString();
                //oData.DeletedTS = DateTime.Now;
                ctx.SaveChanges();
                //ctx.SaveChanges(UserId.ToString());
                msg.status = HttpStatusCode.OK;
                msg.message = "Sukses";
            }
            catch (Exception ex)
            {
                msg.status = HttpStatusCode.ExpectationFailed;
                msg.message = ex.ToString();
            }
            return msg;
        }

        public DataTableBenefit GetDataHargaAsuransi(Guid PengadaanId, Guid userId)
        {
            DataTableBenefit tp = new DataTableBenefit();

            var vendor_id = ctx.Vendors.Where(d => d.Owner == userId).FirstOrDefault().Id;
            var cariBenefitRateHPS = (from a in ctx.InsuranceTarifs
                                      join b in ctx.InsuranceTarifBenefits on a.DocumentId equals b.DocumentId
                                      join c in ctx.BenefitRates on b.BenefitRateId.Id equals c.Id
                                      join d in ctx.ReferenceDatas on c.BenefitCode equals d.Code
                                      join e in ctx.ReferenceDatas on c.BenefitCoverage equals e.Code
                                      join f in ctx.ReferenceDatas on c.RegionCode equals f.Code
                                      where a.PengadaanId == PengadaanId
                                      select new ViewBenefitRate
                                      {
                                          Id = c.Id,
                                          BenefitCode = d.LocalizedName,
                                          BenefitCoverage = e.LocalizedName,
                                          RegionCode = f.LocalizedName,
                                          Rate = c.Rate == null ? null : c.Rate,
                                          RateLowerLimit = c.RateLowerLimit == null ? c.RateLowerLimit : c.RateLowerLimit,
                                          RateUpperLimit = c.RateUpperLimit == null ? c.RateUpperLimit : c.RateUpperLimit,
                                          FlagAttr1 = d.FlagAttr1 == null ? d.FlagAttr1 : d.FlagAttr1,
                                          IsRange = c.IsRange == null ? c.IsRange : c.IsRange,
                                          IsOpen = c.IsOpen == null ? c.IsOpen : c.IsOpen
                                      }).ToList();

            List<ViewBenefitRate> vListBenefitRate = new List<ViewBenefitRate>();
            foreach (var item in cariBenefitRateHPS)
            {
                // save ke submitHarga Rekanan Asuransi

                // - cek sudah ada d table belum
                var cek = ctx.HargaRekananAsuransis.Where(d => d.VendorId == vendor_id && d.BenefitCodeId == item.Id).FirstOrDefault();

                if (cek == null)
                {
                    HargaRekananAsuransi hga = new HargaRekananAsuransi();
                    hga.Id = Guid.NewGuid();
                    hga.BenefitCodeId = item.Id;
                    hga.VendorId = vendor_id;

                    ctx.HargaRekananAsuransis.Add(hga);
                    ctx.SaveChanges();
                }

                // masukin ke view
                JimbisEncrypt code = new JimbisEncrypt();
                var Rate = ctx.HargaRekananAsuransis.Where(d => d.BenefitCodeId == item.Id && d.VendorId == vendor_id).FirstOrDefault();

                ViewBenefitRate nViewListBenefitRate = new ViewBenefitRate();
                nViewListBenefitRate.Id = item.Id;
                nViewListBenefitRate.HargaId = Rate.Id;
                nViewListBenefitRate.BenefitCode = item.BenefitCode;
                nViewListBenefitRate.BenefitCoverage = item.BenefitCoverage;
                nViewListBenefitRate.RegionCode = item.RegionCode;
                nViewListBenefitRate.Rate = Rate.RateEncrypt != null ? Decimal.Parse(code.Decrypt(Rate.RateEncrypt)) : 0;
                nViewListBenefitRate.RateLowerLimit = Rate.RateLowerLimitEncrypt != null ? Decimal.Parse(code.Decrypt(Rate.RateLowerLimitEncrypt)) : 0;
                nViewListBenefitRate.RateUpperLimit = Rate.RateUpperLimitEncrypt != null ? Decimal.Parse(code.Decrypt(Rate.RateUpperLimitEncrypt)) : 0;
                nViewListBenefitRate.FlagAttr1 = item.FlagAttr1;
                nViewListBenefitRate.IsRange = item.IsRange;
                nViewListBenefitRate.IsOpen = item.IsOpen;
                vListBenefitRate.Add(nViewListBenefitRate);
            }
            tp.data = vListBenefitRate.Select(
                aa => new ViewBenefitRate
                {
                    Id = aa.Id,
                    HargaId = aa.HargaId,
                    BenefitCode = aa.BenefitCode,
                    BenefitCoverage = aa.BenefitCoverage,
                    RegionCode = aa.RegionCode,
                    Rate = aa.Rate,
                    RateLowerLimit = aa.RateLowerLimit,
                    RateUpperLimit = aa.RateUpperLimit,
                    FlagAttr1 = aa.FlagAttr1,
                    IsRange = aa.IsRange,
                    IsOpen = aa.IsOpen
                }).OrderByDescending(c => c.FlagAttr1).ThenBy(d => d.BenefitCode).ToList();

            return tp;
        }


        public DataTableBenefit GetDataHargaAsuransiKlarifikasi(Guid PengadaanId, Guid userId)
        {
            DataTableBenefit tp = new DataTableBenefit();

            var vendor_id = ctx.Vendors.Where(d => d.Owner == userId).FirstOrDefault().Id;
            var cariBenefitRateHPS = (from a in ctx.InsuranceTarifs
                                      join b in ctx.InsuranceTarifBenefits on a.DocumentId equals b.DocumentId
                                      join c in ctx.BenefitRates on b.BenefitRateId.Id equals c.Id
                                      join d in ctx.ReferenceDatas on c.BenefitCode equals d.Code
                                      join e in ctx.ReferenceDatas on c.BenefitCoverage equals e.Code
                                      join f in ctx.ReferenceDatas on c.RegionCode equals f.Code
                                      where a.PengadaanId == PengadaanId
                                      select new ViewBenefitRate
                                      {
                                          Id = c.Id,
                                          BenefitCode = d.LocalizedName,
                                          BenefitCoverage = e.LocalizedName,
                                          RegionCode = f.LocalizedName,
                                          Rate = c.Rate == null ? null : c.Rate,
                                          RateLowerLimit = c.RateLowerLimit == null ? c.RateLowerLimit : c.RateLowerLimit,
                                          RateUpperLimit = c.RateUpperLimit == null ? c.RateUpperLimit : c.RateUpperLimit,
                                          FlagAttr1 = d.FlagAttr1 == null ? d.FlagAttr1 : d.FlagAttr1,
                                          IsRange = c.IsRange == null ? c.IsRange : c.IsRange,
                                          IsOpen = c.IsOpen == null ? c.IsOpen : c.IsOpen
                                      }).ToList();

            List<ViewBenefitRate> vListBenefitRate = new List<ViewBenefitRate>();
            foreach (var item in cariBenefitRateHPS)
            {
                // save ke submitHarga Rekanan Asuransi

                // - cek sudah ada d table belum
                var cek = ctx.HargaKlarifikasiRekananAsuransis.Where(d => d.VendorId == vendor_id && d.BenefitCodeId == item.Id).FirstOrDefault();

                if (cek == null)
                {
                    HargaKlarifikasiRekananAsuransi hga = new HargaKlarifikasiRekananAsuransi();
                    hga.Id = Guid.NewGuid();
                    hga.BenefitCodeId = item.Id;
                    hga.VendorId = vendor_id;

                    ctx.HargaKlarifikasiRekananAsuransis.Add(hga);
                    ctx.SaveChanges();
                }

                // masukin ke view
                JimbisEncrypt code = new JimbisEncrypt();
                var Rate = ctx.HargaKlarifikasiRekananAsuransis.Where(d => d.BenefitCodeId == item.Id && d.VendorId == vendor_id).FirstOrDefault();

                ViewBenefitRate nViewListBenefitRate = new ViewBenefitRate();
                nViewListBenefitRate.Id = item.Id;
                nViewListBenefitRate.HargaId = Rate.Id;
                nViewListBenefitRate.BenefitCode = item.BenefitCode;
                nViewListBenefitRate.BenefitCoverage = item.BenefitCoverage;
                nViewListBenefitRate.RegionCode = item.RegionCode;
                nViewListBenefitRate.Rate = Rate.Rate;
                nViewListBenefitRate.RateLowerLimit = Rate.RateLowerLimit;
                nViewListBenefitRate.RateUpperLimit = Rate.RateUpperLimit;
                nViewListBenefitRate.FlagAttr1 = item.FlagAttr1;
                nViewListBenefitRate.IsRange = item.IsRange;
                nViewListBenefitRate.IsOpen = item.IsOpen;
                vListBenefitRate.Add(nViewListBenefitRate);
            }
            tp.data = vListBenefitRate.Select(
                aa => new ViewBenefitRate
                {
                    Id = aa.Id,
                    HargaId = aa.HargaId,
                    BenefitCode = aa.BenefitCode,
                    BenefitCoverage = aa.BenefitCoverage,
                    RegionCode = aa.RegionCode,
                    Rate = aa.Rate,
                    RateLowerLimit = aa.RateLowerLimit,
                    RateUpperLimit = aa.RateUpperLimit,
                    FlagAttr1 = aa.FlagAttr1,
                    IsRange = aa.IsRange,
                    IsOpen = aa.IsOpen
                }).OrderByDescending(c => c.FlagAttr1).ThenBy(d => d.BenefitCode).ToList();

            return tp;
        }

        public DataTableBenefit GetDataHargaAsuransiKlarifikasiLanjutan(Guid PengadaanId, Guid userId)
        {
            DataTableBenefit tp = new DataTableBenefit();

            var vendor_id = ctx.Vendors.Where(d => d.Owner == userId).FirstOrDefault().Id;
            var cariBenefitRateHPS = (from a in ctx.InsuranceTarifs
                                      join b in ctx.InsuranceTarifBenefits on a.DocumentId equals b.DocumentId
                                      join c in ctx.BenefitRates on b.BenefitRateId.Id equals c.Id
                                      join d in ctx.ReferenceDatas on c.BenefitCode equals d.Code
                                      join e in ctx.ReferenceDatas on c.BenefitCoverage equals e.Code
                                      join f in ctx.ReferenceDatas on c.RegionCode equals f.Code
                                      where a.PengadaanId == PengadaanId
                                      select new ViewBenefitRate
                                      {
                                          Id = c.Id,
                                          BenefitCode = d.LocalizedName,
                                          BenefitCoverage = e.LocalizedName,
                                          RegionCode = f.LocalizedName,
                                          Rate = c.Rate == null ? null : c.Rate,
                                          RateLowerLimit = c.RateLowerLimit == null ? c.RateLowerLimit : c.RateLowerLimit,
                                          RateUpperLimit = c.RateUpperLimit == null ? c.RateUpperLimit : c.RateUpperLimit,
                                          FlagAttr1 = d.FlagAttr1 == null ? d.FlagAttr1 : d.FlagAttr1,
                                          IsRange = c.IsRange == null ? c.IsRange : c.IsRange,
                                          IsOpen = c.IsOpen == null ? c.IsOpen : c.IsOpen
                                      }).ToList();

            List<ViewBenefitRate> vListBenefitRate = new List<ViewBenefitRate>();
            foreach (var item in cariBenefitRateHPS)
            {
                // save ke submitHarga Rekanan Asuransi

                // - cek sudah ada d table belum
                var cek = ctx.HargaKlarifikasiLanjutanAsuransis.Where(d => d.VendorId == vendor_id && d.BenefitCodeId == item.Id).FirstOrDefault();

                if (cek == null)
                {
                    HargaKlarifikasiLanjutanAsuransi hga = new HargaKlarifikasiLanjutanAsuransi();
                    hga.Id = Guid.NewGuid();
                    hga.BenefitCodeId = item.Id;
                    hga.VendorId = vendor_id;

                    ctx.HargaKlarifikasiLanjutanAsuransis.Add(hga);
                    ctx.SaveChanges();
                }

                // masukin ke view
                JimbisEncrypt code = new JimbisEncrypt();
                var Rate = ctx.HargaKlarifikasiLanjutanAsuransis.Where(d => d.BenefitCodeId == item.Id && d.VendorId == vendor_id).FirstOrDefault();

                ViewBenefitRate nViewListBenefitRate = new ViewBenefitRate();
                nViewListBenefitRate.Id = item.Id;
                nViewListBenefitRate.HargaId = Rate.Id;
                nViewListBenefitRate.BenefitCode = item.BenefitCode;
                nViewListBenefitRate.BenefitCoverage = item.BenefitCoverage;
                nViewListBenefitRate.RegionCode = item.RegionCode;
                nViewListBenefitRate.Rate = Rate.Rate;
                nViewListBenefitRate.RateLowerLimit = Rate.RateLowerLimit;
                nViewListBenefitRate.RateUpperLimit = Rate.RateUpperLimit;
                nViewListBenefitRate.FlagAttr1 = item.FlagAttr1;
                nViewListBenefitRate.IsRange = item.IsRange;
                nViewListBenefitRate.IsOpen = item.IsOpen;
                vListBenefitRate.Add(nViewListBenefitRate);
            }
            tp.data = vListBenefitRate.Select(
                aa => new ViewBenefitRate
                {
                    Id = aa.Id,
                    HargaId = aa.HargaId,
                    BenefitCode = aa.BenefitCode,
                    BenefitCoverage = aa.BenefitCoverage,
                    RegionCode = aa.RegionCode,
                    Rate = aa.Rate,
                    RateLowerLimit = aa.RateLowerLimit,
                    RateUpperLimit = aa.RateUpperLimit,
                    FlagAttr1 = aa.FlagAttr1,
                    IsRange = aa.IsRange,
                    IsOpen = aa.IsOpen
                }).OrderByDescending(c => c.FlagAttr1).ThenBy(d => d.BenefitCode).ToList();

            return tp;
        }

        public ViewBenefitRate GetDetailBenef(int Id, Guid userId)
        {
            ViewBenefitRate tp = new ViewBenefitRate();

            var vendor_id = ctx.Vendors.Where(d => d.Owner == userId).FirstOrDefault().Id;
            var cariBenefitRateHPS = (from a in ctx.InsuranceTarifs
                                      join b in ctx.InsuranceTarifBenefits on a.DocumentId equals b.DocumentId
                                      join c in ctx.BenefitRates on b.BenefitRateId.Id equals c.Id
                                      join d in ctx.ReferenceDatas on c.BenefitCode equals d.Code
                                      join e in ctx.ReferenceDatas on c.BenefitCoverage equals e.Code
                                      join f in ctx.ReferenceDatas on c.RegionCode equals f.Code
                                      join g in ctx.HargaRekananAsuransis on c.Id equals g.BenefitCodeId
                                      where c.Id == Id && g.VendorId == vendor_id
                                      select new ViewBenefitRate
                                      {
                                          Id = c.Id,
                                          BenefitCode = d.LocalizedName,
                                          BenefitCoverage = e.LocalizedName,
                                          RegionCode = f.LocalizedName,
                                          Rate = g.Rate == null ? null : g.Rate,
                                          RateLowerLimit = g.RateLowerLimit == null ? null : g.RateLowerLimit,
                                          RateUpperLimit = g.RateUpperLimit == null ? null : g.RateUpperLimit,
                                          FlagAttr1 = d.FlagAttr1 == null ? d.FlagAttr1 : d.FlagAttr1,
                                          IsRange = c.IsRange == null ? c.IsRange : c.IsRange,
                                          IsOpen = c.IsOpen == null ? c.IsOpen : c.IsOpen,
                                          HargaId = g.Id
                                      }).FirstOrDefault();

            return cariBenefitRateHPS;
        }

        public ViewBenefitRate GetDetailBenefKlarifikasi(int Id, Guid userId)
        {
            ViewBenefitRate tp = new ViewBenefitRate();

            var vendor_id = ctx.Vendors.Where(d => d.Owner == userId).FirstOrDefault().Id;
            var cariBenefitRateHPS = (from a in ctx.InsuranceTarifs
                                      join b in ctx.InsuranceTarifBenefits on a.DocumentId equals b.DocumentId
                                      join c in ctx.BenefitRates on b.BenefitRateId.Id equals c.Id
                                      join d in ctx.ReferenceDatas on c.BenefitCode equals d.Code
                                      join e in ctx.ReferenceDatas on c.BenefitCoverage equals e.Code
                                      join f in ctx.ReferenceDatas on c.RegionCode equals f.Code
                                      join g in ctx.HargaKlarifikasiRekananAsuransis on c.Id equals g.BenefitCodeId
                                      where c.Id == Id && g.VendorId == vendor_id
                                      select new ViewBenefitRate
                                      {
                                          Id = c.Id,
                                          BenefitCode = d.LocalizedName,
                                          BenefitCoverage = e.LocalizedName,
                                          RegionCode = f.LocalizedName,
                                          Rate = g.Rate == null ? null : g.Rate,
                                          RateLowerLimit = g.RateLowerLimit == null ? null : g.RateLowerLimit,
                                          RateUpperLimit = g.RateUpperLimit == null ? null : g.RateUpperLimit,
                                          FlagAttr1 = d.FlagAttr1 == null ? d.FlagAttr1 : d.FlagAttr1,
                                          IsRange = c.IsRange == null ? c.IsRange : c.IsRange,
                                          IsOpen = c.IsOpen == null ? c.IsOpen : c.IsOpen,
                                          HargaId = g.Id
                                      }).FirstOrDefault();

            return cariBenefitRateHPS;
        }


        public ViewBenefitRate GetDetailBenefKlarifikasiLanjutan(int Id, Guid userId)
        {
            ViewBenefitRate tp = new ViewBenefitRate();

            var vendor_id = ctx.Vendors.Where(d => d.Owner == userId).FirstOrDefault().Id;
            var cariBenefitRateHPS = (from a in ctx.InsuranceTarifs
                                      join b in ctx.InsuranceTarifBenefits on a.DocumentId equals b.DocumentId
                                      join c in ctx.BenefitRates on b.BenefitRateId.Id equals c.Id
                                      join d in ctx.ReferenceDatas on c.BenefitCode equals d.Code
                                      join e in ctx.ReferenceDatas on c.BenefitCoverage equals e.Code
                                      join f in ctx.ReferenceDatas on c.RegionCode equals f.Code
                                      join g in ctx.HargaKlarifikasiLanjutanAsuransis on c.Id equals g.BenefitCodeId
                                      where c.Id == Id && g.VendorId == vendor_id
                                      select new ViewBenefitRate
                                      {
                                          Id = c.Id,
                                          BenefitCode = d.LocalizedName,
                                          BenefitCoverage = e.LocalizedName,
                                          RegionCode = f.LocalizedName,
                                          Rate = g.Rate == null ? null : g.Rate,
                                          RateLowerLimit = g.RateLowerLimit == null ? null : g.RateLowerLimit,
                                          RateUpperLimit = g.RateUpperLimit == null ? null : g.RateUpperLimit,
                                          FlagAttr1 = d.FlagAttr1 == null ? d.FlagAttr1 : d.FlagAttr1,
                                          IsRange = c.IsRange == null ? c.IsRange : c.IsRange,
                                          IsOpen = c.IsOpen == null ? c.IsOpen : c.IsOpen,
                                          HargaId = g.Id
                                      }).FirstOrDefault();

            return cariBenefitRateHPS;
        }

        public List<ViewVendorBenefRate> getKandidatKirimAsuransi(Guid PengadaanId, Guid UserId)
        {
            try
            {
                //var xKandidatPengadaans = (from b in ctx.KandidatPengadaans where b.PengadaanId == PengadaanId select b).ToList();

                //var kirim = (from a in ctx.InsuranceTarifs
                //             join b in ctx.InsuranceTarifBenefits on a.DocumentId equals b.DocumentId
                //             join c in ctx.BenefitRates on b.BenefitRateId.Id equals c.Id
                //             join d in ctx.HargaRekananAsuransis on c.Id equals d.BenefitCodeId
                //             where a.PengadaanId == PengadaanId
                //             select new ViewVendorBenefRate
                //             {
                //                 NamaVendor = d.Vendor.Nama,
                //                 VendorId = d.VendorId
                //             }).Distinct().ToList();
                ////var kirim2 = ctx.HargaRekanans.Where(d => d.RKSDetail.RKSHeader.PengadaanId == PengadaanId).Select(d => d.VendorId).Distinct().ToList();
                ////var kandidat = ctx.KandidatPengadaans.Where(d => d.PengadaanId == PengadaanId && kirim.Contains(d.VendorId)).ToList();
                var KandidatPengadaan = ctx.KandidatPengadaans.Where(d => d.PengadaanId == PengadaanId).Select(d => d.VendorId).ToList();

                var kirim = (from a in ctx.InsuranceTarifs
                             join b in ctx.InsuranceTarifBenefits on a.DocumentId equals b.DocumentId
                             join c in ctx.BenefitRates on b.BenefitRateId.Id equals c.Id
                             join d in ctx.HargaRekananAsuransis on c.Id equals d.BenefitCodeId
                             where a.PengadaanId == PengadaanId && KandidatPengadaan.Contains(d.VendorId)
                             select new ViewVendorBenefRate
                             {
                                 NamaVendor = d.Vendor.Nama
                             }).Distinct().ToList();

                //var send = ctx.HargaRekananAsuransis.Where(d => KandidatPengadaan.Contains(d.VendorId)).Distinct().ToList();

                return kirim;
            }
            catch { return new List<ViewVendorBenefRate>(); }
        }

        public List<ViewVendorBenefRate> getKandidatTidakKirimAsuransi(Guid PengadaanId, Guid UserId)
        {
            try
            {
                var KandidatPengadaan = ctx.KandidatPengadaans.Where(d => d.PengadaanId == PengadaanId).Select(d => d.VendorId).ToList();

                var kirim = (from a in ctx.InsuranceTarifs
                             join b in ctx.InsuranceTarifBenefits on a.DocumentId equals b.DocumentId
                             join c in ctx.BenefitRates on b.BenefitRateId.Id equals c.Id
                             join d in ctx.HargaRekananAsuransis on c.Id equals d.BenefitCodeId
                             where a.PengadaanId == PengadaanId && !KandidatPengadaan.Contains(d.VendorId)
                             select new ViewVendorBenefRate
                             {
                                 NamaVendor = d.Vendor.Nama
                             }).Distinct().ToList();

                return kirim;

                //var kirim = (from a in ctx.InsuranceTarifs
                //             join b in ctx.InsuranceTarifBenefits on a.DocumentId equals b.DocumentId
                //             join c in ctx.BenefitRates on b.BenefitRateId.Id equals c.Id
                //             join d in ctx.HargaRekananAsuransis on c.Id equals d.BenefitCodeId
                //             where a.PengadaanId == PengadaanId
                //             select new ViewVendorBenefRate
                //             {
                //                 NamaVendor = d.Vendor.Nama,
                //                 VendorId = d.VendorId
                //             }).Distinct().ToList();
                //var KandidatPengadaan = ctx.KandidatPengadaans.Where(d => d.PengadaanId == PengadaanId && !kirim.Contains(d.VendorId)).ToList();

                //return KandidatPengadaan;
            }
            catch { return new List<ViewVendorBenefRate>(); }
        }

        public List<VWReportPengadaan> GetRepotPengadan2(DateTime? dari, DateTime? sampai, Guid UserId)
        {
            var oReport = (from b in ctx.Pengadaans
                           join c in ctx.PersonilPengadaans on b.Id equals c.PengadaanId
                           where b.TanggalMenyetujui >= dari && b.TanggalMenyetujui <= sampai && c.tipe == "pic"//c.tanggal >= dari && c.tanggal <= sampai// && c.Tipe == TipeBerkas.BeritaAcaraPenentuanPemenang
                           select new VWReportPengadaan
                           {
                               PengadaanId = b.Id,
                               PIC = c.Nama,
                               Judul = b.Judul,
                               User = b.UnitKerjaPemohon,
                               hps = (from bb in ctx.RKSHeaders
                                      join cc in ctx.RKSDetails on bb.Id equals cc.RKSHeaderId
                                      where bb.PengadaanId == b.Id
                                      select cc).Sum(xx => xx.hps) == null ? 0 :
                                    (from bb in ctx.RKSHeaders
                                     join cc in ctx.RKSDetails on bb.Id equals cc.RKSHeaderId
                                     where bb.PengadaanId == b.Id
                                     select cc).Sum(xx => xx.hps * xx.jumlah).Value
                           }).Distinct().ToList();
            foreach (var item in oReport)
            {
                item.realitas = getPemenangPengadaan(item.PengadaanId.Value, UserId).FirstOrDefault() == null ? 0 :
                    getPemenangPengadaan(item.PengadaanId.Value, UserId).FirstOrDefault().total;
                item.efisiensi = efisiensi(item.PengadaanId.Value, UserId);
                item.Pemenang = getPemenangPengadaan(item.PengadaanId.Value, UserId).FirstOrDefault() == null ? "" :
                            getPemenangPengadaan(item.PengadaanId.Value, UserId).FirstOrDefault().NamaVendor;
                item.Aanwjzing = getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.BeritaAcaraAanwijzing, UserId) == null ? null :
                        getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.BeritaAcaraAanwijzing, UserId).tanggal;
                item.PembukaanAmplop = getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.BeritaAcaraBukaAmplop, UserId) == null ? null :
                        getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.BeritaAcaraBukaAmplop, UserId).tanggal;
                item.Klasrifikasi = getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.BeritaAcaraKlarifikasi, UserId) == null ? null :
                    getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.BeritaAcaraKlarifikasi, UserId).tanggal;
                item.Scoring = getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.BeritaAcaraPenilaian, UserId) == null ? null :
                    getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.BeritaAcaraPenilaian, UserId).tanggal;
                item.NotaPemenang = getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.BeritaAcaraPenentuanPemenang, UserId) == null ? null :
                    getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.BeritaAcaraPenentuanPemenang, UserId).tanggal;
                item.SPK = getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.SuratPerintahKerja, UserId) == null ? null :
                    getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.SuratPerintahKerja, UserId).tanggal;
                item.KlasrifikasiLanjut = getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.BeritaAcaraKlarifikasiLanjutan, UserId) == null ? null :
                    getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.BeritaAcaraKlarifikasiLanjutan, UserId).tanggal;

            }
            return oReport;
        }

        public List<VWReportPengadaan> GetRepotPengadan3(DateTime? dari, DateTime? sampai, Guid UserId, EStatusPengadaan status)
        {
            var oReport = (from b in ctx.Pengadaans
                           join c in ctx.PersonilPengadaans on b.Id equals c.PengadaanId
                           where b.TanggalMenyetujui >= dari && b.TanggalMenyetujui <= sampai && c.tipe == "pic" && b.Status <= status//c.tanggal >= dari && c.tanggal <= sampai// && c.Tipe == TipeBerkas.BeritaAcaraPenentuanPemenang
                           select new VWReportPengadaan
                           {
                               PengadaanId = b.Id,
                               PIC = c.Nama,
                               Judul = b.Judul,
                               User = b.UnitKerjaPemohon,
                               hps = (from bb in ctx.RKSHeaders
                                      join cc in ctx.RKSDetails on bb.Id equals cc.RKSHeaderId
                                      where bb.PengadaanId == b.Id
                                      select cc).Sum(xx => xx.hps) == null ? 0 :
                                    (from bb in ctx.RKSHeaders
                                     join cc in ctx.RKSDetails on bb.Id equals cc.RKSHeaderId
                                     where bb.PengadaanId == b.Id
                                     select cc).Sum(xx => xx.hps * xx.jumlah).Value
                           }).Distinct().ToList();
            foreach (var item in oReport)
            {
                item.realitas = getPemenangPengadaan(item.PengadaanId.Value, UserId).FirstOrDefault() == null ? 0 :
                    getPemenangPengadaan(item.PengadaanId.Value, UserId).FirstOrDefault().total;
                item.efisiensi = efisiensi(item.PengadaanId.Value, UserId);
                item.Pemenang = getPemenangPengadaan(item.PengadaanId.Value, UserId).FirstOrDefault() == null ? "" :
                            getPemenangPengadaan(item.PengadaanId.Value, UserId).FirstOrDefault().NamaVendor;
                item.Aanwjzing = getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.BeritaAcaraAanwijzing, UserId) == null ? null :
                        getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.BeritaAcaraAanwijzing, UserId).tanggal;
                item.PembukaanAmplop = getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.BeritaAcaraBukaAmplop, UserId) == null ? null :
                        getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.BeritaAcaraBukaAmplop, UserId).tanggal;
                item.Klasrifikasi = getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.BeritaAcaraKlarifikasi, UserId) == null ? null :
                    getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.BeritaAcaraKlarifikasi, UserId).tanggal;
                item.Scoring = getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.BeritaAcaraPenilaian, UserId) == null ? null :
                    getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.BeritaAcaraPenilaian, UserId).tanggal;
                item.NotaPemenang = getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.BeritaAcaraPenentuanPemenang, UserId) == null ? null :
                    getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.BeritaAcaraPenentuanPemenang, UserId).tanggal;
                item.SPK = getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.SuratPerintahKerja, UserId) == null ? null :
                    getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.SuratPerintahKerja, UserId).tanggal;
                item.KlasrifikasiLanjut = getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.BeritaAcaraKlarifikasiLanjutan, UserId) == null ? null :
                    getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.BeritaAcaraKlarifikasiLanjutan, UserId).tanggal;

            }
            return oReport;
        }

        public List<VWReportPengadaan> GetRepotPengadan4(DateTime? dari, DateTime? sampai, Guid UserId, EStatusPengadaan status, string divisi)
        {
            var oReport = (from b in ctx.Pengadaans
                           join c in ctx.PersonilPengadaans on b.Id equals c.PengadaanId
                           where b.TanggalMenyetujui >= dari && b.TanggalMenyetujui <= sampai && c.tipe == "pic" && b.Status <= status && b.UnitKerjaPemohon == divisi//c.tanggal >= dari && c.tanggal <= sampai// && c.Tipe == TipeBerkas.BeritaAcaraPenentuanPemenang
                           select new VWReportPengadaan
                           {
                               PengadaanId = b.Id,
                               PIC = c.Nama,
                               Judul = b.Judul,
                               User = b.UnitKerjaPemohon,
                               hps = (from bb in ctx.RKSHeaders
                                      join cc in ctx.RKSDetails on bb.Id equals cc.RKSHeaderId
                                      where bb.PengadaanId == b.Id
                                      select cc).Sum(xx => xx.hps) == null ? 0 :
                                    (from bb in ctx.RKSHeaders
                                     join cc in ctx.RKSDetails on bb.Id equals cc.RKSHeaderId
                                     where bb.PengadaanId == b.Id
                                     select cc).Sum(xx => xx.hps * xx.jumlah).Value
                           }).Distinct().ToList();
            foreach (var item in oReport)
            {
                item.realitas = getPemenangPengadaan(item.PengadaanId.Value, UserId).FirstOrDefault() == null ? 0 :
                    getPemenangPengadaan(item.PengadaanId.Value, UserId).FirstOrDefault().total;
                item.efisiensi = efisiensi(item.PengadaanId.Value, UserId);
                item.Pemenang = getPemenangPengadaan(item.PengadaanId.Value, UserId).FirstOrDefault() == null ? "" :
                            getPemenangPengadaan(item.PengadaanId.Value, UserId).FirstOrDefault().NamaVendor;
                item.Aanwjzing = getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.BeritaAcaraAanwijzing, UserId) == null ? null :
                        getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.BeritaAcaraAanwijzing, UserId).tanggal;
                item.PembukaanAmplop = getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.BeritaAcaraBukaAmplop, UserId) == null ? null :
                        getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.BeritaAcaraBukaAmplop, UserId).tanggal;
                item.Klasrifikasi = getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.BeritaAcaraKlarifikasi, UserId) == null ? null :
                    getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.BeritaAcaraKlarifikasi, UserId).tanggal;
                item.Scoring = getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.BeritaAcaraPenilaian, UserId) == null ? null :
                    getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.BeritaAcaraPenilaian, UserId).tanggal;
                item.NotaPemenang = getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.BeritaAcaraPenentuanPemenang, UserId) == null ? null :
                    getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.BeritaAcaraPenentuanPemenang, UserId).tanggal;
                item.SPK = getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.SuratPerintahKerja, UserId) == null ? null :
                    getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.SuratPerintahKerja, UserId).tanggal;
                item.KlasrifikasiLanjut = getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.BeritaAcaraKlarifikasiLanjutan, UserId) == null ? null :
                    getBeritaAcaraByTipe(item.PengadaanId.Value, TipeBerkas.BeritaAcaraKlarifikasiLanjutan, UserId).tanggal;

            }
            return oReport;
        }

        public int backToState(Guid PengadaanId, Guid UserId, EStatusPengadaan state, DateTime? from, DateTime? to)
        {
            Pengadaan Mpengadaaan = ctx.Pengadaans.Find(PengadaanId);
            //PersonilPengadaan picPersonil = ctx.PersonilPengadaans.Where(d => d.PersonilId == UserId && d.tipe == "pic" && d.PengadaanId == PengadaanId).FirstOrDefault();
            //if (picPersonil == null) return 0;

            Mpengadaaan.Status = state;
            #region kalo anwajzing liat kehadirannya
            if (state == EStatusPengadaan.SUBMITPENAWARAN)
            {
                var kandidatHadir = ctx.KehadiranKandidatAanwijzings.Where(d => d.PengadaanId == PengadaanId).Select(d => d.VendorId).ToList();
                var kandidatTidakHadir = ctx.KandidatPengadaans.Where(d => !kandidatHadir.Contains(d.VendorId) && d.PengadaanId == PengadaanId).ToList();


                foreach (var item in kandidatTidakHadir)
                {
                    var HistoryKandidat = new HistoryKandidatPengadaan();
                    HistoryKandidat.PengadaanId = item.PengadaanId;
                    HistoryKandidat.VendorId = item.VendorId;
                    HistoryKandidat.addKandidatType = item.addKandidatType;
                    ctx.HistoryKandidatPengadaan.Add(HistoryKandidat);

                }
                ctx.KandidatPengadaans.RemoveRange(kandidatTidakHadir);
                ctx.SaveChanges();

            }
            #endregion
            #region rubah jadwal pengadaal
            JadwalPelaksanaan dtJadwl = ctx.JadwalPelaksanaans.Where(d => d.PengadaanId == PengadaanId && d.statusPengadaan == state).FirstOrDefault();
            if (dtJadwl == null)
            {
                JadwalPelaksanaan newJadwal = new JadwalPelaksanaan();
                if (from != null)
                    newJadwal.Mulai = from;
                if (state != EStatusPengadaan.PEMENANG)
                    newJadwal.Sampai = to;
                newJadwal.statusPengadaan = state;
                newJadwal.PengadaanId = PengadaanId;
                if (from != null || to != null)
                    ctx.JadwalPelaksanaans.Add(newJadwal);
            }
            else
            {

                if (from != null && to != null)
                {
                    dtJadwl.Mulai = from;
                    if (state != EStatusPengadaan.PEMENANG)
                        dtJadwl.Sampai = to;
                }
            }
            try
            {
                RiwayatDokumen newRiwayatPengadaan = new RiwayatDokumen();
                newRiwayatPengadaan.PengadaanId = PengadaanId;
                newRiwayatPengadaan.Status = state.ToString();
                newRiwayatPengadaan.ActionDate = DateTime.Now;
                ctx.RiwayatDokumens.Add(newRiwayatPengadaan);
                RiwayatPengadaan newRiwayatPengadaan2 = new RiwayatPengadaan();
                newRiwayatPengadaan2.PengadaanId = PengadaanId;
                newRiwayatPengadaan2.Status = state;
                newRiwayatPengadaan2.Waktu = DateTime.Now;
                ctx.RiwayatPengadaans.Add(newRiwayatPengadaan2);
                ctx.SaveChanges();

                if (state == EStatusPengadaan.PEMENANG)
                {
                    var dt = ctx.PelaksanaanPemilihanKandidats.Where(d => d.PengadaanId == PengadaanId).ToList();
                    var pemenang = ctx.PemenangPengadaans.Where(d => d.PengadaanId == PengadaanId).ToList();
                    ctx.PemenangPengadaans.RemoveRange(pemenang);
                    ctx.SaveChanges();
                    foreach (var item in dt)
                    {
                        var ndata = new PemenangPengadaan()
                        {
                            PengadaanId = PengadaanId,
                            VendorId = item.VendorId,
                            CreateOn = DateTime.Now,
                            CreatedBy = UserId
                        };
                        ctx.PemenangPengadaans.Add(ndata);
                        ctx.SaveChanges();
                    }
                }

                return 1;
            }
            catch
            {
                return 0;
            }
            #endregion



        }
        public PersonilPengadaan deletePersonilPengadaanMasterUser(Guid Id, Guid UserId)
        {
            try
            {
                PersonilPengadaan MPersonilPengadaan = ctx.PersonilPengadaans.Find(Id);
                Pengadaan mpengadaan = ctx.Pengadaans.Find(MPersonilPengadaan.PengadaanId);
                if (mpengadaan == null) return null;
                //if (mpengadaan.Status != EStatusPengadaan.DRAFT) return 0;
                ctx.PersonilPengadaans.Remove(MPersonilPengadaan);
                ctx.SaveChanges();
                return MPersonilPengadaan;
            }
            catch { return null; }
        }

        public PersonilPengadaan savePersonilPengadaanMasterUser(PersonilPengadaan Personil, Guid UserId)
        {
            try
            {
                Pengadaan mpengadaan = ctx.Pengadaans.Find(Personil.PengadaanId);
                if (mpengadaan == null) return Personil;
                //if (mpengadaan.Status != EStatusPengadaan.DRAFT) return Personil;
                PersonilPengadaan mPersonilPengadaan = ctx.PersonilPengadaans.Find(Personil.Id);
                if (mPersonilPengadaan != null)
                {
                    mPersonilPengadaan.Nama = Personil.Nama;
                    mPersonilPengadaan.PersonilId = Personil.PersonilId;
                    mPersonilPengadaan.Jabatan = Personil.Jabatan;
                    mPersonilPengadaan.tipe = Personil.tipe;
                    mpengadaan.ModifiedBy = UserId;
                    mpengadaan.ModifiedOn = DateTime.Now;
                    ctx.SaveChanges();
                    return mPersonilPengadaan;
                }
                else
                {
                    mpengadaan.ModifiedBy = UserId;
                    mpengadaan.ModifiedOn = DateTime.Now;
                    ctx.PersonilPengadaans.Add(Personil);
                    ctx.SaveChanges();
                    return Personil;
                }
            }
            catch
            {
                return Personil;
            }
        }
        public PersonilPengadaan HistoryBackstep(Guid Id, Guid UserId)
        {
            try
            {
                PersonilPengadaan MPersonilPengadaan = ctx.PersonilPengadaans.Find(Id);
                Pengadaan mpengadaan = ctx.Pengadaans.Find(MPersonilPengadaan.PengadaanId);
                return MPersonilPengadaan;
            }
            catch { return null; }
        }
        //public PersonilPengadaan HistoryBackstep(Guid Id, Guid UserId)
        //{
        //    try
        //    {
        //        PersonilPengadaan MPersonilPengadaan = ctx.PersonilPengadaans.Find(Id);
        //        Pengadaan mpengadaan = ctx.Pengadaans.Find(MPersonilPengadaan.PengadaanId);
        //        return MPersonilPengadaan;
        //    }
        //    catch { return null; }
        //}

        public List<VWPks> ListPerpanjanganPKS(Guid UserId)
        {
            var data = (from a in ctx.Pks
                        select new VWPks
                        {
                            TanggalSelesai = a.TanggalSelesai,
                            CreateBy = a.CreateBy
                        }).Distinct().ToList();
            return data;
        }

        public PersetujuanTahapan ClearPersetujuanTahapan(PersetujuanTahapan data, Guid PengadaanId)
        {

            var dataPengadaan = ctx.Pengadaans.Find(data.PengadaanId);
            if (dataPengadaan == null) return new PersetujuanTahapan();
            if (dataPengadaan.AturanPengadaan == "Pengadaan Tertutup" && dataPengadaan.Status == EStatusPengadaan.DISETUJUI)
            {
                dataPengadaan.Status = EStatusPengadaan.AANWIJZING;
            }

            List<PersetujuanTahapan> dataTahapan = dataPengadaan.PersetujuanTahapans.Where(d => d.StatusPengadaan == data.StatusPengadaan && d.PengadaanId == data.PengadaanId).ToList();

            foreach (var item in dataTahapan)
            {
                ctx.PersetujuanTahapans.Remove(item);
                ctx.SaveChanges(PengadaanId.ToString());
            }

            return new PersetujuanTahapan();
        }
        public PersetujuanTahapan ClearPersetujuanTahapan2(PersetujuanTahapan data2, Guid PengadaanId)
        {

            var dataPengadaan = ctx.Pengadaans.Find(data2.PengadaanId);
            if (dataPengadaan == null) return new PersetujuanTahapan();
            if (dataPengadaan.AturanPengadaan == "Pengadaan Tertutup" && dataPengadaan.Status == EStatusPengadaan.DISETUJUI)
            {
                dataPengadaan.Status = EStatusPengadaan.AANWIJZING;
            }

            List<PersetujuanTahapan> dataTahapan = dataPengadaan.PersetujuanTahapans.Where(d => d.StatusPengadaan == data2.StatusPengadaan && d.PengadaanId == data2.PengadaanId).ToList();

            foreach (var item in dataTahapan)
            {
                ctx.PersetujuanTahapans.Remove(item);
                ctx.SaveChanges(PengadaanId.ToString());
            }

            return new PersetujuanTahapan();
        }

        public PersetujuanTahapan nextToDelete(PersetujuanTahapan data2, Guid PengadaanId)
        {

            var dataPengadaan = ctx.Pengadaans.Find(data2.PengadaanId);
            if (dataPengadaan == null) return new PersetujuanTahapan();
            if (dataPengadaan.AturanPengadaan == "Pengadaan Tertutup" && dataPengadaan.Status == EStatusPengadaan.DISETUJUI)
            {
                dataPengadaan.Status = EStatusPengadaan.AANWIJZING;
            }

            List<PersetujuanTahapan> dataTahapan = dataPengadaan.PersetujuanTahapans.Where(d => d.StatusPengadaan == data2.StatusPengadaan && d.PengadaanId == data2.PengadaanId).ToList();

            foreach (var item in dataTahapan)
            {
                ctx.PersetujuanTahapans.Remove(item);
                ctx.SaveChanges(PengadaanId.ToString());
            }

            return new PersetujuanTahapan();
        }

        public int nextToDelete(Guid Id)
        {
            try
            {
                List<TenderScoringHeader> Head = ctx.TenderScoringHeaders.Where(x => x.PengadaanId == Id).ToList();
                List<TenderScoringBobot> Bobot = ctx.TenderScoringBobots.Where(x => x.PengadaanId == Id).ToList();

                foreach (var a in Head)
                {
                    List<TenderScoringDetail> Detail = ctx.TenderScoringDetails.Where(x => x.TenderScoringHeaderId == a.Id).ToList();
                    foreach (var b in Detail)
                    {
                        List<TenderScoringUser> User = ctx.TenderScoringUsers.Where(x => x.TenderScoringDetailId == b.Id).ToList();
                        foreach (var c in User)
                        {
                            ctx.TenderScoringUsers.Remove(c);
                            ctx.SaveChanges();
                        }
                        ctx.TenderScoringDetails.Remove(b);
                        ctx.SaveChanges();
                    }
                    ctx.TenderScoringHeaders.Remove(a);
                    ctx.SaveChanges();
                }

                foreach (var itemx in Bobot)
                {
                    ctx.TenderScoringBobots.Remove(itemx);
                    ctx.SaveChanges();
                }
                return 1;
            }
            catch { return 0; }

        }

        public BudgetingPengadaanHeader GetTotalCOA(Guid PengadaanId, Guid UserId)
        {
            BudgetingPengadaanHeader oCOA = ctx.BudgetingPengadaanHeaders.Where(d => d.PengadaanId == PengadaanId).FirstOrDefault();
            if (oCOA == null) return new BudgetingPengadaanHeader();
            return new BudgetingPengadaanHeader
            {
                Id = oCOA.Id,
                TotalInput = oCOA.TotalInput
            };
        }

        public VWBudgeting GetLoadCOA(Guid PengadaanId, Guid UserId)
        {
            var StatusUpload = (from a in ctx.BudgetingPengadaanHeaders
                                join b in ctx.BudgetingPengadaanDetails on a.Id equals b.BudgetingPengadaanId
                                where a.Id == PengadaanId
                                select new VWBudgeting
                                {
                                    COA = b.NoCOA,
                                    TotalInput = b.Input
                                }).Distinct().FirstOrDefault();
            if (StatusUpload == null) return new VWBudgeting();
            //return new BudgetingPengadaanHeader
            //{
            //    Id = oCOA.Id,
            //    TotalInput = oCOA.TotalInput
            //};
            return StatusUpload;
        }

        public List<VWBudgeting> GetUsingCOA(Guid PengadaanId)
        {
            var HeaderId = ctx.BudgetingPengadaanHeaders.Where(d => d.PengadaanId == PengadaanId).Select(d => d.Id).FirstOrDefault();

            BudgetingPengadaanDetail Detail = ctx.BudgetingPengadaanDetails.Where(x => x.BudgetingPengadaanId == HeaderId).FirstOrDefault();

            List<VWBudgeting> listVWBudgeting = new List<VWBudgeting>();

            int? ver = 0;

            if (Detail != null)
            {
                var oldVersion = (from a in ctx.Budgetings
                                  where a.Year.Equals(Detail.Year)
                                        && a.Jenis.Equals(Detail.BudgetType)
                                  select new VWBudgeting
                                  {
                                      Version = a.Version
                                  }).Distinct().OrderByDescending(x => x.Version).FirstOrDefault();
                if (oldVersion != null)
                {
                    ver = oldVersion.Version;
                }
            }

            if (Detail != null)
            {
                System.Linq.IQueryable<Budgeting> result4 = ctx.Budgetings.AsQueryable();
                var dataa = result4.GroupBy(x =>
                    new
                    {
                        x.Branch,
                        x.Department,
                        x.Description,
                        x.COA,
                        x.Year,
                        x.BudgetReserved,
                        x.Version,
                        x.Jenis
                    }).Select(g =>
                     new
                     {
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
                //start grouping Budget On Process
                var BP = (from a in ctx.BudgetingPengadaanDetails
                          join b in ctx.BudgetingPengadaanHeaders on a.BudgetingPengadaanId equals b.Id
                          join c in ctx.Pengadaans on b.PengadaanId equals c.Id
                          where a.Branch.Equals(Detail.Branch)
                                 && a.Department.Equals(Detail.Department)
                                 && a.Year.Equals(Detail.Year)
                                 && a.BudgetType.Equals(Detail.BudgetType)
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

                //cek dulu apakah sudah dalam pelaksanaan? apabila sudah maka di exclude
                var BPExist = (from a in ctx.BudgetingPengadaanDetails
                               join b in ctx.BudgetingPengadaanHeaders on a.BudgetingPengadaanId equals b.Id
                               join c in ctx.Pengadaans on b.PengadaanId equals c.Id
                               where a.Branch.Equals(Detail.Branch)
                                      && a.Department.Equals(Detail.Department)
                                      && a.Year.Equals(Detail.Year)
                                      && a.BudgetType.Equals(Detail.BudgetType)
                                      && c.GroupPengadaan == EGroupPengadaan.DALAMPELAKSANAAN
                                      && c.Id.Equals(PengadaanId)
                               select new VWBudgeting
                               {
                                   Branch = a.Branch,
                                   Department = a.Department,
                                   Year = a.Year,
                                   BudgetType = a.BudgetType,
                                   BudgetReserved = a.Input,
                                   NoCOA = a.NoCOA
                               }).ToList();

                List<VWBudgeting> dataBOPExist = BPExist.GroupBy(x =>
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

                var budgetlist = dataa.Where(x => x.Version == ver
                                                               && x.Department.Equals(Detail.Department)
                                                               && x.Branch.Equals(Detail.Branch)
                                                               && x.Year.Equals(Detail.Year)
                                                               && x.Jenis.Equals(Detail.BudgetType)).ToList();

                //var data = (from a in ctx.BudgetingPengadaanDetails
                //            join b in budgetlist on a.NoCOA equals b.COA
                //            where a.BudgetingPengadaanId == HeaderId).ToList();
                //List<ViewPertanyaan> listpertanyaan = new List<ViewPertanyaan>();

                var data = (from a in ctx.BudgetingPengadaanDetails
                            where a.BudgetingPengadaanId == HeaderId
                            select new VWBudgeting
                            {
                                //Description = b.Description,
                                //BudgetAmount = b.BudgetAmount,
                                //BudgetLeft = b.BudgetLeft,
                                NoCOA = a.NoCOA,
                                //Month = a.Month,
                                BudgetUsage = a.Input
                            }).ToList();

                foreach (var item in data)
                {
                    VWBudgeting vp = new VWBudgeting();
                    vp.Description = budgetlist.Where(x => x.COA.Equals(item.NoCOA)).Select(x => x.Description).Distinct().FirstOrDefault();
                    vp.NoCOA = item.NoCOA;
                    //vp.Month = item.Month;
                    vp.BudgetAmount = budgetlist.Where(x => x.COA.Equals(item.NoCOA)).Select(x => x.BudgetAmount).Distinct().FirstOrDefault();
                    vp.BudgetLeft = budgetlist.Where(x => x.COA.Equals(item.NoCOA)).Select(x => x.BudgetLeft).Distinct().FirstOrDefault();
                    vp.BudgetUsage = item.BudgetUsage;
                    decimal? budresv = dataBOP.Where(z => z.Branch == Detail.Branch
                                                   && z.Department == Detail.Department
                                                   && z.NoCOA == item.NoCOA
                                                   && z.Year == Detail.Year
                                                   && z.BudgetType == Detail.BudgetType).Select(x => x.BudgetReserved).Distinct().FirstOrDefault();
                    if (budresv != null)
                    {
                        vp.BudgetReserved = budresv;
                    }
                    else
                    {
                        vp.BudgetReserved = 0;
                        budresv = 0;
                    }
                    //ifexist
                    decimal? budresvExist = 0;
                    decimal? budresvExistCheck = dataBOPExist.Where(z => z.Branch == Detail.Branch
                                                  && z.Department == Detail.Department
                                                  && z.NoCOA == item.NoCOA
                                                  && z.Year == Detail.Year
                                                  && z.BudgetType == Detail.BudgetType).Select(x => x.BudgetReserved).Distinct().FirstOrDefault();
                    if (budresvExistCheck != null)
                    {
                        budresvExist = budresvExistCheck;
                    }
                    //end
                    decimal? a = budgetlist.Where(x => x.COA.Equals(item.NoCOA)).Select(x => x.BudgetLeft).Distinct().FirstOrDefault();
                    decimal? b = item.BudgetUsage;
                    vp.SisaBudgetOnProcess = a - b - budresv + budresvExist;
                    listVWBudgeting.Add(vp);
                }
            }
            //return listpertanyaan;
            return listVWBudgeting;
        }

        public List<VWTenderScoringDetails> nilaibobot(Guid Id)
        {
            var detail = (
                          //from b in ctx.TenderScoringHeaders
                          //join c in ctx.TenderScoringDetails on b.Id equals c.TenderScoringHeaderId into gj
                          //from c in gj.DefaultIfEmpty()
                          //join e in ctx.ReferenceDatas on c.Code equals e.Code
                          //join d in ctx.Vendors on b.VendorId equals d.Id
                          //orderby c.Id
                          from a in ctx.TenderScoringBobots
                          orderby a.Code
                          where a.PengadaanId == Id
                          select new VWTenderScoringDetails
                          {
                              pertanyaan = ctx.ReferenceDatas.Where(x => x.Code == a.Code).FirstOrDefault().LocalizedName,
                              Bobot = a.Bobot
                          }).ToList();
            return detail;
        }

        public VWReportAssessment detailnilaibobot(Guid Id)
        {
            var report = (from a in ctx.TenderScoringHeaders
                          where a.PengadaanId == Id
                          select new VWReportAssessment
                          {
                              NamaVendor = ctx.Vendors.Where(x => x.Id == a.VendorId).FirstOrDefault().Nama,
                              thereisvendor = a.VendorId,
                              PengadaanId = a.PengadaanId
                          }).ToList();

            List<VWTenderScoringUser> lsUser = new List<VWTenderScoringUser>();
            List<VWTenderScoringUser> lsScore = new List<VWTenderScoringUser>();
            List<VWReportAssessment> lsVendor = new List<VWReportAssessment>();
            foreach (var item in report)
            {
                var List_user = (from b in ctx.TenderScoringUsers
                                 join c in ctx.TenderScoringDetails on b.TenderScoringDetailId equals c.Id
                                 join d in ctx.TenderScoringHeaders on c.TenderScoringHeaderId equals d.Id
                                 join e in ctx.ReferenceDatas on c.Code equals e.Code
                                 where d.VendorId == item.thereisvendor && d.PengadaanId == item.PengadaanId
                                 select new VWTenderScoringUser
                                 {
                                     NamaUser = ctx.PersonilPengadaans.Where(x => x.PersonilId == b.UserId).FirstOrDefault().Nama,
                                     VendorId = item.thereisvendor,
                                     PengadaanId = d.PengadaanId,
                                     UserId = b.UserId
                                 }).OrderBy(x => x.UserId).Distinct().ToList();
                foreach (var itemx in List_user)
                {
                    var List_score = (from b in ctx.TenderScoringUsers
                                      join c in ctx.TenderScoringDetails on b.TenderScoringDetailId equals c.Id
                                      join d in ctx.TenderScoringHeaders on c.TenderScoringHeaderId equals d.Id
                                      join e in ctx.ReferenceDatas on c.Code equals e.Code
                                      where d.VendorId == itemx.VendorId && d.PengadaanId == itemx.PengadaanId && b.UserId == itemx.UserId
                                      select new VWTenderScoringUser
                                      {
                                          UserId = b.UserId,
                                          PengadaanId = d.PengadaanId,
                                          VendorId = d.VendorId,
                                          Score = b.Score,
                                          code = c.Code,
                                          Bobot = ctx.TenderScoringBobots.Where(x => x.PengadaanId == d.PengadaanId && x.Code == c.Code).FirstOrDefault().Bobot
                                      }).OrderBy(x => x.code).ToList();
                    foreach (var itemxx in List_score)
                    {
                        VWTenderScoringUser lss = new VWTenderScoringUser();
                        lss.VendorId = itemxx.VendorId;
                        lss.PengadaanId = itemxx.PengadaanId;
                        lss.UserId = itemx.UserId;
                        lss.Score = itemxx.Score;
                        lss.code = itemxx.code;
                        lss.Bobot = itemxx.Bobot;
                        lsScore.Add(lss);
                    }
                    VWTenderScoringUser lsu = new VWTenderScoringUser();
                    lsu.NamaUser = itemx.NamaUser;
                    lsu.VendorId = itemx.VendorId;
                    lsu.PengadaanId = itemx.PengadaanId;
                    lsu.UserId = itemx.UserId;
                    lsUser.Add(lsu);
                }
                VWReportAssessment lsv = new VWReportAssessment();
                lsv.NamaVendor = item.NamaVendor;
                lsv.thereisvendor = item.thereisvendor;
                lsv.PengadaanId = item.PengadaanId;
                lsVendor.Add(lsv);
            }

            VWReportAssessment kuya = new VWReportAssessment
            {
                ListScore = lsScore,
                ListUser = lsUser,
                ListVendor = lsVendor
            };


            return kuya;
        }

    }
}