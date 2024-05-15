using Asp_MVC1.Models;

public class PersonRepository : IPersonRepository
{
    private static List<Person> _person = Person.GetDummyData();

    public IEnumerable<Person> GetAll()
    {
        return _person;
    }

    public Person GetById(int id)
    {
        return _person.FirstOrDefault(p => p.Id == id);
    }

    public void Create(Person person)
    {
        _person.Add(person);
    }

    public void Update(Person person)
    {
        var existingPerson = _person.FirstOrDefault(p => p.Id == person.Id);
        if (existingPerson != null)
        {
            existingPerson.FirstName = person.FirstName;
            existingPerson.LastName = person.LastName;
            existingPerson.Gender = person.Gender;
            existingPerson.DateOfBirth = person.DateOfBirth;
            existingPerson.PhoneNumber = person.PhoneNumber;
            existingPerson.BirthPlace = person.BirthPlace;
            existingPerson.IsGraduated = person.IsGraduated;
            existingPerson.Id = person.Id;
        }
    }

    public void Delete(int id)
    {
        var person = _person.FirstOrDefault(p => p.Id == id);
        if (person != null)
        {
            _person.Remove(person);
        }
    }
}