using Bogus;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Person = HttpSpeedTestApi.Models.Person;

namespace HttpSpeedTestApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PersonController : ControllerBase
    {
        private static readonly List<Person> People;

        static PersonController()
        {
            // Bogus kullanarak mock data üretimi
            var personFaker = new Faker<Person>()
                .RuleFor(p => p.Id, f => f.IndexFaker + 1)
                .RuleFor(p => p.FirstName, f => f.Name.FirstName())
                .RuleFor(p => p.LastName, f => f.Name.LastName())
                .RuleFor(p => p.Age, f => f.Random.Int(18, 80))
                .RuleFor(p => p.Email, f => f.Internet.Email());

            People = personFaker.Generate(500000);  
        }

        [HttpGet]
        public ActionResult<IEnumerable<Person>> Get()
        {
            return Ok(People);
        }

        [HttpGet("{id}")]
        public ActionResult<Person> Get(int id)
        {
            var person = People.FirstOrDefault(p => p.Id == id);
            if (person == null)
            {
                return NotFound();
            }
            return Ok(person);
        }

        [HttpPost]
        public ActionResult<Person> Post([FromBody] Person newPerson)
        {
            //newPerson.Id = People.Count + 1;
            People.Add(newPerson);
            return CreatedAtAction(nameof(Get), new { id = newPerson.Id }, newPerson);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Person updatedPerson)
        {
            var person = People.FirstOrDefault(p => p.Id == id);
            if (person == null)
            {
                return NotFound();
            }
            person.FirstName = updatedPerson.FirstName;
            person.LastName = updatedPerson.LastName;
            person.Age = updatedPerson.Age;
            person.Email = updatedPerson.Email;
            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult Patch(int id, [FromBody] Dictionary<string, string> updates)
        {
            var person = People.FirstOrDefault(p => p.Id == id);
            if (person == null)
            {
                return NotFound();
            }

            if (updates.ContainsKey("FirstName"))
            {
                person.FirstName = updates["FirstName"];
            }
            if (updates.ContainsKey("LastName"))
            {
                person.LastName = updates["LastName"];
            }
            if (updates.ContainsKey("Age"))
            {
                person.Age = int.Parse(updates["Age"]);
            }
            if (updates.ContainsKey("Email"))
            {
                person.Email = updates["Email"];
            }

            return NoContent();
        }
    }
}
