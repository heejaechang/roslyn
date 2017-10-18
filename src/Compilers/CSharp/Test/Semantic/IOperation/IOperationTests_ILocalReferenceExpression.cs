// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.


using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Test.Utilities;
using Xunit;

namespace Microsoft.CodeAnalysis.CSharp.UnitTests
{
    public partial class IOperationTests : SemanticModelTestBase
    {
        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void ILocalReferenceExpression_OutVar()
        {
            string source = @"
using System;

public class C1
{
    public virtual void M1()
    {
        M2(out /*<bind>*/var i/*</bind>*/);
    }

    public void M2(out int i )
    {
        i = 0;
    }
}
";
            string expectedOperationTree = @"
IDeclarationExpression ([0] OperationKind.DeclarationExpression, Type: System.Int32) (Syntax: DeclarationExpression, 'var i') (Parent: Argument)
  ILocalReferenceExpression: i (IsDeclaration: True) ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: SingleVariableDesignation, 'i')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<DeclarationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void ILocalReferenceExpression_DeconstructionDeclaration()
        {
            string source = @"
using System;

public class C1
{
    public virtual void M1()
    {
        /*<bind>*/(var i1, var i2)/*</bind>*/ = (1, 2);
    }
}
";
            string expectedOperationTree = @"
ITupleExpression ([0] OperationKind.TupleExpression, Type: (System.Int32 i1, System.Int32 i2)) (Syntax: TupleExpression, '(var i1, var i2)') (Parent: DeconstructionAssignmentExpression)
  Elements(2):
    IDeclarationExpression ([0] OperationKind.DeclarationExpression, Type: System.Int32) (Syntax: DeclarationExpression, 'var i1')
      ILocalReferenceExpression: i1 (IsDeclaration: True) ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: SingleVariableDesignation, 'i1')
    IDeclarationExpression ([1] OperationKind.DeclarationExpression, Type: System.Int32) (Syntax: DeclarationExpression, 'var i2')
      ILocalReferenceExpression: i2 (IsDeclaration: True) ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: SingleVariableDesignation, 'i2')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<TupleExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void ILocalReferenceExpression_DeconstructionDeclaration_AlternateSyntax()
        {
            string source = @"
using System;

public class C1
{
    public virtual void M1()
    {
        /*<bind>*/var (i1, i2)/*</bind>*/ = (1, 2);
    }
}
";
            string expectedOperationTree = @"
IDeclarationExpression ([0] OperationKind.DeclarationExpression, Type: (System.Int32 i1, System.Int32 i2)) (Syntax: DeclarationExpression, 'var (i1, i2)') (Parent: DeconstructionAssignmentExpression)
  ITupleExpression ([0] OperationKind.TupleExpression, Type: (System.Int32 i1, System.Int32 i2)) (Syntax: ParenthesizedVariableDesignation, '(i1, i2)')
    Elements(2):
      ILocalReferenceExpression: i1 (IsDeclaration: True) ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: SingleVariableDesignation, 'i1')
      ILocalReferenceExpression: i2 (IsDeclaration: True) ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: SingleVariableDesignation, 'i2')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<DeclarationExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }
    }
}
