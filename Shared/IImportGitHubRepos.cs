using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shared
{
    public interface IImportGitHubRepos
    {
        Task ImportAsync(List<GitHubRepoDTO> reposToImport);
    }
}