' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.CodeAnalysis.Test.Utilities
Imports Roslyn.Test.Utilities

Namespace Microsoft.CodeAnalysis.VisualBasic.UnitTests.Semantics

    Partial Public Class IOperationTests
        Inherits SemanticModelTestBase

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(17595, "https://github.com/dotnet/roslyn/issues/17591")>
        Public Sub Test_UnaryOperatorExpression_Type_Plus_System_SByte()
            Dim source = <![CDATA[
Class A
    Function Method() As System.SByte
        Dim i As System.SByte = Nothing
        Return +i'BIND:"+i"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Plus, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.SByte) (Syntax: UnaryPlusExpression, '+i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.SByte) (Syntax: IdentifierName, 'i')
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Type_Plus_System_Byte()
            Dim source = <![CDATA[
Class A
    Function Method() As System.Byte
        Dim i As System.Byte = Nothing
        Return +i'BIND:"+i"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Plus, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.Byte) (Syntax: UnaryPlusExpression, '+i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Byte) (Syntax: IdentifierName, 'i')
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Type_Plus_System_Int16()
            Dim source = <![CDATA[
Class A
    Function Method() As System.Int16
        Dim i As System.Int16 = Nothing
        Return +i'BIND:"+i"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Plus, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int16) (Syntax: UnaryPlusExpression, '+i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int16) (Syntax: IdentifierName, 'i')
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Type_Plus_System_UInt16()
            Dim source = <![CDATA[
Class A
    Function Method() As System.UInt16
        Dim i As System.UInt16 = Nothing
        Return +i'BIND:"+i"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Plus, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.UInt16) (Syntax: UnaryPlusExpression, '+i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.UInt16) (Syntax: IdentifierName, 'i')
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Type_Plus_System_Int32()
            Dim source = <![CDATA[
Class A
    Function Method() As System.Int32
        Dim i As System.Int32 = Nothing
        Return +i'BIND:"+i"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Plus, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int32) (Syntax: UnaryPlusExpression, '+i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Type_Plus_System_UInt32()
            Dim source = <![CDATA[
Class A
    Function Method() As System.UInt32
        Dim i As System.UInt32 = Nothing
        Return +i'BIND:"+i"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Plus, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.UInt32) (Syntax: UnaryPlusExpression, '+i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.UInt32) (Syntax: IdentifierName, 'i')
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Type_Plus_System_Int64()
            Dim source = <![CDATA[
Class A
    Function Method() As System.Int64
        Dim i As System.Int64 = Nothing
        Return +i'BIND:"+i"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Plus, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int64) (Syntax: UnaryPlusExpression, '+i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int64) (Syntax: IdentifierName, 'i')
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Type_Plus_System_UInt64()
            Dim source = <![CDATA[
Class A
    Function Method() As System.UInt64
        Dim i As System.UInt64 = Nothing
        Return +i'BIND:"+i"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Plus, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.UInt64) (Syntax: UnaryPlusExpression, '+i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.UInt64) (Syntax: IdentifierName, 'i')
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Type_Plus_System_Decimal()
            Dim source = <![CDATA[
Class A
    Function Method() As System.Decimal
        Dim i As System.Decimal = Nothing
        Return +i'BIND:"+i"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Plus, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.Decimal) (Syntax: UnaryPlusExpression, '+i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Decimal) (Syntax: IdentifierName, 'i')
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Type_Plus_System_Single()
            Dim source = <![CDATA[
Class A
    Function Method() As System.Single
        Dim i As System.Single = Nothing
        Return +i'BIND:"+i"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Plus, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.Single) (Syntax: UnaryPlusExpression, '+i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Single) (Syntax: IdentifierName, 'i')
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Type_Plus_System_Double()
            Dim source = <![CDATA[
Class A
    Function Method() As System.Double
        Dim i As System.Double = Nothing
        Return +i'BIND:"+i"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Plus, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.Double) (Syntax: UnaryPlusExpression, '+i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Double) (Syntax: IdentifierName, 'i')
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Type_Plus_System_Object()
            Dim source = <![CDATA[
Class A
    Function Method() As System.Object
        Dim i As System.Object = Nothing
        Return +i'BIND:"+i"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Plus, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.Object) (Syntax: UnaryPlusExpression, '+i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Object) (Syntax: IdentifierName, 'i')
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Type_Minus_System_SByte()
            Dim source = <![CDATA[
Class A
    Function Method() As System.SByte
        Dim i As System.SByte = Nothing
        Return -i'BIND:"-i"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Minus, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.SByte) (Syntax: UnaryMinusExpression, '-i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.SByte) (Syntax: IdentifierName, 'i')
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Type_Minus_System_Int16()
            Dim source = <![CDATA[
Class A
    Function Method() As System.Int16
        Dim i As System.Int16 = Nothing
        Return -i'BIND:"-i"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Minus, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int16) (Syntax: UnaryMinusExpression, '-i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int16) (Syntax: IdentifierName, 'i')
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Type_Minus_System_Int32()
            Dim source = <![CDATA[
Class A
    Function Method() As System.Int32
        Dim i As System.Int32 = Nothing
        Return -i'BIND:"-i"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Minus, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int32) (Syntax: UnaryMinusExpression, '-i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Type_Minus_System_Int64()
            Dim source = <![CDATA[
Class A
    Function Method() As System.Int64
        Dim i As System.Int64 = Nothing
        Return -i'BIND:"-i"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Minus, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int64) (Syntax: UnaryMinusExpression, '-i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int64) (Syntax: IdentifierName, 'i')
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Type_Minus_System_Decimal()
            Dim source = <![CDATA[
Class A
    Function Method() As System.Decimal
        Dim i As System.Decimal = Nothing
        Return -i'BIND:"-i"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Minus, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.Decimal) (Syntax: UnaryMinusExpression, '-i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Decimal) (Syntax: IdentifierName, 'i')
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Type_Minus_System_Single()
            Dim source = <![CDATA[
Class A
    Function Method() As System.Single
        Dim i As System.Single = Nothing
        Return -i'BIND:"-i"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Minus, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.Single) (Syntax: UnaryMinusExpression, '-i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Single) (Syntax: IdentifierName, 'i')
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Type_Minus_System_Double()
            Dim source = <![CDATA[
Class A
    Function Method() As System.Double
        Dim i As System.Double = Nothing
        Return -i'BIND:"-i"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Minus, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.Double) (Syntax: UnaryMinusExpression, '-i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Double) (Syntax: IdentifierName, 'i')
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Type_Minus_System_Object()
            Dim source = <![CDATA[
Class A
    Function Method() As System.Object
        Dim i As System.Object = Nothing
        Return -i'BIND:"-i"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Minus, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.Object) (Syntax: UnaryMinusExpression, '-i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Object) (Syntax: IdentifierName, 'i')
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Type_Not_System_SByte()
            Dim source = <![CDATA[
Class A
    Function Method() As System.SByte
        Dim i As System.SByte = Nothing
        Return Not i'BIND:"Not i"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Not, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.SByte) (Syntax: NotExpression, 'Not i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.SByte) (Syntax: IdentifierName, 'i')
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Type_Not_System_Byte()
            Dim source = <![CDATA[
Class A
    Function Method() As System.Byte
        Dim i As System.Byte = Nothing
        Return Not i'BIND:"Not i"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Not, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.Byte) (Syntax: NotExpression, 'Not i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Byte) (Syntax: IdentifierName, 'i')
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Type_Not_System_Int16()
            Dim source = <![CDATA[
Class A
    Function Method() As System.Int16
        Dim i As System.Int16 = Nothing
        Return Not i'BIND:"Not i"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Not, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int16) (Syntax: NotExpression, 'Not i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int16) (Syntax: IdentifierName, 'i')
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Type_Not_System_UInt16()
            Dim source = <![CDATA[
Class A
    Function Method() As System.UInt16
        Dim i As System.UInt16 = Nothing
        Return Not i'BIND:"Not i"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Not, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.UInt16) (Syntax: NotExpression, 'Not i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.UInt16) (Syntax: IdentifierName, 'i')
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Type_Not_System_Int32()
            Dim source = <![CDATA[
Class A
    Function Method() As System.Int32
        Dim i As System.Int32 = Nothing
        Return Not i'BIND:"Not i"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Not, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int32) (Syntax: NotExpression, 'Not i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Type_Not_System_UInt32()
            Dim source = <![CDATA[
Class A
    Function Method() As System.UInt32
        Dim i As System.UInt32 = Nothing
        Return Not i'BIND:"Not i"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Not, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.UInt32) (Syntax: NotExpression, 'Not i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.UInt32) (Syntax: IdentifierName, 'i')
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Type_Not_System_Int64()
            Dim source = <![CDATA[
Class A
    Function Method() As System.Int64
        Dim i As System.Int64 = Nothing
        Return Not i'BIND:"Not i"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Not, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int64) (Syntax: NotExpression, 'Not i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int64) (Syntax: IdentifierName, 'i')
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Type_Not_System_UInt64()
            Dim source = <![CDATA[
Class A
    Function Method() As System.UInt64
        Dim i As System.UInt64 = Nothing
        Return Not i'BIND:"Not i"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Not, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.UInt64) (Syntax: NotExpression, 'Not i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.UInt64) (Syntax: IdentifierName, 'i')
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Type_Not_System_Boolean()
            Dim source = <![CDATA[
Class A
    Function Method() As System.Boolean
        Dim i As System.Boolean = Nothing
        Return Not i'BIND:"Not i"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Not, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.Boolean) (Syntax: NotExpression, 'Not i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Boolean) (Syntax: IdentifierName, 'i')
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Type_Not_System_Object()
            Dim source = <![CDATA[
Class A
    Function Method() As System.Object
        Dim i As System.Object = Nothing
        Return Not i'BIND:"Not i"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Not, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.Object) (Syntax: NotExpression, 'Not i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Object) (Syntax: IdentifierName, 'i')
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Method_Plus_System_SByte()
            Dim source = <![CDATA[
Class A
    Function Method() As System.SByte
        Dim i As System.SByte = Nothing
        Return +Method()'BIND:"+Method()"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Plus, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.SByte) (Syntax: UnaryPlusExpression, '+Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( Function A.Method() As System.SByte) ([0] OperationKind.InvocationExpression, Type: System.SByte) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Method_Plus_System_Byte()
            Dim source = <![CDATA[
Class A
    Function Method() As System.Byte
        Dim i As System.Byte = Nothing
        Return +Method()'BIND:"+Method()"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Plus, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.Byte) (Syntax: UnaryPlusExpression, '+Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( Function A.Method() As System.Byte) ([0] OperationKind.InvocationExpression, Type: System.Byte) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Method_Plus_System_Int16()
            Dim source = <![CDATA[
Class A
    Function Method() As System.Int16
        Dim i As System.Int16 = Nothing
        Return +Method()'BIND:"+Method()"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Plus, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int16) (Syntax: UnaryPlusExpression, '+Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( Function A.Method() As System.Int16) ([0] OperationKind.InvocationExpression, Type: System.Int16) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Method_Plus_System_UInt16()
            Dim source = <![CDATA[
Class A
    Function Method() As System.UInt16
        Dim i As System.UInt16 = Nothing
        Return +Method()'BIND:"+Method()"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Plus, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.UInt16) (Syntax: UnaryPlusExpression, '+Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( Function A.Method() As System.UInt16) ([0] OperationKind.InvocationExpression, Type: System.UInt16) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Method_Plus_System_Int32()
            Dim source = <![CDATA[
Class A
    Function Method() As System.Int32
        Dim i As System.Int32 = Nothing
        Return +Method()'BIND:"+Method()"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Plus, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int32) (Syntax: UnaryPlusExpression, '+Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( Function A.Method() As System.Int32) ([0] OperationKind.InvocationExpression, Type: System.Int32) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Method_Plus_System_UInt32()
            Dim source = <![CDATA[
Class A
    Function Method() As System.UInt32
        Dim i As System.UInt32 = Nothing
        Return +Method()'BIND:"+Method()"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Plus, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.UInt32) (Syntax: UnaryPlusExpression, '+Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( Function A.Method() As System.UInt32) ([0] OperationKind.InvocationExpression, Type: System.UInt32) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Method_Plus_System_Int64()
            Dim source = <![CDATA[
Class A
    Function Method() As System.Int64
        Dim i As System.Int64 = Nothing
        Return +Method()'BIND:"+Method()"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Plus, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int64) (Syntax: UnaryPlusExpression, '+Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( Function A.Method() As System.Int64) ([0] OperationKind.InvocationExpression, Type: System.Int64) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Method_Plus_System_UInt64()
            Dim source = <![CDATA[
Class A
    Function Method() As System.UInt64
        Dim i As System.UInt64 = Nothing
        Return +Method()'BIND:"+Method()"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Plus, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.UInt64) (Syntax: UnaryPlusExpression, '+Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( Function A.Method() As System.UInt64) ([0] OperationKind.InvocationExpression, Type: System.UInt64) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Method_Plus_System_Decimal()
            Dim source = <![CDATA[
Class A
    Function Method() As System.Decimal
        Dim i As System.Decimal = Nothing
        Return +Method()'BIND:"+Method()"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Plus, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.Decimal) (Syntax: UnaryPlusExpression, '+Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( Function A.Method() As System.Decimal) ([0] OperationKind.InvocationExpression, Type: System.Decimal) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Method_Plus_System_Single()
            Dim source = <![CDATA[
Class A
    Function Method() As System.Single
        Dim i As System.Single = Nothing
        Return +Method()'BIND:"+Method()"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Plus, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.Single) (Syntax: UnaryPlusExpression, '+Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( Function A.Method() As System.Single) ([0] OperationKind.InvocationExpression, Type: System.Single) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Method_Plus_System_Double()
            Dim source = <![CDATA[
Class A
    Function Method() As System.Double
        Dim i As System.Double = Nothing
        Return +Method()'BIND:"+Method()"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Plus, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.Double) (Syntax: UnaryPlusExpression, '+Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( Function A.Method() As System.Double) ([0] OperationKind.InvocationExpression, Type: System.Double) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Method_Plus_System_Object()
            Dim source = <![CDATA[
Class A
    Function Method() As System.Object
        Dim i As System.Object = Nothing
        Return +Method()'BIND:"+Method()"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Plus, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.Object) (Syntax: UnaryPlusExpression, '+Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( Function A.Method() As System.Object) ([0] OperationKind.InvocationExpression, Type: System.Object) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Method_Minus_System_SByte()
            Dim source = <![CDATA[
Class A
    Function Method() As System.SByte
        Dim i As System.SByte = Nothing
        Return -Method()'BIND:"-Method()"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Minus, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.SByte) (Syntax: UnaryMinusExpression, '-Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( Function A.Method() As System.SByte) ([0] OperationKind.InvocationExpression, Type: System.SByte) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Method_Minus_System_Int16()
            Dim source = <![CDATA[
Class A
    Function Method() As System.Int16
        Dim i As System.Int16 = Nothing
        Return -Method()'BIND:"-Method()"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Minus, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int16) (Syntax: UnaryMinusExpression, '-Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( Function A.Method() As System.Int16) ([0] OperationKind.InvocationExpression, Type: System.Int16) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Method_Minus_System_Int32()
            Dim source = <![CDATA[
Class A
    Function Method() As System.Int32
        Dim i As System.Int32 = Nothing
        Return -Method()'BIND:"-Method()"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Minus, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int32) (Syntax: UnaryMinusExpression, '-Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( Function A.Method() As System.Int32) ([0] OperationKind.InvocationExpression, Type: System.Int32) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Method_Minus_System_Int64()
            Dim source = <![CDATA[
Class A
    Function Method() As System.Int64
        Dim i As System.Int64 = Nothing
        Return -Method()'BIND:"-Method()"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Minus, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int64) (Syntax: UnaryMinusExpression, '-Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( Function A.Method() As System.Int64) ([0] OperationKind.InvocationExpression, Type: System.Int64) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Method_Minus_System_Decimal()
            Dim source = <![CDATA[
Class A
    Function Method() As System.Decimal
        Dim i As System.Decimal = Nothing
        Return -Method()'BIND:"-Method()"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Minus, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.Decimal) (Syntax: UnaryMinusExpression, '-Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( Function A.Method() As System.Decimal) ([0] OperationKind.InvocationExpression, Type: System.Decimal) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Method_Minus_System_Single()
            Dim source = <![CDATA[
Class A
    Function Method() As System.Single
        Dim i As System.Single = Nothing
        Return -Method()'BIND:"-Method()"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Minus, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.Single) (Syntax: UnaryMinusExpression, '-Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( Function A.Method() As System.Single) ([0] OperationKind.InvocationExpression, Type: System.Single) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Method_Minus_System_Double()
            Dim source = <![CDATA[
Class A
    Function Method() As System.Double
        Dim i As System.Double = Nothing
        Return -Method()'BIND:"-Method()"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Minus, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.Double) (Syntax: UnaryMinusExpression, '-Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( Function A.Method() As System.Double) ([0] OperationKind.InvocationExpression, Type: System.Double) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Method_Minus_System_Object()
            Dim source = <![CDATA[
Class A
    Function Method() As System.Object
        Dim i As System.Object = Nothing
        Return -Method()'BIND:"-Method()"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Minus, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.Object) (Syntax: UnaryMinusExpression, '-Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( Function A.Method() As System.Object) ([0] OperationKind.InvocationExpression, Type: System.Object) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Method_Not_System_SByte()
            Dim source = <![CDATA[
Class A
    Function Method() As System.SByte
        Dim i As System.SByte = Nothing
        Return Not Method()'BIND:"Not Method()"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Not, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.SByte) (Syntax: NotExpression, 'Not Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( Function A.Method() As System.SByte) ([0] OperationKind.InvocationExpression, Type: System.SByte) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Method_Not_System_Byte()
            Dim source = <![CDATA[
Class A
    Function Method() As System.Byte
        Dim i As System.Byte = Nothing
        Return Not Method()'BIND:"Not Method()"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Not, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.Byte) (Syntax: NotExpression, 'Not Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( Function A.Method() As System.Byte) ([0] OperationKind.InvocationExpression, Type: System.Byte) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Method_Not_System_Int16()
            Dim source = <![CDATA[
Class A
    Function Method() As System.Int16
        Dim i As System.Int16 = Nothing
        Return Not Method()'BIND:"Not Method()"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Not, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int16) (Syntax: NotExpression, 'Not Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( Function A.Method() As System.Int16) ([0] OperationKind.InvocationExpression, Type: System.Int16) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Method_Not_System_UInt16()
            Dim source = <![CDATA[
Class A
    Function Method() As System.UInt16
        Dim i As System.UInt16 = Nothing
        Return Not Method()'BIND:"Not Method()"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Not, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.UInt16) (Syntax: NotExpression, 'Not Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( Function A.Method() As System.UInt16) ([0] OperationKind.InvocationExpression, Type: System.UInt16) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Method_Not_System_Int32()
            Dim source = <![CDATA[
Class A
    Function Method() As System.Int32
        Dim i As System.Int32 = Nothing
        Return Not Method()'BIND:"Not Method()"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Not, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int32) (Syntax: NotExpression, 'Not Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( Function A.Method() As System.Int32) ([0] OperationKind.InvocationExpression, Type: System.Int32) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Method_Not_System_UInt32()
            Dim source = <![CDATA[
Class A
    Function Method() As System.UInt32
        Dim i As System.UInt32 = Nothing
        Return Not Method()'BIND:"Not Method()"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Not, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.UInt32) (Syntax: NotExpression, 'Not Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( Function A.Method() As System.UInt32) ([0] OperationKind.InvocationExpression, Type: System.UInt32) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Method_Not_System_Int64()
            Dim source = <![CDATA[
Class A
    Function Method() As System.Int64
        Dim i As System.Int64 = Nothing
        Return Not Method()'BIND:"Not Method()"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Not, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int64) (Syntax: NotExpression, 'Not Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( Function A.Method() As System.Int64) ([0] OperationKind.InvocationExpression, Type: System.Int64) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Method_Not_System_UInt64()
            Dim source = <![CDATA[
Class A
    Function Method() As System.UInt64
        Dim i As System.UInt64 = Nothing
        Return Not Method()'BIND:"Not Method()"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Not, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.UInt64) (Syntax: NotExpression, 'Not Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( Function A.Method() As System.UInt64) ([0] OperationKind.InvocationExpression, Type: System.UInt64) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Method_Not_System_Boolean()
            Dim source = <![CDATA[
Class A
    Function Method() As System.Boolean
        Dim i As System.Boolean = Nothing
        Return Not Method()'BIND:"Not Method()"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Not, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.Boolean) (Syntax: NotExpression, 'Not Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( Function A.Method() As System.Boolean) ([0] OperationKind.InvocationExpression, Type: System.Boolean) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Method_Not_System_Object()
            Dim source = <![CDATA[
Class A
    Function Method() As System.Object
        Dim i As System.Object = Nothing
        Return Not Method()'BIND:"Not Method()"
    End Function
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Not, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.Object) (Syntax: NotExpression, 'Not Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( Function A.Method() As System.Object) ([0] OperationKind.InvocationExpression, Type: System.Object) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Type_Plus_E()
            Dim source = <![CDATA[
Class A
    Function Method() As E
        Dim i As E = Nothing
        Return +i'BIND:"+i"
    End Function
End Class
Enum E
A
B
End Enum
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Plus, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int32) (Syntax: UnaryPlusExpression, '+i') (Parent: ConversionExpression)
  Operand: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: IdentifierName, 'i')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: E) (Syntax: IdentifierName, 'i')
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Type_Minus_E()
            Dim source = <![CDATA[
Class A
    Function Method() As E
        Dim i As E = Nothing
        Return -i'BIND:"-i"
    End Function
End Class
Enum E
A
B
End Enum
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Minus, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int32) (Syntax: UnaryMinusExpression, '-i') (Parent: ConversionExpression)
  Operand: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: IdentifierName, 'i')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: E) (Syntax: IdentifierName, 'i')
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Type_Not_E()
            Dim source = <![CDATA[
Class A
    Function Method() As E
        Dim i As E = Nothing
        Return Not i'BIND:"Not i"
    End Function
End Class
Enum E
A
B
End Enum
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Not, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: E) (Syntax: NotExpression, 'Not i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: E) (Syntax: IdentifierName, 'i')
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Method_Plus_E()
            Dim source = <![CDATA[
Class A
    Function Method() As E
        Dim i As E = Nothing
        Return +Method()'BIND:"+Method()"
    End Function
End Class
Enum E
A
B
End Enum
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Plus, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int32) (Syntax: UnaryPlusExpression, '+Method()') (Parent: ConversionExpression)
  Operand: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: InvocationExpression, 'Method()')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IInvocationExpression ( Function A.Method() As E) ([0] OperationKind.InvocationExpression, Type: E) (Syntax: InvocationExpression, 'Method()')
          Instance Receiver: 
            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
          Arguments(0)
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Method_Minus_E()
            Dim source = <![CDATA[
Class A
    Function Method() As E
        Dim i As E = Nothing
        Return -Method()'BIND:"-Method()"
    End Function
End Class
Enum E
A
B
End Enum
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Minus, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int32) (Syntax: UnaryMinusExpression, '-Method()') (Parent: ConversionExpression)
  Operand: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: InvocationExpression, 'Method()')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IInvocationExpression ( Function A.Method() As E) ([0] OperationKind.InvocationExpression, Type: E) (Syntax: InvocationExpression, 'Method()')
          Instance Receiver: 
            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
          Arguments(0)
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Method_Not_E()
            Dim source = <![CDATA[
Class A
    Function Method() As E
        Dim i As E = Nothing
        Return Not Method()'BIND:"Not Method()"
    End Function
End Class
Enum E
A
B
End Enum
    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Not, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: E) (Syntax: NotExpression, 'Not Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( Function A.Method() As E) ([0] OperationKind.InvocationExpression, Type: E) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Type_Plus_CustomType()
            Dim source = <![CDATA[
Class A
    Function Method() As CustomType
        Dim i As CustomType = Nothing
        Return +i'BIND:"+i"
    End Function
End Class

Public Class CustomType
    Public Shared Operator -(x As CustomType) As CustomType
        Return x
    End Operator

    Public Shared operator +(x As CustomType) As CustomType
        return x
    End Operator

    Public Shared operator Not(x As CustomType) As CustomType
        return x
    End Operator
End CLass

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Plus) (OperatorMethod: Function CustomType.op_UnaryPlus(x As CustomType) As CustomType) ([0] OperationKind.UnaryOperatorExpression, Type: CustomType) (Syntax: UnaryPlusExpression, '+i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: CustomType) (Syntax: IdentifierName, 'i')
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Type_Minus_CustomType()
            Dim source = <![CDATA[
Class A
    Function Method() As CustomType
        Dim i As CustomType = Nothing
        Return -i'BIND:"-i"
    End Function
End Class

Public Class CustomType
    Public Shared Operator -(x As CustomType) As CustomType
        Return x
    End Operator

    Public Shared operator +(x As CustomType) As CustomType
        return x
    End Operator

    Public Shared operator Not(x As CustomType) As CustomType
        return x
    End Operator
End CLass

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Minus) (OperatorMethod: Function CustomType.op_UnaryNegation(x As CustomType) As CustomType) ([0] OperationKind.UnaryOperatorExpression, Type: CustomType) (Syntax: UnaryMinusExpression, '-i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: CustomType) (Syntax: IdentifierName, 'i')
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Type_Not_CustomType()
            Dim source = <![CDATA[
Class A
    Function Method() As CustomType
        Dim i As CustomType = Nothing
        Return Not i'BIND:"Not i"
    End Function
End Class

Public Class CustomType
    Public Shared Operator -(x As CustomType) As CustomType
        Return x
    End Operator

    Public Shared operator +(x As CustomType) As CustomType
        return x
    End Operator

    Public Shared operator Not(x As CustomType) As CustomType
        return x
    End Operator
End CLass

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Not) (OperatorMethod: Function CustomType.op_OnesComplement(x As CustomType) As CustomType) ([0] OperationKind.UnaryOperatorExpression, Type: CustomType) (Syntax: NotExpression, 'Not i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: CustomType) (Syntax: IdentifierName, 'i')
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Method_Plus_CustomType()
            Dim source = <![CDATA[
Class A
    Function Method() As CustomType
        Dim i As CustomType = Nothing
        Return +Method()'BIND:"+Method()"
    End Function
End Class

Public Class CustomType
    Public Shared Operator -(x As CustomType) As CustomType
        Return x
    End Operator

    Public Shared operator +(x As CustomType) As CustomType
        return x
    End Operator

    Public Shared operator Not(x As CustomType) As CustomType
        return x
    End Operator
End CLass

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Plus) (OperatorMethod: Function CustomType.op_UnaryPlus(x As CustomType) As CustomType) ([0] OperationKind.UnaryOperatorExpression, Type: CustomType) (Syntax: UnaryPlusExpression, '+Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( Function A.Method() As CustomType) ([0] OperationKind.InvocationExpression, Type: CustomType) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Method_Minus_CustomType()
            Dim source = <![CDATA[
Class A
    Function Method() As CustomType
        Dim i As CustomType = Nothing
        Return -Method()'BIND:"-Method()"
    End Function
End Class

Public Class CustomType
    Public Shared Operator -(x As CustomType) As CustomType
        Return x
    End Operator

    Public Shared operator +(x As CustomType) As CustomType
        return x
    End Operator

    Public Shared operator Not(x As CustomType) As CustomType
        return x
    End Operator
End CLass

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Minus) (OperatorMethod: Function CustomType.op_UnaryNegation(x As CustomType) As CustomType) ([0] OperationKind.UnaryOperatorExpression, Type: CustomType) (Syntax: UnaryMinusExpression, '-Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( Function A.Method() As CustomType) ([0] OperationKind.InvocationExpression, Type: CustomType) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_Method_Not_CustomType()
            Dim source = <![CDATA[
Class A
    Function Method() As CustomType
        Dim i As CustomType = Nothing
        Return Not Method()'BIND:"Not Method()"
    End Function
End Class

Public Class CustomType
    Public Shared Operator -(x As CustomType) As CustomType
        Return x
    End Operator

    Public Shared operator +(x As CustomType) As CustomType
        return x
    End Operator

    Public Shared operator Not(x As CustomType) As CustomType
        return x
    End Operator
End CLass

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Not) (OperatorMethod: Function CustomType.op_OnesComplement(x As CustomType) As CustomType) ([0] OperationKind.UnaryOperatorExpression, Type: CustomType) (Syntax: NotExpression, 'Not Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( Function A.Method() As CustomType) ([0] OperationKind.InvocationExpression, Type: CustomType) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(18135, "https://github.com/dotnet/roslyn/issues/18135")>
        Public Sub Test_UnaryOperatorExpression_Type_And_TrueFalse()
            Dim source = <![CDATA[

Public Class CustomType
    Public Shared Operator IsTrue(x As CustomType) As Boolean
        Return True
    End Operator

    Public Shared Operator IsFalse(x As CustomType) As Boolean
        Return False
    End Operator

    Public Shared Operator And(x As CustomType, y As CustomType) As CustomType
        Return x
    End Operator

    Public Shared Operator Or(x As CustomType, y As CustomType) As CustomType
        Return x
    End Operator
End Class

Class A
    Sub Method()
        Dim x As CustomType = New CustomType()
        Dim y As CustomType = New CustomType()
        If x AndAlso y Then'BIND:"If x AndAlso y Then"
        End If
    End Sub
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IIfStatement ([2] OperationKind.IfStatement) (Syntax: MultiLineIfBlock, 'If x AndAls ... End If') (Parent: BlockStatement)
  Condition: 
    IUnaryOperatorExpression (UnaryOperatorKind.True) (OperatorMethod: Function CustomType.op_True(x As CustomType) As System.Boolean) ([0] OperationKind.UnaryOperatorExpression, Type: System.Boolean, IsImplicit) (Syntax: AndAlsoExpression, 'x AndAlso y')
      Operand: 
        IBinaryOperatorExpression (BinaryOperatorKind.ConditionalAnd) (OperatorMethod: Function CustomType.op_BitwiseAnd(x As CustomType, y As CustomType) As CustomType) ([0] OperationKind.BinaryOperatorExpression, Type: CustomType) (Syntax: AndAlsoExpression, 'x AndAlso y')
          Left: 
            ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: CustomType) (Syntax: IdentifierName, 'x')
          Right: 
            ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: CustomType) (Syntax: IdentifierName, 'y')
  IfTrue: 
    IBlockStatement (0 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: MultiLineIfBlock, 'If x AndAls ... End If')
  IfFalse: 
    null
]]>.Value

            VerifyOperationTreeForTest(Of MultiLineIfBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact(), WorkItem(18135, "https://github.com/dotnet/roslyn/issues/18135")>
        Public Sub Test_UnaryOperatorExpression_Type_Or_TrueFalse()
            Dim source = <![CDATA[

Public Class CustomType
    Public Shared Operator IsTrue(x As CustomType) As Boolean
        Return True
    End Operator

    Public Shared Operator IsFalse(x As CustomType) As Boolean
        Return False
    End Operator

    Public Shared Operator And(x As CustomType, y As CustomType) As CustomType
        Return x
    End Operator

    Public Shared Operator Or(x As CustomType, y As CustomType) As CustomType
        Return x
    End Operator
End Class

Class A
    Sub Method()
        Dim x As CustomType = New CustomType()
        Dim y As CustomType = New CustomType()
        If x OrElse y Then'BIND:"If x OrElse y Then"
        End If
    End Sub
End Class

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IIfStatement ([2] OperationKind.IfStatement) (Syntax: MultiLineIfBlock, 'If x OrElse ... End If') (Parent: BlockStatement)
  Condition: 
    IUnaryOperatorExpression (UnaryOperatorKind.True) (OperatorMethod: Function CustomType.op_True(x As CustomType) As System.Boolean) ([0] OperationKind.UnaryOperatorExpression, Type: System.Boolean, IsImplicit) (Syntax: OrElseExpression, 'x OrElse y')
      Operand: 
        IBinaryOperatorExpression (BinaryOperatorKind.ConditionalAnd) (OperatorMethod: Function CustomType.op_BitwiseOr(x As CustomType, y As CustomType) As CustomType) ([0] OperationKind.BinaryOperatorExpression, Type: CustomType) (Syntax: OrElseExpression, 'x OrElse y')
          Left: 
            ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: CustomType) (Syntax: IdentifierName, 'x')
          Right: 
            ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: CustomType) (Syntax: IdentifierName, 'y')
  IfTrue: 
    IBlockStatement (0 statements) ([1] OperationKind.BlockStatement, IsImplicit) (Syntax: MultiLineIfBlock, 'If x OrElse ... End If')
  IfFalse: 
    null
]]>.Value

            VerifyOperationTreeForTest(Of MultiLineIfBlockSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_With_CustomType_NoRightOperator()
            Dim source = <![CDATA[
Class A
    Function Method() As CustomType
        Dim i As CustomType = Nothing
        Return +i'BIND:"+i"
    End Function
End Class

Public Class CustomType
End CLass

    ]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Plus, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: ?, IsInvalid) (Syntax: UnaryPlusExpression, '+i') (Parent: ConversionExpression)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: CustomType, IsInvalid) (Syntax: IdentifierName, 'i')
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_With_CustomType_DerivedTypes()
            Dim source = <![CDATA[
Class A
    Function Method() As BaseType
        Dim i As DerivedType = Nothing
        Return +i 'BIND:"+i"
    End Function
End Class

Public Class BaseType
    Public Shared Operator +(x As BaseType) As BaseType
        Return x
    End Operator
End Class

Public Class DerivedType
    Inherits BaseType
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Plus) (OperatorMethod: Function BaseType.op_UnaryPlus(x As BaseType) As BaseType) ([0] OperationKind.UnaryOperatorExpression, Type: BaseType) (Syntax: UnaryPlusExpression, '+i') (Parent: ReturnStatement)
  Operand: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: BaseType, IsImplicit) (Syntax: IdentifierName, 'i')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: DerivedType) (Syntax: IdentifierName, 'i')
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_With_CustomType_ImplicitConversion()
            Dim source = <![CDATA[
Class A
    Function Method() As BaseType
        Dim i As DerivedType = Nothing
        Return +i 'BIND:"+i"
    End Function
End Class

Public Class BaseType
    Public Shared Operator +(x As BaseType) As BaseType
        Return x
    End Operator
End Class

Public Class DerivedType
    Public Shared Narrowing Operator CType(ByVal x As DerivedType) As BaseType
        Return New BaseType()
    End Operator
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Plus, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: ?, IsInvalid) (Syntax: UnaryPlusExpression, '+i') (Parent: ConversionExpression)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: DerivedType, IsInvalid) (Syntax: IdentifierName, 'i')
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_With_CustomType_ExplicitConversion()
            Dim source = <![CDATA[
Class A
    Function Method() As BaseType
        Dim i As DerivedType = Nothing
        Return +i 'BIND:"+i"
    End Function
End Class

Public Class BaseType
    Public Shared Operator +(x As BaseType) As BaseType
        Return x
    End Operator
End Class

Public Class DerivedType
    Public Shared Widening Operator CType(ByVal x As DerivedType) As BaseType
        Return New BaseType()
    End Operator
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Plus, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: ?, IsInvalid) (Syntax: UnaryPlusExpression, '+i') (Parent: ConversionExpression)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: DerivedType, IsInvalid) (Syntax: IdentifierName, 'i')
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact()>
        Public Sub Test_UnaryOperatorExpression_With_CustomType_Malformed_Operator()
            Dim source = <![CDATA[
Class A
    Function Method() As BaseType
        Dim i As BaseType = Nothing
        Return +i 'BIND:"+i"
    End Function
End Class

Public Class BaseType
    Public Shared Operator +(x As Integer) As BaseType
        Return New BaseType()
    End Operator
End Class]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Plus, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: ?, IsInvalid) (Syntax: UnaryPlusExpression, '+i') (Parent: ConversionExpression)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: BaseType, IsInvalid) (Syntax: IdentifierName, 'i')
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub VerifyLiftedUnaryOperators1()
            Dim source = <![CDATA[
Class C
    Sub F(x as Integer?)
        dim y = -x 'BIND:"-x"
    End Sub
End Class
]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Minus, IsLifted, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.Nullable(Of System.Int32)) (Syntax: UnaryMinusExpression, '-x') (Parent: VariableInitializer)
  Operand: 
    IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Nullable(Of System.Int32)) (Syntax: IdentifierName, 'x')
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub VerifyNonLiftedUnaryOperators1()
            Dim source = <![CDATA[
Class C
    Sub F(x as Integer)
        dim y = -x 'BIND:"-x"
    End Sub
End Class
]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Minus, Checked) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int32) (Syntax: UnaryMinusExpression, '-x') (Parent: VariableInitializer)
  Operand: 
    IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub VerifyUncheckedLiftedUnaryOperators1()
            Dim source = <![CDATA[
Class C
    Sub F(x as Integer?)
        dim y = -x 'BIND:"-x"
    End Sub
End Class
]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Minus, IsLifted) ([0] OperationKind.UnaryOperatorExpression, Type: System.Nullable(Of System.Int32)) (Syntax: UnaryMinusExpression, '-x') (Parent: VariableInitializer)
  Operand: 
    IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Nullable(Of System.Int32)) (Syntax: IdentifierName, 'x')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            Dim fileName = "a.vb"
            Dim syntaxTree = Parse(source, fileName)
            Dim references = DefaultVbReferences.Concat({ValueTupleRef, SystemRuntimeFacadeRef})
            Dim compilation = CreateCompilationWithMscorlib45AndVBRuntime({syntaxTree}, references:=references, options:=TestOptions.ReleaseDll.WithOverflowChecks(False))

            VerifyOperationTreeAndDiagnosticsForTest(Of UnaryExpressionSyntax)(compilation, fileName, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub VerifyUncheckedNonLiftedUnaryOperators1()
            Dim source = <![CDATA[
Class C
    Sub F(x as Integer)
        dim y = -x 'BIND:"-x"
    End Sub
End Class
]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Minus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int32) (Syntax: UnaryMinusExpression, '-x') (Parent: VariableInitializer)
  Operand: 
    IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
]]>.Value

            Dim expectedDiagnostics = String.Empty

            Dim fileName = "a.vb"
            Dim syntaxTree = Parse(source, fileName)
            Dim references = DefaultVbReferences.Concat({ValueTupleRef, SystemRuntimeFacadeRef})
            Dim compilation = CreateCompilationWithMscorlib45AndVBRuntime({syntaxTree}, references:=references, options:=TestOptions.ReleaseDll.WithOverflowChecks(False))

            VerifyOperationTreeAndDiagnosticsForTest(Of UnaryExpressionSyntax)(compilation, fileName, expectedOperationTree, expectedDiagnostics)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub VerifyLiftedUserDefinedUnaryOperators1()
            Dim source = <![CDATA[
Structure C
    Public Shared Operator -(c as C) as C
    End Operator

    Sub F(x as C?)
        dim y = -x 'BIND:"-x"
    End Sub
End Structure
]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Minus, IsLifted) (OperatorMethod: Function C.op_UnaryNegation(c As C) As C) ([0] OperationKind.UnaryOperatorExpression, Type: System.Nullable(Of C)) (Syntax: UnaryMinusExpression, '-x') (Parent: VariableInitializer)
  Operand: 
    IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Nullable(Of C)) (Syntax: IdentifierName, 'x')
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub

        <CompilerTrait(CompilerFeature.IOperation)>
        <Fact>
        Public Sub VerifyNonLiftedUserDefinedUnaryOperators1()
            Dim source = <![CDATA[
Structure C
    Public Shared Operator -(c as C) as C
    End Operator

    Sub F(x as C)
        dim y = -x 'BIND:"-x"
    End Sub
End Structure
]]>.Value

            Dim expectedOperationTree = <![CDATA[
IUnaryOperatorExpression (UnaryOperatorKind.Minus) (OperatorMethod: Function C.op_UnaryNegation(c As C) As C) ([0] OperationKind.UnaryOperatorExpression, Type: C) (Syntax: UnaryMinusExpression, '-x') (Parent: VariableInitializer)
  Operand: 
    IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: C) (Syntax: IdentifierName, 'x')
]]>.Value

            VerifyOperationTreeForTest(Of UnaryExpressionSyntax)(source, expectedOperationTree)
        End Sub
    End Class
End Namespace
