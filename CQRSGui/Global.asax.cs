using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using SimpleCQRS;

namespace CQRSGui
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
        private static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new {controller = "Home", action = "Index", id = UrlParameter.Optional} // Parameter defaults
                );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterRoutes(RouteTable.Routes);

            var bus = new FakeBus();

            var storage = new EventStore(bus);
            var rep = new Repository<InventoryItemAggregateRoot>(storage);
            var commands = new InventoryItemCommandHandlers(rep);
            bus.RegisterHandler<InventoryItemCheckInCommand>(commands.Handle);
            bus.RegisterHandler<InventoryItemCreateCommand>(commands.Handle);
            bus.RegisterHandler<InventoryItemDeactivateCommand>(commands.Handle);
            bus.RegisterHandler<InventoryItemRemoveCommand>(commands.Handle);
            bus.RegisterHandler<InventoryItemRenameCommand>(commands.Handle);
            var detail = new InventoryItemDetailView();
            bus.RegisterHandler<InventoryItemCreatedEvent>(detail.Handle);
            bus.RegisterHandler<InventoryItemDeactivatedEvent>(detail.Handle);
            bus.RegisterHandler<InventoryItemRenamedEvent>(detail.Handle);
            bus.RegisterHandler<InventoryItemCheckedInEvent>(detail.Handle);
            bus.RegisterHandler<InventoryItemRemovedEvent>(detail.Handle);
            var list = new InventoryListView();
            bus.RegisterHandler<InventoryItemCreatedEvent>(list.Handle);
            bus.RegisterHandler<InventoryItemRenamedEvent>(list.Handle);
            bus.RegisterHandler<InventoryItemDeactivatedEvent>(list.Handle);
            ServiceLocator.Bus = bus;
        }
    }
}