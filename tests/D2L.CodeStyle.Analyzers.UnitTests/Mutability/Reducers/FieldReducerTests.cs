using System.Collections;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;

namespace D2L.CodeStyle.Analyzers.Mutability.Reducers {
	[TestFixtureSource(nameof(m_testFixtureNames))]
	internal sealed class ReducerTests {
		private static readonly IEnumerable m_testFixtureNames;
		private static readonly ImmutableDictionary<string, string> m_testFixtureSource;

		private readonly string m_source;
		private readonly Compilation m_compilation;
		private readonly SyntaxTree m_syntaxRoot;
		private readonly SemanticModel m_model;

		static ReducerTests() {
			var builder = ImmutableDictionary.CreateBuilder<string, string>();

			LoadAllTestFixtureSource( builder );

			m_testFixtureSource = builder.ToImmutable();

			m_testFixtureNames = m_testFixtureSource.Keys.ToImmutableArray();
		}

		public ReducerTests( string testFixtureName ) {
			var source = m_testFixtureSource[testFixtureName];
			var project = GetProjectForSource( testFixtureName, source );

			m_compilation = project.GetCompilationAsync().Result;
			m_syntaxRoot = project.Documents.First().GetSyntaxTreeAsync().Result;
			m_model = m_compilation.GetSemanticModel( m_syntaxRoot );
		}

		[Test]
		public void AllGood() {
			var assertions = m_syntaxRoot.GetRoot()
				.DescendantTrivia()
				.Where( c => c.Kind() == SyntaxKind.MultiLineCommentTrivia );


			var things = assertions.Select( a => a.Token.Parent ).ToList();

			Assert.IsNull( things );

		}

		private static Project GetProjectForSource( string testFixtureName, string source ) {
			var projectId = ProjectId.CreateNewId( debugName: testFixtureName );
			var fileName = testFixtureName + ".cs";
			var documentId = DocumentId.CreateNewId( projectId, debugName: fileName );

			var solution = new AdhocWorkspace()
				.CurrentSolution
				.AddProject( projectId, testFixtureName, testFixtureName, LanguageNames.CSharp )

				// mscorlib
				.AddMetadataReference(
					projectId,
					MetadataReference.CreateFromFile( typeof( object ).Assembly.Location )
				)

				// System.Core
				.AddMetadataReference(
					projectId,
					MetadataReference.CreateFromFile( typeof( IEnumerable ).Assembly.Location )
				)

				// System.Collections.Immutable
				.AddMetadataReference(
					projectId,
					MetadataReference.CreateFromFile( typeof( ImmutableArray<string> ).Assembly.Location )
				)

				.AddDocument( documentId, fileName, SourceText.From( source ) );


			return solution.Projects.First();
		}

		const string RESOURCE_NAME_PREFIX = "D2L.CodeStyle.Analyzers.Mutability.Reducers.";

		public static void LoadAllTestFixtureSource( ImmutableDictionary<string, string>.Builder builder ) {
			var assembly = Assembly.GetExecutingAssembly();

			var testSources = assembly.GetManifestResourceNames()
				.Where( name => name.StartsWith( RESOURCE_NAME_PREFIX ) )
				.Where( name => name.EndsWith( ".cs" ) )
				.ToImmutableArray();

			foreach( var testSourceResourceName in testSources ) {
				string source;

				using( var stream = assembly.GetManifestResourceStream( testSourceResourceName ) )
				using( var reader = new StreamReader( stream ) ) {
					source = reader.ReadToEnd();
				}

				var testFixtureName = testSourceResourceName.Substring( RESOURCE_NAME_PREFIX.Length );
				testFixtureName = testFixtureName.Substring( 0, testFixtureName.Length - 3 );
				builder[testFixtureName] = source;
			}
		}
	}
}
