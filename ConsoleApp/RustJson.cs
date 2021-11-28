using System.Text.Json;
using System.Text.Json.Serialization;

namespace ConsoleApp;
using static Global;

class RustJson
{
    [DllImport(RLIB)] static extern IntPtr get_user(int user_id);
    [DllImport(RLIB)] static extern void post_user([MarshalAs(UnmanagedType.LPWStr)] string userJson);

    class Person
    {
        [JsonPropertyName("person_id")]
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

        // sending json
        var alisson = new User {
            Id = 789456123,
            Password = "hashed password",
            Person = new Person {
                Id = 123,
                FirstName = "Alisson Johana",
                LastName = "Triana",
                Age = 18,
            }
        };

        var alissonJson = JsonSerializer.Serialize(alisson);

        Console.WriteLine("\nJSON data will send to the library:\n{0}\n", alissonJson.PrettyJson());

        Console.ForegroundColor = ConsoleColor.DarkYellow;
        try {
            post_user(alissonJson);
        }
        catch { }
        Console.ForegroundColor = ConsoleColor.Gray;


    }
}

