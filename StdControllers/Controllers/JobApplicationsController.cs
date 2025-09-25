using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StdControllers.Data;
using StdControllers.Models;
using TrackoApi.Models;

namespace StdControllers.Controllers;

[Route("api/[controller]")]
[ApiController]
public class JobApplicationsController : ControllerBase
{
    protected readonly ApplicationDbContext _context;
    public JobApplicationsController(ApplicationDbContext context)
    {
        _context = context;
    }


    [HttpGet(Name = "GetJobApplications")]
    public async Task<ActionResult<List<JobApplication>>> GetJobApplications()
    {
        return Ok(await _context.JobApplications
            .Include(j => j.Company)
            .Include(j => j.Status)
            .ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<JobApplication?>> GetJobApplicationById(int id)
    {
        var jobApplication = await _context.JobApplications
        .Include(j => j.Company)
        .Include(j => j.Status)
        .FirstOrDefaultAsync(j => j.Id == id);

        if (jobApplication is null)
            return NotFound();

        return Ok(jobApplication);
    }

    [HttpPost]
    public async Task<ActionResult<JobApplication>> CreateJobApplication(JobApplication newJobApp)
    {
        if (newJobApp is null)
            return BadRequest();

        _context.JobApplications.Add(newJobApp);
        
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetJobApplicationById), new { id = newJobApp.Id }, newJobApp);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateJobApplication(int id, JobApplication updatedJobApplication)
    {
        var jobApplicationToUpdate = await _context.JobApplications.FindAsync(id);

        if (jobApplicationToUpdate is null)
            return NotFound();

        jobApplicationToUpdate.Role = updatedJobApplication.Role;
        jobApplicationToUpdate.Location = updatedJobApplication.Location;
        jobApplicationToUpdate.ApplicationDate = updatedJobApplication.ApplicationDate;
        jobApplicationToUpdate.CompanyId = updatedJobApplication.CompanyId;
        jobApplicationToUpdate.StatusId = updatedJobApplication.StatusId;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteJobApplication(int id)
    {
        var jobApplicationToDelete = await _context.JobApplications.FindAsync(id);

        if (jobApplicationToDelete is null)
            return NotFound();

        _context.JobApplications.Remove(jobApplicationToDelete);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}