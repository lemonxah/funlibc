using funlib;
namespace HelloWorld
{
    public enum Errors {
        ParsingError,
        NotEven,
    }

    public enum Errors2 {
        SomeError,
        SomeParsingError,
    }

    class Program
    {
        static Either<int, Errors> parse(string s) => int.TryParse(s, out var i) ? i.left<int, Errors>() : Errors.ParsingError.right<int, Errors>();

        static Either<int, Errors> add1(int x) => (x + 1).left<int, Errors>();

        static Either<int, Errors> mul2(int x) => (x * 2).left<int, Errors>();

        static Either<int, Errors> iseven(int x) => x % 2 == 0 ? x.left<int, Errors>() : Errors.NotEven.right<int, Errors>();

        static Either<int, Errors> div2(int x) => (x / 2).left<int, Errors>();

        static Either<int, Errors2> blah(int x) => (x * 2).left<int, Errors2>();

        static int add2(int x) => x + 2;
        static void Main(string[] args) {

        }
    }
}
