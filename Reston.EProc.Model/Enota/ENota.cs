using Reston.Pinata.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reston.Eproc.Model.ENota
{
    [Table("ENota", Schema = AppDbContext.ENOTA_SCHEMA_NAME)]
    public class ENota
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid? Id { get; set; }
        public string Subject { get; set; }
        public string DocumentNo { get; set; }
        public string WorkUnitCode { get; set; }
        public string InternalRefNo { get; set; }
        public decimal? HPSAmount { get; set; }
        public string HPSCurrency { get; set; }
        public int IsDraft { get; set; }
        public string Kepada { get; set; }
        public string Tembusan { get; set; }
        public string Owner { get; set; }
        public string OwnerPersonelName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public ICollection<ENotaAttachment> ENotaAttachments { get; set; }
        public ICollection<ParticipantNota> Participants { get; set; }
        public ICollection<ENotaLogs> ENotaLogs { get; set; }
    }

    [Table("Participants", Schema = AppDbContext.ENOTA_SCHEMA_NAME)]
    public class ParticipantNota
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid? Id { get; set; }
        public ParticipantRoleNota? ParticipantRole { get; set; }
        public string UserId { get; set; }
        public string EmployeeName { get; set; }
        public int? Ordered { get; set; }
        [ForeignKey("ENota")]
        public Guid? ENotaId { get; set; }
        public virtual ENota ENota { get; set; }
    }
    [Table("ENotaLogs", Schema = AppDbContext.ENOTA_SCHEMA_NAME)]
    public class ENotaLogs
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid? Id { get; set; }
        public int Version { get; set; }
        public string Content { get; set; }
        public Decimal HPSAmount { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? ENotaId { get; set; }
        public string AttactmentPenawaran { get; set; }
        public string AttactmentPenawaranType { get; set; }
        public byte[] AttactmentPenawaranFiles { get; set; }
        public string AttachtmentAnalisa { get; set; }
        public string AttachtmentAnalisaType { get; set; }
        public byte[] AttachtmentAnalisaFile { get; set; }
        public string AttachmentLampiran { get; set; }
        public string AttachmentLampiranType { get; set; }
        public byte[] AttachmentLampiranFile { get; set; }
    }

    [Table("Attachments", Schema = AppDbContext.ENOTA_SCHEMA_NAME)]
    public class AttachmentNota
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid? Id { get; set; }
        public string Title { get; set; }
        public string ContentType { get; set; }
        public byte[] ContentData { get; set; }
    }

    [Table("ENotaAttachments", Schema = AppDbContext.ENOTA_SCHEMA_NAME)]
    public class ENotaAttachment
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid? Id { get; set; }
        [ForeignKey("ENota")]
        public Guid? ENotaId { get; set; }
        [ForeignKey("Attachment")]
        public Guid? AttachmentId { get; set; }
        public AttachmentTypeNota? AttachmentType { get; set; }
        public int? Ordered { get; set; }
        public virtual ENota ENota { get; set; }
        public virtual AttachmentNota Attachment { get; set; }
    }

   

    [Table("DocumentNumbers", Schema = AppDbContext.ENOTA_SCHEMA_NAME)]
    public class DocumentNumberNota
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid? Id { get; set; }

        public string WorkUnitCode { get; set; }

        public string LastENotaDocNo { get; set; }

    }

    [Table("ENotaTemplates", Schema = AppDbContext.ENOTA_SCHEMA_NAME)]
    public class ENotaTemplate
    { 
        public Guid? Id { get; set; }

        public string Title { get; set; }

        public string ContentType { get; set; }

        public byte[] ContentData { get; set; }
    }

        public enum StatusENota
    {
        Draft, 
        Validasi, 
        ValidasiDitolak, 
        ApprovalRequest, 
        ApprovalReject, 
        Approved
    }

    public enum AttachmentTypeNota
    {
        ENOTA_ATT_QUOTATION, 
        ENOTA_ATT_COSTBENEFIT, 
        ENOTA_ATT_NOTA, 
        ENOTA_ATT_OTHER
    }

    public enum ParticipantRoleNota
    {
        ENOTA_PTCP_FROM = 0,
        ENOTA_PTCP_REVIEWER = 1, 
        ENOTA_PTCP_APPROVER = 2,
        ENOTA_PTCP_DIREKTUR = 3,
    }
}