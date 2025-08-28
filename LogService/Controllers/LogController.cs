using Infrastructure.DTO.LogService;
using LogService.Infrastructure.Helpers;
using LogService.Infrastructure.Operations;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace LogService.Controllers
{
    [ApiController]
    [Route("api/log")]
    public class LogController : ControllerBase
    {
        private readonly ILoggerOperation loggerOperation;

        public LogController(ILoggerOperation loggerOperation)
        {
            this.loggerOperation = loggerOperation;
        }
        
        [HttpPut("InsertToLog")]
        public IActionResult InsertLog([FromBody] AppLogDto request)
        {
            return loggerOperation.EnqueueAppLog(request) ? Ok() : BadRequest();
        }
       
        [HttpPost("GetByFilter")]
        public async Task<IActionResult> GetByFilter([FromBody] LogFilterDto request)
        {
            var pageLogs =  await loggerOperation.GetAppLogsByFilter(request);              
            return Ok(pageLogs);
        }

 
        [HttpGet("GetAllNames")]
        public async Task<IActionResult> GetAllVirtualMachineNames()
        {
            var names = await loggerOperation.GetAllVirtualMachineNamesCachedAsync();
            return Ok(names);
        }
        
        [HttpPost("GetLogById")]
        public async Task<IActionResult> GetLogById(int id)
        {
            return Ok(await loggerOperation.GetAppLogById(id));
        }
    }
}

