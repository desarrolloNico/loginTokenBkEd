using System.Collections.Generic;
using System.Linq;
using contactos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using AutoMapper;
using contactos.DTOs;

using Microsoft.AspNetCore.Authorization;

namespace contactos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactoController: ControllerBase
    {
        private readonly Context _context;
        private readonly IMapper _mapper;

        public ContactoController(Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<Contacto>>> Get()
        {
            return await _context.Contactos.ToListAsync();
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Contacto>> GetById(int id)
        {
            var miContacto = await _context.Contactos.FindAsync(id);
            
            if(miContacto== null) { return NotFound(); }

            return miContacto;
        }

        [HttpPost]
        public async Task<ActionResult<Contacto>> Create([FromBody] ContactoDto contactoDto)
        {
            if(contactoDto == null) { return BadRequest(); }

            var entidad = _mapper.Map<Contacto>(contactoDto);
            _context.Contactos.Add(entidad);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new{id = entidad.Id}, entidad);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] ContactoDto contactoDto)
        {
            if(id == 0 ) { return BadRequest(); }

            if(contactoDto == null){ return BadRequest(); }

            if(_context.Contactos.Count(x => x.Id == id)<1) { return NotFound(); }

            var entidad = _mapper.Map<Contacto>(contactoDto);
            entidad.Id = id;
            _context.Entry(entidad).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            if(id == 0 ) { return BadRequest(); }

            var existe = _context.Contactos.AnyAsync(x => x.Id == id);
            
            if(existe == null) { return NotFound(); }

            _context.Contactos.Remove(new Contacto{Id = id});
            await _context.SaveChangesAsync();
            
            return NoContent();
        }
    }
}