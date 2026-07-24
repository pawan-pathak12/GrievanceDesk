using AutoMapper;
using SmartCollegeComplaintPortal.Business.DTOs.Complaints;
using SmartCollegeComplaintPortal.Data.QueryResult;
using SmartCollegeComplaintPortal.Domain.Enums;

namespace SmartCollegeComplaintPortal.Business.Mapping
{
    public class ComplaintProfile : Profile
    {
        public ComplaintProfile()
        {
            CreateMap<ComplaintSummaryResult, ComplaintSummaryResponse>()
                .ForMember(d => d.Category, o => o.MapFrom(s => ((ComplaintCategory)s.Category).ToString()))
                .ForMember(d => d.Status, o => o.MapFrom(s => ((ComplaintStatus)s.Status).ToString()))
                .ForMember(d => d.SubmitterName, o => o.MapFrom(s => s.SubmitterUsername));

            CreateMap<ComplaintDetailResult, ComplaintDetailResponse>()
                .ForMember(d => d.Category, o => o.MapFrom(s => ((ComplaintCategory)s.Category).ToString()))
                .ForMember(d => d.Status, o => o.MapFrom(s => ((ComplaintStatus)s.Status).ToString()))
                .ForMember(d => d.SubmitterName, o => o.MapFrom(s => s.SubmitterUsername))
                .ForMember(d => d.Updates, o => o.Ignore());

            CreateMap<ComplaintUpdateResult, ComplaintUpdateResponse>()
                .ForMember(d => d.UpdateType, o => o.MapFrom(s => ((ComplaintUpdateType)s.UpdateType).ToString()))
                .ForMember(d => d.OldStatus, o => o.MapFrom(s => s.OldStatus.HasValue ? ((ComplaintStatus)s.OldStatus.Value).ToString() : null))
                .ForMember(d => d.NewStatus, o => o.MapFrom(s => s.NewStatus.HasValue ? ((ComplaintStatus)s.NewStatus.Value).ToString() : null))
                .ForMember(d => d.UpdatedByName, o => o.MapFrom(s => s.UpdatedByUsername));
        }
    }
}
