namespace FS.Commons.Models;

public class Recursively
{
    public long ParentId { get; set; }
    public long Id { get; set; }
    public string Name { get; set; }
    public int Index { get; set; }
    public bool IsDisabled { get; set; }
    public List<Recursively> Childs { get; set; }
}