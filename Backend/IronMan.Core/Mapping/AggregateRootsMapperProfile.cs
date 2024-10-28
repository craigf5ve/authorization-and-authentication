using AutoMapper;
using IronMan.Core.Dtos.EntityDtos;
using IronMan.Data.AggregateRoots;

namespace IronMan.Core;

public class AggregateRootsMapperProfile : Profile
{
    public AggregateRootsMapperProfile()
    {
        // CreateMap<BasicAggregateRoot<int>, EntityDto<int>>();
        // CreateMap<AuditedAggregateRoot<int>, AuditedEntityDto<int>>()
        //     .IncludeBase<BasicAggregateRoot<int>, EntityDto<int>>();

        // CreateMap<FullAuditedAggregateRoot<int>, FullAuditedEntityDto<int>>()
        //     .IncludeBase<AuditedAggregateRoot<int>, AuditedEntityDto<int>>();

    }       
}
