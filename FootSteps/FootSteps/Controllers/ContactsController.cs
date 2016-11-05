using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using FootSteps.Models;

namespace FootSteps.Controllers
{
    public class ContactsController : TableController<Contacts>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            MobileServiceContext context = new MobileServiceContext();
            DomainManager = new EntityDomainManager<Contacts>(context, Request, Services);
        }

        // GET tables/Contacts
        public IQueryable<Contacts> GetAllContacts()
        {
            return Query(); 
        }

        // GET tables/Contacts/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<Contacts> GetContacts(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/Contacts/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<Contacts> PatchContacts(string id, Delta<Contacts> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/Contacts/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public async Task<IHttpActionResult> PostContacts(Contacts item)
        {
            Contacts current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Contacts/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteContacts(string id)
        {
             return DeleteAsync(id);
        }

    }
}