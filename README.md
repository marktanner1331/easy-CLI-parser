# easy-CLI-parser
Transform any project into a CLI program

Using annotations that are read via reflection at runtime, this library makes it easy to create CLI's

for example:
```
[CLIMethod]
[CLIShortName("+")]
[CLIExample("add 1 2")]
[CLIExample("+ 1.5 -4.8")]
[CLIDescription("Adds two numbers together")]
static double add(double value1, double value2)
{
    return value1 + value2;
}
```
