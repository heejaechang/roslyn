// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Test.Utilities;
using Roslyn.Test.Utilities;
using Xunit;

namespace Microsoft.CodeAnalysis.CSharp.UnitTests
{
    public partial class IOperationTests : SemanticModelTestBase
    {
        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17595, "https://github.com/dotnet/roslyn/issues/17591")]
        public void Test_UnaryOperatorExpression_Type_Plus_System_SByte()
        {
            string source = @"
class A
{
    System.SByte Method()
    {
        System.SByte i = default(System.SByte);
        return /*<bind>*/+i/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Plus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int32, IsInvalid) (Syntax: UnaryPlusExpression, '+i') (Parent: ConversionExpression)
  Operand: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'i')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.SByte, IsInvalid) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Type_Plus_System_Byte()
        {
            string source = @"
class A
{
    System.Byte Method()
    {
        System.Byte i = default(System.Byte);
        return /*<bind>*/+i/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Plus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int32, IsInvalid) (Syntax: UnaryPlusExpression, '+i') (Parent: ConversionExpression)
  Operand: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'i')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Byte, IsInvalid) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Type_Plus_System_Int16()
        {
            string source = @"
class A
{
    System.Int16 Method()
    {
        System.Int16 i = default(System.Int16);
        return /*<bind>*/+i/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Plus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int32, IsInvalid) (Syntax: UnaryPlusExpression, '+i') (Parent: ConversionExpression)
  Operand: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'i')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int16, IsInvalid) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Type_Plus_System_UInt16()
        {
            string source = @"
class A
{
    System.UInt16 Method()
    {
        System.UInt16 i = default(System.UInt16);
        return /*<bind>*/+i/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Plus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int32, IsInvalid) (Syntax: UnaryPlusExpression, '+i') (Parent: ConversionExpression)
  Operand: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'i')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.UInt16, IsInvalid) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Type_Plus_System_Int32()
        {
            string source = @"
class A
{
    System.Int32 Method()
    {
        System.Int32 i = default(System.Int32);
        return /*<bind>*/+i/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Plus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int32) (Syntax: UnaryPlusExpression, '+i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Type_Plus_System_UInt32()
        {
            string source = @"
class A
{
    System.UInt32 Method()
    {
        System.UInt32 i = default(System.UInt32);
        return /*<bind>*/+i/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Plus) ([0] OperationKind.UnaryOperatorExpression, Type: System.UInt32) (Syntax: UnaryPlusExpression, '+i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.UInt32) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Type_Plus_System_Int64()
        {
            string source = @"
class A
{
    System.Int64 Method()
    {
        System.Int64 i = default(System.Int64);
        return /*<bind>*/+i/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Plus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int64) (Syntax: UnaryPlusExpression, '+i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int64) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Type_Plus_System_UInt64()
        {
            string source = @"
class A
{
    System.UInt64 Method()
    {
        System.UInt64 i = default(System.UInt64);
        return /*<bind>*/+i/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Plus) ([0] OperationKind.UnaryOperatorExpression, Type: System.UInt64) (Syntax: UnaryPlusExpression, '+i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.UInt64) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Type_Plus_System_Char()
        {
            string source = @"
class A
{
    System.Char Method()
    {
        System.Char i = default(System.Char);
        return /*<bind>*/+i/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Plus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int32, IsInvalid) (Syntax: UnaryPlusExpression, '+i') (Parent: ConversionExpression)
  Operand: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'i')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Char, IsInvalid) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Type_Plus_System_Decimal()
        {
            string source = @"
class A
{
    System.Decimal Method()
    {
        System.Decimal i = default(System.Decimal);
        return /*<bind>*/+i/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Plus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Decimal) (Syntax: UnaryPlusExpression, '+i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Decimal) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Type_Plus_System_Single()
        {
            string source = @"
class A
{
    System.Single Method()
    {
        System.Single i = default(System.Single);
        return /*<bind>*/+i/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Plus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Single) (Syntax: UnaryPlusExpression, '+i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Single) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Type_Plus_System_Double()
        {
            string source = @"
class A
{
    System.Double Method()
    {
        System.Double i = default(System.Double);
        return /*<bind>*/+i/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Plus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Double) (Syntax: UnaryPlusExpression, '+i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Double) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Type_Plus_System_Boolean()
        {
            string source = @"
class A
{
    System.Boolean Method()
    {
        System.Boolean i = default(System.Boolean);
        return /*<bind>*/+i/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Plus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Object, IsInvalid) (Syntax: UnaryPlusExpression, '+i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Boolean, IsInvalid) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Type_Plus_System_Object()
        {
            string source = @"
class A
{
    System.Object Method()
    {
        System.Object i = default(System.Object);
        return /*<bind>*/+i/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Plus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Object, IsInvalid) (Syntax: UnaryPlusExpression, '+i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Object, IsInvalid) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Type_Minus_System_SByte()
        {
            string source = @"
class A
{
    System.SByte Method()
    {
        System.SByte i = default(System.SByte);
        return /*<bind>*/-i/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Minus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int32, IsInvalid) (Syntax: UnaryMinusExpression, '-i') (Parent: ConversionExpression)
  Operand: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'i')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.SByte, IsInvalid) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Type_Minus_System_Byte()
        {
            string source = @"
class A
{
    System.Byte Method()
    {
        System.Byte i = default(System.Byte);
        return /*<bind>*/-i/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Minus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int32, IsInvalid) (Syntax: UnaryMinusExpression, '-i') (Parent: ConversionExpression)
  Operand: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'i')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Byte, IsInvalid) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Type_Minus_System_Int16()
        {
            string source = @"
class A
{
    System.Int16 Method()
    {
        System.Int16 i = default(System.Int16);
        return /*<bind>*/-i/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Minus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int32, IsInvalid) (Syntax: UnaryMinusExpression, '-i') (Parent: ConversionExpression)
  Operand: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'i')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int16, IsInvalid) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Type_Minus_System_UInt16()
        {
            string source = @"
class A
{
    System.UInt16 Method()
    {
        System.UInt16 i = default(System.UInt16);
        return /*<bind>*/-i/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Minus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int32, IsInvalid) (Syntax: UnaryMinusExpression, '-i') (Parent: ConversionExpression)
  Operand: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'i')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.UInt16, IsInvalid) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Type_Minus_System_Int32()
        {
            string source = @"
class A
{
    System.Int32 Method()
    {
        System.Int32 i = default(System.Int32);
        return /*<bind>*/-i/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Minus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int32) (Syntax: UnaryMinusExpression, '-i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Type_Minus_System_UInt32()
        {
            string source = @"
class A
{
    System.UInt32 Method()
    {
        System.UInt32 i = default(System.UInt32);
        return /*<bind>*/-i/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Minus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int64, IsInvalid) (Syntax: UnaryMinusExpression, '-i') (Parent: ConversionExpression)
  Operand: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int64, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'i')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.UInt32, IsInvalid) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Type_Minus_System_Int64()
        {
            string source = @"
class A
{
    System.Int64 Method()
    {
        System.Int64 i = default(System.Int64);
        return /*<bind>*/-i/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Minus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int64) (Syntax: UnaryMinusExpression, '-i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int64) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Type_Minus_System_UInt64()
        {
            string source = @"
class A
{
    System.UInt64 Method()
    {
        System.UInt64 i = default(System.UInt64);
        return /*<bind>*/-i/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Minus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Object, IsInvalid) (Syntax: UnaryMinusExpression, '-i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.UInt64, IsInvalid) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Type_Minus_System_Char()
        {
            string source = @"
class A
{
    System.Char Method()
    {
        System.Char i = default(System.Char);
        return /*<bind>*/-i/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Minus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int32, IsInvalid) (Syntax: UnaryMinusExpression, '-i') (Parent: ConversionExpression)
  Operand: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'i')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Char, IsInvalid) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Type_Minus_System_Decimal()
        {
            string source = @"
class A
{
    System.Decimal Method()
    {
        System.Decimal i = default(System.Decimal);
        return /*<bind>*/-i/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Minus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Decimal) (Syntax: UnaryMinusExpression, '-i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Decimal) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Type_Minus_System_Single()
        {
            string source = @"
class A
{
    System.Single Method()
    {
        System.Single i = default(System.Single);
        return /*<bind>*/-i/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Minus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Single) (Syntax: UnaryMinusExpression, '-i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Single) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Type_Minus_System_Double()
        {
            string source = @"
class A
{
    System.Double Method()
    {
        System.Double i = default(System.Double);
        return /*<bind>*/-i/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Minus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Double) (Syntax: UnaryMinusExpression, '-i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Double) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Type_Minus_System_Boolean()
        {
            string source = @"
class A
{
    System.Boolean Method()
    {
        System.Boolean i = default(System.Boolean);
        return /*<bind>*/-i/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Minus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Object, IsInvalid) (Syntax: UnaryMinusExpression, '-i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Boolean, IsInvalid) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Type_Minus_System_Object()
        {
            string source = @"
class A
{
    System.Object Method()
    {
        System.Object i = default(System.Object);
        return /*<bind>*/-i/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Minus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Object, IsInvalid) (Syntax: UnaryMinusExpression, '-i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Object, IsInvalid) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_Plus_System_SByte()
        {
            string source = @"
class A
{
    System.SByte Method()
    {
        System.SByte i = default(System.SByte);
        return /*<bind>*/+Method()/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Plus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int32, IsInvalid) (Syntax: UnaryPlusExpression, '+Method()') (Parent: ConversionExpression)
  Operand: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32, IsInvalid, IsImplicit) (Syntax: InvocationExpression, 'Method()')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IInvocationExpression ( System.SByte A.Method()) ([0] OperationKind.InvocationExpression, Type: System.SByte, IsInvalid) (Syntax: InvocationExpression, 'Method()')
          Instance Receiver: 
            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'Method')
          Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_Plus_System_Byte()
        {
            string source = @"
class A
{
    System.Byte Method()
    {
        System.Byte i = default(System.Byte);
        return /*<bind>*/+Method()/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Plus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int32, IsInvalid) (Syntax: UnaryPlusExpression, '+Method()') (Parent: ConversionExpression)
  Operand: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32, IsInvalid, IsImplicit) (Syntax: InvocationExpression, 'Method()')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IInvocationExpression ( System.Byte A.Method()) ([0] OperationKind.InvocationExpression, Type: System.Byte, IsInvalid) (Syntax: InvocationExpression, 'Method()')
          Instance Receiver: 
            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'Method')
          Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_Plus_System_Int16()
        {
            string source = @"
class A
{
    System.Int16 Method()
    {
        System.Int16 i = default(System.Int16);
        return /*<bind>*/+Method()/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Plus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int32, IsInvalid) (Syntax: UnaryPlusExpression, '+Method()') (Parent: ConversionExpression)
  Operand: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32, IsInvalid, IsImplicit) (Syntax: InvocationExpression, 'Method()')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IInvocationExpression ( System.Int16 A.Method()) ([0] OperationKind.InvocationExpression, Type: System.Int16, IsInvalid) (Syntax: InvocationExpression, 'Method()')
          Instance Receiver: 
            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'Method')
          Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_Plus_System_UInt16()
        {
            string source = @"
class A
{
    System.UInt16 Method()
    {
        System.UInt16 i = default(System.UInt16);
        return /*<bind>*/+Method()/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Plus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int32, IsInvalid) (Syntax: UnaryPlusExpression, '+Method()') (Parent: ConversionExpression)
  Operand: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32, IsInvalid, IsImplicit) (Syntax: InvocationExpression, 'Method()')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IInvocationExpression ( System.UInt16 A.Method()) ([0] OperationKind.InvocationExpression, Type: System.UInt16, IsInvalid) (Syntax: InvocationExpression, 'Method()')
          Instance Receiver: 
            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'Method')
          Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_Plus_System_Int32()
        {
            string source = @"
class A
{
    System.Int32 Method()
    {
        System.Int32 i = default(System.Int32);
        return /*<bind>*/+Method()/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Plus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int32) (Syntax: UnaryPlusExpression, '+Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( System.Int32 A.Method()) ([0] OperationKind.InvocationExpression, Type: System.Int32) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_Plus_System_UInt32()
        {
            string source = @"
class A
{
    System.UInt32 Method()
    {
        System.UInt32 i = default(System.UInt32);
        return /*<bind>*/+Method()/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Plus) ([0] OperationKind.UnaryOperatorExpression, Type: System.UInt32) (Syntax: UnaryPlusExpression, '+Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( System.UInt32 A.Method()) ([0] OperationKind.InvocationExpression, Type: System.UInt32) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_Plus_System_Int64()
        {
            string source = @"
class A
{
    System.Int64 Method()
    {
        System.Int64 i = default(System.Int64);
        return /*<bind>*/+Method()/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Plus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int64) (Syntax: UnaryPlusExpression, '+Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( System.Int64 A.Method()) ([0] OperationKind.InvocationExpression, Type: System.Int64) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_Plus_System_UInt64()
        {
            string source = @"
class A
{
    System.UInt64 Method()
    {
        System.UInt64 i = default(System.UInt64);
        return /*<bind>*/+Method()/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Plus) ([0] OperationKind.UnaryOperatorExpression, Type: System.UInt64) (Syntax: UnaryPlusExpression, '+Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( System.UInt64 A.Method()) ([0] OperationKind.InvocationExpression, Type: System.UInt64) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_Plus_System_Char()
        {
            string source = @"
class A
{
    System.Char Method()
    {
        System.Char i = default(System.Char);
        return /*<bind>*/+Method()/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Plus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int32, IsInvalid) (Syntax: UnaryPlusExpression, '+Method()') (Parent: ConversionExpression)
  Operand: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32, IsInvalid, IsImplicit) (Syntax: InvocationExpression, 'Method()')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IInvocationExpression ( System.Char A.Method()) ([0] OperationKind.InvocationExpression, Type: System.Char, IsInvalid) (Syntax: InvocationExpression, 'Method()')
          Instance Receiver: 
            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'Method')
          Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_Plus_System_Decimal()
        {
            string source = @"
class A
{
    System.Decimal Method()
    {
        System.Decimal i = default(System.Decimal);
        return /*<bind>*/+Method()/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Plus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Decimal) (Syntax: UnaryPlusExpression, '+Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( System.Decimal A.Method()) ([0] OperationKind.InvocationExpression, Type: System.Decimal) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_Plus_System_Single()
        {
            string source = @"
class A
{
    System.Single Method()
    {
        System.Single i = default(System.Single);
        return /*<bind>*/+Method()/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Plus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Single) (Syntax: UnaryPlusExpression, '+Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( System.Single A.Method()) ([0] OperationKind.InvocationExpression, Type: System.Single) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_Plus_System_Double()
        {
            string source = @"
class A
{
    System.Double Method()
    {
        System.Double i = default(System.Double);
        return /*<bind>*/+Method()/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Plus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Double) (Syntax: UnaryPlusExpression, '+Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( System.Double A.Method()) ([0] OperationKind.InvocationExpression, Type: System.Double) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_Plus_System_Boolean()
        {
            string source = @"
class A
{
    System.Boolean Method()
    {
        System.Boolean i = default(System.Boolean);
        return /*<bind>*/+Method()/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Plus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Object, IsInvalid) (Syntax: UnaryPlusExpression, '+Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( System.Boolean A.Method()) ([0] OperationKind.InvocationExpression, Type: System.Boolean, IsInvalid) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_Plus_System_Object()
        {
            string source = @"
class A
{
    System.Object Method()
    {
        System.Object i = default(System.Object);
        return /*<bind>*/+Method()/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Plus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Object, IsInvalid) (Syntax: UnaryPlusExpression, '+Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( System.Object A.Method()) ([0] OperationKind.InvocationExpression, Type: System.Object, IsInvalid) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_Minus_System_SByte()
        {
            string source = @"
class A
{
    System.SByte Method()
    {
        System.SByte i = default(System.SByte);
        return /*<bind>*/-Method()/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Minus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int32, IsInvalid) (Syntax: UnaryMinusExpression, '-Method()') (Parent: ConversionExpression)
  Operand: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32, IsInvalid, IsImplicit) (Syntax: InvocationExpression, 'Method()')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IInvocationExpression ( System.SByte A.Method()) ([0] OperationKind.InvocationExpression, Type: System.SByte, IsInvalid) (Syntax: InvocationExpression, 'Method()')
          Instance Receiver: 
            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'Method')
          Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_Minus_System_Byte()
        {
            string source = @"
class A
{
    System.Byte Method()
    {
        System.Byte i = default(System.Byte);
        return /*<bind>*/-Method()/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Minus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int32, IsInvalid) (Syntax: UnaryMinusExpression, '-Method()') (Parent: ConversionExpression)
  Operand: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32, IsInvalid, IsImplicit) (Syntax: InvocationExpression, 'Method()')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IInvocationExpression ( System.Byte A.Method()) ([0] OperationKind.InvocationExpression, Type: System.Byte, IsInvalid) (Syntax: InvocationExpression, 'Method()')
          Instance Receiver: 
            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'Method')
          Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_Minus_System_Int16()
        {
            string source = @"
class A
{
    System.Int16 Method()
    {
        System.Int16 i = default(System.Int16);
        return /*<bind>*/-Method()/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Minus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int32, IsInvalid) (Syntax: UnaryMinusExpression, '-Method()') (Parent: ConversionExpression)
  Operand: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32, IsInvalid, IsImplicit) (Syntax: InvocationExpression, 'Method()')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IInvocationExpression ( System.Int16 A.Method()) ([0] OperationKind.InvocationExpression, Type: System.Int16, IsInvalid) (Syntax: InvocationExpression, 'Method()')
          Instance Receiver: 
            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'Method')
          Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_Minus_System_UInt16()
        {
            string source = @"
class A
{
    System.UInt16 Method()
    {
        System.UInt16 i = default(System.UInt16);
        return /*<bind>*/-Method()/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Minus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int32, IsInvalid) (Syntax: UnaryMinusExpression, '-Method()') (Parent: ConversionExpression)
  Operand: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32, IsInvalid, IsImplicit) (Syntax: InvocationExpression, 'Method()')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IInvocationExpression ( System.UInt16 A.Method()) ([0] OperationKind.InvocationExpression, Type: System.UInt16, IsInvalid) (Syntax: InvocationExpression, 'Method()')
          Instance Receiver: 
            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'Method')
          Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_Minus_System_Int32()
        {
            string source = @"
class A
{
    System.Int32 Method()
    {
        System.Int32 i = default(System.Int32);
        return /*<bind>*/-Method()/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Minus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int32) (Syntax: UnaryMinusExpression, '-Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( System.Int32 A.Method()) ([0] OperationKind.InvocationExpression, Type: System.Int32) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_Minus_System_UInt32()
        {
            string source = @"
class A
{
    System.UInt32 Method()
    {
        System.UInt32 i = default(System.UInt32);
        return /*<bind>*/-Method()/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Minus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int64, IsInvalid) (Syntax: UnaryMinusExpression, '-Method()') (Parent: ConversionExpression)
  Operand: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int64, IsInvalid, IsImplicit) (Syntax: InvocationExpression, 'Method()')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IInvocationExpression ( System.UInt32 A.Method()) ([0] OperationKind.InvocationExpression, Type: System.UInt32, IsInvalid) (Syntax: InvocationExpression, 'Method()')
          Instance Receiver: 
            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'Method')
          Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_Minus_System_Int64()
        {
            string source = @"
class A
{
    System.Int64 Method()
    {
        System.Int64 i = default(System.Int64);
        return /*<bind>*/-Method()/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Minus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int64) (Syntax: UnaryMinusExpression, '-Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( System.Int64 A.Method()) ([0] OperationKind.InvocationExpression, Type: System.Int64) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_Minus_System_UInt64()
        {
            string source = @"
class A
{
    System.UInt64 Method()
    {
        System.UInt64 i = default(System.UInt64);
        return /*<bind>*/-Method()/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Minus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Object, IsInvalid) (Syntax: UnaryMinusExpression, '-Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( System.UInt64 A.Method()) ([0] OperationKind.InvocationExpression, Type: System.UInt64, IsInvalid) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_Minus_System_Char()
        {
            string source = @"
class A
{
    System.Char Method()
    {
        System.Char i = default(System.Char);
        return /*<bind>*/-Method()/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Minus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int32, IsInvalid) (Syntax: UnaryMinusExpression, '-Method()') (Parent: ConversionExpression)
  Operand: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32, IsInvalid, IsImplicit) (Syntax: InvocationExpression, 'Method()')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IInvocationExpression ( System.Char A.Method()) ([0] OperationKind.InvocationExpression, Type: System.Char, IsInvalid) (Syntax: InvocationExpression, 'Method()')
          Instance Receiver: 
            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'Method')
          Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_Minus_System_Decimal()
        {
            string source = @"
class A
{
    System.Decimal Method()
    {
        System.Decimal i = default(System.Decimal);
        return /*<bind>*/-Method()/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Minus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Decimal) (Syntax: UnaryMinusExpression, '-Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( System.Decimal A.Method()) ([0] OperationKind.InvocationExpression, Type: System.Decimal) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_Minus_System_Single()
        {
            string source = @"
class A
{
    System.Single Method()
    {
        System.Single i = default(System.Single);
        return /*<bind>*/-Method()/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Minus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Single) (Syntax: UnaryMinusExpression, '-Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( System.Single A.Method()) ([0] OperationKind.InvocationExpression, Type: System.Single) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_Minus_System_Double()
        {
            string source = @"
class A
{
    System.Double Method()
    {
        System.Double i = default(System.Double);
        return /*<bind>*/-Method()/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Minus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Double) (Syntax: UnaryMinusExpression, '-Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( System.Double A.Method()) ([0] OperationKind.InvocationExpression, Type: System.Double) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_Minus_System_Boolean()
        {
            string source = @"
class A
{
    System.Boolean Method()
    {
        System.Boolean i = default(System.Boolean);
        return /*<bind>*/-Method()/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Minus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Object, IsInvalid) (Syntax: UnaryMinusExpression, '-Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( System.Boolean A.Method()) ([0] OperationKind.InvocationExpression, Type: System.Boolean, IsInvalid) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_Minus_System_Object()
        {
            string source = @"
class A
{
    System.Object Method()
    {
        System.Object i = default(System.Object);
        return /*<bind>*/-Method()/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Minus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Object, IsInvalid) (Syntax: UnaryMinusExpression, '-Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( System.Object A.Method()) ([0] OperationKind.InvocationExpression, Type: System.Object, IsInvalid) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Type_LogicalNot_System_Boolean()
        {
            string source = @"
class A
{
    System.Boolean Method()
    {
        System.Boolean i = default(System.Boolean);
        return /*<bind>*/!i/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Not) ([0] OperationKind.UnaryOperatorExpression, Type: System.Boolean) (Syntax: LogicalNotExpression, '!i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Boolean) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_LogicalNot_System_Boolean()
        {
            string source = @"
class A
{
    System.Boolean Method()
    {
        System.Boolean i = default(System.Boolean);
        return /*<bind>*/!Method()/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Not) ([0] OperationKind.UnaryOperatorExpression, Type: System.Boolean) (Syntax: LogicalNotExpression, '!Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( System.Boolean A.Method()) ([0] OperationKind.InvocationExpression, Type: System.Boolean) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Type_BitwiseNot_System_SByte()
        {
            string source = @"
class A
{
    System.SByte Method()
    {
        System.SByte i = default(System.SByte);
        return /*<bind>*/~i/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.BitwiseNegation) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int32, IsInvalid) (Syntax: BitwiseNotExpression, '~i') (Parent: ConversionExpression)
  Operand: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'i')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.SByte, IsInvalid) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Type_BitwiseNot_System_Byte()
        {
            string source = @"
class A
{
    System.Byte Method()
    {
        System.Byte i = default(System.Byte);
        return /*<bind>*/~i/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.BitwiseNegation) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int32, IsInvalid) (Syntax: BitwiseNotExpression, '~i') (Parent: ConversionExpression)
  Operand: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'i')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Byte, IsInvalid) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Type_BitwiseNot_System_Int16()
        {
            string source = @"
class A
{
    System.Int16 Method()
    {
        System.Int16 i = default(System.Int16);
        return /*<bind>*/~i/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.BitwiseNegation) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int32, IsInvalid) (Syntax: BitwiseNotExpression, '~i') (Parent: ConversionExpression)
  Operand: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'i')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int16, IsInvalid) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Type_BitwiseNot_System_UInt16()
        {
            string source = @"
class A
{
    System.UInt16 Method()
    {
        System.UInt16 i = default(System.UInt16);
        return /*<bind>*/~i/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.BitwiseNegation) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int32, IsInvalid) (Syntax: BitwiseNotExpression, '~i') (Parent: ConversionExpression)
  Operand: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'i')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.UInt16, IsInvalid) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Type_BitwiseNot_System_Int32()
        {
            string source = @"
class A
{
    System.Int32 Method()
    {
        System.Int32 i = default(System.Int32);
        return /*<bind>*/~i/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.BitwiseNegation) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int32) (Syntax: BitwiseNotExpression, '~i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Type_BitwiseNot_System_UInt32()
        {
            string source = @"
class A
{
    System.UInt32 Method()
    {
        System.UInt32 i = default(System.UInt32);
        return /*<bind>*/~i/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.BitwiseNegation) ([0] OperationKind.UnaryOperatorExpression, Type: System.UInt32) (Syntax: BitwiseNotExpression, '~i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.UInt32) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Type_BitwiseNot_System_Int64()
        {
            string source = @"
class A
{
    System.Int64 Method()
    {
        System.Int64 i = default(System.Int64);
        return /*<bind>*/~i/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.BitwiseNegation) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int64) (Syntax: BitwiseNotExpression, '~i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int64) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Type_BitwiseNot_System_UInt64()
        {
            string source = @"
class A
{
    System.UInt64 Method()
    {
        System.UInt64 i = default(System.UInt64);
        return /*<bind>*/~i/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.BitwiseNegation) ([0] OperationKind.UnaryOperatorExpression, Type: System.UInt64) (Syntax: BitwiseNotExpression, '~i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.UInt64) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Type_BitwiseNot_System_Char()
        {
            string source = @"
class A
{
    System.Char Method()
    {
        System.Char i = default(System.Char);
        return /*<bind>*/~i/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.BitwiseNegation) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int32, IsInvalid) (Syntax: BitwiseNotExpression, '~i') (Parent: ConversionExpression)
  Operand: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'i')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Char, IsInvalid) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Type_BitwiseNot_System_Decimal()
        {
            string source = @"
class A
{
    System.Decimal Method()
    {
        System.Decimal i = default(System.Decimal);
        return /*<bind>*/~i/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.BitwiseNegation) ([0] OperationKind.UnaryOperatorExpression, Type: System.Object, IsInvalid) (Syntax: BitwiseNotExpression, '~i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Decimal, IsInvalid) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Type_BitwiseNot_System_Single()
        {
            string source = @"
class A
{
    System.Single Method()
    {
        System.Single i = default(System.Single);
        return /*<bind>*/~i/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.BitwiseNegation) ([0] OperationKind.UnaryOperatorExpression, Type: System.Object, IsInvalid) (Syntax: BitwiseNotExpression, '~i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Single, IsInvalid) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Type_BitwiseNot_System_Double()
        {
            string source = @"
class A
{
    System.Double Method()
    {
        System.Double i = default(System.Double);
        return /*<bind>*/~i/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.BitwiseNegation) ([0] OperationKind.UnaryOperatorExpression, Type: System.Object, IsInvalid) (Syntax: BitwiseNotExpression, '~i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Double, IsInvalid) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Type_BitwiseNot_System_Boolean()
        {
            string source = @"
class A
{
    System.Boolean Method()
    {
        System.Boolean i = default(System.Boolean);
        return /*<bind>*/~i/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.BitwiseNegation) ([0] OperationKind.UnaryOperatorExpression, Type: System.Object, IsInvalid) (Syntax: BitwiseNotExpression, '~i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Boolean, IsInvalid) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Type_BitwiseNot_System_Object()
        {
            string source = @"
class A
{
    System.Object Method()
    {
        System.Object i = default(System.Object);
        return /*<bind>*/~i/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.BitwiseNegation) ([0] OperationKind.UnaryOperatorExpression, Type: System.Object, IsInvalid) (Syntax: BitwiseNotExpression, '~i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Object, IsInvalid) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_BitwiseNot_System_SByte()
        {
            string source = @"
class A
{
    System.SByte Method()
    {
        System.SByte i = default(System.SByte);
        return /*<bind>*/~Method()/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.BitwiseNegation) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int32, IsInvalid) (Syntax: BitwiseNotExpression, '~Method()') (Parent: ConversionExpression)
  Operand: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32, IsInvalid, IsImplicit) (Syntax: InvocationExpression, 'Method()')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IInvocationExpression ( System.SByte A.Method()) ([0] OperationKind.InvocationExpression, Type: System.SByte, IsInvalid) (Syntax: InvocationExpression, 'Method()')
          Instance Receiver: 
            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'Method')
          Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_BitwiseNot_System_Byte()
        {
            string source = @"
class A
{
    System.Byte Method()
    {
        System.Byte i = default(System.Byte);
        return /*<bind>*/~Method()/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.BitwiseNegation) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int32, IsInvalid) (Syntax: BitwiseNotExpression, '~Method()') (Parent: ConversionExpression)
  Operand: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32, IsInvalid, IsImplicit) (Syntax: InvocationExpression, 'Method()')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IInvocationExpression ( System.Byte A.Method()) ([0] OperationKind.InvocationExpression, Type: System.Byte, IsInvalid) (Syntax: InvocationExpression, 'Method()')
          Instance Receiver: 
            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'Method')
          Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_BitwiseNot_System_Int16()
        {
            string source = @"
class A
{
    System.Int16 Method()
    {
        System.Int16 i = default(System.Int16);
        return /*<bind>*/~Method()/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.BitwiseNegation) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int32, IsInvalid) (Syntax: BitwiseNotExpression, '~Method()') (Parent: ConversionExpression)
  Operand: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32, IsInvalid, IsImplicit) (Syntax: InvocationExpression, 'Method()')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IInvocationExpression ( System.Int16 A.Method()) ([0] OperationKind.InvocationExpression, Type: System.Int16, IsInvalid) (Syntax: InvocationExpression, 'Method()')
          Instance Receiver: 
            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'Method')
          Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_BitwiseNot_System_UInt16()
        {
            string source = @"
class A
{
    System.UInt16 Method()
    {
        System.UInt16 i = default(System.UInt16);
        return /*<bind>*/~Method()/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.BitwiseNegation) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int32, IsInvalid) (Syntax: BitwiseNotExpression, '~Method()') (Parent: ConversionExpression)
  Operand: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32, IsInvalid, IsImplicit) (Syntax: InvocationExpression, 'Method()')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IInvocationExpression ( System.UInt16 A.Method()) ([0] OperationKind.InvocationExpression, Type: System.UInt16, IsInvalid) (Syntax: InvocationExpression, 'Method()')
          Instance Receiver: 
            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'Method')
          Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_BitwiseNot_System_Int32()
        {
            string source = @"
class A
{
    System.Int32 Method()
    {
        System.Int32 i = default(System.Int32);
        return /*<bind>*/~Method()/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.BitwiseNegation) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int32) (Syntax: BitwiseNotExpression, '~Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( System.Int32 A.Method()) ([0] OperationKind.InvocationExpression, Type: System.Int32) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_BitwiseNot_System_UInt32()
        {
            string source = @"
class A
{
    System.UInt32 Method()
    {
        System.UInt32 i = default(System.UInt32);
        return /*<bind>*/~Method()/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.BitwiseNegation) ([0] OperationKind.UnaryOperatorExpression, Type: System.UInt32) (Syntax: BitwiseNotExpression, '~Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( System.UInt32 A.Method()) ([0] OperationKind.InvocationExpression, Type: System.UInt32) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_BitwiseNot_System_Int64()
        {
            string source = @"
class A
{
    System.Int64 Method()
    {
        System.Int64 i = default(System.Int64);
        return /*<bind>*/~Method()/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.BitwiseNegation) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int64) (Syntax: BitwiseNotExpression, '~Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( System.Int64 A.Method()) ([0] OperationKind.InvocationExpression, Type: System.Int64) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_BitwiseNot_System_UInt64()
        {
            string source = @"
class A
{
    System.UInt64 Method()
    {
        System.UInt64 i = default(System.UInt64);
        return /*<bind>*/~Method()/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.BitwiseNegation) ([0] OperationKind.UnaryOperatorExpression, Type: System.UInt64) (Syntax: BitwiseNotExpression, '~Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( System.UInt64 A.Method()) ([0] OperationKind.InvocationExpression, Type: System.UInt64) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_BitwiseNot_System_Char()
        {
            string source = @"
class A
{
    System.Char Method()
    {
        System.Char i = default(System.Char);
        return /*<bind>*/~Method()/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.BitwiseNegation) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int32, IsInvalid) (Syntax: BitwiseNotExpression, '~Method()') (Parent: ConversionExpression)
  Operand: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32, IsInvalid, IsImplicit) (Syntax: InvocationExpression, 'Method()')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IInvocationExpression ( System.Char A.Method()) ([0] OperationKind.InvocationExpression, Type: System.Char, IsInvalid) (Syntax: InvocationExpression, 'Method()')
          Instance Receiver: 
            IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'Method')
          Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_BitwiseNot_System_Decimal()
        {
            string source = @"
class A
{
    System.Decimal Method()
    {
        System.Decimal i = default(System.Decimal);
        return /*<bind>*/~Method()/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.BitwiseNegation) ([0] OperationKind.UnaryOperatorExpression, Type: System.Object, IsInvalid) (Syntax: BitwiseNotExpression, '~Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( System.Decimal A.Method()) ([0] OperationKind.InvocationExpression, Type: System.Decimal, IsInvalid) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_BitwiseNot_System_Single()
        {
            string source = @"
class A
{
    System.Single Method()
    {
        System.Single i = default(System.Single);
        return /*<bind>*/~Method()/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.BitwiseNegation) ([0] OperationKind.UnaryOperatorExpression, Type: System.Object, IsInvalid) (Syntax: BitwiseNotExpression, '~Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( System.Single A.Method()) ([0] OperationKind.InvocationExpression, Type: System.Single, IsInvalid) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_BitwiseNot_System_Double()
        {
            string source = @"
class A
{
    System.Double Method()
    {
        System.Double i = default(System.Double);
        return /*<bind>*/~Method()/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.BitwiseNegation) ([0] OperationKind.UnaryOperatorExpression, Type: System.Object, IsInvalid) (Syntax: BitwiseNotExpression, '~Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( System.Double A.Method()) ([0] OperationKind.InvocationExpression, Type: System.Double, IsInvalid) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_BitwiseNot_System_Boolean()
        {
            string source = @"
class A
{
    System.Boolean Method()
    {
        System.Boolean i = default(System.Boolean);
        return /*<bind>*/~Method()/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.BitwiseNegation) ([0] OperationKind.UnaryOperatorExpression, Type: System.Object, IsInvalid) (Syntax: BitwiseNotExpression, '~Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( System.Boolean A.Method()) ([0] OperationKind.InvocationExpression, Type: System.Boolean, IsInvalid) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_BitwiseNot_System_Object()
        {
            string source = @"
class A
{
    System.Object Method()
    {
        System.Object i = default(System.Object);
        return /*<bind>*/~Method()/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.BitwiseNegation) ([0] OperationKind.UnaryOperatorExpression, Type: System.Object, IsInvalid) (Syntax: BitwiseNotExpression, '~Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( System.Object A.Method()) ([0] OperationKind.InvocationExpression, Type: System.Object, IsInvalid) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Type_Plus_dynamic()
        {
            string source = @"
class A
{
    dynamic Method()
    {
        dynamic i = default(dynamic);
        return /*<bind>*/+i/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Plus) ([0] OperationKind.UnaryOperatorExpression, Type: dynamic) (Syntax: UnaryPlusExpression, '+i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: dynamic) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Type_Minus_dynamic()
        {
            string source = @"
class A
{
    dynamic Method()
    {
        dynamic i = default(dynamic);
        return /*<bind>*/-i/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Minus) ([0] OperationKind.UnaryOperatorExpression, Type: dynamic) (Syntax: UnaryMinusExpression, '-i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: dynamic) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Type_BitwiseNot_dynamic()
        {
            string source = @"
class A
{
    dynamic Method()
    {
        dynamic i = default(dynamic);
        return /*<bind>*/~i/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.BitwiseNegation) ([0] OperationKind.UnaryOperatorExpression, Type: dynamic) (Syntax: BitwiseNotExpression, '~i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: dynamic) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Type_LogicalNot_dynamic()
        {
            string source = @"
class A
{
    dynamic Method()
    {
        dynamic i = default(dynamic);
        return /*<bind>*/!i/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Not) ([0] OperationKind.UnaryOperatorExpression, Type: dynamic) (Syntax: LogicalNotExpression, '!i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: dynamic) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_Plus_dynamic()
        {
            string source = @"
class A
{
    dynamic Method()
    {
        dynamic i = default(dynamic);
        return /*<bind>*/+Method()/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Plus) ([0] OperationKind.UnaryOperatorExpression, Type: dynamic) (Syntax: UnaryPlusExpression, '+Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( dynamic A.Method()) ([0] OperationKind.InvocationExpression, Type: dynamic) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_Minus_dynamic()
        {
            string source = @"
class A
{
    dynamic Method()
    {
        dynamic i = default(dynamic);
        return /*<bind>*/-Method()/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Minus) ([0] OperationKind.UnaryOperatorExpression, Type: dynamic) (Syntax: UnaryMinusExpression, '-Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( dynamic A.Method()) ([0] OperationKind.InvocationExpression, Type: dynamic) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_BitwiseNot_dynamic()
        {
            string source = @"
class A
{
    dynamic Method()
    {
        dynamic i = default(dynamic);
        return /*<bind>*/~Method()/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.BitwiseNegation) ([0] OperationKind.UnaryOperatorExpression, Type: dynamic) (Syntax: BitwiseNotExpression, '~Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( dynamic A.Method()) ([0] OperationKind.InvocationExpression, Type: dynamic) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_LogicalNot_dynamic()
        {
            string source = @"
class A
{
    dynamic Method()
    {
        dynamic i = default(dynamic);
        return /*<bind>*/!Method()/*</bind>*/;
    }
}

";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Not) ([0] OperationKind.UnaryOperatorExpression, Type: dynamic) (Syntax: LogicalNotExpression, '!Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( dynamic A.Method()) ([0] OperationKind.InvocationExpression, Type: dynamic) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Type_Plus_Enum()
        {
            string source = @"
class A
{
    Enum Method()
    {
        Enum i = default(Enum);
        return /*<bind>*/+i/*</bind>*/;
    }
}
enum Enum { A, B }
";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Plus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Object, IsInvalid) (Syntax: UnaryPlusExpression, '+i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: Enum, IsInvalid) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Type_Minus_Enum()
        {
            string source = @"
class A
{
    Enum Method()
    {
        Enum i = default(Enum);
        return /*<bind>*/-i/*</bind>*/;
    }
}
enum Enum { A, B }
";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Minus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Object, IsInvalid) (Syntax: UnaryMinusExpression, '-i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: Enum, IsInvalid) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Type_BitwiseNot_Enum()
        {
            string source = @"
class A
{
    Enum Method()
    {
        Enum i = default(Enum);
        return /*<bind>*/~i/*</bind>*/;
    }
}
enum Enum { A, B }
";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.BitwiseNegation) ([0] OperationKind.UnaryOperatorExpression, Type: Enum) (Syntax: BitwiseNotExpression, '~i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: Enum) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_Plus_Enum()
        {
            string source = @"
class A
{
    Enum Method()
    {
        Enum i = default(Enum);
        return /*<bind>*/+Method()/*</bind>*/;
    }
}
enum Enum { A, B }
";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Plus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Object, IsInvalid) (Syntax: UnaryPlusExpression, '+Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( Enum A.Method()) ([0] OperationKind.InvocationExpression, Type: Enum, IsInvalid) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_Minus_Enum()
        {
            string source = @"
class A
{
    Enum Method()
    {
        Enum i = default(Enum);
        return /*<bind>*/-Method()/*</bind>*/;
    }
}
enum Enum { A, B }
";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Minus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Object, IsInvalid) (Syntax: UnaryMinusExpression, '-Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( Enum A.Method()) ([0] OperationKind.InvocationExpression, Type: Enum, IsInvalid) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_BitwiseNot_Enum()
        {
            string source = @"
class A
{
    Enum Method()
    {
        Enum i = default(Enum);
        return /*<bind>*/~Method()/*</bind>*/;
    }
}
enum Enum { A, B }
";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.BitwiseNegation) ([0] OperationKind.UnaryOperatorExpression, Type: Enum) (Syntax: BitwiseNotExpression, '~Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( Enum A.Method()) ([0] OperationKind.InvocationExpression, Type: Enum) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Type_Plus_CustomType()
        {
            string source = @"
class A
{
    CustomType Method()
    {
        CustomType i = default(CustomType);
        return /*<bind>*/+i/*</bind>*/;
    }
}
public struct CustomType
{
    public static CustomType operator +(CustomType x)
    {
        return x;
    }
    public static CustomType operator -(CustomType x)
    {
        return x;
    }
    public static CustomType operator !(CustomType x)
    {
        return x;
    }
    public static CustomType operator ~(CustomType x)
    {
        return x;
    }
}
";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Plus) (OperatorMethod: CustomType CustomType.op_UnaryPlus(CustomType x)) ([0] OperationKind.UnaryOperatorExpression, Type: CustomType) (Syntax: UnaryPlusExpression, '+i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: CustomType) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Type_Minus_CustomType()
        {
            string source = @"
class A
{
    CustomType Method()
    {
        CustomType i = default(CustomType);
        return /*<bind>*/-i/*</bind>*/;
    }
}
public struct CustomType
{
    public static CustomType operator +(CustomType x)
    {
        return x;
    }
    public static CustomType operator -(CustomType x)
    {
        return x;
    }
    public static CustomType operator !(CustomType x)
    {
        return x;
    }
    public static CustomType operator ~(CustomType x)
    {
        return x;
    }
}
";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Minus) (OperatorMethod: CustomType CustomType.op_UnaryNegation(CustomType x)) ([0] OperationKind.UnaryOperatorExpression, Type: CustomType) (Syntax: UnaryMinusExpression, '-i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: CustomType) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Type_BitwiseNot_CustomType()
        {
            string source = @"
class A
{
    CustomType Method()
    {
        CustomType i = default(CustomType);
        return /*<bind>*/~i/*</bind>*/;
    }
}
public struct CustomType
{
    public static CustomType operator +(CustomType x)
    {
        return x;
    }
    public static CustomType operator -(CustomType x)
    {
        return x;
    }
    public static CustomType operator !(CustomType x)
    {
        return x;
    }
    public static CustomType operator ~(CustomType x)
    {
        return x;
    }
}
";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.BitwiseNegation) (OperatorMethod: CustomType CustomType.op_OnesComplement(CustomType x)) ([0] OperationKind.UnaryOperatorExpression, Type: CustomType) (Syntax: BitwiseNotExpression, '~i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: CustomType) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Type_LogicalNot_CustomType()
        {
            string source = @"
class A
{
    CustomType Method()
    {
        CustomType i = default(CustomType);
        return /*<bind>*/!i/*</bind>*/;
    }
}
public struct CustomType
{
    public static CustomType operator +(CustomType x)
    {
        return x;
    }
    public static CustomType operator -(CustomType x)
    {
        return x;
    }
    public static CustomType operator !(CustomType x)
    {
        return x;
    }
    public static CustomType operator ~(CustomType x)
    {
        return x;
    }
}
";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Not) (OperatorMethod: CustomType CustomType.op_LogicalNot(CustomType x)) ([0] OperationKind.UnaryOperatorExpression, Type: CustomType) (Syntax: LogicalNotExpression, '!i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: CustomType) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_Plus_CustomType()
        {
            string source = @"
class A
{
    CustomType Method()
    {
        CustomType i = default(CustomType);
        return /*<bind>*/+Method()/*</bind>*/;
    }
}
public struct CustomType
{
    public static CustomType operator +(CustomType x)
    {
        return x;
    }
    public static CustomType operator -(CustomType x)
    {
        return x;
    }
    public static CustomType operator !(CustomType x)
    {
        return x;
    }
    public static CustomType operator ~(CustomType x)
    {
        return x;
    }
}
";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Plus) (OperatorMethod: CustomType CustomType.op_UnaryPlus(CustomType x)) ([0] OperationKind.UnaryOperatorExpression, Type: CustomType) (Syntax: UnaryPlusExpression, '+Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( CustomType A.Method()) ([0] OperationKind.InvocationExpression, Type: CustomType) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_Minus_CustomType()
        {
            string source = @"
class A
{
    CustomType Method()
    {
        CustomType i = default(CustomType);
        return /*<bind>*/-Method()/*</bind>*/;
    }
}
public struct CustomType
{
    public static CustomType operator +(CustomType x)
    {
        return x;
    }
    public static CustomType operator -(CustomType x)
    {
        return x;
    }
    public static CustomType operator !(CustomType x)
    {
        return x;
    }
    public static CustomType operator ~(CustomType x)
    {
        return x;
    }
}
";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Minus) (OperatorMethod: CustomType CustomType.op_UnaryNegation(CustomType x)) ([0] OperationKind.UnaryOperatorExpression, Type: CustomType) (Syntax: UnaryMinusExpression, '-Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( CustomType A.Method()) ([0] OperationKind.InvocationExpression, Type: CustomType) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_BitwiseNot_CustomType()
        {
            string source = @"
class A
{
    CustomType Method()
    {
        CustomType i = default(CustomType);
        return /*<bind>*/~Method()/*</bind>*/;
    }
}
public struct CustomType
{
    public static CustomType operator +(CustomType x)
    {
        return x;
    }
    public static CustomType operator -(CustomType x)
    {
        return x;
    }
    public static CustomType operator !(CustomType x)
    {
        return x;
    }
    public static CustomType operator ~(CustomType x)
    {
        return x;
    }
}
";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.BitwiseNegation) (OperatorMethod: CustomType CustomType.op_OnesComplement(CustomType x)) ([0] OperationKind.UnaryOperatorExpression, Type: CustomType) (Syntax: BitwiseNotExpression, '~Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( CustomType A.Method()) ([0] OperationKind.InvocationExpression, Type: CustomType) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Method_LogicalNot_CustomType()
        {
            string source = @"
class A
{
    CustomType Method()
    {
        CustomType i = default(CustomType);
        return /*<bind>*/!Method()/*</bind>*/;
    }
}
public struct CustomType
{
    public static CustomType operator +(CustomType x)
    {
        return x;
    }
    public static CustomType operator -(CustomType x)
    {
        return x;
    }
    public static CustomType operator !(CustomType x)
    {
        return x;
    }
    public static CustomType operator ~(CustomType x)
    {
        return x;
    }
}
";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Not) (OperatorMethod: CustomType CustomType.op_LogicalNot(CustomType x)) ([0] OperationKind.UnaryOperatorExpression, Type: CustomType) (Syntax: LogicalNotExpression, '!Method()') (Parent: ReturnStatement)
  Operand: 
    IInvocationExpression ( CustomType A.Method()) ([0] OperationKind.InvocationExpression, Type: CustomType) (Syntax: InvocationExpression, 'Method()')
      Instance Receiver: 
        IInstanceReferenceExpression ([0] OperationKind.InstanceReferenceExpression, Type: A, IsImplicit) (Syntax: IdentifierName, 'Method')
      Arguments(0)
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(18135, "https://github.com/dotnet/roslyn/issues/18135")]
        [WorkItem(18160, "https://github.com/dotnet/roslyn/issues/18160")]
        public void Test_UnaryOperatorExpression_Type_And_TrueFalse()
        {
            string source = @"

public struct S
{
    private int value;
    public S(int v)
    {
        value = v;
    }
    public static S operator |(S x, S y)
    {
        return new S(x.value - y.value);
    }
    public static S operator &(S x, S y)
    {
        return new S(x.value + y.value);
    }
    public static bool operator true(S x)
    {
        return x.value > 0;
    }
    public static bool operator false(S x)
    {
        return x.value <= 0;
    }
}
 
class C
{
    public void M()
    {
        var x = new S(2);
        var y = new S(1);
        /*<bind>*/if (x && y) { }/*</bind>*/
    }
}

";
            string expectedOperationTree = @"
IIfStatement ([2] OperationKind.IfStatement) (Syntax: IfStatement, 'if (x && y) { }') (Parent: BlockStatement)
  Condition: 
    IUnaryOperatorExpression (UnaryOperatorKind.True) (OperatorMethod: System.Boolean S.op_True(S x)) ([0] OperationKind.UnaryOperatorExpression, Type: System.Boolean, IsImplicit) (Syntax: LogicalAndExpression, 'x && y')
      Operand: 
        IOperation:  ([0] OperationKind.None) (Syntax: LogicalAndExpression, 'x && y')
          Children(2):
            ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: S) (Syntax: IdentifierName, 'x')
            ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: S) (Syntax: IdentifierName, 'y')
  IfTrue: 
    IBlockStatement (0 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ }')
  IfFalse: 
    null
";
            VerifyOperationTreeForTest<IfStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(18135, "https://github.com/dotnet/roslyn/issues/18135")]
        [WorkItem(18160, "https://github.com/dotnet/roslyn/issues/18160")]
        public void Test_UnaryOperatorExpression_Type_Or_TrueFalse()
        {
            string source = @"

public struct S
{
    private int value;
    public S(int v)
    {
        value = v;
    }
    public static S operator |(S x, S y)
    {
        return new S(x.value - y.value);
    }
    public static S operator &(S x, S y)
    {
        return new S(x.value + y.value);
    }
    public static bool operator true(S x)
    {
        return x.value > 0;
    }
    public static bool operator false(S x)
    {
        return x.value <= 0;
    }
}
 
class C
{
    public void M()
    {
        var x = new S(2);
        var y = new S(1);
        /*<bind>*/if (x || y) { }/*</bind>*/
    }
}

";
            string expectedOperationTree = @"
IIfStatement ([2] OperationKind.IfStatement) (Syntax: IfStatement, 'if (x || y) { }') (Parent: BlockStatement)
  Condition: 
    IUnaryOperatorExpression (UnaryOperatorKind.True) (OperatorMethod: System.Boolean S.op_True(S x)) ([0] OperationKind.UnaryOperatorExpression, Type: System.Boolean, IsImplicit) (Syntax: LogicalOrExpression, 'x || y')
      Operand: 
        IOperation:  ([0] OperationKind.None) (Syntax: LogicalOrExpression, 'x || y')
          Children(2):
            ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: S) (Syntax: IdentifierName, 'x')
            ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: S) (Syntax: IdentifierName, 'y')
  IfTrue: 
    IBlockStatement (0 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ }')
  IfFalse: 
    null
";
            VerifyOperationTreeForTest<IfStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_With_CustomType_NoRightOperator()
        {
            string source = @"
class A
{
    CustomType Method()
    {
        CustomType i = default(CustomType);
        return /*<bind>*/+i/*</bind>*/;
    }
}
public struct CustomType
{
}
";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Plus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Object, IsInvalid) (Syntax: UnaryPlusExpression, '+i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: CustomType, IsInvalid) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_With_CustomType_DerivedTypes()
        {
            string source = @"
class A
{
    BaseType Method()
    {
        var i = default(DerivedType);
        return /*<bind>*/+i/*</bind>*/;
    }
}
public class BaseType
{
    public static BaseType operator +(BaseType x)
    {
        return new BaseType();
    }
}

public class DerivedType : BaseType
{
}
";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Plus) (OperatorMethod: BaseType BaseType.op_UnaryPlus(BaseType x)) ([0] OperationKind.UnaryOperatorExpression, Type: BaseType) (Syntax: UnaryPlusExpression, '+i') (Parent: ReturnStatement)
  Operand: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: BaseType, IsImplicit) (Syntax: IdentifierName, 'i')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: DerivedType) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_With_CustomType_ImplicitConversion()
        {
            string source = @"
class A
{
    BaseType Method()
    {
        var i = default(DerivedType);
        return /*<bind>*/+i/*</bind>*/;
    }
}
public class BaseType
{
    public static BaseType operator +(BaseType x)
    {
        return new BaseType();
    }
}

public class DerivedType 
{
    public static implicit operator BaseType(DerivedType x)
    {
        return new BaseType();
    }
}
";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Plus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Object, IsInvalid) (Syntax: UnaryPlusExpression, '+i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: DerivedType, IsInvalid) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_With_CustomType_ExplicitConversion()
        {
            string source = @"
class A
{
    BaseType Method()
    {
        var i = default(DerivedType);
        return /*<bind>*/+i/*</bind>*/;
    }
}
public class BaseType
{
    public static BaseType operator +(BaseType x)
    {
        return new BaseType();
    }
}

public class DerivedType 
{
    public static explicit operator BaseType(DerivedType x)
    {
        return new BaseType();
    }
}
";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Plus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Object, IsInvalid) (Syntax: UnaryPlusExpression, '+i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: DerivedType, IsInvalid) (Syntax: IdentifierName, 'i')
";

            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_With_CustomType_Malformed_Operator()
        {
            string source = @"
class A
{
    BaseType Method()
    {
        var i = default(BaseType);
        return /*<bind>*/+i/*</bind>*/;
    }
}
public class BaseType
{
    public static BaseType operator +(int x)
    {
        return new BaseType();
    }
}
";
            string expectedOperationTree = @"
IUnaryOperatorExpression (UnaryOperatorKind.Plus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Object, IsInvalid) (Syntax: UnaryPlusExpression, '+i') (Parent: ReturnStatement)
  Operand: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: BaseType, IsInvalid) (Syntax: IdentifierName, 'i')
";

            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        [WorkItem(18160, "https://github.com/dotnet/roslyn/issues/18160")]
        public void Test_BinaryExpressionSyntax_Type_And_TrueFalse_Condition()
        {
            string source = @"
public struct S
{
    private int value;
    public S(int v)
    {
        value = v;
    }
    public static S operator |(S x, S y)
    {
        return new S(x.value - y.value);
    }
    public static S operator &(S x, S y)
    {
        return new S(x.value + y.value);
    }
    public static bool operator true(S x)
    {
        return x.value > 0;
    }
    public static bool operator false(S x)
    {
        return x.value <= 0;
    }
}

class C
{
    public void M()
    {
        var x = new S(2);
        var y = new S(1);
        if (/*<bind>*/x && y/*</bind>*/) { }
    }
}
";
            string expectedOperationTree = @"
IOperation:  ([0] OperationKind.None) (Syntax: LogicalAndExpression, 'x && y') (Parent: UnaryOperatorExpression)
  Children(2):
    ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: S) (Syntax: IdentifierName, 'x')
    ILocalReferenceExpression: y ([1] OperationKind.LocalReferenceExpression, Type: S) (Syntax: IdentifierName, 'y')
";
            VerifyOperationTreeForTest<BinaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_IncrementExpression()
        {
            string source = @"
class A
{
    int Method()
    {
        var i = 1;
        return /*<bind>*/++i/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IIncrementOrDecrementExpression (Prefix) ([0] OperationKind.IncrementExpression, Type: System.Int32) (Syntax: PreIncrementExpression, '++i') (Parent: ReturnStatement)
  Target: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
";

            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_DecrementExpression()
        {
            string source = @"
class A
{
    int Method()
    {
        var i = 1;
        return /*<bind>*/--i/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IIncrementOrDecrementExpression (Prefix) ([0] OperationKind.DecrementExpression, Type: System.Int32) (Syntax: PreDecrementExpression, '--i') (Parent: ReturnStatement)
  Target: 
    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Nullable()
        {
            string source = @"
class A
{
    void Method()
    {
        var i = /*<bind>*/(int?)1/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IConversionExpression (Explicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32?) (Syntax: CastExpression, '(int?)1') (Parent: VariableInitializer)
  Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  Operand: 
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
";
            VerifyOperationTreeForTest<CastExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void Test_UnaryOperatorExpression_Pointer()
        {
            string source = @"
class A
{
    unsafe void Method()
    {
        int[] a = new int[5] {10, 20, 30, 40, 50};
        
        fixed (int* p = &a[0])  
        {  
            int* p2 = p;  
            int p1 = /*<bind>*/*p2/*</bind>*/;  
        }  
    }
}
";
            string expectedOperationTree = @"
IOperation:  ([0] OperationKind.None) (Syntax: PointerIndirectionExpression, '*p2') (Parent: VariableInitializer)
  Children(1):
    ILocalReferenceExpression: p2 ([0] OperationKind.LocalReferenceExpression, Type: System.Int32*) (Syntax: IdentifierName, 'p2')
";

            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void VerifyLiftedUnaryOperators1()
        {
            var source = @"
 class C
 {
     void F(int? x)
     {
         var y = /*<bind>*/-x/*</bind>*/;
     }
 }";

            string expectedOperationTree =
@"
IUnaryOperatorExpression (UnaryOperatorKind.Minus, IsLifted) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int32?) (Syntax: UnaryMinusExpression, '-x') (Parent: VariableInitializer)
  Operand: 
    IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32?) (Syntax: IdentifierName, 'x')
";

            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void VerifyNonLiftedUnaryOperators1()
        {
            var source = @"
class C
{
    void F(int x)
    {
        var y = /*<bind>*/-x/*</bind>*/;
    }
}";

            string expectedOperationTree =
@"
IUnaryOperatorExpression (UnaryOperatorKind.Minus) ([0] OperationKind.UnaryOperatorExpression, Type: System.Int32) (Syntax: UnaryMinusExpression, '-x') (Parent: VariableInitializer)
  Operand: 
    IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x')
";

            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void VerifyLiftedUserDefinedUnaryOperators1()
        {
            var source = @"
struct C
{
    public static C operator -(C c) { }
    void F(C? x)
    {
        var y = /*<bind>*/-x/*</bind>*/;
    }
}";

            string expectedOperationTree =
@"
IUnaryOperatorExpression (UnaryOperatorKind.Minus, IsLifted) (OperatorMethod: C C.op_UnaryNegation(C c)) ([0] OperationKind.UnaryOperatorExpression, Type: C?) (Syntax: UnaryMinusExpression, '-x') (Parent: VariableInitializer)
  Operand: 
    IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: C?) (Syntax: IdentifierName, 'x')
";

            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void VerifyNonLiftedUserDefinedUnaryOperators1()
        {
            var source = @"
struct C
{
    public static C operator -(C c) { }
    void F(C x)
    {
        var y = /*<bind>*/-x/*</bind>*/;
    }
}";

            string expectedOperationTree =
@"
IUnaryOperatorExpression (UnaryOperatorKind.Minus) (OperatorMethod: C C.op_UnaryNegation(C c)) ([0] OperationKind.UnaryOperatorExpression, Type: C) (Syntax: UnaryMinusExpression, '-x') (Parent: VariableInitializer)
  Operand: 
    IParameterReferenceExpression: x ([0] OperationKind.ParameterReferenceExpression, Type: C) (Syntax: IdentifierName, 'x')
";

            VerifyOperationTreeForTest<PrefixUnaryExpressionSyntax>(source, expectedOperationTree);
        }
    }
}
