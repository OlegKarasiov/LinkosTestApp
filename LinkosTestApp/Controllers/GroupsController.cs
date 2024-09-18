using LinkosTestApp.Dtos;
using LinkosTestApp.Models;
using LinkosTestApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace LinkosTestApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupsController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupsController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpPost("import")]
        public IActionResult Import([FromBody] string content)
        {
            try
            {
                _groupService.ImportFromContent(content);
                return Ok("Дані успішно імпортовано.");
            }
            catch (FormatException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{groupNumber}/availability")]
        public IActionResult CheckAvailability(int groupNumber)
        {
            try
            {
                var isAvailable = _groupService.IsElectricityAvailable(groupNumber);
                return Ok(new { GroupNumber = groupNumber, IsElectricityAvailable = isAvailable });
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{groupNumber}")]
        public IActionResult EditGroupSchedule(int groupNumber, [FromBody] List<ScheduleDto> schedulesDto)
        {
            try
            {
                var schedules = schedulesDto.Select(s => new Schedule
                {
                    From = s.From,
                    To = s.To
                }).ToList();

                _groupService.EditGroupSchedule(groupNumber, schedules);
                return Ok("Графік успішно оновлено.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("export")]
        public IActionResult Export([FromQuery] int? groupNumber)
        {
            try
            {
                var json = _groupService.ExportToJson(groupNumber);
                return Ok(json);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetAllGroups()
        {
            var groups = _groupService.GetAllGroups();
            var groupsDto = groups.Select(g => new GroupDto
            {
                Number = g.Number,
                Schedules = g.Schedules.Select(s => new ScheduleDto
                {
                    From = s.From,
                    To = s.To
                }).ToList()
            }).ToList();

            return Ok(groupsDto);
        }

        [HttpGet("{groupNumber}")]
        public IActionResult GetGroup(int groupNumber)
        {
            var group = _groupService.GetGroup(groupNumber);
            if (group == null)
                return NotFound("Групу не знайдено.");

            var groupDto = new GroupDto
            {
                Number = group.Number,
                Schedules = group.Schedules.Select(s => new ScheduleDto
                {
                    From = s.From,
                    To = s.To
                }).ToList()
            };
            return Ok(groupDto);
        }
    }
}
