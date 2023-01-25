using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FakeUserDataGenerator.Models.UsersData
{
    public class Street
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public int ID { get; set; }
        public string StreetName { get; set; }
        public string Region { get; set; }
    }
}
