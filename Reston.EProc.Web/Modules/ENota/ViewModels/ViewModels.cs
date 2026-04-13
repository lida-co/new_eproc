using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reston.EProc.Web.Base.ViewModels;


namespace Reston.EProc.Web.Modules.ENota.ViewModels
{
    public class ENotaRequest : BaseFilteredRequest
    {
        public ENotaRequestDetail Detail { get; set; }

        public new ENotaFilter Filter { get; set; }
    }

    public class ENotaRequestDetail
    {

        public ENotaRequestDetail()
        {
            Validators = new List<ENotaParticipant>();
            Approvers = new List<ENotaParticipant>();
            AppDireksi = new List<ENotaParticipant>();
            PenawaranAttachments = new List<ENotaAttachment>();
            CostBenefitAttachments = new List<ENotaAttachment>();
            OtherAttachments = new List<ENotaAttachment>();
            ApprovalWorkflowDetails = new List<ENotaApprovalWorkflowDetailRequestDetail>();
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

        public IList<ENotaAttachment> PenawaranAttachments { get; set; }

        public IList<ENotaAttachment> CostBenefitAttachments { get; set; }

        public IList<ENotaAttachment> OtherAttachments { get; set; }

        public IList<ENotaParticipant> Validators { get; set; }

        public IList<ENotaParticipant> Approvers { get; set; }
        public IList<ENotaParticipant> AppDireksi { get; set; }

        public IList<ENotaApprovalWorkflowDetailRequestDetail> ApprovalWorkflowDetails { get; set; }


    }

    public class ENotaResponse : BaseFilteredResponse
    {
        public ENotaResponseDetail Detail { get; set; }

        public IList<ENotaResponseDetail> Details { get; set; }

        public new ENotaFilter Filter { get; set; }

    }

    public class ENotaResponseDetail
    {

        public ENotaResponseDetail()
        {
            Validators = new List<ENotaParticipant>();
            Approvers = new List<ENotaParticipant>();
            AppDireksi = new List<ENotaParticipant>();
            PenawaranAttachments = new List<ENotaAttachment>();
            CostBenefitAttachments = new List<ENotaAttachment>();
            OtherAttachments = new List<ENotaAttachment>();
            ApprovalWorkflowDetails = new List<ENotaApprovalWorkflowDetailResponseDetail>();
        }

        public ENotaResponseDetail(ENotaRequestDetail request)
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
            Validators = new List<ENotaParticipant>(request.Validators);
            Approvers = new List<ENotaParticipant>(request.Approvers);
            AppDireksi = new List<ENotaParticipant>(request.AppDireksi);
            PenawaranAttachments = new List<ENotaAttachment>(request.PenawaranAttachments);
            CostBenefitAttachments = new List<ENotaAttachment>(request.CostBenefitAttachments);
            OtherAttachments = new List<ENotaAttachment>(request.OtherAttachments);
            ApprovalWorkflowDetails = new List<ENotaApprovalWorkflowDetailResponseDetail>();
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

        public IList<ENotaAttachment> PenawaranAttachments { get; set; }

        public IList<ENotaAttachment> CostBenefitAttachments { get; set; }

        public IList<ENotaAttachment> OtherAttachments { get; set; }

        public IList<ENotaParticipant> Validators { get; set; }

        public IList<ENotaParticipant> Approvers { get; set; }
        public IList<ENotaParticipant> AppDireksi { get; set; }

        public IList<ENotaApprovalWorkflowDetailResponseDetail> ApprovalWorkflowDetails { get; set; }

        public decimal? HPSAmount { get; internal set; }

    }

    public class ENotaParticipant
    {
        public Guid? Id { get; set; }

        public string PersonelUserId { get; set; }

        public string PersonelFullName { get; set; }

        public string ParticipantRole { get; set; }

        public int? Order { get; set; }

    }

    public class ENotaAttachment
    {
        public Guid? Id { get; set; }

