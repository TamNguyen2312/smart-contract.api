using FS.Commons.Models;

namespace App.Entity.Entities;

public class Manager : CommonDataModel
{
    public string Id { get; set; } //ref to userId
    public long DepartmentId { get; set; }
}