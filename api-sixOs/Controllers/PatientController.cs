using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using api_sixOs.Models;
using api_sixOs.Data;

namespace api_sixOs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly ApiContext _context;

        public PatientController(ApiContext context)
        {
            _context = context;
        }

        //Create
        [HttpPost]
        public JsonResult Create(Patient patient)
        {
            _context.Patients.Add(patient);

            _context.SaveChanges();

            return new JsonResult(Ok(patient));
        }

        //Read
        [HttpGet]
        public JsonResult Read()
        {
            var patients = _context.Patients.ToList();

            return new JsonResult(Ok(patients));
        }
    }
}
