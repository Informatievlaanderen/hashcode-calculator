# Be.Vlaanderen.Basisregisters.HashCodeCalculator

Calculates hashcode based on given fields.

## Usage

```csharp
public class Address
{
    public string Address1 { get; set; }
    public string City { get; set; }
    public string State { get; set; }

    public IEnumerable<object> HashCodeFields()
    {
        yield return Address1;
        yield return City;
        yield return State;
    }

    public override int GetHashCode()
    {
        return HashCodeCalculator.GetHashCode(HashCodeFields());
    }
}
```
