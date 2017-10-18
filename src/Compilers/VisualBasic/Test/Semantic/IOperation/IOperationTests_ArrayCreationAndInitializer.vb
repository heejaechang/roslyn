' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Test.Utilities

Namespace Microsoft.CodeAnalysis.VisualBasic.UnitTests.Semantics

    Partial Public Class IOperationTests
        Inherits SemanticModelTestBase

        <Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")>
        Public Sub SimpleArrayCreation_PrimitiveType()
            Dim source = <![CDATA[
Class C
    Public Sub F()
        Dim a = New String(0) {}'BIND:"New String(0) {}"
    End Sub
End Class
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.String()) (Syntax: ArrayCreationExpression, 'New String(0) {}') (Parent: VariableInitializer)
  Dimension Sizes(1):
    IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: SimpleArgument, '0')
      Left: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: SimpleArgument, '0')
  Initializer: 
    IArrayInitializer (0 elements) ([1] OperationKind.ArrayInitializer) (Syntax: CollectionInitializer, '{}')
      Element Values(0)
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of ArrayCreationExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")>
        Public Sub SimpleArrayCreation_UserDefinedType()
            Dim source = <![CDATA[
Class M
End Class

Class C
    Public Sub F()
        Dim a = New M() {}'BIND:"New M() {}"
    End Sub
End Class
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: M()) (Syntax: ArrayCreationExpression, 'New M() {}') (Parent: VariableInitializer)
  Dimension Sizes(1):
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0, IsImplicit) (Syntax: ArrayCreationExpression, 'New M() {}')
  Initializer: 
    IArrayInitializer (0 elements) ([1] OperationKind.ArrayInitializer) (Syntax: CollectionInitializer, '{}')
      Element Values(0)
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of ArrayCreationExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")>
        Public Sub SimpleArrayCreation_ConstantDimension()
            Dim source = <![CDATA[
Class M
End Class

Class C
    Public Sub F()
        Const dimension As Integer = 1
        Dim a = New M(dimension) {}'BIND:"New M(dimension) {}"
    End Sub
End Class
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: M()) (Syntax: ArrayCreationExpression, 'New M(dimension) {}') (Parent: VariableInitializer)
  Dimension Sizes(1):
    IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32, Constant: 2, IsImplicit) (Syntax: SimpleArgument, 'dimension')
      Left: 
        ILocalReferenceExpression: dimension ([0] OperationKind.LocalReferenceExpression, Type: System.Int32, Constant: 1) (Syntax: IdentifierName, 'dimension')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: SimpleArgument, 'dimension')
  Initializer: 
    IArrayInitializer (0 elements) ([1] OperationKind.ArrayInitializer) (Syntax: CollectionInitializer, '{}')
      Element Values(0)
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of ArrayCreationExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")>
        Public Sub SimpleArrayCreation_NonConstantDimension()
            Dim source = <![CDATA[
Class M
End Class

Class C
    Public Sub F(dimension As Integer)
        Dim a = New M(dimension) {}'BIND:"New M(dimension) {}"
    End Sub
End Class
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: M()) (Syntax: ArrayCreationExpression, 'New M(dimension) {}') (Parent: VariableInitializer)
  Dimension Sizes(1):
    IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32, IsImplicit) (Syntax: SimpleArgument, 'dimension')
      Left: 
        IParameterReferenceExpression: dimension ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'dimension')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: SimpleArgument, 'dimension')
  Initializer: 
    IArrayInitializer (0 elements) ([1] OperationKind.ArrayInitializer) (Syntax: CollectionInitializer, '{}')
      Element Values(0)
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of ArrayCreationExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")>
        Public Sub SimpleArrayCreation_DimensionWithImplicitConversion()
            Dim source = <![CDATA[
Imports System

Class M
End Class

Class C
    Public Sub F(dimension As UInt16)
        Dim a = New M(dimension) {}'BIND:"New M(dimension) {}"
    End Sub
End Class
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: M()) (Syntax: ArrayCreationExpression, 'New M(dimension) {}') (Parent: VariableInitializer)
  Dimension Sizes(1):
    IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32, IsImplicit) (Syntax: SimpleArgument, 'dimension')
      Left: 
        IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: IdentifierName, 'dimension')
          Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            IParameterReferenceExpression: dimension ([0] OperationKind.ParameterReferenceExpression, Type: System.UInt16) (Syntax: IdentifierName, 'dimension')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: SimpleArgument, 'dimension')
  Initializer: 
    IArrayInitializer (0 elements) ([1] OperationKind.ArrayInitializer) (Syntax: CollectionInitializer, '{}')
      Element Values(0)
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of ArrayCreationExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")>
        Public Sub SimpleArrayCreation_DimensionWithExplicitConversion()
            Dim source = <![CDATA[
Class M
End Class

Class C
    Public Sub F(dimension As Object)
        Dim a = New M(DirectCast(dimension, Integer)) {}'BIND:"New M(DirectCast(dimension, Integer)) {}"
    End Sub
End Class
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: M()) (Syntax: ArrayCreationExpression, 'New M(Direc ... nteger)) {}') (Parent: VariableInitializer)
  Dimension Sizes(1):
    IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32, IsImplicit) (Syntax: SimpleArgument, 'DirectCast( ... n, Integer)')
      Left: 
        IConversionExpression (Explicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32) (Syntax: DirectCastExpression, 'DirectCast( ... n, Integer)')
          Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            IParameterReferenceExpression: dimension ([0] OperationKind.ParameterReferenceExpression, Type: System.Object) (Syntax: IdentifierName, 'dimension')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: SimpleArgument, 'DirectCast( ... n, Integer)')
  Initializer: 
    IArrayInitializer (0 elements) ([1] OperationKind.ArrayInitializer) (Syntax: CollectionInitializer, '{}')
      Element Values(0)
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of ArrayCreationExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")>
        Public Sub ArrayCreationWithInitializer_PrimitiveType()
            Dim source = <![CDATA[
Class C
    Public Sub F(dimension As Object)
        Dim a = New String() {String.Empty}'BIND:"New String() {String.Empty}"
    End Sub
End Class
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.String()) (Syntax: ArrayCreationExpression, 'New String( ... ring.Empty}') (Parent: VariableInitializer)
  Dimension Sizes(1):
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ArrayCreationExpression, 'New String( ... ring.Empty}')
  Initializer: 
    IArrayInitializer (1 elements) ([1] OperationKind.ArrayInitializer) (Syntax: CollectionInitializer, '{String.Empty}')
      Element Values(1):
        IFieldReferenceExpression: System.String.Empty As System.String (Static) ([0] OperationKind.FieldReferenceExpression, Type: System.String) (Syntax: SimpleMemberAccessExpression, 'String.Empty')
          Instance Receiver: 
            null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of ArrayCreationExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")>
        Public Sub ArrayCreationWithInitializer_WithExplicitDimension()
            Dim source = <![CDATA[
Class C
    Public Sub F()
        Dim a = New C(1) {New C, Nothing}'BIND:"New C(1) {New C, Nothing}"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: C()) (Syntax: ArrayCreationExpression, 'New C(1) {N ... C, Nothing}') (Parent: VariableInitializer)
  Dimension Sizes(1):
    IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32, Constant: 2, IsImplicit) (Syntax: SimpleArgument, '1')
      Left: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: SimpleArgument, '1')
  Initializer: 
    IArrayInitializer (2 elements) ([1] OperationKind.ArrayInitializer) (Syntax: CollectionInitializer, '{New C, Nothing}')
      Element Values(2):
        IObjectCreationExpression (Constructor: Sub C..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: C) (Syntax: ObjectCreationExpression, 'New C')
          Arguments(0)
          Initializer: 
            null
        IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: C, Constant: null, IsImplicit) (Syntax: NothingLiteralExpression, 'Nothing')
          Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: null, Constant: null) (Syntax: NothingLiteralExpression, 'Nothing')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of ArrayCreationExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")>
        Public Sub ArrayCreationWithInitializerErrorCase_WithIncorrectExplicitDimension()
            Dim source = <![CDATA[
Class C
    Public Sub F()
        Dim a = New C(2) {New C}'BIND:"New C(2) {New C}"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: C(), IsInvalid) (Syntax: ArrayCreationExpression, 'New C(2) {New C}') (Parent: VariableInitializer)
  Dimension Sizes(1):
    IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32, Constant: 3, IsImplicit) (Syntax: SimpleArgument, '2')
      Left: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: SimpleArgument, '2')
  Initializer: 
    IArrayInitializer (1 elements) ([1] OperationKind.ArrayInitializer, IsInvalid) (Syntax: CollectionInitializer, '{New C}')
      Element Values(1):
        IObjectCreationExpression (Constructor: Sub C..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: C, IsInvalid) (Syntax: ObjectCreationExpression, 'New C')
          Arguments(0)
          Initializer: 
            null
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30567: Array initializer is missing 2 elements.
        Dim a = New C(2) {New C}'BIND:"New C(2) {New C}"
                         ~~~~~~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of ArrayCreationExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")>
        Public Sub ArrayCreationWithInitializerErrorCase_WithNonConstantExpressionExplicitDimension()
            Dim source = <![CDATA[
Class C
    Public Sub F()
        Dim x = New Integer(2) {1, 2, 3}
        x = New Integer(x(0)) {1, 2}'BIND:"New Integer(x(0)) {1, 2}"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IArrayCreationExpression ([1] OperationKind.ArrayCreationExpression, Type: System.Int32(), IsInvalid) (Syntax: ArrayCreationExpression, 'New Integer(x(0)) {1, 2}') (Parent: SimpleAssignmentExpression)
  Dimension Sizes(1):
    IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32, IsImplicit) (Syntax: SimpleArgument, 'x(0)')
      Left: 
        IArrayElementReferenceExpression ([0] OperationKind.ArrayElementReferenceExpression, Type: System.Int32) (Syntax: InvocationExpression, 'x(0)')
          Array reference: 
            ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32()) (Syntax: IdentifierName, 'x')
          Indices(1):
            ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: SimpleArgument, 'x(0)')
  Initializer: 
    IArrayInitializer (2 elements) ([1] OperationKind.ArrayInitializer, IsInvalid) (Syntax: CollectionInitializer, '{1, 2}')
      Element Values(2):
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsInvalid) (Syntax: NumericLiteralExpression, '1')
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2, IsInvalid) (Syntax: NumericLiteralExpression, '2')
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30949: Array initializer cannot be specified for a non constant dimension; use the empty initializer '{}'.
        x = New Integer(x(0)) {1, 2}'BIND:"New Integer(x(0)) {1, 2}"
                              ~~~~~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of ArrayCreationExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")>
        Public Sub ArrayCreationWithInitializer_UserDefinedType()
            Dim source = <![CDATA[
Class M
End Class

Class C
    Public Sub F(dimension As Object)
        Dim a = New M() {New M}'BIND:"New M() {New M}"
    End Sub
End Class
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: M()) (Syntax: ArrayCreationExpression, 'New M() {New M}') (Parent: VariableInitializer)
  Dimension Sizes(1):
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ArrayCreationExpression, 'New M() {New M}')
  Initializer: 
    IArrayInitializer (1 elements) ([1] OperationKind.ArrayInitializer) (Syntax: CollectionInitializer, '{New M}')
      Element Values(1):
        IObjectCreationExpression (Constructor: Sub M..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: M) (Syntax: ObjectCreationExpression, 'New M')
          Arguments(0)
          Initializer: 
            null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of ArrayCreationExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")>
        Public Sub ArrayCreationWithInitializer_ImplicitlyTyped()
            Dim source = <![CDATA[
Class M
End Class

Class C
    Public Sub F(dimension As Object)
        Dim a = {New M}'BIND:"{New M}"
    End Sub
End Class
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: M()) (Syntax: CollectionInitializer, '{New M}') (Parent: VariableInitializer)
  Dimension Sizes(1):
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: CollectionInitializer, '{New M}')
  Initializer: 
    IArrayInitializer (1 elements) ([1] OperationKind.ArrayInitializer, IsImplicit) (Syntax: CollectionInitializer, '{New M}')
      Element Values(1):
        IObjectCreationExpression (Constructor: Sub M..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: M) (Syntax: ObjectCreationExpression, 'New M')
          Arguments(0)
          Initializer: 
            null
]]>.Value
            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of CollectionInitializerSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")>
        Public Sub ArrayCreationWithInitializer_ImplicitlyTypedWithoutInitializerAndDimension()
            Dim source = <![CDATA[
Class C
    Public Sub F()
        Dim a = {}'BIND:"Dim a = {}"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Dim a = {}') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'a')
    Variables: Local_1: a As System.Object()
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValue, '= {}')
        IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.Object()) (Syntax: CollectionInitializer, '{}')
          Dimension Sizes(1):
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0, IsImplicit) (Syntax: CollectionInitializer, '{}')
          Initializer: 
            IArrayInitializer (0 elements) ([1] OperationKind.ArrayInitializer, IsImplicit) (Syntax: CollectionInitializer, '{}')
              Element Values(0)
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")>
        Public Sub ArrayCreationWithInitializer_MultipleInitializersWithConversions()
            Dim source = <![CDATA[
Class C
    Public Sub F()
        Dim a = ""
        Dim b = {"hello", a, Nothing}'BIND:"{"hello", a, Nothing}"
    End Sub
End Class
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.String()) (Syntax: CollectionInitializer, '{"hello", a, Nothing}') (Parent: VariableInitializer)
  Dimension Sizes(1):
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 3, IsImplicit) (Syntax: CollectionInitializer, '{"hello", a, Nothing}')
  Initializer: 
    IArrayInitializer (3 elements) ([1] OperationKind.ArrayInitializer, IsImplicit) (Syntax: CollectionInitializer, '{"hello", a, Nothing}')
      Element Values(3):
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: "hello") (Syntax: StringLiteralExpression, '"hello"')
        ILocalReferenceExpression: a ([1] OperationKind.LocalReferenceExpression, Type: System.String) (Syntax: IdentifierName, 'a')
        IConversionExpression (Implicit, TryCast: False, Unchecked) ([2] OperationKind.ConversionExpression, Type: System.String, Constant: null, IsImplicit) (Syntax: NothingLiteralExpression, 'Nothing')
          Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: null, Constant: null) (Syntax: NothingLiteralExpression, 'Nothing')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of CollectionInitializerSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")>
        Public Sub MultiDimensionalArrayCreation()
            Dim source = <![CDATA[
Class C
    Public Sub F()
        Dim b As Byte(,,) = New Byte(0, 1, 2) {}'BIND:"New Byte(0, 1, 2) {}"
    End Sub
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.Byte(,,)) (Syntax: ArrayCreationExpression, 'New Byte(0, 1, 2) {}') (Parent: VariableInitializer)
  Dimension Sizes(3):
    IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: SimpleArgument, '0')
      Left: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: SimpleArgument, '0')
    IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32, Constant: 2, IsImplicit) (Syntax: SimpleArgument, '1')
      Left: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: SimpleArgument, '1')
    IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([2] OperationKind.BinaryOperatorExpression, Type: System.Int32, Constant: 3, IsImplicit) (Syntax: SimpleArgument, '2')
      Left: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: SimpleArgument, '2')
  Initializer: 
    IArrayInitializer (0 elements) ([3] OperationKind.ArrayInitializer) (Syntax: CollectionInitializer, '{}')
      Element Values(0)
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of ArrayCreationExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")>
        Public Sub MultiDimensionalArrayCreation_WithInitializer()
            Dim source = <![CDATA[
Class C
    Public Sub F()
        Dim b As Byte(,,) = New Byte(,,) {{{1, 2, 3}}, {{4, 5, 6}}}'BIND:"New Byte(,,) {{{1, 2, 3}}, {{4, 5, 6}}}"
    End Sub
End Class
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.Byte(,,)) (Syntax: ArrayCreationExpression, 'New Byte(,, ... {4, 5, 6}}}') (Parent: VariableInitializer)
  Dimension Sizes(3):
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2, IsImplicit) (Syntax: ArrayCreationExpression, 'New Byte(,, ... {4, 5, 6}}}')
    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ArrayCreationExpression, 'New Byte(,, ... {4, 5, 6}}}')
    ILiteralExpression ([2] OperationKind.LiteralExpression, Type: System.Int32, Constant: 3, IsImplicit) (Syntax: ArrayCreationExpression, 'New Byte(,, ... {4, 5, 6}}}')
  Initializer: 
    IArrayInitializer (2 elements) ([3] OperationKind.ArrayInitializer) (Syntax: CollectionInitializer, '{{{1, 2, 3} ... {4, 5, 6}}}')
      Element Values(2):
        IArrayInitializer (1 elements) ([0] OperationKind.ArrayInitializer) (Syntax: CollectionInitializer, '{{1, 2, 3}}')
          Element Values(1):
            IArrayInitializer (3 elements) ([0] OperationKind.ArrayInitializer) (Syntax: CollectionInitializer, '{1, 2, 3}')
              Element Values(3):
                IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Byte, Constant: 1, IsImplicit) (Syntax: NumericLiteralExpression, '1')
                  Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                  Operand: 
                    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
                IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Byte, Constant: 2, IsImplicit) (Syntax: NumericLiteralExpression, '2')
                  Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                  Operand: 
                    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
                IConversionExpression (Implicit, TryCast: False, Unchecked) ([2] OperationKind.ConversionExpression, Type: System.Byte, Constant: 3, IsImplicit) (Syntax: NumericLiteralExpression, '3')
                  Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                  Operand: 
                    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 3) (Syntax: NumericLiteralExpression, '3')
        IArrayInitializer (1 elements) ([1] OperationKind.ArrayInitializer) (Syntax: CollectionInitializer, '{{4, 5, 6}}')
          Element Values(1):
            IArrayInitializer (3 elements) ([0] OperationKind.ArrayInitializer) (Syntax: CollectionInitializer, '{4, 5, 6}')
              Element Values(3):
                IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Byte, Constant: 4, IsImplicit) (Syntax: NumericLiteralExpression, '4')
                  Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                  Operand: 
                    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 4) (Syntax: NumericLiteralExpression, '4')
                IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Byte, Constant: 5, IsImplicit) (Syntax: NumericLiteralExpression, '5')
                  Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                  Operand: 
                    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 5) (Syntax: NumericLiteralExpression, '5')
                IConversionExpression (Implicit, TryCast: False, Unchecked) ([2] OperationKind.ConversionExpression, Type: System.Byte, Constant: 6, IsImplicit) (Syntax: NumericLiteralExpression, '6')
                  Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                  Operand: 
                    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 6) (Syntax: NumericLiteralExpression, '6')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of ArrayCreationExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")>
        Public Sub ArrayCreationOfSingleDimensionalArrays()
            Dim source = <![CDATA[
Class C
    Public Sub F()
        Dim a = {{1, 2, 3}, {4, 5, 6}}'BIND:"{{1, 2, 3}, {4, 5, 6}}"
    End Sub
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.Int32(,)) (Syntax: CollectionInitializer, '{{1, 2, 3}, {4, 5, 6}}') (Parent: VariableInitializer)
  Dimension Sizes(2):
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2, IsImplicit) (Syntax: CollectionInitializer, '{{1, 2, 3}, {4, 5, 6}}')
    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 3, IsImplicit) (Syntax: CollectionInitializer, '{{1, 2, 3}, {4, 5, 6}}')
  Initializer: 
    IArrayInitializer (2 elements) ([2] OperationKind.ArrayInitializer, IsImplicit) (Syntax: CollectionInitializer, '{{1, 2, 3}, {4, 5, 6}}')
      Element Values(2):
        IArrayInitializer (3 elements) ([0] OperationKind.ArrayInitializer) (Syntax: CollectionInitializer, '{1, 2, 3}')
          Element Values(3):
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
            ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
            ILiteralExpression ([2] OperationKind.LiteralExpression, Type: System.Int32, Constant: 3) (Syntax: NumericLiteralExpression, '3')
        IArrayInitializer (3 elements) ([1] OperationKind.ArrayInitializer) (Syntax: CollectionInitializer, '{4, 5, 6}')
          Element Values(3):
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 4) (Syntax: NumericLiteralExpression, '4')
            ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 5) (Syntax: NumericLiteralExpression, '5')
            ILiteralExpression ([2] OperationKind.LiteralExpression, Type: System.Int32, Constant: 6) (Syntax: NumericLiteralExpression, '6')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of CollectionInitializerSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")>
        Public Sub ArrayCreationOfMultiDimensionalArrays()
            Dim source = <![CDATA[
Class C
    Public Sub F()
        Dim a As Integer()(,) = New Integer(0)(,) {}'BIND:"New Integer(0)(,) {}"
    End Sub
End Class
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.Int32()(,)) (Syntax: ArrayCreationExpression, 'New Integer(0)(,) {}') (Parent: VariableInitializer)
  Dimension Sizes(1):
    IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: SimpleArgument, '0')
      Left: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: SimpleArgument, '0')
  Initializer: 
    IArrayInitializer (0 elements) ([1] OperationKind.ArrayInitializer) (Syntax: CollectionInitializer, '{}')
      Element Values(0)
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of ArrayCreationExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")>
        Public Sub ArrayCreationOfMultiDimensionalArrays_MultipleExplicitNonConstantDimensions()
            Dim source = <![CDATA[
Class C
    Public Sub F(x As Integer())
        Dim y = New Integer(x(0), x(1)) {}'BIND:"New Integer(x(0), x(1)) {}"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.Int32(,)) (Syntax: ArrayCreationExpression, 'New Integer ... ), x(1)) {}') (Parent: VariableInitializer)
  Dimension Sizes(2):
    IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32, IsImplicit) (Syntax: SimpleArgument, 'x(0)')
      Left: 
        IArrayElementReferenceExpression ([0] OperationKind.ArrayElementReferenceExpression, Type: System.Int32) (Syntax: InvocationExpression, 'x(0)')
          Array reference: 
            IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32()) (Syntax: IdentifierName, 'x')
          Indices(1):
            ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: SimpleArgument, 'x(0)')
    IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32, IsImplicit) (Syntax: SimpleArgument, 'x(1)')
      Left: 
        IArrayElementReferenceExpression ([0] OperationKind.ArrayElementReferenceExpression, Type: System.Int32) (Syntax: InvocationExpression, 'x(1)')
          Array reference: 
            IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32()) (Syntax: IdentifierName, 'x')
          Indices(1):
            ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: SimpleArgument, 'x(1)')
  Initializer: 
    IArrayInitializer (0 elements) ([2] OperationKind.ArrayInitializer) (Syntax: CollectionInitializer, '{}')
      Element Values(0)
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of ArrayCreationExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")>
        Public Sub ArrayCreationOfMultiDimensionalArrays_MultipleExplicitConstantDimensions()
            Dim source = <![CDATA[
Class C
    Public Sub F()
        Dim y = New Integer(1, 1) {}'BIND:"New Integer(1, 1) {}"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.Int32(,)) (Syntax: ArrayCreationExpression, 'New Integer(1, 1) {}') (Parent: VariableInitializer)
  Dimension Sizes(2):
    IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32, Constant: 2, IsImplicit) (Syntax: SimpleArgument, '1')
      Left: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: SimpleArgument, '1')
    IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32, Constant: 2, IsImplicit) (Syntax: SimpleArgument, '1')
      Left: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: SimpleArgument, '1')
  Initializer: 
    IArrayInitializer (0 elements) ([2] OperationKind.ArrayInitializer) (Syntax: CollectionInitializer, '{}')
      Element Values(0)
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of ArrayCreationExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")>
        Public Sub ArrayCreationOfMultiDimensionalArraysErrorCase_InitializerMissingElements()
            Dim source = <![CDATA[
Class C
    Public Sub F()
        Dim y = New Integer(1, 1) {{}}'BIND:"New Integer(1, 1) {{}}"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.Int32(,), IsInvalid) (Syntax: ArrayCreationExpression, 'New Integer(1, 1) {{}}') (Parent: VariableInitializer)
  Dimension Sizes(2):
    IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32, Constant: 2, IsImplicit) (Syntax: SimpleArgument, '1')
      Left: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: SimpleArgument, '1')
    IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32, Constant: 2, IsImplicit) (Syntax: SimpleArgument, '1')
      Left: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: SimpleArgument, '1')
  Initializer: 
    IArrayInitializer (1 elements) ([2] OperationKind.ArrayInitializer, IsInvalid) (Syntax: CollectionInitializer, '{{}}')
      Element Values(1):
        IArrayInitializer (0 elements) ([0] OperationKind.ArrayInitializer, IsInvalid) (Syntax: CollectionInitializer, '{}')
          Element Values(0)
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30567: Array initializer is missing 1 elements.
        Dim y = New Integer(1, 1) {{}}'BIND:"New Integer(1, 1) {{}}"
                                  ~~~~
