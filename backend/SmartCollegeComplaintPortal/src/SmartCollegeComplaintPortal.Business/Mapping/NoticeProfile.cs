using AutoMapper;
using SmartCollegeComplaintPortal.Business.DTOs.Notices;
using SmartCollegeComplaintPortal.Domain.Entities.Notices;
using SmartCollegeComplaintPortal.Domain.Enums;

namespace SmartCollegeComplaintPortal.Business.Mapping
{
    public class NoticeProfile : Profile
    {
        public NoticeProfile()
        {
            CreateMap<CreateNoticeDto, Notice>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.IsPublished, o => o.Ignore())
                .ForMember(d => d.PublishedAt, o => o.Ignore())
                .ForMember(d => d.CreatedAt, o => o.Ignore())
                .ForMember(d => d.UpdatedAt, o => o.Ignore())
                .ForMember(d => d.CreatedBy, o => o.Ignore())
                .ForMember(d => d.UpdatedBy, o => o.Ignore())
                .ForMember(d => d.IsDeleted, o => o.Ignore());

            CreateMap<UpdateNoticeDto, Notice>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.IsPublished, o => o.Ignore())
                .ForMember(d => d.PublishedAt, o => o.Ignore())
                .ForMember(d => d.CreatedAt, o => o.Ignore())
                .ForMember(d => d.UpdatedAt, o => o.Ignore())
                .ForMember(d => d.CreatedBy, o => o.Ignore())
                .ForMember(d => d.UpdatedBy, o => o.Ignore())
                .ForMember(d => d.IsDeleted, o => o.Ignore());

            CreateMap<Notice, NoticeResponseDto>()
                .ForMember(d => d.Priority, o => o.MapFrom(s => s.Priority.ToString()));

            CreateMap<Notice, UserNoticeDto>()
                .ForMember(d => d.Priority, o => o.MapFrom(s => s.Priority.ToString()));

            CreateMap<Notice, UserNoticeDetailDto>()
                .ForMember(d => d.Priority, o => o.MapFrom(s => s.Priority.ToString()));
        }
    }
}
