using FS.Commons;
using FS.Commons.Models;

namespace FS.Commons.Interfaces;

public interface IDropdownable
{
    Task<List<Dropdown>> GetDropdown();
}