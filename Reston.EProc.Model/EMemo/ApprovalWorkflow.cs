using Reston.Pinata.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Reston.Eproc.Model.EMemo
{
    [Table("ApprovalWorkflows", Schema = AppDbContext.EMEMO_SCHEMA_NAME)]
    public class ApprovalWorkflow
    {
        [Key]
        public Guid? Id { get; set; }

        [ForeignKey("EMemo")]
        public Guid? EMemoId { get; set; }

        public ApprovalWorkflowState? WorkflowState { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string ModifiedBy { get; set; }

        public virtual EMemo EMemo { get; set; }

        public ICollection<ApprovalWorkflowDetail> ApprovalWorkflowDetails { get; set; }
    }

    [Table("ApprovalWorkflowDetails", Schema = AppDbContext.EMEMO_SCHEMA_NAME)]
    public class ApprovalWorkflowDetail
    {
        [Key]
        public Guid? Id { get; set; }

        [ForeignKey("ApprovalWorkflow")]
        public Guid? ApprovalWorkflowId { get; set; }

        [ForeignKey("Participant")]
        public Guid? ParticipantId { get; set; }

        public Guid? ParticipantUserId { get; set; }

        public string ParticipantName { get; set; }

        public ParticipantRole? ParticipantRole { get; set; }

        public ApprovalDecission? ApprovalDecission { get; set; }

        public string ApprovalNotes { get; set; }

        public DateTime? ApprovalDecissionDate { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string ModifiedBy { get; set; }

        public virtual ApprovalWorkflow ApprovalWorkflow { get; set; }

        public virtual Participant Participant { get; set; }

    }

    public enum ApprovalWorkflowState {
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

    public enum ApprovalDecission {
        APPROVE,
        REJECT,
        CONTINUE_WITH_NOTES,
        REVISION
    }


}
