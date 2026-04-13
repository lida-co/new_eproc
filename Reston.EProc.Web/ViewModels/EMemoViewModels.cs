using Reston.Eproc.Model.EMemo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reston.EProc.Web.ViewModels
{
    public class EMemoViewModels
    {
        public string Id { get; set; }
        public string Subject { get; set; }
        public string DocumentNo { get; set; }
        public string InternalRefNo { get; set; }
        public decimal? HPSAmount { get; set; }
        public string HPSCurrency { get; set; }
        public int IsDraft { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public List<EMemoAttachmentsViewModel> EMemoAttachments { get; set; }
        public List<ParticipantsViewModel> Participants { get; set; }
        public int? StatusCode { get; set; }
        public bool IsSubjectChecked { get; set; }
        public bool IsStatusChecked { get; set; }
        public bool IsNoEmemoChecked { get; set; }
        public bool IsNoRefInternalChecked { get; set; }
    }

    public class ParticipantsViewModel
    {
        public string Id { get; set; }
        public string ParticipantRole { get; set; }
        public string UserId { get; set; }
        public string EmployeeName { get; set; }
        public string Ordered { get; set; }
    }

    public class EMemoAttachmentsViewModel
    {
        public string Id { get; set; }
        public string AttachmentType { get; set; }
        public string Ordered { get; set; }
        public AttachmentsViewModel Attachment { get; set; }
    }

    public class AttachmentsViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string ContentType { get; set; }
        public string ContentData { get; set; }
    }
}
