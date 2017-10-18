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
        Public Sub IForLoopStatement_SimpleForLoopsTest()
            Dim source = <![CDATA[
Public Class MyClass1
    Public Shared Sub Main()
        Dim myarray As Integer() = New Integer(2) {1, 2, 3}
        For i As Integer = 0 To myarray.Length - 1'BIND:"For i As Integer = 0 To myarray.Length - 1"
            System.Console.WriteLine(myarray(i))
        Next
    End Sub
End Class
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IForToLoopStatement (LoopKind.ForTo) ([1] OperationKind.LoopStatement) (Syntax: ForBlock, 'For i As In ... Next') (Parent: BlockStatement)
  Locals: Local_1: i As System.Int32
  LoopControlVariable: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: VariableDeclarator, 'i As Integer')
  InitialValue: 
    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  LimitValue: 
    IBinaryOperatorExpression (BinaryOperatorKind.Subtract, Checked) ([2] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: SubtractExpression, 'myarray.Length - 1')
      Left: 
        IPropertyReferenceExpression: ReadOnly Property System.Array.Length As System.Int32 ([0] OperationKind.PropertyReferenceExpression, Type: System.Int32) (Syntax: SimpleMemberAccessExpression, 'myarray.Length')
          Instance Receiver: 
            ILocalReferenceExpression: myarray ([0] OperationKind.LocalReferenceExpression, Type: System.Int32()) (Syntax: IdentifierName, 'myarray')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
  StepValue: 
    IConversionExpression (Explicit, TryCast: False, Unchecked) ([3] OperationKind.ConversionExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For i As In ... Next')
      Conversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For i As In ... Next')
  Body: 
    IBlockStatement (1 statements) ([4] OperationKind.BlockStatement, IsImplicit) (Syntax: ForBlock, 'For i As In ... Next')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... myarray(i))')
        Expression: 
          IInvocationExpression (Sub System.Console.WriteLine(value As System.Int32)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... myarray(i))')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, 'myarray(i)')
                IArrayElementReferenceExpression ([0] OperationKind.ArrayElementReferenceExpression, Type: System.Int32) (Syntax: InvocationExpression, 'myarray(i)')
                  Array reference: 
                    ILocalReferenceExpression: myarray ([0] OperationKind.LocalReferenceExpression, Type: System.Int32()) (Syntax: IdentifierName, 'myarray')
                  Indices(1):
                    ILocalReferenceExpression: i ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  NextVariables(0)
]]>.Value

            VerifyOperationTreeForTest(Of ForBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IForLoopStatement_SimpleForLoopsTestConversion()
            Dim source = <![CDATA[
Option Strict Off
Public Class MyClass1
    Public Shared Sub Main()
        Dim myarray As Integer() = New Integer(1) {}
        myarray(0) = 1
        myarray(1) = 2

        Dim s As Double = 1.1

        For i As Integer = 0 To "1" Step s'BIND:"For i As Integer = 0 To "1" Step s"
            System.Console.WriteLine(myarray(i))
        Next

    End Sub
End Class
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IForToLoopStatement (LoopKind.ForTo) ([4] OperationKind.LoopStatement) (Syntax: ForBlock, 'For i As In ... Next') (Parent: BlockStatement)
  Locals: Local_1: i As System.Int32
  LoopControlVariable: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: VariableDeclarator, 'i As Integer')
  InitialValue: 
    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  LimitValue: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([2] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: StringLiteralExpression, '"1"')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: "1") (Syntax: StringLiteralExpression, '"1"')
  StepValue: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([3] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: IdentifierName, 's')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILocalReferenceExpression: s ([0] OperationKind.LocalReferenceExpression, Type: System.Double) (Syntax: IdentifierName, 's')
  Body: 
    IBlockStatement (1 statements) ([4] OperationKind.BlockStatement, IsImplicit) (Syntax: ForBlock, 'For i As In ... Next')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... myarray(i))')
        Expression: 
          IInvocationExpression (Sub System.Console.WriteLine(value As System.Int32)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... myarray(i))')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, 'myarray(i)')
                IArrayElementReferenceExpression ([0] OperationKind.ArrayElementReferenceExpression, Type: System.Int32) (Syntax: InvocationExpression, 'myarray(i)')
                  Array reference: 
                    ILocalReferenceExpression: myarray ([0] OperationKind.LocalReferenceExpression, Type: System.Int32()) (Syntax: IdentifierName, 'myarray')
                  Indices(1):
                    ILocalReferenceExpression: i ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  NextVariables(0)
]]>.Value

            VerifyOperationTreeForTest(Of ForBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IForLoopStatement_ForLoopStepIsFloatNegativeVar()
            Dim source = <![CDATA[
Option Strict On
Public Class MyClass1
    Public Shared Sub Main()
        Dim s As Double = -1.1

        For i As Double = 2 To 0 Step s'BIND:"For i As Double = 2 To 0 Step s"
            System.Console.WriteLine(i)
        Next

    End Sub
End Class
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IForToLoopStatement (LoopKind.ForTo) ([1] OperationKind.LoopStatement) (Syntax: ForBlock, 'For i As Do ... Next') (Parent: BlockStatement)
  Locals: Local_1: i As System.Double
  LoopControlVariable: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Double) (Syntax: VariableDeclarator, 'i As Double')
  InitialValue: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Double, Constant: 2, IsImplicit) (Syntax: NumericLiteralExpression, '2')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
  LimitValue: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([2] OperationKind.ConversionExpression, Type: System.Double, Constant: 0, IsImplicit) (Syntax: NumericLiteralExpression, '0')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  StepValue: 
    ILocalReferenceExpression: s ([3] OperationKind.LocalReferenceExpression, Type: System.Double) (Syntax: IdentifierName, 's')
  Body: 
    IBlockStatement (1 statements) ([4] OperationKind.BlockStatement, IsImplicit) (Syntax: ForBlock, 'For i As Do ... Next')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... riteLine(i)')
        Expression: 
          IInvocationExpression (Sub System.Console.WriteLine(value As System.Double)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... riteLine(i)')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, 'i')
                ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Double) (Syntax: IdentifierName, 'i')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  NextVariables(0)
]]>.Value

            VerifyOperationTreeForTest(Of ForBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IForLoopStatement_ForLoopObject()
            Dim source = <![CDATA[
Option Strict On
Public Class MyClass1
    Public Shared Sub Main()

        Dim ctrlVar As Object
        Dim initValue As Object = 0
        Dim limit As Object = 2
        Dim stp As Object = 1

        For ctrlVar = initValue To limit Step stp'BIND:"For ctrlVar = initValue To limit Step stp"
            System.Console.WriteLine(ctrlVar)
        Next

    End Sub
End Class
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IForToLoopStatement (LoopKind.ForTo) ([4] OperationKind.LoopStatement) (Syntax: ForBlock, 'For ctrlVar ... Next') (Parent: BlockStatement)
  LoopControlVariable: 
    ILocalReferenceExpression: ctrlVar ([0] OperationKind.LocalReferenceExpression, Type: System.Object) (Syntax: IdentifierName, 'ctrlVar')
  InitialValue: 
    ILocalReferenceExpression: initValue ([1] OperationKind.LocalReferenceExpression, Type: System.Object) (Syntax: IdentifierName, 'initValue')
  LimitValue: 
    ILocalReferenceExpression: limit ([2] OperationKind.LocalReferenceExpression, Type: System.Object) (Syntax: IdentifierName, 'limit')
  StepValue: 
    ILocalReferenceExpression: stp ([3] OperationKind.LocalReferenceExpression, Type: System.Object) (Syntax: IdentifierName, 'stp')
  Body: 
    IBlockStatement (1 statements) ([4] OperationKind.BlockStatement, IsImplicit) (Syntax: ForBlock, 'For ctrlVar ... Next')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... ne(ctrlVar)')
        Expression: 
          IInvocationExpression (Sub System.Console.WriteLine(value As System.Object)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... ne(ctrlVar)')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, 'ctrlVar')
                ILocalReferenceExpression: ctrlVar ([0] OperationKind.LocalReferenceExpression, Type: System.Object) (Syntax: IdentifierName, 'ctrlVar')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  NextVariables(0)
]]>.Value

            VerifyOperationTreeForTest(Of ForBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IForLoopStatement_ForLoopNested()
            Dim source = <![CDATA[
Option Strict On
Option Infer On
Public Class MyClass1
    Public Shared Sub Main()
        For AVarName = 1 To 2'BIND:"For AVarName = 1 To 2"
            For B = 1 To 2
                For C = 1 To 2
                    For D = 1 To 2
                    Next D
                Next C
            Next B
        Next AVarName
    End Sub
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IForToLoopStatement (LoopKind.ForTo) ([0] OperationKind.LoopStatement) (Syntax: ForBlock, 'For AVarNam ... xt AVarName') (Parent: BlockStatement)
  Locals: Local_1: AVarName As System.Int32
  LoopControlVariable: 
    ILocalReferenceExpression: AVarName ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'AVarName')
  InitialValue: 
    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
  LimitValue: 
    ILiteralExpression ([2] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
  StepValue: 
    IConversionExpression (Explicit, TryCast: False, Unchecked) ([3] OperationKind.ConversionExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For AVarNam ... xt AVarName')
      Conversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For AVarNam ... xt AVarName')
  Body: 
    IBlockStatement (1 statements) ([4] OperationKind.BlockStatement, IsImplicit) (Syntax: ForBlock, 'For AVarNam ... xt AVarName')
      IForToLoopStatement (LoopKind.ForTo) ([0] OperationKind.LoopStatement) (Syntax: ForBlock, 'For B = 1 T ... Next B')
        Locals: Local_1: B As System.Int32
        LoopControlVariable: 
          ILocalReferenceExpression: B ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'B')
        InitialValue: 
          ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
        LimitValue: 
          ILiteralExpression ([2] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
        StepValue: 
          IConversionExpression (Explicit, TryCast: False, Unchecked) ([3] OperationKind.ConversionExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For B = 1 T ... Next B')
            Conversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            Operand: 
              ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For B = 1 T ... Next B')
        Body: 
          IBlockStatement (1 statements) ([4] OperationKind.BlockStatement, IsImplicit) (Syntax: ForBlock, 'For B = 1 T ... Next B')
            IForToLoopStatement (LoopKind.ForTo) ([0] OperationKind.LoopStatement) (Syntax: ForBlock, 'For C = 1 T ... Next C')
              Locals: Local_1: C As System.Int32
              LoopControlVariable: 
                ILocalReferenceExpression: C ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'C')
              InitialValue: 
                ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
              LimitValue: 
                ILiteralExpression ([2] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
              StepValue: 
                IConversionExpression (Explicit, TryCast: False, Unchecked) ([3] OperationKind.ConversionExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For C = 1 T ... Next C')
                  Conversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                  Operand: 
                    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For C = 1 T ... Next C')
              Body: 
                IBlockStatement (1 statements) ([4] OperationKind.BlockStatement, IsImplicit) (Syntax: ForBlock, 'For C = 1 T ... Next C')
                  IForToLoopStatement (LoopKind.ForTo) ([0] OperationKind.LoopStatement) (Syntax: ForBlock, 'For D = 1 T ... Next D')
                    Locals: Local_1: D As System.Int32
                    LoopControlVariable: 
                      ILocalReferenceExpression: D ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'D')
                    InitialValue: 
                      ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
                    LimitValue: 
                      ILiteralExpression ([2] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
                    StepValue: 
                      IConversionExpression (Explicit, TryCast: False, Unchecked) ([3] OperationKind.ConversionExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For D = 1 T ... Next D')
                        Conversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                        Operand: 
                          ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For D = 1 T ... Next D')
                    Body: 
                      IBlockStatement (0 statements) ([4] OperationKind.BlockStatement, IsImplicit) (Syntax: ForBlock, 'For D = 1 T ... Next D')
                    NextVariables(1):
                      ILocalReferenceExpression: D ([5] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'D')
              NextVariables(1):
                ILocalReferenceExpression: C ([5] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'C')
        NextVariables(1):
          ILocalReferenceExpression: B ([5] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'B')
  NextVariables(1):
    ILocalReferenceExpression: AVarName ([5] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'AVarName')
]]>.Value

            VerifyOperationTreeForTest(Of ForBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IForLoopStatement_ChangeOuterVarInInnerFor()
            Dim source = <![CDATA[
Option Strict On
Option Infer On
Public Class MyClass1
    Public Shared Sub Main()
        For I = 1 To 2'BIND:"For I = 1 To 2"
            For J = 1 To 2
                I = 3
                System.Console.WriteLine(I)
            Next
        Next
    End Sub
End Class


    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IForToLoopStatement (LoopKind.ForTo) ([0] OperationKind.LoopStatement) (Syntax: ForBlock, 'For I = 1 T ... Next') (Parent: BlockStatement)
  Locals: Local_1: I As System.Int32
  LoopControlVariable: 
    ILocalReferenceExpression: I ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'I')
  InitialValue: 
    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
  LimitValue: 
    ILiteralExpression ([2] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
  StepValue: 
    IConversionExpression (Explicit, TryCast: False, Unchecked) ([3] OperationKind.ConversionExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For I = 1 T ... Next')
      Conversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For I = 1 T ... Next')
  Body: 
    IBlockStatement (1 statements) ([4] OperationKind.BlockStatement, IsImplicit) (Syntax: ForBlock, 'For I = 1 T ... Next')
      IForToLoopStatement (LoopKind.ForTo) ([0] OperationKind.LoopStatement) (Syntax: ForBlock, 'For J = 1 T ... Next')
        Locals: Local_1: J As System.Int32
        LoopControlVariable: 
          ILocalReferenceExpression: J ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'J')
        InitialValue: 
          ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
        LimitValue: 
          ILiteralExpression ([2] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
        StepValue: 
          IConversionExpression (Explicit, TryCast: False, Unchecked) ([3] OperationKind.ConversionExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For J = 1 T ... Next')
            Conversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            Operand: 
              ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For J = 1 T ... Next')
        Body: 
          IBlockStatement (2 statements) ([4] OperationKind.BlockStatement, IsImplicit) (Syntax: ForBlock, 'For J = 1 T ... Next')
            IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: SimpleAssignmentStatement, 'I = 3')
              Expression: 
                ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentStatement, 'I = 3')
                  Left: 
                    ILocalReferenceExpression: I ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'I')
                  Right: 
                    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 3) (Syntax: NumericLiteralExpression, '3')
            IExpressionStatement ([1] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... riteLine(I)')
              Expression: 
                IInvocationExpression (Sub System.Console.WriteLine(value As System.Int32)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... riteLine(I)')
                  Instance Receiver: 
                    null
                  Arguments(1):
                    IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, 'I')
                      ILocalReferenceExpression: I ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'I')
                      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
        NextVariables(0)
  NextVariables(0)
]]>.Value

            VerifyOperationTreeForTest(Of ForBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IForLoopStatement_InnerForRefOuterForVar()
            Dim source = <![CDATA[
Option Strict On
Option Infer On
Public Class MyClass1
    Public Shared Sub Main()
        For I = 1 To 2'BIND:"For I = 1 To 2"
            For J = I + 1 To 2
                System.Console.WriteLine(J)
            Next
        Next
    End Sub
End Class


    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IForToLoopStatement (LoopKind.ForTo) ([0] OperationKind.LoopStatement) (Syntax: ForBlock, 'For I = 1 T ... Next') (Parent: BlockStatement)
  Locals: Local_1: I As System.Int32
  LoopControlVariable: 
    ILocalReferenceExpression: I ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'I')
  InitialValue: 
    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
  LimitValue: 
    ILiteralExpression ([2] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
  StepValue: 
    IConversionExpression (Explicit, TryCast: False, Unchecked) ([3] OperationKind.ConversionExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For I = 1 T ... Next')
      Conversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For I = 1 T ... Next')
  Body: 
    IBlockStatement (1 statements) ([4] OperationKind.BlockStatement, IsImplicit) (Syntax: ForBlock, 'For I = 1 T ... Next')
      IForToLoopStatement (LoopKind.ForTo) ([0] OperationKind.LoopStatement) (Syntax: ForBlock, 'For J = I + ... Next')
        Locals: Local_1: J As System.Int32
        LoopControlVariable: 
          ILocalReferenceExpression: J ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'J')
        InitialValue: 
          IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, 'I + 1')
            Left: 
              ILocalReferenceExpression: I ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'I')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
        LimitValue: 
          ILiteralExpression ([2] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
        StepValue: 
          IConversionExpression (Explicit, TryCast: False, Unchecked) ([3] OperationKind.ConversionExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For J = I + ... Next')
            Conversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            Operand: 
              ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For J = I + ... Next')
        Body: 
          IBlockStatement (1 statements) ([4] OperationKind.BlockStatement, IsImplicit) (Syntax: ForBlock, 'For J = I + ... Next')
            IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... riteLine(J)')
              Expression: 
                IInvocationExpression (Sub System.Console.WriteLine(value As System.Int32)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... riteLine(J)')
                  Instance Receiver: 
                    null
                  Arguments(1):
                    IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, 'J')
                      ILocalReferenceExpression: J ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'J')
                      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
        NextVariables(0)
  NextVariables(0)
]]>.Value

            VerifyOperationTreeForTest(Of ForBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IForLoopStatement_ExitNestedFor()
            Dim source = <![CDATA[
Option Strict On
Option Infer On
Public Class MyClass1
    Public Shared Sub Main()
        For I = 1 To 2'BIND:"For I = 1 To 2"
            For J = 1 To 2
                Exit For
            Next
            System.Console.WriteLine(I)
        Next
    End Sub
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IForToLoopStatement (LoopKind.ForTo) ([0] OperationKind.LoopStatement) (Syntax: ForBlock, 'For I = 1 T ... Next') (Parent: BlockStatement)
  Locals: Local_1: I As System.Int32
  LoopControlVariable: 
    ILocalReferenceExpression: I ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'I')
  InitialValue: 
    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
  LimitValue: 
    ILiteralExpression ([2] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
  StepValue: 
    IConversionExpression (Explicit, TryCast: False, Unchecked) ([3] OperationKind.ConversionExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For I = 1 T ... Next')
      Conversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For I = 1 T ... Next')
  Body: 
    IBlockStatement (2 statements) ([4] OperationKind.BlockStatement, IsImplicit) (Syntax: ForBlock, 'For I = 1 T ... Next')
      IForToLoopStatement (LoopKind.ForTo) ([0] OperationKind.LoopStatement) (Syntax: ForBlock, 'For J = 1 T ... Next')
        Locals: Local_1: J As System.Int32
        LoopControlVariable: 
          ILocalReferenceExpression: J ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'J')
        InitialValue: 
          ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
        LimitValue: 
          ILiteralExpression ([2] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
        StepValue: 
          IConversionExpression (Explicit, TryCast: False, Unchecked) ([3] OperationKind.ConversionExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For J = 1 T ... Next')
            Conversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            Operand: 
              ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For J = 1 T ... Next')
        Body: 
          IBlockStatement (1 statements) ([4] OperationKind.BlockStatement, IsImplicit) (Syntax: ForBlock, 'For J = 1 T ... Next')
            IBranchStatement (BranchKind.Break, Label: exit) ([0] OperationKind.BranchStatement) (Syntax: ExitForStatement, 'Exit For')
        NextVariables(0)
      IExpressionStatement ([1] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... riteLine(I)')
        Expression: 
          IInvocationExpression (Sub System.Console.WriteLine(value As System.Int32)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... riteLine(I)')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: SimpleArgument, 'I')
                ILocalReferenceExpression: I ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'I')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  NextVariables(0)
]]>.Value

            VerifyOperationTreeForTest(Of ForBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IForLoopStatement_EnumAsStart()
            Dim source = <![CDATA[
Option Strict Off
Option Infer Off
Public Class MyClass1
    Public Shared Sub Main()
        For x As e1 = e1.a To e1.c'BIND:"For x As e1 = e1.a To e1.c"
        Next
    End Sub
End Class
Enum e1
    a
    b
    c
End Enum
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IForToLoopStatement (LoopKind.ForTo) ([0] OperationKind.LoopStatement) (Syntax: ForBlock, 'For x As e1 ... Next') (Parent: BlockStatement)
  Locals: Local_1: x As e1
  LoopControlVariable: 
    ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: e1) (Syntax: VariableDeclarator, 'x As e1')
  InitialValue: 
    IFieldReferenceExpression: e1.a (Static) ([1] OperationKind.FieldReferenceExpression, Type: e1, Constant: 0) (Syntax: SimpleMemberAccessExpression, 'e1.a')
      Instance Receiver: 
        null
  LimitValue: 
    IFieldReferenceExpression: e1.c (Static) ([2] OperationKind.FieldReferenceExpression, Type: e1, Constant: 2) (Syntax: SimpleMemberAccessExpression, 'e1.c')
      Instance Receiver: 
        null
  StepValue: 
    IConversionExpression (Explicit, TryCast: False, Unchecked) ([3] OperationKind.ConversionExpression, Type: e1, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For x As e1 ... Next')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For x As e1 ... Next')
  Body: 
    IBlockStatement (0 statements) ([4] OperationKind.BlockStatement, IsImplicit) (Syntax: ForBlock, 'For x As e1 ... Next')
  NextVariables(0)
]]>.Value

            VerifyOperationTreeForTest(Of ForBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IForLoopStatement_PropertyAsStart()
            Dim source = <![CDATA[
Option Strict Off
Option Infer Off
Public Class MyClass1
    Property P1(ByVal x As Long) As Byte
        Get
            Return x - 10
        End Get
        Set(ByVal Value As Byte)
        End Set
    End Property
    Public Shared Sub Main()
    End Sub
    Public Sub F()
        For i As Integer = P1(30 + i) To 30'BIND:"For i As Integer = P1(30 + i) To 30"
        Next
    End Sub
End Class
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IForToLoopStatement (LoopKind.ForTo) ([0] OperationKind.LoopStatement) (Syntax: ForBlock, 'For i As In ... Next') (Parent: BlockStatement)
  Locals: Local_1: i As System.Int32
  LoopControlVariable: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: VariableDeclarator, 'i As Integer')
  InitialValue: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: InvocationExpression, 'P1(30 + i)')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IPropertyReferenceExpression: Property MyClass1.P1(x As System.Int64) As System.Byte ([0] OperationKind.PropertyReferenceExpression, Type: System.Byte) (Syntax: InvocationExpression, 'P1(30 + i)')
          Instance Receiver: 
            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: MyClass1, IsImplicit) (Syntax: IdentifierName, 'P1')
          Arguments(1):
            IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([1] OperationKind.Argument) (Syntax: SimpleArgument, '30 + i')
              IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int64, IsImplicit) (Syntax: AddExpression, '30 + i')
                Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                Operand: 
                  IBinaryOperatorExpression (BinaryOperatorKind.Add, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, '30 + i')
                    Left: 
                      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 30) (Syntax: NumericLiteralExpression, '30')
                    Right: 
                      ILocalReferenceExpression: i ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
              InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
              OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  LimitValue: 
    ILiteralExpression ([2] OperationKind.LiteralExpression, Type: System.Int32, Constant: 30) (Syntax: NumericLiteralExpression, '30')
  StepValue: 
    IConversionExpression (Explicit, TryCast: False, Unchecked) ([3] OperationKind.ConversionExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For i As In ... Next')
      Conversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For i As In ... Next')
  Body: 
    IBlockStatement (0 statements) ([4] OperationKind.BlockStatement, IsImplicit) (Syntax: ForBlock, 'For i As In ... Next')
  NextVariables(0)
]]>.Value

            VerifyOperationTreeForTest(Of ForBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IForLoopStatement_FieldNameAsIteration()
            Dim source = <![CDATA[
Option Strict Off
Option Infer On
Public Class MyClass1
    Dim global_x As Integer = 10
    Const global_y As Long = 20
    Public Shared Sub Main()
        For global_x As Integer = global_y To 10'BIND:"For global_x As Integer = global_y To 10"
        Next
    End Sub
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IForToLoopStatement (LoopKind.ForTo) ([0] OperationKind.LoopStatement) (Syntax: ForBlock, 'For global_ ... Next') (Parent: BlockStatement)
  Locals: Local_1: global_x As System.Int32
  LoopControlVariable: 
    ILocalReferenceExpression: global_x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: VariableDeclarator, 'global_x As Integer')
  InitialValue: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Int32, Constant: 20, IsImplicit) (Syntax: IdentifierName, 'global_y')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IFieldReferenceExpression: MyClass1.global_y As System.Int64 (Static) ([0] OperationKind.FieldReferenceExpression, Type: System.Int64, Constant: 20) (Syntax: IdentifierName, 'global_y')
          Instance Receiver: 
            null
  LimitValue: 
    ILiteralExpression ([2] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10) (Syntax: NumericLiteralExpression, '10')
  StepValue: 
    IConversionExpression (Explicit, TryCast: False, Unchecked) ([3] OperationKind.ConversionExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For global_ ... Next')
      Conversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For global_ ... Next')
  Body: 
    IBlockStatement (0 statements) ([4] OperationKind.BlockStatement, IsImplicit) (Syntax: ForBlock, 'For global_ ... Next')
  NextVariables(0)
]]>.Value

            VerifyOperationTreeForTest(Of ForBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IForLoopStatement_SingleLine()
            Dim source = <![CDATA[
Option Strict On
Public Class MyClass1
    Public Shared Sub Main()
        For x As Integer = 0 To 10 : Next'BIND:"For x As Integer = 0 To 10 : Next"
    End Sub
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IForToLoopStatement (LoopKind.ForTo) ([0] OperationKind.LoopStatement) (Syntax: ForBlock, 'For x As In ... o 10 : Next') (Parent: BlockStatement)
  Locals: Local_1: x As System.Int32
  LoopControlVariable: 
    ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: VariableDeclarator, 'x As Integer')
  InitialValue: 
    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  LimitValue: 
    ILiteralExpression ([2] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10) (Syntax: NumericLiteralExpression, '10')
  StepValue: 
    IConversionExpression (Explicit, TryCast: False, Unchecked) ([3] OperationKind.ConversionExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For x As In ... o 10 : Next')
      Conversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For x As In ... o 10 : Next')
  Body: 
    IBlockStatement (0 statements) ([4] OperationKind.BlockStatement, IsImplicit) (Syntax: ForBlock, 'For x As In ... o 10 : Next')
  NextVariables(0)
]]>.Value

            VerifyOperationTreeForTest(Of ForBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IForLoopStatement_VarDeclOutOfForeach()
            Dim source = <![CDATA[
Option Strict On
Option Infer On
Public Class MyClass1
    Public Shared Sub Main()
        Dim Y As Integer
        For Y = 1 To 2'BIND:"For Y = 1 To 2"
        Next
    End Sub
End Class
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IForToLoopStatement (LoopKind.ForTo) ([1] OperationKind.LoopStatement) (Syntax: ForBlock, 'For Y = 1 T ... Next') (Parent: BlockStatement)
  LoopControlVariable: 
    ILocalReferenceExpression: Y ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'Y')
  InitialValue: 
    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
  LimitValue: 
    ILiteralExpression ([2] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
  StepValue: 
    IConversionExpression (Explicit, TryCast: False, Unchecked) ([3] OperationKind.ConversionExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For Y = 1 T ... Next')
      Conversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For Y = 1 T ... Next')
  Body: 
    IBlockStatement (0 statements) ([4] OperationKind.BlockStatement, IsImplicit) (Syntax: ForBlock, 'For Y = 1 T ... Next')
  NextVariables(0)
]]>.Value

            VerifyOperationTreeForTest(Of ForBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IForLoopStatement_GetDeclaredSymbolOfForStatement()
            Dim source = <![CDATA[
Option Strict On
Option Infer On

Imports System
Imports System.Collection

Class C1
    Public Shared Sub Main()
        For element1 = 23 To 42'BIND:"For element1 = 23 To 42"
        Next

        For element2 As Integer = 23 To 42
        Next
    End Sub
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IForToLoopStatement (LoopKind.ForTo) ([0] OperationKind.LoopStatement) (Syntax: ForBlock, 'For element ... Next') (Parent: BlockStatement)
  Locals: Local_1: element1 As System.Int32
  LoopControlVariable: 
    ILocalReferenceExpression: element1 ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'element1')
  InitialValue: 
    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 23) (Syntax: NumericLiteralExpression, '23')
  LimitValue: 
    ILiteralExpression ([2] OperationKind.LiteralExpression, Type: System.Int32, Constant: 42) (Syntax: NumericLiteralExpression, '42')
  StepValue: 
    IConversionExpression (Explicit, TryCast: False, Unchecked) ([3] OperationKind.ConversionExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For element ... Next')
      Conversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For element ... Next')
  Body: 
    IBlockStatement (0 statements) ([4] OperationKind.BlockStatement, IsImplicit) (Syntax: ForBlock, 'For element ... Next')
  NextVariables(0)
]]>.Value

            VerifyOperationTreeForTest(Of ForBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IForLoopStatement_ForLoopContinue()
            Dim source = <![CDATA[
Option Strict On
Option Infer On

Imports System
Imports System.Collection

Class C1
    Public Shared Sub Main()
        For i As Integer = 0 To 5'BIND:"For i As Integer = 0 To 5"
            If i Mod 2 = 0 Then
                Continue For
            End If
        Next
    End Sub
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IForToLoopStatement (LoopKind.ForTo) ([0] OperationKind.LoopStatement) (Syntax: ForBlock, 'For i As In ... Next') (Parent: BlockStatement)
  Locals: Local_1: i As System.Int32
  LoopControlVariable: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: VariableDeclarator, 'i As Integer')
  InitialValue: 
    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  LimitValue: 
    ILiteralExpression ([2] OperationKind.LiteralExpression, Type: System.Int32, Constant: 5) (Syntax: NumericLiteralExpression, '5')
  StepValue: 
    IConversionExpression (Explicit, TryCast: False, Unchecked) ([3] OperationKind.ConversionExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For i As In ... Next')
      Conversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For i As In ... Next')
  Body: 
    IBlockStatement (1 statements) ([4] OperationKind.BlockStatement, IsImplicit) (Syntax: ForBlock, 'For i As In ... Next')
      IIfStatement ([0] OperationKind.IfStatement) (Syntax: MultiLineIfBlock, 'If i Mod 2  ... End If')
        Condition: 
          IBinaryOperatorExpression (BinaryOperatorKind.Equals, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: EqualsExpression, 'i Mod 2 = 0')
            Left: 
              IBinaryOperatorExpression (BinaryOperatorKind.Remainder, Checked) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: ModuloExpression, 'i Mod 2')
                Left: 
                  ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
                Right: 
                  ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
        IfTrue: 
          IBlockStatement (1 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: MultiLineIfBlock, 'If i Mod 2  ... End If')
            IBranchStatement (BranchKind.Continue, Label: continue) ([0] OperationKind.BranchStatement) (Syntax: ContinueForStatement, 'Continue For')
        IfFalse: 
          null
  NextVariables(0)
]]>.Value

            VerifyOperationTreeForTest(Of ForBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IForLoopStatement_ForReverse()
            Dim source = <![CDATA[
Option Infer On

Module Program
    Sub Main()
        For X = 10 To 0'BIND:"For X = 10 To 0"
        Next
    End Sub
End Module

Module M
    Public X As Integer
End Module

]]>.Value

            Dim expectedOperationTree = <![CDATA[
IForToLoopStatement (LoopKind.ForTo) ([0] OperationKind.LoopStatement) (Syntax: ForBlock, 'For X = 10  ... Next') (Parent: BlockStatement)
  LoopControlVariable: 
    IFieldReferenceExpression: M.X As System.Int32 (Static) ([0] OperationKind.FieldReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'X')
      Instance Receiver: 
        null
  InitialValue: 
    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10) (Syntax: NumericLiteralExpression, '10')
  LimitValue: 
    ILiteralExpression ([2] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  StepValue: 
    IConversionExpression (Explicit, TryCast: False, Unchecked) ([3] OperationKind.ConversionExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For X = 10  ... Next')
      Conversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For X = 10  ... Next')
  Body: 
    IBlockStatement (0 statements) ([4] OperationKind.BlockStatement, IsImplicit) (Syntax: ForBlock, 'For X = 10  ... Next')
  NextVariables(0)
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of ForBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IForLoopStatement_InValid()
            Dim source = <![CDATA[
Option Infer On

Module Program
    Sub Main()
        For X = 10 To 0'BIND:"For X = 10 To 0"
        Next
    End Sub
End Module

Module M
    Public X As String
End Module

]]>.Value

            Dim expectedOperationTree = <![CDATA[
IForToLoopStatement (LoopKind.ForTo) ([0] OperationKind.LoopStatement, IsInvalid) (Syntax: ForBlock, 'For X = 10  ... Next') (Parent: BlockStatement)
  LoopControlVariable: 
    IFieldReferenceExpression: M.X As System.String (Static) ([0] OperationKind.FieldReferenceExpression, Type: System.String, IsInvalid) (Syntax: IdentifierName, 'X')
      Instance Receiver: 
        null
  InitialValue: 
    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10) (Syntax: NumericLiteralExpression, '10')
  LimitValue: 
    ILiteralExpression ([2] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  StepValue: 
    ILiteralExpression ([3] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsInvalid, IsImplicit) (Syntax: ForBlock, 'For X = 10  ... Next')
  Body: 
    IBlockStatement (0 statements) ([4] OperationKind.BlockStatement, IsInvalid, IsImplicit) (Syntax: ForBlock, 'For X = 10  ... Next')
  NextVariables(0)
]]>.Value

            Dim expectedDiagnostics = <![CDATA[
BC30337: 'For' loop control variable cannot be of type 'String' because the type does not support the required operators.
        For X = 10 To 0'BIND:"For X = 10 To 0"
            ~
]]>.Value

            VerifyOperationTreeAndDiagnosticsForTest(Of ForBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")>
        Public Sub IForLoopStatement_ForCombined()
            Dim source = <![CDATA[
Option Infer On

Module Program
    Sub Main(args As String())
        For A = 1 To 2'BIND:"For A = 1 To 2"
            For B = A To 2
        Next B, A
    End Sub
End Module]]>.Value

            Dim expectedOperationTree = <![CDATA[
IForToLoopStatement (LoopKind.ForTo) ([0] OperationKind.LoopStatement) (Syntax: ForBlock, 'For A = 1 T ... Next B, A') (Parent: BlockStatement)
  Locals: Local_1: A As System.Int32
  LoopControlVariable: 
    ILocalReferenceExpression: A ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'A')
  InitialValue: 
    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
  LimitValue: 
    ILiteralExpression ([2] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
  StepValue: 
    IConversionExpression (Explicit, TryCast: False, Unchecked) ([3] OperationKind.ConversionExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For A = 1 T ... Next B, A')
      Conversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For A = 1 T ... Next B, A')
  Body: 
    IBlockStatement (1 statements) ([4] OperationKind.BlockStatement, IsImplicit) (Syntax: ForBlock, 'For A = 1 T ... Next B, A')
      IForToLoopStatement (LoopKind.ForTo) ([0] OperationKind.LoopStatement) (Syntax: ForBlock, 'For B = A T ... Next B, A')
        Locals: Local_1: B As System.Int32
        LoopControlVariable: 
          ILocalReferenceExpression: B ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'B')
        InitialValue: 
          ILocalReferenceExpression: A ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'A')
        LimitValue: 
          ILiteralExpression ([2] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
        StepValue: 
          IConversionExpression (Explicit, TryCast: False, Unchecked) ([3] OperationKind.ConversionExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For B = A T ... Next B, A')
            Conversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            Operand: 
              ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For B = A T ... Next B, A')
        Body: 
          IBlockStatement (0 statements) ([4] OperationKind.BlockStatement, IsImplicit) (Syntax: ForBlock, 'For B = A T ... Next B, A')
        NextVariables(2):
          ILocalReferenceExpression: B ([5] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'B')
          ILocalReferenceExpression: A ([6] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'A')
  NextVariables(0)
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of ForBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub VerifyForToLoop1()
            Dim source = <![CDATA[
Structure C
    Sub F()
        Dim x As Integer
        Dim y As Integer = 16
        For x = 12 To y 'BIND:"For x = 12 To y"
        Next
    End Sub
End Structure
]]>.Value

            Dim expectedOperationTree = <![CDATA[
IForToLoopStatement (LoopKind.ForTo) ([2] OperationKind.LoopStatement) (Syntax: ForBlock, 'For x = 12  ... Next') (Parent: BlockStatement)
  LoopControlVariable: 
    ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
  InitialValue: 
    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 12) (Syntax: NumericLiteralExpression, '12')
  LimitValue: 
    ILocalReferenceExpression: y ([2] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  StepValue: 
    IConversionExpression (Explicit, TryCast: False, Unchecked) ([3] OperationKind.ConversionExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For x = 12  ... Next')
      Conversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For x = 12  ... Next')
  Body: 
    IBlockStatement (0 statements) ([4] OperationKind.BlockStatement, IsImplicit) (Syntax: ForBlock, 'For x = 12  ... Next')
  NextVariables(0)
]]>.Value

            VerifyOperationTreeForTest(Of ForBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub VerifyForToLoop2()
            Dim source = <![CDATA[
Structure C
    Sub F()
        Dim x As Integer?
        Dim y As Integer = 16
        For x = 12 To y 'BIND:"For x = 12 To y"
        Next
    End Sub
End Structure
]]>.Value

            Dim expectedOperationTree = <![CDATA[
IForToLoopStatement (LoopKind.ForTo) ([2] OperationKind.LoopStatement) (Syntax: ForBlock, 'For x = 12  ... Next') (Parent: BlockStatement)
  LoopControlVariable: 
    ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Nullable(Of System.Int32)) (Syntax: IdentifierName, 'x')
  InitialValue: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Nullable(Of System.Int32), IsImplicit) (Syntax: NumericLiteralExpression, '12')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 12) (Syntax: NumericLiteralExpression, '12')
  LimitValue: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([2] OperationKind.ConversionExpression, Type: System.Nullable(Of System.Int32), IsImplicit) (Syntax: IdentifierName, 'y')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILocalReferenceExpression: y ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  StepValue: 
    IConversionExpression (Explicit, TryCast: False, Unchecked) ([3] OperationKind.ConversionExpression, Type: System.Nullable(Of System.Int32), IsImplicit) (Syntax: ForBlock, 'For x = 12  ... Next')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For x = 12  ... Next')
  Body: 
    IBlockStatement (0 statements) ([4] OperationKind.BlockStatement, IsImplicit) (Syntax: ForBlock, 'For x = 12  ... Next')
  NextVariables(0)
]]>.Value

            VerifyOperationTreeForTest(Of ForBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub VerifyForToLoop3()
            Dim source = <![CDATA[
Structure C
    Sub F()
        Dim x As Integer
        Dim y As Integer? = 16
        For x = 12 To y 'BIND:"For x = 12 To y"
        Next
    End Sub
End Structure
]]>.Value

            Dim expectedOperationTree = <![CDATA[
IForToLoopStatement (LoopKind.ForTo) ([2] OperationKind.LoopStatement) (Syntax: ForBlock, 'For x = 12  ... Next') (Parent: BlockStatement)
  LoopControlVariable: 
    ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
  InitialValue: 
    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 12) (Syntax: NumericLiteralExpression, '12')
  LimitValue: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([2] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: IdentifierName, 'y')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILocalReferenceExpression: y ([0] OperationKind.LocalReferenceExpression, Type: System.Nullable(Of System.Int32)) (Syntax: IdentifierName, 'y')
  StepValue: 
    IConversionExpression (Explicit, TryCast: False, Unchecked) ([3] OperationKind.ConversionExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For x = 12  ... Next')
      Conversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For x = 12  ... Next')
  Body: 
    IBlockStatement (0 statements) ([4] OperationKind.BlockStatement, IsImplicit) (Syntax: ForBlock, 'For x = 12  ... Next')
  NextVariables(0)
]]>.Value

            VerifyOperationTreeForTest(Of ForBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub VerifyForToLoop4()
            Dim source = <![CDATA[
Structure C
    Sub F()
        Dim x As Integer?
        Dim y As Integer? = 16
        For x = 12 To y 'BIND:"For x = 12 To y"
        Next
    End Sub
End Structure
]]>.Value

            Dim expectedOperationTree = <![CDATA[
IForToLoopStatement (LoopKind.ForTo) ([2] OperationKind.LoopStatement) (Syntax: ForBlock, 'For x = 12  ... Next') (Parent: BlockStatement)
  LoopControlVariable: 
    ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Nullable(Of System.Int32)) (Syntax: IdentifierName, 'x')
  InitialValue: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Nullable(Of System.Int32), IsImplicit) (Syntax: NumericLiteralExpression, '12')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 12) (Syntax: NumericLiteralExpression, '12')
  LimitValue: 
    ILocalReferenceExpression: y ([2] OperationKind.LocalReferenceExpression, Type: System.Nullable(Of System.Int32)) (Syntax: IdentifierName, 'y')
  StepValue: 
    IConversionExpression (Explicit, TryCast: False, Unchecked) ([3] OperationKind.ConversionExpression, Type: System.Nullable(Of System.Int32), IsImplicit) (Syntax: ForBlock, 'For x = 12  ... Next')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For x = 12  ... Next')
  Body: 
    IBlockStatement (0 statements) ([4] OperationKind.BlockStatement, IsImplicit) (Syntax: ForBlock, 'For x = 12  ... Next')
  NextVariables(0)
]]>.Value

            VerifyOperationTreeForTest(Of ForBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub VerifyForToLoop5()
            Dim source = <![CDATA[
Structure C
    Sub F()
        Dim x As Integer
        Dim y As Integer = 16
        Dim s As Integer? = nothing
        For x = 12 To y Step s 'BIND:"For x = 12 To y Step s"
        Next
    End Sub
End Structure
]]>.Value

            Dim expectedOperationTree = <![CDATA[
IForToLoopStatement (LoopKind.ForTo) ([3] OperationKind.LoopStatement) (Syntax: ForBlock, 'For x = 12  ... Next') (Parent: BlockStatement)
  LoopControlVariable: 
    ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
  InitialValue: 
    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 12) (Syntax: NumericLiteralExpression, '12')
  LimitValue: 
    ILocalReferenceExpression: y ([2] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  StepValue: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([3] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: IdentifierName, 's')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILocalReferenceExpression: s ([0] OperationKind.LocalReferenceExpression, Type: System.Nullable(Of System.Int32)) (Syntax: IdentifierName, 's')
  Body: 
    IBlockStatement (0 statements) ([4] OperationKind.BlockStatement, IsImplicit) (Syntax: ForBlock, 'For x = 12  ... Next')
  NextVariables(0)
]]>.Value

            VerifyOperationTreeForTest(Of ForBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub VerifyForToLoop6()
            Dim source = <![CDATA[
Structure C
    Sub F()
        Dim x As Integer?
        Dim y As Integer = 16
        Dim s As Integer? = nothing
        For x = 12 To y Step s 'BIND:"For x = 12 To y Step s"
        Next
    End Sub
End Structure
]]>.Value

            Dim expectedOperationTree = <![CDATA[
IForToLoopStatement (LoopKind.ForTo) ([3] OperationKind.LoopStatement) (Syntax: ForBlock, 'For x = 12  ... Next') (Parent: BlockStatement)
  LoopControlVariable: 
    ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Nullable(Of System.Int32)) (Syntax: IdentifierName, 'x')
  InitialValue: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Nullable(Of System.Int32), IsImplicit) (Syntax: NumericLiteralExpression, '12')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 12) (Syntax: NumericLiteralExpression, '12')
  LimitValue: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([2] OperationKind.ConversionExpression, Type: System.Nullable(Of System.Int32), IsImplicit) (Syntax: IdentifierName, 'y')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILocalReferenceExpression: y ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'y')
  StepValue: 
    ILocalReferenceExpression: s ([3] OperationKind.LocalReferenceExpression, Type: System.Nullable(Of System.Int32)) (Syntax: IdentifierName, 's')
  Body: 
    IBlockStatement (0 statements) ([4] OperationKind.BlockStatement, IsImplicit) (Syntax: ForBlock, 'For x = 12  ... Next')
  NextVariables(0)
]]>.Value

            VerifyOperationTreeForTest(Of ForBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub VerifyForToLoop7()
            Dim source = <![CDATA[
Structure C
    Sub F()
        Dim x As Integer
        Dim y As Integer? = 16
        Dim s As Integer? = nothing
        For x = 12 To y Step s 'BIND:"For x = 12 To y Step s"
        Next
    End Sub
End Structure
]]>.Value

            Dim expectedOperationTree = <![CDATA[
IForToLoopStatement (LoopKind.ForTo) ([3] OperationKind.LoopStatement) (Syntax: ForBlock, 'For x = 12  ... Next') (Parent: BlockStatement)
  LoopControlVariable: 
    ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
  InitialValue: 
    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 12) (Syntax: NumericLiteralExpression, '12')
  LimitValue: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([2] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: IdentifierName, 'y')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILocalReferenceExpression: y ([0] OperationKind.LocalReferenceExpression, Type: System.Nullable(Of System.Int32)) (Syntax: IdentifierName, 'y')
  StepValue: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([3] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: IdentifierName, 's')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILocalReferenceExpression: s ([0] OperationKind.LocalReferenceExpression, Type: System.Nullable(Of System.Int32)) (Syntax: IdentifierName, 's')
  Body: 
    IBlockStatement (0 statements) ([4] OperationKind.BlockStatement, IsImplicit) (Syntax: ForBlock, 'For x = 12  ... Next')
  NextVariables(0)
]]>.Value

            VerifyOperationTreeForTest(Of ForBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub VerifyForToLoop8()
            Dim source = <![CDATA[
Structure C
    Sub F()
        Dim x As Integer?
        Dim y As Integer? = 16
        Dim s As Integer? = nothing
        For x = 12 To y Step s 'BIND:"For x = 12 To y Step s"
        Next
    End Sub
End Structure
]]>.Value

            Dim expectedOperationTree = <![CDATA[
IForToLoopStatement (LoopKind.ForTo) ([3] OperationKind.LoopStatement) (Syntax: ForBlock, 'For x = 12  ... Next') (Parent: BlockStatement)
  LoopControlVariable: 
    ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.Nullable(Of System.Int32)) (Syntax: IdentifierName, 'x')
  InitialValue: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Nullable(Of System.Int32), IsImplicit) (Syntax: NumericLiteralExpression, '12')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 12) (Syntax: NumericLiteralExpression, '12')
  LimitValue: 
    ILocalReferenceExpression: y ([2] OperationKind.LocalReferenceExpression, Type: System.Nullable(Of System.Int32)) (Syntax: IdentifierName, 'y')
  StepValue: 
    ILocalReferenceExpression: s ([3] OperationKind.LocalReferenceExpression, Type: System.Nullable(Of System.Int32)) (Syntax: IdentifierName, 's')
  Body: 
    IBlockStatement (0 statements) ([4] OperationKind.BlockStatement, IsImplicit) (Syntax: ForBlock, 'For x = 12  ... Next')
  NextVariables(0)
]]>.Value

            VerifyOperationTreeForTest(Of ForBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub VerifyForToLoop_FieldAsIterationVariable()
            Dim source = <![CDATA[
Imports System
Imports System.Collections.Generic
Imports System.Linq

Class C
    Private X As Integer = 0
    Sub M()
        For X = 0 To 10'BIND:"For X = 0 To 10"
        Next X
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IForToLoopStatement (LoopKind.ForTo) ([0] OperationKind.LoopStatement) (Syntax: ForBlock, 'For X = 0 T ... Next X') (Parent: BlockStatement)
  LoopControlVariable: 
    IFieldReferenceExpression: C.X As System.Int32 ([0] OperationKind.FieldReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'X')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C, IsImplicit) (Syntax: IdentifierName, 'X')
  InitialValue: 
    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  LimitValue: 
    ILiteralExpression ([2] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10) (Syntax: NumericLiteralExpression, '10')
  StepValue: 
    IConversionExpression (Explicit, TryCast: False, Unchecked) ([3] OperationKind.ConversionExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For X = 0 T ... Next X')
      Conversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For X = 0 T ... Next X')
  Body: 
    IBlockStatement (0 statements) ([4] OperationKind.BlockStatement, IsImplicit) (Syntax: ForBlock, 'For X = 0 T ... Next X')
  NextVariables(1):
    IFieldReferenceExpression: C.X As System.Int32 ([5] OperationKind.FieldReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'X')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: C, IsImplicit) (Syntax: IdentifierName, 'X')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of ForBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub VerifyForToLoop_FieldWithExplicitReceiverAsIterationVariable()
            Dim source = <![CDATA[
Imports System
Imports System.Collections.Generic
Imports System.Linq

Class C
    Private X As Integer = 0
    Sub M(c As C)
        For c.X = 0 To 10'BIND:"For c.X = 0 To 10"
        Next c.X
    End Sub
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IForToLoopStatement (LoopKind.ForTo) ([0] OperationKind.LoopStatement) (Syntax: ForBlock, 'For c.X = 0 ... Next c.X') (Parent: BlockStatement)
  LoopControlVariable: 
    IFieldReferenceExpression: C.X As System.Int32 ([0] OperationKind.FieldReferenceExpression, Type: System.Int32) (Syntax: SimpleMemberAccessExpression, 'c.X')
      Instance Receiver: 
        IParameterReferenceExpression: c ([0] OperationKind.ParameterReferenceExpression, Type: C) (Syntax: IdentifierName, 'c')
  InitialValue: 
    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  LimitValue: 
    ILiteralExpression ([2] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10) (Syntax: NumericLiteralExpression, '10')
  StepValue: 
    IConversionExpression (Explicit, TryCast: False, Unchecked) ([3] OperationKind.ConversionExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For c.X = 0 ... Next c.X')
      Conversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ForBlock, 'For c.X = 0 ... Next c.X')
  Body: 
    IBlockStatement (0 statements) ([4] OperationKind.BlockStatement, IsImplicit) (Syntax: ForBlock, 'For c.X = 0 ... Next c.X')
  NextVariables(1):
    IFieldReferenceExpression: C.X As System.Int32 ([5] OperationKind.FieldReferenceExpression, Type: System.Int32) (Syntax: SimpleMemberAccessExpression, 'c.X')
      Instance Receiver: 
        IParameterReferenceExpression: c ([0] OperationKind.ParameterReferenceExpression, Type: C) (Syntax: IdentifierName, 'c')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            VerifyOperationTreeAndDiagnosticsForTest(Of ForBlockSyntax)(source, expectedOperationTree, expectedDiagnostics)
        End Sub
    End Class
End Namespace
