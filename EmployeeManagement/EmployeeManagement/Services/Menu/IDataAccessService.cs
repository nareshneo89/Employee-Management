using EmployeeManagement.ViewModel.Menu;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmployeeManagement.Services.Menu
{
    public interface IDataAccessService
    {
        Task<List<NavigationMenuViewModel>> GetMenuItemsAsync(ClaimsPrincipal principal); 
    }
}
