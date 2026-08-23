using System.IdentityModel.Tokens.Jwt;
using API.Models;
using BusinessObjects.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Implementations;
using Services.Interfaces;
using Services.Models.Admin;
namespace API.Controllers;
[ApiController]
[Route("api/admin/export-jobs")]
[Authorize(Roles=AppRoles.Admin)]
public class AdminExportJobsController(IAdminExportService service):ControllerBase
{
    [HttpPost] public async Task<ActionResult<AdminExportJobResponse>> Create(AdminExportJobCreateRequest request,CancellationToken ct){if(!AdminId(out var id))return Unauthorized();try{var job=await service.CreateJobAsync(id,request,ct);return CreatedAtAction(nameof(Get),new{id=job.Id},job);}catch(AdminExportValidationException ex){return ApiErrorFactory.Error(HttpContext,400,ex.Message);}}
    [HttpGet] public async Task<ActionResult<IReadOnlyList<AdminExportJobResponse>>> List(CancellationToken ct){if(!AdminId(out var id))return Unauthorized();return Ok(await service.GetJobsAsync(id,ct));}
    [HttpGet("{id:int}")] public async Task<ActionResult<AdminExportJobResponse>> Get(int id,CancellationToken ct){if(!AdminId(out var admin))return Unauthorized();var job=await service.GetJobAsync(admin,id,ct);return job is null?NotFound():Ok(job);}
    [HttpGet("{id:int}/download")] public async Task<IActionResult> Download(int id,CancellationToken ct){if(!AdminId(out var admin))return Unauthorized();var file=await service.DownloadAsync(admin,id,ct);return file is null?NotFound():File(file.Bytes,file.ContentType,file.FileName);}
    private bool AdminId(out int id)=>int.TryParse(User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value,out id);
}
