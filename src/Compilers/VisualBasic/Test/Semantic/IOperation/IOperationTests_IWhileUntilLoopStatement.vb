' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports Microsoft.CodeAnalysis.Semantics
Imports Microsoft.CodeAnalysis.Test.Utilities
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Test.Utilities

Namespace Microsoft.CodeAnalysis.VisualBasic.UnitTests.Semantics

    Partial Public Class IOperationTests
        Inherits SemanticModelTestBase
        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IWhileUntilLoopStatement_DoWhileLoopsTest()
            Dim source = <![CDATA[
Class Program
    Private Shared Sub Main()
        Dim ids As Integer() = New Integer() {6, 7, 8, 10}
        Dim sum As Integer = 0
        Dim i As Integer = 0
        Do'BIND:"Do"
            sum += ids(i)
            i += 1
        Loop While i < 4

        System.Console.WriteLine(sum)
    End Sub
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IDoLoopStatement (DoLoopKind: DoWhileBottomLoop) (LoopKind.Do) ([3] OperationKind.LoopStatement) (Syntax: DoLoopWhileBlock, 'Do'BIND:"Do ... While i < 4') (Parent: BlockStatement)
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.LessThan, Checked) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'i < 4')
      Left: 
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 4) (Syntax: NumericLiteralExpression, '4')
  IgnoredCondition: 
    null
  Body: 
    IBlockStatement (2 statements) ([0] OperationKind.BlockStatement, IsImplicit) (Syntax: DoLoopWhileBlock, 'Do'BIND:"Do ... While i < 4')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: AddAssignmentStatement, 'sum += ids(i)')
        Expression: 
          ICompoundAssignmentExpression (BinaryOperatorKind.Add, Checked) ([0] OperationKind.CompoundAssignmentExpression, Type: System.Int32) (Syntax: AddAssignmentStatement, 'sum += ids(i)')
            Left: 
              ILocalReferenceExpression: sum ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'sum')
            Right: 
              IArrayElementReferenceExpression ([1] OperationKind.ArrayElementReferenceExpression, Type: System.Int32) (Syntax: InvocationExpression, 'ids(i)')
                Array reference: 
                  ILocalReferenceExpression: ids ([0] OperationKind.LocalReferenceExpression, Type: System.Int32()) (Syntax: IdentifierName, 'ids')
                Indices(1):
                  ILocalReferenceExpression: i ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
      IExpressionStatement ([1] OperationKind.ExpressionStatement) (Syntax: AddAssignmentStatement, 'i += 1')
        Expression: 
          ICompoundAssignmentExpression (BinaryOperatorKind.Add, Checked) ([0] OperationKind.CompoundAssignmentExpression, Type: System.Int32) (Syntax: AddAssignmentStatement, 'i += 1')
            Left: 
              ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
]]>.Value

            VerifyOperationTreeForTest(Of DoLoopBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IWhileUntilLoopStatement_DoUntilLoopTest()
            Dim source = <![CDATA[
Class C
    Private X As Integer = 10
    Sub M(c As C)
        Do Until X < 0'BIND:"Do Until X < 0"
            X = X - 1
        Loop
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IDoLoopStatement (DoLoopKind: DoUntilTopLoop) (LoopKind.Do) ([0] OperationKind.LoopStatement) (Syntax: DoUntilLoopBlock, 'Do Until X  ... Loop') (Parent: BlockStatement)
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.LessThan, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'X < 0')
      Left: 
        IFieldReferenceExpression: C.X As System.Int32 ([0] OperationKind.FieldReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'X')
          Instance Receiver: 
            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C, IsImplicit) (Syntax: IdentifierName, 'X')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  IgnoredCondition: 
    null
  Body: 
    IBlockStatement (1 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: DoUntilLoopBlock, 'Do Until X  ... Loop')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'X = X - 1')
        Expression: 
          ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'X = X - 1')
            Left: 
              IFieldReferenceExpression: C.X As System.Int32 ([0] OperationKind.FieldReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'X')
                Instance Receiver: 
                  IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C, IsImplicit) (Syntax: IdentifierName, 'X')
            Right: 
              IBinaryOperatorExpression (BinaryOperatorKind.Subtract, Checked) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: SubtractExpression, 'X - 1')
                Left: 
                  IFieldReferenceExpression: C.X As System.Int32 ([0] OperationKind.FieldReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'X')
                    Instance Receiver: 
                      IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C, IsImplicit) (Syntax: IdentifierName, 'X')
                Right: 
                  ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of DoLoopBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IWhileUntilLoopStatement_WhileConditionTrue()
            Dim source = <![CDATA[
Class Program
    Private Shared Sub Main()
        Dim index As Integer = 0
        Dim condition As Boolean = True
        While condition'BIND:"While condition"
            Dim value As Integer = System.Threading.Interlocked.Increment(index)
            If value > 10 Then
                condition = False
            End If
        End While
    End Sub
End Class
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IWhileLoopStatement (LoopKind.While) ([2] OperationKind.LoopStatement) (Syntax: WhileBlock, 'While condi ... End While') (Parent: BlockStatement)
  Condition: 
    ILocalReferenceExpression: condition ([0] OperationKind.LocalReferenceExpression, Type: System.Boolean) (Syntax: IdentifierName, 'condition')
  Body: 
    IBlockStatement (2 statements, 1 locals) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: WhileBlock, 'While condi ... End While')
      Locals: Local_1: value As System.Int32
      IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Dim value A ... ment(index)')
        IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'value')
          Variables: Local_1: value As System.Int32
          Initializer: 
            IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValue, '= System.Th ... ment(index)')
              IInvocationExpression (Function System.Threading.Interlocked.Increment(ByRef location As System.Int32) As System.Int32) ([0] OperationKind.InvocationExpression, Type: System.Int32) (Syntax: InvocationExpression, 'System.Thre ... ment(index)')
                Instance Receiver: 
                  null
                Arguments(1):
                  IArgument (ArgumentKind.Explicit, Matching Parameter: location) ([0] OperationKind.Argument) (Syntax: SimpleArgument, 'index')
                    ILocalReferenceExpression: index ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'index')
                    InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      IIfStatement ([1] OperationKind.IfStatement) (Syntax: MultiLineIfBlock, 'If value >  ... End If')
        Condition: 
          IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanExpression, 'value > 10')
            Left: 
              ILocalReferenceExpression: value ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'value')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10) (Syntax: NumericLiteralExpression, '10')
        IfTrue: 
          IBlockStatement (1 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: MultiLineIfBlock, 'If value >  ... End If')
            IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'condition = False')
              Expression: 
                ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Boolean) (Syntax: SimpleAssignmentStatement, 'condition = False')
                  Left: 
                    ILocalReferenceExpression: condition ([0] OperationKind.LocalReferenceExpression, Type: System.Boolean) (Syntax: IdentifierName, 'condition')
                  Right: 
                    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Boolean, Constant: False) (Syntax: FalseLiteralExpression, 'False')
        IfFalse: 
          null
]]>.Value

            VerifyOperationTreeForTest(Of WhileBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IWhileUntilLoopStatement_WhileLoopsTest()
            Dim source = <![CDATA[
Class Program
    Private Shared Function SumWhile() As Integer
        '
        ' Sum numbers 0 .. 4
        '
        Dim sum As Integer = 0
        Dim i As Integer = 0
        While i < 5'BIND:"While i < 5"
            sum += i
            i += 1
        End While
        Return sum
    End Function
End Class
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IWhileLoopStatement (LoopKind.While) ([2] OperationKind.LoopStatement) (Syntax: WhileBlock, 'While i < 5 ... End While') (Parent: BlockStatement)
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.LessThan, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'i < 5')
      Left: 
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 5) (Syntax: NumericLiteralExpression, '5')
  Body: 
    IBlockStatement (2 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: WhileBlock, 'While i < 5 ... End While')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: AddAssignmentStatement, 'sum += i')
        Expression: 
          ICompoundAssignmentExpression (BinaryOperatorKind.Add, Checked) ([0] OperationKind.CompoundAssignmentExpression, Type: System.Int32) (Syntax: AddAssignmentStatement, 'sum += i')
            Left: 
              ILocalReferenceExpression: sum ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'sum')
            Right: 
              ILocalReferenceExpression: i ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
      IExpressionStatement ([1] OperationKind.ExpressionStatement) (Syntax: AddAssignmentStatement, 'i += 1')
        Expression: 
          ICompoundAssignmentExpression (BinaryOperatorKind.Add, Checked) ([0] OperationKind.CompoundAssignmentExpression, Type: System.Int32) (Syntax: AddAssignmentStatement, 'i += 1')
            Left: 
              ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
]]>.Value

            VerifyOperationTreeForTest(Of WhileBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IWhileUntilLoopStatement_WhileWithBreak()
            Dim source = <![CDATA[
Class Program
    Private Shared Sub Main()
        Dim index As Integer = 0
        While True'BIND:"While True"
            Dim value As Integer = System.Threading.Interlocked.Increment(index)
            If value > 5 Then
                System.Console.WriteLine("While-loop break")
                Exit While
            End If
            System.Console.WriteLine("While-loop statement")
        End While
    End Sub
End Class
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IWhileLoopStatement (LoopKind.While) ([1] OperationKind.LoopStatement) (Syntax: WhileBlock, 'While True' ... End While') (Parent: BlockStatement)
  Condition: 
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Boolean, Constant: True) (Syntax: TrueLiteralExpression, 'True')
  Body: 
    IBlockStatement (3 statements, 1 locals) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: WhileBlock, 'While True' ... End While')
      Locals: Local_1: value As System.Int32
      IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Dim value A ... ment(index)')
        IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'value')
          Variables: Local_1: value As System.Int32
          Initializer: 
            IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValue, '= System.Th ... ment(index)')
              IInvocationExpression (Function System.Threading.Interlocked.Increment(ByRef location As System.Int32) As System.Int32) ([0] OperationKind.InvocationExpression, Type: System.Int32) (Syntax: InvocationExpression, 'System.Thre ... ment(index)')
                Instance Receiver: 
                  null
                Arguments(1):
                  IArgument (ArgumentKind.Explicit, Matching Parameter: location) ([0] OperationKind.Argument) (Syntax: SimpleArgument, 'index')
                    ILocalReferenceExpression: index ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'index')
                    InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      IIfStatement ([1] OperationKind.IfStatement) (Syntax: MultiLineIfBlock, 'If value >  ... End If')
        Condition: 
          IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanExpression, 'value > 5')
            Left: 
              ILocalReferenceExpression: value ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'value')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 5) (Syntax: NumericLiteralExpression, '5')
        IfTrue: 
          IBlockStatement (2 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: MultiLineIfBlock, 'If value >  ... End If')
            IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... oop break")')
              Expression: 
                IInvocationExpression (Sub System.Console.WriteLine(value As System.String)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... oop break")')
                  Instance Receiver: 
                    null
                  Arguments(1):
                    IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, '"While-loop break"')
                      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: "While-loop break") (Syntax: StringLiteralExpression, '"While-loop break"')
                      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            IBranchStatement (BranchKind.Break, Label: exit) ([1] OperationKind.BranchStatement) (Syntax: ExitWhileStatement, 'Exit While')
        IfFalse: 
          null
      IExpressionStatement ([2] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... statement")')
        Expression: 
          IInvocationExpression (Sub System.Console.WriteLine(value As System.String)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... statement")')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, '"While-loop statement"')
                ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: "While-loop statement") (Syntax: StringLiteralExpression, '"While-loop statement"')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
]]>.Value

            VerifyOperationTreeForTest(Of WhileBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IWhileUntilLoopStatement_WhileWithThrow()
            Dim source = <![CDATA[
Class Program
    Private Shared Sub Main()
        Dim index As Integer = 0
        While True'BIND:"While True"
            Dim value As Integer = System.Threading.Interlocked.Increment(index)
            If value > 100 Then
                Throw New System.Exception("An exception has occurred.")
            End If
            System.Console.WriteLine("While-loop statement")
        End While
    End Sub
End Class
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IWhileLoopStatement (LoopKind.While) ([1] OperationKind.LoopStatement) (Syntax: WhileBlock, 'While True' ... End While') (Parent: BlockStatement)
  Condition: 
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Boolean, Constant: True) (Syntax: TrueLiteralExpression, 'True')
  Body: 
    IBlockStatement (3 statements, 1 locals) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: WhileBlock, 'While True' ... End While')
      Locals: Local_1: value As System.Int32
      IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Dim value A ... ment(index)')
        IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'value')
          Variables: Local_1: value As System.Int32
          Initializer: 
            IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValue, '= System.Th ... ment(index)')
              IInvocationExpression (Function System.Threading.Interlocked.Increment(ByRef location As System.Int32) As System.Int32) ([0] OperationKind.InvocationExpression, Type: System.Int32) (Syntax: InvocationExpression, 'System.Thre ... ment(index)')
                Instance Receiver: 
                  null
                Arguments(1):
                  IArgument (ArgumentKind.Explicit, Matching Parameter: location) ([0] OperationKind.Argument) (Syntax: SimpleArgument, 'index')
                    ILocalReferenceExpression: index ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'index')
                    InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      IIfStatement ([1] OperationKind.IfStatement) (Syntax: MultiLineIfBlock, 'If value >  ... End If')
        Condition: 
          IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanExpression, 'value > 100')
            Left: 
              ILocalReferenceExpression: value ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'value')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 100) (Syntax: NumericLiteralExpression, '100')
        IfTrue: 
          IBlockStatement (1 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: MultiLineIfBlock, 'If value >  ... End If')
            IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ThrowStatement, 'Throw New S ... occurred.")')
              Expression: 
                IThrowExpression ([0] OperationKind.ThrowExpression, Type: System.Exception) (Syntax: ThrowStatement, 'Throw New S ... occurred.")')
                  IObjectCreationExpression (Constructor: Sub System.Exception..ctor(message As System.String)) ([0] OperationKind.ObjectCreationExpression, Type: System.Exception) (Syntax: ObjectCreationExpression, 'New System. ... occurred.")')
                    Arguments(1):
                      IArgument (ArgumentKind.Explicit, Matching Parameter: message) ([0] OperationKind.Argument) (Syntax: SimpleArgument, '"An excepti ...  occurred."')
                        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: "An exception has occurred.") (Syntax: StringLiteralExpression, '"An excepti ...  occurred."')
                        InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                        OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    Initializer: 
                      null
        IfFalse: 
          null
      IExpressionStatement ([2] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... statement")')
        Expression: 
          IInvocationExpression (Sub System.Console.WriteLine(value As System.String)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... statement")')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, '"While-loop statement"')
                ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: "While-loop statement") (Syntax: StringLiteralExpression, '"While-loop statement"')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
]]>.Value

            VerifyOperationTreeForTest(Of WhileBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IWhileUntilLoopStatement_WhileWithAssignment()
            Dim source = <![CDATA[
Class Program
    Private Shared Sub Main()
        Dim value As Integer = 4
        Dim i As Integer
        While (InlineAssignHelper(i, value)) >= 0'BIND:"While (InlineAssignHelper(i, value)) >= 0"
            System.Console.WriteLine("While {0} {1}", i, value)
            value -= 1
        End While
    End Sub
    Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
        target = value
        Return value
    End Function
End Class
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IWhileLoopStatement (LoopKind.While) ([2] OperationKind.LoopStatement) (Syntax: WhileBlock, 'While (Inli ... End While') (Parent: BlockStatement)
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.GreaterThanOrEqual, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanOrEqualExpression, '(InlineAssi ... alue)) >= 0')
      Left: 
        IParenthesizedExpression ([0] OperationKind.ParenthesizedExpression, Type: System.Int32) (Syntax: ParenthesizedExpression, '(InlineAssi ... (i, value))')
          Operand: 
            IInvocationExpression (Function Program.InlineAssignHelper(Of System.Int32)(ByRef target As System.Int32, value As System.Int32) As System.Int32) ([0] OperationKind.InvocationExpression, Type: System.Int32) (Syntax: InvocationExpression, 'InlineAssig ... r(i, value)')
              Instance Receiver: 
                null
              Arguments(2):
                IArgument (ArgumentKind.Explicit, Matching Parameter: target) ([0] OperationKind.Argument) (Syntax: SimpleArgument, 'i')
                  ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
                  InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                  OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([1] OperationKind.Argument) (Syntax: SimpleArgument, 'value')
                  ILocalReferenceExpression: value ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'value')
                  InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                  OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  Body: 
    IBlockStatement (2 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: WhileBlock, 'While (Inli ... End While')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... , i, value)')
        Expression: 
          IInvocationExpression (Sub System.Console.WriteLine(format As System.String, arg0 As System.Object, arg1 As System.Object)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... , i, value)')
            Instance Receiver: 
              null
            Arguments(3):
              IArgument (ArgumentKind.Explicit, Matching Parameter: format) ([0] OperationKind.Argument) (Syntax: SimpleArgument, '"While {0} {1}"')
                ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: "While {0} {1}") (Syntax: StringLiteralExpression, '"While {0} {1}"')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
              IArgument (ArgumentKind.Explicit, Matching Parameter: arg0) ([1] OperationKind.Argument) (Syntax: SimpleArgument, 'i')
                IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Object, IsImplicit) (Syntax: IdentifierName, 'i')
                  Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                  Operand: 
                    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
              IArgument (ArgumentKind.Explicit, Matching Parameter: arg1) ([2] OperationKind.Argument) (Syntax: SimpleArgument, 'value')
                IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Object, IsImplicit) (Syntax: IdentifierName, 'value')
                  Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                  Operand: 
                    ILocalReferenceExpression: value ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'value')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      IExpressionStatement ([1] OperationKind.ExpressionStatement) (Syntax: SubtractAssignmentStatement, 'value -= 1')
        Expression: 
          ICompoundAssignmentExpression (BinaryOperatorKind.Subtract, Checked) ([0] OperationKind.CompoundAssignmentExpression, Type: System.Int32) (Syntax: SubtractAssignmentStatement, 'value -= 1')
            Left: 
              ILocalReferenceExpression: value ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'value')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
]]>.Value

            VerifyOperationTreeForTest(Of WhileBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IWhileUntilLoopStatement_WhileImplicit()
            Dim source = <![CDATA[
Class Program
    Private Shared Sub Main()
        Dim number As Integer = 10
        While number'BIND:"While number"
        End While
    End Sub
End Class
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IWhileLoopStatement (LoopKind.While) ([1] OperationKind.LoopStatement) (Syntax: WhileBlock, 'While numbe ... End While') (Parent: BlockStatement)
  Condition: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Boolean, IsImplicit) (Syntax: IdentifierName, 'number')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILocalReferenceExpression: number ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'number')
  Body: 
    IBlockStatement (0 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: WhileBlock, 'While numbe ... End While')
]]>.Value

            VerifyOperationTreeForTest(Of WhileBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IWhileUntilLoopStatement_WhileWithReturn()
            Dim source = <![CDATA[
Class Program
    Private Shared Sub Main()
        System.Console.WriteLine(GetFirstEvenNumber(33))
    End Sub
    Public Shared Function GetFirstEvenNumber(number As Integer) As Integer
        While True'BIND:"While True"
            If (number Mod 2) = 0 Then
                Return number
            End If

            number += 1
        End While
    End Function
End Class
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IWhileLoopStatement (LoopKind.While) ([0] OperationKind.LoopStatement) (Syntax: WhileBlock, 'While True' ... End While') (Parent: BlockStatement)
  Condition: 
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Boolean, Constant: True) (Syntax: TrueLiteralExpression, 'True')
  Body: 
    IBlockStatement (2 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: WhileBlock, 'While True' ... End While')
      IIfStatement ([0] OperationKind.IfStatement) (Syntax: MultiLineIfBlock, 'If (number  ... End If')
        Condition: 
          IBinaryOperatorExpression (BinaryOperatorKind.Equals, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: EqualsExpression, '(number Mod 2) = 0')
            Left: 
              IParenthesizedExpression ([0] OperationKind.ParenthesizedExpression, Type: System.Int32) (Syntax: ParenthesizedExpression, '(number Mod 2)')
                Operand: 
                  IBinaryOperatorExpression (BinaryOperatorKind.Remainder, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: ModuloExpression, 'number Mod 2')
                    Left: 
                      IParameterReferenceExpression: number ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'number')
                    Right: 
                      ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
        IfTrue: 
          IBlockStatement (1 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: MultiLineIfBlock, 'If (number  ... End If')
            IReturnStatement ([0] OperationKind.ReturnStatement) (Syntax: ReturnStatement, 'Return number')
              ReturnedValue: 
                IParameterReferenceExpression: number ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'number')
        IfFalse: 
          null
      IExpressionStatement ([1] OperationKind.ExpressionStatement) (Syntax: AddAssignmentStatement, 'number += 1')
        Expression: 
          ICompoundAssignmentExpression (BinaryOperatorKind.Add, Checked) ([0] OperationKind.CompoundAssignmentExpression, Type: System.Int32) (Syntax: AddAssignmentStatement, 'number += 1')
            Left: 
              IParameterReferenceExpression: number ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'number')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
]]>.Value

            VerifyOperationTreeForTest(Of WhileBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IWhileUntilLoopStatement_WhileWithGoto()
            Dim source = <![CDATA[
Class Program
    Private Shared Sub Main()
        System.Console.WriteLine(GetFirstEvenNumber(33))
    End Sub
    Public Shared Function GetFirstEvenNumber(number As Integer) As Integer
        While True'BIND:"While True"
            If (number Mod 2) = 0 Then
                GoTo Even
            End If
            number += 1
Even:
            Return number
        End While
    End Function
End Class
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IWhileLoopStatement (LoopKind.While) ([0] OperationKind.LoopStatement) (Syntax: WhileBlock, 'While True' ... End While') (Parent: BlockStatement)
  Condition: 
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Boolean, Constant: True) (Syntax: TrueLiteralExpression, 'True')
  Body: 
    IBlockStatement (4 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: WhileBlock, 'While True' ... End While')
      IIfStatement ([0] OperationKind.IfStatement) (Syntax: MultiLineIfBlock, 'If (number  ... End If')
        Condition: 
          IBinaryOperatorExpression (BinaryOperatorKind.Equals, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: EqualsExpression, '(number Mod 2) = 0')
            Left: 
              IParenthesizedExpression ([0] OperationKind.ParenthesizedExpression, Type: System.Int32) (Syntax: ParenthesizedExpression, '(number Mod 2)')
                Operand: 
                  IBinaryOperatorExpression (BinaryOperatorKind.Remainder, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: ModuloExpression, 'number Mod 2')
                    Left: 
                      IParameterReferenceExpression: number ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'number')
                    Right: 
                      ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
        IfTrue: 
          IBlockStatement (1 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: MultiLineIfBlock, 'If (number  ... End If')
            IBranchStatement (BranchKind.GoTo, Label: Even) ([0] OperationKind.BranchStatement) (Syntax: GoToStatement, 'GoTo Even')
        IfFalse: 
          null
      IExpressionStatement ([1] OperationKind.ExpressionStatement) (Syntax: AddAssignmentStatement, 'number += 1')
        Expression: 
          ICompoundAssignmentExpression (BinaryOperatorKind.Add, Checked) ([0] OperationKind.CompoundAssignmentExpression, Type: System.Int32) (Syntax: AddAssignmentStatement, 'number += 1')
            Left: 
              IParameterReferenceExpression: number ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'number')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
      ILabeledStatement (Label: Even) ([2] OperationKind.LabeledStatement) (Syntax: LabelStatement, 'Even:')
        Statement: 
          null
      IReturnStatement ([3] OperationKind.ReturnStatement) (Syntax: ReturnStatement, 'Return number')
        ReturnedValue: 
          IParameterReferenceExpression: number ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'number')
]]>.Value

            VerifyOperationTreeForTest(Of WhileBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IWhileUntilLoopStatement_WhileMissingCondition()
            Dim source = <![CDATA[
Class Program
    Private Shared Sub Main()
        Dim index As Integer = 0
        Dim condition As Boolean = True
        While 'BIND:"While "
            Dim value As Integer = System.Threading.Interlocked.Increment(index)
            If value > 100 Then
                condition = False
            End If
        End While
    End Sub
End Class
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IWhileLoopStatement (LoopKind.While) ([2] OperationKind.LoopStatement, IsInvalid) (Syntax: WhileBlock, 'While 'BIND ... End While') (Parent: BlockStatement)
  Condition: 
    IInvalidExpression ([0] OperationKind.InvalidExpression, Type: null, IsInvalid) (Syntax: IdentifierName, '')
      Children(0)
  Body: 
    IBlockStatement (2 statements, 1 locals) ([1] OperationKind.BlockStatement, IsInvalid, IsImplicit) (Syntax: WhileBlock, 'While 'BIND ... End While')
      Locals: Local_1: value As System.Int32
      IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Dim value A ... ment(index)')
        IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'value')
          Variables: Local_1: value As System.Int32
          Initializer: 
            IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValue, '= System.Th ... ment(index)')
              IInvocationExpression (Function System.Threading.Interlocked.Increment(ByRef location As System.Int32) As System.Int32) ([0] OperationKind.InvocationExpression, Type: System.Int32) (Syntax: InvocationExpression, 'System.Thre ... ment(index)')
                Instance Receiver: 
                  null
                Arguments(1):
                  IArgument (ArgumentKind.Explicit, Matching Parameter: location) ([0] OperationKind.Argument) (Syntax: SimpleArgument, 'index')
                    ILocalReferenceExpression: index ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'index')
                    InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      IIfStatement ([1] OperationKind.IfStatement) (Syntax: MultiLineIfBlock, 'If value >  ... End If')
        Condition: 
          IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanExpression, 'value > 100')
            Left: 
              ILocalReferenceExpression: value ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'value')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 100) (Syntax: NumericLiteralExpression, '100')
        IfTrue: 
          IBlockStatement (1 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: MultiLineIfBlock, 'If value >  ... End If')
            IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'condition = False')
              Expression: 
                ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Boolean) (Syntax: SimpleAssignmentStatement, 'condition = False')
                  Left: 
                    ILocalReferenceExpression: condition ([0] OperationKind.LocalReferenceExpression, Type: System.Boolean) (Syntax: IdentifierName, 'condition')
                  Right: 
                    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Boolean, Constant: False) (Syntax: FalseLiteralExpression, 'False')
        IfFalse: 
          null
]]>.Value

            VerifyOperationTreeForTest(Of WhileBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IWhileUntilLoopStatement_WhileMissingStatement()
            Dim source = <![CDATA[
Class Program
    Private Shared Sub Main()
        Dim index As Integer = 0
        Dim condition As Boolean = True
        While (condition)'BIND:"While (condition)"
        End While
    End Sub
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IWhileLoopStatement (LoopKind.While) ([2] OperationKind.LoopStatement) (Syntax: WhileBlock, 'While (cond ... End While') (Parent: BlockStatement)
  Condition: 
    IParenthesizedExpression ([0] OperationKind.ParenthesizedExpression, Type: System.Boolean) (Syntax: ParenthesizedExpression, '(condition)')
      Operand: 
        ILocalReferenceExpression: condition ([0] OperationKind.LocalReferenceExpression, Type: System.Boolean) (Syntax: IdentifierName, 'condition')
  Body: 
    IBlockStatement (0 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: WhileBlock, 'While (cond ... End While')
]]>.Value

            VerifyOperationTreeForTest(Of WhileBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IWhileUntilLoopStatement_WhileWithContinue()
            Dim source = <![CDATA[
Class ContinueTest
    Private Shared Sub Main()
        Dim i As Integer = 0
        While i <= 10'BIND:"While i <= 10"
            i += 1
            If i < 9 Then
                Continue While
            End If
            System.Console.WriteLine(i)
        End While
    End Sub
End Class
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IWhileLoopStatement (LoopKind.While) ([1] OperationKind.LoopStatement) (Syntax: WhileBlock, 'While i <=  ... End While') (Parent: BlockStatement)
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.LessThanOrEqual, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanOrEqualExpression, 'i <= 10')
      Left: 
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10) (Syntax: NumericLiteralExpression, '10')
  Body: 
    IBlockStatement (3 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: WhileBlock, 'While i <=  ... End While')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: AddAssignmentStatement, 'i += 1')
        Expression: 
          ICompoundAssignmentExpression (BinaryOperatorKind.Add, Checked) ([0] OperationKind.CompoundAssignmentExpression, Type: System.Int32) (Syntax: AddAssignmentStatement, 'i += 1')
            Left: 
              ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
      IIfStatement ([1] OperationKind.IfStatement) (Syntax: MultiLineIfBlock, 'If i < 9 Th ... End If')
        Condition: 
          IBinaryOperatorExpression (BinaryOperatorKind.LessThan, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'i < 9')
            Left: 
              ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 9) (Syntax: NumericLiteralExpression, '9')
        IfTrue: 
          IBlockStatement (1 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: MultiLineIfBlock, 'If i < 9 Th ... End If')
            IBranchStatement (BranchKind.Continue, Label: continue) ([0] OperationKind.BranchStatement) (Syntax: ContinueWhileStatement, 'Continue While')
        IfFalse: 
          null
      IExpressionStatement ([2] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... riteLine(i)')
        Expression: 
          IInvocationExpression (Sub System.Console.WriteLine(value As System.Int32)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... riteLine(i)')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, 'i')
                ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
]]>.Value

            VerifyOperationTreeForTest(Of WhileBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IWhileUntilLoopStatement_WhileConditionInvocationExpression()
            Dim source = <![CDATA[
Class ContinueTest
    Private Shared Sub Main()
        Dim i As Integer = 0
        While IsTrue(i)'BIND:"While IsTrue(i)"
            i += 1
            System.Console.WriteLine(i)
        End While
    End Sub
    Private Shared Function IsTrue(i As Integer) As Boolean
        If i < 9 Then
            Return True
        Else
            Return False
        End If
    End Function
End Class
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IWhileLoopStatement (LoopKind.While) ([1] OperationKind.LoopStatement) (Syntax: WhileBlock, 'While IsTru ... End While') (Parent: BlockStatement)
  Condition: 
    IInvocationExpression (Function ContinueTest.IsTrue(i As System.Int32) As System.Boolean) ([0] OperationKind.InvocationExpression, Type: System.Boolean) (Syntax: InvocationExpression, 'IsTrue(i)')
      Instance Receiver: 
        null
      Arguments(1):
        IArgument (ArgumentKind.Explicit, Matching Parameter: i) ([0] OperationKind.Argument) (Syntax: SimpleArgument, 'i')
          ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
          InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  Body: 
    IBlockStatement (2 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: WhileBlock, 'While IsTru ... End While')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: AddAssignmentStatement, 'i += 1')
        Expression: 
          ICompoundAssignmentExpression (BinaryOperatorKind.Add, Checked) ([0] OperationKind.CompoundAssignmentExpression, Type: System.Int32) (Syntax: AddAssignmentStatement, 'i += 1')
            Left: 
              ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
      IExpressionStatement ([1] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... riteLine(i)')
        Expression: 
          IInvocationExpression (Sub System.Console.WriteLine(value As System.Int32)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... riteLine(i)')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, 'i')
                ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
]]>.Value

            VerifyOperationTreeForTest(Of WhileBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IWhileUntilLoopStatement_WhileNested()
            Dim source = <![CDATA[
Class Test
    Private Shared Sub Main()
        Dim i As Integer = 0
        While i < 10'BIND:"While i < 10"
            i += 1
            Dim j As Integer = 0
            While j < 10
                j += 1
                System.Console.WriteLine(j)
            End While
            System.Console.WriteLine(i)
        End While
    End Sub
End Class
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IWhileLoopStatement (LoopKind.While) ([1] OperationKind.LoopStatement) (Syntax: WhileBlock, 'While i < 1 ... End While') (Parent: BlockStatement)
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.LessThan, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'i < 10')
      Left: 
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10) (Syntax: NumericLiteralExpression, '10')
  Body: 
    IBlockStatement (4 statements, 1 locals) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: WhileBlock, 'While i < 1 ... End While')
      Locals: Local_1: j As System.Int32
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: AddAssignmentStatement, 'i += 1')
        Expression: 
          ICompoundAssignmentExpression (BinaryOperatorKind.Add, Checked) ([0] OperationKind.CompoundAssignmentExpression, Type: System.Int32) (Syntax: AddAssignmentStatement, 'i += 1')
            Left: 
              ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
      IVariableDeclarationStatement (1 declarations) ([1] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Dim j As Integer = 0')
        IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'j')
          Variables: Local_1: j As System.Int32
          Initializer: 
            IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValue, '= 0')
              ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
      IWhileLoopStatement (LoopKind.While) ([2] OperationKind.LoopStatement) (Syntax: WhileBlock, 'While j < 1 ... End While')
        Condition: 
          IBinaryOperatorExpression (BinaryOperatorKind.LessThan, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'j < 10')
            Left: 
              ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10) (Syntax: NumericLiteralExpression, '10')
        Body: 
          IBlockStatement (2 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: WhileBlock, 'While j < 1 ... End While')
            IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: AddAssignmentStatement, 'j += 1')
              Expression: 
                ICompoundAssignmentExpression (BinaryOperatorKind.Add, Checked) ([0] OperationKind.CompoundAssignmentExpression, Type: System.Int32) (Syntax: AddAssignmentStatement, 'j += 1')
                  Left: 
                    ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
                  Right: 
                    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
            IExpressionStatement ([1] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... riteLine(j)')
              Expression: 
                IInvocationExpression (Sub System.Console.WriteLine(value As System.Int32)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... riteLine(j)')
                  Instance Receiver: 
                    null
                  Arguments(1):
                    IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, 'j')
                      ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
                      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      IExpressionStatement ([3] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... riteLine(i)')
        Expression: 
          IInvocationExpression (Sub System.Console.WriteLine(value As System.Int32)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... riteLine(i)')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, 'i')
                ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
]]>.Value

            VerifyOperationTreeForTest(Of WhileBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IWhileUntilLoopStatement_WhileChangeOuterInnerValue()
            Dim source = <![CDATA[
Class Test
    Private Shared Sub Main()
        Dim i As Integer = 0
        While i < 10'BIND:"While i < 10"
            i += 1
            Dim j As Integer = 0
            While j < 10
                j += 1
                i = i + j
                System.Console.WriteLine(j)
            End While
            System.Console.WriteLine(i)
        End While
    End Sub
End Class
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IWhileLoopStatement (LoopKind.While) ([1] OperationKind.LoopStatement) (Syntax: WhileBlock, 'While i < 1 ... End While') (Parent: BlockStatement)
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.LessThan, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'i < 10')
      Left: 
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10) (Syntax: NumericLiteralExpression, '10')
  Body: 
    IBlockStatement (4 statements, 1 locals) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: WhileBlock, 'While i < 1 ... End While')
      Locals: Local_1: j As System.Int32
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: AddAssignmentStatement, 'i += 1')
        Expression: 
          ICompoundAssignmentExpression (BinaryOperatorKind.Add, Checked) ([0] OperationKind.CompoundAssignmentExpression, Type: System.Int32) (Syntax: AddAssignmentStatement, 'i += 1')
            Left: 
              ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
      IVariableDeclarationStatement (1 declarations) ([1] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'Dim j As Integer = 0')
        IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'j')
          Variables: Local_1: j As System.Int32
          Initializer: 
            IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValue, '= 0')
              ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
      IWhileLoopStatement (LoopKind.While) ([2] OperationKind.LoopStatement) (Syntax: WhileBlock, 'While j < 1 ... End While')
        Condition: 
          IBinaryOperatorExpression (BinaryOperatorKind.LessThan, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'j < 10')
            Left: 
              ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10) (Syntax: NumericLiteralExpression, '10')
        Body: 
          IBlockStatement (3 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: WhileBlock, 'While j < 1 ... End While')
            IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: AddAssignmentStatement, 'j += 1')
              Expression: 
                ICompoundAssignmentExpression (BinaryOperatorKind.Add, Checked) ([0] OperationKind.CompoundAssignmentExpression, Type: System.Int32) (Syntax: AddAssignmentStatement, 'j += 1')
                  Left: 
                    ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
                  Right: 
                    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
            IExpressionStatement ([1] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'i = i + j')
              Expression: 
                ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'i = i + j')
                  Left: 
                    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
                  Right: 
                    IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, 'i + j')
                      Left: 
                        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
                      Right: 
                        ILocalReferenceExpression: j ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
            IExpressionStatement ([2] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... riteLine(j)')
              Expression: 
                IInvocationExpression (Sub System.Console.WriteLine(value As System.Int32)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... riteLine(j)')
                  Instance Receiver: 
                    null
                  Arguments(1):
                    IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, 'j')
                      ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
                      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      IExpressionStatement ([3] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... riteLine(i)')
        Expression: 
          IInvocationExpression (Sub System.Console.WriteLine(value As System.Int32)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... riteLine(i)')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, 'i')
                ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
]]>.Value

            VerifyOperationTreeForTest(Of WhileBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IWhileUntilLoopStatement_WhileIncrementInCondition()
            Dim source = <![CDATA[
Class Program
    Private Shared Sub Main(args As String())
        Dim i As Integer = 0
        While System.Threading.Interlocked.Increment(i) < 5'BIND:"While System.Threading.Interlocked.Increment(i) < 5"
            System.Console.WriteLine(i)
        End While
    End Sub
End Class
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IWhileLoopStatement (LoopKind.While) ([1] OperationKind.LoopStatement) (Syntax: WhileBlock, 'While Syste ... End While') (Parent: BlockStatement)
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.LessThan, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'System.Thre ... ment(i) < 5')
      Left: 
        IInvocationExpression (Function System.Threading.Interlocked.Increment(ByRef location As System.Int32) As System.Int32) ([0] OperationKind.InvocationExpression, Type: System.Int32) (Syntax: InvocationExpression, 'System.Thre ... ncrement(i)')
          Instance Receiver: 
            null
          Arguments(1):
            IArgument (ArgumentKind.Explicit, Matching Parameter: location) ([0] OperationKind.Argument) (Syntax: SimpleArgument, 'i')
              ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
              InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
              OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 5) (Syntax: NumericLiteralExpression, '5')
  Body: 
    IBlockStatement (1 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: WhileBlock, 'While Syste ... End While')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... riteLine(i)')
        Expression: 
          IInvocationExpression (Sub System.Console.WriteLine(value As System.Int32)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... riteLine(i)')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, 'i')
                ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
]]>.Value

            VerifyOperationTreeForTest(Of WhileBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IWhileUntilLoopStatement_WhileInfiniteLoop()
            Dim source = <![CDATA[
Class C
    Private Shared Sub Main(args As String())
        Dim i As Integer = 1
        While i > 0'BIND:"While i > 0"
            i += 1
        End While
    End Sub
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IWhileLoopStatement (LoopKind.While) ([1] OperationKind.LoopStatement) (Syntax: WhileBlock, 'While i > 0 ... End While') (Parent: BlockStatement)
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanExpression, 'i > 0')
      Left: 
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  Body: 
    IBlockStatement (1 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: WhileBlock, 'While i > 0 ... End While')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: AddAssignmentStatement, 'i += 1')
        Expression: 
          ICompoundAssignmentExpression (BinaryOperatorKind.Add, Checked) ([0] OperationKind.CompoundAssignmentExpression, Type: System.Int32) (Syntax: AddAssignmentStatement, 'i += 1')
            Left: 
              ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
]]>.Value

            VerifyOperationTreeForTest(Of WhileBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IWhileUntilLoopStatement_WhileConstantCheck()
            Dim source = <![CDATA[
Class Program
    Private Function foo() As Boolean
        Const b As Boolean = True
        While b = b'BIND:"While b = b"
            Return b
        End While
    End Function

End Class
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IWhileLoopStatement (LoopKind.While) ([1] OperationKind.LoopStatement) (Syntax: WhileBlock, 'While b = b ... End While') (Parent: BlockStatement)
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.Equals, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean, Constant: True) (Syntax: EqualsExpression, 'b = b')
      Left: 
        ILocalReferenceExpression: b ([0] OperationKind.LocalReferenceExpression, Type: System.Boolean, Constant: True) (Syntax: IdentifierName, 'b')
      Right: 
        ILocalReferenceExpression: b ([1] OperationKind.LocalReferenceExpression, Type: System.Boolean, Constant: True) (Syntax: IdentifierName, 'b')
  Body: 
    IBlockStatement (1 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: WhileBlock, 'While b = b ... End While')
      IReturnStatement ([0] OperationKind.ReturnStatement) (Syntax: ReturnStatement, 'Return b')
        ReturnedValue: 
          ILocalReferenceExpression: b ([0] OperationKind.LocalReferenceExpression, Type: System.Boolean, Constant: True) (Syntax: IdentifierName, 'b')
]]>.Value

            VerifyOperationTreeForTest(Of WhileBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IWhileUntilLoopStatement_WhileWithTryCatch()
            Dim source = <![CDATA[
Public Class TryCatchFinally
    Public Sub TryMethod()
        Dim x As SByte = 111, y As SByte
        While System.Math.Max(System.Threading.Interlocked.Decrement(x), x + 1) > 0'BIND:"While System.Math.Max(System.Threading.Interlocked.Decrement(x), x + 1) > 0"
            Try
                y = CSByte(x / 2)
            Finally
                Throw New System.Exception()
            End Try
        End While

    End Sub
End Class
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IWhileLoopStatement (LoopKind.While) ([1] OperationKind.LoopStatement, IsInvalid) (Syntax: WhileBlock, 'While Syste ... End While') (Parent: BlockStatement)
  Condition: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Boolean, IsInvalid, IsImplicit) (Syntax: GreaterThanExpression, 'System.Math ...  x + 1) > 0')
      Conversion: CommonConversion (Exists: False, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: ?, IsInvalid) (Syntax: GreaterThanExpression, 'System.Math ...  x + 1) > 0')
          Left: 
            IInvalidExpression ([0] OperationKind.InvalidExpression, Type: ?, IsInvalid) (Syntax: InvocationExpression, 'System.Math ... (x), x + 1)')
              Children(3):
                IOperation:  ([0] OperationKind.None) (Syntax: SimpleMemberAccessExpression, 'System.Math.Max')
                  Children(1):
                    IOperation:  ([0] OperationKind.None) (Syntax: SimpleMemberAccessExpression, 'System.Math')
                IInvalidExpression ([1] OperationKind.InvalidExpression, Type: ?, IsInvalid) (Syntax: InvocationExpression, 'System.Thre ... ecrement(x)')
                  Children(2):
                    IOperation:  ([0] OperationKind.None, IsInvalid) (Syntax: SimpleMemberAccessExpression, 'System.Thre ... d.Decrement')
                      Children(1):
                        IOperation:  ([0] OperationKind.None) (Syntax: SimpleMemberAccessExpression, 'System.Thre ... Interlocked')
                    ILocalReferenceExpression: x ([1] OperationKind.LocalReferenceExpression, Type: System.SByte) (Syntax: IdentifierName, 'x')
                IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([2] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, 'x + 1')
                  Left: 
                    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: IdentifierName, 'x')
                      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                      Operand: 
                        ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.SByte) (Syntax: IdentifierName, 'x')
                  Right: 
                    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
          Right: 
            ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  Body: 
    IBlockStatement (1 statements) ([1] OperationKind.BlockStatement, IsInvalid, IsImplicit) (Syntax: WhileBlock, 'While Syste ... End While')
      ITryStatement ([0] OperationKind.TryStatement) (Syntax: TryBlock, 'Try ... End Try')
        Body: 
          IBlockStatement (1 statements) ([0] OperationKind.BlockStatement, IsImplicit) (Syntax: TryBlock, 'Try ... End Try')
            IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'y = CSByte(x / 2)')
              Expression: 
                ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.SByte) (Syntax: SimpleAssignmentStatement, 'y = CSByte(x / 2)')
                  Left: 
                    ILocalReferenceExpression: y ([0] OperationKind.LocalReferenceExpression, Type: System.SByte) (Syntax: IdentifierName, 'y')
                  Right: 
                    IConversionExpression (Explicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.SByte) (Syntax: PredefinedCastExpression, 'CSByte(x / 2)')
                      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                      Operand: 
                        IBinaryOperatorExpression (BinaryOperatorKind.Divide, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Double) (Syntax: DivideExpression, 'x / 2')
                          Left: 
                            IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Double, IsImplicit) (Syntax: IdentifierName, 'x')
                              Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                              Operand: 
                                ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.SByte) (Syntax: IdentifierName, 'x')
                          Right: 
                            IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Double, Constant: 2, IsImplicit) (Syntax: NumericLiteralExpression, '2')
                              Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                              Operand: 
                                ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
        Catch clauses(0)
        Finally: 
          IBlockStatement (1 statements) ([1] OperationKind.BlockStatement) (Syntax: FinallyBlock, 'Finally ... Exception()')
            IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ThrowStatement, 'Throw New S ... Exception()')
              Expression: 
                IThrowExpression ([0] OperationKind.ThrowExpression, Type: System.Exception) (Syntax: ThrowStatement, 'Throw New S ... Exception()')
                  IObjectCreationExpression (Constructor: Sub System.Exception..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: System.Exception) (Syntax: ObjectCreationExpression, 'New System.Exception()')
                    Arguments(0)
                    Initializer: 
                      null
]]>.Value

            VerifyOperationTreeForTest(Of WhileBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IWhileUntilLoopStatement_DoWhileFuncCall()
            Dim source = <![CDATA[
Imports System

Class C
    Sub F()
        Do While G()'BIND:"Do While G()"
            Console.WriteLine(1)
        Loop
    End Sub

    Function G() As Boolean
        Return False
    End Function
End Class
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IDoLoopStatement (DoLoopKind: DoWhileTopLoop) (LoopKind.Do) ([0] OperationKind.LoopStatement) (Syntax: DoWhileLoopBlock, 'Do While G( ... Loop') (Parent: BlockStatement)
  Condition: 
    IInvocationExpression ( Function C.G() As System.Boolean) ([0] OperationKind.InvocationExpression, Type: System.Boolean) (Syntax: InvocationExpression, 'G()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C, IsImplicit) (Syntax: IdentifierName, 'G')
      Arguments(0)
  IgnoredCondition: 
    null
  Body: 
    IBlockStatement (1 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: DoWhileLoopBlock, 'Do While G( ... Loop')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'Console.WriteLine(1)')
        Expression: 
          IInvocationExpression (Sub System.Console.WriteLine(value As System.Int32)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Console.WriteLine(1)')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, '1')
                ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
]]>.Value

            VerifyOperationTreeForTest(Of DoLoopBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IWhileUntilLoopStatement_DoLoopWhileStatement()
            Dim source = <![CDATA[
Imports System

Class C
    Sub F()
        Do'BIND:"Do"
            Console.WriteLine(1)
        Loop While G()
    End Sub

    Function G() As Boolean
        Return False
    End Function
End Class
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IDoLoopStatement (DoLoopKind: DoWhileBottomLoop) (LoopKind.Do) ([0] OperationKind.LoopStatement) (Syntax: DoLoopWhileBlock, 'Do'BIND:"Do ... p While G()') (Parent: BlockStatement)
  Condition: 
    IInvocationExpression ( Function C.G() As System.Boolean) ([1] OperationKind.InvocationExpression, Type: System.Boolean) (Syntax: InvocationExpression, 'G()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C, IsImplicit) (Syntax: IdentifierName, 'G')
      Arguments(0)
  IgnoredCondition: 
    null
  Body: 
    IBlockStatement (1 statements) ([0] OperationKind.BlockStatement, IsImplicit) (Syntax: DoLoopWhileBlock, 'Do'BIND:"Do ... p While G()')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'Console.WriteLine(1)')
        Expression: 
          IInvocationExpression (Sub System.Console.WriteLine(value As System.Int32)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Console.WriteLine(1)')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, '1')
                ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
]]>.Value

            VerifyOperationTreeForTest(Of DoLoopBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IWhileUntilLoopStatement_DoLoopUntilStatement()
            Dim source = <![CDATA[
Class C
    Private X As Integer = 10
    Sub M(c As C)
        Do'BIND:"Do"
            X = X - 1
        Loop Until X < 0
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IDoLoopStatement (DoLoopKind: DoUntilBottomLoop) (LoopKind.Do) ([0] OperationKind.LoopStatement) (Syntax: DoLoopUntilBlock, 'Do'BIND:"Do ... Until X < 0') (Parent: BlockStatement)
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.LessThan, Checked) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'X < 0')
      Left: 
        IFieldReferenceExpression: C.X As System.Int32 ([0] OperationKind.FieldReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'X')
          Instance Receiver: 
            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C, IsImplicit) (Syntax: IdentifierName, 'X')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  IgnoredCondition: 
    null
  Body: 
    IBlockStatement (1 statements) ([0] OperationKind.BlockStatement, IsImplicit) (Syntax: DoLoopUntilBlock, 'Do'BIND:"Do ... Until X < 0')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'X = X - 1')
        Expression: 
          ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'X = X - 1')
            Left: 
              IFieldReferenceExpression: C.X As System.Int32 ([0] OperationKind.FieldReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'X')
                Instance Receiver: 
                  IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C, IsImplicit) (Syntax: IdentifierName, 'X')
            Right: 
              IBinaryOperatorExpression (BinaryOperatorKind.Subtract, Checked) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: SubtractExpression, 'X - 1')
                Left: 
                  IFieldReferenceExpression: C.X As System.Int32 ([0] OperationKind.FieldReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'X')
                    Instance Receiver: 
                      IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C, IsImplicit) (Syntax: IdentifierName, 'X')
                Right: 
                  ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of DoLoopBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IWhileUntilLoopStatement_WhileWithNot()
            Dim source = <![CDATA[
Imports System
Module M1
    Sub Main()
        Dim x As Integer
        Dim breakLoop As Boolean
        x = 1
        breakLoop = False
        While Not breakLoop'BIND:"While Not breakLoop"
            Console.WriteLine("Iterate {0}", x)
            breakLoop = True
        End While
    End Sub
End Module

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IWhileLoopStatement (LoopKind.While) ([4] OperationKind.LoopStatement) (Syntax: WhileBlock, 'While Not b ... End While') (Parent: BlockStatement)
  Condition: 
    IUnaryOperatorExpression (UnaryOperatorKind.Not, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.Boolean) (Syntax: NotExpression, 'Not breakLoop')
      Operand: 
        ILocalReferenceExpression: breakLoop ([0] OperationKind.LocalReferenceExpression, Type: System.Boolean) (Syntax: IdentifierName, 'breakLoop')
  Body: 
    IBlockStatement (2 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: WhileBlock, 'While Not b ... End While')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'Console.Wri ... te {0}", x)')
        Expression: 
          IInvocationExpression (Sub System.Console.WriteLine(format As System.String, arg0 As System.Object)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Console.Wri ... te {0}", x)')
            Instance Receiver: 
              null
            Arguments(2):
              IArgument (ArgumentKind.Explicit, Matching Parameter: format) ([0] OperationKind.Argument) (Syntax: SimpleArgument, '"Iterate {0}"')
                ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: "Iterate {0}") (Syntax: StringLiteralExpression, '"Iterate {0}"')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
              IArgument (ArgumentKind.Explicit, Matching Parameter: arg0) ([1] OperationKind.Argument) (Syntax: SimpleArgument, 'x')
                IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Object, IsImplicit) (Syntax: IdentifierName, 'x')
                  Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                  Operand: 
                    ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      IExpressionStatement ([1] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'breakLoop = True')
        Expression: 
          ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Boolean) (Syntax: SimpleAssignmentStatement, 'breakLoop = True')
            Left: 
              ILocalReferenceExpression: breakLoop ([0] OperationKind.LocalReferenceExpression, Type: System.Boolean) (Syntax: IdentifierName, 'breakLoop')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Boolean, Constant: True) (Syntax: TrueLiteralExpression, 'True')
]]>.Value
            VerifyOperationTreeForTest(Of WhileBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IWhileUntilLoopStatement_DoWhileWithTopAndBottomCondition()
            Dim source = <![CDATA[
Class C
    Sub M(i As Integer)
        Do While i > 0'BIND:"Do While i > 0"
            i = i + 1
        Loop Until i <= 0

    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IDoLoopStatement (DoLoopKind: None) (LoopKind.Do) ([0] OperationKind.LoopStatement, IsInvalid) (Syntax: DoWhileLoopBlock, 'Do While i  ... ntil i <= 0') (Parent: BlockStatement)
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanExpression, 'i > 0')
      Left: 
        IParameterReferenceExpression: i ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  IgnoredCondition: 
    IBinaryOperatorExpression (BinaryOperatorKind.LessThanOrEqual, Checked) ([2] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanOrEqualExpression, 'i <= 0')
      Left: 
        IParameterReferenceExpression: i ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  Body: 
    IBlockStatement (1 statements) ([1] OperationKind.BlockStatement, IsInvalid, IsImplicit) (Syntax: DoWhileLoopBlock, 'Do While i  ... ntil i <= 0')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'i = i + 1')
        Expression: 
          ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'i = i + 1')
            Left: 
              IParameterReferenceExpression: i ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
            Right: 
              IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, 'i + 1')
                Left: 
                  IParameterReferenceExpression: i ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
                Right: 
                  ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30238: 'Loop' cannot have a condition if matching 'Do' has one.
        Loop Until i <= 0
             ~~~~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of DoLoopBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub
    End Class
End Namespace
