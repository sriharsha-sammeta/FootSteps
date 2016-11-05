using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using FootSteps987.DataObjects;
using WebApplication11.Models;
using System.Collections.Generic;
using FootSteps987.Models;
using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace WebApplication11.Controllers
{
    public class GroupController : TableController<Group>
    {
        MobileServiceContext context = new MobileServiceContext();
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);            
            DomainManager = new EntityDomainManager<Group>(context, Request, Services);
        }

        // GET tables/Group
        public IQueryable<Group> GetAllGroup()
        {
            return Query(); 
        }

        // GET tables/Group/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<Group> GetGroup(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/Group/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<Group> PatchGroup(string id, Delta<Group> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/Group
        public async Task<IHttpActionResult> PostGroup(Group item)
        {
            Group current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Group/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteGroup(string id)
        {
             return DeleteAsync(id);
        }

        [Route("api/getGroupFromGroupId")]
        [HttpGet]
        public Group getGroupFromGroupId(string groupId)
        {
            return context.Groups.FirstOrDefault(x=> x.Id==groupId);
        }


        [Route("api/createGroup")]
        [HttpGet]
        public async Task<Group> createGroup(string groupStr, string personsListStr)// Modified during Testing
        {

            Group group =JsonConvert.DeserializeObject<Group>(groupStr);
            List<Person> persons = JsonConvert.DeserializeObject<List<Person>>(personsListStr);
            group.Id = Guid.NewGuid().ToString();
            
            foreach(Person p in persons){
                if (context.People.FirstOrDefault(x => x.Id == p.Id) == null)
                    return group;
            }
            if (await context.Groups.FindAsync(group.Id) == null)
            {                             
                await PostGroup(group);
                foreach (Person tempPerson in persons)
                {
                    Person person = await context.People.FindAsync(tempPerson.Id);
                    if (person != null)
                    {
                        context.GroupMappers.Add(new GroupMapper
                        {
                            Id = Guid.NewGuid().ToString(),
                            groupId = group.Id,
                            personId = person.Id,
                            isPersonVisibleInGroup = person.isGloballyVisible
                        });
                    }
                    
                }
                await context.SaveChangesAsync();
            }
            return group;
        }

        [Route("api/getPeopleInGroup")]
        [HttpGet]
        public List<Person> getPeopleInGroup(string groupId)
        {
            List<Person> persons = new List<Person>();
            if (context.GroupMappers.FirstOrDefault(x=> x.groupId==groupId) != null)
            {                
                List<GroupMapper>groupMappers=context.GroupMappers.Where(x =>
                    x.groupId == groupId).ToList<GroupMapper>();
                foreach (GroupMapper gpm in groupMappers)
                {
                    Person temp = context.People.FirstOrDefault(x=> x.Id== gpm.personId);
                    if (temp != null)
                    {
                        if (!(temp.isGloballyVisible && gpm.isPersonVisibleInGroup))
                        {
                            temp.latitude = Int32.MaxValue;
                            temp.longitude = Int32.MaxValue;
                        }
                        persons.Add(temp);
                    }
                }                    
            }
            return persons;
        }
       
    [Route("api/groupsFromPersonId")]    
     [HttpGet]
    public List<Group> groupsFromPersonId (string personId)
    {
        List<Group> groups = new List<Group>();
        if (context.GroupMappers.FirstOrDefault(x=>x.personId == personId) != null)
        {
            List<GroupMapper> groupMappers = context.GroupMappers.Where(x =>
                x.personId == personId).ToList<GroupMapper>();
            foreach (GroupMapper gpm in groupMappers)
            {
                Group temp = context.Groups.FirstOrDefault(x => x.Id == gpm.groupId);
                if (temp != null)
                {
                    groups.Add(temp);
                }
            }
        }
        return groups;
    }

    [Route("api/deleteGroupAll")]
    [HttpGet] // made during testing
    public async Task deleteAll()
    {
        context.Groups.RemoveRange(context.Groups);
        context.GroupMappers.RemoveRange(context.GroupMappers);
        await context.SaveChangesAsync();
    }

    [Route("api/getNotAvailablePersonsInGroup")]
    [HttpGet]
    public List<Person> getNotAvailablePersonsInGroup(string groupId)
    {
        List<Person> notAvailalePersonsList = new List<Person>();
        List<Person>temp=getPeopleInGroup(groupId);
        foreach (Person pp in temp)
        {
            if (context.GroupMappers.FirstOrDefault(x => x.personId == pp.Id)!=null)
            {
                List<GroupMapper> groupMappers = context.GroupMappers.Where(x => x.personId == pp.Id).ToList<GroupMapper>();
                foreach (GroupMapper gpm in groupMappers)
                {
                    if (context.Connects.FirstOrDefault(x => x.groupMapperId == gpm.Id && x.status == true) != null || !gpm.isPersonVisibleInGroup || !pp.isGloballyVisible)
                    { 
                        notAvailalePersonsList.Add(pp);
                        break;
                    }
                }
            }
        }
        return notAvailalePersonsList;
    }

        [Route("api/isGroupInMeeting")]
        [HttpGet]
    public object isGroupInMeeting(string groupId)
    {
        if (context.GroupMappers.FirstOrDefault(x => x.groupId == groupId) != null)
        {
            List<GroupMapper> groupMappers = context.GroupMappers.Where(x => x.groupId == groupId).ToList<GroupMapper>();
            foreach (var gpm in groupMappers)
            {
                if (context.Connects.FirstOrDefault(x => x.groupMapperId == gpm.Id) != null)
                    return new object();
            }
        }
        return null;
    }
        [Route("api/isListOfGroupsInMeeting")]
        [HttpGet]
        public List<string> isListOfGroupsInMeeting(string groupStr)
        {
            List<Group> groups = JsonConvert.DeserializeObject<List<Group>>(groupStr);
            List<string> result = new List<string>();
            foreach (Group group in groups)
            {
                if (isGroupInMeeting(group.Id)!=null)
                {
                    result.Add(group.Id);
                }
            }
            return result;
        }

        [Route("api/getDestinationForGroup")]
        [HttpGet]
        public Dictionary<string, string> getDestinationForGroup(string groupId)
        {
            Group group=context.Groups.FirstOrDefault(x => x.Id == groupId);
            if (group != null)
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("destinationLatitude", group.destinationLatitude.ToString());
                dict.Add("destinationLongitude", group.destinationLongitude.ToString());
                dict.Add("destinationName", group.destinationName);
                return dict;
            }
            return null;
        }

        [Route("api/updateDestinationForGroup")]
        [HttpGet]
        public async Task updateDestinationForGroup(string destinationLatitude, string destinationLongitude, string destinationName,string groupId)
        {
            Group group=context.Groups.FirstOrDefault(x => x.Id == groupId);
            if (group != null)
            {
                group.destinationLatitude = double.Parse(destinationLatitude);
                group.destinationLongitude = double.Parse(destinationLongitude);
                group.destinationName = destinationName;
                await context.SaveChangesAsync();
            }
        }
    }
   
}