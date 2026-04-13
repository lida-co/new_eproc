using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Helper;
using Reston.Pinata.Model;
using Reston.Eproc.Model.ENota;
using Reston.EProc.Web.Base.ViewModels;
using Reston.EProc.Web.Modules.ENota.ViewModels;
using NLog;
using System.Diagnostics;
using System.IdentityModel.Protocols.WSTrust;
using Reston.Eproc.Model.EMemo;
using Reston.EProc.Web.Modules.EMemo.ViewModels;

namespace Reston.EProc.Web.Modules.ENota.Services
{
    public class ENotaService
    {
        private const string ENOTA_DOC_NO_TEMPLATE = "{SEQNO}/NOTA-OPR/MTF/{WORK_UNIT_CODE}/{MONTH_2_DIGITS}/{YEAR_4_DIGITS}";

        private static Logger _log = LogManager.GetCurrentClassLogger();

        protected AppDbContext db;

        public string AttactmentPenawaran { get; private set; }
        public string AttactmentPenawaranType { get; private set; }
        public byte[] AttactmentPenawaranFiles { get; private set; }
        public string AttachtmentAnalisa { get; private set; }
        public string AttachtmentAnalisaType { get; private set; }
        public byte[] AttachtmentAnalisaFile { get; private set; }
        public string AttachmentLampiranType { get; private set; }
        public string AttachmentLampiran { get; private set; }
        public byte[] AttachmentLampiranFile { get; private set; }

        public ENotaService(AppDbContext dbContext)
        {
            db = dbContext;
        }

