using System;
using System.Threading.Tasks;
using API.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;
using AutoMapper.QueryableExtensions;
using AutoMapper;
using Domain;
using System.Collections.Generic;
using API.Core;
using FluentValidation.Results;
using API.Validator;
using System.Linq;


namespace API.Controllers
{
    public class ActorController : FlickListBaseController
    {
        public ActorController(DataContext context, IMapper mapper) : base(context, mapper)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActors()
        {
            var actors = await _context.People.ProjectTo<PersonDTO>(_mapper.ConfigurationProvider).ToListAsync();

            if (actors == null) return NotFound("No actors found");

            return Ok(actors);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetailedActor(Guid id)
        {
            var actor = await _context.People.ProjectTo<PersonDetailDTO>(_mapper.ConfigurationProvider).SingleOrDefaultAsync(p => p.Id == id);

            if (actor == null) return NotFound("Actor not found");

            return Ok(actor);
        }
    }
}