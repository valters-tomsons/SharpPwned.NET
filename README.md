# SharpPwned.NET

This library wraps HaveIBeenPwned.com v3 RESTful API in .NET Standard.

# Remarks
HaveIBeenPwned.com API version 2 was [superseded](https://haveibeenpwned.com/API/v2) by version [3](https://haveibeenpwned.com/API/v3).<p>
Breaking changes were introduced, which made version 2 unusable.

Main change was introduction of API key because of
continuous abuse ([Troy's blog post](https://www.troyhunt.com/authentication-and-the-have-i-been-pwned-api/)) of the HaveIBeenPwned service.

Paid API key can be obtained [here](https://haveibeenpwned.com/API/Key). 

Usage:

```c#
var client = new HaveIBeenPwnedRestClient(yourApiKey);
var response = client.IsPasswordPwned("hunter2").Result;
Console.WriteLine(response);
```
This will return a bool value, depending on if the password is indeed pwned.

```c#
using SharpPwned.NET.Model;

var client = new HaveIBeenPwnedRestClient(yourApiKey);
var response = client.GetAccountBreaches("gaben@valvesoftware.com").Result;
foreach(Breach x in response)
	{
		Console.WriteLine(x.Domain);
	}
```
GetAccountBreaches will return a list Breach objects, each Breach represents a single breached site and holds values as Name, Domain, Account Count etc. For full list of values, visit [API Documentation](https://haveibeenpwned.com/api/v3).

This project targets [.NET Standard 1.4](https://docs.microsoft.com/en-us/dotnet/standard/library)

# Nuget Package:
  PM> Install-Package SharpPwned.NET

