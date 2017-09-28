using System;
using System.Collections.Generic;
using System.Linq;
using D2L.CodeStyle.Analyzers.Mutability.Goals;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace D2L.CodeStyle.Analyzers.Mutability.Reducers {
	internal static class FieldReducer {
		public static IEnumerable<Goal> Apply(
			SemanticModel model,
			FieldGoal obligation
		) {
			if ( !obligation.Field.IsReadOnly ) {
				throw new Exception( "mutable" );
			}

			var fieldSyntax = GetSyntax( obligation.Field );

			var missingAnInitializer = fieldSyntax
				.Declaration
				.Variables.Any( v => v.Initializer == null );

            // in general we can use the fields type as a base type
            if ( missingAnInitializer ) {
                yield return new TypeGoal( obligation.Field.Type );
                yield break;
            }

            // If every variable has an initializer, analyze those instead.
            // The type is possibly narrower (never wider) and there may be a
            // special case we can apply.
            // BUG: implicit casts aren't visible here (see issue XYZ)
			foreach( var variable in fieldSyntax.Declaration.Variables ) {
				yield return new InitializerGoal(
					variable.Initializer.Value
				);
			}
		}

		private static FieldDeclarationSyntax GetSyntax( IFieldSymbol fieldSymbol ) {
			// This would be unexpected:
			if ( fieldSymbol.DeclaringSyntaxReferences.Length != 1 ) {
				throw new NotImplementedException();
			}

			var syntax = (FieldDeclarationSyntax)fieldSymbol.DeclaringSyntaxReferences[0].GetSyntax();

			// This would be unexpected
			if ( syntax == null ) {
				throw new NotImplementedException();
			}

			return syntax;
		}
	}
}
