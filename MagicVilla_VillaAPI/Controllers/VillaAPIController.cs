using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Data;
using Microsoft.AspNetCore.JsonPatch;
using MagicVilla_VillaAPI.Logging;
using System;
using System.Diagnostics;
using Amazon.DynamoDBv2.DataModel;
using System.Text.Json;
using MagicVilla_VillaAPI.Helpers;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/VillaAPI")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        private readonly ILogging _logger;
        private readonly IDynamoDBContext _dynamoDBContext;
        public VillaAPIController(ILogging logger, IDynamoDBContext dynamoDBContext)
        {
            _logger = logger;
            _dynamoDBContext = dynamoDBContext;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetVillas()
        {
            _logger.Log("Getting all villas", "");
            var conditions = new List<ScanCondition>();
            var villas = await _dynamoDBContext.ScanAsync<VillaDB>(conditions).GetRemainingAsync();
            var villasDtoList = villas.Select(v => new VillaDto(v));
            return Ok(villasDtoList);
        }

        [HttpGet("{id}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetVilla(int id)
        {
            if (id == 0)
            {
                _logger.Log("Get Villa Error with Id " + id, "error");
                return BadRequest();
            }
            var villa = await _dynamoDBContext.LoadAsync<VillaDB>(id);
            if (villa == null)
            {
                return NotFound();
            }
            VillaDto villaDto = new VillaDto(villa);
            return Ok(villaDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CreateVilla([FromBody] VillaDto villaDto)
        {
            var conditions = new List<ScanCondition>();
            var villas = await _dynamoDBContext.ScanAsync<VillaDB>(conditions).GetRemainingAsync();

            if (villas.FirstOrDefault(u => u.Name.ToLower() == villaDto.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError", "Villa already exists!");
                return BadRequest(ModelState);
            }
            if (villaDto == null)
            {
                return BadRequest();
            }
            if (villaDto.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            villaDto.Id = villas.OrderByDescending(v => v.Id).FirstOrDefault().Id + 1;
            VillaDB villaDB = new VillaDB(villaDto);
            await _dynamoDBContext.SaveAsync(villaDB);
            await VillaSQS.SendMassageToNewAddedVillasSQS($"add new villa - {villaDB.Name}");
            return CreatedAtRoute("GetVilla", new { id = villaDto.Id }, villaDto); //add the url of new obj to response
        }

        [HttpDelete("{id}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var villa = await _dynamoDBContext.LoadAsync<VillaDB>(id);
            if (villa == null)
            {
                return NotFound();
            }
            await _dynamoDBContext.DeleteAsync<VillaDB>(id);
            return NoContent();
        }

        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaDto villaDto)
        {
            if (villaDto == null || id != villaDto.Id)
            {
                return BadRequest();
            }
            var villaDB = await _dynamoDBContext.LoadAsync<VillaDB>(id);
            if (villaDB == null)
            {
                return NotFound();
            }
            villaDB = new VillaDB(villaDto);
            await _dynamoDBContext.SaveAsync(villaDB);
            return NoContent();
        }

        //Microsoft.AspNetCore.JsonPatch
        //Microsoft.AspNetCore.Mvc.NewtonsoftJson
        //
        //in swagger according to https://jsonpatch.com/ for replace
        // path: "/name"
        // op: "replace"
        // value: "new value"
        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaDto> patchDto)
        {
            if (patchDto == null || id == 0)
            {
                return BadRequest();
            }
            var villaDB = await _dynamoDBContext.LoadAsync<VillaDB>(id);
            if (villaDB == null)
            {
                return BadRequest();
            }
            var villa = new VillaDto(villaDB);
            patchDto.ApplyTo(villa, ModelState); //in ModelState will be saved errors
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            villaDB = new VillaDB(villa);
            await _dynamoDBContext.SaveAsync(villaDB);
            return NoContent();
        }

    }
}
