using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reston.EProc.Web.Base.ViewModels;


namespace Reston.EProc.Web.Modules.EMemo.ViewModels
{
    public class EMemoRequest : BaseFilteredRequest
    {
        public EMemoRequestDetail Detail { get; set; }

        public new EMemoFilter Filter { get; set; }
    }

    public class EMemoRequestDetail
    {

        public EMemoRequestDetail()
        {
            Validators = new List<EMemoParticipant>();
            Approvers = new List<EMemoParticipant>();
            PenawaranAttachments = new List<EMemoAttachment>();
            CostBenefitAttachments = new List<EMemoAttachment>();
            OtherAttachments = new List<EMemoAttachment>();
            ApprovalWorkflowDetails = new List<EMemoApprovalWorkflowDetailRequestDetail>();
        }

        public Guid? Id { get; set; }

        public string DocumentNo { get; set; }

        public string WorkUnitCode { get; set; }

        public string WorkUnitName { get; set; }

        public string InternalRefNo { get; set; }

        public string Subject { get; set; }

        public bool? IsDraft { get; set; }

        public string OwnerUserId { get; set; }

        public string Kepada { get; set; }

        public string Tembusan { get; set; }
        public decimal HPSAmount { get; set; }

        public string ContentInputMode { get; set; }
        public int? ContentSize { get; set; }

        public string ContentTitle { get; set; }

        public string ContentType { get; set; }

        public string ContentData { get; set; }

        public IList<EMemoAttachment> PenawaranAttachments { get; set; }

        public IList<EMemoAttachment> CostBenefitAttachments { get; set; }

        public IList<EMemoAttachment> OtherAttachments { get; set; }

        public IList<EMemoParticipant> Validators { get; set; }

        public IList<EMemoParticipant> Approvers { get; set; }

        public IList<EMemoParticipant> Direksi { get; set; }

        public IList<EMemoApprovalWorkflowDetailRequestDetail> ApprovalWorkflowDetails { get; set; }
       
    }

    public class EMemoResponse : BaseFilteredResponse
    {
        public EMemoResponseDetail Detail { get; set; }

        public IList<EMemoResponseDetail> Details { get; set; }

        public new EMemoFilter Filter { get; set; }

    }

    public class EMemoResponseDetail
    {

        public EMemoResponseDetail()
        {
            Validators = new List<EMemoParticipant>();
            Approvers = new List<EMemoParticipant>();
            PenawaranAttachments = new List<EMemoAttachment>();
            CostBenefitAttachments = new List<EMemoAttachment>();
            OtherAttachments = new List<EMemoAttachment>();
            ApprovalWorkflowDetails = new List<EMemoApprovalWorkflowDetailResponseDetail>();
        }

        public EMemoResponseDetail(EMemoRequestDetail request)
        {
            Id = request.Id;
            DocumentNo = request.DocumentNo;
            WorkUnitCode = request.WorkUnitCode;
            WorkUnitName = request.WorkUnitName;
            InternalRefNo = request.InternalRefNo;
            Subject = request.Subject;
            IsDraft = request.IsDraft;
            OwnerUserId = request.OwnerUserId;
            ContentSize = request.ContentSize;
            ContentTitle = request.ContentTitle;
            ContentType = request.ContentType;
            ContentData = request.ContentData;
            Kepada = request.Kepada;
            Tembusan = request.Tembusan;
            HPSAmount = request.HPSAmount;
            Validators = new List<EMemoParticipant>(request.Validators);
            Approvers = new List<EMemoParticipant>(request.Approvers);
            PenawaranAttachments = new List<EMemoAttachment>(request.PenawaranAttachments);
            CostBenefitAttachments = new List<EMemoAttachment>(request.CostBenefitAttachments);
            OtherAttachments = new List<EMemoAttachment>(request.OtherAttachments);
            ApprovalWorkflowDetails = new List<EMemoApprovalWorkflowDetailResponseDetail>();
        }

        public Guid? Id { get; set; }

        public string DocumentNo { get; set; }

        public string WorkUnitCode { get; set; }

        public string WorkUnitName { get; set; }

        public string InternalRefNo { get; set; }

        public string Subject { get; set; }

        public bool? IsDraft { get; set; }

        public string WorkflowStatus { get; set; }

        public string OwnerUserId { get; set; }

        public string Kepada { get; set; }

        public string Tembusan { get; set; }

        public string ContentInputMode { get; set; }

        public int? ContentSize { get; set; }

        public string ContentTitle { get; set; }

        public string ContentType { get; set; }

        public string ContentData { get; set; }

        public IList<EMemoAttachment> PenawaranAttachments { get; set; }

        public IList<EMemoAttachment> CostBenefitAttachments { get; set; }

        public IList<EMemoAttachment> OtherAttachments { get; set; }

        public IList<EMemoParticipant> Validators { get; set; }

        public IList<EMemoParticipant> Approvers { get; set; }

        public IList<EMemoApprovalWorkflowDetailResponseDetail> ApprovalWorkflowDetails { get; set; }
        public decimal? HPSAmount { get; internal set; }
    }

    public class EMemoParticipant
    {
        public Guid? Id { get; set; }

        public string PersonelUserId { get; set; }

        public string PersonelFullName { get; set; }

        public string ParticipantRole { get; set; }

        public int? Order { get; set; }

    }

    public class EMemoAttachment
    {
        public Guid? Id { get; set; }

        public string Name { get; set; }

        public int? Size { get; set; }

        public string ContentType { get; set; }

        public string Data { get; set; }