        public ENotaResponse Search(ENotaRequest request, Guid documentOwnerId)
        {
            var response = new ENotaResponse();

            var filterStatus = "";

            /*var query = db.ENotas
                .Include("Participants")
                .Where(e => e.Owner == documentOwnerId.ToString());
            */

            var reqHeader = request.Header;

            Guid userGuid = Guid.Parse(reqHeader.UserId);

            var query = db.ENotas
                .Where(eNota =>
                    eNota.Owner == reqHeader.UserId ||
                    eNota.Participants.Any(p => p.UserId == reqHeader.UserId) ||
                    db.ApprovalWorkflowDetails.Any(wfd =>
                        db.ApprovalWorkflowsNota
                            .Where(wfh => wfh.ENotaId == eNota.Id)
                            .Select(wfh => wfh.Id)
                            .Contains(wfd.ApprovalWorkflowId)
                        && wfd.ParticipantUserId == userGuid
                    )
                )
                .Select(eNota => new
                {
                    Id = eNota.Id,
                    Subject = eNota.Subject,
                    DocumentNo = eNota.DocumentNo,
                    WorkUnitCode = eNota.WorkUnitCode,
                    InternalRefNo = eNota.InternalRefNo,
                    HPSAmount = eNota.HPSAmount,
                    HPSCurrency = eNota.HPSCurrency,
                    IsDraft = eNota.IsDraft,
                    Owner = eNota.Owner,
                    OwnerPersonelName = eNota.OwnerPersonelName,
                    CreatedDate = eNota.CreatedDate,
                    CreatedBy = eNota.CreatedBy,
                    ModifiedDate = eNota.ModifiedDate,
                    ModifiedBy = eNota.ModifiedBy
                });

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

            var result = query.Select(e => new ENotaResponseDetail()
            {
                Id = e.Id,
                Subject = e.Subject,
                DocumentNo = e.DocumentNo,
                InternalRefNo = e.InternalRefNo,
                IsDraft = e.IsDraft == 1,
                //WorkflowStatus = e.ApprovalWorkflowStatus.HasValue ? ((int)e.ApprovalWorkflowStatus.Value).ToString() : "",
                WorkflowStatus = ((int)(from wfHeader
                                  in db.ApprovalWorkflowsNota
                                        where wfHeader.ENotaId == e.Id
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

        public ENotaResponse Get(ENotaRequest request, Guid documentOwnerUserId)
        {
            var requestDetail = request.Detail;
            var response = new ENotaResponse();
            var responseDetail = new ENotaResponseDetail();
            var docGuid = requestDetail.Id;


            var eNotaEntity = db.ENotas
                .Include("ENotaAttachments")
                .Include("ENotaAttachments.Attachment")
                .Include("Participants")
                .Where(e => e.Id == docGuid)
                .FirstOrDefault();

            if (eNotaEntity == null)
            {
                response.Header = request.Header.Clone() as BaseHeader;
                response.Header.StatusCode = StatusCode.NOT_FOUND;
                response.Header.StatusMessage = "Could not find entity with Id = " + docGuid;
                return response;
            }

            responseDetail.Id = eNotaEntity.Id;
            responseDetail.DocumentNo = eNotaEntity.DocumentNo;
            responseDetail.WorkUnitCode = eNotaEntity.WorkUnitCode;
            responseDetail.WorkUnitName = db.ReferenceDatas
                                    .Where(e => e.Qualifier == RefDataQualifier.UnitKerja
                                        && e.Code == eNotaEntity.WorkUnitCode)
                                    .Select(e => e.LocalizedName)
                                    .FirstOrDefault();
            responseDetail.InternalRefNo = eNotaEntity.InternalRefNo;
            responseDetail.Subject = eNotaEntity.Subject;
            responseDetail.Kepada = eNotaEntity.Kepada;
            responseDetail.Tembusan = eNotaEntity.Tembusan;
            responseDetail.HPSAmount = eNotaEntity.HPSAmount;
            responseDetail.IsDraft = eNotaEntity.IsDraft == 1;
            responseDetail.OwnerUserId = eNotaEntity.Owner;
            // TODO consider persisting ContentInputMode
            // for now we simply deduce ContentInputMode
            // from ContentType
            responseDetail.ContentInputMode = responseDetail.ContentType == "text/html" ? "edit" : "upload";

            // Map participants to view model
            eNotaEntity.Participants.ToList().ForEach((e) => {

                var ptcp = new ENotaParticipant()
                {
                    Id = e.Id,
                    PersonelUserId = e.UserId,
                    PersonelFullName = e.EmployeeName,
                    ParticipantRole = e.ParticipantRole.HasValue ? ((int)e.ParticipantRole.Value).ToString() : "",
                    Order = e.Ordered,
                };

                switch (e.ParticipantRole)
                {
                    case ParticipantRoleNota.ENOTA_PTCP_DIREKTUR:
                        responseDetail.AppDireksi.Add(ptcp);
                        break;
                    case ParticipantRoleNota.ENOTA_PTCP_APPROVER:
                        responseDetail.Approvers.Add(ptcp);
                        break;
                    case ParticipantRoleNota.ENOTA_PTCP_REVIEWER:
                        responseDetail.Validators.Add(ptcp);
                        break;
                }

            });

            // Map Attachments to view model
            eNotaEntity.ENotaAttachments.ToList().ForEach((e) => {

                var attcm = new ViewModels.ENotaAttachment()
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
                    case AttachmentTypeNota.ENOTA_ATT_NOTA:
                        responseDetail.ContentType = e.Attachment.ContentType;
                        responseDetail.ContentData = Convert.ToBase64String(e.Attachment.ContentData);
                        break;
                    case AttachmentTypeNota.ENOTA_ATT_QUOTATION:
                        responseDetail.PenawaranAttachments.Add(attcm);
                        break;
                    case AttachmentTypeNota.ENOTA_ATT_COSTBENEFIT:
                        responseDetail.CostBenefitAttachments.Add(attcm);
                        break;
                    case AttachmentTypeNota.ENOTA_ATT_OTHER:
                        responseDetail.OtherAttachments.Add(attcm);
                        break;

                }

            });

            // Map approval workflow details to view model
            responseDetail.ApprovalWorkflowDetails = db.ApprovalWorkflowDetailsNota
                .Include("ApprovalWorkflow")
                .Where(e => e.ApprovalWorkflow.ENotaId == eNotaEntity.Id)
                //.OrderBy(e => e.ParticipantRole)
                .Select(e => new ENotaApprovalWorkflowDetailResponseDetail()
                {
                    Id = e.Id.ToString(),
                    ENotaId = e.ApprovalWorkflow.ENotaId.ToString(),
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

        public ENotaResponse Create(ENotaRequest request)
        {
            var response = new ENotaResponse();
            var now = DateTime.Now;
            var reqHeader = request.Header;
            var reqDetail = request.Detail;
            var userId = reqHeader.UserId;

            // ENota entity
            var eNotaEntity = new Reston.Eproc.Model.ENota.ENota()
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
                Participants = new List<Reston.Eproc.Model.ENota.ParticipantNota>(),
                ENotaAttachments = new List<Reston.Eproc.Model.ENota.ENotaAttachment>(),
            };


            // Participants entitites Reviewers
            var order = 0;
            foreach (var item in reqDetail.Validators)
            {
                var eNotaParticipantEntity = new Reston.Eproc.Model.ENota.ParticipantNota()
                {
                    Id = Guid.NewGuid(),
                    ENotaId = eNotaEntity.Id,
                    ENota = eNotaEntity,
                    UserId = item.PersonelUserId,
                    EmployeeName = item.PersonelFullName,
                    ParticipantRole = ParticipantRoleNota.ENOTA_PTCP_REVIEWER,
                    Ordered = order,
                };
                eNotaEntity.Participants.Add(eNotaParticipantEntity);
                order++;
            }

            // Participants entitites Approvers
            order = 0;
            foreach (var item in reqDetail.Approvers)
            {
                var eNotaParticipantEntity = new Reston.Eproc.Model.ENota.ParticipantNota()
                {
                    Id = Guid.NewGuid(),
                    ENotaId = eNotaEntity.Id,
                    ENota = eNotaEntity,
                    UserId = item.PersonelUserId,
                    EmployeeName = item.PersonelFullName,
                    ParticipantRole = ParticipantRoleNota.ENOTA_PTCP_APPROVER,
                    Ordered = order,
                };
                eNotaEntity.Participants.Add(eNotaParticipantEntity);
                order++;
            }

            // Participants entitites Approvers
            order = 0;
            foreach (var item in reqDetail.AppDireksi)
            {
                var eNotaParticipantEntity = new Reston.Eproc.Model.ENota.ParticipantNota()
                {
                    Id = Guid.NewGuid(),
                    ENotaId = eNotaEntity.Id,
                    ENota = eNotaEntity,
                    UserId = item.PersonelUserId,
                    EmployeeName = item.PersonelFullName,
                    ParticipantRole = ParticipantRoleNota.ENOTA_PTCP_DIREKTUR,
                    Ordered = order,
                };
                eNotaEntity.Participants.Add(eNotaParticipantEntity);
                order++;
            }


            // LOG





            // ENotaAttachments main enota document 
            order = 0;
            // there's only single main enota document
            var attachmentEntity = new Reston.Eproc.Model.ENota.AttachmentNota()
            {
                Id = Guid.NewGuid(),
                ContentType = reqDetail.ContentType,
                ContentData = Convert.FromBase64String(reqDetail.ContentData),
                Title = reqDetail.ContentTitle,
            };
            var eNotaAttachmentEntity = new Eproc.Model.ENota.ENotaAttachment()
            {
                Id = Guid.NewGuid(),
                AttachmentId = attachmentEntity.Id,
                Attachment = attachmentEntity,
                ENotaId = eNotaEntity.Id,
                ENota = eNotaEntity,
                AttachmentType = AttachmentTypeNota.ENOTA_ATT_NOTA,
                Ordered = order
            };
            eNotaEntity.ENotaAttachments.Add(eNotaAttachmentEntity);
            // calculate main doc content size here
            reqDetail.ContentSize = attachmentEntity.ContentData.Length;

            // ENotaAttachments quotation/penawaran documents 
            order = 0;
            string AttactmentPenawaran = "";
            string AttactmentPenawaranType = "";
            byte[] AttactmentPenawaranFiles = null;
            foreach (var item in reqDetail.PenawaranAttachments)
            {
                AttactmentPenawaran = item.Name;
                AttactmentPenawaranType = item.ContentType;
                AttactmentPenawaranFiles = Convert.FromBase64String(item.Data);
                attachmentEntity = new Reston.Eproc.Model.ENota.AttachmentNota()
                {
                    Id = Guid.NewGuid(),
                    ContentType = item.ContentType,
                    ContentData = Convert.FromBase64String(item.Data),
                    Title = item.Name,
                };

                eNotaAttachmentEntity = new Eproc.Model.ENota.ENotaAttachment()
                {
                    Id = Guid.NewGuid(),
                    AttachmentId = attachmentEntity.Id,
                    Attachment = attachmentEntity,
                    ENotaId = eNotaEntity.Id,
                    ENota = eNotaEntity,
                    AttachmentType = AttachmentTypeNota.ENOTA_ATT_QUOTATION,
                    Ordered = order
                };

                eNotaEntity.ENotaAttachments.Add(eNotaAttachmentEntity);
                order++;
            }

            // ENotaAttachments cost benef analysis documents
            order = 0;
            string AttachtmentAnalisa = "";
            string AttachtmentAnalisaType = "";
            byte[] AttachtmentAnalisaFile = null;
            foreach (var item in reqDetail.CostBenefitAttachments)
            {
                AttachtmentAnalisa = item.Name;
                AttachtmentAnalisaType = item.ContentType;
                AttachtmentAnalisaFile = Convert.FromBase64String(item.Data);
                attachmentEntity = new Reston.Eproc.Model.ENota.AttachmentNota()
                {
                    Id = Guid.NewGuid(),
                    ContentType = item.ContentType,
                    ContentData = Convert.FromBase64String(item.Data),
                    Title = item.Name,
                };

                eNotaAttachmentEntity = new Eproc.Model.ENota.ENotaAttachment()
                {
                    Id = Guid.NewGuid(),
                    AttachmentId = attachmentEntity.Id,
                    Attachment = attachmentEntity,
                    ENotaId = eNotaEntity.Id,
                    ENota = eNotaEntity,
                    AttachmentType = AttachmentTypeNota.ENOTA_ATT_COSTBENEFIT,
                    Ordered = order
                };

                eNotaEntity.ENotaAttachments.Add(eNotaAttachmentEntity);
                order++;
            }

            // ENotaAttachments other documents
            order = 0;
            string AttachmentLampiran = "";
            string AttachmentLampiranType = "";
            byte[] AttachmentLampiranFile = null;
            foreach (var item in reqDetail.OtherAttachments)
            {
                AttachmentLampiran = item.Name;
                AttachmentLampiranType = item.ContentType;
                AttachmentLampiranFile = Convert.FromBase64String(item.Data);

                attachmentEntity = new Reston.Eproc.Model.ENota.AttachmentNota()
                {
                    Id = Guid.NewGuid(),
                    ContentType = item.ContentType,
                    ContentData = Convert.FromBase64String(item.Data),
                    Title = item.Name,
                };

                eNotaAttachmentEntity = new Eproc.Model.ENota.ENotaAttachment()
                {
                    Id = Guid.NewGuid(),
                    AttachmentId = attachmentEntity.Id,
                    Attachment = attachmentEntity,
                    ENotaId = eNotaEntity.Id,
                    ENota = eNotaEntity,
                    AttachmentType = AttachmentTypeNota.ENOTA_ATT_OTHER,
                    Ordered = order
                };

                eNotaEntity.ENotaAttachments.Add(eNotaAttachmentEntity);
                order++;
            }


            var logEntity = new Reston.Eproc.Model.ENota.ENotaLogs
            {
                Id = Guid.NewGuid(),
                ENotaId = eNotaEntity.Id, // pick the correct FK
                Version = 1,                                   // first version
                Content = "CREATE NEW", // compact JSON snapshot
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
            db.ENotaLogs.Add(logEntity);            // track the log row



            db.ENotas.Add(eNotaEntity);
            db.SaveChanges(userId);

            response.Header = request.Header.Clone() as BaseHeader;
            response.Header.StatusCode = StatusCode.SUCCESS;
            response.Detail = new ENotaResponseDetail(reqDetail);
            response.Detail.Id = eNotaEntity.Id;
            response.Detail.DocumentNo = eNotaEntity.DocumentNo;




            var ENotaLogEntity = new Reston.Eproc.Model.ENota.ENotaLogs()
            {
                Id = Guid.NewGuid(),
                ENotaId = eNotaEntity.Id,
                Version = 1
            };
            eNotaEntity.ENotaLogs.Add(ENotaLogEntity);

            return response;
        }

        public ENotaResponse Update(ENotaRequest request)
        {
            var response = new ENotaResponse();
            var now = DateTime.Now;
            var reqHeader = request.Header;
            var reqDetail = request.Detail;
            var userId = reqHeader.UserId;

            var eNotaEntity = db.ENotas
                .Include("Participants")
                .Where(e => e.Id == reqDetail.Id)
                .FirstOrDefault();

            if (eNotaEntity == null)
            {
                response.Header = request.Header.Clone() as BaseHeader;
                response.Header.StatusCode = StatusCode.NOT_FOUND;
                response.Header.StatusMessage = "Could not find entity with Id = " + reqDetail.Id;
                return response;
            }

            // ENota entity
            eNotaEntity.DocumentNo = reqDetail.DocumentNo;
            eNotaEntity.WorkUnitCode = reqDetail.WorkUnitCode;
            eNotaEntity.InternalRefNo = reqDetail.InternalRefNo;
            eNotaEntity.Subject = reqDetail.Subject;
            eNotaEntity.Kepada = reqDetail.Kepada;
            eNotaEntity.Tembusan = reqDetail.Tembusan;
            eNotaEntity.HPSAmount = reqDetail.HPSAmount;
            eNotaEntity.Owner = userId;
            eNotaEntity.ModifiedBy = userId.ToString();
            eNotaEntity.ModifiedDate = now;

            // drop existing participants before updating
            var participants = db.ParticipantsNota
                .Where(e => e.ENotaId == eNotaEntity.Id)
                .ToList();
            db.ParticipantsNota.RemoveRange(participants);

            // Participants entitites Reviewers
            var order = 0;
            foreach (var item in reqDetail.Validators)
            {
                var eNotaParticipantEntity = new Reston.Eproc.Model.ENota.ParticipantNota()
                {
                    Id = Guid.NewGuid(),
                    ENotaId = eNotaEntity.Id,
                    ENota = eNotaEntity,
                    UserId = item.PersonelUserId,
                    EmployeeName = item.PersonelFullName,
                    ParticipantRole = ParticipantRoleNota.ENOTA_PTCP_REVIEWER,
                    Ordered = order,
                };
                eNotaEntity.Participants.Add(eNotaParticipantEntity);
                order++;
            }

            // Participants entitites Approvers
            order = 0;
            foreach (var item in reqDetail.Approvers)
            {
                var eNotaParticipantEntity = new Reston.Eproc.Model.ENota.ParticipantNota()
                {
                    Id = Guid.NewGuid(),
                    ENotaId = eNotaEntity.Id,
                    ENota = eNotaEntity,
                    UserId = item.PersonelUserId,
                    EmployeeName = item.PersonelFullName,
                    ParticipantRole = ParticipantRoleNota.ENOTA_PTCP_APPROVER,
                    Ordered = order,
                };
                eNotaEntity.Participants.Add(eNotaParticipantEntity);
                order++;
            }

            // Participants entitites Approvers
            order = 0;
            foreach (var item in reqDetail.AppDireksi)
            {
                var eNotaParticipantEntity = new Reston.Eproc.Model.ENota.ParticipantNota()
                {
                    Id = Guid.NewGuid(),
                    ENotaId = eNotaEntity.Id,
                    ENota = eNotaEntity,
                    UserId = item.PersonelUserId,
                    EmployeeName = item.PersonelFullName,
                    ParticipantRole = ParticipantRoleNota.ENOTA_PTCP_DIREKTUR,
                    Ordered = order,
                };
                eNotaEntity.Participants.Add(eNotaParticipantEntity);
                order++;
            }

            // drop existing attachments before updating
            var eNotaAttachments = db.ENotaAttachments
                .Include("Attachment")
                .Where(e => e.ENotaId == eNotaEntity.Id)
                .ToList();
            var attachments = eNotaAttachments.Select(e => e.Attachment).ToList();
            db.ENotaAttachments.RemoveRange(eNotaAttachments);
            db.AttachmentsNota.RemoveRange(attachments);


            // ENotaAttachments main enota document 
            order = 0;
            // there's only single main enota document
            var attachmentEntity = new Reston.Eproc.Model.ENota.AttachmentNota()
            {
                Id = Guid.NewGuid(),
                ContentType = reqDetail.ContentType,
                ContentData = Convert.FromBase64String(reqDetail.ContentData),
                Title = reqDetail.Subject,
            };
            var eNotaAttachmentEntity = new Eproc.Model.ENota.ENotaAttachment()
            {
                Id = Guid.NewGuid(),
                AttachmentId = attachmentEntity.Id,
                Attachment = attachmentEntity,
                ENotaId = eNotaEntity.Id,
                ENota = eNotaEntity,
                AttachmentType = AttachmentTypeNota.ENOTA_ATT_NOTA,
                Ordered = order
            };
            eNotaEntity.ENotaAttachments.Add(eNotaAttachmentEntity);

            // ENotaAttachments quotation/penawaran documents 
            order = 0;
            string AttactmentPenawaran = "";
            string AttactmentPenawaranType = "";
            byte[] AttactmentPenawaranFiles = null;
            foreach (var item in reqDetail.PenawaranAttachments)
            {
                AttactmentPenawaran = item.Name;
                AttactmentPenawaranType = item.ContentType;
                AttactmentPenawaranFiles = Convert.FromBase64String(item.Data);
                attachmentEntity = new Reston.Eproc.Model.ENota.AttachmentNota()
                {
                    Id = Guid.NewGuid(),
                    ContentType = item.ContentType,
                    ContentData = Convert.FromBase64String(item.Data),
                    Title = item.Name,
                };

                eNotaAttachmentEntity = new Eproc.Model.ENota.ENotaAttachment()
                {
                    Id = Guid.NewGuid(),
                    AttachmentId = attachmentEntity.Id,
                    Attachment = attachmentEntity,
                    ENotaId = eNotaEntity.Id,
                    ENota = eNotaEntity,
                    AttachmentType = AttachmentTypeNota.ENOTA_ATT_QUOTATION,
                    Ordered = order
                };

                eNotaEntity.ENotaAttachments.Add(eNotaAttachmentEntity);
                order++;
            }

            // ENotaAttachments cost benef analysis documents
            order = 0;
            string AttachtmentAnalisa = "";
            string AttachtmentAnalisaType = "";
            byte[] AttachtmentAnalisaFile = null;
            foreach (var item in reqDetail.CostBenefitAttachments)
            {
                AttachtmentAnalisa = item.Name;
                AttachtmentAnalisaType = item.ContentType;
                AttachtmentAnalisaFile = Convert.FromBase64String(item.Data);
                attachmentEntity = new Reston.Eproc.Model.ENota.AttachmentNota()
                {
                    Id = Guid.NewGuid(),
                    ContentType = item.ContentType,
                    ContentData = Convert.FromBase64String(item.Data),
                    Title = item.Name,
                };

                eNotaAttachmentEntity = new Eproc.Model.ENota.ENotaAttachment()
                {
                    Id = Guid.NewGuid(),
                    AttachmentId = attachmentEntity.Id,
                    Attachment = attachmentEntity,
                    ENotaId = eNotaEntity.Id,
                    ENota = eNotaEntity,
                    AttachmentType = AttachmentTypeNota.ENOTA_ATT_COSTBENEFIT,
                    Ordered = order
                };

                eNotaEntity.ENotaAttachments.Add(eNotaAttachmentEntity);
                order++;
            }

            // ENotaAttachments other documents
            order = 0;
            string AttachmentLampiran = "";
            string AttachmentLampiranType = "";
            byte[] AttachmentLampiranFile = null;
            foreach (var item in reqDetail.OtherAttachments)
            {
                AttachmentLampiran = item.Name;
                AttachmentLampiranType = item.ContentType;
                AttachmentLampiranFile = Convert.FromBase64String(item.Data);
                attachmentEntity = new Reston.Eproc.Model.ENota.AttachmentNota()
                {
                    Id = Guid.NewGuid(),
                    ContentType = item.ContentType,
                    ContentData = Convert.FromBase64String(item.Data),
                    Title = item.Name,
                };

                eNotaAttachmentEntity = new Eproc.Model.ENota.ENotaAttachment()
                {
                    Id = Guid.NewGuid(),
                    AttachmentId = attachmentEntity.Id,
                    Attachment = attachmentEntity,
                    ENotaId = eNotaEntity.Id,
                    ENota = eNotaEntity,
                    AttachmentType = AttachmentTypeNota.ENOTA_ATT_OTHER,
                    Ordered = order
                };

                eNotaEntity.ENotaAttachments.Add(eNotaAttachmentEntity);
                order++;
            }

            var logEntity = new Reston.Eproc.Model.ENota.ENotaLogs
            {
                Id = Guid.NewGuid(),
                ENotaId = eNotaEntity.Id, // pick the correct FK
                Version = 1,                                   // first version
                Content = "UPDATE", // compact JSON snapshot
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
            db.ENotaLogs.Add(logEntity);            // track the log row

            db.SaveChanges(userId);

            response.Header = request.Header.Clone() as BaseHeader;
            response.Header.StatusCode = StatusCode.SUCCESS;
            response.Detail = new ENotaResponseDetail(reqDetail);
            response.Detail.Id = eNotaEntity.Id;
            response.Detail.DocumentNo = eNotaEntity.DocumentNo;

            return response;
        }

        public ENotaApprovalWorkflowResponse CreateApprovalWorkflow(ENotaApprovalWorkflowRequest request)
        {
            ENotaApprovalWorkflowResponse response = null;
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
                    var eNotaRequest = new ENotaRequest()
                    {
                        Header = request.Header.Clone() as BaseHeader,
                        Detail = reqDetail.ENotaRequestDetail
                    };
                    ENotaResponse eNotaResponse = null;
                    if (eNotaRequest.Detail.Id == null)
                    {
                        // if the request does not carry an entity Id,
                        // treat it as 'Create' request
                        eNotaResponse = Create(eNotaRequest);
                    }
                    else
                    {
                        // otherwise, treat it as 'Update' request
                        eNotaResponse = Update(eNotaRequest);
                    }

                    //
                    // 2. Create the approval workflow and it's details
                    //
                    if (eNotaResponse.Header?.StatusCode != StatusCode.SUCCESS)
                    {
                        // if previous operations was not sucessfull, 
                        // then stop. There is no point starting a workflow 
                        // on an invalid or nonexistent document
                        response = new ENotaApprovalWorkflowResponse()
                        {
                            Header = eNotaResponse.Header?.Clone() as BaseHeader,
                        };
                        return response;
                    }

                    var eNotaDoc = db.ENotas.Where(e => e.Id == eNotaResponse.Detail.Id).FirstOrDefault();


                    if (eNotaDoc.IsDraft == 1)
                    {


                        var wfHeader = new ApprovalWorkflowNota()
                        {
                            Id = Guid.NewGuid(),
                            ENotaId = eNotaDoc.Id,
                            WorkflowState = ApprovalWorkflowStateNota.BEGIN,
                            ApprovalWorkflowDetails = new List<ApprovalWorkflowDetailNota>(),
                        };

                        foreach (var personel in eNotaDoc.Participants)
                        {
                            var wfDetail = new ApprovalWorkflowDetailNota()
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

                        db.ApprovalWorkflowsNota.Add(wfHeader);

                        // mark the main eNota document as no longer a draft
                        eNotaDoc.IsDraft = 0;

                        db.SaveChanges(request.Header.UserId);
                        tx.Commit();

                        response = new ENotaApprovalWorkflowResponse()
                        {
                            Header = request.Header?.Clone() as BaseHeader,
                            Detail = new ENotaApprovalWorkflowResponseDetail()
                            {
                                Id = wfHeader.Id.ToString(),
                                ENotaId = eNotaDoc.Id.ToString(),
                                WorkflowStatus = wfHeader.WorkflowState.ToString(),
                            }
                        };
                        response.Header.StatusCode = StatusCode.SUCCESS;

                    }
                    else
                    {

                        var eNotaEntity = db.ApprovalWorkflowsNota
                        .Where(e => e.ENotaId == eNotaResponse.Detail.Id)
                        .FirstOrDefault();

                        var wfHeader = new ApprovalWorkflowNota()
                        {
                            Id = eNotaEntity.Id,
                            ENotaId = eNotaDoc.Id,
                            WorkflowState = ApprovalWorkflowStateNota.VALIDATORS_FULLY_APPROVE,
                            ApprovalWorkflowDetails = new List<ApprovalWorkflowDetailNota>(),
                        };

                        var workflow = db.ApprovalWorkflowsNota.FirstOrDefault(wf =>
                            wf.ENotaId == eNotaEntity.ENotaId &&
                            wf.WorkflowState == ApprovalWorkflowStateNota.REVISION
                        );

                        if (workflow != null)
                        {
                            db.ApprovalWorkflowsNota.Remove(workflow);
                            db.SaveChanges();
                        }

                        foreach (var personel in eNotaDoc.Participants)
                        {
                            var xperson = db.ApprovalWorkflowDetailsNota.FirstOrDefault(wf =>
                            wf.ApprovalWorkflowId == eNotaEntity.Id &&
                            wf.ApprovalDecission == ApprovalDecissionNota.REVISION);

                            var xeperson = db.ApprovalWorkflowDetailsNota.FirstOrDefault(wf =>
                            wf.ApprovalWorkflowId == eNotaEntity.Id &&
                            wf.ApprovalDecission == ApprovalDecissionNota.REVISION);



                            if (xperson != null)
                            {

                                var wfDetail = new ApprovalWorkflowDetailNota()
                                {
                                    Id = Guid.NewGuid(),
                                    ApprovalWorkflowId = wfHeader.Id,
                                    ParticipantId = xeperson.ParticipantId,
                                    ParticipantUserId = xeperson.ParticipantUserId,
                                    ParticipantName = xeperson.ParticipantName,
                                    ParticipantRole = xeperson.ParticipantRole,
                                    ApprovalDecission = null,
                                    ApprovalNotes = null,
                                };
                                wfHeader.ApprovalWorkflowDetails.Add(wfDetail);

                                db.ApprovalWorkflowDetailsNota.Remove(xperson);
                                db.SaveChanges();

                            }


                        }

                        db.ApprovalWorkflowsNota.Add(wfHeader);

                        eNotaDoc.IsDraft = 0;

                        db.SaveChanges(request.Header.UserId);
                        tx.Commit();

                        response = new ENotaApprovalWorkflowResponse()
                        {
                            Header = request.Header?.Clone() as BaseHeader,
                            Detail = new ENotaApprovalWorkflowResponseDetail()
                            {
                                Id = eNotaEntity.Id.ToString(),
                                ENotaId = eNotaDoc.Id.ToString(),
                                WorkflowStatus = eNotaEntity.WorkflowState.ToString(),
                            }
                        };
                        response.Header.StatusCode = StatusCode.SUCCESS;

                    }


                }
                catch (Exception ex)
                {
                    _log.Error(ex,
                        "Problem occured when attempting to do this action ${serviceClass}.{serviceMethod}",
                        this.GetType().ToString(),
                        "CreateENotaApproval(ENotaApprovalRequest)");

                    tx.Rollback();
                    response = new ENotaApprovalWorkflowResponse()
                    {
                        Header = request.Header?.Clone() as BaseHeader,
                    };
                    response.Header.StatusCode = StatusCode.SERVICE_ERROR;
                    response.Header.StatusMessage = ex.Message;

                }
            }

            return response;

        }

        public ENotaApprovalWorkflowDetailResponse SearchApprovalWorkflowDetails(ENotaApprovalWorkflowDetailRequest request)
        {
            var reqHeader = request.Header;
            var reqDetail = request.Detail;
            var reqFilter = request.Filter;
            var userId = reqHeader.UserId;
            var userIdGuid = Guid.Parse(reqHeader.UserId);

            // These are WF states where reviewers can see the approval requests
            var reviewerWorkflowStates = new[] {
                ApprovalWorkflowStateNota.BEGIN,
                ApprovalWorkflowStateNota.VALIDATORS_PARTIALLY_APPROVE,
                ApprovalWorkflowStateNota.VALIDATORS_FULLY_APPROVE,
                ApprovalWorkflowStateNota.APPROVERS_PARTIALLY_APPROVE,
                ApprovalWorkflowStateNota.APPROVERS_FULLY_APPROVE,
                ApprovalWorkflowStateNota.CANCELLED,
            };
            // These are WF states where approvers can see the approval requests
            var approverWorkflowStates = new[] {
                ApprovalWorkflowStateNota.VALIDATORS_FULLY_APPROVE,
                ApprovalWorkflowStateNota.APPROVERS_PARTIALLY_APPROVE,
                ApprovalWorkflowStateNota.APPROVERS_FULLY_APPROVE,
                ApprovalWorkflowStateNota.FINAL_APPROVERS_PARTIALLY_APPROVE,
                ApprovalWorkflowStateNota.FINAL_APPROVERS_FULLY_APPROVE,
                ApprovalWorkflowStateNota.CANCELLED,
            };
            var result = (from wfDetail in db.ApprovalWorkflowDetailsNota
                          join wfHeader in db.ApprovalWorkflowsNota on wfDetail.ApprovalWorkflowId equals wfHeader.Id
                          join eNota in db.ENotas on wfHeader.ENotaId equals eNota.Id
                          join reff in db.ReferenceDatas on eNota.WorkUnitCode equals reff.Code
                          join userAccount in db.VUserAccounts on eNota.Owner equals userAccount.Id.ToString()
                          where reff.Qualifier == RefDataQualifier.UnitKerja
                            && wfDetail.ParticipantUserId == userIdGuid
                            && wfHeader.WorkflowState.HasValue
                            && ((wfDetail.ParticipantRole == ParticipantRoleNota.ENOTA_PTCP_REVIEWER && reviewerWorkflowStates.Contains(wfHeader.WorkflowState.Value))
                                || (wfDetail.ParticipantRole == ParticipantRoleNota.ENOTA_PTCP_APPROVER && approverWorkflowStates.Contains(wfHeader.WorkflowState.Value)) || (wfDetail.ParticipantRole == ParticipantRoleNota.ENOTA_PTCP_DIREKTUR && approverWorkflowStates.Contains(wfHeader.WorkflowState.Value)))
                          select new ENotaApprovalWorkflowDetailResponseDetail()
                          {
                              Id = wfDetail.Id.ToString(),
                              ENotaId = eNota.Id.ToString(),
                              Subject = eNota.Subject,
                              DocumentNo = eNota.DocumentNo,
                              WorkUnitCode = eNota.WorkUnitCode,
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


            var response = new ENotaApprovalWorkflowDetailResponse()
            {
                Header = reqHeader.Clone() as BaseHeader,
                Filter = reqFilter,
                Details = result,
            };
            response.Header.StatusCode = StatusCode.SUCCESS;

            return response;

        }

        public ENotaApprovalWorkflowDetailResponse GetApprovalWorkflowDetails(ENotaApprovalWorkflowDetailRequest request)
        {
            var reqHeader = request.Header;
            var reqDetail = request.Detail;
            var userId = reqHeader.UserId;
            var userIdGuid = Guid.Parse(reqHeader.UserId);
            var approvalRequestGuid = Guid.Parse(reqDetail.Id);
            ENotaApprovalWorkflowDetailResponse response = null;

            var doc = db.ApprovalWorkflowDetailsNota
                .Include("ApprovalWorkflow")
                .Include("ApprovalWorkflow.ENota")
                .Where(e => e.Id == approvalRequestGuid && e.ParticipantUserId == userIdGuid)
                .FirstOrDefault();

            if (doc == null)
            {
                response = new ENotaApprovalWorkflowDetailResponse()
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
                .Select(e => new ENotaApprovalWorkflowDetailResponseDetail()
                {
                    Id = e.Id.ToString(),
                    ApproverPersonelName = e.ApproverPersonelName,
                    ApproverPersonelPosition = "", // no position info just yet
                    ApprovalDecission = e.ApprovalDecission.HasValue ? ((int)e.ApprovalDecission.Value).ToString() : "",
                    ApprovalNotes = e.ApprovalNotes,
                    ApprovalDecissionDate = e.ApprovalDecissionDate,
                })
                .ToList();

            var resDetail = new ENotaApprovalWorkflowDetailResponseDetail()
            {
                Id = doc.Id.ToString(),
                ENotaId = doc.ApprovalWorkflow.ENotaId.ToString(),
                ApproverPersonelName = approverDisplayName,
                ApproverUserId = doc.ParticipantUserId.ToString(),
                ApproverPersonelPosition = "", // no position info just yet
                ApprovalDecission = doc.ApprovalDecission.ToString(),
                ApprovalNotes = doc.ApprovalNotes,
                ApprovalDecissionDate = doc.ApprovalDecissionDate,
                OtherApprovalRequests = otherApprovalRequests
                // No need for other fields
            };

            response = new ENotaApprovalWorkflowDetailResponse()
            {
                Header = request.Header.Clone() as BaseHeader,
                Detail = resDetail
            };

            return response;

        }

        public ENotaApprovalWorkflowDetailResponse UpdateApprovalWorkflowDetail(ENotaApprovalWorkflowDetailRequest request)
        {
            ENotaApprovalWorkflowDetailResponse response = null;
            var reqHeader = request.Header;
            var reqDetail = request.Detail;
            var userId = reqHeader.UserId;
            var userIdGuid = Guid.Parse(reqHeader.UserId);
            var approvalRequestGuid = Guid.Parse(reqDetail.Id);

            //
            // Fetch the wf detail
            //
            var wfDetail = db.ApprovalWorkflowDetailsNota
                .Include("ApprovalWorkflow")
                .Where(e => e.Id == approvalRequestGuid)
                .FirstOrDefault();

            if (wfDetail == null)
            {
                response = new ENotaApprovalWorkflowDetailResponse()
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
            if (wfDetail.ApprovalWorkflow != null && wfDetail.ApprovalWorkflow.WorkflowState == ApprovalWorkflowStateNota.CANCELLED)
            {
                response = new ENotaApprovalWorkflowDetailResponse()
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
                response = new ENotaApprovalWorkflowDetailResponse()
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
                ApprovalWorkflowStateNota.BEGIN,
                ApprovalWorkflowStateNota.VALIDATORS_PARTIALLY_APPROVE,
            };
            // These are valid WF states where approvers are allowed to make change
            var approverWorkflowStates = new[] {
                ApprovalWorkflowStateNota.VALIDATORS_FULLY_APPROVE,
                ApprovalWorkflowStateNota.APPROVERS_PARTIALLY_APPROVE,
            };

            if (wfDetail.ParticipantRole == ParticipantRoleNota.ENOTA_PTCP_REVIEWER
                && wfDetail.ApprovalWorkflow.WorkflowState.HasValue
                && !reviewerWorkflowStates.Contains(wfDetail.ApprovalWorkflow.WorkflowState.Value))
            {
                response = new ENotaApprovalWorkflowDetailResponse()
                {
                    Header = reqHeader.Clone() as BaseHeader
                };
                response.Header.StatusCode = StatusCode.BAD_REQUEST;
                response.Header.StatusMessage = "Invalid workflow state Id = " + reqDetail.Id;
                return response;
            }
            if (wfDetail.ParticipantRole == ParticipantRoleNota.ENOTA_PTCP_APPROVER
                && wfDetail.ApprovalWorkflow.WorkflowState.HasValue
                && !approverWorkflowStates.Contains(wfDetail.ApprovalWorkflow.WorkflowState.Value))
            {
                response = new ENotaApprovalWorkflowDetailResponse()
                {
                    Header = reqHeader.Clone() as BaseHeader
                };
                response.Header.StatusCode = StatusCode.BAD_REQUEST;
                response.Header.StatusMessage = "Invalid workflow state Id = " + reqDetail.Id;
                return response;
            }

            if (!Enum.TryParse(reqDetail.ApprovalDecission, out ApprovalDecissionNota approvalDecission))
            {
                response = new ENotaApprovalWorkflowDetailResponse()
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

                    var wxDetail = db.ApprovalWorkflowsNota
                   .Where(e => e.Id == wfDetail.ApprovalWorkflowId)
                   .FirstOrDefault();

                    switch (wfDetail.ApprovalWorkflow.WorkflowState)
                    {
                        case ApprovalWorkflowStateNota.BEGIN:
                            if (approvalDecission == ApprovalDecissionNota.REJECT)
                            {
                                // if response is REJECT, then transition to REJECTED
                                wfDetail.ApprovalWorkflow.WorkflowState = ApprovalWorkflowStateNota.REJECTED;
                                break;
                            }
                            else if (approvalDecission == ApprovalDecissionNota.REVISION)
                            {
                                // if response is REJECT, then transition to REJECTED
                                wfDetail.ApprovalWorkflow.WorkflowState = ApprovalWorkflowStateNota.REVISION;
                                break;

                            }
                            else if (approvalDecission == ApprovalDecissionNota.CONTINUE_WITH_NOTES)
                            {
                                // if response is CONTINUE_WITH_NOTES, then...
                                bool allApprove = wfDetail.ApprovalWorkflow.ApprovalWorkflowDetails
                                    .Where(e => e.ParticipantRole == ParticipantRoleNota.ENOTA_PTCP_REVIEWER)
                                    .All(e => e.ApprovalDecission == ApprovalDecissionNota.APPROVE || e.ApprovalDecission == ApprovalDecissionNota.CONTINUE_WITH_NOTES);

                                if (allApprove)
                                {
                                    // ... if all validators have approved, then transition to VALIDATORS_FULLY_APPROVE
                                    wfDetail.ApprovalWorkflow.WorkflowState = ApprovalWorkflowStateNota.VALIDATORS_FULLY_APPROVE;
                                    break;
                                }
                                else
                                {
                                    // ... else transition to VALIDATORS_PARTIALLY_APPROVE
                                    wfDetail.ApprovalWorkflow.WorkflowState = ApprovalWorkflowStateNota.VALIDATORS_PARTIALLY_APPROVE;
                                    break;

                                }
                            }
                            else if (approvalDecission == ApprovalDecissionNota.APPROVE)
                            {
                                // if response is APPROVE, then...
                                bool allApprove = wfDetail.ApprovalWorkflow.ApprovalWorkflowDetails
                                    .Where(e => e.ParticipantRole == ParticipantRoleNota.ENOTA_PTCP_REVIEWER)
                                    .All(e => e.ApprovalDecission == ApprovalDecissionNota.APPROVE || e.ApprovalDecission == ApprovalDecissionNota.CONTINUE_WITH_NOTES);

                                if (allApprove)
                                {
                                    // ... if all validators have approved, then transition to VALIDATORS_FULLY_APPROVE
                                    wfDetail.ApprovalWorkflow.WorkflowState = ApprovalWorkflowStateNota.VALIDATORS_FULLY_APPROVE;
                                    break;
                                }
                                else
                                {
                                    // ... else transition to VALIDATORS_PARTIALLY_APPROVE
                                    wfDetail.ApprovalWorkflow.WorkflowState = ApprovalWorkflowStateNota.VALIDATORS_PARTIALLY_APPROVE;
                                    break;
                                }
                            }

                            // All else, dont transition state.
                            break;

                        case ApprovalWorkflowStateNota.VALIDATORS_PARTIALLY_APPROVE:
                            if (approvalDecission == ApprovalDecissionNota.REJECT)
                            {
                                // if response is REJECT, then transition to REJECTED
                                wfDetail.ApprovalWorkflow.WorkflowState = ApprovalWorkflowStateNota.REJECTED;
                                break;
                            }
                            else if (approvalDecission == ApprovalDecissionNota.REVISION)
                            {
                                // if response is REJECT, then transition to REJECTED
                                wfDetail.ApprovalWorkflow.WorkflowState = ApprovalWorkflowStateNota.REVISION;
                                break;

                            }
                            else if (approvalDecission == ApprovalDecissionNota.CONTINUE_WITH_NOTES)
                            {
                                // if response is CONTINUE_WITH_NOTES, then...
                                bool allApprove = wfDetail.ApprovalWorkflow.ApprovalWorkflowDetails
                                    .Where(e => e.ParticipantRole == ParticipantRoleNota.ENOTA_PTCP_REVIEWER)
                                    .All(e => e.ApprovalDecission == ApprovalDecissionNota.APPROVE || e.ApprovalDecission == ApprovalDecissionNota.CONTINUE_WITH_NOTES);

                                if (allApprove)
                                {
                                    // ... if all validators have approved, then transition to VALIDATORS_FULLY_APPROVE
                                    wfDetail.ApprovalWorkflow.WorkflowState = ApprovalWorkflowStateNota.VALIDATORS_FULLY_APPROVE;
                                    break;
                                }
                                else
                                {
                                    // ... else transition to VALIDATORS_PARTIALLY_APPROVE
                                    wfDetail.ApprovalWorkflow.WorkflowState = ApprovalWorkflowStateNota.VALIDATORS_PARTIALLY_APPROVE;
                                    break;

                                }
                            }
                            else if (approvalDecission == ApprovalDecissionNota.APPROVE)
                            {
                                // if response is APPROVE, then...
                                bool allApprove = wfDetail.ApprovalWorkflow.ApprovalWorkflowDetails
                                    .Where(e => e.ParticipantRole == ParticipantRoleNota.ENOTA_PTCP_REVIEWER)
                                    .All(e => e.ApprovalDecission == ApprovalDecissionNota.APPROVE || e.ApprovalDecission == ApprovalDecissionNota.CONTINUE_WITH_NOTES);

                                if (allApprove)
                                {
                                    // ... if all validators have approved, then transition to VALIDATORS_FULLY_APPROVE
                                    wfDetail.ApprovalWorkflow.WorkflowState = ApprovalWorkflowStateNota.VALIDATORS_FULLY_APPROVE;
                                    break;
                                }
                                else
                                {
                                    // ... else transition to VALIDATORS_PARTIALLY_APPROVE
                                    wfDetail.ApprovalWorkflow.WorkflowState = ApprovalWorkflowStateNota.VALIDATORS_PARTIALLY_APPROVE;
                                    break;
                                }
                            }

                            // All else, dont transition state.
                            break;
                        case ApprovalWorkflowStateNota.VALIDATORS_FULLY_APPROVE:
                            if (approvalDecission == ApprovalDecissionNota.REJECT)
                            {
                                // if response is REJECT, then transition to REJECTED
                                wfDetail.ApprovalWorkflow.WorkflowState = ApprovalWorkflowStateNota.REJECTED;
                                break;
                            }
                            else if (approvalDecission == ApprovalDecissionNota.REVISION)
                            {
                                // if response is REJECT, then transition to REJECTED
                                wfDetail.ApprovalWorkflow.WorkflowState = ApprovalWorkflowStateNota.REVISION;
                                break;

                            }
                            else if (approvalDecission == ApprovalDecissionNota.CONTINUE_WITH_NOTES)
                            {
                                // if response is CONTINUE_WITH_NOTES, then...
                                bool allApprove = wfDetail.ApprovalWorkflow.ApprovalWorkflowDetails
                                    .Where(e => e.ParticipantRole == ParticipantRoleNota.ENOTA_PTCP_APPROVER)
                                    .All(e => e.ApprovalDecission == ApprovalDecissionNota.APPROVE || e.ApprovalDecission == ApprovalDecissionNota.CONTINUE_WITH_NOTES);

                                if (allApprove)
                                {
                                    // ... if all approvers have approved, then transition to APPROVERS_FULLY_APPROVE
                                    wfDetail.ApprovalWorkflow.WorkflowState = ApprovalWorkflowStateNota.APPROVERS_FULLY_APPROVE;
                                    break;
                                }
                                else
                                {
                                    // ... else transition to APPROVERS_PARTIALLY_APPROVE
                                    wfDetail.ApprovalWorkflow.WorkflowState = ApprovalWorkflowStateNota.APPROVERS_PARTIALLY_APPROVE;
                                    break;

                                }
                            }
                            else if (approvalDecission == ApprovalDecissionNota.APPROVE)
                            {
                                // if response is APPROVE, then...
                                bool allApprove = wfDetail.ApprovalWorkflow.ApprovalWorkflowDetails
                                    .Where(e => e.ParticipantRole == ParticipantRoleNota.ENOTA_PTCP_DIREKTUR)
                                    .All(e => e.ApprovalDecission != ApprovalDecissionNota.APPROVE);


                                if (allApprove == true)
                                {
                                    // ... if all approvers have approved, then transition to APPROVERS_FULLY_APPROVE (BUGS FOUND)
                                    wfDetail.ApprovalWorkflow.WorkflowState = ApprovalWorkflowStateNota.APPROVERS_PARTIALLY_APPROVE;
                                    break;
                                }
                                else
                                {
                                    // ... else transition to APPROVERS_PARTIALLY_APPROVE
                                    wfDetail.ApprovalWorkflow.WorkflowState = ApprovalWorkflowStateNota.APPROVERS_FULLY_APPROVE;
                                    break;
                                }
                            }

                            // All else, dont transition state.
                            break;
                        case ApprovalWorkflowStateNota.APPROVERS_PARTIALLY_APPROVE:
                            if (approvalDecission == ApprovalDecissionNota.REJECT)
                            {
                                // if response is REJECT, then transition to REJECTED
                                wfDetail.ApprovalWorkflow.WorkflowState = ApprovalWorkflowStateNota.REJECTED;
                                break;
                            }
                            else if (approvalDecission == ApprovalDecissionNota.REVISION)
                            {
                                // if response is REJECT, then transition to REJECTED
                                wfDetail.ApprovalWorkflow.WorkflowState = ApprovalWorkflowStateNota.REVISION;
                                break;

                            }
                            else if (approvalDecission == ApprovalDecissionNota.CONTINUE_WITH_NOTES)
                            {
                                // if response is CONTINUE_WITH_NOTES, then...
                                bool allApprove = wfDetail.ApprovalWorkflow.ApprovalWorkflowDetails
                                    .Where(e => e.ParticipantRole == ParticipantRoleNota.ENOTA_PTCP_APPROVER)
                                    .All(e => e.ApprovalDecission == ApprovalDecissionNota.APPROVE || e.ApprovalDecission == ApprovalDecissionNota.CONTINUE_WITH_NOTES);

                                if (allApprove)
                                {
                                    // ... if all approvers have approved, then transition to APPROVERS_FULLY_APPROVE
                                    wfDetail.ApprovalWorkflow.WorkflowState = ApprovalWorkflowStateNota.APPROVERS_FULLY_APPROVE;
                                    break;
                                }
                                else
                                {
                                    // ... else transition to APPROVERS_PARTIALLY_APPROVE
                                    wfDetail.ApprovalWorkflow.WorkflowState = ApprovalWorkflowStateNota.APPROVERS_PARTIALLY_APPROVE;
                                    break;

                                }
                            }
                            else if (approvalDecission == ApprovalDecissionNota.APPROVE)
                            {
                                // if response is APPROVE, then...
                                bool allApprove = wfDetail.ApprovalWorkflow.ApprovalWorkflowDetails
                                    .Where(e => e.ParticipantRole == ParticipantRoleNota.ENOTA_PTCP_APPROVER)
                                    .All(e => e.ApprovalDecission == ApprovalDecissionNota.APPROVE || e.ApprovalDecission == ApprovalDecissionNota.CONTINUE_WITH_NOTES);

                                if (allApprove)
                                {
                                    // ... if all approvers have approved, then transition to APPROVERS_FULLY_APPROVE
                                    wfDetail.ApprovalWorkflow.WorkflowState = ApprovalWorkflowStateNota.APPROVERS_FULLY_APPROVE;
                                    break;
                                }
                                else
                                {
                                    // ... else transition to APPROVERS_PARTIALLY_APPROVE
                                    wfDetail.ApprovalWorkflow.WorkflowState = ApprovalWorkflowStateNota.APPROVERS_PARTIALLY_APPROVE;
                                    break;
                                }
                            }

                            // All else, dont transition state.
                            break;
                        case ApprovalWorkflowStateNota.APPROVERS_FULLY_APPROVE:
                            // Final state. Cannot transition further
                            break;
                        case ApprovalWorkflowStateNota.REJECTED:
                            // Final state. Cannot transition further
                            break;
                        case ApprovalWorkflowStateNota.CANCELLED:
                            // Final state. Cannot transition further
                            break;
                    }

                    // 
                    // Check on the wf state and determine if we need to update it
                    //
                    if (wfDetail.ApprovalWorkflow.WorkflowState == ApprovalWorkflowStateNota.BEGIN
                        || wfDetail.ApprovalWorkflow.WorkflowState == ApprovalWorkflowStateNota.VALIDATORS_PARTIALLY_APPROVE)
                    {

                        // If we are waiting for all validators to approve and
                        // all validators has indeed been approving, then advance
                        // the wf state to VALIDATORS_FULLY_APPROVE

                        bool allReviewersApproved = wfDetail.ApprovalWorkflow.ApprovalWorkflowDetails
                            .Where(e => e.ParticipantRole == ParticipantRoleNota.ENOTA_PTCP_REVIEWER)
                            .All(e => e.ApprovalDecission == ApprovalDecissionNota.APPROVE);

                        if (allReviewersApproved)
                        {
                            wfDetail.ApprovalWorkflow.WorkflowState = ApprovalWorkflowStateNota.VALIDATORS_FULLY_APPROVE;
                        }
                    }
                    else if (wfDetail.ApprovalWorkflow.WorkflowState == ApprovalWorkflowStateNota.VALIDATORS_FULLY_APPROVE
                        || wfDetail.ApprovalWorkflow.WorkflowState == ApprovalWorkflowStateNota.APPROVERS_PARTIALLY_APPROVE)
                    {
                        // If we are waiting for all approvers to approve and
                        // all approvers has indeed been approving, then advance
                        // the wf state to APPROVERS_FULLY_APPROVE

                        bool allApproversApproved = wfDetail.ApprovalWorkflow.ApprovalWorkflowDetails
                            .Where(e => e.ParticipantRole == ParticipantRoleNota.ENOTA_PTCP_APPROVER)
                            .All(e => e.ApprovalDecission == ApprovalDecissionNota.APPROVE);

                        if (allApproversApproved)
                        {
                            wfDetail.ApprovalWorkflow.WorkflowState = ApprovalWorkflowStateNota.VALIDATORS_FULLY_APPROVE;
                        }
                    }
                    else
                    {
                        // Otherwise do nothing to the wf state
                    }

                    var status = "";

                    if (approvalDecission == ApprovalDecissionNota.APPROVE)
                    {
                        status = "APPROVE";
                    }
                    else if (approvalDecission == ApprovalDecissionNota.REJECT)
                    {
                        status = "REJECT";
                    }
                    else if (approvalDecission == ApprovalDecissionNota.REVISION)
                    {
                        status = "REVISION";
                    }
                    else if (approvalDecission == ApprovalDecissionNota.CONTINUE_WITH_NOTES)
                    {
                        status = "CONTINUE WITH NOTES";
                    }
                    var logEntity = new Reston.Eproc.Model.ENota.ENotaLogs
                    {
                        Id = Guid.NewGuid(),
                        ENotaId = wxDetail.ENotaId, // pick the correct FK
                        Version = 1,                                   // first version
                        Content = wfDetail.ParticipantName + "<br> Status: <br>" + status + "<br> Catatan: <br>" + reqDetail.ApprovalNotes, // compact JSON snapshot
                        CreateDate = DateTime.Now,

                    };
                    db.ENotaLogs.Add(logEntity);            // track the log row

                    // Save all the changes
                    db.SaveChanges(userId);
                    tx.Commit();

                    response = new ENotaApprovalWorkflowDetailResponse()
                    {
                        Header = request.Header.Clone() as BaseHeader,
                    };
                    response.Header.StatusCode = StatusCode.SUCCESS;
                }
                catch (Exception ex)
                {
                    _log.Error(ex, "Failed to update workflow response. Id = " + reqDetail.Id);
                    tx.Rollback();
                    response = new ENotaApprovalWorkflowDetailResponse()
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

        public ENotaTemplateResponse SearchENotaTemplates(ENotaTemplateRequest request)
        {
            var reqDetail = request.Detail;
            var reqFilter = request.Filter;

            var query = from template in db.ENotaTemplates
                        select template;

            reqFilter.TotalRecordsCount = query.Count();

            query = from template in query
                    where template.Title.Contains(reqFilter.Title)
                    select template;

            reqFilter.FilteredRecordsCount = query.Count();

            var results = query.OrderBy(template => template.Title)
                .Skip(reqFilter.Offset.GetValueOrDefault())
                .Take(reqFilter.PageLength.GetValueOrDefault(7))
                .Select(template => new ENotaTemplateResponseDetail()
                {
                    Id = template.Id,
                    Title = template.Title,
                    ContentType = template.ContentType,
                    //ContentData = Convert.ToBase64String(template.ContentData),
                }).ToList();

            var response = new ENotaTemplateResponse()
            {
                Header = request.Header.Clone() as BaseHeader,
                Details = results,
                Filter = reqFilter
            };

            return response;
        }

        public ENotaTemplateResponse GetENotaTemplate(ENotaTemplateRequest request)
        {
            var reqHeader = request.Header;
            var reqDetail = request.Detail;
            var docId = reqDetail.Id;
            var docIdStr = docId.ToString();
            var userIdStr = reqHeader.UserId;
            var response = new ENotaTemplateResponse()
            {
                Header = request.Header.Clone() as BaseHeader,
            };

            var entity = (from template in db.ENotaTemplates
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
            response.Detail = new ENotaTemplateResponseDetail()
            {
                Id = entity.Id,
                Title = entity.Title,
                ContentType = entity.ContentType,
                ContentData = Convert.ToBase64String(entity.ContentData)
            };

            return response;

        }

        private string GenerateDocumentNumber(ENotaRequest request)
        {
            var reqDetail = request.Detail;
            var workUnitCode = reqDetail.WorkUnitCode;
            var now = DateTime.Now;
            var result = "";


            // cari nomor dokumen terakhir untuk unit kerja tersebut
            var existingWorkUnitDocNumber = db.DocumentNumbersNota
                .Where(e => e.WorkUnitCode == workUnitCode)
                .FirstOrDefault();

            if (existingWorkUnitDocNumber == null)
            {
                // jika tidak ditemukan, maka buat nomor perdana, simpan, dan kembalikan
                var newDocumentNumber = ENOTA_DOC_NO_TEMPLATE;
                newDocumentNumber = newDocumentNumber.Replace("{SEQNO}", "001");
                newDocumentNumber = newDocumentNumber.Replace("{WORK_UNIT_CODE}", workUnitCode);
                newDocumentNumber = newDocumentNumber.Replace("{MONTH_2_DIGITS}", now.ToString("MM"));
                newDocumentNumber = newDocumentNumber.Replace("{YEAR_4_DIGITS}", now.ToString("yyyy"));

                db.DocumentNumbersNota.Add(new DocumentNumberNota()
                {
                    Id = Guid.NewGuid(),
                    WorkUnitCode = workUnitCode,
                    LastENotaDocNo = newDocumentNumber
                });
                db.SaveChanges();

                result = newDocumentNumber;
            }
            else
            {
                // jika ditemukan

                var lastNumber = existingWorkUnitDocNumber.LastENotaDocNo;
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
                var newDocumentNumber = ENOTA_DOC_NO_TEMPLATE;
                newDocumentNumber = newDocumentNumber.Replace("{SEQNO}", nextSeq);
                newDocumentNumber = newDocumentNumber.Replace("{WORK_UNIT_CODE}", workUnitCode);
                newDocumentNumber = newDocumentNumber.Replace("{MONTH_2_DIGITS}", thisMonth);
                newDocumentNumber = newDocumentNumber.Replace("{YEAR_4_DIGITS}", thisYear);
                existingWorkUnitDocNumber.LastENotaDocNo = newDocumentNumber;
                db.SaveChanges();
                result = newDocumentNumber;

            }

            return result;
        }
    }
}
