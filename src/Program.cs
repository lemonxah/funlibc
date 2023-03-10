using funlib;
namespace HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            var none = None._;
            var some = 1.some();
            var next = some.map(x => x + 1).bind(x => x.some());
            Console.WriteLine($"option maps! {next}");

            var list = 1.cons(2.cons(3.cons(4.cons(5.cons(Nil._)))));
            var list2 = 6.cons(7.cons(8.cons(9.cons(10.cons(Nil._)))));
            var list3 = list.append(list2);
            Console.WriteLine($"list cons! {list3}");
            var list4 = list3.map(x => x + 1);
            Console.WriteLine($"list maps! {list4}");
        }
    }
}
