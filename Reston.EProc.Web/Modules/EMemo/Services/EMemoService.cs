using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Helper;
using Reston.Pinata.Model;
using Reston.Eproc.Model.EMemo;
using Reston.EProc.Web.Base.ViewModels;
using Reston.EProc.Web.Modules.EMemo.ViewModels;
using NLog;
using Reston.Eproc.Model.Monitoring.Entities;
using DocumentFormat.OpenXml.Vml.Office;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using Reston.Pinata.Model.PengadaanRepository;
using System.IdentityModel.Protocols.WSTrust;

namespace Reston.EProc.Web.Modules.EMemo.Services
{
    public class EMemoService
    {
        private const string EMEMO_DOC_NO_TEMPLATE = "{SEQNO}/MEMO-OPR/MTF/{WORK_UNIT_CODE}/{MONTH_2_DIGITS}/{YEAR_4_DIGITS}";

        private static Logger _log = LogManager.GetCurrentClassLogger();

        protected AppDbContext db;

        public EMemoService(AppDbContext dbContext)
        {
            db = dbContext;
        }

        public EMemoResponse Search(EMemoRequest request, Guid documentOwnerId)
        {

            var response = new EMemoResponse();

            var filterStatus = "";
            /*var query = db.EMemos
           .Include("Participants")
            .Where(e => e.Owner == documentOwnerId.ToString());
             */
            /*var query = db.EMemos
              //Include("EMemoAttachments")
              //.Include("EMemoAttachments.Attachments")
              .Include("Participants")
              .GroupJoin(db.ApprovalWorkflows
                  , eMemo => eMemo.Id
                  , wfHeader => wfHeader.EMemoId
                  , (eMemo, wfHeader) => new {
                      Id = eMemo.Id,
                      Subject = eMemo.Subject,
                      DocumentNo = eMemo.DocumentNo,
                      WorkUnitCode = eMemo.WorkUnitCode,
                      InternalRefNo = eMemo.InternalRefNo,
                      HPSAmount = eMemo.HPSAmount,
                      HPSCurrency = eMemo.HPSCurrency,
                      IsDraft = eMemo.IsDraft,
                      Owner = eMemo.Owner,
                      OwnerPersonelName = eMemo.OwnerPersonelName,
                      CreatedDate = eMemo.CreatedDate,
                      CreatedBy = eMemo.CreatedBy,
                      ModifiedDate = eMemo.ModifiedDate,
                      ModifiedBy = eMemo.ModifiedBy,
                      WFHeader = wfHeader.DefaultIfEmpty(),
                  })
              .Where(e => e.Owner == documentOwnerId.ToString());*/

            var reqHeader = request.Header;

            Guid userGuid = Guid.Parse(reqHeader.UserId);

            var query = db.EMemos
                .Where(eMemo =>
                    eMemo.Owner == reqHeader.UserId ||
                    eMemo.Participants.Any(p => p.UserId == reqHeader.UserId) ||
                    db.ApprovalWorkflowDetails.Any(wfd =>
                        db.ApprovalWorkflows
                            .Where(wfh => wfh.EMemoId == eMemo.Id)
                            .Select(wfh => wfh.Id)
                            .Contains(wfd.ApprovalWorkflowId)
                        && wfd.ParticipantUserId == userGuid
                    )
                )
                .Select(eMemo => new
                {
                    Id = eMemo.Id,
                    Subject = eMemo.Subject,
                    DocumentNo = eMemo.DocumentNo,
                    WorkUnitCode = eMemo.WorkUnitCode,
                    InternalRefNo = eMemo.InternalRefNo,
                    HPSAmount = eMemo.HPSAmount,
                    HPSCurrency = eMemo.HPSCurrency,
                    IsDraft = eMemo.IsDraft,
                    Owner = eMemo.Owner,
                    OwnerPersonelName = eMemo.OwnerPersonelName,
                    CreatedDate = eMemo.CreatedDate,
                    CreatedBy = eMemo.CreatedBy,
                    ModifiedDate = eMemo.ModifiedDate,
                    ModifiedBy = eMemo.ModifiedBy
                });


            //System.Diagnostics.Debug.WriteLine("aaaaaaa");






            /* var query = from eMemo in db.EMemos
                           join ptcp in db.Participants on eMemo.Id equals ptcp.EMemoId
                           where eMemo.Owner == documentOwnerId.ToString()
                           select eMemo;
               from wfHeader in db.ApprovalWorkflows.DefaultIfEmpty()
               select new
               {
                   Id = eMemo.Id,
                   Subject = eMemo.Subject,
                   DocumentNo = eMemo.DocumentNo,
                   WorkUnitCode = eMemo.WorkUnitCode,
                   InternalRefNo = eMemo.InternalRefNo,
                   HPSAmount = eMemo.HPSAmount,
                   HPSCurrency = eMemo.HPSCurrency,
                   IsDraft = eMemo.IsDraft,
                   Owner = eMemo.Owner,
                   OwnerPersonelName = eMemo.OwnerPersonelName,
                   CreatedDate = eMemo.CreatedDate,
                   CreatedBy = eMemo.CreatedBy,
                   ModifiedDate = eMemo.ModifiedDate,
                   ModifiedBy = eMemo.ModifiedBy,
                   ApprovalWorkflowStatus = (wfHeader == null ? null : wfHeader.WorkflowState)
               };*/


            if (request.Filter != null)
            {
                var filter = request.Filter;

                filter.TotalRecordsCount = query.Count();

                // Filter by Subject
                if (!string.IsNullOrEmpty(filter.Subject))
                {
                    query = query.Where(e => e.Subject.Contains(filter.Subject));
                }

                // Filter by Document No
                if (!string.IsNullOrEmpty(filter.DocumentNo))
                {
                    query = query.Where(e => e.DocumentNo.Contains(filter.DocumentNo));
                }

                // Filter by Internal Reff No
                if (!string.IsNullOrEmpty(filter.InternalRefNo))
                {
                    query = query.Where(e => e.InternalRefNo.Contains(filter.InternalRefNo));
                }

                // Filter by Status
                if (!string.IsNullOrEmpty(filter.StatusCode))
                {
                    // right now only draft filter is working
                    if (filter.StatusCode.Equals("0"))
                    {
                        query = query.Where(e => e.IsDraft == 1);
                    }
                    else
                    {
                        query = query.Where(e => e.IsDraft != 1);
                    }
                }

                filter.TotalRecordsCount = query.Count();

                // Handle paging
                /*
                if (filter.Offset.HasValue) 
                {
                    query.Skip(filter.Offset.Value);
                }

                if (filter.PageLength.HasValue)
                {
                    query.Take(filter.PageLength.Value);
                }
                */

                request.Filter.FilteredRecordsCount = query.Count();
            }

            var result = query.Select(e => new EMemoResponseDetail()
            {
                Id = e.Id,
                Subject = e.Subject,
                DocumentNo = e.DocumentNo,
                InternalRefNo = e.InternalRefNo,
                IsDraft = e.IsDraft == 1,
                //WorkflowStatus = e.ApprovalWorkflowStatus.HasValue ? ((int)e.ApprovalWorkflowStatus.Value).ToString() : "",
                WorkflowStatus = ((int)(from wfHeader
                                  in db.ApprovalWorkflows
                                        where wfHeader.EMemoId == e.Id
                                        select wfHeader.WorkflowState)
                                  .FirstOrDefault()
                                  .Value).ToString()

            }).ToList();

            response.Header = request.Header?.Clone() as BaseHeader;
            response.Header.StatusCode = StatusCode.SUCCESS;
            response.Details = result;
            response.Filter = request.Filter;

            return response;

        }

