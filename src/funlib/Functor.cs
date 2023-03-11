namespace funlib {
    public interface Functor<M>: Applicative<M> {
        HKT<B, M> Map<A, B>(Func<A, B> f, HKT<A, M> ma);
    }
   public interface Functor2<M>: Applicative2<M> {
        HKT2<B, C, M> Map<A, B, C>(Func<A, B> f, HKT2<A, C, M> ma);
    }
}