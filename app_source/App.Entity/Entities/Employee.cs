using FS.Commons.Models;

namespace App.Entity.Entities;

public partial class Employee : CommonDataModel
{
    public string Id { get; set; }  //ref to userId
    public long DepartmentId { get; set; }
}