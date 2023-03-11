namespace funlib {
    public interface Applicative<M> {
        HKT<A, M> Pure<A>(A a);
    }

    public interface Applicative2<M> {
        HKT2<A, B, M> Pure<A, B>(A a);
    }

}