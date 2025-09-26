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
public class JobApplicationsController : ControllerBase
{
    protected readonly ApplicationDbContext _context;
    public JobApplicationsController(ApplicationDbContext context)
    {
        _context = context;
    }


    [HttpGet(Name = "GetJobApplications")]
    public async Task<ActionResult<List<JobApplicationDto>>> GetJobApplications()
    {
        var jobApps = await _context.JobApplications
                    .Include(j => j.Status)
                    .AsNoTracking()
                    .ToListAsync();

        var jobAppsDto = new List<JobApplicationDto>();
        foreach (JobApplication j in jobApps)
        {
            jobAppsDto.Add(j.Adapt<JobApplicationDto>());
        }

        return Ok(jobAppsDto);
    }

    [HttpGet("with-company")]
    public async Task<ActionResult<List<JobApplicationWithCompanyDto>>> GetJobApplicationsWithCompany()
    {
        var jobApps = await _context.JobApplications
                    .Include(j => j.Company)
                    .Include(j => j.Status)
                    .AsNoTracking()
                    .ToListAsync();

        var jobAppsDto = new List<JobApplicationWithCompanyDto>();
        foreach (JobApplication j in jobApps)
        {
            // null check
            j.Company?.Adapt<CompanyDto>();

            jobAppsDto.Add(j.Adapt<JobApplicationWithCompanyDto>());
        }

        return Ok(jobAppsDto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<JobApplicationDto?>> GetJobApplicationById(int id)
    {
        var jobApplication = await _context.JobApplications
        .Include(j => j.Status)
        .AsNoTracking()
        .FirstOrDefaultAsync(j => j.Id == id);

        if (jobApplication is null)
            return NotFound();

        return Ok(jobApplication.Adapt<JobApplicationWithCompanyDto>());
    }

    [HttpGet("{id}/with-company")]
    public async Task<ActionResult<JobApplicationWithCompanyDto?>> GetJobApplicationWithCompanyById(int id)
    {
        var jobApplication = await _context.JobApplications
        .Include(j => j.Company)
        .Include(j => j.Status)
        .AsNoTracking()
        .FirstOrDefaultAsync(j => j.Id == id);

        if (jobApplication is null)
            return NotFound();

        var companiesDto = new List<CompanyDto>();
        if (jobApplication.Company is not null)
            jobApplication.Company?.Adapt<CompanyDto>();

        return Ok(jobApplication.Adapt<JobApplicationWithCompanyDto>());
    }

    [HttpPost]
    public async Task<ActionResult<JobApplicationDto>> CreateJobApplication(JobApplicationRequestDto newJobAppDto)
    {
        if (newJobAppDto is null)
            return BadRequest();

        var jobApp = newJobAppDto.Adapt<JobApplication>();
        _context.JobApplications.Add(jobApp);

        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetJobApplicationById), new { id = jobApp.Id }, newJobAppDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateJobApplication(int id, JobApplicationRequestDto updatedJobApplicationDto)
    {
        var jobApplicationToUpdate = await _context.JobApplications.FindAsync(id);

        if (jobApplicationToUpdate is null)
            return NotFound();

        jobApplicationToUpdate.Role = updatedJobApplicationDto.Role;
        jobApplicationToUpdate.Location = updatedJobApplicationDto.Location;
        jobApplicationToUpdate.ApplicationDate = updatedJobApplicationDto.ApplicationDate;
        jobApplicationToUpdate.CompanyId = updatedJobApplicationDto.CompanyId;

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