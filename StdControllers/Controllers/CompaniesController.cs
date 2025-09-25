using System.Threading.Tasks;
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
    public async Task<ActionResult<List<Company>>> GetCompanies()
    {
        return Ok(await _context.Companies
            .Include(c => c.JobApplications)
            .ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Company?>> GetCompanyById(int id)
    {
        var company = await _context.Companies
            .Include(c => c.JobApplications)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (company is null)
            return NotFound();

        return Ok(company);
    }

    [HttpPost]
    public async Task<ActionResult<Company>> CreateCompany(Company newCompany)
    {
        if (newCompany is null)
            return BadRequest();

        _context.Companies.Add(newCompany);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCompanyById), new { id = newCompany.Id }, newCompany);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateCompany(int id, Company updatedCompany)
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