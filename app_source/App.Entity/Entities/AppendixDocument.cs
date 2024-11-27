using FS.Commons.Models;

namespace App.Entity.Entities;

public class AppendixDocument : CommonDataModel
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? FileName { get; set; }
}