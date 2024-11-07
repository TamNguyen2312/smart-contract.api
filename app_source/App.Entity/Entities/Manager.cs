using FS.Commons.Models;

namespace App.Entity.Entities;

public class Manager : CommonDataModel
{
    public long Id { get; set; } //ref to userId
    public long DepartmentId { get; set; }
}