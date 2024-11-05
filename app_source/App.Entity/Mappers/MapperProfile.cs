using System.Reflection;
using AutoMapper;

namespace App.Entity.Mappers;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        var entityAssembly = Assembly.Load("App.Entity");


        var entityTypes = entityAssembly.GetTypes().Where(t => t.IsClass && t.Namespace == "App.Entity.Entities");


        foreach (var entityType in entityTypes)
        {
            var dtoTypes = entityAssembly.GetTypes()
                .Where(t => t.IsClass && t.Namespace == $"App.Entity.DTOs.{entityType.Name}" && t.Name.StartsWith(entityType.Name));

            foreach (var dtoType in dtoTypes)
            {
                CreateMap(entityType, dtoType).ReverseMap();
            }
        }
    }
}