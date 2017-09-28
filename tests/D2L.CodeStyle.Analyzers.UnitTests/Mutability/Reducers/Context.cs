using System;

namespace D2L.CodeStyle.Analyzers.Mutability.Reducers {
	public class MutableClass {
		public int x;
	}

	public sealed class ImmutableClass { }

	public sealed class NonReadOnlyFields {
		public int m_intField; // -> ERROR
		public int[] m_intArray; // -> ERROR
		public MutableClass m_mutableClass; // -> ERROR
		public ImmutableClass m_immutableClass; // -> ERROR
		public object m_object; // -> ERROR

		public object m_objectWithInitializer = new object(); // -> ERROR
		public int m_intFieldWithInitializer = 3; // -> ERROR

		public static int m_staticInt; // -> ERROR
	}

	public sealed class ReadOnlyFieldsWithoutInitializer {
		public readonly int m_intField; // -> Type{int}
		public readonly int[] m_intArray; // -> Type{int[]}
		public readonly MutableClass m_mutableClass; // -> Type{MutableClass}
		public readonly ImmutableClass m_immutableClass; // -> Type{ImmutableClass}

		public readonly object m_object; // -> Type{object}

		public static readonly int m_staticInt; // -> Type{int}
	}

	public sealed class ReadOnlyFieldsWithInitializers {
		public readonly int m_intField = 3 + 1; // -> Initializer{3+1}
		public readonly int[] m_intArray = new[] { 0 }; // -> Initializer{new[] { 0 }}
		public readonly MutableClass m_mutableClass = new MutableClass(); // -> Initializer{new MutableClass()}
		public static readonly int m_staticInt = 4; // -> Initializer{4}
	}

	public sealed class MultipleVariables {
		public readonly int a, b; // -> Type{int}
		public readonly int c = 1, d; // -> Type{int}
		public readonly int e, f = 1; // -> Type{int}
		public readonly int g = 1, h = 2; // -> Initializer{1], Initializer{2}

		public readonly int w, x, y = 42, z; // -> Type{int} 
	}
}