using LinqTasks.Models;
namespace LinqTasks.Extensions;

public static class CustomExtensionMethods
{
    //Put your extension methods here
    public static IEnumerable<Emp> GetEmpsWithSubordinates(this IEnumerable<Emp> emps)
    {
        //pracowników, którzy mają min. 1 bezpośredniego podwładnego.
        ///     Pracownicy powinny w ramach kolekcji być posortowani po nazwisku (rosnąco) i pensji (malejąco).
        
        return null;
    }
}