        public int? Order { get; set; }
    }

    public class EMemoFilter : BaseFilter
    {

        public string Subject { get; set; }

        public string DocumentNo { get; set; }

        public string StatusCode { get; set; }

        public string InternalRefNo { get; set; }

    }

    public class EMemoApprovalWorkflowRequest : BaseFilteredRequest
    {
        public EMemoApprovalWorkflowRequestDetail Detail { get; set; }
    }

    public class EMemoApprovalWorkflowRequestDetail
    {
        public string Id { get; set; }

        public EMemoRequestDetail EMemoRequestDetail { get; set; }

        public string ApproverUserId { get; set; }

    }

    public class EMemoApprovalWorkflowResponse : BaseFilteredRequest
    {
        public EMemoApprovalWorkflowResponseDetail Detail { get; set; }

        public IList<EMemoApprovalWorkflowResponseDetail> Details { get; set; }

    }

    public class EMemoApprovalWorkflowResponseDetail
    {
        public string Id { get; set; }

        public string EMemoId { get; set; }

        public string WorkflowStatus { get; set; }

    }

    public class EMemoApprovalFilter : BaseFilter
    {
        public EMemoApprovalFilter()
        {
            WorkUnitCodes = new List<string>();
        }

        public string Subject { get; set; }

        public string NamaPengirim { get; set; }

        public string WorkUnitCode { get; set; }

        public IList<string> WorkUnitCodes { get; set; }

        public string DateTimeFrom { get; set; }

        public string DateTimeTo { get; set; }
    }

    public class EMemoApprovalWorkflowDetailRequest : BaseFilteredRequest
    {
        public EMemoApprovalWorkflowDetailRequestDetail Detail { get; set; }

        public new EMemoApprovalWorkflowDetailFilter Filter { get; set; }

    }

    public class EMemoApprovalWorkflowDetailRequestDetail
    {
        public string Id { get; set; }

        public string EMemoId { get; set; }

        public string Subject { get; set; }

        public string DocumentNo { get; set; }

        public string WorkUnitCode { get; set; }

        public string WorkUnitName { get; set; }

        public string InternalRefNo { get; set; }

        public string ApproverUserId { get; set; }

        public string RequesterUserId { get; set; }

        public string RequesterPersonelName { get; set; }

        public string ApproverPersonelName { get; set; }

        public string ApproverPersonelPosition { get; set; }

        public string ApproverRole { get; set; }

        public string ApprovalDecission { get; set; }

        public string ApprovalNotes { get; set; }

        public DateTime? ApprovalDecissionDate { get; set; }

        public string WorkflowState { get; set; }

        public DateTime? RequestDate { get; set; }
    }

    public class EMemoApprovalWorkflowDetailResponse : BaseFilteredResponse
    {
        public EMemoApprovalWorkflowDetailResponseDetail Detail { get; set; }

        public IList<EMemoApprovalWorkflowDetailResponseDetail> Details { get; set; }

        public new EMemoApprovalWorkflowDetailFilter Filter { get; set; }

    }

    public class EMemoApprovalWorkflowDetailResponseDetail
    {

        public EMemoApprovalWorkflowDetailResponseDetail()
        {
            OtherApprovalRequests = new List<EMemoApprovalWorkflowDetailResponseDetail>();
        }

        public string Id { get; set; }

        public string EMemoId { get; set; }

        public string Subject { get; set; }

        public string DocumentNo { get; set; }

        public string WorkUnitCode { get; set; }

        public string WorkUnitName { get; set; }

        public string InternalRefNo { get; set; }

        public string ApproverUserId { get; set; }

        public string ApproverRole { get; set; }

        public string RequesterUserId { get; set; }

        public string RequesterPersonelName { get; set; }

        public string ApproverPersonelName { get; set; }

        public string ApproverPersonelPosition { get; set; }

        public string ApprovalDecission { get; set; }

        public string ApprovalNotes { get; set; }

        public DateTime? ApprovalDecissionDate { get; set; }

        public string WorkflowState { get; set; }

        public DateTime? RequestDate { get; set; }

        public IList<EMemoApprovalWorkflowDetailResponseDetail> OtherApprovalRequests { get; set; }
    }

    public class EMemoApprovalWorkflowDetailFilter : BaseFilter
    {
    }

    public class EMemoDocumentViewerRequest : BaseRequest
    { 
        public EMemoDocumentViewerRequestDetail Detail { get; set; }
    }

    public class EMemoDocumentViewerRequestDetail
    {
        public string ContentTitle { get; set; }
     
        public string ContentType { get; set; }

        public string ContentDataBase64 { get; set; }
    }

    public class EMemoTemplateRequest : BaseFilteredRequest
    {
        public EMemoTemplateRequestDetail Detail { get; set;}

        public new EMemoTemplateFilter Filter { get; set; }
    }

    public class EMemoTemplateRequestDetail
    { 
        public Guid? Id { get; set; }
    }

    public class EMemoTemplateResponse : BaseFilteredResponse
    {
        public EMemoTemplateResponseDetail Detail { get; set; }

        public List<EMemoTemplateResponseDetail> Details { get; set; }

        public new EMemoTemplateFilter Filter { get; set; }
    }

    public class EMemoTemplateResponseDetail
    {
        public Guid? Id { get; set; }

        public string Title { get; set; }

        public string ContentType { get; set; }

        public string ContentData { get; set; }

    }

    public class EMemoTemplateFilter : BaseFilter
    { 
        public string Title { get; set; }
    }




}
