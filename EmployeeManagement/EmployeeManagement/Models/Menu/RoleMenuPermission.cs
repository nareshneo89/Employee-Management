using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Models.Menu
{
    [Table(name: "AspNetRoleMenuPermission")]    
    public class RoleMenuPermission
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("ApplicationRole")]
        public string RoleId { get; set; }

        [ForeignKey("NavigationMenu")]
        public int NavigationMenuId { get; set; }
        public bool Status { get; set; }

        public NavigationMenu NavigationMenu { get; set; }

    }
}
