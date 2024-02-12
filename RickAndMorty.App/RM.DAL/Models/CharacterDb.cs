namespace RM.DAL.Models
{
    public class CharacterDb
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Status { get; set; }

        public string Species { get; set; }

        public string Type { get; set; }

        public string Gender { get; set; }

        public string OriginName { get; set; }

        public string LocationName { get; set; }

        public string Image { get; set; }

        public string Episode { get; set; }

        public string Url { get; set; }

        public DateTime Created { get; set; }
    }
}