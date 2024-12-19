using DO;

namespace DalApi;

/// <summary>
/// Provides CRUD (Create, Read, Update, Delete) operations for a specified type.
/// </summary>
/// <typeparam name="T">The type of the entity for which CRUD operations are defined (Assignment,Call,Volunteer).</typeparam>
public interface ICrud<T> where T : class
{
    void Create(T item); 
    T? Read(int id);
    T? Read(Func<T, bool> filter);
    IEnumerable<T>? ReadAll(Func<T, bool>? filter = null);
    void Update(T item);
    void Delete(int id);
    void DeleteAll();

}

