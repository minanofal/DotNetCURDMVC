namespace DotNetCore6CRUD.Models
{
    public class Genre
    {
        public byte Id { get; set; }

        public string Name { get; set; }

        public virtual List<Movie>? movies { get; set; }

    }
}
