# SharpPwned.NET

This library wraps HaveIBeenPwned.com v2 RESTful API in .NET Standard.

Usage:

```c#
var client = new HaveIBeenPwnedRestClient();
var response = client.IsPasswordPwned("hunter2").Result;
Console.WriteLine(response);
```
This will return a bool value, depending on if the password is indeed pwned.

```c#
using SharpPwned.NET.Model;

var client = new HaveIBeenPwnedRestClient();
var response = client.GetAccountBreaches("gaben@valvesoftware.com").Result;
foreach(Breach x in response)
	{
		Console.WriteLine(x.Domain);
	}
```
GetAccountBreaches will return a list Breach objects, each Breach represents a single breached site and holds values as Name, Domain, Account Count etc. For full list of values, visit [API Documentation](https://haveibeenpwned.com/api/v2).

This project targets [.NET Standard 1.4](https://docs.microsoft.com/en-us/dotnet/standard/library)

# Nuget Package:
  PM> Install-Package Install-Package SharpPwned.NET