        public EMemoResponse Get(EMemoRequest request, Guid documentOwnerUserId)
        {
            var requestDetail = request.Detail;
            var response = new EMemoResponse();
            var responseDetail = new EMemoResponseDetail();
            var docGuid = requestDetail.Id;


            var eMemoEntity = db.EMemos
                .Include("EMemoAttachments")
                .Include("EMemoAttachments.Attachment")
                .Include("Participants")
                .Where(e => e.Id == docGuid)
                .FirstOrDefault();

            if (eMemoEntity == null)
            {
                response.Header = request.Header.Clone() as BaseHeader;
                response.Header.StatusCode = StatusCode.NOT_FOUND;
                response.Header.StatusMessage = "Could not find entity with Id = " + docGuid;
                return response;
            }

            responseDetail.Id = eMemoEntity.Id;
            responseDetail.DocumentNo = eMemoEntity.DocumentNo;
            responseDetail.WorkUnitCode = eMemoEntity.WorkUnitCode;
            responseDetail.WorkUnitName = db.ReferenceDatas
                                    .Where(e => e.Qualifier == RefDataQualifier.UnitKerja
                                        && e.Code == eMemoEntity.WorkUnitCode)
                                    .Select(e => e.LocalizedName)
                                    .FirstOrDefault();
            responseDetail.InternalRefNo = eMemoEntity.InternalRefNo;
            responseDetail.Subject = eMemoEntity.Subject;
            responseDetail.Kepada = eMemoEntity.Kepada;
            responseDetail.Tembusan = eMemoEntity.Tembusan;
            responseDetail.HPSAmount = eMemoEntity.HPSAmount;
            responseDetail.IsDraft = eMemoEntity.IsDraft == 1;
            responseDetail.OwnerUserId = eMemoEntity.Owner;
            // TODO consider persisting ContentInputMode
            // for now we simply deduce ContentInputMode
            // from ContentType
            responseDetail.ContentInputMode = responseDetail.ContentType == "text/html" ? "edit" : "upload";

            // Map participants to view model
            eMemoEntity.Participants.ToList().ForEach((e) => {

                var ptcp = new EMemoParticipant()
                {
                    Id = e.Id,
                    PersonelUserId = e.UserId,
                    PersonelFullName = e.EmployeeName,
                    ParticipantRole = e.ParticipantRole.HasValue ? ((int)e.ParticipantRole.Value).ToString() : "",
                    Order = e.Ordered,
                };

                switch (e.ParticipantRole)
                {
                    case ParticipantRole.EMEMO_PTCP_APPROVER:
                        responseDetail.Approvers.Add(ptcp);
                        break;
                    case ParticipantRole.EMEMO_PTCP_REVIEWER:
                        responseDetail.Validators.Add(ptcp);
                        break;
                }

            });

            // Map Attachments to view model
            eMemoEntity.EMemoAttachments.ToList().ForEach((e) => {

                var attcm = new ViewModels.EMemoAttachment()
                {
                    Id = e.Attachment.Id,
                    Name = e.Attachment.Title,
                    ContentType = e.Attachment.ContentType,
                    Data = Convert.ToBase64String(e.Attachment.ContentData),
                    Size = e.Attachment.ContentData.Length,
                    Order = e.Ordered,
                };

                switch (e.AttachmentType)
                {
                    case AttachmentType.EMEMO_ATT_MEMO:
                        responseDetail.ContentType = e.Attachment.ContentType;
                        responseDetail.ContentData = Convert.ToBase64String(e.Attachment.ContentData);
                        break;
                    case AttachmentType.EMEMO_ATT_QUOTATION:
                        responseDetail.PenawaranAttachments.Add(attcm);
                        break;
                    case AttachmentType.EMEMO_ATT_COSTBENEFIT:
                        responseDetail.CostBenefitAttachments.Add(attcm);
                        break;
                    case AttachmentType.EMEMO_ATT_OTHER:
                        responseDetail.OtherAttachments.Add(attcm);
                        break;

                }

            });

            // Map approval workflow details to view model
            responseDetail.ApprovalWorkflowDetails = db.ApprovalWorkflowDetails
                .Include("ApprovalWorkflow")
                .Where(e => e.ApprovalWorkflow.EMemoId == eMemoEntity.Id)
                //.OrderBy(e => e.ParticipantRole)
                .Select(e => new EMemoApprovalWorkflowDetailResponseDetail()
                {
                    Id = e.Id.ToString(),
                    EMemoId = e.ApprovalWorkflow.EMemoId.ToString(),
                    ApproverPersonelName = e.ParticipantName,
                    ApproverUserId = e.ParticipantUserId.ToString(),
                    ApprovalDecission = e.ApprovalDecission.HasValue ? ((int)e.ApprovalDecission.Value).ToString() : "",
                    ApproverPersonelPosition = "", // TODO 
                    ApprovalNotes = e.ApprovalNotes,
                    ApprovalDecissionDate = e.ApprovalDecissionDate,
                    ApproverRole = e.ParticipantRole.HasValue ? ((int)e.ParticipantRole.Value).ToString() : ""
                })
                .ToList();

            response.Detail = responseDetail;
            response.Header = request.Header.Clone() as BaseHeader;
            response.Header.StatusCode = StatusCode.SUCCESS;

            return response;
        }

        public EMemoResponse Create(EMemoRequest request)
        {
            var response = new EMemoResponse();
            var now = DateTime.Now;

            if (request == null || request.Header == null || request.Detail == null)
                throw new Exception("Request tidak valid");

            var reqHeader = request.Header;
            var reqDetail = request.Detail;
            var userId = reqHeader.UserId;

            reqDetail.ContentTitle = reqDetail.ContentTitle?.Trim();
            reqDetail.InternalRefNo = reqDetail.InternalRefNo?.Trim();
            reqDetail.Subject = reqDetail.Subject?.Trim();
            reqDetail.Kepada = reqDetail.Kepada?.Trim();
            reqDetail.Tembusan = reqDetail.Tembusan?.Trim();

            if (reqDetail.PenawaranAttachments != null)
            {
                foreach (var item in reqDetail.PenawaranAttachments)
                    item.Name = item.Name?.Trim();
            }

            if (reqDetail.CostBenefitAttachments != null)
            {
                foreach (var item in reqDetail.CostBenefitAttachments)
                    item.Name = item.Name?.Trim();
            }

            if (reqDetail.OtherAttachments != null)
            {
                foreach (var item in reqDetail.OtherAttachments)
                    item.Name = item.Name?.Trim();
            }

            var eMemoEntity = new Reston.Eproc.Model.EMemo.EMemo()
            {
                Id = Guid.NewGuid(),
                DocumentNo = GenerateDocumentNumber(request),
                WorkUnitCode = reqDetail.WorkUnitCode,
                InternalRefNo = reqDetail.InternalRefNo,
                Subject = reqDetail.Subject,
                Kepada = reqDetail.Kepada,
                Tembusan = reqDetail.Tembusan,
                HPSAmount = reqDetail.HPSAmount,
                Owner = userId,
                IsDraft = 1,
                Participants = new List<Reston.Eproc.Model.EMemo.Participant>(),
                EMemoAttachments = new List<Reston.Eproc.Model.EMemo.EMemoAttachment>(),
                EMemoLogs = new List<Reston.Eproc.Model.EMemo.EMemoLogs>()
            };

            var order = 0;

            if (reqDetail.Validators != null)
            {
                foreach (var item in reqDetail.Validators)
                {
                    var entity = new Reston.Eproc.Model.EMemo.Participant()
                    {
                        Id = Guid.NewGuid(),
                        EMemoId = eMemoEntity.Id,
                        EMemo = eMemoEntity,
                        UserId = item.PersonelUserId,
                        EmployeeName = item.PersonelFullName,
                        ParticipantRole = ParticipantRole.EMEMO_PTCP_REVIEWER,
                        Ordered = order++,
                    };
                    eMemoEntity.Participants.Add(entity);
                }
            }

            order = 0;
            if (reqDetail.Approvers != null)
            {
                foreach (var item in reqDetail.Approvers)
                {
                    var entity = new Reston.Eproc.Model.EMemo.Participant()
                    {
                        Id = Guid.NewGuid(),
                        EMemoId = eMemoEntity.Id,
                        EMemo = eMemoEntity,
                        UserId = item.PersonelUserId,
                        EmployeeName = item.PersonelFullName,
                        ParticipantRole = ParticipantRole.EMEMO_PTCP_APPROVER,
                        Ordered = order++,
                    };
                    eMemoEntity.Participants.Add(entity);
                }
            }

            byte[] mainContent = null;
            try
            {
                mainContent = string.IsNullOrEmpty(reqDetail.ContentData)
                    ? null
                    : Convert.FromBase64String(reqDetail.ContentData);
            }
            catch
            {
                throw new Exception("ContentData tidak valid (base64)");
            }

            var attachmentEntity = new Reston.Eproc.Model.EMemo.Attachment()
            {
                Id = Guid.NewGuid(),
                ContentType = reqDetail.ContentType,
                ContentData = mainContent,
                Title = reqDetail.ContentTitle,
            };

            var eMemoAttachmentEntity = new Eproc.Model.EMemo.EMemoAttachment()
            {
                Id = Guid.NewGuid(),
                AttachmentId = attachmentEntity.Id,
                Attachment = attachmentEntity,
                EMemoId = eMemoEntity.Id,
                EMemo = eMemoEntity,
                AttachmentType = AttachmentType.EMEMO_ATT_MEMO,
                Ordered = 0
            };

            eMemoEntity.EMemoAttachments.Add(eMemoAttachmentEntity);

            if (mainContent != null)
                reqDetail.ContentSize = mainContent.Length;

            int orderPenawaran = 0;
            string AttactmentPenawaran = "";
            string AttactmentPenawaranType = "";
            byte[] AttactmentPenawaranFiles = null;

            if (reqDetail.PenawaranAttachments != null)
            {
                foreach (var item in reqDetail.PenawaranAttachments)
                {
                    byte[] fileData = null;
                    try { fileData = Convert.FromBase64String(item.Data); } catch { }

                    AttactmentPenawaran = item.Name;
                    AttactmentPenawaranType = item.ContentType;
                    AttactmentPenawaranFiles = fileData;

                    var att = new Reston.Eproc.Model.EMemo.Attachment()
                    {
                        Id = Guid.NewGuid(),
                        ContentType = item.ContentType,
                        ContentData = fileData,
                        Title = item.Name,
                    };

                    var em = new Eproc.Model.EMemo.EMemoAttachment()
                    {
                        Id = Guid.NewGuid(),
                        AttachmentId = att.Id,
                        Attachment = att,
                        EMemoId = eMemoEntity.Id,
                        EMemo = eMemoEntity,
                        AttachmentType = AttachmentType.EMEMO_ATT_QUOTATION,
                        Ordered = orderPenawaran++
                    };

                    eMemoEntity.EMemoAttachments.Add(em);
                }
            }

            int orderAnalisa = 0;
            string AttachtmentAnalisa = "";
            string AttachtmentAnalisaType = "";
            byte[] AttachtmentAnalisaFile = null;

            if (reqDetail.CostBenefitAttachments != null)
            {
                foreach (var item in reqDetail.CostBenefitAttachments)
                {
                    byte[] fileData = null;
                    try { fileData = Convert.FromBase64String(item.Data); } catch { }

                    AttachtmentAnalisa = item.Name;
                    AttachtmentAnalisaType = item.ContentType;
                    AttachtmentAnalisaFile = fileData;

                    var att = new Reston.Eproc.Model.EMemo.Attachment()
                    {
                        Id = Guid.NewGuid(),
                        ContentType = item.ContentType,
                        ContentData = fileData,
                        Title = item.Name,
                    };

                    var em = new Eproc.Model.EMemo.EMemoAttachment()
                    {
                        Id = Guid.NewGuid(),
                        AttachmentId = att.Id,
                        Attachment = att,
                        EMemoId = eMemoEntity.Id,
                        EMemo = eMemoEntity,
                        AttachmentType = AttachmentType.EMEMO_ATT_COSTBENEFIT,
                        Ordered = orderAnalisa++
                    };

                    eMemoEntity.EMemoAttachments.Add(em);
                }
            }

            int orderOther = 0;
            string AttachmentLampiran = "";
            string AttachmentLampiranType = "";
            byte[] AttachmentLampiranFile = null;

            if (reqDetail.OtherAttachments != null)
            {
                foreach (var item in reqDetail.OtherAttachments)
                {
                    byte[] fileData = null;
                    try { fileData = Convert.FromBase64String(item.Data); } catch { }

                    AttachmentLampiran = item.Name;
                    AttachmentLampiranType = item.ContentType;
                    AttachmentLampiranFile = fileData;

                    var att = new Reston.Eproc.Model.EMemo.Attachment()
                    {
                        Id = Guid.NewGuid(),
                        ContentType = item.ContentType,
                        ContentData = fileData,
                        Title = item.Name,
                    };

                    var em = new Eproc.Model.EMemo.EMemoAttachment()
                    {
                        Id = Guid.NewGuid(),
                        AttachmentId = att.Id,
                        Attachment = att,
                        EMemoId = eMemoEntity.Id,
                        EMemo = eMemoEntity,
                        AttachmentType = AttachmentType.EMEMO_ATT_OTHER,
                        Ordered = orderOther++
                    };

                    eMemoEntity.EMemoAttachments.Add(em);
                }
            }

            var logEntity = new Reston.Eproc.Model.EMemo.EMemoLogs
            {
                Id = Guid.NewGuid(),
                EMemoId = eMemoEntity.Id,
                Version = 1,
                Content = "CREATE NEW",
                CreateDate = now,
                Status = "DRAFT",
                AttactmentPenawaran = AttactmentPenawaran,
                AttactmentPenawaranType = AttactmentPenawaranType,
                AttactmentPenawaranFiles = AttactmentPenawaranFiles,
                AttachtmentAnalisa = AttachtmentAnalisa,
                AttachtmentAnalisaType = AttachtmentAnalisaType,
                AttachtmentAnalisaFile = AttachtmentAnalisaFile,
                AttachmentLampiran = AttachmentLampiran,
                AttachmentLampiranType = AttachmentLampiranType,
                AttachmentLampiranFile = AttachmentLampiranFile
            };

            db.EMemoLogs.Add(logEntity);
            db.EMemos.Add(eMemoEntity);
            db.SaveChanges(userId);

            response.Header = request.Header.Clone() as BaseHeader;
            response.Header.StatusCode = StatusCode.SUCCESS;

            response.Detail = new EMemoResponseDetail(reqDetail);
            response.Detail.Id = eMemoEntity.Id;
            response.Detail.DocumentNo = eMemoEntity.DocumentNo;

            return response;
        }