BC30567: Array initializer is missing 2 elements.
        Dim y = New Integer(1, 1) {{}}'BIND:"New Integer(1, 1) {{}}"
                                   ~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of ArrayCreationExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")>
        Public Sub ArrayCreationOfMultiDimensionalArraysErrorCase_InitializerMissingElements02()
            Dim source = <![CDATA[
Class C
    Public Sub F()
        Dim y = New Integer(1, 1) {{}, {}}'BIND:"New Integer(1, 1) {{}, {}}"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.Int32(,), IsInvalid) (Syntax: ArrayCreationExpression, 'New Integer ... 1) {{}, {}}') (Parent: VariableInitializer)
  Dimension Sizes(2):
    IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32, Constant: 2, IsImplicit) (Syntax: SimpleArgument, '1')
      Left: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: SimpleArgument, '1')
    IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32, Constant: 2, IsImplicit) (Syntax: SimpleArgument, '1')
      Left: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: SimpleArgument, '1')
  Initializer: 
    IArrayInitializer (2 elements) ([2] OperationKind.ArrayInitializer, IsInvalid) (Syntax: CollectionInitializer, '{{}, {}}')
      Element Values(2):
        IArrayInitializer (0 elements) ([0] OperationKind.ArrayInitializer, IsInvalid) (Syntax: CollectionInitializer, '{}')
          Element Values(0)
        IArrayInitializer (0 elements) ([1] OperationKind.ArrayInitializer, IsInvalid) (Syntax: CollectionInitializer, '{}')
          Element Values(0)
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30567: Array initializer is missing 2 elements.
        Dim y = New Integer(1, 1) {{}, {}}'BIND:"New Integer(1, 1) {{}, {}}"
                                   ~~
