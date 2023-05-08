﻿using Microsoft.AspNetCore.Http;
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

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/VillaAPI")]
    //[Route("api/[controller]")]
    [ApiController] // if using ModelState for validation
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
        public ActionResult<IEnumerable<VillaDto>> GetVillas()
        {
            _logger.Log("Getting all villas","");
            return Ok(VillaStore.villaList);
        }

        [HttpGet("{id}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetVilla(int id)
        {
            if (id == 0)
            {
                _logger.Log("Get Villa Error with Id " + id,"error");
                return BadRequest();
            }
            var villa = await _dynamoDBContext.LoadAsync<VillaDB>(id);
            if (villa == null)
            {
                return NotFound();
            }
            VillaDto villaDto = new VillaDto { Id = villa.Id, Name = villa.Name, Info = JsonSerializer.Deserialize<Info>(villa.Body) };
            return Ok(villaDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<VillaDto> CreateVilla([FromBody] VillaDto villaDto)
        {
            /*            if (!ModelState.IsValid) // if not using APIController
                        {
                            return BadRequest (ModelState);
                        }*/
            if (VillaStore.villaList.FirstOrDefault(u => u.Name.ToLower() == villaDto.Name.ToLower()) != null)
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
            villaDto.Id = VillaStore.villaList.OrderByDescending(u => u.Id).FirstOrDefault().Id+1;          
            VillaStore.villaList.Add(villaDto);
            return CreatedAtRoute("GetVilla",new { id = villaDto.Id }, villaDto); //add the url of new obj to response
        }

        [HttpDelete("{id}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteVilla (int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
            if (villa == null)
            {
                return NotFound();
            }
            VillaStore.villaList.Remove(villa);
            return NoContent();
        }

        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdateVilla (int id, [FromBody] VillaDto villaDto)
        {
            if (villaDto == null || id != villaDto.Id)
            {
                return BadRequest();
            }
            var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
            villa.Name = villaDto.Name;
            villa.Sqft = villaDto.Sqft;
            villa.Occupancy = villaDto.Occupancy;
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
        public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDto> patchDto)
        {
            if (patchDto == null || id == 0)
            {
                return BadRequest();
            }
            var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
            if (villa == null)
            {
                return BadRequest();
            }
            patchDto.ApplyTo(villa, ModelState); //in ModelState will be saved errors
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            return NoContent();
        }

    }
}