        public EMemoResponse Update(EMemoRequest request)
        {
            var response = new EMemoResponse();
            var now = DateTime.Now;
            var reqHeader = request.Header;
            var reqDetail = request.Detail;
            var userId = reqHeader.UserId;

            var eMemoEntity = db.EMemos
                .Include("Participants")
                .Where(e => e.Id == reqDetail.Id)
                .FirstOrDefault();

            if (eMemoEntity == null)
            {
                response.Header = request.Header.Clone() as BaseHeader;
                response.Header.StatusCode = StatusCode.NOT_FOUND;
                response.Header.StatusMessage = "Could not find entity with Id = " + reqDetail.Id;
                return response;
            }

            // EMemo entity
            eMemoEntity.DocumentNo = reqDetail.DocumentNo;
            eMemoEntity.WorkUnitCode = reqDetail.WorkUnitCode;
            eMemoEntity.InternalRefNo = reqDetail.InternalRefNo;
            eMemoEntity.Subject = reqDetail.Subject;
            eMemoEntity.Kepada = reqDetail.Kepada;
            eMemoEntity.Tembusan = reqDetail.Tembusan;
            eMemoEntity.HPSAmount = reqDetail.HPSAmount;
            eMemoEntity.Owner = userId;
            eMemoEntity.ModifiedBy = userId.ToString();
            eMemoEntity.ModifiedDate = now;

            // drop existing participants before updating
            var participants = db.Participants
                .Where(e => e.EMemoId == eMemoEntity.Id)
                .ToList();
            db.Participants.RemoveRange(participants);

            // Participants entitites Reviewers
            var order = 0;
            foreach (var item in reqDetail.Validators)
            {
                var eMemoParticipantEntity = new Reston.Eproc.Model.EMemo.Participant()
                {
                    Id = Guid.NewGuid(),
                    EMemoId = eMemoEntity.Id,
                    EMemo = eMemoEntity,
                    UserId = item.PersonelUserId,
                    EmployeeName = item.PersonelFullName,
                    ParticipantRole = ParticipantRole.EMEMO_PTCP_REVIEWER,
                    Ordered = order,
                };
                eMemoEntity.Participants.Add(eMemoParticipantEntity);
                order++;
            }

            // Participants entitites Approvers
            order = 0;
            foreach (var item in reqDetail.Approvers)
            {
                var eMemoParticipantEntity = new Reston.Eproc.Model.EMemo.Participant()
                {
                    Id = Guid.NewGuid(),
                    EMemoId = eMemoEntity.Id,
                    EMemo = eMemoEntity,
                    UserId = item.PersonelUserId,
                    EmployeeName = item.PersonelFullName,
                    ParticipantRole = ParticipantRole.EMEMO_PTCP_APPROVER,
                    Ordered = order,
                };
                eMemoEntity.Participants.Add(eMemoParticipantEntity);
                order++;
            }

            // drop existing attachments before updating
            var eMemoAttachments = db.EMemoAttachments
                .Include("Attachment")
                .Where(e => e.EMemoId == eMemoEntity.Id)
                .ToList();
            var attachments = eMemoAttachments.Select(e => e.Attachment).ToList();
            db.EMemoAttachments.RemoveRange(eMemoAttachments);
            db.Attachments.RemoveRange(attachments);


            // EMemoAttachments main ememo document 
            order = 0;
            // there's only single main ememo document
            var attachmentEntity = new Reston.Eproc.Model.EMemo.Attachment()
            {
                Id = Guid.NewGuid(),
                ContentType = reqDetail.ContentType,
                ContentData = Convert.FromBase64String(reqDetail.ContentData),
                Title = reqDetail.Subject,
            };
            var eMemoAttachmentEntity = new Eproc.Model.EMemo.EMemoAttachment()
            {
                Id = Guid.NewGuid(),
                AttachmentId = attachmentEntity.Id,
                Attachment = attachmentEntity,
                EMemoId = eMemoEntity.Id,
                EMemo = eMemoEntity,
                AttachmentType = AttachmentType.EMEMO_ATT_MEMO,
                Ordered = order
            };
            eMemoEntity.EMemoAttachments.Add(eMemoAttachmentEntity);

            // EMemoAttachments quotation/penawaran documents 
            order = 0;
            string AttactmentPenawaran = "";
            string AttactmentPenawaranType = "";
            byte[] AttactmentPenawaranFiles = null;
            foreach (var item in reqDetail.PenawaranAttachments)
            {
                AttactmentPenawaran = item.Name;
                AttactmentPenawaranType = item.ContentType;
                AttactmentPenawaranFiles = Convert.FromBase64String(item.Data);
                attachmentEntity = new Reston.Eproc.Model.EMemo.Attachment()
                {
                    Id = Guid.NewGuid(),
                    ContentType = item.ContentType,
                    ContentData = Convert.FromBase64String(item.Data),
                    Title = item.Name,
                };

                eMemoAttachmentEntity = new Eproc.Model.EMemo.EMemoAttachment()
                {
                    Id = Guid.NewGuid(),
                    AttachmentId = attachmentEntity.Id,
                    Attachment = attachmentEntity,
                    EMemoId = eMemoEntity.Id,
                    EMemo = eMemoEntity,
                    AttachmentType = AttachmentType.EMEMO_ATT_QUOTATION,
                    Ordered = order
                };

                eMemoEntity.EMemoAttachments.Add(eMemoAttachmentEntity);
                order++;
            }

            // EMemoAttachments cost benef analysis documents
            order = 0;
            string AttachtmentAnalisa = "";
            string AttachtmentAnalisaType = "";
            byte[] AttachtmentAnalisaFile = null;
            foreach (var item in reqDetail.CostBenefitAttachments)
            {
                AttachtmentAnalisa = item.Name;
                AttachtmentAnalisaType = item.ContentType;
                AttachtmentAnalisaFile = Convert.FromBase64String(item.Data);
                attachmentEntity = new Reston.Eproc.Model.EMemo.Attachment()
                {
                    Id = Guid.NewGuid(),
                    ContentType = item.ContentType,
                    ContentData = Convert.FromBase64String(item.Data),
                    Title = item.Name,
                };

                eMemoAttachmentEntity = new Eproc.Model.EMemo.EMemoAttachment()
                {
                    Id = Guid.NewGuid(),
                    AttachmentId = attachmentEntity.Id,
                    Attachment = attachmentEntity,
                    EMemoId = eMemoEntity.Id,
                    EMemo = eMemoEntity,
                    AttachmentType = AttachmentType.EMEMO_ATT_COSTBENEFIT,
                    Ordered = order
                };

                eMemoEntity.EMemoAttachments.Add(eMemoAttachmentEntity);
                order++;
            }

            // EMemoAttachments other documents
            order = 0;
            string AttachmentLampiran = "";
            string AttachmentLampiranType = "";
            byte[] AttachmentLampiranFile = null;
            foreach (var item in reqDetail.OtherAttachments)
            {
                AttachmentLampiran = item.Name;
                AttachmentLampiranType = item.ContentType;
                AttachmentLampiranFile = Convert.FromBase64String(item.Data);
                attachmentEntity = new Reston.Eproc.Model.EMemo.Attachment()
                {
                    Id = Guid.NewGuid(),
                    ContentType = item.ContentType,
                    ContentData = Convert.FromBase64String(item.Data),
                    Title = item.Name,
                };

                eMemoAttachmentEntity = new Eproc.Model.EMemo.EMemoAttachment()
                {
                    Id = Guid.NewGuid(),
                    AttachmentId = attachmentEntity.Id,
                    Attachment = attachmentEntity,
                    EMemoId = eMemoEntity.Id,
                    EMemo = eMemoEntity,
                    AttachmentType = AttachmentType.EMEMO_ATT_OTHER,
                    Ordered = order
                };

                eMemoEntity.EMemoAttachments.Add(eMemoAttachmentEntity);
                order++;
            }

            var logEntity = new Reston.Eproc.Model.EMemo.EMemoLogs
            {
                Id = Guid.NewGuid(),
                EMemoId = eMemoEntity.Id, // pick the correct FK
                Version = 1,                                   // first version
                Content = "UPDATE ", // compact JSON snapshot
                CreateDate = now,
                AttactmentPenawaran = AttactmentPenawaran,
                AttactmentPenawaranType = AttactmentPenawaranType,
                AttactmentPenawaranFiles = AttactmentPenawaranFiles,
                AttachtmentAnalisa = AttachtmentAnalisa,
                AttachtmentAnalisaType = AttachtmentAnalisaType,
                AttachtmentAnalisaFile = AttachtmentAnalisaFile,
                AttachmentLampiran = AttachmentLampiran,
                AttachmentLampiranType = AttachmentLampiranType,
                AttachmentLampiranFile = AttachmentLampiranFile

            };
            db.EMemoLogs.Add(logEntity);            // track the log row

            db.SaveChanges(userId);



            response.Header = request.Header.Clone() as BaseHeader;
            response.Header.StatusCode = StatusCode.SUCCESS;
            response.Detail = new EMemoResponseDetail(reqDetail);
            response.Detail.Id = eMemoEntity.Id;
            response.Detail.DocumentNo = eMemoEntity.DocumentNo;

            return response;
        }

