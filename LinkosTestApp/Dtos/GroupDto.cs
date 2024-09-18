namespace LinkosTestApp.Dtos
{
    public class GroupDto
    {
        public int Number { get; set; }
        public List<ScheduleDto> Schedules { get; set; } = new List<ScheduleDto>();
    }
}
