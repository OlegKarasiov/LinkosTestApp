using LinkosTestApp.Models;
using Newtonsoft.Json;
using Group = LinkosTestApp.Models.Group;

namespace LinkosTestApp.Services
{
    public class GroupService : IGroupService
    {
        private readonly List<Group> _groups = new List<Group>();

        public void ImportFromContent(string content)
        {
            var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var groups = new List<Group>();
            var lineNumber = 0;
            foreach (var line in lines)
            {
                lineNumber++;
                try
                {
                    var parts = line.Split('.');
                    if (parts.Length != 2) throw new FormatException();

                    int groupNumber = int.Parse(parts[0].Trim());
                    var scheduleParts = parts[1].Split(';');

                    var schedules = new List<Schedule>();
                    foreach (var schedulePart in scheduleParts)
                    {
                        var times = schedulePart.Split('-');
                        if (times.Length != 2) throw new FormatException();

                        var from = TimeSpan.Parse(times[0].Trim());
                        var to = TimeSpan.Parse(times[1].Trim());

                        schedules.Add(new Schedule { From = from, To = to });
                    }

                    groups.Add(new Group { Number = groupNumber, Schedules = schedules });
                }
                catch
                {
                    throw new FormatException($"Неправильний формат даних у рядку #{lineNumber}: {line}");
                }
            }

            _groups.Clear();
            _groups.AddRange(groups);
        }

        public string ExportToJson(int? groupNumber = null)
        {
            var groupsToExport = groupNumber.HasValue ?
                _groups.Where(g => g.Number == groupNumber.Value).ToList() :
                _groups;

            var json = JsonConvert.SerializeObject(groupsToExport, Formatting.Indented);
            return json;
        }

        public Group GetGroup(int number)
        {
            return _groups.FirstOrDefault(g => g.Number == number);
        }

        public List<Group> GetAllGroups()
        {
            return _groups;
        }

        public void EditGroupSchedule(int number, List<Schedule> schedules)
        {
            var group = GetGroup(number);
            if (group != null)
            {
                group.Schedules = schedules;
            }
            else
            {
                throw new Exception("Групу не знайдено.");
            }
        }

        public bool IsElectricityAvailable(int number)
        {
            var group = GetGroup(number);
            if (group == null) throw new Exception("Групу не знайдено.");

            var now = DateTime.Now.TimeOfDay;
            return !group.Schedules.Any(s => s.From <= now && now <= s.To);
        }
    }
}
