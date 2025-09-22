using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.VisualBasic;

class Prog
{
    static void Main(string[] args)
    {
        using (ApplicationContext db = new ApplicationContext())
        {
            var users = db.Users.ToList();
            Console.WriteLine("Данные после добавления:");
            foreach (User u in users)
            {
                Console.WriteLine($"{u.ID}.{u.Name} - {u.Number_Phone}");
            }
        }
    }
}