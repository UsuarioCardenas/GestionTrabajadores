using AutoMapper;
using GestiónTrabajadores.Application.DTOs;
using GestiónTrabajadores.Domain.Entities;

namespace GestiónTrabajadores.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Trabajador, TrabajadorDto>();
        CreateMap<CreateTrabajadorDto, Trabajador>();
        CreateMap<UpdateTrabajadorDto, Trabajador>();
    }
}