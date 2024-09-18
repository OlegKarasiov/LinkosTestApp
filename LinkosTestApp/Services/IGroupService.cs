using LinkosTestApp.Models;
using Group = LinkosTestApp.Models.Group;

namespace LinkosTestApp.Services
{
    public interface IGroupService
    {
        void ImportFromContent(string content);
        string ExportToJson(int? groupNumber = null);
        Group GetGroup(int number);
        List<Group> GetAllGroups();
        void EditGroupSchedule(int number, List<Schedule> schedules);
        bool IsElectricityAvailable(int number);
    }
}
