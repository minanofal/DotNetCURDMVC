namespace DotNetCore6CRUD.Models
{
    public class Movie
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public int Year { get; set; }

        public double Rate { get; set; }

        public string Storeline { get; set; }

        public byte[] Poster { get; set; }

        public byte GenreId { get; set; }

        public virtual Genre? Genre { get; set; }
    }
}
