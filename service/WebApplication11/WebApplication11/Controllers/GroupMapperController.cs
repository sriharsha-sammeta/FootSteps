using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using FootSteps987.DataObjects;
using WebApplication11.Models;
using System;
using System.Net.Http;
using System.Collections.Generic;
using FootSteps987.Models;
using WebApplication11.DataObjects;
using Newtonsoft.Json;

namespace WebApplication11.Controllers
{
    public class GroupMapperController : TableController<GroupMapper>
    {
        MobileServiceContext context = new MobileServiceContext();
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);

            DomainManager = new EntityDomainManager<GroupMapper>(context, Request, Services);
        }

        // GET tables/GroupMapper
        public IQueryable<GroupMapper> GetAllGroupMapper()
        {
            return Query();
        }

        // GET tables/GroupMapper/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<GroupMapper> GetGroupMapper(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/GroupMapper/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<GroupMapper> PatchGroupMapper(string id, Delta<GroupMapper> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/GroupMapper
        public async Task<IHttpActionResult> PostGroupMapper(GroupMapper item)
        {
            GroupMapper current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/GroupMapper/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteGroupMapper(string id)
        {
            return DeleteAsync(id);
        }


        [Route("api/deleteGroupMapperAll")]
        public async Task deleteAll()
        {

            context.GroupMappers.RemoveRange(context.GroupMappers);
            context.Groups.RemoveRange(context.Groups);
            await context.SaveChangesAsync();
        }

        [Route("api/togglePersonGroupVisibility")]
        [HttpGet]
        public object TogglePersonGroupVisibility(string personId, string groupId, bool toggleValue)
        {
            GroupMapper mapperItem = context.GroupMappers.FirstOrDefault(x => x.groupId == groupId && x.personId == personId);
            if (mapperItem != null)
            {
                mapperItem.isPersonVisibleInGroup = toggleValue;
                if (!toggleValue)
                {                    
                    Connect connect = context.Connects.FirstOrDefault(x => x.groupMapperId == mapperItem.Id);
                    if (connect != null)
                    {
                        context.Connects.Remove(connect);
                    }                    
                }
                context.SaveChangesAsync();
                return new object();                
            }
            return null;
        }


        [Route("api/getToggleValueForSingleGroup")]
        [HttpGet]
        public string getToggleValueForSingleGroup(string personId, string groupId)
        {
            GroupMapper groupMapper = context.GroupMappers.FirstOrDefault(x => x.personId == personId && x.groupId == groupId);
            if (groupMapper != null)
            {
                return groupMapper.isPersonVisibleInGroup.ToString();
            }
            return null;
        }

        [Route("api/getToggleValuesForGroupList")]
        [HttpGet]
        public Dictionary<string,string> getToggleValuesForGroup(string personId, string groupsListStr)
        {
            List<Group> groupsList=JsonConvert.DeserializeObject<List<Group>>(groupsListStr);
            Dictionary<string, string> groupToToggleValueDict = new Dictionary<string, string>();
            foreach (var group in groupsList)
            {
                groupToToggleValueDict.Add(group.Id,getToggleValueForSingleGroup(personId, group.Id));
            }
            return groupToToggleValueDict;
        }

        [Route("api/removePersonFromGroup")]
        [HttpGet]
        public async Task removePersonFromGroup(string personId, string groupId)
        {
            GroupMapper gp = context.GroupMappers.FirstOrDefault(x => x.personId == personId && x.groupId == groupId);
            if (gp != null)
            {
                context.GroupMappers.Remove(gp);
                await context.SaveChangesAsync();

                if (context.GroupMappers.FirstOrDefault(x => x.groupId == groupId) == null)
                {
                    Group tempGroup = context.Groups.FirstOrDefault(x => x.Id == groupId);
                    if (tempGroup != null)
                    {
                        context.Groups.Remove(tempGroup);
                    }
                    await context.SaveChangesAsync();
                }
            }
        }

        [Route("api/addpersontogroup")]
        [HttpGet]
        public async Task AddPersonToGroup(string personId, string groupId)
        {
            var group = context.Groups.FirstOrDefault(x => x.Id == groupId);
            var person = context.People.FirstOrDefault(x => x.Id == personId);
            if (group != null && person != null)
            {
                context.GroupMappers.Add(new GroupMapper
                {
                    Id = Guid.NewGuid().ToString(),
                    groupId = group.Id,
                    personId = person.Id,
                    isPersonVisibleInGroup=person.isGloballyVisible
                });
                await context.SaveChangesAsync();
            }
        }
    }
}