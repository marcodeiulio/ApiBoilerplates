using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StdControllers.Data;
using StdControllers.Models;
using TrackoApi.Models;

namespace StdControllers.Controllers;

[Route("api/[controller]")]
[ApiController]
public class JobApplicationStatusesController : ControllerBase
{
    protected readonly ApplicationDbContext _context;
    public JobApplicationStatusesController(ApplicationDbContext context)
    {
        _context = context;
    }


    [HttpGet(Name = "GetJobApplicationStatuses")]
    public async Task<ActionResult<List<JobApplicationStatuses>>> GetJobApplicationStatuses()
    {
        return Ok(await _context.JobApplicationStatuses.ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<JobApplicationStatuses?>> GetJobApplicationById(int id)
    {
        var jobAppStatus = await _context.JobApplicationStatuses.FirstOrDefaultAsync(j => j.Id == id);

        if (jobAppStatus is null)
            return NotFound();

        return Ok(jobAppStatus);
    }

    [HttpPost]
    public async Task<ActionResult<JobApplicationStatuses>> CreateJobApplication(JobApplicationStatusesRequestDto newJobAppStatusRequest)
    {
        if (newJobAppStatusRequest is null)
            return BadRequest();

        var newJobAppStatus = newJobAppStatusRequest.Adapt<JobApplicationStatuses>();
        _context.JobApplicationStatuses.Add(newJobAppStatus);

        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetJobApplicationById), new { id = newJobAppStatus.Id }, newJobAppStatusRequest);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateJobApplication(int id, JobApplicationStatusesRequestDto updatedJobAppStatus)
    {
        var jobAppStatusToUpdate = await _context.JobApplicationStatuses.FindAsync(id);

        if (jobAppStatusToUpdate is null)
            return NotFound();

        jobAppStatusToUpdate.Name = updatedJobAppStatus.Name;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteJobApplication(int id)
    {
        var jobAppStatusToDelete = await _context.JobApplicationStatuses.FindAsync(id);

        if (jobAppStatusToDelete is null)
            return NotFound();

        _context.JobApplicationStatuses.Remove(jobAppStatusToDelete);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}