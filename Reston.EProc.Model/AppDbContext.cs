using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
using Reston.Pinata.Model.JimbisModel;
using Reston.Pinata.Model.Asuransi;
using Reston.Pinata.Model.PengadaanRepository;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure;
using Reston.Eproc.Model.Monitoring.Entities;
using Reston.Eproc.Model.Entities;
using Reston.Eproc.Model.EMemo;
using Reston.Eproc.Model.ENota;
using Reston.Pinata.Model.Asuransi;
using NLog;
using System.Reflection;

namespace Reston.Pinata.Model
{
    public class SysLog
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public string RecordID { get; set; }
        public string UserID { get; set; }
        public string ColumnName { get; set; }
        public string CurrentValue { get; set; }
        public string BeforeValue { get; set; }
        public Nullable<DateTime> EventDateUTC { get; set; }
        public string EventType { get; set; }
    }

    public class AppDbContext:DbContext
    {
        private static Logger _log = LogManager.GetCurrentClassLogger();

        public const string VENDOR_SCHEMA_NAME = "vendor";
        public const string VENDORREG_SCHEMA_NAME = "vendorreg";
        public const string CATALOG_SCHEMA_NAME = "catalog";
        public const string MASTER_SCHEMA_NAME = "master";
        public const string PENGADAAN_SCHEMA_NAME = "pengadaan";
        public const string MONITORING_SCHEMA_NAME = "monitoring";
        public const string WORKFLOW_SCHEMA_NAME = "workflow";
        public const string PROYEK_SCHEMA_NAME = "proyek";
        public const string PO_SCHEMA_NAME = "po";
        public const string MENU_SCHEMA_NAME = "menu";
        public const string ASURANSI_SCHEMA_NAME = "asuransi";
        public const string DBO_SCHEMA_NAME = "dbo";
        public const string EMEMO_SCHEMA_NAME = "ememo";
        public const string ENOTA_SCHEMA_NAME = "enota";

        public AppDbContext()
            : base("name=JimbisEntities")
        {
            //Configuration.ProxyCreationEnabled = false;
            Database.Log = (msg) => _log.Debug(msg);
        }
		
        public virtual DbSet<Vendor> Vendors { get; set; }
        public virtual DbSet<BankInfo> BankInfos { get; set; }
        public virtual DbSet<VendorPerson> VendorPersons { get; set; }
        public virtual DbSet<RiwayatPengajuanVendor> RiwayatPengajuans { get; set; }
        public virtual DbSet<Dokumen> Dokumens { get; set; }
        public virtual DbSet<DokumenDetail> DokumenDetails { get; set; }
        public virtual DbSet<AktaDokumenDetail> AktaDokumenDetails { get; set; }
        public virtual DbSet<IzinUsahaDokumenDetail> IzinUsahaDokumenDetails { get; set; }
        public virtual DbSet<RegVendor> RegVendors { get; set; }
        public virtual DbSet<RegBankInfo> RegBankInfos { get; set; }
        public virtual DbSet<RegVendorPerson> RegVendorPersons { get; set; }
        public virtual DbSet<RegRiwayatPengajuanVendor> RegRiwayatPengajuanVendors { get; set; }
        public virtual DbSet<RegDokumen> RegDokumens { get; set; }
        public virtual DbSet<RegDokumenDetail> RegDokumenDetails { get; set; }
        public virtual DbSet<RegAktaDokumenDetail> RegAktaDokumenDetails { get; set; }
        public virtual DbSet<RegIzinUsahaDokumenDetail> RegIzinUsahaDokumenDetails { get; set; }
        public virtual DbSet<Produk> Produks { get; set; }
        public virtual DbSet<KategoriSpesifikasi> KategoriSpesifikasis { get; set; }
        public virtual DbSet<AtributSpesifikasi> AtributSpesifikasis { get; set; }
        public virtual DbSet<RiwayatHarga> RiwayatHargas { get; set; }
        public virtual DbSet<Pengadaan> Pengadaans { get; set; }
        public virtual DbSet<DokumenPengadaan> DokumenPengadaans { get; set; }
        public virtual DbSet<KandidatPengadaan> KandidatPengadaans { get; set; }
        public virtual DbSet<HistoryKandidatPengadaan> HistoryKandidatPengadaan { get; set; }
        public virtual DbSet<JadwalPengadaan> JadwalPengadaans { get; set; }
        public virtual DbSet<PersonilPengadaan> PersonilPengadaans { get; set; }
        public virtual DbSet<RiwayatPengadaan> RiwayatPengadaans { get; set; }
        public virtual DbSet<BintangPengadaan> BintangPengadaans { get; set; }
        public virtual DbSet<RKSHeader> RKSHeaders { get; set; }
        public virtual DbSet<RKSDetail> RKSDetails { get; set; }
        public virtual DbSet<MessagePengadaan> MessagePengadaans { get; set; }
        public virtual DbSet<ReferenceData> ReferenceDatas { get; set; }
        public virtual DbSet<PelaksanaanAanwijzing> PelaksanaanAanwijzings { get; set; }
        public virtual DbSet<KehadiranKandidatAanwijzing> KehadiranKandidatAanwijzings { get; set; }
        public virtual DbSet<PelaksanaanSubmitPenawaran> PelaksanaanSubmitPenawarans { get; set; }
        public virtual DbSet<PelaksanaanBukaAmplop> PelaksanaanBukaAmplops { get; set; }
        public virtual DbSet<PersetujuanBukaAmplop> PersetujuanBukaAmplops { get; set; }
        public virtual DbSet<PelaksanaanPenilaianKandidat> PelaksanaanPenilaianKandidats { get; set; }
        public virtual DbSet<PemenangPengadaan> PemenangPengadaans { get; set; }
        public virtual DbSet<PelaksanaanKlarifikasi> PelaksanaanKlarifikasis { get; set; }
        public virtual DbSet<JadwalPelaksanaan> JadwalPelaksanaans { get; set; }
        public virtual DbSet<KualifikasiKandidat> KualifikasiKandidats { get; set; }
        public virtual DbSet<HargaRekanan> HargaRekanans { get; set; }
        public virtual DbSet<PelaksanaanPemilihanKandidat> PelaksanaanPemilihanKandidats { get; set; }
        public virtual DbSet<HargaKlarifikasiRekanan> HargaKlarifikasiRekanans { get; set; }
        public virtual DbSet<CatatanPengadaan> CatatanPengadaans { get; set; }
        public virtual DbSet<KreteriaPembobotan> KreteriaPembobotans { get; set; }
        public virtual DbSet<PembobotanPengadaan> PembobotanPengadaans { get; set; }
        public virtual DbSet<PembobotanPengadaanVendor> PembobotanPengadaanVendors { get; set; }
        public virtual DbSet<NoDokumenGenerator> NoDokumenGenerators { get; set; }
        public virtual DbSet<BeritaAcara> BeritaAcaras { get; set; }
        public virtual DbSet<ReportPengadaan> ReportPengadaans { get; set; }
        public virtual DbSet<CaptchaRegistration> CaptchaRegistration { get; set; }
        public virtual DbSet<PembatalanPengadaan> PembatalanPengadaans { get; set; }
        public virtual DbSet<PenolakanPengadaan> PenolakanPengadaans { get; set; }
        public virtual DbSet<RiwayatDokumen> RiwayatDokumens { get; set; }
        public virtual DbSet<RKSHeaderTemplate> RKSHeaderTemplate { get; set; }
        public virtual DbSet<RKSDetailTemplate> RKSDetailTemplate { get; set; }
        public virtual DbSet<MonitoringPekerjaan> MonitoringPekerjaans { get; set; }
        public virtual DbSet<DetailPekerjaan> DetailPekerjaans { get; set; }
        public virtual DbSet<JadwalProyek> JadwalProyeks { get; set; }
        public virtual DbSet<HargaKlarifikasiLanLanjutan> HargaKlarifikasiLanLanjutans { get; set; }
        public virtual DbSet<MasterBranch> MasterBranchs { get; set; }
        public virtual DbSet<MasterDepartment> MasterDepartments { get; set; }
        public virtual DbSet<MasterBranchDepartmentRelationship> MasterBranchDepartmentRelationships { get; set; }
        public virtual DbSet<DokumenBudget> DokumenBudgets { get; set; }
        public virtual DbSet<Budgeting> Budgetings { get; set; }
        public virtual DbSet<BudgetingPengadaanHeader> BudgetingPengadaanHeaders { get; set; }
        public virtual DbSet<BudgetingPengadaanDetail> BudgetingPengadaanDetails { get; set; }

        //--------
        //Asuransi
        public virtual DbSet<InsuranceTarif> InsuranceTarifs { get; set; }
        public virtual DbSet<InsuranceTarifBenefit> InsuranceTarifBenefits { get; set; }
        public virtual DbSet<InsuranceTarifTemplate> InsuranceTarifTemplates { get; set; }
        public virtual DbSet<InsuranceTarifBenefitTemplate> InsuranceTarifBenefitTemplates { get; set; }
        public virtual DbSet<BenefitRate> BenefitRates { get; set; }
        public virtual DbSet<BenefitRateTemplate> BenefitRateTemplates { get; set; }
        public virtual DbSet<IncurenceValuePengadaan> IncurenceValuePengadaans { get; set; }
        public virtual DbSet<HargaRekananAsuransi> HargaRekananAsuransis { get; set; }
        public virtual DbSet<HargaKlarifikasiRekananAsuransi> HargaKlarifikasiRekananAsuransis { get; set; }
        public virtual DbSet<HargaKlarifikasiLanjutanAsuransi> HargaKlarifikasiLanjutanAsuransis { get; set; }

        //----------------------------------------------------------------------------------
        // Proyek
        public virtual DbSet<TahapanProyek> TahapanProyeks { get; set; }
        public virtual DbSet<RencanaProyek> RencanaProyeks { get; set; }
        public virtual DbSet<PICProyek> PICProyeks { get; set; }
        public virtual DbSet<DokumenProyek> DokumenProyeks { get; set; }
        
        public virtual DbSet<PersetujuanPemenang> PersetujuanPemenangs { get; set; }
        public virtual DbSet<Pks> Pks { get; set; }
        public virtual DbSet<CatatanPks> CatatanPks { get; set; }
		public virtual DbSet<PenilaianVendorHeader> PenilaianVendorHeaders { get; set; }
        public virtual DbSet<PenilaianVendorDetail> PenilaianVendorDetails { get; set; }
        public virtual DbSet<DokumenPks> DokumenPks { get; set; }
         public virtual DbSet<RiwayatDokumenPks> RiwayatDokumenPks { get; set; }
        public virtual DbSet<Spk> Spk { get; set; }
        public virtual DbSet<DokumenSpk> DokumenSpk { get; set; }
        public virtual DbSet<RiwayatDokumenSpk> RiwayatDokumenSpk { get; set; }
        public virtual DbSet<COA> COAs { get; set; }
        
        public virtual DbSet<PO> POs { get; set; }
        public virtual DbSet<PODetail> PODetails { get; set; }
        public virtual DbSet<DokumenPO> DokumenPO { get; set; }
        public virtual DbSet<LewatTahapan> LewatTahapans { get; set; }
        public virtual DbSet<TenderScoringHeader> TenderScoringHeaders { get; set; }
        public virtual DbSet<TenderScoringBobot> TenderScoringBobots { get; set; }
        public virtual DbSet<TenderScoringDetail> TenderScoringDetails { get; set; }
        public virtual DbSet<TenderScoringUser> TenderScoringUsers { get; set; }
        public virtual DbSet<DokumenSpkNonPks> DokumenSpkNonPks { get; set; }

        //persetujuan tahapan

        public virtual DbSet<PersetujuanTahapan> PersetujuanTahapans { get; set; }
        //
        public virtual DbSet<PersetujuanTerkait> PersetujuanTerkait { get; set; }
        
        //menu
        public virtual DbSet<Reston.Eproc.Model.Entities.Menu> Menu { get; set; }
        public virtual DbSet<RoleMenu> RoleMenu { get; set; }
        public virtual DbSet<SysLog> SysLogs { get; set; }

        //Asuransi
        //public virtual DbSet<Benefit> Benefits { get; set; }
        //public virtual DbSet<RiwayatRate> RiwayatRates { get; set; }
        //public virtual DbSet<RiwayatUP> RiwayatUPs { get; set; }
        //public virtual DbSet<KategoriSpesifikasiAsuransi> KategoriSpesifikasiAsuransis { get; set; }

        //Penilaian
        public virtual DbSet<TenderScoringPenilai> TenderScoringPenilais { get; set; }
        public virtual DbSet<ApprisalWorksheet> ApprisalWorksheets { get; set; }
        public virtual DbSet<ApprisalWorksheetDetail> ApprisalWorksheetDetails { get; set; }
        public virtual DbSet<ApprisalWorksheetResponse> ApprisalWorksheetResponses { get; set; }
        public virtual DbSet<ApprisalWorksheetResposeDetail> ApprisalWorksheetResposeDetails { get; set; }
        public virtual DbSet<Sanksi> Sanksis { get; set; }

        //vendor ext reg
        public virtual DbSet<RegVendorExt> RegVendorExts { get; set; }
        public virtual DbSet<RegVendorExtBankInfo> RegVendorExtBankInfoes { get; set; }
        public virtual DbSet<RegVendorExtPerson> RegVendorExtPersons { get; set; }
        public virtual DbSet<RegVendorExtHumanResource> RegVendorExtHumanResources { get; set; }
        public virtual DbSet<RegVendorExtFinStatement> RegVendorExtFinStatements { get; set; } 
        public virtual DbSet<RegVendorExtEquipment> RegVendorExtEquipments { get; set; }
        public virtual DbSet<RegVendorExtJobHistory> RegVendorExtJobHistories { get; set; }
        public virtual DbSet<RegVendorDocumentExt> RegVendorDocumentExts { get; set; }
        public virtual DbSet<RegDocumentExt> RegDocumentExts { get; set; }
        public virtual DbSet<RegDocumentImageExt> RegDocumentImageExts { get; set; }

        //vendor ext 
        public virtual DbSet<VendorExt> VendorExts { get; set; }
        public virtual DbSet<VendorExtBankInfo> VendorExtBankInfoes { get; set; }
        public virtual DbSet<VendorExtPerson> VendorExtPersons { get; set; }
        public virtual DbSet<VendorExtHumanResource> VendorExtHumanResources { get; set; }
        public virtual DbSet<VendorExtFinStatement> VendorExtFinStatements { get; set; }
        public virtual DbSet<VendorExtEquipment> VendorExtEquipments { get; set; }
        public virtual DbSet<VendorExtJobHistory> VendorExtJobHistories { get; set; }
        public virtual DbSet<VendorDocumentExt> VendorDocumentExts { get; set; }
        public virtual DbSet<DocumentExt> DocumentExts { get; set; }
        public virtual DbSet<DocumentImageExt> DocumentImageExts { get; set; }


        // --------------
        // EMemo
        // --------------

        public virtual DbSet<EMemo> EMemos { get; set; }

        public virtual DbSet<EMemoTemplate> EMemoTemplates { get; set; }

        public virtual DbSet<Eproc.Model.EMemo.Participant> Participants { get; set; }
        
        public virtual DbSet<EMemoAttachment> EMemoAttachments { get; set; }
        
        public virtual DbSet<Eproc.Model.EMemo.Attachment> Attachments { get; set; }

        public virtual DbSet<Eproc.Model.EMemo.DocumentNumber> DocumentNumbers { get; set; }

        public virtual DbSet<Eproc.Model.EMemo.EMemoLogs> EMemoLogs { get; set; }

        public virtual DbSet<Eproc.Model.EMemo.ApprovalWorkflow> ApprovalWorkflows { get; set; }

        public virtual DbSet<Eproc.Model.EMemo.ApprovalWorkflowDetail> ApprovalWorkflowDetails { get; set; }

        public virtual DbSet<Eproc.Model.EMemo.VUserAccount> VUserAccounts { get; set; }

        // --------------
        // ENota
        // --------------

        public virtual DbSet<ENota> ENotas { get; set; }

        public virtual DbSet<ENotaTemplate> ENotaTemplates { get; set; }

        public virtual DbSet<Eproc.Model.ENota.ParticipantNota> ParticipantsNota { get; set; }

        public virtual DbSet<ENotaAttachment> ENotaAttachments { get; set; }

        public virtual DbSet<Eproc.Model.ENota.AttachmentNota> AttachmentsNota { get; set; }

        public virtual DbSet<Eproc.Model.ENota.DocumentNumberNota> DocumentNumbersNota { get; set; }

        public virtual DbSet<Eproc.Model.ENota.ApprovalWorkflowNota> ApprovalWorkflowsNota { get; set; }

        public virtual DbSet<Eproc.Model.ENota.ApprovalWorkflowDetailNota> ApprovalWorkflowDetailsNota { get; set; }
        public DbSet<Eproc.Model.ENota.ENotaLogs> ENotaLogs { get; set; }


        public void ValidateReferenceData()
        {
            //Qualifier dan Code tidak bisa diubah
            foreach (var entry in this.ChangeTracker.Entries<ReferenceData>().Where(a => a.State == EntityState.Modified))
            {
                entry.Property(a => a.Qualifier).IsModified = false;
                entry.Property(a => a.Code).IsModified = false;
            }

            //untuk reference data yang baru dimasukkan, cek reference data
            foreach (var entry in this.ChangeTracker.Entries<ReferenceData>().Where(a => a.State == EntityState.Added))
            {
                //cari yang qualifier dan kodenya sama
                var duplicates = this.ReferenceDatas.Where(a =>
                    a.Qualifier == entry.Entity.Qualifier
                    && a.Code == entry.Entity.Code).ToList();

                //gunakan prefix untuk menghindari duplikat pada kode yang sudah dihapus
                if (duplicates.Count > 0)
                {
                    entry.Entity.Code += duplicates.Count;
                }
            }
        }

        #region Audit Log
        public int SaveChanges(string userId)
        {
            var now = DateTime.Now;

            try
            {
                // Get all Added/Deleted/Modified entities (not Unmodified or Detached)
                foreach (var ent in this.ChangeTracker.Entries().Where(p => p.State == EntityState.Added || p.State == EntityState.Deleted || p.State == EntityState.Modified))
                {
                    // For each changed record, get the audit record entries and add them
                    foreach (SysLog x in GetAuditRecordsForChange(ent, userId))
                    {
                        this.SysLogs.Add(x);
                    }

                    if (ent.State == EntityState.Added)
                    {
                        SetAllAuditFields(ent.Entity, userId, now, userId, now);
                    }
                    else if (ent.State == EntityState.Modified)
                    {
                        SetUpdateAuditFields(ent.Entity, userId, now);
                    }
                }
            }
            catch
            {
            }

            // Call the original SaveChanges(), which will save both the changes made and the audit records
            return base.SaveChanges();
        }

        private string GetPrimaryKeyName(DbEntityEntry entry)
        {
            var objectStateEntry = ((IObjectContextAdapter)this).ObjectContext.ObjectStateManager.GetObjectStateEntry(entry.Entity);
            return objectStateEntry.EntitySet.ElementType.KeyMembers.Single().Name;
        }

        private List<SysLog> GetAuditRecordsForChange(DbEntityEntry dbEntry, string userId)
        {
            List<SysLog> result = new List<SysLog>();

            DateTime changeTime = DateTime.Now;
            TableAttribute tableAttr = null;
            string tableName = string.Empty;
            string keyName = string.Empty;

            try
            {
                // Get the Table() attribute, if one exists
                tableAttr = dbEntry.Entity.GetType().GetCustomAttributes(typeof(TableAttribute), false).SingleOrDefault() as TableAttribute;

                // Get table name (if it has a Table attribute, use that, otherwise get the pluralized name)
                tableName = tableAttr != null ? tableAttr.Name : dbEntry.Entity.GetType().Name;

                // Get primary key value (If you have more than one key column, this will need to be adjusted)
                keyName = GetPrimaryKeyName(dbEntry);


                if (dbEntry.State == EntityState.Added)
                {
                    // For Inserts, just add the whole record
                    result.Add(new SysLog()
                    {
                        UserID = userId,
                        EventDateUTC = changeTime,
                        EventType = "A", // Added
                        TableName = tableName,
                        RecordID = dbEntry.CurrentValues.GetValue<object>(keyName).ToString(),
                        ColumnName = "*ALL",
                        CurrentValue = DescribeEntity(dbEntry.CurrentValues.ToObject())
                    }
                        );
                }
                else if (dbEntry.State == EntityState.Deleted)
                {
                    // Same with deletes, do the whole record
                    result.Add(new SysLog()
                    {
                        UserID = userId,
                        EventDateUTC = changeTime,
                        EventType = "D", // Deleted
                        TableName = tableName,
                        RecordID = dbEntry.OriginalValues.GetValue<object>(keyName).ToString(),
                        ColumnName = "*ALL",
                        BeforeValue = DescribeEntity(dbEntry.OriginalValues.ToObject())
                    }
                        );
                }
                else if (dbEntry.State == EntityState.Modified)
                {
                    foreach (string propertyName in dbEntry.OriginalValues.PropertyNames)
                    {
                        // For updates, we only want to capture the columns that actually changed
                        if (!object.Equals(dbEntry.GetDatabaseValues().GetValue<object>(propertyName), dbEntry.CurrentValues.GetValue<object>(propertyName)))
                        {
                            result.Add(new SysLog()
                            {
                                UserID = userId,
                                EventDateUTC = changeTime,
                                EventType = "M",    // Modified
                                TableName = tableName,
                                RecordID = dbEntry.OriginalValues.GetValue<object>(keyName).ToString(),
                                ColumnName = propertyName,
                                BeforeValue = dbEntry.GetDatabaseValues().GetValue<object>(propertyName) == null ? null : dbEntry.GetDatabaseValues().GetValue<object>(propertyName).ToString(),
                                CurrentValue = dbEntry.CurrentValues.GetValue<object>(propertyName) == null ? null : dbEntry.CurrentValues.GetValue<object>(propertyName).ToString()
                            }
                                );
                        }
                    }
                }
            }
            catch
            {
            }

            return result;
        }

        private string DescribeEntity(object obj)
        {
            string returnValue = string.Empty;
            returnValue = "{";
            var properties = from p in obj.GetType().GetProperties()
                             where p.CanRead && p.CanWrite
                             select p;
            foreach (var property in properties)
            {
                var value = property.GetValue(obj, null) == null ? string.Empty : property.GetValue(obj, null).ToString();
                returnValue += property.Name + ":" + "\"" + value + "\" , ";

            }
            returnValue += "}";
            return returnValue;
        }
        #endregion

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RiwayatHarga>().Property(x => x.Harga).HasPrecision(18, 4);
        }

        private void SetAllAuditFields(object instance, string createdBy, DateTime? createdDt, string updatedBy, DateTime? updatedDt)
        {
            SetPropertyIfExists(instance, "CreatedBy", createdBy);
            SetPropertyIfExists(instance, "CreatedDate", createdDt);
            SetPropertyIfExists(instance, "ModifiedBy", updatedBy);
            SetPropertyIfExists(instance, "ModifiedDate", updatedDt);
        }

        private void SetUpdateAuditFields(object instance, string updatedBy, DateTime? updatedDt)
        {
            SetPropertyIfExists(instance, "ModifiedBy", updatedBy);
            SetPropertyIfExists(instance, "ModifiedDate", updatedDt);
        }


        private void SetPropertyIfExists(object instance, string propertyName, object value)
        {
            Type type = instance.GetType();
            Type paramType = value.GetType();
            PropertyInfo propertyInfo = type.GetProperty(propertyName);
            if (propertyInfo != null && propertyInfo.CanWrite)
            {
                MethodInfo setMethod = propertyInfo.GetSetMethod();

                if (setMethod != null && setMethod.GetParameters().Length == 1)
                {
                    var propertyType = setMethod.GetParameters()[0].ParameterType;

                    if (Nullable.GetUnderlyingType(propertyType) != null)
                    {
                        propertyType = Nullable.GetUnderlyingType(propertyType);
                    }

                    if (propertyType == paramType) 
                    {
                        setMethod.Invoke(instance, new object[] { value });
                    }
                    
                }
            }
        }
    }

    
}
