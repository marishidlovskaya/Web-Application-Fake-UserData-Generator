using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FakeUserDataGenerator.Models.UsersData
{
    public class Name
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string Region { get; set; }

    }
}
