namespace funlib {
    public interface Applicative<M> {
        HKT<A, M> Pure<A>(A a);
    }
    public static class Applicative {
        public static HKT<A, M> Pure<A, M>(this Applicative<M> applicative, A a) => applicative.Pure(a);
    }
}