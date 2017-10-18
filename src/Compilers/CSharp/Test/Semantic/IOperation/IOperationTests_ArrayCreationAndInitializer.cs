// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Test.Utilities;
using Roslyn.Test.Utilities;
using Xunit;

namespace Microsoft.CodeAnalysis.CSharp.UnitTests
{
    public partial class IOperationTests : SemanticModelTestBase
    {
        [Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")]
        public void SimpleArrayCreation_PrimitiveType()
        {
            string source = @"
class C
{
    public void F()
    {
        var a = /*<bind>*/new string[1]/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.String[]) (Syntax: ArrayCreationExpression, 'new string[1]') (Parent: VariableInitializer)
  Dimension Sizes(1):
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
  Initializer: 
    null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ArrayCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")]
        public void SimpleArrayCreation_UserDefinedType()
        {
            string source = @"
class M { }

class C
{
    public void F()
    {
        var a = /*<bind>*/new M[1]/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: M[]) (Syntax: ArrayCreationExpression, 'new M[1]') (Parent: VariableInitializer)
  Dimension Sizes(1):
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
  Initializer: 
    null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ArrayCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")]
        public void SimpleArrayCreation_ConstantDimension()
        {
            string source = @"
class M { }

class C
{
    public void F()
    {
        const int dimension = 1;
        var a = /*<bind>*/new M[dimension]/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: M[]) (Syntax: ArrayCreationExpression, 'new M[dimension]') (Parent: VariableInitializer)
  Dimension Sizes(1):
    ILocalReferenceExpression: dimension ([0] OperationKind.LocalReferenceExpression, Type: System.Int32, Constant: 1) (Syntax: IdentifierName, 'dimension')
  Initializer: 
    null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ArrayCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")]
        public void SimpleArrayCreation_NonConstantDimension()
        {
            string source = @"
class M { }

class C
{
    public void F(int dimension)
    {
        var a = /*<bind>*/new M[dimension]/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: M[]) (Syntax: ArrayCreationExpression, 'new M[dimension]') (Parent: VariableInitializer)
  Dimension Sizes(1):
    IParameterReferenceExpression: dimension ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'dimension')
  Initializer: 
    null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ArrayCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")]
        public void SimpleArrayCreation_DimensionWithImplicitConversion()
        {
            string source = @"
class M { }

class C
{
    public void F(char dimension)
    {
        var a = /*<bind>*/new M[dimension]/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: M[]) (Syntax: ArrayCreationExpression, 'new M[dimension]') (Parent: VariableInitializer)
  Dimension Sizes(1):
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: IdentifierName, 'dimension')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IParameterReferenceExpression: dimension ([0] OperationKind.ParameterReferenceExpression, Type: System.Char) (Syntax: IdentifierName, 'dimension')
  Initializer: 
    null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ArrayCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")]
        public void SimpleArrayCreation_DimensionWithExplicitConversion()
        {
            string source = @"
class M { }

class C
{
    public void F(object dimension)
    {
        var a = /*<bind>*/new M[(int)dimension]/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: M[]) (Syntax: ArrayCreationExpression, 'new M[(int)dimension]') (Parent: VariableInitializer)
  Dimension Sizes(1):
    IConversionExpression (Explicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32) (Syntax: CastExpression, '(int)dimension')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IParameterReferenceExpression: dimension ([0] OperationKind.ParameterReferenceExpression, Type: System.Object) (Syntax: IdentifierName, 'dimension')
  Initializer: 
    null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ArrayCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")]
        public void ArrayCreationWithInitializer_PrimitiveType()
        {
            string source = @"
class C
{
    public void F()
    {
        var a = /*<bind>*/new string[] { string.Empty }/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.String[]) (Syntax: ArrayCreationExpression, 'new string[ ... ing.Empty }') (Parent: VariableInitializer)
  Dimension Sizes(1):
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ArrayCreationExpression, 'new string[ ... ing.Empty }')
  Initializer: 
    IArrayInitializer (1 elements) ([1] OperationKind.ArrayInitializer) (Syntax: ArrayInitializerExpression, '{ string.Empty }')
      Element Values(1):
        IFieldReferenceExpression: System.String System.String.Empty (Static) ([0] OperationKind.FieldReferenceExpression, Type: System.String) (Syntax: SimpleMemberAccessExpression, 'string.Empty')
          Instance Receiver: 
            null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ArrayCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")]
        public void ArrayCreationWithInitializer_PrimitiveTypeWithExplicitDimension()
        {
            string source = @"
class C
{
    public void F()
    {
        var a = /*<bind>*/new string[1] { string.Empty }/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.String[]) (Syntax: ArrayCreationExpression, 'new string[ ... ing.Empty }') (Parent: VariableInitializer)
  Dimension Sizes(1):
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
  Initializer: 
    IArrayInitializer (1 elements) ([1] OperationKind.ArrayInitializer) (Syntax: ArrayInitializerExpression, '{ string.Empty }')
      Element Values(1):
        IFieldReferenceExpression: System.String System.String.Empty (Static) ([0] OperationKind.FieldReferenceExpression, Type: System.String) (Syntax: SimpleMemberAccessExpression, 'string.Empty')
          Instance Receiver: 
            null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ArrayCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")]
        public void ArrayCreationWithInitializerErrorCase_PrimitiveTypeWithIncorrectExplicitDimension()
        {
            string source = @"
class C
{
    public void F()
    {
        var a = /*<bind>*/new string[2] { string.Empty }/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.String[], IsInvalid) (Syntax: ArrayCreationExpression, 'new string[ ... ing.Empty }') (Parent: VariableInitializer)
  Dimension Sizes(1):
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
  Initializer: 
    IArrayInitializer (1 elements) ([1] OperationKind.ArrayInitializer, IsInvalid) (Syntax: ArrayInitializerExpression, '{ string.Empty }')
      Element Values(1):
        IFieldReferenceExpression: System.String System.String.Empty (Static) ([0] OperationKind.FieldReferenceExpression, Type: System.String, IsInvalid) (Syntax: SimpleMemberAccessExpression, 'string.Empty')
          Instance Receiver: 
            null
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0847: An array initializer of length '2' is expected
                //         var a = /*<bind>*/new string[2] { string.Empty }/*</bind>*/;
                Diagnostic(ErrorCode.ERR_ArrayInitializerIncorrectLength, "{ string.Empty }").WithArguments("2").WithLocation(6, 41)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ArrayCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")]
        public void ArrayCreationWithInitializerErrorCase_PrimitiveTypeWithNonConstantExplicitDimension()
        {
            string source = @"
class C
{
    public void F(int dimension)
    {
        var a = /*<bind>*/new string[dimension] { string.Empty }/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.String[], IsInvalid) (Syntax: ArrayCreationExpression, 'new string[ ... ing.Empty }') (Parent: VariableInitializer)
  Dimension Sizes(1):
    IParameterReferenceExpression: dimension ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32, IsInvalid) (Syntax: IdentifierName, 'dimension')
  Initializer: 
    IArrayInitializer (1 elements) ([1] OperationKind.ArrayInitializer) (Syntax: ArrayInitializerExpression, '{ string.Empty }')
      Element Values(1):
        IFieldReferenceExpression: System.String System.String.Empty (Static) ([0] OperationKind.FieldReferenceExpression, Type: System.String) (Syntax: SimpleMemberAccessExpression, 'string.Empty')
          Instance Receiver: 
            null
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0150: A constant value is expected
                //         var a = /*<bind>*/new string[dimension] { string.Empty }/*</bind>*/;
                Diagnostic(ErrorCode.ERR_ConstantExpected, "dimension").WithLocation(6, 38)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ArrayCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")]
        public void ArrayCreationWithInitializer_NoExplicitArrayCreationExpression()
        {
            string source = @"
class C
{
    public void F(int dimension)
    {
        /*<bind>*/int[] x = { 1, 2 };/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'int[] x = { 1, 2 };') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'x = { 1, 2 }')
    Variables: Local_1: System.Int32[] x
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= { 1, 2 }')
        IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.Int32[]) (Syntax: ArrayInitializerExpression, '{ 1, 2 }')
          Dimension Sizes(1):
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2, IsImplicit) (Syntax: ArrayInitializerExpression, '{ 1, 2 }')
          Initializer: 
            IArrayInitializer (2 elements) ([1] OperationKind.ArrayInitializer) (Syntax: ArrayInitializerExpression, '{ 1, 2 }')
              Element Values(2):
                ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
                ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")]
        public void ArrayCreationWithInitializer_UserDefinedType()
        {
            string source = @"
class M { }

class C
{
    public void F()
    {
        var a = /*<bind>*/new M[] { new M() }/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: M[]) (Syntax: ArrayCreationExpression, 'new M[] { new M() }') (Parent: VariableInitializer)
  Dimension Sizes(1):
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ArrayCreationExpression, 'new M[] { new M() }')
  Initializer: 
    IArrayInitializer (1 elements) ([1] OperationKind.ArrayInitializer) (Syntax: ArrayInitializerExpression, '{ new M() }')
      Element Values(1):
        IObjectCreationExpression (Constructor: M..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: M) (Syntax: ObjectCreationExpression, 'new M()')
          Arguments(0)
          Initializer: 
            null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ArrayCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")]
        public void ArrayCreationWithInitializer_ImplicitlyTyped()
        {
            string source = @"
class M { }

class C
{
    public void F()
    {
        var a = /*<bind>*/new[] { new M() }/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: M[]) (Syntax: ImplicitArrayCreationExpression, 'new[] { new M() }') (Parent: VariableInitializer)
  Dimension Sizes(1):
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ImplicitArrayCreationExpression, 'new[] { new M() }')
  Initializer: 
    IArrayInitializer (1 elements) ([1] OperationKind.ArrayInitializer) (Syntax: ArrayInitializerExpression, '{ new M() }')
      Element Values(1):
        IObjectCreationExpression (Constructor: M..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: M) (Syntax: ObjectCreationExpression, 'new M()')
          Arguments(0)
          Initializer: 
            null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ImplicitArrayCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")]
        public void ArrayCreationWithInitializerErrorCase_ImplicitlyTypedWithoutInitializerAndDimension()
        {
            string source = @"
class C
{
    public void F(int dimension)
    {
        var x = /*<bind>*/new[]/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: ?[], IsInvalid) (Syntax: ImplicitArrayCreationExpression, 'new[]/*</bind>*/') (Parent: VariableInitializer)
  Dimension Sizes(1):
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0, IsInvalid, IsImplicit) (Syntax: ImplicitArrayCreationExpression, 'new[]/*</bind>*/')
  Initializer: 
    IArrayInitializer (0 elements) ([1] OperationKind.ArrayInitializer, IsInvalid) (Syntax: ArrayInitializerExpression, '')
      Element Values(0)
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS1514: { expected
                //         var x = /*<bind>*/new[]/*</bind>*/;
                Diagnostic(ErrorCode.ERR_LbraceExpected, ";").WithLocation(6, 43),
                // CS1513: } expected
                //         var x = /*<bind>*/new[]/*</bind>*/;
                Diagnostic(ErrorCode.ERR_RbraceExpected, ";").WithLocation(6, 43),
                // CS0826: No best type found for implicitly-typed array
                //         var x = /*<bind>*/new[]/*</bind>*/;
                Diagnostic(ErrorCode.ERR_ImplicitlyTypedArrayNoBestType, "new[]/*</bind>*/").WithLocation(6, 27)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ImplicitArrayCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")]
        public void ArrayCreationWithInitializerErrorCase_ImplicitlyTypedWithoutInitializer()
        {
            string source = @"
class C
{
    public void F(int dimension)
    {
        var x = /*<bind>*/new[2]/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.Int32[], IsInvalid) (Syntax: ImplicitArrayCreationExpression, 'new[2]/*</bind>*/') (Parent: VariableInitializer)
  Dimension Sizes(1):
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsInvalid, IsImplicit) (Syntax: ImplicitArrayCreationExpression, 'new[2]/*</bind>*/')
  Initializer: 
    IArrayInitializer (1 elements) ([1] OperationKind.ArrayInitializer, IsInvalid) (Syntax: ArrayInitializerExpression, '2]/*</bind>*/')
      Element Values(1):
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2, IsInvalid) (Syntax: NumericLiteralExpression, '2')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS1003: Syntax error, ']' expected
                //         var x = /*<bind>*/new[2]/*</bind>*/;
                Diagnostic(ErrorCode.ERR_SyntaxError, "2").WithArguments("]", "").WithLocation(6, 31),
                // CS1514: { expected
                //         var x = /*<bind>*/new[2]/*</bind>*/;
                Diagnostic(ErrorCode.ERR_LbraceExpected, "2").WithLocation(6, 31),
                // CS1003: Syntax error, ',' expected
                //         var x = /*<bind>*/new[2]/*</bind>*/;
                Diagnostic(ErrorCode.ERR_SyntaxError, "]").WithArguments(",", "]").WithLocation(6, 32),
                // CS1513: } expected
                //         var x = /*<bind>*/new[2]/*</bind>*/;
                Diagnostic(ErrorCode.ERR_RbraceExpected, ";").WithLocation(6, 44)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ImplicitArrayCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")]
        public void ArrayCreationWithInitializer_MultipleInitializersWithConversions()
        {
            string source = @"
class C
{
    public void F()
    {
        var a = """";
        var b = /*<bind>*/new[] { ""hello"", a, null }/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.String[]) (Syntax: ImplicitArrayCreationExpression, 'new[] { ""he ... , a, null }') (Parent: VariableInitializer)
  Dimension Sizes(1):
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 3, IsImplicit) (Syntax: ImplicitArrayCreationExpression, 'new[] { ""he ... , a, null }')
  Initializer: 
    IArrayInitializer (3 elements) ([1] OperationKind.ArrayInitializer) (Syntax: ArrayInitializerExpression, '{ ""hello"", a, null }')
      Element Values(3):
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: ""hello"") (Syntax: StringLiteralExpression, '""hello""')
        ILocalReferenceExpression: a ([1] OperationKind.LocalReferenceExpression, Type: System.String) (Syntax: IdentifierName, 'a')
        IConversionExpression (Implicit, TryCast: False, Unchecked) ([2] OperationKind.ConversionExpression, Type: System.String, Constant: null, IsImplicit) (Syntax: NullLiteralExpression, 'null')
          Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: null, Constant: null) (Syntax: NullLiteralExpression, 'null')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ImplicitArrayCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")]
        public void MultiDimensionalArrayCreation()
        {
            string source = @"
class C
{
    public void F()
    {
        byte[,,] b = /*<bind>*/new byte[1,2,3]/*</bind>*/;

    }
}
";
            string expectedOperationTree = @"
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.Byte[,,]) (Syntax: ArrayCreationExpression, 'new byte[1,2,3]') (Parent: VariableInitializer)
  Dimension Sizes(3):
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
    ILiteralExpression ([2] OperationKind.LiteralExpression, Type: System.Int32, Constant: 3) (Syntax: NumericLiteralExpression, '3')
  Initializer: 
    null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ArrayCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")]
        public void MultiDimensionalArrayCreation_WithInitializer()
        {
            string source = @"
class C
{
    public void F()
    {
        byte[,,] b = /*<bind>*/new byte[,,] { { { 1, 2, 3 } }, { { 4, 5, 6 } } }/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.Byte[,,]) (Syntax: ArrayCreationExpression, 'new byte[,, ...  5, 6 } } }') (Parent: VariableInitializer)
  Dimension Sizes(3):
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2, IsImplicit) (Syntax: ArrayCreationExpression, 'new byte[,, ...  5, 6 } } }')
    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ArrayCreationExpression, 'new byte[,, ...  5, 6 } } }')
    ILiteralExpression ([2] OperationKind.LiteralExpression, Type: System.Int32, Constant: 3, IsImplicit) (Syntax: ArrayCreationExpression, 'new byte[,, ...  5, 6 } } }')
  Initializer: 
    IArrayInitializer (2 elements) ([3] OperationKind.ArrayInitializer) (Syntax: ArrayInitializerExpression, '{ { { 1, 2, ...  5, 6 } } }')
      Element Values(2):
        IArrayInitializer (1 elements) ([0] OperationKind.ArrayInitializer) (Syntax: ArrayInitializerExpression, '{ { 1, 2, 3 } }')
          Element Values(1):
            IArrayInitializer (3 elements) ([0] OperationKind.ArrayInitializer) (Syntax: ArrayInitializerExpression, '{ 1, 2, 3 }')
              Element Values(3):
                IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Byte, Constant: 1, IsImplicit) (Syntax: NumericLiteralExpression, '1')
                  Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                  Operand: 
                    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
                IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Byte, Constant: 2, IsImplicit) (Syntax: NumericLiteralExpression, '2')
                  Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                  Operand: 
                    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
                IConversionExpression (Implicit, TryCast: False, Unchecked) ([2] OperationKind.ConversionExpression, Type: System.Byte, Constant: 3, IsImplicit) (Syntax: NumericLiteralExpression, '3')
                  Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                  Operand: 
                    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 3) (Syntax: NumericLiteralExpression, '3')
        IArrayInitializer (1 elements) ([1] OperationKind.ArrayInitializer) (Syntax: ArrayInitializerExpression, '{ { 4, 5, 6 } }')
          Element Values(1):
            IArrayInitializer (3 elements) ([0] OperationKind.ArrayInitializer) (Syntax: ArrayInitializerExpression, '{ 4, 5, 6 }')
              Element Values(3):
                IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Byte, Constant: 4, IsImplicit) (Syntax: NumericLiteralExpression, '4')
                  Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                  Operand: 
                    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 4) (Syntax: NumericLiteralExpression, '4')
                IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Byte, Constant: 5, IsImplicit) (Syntax: NumericLiteralExpression, '5')
                  Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                  Operand: 
                    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 5) (Syntax: NumericLiteralExpression, '5')
                IConversionExpression (Implicit, TryCast: False, Unchecked) ([2] OperationKind.ConversionExpression, Type: System.Byte, Constant: 6, IsImplicit) (Syntax: NumericLiteralExpression, '6')
                  Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                  Operand: 
                    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 6) (Syntax: NumericLiteralExpression, '6')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ArrayCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")]
        public void ArrayCreationOfSingleDimensionalArrays()
        {
            string source = @"
class C
{
    public void F()
    {
        int[][] a = /*<bind>*/new int[][] { new[] { 1, 2, 3 }, new int[5] }/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.Int32[][]) (Syntax: ArrayCreationExpression, 'new int[][] ... ew int[5] }') (Parent: VariableInitializer)
  Dimension Sizes(1):
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2, IsImplicit) (Syntax: ArrayCreationExpression, 'new int[][] ... ew int[5] }')
  Initializer: 
    IArrayInitializer (2 elements) ([1] OperationKind.ArrayInitializer) (Syntax: ArrayInitializerExpression, '{ new[] { 1 ... ew int[5] }')
      Element Values(2):
        IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.Int32[]) (Syntax: ImplicitArrayCreationExpression, 'new[] { 1, 2, 3 }')
          Dimension Sizes(1):
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 3, IsImplicit) (Syntax: ImplicitArrayCreationExpression, 'new[] { 1, 2, 3 }')
          Initializer: 
            IArrayInitializer (3 elements) ([1] OperationKind.ArrayInitializer) (Syntax: ArrayInitializerExpression, '{ 1, 2, 3 }')
              Element Values(3):
                ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
                ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
                ILiteralExpression ([2] OperationKind.LiteralExpression, Type: System.Int32, Constant: 3) (Syntax: NumericLiteralExpression, '3')
        IArrayCreationExpression ([1] OperationKind.ArrayCreationExpression, Type: System.Int32[]) (Syntax: ArrayCreationExpression, 'new int[5]')
          Dimension Sizes(1):
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 5) (Syntax: NumericLiteralExpression, '5')
          Initializer: 
            null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ArrayCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")]
        public void ArrayCreationOfMultiDimensionalArrays()
        {
            string source = @"
class C
{
    public void F()
    {
        int[][,] a = /*<bind>*/new int[1][,]/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.Int32[][,]) (Syntax: ArrayCreationExpression, 'new int[1][,]') (Parent: VariableInitializer)
  Dimension Sizes(1):
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
  Initializer: 
    null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ArrayCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")]
        public void ArrayCreationOfImplicitlyTypedMultiDimensionalArrays_WithInitializer()
        {
            string source = @"
class C
{
    public void F()
    {
        var a = /*<bind>*/new[] { new[, ,] { { { 1, 2 } } }, new[, ,] { { { 3, 4 } } } }/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.Int32[][,,]) (Syntax: ImplicitArrayCreationExpression, 'new[] { new ... , 4 } } } }') (Parent: VariableInitializer)
  Dimension Sizes(1):
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2, IsImplicit) (Syntax: ImplicitArrayCreationExpression, 'new[] { new ... , 4 } } } }')
  Initializer: 
    IArrayInitializer (2 elements) ([1] OperationKind.ArrayInitializer) (Syntax: ArrayInitializerExpression, '{ new[, ,]  ... , 4 } } } }')
      Element Values(2):
        IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.Int32[,,]) (Syntax: ImplicitArrayCreationExpression, 'new[, ,] {  ...  1, 2 } } }')
          Dimension Sizes(3):
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ImplicitArrayCreationExpression, 'new[, ,] {  ...  1, 2 } } }')
            ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ImplicitArrayCreationExpression, 'new[, ,] {  ...  1, 2 } } }')
            ILiteralExpression ([2] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2, IsImplicit) (Syntax: ImplicitArrayCreationExpression, 'new[, ,] {  ...  1, 2 } } }')
          Initializer: 
            IArrayInitializer (1 elements) ([3] OperationKind.ArrayInitializer) (Syntax: ArrayInitializerExpression, '{ { { 1, 2 } } }')
              Element Values(1):
                IArrayInitializer (1 elements) ([0] OperationKind.ArrayInitializer) (Syntax: ArrayInitializerExpression, '{ { 1, 2 } }')
                  Element Values(1):
                    IArrayInitializer (2 elements) ([0] OperationKind.ArrayInitializer) (Syntax: ArrayInitializerExpression, '{ 1, 2 }')
                      Element Values(2):
                        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
                        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
        IArrayCreationExpression ([1] OperationKind.ArrayCreationExpression, Type: System.Int32[,,]) (Syntax: ImplicitArrayCreationExpression, 'new[, ,] {  ...  3, 4 } } }')
          Dimension Sizes(3):
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ImplicitArrayCreationExpression, 'new[, ,] {  ...  3, 4 } } }')
            ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ImplicitArrayCreationExpression, 'new[, ,] {  ...  3, 4 } } }')
            ILiteralExpression ([2] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2, IsImplicit) (Syntax: ImplicitArrayCreationExpression, 'new[, ,] {  ...  3, 4 } } }')
          Initializer: 
            IArrayInitializer (1 elements) ([3] OperationKind.ArrayInitializer) (Syntax: ArrayInitializerExpression, '{ { { 3, 4 } } }')
              Element Values(1):
                IArrayInitializer (1 elements) ([0] OperationKind.ArrayInitializer) (Syntax: ArrayInitializerExpression, '{ { 3, 4 } }')
                  Element Values(1):
                    IArrayInitializer (2 elements) ([0] OperationKind.ArrayInitializer) (Syntax: ArrayInitializerExpression, '{ 3, 4 }')
                      Element Values(2):
                        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 3) (Syntax: NumericLiteralExpression, '3')
                        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 4) (Syntax: NumericLiteralExpression, '4')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ImplicitArrayCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")]
        public void ArrayCreationErrorCase_MissingDimension()
        {
            string source = @"
class C
{
    public void F()
    {
        var a = /*<bind>*/new string[]/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.String[], IsInvalid) (Syntax: ArrayCreationExpression, 'new string[]') (Parent: VariableInitializer)
  Dimension Sizes(0)
  Initializer: 
    null
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS1586: Array creation must have array size or array initializer
                //         var a = /*<bind>*/new string[]/*</bind>*/;
                Diagnostic(ErrorCode.ERR_MissingArraySize, "[]").WithLocation(6, 37)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ArrayCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")]
        public void ArrayCreationErrorCase_InvalidInitializer()
        {
            string source = @"
class C
{
    public void F()
    {
        var a = /*<bind>*/new string[] { 1 }/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.String[], IsInvalid) (Syntax: ArrayCreationExpression, 'new string[] { 1 }') (Parent: VariableInitializer)
  Dimension Sizes(1):
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsInvalid, IsImplicit) (Syntax: ArrayCreationExpression, 'new string[] { 1 }')
  Initializer: 
    IArrayInitializer (1 elements) ([1] OperationKind.ArrayInitializer, IsInvalid) (Syntax: ArrayInitializerExpression, '{ 1 }')
      Element Values(1):
        IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.String, IsInvalid, IsImplicit) (Syntax: NumericLiteralExpression, '1')
          Conversion: CommonConversion (Exists: False, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsInvalid) (Syntax: NumericLiteralExpression, '1')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0029: Cannot implicitly convert type 'int' to 'string'
                //         var a = /*<bind>*/new string[] { 1 }/*</bind>*/;
                Diagnostic(ErrorCode.ERR_NoImplicitConv, "1").WithArguments("int", "string").WithLocation(6, 42)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ArrayCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")]
        public void ArrayCreationErrorCase_MissingExplicitCast()
        {
            string source = @"
class C
{
    public void F(object b)
    {
        var a = /*<bind>*/new string[b]/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.String[], IsInvalid) (Syntax: ArrayCreationExpression, 'new string[b]') (Parent: VariableInitializer)
  Dimension Sizes(1):
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'b')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IParameterReferenceExpression: b ([0] OperationKind.ParameterReferenceExpression, Type: System.Object, IsInvalid) (Syntax: IdentifierName, 'b')
  Initializer: 
    null
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0266: Cannot implicitly convert type 'object' to 'int'. An explicit conversion exists (are you missing a cast?)
                //         var a = /*<bind>*/new string[b]/*</bind>*/;
                Diagnostic(ErrorCode.ERR_NoImplicitConvCast, "new string[b]").WithArguments("object", "int").WithLocation(6, 27)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ArrayCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")]
        public void ArrayCreation_InvocationExpressionAsDimension()
        {
            string source = @"
class C
{
    public void F()
    {
        var a = /*<bind>*/new string[M()]/*</bind>*/;
    }

    public int M() => 1;
}
";
            string expectedOperationTree = @"
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.String[]) (Syntax: ArrayCreationExpression, 'new string[M()]') (Parent: VariableInitializer)
  Dimension Sizes(1):
    IInvocationExpression ( System.Int32 C.M()) ([0] OperationKind.InvocationExpression, Type: System.Int32) (Syntax: InvocationExpression, 'M()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C, IsImplicit) (Syntax: IdentifierName, 'M')
      Arguments(0)
  Initializer: 
    null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ArrayCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")]
        public void ArrayCreation_InvocationExpressionWithConversionAsDimension()
        {
            string source = @"
class C
{
    public void F()
    {
        var a = /*<bind>*/new string[(int)M()]/*</bind>*/;
    }

    public object M() => null;
}
";
            string expectedOperationTree = @"
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.String[]) (Syntax: ArrayCreationExpression, 'new string[(int)M()]') (Parent: VariableInitializer)
  Dimension Sizes(1):
    IConversionExpression (Explicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32) (Syntax: CastExpression, '(int)M()')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IInvocationExpression ( System.Object C.M()) ([0] OperationKind.InvocationExpression, Type: System.Object) (Syntax: InvocationExpression, 'M()')
          Instance Receiver: 
            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C, IsImplicit) (Syntax: IdentifierName, 'M')
          Arguments(0)
  Initializer: 
    null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ArrayCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")]
        public void ArrayCreationErrorCase_InvocationExpressionAsDimension()
        {
            string source = @"
class C
{
    public static void F()
    {
        var a = /*<bind>*/new string[M()]/*</bind>*/;
    }

    public object M() => null;
}
";
            string expectedOperationTree = @"
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.String[], IsInvalid) (Syntax: ArrayCreationExpression, 'new string[M()]') (Parent: VariableInitializer)
  Dimension Sizes(1):
    IInvocationExpression ( System.Object C.M()) ([0] OperationKind.InvocationExpression, Type: System.Object, IsInvalid) (Syntax: InvocationExpression, 'M()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'M')
      Arguments(0)
  Initializer: 
    null
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0120: An object reference is required for the non-static field, method, or property 'C.M()'
                //         var a = /*<bind>*/new string[M()]/*</bind>*/;
                Diagnostic(ErrorCode.ERR_ObjectRequired, "M").WithArguments("C.M()").WithLocation(6, 38)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ArrayCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")]
        public void ArrayCreationErrorCase_InvocationExpressionWithConversionAsDimension()
        {
            string source = @"
class C
{
    public void F()
    {
        var a = /*<bind>*/new string[(int)M()]/*</bind>*/;
    }

    public C M() => new C();
}
";
            string expectedOperationTree = @"
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.String[], IsInvalid) (Syntax: ArrayCreationExpression, 'new string[(int)M()]') (Parent: VariableInitializer)
  Dimension Sizes(1):
    IConversionExpression (Explicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32, IsInvalid) (Syntax: CastExpression, '(int)M()')
      Conversion: CommonConversion (Exists: False, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IInvocationExpression ( C C.M()) ([0] OperationKind.InvocationExpression, Type: C, IsInvalid) (Syntax: InvocationExpression, 'M()')
          Instance Receiver: 
            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'M')
          Arguments(0)
  Initializer: 
    null
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0030: Cannot convert type 'C' to 'int'
                //         var a = /*<bind>*/new string[(int)M()]/*</bind>*/;
                Diagnostic(ErrorCode.ERR_NoExplicitConv, "(int)M()").WithArguments("C", "int").WithLocation(6, 38)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ArrayCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [Fact, WorkItem(7299, "https://github.com/dotnet/roslyn/issues/7299")]
        public void SimpleArrayCreation_ConstantConversion()
        {
            string source = @"
class C
{
    public void F()
    {
        var a = /*<bind>*/new string[0.0]/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.String[], IsInvalid) (Syntax: ArrayCreationExpression, 'new string[0.0]') (Parent: VariableInitializer)
  Dimension Sizes(1):
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32, Constant: 0, IsInvalid, IsImplicit) (Syntax: NumericLiteralExpression, '0.0')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Double, Constant: 0, IsInvalid) (Syntax: NumericLiteralExpression, '0.0')
  Initializer: 
    null
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // (6,27): error CS0266: Cannot implicitly convert type 'double' to 'int'. An explicit conversion exists (are you missing a cast?)
                //         var a = /*<bind>*/new string[0.0]/*</bind>*/;
                Diagnostic(ErrorCode.ERR_NoImplicitConvCast, "new string[0.0]").WithArguments("double", "int").WithLocation(6, 27)
            };

            VerifyOperationTreeAndDiagnosticsForTest<ArrayCreationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }
    }
}
