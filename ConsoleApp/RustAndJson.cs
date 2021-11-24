using System.Text.Json;
using System.Text.Json.Serialization;

namespace ConsoleApp;

class RustAndJson
{
    const string RUSTLIB = @"..\..\..\..\rstlib\target\release\rstlib.dll";

    [DllImport(RUSTLIB)] static extern IntPtr get_user(int user_id);

    class Person
    {
        [JsonPropertyName("Wind")] 
        public int Id { get; set; }
        [JsonPropertyName("first_name")]
        public string? FirstName { get; set; }
        [JsonPropertyName("last_name")]
        public string? LastName { get; set; }
        [JsonPropertyName("age")]
        public int Age { get; set; }
    }
    class User
    {
        [JsonPropertyName("user_id")]
        public int Id { get; set; }
        [JsonPropertyName("person")]
        public Person? Person { get; set; }
        [JsonPropertyName("password")]
        public string? Password { get; set; }
    }
    // OR
    //record Person(
    //    int person_id,
    //    int age,
    //    string first_name,
    //    string last_name);
    //record User(
    //    int user_id,
    //    string password,
    //    Person person);

    public static void Run()
    {
        Console.WriteLine("\nCOMPOSED OBJECTS\n");

        var jsPointer = get_user(79);
        var js = jsPointer.TextFromPointer() ?? string.Empty;
        var user = JsonSerializer.Deserialize<User>(js);

        Console.WriteLine("JSON data obtained from the library:\n{0}\n", js.PrettyJson());
        Console.WriteLine("Deserialized:");
        Console.WriteLine("User identifier : {0}", user?.Id);
        Console.WriteLine("First name      : {0}", user?.Person?.FirstName);
        Console.WriteLine("Last name       : {0}", user?.Person?.LastName);
        Console.WriteLine("Age             : {0}", user?.Person?.Age);
    }
}

