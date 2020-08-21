using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shared;
using Web.Data;
using Web.Models;

namespace Web.GitHubImport
{
    public class ImportGitHubRepoClient : IImportGitHubRepos
    {
        private GitHubStatsDbContext _db;

        public ImportGitHubRepoClient(GitHubStatsDbContext db)
        {
            this._db = db;
        }

        public async Task ImportAsync(List<GitHubRepoDTO> reposToImport)
        {
            var existing = await this._db.Repositories.ToListAsync();
            var existingRepos = existing.ToDictionary(r => r.GitHubId);
            
            foreach (var toImport in reposToImport)
            {
                if (existingRepos.ContainsKey(toImport.GitHubId))
                {
                    var toUpdate = existingRepos[toImport.GitHubId];
                    toUpdate.FullPath = toImport.Path;
                    toUpdate.Description = toImport.Description;
                    toUpdate.StarsAmount = toImport.Stars;
                    toUpdate.LastUpdated = DateTimeOffset.UtcNow;
                }
                else
                {
                    await this._db.AddAsync(new GitHubRepository
                    {
                        Id = Guid.NewGuid(),
                        GitHubId = toImport.GitHubId,
                        FullPath = toImport.Path,
                        Description = toImport.Description,
                        StarsAmount = toImport.Stars,
                        LastUpdated = DateTimeOffset.UtcNow
                    });
                }
            }
            await this._db.SaveChangesAsync();
        }
    }
}