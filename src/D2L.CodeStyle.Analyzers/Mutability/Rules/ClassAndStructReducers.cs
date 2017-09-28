using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using D2L.CodeStyle.Analyzers.Common;
using D2L.CodeStyle.Analyzers.Mutability.Goals;
using Microsoft.CodeAnalysis;

namespace D2L.CodeStyle.Analyzers.Mutability.Rules {
	internal static class ClassAndStructRules {
		public static IEnumerable<Goal> Apply(
			SemanticModel model,
			ClassGoal goal
		) {
			// This reducer doesn't work for types defined in other assemblies
			if( model.Compilation.Assembly != goal.Type.ContainingAssembly ) {
				yield return goal;
				yield break;
			}

			yield return new ConcreteTypeGoal(
				goal.Type.BaseType
			);

			foreach( var obl in ApplyToMembers( goal.Type ) ) {
				yield return obl;
			}
		}

		public static IEnumerable<Goal> Apply(
			SemanticModel model,
			StructGoal goal
		) {
			// This reducer doesn't work for types defined in other assemblies
			if( model.Compilation.Assembly != goal.Type.ContainingAssembly ) {
				return ImmutableArray.Create( goal );
			}

			// Structs have no base type
			return ApplyToMembers( goal.Type );
		}

		private static IEnumerable<Goal> ApplyToMembers(
			ITypeSymbol type
		) {
			var members = type.GetExplicitNonStaticMembers();
			foreach( ISymbol member in members ) {
				Goal obl;
				if( MemberToObligation( member, out obl ) ) {
					yield return obl;
				}
			}
		}

		private static bool MemberToObligation(
			ISymbol member,
			out Goal res
		) {
			switch( member.Kind ) {
				case SymbolKind.Field:
					res = new FieldGoal( member as IFieldSymbol );
					return true;

				case SymbolKind.Property:
					res = new PropertyGoal( member as IPropertySymbol );
					return true;

				case SymbolKind.Method:
					res = null;
					return false;

				default:
					throw new NotImplementedException();
			}
		}
	}
}
