' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports Microsoft.CodeAnalysis.Semantics
Imports Microsoft.CodeAnalysis.Test.Utilities
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Test.Utilities

Namespace Microsoft.CodeAnalysis.VisualBasic.UnitTests.Semantics

    Partial Public Class IOperationTests
        Inherits SemanticModelTestBase

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(17598, "https://github.com/dotnet/roslyn/issues/17598")>
        Public Sub InvalidInvocationExpression_BadReceiver()
            Dim source = <![CDATA[
Imports System

Class Program
    Public Shared Sub Main(args As String())
        Console.WriteLine2()'BIND:"Console.WriteLine2()"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IInvalidExpression ([0] OperationKind.InvalidExpression, Type: ?, IsInvalid) (Syntax: InvocationExpression, 'Console.WriteLine2()') (Parent: ExpressionStatement)
  Children(1):
    IInvalidExpression ([0] OperationKind.InvalidExpression, Type: ?, IsInvalid) (Syntax: SimpleMemberAccessExpression, 'Console.WriteLine2')
      Children(1):
        IOperation:  ([0] OperationKind.None, IsInvalid) (Syntax: IdentifierName, 'Console')
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30456: 'WriteLine2' is not a member of 'Console'.
        Console.WriteLine2()'BIND:"Console.WriteLine2()"
        ~~~~~~~~~~~~~~~~~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of InvocationExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(17598, "https://github.com/dotnet/roslyn/issues/17598")>
        Public Sub InvalidInvocationExpression_OverloadResolutionFailureBadArgument()
            Dim source = <![CDATA[
Class Program
    Public Shared Sub Main(args As String())
        F(String.Empty)'BIND:"F(String.Empty)"
    End Sub

    Private Sub F(x As Integer)
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IInvocationExpression ( Sub Program.F(x As System.Int32)) ([0] OperationKind.InvocationExpression, Type: System.Void, IsInvalid) (Syntax: InvocationExpression, 'F(String.Empty)') (Parent: ExpressionStatement)
  Instance Receiver: 
    IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: Program, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'F')
  Arguments(1):
    IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([1] OperationKind.Argument) (Syntax: SimpleArgument, 'String.Empty')
      IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: SimpleMemberAccessExpression, 'String.Empty')
        Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
        Operand: 
          IFieldReferenceExpression: System.String.Empty As System.String (Static) ([0] OperationKind.FieldReferenceExpression, Type: System.String) (Syntax: SimpleMemberAccessExpression, 'String.Empty')
            Instance Receiver: 
              null
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30369: Cannot refer to an instance member of a class from within a shared method or shared member initializer without an explicit instance of the class.
        F(String.Empty)'BIND:"F(String.Empty)"
        ~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of InvocationExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(17598, "https://github.com/dotnet/roslyn/issues/17598")>
        Public Sub InvalidInvocationExpression_OverloadResolutionFailureExtraArgument()
            Dim source = <![CDATA[
Class Program
    Public Shared Sub Main(args As String())
        F(String.Empty)'BIND:"F(String.Empty)"
    End Sub

    Private Sub F()
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IInvalidExpression ([0] OperationKind.InvalidExpression, Type: System.Void, IsInvalid) (Syntax: InvocationExpression, 'F(String.Empty)') (Parent: ExpressionStatement)
  Children(2):
    IOperation:  ([0] OperationKind.None) (Syntax: IdentifierName, 'F')
      Children(1):
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: Program, IsImplicit) (Syntax: IdentifierName, 'F')
    IFieldReferenceExpression: System.String.Empty As System.String (Static) ([1] OperationKind.FieldReferenceExpression, Type: System.String, IsInvalid) (Syntax: SimpleMemberAccessExpression, 'String.Empty')
      Instance Receiver: 
        null
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30057: Too many arguments to 'Private Sub F()'.
        F(String.Empty)'BIND:"F(String.Empty)"
          ~~~~~~~~~~~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of InvocationExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(17598, "https://github.com/dotnet/roslyn/issues/17598")>
        Public Sub InvalidFieldReferenceExpression()
            Dim source = <![CDATA[
Class Program
    Public Shared Sub Main(args As String())
        Dim x = New Program()
        Dim y = x.MissingField'BIND:"x.MissingField"
    End Sub

    Private Sub F()
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IInvalidExpression ([0] OperationKind.InvalidExpression, Type: ?, IsInvalid) (Syntax: SimpleMemberAccessExpression, 'x.MissingField') (Parent: VariableInitializer)
  Children(1):
    ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: Program, IsInvalid) (Syntax: IdentifierName, 'x')
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30456: 'MissingField' is not a member of 'Program'.
        Dim y = x.MissingField'BIND:"x.MissingField"
                ~~~~~~~~~~~~~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of MemberAccessExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(17598, "https://github.com/dotnet/roslyn/issues/17598")>
        Public Sub InvalidConversionExpression_ImplicitCast()
            Dim source = <![CDATA[
Class Program
    Private i1 As Integer
    Public Shared Sub Main(args As String())
        Dim x = New Program()
        Dim y As Program = x.i1'BIND:"x.i1"
    End Sub

    Private Sub F()
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IFieldReferenceExpression: Program.i1 As System.Int32 ([0] OperationKind.FieldReferenceExpression, Type: System.Int32, IsInvalid) (Syntax: SimpleMemberAccessExpression, 'x.i1') (Parent: ConversionExpression)
  Instance Receiver: 
    ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: Program, IsInvalid) (Syntax: IdentifierName, 'x')
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30311: Value of type 'Integer' cannot be converted to 'Program'.
        Dim y As Program = x.i1'BIND:"x.i1"
                           ~~~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of MemberAccessExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(17598, "https://github.com/dotnet/roslyn/issues/17598")>
        Public Sub InvalidConversionExpression_ExplicitCast()
            Dim source = <![CDATA[
Class Program
    Private i1 As Integer
    Public Shared Sub Main(args As String())
        Dim x = New Program()
        Dim y As Program = DirectCast(x.i1, Program)'BIND:"DirectCast(x.i1, Program)"
    End Sub

    Private Sub F()
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IConversionExpression (Explicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: Program, IsInvalid) (Syntax: DirectCastExpression, 'DirectCast( ... 1, Program)') (Parent: VariableInitializer)
  Conversion: CommonConversion (Exists: False, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  Operand: 
    IFieldReferenceExpression: Program.i1 As System.Int32 ([0] OperationKind.FieldReferenceExpression, Type: System.Int32, IsInvalid) (Syntax: SimpleMemberAccessExpression, 'x.i1')
      Instance Receiver: 
        ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: Program, IsInvalid) (Syntax: IdentifierName, 'x')
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30311: Value of type 'Integer' cannot be converted to 'Program'.
        Dim y As Program = DirectCast(x.i1, Program)'BIND:"DirectCast(x.i1, Program)"
                                      ~~~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of DirectCastExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(17598, "https://github.com/dotnet/roslyn/issues/17598")>
        Public Sub InvalidUnaryExpression()
            Dim source = <![CDATA[
Imports System

Class Program
    Public Shared Sub Main(args As String())
        Dim x = New Program()
        Console.Write(+x)'BIND:"+x"
    End Sub

    Private Sub F()
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Plus, Checked) ([1] OperationKind.UnaryOperatorExpression, Type: ?, IsInvalid) (Syntax: UnaryPlusExpression, '+x') (Parent: InvalidExpression)
  Operand: 
    ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: Program, IsInvalid) (Syntax: IdentifierName, 'x')
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30487: Operator '+' is not defined for type 'Program'.
        Console.Write(+x)'BIND:"+x"
                      ~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(17598, "https://github.com/dotnet/roslyn/issues/17598")>
        Public Sub InvalidBinaryExpression()
            Dim source = <![CDATA[
Imports System

Class Program
    Public Shared Sub Main(args As String())
        Dim x = New Program()
        Console.Write(x + (y * args.Length))'BIND:"x + (y * args.Length)"
    End Sub

    Private Sub F()
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([1] OperationKind.BinaryOperatorExpression, Type: ?, IsInvalid) (Syntax: AddExpression, 'x + (y * args.Length)') (Parent: InvalidExpression)
  Left: 
    ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: Program) (Syntax: IdentifierName, 'x')
  Right: 
    IParenthesizedExpression ([1] OperationKind.ParenthesizedExpression, Type: ?, IsInvalid) (Syntax: ParenthesizedExpression, '(y * args.Length)')
      Operand: 
        IBinaryOperatorExpression (BinaryOperatorKind.Multiply, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: ?, IsInvalid) (Syntax: MultiplyExpression, 'y * args.Length')
          Left: 
            IInvalidExpression ([0] OperationKind.InvalidExpression, Type: ?, IsInvalid) (Syntax: IdentifierName, 'y')
              Children(0)
          Right: 
            IPropertyReferenceExpression: ReadOnly Property System.Array.Length As System.Int32 ([1] OperationKind.PropertyReferenceExpression, Type: System.Int32) (Syntax: SimpleMemberAccessExpression, 'args.Length')
              Instance Receiver: 
                IParameterReferenceExpression: args ([0] OperationKind.ParameterReferenceExpression, Type: System.String()) (Syntax: IdentifierName, 'args')
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30451: 'y' is not declared. It may be inaccessible due to its protection level.
        Console.Write(x + (y * args.Length))'BIND:"x + (y * args.Length)"
                           ~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of BinaryExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(17598, "https://github.com/dotnet/roslyn/issues/17598")>
        Public Sub InvalidLambdaBinding_UnboundLambda()
            Dim source = <![CDATA[
Imports System

Class Program
    Public Shared Sub Main(args As String())
        Dim x = Function() F()'BIND:"Function() F()"
    End Sub

    Private Shared Sub F()
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IAnonymousFunctionExpression (Symbol: Function () As ?) ([0] OperationKind.AnonymousFunctionExpression, Type: null, IsInvalid) (Syntax: SingleLineFunctionLambdaExpression, 'Function() F()') (Parent: DelegateCreationExpression)
  IBlockStatement (3 statements, 1 locals) ([0] OperationKind.BlockStatement, IsInvalid, IsImplicit) (Syntax: SingleLineFunctionLambdaExpression, 'Function() F()')
    Locals: Local_1: <anonymous local> As ?
    IReturnStatement ([0] OperationKind.ReturnStatement, IsInvalid, IsImplicit) (Syntax: InvocationExpression, 'F()')
      ReturnedValue: 
        IInvalidExpression ([0] OperationKind.InvalidExpression, Type: ?, IsInvalid, IsImplicit) (Syntax: InvocationExpression, 'F()')
          Children(1):
            IInvocationExpression (Sub Program.F()) ([0] OperationKind.InvocationExpression, Type: System.Void, IsInvalid) (Syntax: InvocationExpression, 'F()')
              Instance Receiver: 
                null
              Arguments(0)
    ILabeledStatement (Label: exit) ([1] OperationKind.LabeledStatement, IsInvalid, IsImplicit) (Syntax: SingleLineFunctionLambdaExpression, 'Function() F()')
      Statement: 
        null
    IReturnStatement ([2] OperationKind.ReturnStatement, IsInvalid, IsImplicit) (Syntax: SingleLineFunctionLambdaExpression, 'Function() F()')
      ReturnedValue: 
        ILocalReferenceExpression:  ([0] OperationKind.LocalReferenceExpression, Type: ?, IsInvalid, IsImplicit) (Syntax: SingleLineFunctionLambdaExpression, 'Function() F()')
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30491: Expression does not produce a value.
        Dim x = Function() F()'BIND:"Function() F()"
                           ~~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of SingleLineLambdaExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(17598, "https://github.com/dotnet/roslyn/issues/17598")>
        Public Sub InvalidFieldInitializer()
            Dim source = <![CDATA[
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading.Tasks

Class Program
    Private x As Integer = Program'BIND:"= Program"
    Public Shared Sub Main(args As String())
        Dim x = New Program() With {
            .x = Program
        }
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IFieldInitializer (Field: Program.x As System.Int32) ([Root] OperationKind.FieldInitializer, IsInvalid) (Syntax: EqualsValue, '= Program') (Parent: )
  IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'Program')
    Conversion: CommonConversion (Exists: False, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    Operand: 
      IOperation:  ([0] OperationKind.None, IsInvalid) (Syntax: IdentifierName, 'Program')
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30109: 'Program' is a class type and cannot be used as an expression.
    Private x As Integer = Program'BIND:"= Program"
                           ~~~~~~~
BC30109: 'Program' is a class type and cannot be used as an expression.
            .x = Program
                 ~~~~~~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of EqualsValueSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(Skip:="https://github.com/dotnet/roslyn/issues/18074"), WorkItem(17598, "https://github.com/dotnet/roslyn/issues/17598")>
        Public Sub InvalidArrayInitializer()
            Dim source = <![CDATA[
Class Program
    Public Shared Sub Main(args As String())
        Dim x = New Integer(1, 1) {{{1, 1}}, {2, 2}}'BIND:"{{1, 1}}"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IArrayInitializer (1 elements) (OperationKind.ArrayInitializer, IsInvalid) (Syntax: '{{1, 1}}')
  Element Values(1): IInvalidExpression (OperationKind.InvalidExpression, Type: ?, IsInvalid) (Syntax: '{1, 1}')
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30567: Array initializer is missing 1 elements.
        Dim x = New Integer(1, 1) {{{1, 1}}, {2, 2}}'BIND:"{{1, 1}}"
                                   ~~~~~~~~
BC30566: Array initializer has too many dimensions.
        Dim x = New Integer(1, 1) {{{1, 1}}, {2, 2}}'BIND:"{{1, 1}}"
                                    ~~~~~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of CollectionInitializerSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(17598, "https://github.com/dotnet/roslyn/issues/17598")>
        Public Sub InvalidArrayCreation()
            Dim source = <![CDATA[
Class Program
    Public Shared Sub Main(args As String())
        Dim x = New X(Program - 1) {{1}}'BIND:"New X(Program - 1) {{1}}"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: X(), IsInvalid) (Syntax: ArrayCreationExpression, 'New X(Program - 1) {{1}}') (Parent: VariableInitializer)
  Dimension Sizes(1):
    IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32, IsInvalid, IsImplicit) (Syntax: SimpleArgument, 'Program - 1')
      Left: 
        IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32, IsInvalid, IsImplicit) (Syntax: SubtractExpression, 'Program - 1')
          Conversion: CommonConversion (Exists: False, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            IBinaryOperatorExpression (BinaryOperatorKind.Subtract, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: ?, IsInvalid) (Syntax: SubtractExpression, 'Program - 1')
              Left: 
                IOperation:  ([0] OperationKind.None, IsInvalid) (Syntax: IdentifierName, 'Program')
              Right: 
                ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsInvalid, IsImplicit) (Syntax: SimpleArgument, 'Program - 1')
  Initializer: 
    IArrayInitializer (1 elements) ([1] OperationKind.ArrayInitializer, IsInvalid) (Syntax: CollectionInitializer, '{{1}}')
      Element Values(1):
        IInvalidExpression ([0] OperationKind.InvalidExpression, Type: ?, IsInvalid) (Syntax: CollectionInitializer, '{1}')
          Children(0)
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30002: Type 'X' is not defined.
        Dim x = New X(Program - 1) {{1}}'BIND:"New X(Program - 1) {{1}}"
                    ~
BC30109: 'Program' is a class type and cannot be used as an expression.
        Dim x = New X(Program - 1) {{1}}'BIND:"New X(Program - 1) {{1}}"
                      ~~~~~~~
BC30949: Array initializer cannot be specified for a non constant dimension; use the empty initializer '{}'.
        Dim x = New X(Program - 1) {{1}}'BIND:"New X(Program - 1) {{1}}"
                                   ~~~~~
BC30566: Array initializer has too many dimensions.
        Dim x = New X(Program - 1) {{1}}'BIND:"New X(Program - 1) {{1}}"
                                    ~~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of ArrayCreationExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(17598, "https://github.com/dotnet/roslyn/issues/17598")>
        Public Sub InvalidParameterDefaultValueInitializer()
            Dim source = <![CDATA[
Class Program
    Private Shared Function M() As Integer
        Return 0
    End Function
    Private Sub F(Optional p As Integer = M())'BIND:"= M()"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IParameterInitializer (Parameter: [p As System.Int32]) ([Root] OperationKind.ParameterInitializer, IsInvalid) (Syntax: EqualsValue, '= M()') (Parent: )
  IInvalidExpression ([0] OperationKind.InvalidExpression, Type: System.Int32, IsInvalid, IsImplicit) (Syntax: InvocationExpression, 'M()')
    Children(1):
      IInvocationExpression (Function Program.M() As System.Int32) ([0] OperationKind.InvocationExpression, Type: System.Int32, IsInvalid) (Syntax: InvocationExpression, 'M()')
        Instance Receiver: 
          null
        Arguments(0)
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30059: Constant expression is required.
    Private Sub F(Optional p As Integer = M())'BIND:"= M()"
                                          ~~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of EqualsValueSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub
    End Class
End Namespace
