namespace funlib {
    public interface Monad<M>: Functor<M>  {
        HKT<B, M> Bind<A, B>(Func<A, HKT<B, M>> f, HKT<A, M> ma);
        HKT<B, M> Functor<M>.Map<A, B>(Func<A, B> f, HKT<A, M> ma) => Bind(a => Pure(f(a)), ma);
    }

    public static class Option {
        public struct M {}
        public static Option<A> narrowKind<A>(this HKT<A, M> hkt) => (Option<A>)hkt;
        private static Option<A> none<A>() => new ();
        public static Option<B> map<A, B>(this Option<A> option, Func<A, B> f) => Option.monad.Map(f, option).narrowKind();
        public static Option<B> bind<A, B>(this Option<A> option, Func<A, Option<B>> f) => Option.monad.Bind(a => f(a), option).narrowKind();
        public static readonly Monad<M> monad = new Monad();
        class Monad: Monad<M> {
            public HKT<B, M> Bind<A, B>(Func<A, HKT<B, M>> f, HKT<A, M> ma) => ma.narrowKind().isSome ? f(ma.narrowKind().Value) : none<B>();
            public HKT<A, M> Pure<A>(A a) => a.some();
        }
    }

    public readonly struct Option<A>: HKT<A, Option.M> {
        public readonly bool isSome;
        public readonly A __unsafeValue;

        public Option(A value) {
            isSome = true;
            __unsafeValue = value;
        }        

        public A Value => isSome ? __unsafeValue : throw new Exception("Option is None");
        public A ValueOr(A defaultValue) => isSome ? __unsafeValue : defaultValue;
        public A ValueOr(Func<A> defaultValue) => isSome ? __unsafeValue : defaultValue();
        public static implicit operator Option<A>(None _) => new ();
        public override string ToString() => isSome ? $"Some({__unsafeValue})" : "None";
    };

    public readonly struct None {
        public static None _ => new None();
        public static None none => new None();
    }
    public static class Some {
        public static Option<A> a<A>(A value) => value.some();
        public static Option<A> some<A>(this A value) => new Option<A>(value);
    }

    public readonly struct Nil {
        public static Nil _ => new Nil();
        public static Nil nil => new Nil();
    }

    public class List<A>: HKT<A, List.M> {
        public readonly bool isNil;
        public readonly A head;
        public readonly List<A> tail;
        public List() => isNil = true;
        public List(A head, List<A> tail) {
            this.head = head;
            this.tail = tail;
            isNil = false;
        }
        public List<A> append(List<A> other) => isNil ? other : head.cons(tail.append(other));
        public override string ToString() => isNil ? "[]" : $"[{head}{tail}]";
        public static implicit operator List<A>(Nil _) => new ();
    }

    public static class List {
        public static List<A> cons<A>(this A head, List<A> tail) => new (head, tail);
        public static List<A> cons<A>(this A head) => new (head, new ());
        public struct M {}
        public static List<A> narrowKind<A>(this HKT<A, M> hkt) => (List<A>)hkt;
        private static List<A> nil<A>() => new ();
        public static List<B> map<A, B>(this List<A> list, Func<A, B> f) => List.monad.Map(f, list).narrowKind();
        public static List<B> bind<A, B>(this List<A> list, Func<A, List<B>> f) => List.monad.Bind(a => f(a), list).narrowKind();
        public static readonly Monad<M> monad = new Monad();
        class Monad: Monad<M> {
            public HKT<B, M> Bind<A, B>(Func<A, HKT<B, M>> f, HKT<A, M> ma) => ma.narrowKind().isNil ? nil<B>() : f(ma.narrowKind().head).narrowKind().append(ma.narrowKind().tail.bind(a => f(a).narrowKind()));
            public HKT<A, M> Pure<A>(A a) => a.cons();
        }
    }
}

