﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp.Test.Utilities;
using Microsoft.CodeAnalysis.Test.Utilities;
using Roslyn.Test.Utilities;
using Xunit;

namespace Microsoft.CodeAnalysis.CSharp.UnitTests
{
    public partial class IOperationTests : SemanticModelTestBase
    {
        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(10856, "https://github.com/dotnet/roslyn/issues/10856")]
        public void TupleExpression_NoConversions()
        {
            string source = @"
using System;

class C
{
    static void Main()
    {
        (int, int) t = /*<bind>*/(1, 2)/*</bind>*/;
        Console.WriteLine(t);
    }
}
";
            string expectedOperationTree = @"
ITupleExpression ([0] OperationKind.TupleExpression, Type: (System.Int32, System.Int32)) (Syntax: TupleExpression, '(1, 2)') (Parent: VariableInitializer)
  Elements(2):
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<TupleExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(10856, "https://github.com/dotnet/roslyn/issues/10856")]
        public void TupleExpression_NoConversions_ParentVariableDeclaration()
        {
            string source = @"
using System;

class C
{
    static void Main()
    {
        /*<bind>*/(int, int) t = (1, 2);/*</bind>*/
        Console.WriteLine(t);
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, '(int, int) t = (1, 2);') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 't = (1, 2)')
    Variables: Local_1: (System.Int32, System.Int32) t
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= (1, 2)')
        ITupleExpression ([0] OperationKind.TupleExpression, Type: (System.Int32, System.Int32)) (Syntax: TupleExpression, '(1, 2)')
          Elements(2):
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
            ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(10856, "https://github.com/dotnet/roslyn/issues/10856")]
        public void TupleExpression_ImplicitConversions()
        {
            string source = @"
using System;

class C
{
    static void Main()
    {
        (uint, uint) t = /*<bind>*/(1, 2)/*</bind>*/;
        Console.WriteLine(t);
    }
}
";
            string expectedOperationTree = @"
ITupleExpression ([0] OperationKind.TupleExpression, Type: (System.UInt32, System.UInt32)) (Syntax: TupleExpression, '(1, 2)') (Parent: ConversionExpression)
  Elements(2):
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.UInt32, Constant: 1, IsImplicit) (Syntax: NumericLiteralExpression, '1')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.UInt32, Constant: 2, IsImplicit) (Syntax: NumericLiteralExpression, '2')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<TupleExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(10856, "https://github.com/dotnet/roslyn/issues/10856")]
        public void TupleExpression_ImplicitConversions_ParentVariableDeclaration()
        {
            string source = @"
using System;

class C
{
    static void Main()
    {
        /*<bind>*/(uint, uint) t = (1, 2);/*</bind>*/
        Console.WriteLine(t);
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, '(uint, uint) t = (1, 2);') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 't = (1, 2)')
    Variables: Local_1: (System.UInt32, System.UInt32) t
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= (1, 2)')
        IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: (System.UInt32, System.UInt32), IsImplicit) (Syntax: TupleExpression, '(1, 2)')
          Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            ITupleExpression ([0] OperationKind.TupleExpression, Type: (System.UInt32, System.UInt32)) (Syntax: TupleExpression, '(1, 2)')
              Elements(2):
                IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.UInt32, Constant: 1, IsImplicit) (Syntax: NumericLiteralExpression, '1')
                  Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                  Operand: 
                    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
                IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.UInt32, Constant: 2, IsImplicit) (Syntax: NumericLiteralExpression, '2')
                  Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                  Operand: 
                    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(10856, "https://github.com/dotnet/roslyn/issues/10856")]
        public void TupleExpression_ImplicitConversionsWithTypedExpression()
        {
            string source = @"
using System;

class C
{
    static void Main()
    {
        int a = 1;
        int b = 2;
        (long, long) t = /*<bind>*/(a, b)/*</bind>*/;
        Console.WriteLine(t);
    }
}
";
            string expectedOperationTree = @"
ITupleExpression ([0] OperationKind.TupleExpression, Type: (System.Int64 a, System.Int64 b)) (Syntax: TupleExpression, '(a, b)') (Parent: ConversionExpression)
  Elements(2):
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int64, IsImplicit) (Syntax: IdentifierName, 'a')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILocalReferenceExpression: a ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'a')
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Int64, IsImplicit) (Syntax: IdentifierName, 'b')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILocalReferenceExpression: b ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'b')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<TupleExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(10856, "https://github.com/dotnet/roslyn/issues/10856")]
        public void TupleExpression_ImplicitConversionsWithTypedExpression_WithParentDeclaration()
        {
            string source = @"
using System;

class C
{
    static void Main()
    {
        int a = 1;
        int b = 2;
        /*<bind>*/(long, long) t = (a, b);/*</bind>*/
        Console.WriteLine(t);
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (1 declarations) ([2] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, '(long, long) t = (a, b);') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 't = (a, b)')
    Variables: Local_1: (System.Int64, System.Int64) t
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= (a, b)')
        IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: (System.Int64, System.Int64), IsImplicit) (Syntax: TupleExpression, '(a, b)')
          Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            ITupleExpression ([0] OperationKind.TupleExpression, Type: (System.Int64 a, System.Int64 b)) (Syntax: TupleExpression, '(a, b)')
              Elements(2):
                IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int64, IsImplicit) (Syntax: IdentifierName, 'a')
                  Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                  Operand: 
                    ILocalReferenceExpression: a ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'a')
                IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.Int64, IsImplicit) (Syntax: IdentifierName, 'b')
                  Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                  Operand: 
                    ILocalReferenceExpression: b ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'b')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(10856, "https://github.com/dotnet/roslyn/issues/10856")]
        public void TupleExpression_ImplicitConversionFromNull()
        {
            string source = @"
using System;

class C
{
    static void Main()
    {
        (uint, string) t = /*<bind>*/(1, null)/*</bind>*/;
        Console.WriteLine(t);
    }
}
";
            string expectedOperationTree = @"
ITupleExpression ([0] OperationKind.TupleExpression, Type: (System.UInt32, System.String)) (Syntax: TupleExpression, '(1, null)') (Parent: ConversionExpression)
  Elements(2):
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.UInt32, Constant: 1, IsImplicit) (Syntax: NumericLiteralExpression, '1')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.String, Constant: null, IsImplicit) (Syntax: NullLiteralExpression, 'null')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: null, Constant: null) (Syntax: NullLiteralExpression, 'null')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<TupleExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(10856, "https://github.com/dotnet/roslyn/issues/10856")]
        public void TupleExpression_ImplicitConversionFromNull_ParentVariableDeclaration()
        {
            string source = @"
using System;

class C
{
    static void Main()
    {
        /*<bind>*/(uint, string) t = (1, null);/*</bind>*/
        Console.WriteLine(t);
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, '(uint, stri ...  (1, null);') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 't = (1, null)')
    Variables: Local_1: (System.UInt32, System.String) t
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= (1, null)')
        IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: (System.UInt32, System.String), IsImplicit) (Syntax: TupleExpression, '(1, null)')
          Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            ITupleExpression ([0] OperationKind.TupleExpression, Type: (System.UInt32, System.String)) (Syntax: TupleExpression, '(1, null)')
              Elements(2):
                IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.UInt32, Constant: 1, IsImplicit) (Syntax: NumericLiteralExpression, '1')
                  Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                  Operand: 
                    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
                IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.String, Constant: null, IsImplicit) (Syntax: NullLiteralExpression, 'null')
                  Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
                  Operand: 
                    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: null, Constant: null) (Syntax: NullLiteralExpression, 'null')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(10856, "https://github.com/dotnet/roslyn/issues/10856")]
        public void TupleExpression_NamedElements()
        {
            string source = @"
using System;

class C
{
    static void Main()
    {
        var t = /*<bind>*/(A: 1, B: 2)/*</bind>*/;
        Console.WriteLine(t);
    }
}
";
            string expectedOperationTree = @"
ITupleExpression ([0] OperationKind.TupleExpression, Type: (System.Int32 A, System.Int32 B)) (Syntax: TupleExpression, '(A: 1, B: 2)') (Parent: VariableInitializer)
  Elements(2):
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<TupleExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(10856, "https://github.com/dotnet/roslyn/issues/10856")]
        public void TupleExpression_NamedElements_ParentVariableDeclaration()
        {
            string source = @"
using System;

class C
{
    static void Main()
    {
        /*<bind>*/var t = (A: 1, B: 2);/*</bind>*/
        Console.WriteLine(t);
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'var t = (A: 1, B: 2);') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 't = (A: 1, B: 2)')
    Variables: Local_1: (System.Int32 A, System.Int32 B) t
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= (A: 1, B: 2)')
        ITupleExpression ([0] OperationKind.TupleExpression, Type: (System.Int32 A, System.Int32 B)) (Syntax: TupleExpression, '(A: 1, B: 2)')
          Elements(2):
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
            ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(10856, "https://github.com/dotnet/roslyn/issues/10856")]
        public void TupleExpression_NamedElementsInTupleType()
        {
            string source = @"
using System;

class C
{
    static void Main()
    {
        (int A, int B) t = /*<bind>*/(1, 2)/*</bind>*/;
        Console.WriteLine(t);
    }
}
";
            string expectedOperationTree = @"
ITupleExpression ([0] OperationKind.TupleExpression, Type: (System.Int32, System.Int32)) (Syntax: TupleExpression, '(1, 2)') (Parent: ConversionExpression)
  Elements(2):
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<TupleExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(10856, "https://github.com/dotnet/roslyn/issues/10856")]
        public void TupleExpression_NamedElementsInTupleType_ParentVariableDeclaration()
        {
            string source = @"
using System;

class C
{
    static void Main()
    {
        /*<bind>*/(int A, int B) t = (1, 2);/*</bind>*/
        Console.WriteLine(t);
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, '(int A, int ... t = (1, 2);') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 't = (1, 2)')
    Variables: Local_1: (System.Int32 A, System.Int32 B) t
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= (1, 2)')
        IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: (System.Int32 A, System.Int32 B), IsImplicit) (Syntax: TupleExpression, '(1, 2)')
          Conversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            ITupleExpression ([0] OperationKind.TupleExpression, Type: (System.Int32, System.Int32)) (Syntax: TupleExpression, '(1, 2)')
              Elements(2):
                ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
                ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(10856, "https://github.com/dotnet/roslyn/issues/10856")]
        public void TupleExpression_NamedElementsAndImplicitConversions()
        {
            string source = @"
using System;

class C
{
    static void Main()
    {
        (short, string) t = /*<bind>*/(A: 1, B: null)/*</bind>*/;
        Console.WriteLine(t);
    }
}
";
            string expectedOperationTree = @"
ITupleExpression ([0] OperationKind.TupleExpression, Type: (System.Int16 A, System.String B)) (Syntax: TupleExpression, '(A: 1, B: null)') (Parent: ConversionExpression)
  Elements(2):
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int16, Constant: 1, IsImplicit) (Syntax: NumericLiteralExpression, '1')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.String, Constant: null, IsImplicit) (Syntax: NullLiteralExpression, 'null')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: null, Constant: null) (Syntax: NullLiteralExpression, 'null')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS8123: The tuple element name 'A' is ignored because a different name or no name is specified by the target type '(short, string)'.
                //         (short, string) t = /*<bind>*/(A: 1, B: null)/*</bind>*/;
                Diagnostic(ErrorCode.WRN_TupleLiteralNameMismatch, "A: 1").WithArguments("A", "(short, string)").WithLocation(8, 40),
                // CS8123: The tuple element name 'B' is ignored because a different name or no name is specified by the target type '(short, string)'.
                //         (short, string) t = /*<bind>*/(A: 1, B: null)/*</bind>*/;
                Diagnostic(ErrorCode.WRN_TupleLiteralNameMismatch, "B: null").WithArguments("B", "(short, string)").WithLocation(8, 46)
            };

            VerifyOperationTreeAndDiagnosticsForTest<TupleExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(10856, "https://github.com/dotnet/roslyn/issues/10856")]
        public void TupleExpression_NamedElementsAndImplicitConversions_ParentVariableDeclaration()
        {
            string source = @"
using System;

class C
{
    static void Main()
    {
        /*<bind>*/(short, string) t = (A: 1, B: null);/*</bind>*/
        Console.WriteLine(t);
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, '(short, str ... , B: null);') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 't = (A: 1, B: null)')
    Variables: Local_1: (System.Int16, System.String) t
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= (A: 1, B: null)')
        IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: (System.Int16, System.String), IsImplicit) (Syntax: TupleExpression, '(A: 1, B: null)')
          Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            ITupleExpression ([0] OperationKind.TupleExpression, Type: (System.Int16 A, System.String B)) (Syntax: TupleExpression, '(A: 1, B: null)')
              Elements(2):
                IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int16, Constant: 1, IsImplicit) (Syntax: NumericLiteralExpression, '1')
                  Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                  Operand: 
                    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
                IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.String, Constant: null, IsImplicit) (Syntax: NullLiteralExpression, 'null')
                  Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
                  Operand: 
                    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: null, Constant: null) (Syntax: NullLiteralExpression, 'null')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS8123: The tuple element name 'A' is ignored because a different name or no name is specified by the target type '(short, string)'.
                //         /*<bind>*/(short, string) t = (A: 1, B: null)/*</bind>*/;
                Diagnostic(ErrorCode.WRN_TupleLiteralNameMismatch, "A: 1").WithArguments("A", "(short, string)").WithLocation(8, 40),
                // CS8123: The tuple element name 'B' is ignored because a different name or no name is specified by the target type '(short, string)'.
                //         /*<bind>*/(short, string) t = (A: 1, B: null)/*</bind>*/;
                Diagnostic(ErrorCode.WRN_TupleLiteralNameMismatch, "B: null").WithArguments("B", "(short, string)").WithLocation(8, 46)
            };

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(10856, "https://github.com/dotnet/roslyn/issues/10856")]
        public void TupleExpression_UserDefinedConversionsForArguments()
        {
            string source = @"
using System;

class C
{
    private readonly int _x;
    public C(int x)
    {
        _x = x;
    }

    public static implicit operator C(int value)
    {
        return new C(value);
    }

    public static implicit operator short(C c)
    {
        return (short)c._x;
    }

    public static implicit operator string(C c)
    {
        return c._x.ToString();
    }

    public void M(C c1)
    {
        (short, string) t = /*<bind>*/(new C(0), c1)/*</bind>*/;
        Console.WriteLine(t);
    }
}
";
            string expectedOperationTree = @"
ITupleExpression ([0] OperationKind.TupleExpression, Type: (System.Int16, System.String c1)) (Syntax: TupleExpression, '(new C(0), c1)') (Parent: ConversionExpression)
  Elements(2):
    IConversionExpression (Implicit, TryCast: False, Unchecked) (OperatorMethod: System.Int16 C.op_Implicit(C c)) ([0] OperationKind.ConversionExpression, Type: System.Int16, IsImplicit) (Syntax: ObjectCreationExpression, 'new C(0)')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: True) (MethodSymbol: System.Int16 C.op_Implicit(C c))
      Operand: 
        IObjectCreationExpression (Constructor: C..ctor(System.Int32 x)) ([0] OperationKind.ObjectCreationExpression, Type: C) (Syntax: ObjectCreationExpression, 'new C(0)')
          Arguments(1):
            IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([0] OperationKind.Argument) (Syntax: Argument, '0')
              ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
              InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
              OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          Initializer: 
            null
    IConversionExpression (Implicit, TryCast: False, Unchecked) (OperatorMethod: System.String C.op_Implicit(C c)) ([1] OperationKind.ConversionExpression, Type: System.String, IsImplicit) (Syntax: IdentifierName, 'c1')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: True) (MethodSymbol: System.String C.op_Implicit(C c))
      Operand: 
        IParameterReferenceExpression: c1 ([0] OperationKind.ParameterReferenceExpression, Type: C) (Syntax: IdentifierName, 'c1')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<TupleExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(10856, "https://github.com/dotnet/roslyn/issues/10856")]
        public void TupleExpression_UserDefinedConversionsForArguments_ParentVariableDeclaration()
        {
            string source = @"
using System;

class C
{
    private readonly int _x;
    public C(int x)
    {
        _x = x;
    }

    public static implicit operator C(int value)
    {
        return new C(value);
    }

    public static implicit operator short(C c)
    {
        return (short)c._x;
    }

    public static implicit operator string(C c)
    {
        return c._x.ToString();
    }

    public void M(C c1)
    {
        /*<bind>*/(short, string) t = (new C(0), c1);/*</bind>*/
        Console.WriteLine(t);
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, '(short, str ...  C(0), c1);') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 't = (new C(0), c1)')
    Variables: Local_1: (System.Int16, System.String) t
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= (new C(0), c1)')
        IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: (System.Int16, System.String), IsImplicit) (Syntax: TupleExpression, '(new C(0), c1)')
          Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            ITupleExpression ([0] OperationKind.TupleExpression, Type: (System.Int16, System.String c1)) (Syntax: TupleExpression, '(new C(0), c1)')
              Elements(2):
                IConversionExpression (Implicit, TryCast: False, Unchecked) (OperatorMethod: System.Int16 C.op_Implicit(C c)) ([0] OperationKind.ConversionExpression, Type: System.Int16, IsImplicit) (Syntax: ObjectCreationExpression, 'new C(0)')
                  Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: True) (MethodSymbol: System.Int16 C.op_Implicit(C c))
                  Operand: 
                    IObjectCreationExpression (Constructor: C..ctor(System.Int32 x)) ([0] OperationKind.ObjectCreationExpression, Type: C) (Syntax: ObjectCreationExpression, 'new C(0)')
                      Arguments(1):
                        IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([0] OperationKind.Argument) (Syntax: Argument, '0')
                          ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
                          InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                          OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                      Initializer: 
                        null
                IConversionExpression (Implicit, TryCast: False, Unchecked) (OperatorMethod: System.String C.op_Implicit(C c)) ([1] OperationKind.ConversionExpression, Type: System.String, IsImplicit) (Syntax: IdentifierName, 'c1')
                  Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: True) (MethodSymbol: System.String C.op_Implicit(C c))
                  Operand: 
                    IParameterReferenceExpression: c1 ([0] OperationKind.ParameterReferenceExpression, Type: C) (Syntax: IdentifierName, 'c1')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(10856, "https://github.com/dotnet/roslyn/issues/10856")]
        public void TupleExpression_UserDefinedConversionFromTupleExpression()
        {
            string source = @"
using System;

class C
{
    private readonly int _x;
    public C(int x)
    {
        _x = x;
    }

    public static implicit operator C((int, string) x)
    {
        return new C(x.Item1);
    }

    public static implicit operator (int, string) (C c)
    {
        return (c._x, c._x.ToString());
    }

    public void M(C c1)
    {
        C t = /*<bind>*/(0, null)/*</bind>*/;
        Console.WriteLine(t);
    }
}
";
            string expectedOperationTree = @"
ITupleExpression ([0] OperationKind.TupleExpression, Type: (System.Int32, System.String)) (Syntax: TupleExpression, '(0, null)') (Parent: ConversionExpression)
  Elements(2):
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.String, Constant: null, IsImplicit) (Syntax: NullLiteralExpression, 'null')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: null, Constant: null) (Syntax: NullLiteralExpression, 'null')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<TupleExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(10856, "https://github.com/dotnet/roslyn/issues/10856")]
        public void TupleExpression_UserDefinedConversionFromTupleExpression_ParentVariableDeclaration()
        {
            string source = @"
using System;

class C
{
    private readonly int _x;
    public C(int x)
    {
        _x = x;
    }

    public static implicit operator C((int, string) x)
    {
        return new C(x.Item1);
    }

    public static implicit operator (int, string) (C c)
    {
        return (c._x, c._x.ToString());
    }

    public void M(C c1)
    {
        /*<bind>*/C t = (0, null);/*</bind>*/
        Console.WriteLine(t);
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'C t = (0, null);') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 't = (0, null)')
    Variables: Local_1: C t
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= (0, null)')
        IConversionExpression (Implicit, TryCast: False, Unchecked) (OperatorMethod: C C.op_Implicit((System.Int32, System.String) x)) ([0] OperationKind.ConversionExpression, Type: C, IsImplicit) (Syntax: TupleExpression, '(0, null)')
          Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: True) (MethodSymbol: C C.op_Implicit((System.Int32, System.String) x))
          Operand: 
            IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: (System.Int32, System.String), IsImplicit) (Syntax: TupleExpression, '(0, null)')
              Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
              Operand: 
                ITupleExpression ([0] OperationKind.TupleExpression, Type: (System.Int32, System.String)) (Syntax: TupleExpression, '(0, null)')
                  Elements(2):
                    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
                    IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.String, Constant: null, IsImplicit) (Syntax: NullLiteralExpression, 'null')
                      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
                      Operand: 
                        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: null, Constant: null) (Syntax: NullLiteralExpression, 'null')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(10856, "https://github.com/dotnet/roslyn/issues/10856")]
        public void TupleExpression_UserDefinedConversionToTupleType()
        {
            string source = @"
using System;

class C
{
    private readonly int _x;
    public C(int x)
    {
        _x = x;
    }

    public static implicit operator C((int, string) x)
    {
        return new C(x.Item1);
    }

    public static implicit operator (int, string) (C c)
    {
        return (c._x, c._x.ToString());
    }

    public void M(C c1)
    {
        (int, string) t = /*<bind>*/c1/*</bind>*/;
        Console.WriteLine(t);
    }
}
";
            string expectedOperationTree = @"
IParameterReferenceExpression: c1 ([0] OperationKind.ParameterReferenceExpression, Type: C) (Syntax: IdentifierName, 'c1') (Parent: ConversionExpression)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<IdentifierNameSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(10856, "https://github.com/dotnet/roslyn/issues/10856")]
        public void TupleExpression_UserDefinedConversionToTupleType_ParentVariableDeclaration()
        {
            string source = @"
using System;

class C
{
    private readonly int _x;
    public C(int x)
    {
        _x = x;
    }

    public static implicit operator C((int, string) x)
    {
        return new C(x.Item1);
    }

    public static implicit operator (int, string) (C c)
    {
        return (c._x, c._x.ToString());
    }

    public void M(C c1)
    {
        /*<bind>*/(int, string) t = c1;/*</bind>*/
        Console.WriteLine(t);
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, '(int, string) t = c1;') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 't = c1')
    Variables: Local_1: (System.Int32, System.String) t
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= c1')
        IConversionExpression (Implicit, TryCast: False, Unchecked) (OperatorMethod: (System.Int32, System.String) C.op_Implicit(C c)) ([0] OperationKind.ConversionExpression, Type: (System.Int32, System.String), IsImplicit) (Syntax: IdentifierName, 'c1')
          Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: True) (MethodSymbol: (System.Int32, System.String) C.op_Implicit(C c))
          Operand: 
            IParameterReferenceExpression: c1 ([0] OperationKind.ParameterReferenceExpression, Type: C) (Syntax: IdentifierName, 'c1')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(10856, "https://github.com/dotnet/roslyn/issues/10856")]
        public void TupleExpression_InvalidConversion()
        {
            string source = @"
using System;

class C
{
    private readonly int _x;
    public C(int x)
    {
        _x = x;
    }

    public static implicit operator C(int value)
    {
        return new C(value);
    }

    public static implicit operator int(C c)
    {
        return (short)c._x;
    }

    public static implicit operator string(C c)
    {
        return c._x.ToString();
    }

    public void M(C c1)
    {
        (short, string) t = /*<bind>*/(new C(0), c1)/*</bind>*/;
        Console.WriteLine(t);
    }
}
";
            string expectedOperationTree = @"
ITupleExpression ([0] OperationKind.TupleExpression, Type: (System.Int16, System.String c1), IsInvalid) (Syntax: TupleExpression, '(new C(0), c1)') (Parent: ConversionExpression)
  Elements(2):
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int16, IsInvalid, IsImplicit) (Syntax: ObjectCreationExpression, 'new C(0)')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IConversionExpression (Implicit, TryCast: False, Unchecked) (OperatorMethod: System.Int32 C.op_Implicit(C c)) ([0] OperationKind.ConversionExpression, Type: System.Int32, IsInvalid, IsImplicit) (Syntax: ObjectCreationExpression, 'new C(0)')
          Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: True) (MethodSymbol: System.Int32 C.op_Implicit(C c))
          Operand: 
            IObjectCreationExpression (Constructor: C..ctor(System.Int32 x)) ([0] OperationKind.ObjectCreationExpression, Type: C, IsInvalid) (Syntax: ObjectCreationExpression, 'new C(0)')
              Arguments(1):
                IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([0] OperationKind.Argument, IsInvalid) (Syntax: Argument, '0')
                  ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0, IsInvalid) (Syntax: NumericLiteralExpression, '0')
                  InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                  OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
              Initializer: 
                null
    IConversionExpression (Implicit, TryCast: False, Unchecked) (OperatorMethod: System.String C.op_Implicit(C c)) ([1] OperationKind.ConversionExpression, Type: System.String, IsImplicit) (Syntax: IdentifierName, 'c1')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: True) (MethodSymbol: System.String C.op_Implicit(C c))
      Operand: 
        IParameterReferenceExpression: c1 ([0] OperationKind.ParameterReferenceExpression, Type: C) (Syntax: IdentifierName, 'c1')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0029: Cannot implicitly convert type 'C' to 'short'
                //         (short, string) t = /*<bind>*/(new C(0), c1)/*</bind>*/;
                Diagnostic(ErrorCode.ERR_NoImplicitConv, "new C(0)").WithArguments("C", "short").WithLocation(29, 40)
            };

            VerifyOperationTreeAndDiagnosticsForTest<TupleExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(10856, "https://github.com/dotnet/roslyn/issues/10856")]
        public void TupleExpression_InvalidConversion_ParentVariableDeclaration()
        {
            string source = @"
using System;

class C
{
    private readonly int _x;
    public C(int x)
    {
        _x = x;
    }

    public static implicit operator C(int value)
    {
        return new C(value);
    }

    public static implicit operator int(C c)
    {
        return (short)c._x;
    }

    public static implicit operator string(C c)
    {
        return c._x.ToString();
    }

    public void M(C c1)
    {
        /*<bind>*/(short, string) t = (new C(0), c1);/*</bind>*/
        Console.WriteLine(t);
    }
}
";
            string expectedOperationTree = @"
IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement, IsInvalid) (Syntax: LocalDeclarationStatement, '(short, str ...  C(0), c1);') (Parent: BlockStatement)
  IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration, IsInvalid) (Syntax: VariableDeclarator, 't = (new C(0), c1)')
    Variables: Local_1: (System.Int16, System.String) t
    Initializer: 
      IVariableInitializer ([0] OperationKind.VariableInitializer, IsInvalid) (Syntax: EqualsValueClause, '= (new C(0), c1)')
        IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: (System.Int16, System.String), IsInvalid, IsImplicit) (Syntax: TupleExpression, '(new C(0), c1)')
          Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            ITupleExpression ([0] OperationKind.TupleExpression, Type: (System.Int16, System.String c1), IsInvalid) (Syntax: TupleExpression, '(new C(0), c1)')
              Elements(2):
                IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int16, IsInvalid, IsImplicit) (Syntax: ObjectCreationExpression, 'new C(0)')
                  Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                  Operand: 
                    IConversionExpression (Implicit, TryCast: False, Unchecked) (OperatorMethod: System.Int32 C.op_Implicit(C c)) ([0] OperationKind.ConversionExpression, Type: System.Int32, IsInvalid, IsImplicit) (Syntax: ObjectCreationExpression, 'new C(0)')
                      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: True) (MethodSymbol: System.Int32 C.op_Implicit(C c))
                      Operand: 
                        IObjectCreationExpression (Constructor: C..ctor(System.Int32 x)) ([0] OperationKind.ObjectCreationExpression, Type: C, IsInvalid) (Syntax: ObjectCreationExpression, 'new C(0)')
                          Arguments(1):
                            IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([0] OperationKind.Argument, IsInvalid) (Syntax: Argument, '0')
                              ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0, IsInvalid) (Syntax: NumericLiteralExpression, '0')
                              InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                              OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                          Initializer: 
                            null
                IConversionExpression (Implicit, TryCast: False, Unchecked) (OperatorMethod: System.String C.op_Implicit(C c)) ([1] OperationKind.ConversionExpression, Type: System.String, IsImplicit) (Syntax: IdentifierName, 'c1')
                  Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: True) (MethodSymbol: System.String C.op_Implicit(C c))
                  Operand: 
                    IParameterReferenceExpression: c1 ([0] OperationKind.ParameterReferenceExpression, Type: C) (Syntax: IdentifierName, 'c1')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0029: Cannot implicitly convert type 'C' to 'short'
                //         /*<bind>*/(short, string) t = (new C(0), c1)/*</bind>*/;
                Diagnostic(ErrorCode.ERR_NoImplicitConv, "new C(0)").WithArguments("C", "short").WithLocation(29, 40)
            };

            VerifyOperationTreeAndDiagnosticsForTest<LocalDeclarationStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(10856, "https://github.com/dotnet/roslyn/issues/10856")]
        public void TupleExpression_Deconstruction()
        {
            string source = @"
class Point
{
    public int X { get; }
    public int Y { get; }

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public void Deconstruct(out int x, out int y)
    {
        x = X;
        y = Y;
    }
}

class Class1
{
    public void M()
    {
        /*<bind>*/var (x, y) = new Point(0, 1)/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IDeconstructionAssignmentExpression ([0] OperationKind.DeconstructionAssignmentExpression, Type: (System.Int32 x, System.Int32 y)) (Syntax: SimpleAssignmentExpression, 'var (x, y)  ... Point(0, 1)') (Parent: ExpressionStatement)
  Left: 
    IDeclarationExpression ([0] OperationKind.DeclarationExpression, Type: (System.Int32 x, System.Int32 y)) (Syntax: DeclarationExpression, 'var (x, y)')
      ITupleExpression ([0] OperationKind.TupleExpression, Type: (System.Int32 x, System.Int32 y)) (Syntax: ParenthesizedVariableDesignation, '(x, y)')
        Elements(2):
          ILocalReferenceExpression: x (IsDeclaration: True) ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: SingleVariableDesignation, 'x')
          ILocalReferenceExpression: y (IsDeclaration: True) ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: SingleVariableDesignation, 'y')
  Right: 
    IObjectCreationExpression (Constructor: Point..ctor(System.Int32 x, System.Int32 y)) ([1] OperationKind.ObjectCreationExpression, Type: Point) (Syntax: ObjectCreationExpression, 'new Point(0, 1)')
      Arguments(2):
        IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([0] OperationKind.Argument) (Syntax: Argument, '0')
          ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
          InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
        IArgument (ArgumentKind.Explicit, Matching Parameter: y) ([1] OperationKind.Argument) (Syntax: Argument, '1')
          ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
          InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Initializer: 
        null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<AssignmentExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(10856, "https://github.com/dotnet/roslyn/issues/10856")]
        public void TupleExpression_Deconstruction_ForEach()
        {
            string source = @"
class Point
{
    public int X { get; }
    public int Y { get; }

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public void Deconstruct(out uint x, out uint y)
    {
        x = 0;
        y = 0;
    }
}

class Class1
{
    public void M()
    {
        /*<bind>*/foreach (var (x, y) in new Point[]{ new Point(0, 1) })
        {
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IForEachLoopStatement (LoopKind.ForEach) ([0] OperationKind.LoopStatement) (Syntax: ForEachVariableStatement, 'foreach (va ... }') (Parent: BlockStatement)
  Locals: Local_1: System.UInt32 x
    Local_2: System.UInt32 y
  LoopControlVariable: 
    IDeclarationExpression ([1] OperationKind.DeclarationExpression, Type: (System.UInt32 x, System.UInt32 y)) (Syntax: DeclarationExpression, 'var (x, y)')
      ITupleExpression ([0] OperationKind.TupleExpression, Type: (System.UInt32 x, System.UInt32 y)) (Syntax: ParenthesizedVariableDesignation, '(x, y)')
        Elements(2):
          ILocalReferenceExpression: x (IsDeclaration: True) ([0] OperationKind.LocalReferenceExpression, Type: System.UInt32) (Syntax: SingleVariableDesignation, 'x')
          ILocalReferenceExpression: y (IsDeclaration: True) ([1] OperationKind.LocalReferenceExpression, Type: System.UInt32) (Syntax: SingleVariableDesignation, 'y')
  Collection: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Collections.IEnumerable, IsImplicit) (Syntax: ArrayCreationExpression, 'new Point[] ... int(0, 1) }')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        IArrayCreationExpression ([0] OperationKind.ArrayCreationExpression, Type: Point[]) (Syntax: ArrayCreationExpression, 'new Point[] ... int(0, 1) }')
          Dimension Sizes(1):
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1, IsImplicit) (Syntax: ArrayCreationExpression, 'new Point[] ... int(0, 1) }')
          Initializer: 
            IArrayInitializer (1 elements) ([1] OperationKind.ArrayInitializer) (Syntax: ArrayInitializerExpression, '{ new Point(0, 1) }')
              Element Values(1):
                IObjectCreationExpression (Constructor: Point..ctor(System.Int32 x, System.Int32 y)) ([0] OperationKind.ObjectCreationExpression, Type: Point) (Syntax: ObjectCreationExpression, 'new Point(0, 1)')
                  Arguments(2):
                    IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([0] OperationKind.Argument) (Syntax: Argument, '0')
                      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
                      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    IArgument (ArgumentKind.Explicit, Matching Parameter: y) ([1] OperationKind.Argument) (Syntax: Argument, '1')
                      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
                      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                  Initializer: 
                    null
  Body: 
    IBlockStatement (0 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
  NextVariables(0)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<ForEachVariableStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(10856, "https://github.com/dotnet/roslyn/issues/10856")]
        public void TupleExpression_DeconstructionWithConversion()
        {
            string source = @"
class Point
{
    public int X { get; }
    public int Y { get; }

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public void Deconstruct(out uint x, out uint y)
    {
        x = 0;
        y = 0;
    }
}

class Class1
{
    public void M()
    {
        /*<bind>*/(uint x, uint y) = new Point(0, 1)/*</bind>*/;
    }
}
";
            string expectedOperationTree = @"
IDeconstructionAssignmentExpression ([0] OperationKind.DeconstructionAssignmentExpression, Type: (System.UInt32 x, System.UInt32 y)) (Syntax: SimpleAssignmentExpression, '(uint x, ui ... Point(0, 1)') (Parent: ExpressionStatement)
  Left: 
    ITupleExpression ([0] OperationKind.TupleExpression, Type: (System.UInt32 x, System.UInt32 y)) (Syntax: TupleExpression, '(uint x, uint y)')
      Elements(2):
        IDeclarationExpression ([0] OperationKind.DeclarationExpression, Type: System.UInt32) (Syntax: DeclarationExpression, 'uint x')
          ILocalReferenceExpression: x (IsDeclaration: True) ([0] OperationKind.LocalReferenceExpression, Type: System.UInt32) (Syntax: SingleVariableDesignation, 'x')
        IDeclarationExpression ([1] OperationKind.DeclarationExpression, Type: System.UInt32) (Syntax: DeclarationExpression, 'uint y')
          ILocalReferenceExpression: y (IsDeclaration: True) ([0] OperationKind.LocalReferenceExpression, Type: System.UInt32) (Syntax: SingleVariableDesignation, 'y')
  Right: 
    IObjectCreationExpression (Constructor: Point..ctor(System.Int32 x, System.Int32 y)) ([1] OperationKind.ObjectCreationExpression, Type: Point) (Syntax: ObjectCreationExpression, 'new Point(0, 1)')
      Arguments(2):
        IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([0] OperationKind.Argument) (Syntax: Argument, '0')
          ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
          InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
        IArgument (ArgumentKind.Explicit, Matching Parameter: y) ([1] OperationKind.Argument) (Syntax: Argument, '1')
          ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
          InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Initializer: 
        null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<AssignmentExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }
    }
}
