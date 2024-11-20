using System.Reflection;
using FS.Commons.Models;

namespace FS.Commons.Extensions;

public static class EfCoreExtension
{
    public static IQueryable<T> ToPagedList<T>(this IQueryable<T> list, int pageNumber, int pageSize)
    {
        return list.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }

    public static IQueryable<T> ApplyOrderDate<T>(this IQueryable<T> source, OrderDate? orderOption)
        where T : CommonDataModel
    {
        if (orderOption.HasValue)
        {
            switch (orderOption.Value)
            {
                case OrderDate.DescesdendingCreated:
                    source = source.OrderByDescending(x => x.CreatedDate);
                    break;
                case OrderDate.IncreasingCreated:
                    source = source.OrderBy(x => x.CreatedDate);
                    break;
                case OrderDate.DescendingModified:
                    source = source.OrderByDescending(x => x.ModifiedDate ?? DateTime.MinValue);
                    break;
                case OrderDate.IncreasingModified:
                    source = source.OrderBy(x => x.ModifiedDate ?? DateTime.MaxValue);
                    break;
            }
        }
        return source;
    }

    public static IEnumerable<T> ToPagedList<T>(this IEnumerable<T> list, int pageNumber, int pageSize)
    {
        return list.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }
    
    public static void UpdateNonDefaultProperties<TSource, TTarget>(this TSource source, TTarget target)
        where TSource : class
        where TTarget : class
    {
        if (source == null || target == null) throw new ArgumentNullException();

        var sourceProperties = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var targetProperties = typeof(TTarget).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var sourceProperty in sourceProperties)
        {
            // Lấy giá trị từ thuộc tính của source
            var value = sourceProperty.GetValue(source);
            
            if (value == null || (value is string str && string.IsNullOrEmpty(str)))
            {
                // Giữ lại giá trị cũ của target, bỏ qua việc cập nhật
                continue;
            }

            // Tìm thuộc tính tương ứng trong target theo tên
            var targetProperty = Array.Find(targetProperties, p => p.Name == sourceProperty.Name);

            // Kiểm tra nếu thuộc tính tồn tại và có thể ghi
            if (targetProperty != null && targetProperty.CanWrite)
            {
                // Cập nhật giá trị từ source cho target
                targetProperty.SetValue(target, value);
            }
        }
    }
}