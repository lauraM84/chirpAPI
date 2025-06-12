using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using chirpApi.Services.Model;
using chirpApi.Services.Services.Interfaces;
using chirpApi.Services.Model.Filters;
using chirpApi.Services.Model.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;

namespace chirpAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChirpsController : ControllerBase
    {
        private readonly IChirpsService _chirpsService;
        private readonly CinguettioContext _context;
        private readonly ILogger<ChirpsController> _logger;

        public ChirpsController(IChirpsService chirpsService, CinguettioContext context, ILogger<ChirpsController> logger)
        {
            _chirpsService = chirpsService;
            _context = context;
            _logger = logger;
        }

        // GET: api/Chirps
        [HttpGet]
        public async Task<IActionResult>GetChirpsByFilter([FromQuery] ChirpFilter filter)
        {
            _logger.LogInformation("ChirpsController: GetChirpsByFilter called with filter: {@Filter}", filter);

            var result = await _chirpsService.GetChirpsByFilter(filter);

            if (result == null || !result.Any())
            {
                return NoContent();
            }
            else
            {
                return Ok(result);
            }
        }

        // GET: api/Chirps
        [HttpGet("all")]
        public async Task<IActionResult> GetAllChirps()
        {
            var result = await _chirpsService.GetAllChirps();
            if (result == null || !result.Any())
            {
                _logger.LogInformation("no chirps found");
                return NoContent();
            }
            else
            {
                _logger.LogInformation("found {Count} chirps", result.Count);
                return Ok(result);
            }
        }

        // GET: api/Chirps/5
        [HttpGet("{id}")]
        public async Task<IActionResult>GetChirpById([FromRoute] int id)
        {
            var chirp = await _context.Chirps.FindAsync(id);

            if (chirp == null)
            {
                return NotFound();
            }

            return Ok(chirp);
        }

        // PUT: api/Chirps/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutChirp([FromRoute] int id, [FromBody] ChirpUpdateDTO chirp)
        {
            var result = await _chirpsService.UpdateChirp(id, chirp);

            if (result == false)
            {
               
                return BadRequest("Chirp not found or update failed. Please check the ID and data provided.");
            }
            return NoContent();
        }

        // POST: api/Chirps
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostChirp([FromBody] ChirpCreateDTO chirp)
        {
            var chirpId = await _chirpsService.CreateChirp(chirp);

            if (chirpId ==null )
            {
                return BadRequest("text obbligatorio");
            }
            

            return Created($"api/Chirps/{chirp}", chirpId);
        }

        // DELETE: api/Chirps/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChirp([FromRoute] int id)
        {
            int? result = await _chirpsService.DeleteChirp(id);

            if (result == null)
            {
                return BadRequest("chirp non esistente");
            }
            if (result == -1)
            {
                return BadRequest("eliminare prima i commenti");
            }

            return Ok(result);
        }
       
    }
}