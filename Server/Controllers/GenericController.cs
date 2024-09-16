using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ServerLibrary.Repositories.Contracts;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenericController<T>(IGenericRepositoryInterface<T> genericRepository) : Controller where T : class
    {
        [HttpGet("all")]
        public async Task<IActionResult> GetAll() => Ok(await genericRepository.GetAll());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0) return BadRequest("Invalid ID.");
            var entity = await genericRepository.GetById(id);
            return entity is null ? NotFound() : Ok(entity);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteById(int id)
        {
            if (id <= 0) return BadRequest("Invalid ID.");
            return Ok(await genericRepository.DeleteById(id));
        }

        [HttpPut]
        public async Task<IActionResult> Update(T entity)
        {
            if (entity is null) return BadRequest("Entity is null.");
            return Ok(await genericRepository.Update(entity));
        }

        [HttpPost]
        public async Task<IActionResult> Create(T entity)
        {
            if (entity is null) return BadRequest("Entity is null.");
            return Ok(await genericRepository.Create(entity));
        }
    }
}