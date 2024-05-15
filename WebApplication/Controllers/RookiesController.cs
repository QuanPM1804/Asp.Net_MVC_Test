using OfficeOpenXml;
using Asp_MVC1.Models;
using Microsoft.AspNetCore.Mvc;

namespace Asp_MVC1.Controllers
{
    [Route("NashTech/Rookies")]
    public class RookiesController : Controller
    {
        private readonly IPersonRepository _personRepository;

        public RookiesController(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View(_personRepository.GetAll());
        }
        [HttpGet("MaleMembers")]
        public IActionResult MaleMembers()
        {
            var maleMembers = _personRepository.GetAll().Where(p => p.Gender == "Male").ToList();
            return View("Result", maleMembers);
        }
        [HttpGet("OldestMember")]
        public IActionResult OldestMember()
        {
            var oldestMember = _personRepository.GetAll().OrderBy(p => p.DateOfBirth).FirstOrDefault();
            return View("Result", oldestMember);
        }
        [HttpGet("FullNames")]
        public IActionResult FullNames()
        {
            var fullNames = _personRepository.GetAll().Select(p => $"{p.LastName} {p.FirstName}").ToList();
            return View("Result", fullNames);
        }
        [HttpGet("FilterByBirthYear")]
        public IActionResult FilterByBirthYear(int year)
        {
            var filteredMembers = _personRepository.GetAll().Where(p => p.DateOfBirth.Year == year).ToList();
            return View("Result", filteredMembers);
        }
        [HttpGet("ExportToExcel")]
        public IActionResult ExportToExcel()
        {
            var stream = new MemoryStream();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Persons");
                worksheet.Cells["A1"].LoadFromCollection(_personRepository.GetAll().Select(p => new
                {
                    p.FirstName,
                    p.LastName,
                    p.Gender,
                    DateOfBirth = p.DateOfBirth.ToString("yyyy-MM-dd"),
                    p.PhoneNumber,
                    p.BirthPlace,
                    p.IsGraduated
                }), true);

                package.Save();
            }

            stream.Position = 0;
            var fileName = $"Persons_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        [HttpPost]
        public IActionResult HandleOption(int option)
        {
            switch (option)
            {
                case 1:
                    return RedirectToAction("MaleMembers");
                case 2:
                    return RedirectToAction("OldestMember");
                case 3:
                    return RedirectToAction("FullNames");
                case 4:
                    return RedirectToAction("FilterByBirthYear", new { year = 2000 });
                case 5:
                    return RedirectToAction("FilterByBirthYear", new { year = 2001 });
                case 6:
                    return RedirectToAction("FilterByBirthYear", new { year = 1999 });
                case 7:
                    return RedirectToAction("ExportToExcel");
                case 8:
                    return RedirectToAction("Create");
                default:
                    return RedirectToAction("Index");
            }
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Person person)
        {
            if (ModelState.IsValid)
            {
                _personRepository.Create(person);
                return RedirectToAction(nameof(Index));
            }

            return View("Create", person);
        }

        [HttpGet("Detail/{id}")]
        public IActionResult Detail(int id)
        {
            var person = _personRepository.GetById(id);
            return View("Detail", person);
        }

        [HttpGet("Edit/{id}")]
        public IActionResult Edit(int id)
        {
            var person = _personRepository.GetById(id);
            if (person == null)
            {
                return NotFound();
            }
            return View("Edit", person);
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Person person)
        {
            if (id != person.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                _personRepository.Update(person);
                return RedirectToAction(nameof(Index));
            }

            return View("Edit", person);
        }

        [HttpGet("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            var person = _personRepository.GetById(id);
            if (person == null)
            {
                return NotFound();
            }
            _personRepository.Delete(id);
            return RedirectToAction(nameof(Index), new { message = $"Person {id} was removed from the list successfully!" });
        }
    }
}
