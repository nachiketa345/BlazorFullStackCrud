using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace BlazorFullStackCrud.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuperHeroController : ControllerBase
    {
        private readonly DataContext _context;

        public SuperHeroController(DataContext context)
        {
            _context = context;
        }
   
        [HttpGet]
        public async Task<ActionResult<List<SuperHero>>> GetSuperHeroes()
        {
            var heroes =await _context.SuperHeroes.ToListAsync();
             return Ok(heroes);
        }

        [HttpGet("comics")]
        public async Task<ActionResult<List<Comic>>> GetComics()
        {
            var comics = await _context.Comics.ToListAsync();
            return Ok(comics);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SuperHero>> GetSingleHero(int id)
        {
            var hero = await _context.SuperHeroes.Include(h => h.Comic)
                .FirstOrDefaultAsync(h => h.Id == id);
            if(hero== null)
            {
                return NotFound("Sorry, no hero here. :/");
            }
            return Ok(hero);
        }

        [HttpPost]
        public async Task<ActionResult<List<SuperHero>>> CreateSuperHero(SuperHero hero)
        {
            hero.Comic = null;
            _context.SuperHeroes.Add(hero);
            await _context.SaveChangesAsync();

            return Ok(await GetDbHeroes());
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<List<SuperHero>>> UpdateSuperHero(SuperHero hero, int id)
        {
            var dbhero = await _context.SuperHeroes.
                Include(sh => sh.Comic).FirstOrDefaultAsync(sh => sh.Id == id);
            if(dbhero==null)
                return NotFound("Sorry but no hero for you. /");

            dbhero.FirstName = hero.FirstName;
            dbhero.LastName = hero.LastName;
            dbhero.HeroName = hero.HeroName;
            dbhero.ComicId = hero.ComicId;

            await _context.SaveChangesAsync();

            return Ok(await GetDbHeroes());
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<List<SuperHero>>> DeleteSuperHero(int id)
        {
            var dbhero = await _context.SuperHeroes.
                Include(sh => sh.Comic).FirstOrDefaultAsync(sh => sh.Id == id);
            if (dbhero == null)
                return NotFound("Sorry but no hero for you. /");

             _context.Remove(dbhero);


            await _context.SaveChangesAsync();

            return Ok(await GetDbHeroes());
        }


        private async Task<List<SuperHero>> GetDbHeroes()
        {
            return await _context.SuperHeroes.Include(sh => sh.Comic).ToListAsync();
        }

    }
}
