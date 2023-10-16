using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace EmployeeManagement.Models.Menu
{
    [Table(name: "AspNetNavigationMenu")]
    public class NavigationMenu
    {
        public NavigationMenu()
        {
            Roles =new List<SelectListItem>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [ForeignKey("ParentNavigationMenu")]
        public int? ParentMenuId { get; set; }
        [StringLength(90)]
        public string ControllerName { get; set; }
        [StringLength(90)]
        public string ActionName { get; set; }
        [StringLength(150)]
        public string Icon { get; set; }        
        public int Position { get; set; }
        public bool Status { get; set; }

        [NotMapped]
        public bool Permitted { get; set; }

        public virtual NavigationMenu ParentNavigationMenu { get; set; }
        public virtual ICollection<RoleMenuPermission> roleMenuPermission { get; set; }
        public virtual ICollection<NavigationMenu> navigationMenu { get; set; }
        [NotMapped]
        public List<SelectListItem> Roles { get; set; }
    }
}
