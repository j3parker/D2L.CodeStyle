using System;
using System.Collections.Generic;
using D2L.CodeStyle.Analyzers.Mutability.Goals;
using D2L.CodeStyle.Analyzers.Mutability.Reducers.Utility;
using Microsoft.CodeAnalysis;

namespace D2L.CodeStyle.Analyzers.Mutability.Reducers {
	internal static class ConcreteTypeTactics {
		public static SequencedReducer<ConcreteTypeGoal> CombinedTactic = new SequencedReducer<ConcreteTypeGoal>(
			// common with BaseType
			//KnownExternalTypesTactic.Apply,
			ContainerTypeReducer.Apply,
			//AnnotatedImmutableTactic.Apply,

			// specific to ConcreteType
			DefaultApply
		); 

		public static IEnumerable<Goal> DefaultApply(
			SemanticModel model,
			ConcreteTypeGoal obligation
		) {
			switch( obligation.Type.TypeKind ) {
				case TypeKind.Array:
					throw new Exception( "mutable" );

				case TypeKind.Class:
					yield return new ClassGoal( obligation.Type );
					yield break;

				case TypeKind.Dynamic:
					throw new Exception( "mutable" );

				case TypeKind.Enum:
					// Enums are value types
					yield break;

				case TypeKind.Error:
					// We can safetly ignore these types. There is already
					// something more fundamental failing the build.
					yield break;

				case TypeKind.Struct: // equivalent to TypeKind.Structure
					yield return new StructGoal( obligation.Type );
					yield break;

				default:
					// not handled: Interface, TypeParameter, Unknown, Module, Pointer, Submission, Delegate.
					throw new NotImplementedException(
						$"TypeKind.{obligation.Type.Kind} not handled by ConcreteType analysis"
					);
			}
		} 
	}
}
