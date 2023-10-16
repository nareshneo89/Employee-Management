using AspNetCoreHero.ToastNotification.Abstractions;
using EmployeeManagement.Models;
using EmployeeManagement.Models.Menu;
using EmployeeManagement.Services.Menu;
using EmployeeManagement.ViewModel;
using EmployeeManagement.ViewModel.Menu;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NLog.Filters;
using ReflectionIT.Mvc.Paging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace EmployeeManagement.Controllers
{
    public class MenuController : Controller
    {
        private readonly AppDbContext appDbContext;
        private readonly INavigationMenuService navigationMenuService;
        private readonly INotyfService notyfService;
        private readonly RoleManager<IdentityRole> roleManager;

        public MenuController(AppDbContext appDbContext, INavigationMenuService navigationMenuService, INotyfService notyfService, RoleManager<IdentityRole> roleManager)
        {
            this.appDbContext = appDbContext;
            this.navigationMenuService = navigationMenuService;
            this.notyfService = notyfService;
            this.roleManager = roleManager;
        }
        [Obsolete]
        public IActionResult Index()     ///string filter, int page = 1, string sortExpression = "Name"
        {
            var menu=navigationMenuService.GetMasterMenu();
            return View(menu);
            //var qry = appDbContext.navigationMenu.AsNoTracking().Where(p => p.ParentMenuId == null);
            ////.Include(p => p.Name)
            ////.Include(p => p.ControllerName)
            ////.AsQueryable();

            //if (!string.IsNullOrWhiteSpace(filter))
            //{
            //    qry = qry.Where(p => p.Name.Contains(filter));
            //}

            //var model = await PagingList<NavigationMenu>.CreateAsync(qry, 20, page, sortExpression, "Name");

            //model.RouteValue = new RouteValueDictionary {
            //    { "Filter", filter}
            //};

            //return View(model);

        }
        public IActionResult Create()
        {
            var models = navigationMenuService.GetMasterMenu();
            if (models.Any())
            {
                var position = models.Select(i => i.Position).Max();
                ViewData["position"] = position + 1;
                return View();
            }
            ViewData["position"] = 1;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(NavigationMenuViewModel navigationMenuViewModel)
        {
            if (ModelState.IsValid)
            {
                NavigationMenu navigation = new NavigationMenu()
                {
                    Id = navigationMenuViewModel.Id,
                    Name = navigationMenuViewModel.Name,
                    ParentMenuId = navigationMenuViewModel.ParentMenuId,
                    ControllerName = navigationMenuViewModel.ControllerName,
                    ActionName = navigationMenuViewModel.ActionName,
                    Icon = navigationMenuViewModel.Icon,
                    Position = navigationMenuViewModel.Position,
                    Status = navigationMenuViewModel.Status,

                };
                navigationMenuService.AddMenu(navigation);
                notyfService.Success("You have successfully saved the data.");

                return RedirectToAction("Index");
            }
            return View();
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.MenuId = id;
            NavigationMenu MasternavigationMenu = await navigationMenuService.GetMasterMenuById(id);
            if (MasternavigationMenu != null)
            {
               
                var MenuRole = await navigationMenuService.GetRoleMenuByMenuId(id);
                var rolesList = roleManager.Roles.ToList();
                //(from roles in roleManager.Roles
                //             select new SelectListItem()
                //             {
                //                 Text = roles.Name,
                //                 Value = roles.Id.ToString(),
                //             }).ToList();

                var selectRoleMenu = rolesList.Join(// outer sequence 
                                      MenuRole,  // inner sequence 
                                      r => r.Id,    // outerKeySelector
                                      rm => rm.RoleId,  // innerKeySelector
                                      (rm, r) => new  // result selector
                                      {
                                          //Id = r.RoleId,
                                          RoleName = rm.Name,
                                      });

                var MasterMenu = new NavigationMenuViewModel
                {
                    Id = MasternavigationMenu.Id,
                    Name = MasternavigationMenu.Name,
                    ParentMenuId = MasternavigationMenu.ParentMenuId,
                    ControllerName = MasternavigationMenu.ControllerName,
                    ActionName = MasternavigationMenu.ActionName,
                    Icon = MasternavigationMenu.Icon,
                    Position = MasternavigationMenu.Position,
                    Status = MasternavigationMenu.Status,
                    Roles= MenuRole
                };
                return View(MasterMenu);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(NavigationMenuViewModel navigationMenuViewModel)
        {
            if (ModelState.IsValid)
            {
                NavigationMenu navigation = await navigationMenuService.GetMasterMenuById(navigationMenuViewModel.Id);

                navigation.Name = navigationMenuViewModel.Name;
                navigation.ParentMenuId = navigationMenuViewModel.ParentMenuId;
                navigation.ControllerName = navigationMenuViewModel.ControllerName;
                navigation.ActionName = navigationMenuViewModel.ActionName;
                navigation.Icon = navigationMenuViewModel.Icon;
                navigation.Position = navigationMenuViewModel.Position;
                navigation.Status = navigationMenuViewModel.Status;

                await navigationMenuService.UpdateMenu(navigation);
                ModelState.Clear();
                notyfService.Success("You have successfully update the data.");
                return RedirectToAction("Index");
            }
            return View();
        }
        public async Task<IActionResult> Delete(int id)
        {
            var subMenu = navigationMenuService.GetSubMenuById(id);
            if (subMenu.Result == null)
            {
                NavigationMenu navigationMenu = await navigationMenuService.GetMasterMenuById(id);
                NavigationMenuViewModel navigation = new NavigationMenuViewModel()
                {
                    Name = navigationMenu.Name,
                    ParentMenuId = navigationMenu.ParentMenuId,
                    ControllerName = navigationMenu.ControllerName,
                    ActionName = navigationMenu.ActionName,
                    Icon = navigationMenu.Icon,
                    Position = navigationMenu.Position,
                    Status = navigationMenu.Status
                };
                return View(navigation);
            }
            else
            {
                notyfService.Error("First Delete Sub Menu Data then delete .");
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Delete(NavigationMenuViewModel menuModelView)
        {
            NavigationMenu menuModel = await navigationMenuService.GetMasterMenuById(menuModelView.Id);

            if (menuModel == null)
            {
                ViewBag.ErrorMessage = $"Employee with Id= {menuModelView.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                try
                {
                    var result = navigationMenuService.DeleteMenu(menuModelView.Id);
                    notyfService.Warning("You have successfully Deleted the data.");
                    return RedirectToAction("Index");
                }
                catch (DbUpdateException ex)
                {
                    return View("Error");
                }
            }

        }

        public async Task<IActionResult> SubMenuDetails(int id)
        {
            var navigationMenu = await navigationMenuService.GetSubMenu(id);
            ViewBag.MenuId = id;

            return View(navigationMenu);
        }
        public async Task<IActionResult> CreateSubMenu(int id)
        {
            NavigationMenu navigation = await navigationMenuService.GetSubMenuById(id);

            if (navigation != null)
            {
                NavigationMenuViewModel navigationMenu = new NavigationMenuViewModel()
                {
                    ParentMenuId = id,
                    Position = navigation.Position + 1
                };
                return View(navigationMenu);
            }
            else
            {
                NavigationMenuViewModel navigationMenu = new NavigationMenuViewModel()
                {
                    ParentMenuId = id,
                    Position = 1
                };
                return View(navigationMenu);
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSubMenu(NavigationMenuViewModel navigationMenuViewModel)
        {

            if (ModelState.IsValid)
            {
                NavigationMenu navigation = new NavigationMenu()
                {
                    Name = navigationMenuViewModel.Name,
                    ParentMenuId = navigationMenuViewModel.ParentMenuId,
                    ControllerName = navigationMenuViewModel.ControllerName,
                    ActionName = navigationMenuViewModel.ActionName,
                    Icon = navigationMenuViewModel.Icon,
                    Position = navigationMenuViewModel.Position,
                    Status = navigationMenuViewModel.Status,

                };
                navigationMenuService.AddMenu(navigation);
                notyfService.Success("You have successfully saved the data.");
                return RedirectToAction("Index", new { id = navigation.Id });
            }
            return View();
        }

        public IActionResult ManageMenuRoles(int MenuId)
        {
            ViewBag.MenuId = MenuId;
            //var user = await userManager.FindByIdAsync(UserId);
            //if (user == null)
            //{
            //    ViewBag.ErrorMessage = $"User with Id= {UserId} cannot be found";
            //    return View("NotFound");
            //}
            var model = new List<MenuRoleViewModel>();
            foreach (var role in roleManager.Roles.ToList())
            {
                var userRolesViewModel = new MenuRoleViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };
                if (navigationMenuService.CheckRoleMenu(role.Id, MenuId))
                {
                    userRolesViewModel.IsSelected = true;
                }
                else
                {
                    userRolesViewModel.IsSelected = false;
                }
                model.Add(userRolesViewModel);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageMenuRoles(List<MenuRoleViewModel> model, int menuId)
        {
            ViewBag.MenuId = menuId;
            if (model.Any())
            {
                foreach (var role in model)
                {
                    RoleMenuPermission roleMenuPermission = await navigationMenuService.GetRoleMenu(role.RoleId, menuId);
                    var MenuRole = await navigationMenuService.GetRoleMenu(role.RoleId,menuId);
                    if (roleMenuPermission == null)
                    {
                        roleMenuPermission=new RoleMenuPermission();
                        roleMenuPermission.RoleId = role.RoleId;
                        roleMenuPermission.NavigationMenuId = menuId;
                        roleMenuPermission.Status = role.IsSelected;
                        navigationMenuService.AddRoleMenu(roleMenuPermission);
                    }
                    else
                    {
                        //roleMenuPermission.Id = roleDetail.Id;
                        roleMenuPermission.RoleId = role.RoleId;
                        roleMenuPermission.NavigationMenuId = menuId;
                        roleMenuPermission.Status = role.IsSelected;
                        //roleMenuPermission.Id = Id;
                        navigationMenuService.UpdateRoleMenu(roleMenuPermission);
                    }

                }
                notyfService.Success("You have successfully given permition.");
                return RedirectToAction("ManageMenuRoles", new { MenuId = menuId });
                
            }
            return View(model);
        }
        //    public IActionResult MenuPermition()
        //{
        //    NavigationMenuViewModel navigationMenuViewModel = new NavigationMenuViewModel();
        //    navigationMenuViewModel.Role = (from roles in roleManager.Roles
        //                                     select new SelectListItem()
        //                                     {
        //                                         Text = roles.Name,
        //                                         Value = roles.Id.ToString(),
        //                                     }).ToList();
        //    navigationMenuViewModel.navigationMenu = navigationMenuService.GetMenu().ToList();

        //   // navigationMenuViewModel.roleMenuPermission = navigationMenuService.GetRoleMenu().ToList();

        //    return View(navigationMenuViewModel);

        //    //var rolesList = (from roles in roleManager.Roles
        //    //                 select new SelectListItem()
        //    //                 {
        //    //                     Text = roles.Name,
        //    //                     Value = roles.Id.ToString(),
        //    //                 }).ToList();

        //    //rolesList.Insert(0, new SelectListItem()
        //    //{
        //    //    Text = "----Select----",
        //    //    Value = string.Empty
        //    //});
        //    //var menus = navigationMenuService.GetMenu()
        //    //  .Select(e =>
        //    //  {
        //    //      e.Roles = rolesList;
        //    //      return e;
        //    //  });

        //    //var model = new List<NavigationMenuViewModel>();
        //    // var modelView = new NavigationMenuViewModel()
        //    // {
        //    //     //navigationMenuViewModel = navigationMenuService.GetMenu()
        //    //     Id = menus.,
        //    //     Name = model.Name,
        //    //    // Roles = rolesList
        //    // };
        //    ////var role= roleManager.Roles.FirstOrDefaultAsync();
        //    //// ViewBag.Roles = role;




        //    //ViewBag.Listofproducts = rolesList;
        //    //return View(menus);
        //}
        //[HttpPost]
        //public IActionResult MenuPermition(NavigationMenuViewModel navigationMenuViewModel)
        //{
        //    RoleMenuPermission roleMenuPermission = new RoleMenuPermission();
        //    var menuId = navigationMenuViewModel.Id;
        //    for (int i = 0; i < navigationMenuViewModel.Role.Count; i++)
        //    {
        //       //  = roleManager.Roles.Where(x => x.Name == navigationMenuViewModel.Roles[i].)
        //        int RoleId = navigationMenuService.GetRoleMenuId(navigationMenuViewModel.Role[i].Value.ToString(), menuId);
        //        if (navigationMenuService.CheckRole(navigationMenuViewModel.Role[i].Value) && navigationMenuViewModel.Role[i].Selected && RoleId==0)////&& !(await userManager.IsInRoleAsync(user, role.Name))
        //        {
        //            roleMenuPermission.RoleId = navigationMenuViewModel.Role[i].Value.ToString();
        //            roleMenuPermission.NavigationMenuId = menuId;
        //            roleMenuPermission.Status = true;
        //            navigationMenuService.AddRoleMenu(roleMenuPermission);
        //            ModelState.Clear();
        //        }
        //        else if (!navigationMenuService.CheckRole(navigationMenuViewModel.Role[i].Value) && navigationMenuViewModel.Role[i].Selected && RoleId > 0)////!navigationMenuViewModel.Roles[i].Selected
        //        {
        //           // roleMenuPermission.Id = RoleId;
        //            roleMenuPermission.RoleId = navigationMenuViewModel.Role[i].Value.ToString();
        //            roleMenuPermission.NavigationMenuId = menuId;
        //            roleMenuPermission.Status = true;
        //            //roleMenuPermission.Id = Id;
        //            navigationMenuService.UpdateRoleMenu(roleMenuPermission);
        //            ModelState.Clear();
        //        }
        //        else if (!navigationMenuService.CheckRole(navigationMenuViewModel.Role[i].Value) && !navigationMenuViewModel.Role[i].Selected && RoleId > 0)////!navigationMenuViewModel.Roles[i].Selected
        //        {
        //           // roleMenuPermission.Id = RoleId;
        //            roleMenuPermission.RoleId = navigationMenuViewModel.Role[i].Value.ToString();
        //            roleMenuPermission.NavigationMenuId = menuId;
        //            roleMenuPermission.Status = false;
        //            //roleMenuPermission.Id = Id;
        //            navigationMenuService.UpdateRoleMenu(roleMenuPermission);
        //            ModelState.Clear();
        //        } 
        //    }
        //        notyfService.Success("You have successfully given permition.");

        //        return RedirectToAction("MenuPermition");
        //        //else
        //        //{
        //        //    continue;
        //        //}
        //        //if (result.Succeeded)
        //        //{
        //        //    if (i < (model.Count - 1))
        //        //        continue;
        //        //    else
        //        //        return RedirectToAction("EditRole", new { Id = roleId });
        //        //}

        //        //NavigationMenu navigation = new NavigationMenu()
        //        //{
        //        //    Id = navigationMenuViewModel.Id,
        //        //    Roles = (from roles in navigationMenuViewModel.Roles
        //        //             select new SelectListItem()
        //        //             {
        //        //                 Text = roles.Text,
        //        //                 Value = roles.Value,
        //        //                 Selected = roles.Selected,
        //        //             }).ToList()

        //        //};
        //        //navigationMenuService.AddRoleMenu(navigation);
        //        //notyfService.Success("You have successfully given permition.");

        //        //return RedirectToAction("MenuPermition");
        //}
    }
}
