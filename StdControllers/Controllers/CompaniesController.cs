using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StdControllers.Data;
using StdControllers.Models;

namespace StdControllers.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CompaniesController : ControllerBase
{
    protected readonly ApplicationDbContext _context;
    public CompaniesController(ApplicationDbContext context)
    {
        _context = context;
    }


    [HttpGet(Name = "GetCompanies")]
    public async Task<ActionResult<List<CompanyDto>>> GetCompanies()
    {
        var companies = await _context.Companies
            .Include(c => c.JobApplications)
            .AsNoTracking()
            .ToListAsync();

        var companiesDto = new List<CompanyDto>();

        foreach (Company c in companies)
        {
            companiesDto.Add(c.Adapt<Company, CompanyDto>());
        }

        return Ok(companiesDto);
    }

    [HttpGet("with-job-applications")]
    public async Task<ActionResult<List<CompanyWithJobApplicationDto>>> GetCompaniesWithJobApps()
    {
        var companies = await _context.Companies
            .Include(c => c.JobApplications)
            .AsNoTracking()
            .ToListAsync();

        var companiesDto = new List<CompanyWithJobApplicationDto>();

        foreach (Company c in companies)
        {
            if (c.JobApplications is not null)
            {
                var jobAppsDto = new List<JobApplicationDto>();
                foreach (JobApplication j in c.JobApplications)
                {
                    j.Adapt<JobApplicationDto>();
                }
            }

            companiesDto.Add(c.Adapt<Company, CompanyWithJobApplicationDto>());
        }

        return Ok(companiesDto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CompanyDto?>> GetCompanyById(int id)
    {
        var company = await _context.Companies
            .Include(c => c.JobApplications)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (company is null)
            return NotFound();

        return Ok(company.Adapt<CompanyDto>());
    }

    [HttpGet("{id}/with-job-applications")]
    public async Task<ActionResult<CompanyWithJobApplicationDto?>> GetCompanyWithJobAppsById(int id)
    {
        var company = await _context.Companies
            .Include(c => c.JobApplications)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (company is null)
            return NotFound();

        if (company.JobApplications is not null)
        {
            var jobAppsDto = new List<JobApplicationDto>();
            foreach (JobApplication j in company.JobApplications)
            {
                j.Adapt<JobApplicationDto>();
            }
        }

        return Ok(company.Adapt<CompanyWithJobApplicationDto>());
    }

    [HttpPost]
    public async Task<ActionResult<CompanyDto>> CreateCompany(CompanyRequestDto newCompany)
    {
        if (newCompany is null)
            return BadRequest();

        var company = newCompany.Adapt<Company>();
        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCompanyById), new { id = company.Id }, newCompany);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateCompany(int id, CompanyRequestDto updatedCompany)
    {
        var companyToUpdate = await _context.Companies.FindAsync(id);

        if (companyToUpdate is null)
            return NotFound();

        companyToUpdate.Name = updatedCompany.Name;
        companyToUpdate.HR = updatedCompany.HR;
        companyToUpdate.Headquarters = updatedCompany.Headquarters;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCompany(int id)
    {
        var companyToDelete = await _context.Companies.FindAsync(id);

        if (companyToDelete is null)
            return NotFound();

        _context.Companies.Remove(companyToDelete);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}