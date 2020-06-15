using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SehirRehberi.API.Data;
using SehirRehberi.API.Dtos;
using SehirRehberi.API.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SehirRehberi.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : Controller
    {
        private IAppRepository _appRepository;
        private IMapper _mapper;
        public CitiesController(IAppRepository appRepository, IMapper mapper)
        {
            _appRepository = appRepository;
            _mapper = mapper;
        }

        // GET: /<controller>/
        public ActionResult GetCities()
        {
            var cities = _appRepository.GetCities();
            //    .Select(c =>
            //    new CityForListDto { Description =  c.Description,Name = c.Name,Id = c.Id,PhotoUrl = c.Photos.FirstOrDefault(p=>p.IsMain==true).Url}).ToList();
            //Data Transfering Object kullandik city tablosunda sadece istedigimiz alanlarin gözükmesi icin fakat bu herseferinde cok vakit alacagindan AutoMapper kullancagiz.
            var citiesToReturn = _mapper.Map<List<CityForListDto>>(cities);
            return Ok(citiesToReturn);
        }

        [HttpPost]
        [Route("add")]
        public ActionResult Add([FromBody]City city)
        {
            _appRepository.Add(city);
            _appRepository.SaveAll();
            return Ok(city);

        }

        [HttpGet]
        [Route("detail")]
        public ActionResult GetCitiesById(int id)
        {
            var city = _appRepository.GetCityById(id);
            var cityToReturn = _mapper.Map<CityForDetailDto>(city);
            return Ok(cityToReturn);
        }

        [HttpGet]
        [Route("Photos")]
        public ActionResult GetPhotosByCity(int cityId)
        {
            var photos = _appRepository.GetPhotoByCity(cityId);
            return Ok(photos);

        }

    }
}
