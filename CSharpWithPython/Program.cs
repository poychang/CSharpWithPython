using IronPython.Hosting;

ExecutePythonFromString();
ExecutePythonFromFile();
ScopeAndDynamic();
PassVariableToPython();

// 為了方便測試，這裡都用預測設定的 Python Runtime 來操作
// 也可以使用 Python.CreateEngine() 來詳細設定 Python Engine

void ExecutePythonFromString()
{
    Console.WriteLine($"\n{nameof(ExecutePythonFromString)}\n------------------------------");

    var engine = Python.CreateEngine();
    var scope = engine.CreateScope();
    scope.Engine.Execute(@"
def greetings(name):
    return 'Hello ' + name.title() + '!'
    ", scope);
    dynamic greetings = scope.GetVariable("greetings");
    Console.WriteLine(greetings("world"));
}

void ExecutePythonFromFile()
{
    Console.WriteLine($"\n{nameof(ExecutePythonFromFile)}\n------------------------------");

    var pyRuntime = Python.CreateRuntime();
    var fileScope = pyRuntime.UseFile(Path.Combine(Environment.CurrentDirectory, "Python/sample.py"));
    Console.WriteLine(fileScope.GetVariable("name"));

    dynamic add = fileScope.GetVariable("add");
    Console.WriteLine(add(1, 2));
}

void ScopeAndDynamic()
{
    Console.WriteLine($"\n{nameof(ScopeAndDynamic)}\n------------------------------");

    var pyRuntime = Python.CreateRuntime();

    Console.WriteLine("\nScope\n==========");
    var scope = pyRuntime.UseFile(Path.Combine(Environment.CurrentDirectory, "Python/sample.py"));
    Console.WriteLine(scope.GetVariable("name"));
    dynamic add = scope.GetVariable("add");
    Console.WriteLine(add(1, 2));

    Console.WriteLine("\nDynamic\n==========");
    dynamic dyn = pyRuntime.UseFile(Path.Combine(Environment.CurrentDirectory, "Python/sample.py"));
    Console.WriteLine(dyn.name);
    Console.WriteLine(dyn.add(1, 2));
}

void PassVariableToPython()
{
    Console.WriteLine($"\n{nameof(PassVariableToPython)}\n------------------------------");
    
    var engine = Python.CreateEngine();
    var scope = engine.CreateScope();
    var demo = new Demo { Id = 1, Name = "Poy Chang" };
    scope.SetVariable("demo", demo);
    var result = scope.Engine.Execute(@"
print('Hello %s' %demo.Name)
demo.DoSomething()
    ", scope);
    Console.WriteLine(result);
    
    /*
    var engine = Python.CreateEngine();
    var scope = engine.CreateScope();
    var demo = new Demo { Id = 1, Name = "Poy Chang" };
    scope.SetVariable("demo", demo);
    var script = @"
print('Hello %s' %demo.Name)
demo.DoSomething()
        ";
    var sourceCode = engine.CreateScriptSourceFromString(script);
    var result = sourceCode.Execute<object>(scope);
    Console.WriteLine(result);
    */
}

public class Demo
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string DoSomething() => $"{Id}: {Name}";
}