using EmployeeManagement.Models;
using EmployeeManagement.ViewModel.Menu;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmployeeManagement.Services.Menu
{
    public class DataAccessService : IDataAccessService
    {
        private readonly AppDbContext appDbContext;

        public DataAccessService(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task<List<NavigationMenuViewModel>> GetMenuItemsAsync(ClaimsPrincipal principal)
        {
            var isAuthenticated = principal.Identity.IsAuthenticated;
            if (!isAuthenticated)
                return new List<NavigationMenuViewModel>();

            var roleIds = await GetUserRoleIds(principal);
            var data = await(from menu in appDbContext.roleMenuPermissions where roleIds.Contains(menu.RoleId)
                             select menu)
                              .Select(m => new NavigationMenuViewModel()
                              {
                                  Id = m.NavigationMenu.Id,
                                  Name = m.NavigationMenu.Name,
                                  ActionName = m.NavigationMenu.ActionName,
                                  ControllerName = m.NavigationMenu.ControllerName,
                                  Icon=m.NavigationMenu.Icon,
                                  ParentMenuId = m.NavigationMenu.ParentMenuId,
                                  Status= m.NavigationMenu.Status,
                              }).Distinct().ToListAsync();

            return data;
        }
        private async Task<List<string>> GetUserRoleIds(ClaimsPrincipal ctx)
        {
            var userId = GetUserId(ctx);
            var data = await (from role in appDbContext.UserRoles
                              where role.UserId == userId
                              select role.RoleId).ToListAsync();

            return data;
        }

        private string GetUserId(ClaimsPrincipal user)
        {
            return ((ClaimsIdentity)user.Identity).FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