        public EMemoApprovalWorkflowResponse CreateApprovalWorkflow(EMemoApprovalWorkflowRequest request)
        {
            EMemoApprovalWorkflowResponse response = null;
            var reqDetail = request?.Detail;

            // This task involve two steps:
            // 1. Create or Update the document
            // 2. Create the approval workflow and it's details

            // We need to wrap these steps in a single transaction
            using (var tx = db.Database.BeginTransaction())
            {
                try
                {
                    //
                    // 1. Create or Update the document
                    //
                    var eMemoRequest = new EMemoRequest()
                    {
                        Header = request.Header.Clone() as BaseHeader,
                        Detail = reqDetail.EMemoRequestDetail
                    };
                    EMemoResponse eMemoResponse = null;
                    if (eMemoRequest.Detail.Id == null)
                    {
                        // if the request does not carry an entity Id,
                        // treat it as 'Create' request
                        eMemoResponse = Create(eMemoRequest);
                    }
                    else
                    {
                        // otherwise, treat it as 'Update' request
                        eMemoResponse = Update(eMemoRequest);
                    }

                    //
                    // 2. Create the approval workflow and it's details
                    //
                    if (eMemoResponse.Header?.StatusCode != StatusCode.SUCCESS)
                    {
                        // if previous operations was not sucessfull, 
                        // then stop. There is no point starting a workflow 
                        // on an invalid or nonexistent document
                        response = new EMemoApprovalWorkflowResponse()
                        {
                            Header = eMemoResponse.Header?.Clone() as BaseHeader,
                        };
                        return response;
                    }

                    var eMemoDoc = db.EMemos.Where(e => e.Id == eMemoResponse.Detail.Id).FirstOrDefault();
                    if (eMemoDoc.IsDraft == 1)
                    {
                        var wfHeader = new ApprovalWorkflow()
                        {
                            Id = Guid.NewGuid(),
                            EMemoId = eMemoDoc.Id,
                            WorkflowState = ApprovalWorkflowState.BEGIN,
                            ApprovalWorkflowDetails = new List<ApprovalWorkflowDetail>(),
                        };

                        foreach (var personel in eMemoDoc.Participants)
                        {
                            var wfDetail = new ApprovalWorkflowDetail()
                            {
                                Id = Guid.NewGuid(),
                                ApprovalWorkflowId = wfHeader.Id,
                                ParticipantId = personel.Id,
                                ParticipantUserId = Guid.Parse(personel.UserId),
                                ParticipantName = personel.EmployeeName,
                                ParticipantRole = personel.ParticipantRole,
                                ApprovalDecission = null,
                                ApprovalNotes = null,
                            };
                            wfHeader.ApprovalWorkflowDetails.Add(wfDetail);
                        }

                        db.ApprovalWorkflows.Add(wfHeader);

                        eMemoDoc.IsDraft = 0;

                        db.SaveChanges(request.Header.UserId);
                        tx.Commit();

                        response = new EMemoApprovalWorkflowResponse()
                        {
                            Header = request.Header?.Clone() as BaseHeader,
                            Detail = new EMemoApprovalWorkflowResponseDetail()
                            {
                                Id = wfHeader.Id.ToString(),
                                EMemoId = eMemoDoc.Id.ToString(),
                                WorkflowStatus = wfHeader.WorkflowState.ToString(),
                            }
                        };
                        response.Header.StatusCode = StatusCode.SUCCESS;
                    }
                    else
                    {

                        var eMemoEntity = db.ApprovalWorkflows
                        .Where(e => e.EMemoId == eMemoResponse.Detail.Id)
                        .FirstOrDefault();

                        var wfHeader = new ApprovalWorkflow()
                        {
                            Id = eMemoEntity.Id,
                            EMemoId = eMemoDoc.Id,
                            WorkflowState = ApprovalWorkflowState.VALIDATORS_FULLY_APPROVE,
                            ApprovalWorkflowDetails = new List<ApprovalWorkflowDetail>(),
                        };

                        var workflow = db.ApprovalWorkflows.FirstOrDefault(wf =>
                            wf.EMemoId == eMemoEntity.EMemoId &&
                            wf.WorkflowState == ApprovalWorkflowState.REVISION
                        );

                        if (workflow != null)
                        {
                            db.ApprovalWorkflows.Remove(workflow);
                            db.SaveChanges();
                        }

                        foreach (var personel in eMemoDoc.Participants)
                        {
                            var person = db.ApprovalWorkflowDetails.FirstOrDefault(wf =>
                            wf.ApprovalWorkflowId == eMemoEntity.Id &&
                            wf.ApprovalDecission == ApprovalDecission.REVISION);

                            var eperson = db.ApprovalWorkflowDetails.FirstOrDefault(wf =>
                            wf.ApprovalWorkflowId == eMemoEntity.Id &&
                            wf.ApprovalDecission == ApprovalDecission.REVISION);



                            if (person != null)
                            {

                                var wfDetail = new ApprovalWorkflowDetail()
                                {
                                    Id = Guid.NewGuid(),
                                    ApprovalWorkflowId = wfHeader.Id,
                                    ParticipantId = eperson.ParticipantId,
                                    ParticipantUserId = eperson.ParticipantUserId,
                                    ParticipantName = eperson.ParticipantName,
                                    ParticipantRole = eperson.ParticipantRole,
                                    ApprovalDecission = null,
                                    ApprovalNotes = null,
                                };
                                wfHeader.ApprovalWorkflowDetails.Add(wfDetail);

                                db.ApprovalWorkflowDetails.Remove(person);
                                db.SaveChanges();

                            }


                        }

                        db.ApprovalWorkflows.Add(wfHeader);

                        eMemoDoc.IsDraft = 0;

                        db.SaveChanges(request.Header.UserId);
                        tx.Commit();

                        response = new EMemoApprovalWorkflowResponse()
                        {
                            Header = request.Header?.Clone() as BaseHeader,
                            Detail = new EMemoApprovalWorkflowResponseDetail()
                            {
                                Id = eMemoEntity.Id.ToString(),
                                EMemoId = eMemoDoc.Id.ToString(),
                                WorkflowStatus = eMemoEntity.WorkflowState.ToString(),
                            }
                        };
                        response.Header.StatusCode = StatusCode.SUCCESS;

                    }


                    // mark the main eMemo document as no longer a draft


                }
                catch (Exception ex)
                {
                    _log.Error(ex,
                        "Problem occured when attempting to do this action ${serviceClass}.{serviceMethod}",
                        this.GetType().ToString(),
                        "CreateEMemoApproval(EMemoApprovalRequest)");

                    tx.Rollback();
                    response = new EMemoApprovalWorkflowResponse()
                    {
                        Header = request.Header?.Clone() as BaseHeader,
                    };
                    response.Header.StatusCode = StatusCode.SERVICE_ERROR;
                    response.Header.StatusMessage = ex.Message;

                }
            }

            return response;

        }

