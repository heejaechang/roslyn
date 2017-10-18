﻿' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports Microsoft.CodeAnalysis.Semantics
Imports Microsoft.CodeAnalysis.Test.Utilities
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Test.Utilities

Namespace Microsoft.CodeAnalysis.VisualBasic.UnitTests.Semantics

    <CompilerTrait(CompilerFeature.IOperation)>
    Partial Public Class IOperationTests
        Inherits SemanticModelTestBase

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub InvalidUserDefinedOperators()
            Dim source = <compilation>
                             <file name="c.vb">
                                 <![CDATA[
Public Class B2
    Public Shared Operator +(x As B2, y As B2) As B2
        System.Console.WriteLine("+")
        Return x
    End Operator

    Public Shared Operator -(x As B2) As B2
        System.Console.WriteLine("-")
        Return x
    End Operator

    Public Shared Operator -(x As B2) As B2
        System.Console.WriteLine("-")
        Return x
    End Operator
End Class

Module Module1
    Sub Main()
        Dim x, y As New B2()
        x = x + 10
        x = x + y
        x = -x
    End Sub
End Module
]]>
                             </file>
                         </compilation>

            Dim comp = CompilationUtils.CreateCompilationWithMscorlibAndVBRuntime(source, parseOptions:=TestOptions.RegularWithIOperationFeature)
            Dim tree = comp.SyntaxTrees.Single()
            Dim model = comp.GetSemanticModel(tree)
            Dim nodes = tree.GetRoot().DescendantNodes().OfType(Of AssignmentStatementSyntax).ToArray()
            Assert.Equal(nodes.Length, 3)

            ' x = x + 10 fails semantic analysis and does not have an operator method, but the operands are available.

            Assert.Equal("x = x + 10", nodes(0).ToString())
            Dim statement1 As IOperation = model.GetOperation(nodes(0))
            Assert.Equal(statement1.Kind, OperationKind.ExpressionStatement)
            Dim expression1 As IOperation = DirectCast(statement1, IExpressionStatement).Expression
            Assert.Equal(expression1.Kind, OperationKind.SimpleAssignmentExpression)
            Dim assignment1 As ISimpleAssignmentExpression = DirectCast(expression1, ISimpleAssignmentExpression)
            Assert.Equal(assignment1.Value.Kind, OperationKind.BinaryOperatorExpression)
            Dim add1 As IBinaryOperatorExpression = DirectCast(assignment1.Value, IBinaryOperatorExpression)
            Assert.Equal(add1.OperatorKind, CodeAnalysis.Semantics.BinaryOperatorKind.Add)
            Assert.Null(add1.OperatorMethod)
            Dim left1 As IOperation = add1.LeftOperand
            Assert.Equal(left1.Kind, OperationKind.LocalReferenceExpression)
            Assert.Equal(DirectCast(left1, ILocalReferenceExpression).Local.Name, "x")
            Dim right1 As IOperation = add1.RightOperand
            Assert.Equal(right1.Kind, OperationKind.LiteralExpression)
            Dim literal1 As ILiteralExpression = DirectCast(right1, ILiteralExpression)
            Assert.Equal(CInt(literal1.ConstantValue.Value), 10)

            comp.VerifyOperationTree(nodes(0), expectedOperationTree:="
IExpressionStatement ([1] OperationKind.ExpressionStatement, IsInvalid) (Syntax: SimpleAssignmentStatement, 'x = x + 10') (Parent: BlockStatement)
  Expression: 
    ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: B2, IsInvalid) (Syntax: SimpleAssignmentStatement, 'x = x + 10')
      Left: 
        ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'x')
      Right: 
        IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([1] OperationKind.BinaryOperatorExpression, Type: B2, IsInvalid) (Syntax: AddExpression, 'x + 10')
          Left: 
            ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'x')
          Right: 
            ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10, IsInvalid) (Syntax: NumericLiteralExpression, '10')")

            ' x = x + y passes semantic analysis.

            Assert.Equal("x = x + y", nodes(1).ToString())
            Dim statement2 As IOperation = model.GetOperation(nodes(1))
            Assert.Equal(statement2.Kind, OperationKind.ExpressionStatement)
            Dim expression2 As IOperation = DirectCast(statement2, IExpressionStatement).Expression
            Assert.Equal(expression2.Kind, OperationKind.SimpleAssignmentExpression)
            Dim assignment2 As ISimpleAssignmentExpression = DirectCast(expression2, ISimpleAssignmentExpression)
            Assert.Equal(assignment2.Value.Kind, OperationKind.BinaryOperatorExpression)
            Dim add2 As IBinaryOperatorExpression = DirectCast(assignment2.Value, IBinaryOperatorExpression)
            Assert.Equal(add2.OperatorKind, CodeAnalysis.Semantics.BinaryOperatorKind.Add)
            Assert.NotNull(add2.OperatorMethod)
            Assert.Equal(add2.OperatorMethod.Name, "op_Addition")
            Dim left2 As IOperation = add2.LeftOperand
            Assert.Equal(left2.Kind, OperationKind.LocalReferenceExpression)
            Assert.Equal(DirectCast(left2, ILocalReferenceExpression).Local.Name, "x")
            Dim right2 As IOperation = add2.RightOperand
            Assert.Equal(right2.Kind, OperationKind.LocalReferenceExpression)
            Assert.Equal(DirectCast(right2, ILocalReferenceExpression).Local.Name, "y")

            comp.VerifyOperationTree(nodes(1), expectedOperationTree:="
IExpressionStatement ([2] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'x = x + y') (Parent: BlockStatement)
  Expression: 
    ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: B2) (Syntax: SimpleAssignmentStatement, 'x = x + y')
      Left: 
        ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'x')
      Right: 
        IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) (OperatorMethod: Function B2.op_Addition(x As B2, y As B2) As B2) ([1] OperationKind.BinaryOperatorExpression, Type: B2) (Syntax: AddExpression, 'x + y')
          Left: 
            ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'x')
          Right: 
            ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'y')")

            ' -x fails semantic analysis and does not have an operator method, but the operand is available.

            Assert.Equal("x = -x", nodes(2).ToString())
            Dim statement3 As IOperation = model.GetOperation(nodes(2))
            Assert.Equal(statement3.Kind, OperationKind.ExpressionStatement)
            Dim expression3 As IOperation = DirectCast(statement3, IExpressionStatement).Expression
            Assert.Equal(expression3.Kind, OperationKind.SimpleAssignmentExpression)
            Dim assignment3 As ISimpleAssignmentExpression = DirectCast(expression3, ISimpleAssignmentExpression)
            Assert.Equal(assignment3.Value.Kind, OperationKind.UnaryOperatorExpression)
            Dim negate3 As IUnaryOperatorExpression = DirectCast(assignment3.Value, IUnaryOperatorExpression)
            Assert.Equal(negate3.OperatorKind, CodeAnalysis.Semantics.UnaryOperatorKind.Minus)
            Assert.Null(negate3.OperatorMethod)
            Dim operand3 As IOperation = negate3.Operand
            Assert.Equal(operand3.Kind, OperationKind.LocalReferenceExpression)
            Assert.Equal(DirectCast(operand3, ILocalReferenceExpression).Local.Name, "x")

            comp.VerifyOperationTree(nodes(2), expectedOperationTree:="
IExpressionStatement ([3] OperationKind.ExpressionStatement, IsInvalid) (Syntax: SimpleAssignmentStatement, 'x = -x') (Parent: BlockStatement)
  Expression: 
    ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: B2, IsInvalid) (Syntax: SimpleAssignmentStatement, 'x = -x')
      Left: 
        ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'x')
      Right: 
        IUnaryOperatorExpression (UnaryOperatorKind.Minus) ([1] OperationKind.UnaryOperatorExpression, Type: B2, IsInvalid) (Syntax: UnaryMinusExpression, '-x')
          Operand: 
            ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: B2, IsInvalid) (Syntax: IdentifierName, 'x')")
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub SimpleCompoundAssignment()
            Dim source = <compilation>
                             <file name="c.vb">
                                 <![CDATA[
Public Class B2
    Public Shared Operator +(x As B2, y As B2) As B2
        System.Console.WriteLine("+")
        Return x
    End Operator
End Class

Module Module1
    Sub Main()
        Dim x, y As Integer
        Dim a, b As New B2()
        x += y
        a += b
    End Sub
End Module
]]>
                             </file>
                         </compilation>

            Dim comp = CompilationUtils.CreateCompilationWithMscorlibAndVBRuntime(source, parseOptions:=TestOptions.RegularWithIOperationFeature)
            Dim tree = comp.SyntaxTrees.Single()
            Dim model = comp.GetSemanticModel(tree)
            Dim nodes = tree.GetRoot().DescendantNodes().OfType(Of AssignmentStatementSyntax).ToArray()
            Assert.Equal(nodes.Length, 2)

            ' x += y produces a compound assignment with an integer add.

            Assert.Equal("x += y", nodes(0).ToString())
            Dim statement1 As IOperation = model.GetOperation(nodes(0))
            Assert.Equal(statement1.Kind, OperationKind.ExpressionStatement)
            Dim expression1 As IOperation = DirectCast(statement1, IExpressionStatement).Expression
            Assert.Equal(expression1.Kind, OperationKind.CompoundAssignmentExpression)
            Dim assignment1 As ICompoundAssignmentExpression = DirectCast(expression1, ICompoundAssignmentExpression)
            Dim target1 As ILocalReferenceExpression = TryCast(assignment1.Target, ILocalReferenceExpression)
            Assert.NotNull(target1)
            Assert.Equal(target1.Local.Name, "x")
            Dim value1 As ILocalReferenceExpression = TryCast(assignment1.Value, ILocalReferenceExpression)
            Assert.NotNull(value1)
            Assert.Equal(value1.Local.Name, "y")
            Assert.Equal(assignment1.OperatorKind, CodeAnalysis.Semantics.BinaryOperatorKind.Add)
            Assert.Null(assignment1.OperatorMethod)

            comp.VerifyOperationTree(nodes(0), expectedOperationTree:="
IExpressionStatement ([2] OperationKind.ExpressionStatement) (Syntax: AddAssignmentStatement, 'x += y') (Parent: BlockStatement)
  Expression: 
    ICompoundAssignmentExpression (BinaryOperatorKind.Add, Checked) ([0] OperationKind.CompoundAssignmentExpression, Type: System.Int32) (Syntax: AddAssignmentStatement, 'x += y')
      Left: 
        ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
      Right: 
        ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')")

            ' a += b produces a compound assignment with an operator method add.

            Assert.Equal("a += b", nodes(1).ToString())
            Dim statement2 As IOperation = model.GetOperation(nodes(1))
            Assert.Equal(statement2.Kind, OperationKind.ExpressionStatement)
            Dim expression2 As IOperation = DirectCast(statement2, IExpressionStatement).Expression
            Assert.Equal(expression2.Kind, OperationKind.CompoundAssignmentExpression)
            Dim assignment2 As ICompoundAssignmentExpression = DirectCast(expression2, ICompoundAssignmentExpression)
            Dim target2 As ILocalReferenceExpression = TryCast(assignment2.Target, ILocalReferenceExpression)
            Assert.NotNull(target2)
            Assert.Equal(target2.Local.Name, "a")
            Dim value2 As ILocalReferenceExpression = TryCast(assignment2.Value, ILocalReferenceExpression)
            Assert.NotNull(value2)
            Assert.Equal(value2.Local.Name, "b")
            Assert.Equal(assignment2.OperatorKind, CodeAnalysis.Semantics.BinaryOperatorKind.Add)
            Assert.NotNull(assignment2.OperatorMethod)
            Assert.Equal(assignment2.OperatorMethod.Name, "op_Addition")

            comp.VerifyOperationTree(nodes(1), expectedOperationTree:="
IExpressionStatement ([3] OperationKind.ExpressionStatement) (Syntax: AddAssignmentStatement, 'a += b') (Parent: BlockStatement)
  Expression: 
    ICompoundAssignmentExpression (BinaryOperatorKind.Add, Checked) (OperatorMethod: Function B2.op_Addition(x As B2, y As B2) As B2) ([0] OperationKind.CompoundAssignmentExpression, Type: B2) (Syntax: AddAssignmentStatement, 'a += b')
      Left: 
        ILocalReferenceExpression: a ([0] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'a')
      Right: 
        ILocalReferenceExpression: b ([1] OperationKind.LocalReferenceExpression, Type: B2) (Syntax: IdentifierName, 'b')")
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub VerifyOperationTree_IfStatement()
            Dim source = <![CDATA[
Class C
    Sub F(x As Integer)
        If x <> 0 Then'BIND:"If x <> 0 Then"
            System.Console.Write(x)
        End If
    End Sub
End Class
]]>.Value

            Dim expectedOperationTree = <![CDATA[
IIfStatement ([0] OperationKind.IfStatement) (Syntax: MultiLineIfBlock, 'If x <> 0 T ... End If') (Parent: BlockStatement)
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.NotEquals, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: NotEqualsExpression, 'x <> 0')
      Left: 
        IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  IfTrue: 
    IBlockStatement (1 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: MultiLineIfBlock, 'If x <> 0 T ... End If')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Console.Write(x)')
        Expression: 
          IInvocationExpression (Sub System.Console.Write(value As System.Int32)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Console.Write(x)')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, 'x')
                IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  IfFalse: 
    null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of MultiLineIfBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub VerifyOperationTree_ForStatement()
            Dim source = <![CDATA[
Class C
    Sub F()
        For i = 0 To 10'BIND:"For i = 0 To 10"
            System.Console.Write(i)
        Next
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IForToLoopStatement (LoopKind.ForTo) ([0] OperationKind.LoopStatement) (Syntax: ForBlock, 'For i = 0 T ... Next') (Parent: BlockStatement)
  Locals: Local_1: i As System.Int32
  LoopControlVariable: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
  InitialValue: 
    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  LimitValue: 
    ILiteralExpression ([2] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10) (Syntax: NumericLiteralExpression, '10')
  StepValue: 
    IConversionExpression (Explicit, TryCast: False, Unchecked) ([3] OperationKind.ConversionExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For i = 0 T ... Next')
      Conversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For i = 0 T ... Next')
  Body: 
    IBlockStatement (1 statements) ([4] OperationKind.BlockStatement, IsImplicit) (Syntax: ForBlock, 'For i = 0 T ... Next')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Console.Write(i)')
        Expression: 
          IInvocationExpression (Sub System.Console.Write(value As System.Int32)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Console.Write(i)')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, 'i')
                ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  NextVariables(0)
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of ForBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        <WorkItem(382240, "https://devdiv.visualstudio.com/DevDiv/_workitems?id=382240")>
        Public Sub NothingOrAddressOfInPlaceOfParamArray()
            Dim source = <compilation>
                             <file name="c.vb">
                                 <![CDATA[
Module Module1
    Sub Main()
        Test1(Nothing)
        Test2(New System.Guid(), Nothing)
        Test1(AddressOf Main)
        Test2(New System.Guid(), AddressOf Main)
    End Sub

    Sub Test1(ParamArray x as Integer())
    End Sub

    Sub Test2(y As Integer, ParamArray x as Integer())
    End Sub
End Module
]]>
                             </file>
                         </compilation>

            Dim comp = CompilationUtils.CreateCompilationWithMscorlibAndVBRuntime(source, parseOptions:=TestOptions.RegularWithIOperationFeature)
            Dim tree = comp.SyntaxTrees.Single()

            comp.AssertTheseDiagnostics(
<expected>
BC30311: Value of type 'Guid' cannot be converted to 'Integer'.
        Test2(New System.Guid(), Nothing)
              ~~~~~~~~~~~~~~~~~
BC30581: 'AddressOf' expression cannot be converted to 'Integer' because 'Integer' is not a delegate type.
        Test1(AddressOf Main)
              ~~~~~~~~~~~~~~
BC30311: Value of type 'Guid' cannot be converted to 'Integer'.
        Test2(New System.Guid(), AddressOf Main)
              ~~~~~~~~~~~~~~~~~
BC30581: 'AddressOf' expression cannot be converted to 'Integer' because 'Integer' is not a delegate type.
        Test2(New System.Guid(), AddressOf Main)
                                 ~~~~~~~~~~~~~~
</expected>)

            Dim nodes = tree.GetRoot().DescendantNodes().OfType(Of InvocationExpressionSyntax)().ToArray()

            comp.VerifyOperationTree(nodes(0), expectedOperationTree:=
"IInvocationExpression (Sub Module1.Test1(ParamArray x As System.Int32())) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Test1(Nothing)') (Parent: ExpressionStatement)
  Instance Receiver: 
    null
  Arguments(1):
    IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([0] OperationKind.Argument) (Syntax: SimpleArgument, 'Nothing')
      IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32(), Constant: null, IsImplicit) (Syntax: NothingLiteralExpression, 'Nothing')
        Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
        Operand: 
          ILiteralExpression ([0] OperationKind.LiteralExpression, Type: null, Constant: null) (Syntax: NothingLiteralExpression, 'Nothing')
      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)")

            comp.VerifyOperationTree(nodes(1), expectedOperationTree:=
"IInvalidExpression ([0] OperationKind.InvalidExpression, Type: System.Void, IsInvalid) (Syntax: InvocationExpression, 'Test2(New S ... ), Nothing)') (Parent: ExpressionStatement)
  Children(3):
    IOperation:  ([0] OperationKind.None) (Syntax: IdentifierName, 'Test2')
      Children(1):
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: Module1, IsImplicit) (Syntax: IdentifierName, 'Test2')
    IObjectCreationExpression (Constructor: Sub System.Guid..ctor()) ([1] OperationKind.ObjectCreationExpression, Type: System.Guid, IsInvalid) (Syntax: ObjectCreationExpression, 'New System.Guid()')
      Arguments(0)
      Initializer: 
        null
    ILiteralExpression ([2] OperationKind.LiteralExpression, Type: null, Constant: null) (Syntax: NothingLiteralExpression, 'Nothing')")

            comp.VerifyOperationTree(nodes(2), expectedOperationTree:=
"IInvalidExpression ([0] OperationKind.InvalidExpression, Type: System.Void, IsInvalid) (Syntax: InvocationExpression, 'Test1(AddressOf Main)') (Parent: ExpressionStatement)
  Children(2):
    IOperation:  ([0] OperationKind.None) (Syntax: IdentifierName, 'Test1')
      Children(1):
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: Module1, IsImplicit) (Syntax: IdentifierName, 'Test1')
    IOperation:  ([1] OperationKind.None, IsInvalid) (Syntax: AddressOfExpression, 'AddressOf Main')
      Children(1):
        IOperation:  ([0] OperationKind.None, IsInvalid) (Syntax: IdentifierName, 'Main')
          Children(1):
            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: Module1, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'Main')")

            comp.VerifyOperationTree(nodes(3), expectedOperationTree:=
"IInvalidExpression ([0] OperationKind.InvalidExpression, Type: System.Void, IsInvalid) (Syntax: InvocationExpression, 'Test2(New S ... essOf Main)') (Parent: ExpressionStatement)
  Children(3):
    IOperation:  ([0] OperationKind.None) (Syntax: IdentifierName, 'Test2')
      Children(1):
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: Module1, IsImplicit) (Syntax: IdentifierName, 'Test2')
    IObjectCreationExpression (Constructor: Sub System.Guid..ctor()) ([1] OperationKind.ObjectCreationExpression, Type: System.Guid, IsInvalid) (Syntax: ObjectCreationExpression, 'New System.Guid()')
      Arguments(0)
      Initializer: 
        null
    IOperation:  ([2] OperationKind.None, IsInvalid) (Syntax: AddressOfExpression, 'AddressOf Main')
      Children(1):
        IOperation:  ([0] OperationKind.None, IsInvalid) (Syntax: IdentifierName, 'Main')
          Children(1):
            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: Module1, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'Main')")
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub BoundMethodGroup_ExposesReceiver()
            Dim source = <![CDATA[
Imports System
Module Program
    Sub Main(args As String())
        Dim c1 As New C1
        Console.WriteLine(New With {Key .a = AddressOf c1.S})'BIND:"New With {Key .a = AddressOf c1.S}"
    End Sub

    Class C1
        Sub S()
        End Sub
    End Class
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IAnonymousObjectCreationExpression ([1] OperationKind.AnonymousObjectCreationExpression, Type: <anonymous type: Key a As ?>, IsInvalid) (Syntax: AnonymousObjectCreationExpression, 'New With {K ... essOf c1.S}') (Parent: InvalidExpression)
  Initializers(1):
    ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: ?, IsInvalid, IsImplicit) (Syntax: NamedFieldInitializer, 'Key .a = AddressOf c1.S')
      Left: 
        IPropertyReferenceExpression: ReadOnly Property <anonymous type: Key a As ?>.a As ? ([0] OperationKind.PropertyReferenceExpression, Type: ?) (Syntax: IdentifierName, 'a')
          Instance Receiver: 
            null
      Right: 
        IInvalidExpression ([1] OperationKind.InvalidExpression, Type: ?, IsInvalid, IsImplicit) (Syntax: AddressOfExpression, 'AddressOf c1.S')
          Children(1):
            IOperation:  ([0] OperationKind.None, IsInvalid) (Syntax: AddressOfExpression, 'AddressOf c1.S')
              Children(1):
                IOperation:  ([0] OperationKind.None, IsInvalid) (Syntax: SimpleMemberAccessExpression, 'c1.S')
                  Children(1):
                    IOperation:  ([0] OperationKind.None, IsInvalid) (Syntax: IdentifierName, 'c1')
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30491: Expression does not produce a value.
        Console.WriteLine(New With {Key .a = AddressOf c1.S})'BIND:"New With {Key .a = AddressOf c1.S}"
                                             ~~~~~~~~~~~~~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of AnonymousObjectCreationExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub BoundPropertyGroup_ExposesReceiver()
            Dim source = <![CDATA[
Option Strict Off

Class C
    Private Sub M(c As C, d As Object)
        Dim x = c(c, d)'BIND:"c(c, d)"
    End Sub

    Default ReadOnly Property P1(x As Integer, x2 As Object) As Integer
        Get
            Return 1
        End Get
    End Property

    Default ReadOnly Property P1(x As String, x2 As Object) As Integer
        Get
            Return 1
        End Get
    End Property
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IInvalidExpression ([0] OperationKind.InvalidExpression, Type: System.Int32, IsInvalid) (Syntax: InvocationExpression, 'c(c, d)') (Parent: VariableInitializer)
  Children(3):
    IOperation:  ([0] OperationKind.None, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'c')
      Children(1):
        IParameterReferenceExpression: c ([0] OperationKind.ParameterReferenceExpression, Type: C, IsInvalid) (Syntax: IdentifierName, 'c')
    IParameterReferenceExpression: c ([1] OperationKind.ParameterReferenceExpression, Type: C) (Syntax: IdentifierName, 'c')
    IParameterReferenceExpression: d ([2] OperationKind.ParameterReferenceExpression, Type: System.Object) (Syntax: IdentifierName, 'd')
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30518: Overload resolution failed because no accessible 'P1' can be called with these arguments:
    'Public ReadOnly Default Property P1(x As Integer, x2 As Object) As Integer': Value of type 'C' cannot be converted to 'Integer'.
    'Public ReadOnly Default Property P1(x As String, x2 As Object) As Integer': Value of type 'C' cannot be converted to 'String'.
        Dim x = c(c, d)'BIND:"c(c, d)"
                ~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of InvocationExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub TestClone()
            Dim sourceCode = TestResource.AllInOneVisualBasicCode

            Dim fileName = "a.vb"
            Dim syntaxTree = Parse(sourceCode, fileName, options:=Nothing)

            Dim compilation = CreateCompilationWithMscorlib45AndVBRuntime({syntaxTree}, DefaultVbReferences.Concat({ValueTupleRef, SystemRuntimeFacadeRef}))
            Dim tree = (From t In compilation.SyntaxTrees Where t.FilePath = fileName).Single()
            Dim model = compilation.GetSemanticModel(tree)

            VerifyClone(model)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub TestParentOperations()
            Dim sourceCode = TestResource.AllInOneVisualBasicCode

            Dim fileName = "a.vb"
            Dim syntaxTree = Parse(sourceCode, fileName, options:=Nothing)

            Dim compilation = CreateCompilationWithMscorlib45AndVBRuntime({syntaxTree}, DefaultVbReferences.Concat({ValueTupleRef, SystemRuntimeFacadeRef}))
            Dim tree = (From t In compilation.SyntaxTrees Where t.FilePath = fileName).Single()
            Dim model = compilation.GetSemanticModel(tree)

            VerifyParentOperations(model)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub TestEndStatement()
            Dim source = <![CDATA[
Class C
    Public Shared i as Integer
    Public Shared Sub Main()
        If i = 0 Then
            End'BIND:"End"
        End If
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IEndStatement ([0] OperationKind.EndStatement) (Syntax: EndStatement, 'End') (Parent: BlockStatement)
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of StopOrEndStatementSyntax)(source, expectedOperationTree, expectedDiagnostics, compilationOptions:=TestOptions.ReleaseExe)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub TestEndStatement_Parent()
            Dim source = <![CDATA[
Class C
    Public Shared i As Integer
    Public Shared Sub Main()
        If i = 0 Then'BIND:"If i = 0 Then"
            End
        End If
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IIfStatement ([0] OperationKind.IfStatement) (Syntax: MultiLineIfBlock, 'If i = 0 Th ... End If') (Parent: BlockStatement)
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.Equals, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: EqualsExpression, 'i = 0')
      Left: 
        IFieldReferenceExpression: C.i As System.Int32 (Static) ([0] OperationKind.FieldReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
          Instance Receiver: 
            null
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  IfTrue: 
    IBlockStatement (1 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: MultiLineIfBlock, 'If i = 0 Th ... End If')
      IEndStatement ([0] OperationKind.EndStatement) (Syntax: EndStatement, 'End')
  IfFalse: 
    null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of MultiLineIfBlockSyntax)(source, expectedOperationTree, expectedDiagnostics, compilationOptions:=TestOptions.ReleaseExe)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub TestStopStatement()
            Dim source = <![CDATA[
Class C
    Public Shared i as Integer
    Public Shared Sub Main()
        If i = 0 Then
            Stop'BIND:"Stop"
        End If
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IStopStatement ([0] OperationKind.StopStatement) (Syntax: StopStatement, 'Stop') (Parent: BlockStatement)
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of StopOrEndStatementSyntax)(source, expectedOperationTree, expectedDiagnostics, compilationOptions:=TestOptions.ReleaseExe)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub TestStopStatement_Parent()
            Dim source = <![CDATA[
Class C
    Public Shared i As Integer
    Public Shared Sub Main()
        If i = 0 Then'BIND:"If i = 0 Then"
            Stop
        End If
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IIfStatement ([0] OperationKind.IfStatement) (Syntax: MultiLineIfBlock, 'If i = 0 Th ... End If') (Parent: BlockStatement)
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.Equals, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: EqualsExpression, 'i = 0')
      Left: 
        IFieldReferenceExpression: C.i As System.Int32 (Static) ([0] OperationKind.FieldReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
          Instance Receiver: 
            null
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  IfTrue: 
    IBlockStatement (1 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: MultiLineIfBlock, 'If i = 0 Th ... End If')
      IStopStatement ([0] OperationKind.StopStatement) (Syntax: StopStatement, 'Stop')
  IfFalse: 
    null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of MultiLineIfBlockSyntax)(source, expectedOperationTree, expectedDiagnostics, compilationOptions:=TestOptions.ReleaseExe)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub TestCatchClause()
            Dim source = <![CDATA[
Imports System

Module Program
    Sub Main(args As String())
        Try
            Main(Nothing)
        Catch ex As Exception When ex Is Nothing'BIND:"Catch ex As Exception When ex Is Nothing"

        End Try
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
ICatchClause (Exception type: System.Exception) ([1] OperationKind.CatchClause) (Syntax: CatchBlock, 'Catch ex As ...  Is Nothing') (Parent: TryStatement)
  Locals: Local_1: ex As System.Exception
  ExceptionDeclarationOrExpression: 
    IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: IdentifierName, 'ex')
      Variables: Local_1: ex As System.Exception
      Initializer: 
        null
  Filter: 
    IBinaryOperatorExpression (BinaryOperatorKind.Equals) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: IsExpression, 'ex Is Nothing')
      Left: 
        IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Object, IsImplicit) (Syntax: IdentifierName, 'ex')
          Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            ILocalReferenceExpression: ex ([0] OperationKind.LocalReferenceExpression, Type: System.Exception) (Syntax: IdentifierName, 'ex')
      Right: 
        IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Object, Constant: null, IsImplicit) (Syntax: NothingLiteralExpression, 'Nothing')
          Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: null, Constant: null) (Syntax: NothingLiteralExpression, 'Nothing')
  Handler: 
    IBlockStatement (0 statements) ([2] OperationKind.BlockStatement, IsImplicit) (Syntax: CatchBlock, 'Catch ex As ...  Is Nothing')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of CatchBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub TestSubSingleLineLambda()
            Dim source = <![CDATA[
Imports System

Class Test
    Sub Method()
        Dim a As Action = Sub() Console.Write("hello")'BIND:"Sub() Console.Write("hello")"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IAnonymousFunctionExpression (Symbol: Sub ()) ([0] OperationKind.AnonymousFunctionExpression, Type: null) (Syntax: SingleLineSubLambdaExpression, 'Sub() Conso ... te("hello")') (Parent: DelegateCreationExpression)
  IBlockStatement (3 statements) ([0] OperationKind.BlockStatement, IsImplicit) (Syntax: SingleLineSubLambdaExpression, 'Sub() Conso ... te("hello")')
    IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'Console.Write("hello")')
      Expression: 
        IInvocationExpression (Sub System.Console.Write(value As System.String)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Console.Write("hello")')
          Instance Receiver: 
            null
          Arguments(1):
            IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, '"hello"')
              ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: "hello") (Syntax: StringLiteralExpression, '"hello"')
              InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
              OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
    ILabeledStatement (Label: exit) ([1] OperationKind.LabeledStatement, IsImplicit) (Syntax: SingleLineSubLambdaExpression, 'Sub() Conso ... te("hello")')
      Statement: 
        null
    IReturnStatement ([2] OperationKind.ReturnStatement, IsImplicit) (Syntax: SingleLineSubLambdaExpression, 'Sub() Conso ... te("hello")')
      ReturnedValue: 
        null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of SingleLineLambdaExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub TestFunctionSingleLineLambda()
            Dim source = <![CDATA[
Imports System

Class Test
    Sub Method()
        Dim a As Func(Of Integer) = Function() 1'BIND:"Function() 1"
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IAnonymousFunctionExpression (Symbol: Function () As System.Int32) ([0] OperationKind.AnonymousFunctionExpression, Type: null) (Syntax: SingleLineFunctionLambdaExpression, 'Function() 1') (Parent: DelegateCreationExpression)
  IBlockStatement (3 statements, 1 locals) ([0] OperationKind.BlockStatement, IsImplicit) (Syntax: SingleLineFunctionLambdaExpression, 'Function() 1')
    Locals: Local_1: <anonymous local> As System.Int32
    IReturnStatement ([0] OperationKind.ReturnStatement, IsImplicit) (Syntax: NumericLiteralExpression, '1')
      ReturnedValue: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
    ILabeledStatement (Label: exit) ([1] OperationKind.LabeledStatement, IsImplicit) (Syntax: SingleLineFunctionLambdaExpression, 'Function() 1')
      Statement: 
        null
    IReturnStatement ([2] OperationKind.ReturnStatement, IsImplicit) (Syntax: SingleLineFunctionLambdaExpression, 'Function() 1')
      ReturnedValue: 
        ILocalReferenceExpression:  ([0] OperationKind.LocalReferenceExpression, Type: System.Int32, IsImplicit) (Syntax: SingleLineFunctionLambdaExpression, 'Function() 1')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of SingleLineLambdaExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub TestFunctionMultiLineLambda()
            Dim source = <![CDATA[
Imports System

Class Test
    Sub Method()
        Dim a As Func(Of Integer) = Function()'BIND:"Function()"
                                        Return 1
                                    End Function
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IAnonymousFunctionExpression (Symbol: Function () As System.Int32) ([0] OperationKind.AnonymousFunctionExpression, Type: null) (Syntax: MultiLineFunctionLambdaExpression, 'Function()' ... nd Function') (Parent: DelegateCreationExpression)
  IBlockStatement (3 statements, 1 locals) ([0] OperationKind.BlockStatement, IsImplicit) (Syntax: MultiLineFunctionLambdaExpression, 'Function()' ... nd Function')
    Locals: Local_1: <anonymous local> As System.Int32
    IReturnStatement ([0] OperationKind.ReturnStatement) (Syntax: ReturnStatement, 'Return 1')
      ReturnedValue: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
    ILabeledStatement (Label: exit) ([1] OperationKind.LabeledStatement, IsImplicit) (Syntax: EndFunctionStatement, 'End Function')
      Statement: 
        null
    IReturnStatement ([2] OperationKind.ReturnStatement, IsImplicit) (Syntax: EndFunctionStatement, 'End Function')
      ReturnedValue: 
        ILocalReferenceExpression:  ([0] OperationKind.LocalReferenceExpression, Type: System.Int32, IsImplicit) (Syntax: EndFunctionStatement, 'End Function')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of MultiLineLambdaExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub TestSubMultiLineLambda()
            Dim source = <![CDATA[
Imports System

Class Test
    Sub Method()
        Dim a As Action = Sub()'BIND:"Sub()"
                              Return
                          End Sub
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IAnonymousFunctionExpression (Symbol: Sub ()) ([0] OperationKind.AnonymousFunctionExpression, Type: null) (Syntax: MultiLineSubLambdaExpression, 'Sub()'BIND: ... End Sub') (Parent: DelegateCreationExpression)
  IBlockStatement (3 statements) ([0] OperationKind.BlockStatement, IsImplicit) (Syntax: MultiLineSubLambdaExpression, 'Sub()'BIND: ... End Sub')
    IReturnStatement ([0] OperationKind.ReturnStatement) (Syntax: ReturnStatement, 'Return')
      ReturnedValue: 
        null
    ILabeledStatement (Label: exit) ([1] OperationKind.LabeledStatement, IsImplicit) (Syntax: EndSubStatement, 'End Sub')
      Statement: 
        null
    IReturnStatement ([2] OperationKind.ReturnStatement, IsImplicit) (Syntax: EndSubStatement, 'End Sub')
      ReturnedValue: 
        null
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of MultiLineLambdaExpressionSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub
    End Class
End Namespace
