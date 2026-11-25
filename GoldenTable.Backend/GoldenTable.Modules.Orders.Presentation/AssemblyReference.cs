using System.Reflection;

namespace GoldenTable.Modules.Orders.Presentation;

public static class AssemblyReference
{
    public static Assembly Assembly => typeof(AssemblyReference).Assembly;
}