        public EMemoApprovalWorkflowDetailResponse SearchApprovalWorkflowDetails(EMemoApprovalWorkflowDetailRequest request)
        {
            var reqHeader = request.Header;
            var reqDetail = request.Detail;
            var reqFilter = request.Filter;
            var userId = reqHeader.UserId;
            var userIdGuid = Guid.Parse(reqHeader.UserId);

            // These are WF states where reviewers can see the approval requests
            var reviewerWorkflowStates = new[] {
                ApprovalWorkflowState.BEGIN,
                ApprovalWorkflowState.VALIDATORS_PARTIALLY_APPROVE,
                ApprovalWorkflowState.VALIDATORS_FULLY_APPROVE,
                ApprovalWorkflowState.APPROVERS_PARTIALLY_APPROVE,
                ApprovalWorkflowState.APPROVERS_FULLY_APPROVE,
                ApprovalWorkflowState.CANCELLED,
            };
            // These are WF states where approvers can see the approval requests
            var approverWorkflowStates = new[] {
                ApprovalWorkflowState.VALIDATORS_FULLY_APPROVE,
                ApprovalWorkflowState.APPROVERS_PARTIALLY_APPROVE,
                ApprovalWorkflowState.APPROVERS_FULLY_APPROVE,
                ApprovalWorkflowState.FINAL_APPROVERS_PARTIALLY_APPROVE,
                ApprovalWorkflowState.FINAL_APPROVERS_FULLY_APPROVE,
                ApprovalWorkflowState.CANCELLED,
            };
            var result = (from wfDetail in db.ApprovalWorkflowDetails
                          join wfHeader in db.ApprovalWorkflows on wfDetail.ApprovalWorkflowId equals wfHeader.Id
                          join eMemo in db.EMemos on wfHeader.EMemoId equals eMemo.Id
                          join reff in db.ReferenceDatas on eMemo.WorkUnitCode equals reff.Code
                          join userAccount in db.VUserAccounts on eMemo.Owner equals userAccount.Id.ToString()
                          where reff.Qualifier == RefDataQualifier.UnitKerja
                            && wfDetail.ParticipantUserId == userIdGuid
                            && wfHeader.WorkflowState.HasValue
                            && ((wfDetail.ParticipantRole == ParticipantRole.EMEMO_PTCP_REVIEWER && reviewerWorkflowStates.Contains(wfHeader.WorkflowState.Value))
                                || (wfDetail.ParticipantRole == ParticipantRole.EMEMO_PTCP_APPROVER && approverWorkflowStates.Contains(wfHeader.WorkflowState.Value)))
                          select new EMemoApprovalWorkflowDetailResponseDetail()
                          {
                              Id = wfDetail.Id.ToString(),
                              EMemoId = eMemo.Id.ToString(),
                              Subject = eMemo.Subject,
                              DocumentNo = eMemo.DocumentNo,
                              WorkUnitCode = eMemo.WorkUnitCode,
                              WorkUnitName = reff.LocalizedName,
                              RequesterUserId = userAccount.Id.ToString(),
                              RequesterPersonelName = userAccount.DisplayName,
                              RequestDate = wfHeader.CreatedDate,
                              ApprovalDecission = wfDetail.ApprovalDecission.HasValue ? ((int)wfDetail.ApprovalDecission.Value).ToString() : "",
                              WorkflowState = wfHeader.WorkflowState.ToString()
                          })
                          .ToList();

            reqFilter.FilteredRecordsCount = result.Count();
            reqFilter.TotalRecordsCount = result.Count();


            var response = new EMemoApprovalWorkflowDetailResponse()
            {
                Header = reqHeader.Clone() as BaseHeader,
                Filter = reqFilter,
                Details = result,
            };
            response.Header.StatusCode = StatusCode.SUCCESS;

            return response;

        }

        public EMemoApprovalWorkflowDetailResponse GetApprovalWorkflowDetails(EMemoApprovalWorkflowDetailRequest request)
        {
            var reqHeader = request.Header;
            var reqDetail = request.Detail;
            var userId = reqHeader.UserId;
            var userIdGuid = Guid.Parse(reqHeader.UserId);
            var approvalRequestGuid = Guid.Parse(reqDetail.Id);
            EMemoApprovalWorkflowDetailResponse response = null;

            var doc = db.ApprovalWorkflowDetails
                .Include("ApprovalWorkflow")
                .Include("ApprovalWorkflow.EMemo")
                .Where(e => e.Id == approvalRequestGuid && e.ParticipantUserId == userIdGuid)
                .FirstOrDefault();

            if (doc == null)
            {
                response = new EMemoApprovalWorkflowDetailResponse()
                {
                    Header = reqHeader.Clone() as BaseHeader
                };
                response.Header.StatusCode = StatusCode.NOT_FOUND;
                response.Header.StatusMessage = "Could not find entity with Id = " + reqDetail.Id;
                return response;
            }

            var approverDisplayName = db.VUserAccounts
                .Where(e => e.Id == doc.ParticipantUserId.ToString())
                .Select(e => e.DisplayName)
                .FirstOrDefault();

            // fetch approval requests for other people
            var otherApprovalRequests = db.ApprovalWorkflowDetails
                .Join(db.VUserAccounts
                    , wfDetail => wfDetail.ParticipantUserId.ToString()
                    , userAccount => userAccount.Id
                    , (wfDetail, userAccount) => new {
                        Id = wfDetail.Id,
                        ApprovalWorkflowId = wfDetail.ApprovalWorkflowId,
                        ApproverPersonelName = userAccount.DisplayName,
                        ParticipantUserId = wfDetail.ParticipantUserId,
                        ApprovalDecission = wfDetail.ApprovalDecission,
                        ApprovalNotes = wfDetail.ApprovalNotes,
                        ApprovalDecissionDate = wfDetail.ApprovalDecissionDate,
                    })
                .Where(e => e.ApprovalWorkflowId == doc.ApprovalWorkflowId
                    && e.ParticipantUserId != doc.ParticipantUserId)
                .Select(e => new EMemoApprovalWorkflowDetailResponseDetail()
                {
                    Id = e.Id.ToString(),
                    ApproverPersonelName = e.ApproverPersonelName,
                    ApproverPersonelPosition = "", // no position info just yet
                    ApprovalDecission = e.ApprovalDecission.HasValue ? ((int)e.ApprovalDecission.Value).ToString() : "",
                    ApprovalNotes = e.ApprovalNotes,
                    ApprovalDecissionDate = e.ApprovalDecissionDate,
                })
                .ToList();

            var resDetail = new EMemoApprovalWorkflowDetailResponseDetail()
            {
                Id = doc.Id.ToString(),
                EMemoId = doc.ApprovalWorkflow.EMemoId.ToString(),
                ApproverPersonelName = approverDisplayName,
                ApproverUserId = doc.ParticipantUserId.ToString(),
                ApproverPersonelPosition = "", // no position info just yet
                ApprovalDecission = doc.ApprovalDecission.ToString(),
                ApprovalNotes = doc.ApprovalNotes,
                ApprovalDecissionDate = doc.ApprovalDecissionDate,
                OtherApprovalRequests = otherApprovalRequests
                // No need for other fields
            };

            response = new EMemoApprovalWorkflowDetailResponse()
            {
                Header = request.Header.Clone() as BaseHeader,
                Detail = resDetail
            };

            return response;

        }

