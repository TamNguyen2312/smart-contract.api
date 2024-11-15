using System;
using FS.Commons.Models;

namespace App.Entity.Entities;

public partial class ContractType : CommonDataModel
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;

}
