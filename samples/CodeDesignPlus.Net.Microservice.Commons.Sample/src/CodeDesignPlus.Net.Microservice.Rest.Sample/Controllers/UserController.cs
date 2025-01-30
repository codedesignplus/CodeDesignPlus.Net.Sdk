using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CodeDesignPlus.Net.Microservice.Rest.Sample.Controllers
{
    /// <summary>
    /// Controller to manage the users.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        /// <summary>
        /// Gets all users.
        /// </summary>
        /// <returns>A list of users.</returns>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }

        /// <summary>
        /// Gets a user by ID.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <returns>The user with the specified ID.</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            return Ok();
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <returns>The result of the creation operation.</returns>
        [HttpPost]
        public IActionResult Post()
        {
            return Ok();
        }

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <returns>The result of the update operation.</returns>
        [HttpPut("{id}")]
        public IActionResult Put(int id)
        {
            return Ok();
        }

        /// <summary>
        /// Deletes a user by ID.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <returns>The result of the deletion operation.</returns>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return Ok();
        }
    }
}
