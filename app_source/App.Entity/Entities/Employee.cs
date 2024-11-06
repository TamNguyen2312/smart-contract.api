using FS.Commons.Models;

namespace App.Entity.Entities;

public partial class Employee : CommonDataModel
{
    public long Id { get; set; }  //ref to userId
    public string DepartmentName { get; set; } = null!;
}