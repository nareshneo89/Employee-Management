using EmployeeManagement.Models.Menu;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagement.ViewModel.Menu
{
    public class NavigationMenuViewModel
    {
        //public NavigationMenuViewModel()
        //{
        //    Roles = new List<string>();
        //}

        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentMenuId { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string Icon { get; set; }
        public int Position { get; set; }
        public bool Status { get; set; }
        public bool Permitted { get; set; }

        public virtual ICollection<RoleMenuPermission> roleMenuPermission { get; set; }
        public virtual ICollection<NavigationMenu> navigationMenu { get; set; }
        [NotMapped]
        public List<SelectListItem> Role { get; set; }
        public virtual IEnumerable<RoleMenuPermission> Roles { get; set; }
    }
}
