using System;
using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class GitHubRepository
    {
        [Key]
        public Guid Id { get; set; }
        public int GitHubId { get; set; }
        public string FullPath { get; set; }
        public string Description { get; set; }
        public int StarsAmount { get; set; }
        public DateTimeOffset LastUpdated { get; set; }

        public GitHubRepositoryViewModel ToViewModel() =>
            new GitHubRepositoryViewModel
            {
                Id = this.Id,
                Path = this.FullPath,
                Description = this.Description,
                Stars = this.StarsAmount,
                LastUpdated = this.LastUpdated.ToString("g")
            };
    }

    public class GitHubRepositoryViewModel
    {
        public Guid Id { get; set; }
        public string Path { get; set; }
        public string Description { get; set; }
        public int Stars { get; set; }
        public string LastUpdated { get; set; }
    }
}