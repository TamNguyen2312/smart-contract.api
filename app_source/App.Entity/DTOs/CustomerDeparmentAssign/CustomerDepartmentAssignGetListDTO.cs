using FS.Commons.Models;

namespace App.Entity.DTOs.CustomerDeparmentAssign;

public class CustomerDepartmentAssignGetListDTO : PagingModel
{
    public long? CustomerOrDepartmentId { get; set; }
}