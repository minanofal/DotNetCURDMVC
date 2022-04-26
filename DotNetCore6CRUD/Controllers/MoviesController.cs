using DotNetCore6CRUD.Data;
using DotNetCore6CRUD.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace DotNetCore6CRUD.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IToastNotification _toastNotification;
        private List<string> _AllowedExtentions = new List<string> { ".png", ".jpg" };
        private long _maxSize = 5048576;


        public MoviesController(ApplicationDbContext context , IToastNotification toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }
        public async Task<IActionResult> Index()
        {
            var Movies = await _context.Movies.OrderByDescending(m=>m.Rate).ToListAsync();
            return View(Movies);
        }

        public async Task<IActionResult> Create()
        {
            var viewModel = new MovieFormViewModel()
            {
                Genres = await _context.Genres.OrderBy(m=>m.Name).ToListAsync()
            };
            
            return View("MovieForm",viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovieFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                return View("MovieForm",model);
            }

            var files = Request.Form.Files;
            if (!files.Any())
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("Poster", "please Select A Poster !");
                return View("MovieForm", model);
            }
            var poster = files.FirstOrDefault();
            if (!_AllowedExtentions.Contains(Path.GetExtension(poster.FileName).ToLower()))
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("Poster", "only .png , .jpg images are allowed");
                return View("MovieForm", model);
            }

            if (poster.Length > _maxSize)
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("Poster", "poster cannot be can more thean 1 MB");
                return View("MovieForm", model);
            }


            using var dataStream = new MemoryStream();
            await poster.CopyToAsync(dataStream);


            var movie = new Movie()
            {
                Title = model.Title,
                GenreId = model.GenreId,
                Year = model.Year,
                Rate = model.Rate,
                Storeline = model.Storeline,
                Poster = dataStream.ToArray()
            };
            _context.Movies.Add(movie);
            _context.SaveChanges();
            _toastNotification.AddSuccessToastMessage("Movie Created successfully");
            return RedirectToAction(nameof(Index));
        }
      
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
                return BadRequest();

            var model = new MovieFormViewModel()
            {
                Id=movie.Id,
                Title = movie.Title,
                Year = movie.Year,
                GenreId = movie.GenreId,
                Storeline = movie.Storeline,
                Poster = movie.Poster,
                Rate = movie.Rate,
                Genres = await _context.Genres.OrderBy(m=>m.Name).ToListAsync()

            };
            return View("MovieForm" , model);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MovieFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                return View("MovieForm", model);
            }
            var movie = await _context.Movies.FindAsync(model.Id);
            if (movie == null)
                return BadRequest();

            var files = Request.Form.Files;
            var poster = files.FirstOrDefault();
            if (files.Any())
            {
                using var dataStream = new MemoryStream();
                await poster.CopyToAsync(dataStream);
                model.Poster = dataStream.ToArray();

                if (!_AllowedExtentions.Contains(Path.GetExtension(poster.FileName).ToLower()))
                {
                    model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                    ModelState.AddModelError("Poster", "only .png , .jpg images are allowed");
                    return View("MovieForm", model);
                }

                if (poster.Length > _maxSize)
                {
                    model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                    ModelState.AddModelError("Poster", "poster cannot be can more thean 1 MB");
                    return View("MovieForm", model);
                }
               
                movie.Poster = model.Poster;
            }
            movie.Title = model.Title;
            movie.Year = model.Year;
            movie.GenreId = model.GenreId;
            movie.Storeline = model.Storeline;
            movie.Rate = model.Rate;
            _context.SaveChanges();
            _toastNotification.AddSuccessToastMessage("Movie Updated Successfully");
            
            return RedirectToAction(nameof(Index));

            
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return BadRequest();
            var movie = await _context.Movies.Include(m => m.Genre).SingleOrDefaultAsync(m=> m.Id==id);
            if (movie == null)
                return NotFound();

            return View(movie);  
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return BadRequest();
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
                return NotFound();
            _context.Movies.Remove(movie);
            _context.SaveChanges(true);
            return Ok();
        }




    }
}
