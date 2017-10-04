﻿using Microsoft.CodeAnalysis;

namespace D2L.CodeStyle.Analyzers.Mutability.Goals {
	internal sealed class PropertyGoal : Goal {
		public PropertyGoal( IPropertySymbol property ) {
			Property = property;
		}

		public IPropertySymbol Property { get; }
	}
}
