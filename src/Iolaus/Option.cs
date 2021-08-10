using System;

namespace Iolaus
{
    public static partial class F
    {
        public static Option.None None
            => Option.None.Default;

        public static Option.Some<T> Some<T>(T value)
            => new Option.Some<T>(value);
    }

    namespace Option
    {
        public struct None
        {
            internal static readonly None Default = new None();
        }

        public struct Some<T>
        {
            internal T Value {get;}

            internal Some(T value)
            {
                Value = value ?? throw new ArgumentNullException();
            }
        }
    }

    public struct Option<T>
    {
        private readonly T _value;

        private Option(T value)
        {
            _value = value;
        }

        public static implicit operator Option<T>(Option.None _)
            => new Option<T>();
        
        public static implicit operator Option<T>(Option.Some<T> some)
            => new Option<T>(some.Value);

        public static implicit operator Option<T>(T value)
            => value is null ? F.None : F.Some(value);

        public R Match<R>(Func<R> None, Func<T, R> Some)
            => _value is null ? None() : Some(_value);

        public T Unsafe()
            => _value is null ? throw new Exception() : _value;
    }
}