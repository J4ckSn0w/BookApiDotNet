using BookAppVS2019.Dtos;
using BookAppVS2019.Models;
using BookAppVS2019.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookAppVS2019.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : Controller
    {
        private ICountryRepository _countryRepository;

        public CountriesController(ICountryRepository countryRepositorty)
        {
            _countryRepository = countryRepositorty;
        }

        //api/countries
        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CountryDto>))]
        public IActionResult GetCountries()
        {
            var countries = _countryRepository.GetCountries().ToList();

            if (!ModelState.IsValid) //En caso de que la llamada sea erronea
                return BadRequest(ModelState);

            var countriesDto = new List<CountryDto>();

            foreach (var country in countries)
            {
                countriesDto.Add(new CountryDto
                {
                    Id = country.Id,
                    Name = country.Name
                });
            }

            return Ok(countriesDto);
        }

        //api/countries/countryId/{countryId}
        [HttpGet("{countryId}", Name = "GetCountry")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(CountryDto))]
        public IActionResult GetCountry(int countryId)
        {
            if (!_countryRepository.CountryExists(countryId))
                return NotFound();

            var country = _countryRepository.GetCountry(countryId);

            if (!ModelState.IsValid) //En caso de que la llamada sea erronea
                return BadRequest(ModelState);

            var countryDto = new CountryDto() {
                Id = country.Id,
                Name = country.Name
            };

            return Ok(countryDto);
        }

        //api/countries/authors/{authorId}
        [HttpGet("authors/{authorId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(CountryDto))]
        public IActionResult GetCountryOfAnAuthor(int authorId)
        {
            var country = _countryRepository.GetCountryOfAnAuthor(authorId);

            if (!ModelState.IsValid)
                return BadRequest();

            var countryDto = new CountryDto()
            {
                Id = country.Id,
                Name = country.Name
            };

            return Ok(countryDto);
        }

        //api/countries/countryId/Authors
        [HttpGet("{countryId}/authors")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AuthorDto>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetAuthosFromACountry(int countryId)
        {
            if (!_countryRepository.CountryExists(countryId))
                return NotFound();
            var authors = _countryRepository.GetAuthorsFromCountry(countryId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var authorsDto = new List<AuthorDto>();

            foreach (var author in authors)
            {
                authorsDto.Add(new AuthorDto
                {
                    authorId = author.Id,
                    firstName = author.FirstName,
                    lastName = author.LastName
                });
            }

            return Ok(authorsDto);
        }

        //api/countries
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Country))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult CreateCountry([FromBody] Country countryToCreate)
        {
            if (countryToCreate == null)
                return BadRequest();

            var country = _countryRepository.GetCountries()
                .Where(c => c.Name.Trim().ToUpper() == countryToCreate.Name.Trim().ToUpper())
                .FirstOrDefault();

            if (country != null)
            {
                ModelState.AddModelError("", $"Country {countryToCreate.Name} already exists.");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_countryRepository.CreateCountry(countryToCreate))
            {
                ModelState.AddModelError("", $"Something went wrong saving {countryToCreate.Name}.");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetCountry", new { countryId = countryToCreate.Id }, countryToCreate);
        }

        //api/countries/countryId
        [HttpPut("{countryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult UpdateCountry(int countryId, [FromBody] Country updatedCountryInfo)
        {
            if (updatedCountryInfo == null)
                return BadRequest(ModelState);
            if (countryId != updatedCountryInfo.Id)
                return BadRequest(ModelState);

            if (!_countryRepository.CountryExists(countryId))
                return NotFound();

            if (_countryRepository.IsDuplicateCountryName(countryId, updatedCountryInfo.Name))
            {
                ModelState.AddModelError("", $"Country { updatedCountryInfo.Name } already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_countryRepository.UpdateCountry(updatedCountryInfo))
            {
                ModelState.AddModelError("", $"Something went wrong saving { updatedCountryInfo.Name }.");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{countryId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult DeleteCountry(int countryId)
        {
            if (!_countryRepository.CountryExists(countryId))
                return NotFound();

            var countryToDelete = _countryRepository.GetCountry(countryId);

            if(_countryRepository.GetAuthorsFromCountry(countryId).Count > 0)
            {
                ModelState.AddModelError("", $"Country {countryToDelete.Name}, cannot be deleted because it is used by at least one author.");

                return StatusCode(409, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if(!_countryRepository.DeleteCountry(countryToDelete))
            {
                ModelState.AddModelError("", $"Something went wrong deleting {countryToDelete.Name}.");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

    }
}
