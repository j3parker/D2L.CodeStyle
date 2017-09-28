using System;
using System.Collections.Generic;
using D2L.CodeStyle.Analyzers.Common;
using D2L.CodeStyle.Analyzers.Mutability.Goals;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace D2L.CodeStyle.Analyzers.Mutability.Reducers {
	internal static class DefaultPropertyTactic {
		public static IEnumerable<Goal> Apply(
			SemanticModel model,
			PropertyGoal obligation
		) {
			var propertySyntax = GetSyntax( obligation.Property );

			// Properties that aren't auto-implemented, e.g.
			//   private int Celcius {
			//     get { return m_farenheit*3; }
			//     set { m_farenheit = value+3; }
			//   }
			// don't hold state themselves (in this case, m_farenheit holds
			// state.) They are essentially syntax-sugar for getter/setter
			// methods.
			if ( !propertySyntax.IsAutoImplemented() ) {
				yield break;
			}

			// TODO: validate that m_property.ExpressionBody is null

			if ( !obligation.Property.IsReadOnly ) {
				throw new Exception("mutable");
			}

			if ( propertySyntax.Initializer != null ) {
				// Prefer the initializer for analysis because it may be of a
				// narrower type or a special case (e.g. "new T()")
				yield return new InitializerGoal( propertySyntax.Initializer.Value );
			} else {
				// In general the property may hold a value of any subtype of
				// the properties declared type.
				yield return new TypeGoal( obligation.Property.Type );
			}
		} 

		private static PropertyDeclarationSyntax GetSyntax(
			IPropertySymbol propertySymbol
		) {
			if ( propertySymbol.DeclaringSyntaxReferences.Length != 1 ) {
				throw new NotImplementedException(
					$"Unexpected number of declaring syntax references for a property: {propertySymbol.DeclaringSyntaxReferences.Length}"
				);
			}

			var syntax = (PropertyDeclarationSyntax)propertySymbol.DeclaringSyntaxReferences[0].GetSyntax();

			if ( syntax == null ) {
				throw new NotImplementedException(
					"Unexpected failure casting to PropertyDeclarationSyntax"
				);
			}

			return syntax;
		}
	}
}
