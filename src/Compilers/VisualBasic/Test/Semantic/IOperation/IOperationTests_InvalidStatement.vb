' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports Microsoft.CodeAnalysis.Semantics
Imports Microsoft.CodeAnalysis.Test.Utilities
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Test.Utilities

Namespace Microsoft.CodeAnalysis.VisualBasic.UnitTests.Semantics

    Partial Public Class IOperationTests
        Inherits SemanticModelTestBase

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(17607, "https://github.com/dotnet/roslyn/issues/17607")>
        Public Sub InvalidVariableDeclarationStatement()
            Dim source = <![CDATA[
Class Program
    Public Shared Sub Main(args As String())
        Dim x, 1 As Integer'BIND:"Dim x, 1 As Integer"
    End Sub
End Class
]]>.Value

            Dim expectedOperationTree = <![CDATA[
IVariableDeclarationStatement (2 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: LocalDeclarationStatement, 'Dim x, 1 As Integer') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: ModifiedIdentifier, 'x')
    Variables: Local_1: x As System.Int32
    Initializer: 
      null
  IVariableDeclaration (1 variables) ([1] OperationKind.VariableDeclaration, IsInvalid) (Syntax: ModifiedIdentifier, '')
    Variables: Local_1:  As System.Int32
    Initializer: 
      null
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC42024: Unused local variable: 'x'.
        Dim x, 1 As Integer'BIND:"Dim x, 1 As Integer"
            ~
BC30203: Identifier expected.
        Dim x, 1 As Integer'BIND:"Dim x, 1 As Integer"
               ~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of LocalDeclarationStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(17607, "https://github.com/dotnet/roslyn/issues/17607")>
        Public Sub InvalidSwitchStatementExpression()
            Dim source = <![CDATA[
Class Program
    Public Shared Sub Main(args As String())
        Select Case Program'BIND:"Select Case Program"
            Case 1
        End Select
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
ISwitchStatement (1 cases) ([0] OperationKind.SwitchStatement, IsInvalid) (Syntax: SelectBlock, 'Select Case ... End Select') (Parent: BlockStatement)
  Switch expression: 
    IOperation:  ([0] OperationKind.None, IsInvalid) (Syntax: IdentifierName, 'Program')
  Sections:
    ISwitchCase (1 case clauses, 1 statements) ([1] OperationKind.SwitchCase) (Syntax: CaseBlock, 'Case 1')
        Clauses:
          ISingleValueCaseClause (CaseKind.SingleValue) ([0] OperationKind.CaseClause) (Syntax: SimpleCaseClause, '1')
            Value: 
              null
        Body:
          IBlockStatement (0 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: CaseBlock, 'Case 1')
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30109: 'Program' is a class type and cannot be used as an expression.
        Select Case Program'BIND:"Select Case Program"
                    ~~~~~~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of SelectBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(17607, "https://github.com/dotnet/roslyn/issues/17607")>
        Public Sub InvalidSwitchStatementCaseLabel()
            Dim source = <![CDATA[
Class Program
    Public Shared Sub Main(args As String())
        Dim x = New Program()
        Select Case x.ToString()'BIND:"Select Case x.ToString()"
            Case x
                Exit Select
        End Select
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
ISwitchStatement (1 cases) ([1] OperationKind.SwitchStatement, IsInvalid) (Syntax: SelectBlock, 'Select Case ... End Select') (Parent: BlockStatement)
  Switch expression: 
    IInvocationExpression (virtual Function System.Object.ToString() As System.String) ([0] OperationKind.InvocationExpression, Type: System.String) (Syntax: InvocationExpression, 'x.ToString()')
      Instance Receiver: 
        ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: Program) (Syntax: IdentifierName, 'x')
      Arguments(0)
  Sections:
    ISwitchCase (1 case clauses, 1 statements) ([1] OperationKind.SwitchCase, IsInvalid) (Syntax: CaseBlock, 'Case x ... Exit Select')
        Clauses:
          ISingleValueCaseClause (CaseKind.SingleValue) ([0] OperationKind.CaseClause, IsInvalid) (Syntax: SimpleCaseClause, 'x')
            Value: 
              null
        Body:
          IBlockStatement (1 statements) ([1] OperationKind.BlockStatement, IsInvalid, IsImplicit) (Syntax: CaseBlock, 'Case x ... Exit Select')
            IBranchStatement (BranchKind.Break, Label: exit) ([0] OperationKind.BranchStatement) (Syntax: ExitSelectStatement, 'Exit Select')
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30311: Value of type 'Program' cannot be converted to 'String'.
            Case x
                 ~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of SelectBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(17607, "https://github.com/dotnet/roslyn/issues/17607")>
        Public Sub InvalidIfStatement()
            Dim source = <![CDATA[
Class Program
    Public Shared Sub Main(args As String())
        Dim x = New Program()
        If x = Nothing Then'BIND:"If x = Nothing Then"
        End If
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IIfStatement ([1] OperationKind.IfStatement, IsInvalid) (Syntax: MultiLineIfBlock, 'If x = Noth ... End If') (Parent: BlockStatement)
  Condition: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Boolean, IsInvalid, IsImplicit) (Syntax: EqualsExpression, 'x = Nothing')
      Conversion: CommonConversion (Exists: False, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IBinaryOperatorExpression (BinaryOperatorKind.Equals, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: ?, IsInvalid) (Syntax: EqualsExpression, 'x = Nothing')
          Left: 
            ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: Program, IsInvalid) (Syntax: IdentifierName, 'x')
          Right: 
            IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: Program, Constant: null, IsInvalid, IsImplicit) (Syntax: NothingLiteralExpression, 'Nothing')
              Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
              Operand: 
                ILiteralExpression ([0] OperationKind.LiteralExpression, Type: null, Constant: null, IsInvalid) (Syntax: NothingLiteralExpression, 'Nothing')
  IfTrue: 
    IBlockStatement (0 statements) ([1] OperationKind.BlockStatement, IsInvalid, IsImplicit) (Syntax: MultiLineIfBlock, 'If x = Noth ... End If')
  IfFalse: 
    null
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30452: Operator '=' is not defined for types 'Program' and 'Program'.
        If x = Nothing Then'BIND:"If x = Nothing Then"
           ~~~~~~~~~~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of MultiLineIfBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(17607, "https://github.com/dotnet/roslyn/issues/17607")>
        Public Sub InvalidIfElseIfStatement()
            Dim source = <![CDATA[
Class Program
    Public Shared Sub Main(args As String())
        Dim x = New Program()
        If Then'BIND:"If Then"
        ElseIf x Then
            x
        Else
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IIfStatement ([1] OperationKind.IfStatement, IsInvalid) (Syntax: MultiLineIfBlock, 'If Then'BIN ... Else') (Parent: BlockStatement)
  Condition: 
    IInvalidExpression ([0] OperationKind.InvalidExpression, Type: null, IsInvalid) (Syntax: IdentifierName, '')
      Children(0)
  IfTrue: 
    IBlockStatement (0 statements) ([1] OperationKind.BlockStatement, IsInvalid, IsImplicit) (Syntax: MultiLineIfBlock, 'If Then'BIN ... Else')
  IfFalse: 
    IIfStatement ([2] OperationKind.IfStatement, IsInvalid) (Syntax: ElseIfBlock, 'ElseIf x Th ... x')
      Condition: 
        IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Boolean, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'x')
          Conversion: CommonConversion (Exists: False, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: Program, IsInvalid) (Syntax: IdentifierName, 'x')
      IfTrue: 
        IBlockStatement (1 statements) ([1] OperationKind.BlockStatement, IsInvalid, IsImplicit) (Syntax: ElseIfBlock, 'ElseIf x Th ... x')
          IExpressionStatement ([0] OperationKind.ExpressionStatement, IsInvalid) (Syntax: ExpressionStatement, 'x')
            Expression: 
              IInvalidExpression ([0] OperationKind.InvalidExpression, Type: ?, IsInvalid) (Syntax: InvocationExpression, 'x')
                Children(1):
                  ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: Program, IsInvalid) (Syntax: IdentifierName, 'x')
      IfFalse: 
        IBlockStatement (0 statements) ([2] OperationKind.BlockStatement) (Syntax: ElseBlock, 'Else')
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30081: 'If' must end with a matching 'End If'.
        If Then'BIND:"If Then"
        ~~~~~~~
BC30201: Expression expected.
        If Then'BIND:"If Then"
           ~
BC30311: Value of type 'Program' cannot be converted to 'Boolean'.
        ElseIf x Then
               ~
BC30454: Expression is not a method.
            x
            ~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of MultiLineIfBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(17607, "https://github.com/dotnet/roslyn/issues/17607")>
        Public Sub InvalidForStatement_MissingConditionAndStep()
            Dim source = <![CDATA[
Module Program
    Sub Main(args As String())
        For i As Integer = 0'BIND:"For i As Integer = 0"
        Next i
    End Sub
End Module
]]>.Value

            Dim expectedOperationTree = <![CDATA[
IForToLoopStatement (LoopKind.ForTo) ([0] OperationKind.LoopStatement, IsInvalid) (Syntax: ForBlock, 'For i As In ... Next i') (Parent: BlockStatement)
  Locals: Local_1: i As System.Int32
  LoopControlVariable: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: VariableDeclarator, 'i As Integer')
  InitialValue: 
    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0, IsInvalid) (Syntax: NumericLiteralExpression, '0')
  LimitValue: 
    IInvalidExpression ([2] OperationKind.InvalidExpression, Type: null, IsInvalid) (Syntax: IdentifierName, '')
      Children(0)
  StepValue: 
    IConversionExpression (Explicit, TryCast: False, Unchecked) ([3] OperationKind.ConversionExpression, Type: System.Int32, Constant: 1, IsInvalid, IsImplicit) (Syntax: ForBlock, 'For i As In ... Next i')
      Conversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsInvalid, IsImplicit) (Syntax: ForBlock, 'For i As In ... Next i')
  Body: 
    IBlockStatement (0 statements) ([4] OperationKind.BlockStatement, IsInvalid, IsImplicit) (Syntax: ForBlock, 'For i As In ... Next i')
  NextVariables(1):
    ILocalReferenceExpression: i ([5] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30035: Syntax error.
        For i As Integer = 0'BIND:"For i As Integer = 0"
                            ~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of ForBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(17607, "https://github.com/dotnet/roslyn/issues/17607")>
        Public Sub InvalidForStatement_MissingConditionAndInitialization()
            Dim source = <![CDATA[
Module Program
    Sub Main(args As String())
        For Step (Method() + 1)'BIND:"For Step (Method() + 1)"
        Next
    End Sub

    Private Function Method() As Integer
        Return 0
    End Function
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IForToLoopStatement (LoopKind.ForTo) ([0] OperationKind.LoopStatement, IsInvalid) (Syntax: ForBlock, 'For Step (M ... Next') (Parent: BlockStatement)
  Locals: Local_1:  As System.Object
  LoopControlVariable: 
    ILocalReferenceExpression:  ([0] OperationKind.LocalReferenceExpression, Type: System.Object, IsInvalid) (Syntax: IdentifierName, '')
  InitialValue: 
    IInvalidExpression ([1] OperationKind.InvalidExpression, Type: null, IsInvalid) (Syntax: IdentifierName, '')
      Children(0)
  LimitValue: 
    IInvalidExpression ([2] OperationKind.InvalidExpression, Type: null, IsInvalid) (Syntax: IdentifierName, '')
      Children(0)
  StepValue: 
    IConversionExpression (Explicit, TryCast: False, Unchecked) ([3] OperationKind.ConversionExpression, Type: System.Object, IsInvalid, IsImplicit) (Syntax: ForBlock, 'For Step (M ... Next')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsInvalid, IsImplicit) (Syntax: ForBlock, 'For Step (M ... Next')
  Body: 
    IBlockStatement (0 statements) ([4] OperationKind.BlockStatement, IsInvalid, IsImplicit) (Syntax: ForBlock, 'For Step (M ... Next')
  NextVariables(0)
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30035: Syntax error.
        For Step (Method() + 1)'BIND:"For Step (Method() + 1)"
            ~
BC30035: Syntax error.
        For Step (Method() + 1)'BIND:"For Step (Method() + 1)"
            ~
BC30201: Expression expected.
        For Step (Method() + 1)'BIND:"For Step (Method() + 1)"
            ~
BC30249: '=' expected.
        For Step (Method() + 1)'BIND:"For Step (Method() + 1)"
            ~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of ForBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(17607, "https://github.com/dotnet/roslyn/issues/17607")>
        Public Sub InvalidForStatement_InvalidConditionAndStep()
            Dim source = <![CDATA[
Module Program
    Sub Main(args As String())
        For i As Integer = 0 To Program Step x'BIND:"For i As Integer = 0 To Program Step x"
        Next i
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IForToLoopStatement (LoopKind.ForTo) ([0] OperationKind.LoopStatement, IsInvalid) (Syntax: ForBlock, 'For i As In ... Next i') (Parent: BlockStatement)
  Locals: Local_1: i As System.Int32
  LoopControlVariable: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: VariableDeclarator, 'i As Integer')
  InitialValue: 
    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  LimitValue: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([2] OperationKind.ConversionExpression, Type: System.Int32, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'Program')
      Conversion: CommonConversion (Exists: False, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IOperation:  ([0] OperationKind.None, IsInvalid) (Syntax: IdentifierName, 'Program')
  StepValue: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([3] OperationKind.ConversionExpression, Type: System.Int32, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'x')
      Conversion: CommonConversion (Exists: False, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IInvalidExpression ([0] OperationKind.InvalidExpression, Type: ?, IsInvalid) (Syntax: IdentifierName, 'x')
          Children(0)
  Body: 
    IBlockStatement (0 statements) ([4] OperationKind.BlockStatement, IsInvalid, IsImplicit) (Syntax: ForBlock, 'For i As In ... Next i')
  NextVariables(1):
    ILocalReferenceExpression: i ([5] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30108: 'Program' is a type and cannot be used as an expression.
        For i As Integer = 0 To Program Step x'BIND:"For i As Integer = 0 To Program Step x"
                                ~~~~~~~
BC30451: 'x' is not declared. It may be inaccessible due to its protection level.
        For i As Integer = 0 To Program Step x'BIND:"For i As Integer = 0 To Program Step x"
                                             ~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of ForBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(17607, "https://github.com/dotnet/roslyn/issues/17607")>
        Public Sub InvalidGotoStatement_MissingLabel()
            Dim source = <![CDATA[
Module Program
    Sub Main(args As String())
        Select Case args.Length
            Case 1
                GoTo Label1'BIND:"GoTo Label1"
        End Select
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IInvalidStatement ([0] OperationKind.InvalidStatement, IsInvalid) (Syntax: GoToStatement, 'GoTo Label1') (Parent: BlockStatement)
  Children(1):
    IInvalidExpression ([0] OperationKind.InvalidExpression, Type: null, IsInvalid) (Syntax: IdentifierLabel, 'Label1')
      Children(0)
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30132: Label 'Label1' is not defined.
                GoTo Label1'BIND:"GoTo Label1"
                     ~~~~~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of GoToStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(17607, "https://github.com/dotnet/roslyn/issues/17607")>
        Public Sub InvalidExitStatement_OutsideLoopOrSwitch()
            Dim source = <![CDATA[
Module Program
    Sub Main(args As String())
        Exit For'BIND:"Exit For"
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IInvalidStatement ([0] OperationKind.InvalidStatement, IsInvalid) (Syntax: ExitForStatement, 'Exit For') (Parent: BlockStatement)
  Children(0)
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30096: 'Exit For' can only appear inside a 'For' statement.
        Exit For'BIND:"Exit For"
        ~~~~~~~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of ExitStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(17607, "https://github.com/dotnet/roslyn/issues/17607")>
        Public Sub InvalidContinueStatement_OutsideLoopOrSwitch()
            Dim source = <![CDATA[
Module Program
    Sub Main(args As String())
        Continue'BIND:"Continue"
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IInvalidStatement ([0] OperationKind.InvalidStatement, IsInvalid) (Syntax: ContinueDoStatement, 'Continue') (Parent: BlockStatement)
  Children(0)
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30782: 'Continue Do' can only appear inside a 'Do' statement.
        Continue'BIND:"Continue"
        ~~~~~~~~
BC30781: 'Continue' must be followed by 'Do', 'For' or 'While'.
        Continue'BIND:"Continue"
                ~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of ContinueStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(17607, "https://github.com/dotnet/roslyn/issues/17607")>
        Public Sub InvalidCaseStatement_OutsideSwitchBlock()
            Dim source = <![CDATA[
Module Program
    Sub Main(args As String())
        Case 0'BIND:"Case 0"
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IOperation:  ([0] OperationKind.None, IsInvalid) (Syntax: CaseStatement, 'Case 0') (Parent: InvalidStatement)
  Children(1):
    ISingleValueCaseClause (CaseKind.SingleValue) ([0] OperationKind.CaseClause, IsInvalid) (Syntax: SimpleCaseClause, '0')
      Value: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0, IsInvalid) (Syntax: NumericLiteralExpression, '0')
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30072: 'Case' can only appear inside a 'Select Case' statement.
        Case 0'BIND:"Case 0"
        ~~~~~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of CaseStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact, WorkItem(17607, "https://github.com/dotnet/roslyn/issues/17607")>
        Public Sub InvalidElseIfStatement_NoPrecedingIfStatement()
            Dim source = <![CDATA[
Imports System
Imports System.Collections.Generic
Imports System.Linq

Module Program
    Sub Main(args As String())
        ElseIf args.Length = 0'BIND:"ElseIf args.Length = 0"
    End Sub
End Module]]>.Value

Dim expectedOperationTree = <![CDATA[
IInvalidStatement ([0] OperationKind.InvalidStatement, IsInvalid) (Syntax: ElseIfStatement, 'ElseIf args.Length = 0') (Parent: BlockStatement)
  Children(1):
    IBinaryOperatorExpression (BinaryOperatorKind.Equals, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean, IsInvalid) (Syntax: EqualsExpression, 'args.Length = 0')
      Left: 
        IPropertyReferenceExpression: ReadOnly Property System.Array.Length As System.Int32 ([0] OperationKind.PropertyReferenceExpression, Type: System.Int32, IsInvalid) (Syntax: SimpleMemberAccessExpression, 'args.Length')
          Instance Receiver: 
            IParameterReferenceExpression: args ([0] OperationKind.ParameterReferenceExpression, Type: System.String(), IsInvalid) (Syntax: IdentifierName, 'args')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0, IsInvalid) (Syntax: NumericLiteralExpression, '0')
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC36005: 'ElseIf' must be preceded by a matching 'If' or 'ElseIf'.
        ElseIf args.Length = 0'BIND:"ElseIf args.Length = 0"
        ~~~~~~~~~~~~~~~~~~~~~~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of ElseIfStatementSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub
    End Class
End Namespace
