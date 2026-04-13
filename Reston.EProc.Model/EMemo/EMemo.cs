using Reston.Pinata.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Model.Helper.IdLdapConstants;

namespace Reston.Eproc.Model.EMemo
{
    [Table("EMemo", Schema = AppDbContext.EMEMO_SCHEMA_NAME)]
    public class EMemo
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
        public ICollection<EMemoAttachment> EMemoAttachments { get; set; }
        public ICollection<Participant> Participants { get; set; }
        public ICollection<EMemoLogs> EMemoLogs { get; set; }
    }

    [Table("Participants", Schema = AppDbContext.EMEMO_SCHEMA_NAME)]
    public class Participant
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid? Id { get; set; }
        public ParticipantRole? ParticipantRole { get; set; }
        public string UserId { get; set; }
        public string EmployeeName { get; set; }
        public int? Ordered { get; set; }
        [ForeignKey("EMemo")]
        public Guid? EMemoId { get; set; }
        public virtual EMemo EMemo { get; set; }
    }

    [Table("Attachments", Schema = AppDbContext.EMEMO_SCHEMA_NAME)]
    public class Attachment
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid? Id { get; set; }
        public string Title { get; set; }
        public string ContentType { get; set; }
        public byte[] ContentData { get; set; }
    }

    [Table("EMemoAttachments", Schema = AppDbContext.EMEMO_SCHEMA_NAME)]
    public class EMemoAttachment
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid? Id { get; set; }
        [ForeignKey("EMemo")]
        public Guid? EMemoId { get; set; }
        [ForeignKey("Attachment")]
        public Guid? AttachmentId { get; set; }
        public AttachmentType? AttachmentType { get; set; }
        public int? Ordered { get; set; }
        public virtual EMemo EMemo { get; set; }
        public virtual Attachment Attachment { get; set; }
    }

    [Table("AspNetUsers", Schema = AppDbContext.DBO_SCHEMA_NAME)]
    public class VUserAccount
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        
        public string UserName { get; set; }

        public string DisplayName { get; set; }

    }

    [Table("DocumentNumbers", Schema = AppDbContext.EMEMO_SCHEMA_NAME)]
    public class DocumentNumber
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid? Id { get; set; }

        public string WorkUnitCode { get; set; }

        public string LastEMemoDocNo { get; set; }

    }


    [Table("EMemoLogs", Schema = AppDbContext.EMEMO_SCHEMA_NAME)]
    public class EMemoLogs
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid? Id { get; set; }
        public Guid? EMemoId { get; set; }
        public int? Version { get; set; }
        public string Content { get; set; }
        public DateTime? CreateDate { get; set; }
        public Decimal HPSAmount { get; set; }
        public object UserId { get; set; }
        public string Status { get; set; }
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


    [Table("EMemoTemplates", Schema = AppDbContext.EMEMO_SCHEMA_NAME)]
    public class EMemoTemplate
    { 
        public Guid? Id { get; set; }

        public string Title { get; set; }

        public string ContentType { get; set; }

        public byte[] ContentData { get; set; }
    }

        public enum StatusEMemo
    {
        Draft, 
        Validasi, 
        ValidasiDitolak, 
        ApprovalRequest, 
        ApprovalReject, 
        Approved
    }

    public enum AttachmentType
    {
        EMEMO_ATT_QUOTATION, 
        EMEMO_ATT_COSTBENEFIT, 
        EMEMO_ATT_MEMO, 
        EMEMO_ATT_OTHER
    }

    public enum ParticipantRole
    {
        EMEMO_PTCP_FROM = 0,
        EMEMO_PTCP_REVIEWER = 1, 
        EMEMO_PTCP_APPROVER = 2,
        EMEMO_PTCP_DIREKSI = 3,
    }
}