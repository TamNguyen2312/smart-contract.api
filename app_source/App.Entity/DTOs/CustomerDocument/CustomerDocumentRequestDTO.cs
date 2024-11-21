using System.ComponentModel.DataAnnotations;
using FS.Commons;
using FS.Commons.Interfaces;

namespace App.Entity.DTOs.CustomerDocument;

public class CustomerDocumentRequestDTO : IEntity<Entities.CustomerDocument>
{
    public long Id { get; set; }
    
    [Required(ErrorMessage = Constants.Required)]
    public string Description { get; set; } = null!;
    
    [Required(ErrorMessage = Constants.Required)]
    [StringLength(10, ErrorMessage = Constants.MaxlengthError)]
    public string FilePath { get; set; } = null!;

    [Required(ErrorMessage = Constants.Required)]
    public long CustomerId { get; set; }


    public Entities.CustomerDocument GetEntity()
    {
        return new Entities.CustomerDocument
        {
            Id = Id,
            Description = Description,
            FilePath = FilePath,
            CustomerId = CustomerId
        };
    }
}