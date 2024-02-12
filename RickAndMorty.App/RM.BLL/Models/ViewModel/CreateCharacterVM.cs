using System.ComponentModel.DataAnnotations;

namespace RM.BLL.Models.ViewModel
{
    public class CreateCharacterVM
    {
        public const int UrlMaxLength = 256;

        public const string UrlRegexPattern = "^(http:\\/\\/www\\.|https:\\/\\/www\\.|http:\\/\\/|https:\\/\\/)?[a-z0-9]+([\\-\\.]{1}[a-z0-9]+)*\\.[a-z]{2,5}(:[0-9]{1,5})?(\\/.*)?$";

        [Required]
        public string Name { get; set; }

        public string Status { get; set; }

        public string Species { get; set; }

        public string Type { get; set; }

        public string Gender { get; set; }

        public string OriginName { get; set; }

        public string LocationName { get; set; }

        [MaxLength(UrlMaxLength)]
        [RegularExpression(UrlRegexPattern)]
        public string Image { get; set; }

        public List<string> Episode { get; set; }

        [MaxLength(UrlMaxLength)]
        [RegularExpression(UrlRegexPattern)]
        public string Url { get; set; }
    }
}