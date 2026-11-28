using System.Reflection;

namespace GoldenTable.Modules.Orders.Application;

public static class AssemblyReference
{
    public static Assembly Assembly => typeof(AssemblyReference).Assembly;
}
