using Asp_MVC1.Models;

public interface IPersonRepository
{
    IEnumerable<Person> GetAll();
    Person GetById(int id);
    void Create(Person person);
    void Update(Person person);
    void Delete(int id);
}