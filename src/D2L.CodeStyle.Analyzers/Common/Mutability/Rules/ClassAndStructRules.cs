﻿using System;
using System.Collections.Generic;
using D2L.CodeStyle.Analyzers.Common;
using D2L.CodeStyle.Analyzers.Common.Mutability.Goals;
using Microsoft.CodeAnalysis;

namespace D2L.CodeStyle.Analyzers.Commmon.Mutability.Rules {
	internal static class ClassAndStructRules {
		public static IEnumerable<Goal> Apply(
			SemanticModel model,
			ClassGoal goal
		) {
			yield return new ConcreteTypeGoal(
				goal.Type.BaseType
			);

			foreach( var obl in ApplyToMembers( model, goal.Type ) ) {
				yield return obl;
			}
		}

		public static IEnumerable<Goal> Apply(
			SemanticModel model,
			StructGoal goal
		) {
			// Structs have no base type

			foreach( var obl in ApplyToMembers( model, goal.Type ) ) {
				yield return obl;
			}
		}

		private static IEnumerable<Goal> ApplyToMembers(
			SemanticModel model,
			ITypeSymbol type
		) {
			// ClassGoal and StructGoal will only be generated by TypeGoal and
			// ConcreteTypeGoal rules, and those rules won't generate this goal
			// for types in other assemblies. The foreach below wouldn't
			// function correctly for those types (e.g. private members would
			// be omitted.)
			if( model.Compilation.Assembly != type.ContainingAssembly ) {
				throw new InvalidOperationException( "pre-condition not met in ClassOrStructRule" );
			}

			var members = type.GetExplicitNonStaticMembers();

			foreach( ISymbol member in members ) {
				Goal obl;
				if( MemberToGoal( member, out obl ) ) {
					yield return obl;
				}
			}
		}

		private static bool MemberToGoal(
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
