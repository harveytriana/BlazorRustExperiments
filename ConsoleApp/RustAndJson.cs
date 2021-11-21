using System.Text.Json;

namespace ConsoleApp
{
    class RustAndJson
    {
        const string RUSTLIB = @"..\..\..\..\rstlib\target\release\rstlib.dll";

        [DllImport(RUSTLIB)] static extern IntPtr get_user(int user_id);

        record Person(int person_id, int age, string first_name, string last_name, string full_name);
        record User(int user_id, string password, Person person);

        public static void Run()
        {
            Console.WriteLine("CALLING RUST FUNCTIONS");

            // COMPOSED OBJECTS
            // ---------------------------------------------------------
            Console.WriteLine("\nCOMPOSED OBJECTS");

            var jsPointer = get_user(79);
            var js = jsPointer.TextFromPointer() ?? string.Empty;
            var user = JsonSerializer.Deserialize<User>(js);

            Console.WriteLine("JSON data obtained from the library:\n{0}\n", js.PrettyJson());
            Console.WriteLine("Deserialized:");
            Console.WriteLine("User identifier : {0}", user?.user_id);
            Console.WriteLine("User first name : {0}", user?.person.first_name);
            Console.WriteLine("User last name  : {0}", user?.person.last_name);
        }
    }
}
