namespace funlib {
    public interface Monoid<A> : Semigroup<A> {
        A Empty();
    }
}