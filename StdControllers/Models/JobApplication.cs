using System.Text.Json.Serialization;
using TrackoApi.Models;

namespace StdControllers.Models;

public class JobApplication
{
    public int Id { get; set; }
    public required string Role { get; set; }
    public string? Location { get; set; }
    public DateOnly? ApplicationDate { get; set; }

    public int? StatusId { get; set; }
    public JobApplicationStatuses? Status { get; set; }

    public int? CompanyId { get; set; }
    public Company? Company { get; set; }
}