using AutoMapper;
using backend.Models;

namespace backend.Mappers;

public class MappingProfile : Profile
{
  public MappingProfile()
  {
    CreateMap<User, UserDTO>();
    CreateMap<Permission, PermissionDTO>();
  }
}