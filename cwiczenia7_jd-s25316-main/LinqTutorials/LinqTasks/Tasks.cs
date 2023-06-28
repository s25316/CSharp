using LinqTasks.Extensions;
using LinqTasks.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;

namespace LinqTasks;

public static partial class Tasks
{
    public static IEnumerable<Emp> Emps { get; set; }
    public static IEnumerable<Dept> Depts { get; set; }

    static Tasks()
    {
        Depts = LoadDepts();
        Emps = LoadEmps();
    }

    /// <summary>
    ///     SELECT * FROM Emps WHERE Job = "Backend programmer";
    /// </summary>
    public static IEnumerable<Emp> Task1()
    {
        IEnumerable<Emp> emp = Emps.Where(a => a.Job == "Backend programmer");
        return emp;
    }

    /// <summary>
    ///     SELECT * FROM Emps Job = "Frontend programmer" AND Salary>1000 ORDER BY Ename DESC;
    /// </summary>
    public static IEnumerable<Emp> Task2()
    {
        IEnumerable<Emp> emp = Emps.Where(a => a.Job == "Frontend programmer" && a.Salary > 1000).OrderByDescending(a => a.Ename);
        return emp;
    }


    /// <summary>
    ///     SELECT MAX(Salary) FROM Emps;
    /// </summary>
    public static int Task3()
    {
        int sal = Emps.Max(a => a.Salary);
        return sal;
    }

    /// <summary>
    ///     SELECT * FROM Emps WHERE Salary=(SELECT MAX(Salary) FROM Emps);
    /// </summary>
    public static IEnumerable<Emp> Task4()
    {
        IEnumerable<Emp> emp = Emps.Where(a => a.Salary == (Emps.Max(b => b.Salary)));
        return emp;
    }

    /// <summary>
    ///    SELECT ename AS Nazwisko, job AS Praca FROM Emps;
    /// </summary>
    public static IEnumerable<object> Task5()
    {
        IEnumerable<object> objects = Emps.Select(a => new { Nazwisko = a.Ename, Praca = a.Job});
        return objects;
    }

    /// <summary>
    ///     SELECT Emps.Ename, Emps.Job, Depts.Dname FROM Emps
    ///     INNER JOIN Depts ON Emps.Deptno=Depts.Deptno
    ///     Rezultat: Złączenie kolekcji Emps i Depts.
    /// </summary>
    public static IEnumerable<object> Task6()
    {
        IEnumerable<object> objects = Emps.Join(Depts, e => e.Deptno, d => d.Deptno, (e,d) => new {e.Ename, e.Job, d.Dname });
        return objects;
    }

    /// <summary>
    ///     SELECT Job AS Praca, COUNT(1) LiczbaPracownikow FROM Emps GROUP BY Job;
    /// </summary>
    public static IEnumerable<object> Task7()
    {
        IEnumerable<object> objects = Emps.GroupBy(a => a.Job).Select(a => new { Praca = a.Key, LiczbaPracownikow = a.Count() });
        return objects;
    }

    /// <summary>
    ///     Zwróć wartość "true" jeśli choć jeden
    ///     z elementów kolekcji pracuje jako "Backend programmer".
    /// </summary>
    public static bool Task8()
    { 
        
        int counter = Emps.Count( a => a.Job == "Backend programmer");
        return counter > 0;
    }

    /// <summary>
    ///     SELECT TOP 1 * FROM Emp WHERE Job="Frontend programmer"
    ///     ORDER BY HireDate DESC;
    /// </summary>
    public static Emp Task9()
    {
        Emp emp = Emps.Where(a => a.Job == "Frontend programmer").OrderByDescending(a => a.HireDate).ElementAt(0);
        return emp;
    }

    /// <summary>
    ///     SELECT Ename, Job, Hiredate FROM Emps
    ///     UNION
    ///     SELECT "Brak wartości", null, null;
    /// </summary>
    public static IEnumerable<object> Task10()
    {
        IEnumerable<object> objects = Emps.Select(a => new { a.Ename, a.Job, a.HireDate}).
            Union(
            Emps.Select(a => new { Ename = "Brak wartości", Job = (string) null, HireDate = (DateTime?)null })
            );
        return objects;
    }

    /// <summary>
    ///     Wykorzystując LINQ pobierz pracowników podzielony na departamenty pamiętając, że:
    ///     1. Interesują nas tylko departamenty z liczbą pracowników powyżej 1
    ///     2. Chcemy zwrócić listę obiektów o następującej srukturze:
    ///     [
    ///     {name: "RESEARCH", numOfEmployees: 3},
    ///     {name: "SALES", numOfEmployees: 5},
    ///     ...
    ///     ]
    ///     3. Wykorzystaj typy anonimowe
    /// </summary>
    public static IEnumerable<object> Task11()
    {
        IEnumerable<object> objects = Emps.Join(Depts, e => e.Deptno, d => d.Deptno, (e, d) => new { d.Dname, e.Empno }).
            GroupBy( a => a.Dname).Select( a => new { name = a.Key, numOfEmployees = a.Count()} );
        return objects;
    }

    /// <summary>
    ///     Napisz własną metodę rozszerzeń, która pozwoli skompilować się poniższemu fragmentowi kodu.
    ///     Metodę dodaj do klasy CustomExtensionMethods, która zdefiniowana jest poniżej.
    ///     Metoda powinna zwrócić tylko tych pracowników, którzy mają min. 1 bezpośredniego podwładnego.
    ///     Pracownicy powinny w ramach kolekcji być posortowani po nazwisku (rosnąco) i pensji (malejąco).
    /// </summary>
    public static IEnumerable<Emp> Task12()
    {
        IEnumerable<Emp> result = Emps.GetEmpsWithSubordinates();
        
        return result;
    }

    /// <summary>
    ///     Poniższa metoda powinna zwracać pojedyczną liczbę int.
    ///     Na wejściu przyjmujemy listę liczb całkowitych.
    ///     Spróbuj z pomocą LINQ'a odnaleźć tę liczbę, które występuja w tablicy int'ów nieparzystą liczbę razy.
    ///     Zakładamy, że zawsze będzie jedna taka liczba.
    ///     Np: {1,1,1,1,1,1,10,1,1,1,1} => 10
    /// </summary>
    public static int Task13(int[] arr)
    {
        int num = arr.GroupBy( a => a ).Select( a => new { num = a.Key, count = a.Count() }).
            Where ( a => a.count % 2 ==1 ).ElementAt(0).num;
        return num;
    }

    /// <summary>
    ///     Zwróć tylko te departamenty, które mają 5 pracowników lub nie mają pracowników w ogóle.
    ///     Posortuj rezultat po nazwie departament rosnąco.
    /// </summary>
    public static IEnumerable<Dept> Task14()
    {
        IEnumerable<Dept> depts = Depts.Join(Emps, d => d.Deptno, e => e.Deptno, (d, e) => new { d, e }).
            GroupBy(a => a.d).Select(a => new { key = a.Key, count = a.Count() }).Where(a => a.count != 5 || a.count != 0).
            Select(a => a.key).OrderBy( a => a.Dname);
        return Depts.Except(depts);
    }
}