        public EMemoApprovalWorkflowDetailResponse UpdateApprovalWorkflowDetail(EMemoApprovalWorkflowDetailRequest request)
        {
            EMemoApprovalWorkflowDetailResponse response = null;
            var reqHeader = request.Header;
            var reqDetail = request.Detail;
            var userId = reqHeader.UserId;
            var userIdGuid = Guid.Parse(reqHeader.UserId);
            var approvalRequestGuid = Guid.Parse(reqDetail.Id);

            //
            // Fetch the wf detail
            //



            var wfDetail = db.ApprovalWorkflowDetails
                .Include("ApprovalWorkflow")
                .Where(e => e.Id == approvalRequestGuid)
                .FirstOrDefault();

            if (wfDetail == null)
            {
                response = new EMemoApprovalWorkflowDetailResponse()
                {
                    Header = reqHeader.Clone() as BaseHeader
                };
                response.Header.StatusCode = StatusCode.NOT_FOUND;
                response.Header.StatusMessage = "Could not find entity with Id = " + reqDetail.Id;
                return response;
            }

            //
            // Make sure the main wf has not been cancelled
            //
            if (wfDetail.ApprovalWorkflow != null && wfDetail.ApprovalWorkflow.WorkflowState == ApprovalWorkflowState.CANCELLED)
            {
                response = new EMemoApprovalWorkflowDetailResponse()
                {
                    Header = reqHeader.Clone() as BaseHeader
                };
                response.Header.StatusCode = StatusCode.BAD_REQUEST;
                response.Header.StatusMessage = "This approval request has been cancelled Id = " + reqDetail.Id;
                return response;
            }

            //
            // Make sure the user doing the update is the same as the principal id in the wf detail
            //
            if (wfDetail.ParticipantUserId != userIdGuid)
            {
                response = new EMemoApprovalWorkflowDetailResponse()
                {
                    Header = reqHeader.Clone() as BaseHeader
                };
                response.Header.StatusCode = StatusCode.BAD_REQUEST;
                response.Header.StatusMessage = "Invalid user doing the update Id = " + reqDetail.Id;
                return response;
            }

            //
            // Make sure the wf is in correct state to make the update
            //
            // These are valid WF states where reviewers are allowed to make change
            var reviewerWorkflowStates = new[] {
                ApprovalWorkflowState.BEGIN,
                ApprovalWorkflowState.VALIDATORS_PARTIALLY_APPROVE,
                ApprovalWorkflowState.VALIDATORS_FULLY_APPROVE,
            };
            // These are valid WF states where approvers are allowed to make change
            var approverWorkflowStates = new[] {
                ApprovalWorkflowState.VALIDATORS_FULLY_APPROVE,
                ApprovalWorkflowState.APPROVERS_PARTIALLY_APPROVE,
                ApprovalWorkflowState.APPROVERS_FULLY_APPROVE,
            };

            if (wfDetail.ParticipantRole == ParticipantRole.EMEMO_PTCP_REVIEWER
                && wfDetail.ApprovalWorkflow.WorkflowState.HasValue
                && !reviewerWorkflowStates.Contains(wfDetail.ApprovalWorkflow.WorkflowState.Value))
            {
                response = new EMemoApprovalWorkflowDetailResponse()
                {
                    Header = reqHeader.Clone() as BaseHeader
                };
                response.Header.StatusCode = StatusCode.BAD_REQUEST;
                response.Header.StatusMessage = "Invalid workflow state Id = " + reqDetail.Id;
                return response;
            }
            if (wfDetail.ParticipantRole == ParticipantRole.EMEMO_PTCP_APPROVER
                && wfDetail.ApprovalWorkflow.WorkflowState.HasValue
                && !approverWorkflowStates.Contains(wfDetail.ApprovalWorkflow.WorkflowState.Value))
            {
                response = new EMemoApprovalWorkflowDetailResponse()
                {
                    Header = reqHeader.Clone() as BaseHeader
                };
                response.Header.StatusCode = StatusCode.BAD_REQUEST;
                response.Header.StatusMessage = "Invalid workflow state Id = " + reqDetail.Id;
                return response;
            }

            if (!Enum.TryParse(reqDetail.ApprovalDecission, out ApprovalDecission approvalDecission))
            {
                response = new EMemoApprovalWorkflowDetailResponse()
                {
                    Header = reqHeader.Clone() as BaseHeader
                };
                response.Header.StatusCode = StatusCode.BAD_REQUEST;
                response.Header.StatusMessage = "Invalid workflow state Id = " + reqDetail.Id;
                return response;

            }

            //
            // Update the decission and decission timestamp of this wf detail
            // 
            using (var tx = db.Database.BeginTransaction())
            {
                try
                {

                    wfDetail.ApprovalDecission = approvalDecission;
                    wfDetail.ApprovalDecissionDate = DateTime.Now;
                    wfDetail.ApprovalNotes = reqDetail.ApprovalNotes;

                    var wxDetail = db.ApprovalWorkflows
                    .Where(e => e.Id == wfDetail.ApprovalWorkflowId)
                    .FirstOrDefault();

                    switch (wfDetail.ApprovalWorkflow.WorkflowState)
                    {
                        case ApprovalWorkflowState.BEGIN:
                            if (approvalDecission == ApprovalDecission.REJECT)
                            {
                                // if response is REJECT, then transition to REJECTED
                                wfDetail.ApprovalWorkflow.WorkflowState = ApprovalWorkflowState.REJECTED;
                                break;



                            }
                            else if (approvalDecission == ApprovalDecission.REVISION)
                            {
                                // if response is REJECT, then transition to REJECTED
                                wfDetail.ApprovalWorkflow.WorkflowState = ApprovalWorkflowState.REVISION;
                                break;

                            }
                            else if (approvalDecission == ApprovalDecission.CONTINUE_WITH_NOTES)
                            {
                                // if response is CONTINUE_WITH_NOTES, then...
                                bool allApprove = wfDetail.ApprovalWorkflow.ApprovalWorkflowDetails
                                    .Where(e => e.ParticipantRole == ParticipantRole.EMEMO_PTCP_REVIEWER)
                                    .All(e => e.ApprovalDecission == ApprovalDecission.APPROVE || e.ApprovalDecission == ApprovalDecission.CONTINUE_WITH_NOTES);

                                if (allApprove)
                                {
                                    // ... if all validators have approved, then transition to VALIDATORS_FULLY_APPROVE
                                    wfDetail.ApprovalWorkflow.WorkflowState = ApprovalWorkflowState.VALIDATORS_FULLY_APPROVE;
                                    break;
                                }
                                else
                                {
                                    // ... else transition to VALIDATORS_PARTIALLY_APPROVE
                                    wfDetail.ApprovalWorkflow.WorkflowState = ApprovalWorkflowState.VALIDATORS_PARTIALLY_APPROVE;
                                    break;

                                }
                            }
                            else if (approvalDecission == ApprovalDecission.APPROVE)
                            {
                                // if response is APPROVE, then...
                                bool allApprove = wfDetail.ApprovalWorkflow.ApprovalWorkflowDetails
                                    .Where(e => e.ParticipantRole == ParticipantRole.EMEMO_PTCP_REVIEWER)
                                    .All(e => e.ApprovalDecission == ApprovalDecission.APPROVE || e.ApprovalDecission == ApprovalDecission.CONTINUE_WITH_NOTES);

                                if (allApprove)
                                {
                                    // ... if all validators have approved, then transition to VALIDATORS_FULLY_APPROVE
                                    wfDetail.ApprovalWorkflow.WorkflowState = ApprovalWorkflowState.VALIDATORS_FULLY_APPROVE;
                                    break;
                                }
                                else
                                {
                                    // ... else transition to VALIDATORS_PARTIALLY_APPROVE
                                    wfDetail.ApprovalWorkflow.WorkflowState = ApprovalWorkflowState.VALIDATORS_PARTIALLY_APPROVE;
                                    break;
                                }
                            }

                            // All else, dont transition state.
                            break;

                        case ApprovalWorkflowState.VALIDATORS_PARTIALLY_APPROVE:
                            if (approvalDecission == ApprovalDecission.REJECT)
                            {
                                // if response is REJECT, then transition to REJECTED
                                wfDetail.ApprovalWorkflow.WorkflowState = ApprovalWorkflowState.REJECTED;
                                break;
                            }
                            else if (approvalDecission == ApprovalDecission.REVISION)
                            {
                                // if response is REJECT, then transition to REJECTED
                                wfDetail.ApprovalWorkflow.WorkflowState = ApprovalWorkflowState.REVISION;
                                break;

                            }
                            else if (approvalDecission == ApprovalDecission.CONTINUE_WITH_NOTES)
                            {
                                // if response is CONTINUE_WITH_NOTES, then...
                                bool allApprove = wfDetail.ApprovalWorkflow.ApprovalWorkflowDetails
                                    .Where(e => e.ParticipantRole == ParticipantRole.EMEMO_PTCP_REVIEWER)
                                    .All(e => e.ApprovalDecission == ApprovalDecission.APPROVE || e.ApprovalDecission == ApprovalDecission.CONTINUE_WITH_NOTES);

                                if (allApprove)
                                {
                                    // ... if all validators have approved, then transition to VALIDATORS_FULLY_APPROVE
                                    wfDetail.ApprovalWorkflow.WorkflowState = ApprovalWorkflowState.VALIDATORS_FULLY_APPROVE;
                                    break;
                                }
                                else
                                {
                                    // ... else transition to VALIDATORS_PARTIALLY_APPROVE
                                    wfDetail.ApprovalWorkflow.WorkflowState = ApprovalWorkflowState.VALIDATORS_PARTIALLY_APPROVE;
                                    break;

                                }
                            }
                            else if (approvalDecission == ApprovalDecission.APPROVE)
                            {
                                // if response is APPROVE, then...
                                bool allApprove = wfDetail.ApprovalWorkflow.ApprovalWorkflowDetails
                                    .Where(e => e.ParticipantRole == ParticipantRole.EMEMO_PTCP_REVIEWER)
                                    .All(e => e.ApprovalDecission == ApprovalDecission.APPROVE || e.ApprovalDecission == ApprovalDecission.CONTINUE_WITH_NOTES);

                                if (allApprove)
                                {
                                    // ... if all validators have approved, then transition to VALIDATORS_FULLY_APPROVE
                                    wfDetail.ApprovalWorkflow.WorkflowState = ApprovalWorkflowState.VALIDATORS_FULLY_APPROVE;
                                    break;
                                }
                                else
                                {
                                    // ... else transition to VALIDATORS_PARTIALLY_APPROVE
                                    wfDetail.ApprovalWorkflow.WorkflowState = ApprovalWorkflowState.VALIDATORS_PARTIALLY_APPROVE;
                                    break;
                                }
                            }

                            // All else, dont transition state.
                            break;
                        case ApprovalWorkflowState.VALIDATORS_FULLY_APPROVE:
                            if (approvalDecission == ApprovalDecission.REJECT)
                            {
                                // if response is REJECT, then transition to REJECTED
                                wfDetail.ApprovalWorkflow.WorkflowState = ApprovalWorkflowState.REJECTED;
                                break;
                            }
                            else if (approvalDecission == ApprovalDecission.REVISION)
                            {
                                // if response is REJECT, then transition to REJECTED
                                wfDetail.ApprovalWorkflow.WorkflowState = ApprovalWorkflowState.REVISION;
                                break;

                            }
                            else if (approvalDecission == ApprovalDecission.CONTINUE_WITH_NOTES)
                            {
                                // if response is CONTINUE_WITH_NOTES, then...
                                bool allApprove = wfDetail.ApprovalWorkflow.ApprovalWorkflowDetails
                                    .Where(e => e.ParticipantRole == ParticipantRole.EMEMO_PTCP_APPROVER)
                                    .All(e => e.ApprovalDecission == ApprovalDecission.APPROVE || e.ApprovalDecission == ApprovalDecission.CONTINUE_WITH_NOTES);

                                if (allApprove)
                                {
                                    // ... if all approvers have approved, then transition to APPROVERS_FULLY_APPROVE
                                    wfDetail.ApprovalWorkflow.WorkflowState = ApprovalWorkflowState.APPROVERS_FULLY_APPROVE;
                                    break;
                                }
                                else
                                {
                                    // ... else transition to APPROVERS_PARTIALLY_APPROVE
                                    wfDetail.ApprovalWorkflow.WorkflowState = ApprovalWorkflowState.APPROVERS_PARTIALLY_APPROVE;
                                    break;

                                }
                            }
                            else if (approvalDecission == ApprovalDecission.APPROVE)
                            {
                                // if response is APPROVE, then...
                                bool allApprove = wfDetail.ApprovalWorkflow.ApprovalWorkflowDetails
                                    .Where(e => e.ParticipantRole == ParticipantRole.EMEMO_PTCP_APPROVER)
                                    .All(e => e.ApprovalDecission == ApprovalDecission.APPROVE || e.ApprovalDecission == ApprovalDecission.CONTINUE_WITH_NOTES);

                                if (allApprove)
                                {
                                    // ... if all approvers have approved, then transition to APPROVERS_FULLY_APPROVE
                                    wfDetail.ApprovalWorkflow.WorkflowState = ApprovalWorkflowState.APPROVERS_FULLY_APPROVE;
                                    break;
                                }
                                else
                                {
                                    // ... else transition to APPROVERS_PARTIALLY_APPROVE
                                    wfDetail.ApprovalWorkflow.WorkflowState = ApprovalWorkflowState.APPROVERS_PARTIALLY_APPROVE;
                                    break;
                                }
                            }

                            // All else, dont transition state.
                            break;
                        case ApprovalWorkflowState.APPROVERS_PARTIALLY_APPROVE:
                            if (approvalDecission == ApprovalDecission.REJECT)
                            {
                                // if response is REJECT, then transition to REJECTED
                                wfDetail.ApprovalWorkflow.WorkflowState = ApprovalWorkflowState.REJECTED;
                                break;
                            }
                            else if (approvalDecission == ApprovalDecission.REVISION)
                            {
                                // if response is REJECT, then transition to REJECTED
                                wfDetail.ApprovalWorkflow.WorkflowState = ApprovalWorkflowState.REVISION;
                                break;

                            }
                            else if (approvalDecission == ApprovalDecission.CONTINUE_WITH_NOTES)
                            {
                                // if response is CONTINUE_WITH_NOTES, then...
                                bool allApprove = wfDetail.ApprovalWorkflow.ApprovalWorkflowDetails
                                    .Where(e => e.ParticipantRole == ParticipantRole.EMEMO_PTCP_APPROVER)
                                    .All(e => e.ApprovalDecission == ApprovalDecission.APPROVE || e.ApprovalDecission == ApprovalDecission.CONTINUE_WITH_NOTES);

                                if (allApprove)
                                {
                                    // ... if all approvers have approved, then transition to APPROVERS_FULLY_APPROVE
                                    wfDetail.ApprovalWorkflow.WorkflowState = ApprovalWorkflowState.APPROVERS_FULLY_APPROVE;
                                    break;
                                }
                                else
                                {
                                    // ... else transition to APPROVERS_PARTIALLY_APPROVE
                                    wfDetail.ApprovalWorkflow.WorkflowState = ApprovalWorkflowState.APPROVERS_PARTIALLY_APPROVE;
                                    break;

                                }
                            }
                            else if (approvalDecission == ApprovalDecission.APPROVE)
                            {
                                // if response is APPROVE, then...
                                bool allApprove = wfDetail.ApprovalWorkflow.ApprovalWorkflowDetails
                                    .Where(e => e.ParticipantRole == ParticipantRole.EMEMO_PTCP_APPROVER)
                                    .All(e => e.ApprovalDecission == ApprovalDecission.APPROVE || e.ApprovalDecission == ApprovalDecission.CONTINUE_WITH_NOTES);

                                if (allApprove)
                                {
                                    // ... if all approvers have approved, then transition to APPROVERS_FULLY_APPROVE
                                    wfDetail.ApprovalWorkflow.WorkflowState = ApprovalWorkflowState.APPROVERS_FULLY_APPROVE;
                                    break;
                                }
                                else
                                {
                                    // ... else transition to APPROVERS_PARTIALLY_APPROVE
                                    wfDetail.ApprovalWorkflow.WorkflowState = ApprovalWorkflowState.APPROVERS_PARTIALLY_APPROVE;
                                    break;
                                }
                            }

                            // All else, dont transition state.
                            break;
                        case ApprovalWorkflowState.APPROVERS_FULLY_APPROVE:
                            // Final state. Cannot transition further
                            break;
                        case ApprovalWorkflowState.REJECTED:
                            // Final state. Cannot transition further
                            break;
                        case ApprovalWorkflowState.REVISION:
                            // Final state. Cannot transition further
                            break;
                        case ApprovalWorkflowState.CANCELLED:
                            // Final state. Cannot transition further
                            break;
                    }

                    // 
                    // Check on the wf state and determine if we need to update it
                    //
                    if (wfDetail.ApprovalWorkflow.WorkflowState == ApprovalWorkflowState.BEGIN
                        || wfDetail.ApprovalWorkflow.WorkflowState == ApprovalWorkflowState.VALIDATORS_PARTIALLY_APPROVE)
                    {

                        // If we are waiting for all validators to approve and
                        // all validators has indeed been approving, then advance
                        // the wf state to VALIDATORS_FULLY_APPROVE

                        bool allReviewersApproved = wfDetail.ApprovalWorkflow.ApprovalWorkflowDetails
                            .Where(e => e.ParticipantRole == ParticipantRole.EMEMO_PTCP_REVIEWER)
                            .All(e => e.ApprovalDecission == ApprovalDecission.APPROVE);

                        if (allReviewersApproved)
                        {
                            wfDetail.ApprovalWorkflow.WorkflowState = ApprovalWorkflowState.VALIDATORS_FULLY_APPROVE;
                        }
                    }
                    else if (wfDetail.ApprovalWorkflow.WorkflowState == ApprovalWorkflowState.VALIDATORS_FULLY_APPROVE
                        || wfDetail.ApprovalWorkflow.WorkflowState == ApprovalWorkflowState.APPROVERS_PARTIALLY_APPROVE)
                    {
                        // If we are waiting for all approvers to approve and
                        // all approvers has indeed been approving, then advance
                        // the wf state to APPROVERS_FULLY_APPROVE

                        bool allApproversApproved = wfDetail.ApprovalWorkflow.ApprovalWorkflowDetails
                            .Where(e => e.ParticipantRole == ParticipantRole.EMEMO_PTCP_APPROVER)
                            .All(e => e.ApprovalDecission == ApprovalDecission.APPROVE);

                        if (allApproversApproved)
                        {
                            wfDetail.ApprovalWorkflow.WorkflowState = ApprovalWorkflowState.VALIDATORS_FULLY_APPROVE;
                        }
                    }
                    else
                    {
                        // Otherwise do nothing to the wf state
                    }

                    var status = "";

                    if (approvalDecission == ApprovalDecission.APPROVE)
                    {
                        status = "APPROVE";
                    }
                    else if (approvalDecission == ApprovalDecission.REJECT)
                    {
                        status = "REJECT";
                    }
                    else if (approvalDecission == ApprovalDecission.REVISION)
                    {
                        status = "REVISION";
                    }
                    else if (approvalDecission == ApprovalDecission.CONTINUE_WITH_NOTES)
                    {
                        status = "CONTINUE WITH NOTES";
                    }


                    var logEntity = new Reston.Eproc.Model.EMemo.EMemoLogs
                    {
                        Id = Guid.NewGuid(),
                        EMemoId = wxDetail.EMemoId, // pick the correct FK
                        Version = 1,                                   // first version
                        Content = wfDetail.ParticipantName + "<br> Status: <br>" + status + "<br> Catatan: <br>" + reqDetail.ApprovalNotes, // compact JSON snapshot
                        CreateDate = DateTime.Now,

                    };
                    db.EMemoLogs.Add(logEntity);            // track the log row

                    // Save all the changes
                    db.SaveChanges(userId);
                    tx.Commit();

                    response = new EMemoApprovalWorkflowDetailResponse()
                    {
                        Header = request.Header.Clone() as BaseHeader,
                    };
                    response.Header.StatusCode = StatusCode.SUCCESS;
                }
                catch (Exception ex)
                {
                    _log.Error(ex, "Failed to update workflow response. Id = " + reqDetail.Id);
                    tx.Rollback();
                    response = new EMemoApprovalWorkflowDetailResponse()
                    {
                        Header = reqHeader.Clone() as BaseHeader
                    };
                    response.Header.StatusCode = StatusCode.SERVICE_ERROR;
                    response.Header.StatusMessage = "Failed to update workflow response. Id = " + reqDetail.Id + ". " + ex.Message;
                    return response;
                }
            }

            return response;
        }



