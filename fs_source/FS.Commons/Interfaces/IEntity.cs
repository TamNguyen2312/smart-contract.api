namespace FS.Common.Models.Models.Interfaces;

public interface IEntity<TEntity>
{
    TEntity GetEntity();
}