using System;
using AutoMapper;
using AutoMapper.Configuration;

namespace DiscordBot.AdminWeb
{
    public static class AdminMapper
    {
        private static IMapper _mapper = null;

        public static IMapper GetMapper()
        {
            if (_mapper == null)
            {
                var cfg = new MapperConfigurationExpression();

                cfg.CreateMap<Domain.Entities.Zones.Zone, ViewModels.ZoneViewModel>()
                    .ForMember(
                        dest => dest.OwnerId,
                        opt => opt.MapFrom(src => src.Owner.Id)
                        );

                var mapperConfig = new MapperConfiguration(cfg);
                _mapper = new Mapper(mapperConfig);
            }
            return _mapper;
        }
    }
}
