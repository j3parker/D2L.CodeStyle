using System.Collections.Generic;
using System.Collections.Immutable;
using D2L.CodeStyle.Analyzers.Common;
using D2L.CodeStyle.Analyzers.Mutability.Goals;
using Microsoft.CodeAnalysis;

namespace D2L.CodeStyle.Analyzers.Mutability.Reducers {
	internal static class ExternalImmutableTypes {
        // These special types are safe for Type and ConcreteType: think of them
        // as [Objects.Immutable]
        private readonly static ImmutableArray<SpecialType> s_ImmutableSpecialTypes = ImmutableArray.Create(
            SpecialType.System_Enum,
            SpecialType.System_Boolean,
            SpecialType.System_Char,
            SpecialType.System_SByte,
            SpecialType.System_Byte,
            SpecialType.System_Int16,
            SpecialType.System_UInt16,
            SpecialType.System_Int32,
            SpecialType.System_UInt32,
            SpecialType.System_Int64,
            SpecialType.System_UInt64,
            SpecialType.System_Decimal,
            SpecialType.System_Single,
            SpecialType.System_Double,
            SpecialType.System_String,
            SpecialType.System_IntPtr,
            SpecialType.System_UIntPtr
        );

		// Be careful when adding to this list.
		// Inspect the implementation of this type. If it's in .NET or from
		// Microsoft then check https://referencesource.microsoft.com to
		// start, and their GitHub repos. Run the analysis in your head.
		// There may be cases where analysis wouldn't pass but we may still
		// add it to this list. Consider the context of the class and the
		// intent of the authors.
		private static readonly ImmutableHashSet<string> s_ExternalImmutableConcreteTypes = ImmutableHashSet.Create(
            "System.Random"
        );

        // Be VERY careful when adding to this list.
        // Adding to this list is strictly more powerful than the one above
        // for ConcreteType. It additionally implies that every derived type
        // is also immutable.
        // Before adding to this list
        //   1. Validate that this concrete type is immutable (same as for
        //      the above list)
        //   2. That all of it's (external) subtypes are immutable
        //      (recursively follow the implementations, as they too may be
        //      base classes.)
        //   3. Consider the bigger picture: does it make sense that all
        //      implementations should be immutable? Which packages are
        //      likely/capable of adding new subtypes? (If the type is in
        //      .NET then any 3rd party package could add a new subtype.)
        //      The danger comes from subtypes in DLLs not scanned by our
        //      analyzers
        // Types in this list will effectively behave the same as if they
        // had [Objects.Immutable] applied to them.
        private static readonly ImmutableHashSet<string> s_ExternalImmutableTypes = ImmutableHashSet.Create(
            "System.Text.Encoding"
        );

		public static IEnumerable<Goal> Apply(
			SemanticModel model,
			ConcreteTypeGoal goal
		) {
            if( s_ImmutableSpecialTypes.Contains( goal.Type.SpecialType ) ) {
                yield break;
            }

            if( goal.Type.SpecialType == SpecialType.System_Object ) {
                // The *concrete( type object is always safe: it has no
                // properties or fields and doesn't have a base class. This
                // comes up in these scenarios:
                //   1. private readonly m_lock = new object();
                //   2. Every class is a subtype of object
                yield break;
            }

            var typeName = goal.Type.GetFullTypeName();

            // external things safe as Type are also safe as ConcreteType
			if ( s_ExternalImmutableTypes.Contains( typeName ) ) {
				yield break;
			}

			if( s_ExternalImmutableConcreteTypes.Contains( typeName ) ) {
                yield break;
            } 

			yield return goal;
		}

		public static IEnumerable<Goal> Apply(
			SemanticModel model,
			TypeGoal goal
		) {
			var typeName = goal.Type.GetFullTypeName();

			if ( s_ImmutableSpecialTypes.Contains( goal.Type.SpecialType ) ) {
				yield break;
			}

			if ( s_ExternalImmutableTypes.Contains( typeName ) ) {
				yield break;
			}

			yield return goal;
		}
	}
}