        public EMemoTemplateResponse SearchEMemoTemplates(EMemoTemplateRequest request)
        {
            var reqDetail = request.Detail;
            var reqFilter = request.Filter;

            var query = from template in db.EMemoTemplates
                        select template;

            reqFilter.TotalRecordsCount = query.Count();

            query = from template in query
                    where template.Title.Contains(reqFilter.Title)
                    select template;

            reqFilter.FilteredRecordsCount = query.Count();

            var results = query.OrderBy(template => template.Title)
                .Skip(reqFilter.Offset.GetValueOrDefault())
                .Take(reqFilter.PageLength.GetValueOrDefault(7))
                .Select(template => new EMemoTemplateResponseDetail()
                {
                    Id = template.Id,
                    Title = template.Title,
                    ContentType = template.ContentType,
                    //ContentData = Convert.ToBase64String(template.ContentData),
                }).ToList();

            var response = new EMemoTemplateResponse()
            {
                Header = request.Header.Clone() as BaseHeader,
                Details = results,
                Filter = reqFilter
            };

            return response;
        }

        public EMemoTemplateResponse GetEMemoTemplate(EMemoTemplateRequest request)
        {
            var reqHeader = request.Header;
            var reqDetail = request.Detail;
            var docId = reqDetail.Id;
            var docIdStr = docId.ToString();
            var userIdStr = reqHeader.UserId;
            var response = new EMemoTemplateResponse()
            {
                Header = request.Header.Clone() as BaseHeader,
            };

            var entity = (from template in db.EMemoTemplates
                          where template.Id == docId
                          select template
                      ).FirstOrDefault();

            if (entity == null)
            {
                response.Header.StatusCode = StatusCode.NOT_FOUND;
                response.Header.StatusMessage = "Could not find entity with Id = " + docIdStr;
                return response;
            }

            response.Header.StatusCode = StatusCode.SUCCESS;
            response.Detail = new EMemoTemplateResponseDetail()
            {
                Id = entity.Id,
                Title = entity.Title,
                ContentType = entity.ContentType,
                ContentData = Convert.ToBase64String(entity.ContentData)
            };

            return response;

        }

