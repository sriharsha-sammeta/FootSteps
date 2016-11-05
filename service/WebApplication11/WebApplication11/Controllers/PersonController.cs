using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using FootSteps987.Models;
using WebApplication11.Models;
using System.Collections.Generic;
using FootSteps987.DataObjects;
using Newtonsoft.Json;

namespace WebApplication11.Controllers
{
    public class PersonController : TableController<Person>
    {
        MobileServiceContext context = new MobileServiceContext();
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);            
            DomainManager = new EntityDomainManager<Person>(context, Request, Services);
        }

        // GET tables/Person
        public IQueryable<Person> GetAllPerson()
        {
            return Query(); 
        }

        // GET tables/Person/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<Person> GetPerson(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/Person/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<Person> PatchPerson(string id, Delta<Person> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/Person
        public async Task<IHttpActionResult> PostPerson(Person item)
        {            
            Person current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Person/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeletePerson(string id)
        {
             return DeleteAsync(id);
        }
        [Route("api/deletePersonAll")]
        [HttpGet] // made during testing
        public async Task deletePersonAll()
        {
            context.GroupMappers.RemoveRange(context.GroupMappers);
            context.Groups.RemoveRange(context.Groups);
            context.People.RemoveRange(context.People);
            await context.SaveChangesAsync();
        }

        [Route("api/getPersonFromPersonId")]
        [HttpGet] // made during testing
        public Person getPersonFromPersonId(string personId)
        {
            return context.People.FirstOrDefault(x => x.Id == personId);            
        }

        [Route("api/insertIfNotExistsPerson")]
        [HttpGet] // made during testing
        public async Task<object> InsertIfNotExistsPerson(string personStr)
        {
            Person person = JsonConvert.DeserializeObject<Person>(personStr);
            Person temp = context.People.FirstOrDefault(x => x.Id == person.Id);
            if (temp == null)
            {

                await PostPerson(person);
                return new object();
            }

            return null;
        }

        [Route("api/updateLocation")]
        [HttpGet] // made during testing
        public async Task updateLocation(string  personStr)
        {
            Person person = JsonConvert.DeserializeObject<Person>(personStr);
            if (context.People.FirstOrDefault(x => x.Id == person.Id) != null)
            {
                Person tempPerson = context.People.FirstOrDefault(x => x.Id == person.Id);
                tempPerson.latitude = person.latitude;
                tempPerson.longitude = person.longitude;

                await context.SaveChangesAsync();                
            }
            
        }

        [Route("api/togglePersonGlobalVisibility")]
        [HttpGet]
        public async Task<object> TogglePersonGlobalVisibility(string personId, bool toggleValue)
        {
            Person person = context.People.FirstOrDefault(x => x.Id==personId);
            if (person != null)
            {
                person.isGloballyVisible = toggleValue;
                
                // remove all connects of that person w.r.t person ID only
                if(context.GroupMappers.FirstOrDefault(x=>x.personId==person.Id)!=null){
                    List<GroupMapper> groupMappers = context.GroupMappers.Where(x => x.personId == person.Id).ToList<GroupMapper>();
                    foreach (var gpm in groupMappers)
                    {
                        if (context.Connects.FirstOrDefault(x => x.groupMapperId == gpm.Id) != null)
                        {
                            context.Connects.RemoveRange(context.Connects.Where(x => x.groupMapperId == gpm.Id));
                            await context.SaveChangesAsync();
                        }
                    }
                }                
                return new object();
            }
            return null;
        }

        [Route("api/isPersonInMeeting")]
        [HttpGet] // made during testing
        public object isPersonInMeeting(string personId)
        {
            Person person = context.People.FirstOrDefault(x => x.Id == personId);
            if (context.GroupMappers.FirstOrDefault(x => x.personId == personId) != null && person != null && person.isGloballyVisible)
            {
                List<GroupMapper> groupMappers = context.GroupMappers.Where(x => x.personId == personId).ToList<GroupMapper>();
                foreach (var gpm in groupMappers)
                {
                    if (context.Connects.FirstOrDefault(x => x.groupMapperId == gpm.Id && x.status==true) != null)
                        return new object();
                }
            }
            return null;
        }

        [Route("api/getContactsMatchingFootsteps")]
        [HttpGet]
        public List<Person> getContactsMatchingFootsteps(string contacts_Person_arr)
        {
            List<Person> contacts_phone = JsonConvert.DeserializeObject<List<Person>>(contacts_Person_arr);
            List<Person> contacts_fs = new List<Person>();

            foreach (var contact in contacts_phone)
            {
                if (context.People.FirstOrDefault(x=> x.phoneNo==contact.phoneNo) != null)
                {
                    contacts_fs.Add(contact);
                }
            }
            return contacts_fs;
        }


    }
}