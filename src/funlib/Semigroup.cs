namespace funlib {
    public interface Semigroup<A> {
        A Append(A a, A b);
    }
}