        private string GenerateDocumentNumber(EMemoRequest request)
        {
            var reqDetail = request.Detail;
            var workUnitCode = reqDetail.WorkUnitCode;
            var now = DateTime.Now;
            var result = "";


            // cari nomor dokumen terakhir untuk unit kerja tersebut
            var existingWorkUnitDocNumber = db.DocumentNumbers
                .Where(e => e.WorkUnitCode == workUnitCode)
                .FirstOrDefault();

            if (existingWorkUnitDocNumber == null)
            {
                // jika tidak ditemukan, maka buat nomor perdana, simpan, dan kembalikan
                var newDocumentNumber = EMEMO_DOC_NO_TEMPLATE;
                newDocumentNumber = newDocumentNumber.Replace("{SEQNO}", "001");
                newDocumentNumber = newDocumentNumber.Replace("{WORK_UNIT_CODE}", workUnitCode);
                newDocumentNumber = newDocumentNumber.Replace("{MONTH_2_DIGITS}", now.ToString("MM"));
                newDocumentNumber = newDocumentNumber.Replace("{YEAR_4_DIGITS}", now.ToString("yyyy"));

                db.DocumentNumbers.Add(new DocumentNumber()
                {
                    Id = Guid.NewGuid(),
                    WorkUnitCode = workUnitCode,
                    LastEMemoDocNo = newDocumentNumber
                });
                db.SaveChanges();

                result = newDocumentNumber;
            }
            else
            {
                // jika ditemukan

                var lastNumber = existingWorkUnitDocNumber.LastEMemoDocNo;
                var lastMonth = lastNumber.Substring(Math.Max(0, lastNumber.Length - 7), 2);
                var lastYear = lastNumber.Substring(Math.Max(0, lastNumber.Length - 4));
                var thisMonth = now.ToString("MM");
                var thisYear = now.ToString("yyyy");
                var nextSeq = "";


                if (lastMonth != thisMonth || lastYear != thisYear)
                {
                    // if beda bulan or beda tahun, start over sequence from 001
                    nextSeq = "001";
                }
                else
                {
                    // if we are still in the same month and year, advance sequence by one
                    if (lastNumber.Length >= 3)
                    {
                        // grab the sequence part
                        var seqPart = lastNumber.Substring(0, 3);
                        int.TryParse(seqPart, out int seqNo);
                        if (seqNo == 999)
                        {
                            seqNo = 0;
                        }
                        else
                        {
                            seqNo++;
                        }
                        nextSeq = seqNo.ToString("D3");
                    }
                    else
                    {
                        nextSeq = "001";
                    }

                }
                var newDocumentNumber = EMEMO_DOC_NO_TEMPLATE;
                newDocumentNumber = newDocumentNumber.Replace("{SEQNO}", nextSeq);
                newDocumentNumber = newDocumentNumber.Replace("{WORK_UNIT_CODE}", workUnitCode);
                newDocumentNumber = newDocumentNumber.Replace("{MONTH_2_DIGITS}", thisMonth);
                newDocumentNumber = newDocumentNumber.Replace("{YEAR_4_DIGITS}", thisYear);
                existingWorkUnitDocNumber.LastEMemoDocNo = newDocumentNumber;
                db.SaveChanges();
                result = newDocumentNumber;

            }

            return result;
        }

        public EMemoResponse GetRiwayat(EMemoRequest request, Guid documentOwnerId)
        {

            var response = new EMemoResponse();

            var filterStatus = "";


            var query = db.EMemos
     .Include("Participants")
     .Select(eMemo => new
     {
         eMemo,
         WFHeaders = db.ApprovalWorkflows.Where(wf => wf.EMemoId == eMemo.Id),
     })
     .Select(x => new
     {
         x.eMemo,
         WFDetails = db.ApprovalWorkflowDetails
             .Where(wfd => x.WFHeaders.Select(wf => wf.Id).Contains(wfd.ApprovalWorkflowId))
     })
     .Where(x =>
         x.eMemo.Owner == documentOwnerId.ToString() ||
         x.WFDetails.Any(d => d.ParticipantUserId == documentOwnerId))
     .Select(x => new
     {
         x.eMemo.Id,
         x.eMemo.Subject,
         x.eMemo.DocumentNo,
         x.eMemo.WorkUnitCode,
         x.eMemo.InternalRefNo,
         x.eMemo.HPSAmount,
         x.eMemo.HPSCurrency,
         x.eMemo.IsDraft,
         x.eMemo.Owner,
         x.eMemo.OwnerPersonelName,
         x.eMemo.CreatedDate,
         x.eMemo.CreatedBy,
         x.eMemo.ModifiedDate,
         x.eMemo.ModifiedBy
         // You can add aggregated WFHeader/WFDetails if needed
     })
     .Distinct(); // optional depending on the result


            var result = query.Select(e => new EMemoResponseDetail()
            {
                Id = e.Id,
                Subject = e.Subject,
                DocumentNo = e.DocumentNo,
                InternalRefNo = e.InternalRefNo,
                IsDraft = e.IsDraft == 1,
                //WorkflowStatus = e.ApprovalWorkflowStatus.HasValue ? ((int)e.ApprovalWorkflowStatus.Value).ToString() : "",
                WorkflowStatus = ((int)(from wfHeader
                                  in db.ApprovalWorkflows
                                        where wfHeader.EMemoId == e.Id
                                        select wfHeader.WorkflowState)
                                  .FirstOrDefault()
                                  .Value).ToString()

            }).ToList();

            response.Header = request.Header?.Clone() as BaseHeader;
            response.Header.StatusCode = StatusCode.SUCCESS;
            response.Details = result;
            response.Filter = request.Filter;

            return response;

        }
    }
}
