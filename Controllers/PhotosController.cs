using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SehirRehberi.API.Data;
using SehirRehberi.API.Dtos;
using SehirRehberi.API.Helpers;
using SehirRehberi.API.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SehirRehberi.API.Controllers
{
    [Route("api/cities/{cityid}/photos")]
    [ApiController]
    public class PhotosController : Controller
    {

        private IAppRepository _appRepository;
        private IMapper _mapper;
        private IOptions<CloudinarySettings> _cloudinaryConfig; //servis kisminda configire etmistik
        private Cloudinary _cloudinary;
        
        public PhotosController(IAppRepository appRepository, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _appRepository = appRepository;
            _mapper = mapper;
            _cloudinaryConfig = cloudinaryConfig;

            Account account = new Account(_cloudinaryConfig.Value.CloudName, _cloudinaryConfig.Value.ApiKey, _cloudinaryConfig.Value.ApiSecret);
            _cloudinary = new Cloudinary(account);

            
        }

        [HttpPost]
        public IActionResult AddPhotoCity(int cityId, [FromForm]PhotoForCreationDto photoForCreationDto)
        {
            var city = _appRepository.GetCityById(cityId);

            if (city==null)
            {
                return BadRequest("Could not find the city");
            }


            //var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value); Burada sorun var sonra incele

            //if (currentUserId != city.UserId)
            //{
            //    return Unauthorized();
            //}

            var file = photoForCreationDto.File;

            var uploadResult = new ImageUploadResult();

            if (file.Length>0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.Name, stream)
                    };
                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }

            photoForCreationDto.Url = uploadResult.Uri.ToString();
            photoForCreationDto.PublicId = uploadResult.PublicId;

            var photo = _mapper.Map<Photo>(photoForCreationDto);
            photo.City = city;

            if (!city.Photos.Any(p=>p.IsMain))
            {
                photo.IsMain = true;
            }

            _appRepository.Add(photo);

            if (_appRepository.SaveAll())
            {
                var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);
                return CreatedAtRoute("GetPhoto", new { id = photo.Id }, photoToReturn); // Http 201 mesajinin basilarak GetPhoto metoduna route ediyor
            }

            return BadRequest("Could not add the photo");
            
        }

        [HttpGet("{id}",Name ="GetPhoto")]
        public IActionResult GetPhoto(int id)
        {
            var photoFromDb = _appRepository.GetPhoto(id);
            var photo = _mapper.Map<PhotoForReturnDto>(photoFromDb);

            return Ok(photo);
        }
    }
}
