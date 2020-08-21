using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Coravel;
using Coravel.Invocable;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Shared;

namespace GitHubImporter
{
    public static class Register
    {
        public static IServiceCollection AddGitHubImporterService(this IServiceCollection services, string gitHubUser)
        {
            services
                .AddTransient<ImportGitHubReposFromAPI>()
                .AddScheduler()
                .AddHttpClient("GitHubRepoImport",
                    c =>
                    {
                        c.BaseAddress = new System.Uri($"https://api.github.com/users/{gitHubUser}/repos");
                        c.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
                        c.DefaultRequestHeaders.Add("User-Agent", "GitHubImporter");
                    });

            return services;
        }

        public static IApplicationBuilder UseGitHubImporterService(this IApplicationBuilder app)
        {
            app.ApplicationServices.UseScheduler(s =>
            {
                s.Schedule<ImportGitHubReposFromAPI>()
                    .EveryThirtySeconds()
                    .PreventOverlapping(nameof(ImportGitHubReposFromAPI));
            });
            return app;
        }
    }

    internal class ImportGitHubReposFromAPI : IInvocable
    {
        private readonly HttpClient _http;
        private readonly IImportGitHubRepos _importRecords;

        public ImportGitHubReposFromAPI(IHttpClientFactory factory, IImportGitHubRepos importRecords)
        {
            this._http = factory.CreateClient("GitHubRepoImport");
            this._importRecords = importRecords;
        }

        public async Task Invoke()
        {
            // We won't worry about paging, etc.
            using (var stream = await this._http.GetStreamAsync(""))
            {
                var repos = await JsonSerializer.DeserializeAsync<List<RepoJSONResponse>>(stream);
                var reposAsImportType = repos
                    .Select(RepoJSONResponse.ToGitHubRepoDTO)
                    .ToList();
                await this._importRecords.ImportAsync(reposAsImportType);
            }
        }

        internal class RepoJSONResponse
        {
            public int id { get; set; }
            public string full_name { get; set; }
            public string description { get; set; }
            public int? stargazers_count { get; set; }

            public static GitHubRepoDTO ToGitHubRepoDTO(RepoJSONResponse me)
            {
                return new GitHubRepoDTO
                {
                    Path = me.full_name,
                    Description = me.description,
                    Stars = me.stargazers_count ?? 0,
                    GitHubId = me.id
                };
            }
        }
    }
}