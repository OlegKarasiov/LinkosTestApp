namespace LinkosTestApp.Models
{
    public class Group
    {
        public int Number { get; set; }
        public List<Schedule> Schedules { get; set; } = new List<Schedule>();
    }
}
