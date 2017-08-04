using System;
using SharpPwned.NET;
using Newtonsoft.Json;
using SharpPwned.NET.Model;


namespace SharpPwned.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new HaveIBeenPwnedRestClient();
            //var response = client.IsPasswordPwned("qwerty123").Result;
            //var response = client.GetAccountBreaches("gaben@valvesoftware.com").Result;
            //var response = client.GetAllBreaches().Result;
            //var response = client.GetBreach("adobe").Result;
            //var response = client.GetPasteAccount("gaben@valvesoftware.com").Result;


            Console.ReadLine();
        }
    }
}