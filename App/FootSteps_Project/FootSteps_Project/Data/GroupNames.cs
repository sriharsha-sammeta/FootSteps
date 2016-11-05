using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootSteps_Project.Data {
    class GroupNames {
        public int id { get; set; }
        public String groupName { get; set; }

        public List<Phone_contacts> members = new List<Phone_contacts>();
        public String purpose { get; set; }
    }

    class GroupDetails { 
            public List<Phone_contacts> contacts_phone; 
            public List<GroupNames> groups_details;

            public GroupDetails() {
                enter_data();
            }
            void enter_data() {
                contacts_phone= new List<Phone_contacts>();
                contacts_phone.Add(new Phone_contacts { contactName = "Name2", contactNumber = "(222) 222-2222" });
                contacts_phone.Add(new Phone_contacts { contactName = "Name3", contactNumber = "(333) 333-3333" });
                contacts_phone.Add(new Phone_contacts { contactName = "Name5", contactNumber = "(555) 555-5555" });

                //enter data in groups list
                groups_details = new List<GroupNames>();
                groups_details.Add(new GroupNames { id = 0, groupName = "Group1", members = contacts_phone, purpose = "Movie" });
                groups_details.Add(new GroupNames { id = 1, groupName = "Group2", members = contacts_phone, purpose = "Meeting" });
            }
    }
}
