using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using WebApplication11.DataObjects;
using WebApplication11.Models;
using FootSteps987.DataObjects;
using System;
using System.Collections.Generic;
using FootSteps987.Models;
using Newtonsoft.Json;

namespace WebApplication11.Controllers
{
    public class ConnectController : TableController<Connect>
    {
        MobileServiceContext context = new MobileServiceContext();
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            DomainManager = new EntityDomainManager<Connect>(context, Request, Services);
        }

        // GET tables/Connect
        public IQueryable<Connect> GetAllConnect()
        {
            return Query(); 
        }

        // GET tables/Connect/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<Connect> GetConnect(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/Connect/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<Connect> PatchConnect(string id, Delta<Connect> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/Connect
        public async Task<IHttpActionResult> PostConnect(Connect item)
        {
            Connect current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Connect/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteConnect(string id)
        {
             return DeleteAsync(id);
        }


        [Route("api/addConnect")]
        [HttpGet]
        public async Task<object> addConnect(string groupId, string personId, bool addMyself)
        {
            GroupMapper gpm = context.GroupMappers.FirstOrDefault(x => x.personId == personId);
            Person person=context.People.FirstOrDefault(x=>x.Id==personId);
            if (addMyself)
            {
                if (gpm != null && (context.Connects.FirstOrDefault(x => x.groupMapperId == gpm.Id) != null))
                {
                    foreach (var connect in context.Connects.Where(x => x.groupMapperId == gpm.Id).ToList<Connect>())
                    {
                        await DeleteConnect(connect.Id);
                    }
                }
            }
            if (gpm.isPersonVisibleInGroup && person != null && person.isGloballyVisible)
            {
                    await PostConnect(new Connect
                      {
                          Id = Guid.NewGuid().ToString(),
                          groupMapperId = gpm.Id,
                          status = addMyself
                      });
                    return new object();               
            }

            return null;
        }

        [Route("api/addConnect2")]
        [HttpGet]
        public async Task addConnect2(string groupId, string personId, bool addMyself)
        {
            if (addMyself)
            {

                if(context.GroupMappers.FirstOrDefault(x=>x.personId==personId)!=null){
                    List<GroupMapper> groupMapperList = context.GroupMappers.Where(x => x.personId == personId).ToList();
                    foreach (var gpm in groupMapperList)
                    {
                        Connect tempConnect = context.Connects.FirstOrDefault(x => x.groupMapperId == gpm.Id);
                        if (tempConnect != null)
                        {
                          await DeleteConnect(tempConnect.Id);
                        }
                    }
                }                               
            }

            Person person = context.People.FirstOrDefault(x => x.Id == personId);
            GroupMapper gpm2 =context.GroupMappers.FirstOrDefault(x=>x.groupId==groupId && x.personId==personId);
            if (person != null && person.isGloballyVisible && gpm2!=null && gpm2.isPersonVisibleInGroup)
            {

                if (context.Connects.FirstOrDefault(x => x.groupMapperId == gpm2.Id) == null)
                {
                    await PostConnect(new Connect
                    {
                        Id = Guid.NewGuid().ToString(),
                        groupMapperId = gpm2.Id,
                        status = addMyself
                    });
                }
            }
        }

        [Route("api/addConnectForWholeGroup")]
        [HttpGet]
        public async Task addConnectForWholeGroup(string groupId, string personAdminId,string destinationLat, string destinationLong)
        {
            List<GroupMapper> gpmList = null;
            if (context.GroupMappers.FirstOrDefault(x => x.groupId == groupId) != null)
            {
                gpmList = context.GroupMappers.Where(x => x.groupId == groupId).ToList<GroupMapper>();
            }
            foreach (var gpm in gpmList)
            {
                Person temp = context.People.FirstOrDefault(x => x.Id == gpm.personId);
                if (temp != null && temp.isGloballyVisible && gpm.isPersonVisibleInGroup)
                {                    
                    if (gpm.personId == personAdminId)
                    {
                        await addConnect2(gpm.groupId, gpm.personId, true);

                        Group grp=context.Groups.FirstOrDefault(x => x.Id == groupId);
                        if (grp != null)
                        {
                            grp.destinationLatitude = JsonConvert.DeserializeObject<double>(destinationLat);
                            grp.destinationLongitude = JsonConvert.DeserializeObject<double>(destinationLong);
                        }
                        await context.SaveChangesAsync();
                    }
                    else
                    {
                        await addConnect2(gpm.groupId, gpm.personId, false);
                    }
                }
            }
            
        }

        [Route("api/deleteConnectUsingGroupAndPerson")]
        [HttpGet]
        public async Task deleteConnectUsingGroupAndPerson(string groupId, string personId)
        {
            var gpm = context.GroupMappers.FirstOrDefault(x => x.personId == personId && x.groupId == groupId);
            if (gpm != null)
            {
                var myconnect = context.Connects.FirstOrDefault(x => x.groupMapperId == gpm.Id);
                if (myconnect != null)
                {
                   await DeleteConnect(myconnect.Id);
                }
            }
        }

        [Route("api/deleteAllConnectUsingPersonId")]
        [HttpGet]
        public void deleteAllConnectUsingPersonId(string personId)
        {
            List<GroupMapper> groupMappers = new List<GroupMapper>();
            if(context.GroupMappers.FirstOrDefault(x => x.personId == personId)!=null){
                groupMappers = context.GroupMappers.Where(x => x.personId == personId).ToList<GroupMapper>();
            }
            foreach (var gpm in groupMappers)
            {
                var connect = context.Connects.FirstOrDefault(x => x.groupMapperId == gpm.Id);
                if (connect != null)
                {
                    DeleteConnect(connect.Id);
                }
            }            
        }

        [Route("api/deleteConnectUsingGroupId")]
        [HttpGet]
        public async Task deleteConnectUsingGroupId(string groupId)
        {
            List<GroupMapper> groupMappers = new List<GroupMapper>();
            if (context.GroupMappers.FirstOrDefault(x => x.groupId == groupId) != null)
            {
                groupMappers = context.GroupMappers.Where(x => x.groupId == groupId).ToList<GroupMapper>();
            }
            foreach (var gpm in groupMappers)
            {
                var connect = context.Connects.FirstOrDefault(x => x.groupMapperId == gpm.Id);
                if (connect != null)
                {
                    await DeleteConnect(connect.Id);
                }
            }
            Group grp = context.Groups.FirstOrDefault(x => x.Id == groupId);
            if (grp != null)
            {
                grp.destinationLatitude = 1000;
                grp.destinationLongitude = 1000;
            }
            await context.SaveChangesAsync();
        }

        [Route("api/getPendingConnectsForPerson")]
        [HttpGet] // made during testing
        public List<Connect> getPendingConnectsForPerson(string personId)
        {
            List<Connect>pendingConnects=new List<Connect>();
            if(context.GroupMappers.FirstOrDefault(x=>x.personId==personId)!=null && context.People.FirstOrDefault(x=>x.Id==personId)!=null){
                List<GroupMapper> groupMappers = context.GroupMappers.Where(x => x.personId == personId).ToList<GroupMapper>();
                foreach (var gpm in groupMappers)
                {
                    if (context.Connects.FirstOrDefault(x => x.groupMapperId == gpm.Id && x.status == false) != null)
                        pendingConnects.AddRange(context.Connects.Where(x => x.groupMapperId == gpm.Id && x.status == false).ToList<Connect>());
                }
            }            
            return pendingConnects;
        }

        [Route("api/updateConnectUsingConnectID")]
        [HttpGet] // Added during testing
        public async Task<object> updateConnect(string connectId, bool status) // return type made to object during testing
        {
            Connect connect = context.Connects.FirstOrDefault(x => x.Id == connectId);
            if (connect != null)
            {
                if (status)
                {
                    GroupMapper gpm = context.GroupMappers.FirstOrDefault(x => x.Id == connect.groupMapperId);
                    if (gpm != null)
                    {
                        Person person = context.People.FirstOrDefault(x => x.Id == gpm.personId);
                        if (person != null)
                        {
                            person.isGloballyVisible = true;
                            gpm.isPersonVisibleInGroup = true;
                            connect.status = status;
                            await context.SaveChangesAsync();
                        }
                    }
                }
                else
                    await DeleteConnect(connectId);
            }

            return new object();

        }

        [Route("api/updateConnect")]
        [HttpGet] // Added during testing
        public async Task updateConnect(string groupId, string personId, bool status)
        {
            Connect connect=getConnectUsingPersonAndGroup(groupId,personId);
            if (connect != null)
            {
                await updateConnect(connect.Id, status);
            }                        
        }

        [Route("api/getConnectUsingPersonAndGroup")]
        [HttpGet] // Added during testing
        public Connect getConnectUsingPersonAndGroup(string groupId,string personId)
        {

            GroupMapper groupMapper = context.GroupMappers.FirstOrDefault(x => x.groupId == groupId && x.personId == personId);
            Connect connect=null;
            if (groupMapper != null)
            {
                connect = context.Connects.FirstOrDefault(x => x.groupMapperId == groupMapper.Id);
            }            
            return connect;
        }

         [Route("api/getMeeetingStateUsingPersonAndGroup")]
        [HttpGet] // Added during testing
        public State getMeetingStateUsingPersonAndGroup(string groupId,string personId)
        {

            State participantsState = new State();
            List<GroupMapper> gpmList = null;
            if (context.GroupMappers.FirstOrDefault(x => x.groupId == groupId)!=null)
            {
                gpmList = context.GroupMappers.Where(x => x.groupId == groupId).ToList<GroupMapper>();

                foreach (var gpm in gpmList)
                {
                    if (context.Connects.FirstOrDefault(x => x.groupMapperId == gpm.Id) != null)
                    {
                        participantsState.groupInMeeting = true;
                        break;
                    }
                }
            }            
            if (context.GroupMappers.FirstOrDefault(x => x.personId == personId) != null)
            {
                gpmList = context.GroupMappers.Where(x => x.personId == personId).ToList<GroupMapper>();
                foreach (var gpm in gpmList)
                {
                    if (context.Connects.FirstOrDefault(x => x.groupMapperId == gpm.Id) != null)
                    {
                        participantsState.personInMeeting = true;
                        break;
                    }
                }
            }
                       
            if (context.GroupMappers.FirstOrDefault(x => x.groupId == groupId && x.personId==personId) != null)
            {
                gpmList = context.GroupMappers.Where(x => x.groupId == groupId && x.personId == personId).ToList<GroupMapper>();
                foreach (var gpm in gpmList)
                {
                    if (context.Connects.FirstOrDefault(x => x.groupMapperId == gpm.Id) != null)
                    {
                        participantsState.personAndGroupInSameMeeting = true;
                        break;
                    }
                }

            }

            return participantsState;
        }

         [Route("api/getDestination")]
         [HttpGet]
         public Dictionary<string, string> getDestination(string groupId)
         {


              Group grp = context.Groups.FirstOrDefault(x => x.Id == groupId);
              string latitude =null;
              string longitude=null;
              if (context.GroupMappers.FirstOrDefault(x => x.groupId == groupId) != null && grp != null)
             {
                 List<GroupMapper> groupMappersList = context.GroupMappers.Where(x => x.groupId == groupId).ToList();
                 foreach (var gpm in groupMappersList)
                 {
                     if (context.Connects.FirstOrDefault(x=>x.groupMapperId==gpm.Id)!=null)
                     {
                         latitude= grp.destinationLatitude.ToString();
                         longitude = grp.destinationLongitude.ToString();
                         break;
                     }
                 }
                 if (latitude == null)
                 {
                     latitude = (1000).ToString();
                     longitude = (1000).ToString();
                 }
                 Dictionary<string, string> dict = new Dictionary<string, string>();
                 dict.Add("latitude", latitude);
                 dict.Add("longitude", longitude);
                 return dict;
             }                         
             return null;

         }

        [Route("api/deleteAllConnects")]
        [HttpGet]
         public async Task deleteAllConnects()
         {
             context.Connects.RemoveRange(context.Connects);
             await context.SaveChangesAsync();
         }
    }
}