BC30567: Array initializer is missing 2 elements.
        Dim y = New Integer(1, 1) {{}, {}}'BIND:"New Integer(1, 1) {{}, {}}"
                                       ~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of ArrayCreationExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")>
        Public Sub ArrayCreationOfMultiDimensionalArraysErrorCase_InitializerMissingElements03()
            Dim source = <![CDATA[
Class C
    Public Sub F()
        Dim y = New Integer(1, 1) {{1, 2}}'BIND:"New Integer(1, 1) {{1, 2}}"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.Int32(,), IsInvalid) (Syntax: ArrayCreationExpression, 'New Integer ... 1) {{1, 2}}') (Parent: VariableInitializer)
  Dimension Sizes(2):
    IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32, Constant: 2, IsImplicit) (Syntax: SimpleArgument, '1')
      Left: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: SimpleArgument, '1')
    IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32, Constant: 2, IsImplicit) (Syntax: SimpleArgument, '1')
      Left: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: SimpleArgument, '1')
  Initializer: 
    IArrayInitializer (1 elements) ([2] OperationKind.ArrayInitializer, IsInvalid) (Syntax: CollectionInitializer, '{{1, 2}}')
      Element Values(1):
        IArrayInitializer (2 elements) ([0] OperationKind.ArrayInitializer, IsInvalid) (Syntax: CollectionInitializer, '{1, 2}')
          Element Values(2):
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsInvalid) (Syntax: NumericLiteralExpression, '1')
            ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2, IsInvalid) (Syntax: NumericLiteralExpression, '2')
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30567: Array initializer is missing 1 elements.
        Dim y = New Integer(1, 1) {{1, 2}}'BIND:"New Integer(1, 1) {{1, 2}}"
                                  ~~~~~~~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of ArrayCreationExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")>
        Public Sub ArrayCreationOfMultiDimensionalArraysErrorCase_InitializerMissingElements04()
            Dim source = <![CDATA[
Class C
    Public Sub F()
        Dim y = New Integer(1, 1) {{1, 2}, {}}'BIND:"New Integer(1, 1) {{1, 2}, {}}"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.Int32(,), IsInvalid) (Syntax: ArrayCreationExpression, 'New Integer ... {1, 2}, {}}') (Parent: VariableInitializer)
  Dimension Sizes(2):
    IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32, Constant: 2, IsImplicit) (Syntax: SimpleArgument, '1')
      Left: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: SimpleArgument, '1')
    IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32, Constant: 2, IsImplicit) (Syntax: SimpleArgument, '1')
      Left: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: SimpleArgument, '1')
  Initializer: 
    IArrayInitializer (2 elements) ([2] OperationKind.ArrayInitializer, IsInvalid) (Syntax: CollectionInitializer, '{{1, 2}, {}}')
      Element Values(2):
        IArrayInitializer (2 elements) ([0] OperationKind.ArrayInitializer) (Syntax: CollectionInitializer, '{1, 2}')
          Element Values(2):
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
            ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
        IArrayInitializer (0 elements) ([1] OperationKind.ArrayInitializer, IsInvalid) (Syntax: CollectionInitializer, '{}')
          Element Values(0)
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30567: Array initializer is missing 2 elements.
        Dim y = New Integer(1, 1) {{1, 2}, {}}'BIND:"New Integer(1, 1) {{1, 2}, {}}"
                                           ~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of ArrayCreationExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")>
        Public Sub ArrayCreationOfMultiDimensionalArrays_InitializerWithNestedArrayInitializers()
            Dim source = <![CDATA[
Class C
    Public Sub F()
        Dim y = New Integer(1, 1) {{1, 2}, {1, 2}}'BIND:"New Integer(1, 1) {{1, 2}, {1, 2}}"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.Int32(,)) (Syntax: ArrayCreationExpression, 'New Integer ... 2}, {1, 2}}') (Parent: VariableInitializer)
  Dimension Sizes(2):
    IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32, Constant: 2, IsImplicit) (Syntax: SimpleArgument, '1')
      Left: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: SimpleArgument, '1')
    IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32, Constant: 2, IsImplicit) (Syntax: SimpleArgument, '1')
      Left: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: SimpleArgument, '1')
  Initializer: 
    IArrayInitializer (2 elements) ([2] OperationKind.ArrayInitializer) (Syntax: CollectionInitializer, '{{1, 2}, {1, 2}}')
      Element Values(2):
        IArrayInitializer (2 elements) ([0] OperationKind.ArrayInitializer) (Syntax: CollectionInitializer, '{1, 2}')
          Element Values(2):
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
            ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
        IArrayInitializer (2 elements) ([1] OperationKind.ArrayInitializer) (Syntax: CollectionInitializer, '{1, 2}')
          Element Values(2):
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
            ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of ArrayCreationExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")>
        Public Sub ArrayCreationOfImplicitlyTypedMultiDimensionalArrays_WithInitializer()
            Dim source = <![CDATA[
Class C
    Public Sub F()
        Dim a = {{{{1, 2}}}, {{{3, 4}}}}'BIND:"{{{{1, 2}}}, {{{3, 4}}}}"
    End Sub
End Class
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.Int32(,,,)) (Syntax: CollectionInitializer, '{{{{1, 2}}}, {{{3, 4}}}}') (Parent: VariableInitializer)
  Dimension Sizes(4):
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2, IsImplicit) (Syntax: CollectionInitializer, '{{{{1, 2}}}, {{{3, 4}}}}')
    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: CollectionInitializer, '{{{{1, 2}}}, {{{3, 4}}}}')
    ILiteralExpression ([2] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: CollectionInitializer, '{{{{1, 2}}}, {{{3, 4}}}}')
    ILiteralExpression ([3] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2, IsImplicit) (Syntax: CollectionInitializer, '{{{{1, 2}}}, {{{3, 4}}}}')
  Initializer: 
    IArrayInitializer (2 elements) ([4] OperationKind.ArrayInitializer, IsImplicit) (Syntax: CollectionInitializer, '{{{{1, 2}}}, {{{3, 4}}}}')
      Element Values(2):
        IArrayInitializer (1 elements) ([0] OperationKind.ArrayInitializer) (Syntax: CollectionInitializer, '{{{1, 2}}}')
          Element Values(1):
            IArrayInitializer (1 elements) ([0] OperationKind.ArrayInitializer) (Syntax: CollectionInitializer, '{{1, 2}}')
              Element Values(1):
                IArrayInitializer (2 elements) ([0] OperationKind.ArrayInitializer) (Syntax: CollectionInitializer, '{1, 2}')
                  Element Values(2):
                    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
                    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
        IArrayInitializer (1 elements) ([1] OperationKind.ArrayInitializer) (Syntax: CollectionInitializer, '{{{3, 4}}}')
          Element Values(1):
            IArrayInitializer (1 elements) ([0] OperationKind.ArrayInitializer) (Syntax: CollectionInitializer, '{{3, 4}}')
              Element Values(1):
                IArrayInitializer (2 elements) ([0] OperationKind.ArrayInitializer) (Syntax: CollectionInitializer, '{3, 4}')
                  Element Values(2):
                    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 3) (Syntax: NumericLiteralExpression, '3')
                    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 4) (Syntax: NumericLiteralExpression, '4')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of CollectionInitializerSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")>
        Public Sub ArrayCreationErrorCase_MissingDimension()
            Dim source = <![CDATA[
Class C
    Public Sub F()
        Dim a = New String(1,) {}'BIND:"New String(1,) {}"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.String(,), IsInvalid) (Syntax: ArrayCreationExpression, 'New String(1,) {}') (Parent: VariableInitializer)
  Dimension Sizes(2):
    IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32, Constant: 2, IsImplicit) (Syntax: SimpleArgument, '1')
      Left: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: SimpleArgument, '1')
    IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32, IsInvalid, IsImplicit) (Syntax: OmittedArgument, '')
      Left: 
        IInvalidExpression ([0] OperationKind.InvalidExpression, Type: ?, IsInvalid) (Syntax: OmittedArgument, '')
          Children(0)
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsInvalid, IsImplicit) (Syntax: OmittedArgument, '')
  Initializer: 
    IArrayInitializer (0 elements) ([2] OperationKind.ArrayInitializer) (Syntax: CollectionInitializer, '{}')
      Element Values(0)
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30306: Array subscript expression missing.
        Dim a = New String(1,) {}'BIND:"New String(1,) {}"
                             ~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of ArrayCreationExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")>
        Public Sub ArrayCreationErrorCase_InvalidInitializer()
            Dim source = <![CDATA[
Class C
    Public Sub F()
        Dim a = New C() {1}'BIND:"New C() {1}"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: C(), IsInvalid) (Syntax: ArrayCreationExpression, 'New C() {1}') (Parent: VariableInitializer)
  Dimension Sizes(1):
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsInvalid, IsImplicit) (Syntax: ArrayCreationExpression, 'New C() {1}')
  Initializer: 
    IArrayInitializer (1 elements) ([1] OperationKind.ArrayInitializer, IsInvalid) (Syntax: CollectionInitializer, '{1}')
      Element Values(1):
        IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: C, IsInvalid, IsImplicit) (Syntax: NumericLiteralExpression, '1')
          Conversion: CommonConversion (Exists: False, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsInvalid) (Syntax: NumericLiteralExpression, '1')
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30311: Value of type 'Integer' cannot be converted to 'C'.
        Dim a = New C() {1}'BIND:"New C() {1}"
                         ~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of ArrayCreationExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")>
        Public Sub ArrayCreationErrorCase_MissingExplicitCast()
            Dim source = <![CDATA[
Class C
    Public Sub F(c As C)
        Dim a = New C(c) {}'BIND:"New C(c) {}"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: C(), IsInvalid) (Syntax: ArrayCreationExpression, 'New C(c) {}') (Parent: VariableInitializer)
  Dimension Sizes(1):
    IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32, IsInvalid, IsImplicit) (Syntax: SimpleArgument, 'c')
      Left: 
        IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'c')
          Conversion: CommonConversion (Exists: False, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            IParameterReferenceExpression: c ([0] OperationKind.ParameterReferenceExpression, Type: C, IsInvalid) (Syntax: IdentifierName, 'c')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsInvalid, IsImplicit) (Syntax: SimpleArgument, 'c')
  Initializer: 
    IArrayInitializer (0 elements) ([1] OperationKind.ArrayInitializer) (Syntax: CollectionInitializer, '{}')
      Element Values(0)
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30311: Value of type 'C' cannot be converted to 'Integer'.
        Dim a = New C(c) {}'BIND:"New C(c) {}"
                      ~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of ArrayCreationExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")>
        Public Sub ArrayCreation_InvocationExpressionAsDimension()
            Dim source = <![CDATA[
Class C
    Public Sub F(c As C)
        Dim a = New C(M()) {}'BIND:"New C(M()) {}"
    End Sub

    Public Function M() As Integer
        Return 1
    End Function
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: C()) (Syntax: ArrayCreationExpression, 'New C(M()) {}') (Parent: VariableInitializer)
  Dimension Sizes(1):
    IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32, IsImplicit) (Syntax: SimpleArgument, 'M()')
      Left: 
        IInvocationExpression ( Function C.M() As System.Int32) ([0] OperationKind.InvocationExpression, Type: System.Int32) (Syntax: InvocationExpression, 'M()')
          Instance Receiver: 
            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C, IsImplicit) (Syntax: IdentifierName, 'M')
          Arguments(0)
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: SimpleArgument, 'M()')
  Initializer: 
    IArrayInitializer (0 elements) ([1] OperationKind.ArrayInitializer) (Syntax: CollectionInitializer, '{}')
      Element Values(0)
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of ArrayCreationExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")>
        Public Sub ArrayCreation_InvocationExpressionWithConversionAsDimension()
            Dim source = <![CDATA[
Option Strict On
Class C
    Public Sub F(c As C)
        Dim a = New C(DirectCast(M(), Integer)) {}'BIND:"New C(DirectCast(M(), Integer)) {}"
    End Sub

    Public Function M() As Object
        Return 1
    End Function
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: C()) (Syntax: ArrayCreationExpression, 'New C(Direc ... nteger)) {}') (Parent: VariableInitializer)
  Dimension Sizes(1):
    IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32, IsImplicit) (Syntax: SimpleArgument, 'DirectCast(M(), Integer)')
      Left: 
        IConversionExpression (Explicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32) (Syntax: DirectCastExpression, 'DirectCast(M(), Integer)')
          Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            IInvocationExpression ( Function C.M() As System.Object) ([0] OperationKind.InvocationExpression, Type: System.Object) (Syntax: InvocationExpression, 'M()')
              Instance Receiver: 
                IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C, IsImplicit) (Syntax: IdentifierName, 'M')
              Arguments(0)
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: SimpleArgument, 'DirectCast(M(), Integer)')
  Initializer: 
    IArrayInitializer (0 elements) ([1] OperationKind.ArrayInitializer) (Syntax: CollectionInitializer, '{}')
      Element Values(0)
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of ArrayCreationExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")>
        Public Sub ArrayCreationErrorCase_InvocationExpressionAsDimension()
            Dim source = <![CDATA[
Option Strict On
Class C
    Public Sub F(c As C)
        Dim a = New C(M()) {}'BIND:"New C(M()) {}"
    End Sub

    Public Function M() As Object
        Return 1
    End Function
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: C(), IsInvalid) (Syntax: ArrayCreationExpression, 'New C(M()) {}') (Parent: VariableInitializer)
  Dimension Sizes(1):
    IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32, IsInvalid, IsImplicit) (Syntax: SimpleArgument, 'M()')
      Left: 
        IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32, IsInvalid, IsImplicit) (Syntax: InvocationExpression, 'M()')
          Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            IInvocationExpression ( Function C.M() As System.Object) ([0] OperationKind.InvocationExpression, Type: System.Object, IsInvalid) (Syntax: InvocationExpression, 'M()')
              Instance Receiver: 
                IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'M')
              Arguments(0)
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsInvalid, IsImplicit) (Syntax: SimpleArgument, 'M()')
  Initializer: 
    IArrayInitializer (0 elements) ([1] OperationKind.ArrayInitializer) (Syntax: CollectionInitializer, '{}')
      Element Values(0)
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30512: Option Strict On disallows implicit conversions from 'Object' to 'Integer'.
        Dim a = New C(M()) {}'BIND:"New C(M()) {}"
                      ~~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of ArrayCreationExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")>
        Public Sub ArrayCreationErrorCase_InvocationExpressionWithConversionAsDimension()
            Dim source = <![CDATA[
Option Strict On
Class C
    Public Sub F(c As C)
        Dim a = New C(DirectCast(M(), Integer)) {}'BIND:"New C(DirectCast(M(), Integer)) {}"
    End Sub

    Public Function M() As C
        Return New C
    End Function
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: C(), IsInvalid) (Syntax: ArrayCreationExpression, 'New C(Direc ... nteger)) {}') (Parent: VariableInitializer)
  Dimension Sizes(1):
    IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32, IsInvalid, IsImplicit) (Syntax: SimpleArgument, 'DirectCast(M(), Integer)')
      Left: 
        IConversionExpression (Explicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32, IsInvalid) (Syntax: DirectCastExpression, 'DirectCast(M(), Integer)')
          Conversion: CommonConversion (Exists: False, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            IInvocationExpression ( Function C.M() As C) ([0] OperationKind.InvocationExpression, Type: C, IsInvalid) (Syntax: InvocationExpression, 'M()')
              Instance Receiver: 
                IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'M')
              Arguments(0)
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsInvalid, IsImplicit) (Syntax: SimpleArgument, 'DirectCast(M(), Integer)')
  Initializer: 
    IArrayInitializer (0 elements) ([1] OperationKind.ArrayInitializer) (Syntax: CollectionInitializer, '{}')
      Element Values(0)
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30311: Value of type 'C' cannot be converted to 'Integer'.
        Dim a = New C(DirectCast(M(), Integer)) {}'BIND:"New C(DirectCast(M(), Integer)) {}"
                                 ~~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of ArrayCreationExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <Fact, WorkItem(17596, "https://github.com/dotnet/roslyn/issues/17596")>
        Public Sub ArrayCreation_DeclarationWithExplicitDimension()
            Dim source = <![CDATA[
Class C
    Public Sub F()
        Dim x(2) As Integer'BIND:"Dim x(2) As Integer"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Dim x(2) As Integer') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'x(2)')
    Variables: Local_1: x As System.Int32()
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer, IsImplicit) (Syntax: ModifiedIdentifier, 'x(2)')
        IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.Int32()) (Syntax: ModifiedIdentifier, 'x(2)')
          Dimension Sizes(1):
            IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32, Constant: 3, IsImplicit) (Syntax: SimpleArgument, '2')
              Left: 
                ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
              Right: 
                ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: SimpleArgument, '2')
          Initializer: 
            null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <Fact, WorkItem(7299, "https://github.com/dotnet/roslyn/issues/7299")>
        Public Sub SimpleArrayCreation_ConstantConversion()
            Dim source = <![CDATA[
Option Strict On
Class C
    Public Sub F()
        Dim a = New String(0.0) {}'BIND:"New String(0.0) {}"
    End Sub
End Class
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: System.String(), IsInvalid) (Syntax: ArrayCreationExpression, 'New String(0.0) {}') (Parent: VariableInitializer)
  Dimension Sizes(1):
    IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32, Constant: 1, IsInvalid, IsImplicit) (Syntax: SimpleArgument, '0.0')
      Left: 
        IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32, Constant: 0, IsInvalid, IsImplicit) (Syntax: NumericLiteralExpression, '0.0')
          Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Double, Constant: 0, IsInvalid) (Syntax: NumericLiteralExpression, '0.0')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsInvalid, IsImplicit) (Syntax: SimpleArgument, '0.0')
  Initializer: 
    IArrayInitializer (0 elements) ([1] OperationKind.ArrayInitializer) (Syntax: CollectionInitializer, '{}')
      Element Values(0)
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30512: Option Strict On disallows implicit conversions from 'Double' to 'Integer'.
        Dim a = New String(0.0) {}'BIND:"New String(0.0) {}"
                           ~~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of ArrayCreationExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub
    End Class
End Namespace
