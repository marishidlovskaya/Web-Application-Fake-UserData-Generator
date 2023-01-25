using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FakeUserDataGenerator.Models.UsersData
{
    public class Mobile
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public int ID { get; set; }
        public string? MobileNumber { get; set; }
        public string Region { get; set; }
    }
}
