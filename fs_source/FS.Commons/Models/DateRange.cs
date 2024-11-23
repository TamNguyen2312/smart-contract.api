using System.ComponentModel.DataAnnotations;

namespace FS.Commons.Models;

public class DateRange
{
    [DataType(DataType.Date)]
    public DateTime? From { get; set; }
    [DataType(DataType.Date)]
    public DateTime? To { get; set; }
}