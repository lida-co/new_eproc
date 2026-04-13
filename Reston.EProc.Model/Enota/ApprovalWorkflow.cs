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
    [Table("ApprovalWorkflows", Schema = AppDbContext.ENOTA_SCHEMA_NAME)]
    public class ApprovalWorkflowNota
    {
        [Key]
        public Guid? Id { get; set; }

        [ForeignKey("ENota")]
        public Guid? ENotaId { get; set; }

        public ApprovalWorkflowStateNota? WorkflowState { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string ModifiedBy { get; set; }

        public virtual ENota ENota { get; set; }

        public ICollection<ApprovalWorkflowDetailNota> ApprovalWorkflowDetails { get; set; }
    }

    [Table("ApprovalWorkflowDetails", Schema = AppDbContext.ENOTA_SCHEMA_NAME)]
    public class ApprovalWorkflowDetailNota
    {
        [Key]
        public Guid? Id { get; set; }

        [ForeignKey("ApprovalWorkflow")]
        public Guid? ApprovalWorkflowId { get; set; }

        [ForeignKey("Participant")]
        public Guid? ParticipantId { get; set; }

        public Guid? ParticipantUserId { get; set; }

        public string ParticipantName { get; set; }

        public ParticipantRoleNota? ParticipantRole { get; set; }

        public ApprovalDecissionNota? ApprovalDecission { get; set; }

        public string ApprovalNotes { get; set; }

        public DateTime? ApprovalDecissionDate { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string ModifiedBy { get; set; }

        public virtual ApprovalWorkflowNota ApprovalWorkflow { get; set; }

        public virtual ParticipantNota Participant { get; set; }

    }

    public enum ApprovalWorkflowStateNota {
        BEGIN,    
        VALIDATORS_PARTIALLY_APPROVE, 
        VALIDATORS_FULLY_APPROVE, 
        APPROVERS_PARTIALLY_APPROVE, 
        APPROVERS_FULLY_APPROVE,
        FINAL_APPROVERS_PARTIALLY_APPROVE,
        FINAL_APPROVERS_FULLY_APPROVE,
        CANCELLED,
        REJECTED,
        REVISION,
    }

    public enum ApprovalDecissionNota {
        APPROVE,
        REJECT,
        CONTINUE_WITH_NOTES,
        REVISION
    }


}
