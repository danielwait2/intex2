using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieProject.API.Data;

namespace MovieProject.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private MovieDbContext _movieContext;

        public MovieController(MovieDbContext temp) => _movieContext = temp;

        [HttpGet("AllProjects")]
        public IActionResult GetProjects(int pageSize = 10, int pageNum = 1, [FromQuery] List<string>? projectTypes = null)
        {
            var query = _movieContext.Movies.AsQueryable();

            if (projectTypes != null && projectTypes.Any())
            {
                query = query.Where(p => projectTypes.Contains(p.ProjectType));
            }

            var totalNumProjects = query.Count();

            var something = query
                .Skip((pageNum-1) * pageSize)
                .Take(pageSize)
                .ToList();

            var someObject = new
            {
                Projects = something,
                TotalNumProjects = totalNumProjects
            };

            return Ok(someObject);
        }

        [HttpGet("GetProjectTypes")]
        public IActionResult GetProjectTypes ()
        {
            var projectTypes = _movieContext.Movies
                .Select(p => p.ProjectType)
                .Distinct()
                .ToList();

            return Ok(projectTypes);
        }

        [HttpPost("AddProject")]
        public IActionResult AddMovie([FromBody] Movie newMovie)
        {
            _movieContext.Movies.Add(newMovie);
            _movieContext.SaveChanges();
            return Ok(newMovie);
        }

        [HttpPut("UpdateProject/{projectId}")]
        public IActionResult UpdateProject(int projectId, [FromBody] Movie updatedMovie)
        {
            var existingMovie = _movieContext.Movies.Find(projectId);

            existingMovie.ProjectName = updatedMovie.ProjectName;
            existingMovie.ProjectType = updatedMovie.ProjectType;
            existingMovie.ProjectRegionalProgram = updatedMovie.ProjectRegionalProgram;
            existingMovie.ProjectImpact = updatedMovie.ProjectImpact;
            existingMovie.ProjectPhase = updatedMovie.ProjectPhase;
            existingMovie.ProjectFunctionalityStatus = updatedMovie.ProjectFunctionalityStatus;

            _movieContext.Movies.Update(existingMovie);
            _movieContext.SaveChanges();

            return Ok(existingMovie);
        }

        [HttpDelete("DeleteProject/{projectId}")]
        public IActionResult DeleteProject(int projectId)
        {
            var project = _movieContext.Movies.Find(projectId);

            if (project == null)
            {
                return NotFound(new {message = "Project not found"});
            }

            _movieContext.Movies.Remove(project);
            _movieContext.SaveChanges();

            return NoContent();
        }

    }
}
