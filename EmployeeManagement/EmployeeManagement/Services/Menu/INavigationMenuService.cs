using EmployeeManagement.Models;
using EmployeeManagement.Models.Menu;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeManagement.Services.Menu
{
    public interface INavigationMenuService
    {
        IEnumerable<NavigationMenu> GetMenu();
        NavigationMenu AddMenu(NavigationMenu navigationMenu);
        Task<int> UpdateMenu(NavigationMenu navigationMenu);
        Task<NavigationMenu> DeleteMenu(int id);
		IEnumerable<NavigationMenu> GetMasterMenu();
		Task<NavigationMenu> GetMasterMenuById(int id);
		Task<IEnumerable<NavigationMenu>> GetSubMenu(int id);
		Task <NavigationMenu> GetSubMenuById(int id);
        Task<IList<RoleMenuPermission>> GetRoleMenuByMenuId(int MenuId);
        Task<RoleMenuPermission> GetRoleMenu(string RoleId,int MenuId);
        RoleMenuPermission AddRoleMenu(RoleMenuPermission roleMenuPermission);
        RoleMenuPermission UpdateRoleMenu(RoleMenuPermission roleMenuPermission);
        //public bool CheckRole(string RoleId);
        //public int GetRoleMenuId(string RoleId,int MenuId);
        public bool CheckRoleMenu(string RoleId, int MenuId);

    }
}
