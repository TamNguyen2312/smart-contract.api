using System.ComponentModel.DataAnnotations;
using FS.Commons;
using FS.Commons.Interfaces;

namespace App.Entity.DTOs.AppendixDocument;

public class AppendixDocumentRequestDTO : IEntity<Entities.AppendixDocument>
{
    public long Id { get; set; }
    
    [Required(ErrorMessage = Constants.Required)]
    public string Name { get; set; } = null!;
    
    public string? Description { get; set; }
    
    public string? FileName { get; set; }
    
    [Required(ErrorMessage = Constants.Required)]
    public long AppendixId { get; set; }

    public Entities.AppendixDocument GetEntity()
    {
        return new Entities.AppendixDocument
        {
            Id = Id,
            Name = Name,
            Description = Description,
            FileName = FileName,
            AppendixId = AppendixId
        };
    }
}