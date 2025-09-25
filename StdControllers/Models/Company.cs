using System.Text.Json.Serialization;

namespace StdControllers.Models;

public class Company
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? HR { get; set; }
    public string? Headquarters { get; set; }

    public List<JobApplication>? JobApplications { get; set; }
}