using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;

namespace RazorPagesMovie.Pages.Movies
{
    public class IndexModel : PageModel
    {
        private readonly RazorPagesMovie.Data.RazorPagesMovieContext _context;

        public IndexModel(RazorPagesMovie.Data.RazorPagesMovieContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; }

        public SelectList Genres { get; set; }

        [BindProperty(SupportsGet = true)]
        public string MovieGenre { get; set; }

        public IList<Movie> Movie { get;set; }

        public async Task OnGetAsync()
        {
            IQueryable<string> genreQuery = from movie in _context.Movie
                                            select movie.Genre;

            var movieQuery = from movie in _context.Movie
                             select movie;

            if (!string.IsNullOrEmpty(SearchString))
            {
                movieQuery = movieQuery.Where(movie => movie.Title.Contains(SearchString));
            }    

            if (!string.IsNullOrEmpty(MovieGenre))
            {
                movieQuery = movieQuery.Where(movie => movie.Genre.Equals(MovieGenre));
            }

            Genres = new SelectList(await genreQuery.Distinct().ToListAsync());
            Movie = await movieQuery.ToListAsync(); ;
        }
    }
}
