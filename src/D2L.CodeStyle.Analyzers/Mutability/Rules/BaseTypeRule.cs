using System;
using System.Collections.Generic;
using D2L.CodeStyle.Analyzers.Mutability.Goals;
using D2L.CodeStyle.Analyzers.Mutability.Reducers.Utility;
using D2L.CodeStyle.Analyzers.Mutability.Tactics.Utility;
using Microsoft.CodeAnalysis;

namespace D2L.CodeStyle.Analyzers.Mutability.Rules {
	internal static class BaseTypeRule {
		public static SequencedReducer<TypeGoal> CombinedRule = new SequencedReducer<TypeGoal>(
			// similar (some are different overloads though) to ConcreteType
			//KnownExternalTypesTactic.Apply,
			//ContainerTypeTactic.Apply,
			AnnotatedImmutableTactic.Apply,
			//GuardAgainstExternalTypesTactic.Apply,

			DefaultApply
		);

		public static IEnumerable<Goal> DefaultApply(
			SemanticModel model,
			TypeGoal goal
		) {
			switch( goal.Type.TypeKind ) {
				case TypeKind.Array:
					throw new Exception( "mutable" );

				case TypeKind.Class:
				case TypeKind.Struct: // equivalent to TypeKind.Structure
					yield return new ConcreteTypeGoal( goal.Type );
					yield return new SubTypesGoal( goal.Type );
					yield break;

				case TypeKind.Interface:
					yield return new SubTypesGoal( goal.Type );
					yield break;

				case TypeKind.Dynamic:
					throw new Exception( "mutable" );

				case TypeKind.Enum:
					// Enums have no subtypes and are value types
					yield break;

				case TypeKind.Error:
					yield break;

				case TypeKind.TypeParameter:
					yield return new GenericTypeParameterGoal( goal.Type );
					yield break;

				default:
					// not handled: Unknown, Module, Pointer, Submission, Delegate.
					throw new NotImplementedException(
						$"TypeKind.{goal.Type.Kind} not handled by BaseType analysis"
					);
			}
		}
	}
}
