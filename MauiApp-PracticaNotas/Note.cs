namespace MauiApp_PracticaNotas.Models
{
    public class Note
    {
        public string Id { get; set; }
        public string Activity { get; set; }
        public bool IsUrgent { get; set; }
        public DateTime CreatedDate { get; set; }

        public bool IsDeleted { get; set; }

        public Note()
        {
            Id = Guid.NewGuid().ToString();
            CreatedDate = DateTime.Now;
            IsDeleted = false;
        }
    }
}