        public string Name { get; set; }

        public int? Size { get; set; }

        public string ContentType { get; set; }

        public string Data { get; set; }

        public int? Order { get; set; }
    }

    public class ENotaFilter : BaseFilter
    {

        public string Subject { get; set; }

        public string DocumentNo { get; set; }

        public string StatusCode { get; set; }

        public string InternalRefNo { get; set; }

    }

    public class ENotaApprovalWorkflowRequest : BaseFilteredRequest
    {
        public ENotaApprovalWorkflowRequestDetail Detail { get; set; }
    }

    public class ENotaApprovalWorkflowRequestDetail
    {
        public string Id { get; set; }

        public ENotaRequestDetail ENotaRequestDetail { get; set; }

        public string ApproverUserId { get; set; }

    }

    public class ENotaApprovalWorkflowResponse : BaseFilteredRequest
    {
        public ENotaApprovalWorkflowResponseDetail Detail { get; set; }

        public IList<ENotaApprovalWorkflowResponseDetail> Details { get; set; }

    }

    public class ENotaApprovalWorkflowResponseDetail
    {
        public string Id { get; set; }

        public string ENotaId { get; set; }

        public string WorkflowStatus { get; set; }

    }

    public class ENotaApprovalFilter : BaseFilter
    {
        public ENotaApprovalFilter()
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

    public class ENotaApprovalWorkflowDetailRequest : BaseFilteredRequest
    {
        public ENotaApprovalWorkflowDetailRequestDetail Detail { get; set; }

        public new ENotaApprovalWorkflowDetailFilter Filter { get; set; }

    }

    public class ENotaApprovalWorkflowDetailRequestDetail
    {
        public string Id { get; set; }

        public string ENotaId { get; set; }

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

    public class ENotaApprovalWorkflowDetailResponse : BaseFilteredResponse
    {
        public ENotaApprovalWorkflowDetailResponseDetail Detail { get; set; }

        public IList<ENotaApprovalWorkflowDetailResponseDetail> Details { get; set; }

        public new ENotaApprovalWorkflowDetailFilter Filter { get; set; }

    }

    public class ENotaApprovalWorkflowDetailResponseDetail
    {

        public ENotaApprovalWorkflowDetailResponseDetail()
        {
            OtherApprovalRequests = new List<ENotaApprovalWorkflowDetailResponseDetail>();
        }

        public string Id { get; set; }

        public string ENotaId { get; set; }

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

        public IList<ENotaApprovalWorkflowDetailResponseDetail> OtherApprovalRequests { get; set; }
    }

    public class ENotaApprovalWorkflowDetailFilter : BaseFilter
    {
    }

    public class ENotaDocumentViewerRequest : BaseRequest
    { 
        public ENotaDocumentViewerRequestDetail Detail { get; set; }
    }

    public class ENotaDocumentViewerRequestDetail
    {
        public string ContentTitle { get; set; }
     
        public string ContentType { get; set; }

        public string ContentDataBase64 { get; set; }
    }

    public class ENotaTemplateRequest : BaseFilteredRequest
    {
        public ENotaTemplateRequestDetail Detail { get; set;}

        public new ENotaTemplateFilter Filter { get; set; }
    }

    public class ENotaTemplateRequestDetail
    { 
        public Guid? Id { get; set; }
    }

    public class ENotaTemplateResponse : BaseFilteredResponse
    {
        public ENotaTemplateResponseDetail Detail { get; set; }

        public List<ENotaTemplateResponseDetail> Details { get; set; }

        public new ENotaTemplateFilter Filter { get; set; }
    }

    public class ENotaTemplateResponseDetail
    {
        public Guid? Id { get; set; }

        public string Title { get; set; }

        public string ContentType { get; set; }

        public string ContentData { get; set; }

    }

    public class ENotaTemplateFilter : BaseFilter
    { 
        public string Title { get; set; }
    }




}
