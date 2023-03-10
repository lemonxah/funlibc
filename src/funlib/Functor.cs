namespace funlib {
    public interface Functor<M>: Applicative<M> {
        HKT<B, M> Map<A, B>(Func<A, B> f, HKT<A, M> ma);
    }
}