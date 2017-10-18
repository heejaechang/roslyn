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
        public void TryCatchFinally_Basic()
        {
            string source = @"
using System;

class C
{
    void M(int i)
    {
        /*<bind>*/try
        {
            i = 0;
        }
        catch (Exception ex) when (i > 0)
        {
            throw ex;
        }
        finally
        {
            i = 1;
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
ITryStatement ([0] OperationKind.TryStatement) (Syntax: TryStatement, 'try ... }') (Parent: BlockStatement)
  Body: 
    IBlockStatement (1 statements) ([0] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'i = 0;')
        Expression: 
          ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'i = 0')
            Left: 
              IParameterReferenceExpression: i ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  Catch clauses(1):
    ICatchClause (Exception type: System.Exception) ([1] OperationKind.CatchClause) (Syntax: CatchClause, 'catch (Exce ... }')
      Locals: Local_1: System.Exception ex
      ExceptionDeclarationOrExpression: 
        IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: CatchDeclaration, '(Exception ex)')
          Variables: Local_1: System.Exception ex
          Initializer: 
            null
      Filter: 
        IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanExpression, 'i > 0')
          Left: 
            IParameterReferenceExpression: i ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
          Right: 
            ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
      Handler: 
        IBlockStatement (1 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
          IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ThrowStatement, 'throw ex;')
            Expression: 
              IThrowExpression ([0] OperationKind.ThrowExpression, Type: System.Exception) (Syntax: ThrowStatement, 'throw ex;')
                ILocalReferenceExpression: ex ([0] OperationKind.LocalReferenceExpression, Type: System.Exception) (Syntax: IdentifierName, 'ex')
  Finally: 
    IBlockStatement (1 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'i = 1;')
        Expression: 
          ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'i = 1')
            Left: 
              IParameterReferenceExpression: i ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<TryStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void TryCatchFinally_Parent()
        {
            string source = @"
using System;

class C
{
    void M(int i)
    /*<bind>*/{
        try
        {
            i = 0;
        }
        catch (Exception ex) when (i > 0)
        {
            throw ex;
        }
        finally
        {
            i = 1;
        }
    }/*</bind>*/
}
";
            string expectedOperationTree = @"
IBlockStatement (1 statements) ([Root] OperationKind.BlockStatement) (Syntax: Block, '{ ... }') (Parent: )
  ITryStatement ([0] OperationKind.TryStatement) (Syntax: TryStatement, 'try ... }')
    Body: 
      IBlockStatement (1 statements) ([0] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
        IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'i = 0;')
          Expression: 
            ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'i = 0')
              Left: 
                IParameterReferenceExpression: i ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
              Right: 
                ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
    Catch clauses(1):
      ICatchClause (Exception type: System.Exception) ([1] OperationKind.CatchClause) (Syntax: CatchClause, 'catch (Exce ... }')
        Locals: Local_1: System.Exception ex
        ExceptionDeclarationOrExpression: 
          IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: CatchDeclaration, '(Exception ex)')
            Variables: Local_1: System.Exception ex
            Initializer: 
              null
        Filter: 
          IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanExpression, 'i > 0')
            Left: 
              IParameterReferenceExpression: i ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
        Handler: 
          IBlockStatement (1 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
            IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ThrowStatement, 'throw ex;')
              Expression: 
                IThrowExpression ([0] OperationKind.ThrowExpression, Type: System.Exception) (Syntax: ThrowStatement, 'throw ex;')
                  ILocalReferenceExpression: ex ([0] OperationKind.LocalReferenceExpression, Type: System.Exception) (Syntax: IdentifierName, 'ex')
    Finally: 
      IBlockStatement (1 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
        IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'i = 1;')
          Expression: 
            ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'i = 1')
              Left: 
                IParameterReferenceExpression: i ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
              Right: 
                ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<BlockSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void TryCatch_SingleCatchClause()
        {
            string source = @"
class C
{
    static void Main()
    {
        /*<bind>*/try
        {
        }
        catch (System.IO.IOException e)
        {
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
ITryStatement ([0] OperationKind.TryStatement) (Syntax: TryStatement, 'try ... }') (Parent: BlockStatement)
  Body: 
    IBlockStatement (0 statements) ([0] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
  Catch clauses(1):
    ICatchClause (Exception type: System.IO.IOException) ([1] OperationKind.CatchClause) (Syntax: CatchClause, 'catch (Syst ... }')
      Locals: Local_1: System.IO.IOException e
      ExceptionDeclarationOrExpression: 
        IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: CatchDeclaration, '(System.IO. ... xception e)')
          Variables: Local_1: System.IO.IOException e
          Initializer: 
            null
      Filter: 
        null
      Handler: 
        IBlockStatement (0 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
  Finally: 
    null
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0168: The variable 'e' is declared but never used
                //         catch (System.IO.IOException e)
                Diagnostic(ErrorCode.WRN_UnreferencedVar, "e").WithArguments("e").WithLocation(9, 38)
            };

            VerifyOperationTreeAndDiagnosticsForTest<TryStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void TryCatch_SingleCatchClauseAndFilter()
        {
            string source = @"
class C
{
    static void Main()
    {
        /*<bind>*/try
        {
        }
        catch (System.IO.IOException e) when (e.Message != null)
        {
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
ITryStatement ([0] OperationKind.TryStatement) (Syntax: TryStatement, 'try ... }') (Parent: BlockStatement)
  Body: 
    IBlockStatement (0 statements) ([0] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
  Catch clauses(1):
    ICatchClause (Exception type: System.IO.IOException) ([1] OperationKind.CatchClause) (Syntax: CatchClause, 'catch (Syst ... }')
      Locals: Local_1: System.IO.IOException e
      ExceptionDeclarationOrExpression: 
        IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: CatchDeclaration, '(System.IO. ... xception e)')
          Variables: Local_1: System.IO.IOException e
          Initializer: 
            null
      Filter: 
        IBinaryOperatorExpression (BinaryOperatorKind.NotEquals) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: NotEqualsExpression, 'e.Message != null')
          Left: 
            IPropertyReferenceExpression: System.String System.Exception.Message { get; } ([0] OperationKind.PropertyReferenceExpression, Type: System.String) (Syntax: SimpleMemberAccessExpression, 'e.Message')
              Instance Receiver: 
                ILocalReferenceExpression: e ([0] OperationKind.LocalReferenceExpression, Type: System.IO.IOException) (Syntax: IdentifierName, 'e')
          Right: 
            IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.String, Constant: null, IsImplicit) (Syntax: NullLiteralExpression, 'null')
              Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
              Operand: 
                ILiteralExpression ([0] OperationKind.LiteralExpression, Type: null, Constant: null) (Syntax: NullLiteralExpression, 'null')
      Handler: 
        IBlockStatement (0 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
  Finally: 
    null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<TryStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void TryCatch_MultipleCatchClausesWithDifferentCaughtTypes()
        {
            string source = @"
class C
{
    static void Main()
    {
        /*<bind>*/try
        {
        }
        catch (System.IO.IOException e)
        {
        }
        catch (System.Exception e) when (e.Message != null)
        {
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
ITryStatement ([0] OperationKind.TryStatement) (Syntax: TryStatement, 'try ... }') (Parent: BlockStatement)
  Body: 
    IBlockStatement (0 statements) ([0] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
  Catch clauses(2):
    ICatchClause (Exception type: System.IO.IOException) ([1] OperationKind.CatchClause) (Syntax: CatchClause, 'catch (Syst ... }')
      Locals: Local_1: System.IO.IOException e
      ExceptionDeclarationOrExpression: 
        IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: CatchDeclaration, '(System.IO. ... xception e)')
          Variables: Local_1: System.IO.IOException e
          Initializer: 
            null
      Filter: 
        null
      Handler: 
        IBlockStatement (0 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
    ICatchClause (Exception type: System.Exception) ([2] OperationKind.CatchClause) (Syntax: CatchClause, 'catch (Syst ... }')
      Locals: Local_1: System.Exception e
      ExceptionDeclarationOrExpression: 
        IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: CatchDeclaration, '(System.Exception e)')
          Variables: Local_1: System.Exception e
          Initializer: 
            null
      Filter: 
        IBinaryOperatorExpression (BinaryOperatorKind.NotEquals) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: NotEqualsExpression, 'e.Message != null')
          Left: 
            IPropertyReferenceExpression: System.String System.Exception.Message { get; } ([0] OperationKind.PropertyReferenceExpression, Type: System.String) (Syntax: SimpleMemberAccessExpression, 'e.Message')
              Instance Receiver: 
                ILocalReferenceExpression: e ([0] OperationKind.LocalReferenceExpression, Type: System.Exception) (Syntax: IdentifierName, 'e')
          Right: 
            IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.String, Constant: null, IsImplicit) (Syntax: NullLiteralExpression, 'null')
              Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
              Operand: 
                ILiteralExpression ([0] OperationKind.LiteralExpression, Type: null, Constant: null) (Syntax: NullLiteralExpression, 'null')
      Handler: 
        IBlockStatement (0 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
  Finally: 
    null
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0168: The variable 'e' is declared but never used
                //         catch (System.IO.IOException e)
                Diagnostic(ErrorCode.WRN_UnreferencedVar, "e").WithArguments("e").WithLocation(9, 38)
            };

            VerifyOperationTreeAndDiagnosticsForTest<TryStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void TryCatch_MultipleCatchClausesWithDuplicateCaughtTypes()
        {
            string source = @"
class C
{
    static void Main()
    {
        /*<bind>*/try
        {
        }
        catch (System.IO.IOException e)
        {
        }
        catch (System.IO.IOException e) when (e.Message != null)
        {
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
ITryStatement ([0] OperationKind.TryStatement, IsInvalid) (Syntax: TryStatement, 'try ... }') (Parent: BlockStatement)
  Body: 
    IBlockStatement (0 statements) ([0] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
  Catch clauses(2):
    ICatchClause (Exception type: System.IO.IOException) ([1] OperationKind.CatchClause) (Syntax: CatchClause, 'catch (Syst ... }')
      Locals: Local_1: System.IO.IOException e
      ExceptionDeclarationOrExpression: 
        IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: CatchDeclaration, '(System.IO. ... xception e)')
          Variables: Local_1: System.IO.IOException e
          Initializer: 
            null
      Filter: 
        null
      Handler: 
        IBlockStatement (0 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
    ICatchClause (Exception type: System.IO.IOException) ([2] OperationKind.CatchClause, IsInvalid) (Syntax: CatchClause, 'catch (Syst ... }')
      Locals: Local_1: System.IO.IOException e
      ExceptionDeclarationOrExpression: 
        IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration, IsInvalid) (Syntax: CatchDeclaration, '(System.IO. ... xception e)')
          Variables: Local_1: System.IO.IOException e
          Initializer: 
            null
      Filter: 
        IBinaryOperatorExpression (BinaryOperatorKind.NotEquals) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: NotEqualsExpression, 'e.Message != null')
          Left: 
            IPropertyReferenceExpression: System.String System.Exception.Message { get; } ([0] OperationKind.PropertyReferenceExpression, Type: System.String) (Syntax: SimpleMemberAccessExpression, 'e.Message')
              Instance Receiver: 
                ILocalReferenceExpression: e ([0] OperationKind.LocalReferenceExpression, Type: System.IO.IOException) (Syntax: IdentifierName, 'e')
          Right: 
            IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.String, Constant: null, IsImplicit) (Syntax: NullLiteralExpression, 'null')
              Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
              Operand: 
                ILiteralExpression ([0] OperationKind.LiteralExpression, Type: null, Constant: null) (Syntax: NullLiteralExpression, 'null')
      Handler: 
        IBlockStatement (0 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
  Finally: 
    null
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0160: A previous catch clause already catches all exceptions of this or of a super type ('IOException')
                //         catch (System.IO.IOException e) when (e.Message != null)
                Diagnostic(ErrorCode.ERR_UnreachableCatch, "System.IO.IOException").WithArguments("System.IO.IOException").WithLocation(12, 16),
                // CS0168: The variable 'e' is declared but never used
                //         catch (System.IO.IOException e)
                Diagnostic(ErrorCode.WRN_UnreferencedVar, "e").WithArguments("e").WithLocation(9, 38)
            };

            VerifyOperationTreeAndDiagnosticsForTest<TryStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void TryCatch_CatchClauseWithoutExceptionLocal()
        {
            string source = @"
using System;

class C
{
    static void M(string s)
    {
        /*<bind>*/try
        {
        }
        catch (Exception)
        {
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
ITryStatement ([0] OperationKind.TryStatement) (Syntax: TryStatement, 'try ... }') (Parent: BlockStatement)
  Body: 
    IBlockStatement (0 statements) ([0] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
  Catch clauses(1):
    ICatchClause (Exception type: System.Exception) ([1] OperationKind.CatchClause) (Syntax: CatchClause, 'catch (Exce ... }')
      ExceptionDeclarationOrExpression: 
        null
      Filter: 
        null
      Handler: 
        IBlockStatement (0 statements) ([0] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
  Finally: 
    null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<TryStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void TryCatch_CatchClauseWithoutCaughtTypeOrExceptionLocal()
        {
            string source = @"
class C
{
    static void M(object o)
    {
        /*<bind>*/try
        {
        }
        catch
        {
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
ITryStatement ([0] OperationKind.TryStatement) (Syntax: TryStatement, 'try ... }') (Parent: BlockStatement)
  Body: 
    IBlockStatement (0 statements) ([0] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
  Catch clauses(1):
    ICatchClause (Exception type: null) ([1] OperationKind.CatchClause) (Syntax: CatchClause, 'catch ... }')
      ExceptionDeclarationOrExpression: 
        null
      Filter: 
        null
      Handler: 
        IBlockStatement (0 statements) ([0] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
  Finally: 
    null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<TryStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void TryCatch_FinallyWithoutCatchClause()
        {
            string source = @"
using System;

class C
{
    static void M(string s)
    {
        /*<bind>*/try
        {
        }
        finally
        {
            Console.WriteLine(s);
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
ITryStatement ([0] OperationKind.TryStatement) (Syntax: TryStatement, 'try ... }') (Parent: BlockStatement)
  Body: 
    IBlockStatement (0 statements) ([0] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
  Catch clauses(0)
  Finally: 
    IBlockStatement (1 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'Console.WriteLine(s);')
        Expression: 
          IInvocationExpression (void System.Console.WriteLine(System.String value)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Console.WriteLine(s)')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: Argument, 's')
                IParameterReferenceExpression: s ([0] OperationKind.ParameterReferenceExpression, Type: System.String) (Syntax: IdentifierName, 's')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<TryStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void TryCatch_TryBlockWithLocalDeclaration()
        {
            string source = @"
using System;

class C
{
    static void M(string s)
    {
        /*<bind>*/try
        {
            int i = 0;
        }
        catch (Exception)
        {            
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
ITryStatement ([0] OperationKind.TryStatement) (Syntax: TryStatement, 'try ... }') (Parent: BlockStatement)
  Body: 
    IBlockStatement (1 statements, 1 locals) ([0] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      Locals: Local_1: System.Int32 i
      IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'int i = 0;')
        IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'i = 0')
          Variables: Local_1: System.Int32 i
          Initializer: 
            IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= 0')
              ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  Catch clauses(1):
    ICatchClause (Exception type: System.Exception) ([1] OperationKind.CatchClause) (Syntax: CatchClause, 'catch (Exce ... }')
      ExceptionDeclarationOrExpression: 
        null
      Filter: 
        null
      Handler: 
        IBlockStatement (0 statements) ([0] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
  Finally: 
    null
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0219: The variable 'i' is assigned but its value is never used
                //             int i = 0;
                Diagnostic(ErrorCode.WRN_UnreferencedVarAssg, "i").WithArguments("i").WithLocation(10, 17)
            };

            VerifyOperationTreeAndDiagnosticsForTest<TryStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void TryCatch_CatchClauseWithLocalDeclaration()
        {
            string source = @"
using System;

class C
{
    static void M(string s)
    {
        /*<bind>*/try
        {
        }
        catch (Exception)
        {
            int i = 0;
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
ITryStatement ([0] OperationKind.TryStatement) (Syntax: TryStatement, 'try ... }') (Parent: BlockStatement)
  Body: 
    IBlockStatement (0 statements) ([0] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
  Catch clauses(1):
    ICatchClause (Exception type: System.Exception) ([1] OperationKind.CatchClause) (Syntax: CatchClause, 'catch (Exce ... }')
      ExceptionDeclarationOrExpression: 
        null
      Filter: 
        null
      Handler: 
        IBlockStatement (1 statements, 1 locals) ([0] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
          Locals: Local_1: System.Int32 i
          IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'int i = 0;')
            IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'i = 0')
              Variables: Local_1: System.Int32 i
              Initializer: 
                IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= 0')
                  ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  Finally: 
    null
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0219: The variable 'i' is assigned but its value is never used
                //             int i = 0;
                Diagnostic(ErrorCode.WRN_UnreferencedVarAssg, "i").WithArguments("i").WithLocation(13, 17)
            };

            VerifyOperationTreeAndDiagnosticsForTest<TryStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void TryCatch_CatchFilterWithLocalDeclaration()
        {
            string source = @"
using System;

class C
{
    static void M(object o)
    {
        /*<bind>*/try
        {
        }
        catch (Exception) when (o is string s)
        {            
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
ITryStatement ([0] OperationKind.TryStatement) (Syntax: TryStatement, 'try ... }') (Parent: BlockStatement)
  Body: 
    IBlockStatement (0 statements) ([0] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
  Catch clauses(1):
    ICatchClause (Exception type: System.Exception) ([1] OperationKind.CatchClause) (Syntax: CatchClause, 'catch (Exce ... }')
      Locals: Local_1: System.String s
      ExceptionDeclarationOrExpression: 
        null
      Filter: 
        IIsPatternExpression ([0] OperationKind.IsPatternExpression, Type: System.Boolean) (Syntax: IsPatternExpression, 'o is string s')
          Expression: 
            IParameterReferenceExpression: o ([0] OperationKind.ParameterReferenceExpression, Type: System.Object) (Syntax: IdentifierName, 'o')
          Pattern: 
            IDeclarationPattern (Declared Symbol: System.String s) ([1] OperationKind.DeclarationPattern) (Syntax: DeclarationPattern, 'string s')
      Handler: 
        IBlockStatement (0 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
  Finally: 
    null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<TryStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void TryCatch_CatchFilterAndSourceWithLocalDeclaration()
        {
            string source = @"
using System;

class C
{
    static void M(object o)
    {
        /*<bind>*/try
        {
        }
        catch (Exception e) when (o is string s)
        {            
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
ITryStatement ([0] OperationKind.TryStatement) (Syntax: TryStatement, 'try ... }') (Parent: BlockStatement)
  Body: 
    IBlockStatement (0 statements) ([0] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
  Catch clauses(1):
    ICatchClause (Exception type: System.Exception) ([1] OperationKind.CatchClause) (Syntax: CatchClause, 'catch (Exce ... }')
      Locals: Local_1: System.Exception e
        Local_2: System.String s
      ExceptionDeclarationOrExpression: 
        IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: CatchDeclaration, '(Exception e)')
          Variables: Local_1: System.Exception e
          Initializer: 
            null
      Filter: 
        IIsPatternExpression ([1] OperationKind.IsPatternExpression, Type: System.Boolean) (Syntax: IsPatternExpression, 'o is string s')
          Expression: 
            IParameterReferenceExpression: o ([0] OperationKind.ParameterReferenceExpression, Type: System.Object) (Syntax: IdentifierName, 'o')
          Pattern: 
            IDeclarationPattern (Declared Symbol: System.String s) ([1] OperationKind.DeclarationPattern) (Syntax: DeclarationPattern, 'string s')
      Handler: 
        IBlockStatement (0 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
  Finally: 
    null
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // file.cs(11,26): warning CS0168: The variable 'e' is declared but never used
                //         catch (Exception e) when (o is string s)
                Diagnostic(ErrorCode.WRN_UnreferencedVar, "e").WithArguments("e").WithLocation(11, 26)
            };

            VerifyOperationTreeAndDiagnosticsForTest<TryStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void TryCatch_FinallyWithLocalDeclaration()
        {
            string source = @"
class C
{
    static void Main()
    {
        /*<bind>*/try
        {
        }
        finally
        {
            int i = 0;
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
ITryStatement ([0] OperationKind.TryStatement) (Syntax: TryStatement, 'try ... }') (Parent: BlockStatement)
  Body: 
    IBlockStatement (0 statements) ([0] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
  Catch clauses(0)
  Finally: 
    IBlockStatement (1 statements, 1 locals) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      Locals: Local_1: System.Int32 i
      IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'int i = 0;')
        IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'i = 0')
          Variables: Local_1: System.Int32 i
          Initializer: 
            IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= 0')
              ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0219: The variable 'i' is assigned but its value is never used
                //             int i = 0;
                Diagnostic(ErrorCode.WRN_UnreferencedVarAssg, "i").WithArguments("i").WithLocation(11, 17)
            };

            VerifyOperationTreeAndDiagnosticsForTest<TryStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void TryCatch_InvalidCaughtType()
        {
            string source = @"
class C
{
    static void Main()
    {
        try
        {
        }
        /*<bind>*/catch (int e)
        {
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
ICatchClause (Exception type: System.Int32) ([1] OperationKind.CatchClause, IsInvalid) (Syntax: CatchClause, 'catch (int  ... }') (Parent: TryStatement)
  Locals: Local_1: System.Int32 e
  ExceptionDeclarationOrExpression: 
    IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration, IsInvalid) (Syntax: CatchDeclaration, '(int e)')
      Variables: Local_1: System.Int32 e
      Initializer: 
        null
  Filter: 
    null
  Handler: 
    IBlockStatement (0 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
";
            var expectedDiagnostics = new DiagnosticDescription[] {
                // CS0155: The type caught or thrown must be derived from System.Exception
                //         /*<bind>*/catch (int e)
                Diagnostic(ErrorCode.ERR_BadExceptionType, "int").WithLocation(9, 26),
                // CS0168: The variable 'e' is declared but never used
                //         /*<bind>*/catch (int e)
                Diagnostic(ErrorCode.WRN_UnreferencedVar, "e").WithArguments("e").WithLocation(9, 30)
            };

            VerifyOperationTreeAndDiagnosticsForTest<CatchClauseSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void TryCatch_GetOperationForCatchClause()
        {
            string source = @"
class C
{
    static void Main()
    {
        try
        {
        }
        /*<bind>*/catch (System.IO.IOException e) when (e.Message != null)
        {
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
ICatchClause (Exception type: System.IO.IOException) ([1] OperationKind.CatchClause) (Syntax: CatchClause, 'catch (Syst ... }') (Parent: TryStatement)
  Locals: Local_1: System.IO.IOException e
  ExceptionDeclarationOrExpression: 
    IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: CatchDeclaration, '(System.IO. ... xception e)')
      Variables: Local_1: System.IO.IOException e
      Initializer: 
        null
  Filter: 
    IBinaryOperatorExpression (BinaryOperatorKind.NotEquals) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: NotEqualsExpression, 'e.Message != null')
      Left: 
        IPropertyReferenceExpression: System.String System.Exception.Message { get; } ([0] OperationKind.PropertyReferenceExpression, Type: System.String) (Syntax: SimpleMemberAccessExpression, 'e.Message')
          Instance Receiver: 
            ILocalReferenceExpression: e ([0] OperationKind.LocalReferenceExpression, Type: System.IO.IOException) (Syntax: IdentifierName, 'e')
      Right: 
        IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.String, Constant: null, IsImplicit) (Syntax: NullLiteralExpression, 'null')
          Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            ILiteralExpression ([0] OperationKind.LiteralExpression, Type: null, Constant: null) (Syntax: NullLiteralExpression, 'null')
  Handler: 
    IBlockStatement (0 statements) ([2] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<CatchClauseSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void TryCatch_GetOperationForCatchDeclaration()
        {
            string source = @"
class C
{
    static void Main()
    {
        try
        {
        }
        catch /*<bind>*/(System.IO.IOException e)/*</bind>*/ when (e.Message != null)
        {
        }
    }
}
";
            string expectedOperationTree = @"
IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: CatchDeclaration, '(System.IO. ... xception e)') (Parent: CatchClause)
  Variables: Local_1: System.IO.IOException e
  Initializer: 
    null
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<CatchDeclarationSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void TryCatch_GetOperationForCatchFilterClause()
        {
            string source = @"
using System;

class C
{
    static void M(string s)
    {
        try
        {
        }
        catch (Exception) /*<bind>*/when (s != null)/*</bind>*/
        {
        }
    }
}
";
            // GetOperation returns null for CatchFilterClauseSyntax
            Assert.Null(GetOperationTreeForTest<CatchFilterClauseSyntax>(source));
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void TryCatch_GetOperationForCatchFilterClauseExpression()
        {
            string source = @"
using System;

class C
{
    static void M(string s)
    {
        try
        {
        }
        catch (Exception) when (/*<bind>*/s != null/*</bind>*/)
        {
        }
    }
}
";
            string expectedOperationTree = @"
IBinaryOperatorExpression (BinaryOperatorKind.NotEquals) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: NotEqualsExpression, 's != null') (Parent: CatchClause)
  Left: 
    IParameterReferenceExpression: s ([0] OperationKind.ParameterReferenceExpression, Type: System.String) (Syntax: IdentifierName, 's')
  Right: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.String, Constant: null, IsImplicit) (Syntax: NullLiteralExpression, 'null')
      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: True, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: null, Constant: null) (Syntax: NullLiteralExpression, 'null')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<BinaryExpressionSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact]
        public void TryCatch_GetOperationForFinallyClause()
        {
            string source = @"
using System;

class C
{
    static void M(string s)
    {
        try
        {
        }
        /*<bind>*/finally
        {
            Console.WriteLine(s);
        }/*</bind>*/
    }
}
";
            // GetOperation returns null for FinallyClauseSyntax
            Assert.Null(GetOperationTreeForTest<FinallyClauseSyntax>(source));
        }
    }
}
