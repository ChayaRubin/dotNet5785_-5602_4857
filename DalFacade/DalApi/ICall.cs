

namespace DalApi;
using DO;
public interface ICall
{
    void Create(Call item); 
    Call? Read(int id);  
    List<Call> ReadAll(); 
    void Update(Call item); 
    void Delete(int id); 
    void DeleteAll(); 

}
