using EmployeeManagement.Services.Menu;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EmployeeManagement.ViewComponents.Menu
{
    public class NavigationMenuViewComponent : ViewComponent
    {
        private readonly IDataAccessService dataAccessService;

        public NavigationMenuViewComponent(IDataAccessService dataAccessService)
        {
            this.dataAccessService = dataAccessService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var items = await dataAccessService.GetMenuItemsAsync(HttpContext.User);

            return View(items);
        }
    }
}
