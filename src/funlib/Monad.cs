namespace funlib {
    public interface Monad<M>: Functor<M>  {
        HKT<B, M> Bind<A, B>(Func<A, HKT<B, M>> f, HKT<A, M> ma);
        HKT<B, M> Functor<M>.Map<A, B>(Func<A, B> f, HKT<A, M> ma) => Bind(a => Pure(f(a)), ma);
    }
    public interface Monad2<M>: Functor2<M>  {
        HKT2<B, C, M> Bind<A, B, C>(Func<A, HKT2<B, C, M>> f, HKT2<A, C, M> ma);
        HKT2<B, C, M> Functor2<M>.Map<A, B, C>(Func<A, B> f, HKT2<A, C, M> ma) => Bind(a => Pure<B, C>(f(a)), ma);
    }

    #region Option
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

    #endregion Option

    #region List

    public readonly struct Nil {
        public static Nil _ => new Nil();
        public static Nil nil => new Nil();
    }

    public class List<A>: HKT<A, List.M> {
        public readonly bool isNil;
        public readonly A? head;
        public readonly List<A>? tail;
        public List() => isNil = true;
        public List(A head, List<A> tail) {
            this.head = head;
            this.tail = tail;
            isNil = false;
        }
        public List<A> append(List<A> other) => isNil ? other : head!.cons(tail!.append(other));
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
            public HKT<B, M> Bind<A, B>(Func<A, HKT<B, M>> f, HKT<A, M> ma) =>
                ma.narrowKind().isNil ? nil<B>() : f(ma.narrowKind().head!).narrowKind().append(ma.narrowKind().tail!.bind(a => f(a).narrowKind()));
            public HKT<A, M> Pure<A>(A a) => a.cons();
        }
    }

    #endregion List

    #region Either

    // class Either
    public struct Either<A, E> : HKT2<A, E, Either.M> {
        public readonly bool isLeft;
        public readonly A? __unsafeLeft;
        public readonly E? __unsafeRight;
        public Either(A left) {
            isLeft = true;
            __unsafeLeft = left;
        }
        public Either(E right) {
            isLeft = false;
            __unsafeRight = right;
        }

        override public string ToString() => isLeft ? $"Left({__unsafeLeft})" : $"Right({__unsafeRight})";
    };

    public static class Either {
        public interface M {}
        public static Either<A, E> narrowKind<A, E>(this HKT2<A, E, M> hkt) => (Either<A, E>)hkt;
        public static Either<B, E> map<A, B, E>(this Either<A, E> either, Func<A, B> f) =>
            Either.monad.Map(f, either).narrowKind();
        public static Either<A, F> mapRight<A, B, E, F>(this Either<A, E> either, Func<E, F> f) =>
            either.isLeft ? Either.Left<A, F>(either.__unsafeLeft!) : f(either.__unsafeRight!).right<A, F>();
        public static Either<B, E> bind<A, B, E>(this Either<A, E> either, Func<A, Either<B, E>> f) =>
            Either.monad.Bind(a => f(a), either).narrowKind();

        public static readonly Monad2<M> monad = new Monad();

        class Monad: Monad2<M> {
            public HKT2<B, C, M> Bind<A, B, C>(Func<A, HKT2<B, C, M>> f, HKT2<A, C, M> ma) =>
                ma.narrowKind().isLeft ? f(ma.narrowKind().__unsafeLeft!).narrowKind() : ma.narrowKind().__unsafeRight!.right<B, C>();
            public HKT2<A, B, M> Pure<A, B>(A a) => a.left<A, B>();
        }

        public static Either<A, E> Left<A, E>(A value) => value.left<A, E>();
        public static Either<A, E> Right<A, E>(E value) => value.right<A, E>();
    }


    public static class Left {
        public static Either<A, E> a<A, E>(A value) => value.left<A, E>();
        public static Either<A, E> left<A, E>(this A left) => new Either<A, E>(left);
    }
    public static class Right {
        public static Either<A, E> e<A, E>(E right) => right.right<A, E>();
        public static Either<A, E> right<A, E>(this E right) => new Either<A, E>(right);
    }

    #endregion Either

}

