using System.ComponentModel;
using System.Linq.Expressions;
using AutoMapper;
using FS.Commons.Attributes;

namespace FS.Commons.Extensions;

public static class AutoMapperExtension
  {
    /// <summary>
    /// Mapping with an ignore property.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TDestination"></typeparam>
    /// <param name="map"></param>
    /// <param name="selector"></param>
    /// <returns></returns>
    public static IMappingExpression<TSource, TDestination> Ignore<TSource, TDestination>(
    this IMappingExpression<TSource, TDestination> map,
    Expression<Func<TDestination, object>> selector)
    {
      map.ForMember(selector, config => config.Ignore());
      return map;
    }

    /// <summary>
    /// Mapping with a collection of ignore properties.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TDestination"></typeparam>
    /// <param name="map"></param>
    /// <param name="selectors"></param>
    /// <returns></returns>
    public static IMappingExpression<TSource, TDestination> Ignore<TSource, TDestination>(
    this IMappingExpression<TSource, TDestination> map,
    params Expression<Func<TDestination, object>>[] selectors)
    {
      for (int i = 0; i < selectors.Length; i++)
      {
        map.ForMember(selectors[i], config => config.Ignore());
      }      
      return map;
    }

    /// <summary>
    /// Auto mapping with MapperIgnoreAttribute
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TDestination"></typeparam>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static IMappingExpression<TSource, TDestination> IgnoreNoMap<TSource, TDestination>(
            this IMappingExpression<TSource, TDestination> expression)
    {
      var sourceType = typeof(TSource);
      foreach (var property in sourceType.GetProperties())
      {
        PropertyDescriptor descriptor = TypeDescriptor.GetProperties(sourceType)[property.Name];
        MapperIgnoreAttribute attribute = (MapperIgnoreAttribute)descriptor.Attributes[typeof(MapperIgnoreAttribute)];
        if (attribute != null)
          expression.ForMember(property.Name, opt => opt.Ignore());
      }
      return expression;
    }

    /// <summary>
    /// Execute simple mapper
    /// </summary>
    /// <param name="source"></param>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TDestination"></typeparam>
    /// <returns></returns>
    public static TDestination Map<TSource, TDestination>(this TSource source)
    {
      var config = new MapperConfiguration(ext =>
      {
        ext.CreateMap<TSource, TDestination>();
      });
      var mapper = config.CreateMapper();
      return mapper.Map<TSource, TDestination>(source);
    }

    /// <summary>
    /// Execute simple map with a collection
    /// </summary>
    /// <param name="source"></param>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TDestination"></typeparam>
    /// <returns></returns>
    public static List<TDestination> MapList<TSource, TDestination>(this List<TSource> source)
    {
      return source.Select(m => Map<TSource, TDestination>(m)).ToList();
    }
  }