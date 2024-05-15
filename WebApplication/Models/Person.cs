using Asp_MVC1.Models;

namespace Asp_MVC1.Models
{
    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public string BirthPlace { get; set; }
        public bool IsGraduated { get; set; }
        public string FullName => LastName + ' ' + FirstName;
        public int Id { get; set; }
        public static List<Person> GetDummyData()
        {
            return new List<Person>
        {
            new Person { FirstName = "Van A", LastName = "Nguyen", Gender = "Male", DateOfBirth = new DateTime(2000, 3, 15), PhoneNumber = "1234567890", BirthPlace = "Ha Noi", IsGraduated = true, Id = 1 },
            new Person { FirstName = "Van B", LastName = "Nguyen", Gender = "Male", DateOfBirth = new DateTime(1999, 6, 20), PhoneNumber = "9876543210", BirthPlace = "Sai Gon", IsGraduated = false, Id = 2 },
            new Person { FirstName = "Thi C", LastName = "Nguyen", Gender = "Female", DateOfBirth = new DateTime(2001, 6, 20), PhoneNumber = "9876543210", BirthPlace = "Sai Gon", IsGraduated = false, Id = 3 },
        };
        }
    }
    
}
