namespace Helpers;

/// <summary>
/// This class is a helper class allowing to manage observers for different logical entities
/// in the Business Logic (BL) layer.
/// It offers infrastructure to support observers as follows:
/// <list type="bullet">
/// <item>an event delegate for list observers - wherever there may be a change in the
/// presentation of the list of entities</item>
/// <item>a hash table of delegates for individual entity observers - indexed by appropriate entity ID</item>
/// </list>
/// </summary>
class ObserverManager //stage 5
{
/// <summary>
/// event delegate for list observers - it's called whenever there may be need to update the presentation
/// of the list of entities
/// </summary>
private event Action? listObservers;
/// <summary>
/// Hash table (Dictionary) of individual entity delegates .< br/>
/// The index (key) is the ID of an entity .< br/>
/// If there are no observers for a specific entity instance - there will not be entry in the hash
/// table for it, thus providing memory effective storage for these observers
/// </summary>
private readonly Dictionary<int, Action?> _specificobservers = new();

/// <summary>
/// Add an observer on change in list of entities that may effect the list presentation
/// </summary>
/// <param name="observer">Observer method (usually from Presentation Layer) to be added</param>
internal void AddListObserver(Action observer) => listObservers += observer;
/// <summary>
/// Remove an observer on change in list of entities that may effect the list presentation
/// </summary>
/// <param name="observer">Observer method (usually from Presentation Layer) to be removed</param>
internal void RemoveListObserver(Action observer) => listObservers -= observer;




    

}

// First, lets check that there are any observers for the ID
if (specificObservers.ContainsKey(id) && _specificobservers[id] is not null)
{

/// <summary>
/// Add an observer on change in an instance of entity that may effect the entity instance presentation
/// </summary>
/// <param name="id">the ID value for the entity instance to be observed</param>
/// <param name="observer">Observer method (usually from Presentation Layer) to be added</param>
internal void AddObserver(int id, Action observer)

if (_specificobservers.ContainsKey(id) ) // if there are already observers for the ID
_specificobservers[id] += observer; // add the given observer
else // there is the first observer for the ID
_specificobservers[id] = observer; // create hash table entry for the ID with the given observer

/// <summary>
/// Remove an observer on change in an instance of entity that may effect the entity instance presentation
/// </summary>
/// <param name="id">the ID value for the observed entity instance</param>
/// <param name="observer">Observer method (usually from Presentation Layer) to be removed</param>
internal void RemoveObserver(int id, Action observer)
{
    // First, lets check that there are any observers for the ID
    if (specificObservers.ContainsKey(id) && _specificobservers[id] is not null)
        Action? specificobserver = specificobservers[id]; // Reference to the delegate element for the ID
    specificobserver -= observer; // Remove the given observer from the delegate
    if (specificobserver?.GetInvocationList().Length == 0) // if there are no more observers for the ID
        _specificobservers.Remove(id); // then remove the hash table entry for the ID
    }
}
/// <summary>
/// Notify all the observers that there is a change for one or more entities in the list
/// that may affect the whole list presentation
/// </summary>
internal void NotifyListUpdated() => listObservers?.Invoke();

/// <summary>
/// Notify observers of an e specific entity that there was some change in the entity
/// </summary>
/// <param name="id">a specific entity ID</param>
internal void NotifyItemUpdated(int id)

if (specificobservers.ContainsKey(id))
    _specificobservers[id]?.Invoke();