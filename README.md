# JsonConverters
A collection of JsonConverters found on Stack Overflow that I found helpful / indispensible.  These work with Newtonsoft's Json.NET.

## ArrayToObjectConverter
Takes a Json array and converts it into an object, as long as the element order is consistent.  Given the following Json:

```
{
   "coordinates": [
      [
         -91.575245,
         38.166659
      ],
      [
         -91.488450,
         38.069393
      ]
   ]
}
```
You can decorate your class like this:
```
[JsonConverter(typeof(ArrayToObjectConverter<Coordinate>))]
public class Coordinate
{
	[JsonArrayIndex(0)]
	public double Longitude { get; set; }

	[JsonArrayIndex(1)]
	public double Latitude { get; set; }
}
```
and deserialize like this:
```
var CoordinateList = JsonConvert.DeserializeObject<List<Coordinate>>(json);
```
Currently, this converter only support deserialization.
## TolerantObjectConverter\<T\>
I came across a Json element that would contain an object formatted like a dictionary if there was data to return, but if not the element would be an empty array.  This converter will return a T if the element is a T, and an empty T if the data doesn't match.

Example Json:
```
{
   "duplicateProducts": [],
   "newRecords": {
      "318358964": "download-test",
      "318358965": "download-test",
      "318358967": "download-test"
   }
}
```
Example Class:
```
public class Example
{
   [JsonProperty(PropertyName = "duplicateProducts")]
   [JsonConverter(typeof(TolerantObjectConverter<Dictionary<string, string>>))]
   public Dictionary<string, string> DuplicateProducts { get; set; }

   [JsonProperty(PropertyName = "newRecords")]
   [JsonConverter(typeof(TolerantObjectConverter<Dictionary<string, string>>))]
   public Dictionary<string, string> NewRecords { get; set; }
}
```
Deserialization:
```
   var example = JsonConvert.DeserializeObject<Example>(json);
   // example.DuplicateProducts contains empty (new) Dictionary
   // example.NewRecords contains Dictionary with three items
```
This converter only supports deserialization.