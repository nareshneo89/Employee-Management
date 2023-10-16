using EmployeeManagement.Models;
using EmployeeManagement.Models.Menu;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Services.Menu
{
    public class NavigationMenuService : INavigationMenuService
    {
        private readonly AppDbContext appDbContext;

        public NavigationMenuService(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }
        
        public NavigationMenu AddMenu(NavigationMenu navigationMenu)
        {
            if (navigationMenu != null)
            {
                appDbContext.navigationMenu.AddAsync(navigationMenu);

                appDbContext.SaveChangesAsync();
                return navigationMenu;
            }
            return null;
        }
        public async Task<NavigationMenu> DeleteMenu(int id)
        {
            var person = await appDbContext.navigationMenu.FindAsync(id);
            if (person is not null) appDbContext.Remove(person);
            await appDbContext.SaveChangesAsync();
            return null;
            
        }
        
        public IEnumerable<NavigationMenu> GetMasterMenu()
        {
            if (appDbContext != null)
            {
                return appDbContext.navigationMenu.Where(so => so.ParentMenuId == null).OrderBy(x => x.Position);               
            }

            return null;
        }
        public async Task<NavigationMenu> GetMasterMenuById(int id)
        {
            if (appDbContext != null)
            {
                return await appDbContext.navigationMenu.FirstOrDefaultAsync(e => e.Id == id);

            }
            return null;
        }

        public IEnumerable<NavigationMenu> GetMenu()
        {
            return appDbContext.navigationMenu;
        }

        public async Task<IList<RoleMenuPermission>> GetRoleMenuByMenuId(int MenuId)
        {
            if (appDbContext != null)
            {
                return appDbContext.roleMenuPermissions.Where(i => i.NavigationMenuId == MenuId && i.Status==true).ToList();

            }
            return null;
        }
        public async Task<RoleMenuPermission> GetRoleMenu(string RoleId, int MenuId)
        {
            if (appDbContext != null)
            {
                return await appDbContext.roleMenuPermissions.FirstOrDefaultAsync(i => i.RoleId == RoleId && i.NavigationMenuId==MenuId);

            }
            return null;
        }
        public async Task<IEnumerable<NavigationMenu>> GetSubMenu(int id)
        {
            if (appDbContext != null)
            {
                return appDbContext.navigationMenu.Where(so => so.ParentMenuId == id).OrderBy(x => x.Position);                
            }

            return null;
        }
        public async Task<NavigationMenu> GetSubMenuById(int id)
        {
            if (appDbContext != null)
            {
                return await appDbContext.navigationMenu.FirstOrDefaultAsync(so => so.ParentMenuId == id);
            }
            return null;
        }
       
        public async Task<int> UpdateMenu(NavigationMenu navigationMenu)
        {
            if (navigationMenu != null)
            {
                appDbContext.navigationMenu.Update(navigationMenu);
                await appDbContext.SaveChangesAsync();
                return navigationMenu.Id;
            }
            return 0;
        }
        public RoleMenuPermission AddRoleMenu(RoleMenuPermission roleMenuPermission)
        {
            if(roleMenuPermission != null)
            { 
                appDbContext.roleMenuPermissions.Add(roleMenuPermission);
                appDbContext.SaveChanges();
                return roleMenuPermission;
            }
            return null;
        }
        public RoleMenuPermission UpdateRoleMenu(RoleMenuPermission roleMenuPermission)
        {
            if (roleMenuPermission != null)
            {
                appDbContext.roleMenuPermissions.Update(roleMenuPermission);
                appDbContext.SaveChanges();
                return roleMenuPermission;
            }
            return null;
        }
        //public bool CheckRole(string RoleId)
        //{
        //    if (appDbContext.roleMenuPermissions.Any(i=>i.RoleId==RoleId))
        //    {
        //       return true;
        //    }
        //    return false;
        //}

        //public int GetRoleMenuId(string roleId, int menuId)
        //{
        //    int roleMenuId = 0;
        //    var roleList= appDbContext.roleMenuPermissions.ToList();
        //    foreach (var roleMenu in roleList.FindAll(x => x.NavigationMenuId == 1 && x.RoleId == roleId)) 
        //    {
        //        roleMenuId = roleMenu.Id;
        //    }
        //                //appDbContext.roleMenuPermissions.Where(x => x.RoleId == RoleId && x.NavigationMenuId == MenuId).FirstOrDefault();
        //    //int k = appDbContext.roleMenuPermissions.Where(x => x.RoleId == RoleId && x.NavigationMenuId == MenuId).Select(i => i.Id);// (i => i.RoleId == RoleId && i.NavigationMenuId == MenuId);
        //    // Id;
        //    return roleMenuId;
        //}

        public bool CheckRoleMenu(string RoleId, int MenuId)
        {
            var roleList = appDbContext.roleMenuPermissions.Where(i=>i.RoleId==RoleId && i.NavigationMenuId==MenuId && i.Status == true).ToList();
            if (roleList.Any())
            {
                return true;
            }
            //foreach (var roleMenu in roleList.FindAll(x => x.NavigationMenuId == 1 && x.RoleId == RoleId))
            //{
            //   if(roleMenu.)
            //}
            return false;
        }
    }
}
